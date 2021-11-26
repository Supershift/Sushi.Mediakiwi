using System.Text.Json.Serialization;

namespace Sushi.Mediakiwi.API.Transport
{
    public class BreadCrumbItem
    {
        [JsonPropertyName("text")]
        public string Text { get; set; }

        [JsonPropertyName("href")]
        public string Href { get; set; }

        [JsonPropertyName("isBack")]
        public bool IsBack { get; set; }
    }
}
