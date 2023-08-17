using Sushi.Mediakiwi.Demonstration.Templates.Entities;
using Sushi.Mediakiwi.Framework;
using Sushi.Mediakiwi.Framework.ContentListItem;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Demonstration.Templates.List
{
    public class DemoList1 : ComponentListTemplate
    {
        DemoObject1 Implement;
        public DemoList1()
        {
            ListSearch += DemoList1_ListSearch;
            ListLoad += DemoList1_ListLoad;
            ListInit += DemoList1_ListInit;
            ListSave += DemoList1_ListSave;
            wim.OpenInEditMode = true;
        }

        private async Task DemoList1_ListSave(ComponentListEventArgs arg)
        {
            Implement.Text = this.Text;

            await Implement.SaveAsync().ConfigureAwait(false);
        }

        private async Task DemoList1_ListInit()
        {
            wim.ListInfoApply("Demo List 1", "<strong>ListDataAdd</strong> used", "This list works when viewing it directly, shows items and when clicking on an item, the items from DemoList 2 show up");
        }

        private async Task DemoList1_ListLoad(ComponentListEventArgs e)
        {
            await wim.Page.Resources.AddAsync(ResourceLocation.HEADER, ResourceType.JAVASCRIPT, "/js/dingesHeader1LOAD.js", true, true);

            Implement = await DemoObject1.FetchSingle(e.SelectedKey);
            Utils.ReflectProperty(Implement, this);
        }

        private async Task DemoList1_ListSearch(ComponentListSearchEventArgs arg)
        {
            wim.ListDataColumns.Add(new ListDataColumn("ID", nameof(DemoObject1.ID), ListDataColumnType.UniqueIdentifier));
            wim.ListDataColumns.Add(new ListDataColumn("Title", nameof(DemoObject1.Title)));
            wim.ListDataColumns.Add(new ListDataColumn("Saved", nameof(DemoObject1.Created), ListDataColumnType.Default));

            var allItems = await DemoObject1.FetchAll();

            // Resources test
            await wim.Page.Resources.AddAsync(ResourceLocation.HEADER, ResourceType.JAVASCRIPT, "/js/dingesHeader1.js", true, true);
            await wim.Page.Head.AddScriptAsync("/js/dingesHeader2.js");
            await wim.Page.Body.AddAsync("<script type=\"text/javascript\">var dingesBody1 = '';</script>", false);
            await wim.Page.Resources.AddAsync(ResourceLocation.BODY_NESTED, ResourceType.HTML, "<script type=\"text/javascript\">var dingesBody2 = '';</script>");
            await wim.Page.Resources.AddAsync(ResourceLocation.BODY_BELOW, ResourceType.STYLESHEET, "/css/dingesFooter1.css", true, true);

            wim.ListDataAdd(allItems);
        }

        [TextLine("Title")]
        public string Title { get; set; }

        [Binary_Image("Select image")]
        public int ImageId { get; set; }

        [RichText("Rich", 0)]
        public string Text { get; set; }

        //[DataList(typeof(DemoList2))]
        //public DataList Items2 { get; set; }

        [Button("Test button", OpenInPopupLayer = true, PopupLayerHeight = "400px", PopupLayerWidth = "80%", PopupLayerSize = LayerSize.Normal, PopupLayerScrollBar = true, PopupTitle = "Test popup", ListInPopupLayer = "d4721c3a-bf3b-4192-a394-767bbf4784a6")]
        public bool Button_Test { get; set; }

        [Button("Test button 2", OpenInPopupLayer = true, PopupLayerHeight = "400px", PopupLayerWidth = "80%", PopupLayerSize = LayerSize.Normal, PopupLayerScrollBar = true, PopupTitle = "Test popup", CustomUrlProperty = nameof(Button_Test2_URL))]
        public bool Button_Test2 { get; set; }

        public string Button_Test2_URL { get; set; } = "https://www.google.nl";

    }
}
