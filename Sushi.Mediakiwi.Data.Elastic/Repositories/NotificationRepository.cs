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
        public NotificationRepository(Nest.ConnectionSettings connectionSettings, string dataStreamName)
        {   
            // create client
            _client = new Nest.ElasticClient(connectionSettings);
            _dataStreamName = dataStreamName;
        }
        
        public Data.Notification Save(Data.Notification notification)
        {
            var elasticNotification = new Notification(notification);
            var response = _client.Index(elasticNotification, i => i.Index(_dataStreamName));
            elasticNotification.ElasticId = new ElasticId(response.Id, response.Index);
            return elasticNotification;
        }

        public async Task<Data.Notification> SaveAsync(Data.Notification notification)
        {
            var elasticNotification = new Notification(notification);
            var response = await _client.IndexAsync(elasticNotification, i => i.Index(_dataStreamName));
            elasticNotification.ElasticId = new ElasticId(response.Id, response.Index);
            return elasticNotification;
        }

        
    }
}
