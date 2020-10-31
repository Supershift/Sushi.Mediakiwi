using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sushi.MicroORM;

namespace Sushi.Mediakiwi.Demonstration
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IConfiguration configuration)
        {
            //setx ASPNETCORE_ENVIRONMENT "Development"
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseStaticFiles();

            // All requests ending in .html will follow this branch.
            app.MapWhen(
                context => context.Request.Path.ToString().EndsWith(
                    configuration.GetValue<string>("mediakiwi:portal_path")),
                appBranch => {
                    appBranch.UseMediakiwi();
                });
        }
    }
}
