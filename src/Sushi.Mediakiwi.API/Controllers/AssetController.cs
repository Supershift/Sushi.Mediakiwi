using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sushi.Mediakiwi.API.Filters;
using Sushi.Mediakiwi.API.Transport.Requests;
using Sushi.Mediakiwi.API.Transport.Responses;
using Sushi.Mediakiwi.Data.Configuration;
using Sushi.Mediakiwi.Persisters;
using System.IO;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.API.Controllers
{
    [MediakiwiApiAuthorize]
    [Route(Common.MK_CONTROLLERS_PREFIX + "asset")]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public class AssetController : ControllerBase
    {
        private static string Azure_Image_Container
        {
            get
            {
                return WimServerConfiguration.Instance?.Azure_Image_Container;
            }
        }

        private static string Azure_Cdn_Uri
        {
            get
            {
                return WimServerConfiguration.Instance?.Azure_Cdn_Uri;
            }
        }

        private static BlobPersister GetPersistor
        {
            get { return new BlobPersister(); }
        }

        private System.Drawing.Image CreateThumbnailImage(System.Drawing.Image input)
        {
            try
            {
                var imageHeight = input.Height;
                var imageWidth = input.Width;

                var maxThumbWidth = System.Math.Max(64, WimServerConfiguration.Instance.Thumbnails.CreateThumbnailWidth);
                var maxThumbHeight = System.Math.Max(48, WimServerConfiguration.Instance.Thumbnails.CreateThumbnailHeight);

                if (imageHeight > imageWidth)
                {
                    var factor = ((float)maxThumbHeight / (float)imageHeight);
                    imageWidth = (int)(factor * imageWidth);
                    imageHeight = maxThumbHeight;
                }
                else
                {
                    var factor = ((float)maxThumbWidth / (float)imageWidth);
                    imageHeight = (int)(factor * imageHeight);
                    imageWidth = maxThumbWidth;
                }

                return input.GetThumbnailImage(imageWidth, imageHeight, () => false, System.IntPtr.Zero);

            }
            catch (System.Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// Saves the supplied asset to Azure and returns information about the created/updated asset
        /// </summary>
        /// <param name="request">Contains all information about the asset</param>
        /// <returns></returns>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpPost("SaveAsset")]
        public async Task<ActionResult<SaveAssetResponse>> SaveAsset([FromForm] SaveAssetRequest request)
        {
            // Request is empty
            if (request == null)
            {
                return BadRequest();
            }

            // New image
            if (request.ID.GetValueOrDefault(0) == 0)
            {
                // But no data supplied
                if (request.Data?.Length == 0 || string.IsNullOrWhiteSpace(request.Data?.FileName) == true)
                {
                    return BadRequest();
                }

                // But no gallery supplied
                if (request.GalleryID.GetValueOrDefault(0) == 0)
                {
                    return BadRequest();
                }
            }

            // The target asset
            Data.Asset targetAsset = new Data.Asset();

            // We're updating an asset, get it from DB
            if (request.ID.GetValueOrDefault(0) > 0)
            {
                targetAsset = await Data.Asset.SelectOneAsync(request.ID.Value);
            }

            // We have new data provided, process it
            if (request.Data != null)
            {
                targetAsset.Type = request.Data.ContentType;
                targetAsset.Extention = request.Data.FileName.Substring(request.Data.FileName.LastIndexOf('.'));
                targetAsset.FileName = request.Data.FileName;
                targetAsset.Size = request.Data.Length;

                // set the new filename
                targetAsset.FileName = request.Data.FileName;

                // Whenever it's an image, extract image data
                if (request.Data.ContentType.Equals("image/jpeg", System.StringComparison.InvariantCultureIgnoreCase) ||
                   request.Data.ContentType.Equals("image/jpg", System.StringComparison.InvariantCultureIgnoreCase) ||
                   request.Data.ContentType.Equals("image/gif", System.StringComparison.InvariantCultureIgnoreCase) ||
                   request.Data.ContentType.Equals("image/png", System.StringComparison.InvariantCultureIgnoreCase))
                {
                    targetAsset.IsImage = true; 
                    var extension = request.Data.FileName.Substring(request.Data.FileName.LastIndexOf('.') + 1);
                    var name = request.Data.FileName.Substring(0, request.Data.FileName.LastIndexOf('.'));

                    // Create an image based on the supplied data
                    using (var image = System.Drawing.Image.FromStream(request.Data.OpenReadStream()))
                    {
                        targetAsset.Width = image.Width;
                        targetAsset.Height = image.Height;
                        
                        // Create a thumbnail ?
                        if (WimServerConfiguration.Instance.Thumbnails.CreateThumbnails)
                        {
                            try
                            {

                                using (var thumb = CreateThumbnailImage(image))
                                {
                                    if (thumb != null)
                                    {
                                  
                                        var thumbName = $"{name}_thumb.{extension}";

                                        using (MemoryStream ms = new MemoryStream())
                                        {
                                            thumb.Save(ms, image.RawFormat);
                                            ms.Position = 0;

                                            var thumbUpload = await GetPersistor.UploadAsync(ms, Azure_Image_Container, thumbName, request.Data.ContentType).ConfigureAwait(false);
                                            if (string.IsNullOrWhiteSpace(Azure_Cdn_Uri))
                                            {
                                                targetAsset.RemoteLocation_Thumb = $"{thumbUpload.Uri}";
                                            }
                                            else
                                            {
                                                targetAsset.RemoteLocation_Thumb = $"{Azure_Cdn_Uri}{thumbUpload.Uri.PathAndQuery}";
                                            }
                                        }
                                    }
                                }
                            }
                            catch (System.Exception ex)
                            {
                                await Data.Notification.InsertOneAsync(nameof(AssetController), $"Could not create thumbnail: {ex.Message}");
                            }
                        }
                    }
                }

                // SVG is also an image, but not one from which you can get the width and height
                if (request.Data.ContentType.Equals("image/svg+xml", System.StringComparison.InvariantCultureIgnoreCase))
                {
                    targetAsset.IsImage = true;
                }

                // Upload the asset to azure.
                var upload = await GetPersistor.UploadAsync(request.Data.OpenReadStream(), Azure_Image_Container, request.Data.FileName, request.Data.ContentType).ConfigureAwait(false);
                if (string.IsNullOrWhiteSpace(Azure_Cdn_Uri))
                {
                    targetAsset.RemoteLocation = $"{upload.Uri}";
                }
                else
                {
                    targetAsset.RemoteLocation = $"{Azure_Cdn_Uri}{upload.Uri.PathAndQuery}";
                }
            }

            // Update gallery when set
            if (request.GalleryID.GetValueOrDefault(0) > 0)
            {
                targetAsset.GalleryID = request.GalleryID.Value;
            }

            targetAsset.Description = request.Description;
            if (string.IsNullOrWhiteSpace(request.Title) == false)
            {
                targetAsset.Title = request.Title;
            }

            if (string.IsNullOrWhiteSpace(targetAsset.Title))
            {
                targetAsset.Title = targetAsset.FileName;
            }

            await targetAsset.SaveAsync();

            return Ok(new SaveAssetResponse() 
            {
                Description = targetAsset.Description,
                Title = targetAsset.Title,
                GalleryID = targetAsset.GalleryID,
                ID = targetAsset.ID,
                RemoteLocation = targetAsset.RemoteLocation,
                RemoteLocationThumb  = targetAsset.RemoteLocation_Thumb
            });
        }
    }
}
