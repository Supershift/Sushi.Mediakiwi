using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Sushi.Mediakiwi.Data;

namespace Wim.Utilities
{
    public class PortalAuthentication
    {
        public HttpCookie SetAuthentication(IApplicationUser user, bool isSecure = true)
        {
            DateTime expiration = DateTime.Now.AddDays(1);
            string[] data = new[] { user.GUID.ToString() };
            var ticket = new FormsAuthenticationTicket(
               1,                                       // ticket version
               user.Displayname,                        // authenticated username
               DateTime.Now,                            // issueDate
               expiration,                              // expiryDate
               user.RememberMe,                         // true to persist across browser sessions
               string.Join("|", data),                  // can be used to store additional user data
               FormsAuthentication.FormsCookiePath);    // the path for the cookie

            // Encrypt the ticket using the machine key
            string encryptedTicket = FormsAuthentication.Encrypt(ticket);

            // Add the cookie to the request to save it
            HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
            cookie.HttpOnly = false;
            cookie.Expires = expiration;
            cookie.Secure = isSecure;
            cookie.Shareable = false;
            if (CommonConfiguration.COOKIEDOMAIN_LOCALHOST_AS_NULL && cookie?.Domain?.ToLowerInvariant() == "localhost")
                cookie.Domain = null;

            return cookie;
        }
    }
}
