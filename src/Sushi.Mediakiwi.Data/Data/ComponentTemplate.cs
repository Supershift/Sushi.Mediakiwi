using Sushi.Mediakiwi.Data.MicroORM;
using Sushi.MicroORM.Mapping;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Data
{
    [DataMap(typeof(ComponentTemplateMap))]
    public class ComponentTemplate : IExportable
    {
        public class ComponentTemplateMap : DataMap<ComponentTemplate>
        {
            public ComponentTemplateMap()
            {
                Table("wim_ComponentTemplates");
                Id(x => x.ID, "ComponentTemplate_Key").Identity().SqlType(SqlDbType.Int);
                Map(x => x.Name, "ComponentTemplate_Name").SqlType(SqlDbType.NVarChar).Length(50);
                Map(x => x.Location, "ComponentTemplate_Location").SqlType(SqlDbType.VarChar).Length(50);
                Map(x => x.TypeDefinition, "ComponentTemplate_Type").SqlType(SqlDbType.NVarChar).Length(250);
                Map(x => x.SourceTag, "ComponentTemplate_Tag").SqlType(SqlDbType.VarChar).Length(25);
                Map(x => x.Source, "ComponentTemplate_Source").SqlType(SqlDbType.NText);
                Map(x => x.ReferenceID, "ComponentTemplate_ReferenceId").SqlType(SqlDbType.Int);
                Map(x => x.IsSearchable, "ComponentTemplate_IsSearchable").SqlType(SqlDbType.Bit);
                Map(x => x.SiteID, "ComponentTemplate_Site_Key").SqlType(SqlDbType.Int);
                Map(x => x.IsFixedOnPage, "ComponentTemplate_IsFixed").SqlType(SqlDbType.Bit);
                Map(x => x.Description, "ComponentTemplate_Description").SqlType(SqlDbType.NVarChar).Length(500);
                Map(x => x.CanReplicate, "ComponentTemplate_CanReplicate").SqlType(SqlDbType.Bit);
                Map(x => x.CacheLevel, "ComponentTemplate_CacheLevel").SqlType(SqlDbType.Int);
                Map(x => x.OutputCacheParams, "ComponentTemplate_CacheParams").SqlType(SqlDbType.VarChar).Length(50);
                Map(x => x.CanDeactivate, "ComponentTemplate_CanDeactivate").SqlType(SqlDbType.Bit);
                Map(x => x.AjaxType, "ComponentTemplate_AjaxType").SqlType(SqlDbType.Int);
                Map(x => x.CanMoveUpDown, "ComponentTemplate_CanMove").SqlType(SqlDbType.Bit);
                Map(x => x.IsHeader, "ComponentTemplate_IsHeader").SqlType(SqlDbType.Bit);
                Map(x => x.IsFooter, "ComponentTemplate_IsFooter").SqlType(SqlDbType.Bit);
                Map(x => x.IsSecundaryContainerItem, "ComponentTemplate_IsSecundaryContainerItem").SqlType(SqlDbType.Bit);
                Map(x => x.IsShared, "ComponentTemplate_IsShared").SqlType(SqlDbType.Bit);
                Map(x => x.GUID, "ComponentTemplate_GUID").SqlType(SqlDbType.UniqueIdentifier);
                Map(x => x.IsListTemplate, "ComponentTemplate_IsListTemplate").SqlType(SqlDbType.Bit);
                Map(x => x.MetaData, "ComponentTemplate_MetaData").SqlType(SqlDbType.NText);
                Map(x => x.LastWriteTimeUtc, "ComponentTemplate_LastWriteTimeUtc").SqlType(SqlDbType.DateTime);
                Map(x => x.NestedType, "ComponentTemplate_NestType").SqlType(SqlDbType.Int);
            }
        }

        #region properties

        /// <summary>
        /// Gets or sets the ID.
        /// </summary>
        /// <value>The ID.</value>
        public int ID { get; set; }

        /// <summary>
        /// Name of the template
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the nested type setting
        /// </summary>
        /// <value>The ID.</value>
        public int? NestedType { get; set; }

        /// <summary>
        /// Relative path of the component template file (.ascx)
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// Template type definition.
        /// </summary>
        public string TypeDefinition { get; set; }

        public string SourceTag { get; set; }

        public string Source { get; set; }

        /// <summary>
        /// Gets or sets the reference id.
        /// </summary>
        /// <value>The reference id.</value>
        public int? ReferenceID { get; set; }

        public bool HasEditableSource { get; set; }

        /// <summary>
        /// The possibility to search the content of this template
        /// </summary>
        public bool IsSearchable { get; set; }

        /// <summary>
        /// Part of a specific site or site group (when the site is a master site with children this template is automatically part of those children).
        /// </summary>
        public int? SiteID { get; set; }

        /// <summary>
        /// Is the template fixed on a page?
        /// </summary>
        public bool IsFixedOnPage { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance can replicate.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can replicate; otherwise, <c>false</c>.
        /// </value>
        public bool CanReplicate { get; set; }

        /// <summary>
        /// Gets or sets the cache level.
        /// 0 = no cache, 1 = component based cache, 2 = page level cache
        /// </summary>
        /// <value>The cache level.</value>
        public int CacheLevel { get; set; }

        /// <summary>
        /// Gets the cache level info.
        /// </summary>
        /// <value>The cache level info.</value>
        public string CacheLevelInfo
        {
            get
            {
                if (CacheLevel == (int)CacheLevelType.Component || CacheLevel == (int)CacheLevelType.Page)
                {
                    CacheLevelType c = (CacheLevelType)CacheLevel;
                    return c.ToString("g");
                }
                else
                {
                    return CacheLevelType.None.ToString("g");
                }
            }
        }

        /// <summary>
        /// Gets or sets the output cache params.
        /// </summary>
        /// <value>The output cache params.</value>
        public string OutputCacheParams { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance can deactivate.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can deactivate; otherwise, <c>false</c>.
        /// </value>
        public bool CanDeactivate { get; set; }

        /// <summary>
        /// Gets or sets the type of the ajax.
        /// </summary>
        /// <value>The type of the ajax.</value>
        public int AjaxType { get; set; }

        public bool HasAjaxCall
        {
            get { return this.AjaxType > 0; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance can move up down.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can move up down; otherwise, <c>false</c>.
        /// </value>
        public bool CanMoveUpDown { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is header.
        /// </summary>
        /// <value><c>true</c> if this instance is header; otherwise, <c>false</c>.</value>
        public bool IsHeader { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is footer.
        /// </summary>
        /// <value><c>true</c> if this instance is footer; otherwise, <c>false</c>.</value>
        public bool IsFooter { get; set; }

        /// <summary>
        /// Is this component template build for the secundary (mostly the service column) container?
        /// Only used if applied as page component.
        public bool IsSecundaryContainerItem { get; set; }

        public bool IsShared { get; set; }

        private Guid m_GUID;

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

        /// <summary>
        /// Is this template basis for a list template?
        /// </summary>
        public bool IsListTemplate { get; set; }

        /// <summary>
        /// Gets or sets the meta data.
        /// </summary>
        /// <value>The meta data.</value>
        public string MetaData { get; set; }

        /// <summary>
        /// Gets or sets the last write time UTC of the page template (ASPX).
        /// </summary>
        /// <value>The last write time UTC.</value>
        public DateTime? LastWriteTimeUtc { get; set; }

        public string Source2 { get; set; }

        public DateTime? Updated
        {
            get { return LastWriteTimeUtc; }
        }

        public bool IsNewInstance
        {
            get { return (this?.ID > 0) == false; }
        }

        #endregion properties

        /// <summary>
        /// Select a ComponentTemplate based on its primary key
        /// </summary>
        /// <param name="ID">Uniqe identifier of the ComponentTemplate</param>
        /// <returns></returns>
        public static ComponentTemplate SelectOne(int ID)
        {
            var connector = ConnectorFactory.CreateConnector<ComponentTemplate>();
            return connector.FetchSingle(ID);
        }

        /// <summary>
        /// Select a ComponentTemplate based on its primary key
        /// </summary>
        /// <param name="ID">Uniqe identifier of the ComponentTemplate</param>
        public static async Task<ComponentTemplate> SelectOneAsync(int ID)
        {
            var connector = ConnectorFactory.CreateConnector<ComponentTemplate>();
            return await connector.FetchSingleAsync(ID);
        }

        /// <summary>
        /// Select a Component Template instance based on its migration (GUID) key
        /// </summary>
        /// <param name="componentTemplateGUID">The component template GUID.</param>
        public static ComponentTemplate SelectOne(Guid componentTemplateGUID)
        {
            var connector = ConnectorFactory.CreateConnector<ComponentTemplate>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.GUID, componentTemplateGUID);
            return connector.FetchSingle(filter);
        }

        /// <summary>
        /// Select a Component Template instance based on its migration (GUID) key
        /// </summary>
        /// <param name="componentTemplateGUID">The component template GUID.</param>
        public static async Task<ComponentTemplate> SelectOneAsync(Guid componentTemplateGUID)
        {
            var connector = ConnectorFactory.CreateConnector<ComponentTemplate>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.GUID, componentTemplateGUID);
            return await connector.FetchSingleAsync(filter);
        }

        /// <summary>
        /// Selects one Template by the supplied Reference ID
        /// </summary>
        /// <param name="referenceId"></param>
        /// <returns></returns>
        public static ComponentTemplate SelectOneByReference(int referenceId)
        {
            var connector = ConnectorFactory.CreateConnector<ComponentTemplate>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.ReferenceID, referenceId);
            return connector.FetchSingle(filter);
        }

        /// <summary>
        /// Selects one Template by the supplied Reference ID Async
        /// </summary>
        /// <param name="referenceId"></param>
        /// <returns></returns>
        public static async Task<ComponentTemplate> SelectOneByReferenceAsync(int referenceId)
        {
            var connector = ConnectorFactory.CreateConnector<ComponentTemplate>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.ReferenceID, referenceId);
            return await connector.FetchSingleAsync(filter);
        }

        public static ComponentTemplate SelectOneBySourceTag(string sourceTag)
        {
            var connector = ConnectorFactory.CreateConnector<ComponentTemplate>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.SourceTag, sourceTag);
            return connector.FetchSingle(filter);
        }

        public static async Task<ComponentTemplate> SelectOneBySourceTagAsync(string sourceTag)
        {
            var connector = ConnectorFactory.CreateConnector<ComponentTemplate>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.SourceTag, sourceTag);
            return await connector.FetchSingleAsync(filter);
        }

        /// <summary>
        /// Select a Component Template instance based on its type
        /// </summary>
        public static ComponentTemplate SelectOne_BasedOnType(Type type)
        {
            var connector = ConnectorFactory.CreateConnector<ComponentTemplate>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.TypeDefinition, type.ToString());
            return connector.FetchSingle(filter);
        }

        public static async Task<ComponentTemplate> SelectOne_BasedOnTypeAsync(Type type)
        {
            var connector = ConnectorFactory.CreateConnector<ComponentTemplate>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.TypeDefinition, type.ToString());
            return await connector.FetchSingleAsync(filter);
        }

        /// <summary>
        /// Selects all.
        /// </summary>
        public static ComponentTemplate[] SelectAll()
        {
            var connector = ConnectorFactory.CreateConnector<ComponentTemplate>();
            var filter = connector.CreateDataFilter();
            return connector.FetchAll(filter).ToArray();
        }

        /// <summary>
        /// Selects all.
        /// </summary>
        public static async Task<ComponentTemplate[]> SelectAllAsync()
        {
            var connector = ConnectorFactory.CreateConnector<ComponentTemplate>();
            var filter = connector.CreateDataFilter();
            var result = await connector.FetchAllAsync(filter);
            return result.ToArray();
        }

        /// <summary>
        /// Selects the all_ available.
        /// </summary>
        /// <param name="pageTemplateID">The page template ID.</param>
        public static ComponentTemplate[] SelectAllAvailable(int pageTemplateID)
        {
            //  Get all templates
            var all_template = SelectAll();
            //  Get all available templates
            var all_availableTemplate = AvailableTemplate.SelectAll();
            //  Select all available templates based on the page template ID
            var sel_availableTemplate =
                from availableTemplate in all_availableTemplate
                where availableTemplate.PageTemplateID == pageTemplateID && availableTemplate.IsPossible == true
                select availableTemplate;
            //  Select all templates based on the available templates
            var sel_template =
                from template in all_template
                join availableTemplate in sel_availableTemplate on template.ID equals availableTemplate.ComponentTemplateID
                select template;

            return sel_template.ToArray();
        }

        /// <summary>
        /// Selects the all_ available.
        /// </summary>
        /// <param name="pageTemplateID">The page template ID.</param>
        public static async Task<ComponentTemplate[]> SelectAllAvailableAsync(int pageTemplateID)
        {
            //  Get all templates
            var all_template = await SelectAllAsync();
            //  Get all available templates
            var all_availableTemplate = await AvailableTemplate.SelectAllAsync();
            //  Select all available templates based on the page template ID
            var sel_availableTemplate =
                from availableTemplate in all_availableTemplate
                where availableTemplate.PageTemplateID == pageTemplateID && availableTemplate.IsPossible == true
                select availableTemplate;
            //  Select all templates based on the available templates
            var sel_template =
                from template in all_template
                join availableTemplate in sel_availableTemplate on template.ID equals availableTemplate.ComponentTemplateID
                select template;

            return sel_template.ToArray();
        }

        /// <summary>
        /// Selects the all available.
        /// </summary>
        /// <param name="pageTemplateId">The page template id.</param>
        /// <param name="isSecundary">if set to <c>true</c> [is secundary].</param>
        public static ComponentTemplate[] SelectAllAvailable(int pageTemplateId, bool isSecundary)
        {
            return SelectAllAvailable(pageTemplateId, isSecundary, false);
        }

        /// <summary>
        /// Selects the all available.
        /// </summary>
        /// <param name="pageTemplateId">The page template id.</param>
        /// <param name="isSecundary">if set to <c>true</c> [is secundary].</param>
        public static async Task<ComponentTemplate[]> SelectAllAvailableAsync(int pageTemplateId, bool isSecundary)
        {
            return await SelectAllAvailableAsync(pageTemplateId, isSecundary, false);
        }

        /// <summary>
        /// Selects the all available.
        /// </summary>
        /// <param name="pageTemplateId">The page template id.</param>
        /// <param name="isSecundary">if set to <c>true</c> [is secundary].</param>
        /// <param name="isPresent">if set to <c>true</c> [is present].</param>
        public static ComponentTemplate[] SelectAllAvailable(int pageTemplateID, bool isSecundary, bool isPresent)
        {
            //[MJ:03-01-2020] TEST this method !
            //  Get all templates
            var all_template = SelectAll();
            //  Get all available templates
            var all_availableTemplate = AvailableTemplate.SelectAll();
            //  Select all available templates based on the page template ID
            var sel_availableTemplate =
                from availableTemplate in all_availableTemplate
                where availableTemplate.PageTemplateID == pageTemplateID
                && availableTemplate.IsPossible == true
                && availableTemplate.IsPresent == isPresent
                && availableTemplate.IsSecundary == isSecundary
                select availableTemplate;
            //  Select all templates based on the available templates
            var sel_template =
                from template in all_template
                join availableTemplate in sel_availableTemplate on template.ID equals availableTemplate.ComponentTemplateID
                select template;

            return sel_template.ToArray();
        }

        /// <summary>
        /// Selects the all available.
        /// </summary>
        /// <param name="pageTemplateId">The page template id.</param>
        /// <param name="isSecundary">if set to <c>true</c> [is secundary].</param>
        /// <param name="isPresent">if set to <c>true</c> [is present].</param>
        public static async Task<ComponentTemplate[]> SelectAllAvailableAsync(int pageTemplateID, bool isSecundary, bool isPresent)
        {
            //[MJ:03-01-2020] TEST this method !
            //  Get all templates
            var all_template = await SelectAllAsync();
            //  Get all available templates
            var all_availableTemplate = await AvailableTemplate.SelectAllAsync();
            //  Select all available templates based on the page template ID
            var sel_availableTemplate =
                from availableTemplate in all_availableTemplate
                where availableTemplate.PageTemplateID == pageTemplateID
                && availableTemplate.IsPossible == true
                && availableTemplate.IsPresent == isPresent
                && availableTemplate.IsSecundary == isSecundary
                select availableTemplate;
            //  Select all templates based on the available templates
            var sel_template =
                from template in all_template
                join availableTemplate in sel_availableTemplate on template.ID equals availableTemplate.ComponentTemplateID
                select template;

            return sel_template.ToArray();
        }

        /// <summary>
        /// Select all the newly assigned (available) component templates
        /// </summary>
        /// <param name="pageTemplateId">The page template id.</param>
        /// <param name="pageId">The page id.</param>
        /// <returns></returns>
        public static ComponentTemplate[] SelectAll(int pageTemplateID, int pageID)
        {
            //[MJ:03-01-2020] TEST this method !
            var connector = ConnectorFactory.CreateConnector<ComponentTemplate>();
            var filter = connector.CreateDataFilter();
            filter.AddOrder(x => x.Name);
            filter.AddParameter("@pageTemplateID", pageTemplateID);
            filter.AddParameter("@pageID", pageID);

            return connector.FetchAll(
                @"  SELECT *
                    FROM wim_ComponentTemplates
                    JOIN wim_AvailableTemplates ON AvailableTemplates_ComponentTemplate_Key = ComponentTemplate_Key
                    LEFT JOIN wim_ComponentVersions on ComponentTemplate_Key = ComponentVersion_ComponentTemplate_Key
                    WHERE ComponentVersion_Page_Key = @pageID
                      AND AvailableTemplates_PageTemplate_Key = @pageTemplateID
                  ", filter).ToArray();
        }

        /// <summary>
        /// Select all the newly assigned (available) component templates
        /// </summary>
        /// <param name="pageTemplateId">The page template id.</param>
        /// <param name="pageId">The page id.</param>
        /// <returns></returns>
        public static async Task<ComponentTemplate[]> SelectAllAsync(int pageTemplateID, int pageID)
        {
            //[MJ:03-01-2020] TEST this method !
            var connector = ConnectorFactory.CreateConnector<ComponentTemplate>();
            var filter = connector.CreateDataFilter();
            filter.AddOrder(x => x.Name);
            filter.AddParameter("@pageTemplateID", pageTemplateID);
            filter.AddParameter("@pageID", pageID);

            var result = await connector.FetchAllAsync(
                @"  SELECT *
                    FROM wim_ComponentTemplates
                    JOIN wim_AvailableTemplates ON AvailableTemplates_ComponentTemplate_Key = ComponentTemplate_Key
                    LEFT JOIN wim_ComponentVersions on ComponentTemplate_Key = ComponentVersion_ComponentTemplate_Key
                    WHERE ComponentVersion_Page_Key = @pageID
                      AND AvailableTemplates_PageTemplate_Key = @pageTemplateID
                  ", filter);

            return result.ToArray();
        }

        public static ComponentTemplate[] SelectAllShared()
        {
            var connector = ConnectorFactory.CreateConnector<ComponentTemplate>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.IsShared, true);
            return connector.FetchAll(filter).ToArray();
        }

        /// <summary>
        /// Selects all.
        /// </summary>
        public static async Task<ComponentTemplate[]> SelectAllSharedAsync()
        {
            var connector = ConnectorFactory.CreateConnector<ComponentTemplate>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.IsShared, true);
            var result = await connector.FetchAllAsync(filter);
            return result.ToArray();
        }

        /// <summary>
        /// Save a database entity. This method will indentify the need for an INSERT or an UPDATE.
        /// </summary>
        public void Save()
        {
            var connector = ConnectorFactory.CreateConnector<ComponentTemplate>();
            connector.Save(this);
        }


        /// <summary>
        /// Save a database entity. This method will indentify the need for an INSERT or an UPDATE.
        /// </summary>
        public async Task SaveAsync()
        {
            var connector = ConnectorFactory.CreateConnector<ComponentTemplate>();
            await connector.SaveAsync(this);
        }

        /// <summary>
        /// Delete an implementation record.
        /// </summary>
        /// <returns></returns>
        public void Delete()
        {
            var connector = ConnectorFactory.CreateConnector<ComponentTemplate>();
            var filter = connector.CreateDataFilter();
            filter.AddParameter("@componentTemplateID", this.ID);
            connector.ExecuteNonQuery(
                @"DELETE FROM [wim_ComponentSearch] WHERE [ComponentSearch_Ref_Key] = @componentTemplateID
                  DELETE FROM [wim_Components] WHERE [Component_ComponentTemplate_Key] = @componentTemplateID
                  DELETE FROM [wim_ComponentVersions] WHERE [ComponentVersion_ComponentTemplate_Key] = @componentTemplateID
                  DELETE FROM [wim_AvailableTemplates] WHERE [AvailableTemplates_ComponentTemplate_Key] = @componentTemplateID",
                filter);
			connector.Cache?.FlushRegion(connector.CacheRegion);

            connector.Delete(this);
        }


        /// <summary>
        /// Delete an implementation record.
        /// </summary>
        /// <returns></returns>
        public async Task DeleteAsync()
        {
            var connector = ConnectorFactory.CreateConnector<ComponentTemplate>();
            var filter = connector.CreateDataFilter();
            filter.AddParameter("@componentTemplateID", this.ID);
            await connector.ExecuteNonQueryAsync(
                @"DELETE FROM [wim_ComponentSearch] WHERE [ComponentSearch_Ref_Key] = @componentTemplateID
                  DELETE FROM [wim_Components] WHERE [Component_ComponentTemplate_Key] = @componentTemplateID
                  DELETE FROM [wim_ComponentVersions] WHERE [ComponentVersion_ComponentTemplate_Key] = @componentTemplateID
                  DELETE FROM [wim_AvailableTemplates] WHERE [AvailableTemplates_ComponentTemplate_Key] = @componentTemplateID",
                filter);
			connector.Cache?.FlushRegion(connector.CacheRegion);

            await connector.DeleteAsync(this);
        }
    }
}