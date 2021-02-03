using System.Runtime.Serialization;

namespace Sushi.Mediakiwi.Headless.Data
{
    /// <summary>
    /// A Single Meta tag
    /// </summary>
    [DataContract]
    public class ContentMetaTag
    {
        public ContentMetaTag(string name, string content, MetaTagRenderKey renderKey = MetaTagRenderKey.NAME)
        {
            Name = name;
            Value = content;
            RenderKey = renderKey;
        }

        /// <summary>
        /// Will this metatag render its key as a 'Name' or 'Property' attribute
        /// </summary>
        [DataMember(Name = "renderKey")]
        public MetaTagRenderKey RenderKey { get; set; }

        /// <summary>
        /// The Key (name / property) of this Metatag 
        /// </summary>
        [DataMember(Name = "name")]
        public string Name { get; set; }

        /// <summary>
        /// The value of this Metatag
        /// </summary>
        [DataMember(Name = "value")]
        public string Value { get; set; }
    }
}
