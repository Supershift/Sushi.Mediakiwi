using Sushi.Mediakiwi.Data;
using Sushi.Mediakiwi.Framework;
using Sushi.Mediakiwi.Framework.ContentListItem;
using Sushi.Mediakiwi.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Demonstration.Templates.List
{

    public class DemoList1 : ComponentListTemplate
    {
        public Random random { get; set; } = new Random(DateTime.UtcNow.Millisecond);

        [Framework.ContentSettingItem.Choice_Checkbox("Generate items", InteractiveHelp = "When no items exist, create 100 items")]
        public bool Setting_CreateItems { get; set; }

        [Framework.ContentSettingItem.Choice_Checkbox("Own filter cache", InteractiveHelp = "When true, the list will use it's own filter cache")]
        public bool Setting_OwnFilterCache { get; set; }

        Data.DemoObject1 Implement;
        public DemoList1()
        {
            ListSearch += DemoList1_ListSearch;
            ListLoad += DemoList1_ListLoad;
            ListInit += DemoList1_ListInit;
            ListSave += DemoList1_ListSave;
            ListDelete += DemoList1_ListDelete;
            ListDataReceived += DemoList1_ListDataReceived;
            ListDataItemCreated += DemoList1_ListDataItemCreated;
        }

        private void DemoList1_ListDataItemCreated(object sender, ListDataItemCreatedEventArgs e)
        {
            if (e.Item is Data.DemoObject1 item && e.Type == DataItemType.TableCell)
            {
                if (e.ColumnProperty == nameof(Data.DemoObject1.ID))
                {
                    e.Attribute.Add("Something", $"dinges_{item.ID}");
                }
            }
        }

        private async Task DemoList1_ListDataReceived(Framework.EventArguments.ComponentListDataReceived arg)
        {
            if (string.IsNullOrWhiteSpace(arg.FullTypeName) == false)
            {
                var targetObjectType = Type.GetType(arg.FullTypeName);
                if (targetObjectType != null)
                {
                    // Only save new items
                    foreach (var record in arg.ReceivedProperties.Where(x=>x.ItemType == Framework.EventArguments.ReceivedItemTypeEnum.NEW))
                    {
                        var newObject = Activator.CreateInstance(targetObjectType);

                        foreach (var item in record.PropertyValues)
                        {
                            var propertyToSet = targetObjectType.GetProperty(item.Key);
                            if (propertyToSet?.CanWrite == true)
                            {
                                propertyToSet.SetValue(newObject, item.Value, null);
                            }
                        }

                        if (newObject is Data.DemoObject1 demoObject)
                        {
                            await demoObject.SaveAsync();
                        }
                    }
                }
            }
        }

        private ListItemCollection m_GroupOptions;

        public ListItemCollection GroupOptions
        {
            get 
            {
                if (m_GroupOptions == null)
                {
                    m_GroupOptions = new ListItemCollection();
                    
                    m_GroupOptions.Add(new ListItem("Test Group 1", "Test Group 1"));
                    m_GroupOptions.Add(new ListItem("Test Group 2", "Test Group 2"));
                    m_GroupOptions.Add(new ListItem("Test Group 3", "Test Group 3"));
                    m_GroupOptions.Add(new ListItem("Test Group 4", "Test Group 4"));
                    m_GroupOptions.Add(new ListItem("Test Group 5", "Test Group 5"));

                }
                return m_GroupOptions; 
            }
        }

        private ListItemCollection m_GroupOptionsWithEmpty;
        public ListItemCollection GroupOptionsWithEmpty
        {
            get 
            {
                if (m_GroupOptionsWithEmpty == null)
                {
                    m_GroupOptionsWithEmpty = GroupOptions;
                    m_GroupOptionsWithEmpty.Items.Insert(0, new ListItem("-- ALL --", "  "));
                }
                return m_GroupOptionsWithEmpty;
            }
        }

        [Framework.ContentListSearchItem.Choice_Dropdown("Group", nameof(GroupOptionsWithEmpty))]
        public string Filter_Group { get; set; }

        [Framework.ContentListSearchItem.Date("From date", Expression = OutputExpression.Left)]
        public DateTime Filter_DateFrom { get; set; }

        [Framework.ContentListSearchItem.Date("Till date", Expression = OutputExpression.Right)]
        public DateTime Filter_DateTill { get; set; }

        private string getFilterContent()
        {
            string temp = "-no filter data-";
            string rawContent = "";
            if (wim.HasOwnSearchListCache)
            {
                rawContent = wim.CurrentVisitor.Data[$"wim_FilterInfo_{ wim.CurrentList.GUID}"].Value;
            }
            else
            {
                rawContent = wim.CurrentVisitor.Data[$"wim_FilterInfo"].Value;
            }

            if (string.IsNullOrWhiteSpace(rawContent) == false)
            {
                var content = Content.GetDeserialized(rawContent);
                if (content?.Fields?.Length > 0)
                {
                    temp = "";
                    foreach (var item in content.Fields)
                    {
                        temp += $"<br/><b>{item.Property}</b>: {item.Value}";
                    }
                }
            }

            return temp;
        }

        private async Task DemoList1_ListDelete(ComponentListEventArgs arg)
        {
            await Implement.DeleteAsync();
        }

        private async Task DemoList1_ListSave(ComponentListEventArgs arg)
        {
            if (Implement == null)
            {
                Implement = new Data.DemoObject1();
            }
            Utils.ReflectProperty(this, Implement);

            if (Implement.Updated == DateTime.MinValue)
            {
                Implement.Updated = null;
            }

            if (Implement.Created == DateTime.MinValue)
            {
                Implement.Created = DateTime.UtcNow;
            }

            await Implement.SaveAsync();
        }

        private async Task DemoList1_ListInit()
        {
            wim.HasOwnSearchListCache = Setting_OwnFilterCache;
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
            wim.ListDataColumns.Add(new ListDataColumn("JSON", nameof(Data.DemoObject1.JSON), ListDataColumnType.APIOnly));
            wim.ListDataColumns.Add(new ListDataColumn("Group", nameof(Data.DemoObject1.Group), ListDataColumnType.Default));
            wim.ListDataColumns.Add(new ListDataColumn("Created", nameof(Data.DemoObject1.Created), ListDataColumnType.Default) { ColumnWidth = 90 });
            wim.ListDataColumns.Add(new ListDataColumn("Updated", nameof(Data.DemoObject1.Updated), ListDataColumnType.Default) { ColumnWidth = 90 });

            
            var filteredItems = await Data.DemoObject1.FetchAllAsync(Filter_Group, Filter_DateFrom, Filter_DateTill);

            // Insert a bunch of items when we got none
            if (Setting_CreateItems)
            {
                var allitems = await Data.DemoObject1.FetchAllAsync();
                if (allitems == null || allitems.Count == 0)
                {
                    for (int i = 0; i < 100; i++)
                    {
                        string? group = null;
                        int randomIdx = random.Next(0, 10);
                        if (randomIdx < GroupOptions.Items.Count)
                        {
                            group = GroupOptions[randomIdx].Value;
                        }

                        var newObject = new Data.DemoObject1
                        {
                            Created = DateTime.UtcNow.AddDays(-(randomIdx * 2)),
                            Title = $"DemoObject nr. {i:000}",
                            Group = group,
                            Updated = DateTime.UtcNow.AddDays(-randomIdx).AddMinutes(randomIdx),
                        };
                        await newObject.SaveAsync();

                        filteredItems.Add(newObject);
                    }
                }
            }

            // Resources test
            await wim.Page.Resources.AddAsync(ResourceLocation.HEADER, ResourceType.JAVASCRIPT, "/js/dingesHeader1.js", true, true);
            await wim.Page.Head.AddScriptAsync("/js/dingesHeader2.js");
            await wim.Page.Body.AddAsync("<script type=\"text/javascript\">var dingesBody1 = '';</script>", false);
            await wim.Page.Resources.AddAsync(ResourceLocation.BODY_NESTED, ResourceType.HTML, "<script type=\"text/javascript\">var dingesBody2 = '';</script>");
            await wim.Page.Resources.AddAsync(ResourceLocation.BODY_BELOW, ResourceType.STYLESHEET, "/css/dingesFooter1.css", true, true);

            wim.ListDataAdd(filteredItems);

            if (wim.HasOwnSearchListCache)
            {
                wim.ListInfoApply("Demo list 1", "OWN Filter", getFilterContent());
            }
            else
            {
                wim.ListInfoApply("Demo list 1", "SHARED Filter", getFilterContent());
            }
        }

        [TextField("Title", 100, true)]
        public string Title { get; set; }

        [Choice_Dropdown("Group", nameof(GroupOptions))]
        public string Group { get; set; }

        [DateTime("Created datetime", true)]
        public DateTime Created { get; set; }

        [Date("Updated date", false)]
        public DateTime Updated { get; set; }

        [DataList(typeof(DemoList2))]
        public DataList Items2 { get; set; }

        [Button("Test button", OpenInPopupLayer = true, PopupLayerHeight = "400px", PopupLayerWidth = "80%", PopupLayerSize = LayerSize.Normal, PopupLayerScrollBar = true, PopupTitle = "Test popup", ListInPopupLayer = "d4721c3a-bf3b-4192-a394-767bbf4784a6")]
        public bool Button_Test { get; set; }

        [Button("Test button 2", OpenInPopupLayer = true, PopupLayerHeight = "400px", PopupLayerWidth = "80%", PopupLayerSize = LayerSize.Normal, PopupLayerScrollBar = true, PopupTitle = "Test popup", CustomUrlProperty = nameof(Button_Test2_URL))]
        public bool Button_Test2 { get; set; }

        public string Button_Test2_URL { get; set; } = "https://www.google.nl";

    }
}
