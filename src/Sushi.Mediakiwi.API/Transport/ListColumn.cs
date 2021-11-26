using System.Text.Json.Serialization;

namespace Sushi.Mediakiwi.API.Transport
{
    public class ListColumn
    {
        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("width")]
        public int Width { get; set; }

        [JsonPropertyName("align")]
        public int Align { get; set; }

        [JsonPropertyName("isHidden")]
        public bool IsHidden { get; set; }

        [JsonPropertyName("prefix")]
        public string Prefix { get; set; }

        [JsonPropertyName("suffix")]
        public string Suffix { get; set; }

        [JsonPropertyName("helpText")]
        public string helpText { get; set; }

        [JsonPropertyName("isSum")]
        public bool IsSum { get; set; }

        [JsonPropertyName("isAverage")]
        public bool IsAverage { get; set; }
    }
}
