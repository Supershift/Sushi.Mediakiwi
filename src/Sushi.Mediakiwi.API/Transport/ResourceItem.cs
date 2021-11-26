using System.Text.Json.Serialization;

namespace Sushi.Mediakiwi.API.Transport
{
    public class ResourceItem
    {
        [JsonPropertyName("type")]
        public int Type { get; set; }

        [JsonPropertyName("position")]
        public int Position { get; set; }

        [JsonPropertyName("isSync")]
        public bool IsSync { get; set; }

        [JsonPropertyName("path")]
        public string Path { get; set; }

        [JsonPropertyName("sourceCode")]
        public string SourceCode { get; set; }
    }
}
