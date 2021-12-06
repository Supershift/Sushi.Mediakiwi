using System.Text.Json.Serialization;

namespace Sushi.Mediakiwi.API.Transport
{
    public class ResourceItem
    {
        /// <summary>
        /// The type of resource (JS = 1, CSS = 2, HTML = 3)
        /// </summary>
        [JsonPropertyName("type")]
        public int Type { get; set; }
                       
        /// <summary>
        /// Where should this resource be added (HEADER = 1, BODY_NESTED = 2, BODY_BELOW = 3)
        /// </summary>
        [JsonPropertyName("position")]
        public int Position { get; set; }

        /// <summary>
        /// Should this resource be loaded Synchronous
        /// </summary>
        [JsonPropertyName("isSync")]
        public bool IsSync { get; set; }

        /// <summary>
        /// The path to the resource
        /// </summary>
        [JsonPropertyName("path")]
        public string Path { get; set; }

        /// <summary>
        /// The Source code for this resource
        /// </summary>
        [JsonPropertyName("sourceCode")]
        public string SourceCode { get; set; }
    }
}
