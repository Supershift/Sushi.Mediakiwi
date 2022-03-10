using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sushi.Mediakiwi.Data.Elastic;
using System;
using System.Text.Json;
using Sushi.Mediakiwi.API.Extensions;
using Sushi.MailTemplate.SendGrid;

namespace Sushi.Mediakiwi.Demonstration
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
            services.AddHttpContextAccessor();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            services.AddControllersWithViews(options =>
            {
            }).AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.IgnoreNullValues = true;
                options.JsonSerializerOptions.IgnoreReadOnlyProperties = true;
                options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            });

            services.AddMediakiwi();
            services.AddMediakiwiApi();
            //services.AddMediakiwiGlobalListSetting<string>("googleSheetsUrl", "Google sheets URL", "The URL of the Google sheets doc representing this list");


            var elasticSettings = new Nest.ConnectionSettings(new Uri(Configuration["ElasticUrl"]))
                .BasicAuthentication(Configuration["ElasticUsername"], Configuration["ElasticPassword"])
                .ThrowExceptions(true);

            var elasticClient = new Nest.ElasticClient(elasticSettings);

            services.AddElasticNotifications(elasticClient);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseHttpsRedirection();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
             
            }
            app.UseStaticFiles();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            // swagger
            app.UseSwagger();
            app.UseSwaggerUI();

            string[] excludePaths = new string[] { "/api/custom", "/myfiles", "/mkapi", "/api" };
            
            app.UseMediakiwi(excludePaths);
            app.UseMediakiwiApi();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            // Set Azure Storage account for storing mail queues
            var emailStorageAccount = Configuration["EmailStorageAccount"];
            
            // Set Azure storage account container for storing mail queues
            var emailBlobContainer = Configuration["EmailBlobContainer"];

            // Set Azure storage queue name for mail
            var emailQueueName = Configuration["EmailQueueName"];

            // Set Sendgrid API key for sending e-mails
            var sendGridApiKey = Configuration["SendGridAPIKey"];

            // Hook up the Mailer 
            if (
                string.IsNullOrWhiteSpace(emailStorageAccount) == false
                && string.IsNullOrWhiteSpace(emailBlobContainer) == false
                && string.IsNullOrWhiteSpace(emailQueueName) == false
                && string.IsNullOrWhiteSpace(sendGridApiKey) == false)
            {
                _ = new Mailer(emailStorageAccount, emailBlobContainer, emailQueueName, sendGridApiKey);
            }
        }
    }
}
