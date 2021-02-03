using System;
using System.Data;
using System.Data.Linq.Mapping;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Sushi.Mediakiwi.Data.DalReflection;
using Sushi.Mediakiwi.Framework;
using System.Web.UI.WebControls;
using System.Web;

namespace Sushi.Mediakiwi.Data.Parsers
{
    /// <summary>
    /// Pagemapings are virtual url's that are redirected to a real url or file.
    /// </summary>
    public partial class PageMappingParser : IPageMappingParser
    {
        static ISqlEntityParser _DataParser;
        static ISqlEntityParser DataParser
        {
            get
            {
                if (_DataParser == null)
                    _DataParser = Environment.GetInstance<ISqlEntityParser>();
                return _DataParser;
            }
        }

        /// <summary>
        /// Registers the URL and return the relative path.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="query">The query.</param>
        /// <param name="pageTitle">The page title.</param>
        /// <param name="pageID">The page ID.</param>
        /// <param name="applyApplicationPath">if set to <c>true</c> [apply application path].</param>
        /// <param name="listID">The list ID.</param>
        /// <param name="itemID">The item ID.</param>
        /// <returns></returns>
        public virtual IPageMapping RegisterUrl(string url, string query, string pageTitle, int pageID, bool applyApplicationPath, int? listID, int? itemID)
        {
            var map = SelectOne(url);
            if (map.IsNewInstance)
            {
                map.Path = ConvertUrl(url);
                map.Query = query;
                map.PageID = pageID;
                map.ListID = listID;
                map.ItemID = itemID;
                map.Title = pageTitle;
                map.Save();
            }
            else
            {
                if (map.PageID != pageID || map.ListID != listID || map.ItemID != itemID || map.Title != pageTitle)
                {
                    map.Query = query;
                    map.PageID = pageID;
                    map.ListID = listID;
                    map.ItemID = itemID;
                    map.Title = pageTitle;
                    map.Save();
                }
            }
            return map;
        }

        static string[] m_Char_o = new string[] { "ð", "ò", "ó", "ô", "õ", "ö", "ø", "ō", "ŏ", "ő", "œ", "ǒ", "ǫ", "ǭ", "ǿ", "ȍ", "ȏ", "ȫ", "ȭ", "ȯ", "ȱ", "ɻ" };
        static string[] m_Char_O = new string[] { "Ò", "Ó", "Ô", "Õ", "Ö", "Ø", "Ō", "Ŏ", "Ő", "Œ", "Ǫ", "Ǭ", "Ǿ", "Ȍ", "Ȏ", "Ȫ", "Ȭ", "Ȯ", "Ȱ" };
        static string[] m_Char_i = new string[] { "ì", "í", "î", "ï", "ĩ", "ī", "ĭ", "į", "ı", "ǐ", "ȉ", "ȋ", "ɂ" };
        static string[] m_Char_I = new string[] { "Ì", "Í", "Î", "Ï", "Ĩ", "Ī", "Ĭ", "Į", "İ", "Ɩ", "Ɨ", "Ǐ", "Ȉ", "Ȋ" };
        static string[] m_Char_c = new string[] { "ć", "ĉ", "Ċ", "ċ", "č", "ȼ", "ʏ" };
        static string[] m_Char_C = new string[] { "Ç", "Ć", "Ĉ", "Ċ", "Č", "Ƈ", "Ȼ", "Đ" };
        static string[] m_Char_e = new string[] { "è", "é", "ê", "ë", "ē", "ĕ", "ė", "ę", "ě", "ȅ", "ȇ", "ȩ", "ɇ", "ɛ" };
        static string[] m_Char_E = new string[] { "È", "É", "Ê", "Ë", "Ē", "Ĕ", "Ė", "Ę", "Ě", "Ȅ", "Ȇ", "Ȩ", "Ɇ" };
        static string[] m_Char_a = new string[] { "à", "á", "â", "ã", "ä", "å", "æ", "ā", "ă", "ą", "ǎ", "ǟ", "ǡ", "ǣ", "ǻ", "ǽ", "ȁ", "ȃ", "ȧ", "Ɉ" };
        static string[] m_Char_A = new string[] { "À", "Á", "Â", "Ã", "Ä", "Å", "Æ", "Ā", "Ă", "Ą", "Ǟ", "Ǡ", "Ǣ", "Ǻ", "Ǽ", "Ȁ", "Ȃ", "Ɋ" };
        static string[] m_Char_r = new string[] { "ŕ", "ŗ", "ȑ", "ȓ" };
        static string[] m_Char_R = new string[] { "Ŕ", "Ŗ", "Ř", "Ȑ", "Ȓ" };
        static string[] m_Char_s = new string[] { "ś", "ŝ", "ş", "š", "ș" };
        static string[] m_Char_S = new string[] { "Ś", "Ŝ", "Ş", "Š", "Ș" };
        static string[] m_Char_z = new string[] { "ź", "ż", "ž", "ɀ" };
        static string[] m_Char_Z = new string[] { "Ź", "Ż", "Ž", "Ƶ" };
        static string[] m_Char_n = new string[] { "ñ", "ń", "ņ", "ň", "ŉ", "ŋ", "ƞ", "ǹ" };
        static string[] m_Char_N = new string[] { "Ń", "Ņ", "Ň", "Ŋ", "Ɲ", "Ǹ" };
        static string[] m_Char_U = new string[] { "Ù", "Ú", "Û", "Ü", "Ũ", "Ū", "Ŭ", "Ů", "Ű", "Ų", "Ǔ", "Ǖ", "Ǘ", "Ǚ", "Ǜ", "Ȕ", "Ȗ" };
        static string[] m_Char_u = new string[] { "ù", "ú", "û", "ü", "ũ", "ū", "ŭ", "ů", "ű", "ų", "ư", "ǔ", "ǖ", "ǘ", "ǚ", "ǜ", "ȕ", "ȗ" };

        public virtual string ConvertUrl(string url)
        {
            string replacement = Data.Environment.Current["SPACE_REPLACEMENT"];
            string page_wildcard_extention = Data.Environment.Current["PAGE_WILDCARD_EXTENTION"];
          
            
            //  Second space replace is needed, do not remove!
            url = url.Replace("&", Wim.CommonConfiguration.PAGEMAP_AND_REPLACEMENT);
            url = url.Replace(" ", replacement).Replace(" ", replacement);
            //CB; Change voor Gametron.. verwijder alle punten in de url met de replacement
            //    Edoch behoud de extentie voor privilege
            url = url.Replace(".", replacement);
            if (!String.IsNullOrEmpty(page_wildcard_extention) && page_wildcard_extention != ".") 
            {
                Regex endsWithBadExt = new Regex(replacement+page_wildcard_extention + "$");
                if (endsWithBadExt.IsMatch(url))
                    url = endsWithBadExt.Replace(url, "." + page_wildcard_extention);
            }
             
            foreach (var i in m_Char_o) url = url.Replace(i, "o");
            foreach (var i in m_Char_O) url = url.Replace(i, "O");
            foreach (var i in m_Char_i) url = url.Replace(i, "i");
            foreach (var i in m_Char_I) url = url.Replace(i, "I");
            foreach (var i in m_Char_c) url = url.Replace(i, "c");
            foreach (var i in m_Char_C) url = url.Replace(i, "C");
            foreach (var i in m_Char_e) url = url.Replace(i, "e");
            foreach (var i in m_Char_E) url = url.Replace(i, "E");
            foreach (var i in m_Char_a) url = url.Replace(i, "a");
            foreach (var i in m_Char_A) url = url.Replace(i, "A");
            foreach (var i in m_Char_s) url = url.Replace(i, "s");
            foreach (var i in m_Char_S) url = url.Replace(i, "S");
            foreach (var i in m_Char_z) url = url.Replace(i, "z");
            foreach (var i in m_Char_Z) url = url.Replace(i, "Z");
            foreach (var i in m_Char_n) url = url.Replace(i, "n");
            foreach (var i in m_Char_N) url = url.Replace(i, "N");
            foreach (var i in m_Char_U) url = url.Replace(i, "U");
            foreach (var i in m_Char_u) url = url.Replace(i, "u");
            foreach (var i in m_Char_R) url = url.Replace(i, "R");
            foreach (var i in m_Char_r) url = url.Replace(i, "r");

            Regex rex = new Regex(Wim.Utility.GlobalRegularExpression.ReplaceNotAcceptableFilenameCharacter);
            return rex.Replace(url, string.Empty);
        }

        /// <summary>
        /// Selects the one.
        /// </summary>
        /// <param name="relativePath">The relative path.</param>
        /// <returns></returns>
        public virtual IPageMapping SelectOne(string relativePath)
        {
            string path = Wim.Utility.RemApplicationPath(ConvertUrl(System.Web.HttpUtility.UrlDecode(relativePath)));

            List<DatabaseDataValueColumn> whereClause = new List<DatabaseDataValueColumn>();
            whereClause.Add(new DatabaseDataValueColumn("PageMap_Path", SqlDbType.VarChar, path));

            return DataParser.SelectOne<IPageMapping>(whereClause);
        }

        /// <summary>
        /// Selects the one.
        /// </summary>
        /// <param name="listID">The list ID.</param>
        /// <param name="itemID">The item ID.</param>
        /// <returns></returns>
        public virtual IPageMapping SelectOne(int? listID, int itemID)
        {
            List<DatabaseDataValueColumn> whereClause = new List<DatabaseDataValueColumn>();
            whereClause.Add(new DatabaseDataValueColumn("PageMap_Item_Key", SqlDbType.Int, itemID));

            return DataParser.SelectOne<IPageMapping>(whereClause);
        }

        public virtual IPageMapping SelectOne(int ID)
        {
            return DataParser.SelectOne<IPageMapping>(ID, true);
        }

        public virtual IPageMapping SelectOne(int? listID, int itemID, int pageID)
        {
            List<DatabaseDataValueColumn> whereClause = new List<DatabaseDataValueColumn>();
            whereClause.Add(new DatabaseDataValueColumn("PageMap_Item_Key", SqlDbType.Int, itemID));
            whereClause.Add(new DatabaseDataValueColumn("PageMap_Page_Key", SqlDbType.Int, pageID));

            return DataParser.SelectOne<IPageMapping>(whereClause);
        }

        public virtual IPageMapping SelectOneByPageAndQuery(int pageID, string query)
        {
            string q = query.ToLower();

            List<DatabaseDataValueColumn> whereClause = new List<DatabaseDataValueColumn>();
            whereClause.Add(new DatabaseDataValueColumn("PageMap_Query", SqlDbType.NVarChar, q));
            whereClause.Add(new DatabaseDataValueColumn("PageMap_Page_Key", SqlDbType.Int, pageID));

            return DataParser.SelectOne<IPageMapping>(whereClause);
        }

        public virtual IPageMapping[] SelectAllBasedOnPathPrefix(string prefix)
        {
            if (string.IsNullOrEmpty(prefix))
                return new PageMapping[0];

            prefix += "%";
            List<DatabaseDataValueColumn> whereClause = new List<DatabaseDataValueColumn>();
            whereClause.Add(new DatabaseDataValueColumn("PageMap_Path", SqlDbType.NVarChar, prefix, DatabaseDataValueCompareType.Like));

            return DataParser.SelectAll<IPageMapping>(whereClause).ToArray();
        }

        public virtual IPageMapping[] SelectAllBasedOnPageID(int pageID)
        {
            List<DatabaseDataValueColumn> whereClause = new List<DatabaseDataValueColumn>();
            whereClause.Add(new DatabaseDataValueColumn("PageMap_Page_Key", SqlDbType.Int, pageID));

            return DataParser.SelectAll<IPageMapping>(whereClause).ToArray();
        }

        public virtual IPageMapping[] SelectAllBasedOnPathPrefix(string prefix, int pageID)
        {
            if (string.IsNullOrEmpty(prefix))
                return new PageMapping[0];

            string candidate = ConvertUrl(prefix) + '%';

            List<DatabaseDataValueColumn> whereClause = new List<DatabaseDataValueColumn>();
            whereClause.Add(new DatabaseDataValueColumn("PageMap_Page_Key", SqlDbType.Int, pageID));
            whereClause.Add(new DatabaseDataValueColumn("PageMap_Path", SqlDbType.NVarChar, candidate, DatabaseDataValueCompareType.Like));

            return DataParser.SelectAll<IPageMapping>(whereClause).ToArray();
        }

        public virtual IPageMapping[] SelectAll()
        {
            return DataParser.SelectAll<IPageMapping>().ToArray();
        }

        public virtual IPageMapping[] SelectAllNonList(int typeId, bool onlyActive)
        {
            List<DatabaseDataValueColumn> whereClause = new List<DatabaseDataValueColumn>();

            if (typeId > 0)
            {
                whereClause.Add(new DatabaseDataValueColumn("PageMap_Type", SqlDbType.Int, typeId));
                whereClause.Add(new DatabaseDataValueColumn("PageMap_TargetType", SqlDbType.Int, 0));
            }
            // Special case for File redirects
            else if (typeId == -2)
            {
                whereClause.Add(new DatabaseDataValueColumn("PageMap_TargetType", SqlDbType.Int, 1));
            }
            if (onlyActive)
                whereClause.Add(new DatabaseDataValueColumn("PageMap_IsActive", SqlDbType.Bit, true));
            return DataParser.SelectAll<IPageMapping>(whereClause).ToArray();
        }

        public virtual bool Save(IPageMapping entity)
        {
            DataParser.Save<IPageMapping>(entity);
            return true;
        }

        public virtual void Delete(IPageMapping entity)
        {
            DataParser.Delete<IPageMapping>(entity);
        }
    }
}