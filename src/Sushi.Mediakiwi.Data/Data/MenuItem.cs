using Sushi.MicroORM.Mapping;
using System.Data;
using System.Threading.Tasks;
using Sushi.Mediakiwi.Data.MicroORM;

namespace Sushi.Mediakiwi.Data
{
    [DataMap(typeof(MenuItemMap))]
    public class MenuItem : IMenuItem
    {
        public class MenuItemMap : DataMap<MenuItem>
        {
            public MenuItemMap()
            {
                Table("wim_MenuItems");
                Id(x => x.ID, "MenuItem_Key").Identity().SqlType(SqlDbType.Int);
                Map(x => x.MenuID, "MenuItem_Menu_Key").SqlType(SqlDbType.Int);
                Map(x => x.DashboardID, "MenuItem_Dashboard_Key").SqlType(SqlDbType.Int);
                Map(x => x.ItemID, "MenuItem_Item_Key").SqlType(SqlDbType.Int);
                Map(x => x.Position, "MenuItem_Position").SqlType(SqlDbType.Int);
                Map(x => x.Sort, "MenuItem_Order").SqlType(SqlDbType.Int);
                Map(x => x.TypeID, "MenuItem_Type_Key").SqlType(SqlDbType.Int);
            }
        }

        #region properties

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

        /// <summary>
        /// Addition to the url, when this MenuItem is called.
        /// </summary>
        public string Url
        {
            get
            {
                switch (this.TypeID)
                {
                    case 1:
                        return string.Concat("?list=", this.ItemID);

                    case 2:
                        return string.Concat("?folder=", this.ItemID);

                    case 3:
                        return string.Concat("?page=", this.ItemID);

                    case 4:
                        return string.Concat("?dashboard=", this.ItemID);

                    case 5:
                        return string.Concat("?gallery=", this.ItemID);

                    default:
                        return "#";
                }
            }
        }

        /// <summary>
        /// Addition when this MenuItem is called.
        /// </summary>
        public virtual string Name
        {
            get
            {
                switch (this.TypeID)
                {
                    case 1:
                        return string.Concat("?list=", this.ItemID);

                    case 2:
                        return string.Concat("?folder=", this.ItemID);

                    case 3:
                        return string.Concat("?page=", this.ItemID);

                    case 4:
                        return string.Concat("?dashboard=", this.ItemID);

                    case 5:
                        return string.Concat("?gallery=", this.ItemID);

                    default:
                        return "#";
                }
            }
        }

        #endregion properties

        /// <summary>
        /// Select all MenuItems for a specific Menu
        /// </summary>
        /// <param name="menuID">Identifier of the Menu the MenuItems belongs to.</param>
        public static IMenuItem[] SelectAll(int menuID)
        {
            var connector = ConnectorFactory.CreateConnector<MenuItem>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.MenuID, menuID);
            filter.AddOrder(x => x.Position);
            filter.AddOrder(x => x.Sort);
            return connector.FetchAll(filter).ToArray();
        }

        /// <summary>
        /// Select all MenuItems for a specific Menu
        /// </summary>
        /// <param name="menuID">Identifier of the Menu the MenuItems belongs to.</param>
        public static async Task<IMenuItem[]> SelectAllAsync(int menuID)
        {
            var connector = ConnectorFactory.CreateConnector<MenuItem>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.MenuID, menuID);
            filter.AddOrder(x => x.Position);
            filter.AddOrder(x => x.Sort);
            var result = await connector.FetchAllAsync(filter);
            return result.ToArray();
        }

        /// <summary>
        /// Select all MenuItems for a specific Dashboard
        /// </summary>
        /// <param name="dashboardID">Identifier of the Dashboard the MenuItems belongs to.</param>
        public static IMenuItem[] SelectAll_Dashboard(int dashboardID)
        {
            var connector = ConnectorFactory.CreateConnector<MenuItem>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.DashboardID, dashboardID);
            filter.AddOrder(x => x.MenuID);
            filter.AddOrder(x => x.Position);
            filter.AddOrder(x => x.Sort);
            return connector.FetchAll(filter).ToArray();
        }

        /// <summary>
        /// Select all MenuItems for a specific Dashboard
        /// </summary>
        /// <param name="dashboardID">Identifier of the Dashboard the MenuItems belongs to.</param>
        public static async Task<IMenuItem[]> SelectAll_DashboardAsync(int dashboardID)
        {
            var connector = ConnectorFactory.CreateConnector<MenuItem>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.DashboardID, dashboardID);
            filter.AddOrder(x => x.MenuID);
            filter.AddOrder(x => x.Position);
            filter.AddOrder(x => x.Sort);
            var result = await connector.FetchAllAsync(filter);
            return result.ToArray();
        }

        /// <summary>
        /// Saves the MenuItem to database
        /// </summary>
        public void Save()
        {
            var connector = ConnectorFactory.CreateConnector<MenuItem>();
            connector.Save(this);
        }

        /// <summary>
        /// Saves the MenuItem to database
        /// </summary>
        public async Task SaveAsync()
        {
            var connector = ConnectorFactory.CreateConnector<MenuItem>();
            await connector.SaveAsync(this);
        }

        /// <summary>
        /// Deletes the MenuItem from the database
        /// </summary>
        public virtual void Delete()
        {
            var connector = ConnectorFactory.CreateConnector<MenuItem>();
            connector.Delete(this);
        }

        /// <summary>
        /// Deletes the MenuItem from the database
        /// </summary>
        public async Task DeleteAsync()
        {
            var connector = ConnectorFactory.CreateConnector<MenuItem>();
            await connector.DeleteAsync(this);
        }
    }
}