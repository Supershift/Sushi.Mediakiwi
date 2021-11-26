using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Sushi.Mediakiwi.API.Transport
{
    public class BrowseFolder
    {
        [JsonPropertyName("id")]
        public int ID { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("href")]
        public string Href { get; set; }

        [JsonPropertyName("items")]
        public ICollection<BrowseItem> Items { get; set; } = new List<BrowseItem>();
    }
}
