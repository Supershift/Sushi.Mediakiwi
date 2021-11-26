using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Sushi.Mediakiwi.API.Transport.Responses
{
    public class GetPageResponse
    {
        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("completePath")]
        public string CompletePath { get; set; }

        [JsonPropertyName("slots")]
        public ICollection<PageSlot> Slots { get; set; } = new List<PageSlot>();

        [JsonPropertyName("isPublished")]
        public bool IsPublished { get; set; }

        [JsonPropertyName("id")]
        public int ID { get; set; }
    }
}
