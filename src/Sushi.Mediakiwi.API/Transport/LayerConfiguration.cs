using System.Text.Json.Serialization;

namespace Sushi.Mediakiwi.API.Transport
{
    public class LayerConfiguration
    {
        [JsonPropertyName("width")]
        public int Width { get; set; }

        [JsonPropertyName("height")]
        public int Height { get; set; }

        [JsonPropertyName("widthUnitType")]
        public UnitTypeEnum WidthUnitType { get; set; }

        [JsonPropertyName("heightUnitType")]
        public UnitTypeEnum HeightUnitType { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("hasScrollbar")]
        public bool HasScrollbar { get; set; }
    }
}
