using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Sushi.Mediakiwi.API.Transport
{
    public class Notification
    {
        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("isError")]
        public bool IsError { get; set; }

        [JsonPropertyName("propertyNames")]
        public ICollection<string> PropertyNames { get; set; } = new List<string>();
    }
}
