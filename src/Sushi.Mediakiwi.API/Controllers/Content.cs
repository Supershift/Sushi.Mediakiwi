using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sushi.Mediakiwi.API.Filters;
using Sushi.Mediakiwi.API.Services;
using Sushi.Mediakiwi.API.Transport.Requests;
using Sushi.Mediakiwi.API.Transport.Responses;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.API.Controllers
{
    [ApiController]
    [MediakiwiApiAuthorize]
    [Route(Common.MK_CONTROLLERS_PREFIX + "content")]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public class Content : BaseMediakiwiApiController
    {
        private readonly IContentService _contentService;

        public Content(IContentService _service)
        {
            _contentService = _service;
        }

        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpGet("GetContent")]
        public async Task<ActionResult<GetContentResponse>> GetContent([FromBody] GetContentRequest request)
        {
            GetContentResponse result = new GetContentResponse();

            if (request.CurrentSiteID == 0)
            {
                return BadRequest();
            }

            switch (Resolver.ItemType)
            {
                case RequestItemType.Undefined:
                    break;
                case RequestItemType.Item:
                    break;
                case RequestItemType.Page:
                    break;
                case RequestItemType.Asset:
                    break;
                case RequestItemType.Dashboard:
                    break;
                default:
                    break;
            }

            // We are looking at a list
            if (Resolver.ListInstance != null)
            {
                result.List = await _contentService.GetListResponseAsync(Resolver).ConfigureAwait(false);
                result.IsEditMode = result.List.IsEditMode;
                result.StatusCode = System.Net.HttpStatusCode.OK;
            }

            return Ok(result);
        }

        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpPost("PostContent")]
        public async Task<ActionResult<PostContentResponse>> PostContent([FromBody] PostContentRequest request)
        {
            PostContentResponse result = new PostContentResponse();

            if (request.CurrentSiteID == 0 || string.IsNullOrEmpty(request.PostedField))
            {
                return BadRequest();
            }

            switch (Resolver.ItemType)
            {
                case RequestItemType.Undefined:
                    break;
                case RequestItemType.Item:
                    break;
                case RequestItemType.Page:
                    break;
                case RequestItemType.Asset:
                    break;
                case RequestItemType.Dashboard:
                    break;
                default:
                    break;
            }

            // We are looking at a list
            if (Resolver.ListInstance != null)
            {
                result.List = await _contentService.GetListResponseAsync(Resolver).ConfigureAwait(false);
                result.IsEditMode = result.List.IsEditMode;
                result.StatusCode = System.Net.HttpStatusCode.OK;
            }

            return Ok(result);
        }
    }
}
