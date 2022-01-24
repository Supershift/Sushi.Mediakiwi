using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Interfaces
{
    public abstract class MediaKiwiProfileManager : CookieAuthenticationEvents
    {
        public virtual string CookieName { get { return ""; } }

        public virtual string SchemeName { get { return ""; } }

        /// <summary>
        /// Sign in by using E-mail address and password
        /// </summary>
        /// <param name="context">The http context</param>
        /// <param name="emailAddress">The profile's e-mail address</param>
        /// <param name="password">The profile's password</param>
        /// <returns><c>true</c> when successful, <c>false</c> when unsuccesful</returns>
        public virtual async Task<bool> SignInByEmailPasswordAsync(HttpContext context, string emailAddress, string password) 
        {
            return false;
        }

        /// <summary>
        /// Sign in by using username and password
        /// </summary>
        /// <param name="context">The http context</param>
        /// <param name="userName">The profile's username</param>
        /// <param name="password">The profile's password</param>
        /// <returns><c>true</c> when successful, <c>false</c> when unsuccesful</returns>
        public virtual async Task<bool> SignInByUsernamePasswordAsync(HttpContext context, string userName, string password)
        {
            return false;
        }

        /// <summary>
        /// Sign in by using E-mail address and password
        /// </summary>
        /// <param name="context">The http context</param>
        /// <param name="emailAddress">The profile's e-mail address</param>
        /// <param name="password">The profile's password, can be left empty to generate a random password</param>
        /// <returns><c>true</c> when successful, <c>false</c> when unsuccesful</returns>
        public virtual async Task<bool> CreateByEmailAsync(HttpContext context, string emailAddress, string password)
        {
            return false;
        }

        /// <summary>
        /// Sign in by using username and password
        /// </summary>
        /// <param name="context">The http context</param>
        /// <param name="userName">The profile's username</param>
        /// <param name="password">The profile's password, can be left empty to generate a random password</param>
        /// <returns><c>true</c> when successful, <c>false</c> when unsuccesful</returns>
        public virtual async Task<bool> CreateByUsernameAsync(HttpContext context, string userName, string password)
        {
            return false;
        }

        /// <summary>
        /// Sign in by using a custom Object
        /// </summary>
        /// <param name="context">The http context</param>
        /// <param name="customObject">The custom object to use for signing in</param>
        /// <returns><c>true</c> when successful, <c>false</c> when unsuccesful</returns>
        public virtual async Task<bool> SignInAsync(HttpContext context, object customObject)
        {
            return false;
        }
    }
}
