using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Sushi.Mediakiwi.API.Transport
{
    public class NavigationItem
    {
        /// <summary>
        /// The text (label) for this navigation item
        /// </summary>
        [JsonPropertyName("text")]
        public string Text { get; set; }

        /// <summary>
        /// The URL for this navigation item
        /// </summary>
        [JsonPropertyName("href")]
        public string Href { get; set; }

        /// <summary>
        /// The CSS class for this navigation item
        /// </summary>
        [JsonPropertyName("iconClass")]
        public string IconClass { get; set; }

        /// <summary>
        /// Is this navigation item highlighted
        /// </summary>
        [JsonPropertyName("isHighlighted")]
        public bool IsHighlighted { get; set; }

        /// <summary>
        /// Is this navigation item a back button
        /// </summary>
        [JsonPropertyName("isBack")]
        public bool IsBack { get; set; }

        /// <summary>
        /// If this item has any kind of badge content (like a counter) it will be available here
        /// </summary>
        [JsonPropertyName("badgeContent")]
        public string BadgeContent { get; set; }

        /// <summary>
        /// All child navigation items for this navigation item
        /// </summary>
        [JsonPropertyName("items")]
        public ICollection<NavigationItem> Items { get; set; } = new List<NavigationItem>();

        /// <summary>
        /// The Item ID for this navigation item
        /// </summary>
        [JsonIgnore]
        public int ItemID { get; set; }
    }
}
