using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Sushi.Mediakiwi.Headless.Data
{
    /// <summary>
    /// Page Meta Data class
    /// </summary>
    [DataContract]
    public class ContentMetaData
    {
        public ContentMetaData()
        {
            MetaTags = new List<ContentMetaTag>();
        }

        /// <summary>
        /// The Page Title
        /// </summary>
        [DataMember(Name = "pageTitle")]
        public string PageTitle { get; set; }

        /// <summary>
        /// The Page description
        /// </summary>
        [DataMember(Name = "pageDescription")]
        public string PageDescription { get; set; }

        /// <summary>
        /// The Html Language
        /// </summary>
        [DataMember(Name = "htmlLang")]
        public string HtmlLang { get; set; }

        /// <summary>
        /// The Meta Tags collection
        /// </summary>
        [DataMember(Name = "metaTags")]
        public List<ContentMetaTag> MetaTags { get; set; }

        /// <summary>
        /// Gets a metatag by its Key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public ContentMetaTag this[string key]
        {
            get
            {
                if (MetaTags?.Count > 0)
                {
                    return MetaTags?.FirstOrDefault(x => x.Name == key);
                }

                return null;
            }
        }

        public bool ContainsKey(string key)
        {
            return this[key] != null;
        }

        /// <summary>
        /// Adds a meta data item to the collection
        /// </summary>
        /// <param name="name">The name of the metadata item</param>
        /// <param name="content">The content of the metadata item</param>
        /// <param name="renderKey">How will the name of the tag be rendered</param>
        /// <param name="overWriteIfExists">When this name already exists, overwrite or not.</param>
        public void Add(string name, string content, MetaTagRenderKey renderKey = MetaTagRenderKey.NAME, bool overWriteIfExists = false)
        {
            if (ContainsKey(name) == false)
            {
                if (MetaTags == null)
                {
                    MetaTags = new List<ContentMetaTag>();
                }

                MetaTags.Add(new ContentMetaTag(name, content, renderKey));
            }
            else if (overWriteIfExists)
            {
                ContentMetaTag metaTag = MetaTags.FirstOrDefault(x => x.Name == name);
                metaTag.Value = content;
                metaTag.RenderKey = renderKey;
            }
        }
    }
}
