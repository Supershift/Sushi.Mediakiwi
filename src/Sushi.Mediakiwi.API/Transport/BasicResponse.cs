using System.Net;
using System.Text.Json.Serialization;

namespace Sushi.Mediakiwi.API.Transport
{
    public abstract class BasicResponse
    {
        [JsonPropertyName("statusCode")]
        public HttpStatusCode StatusCode { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }
    }
}
