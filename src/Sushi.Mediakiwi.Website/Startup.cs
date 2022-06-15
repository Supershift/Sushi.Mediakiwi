using Sushi.Mediakiwi.Website.Models;
using Sushi.Mediakiwi.Headless;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OwaspHeaders.Core.Extensions;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using Sushi.Mediakiwi.Website.RazorAdditions;
using System.Net.Http.Headers;
using System;
using Sushi.Mediakiwi.Headless.Config;
using Sushi.Mediakiwi.Headless.SectionHelper;

namespace Sushi.Mediakiwi.Website
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        private ApplicationSettings appSettings { get; set; }

        public Startup(IConfiguration config)
        {
            Configuration = config;

            appSettings = new ApplicationSettings();
            Configuration.Bind(appSettings);
        }

        private bool EnableCompression = true;


        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            if (appSettings.EnableResponseCaching)
            {
                services.AddResponseCaching(options =>
                {
                    options.MaximumBodySize = 1024 * 100;
                    options.UseCaseSensitivePaths = false;
                });
            }

            services.AddMemoryCache();

            // Add strongly typed Config
            services.AddSingleton(appSettings);
            services.AddSingleton<ISushiApplicationSettings>(appSettings);

            // Enable Compression ?
            if (EnableCompression)
            {

                // Configure Compression level
                services.Configure<GzipCompressionProviderOptions>(options => options.Level = CompressionLevel.Optimal);
                services.Configure<BrotliCompressionProviderOptions>(options => options.Level = CompressionLevel.Optimal);

                services.AddResponseCompression(options =>
                {
                    List<string> MimeTypes = new List<string>()
                    {
                     // General
                     "text/plain",
                     "text/css",
                     "font/woff2",
                     "font/woff",
                     "application/javascript",
                     "image/x-icon",
                     "image/png",
                     "image/jpeg",
                     "image/webp",
                     "text/html"
                     };

                    options.EnableForHttps = true;
                    options.MimeTypes = MimeTypes;
                    options.Providers.Add<GzipCompressionProvider>();
                    options.Providers.Add<BrotliCompressionProvider>();
                });
            }

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.Configure<RouteOptions>(options =>
            {
                options.LowercaseUrls = true;
                options.LowercaseQueryStrings = true;
            });

            services.Configure<IISServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
                options.AutomaticAuthentication = true;
            });

            services.AddRazorPages(options => {
                options.Conventions.AddAreaPageRoute("Internals", "/SitemapXML", "sitemap.xml");
                // You can also configure [AllowAnonymous] for pages/folders/areas
                //options.Conventions.AllowAnonymousToAreaPage(".auth", "me");

            });

            if (appSettings.EnableResponseCaching)
            {
                services.AddMvc(options =>
                {
                    options.CacheProfiles.Add("Default", new Microsoft.AspNetCore.Mvc.CacheProfile()
                    {
                        Duration = 120,
                        Location = Microsoft.AspNetCore.Mvc.ResponseCacheLocation.Any,
                        VaryByHeader = "User-Agent,Accept-Encoding,x-mediakiwi-fullrequesturl",
                    });
                });
            }
            else
            {
                services.AddMvc(options =>
                {
                    options.CacheProfiles.Add("Default", new Microsoft.AspNetCore.Mvc.CacheProfile()
                    {
                        NoStore = true,
                        Location = Microsoft.AspNetCore.Mvc.ResponseCacheLocation.None,
                    });
                });
            }

            //services.AddAuthentication(options =>
            //{
            //    options.DefaultScheme = "Cookies";
            //    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            //})
            //    .AddCookie("Cookies")
            //    //.AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
            //    //{
            //    //    options.SignInScheme = "Cookies";
            //    //    options.RequireHttpsMetadata = false;
            //    //    options.Authority = "http://localhost:5000/";
            //    //    options.ClientId = "mvcclient";
            //    //    options.SaveTokens = true;
            //    //})
            //    ;
            ////services.AddServerSideBlazor(options =>
            ////{
            ////    options.DetailedErrors = true;
            ////});

            services.AddClients();
            services.AddMediaKiwiContentService();
            services.AddTransient<ITagHelperComponent, ScriptTagHelperComponent>();
            services.AddTransient<ITagHelperComponent, MetaTagHelperComponent>();
            services.AddSectionHelper();
            services.AddHttpContextAccessor();
            services.AddApplicationInsightsTelemetry();

        }

        static OwaspHeaders.Core.Models.SecureHeadersMiddlewareConfiguration BuilSecurityConfiguration()
        {
            return SecureHeadersMiddlewareBuilder
                .CreateBuilder()
                .UseHsts()
                .UseXFrameOptions(OwaspHeaders.Core.Enums.XFrameOptions.Sameorigin)
                .UseXSSProtection()
                .UseContentTypeOptions()
                //.UseContentDefaultSecurityPolicy()
                .UsePermittedCrossDomainPolicies()
                .UseReferrerPolicy()
                .RemovePoweredByHeader()
                .Build();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHttpContextAccessor contextAccessor, IConfiguration configuration)
        {
            app.UseHsts();

            if (appSettings.EnableResponseCaching)
                app.UseResponseCaching();

            // use security headers
            //app.UseSecureHeadersMiddleware(BuilSecurityConfiguration());

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Set static file options
            var staticFileOptions = new StaticFileOptions();

            // When static file caching is enabled, use it
            if (appSettings.EnableStaticFileCaching)
            {
                staticFileOptions = new StaticFileOptions()
                {
                    OnPrepareResponse = context => context.Context.Response.GetTypedHeaders()
                        .CacheControl = new Microsoft.Net.Http.Headers.CacheControlHeaderValue()
                        {
                            Public = true,
                            MaxAge = TimeSpan.FromDays(365) // 1 year
                        }
                };
            }

            app.UseStaticFiles(staticFileOptions);

            // URL Rewrite options
            var options = new RewriteOptions();

            // URL Rewrite rules from XML
            if (File.Exists("URLRewrites.xml"))
            {
                using (StreamReader iisUrlRewriteStreamReader = File.OpenText("URLRewrites.xml"))
                    options.AddIISUrlRewrite(iisUrlRewriteStreamReader);
            }

            // URL Rewrite for MediaKiwi
            options.Add(new MediaKiwiRewriteRule(app.ApplicationServices));

            app.UseRewriter(options);

            // Use compression ?
            if (EnableCompression)
                app.UseResponseCompression();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                //endpoints.MapBlazorHub();
                endpoints.MapControllers();
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
