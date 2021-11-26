using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Sushi.Mediakiwi.API.Transport.Responses
{
    public class GetSitesResponse : BasicResponse
    {
        [JsonPropertyName("items")]
        public ICollection<SiteItem> Items { get; set; } = new List<SiteItem>();
    }
}
