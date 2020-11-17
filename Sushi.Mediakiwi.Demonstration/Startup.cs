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
            services.AddAuthentication().AddScheme<AuthenticationSchemeOptions, PromptHandler>("BasicAuthentication", null);
     
            // configure DI for application services
            services.AddScoped<IUserService, UserService>();
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

            if (configuration.GetValue<bool>("authentication"))
            {
                UserService.Add(configuration.GetValue<string>("basic_user"), configuration.GetValue<string>("basic_password"));

                app.Use(async (context, next) => {
                    var authenticationresult = await context.AuthenticateAsync("BasicAuthentication");
                    if (!authenticationresult.Succeeded)
                        await context.Response.WriteAsync("unauthenticated");
                    else
                        await next.Invoke();
                });
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
