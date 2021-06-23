using Sushi.Mediakiwi.Data.MicroORM;
using Sushi.MicroORM.Mapping;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Data
{
    /// <summary>
    ///
    /// </summary>
    [DataMap(typeof(DashboardListItemMap))]
    public class DashboardListItem
    {
        public class DashboardListItemMap : DataMap<DashboardListItem>
        {
            public DashboardListItemMap()
            {
                Table("wim_DashboardLists");

                Map(x => x.DashboardID, "DashboardList_Dashboard_Key");
                Map(x => x.ListID, "DashboardList_List_Key");
                Map(x => x.ColumnID, "DashboardList_Column");
                Map(x => x.SortOrder, "DashboardList_SortOrder");
            }
        }

        /// <summary>
        /// Selects all.
        /// </summary>
        /// <returns></returns>
        public static DashboardListItem[] SelectAll()
        {
            var connector = ConnectorFactory.CreateConnector<DashboardListItem>();
            var filter = connector.CreateDataFilter();
            filter.AddOrder(x => x.SortOrder);

            return connector.FetchAll(filter).ToArray();
        }

        /// <summary>
        /// Selects all Async.
        /// </summary>
        /// <returns></returns>
        public static async Task<DashboardListItem[]> SelectAllAsync()
        {
            var connector = ConnectorFactory.CreateConnector<DashboardListItem>();
            var filter = connector.CreateDataFilter();
            filter.AddOrder(x => x.SortOrder);

            var result = await connector.FetchAllAsync(filter);
            return result.ToArray();
        }

        /// <summary>
        /// Selects all.
        /// </summary>
        /// <param name="dashboardID">The dashboard ID.</param>
        /// <returns></returns>
        public static DashboardListItem[] SelectAll(int dashboardID)
        {
            var connector = ConnectorFactory.CreateConnector<DashboardListItem>();
            var filter = connector.CreateDataFilter();
            filter.AddOrder(x => x.SortOrder);
            filter.Add(x => x.DashboardID, dashboardID);

            return connector.FetchAll(filter).ToArray();
        }

        /// <summary>
        /// Selects all Async.
        /// </summary>
        /// <param name="dashboardID">The dashboard ID.</param>
        /// <returns></returns>
        public static async Task<DashboardListItem[]> SelectAllAsync(int dashboardID)
        {
            var connector = ConnectorFactory.CreateConnector<DashboardListItem>();
            var filter = connector.CreateDataFilter();
            filter.AddOrder(x => x.SortOrder);
            filter.Add(x => x.DashboardID, dashboardID);

            var result = await connector.FetchAllAsync(filter);
            return result.ToArray();
        }

        /// <summary>
        /// Gets or sets the dashboard ID.
        /// </summary>
        /// <value>The dashboard ID.</value>
        public int DashboardID { get; set; }

        /// <summary>
        /// Gets or sets the list ID.
        /// </summary>
        /// <value>The list ID.</value>
        public int ListID { get; set; }

        /// <summary>
        /// Gets or sets the column ID.
        /// </summary>
        /// <value>The column ID.</value>
        public int ColumnID { get; set; }

        /// <summary>
        /// Gets or sets the column ID.
        /// </summary>
        /// <value>The column ID.</value>
        public int SortOrder { get; set; }
    }
}