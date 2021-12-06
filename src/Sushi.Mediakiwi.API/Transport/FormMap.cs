using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Sushi.Mediakiwi.API.Transport
{
    public class FormMap
    {
        /// <summary>
        /// The C# classname for this formmap
        /// </summary>
        [JsonPropertyName("className")]
        public string ClassName { get; set; }

        /// <summary>
        /// The fields within this formmap
        /// </summary>
        [JsonPropertyName("fields")]
        public List<ContentField> Fields { get; set; } = new List<ContentField>();

        /// <summary>
        /// The buttons within this formmap
        /// </summary>
        [JsonPropertyName("buttons")]
        public List<ButtonField> Buttons { get; set; } = new List<ButtonField>();

        /// <summary>
        /// The title for this formmap
        /// </summary>
        [JsonPropertyName("title")]
        public string Title { get; set; }
    }
}
