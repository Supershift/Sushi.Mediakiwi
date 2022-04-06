using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Sushi.Mediakiwi.API.Filters;
using Sushi.Mediakiwi.API.Services;
using Sushi.Mediakiwi.API.Transport.Requests;
using Sushi.Mediakiwi.API.Transport.Responses;
using Sushi.Mediakiwi.Framework.Interfaces;
using System.Collections.Generic;
using System.Linq;
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

        /// <summary>
        /// Returns the CMS content belonging to the URL being viewed.
        /// This can be anything from a Page, List, Gallery, Asset or a Folder Explorer
        /// </summary>
        /// <param name="request">The request containing the needed information</param>
        /// <returns></returns>
        /// <response code="200">The Content is succesfully retrieved</response>
        /// <response code="400">Some needed information is missing from the request</response>
        /// <response code="401">The user is not succesfully authenticated</response>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpGet("GetContent")]
        public async Task<ActionResult<GetContentResponse>> GetContent([FromQuery] GetContentRequest request)
        {
            GetContentResponse result = new GetContentResponse();

            if (request.CurrentSiteID == 0)
            {
                return BadRequest();
            }

            // We are looking at a list, but not the Browsing list
            if (Resolver.ListInstance != null && Resolver.List.ClassName.Contains("Sushi.Mediakiwi.AppCentre.Data.Implementation.Browsing", System.StringComparison.InvariantCultureIgnoreCase)== false)
            {
                result.List = await _contentService.GetListResponseAsync(Resolver).ConfigureAwait(false);
                result.IsEditMode = result.List.IsEditMode;
                result.StatusCode = System.Net.HttpStatusCode.OK;
            }
            // We are looking at a page
            else if (Resolver.Page != null)
            {
                //result.Page = await _contentService.GetExplorerResponseAsync(Resolver).ConfigureAwait(false);
            }
            // We are browsing
            else 
            {
                result.Explorer = await _contentService.GetExplorerResponseAsync(Resolver).ConfigureAwait(false);
                result.StatusCode = System.Net.HttpStatusCode.OK;
            }

            return Ok(result);
        }

        /// <summary>
        /// Handles the incoming Post parameters and returns the CMS content belonging to the URL being viewed.
        /// This can be anything from a Page, List, Gallery, Asset or a Folder Explorer
        /// </summary>
        /// <param name="request">The request containing the needed information</param>
        /// <returns></returns>
        /// <response code="200">The Content is succesfully retrieved</response>
        /// <response code="400">Some needed information is missing from the request</response>
        /// <response code="401">The user is not succesfully authenticated</response>
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

            // We are looking at a list
            if (Resolver.ListInstance != null)
            {
                // Whenever a postback hits, set editmode to true, so that 
                // possible filters can be set
                Resolver.ListInstance.wim.IsEditMode = true;
                
                // Set Correct View (Browsing mode / Edit mode)
                if (Resolver.ItemID.GetValueOrDefault(-1) > -1 
                    || Resolver.ItemObject != null 
                    || Resolver?.ListInstance?.wim?.CanContainSingleInstancePerDefinedList == true)
                {
                    // Edit / Save Mode
                    Resolver.ListInstance.wim.Console.View = 0; // 0 == ContainerView.ItemSelect
                }
                else 
                {
                    // Browse Mode
                    Resolver.ListInstance.wim.Console.View = 2; // 2 == ContainerView.BrowsingRequest
                }

                result.List = await _contentService.GetListResponseAsync(Resolver).ConfigureAwait(false);

                // List Module addition
                if (request?.PostedField?.StartsWith("listmod_", System.StringComparison.InvariantCulture) == true)
                {
                    string pBack = request.PostedField.Replace("listmod_", "", System.StringComparison.InvariantCultureIgnoreCase);

                    IListModule targetListModule = default(IListModule);

                    if (HttpContext?.RequestServices?.GetServices<IListModule>().Any() == true)
                    {
                        targetListModule = HttpContext.RequestServices.GetServices<IListModule>().FirstOrDefault(x => x.GetType().Name == pBack);
                    }

                    if (targetListModule != null)
                    {
                        try
                        {
                            var temp = await targetListModule.ExecuteAsync(Resolver.ListInstance, Resolver.ApplicationUser, HttpContext);
                            if (temp.IsSuccess && string.IsNullOrWhiteSpace(temp.RedirectUrl) == false)
                            { 
                                result.List.RedirectURL = temp.RedirectUrl;
                            }
                        }
                        catch (System.Exception ex)
                        {
                            await Data.Notification.InsertOneAsync(pBack, ex);
                        }
                    }

                }

                result.IsEditMode = result.List.IsEditMode;
                result.StatusCode = System.Net.HttpStatusCode.OK;
            }

            return Ok(result);
        }
    }
}
