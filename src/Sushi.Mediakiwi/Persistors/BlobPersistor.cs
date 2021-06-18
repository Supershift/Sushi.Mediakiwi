using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Persistors
{
    public class BlobPersister
    {
        public BlobPersister() : this(Mediakiwi.Data.Common.GetConnection("azurestore"))
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

        protected static string ContainerCachePrefix = Guid.NewGuid().ToString();
        protected static ConcurrentDictionary<string, bool> Cache = new ConcurrentDictionary<string, bool>();

        public async Task<BlobClient> GetContainerAsync(string name)
        {
            var storageAccount = new BlobContainerClient(ConnectionString, name);
            // Create the blob client.
            var blobClient = storageAccount.GetBlobClient(name);
            return blobClient;
        }

        public async Task<bool> ExistsAsync(string containerName, string blobName)
        {
            var blobReference = await GetBlockBlobReferenceAsync(containerName, blobName);
            return await blobReference.ExistsAsync();
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

            var blob = await GetBlockBlobReferenceAsync(container, blobName);
            var data = await blob.UploadAsync(stream, info);
            return blob;
        }

        public string GetSasTokenLink(string containerName, string blobName)
        {
            var storageAccount = new BlobContainerClient(ConnectionString, containerName);

            var accountName = storageAccount.AccountName;
            var uri = storageAccount.GenerateSasUri( BlobContainerSasPermissions.Read, DateTimeOffset.UtcNow.AddHours(1));
            return uri.ToString();
        }

        public async Task<T> LoadFromJsonAsync<T>(string containerName, string blobName)
        {
            var blob = await GetBlockBlobReferenceAsync(containerName, blobName);
            if (await blob.ExistsAsync())
            {
                using (var blobStream = await blob.OpenReadAsync())
                {
                    return await System.Text.Json.JsonSerializer.DeserializeAsync<T>(blobStream);
                }
            }
            else return default;
        }
    }
}
