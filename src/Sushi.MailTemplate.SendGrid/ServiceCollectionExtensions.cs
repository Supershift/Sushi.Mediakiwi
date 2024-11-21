using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Sushi.MailTemplate.SendGrid
{
    public static class ServiceCollectionExtensions
    {
        internal static readonly string StorageClientName = "Sushi.MailTemplate.SendGrid";
        
        public static void AddSushiMailTemplateSendgrid(this IServiceCollection services, Action<SendGridMailerOptions> configureOptions)
        {
            services.Configure(configureOptions);
            var options = new SendGridMailerOptions();
            configureOptions(options);
            AddSushiMailTemplateSendgrid(services, options.AzureStorageAccount);
        }

        public static void AddSushiMailTemplateSendgrid(this IServiceCollection services, IConfiguration namedConfigurationSection)
        {
            var options = new SendGridMailerOptions();
            namedConfigurationSection.Bind(options);
            AddSushiMailTemplateSendgrid(services, namedConfigurationSection, options.AzureStorageAccount);
        }

        public static void AddSushiMailTemplateSendgrid(this IServiceCollection services, IConfiguration namedConfigurationSection, string azureStorageAccount)
        {
            services.Configure<SendGridMailerOptions>(namedConfigurationSection);
            
            AddSushiMailTemplateSendgrid(services, azureStorageAccount);
        }

        private static void AddSushiMailTemplateSendgrid(IServiceCollection services, string azureStorageAccount)
        {
            if(string.IsNullOrEmpty(azureStorageAccount))
            {
                throw new ArgumentNullException(nameof(azureStorageAccount), $"Setting {nameof(SendGridMailerOptions.AzureStorageAccount)} on {nameof(SendGridMailerOptions)} is required");
            }
            
            services.AddTransient<Mailer>();
            services.AddTransient<Logic.ISendPreviewEmailEventHandler, Mailer>();
            services.AddTransient<BlobPersister>();
            services.AddTransient<QueuePersister>();
            services.AddAzureClients(builder =>
            {
                builder.AddBlobServiceClient(azureStorageAccount).WithName(StorageClientName);
                builder.AddQueueServiceClient(azureStorageAccount).WithName(StorageClientName);
            });
        }
    }
}
