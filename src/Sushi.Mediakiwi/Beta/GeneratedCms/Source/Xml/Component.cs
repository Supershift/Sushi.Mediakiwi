using System.Text;

namespace Sushi.Mediakiwi.Beta.GeneratedCms.Source.Xml
{
    class Component
    {
        internal static string Get(Console container, int componentTemplateId, int pageId, int componentId, string target)
        {
            StringBuilder build = new StringBuilder();
            // [CB: 24-06-2015] With permision of marc I can omit this
            //build.Append(@"<?xml version=""1.0""?><root>");

            container.ApplyList(Sushi.Mediakiwi.Data.ComponentListType.Browsing);

            container.Item = pageId;
            container.CurrentListInstance.wim.IsEditMode = true;
            container.ItemType = RequestItemType.Page;

            GeneratedCms.Source.Component component = new GeneratedCms.Source.Component();
            build.Append(component.CreateComponentContent(container, componentTemplateId, pageId, componentId, target).Replace("&", "&amp;"));
            // [CB: 24-06-2015] With permision of marc I can omit this
            //build.Append(@"</root>");
            
            return build.ToString();
        }
    }
}
