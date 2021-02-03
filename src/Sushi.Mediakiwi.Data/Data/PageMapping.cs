using Sushi.MicroORM;
using Sushi.MicroORM.Mapping;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sushi.Mediakiwi.Data.MicroORM;

namespace Sushi.Mediakiwi.Data
{
    /// <summary>
    /// Pagemapings are virtual url's that are redirected to a real url or file.
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
                Map(x => x.Created, "PageMap_Created");
                Map(x => x.ListID, "PageMap_List_Key");
                Map(x => x.ItemID, "PageMap_Item_Key");
                Map(x => x.TargetType, "PageMap_TargetType");
                Map(x => x.TypeID, "PageMap_Type");
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
            this.Created = DateTime.UtcNow;
        }

        public virtual bool IsNewInstance
        {
            get { return ID == 0; }
        }

        public virtual int ID { get; set; }

        public virtual bool IsInternalDoc { get; set; }

        public virtual bool IsInternalLink { get; set; }

        public virtual string Path { get; set; }

        /// <summary>
        /// The date on which the mapping is created
        /// </summary>
        public virtual DateTime Created { get; set; }

        public virtual int? ListID { get; set; }

        public virtual int? ItemID { get; set; }

        /// <summary>
        /// PageOrFile tells whether the redirect is to a page or a file.
        /// If PageOrFile=0 use the PageId
        /// If PageOrFile=1 use the AssetId
        /// </summary>
        public virtual int TargetType { get; set; }

        /// <summary>
        /// This specifies which pagetype redirect is used. It influences the HTTP status used
        /// </summary>
        public virtual int TypeID { get; set; }

        /// <summary>
        /// Specifies the asset to redirect if PageOrFile=1
        /// </summary>
        public virtual int AssetID { get; set; }

        /// <summary>
        /// Specifies the page to redirect if PageOrFile=0
        /// </summary>
        public virtual int PageID { get; set; }

        /// <summary>
        /// A specific Query string which is added to the end of the URL which the user is redirect to
        /// </summary>
        public virtual string Query { get; set; }

        /// <summary>
        /// This sets the page title when a rewrite is used. If not specified the page title is used
        /// </summary>
        public virtual string Title { get; set; }

        private Page m_Page;

        public virtual Page Page
        {
            get
            {
                if (m_Page == null)
                    m_Page = Page.SelectOne(PageID);
                return m_Page;
            }
        }

        /// <summary>
        /// Is the Pagemapping is not active the mapping is not used.
        /// </summary>
        public virtual bool IsActive { get; set; }

        private static string[] m_Char_o = new string[] { "ð", "ò", "ó", "ô", "õ", "ö", "ø", "ō", "ŏ", "ő", "œ", "ǒ", "ǫ", "ǭ", "ǿ", "ȍ", "ȏ", "ȫ", "ȭ", "ȯ", "ȱ", "ɻ" };
        private static string[] m_Char_O = new string[] { "Ò", "Ó", "Ô", "Õ", "Ö", "Ø", "Ō", "Ŏ", "Ő", "Œ", "Ǫ", "Ǭ", "Ǿ", "Ȍ", "Ȏ", "Ȫ", "Ȭ", "Ȯ", "Ȱ" };
        private static string[] m_Char_i = new string[] { "ì", "í", "î", "ï", "ĩ", "ī", "ĭ", "į", "ı", "ǐ", "ȉ", "ȋ", "ɂ" };
        private static string[] m_Char_I = new string[] { "Ì", "Í", "Î", "Ï", "Ĩ", "Ī", "Ĭ", "Į", "İ", "Ɩ", "Ɨ", "Ǐ", "Ȉ", "Ȋ" };
        private static string[] m_Char_c = new string[] { "ć", "ĉ", "Ċ", "ċ", "č", "ȼ", "ʏ" };
        private static string[] m_Char_C = new string[] { "Ç", "Ć", "Ĉ", "Ċ", "Č", "Ƈ", "Ȼ", "Đ" };
        private static string[] m_Char_e = new string[] { "è", "é", "ê", "ë", "ē", "ĕ", "ė", "ę", "ě", "ȅ", "ȇ", "ȩ", "ɇ", "ɛ" };
        private static string[] m_Char_E = new string[] { "È", "É", "Ê", "Ë", "Ē", "Ĕ", "Ė", "Ę", "Ě", "Ȅ", "Ȇ", "Ȩ", "Ɇ" };
        private static string[] m_Char_a = new string[] { "à", "á", "â", "ã", "ä", "å", "æ", "ā", "ă", "ą", "ǎ", "ǟ", "ǡ", "ǣ", "ǻ", "ǽ", "ȁ", "ȃ", "ȧ", "Ɉ" };
        private static string[] m_Char_A = new string[] { "À", "Á", "Â", "Ã", "Ä", "Å", "Æ", "Ā", "Ă", "Ą", "Ǟ", "Ǡ", "Ǣ", "Ǻ", "Ǽ", "Ȁ", "Ȃ", "Ɋ" };
        private static string[] m_Char_r = new string[] { "ŕ", "ŗ", "ȑ", "ȓ" };
        private static string[] m_Char_R = new string[] { "Ŕ", "Ŗ", "Ř", "Ȑ", "Ȓ" };
        private static string[] m_Char_s = new string[] { "ś", "ŝ", "ş", "š", "ș" };
        private static string[] m_Char_S = new string[] { "Ś", "Ŝ", "Ş", "Š", "Ș" };
        private static string[] m_Char_z = new string[] { "ź", "ż", "ž", "ɀ" };
        private static string[] m_Char_Z = new string[] { "Ź", "Ż", "Ž", "Ƶ" };
        private static string[] m_Char_n = new string[] { "ñ", "ń", "ņ", "ň", "ŉ", "ŋ", "ƞ", "ǹ" };
        private static string[] m_Char_N = new string[] { "Ń", "Ņ", "Ň", "Ŋ", "Ɲ", "Ǹ" };
        private static string[] m_Char_U = new string[] { "Ù", "Ú", "Û", "Ü", "Ũ", "Ū", "Ŭ", "Ů", "Ű", "Ų", "Ǔ", "Ǖ", "Ǘ", "Ǚ", "Ǜ", "Ȕ", "Ȗ" };
        private static string[] m_Char_u = new string[] { "ù", "ú", "û", "ü", "ũ", "ū", "ŭ", "ů", "ű", "ų", "ư", "ǔ", "ǖ", "ǘ", "ǚ", "ǜ", "ȕ", "ȗ" };

        #endregion Properties

        /// <summary>
        /// Selects a single PageMapping by ListID and ItemID.
        /// </summary>
        /// <param name="listID">The list ID (NOT USED IN THE QUERY).</param>
        /// <param name="itemID">The item ID.</param>
        /// <returns></returns>
        public virtual IPageMapping SelectOne(int? listID, int itemID)
        {
            var connector = ConnectorFactory.CreateConnector<PageMapping>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.ItemID, ItemID);

            return connector.FetchSingle(filter);
        }

        /// <summary>
        /// Selects a single PageMapping by ListID and ItemID Async.
        /// </summary>
        /// <param name="listID">The list ID (NOT USED IN THE QUERY).</param>
        /// <param name="itemID">The item ID.</param>
        /// <returns></returns>
        public virtual async Task<IPageMapping> SelectOneAsync(int? listID, int itemID)
        {
            var connector = ConnectorFactory.CreateConnector<PageMapping>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.ItemID, ItemID);

            return await connector.FetchSingleAsync(filter);
        }

        /// <summary>
        /// Selects a single PageMapping by ID.
        /// </summary>
        /// <param name="ID">The PageMapping ID</param>
        /// <returns></returns>
        public static IPageMapping SelectOne(int ID)
        {
            var connector = ConnectorFactory.CreateConnector<PageMapping>();
            return connector.FetchSingle(ID);
        }

        /// <summary>
        /// Selects a single PageMapping by ID Async.
        /// </summary>
        /// <param name="ID">The PageMapping ID</param>
        /// <returns></returns>
        public static async Task<IPageMapping> SelectOneAsync(int ID)
        {
            var connector = ConnectorFactory.CreateConnector<PageMapping>();
            return await connector.FetchSingleAsync(ID);
        }

        /// <summary>
        /// Selects a single PageMapping by ListID, ItemID and PageID.
        /// </summary>
        /// <param name="listID">The list ID (NOT USED IN THE QUERY).</param>
        /// <param name="itemID">The item ID.</param>
        /// <param name="pageID">The page ID.</param>
        /// <returns></returns>
        public static IPageMapping SelectOne(int? listID, int itemID, int pageID)
        {
            var connector = ConnectorFactory.CreateConnector<PageMapping>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.ItemID, itemID);
            filter.Add(x => x.PageID, pageID);

            return connector.FetchSingle(filter);
        }

        /// <summary>
        /// Selects a single PageMapping by ListID, ItemID and PageID ASYNC.
        /// </summary>
        /// <param name="listID">The list ID (NOT USED IN THE QUERY).</param>
        /// <param name="itemID">The item ID.</param>
        /// <param name="pageID">The page ID.</param>
        /// <returns></returns>
        public static async Task<IPageMapping> SelectOneAsync(int? listID, int itemID, int pageID)
        {
            var connector = ConnectorFactory.CreateConnector<PageMapping>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.ItemID, itemID);
            filter.Add(x => x.PageID, pageID);

            return await connector.FetchSingleAsync(filter);
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
        public static IPageMapping[] SelectAllBasedOnPathPrefix(string prefix)
        {
            if (string.IsNullOrEmpty(prefix))
                return new List<PageMapping>().ToArray();

            prefix += "%";

            var connector = ConnectorFactory.CreateConnector<PageMapping>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.Path, prefix, ComparisonOperator.Like);

            return connector.FetchAll(filter).ToArray();
        }

        /// <summary>
        /// Selects all PageMappings by path prefix Async
        /// </summary>
        /// <param name="prefix">The path prefix</param>
        /// <returns></returns>
        public static async Task<IPageMapping[]> SelectAllBasedOnPathPrefixAsync(string prefix)
        {
            if (string.IsNullOrEmpty(prefix))
                return new List<PageMapping>().ToArray();

            prefix += "%";

            var connector = ConnectorFactory.CreateConnector<PageMapping>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.Path, prefix, ComparisonOperator.Like);

            var result = await connector.FetchAllAsync(filter);
            return result.ToArray();
        }

        /// <summary>
        /// Selects all PageMappings by page ID
        /// </summary>
        /// <param name="pageID">The page ID</param>
        /// <returns></returns>
        public static IPageMapping[] SelectAllBasedOnPageID(int pageID)
        {
            var connector = ConnectorFactory.CreateConnector<PageMapping>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.PageID, pageID);

            return connector.FetchAll(filter).ToArray();
        }

        /// <summary>
        /// Selects all PageMappings by page ID Async
        /// </summary>
        /// <param name="pageID">The page ID</param>
        /// <returns></returns>
        public static async Task<IPageMapping[]> SelectAllBasedOnPageIDAsync(int pageID)
        {
            var connector = ConnectorFactory.CreateConnector<PageMapping>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.PageID, pageID);

            var result = await connector.FetchAllAsync(filter);
            return result.ToArray();
        }

        /// <summary>
        /// Selects all PageMappings
        /// </summary>
        /// <returns></returns>
        public static IPageMapping[] SelectAll()
        {
            var connector = ConnectorFactory.CreateConnector<PageMapping>();
            var filter = connector.CreateDataFilter();
            return connector.FetchAll(filter).ToArray();
        }

        /// <summary>
        /// Selects all PageMappings Async
        /// </summary>
        /// <returns></returns>
        public static async Task<IPageMapping[]> SelectAllAsync()
        {
            var connector = ConnectorFactory.CreateConnector<PageMapping>();
            var filter = connector.CreateDataFilter();

            var result = await connector.FetchAllAsync(filter);
            return result.ToArray();
        }

        /// <summary>
        /// Selects all PageMappings based on Type ID and OnlyActive
        /// </summary>
        /// <param name="typeId">The TypeID</param>
        /// <param name="onlyActive">ONly return active mappings ?</param>
        /// <returns></returns>
        public static IPageMapping[] SelectAllNonList(int typeId, bool onlyActive)
        {
            var connector = ConnectorFactory.CreateConnector<PageMapping>();
            var filter = connector.CreateDataFilter();

            if (typeId > 0)
            {
                filter.Add(x => x.TypeID, typeId);
                filter.Add(x => x.TargetType, 0);
            }
            // Special case for File redirects
            else if (typeId == -2)
            {
                filter.Add(x => x.TargetType, 1);
            }
            if (onlyActive)
                filter.Add(x => x.IsActive, true);

            return connector.FetchAll(filter).ToArray();
        }

        /// <summary>
        /// Selects all PageMappings based on Type ID and OnlyActive
        /// </summary>
        /// <param name="typeId">The TypeID</param>
        /// <param name="onlyActive">ONly return active mappings ?</param>
        /// <returns></returns>
        public static async Task<IPageMapping[]> SelectAllNonListAsync(int typeId, bool onlyActive)
        {
            var connector = ConnectorFactory.CreateConnector<PageMapping>();
            var filter = connector.CreateDataFilter();

            if (typeId > 0)
            {
                filter.Add(x => x.TypeID, typeId);
                filter.Add(x => x.TargetType, 0);
            }
            // Special case for File redirects
            else if (typeId == -2)
            {
                filter.Add(x => x.TargetType, 1);
            }
            if (onlyActive)
                filter.Add(x => x.IsActive, true);

            var result = await connector.FetchAllAsync(filter);
            return result.ToArray();
        }

        public virtual bool Save()
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

        public virtual async Task<bool> SaveAsync()
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

        public virtual void Delete()
        {
            var connector = ConnectorFactory.CreateConnector<PageMapping>();
            connector.Delete(this);
        }

        public virtual async Task DeleteAsync()
        {
            var connector = ConnectorFactory.CreateConnector<PageMapping>();
            await connector.DeleteAsync(this);
        }
    }
}