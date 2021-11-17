using Sushi.Mediakiwi.Data;
using Sushi.Mediakiwi.Framework;
using Sushi.Mediakiwi.Framework.ContentListItem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Demonstration.Templates.List
{
    public class DemoObject1
    {
        public int ID { get; set; }
        public string Title { get; set; }

        public static List<DemoObject1> FetchAll()
        {
            List<DemoObject1> tmp = new List<DemoObject1>();
            for (int i = 1; i < 200; i++)
            {
                tmp.Add(new DemoObject1()
                {
                    ID = i,
                    Title = $"Demo1Title {i}"
                });
            }

            return tmp;
        }


        public static DemoObject1 FetchSingle(int id)
        {
            return new DemoObject1()
            {
                ID = id,
                Title = $"Demo1Title {id}"
            };
        }
    }

    public class DemoList1 : ComponentListTemplate
    {
        DemoObject1 Implement;
        public DemoList1()
        {
            ListSearch += DemoList1_ListSearch;
            ListLoad += DemoList1_ListLoad;
            ListInit += DemoList1_ListInit;
        }

        private Task DemoList1_ListInit()
        {
            wim.ListInfoApply("Demo List 1", "<strong>ListDataAdd</strong> used", "This list works when viewing it directly, shows items and when clicking on an item, the items from DemoList 2 show up");
            return Task.CompletedTask;
        }

        private Task DemoList1_ListLoad(ComponentListEventArgs e)
        {
            Implement = DemoObject1.FetchSingle(e.SelectedKey);
            Utils.ReflectProperty(Implement, this);
            return Task.CompletedTask;
        }

        private Task DemoList1_ListSearch(ComponentListSearchEventArgs arg)
        {
            wim.ListDataColumns.Add(new ListDataColumn("ID", nameof(DemoObject1.ID), ListDataColumnType.UniqueIdentifier));
            wim.ListDataColumns.Add(new ListDataColumn("Title", nameof(DemoObject1.Title), ListDataColumnType.HighlightPresent));

            var allItems = DemoObject1.FetchAll();

            wim.ListDataAdd(allItems);
            return Task.CompletedTask;
        }

        [TextLine("Title")]
        public string Title { get; set; }
        
        [Binary_Image("Select image")]
        public int ImageId { get; set; }

        [DataList(typeof(DemoList2))]
        public DataList Items2 { get; set; }
    }
}
