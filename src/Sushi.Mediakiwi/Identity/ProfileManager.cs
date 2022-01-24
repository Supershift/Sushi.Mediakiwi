using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Sushi.Mediakiwi.Data;
using Sushi.Mediakiwi.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Identity
{
    public class ProfileManager : MediaKiwiProfileManager
    {
        public override string CookieName => "MEDIAKIWIPROFILE";
        public override string SchemeName => "SCHEME.MK.PROFILE";
        
        public override async Task ValidatePrincipal(CookieValidatePrincipalContext context)
        {
            var profileGuidString = context.Principal.Claims.First(x => x.Type == "guid").Value;

            // When Profile Guid is empty, return false
            if (string.IsNullOrWhiteSpace(profileGuidString))
            {
                context.RejectPrincipal();
                await context.HttpContext.SignOutAsync(SchemeName).ConfigureAwait(false);
            }

            if (Utils.IsGuid(profileGuidString, out Guid profileGuid))
            {
                var profile = await Profile.FetchSingleByGuidAsync(profileGuid);

                // When no user was found, or user is inactive
                if (profile == null || profile.ID == 0)
                {
                    context.RejectPrincipal();
                    await context.HttpContext.SignOutAsync(SchemeName).ConfigureAwait(false);
                }
            }
        }

        public override async Task<bool> SignInByEmailPasswordAsync(HttpContext context, string emailAddress, string password)
        {
            // Retrieve a profile based on the email address
            var profile = await Profile.FetchSingleByEmailAsync(emailAddress);
            if (profile == null || profile.ID == 0)
            {
                return false;
            }

            // Check password
            var passWordCheck = await profile.CheckPasswordAsync(password);
            if (passWordCheck == false)
            {
                return false;
            }

            var claims = new List<Claim>
            {
                new Claim("id", profile.ID.ToString()),
                new Claim("guid", profile.GUID.ToString()),
                new Claim("email", profile.Email),
            };

            var claimsIdentity = new ClaimsIdentity(claims, SchemeName);

            var authProperties = new AuthenticationProperties
            {
                ExpiresUtc = DateTime.UtcNow.AddHours(2), // TODO: make this a setting
                IsPersistent = true,
                IssuedUtc = DateTime.UtcNow
            };

            await context.SignInAsync(
                SchemeName,
                new ClaimsPrincipal(claimsIdentity),
                authProperties).ConfigureAwait(false);

            return false;
        }

        public override async Task<bool> CreateByEmailAsync(HttpContext context, string emailAddress, string password)
        {
            var profile = await Profile.FetchSingleByEmailAsync(emailAddress);
        }
    }
}
