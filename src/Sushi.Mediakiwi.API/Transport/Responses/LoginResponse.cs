using System.Text.Json.Serialization;

namespace Sushi.Mediakiwi.API.Transport.Responses
{
    public class LoginResponse : BasicResponse
    {
        [JsonPropertyName("targetUrl")]
        public string TargetUrl { get; set; }

        [JsonPropertyName("userEmail")]
        public string UserEmail { get; set; }

        [JsonPropertyName("userName")]
        public string UserName { get; set; }

        [JsonPropertyName("userAvatarUrl")]
        public string UserAvatarUrl { get; set; }
    }
}
