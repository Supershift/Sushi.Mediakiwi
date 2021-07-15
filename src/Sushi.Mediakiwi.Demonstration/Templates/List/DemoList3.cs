using Sushi.Mediakiwi.Data;
using Sushi.Mediakiwi.Framework;
using Sushi.Mediakiwi.Framework.ContentListItem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Demonstration.Templates.List
{
    public class DemoObject3
    {
        public int ID { get; set; }
        public string Title { get; set; }

        public static List<DemoObject3> FetchAll()
        {
            List<DemoObject3> tmp = new List<DemoObject3>();
            for (int i = 1; i < 200; i++)
            {
                tmp.Add(new DemoObject3()
                {
                    ID = i,
                    Title = $"Demo 3.{i}"
                });
            }

            return tmp;
        }


        public static DemoObject3 FetchSingle(int id)
        {
            return new DemoObject3()
            {
                ID = id,
                Title = $"Demo 3.{id}"
            };
        }
    }

    public class DemoList3 : ComponentListTemplate
    {
        DemoObject3 Implement;
        public DemoList3()
        {
            ListSearch += DemoList3_ListSearch;
            ListLoad += DemoList3_ListLoad;
            ListInit += DemoList3_ListInit;
        }

        private Task DemoList3_ListInit()
        {
            wim.ListInfoApply("Demo List 3", "Used maily by Demo list 4", "This list states that it should open in a layer");
            return Task.CompletedTask;
        }

        private Task DemoList3_ListLoad(ComponentListEventArgs e)
        {
            Implement = DemoObject3.FetchSingle(e.SelectedKey);
            Utils.ReflectProperty(Implement, this);
            return Task.CompletedTask;
        }

        private Task DemoList3_ListSearch(ComponentListSearchEventArgs arg)
        {
            wim.ListDataColumns.Add(new ListDataColumn("ID", nameof(DemoObject3.ID), ListDataColumnType.UniqueIdentifier));
            wim.ListDataColumns.Add(new ListDataColumn("Title", nameof(DemoObject3.Title), ListDataColumnType.HighlightPresent));

            var allItems = DemoObject3.FetchAll();
          
            wim.ListDataAdd(allItems);

            wim.Page.Body.Grid.SetClickLayer(new Grid.LayerSpecification()
            {
                Height = 400,
                Width = 700,
                Title = "Should be Demo 3 item"

            });
            return Task.CompletedTask;
        }

        [TextLine("Title")]
        public string Title { get; set; }

    }
}
