using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Sushi.Mediakiwi.Authentication;
using Sushi.Mediakiwi.Framework;
using Sushi.Mediakiwi.PageModules.ExportPage;

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
            services.AddSingleton<Interfaces.ITrailExtension, Logic.WikiTrailExtension>();

            services.AddSingleton(s =>
            {
                // todo: inject config using options pattern
                string tenant = Data.Configuration.WimServerConfiguration.Instance?.Authentication?.Aad?.Tenant;
                var stsDiscoveryEndpoint = $@"https://login.microsoftonline.com/{tenant}/.well-known/openid-configuration";
                var result = new ConfigurationManager<OpenIdConnectConfiguration>(stsDiscoveryEndpoint, new OpenIdConnectConfigurationRetriever());
                result.AutomaticRefreshInterval = System.TimeSpan.FromHours(1);
                return result;
            });
        }
    }
}
