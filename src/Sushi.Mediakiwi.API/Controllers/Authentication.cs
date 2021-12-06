using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Sushi.Mediakiwi.API.Services;
using Sushi.Mediakiwi.API.Transport.Requests;
using Sushi.Mediakiwi.API.Transport.Responses;
using System;
using Sushi.Mediakiwi.API.Filters;

namespace Sushi.Mediakiwi.API.Controllers
{
    [ApiController]
    [MediakiwiApiAuthorize]
    [Route(Common.MK_CONTROLLERS_PREFIX + "authentication")]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public class Authentication : BaseMediakiwiApiController
    {
        private readonly IUserService _userService;

        public Authentication(IUserService _service)
        {
            _userService = _service;
        }

        /// <summary>
        /// Logs a user in based on their Email / Password combination. 
        /// This method will also set a cookie 'MKAPI' which contains the JWT token.
        /// The default Expiration time is 2 hours
        /// </summary>
        /// <param name="request">The request containing the needed credentials</param>
        /// <returns></returns>6
        /// <response code="200">The user is succesfully authenticated</response>
        /// <response code="400">Some needed information is missing from the request</response>
        /// <response code="401">The user is not succesfully authenticated</response>
        [AllowAnonymous]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpPost("Login")]
        public async Task<ActionResult<LoginResponse>> Login([FromBody]LoginRequest request)
        {
            if (request == null || _userService == null || string.IsNullOrWhiteSpace(request.ApiKey) || string.IsNullOrWhiteSpace(request.EmailAddress) || string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest();
            }

            // Compare API key with environment
            var env = await Data.Environment.SelectOneAsync().ConfigureAwait(false);
            if (env.ApiKey.Equals(request.ApiKey, StringComparison.InvariantCulture) == false)
            {
                return Unauthorized();
            }

            // Check for user existence and create jwtToken if valid.
            LoginResponse result = await _userService.LoginAsync(request).ConfigureAwait(false);

            // When successful, return filled response object
            if (result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return Ok(result);
            }
            // When unsuccessful, return unauthorized
            else if (result.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                await _userService.LogoutAsync().ConfigureAwait(false);
                return Unauthorized();
            }

            // Fallback to BadRequest
            return BadRequest();
        }

        /// <summary>
        /// Sends an email to a user containing information on how to reset his/her password. 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <response code="200">The mail was succesfully sent</response>
        /// <response code="400">Some needed information is missing from the request</response>
        /// <response code="401">The user is not succesfully authenticated</response>
        /// <response code="424">The Send mail dependancy failed to operate</response>
        [AllowAnonymous]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status424FailedDependency)]
        [HttpPost("ResetPassword")]
        public async Task<ActionResult<ResetPasswordResponse>> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            if (request == null || _userService == null || string.IsNullOrWhiteSpace(request.EmailAddress))
            {
                return BadRequest();
            }

            // Check for user existence and create jwtToken if valid.
            ResetPasswordResponse result = await _userService.ResetPassword(request, Console).ConfigureAwait(false);

            // When successful, return filled response object
            if (result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return Ok(result);
            }
            // When Unauthorized, return unauthorized
            else if (result.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                return Unauthorized();
            }
            // When SMTP client returned an error
            else if (result.StatusCode == System.Net.HttpStatusCode.FailedDependency)
            {
                return new StatusCodeResult((int)System.Net.HttpStatusCode.FailedDependency);
            }

            // Fallback to BadRequest
            return BadRequest();
        }


        /// <summary>
        /// Will remove the cookie for an authenticated user.
        /// </summary>
        /// <returns></returns>
        /// <response code="200">The user was succesfully logged out</response>
        /// <response code="400">Some needed information is missing from the request</response>
        /// <response code="401">The user is not succesfully authenticated</response>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpPost("Logout")]
        public async Task<ActionResult<LogoutResponse>> Logout()
        {
            if (_userService == null)
            {
                return BadRequest();
            }

            LogoutResponse result = new LogoutResponse()
            {
                TargetUrl = "/",
                StatusCode = System.Net.HttpStatusCode.OK
            };

            // Remove cookie
            await _userService.LogoutAsync().ConfigureAwait(false);

            return Ok(result);
        }
    }
}
