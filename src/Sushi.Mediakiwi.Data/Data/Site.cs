using Sushi.MicroORM;
using Sushi.MicroORM.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sushi.Mediakiwi.Data.MicroORM;
using Sushi.Mediakiwi.Data.Interfaces;

namespace Sushi.Mediakiwi.Data
{
    /// <summary>
    /// A site can be seen as a single website, a subsite like a language or a list container.
    /// </summary>
    [DataMap(typeof(SiteMap))]
    public class Site : ISite
    {
        public class SiteMap : DataMap<Site>
        {
            public SiteMap()
            {
                Table("wim_Sites");//.Alias("innerSites");
                Id(x => x.ID, "Site_key").Identity();
                Map(x => x.Name, "Site_Displayname").Length(100);
                Map(x => x.CountryID, "Site_Country");
                Map(x => x.Language, "Site_Language").Length(8);
                Map(x => x.TimeZoneIndex, "Site_TimeZone").Length(50);
                Map(x => x.Culture, "Site_Culture").Length(15);
                Map(x => x.MasterID, "Site_Master_Key");
                Map(x => x.AutoPublishInherited, "Site_AutoPublishInherited");
                Map(x => x.IsActive, "Site_IsActive");
                Map(x => x.Type, "Site_Type").ReadOnly();
                Map(x => x.HasPages, "Site_HasPages");
                Map(x => x.HasLists, "Site_HasLists");
                Map(x => x.Created, "Site_Created");
                Map(x => x.DefaultPageTitle, "Site_DefaultTitle").Length(100);
                Map(x => x.DefaultFolder, "Site_DefaultFolder").Length(50);
                Map(x => x.Domain, "Site_Domain").Length(255);
                Map(x => x.HomepageID, "Site_HomePage_Key");
                Map(x => x.PageNotFoundID, "Site_PageNotFoundPage_Key");
                Map(x => x.ErrorPageID, "Site_ErrorPage_Key");
                Map(x => x.GUID, "Site_Guid");

                Map(x => x.ChildCount, "(SELECT COUNT(*) FROM [wim_Sites] sites WHERE sites.[Site_Master_Key] = [Site_Key])").Alias("Site_ChildCount").ReadOnly();
                Map(x => x.PageCount, "(SELECT COUNT(*) FROM [wim_Pages] JOIN [wim_Folders] ON [Page_Folder_Key] = [Folder_Key] WHERE [Folder_Site_Key] = [Site_Key])").Alias("Site_PageCount").ReadOnly();
                Map(X => X.ListCount, "(SELECT COUNT(*) FROM [wim_ComponentLists] WHERE [ComponentList_Site_Key] = [Site_Key])").Alias("Site_ListCount").ReadOnly();
                
                // [MR:20-01-2020] this causes a StackOverflow exception
                // So it's been refactored to a SelectOne - not optimal - but at least it works
                // Map(x => x.MasterImplement, "Site_Master_Key").Alias("masterSite").ExtendOn("Site_Key", true);
            }
        }

        #region Properties

        /// <summary>
        /// Does this site have inheritance children?
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has children; otherwise, <c>false</c>.
        /// </value>
        public bool HasChildren
        {
            get { return ChildCount > 0; }
        }

        /// <summary>
        /// The amount of children (inherited sites) that this site has
        /// </summary>
        /// <value>The child count.</value>
        public int ChildCount { get; set; }

        /// <summary>
        /// Gets or sets the page count.
        /// </summary>
        /// <value>The page count.</value>
        public int PageCount { get; set; }
        
        /// <summary>
        /// Gets or sets the list count.
        /// </summary>
        /// <value>The list count.</value>
        public int ListCount { get; set; }

        private string m_Name;

        /// <summary>
        /// Name of this site
        /// </summary>
        public string Name
        {
            get
            {
                if (m_Name == "[internal]" && this.Type.GetValueOrDefault(0) == 1)
                    m_Name = "Administration";
                return m_Name;
            }
            set { m_Name = value; }
        }

        private string[] m_Domains;

        /// <summary>
        /// Gets the domains.
        /// </summary>
        public string[] Domains
        {
            get
            {
                if (m_Domains == null)
                {
                    if (string.IsNullOrEmpty(this.Domain))
                        m_Domains = new string[0];
                    else
                        m_Domains = Domain.Split(',');
                }
                return m_Domains;
            }
        }

        /// <summary>
        /// Country identifier of the site
        /// </summary>
        public int? CountryID { get; set; }

        private ICountry m_Country;

        /// <summary>
        /// Corresponding site country
        /// </summary>
        public ICountry Country
        {
            get
            {
                if (m_Country == null && CountryID.HasValue)
                    m_Country = Data.Country.SelectOne(CountryID.Value);
                return m_Country;
            }
        }

        /// <summary>
        /// Language of this site
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// The TimeZoneIndex for this site
        /// </summary>
        public string TimeZoneIndex { get; set; }

        private TimeZoneInfo m_TimeZone;

        /// <summary>
        /// TimeZone information
        /// </summary>
        public TimeZoneInfo TimeZone
        {
            get
            {
                if (m_TimeZone == null)
                {
                    try
                    {
                        if (string.IsNullOrEmpty(TimeZoneIndex))
                            m_TimeZone = TimeZoneInfo.Local;
                        else
                            m_TimeZone = TimeZoneInfo.FindSystemTimeZoneById(TimeZoneIndex);
                    }
                    catch (Exception ex)
                    {
                        m_TimeZone = TimeZoneInfo.Local;
                        Notification.InsertOne("Timezone", ex.ToString());
                    }
                }
                return m_TimeZone;
            }
        }

        private string m_Culture;

        /// <summary>
        /// The Culture of this site
        /// </summary>
        public string Culture
        {
            get { return ApplyCultureFix(m_Culture); }
            set { m_Culture = value; }
        }

        /// <summary>
        /// The Culture name for this site
        /// </summary>
        public string CultureName
        {
            get
            {
                var cult = new System.Globalization.CultureInfo(Culture);
                if (cult != null && cult.DisplayName != null)
                {
                    if (cult.DisplayName.IndexOf('(') > 0)
                        return cult.DisplayName.Substring(0, cult.DisplayName.IndexOf('(') - 1);
                    else
                        return cult.DisplayName;
                }
                return "";
            }
        }

        /// <summary>
        /// The site from which this site inherites all its content
        /// </summary>
        public int? MasterID { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether auto publish inherited content is on. When on this copies the Page_IsPublished and Page_CustomDate.
        /// </summary>
        public bool AutoPublishInherited { get; set; }

        /// <summary>
        /// Is this site active (visible)?
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Gets or sets the type of site.
        /// </summary>
        public int? Type { get; set; }

        /// <summary>
        /// Does this site have pages?
        /// </summary>
        public bool HasPages { get; set; }

        /// <summary>
        /// Does this site have lists?
        /// </summary>
        public bool HasLists { get; set; } = true;

        private DateTime m_Created;

        /// <summary>
        /// The creation date/time (UTC) of this site
        /// </summary>
        public DateTime Created
        {
            get
            {
                if (this.m_Created == DateTime.MinValue)
                    this.m_Created = Common.DatabaseDateTime;
                return m_Created;
            }
            set { m_Created = value; }
        }

        /// <summary>
        /// The default page title of the pages within this site
        /// </summary>
        public string DefaultPageTitle { get; set; }

        /// <summary>
        /// The default startup folder of this site
        /// </summary>
        public string DefaultFolder { get; set; }

        /// <summary>
        /// Gets or sets the domain for this site.
        /// </summary>
        public string Domain { get; set; }

        /// <summary>
        /// The page identifier of the homepage
        /// </summary>
        public int? HomepageID { get; set; }

        /// <summary>
        /// The page identifier of the page not found page
        /// </summary>
        public int? PageNotFoundID { get; set; }

        /// <summary>
        /// The page identifier of the error page
        /// </summary>
        public int? ErrorPageID { get; set; }

        /// <summary>
        /// Uniqe identifier of the site
        /// </summary>
        public int ID { get; set; }

        internal Guid m_GUID;

        /// <summary>
        /// Unique identifier used for import/export
        /// </summary>
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

        private Site m_MasterImplement;
        /// <summary>
        /// The Master (parent) site of this site
        /// </summary>
        public Site MasterImplement
        {
            get
            {
                if (m_MasterImplement == null && MasterID.GetValueOrDefault(0) > 0)
                    m_MasterImplement = Site.SelectOne(MasterID.Value);

                return m_MasterImplement;
            }
            set 
            {
                m_MasterImplement = value;
            }
        }

        /// <summary>
        /// The name of the site from which this site inherites all its content
        /// </summary>
        public string Master
        {
            get
            {
                return (MasterImplement?.ID > 0) ? MasterImplement.Name : "";
            }
        }

        #endregion Properties

        public void Save()
        {
            var connector = ConnectorFactory.CreateConnector<Site>();
            connector.Save(this);
        }

        public async Task SaveAsync()
        {
            var connector = ConnectorFactory.CreateConnector<Site>();
            await connector.SaveAsync(this);
        }

        /// <summary>
        /// Delete an implementation record.
        /// </summary>
        /// <returns></returns>
        public void Delete()
        {
            var connector = ConnectorFactory.CreateConnector<Site>();
            var filter = connector.CreateDataFilter();
            filter.AddParameter("@siteID", ID);

            if (this.HasPages || this.HasLists)
                throw new Exception("Before a site can be deleted the HasPages and HasLists should be False");

            connector.ExecuteNonQuery(@"
delete from wim_Components where Component_Page_Key in
    (select Page_Key from wim_Pages where Page_Folder_Key in
        (select Folder_Key from wim_Folders where Folder_Site_Key = @siteID)
    )", filter);

            connector.ExecuteNonQuery(@"
delete from wim_ComponentVersions where ComponentVersion_Page_Key in
    (select Page_Key from wim_Pages where Page_Folder_Key in
	    (select Folder_Key from wim_Folders where Folder_Site_Key = @siteID)
    )", filter);

            connector.ExecuteNonQuery(@"
    delete from wim_Pages where Page_Folder_Key in (
	    select Folder_Key from wim_Folders where Folder_Site_Key = @siteID
    )", filter);

            connector.ExecuteNonQuery("delete from wim_RoleRights where RoleRight_Child_Key = @siteID and RoleRight_Child_Type = 2", filter);
            connector.ExecuteNonQuery("update wim_ComponentLists set ComponentList_Site_Key = null, ComponentList_Folder_Key = null where ComponentList_Site_Key = @siteID", filter);

            // [26-09-2008:MM] Intersystems cache cannot recognize parent child relations in one table.
            foreach (Folder folder in Folder.SelectAllForDeletion(this.ID))
                folder.Delete();

            connector.Delete(this);
			connector.Cache?.FlushRegion(connector.CacheRegion);
        }

        /// <summary>
        /// Delete an implementation record Async.
        /// </summary>
        /// <returns></returns>
        public async Task DeleteAsync()
        {
            var connector = ConnectorFactory.CreateConnector<Site>();
            var filter = connector.CreateDataFilter();
            filter.AddParameter("@siteID", ID);

            if (this.HasPages || this.HasLists)
                throw new Exception("Before a site can be deleted the HasPages and HasLists should be False");

            await connector.ExecuteNonQueryAsync(@"
delete from wim_Components where Component_Page_Key in
    (select Page_Key from wim_Pages where Page_Folder_Key in
        (select Folder_Key from wim_Folders where Folder_Site_Key = @siteID)
    )", filter);

            await connector.ExecuteNonQueryAsync(@"
delete from wim_ComponentVersions where ComponentVersion_Page_Key in
    (select Page_Key from wim_Pages where Page_Folder_Key in
	    (select Folder_Key from wim_Folders where Folder_Site_Key = @siteID)
    )", filter);

            await connector.ExecuteNonQueryAsync(@"
    delete from wim_Pages where Page_Folder_Key in (
	    select Folder_Key from wim_Folders where Folder_Site_Key = @siteID
    )", filter);

            await connector.ExecuteNonQueryAsync("delete from wim_RoleRights where RoleRight_Child_Key = @siteID and RoleRight_Child_Type = 2", filter);
            await connector.ExecuteNonQueryAsync("update wim_ComponentLists set ComponentList_Site_Key = null, ComponentList_Folder_Key = null where ComponentList_Site_Key = @siteID", filter);

            // [26-09-2008:MM] Intersystems cache cannot recognize parent child relations in one table.
            foreach (Folder folder in await Folder.SelectAllForDeletionAsync(this.ID))
                await folder.DeleteAsync();

            await connector.DeleteAsync(this);
			connector.Cache?.FlushRegion(connector.CacheRegion);
        }

        /// <summary>
        /// Select a Site based on its primary key
        /// </summary>
        public static Site SelectOne(int ID)
        {
            var connector = ConnectorFactory.CreateConnector<Site>();
            return connector.FetchSingle(ID);
        }

        /// <summary>
        /// Select a Site based on its primary key
        /// </summary>
        public static async Task<Site> SelectOneAsync(int ID)
        {
            var connector = ConnectorFactory.CreateConnector<Site>();
            return await connector.FetchSingleAsync(ID);
        }

        /// <summary>
        /// Get a site based on its unique GUID key
        /// </summary>
        /// <param name="guid">The site GUID.</param>
        public static Site SelectOne(Guid guid)
        {
            var connector = ConnectorFactory.CreateConnector<Site>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.GUID, guid);
            return connector.FetchSingle(filter);
        }

        /// <summary>
        /// Get a site based on its unique GUID key Async
        /// </summary>
        /// <param name="guid">The site GUID.</param>
        public static async Task<Site> SelectOneAsync(Guid guid)
        {
            var connector = ConnectorFactory.CreateConnector<Site>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.GUID, guid);
            return await connector.FetchSingleAsync(filter);
        }

        /// <summary>
        /// Gets the domain prefix.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns></returns>
        public static string GetDomainPrefix(Uri url)
        {
            Site site;
            return GetDomainPrefix(url, out site);
        }

        /// <summary>
        /// Gets the site which matches the domain prefix.
        /// </summary>
        /// <param name="url">The URL (domain prefix).</param>
        /// <param name="siteMatch">The site which matches.</param>
        /// <returns></returns>
        public static string GetDomainPrefix(Uri url, out Site siteMatch)
        {
            foreach (Site site in SelectAll())
            {
                if (site.IsDomain(url))
                {
                    siteMatch = site;
                    return site.DefaultFolder;
                }
            }
            siteMatch = null;
            return null;
        }

        /// <summary>
        /// Determines whether the specified URL matches one of this sites domains.
        /// </summary>
        /// <param name="url">The URL.</param>
        public bool IsDomain(Uri url)
        {
            if (string.IsNullOrEmpty(this.Domain)) return false;
            string candidate = url.ToString().Replace("http://", string.Empty).Replace("https://", string.Empty).Split('/')[0].ToLower();
            foreach (string item in Domains)
            {
                if (item == candidate)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Applies the culture fix.
        /// </summary>
        /// <param name="culture">The culture.</param>
        /// <returns></returns>
        private string ApplyCultureFix(string culture)
        {
            switch (culture)
            {
                case "az-AZ-Latn": return "az-Latn-AZ";
                case "uz-UZ-Latn": return "uz-Latn-UZ";
                case "sr-SP-Latn": return "sr-Latn-CS";
                case "az-AZ-Cyrl": return "az-Cyrl-AZ";
                case "uz-UZ-Cyrl": return "uz-Cyrl-UZ";
                case "sr-SP-Cyrl": return "sr-Cyrl-CS";
                case "bs-BA-Cyrl": return "bs-Cyrl-BA";
                case "sr-BA-Latn": return "sr-Latn-BA";
                case "sr-BA-Cyrl": return "sr-Cyrl-BA";
                case "bs-BA-Latn": return "bs-Latn-BA";
                case "iu-CA-Latn": return "iu-Latn-CA";
                case "div-MV": return "dv-MV";
                case "en-CB": return "en-029";
            }
            return culture;
        }

        /// <summary>
        /// Selects a site based on the Page Identifier.
        /// </summary>
        /// <param name="pageID">The page ID.</param>
        /// <returns></returns>
        public static Site SelectOneByPage(int pageID)
        {
            var connector = ConnectorFactory.CreateConnector<Site>();
            var filter = connector.CreateDataFilter();
            filter.AddParameter("@pageID", pageID);

            return connector.FetchSingle(@"SELECT TOP 1 *
                    FROM [dbo].[wim_Sites]
                    left join [dbo].[wim_Folders] on [Folder_Site_Key] = [Site_Key]
                    left join [dbo].[wim_Pages] on [Page_Folder_Key] = [Folder_Key]
                    where [Page_Key] = @pageID
                  ", filter);
        }

        /// <summary>
        /// Selects a site based on the Page Identifier Async.
        /// </summary>
        /// <param name="pageID">The page ID.</param>
        /// <returns></returns>
        public static async Task<Site> SelectOneByPageAsync(int pageID)
        {
            var connector = ConnectorFactory.CreateConnector<Site>();
            var filter = connector.CreateDataFilter();
            filter.AddParameter("@pageID", pageID);

            return await connector.FetchSingleAsync(@"SELECT TOP 1 *
                    FROM [dbo].[wim_Sites]
                    left join [dbo].[wim_Folders] on [Folder_Site_Key] = [Site_Key]
                    left join [dbo].[wim_Pages] on [Page_Folder_Key] = [Folder_Key]
                    where [Page_Key] = @pageID
                  ", filter);
        }

        /// <summary>
        /// Selects a site based on a Folder Identifier.
        /// </summary>
        /// <param name="folderID">The folder ID.</param>
        public static Site SelectOneByFolder(int folderID)
        {
            var connector = ConnectorFactory.CreateConnector<Site>();
            var filter = connector.CreateDataFilter();
            filter.AddParameter("@folderID", folderID);

            return connector.FetchSingle(@"SELECT TOP 1 *
                    FROM [dbo].[wim_Sites]
                    left join [dbo].[wim_Folders] on [Folder_Site_Key] = [Site_Key]
                    where [Folder_Key] = @folderID
                  ", filter);
        }

        /// <summary>
        /// Selects a site based on a Folder Identifier Async.
        /// </summary>
        /// <param name="folderID">The folder ID.</param>
        public static async Task<Site> SelectOneByFolderAsync(int folderID)
        {
            var connector = ConnectorFactory.CreateConnector<Site>();
            var filter = connector.CreateDataFilter();
            filter.AddParameter("@folderID", folderID);

            return await connector.FetchSingleAsync(@"SELECT TOP 1 *
                    FROM [dbo].[wim_Sites]
                    left join [dbo].[wim_Folders] on [Folder_Site_Key] = [Site_Key]
                    where [Folder_Key] = @folderID
                  ", filter);
        }

        /// <summary>
        /// Select a site based on a relative folder path (tries to match this with the default folder).
        /// </summary>
        /// <param name="searchPath">Relative search path</param>
        /// <param name="excludeAdmin">Exclude the 'Administrator' site</param>
        /// <returns>Site object</returns>
        public static Site SelectOneSiteResolution(string searchPath, bool excludeAdmin = false)
        {
            var connector = ConnectorFactory.CreateConnector<Site>();
            var filter = connector.CreateDataFilter();
            filter.AddOrder(x => x.DefaultFolder, SortOrder.DESC);
            filter.AddParameter("@searchPath", $"'{searchPath}%'");

            if (string.IsNullOrEmpty(searchPath))
                filter.Add(x => x.DefaultFolder, null);
            else
                filter.AddSql("Site_DefaultFolder + '/' like @searchPath");

            if (excludeAdmin)
                filter.Add(x => x.Name, "Administration", ComparisonOperator.NotEqualTo);

            return connector.FetchSingle(filter);
        }

        /// <summary>
        /// Select a site based on a relative folder path (tries to match this with the default folder) Async.
        /// </summary>
        /// <param name="searchPath">Relative search path</param>
        /// <param name="excludeAdmin">Exclude the 'Administrator' site</param>
        /// <returns>Site object</returns>
        public static async Task<Site> SelectOneSiteResolutionAsync(string searchPath, bool excludeAdmin = false)
        {
            var connector = ConnectorFactory.CreateConnector<Site>();
            var filter = connector.CreateDataFilter();
            filter.AddOrder(x => x.DefaultFolder, SortOrder.DESC);
            filter.AddParameter("@searchPath", $"'{searchPath}%'");

            if (string.IsNullOrEmpty(searchPath))
                filter.Add(x => x.DefaultFolder, null);
            else
                filter.AddSql("Site_DefaultFolder + '/' like @searchPath");

            if (excludeAdmin)
                filter.Add(x => x.Name, "Administration", ComparisonOperator.NotEqualTo);

            return await connector.FetchSingleAsync(filter);
        }

        /// <summary>
        /// Select a site based on a relative folder path (tries to match this with the default folder).
        /// </summary>
        /// <param name="searchPath">Relative search path</param>
        public static Site SelectOne(string searchPath)
        {
            var connector = ConnectorFactory.CreateConnector<Site>();
            var filter = connector.CreateDataFilter();
            filter.AddOrder(x => x.DefaultFolder, SortOrder.DESC);

            if (string.IsNullOrEmpty(searchPath))
                filter.Add(x => x.DefaultFolder, null);
            else
                filter.Add(x => x.DefaultFolder, $"{searchPath}%", ComparisonOperator.Like);

            return connector.FetchSingle(filter);
        }

        /// <summary>
        /// Select a site based on a relative folder path (tries to match this with the default folder) Async.
        /// </summary>
        /// <param name="searchPath">Relative search path</param>
        public static async Task<Site> SelectOneAsync(string searchPath)
        {
            var connector = ConnectorFactory.CreateConnector<Site>();
            var filter = connector.CreateDataFilter();
            filter.AddOrder(x => x.DefaultFolder, SortOrder.DESC);

            if (string.IsNullOrEmpty(searchPath))
                filter.Add(x => x.DefaultFolder, null);
            else
                filter.Add(x => x.DefaultFolder, $"{searchPath}%", ComparisonOperator.Like);

            return await connector.FetchSingleAsync(filter);
        }

        /// <summary>
        /// Selects the base.
        /// </summary>
        /// <returns></returns>
        private static Site SelectBase()
        {
            var selection = (from item in SelectAll() where item.Type.HasValue && item.Type == 1 select item).ToArray();
            return selection.Count() == 1 ? selection[0] : new Site();
        }

        /// <summary>
        /// Selects all accessible.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="accessFilter">The access filter.</param>
        /// <returns></returns>
        public static Site[] SelectAllAccessible(IApplicationUser user, AccessFilter accessFilter)
        {
            List<Site> sites = null;
            if (accessFilter == AccessFilter.RoleBased)
            {
                if (!user.Role().All_Sites)
                {
                    if (user.Role().IsAccessSite)
                    {
                        var rolerights = RoleRight.SelectAll(user.Role().ID, RoleRightType.Site);

                        sites = (
                            from item in SelectAll(true)
                            join relation in rolerights on item.ID equals relation.ItemID
                            select item).ToList();
                    }
                    else
                    {
                        var rolerights = RoleRight.SelectAll(user.Role().ID, RoleRightType.Site);

                        var acl = (
                            from item in SelectAll()
                            join relation in rolerights on item.ID equals relation.ItemID
                            into combination
                            from relation in combination.DefaultIfEmpty()
                            select new { ID = item.ID, HasAccess = relation == null });

                        sites = (
                            from item in acl
                            join relation in SelectAll() on item.ID equals relation.ID
                            where item.HasAccess
                            select relation).ToList();
                    }
                }
                else
                    return SelectAll().ToArray();
            }
            else if (accessFilter == AccessFilter.UserBased)
            {
                var acl = (
                    from item in SelectAll()
                    join relation in RoleRight.SelectAll(user.ID, RoleRightType.SiteByUser) on item.ID equals relation.ItemID
                    into combination
                    from relation in combination.DefaultIfEmpty()
                    select new { item.ID, Access = (relation == null ? Access.Inherit : relation.AccessType) });

                sites = (
                    from item in acl
                    join relation in SelectAll() on item.ID equals relation.ID
                    where item.Access == Access.Granted
                    select relation).ToList();
            }
            else if (accessFilter == AccessFilter.RoleAndUser)
            {
                //  Get the access list form the user list
                //  This contains all sites with the Access enum set.
                var acl = (
                    from item in SelectAll()
                    join relation in RoleRight.SelectAll(user.ID, RoleRightType.SiteByUser) on item.ID equals relation.ItemID
                    into combination
                    from relation in combination.DefaultIfEmpty()
                    select new { item.ID, Access = (relation == null ? Access.Inherit : relation.AccessType) });
                //  Get the role based site access list
                var roleBased = SelectAllAccessible(user, AccessFilter.RoleBased);
                //  Combine the role and user based access lists.
                //  Where it says user inherit use the setting from the role (present = granted, not present = denied)
                var combined = (
                    from item in acl
                    join relation in roleBased on item.ID equals relation.ID
                    into combination
                    from relation in combination.DefaultIfEmpty()
                    select new
                    {
                        item.ID,
                        Access =
                        (
                        relation == null
                            ? item.Access == Access.Inherit ? Access.Denied : item.Access
                            : item.Access == Access.Inherit ? Access.Granted : item.Access
                        )
                    }).ToArray();
                //  Remove the denied types from the combined access list.
                var outcome =
                    (from item in SelectAll()
                     join relation in combined on item.ID equals relation.ID
                     where relation.Access == Access.Granted
                     select item).ToArray();
                //  Return the outcome
                return outcome;
            }

            //sites.Add(SelectBase());
            return sites.ToArray();
        }

        /// <summary>
        /// Selects all accessible.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="accessFilter">The access filter.</param>
        /// <returns></returns>
        public static async Task<Site[]> SelectAllAccessibleAsync(IApplicationUser user, AccessFilter accessFilter)
        {
            List<Site> sites = null;
            if (accessFilter == AccessFilter.RoleBased)
            {
                if (!user.Role().All_Sites)
                {
                    if (user.Role().IsAccessSite)
                    {
                        sites = (
                            from item in await SelectAllAsync()
                            join relation in await RoleRight.SelectAllAsync(user.Role().ID, RoleRightType.Site) on item.ID equals relation.ItemID
                            select item).ToList();
                    }
                    else
                    {
                        var acl = (
                            from item in await SelectAllAsync()
                            join relation in await RoleRight.SelectAllAsync(user.Role().ID, RoleRightType.Site) on item.ID equals relation.ItemID
                            into combination
                            from relation in combination.DefaultIfEmpty()
                            select new { ID = item.ID, HasAccess = relation == null });

                        sites = (
                            from item in acl
                            join relation in await SelectAllAsync() on item.ID equals relation.ID
                            where item.HasAccess
                            select relation).ToList();
                    }
                }
                else
                    return (await SelectAllAsync()).ToArray();
            }
            else if (accessFilter == AccessFilter.UserBased)
            {
                var acl = (
                    from item in await SelectAllAsync()
                    join relation in await RoleRight.SelectAllAsync(user.ID, RoleRightType.SiteByUser) on item.ID equals relation.ItemID
                    into combination
                    from relation in combination.DefaultIfEmpty()
                    select new { item.ID, Access = (relation == null ? Access.Inherit : relation.AccessType) });

                sites = (
                    from item in acl
                    join relation in await SelectAllAsync() on item.ID equals relation.ID
                    where item.Access == Access.Granted
                    select relation).ToList();
            }
            else if (accessFilter == AccessFilter.RoleAndUser)
            {
                //  Get the access list form the user list
                //  This contains all sites with the Access enum set.
                var acl = (
                    from item in await SelectAllAsync()
                    join relation in await RoleRight.SelectAllAsync(user.ID, RoleRightType.SiteByUser) on item.ID equals relation.ItemID
                    into combination
                    from relation in combination.DefaultIfEmpty()
                    select new { item.ID, Access = (relation == null ? Access.Inherit : relation.AccessType) });
                //  Get the role based site access list
                var roleBased = await SelectAllAccessibleAsync(user, AccessFilter.RoleBased);
                //  Combine the role and user based access lists.
                //  Where it says user inherit use the setting from the role (present = granted, not present = denied)
                var combined = (
                    from item in acl
                    join relation in roleBased on item.ID equals relation.ID
                    into combination
                    from relation in combination.DefaultIfEmpty()
                    select new
                    {
                        item.ID,
                        Access =
                        (
                        relation == null
                            ? item.Access == Access.Inherit ? Access.Denied : item.Access
                            : item.Access == Access.Inherit ? Access.Granted : item.Access
                        )
                    }).ToArray();
                //  Remove the denied types from the combined access list.
                var outcome =
                    (from item in await SelectAllAsync()
                     join relation in combined on item.ID equals relation.ID
                     where relation.Access == Access.Granted
                     select item).ToArray();
                //  Return the outcome
                return outcome;
            }

            sites.Add(SelectBase());
            return sites.ToArray();
        }

        /// <summary>
        /// Selects all active sites.
        /// </summary>
        internal static List<Site> SelectAllActive()
        {
            var connector = ConnectorFactory.CreateConnector<Site>();
            var filter = connector.CreateDataFilter();
            filter.AddOrder(x => x.Type);
            filter.AddOrder(x => x.Name);

            filter.Add(x => x.IsActive, true);
            filter.Add(x => x.Type, null);

            return connector.FetchAll(filter);
        }

        /// <summary>
        /// Selects all active sites Async.
        /// </summary>
        internal static async Task<List<Site>> SelectAllActiveAsync()
        {
            var connector = ConnectorFactory.CreateConnector<Site>();
            var filter = connector.CreateDataFilter();
            filter.AddOrder(x => x.Type);
            filter.AddOrder(x => x.Name);

            filter.Add(x => x.IsActive, true);
            filter.Add(x => x.Type, null);

            return await connector.FetchAllAsync(filter);
        }

        /// <summary>
        /// Select all sites
        /// </summary>
        public static List<Site> SelectAll(bool ignoreAdministration = true)
        {
            var connector = ConnectorFactory.CreateConnector<Site>();
            var filter = connector.CreateDataFilter();
            filter.AddOrder(x => x.Type);
            filter.AddOrder(x => x.Name);

            if (ignoreAdministration)
                filter.Add(x => x.Type, null);

            return connector.FetchAll(filter);
        }

        /// <summary>
        /// Select all sites
        /// </summary>
        public static async Task<List<Site>> SelectAllAsync(bool ignoreAdministration = true)
        {
            var connector = ConnectorFactory.CreateConnector<Site>();
            var filter = connector.CreateDataFilter();
            filter.AddOrder(x => x.Type);
            filter.AddOrder(x => x.Name);

            if (ignoreAdministration)
                filter.Add(x => x.Type, null);

            return await connector.FetchAllAsync(filter);
        }

        /// <summary>
        /// Select all available sites based on part of the title
        /// </summary>
        /// <returns>Array of site objects</returns>
        public static List<Site> SelectAll(string title)
        {
            return SelectAll(title, FolderType.Page);
        }

        /// <summary>
        /// Select all available sites based on part of the title and the folder Type
        /// </summary>
        /// <returns>Array of site objects</returns>
        public static List<Site> SelectAll(string title, FolderType type)
        {
            var connector = ConnectorFactory.CreateConnector<Site>();
            var filter = connector.CreateDataFilter();
            filter.AddOrder(x => x.Name);
            filter.AddParameter("@folderTypeID", (int)type);
            filter.AddParameter("@siteDisplayName", title);

            return connector.FetchAll(@"SELECT *
                    FROM [dbo].[wim_Sites]
                    LEFT JOIN [dbo].[wim_Folders] on [Folder_Site_Key] = [Site_Key]
                    WHERE
                        [Folder_Type] = @folderTypeID
                    AND [Folder_Folder_Key] IS NULL
                    AND [Site_Displayname] = @siteDisplayName
                    AND [Site_Type] is null
                  ", filter);
        }

        /// <summary>
        /// Select all available sites based on part of the title
        /// </summary>
        /// <returns>Array of site objects</returns>
        public static async Task<List<Site>> SelectAllAsync(string title)
        {
            return await SelectAllAsync(title, FolderType.Page);
        }

        /// <summary>
        /// Select all available sites based on part of the title and the folder Type
        /// </summary>
        /// <returns>Array of site objects</returns>
        public static async Task<List<Site>> SelectAllAsync(string title, FolderType type)
        {
            var connector = ConnectorFactory.CreateConnector<Site>();
            var filter = connector.CreateDataFilter();
            filter.AddOrder(x => x.Name);
            filter.AddParameter("@folderTypeID", (int)type);
            filter.AddParameter("@siteDisplayName", title);

            return await connector.FetchAllAsync(@"SELECT *
                    FROM [dbo].[wim_Sites]
                    LEFT JOIN [dbo].[wim_Folders] on [Folder_Site_Key] = [Site_Key]
                    WHERE
                        [Folder_Type] = @folderTypeID
                    AND [Folder_Folder_Key] IS NULL
                    AND [Site_Displayname] = @siteDisplayName
                    AND [Site_Type] is null
                  ", filter);
        }
    }
}