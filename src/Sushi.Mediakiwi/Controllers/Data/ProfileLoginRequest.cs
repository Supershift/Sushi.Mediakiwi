using System.Runtime.Serialization;

namespace Sushi.Mediakiwi.Controllers.Data
{
    [DataContract]
    public class ProfileLoginRequest
    {
        /// <summary>
        /// Either e-mail address or username should be set
        /// </summary>
        [DataMember(Name = "emailAddress")]
        public string EmailAddress { get; set; }

        /// <summary>
        /// Either e-mail address or username should be set
        /// </summary>
        [DataMember(Name = "userName")]
        public string UserName { get; set; }

        /// <summary>
        /// The Password for this profile
        /// </summary>
        [DataMember(Name = "password")]
        public string Password { get; set; }

    }
}
