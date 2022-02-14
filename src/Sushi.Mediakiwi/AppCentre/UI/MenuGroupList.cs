using Sushi.Mediakiwi.Data;
using Sushi.Mediakiwi.Framework;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.AppCentre.Data
{
    public class MenuGroupList : ComponentListTemplate
    {
        public MenuGroupList()
        {
            ListSave += MenuGroupList_ListSave;
            ListDelete += MenuGroupList_ListDelete;
            ListLoad += MenuGroupList_ListLoad;
            ListSearch += MenuGroupList_ListSearch;
            ListDelete += MenuGroupList_ListDelete;
            ListPreRender += MenuGroupList_ListPreRender;
        }

        private async Task MenuGroupList_ListPreRender(ComponentListEventArgs arg)
        {
            if (wim.IsSaveMode)
            {
                if (Title?.Equals("default", System.StringComparison.InvariantCultureIgnoreCase) == true)
                {
                    wim.Notification.AddError(nameof(Title), "This is a reserved word, please choose another title");
                }

                // Check if we already have an existing group with this tag
                var existingGroup = await MenuGroup.FetchSingleAsync(Tag);
                if (existingGroup?.ID > 0 && existingGroup.ID != Implement.ID)
                {
                    wim.Notification.AddError(nameof(Tag), "This tag is already used, please choose another tag");
                }
            }
        }

        async Task MenuGroupList_ListSave(ComponentListEventArgs e)
        {
            Utils.ReflectProperty(this, Implement);
            await Implement.SaveAsync();
        }

        async Task MenuGroupList_ListDelete(ComponentListEventArgs e)
        {
            foreach (var item in await Menu.SelectAllAsync())
            {
                if (item.GroupID.GetValueOrDefault(0) > 0 && item.GroupID == Implement.ID)
                {
                    item.IsActive = false;
                    await item.SaveAsync();
                }
            }

            await Implement.DeleteAsync();
        }

        MenuGroup Implement { get; set; }

        async Task MenuGroupList_ListLoad(ComponentListEventArgs e)
        {
            Implement = await MenuGroup.FetchSingleAsync(e.SelectedKey);
            if (Implement == null)
            {
                Implement = new MenuGroup();
            }
            Utils.ReflectProperty(Implement, this);
        }

        async Task MenuGroupList_ListSearch(ComponentListSearchEventArgs e)
        {
            wim.CanAddNewItem = true;
            wim.ListDataColumns.Add(new ListDataColumn("ID", nameof(MenuGroup.ID), ListDataColumnType.UniqueIdentifier));
            wim.ListDataColumns.Add(new ListDataColumn("Title", nameof(MenuGroup.Title), ListDataColumnType.HighlightPresent));
            wim.ListDataColumns.Add(new ListDataColumn("Tag", nameof(MenuGroup.Tag), ListDataColumnType.Default) { ColumnWidth = 100 });
            wim.ListDataColumns.Add(new ListDataColumn("Description", nameof(MenuGroup.Description)));

            var results = await MenuGroup.FetchAllAsync();
            wim.ListDataAdd(results);
        }

        [Framework.ContentListItem.TextField("Title", 50, true, Expression = OutputExpression.FullWidth, InteractiveHelp = "The title for this group, is only for internal reference")]
        public string Title { get; set; }

        [Framework.ContentListItem.TextField("Tag", 20, true, Expression = OutputExpression.FullWidth, InteractiveHelp = "The tag for this group, must be globally unique. Can be used in the API to retrieve the correct menu.")]
        public string Tag { get; set; }

        [Framework.ContentListItem.TextField("Description", 500, true, Expression = OutputExpression.FullWidth)]
        public string Description { get; set; }

    }
}
