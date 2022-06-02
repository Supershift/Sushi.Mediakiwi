using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Sushi.Mediakiwi.Data;
using Sushi.Mediakiwi.Data.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Logic
{
    public class AssetService
    {
        private readonly string[] ImageTypes = new string[]
        {
            "image/jpeg",
            "image/jpg",
            "image/gif",
            "image/png",
            "image/bmp"
        };
        private readonly BlobServiceClient _blobServiceClient;

        public AssetService(BlobServiceClient blobServiceClient)
        {
            _blobServiceClient = blobServiceClient;
        }

        public async Task<Asset> CreateAssetAsync(Stream inputStream, string container, string fileName, string contentType, int galleryID, string? title = null, string? description = null)
        {
            // create asset
            var asset = new Asset()
            {
                Created = DateTime.UtcNow
            };

            return await UpsertAssetAsync(asset, inputStream, container, fileName, contentType, galleryID, title, description);
        }

        
        
        
        public async Task<Asset?> UpdateAssetAsync(int id, Stream inputStream, string container, string fileName, string contentType, int? galleryID = null, string? title = null, string? description = null)
        {
            // get asset
            var asset = await Asset.SelectOneAsync(id);

            if (asset == null || asset.ID == 0)
                return null;

            return await UpsertAssetAsync(asset, inputStream, container, fileName, contentType, galleryID, title, description);
        }

        public async Task<bool> DeleteAsync(Asset asset, string container)
        {   
            // get blob client
            var containerClient = _blobServiceClient.GetBlobContainerClient(container);
            var blobClient = containerClient.GetBlobClient(asset.FileName);

            // delete blob
            var result = await blobClient.DeleteIfExistsAsync();

            if (result)
            {
                // Delete the DB record
                await asset.DeleteAsync().ConfigureAwait(false);

                // Get the parent gallery for the asset
                var gallery = await Gallery.SelectOneAsync(asset.GalleryID).ConfigureAwait(false);

                if (gallery?.ID > 0)
                {
                    await gallery.UpdateCountAsync().ConfigureAwait(false);
                }
            }

            return result;
        }

        /// <summary>
        /// If <see cref="WimServerConfiguration.Azure_Cdn_Uri"/> is set, a URL to the blob using the CDN url is returned.
        /// Otherwise the <see cref="Asset.RemoteLocation"/> is returned.
        /// </summary>
        /// <param name="asset"></param>
        /// <returns></returns>
        public string GetCdnUrl(Asset asset)
        {
            if (!string.IsNullOrWhiteSpace(asset?.RemoteLocation))
            {
                if (!string.IsNullOrWhiteSpace(WimServerConfiguration.Instance?.Azure_Cdn_Uri))
                {
                    var blobUrl = new Uri(asset.RemoteLocation);
                    return $"{WimServerConfiguration.Instance.Azure_Cdn_Uri}{blobUrl.PathAndQuery}";
                }
                else
                    return asset.RemoteLocation;
            }
            else
            {
                return null;
            }
        }

        private async Task<Asset> UpsertAssetAsync(Asset asset, Stream? inputStream, string container, string? fileName, string? contentType, int? galleryID = null, string? title = null, string? description = null)
        {
            // get extension
            string extension = null;
            var extensionIndex = fileName.LastIndexOf('.');
            if (extensionIndex >= 0)
            {
                extension = fileName.Substring(extensionIndex);
            }

            // set asset properties            
            asset.GalleryID = galleryID.GetValueOrDefault(asset.GalleryID);
            asset.Description = description;
            asset.Extension = extension;
            asset.Type = contentType;
            
            // if a stream is supplied, upload to blob
            if (inputStream != null)
            {
                asset.Size = inputStream.Length;

                // is uploaded file an image? determine from contentType
                bool isImage = ImageTypes.Contains(contentType, StringComparer.InvariantCultureIgnoreCase);

                if (isImage)
                {
                    asset.IsImage = true;
                    // determine width and heigth from image stream
                    var imageInfo = await SixLabors.ImageSharp.Image.IdentifyAsync(inputStream);

                    asset.Width = imageInfo.Width;
                    asset.Height = imageInfo.Height;
                }

                // SVG is also an image, but not one from which you can get the width and height
                if (contentType.Equals("image/svg+xml", StringComparison.InvariantCultureIgnoreCase))
                {
                    asset.IsImage = true;
                }

                // upload stream to blob storage            
                var containerClient = _blobServiceClient.GetBlobContainerClient(container);
                var blobClient = containerClient.GetBlobClient(fileName);

                // check if exists. if already exists, append ticks to create unique filename
                bool alreadyExists = await blobClient.ExistsAsync();
                if (alreadyExists)
                {
                    if (extensionIndex > 0)
                    {
                        fileName = $"{fileName.Substring(0, extensionIndex)}-{DateTime.UtcNow.Ticks}{asset.Extension}";
                    }
                    else
                    {
                        // filename does not have an extension, append ticks at end
                        fileName = $"{fileName}-{DateTime.UtcNow.Ticks}";
                    }

                    // create new client for new blobname
                    blobClient = containerClient.GetBlobClient(fileName);
                }

                // upload to blob
                var headers = new BlobHttpHeaders
                {
                    ContentType = asset.Type
                };

                inputStream.Position = 0;
                await blobClient.UploadAsync(inputStream, new BlobUploadOptions() { HttpHeaders = headers });

                // set blob url and unique filename
                asset.FileName = fileName;
                asset.RemoteLocation = blobClient.Uri.ToString();
            }
            // if no title supplied, set title to filename
            if (string.IsNullOrWhiteSpace(title))
            {
                asset.Title = asset.FileName;
            }
            else
            {
                asset.Title = title;
            }

            // save asset to database
            await asset.SaveAsync();

            return asset;
        }

        private async Task UploadToBlobAsync(Stream inputStream, Asset asset, string fileName, string container)
        {
            
        }
    }
}
