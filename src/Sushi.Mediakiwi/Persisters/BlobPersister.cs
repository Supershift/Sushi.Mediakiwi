using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Persisters
{
    public class BlobPersister
    {
        public BlobPersister() : this(Data.Common.GetConnection("azurestore"))
        {

        }

        public BlobPersister(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public string ConnectionString
        {
            get; protected set;
        }

        static protected string ContainerCachePrefix = Guid.NewGuid().ToString();
        static protected ConcurrentDictionary<string, bool> Cache = new ConcurrentDictionary<string, bool>();

        public async Task<BlobClient> GetContainerAsync(string name)
        {
            var storageAccount = new BlobContainerClient(ConnectionString, name);
            // Create the blob client.
            var blobClient = storageAccount.GetBlobClient(name);
            return blobClient;
        }

        public async Task<bool> ExistsAsync(string containerName, string blobName)
        {
            var blobReference = await GetBlockBlobReferenceAsync(containerName, blobName).ConfigureAwait(false);
            return await blobReference.ExistsAsync().ConfigureAwait(false);
        }

        public async Task<BlobClient> GetBlockBlobReferenceAsync(string blobName)
        {
            if (string.IsNullOrWhiteSpace(Data.Configuration.WimServerConfiguration.Instance?.Azure_Image_Container) == false)
            {
                string containerName = Data.Configuration.WimServerConfiguration.Instance?.Azure_Image_Container;
                try
                {
                    return await GetBlockBlobReferenceAsync(containerName, blobName).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    await Data.Notification.InsertOneAsync(nameof(BlobPersister), ex).ConfigureAwait(false);
                    return null;
                }
            }
            else
            {
                await Data.Notification.InsertOneAsync(nameof(BlobPersister), "Azure_Image_Container is empty").ConfigureAwait(false);
                return null;
            }
        }

        public async Task<BlobClient> GetBlockBlobReferenceAsync(string containerName, string blobName)
        {
            var storageAccount = new BlobContainerClient(ConnectionString, containerName);
            // Create the blob client.
            var blobClient = storageAccount.GetBlobClient(blobName);
            return blobClient;
        }

        public async Task<BlobClient> UploadAsync(System.IO.Stream stream, string container, string blobName, string contentType = null)
        {
            var info = new BlobUploadOptions();
            if (!string.IsNullOrWhiteSpace(contentType))
            {
                info.HttpHeaders = new BlobHttpHeaders();
                info.HttpHeaders.ContentType = contentType;
            }

            var blob = await GetBlockBlobReferenceAsync(container, blobName).ConfigureAwait(false);
            await blob.UploadAsync(stream, info).ConfigureAwait(false);
            return blob;
        }

        public string GetSasTokenLink(string containerName)
        {
            var storageAccount = new BlobContainerClient(ConnectionString, containerName);

            var uri = storageAccount.GenerateSasUri(BlobContainerSasPermissions.Read, DateTimeOffset.UtcNow.AddHours(1));
            return uri.ToString();
        }

        public async Task<T> LoadFromJsonAsync<T>(string containerName, string blobName)
        {
            var blob = await GetBlockBlobReferenceAsync(containerName, blobName).ConfigureAwait(false);
            if (await blob.ExistsAsync().ConfigureAwait(false))
            {
                using (var blobStream = await blob.OpenReadAsync().ConfigureAwait(false))
                {
                    return await System.Text.Json.JsonSerializer.DeserializeAsync<T>(blobStream).ConfigureAwait(false);
                }
            }
            else return default;
        }
    }
}
