using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Sushi.Mediakiwi.API.Transport
{
    public class Component
    {
        /// <summary>
        /// The title of this component
        /// </summary>
        [JsonPropertyName("title")]
        public string Title { get; set; }

        /// <summary>
        /// Can this component be moved 
        /// </summary>
        [JsonPropertyName("canMove")]
        public bool CanMove { get; set; }

        /// <summary>
        /// Can this component be deactivated
        /// </summary>
        [JsonPropertyName("canDeactivate")]
        public bool CanDeactivate { get; set; }

        /// <summary>
        /// Does this component belong to the header
        /// </summary>
        [JsonPropertyName("isHeader")]
        public bool IsHeader { get; set; }

        /// <summary>
        /// Does this component belong to the footer
        /// </summary>
        [JsonPropertyName("isFooter")]
        public bool IsFooter { get; set; }

        /// <summary>
        /// What is the sortorder (index) of this component
        /// </summary>
        [JsonPropertyName("sortOrder")]
        public int SortOrder { get; set; }

        /// <summary>
        /// The actual content for this component
        /// </summary>
        [JsonPropertyName("content")]
        public ICollection<ContentField> Content { get; set; } = new List<ContentField>();

        /// <summary>
        /// What is the Component Template ID for this component
        /// </summary>
        [JsonPropertyName("templateId")]
        public int TemplateID { get; set; }

        /// <summary>
        /// What is the Component Version ID for this component
        /// </summary>
        [JsonPropertyName("versionId")]
        public int VersionID { get; set; }
    }
}
