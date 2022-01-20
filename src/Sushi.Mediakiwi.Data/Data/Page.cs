using Sushi.Mediakiwi.Data.MicroORM;
using Sushi.MicroORM;
using Sushi.MicroORM.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Data
{
    /// <summary>
    /// Represents a Page entity.
    /// </summary>
    [DataMap(typeof(PageMap))]
    public class Page : IExportable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Page"/> class.
        /// </summary>
        public Page()
        {
            Folder = new Folder();
            m_PageTemplate = new PageTemplate();
        }

        public class PageMap : DataMap<Page>
        {
            public PageMap() : this(false) { }
                        
            public PageMap(bool isSave)
            {
                if (isSave)
                {
                    Table("wim_Pages");
                }
                else
                {
                    Table("wim_Pages left join wim_Folders on Page_Folder_Key = Folder_Key left join wim_Sites on Site_Key = Folder_Site_Key");
                }

                Id(x => x.ID, "Page_Key").Identity();
                Map(x => x.GUID, "Page_GUID");
                Map(x => x.Name, "Page_Name").Length(150);
                Map(x => x.LinkText, "Page_LinkText").Length(150);
                Map(x => x.Title, "Page_Title").Length(250);
                Map(x => x.Keywords, "Page_KeyWords").Length(500);
                Map(x => x.Description, "Page_Description").Length(500);
                Map(x => x.FolderID, "Page_Folder_Key");
                
                Map(x => x.SubFolderID, "Page_SubFolder_Key");
                Map(x => x.TemplateID, "Page_Template_Key");
                Map(x => x.MasterID, "Page_Master_Key");
                Map(x => x.Created, "Page_Created");
                Map(x => x.Published, "Page_Published");
                Map(x => x.Updated, "Page_Updated");
                Map(x => x.InheritContent, "Page_InheritContent");
                Map(x => x.IsLocalized, "Page_InheritContentEdited");
                Map(x => x.IsSearchable, "Page_IsSearchable");
                Map(x => x.IsFixed, "Page_IsFixed");
                Map(x => x.IsPublished, "Page_IsPublished");

                Map(x => x.IsFolderDefault, "Page_IsDefault");
                Map(x => x.IsSecure, "Page_IsSecure");
                Map(x => x.Publication, "Page_Publish");
                Map(x => x.Expiration, "Page_Expire");
                Map(x => x.CustomDate, "Page_CustomDate");
                Map(x => x.InternalPath, "Page_CompletePath").Length(500);
                Map(x => x.SortOrder, "Page_SortOrder");

                // Joins from wim_site :
                Map(x => x.InheritPublicationInfo, "Site_AutoPublishInherited").ReadOnly();
                Map(x => x.SiteID, "Site_Key").ReadOnly();
                Map(x => x.SitePath, "Site_DefaultFolder").Length(500).ReadOnly();

                // Joins from wim_folder :
                Map(x => x.FolderSiteID, "Folder_Site_Key").ReadOnly();
                Map(x => x.ParentFolderID, "Folder_Folder_Key").ReadOnly();

            }
        }

        /// <summary>
        /// Returns all pages that inherit from this page
        /// </summary>
        /// <returns></returns>
        public async Task<List<Page>> GetInheritedPages()
        {
            var connector = ConnectorFactory.CreateConnector<Page>();
            var filter = connector.CreateQuery();
            filter.Add(x => x.MasterID, ID);
            return await connector.FetchAllAsync(filter);
        }

        /// <summary>
        /// Are there any pages that inherited from this page ?
        /// </summary>
        /// <returns></returns>
        public async Task<bool> HasInheritedPagesAsync()
        {
            var inheritedPages = await GetInheritedPages().ConfigureAwait(false);
            return inheritedPages.Any();
        }

        internal static async Task<List<Page>> SelectAllUninheritedAsync(int masterID, int siteID)
        {
            var connector = ConnectorFactory.CreateConnector<Page>();
            var filter = connector.CreateQuery();
            filter.AddParameter("@siteId", siteID);
            filter.Add(x => x.FolderSiteID, masterID);
            filter.AddSql("(select COUNT(*) from wim_Pages pages left join wim_Folders on pages.Page_Folder_Key = Folder_Key where Folder_Site_Key = @siteId and pages.Page_Master_Key = wim_Pages.Page_Key) = 0");
            
            return await connector.FetchAllAsync(filter);
        }

        private Regex getWimLinks = new Regex(@"""wim:(.*?)""", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        /// <summary>
        /// Identifier of the page
        /// </summary>
        /// <value>The ID.</value>
        public int ID { get; set; }

        internal Guid m_GUID;

        /// <summary>
        /// Unique identifier used for import/export
        /// </summary>
        /// <value>The GUID.</value>
        public Guid GUID
        {
            get
            {
                if (m_GUID == Guid.Empty)
                {
                    m_GUID = Guid.NewGuid();
                }
                return m_GUID;
            }
            set { m_GUID = value; }
        }

        /// <summary>
        /// Name of the page
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// If a page is linked through <i>Page.Apply(...)</i> this text will be shown.
        /// </summary>
        public string LinkText { get; set; }

        /// <summary>
        /// The title of the page
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Descriptive text
        /// </summary>
        public string Keywords { get; set; }

        /// <summary>
        /// Descriptive text
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The identifier of the folder the page is nested in.
        /// </summary>
        /// <value>The folder ID.</value>
        public int FolderID { get; set; }

        public int ParentFolderID { get; set; }

        /// <summary>
        /// Gets or sets the sub folder id.
        /// </summary>
        /// <value>The sub folder id.</value>
        public int SubFolderID { get; set; }

        /// <summary>
        /// The identifier of the pagetemplate used by this page.
        /// </summary>
        public int TemplateID { get; set; }

        /// <summary>
        /// The identifier of the page whom is the master of the this page.
        /// </summary>
        public int? MasterID { get; set; }

        private DateTime m_Created;

        /// <summary>
        /// The creation date of this page.
        /// </summary>
        public DateTime Created
        {
            get
            {
                if (m_Created == DateTime.MinValue)
                {
                    m_Created = Common.DatabaseDateTime;
                }
                return m_Created;
            }
            set { m_Created = value; }
        }

        /// <summary>
        /// The publication date of this page.
        /// </summary>
        /// <value>The published.</value>
        public DateTime? Published { get; set; }

        private DateTime m_Updated;

        /// <summary>
        /// The last update date of this page.
        /// </summary>
        /// <value>The updated.</value>
        public DateTime Updated
        {
            get
            {
                if (m_Updated == DateTime.MinValue)
                {
                    m_Updated = Common.DatabaseDateTime;
                }
                return m_Updated;
            }
            set { m_Updated = value; }
        }

        /// <summary>
        /// Does this page inherit content.
        /// </summary>
        public bool InheritContent { get; set; }

        /// <summary>
        /// Is this inherited page localized, so decoupled from it's parent content
        /// </summary>
        /// <value>
        /// When an inherited page is Localized, this is set to <c>true</c>
        /// </value>
        public bool IsLocalized { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [inherit publication info].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [inherit publication info]; otherwise, <c>false</c>.
        /// </value>
        public bool InheritPublicationInfo { get; set; }

        /// <summary>
        /// The possibility to search the content of this template
        /// </summary>
        public bool IsSearchable { get; set; }

        /// <summary>
        /// Is this page fixed (can not be removed)?
        /// </summary>
        public bool IsFixed { get; set; }

        /// <summary>
        /// Is this page currently published?
        /// </summary>
        public bool IsPublished { get; set; }

        /// <summary>
        /// Is this page currently edited?
        /// </summary>
        //[DatabaseColumn("Page_IsEdited", SqlDbType.Bit)]
        public bool IsEdited
        {
            get
            {
                return (Published.GetValueOrDefault().Ticks != Updated.Ticks);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [add to output cache].
        /// </summary>
        /// <value><c>true</c> if [add to output cache]; otherwise, <c>false</c>.</value>
        public bool AddToOutputCache
        {
            get 
            {
                if (Template?.ID > 0)
                {
                    return Template.IsAddedOutputCache;
                }
                return false;
            }
        }

        /// <summary>
        /// Is this page the folder default?
        /// </summary>
        public bool IsFolderDefault { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is secure.
        /// </summary>
        /// <value><c>true</c> if this instance is secure; otherwise, <c>false</c>.</value>
        public bool IsSecure { get; set; }

        /// <summary>
        /// Publication date of this page.
        /// </summary>
        /// <value>The publication.</value>
        public DateTime? Publication { get; set; }

        /// <summary>
        /// Expiration date of this page.
        /// </summary>
        /// <value>The expiration.</value>
        public DateTime? Expiration { get; set; }

        /// <summary>
        /// CustomDate date of this page.
        /// </summary>
        /// <value>The custom date.</value>
        public DateTime? CustomDate { get; set; }

        private PageTemplate m_PageTemplate;

        /// <summary>
        /// The corresponding page template
        /// </summary>
        /// <value>The template.</value>
        public PageTemplate Template
        {
            get
            {
                //[MR:03-01-2020] klopt deze ID check wel ?? ziet er gruwlijk uit namelijk
                if (m_PageTemplate != null && m_PageTemplate.ID == 0 && TemplateID > 0)
                {
                    m_PageTemplate = PageTemplate.SelectOne(TemplateID);
                    if (m_PageTemplate != null && m_PageTemplate.ID != TemplateID)
                    {
                        m_PageTemplate = PageTemplate.SelectOne(TemplateID);
                    }

                    //CB - 15-09-2014: Try to find the overwriten variant
                    var overwriteTemplate = PageTemplate.SelectOneOverwrite(SiteID, TemplateID);
                    // if overwrite template found, then we use that one
                    if (overwriteTemplate != null && overwriteTemplate.ID > 0)
                    {
                        m_PageTemplate = overwriteTemplate;
                    }
                }
                return m_PageTemplate;
            }
            set { m_PageTemplate = value; }
        }

        private Folder m_Folder;
        /// <summary>
        /// The corresponding folder
        /// </summary>
        /// <value>The folder.</value>
        //[DatabaseEntityAttribute(CollectionLevel = DatabaseColumnGroup.Default)]
        public Folder Folder
        {
            get
            {
                if (m_Folder != null && (m_Folder.IsNewInstance || m_Folder.ID != FolderID))
                {
                    m_Folder = Folder.SelectOne(FolderID);
                }
                return m_Folder;
            }
            set { m_Folder = value; }
        }

        /// <summary>
        /// The indentifier of the site where this page is situated.
        /// </summary>
        /// <value>The site ID.</value>
        public int SiteID { get; set; }

        public int? FolderSiteID { get; set; }

        private Site m_Site;

        /// <summary>
        /// The corresponding folder
        /// </summary>
        /// <value>The site.</value>
        public Site Site
        {
            get
            {
                if (m_Site == null)
                {
                    m_Site = Site.SelectOne(SiteID);
                }
                return m_Site;
            }
            set { m_Site = value; }
        }

        public string HRef
        {
            get
            {
                return CompletePath;
            }

        }

        /// <summary>
        /// The complete page URL
        /// </summary>
        /// <value>The full Href to this page.</value>
        public string HRefFull
        {
            get
            {
                if (string.IsNullOrEmpty(Name))
                    return null;

                var domain = "https://www.website.com";

                if (Site != null && !string.IsNullOrWhiteSpace(Site.Domain))
                {
                    domain = Site.Domain;
                }
                else
                {
                    // Create a parent site collection
                    Site parentSiteWithDomain = default;


                    var site = Site;
                    while (site.MasterID.GetValueOrDefault(0) > 0)
                    {
                        site = site.MasterImplement;
                        if (site?.ID > 0 && string.IsNullOrWhiteSpace(site.Domain) == false)
                        {
                            parentSiteWithDomain = site;
                            break;
                        }
                    }

                    if (parentSiteWithDomain?.ID > 0)
                    {
                        domain = parentSiteWithDomain.Domain;
                    }
                }

                return $"{domain}{CompletePath}";

                //string ext = Sushi.Mediakiwi.Data.Environment.Current.GetRegistryValue("PAGE_WILDCARD_EXTENTION", "aspx");
                //ext = (ext == ".") ? "" : string.Concat(".", ext);

                //if (CompletePath.StartsWith(HttpContext.Current.Request.Url.Scheme))
                //    return string.Concat(CompletePath, ext);


                //string[] urlArr = Http`.Current.Request.Url.AbsoluteUri.Split('/');
                ////  Creates /www.url.ext
                //string rebuild = string.Format("{0}/{1}", urlArr[1], urlArr[2]);

                ////  Creates /www.url.ext/folder/file.ext
                //string tmp = string.Concat(rebuild, CompletePath, ext);

                //if (Wim.CommonConfiguration.REDIRECT_CHANNEL_PATH
                //    && !Wim.CommonConfiguration.IS_LOCAL_DEVELOPMENT
                //    && !string.IsNullOrEmpty(Site.Domain))
                //{
                //    tmp = string.Concat("/", Site.Domains[0], Wim.Utility.RemApplicationPath(CompletePath), ext);
                //}

                //string unEncoded = Utility.GlobalRegularExpression.Implement.CleanRelativePathSlash.Replace(tmp, "/");
                ////  Creates http://www.url.ext/folder/file.ext
                //return string.Format("{0}/{1}", urlArr[0], Utility.CleanUrl(unEncoded));
            }
        }

        /// <summary>
        /// The complete page path including the possible application path
        /// </summary>
        /// <value>The complete path.</value>
        public string InternalPath { get; set; }

        /// <summary>
        /// The complete page path including the possible application path
        /// </summary>
        /// <value>The complete path.</value>
        [Obsolete("This will return InternalPath, because CompletePath relied on an HttpContext which isn't available in dotnetcore")]
        public string CompletePath
        {
            get 
            {
                return InternalPath;
            }
        }

        /// <summary>
        /// The complete page path including the possible application path
        /// </summary>
        /// <value>The complete path.</value>
        public string SitePath { get; set; }

        /// <summary>
        /// The sortorder for this page
        /// </summary>
        public int SortOrder { get; set; }

        /// <summary>
        /// Selects all pages .
        /// </summary>
        /// <returns></returns>
        public static Page[] SelectAll()
        {
            var connector = ConnectorFactory.CreateConnector<Page>();
            var filter = connector.CreateQuery();
            filter.AddOrder(x => x.MasterID);

            return connector.FetchAll(filter).ToArray();
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
        public static List<Page> SelectAll(int folderID, PageFolderSortType folderType = PageFolderSortType.Folder, PageReturnProperySet propertySet = PageReturnProperySet.All, PageSortBy sort = PageSortBy.SortOrder, bool onlyReturnPublishedPages = false)
        {
            var connector = ConnectorFactory.CreateConnector<Page>();
            var filter = connector.CreateQuery();
            filter.AddOrder(x => x.MasterID);

            if (sort == PageSortBy.Name)
            {
                filter.AddOrder(x => x.Name, Sushi.MicroORM.SortOrder.ASC);
            }
            else if (sort == PageSortBy.LinkText)
            {
                filter.AddOrder(x => x.LinkText, Sushi.MicroORM.SortOrder.ASC);
            }
            else if (sort == PageSortBy.SortOrder)
            {
                filter.AddOrder(x => x.SortOrder, Sushi.MicroORM.SortOrder.ASC);
            }
            else if (sort == PageSortBy.CustomDate)
            {
                filter.AddOrder(x => x.CustomDate, Sushi.MicroORM.SortOrder.ASC);
                filter.AddOrder(x => x.SortOrder, Sushi.MicroORM.SortOrder.ASC);
            }
            else if (sort == PageSortBy.CustomDateDown)
            {
                filter.AddOrder(x => x.CustomDate, Sushi.MicroORM.SortOrder.DESC);
                filter.AddOrder(x => x.SortOrder, Sushi.MicroORM.SortOrder.ASC);
            }

            switch (propertySet)
            {
                case PageReturnProperySet.OnlyDefault:
                    {
                        filter.Add(x => x.IsFolderDefault, true);
                    }
                    break;
                case PageReturnProperySet.AllExceptDefault:
                    {
                        filter.Add(x => x.IsFolderDefault, false);
                    }
                    break;
            }

            if (onlyReturnPublishedPages)
            {
                filter.Add(x => x.IsPublished, true);
                filter.AddParameter("date", DateTime.UtcNow);
                filter.AddSql("(Page_Publish is null or Page_Publish <= @date and (Page_Expire is null or Page_Expire >= @seadaterch)");
            }
            filter.Add(x => x.FolderID, folderID);

            return connector.FetchAll(filter);
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
        public static async Task<List<Page>> SelectAllAsync(int folderID, PageFolderSortType folderType = PageFolderSortType.Folder, PageReturnProperySet propertySet = PageReturnProperySet.All, PageSortBy sort = PageSortBy.SortOrder, bool onlyReturnPublishedPages = false)
        {
            var connector = ConnectorFactory.CreateConnector<Page>();
            var filter = connector.CreateQuery();
            filter.AddOrder(x => x.MasterID);

            if (sort == PageSortBy.Name)
            {
                filter.AddOrder(x => x.Name, Sushi.MicroORM.SortOrder.ASC);
            }
            else if (sort == PageSortBy.LinkText)
            {
                filter.AddOrder(x => x.LinkText, Sushi.MicroORM.SortOrder.ASC);
            }
            else if (sort == PageSortBy.SortOrder)
            {
                filter.AddOrder(x => x.SortOrder, Sushi.MicroORM.SortOrder.ASC);
            }
            else if (sort == PageSortBy.CustomDate)
            {
                filter.AddOrder(x => x.CustomDate, Sushi.MicroORM.SortOrder.ASC);
                filter.AddOrder(x => x.SortOrder, Sushi.MicroORM.SortOrder.ASC);
            }
            else if (sort == PageSortBy.CustomDateDown)
            {
                filter.AddOrder(x => x.CustomDate, Sushi.MicroORM.SortOrder.DESC);
                filter.AddOrder(x => x.SortOrder, Sushi.MicroORM.SortOrder.ASC);
            }

            switch (propertySet)
            {
                case PageReturnProperySet.OnlyDefault:
                    {
                        filter.Add(x => x.IsFolderDefault, true);
                    }
                    break;
                case PageReturnProperySet.AllExceptDefault:
                    {
                        filter.Add(x => x.IsFolderDefault, false);
                    }
                    break;
            }

            if (onlyReturnPublishedPages)
            {
                filter.Add(x => x.IsPublished, true);
                filter.AddParameter("date", DateTime.UtcNow);
                filter.AddSql("(Page_Publish is null or Page_Publish <= @date and (Page_Expire is null or Page_Expire >= @seadaterch)");
            }
            filter.Add(x => x.FolderID, folderID);

            return await connector.FetchAllAsync(filter);
        }

        /// <summary>
        /// Selects all pages Async.
        /// </summary>
        /// <returns></returns>
        public static async Task<Page[]> SelectAllAsync()
        {
            var connector = ConnectorFactory.CreateConnector<Page>();
            var filter = connector.CreateQuery();
            filter.AddOrder(x => x.MasterID);

            var result = await connector.FetchAllAsync(filter);
            return result.ToArray();
        }

        /// <summary>
        /// Selects all pages Async.
        /// </summary>
        /// <returns></returns>
        public static async Task<Page[]> SelectAllAsync(DateTime lastModified)
        {
            var connector = ConnectorFactory.CreateConnector<Page>();
            var filter = connector.CreateQuery();
            filter.AddOrder(x => x.MasterID);
            filter.Add(x => x.Updated, lastModified, ComparisonOperator.GreaterThanOrEquals);

            var result = await connector.FetchAllAsync(filter);
            return result.ToArray();
        }

        /// <summary>
        /// Selects all children By master ID.
        /// </summary>
        /// <param name="masterID">The master ID.</param>
        /// <returns></returns>
        public static Page[] SelectAllChildren(int masterID)
        {
            var connector = ConnectorFactory.CreateConnector<Page>();
            var filter = connector.CreateQuery();
            filter.AddOrder(x => x.MasterID);
            filter.Add(x => x.MasterID, masterID);

            return connector.FetchAll(filter).ToArray();
        }

        /// <summary>
        /// Selects all children By master ID Async.
        /// </summary>
        /// <param name="masterID">The master ID.</param>
        /// <returns></returns>
        public static async Task<Page[]> SelectAllChildrenAsync(int masterID)
        {
            var connector = ConnectorFactory.CreateConnector<Page>();
            var filter = connector.CreateQuery();
            filter.AddOrder(x => x.MasterID);
            filter.Add(x => x.MasterID, masterID);

            var result = await connector.FetchAllAsync(filter);
            return result.ToArray();
        }

        /// <summary>
        /// Update an implementation record.
        /// </summary>
        /// <returns></returns>
        public bool Update()
        {
            var connector = ConnectorFactory.CreateConnector(new PageMap(true));
            try
            {
                connector.Update(this);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Update an implementation record Async.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> UpdateAsync()
        {
            var connector = ConnectorFactory.CreateConnector(new PageMap(true));
            try
            {
                await connector.UpdateAsync(this);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        internal void SetInternalPath()
        {
            string replacement = CommonConfiguration.SPACE_REPLACEMENT;
            var path = Folder.CompletePath;
            if (path == null)
            {
                path = "/";
            }

            if (!path.EndsWith("/", StringComparison.CurrentCulture))
            {
                path += "/";
            }

            InternalPath = string.Concat(SitePath, path, Name).Replace(" ", replacement);
        }

        private string MatchAndReplaceTextForLinks(string value, List<Link> pageLinks = null)
        {
            if (value != null)
            {
                return getWimLinks.Replace(value, delegate (Match match) {
                    if (match.Groups.Count > 1)
                    {
                        string v = match.Groups[1].Value;
                        var link = Link.SelectOne(Utility.ConvertToInt(v));
                        if (link.ID > 0)
                        {
                            var newlink = new Link();
                            Utility.ReflectProperty(link, newlink);
                            newlink.ID = 0;
                            newlink.GUID = Guid.NewGuid();
                            newlink.Save();

                            if (pageLinks != null && newlink.PageID.HasValue)
                            {
                                pageLinks.Add(newlink);
                            }
                            return $@"""wim:{newlink.ID.ToString()}""";
                        }
                        else
                        {
                            return value;
                        }
                    }
                    else
                    {
                        return value;
                    }
                });
            }
            return null;
        }

        /// <summary>
        /// Save a database entity.
        /// This method will indentify the need for an INSERT or an UPDATE.
        /// </summary>
        /// <returns></returns>
        public bool Save()
        {
            SetInternalPath();

            var connector = ConnectorFactory.CreateConnector(new PageMap(true));
            try
            {
                connector.Save(this);

                var filter = connector.CreateQuery();
                filter.AddParameter("@thisId", ID);

                connector.ExecuteNonQuery(@"
                UPDATE [wim_Pages]
                SET
	                [Page_IsPublished] = (SELECT a.[Page_IsPublished] FROM [wim_Pages] a WHERE a.[Page_Key] = [wim_Pages].[Page_Master_Key])
                ,	[Page_CustomDate] = (SELECT a.[Page_CustomDate] FROM [wim_Pages] a WHERE a.[Page_Key] = [wim_Pages].[Page_Master_Key])
                WHERE
	                [Page_Master_Key] = @thisId
                    AND [Page_InheritContent] = 1
                    AND [Page_Folder_Key] in (
		                SELECT [Folder_Key] FROM [wim_Folders] JOIN [wim_Sites] ON [Folder_Site_Key] = [Site_Key] AND [Site_AutoPublishInherited] = 1
                )", filter);
				connector.Cache?.FlushRegion(connector.CacheRegion);

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Save a database entity Async.
        /// This method will indentify the need for an INSERT or an UPDATE.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> SaveAsync()
        {
            SetInternalPath();

            var connector = ConnectorFactory.CreateConnector(new PageMap(true));
            try
            {
                await connector.SaveAsync(this);

                var filter = connector.CreateQuery();
                filter.AddParameter("@thisId", ID);

                await connector.ExecuteNonQueryAsync(@"
                UPDATE [wim_Pages]
                SET
	                [Page_IsPublished] = (SELECT a.[Page_IsPublished] FROM [wim_Pages] a WHERE a.[Page_Key] = [wim_Pages].[Page_Master_Key])
                ,	[Page_CustomDate] = (SELECT a.[Page_CustomDate] FROM [wim_Pages] a WHERE a.[Page_Key] = [wim_Pages].[Page_Master_Key])
                WHERE
	                [Page_Master_Key] = @thisId
                    AND [Page_InheritContent] = 1
                    AND [Page_Folder_Key] in (
		                SELECT [Folder_Key] FROM [wim_Folders] JOIN [wim_Sites] ON [Folder_Site_Key] = [Site_Key] AND [Site_AutoPublishInherited] = 1
                )", filter);
				connector.Cache?.FlushRegion(connector.CacheRegion);

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Insert an implementation record.
        /// </summary>
        /// <returns></returns>
        public bool Insert()
        {
            if (ID == 0)
            {
                string name = Name;
                Name = GetPageNameProposal(FolderID, Name);
                if (!Name.Equals(name))
                {
                    InternalPath = $"{InternalPath.Substring(0, InternalPath.Length - name.Length)}{Name}";
                }
            }

            try
            {
                var connector = ConnectorFactory.CreateConnector(new PageMap(true));
                connector.Insert(this);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Insert an implementation record.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> InsertAsync()
        {
            if (ID == 0)
            {
                string name = Name;
                Name = await GetPageNameProposalAsync(FolderID, Name);
                if (!Name.Equals(name))
                {
                    InternalPath = $"{InternalPath.Substring(0, InternalPath.Length - name.Length)}{Name}";
                }
            }

            try
            {
                var connector = ConnectorFactory.CreateConnector(new PageMap(true));
                await connector.InsertAsync(this);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Delete an implementation record.
        /// </summary>
        /// <returns></returns>
        public bool Delete()
        {
            var connector = ConnectorFactory.CreateConnector(new PageMap(true));
            var filter = connector.CreateQuery();
            filter.AddParameter("@thisId", ID);

            connector.ExecuteNonQuery(@"DELETE FROM [wim_componentVersions] WHERE [ComponentVersion_Page_Key] IN (SELECT [Page_Key] FROM [wim_pages] WHERE ([Page_Key] = @thisId OR [Page_Master_Key] = @thisId))", filter);
            connector.ExecuteNonQuery(@"DELETE FROM [wim_components] WHERE [Component_Page_Key] IN (SELECT [Page_Key] FROM [wim_pages] WHERE ([Page_Key] = @thisId OR [Page_Master_Key] = @thisId))", filter);
            connector.ExecuteNonQuery(@"DELETE FROM [wim_pages] WHERE ([Page_Key] = @thisId OR [Page_Master_Key] = @thisId)", filter);
            connector.Cache?.FlushRegion(connector.CacheRegion);
            return true;
        }

        /// <summary>
        /// Delete an implementation record.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> DeleteAsync()
        {
            var connector = ConnectorFactory.CreateConnector(new PageMap(true));
            var filter = connector.CreateQuery();
            filter.AddParameter("@thisId", ID);

            await connector.ExecuteNonQueryAsync(@"DELETE FROM [wim_componentVersions] WHERE [ComponentVersion_Page_Key] IN (SELECT [Page_Key] FROM [wim_pages] WHERE ([Page_Key] = @thisId OR [Page_Master_Key] = @thisId))", filter);
            await connector.ExecuteNonQueryAsync(@"DELETE FROM [wim_components] WHERE [Component_Page_Key] IN (SELECT [Page_Key] FROM [wim_pages] WHERE ([Page_Key] = @thisId OR [Page_Master_Key] = @thisId))", filter);
            await connector.ExecuteNonQueryAsync(@"DELETE FROM [wim_pages] WHERE ([Page_Key] = @thisId OR [Page_Master_Key] = @thisId)", filter);
            connector.Cache?.FlushRegion(connector.CacheRegion);
            return true;
        }

        private bool m_IsPageFullyCachable;
        private bool m_IsPageFullyCachableSet;

        /// <summary>
        /// Determines whether [is page fully cachable].
        /// </summary>
        public bool IsPageFullyCachable()
        {
            if (!AddToOutputCache)
            {
                return false;
            }

            if (!m_IsPageFullyCachableSet)
            {
                var connector = ConnectorFactory.CreateConnector<Page>();
                var filter = connector.CreateQuery();
                filter.AddParameter("@templateId", TemplateID);
                int count = connector.ExecuteScalar<int>("SELECT COUNT(*) FROM [wim_ComponentTemplates] JOIN [wim_AvailableTemplates] ON [AvailableTemplates_ComponentTemplate_Key] = [ComponentTemplate_Key] WHERE [ComponentTemplate_CacheLevel] = 0 AND [AvailableTemplates_PageTemplate_Key] = @templateId", filter);

                m_IsPageFullyCachable = (count == 0);
                m_IsPageFullyCachableSet = true;
            }

            return m_IsPageFullyCachable;
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
                {
                    pageList.Add(GetSortField(page, sortby), page);
                }
            }
            if (pageArray2 != null)
            {
                foreach (Page page in pageArray2)
                {
                    pageList.Add(GetSortField(page, sortby), page);
                }
            }
            List<Page> list = new List<Page>();

            foreach (KeyValuePair<string, Page> item in pageList)
            {
                list.Add(item.Value);
            }

            return list;
        }

        /// <summary>
        /// Combine two page array's and sort them
        /// </summary>
        /// <param name="pageArray1">The page array1.</param>
        /// <param name="pageArray2">The page array2.</param>
        /// <param name="sortby">How to sort</param>
        /// <returns></returns>
        public Page[] Combine(Page[] pageArray1, Page[] pageArray2, PageSortBy sortby)
        {
            SortedList<string, Page> pageList = new SortedList<string, Page>();
            m_count = 0;
            if (pageArray1 != null)
            {
                foreach (Page page in pageArray1)
                {
                    pageList.Add(GetSortField(page, sortby), page);
                }
            }
            if (pageArray2 != null)
            {
                foreach (Page page in pageArray2)
                {
                    pageList.Add(GetSortField(page, sortby), page);
                }
            }
            List<Page> list = new List<Page>();

            foreach (KeyValuePair<string, Page> item in pageList)
            {
                list.Add(item.Value);
            }

            return list.ToArray();
        }

        private int m_count;

        /// <summary>
        /// Gets the sort field.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="sortby">The sortby.</param>
        /// <returns></returns>
        private string GetSortField(Page page, PageSortBy sortby)
        {
            m_count++;
            if (sortby == PageSortBy.Name)
            {
                return string.Concat(page.Name, m_count);
            }

            if (sortby == PageSortBy.LinkText)
            {
                return string.Concat(page.LinkText, m_count);
            }

            return string.Format("{0}{1}", page.CustomDate.GetValueOrDefault(page.Published.GetValueOrDefault()).Ticks, m_count);
        }

        /// <summary>
        /// Select a page entity (returns only published page)
        /// </summary>
        /// <param name="ID">The ID.</param>
        /// <returns>Page entity</returns>
        public static Page SelectOne(int ID)
        {
            return SelectOne(ID, true);
        }

        /// <summary>
        /// Select a page entity by GUID
        /// </summary>
        /// <param name="guid">The GUID.</param>
        /// <param name="returnOnlyPublishedPage">If true, only published pages will be returned</param>
        /// <returns>Page entity</returns>
        public static Page SelectOne(Guid guid, bool returnOnlyPublishedPage = false)
        {
            var connector = ConnectorFactory.CreateConnector<Page>();
            var filter = connector.CreateQuery();
            filter.Add(x => x.GUID, guid);

            if (returnOnlyPublishedPage)
            {
                filter.Add(x => x.IsPublished, true);
            }

            return connector.FetchSingle(filter);
        }

        /// <summary>
        /// Select a page entity by GUID
        /// </summary>
        /// <param name="guid">The GUID.</param>
        /// <param name="returnOnlyPublishedPage">If true, only published pages will be returned</param>
        /// <returns>Page entity</returns>
        public static async Task<Page> SelectOneAsync(Guid guid, bool returnOnlyPublishedPage = false)
        {
            var connector = ConnectorFactory.CreateConnector<Page>();
            var filter = connector.CreateQuery();
            filter.Add(x => x.GUID, guid);

            if (returnOnlyPublishedPage)
            {
                filter.Add(x => x.IsPublished, true);
            }

            return await connector.FetchSingleAsync(filter);
        }

        /// <summary>
        /// Select a page entity (returns only published page)
        /// </summary>
        /// <param name="ID">The ID.</param>
        /// <returns>Page entity</returns>
        public static async Task<Page> SelectOneAsync(int ID)
        {
            return await SelectOneAsync(ID, true);
        }

        /// <summary>
        /// Validates the access right.
        /// </summary>
        /// <param name="pages">The pages.</param>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        public static IEnumerable<Page> ValidateAccessRight(IEnumerable<Page> pages, IApplicationUser user)
        {
            return (from item in pages join relation in Folder.SelectAllAccessible(user) on item.FolderID equals relation.ID select item).ToArray();
        }

        /// <summary>
        /// Validates the access right.
        /// </summary>
        /// <param name="pages">The pages.</param>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        public static async Task<IEnumerable<Page>> ValidateAccessRightAsync(IEnumerable<Page> pages, IApplicationUser user)
        {
            var allFolders = await Folder.SelectAllAccessibleAsync(user).ConfigureAwait(false);
            return (from item in pages join relation in allFolders on item.FolderID equals relation.ID select item).ToArray();
        }

        /// <summary>
        /// Updates the sort order.
        /// </summary>
        /// <param name="ID">The ID.</param>
        /// <param name="sortOrder">The sort order.</param>
        /// <returns></returns>
        public static bool UpdateSortOrder(int ID, int sortOrder)
        {
            var connector = ConnectorFactory.CreateConnector<Page>();
            var filter = connector.CreateQuery();
            filter.AddParameter("@sortOrder", sortOrder);
            filter.AddParameter("@thisId", ID);

            connector.ExecuteNonQuery("UPDATE [wim_Pages] SET [Page_SortOrder] = @sortOrder WHERE [Page_Key] = @thisId", filter);
			connector.Cache?.FlushRegion(connector.CacheRegion);
            return true;
        }

        /// <summary>
        /// Updates the sort order Async.
        /// </summary>
        /// <param name="ID">The ID.</param>
        /// <param name="sortOrder">The sort order.</param>
        /// <returns></returns>
        public static async Task<bool> UpdateSortOrderAsync(int ID, int sortOrder)
        {
            var connector = ConnectorFactory.CreateConnector<Page>();
            var filter = connector.CreateQuery();
            filter.AddParameter("@sortOrder", sortOrder);
            filter.AddParameter("@thisId", ID);

            await connector.ExecuteNonQueryAsync("UPDATE [wim_Pages] SET [Page_SortOrder] = @sortOrder WHERE [Page_Key] = @thisId", filter);
			connector.Cache?.FlushRegion(connector.CacheRegion);
            return true;
        }

        /// <summary>
        /// Selects the one_ by sub folder.
        /// </summary>
        /// <param name="subFolderID">The sub folder ID.</param>
        /// <param name="returnOnlyPublishedPage">if set to <c>true</c> [return only published page].</param>
        /// <returns></returns>
        public static Page SelectOneBySubFolder(int subFolderID, bool returnOnlyPublishedPage)
        {
            var connector = ConnectorFactory.CreateConnector<Page>();
            var filter = connector.CreateQuery();
            filter.AddOrder(x => x.SortOrder);
            filter.Add(x => x.SubFolderID, subFolderID);

            if (returnOnlyPublishedPage)
            {
                filter.Add(x => x.IsPublished, true);
            }

            return connector.FetchSingle(filter);
        }

        /// <summary>
        /// Selects the one_ by sub folder Async.
        /// </summary>
        /// <param name="subFolderID">The sub folder ID.</param>
        /// <param name="returnOnlyPublishedPage">if set to <c>true</c> [return only published page].</param>
        /// <returns></returns>
        public static async Task<Page> SelectOneBySubFolderAsync(int subFolderID, bool returnOnlyPublishedPage)
        {
            var connector = ConnectorFactory.CreateConnector<Page>();
            var filter = connector.CreateQuery();
            filter.AddOrder(x => x.SortOrder);
            filter.Add(x => x.SubFolderID, subFolderID);

            if (returnOnlyPublishedPage)
            {
                filter.Add(x => x.IsPublished, true);
            }

            return await connector.FetchSingleAsync(filter);
        }

        /// <summary>
        /// Select a page entity
        /// </summary>
        /// <param name="ID">The page ID.</param>
        /// <param name="returnOnlyPublishedPage">Only return published pages. Additional feature is when False is applied the output is not cached.</param>
        /// <returns>Page entity</returns>
        public static Page SelectOne(int ID, bool returnOnlyPublishedPage)
        {
            var connector = ConnectorFactory.CreateConnector<Page>();
            var filter = connector.CreateQuery();
            filter.AddOrder(x => x.SortOrder);
            filter.Add(x => x.ID, ID);

            if (returnOnlyPublishedPage)
            {
                filter.Add(x => x.IsPublished, true);
            }

            return connector.FetchSingle(filter);
        }

        /// <summary>
        /// Selects a page entity based on its path
        /// </summary>
        /// <param name="path">The path to look for</param>
        /// <param name="returnOnlyPublishedPage">Only returns published pages</param>
        /// <returns></returns>
        public static Page SelectOne(string path, bool returnOnlyPublishedPage)
        {
            return SelectOne(path, returnOnlyPublishedPage, null);
        }


        /// <summary>
        /// Selects a page entity based on its path
        /// </summary>
        /// <param name="path">The path to look for</param>
        /// <param name="returnOnlyPublishedPage">Only returns published pages</param>
        /// <param name="siteId">The Site ID to look in</param>
        /// <returns></returns>
        public static Page SelectOne(string path, bool returnOnlyPublishedPage, int? siteId)
        {
            var connector = ConnectorFactory.CreateConnector<Page>();
            var filter = connector.CreateQuery();
            filter.AddOrder(x => x.SortOrder);
            filter.Add(x => x.InternalPath, path);
            if (siteId.GetValueOrDefault(0) > 0)
            {
                filter.Add(x => x.SiteID, siteId.Value);
            }

            if (returnOnlyPublishedPage)
            {
                filter.Add(x => x.IsPublished, true);
            }

            return connector.FetchSingle(filter);
        }

        /// <summary>
        /// Selects a page entity based on its path
        /// </summary>
        /// <param name="path">The path to look for</param>
        /// <param name="returnOnlyPublishedPage">Only returns published pages</p
        public static async Task<Page> SelectOneAsync(string path, bool returnOnlyPublishedPage)
        {
            return await SelectOneAsync(path, returnOnlyPublishedPage, null);
        }

        /// <summary>
        /// Selects a page entity based on its path
        /// </summary>
        /// <param name="path">The path to look for</param>
        /// <param name="returnOnlyPublishedPage">Only returns published pages</param>
        /// <param name="siteId">The Site ID to look in</param>
        /// <returns></returns>
        public static async Task<Page> SelectOneAsync(string path, bool returnOnlyPublishedPage, int? siteId)
        {
            var connector = ConnectorFactory.CreateConnector<Page>();
            var filter = connector.CreateQuery();
            filter.AddOrder(x => x.SortOrder);
            filter.Add(x => x.InternalPath, path);

            if (siteId.GetValueOrDefault(0) > 0)
            {
                filter.Add(x => x.SiteID, siteId.Value);
            }

            if (returnOnlyPublishedPage)
            {
                filter.Add(x => x.IsPublished, true);
            }

            return await connector.FetchSingleAsync(filter);
        }

        /// <summary>
        /// Select a page entity Async
        /// </summary>
        /// <param name="ID">The page ID.</param>
        /// <param name="returnOnlyPublishedPage">Only return published pages. Additional feature is when False is applied the output is not cached.</param>
        /// <returns>Page entity</returns>
        public static async Task<Page> SelectOneAsync(int ID, bool returnOnlyPublishedPage)
        {
            var connector = ConnectorFactory.CreateConnector<Page>();
            var filter = connector.CreateQuery();
            filter.AddOrder(x => x.SortOrder);
            filter.Add(x => x.ID, ID);

            if (returnOnlyPublishedPage)
            {
                filter.Add(x => x.IsPublished, true);
            }

            return await connector.FetchSingleAsync(filter);
        }

        /// <summary>
        /// Selects the name of the one based on.
        /// </summary>
        /// <param name="pageName">Name of the page.</param>
        /// <param name="returnOnlyPublishedPage">if set to <c>true</c> [return only published page].</param>
        /// <returns></returns>
        public static Page SelectOneBasedOnName(string pageName, bool returnOnlyPublishedPage)
        {
            var connector = ConnectorFactory.CreateConnector<Page>();
            var filter = connector.CreateQuery();
            filter.Add(x => x.InternalPath, string.Concat("%/", pageName), ComparisonOperator.Like);

            if (returnOnlyPublishedPage)
            {
                filter.Add(x => x.IsPublished, true);
            }

            return connector.FetchSingle(filter);
        }

        /// <summary>
        /// Selects the name of the one based on.
        /// </summary>
        /// <param name="pageName">Name of the page.</param>
        /// <param name="returnOnlyPublishedPage">if set to <c>true</c> [return only published page].</param>
        /// <returns></returns>
        public static async Task<Page> SelectOneBasedOnNameAsync(string pageName, bool returnOnlyPublishedPage)
        {
            var connector = ConnectorFactory.CreateConnector<Page>();
            var filter = connector.CreateQuery();
            filter.Add(x => x.InternalPath, string.Concat("%/", pageName), ComparisonOperator.Like);

            if (returnOnlyPublishedPage)
            {
                filter.Add(x => x.IsPublished, true);
            }

            return await connector.FetchSingleAsync(filter);
        }

        /// <summary>
        /// Select the default folder page
        /// </summary>
        /// <param name="folderID">The folder ID.</param>
        /// <returns></returns>
        public static Page SelectOneDefault(int folderID)
        {
            return SelectOneDefault(folderID, true);
        }

        /// <summary>
        /// Select the default folder page
        /// </summary>
        /// <param name="folderID">The folder ID.</param>
        /// <param name="returnOnlyPublishedPage">Should this return only published pages?</param>
        /// <returns></returns>
        public static Page SelectOneDefault(int folderID, bool returnOnlyPublishedPage)
        {
            var connector = ConnectorFactory.CreateConnector<Page>();
            var filter = connector.CreateQuery();
            filter.Add(x => x.FolderID, folderID);
            filter.Add(x => x.IsFolderDefault, true);

            if (returnOnlyPublishedPage)
            {
                filter.Add(x => x.IsPublished, true);
            }

            return connector.FetchSingle(filter);
        }

        /// <summary>
        /// Select the default folder page Async
        /// </summary>
        /// <param name="folderID">The folder ID.</param>
        /// <param name="returnOnlyPublishedPage">Should this return only published pages?</param>
        /// <returns></returns>
        public static async Task<Page> SelectOneDefaultAsync(int folderID, bool returnOnlyPublishedPage)
        {
            var connector = ConnectorFactory.CreateConnector<Page>();
            var filter = connector.CreateQuery();
            filter.Add(x => x.FolderID, folderID);
            filter.Add(x => x.IsFolderDefault, true);

            if (returnOnlyPublishedPage)
            {
                filter.Add(x => x.IsPublished, true);
            }

            return await connector.FetchSingleAsync(filter);
        }

        /// <summary>
        /// Updates the default.
        /// </summary>
        /// <param name="ID">The page ID.</param>
        /// <param name="folderID">The folder ID.</param>
        /// <returns>
        /// The indentifier of the newly inserted page
        /// </returns>
        public static void UpdateDefault(int ID, int folderID)
        {
            var connector = ConnectorFactory.CreateConnector<Page>();
            var filter = connector.CreateQuery();
            filter.AddParameter("@pageId", ID);
            filter.AddParameter("@folderId", folderID);

            connector.ExecuteNonQuery("UPDATE [wim_Pages] SET [Page_IsDefault] = 0 WHERE [Page_Folder_Key] = @folderId AND NOT [Page_Key] = @pageId", filter);
            connector.Cache?.FlushRegion(connector.CacheRegion);
        }

        /// <summary>
        /// Updates the default async.
        /// </summary>
        /// <param name="ID">The page ID.</param>
        /// <param name="folderID">The folder ID.</param>
        /// <returns>
        /// The indentifier of the newly inserted page
        /// </returns>
        public static async Task UpdateDefaultAsync(int ID, int folderID)
        {
            var connector = ConnectorFactory.CreateConnector<Page>();
            var filter = connector.CreateQuery();
            filter.AddParameter("@pageId", ID);
            filter.AddParameter("@folderId", folderID);

            await connector.ExecuteNonQueryAsync("UPDATE [wim_Pages] SET [Page_IsDefault] = 0 WHERE [Page_Folder_Key] = @folderId AND NOT [Page_Key] = @pageId", filter);
            connector.Cache?.FlushRegion(connector.CacheRegion);
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
            string nameProposal = Utility.GlobalRegularExpression.Implement.ReplaceNotAcceptableFilenameCharacter.Replace(page, "");

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
        /// Page names should always be unique within the system. When a pagename is existing this method can be
        /// called to suggest a new prososed page name (internally also used for Page.Insert and Page.Update).
        /// When the pagename doesn't exist the input name is returned.
        /// </summary>
        /// <param name="folderID">The folder ID.</param>
        /// <param name="page">Page name</param>
        /// <returns></returns>
        public async Task<string> GetPageNameProposalAsync(int folderID, string page)
        {
            string nameProposal = Utility.GlobalRegularExpression.Implement.ReplaceNotAcceptableFilenameCharacter.Replace(page, "");

            bool pageExistsInFolder = await IsPageAlreadyTakenAsync(folderID, page);
            int nameExtentionCount = 0;

            while (pageExistsInFolder)
            {
                nameExtentionCount++;
                nameProposal = string.Format("{0}_{1}", page, nameExtentionCount);
                pageExistsInFolder = await IsPageAlreadyTakenAsync(folderID, nameProposal);
            }
            return nameProposal;
        }

        /// <summary>
        /// Verifies the existance of a particular page(name) in a folder.
        /// </summary>
        /// <param name="folderID">The folder ID.</param>
        /// <param name="page">Page name</param>
        public bool IsPageAlreadyTaken(int folderID, string page)
        {
            var connector = ConnectorFactory.CreateConnector<Page>();
            var filter = connector.CreateQuery();
            filter.AddParameter("@pageName", page);
            filter.AddParameter("@folderId", folderID);
            var count = connector.ExecuteScalar<int>("SELECT COUNT(*) FROM [wim_pages] WHERE [page_Folder_Key] = @folderId AND [Page_Name] = @pageName", filter);

            return (count > 0);
        }

         /// <summary>
        /// Verifies the existance of a particular page(name) in a folder.
        /// </summary>
        /// <param name="folderID">The folder ID.</param>
        /// <param name="page">Page name</param>
        public async Task<bool> IsPageAlreadyTakenAsync(int folderID, string page)
        {
            var connector = ConnectorFactory.CreateConnector<Page>();
            var filter = connector.CreateQuery();
            filter.AddParameter("@pageName", page);
            filter.AddParameter("@folderId", folderID);
            var count = await connector.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM [wim_pages] WHERE [page_Folder_Key] = @folderId AND [Page_Name] = @pageName", filter);

            return (count > 0);
        }

        /// <summary>
        /// Deletes all component search references.
        /// </summary>
        /// <param name="ID">The page ID.</param>
        public static async Task DeleteAllComponentSearchReferencesAsync(int ID)
        {
            var connector = ConnectorFactory.CreateConnector<Page>();
            var filter = connector.CreateQuery();
            filter.AddParameter("@pageId", ID);

            await connector.ExecuteNonQueryAsync(@"
                DELETE FROM [wim_ComponentSearch]
                WHERE
                        [ComponentSearch_Type] = 1
                    AND [ComponentSearch_Ref_Key] IN (
                        SELECT
                            [Component_Key]
                        FROM
                            [wim_Components]
                        WHERE
                            [Component_Page_Key] = @pageId)
            ", filter);
            connector.Cache?.FlushRegion(connector.CacheRegion);
        }

        /// <summary>
        /// Clean up the component list after take done because some residual components could stil be present.
        /// </summary>
        public async Task CleanUpAfterTakeDownAsync()
        {
            await DeleteAllComponentSearchReferencesAsync(ID);

            var connector = ConnectorFactory.CreateConnector<Page>();
            var filter = connector.CreateQuery();
            filter.AddParameter("@pageId", ID);

            await connector.ExecuteNonQueryAsync(@"DELETE FROM [wim_Components] WHERE [Component_Page_Key] = @pageId", filter);
            connector.Cache?.FlushRegion(connector.CacheRegion);
        }

        /// <summary>
        /// Selects all.
        /// </summary>
        /// <param name="searchQuery">The search query.</param>
        /// <param name="isPartOfPath">if set to <c>true</c> [is part of path].</param>
        /// <returns></returns>
        public static Page[] SelectAll(string searchQuery, bool isPartOfPath)
        {
            if (string.IsNullOrEmpty(searchQuery))
                return new Page[0];

            var connector = ConnectorFactory.CreateConnector<Page>();
            var filter = connector.CreateQuery();

            string query = string.Format("%{0}%", searchQuery.Replace(" ", "%"));
            filter.AddParameter("@query", query);
            if (isPartOfPath)
            {
                filter.AddSql("([Page_Name] LIKE @query OR [Page_LinkText] LIKE @query OR [Page_Description] LIKE @query OR [Page_CompletePath] LIKE @query)");
            }
            else
            {
                filter.AddSql("([Page_Name] LIKE @query OR [Page_LinkText] LIKE @query OR [Page_Description] LIKE @query)");
            }

            return connector.FetchAll(filter).ToArray();
        }

        /// <summary>
        /// Selects all.
        /// </summary>
        /// <param name="searchQuery">The search query.</param>
        /// <param name="isPartOfPath">if set to <c>true</c> [is part of path].</param>
        /// <returns></returns>
        public static async Task<Page[]> SelectAllAsync(string searchQuery, bool isPartOfPath)
        {
            if (string.IsNullOrEmpty(searchQuery))
                return new Page[0];

            var connector = ConnectorFactory.CreateConnector<Page>();
            var filter = connector.CreateQuery();

            string query = string.Format("%{0}%", searchQuery.Replace(" ", "%"));
            filter.AddParameter("@query", query);
            if (isPartOfPath)
            {
                filter.AddSql("([Page_Name] LIKE @query OR [Page_LinkText] LIKE @query OR [Page_Description] LIKE @query OR [Page_CompletePath] LIKE @query)");
            }
            else
            {
                filter.AddSql("([Page_Name] LIKE @query OR [Page_LinkText] LIKE @query OR [Page_Description] LIKE @query)");
            }

            var result = await connector.FetchAllAsync(filter);
            return result.ToArray();
        }

        /// <summary>
        /// Selects all.
        /// </summary>
        /// <param name="keylist">The keylist.</param>
        /// <returns></returns>
        public static Page[] SelectAll(int[] keylist)
        {
            var connector = ConnectorFactory.CreateConnector<Page>();
            var filter = connector.CreateQuery();
            filter.Add(x => x.ID, keylist, ComparisonOperator.In);

            return connector.FetchAll(filter).ToArray();
        }


        /// <summary>
        /// Selects all.
        /// </summary>
        /// <param name="keylist">The keylist.</param>
        /// <returns></returns>
        public static async Task<Page[]> SelectAllAsync(int[] keylist)
        {
            var connector = ConnectorFactory.CreateConnector<Page>();
            var filter = connector.CreateQuery();
            filter.Add(x => x.ID, keylist, ComparisonOperator.In);

            var result = await connector.FetchAllAsync(filter);
            return result.ToArray();
        }

        /// <summary>
        /// Selects all based on page template.
        /// </summary>
        /// <param name="pageTemplateID">The page template ID.</param>
        /// <returns></returns>
        public static Page[] SelectAllBasedOnPageTemplate(int pageTemplateID)
        {
            var connector = ConnectorFactory.CreateConnector<Page>();
            var filter = connector.CreateQuery();
            filter.Add(x => x.TemplateID, pageTemplateID);

            return connector.FetchAll(filter).ToArray();
        }

        /// <summary>
        /// Selects all based on page template.
        /// </summary>
        /// <param name="pageTemplateID">The page template ID.</param>
        /// <returns></returns>
        public static async Task<Page[]> SelectAllBasedOnPageTemplateAsync(int pageTemplateID)
        {
            var connector = ConnectorFactory.CreateConnector<Page>();
            var filter = connector.CreateQuery();
            filter.Add(x => x.TemplateID, pageTemplateID);

            var result = await connector.FetchAllAsync(filter);
            return result.ToArray();
        }

        /// <summary>
        /// Selects all based on page template.
        /// </summary>
        /// <param name="pageTemplateArray">The page template array.</param>
        /// <returns></returns>
        public static Page[] SelectAllBasedOnPageTemplate(int[] pageTemplateArray)
        {
            var connector = ConnectorFactory.CreateConnector<Page>();
            var filter = connector.CreateQuery();
            filter.Add(x => x.TemplateID, pageTemplateArray, ComparisonOperator.In);

            return connector.FetchAll(filter).ToArray();
        }


        /// <summary>
        /// Selects all based on page template.
        /// </summary>
        /// <param name="pageTemplateArray">The page template array.</param>
        /// <returns></returns>
        public static async Task<Page[]> SelectAllBasedOnPageTemplateAsync(int[] pageTemplateArray)
        {
            var connector = ConnectorFactory.CreateConnector<Page>();
            var filter = connector.CreateQuery();
            filter.Add(x => x.TemplateID, pageTemplateArray, ComparisonOperator.In);

            var result = await connector.FetchAllAsync(filter);
            return result.ToArray();
        }

        /// <summary>
        /// Select all pages in a site (unregarding their state)
        /// </summary>
        /// <param name="siteID">The site ID.</param>
        /// <returns></returns>
        public static Page[] SelectAllBySite(int siteID)
        {
            var connector = ConnectorFactory.CreateConnector<Page>();
            var filter = connector.CreateQuery();
            filter.Add(x => x.SiteID, siteID);

            return connector.FetchAll(filter).ToArray();
        }


        /// <summary>
        /// Select a page inherited child (returns only published pages)
        /// </summary>
        /// <param name="pageID">The page ID.</param>
        /// <param name="siteID">The site ID.</param>
        /// <returns></returns>
        public static Page SelectOneChild(int pageID, int siteID)
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
        public static Page SelectOneChild(int pageID, int siteID, bool returnOnlyPublishedPage)
        {
            var connector = ConnectorFactory.CreateConnector<Page>();
            var filter = connector.CreateQuery();
            filter.AddSql("Page_Master_Key = @Page OR Page_Key = @Page");
            filter.AddParameter("Page", pageID);
            filter.Add(x => x.SiteID, siteID);

            if (returnOnlyPublishedPage)
            {
                filter.Add(x => x.IsPublished, true);
            }

            var result = connector.FetchSingle(filter);
            return result;
        }

        /// <summary>
        /// Select a page inherited child
        /// </summary>
        /// <param name="pageID">The page ID.</param>
        /// <param name="siteID">The site ID.</param>
        /// <param name="returnOnlyPublishedPage">Only return published pages</param>
        /// <returns></returns>
        public static async Task<Page> SelectOneChildAsync(int pageID, int siteID, bool returnOnlyPublishedPage)
        {
            var connector = ConnectorFactory.CreateConnector<Page>();
            var filter = connector.CreateQuery();
            filter.AddSql("Page_Master_Key = @Page OR Page_Key = @Page");
            filter.AddParameter("Page", pageID);
            filter.Add(x => x.SiteID, siteID);

            if (returnOnlyPublishedPage)
            {
                filter.Add(x => x.IsPublished, true);
            }

            var result = await connector.FetchSingleAsync(filter);
            return result;
        }

        /// <summary>
        /// Select all pages in a site (unregarding their state)
        /// </summary>
        /// <param name="siteID">The site ID.</param>
        /// <returns></returns>
        public static async Task<Page[]> SelectAllBySiteAsync(int siteID)
        {
            var connector = ConnectorFactory.CreateConnector<Page>();
            var filter = connector.CreateQuery();
            filter.Add(x => x.SiteID, siteID);

            var result = await connector.FetchAllAsync(filter);
            return result.ToArray();
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
        /// <returns></returns>
        public static async Task<Page[]> SelectAllByCustomDateAsync(int folderID)
        {
            return await SelectAllByCustomDateAsync(folderID, null, true, 0);
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
        /// Default: Sorted DESCENDING
        /// </summary>
        /// <param name="folderID">The folder ID.</param>
        /// <param name="pageTemplateID">The page template ID.</param>
        /// <returns></returns>
        public static async Task<Page[]> SelectAllByCustomDateAsync(int folderID, int? pageTemplateID)
        {
            return await SelectAllByCustomDateAsync(folderID, pageTemplateID, true, 0);
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
        /// <returns></returns>
        public static async Task<Page[]> SelectAllByCustomDateAsync(int folderID, int? pageTemplateID, bool desc)
        {
            return await SelectAllByCustomDateAsync(folderID, pageTemplateID, desc, 0);
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
            var connector = ConnectorFactory.CreateConnector<Page>();
            var filter = connector.CreateQuery();

            if (maxReturnCount != 0)
            {
                filter.MaxResults = maxReturnCount;
            }

            if (desc)
            {
                filter.AddOrder(x => x.CustomDate, Sushi.MicroORM.SortOrder.DESC);
            }
            else
            {
                filter.AddOrder(x => x.CustomDate, Sushi.MicroORM.SortOrder.ASC);
            }

            filter.Add(x => x.FolderID, folderID);
            filter.Add(x => x.IsPublished, true);
            filter.Add(x => x.CustomDate, null, ComparisonOperator.NotEqualTo);
            filter.AddParameter("@currentDate", DateTime.Now);

            filter.AddSql("([Page_Publish] IS NULL OR [Page_Publish] < @currentDate)");
            filter.AddSql("([Page_Expire] IS NULL OR [Page_Expire] > @currentDate)");

            if (pageTemplateID.HasValue)
            {
                filter.Add(x => x.TemplateID, pageTemplateID.Value);
            }

            return connector.FetchAll(filter).ToArray();
        }


        /// <summary>
        /// Select all pages present in designated folder which al published and having a custom date field attached to it which is applied
        /// </summary>
        /// <param name="folderID">The folder ID.</param>
        /// <param name="pageTemplateID">The page template ID.</param>
        /// <param name="desc">sort order is descending?</param>
        /// <param name="maxReturnCount">Maximum number of pages to return</param>
        /// <returns></returns>
        public static async Task<Page[]> SelectAllByCustomDateAsync(int folderID, int? pageTemplateID, bool desc, int maxReturnCount)
        {
            var connector = ConnectorFactory.CreateConnector<Page>();
            var filter = connector.CreateQuery();

            if (maxReturnCount != 0)
            {
                filter.MaxResults = maxReturnCount;
            }

            if (desc)
            {
                filter.AddOrder(x => x.CustomDate, Sushi.MicroORM.SortOrder.DESC);
            }
            else
            {
                filter.AddOrder(x => x.CustomDate, Sushi.MicroORM.SortOrder.ASC);
            }

            filter.Add(x => x.FolderID, folderID);
            filter.Add(x => x.IsPublished, true);
            filter.Add(x => x.CustomDate, null, ComparisonOperator.NotEqualTo);
            filter.AddParameter("@currentDate", DateTime.Now);

            filter.AddSql("([Page_Publish] IS NULL OR [Page_Publish] < @currentDate)");
            filter.AddSql("([Page_Expire] IS NULL OR [Page_Expire] > @currentDate)");

            if (pageTemplateID.HasValue)
            {
                filter.Add(x => x.TemplateID, pageTemplateID.Value);
            }

            var result = await connector.FetchAllAsync(filter);
            return result.ToArray();
        }

        /// <summary>
        /// Selects all dated.
        /// </summary>
        /// <returns></returns>
        public static Page[] SelectAllDated()
        {
            var connector = ConnectorFactory.CreateConnector<Page>();
            var filter = connector.CreateQuery();
            filter.AddParameter("@currentDate", DateTime.Now);
            filter.AddSql("((NOT [Page_Publish] IS NULL OR NOT [Page_Expire] IS NULL) and ([Page_Publish] <= @currentDate OR [Page_Expire] <= @currentDate))");

            return connector.FetchAll(filter).ToArray();
        }

        /// <summary>
        /// Selects all dated.
        /// </summary>
        /// <returns></returns>
        public static async Task<Page[]> SelectAllDatedAsync()
        {
            var connector = ConnectorFactory.CreateConnector<Page>();
            var filter = connector.CreateQuery();
            filter.AddParameter("@currentDate", DateTime.Now);
            filter.AddSql("((NOT [Page_Publish] IS NULL OR NOT [Page_Expire] IS NULL) and ([Page_Publish] <= @currentDate OR [Page_Expire] <= @currentDate))");

            var result = await connector.FetchAllAsync(filter);
            return result.ToArray();
        }

        DateTime? IExportable.Updated
        {
            get
            {
                return Updated;
            }
        }
    }
}