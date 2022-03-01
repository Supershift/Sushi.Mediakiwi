using Sushi.Mediakiwi.Data;
using Sushi.Mediakiwi.Framework;
using Sushi.Mediakiwi.UI;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.AppCentre.Data
{
    /// <summary>
    /// 
    /// </summary>
    public class MenuList : ComponentListTemplate
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MenuList"/> class.
        /// </summary>
        public MenuList()
        {
            ListSave += MenuList_ListSave;
            ListDelete += MenuList_ListDelete;
            ListLoad += MenuList_ListLoad;
            ListSearch += MenuList_ListSearch;
            ListDelete += MenuList_ListDelete;
        }

        async Task MenuList_ListSave(ComponentListEventArgs e)
        {
            Implement.Name = Name;
            Implement.RoleID = RoleID;
            Implement.SiteID = SiteID;
            if (GroupID.GetValueOrDefault(0) == 0)
            {
                Implement.GroupID = null;
            }
            else 
            {
                Implement.GroupID = GroupID;
            }
            Implement.IsActive = Active;

            await Implement.SaveAsync().ConfigureAwait(false);

            await SaveItem(0, MenuItem0).ConfigureAwait(false);
            await SaveItem(1, MenuItem1).ConfigureAwait(false);
            await SaveItem(2, MenuItem2).ConfigureAwait(false);
            await SaveItem(3, MenuItem3).ConfigureAwait(false);
            await SaveItem(4, MenuItem4).ConfigureAwait(false);
            await SaveItem(5, MenuItem5).ConfigureAwait(false);
            await SaveItem(6, MenuItem6).ConfigureAwait(false);
            await SaveItem(7, MenuItem7).ConfigureAwait(false);
            await SaveItem(8, MenuItem8).ConfigureAwait(false);
            wim.FlushCache();
        }

        async Task SaveItem(int position, SubList container)
        {
            int index = 0;
            //  Remove obsolete
            var existingMenuItems = (from item in Items where item.Position == position select item).ToArray();
            foreach (var item in existingMenuItems)
            {
                var seek = (from mi in container.Items where mi.TextID == item.Tag select mi).Count();
                if (seek == 0)
                {
                    await item.DeleteAsync().ConfigureAwait(false);
                }
            }

            //  Add Or Update
            index = 0;
            foreach (var item in container.Items)
            {
                index++;
                var split = item.TextID.Split('_');
                var menuItem = (from mi in existingMenuItems where mi.Tag == item.TextID select mi).FirstOrDefault();
                if (menuItem == null) menuItem = new MenuItem();
                menuItem.MenuID = Implement.ID;
                menuItem.ItemID = Convert.ToInt32(split[1]);
                menuItem.TypeID = Convert.ToInt32(split[0].Replace("T", string.Empty));
                menuItem.Position = position;
                menuItem.Sort = index;
                await menuItem.SaveAsync().ConfigureAwait(false);
            }
        }

        async Task MenuList_ListDelete(ComponentListEventArgs e)
        {
            foreach (var item in Items)
            {
                await item.DeleteAsync().ConfigureAwait(false);
            }
        }

        IMenu Implement { get; set; }
        IMenuItem[] Items { get; set; }

        async Task MenuList_ListLoad(ComponentListEventArgs e)
        {
            Implement = await Menu.SelectOneAsync(e.SelectedKey).ConfigureAwait(false);
            if (Implement == null) Implement = new Menu();

            Name = Implement.Name;
            RoleID = Implement.RoleID;
            SiteID = Implement.SiteID;
            Active = Implement.IsActive;
            GroupID = Implement.GroupID;

            Items = await MenuItem.SelectAllAsync(Implement.ID).ConfigureAwait(false);
            MenuItem0 = new SubList();
            MenuItem1 = new SubList();
            MenuItem2 = new SubList();
            MenuItem3 = new SubList();
            MenuItem4 = new SubList();
            MenuItem5 = new SubList();
            MenuItem6 = new SubList();
            MenuItem7 = new SubList();
            MenuItem8 = new SubList();

            await LoadItem(0, MenuItem0).ConfigureAwait(false);
            await LoadItem(1, MenuItem1).ConfigureAwait(false);
            await LoadItem(2, MenuItem2).ConfigureAwait(false);
            await LoadItem(3, MenuItem3).ConfigureAwait(false);
            await LoadItem(4, MenuItem4).ConfigureAwait(false);
            await LoadItem(5, MenuItem5).ConfigureAwait(false);
            await LoadItem(6, MenuItem6).ConfigureAwait(false);
            await LoadItem(7, MenuItem7).ConfigureAwait(false);
            await LoadItem(8, MenuItem8).ConfigureAwait(false);
        }

        async Task LoadItem(int position, SubList container)
        {
            var items = (from item in Items where item.Position == position select item).ToArray();

            List<string> tags = new List<string>();
            foreach (var item in items)
            {
                tags.Add(item.Tag);
            }
            var selection = await SearchView.SelectAllAsync(tags.ToArray()).ConfigureAwait(false);
            foreach (var item in items)
            {
                var find = (from x in selection where x.ID == item.Tag select x).FirstOrDefault();
                if (find != null)
                {
                    container.Add(new SubList.SubListitem(find.ID, find.TitleHighlighted));
                }
            }
        }

        async Task MenuList_ListSearch(ComponentListSearchEventArgs e)
        {
            wim.CanAddNewItem = true;
            wim.ListDataColumns.Add(new ListDataColumn("ID", nameof(Menu.ID), ListDataColumnType.UniqueIdentifier));
            wim.ListDataColumns.Add(new ListDataColumn("Name", nameof(Menu.Name), ListDataColumnType.HighlightPresent) { ColumnWidth = 200 });
            wim.ListDataColumns.Add(new ListDataColumn("Group", nameof(Menu.GroupTitle), ListDataColumnType.Default) { ColumnWidth = 150 });
            wim.ListDataColumns.Add(new ListDataColumn("", nameof(Menu.IsActive)) { ColumnWidth = 30 });

            var results = await Menu.SelectAllAsync().ConfigureAwait(false);
            wim.ListDataAdd(results);
        }

        [Framework.ContentListItem.TextField("Menu", 50, true, Expression = OutputExpression.Alternating)]
        public string Name { get; set; }

        [Framework.ContentListItem.Choice_Checkbox("Active", Expression = OutputExpression.Alternating)]
        public bool Active { get; set; }

        [Framework.ContentListItem.Choice_Dropdown("Role", nameof(AvailableRoles), false, false, Expression = OutputExpression.Alternating)]
        public int? RoleID { get; set; }

        [Framework.ContentListItem.Choice_Dropdown("Site", nameof(AvailableSites), false, false, Expression = OutputExpression.Alternating)]
        public int? SiteID { get; set; }

        [Framework.ContentListItem.Choice_Dropdown("Group", nameof(AvailableGroups), false, false, Expression = OutputExpression.Alternating, InteractiveHelp = "Allows for a custom menu to be created. Choose Default for the normal (top) navigation.")]
        public int? GroupID { get; set; }

        [Framework.ContentListItem.SubListSelect("Home", "1a1fe050-219c-4f63-a697-7e2e8e790521", true, "", CanContainOneItem = true)]
        public SubList MenuItem0 { get; set; }

        [Framework.ContentListItem.SubListSelect("Position #1", "1a1fe050-219c-4f63-a697-7e2e8e790521", true, "")]
        public SubList MenuItem1 { get; set; }

        [Framework.ContentListItem.SubListSelect("Position #2", "1a1fe050-219c-4f63-a697-7e2e8e790521", false, "")]
        public SubList MenuItem2 { get; set; }

        [Framework.ContentListItem.SubListSelect("Position #3", "1a1fe050-219c-4f63-a697-7e2e8e790521", false, "")]
        public SubList MenuItem3 { get; set; }

        [Framework.ContentListItem.SubListSelect("Position #4", "1a1fe050-219c-4f63-a697-7e2e8e790521", false, "")]
        public SubList MenuItem4 { get; set; }

        [Framework.ContentListItem.SubListSelect("Position #5", "1a1fe050-219c-4f63-a697-7e2e8e790521", false, "")]
        public SubList MenuItem5 { get; set; }

        [Framework.ContentListItem.SubListSelect("Position #6", "1a1fe050-219c-4f63-a697-7e2e8e790521", false, "")]
        public SubList MenuItem6 { get; set; }

        [Framework.ContentListItem.SubListSelect("Position #7", "1a1fe050-219c-4f63-a697-7e2e8e790521", false, "")]
        public SubList MenuItem7 { get; set; }

        [Framework.ContentListItem.SubListSelect("Position #8", "1a1fe050-219c-4f63-a697-7e2e8e790521", false, "")]
        public SubList MenuItem8 { get; set; }


        ListItemCollection _AvailableRoles;
        /// <summary>
        /// Gets the available roles.
        /// </summary>
        /// <value>The available roles.</value>
        public ListItemCollection AvailableRoles
        {
            get
            {
                if (_AvailableRoles == null)
                {
                    _AvailableRoles = new ListItemCollection();
                    _AvailableRoles.Add(new ListItem("Select a role", ""));
                    foreach (var role in ApplicationRole.SelectAll())
                    {
                        _AvailableRoles.Add(new ListItem(role.Name, $"{role.ID}"));
                    }
                }
                return _AvailableRoles;
            }
        }

        ListItemCollection _AvailableSites;
        /// <summary>
        /// Gets the available sites.
        /// </summary>
        /// <value>The available sites.</value>
        public ListItemCollection AvailableSites
        {
            get
            {
                if (_AvailableSites == null)
                {
                    _AvailableSites = new ListItemCollection();
                    _AvailableSites.Add(new ListItem("Select a site", ""));
                    foreach (Site site in Site.SelectAll())
                    {
                        _AvailableSites.Add(new ListItem(site.Name, $"{site.ID}"));
                    }
                }
                return _AvailableSites;
            }
        }

        ListItemCollection _AvailableGroups;
        /// <summary>
        /// Gets the available Menu Groups.
        /// </summary>
        /// <value>The available sites.</value>
        public ListItemCollection AvailableGroups
        {
            get
            {
                if (_AvailableGroups == null)
                {
                    _AvailableGroups = new ListItemCollection();
                    _AvailableGroups.Add(new ListItem("Default", "0"));
                    foreach (MenuGroup group in MenuGroup.FetchAll())
                    {
                        _AvailableGroups.Add(new ListItem(group.Title, $"{group.ID}"));
                    }
                }
                return _AvailableGroups;
            }
        }
    }
}
