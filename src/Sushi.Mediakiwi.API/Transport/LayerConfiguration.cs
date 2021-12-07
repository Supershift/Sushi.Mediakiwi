using System.Text.Json.Serialization;

namespace Sushi.Mediakiwi.API.Transport
{
    public class LayerConfiguration
    {
        /// <summary>
        /// The height of the popup
        /// </summary>
        [JsonPropertyName("width")]
        public int Width { get; set; }

        /// <summary>
        /// The width of the popup
        /// </summary>
        [JsonPropertyName("height")]
        public int Height { get; set; }

        /// <summary>
        /// Is the supplied width in Pixels or Percentage ?
        /// </summary>
        [JsonPropertyName("widthUnitType")]
        public UnitTypeEnum WidthUnitType { get; set; }

        /// <summary>
        /// Is the supplied height in Pixels or Percentage ?
        /// </summary>
        [JsonPropertyName("heightUnitType")]
        public UnitTypeEnum HeightUnitType { get; set; }

        /// <summary>
        /// The title of this popup
        /// </summary>
        [JsonPropertyName("title")]
        public string Title { get; set; }

        /// <summary>
        /// Does this popup has a scrollbar ?
        /// </summary>
        [JsonPropertyName("hasScrollbar")]
        public bool HasScrollbar { get; set; }
    }
}
