using System.Text;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Beta.GeneratedCms.Source.Xml
{
    class Component
    {
        internal static async Task<string> GetAsync(Console container, int componentTemplateId, int pageId, int componentId, string target)
        {
            StringBuilder build = new StringBuilder();
            
            await container.ApplyListAsync(Data.ComponentListType.Browsing).ConfigureAwait(false);

            container.Item = pageId;
            container.CurrentListInstance.wim.IsEditMode = true;
            container.ItemType = RequestItemType.Page;

            Source.Component component = new Source.Component();
            var componentContent = await component.CreateComponentContentAsync(container, componentTemplateId, pageId, componentId, target).ConfigureAwait(false);
            build.Append(componentContent.Replace("&", "&amp;"));
            
            
            return build.ToString();
        }
    }
}
