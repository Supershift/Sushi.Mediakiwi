using System;
using System.Security.Principal;

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

    }
}
