using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Sushi.Mediakiwi.API.Transport
{
    public class ContentField
    {
        [JsonPropertyName("contentType")]
        public ContentTypeEnum ContentType { get; set; }

        [JsonPropertyName("propertyName")]
        public string PropertyName { get; set; }

        [JsonPropertyName("propertyType")]
        public string PropertyType { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("vueType")]
        public VueTypeEnum VueType { get; set; }

        [JsonPropertyName("expression")]
        public int Expression { get; set; }

        [JsonPropertyName("value")]
        public string Value { get; set; }

        [JsonPropertyName("options")]
        public ICollection<ListItemCollectionOption> Options { get; set; } = new List<ListItemCollectionOption>();

        [JsonPropertyName("className")]
        public string ClassName { get; set; }

        [JsonPropertyName("event")]
        public JSEventEnum Event { get; set; } = JSEventEnum.None;
        
        [JsonPropertyName("section")]
        public int Section { get; set; }

        [JsonPropertyName("isHidden")]
        public bool IsHidden { get; set; }

        [JsonPropertyName("groupName")]
        public string Groupname { get; set; }

        [JsonPropertyName("suffix")]
        public string Suffix { get; set; }

        [JsonPropertyName("prefix")]
        public string Prefix { get; set; }

        [JsonPropertyName("formSection")]
        public string FormSection { get; set; }

        [JsonPropertyName("canToggleSection")]
        public bool CanToggleSection { get; set; }

        [JsonPropertyName("canDeleteSection")]
        public bool CanDeleteSection { get; set; }
        
        [JsonPropertyName("toggleDefaultClosed")]
        public bool ToggleDefaultClosed { get; set; }

        [JsonPropertyName("isReadOnly")]
        public bool IsReadOnly { get; set; }

        [JsonPropertyName("helpText")]
        public string HelpText { get; set; }

        [JsonPropertyName("layerConfiguration")]
        public LayerConfiguration LayerConfiguration { get; set; }

        [JsonPropertyName("isMandatory")]
        public bool IsMandatory { get; set; }

        [JsonPropertyName("maxLength")]
        public int MaxLength { get; set; }

        [JsonPropertyName("isAutoPostback")]
        public bool IsAutoPostback { get; set; }
    }
}
