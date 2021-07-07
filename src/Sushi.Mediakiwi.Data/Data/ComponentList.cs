using Sushi.Mediakiwi.Data.MicroORM;
using Sushi.MicroORM;
using Sushi.MicroORM.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Data
{
    /// <summary>
    /// Represents a ComponentList entity.
    /// </summary>
    [DataMap(typeof(ComponentListmap))]
    public class ComponentList : IExportable, IComponentList
    {
        public class ComponentListmap : DataMap<ComponentList>
        {
            public ComponentListmap()
            {
                Table("wim_ComponentLists");

                Id(x => x.ID, "ComponentList_Key").Identity();
                Map(x => x.Name, "ComponentList_Name").Length(50);
                Map(x => x.ReferenceID, "ComponentList_ReferenceId");
                Map(x => x.SingleItemName, "ComponentList_SingleItemName").Length(30);
                Map(x => x.IsVisible, "ComponentList_IsVisible");
                Map(x => x.AssemblyName, "ComponentList_Assembly").Length(150);
                Map(x => x.ClassName, "ComponentList_ClassName").Length(250);
                Map(x => x.Icon, "ComponentList_Icon").Length(50);
                Map(x => x.Description, "ComponentList_Description").Length(500);
                Map(x => x.GUID, "ComponentList_GUID");
                Map(x => x.SiteID, "ComponentList_Site_Key");
                Map(x => x.IsInherited, "ComponentList_IsInherited");
                Map(x => x.FolderID, "ComponentList_Folder_Key");
                Map(x => x.Target, "ComponentList_TargetType");
                Map(x => x.Type, "ComponentList_Type");
                Map(x => x.ComponentTemplateID, "ComponentList_ComponentTemplate_Key");
                Map(x => x.SenseInterval, "ComponentList_ScheduleInterval");
                Map(x => x.SenseScheduled, "ComponentList_Scheduled");
                Map(x => x.HasOneChild, "ComponentList_ContainsOneChild");
                Map(x => x.IsTemplate, "ComponentList_IsTemplate");
                Map(x => x.CatalogID, "ComponentList_Catalog_Key");
                Map(x => x.IsSingleInstance, "ComponentList_IsSingle");
                Map(x => x.Group, "ComponentList_Group").Length(50);
                Map(x => x.CanSortOrder, "ComponentList_CanSort");
                Map(x => x.Class, "ComponentList_Class").Length(50);
                Map(x => x.DataString, "ComponentList_Data").SqlType(System.Data.SqlDbType.Xml);
                Map(x => x.SettingsString, "ComponentList_Settings").SqlType(System.Data.SqlDbType.Xml);
                Map(x => x.SortOrder, "ComponentList_SortOrder");
            }
        }

        #region Properties

        /// <summary>
        /// The primary key
        /// </summary>
        public virtual int ID { get; set; }

        /// <summary>
        /// The name of this list
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// The reference of this list
        /// </summary>
        public virtual int ReferenceID { get; set; }

        /// <summary>
        /// The name of this list when a single instance is shown
        /// </summary>
        public virtual string SingleItemName { get; set; }

        /// <summary>
        /// The visibility of this list
        /// </summary>
        public virtual bool IsVisible { get; set; }

        /// <summary>
        /// The assembly that this list belongs to.
        /// </summary>
        public virtual string AssemblyName { get; set; }

        /// <summary>
        /// The corresponding class of this list
        /// </summary>
        public virtual string ClassName { get; set; }

        public virtual string Icon { get; set; }

        /// <summary>
        /// The description of this list
        /// </summary>
        public virtual string Description { get; set; }

        private Guid m_GUID;

        /// <summary>
        /// Unique identifier used for import/export
        /// </summary>
        public virtual Guid GUID
        {
            get
            {
                if (m_GUID == Guid.Empty)
                    m_GUID = Guid.NewGuid();
                return m_GUID;
            }
            set { m_GUID = value; }
        }

        /// <summary>
        /// The site(channel) ID
        /// </summary>
        public virtual int? SiteID { get; set; }

        /// <summary>
        /// Is this list automatically inherited across child channels
        /// </summary>
        public virtual bool IsInherited { get; set; }

        /// <summary>
        /// The folder in which this list resides
        /// </summary>
        public virtual int? FolderID { get; set; }

        /// <summary>
        /// OBSOLETE: The section of the portal to which this list belongs to
        /// </summary>
        public virtual ComponentListTarget Target { get; set; }

        /// <summary>
        /// OBSOLETE: The specific portal type to which this list belongs to.
        /// </summary>
        public virtual ComponentListType Type { get; set; }

        /// <summary>
        /// OBSOLETE: Unique ID of the component template that is the basis of this list template.
        /// </summary>
        public virtual int? ComponentTemplateID { get; set; }

        /// <summary>
        /// OBSOLETE: Is this list of type class reference?
        /// </summary>
        [System.Xml.Serialization.XmlIgnore()]
        public virtual bool IsClassReference
        {
            get
            {
                return !string.IsNullOrEmpty(AssemblyName);
            }
        }

        /// <summary>
        /// The scheduled interval
        /// </summary>
        public virtual int? SenseInterval { get; set; }

        /// <summary>
        /// The next sceduled service call
        /// </summary>
        public virtual DateTime? SenseScheduled { get; set; }

        /// <summary>
        /// OBSOLETE: Does this list have one child
        /// </summary>
        public virtual bool HasOneChild { get; set; }

        /// <summary>
        /// OBSOLETE: Is this list a template
        /// </summary>
        public virtual bool IsTemplate { get; set; }

        /// <summary>
        /// The corresponding catalog identifier
        /// </summary>
        public virtual int CatalogID { get; set; }

        /// <summary>
        /// Is this template a single instance list
        /// </summary>
        public virtual bool IsSingleInstance { get; set; }

        /// <summary>
        /// OBSOLETE: The group this list belongs to.
        /// </summary>
        public virtual string Group { get; set; }

        /// <summary>
        /// DOes this list have a sort order options
        /// </summary>
        public virtual bool CanSortOrder { get; set; }

        /// <summary>
        /// The class of the underlying object
        /// </summary>
        public virtual string Class { get; set; }

        /// <summary>
        /// The SortOrder for this ComponentList
        /// </summary>
        public virtual int SortOrder { get; set; }

        /// <summary>
        /// XML representation of the DATA property
        /// </summary>
        private string DataString { get; set; }

        private CustomData m_Data;

        /// <summary>
        /// Holds all customData properties
        /// </summary>
        public CustomData Data
        {
            get
            {
                if (m_Data == null)
                    m_Data = new CustomData(DataString);

                return m_Data;
            }
            set
            {
                m_Data = value;
                DataString = m_Data.Serialized;
            }
        }

        /// <summary>
        /// XML representation of the Settings property
        /// </summary>
        private string SettingsString { get; set; }

        private CustomData m_Settings;

        /// <summary>
        /// Holds all Setting properties
        /// </summary>
        public CustomData Settings
        {
            get
            {
                if (m_Settings == null)
                    m_Settings = new CustomData(SettingsString);

                return m_Settings;
            }
            set
            {
                m_Settings = value;
                SettingsString = m_Settings.Serialized;
            }
        }

        /// <summary>
        /// Can create a new item
        /// </summary>
        public virtual bool Option_CanCreate
        {
            get { return this.Data["wim_CanCreate"].ParseBoolean(true); }
            set { this.Data.ApplyObject("wim_CanCreate", value); }
        }

        /// <summary>
        /// Can save an item
        /// </summary>
        public virtual bool Option_CanSave
        {
            get { return this.Data["wim_CanSave"].ParseBoolean(true); }
            set { this.Data.ApplyObject("wim_CanSave", value); }
        }

        /// <summary>
        /// Can delete an item
        /// </summary>
        public virtual bool Option_CanDelete
        {
            get { return this.Data["wim_CanDelete"].ParseBoolean(true); }
            set { this.Data.ApplyObject("wim_CanDelete", value); }
        }

        /// <summary>
        /// Can save and add a new item
        /// </summary>
        public virtual bool Option_CanSaveAndAddNew
        {
            get { return this.Data["wim_01"].ParseBoolean(); }
            set { this.Data.ApplyObject("wim_01", value); }
        }

        /// <summary>
        /// Has the export to XLS option
        /// </summary>
        public virtual bool Option_HasExportXLS
        {
            get { return this.Data["wim_hasExport_XLS"].ParseBoolean(false); }
            set { this.Data.ApplyObject("wim_hasExport_XLS", value); }
        }

        /// <summary>
        /// Export the column titles to the XLS export
        /// </summary>
        public virtual bool Option_HasExportColumnTitlesXLS
        {
            get { return this.Data["wim_ExportCol_XLS"].ParseBoolean(false); }
            set { this.Data.ApplyObject("wim_ExportCol_XLS", value); }
        }

        /// <summary>
        /// Always open the list in edit mode
        /// </summary>
        public virtual bool Option_OpenInEditMode
        {
            get { return this.Data["wim_OpenInEdit"].ParseBoolean(true); }
            set { this.Data.ApplyObject("wim_OpenInEdit", value); }
        }

        /// <summary>
        /// Can you subscribe to this list
        /// </summary>
        public virtual bool Option_HasSubscribeOption
        {
            get { return this.Data["wim_hasSubscribeOption"].ParseBoolean(false); }
            set { this.Data.ApplyObject("wim_hasSubscribeOption", value); }
        }

        /// <summary>
        /// Can you show all items in the list
        /// </summary>
        public virtual bool Option_HasShowAll
        {
            get { return this.Data["wim_hasShowAll"].ParseBoolean(false); }
            set { this.Data.ApplyObject("wim_hasShowAll", value); }
        }

        /// <summary>
        /// Only trigger search when a button is clicked
        /// </summary>
        public virtual bool Option_PostBackSearch
        {
            get { return this.Data["wim_PostbackSearch"].ParseBoolean(); }
            set { this.Data.ApplyObject("wim_PostbackSearch", value); }
        }

        /// <summary>
        /// After save return to the list overview
        /// </summary>
        public virtual bool Option_AfterSaveListView
        {
            get { return this.Data["wim_AfterSaveListView"].ParseBoolean(true); }
            set { this.Data.ApplyObject("wim_AfterSaveListView", value); }
        }

        /// <summary>
        /// Show the search result asynchronous
        /// </summary>
        public virtual bool Option_SearchAsync
        {
            get { return this.Data["wim_SearchAsync"].ParseBoolean(); }
            set { this.Data.ApplyObject("wim_SearchAsync", value); }
        }

        /// <summary>
        /// Does the Form support ASYNC calls ?
        /// </summary>
        public virtual bool Option_FormAsync
        {
            get { return this.Data["wim_FormAsync"].ParseBoolean(); }
            set { this.Data.ApplyObject("wim_FormAsync", value); }
        }

        /// <summary>
        /// Open this listitem in a layer
        /// </summary>
        public virtual bool Option_LayerResult
        {
            get { return this.Data["wim_LayerResult"].ParseBoolean(); }
            set { this.Data.ApplyObject("wim_LayerResult", value); }
        }

        /// <summary>
        /// Does this list have breadcrumbs
        /// </summary>
        public virtual bool Option_HideBreadCrumbs
        {
            get { return this.Data["wim_HideCrumbs"].ParseBoolean(); }
            set { this.Data.ApplyObject("wim_HideCrumbs", value); }
        }

        /// <summary>
        /// Does this list have a datareport (count in overview and naviation)
        /// </summary>
        public virtual bool Option_HasDataReport
        {
            get { return this.Data["wim_DataReport"].ParseBoolean(); }
            set { this.Data.ApplyObject("wim_DataReport", value); }
        }

        /// <summary>
        /// Hide the lefthand navigation
        /// </summary>
        public virtual bool Option_HideNavigation
        {
            get { return this.Data["wim_HideNavigation"].ParseBoolean(); }
            set { this.Data.ApplyObject("wim_HideNavigation", value); }
        }

        /// <summary>
        /// Converts the displayed datetime from UTC to timezone from channel
        /// </summary>
        public virtual bool Option_ConvertUTCToLocalTime
        {
            get { return this.Data["wim_ConvertUTCToLocalTime"].ParseBoolean(); }
            set { this.Data.ApplyObject("wim_ConvertUTCToLocalTime", value); }
        }

        /// <summary>
        /// The label shown on the new item button
        /// </summary>
        public virtual string Label_NewRecord
        {
            get { return this.Data["wim_LblNew"].Value; }
            set { this.Data.ApplyObject("wim_LblNew", value); }
        }

        /// <summary>
        /// The label on the search button
        /// </summary>
        public virtual string Label_Search
        {
            get { return this.Data["wim_LblSearch"].Value; }
            set { this.Data.ApplyObject("wim_LblSearch", value); }
        }

        /// <summary>
        /// The label on the save button
        /// </summary>
        public virtual string Label_Save
        {
            get { return this.Data["wim_LblSave"].Value; }
            set { this.Data.ApplyObject("wim_LblSave", value); }
        }

        /// <summary>
        /// The notification text shown when the list is saved
        /// </summary>
        public virtual string Label_Saved
        {
            get { return this.Data["wim_LblSaved"].Value; }
            set { this.Data.ApplyObject("wim_LblSaved", value); }
        }

        /// <summary>
        /// The maximum amount of pages shown in the search grid
        /// </summary>
        public virtual int Option_Search_MaxViews
        {
            get { return this.Data["wim_MaxViews"].ParseInt().GetValueOrDefault(10); }
            set { this.Data.ApplyObject("wim_MaxViews", value); }
        }

        /// <summary>
        /// The maximum amount of items shown in a paged result
        /// </summary>
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
            get
            {
                return Option_Search_MaxViews * Option_Search_MaxResultPerPage;
            }
        }

        /// <summary>
        /// Generic list: Filter result by individual list
        /// </summary>
        public virtual bool HasGenericListFilter
        {
            get { return this.Data["wim_GenericByList"].ParseBoolean(true); }
            set { this.Data.ApplyObject("wim_GenericByList", value); }
        }

        /// <summary>
        /// Generic list: Filter result by site
        /// </summary>
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

        #endregion Properties

        #region Static methods

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
            if (folder.HasValue && !site.HasValue)
                site = Folder.SelectOne(folder.Value).SiteID;

            var assembly = Assembly.GetAssembly(type);
            var instance = new ComponentList();
            instance.GUID = guid;
            instance.ClassName = type.ToString();
            instance.AssemblyName = assembly.ManifestModule.Name;
            instance.Name = name;
            instance.SingleItemName = singleItemName;
            instance.IsVisible = isVisible;
            instance.Description = description;
            instance.Target = target;
            instance.FolderID = folder;
            instance.SiteID = site;
            instance.Save();

            return instance;
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
        public static async Task<IComponentList> AddAsync(Guid guid, Type type, string name, string singleItemName, int? folder, int? site, bool isVisible = true, string description = null, ComponentListTarget target = ComponentListTarget.List)
        {
            if (folder.HasValue && !site.HasValue)
                site = Folder.SelectOne(folder.Value).SiteID;

            var assembly = Assembly.GetAssembly(type);
            var instance = new ComponentList();
            instance.GUID = guid;
            instance.ClassName = type.ToString();
            instance.AssemblyName = assembly.ManifestModule.Name;
            instance.Name = name;
            instance.SingleItemName = singleItemName;
            instance.IsVisible = isVisible;
            instance.Description = description;
            instance.Target = target;
            instance.FolderID = folder;
            instance.SiteID = site;
            await instance.SaveAsync().ConfigureAwait(false);

            return instance;
        }

        /// <summary>
        /// Updates the sort order.
        /// </summary>
        /// <param name="listID">The list ID.</param>
        /// <param name="sortOrder">The sort order.</param>
        /// <returns></returns>
        public static bool UpdateSortOrder(int listID, int sortOrder)
        {
            var connector = ConnectorFactory.CreateConnector<ComponentList>();
            var filter = connector.CreateDataFilter();

            filter.AddParameter("@sortOrder", sortOrder);
            filter.AddParameter("@listId", listID);

            connector.ExecuteNonQuery("UPDATE [wim_ComponentLists] SET [ComponentList_SortOrder] = @sortOrder WHERE [ComponentList_Key] = @listId", filter);
            connector.Cache?.FlushRegion(connector.CacheRegion);
            return true;
        }

        /// <summary>
        /// Updates the sort order Async.
        /// </summary>
        /// <param name="listID">The list ID.</param>
        /// <param name="sortOrder">The sort order.</param>
        /// <returns></returns>
        public static async Task<bool> UpdateSortOrderAsync(int listID, int sortOrder)
        {
            var connector = ConnectorFactory.CreateConnector<ComponentList>();
            var filter = connector.CreateDataFilter();

            filter.AddParameter("@sortOrder", sortOrder);
            filter.AddParameter("@listId", listID);

            await connector.ExecuteNonQueryAsync("UPDATE [wim_ComponentLists] SET [ComponentList_SortOrder] = @sortOrder WHERE [ComponentList_Key] = @listId", filter)
                .ConfigureAwait(false);
            connector.Cache?.FlushRegion(connector.CacheRegion);
            return true;
        }

        /// <summary>
        /// Select all componentList entities
        /// </summary>
        /// <returns></returns>
        public static IComponentList[] SelectAll()
        {
            var connector = ConnectorFactory.CreateConnector<ComponentList>();
            var filter = connector.CreateDataFilter();
            filter.AddOrder(x => x.SortOrder);
            filter.AddOrder(x => x.ReferenceID);
            filter.AddOrder(x => x.Name);

            return connector.FetchAll(filter).ToArray();
        }

        /// <summary>
        /// Select all componentList entities Async
        /// </summary>
        /// <returns></returns>
        public static async Task<IComponentList[]> SelectAllAsync()
        {
            var connector = ConnectorFactory.CreateConnector<ComponentList>();
            var filter = connector.CreateDataFilter();
            filter.AddOrder(x => x.SortOrder);
            filter.AddOrder(x => x.ReferenceID);
            filter.AddOrder(x => x.Name);

            var result = await connector.FetchAllAsync(filter).ConfigureAwait(false);
            return result.ToArray();
        }

        /// <summary>
        /// Select all componentList entities by site(channel).
        /// </summary>
        /// <param name="siteID"></param>
        /// <returns></returns>
        public static IComponentList[] SelectAllBySite(int siteID)
        {
            var connector = ConnectorFactory.CreateConnector<ComponentList>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.SiteID, siteID);
            filter.AddOrder(x => x.SortOrder);
            filter.AddOrder(x => x.ReferenceID);
            filter.AddOrder(x => x.Name);

            return connector.FetchAll(filter).ToArray();
        }

        /// <summary>
        /// Select all componentList entities by site(channel).
        /// </summary>
        /// <param name="siteID"></param>
        /// <returns></returns>
        public static async Task<IComponentList[]> SelectAllBySiteAsync(int siteID)
        {
            var connector = ConnectorFactory.CreateConnector<ComponentList>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.SiteID, siteID);
            filter.AddOrder(x => x.SortOrder);
            filter.AddOrder(x => x.ReferenceID);
            filter.AddOrder(x => x.Name);

            var result = await connector.FetchAllAsync(filter).ConfigureAwait(false);
            return result.ToArray();
        }

        /// <summary>
        /// Select all componentList entities by searching by text and optionaly by siteID.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="siteID">The site ID.</param>
        /// <returns></returns>
        public static IComponentList[] SelectAll(string text, int? siteID)
        {
            var connector = ConnectorFactory.CreateConnector<ComponentList>();
            var filter = connector.CreateDataFilter();
            filter.AddOrder(x => x.SortOrder);
            filter.AddOrder(x => x.ReferenceID);
            filter.AddOrder(x => x.Name);

            List<int> targets = new List<int>()
            {
                (int)ComponentListTarget.List,
                (int)ComponentListTarget.Administration
            };

            if (siteID.GetValueOrDefault(0) > 0)
                filter.Add(x => x.SiteID, siteID.Value);

            if (!string.IsNullOrWhiteSpace(text))
                filter.Add(x => x.Name, $"%{text}%", ComparisonOperator.Like);

            filter.Add(x => x.Target, targets, ComparisonOperator.In);

            return connector.FetchAll(filter).ToArray();
        }

        /// <summary>
        /// Select all componentList entities by searching by text and optionaly by siteID. Async
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="siteID">The site ID.</param>
        /// <returns></returns>
        public static async Task<IComponentList[]> SelectAllAsync(string text, int? siteID)
        {
            var connector = ConnectorFactory.CreateConnector<ComponentList>();
            var filter = connector.CreateDataFilter();
            filter.AddOrder(x => x.SortOrder);
            filter.AddOrder(x => x.ReferenceID);
            filter.AddOrder(x => x.Name);

            List<int> targets = new List<int>()
            {
                (int)ComponentListTarget.List,
                (int)ComponentListTarget.Administration
            };

            if (siteID.GetValueOrDefault(0) > 0)
                filter.Add(x => x.SiteID, siteID.Value);

            if (!string.IsNullOrWhiteSpace(text))
                filter.Add(x => x.Name, $"%{text}%", ComparisonOperator.Like);

            filter.Add(x => x.Target, targets, ComparisonOperator.In);

            var result = await connector.FetchAllAsync(filter).ConfigureAwait(false);
            return result.ToArray();
        }

        /// <summary>
        /// Select all componentList entities based on folder and visibility.
        /// </summary>
        /// <param name="folderID">The folder ID.</param>
        /// <param name="includeHidden">if set to <c>true</c> [include hidden].</param>
        /// <returns></returns>
        public static IComponentList[] SelectAll(int folderID, bool includeHidden = false)
        {
            var connector = ConnectorFactory.CreateConnector<ComponentList>();
            var filter = connector.CreateDataFilter();
            filter.AddOrder(x => x.SortOrder);
            filter.AddOrder(x => x.ReferenceID);
            filter.AddOrder(x => x.Name);

            filter.Add(x => x.FolderID, folderID);
            if (!includeHidden)
                filter.Add(x => x.IsVisible, true);

            Folder folder = Folder.SelectOne(folderID);

            List<IComponentList> local = connector.FetchAll(filter).ToList<IComponentList>();

            //  Get all inherited lists
            while (folder.MasterID.HasValue)
            {
                if (!folder.MasterID.HasValue)
                    break;

                filter = connector.CreateDataFilter();
                filter.Add(x => x.FolderID, folder.MasterID.Value);
                filter.Add(x => x.IsInherited, true);
                filter.AddOrder(x => x.SortOrder);
                filter.AddOrder(x => x.ReferenceID);
                filter.AddOrder(x => x.Name);

                if (!includeHidden)
                    filter.Add(x => x.IsVisible, true);

                List<IComponentList> parent = connector.FetchAll(filter).ToList<IComponentList>();

                if (parent.Count > 0)
                    local.AddRange(parent);

                folder = Folder.SelectOne(folder.MasterID.Value);
            }

            return local.ToArray();
        }

        /// <summary>
        /// Select all componentList entities based on folder and visibility. Async
        /// </summary>
        /// <param name="folderID">The folder ID.</param>
        /// <param name="includeHidden">if set to <c>true</c> [include hidden].</param>
        /// <returns></returns>
        public static async Task<IComponentList[]> SelectAllAsync(int folderID, bool includeHidden = false)
        {
            var connector = ConnectorFactory.CreateConnector<ComponentList>();
            var filter = connector.CreateDataFilter();
            filter.AddOrder(x => x.SortOrder);
            filter.AddOrder(x => x.ReferenceID);
            filter.AddOrder(x => x.Name);

            filter.Add(x => x.FolderID, folderID);
            if (includeHidden == false)
                filter.Add(x => x.IsVisible, true);

            Folder folder = await Folder.SelectOneAsync(folderID).ConfigureAwait(false);

            var localResult = await connector.FetchAllAsync(filter).ConfigureAwait(false);
            List<IComponentList> local = localResult.ToList<IComponentList>();

            //  Get all inherited lists
            while (folder.MasterID.HasValue)
            {
                if (!folder.MasterID.HasValue)
                    break;

                filter = connector.CreateDataFilter();
                filter.Add(x => x.FolderID, folder.MasterID.Value);
                filter.Add(x => x.IsInherited, true);
                filter.AddOrder(x => x.SortOrder);
                filter.AddOrder(x => x.ReferenceID);
                filter.AddOrder(x => x.Name);

                if (includeHidden == false)
                    filter.Add(x => x.IsVisible, true);

                var parentResult = await connector.FetchAllAsync(filter).ConfigureAwait(false);
                List<IComponentList> parent = parentResult.ToList<IComponentList>();

                if (parent.Count > 0)
                    local.AddRange(parent);

                folder = await Folder.SelectOneAsync(folder.MasterID.Value).ConfigureAwait(false);
            }

            return local.ToArray();
        }

        /// <summary>
        /// Select all componentList entities based on folder, the right of the applicationUser and visibility
        /// </summary>
        /// <param name="folderID">The folder ID.</param>
        /// <param name="user">The current application User</param>
        /// <returns></returns>
        public static IComponentList[] SelectAll(int folderID, IApplicationUser user, bool includeHidden = false)
        {
            IComponentList[] lists = null;
            if (!user.Role().All_Lists)
            {
                if (user.Role().IsAccessList)
                {
                    lists = (
                        from item in SelectAll(folderID, includeHidden)
                        join relation in RoleRight.SelectAll(user.Role().ID, RoleRightType.List) on item.ID equals relation.ItemID
                        select item).ToArray();
                }
                else
                {
                    var acl = (
                        from item in SelectAll(folderID, includeHidden)
                        join relation in RoleRight.SelectAll(user.Role().ID, RoleRightType.List) on item.ID equals relation.ItemID
                        into combination
                        from relation in combination.DefaultIfEmpty()
                        select new { ID = item.ID, HasAccess = relation == null });

                    lists = (
                        from item in acl
                        join relation in SelectAll(folderID, includeHidden) on item.ID equals relation.ID
                        where item.HasAccess
                        select relation).ToArray();
                }
            }
            else
                lists = SelectAll(folderID, includeHidden);
            return lists;
        }

        /// <summary>
        /// Select all componentList entities based on folder, the right of the applicationUser and visibility Async
        /// </summary>
        /// <param name="folderID">The folder ID.</param>
        /// <param name="user">The current application User</param>
        /// <returns></returns>
        public static async Task<IComponentList[]> SelectAllAsync(int folderID, IApplicationUser user, bool includeHidden = false)
        {
            IComponentList[] lists = null;
            if (!user.Role().All_Lists)
            {
                if (user.Role().IsAccessList)
                {
                    lists = (
                        from item in await SelectAllAsync(folderID, includeHidden).ConfigureAwait(false)
                        join relation in await RoleRight.SelectAllAsync(user.Role().ID, RoleRightType.List).ConfigureAwait(false) 
                        on item.ID equals relation.ItemID
                        select item).ToArray();
                }
                else
                {
                    var acl = (
                        from item in await SelectAllAsync(folderID, includeHidden).ConfigureAwait(false)
                        join relation in await RoleRight.SelectAllAsync(user.Role().ID, RoleRightType.List).ConfigureAwait(false)
                        on item.ID equals relation.ItemID
                        into combination
                        from relation in combination.DefaultIfEmpty()
                        select new { ID = item.ID, HasAccess = relation == null });

                    lists = (
                        from item in acl
                        join relation in await SelectAllAsync(folderID, includeHidden).ConfigureAwait(false)
                        on item.ID equals relation.ID
                        where item.HasAccess
                        select relation).ToArray();
                }
            }
            else
                lists = await SelectAllAsync(folderID, includeHidden).ConfigureAwait(false);

            return lists;
        }

        /// <summary>
        /// Select all componentList entities based on the right of the user and the specific rightType
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static IComponentList[] SelectAllAccessibleLists(IApplicationUser user, RoleRightType type)
        {
            IComponentList[] lists = null;
            if (!user.Role().All_Lists)
            {
                if (user.Role().IsAccessList)
                {
                    lists = (
                        from item in SelectAll()
                        join relation in RoleRight.SelectAll(user.Role().ID, type) on item.ID equals relation.ItemID
                        select item).ToArray();
                }
                else
                {
                    var acl = (
                        from item in SelectAll()
                        join relation in RoleRight.SelectAll(user.Role().ID, type) on item.ID equals relation.ItemID
                        into combination
                        from relation in combination.DefaultIfEmpty()
                        select new { ID = item.ID, HasAccess = relation == null });

                    lists = (
                        from item in acl
                        join relation in SelectAll() on item.ID equals relation.ID
                        where item.HasAccess
                        select relation).ToArray();
                }
            }
            else
                lists = SelectAll();
            return lists;
        }

        /// <summary>
        /// Select all componentList entities based on the right of the user and the specific rightType Async
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static async Task<IComponentList[]> SelectAllAccessibleListsAsync(IApplicationUser user, RoleRightType type)
        {
            IComponentList[] lists = null;
            if (!user.Role().All_Lists)
            {
                if (user.Role().IsAccessList)
                {
                    lists = (
                        from item in await SelectAllAsync().ConfigureAwait(false)
                        join relation in await RoleRight.SelectAllAsync(user.Role().ID, type).ConfigureAwait(false)
                        on item.ID equals relation.ItemID
                        select item).ToArray();
                }
                else
                {
                    var acl = (
                        from item in await SelectAllAsync().ConfigureAwait(false)
                        join relation in await RoleRight.SelectAllAsync(user.Role().ID, type).ConfigureAwait(false)
                        on item.ID equals relation.ItemID
                        into combination
                        from relation in combination.DefaultIfEmpty()
                        select new { ID = item.ID, HasAccess = relation == null });

                    lists = (
                        from item in acl
                        join relation in await SelectAllAsync().ConfigureAwait(false)
                        on item.ID equals relation.ID
                        where item.HasAccess
                        select relation).ToArray();
                }
            }
            else
                lists = await SelectAllAsync().ConfigureAwait(false);

            return lists;
        }

        /// <summary>
        /// Extracts the componentLists that are allowed for the specifc user.
        /// </summary>
        /// <param name="lists">The lists.</param>
        /// <param name="user">The user.</param>
        /// <param name="siteID">The site ID.</param>
        /// <returns></returns>
        public static IComponentList[] ValidateAccessRight(IComponentList[] lists, IApplicationUser user)
        {
            return (from item in lists join relation in SelectAllAccessibleLists(user, RoleRightType.List) on item.ID equals relation.ID select item).ToArray();
        }

        /// <summary>
        /// Extracts the componentLists that are allowed for the specifc user Async.
        /// </summary>
        /// <param name="lists">The lists.</param>
        /// <param name="user">The user.</param>
        /// <param name="siteID">The site ID.</param>
        /// <returns></returns>
        public static async Task<IComponentList[]> ValidateAccessRightAsync(IComponentList[] lists, IApplicationUser user)
        {
            return (from item in lists join relation in await SelectAllAccessibleListsAsync(user, RoleRightType.List).ConfigureAwait(false)
                    on item.ID equals relation.ID select item).ToArray();
        }

        /// <summary>
        /// Select all componentList entities that are assigned to a dashboard column
        /// </summary>
        /// <param name="dashboardID">The dashboard ID.</param>
        /// <param name="column">The column.</param>
        /// <returns></returns>
        public static IComponentList[] SelectAllDashboardLists(int dashboardID, int column)
        {
            var candidate =
                         from item in SelectAll()
                         join relation in DashboardListItem.SelectAll(dashboardID) on item.ID equals relation.ListID
                         where relation.ColumnID == column
                         orderby relation.SortOrder
                         select item;
            return candidate.ToArray();
        }

        /// <summary>
        /// Select all componentList entities that are assigned to a dashboard column
        /// </summary>
        /// <param name="dashboardID">The dashboard ID.</param>
        /// <param name="column">The column.</param>
        /// <returns></returns>
        public static async Task<IComponentList[]> SelectAllDashboardListsAsync(int dashboardID, int column)
        {
            var candidate =
                         from item in await SelectAllAsync().ConfigureAwait(false)
                         join relation in await DashboardListItem.SelectAllAsync(dashboardID).ConfigureAwait(false)
                         on item.ID equals relation.ListID
                         where relation.ColumnID == column
                         orderby relation.SortOrder
                         select item;
            return candidate.ToArray();
        }

        /// <summary>
        /// Select all scheduled componentLists
        /// </summary>
        /// <returns></returns>
        public static IComponentList[] SelectAllScheduled()
        {
            var connector = ConnectorFactory.CreateConnector<ComponentList>();
            var filter = connector.CreateDataFilter();
            filter.AddOrder(x => x.SortOrder);
            filter.AddOrder(x => x.ReferenceID);
            filter.AddOrder(x => x.Name);
            filter.AddSql("NOT [ComponentList_Scheduled] IS NULL");

            return connector.FetchAll(filter).ToArray();
        }

        /// <summary>
        /// Select all scheduled componentLists Async
        /// </summary>
        /// <returns></returns>
        public static async Task<IComponentList[]> SelectAllScheduledAsync()
        {
            var connector = ConnectorFactory.CreateConnector<ComponentList>();
            var filter = connector.CreateDataFilter();
            filter.AddOrder(x => x.SortOrder);
            filter.AddOrder(x => x.ReferenceID);
            filter.AddOrder(x => x.Name);
            filter.AddSql("NOT [ComponentList_Scheduled] IS NULL");

            var result = await connector.FetchAllAsync(filter).ConfigureAwait(false);
            return result.ToArray();
        }

        /// <summary>
        /// Select a componentList based on it's primary key
        /// </summary>
        /// <param name="ID">The ID.</param>
        /// <returns></returns>
        public static IComponentList SelectOne(int ID)
        {
            var connector = ConnectorFactory.CreateConnector<ComponentList>();
            return connector.FetchSingle(ID);
        }

        /// <summary>
        /// Select a componentList based on it's primary key
        /// </summary>
        /// <param name="ID">The ID.</param>
        /// <returns></returns>
        public static async Task<IComponentList> SelectOneAsync(int ID)
        {
            var connector = ConnectorFactory.CreateConnector<ComponentList>();
            return await connector.FetchSingleAsync(ID).ConfigureAwait(false);
        }

        /// <summary>
        /// Select a componentList based on it's predefined type
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static IComponentList SelectOne(ComponentListType type)
        {
            var connector = ConnectorFactory.CreateConnector<ComponentList>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.Type, type);

            return connector.FetchSingle(filter);
        }

        /// <summary>
        /// Select a componentList based on it's predefined type Async
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static async Task<IComponentList> SelectOneAsync(ComponentListType type)
        {
            var connector = ConnectorFactory.CreateConnector<ComponentList>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.Type, type);

            return await connector.FetchSingleAsync(filter).ConfigureAwait(false);
        }

        /// <summary>
        /// Select a componentList based on it's class
        /// </summary>
        /// <param name="className">Name of the class.</param>
        /// <returns></returns>
        public static IComponentList SelectOne(string className)
        {
            var connector = ConnectorFactory.CreateConnector<ComponentList>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.ClassName, className);

            return connector.FetchSingle(filter);
        }

        /// <summary>
        /// Select a componentList based on it's name and it residing folder
        /// </summary>
        /// <returns></returns>
        public static IComponentList SelectOne(string name, int? folder)
        {
            var connector = ConnectorFactory.CreateConnector<ComponentList>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.Name, name);

            if (folder.HasValue)
            {
                filter.Add(x => x.FolderID, folder);
            }

            return connector.FetchSingle(filter);
        }

        /// <summary>
        /// Select a componentList based on it's name and it residing folder
        /// </summary>
        /// <returns></returns>
        public static async Task<IComponentList> SelectOneAsync(string name, int? folder)
        {
            var connector = ConnectorFactory.CreateConnector<ComponentList>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.Name, name);

            if (folder.HasValue)
            {
                filter.Add(x => x.FolderID, folder);
            }

            return await connector.FetchSingleAsync(filter).ConfigureAwait(false);
        }

        /// <summary>
        /// Select a componentList based on it's class Async
        /// </summary>
        /// <param name="className">Name of the class.</param>
        /// <returns></returns>
        public static async Task<IComponentList> SelectOneAsync(string className)
        {
            var connector = ConnectorFactory.CreateConnector<ComponentList>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.ClassName, className);

            return await connector.FetchSingleAsync(filter).ConfigureAwait(false);
        }

        /// <summary>
        /// Select a componentList based on it's type
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static IComponentList SelectOne(Type type)
        {
            return SelectOne(type.ToString());
        }

        /// <summary>
        /// Select a componentList based on it's type
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static async Task<IComponentList> SelectOneAsync(Type type)
        {
            return await SelectOneAsync(type.ToString()).ConfigureAwait(false);
        }

        /// <summary>
        /// Select a componentList based on it's reference
        /// </summary>
        /// <param name="referenceID">The reference ID.</param>
        /// <returns></returns>
        public static IComponentList SelectOneByReference(int referenceID)
        {
            var connector = ConnectorFactory.CreateConnector<ComponentList>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.ReferenceID, referenceID);

            return connector.FetchSingle(filter);
        }

        /// <summary>
        /// Select a componentList based on it's reference Async
        /// </summary>
        /// <param name="referenceID">The reference ID.</param>
        /// <returns></returns>
        public static async Task<IComponentList> SelectOneByReferenceAsync(int referenceID)
        {
            var connector = ConnectorFactory.CreateConnector<ComponentList>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.ReferenceID, referenceID);

            return await connector.FetchSingleAsync(filter).ConfigureAwait(false);
        }

        /// <summary>
        /// Select a componentList based on it's GUID
        /// </summary>
        /// <param name="componentListGUID">The component list GUID.</param>
        /// <returns></returns>
        public static IComponentList SelectOne(Guid componentListGUID)
        {
            var connector = ConnectorFactory.CreateConnector<ComponentList>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.GUID, componentListGUID);

            return connector.FetchSingle(filter);
        }

        /// <summary>
        /// Select a componentList based on it's GUID Async
        /// </summary>
        /// <param name="componentListGUID">The component list GUID.</param>
        /// <returns></returns>
        public static async Task<IComponentList> SelectOneAsync(Guid componentListGUID)
        {
            var connector = ConnectorFactory.CreateConnector<ComponentList>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.GUID, componentListGUID);

            return await connector.FetchSingleAsync(filter).ConfigureAwait(false);
        }

        /// <summary>
        /// Select a componentList based on it's GUID and assign a handler that creates a new list if it does not exists
        /// </summary>
        /// <param name="componentListGUID"></param>
        /// <param name="ifNotPresentCreate">The creation handler</param>
        /// <returns></returns>
        public virtual IComponentList SelectOne(Guid componentListGUID, Func<Guid, IComponentList> ifNotPresentCreate)
        {
            var candidate = (from item in SelectAll() where item.GUID == componentListGUID select item);
            IComponentList tmp;
            if (!candidate.Any())
            {
                if (ifNotPresentCreate == null)
                    tmp = new ComponentList();
                else
                    tmp = ifNotPresentCreate(componentListGUID);
            }
            else
                tmp = candidate.ToArray()[0];

            return tmp;
        }

        #endregion Static methods

        #region Methods

        /// <summary>
        /// Saves the entity. This method will indentify the need for an INSERT or an UPDATE.
        /// </summary>
        /// <returns></returns>
        public int Save()
        {
            if (m_Data != null)
                DataString = m_Data.Serialized;

            if (m_Settings != null)
                SettingsString = m_Settings.Serialized;


            var connector = ConnectorFactory.CreateConnector<ComponentList>();
            connector.Save(this);

            return this.ID;
        }

        /// <summary>
        /// Saves the entity Async. This method will indentify the need for an INSERT or an UPDATE.
        /// </summary>
        /// <returns></returns>
        public async Task<int> SaveAsync()
        {
            if (m_Data != null)
                DataString = m_Data.Serialized;

            if (m_Settings != null)
                SettingsString = m_Settings.Serialized;

            var connector = ConnectorFactory.CreateConnector<ComponentList>();
            await connector.SaveAsync(this).ConfigureAwait(false);

            return this.ID;
        }

        /// <summary>
        /// Determine if the user is allowed to view this componentList
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns>
        /// 	<c>true</c> if [has role access] [the specified role id]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasRoleAccess(IApplicationUser user)
        {
            if (ID == 0 || user.Role().All_Lists)
                return true;

            var selection = from item in SelectAllAccessibleLists(user, RoleRightType.List) where item.ID == ID select item;
            bool xs = selection.Count() == 1;
            return xs;
        }

        /// <summary>
        /// Determine if the user is allowed to view this componentList Async
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns>
        /// 	<c>true</c> if [has role access] [the specified role id]; otherwise, <c>false</c>.
        /// </returns>
        public async Task<bool> HasRoleAccessAsync(IApplicationUser user)
        {
            if (ID == 0 || user.Role().All_Lists)
                return true;

            var selection = from item in await SelectAllAccessibleListsAsync(user, RoleRightType.List)
                            .ConfigureAwait(false)
                            where item.ID == ID select item;
            bool xs = selection.Count() == 1;
            return xs;
        }

        /// <summary>
        /// Delete an implementation record.
        /// </summary>
        /// <returns></returns>
        public virtual bool Delete()
        {
            var connector = ConnectorFactory.CreateConnector<ComponentList>();
            connector.Delete(this);
            return true;
        }

        /// <summary>
        /// Delete an implementation record.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> DeleteAsync()
        {
            var connector = ConnectorFactory.CreateConnector<ComponentList>();
            await connector.DeleteAsync(this).ConfigureAwait(false);
            return true;
        }

        /// <summary>
        /// The complete path of this list in the portal
        /// </summary>
        /// <returns></returns>
        public virtual string CompletePath()
        {
            if (!FolderID.HasValue)
                return this.Name;

            return$"{Folder.SelectOne(FolderID.Value).CompletePath}{Name}";
        }

        #endregion Methods
    }
}