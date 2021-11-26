using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens;
using Sushi.Mediakiwi.Data;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;

namespace Sushi.Mediakiwi.API
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class MediakiwiApiAuthorizeAttribute : Attribute, IAuthorizationFilter
    {

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            // We are in an AllowAnonymous call
            if (context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any())
            {
                return;
            }

            JsonResult unAuthorized = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };

            // we don't have the api cookie, so we are not logged in
            if (context.HttpContext?.User?.Identity?.IsAuthenticated != true)
            {
                context.Result = unAuthorized;
            }
        }
    }
}
