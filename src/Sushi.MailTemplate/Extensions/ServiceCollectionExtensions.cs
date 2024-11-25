using Microsoft.Extensions.DependencyInjection;

namespace Sushi.MailTemplate.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddSushiMailTemplate(this IServiceCollection services)
        {
            services.AddTransient<Data.DefaultValuePlaceholderRepository>();
            services.AddTransient<Data.MailTemplateListRepository>();
            services.AddTransient<Data.MailTemplateRepository>();

            services.AddTransient<Logic.PlaceholderLogic>();
            services.AddTransient<MailTemplateHelper>();
        }
    }
}
