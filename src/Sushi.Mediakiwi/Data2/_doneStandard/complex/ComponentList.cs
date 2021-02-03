using System;
using System.Data;
using Sushi.Mediakiwi.Data.DalReflection;
using Sushi.Mediakiwi.Framework;
using Sushi.Mediakiwi.Data.Parsers;

namespace Sushi.Mediakiwi.Data
{
    /// <summary>
    /// Represents a ComponentList entity. All data access is based on the static SelectAll method which is cached.
    /// </summary>
    [DatabaseTable("wim_ComponentLists", Order = "ComponentList_SortOrder ASC, ComponentList_ReferenceId ASC, ComponentList_Name ASC")]
    public partial class ComponentList : iExportable, IComponentList
    {
        static IComponentListParser _Parser;
        static IComponentListParser Parser
        {
            get
            {
                if (_Parser == null)
                    _Parser = Environment.GetInstance<IComponentListParser>();
                return _Parser;
            }
        }

        /// <summary>
        /// OBSOLETE: Reference: Get the name of the site that this list is assigned to
        /// </summary>
        //[DatabaseColumn("ComponentList_Site_Name", SqlDbType.Int, IsOnlyRead = true, IsNullable = true,
        //    ColumnSubQuery = "select Site_DisplayName from wim_Sites where ComponentList_Site_Key = Site_Key")]
        //public virtual string SiteName { get; set; }    //NAAR STANDARD, KAN WEG, WORDT NERGENS GEBRUIKT

        /// <summary>
        /// Does this list have a scheduled service call; if true then the SenseInterval should be > 0. 
        /// </summary>
        //[OnlyVisibleWhenTrue("IsConfiguration_Sense")]
        //[Framework.ContentListItem.Choice_Checkbox("Is active", false, Expression = OutputExpression.Alternating)]
        //public virtual bool HasServiceCall { get; set; }    //WEG


        //public Translator RenameTitle { get; set; }         //WEG


        /// <summary>
        /// Selects all componentList entities from a specific portal
        /// </summary>
        /// <param name="portal">The portal.</param>
        /// <returns></returns>
        public static IComponentList[] SelectAll(WimServerPortal portal)
        {
            return Parser.SelectAll(portal);
        }

        /// <summary>
        /// Select a componentList based on it's primary key and the corresponding portal
        /// </summary>
        /// <param name="ID">The ID.</param>
        /// <param name="portal">The portal.</param>
        /// <returns></returns>
        public static IComponentList SelectOne(int ID, WimServerPortal portal)
        {
            return Parser.SelectOne(ID, portal);
        }

        /// <summary>
        /// Select a componentList based on it's class beloging to a specific portal
        /// </summary>
        /// <param name="className">Name of the class.</param>
        /// <param name="portal">The portal.</param>
        /// <returns></returns>
        public static IComponentList SelectOne(string className, WimServerPortal portal)
        {
            return Parser.SelectOne(className, portal);
        }

        /// <summary>
        /// Select a componentList based on it's type belonging to a specific portal
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="portal">The portal.</param>
        /// <returns></returns>
        public static IComponentList SelectOne(Type type, string portal)
        {
            return Parser.SelectOne(type, portal);
        }


        /// <summary>
        /// Select a componentList based on it's GUID belonging to a specific portal
        /// </summary>
        /// <param name="componentListGUID"></param>
        /// <param name="portal"></param>
        /// <param name="defaultIsEmpty"></param>
        /// <returns></returns>
        public static IComponentList SelectOne(Guid componentListGUID, WimServerPortal portal, bool defaultIsEmpty = true)
        {
            return Parser.SelectOne(componentListGUID, portal, defaultIsEmpty);
        }

        #region REPLICATED TO Sushi.Mediakiwi.Data.Standard

        #region Static methods


        /// <summary>
        /// Select a componentList based on it's GUID and assign a handler that creates a new list if it does not exists
        /// </summary>
        /// <param name="componentListGUID"></param>
        /// <param name="ifNotPresentCreate">The creation handler</param>
        /// <returns></returns>
        public static IComponentList SelectOne(Guid componentListGUID, Func<Guid, IComponentList> ifNotPresentCreate)
        {
            return Parser.SelectOne(componentListGUID, ifNotPresentCreate);
        }

        /// <summary>
        /// Adds a thread safe component list entity
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="singleItemName"></param>
        /// <param name="folder"></param>
        /// <param name="site"></param>
        /// <param name="isVisible"></param>
        /// <param name="description"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static IComponentList Add(Guid guid, Type type, string name, string singleItemName, int? folder, int? site, bool isVisible = true, string description = null, ComponentListTarget target = ComponentListTarget.List)
        {
            return Parser.Add(guid, type, name, singleItemName, folder, site, isVisible, description, target);
        }
        /// <summary>
        /// Updates the sort order.
        /// </summary>
        /// <param name="listID">The list ID.</param>
        /// <param name="sortOrder">The sort order.</param>
        /// <returns></returns>
        public static bool UpdateSortOrder(int listID, int sortOrder)
        {
            return Parser.UpdateSortOrder(listID, sortOrder);
        }
        /// <summary>
        /// Select all componentList entities
        /// </summary>
        /// <returns></returns>
        public static IComponentList[] SelectAll()
        {
            return Parser.SelectAll();
        }
       
        /// <summary>
        /// Select all componentList entities by site(channel).
        /// </summary>
        /// <param name="siteID"></param>
        /// <returns></returns>
        public static IComponentList[] SelectAllBySite(int siteID)
        {
            return Parser.SelectAllBySite(siteID);
        }
        /// <summary>
        /// Select all componentList entities by searching by text and optionaly by siteID.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="siteID">The site ID.</param>
        /// <returns></returns>
        public static IComponentList[] SelectAll(string text, int? siteID)
        {
            return Parser.SelectAll(text, siteID);
        }
        /// <summary>
        /// Select all componentList entities based on folder and visibility.
        /// </summary>
        /// <param name="folderID">The folder ID.</param>
        /// <param name="includeHidden">if set to <c>true</c> [include hidden].</param>
        /// <returns></returns>
        public static IComponentList[] SelectAll(int folderID, bool includeHidden = false)
        {
            return Parser.SelectAll(folderID, includeHidden);
        }
        /// <summary>
        /// Select all componentList entities based on folder, the right of the applicationUser and visibility
        /// </summary>
        /// <param name="folderID">The folder ID.</param>
        /// <param name="user">The current application User</param>
        /// <returns></returns>
        public static IComponentList[] SelectAll(int folderID, IApplicationUser user, bool includeHidden = false)
        {
            return Parser.SelectAll(folderID, user, includeHidden);
        }
        /// <summary>
        /// Select all componentList entities based on the right of the user and the specific rightType 
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static IComponentList[] SelectAllAccessibleLists(IApplicationUser user, RoleRightType type)
        {
            return Parser.SelectAllAccessibleLists(user, type);
        }
        /// <summary>
        /// Extracts the componentLists that are allowed for the specifc user.
        /// </summary>
        /// <param name="lists">The lists.</param>
        /// <param name="user">The user.</param>
        /// <param name="siteID">The site ID.</param>
        /// <returns></returns>
        public static IComponentList[] ValidateAccessRight(IComponentList[] lists, IApplicationUser user, int siteID)
        {
            return Parser.ValidateAccessRight(lists, user, siteID);
        }
        /// <summary>
        /// Select all componentList entities that are assigned to a dashboard column
        /// </summary>
        /// <param name="dashboardID">The dashboard ID.</param>
        /// <param name="column">The column.</param>
        /// <returns></returns>
        public static IComponentList[] SelectAllDashboardLists(int dashboardID, int column)
        {
            return Parser.SelectAllDashboardLists(dashboardID, column);
        }
        /// <summary>
        /// Select all scheduled componentLists
        /// </summary>
        /// <returns></returns>
        public static IComponentList[] SelectAllScheduled()
        {
            return Parser.SelectAllScheduled();
        }
        /// <summary>
        /// Select a componentList based on it's primary key
        /// </summary>
        /// <param name="ID">The ID.</param>
        /// <returns></returns>
        public static IComponentList SelectOne(int ID)
        {
            return Parser.SelectOne(ID);
        }
      
        /// <summary>
        /// Select a componentList based on it's predefined type
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static IComponentList SelectOne(ComponentListType type)
        {
            return Parser.SelectOne(type);
        }
        /// <summary>
        /// Select a componentList based on it's class
        /// </summary>
        /// <param name="className">Name of the class.</param>
        /// <returns></returns>
        public static IComponentList SelectOne(string className)
        {
            return Parser.SelectOne(className);
        }
       
        /// <summary>
        /// Select a componentList based on it's type
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static IComponentList SelectOne(Type type)
        {
            return Parser.SelectOne(type);
        }
       
        /// <summary>
        /// Select a componentList based on it's reference
        /// </summary>
        /// <param name="referenceID">The reference ID.</param>
        /// <returns></returns>
        public static IComponentList SelectOneByReference(int referenceID)
        {
            return Parser.SelectOneByReference(referenceID);
        }
        /// <summary>
        /// Select a componentList based on it's GUID
        /// </summary>
        /// <param name="componentListGUID">The component list GUID.</param>
        /// <returns></returns>
        public static IComponentList SelectOne(Guid componentListGUID)
        {
            return Parser.SelectOne(componentListGUID);
        }
      
        #endregion Static methods

        #region Methods
        /// <summary>
        /// Saves the entity. This method will indentify the need for an INSERT or an UPDATE.
        /// </summary>
        /// <returns></returns>
        public virtual int Save()
        {
            return Parser.Save(this);
        }
        /// <summary>
        /// Determine if the user is allowed to view this componentList
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns>
        /// 	<c>true</c> if [has role access] [the specified role id]; otherwise, <c>false</c>.
        /// </returns>
        public virtual bool HasRoleAccess(IApplicationUser user)
        {
            return Parser.HasRoleAccess(ID, user);
        }
       
        /// <summary>
        /// Delete an implementation record.
        /// </summary>
        /// <returns></returns>
        public virtual bool Delete()
        {
            return Parser.Delete(this);
        }

        /// <summary>
        /// The complete path of this list in the portal
        /// </summary>
        /// <returns></returns>
        public virtual string CompletePath()
        {
            if (!FolderID.HasValue)
                return this.Name;

            return string.Format("{0}{1}", Sushi.Mediakiwi.Data.Folder.SelectOne(FolderID.Value).CompletePath, this.Name);
        }
        #endregion Methods

        #region Properties
        /// <summary>
        /// The primary key
        /// </summary>
        [DatabaseColumn("ComponentList_Key", SqlDbType.Int, IsPrimaryKey = true)]
        public virtual int ID { get; set; }
        /// <summary>
        /// The name of this list
        /// </summary>
        [OnlyVisibleWhenTrue("IsConfiguration_Template")]
        [Framework.ContentListItem.TextField("Title", 50, true, Expression = OutputExpression.Alternating)]
        [DatabaseColumn("ComponentList_Name", SqlDbType.NVarChar, Length = 50)]
        public virtual string Name { get; set; }
        /// <summary>
        /// The reference of this list
        /// </summary>
        [Framework.ContentListItem.TextField("Reference", 7, false, false, null, Wim.Utility.GlobalRegularExpression.OnlyNumeric, Expression = OutputExpression.Alternating)]
        [OnlyVisibleWhenTrue("IsConfiguration_Template")]
        [DatabaseColumn("ComponentList_ReferenceId", SqlDbType.Int, IsNullable = true)]
        public virtual int ReferenceID { get; set; }
        /// <summary>
        /// The name of this list when a single instance is shown
        /// </summary>
        [Framework.ContentListItem.TextField("Single item title", 30, true, Expression = OutputExpression.Alternating)]
        [OnlyVisibleWhenTrue("IsConfiguration_Template")]
        [DatabaseColumn("ComponentList_SingleItemName", SqlDbType.NVarChar, Length = 30, IsNullable = true)]
        public virtual string SingleItemName { get; set; }
        /// <summary>
        /// The visibility of this list
        /// </summary>
        [Framework.ContentListItem.Choice_Checkbox("Is visible", Expression = OutputExpression.Alternating)]
        [OnlyVisibleWhenTrue("IsConfiguration_Template")]
        [DatabaseColumn("ComponentList_IsVisible", SqlDbType.Bit)]
        public virtual bool IsVisible { get; set; }
        /// <summary>
        /// The assembly that this list belongs to.
        /// </summary>
        [Framework.ContentListItem.Choice_Dropdown("Assembly", "Assemblies", true, true)]
        [OnlyVisibleWhenTrue("IsConfiguration_Template")]
        [DatabaseColumn("ComponentList_Assembly", SqlDbType.VarChar, Length = 150, IsNullable = true)]
        public virtual string AssemblyName { get; set; }
        /// <summary>
        /// The corresponding class of this list
        /// </summary>
        [Framework.ContentListItem.Choice_Dropdown("Class", "Classes", true, true)]
        [OnlyVisibleWhenTrue("IsConfiguration_Template")]
        [DatabaseColumn("ComponentList_ClassName", SqlDbType.VarChar, Length = 250, IsNullable = true)]
        public virtual string ClassName { get; set; }

        [Framework.ContentListItem.TextField("Icon", 50, false)]
        [OnlyVisibleWhenTrue("IsConfiguration_Template")]
        [DatabaseColumn("ComponentList_Icon", SqlDbType.VarChar, Length = 50, IsNullable = true)]
        public virtual string Icon { get; set; }

        /// <summary>
        /// The description of this list
        /// </summary>
        [Framework.ContentListItem.TextArea("Description", 500, false)]
        [OnlyVisibleWhenTrue("IsConfiguration_Template")]
        [DatabaseColumn("ComponentList_Description", SqlDbType.NVarChar, Length = 500, IsNullable = true)]
        public virtual string Description { get; set; }

        //[DatabaseColumn("ComponentList_Help", SqlDbType.NVarChar, IsNullable = true)]

        //[Sushi.Mediakiwi.Framework.ContentListItem.MultiField("Help")]
        //public virtual string Help { get; set; }

        private Guid m_GUID;
        /// <summary>
        /// Unique identifier used for import/export
        /// </summary>
        [Framework.ContentListItem.TextField("GUID", 36)]
        [OnlyVisibleWhenTrue("IsConfiguration_Template")]
        [DatabaseColumn("ComponentList_GUID", SqlDbType.UniqueIdentifier, IsMigrationKey = true)]
        public virtual Guid GUID
        {
            get
            {
                if (this.m_GUID == Guid.Empty) this.m_GUID = Guid.NewGuid();
                return m_GUID;
            }
            set { m_GUID = value; }
        }
        string m_ChannelInfo = "Channel";
        /// <summary>
        /// Gets or sets the zz channel info.
        /// </summary>
        /// <value>The zz channel info.</value>
        [Framework.ContentListItem.Section()]
        [OnlyVisibleWhenTrue("IsConfiguration_Template")]
        public virtual string zzChannelInfo
        {
            get { return m_ChannelInfo; }
            set { m_ChannelInfo = value; }
        }
        /// <summary>
        /// The site(channel) ID
        /// </summary>
        [Framework.ContentListItem.Choice_Dropdown("Channel", "Sites", true, true, Expression = OutputExpression.Alternating)]
        [OnlyVisibleWhenTrue("IsConfiguration_Template")]
        [DatabaseColumn("ComponentList_Site_Key", SqlDbType.Int, IsNullable = true)]
        public virtual int? SiteID { get; set; }
        /// <summary>
        /// Is this list automatically inherited across child channels 
        /// </summary>
        [Framework.ContentListItem.Choice_Checkbox("Is inherited", Expression = OutputExpression.Alternating)]
        [OnlyVisibleWhenTrue("IsConfiguration_Template")]
        [DatabaseColumn("ComponentList_IsInherited", SqlDbType.Bit)]
        public virtual bool IsInherited { get; set; }
        /// <summary>
        /// The folder in which this list resides
        /// </summary>
        [Framework.ContentListItem.FolderSelect("Folder", true, FolderType.Administration_Or_List, null)]
        //[Sushi.Mediakiwi.Framework.OnlyEditableWhenTrue("IsWimType", false)]
        [OnlyVisibleWhenTrue("IsConfiguration_Template")]
        [DatabaseColumn("ComponentList_Folder_Key", SqlDbType.Int, IsNullable = true)]
        public virtual int? FolderID { get; set; }
    
        /// <summary>
        /// OBSOLETE: The section of the portal to which this list belongs to
        /// </summary>
        [DatabaseColumn("ComponentList_TargetType", SqlDbType.Int)]
        public virtual ComponentListTarget Target { get; set; }
        /// <summary>
        /// OBSOLETE: The specific portal type to which this list belongs to.
        /// </summary>
        [DatabaseColumn("ComponentList_Type", SqlDbType.Int)]
        public virtual ComponentListType Type { get; set; }
        /// <summary>
        /// OBSOLETE: Unique ID of the component template that is the basis of this list template.
        /// </summary>
        [DatabaseColumn("ComponentList_ComponentTemplate_Key", SqlDbType.Int, IsNullable = true)]
        public virtual int? ComponentTemplateID { get; set;  }
        /// <summary>
        /// OBSOLETE: Is this list of type class reference?
        /// </summary>
        [System.Xml.Serialization.XmlIgnore()]
        public virtual bool IsClassReference
        {
            get {
                return !string.IsNullOrEmpty(this.AssemblyName);
            }
        }
        /// <summary>
        /// The scheduled interval 
        /// </summary>
        [OnlyVisibleWhenTrue("IsConfiguration_Sense")]
        [Framework.ContentListItem.TextField("Interval (minutes)", 10, false, null, Wim.Utility.GlobalRegularExpression.OnlyNumeric, Expression = OutputExpression.Alternating)]
        [DatabaseColumn("ComponentList_ScheduleInterval", SqlDbType.Int, IsNullable = true)]
        public virtual int? SenseInterval { get; set; }
    
        /// <summary>
        /// The next sceduled service call
        /// </summary>
        [OnlyVisibleWhenTrue("IsConfiguration_Sense")]
        [Framework.ContentListItem.DateTime("Next run (UTC)", false)]
        [DatabaseColumn("ComponentList_Scheduled", SqlDbType.DateTime, IsNullable = true)]
        public virtual DateTime? SenseScheduled { get; set; }
        /// <summary>
        /// OBSOLETE: Does this list have one child 
        /// </summary>
        [DatabaseColumn("ComponentList_ContainsOneChild", SqlDbType.Bit)]
        public virtual bool HasOneChild { get; set;  }
        /// <summary>
        /// OBSOLETE: Is this list a template
        /// </summary>
        [DatabaseColumn("ComponentList_IsTemplate", SqlDbType.Bit)]
        public virtual bool IsTemplate { get; set; }
        /// <summary>
        /// OBSOLETE: The template identifier
        /// </summary>
        [DatabaseColumn("ComponentList_Template_Key", SqlDbType.Int, IsNullable = true)]
        public virtual int? TemplateID { get; set; }
        string m_ChannelInfo2 = "Data";
        /// <summary>
        /// Gets or sets the zz channel info.
        /// </summary>
        /// <value>The zz channel info.</value>
        [Framework.ContentListItem.Section()]
        [OnlyVisibleWhenTrue("IsConfiguration_Template")]
        public virtual string zzChannelInfo2
        {
            get { return m_ChannelInfo2; }
            set { m_ChannelInfo2 = value; }
        }
        /// <summary>
        /// The corresponding catalog identifier
        /// </summary>
        [OnlyVisibleWhenTrue("IsConfiguration_Template")]
        [Framework.ContentListItem.Choice_Dropdown("Catalog", "Catalogs", false)]
        [DatabaseColumn("ComponentList_Catalog_Key", SqlDbType.Int, IsNullable = true)]
        public virtual int CatalogID { get; set; }
        /// <summary>
        /// Is this template a single instance list
        /// </summary>
        [OnlyVisibleWhenTrue("IsConfiguration_Template")]
        [OnlyEditableWhenTrue("OldTypeGenerics")]
        [Framework.ContentListItem.Choice_Checkbox("Is single screen", false, Expression = OutputExpression.Alternating)]
        [DatabaseColumn("ComponentList_IsSingle", SqlDbType.Bit)]
        public virtual bool IsSingleInstance { get; set; }
        /// <summary>
        /// OBSOLETE: The group this list belongs to.
        /// </summary>
        [OnlyVisibleWhenTrue("IsConfiguration_Template")]
        [OnlyEditableWhenTrue("IsGenericClassName")]
        [Framework.ContentListItem.TextField("Group", 50, false, Expression = OutputExpression.Alternating)]
        [DatabaseColumn("ComponentList_Group", SqlDbType.VarChar, Length = 50, IsNullable = true)]
        public virtual string Group { get; set; }
        /// <summary>
        /// DOes this list have a sort order options
        /// </summary>
        [OnlyVisibleWhenTrue("IsConfiguration_Template")]
        [OnlyEditableWhenTrue("OldTypeGenerics")]
        [Framework.ContentListItem.Choice_Checkbox("Can sort", Expression = OutputExpression.Alternating)]
        [DatabaseColumn("ComponentList_CanSort", SqlDbType.Bit)]
        public virtual bool CanSortOrder { get; set; }
        /// <summary>
        /// The class of the underlying object
        /// </summary>
        [OnlyVisibleWhenTrue("IsConfiguration_Template")]
        [OnlyEditableWhenTrue("IsGenericClassName")]
        [Framework.ContentListItem.TextField("Class", 50, false, Expression = OutputExpression.Alternating)]
        [DatabaseColumn("ComponentList_Class", SqlDbType.VarChar, Length = 50, IsNullable = true)]
        public virtual string Class { get; set; }
        /// <summary>
        /// The custom properties dataStore on the componentList
        /// </summary>
        CustomData m_Data;
        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        /// <value>The data.</value>
        [DatabaseColumn("ComponentList_Data", SqlDbType.Xml, IsNullable = true)]
        public virtual CustomData Data
        {
            get
            {
                if (m_Data == null)
                    m_Data = new CustomData();
                return m_Data;
            }
            set { m_Data = value; }
        }
        CustomData m_Settings;
        /// <summary>
        /// The custom list settings dataStore off the componentList
        /// </summary>
        [DatabaseColumn("ComponentList_Settings", SqlDbType.Xml, IsNullable = true)]
        public virtual CustomData Settings
        {
            get
            {
                if (m_Settings == null)
                    m_Settings = new CustomData();
                return m_Settings;
            }
            set { m_Settings = value; }
        }
        /// <summary>
        /// Gets or sets the ZZ_ generics0.
        /// </summary>
        /// <value>The ZZ_ generics0.</value>
        [OnlyVisibleWhenTrue("IsConfiguration_Template")]
        [Framework.ContentListItem.Section()]
        public virtual string zz_Generics0 { get; set; }
        /// <summary>
        /// Can create a new item
        /// </summary>
        [OnlyVisibleWhenTrue("IsConfiguration_Template")]
        [Framework.ContentListItem.Choice_Checkbox("Create new", Expression = OutputExpression.Alternating)]
        public virtual bool Option_CanCreate
        {
            get { return this.Data["wim_CanCreate"].ParseBoolean(true); }
            set { this.Data.ApplyObject("wim_CanCreate", value); }
        }
        /// <summary>
        /// Can save an item
        /// </summary>
        [OnlyVisibleWhenTrue("IsConfiguration_Template")]
        [Framework.ContentListItem.Choice_Checkbox("Save record", Expression = OutputExpression.Alternating)]
        public virtual bool Option_CanSave
        {
            get { return this.Data["wim_CanSave"].ParseBoolean(true); }
            set { this.Data.ApplyObject("wim_CanSave", value); }
        }
        /// <summary>
        /// Can delete an item
        /// </summary>
        [OnlyVisibleWhenTrue("IsConfiguration_Template")]
        [Framework.ContentListItem.Choice_Checkbox("Delete record", Expression = OutputExpression.Alternating)]
        public virtual bool Option_CanDelete
        {
            get { return this.Data["wim_CanDelete"].ParseBoolean(true); }
            set { this.Data.ApplyObject("wim_CanDelete", value); }
        }
        /// <summary>
        /// Can save and add a new item
        /// </summary>
        [OnlyVisibleWhenTrue("IsConfiguration_Template")]
        [Framework.ContentListItem.Choice_Checkbox("Save and add new", Expression = OutputExpression.Alternating)]
        public virtual bool Option_CanSaveAndAddNew
        {
            get { return this.Data["wim_01"].ParseBoolean(true); }
            set { this.Data.ApplyObject("wim_01", value); }
        }
        /// <summary>
        /// Has the export to XLS option
        /// </summary>
        [OnlyVisibleWhenTrue("IsConfiguration_Template")]
        [Framework.ContentListItem.Choice_Checkbox("Has export (XLS)", Expression = OutputExpression.Alternating)]
        public virtual bool Option_HasExportXLS
        {
            get { return this.Data["wim_hasExport_XLS"].ParseBoolean(true); }
            set { this.Data.ApplyObject("wim_hasExport_XLS", value); }
        }
        /// <summary>
        /// Export the column titles to the XLS export
        /// </summary>
        [OnlyVisibleWhenTrue("IsConfiguration_Template")]
        [Framework.ContentListItem.Choice_Checkbox("Export columns", Expression = OutputExpression.Alternating)]
        public virtual bool Option_HasExportColumnTitlesXLS
        {
            get { return this.Data["wim_ExportCol_XLS"].ParseBoolean(true); }
            set { this.Data.ApplyObject("wim_ExportCol_XLS", value); }
        }
        /// <summary>
        /// Always open the list in edit mode
        /// </summary>
        [OnlyVisibleWhenTrue("IsConfiguration_Template")]
        [Framework.ContentListItem.Choice_Checkbox("Open in editmode", Expression = OutputExpression.Alternating)]
        public virtual bool Option_OpenInEditMode
        {
            get { return this.Data["wim_OpenInEdit"].ParseBoolean(); }
            set { this.Data.ApplyObject("wim_OpenInEdit", value); }
        }
        /// <summary>
        /// Can you subscribe to this list
        /// </summary>
        [OnlyVisibleWhenTrue("IsConfiguration_Template")]
        [Framework.ContentListItem.Choice_Checkbox("Can Subscribe", Expression = OutputExpression.Alternating)]
        public virtual bool Option_HasSubscribeOption
        {
            get { return this.Data["wim_hasSubscribeOption"].ParseBoolean(false); }
            set { this.Data.ApplyObject("wim_hasSubscribeOption", value); }
        }
        //[Sushi.Mediakiwi.Framework.OnlyVisibleWhenTrue("IsConfiguration_Template")]
        //public virtual bool Option_HasExportPDF
        //{
        //    get { return this.Data["wim_hasExport_PDF"].ParseBoolean(true); }
        //    set { this.Data.ApplyObject("wim_hasExport_PDF", value); }
        //}
        /// <summary>
        /// Can you show all items in the list
        /// </summary>
        [OnlyVisibleWhenTrue("IsConfiguration_Template")]
        [Framework.ContentListItem.Choice_Checkbox("Has 'Show All'", Expression = OutputExpression.Alternating)]
        public virtual bool Option_HasShowAll
        {
            get { return this.Data["wim_hasShowAll"].ParseBoolean(true); }
            set { this.Data.ApplyObject("wim_hasShowAll", value); }
        }
        /// <summary>
        /// Only trigger search when a button is clicked
        /// </summary>
        [OnlyVisibleWhenTrue("IsConfiguration_Template")]
        [Framework.ContentListItem.Choice_Checkbox("Postback search", Expression = OutputExpression.Alternating)]
        public virtual bool Option_PostBackSearch
        {
            get { return this.Data["wim_PostbackSearch"].ParseBoolean(); }
            set { this.Data.ApplyObject("wim_PostbackSearch", value); }
        }
        /// <summary>
        /// After save return to the list overview
        /// </summary>
        [OnlyVisibleWhenTrue("IsConfiguration_Template")]
        [Framework.ContentListItem.Choice_Checkbox("After save view list", Expression = OutputExpression.Alternating)]
        public virtual bool Option_AfterSaveListView
        {
            get { return this.Data["wim_AfterSaveListView"].ParseBoolean(); }
            set { this.Data.ApplyObject("wim_AfterSaveListView", value); }
        }
        /// <summary>
        /// Show the search result asynchronous
        /// </summary>
        [OnlyVisibleWhenTrue("IsConfiguration_Template")]
        [Framework.ContentListItem.Choice_Checkbox("Search async", Expression = OutputExpression.Alternating, InteractiveHelp = "Load the search result via ajax")]
        public virtual bool Option_SearchAsync
        {
            get { return this.Data["wim_SearchAsync"].ParseBoolean(); }
            set { this.Data.ApplyObject("wim_SearchAsync", value); }
        }
        [OnlyVisibleWhenTrue("IsConfiguration_Template")]
        [Framework.ContentListItem.Choice_Checkbox("Form async", Expression = OutputExpression.Alternating, InteractiveHelp = "Load the search result via ajax")]
        public virtual bool Option_FormAsync
        {
            get { return this.Data["wim_FormAsync"].ParseBoolean(); }
            set { this.Data.ApplyObject("wim_FormAsync", value); }
        }
        /// <summary>
        /// Open this listitem in a layer
        /// </summary>
        [OnlyVisibleWhenTrue("IsConfiguration_Template")]
        [Framework.ContentListItem.Choice_Checkbox("Layered item", Expression = OutputExpression.Alternating, InteractiveHelp = "Open an search result item in a layer")]
        public virtual bool Option_LayerResult
        {
            get { return this.Data["wim_LayerResult"].ParseBoolean(); }
            set { this.Data.ApplyObject("wim_LayerResult", value); }
        }
        /// <summary>
        /// Does this list have breadcrumbs
        /// </summary>
        [OnlyVisibleWhenTrue("IsConfiguration_Template")]
        [Framework.ContentListItem.Choice_Checkbox("Hide breadcrumbs", Expression = OutputExpression.Alternating, InteractiveHelp = "Open an search result item in a layer")]
        public virtual bool Option_HideBreadCrumbs
        {
            get { return this.Data["wim_HideCrumbs"].ParseBoolean(); }
            set { this.Data.ApplyObject("wim_HideCrumbs", value); }
        }
        /// <summary>
        /// Does this list have a datareport (count in overview and naviation)
        /// </summary>
        [OnlyVisibleWhenTrue("IsConfiguration_Template")]
        [Framework.ContentListItem.Choice_Checkbox("Has datareport", Expression = OutputExpression.Alternating, InteractiveHelp = "Has a datareport event, so call it (ListDataReport)")]
        public virtual bool Option_HasDataReport
        {
            get { return this.Data["wim_DataReport"].ParseBoolean(); }
            set { this.Data.ApplyObject("wim_DataReport", value); }
        }
        /// <summary>
        /// Hide the lefthand navigation
        /// </summary>
        [OnlyVisibleWhenTrue("IsConfiguration_Template")]
        [Framework.ContentListItem.Choice_Checkbox("Hide navigation", Expression = OutputExpression.Alternating, InteractiveHelp = "Has a datareport event, so call it (ListDataReport)")]
        public virtual bool Option_HideNavigation
        {
            get { return this.Data["wim_HideNavigation"].ParseBoolean(); }
            set { this.Data.ApplyObject("wim_HideNavigation", value); }
        }

        /// <summary>
        /// Converts the displayed datetime from UTC to timezone from channel
        /// </summary>
        [OnlyVisibleWhenTrue("IsConfiguration_Template")]
        [Framework.ContentListItem.Choice_Checkbox("UTC to TimeZone", Expression = OutputExpression.Alternating, InteractiveHelp = "Converts UTC time to timezone set in Channel for this lists DateTime,<br/>Date and TextLine properties. As well as the ListSearch columns.")]
        public virtual bool Option_ConvertUTCToLocalTime
        {
            get { return this.Data["wim_ConvertUTCToLocalTime"].ParseBoolean(); }
            set { this.Data.ApplyObject("wim_ConvertUTCToLocalTime", value); }
        }

        /// <summary>
        /// Gets or sets the ZZ_ generics1.
        /// </summary>
        /// <value>The ZZ_ generics1.</value>
        [OnlyVisibleWhenTrue("IsConfiguration_Template")]
        [Framework.ContentListItem.Section()]
        public virtual string zz_Generics1 { get; set; }
        /// <summary>
        /// The label shown on the new item button
        /// </summary>
        [OnlyVisibleWhenTrue("IsConfiguration_Template")]
        [Framework.ContentListItem.TextField("Label 'new record'", 50, false, Expression = OutputExpression.Alternating)]
        public virtual string Label_NewRecord
        {
            get { return this.Data["wim_LblNew"].Value; }
            set { this.Data.ApplyObject("wim_LblNew", value); }
        }
        /// <summary>
        /// The label on the search button
        /// </summary>
        [OnlyVisibleWhenTrue("IsConfiguration_Template")]
        [Framework.ContentListItem.TextField("Label 'search'", 25, false, Expression = OutputExpression.Alternating)]
        public virtual string Label_Search
        {
            get { return this.Data["wim_LblSearch"].Value; }
            set { this.Data.ApplyObject("wim_LblSearch", value); }
        }
        /// <summary>
        /// The label on the save button
        /// </summary>
        [OnlyVisibleWhenTrue("IsConfiguration_Template")]
        [Framework.ContentListItem.TextField("Label 'save'", 25, false, Expression = OutputExpression.Alternating)]
        public virtual string Label_Save
        {
            get { return this.Data["wim_LblSave"].Value; }
            set { this.Data.ApplyObject("wim_LblSave", value); }
        }
        /// <summary>
        /// The notification text shown when the list is saved
        /// </summary>
        [OnlyVisibleWhenTrue("IsConfiguration_Template")]
        [Framework.ContentListItem.TextField("Label 'saved'", 500, false, Expression = OutputExpression.Alternating)]
        public virtual string Label_Saved
        {
            get { return this.Data["wim_LblSaved"].Value; }
            set { this.Data.ApplyObject("wim_LblSaved", value); }
        }
        [OnlyVisibleWhenTrue("IsConfiguration_Template")]
        [Framework.ContentListItem.Section()]
        public virtual string zz_Generics2 { get; set; }
        /// <summary>
        /// The maximum amount of pages shown in the search grid
        /// </summary>
        [OnlyVisibleWhenTrue("IsConfiguration_Template")]
        [Framework.ContentListItem.TextField("Maximum pages", 4, false, null, Wim.Utility.GlobalRegularExpression.OnlyNumeric, Expression = OutputExpression.Alternating)]
        public virtual int Option_Search_MaxViews
        {
            get { return this.Data["wim_MaxViews"].ParseInt().GetValueOrDefault(10); }
            set { this.Data.ApplyObject("wim_MaxViews", value); }
        }
        /// <summary>
        /// The maximum amount of items shown in a paged result
        /// </summary>
        [OnlyVisibleWhenTrue("IsConfiguration_Template")]
        [Framework.ContentListItem.TextField("Result per page", 4, false, null, Wim.Utility.GlobalRegularExpression.OnlyNumeric, Expression = OutputExpression.Alternating)]
        public virtual int Option_Search_MaxResultPerPage
        {
            get { return this.Data["wim_MaxResult"].ParseInt().GetValueOrDefault(25); }
            set { this.Data.ApplyObject("wim_MaxResult", value); }
        }
        /// <summary>
        /// The maximum amount of search results
        /// </summary>
        public virtual int Option_Search_MaxResult
        {
            get {
                return Option_Search_MaxViews * Option_Search_MaxResultPerPage;
            }
        }
        [OnlyVisibleWhenTrue("IsConfiguration_Template")]
        [Framework.ContentListItem.Section()]
        public virtual string zz_Generics3 { get; set; }
        /// <summary>
        /// Generic list: Filter result by individual list
        /// </summary>
        [OnlyEditableWhenTrue("OldTypeGenerics")]
        [OnlyVisibleWhenTrue("IsConfiguration_Template")]
        [Framework.ContentListItem.Choice_Checkbox("Filter by list", Expression = OutputExpression.Alternating)]
        public virtual bool HasGenericListFilter
        {
            get { return this.Data["wim_GenericByList"].ParseBoolean(true); }
            set { this.Data.ApplyObject("wim_GenericByList", value); }
        }
        /// <summary>
        /// Generic list: Filter result by site
        /// </summary>
        [OnlyEditableWhenTrue("OldTypeGenerics")]
        [OnlyVisibleWhenTrue("IsConfiguration_Template")]
        [Framework.ContentListItem.Choice_Checkbox("Filter by site", Expression = OutputExpression.Alternating)]
        public virtual bool HasGenericSiteFilter
        {
            get { return this.Data["wim_GenericBySite"].ParseBoolean(false); }
            set { this.Data.ApplyObject("wim_GenericBySite", value); }
        }
        public virtual DateTime? Updated
        {
            get { return null; }
        }
        /// <summary>
        /// This this list a new instance (ID equals 0)
        /// </summary>
        public virtual bool IsNewInstance
        {
            get
            {
                return this.ID == 0;
            }
        }

        #endregion

        #endregion REPLICATED TO Sushi.Mediakiwi.Data.Standard

        #region MOVED to EXTENSION / LOGIC


        ///// <summary>
        ///// Get the corresponding catalog for a specific portal
        ///// </summary>
        ///// <param name="portal">The portal.</param>
        ///// <returns></returns>
        //public virtual Sushi.Mediakiwi.Data.Catalog Catalog(Sushi.Mediakiwi.Framework.WimServerPortal portal = null)
        //{
        //    if (this.CatalogID > 0)
        //        return Sushi.Mediakiwi.Data.Catalog.SelectOne(this.CatalogID, portal);

        //    return null;
        //}

        ///// <summary>
        ///// Determine if the user is allowed to view this componentList
        ///// </summary>
        ///// <param name="user">The user.</param>
        ///// <param name="portal">The portal.</param>
        ///// <returns>
        /////   <c>true</c> if [has role access2] [the specified user]; otherwise, <c>false</c>.
        ///// </returns>
        //public virtual bool HasRoleAccess(Sushi.Mediakiwi.Data.IApplicationUser user, string portal)
        //{
        //    return Parser.HasRoleAccess(ID, user, portal);
        //}

        ///// <summary>
        ///// Get an instance of the class behind this componentList
        ///// </summary>
        ///// <returns></returns>
        ///// <value>The instance.</value>
        //public virtual IComponentListTemplate GetInstance()
        //{
        //    IComponentListTemplate m_Instance = null;
        //    Type candidate = null;
        //    Object instance = null;

        //    #region Code generation: Depricated
        //    //if (Wim.CommonConfiguration.HasCodeGeneration)
        //    //{
        //    //    string className = null;
        //    //    string assemblyName = null;

        //    //    if (this.ClassName == "Sushi.Mediakiwi.AppCentre.Data.Implementation.Identity.ProfileList")
        //    //    {
        //    //        //  Profiles
        //    //        assemblyName = Wim.CommonConfiguration.CODEGENERATION_ASSEMBLY;
        //    //        className = string.Concat(Wim.CommonConfiguration.CODEGENERATION_NAMESPACE, ".Identity.UI.RegisteredProfileList");
        //    //    }
        //    //    else if (this.ClassName == "Sushi.Mediakiwi.AppCentre.Data.Implementation.Identity.VisitorList")
        //    //    {
        //    //        //  Visitors
        //    //        assemblyName = Wim.CommonConfiguration.CODEGENERATION_ASSEMBLY;
        //    //        className = string.Concat(Wim.CommonConfiguration.CODEGENERATION_NAMESPACE, ".Identity.UI.RegisteredVisitorList");
        //    //    }
        //    //    else if (!string.IsNullOrEmpty(this.Class))
        //    //    {
        //    //        //  Generics
        //    //        string groupName = string.IsNullOrEmpty(this.Group) ? null : string.Concat(this.Group.Replace(" ", "_"), ".");
        //    //        assemblyName = Wim.CommonConfiguration.CODEGENERATION_ASSEMBLY;
        //    //        className = string.Concat(Wim.CommonConfiguration.CODEGENERATION_NAMESPACE, ".Generics.", groupName, "UI.", this.Class, "List");
        //    //    }

        //    //    if (!string.IsNullOrEmpty(className))
        //    //    {
        //    //        instance = Wim.Utility.CreateInstance(assemblyName, className, out candidate, false);
        //    //        if (instance != null)
        //    //            m_Instance = instance as Framework.ComponentListClassTemplate;
        //    //    }

        //    //}
        //    #endregion Code generation: Depricated

        //    if (m_Instance == null)
        //    {
        //        instance = Wim.Utility.CreateInstance(this.AssemblyName, this.ClassName, out candidate, false);

        //        //  [11 nov 14:MM] Added routing support
        //        #region Routing support

        //        // BD 2016-10-07: Added Nullcheck due to fatal errors
        //        if (instance != null)
        //        {
        //            var routingAttributes = instance.GetType().GetCustomAttributes(typeof(Sushi.Mediakiwi.Framework.ComponentListRouting), true);
        //            if (routingAttributes != null && routingAttributes.Length > 0)
        //            {
        //                //  Take first routing and process it
        //                var routingAttribute = routingAttributes[0] as Sushi.Mediakiwi.Framework.ComponentListRouting;
        //                if (routingAttribute != null && routingAttribute.Routing != null)
        //                {
        //                    //  When routing exists, validate this route, when NULL, ignore it.
        //                    ComponentListRoutingArgs e = null;
        //                    //  Dirty code, but SOLID priciples can not apply (yet).
        //                    var context = System.Web.HttpContext.Current;
        //                    if (context != null)
        //                    {
        //                        e = new ComponentListRoutingArgs()
        //                        {
        //                            SelectedKey = Wim.Utility.ConvertToIntNullable(context.Request.QueryString["item"]),
        //                            SelectedGroup = Wim.Utility.ConvertToIntNullable(context.Request.QueryString["group"]),
        //                            SelectedGroupItem = Wim.Utility.ConvertToIntNullable(context.Request.QueryString["groupitem"])
        //                        };
        //                    }

        //                    var instanceCandidate = routingAttribute.Routing.Validate(this, e);
        //                    if (instanceCandidate != null)
        //                        instance = instanceCandidate;
        //                }
        //            }
        //        }
        //        #endregion Routing support
        //    }

        //    if (instance != null)
        //    {
        //        m_Instance = (IComponentListTemplate)instance;
        //        if (m_Instance.wim.PassOverClassInstance != null)
        //            m_Instance = m_Instance.wim.PassOverClassInstance;

        //        m_Instance.wim.CurrentList = this;
        //    }

        //    return m_Instance;
        //}


        #endregion MOVED to EXTENSION / LOGIC
    }
}
