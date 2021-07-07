using Sushi.Mediakiwi.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sushi.Mediakiwi.Controllers.Data
{
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
                    var source = Content[key];
                    clone.Content.Add(key, Content[key]);
                }
            }
            return clone;

        }
        public ContentComponent()
        {
            Content = new Dictionary<string, ContentItem>();
        }

        public string ComponentName { get; set; }
        public int ComponentID { get; set; }
        public int SortOrder { get; set; }
        public string Title { get; set; }
        public string Slot { get; set; }
        public Dictionary<string, ContentItem> Content { get; set; }

        public List<ContentComponent> Nested { get; set; }
        public bool IsShared { get; set; }
    }
}
