using Sushi.Mediakiwi.Framework;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Demonstration.Templates.List
{
    public class TabListSub2 : ComponentListTemplate
    {
        public TabListSub2()
        {
            ListLoad += TabListSub_ListLoad;
        }

        private Task TabListSub_ListLoad(ComponentListEventArgs arg)
        {   
            return Task.CompletedTask;
        }
    }
}
