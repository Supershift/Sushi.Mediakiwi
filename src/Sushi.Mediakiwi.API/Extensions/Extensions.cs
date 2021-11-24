using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Sushi.Mediakiwi.API.Services;
using Swashbuckle.Swagger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Sushi.Mediakiwi.API.Extensions
{
    public static class Extensions
    {

        public static HttpContext Clone(this HttpContext httpContext, bool copyBody)
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

            if (copyBody)
            {
                // We need to buffer first, otherwise the body won't be copied
                // Won't work if the body stream was accessed already without calling EnableBuffering() first or without leaveOpen
                httpContext.Request.EnableBuffering();
                httpContext.Request.Body.Seek(0, SeekOrigin.Begin);
                requestFeature.Body = existingRequestFeature.Body;
            }

            var features = new FeatureCollection();
            features.Set<IHttpRequestFeature>(requestFeature);
            // Unless we need the response we can ignore it...
            features.Set<IHttpResponseFeature>(new HttpResponseFeature());
            features.Set<IHttpResponseBodyFeature>(new StreamResponseBodyFeature(Stream.Null));

            var newContext = new DefaultHttpContext(features);

            if (copyBody)
            {
                // Rewind for any future use...
                httpContext.Request.Body.Seek(0, SeekOrigin.Begin);
            }

            // Can happen if the body was not copied
            if (httpContext.Request.HasFormContentType && httpContext.Request.Form.Count != newContext.Request.Form.Count)
            {
                newContext.Request.Form = new FormCollection(httpContext.Request.Form.ToDictionary(f => f.Key, f => f.Value));
            }

            return newContext;
        }

        public static IApplicationBuilder UseMediakiwiApi(this IApplicationBuilder app)
        {
            app.UseSwagger(options=> {
                options.RouteTemplate = "mkapi/swagger/{documentname}/swagger.json";
                

                options.PreSerializeFilters.Add((swaggerDoc, httpReq) => {
                    IDictionary<string, PathItem> paths = new Dictionary<string, PathItem>();
                    OpenApiPaths correctedPaths = swaggerDoc.Paths;

                    foreach (var item in correctedPaths.Where(x => x.Key.Contains("mkapi/", StringComparison.InvariantCulture) == false))
                    {
                        swaggerDoc.Paths.Remove(item.Key);
                    }
                });
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
            services.AddSingleton<IUserService, UserService>();
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSwaggerGen(options =>
            {
                var filePath = Path.Combine(AppContext.BaseDirectory, "Sushi.Mediakiwi.API.xml");
                options.IncludeXmlComments(filePath);
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
            });

            services.AddAuthentication(option =>
            {
                option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateLifetime = false,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = Common.API_AUTHENTICATION_ISSUER,
                    ValidAudience = Common.API_AUTHENTICATION_AUDIENCE,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Common.API_AUTHENTICATION_KEY))
                };
            });
        }
    }
}
