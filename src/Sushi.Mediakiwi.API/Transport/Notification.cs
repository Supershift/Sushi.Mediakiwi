using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Sushi.Mediakiwi.API.Transport
{
    public class Notification
    {
        /// <summary>
        /// The notification message
        /// </summary>
        [JsonPropertyName("message")]
        public string Message { get; set; }

        /// <summary>
        /// Is this notification an error ?
        /// </summary>
        [JsonPropertyName("isError")]
        public bool IsError { get; set; }

        /// <summary>
        /// The faulty property names 
        /// </summary>
        [JsonPropertyName("propertyNames")]
        public ICollection<string> PropertyNames { get; set; } = new List<string>();
    }
}
