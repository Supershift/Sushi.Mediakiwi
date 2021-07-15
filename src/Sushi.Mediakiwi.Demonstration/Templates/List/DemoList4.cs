using Sushi.Mediakiwi.Data;
using Sushi.Mediakiwi.Framework;
using Sushi.Mediakiwi.Framework.ContentListItem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Demonstration.Templates.List
{
    public class DemoObject4
    {
        public int ID { get; set; }
        public string Title { get; set; }

        public static List<DemoObject4> FetchAll()
        {
            List<DemoObject4> tmp = new List<DemoObject4>();
            for (int i = 1; i < 200; i++)
            {
                tmp.Add(new DemoObject4()
                {
                    ID = i,
                    Title = $"Demo 4.{i}"
                });
            }

            return tmp;
        }


        public static DemoObject4 FetchSingle(int id)
        {
            return new DemoObject4()
            {
                ID = id,
                Title = $"Demo 4.{id}"
            };
        }
    }

    public class DemoList4 : ComponentListTemplate
    {
        DemoObject4 Implement;
        public DemoList4()
        {
            ListSearch += DemoList4_ListSearch;
            ListLoad += DemoList4_ListLoad;
            ListInit += DemoList4_ListInit;
        }

        private Task DemoList4_ListInit()
        {
            wim.ListInfoApply("Demo List 4", "", "When you load a Demo 4 item, you can see the Demo3 datalist below. When clicking on one of the Demo3 items, i expect a Demo3 item to load in the layer, but instead a Demo4 item opens in the layer.");
            return Task.CompletedTask;
        }

        private Task DemoList4_ListLoad(ComponentListEventArgs e)
        {
            Implement = DemoObject4.FetchSingle(e.SelectedKey);
            Utils.ReflectProperty(Implement, this);
            return Task.CompletedTask;
        }

        private Task DemoList4_ListSearch(ComponentListSearchEventArgs arg)
        {
            wim.ListDataColumns.Add(new ListDataColumn("ID", nameof(DemoObject4.ID), ListDataColumnType.UniqueIdentifier));
            wim.ListDataColumns.Add(new ListDataColumn("Title", nameof(DemoObject4.Title), ListDataColumnType.HighlightPresent));

            var allItems = DemoObject4.FetchAll();

            wim.ListDataAdd(allItems);
            return Task.CompletedTask;
        }

        [TextLine("Title")]
        public string Title { get; set; }

        [DataList(typeof(DemoList3))]
        public DataList Items3 { get; set; }
    }
}
