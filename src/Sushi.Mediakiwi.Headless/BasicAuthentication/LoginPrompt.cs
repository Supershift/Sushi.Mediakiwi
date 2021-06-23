using Microsoft.AspNetCore.Http;
using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Headless.BasicAuthentication
{
    public class LoginPrompt
    {
        private readonly RequestDelegate _next;
        private readonly Credential _credentials;


        public LoginPrompt()
        {
        }

        public LoginPrompt(
            RequestDelegate next,
            Credential credentials)
        {
            this._next = next;
            this._credentials = credentials;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // exclude Microsoft Frontdoor health check (extend with frontdoor identification
            // https://docs.microsoft.com/bs-cyrl-ba/azure/frontdoor/front-door-http-headers-protocol
            bool isexcluded = context.Request.Headers["X-FD-HealthProbe"] == "1" || context.Request.Method == "HEAD";
            if (!isexcluded)
            {
                //Only do the secondary auth if the user is already authenticated
                if (!context.User.Identity.IsAuthenticated)
                {
                    string authHeader = context.Request.Headers["Authorization"];
                    if (authHeader != null && authHeader.StartsWith("Basic "))
                    {
                        // Get the encoded username and password
                        var encodedUsernamePassword = authHeader.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries)[1]?.Trim();

                        // Decode from Base64 to string
                        var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));

                        // Split username and password
                        var username = decodedUsernamePassword.Split(':', 2)[0];
                        var password = decodedUsernamePassword.Split(':', 2)[1];

                        // Check if login is correct
                        if (IsAuthorized(username, password))
                        {

                            await _next.Invoke(context);
                            return;
                        }
                    }

                    // Return authentication type (causes browser to show login dialog)
                    context.Response.Headers["WWW-Authenticate"] = "Basic";

                    // Return unauthorized
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                }
                else
                {
                    await _next.Invoke(context);
                }
            }
        }

        private bool IsAuthorized(string username, string password) =>
            _credentials.Username == username && _credentials.Password == password;
    }
}