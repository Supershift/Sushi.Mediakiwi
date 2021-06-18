using Sushi.MicroORM.Mapping;
using System.Data;
using System.Threading.Tasks;
using Sushi.Mediakiwi.Data.MicroORM;

namespace Sushi.Mediakiwi.Data
{
    [DataMap(typeof(MenuMap))]
    public class Menu : IMenu
    {       
        public class MenuMap : DataMap<Menu>
        {
            public MenuMap()
            {
                Table("wim_Menus");
                Id(x => x.ID, "Menu_Key").Identity().SqlType(SqlDbType.Int);
                Map(x => x.Name, "Menu_Name").SqlType(SqlDbType.NVarChar).Length(50);
                Map(x => x.SiteID, "Menu_Site_key").SqlType(SqlDbType.Int);
                Map(x => x.RoleID, "Menu_Role_Key").SqlType(SqlDbType.Int);
                Map(x => x.IsActive, "Menu_IsActive").SqlType(SqlDbType.Bit);
            }
        }
        #region properties
        /// <summary>
        /// Uniqe identifier of the Menu
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Displayname of the Menu
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Asset key of the Site for this menu
        /// </summary>
        public int? SiteID { get; set; }

        /// <summary>
        /// Role of the Menu
        /// </summary>
        public int? RoleID { get; set; }

        /// <summary>
        /// Is this menu active (visible)?
        /// </summary>
        public bool IsActive { get; set; }

        #endregion properties

        /// <summary>
        /// Select a Menu based on its primary key
        /// </summary>
        /// <param name="ID">Uniqe identifier of the Menu</param>
        public static IMenu SelectOne(int ID)
        {
            var connector = ConnectorFactory.CreateConnector<Menu>();
            return connector.FetchSingle(ID);
        }

        /// <summary>
        /// Select a Menu based on its primary key
        /// </summary>
        /// <param name="ID">Uniqe identifier of the Menu</param>
        public static async Task<IMenu> SelectOneAsync(int ID)
        {
            var connector = ConnectorFactory.CreateConnector<Menu>();
            return await connector.FetchSingleAsync(ID);
        }

        /// <summary>
        /// Select all active Menu's
        /// </summary>
        public static IMenu[] SelectAll()
        {
            var connector = ConnectorFactory.CreateConnector<Menu>();
            var filter = connector.CreateDataFilter();
            return connector.FetchAll(filter).ToArray();
        }

        /// <summary>
        /// Select all active Menu's
        /// </summary>
        public static async Task<IMenu[]> SelectAllAsync()
        {
            var connector = ConnectorFactory.CreateConnector<Menu>();
            var filter = connector.CreateDataFilter();
            var result = await connector.FetchAllAsync(filter);
            return result.ToArray();
        }

        /// <summary>
        /// Saves the Menu to database
        /// </summary>
        public void Save()
        {
            var connector = ConnectorFactory.CreateConnector<Menu>();
            connector.Save(this);
        }

        /// <summary>
        /// Saves the Menu to database
        /// </summary>
        public async Task SaveAsync()
        {
            var connector = ConnectorFactory.CreateConnector<Menu>();
            await connector.SaveAsync(this);
        }

        /// <summary>
        /// Deletes the Menu from the database
        /// </summary>
        public virtual void Delete()
        {
            var connector = ConnectorFactory.CreateConnector<Menu>();
            connector.Delete(this);
        }

        /// <summary>
        /// Deletes the Menu from the database
        /// </summary>
        public async Task DeleteAsync()
        {
            var connector = ConnectorFactory.CreateConnector<Menu>();
            await connector.DeleteAsync(this);
        }
    }
}