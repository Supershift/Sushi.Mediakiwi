using System.Text.Json.Serialization;

namespace Sushi.Mediakiwi.Headless.Data.TrackingScript
{
    public class TrackingScriptReplacementItem
    {
        [JsonPropertyName("code")]
        public string Code { get; set; }
        
        [JsonPropertyName("fallBack")]
        public string Fallback { get; set; }
        
        [JsonPropertyName("mustBeFilled")]
        public bool MustBeFilled { get; set; }

        [JsonPropertyName("value")]
        public string Value { get; set; }
    }
}
