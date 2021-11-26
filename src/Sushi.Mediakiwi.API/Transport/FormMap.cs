using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Sushi.Mediakiwi.API.Transport
{
    public class FormMap
    {
        [JsonPropertyName("className")]
        public string ClassName { get; set; }

        [JsonPropertyName("fields")]
        public ICollection<ContentField> Fields { get; set; } = new List<ContentField>();

        [JsonPropertyName("buttons")]
        public ICollection<ButtonField> Buttons { get; set; } = new List<ButtonField>();

        [JsonPropertyName("title")]
        public string Title { get; set; }
    }
}
