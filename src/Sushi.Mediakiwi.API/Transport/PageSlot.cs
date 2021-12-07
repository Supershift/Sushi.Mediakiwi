using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Sushi.Mediakiwi.API.Transport
{
    public class PageSlot
    {
        /// <summary>
        /// The title for this page slot
        /// </summary>
        [JsonPropertyName("title")]
        public string Title { get; set; }

        /// <summary>
        /// The sortorder (index) for this page slot
        /// </summary>
        [JsonPropertyName("sortOrder")]
        public int SortOrder { get; set; }

        /// <summary>
        /// All components belonging to this page slot
        /// </summary>
        [JsonPropertyName("components")]
        public ICollection<Component> Components { get; set; } = new List<Component>();
    }
}
