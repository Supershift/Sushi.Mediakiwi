using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Text.Json;
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
