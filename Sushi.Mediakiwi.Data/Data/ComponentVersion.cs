using Sushi.MicroORM;
using Sushi.MicroORM.Mapping;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sushi.Mediakiwi.Data.Caching;
using Sushi.Mediakiwi.Data.MircoORM;

namespace Sushi.Mediakiwi.Data
{
    [DataMap(typeof(ComponentVersionMap))]
    public class ComponentVersion : IExportable, IComponent
    {
        public class ComponentVersionMap : DataMap<ComponentVersion>
        {
            public ComponentVersionMap() : this(false) { }
            
            public ComponentVersionMap(bool isSave)
            {
                if(isSave)
                    Table("wim_ComponentVersions");
                else
                    Table("wim_ComponentVersions join wim_ComponentTemplates on ComponentVersion_ComponentTemplate_Key = ComponentTemplate_Key ");

                Id(x => x.ID, "ComponentVersion_Key").Identity();
                Map(x => x.ApplicationUserID, "ComponentVersion_User_Key");
                Map(x => x.MasterID, "ComponentVersion_Master_Key");
                Map(x => x.Created, "ComponentVersion_Created");
                Map(x => x.Updated, "ComponentVersion_Updated");
                Map(x => x.GUID, "ComponentVersion_GUID");
                Map(x => x.PageID, "ComponentVersion_Page_Key");
                Map(x => x.AvailableTemplateID, "ComponentVersion_AvailableTemplate_Key");
                Map(x => x.TemplateID, "ComponentVersion_ComponentTemplate_Key");
                Map(x => x.IsFixed, "ComponentVersion_IsFixedOnTemplate");
                Map(x => x.IsAlive, "ComponentVersion_IsAlive");
                Map(x => x.IsActive, "ComponentVersion_IsActive");
                Map(x => x.SiteID, "ComponentVersion_Site_Key");
                Map(x => x.IsSecundary, "ComponentVersion_IsSecundary");
                Map(x => x.FixedFieldName, "ComponentVersion_Fixed_Id").Length(50);
                Map(x => x.Target, "ComponentVersion_Target").Length(25);
                Map(x => x.InstanceName, "ComponentVersion_Name").Length(50);
                Map(x => x.Serialized_XML, "ComponentVersion_XML");
                Map(x => x.SortField_Date, "ComponentVersion_SortDate");
                Map(x => x.SortOrder, "ComponentVersion_SortOrder");

                Map(x => x.Name, "ComponentTemplate_Name").Length(50).ReadOnly();
                Map(x => x.Source, "ComponentTemplate_Source").ReadOnly();
                Map(x => x.TemplateIsShared, "ComponentTemplate_IsShared").ReadOnly();
                Map(x => x.IsSearchable, "ComponentTemplate_IsSearchable").ReadOnly();
                Map(x => x.LastWriteTimeUtc, "ComponentTemplate_LastWriteTimeUtc").ReadOnly();
                Map(x => x.TemplateReferenceID, "ComponentTemplate_ReferenceId").ReadOnly();
                Map(x => x.TemplateLocation, "ComponentTemplate_Location").Length(250).ReadOnly();
            }
        }

        ComponentTemplate m_Template;
        /// <summary>
        /// Gets the template.
        /// </summary>
        /// <value>The template.</value>
        public ComponentTemplate Template
        {
            get
            {
                if (m_Template == null)
                    m_Template = ComponentTemplate.SelectOne(this.TemplateID);
                return m_Template;
            }
        }

        #region Properties
        /// <summary>
        /// Unique identifier of this componentVersion
        /// </summary>
        /// <value>The ID.</value>
        public int ID { get; set; }

        /// <summary>
        /// Gets or sets the application user ID.
        /// </summary>
        /// <value>The application user ID.</value>
        public int? ApplicationUserID { get; set; }

        /// <summary>
        /// The master componentversion identifier
        /// </summary>
        /// <value>The master ID.</value>
        public int? MasterID { get; set; }

        private DateTime m_Created;

        /// <summary>
        /// Gets or sets the created.
        /// </summary>
        /// <value>The created.</value>
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

        private DateTime m_Updated;

        public DateTime Updated
        {
            get
            {
                if (this.m_Updated == DateTime.MinValue)
                    this.m_Updated = Created;
                return m_Updated;
            }
            set { m_Updated = value; }
        }

        private Guid m_GUID;

        /// <summary>
        /// Global Unique identifier of this componentversion
        /// </summary>
        /// <value>The GUID.</value>
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

        /// <summary>
        /// The page to which this componentversion belongs
        /// </summary>
        /// <value>The page ID.</value>
        public int? PageID { get; set; }

        /// <summary>
        /// The corresponding componentTemplate identifier
        /// </summary>
        /// <value>The available template ID.</value>
        public int? AvailableTemplateID { get; set; }

        /// <summary>
        /// The corresponding componentTemplate identifier
        /// </summary>
        /// <value>The template ID.</value>
        public int TemplateID { get; set; }

        public bool TemplateIsShared { get; set; }

        /// <summary>
        /// The (HTML) source of this component Version
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// Is this component fixed on the page?
        /// </summary>
        /// <value><c>true</c> if this instance is fixed; otherwise, <c>false</c>.</value>
        public bool IsFixed { get; set; }

        /// <summary>
        /// Is this component alive? Alive: When a fixed component is removed from a page it is still stored.
        /// When this component is reintroduced this content will be restored.
        /// </summary>
        /// <value><c>true</c> if this instance is alive; otherwise, <c>false</c>.</value>
        public bool IsAlive { get; set; }

        /// <summary>
        /// Is this component alive? Alive: When a fixed component is removed from a page it is still stored.
        /// When this component is reintroduced this content will be restored.
        /// </summary>
        /// <value><c>true</c> if this instance is active; otherwise, <c>false</c>.</value>
        public bool IsActive { get; set; }

        /// <summary>
        /// To which site does this Compnent version belongs
        /// </summary>
        public int? SiteID { get; set; }

        /// <summary>
        /// Does this componentversion belong to the secundary container (like f.e. the service column)?
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is secundary; otherwise, <c>false</c>.
        /// </value>
        public bool IsSecundary { get; set; }

        /// <summary>
        /// The component template reference ID
        /// </summary>
        /// <value>The template reference ID.</value>
        public int TemplateReferenceID { get; set; }

        /// <summary>
        /// Is this component searchable?
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is searchable; otherwise, <c>false</c>.
        /// </value>
        public bool IsSearchable { get; set; }

        /// <summary>
        /// The identifying name of the fixed component
        /// </summary>
        /// <value>The name of the fixed field.</value>
        public string FixedFieldName { get; set; }

        /// <summary>
        /// The name of the Component Template
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        /// <summary>
        /// The target
        /// </summary>
        public string Target { get; set; }

        /// <summary>
        /// The name of this Component Version
        /// </summary>
        public string InstanceName { get; set; }

        /// <summary>
        /// Gets or sets the last write time UTC of the page template (ASPX).
        /// </summary>
        /// <value>The last write time UTC.</value>
        public DateTime LastWriteTimeUtc { get; set; }

        /// <summary>
        /// Gets or sets the template location.
        /// </summary>
        /// <value>The template location.</value>
        public string TemplateLocation { get; set; }

        /// <summary>
        /// The serialized content matching this componentversion
        /// </summary>
        /// <value>The serialized_ XML.</value>
        public string Serialized_XML { get; set; }

        /// <summary>
        /// The sortfield's introduced date.
        /// </summary>
        /// <value>The sort field_ date.</value>
        public DateTime? SortField_Date { get; set; }

        /// <summary>
        /// Sortorder
        /// </summary>
        /// <value>The sort order.</value>
        public int SortOrder { get; set; }

        DateTime? IExportable.Updated
        {
            get { return Updated; }
        }

        #endregion Properties



        /// <summary>
        /// Remove the components versions from a page if they are not assigned any more.
        /// </summary>
        /// <param name="pageID">The page ID.</param>
        internal static void RemoveInvalidPageReference(int pageID)
        {
            var connector = ConnectorFactory.CreateConnector<ComponentVersion>();
            var filter = connector.CreateDataFilter();
            filter.AddParameter("@pageId", pageID);

            connector.ExecuteNonQuery(@"
                DELETE FROM [wim_ComponentVersions]
                WHERE [ComponentVersion_Key] IN (
                    SELECT [ComponentVersion_Key] FROM [wim_ComponentVersions]
	                LEFT JOIN [wim_AvailableTemplates] ON [AvailableTemplates_Key] = [ComponentVersion_AvailableTemplate_Key]
                        AND ([AvailableTemplates_Target] = [ComponentVersion_Target] OR ([AvailableTemplates_Target] IS NULL AND [ComponentVersion_Target] IS NULL))
                        WHERE
	                        [ComponentVersion_Page_Key] = @pageId
	                    AND
                            [AvailableTemplates_Key] IS NULL AND [ComponentVersion_IsFixedOnTemplate] = 1)", filter);
			connector.Cache.FlushRegion(connector.CacheRegion);
        }

        /// <summary>
        /// Remove the components versions from a page if they are not assigned any more.
        /// </summary>
        /// <param name="pageID">The page ID.</param>
        internal static async Task RemoveInvalidPageReferenceAsync(int pageID)
        {
            var connector = ConnectorFactory.CreateConnector<ComponentVersion>();
            var filter = connector.CreateDataFilter();
            filter.AddParameter("@pageId", pageID);

            await connector.ExecuteNonQueryAsync(@"
                DELETE FROM [wim_ComponentVersions]
                WHERE [ComponentVersion_Key] IN (
                    SELECT [ComponentVersion_Key] FROM [wim_ComponentVersions]
	                LEFT JOIN [wim_AvailableTemplates] ON [AvailableTemplates_Key] = [ComponentVersion_AvailableTemplate_Key]
                        AND ([AvailableTemplates_Target] = [ComponentVersion_Target] OR ([AvailableTemplates_Target] IS NULL AND [ComponentVersion_Target] IS NULL))
                        WHERE
	                        [ComponentVersion_Page_Key] = @pageId
	                    AND
                            [AvailableTemplates_Key] IS NULL AND [ComponentVersion_IsFixedOnTemplate] = 1)", filter);
			connector.Cache.FlushRegion(connector.CacheRegion);
        }

        /// <summary>
        /// Updates the container target by available template.
        /// </summary>
        /// <param name="availableTemplateID">The available template ID.</param>
        /// <param name="target">The target.</param>
        internal static void UpdateContainerTargetByAvailableTemplate(int availableTemplateID, string target)
        {
            var connector = ConnectorFactory.CreateConnector<ComponentVersion>();
            var filter = connector.CreateDataFilter();
            filter.AddParameter("@templateId", availableTemplateID);

            if (string.IsNullOrEmpty(target))
            {
                connector.ExecuteNonQuery(@"UPDATE [wim_ComponentVersions] SET [ComponentVersion_Target] = null WHERE [ComponentVersion_AvailableTemplate_Key] = @templateId", filter);
            }
            else
            {
                filter.AddParameter("@target", target);
                connector.ExecuteNonQuery(@"UPDATE [wim_ComponentVersions] SET [ComponentVersion_Target] = '@target' WHERE [ComponentVersion_AvailableTemplate_Key] = @templateId", filter);
            }
			connector.Cache.FlushRegion(connector.CacheRegion);
        }

        /// <summary>
        /// Updates the container target by available template.
        /// </summary>
        /// <param name="availableTemplateID">The available template ID.</param>
        /// <param name="target">The target.</param>
        internal static async Task UpdateContainerTargetByAvailableTemplateAsync(int availableTemplateID, string target)
        {
            var connector = ConnectorFactory.CreateConnector<ComponentVersion>();
            var filter = connector.CreateDataFilter();
            filter.AddParameter("@templateId", availableTemplateID);

            if (string.IsNullOrEmpty(target))
            {
                await connector.ExecuteNonQueryAsync(@"UPDATE [wim_ComponentVersions] SET [ComponentVersion_Target] = null WHERE [ComponentVersion_AvailableTemplate_Key] = @templateId", filter);
            }
            else
            {
                filter.AddParameter("@target", target);
                await connector.ExecuteNonQueryAsync(@"UPDATE [wim_ComponentVersions] SET [ComponentVersion_Target] = '@target' WHERE [ComponentVersion_AvailableTemplate_Key] = @templateId", filter);
            }
			connector.Cache.FlushRegion(connector.CacheRegion);
        }

        /// <summary>
        /// Selects all.
        /// </summary>
        /// <returns></returns>
        public static ComponentVersion[] SelectAll()
        {
            var connector = ConnectorFactory.CreateConnector<ComponentVersion>();
            var filter = connector.CreateDataFilter();
            filter.AddOrder(x => x.SortOrder);

            return connector.FetchAll(filter).ToArray();
        }

        /// <summary>
        /// Selects all.
        /// </summary>
        /// <returns></returns>
        public static async Task<ComponentVersion[]> SelectAllAsync()
        {
            var connector = ConnectorFactory.CreateConnector<ComponentVersion>();
            var filter = connector.CreateDataFilter();
            filter.AddOrder(x => x.SortOrder);

            var result = await connector.FetchAllAsync(filter);
            return result.ToArray();
        }

        /// <summary>
        /// Selects all by Page ID and Target.
        /// </summary>
        /// <param name="pageID">The page ID.</param>
        /// <returns></returns>
        public static ComponentVersion[] SelectAll(int pageID, string target = null)
        {
            var connector = ConnectorFactory.CreateConnector<ComponentVersion>();
            var filter = connector.CreateDataFilter();
            filter.AddOrder(x => x.SortOrder);
            filter.Add(x => x.PageID, pageID);

            if (!string.IsNullOrEmpty(target))
                filter.Add(x => x.Target, target);

            return connector.FetchAll(filter).ToArray();
        }

        /// <summary>
        /// Selects all by Page ID and Target Async.
        /// </summary>
        /// <param name="pageID">The page ID.</param>
        /// <returns></returns>
        public static async Task<ComponentVersion[]> SelectAllAsync(int pageID, string target = null)
        {
            var connector = ConnectorFactory.CreateConnector<ComponentVersion>();
            var filter = connector.CreateDataFilter();
            filter.AddOrder(x => x.SortOrder);
            filter.Add(x => x.PageID, pageID);

            if (!string.IsNullOrEmpty(target))
                filter.Add(x => x.Target, target);

            var result = await connector.FetchAllAsync(filter);
            return result.ToArray();
        }

        /// <summary>
        /// Selecta all by Site ID
        /// </summary>
        /// <param name="siteID"></param>
        /// <returns></returns>
        public static ComponentVersion[] SelectAllSharedForSite(int siteID)
        {
            var connector = ConnectorFactory.CreateConnector<ComponentVersion>();
            var filter = connector.CreateDataFilter();
            filter.AddOrder(x => x.SortOrder);
            filter.Add(x => x.PageID, null);
            filter.Add(x => x.TemplateIsShared, true);
            filter.AddSql($"([ComponentVersion_Site_Key] IS NULL OR [ComponentVersion_Site_Key] = {siteID})");

            return connector.FetchAll(filter).ToArray();
        }

        /// <summary>
        /// Selecta all by Site ID Async
        /// </summary>
        /// <param name="siteID"></param>
        /// <returns></returns>
        public static async Task<ComponentVersion[]> SelectAllSharedForSiteAsync(int siteID)
        {
            var connector = ConnectorFactory.CreateConnector<ComponentVersion>();
            var filter = connector.CreateDataFilter();
            filter.AddOrder(x => x.SortOrder);
            filter.Add(x => x.PageID, null);
            filter.Add(x => x.TemplateIsShared, true);
            filter.AddSql($"([ComponentVersion_Site_Key] IS NULL OR [ComponentVersion_Site_Key] = {siteID})");

            var result = await connector.FetchAllAsync(filter);
            return result.ToArray();
        }

        /// <summary>
        /// Selects all.
        /// </summary>
        /// <param name="pageID">The page ID.</param>
        /// <param name="isSecundary">if set to <c>true</c> [is secundary].</param>
        /// <returns></returns>
        public static ComponentVersion[] SelectAll(int pageID, bool isSecundary)
        {
            var connector = ConnectorFactory.CreateConnector<ComponentVersion>();
            var filter = connector.CreateDataFilter();
            filter.AddOrder(x => x.SortOrder);
            filter.Add(x => x.PageID, pageID);
            filter.Add(x => x.IsSecundary, isSecundary);

            return connector.FetchAll(filter).ToArray();
        }

        /// <summary>
        /// Selects all.
        /// </summary>
        /// <param name="pageID">The page ID.</param>
        /// <param name="isSecundary">if set to <c>true</c> [is secundary].</param>
        /// <returns></returns>
        public static async Task<ComponentVersion[]> SelectAllAsync(int pageID, bool isSecundary)
        {
            var connector = ConnectorFactory.CreateConnector<ComponentVersion>();
            var filter = connector.CreateDataFilter();
            filter.AddOrder(x => x.SortOrder);
            filter.Add(x => x.PageID, pageID);
            filter.Add(x => x.IsSecundary, isSecundary);

            var result = await connector.FetchAllAsync(filter);
            return result.ToArray();
        }

        /// <summary>
        /// Applies the specified component.
        /// </summary>
        /// <param name="component">The component.</param>
        public void Apply(Component component)
        {
            component.GUID = this.GUID;
            component.PageID = this.PageID;
            component.ComponentTemplateID = this.TemplateID;
            component.Name = this.Name;
            component.IsFixedOnTemplate = this.IsFixed;
            component.IsSecundaryContainerItem = this.IsSecundary;
            component.IsSearchable = this.IsSearchable;
            component.FixedId = this.FixedFieldName;
            component.Location = this.TemplateLocation;
            component.Serialized_XML = this.Serialized_XML;
            component.SortDate = this.SortField_Date;
            component.SortOrder = this.SortOrder;
            component.Updated = this.Updated;
            component.Target = this.Target;
            component.Source = this.Source;
        }

        public bool Delete()
        {
            var connector = ConnectorFactory.CreateConnector<ComponentVersion>(new ComponentVersionMap(true));
            try
            {
                connector.Delete(this);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> DeleteAsync()
        {
            var connector = ConnectorFactory.CreateConnector<ComponentVersion>(new ComponentVersionMap(true));
            try
            {
                await connector.DeleteAsync(this);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
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
        public static ComponentVersion SelectOneBasedOnType(int pageID, Type type)
        {
            var connector = ConnectorFactory.CreateConnector<ComponentVersion>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.PageID, pageID);
            filter.Add("[ComponentTemplate_Type]", System.Data.SqlDbType.NVarChar, type.BaseType.ToString());

            return connector.FetchSingle(filter);
        }

        /// <summary>
        /// Select a component version on page based System.Type
        /// </summary>
        /// <param name="pageID">The page ID.</param>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static async Task<ComponentVersion> SelectOneBasedOnTypeAsync(int pageID, Type type)
        {
            var connector = ConnectorFactory.CreateConnector<ComponentVersion>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.PageID, pageID);
            filter.Add("[ComponentTemplate_Type]", System.Data.SqlDbType.NVarChar, type.BaseType.ToString());

            return await connector.FetchSingleAsync(filter);
        }

        /// <summary>
        /// Select a fixed componentversion object
        /// </summary>
        /// <param name="pageID">The page ID.</param>
        /// <param name="componentversionFixedName">name of the fixed componentversion</param>
        /// <returns>a componentversion object</returns>
        public static ComponentVersion SelectOneFixed(int pageID, string componentversionFixedName)
        {
            var connector = ConnectorFactory.CreateConnector<ComponentVersion>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.FixedFieldName, componentversionFixedName);
            filter.Add(x => x.PageID, pageID);
            filter.Add(x => x.IsFixed, true);

            return connector.FetchSingle(filter);
        }

        /// <summary>
        /// Select a fixed componentversion object Async
        /// </summary>
        /// <param name="pageID">The page ID.</param>
        /// <param name="componentversionFixedName">name of the fixed componentversion</param>
        /// <returns>a componentversion object</returns>
        public static async Task<ComponentVersion> SelectOneFixedAsync(int pageID, string componentversionFixedName)
        {
            var connector = ConnectorFactory.CreateConnector<ComponentVersion>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.FixedFieldName, componentversionFixedName);
            filter.Add(x => x.PageID, pageID);
            filter.Add(x => x.IsFixed, true);

            return await connector.FetchSingleAsync(filter);
        }

        /// <summary>
        /// Select a single componentversion instance
        /// </summary>
        /// <param name="ID">The ID.</param>
        /// <returns>componentversion object</returns>
        public static ComponentVersion SelectOne(int ID)
        {
            var connector = ConnectorFactory.CreateConnector<ComponentVersion>();

            return connector.FetchSingle(ID);
        }

        /// <summary>
        /// Select a single componentversion instance
        /// </summary>
        /// <param name="ID">The ID.</param>
        /// <returns>componentversion object</returns>
        public static async Task<ComponentVersion> SelectOneAsync(int ID)
        {
            var connector = ConnectorFactory.CreateConnector<ComponentVersion>();

            return await connector.FetchSingleAsync(ID);
        }

        /// <summary>
        /// Select a single componentversion instance By GUID
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public static ComponentVersion SelectOne(Guid guid)
        {
            var connector = ConnectorFactory.CreateConnector<ComponentVersion>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.GUID, guid);

            return connector.FetchSingle(filter);
        }

        /// <summary>
        /// Select a single componentversion instance By GUID
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public static async Task<ComponentVersion> SelectOneAsync(Guid guid)
        {
            var connector = ConnectorFactory.CreateConnector<ComponentVersion>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.GUID, guid);

            return await connector.FetchSingleAsync(filter);
        }

        /// <summary>
        /// Select a single componentversion instance By Component Template ID
        /// </summary>
        /// <param name="componentTemplateID"></param>
        /// <returns></returns>
        public static ComponentVersion SelectOneShared(int componentTemplateID)
        {
            var connector = ConnectorFactory.CreateConnector<ComponentVersion>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.PageID, null);
            filter.Add(x => x.TemplateID, componentTemplateID);

            return connector.FetchSingle(filter);
        }

        /// <summary>
        /// Select a single componentversion instance By Component Template ID Async
        /// </summary>
        /// <param name="componentTemplateID"></param>
        /// <returns></returns>
        public static async Task<ComponentVersion> SelectOneSharedAsync(int componentTemplateID)
        {
            var connector = ConnectorFactory.CreateConnector<ComponentVersion>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.PageID, null);
            filter.Add(x => x.TemplateID, componentTemplateID);

            return await connector.FetchSingleAsync(filter);
        }

        /// <summary>
        /// Select all componentversions on a page
        /// </summary>
        /// <param name="pageID">The page ID.</param>
        /// <returns>array of componentversions on a page</returns>
        public static ComponentVersion[] SelectAllOnPage(int pageID)
        {
            var connector = ConnectorFactory.CreateConnector<ComponentVersion>();
            var filter = connector.CreateDataFilter();
            filter.AddOrder(x => x.SortOrder);
            filter.Add(x => x.PageID, pageID);
            filter.Add(x => x.IsAlive, true);

            return connector.FetchAll(filter).ToArray();
        }

        /// <summary>
        /// Select all componentversions on a page Async
        /// </summary>
        /// <param name="pageID">The page ID.</param>
        /// <returns>array of componentversions on a page</returns>
        public static async Task<ComponentVersion[]> SelectAllOnPageAsync(int pageID)
        {
            var connector = ConnectorFactory.CreateConnector<ComponentVersion>();
            var filter = connector.CreateDataFilter();
            filter.AddOrder(x => x.SortOrder);
            filter.Add(x => x.PageID, pageID);
            filter.Add(x => x.IsAlive, true);

            var result = await connector.FetchAllAsync(filter);
            return result.ToArray();
        }

        /// <summary>
        /// Deletes all on page.
        /// </summary>
        /// <param name="pageKey">The page key.</param>
        /// <returns></returns>
        public static bool DeleteAllOnPage(int pageKey)
        {
            var connector = ConnectorFactory.CreateConnector<ComponentVersion>(new ComponentVersionMap(true));
            var filter = connector.CreateDataFilter();
            filter.AddParameter("@pageId", pageKey);

            try
            {
                connector.ExecuteNonQuery("DELETE FROM [wim_ComponentVersions] WHERE [ComponentVersion_Page_Key] = @pageId", filter);
				connector.Cache.FlushRegion(connector.CacheRegion);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Deletes all on page.
        /// </summary>
        /// <param name="pageKey">The page key.</param>
        /// <returns></returns>
        public static async Task<bool> DeleteAllOnPageAsync(int pageKey)
        {
            var connector = ConnectorFactory.CreateConnector<ComponentVersion>(new ComponentVersionMap(true));
            var filter = connector.CreateDataFilter();
            filter.AddParameter("@pageId", pageKey);

            try
            {
                await connector.ExecuteNonQueryAsync("DELETE FROM [wim_ComponentVersions] WHERE [ComponentVersion_Page_Key] = @pageId", filter);
				connector.Cache.FlushRegion(connector.CacheRegion);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Select all componentversions on a page
        /// </summary>
        /// <param name="pageID">The page ID.</param>
        /// <param name="isSecundary">should return secundary container componentversions?</param>
        /// <returns>array of componentversion on a page</returns>
        public static ComponentVersion[] SelectAllOnPage(int pageID, bool isSecundary)
        {
            var connector = ConnectorFactory.CreateConnector<ComponentVersion>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.IsSecundary, isSecundary);
            filter.Add(x => x.PageID, pageID);
            filter.Add(x => x.IsAlive, true);

            return connector.FetchAll(filter).ToArray();
        }

        /// <summary>
        /// Select all componentversions on a page
        /// </summary>
        /// <param name="pageID">The page ID.</param>
        /// <param name="isSecundary">should return secundary container componentversions?</param>
        /// <returns>array of componentversion on a page</returns>
        public static async Task<ComponentVersion[]> SelectAllOnPageAsync(int pageID, bool isSecundary)
        {
            var connector = ConnectorFactory.CreateConnector<ComponentVersion>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.IsSecundary, isSecundary);
            filter.Add(x => x.PageID, pageID);
            filter.Add(x => x.IsAlive, true);

            var result = await connector.FetchAllAsync(filter);
            return result.ToArray();
        }

        /// <summary>
        /// Select all fixed componentversion on a page
        /// </summary>
        /// <param name="pageID">The page ID.</param>
        /// <param name="setAliveTemporaryOff">Should the isAlive property be turned off for all returned objects?</param>
        /// <returns>
        /// An array of fixed componentversions on a page
        /// </returns>
        public static ComponentVersion[] SelectAllFixed(int pageID, bool setAliveTemporaryOff)
        {
            var connector = ConnectorFactory.CreateConnector<ComponentVersion>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.IsFixed, true);
            filter.Add(x => x.PageID, pageID);

            var result = connector.FetchAll(filter);
            List<ComponentVersion> temp = new List<ComponentVersion>();

            foreach (var item in result)
            {
                if (setAliveTemporaryOff)
                    item.IsActive = false;
                temp.Add(item);
            }
            return temp.ToArray();
        }

        /// <summary>
        /// Select all fixed componentversion on a page
        /// </summary>
        /// <param name="pageID">The page ID.</param>
        /// <param name="setAliveTemporaryOff">Should the isAlive property be turned off for all returned objects?</param>
        /// <returns>
        /// An array of fixed componentversions on a page
        /// </returns>
        public static async Task<ComponentVersion[]> SelectAllFixedAsync(int pageID, bool setAliveTemporaryOff)
        {
            var connector = ConnectorFactory.CreateConnector<ComponentVersion>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.IsFixed, true);
            filter.Add(x => x.PageID, pageID);

            var result = await connector.FetchAllAsync(filter);
            List<ComponentVersion> temp = new List<ComponentVersion>();

            foreach (var item in result)
            {
                if (setAliveTemporaryOff)
                    item.IsActive = false;
                temp.Add(item);
            }
            return temp.ToArray();
        }

        /// <summary>
        /// Saves this instance
        /// </summary>
        public void Save()
        {
            var connector = ConnectorFactory.CreateConnector<ComponentVersion>(new ComponentVersionMap(true));
            connector.Save(this);
        }

        /// <summary>
        /// Saves this instance Async
        /// </summary>
        public async Task SaveAsync()
        {
            var connector = ConnectorFactory.CreateConnector<ComponentVersion>(new ComponentVersionMap(true));
            await connector.SaveAsync(this);
        }

        public static ComponentVersion[] SelectAllShared(int pageID)
        {
            return SelectAllShared(pageID, false);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageID"></param>
        /// <returns></returns>
        public static ComponentVersion[] SelectAllShared(int pageID, bool onlyAlive)
        {
            var connector = ConnectorFactory.CreateConnector<ComponentVersion>();

            Page page = Page.SelectOne(pageID, false);

            //  If the page is set to hold inherited content, please take that content
            if (page.InheritContent && page.MasterID.HasValue)
                pageID = page.MasterID.Value;

            var filter = connector.CreateDataFilter();
            filter.AddParameter("@pageId", pageID);
            filter.AddParameter("@isShared", true);

            // [MR:24-01-2020] not my proudest moment, though not sure how to resolve better.
            string additionalWhereClause = "";
            if (onlyAlive) 
            {
                filter.AddParameter("@isAlive", true);
                additionalWhereClause = "AND [ComponentVersion_IsAlive] = @isAlive";
            }

            var result = connector.FetchAll($@"
                SELECT [wim_ComponentVersions].* FROM [wim_ComponentVersions] 
                JOIN [wim_ComponentTargets] ON [ComponentVersion_GUID] = [ComponentTarget_Component_Source]
				LEFT JOIN [dbo].[wim_ComponentTemplates] ON [ComponentTemplate_Key] = Component_ComponentTemplate_Key
                WHERE [ComponentTarget_Page_Key] = @pageId
                AND [ComponentTemplate_IsShared] = @isShared
                {additionalWhereClause}", filter);

            // [MR:24-01-2020] this should go !
            //if (!string.IsNullOrEmpty(SqlConnectionString2)) component.SqlConnectionString = SqlConnectionString2;
            foreach (ComponentVersion cv in result)
            {
                if (page.InheritContent && page.MasterID.HasValue) 
                    cv.PageID = page.ID;
            }

            return result.ToArray();
        }

        public static async Task<ComponentVersion[]> SelectAllSharedAsync(int pageID)
        {
            return await SelectAllSharedAsync(pageID, false);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageID"></param>
        /// <returns></returns>
        public static async Task<ComponentVersion[]> SelectAllSharedAsync(int pageID, bool onlyAlive)
        {
            var connector = ConnectorFactory.CreateConnector<ComponentVersion>();

            Page page = await Page.SelectOneAsync(pageID, false);

            //  If the page is set to hold inherited content, please take that content
            if (page.InheritContent && page.MasterID.HasValue)
                pageID = page.MasterID.Value;

            var filter = connector.CreateDataFilter();
            filter.AddParameter("@pageId", pageID);
            filter.AddParameter("@isShared", true);

            // [MR:24-01-2020] not my proudest moment, though not sure how to resolve better.
            string additionalWhereClause = "";
            if (onlyAlive)
            {
                filter.AddParameter("@isAlive", true);
                additionalWhereClause = "AND [ComponentVersion_IsAlive] = @isAlive";
            }

            var result = await connector.FetchAllAsync($@"
                SELECT [wim_ComponentVersions].* FROM [wim_ComponentVersions] 
                JOIN [wim_ComponentTargets] ON [ComponentVersion_GUID] = [ComponentTarget_Component_Source]
				LEFT JOIN [dbo].[wim_ComponentTemplates] ON [ComponentTemplate_Key] = Component_ComponentTemplate_Key
                WHERE [ComponentTarget_Page_Key] = @pageId
                AND [ComponentTemplate_IsShared] = @isShared
                {additionalWhereClause}", filter);

            // [MR:24-01-2020] this should go !
            //if (!string.IsNullOrEmpty(SqlConnectionString2)) component.SqlConnectionString = SqlConnectionString2;
            foreach (ComponentVersion cv in result)
            {
                if (page.InheritContent && page.MasterID.HasValue)
                    cv.PageID = page.ID;
            }

            return result.ToArray();
        }

        /// <summary>
        /// Gets the content.
        /// </summary>
        /// <returns></returns>
        public Content GetContent()
        {
            if (Serialized_XML == null || Serialized_XML.Trim().Length == 0) return null;
            return Content.GetDeserialized(Serialized_XML);
        }
    }
}