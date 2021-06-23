using System;
using System.Collections.Generic;
using System.Text;

namespace Sushi.Mediakiwi.Controllers.Data
{
    public class ContentMetaTag
    {
        public ContentMetaTag(string name, string content, MetaTagRenderKey renderKey = MetaTagRenderKey.NAME)
        {
            Name = name;
            Value = content;
            RenderKey = renderKey;
        }

        public MetaTagRenderKey RenderKey { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
