using Sushi.Mediakiwi.Framework;
using Sushi.Mediakiwi.Framework.ContentListItem;
using Sushi.Mediakiwi.UI;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Demonstration.Templates.List
{
    public class TabListSub : ComponentListTemplate
    {
        public TabListSub()
        {
            ListLoad += TabListSub_ListLoad;
            ListInit += TabListSub_ListInit;
            ListSearch += TabListSub_ListSearch;
        }

        [Choice_Dropdown("Something", "MyCollection")]
        public int Something { get; set; }

        public ListItemCollection MyCollection
        {
            get
            {
                return new ListItemCollection();
            }
        }

        private Task TabListSub_ListSearch(ComponentListSearchEventArgs arg)
        {
            return Task.CompletedTask;
        }

        private Task TabListSub_ListInit()
        {
            return Task.CompletedTask;
        }

        private Task TabListSub_ListLoad(ComponentListEventArgs arg)
        {
            return Task.CompletedTask;
        }
    }
}
