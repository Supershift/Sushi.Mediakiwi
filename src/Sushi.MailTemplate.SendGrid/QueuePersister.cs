using Azure.Storage.Queues;
using Microsoft.Extensions.Azure;
using System.Threading.Tasks;

namespace Sushi.MailTemplate.SendGrid
{
    public class QueuePersister
    {
        private readonly QueueServiceClient _client;

        public QueuePersister(IAzureClientFactory<QueueServiceClient> clientFactory)
        {
            _client = clientFactory.CreateClient(ServiceCollectionExtensions.StorageClientName);
        }

        public string ConnectionString { get; protected set; }


        public async Task<QueueClient> GetQueueAsync(string name)
        {
            var queue = _client.GetQueueClient(name);
            await queue.CreateIfNotExistsAsync();

            return queue;
        }
    }
}