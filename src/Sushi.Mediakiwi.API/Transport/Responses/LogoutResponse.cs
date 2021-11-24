using System.Text.Json.Serialization;

namespace Sushi.Mediakiwi.API.Transport.Responses
{
    public class LogoutResponse : BasicResponse
    {
        [JsonPropertyName("targetUrl")]
        public string TargetUrl { get; set; }
    }
}
