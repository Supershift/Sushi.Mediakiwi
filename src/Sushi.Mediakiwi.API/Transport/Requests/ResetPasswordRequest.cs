using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Sushi.Mediakiwi.API.Transport.Requests
{
    public class ResetPasswordRequest
    {
        [JsonPropertyName("emailAddress")]
        [Required]
        public string EmailAddress { get; set; }
    }
}
