using System;
using System.Linq;
using System.Text;
using System.Web;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Sushi.Mediakiwi.Data.DalReflection;
using System.Xml.Serialization;
using Sushi.Mediakiwi.Framework;
using System.Text.RegularExpressions;

namespace Sushi.Mediakiwi.Data
{

    /// <summary>
    /// Represents a Page entity.
    /// </summary>
    [DatabaseTable("wim_Pages", Order = "Page_SortOrder ASC", Join = "left join wim_PageTemplates on Page_Template_Key = PageTemplate_Key left join wim_Folders on Page_Folder_Key = Folder_Key left join wim_Sites on Site_Key = Folder_SIte_Key")]
    public class Page : DatabaseEntity, iExportable
    {
       
        /// <summary>
        /// Selects all uninherited.
        /// </summary>
        /// <param name="masterID">The master ID.</param>
        /// <param name="siteID">The site ID.</param>
        /// <returns></returns>
        internal static Page[] SelectAllUninherited(int masterID, int siteID)
        {
            List<Page> list = new List<Page>();
            Page page = new Page();
            page.SqlTable = "wim_Pages x";

            List<DatabaseDataValueColumn> where = new List<DatabaseDataValueColumn>();
            where.Add(new DatabaseDataValueColumn("Folder_Site_Key", SqlDbType.Int, masterID));
            where.Add(new DatabaseDataValueColumn(
                string.Format("(select COUNT(*) from wim_Pages left join wim_Folders on Page_Folder_Key = Folder_Key where Folder_Site_Key = {0} and Page_Master_Key = x.Page_Key) = 0", siteID)));

            foreach (object o in page._SelectAll(where)) list.Add((Page)o);
            return list.ToArray();
        }


        /// <summary>
        /// The complete page path including the possible application path
        /// </summary>
        /// <value>The complete path.</value>
        public string CompletePath
        {
            get
            {
                HttpContext context = HttpContext.Current;
                if (context != null)
                {
                    var islocal = Wim.CommonConfiguration.IS_LOCAL_DEVELOPMENT;
                    if (!islocal)
                    {
                        if (!string.IsNullOrEmpty(this.Site.Domain))
                        {
                            string tmp = string.IsNullOrEmpty(this.Site.DefaultFolder)
                                ? InternalPath
                                //  [20 nov 14:mm] Added as this goes wrong with some complete paths. With inheritence it does not add the default folder to the completepath (localised pages do have this).
                                : InternalPath.StartsWith(string.Concat(this.Site.DefaultFolder, "/"), StringComparison.InvariantCultureIgnoreCase)
                                    ? InternalPath.Remove(0, this.Site.DefaultFolder.Length)
                                    : InternalPath;

                            if (!this.Site.IsDomain(context.Request.Url))
                            {
                                string clean = Wim.Utility.AddApplicationPath(tmp);

                                return string.Concat(context.Request.Url.Scheme, "://"
                                        , this.Site.Domains[0]
                                        , context.Request.Url.Port == 80 ? "" : string.Concat(":", context.Request.Url.Port)
                                        , clean
                                    );
                            }

                            return Wim.Utility.AddApplicationPath(tmp);
                        }
                    }
                }
                return Wim.Utility.AddApplicationPath(InternalPath);
            }
        }


        /// <summary>
        /// Gets the exposed URL.
        /// </summary>
        /// <value>
        /// The URL.
        /// </value>
        public string Url
        {
            get
            {
                return HRefFull;
            }
        }

        /// <summary>
        /// The complete page URL
        /// </summary>
        /// <value>The H ref full.</value>
        public string HRefFull
        {
            get
            {
                if (string.IsNullOrEmpty(Name))
                    return null;

                string ext = Sushi.Mediakiwi.Data.Environment.Current.GetRegistryValue("PAGE_WILDCARD_EXTENTION", "aspx");
                ext = (ext == ".") ? "" : string.Concat(".", ext);

                if (CompletePath.StartsWith(HttpContext.Current.Request.Url.Scheme))
                    return string.Concat(CompletePath, ext);


                string[] urlArr =  HttpContext.Current.Request.Url.AbsoluteUri.Split('/');
                //  Creates /www.url.ext
                string rebuild = string.Format("{0}/{1}", urlArr[1], urlArr[2]);

                //  Creates /www.url.ext/folder/file.ext
                string tmp = string.Concat(rebuild, CompletePath, ext);

                if (Wim.CommonConfiguration.REDIRECT_CHANNEL_PATH
                    && !Wim.CommonConfiguration.IS_LOCAL_DEVELOPMENT
                    && !string.IsNullOrEmpty(this.Site.Domain))
                {
                    tmp = string.Concat("/", this.Site.Domains[0], Wim.Utility.RemApplicationPath(CompletePath), ext);
                }

                string unEncoded = Utility.GlobalRegularExpression.Implement.CleanRelativePathSlash.Replace(tmp, "/");
                //  Creates http://www.url.ext/folder/file.ext
                return string.Format("{0}/{1}", urlArr[0], Utility.CleanUrl(unEncoded));
            }
        }

        /// <summary>
        /// The relative page URL
        /// </summary>
        /// <value>The H ref.</value>
        public string HRef
        {
            get
            {
                if (string.IsNullOrEmpty(Name))
                    return null;

                string ext = Sushi.Mediakiwi.Data.Environment.Current.GetRegistryValue("PAGE_WILDCARD_EXTENTION", "aspx");
                ext = (ext == ".") ? "" : string.Concat(".", ext);

                bool isHome = false;
                if (ext == "" && this.Site.HomepageID.HasValue && Sushi.Mediakiwi.Data.Environment.Current.DefaultSiteID.HasValue && Sushi.Mediakiwi.Data.Environment.Current.DefaultSiteID.Value == this.Site.ID)
                    isHome = (this.Site.HomepageID.Value == this.ID);

                string tmp = string.Concat(CompletePath, ext);
                if (CompletePath.StartsWith(HttpContext.Current.Request.Url.Scheme))
                {
                    return isHome ? tmp.Replace(this.Name, string.Empty) : tmp;
                }

                //string unEncoded = Utility.GlobalRegularExpression.Implement.CleanRelativePathSlash.Replace(tmp, "/");
                tmp = Utility.CleanUrl(tmp);
                return isHome ? tmp.Replace(this.Name, string.Empty) : tmp;
            }
        }

        /// <summary>
        /// Gets the H ref_ short.
        /// </summary>
        /// <value>The H ref_ short.</value>
        internal string HRef_Short
        {
            get
            {
                if (string.IsNullOrEmpty(Name))
                    return null;

                string ext = Sushi.Mediakiwi.Data.Environment.Current.GetRegistryValue("PAGE_WILDCARD_EXTENTION", "aspx");
                ext = (ext == ".") ? "" : string.Concat(".", ext);

                string completePath = Wim.Utility.AddApplicationPath(InternalPath);

                string tmp = string.Concat(completePath, ext);

                if (CompletePath.StartsWith(HttpContext.Current.Request.Url.Scheme))
                    return tmp;

                //string unEncoded = Utility.GlobalRegularExpression.Implement.CleanRelativePathSlash.Replace(tmp, "/");
                return Utility.CleanUrl(tmp);
            }
        }

        /// <summary>
        /// Gets the local cache file.
        /// </summary>
        /// <value>The local cache file.</value>
        //public string LocalCacheFile
        //{
        //    get
        //    {
        //        if (string.IsNullOrEmpty(Name))
        //            return null;

        //        string tmp = string.Concat("/repository/cache/", InternalPath, ".html");
        //        return System.Web.HttpContext.Current.Server.MapPath(Wim.Utility.AddApplicationPath(tmp));
        //    }
        //}

        /// <summary>
        /// Selects the one.
        /// </summary>
        /// <param name="guid">The GUID.</param>
        /// <param name="portal">The portal.</param>
        /// <returns></returns>
        public static Sushi.Mediakiwi.Data.Page SelectOne(Guid guid, string portal)
        {
            Page tmp = new Page();
            List<DatabaseDataValueColumn> where = new List<DatabaseDataValueColumn>();
            where.Add(new DatabaseDataValueColumn("Page_GUID", SqlDbType.UniqueIdentifier, guid));

            if (!string.IsNullOrEmpty(portal))
            {
                tmp.SqlConnectionString = Sushi.Mediakiwi.Data.Common.GetPortal(portal).Connection;
                tmp.ConnectionType = Sushi.Mediakiwi.Data.Common.GetPortal(portal).Type;
            }

            return (Page)tmp._SelectOne(where, "GUID", guid.ToString());
            //return (Page)new Page()._SelectOne(where, "ID", pageID.ToString());
        }

        /// <summary>
        /// Selects one by PageID and PortalMappping.
        /// </summary>
        /// <param name="pageID">The Page ID.</param>
        /// <param name="portal">The portal.</param>
        /// <returns></returns>
        public static Sushi.Mediakiwi.Data.Page SelectOne(int pageID, string portal)
        {
            Page tmp = new Page();
            List<DatabaseDataValueColumn> where = new List<DatabaseDataValueColumn>();
            where.Add(new DatabaseDataValueColumn("Page_Key", SqlDbType.Int, pageID));

            if (!string.IsNullOrEmpty(portal))
            {
                tmp.SqlConnectionString = Sushi.Mediakiwi.Data.Common.GetPortal(portal).Connection;
                tmp.ConnectionType = Sushi.Mediakiwi.Data.Common.GetPortal(portal).Type;
            }

            return (Page)tmp._SelectOne(where);
        }

        /// <summary>
        /// Selects the one_ import export.
        /// </summary>
        /// <param name="portal">The portal.</param>
        /// <param name="guid">The GUID.</param>
        /// <param name="folderID">The folder ID.</param>
        /// <param name="completePath">The complete path.</param>
        /// <returns></returns>
        public static Page SelectOne_ImportExport(string portal, Guid guid, int folderID, string name)
        {
            Page implement = new Page();

            implement.SqlConnectionString = Sushi.Mediakiwi.Data.Common.GetPortal(portal).Connection;
            implement.ConnectionType = Sushi.Mediakiwi.Data.Common.GetPortal(portal).Type;

            List<DatabaseDataValueColumn> where = new List<DatabaseDataValueColumn>();
            where.Add(new DatabaseDataValueColumn("Page_Folder_Key", SqlDbType.Int, folderID));
            where.Add(new DatabaseDataValueColumn("Page_GUID", SqlDbType.UniqueIdentifier, guid));
            where.Add(new DatabaseDataValueColumn("Page_Name", SqlDbType.NVarChar, name, DatabaseDataValueConnectType.Or));

            return (Page)implement._SelectOne(where);
        }

        /// <summary>
        /// Selects the one_ by component template.
        /// </summary>
        /// <param name="componentTemplateID">The component template ID.</param>
        /// <returns></returns>
        internal static Sushi.Mediakiwi.Data.Page SelectOneByComponentTemplate(int componentTemplateID)
        {
            Page implement = new Page();
            implement.SqlRowCount = 1;
            implement.SqlJoin += " join wim_ComponentVersions on ComponentVersion_Page_Key = page_Key";

            List<DatabaseDataValueColumn> where = new List<DatabaseDataValueColumn>();
            where.Add(new DatabaseDataValueColumn("ComponentVersion_ComponentTemplate_Key", SqlDbType.Int, componentTemplateID));

            return (Page)implement._SelectOne(where);
        }

        /// <summary>
        /// Select a page entity
        /// </summary>
        /// <param name="completePath">Folderpath to search in (without applicationpath!)</param>
        /// <param name="pageName">Page to look for</param>
        /// <param name="returnOnlyPublishedPage">if set to <c>true</c> [return only published page].</param>
        /// <returns>Page entity</returns>
        public static Sushi.Mediakiwi.Data.Page SelectOne(string completePath, string pageName, bool returnOnlyPublishedPage)
        {
            List<DatabaseDataValueColumn> where = new List<DatabaseDataValueColumn>();

            string url = Utility.CleanUrl(string.Concat(completePath, pageName));

            where.Add(new DatabaseDataValueColumn("Page_CompletePath", SqlDbType.NVarChar, url, 500));
            Page page = new Page();

            page.SqlJoin = "join wim_PageTemplates on Page_Template_Key = PageTemplate_Key";

            page.CollectionLevel = DatabaseColumnGroup.Minimal;

            page = (Page)page._SelectOne(where, "completepath", $"{url}");

            if (page.IsNewInstance)
            {
                //  Second try for 
                where = new List<DatabaseDataValueColumn>();
                where.Add(new DatabaseDataValueColumn("Page_CompletePath", SqlDbType.NVarChar, completePath, 500, DatabaseDataValueCompareType.Default, DatabaseDataValueConnectType.Or));
                where.Add(new DatabaseDataValueColumn("Page_Name", SqlDbType.NVarChar, pageName));
                page = (Page)page._SelectOne(where);
            }
            if (page.IsNewInstance) return null;
            return SelectOne(page.ID, returnOnlyPublishedPage);
        }


        internal class ContextContainer
        {
            public HttpContext Context;
            public Sushi.Mediakiwi.Data.IApplicationUser User;
            public IPagePublication PagePublication;
        }

        /// <summary>
        /// Clean up the component list after publication because some residual components could stil be present.
        /// </summary>
        /// <param name="pageID">The page ID.</param>
        //void CleanUpAfterPublication(int pageID)
        //{
        //    //Page candidate = new Page();
        //    //string sqlText = string.Format(@"delete from wim_ComponentVersions left join wim_Components on Component_GUID = ComponentVersion_GUID where Component_Page_Key = {0} and ComponentVersion_IsActive = 1 and Component_Key IS NULL", pageID);
        //    //candidate.Execute(sqlText);
        //}


        /// <summary>
        /// Select all pages in a folder.
        /// The default setting is: select all published pages in the requested folder sorted by the linkText.
        /// </summary>
        /// <param name="folderID">The folder ID.</param>
        /// <returns></returns>
        public static Page[] SelectAll(int folderID)
        {
            return SelectAll(folderID, PageFolderSortType.Folder);
        }

        /// <summary>
        /// Select all pages in a folder.
        /// The default setting is: select all published pages in the requested folder sorted by the linkText.
        /// </summary>
        /// <param name="folderID">The folder ID.</param>
        /// <param name="folderType">Type of the folder.</param>
        /// <returns></returns>
        public static Page[] SelectAll(int folderID, PageFolderSortType folderType)
        {
            return SelectAll(folderID, folderType, PageReturnProperySet.All);
        }

        /// <summary>
        /// Select all pages in a folder.
        /// The default setting is: select all published pages in the requested folder sorted by the linkText.
        /// </summary>
        /// <param name="folderID">The folder ID.</param>
        /// <param name="folderType">Type of the folder.</param>
        /// <param name="propertySet">The property set.</param>
        /// <returns></returns>
        public static Page[] SelectAll(int folderID, PageFolderSortType folderType, PageReturnProperySet propertySet)
        {
            return SelectAll(folderID, folderType, propertySet, PageSortBy.LinkText);
        }

        /// <summary>
        /// Select all pages in a folder.
        /// The default setting is: select all published pages in the requested folder sorted by the linkText.
        /// </summary>
        /// <param name="folderID">The folder ID.</param>
        /// <param name="folderType">Type of the folder.</param>
        /// <param name="propertySet">The property set.</param>
        /// <param name="sort">The sort.</param>
        /// <returns></returns>
        public static Page[] SelectAll(int folderID, PageFolderSortType folderType, PageReturnProperySet propertySet, PageSortBy sort)
        {
            return SelectAll(folderID, folderType, propertySet, sort, true);
        }

        /// <summary>
        /// Select all pages in a folder.
        /// The default setting is: select all published pages in the requested folder sorted by the linkText.
        /// </summary>
        /// <param name="folderID">The folder ID.</param>
        /// <param name="folderType">Type of the folder.</param>
        /// <param name="propertySet">The property set.</param>
        /// <param name="sort">The sort.</param>
        /// <param name="onlyReturnPublishedPages">if set to <c>true</c> [only return published pages].</param>
        /// <returns></returns>
        public static Page[] SelectAll(int folderID, PageFolderSortType folderType, PageReturnProperySet propertySet, PageSortBy sort, bool onlyReturnPublishedPages)
        {
            using (Wim.Utilities.CacheItemManager cman = new Wim.Utilities.CacheItemManager())
            {
                string ckey = string.Concat("Data_Sushi.Mediakiwi.Data.Page.", Wim.Utility.HashStringBySHA1(string.Concat(folderType, propertySet, sort, onlyReturnPublishedPages)));

                //  Get page from cache only when a published page is requested and the key exists is cache.
                if (cman.IsCached(ckey))
                {
                    Page[] tmp = cman.GetItem(ckey) as Page[];
                    if (tmp != null)
                    {
                        if (folderType == PageFolderSortType.Folder)
                        {
                            var sublist = (from item in tmp where item.FolderID == folderID select item).ToArray();
                            return sublist;
                        }
                        else
                        {
                            var sublist = (from item in tmp where item.ParentFolderID == folderID select item).ToArray();
                            return sublist;
                        }
                    }
                }

                List<Page> list = new List<Page>();
                Page page = new Page();

                if (sort == PageSortBy.Name) page.SqlOrder = "Page_Name ASC";
                if (sort == PageSortBy.LinkText) page.SqlOrder = "Page_LinkText ASC";
                if (sort == PageSortBy.SortOrder) page.SqlOrder = "Page_SortOrder ASC";
                if (sort == PageSortBy.CustomDate) page.SqlOrder = "Page_CustomDate ASC, Page_SortOrder ASC";
                if (sort == PageSortBy.CustomDateDown) page.SqlOrder = "Page_CustomDate DESC, Page_SortOrder ASC";

                List<DatabaseDataValueColumn> where = new List<DatabaseDataValueColumn>();
                switch (propertySet)
                {
                    case PageReturnProperySet.OnlyDefault:
                        where.Add(new DatabaseDataValueColumn("Page_IsDefault", SqlDbType.Bit, true));
                        break;
                    case PageReturnProperySet.AllExceptDefault:
                        where.Add(new DatabaseDataValueColumn("Page_IsDefault", SqlDbType.Bit, false));
                        break;
                }

                if (onlyReturnPublishedPages)
                    where.Add(new DatabaseDataValueColumn("Page_IsPublished", SqlDbType.Bit, onlyReturnPublishedPages));

                //if (folderType == PageFolderSortType.Folder)
                //    where.Add(new DatabaseDataValueColumn("Folder_Key", SqlDbType.Int, folderID));
                //else
                //    where.Add(new DatabaseDataValueColumn("Folder_Folder_Key", SqlDbType.Int, folderID));

                if (onlyReturnPublishedPages)
                {
                    where.Add(new DatabaseDataValueColumn("Page_Publish", SqlDbType.DateTime, null, 0));
                    where.Add(new DatabaseDataValueColumn("Page_Publish", SqlDbType.DateTime, DateTime.Now, DatabaseDataValueCompareType.SmallerThen, DatabaseDataValueConnectType.Or));
                    where.Add(new DatabaseDataValueColumn("Page_Expire", SqlDbType.DateTime, null));
                    where.Add(new DatabaseDataValueColumn("Page_Expire", SqlDbType.DateTime, DateTime.Now, DatabaseDataValueCompareType.BiggerThen, DatabaseDataValueConnectType.Or));
                }

                foreach (object o in page._SelectAll(where))
                    list.Add((Page)o);


                //if (onlyReturnPublishedPages)
                cman.Add(ckey, list.ToArray(), Common.EntityCacheExpiration, null);

                if (folderType == PageFolderSortType.Folder)
                {
                    var sublist = (from item in list where item.FolderID == folderID select item).ToArray();
                    return sublist;
                }
                else
                {
                    var sublist = (from item in list where item.ParentFolderID == folderID select item).ToArray();
                    return sublist;
                }
            }
        }


        #region MOVED to EXTENSION / LOGIC

        ///// <summary>
        ///// Apply a link to this page.
        ///// </summary>
        ///// <param name="hyperlink">The hyperlink to apply the page linkage properties to.</param>
        //public void Apply(System.Web.UI.WebControls.HyperLink hyperlink)
        //{
        //    Apply(hyperlink, false);
        //}

        //public void Apply(System.Web.UI.WebControls.HyperLink hyperlink, bool onlySetNavigationUrl)
        //{
        //    Apply(hyperlink, onlySetNavigationUrl, false);
        //}

        ///// <summary>
        ///// Apply a link to this page.
        ///// </summary>
        ///// <param name="hyperlink">The hyperlink to apply the page linkage properties to.</param>
        ///// <param name="onlySetNavigationUrl">Only set the navigation url, leave the text property as is.</param>
        //public void Apply(System.Web.UI.WebControls.HyperLink hyperlink, bool onlySetNavigationUrl, bool setFullUrlPath)
        //{
        //    hyperlink.Visible = false;

        //    if (Name == null || LinkText.Trim().Length == 0)
        //        return;
        //    if (LinkText == null || LinkText.Trim().Length == 0)
        //        m_LinkText = Name;

        //    if (!IsPublished)
        //        return;

        //    if (Publication != DateTime.MinValue)
        //    {
        //        if (DateTime.Now.Ticks < Publication.Ticks)
        //            return;
        //    }
        //    if (Expiration != DateTime.MinValue)
        //    {
        //        if (DateTime.Now.Ticks > Expiration.Ticks)
        //            return;
        //    }

        //    if (setFullUrlPath)
        //        hyperlink.NavigateUrl = HRefFull;
        //    else
        //        hyperlink.NavigateUrl = HRef;

        //    if (!onlySetNavigationUrl)
        //        hyperlink.Text = HttpContext.Current.Server.HtmlEncode(LinkText);

        //    hyperlink.ToolTip = Description == null ? "" : "";
        //    hyperlink.Visible = true;
        //}

        ///// <summary>
        ///// Gets the local cache href.
        ///// </summary>
        ///// <param name="queryString">The query string.</param>
        ///// <returns></returns>
        ///// <value>The local cache href.</value>
        //public string GetLocalCacheHref(System.Collections.Specialized.NameValueCollection queryString)
        //{
        //    if (string.IsNullOrEmpty(Name))
        //        return null;

        //    string add = null;
        //    if (queryString.Count > 0)
        //    {
        //        add = "_q";
        //        foreach (string key in queryString.AllKeys)
        //        {
        //            add += string.Concat("_", key, "_", queryString[key]);
        //        }
        //    }

        //    string tmp = string.Concat("/repository/cache/", InternalPath, add, ".html");
        //    return Wim.Utility.AddApplicationPath(tmp);
        //}

        //public string GetLocalCacheFile(System.Collections.Specialized.NameValueCollection queryString)
        //{
        //    if (string.IsNullOrEmpty(Name))
        //        return null;

        //    string add = null;
        //    if (queryString.Count > 0)
        //    {
        //        add = "_q";
        //        foreach (string key in queryString.AllKeys)
        //        {
        //            if (key != "?")
        //                add += string.Concat("_", key, "_", queryString[key]);
        //        }
        //    }

        //    string tmp = string.Concat("/repository/cache/", InternalPath, add, ".html");
        //    return System.Web.HttpContext.Current.Server.MapPath(Wim.Utility.AddApplicationPath(tmp));
        //}

        ////  Supporting fields
        //private Sushi.Mediakiwi.Data.Content[] m_ContentItems;
        //private Sushi.Mediakiwi.Data.Component[] m_Components;
        //private int m_CurrentCalledcomponentTemplateKey;
        ///// <summary>
        ///// Get all property content from the page component 
        ///// </summary>
        ///// <param name="componentTemplateKey">Component template to search in</param>
        ///// <param name="propertyName">The property to look for and return its value</param>
        ///// <returns>The property values</returns>
        //public string[] GetComponentProperties(int componentTemplateKey, string propertyName)
        //{
        //    if (m_Components == null || componentTemplateKey != m_CurrentCalledcomponentTemplateKey)
        //    {
        //        m_CurrentCalledcomponentTemplateKey = componentTemplateKey;
        //        m_Components = Sushi.Mediakiwi.Data.Component.SelectAll(this.ID, componentTemplateKey);

        //        List<Sushi.Mediakiwi.Data.Content> contentList = new List<Content>();

        //        foreach (Sushi.Mediakiwi.Data.Component component in m_Components)
        //        {
        //            //  Get content
        //            if (component.Serialized_XML == null || component.Serialized_XML.Length == 0)
        //                continue;

        //            //  Get deserialized content
        //            Sushi.Mediakiwi.Data.Content content =
        //                Sushi.Mediakiwi.Data.Content.GetDeserialized(component.Serialized_XML);

        //            //  Validate fields
        //            if (content.Fields == null || content.Fields.Length == 0)
        //                continue;

        //            contentList.Add(content);
        //        }

        //        m_ContentItems = contentList.ToArray();

        //    }
        //    if (m_ContentItems == null || m_ContentItems.Length == 0)
        //        return null;

        //    List<string> candidates = new List<string>();
        //    foreach (Sushi.Mediakiwi.Data.Content content in m_ContentItems)
        //    {
        //        foreach (Sushi.Mediakiwi.Data.Content.Field field in content.Fields)
        //        {
        //            if (field.Property == propertyName)
        //            {
        //                if (field.Value == null || field.Value.Length == 0)
        //                    continue;

        //                string candidate = field.Value;

        //                if (field.Type == (int)Sushi.Mediakiwi.Framework.ContentType.RichText)
        //                {
        //                    //Sushi.Mediakiwi.Framework.Templates.RichLink richLink =
        //                    //    new Sushi.Mediakiwi.Framework.Templates.RichLink();

        //                    candidate = Wim.Utility.ApplyRichtextLinks(this.Site, field.Value);
        //                    //candidate = Sushi.Mediakiwi.Framework.Templates.RichLink.GetCleaner.Replace(field.Value, richLink.CleanLink);
        //                }

        //                candidates.Add(candidate);
        //            }
        //        }
        //    }
        //    return candidates.ToArray();
        //}

        ///// <summary>
        ///// Gets the component properties.
        ///// </summary>
        ///// <param name="sourceTag">The source tag.</param>
        ///// <param name="propertyName">Name of the property.</param>
        ///// <returns></returns>
        //public string[] GetComponentProperties(string sourceTag, string propertyName)
        //{
        //    var ct = Sushi.Mediakiwi.Data.ComponentTemplate.SelectOneBySourceTag(sourceTag);
        //    if (ct == null || ct.IsNewInstance)
        //        return null;

        //    return GetComponentProperties(ct.ID, propertyName);
        //}

        ///// <summary>
        ///// Get the first property content from the page component
        ///// </summary>
        ///// <param name="componentTemplateKey">Component template to search in</param>
        ///// <param name="propertyName">The property to look for and return its value</param>
        ///// <returns>The property value</returns>
        //public string GetComponentProperty(int componentTemplateKey, string propertyName)
        //{
        //    string[] candidates = GetComponentProperties(componentTemplateKey, propertyName);
        //    if (candidates != null && candidates.Length > 0)
        //        return candidates[0];
        //    return null;
        //}

        //internal void CopyComponents(int userID)
        //{
        //    Sushi.Mediakiwi.Data.Component[] liveComponents = Sushi.Mediakiwi.Data.Component.SelectAllInherited(this.ID, true);
        //    Sushi.Mediakiwi.Data.ComponentVersion[] stagingComponents = Sushi.Mediakiwi.Data.ComponentVersion.SelectAll(this.ID);

        //    CopyComponents(userID, liveComponents, stagingComponents);

        //    //Sushi.Mediakiwi.Data.Component[] liveComponentsShared = Sushi.Mediakiwi.Data.Component.SelectAllShared(this.ID);
        //    //Sushi.Mediakiwi.Data.ComponentVersion[] stagingComponentsShared = Sushi.Mediakiwi.Data.ComponentVersion.SelectAllShared(this.ID);
        //    //CopyComponents(userID, liveComponentsShared, stagingComponentsShared);
        //}

        ///// <summary>
        ///// Publishes the specified user ID.
        ///// </summary>
        ///// <param name="userID">The user ID.</param>
        //void CopyComponents(int userID, Sushi.Mediakiwi.Data.Component[] liveComponents, Sushi.Mediakiwi.Data.ComponentVersion[] stagingComponents)
        //{
        //    System.Guid batchGuid = System.Guid.NewGuid();

        //    int sortOrderCount = 0;

        //    List<int> foundComponentArr = new List<int>();
        //    foreach (Sushi.Mediakiwi.Data.ComponentVersion componentStaged in stagingComponents)
        //    {
        //        if (!componentStaged.IsActive) continue;
        //        sortOrderCount++;

        //        bool foundComponent = false;
        //        foreach (Sushi.Mediakiwi.Data.Component component in liveComponents)
        //        {
        //            if (component.GUID == componentStaged.GUID)
        //            {
        //                //  Set the found component
        //                foundComponentArr.Add(component.ID);

        //                componentStaged.Apply(component);
        //                component.Save();
        //                component.SortOrder = sortOrderCount;
        //                foundComponent = true;
        //                break;
        //            }
        //        }
        //        if (foundComponent) continue;

        //        try
        //        {
        //            Sushi.Mediakiwi.Data.Component component2 = new Component();
        //            componentStaged.Apply(component2);
        //            component2.SortOrder = sortOrderCount;
        //            component2.Save();
        //        }
        //        catch (Exception ex)
        //        {
        //            Console.WriteLine(ex.Message);
        //        }
        //    }

        //    bool found = false;
        //    foreach (Sushi.Mediakiwi.Data.Component component in liveComponents)
        //    {
        //        foreach (int id in foundComponentArr)
        //        {
        //            if (id == component.ID)
        //            {
        //                found = true;
        //                break;
        //            }
        //        }
        //        if (!found)
        //            component.Delete();
        //        found = false;
        //    }
        //    //CleanUpAfterPublication(this.ID);

        //}

        ///// <summary>
        ///// Publishes the page
        ///// </summary>
        ///// <param name="?">The ?.</param>
        ///// <param name="user">The user.</param>
        //public void Publish(IPagePublication pagePublication, Sushi.Mediakiwi.Data.IApplicationUser user)
        //{
        //    Sushi.Mediakiwi.Data.Page.ContextContainer context = new Sushi.Mediakiwi.Data.Page.ContextContainer();
        //    context.Context = HttpContext.Current;
        //    context.User = user;
        //    context.PagePublication = pagePublication;

        //    if (pagePublication.DoPrePublishValidation(user, this))
        //    {
        //        System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(this.PublishPage), context);

        //        if (!string.IsNullOrEmpty(this.HRef_Short))
        //            HttpResponse.RemoveOutputCacheItem(this.HRef_Short);
        //    }
        //}

        ///// <summary>
        ///// Takes down.
        ///// </summary>
        ///// <param name="userID">The user ID.</param>
        //public void TakeDown(IPagePublication pagePublication, Sushi.Mediakiwi.Data.IApplicationUser user)
        //{
        //    Sushi.Mediakiwi.Data.Page.ContextContainer context = new Sushi.Mediakiwi.Data.Page.ContextContainer();
        //    context.Context = HttpContext.Current;
        //    context.User = user;
        //    context.PagePublication = pagePublication;

        //    System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(this.TakeDownPage), context);

        //    if (!string.IsNullOrEmpty(this.HRef))
        //        HttpResponse.RemoveOutputCacheItem(this.HRef);
        //}

        ///// <summary>
        ///// Takes down page.
        ///// </summary>
        ///// <param name="context">The context.</param>
        //internal void TakeDownPage(object context)
        //{
        //    ContextContainer c = (ContextContainer)context;
        //    try
        //    {
        //        this.Updated = Sushi.Mediakiwi.Data.Common.DatabaseDateTime;
        //        this.Published = DateTime.MinValue;
        //        this.IsPublished = false;
        //        this.Save();

        //        CleanUpAfterTakeDown(this.ID);

        //        Sushi.Mediakiwi.Data.EnvironmentVersionLogic.Flush(true, c.Context);
        //    }
        //    catch (Exception ex)
        //    {
        //        Sushi.Mediakiwi.Data.Notification.InsertOne("Threading (page)", ex);
        //    }
        //}
        ///// <summary>
        ///// Clean up the component list after take done because some residual components could stil be present.
        ///// </summary>
        ///// <param name="pageID">The page ID.</param>
        //void CleanUpAfterTakeDown(int pageID)
        //{
        //    DeleteAllComponentSearchReferences(pageID);
        //    using (Sushi.Mediakiwi.Data.Connection.DataCommander dac = new Sushi.Mediakiwi.Data.Connection.DataCommander(this.SqlConnectionString))
        //    {
        //        //  First cleanup ComponentSearch and secondly cleanup Components
        //        dac.Text = @"delete from wim_Components where Component_Page_Key = @Page_Key";
        //        dac.SetParameterInput("@Page_Key", pageID, SqlDbType.Int);
        //        dac.ExecNonQuery();
        //    }
        //}

        ///// <summary>
        ///// Publishes the page.
        ///// </summary>
        ///// <param name="context">The context.</param>
        //internal void PublishPage(object context)
        //{
        //    this.SetCompletePath();
        //    ContextContainer c = (ContextContainer)context;
        //    try
        //    {
        //        this.Updated = Sushi.Mediakiwi.Data.Common.DatabaseDateTime;
        //        this.Published = this.Updated;
        //        this.IsPublished = true;
        //        this.InheritContent = this.InheritContentEdited;

        //        int userID = c.User == null ? 0 : c.User.ID;
        //        CopyComponents(userID);
        //        //Publish(c.UserID, false);

        //        this.Save();

        //        Sushi.Mediakiwi.Data.EnvironmentVersionLogic.Flush(true, c.Context);

        //        SubmitPageForSearch(this.ID);

        //        if (c.PagePublication != null)
        //            c.PagePublication.DoPostPublishValidation(c.User, this);

        //    }
        //    catch (Exception ex)
        //    {
        //        Sushi.Mediakiwi.Data.Notification.InsertOne("Threading (page)", ex);
        //    }
        //}


        ///// <summary>
        ///// Submits the page for search.
        ///// </summary>
        //public void SubmitPageForSearch()
        //{
        //    SubmitPageForSearch(this.ID);
        //}

        ///// <summary>
        ///// Submits the page for search.
        ///// </summary>
        ///// <param name="pageID">The page ID.</param>
        //void SubmitPageForSearch(int pageID)
        //{
        //    if (!this.IsSearchable)
        //    {
        //        DeleteAllComponentSearchReferences(pageID);
        //        return;
        //    }

        //    Sushi.Mediakiwi.Data.Component[] pageComponents = Sushi.Mediakiwi.Data.Component.SelectAll(pageID);

        //    StringBuilder searchableContent = null;
        //    foreach (Sushi.Mediakiwi.Data.Component component in pageComponents)
        //    {
        //        if (!component.IsSearchable) continue;

        //        searchableContent = new StringBuilder();

        //        //  Get content
        //        if (component.Serialized_XML == null || component.Serialized_XML.Length == 0)
        //            continue;

        //        Sushi.Mediakiwi.Data.Content content =
        //            Sushi.Mediakiwi.Data.Content.GetDeserialized(component.Serialized_XML);

        //        //  Get correct field content
        //        if (content.Fields == null || content.Fields.Length == 0)
        //            continue;

        //        foreach (Sushi.Mediakiwi.Data.Content.Field field in content.Fields)
        //        {
        //            if (field.Value == null || field.Value.Length == 0)
        //                continue;

        //            if (field.Type == (int)Sushi.Mediakiwi.Framework.ContentType.TextField ||
        //                field.Type == (int)Sushi.Mediakiwi.Framework.ContentType.TextArea ||
        //                field.Type == (int)Sushi.Mediakiwi.Framework.ContentType.RichText)
        //            {
        //                searchableContent.Append(" " + field.Value);
        //            }
        //        }

        //        if (searchableContent.Length != 0)
        //        {
        //            string finalContent = searchableContent.ToString();

        //            Sushi.Mediakiwi.Data.ComponentSearch.AddOne(1, component.ID, finalContent, this.SiteID);
        //        }
        //    }
        //    //using (Sushi.Mediakiwi.Data.Connection.DataCommander dac = new Sushi.Mediakiwi.Data.Connection.DataCommander(this.SqlConnectionString))
        //    //{
        //    //    dac.Text = string.Format("delete from wim_Components where Component_Page_Key = {0} and not Component_Key in (select Component_Key from wim_Components join wim_ComponentVersions on Component_GUID = ComponentVersion_GUID where Component_Page_Key = {0})", pageID);
        //    //    dac.ExecNonQuery();
        //    //}
        //}


        ///// <summary>
        ///// Saves component and page state to a pageversion and inserts an audit trail
        ///// </summary>
        ///// <param name="targetComponents">The components of the page</param>
        ///// <param name="user">the user performing the operation</param>
        //private void SaveThisVersion(Sushi.Mediakiwi.Data.ComponentVersion[] targetComponents, IApplicationUser user)
        //{

        //    StringBuilder contentHash = new StringBuilder();
        //    foreach (var version in targetComponents)
        //    {

        //        var content = version.GetContent();

        //        if (content != null && content.Fields != null)
        //        {

        //            foreach (var item in content.Fields)
        //            {
        //                if (!string.IsNullOrEmpty(item.Value))
        //                {
        //                    if (
        //                        item.Type == (int)Sushi.Mediakiwi.Framework.ContentType.Binary_Image
        //                        || item.Type == (int)Sushi.Mediakiwi.Framework.ContentType.Hyperlink
        //                        || item.Type == (int)Sushi.Mediakiwi.Framework.ContentType.Binary_Document
        //                        || item.Type == (int)Sushi.Mediakiwi.Framework.ContentType.PageSelect
        //                        || item.Type == (int)Sushi.Mediakiwi.Framework.ContentType.FolderSelect
        //                        || item.Type == (int)Sushi.Mediakiwi.Framework.ContentType.Choice_Dropdown
        //                        )
        //                    {
        //                        if (item.Value == "0")
        //                            continue;
        //                    }


        //                    contentHash.Append(item.Value);
        //                }
        //            }
        //        }
        //    }
        //    // first backup the current version
        //    var pvOld = new Sushi.Mediakiwi.Data.PageVersion();
        //    pvOld.ContentXML = Wim.Utility.GetSerialized(targetComponents); ;
        //    pvOld.MetaDataXML = Wim.Utility.GetSerialized(this);
        //    pvOld.UserID = user.ID;
        //    pvOld.PageID = ID;
        //    pvOld.TemplateID = TemplateID;
        //    pvOld.IsArchived = false;
        //    pvOld.Name = Name;
        //    pvOld.CompletePath = CompletePath;
        //    pvOld.Hash = Wim.Utility.HashString(contentHash.ToString());
        //    pvOld.Save();
        //    Sushi.Mediakiwi.Framework2.Functions.AuditTrail.Insert(user, this, Framework2.Functions.Auditing.ActionType.Update, pvOld.ID);
        //}

        ///// <summary>
        ///// Copies component content and page meta data (CustomDate, Description, Expiration, Keywords, LinkText, Name and Title) from the pageversion
        ///// </summary>
        ///// <param name="pageVersion"></param>
        ///// <param name="user">The user who is executing the operation</param>
        //internal void CopyFromVersion(IPageVersion pageVersion, IApplicationUser user)
        //{
        //    var targetComponents = Sushi.Mediakiwi.Data.ComponentVersion.SelectAll(ID);
        //    SaveThisVersion(targetComponents, user);
        //    foreach (var c in targetComponents)
        //    {
        //        c.Delete();
        //    }
        //    var sourceComponents = Wim.Utility.GetDeserialized(typeof(Data.ComponentVersion[]), pageVersion.ContentXML) as Data.ComponentVersion[];
        //    foreach (var c in sourceComponents)
        //    {
        //        var component = new Sushi.Mediakiwi.Data.ComponentVersion();
        //        Wim.Utility.ReflectProperty(c, component);
        //        c.ID = 0;
        //        c.GUID = Guid.NewGuid();
        //        c.PageID = ID;
        //        c.Save(false);
        //    }
        //    var sourcePage = Wim.Utility.GetDeserialized(typeof(Data.Page), pageVersion.MetaDataXML) as Sushi.Mediakiwi.Data.Page;
        //    this.CustomDate = sourcePage.CustomDate;
        //    this.Description = sourcePage.Description;
        //    this.Expiration = sourcePage.Expiration;
        //    this.Keywords = sourcePage.Keywords;
        //    this.LinkText = sourcePage.LinkText;
        //    this.Name = sourcePage.Name;
        //    this.Title = sourcePage.Title;
        //    this.Save();
        //    Wim.Utilities.CacheItemManager.FlushIndexOfCacheObjects(string.Concat("Data_", this.GetType().ToString()));
        //}

        ///// <summary>
        ///// Copies the page content from the given sourcepage to the current page as the target
        ///// </summary>
        ///// <param name="sourcePage">The page to copy content from</param>
        ///// <param name="user">The user who is executing the operation</param>
        ///// <returns>True; when copying was success, False or exception when it wasn't</returns>
        //public bool OverridePageContentFromPage(Page sourcePage, IApplicationUser user)
        //{
        //    if (sourcePage.TemplateID != this.TemplateID) throw new ArgumentException("The sourcePage must match it's templateID with the target page");
        //    var targetComponents = Sushi.Mediakiwi.Data.ComponentVersion.SelectAll(ID);

        //    SaveThisVersion(targetComponents, user);
        //    foreach (var c in targetComponents)
        //    {
        //        c.Delete();
        //    }
        //    var sourceComponents = Sushi.Mediakiwi.Data.ComponentVersion.SelectAll(sourcePage.ID);
        //    foreach (var c in sourceComponents)
        //    {
        //        var component = new Sushi.Mediakiwi.Data.ComponentVersion();
        //        Wim.Utility.ReflectProperty(c, component);
        //        RecreateLinksInComponentForCopy(c, null);
        //        c.ID = 0;
        //        c.GUID = Guid.NewGuid();
        //        c.PageID = ID;
        //        c.Save(false);
        //    }
        //    Wim.Utilities.CacheItemManager.FlushIndexOfCacheObjects(string.Concat("Data_", this.GetType().ToString()));
        //    this.Save();
        //    return true;
        //}

        //public void RecreateLinksInComponentForCopy(ComponentVersion component, Dictionary<int, Sushi.Mediakiwi.Data.Folder> oldNewFolderMapping = null,
        //    List<Sushi.Mediakiwi.Data.Link> pageLinks = null)
        //{
        //    var fieldList = new List<Sushi.Mediakiwi.Data.Content.Field>();
        //    var currentContent = component.GetContent();
        //    if (currentContent != null && currentContent.Fields != null)
        //    {
        //        foreach (var field in currentContent.Fields)
        //        {
        //            if (field.Type == (int)ContentType.FolderSelect && oldNewFolderMapping != null)
        //            {
        //                int oldFolderView = Wim.Utility.ConvertToInt(field.Value, 0);
        //                if (oldFolderView > 0 && oldNewFolderMapping.ContainsKey(oldFolderView))
        //                    field.Value = oldNewFolderMapping[oldFolderView].ToString();
        //            }


        //            if (field.Type == (int)ContentType.Hyperlink)
        //            {
        //                var link = field.Link;
        //                if (link.ID > 0)
        //                {
        //                    var newlink = new Sushi.Mediakiwi.Data.Link();
        //                    Wim.Utility.ReflectProperty(link, newlink);
        //                    newlink.ID = 0;
        //                    newlink.GUID = Guid.NewGuid();
        //                    newlink.Save(false);
        //                    if (pageLinks != null && newlink.PageID.HasValue)
        //                        pageLinks.Add(newlink);
        //                    field.Value = newlink.ID.ToString();
        //                }
        //            }
        //            if (field.Type == (int)ContentType.RichText)
        //            {
        //                var result = MatchAndReplaceTextForLinks(field.Value, pageLinks);
        //                field.Value = result;
        //            }
        //            fieldList.Add(field);
        //        }

        //        var content = new Sushi.Mediakiwi.Data.Content();
        //        content.Fields = fieldList.ToArray();
        //        component.Serialized_XML = Data.Content.GetSerialized(content);
        //    }
        //}


        //Regex getWimLinks = new Regex(@"""wim:(.*?)""", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        //private string MatchAndReplaceTextForLinks(string value, List<Sushi.Mediakiwi.Data.Link> pageLinks = null)
        //{
        //    if (value != null)
        //    {
        //        return getWimLinks.Replace(value, delegate (Match match)
        //        {
        //            if (match.Groups.Count > 1)
        //            {
        //                string v = match.Groups[1].Value;
        //                var link = Sushi.Mediakiwi.Data.Link.SelectOne(Wim.Utility.ConvertToInt(v));
        //                if (link.ID > 0)
        //                {
        //                    var newlink = new Sushi.Mediakiwi.Data.Link();
        //                    Wim.Utility.ReflectProperty(link, newlink);
        //                    newlink.ID = 0;
        //                    newlink.GUID = Guid.NewGuid();
        //                    newlink.Save(false);
        //                    if (pageLinks != null && newlink.PageID.HasValue)
        //                        pageLinks.Add(newlink);
        //                    return $@"""wim:{newlink.ID.ToString()}""";
        //                }
        //                else
        //                    return value;
        //            }
        //            else
        //                return value;
        //        });
        //    }
        //    return null;
        //}

        #endregion MOVED to EXTENSION / LOGIC

        #region REPLICATED TO Sushi.Mediakiwi.Data.Standard

        /// <summary>
        /// Initializes a new instance of the <see cref="Page"/> class.
        /// </summary>
        public Page()
        {
            m_Folder = new Folder();
            m_PageTemplate = new PageTemplate();
        }

        private int m_ID;
        /// <summary>
        /// Identifier of the page
        /// </summary>
        /// <value>The ID.</value>
        [DatabaseColumn("Page_Key", SqlDbType.Int, IsPrimaryKey = true, CollectionLevel = DatabaseColumnGroup.Minimal)]
        public int ID
        {
            get { return m_ID; }
            set { m_ID = value; }
        }

        internal Guid m_GUID;
        /// <summary>
        /// Unique identifier used for import/export
        /// </summary>
        /// <value>The GUID.</value>
        [DatabaseColumn("Page_GUID", SqlDbType.UniqueIdentifier, IsMigrationKey = true)]
        public Guid GUID
        {
            get
            {
                if (this.m_GUID == Guid.Empty) this.m_GUID = Guid.NewGuid();
                return m_GUID;
            }
            set { m_GUID = value; }
        }

        private string m_Name;
        /// <summary>
        /// Name of the page
        /// </summary>
        [DatabaseColumn("Page_Name", SqlDbType.NVarChar, Length = 150)]
        public string Name
        {
            get { return m_Name; }
            set { m_Name = value; }
        }

        private string m_LinkText;
        /// <summary>
        /// If a page is linked through <i>Page.Apply(...)</i> this text will be shown.
        /// </summary>
        [DatabaseColumn("Page_LinkText", SqlDbType.NVarChar, Length = 150)]
        public string LinkText
        {
            get { return m_LinkText; }
            set { m_LinkText = value; }
        }

        private string m_Title;
        /// <summary>
        /// The title of the page
        /// </summary>
        [DatabaseColumn("Page_Title", SqlDbType.NVarChar, Length = 250, IsNullable = true)]
        public string Title
        {
            get { return m_Title; }
            set { m_Title = value; }
        }

        private string m_Keywords;
        /// <summary>
        /// Descriptive text
        /// </summary>
        [DatabaseColumn("Page_KeyWords", SqlDbType.NVarChar, Length = 500, IsNullable = true)]
        public string Keywords
        {
            get { return m_Keywords; }
            set { m_Keywords = value; }
        }

        private string m_Description;
        /// <summary>
        /// Descriptive text
        /// </summary>
        [DatabaseColumn("Page_Description", SqlDbType.NVarChar, Length = 500, IsNullable = true)]
        public string Description
        {
            get { return m_Description; }
            set { m_Description = value; }
        }

        /// <summary>
        /// The identifier of the folder the page is nested in.
        /// </summary>
        /// <value>The folder ID.</value>
        [DatabaseColumn("Page_Folder_Key", SqlDbType.Int)]
        public int FolderID { get; set; }

        [DatabaseColumn("Folder_Folder_Key", SqlDbType.Int, IsOnlyRead = true)]
        public int ParentFolderID { get; set; }


        private int m_SubFolderID;
        /// <summary>
        /// Gets or sets the sub folder id.
        /// </summary>
        /// <value>The sub folder id.</value>
        [DatabaseColumn("Page_SubFolder_Key", SqlDbType.Int)]
        public int SubFolderID
        {
            get { return m_SubFolderID; }
            set { m_SubFolderID = value; }
        }

        private int m_TemplateID;
        /// <summary>
        /// The identifier of the pagetemplate used by this page.
        /// </summary>
        [DatabaseColumn("Page_Template_Key", SqlDbType.Int)]
        public int TemplateID
        {
            get { return m_TemplateID; }
            set { m_TemplateID = value; }
        }

        private int? m_MasterID;
        /// <summary>
        /// The identifier of the page whom is the master of the this page.
        /// </summary>
        [DatabaseColumn("Page_Master_Key", SqlDbType.Int, IsNullable = true)]
        public int? MasterID
        {
            get { return m_MasterID; }
            set { m_MasterID = value; }
        }

        private DateTime m_Created;
        /// <summary>
        /// The creation date of this page.
        /// </summary>
        [DatabaseColumn("Page_Created", SqlDbType.DateTime)]
        public DateTime Created
        {
            get
            {
                if (this.m_Created == DateTime.MinValue) this.m_Created = Sushi.Mediakiwi.Data.Common.DatabaseDateTime;
                return m_Created;
            }
            set { m_Created = value; }
        }

        private DateTime m_Published;
        /// <summary>
        /// The publication date of this page.
        /// </summary>
        /// <value>The published.</value>
        [DatabaseColumn("Page_Published", SqlDbType.DateTime, IsNullable = true)]
        public DateTime Published
        {
            get { return m_Published; }
            set { m_Published = value; }
        }

        private DateTime m_Updated;
        /// <summary>
        /// The last update date of this page.
        /// </summary>
        /// <value>The updated.</value>
        [DatabaseColumn("Page_Updated", SqlDbType.DateTime)]
        public DateTime Updated
        {
            get
            {
                if (this.m_Updated == DateTime.MinValue) this.m_Updated = Sushi.Mediakiwi.Data.Common.DatabaseDateTime;
                return m_Updated;
            }
            set { m_Updated = value; }
        }

        private bool m_InheritContent;
        /// <summary>
        /// Does this page inherit content.
        /// </summary>
        [DatabaseColumn("Page_InheritContent", SqlDbType.Bit)]
        public bool InheritContent
        {
            get { return m_InheritContent; }
            set { m_InheritContent = value; }
        }

        /// <summary>
        /// Does this page inherit content in edited mode
        /// </summary>
        /// <value>
        /// The inherit content edited.
        /// </value>
        [DatabaseColumn("Page_InheritContentEdited", SqlDbType.Bit, IsNullable = true)]
        public bool InheritContentEdited { get; set; }

        private bool m_InheritPublicationInfo;
        /// <summary>
        /// Gets or sets a value indicating whether [inherit publication info].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [inherit publication info]; otherwise, <c>false</c>.
        /// </value>
        [DatabaseColumn("Site_AutoPublishInherited", SqlDbType.Bit, IsOnlyRead = true)]
        public bool InheritPublicationInfo
        {
            get { return m_InheritPublicationInfo; }
            set { m_InheritPublicationInfo = value; }
        }

        private bool m_IsSearchable;
        /// <summary>
        /// The possibility to search the content of this template 
        /// </summary>
        [DatabaseColumn("Page_IsSearchable", SqlDbType.Bit)]
        public bool IsSearchable
        {
            get { return m_IsSearchable; }
            set { m_IsSearchable = value; }
        }

        private bool m_IsFixed;
        /// <summary>
        /// Is this page fixed (can not be removed)?
        /// </summary>
        [DatabaseColumn("Page_IsFixed", SqlDbType.Bit)]
        public bool IsFixed
        {
            get { return m_IsFixed; }
            set { m_IsFixed = value; }
        }

        private bool m_IsPublished;
        /// <summary>
        /// Is this page currently published?
        /// </summary>
        [DatabaseColumn("Page_IsPublished", SqlDbType.Bit)]
        public bool IsPublished
        {
            get { return m_IsPublished; }
            set { m_IsPublished = value; }
        }

        private bool m_IsEdited;
        /// <summary>
        /// Is this page currently edited?
        /// </summary>
        //[DatabaseColumn("Page_IsEdited", SqlDbType.Bit)]
        public bool IsEdited
        {
            get { return (this.Published.Ticks != this.Updated.Ticks); }
            //set { m_IsEdited = value; }
        }

        private bool m_AddToOutputCache;
        /// <summary>
        /// Gets or sets a value indicating whether [add to output cache].
        /// </summary>
        /// <value><c>true</c> if [add to output cache]; otherwise, <c>false</c>.</value>
        [DatabaseColumn("PageTemplate_AddToOutputCache", SqlDbType.Bit, IsOnlyRead = true, CollectionLevel = DatabaseColumnGroup.Minimal)]
        public bool AddToOutputCache
        {
            get { return m_AddToOutputCache; }
            set { m_AddToOutputCache = value; }
        }

        private bool m_IsFolderDefault;
        /// <summary>
        /// Is this page the folder default?
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is folder default; otherwise, <c>false</c>.
        /// </value>
        [DatabaseColumn("Page_IsDefault", SqlDbType.Bit)]
        public bool IsFolderDefault
        {
            get { return m_IsFolderDefault; }
            set { m_IsFolderDefault = value; }
        }

        private bool m_IsSecure;
        /// <summary>
        /// Gets or sets a value indicating whether this instance is secure.
        /// </summary>
        /// <value><c>true</c> if this instance is secure; otherwise, <c>false</c>.</value>
        [DatabaseColumn("Page_IsSecure", SqlDbType.Bit, IsNullable = true)]
        public bool IsSecure
        {
            get { return m_IsSecure; }
            set { m_IsSecure = value; }
        }


        private string m_SitePath;
        /// <summary>
        /// The complete page path including the possible application path
        /// </summary>
        /// <value>The complete path.</value>
        [DatabaseColumn("Site_DefaultFolder", SqlDbType.NVarChar, Length = 500, IsNullable = true, IsOnlyRead = true)]
        public string SitePath
        {
            get { return m_SitePath; }
            set { m_SitePath = value; }
        }

        private string m_InternalPath;
        /// <summary>
        /// The complete page path including the possible application path
        /// </summary>
        /// <value>The complete path.</value>
        [DatabaseColumn("Page_CompletePath", SqlDbType.NVarChar, Length = 500, IsNullable = true)]
        public string InternalPath
        {
            get { return m_InternalPath; }
            set
            {
                m_InternalPath = value;
            }
        }

        private DateTime m_Publication;
        /// <summary>
        /// Publication date of this page.
        /// </summary>
        /// <value>The publication.</value>
        [DatabaseColumn("Page_Publish", SqlDbType.DateTime, IsNullable = true)]
        public DateTime Publication
        {
            get { return m_Publication; }
            set { m_Publication = value; }
        }

        private DateTime m_Expiration;
        /// <summary>
        /// Expiration date of this page.
        /// </summary>
        /// <value>The expiration.</value>
        [DatabaseColumn("Page_Expire", SqlDbType.DateTime, IsNullable = true)]
        public DateTime Expiration
        {
            get { return m_Expiration; }
            set { m_Expiration = value; }
        }

        private DateTime? m_CustomDate;
        /// <summary>
        /// CustomDate date of this page.
        /// </summary>
        /// <value>The custom date.</value>
        [DatabaseColumn("Page_CustomDate", SqlDbType.DateTime, IsNullable = true)]
        public DateTime? CustomDate
        {
            get { return m_CustomDate; }
            set { m_CustomDate = value; }
        }

        private Sushi.Mediakiwi.Data.PageTemplate m_PageTemplate;
        /// <summary>
        /// The corresponding page template
        /// </summary>
        /// <value>The template.</value>
        //[DatabaseEntityAttribute(CollectionLevel = DatabaseColumnGroup.Default)]
        [PolutionIgnore()]
        public Sushi.Mediakiwi.Data.PageTemplate Template
        {
            get
            {

                if (m_PageTemplate != null && m_PageTemplate.IsNewInstance && TemplateID > 0)
                {
                    m_PageTemplate = PageTemplate.SelectOne(TemplateID);
                    if (m_PageTemplate != null && m_PageTemplate.ID != TemplateID)
                        m_PageTemplate = PageTemplate.SelectOne(TemplateID);

                    //CB - 15-09-2014: Try to find the overwriten variant 
                    var overwriteTemplate = Sushi.Mediakiwi.Data.PageTemplate.SelectOneOverwrite(SiteID, TemplateID);
                    // if overwrite template found, then we use that one
                    if (overwriteTemplate != null && overwriteTemplate.ID > 0)
                        m_PageTemplate = overwriteTemplate;

                }
                return m_PageTemplate;
            }
            set { m_PageTemplate = value; }
        }

        private Sushi.Mediakiwi.Data.Folder m_Folder;
        /// <summary>
        /// The corresponding folder
        /// </summary>
        /// <value>The folder.</value>
        //[DatabaseEntityAttribute(CollectionLevel = DatabaseColumnGroup.Default)]
        public Sushi.Mediakiwi.Data.Folder Folder
        {
            get
            {
                if (m_Folder != null && (m_Folder.IsNewInstance || m_Folder.ID != FolderID))
                    m_Folder = Sushi.Mediakiwi.Data.Folder.SelectOne(FolderID);
                return m_Folder;
            }
            set { m_Folder = value; }
        }

        private int m_SiteID;
        /// <summary>
        /// The indentifier of the site where this page is situated.
        /// </summary>
        /// <value>The site ID.</value>
        [DatabaseColumn("Site_Key", SqlDbType.Int, IsOnlyRead = true)]
        public int SiteID
        {
            get { return m_SiteID; }
            set { m_SiteID = value; }
        }

        private Sushi.Mediakiwi.Data.Site m_Site;
        /// <summary>
        /// The corresponding folder
        /// </summary>
        /// <value>The site.</value>
        public Sushi.Mediakiwi.Data.Site Site
        {
            get
            {
                if (m_Site == null) m_Site = Sushi.Mediakiwi.Data.Site.SelectOne(SiteID);
                return m_Site;
            }
        }


        DateTime? iExportable.Updated
        {
            get
            {
                return Updated;
            }
        }


        bool m_IsPageFullyCachable;
        bool m_IsPageFullyCachableSet;
        /// <summary>
        /// Determines whether [is page fully cachable].
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if [is page fully cachable]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsPageFullyCachable()
        {
            if (!this.AddToOutputCache)
                return false;

            if (!m_IsPageFullyCachableSet)
            {
                int count = Convert.ToInt32(Execute(string.Concat("select COUNT(*) from wim_ComponentTemplates join wim_AvailableTemplates on AvailableTemplates_ComponentTemplate_Key = ComponentTemplate_Key where ComponentTemplate_CacheLevel = 0 and AvailableTemplates_PageTemplate_Key = ", this.TemplateID)));
                m_IsPageFullyCachable = (count == 0);
                m_IsPageFullyCachableSet = true;
            }
            return m_IsPageFullyCachable;
        }


        /// <summary>
        /// Selects all.
        /// </summary>
        /// <returns></returns>
        public static Page[] SelectAll()
        {
            List<Page> list = new List<Page>();
            Page page = new Page();

            page.SqlOrder = "Page_Master_Key ASC";
            if (!string.IsNullOrEmpty(SqlConnectionString2)) page.SqlConnectionString = SqlConnectionString2;
            foreach (object o in page._SelectAll(true))
                list.Add((Page)o);
            return list.ToArray();
        }

        /// <summary>
        /// Selects all children.
        /// </summary>
        /// <param name="masterID">The master ID.</param>
        /// <returns></returns>
        internal static Page[] SelectAllChildren(int masterID)
        {
            List<Page> list = new List<Page>();
            Page page = new Page();

            List<DatabaseDataValueColumn> where = new List<DatabaseDataValueColumn>();
            where.Add(new DatabaseDataValueColumn("page_Master_Key", SqlDbType.Int, masterID));
            foreach (object o in page._SelectAll(where)) list.Add((Page)o);
            return list.ToArray();
        }

        /// <summary>
        /// Update an implementation record.
        /// </summary>
        /// <returns></returns>
        public override bool Update()
        {
            return base.Update();
        }

        internal void SetCompletePath()
        {
            string replacement = Data.Environment.Current["SPACE_REPLACEMENT"];
            var path = this.Folder.CompletePath;
            if (path == null)
                path = "/";
            if (!path.EndsWith("/"))
                path += "/";
            this.InternalPath = string.Concat(this.SitePath, path, this.Name).Replace(" ", replacement);
        }



        /// <summary>
        /// Save a database entity. This method will indentify the need for an INSERT or an UPDATE.
        /// </summary>
        /// <returns></returns>
        public override bool Save()
        {
            SetCompletePath();

            bool isSaved = base.Save(false);
            Execute(string.Format(@"
update wim_Pages
set	
	Page_IsPublished = (select a.Page_IsPublished from wim_Pages a where a.Page_Key = wim_Pages.Page_Master_Key)
,	Page_CustomDate = (select a.Page_CustomDate from wim_Pages a where a.Page_Key = wim_Pages.Page_Master_Key)
where
	Page_Master_Key = {0}
    and Page_InheritContent = 1
    and Page_Folder_Key in (
		select Folder_Key from wim_Folders join wim_Sites on Folder_Site_Key = Site_Key and Site_AutoPublishInherited = 1
)"
                , this.ID));

            return isSaved;
        }

        /// <summary>
        /// Insert an implementation record.
        /// </summary>
        /// <returns></returns>
        public override bool Insert()
        {
            if (this.IsNewInstance)
            {
                string name = this.Name;
                this.Name = GetPageNameProposal(this.FolderID, this.Name);
                if (!this.Name.Equals(name))
                {
                    this.InternalPath = $"{this.InternalPath.Substring(0, this.InternalPath.Length - name.Length)}{this.Name}";
                }
            }

            return base.Insert();
        }

        /// <summary>
        /// Delete an implementation record.
        /// </summary>
        /// <returns></returns>
        public override bool Delete()
        {
            Execute(string.Format(@"DELETE from wim_componentVersions where ComponentVersion_Page_Key in (select Page_Key from wim_pages where (Page_Key = {0} or Page_Master_Key = {0}))", this.ID));
            Execute(string.Format(@"DELETE from wim_components where Component_Page_Key in (select Page_Key from wim_pages where (Page_Key = {0}or Page_Master_Key = {0}))", this.ID));
            Execute(string.Format(@"DELETE from wim_pages where (Page_Key = {0} or Page_Master_Key = {0})", this.ID));
            return true;
        }
        /// <summary>
        /// Save a database entity based on the Migration key.
        /// </summary>
        /// <param name="pageGUID">The page GUID.</param>
        /// <returns></returns>
        public override bool Save(Guid pageGUID)
        {
            return base.Save(pageGUID);
        }


        /// <summary>
        /// Combine two page array's and sort them
        /// </summary>
        /// <param name="pageArray1">The page array1.</param>
        /// <param name="pageArray2">The page array2.</param>
        /// <param name="sortby">How to sort</param>
        /// <returns></returns>
        public List<Page> Combine(List<Page> pageArray1, List<Page> pageArray2, PageSortBy sortby)
        {
            SortedList<string, Page> pageList = new SortedList<string, Page>();
            m_count = 0;
            if (pageArray1 != null)
            {
                foreach (Page page in pageArray1)
                    pageList.Add(GetSortField(page, sortby), page);
            }
            if (pageArray2 != null)
            {
                foreach (Page page in pageArray2)
                    pageList.Add(GetSortField(page, sortby), page);
            }
            List<Page> list = new List<Page>();

            foreach (KeyValuePair<string, Page> item in pageList)
                list.Add(item.Value);

            return list;
        }

        /// <summary>
        /// Combine two page array's and sort them
        /// </summary>
        /// <param name="pageArray1">The page array1.</param>
        /// <param name="pageArray2">The page array2.</param>
        /// <param name="sortby">How to sort</param>
        /// <returns></returns>
        public Sushi.Mediakiwi.Data.Page[] Combine(Sushi.Mediakiwi.Data.Page[] pageArray1, Sushi.Mediakiwi.Data.Page[] pageArray2, PageSortBy sortby)
        {
            SortedList<string, Page> pageList = new SortedList<string, Page>();
            m_count = 0;
            if (pageArray1 != null)
            {
                foreach (Page page in pageArray1)
                    pageList.Add(GetSortField(page, sortby), page);
            }
            if (pageArray2 != null)
            {
                foreach (Page page in pageArray2)
                    pageList.Add(GetSortField(page, sortby), page);
            }
            List<Page> list = new List<Page>();

            foreach (KeyValuePair<string, Page> item in pageList)
                list.Add(item.Value);

            return list.ToArray();
        }

        private int m_count;
        /// <summary>
        /// Gets the sort field.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="sortby">The sortby.</param>
        /// <returns></returns>
        string GetSortField(Sushi.Mediakiwi.Data.Page page, PageSortBy sortby)
        {
            m_count++;
            if (sortby == PageSortBy.Name)
                return string.Format("{0}{1}", page.Name, m_count);

            if (sortby == PageSortBy.LinkText)
                return string.Format("{0}{1}", page.LinkText, m_count);

            return string.Format("{0}{1}", page.CustomDate.GetValueOrDefault(page.Published).Ticks, m_count);
        }

        /// <summary>
        /// Select a page entity (returns only published page)
        /// </summary>
        /// <param name="ID">The ID.</param>
        /// <returns>Page entity</returns>
        public static Sushi.Mediakiwi.Data.Page SelectOne(int ID)
        {
            return SelectOne(ID, true);
        }

        /// <summary>
        /// Validates the access right.
        /// </summary>
        /// <param name="pages">The pages.</param>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        public static Sushi.Mediakiwi.Data.Page[] ValidateAccessRight(Sushi.Mediakiwi.Data.Page[] pages, Sushi.Mediakiwi.Data.IApplicationUser user)
        {
            return (from item in pages join relation in Sushi.Mediakiwi.Data.Folder.SelectAllAccessible(user) on item.FolderID equals relation.ID select item).ToArray();
        }

        /// <summary>
        /// Updates the sort order.
        /// </summary>
        /// <param name="ID">The ID.</param>
        /// <param name="sortOrder">The sort order.</param>
        /// <returns></returns>
        public static bool UpdateSortOrder(int ID, int sortOrder)
        {
            Page tmp = new Page();
            tmp.Execute(string.Format("update wim_Pages set Page_SortOrder = {0} where Page_Key = {1}", sortOrder, ID));
            return true;
        }

        /// <summary>
        /// Selects the one_ by sub folder.
        /// </summary>
        /// <param name="subFolderID">The sub folder ID.</param>
        /// <param name="returnOnlyPublishedPage">if set to <c>true</c> [return only published page].</param>
        /// <returns></returns>
        public static Sushi.Mediakiwi.Data.Page SelectOneBySubFolder(int subFolderID, bool returnOnlyPublishedPage)
        {
            List<DatabaseDataValueColumn> where = new List<DatabaseDataValueColumn>();
            where.Add(new DatabaseDataValueColumn("Page_SubFolder_Key", SqlDbType.Int, subFolderID));
            if (returnOnlyPublishedPage)
                where.Add(new DatabaseDataValueColumn("Page_IsPublished", SqlDbType.Bit, returnOnlyPublishedPage));

            Page page = new Page();
            page.SqlOrder = "Page_SortOrder ASC";
            page = (Page)page._SelectOne(where, "subfolder", $"{subFolderID}_{returnOnlyPublishedPage}");
            return page;
        }

        /// <summary>
        /// Select a page entity
        /// </summary>
        /// <param name="pageID">The page ID.</param>
        /// <param name="returnOnlyPublishedPage">Only return published pages. Additional feature is when False is applied the output is not cached.</param>
        /// <returns>Page entity</returns>
        public static Sushi.Mediakiwi.Data.Page SelectOne(int pageID, bool returnOnlyPublishedPage)
        {
            List<DatabaseDataValueColumn> where = new List<DatabaseDataValueColumn>();
            where.Add(new DatabaseDataValueColumn("Page_Key", SqlDbType.Int, pageID));
            if (returnOnlyPublishedPage)
                where.Add(new DatabaseDataValueColumn("Page_IsPublished", SqlDbType.Bit, returnOnlyPublishedPage));

            return (Page)new Page()._SelectOne(where, "ID", pageID.ToString());
            //return (Page)new Page()._SelectOne(where, "ID", pageID.ToString());
        }

        /// <summary>
        /// Selects the name of the one based on.
        /// </summary>
        /// <param name="pageName">Name of the page.</param>
        /// <param name="returnOnlyPublishedPage">if set to <c>true</c> [return only published page].</param>
        /// <returns></returns>
        public static Sushi.Mediakiwi.Data.Page SelectOneBasedOnName(string pageName, bool returnOnlyPublishedPage)
        {
            List<DatabaseDataValueColumn> where = new List<DatabaseDataValueColumn>();

            where.Add(new DatabaseDataValueColumn("Page_CompletePath", SqlDbType.NVarChar, string.Concat("%/", pageName), 500, DatabaseDataValueCompareType.Like));

            if (returnOnlyPublishedPage)
                where.Add(new DatabaseDataValueColumn("Page_IsPublished", SqlDbType.Bit, returnOnlyPublishedPage));

            return (Page)new Page()._SelectOne(where, "PageOnName", $"{pageName}_{returnOnlyPublishedPage}");
        }

        /// <summary>
        /// Select a page inherited child (returns only published pages)
        /// </summary>
        /// <param name="pageID">The page ID.</param>
        /// <param name="siteID">The site ID.</param>
        /// <returns></returns>
        public static Sushi.Mediakiwi.Data.Page SelectOneChild(int pageID, int siteID)
        {
            return SelectOneChild(pageID, siteID, true);
        }

        /// <summary>
        /// Select a page inherited child
        /// </summary>
        /// <param name="pageID">The page ID.</param>
        /// <param name="siteID">The site ID.</param>
        /// <param name="returnOnlyPublishedPage">Only return published pages</param>
        /// <returns></returns>
        public static Sushi.Mediakiwi.Data.Page SelectOneChild(int pageID, int siteID, bool returnOnlyPublishedPage)
        {
            using (Wim.Utilities.CacheItemManager cman = new Wim.Utilities.CacheItemManager())
            {
                string ckey = string.Concat("cp", pageID, "_", siteID);
                if (cman.IsCached(ckey))
                {
                    return cman.GetItem(ckey) as Page;
                }

                List<DatabaseDataValueColumn> where = new List<DatabaseDataValueColumn>();
                where.Add(new DatabaseDataValueColumn("Page_Key", SqlDbType.Int, pageID));
                where.Add(new DatabaseDataValueColumn("Page_Master_Key", SqlDbType.Int, pageID, DatabaseDataValueConnectType.Or));
                where.Add(new DatabaseDataValueColumn("Folder_Site_Key", SqlDbType.Int, siteID));
                if (returnOnlyPublishedPage)
                    where.Add(new DatabaseDataValueColumn("Page_IsPublished", SqlDbType.Bit, returnOnlyPublishedPage));

                Page page = new Page();
                page = (Page)page._SelectOne(where);
                if (page.IsNewInstance) return page;

                cman.AddSliding(ckey, page, Common.EntitySlidingExpiration, null);
                return page;
            }
        }

        /// <summary>
        /// Select the default folder page
        /// </summary>
        /// <param name="folderID">The folder ID.</param>
        /// <returns></returns>
        public static Sushi.Mediakiwi.Data.Page SelectOneDefault(int folderID)
        {
            return SelectOneDefault(folderID, true);
        }

        /// <summary>
        /// Select the default folder page
        /// </summary>
        /// <param name="folderID">The folder ID.</param>
        /// <param name="returnOnlyPublishedPage">Should this return only published pages?</param>
        /// <returns></returns>
        public static Sushi.Mediakiwi.Data.Page SelectOneDefault(int folderID, bool returnOnlyPublishedPage)
        {
            List<DatabaseDataValueColumn> where = new List<DatabaseDataValueColumn>();
            where.Add(new DatabaseDataValueColumn("Page_Folder_Key", SqlDbType.Int, folderID));
            where.Add(new DatabaseDataValueColumn("Page_IsDefault", SqlDbType.Bit, true));

            if (returnOnlyPublishedPage)
                where.Add(new DatabaseDataValueColumn("Page_IsPublished", SqlDbType.Bit, returnOnlyPublishedPage));

            Page page = new Page();
            return (Page)page._SelectOne(where, "DefaultPage", folderID.ToString());
        }

        /// <summary>
        /// Updates the default.
        /// </summary>
        /// <param name="pageID">The page ID.</param>
        /// <param name="folderID">The folder ID.</param>
        /// <returns>
        /// The indentifier of the newly inserted page
        /// </returns>
        public static void UpdateDefault(int pageID, int folderID)
        {
            using (Connection.DataCommander dac = new Connection.DataCommander(Sushi.Mediakiwi.Data.Common.DatabaseConnectionString))
            {
                dac.Text = "update wim_Pages set Page_IsDefault = 0 where Page_Folder_Key = @Page_Folder_Key and not Page_Key = @Page_Key";
                dac.SetParameterInput("@Page_Key", pageID, SqlDbType.Int);
                dac.SetParameterInput("@Page_Folder_Key", folderID, SqlDbType.Int);
                dac.ExecNonQuery();
            }
        }

        /// <summary>
        /// Page names should always be unique within the system. When a pagename is existing this method can be
        /// called to suggest a new prososed page name (internally also used for Page.Insert and Page.Update).
        /// When the pagename doesn't exist the input name is returned.
        /// </summary>
        /// <param name="folderID">The folder ID.</param>
        /// <param name="page">Page name</param>
        /// <returns></returns>
        public string GetPageNameProposal(int folderID, string page)
        {
            string nameProposal = Wim.Utility.GlobalRegularExpression.Implement.ReplaceNotAcceptableFilenameCharacter.Replace(page, "");

            bool pageExistsInFolder = IsPageAlreadyTaken(folderID, page);
            int nameExtentionCount = 0;

            while (pageExistsInFolder)
            {
                nameExtentionCount++;
                nameProposal = string.Format("{0}_{1}", page, nameExtentionCount);
                pageExistsInFolder = IsPageAlreadyTaken(folderID, nameProposal);
            }
            return nameProposal;
        }

        /// <summary>
        /// Verifies the existance of a particular page(name) in a folder.
        /// </summary>
        /// <param name="folderID">The folder ID.</param>
        /// <param name="page">Page name</param>
        /// <returns>
        /// 	<c>true</c> if [is page already taken] [the specified folder key]; otherwise, <c>false</c>.
        /// </returns>
        bool IsPageAlreadyTaken(int folderID, string page)
        {
            using (Connection.DataCommander dac = new Connection.DataCommander(this.SqlConnectionString))
            {
                dac.Text = "select count(*) from wim_pages where page_Folder_Key = @Folder_Key and Page_Name = @Page_Name";
                dac.SetParameterInput("@Folder_Key", folderID, SqlDbType.Int);
                dac.SetParameterInput("@Page_Name", page, SqlDbType.NVarChar, 50);
                int count = (int)dac.ExecScalar();
                if (count == 0)
                    return false;
                return true;
            }
        }

        /// <summary>
        /// Deletes all component search references.
        /// </summary>
        /// <param name="pageID">The page ID.</param>
        internal static void DeleteAllComponentSearchReferences(int pageID)
        {
            Page candidate = new Page();
            string sqlText = string.Format(@"
    delete from wim_ComponentSearch 
    where 
        ComponentSearch_Type = 1 
        and ComponentSearch_Ref_Key in (
            select 
                Component_Key 
            from 
                wim_Components 
            where 
                Component_Page_Key = {0})
", pageID);
            candidate.Execute(sqlText);
        }


        /// <summary>
        /// Selects all.
        /// </summary>
        /// <param name="searchQuery">The search query.</param>
        /// <param name="isPartOfPath">if set to <c>true</c> [is part of path].</param>
        /// <returns></returns>
        public static Page[] SelectAll(string searchQuery, bool isPartOfPath)
        {
            if (string.IsNullOrEmpty(searchQuery)) return new Page[0];

            string query = string.Format("%{0}%", searchQuery.Replace(" ", "%"));

            Page candidate = new Page();
            List<Page> list = new List<Page>();
            List<DatabaseDataValueColumn> where = new List<DatabaseDataValueColumn>();
            where.Add(new DatabaseDataValueColumn("Page_Name", SqlDbType.VarChar, query, DatabaseDataValueCompareType.Like));
            where.Add(new DatabaseDataValueColumn("Page_LinkText", SqlDbType.VarChar, query, DatabaseDataValueCompareType.Like, DatabaseDataValueConnectType.Or));
            where.Add(new DatabaseDataValueColumn("Page_Description", SqlDbType.VarChar, query, DatabaseDataValueCompareType.Like, DatabaseDataValueConnectType.Or));
            if (isPartOfPath)
                where.Add(new DatabaseDataValueColumn("Page_CompletePath", SqlDbType.VarChar, query, DatabaseDataValueCompareType.Like, DatabaseDataValueConnectType.Or));

            foreach (object o in candidate._SelectAll(where))
                list.Add((Page)o);

            return list.ToArray();
        }

        /// <summary>
        /// Selects all.
        /// </summary>
        /// <param name="keylist">The keylist.</param>
        /// <returns></returns>
        public static Page[] SelectAll(int[] keylist)
        {
            List<Page> list = new List<Page>();
            Page page = new Page();

            List<DatabaseDataValueColumn> where = new List<DatabaseDataValueColumn>();
            where.Add(new DatabaseDataValueColumn("Page_Key", SqlDbType.Int, Wim.Utility.ConvertToCsvString(keylist), DatabaseDataValueCompareType.In));

            foreach (object o in page._SelectAll(where))
                list.Add((Page)o);
            return list.ToArray();
        }


        /// <summary>
        /// Selects all based on page template.
        /// </summary>
        /// <param name="pageTemplateID">The page template ID.</param>
        /// <returns></returns>
        public static Page[] SelectAllBasedOnPageTemplate(int pageTemplateID)
        {
            List<Page> list = new List<Page>();
            Page page = new Page();

            List<DatabaseDataValueColumn> where = new List<DatabaseDataValueColumn>();
            where.Add(new DatabaseDataValueColumn("Page_Template_Key", SqlDbType.Int, pageTemplateID));

            foreach (object o in page._SelectAll(where))
                list.Add((Page)o);
            return list.ToArray();
        }

        /// <summary>
        /// Selects all based on page template.
        /// </summary>
        /// <param name="pageTemplateArray">The page template array.</param>
        /// <returns></returns>
        public static Page[] SelectAllBasedOnPageTemplate(int[] pageTemplateArray)
        {
            List<Page> list = new List<Page>();
            Page page = new Page();

            List<DatabaseDataValueColumn> where = new List<DatabaseDataValueColumn>();
            where.Add(new DatabaseDataValueColumn("Page_Template_Key", SqlDbType.Int, Wim.Utility.ConvertToCsvString(pageTemplateArray), DatabaseDataValueCompareType.In));

            foreach (object o in page._SelectAll(where))
                list.Add((Page)o);
            return list.ToArray();
        }

        /// <summary>
        /// Select all pages in a site (unregarding their state)
        /// </summary>
        /// <param name="siteID">The site ID.</param>
        /// <returns></returns>
        public static Page[] SelectAllBySite(int siteID)
        {
            List<Page> list = new List<Page>();
            Page page = new Page();

            List<DatabaseDataValueColumn> where = new List<DatabaseDataValueColumn>();
            where.Add(new DatabaseDataValueColumn("Folder_Site_Key", SqlDbType.Int, siteID));

            foreach (object o in page._SelectAll(where))
                list.Add((Page)o);
            return list.ToArray();
        }

        /// <summary>
        /// Select all pages present in designated folder which al published and having a custom date field attached to it which is applied
        /// Default: Sorted DESCENDING
        /// </summary>
        /// <param name="folderID">The folder ID.</param>
        /// <returns></returns>
        public static Page[] SelectAllByCustomDate(int folderID)
        {
            return SelectAllByCustomDate(folderID, null, true, 0);
        }

        /// <summary>
        /// Select all pages present in designated folder which al published and having a custom date field attached to it which is applied
        /// Default: Sorted DESCENDING
        /// </summary>
        /// <param name="folderID">The folder ID.</param>
        /// <param name="pageTemplateID">The page template ID.</param>
        /// <returns></returns>
        public static Page[] SelectAllByCustomDate(int folderID, int? pageTemplateID)
        {
            return SelectAllByCustomDate(folderID, pageTemplateID, true, 0);
        }

        /// <summary>
        /// Select all pages present in designated folder which al published and having a custom date field attached to it which is applied
        /// </summary>
        /// <param name="folderID">The folder ID.</param>
        /// <param name="pageTemplateID">The page template ID.</param>
        /// <param name="desc">sort order is descending?</param>
        /// <returns></returns>
        public static Page[] SelectAllByCustomDate(int folderID, int? pageTemplateID, bool desc)
        {
            return SelectAllByCustomDate(folderID, pageTemplateID, desc, 0);
        }

        /// <summary>
        /// Select all pages present in designated folder which al published and having a custom date field attached to it which is applied
        /// </summary>
        /// <param name="folderID">The folder ID.</param>
        /// <param name="pageTemplateID">The page template ID.</param>
        /// <param name="desc">sort order is descending?</param>
        /// <param name="maxReturnCount">Maximum number of pages to return</param>
        /// <returns></returns>
        public static Page[] SelectAllByCustomDate(int folderID, int? pageTemplateID, bool desc, int maxReturnCount)
        {
            List<Page> list = new List<Page>();
            Page page = new Page();

            if (maxReturnCount != 0)
                page.SqlRowCount = maxReturnCount;

            //if (callingPage.InheritContent)
            //    page.SqlJoin = "join wim_Folders on Page_Folder_Key = Folder_Key join wim_Components on Component_Page_Key = Page_Master_Key join wim_PageTemplates on Page_Template_Key = PageTemplate_Key join wim_Sites on Folder_Site_Key = Site_Key";
            //else
            //    page.SqlJoin = "join wim_Folders on Page_Folder_Key = Folder_Key join wim_Components on Component_Page_Key = Page_Key join wim_PageTemplates on Page_Template_Key = PageTemplate_Key join wim_Sites on Folder_Site_Key = Site_Key";

            if (desc)
                page.SqlOrder = "Page_CustomDate DESC";
            else
                page.SqlOrder = "Page_CustomDate ASC";

            List<DatabaseDataValueColumn> where = new List<DatabaseDataValueColumn>();
            where.Add(new DatabaseDataValueColumn("Folder_Key", SqlDbType.Int, folderID));
            where.Add(new DatabaseDataValueColumn("Page_IsPublished", SqlDbType.Bit, true));
            where.Add(new DatabaseDataValueColumn("not Page_CustomDate", SqlDbType.DateTime, null));

            where.Add(new DatabaseDataValueColumn("Page_Publish", SqlDbType.DateTime, null, 0));
            where.Add(new DatabaseDataValueColumn("Page_Publish", SqlDbType.DateTime, DateTime.Now, DatabaseDataValueCompareType.SmallerThen, DatabaseDataValueConnectType.Or));
            where.Add(new DatabaseDataValueColumn("Page_Expire", SqlDbType.DateTime, null));
            where.Add(new DatabaseDataValueColumn("Page_Expire", SqlDbType.DateTime, DateTime.Now, DatabaseDataValueCompareType.BiggerThen, DatabaseDataValueConnectType.Or));

            if (pageTemplateID.HasValue)
                where.Add(new DatabaseDataValueColumn("Page_Template_Key", SqlDbType.Int, pageTemplateID));

            foreach (object o in page._SelectAll(where))
                list.Add((Page)o);
            return list.ToArray();
        }

        /// <summary>
        /// Selects all dated.
        /// </summary>
        /// <returns></returns>
        public static Page[] SelectAllDated()
        {
            List<Page> list = new List<Page>();
            Page page = new Page();

            List<DatabaseDataValueColumn> where = new List<DatabaseDataValueColumn>();
            where.Add(new DatabaseDataValueColumn("not Page_Publish", SqlDbType.DateTime, null));
            where.Add(new DatabaseDataValueColumn("not Page_Expire", SqlDbType.DateTime, null, DatabaseDataValueConnectType.Or));
            where.Add(new DatabaseDataValueColumn("Page_Publish", SqlDbType.DateTime, DateTime.Now, DatabaseDataValueCompareType.SmallerThenOrEquals));
            where.Add(new DatabaseDataValueColumn("Page_Expire", SqlDbType.DateTime, DateTime.Now, DatabaseDataValueCompareType.SmallerThenOrEquals, DatabaseDataValueConnectType.Or));

            foreach (object o in page._SelectAll(where))
                list.Add((Page)o);
            return list.ToArray();
        }


        #endregion REPLICATED TO Sushi.Mediakiwi.Data.Standard
    }
}
