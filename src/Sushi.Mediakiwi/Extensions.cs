using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Sushi.Mediakiwi.Authentication;
using Sushi.Mediakiwi.Framework;
using Sushi.Mediakiwi.Identity;
using Sushi.Mediakiwi.Interfaces;
using Sushi.Mediakiwi.PageModules.ExportPage;
using System;

namespace Sushi.Mediakiwi
{
    public static class Extensions
    {
        /// <summary>
        /// Adds the MediaKiwi middleware to the pipeline
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseMediakiwi(this IApplicationBuilder app)
        {
            return app.UseMediakiwi(null);
        }

        /// <summary>
        /// Adds the MediaKiwi middleware to the pipeline
        /// </summary>
        /// <param name="app"></param>
        /// <param name="excludePaths">If a request is made to one of these paths, the MediaKiwi middleware is not executed. 
        /// Paths must start with '/' and partial matching is supported, e.g. if '/api/custom' is supplied, a request to '/api/custom/myFolder/myResource' does not trigger MediaKiwi.</param>
        /// <returns></returns>
        public static IApplicationBuilder UseMediakiwi(this IApplicationBuilder app, string[] excludePaths)
        {
            return app.UseWhen((context) =>
            {
               if (excludePaths != null)
               {
                   foreach (var path in excludePaths)
                   {
                       var isMatch = context.Request.Path.StartsWithSegments(path, System.StringComparison.InvariantCultureIgnoreCase);
                       if (isMatch)
                           return false;
                   }
               }
               return true;
           },
           a => a.UseMiddleware<Portal>()
           );
        }

        public static void AddMediakiwi(this IServiceCollection services)
        {
            services.AddAuthentication()
                .AddScheme<MediaKiwiAuthenticationOptions, MediaKiwiAuthenticationHandler>(AuthenticationDefaults.AuthenticationScheme, null);
            
            services.AddSingleton<IPageModule, ExportPageModule>();
            services.AddSingleton<ITrailExtension, Logic.WikiTrailExtension>();
        }


        /// <summary>
        /// Add the default mediakiwi profile manager, for use with the <c>wim_profiles</c> database
        /// </summary>
        /// <param name="services">The current service collection</param>
        public static void AddMediakiwiProfileManager(this IServiceCollection services)
        {
            ProfileManager manager = new ProfileManager();
            AddMediakiwiProfileManager(services, manager);
        }

        /// <summary>
        /// Add a custom mediakiwi profile manager
        /// </summary>
        /// <param name="services">The current service collection</param>
        /// <param name="profileManager">The custom <c>IProfileManager</c> implementation</param>
        public static void AddMediakiwiProfileManager(this IServiceCollection services, MediaKiwiProfileManager profileManager)
        {
            // Add Cookie validator and authentication
            services.AddScoped(factory => { return profileManager; });

            services.AddAuthentication(profileManager.SchemeName)
                .AddCookie(options =>
                {
                    options.Cookie.Name = profileManager.CookieName;
                    options.EventsType = profileManager.GetType();
                    options.Cookie.IsEssential = true;
                    options.Cookie.HttpOnly = false;
                    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                    options.Cookie.SameSite = SameSiteMode.None;
                    options.Cookie.Path = "/";
                });
        }
    }
}
