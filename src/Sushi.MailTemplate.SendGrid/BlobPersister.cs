using Azure.Storage.Blobs;
using Microsoft.Extensions.Azure;
using System.Threading.Tasks;

namespace Sushi.MailTemplate.SendGrid
{
    public class BlobPersister
    {
        private readonly BlobServiceClient _client;

        public BlobPersister(IAzureClientFactory<BlobServiceClient> clientFactory)
        {
            _client = clientFactory.CreateClient(ServiceCollectionExtensions.StorageClientName);
        }

        public async Task<BlobClient> GetBlobClientAsync(string containerName, string blobName)
        {
            var container = _client.GetBlobContainerClient(containerName);
            await container.CreateIfNotExistsAsync();
            
            var blobClient = container.GetBlobClient(blobName);
            return blobClient;
        }
    }
}
