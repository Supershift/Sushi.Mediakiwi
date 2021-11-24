using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Sushi.Mediakiwi.API.Transport.Responses
{
    public class GetSideNavigationResponse : BasicResponse
    {
        [JsonPropertyName("items")]
        public ICollection<NavigationItem> Items { get; set; } = new List<NavigationItem>();
    }
}
