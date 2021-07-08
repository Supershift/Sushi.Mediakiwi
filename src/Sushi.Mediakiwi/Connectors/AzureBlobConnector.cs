using Azure.Storage.Blobs;
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

        private BlobContainerClient _containerCient;
        private BlobContainerClient ContainerCient
        {
            get
            {
                if (_containerCient == null && !string.IsNullOrWhiteSpace(AzureStorageBlobURL))
                    _containerCient = new BlobContainerClient(AzureStorageBlobURL, AzureStorageContainer);

                return _containerCient;
            }
        }


        public async Task<string> UploadAsync(string filename, string contentType, byte[] bytes)
        {
            await ContainerCient.CreateIfNotExistsAsync(Azure.Storage.Blobs.Models.PublicAccessType.Blob).ConfigureAwait(false);

            var blockBlob = ContainerCient.GetBlobClient(filename);
            await blockBlob.UploadAsync(new BinaryData(bytes)).ConfigureAwait(false);

            var dict = new Dictionary<string, string>();
            const int durationInSeconds = 60 * 60 * 24 * 100; // 100 days
            dict.Add("CacheControl", "public,max-age=" + durationInSeconds);
            await blockBlob.SetMetadataAsync(dict).ConfigureAwait(false);

            var uri = blockBlob.Uri.ToString();
            return uri;
        }

        public void Dispose()
        {
            _containerCient = null;
        }
    }
}
