using System.Text.Json.Serialization;

namespace Sushi.Mediakiwi.API.Transport.Requests
{
    public class PostContentRequest : GetContentRequest
    {
        [JsonPropertyName("postedField")]
        public string PostedField { get; set; }
    }
}
