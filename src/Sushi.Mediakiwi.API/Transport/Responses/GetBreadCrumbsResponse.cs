using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Sushi.Mediakiwi.API.Transport.Responses
{
    public class GetBreadCrumbsResponse
    {
        [JsonPropertyName("items")]
        public ICollection<BreadCrumbItem> Items { get; set; } = new List<BreadCrumbItem>();
    }
}
