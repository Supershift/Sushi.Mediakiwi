using Sushi.Mediakiwi.Data.MicroORM;
using Sushi.MicroORM.Mapping;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System.Linq;

namespace Sushi.Mediakiwi.Data
{
    [DataMap(typeof(MenuItemViewMap))]
    public class MenuItemView : IMenuItemView
    {
        public class MenuItemViewMap : DataMap<MenuItemView>
        {
            public MenuItemViewMap()
            {
                Table("wim_MenuItems");
                Id(x => x.ID, "Menu_Key").Identity().SqlType(SqlDbType.Int);
                Map(x => x.MenuID, "MenuItem_Menu_Key").SqlType(SqlDbType.Int);
                Map(x => x.DashboardID, "MenuItem_Dashboard_Key").SqlType(SqlDbType.Int);
                Map(x => x.ItemID, "MenuItem_Item_Key").SqlType(SqlDbType.Int);
                Map(x => x.Position, "MenuItem_Position").SqlType(SqlDbType.Int);
                Map(x => x.Sort, "MenuItem_Order").SqlType(SqlDbType.Int);
                Map(x => x.TypeID, "MenuItem_Type_Key").SqlType(SqlDbType.Int);
                Map(x => x.Name, "SearchView_Title").SqlType(SqlDbType.NVarChar).Length(50);
                Map(x => x.Section, "MenuItem_Section").SqlType(SqlDbType.Int);
                Map(x => x.SiteID, "SearchView_Site_Key").SqlType(SqlDbType.Int);
            }
        }

        #region properties

        public string Name { get; set; }
        public int? Section { get; set; }
        public int? SiteID { get; set; }

        /// <summary>
        /// Uniqe identifier of the MenuItem
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Identifier of the Menu this MenuItem belongs to
        /// </summary>
        public int MenuID { get; set; }

        /// <summary>
        /// Identifier of the Dashboard this MenuItem belongs to
        /// </summary>
        public int DashboardID { get; set; }

        /// <summary>
        /// What type this MenuItem is.
        /// <list>
        /// <item><description>1: List</description></item>
        /// <item><description>2: Folder</description></item>
        /// <item><description>3: Page</description></item>
        /// <item><description>4: Dashboard</description></item>
        /// <item><description>5: Gallery</description></item>
        /// </list>
        /// </summary>
        public int TypeID { get; set; }

        /// <summary>
        /// Corresponding Item Id.
        /// </summary>
        public int ItemID { get; set; }

        /// <summary>
        /// Position within the menu.
        /// </summary>
        public int Position { get; set; }

        /// <summary>
        /// The Order within, when is has multiple in the same Position .
        /// </summary>
        public int Sort { get; set; }

        /// <summary>
        /// Tag of the MenuItem, format: 'TypeID'_'ItemID'
        /// </summary>
        public string Tag
        {
            get { return string.Format("{0}_{1}", this.TypeID, this.ItemID); }
        }

        int IMenuItemView.Section { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        int IMenuItemView.SiteID { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        #endregion properties

        public static IMenuItemView[] SelectAll(int siteID, int roleID) 
        { 
            return SelectAll(siteID, roleID, "");
        }

        public static IMenuItemView[] SelectAll(int siteID, int roleID, string groupTag)
        {
            return SelectAll(siteID, roleID, groupTag, 1, 2, 3, 4, 5, 6, 7, 8);
        }

        public static async Task<List<IMenuItemView>> SelectAllAsync(int siteID, int roleID)
        {
            return await SelectAllAsync(siteID, roleID, "");
        }

        public static async Task<List<IMenuItemView>> SelectAllAsync(int siteID, int roleID, string groupTag)
        {
            return await SelectAllAsync(siteID, roleID, groupTag,  1, 2, 3, 4, 5, 6, 7, 8).ConfigureAwait(false);
        }

        /// <summary>
        /// Selects all.
        /// </summary>
        /// <param name="siteID">The site ID.</param>
        /// <param name="roleID">The role ID.</param>
        /// <param name="groupTag">The group tag.</param>
        /// <param name="items">The positions.</param>
        /// <returns></returns>
        public static IMenuItemView[] SelectAll(int siteID, int roleID, string groupTag, params int[] items)
        {
            var connector = ConnectorFactory.CreateConnector<MenuItemView>();
            var filter = connector.CreateQuery();
            filter.AddParameter("Site", SqlDbType.Int, siteID);
            filter.AddParameter("Role", SqlDbType.Int, roleID);

            List<MenuItemView> result;
            if (items != null && items.Length > 0)
            {
                int i = 0;
                var sql_in = new List<string>();
                foreach(var item in items)
                {
                    i++;
                    filter.AddParameter($"p{i}", SqlDbType.Int, item);
                    sql_in.Add($"@p{i}");
                }

                // Include groupTag
                if (string.IsNullOrWhiteSpace(groupTag) == false)
                {
                    filter.AddParameter("@groupTag", SqlDbType.NVarChar, groupTag.ToUpperInvariant());

                    result = connector.FetchAll(@"
                                            select 
                                                wim_MenuItems.*
                                            ,   SearchView_Title
                                            ,   SearchView_Site_Key 
                                            from 
                                                wim_Menus
                                                join wim_MenuItems on Menu_Key = MenuItem_Menu_Key
	                                            join wim_SearchView on SearchView_Type = MenuItem_Type_Key and MenuItem_Item_Key = SearchView_Item_Key
                                                join wim_MenuGroups on Menu_Group_Key = MenuGroup_Key
                                            where
                                                ISNULL(Menu_Site_key, @Site) = @Site
                                                and ISNULL(Menu_Role_Key, @Role) = @Role
                                                and Menu_IsActive = 1
                                                and MenuGroup_Tag = @groupTag
                                                and MenuItem_Position in (" + string.Join(",", sql_in) + @")
                                            order by
                                                MenuItem_Position asc, MenuItem_Order asc 
                                            ", filter);
                }
                else
                {
                    result = connector.FetchAll(@"
                                            select 
                                                wim_MenuItems.*
                                            ,   SearchView_Title
                                            ,   SearchView_Site_Key 
                                            from 
                                                wim_Menus
                                                join wim_MenuItems on Menu_Key = MenuItem_Menu_Key
	                                            join wim_SearchView on SearchView_Type = MenuItem_Type_Key and MenuItem_Item_Key = SearchView_Item_Key
                                            where
                                                ISNULL(Menu_Site_key, @Site) = @Site
                                                and ISNULL(Menu_Role_Key, @Role) = @Role
                                                and Menu_IsActive = 1
                                                and MenuItem_Position in (" + string.Join(",", sql_in) + @")
                                            order by
                                                MenuItem_Position asc, MenuItem_Order asc 
                                            ", filter);
                }
            }
            else
            {
                // Include groupTag
                if (string.IsNullOrWhiteSpace(groupTag) == false)
                {
                    filter.AddParameter("@groupTag", SqlDbType.NVarChar, groupTag.ToUpperInvariant());
                    result = connector.FetchAll(@"
                                            select 
                                                wim_MenuItems.*
                                            ,   SearchView_Title
                                            ,   SearchView_Site_Key 
                                            from 
                                                wim_Menus
                                                join wim_MenuItems on Menu_Key = MenuItem_Menu_Key
	                                            join wim_SearchView on SearchView_Type = MenuItem_Type_Key and MenuItem_Item_Key = SearchView_Item_Key
                                                join wim_MenuGroups on Menu_Group_Key = MenuGroup_Key
                                            where
                                                ISNULL(Menu_Site_key, @Site) = @Site
                                                and ISNULL(Menu_Role_Key, @Role) = @Role
                                                and Menu_IsActive = 1
                                                and MenuGroup_Tag = @groupTag
                                            order by
                                                MenuItem_Position asc, MenuItem_Order asc 
                                            ", filter);
                }
                else
                {
                    result = connector.FetchAll(@"
                                            select 
                                                wim_MenuItems.*
                                            ,   SearchView_Title
                                            ,   SearchView_Site_Key 
                                            from 
                                                wim_Menus
                                                join wim_MenuItems on Menu_Key = MenuItem_Menu_Key
	                                            join wim_SearchView on SearchView_Type = MenuItem_Type_Key and MenuItem_Item_Key = SearchView_Item_Key
                                            where
                                                ISNULL(Menu_Site_key, @Site) = @Site
                                                and ISNULL(Menu_Role_Key, @Role) = @Role
                                                and Menu_IsActive = 1
                                            order by
                                                MenuItem_Position asc, MenuItem_Order asc 
                                            ", filter);
                }
            }
            return result.ToArray();
        }

        /// <summary>
        /// Selects all.
        /// </summary>
        /// <param name="siteID">The site ID.</param>
        /// <param name="roleID">The role ID.</param>
        /// <param name="groupTag">The group tag.</param>
        /// <param name="items">The positions.</param>
        /// <returns></returns>
        public static async Task<List<IMenuItemView>> SelectAllAsync(int siteID, int roleID, string groupTag, params int[] items)
        {
            var connector = ConnectorFactory.CreateConnector<MenuItemView>();
            var filter = connector.CreateQuery();
            filter.AddParameter("Site", SqlDbType.Int, siteID);
            filter.AddParameter("Role", SqlDbType.Int, roleID);

            List<MenuItemView> result;
            if (items != null && items.Length > 0)
            {
                int i = 0;
                var sql_in = new List<string>();
                foreach (var item in items)
                {
                    i++;
                    filter.AddParameter($"p{i}", SqlDbType.Int, item);
                    sql_in.Add($"@p{i}");
                }

                // Include groupTag
                if (string.IsNullOrWhiteSpace(groupTag) == false)
                {
                    filter.AddParameter("@groupTag", SqlDbType.NVarChar, groupTag.ToUpperInvariant());

                    result = await connector.FetchAllAsync(@"
                                            select 
                                                wim_MenuItems.*
                                            ,   SearchView_Title
                                            ,   SearchView_Site_Key 
                                            from 
                                                wim_Menus
                                                join wim_MenuItems on Menu_Key = MenuItem_Menu_Key
	                                            join wim_SearchView on SearchView_Type = MenuItem_Type_Key and MenuItem_Item_Key = SearchView_Item_Key
                                                join wim_MenuGroups on Menu_Group_Key = MenuGroup_Key
                                            where
                                                ISNULL(Menu_Site_key, @Site) = @Site
                                                and ISNULL(Menu_Role_Key, @Role) = @Role
                                                and Menu_IsActive = 1
                                                and MenuGroup_Tag = @groupTag
                                                and MenuItem_Position in (" + string.Join(",", sql_in) + @")
                                            order by
                                                MenuItem_Position asc, MenuItem_Order asc 
                                            ", filter);
                }
                else
                {
                    result = await connector.FetchAllAsync(@"
                                            select 
                                                wim_MenuItems.*
                                            ,   SearchView_Title
                                            ,   SearchView_Site_Key 
                                            from 
                                                wim_Menus
                                                join wim_MenuItems on Menu_Key = MenuItem_Menu_Key
	                                            join wim_SearchView on SearchView_Type = MenuItem_Type_Key and MenuItem_Item_Key = SearchView_Item_Key
                                            where
                                                ISNULL(Menu_Site_key, @Site) = @Site
                                                and ISNULL(Menu_Role_Key, @Role) = @Role
                                                and Menu_IsActive = 1
                                                and MenuItem_Position in (" + string.Join(",", sql_in) + @")
                                            order by
                                                MenuItem_Position asc, MenuItem_Order asc 
                                            ", filter);
                }
            }
            else
            {
                // Include groupTag
                if (string.IsNullOrWhiteSpace(groupTag) == false)
                {
                    filter.AddParameter("@groupTag", SqlDbType.NVarChar, groupTag.ToUpperInvariant());
                    result = await connector.FetchAllAsync(@"
                                            select 
                                                wim_MenuItems.*
                                            ,   SearchView_Title
                                            ,   SearchView_Site_Key 
                                            from 
                                                wim_Menus
                                                join wim_MenuItems on Menu_Key = MenuItem_Menu_Key
	                                            join wim_SearchView on SearchView_Type = MenuItem_Type_Key and MenuItem_Item_Key = SearchView_Item_Key
                                                join wim_MenuGroups on Menu_Group_Key = MenuGroup_Key
                                            where
                                                ISNULL(Menu_Site_key, @Site) = @Site
                                                and ISNULL(Menu_Role_Key, @Role) = @Role
                                                and Menu_IsActive = 1
                                                and MenuGroup_Tag = @groupTag
                                            order by
                                                MenuItem_Position asc, MenuItem_Order asc 
                                            ", filter);
                }
                else
                {
                    result = await connector.FetchAllAsync(@"
                                            select 
                                                wim_MenuItems.*
                                            ,   SearchView_Title
                                            ,   SearchView_Site_Key 
                                            from 
                                                wim_Menus
                                                join wim_MenuItems on Menu_Key = MenuItem_Menu_Key
	                                            join wim_SearchView on SearchView_Type = MenuItem_Type_Key and MenuItem_Item_Key = SearchView_Item_Key
                                            where
                                                ISNULL(Menu_Site_key, @Site) = @Site
                                                and ISNULL(Menu_Role_Key, @Role) = @Role
                                                and Menu_IsActive = 1
                                            order by
                                                MenuItem_Position asc, MenuItem_Order asc 
                                            ", filter);
                }
            }

            return result.ToList<IMenuItemView>();
        }


        /// <summary>
        /// Selects al menu items for use on the dashboard
        /// </summary>
        /// <param name="dashboardID">The dashboard identifier.</param>
        /// <returns></returns>
        public static IMenuItemView[] SelectAll_Dashboard(int dashboardID)
        {
            var connector = ConnectorFactory.CreateConnector<MenuItemView>();
            var filter = connector.CreateQuery();
            filter.AddParameter("Dashboard", SqlDbType.Int, dashboardID);
            
            var result = connector.FetchAll(@"
select 
    wim_MenuItems.*
,   SearchView_Title
,   SearchView_Site_Key 
from 
    wim_MenuItems
	join wim_SearchView on SearchView_Type = MenuItem_Type_Key and MenuItem_Item_Key = SearchView_Item_Key
where
    MenuItem_Dashboard_key = @Dashboard
order by
    MenuItem_Position asc, MenuItem_Order asc 
");
            string sql = string.Format(@"

"
                , dashboardID);
            return result.ToArray();
        }
    }
}