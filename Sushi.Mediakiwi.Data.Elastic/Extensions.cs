using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sushi.Mediakiwi.Data.Elastic
{
    public static class Extensions
    {
        public static void AddMediakiwi(this IServiceCollection services, Nest.IElasticClient client, string dataStreamName = "notifications")
        {
            var repository = new Repositories.NotificationRepository(client, dataStreamName);
            services.AddSingleton<Data.Repositories.INotificationRepository>(repository);

            // set the repository on static Notification for backwards compatability
            Data.Notification.Repository = repository;
        }
    }
}
