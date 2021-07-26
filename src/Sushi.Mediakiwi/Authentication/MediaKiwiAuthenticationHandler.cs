using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sushi.Mediakiwi.Data;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

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
