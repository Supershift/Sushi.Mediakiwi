using Sushi.Mediakiwi.Framework;
using Sushi.Mediakiwi.Framework.ContentListItem;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Demonstration.Templates.List
{
    public class TabListMain : ComponentListTemplate
    {
        public TabListMain()
        {   
            ListLoad += TabListMain_ListLoad;
            ListSearch += TabListMain_ListSearch;
        }

        [TextLine("Name")]
        public string Name { get; set; }

        public class MyClass
        {
            public int ID { get; set; }
            public string Name { get; set; }
        }

        private Task TabListMain_ListSearch(ComponentListSearchEventArgs arg)
        {
            var items = new List<MyClass>();
            items.Add(new MyClass { Name = "Test 1", ID = 1 });
            wim.ListDataColumns.Add(new ListDataColumn("ID", nameof(MyClass.ID), ListDataColumnType.UniqueIdentifier));
            wim.ListDataColumns.Add(new ListDataColumn("Title", nameof(MyClass.Name), ListDataColumnType.HighlightPresent));
            wim.ListDataAdd(items);
            return Task.CompletedTask;
        }

        private Task TabListMain_ListLoad(ComponentListEventArgs arg)
        {
            wim.AddTab(typeof(TabListSub));
            wim.AddTab(typeof(TabListSub2));
            return Task.CompletedTask;
        }
    }
}
