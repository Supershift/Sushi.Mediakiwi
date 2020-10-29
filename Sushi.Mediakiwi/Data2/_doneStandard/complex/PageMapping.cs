using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Sushi.Mediakiwi.Data.DalReflection;
using Sushi.Mediakiwi.Framework;
using System.Web.UI.WebControls;
using System.Web;
using Sushi.Mediakiwi.Data.Parsers;

namespace Sushi.Mediakiwi.Data
{
    /// <summary>
    /// Pagemapings are virtual url's that are redirected to a real url or file.
    /// </summary>
    [DatabaseTable("wim_PageMappings")]   
    public class PageMapping : IPageMapping
    {
        static IPageMappingParser _Parser;
        static IPageMappingParser Parser
        {
            get
            {
                if (_Parser == null)
                    _Parser = Environment.GetInstance<IPageMappingParser>();
                return _Parser;
            }
        }

        public virtual string RedirectTo
        {
            get
            {
                if (Query != "?")
                    return String.Concat(PageName, Query);
                return PageName;
            }
        }

        public virtual string PageName
        {
            get
            {
                if (Page != null)
                    return Page.CompletePath;
                else
                    return string.Empty;
            }
            set { }
        }

        public virtual string TestLink
        {
            get
            {

                return string.Format(@"<a href=""{0}"" target=""_blank"">Test URL</a>", Wim.Utility.AddApplicationPath(Path));

            }
        }

        public virtual string EditLink
        {
            get
            {

                return string.Format(@"<a href=""{0}&item={1}"">Edit</a>", HttpContext.Current.Request.Url.ToString(), this.ID);

            }
        }
        public virtual string NavigateURL
        {
            get
            {
                return Wim.Utility.AddApplicationPath(Path);
            }
        }
        /// <summary>
        /// The type of redirect in text form
        /// </summary>
        public virtual string Type
        {
            get
            {
                if (TargetType == 0)
                {
                    foreach (ListItem item in MappingTypes)
                    {
                        if (item.Value == TypeID + "")
                            return item.Text;
                    }
                }
                else
                    return "Bestand";
                return "?";
            }
        }

        /// <summary>
        ///  Collectio of PageOrFile options
        /// </summary>
        public virtual ListItemCollection TargetTypes
        {
            get
            {
                var lc = new ListItemCollection();
                lc.Add(new ListItem("Pagina", "0"));
                lc.Add(new ListItem("Bestand", "1"));
                return lc;
            }
        }

        private static ListItemCollection m_MappingTypes;
        public static ListItemCollection MappingTypes
        {
            get
            {
                if (m_MappingTypes == null)
                {
                    m_MappingTypes = new ListItemCollection();
                    m_MappingTypes.Add(new ListItem("Rewrite(200)", ((int)PageMappingType.Rewrite200) + ""));
                    m_MappingTypes.Add(new ListItem("Tijdelijke redirect(302)", ((int)PageMappingType.Redirect302) + ""));
                    m_MappingTypes.Add(new ListItem("Permanente redirect(301)", ((int)PageMappingType.Redirect301) + ""));
                    m_MappingTypes.Add(new ListItem("Niet gevonden(404)", ((int)PageMappingType.NotFound404) + ""));
                }
                return m_MappingTypes;
            }
        }


        public static string ConvertUrl(string url)
        {
            return Parser.ConvertUrl(url);
        }


        /// <summary>
        /// Selects the one.
        /// </summary>
        /// <param name="relativePath">The relative path.</param>
        /// <returns></returns>
        public static IPageMapping SelectOne(string relativePath)
        {
            return Parser.SelectOne(relativePath);
        }

        public static IPageMapping[] SelectAllBasedOnPathPrefix(string prefix, int pageID)
        {
            return Parser.SelectAllBasedOnPathPrefix(prefix, pageID);
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
        public static IPageMapping RegisterUrl(string url, string query, string pageTitle, int pageID, bool applyApplicationPath, int? listID, int? itemID)
        {
            return Parser.RegisterUrl(url, query, pageTitle, pageID, applyApplicationPath, listID, itemID);
        }


        #region REPLICATED TO Sushi.Mediakiwi.Data.Standard

        public PageMapping() {
            this.Created = DateTime.UtcNow;// Sushi.Mediakiwi.Data.Common.DatabaseDateTime;
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

        /// <summary>
        /// Selects the one.
        /// </summary>
        /// <param name="listID">The list ID.</param>
        /// <param name="itemID">The item ID.</param>
        /// <returns></returns>
        public static IPageMapping SelectOne(int? listID, int itemID)
        {
            return Parser.SelectOne(listID, itemID);
        }

        public static IPageMapping SelectOne(int ID)
        {
            return Parser.SelectOne(ID);
        }

        public static IPageMapping SelectOne(int? listID, int itemID, int pageID)
        {
            return Parser.SelectOne(listID, itemID, pageID);
        }

        public static IPageMapping SelectOneByPageAndQuery(int pageID, string query)
        {
            return Parser.SelectOneByPageAndQuery(pageID, query);
        }

        public static IPageMapping[] SelectAllBasedOnPathPrefix(string prefix)
        {
            return Parser.SelectAllBasedOnPathPrefix(prefix);
        }

        public static IPageMapping[] SelectAllBasedOnPageID(int pageID)
        {
            return Parser.SelectAllBasedOnPageID(pageID);
        }

        public static IPageMapping[] SelectAll()
        {
            return Parser.SelectAll();
        }

        public static IPageMapping[] SelectAllNonList(int typeId, bool onlyActive)
        {
            return Parser.SelectAllNonList(typeId, onlyActive);
        }

        public virtual bool Save()
        {
            return Parser.Save(this);
        }

        public virtual void Delete()
        {
            Parser.Delete(this);
        }

        #region Properties
        public virtual bool IsNewInstance
        {
            get { return this.ID == 0; }
        }

        [DatabaseColumn("PageMap_Key", SqlDbType.Int, IsPrimaryKey = true)]
        public virtual int ID { get; set;  }


        public virtual bool IsInternalDoc { get; set; }

        public virtual bool IsInternalLink { get; set; }

        private string m_Path;
        /// <summary>
        /// The virtual url path which can be called by an user
        /// </summary>
        [Sushi.Mediakiwi.Framework.ContentListItem.TextField("Url", 150, InteractiveHelp="bv; /sectieA/onderdeelB/nietbestaandePagina")]
        [DatabaseColumn("PageMap_Path", SqlDbType.NVarChar)]
        public virtual string Path { get; set;  }

        /// <summary>
        /// The date on which the mapping is created
        /// </summary>
        [DatabaseColumn("PageMap_Created", SqlDbType.DateTime)]
        public virtual DateTime Created { get; set; }

  
        [DatabaseColumn("PageMap_List_Key", SqlDbType.Int, IsNullable = true)]
        public virtual int? ListID { get; set; }

        [DatabaseColumn("PageMap_Item_Key", SqlDbType.Int, IsNullable = true)]
        public virtual int? ItemID { get; set; }

        private int m_TargetType;
        /// <summary>
        /// PageOrFile tells whether the redirect is to a page or a file. 
        /// If PageOrFile=0 use the PageId
        /// If PageOrFile=1 use the AssetId
        /// </summary>
        [Sushi.Mediakiwi.Framework.ContentListItem.Choice_Radio("Pagina/Bestand", "TargetTypes", "TargetType", true, true)]
        [DatabaseColumn("PageMap_TargetType", SqlDbType.Int, IsNullable = true)]
        public virtual int TargetType { get; set; }

        /// <summary>
        /// This specifies which pagetype redirect is used. It influences the HTTP status used
        /// </summary>
        [Sushi.Mediakiwi.Framework.ContentListItem.Choice_Dropdown("Type", "MappingTypes")]
        [DatabaseColumn("PageMap_Type", SqlDbType.Int, IsNullable = true)]
        [Sushi.Mediakiwi.Framework.OnlyVisibleWhenTrue("IsInternalLink")]
        public virtual int TypeID { get; set; }
      

        /// <summary>
        /// Specifies the asset to redirect if PageOrFile=1
        /// </summary>
        [Sushi.Mediakiwi.Framework.ContentListItem.Binary_Document("Interne document")]
        [Sushi.Mediakiwi.Framework.OnlyVisibleWhenTrue("IsInternalDoc")]
        [DatabaseColumn("PageMap_Asset_Key", SqlDbType.Int, IsNullable = true)]
        public virtual int AssetID { get; set; }

        /// <summary>
        /// Specifies the page to redirect if PageOrFile=0
        /// </summary>
        [Sushi.Mediakiwi.Framework.ContentListItem.PageSelect("Interne pagina")]
        [Sushi.Mediakiwi.Framework.OnlyVisibleWhenTrue("IsInternalLink")]
        [DatabaseColumn("PageMap_Page_Key", SqlDbType.Int)]
        public virtual int PageID { get; set; }

        /// <summary>
        /// A specific Query string which is added to the end of the URL which the user is redirect to
        /// </summary>
        [Sushi.Mediakiwi.Framework.ContentListItem.TextField("Query string", 50, InteractiveHelp = "Gebruik dit voor een specifiek item weer te geven bv; ?productID=1")]
        [DatabaseColumn("PageMap_Query", SqlDbType.NVarChar, IsNullable = true)]
        [Sushi.Mediakiwi.Framework.OnlyVisibleWhenTrue("IsInternalLink")]
        public virtual string Query { get; set; }

        /// <summary>
        /// This sets the page title when a rewrite is used. If not specified the page title is used
        /// </summary>
        [Sushi.Mediakiwi.Framework.ContentListItem.TextField("Browser titel", 150, InteractiveHelp = "Als deze niet wordt ingevuld dan wordt de titel van de pagina gebruikt(kan alleen bij Rewrite)")]
        [DatabaseColumn("PageMap_Title", SqlDbType.NVarChar, IsNullable = true)]
        public virtual string Title { get; set; }

        private Sushi.Mediakiwi.Data.Page m_Page;
        public virtual Sushi.Mediakiwi.Data.Page Page
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
         [Sushi.Mediakiwi.Framework.ContentListItem.Choice_Checkbox("Is actief")]
        [DatabaseColumn("PageMap_IsActive", SqlDbType.Bit, IsNullable = true)]
        public virtual bool IsActive { get; set; }

        #endregion properties


        #endregion REPLICATED TO Sushi.Mediakiwi.Data.Standard
    }
}