using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sushi.Mediakiwi.Controllers.Data
{
    public class ContentMetaData
    {
        public ContentMetaData()
        {

        }

        public string PageTitle { get; set; }
        public string PageDescription { get; set; }
        public string HtmlLang { get; set; }
        public List<ContentMetaTag> MetaTags { get; set; }
        public ContentMetaTag this[string key]
        {
            get
            {
                if (MetaTags?.Count > 0)
                    return MetaTags?.Where(x => x.Name == key).FirstOrDefault();

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
                MetaTags.Add(new ContentMetaTag(name, content, renderKey));
            }
            else if (overWriteIfExists)
            {
                ContentMetaTag metaTag = MetaTags.Where(x => x.Name == name).FirstOrDefault();
                metaTag.Value = content;
                metaTag.RenderKey = renderKey;
            }
        }
    }
}
