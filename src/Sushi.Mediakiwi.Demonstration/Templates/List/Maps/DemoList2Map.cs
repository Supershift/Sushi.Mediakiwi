using Sushi.Mediakiwi.Data;
using Sushi.Mediakiwi.Framework;
using Sushi.Mediakiwi.UI;

namespace Sushi.Mediakiwi.Demonstration.Templates.List.Maps
{
    public class DemoList2Map : FormMap<DemoObject2>
    {
        private ListItemCollection _MultiOptions;

        public ListItemCollection MultiOptions
        {
            get 
            {
                if (_MultiOptions == null)
                {
                    _MultiOptions = new ListItemCollection();
                    for (int i = 1; i <= 10; i++)
                    {
                        _MultiOptions.Items.Add(new ListItem($"Item {i}", i.ToString()));
                    }
                }
                return _MultiOptions; 
            }
        }

        public DemoList2Map(DemoObject2 implement)
        {
            Load(implement);

            Map(x => x.Title).TextField("Title").Expression(OutputExpression.Left);
            Map(x => x.Button_Additional, this).Button("Test Button");
            Map(x => x.Sublisttt, this).SubListSelect("test sublist", typeof(DemoList1));
            Map(x => x.MultiSelect, this).Dropdown("Multi select", nameof(MultiOptions), false, false, true, "");
            Map(x => x.ImageID, this).Image("Image", canOnlyCreate: false);
        }

        public int[] MultiSelect { get; set; }

        public bool Button_Additional { get; set; }

        public SubList Sublisttt { get; set; }

        public int ImageID { get; set; }
    }
}
