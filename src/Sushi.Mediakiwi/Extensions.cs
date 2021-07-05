using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
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
            services.AddSingleton<IPageModule, ExportPageModule>();
        }
    }
}
