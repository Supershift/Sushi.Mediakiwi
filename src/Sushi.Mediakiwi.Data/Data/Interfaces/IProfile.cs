using System;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Data
{
    public interface IProfile : IIdentity
    {
        /// <summary>
        /// The Profile ID
        /// </summary>
        int ID { get; set; }

        /// <summary>
        /// The Profile GUID
        /// </summary>
        Guid GUID { get; set; }

        /// <summary>
        /// The Profile Creation datetime
        /// </summary>
        DateTime Created { get; set; }

        /// <summary>
        /// Remember e-mail address 
        /// </summary>
        bool? RememberMe { get; set; }

        /// <summary>
        /// The Profile e-mail address
        /// </summary>
        string Email { get; set; }

        /// <summary>
        /// The Profile username
        /// </summary>
        string Username { get; set; }

        /// <summary>
        /// The Encrypted Profile Password
        /// </summary>
        string Password { get; set; }

        /// <summary>
        /// Any custom data for this profile
        /// </summary>
        CustomData Data { get; set; }

        /// <summary>
        /// Encrypts the provided password and sets it to the Password value
        /// </summary>
        /// <param name="cleartextPassword">The cleartext password</param>
        /// <returns></returns>
        Task SetPasswordAsync(string cleartextPassword);

        /// <summary>
        /// Checks the provided cleartext password against the saved (encrypted) password
        /// </summary>
        /// <param name="cleartextPassword">The cleartext password</param>
        /// <returns><c>true</c> when the passwords match. <c>false</c> when they do not match</returns>
        Task<bool> CheckPasswordAsync(string cleartextPassword);

    }
}
