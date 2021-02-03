using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Sushi.Mediakiwi.Data.DalReflection;

namespace Sushi.Mediakiwi.Data
{
    public enum AccessFilter
    {
        RoleBased,
        UserBased,
        RoleAndUser
    }

    /// <summary>
    /// A site can be seen as a single website, a subsite like a language or a list container.
    /// </summary>
    [DatabaseTable("wim_Sites", Order = "Site_Type, Site_Displayname")]
    public class Site : DatabaseEntity
    {
        private string m_SubText = "Website properties";
        /// <summary>
        /// Gets or sets the ZZ_ sub text.
        /// </summary>
        /// <value>The ZZ_ sub text.</value>
        [Sushi.Mediakiwi.Framework.ContentListItem.TextLine(null)]
        public string zz_SubText
        {
            get { return m_SubText; }
            set { m_SubText = value; }
        }

        /// <summary>
        /// Save a database entity. This method will indentify the need for an INSERT or an UPDATE.
        /// </summary>
        /// <returns></returns>
        public override bool Save()
        {
            bool isNew = this.ID == 0;
            bool isSaved = base.Save();

            if (this.AutoPublishInherited)
            {
                this.Execute(string.Format(@"
update wim_Pages
set	
	Page_IsPublished = (select a.Page_IsPublished from wim_Pages a where a.Page_Key = wim_Pages.Page_Master_Key)
,	Page_CustomDate = (select a.Page_CustomDate from wim_Pages a where a.Page_Key = wim_Pages.Page_Master_Key)
,   Page_Published = (select a.Page_Published from wim_Pages a where a.Page_Key = wim_Pages.Page_Master_Key)
,   Page_Updated = (select a.Page_Updated from wim_Pages a where a.Page_Key = wim_Pages.Page_Master_Key)
where
	Page_InheritContent = 1
	and Page_Folder_Key in (
		select Folder_Key from wim_Folders join wim_Sites on Folder_Site_Key = Site_Key and Site_Key = {0}
)
"
                    , this.ID)
                    );
            }

            if (isNew)
            {
                this.CreateSiteFolders(this.ID);
            }

            if (this.MasterID.HasValue && this.HasPages)
            {
                Sushi.Mediakiwi.Framework.Inheritance.Folder.CreateFolderTree(MasterID.Value, ID, Sushi.Mediakiwi.Data.FolderType.Page);
                Sushi.Mediakiwi.Framework.Inheritance.Page.CreatePageTree(MasterID.Value, ID);

         
            }
            if (this.MasterID.HasValue && this.HasLists)
            {
                Sushi.Mediakiwi.Framework.Inheritance.Folder.CreateFolderTree(MasterID.Value, ID, Sushi.Mediakiwi.Data.FolderType.List);
            }
            return isSaved;
        }

        /// <summary>
        /// Selects all accessible.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="accessFilter">The access filter.</param>
        /// <returns></returns>
        public static Site[] SelectAllAccessible(Sushi.Mediakiwi.Data.IApplicationUser user, AccessFilter accessFilter)
        {
            List<Site> sites = null;
            if (accessFilter == AccessFilter.RoleBased)
            {
                if (!user.Role().All_Sites)
                {
                    if (user.Role().IsAccessSite)
                    {
                        sites = (
                            from item in SelectAll()
                            join relation in Sushi.Mediakiwi.Data.RoleRight.SelectAll(user.Role().ID, Sushi.Mediakiwi.Data.RoleRightType.Site) on item.ID equals relation.ItemID
                            select item).ToList();
                    }
                    else
                    {
                        var acl = (
                            from item in SelectAll()
                            join relation in Sushi.Mediakiwi.Data.RoleRight.SelectAll(user.Role().ID, Sushi.Mediakiwi.Data.RoleRightType.Site) on item.ID equals relation.ItemID
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
                    return SelectAll();
            }
            else if (accessFilter == AccessFilter.UserBased)
            {
                var acl = (
                    from item in SelectAll()
                    join relation in Sushi.Mediakiwi.Data.RoleRight.SelectAll(user.ID, Sushi.Mediakiwi.Data.RoleRightType.SiteByUser) on item.ID equals relation.ItemID
                    into combination
                    from relation in combination.DefaultIfEmpty()
                    select new { item.ID, Access = (relation == null ? Sushi.Mediakiwi.Data.RoleRight.Access.Inherit : relation.AccessType) });

                sites = (
                    from item in acl
                    join relation in SelectAll() on item.ID equals relation.ID
                    where item.Access == RoleRight.Access.Granted
                    select relation).ToList();
            }
            else if (accessFilter == AccessFilter.RoleAndUser)
            {
                //  Get the access list form the user list 
                //  This contains all sites with the Access enum set.
                var acl = (
                    from item in SelectAll()
                    join relation in Sushi.Mediakiwi.Data.RoleRight.SelectAll(user.ID, Sushi.Mediakiwi.Data.RoleRightType.SiteByUser) on item.ID equals relation.ItemID
                    into combination
                    from relation in combination.DefaultIfEmpty()
                    select new { item.ID, Access = (relation == null ? Sushi.Mediakiwi.Data.RoleRight.Access.Inherit : relation.AccessType) });
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
                            ? item.Access == RoleRight.Access.Inherit ? Sushi.Mediakiwi.Data.RoleRight.Access.Denied : item.Access
                            : item.Access == RoleRight.Access.Inherit ? Sushi.Mediakiwi.Data.RoleRight.Access.Granted : item.Access
                        )
                    }).ToArray();
                //  Remove the denied types from the combined access list.
                var outcome =
                    (from item in SelectAll()
                     join relation in combined on item.ID equals relation.ID
                     where relation.Access == RoleRight.Access.Granted
                     select item).ToArray();
                //  Return the outcome
                return outcome;
            }

            sites.Add(SelectBase());
            return sites.ToArray();
        }

        /// <summary>
        /// Selects the all_ import export.
        /// </summary>
        /// <param name="portal">The portal.</param>
        /// <returns></returns>
        public static List<Site> SelectAll_ImportExport(string portal)
        {
            Site implement = new Site();
            List<Site> list = new List<Site>();

            implement.SqlConnectionString = Sushi.Mediakiwi.Data.Common.GetPortal(portal).Connection;
            implement.ConnectionType = Sushi.Mediakiwi.Data.Common.GetPortal(portal).Type;

            foreach (object o in implement._SelectAll(null, false, "SiteImportExport", portal))
                list.Add((Site)o);

            return list;
        }

        /// <summary>
        /// Selects the one_ import export.
        /// </summary>
        /// <param name="portal">The portal.</param>
        /// <param name="guid">The GUID.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static Site SelectOne_ImportExport(string portal, Guid guid, string name)
        {
            Site implement = new Site();
            List<Site> list = new List<Site>();

            implement.SqlConnectionString = Sushi.Mediakiwi.Data.Common.GetPortal(portal).Connection;
            implement.ConnectionType = Sushi.Mediakiwi.Data.Common.GetPortal(portal).Type;

            List<DatabaseDataValueColumn> where = new List<DatabaseDataValueColumn>();
            where.Add(new DatabaseDataValueColumn("Site_Guid", SqlDbType.UniqueIdentifier, guid));
            where.Add(new DatabaseDataValueColumn("Site_Displayname", SqlDbType.NVarChar, name, DatabaseDataValueConnectType.Or));

            return (Site)implement._SelectOne(where);
        }

        #region MOVED to EXTENSION / LOGIC


        ///// <summary>
        ///// Determines whether [has role access] [the specified role ID].
        ///// </summary>
        ///// <param name="user">The user.</param>
        ///// <returns>
        ///// 	<c>true</c> if [has role access] [the specified role ID]; otherwise, <c>false</c>.
        ///// </returns>
        //public bool HasRoleAccess(Sushi.Mediakiwi.Data.IApplicationUser user)
        //{
        //    if (this.ID == 0 || user.Role().All_Sites) return true;
        //    var selection = from item in SelectAllAccessible(user, AccessFilter.RoleAndUser) where item.ID == this.ID select item;
        //    bool xs = selection.Count() == 1;
        //    return xs;
        //}

        //void CreateSiteFolders(int siteID)
        //{
        //    Sushi.Mediakiwi.Data.Folder webFolder = new Sushi.Mediakiwi.Data.Folder();
        //    webFolder.Type = Sushi.Mediakiwi.Data.FolderType.Page;
        //    webFolder.Name = Sushi.Mediakiwi.Data.Common.FolderRoot;
        //    webFolder.CompletePath = Sushi.Mediakiwi.Data.Common.FolderRoot;
        //    webFolder.SiteID = siteID;
        //    webFolder.Save();

        //    Sushi.Mediakiwi.Data.Folder logicFolder = new Sushi.Mediakiwi.Data.Folder();
        //    logicFolder.Type = Sushi.Mediakiwi.Data.FolderType.List;
        //    logicFolder.Name = Sushi.Mediakiwi.Data.Common.FolderRoot;
        //    logicFolder.CompletePath = Sushi.Mediakiwi.Data.Common.FolderRoot;
        //    logicFolder.SiteID = siteID;
        //    logicFolder.Save();
        //}


        #endregion MOVED to EXTENSION / LOGIC

        #region REPLICATED TO Sushi.Mediakiwi.Data.Standard

        /// <summary>
        /// Does this site have inheritance children?
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has children; otherwise, <c>false</c>.
        /// </value>
        public bool HasChildren
        {
            get { return m_ChildCount > 0; }
        }

        int m_ChildCount;
        /// <summary>
        /// The amount of children (inherited sites) that this site has
        /// </summary>
        /// <value>The child count.</value>
        [DatabaseColumn("Site_ChildCount", SqlDbType.Int, IsOnlyRead = true,
            ColumnSubQuery = "select count(*) from wim_Sites site where site.Site_Master_Key = wim_Sites.Site_Key")]
        public int ChildCount
        {
            get { return m_ChildCount; }
            set { m_ChildCount = value; }
        }

        int m_PageCount;
        /// <summary>
        /// Gets or sets the page count.
        /// </summary>
        /// <value>The page count.</value>
        [DatabaseColumn("Site_PageCount", SqlDbType.Int, IsOnlyRead = true,
            ColumnSubQuery = "select count(*) from wim_Pages join wim_Folders on Page_Folder_Key = Folder_Key where Folder_Site_Key = Site_Key")]
        public int PageCount
        {
            get { return m_PageCount; }
            set { m_PageCount = value; }
        }

        int m_ListCount;
        /// <summary>
        /// Gets or sets the list count.
        /// </summary>
        /// <value>The list count.</value>
        [DatabaseColumn("Site_ListCount", SqlDbType.Int, IsOnlyRead = true,
            ColumnSubQuery = "select count(*) from wim_ComponentLists where ComponentList_Site_Key = Site_Key")]
        public int ListCount
        {
            get { return m_ListCount; }
            set { m_ListCount = value; }
        }

        /// <summary>
        /// Delete an implementation record.
        /// </summary>
        /// <returns></returns>
        public override bool Delete()
        {
            if (this.HasPages || this.HasLists)
                throw new Exception("Before a site can be deleted the HasPages and HasLists should be False");

            Execute(string.Format(@"
delete from wim_Components where Component_Page_Key in 
    (select Page_Key from wim_Pages where Page_Folder_Key in 
        (select Folder_Key from wim_Folders where Folder_Site_Key = {0})
    )", this.ID));

            Execute(string.Format(@"
delete from wim_ComponentVersions where ComponentVersion_Page_Key in
    (select Page_Key from wim_Pages where Page_Folder_Key in
	    (select Folder_Key from wim_Folders where Folder_Site_Key = {0})
    )", this.ID));

            Execute(string.Format(@"
    delete from wim_Pages where Page_Folder_Key in (
	    select Folder_Key from wim_Folders where Folder_Site_Key = {0}
    )", this.ID));

            Execute(string.Format("delete from wim_RoleRights where RoleRight_Child_Key = {0} and RoleRight_Child_Type = 2", this.ID));
            Execute(string.Format("update wim_ComponentLists set ComponentList_Site_Key = null, ComponentList_Folder_Key = null where ComponentList_Site_Key = {0}", this.ID));
            //Execute(string.Format("update wim_ComponentLists set ComponentList_Folder_Key = null where ComponentList_Folder_Key in (select Folder_Key from wim_Folders where Folder_Site_Key = {0})", this.ID));
            
            // [26-09-2008:MM] Intersystems cache cannot recognize parent child relations in one table.
            foreach(Sushi.Mediakiwi.Data.Folder folder in Sushi.Mediakiwi.Data.Folder.SelectAllForDeletion(this.ID))
                folder.Delete();

            //Execute(string.Format("delete from wim_Folders where Folder_Site_Key = {0}", this.ID));
            return base.Delete();
        }

        /// <summary>
        /// Select a Site based on its primary key
        /// </summary>
        public static Site SelectOne(int ID)
        {
            return SelectOne(ID, false);
        }

        /// <summary>
        /// Select a Site based on its primary key
        /// </summary>
        /// <param name="ID">The ID.</param>
        /// <param name="forceLoad">if set to <c>true</c> [force load].</param>
        /// <returns></returns>
        public static Site SelectOne(int ID, bool forceLoad)
        {
            return (Site)new Site()._SelectOne(ID);
        }

        /// <summary>
        /// Get a site based on its unique key
        /// </summary>
        /// <param name="siteGUID">The site GUID.</param>
        /// <returns></returns>
        public static Site SelectOne(Guid siteGUID)
        {
            List<DatabaseDataValueColumn> where = new List<DatabaseDataValueColumn>();
            where.Add(new DatabaseDataValueColumn("Site_Guid", SqlDbType.UniqueIdentifier, siteGUID));
            return (Site)new Site()._SelectOne(where);
        }

 

        string m_Name;
        /// <summary>
        /// Name of this site
        /// </summary>
        /// <value>The name.</value>
        [Sushi.Mediakiwi.Framework.ContentListItem.TextField("Title", 100, true)]
        [DatabaseColumn("Site_Displayname", SqlDbType.NVarChar, Length = 100)]
        public string Name
        {
            get {
                if (m_Name == "[internal]" && this.Type.GetValueOrDefault(0) == 1)
                    m_Name = "Administration";
                return m_Name; 
            }
            set { m_Name = value; }
        }


        string[] m_Domains;
        /// <summary>
        /// Gets the domains.
        /// </summary>
        /// <value>The domains.</value>
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
        /// Gets the domain prefix.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns></returns>
        public static string GetDomainPrefix(System.Uri url)
        {
            Site site;
            return GetDomainPrefix(url, out site);
        }

        /// <summary>
        /// Gets the domain prefix.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="siteMatch">The site match.</param>
        /// <returns></returns>
        public static string GetDomainPrefix(System.Uri url, out Site siteMatch)
        {
            foreach(Site site in SelectAll())
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
        /// Determines whether the specified URL is domain.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns>
        /// 	<c>true</c> if the specified URL is domain; otherwise, <c>false</c>.
        /// </returns>
        public bool IsDomain(System.Uri url)
        {
            return IsDomain(url, null);
        }

        /// <summary>
        /// Determines whether the specified URL is domain.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="defaultFolder">The default folder.</param>
        /// <returns>
        /// 	<c>true</c> if the specified URL is domain; otherwise, <c>false</c>.
        /// </returns>
        public bool IsDomain(System.Uri url, string defaultFolder)
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

        int? m_CountryID;
        /// <summary>
        /// Country identifier of the site
        /// </summary>
        [Sushi.Mediakiwi.Framework.ContentListItem.Choice_Dropdown("Country", "AvailableCountries", false)]
        [DatabaseColumn("Site_Country", SqlDbType.Int, IsNullable = true)]
        public int? CountryID
        {
            get { return m_CountryID; }
            set { m_CountryID = value; }
        }

        ICountry m_Country;
        /// <summary>
        /// Corresponding site country
        /// </summary>
        public ICountry Country
        {
            get
            {
                if (m_Country == null && m_CountryID.HasValue) m_Country = Sushi.Mediakiwi.Data.Country.SelectOne(m_CountryID.Value);
                return m_Country;
            }
        }

        string m_Language;
        /// <summary>
        /// Language of this site
        /// </summary>
        /// <value>The language.</value>
        [Sushi.Mediakiwi.Framework.ContentListItem.Choice_Dropdown("Language", "AvailableCulturesCollection", false)]
        [DatabaseColumn("Site_Language", SqlDbType.VarChar, Length = 8, IsNullable = true)]
        public string Language
        {
            get { return m_Language; }
            set { m_Language = value; }
        }

        string m_TimeZoneIndex;
        /// <summary>
        /// TimeZoneIndex
        /// </summary>
        /// <value>The index of the time zone.</value>
        [Sushi.Mediakiwi.Framework.ContentListItem.Choice_Dropdown("Timezone", "AvailableTimeZones", false)]
        [DatabaseColumn("Site_TimeZone", SqlDbType.NVarChar, Length = 50, IsNullable = true)]
        public string TimeZoneIndex
        {
            get { return m_TimeZoneIndex; }
            set { m_TimeZoneIndex = value; }
        }

        System.TimeZoneInfo m_TimeZone;
        /// <summary>
        /// TimeZone information
        /// </summary>
        /// <value>The time zone.</value>
        public System.TimeZoneInfo TimeZone
        {
            get
            {
                if (m_TimeZone == null)
                {
                    try
                    {
                        if (string.IsNullOrEmpty(m_TimeZoneIndex))
                            m_TimeZone = System.TimeZoneInfo.Local;
                        else
                            m_TimeZone = System.TimeZoneInfo.FindSystemTimeZoneById(m_TimeZoneIndex);// Wim.Utilities.TimeZoneInformation.FromIndex(m_TimeZoneIndex);
                    }
                    catch (Exception ex)
                    {
                        m_TimeZone = System.TimeZoneInfo.Local;
                        Sushi.Mediakiwi.Data.Notification.InsertOne("Timezone", ex);
                    }
                }
                return m_TimeZone;
            }
        }
        /*
        Wim.Utilities.TimeZoneInformation m_TimeZone;
        /// <summary>
        /// TimeZone information
        /// </summary>
        /// <value>The time zone.</value>
        public Wim.Utilities.TimeZoneInformation TimeZone
        {
            get
            {
                if (string.IsNullOrEmpty(m_TimeZoneIndex))
                    m_TimeZone = Wim.Utilities.TimeZoneInformation.CurrentTimeZone;
                else
                    m_TimeZone = Wim.Utilities.TimeZoneInformation.FromIndex(m_TimeZoneIndex);

                return m_TimeZone;
            }
        }
         */

        string m_Culture;
        /// <summary>
        /// Culture of this site
        /// </summary>
        /// <value>The culture.</value>
        [Sushi.Mediakiwi.Framework.ContentListItem.Choice_Dropdown("Culture", "AvailableCulturesCollection", false)]
        [DatabaseColumn("Site_Culture", SqlDbType.VarChar, Length = 15, IsNullable = true)]
        public string Culture
        {
            get { return ApplyCultureFix(m_Culture); }
            set { m_Culture = value; }
        }

        public string CultureName
        {
            get { 
                var cult =  new System.Globalization.CultureInfo(Culture);
                if (cult != null && cult.DisplayName != null)
                {
                    if (cult.DisplayName.IndexOf('(') > 0)
                        return cult.DisplayName.Substring(0 , cult.DisplayName.IndexOf('(') - 1);
                    else 
                        return cult.DisplayName;

                }
                return "";
            }
        }


        int? m_MasterID;
        /// <summary>
        /// The site from which this site inherites all its content
        /// </summary>
        [Sushi.Mediakiwi.Framework.OnlyEditableWhenTrue("InheritenceIsSet", false)]
        [Sushi.Mediakiwi.Framework.ContentListItem.Choice_Dropdown("Inherit from", "AvailableSitesCollection", false, "Site to inherit content from. <b>Important note</b>: <u>When chosen this can not be changed!<u>")]
        [DatabaseColumn("Site_Master_Key", SqlDbType.Int, IsNullable = true)]
        public int? MasterID
        {
            get { return m_MasterID; }
            set { m_MasterID = value; }
        }

        bool m_AutoPublishInherited;
        /// <summary>
        /// Gets or sets a value indicating whether auto publish inherited content is on. When on this copies the Page_IsPublished and Page_CustomDate.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [auto publish inherited]; otherwise, <c>false</c>.
        /// </value>
        [Sushi.Mediakiwi.Framework.ContentListItem.Choice_Checkbox("Auto-Publish inherited content")]
        [DatabaseColumn("Site_AutoPublishInherited", SqlDbType.Bit)]
        public bool AutoPublishInherited
        {
            get { return m_AutoPublishInherited; }
            set { m_AutoPublishInherited = value; }
        }

        bool m_IsActive;
        /// <summary>
        /// Is this site active (visible)?
        /// </summary>
        /// <value><c>true</c> if this instance is active; otherwise, <c>false</c>.</value>
        [Sushi.Mediakiwi.Framework.ContentListItem.Choice_Checkbox("Is active", "Is this site active (visible)?")]
        [DatabaseColumn("Site_IsActive", SqlDbType.Bit)]
        public bool IsActive
        {
            get { return m_IsActive; }
            set { m_IsActive = value; }
        }

        int? m_Type;
        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        [DatabaseColumn("Site_Type", SqlDbType.Int, IsNullable = true)]
        public int? Type
        {
            get { return m_Type; }
            set { m_Type = value; }
        }

        bool m_HasPages;
        /// <summary>
        /// Does this site have pages?
        /// </summary>
        /// <value><c>true</c> if this instance has pages; otherwise, <c>false</c>.</value>
        [Sushi.Mediakiwi.Framework.ContentListItem.Choice_Checkbox("Has pages", "Does this site have content pages?")]
        [DatabaseColumn("Site_HasPages", SqlDbType.Bit)]
        public bool HasPages
        {
            get { return m_HasPages; }
            set { m_HasPages = value; }
        }

        bool m_HasLists;
        /// <summary>
        /// Does this site have lists?
        /// </summary>
        /// <value><c>true</c> if this instance has lists; otherwise, <c>false</c>.</value>
        [Sushi.Mediakiwi.Framework.ContentListItem.Choice_Checkbox("Has lists", "Does this site have lists?")]
        [DatabaseColumn("Site_HasLists", SqlDbType.Bit)]
        public bool HasLists
        {
            get { return m_HasLists; }
            set { m_HasLists = value; }
        }

        DateTime m_Created;
        /// <summary>
        /// The creation date/time (UTC) of this site
        /// </summary>
        [Sushi.Mediakiwi.Framework.ContentListItem.TextLine("Created")]
        [DatabaseColumn("Site_Created", SqlDbType.DateTime)]
        public DateTime Created
        {
            get
            {
                if (this.m_Created == DateTime.MinValue) this.m_Created = Sushi.Mediakiwi.Data.Common.DatabaseDateTime;
                return m_Created;
            }
            set { m_Created = value; }
        }


        string m_DefaultPageTitle;
        /// <summary>
        /// The default page title of the pages within this site
        /// </summary>
        [Sushi.Mediakiwi.Framework.ContentListItem.TextField("Default page title", 100, false, "The default browser title")]
        [DatabaseColumn("Site_DefaultTitle", SqlDbType.NVarChar, Length = 100, IsNullable = true)]
        public string DefaultPageTitle
        {
            get { return m_DefaultPageTitle; }
            set { m_DefaultPageTitle = value; }
        }

        string m_DefaultFolder;
        /// <summary>
        /// The default startup folder of this site
        /// </summary>
        /// <value>The default folder.</value>
        [Sushi.Mediakiwi.Framework.ContentListItem.TextField("Default folder", 50, false, "Default deeplink path (never end with a /)")]
        [DatabaseColumn("Site_DefaultFolder", SqlDbType.VarChar, Length = 50, IsNullable = true)]
        public string DefaultFolder
        {
            get { return m_DefaultFolder; }
            set { m_DefaultFolder = value; }
        }

        /// <summary>
        /// Gets or sets the domain.
        /// </summary>
        /// <value>The domain.</value>
        [Sushi.Mediakiwi.Framework.ContentListItem.TextField("Domains", 255, false, "Apply all channel specific domains comma seperated.")]
        [DatabaseColumn("Site_Domain", SqlDbType.VarChar, Length = 255, IsNullable = true)]
        public string Domain { get; set; }

        int? m_HomepageID;
        /// <summary>
        /// The page identifier of the homepage
        /// </summary>
        [Sushi.Mediakiwi.Framework.ContentListItem.PageSelect("Homepage", false)]
        [Sushi.Mediakiwi.Framework.OnlyVisibleWhenTrue("IsNewElement", false)]
        [DatabaseColumn("Site_HomePage_Key", SqlDbType.Int, IsNullable = true)]
        public int? HomepageID
        {
            get { return m_HomepageID; }
            set { m_HomepageID = value; }
        }

        private int? m_PageNotFoundID;
        /// <summary>
        /// The page identifier of the page not found page
        /// </summary>
        [Sushi.Mediakiwi.Framework.OnlyVisibleWhenTrue("IsNewElement", false)]
        [Sushi.Mediakiwi.Framework.ContentListItem.PageSelect("Not found page", false, "When a page doesn't exists or isn't published yet/anymore this page is called")]
        [DatabaseColumn("Site_PageNotFoundPage_Key", SqlDbType.Int, IsNullable = true)]
        public int? PageNotFoundID
        {
            get { return m_PageNotFoundID; }
            set { m_PageNotFoundID = value; }
        }

        int? m_ErrorPageID;
        /// <summary>
        /// The page identifier of the error page
        /// </summary>
        [Sushi.Mediakiwi.Framework.ContentListItem.PageSelect("Error page", false, "When an error occurs this page is called")]
        [Sushi.Mediakiwi.Framework.OnlyVisibleWhenTrue("IsNewElement", false)]
        [DatabaseColumn("Site_ErrorPage_Key", SqlDbType.Int, IsNullable = true)]
        public int? ErrorPageID
        {
            get { return m_ErrorPageID; }
            set { m_ErrorPageID = value; }
        }

        int m_ID;
        /// <summary>
        /// Uniqe identifier of the site
        /// </summary>
		[DatabaseColumn("Site_Key", SqlDbType.Int, IsPrimaryKey=true)]
        public int ID 
		{
            get { return m_ID; }
            set { m_ID = value; }
		}

        internal Guid m_GUID;
        /// <summary>
        /// Unique identifier used for import/export
        /// </summary>
        [DatabaseColumn("Site_Guid", SqlDbType.UniqueIdentifier, IsMigrationKey = true)]
        public Guid GUID
        {
            get {
                if (this.m_GUID == Guid.Empty) this.m_GUID = Guid.NewGuid();
                return m_GUID; }
            set { m_GUID = value; }
        }


        string m_Master;
        /// <summary>
        /// The name of the site from which this site inherites all its content
        /// </summary>
        /// <value>The master.</value>
        [DatabaseColumn("Site_Master", SqlDbType.NVarChar, Length = 250, IsNullable = true,
          ColumnSubQuery = "select Site_Displayname from wim_Sites parent where parent.Site_Key = wim_Sites.Site_Master_Key")]
        public string Master
        {
            get { return m_Master; }
            set { m_Master = value; }
        }

        /// <summary>
        /// Applies the culture fix.
        /// </summary>
        /// <param name="culture">The culture.</param>
        /// <returns></returns>
        string ApplyCultureFix(string culture)
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
        /// Selects the one by page.
        /// </summary>
        /// <param name="pageID">The page ID.</param>
        /// <returns></returns>
        public static Site SelectOneByPage(int pageID)
        {
            Site implement = new Site();
            implement.SqlJoin = "left join wim_Sites parent on parent.Site_Key = wim_Sites.Site_Master_Key join wim_Folders on Folder_Site_Key = wim_Sites.Site_Key join wim_Pages on Page_Folder_Key = Folder_Key";

            List<DatabaseDataValueColumn> list = new List<DatabaseDataValueColumn>();
            list.Add(new DatabaseDataValueColumn("Page_key", SqlDbType.Int, pageID));
            return (Site)implement._SelectOne(list);
        }

        /// <summary>
        /// Select a site based on a folder identifier
        /// </summary>
        /// <param name="folderID">The folder ID.</param>
        /// <returns>Site object</returns>
        public static Site SelectOneByFolder(int folderID)
        {
            Site implement = new Site();
            implement.SqlJoin = "join wim_Folders on Folder_Site_Key = wim_Sites.Site_Key left join wim_Sites parent on parent.Site_Key = wim_Sites.Site_Master_Key";

            List<DatabaseDataValueColumn> list = new List<DatabaseDataValueColumn>();
            list.Add(new DatabaseDataValueColumn("Folder_Key", SqlDbType.Int, folderID));
            return (Site)implement._SelectOne(list);
        } 

        /// <summary>
        /// Select a site based on a relative folder path (tries to match this with the default folder).
        /// </summary>
        /// <param name="searchPath">Relative search path</param>
        /// <returns>Site object</returns>
        /// [MR:06-11-2013] This is created for use in the HttpRewrite class.
        /// [CB:01-07-2015]  added the exclude admin to omit the adminstration site
        public static Site SelectOneSiteResolution(string searchPath, bool excludeAdmin =false)
        {
            Site implement = new Site();
            implement.SqlOrder = "Site_DefaultFolder DESC";
            List<DatabaseDataValueColumn> list = new List<DatabaseDataValueColumn>();

            if (string.IsNullOrEmpty(searchPath))
                list.Add(new DatabaseDataValueColumn("Site_DefaultFolder is null"));
            else
                list.Add(new DatabaseDataValueColumn(string.Format("Site_DefaultFolder + '/' like '{0}%'", searchPath)));
            if (excludeAdmin)
                list.Add(new DatabaseDataValueColumn("site_displayname != 'Administration'"));
            return (Site)implement._SelectOne(list);
        }

        /// <summary>
        /// Select a site based on a relative folder path (tries to match this with the default folder).
        /// </summary>
        /// <param name="searchPath">Relative search path</param>
        /// <returns>Site object</returns>
        public static Site SelectOne(string searchPath)
        {
            Site implement = new Site();
            implement.SqlOrder = "Site_DefaultFolder DESC";
            List<DatabaseDataValueColumn> list = new List<DatabaseDataValueColumn>();

            if (string.IsNullOrEmpty(searchPath))
                list.Add(new DatabaseDataValueColumn("Site_DefaultFolder", SqlDbType.NVarChar, null));
            else
                list.Add(new DatabaseDataValueColumn("Site_DefaultFolder", SqlDbType.NVarChar, string.Concat(searchPath, "%"), DatabaseDataValueCompareType.Like));

            return (Site)implement._SelectOne(list);
        }

        /// <summary>
        /// Selects the base.
        /// </summary>
        /// <returns></returns>
        static Site SelectBase()
        {
            var selection = (from item in SelectAll() where item.Type.HasValue && item.Type == 1 select item).ToArray();
            return selection.Count() == 1 ? selection[0] : new Site();
        }

        /// <summary>
        /// Selects all active.
        /// </summary>
        /// <returns></returns>
        internal static Site[] SelectAllActive()
        {
            Site implement = new Site();

            List<DatabaseDataValueColumn> whereClause = new List<DatabaseDataValueColumn>();
            whereClause.Add(new DatabaseDataValueColumn("Site_IsActive", SqlDbType.Bit, true));
            whereClause.Add(new DatabaseDataValueColumn("Site_Type", SqlDbType.Int, null));

            List<Site> list = new List<Site>();
            foreach (object o in implement._SelectAll(whereClause))
                list.Add((Site)o);
            return list.ToArray();
        } 

        /// <summary>
        /// Select all available sites
        /// </summary>
        /// <returns>Array of site objects</returns>
        public static Site[] SelectAll()
        {
            List<Site> list = new List<Site>();
            List<DatabaseDataValueColumn> whereClause = new List<DatabaseDataValueColumn>();
            //whereClause.Add(new DatabaseDataValueColumn("Site_Type", SqlDbType.Int, null));
            whereClause.Add(new DatabaseDataValueColumn("ISNULL(Site_Type, 1) = 1"));
            
            foreach (object o in new Site()._SelectAll(whereClause, false, "Sites", "ALL"))
                list.Add((Site)o);
            return list.ToArray();
        }

        /// <summary>
        /// Select all available sites based on part of the title
        /// </summary>
        /// <returns>Array of site objects</returns>
        public static Sushi.Mediakiwi.Data.Site[] SelectAll(string title)
        {
            return SelectAll(title, FolderType.Page);
        }

        /// <summary>
        /// Select all available sites based on part of the title
        /// </summary>
        /// <returns>Array of site objects</returns>
        public static Sushi.Mediakiwi.Data.Site[] SelectAll(string title, Sushi.Mediakiwi.Data.FolderType type)
        {
            Site implement = new Site();
            implement.SqlOrder = "Site_Displayname ASC";
            implement.SqlJoin = "join wim_Folders on Site_Key = Folder_Site_Key";

            List<DatabaseDataValueColumn> whereClause = new List<DatabaseDataValueColumn>();
            whereClause.Add(new DatabaseDataValueColumn("Folder_Type", SqlDbType.Int, (int)type));
            whereClause.Add(new DatabaseDataValueColumn("Folder_Folder_Key", SqlDbType.Int, null));
            whereClause.Add(new DatabaseDataValueColumn("Site_Displayname", SqlDbType.NVarChar, title));
            whereClause.Add(new DatabaseDataValueColumn("Site_Type", SqlDbType.Int, null));

            List<Site> list = new List<Site>();
            foreach (object o in implement._SelectAll(whereClause))
                list.Add((Site)o);
            return list.ToArray();
        }

        #endregion REPLICATED TO Sushi.Mediakiwi.Data.Standard
    }

}
