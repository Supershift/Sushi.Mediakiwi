using Sushi.Mediakiwi.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Demonstration.Templates.List
{
    public class DemoObject2
    {
        public int ID { get; set; }
        public string Title { get; set; }

        public static List<DemoObject2> FetchAll()
        {
            List<DemoObject2> tmp = new List<DemoObject2>();
            for (int i = 1; i < 200; i++)
            {
                tmp.Add(new DemoObject2()
                {
                    ID = i,
                    Title = $"Demo2Title {i}"
                });
            }

            return tmp;
        }

        public static DemoObject2 FetchSingle(int id) 
        {
            return new DemoObject2() 
            { 
                ID = id, 
                Title = $"Demo2Title {id}" 
            };
        }
    }

    public class DemoList2 : ComponentListTemplate
    {
        public DemoList2()
        {
            ListSearch += DemoList2_ListSearch;
            ListInit += DemoList2_ListInit;
            ListLoad += DemoList2_ListLoad;
            ListSave += DemoList2_ListSave;
            ListDelete += DemoList2_ListDelete;
        }

        private async Task DemoList2_ListDelete(ComponentListEventArgs arg)
        {
        }

        private async Task DemoList2_ListSave(ComponentListEventArgs arg)
        {
        }

        public DemoObject2 Implement { get; set; }

        private async Task DemoList2_ListLoad(ComponentListEventArgs e)
        {
            if (e.SelectedKey > 0)
            {
                Implement = DemoObject2.FetchSingle(e.SelectedKey);

                var map = new Maps.DemoList2Map(Implement);
                if (FormMaps.List.Contains(map) == false)
                {
                    FormMaps.Add(map);
                }
            }
        }

        private Task DemoList2_ListInit()
        {
            wim.ListInfoApply("Demo List 2", "<strong>ListDataApply</strong> used", "This list DOES NOT work when viewing it directly");
            return Task.CompletedTask;
        }

        private async Task DemoList2_ListSearch(ComponentListSearchEventArgs arg)
        {
            wim.ListDataColumns.Add(new ListDataColumn("ID", nameof(DemoObject2.ID), ListDataColumnType.UniqueIdentifier));
            wim.ListDataColumns.Add(new ListDataColumn("Title", nameof(DemoObject2.Title), ListDataColumnType.HighlightPresent));

            var allItems = DemoObject2.FetchAll();

            if (!wim.IsListItemCollectionMode)
            {
                var list = Data.ComponentList.SelectOne(typeof(DemoList1));

                var result = Utils.GetListCollection(wim, list);
                Console.WriteLine(result.Count);
            }

            wim.ListDataAdd(allItems);
        }
    }
}
