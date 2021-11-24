using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Sushi.Mediakiwi.API.Transport.Responses
{
    public class GetTopNavigationResponse : BasicResponse
    {
        [JsonPropertyName("logoUrl")]
        public string LogoUrl { get; set; }

        [JsonPropertyName("homeUrl")]
        public string HomeUrl { get; set; }

        [JsonPropertyName("items")]
        public ICollection<NavigationItem> Items { get; set; } = new List<NavigationItem>();
    }
}
