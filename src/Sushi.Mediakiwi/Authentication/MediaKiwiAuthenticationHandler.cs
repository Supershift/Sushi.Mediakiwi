using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sushi.Mediakiwi.Data;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Sushi.Mediakiwi.Data.Configuration;
using Sushi.Mediakiwi.Logic;

namespace Sushi.Mediakiwi.Authentication
{
    public class MediaKiwiAuthenticationHandler : AuthenticationHandler<MediaKiwiAuthenticationOptions>
    {
        public MediaKiwiAuthenticationHandler(
            IOptionsMonitor<MediaKiwiAuthenticationOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock
            )
            : base(options, logger, encoder, clock)
        {
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (WimServerConfiguration.Instance?.Authentication?.Aad?.Enabled == true)
            {
                return await HandleAuthenticateAadAsync().ConfigureAwait(false);
            }
            else
            {
                return await HandleAuthenticateLocalAsync().ConfigureAwait(false);
            }
        }

        private async Task<AuthenticateResult> HandleAuthenticateAadAsync()
        {
            var visManager = new VisitorManager(Context);

            var currentVisitor = await visManager.SelectAsync().ConfigureAwait(false);
            if (!string.IsNullOrWhiteSpace(currentVisitor?.Jwt))
            {
                // Validate the JWT
                string email = await OAuth2Logic.ExtractUpnAsync(WimServerConfiguration.Instance.Authentication, currentVisitor.Jwt, Context);
                if (!string.IsNullOrWhiteSpace(email))
                {
                    var currentApplicationUser = await ApplicationUser.SelectOneByEmailAsync(email, true);
                    if (currentApplicationUser != null)
                    {
                        var claims = new List<Claim>();
                        claims.Add(new Claim(ClaimTypes.Name, currentApplicationUser.Displayname));
                        claims.Add(new Claim(ClaimTypes.Email, currentApplicationUser.Email));
                        var identity = new ClaimsIdentity(claims, AuthenticationDefaults.AuthenticationType);

                        var principal = new ClaimsPrincipal(identity);
                        var ticket = new AuthenticationTicket(principal, Scheme.Name);
                        return AuthenticateResult.Success(ticket);
                    }
                    else
                    {
                        return AuthenticateResult.Fail($"No active user found with email address: {email}");
                    }
                }
                else
                {
                    return AuthenticateResult.Fail("UPN could not be extracted from JWT");
                }
            }
            else
            {
                return AuthenticateResult.Fail("No JWT was found on the visitor");
            }
        }

        private async Task<AuthenticateResult> HandleAuthenticateLocalAsync()
        {
            // If we land here from an API call where a MKAPI call has been made first,
            // we can identify the user based on it's GUID claim
            if (Context?.User?.HasClaim(x => x.Type == "guid") == true)
            {
                var userGuid = Guid.Parse(Context.User.Claims.FirstOrDefault(x => x.Type == "guid").Value);

                var currentApplicationUser = await ApplicationUser.SelectOneAsync(userGuid).ConfigureAwait(false);
                if (currentApplicationUser?.ID > 0 && currentApplicationUser.IsActive == true)
                {
                    var principal = new ClaimsPrincipal(Context.User.Identity);
                    var ticket = new AuthenticationTicket(principal, Scheme.Name);

                    return AuthenticateResult.Success(ticket);
                }
                else
                {
                    return AuthenticateResult.Fail("Invalid user");
                }
            }

            // If we land here directly from the Portal, we should have a visitor
            // Containing user information
            var visManager = new VisitorManager(Context);

            var currentVisitor = await visManager.SelectAsync().ConfigureAwait(false);
            if (currentVisitor != null
                 && currentVisitor.ApplicationUserID.HasValue
                 && currentVisitor.ApplicationUserID.Value > 0
                 )
            {
                var currentApplicationUser = await ApplicationUser.SelectOneAsync(currentVisitor.ApplicationUserID.Value, true).ConfigureAwait(false);

                var claims = new List<Claim>();
                claims.Add(new Claim(ClaimTypes.Name, currentApplicationUser.Displayname));
                claims.Add(new Claim(ClaimTypes.Email, currentApplicationUser.Email));
                var identity = new ClaimsIdentity(claims, AuthenticationDefaults.AuthenticationType);

                var principal = new ClaimsPrincipal(identity);
                var ticket = new AuthenticationTicket(principal, Scheme.Name);
                return AuthenticateResult.Success(ticket);
            }

            return AuthenticateResult.Fail("Invalid user");
        }
    }
}
