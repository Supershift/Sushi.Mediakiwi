using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Sushi.Mediakiwi.API.Transport
{
   public class ListRow
    {
        [JsonPropertyName("href")]
        public string Href { get; set; }

        [JsonPropertyName("id")]
        public int ID { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("items")]
        public ICollection<ListRowItem> Items { get; set; } = new List<ListRowItem>();
    }
}
