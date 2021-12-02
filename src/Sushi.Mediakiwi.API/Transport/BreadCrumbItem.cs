using System.Text.Json.Serialization;

namespace Sushi.Mediakiwi.API.Transport
{
    public class BreadCrumbItem
    {
        /// <summary>
        /// The label for this BreadCrumb item
        /// </summary>
        [JsonPropertyName("text")]
        public string Text { get; set; }

        /// <summary>
        /// The Url for this BreadCrumb item
        /// </summary>
        [JsonPropertyName("href")]
        public string Href { get; set; }

        /// <summary>
        /// Is this a Back indicator
        /// </summary>
        [JsonPropertyName("isBack")]
        public bool IsBack { get; set; }
    }
}
