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

namespace Sushi.Mediakiwi.API.Filters
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
            if (context.HttpContext.Request.Cookies.ContainsKey(Common.API_COOKIE_KEY) == false)
            {
                context.Result = unAuthorized;
            }

            // Decode token
            if (context.HttpContext.Request.Cookies.TryGetValue(Common.API_COOKIE_KEY, out string token))
            {
                ValidateUserResult userValidationResult = new ValidateUserResult();

                if (token != null)
                {
                    userValidationResult = validateUser(token, context.HttpContext);
                }

                if (userValidationResult.IsValid == false)
                {
                    // not logged in
                    context.Result = unAuthorized;
                }
            }
            else
            {
                context.Result = unAuthorized;
            }
        }

        private class ValidateUserResult
        {
            public bool IsValid { get; set; }
            public IApplicationUser User { get; set; }
        }

        private ValidateUserResult validateUser(string token, HttpContext httpContext)
        {
            ValidateUserResult result = new ValidateUserResult();

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(Common.API_AUTHENTICATION_KEY);

                var validations = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = false,
                    ValidIssuer = Common.API_AUTHENTICATION_ISSUER,
                    ValidAudience = Common.API_AUTHENTICATION_AUDIENCE
                };

                var jwtUser = tokenHandler.ValidateToken(token, validations, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var userGuidString = jwtToken.Claims.First(x => x.Type == "guid").Value;
                var apiKeyString = jwtToken.Claims.First(x => x.Type == "apiKey").Value;

                // When userGuid or ApiKey are empty, return false
                if (string.IsNullOrWhiteSpace(apiKeyString) || string.IsNullOrWhiteSpace(userGuidString))
                {
                    return result;
                }

                Guid userGuid = new Guid();

                if (Utils.IsGuid(userGuidString, out userGuid))
                {
                    var user = ApplicationUser.SelectOne(userGuid);

                    // When no user was found, or user is inactive
                    if (user?.IsActive != true)
                    {
                        return result;
                    }

                    var env = Data.Environment.SelectOne();
                    if (env.ApiKey.Equals(apiKeyString, StringComparison.InvariantCulture) == false)
                    {
                        return result;
                    }

                    result.IsValid = true;
                    result.User = user;

                    // Set ClaimsPrincipal to jwtUser
                    httpContext.User = jwtUser;

                    // Add application User to context
                    if (httpContext.Items.ContainsKey(Common.API_USER_CONTEXT) == false)
                    {
                        httpContext.Items.Add(Common.API_USER_CONTEXT, user);
                    }
                }
                else
                {
                    return result;
                }
            }
            catch (Exception ex)
            {
                return result;
            }

            return result;
        }
    }
}
