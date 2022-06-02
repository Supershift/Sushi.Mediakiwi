using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sushi.Mediakiwi.API.Filters;
using Sushi.Mediakiwi.API.Transport;
using Sushi.Mediakiwi.API.Transport.Requests;
using Sushi.Mediakiwi.API.Transport.Responses;
using Sushi.Mediakiwi.Data.Configuration;
using Sushi.Mediakiwi.Logic;
using Sushi.Mediakiwi.Persisters;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.API.Controllers
{
    [MediakiwiApiAuthorize]
    [Route(Common.MK_CONTROLLERS_PREFIX + "asset")]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public class AssetController : ControllerBase
    {
        private readonly AssetService _assetService;

        public AssetController(AssetService assetService)
        {
            _assetService = assetService;
        }
        
        private static string Azure_Image_Container
        {
            get
            {
                return WimServerConfiguration.Instance?.Azure_Image_Container;
            }
        }

        protected Data.IApplicationUser MediakiwiUser
        {
            get
            {
                if (HttpContext.Items.ContainsKey(Common.API_USER_CONTEXT))
                {
                    return HttpContext.Items[Common.API_USER_CONTEXT] as Data.IApplicationUser;
                }
                else
                {
                    return null;
                }
            }
        }

        

        /// <summary>
        /// Returns all galleries that are available for the supplied user
        /// </summary>
        /// <returns></returns>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpGet("GetGalleries")]
        public async Task<ActionResult<List<ListItemCollectionOption>>> GetGalleries()
        {
            var temp = new List<ListItemCollectionOption>();

            var galleries = await Data.Gallery.SelectAllAccessibleAsync(MediakiwiUser);
            foreach (var gallery in galleries)
            {
                temp.Add(new ListItemCollectionOption() 
                { 
                    Text = gallery.CompletePath, 
                    Value = gallery.ID.ToString(), 
                    IsEnabled = true 
                });
            }

            return Ok(temp);
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

            // We received a filename without extension, throw an error
            if (request.Data?.FileName?.Contains(".", System.StringComparison.InvariantCulture) == false)
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

            // We have new data provided, process it
            Data.Asset asset;
            if (request.Data != null)
            {
                // upload to blob and save
                using var stream = request.Data.OpenReadStream();

                if (request.ID.HasValue)
                {
                    asset = await _assetService.UpdateAssetAsync(request.ID.Value, stream, Azure_Image_Container, request.Data.FileName, request.Data.ContentType, request.GalleryID, request.Title,
                        request.Description);
                }
                else
                {
                    asset = await _assetService.CreateAssetAsync(stream, Azure_Image_Container, request.Data.FileName, request.Data.ContentType, request.GalleryID.Value, request.Title, request.Description);
                }
            }
            else
            {
                asset = await _assetService.UpdateAssetAsync(request.ID.Value, null, Azure_Image_Container, request.Data.FileName, request.Data.ContentType, request.GalleryID, request.Title,
                        request.Description);
            }

            return Ok(new SaveAssetResponse() 
            {
                Description = asset.Description,
                Title = asset.Title,
                GalleryID = asset.GalleryID,
                ID = asset.ID,
                RemoteLocation = asset.RemoteLocation,
                RemoteLocationThumb  = asset.RemoteLocation_Thumb
            });
        }
    }
}
