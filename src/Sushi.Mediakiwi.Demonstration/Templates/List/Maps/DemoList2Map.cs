using Sushi.Mediakiwi.Data;
using Sushi.Mediakiwi.Framework;

namespace Sushi.Mediakiwi.Demonstration.Templates.List.Maps
{
    public class DemoList2Map : FormMap<DemoObject2>
    {
        public DemoList2Map(DemoObject2 implement)
        {
            Load(implement);

            Map(x => x.Title).TextField("Title").Expression(OutputExpression.Left);
            Map(x => x.Button_Additional, this).Button("Test Button");
            Map(x => x.Sublisttt, this).SubListSelect("test sublist", typeof(DemoList1));
        }

        public bool Button_Additional { get; set; }

        public SubList Sublisttt { get; set; }
    }
}
