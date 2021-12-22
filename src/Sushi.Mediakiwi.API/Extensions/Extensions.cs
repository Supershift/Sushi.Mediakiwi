using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using Microsoft.OpenApi.Models;
using Sushi.Mediakiwi.API.Authentication;
using Sushi.Mediakiwi.API.Filters;
using Sushi.Mediakiwi.API.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Sushi.Mediakiwi.API.Extensions
{
    public static class Extensions
    {
        public static Framework.Api.MediakiwiPostRequest GetPostRequest(this ICollection<Transport.FormMap> maps)
        {
            Framework.Api.MediakiwiPostRequest result = new Framework.Api.MediakiwiPostRequest();
            Dictionary<string, object> postForm = new Dictionary<string, object>();

            if (maps?.Count > 0)
            {
                foreach (var map in maps)
                {
                    foreach (var element in map.Fields)
                    {
                        postForm.Add(element.PropertyName, element.Value);
                    }
                }
            }

            result.FormFields = postForm;
            return result;
        }

        public static HttpContext Clone(this HttpContext httpContext)
        {
            var existingRequestFeature = httpContext.Features.Get<IHttpRequestFeature>();

            var requestHeaders = new Dictionary<string, StringValues>(existingRequestFeature.Headers.Count, StringComparer.OrdinalIgnoreCase);
            foreach (var header in existingRequestFeature.Headers)
            {
                requestHeaders[header.Key] = header.Value;
            }

            var requestFeature = new HttpRequestFeature
            {
                Protocol = existingRequestFeature.Protocol,
                Method = existingRequestFeature.Method,
                Scheme = existingRequestFeature.Scheme,
                Path = existingRequestFeature.Path,
                PathBase = existingRequestFeature.PathBase,
                QueryString = existingRequestFeature.QueryString,
                RawTarget = existingRequestFeature.RawTarget,
                Headers = new HeaderDictionary(requestHeaders),
            };

            var features = new FeatureCollection();
            features.Set<IHttpRequestFeature>(requestFeature);
            // Unless we need the response we can ignore it...
            features.Set<IHttpResponseFeature>(new HttpResponseFeature());
            features.Set<IHttpResponseBodyFeature>(new StreamResponseBodyFeature(Stream.Null));

            var newContext = new DefaultHttpContext(features);

            // Can happen if the body was not copied
            if (httpContext.Request.HasFormContentType && httpContext.Request.Form.Count != newContext.Request.Form.Count)
            {
                newContext.Request.Form = new FormCollection(httpContext.Request.Form.ToDictionary(f => f.Key, f => f.Value));
            }

            return newContext;
        }

        public static IApplicationBuilder UseMediakiwiApi(this IApplicationBuilder app)
        {
            if (CommonConfiguration.IS_LOCAL_DEVELOPMENT)
            {
                app.UseMiddleware<Middleware.ApiCorsMiddleware>();
            }

            app.UseCors(Common.API_CORS_POLICY);

            app.UseSwagger(options => {
                options.RouteTemplate = "mkapi/swagger/{documentname}/swagger.json";
            });

            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("v0.1/swagger.json", "Mediakiwi API V0.1");
                options.RoutePrefix = "mkapi/swagger";
            });

            return app;
        }

        public static void AddMediakiwiApi(this IServiceCollection services)
        {
            var config = services.BuildServiceProvider().GetService<IConfiguration>();
            if (config != null)
            {
                Data.Configuration.WimServerConfiguration.LoadJsonConfig(config);
            }
            else 
            {
                Console.WriteLine("No WimServerConfiguration available, was AddMediakiwi added first ?");
            }

            services.AddCors(options =>
            {
                options.AddPolicy(name: Common.API_CORS_POLICY,
                                     builder =>
                                     {
                                         builder.AllowCredentials();
                                         builder.AllowAnyMethod();
                                         builder.WithExposedHeaders(new string[] { "set-cookie" });
                                         builder.AllowAnyHeader();

                                         if (CommonConfiguration.IS_LOCAL_DEVELOPMENT)
                                         {
                                             builder.SetIsOriginAllowed(origin => new Uri(origin).Host == "localhost");
                                         }
                                     });

            });

            // Add API services
            services.AddSingleton<IUserService, UserService>();
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<IContentService, ContentService>();
            services.AddControllers(o =>
            {
                o.Conventions.Add(new SwaggerSchemaFilter());
            });

            // Add swagger
            services.AddSwaggerGen(options =>
            {
                var filePath = Path.Combine(AppContext.BaseDirectory, $"{Common.API_ASSEMBLY_NAME}.xml");
                if (File.Exists(filePath))
                {
                    options.IncludeXmlComments(filePath);
                }
                options.EnableAnnotations(true, true);
                options.SwaggerDoc("v0.1", new OpenApiInfo
                {
                    Version = "v0.1",
                    Title = "Mediakiwi API",
                    Description = "An ASP.NET Core Web API for managing the Mediakiwi CMS",
                    Contact = new OpenApiContact() 
                    { 
                        Email = "mark.rienstra@supershift.nl",
                        Name = "Supershift",
                        Url = new Uri("https://www.supershift.nl")
                    }, 
                });
                options.OperationFilter<SwaggerUrlHeaderFilter>();
                options.OperationFilter<SwaggerCookieFilter>();
                options.DocumentFilter<SwaggerDocumentFilter>();
                //options.SchemaFilter<SwaggerSchemaFilter>();
            });

            // Add Cookie validator and authentication
            services.AddScoped<MediakiwiCookieValidator>();
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.Cookie.Name = Common.API_COOKIE_KEY;
                    options.EventsType = typeof(MediakiwiCookieValidator);
                    options.Cookie.IsEssential = true;
                    options.Cookie.HttpOnly = false;
                    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                    options.Cookie.SameSite = SameSiteMode.None;
                    options.Cookie.Path = "/";
                });
        }
    }
}
