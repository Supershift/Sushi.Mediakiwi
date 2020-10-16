using Sushi.MicroORM;
using Sushi.MicroORM.Mapping;
using System;
using System.Linq;
using System.Threading.Tasks;
using Sushi.Mediakiwi.Data.MircoORM;

namespace Sushi.Mediakiwi.Data
{
    /// <summary>
    /// Represents a ComponentListVersion entity.
    /// </summary>
    [DataMap(typeof(ComponentListVersionMap))]
    public class ComponentListVersion : IExportable
    {
        public class ComponentListVersionMap : DataMap<ComponentListVersion>
        {
            public ComponentListVersionMap()
            {
                Table("wim_ComponentListVersions");
                Id(x => x.ID, "ComponentListVersion_Key").Identity();
                Map(x => x.ApplicationUserID, "ComponentListVersion_User_Key");
                Map(x => x.Version, "ComponentListVersion_Version");
                Map(x => x.TypeID, "ComponentListVersion_Type");
                Map(x => x.SiteID, "ComponentListVersion_Site_Key");
                Map(x => x.ComponentListID, "ComponentListVersion_ComponentList_Key");
                Map(x => x.ComponentListItemID, "ComponentListVersion_Listitem_Key");
                Map(x => x.Serialized_XML, "ComponentListVersion_XML").SqlType(System.Data.SqlDbType.Xml);
                Map(x => x.DescriptiveTag, "ComponentListVersion_DescriptionTag").Length(100);
                Map(x => x.Created, "ComponentListVersion_Created");
                Map(x => x.IsActive, "ComponentListVersion_IsActive");
                
            }
        }

        #region Properties
        /// <summary>
        /// Gets or sets the ID.
        /// </summary>
        /// <value>The ID.</value>
        public int ID { get; set; }

        /// <summary>
        /// Gets or sets the application user ID.
        /// </summary>
        /// <value>The application user ID.</value>
        public int ApplicationUserID { get; set; }

        /// <summary>
        /// Gets or sets the application user ID.
        /// </summary>
        /// <value>The application user ID.</value>
        public int Version { get; set; }

        /// <summary>
        /// Gets or sets the type ID ( 0 = created. 1 = updated, 2 = deleted).
        /// </summary>
        /// <value>The type ID.</value>
        public int TypeID { get; set; }

        /// <summary>
        /// Gets or sets the site ID.
        /// </summary>
        /// <value>The site ID.</value>
        public int SiteID { get; set; }

        /// <summary>
        /// Gets or sets the component list ID.
        /// </summary>
        /// <value>The component list ID.</value>
        public int ComponentListID { get; set; }

        /// <summary>
        /// Gets or sets the component list item ID.
        /// </summary>
        /// <value>The component list item ID.</value>
        public int ComponentListItemID { get; set; }

        /// <summary>
        /// Gets or sets the serialized_ XML.
        /// </summary>
        /// <value>The serialized_ XML.</value>
        public string Serialized_XML { get; set; }

        /// <summary>
        /// Gets or sets the descriptive tag.
        /// </summary>
        /// <value>The descriptive tag.</value>
        public string DescriptiveTag { get; set; }

        private DateTime m_Created;

        /// <summary>
        /// Gets the created.
        /// </summary>
        /// <value>The created.</value>
        public DateTime Created
        {
            get
            {
                if (this.m_Created == DateTime.MinValue)
                    m_Created = Common.DatabaseDateTime;
                return m_Created;
            }
            set { m_Created = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is active.
        /// </summary>
        /// <value><c>true</c> if this instance is active; otherwise, <c>false</c>.</value>
        public bool? IsActive { get; set; }

        private Content m_Content;

        /// <summary>
        /// Get the content of this component
        /// </summary>
        /// <value>The content.</value>
        public Content Content
        {
            get
            {
                if (m_Content != null)
                    return m_Content;

                if (Serialized_XML == null || Serialized_XML.Trim().Length == 0)
                    return null;

                m_Content = Content.GetDeserialized(Serialized_XML);
                return m_Content;
            }
        }
        #endregion Properties

        /// <summary>
        /// Select a single ComponentlistVersion object
        /// </summary>
        /// <param name="ID">The componentlist version ID.</param>
        /// <returns>ComponentlistVersion object</returns>
        public static ComponentListVersion SelectOne(int ID)
        {
            var connector = ConnectorFactory.CreateConnector<ComponentListVersion>();
            return connector.FetchSingle(ID);
        }

        /// <summary>
        /// Select a single ComponentlistVersion object
        /// </summary>
        /// <param name="ID">The componentlist version ID.</param>
        /// <returns>ComponentlistVersion object</returns>
        public static async Task<ComponentListVersion> SelectOneAsync(int ID)
        {
            var connector = ConnectorFactory.CreateConnector<ComponentListVersion>();
            return await connector.FetchSingleAsync(ID);
        }

        /// <summary>
        /// Select the last introduced ComponentlistVersion object based on the componentlist Key
        /// and the componentListVersionItemKey (if the componentlist is set as for single page configuration, this key is always the siteId)
        /// </summary>
        /// <param name="componentListID">The component list ID.</param>
        /// <param name="componentListVersionItemID">The component list version item ID.</param>
        /// <returns>ComponentlistVersion object</returns>
        public static ComponentListVersion SelectOne(int componentListID, int componentListVersionItemID)
        {
            if (componentListVersionItemID < 1)
                return new ComponentListVersion();

            var connector = ConnectorFactory.CreateConnector<ComponentListVersion>();
            var filter = connector.CreateDataFilter();
            
            filter.MaxResults = 1;
            filter.AddOrder(x => x.ID, SortOrder.DESC);
            filter.Add(x => x.ComponentListID, componentListID);
            filter.Add(x => x.ComponentListItemID, componentListVersionItemID);

            return connector.FetchSingle(filter);
        }

        /// <summary>
        /// Select the last introduced ComponentlistVersion object based on the componentlist Key
        /// and the componentListVersionItemKey (if the componentlist is set as for single page configuration, this key is always the siteId)
        /// Async
        /// </summary>
        /// <param name="componentListID">The component list ID.</param>
        /// <param name="componentListVersionItemID">The component list version item ID.</param>
        /// <returns>ComponentlistVersion object</returns>
        public static async Task<ComponentListVersion> SelectOneAsync(int componentListID, int componentListVersionItemID)
        {
            if (componentListVersionItemID < 1)
                return new ComponentListVersion();

            var connector = ConnectorFactory.CreateConnector<ComponentListVersion>();
            var filter = connector.CreateDataFilter();
            
            filter.MaxResults = 1;
            filter.AddOrder(x => x.ID, SortOrder.DESC);
            filter.Add(x => x.ComponentListID, componentListID);
            filter.Add(x => x.ComponentListItemID, componentListVersionItemID);

            return await connector.FetchSingleAsync(filter);
        }

        /// <summary>
        /// Selects the one.
        /// </summary>
        /// <param name="siteID">The site ID.</param>
        /// <param name="componentListID">The component list ID.</param>
        /// <param name="componentListVersionItemID">The component list version item ID.</param>
        /// <returns></returns>
        public static ComponentListVersion SelectOne(int siteID, int componentListID, int componentListVersionItemID)
        {
            var connector = ConnectorFactory.CreateConnector<ComponentListVersion>();
            var filter = connector.CreateDataFilter();
            
            filter.MaxResults = 1;
            filter.AddOrder(x => x.ID, SortOrder.DESC);
            filter.Add(x => x.SiteID, siteID);
            filter.Add(x => x.ComponentListID, componentListID);
            filter.Add(x => x.ComponentListItemID, componentListVersionItemID);

            return connector.FetchSingle(filter);
        }

        /// <summary>
        /// Selects the one.
        /// </summary>
        /// <param name="siteID">The site ID.</param>
        /// <param name="componentListID">The component list ID.</param>
        /// <param name="componentListVersionItemID">The component list version item ID.</param>
        /// <returns></returns>
        public static async Task<ComponentListVersion> SelectOneAsync(int siteID, int componentListID, int componentListVersionItemID)
        {
            var connector = ConnectorFactory.CreateConnector<ComponentListVersion>();
            var filter = connector.CreateDataFilter();
            
            filter.MaxResults = 1;
            filter.AddOrder(x => x.ID, SortOrder.DESC);
            filter.Add(x => x.SiteID, siteID);
            filter.Add(x => x.ComponentListID, componentListID);
            filter.Add(x => x.ComponentListItemID, componentListVersionItemID);

            return await connector.FetchSingleAsync(filter);
        }

        /// <summary>
        /// Select all active ComponentlistVersion objects having a Descriptive tag
        /// </summary>
        /// <param name="componentListID">The component list ID.</param>
        /// <param name="siteID">The site ID.</param>
        /// <returns>array of ComponentListVersion</returns>
        public static ComponentListVersion[] SelectAll(int componentListID, int siteID)
        {
            var connector = ConnectorFactory.CreateConnector<ComponentListVersion>();
            var filter = connector.CreateDataFilter();
            filter.AddOrder(x => x.ID, SortOrder.DESC);
            filter.AddParameter("@siteId", siteID);

            filter.Add(x => x.IsActive, true);
            filter.Add(x => x.DescriptiveTag, null, ComparisonOperator.NotEqualTo);
            filter.Add(x => x.ComponentListID, componentListID);
            filter.AddSql("([ComponentListVersion_Site_Key] IS NULL OR [ComponentListVersion_Site_Key] = @siteId)");
            filter.Add(x => x.ComponentListItemID, 0, ComparisonOperator.NotEqualTo);
            filter.Add(x => x.Serialized_XML, null, ComparisonOperator.NotEqualTo);

            var result = connector.FetchAll(filter);
            return result.ToArray();
        }

        /// <summary>
        /// Select all active ComponentlistVersion objects having a Descriptive tag
        /// </summary>
        /// <param name="componentListID">The component list ID.</param>
        /// <param name="siteID">The site ID.</param>
        /// <returns>array of ComponentListVersion</returns>
        public static async Task<ComponentListVersion[]> SelectAllAsync(int componentListID, int siteID)
        {
            var connector = ConnectorFactory.CreateConnector<ComponentListVersion>();
            var filter = connector.CreateDataFilter();
            filter.AddOrder(x => x.ID, SortOrder.DESC);
            filter.AddParameter("@siteId", siteID);

            filter.Add(x => x.IsActive, true);
            filter.Add(x => x.DescriptiveTag, null, ComparisonOperator.NotEqualTo);
            filter.Add(x => x.ComponentListID, componentListID);
            filter.AddSql("([ComponentListVersion_Site_Key] IS NULL OR [ComponentListVersion_Site_Key] = @siteId)");
            filter.Add(x => x.ComponentListItemID, 0, ComparisonOperator.NotEqualTo);
            filter.Add(x => x.Serialized_XML, null, ComparisonOperator.NotEqualTo);

            var result = await connector.FetchAllAsync(filter);
            return result.ToArray();
        }

        /// <summary>
        /// Select the last componentlist version based on the ComponentList Identifier (GUID)
        /// </summary>
        /// <param name="componentListVersionGUID">The component list version GUID.</param>
        /// <param name="siteID">The site ID.</param>
        /// <returns></returns>
        public static ComponentListVersion SelectOne(Guid componentListVersionGUID, int siteID)      
        {
            var connector = ConnectorFactory.CreateConnector<ComponentListVersion>();
            var filter = connector.CreateDataFilter();
            filter.AddSql("EXISTS (SELECT * FROM wim_ComponentLists WHERE ComponentListVersion_ComponentList_Key = ComponentList_Key AND ComponentList_GUID = @componentListGUID)");
            filter.AddParameter("@componentListGUID", componentListVersionGUID);
            filter.Add(x => x.SiteID, siteID);

            return connector.FetchSingle(filter);
        }

        /// <summary>
        /// Async Select the last componentlist version based on the ComponentList Identifier (GUID)
        /// </summary>
        /// <param name="componentListVersionGUID">The component list version GUID.</param>
        /// <param name="siteID">The site ID.</param>
        /// <returns></returns>
        public static async Task<ComponentListVersion> SelectOneAsync(Guid componentListVersionGUID, int siteID)
        {
            var connector = ConnectorFactory.CreateConnector<ComponentListVersion>();
            var filter = connector.CreateDataFilter();
            filter.AddSql("EXISTS (SELECT * FROM wim_ComponentLists WHERE ComponentListVersion_ComponentList_Key = ComponentList_Key AND ComponentList_GUID = @componentListGUID)");
            filter.AddParameter("@componentListGUID", componentListVersionGUID);
            filter.Add(x => x.SiteID, siteID);

            return await connector.FetchSingleAsync(filter);
        }

        /// <summary>
        /// Select the last componentlist version based on the componentTemplate key
        /// and the componentListVersionItemKey (if the componentlist is set as for single page configuration, this key is always the siteId)
        /// </summary>
        /// <param name="componentTemplateID">The component template ID.</param>
        /// <param name="componentListVersionItemID">The component list version item ID.</param>
        /// <returns>ComponentlistVersion object</returns>
        [Obsolete("[MR:25-02-2020] TemplateID is not SET anywhere, is this an obsolete method?")]
        public static ComponentListVersion SelectOneByTemplate(int componentTemplateID, int componentListVersionItemID)
        {
            var connector = ConnectorFactory.CreateConnector<ComponentListVersion>();
            var filter = connector.CreateDataFilter();
            filter.AddSql("EXISTS (SELECT * FROM wim_ComponentLists WHERE ComponentListVersion_ComponentList_Key = ComponentList_Key AND ComponentList_ComponentTemplate_Key = @componentTemplateID)");
            filter.AddParameter("@componentTemplateID", componentTemplateID);            
            filter.Add(x => x.ComponentListItemID, componentListVersionItemID);

            return connector.FetchSingle(filter);
        }

        /// <summary>
        /// Async Select the last componentlist version based on the componentTemplate key
        /// and the componentListVersionItemKey (if the componentlist is set as for single page configuration, this key is always the siteId)
        /// </summary>
        /// <param name="componentTemplateID">The component template ID.</param>
        /// <param name="componentListVersionItemID">The component list version item ID.</param>
        /// <returns>ComponentlistVersion object</returns>
        [Obsolete("[MR:25-02-2020] TemplateID is not SET anywhere, is this an obsolete method?")]
        public static async Task<ComponentListVersion> SelectOneByTemplateAsync(int componentTemplateID, int componentListVersionItemID)
        {
            var connector = ConnectorFactory.CreateConnector<ComponentListVersion>();
            var filter = connector.CreateDataFilter();
            filter.AddSql("EXISTS (SELECT * FROM wim_ComponentLists WHERE ComponentListVersion_ComponentList_Key = ComponentList_Key AND ComponentList_ComponentTemplate_Key = @componentTemplateID)");
            filter.AddParameter("@componentTemplateID", componentTemplateID);
            filter.Add(x => x.ComponentListItemID, componentListVersionItemID);

            return await connector.FetchSingleAsync(filter);
        }

        /// <summary>
        /// Save a database entity. This method will indentify the need for an INSERT or an UPDATE.
        /// </summary>
        /// <returns></returns>
        public bool Save()
        {
            var connector = ConnectorFactory.CreateConnector<ComponentListVersion>();
            var filter = connector.CreateDataFilter();
            filter.AddParameter("@siteId", SiteID);
            filter.AddParameter("@componentListId", ComponentListID);
            filter.AddParameter("@componentListItemId", ComponentListItemID);

            bool isSaved = false;

            if (IsActive.HasValue == false)
                IsActive = true;

            Version = connector.ExecuteScalar<int>("SELECT ISNULL(MAX([ComponentListVersion_Version]), 0) + 1 FROM [wim_ComponentListVersions] WHERE [ComponentListVersion_Site_Key] = @siteId AND [ComponentListVersion_ComponentList_Key] = @componentListId AND [ComponentListVersion_Listitem_Key] = @componentListItemId", filter);

            try
            {
                connector.Save(this);
                isSaved = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            filter.AddParameter("@version", Version);

            connector.ExecuteNonQuery("UPDATE [wim_ComponentListVersions] SET [ComponentListVersion_IsActive] = 0 WHERE [ComponentListVersion_Site_Key] = @siteId AND [ComponentListVersion_ComponentList_Key] = @componentListId AND [ComponentListVersion_Listitem_Key] = @componentListItemId AND [ComponentListVersion_Version] < @version", filter);
			connector.Cache.FlushRegion(connector.CacheRegion);
            return isSaved;
        }

        /// <summary>
        /// Save a database entity Async. This method will indentify the need for an INSERT or an UPDATE.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> SaveAsync()
        {
            var connector = ConnectorFactory.CreateConnector<ComponentListVersion>();
            var filter = connector.CreateDataFilter();
            filter.AddParameter("@siteId", SiteID);
            filter.AddParameter("@componentListId", ComponentListID);
            filter.AddParameter("@componentListItemId", ComponentListItemID);

            bool isSaved = false;

            if (IsActive.HasValue == false)
                IsActive = true;

            Version = await connector.ExecuteScalarAsync<int>("SELECT ISNULL(MAX([ComponentListVersion_Version]), 0) + 1 FROM [wim_ComponentListVersions] WHERE [ComponentListVersion_Site_Key] = @siteId AND [ComponentListVersion_ComponentList_Key] = @componentListId AND [ComponentListVersion_Listitem_Key] = @componentListItemId", filter);

            try
            {
                await connector.SaveAsync(this);
                isSaved = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            filter.AddParameter("@version", Version);

            await connector.ExecuteNonQueryAsync("UPDATE [wim_ComponentListVersions] SET [ComponentListVersion_IsActive] = 0 WHERE [ComponentListVersion_Site_Key] = @siteId AND [ComponentListVersion_ComponentList_Key] = @componentListId AND [ComponentListVersion_Listitem_Key] = @componentListItemId AND [ComponentListVersion_Version] < @version", filter);
			connector.Cache.FlushRegion(connector.CacheRegion);
            return isSaved;
        }

        /// <summary>
        /// Delete an implementation record.
        /// </summary>
        /// <returns></returns>
        public bool Delete()
        {
            IsActive = false;
            return Save();
        }


        /// <summary>
        /// Delete an implementation record.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> DeleteAsync()
        {
            IsActive = false;
            return await SaveAsync();
        }

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

        public DateTime? Updated
        {
            get { return DateTime.Now; }
        }
    }
}