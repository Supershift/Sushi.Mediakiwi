using System;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using System.Web;
using Sushi.Mediakiwi.Framework;
using Sushi.Mediakiwi.Data;
using Sushi.Mediakiwi.UI;
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
            this.ListSave += MenuList_ListSave;
            this.ListDelete += MenuList_ListDelete;
            this.ListLoad += MenuList_ListLoad;
            this.ListSearch += MenuList_ListSearch;
            this.ListDelete += MenuList_ListDelete;
        }

        async Task MenuList_ListSave(ComponentListEventArgs e)
        {
            Implement.Name = this.Name;
            Implement.RoleID = this.RoleID;
            Implement.SiteID = this.SiteID;
            Implement.IsActive = this.Active;
            await Implement.SaveAsync();

            //var items = (from item in this.Items where item.Position == 1 select item).ToArray();
            SaveItem(0, this.MenuItem0);
            SaveItem(1, this.MenuItem1);
            SaveItem(2, this.MenuItem2);
            SaveItem(3, this.MenuItem3);
            SaveItem(4, this.MenuItem4);
            SaveItem(5, this.MenuItem5);
            SaveItem(6, this.MenuItem6);
            SaveItem(7, this.MenuItem7);
            SaveItem(8, this.MenuItem8);
            wim.FlushCache();
        }

        void SaveItem(int position, SubList container)
        {
            int index = 0;
            //  Remove obsolete
            var existingMenuItems = (from item in this.Items where item.Position == position select item).ToArray();
            foreach (var item in existingMenuItems)
            {
                var seek = (from mi in container.Items where mi.TextID == item.Tag select mi).Count();
                if (seek == 0)
                    item.Delete();
            }
            //  Add Or Update
            index = 0;
            foreach (var item in container.Items)
            {
                index++;
                var split = item.TextID.Split('_');
                var menuItem = (from mi in existingMenuItems where mi.Tag == item.TextID select mi).FirstOrDefault();
                if (menuItem == null) menuItem = new Sushi.Mediakiwi.Data.MenuItem();
                menuItem.MenuID = Implement.ID;
                menuItem.ItemID = Convert.ToInt32(split[1]);
                menuItem.TypeID = Convert.ToInt32(split[0].Replace("T", string.Empty));
                menuItem.Position = position;
                menuItem.Sort = index;
                menuItem.Save();
            }
        }

        async Task MenuList_ListDelete(ComponentListEventArgs e)
        {
            foreach (var item in this.Items)
                await item.DeleteAsync();
        }

        Sushi.Mediakiwi.Data.IMenu Implement { get; set; }
        Sushi.Mediakiwi.Data.IMenuItem[] Items { get; set; }

        async Task MenuList_ListLoad(ComponentListEventArgs e)
        {
            Implement = await Menu.SelectOneAsync(e.SelectedKey);
            if (Implement == null) Implement = new Sushi.Mediakiwi.Data.Menu();

            this.Name = Implement.Name;
            this.RoleID = Implement.RoleID;
            this.SiteID = Implement.SiteID;
            this.Active = Implement.IsActive;

            Items = await MenuItem.SelectAllAsync(Implement.ID);
            this.MenuItem0 = new SubList();
            this.MenuItem1 = new SubList();
            this.MenuItem2 = new SubList();
            this.MenuItem3 = new SubList();
            this.MenuItem4 = new SubList();
            this.MenuItem5 = new SubList();
            this.MenuItem6 = new SubList();
            this.MenuItem7 = new SubList();
            this.MenuItem8 = new SubList();

            LoadItem(0, this.MenuItem0);
            LoadItem(1, this.MenuItem1);
            LoadItem(2, this.MenuItem2);
            LoadItem(3, this.MenuItem3);
            LoadItem(4, this.MenuItem4);
            LoadItem(5, this.MenuItem5);
            LoadItem(6, this.MenuItem6);
            LoadItem(7, this.MenuItem7);
            LoadItem(8, this.MenuItem8);
        }

        void LoadItem(int position, SubList container)
        {
            var items = (from item in this.Items where item.Position == position select item).ToArray();

            List<string> tags = new List<string>();
            foreach (var item in items)
            {
                tags.Add(item.Tag);
            }
            var selection = SearchView.SelectAll(tags.ToArray());
            foreach (var item in items)
            {
                var find = (from x in selection where x.ID == item.Tag select x).FirstOrDefault();
                if (find != null)
                    container.Add(new SubList.SubListitem(find.ID, find.TitleHighlighted));
            }
        }

        async Task MenuList_ListSearch(ComponentListSearchEventArgs e)
        {
            wim.CanAddNewItem = true;
            wim.ListDataColumns.Add("ID", "ID", ListDataColumnType.UniqueIdentifier);
            wim.ListDataColumns.Add("Name", "Name", ListDataColumnType.HighlightPresent);
            wim.ListDataColumns.Add(new ListDataColumn("", "IsActive") { ColumnWidth = 30  });

            //if (wim.Grid.IsDataBinding)
            wim.ListDataAdd(await Sushi.Mediakiwi.Data.Menu.SelectAllAsync());
        }

        [Sushi.Mediakiwi.Framework.ContentListItem.TextField("Menu", 50, true, Expression = Sushi.Mediakiwi.Framework.OutputExpression.Alternating)]
        public string Name { get; set; }

        [Sushi.Mediakiwi.Framework.ContentListItem.Choice_Checkbox("Active", Expression = Sushi.Mediakiwi.Framework.OutputExpression.Alternating)]
        public bool Active { get; set; }

        [Sushi.Mediakiwi.Framework.ContentListItem.Choice_Dropdown("Role", "AvailableRoles", false, false, Expression = Sushi.Mediakiwi.Framework.OutputExpression.Alternating)]
        public int? RoleID { get; set; }

        [Sushi.Mediakiwi.Framework.ContentListItem.Choice_Dropdown("Site", "AvailableSites", false, false, Expression = Sushi.Mediakiwi.Framework.OutputExpression.Alternating)]
        public int? SiteID { get; set; }

        [Sushi.Mediakiwi.Framework.ContentListItem.SubListSelect("Home", "1a1fe050-219c-4f63-a697-7e2e8e790521", true, "", CanContainOneItem = true)]
        public Sushi.Mediakiwi.Data.SubList MenuItem0 { get; set; }

        [Sushi.Mediakiwi.Framework.ContentListItem.SubListSelect("Position #1", "1a1fe050-219c-4f63-a697-7e2e8e790521", true, "")]
        public Sushi.Mediakiwi.Data.SubList MenuItem1 { get; set; }

        [Sushi.Mediakiwi.Framework.ContentListItem.SubListSelect("Position #2", "1a1fe050-219c-4f63-a697-7e2e8e790521", false, "")]
        public Sushi.Mediakiwi.Data.SubList MenuItem2 { get; set; }

        [Sushi.Mediakiwi.Framework.ContentListItem.SubListSelect("Position #3", "1a1fe050-219c-4f63-a697-7e2e8e790521", false, "")]
        public Sushi.Mediakiwi.Data.SubList MenuItem3 { get; set; }

        [Sushi.Mediakiwi.Framework.ContentListItem.SubListSelect("Position #4", "1a1fe050-219c-4f63-a697-7e2e8e790521", false, "")]
        public Sushi.Mediakiwi.Data.SubList MenuItem4 { get; set; }

        [Sushi.Mediakiwi.Framework.ContentListItem.SubListSelect("Position #5", "1a1fe050-219c-4f63-a697-7e2e8e790521", false, "")]
        public Sushi.Mediakiwi.Data.SubList MenuItem5 { get; set; }

        [Sushi.Mediakiwi.Framework.ContentListItem.SubListSelect("Position #6", "1a1fe050-219c-4f63-a697-7e2e8e790521", false, "")]
        public Sushi.Mediakiwi.Data.SubList MenuItem6 { get; set; }

        [Sushi.Mediakiwi.Framework.ContentListItem.SubListSelect("Position #7", "1a1fe050-219c-4f63-a697-7e2e8e790521", false, "")]
        public Sushi.Mediakiwi.Data.SubList MenuItem7 { get; set; }

        [Sushi.Mediakiwi.Framework.ContentListItem.SubListSelect("Position #8", "1a1fe050-219c-4f63-a697-7e2e8e790521", false, "")]
        public Sushi.Mediakiwi.Data.SubList MenuItem8 { get; set; }


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
                    //Page.Trace.Write("AvailableRoles get{}", string.Format("{0}-{1}", SearchRole, SearchRole2));
                    _AvailableRoles = new ListItemCollection();
                    _AvailableRoles.Add(new ListItem("Select a role", ""));
                    foreach (Sushi.Mediakiwi.Data.ApplicationRole role in Sushi.Mediakiwi.Data.ApplicationRole.SelectAll())
                    {
                        _AvailableRoles.Add(new ListItem(role.Name, role.ID.ToString()));
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
                    //Page.Trace.Write("AvailableRoles get{}", string.Format("{0}-{1}", SearchRole, SearchRole2));
                    _AvailableSites = new ListItemCollection();
                    _AvailableSites.Add(new ListItem("Select a site", ""));
                    foreach (Sushi.Mediakiwi.Data.Site site in Sushi.Mediakiwi.Data.Site.SelectAll())
                    {
                        _AvailableSites.Add(new ListItem(site.Name, site.ID.ToString()));
                    }
                }
                return _AvailableSites;
            }
        }
    }
}
