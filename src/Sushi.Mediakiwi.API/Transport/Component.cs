using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Sushi.Mediakiwi.API.Transport
{
    public class Component
    {
        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("canMove")]
        public bool CanMove { get; set; }

        [JsonPropertyName("canDeactivate")]
        public bool CanDeactivate { get; set; }

        [JsonPropertyName("isHeader")]
        public bool IsHeader { get; set; }

        [JsonPropertyName("isFooter")]
        public bool IsFooter { get; set; }

        [JsonPropertyName("sortOrder")]
        public int SortOrder { get; set; }

        [JsonPropertyName("content")]
        public ICollection<ContentField> Content { get; set; } = new List<ContentField>();

        [JsonPropertyName("templateId")]
        public int TemplateID { get; set; }

        [JsonPropertyName("versionId")]
        public int VersionID { get; set; }
    }
}
