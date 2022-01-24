using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Sushi.Mediakiwi.Controllers.Data;
using Sushi.Mediakiwi.Interfaces;
using System;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Controllers
{
    [ApiController]
    [Route("api/profile/v1.0")]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public class ProfileController : MediakiwiController
    {
        MediaKiwiProfileManager _profileManager;

        public ProfileController(IServiceProvider services)
        {
            _profileManager = services.GetService<MediaKiwiProfileManager>();
        }

        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet]
        [Route("ping")]
        public async Task<ActionResult<string>> PingAsync()
        {
            return Ok("Hello from profile service");
        }

        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpGet]
        [Route("login")]
        public async Task<ActionResult> LoginAsync([FromBody]ProfileLoginRequest request)
        {
            if (_profileManager == null)
            {
                return BadRequest("No ProfileService has been added.");
            }

            if (request == null || (string.IsNullOrWhiteSpace(request.EmailAddress) && string.IsNullOrWhiteSpace(request.UserName)) || string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest("Request is empty");
            }

            bool isLoggedIn = false;


            if (string.IsNullOrWhiteSpace(request.EmailAddress) == false)
            {
                isLoggedIn = await _profileManager.SignInByEmailPasswordAsync(ControllerContext.HttpContext, request.EmailAddress, request.Password);
            }
            else
            {
                isLoggedIn = await _profileManager.SignInByUsernamePasswordAsync(ControllerContext.HttpContext, request.UserName, request.Password);
            }

            if (isLoggedIn)
            {
                return Ok();
            }
            else 
            { 
                return Unauthorized();
            }
        }

        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpPost]
        [Route("createprofile")]
        public async Task<ActionResult> CreateProfileAsync([FromBody] CreateProfileRequest request)
        {
            if (_profileManager == null)
            {
                return BadRequest("No ProfileService has been added.");
            }

            if (request == null || (string.IsNullOrWhiteSpace(request.EmailAddress) && string.IsNullOrWhiteSpace(request.UserName)))
            {
                return BadRequest("Request is empty");
            }

            bool isLoggedIn = false;


            if (string.IsNullOrWhiteSpace(request.EmailAddress) == false)
            {
                isLoggedIn = await _profileManager.SignInByEmailPasswordAsync(ControllerContext.HttpContext, request.EmailAddress, request.Password);
            }
            else
            {
                isLoggedIn = await _profileManager.SignInByUsernamePasswordAsync(ControllerContext.HttpContext, request.UserName, request.Password);
            }

            if (isLoggedIn)
            {
                return Ok();
            }
            else
            {
                return Unauthorized();
            }
        }
    }
}
