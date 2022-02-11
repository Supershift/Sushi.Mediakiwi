using System.Text.Json.Serialization;

namespace Sushi.Mediakiwi.API.Transport.Requests
{
    public class GetTopNavigationRequest : AuthenticatedRequest
    {
        [JsonPropertyName("groupTag")]
        public string GroupTag { get; set; }
    }
}
