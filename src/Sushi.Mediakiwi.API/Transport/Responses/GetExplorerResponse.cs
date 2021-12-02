using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Sushi.Mediakiwi.API.Transport.Responses
{
    public class GetExplorerResponse
    {
        [JsonPropertyName("items")]
        public List<BrowseFolder> Items { get; set; } = new List<BrowseFolder>();
    }
}
