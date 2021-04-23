using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Sushi.Mediakiwi.Headless.Data
{
    /// <summary>
    /// Component content
    /// </summary>
    [DataContract]
    public class ContentComponent
    {
        public ContentComponent Clone()
        {
            var clone = new ContentComponent
            {
                ComponentName = ComponentName,
                ComponentID = ComponentID,
                SortOrder = SortOrder,
                Title = Title,
                Slot = Slot,
                IsShared = IsShared
            };

            if (Content != null && Content.Any())
            {
                clone.Content = new Dictionary<string, ContentItem>();
                foreach (var key in Content.Keys)
                {
                    clone.Content.Add(key, Content[key]);
                }
            }
            return clone;

        }

        public ContentComponent()
        {
            Content = new Dictionary<string, ContentItem>();
            Nested = new List<ContentComponent>();
        }

        /// <summary>
        /// The path (source tag) of the component
        /// </summary>
        [DataMember(Name = "componentName")]
        public string ComponentName { get; set; }
        
        /// <summary>
        /// The Component ID 
        /// </summary>
        [DataMember(Name = "componentId")]
        public int ComponentID { get; set; }

        /// <summary>
        /// The sortorder for this SortOrder
        /// </summary>
        [DataMember(Name = "sortOrder")]
        public int SortOrder { get; set; }

        /// <summary>
        /// For which slot is this component relevant
        /// </summary>
        [DataMember(Name = "slot")]
        public string Slot { get; set; }

        /// <summary>
        /// The Dictionary with content for this Component
        /// </summary>
        [DataMember(Name = "content")]
        public Dictionary<string, ContentItem> Content { get; set; }

        /// <summary>
        /// Get the nested Components
        /// </summary>
        [DataMember(Name = "nested")]
        public List<ContentComponent> Nested { get; set; }

        /// <summary>
        /// Internal information for this Component (if any)
        /// </summary>
        [DataMember(Name = "internalInfo")]
        public InternalInformation InternalInfo { get; set; } = new InternalInformation();

        /// <summary>
        /// Is this a shared component ?
        /// </summary>
        [DataMember(Name = "isShared")]
        public bool IsShared { get; set; }

        /// <summary>
        /// What is the component template title (name) ?
        /// </summary>
        [DataMember(Name = "title")]
        public string Title { get; set; }
    }
}
