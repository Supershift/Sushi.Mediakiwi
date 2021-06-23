using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Connectors
{
    public class AzureBlobConnector : IDisposable
    {
        public static string AzureStorageBlobURL;
        public static string AzureStorageContainer;
        public static string AzurePrefix;
        public static string AzurePrefixAllowed;

        private CloudStorageAccount _Account;
        private CloudStorageAccount Account
        {
            get
            {
                if (_Account == null && !string.IsNullOrWhiteSpace(AzureStorageBlobURL))
                    CloudStorageAccount.TryParse(AzureStorageBlobURL, out _Account);

                return _Account;
            }
        }


        public async Task<string> UploadAsync(string filename, string contentType, byte[] bytes)
        {
            var blobClient = Account.CreateCloudBlobClient();

            var containerCient = blobClient.GetContainerReference(AzureStorageContainer);
            await containerCient.CreateIfNotExistsAsync();

            await containerCient.SetPermissionsAsync(new BlobContainerPermissions
            {
                PublicAccess = BlobContainerPublicAccessType.Blob
            });

            var blockBlob = containerCient.GetBlockBlobReference(filename);
            await blockBlob.UploadFromByteArrayAsync(bytes, 0, bytes.Length);

            const int durationInSeconds = 60 * 60 * 24 * 100; // 100 days
            blockBlob.Properties.CacheControl = "public,max-age=" + durationInSeconds;

            blockBlob.Properties.ContentType = contentType;
            //blockBlob.Properties.ContentEncoding = "gzip";
            await blockBlob.SetPropertiesAsync();
            var uri = blockBlob.Uri.ToString();
            Console.WriteLine($"Uploaded to Azure: {uri}");
            return uri;
        }

        public void Dispose()
        {
            _Account = null;
        }
    }
}
