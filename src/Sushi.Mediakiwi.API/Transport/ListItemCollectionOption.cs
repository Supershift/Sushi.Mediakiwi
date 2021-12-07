using System.Text.Json.Serialization;

namespace Sushi.Mediakiwi.API.Transport
{
    public class ListItemCollectionOption
    {
        /// <summary>
        /// The text (label) for this option
        /// </summary>
        [JsonPropertyName("text")]
        public string Text { get; set; }

        /// <summary>
        /// The value for this option
        /// </summary>
        [JsonPropertyName("value")]
        public string Value { get; set; }

        /// <summary>
        /// Is this option selected ?
        /// </summary>
        [JsonPropertyName("isSelected")]
        public bool IsSelected { get; set; }

        /// <summary>
        /// Is this option enabled ?
        /// </summary>
        [JsonPropertyName("isEnabled")]
        public bool IsEnabled { get; set; }
    }
}
