using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Sushi.Mediakiwi.API.Transport
{
    public class ContentField
    {
        /// <summary>
        /// What is the type of content this Field represents
        /// </summary>
        [JsonPropertyName("contentType")]
        public ContentTypeEnum ContentType { get; set; }

        /// <summary>
        /// The name of the property connected to this Field
        /// </summary>
        [JsonPropertyName("propertyName")]
        public string PropertyName { get; set; }

        /// <summary>
        /// The C# type of the property connected to this Field
        /// </summary>
        [JsonPropertyName("propertyType")]
        public string PropertyType { get; set; }

        /// <summary>
        /// The title (label) for this Field
        /// </summary>
        [JsonPropertyName("title")]
        public string Title { get; set; }

        /// <summary>
        /// What is the type that should be used on the JS clientside 
        /// </summary>
        [JsonPropertyName("vueType")]
        public VueTypeEnum VueType { get; set; }

        /// <summary>
        /// The Expression of this Field, is it Full width, alternating, left or right
        /// </summary>
        [JsonPropertyName("expression")]
        public int Expression { get; set; }

        /// <summary>
        /// The value of this Field, could be one of :
        /// Integer, Decimal, Float, Double, String, String[]
        /// </summary>
        [JsonPropertyName("value")]
        public object Value { get; set; }

        /// <summary>
        /// The options available for this field (for dropdowns and radios and such)
        /// </summary>
        [JsonPropertyName("options")]
        public List<ListItemCollectionOption> Options { get; set; }

        /// <summary>
        /// The CSS classname for this field
        /// </summary>
        [JsonPropertyName("className")]
        public string ClassName { get; set; }

        /// <summary>
        /// The javascript event attached to this field
        /// </summary>
        [JsonPropertyName("event")]
        public JSEventEnum Event { get; set; } = JSEventEnum.None;
        
        /// <summary>
        /// Does this belong to the Top (0) or Bottom (1)
        /// </summary>
        [JsonPropertyName("section")]
        public int Section { get; set; }

        /// <summary>
        /// Is this a hidden field ?
        /// </summary>
        [JsonPropertyName("isHidden")]
        public bool IsHidden { get; set; }

        /// <summary>
        /// What is the groupName for this field (applicable on radios)
        /// </summary>
        [JsonPropertyName("groupName")]
        public string Groupname { get; set; }

        /// <summary>
        /// The value suffix for this field
        /// </summary>
        [JsonPropertyName("suffix")]
        public string Suffix { get; set; }

        /// <summary>
        /// The value prefix for this field
        /// </summary>
        [JsonPropertyName("prefix")]
        public string Prefix { get; set; }

        /// <summary>
        /// To which formmap does this field belong
        /// </summary>
        [JsonPropertyName("formSection")]
        public string FormSection { get; set; }

        /// <summary>
        /// Can this field toggle a section
        /// </summary>
        [JsonPropertyName("canToggleSection")]
        public bool CanToggleSection { get; set; }

        /// <summary>
        /// Can this field delete a section
        /// </summary>
        [JsonPropertyName("canDeleteSection")]
        public bool CanDeleteSection { get; set; }
        
        /// <summary>
        /// Can this field toggle the default closed 
        /// </summary>
        [JsonPropertyName("toggleDefaultClosed")]
        public bool ToggleDefaultClosed { get; set; }

        /// <summary>
        /// Is this field readonly
        /// </summary>
        [JsonPropertyName("isReadOnly")]
        public bool IsReadOnly { get; set; }

        /// <summary>
        /// The interactive help for this field
        /// </summary>
        [JsonPropertyName("helpText")]
        public string HelpText { get; set; }

        /// <summary>
        /// The layer configuration for this field (applicable on buttons)
        /// </summary>
        [JsonPropertyName("layerConfiguration")]
        public LayerConfiguration LayerConfiguration { get; set; }

        /// <summary>
        /// Is this field mandatory ?
        /// </summary>
        [JsonPropertyName("isMandatory")]
        public bool IsMandatory { get; set; }

        /// <summary>
        /// The maximum value length for this field
        /// </summary>
        [JsonPropertyName("maxLength")]
        public int MaxLength { get; set; }

        /// <summary>
        /// Does this field trigger a postback
        /// </summary>
        [JsonPropertyName("isAutoPostback")]
        public bool IsAutoPostback { get; set; }

        /// <summary>
        /// Any additional settings that apply for this field.
        /// </summary>
        [JsonPropertyName("settings")]
        public Dictionary<string, string> Settings { get; internal set; }
    }
}
