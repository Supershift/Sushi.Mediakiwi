using Sushi.Mediakiwi.Data;
using Sushi.Mediakiwi.Extention;
using Sushi.Mediakiwi.Framework;
using Sushi.Mediakiwi.Framework.ContentListItem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Demonstration.Templates.List
{

    public class DemoList1 : ComponentListTemplate
    {
        Data.DemoObject1 Implement;
        public DemoList1()
        {
            ListSearch += DemoList1_ListSearch;
            ListLoad += DemoList1_ListLoad;
            ListInit += DemoList1_ListInit;
            ListSave += DemoList1_ListSave;
            ListDelete += DemoList1_ListDelete;
        }

        private async Task DemoList1_ListDelete(ComponentListEventArgs arg)
        {
            await Implement.DeleteAsync();
        }

        private async Task DemoList1_ListSave(ComponentListEventArgs arg)
        {
            Utils.ReflectProperty(this, Implement);
            await Implement.SaveAsync();
        }

        private async Task DemoList1_ListInit()
        {
            wim.ListInfoApply("Demo List 1", "<strong>ListDataAdd</strong> used", "This list works when viewing it directly, shows items and when clicking on an item, the items from DemoList 2 show up");
        }

        private async Task DemoList1_ListLoad(ComponentListEventArgs e)
        {
            await wim.Page.Resources.AddAsync(ResourceLocation.HEADER, ResourceType.JAVASCRIPT, "/js/dingesHeader1LOAD.js", true, true);

            Implement = await Data.DemoObject1.FetchOneAsync(e.SelectedKey);
            Utils.ReflectProperty(Implement, this);
        }

        private async Task DemoList1_ListSearch(ComponentListSearchEventArgs arg)
        {
            wim.ListDataColumns.Add(new ListDataColumn("ID", nameof(Data.DemoObject1.ID), ListDataColumnType.UniqueIdentifier));
            wim.ListDataColumns.Add(new ListDataColumn("Title", nameof(Data.DemoObject1.Title), ListDataColumnType.HighlightPresent));
            wim.ListDataColumns.Add(new ListDataColumn("Datetime", nameof(Data.DemoObject1.Created), ListDataColumnType.Default));

            var allItems = await Data.DemoObject1.FetchAllAsync();

            // Insert a bunch of items when we got none
            if (allItems.Count == 0)
            {
                for (int i = 0; i < 50; i++)
                {
                    var newObject = new Data.DemoObject1 
                    { 
                        Created =  DateTime.UtcNow,
                        Title = $"DemoObject nr.{i}"
                    };
                    await newObject.SaveAsync();

                    allItems.Add(newObject);
                }
            }

            // Resources test
            await wim.Page.Resources.AddAsync(ResourceLocation.HEADER, ResourceType.JAVASCRIPT, "/js/dingesHeader1.js", true, true);
            await wim.Page.Head.AddScriptAsync("/js/dingesHeader2.js");
            await wim.Page.Body.AddAsync("<script type=\"text/javascript\">var dingesBody1 = '';</script>", false);
            await wim.Page.Resources.AddAsync(ResourceLocation.BODY_NESTED, ResourceType.HTML, "<script type=\"text/javascript\">var dingesBody2 = '';</script>");
            await wim.Page.Resources.AddAsync(ResourceLocation.BODY_BELOW, ResourceType.STYLESHEET, "/css/dingesFooter1.css", true, true);

            wim.ListDataAdd(allItems);
        }

        [TextField("Title", 100, true)]
        public string Title { get; set; }

        [DateTime("Custom datetime", true)]
        public DateTime Created { get; set; }

        [DataList(typeof(DemoList2))]
        public DataList Items2 { get; set; }

        [Button("Test button", OpenInPopupLayer = true, PopupLayerHeight = "400px", PopupLayerWidth = "80%", PopupLayerSize = LayerSize.Normal, PopupLayerScrollBar = true, PopupTitle = "Test popup", ListInPopupLayer = "d4721c3a-bf3b-4192-a394-767bbf4784a6")]
        public bool Button_Test { get; set; }

        [Button("Test button 2", OpenInPopupLayer = true, PopupLayerHeight = "400px", PopupLayerWidth = "80%", PopupLayerSize = LayerSize.Normal, PopupLayerScrollBar = true, PopupTitle = "Test popup", CustomUrlProperty = nameof(Button_Test2_URL))]
        public bool Button_Test2 { get; set; }

        public string Button_Test2_URL { get; set; } = "https://www.google.nl";

    }
}
