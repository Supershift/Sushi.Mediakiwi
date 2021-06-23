using Sushi.Mediakiwi.Data.Interfaces;
using Sushi.Mediakiwi.Data.MicroORM;
using Sushi.MicroORM;
using Sushi.MicroORM.Mapping;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Data
{
    [DataMap(typeof(SearchViewMap))]
    public class SearchView : ISearchView
    {
        public class SearchViewMap : DataMap<SearchView>
        {
            public SearchViewMap()
            {
                Table("wim_SearchView");

                Id(x => x.ID, "SearchView_Key");
                Map(x => x.Title, "SearchView_Title");
                Map(x => x.Description, "SearchView_Description");
                Map(x => x.SiteID, "SearchView_Site_Key");
                Map(x => x.FolderID, "SearchView_Folder_Key");
                Map(x => x.SortOrder, "SearchView_SortOrder");
                Map(x => x.TypeID, "SearchView_Type");
                Map(x => x.ItemID, "SearchView_Item_Key");
            }
        }

        #region Properties

        public virtual string TitleHighlighted
        {
            get
            {
                return string.Format("{0} <b>({1})</b>", this.Title, this.Type);
            }
        }

        public virtual string ID { get; set; }

        public virtual string Title { get; set; }

        public virtual string Description { get; set; }

        public virtual int SiteID { get; set; }

        public virtual int FolderID { get; set; }

        public virtual int SortOrder { get; set; }

        public virtual int TypeID { get; set; }

        public virtual int ItemID { get; set; }

        public virtual string Type
        {
            get
            {
                switch (TypeID)
                {
                    case 8: return "Folder/container";
                    case 7: return "Section";
                    case 6: return "Website";
                    case 5: return "Gallery";
                    case 4: return "Dashboard";
                    case 3: return "Page";
                    case 2: return "Folder";
                    case 1: return "List";
                }
                return null;
            }
        }

        #endregion Properties

        /// <summary>
        /// Selects all by supplying a folder ID
        /// </summary>
        /// <param name="folderID">The Folder ID</param>
        /// <returns></returns>
        public static ISearchView[] SelectAll(int folderID)
        {
            var connector = ConnectorFactory.CreateConnector<SearchView>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.FolderID, folderID);
            filter.AddOrder(x => x.SortOrder);

            return connector.FetchAll(filter).ToArray();
        }

        /// <summary>
        /// Selects all by supplying a folder ID Async
        /// </summary>
        /// <param name="folderID">The Folder ID</param>
        /// <returns></returns>
        public static async Task<ISearchView[]> SelectAllAsync(int folderID)
        {
            var connector = ConnectorFactory.CreateConnector<SearchView>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.FolderID, folderID);
            filter.AddOrder(x => x.SortOrder);

            var result = await connector.FetchAllAsync(filter);
            return result.ToArray();
        }

        /// <summary>
        /// Selects all by supplying a Site ID, filter Type and Search
        /// </summary>
        /// <param name="siteID">The Site ID</param>
        /// <param name="filterType">The filter type</param>
        /// <param name="search">The search value</param>
        /// <returns></returns>
        public static ISearchView[] SelectAll(int? siteID, int? filterType, string search)
        {
            var connector = ConnectorFactory.CreateConnector<SearchView>();
            var filter = connector.CreateDataFilter();
            filter.AddOrder(x => x.SortOrder);

            if (string.IsNullOrEmpty(search) == false)
            {
                search = string.Format("%{0}%", search.Trim().Replace(" ", "%"));
                filter.AddParameter("@search", System.Data.SqlDbType.NVarChar, search);
                filter.AddSql("(SearchView_Title like @search OR SearchView_Description like @search)");
            }

            if (siteID.HasValue)
                filter.Add(x => x.SiteID, siteID.Value);

            if (filterType.HasValue)
                filter.Add(x => x.FolderID, filterType.Value);

            return connector.FetchAll(filter).ToArray();
        }

        /// <summary>
        /// Selects all by supplying a Site ID, filter Type and Search Async
        /// </summary>
        /// <param name="siteID">The Site ID</param>
        /// <param name="filterType">The filter type</param>
        /// <param name="search">The search value</param>
        /// <returns></returns>
        public static async Task<ISearchView[]> SelectAllAsync(int? siteID, int? filterType, string search)
        {
            var connector = ConnectorFactory.CreateConnector<SearchView>();
            var filter = connector.CreateDataFilter();
            filter.AddOrder(x => x.SortOrder);

            if (string.IsNullOrEmpty(search) == false)
            {
                search = string.Format("%{0}%", search.Trim().Replace(" ", "%"));
                filter.AddParameter("@search", System.Data.SqlDbType.NVarChar, search);
                filter.AddSql("(SearchView_Title like @search OR SearchView_Description like @search)");
            }

            if (siteID.HasValue)
                filter.Add(x => x.SiteID, siteID.Value);

            if (filterType.HasValue)
                filter.Add(x => x.TypeID, filterType.Value);

            var result = await connector.FetchAllAsync(filter);
            return result.ToArray();
        }

        /// <summary>
        /// Selects all by supplying an array of identifiers
        /// </summary>
        /// <param name="items">The search item identifiers</param>
        /// <returns></returns>
        public static ISearchView[] SelectAll(string[] items)
        {
            if (items?.Length == 0)
                return null;

            var connector = ConnectorFactory.CreateConnector<SearchView>();
            var filter = connector.CreateDataFilter();
            filter.AddOrder(x => x.SortOrder);
            filter.Add(x => x.ID, items, ComparisonOperator.In);

            return connector.FetchAll(filter).ToArray();
        }

        /// <summary>
        /// Selects all by supplying an array of identifiers Async
        /// </summary>
        /// <param name="items">The search item identifiers</param>
        /// <returns></returns>
        public static async Task<ISearchView[]> SelectAllAsync(string[] items)
        {
            if (items?.Length == 0)
                return null;

            var connector = ConnectorFactory.CreateConnector<SearchView>();
            var filter = connector.CreateDataFilter();
            filter.AddOrder(x => x.SortOrder);
            filter.Add(x => x.ID, items, ComparisonOperator.In);

            var result = await connector.FetchAllAsync(filter);
            return result.ToArray();
        }
    }
}