using System.Text.Json.Serialization;

namespace Sushi.Mediakiwi.API.Transport
{
    public class SiteItem
    {
        /// <summary>
        /// The site (channel) Identifier
        /// </summary>
        [JsonPropertyName("id")]
        public int ID { get; set; }

        /// <summary>
        /// The title of this site
        /// </summary>
        [JsonPropertyName("title")]
        public string Title { get; set; }
        
        /// <summary>
        /// The culture of this site
        /// </summary>
        [JsonPropertyName("culture")]
        public string Culture { get; set; }

        /// <summary>
        /// The weekstart of this site
        /// </summary>
        [JsonPropertyName("weekStart")]
        public int WeekStart { get; set; }
    }
}
