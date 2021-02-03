using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Sushi.Mediakiwi.Data.DalReflection;

namespace Sushi.Mediakiwi.Data
{
    /// <summary>
    /// This entity gives the option to search within Wim-component content.
    /// </summary>
    [DatabaseTable("wim_ComponentSearch")]
    public class ComponentSearch : DatabaseEntity
    {
        #region Properties
        int m_ID;
        /// <summary>
        /// Unique identifier of this component
        /// </summary>
        /// <value>The ID.</value>
        [DatabaseColumn("ComponentSearch_Key", SqlDbType.Int, IsPrimaryKey = true)]
        public int ID
        {
            get { return m_ID; }
            set { m_ID = value; }
        }

        Int16 m_Type;
        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        [DatabaseColumn("ComponentSearch_Type", SqlDbType.SmallInt)]
        public Int16 Type
        {
            get { return m_Type; }
            set { m_Type = value; }
        }

        int m_ReferenceID;
        /// <summary>
        /// Gets or sets the reference ID.
        /// </summary>
        /// <value>The reference ID.</value>
        [DatabaseColumn("ComponentSearch_Ref_Key", SqlDbType.Int)]
        public int ReferenceID
        {
            get { return m_ReferenceID; }
            set { m_ReferenceID = value; }
        }

        string m_Text;
        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
        [DatabaseColumn("ComponentSearch_Text", SqlDbType.NText, Length = 4000, IsNullable = true)]
        public string Text
        {
            get { return m_Text; }
            set { m_Text = value; }
        }

        int m_SiteID;
        [DatabaseColumn("ComponentSearch_Site_Key", SqlDbType.Int, IsNullable = true)]
        public int SiteID
        {
            get { return m_SiteID; }
            set { m_SiteID = value; }
        }
        #endregion

        /// <summary>
        /// Adds the one.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="referenceKey">The reference key.</param>
        /// <param name="content">The content.</param>
        /// <param name="site">The site.</param>
        public static void AddOne(short type, int referenceKey, string content, int site)
        {
            ComponentSearch implement = ComponentSearch.SelectOne(type, referenceKey);
            implement.Type = type;
            implement.ReferenceID = referenceKey;
            implement.Text = Wim.Utility.CleanFormatting(content);
            implement.SiteID = site;
            implement.Save();
        }

        /// <summary>
        /// Selects the one.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="referenceKey">The reference key.</param>
        /// <returns></returns>
        public static ComponentSearch SelectOne(short type, int referenceKey)
        {
            List<DatabaseDataValueColumn> list = new List<DatabaseDataValueColumn>();
            list.Add(new DatabaseDataValueColumn("ComponentSearch_Type", SqlDbType.SmallInt, type));
            list.Add(new DatabaseDataValueColumn("ComponentSearch_Ref_Key", SqlDbType.Int, referenceKey));

            return (ComponentSearch)new ComponentSearch()._SelectOne(list);
        }

        /// <summary>
        /// Selects all.
        /// </summary>
        /// <param name="searchParameter">The search parameter.</param>
        /// <param name="searchConfiguration">The search configuration.</param>
        /// <returns></returns>
        public static Sushi.Mediakiwi.Data.SearchResult[] SelectAll(string searchParameter, ref SearchConfiguration searchConfiguration)
        {
            if (searchConfiguration == null) return null;

            //System.Web.HttpContext.Current.Response.Write("<br>" + searchParameter);

            searchConfiguration.SetCacheKey(searchParameter);
            using (Wim.Utilities.CacheItemManager cman = new Wim.Utilities.CacheItemManager())
            {
                Wim.Utilities.Splitlist splitlist = null;
                //if (cman.IsCached(searchConfiguration.CacheKey))
                //    splitlist = (Wim.Utilities.Splitlist)cman.GetItem(searchConfiguration.CacheKey).Value;
                //else
                //{
                    SearchResult[] productKeylist = SelectAll2(searchParameter, ref searchConfiguration);

                    splitlist = new Wim.Utilities.Splitlist(productKeylist, searchConfiguration.MaxResultSize, searchConfiguration.MaxPageSize);

                //    cman.Add(searchConfiguration.CacheKey, splitlist, searchConfiguration.CacheExpiration);
                //}

                return GetSplitListItem(splitlist, ref searchConfiguration);
            }
        }

        /// <summary>
        /// Gets the split list item.
        /// </summary>
        /// <param name="splitlist">The splitlist.</param>
        /// <param name="settings">The settings.</param>
        /// <returns></returns>
        static Sushi.Mediakiwi.Data.SearchResult[] GetSplitListItem(Wim.Utilities.Splitlist splitlist, ref SearchConfiguration settings)
        {
            if (splitlist == null || splitlist.ItemCount == 0)
                return new Sushi.Mediakiwi.Data.SearchResult[0];

            settings.m_Pages = splitlist.ListCount;
            settings.m_Elements = splitlist.ItemCount;

            int index = settings.CurrentPage - 1;
            if (index > (settings.m_Pages - 1)) index = (settings.m_Pages - 1);

            List<object> keylist = (List<object>)splitlist[index];
            List<int> components = null;

            Dictionary<string, Sushi.Mediakiwi.Data.SearchResult> searchResult = new Dictionary<string, Sushi.Mediakiwi.Data.SearchResult>();

            foreach (object key in keylist)
            {
                SearchResult result = key as SearchResult;
                if (components == null) components = new List<int>();
                components.Add(result.Id);
                
                string searchKey = string.Format("{0}.{1}", result.Type, result.Id);

                //System.Web.HttpContext.Current.Response.Write("<br>" + result.Id);
                if (!searchResult.ContainsKey(searchKey))
                    searchResult.Add(searchKey, result);
                //else
                //    System.Web.HttpContext.Current.Response.Write("!!!");


            }
            //return new SearchResult[0];
            SearchResult[] results = SelectAll_BasedOnList(searchResult, components);

            //System.Web.HttpContext.Current.Response.Write("<br/> ==> " + settings.CacheKey + " - " + results.Length.ToString());

            return results;
        }

        /// <summary>
        /// Selects the all_ based on list.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="components">The components.</param>
        /// <returns></returns>
        static Sushi.Mediakiwi.Data.SearchResult[] SelectAll_BasedOnList(Dictionary<string, Sushi.Mediakiwi.Data.SearchResult> result, List<int> components)
        {
            string sqlText = "";
            if (components != null)
            {
                sqlText += string.Format(@"
    select 
        CAST(ComponentSearch_Type AS VARCHAR(4)) + '.' + CAST(ComponentSearch_Key as VARCHAR(10)) as [KEY]
    ,   Page_Key [ID]
    ,   Page_LinkText [TITLE]
    ,   ISNULL(CAST(Page_Description AS NVARCHAR(4000)), ComponentSearch_Text) [TEXT]
    ,   Page_Publish [DATE]
    from 
        wim_ComponentSearch 
        join wim_Components on ComponentSearch_Ref_Key = component_Key
        join wim_Pages on Component_Page_Key = Page_Key
        join wim_Folders on Page_Folder_Key = Folder_Key
    where
		    ComponentSearch_Key in ({0})
",
                    Wim.Utility.ConvertToCsvString(components.ToArray()));
            }
            if (string.IsNullOrEmpty(sqlText)) return null;

            using (Connection.SqlCommander dac = new Connection.SqlCommander(Sushi.Mediakiwi.Data.Common.DatabaseConnectionString))
            {
                dac.SqlText = sqlText;
                SqlDataReader reader = dac.ExecReader;
                while (reader.Read())
                {
                    Sushi.Mediakiwi.Data.SearchResult sr = result[reader.GetString(0)] as Sushi.Mediakiwi.Data.SearchResult;
                    sr.Id = (reader.GetInt32(1));
                    sr.LinkText = (reader.GetString(2));
                    sr.Description = Wim.Utility.CleanFormatting(reader.GetString(3));
                    sr.Published = GetReaderNullableDateTime(reader[4]);
                }
            }
            Sushi.Mediakiwi.Data.SearchResult[] resultArr = new Sushi.Mediakiwi.Data.SearchResult[result.Count];
            result.Values.CopyTo(resultArr, 0);
            if (resultArr == null)
                resultArr = new SearchResult[0];

            return resultArr;
        }

        static System.Nullable<DateTime> GetReaderNullableDateTime(object readerValue)
        {
            if (readerValue == DBNull.Value)
                return null;
            return (DateTime)readerValue;
        }

        /// <summary>
        /// Gets the correct search query.
        /// </summary>
        /// <param name="searchParameter">The search parameter.</param>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        static string GetCorrectSearchQuery(string searchParameter, Sushi.Mediakiwi.Data.SearchType type)
        {
            if (string.IsNullOrEmpty(searchParameter)) return null;
            string searchTextQuery = string.Empty;

            if (type == SearchType.Exact_Phrase)
                return searchTextQuery += string.Format(" \"{0}\" ", searchParameter);

            string[] candidates = searchParameter.Split(' ');
            foreach (string candidate in candidates)
            {
                if (searchTextQuery.Length > 0)
                {
                    if (type == SearchType.All_Word || type == SearchType.All_Word_And_Partial)
                        searchTextQuery += " AND ";
                    else
                        searchTextQuery += " OR ";
                }

                if (type == SearchType.All_Word || type == SearchType.Any_Word) searchTextQuery += string.Format(" \"{0}\" ", candidate);
                if (type == SearchType.All_Word_And_Partial || type == SearchType.Any_Word_And_Partial) searchTextQuery += string.Format(" \"*{0}*\" ", candidate);
            }
            return searchTextQuery;
        }

        /// <summary>
        /// Selects the all2.
        /// </summary>
        /// <param name="searchParameter">The search parameter.</param>
        /// <param name="searchConfiguration">The search configuration.</param>
        /// <returns></returns>
        static SearchResult[] SelectAll2(string searchParameter, ref SearchConfiguration searchConfiguration)
        {
            if (string.IsNullOrEmpty(searchParameter)) return null;
            string[] candidates = searchParameter.Split(' ');
            string searchTextQuery = GetCorrectSearchQuery(searchParameter, searchConfiguration.SearchType);

            //List<SearchResult> result = new List<SearchResult>();

            string sqlText = "";

            if (!string.IsNullOrEmpty(sqlText))
                sqlText += " union ";

            #region SearchArea.Page
            sqlText += string.Format(@"select {1}, ComponentSearch_Key as ReferenceKey, Ftt.RANK from wim_ComponentSearch inner join CONTAINSTABLE(wim_ComponentSearch, *, '{0}') AS Ftt ON ComponentSearch_Key = Ftt.[KEY] {2}", searchTextQuery, 1 //Page
                            , string.Format("where ComponentSearch_Site_Key = {0}", searchConfiguration.Site)
                );
            #endregion

            if (!string.IsNullOrEmpty(sqlText)) sqlText += " order by RANK DESC";

            SortedList<int, SearchResult> tmp = new SortedList<int, SearchResult>();

            using (Connection.SqlCommander dac = new Connection.SqlCommander(Sushi.Mediakiwi.Data.Common.DatabaseConnectionString))
            {
                dac.SqlText = sqlText;
                SqlDataReader reader = dac.ExecReader;
                while (reader.Read())
                {
                    int key = reader.GetInt32(2);
                    if (!tmp.ContainsKey(key))
                        tmp.Add(key, new SearchResult(1, reader.GetInt32(1), reader.GetInt32(2)));
                }
            }

            List<SearchResult> result = new List<SearchResult>();
            foreach (KeyValuePair<int, SearchResult> item in tmp)
                result.Add(item.Value);

            return result.ToArray();
        }
    }
}
