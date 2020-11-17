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
using Sushi.Mediakiwi.Headless.BasicPrompt;
using Sushi.MicroORM;

namespace Sushi.Mediakiwi.Demonstration
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            // configure basic authentication 
            services.AddAuthentication("BasicAuthentication")
                .AddScheme<AuthenticationSchemeOptions, PromptHandler>("BasicAuthentication", null);

     
            // configure DI for application services
            services.AddScoped<IUserService, UserService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IConfiguration configuration)
        {
            UserService.Add(configuration.GetValue<string>("basic_user"), configuration.GetValue<string>("basic_password"));

            if (env.IsDevelopment())
            {
                //app.UseAuthentication();
                app.UseDeveloperExceptionPage();
            }
            app.UseStaticFiles();

            app.MapWhen(
                context => context.Request.Path.ToString().EndsWith(
                    configuration.GetValue<string>("mediakiwi:portal_path")),
                appBranch => {
                    appBranch.UseAuthentication();
                    appBranch.UseMediakiwi();
                });
        }
    }
}
