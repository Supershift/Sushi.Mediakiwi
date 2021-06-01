using Sushi.MicroORM;
using Sushi.MicroORM.Mapping;
using System;
using System.Data;
using System.Threading.Tasks;
using Sushi.Mediakiwi.Data.MicroORM;

namespace Sushi.Mediakiwi.Data
{
    [DataMap(typeof(AvailableTemplateMap))]
    public class AvailableTemplate : IAvailableTemplate, IExportable, IComponent
    {
        public bool IsNewInstance { get { return ID == 0; } }

        public class AvailableTemplateMap : DataMap<AvailableTemplate>
        {
            public AvailableTemplateMap() : this(false) { }

            public AvailableTemplateMap(bool isSave)
            {
                if (isSave)
                    Table("wim_AvailableTemplates");
                else
                    Table("wim_AvailableTemplates LEFT JOIN wim_ComponentTemplates ON ComponentTemplate_Key = AvailableTemplates_ComponentTemplate_Key");
                Id(x => x.ID, "AvailableTemplates_Key").Identity().SqlType(SqlDbType.Int);
                Map(x => x.GUID, "AvailableTemplates_Guid").SqlType(SqlDbType.UniqueIdentifier);
                Map(x => x.PageTemplateID, "AvailableTemplates_PageTemplate_Key").SqlType(SqlDbType.Int);
                Map(x => x.ComponentTemplateID, "AvailableTemplates_ComponentTemplate_Key").SqlType(SqlDbType.Int);
                Map(x => x.Target, "AvailableTemplates_Target").SqlType(SqlDbType.VarChar).Length(25);
                Map(x => x.IsPossible, "AvailableTemplates_IsPossible").SqlType(SqlDbType.Bit);
                Map(x => x.IsSecundary, "AvailableTemplates_IsSecundary").SqlType(SqlDbType.Bit);
                Map(x => x.IsPresent, "AvailableTemplates_IsPresent").SqlType(SqlDbType.Bit);
                Map(x => x.SortOrder, "AvailableTemplates_SortOrder").SqlType(SqlDbType.Int);
                Map(x => x.FixedFieldName, "AvailableTemplates_Fixed_Id").SqlType(SqlDbType.NVarChar).Length(50);
                Map(x => x.ComponentTemplate, "ComponentTemplate_Name").ReadOnly();
                Map(x => x.SlotID, "AvailableTemplates_Slot");
            }
        }

        #region properties
        public int? SlotID { get; set; }

        /// <summary>
        /// Gets or sets the ID.
        /// </summary>
        /// <value>The ID.</value>
        public int ID { get; set; }

        private Guid m_GUID;

        /// <summary>
        /// Gets or sets the GUID.
        /// </summary>
        /// <value>The GUID.</value>
        public Guid GUID
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
        /// Gets or sets the page template ID.
        /// </summary>
        /// <value>The page template ID.</value>
        public int PageTemplateID { get; set; }

        /// <summary>
        /// Gets or sets the component template ID.
        /// </summary>
        /// <value>The component template ID.</value>
        public int ComponentTemplateID { get; set; }
        
        public string Target { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is possible.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is possible; otherwise, <c>false</c>.
        /// </value>
        public bool IsPossible { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is secundary.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is secundary; otherwise, <c>false</c>.
        /// </value>
        public bool IsSecundary { get; set; }

        /// <summary>
        /// The Name of the component template
        /// </summary>
        public string ComponentTemplate { get; set; }        

        /// <summary>
        /// Gets or sets a value indicating whether this instance is present.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is present; otherwise, <c>false</c>.
        /// </value>
        public bool IsPresent { get; set; }

        /// <summary>
        /// Gets or sets the sort order.
        /// </summary>
        /// <value>The sort order.</value>
        public int SortOrder { get; set; }

        /// <summary>
        /// Gets or sets the name of the fixed field.
        /// </summary>
        /// <value>The name of the fixed field.</value>
        public string FixedFieldName { get; set; }

        public DateTime? Updated
        {
            get
            {
                return null;
            }
        }

        ComponentTemplate IComponent.Template { get; }

        #endregion properties

        /// <summary>
        /// Select an AvailableTemplate based on its primary key
        /// </summary>
        /// <param name="availableTemplateID">The available template identifier.</param>
        /// <returns></returns>
        public static IAvailableTemplate SelectOne(int availableTemplateID)
        {
            var connector = ConnectorFactory.CreateConnector<AvailableTemplate>();
            return connector.FetchSingle(availableTemplateID);
        }

        /// <summary>
        /// Select an AvailableTemplate based on its primary key
        /// </summary>
        /// <param name="availableTemplateID">The available template identifier.</param>
        /// <returns></returns>
        public static async Task<IAvailableTemplate> SelectOneAsync(int availableTemplateID)
        {
            var connector = ConnectorFactory.CreateConnector<AvailableTemplate>();
            return await connector.FetchSingleAsync(availableTemplateID);
        }

        /// <summary>
        /// Selects the one.
        /// </summary>
        /// <param name="pageTemplateID">The page template identifier.</param>
        /// <param name="fixedTag">The fixed tag.</param>
        /// <returns></returns>
        public static IAvailableTemplate SelectOne(int pageTemplateID, string fixedTag)
        {
            var connector = ConnectorFactory.CreateConnector<AvailableTemplate>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.PageTemplateID, pageTemplateID);
            filter.Add(x => x.FixedFieldName, fixedTag);
            return connector.FetchSingle(filter);
        }

        /// <summary>
        /// Selects the one.
        /// </summary>
        /// <param name="pageTemplateID">The page template identifier.</param>
        /// <param name="fixedTag">The fixed tag.</param>
        /// <returns></returns>
        public static async Task<IAvailableTemplate> SelectOneAsync(int pageTemplateID, string fixedTag)
        {
            var connector = ConnectorFactory.CreateConnector<AvailableTemplate>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.PageTemplateID, pageTemplateID);
            filter.Add(x => x.FixedFieldName, fixedTag);
            return await connector.FetchSingleAsync(filter);
        }

        /// <summary>
        /// Select all available sites
        /// </summary>
        /// <returns>Array of site objects</returns>
        public static IAvailableTemplate[] SelectAll()
        {
            var connector = ConnectorFactory.CreateConnector<AvailableTemplate>();
            var filter = connector.CreateDataFilter();
            filter.AddOrder(x => x.SortOrder);
            return connector.FetchAll(filter).ToArray();
        }

        /// <summary>
        /// Select all available sites
        /// </summary>
        /// <returns>Array of site objects</returns>
        public static async Task<IAvailableTemplate[]> SelectAllAsync()
        {
            var connector = ConnectorFactory.CreateConnector<AvailableTemplate>();
            var filter = connector.CreateDataFilter();
            filter.AddOrder(x => x.SortOrder);
            var result = await connector.FetchAllAsync(filter);
            return result.ToArray();
        }

        /// <summary>
        /// Selects all.
        /// </summary>
        /// <param name="pageTemplateID">The page template ID.</param>
        /// <returns></returns>
        public static IAvailableTemplate[] SelectAll(int pageTemplateID, string target = null)
        {
            var connector = ConnectorFactory.CreateConnector<AvailableTemplate>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.PageTemplateID, pageTemplateID);
            if (!string.IsNullOrEmpty(target))
                filter.Add(x => x.Target, target);
            filter.AddOrder(x => x.SortOrder);
            return connector.FetchAll(filter).ToArray();
        }

        /// <summary>
        /// Selects all.
        /// </summary>
        /// <param name="pageTemplateID">The page template ID.</param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static async Task<IAvailableTemplate[]> SelectAllAsync(int pageTemplateID, string target = null)
        {
            var connector = ConnectorFactory.CreateConnector<AvailableTemplate>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.PageTemplateID, pageTemplateID);
            if (!string.IsNullOrEmpty(target))
                filter.Add(x => x.Target, target);
            filter.AddOrder(x => x.SortOrder);
            var result = await connector.FetchAllAsync(filter);
            return result.ToArray();
        }

        public static IAvailableTemplate[] SelectAllByComponentTemplate(int componentTemplateID)
        {
            var connector = ConnectorFactory.CreateConnector<AvailableTemplate>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.ComponentTemplateID, componentTemplateID);
            filter.AddOrder(x => x.SortOrder);
            var result = connector.FetchAll(filter);
            return result.ToArray();
        }

        public static IAvailableTemplate[] SelectAllBySlot(int slotID)
        {
            var connector = ConnectorFactory.CreateConnector<AvailableTemplate>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.SlotID, slotID);
            filter.AddOrder(x => x.SortOrder);
            var result = connector.FetchAll(filter);
            return result.ToArray();
        }
        public static async Task<IAvailableTemplate[]> SelectAllBySlotAsync(int slotID)
        {
            var connector = ConnectorFactory.CreateConnector<AvailableTemplate>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.SlotID, slotID);
            filter.AddOrder(x => x.SortOrder);
            var result = await connector.FetchAllAsync(filter);
            return result.ToArray();
        }

        public static IAvailableTemplate[] SelectAllBySlSelectAllByComponentTemplate(int templateID)
        {
            var connector = ConnectorFactory.CreateConnector<AvailableTemplate>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.ComponentTemplateID, templateID);
            filter.AddOrder(x => x.SortOrder);
            var result = connector.FetchAll(filter);
            return result.ToArray();
        }
        public static async Task<IAvailableTemplate[]> SelectAllByComponentTemplateAsync(int templateID)
        {
            var connector = ConnectorFactory.CreateConnector<AvailableTemplate>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.ComponentTemplateID, templateID);
            filter.AddOrder(x => x.SortOrder);
            var result = await connector.FetchAllAsync(filter);
            return result.ToArray();
        }

        /// <summary>
        /// Selects the all template that are currently not on the page.
        /// </summary>
        /// <param name="pageTemplateID"></param>
        /// <param name="pageID"></param>
        /// <param name="onlyReturnFixedInCode"></param>
        /// <returns></returns>
        internal static IAvailableTemplate[] SelectAll(int pageTemplateID, int pageID, bool onlyReturnFixedInCode = false)
        {
            //[MJ:03-01-2020] TEST this method !
            var connector = ConnectorFactory.CreateConnector<AvailableTemplate>();
            var filter = connector.CreateDataFilter();
            filter.AddParameter("@pageTemplateID", pageTemplateID);
            filter.AddParameter("@pageID", pageID);

            string sqlText = @" SELECT *
                                FROM wim_AvailableTemplates
                                JOIN wim_ComponentTemplates on ComponentTemplate_Key = AvailableTemplates_ComponentTemplate_Key
                                LEFT JOIN wim_ComponentVersions on ComponentVersion_ComponentTemplate_Key = AvailableTemplates_ComponentTemplate_Key
                                WHERE ComponentVersion_Page_Key = @pageID
                                  AND AvailableTemplates_PageTemplate_Key = @pageTemplateID";

            if (onlyReturnFixedInCode)
            {
                sqlText += "  AND (not AvailableTemplates_Fixed_Id is null or ComponentTemplate_IsFixed = 1)";
            }

            sqlText += " ORDER BY AvailableTemplates_SortOrder ASC";

            return connector.FetchAll(sqlText, filter).ToArray();
        }

        /// <summary>
        /// Selects the all template that are currently not on the page.
        /// </summary>
        /// <param name="pageTemplateID"></param>
        /// <param name="pageID"></param>
        /// <param name="onlyReturnFixedInCode"></param>
        /// <returns></returns>
        internal static async Task<IAvailableTemplate[]> SelectAllAsync(int pageTemplateID, int pageID, bool onlyReturnFixedInCode = false)
        {
            //[MJ:03-01-2020] TEST this method !
            var connector = ConnectorFactory.CreateConnector<AvailableTemplate>();
            var filter = connector.CreateDataFilter();
            filter.AddParameter("@pageTemplateID", pageTemplateID);
            filter.AddParameter("@pageID", pageID);

            string sqlText = @" SELECT *
                                FROM wim_AvailableTemplates
                                JOIN wim_ComponentTemplates on ComponentTemplate_Key = AvailableTemplates_ComponentTemplate_Key
                                LEFT JOIN wim_ComponentVersions on ComponentVersion_AvailableTemplate_Key = AvailableTemplates_Key
                                WHERE ComponentVersion_Page_Key = @pageID
                                  AND AvailableTemplates_PageTemplate_Key = @pageTemplateID";

            if (onlyReturnFixedInCode)
            {
                sqlText += "  AND NOT AvailableTemplates_Fixed_Id IS NULL";
            }

            sqlText += " ORDER BY AvailableTemplates_SortOrder ASC";

            var result = await connector.FetchAllAsync(sqlText, filter);
            return result.ToArray();
        }

        /// <summary>
        /// Saves the AvailableTemplate to database
        /// </summary>
        public void Save()
        {
            var connector = ConnectorFactory.CreateConnector<AvailableTemplate>(new AvailableTemplateMap(true));
            connector.Save(this);
        }

        /// <summary>
        /// Saves the AvailableTemplate to database
        /// </summary>
        public async Task SaveAsync()
        {
            var connector = ConnectorFactory.CreateConnector<AvailableTemplate>(new AvailableTemplateMap(true));
            await connector.SaveAsync(this);
        }

        /// <summary>
        /// Deletes the AvailableTemplate from the database
        /// </summary>
        public void Delete()
        {
            var connector = ConnectorFactory.CreateConnector<AvailableTemplate>(new AvailableTemplateMap(true));
            connector.Delete(this);
        }

        /// <summary>
        /// Deletes the AvailableTemplate from the database
        /// </summary>
        public async Task DeleteAsync()
        {
            var connector = ConnectorFactory.CreateConnector<AvailableTemplate>(new AvailableTemplateMap(true));
            await connector.DeleteAsync(this);
        }

        /// <summary>
        /// Deletes the specified page template ID.
        /// </summary>
        /// <param name="pageTemplateID">The page template ID.</param>
        /// <param name="portal">The portal.</param>
        public static void Delete(int pageTemplateID, string portal)
        {
            // TODO MJ 2019-01-03: Call to Portal settings nodig
            //if (!string.IsNullOrEmpty(portal))
            //{
            //    AvailableTemplate.SqlConnectionString = Common.GetPortal(portal).Connection;
            //    AvailableTemplate.ConnectionType = Common.GetPortal(portal).Type;
            //}

            var connector = ConnectorFactory.CreateConnector<AvailableTemplate>(new AvailableTemplateMap(true));
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.PageTemplateID, pageTemplateID);
            connector.Delete(filter);
        }

        /// <summary>
        /// Deletes the specified page template ID.
        /// </summary>
        /// <param name="pageTemplateID">The page template ID.</param>
        /// <param name="isSecundary">if set to <c>true</c> [is secundary].</param>
        /// <param name="target">The target.</param>
        public static void Delete(int pageTemplateID, bool isSecundary, string target)
        {
            var connector = ConnectorFactory.CreateConnector<AvailableTemplate>(new AvailableTemplateMap(true));
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.PageTemplateID, pageTemplateID);
            filter.Add(x => x.IsSecundary, isSecundary);
            if (string.IsNullOrEmpty(target))
                filter.Add(x => x.Target, null);
            else
                filter.Add(x => x.Target, target);
            connector.Delete(filter);
        }
    }
}