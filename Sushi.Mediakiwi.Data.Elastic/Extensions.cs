using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sushi.Mediakiwi.Data.Elastic
{
    public static class Extensions
    {
        /// <summary>
        /// Adds a singleton instance for <see cref="Data.Repositories.INotificationRepository" /> to the service collection, which uses Elastic as backingstore.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="client"></param>
        /// <param name="dataStreamName"></param>
        public static void AddElasticNotifications(this IServiceCollection services, Nest.IElasticClient client, string dataStreamName = "notifications")
        {
            var repository = new Repositories.NotificationRepository(client, dataStreamName);
            services.AddSingleton<Data.Repositories.INotificationRepository>(repository);

            // set the repository on static Notification for backwards compatability
            Data.Notification.Repository = repository;
        }
    }
}
