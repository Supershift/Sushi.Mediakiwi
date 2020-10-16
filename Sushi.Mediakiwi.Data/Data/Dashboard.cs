using Sushi.MicroORM;
using Sushi.MicroORM.Mapping;
using System;
using System.Threading.Tasks;
using Sushi.Mediakiwi.Data.MircoORM;

namespace Sushi.Mediakiwi.Data
{
    /// <summary>
    /// Represents a Dashboard entity.
    /// </summary>
    [DataMap(typeof(DashboardMap))]
    public class Dashboard
    {
        public class DashboardMap : DataMap<Dashboard>
        {
            public DashboardMap()
            {
                Table("wim_Dashboards");
                Id(x => x.ID, "Dashboard_Key").Identity();
                Map(x => x.GUID, "Dashboard_Guid");
                Map(x => x.Name, "Dashboard_Name").Length(50);
                Map(x => x.Title, "Dashboard_Title").Length(50);
                Map(x => x.Body, "Dashboard_Body");
                Map(x => x.Created, "Dashboard_Created");
            }
        }

        /// <summary>
        /// Selects all.
        /// </summary>
        /// <returns></returns>
        public static Dashboard[] SelectAll()
        {
            var connector = ConnectorFactory.CreateConnector<Dashboard>();
            var filter = connector.CreateDataFilter();
            filter.AddOrder(x => x.Title);

            return connector.FetchAll(filter).ToArray();
        }

        /// <summary>
        /// Selects all.
        /// </summary>
        /// <returns></returns>
        public static async Task<Dashboard[]> SelectAllAsync()
        {
            var connector = ConnectorFactory.CreateConnector<Dashboard>();
            var filter = connector.CreateDataFilter();
            filter.AddOrder(x => x.Title);

            var result = await connector.FetchAllAsync(filter);
            return result.ToArray();
        }

        /// <summary>
        /// Selects the one.
        /// </summary>
        /// <param name="ID">The ID.</param>
        /// <returns></returns>
        public static Dashboard SelectOne(int ID)
        {
            var connector = ConnectorFactory.CreateConnector<Dashboard>();

            return connector.FetchSingle(ID);
        }

        /// <summary>
        /// Selects the one Async.
        /// </summary>
        /// <param name="ID">The ID.</param>
        /// <returns></returns>
        public static async Task<Dashboard> SelectOneAsync(int ID)
        {
            var connector = ConnectorFactory.CreateConnector<Dashboard>();

            return await connector.FetchSingleAsync(ID);
        }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>The id.</value>
        public int ID { get; set; }

        private Guid m_GUID;

        /// <summary>
        /// Gets or sets the GUID.
        /// </summary>
        /// <value>The GUID.</value>
        public Guid GUID
        {
            get
            {
                if (this.m_GUID == Guid.Empty)
                    this.m_GUID = Guid.NewGuid();
                return m_GUID;
            }
            set { m_GUID = value; }
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>The title.</value>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>The title.</value>
        public string Body { get; set; }

        private DateTime m_Created;

        /// <summary>
        /// Gets or sets the created.
        /// </summary>
        /// <value>The created.</value>
        public DateTime Created
        {
            get
            {
                if (m_Created == DateTime.MinValue)
                    m_Created = Common.DatabaseDateTime;
                return m_Created;
            }
            set { m_Created = value; }
        }

        /// <summary>
        /// Updates the specified dashboard.
        /// </summary>
        /// <param name="dashboard">The dashboard.</param>
        /// <param name="column">The column.</param>
        /// <param name="subList">The sub list.</param>
        public static void Update(int dashboard, int column, SubList subList)
        {
            Prepare(dashboard, column);
            if (subList != null && subList.Items != null && subList.Items.Length > 0)
            {
                var connector = ConnectorFactory.CreateConnector<Dashboard>();
                var filter = connector.CreateDataFilter();
                filter.AddParameter("@columnId", column);
                filter.AddParameter("@dashboardId", dashboard);

                int sortOrder = 0;
                foreach (SubList.SubListitem item in subList.Items)
                {
                    sortOrder++;
                    filter.AddParameter("@thisId", item.ID);
                    filter.AddParameter("@sortOrder", sortOrder);

                    int count = connector.ExecuteScalar<int>("SELECT COUNT(*) FROM [wim_DashboardLists] WHERE [DashboardList_Dashboard_Key] = @dashboardId AND [DashboardList_List_Key] = @thisId AND [DashboardList_Column] = @columnId", filter);
                    if (count == 0)
                        connector.ExecuteNonQuery("INSERT INTO [wim_DashboardLists]([DashboardList_Dashboard_Key], [DashboardList_List_Key], [DashboardList_Column], [DashboardList_Update], [DashboardList_SortOrder]) VALUES (@dashboardId, @thisId, @columnId, 1, @sortOrder)", filter);
                    else
                        connector.ExecuteNonQuery("UPDATE [wim_DashboardLists] SET [DashboardList_Update] = 1, [DashboardList_SortOrder] = @sortOrder WHERE [DashboardList_Dashboard_Key] = @dashboardId AND [DashboardList_List_Key] = @thisId AND [DashboardList_Column] = @columnId", filter);
					connector.Cache.FlushRegion(connector.CacheRegion);
                }
            }
            CleanUp(dashboard, column);
        }

        /// <summary>
        /// Updates the specified dashboard.
        /// </summary>
        /// <param name="dashboard">The dashboard.</param>
        /// <param name="column">The column.</param>
        /// <param name="subList">The sub list.</param>
        public static async Task UpdateAsync(int dashboard, int column, SubList subList)
        {
            await PrepareAsync(dashboard, column);
            if (subList != null && subList.Items != null && subList.Items.Length > 0)
            {
                var connector = ConnectorFactory.CreateConnector<Dashboard>();
                var filter = connector.CreateDataFilter();
                filter.AddParameter("@columnId", column);
                filter.AddParameter("@dashboardId", dashboard);

                int sortOrder = 0;
                foreach (SubList.SubListitem item in subList.Items)
                {
                    sortOrder++;
                    filter.AddParameter("@thisId", item.ID);
                    filter.AddParameter("@sortOrder", sortOrder);

                    int count = await connector.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM [wim_DashboardLists] WHERE [DashboardList_Dashboard_Key] = @dashboardId AND [DashboardList_List_Key] = @thisId AND [DashboardList_Column] = @columnId", filter);
                    if (count == 0)
                        await connector.ExecuteNonQueryAsync("INSERT INTO [wim_DashboardLists]([DashboardList_Dashboard_Key], [DashboardList_List_Key], [DashboardList_Column], [DashboardList_Update], [DashboardList_SortOrder]) VALUES (@dashboardId, @thisId, @columnId, 1, @sortOrder)", filter);
                    else
                        await connector.ExecuteNonQueryAsync("UPDATE [wim_DashboardLists] SET [DashboardList_Update] = 1, [DashboardList_SortOrder] = @sortOrder WHERE [DashboardList_Dashboard_Key] = @dashboardId AND [DashboardList_List_Key] = @thisId AND [DashboardList_Column] = @columnId", filter);
					connector.Cache.FlushRegion(connector.CacheRegion);
                }
            }
            await CleanUpAsync(dashboard, column);
        }

        /// <summary>
        /// Prepares the specified dashboard.
        /// </summary>
        /// <param name="dashboard">The dashboard.</param>
        /// <param name="column">The column.</param>
        private static void Prepare(int dashboard, int column)
        {
            var connector = ConnectorFactory.CreateConnector<Dashboard>();
            var filter = connector.CreateDataFilter();
            filter.AddParameter("@columnId", column);
            filter.AddParameter("@dashboardId", dashboard);

            connector.ExecuteNonQuery("UPDATE [wim_DashboardLists] SET [DashboardList_Update] = 0 WHERE [DashboardList_Dashboard_Key] = @dashboardId AND [DashboardList_Column] = @columnId", filter);
			connector.Cache.FlushRegion(connector.CacheRegion);
        }

        /// <summary>
        /// Prepares the specified dashboard.
        /// </summary>
        /// <param name="dashboard">The dashboard.</param>
        /// <param name="column">The column.</param>
        private static async Task PrepareAsync(int dashboard, int column)
        {
            var connector = ConnectorFactory.CreateConnector<Dashboard>();
            var filter = connector.CreateDataFilter();
            filter.AddParameter("@columnId", column);
            filter.AddParameter("@dashboardId", dashboard);

            await connector.ExecuteNonQueryAsync("UPDATE [wim_DashboardLists] SET [DashboardList_Update] = 0 WHERE [DashboardList_Dashboard_Key] = @dashboardId AND [DashboardList_Column] = @columnId", filter);
			connector.Cache.FlushRegion(connector.CacheRegion);
        }

        /// <summary>
        /// Cleans up.
        /// </summary>
        /// <param name="dashboard">The dashboard.</param>
        /// <param name="column">The column.</param>
        private static void CleanUp(int dashboard, int column)
        {
            var connector = ConnectorFactory.CreateConnector<Dashboard>();
            var filter = connector.CreateDataFilter();
            filter.AddParameter("@columnId", column);
            filter.AddParameter("@dashboardId", dashboard);

            connector.ExecuteNonQuery("DELETE FROM [wim_DashboardLists] WHERE [DashboardList_Update] = 0 AND [DashboardList_Dashboard_Key] = @dashboardId AND [DashboardList_Column] = @columnId", filter);
			connector.Cache.FlushRegion(connector.CacheRegion);
        }

        /// <summary>
        /// Cleans up.
        /// </summary>
        /// <param name="dashboard">The dashboard.</param>
        /// <param name="column">The column.</param>
        private static async Task CleanUpAsync(int dashboard, int column)
        {
            var connector = ConnectorFactory.CreateConnector<Dashboard>();
            var filter = connector.CreateDataFilter();
            filter.AddParameter("@columnId", column);
            filter.AddParameter("@dashboardId", dashboard);

            await connector.ExecuteNonQueryAsync("DELETE FROM [wim_DashboardLists] WHERE [DashboardList_Update] = 0 AND [DashboardList_Dashboard_Key] = @dashboardId AND [DashboardList_Column] = @columnId", filter);
			connector.Cache.FlushRegion(connector.CacheRegion);
        }

        private IComponentList[] _DashboardTarget;

        /// <summary>
        /// Gets the dashboard target1.
        /// </summary>
        /// <value>The dashboard target1.</value>
        public IComponentList[] DashboardTarget
        {
            get
            {
                if (_DashboardTarget == null)
                    _DashboardTarget = ComponentList.SelectAllDashboardLists(this.ID, 0);
                return _DashboardTarget;
            }
        }
    }
}