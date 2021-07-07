using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Sushi.Mediakiwi.Data
{
    [DataContract]
    public class ContentItem
    {
        public ContentItem()
        {

        }

        public ContentItem(string text)
        {
            Text = text;
        }

        /// <summary>
        /// The Url for this content (probably an Image)
        /// </summary>
        [DataMember(Name = "url")]
        public string Url { get; set; }

        /// <summary>
        /// The Target for this Link
        /// </summary>
        [DataMember(Name = "target")]
        public string Target { get; set; }

        /// <summary>
        /// The Url for this Link
        /// </summary>
        [DataMember(Name = "href")]
        public string Href { get; set; }

        /// <summary>
        /// Can both bee TEXT or HTML or alt
        /// </summary>
        [DataMember(Name = "text")]
        public string Text { get; set; }

        /// <summary>
        /// Can both bee TEXT or HTML or alt
        /// </summary>
        [DataMember(Name = "items")]
        public ICollection<object> Items { get; set; }

        /// <summary>
        /// The height of this Image
        /// </summary>
        [DataMember(Name = "height")]
        public int? Height { get; set; }

        /// <summary>
        /// The Width of this Image
        /// </summary>
        [DataMember(Name = "width")]
        public int? Width { get; set; }

        /// <summary>
        /// If this is a multifield, this will contain its content
        /// </summary>
        [DataMember(Name = "multiFieldContent")]
        public Dictionary<string, ContentItem> MultiFieldContent { get; set; }
    }
}
