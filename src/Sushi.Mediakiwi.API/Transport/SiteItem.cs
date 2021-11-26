using System.Text.Json.Serialization;

namespace Sushi.Mediakiwi.API.Transport
{
    public class SiteItem
    {
        [JsonPropertyName("id")]
        public int ID { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }
        
        [JsonPropertyName("culture")]
        public string Culture { get; set; }

        [JsonPropertyName("weekStart")]
        public int WeekStart { get; set; }
    }
}
