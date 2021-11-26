using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Sushi.Mediakiwi.API.Transport
{
   public class PageSlot
    {
        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("sortOrder")]
        public int SortOrder { get; set; }

        [JsonPropertyName("components")]
        public ICollection<Component> Components { get; set; } = new List<Component>();
    }
}
