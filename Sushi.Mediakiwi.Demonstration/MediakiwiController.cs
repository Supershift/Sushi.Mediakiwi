using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Sushi.Mediakiwi.Data.Caching;
using Sushi.Mediakiwi.UI;

namespace Sushi.Mediakiwi.Demonstration
{
    //[Authorize]
    [Route("mediakiwi")]
    [ApiController]
    public class MediakiwiController : Controller
    {
        private readonly IHostingEnvironment Environment;
        private readonly IConfiguration Configuration;

        //requires using Microsoft.Extensions.Configuration  
        public MediakiwiController(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;
            Environment = env;
        }

        [AllowAnonymous]
        [HttpGet("{scheme?}")]
        public async Task<IActionResult> SignIn([FromRoute] string scheme)
        {
            if (Configuration.GetValue<bool>("mediakiwi:authentication"))
            {
                var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                if (result.Succeeded != true)
                {
                    scheme ??= OpenIdConnectDefaults.AuthenticationScheme;
                    //var redirectUrl = Url.Content("~/.auth/login/aad/callback");
                    return Challenge(
                        new AuthenticationProperties { },
                        scheme);
                }
            }
            return Ok("hello");// RedirectToAction("Login", "Mediakiwi");
        }

        [AllowAnonymous]
        [HttpGet("portal")]
        public async Task<IActionResult> externallogincallback()
        {
            if (Configuration.GetValue<bool>("mediakiwi:authentication"))
            {
                var request = HttpContext.Request;
                //Here we can retrieve the claims
                var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                if (result.Succeeded != true)
                {
                    return RedirectToAction("Login", "Mediakiwi");

                    throw new Exception("External authentication error");
                }

                // retrieve claims of the external user
                var externalUser = result.Principal;
                if (externalUser == null)
                {
                    throw new Exception("External authentication error");
                }

                // retrieve claims of the external user
                var claims = externalUser.Claims.ToList();

                // try to determine the unique id of the external user - the most common claim type for that are the sub claim and the NameIdentifier
                // depending on the external provider, some other claim type might be used
                //var userIdClaim = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.Subject);
                var userIdClaim = claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                {
                    throw new Exception("Unknown userid");
                }

                var externalUserId = userIdClaim.Value;
                var externalProvider = userIdClaim.Issuer;

                // use externalProvider and externalUserId to find your user, or provision a new user
                return Ok($"Welcome {externalUser.Identity.Name}");

            }
            Portal2 portal = new Portal2(Environment, Configuration);
            return Ok(await portal.Invoke(HttpContext));
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        [HttpGet("Login")]
        public async Task<IActionResult> LoginAsync()
        {
            //Configure(context);
            Portal2 portal = new Portal2(Environment, Configuration);
            return Ok(await portal.Invoke(HttpContext));
        }
    }
}
