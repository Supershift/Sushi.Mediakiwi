using Sushi.Mediakiwi.Data.MicroORM;
using Sushi.MicroORM;
using Sushi.MicroORM.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Data
{
    /// <summary>
    /// Pagemapings are  url's that are redirected to a real url or file.
    /// </summary>
    [DataMap(typeof(PageMappingMap))]
    public class PageMapping : IPageMapping
    {

        public class PageMappingMap : DataMap<PageMapping>
        {
            public PageMappingMap()
            {
                Table("wim_PageMappings");
                Id(x => x.ID, "PageMap_Key").Identity();
                Map(x => x.Path, "PageMap_Path").Length(150);
                Map(x => x.Expression, "PageMap_Expression").Length(200);
                Map(x => x.Created, "PageMap_Created");
                Map(x => x.TargetTypeID, "PageMap_TargetType");
                Map(x => x.MappingTypeID, "PageMap_Type");
                Map(x => x.AssetID, "PageMap_Asset_Key");
                Map(x => x.PageID, "PageMap_Page_Key");
                Map(x => x.Query, "PageMap_Query").Length(50);
                Map(x => x.Title, "PageMap_Title").Length(200);
                Map(x => x.IsActive, "PageMap_IsActive");
            }
        }

        #region Properties

        public PageMapping()
        {
            Created = DateTime.UtcNow;
        }

        /// <summary>
        /// The Unique Pagemap identifier
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// The Expression for this pagemap
        /// </summary>
        public string Expression { get; set; }

        /// <summary>
        /// The Path (URL) to act to
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// The date on which the mapping is created
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// What kind of target is this pagemap for
        /// </summary>
        public int TargetTypeID { get; set; }

        /// <summary>
        /// What kind of target is this pagemap for
        /// </summary>
        public PageMappingTargetType TargetType
        {
            get { return (PageMappingTargetType)TargetTypeID; }
            set { TargetTypeID = (int)value; }
        }

        /// <summary>
        /// This specifies which pagetype redirect is used. It influences the HTTP status used
        /// </summary>
        public PageMappingType MappingType
        {
            get { return (PageMappingType)MappingTypeID; }
            set { MappingTypeID = (int)value; }
        }

        /// <summary>
        /// This specifies which pagetype redirect is used. It influences the HTTP status used
        /// </summary>
        public int MappingTypeID { get; set; }

        /// <summary>
        /// Specifies the asset to redirect if PageOrFile=1
        /// </summary>
        public int AssetID { get; set; }

        /// <summary>
        /// Specifies the page to redirect if PageOrFile=0
        /// </summary>
        public int PageID { get; set; }

        /// <summary>
        /// A specific Query string which is added to the end of the URL which the user is redirect to
        /// </summary>
        public string Query { get; set; }

        /// <summary>
        /// This sets the page title when a rewrite is used. If not specified the page title is used
        /// </summary>
        public string Title { get; set; }

        private Page m_Page;
        /// <summary>
        /// The page this pagemap is linked to
        /// </summary>
        public Page Page
        {
            get
            {
                if (m_Page == null)
                {
                    m_Page = Page.SelectOne(PageID);
                }
                return m_Page;
            }
        }

        /// <summary>
        /// The complete target URL (page URL + query if any)
        /// </summary>
        public string TargetURL
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Query) == false)
                {
                    return string.Concat(PageName, Query);
                }

                return PageName;
            }
        }

        /// <summary>
        /// The Page name
        /// </summary>
        public string PageName
        {
            get
            {
                if (Page?.ID > 0)
                {
                    return Page.CompletePath;
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        /// <summary>
        /// Is the Pagemapping is not active the mapping is not used.
        /// </summary>
        public bool IsActive { get; set; }

        #endregion Properties

        /// <summary>
        /// Selects a single PageMapping by ListID and ItemID.
        /// </summary>
        /// <param name="listID">The list ID (NOT USED IN THE QUERY).</param>
        /// <param name="itemID">The item ID.</param>
        /// <returns></returns>
        public static IPageMapping SelectOne(int key)
        {
            var connector = ConnectorFactory.CreateConnector<PageMapping>();
            return connector.FetchSingle(key);
        }

        /// <summary>
        /// Selects a single PageMapping by ListID and ItemID Async.
        /// </summary>
        /// <param name="listID">The list ID (NOT USED IN THE QUERY).</param>
        /// <param name="itemID">The item ID.</param>
        /// <returns></returns>
        public static async Task<IPageMapping> SelectOneAsync(int key)
        {
            var connector = ConnectorFactory.CreateConnector<PageMapping>();
            return await connector.FetchSingleAsync(key);
        }

        /// <summary>
        /// Selects a single PageMapping by and PageID and Query.
        /// </summary>
        /// <param name="pageID">The page ID</param>
        /// <param name="query">The query</param>
        /// <returns></returns>
        public static IPageMapping SelectOneByPageAndQuery(int pageID, string query)
        {
            var connector = ConnectorFactory.CreateConnector<PageMapping>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.Query, query);
            filter.Add(x => x.PageID, pageID);

            return connector.FetchSingle(filter);
        }

        /// <summary>
        /// Selects a single PageMapping by and PageID and Query Async.
        /// </summary>
        /// <param name="pageID">The page ID</param>
        /// <param name="query">The query</param>
        /// <returns></returns>
        public static async Task<IPageMapping> SelectOneByPageAndQueryAsync(int pageID, string query)
        {
            var connector = ConnectorFactory.CreateConnector<PageMapping>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.Query, query);
            filter.Add(x => x.PageID, pageID);

            return await connector.FetchSingleAsync(filter);
        }

        /// <summary>
        /// Selects all PageMappings by path prefix
        /// </summary>
        /// <param name="prefix">The path prefix</param>
        /// <returns></returns>
        public static List<IPageMapping> SelectAllBasedOnPathPrefix(string prefix)
        {
            if (string.IsNullOrEmpty(prefix))
                return new List<IPageMapping>();

            prefix += "%";

            var connector = ConnectorFactory.CreateConnector<PageMapping>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.Path, prefix, ComparisonOperator.Like);

            var result = connector.FetchAll(filter);
            return result.ToList<IPageMapping>();
        }

        /// <summary>
        /// Selects all PageMappings by path prefix Async
        /// </summary>
        /// <param name="prefix">The path prefix</param>
        /// <returns></returns>
        public static async Task<List<IPageMapping>> SelectAllBasedOnPathPrefixAsync(string prefix)
        {
            if (string.IsNullOrEmpty(prefix))
                return new List<IPageMapping>();

            prefix += "%";

            var connector = ConnectorFactory.CreateConnector<PageMapping>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.Path, prefix, ComparisonOperator.Like);

            var result = await connector.FetchAllAsync(filter);
            return result.ToList<IPageMapping>();
        }

        /// <summary>
        /// Selects all PageMappings by page ID
        /// </summary>
        /// <param name="pageID">The page ID</param>
        /// <returns></returns>
        public static List<IPageMapping> SelectAllBasedOnPageID(int pageID)
        {
            var connector = ConnectorFactory.CreateConnector<PageMapping>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.PageID, pageID);

            var result = connector.FetchAll(filter);
            return result.ToList<IPageMapping>();
        }

        /// <summary>
        /// Selects all PageMappings by page ID Async
        /// </summary>
        /// <param name="pageID">The page ID</param>
        /// <returns></returns>
        public static async Task<List<IPageMapping>> SelectAllBasedOnPageIDAsync(int pageID)
        {
            var connector = ConnectorFactory.CreateConnector<PageMapping>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.PageID, pageID);

            var result = await connector.FetchAllAsync(filter);
            return result.ToList<IPageMapping>();
        }

        /// <summary>
        /// Selects all PageMappings
        /// </summary>
        /// <returns></returns>
        public static List<IPageMapping> SelectAll()
        {
            var connector = ConnectorFactory.CreateConnector<PageMapping>();
            var filter = connector.CreateDataFilter();

            var result = connector.FetchAll(filter);
            return result.ToList<IPageMapping>();
        }

        /// <summary>
        /// Selects all PageMappings Async
        /// </summary>
        /// <returns></returns>
        public static async Task<List<IPageMapping>> SelectAllAsync()
        {
            var connector = ConnectorFactory.CreateConnector<PageMapping>();
            var filter = connector.CreateDataFilter();

            var result = await connector.FetchAllAsync(filter);
            return result.ToList<IPageMapping>();
        }

        /// <summary>
        /// Selects all PageMappings based on Type ID and OnlyActive
        /// </summary>
        /// <param name="typeId">The TypeID</param>
        /// <param name="onlyActive">ONly return active mappings ?</param>
        /// <returns></returns>
        public static List<IPageMapping> SelectAll(int typeId, bool onlyActive)
        {
            var connector = ConnectorFactory.CreateConnector<PageMapping>();
            var filter = connector.CreateDataFilter();

            if (typeId > 0)
            {
                filter.Add(x => x.MappingTypeID, typeId);
                filter.Add(x => x.TargetTypeID, (int)PageMappingTargetType.PAGE);
            }
            // Special case for File redirects
            else if (typeId == -2)
            {
                filter.Add(x => x.TargetTypeID, (int)PageMappingTargetType.FILE);
            }

            if (onlyActive)
            {
                filter.Add(x => x.IsActive, true);
            }

            var result = connector.FetchAll(filter);
            return result.ToList<IPageMapping>();
        }

        /// <summary>
        /// Selects all PageMappings based on Type ID and OnlyActive
        /// </summary>
        /// <param name="typeId">The TypeID</param>
        /// <param name="onlyActive">ONly return active mappings ?</param>
        /// <returns></returns>
        public static async Task<List<IPageMapping>> SelectAllAsync(int typeId, bool onlyActive)
        {
            var connector = ConnectorFactory.CreateConnector<PageMapping>();
            var filter = connector.CreateDataFilter();

            if (typeId > 0)
            {
                filter.Add(x => x.MappingTypeID, typeId);
                filter.Add(x => x.TargetTypeID, (int)PageMappingTargetType.PAGE);
            }
            // Special case for File redirects
            else if (typeId == -2)
            {
                filter.Add(x => x.TargetTypeID, (int)PageMappingTargetType.FILE);
            }
            if (onlyActive)
            {
                filter.Add(x => x.IsActive, true);
            }

            var result = await connector.FetchAllAsync(filter);
            return result.ToList<IPageMapping>();
        }

        /// <summary>
        /// Saves the Pagemap
        /// </summary>
        /// <returns></returns>
        public bool Save()
        {
            var connector = ConnectorFactory.CreateConnector<PageMapping>();
            try
            {
                connector.Save(this);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Async saves the Pagemap
        /// </summary>
        /// <returns></returns>
        public async Task<bool> SaveAsync()
        {
            var connector = ConnectorFactory.CreateConnector<PageMapping>();
            try
            {
                await connector.SaveAsync(this);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Deletes the Pagemap
        /// </summary>
        public void Delete()
        {
            var connector = ConnectorFactory.CreateConnector<PageMapping>();
            connector.Delete(this);
        }

        /// <summary>
        /// Async deletes the Pagemap
        /// </summary>
        /// <returns></returns>
        public async Task DeleteAsync()
        {
            var connector = ConnectorFactory.CreateConnector<PageMapping>();
            await connector.DeleteAsync(this);
        }
    }
}