using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Sushi.Mediakiwi.Authentication;
using Sushi.Mediakiwi.Framework;
using Sushi.Mediakiwi.PageModules.ExportPage;

namespace Sushi.Mediakiwi
{
    public static class Extensions
    {
        public static IApplicationBuilder UseMediakiwi(this IApplicationBuilder app)
        {
            return app.UseMiddleware<Portal>();
        }

        public static void AddMediakiwi(this IServiceCollection services)
        {
            services.AddAuthentication()
                .AddScheme<MediaKiwiAuthenticationOptions, MediaKiwiAuthenticationHandler>(AuthenticationDefaults.AuthenticationScheme, null);
            
            services.AddSingleton<IPageModule, ExportPageModule>();
        }
    }
}
