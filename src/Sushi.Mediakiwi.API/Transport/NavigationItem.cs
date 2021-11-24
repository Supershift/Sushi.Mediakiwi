using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Sushi.Mediakiwi.API.Transport
{
    public class NavigationItem
    {
        [JsonPropertyName("text")]
        public string Text { get; set; }

        [JsonPropertyName("href")]
        public string Href { get; set; }

        [JsonPropertyName("iconClass")]
        public string IconClass { get; set; }

        [JsonPropertyName("isHighlighted")]
        public bool IsHighlighted { get; set; }

        [JsonPropertyName("isBack")]
        public bool IsBack { get; set; }

        [JsonPropertyName("items")]
        public ICollection<NavigationItem> Items { get; set; } = new List<NavigationItem>();

        [JsonIgnore]
        public int ItemID { get; set; }
    }
}
