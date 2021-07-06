using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sushi.Mediakiwi.Data;
using Sushi.Mediakiwi.Framework;
using Sushi.Mediakiwi.PageModules.ExportPage;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi
{
    public static class Extensions
    {
        public static IApplicationBuilder UseMediakiwi(this IApplicationBuilder app)
        {
            return app.UseMiddleware<Portal>();
        }

        public static void AddMediakiwi(this IServiceCollection services)
        {
            // Assign json section to config
            services.AddAuthentication()
                .AddScheme<MediaKiwiAuthenticationOptions, MediaKiwiAuthenticationHandler>(Common.AuthenticationScheme, null);
            
            services.AddSingleton<IPageModule, ExportPageModule>();

        }

        public class MediaKiwiAuthenticationOptions : AuthenticationSchemeOptions
        {
        }

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
                var currentVisitor = new VisitorManager(Context).Select();
                if (currentVisitor != null
                     && currentVisitor.ApplicationUserID.HasValue
                     && currentVisitor.ApplicationUserID.Value > 0
                     )
                {
                    var currentApplicationUser = await ApplicationUser.SelectOneAsync(currentVisitor.ApplicationUserID.Value, true).ConfigureAwait(false);

                    var claims = new List<Claim>();
                    claims.Add(new Claim(ClaimTypes.Name, currentApplicationUser.Displayname));
                    claims.Add(new Claim(ClaimTypes.Email, currentApplicationUser.Email));
                    var identity = new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie);

                    var principal = new ClaimsPrincipal(identity);
                    var ticket = new AuthenticationTicket(principal, Scheme.Name);
                    return AuthenticateResult.Success(ticket);
                }

                return AuthenticateResult.Fail("Invalid username or password");
            }
        }
    }
}
