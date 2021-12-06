using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Sushi.Mediakiwi.API.Transport
{
    public class ListRow
    {
        /// <summary>
        /// The URL for this List Row
        /// </summary>
        [JsonPropertyName("href")]
        public string Href { get; set; }

        /// <summary>
        /// The ID for this List Row
        /// </summary>
        [JsonPropertyName("id")]
        public int ID { get; set; }

        /// <summary>
        /// The Title for this List Row
        /// </summary>
        [JsonPropertyName("title")]
        public string Title { get; set; }

        /// <summary>
        /// The items within this List Row
        /// </summary>
        [JsonPropertyName("items")]
        public ICollection<ListRowItem> Items { get; set; } = new List<ListRowItem>();
    }
}
