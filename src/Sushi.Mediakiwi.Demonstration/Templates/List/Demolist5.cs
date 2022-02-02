using Sushi.Mediakiwi.Framework;
using Sushi.Mediakiwi.Framework.ContentListItem;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Demonstration.Templates.List
{
    public class DemoObject5
    {
        public int ID { get; set; }
        public string Title { get; set; }

        public static List<DemoObject5> FetchAll()
        {
            List<DemoObject5> tmp = new List<DemoObject5>();
            for (int i = 1; i < 200; i++)
            {
                tmp.Add(new DemoObject5()
                {
                    ID = i,
                    Title = $"DemoObject 5.{i:000}"
                });
            }

            return tmp;
        }


        public static DemoObject5 FetchSingle(int id)
        {
            return new DemoObject5()
            {
                ID = id,
                Title = $"DemoObject 5.{id:000}"
            };
        }
    }

    public class DemoList5 : ComponentListTemplate
    {
        DemoObject5 Implement;
        public DemoList5()
        {
            ListLoad += DemoList5_ListLoad;
            ListInit += DemoList5_ListInit;

            wim.CanContainSingleInstancePerDefinedList = true;

        }

        private async Task DemoList5_ListInit()
        {
            wim.ListInfoApply("Demo List 5", "Single instance", "");
        }

        private async Task DemoList5_ListLoad(ComponentListEventArgs e)
        {
            Implement = DemoObject5.FetchSingle(e.SelectedKey);
            
            Utils.ReflectProperty(Implement, this);
        }

        [TextLine("Title")]
        public string Title { get; set; }

    }
}
