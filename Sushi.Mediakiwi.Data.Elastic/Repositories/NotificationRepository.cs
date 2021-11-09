using Sushi.Mediakiwi.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Data.Elastic.Repositories
{
    /// <summary>
    /// Provides methods to store and retrieve instances of <see cref="Notification"/> from a Sql database.
    /// </summary>
    public class NotificationRepository : INotificationRepository
    {
        private Nest.IElasticClient _client;
        private string _dataStreamName;
        public NotificationRepository(Nest.IElasticClient client, string dataStreamName)
        {
            _client = client;
            _dataStreamName = dataStreamName;
        }
        
        public Notification Save(Notification notification)
        {
            var response = _client.Index(notification, i => i.Index(_dataStreamName));
            notification.ID = response.Id;
            return notification;
        }

        public async Task<Notification> SaveAsync(Notification notification)
        {
            var response = await _client.IndexAsync(notification, i => i.Index(_dataStreamName));
            notification.ID = response.Id;
            return notification;
        }
    }
}
