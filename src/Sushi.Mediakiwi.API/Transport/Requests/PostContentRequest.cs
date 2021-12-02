using System.Text.Json.Serialization;

namespace Sushi.Mediakiwi.API.Transport.Requests
{
    public class PostContentRequest : GetContentRequest
    {
        /// <summary>
        /// The name of the field that caused this post to happen
        /// </summary>
        [JsonPropertyName("postedField")]
        public string PostedField { get; set; }
    }
}
