using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sushi.Mediakiwi.Headless.BasicAuthentication;
using Sushi.MicroORM;

namespace Sushi.Mediakiwi.Demonstration
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<LoginPrompt>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IConfiguration configuration)
        {

            app.UseRouting();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
             
            }
            app.UseStaticFiles();

            if (env.IsStaging() || env.IsDevelopment())
            {
                if (configuration.GetValue<bool>("authentication"))
                {
                    app.UseLoginPrompt(credential => {
                        credential.Username = configuration.GetValue<string>("basic_user");
                        credential.Password = configuration.GetValue<string>("basic_password");
                    });
                }
            }

            app.MapWhen(
                context => context.Request.Path.ToString().EndsWith(
                    configuration.GetValue<string>("mediakiwi:portal_path")),
                appBranch => {
                    appBranch.UseMediakiwi();
                });
        }
    }
}
