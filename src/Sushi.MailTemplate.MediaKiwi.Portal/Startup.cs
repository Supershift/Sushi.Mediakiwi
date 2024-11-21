using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Sushi.Mediakiwi;
using Sushi.MailTemplate.Extensions;
using Sushi.MailTemplate.SendGrid;
using Sushi.Mediakiwi.MicroORM;

namespace Sushi.MailTemplate.MediaKiwi.Portal
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews(options => {
                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
                options.Filters.Add(new AuthorizeFilter(policy));
            });

            string databaseConnectionString = Configuration.GetConnectionString("datastore");
            string azureConnectionString = Configuration.GetConnectionString("azurestore");

            // add sushi micro orm            
            services.AddMicroORM(databaseConnectionString);

            // add mail templating
            services.AddSushiMailTemplate();

            // add sendgrid
            services.AddSushiMailTemplateSendgrid(Configuration.GetSection("SendGrid"), azureConnectionString);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

            }
            app.UseStaticFiles();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseMediakiwi();
        }
    }
}
