using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Sushi.Mediakiwi.API.Transport
{
    public class BrowseFolder
    {
        /// <summary>
        /// The unique identifier for this folder
        /// </summary>
        [JsonPropertyName("id")]
        public int ID { get; set; }

        /// <summary>
        /// The title for this folder
        /// </summary>
        [JsonPropertyName("title")]
        public string Title { get; set; }

        /// <summary>
        /// The description for this folder
        /// </summary>
        [JsonPropertyName("description")]
        public string Description { get; set; }

        /// <summary>
        /// The URL for this folder
        /// </summary>
        [JsonPropertyName("href")]
        public string Href { get; set; }

        /// <summary>
        /// Contains css classes specific for this folder
        /// </summary>
        [JsonPropertyName("iconClasses")]
        public ICollection<string> IconClasses { get; set; } = new List<string>();

        /// <summary>
        /// All items belonging to this folder
        /// </summary>
        [JsonPropertyName("items")]
        public ICollection<BrowseItem> Items { get; set; } = new List<BrowseItem>();
    }
}
