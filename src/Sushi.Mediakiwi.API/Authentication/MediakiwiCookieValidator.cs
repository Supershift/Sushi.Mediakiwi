using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Sushi.Mediakiwi.Data;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.API.Authentication
{
    public class MediakiwiCookieValidator : CookieAuthenticationEvents
    {
        public override async Task ValidatePrincipal(CookieValidatePrincipalContext context)
        {
            var userGuidString = context.Principal.Claims.First(x => x.Type == "guid").Value;
            var apiKeyString = context.Principal.Claims.First(x => x.Type == "apiKey").Value;

            // When userGuid or ApiKey are empty, return false
            if (string.IsNullOrWhiteSpace(apiKeyString) || string.IsNullOrWhiteSpace(userGuidString))
            {
                context.RejectPrincipal();
                await context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme).ConfigureAwait(false);
            }

            Guid userGuid = new Guid();

            if (Utils.IsGuid(userGuidString, out userGuid))
            {
                var user = ApplicationUser.SelectOne(userGuid);

                // When no user was found, or user is inactive
                if (user?.IsActive != true)
                {
                    context.RejectPrincipal();
                    await context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme).ConfigureAwait(false);
                }

                var env = Data.Environment.SelectOne();
                if (env.ApiKey.Equals(apiKeyString, StringComparison.InvariantCulture) == false)
                {
                    context.RejectPrincipal();
                    await context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme).ConfigureAwait(false);
                }

                // Add application User to context
                if (context.HttpContext.Items.ContainsKey(Common.API_USER_CONTEXT) == false)
                {
                    context.HttpContext.Items.Add(Common.API_USER_CONTEXT, user);
                }
            }
        }
    }
}
