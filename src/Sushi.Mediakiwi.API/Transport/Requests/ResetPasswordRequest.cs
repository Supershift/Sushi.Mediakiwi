using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Sushi.Mediakiwi.API.Transport.Requests
{
    public class ResetPasswordRequest
    {
        /// <summary>
        /// The E-mail address that requested a password reset
        /// </summary>
        [JsonPropertyName("emailAddress")]
        [Required]
        public string EmailAddress { get; set; }
        
        /// <summary>
        /// Should we also send an Email to the user ? (Default = TRUE)
        /// </summary>
        [JsonPropertyName("sendEmail")]
        public bool SendEmail { get; set; } = true;
    }
}
