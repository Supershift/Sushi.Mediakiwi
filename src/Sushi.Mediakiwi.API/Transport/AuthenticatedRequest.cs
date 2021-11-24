using System.Text.Json.Serialization;

namespace Sushi.Mediakiwi.API.Transport
{
    public abstract class AuthenticatedRequest
    {
        [JsonPropertyName("currentSiteId")]
        public int CurrentSiteID { get; set; }
    }
}
