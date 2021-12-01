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
        }

        public bool Button_Additional { get; set; }
    }
}
