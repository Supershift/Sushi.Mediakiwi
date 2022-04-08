using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Sushi.Mediakiwi.API.Transport.Requests
{
    public class SetPasswordRequest
    {
        /// <summary>
        /// The email address to set the password for
        /// </summary>
        [JsonPropertyName("emailAddress")]
        [Required]
        public string EmailAddress { get; set; }

        /// <summary>
        /// The Reset GUID received in the password reset email
        /// </summary>
        [JsonPropertyName("resetGuid")]
        [Required]
        public string ResetGuid { get; set; }

        /// <summary>
        /// The new password to set
        /// </summary>
        [JsonPropertyName("password")]
        [Required]
        public string Password { get; set; }
    }
}
