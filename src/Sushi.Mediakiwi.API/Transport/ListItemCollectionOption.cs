using System.Text.Json.Serialization;

namespace Sushi.Mediakiwi.API.Transport
{
    public class ListItemCollectionOption
    {
        [JsonPropertyName("text")]
        public string Text { get; set; }

        [JsonPropertyName("value")]
        public string Value { get; set; }

        [JsonPropertyName("isSelected")]
        public bool IsSelected { get; set; }

        [JsonPropertyName("isEnabled")]
        public bool IsEnabled { get; set; }
    }
}
