using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Data.SqlClient;
using Sushi.Mediakiwi.Data.DalReflection;
using Sushi.Mediakiwi.Framework;
using HtmlAgilityPack;
namespace Sushi.Mediakiwi.Data
{


    /// <summary>
    /// Represents a ComponentVersion entity.
    /// </summary>
    [DatabaseTable("wim_ComponentVersions",
        Join = "join wim_ComponentTemplates on ComponentVersion_ComponentTemplate_Key = ComponentTemplate_Key",
        Order = "ComponentTemplate_IsHeader DESC, ComponentTemplate_IsFooter ASC, ComponentVersion_SortOrder ASC")
    ]
    public class ComponentVersion : DatabaseEntity, iExportable, IComponent
    {
        [DatabaseColumn("ComponentTarget_Component_Target", SqlDbType.UniqueIdentifier, CollectionLevel = DatabaseColumnGroup.Additional, IsOnlyRead = true)]
        public Guid ComponentTarget { get; set; }  // REVIEW MARC, dit zou een extra join betekenen

        #region Commented, Not used anymore


        ///// <summary>
        ///// Selects all on page_ import export.
        ///// </summary>
        ///// <param name="portal">The portal.</param>
        ///// <param name="pageID">The page ID.</param>
        ///// <returns></returns>
        //public static Sushi.Mediakiwi.Data.ComponentVersion[] SelectAllOnPage_ImportExport(string portal, int pageID)       //WEG
        //{
        //    ComponentVersion implement = new ComponentVersion();

        //    implement.SqlConnectionString = Sushi.Mediakiwi.Data.Common.GetPortal(portal).Connection;
        //    implement.ConnectionType = Sushi.Mediakiwi.Data.Common.GetPortal(portal).Type;

        //    List<DatabaseDataValueColumn> whereClause = new List<DatabaseDataValueColumn>();
        //    whereClause.Add(new DatabaseDataValueColumn("ComponentVersion_Page_Key", SqlDbType.Int, pageID));
        //    whereClause.Add(new DatabaseDataValueColumn("ComponentVersion_IsAlive", SqlDbType.Bit, true));

        //    List<ComponentVersion> list = new List<ComponentVersion>();
        //    foreach (object o in implement._SelectAll(whereClause))
        //        list.Add((ComponentVersion)o);
        //    return list.ToArray();
        //}

        #endregion Commented, Not used anymore

        #region MOVED to EXTENSION / LOGIC

        ///// <summary>
        ///// save overwrite. this implementation can also flush the cache automaticly
        ///// </summary>
        ///// <param name="doFlush">Default is set to true to flush the cache after insert or update</param>
        ///// <returns></returns>
        //public bool Save(bool doFlush = true)           //NAAR LOGIC, INTERNALS
        //{
        //    bool save;

        //    //if (this.TemplateIsShared && this.PageID > 0)
        //    //{
        //    //    ComponentVersion sharedVersion = ComponentVersion.SelectOneShared(this.TemplateID);
        //    //    sharedVersion.Serialized_XML = this.Serialized_XML;
        //    //    sharedVersion.TemplateID = this.TemplateID;
        //    //    sharedVersion.IsActive = true;
        //    //    sharedVersion.IsAlive = true;
        //    //    sharedVersion.Name = this.Name;
        //    //    sharedVersion.Save();

        //    //    int id = sharedVersion.ID;
        //    //}

        //    this.Updated = DateTime.Now;

        //    if (PrimairyKeyValue.HasValue && PrimairyKeyValue != 0)
        //        save = Update();
        //    else
        //        save = Insert();

        //    if (doFlush)
        //    {
        //        Wim.Utilities.CacheItemManager.FlushIndexOfCacheObjects(string.Concat("Data_", this.GetType().ToString()));
        //    }
        //    if (System.Web.HttpContext.Current != null && System.Web.HttpContext.Current.Items["wim.Saved.ID"] == null)
        //        System.Web.HttpContext.Current.Items["wim.Saved.ID"] = PrimairyKeyValue;


        //    return save;
        //}

        ///// <summary>
        ///// Copies from master.
        ///// </summary>
        ///// <param name="pageID">The page ID.</param>
        //public static void CopyFromMaster(int pageID)       //NAAR LOGIC, INTERNALS
        //{
        //    Sushi.Mediakiwi.Data.Page page = Sushi.Mediakiwi.Data.Page.SelectOne(pageID);
        //    Sushi.Mediakiwi.Data.Page masterPage = Sushi.Mediakiwi.Data.Page.SelectOne(page.MasterID.GetValueOrDefault());

        //    ComponentVersion[] versions = SelectAll(pageID);
        //    ComponentVersion[] masterVersions = SelectAll(page.MasterID.GetValueOrDefault());

        //    foreach (ComponentVersion version in versions)
        //    {
        //        ComponentVersion master = null;
        //        foreach (ComponentVersion item in masterVersions)
        //        {
        //            if (item.ID == version.MasterID)
        //            {
        //                master = item;
        //                break;
        //            }
        //        }
        //        if (master == null) continue;

        //        Content content = master.GetContent();
        //        version.Serialized_XML = master.Serialized_XML;

        //        if (content != null && content.Fields != null)
        //        {
        //            foreach (Content.Field field in content.Fields)
        //            {
        //                if (string.IsNullOrEmpty(field.Value) || field.Value == "0")
        //                    continue;

        //                if (field.Type == (int)Sushi.Mediakiwi.Framework.ContentType.RichText)
        //                {
        //                    string candidate = field.Value;
        //                    Sushi.Mediakiwi.Framework.ContentInfoItem.RichTextLink.CreateLinkMasterCopy(ref candidate, page.SiteID);
        //                    field.Value = candidate;
        //                }
        //                else if (field.Type == (int)Sushi.Mediakiwi.Framework.ContentType.FolderSelect)
        //                {
        //                    Sushi.Mediakiwi.Data.Folder folderInstance = Sushi.Mediakiwi.Data.Folder.SelectOneChild(Wim.Utility.ConvertToInt(field.Value), page.SiteID);
        //                    field.Value = folderInstance.ID.ToString();
        //                }
        //                else if (field.Type == (int)Sushi.Mediakiwi.Framework.ContentType.Hyperlink)
        //                {
        //                    Sushi.Mediakiwi.Data.Link link = Sushi.Mediakiwi.Data.Link.SelectOne(Wim.Utility.ConvertToInt(field.Value));
        //                    if (link != null && !link.IsNewInstance)
        //                    {
        //                        if (link.Type == Sushi.Mediakiwi.Data.Link.LinkType.InternalPage)
        //                        {
        //                            Sushi.Mediakiwi.Data.Page pageInstance = Sushi.Mediakiwi.Data.Page.SelectOneChild(link.PageID.Value, page.SiteID, false);
        //                            if (page != null)
        //                            {
        //                                link.ID = 0;
        //                                link.PageID = pageInstance.ID;
        //                                link.Save();
        //                                field.Value = link.ID.ToString();
        //                            }
        //                        }
        //                        else
        //                        {
        //                            link.ID = 0;
        //                            link.Save();
        //                            field.Value = link.ID.ToString();
        //                        }
        //                    }
        //                }
        //                else if (field.Type == (int)Sushi.Mediakiwi.Framework.ContentType.PageSelect)
        //                {
        //                    Sushi.Mediakiwi.Data.Page pageInstance = Sushi.Mediakiwi.Data.Page.SelectOneChild(Wim.Utility.ConvertToInt(field.Value), page.SiteID, false);
        //                    field.Value = pageInstance.ID.ToString();
        //                }
        //            }
        //            version.Serialized_XML = Sushi.Mediakiwi.Data.Content.GetSerialized(content);
        //        }
        //        else
        //            version.Serialized_XML = null;
        //        version.Save();
        //    }
        //}

        //public void Publish()
        //{
        //    try
        //    {
        //        var component = Sushi.Mediakiwi.Data.Component.SelectOne(this.GUID);
        //        Apply(component);
        //        component.Save();

        //        if (HttpContext.Current != null)
        //            Sushi.Mediakiwi.Data.EnvironmentVersionLogic.Flush();
        //    }
        //    catch (Exception ex)
        //    {
        //        this.Name = ex.Message;
        //    }
        //}

        //public void TakeDown()
        //{
        //    try
        //    {
        //        var published = Sushi.Mediakiwi.Data.Component.SelectOne(this.GUID);
        //        published.Delete();

        //        if (HttpContext.Current != null)
        //            Sushi.Mediakiwi.Data.EnvironmentVersionLogic.Flush();
        //    }
        //    catch (Exception ex)
        //    {
        //        this.Name = ex.Message;
        //    }
        //}


        #endregion MOVED to EXTENSION / LOGIC

        #region REPLICATED TO Sushi.Mediakiwi.Data.Standard

        public static Sushi.Mediakiwi.Data.ComponentVersion[] SelectAllShared1(int pageID)
        {
            return SelectAllShared1(pageID, false);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageID"></param>
        /// <returns></returns>
        public static Sushi.Mediakiwi.Data.ComponentVersion[] SelectAllShared1(int pageID, bool onlyAlive)      //CACHING WEG, ZIE COMPONENT
        {
            ComponentVersion component = new ComponentVersion();
            List<ComponentVersion> list = new List<ComponentVersion>();

            Page page = Page.SelectOne(pageID, false);
            //  If the page is set to hold inherited content, please take that content
            if (page.InheritContent && page.MasterID.HasValue)
                pageID = page.MasterID.Value;

            component.CollectionLevel = DatabaseColumnGroup.Additional;
            component.SqlJoin += " join wim_ComponentTargets on ComponentVersion_GUID = ComponentTarget_Component_Source";

            List<DatabaseDataValueColumn> whereClause = new List<DatabaseDataValueColumn>();
            whereClause.Add(new DatabaseDataValueColumn("ComponentTarget_Page_Key", SqlDbType.Int, pageID));
            whereClause.Add(new DatabaseDataValueColumn("ComponentTemplate_IsShared", SqlDbType.Bit, true));
            if (onlyAlive)
                whereClause.Add(new DatabaseDataValueColumn("ComponentVersion_IsAlive", SqlDbType.Bit, true));

            if (!string.IsNullOrEmpty(SqlConnectionString2)) component.SqlConnectionString = SqlConnectionString2;
            foreach (object o in component._SelectAll(whereClause, false, "PageShared2", pageID.ToString()))
            {
                ComponentVersion c = (ComponentVersion)o;
                //  When the content is set to inherit please apply the child page ID
                if (page.InheritContent && page.MasterID.HasValue) c.PageID = page.ID;

                list.Add(c);
            }
            return list.ToArray();
        }

        /// <summary>
        /// Gets the content.
        /// </summary>
        /// <returns></returns>
        public Sushi.Mediakiwi.Data.Content GetContent()
        {
            if (Serialized_XML == null || Serialized_XML.Trim().Length == 0) return null;
            return Sushi.Mediakiwi.Data.Content.GetDeserialized(Serialized_XML);
        }

        /// <summary>
        /// Remove the components versions from a page if they are not assigned any more.
        /// </summary>
        /// <param name="pageID">The page ID.</param>
        internal static void RemoveInvalidPageReference(int pageID)
        {
            ComponentVersion implement = new ComponentVersion();
            implement.Execute(string.Format(@"
 delete from wim_ComponentVersions where ComponentVersion_Key in (
 select ComponentVersion_Key from  
	wim_ComponentVersions 
	left join wim_AvailableTemplates on AvailableTemplates_Key = ComponentVersion_AvailableTemplate_Key 
        and (AvailableTemplates_Target = ComponentVersion_Target or (AvailableTemplates_Target is null and ComponentVersion_Target is null) )
 where 
	ComponentVersion_Page_Key = {0}
	and AvailableTemplates_Key is null and ComponentVersion_IsFixedOnTemplate = 1)
"
                , pageID));

            //  [20090127:MM] : and AvailableTemplates_IsSecundary = ComponentVersion_IsSecundary
        }


        /// <summary>
        /// Updates the container target by available template.
        /// </summary>
        /// <param name="availableTemplateID">The available template ID.</param>
        /// <param name="target">The target.</param>
        internal static void UpdateContainerTargetByAvailableTemplate(int availableTemplateID, string target)
        {
            ComponentVersion implement = new ComponentVersion();
            if (string.IsNullOrEmpty(target))
                implement.Execute(string.Format(@"update wim_ComponentVersions set ComponentVersion_Target = null where ComponentVersion_AvailableTemplate_Key = {0}", availableTemplateID, target));
            else
                implement.Execute(string.Format(@"update wim_ComponentVersions set ComponentVersion_Target = '{1}' where ComponentVersion_AvailableTemplate_Key = {0}", availableTemplateID, target));
        }


        /// <summary>
        /// Selects all.
        /// </summary>
        /// <returns></returns>
        public static ComponentVersion[] SelectAll()
        {
            List<ComponentVersion> list = new List<ComponentVersion>();
            ComponentVersion componentVersion = new ComponentVersion();
            if (!string.IsNullOrEmpty(SqlConnectionString2)) componentVersion.SqlConnectionString = SqlConnectionString2;
            foreach (object o in componentVersion._SelectAll())
                list.Add((ComponentVersion)o);
            return list.ToArray();
        }

        /// <summary>
        /// Selects all.
        /// </summary>
        /// <param name="pageID">The page ID.</param>
        /// <returns></returns>
        public static ComponentVersion[] SelectAll(int pageID, string target = null)
        {
            List<ComponentVersion> list = new List<ComponentVersion>();
            ComponentVersion componentVersion = new ComponentVersion();

            List<DatabaseDataValueColumn> where = new List<DatabaseDataValueColumn>();
            where.Add(new DatabaseDataValueColumn("ComponentVersion_Page_Key", SqlDbType.Int, pageID));

            if (!string.IsNullOrEmpty(target))
                where.Add(new DatabaseDataValueColumn("ComponentVersion_Target", SqlDbType.VarChar, target)); 
            if (!string.IsNullOrEmpty(SqlConnectionString2)) componentVersion.SqlConnectionString = SqlConnectionString2;

            foreach (object o in componentVersion._SelectAll(where)) 
                //foreach (object o in componentVersion._SelectAll(where, false, "Page", pageID.ToString()))
                list.Add((ComponentVersion)o);
            return list.ToArray();
        }

        public static Sushi.Mediakiwi.Data.ComponentVersion[] SelectAllSharedForSite(int siteID)
        {
            List<ComponentVersion> list = new List<ComponentVersion>();
            ComponentVersion componentVersion = new ComponentVersion();

            List<DatabaseDataValueColumn> where = new List<DatabaseDataValueColumn>();

            where.Add(new DatabaseDataValueColumn("ComponentVersion_Page_Key", SqlDbType.Int, null));
            where.Add(new DatabaseDataValueColumn("ComponentTemplate_IsShared", SqlDbType.Bit, true));
            where.Add(new DatabaseDataValueColumn(string.Format("(ComponentVersion_Site_Key is null or ComponentVersion_Site_Key = {0})", siteID)));

            if (!string.IsNullOrEmpty(SqlConnectionString2)) componentVersion.SqlConnectionString = SqlConnectionString2;
            foreach (object o in componentVersion._SelectAll(where, false, "PageShared", string.Concat("all-", siteID)))
                list.Add((ComponentVersion)o);
            return list.ToArray();
        }

       
        /// <summary>
        /// Selects all.
        /// </summary>
        /// <param name="pageID">The page ID.</param>
        /// <param name="isSecundary">if set to <c>true</c> [is secundary].</param>
        /// <returns></returns>
        public static ComponentVersion[] SelectAll(int pageID, bool isSecundary)
        {
            List<ComponentVersion> list = new List<ComponentVersion>();
            ComponentVersion componentVersion = new ComponentVersion();

            List<DatabaseDataValueColumn> where = new List<DatabaseDataValueColumn>();
            where.Add(new DatabaseDataValueColumn("ComponentVersion_Page_Key", SqlDbType.Int, pageID));
            where.Add(new DatabaseDataValueColumn("ComponentTemplate_IsSecundaryContainerItem", SqlDbType.Bit, isSecundary));

            if (!string.IsNullOrEmpty(SqlConnectionString2)) componentVersion.SqlConnectionString = SqlConnectionString2;
            foreach (object o in componentVersion._SelectAll(where, false, "v", string.Concat(pageID, "_", isSecundary)))
                list.Add((ComponentVersion)o);
            return list.ToArray();
        }

        private int m_ID;
        /// <summary>
        /// Unique identifier of this componentVersion
        /// </summary>
        /// <value>The ID.</value>
        [DatabaseColumn("ComponentVersion_Key", SqlDbType.Int, IsPrimaryKey = true)]
        public int ID
        {
            get { return m_ID; }
            set { m_ID = value; }
        }

        private int? m_ApplicationUserID;
        /// <summary>
        /// Gets or sets the application user ID.
        /// </summary>
        /// <value>The application user ID.</value>
        [DatabaseColumn("ComponentVersion_User_Key", SqlDbType.Int, IsNullable = true)]
        public int? ApplicationUserID
        {
            get { return m_ApplicationUserID; }
            set { m_ApplicationUserID = value; }
        }

        private int? m_MasterID;
        /// <summary>
        /// The master componentversion identifier
        /// </summary>
        /// <value>The master ID.</value>
        [DatabaseColumn("ComponentVersion_Master_Key", SqlDbType.Int, IsNullable = true)]
        public int? MasterID
        {
            get { return m_MasterID; }
            set { m_MasterID = value; }
        }

        DateTime m_Created;
        /// <summary>
        /// Gets or sets the created.
        /// </summary>
        /// <value>The created.</value>
        [DatabaseColumn("ComponentVersion_Created", SqlDbType.DateTime, IsNullable = false)]
        public DateTime Created
        {
            get
            {
                if (this.m_Created == DateTime.MinValue) this.m_Created = Sushi.Mediakiwi.Data.Common.DatabaseDateTime;
                return m_Created;
            }
            set { m_Created = value; }
        }

        DateTime m_Updated;
        [DatabaseColumn("ComponentVersion_Updated", SqlDbType.DateTime, IsNullable = false)]
        public DateTime Updated
        {
            get
            {
                if (this.m_Updated == DateTime.MinValue) this.m_Updated = Created;
                return m_Updated;
            }
            set { m_Updated = value; }
        }

        private Guid m_GUID;
        /// <summary>
        /// Global Unique identifier of this componentversion
        /// </summary>
        /// <value>The GUID.</value>
        [DatabaseColumn("ComponentVersion_GUID", SqlDbType.UniqueIdentifier, IsMigrationKey = true)]
        public Guid GUID
        {
            get
            {
                if (this.m_GUID == Guid.Empty) this.m_GUID = Guid.NewGuid();
                return m_GUID;
            }
            set { m_GUID = value; }
        }

        //private int m_PageID;
        /// <summary>
        /// The page to which this componentversion belongs
        /// </summary>
        /// <value>The page ID.</value>
        [DatabaseColumn("ComponentVersion_Page_Key", SqlDbType.Int, IsNullable = true)]
        public int? PageID { get; set; }
        //{
        //    get { return m_PageID; }
        //    set { m_PageID = value; }
        //}

        private int? m_AvailableTemplateID;
        /// <summary>
        /// The corresponding componentTemplate identifier
        /// </summary>
        /// <value>The available template ID.</value>
        [DatabaseColumn("ComponentVersion_AvailableTemplate_Key", SqlDbType.Int, IsNullable = true)]
        public int? AvailableTemplateID
        {
            get { return m_AvailableTemplateID; }
            set { m_AvailableTemplateID = value; }
        }

        private int m_TemplateID;
        /// <summary>
        /// The corresponding componentTemplate identifier
        /// </summary>
        /// <value>The template ID.</value>
        [DatabaseColumn("ComponentVersion_ComponentTemplate_Key", SqlDbType.Int)]
        public int TemplateID
        {
            get { return m_TemplateID; }
            set { m_TemplateID = value; }
        }

        [DatabaseColumn("ComponentTemplate_IsShared", SqlDbType.Bit, IsOnlyRead = true)]
        public bool TemplateIsShared { get; set; }

        Sushi.Mediakiwi.Data.ComponentTemplate m_Template;
        /// <summary>
        /// Gets the template.
        /// </summary>
        /// <value>The template.</value>
        public Sushi.Mediakiwi.Data.ComponentTemplate Template
        {
            get
            {
                if (m_Template == null)
                    m_Template = Sushi.Mediakiwi.Data.ComponentTemplate.SelectOne(this.TemplateID);
                return m_Template;
            }
        }

        [DatabaseColumn("ComponentTemplate_Source", SqlDbType.NText, IsNullable = true, IsOnlyRead = true)]
        public string Source { get; set; }

        private bool m_IsFixed;
        /// <summary>
        /// Is this component fixed on the page?
        /// </summary>
        /// <value><c>true</c> if this instance is fixed; otherwise, <c>false</c>.</value>
        [DatabaseColumn("ComponentVersion_IsFixedOnTemplate", SqlDbType.Bit)]
        public bool IsFixed
        {
            get { return m_IsFixed; }
            set { m_IsFixed = value; }
        }

        private bool m_IsAlive;
        /// <summary>
        /// Is this component alive? Alive: When a fixed component is removed from a page it is still stored.
        /// When this component is reintroduced this content will be restored.
        /// </summary>
        /// <value><c>true</c> if this instance is alive; otherwise, <c>false</c>.</value>
        [DatabaseColumn("ComponentVersion_IsAlive", SqlDbType.Bit)]
        public bool IsAlive
        {
            get { return m_IsAlive; }
            set { m_IsAlive = value; }
        }

        private bool m_IsActive;
        /// <summary>
        /// Is this component alive? Alive: When a fixed component is removed from a page it is still stored.
        /// When this component is reintroduced this content will be restored.
        /// </summary>
        /// <value><c>true</c> if this instance is active; otherwise, <c>false</c>.</value>
        [DatabaseColumn("ComponentVersion_IsActive", SqlDbType.Bit)]
        public bool IsActive
        {
            get { return m_IsActive; }
            set { m_IsActive = value; }
        }

        [DatabaseColumn("ComponentVersion_Site_Key", SqlDbType.Int, IsNullable = true)]
        public int? SiteID { get; set; }

        [DatabaseColumn("ComponentTemplate_IsShared", SqlDbType.Bit, IsOnlyRead = true)]
        public bool IsShared { get; set; }

        private bool m_IsSecundary;
        /// <summary>
        /// Does this componentversion belong to the secundary container (like f.e. the service column)?
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is secundary; otherwise, <c>false</c>.
        /// </value>
        [DatabaseColumn("ComponentVersion_IsSecundary", SqlDbType.Bit)]
        public bool IsSecundary
        {
            get { return m_IsSecundary; }
            set { m_IsSecundary = value; }
        }

        string m_TemplateReferenceID;
        /// <summary>
        /// The component template reference ID
        /// </summary>
        /// <value>The template reference ID.</value>
        [DatabaseColumn("ComponentTemplate_ReferenceId", SqlDbType.NVarChar, Length = 5, IsOnlyRead = true, IsNullable = true)]
        public string TemplateReferenceID
        {
            get { return m_TemplateReferenceID; }
            set { m_TemplateReferenceID = value; }
        }

        bool m_IsSearchable;
        /// <summary>
        /// Is this component searchable?
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is searchable; otherwise, <c>false</c>.
        /// </value>
        [DatabaseColumn("ComponentTemplate_IsSearchable", SqlDbType.Bit, IsOnlyRead = true)]
        public bool IsSearchable
        {
            get { return m_IsSearchable; }
            set { m_IsSearchable = value; }
        }

        private string m_FixedFieldName;
        /// <summary>
        /// The identifying name of the fixed component
        /// </summary>
        /// <value>The name of the fixed field.</value>
        [DatabaseColumn("ComponentVersion_Fixed_Id", SqlDbType.VarChar, Length = 50, IsNullable = true)]
        public string FixedFieldName
        {
            get { return m_FixedFieldName; }
            set { m_FixedFieldName = value; }
        }

        private string m_Name;
        /// <summary>
        /// The name of the componentversion
        /// </summary>
        /// <value>The name.</value>
        //[DatabaseColumn("ComponentVersion_Name", SqlDbType.VarChar, Length = 50)]
        [DatabaseColumn("ComponentTemplate_Name", SqlDbType.VarChar, Length = 50, IsOnlyRead = true)]
        public string Name
        {
            get { return m_Name; }
            set { m_Name = value; }
        }

        [DatabaseColumn("ComponentVersion_Target", SqlDbType.VarChar, Length = 25, IsNullable = true)]
        public string Target { get; set; }

        [DatabaseColumn("ComponentVersion_Name", SqlDbType.VarChar, Length = 50, IsNullable = true)]
        public string InstanceName { get; set; }

        DateTime m_LastWriteTimeUtc;
        /// <summary>
        /// Gets or sets the last write time UTC of the page template (ASPX).
        /// </summary>
        /// <value>The last write time UTC.</value>
        [DatabaseColumn("ComponentTemplate_LastWriteTimeUtc", SqlDbType.DateTime, IsOnlyRead = true)]
        public DateTime LastWriteTimeUtc
        {
            get { return m_LastWriteTimeUtc; }
            set { m_LastWriteTimeUtc = value; }
        }

        private string m_TemplateLocation;
        /// <summary>
        /// Gets or sets the template location.
        /// </summary>
        /// <value>The template location.</value>
        [DatabaseColumn("ComponentTemplate_Location", SqlDbType.VarChar, Length = 250, IsOnlyRead = true)]
        public string TemplateLocation
        {
            get { return m_TemplateLocation; }
            set { m_TemplateLocation = value; }
        }

        private string m_Serialized_XML;
        /// <summary>
        /// The serialized content matching this componentversion
        /// </summary>
        /// <value>The serialized_ XML.</value>
        [DatabaseColumn("ComponentVersion_XML", SqlDbType.NText, IsNullable = true)]
        public string Serialized_XML
        {
            get { return m_Serialized_XML; }
            set { m_Serialized_XML = value; }
        }

        private DateTime? m_SortField_Date;
        /// <summary>
        /// The sortfield's introduced date.
        /// </summary>
        /// <value>The sort field_ date.</value>
        [DatabaseColumn("ComponentVersion_SortDate", SqlDbType.DateTime, IsNullable = true)]
        public DateTime? SortField_Date
        {
            get { return m_SortField_Date; }
            set { m_SortField_Date = value; }
        }



        private int m_SortOrder;
        /// <summary>
        /// Sortorder
        /// </summary>
        /// <value>The sort order.</value>
        [DatabaseColumn("ComponentVersion_SortOrder", SqlDbType.Int, IsNullable = true)]
        public int SortOrder
        {
            get { return m_SortOrder; }
            set { m_SortOrder = value; }
        }

        /// <summary>
        /// Applies the specified component.
        /// </summary>
        /// <param name="component">The component.</param>
        public void Apply(Component component)
        {
            component.GUID = this.GUID;
            component.PageID = this.PageID;
            component.TemplateID = this.TemplateID;
            component.Name = this.Name;
            component.IsFixed = this.IsFixed;
            component.IsSecundary = this.IsSecundary;
            component.IsSearchable = this.IsSearchable;
            component.FixedFieldName = this.FixedFieldName;
            component.TemplateLocation = this.TemplateLocation;
            component.Serialized_XML = this.Serialized_XML;
            component.SortField_Date = this.SortField_Date;
            component.SortOrder = this.SortOrder;
            component.Updated = this.Updated;
            component.Target = this.Target;
            component.Source = this.Source;
        }

        public override bool Delete()
        {
            return base.Delete();
        }

        /// <summary>
        /// Converts this instance.
        /// </summary>
        /// <returns></returns>
        public Component Convert()
        {
            Component component = new Component();
            Apply(component);
            component.ID = this.ID;
            return component;
        }

        public Component Convert2()
        {
            Component component = new Component();
            Apply(component);
            return component;
        }

        /// <summary>
        /// Select a component version on page based System.Type
        /// </summary>
        /// <param name="pageID">The page ID.</param>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static Sushi.Mediakiwi.Data.ComponentVersion SelectOneBasedOnType(int pageID, System.Type type)
        {
            List<DatabaseDataValueColumn> where = new List<DatabaseDataValueColumn>();
            where.Add(new DatabaseDataValueColumn("ComponentVersion_Page_Key", SqlDbType.Int, pageID));
            where.Add(new DatabaseDataValueColumn("ComponentTemplate_Type", SqlDbType.NVarChar, type.BaseType.ToString()));
            return (ComponentVersion)new ComponentVersion()._SelectOne(where);
        }

        /// <summary>
        /// Select a fixed componentversion object
        /// </summary>
        /// <param name="pageID">The page ID.</param>
        /// <param name="componentversionFixedName">name of the fixed componentversion</param>
        /// <returns>a componentversion object</returns>
        public static Sushi.Mediakiwi.Data.ComponentVersion SelectOneFixed(int pageID, string componentversionFixedName)
        {
            List<DatabaseDataValueColumn> where = new List<DatabaseDataValueColumn>();
            where.Add(new DatabaseDataValueColumn("ComponentVersion_Fixed_Id", SqlDbType.NVarChar, componentversionFixedName));
            where.Add(new DatabaseDataValueColumn("ComponentVersion_Page_Key", SqlDbType.Int, pageID));
            where.Add(new DatabaseDataValueColumn("ComponentVersion_IsFixedOnTemplate", SqlDbType.Bit, true));
            return (ComponentVersion)new ComponentVersion()._SelectOne(where);
        }

        /// <summary>
        /// Select a single componentversion instance
        /// </summary>
        /// <param name="ID">The ID.</param>
        /// <returns>componentversion object</returns>
        public static Sushi.Mediakiwi.Data.ComponentVersion SelectOne(int ID)
        {
            return (ComponentVersion)new ComponentVersion()._SelectOne(ID);
        }

        public static Sushi.Mediakiwi.Data.ComponentVersion SelectOne(Guid ID)
        {
            List<DatabaseDataValueColumn> where = new List<DatabaseDataValueColumn>();
            where.Add(new DatabaseDataValueColumn("ComponentVersion_GUID", SqlDbType.UniqueIdentifier, ID));
            return (ComponentVersion)new ComponentVersion()._SelectOne(where);
        }

        public static Sushi.Mediakiwi.Data.ComponentVersion SelectOneShared(int componentTemplateID)
        {
            List<DatabaseDataValueColumn> where = new List<DatabaseDataValueColumn>();
            where.Add(new DatabaseDataValueColumn("ComponentVersion_Page_Key", SqlDbType.Int, null));
            where.Add(new DatabaseDataValueColumn("ComponentVersion_ComponentTemplate_Key", SqlDbType.Int, componentTemplateID));
            return (ComponentVersion)new ComponentVersion()._SelectOne(where);
        }

        /// <summary>
        /// Select all componentversions on a page
        /// </summary>
        /// <param name="pageID">The page ID.</param>
        /// <returns>array of componentversions on a page</returns>
        public static Sushi.Mediakiwi.Data.ComponentVersion[] SelectAllOnPage(int pageID)
        {
            List<DatabaseDataValueColumn> whereClause = new List<DatabaseDataValueColumn>();
            whereClause.Add(new DatabaseDataValueColumn("ComponentVersion_Page_Key", SqlDbType.Int, pageID));
            whereClause.Add(new DatabaseDataValueColumn("ComponentVersion_IsAlive", SqlDbType.Bit, true));

            List<ComponentVersion> list = new List<ComponentVersion>();
            foreach (object o in new ComponentVersion()._SelectAll(whereClause))
                list.Add((ComponentVersion)o);
            return list.ToArray();
        }

        //public static Sushi.Mediakiwi.Data.ComponentVersion[] SelectAllOnPageShared1(int pageID)
        //{
        //    List<DatabaseDataValueColumn> whereClause = new List<DatabaseDataValueColumn>();
        //    whereClause.Add(new DatabaseDataValueColumn("ComponentTarget_Page_Key", SqlDbType.Int, pageID));
        //    whereClause.Add(new DatabaseDataValueColumn("ComponentTemplate_IsShared", SqlDbType.Bit, true));

        //    //whereClause.Add(new DatabaseDataValueColumn(string.Format("ComponentVersion_ComponentTemplate_Key in (select ComponentVersion_ComponentTemplate_Key from wim_ComponentVersions where ComponentVersion_Page_Key = {0})", pageID)));

        //    var component = new ComponentVersion();
        //    component.SqlJoin += " join wim_ComponentTargets on ComponentVersion_GUID = ComponentTarget_Component_Source";

        //    List<ComponentVersion> list = new List<ComponentVersion>();
        //    foreach (object o in component._SelectAll(whereClause))
        //        list.Add((ComponentVersion)o);
        //    return list.ToArray();
        //}

        /// <summary>
        /// Deletes all on page.
        /// </summary>
        /// <param name="pageKey">The page key.</param>
        /// <returns></returns>
        public static bool DeleteAllOnPage(int pageKey)
        {
            Sushi.Mediakiwi.Data.ComponentVersion version = new ComponentVersion();
            List<DatabaseDataValueColumn> whereClause = new List<DatabaseDataValueColumn>();
            whereClause.Add(new DatabaseDataValueColumn("ComponentVersion_Page_Key", SqlDbType.Int, pageKey));
            return version.Delete(whereClause);
        }

        /// <summary>
        /// Select all componentversions on a page
        /// </summary>
        /// <param name="pageID">The page ID.</param>
        /// <param name="isSecundary">should return secundary container componentversions?</param>
        /// <returns>array of componentversion on a page</returns>
        public static Sushi.Mediakiwi.Data.ComponentVersion[] SelectAllOnPage(int pageID, bool isSecundary)
        {
            List<DatabaseDataValueColumn> whereClause = new List<DatabaseDataValueColumn>();
            whereClause.Add(new DatabaseDataValueColumn("ComponentVersion_IsSecundary", SqlDbType.Bit, isSecundary));
            whereClause.Add(new DatabaseDataValueColumn("ComponentVersion_Page_Key", SqlDbType.Int, pageID));
            whereClause.Add(new DatabaseDataValueColumn("ComponentVersion_IsAlive", SqlDbType.Bit, true));

            List<ComponentVersion> list = new List<ComponentVersion>();
            foreach (object o in new ComponentVersion()._SelectAll(whereClause))
                list.Add((ComponentVersion)o);
            return list.ToArray();
        }

        //        /// <summary>
        //        /// Select all componentversions on a page related to it master page without boundaries (used for inheritance purposes only).
        //        /// </summary>
        //        /// <param name="masterPageID">The master page ID.</param>
        //        /// <param name="childPageID">The child page ID.</param>
        //        /// <param name="selectOnlyUnInheritedComponent">if set to <c>true</c> [select only un inherited component].</param>
        //        /// <returns>Array of ComponentVersion objects</returns>
        //        internal static Sushi.Mediakiwi.Data.ComponentVersion[] SelectAll(int? masterPageID, int childPageID, bool selectOnlyUnInheritedComponent)
        //        {
        //            List<DatabaseDataValueColumn> whereClause = new List<DatabaseDataValueColumn>();
        //            whereClause.Add(new DatabaseDataValueColumn("ComponentVersion_IsSecundary", SqlDbType.Bit, isSecundary));
        //            whereClause.Add(new DatabaseDataValueColumn("ComponentVersion_Page_Key", SqlDbType.Int, pageID));
        //            whereClause.Add(new DatabaseDataValueColumn("ComponentVersion_IsAlive", SqlDbType.Bit, true));

        //            List<ComponentVersion> list = new List<ComponentVersion>();
        //            foreach (object o in new ComponentVersion()._SelectAll(whereClause))
        //                list.Add((ComponentVersion)o);
        //            return list.ToArray();


        //            using (Connection.SqlCommander dac = new Connection.SqlCommander(Common.DatabaseConnection))
        //            {
        //                //  --Order by ComponentVersion_Fixed_Id DESC
        //                dac.SqlText = string.Format(@"
        //    select 
        //	    parent.ComponentVersion_Key
        //    ,	child.ComponentVersion_Master_Key
        //    ,	parent.ComponentVersion_GUID
        //    ,	parent.ComponentVersion_User_Key
        //    ,	parent.ComponentVersion_Page_Key
        //    ,	parent.ComponentVersion_ComponentTemplate_Key
        //    ,	parent.ComponentVersion_SortOrder
        //    ,	parent.ComponentVersion_Name
        //    ,	parent.ComponentVersion_Fixed_Id
        //    ,	parent.ComponentVersion_Created
        //    ,	parent.ComponentVersion_Updated
        //    ,	parent.ComponentVersion_IsFixedOnTemplate
        //    ,	parent.ComponentVersion_IsAlive
        //    ,	parent.ComponentVersion_IsSecundary
        //    ,	parent.ComponentVersion_XML
        //    ,	parent.ComponentVersion_SortDate
        //    ,   parent.ComponentVersion_AvailableTemplate_Key
        //    ,   wim_ComponentTemplates.*
        //    from
        //	    wim_ComponentVersions parent 
        //	    left join wim_ComponentVersions child on child.ComponentVersion_Master_Key = parent.ComponentVersion_Key
        //            and child.ComponentVersion_Page_Key = @ChildPage and child.ComponentVersion_IsAlive = 1 
        //        join wim_ComponentTemplates on parent.ComponentVersion_ComponentTemplate_Key = ComponentTemplate_Key 
        //    where
        //        parent.ComponentVersion_Page_Key = @MasterPage 
        //        and parent.ComponentVersion_IsAlive = 1        
        //        {0}
        //"
        //                    , selectOnlyUnInheritedComponent ? "and child.ComponentVersion_Master_Key IS NULL" : "");

        //                if (masterPageKey.HasValue)
        //                    dac.SetParameterInput("@MasterPage", masterPageKey.Value, SqlDbType.Int);
        //                else
        //                    dac.SetParameterInput("@MasterPage", null, SqlDbType.Int);

        //                dac.SetParameterInput("@ChildPage", childPageKey, SqlDbType.Int);

        //                SqlDataReader reader = dac.ExecReader;
        //                List<ComponentVersion> list = new List<ComponentVersion>();
        //                while (reader.Read())
        //                {
        //                    Sushi.Mediakiwi.Data.ComponentVersion component = new Sushi.Mediakiwi.Data.ComponentVersion();
        //                    ReadOne(reader, component);
        //                    list.Add(component);
        //                }
        //                reader.Close();
        //                return list.ToArray();
        //            }
        //        }

        /// <summary>
        /// Select all fixed componentversion on a page
        /// </summary>
        /// <param name="pageID">The page ID.</param>
        /// <param name="setAliveTemporaryOff">Should the isAlive property be turned off for all returned objects?</param>
        /// <returns>
        /// An array of fixed componentversions on a page
        /// </returns>
        public static Sushi.Mediakiwi.Data.ComponentVersion[] SelectAllFixed(int pageID, bool setAliveTemporaryOff)
        {
            List<DatabaseDataValueColumn> whereClause = new List<DatabaseDataValueColumn>();
            whereClause.Add(new DatabaseDataValueColumn("ComponentVersion_Page_Key", SqlDbType.Int, pageID));
            whereClause.Add(new DatabaseDataValueColumn("ComponentVersion_IsFixedOnTemplate", SqlDbType.Bit, true));

            List<ComponentVersion> list = new List<ComponentVersion>();
            foreach (object o in new ComponentVersion()._SelectAll(whereClause))
            {
                ComponentVersion c = (ComponentVersion)o;
                if (setAliveTemporaryOff) c.IsActive = false;
                list.Add(c);
            }
            return list.ToArray();
        }

        //        /// <summary>
        //        /// Check if all componentversions are present which have been set on the page template.
        //        /// </summary>
        //        /// <param name="pageKey"></param>
        //        /// <param name="applicationUserKey"></param>
        //        /// <returns>Update count</returns>
        //        public static int UpdateAll_PresentOnPage(int pageKey, int applicationUserKey)
        //        {
        //            int count = 0;
        //            using (Connection.SqlCommander dac = new Connection.SqlCommander(Common.DatabaseConnection))
        //            {
        //                dac.SqlText = @"
        //    select 
        //	    AvailableTemplates_Key
        //    ,   AvailableTemplates_ComponentTemplate_Key
        //    ,	ComponentTemplate_Name
        //    ,	AvailableTemplates_IsSecundary
        //    ,	(select top 1
        //		    ComponentVersion_Key
        //	    from
        //		    wim_ComponentVersions
        //	    where
        //		    ComponentVersion_Page_Key = @Page_Key
        //		    and ComponentVersion_ComponentTemplate_Key = AvailableTemplates_ComponentTemplate_Key
        //		    and ComponentVersion_IsSecundary = AvailableTemplates_IsSecundary
        //		    and ComponentVersion_IsFixedOnTemplate = 0) HasComponent
        //    from 
        //	    wim_pages 
        //	    join wim_AvailableTemplates on AvailableTemplates_PageTemplate_Key = Page_Template_Key
        //	    join wim_ComponentTemplates on ComponentTemplate_Key = AvailableTemplates_ComponentTemplate_Key
        //	    left join wim_ComponentVersions on AvailableTemplates_Key = ComponentVersion_AvailableTemplate_Key
        //            and ComponentVersion_Page_Key = @Page_Key
        //    where
        //	    Page_Key = @Page_Key
        //        and AvailableTemplates_IsPresent = 1
        //	    and AvailableTemplates_IsPossible = 1 
        //        and ComponentVersion_AvailableTemplate_Key IS NULL
        //";
        //                dac.SetParameterInput("@Page_Key", pageKey, SqlDbType.Int);
        //                SqlDataReader reader = dac.ExecReader;

        //                while (reader.Read())
        //                {
        //                    int missingComponentTemplate = (int)reader["AvailableTemplates_ComponentTemplate_Key"];
        //                    int availableTemplateKey = (int)reader["AvailableTemplates_Key"];
        //                    string name = (string)reader["ComponentTemplate_Name"];
        //                    bool isSecundary = (bool)reader["AvailableTemplates_IsSecundary"];
        //                    int simularComponent = GetReaderInt(reader["HasComponent"]);

        //                    if (simularComponent == 0)
        //                        simularComponent = InsertOne(missingComponentTemplate, applicationUserKey, pageKey, name, null, null, false, true, isSecundary, 0, null);
        //                    UpdateOne_Available(simularComponent, availableTemplateKey);
        //                    count++;
        //                }
        //                reader.Close();
        //            }
        //            return count;
        //        }

        //        private static bool UpdateOne_Available(int componentversionKey, int availableTemplateKey)
        //        {
        //            using (Connection.SqlCommander dac = new Connection.SqlCommander(Common.DatabaseConnection))
        //            {
        //                dac.SqlText = @"
        //    update wim_ComponentVersions 
        //    set 
        //        ComponentVersion_AvailableTemplate_Key = @AvailableTemplate_Key
        //    where 
        //        ComponentVersion_Key = @ComponentVersion_Key
        //";
        //                dac.SetParameterInput("@ComponentVersion_Key", componentversionKey, SqlDbType.Int);
        //                dac.SetParameterInput("@AvailableTemplate_Key", availableTemplateKey, SqlDbType.Int);
        //                dac.ExecNonQuery();
        //                return true;
        //            }
        //        }


        DateTime? iExportable.Updated
        {
            get { return Updated; }
        }

        #endregion REPLICATED TO Sushi.Mediakiwi.Data.Standard
    }
}
