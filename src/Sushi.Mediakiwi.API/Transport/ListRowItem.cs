using System.Text.Json.Serialization;

namespace Sushi.Mediakiwi.API.Transport
{
    public class ListRowItem
    {
        [JsonPropertyName("value")]
        public string Value { get; set; }

        [JsonPropertyName("vueType")]
        public VueTypeEnum VueType { get; set; }

        [JsonPropertyName("canWrap")]
        public bool CanWrap { get; set; }
    }
}
