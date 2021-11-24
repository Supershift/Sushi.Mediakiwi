using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Sushi.Mediakiwi.API.Transport.Requests
{
    public class LoginRequest
    {
        [JsonPropertyName("emailAddress")]
        [Required]
        public string EmailAddress { get; set; }
        
        [JsonPropertyName("password")]
        [Required]
        public string Password { get; set; }

        [JsonPropertyName("apiKey")]
        [Required]
        public string ApiKey { get; set; }
    }
}
