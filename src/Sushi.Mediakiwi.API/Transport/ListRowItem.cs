using System.Text.Json.Serialization;

namespace Sushi.Mediakiwi.API.Transport
{
    public class ListRowItem
    {
        /// <summary>
        /// The value for this Row Item 
        /// </summary>
        [JsonPropertyName("value")]
        public string Value { get; set; }

        /// <summary>
        /// What is the type that should be used on the JS clientside  
        /// </summary>
        [JsonPropertyName("vueType")]
        public VueTypeEnum VueType { get; set; }

        /// <summary>
        /// Can this item wrap ?
        /// </summary>
        [JsonPropertyName("canWrap")]
        public bool CanWrap { get; set; }
    }
}
