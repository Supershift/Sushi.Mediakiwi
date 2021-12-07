using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Sushi.Mediakiwi.API.Transport
{
    public class BrowseItem
    {
        /// <summary>
        /// The identifier for this item
        /// </summary>
        [JsonPropertyName("id")]
        public int ID { get; set; }

        /// <summary>
        /// The title for this item
        /// </summary>
        [JsonPropertyName("title")]
        public string Title { get; set; }

        /// <summary>
        /// The URL for this item
        /// </summary>
        [JsonPropertyName("href")]
        public string Href { get; set; }

        /// <summary>
        /// If this item has any kind of badge content (like a counter) it will be available here
        /// </summary>
        [JsonPropertyName("badgeContent")]
        public string BadgeContent { get; set; }

        /// <summary>
        /// Contains css classes specific for this item
        /// </summary>
        [JsonPropertyName("iconClasses")]
        public ICollection<string> IconClasses { get; set; } = new List<string>();
    }
}
