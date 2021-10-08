using Sushi.Mediakiwi.Data.MicroORM;
using Sushi.MicroORM.Mapping;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Data
{
    [DataMap(typeof(ComponentMap))]
    public class Component : IExportable
    {
        public class ComponentMap : DataMap<Component>
        {
            public ComponentMap() : this(false) { }
            
            public ComponentMap(bool isSave)
            {
                if(isSave)
                    Table("wim_Components");
                else
                    Table("wim_Components left join wim_ComponentTemplates on Component_ComponentTemplate_Key = ComponentTemplate_Key");

                Id(x => x.ID, "Component_Key").Identity();
                Map(x => x.GUID, "Component_GUID");
                Map(x => x.PageID, "Component_Page_Key");
                Map(x => x.ComponentTemplateID, "Component_ComponentTemplate_Key");
                Map(x => x.Target, "Component_Target").SqlType(SqlDbType.VarChar);
                Map(x => x.FixedId, "Component_Fixed_Id").SqlType(SqlDbType.NVarChar);
                Map(x => x.Created, "Component_Created").SqlType(SqlDbType.DateTime);
                Map(x => x.Updated, "Component_Updated").SqlType(SqlDbType.DateTime);
                Map(x => x.IsFixedOnTemplate, "Component_IsFixedOnTemplate");
                Map(x => x.IsAlive, "Component_IsAlive");
                Map(x => x.Serialized_XML, "Component_XML").SqlType(SqlDbType.Xml);
                Map(x => x.SortDate, "Component_SortDate").SqlType(SqlDbType.DateTime);
                Map(x => x.SortOrder, "Component_SortOrder");

                Map(x => x.IsSecundaryContainerItem, "ComponentTemplate_IsSecundaryContainerItem").ReadOnly();
                Map(x => x.IsShared, "ComponentTemplate_IsShared").ReadOnly(); 
                Map(x => x.IsSearchable, "ComponentTemplate_IsSearchable").ReadOnly();
                Map(x => x.AjaxType, "ComponentTemplate_AjaxType").ReadOnly();
                Map(x => x.CacheLevel, "ComponentTemplate_CacheLevel").ReadOnly();
                Map(x => x.Name, "ComponentTemplate_Name").SqlType(SqlDbType.NVarChar).ReadOnly();
                Map(x => x.CacheParams, "ComponentTemplate_CacheParams").SqlType(SqlDbType.VarChar).ReadOnly();
                Map(x => x.Location, "ComponentTemplate_Location").SqlType(SqlDbType.VarChar).ReadOnly();
                Map(x => x.Source, "ComponentTemplate_Source").SqlType(SqlDbType.NText).ReadOnly();
            }
        }

        public int ID { get; set; }
        public Guid GUID { get; set; }
        public int? PageID { get; set; }
        public int? ComponentTemplateID { get; set; }
        public string Target { get; set; }
        public string FixedId { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
        public bool? IsFixedOnTemplate { get; set; }
        public bool? IsAlive { get; set; }
        public string Serialized_XML { get; set; }
        public DateTime? SortDate { get; set; }
        public int? SortOrder { get; set; }
        
        public bool IsSecundaryContainerItem { get; set; }
        public bool? IsShared { get; set; }
        public bool IsSearchable { get; set; }
        public int? AjaxType { get; set; }
        public int? CacheLevel { get; set; }
        public string Name { get; set; }
        public string CacheParams { get; set; }
        public string Location { get; set; }
        public string Source { get; set; }

        private Content m_Content;

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
                {
                    m_Template = ComponentTemplate.SelectOne(ComponentTemplateID.GetValueOrDefault());
                }
                return m_Template;
            }
        }

        /// <summary>
        /// Get the content of this component
        /// </summary>
        /// <value>The content.</value>
        public Content Content
        {
            get
            {
                if (m_Content != null)
                {
                    return m_Content;
                }

                if (Serialized_XML == null || Serialized_XML.Trim().Length == 0)
                {
                    return null;
                }

                m_Content = Content.GetDeserialized(Serialized_XML);
                return m_Content;
            }
        }

        /// <summary>
        /// Select all available components
        /// </summary>
        public static Component[] SelectAll()
        {
            var connector = ConnectorFactory.CreateConnector<Component>();
            var filter = connector.CreateDataFilter();
            filter.AddOrder(x => x.SortOrder);

            return connector.FetchAll(filter).ToArray();
        }

        /// <summary>
        /// Select all available components Async
        /// </summary>
        public static async Task<Component[]> SelectAllAsync()
        {
            var connector = ConnectorFactory.CreateConnector<Component>();
            var filter = connector.CreateDataFilter();
            filter.AddOrder(x => x.SortOrder);

            var result = await connector.FetchAllAsync(filter).ConfigureAwait(false);
            return result.ToArray();
        }

        /// <summary>
        /// Selects the one.
        /// </summary>
        /// <param name="ID">The ID.</param>
        /// <returns></returns>
        public static Component SelectOne(int ID)
        {
            var connector = ConnectorFactory.CreateConnector<Component>();
            return connector.FetchSingle(ID);
        }

        /// <summary>
        /// Selects the one Async.
        /// </summary>
        /// <param name="ID">The ID.</param>
        /// <returns></returns>
        public static async Task<Component> SelectOneAsync(int ID)
        {
            var connector = ConnectorFactory.CreateConnector<Component>();
            return await connector.FetchSingleAsync(ID).ConfigureAwait(false);
        }

        /// <summary>
        /// Selects the one by GUID.
        /// </summary>
        /// <param name="componentGUID">The component GUID.</param>
        /// <returns></returns>
        public static Component SelectOne(Guid componentGUID)
        {
            var connector = ConnectorFactory.CreateConnector<Component>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.GUID, componentGUID);

            return connector.FetchSingle(filter);
        }

        /// <summary>
        /// Selects the one by GUID.
        /// </summary>
        /// <param name="componentGUID">The component GUID.</param>
        /// <returns></returns>
        public static async Task<Component> SelectOneAsync(Guid componentGUID)
        {
            var connector = ConnectorFactory.CreateConnector<Component>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.GUID, componentGUID);

            return await connector.FetchSingleAsync(filter).ConfigureAwait(false);
        }

        /// <summary>
        /// Selects the one by Page ID and Component ID.
        /// </summary>
        /// <param name="pageID">The page ID.</param>
        /// <param name="fixedComponentId">The fixed component ID.</param>
        /// <returns></returns>
        public static Component SelectOne(int pageID, string fixedComponentId)
        {
            var connector = ConnectorFactory.CreateConnector<Component>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.FixedId, fixedComponentId);
            filter.Add(x => x.IsFixedOnTemplate, true);
            filter.Add(x => x.PageID, pageID);

            return connector.FetchSingle(filter);
        }

        /// <summary>
        /// Selects the one by Page ID and Component ID.
        /// </summary>
        /// <param name="pageID">The page ID.</param>
        /// <param name="componentID">The component ID.</param>
        /// <returns></returns>
        public static async Task<Component> SelectOneAsync(int pageID, string componentID)
        {
            var connector = ConnectorFactory.CreateConnector<Component>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.FixedId, componentID);
            filter.Add(x => x.IsFixedOnTemplate, true);
            filter.Add(x => x.PageID, pageID);

            return await connector.FetchSingleAsync(filter).ConfigureAwait(false);
        }

        /// <summary>
        /// Selects the one by Page ID and Type.
        /// </summary>
        /// <param name="pageID">The page ID.</param>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static Component SelectOne(int pageID, Type type)
        {
            //[MR:07-01-2020] CHECK if this query works
            var connector = ConnectorFactory.CreateConnector<Component>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.PageID, pageID);
            filter.Add("[ComponentTemplate_Type]", SqlDbType.NVarChar, type.BaseType.ToString());

            return connector.FetchSingle(filter);
        }

        /// <summary>
        /// Selects the one by Page ID and Type.
        /// </summary>
        /// <param name="pageID">The page ID.</param>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static async Task<Component> SelectOneAsync(int pageID, Type type)
        {
            //[MR:07-01-2020] CHECK if this query works
            var connector = ConnectorFactory.CreateConnector<Component>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.PageID, pageID);
            filter.Add("[ComponentTemplate_Type]", SqlDbType.NVarChar, type.BaseType.ToString());

            return await connector.FetchSingleAsync(filter).ConfigureAwait(false);
        }

        /// <summary>
        /// Select all available components on a page with a particular component template as basis
        /// </summary>
        /// <param name="pageID">The page ID.</param>
        /// <param name="componentTemplateID">The component template ID.</param>
        /// <returns></returns>
        public static Component[] SelectAll(int pageID, int componentTemplateID)
        {
            var connector = ConnectorFactory.CreateConnector<Component>();
            var filter = connector.CreateDataFilter();
            filter.AddOrder(x => x.SortOrder);
            filter.Add(x => x.ComponentTemplateID, componentTemplateID);
            filter.Add(x => x.PageID, pageID);

            return connector.FetchAll(filter).ToArray();
        }

        /// <summary>
        /// Select all available components on a page with a particular component template as basis Async
        /// </summary>
        /// <param name="pageID">The page ID.</param>
        /// <param name="componentTemplateID">The component template ID.</param>
        /// <returns></returns>
        public static async Task<Component[]> SelectAllAsync(int pageID, int componentTemplateID)
        {
            var connector = ConnectorFactory.CreateConnector<Component>();
            var filter = connector.CreateDataFilter();
            filter.AddOrder(x => x.SortOrder);
            filter.Add(x => x.ComponentTemplateID, componentTemplateID);
            filter.Add(x => x.PageID, pageID);

            var result = await connector.FetchAllAsync(filter).ConfigureAwait(false);
            return result.ToArray();
        }

        /// <summary>
        /// Selects all Based on Page ID and IsSecundary.
        /// </summary>
        /// <param name="pageID">The page ID.</param>
        /// <param name="isSecundary">if set to <c>true</c> [is secundary].</param>
        /// <returns></returns>
        public static Component[] SelectAll(int pageID, bool isSecundary)
        {
            var connector = ConnectorFactory.CreateConnector<Component>();
            var filter = connector.CreateDataFilter();
            filter.AddOrder(x => x.SortOrder);

            if (isSecundary)
            {
                filter.Add(x => x.IsSecundaryContainerItem, isSecundary);
            }
            else
            {
                filter.AddSql("ISNULL([ComponentTemplate_IsSecundaryContainerItem], 0) = 0");
            }

            filter.Add(x => x.PageID, pageID);

            return connector.FetchAll(filter).ToArray();
        }

        /// <summary>
        /// Selects all Based on Page ID and IsSecundary Async
        /// </summary>
        /// <param name="pageID">The page ID.</param>
        /// <param name="isSecundary">if set to <c>true</c> [is secundary].</param>
        /// <returns></returns>
        public static async Task<Component[]> SelectAllAsync(int pageID, bool isSecundary)
        {
            var connector = ConnectorFactory.CreateConnector<Component>();
            var filter = connector.CreateDataFilter();
            filter.AddOrder(x => x.SortOrder);

            if (isSecundary)
            {
                filter.Add(x => x.IsSecundaryContainerItem, isSecundary);
            }
            else
            {
                filter.AddSql("ISNULL([ComponentTemplate_IsSecundaryContainerItem], 0) = 0");
            }

            filter.Add(x => x.PageID, pageID);

            var result = await connector.FetchAllAsync(filter).ConfigureAwait(false);
            return result.ToArray();
        }

        /// <summary>
        /// Saves this instance
        /// </summary>
        public void Save()
        {
            var connector = ConnectorFactory.CreateConnector(new ComponentMap(true));
            connector.Save(this);
        }

        /// <summary>
        /// Saves this instance Async
        /// </summary>
        public async Task SaveAsync()
        {
            var connector = ConnectorFactory.CreateConnector(new ComponentMap(true));
            await connector.SaveAsync(this).ConfigureAwait(false);
        }

        /// <summary>
        /// Deletes this instance
        /// </summary>
        public void Delete()
        {
            var connector = ConnectorFactory.CreateConnector(new ComponentMap(true));
            connector.Delete(this);
        }

        /// <summary>
        /// Deletes this instance Async
        /// </summary>
        public async Task DeleteAsync()
        {
            var connector = ConnectorFactory.CreateConnector(new ComponentMap(true));
            await connector.DeleteAsync(this).ConfigureAwait(false);
        }


        public static IComponent[] VerifyVisualisation(PageTemplate template, IAvailableTemplate[] components, string section, ref bool isSecundaryContentContainer, bool isCmsRequest = false)  
        {
            List<IComponent> selection = new List<IComponent>();
            foreach (var t in components)
            {
                var a = (AvailableTemplate)t;
                selection.Add(a);
            }
            return VerifyVisualisation(template, selection.ToArray(), section, ref isSecundaryContentContainer, isCmsRequest);
        }

        public static IComponent[] VerifyVisualisation(PageTemplate template, IComponent[] components, string section, ref bool isSecundaryContentContainer, bool isCmsRequest = false)  
        {
            bool isLegacySection = section == null;
            string legacyContentTab, legacyServiceTab;
            if (template != null)
            {
                legacyContentTab = template.Data["TAB.LCT"].Value ?? CommonConfiguration.DEFAULT_CONTENT_TAB;
                legacyServiceTab = template.Data["TAB.LST"].Value ?? CommonConfiguration.DEFAULT_SERVICE_TAB;

                if (!string.IsNullOrEmpty(legacyServiceTab) && section == legacyServiceTab)
                {
                    isSecundaryContentContainer = true;
                }
            }

            List<IComponent> selection = new List<IComponent>();
            foreach (var component in components)
            {
                //  Section 1 = service column
                //  This was !container.IsComponent
                if (template != null)
                {
                    if (component.Template.IsSecundaryContainerItem != isSecundaryContentContainer)
                    {
                        continue;
                    }

                    if (component.FixedFieldName != null)
                    {
                        if (!isCmsRequest)
                        {
                            continue;
                        }
                        else
                        {
                            if (isSecundaryContentContainer)
                            {
                                continue;
                            }
                        }
                    }

                    if (component.Target == null || component.Target == section)
                    {
                        // ALL IS OK!
                    }
                    else
                    {
                        continue;
                    }

                    selection.Add(component);
                }
            }
            return selection.ToArray();
        }

        /// <summary>
        /// Select all available components on a page
        /// </summary>
        /// <param name="pageID">The page ID.</param>
        /// <returns></returns>
        public static Component[] SelectAll(int pageID)                                      
        {
            return SelectAllInherited(pageID, false);
        }

        public static Component[] SelectAllInherited(int pageID, bool ignoreInheritance)   
        {
            var connector = ConnectorFactory.CreateConnector<Component>();
        
            Page page = Page.SelectOne(pageID, false);

            //  If the page is set to hold inherited content, please take that content
            if (!ignoreInheritance && page.InheritContent && page.MasterID.HasValue)
            {
                pageID = page.MasterID.Value;
            }

            var filter = connector.CreateDataFilter();
            filter.Add(x => x.PageID, pageID);
            
            var result = connector.FetchAll(filter);
            foreach (var component in result)
            {
                if (!ignoreInheritance && page.InheritContent && page.MasterID.HasValue)
                {
                    component.PageID = page.ID;
                }
            }

            return result.ToArray();
        }

        public static Component[] SelectAllShared(int pageID)   
        {
            var connector = ConnectorFactory.CreateConnector<Component>();

            Page page = Page.SelectOne(pageID, false);

            //  If the page is set to hold inherited content, please take that content
            if (page.InheritContent && page.MasterID.HasValue)
            {
                pageID = page.MasterID.Value;
            }
            
            var filter = connector.CreateDataFilter();
            filter.AddParameter("@pageId", pageID);
            filter.AddParameter("@isShared", true);

            var result = connector.FetchAll(@"
                SELECT [wim_Components].* FROM [wim_Components] 
                JOIN [wim_ComponentTargets] ON [Component_GUID] = [ComponentTarget_Component_Source]
				LEFT JOIN [dbo].[wim_ComponentTemplates] ON [ComponentTemplate_Key] = Component_ComponentTemplate_Key
                WHERE [ComponentTarget_Page_Key] = @pageId
                AND [ComponentTemplate_IsShared] = @isShared", filter);

            foreach (var component in result)
            {
                //  When the content is set to inherit please apply the child page ID
                if (page.InheritContent && page.MasterID.HasValue)
                {
                    component.PageID = page.ID;
                }
            }

            return result.ToArray();
        }

        public static async Task<Component[]> SelectAllAsync(int pageID)
        {
            return await SelectAllInheritedAsync(pageID, false, true).ConfigureAwait(false);
        }

        /// <summary>
        /// Select all available components on a page
        /// </summary>
        /// <param name="pageID">The page ID.</param>
        /// <param name="ignoreInheritance">Ignore ingeritance.</param>
        /// <param name="allowCache">Is caching based on the query allowed.</param>
        /// <returns></returns>
        public static async Task<Component[]> SelectAllInheritedAsync(int pageID, bool ignoreInheritance, bool allowCache)
        {
            var connector = ConnectorFactory.CreateConnector<Component>();
            connector.UseCacheOnSelect = allowCache;

            Page page = await Page.SelectOneAsync(pageID, false).ConfigureAwait(false);

            //  If the page is set to hold inherited content, please take that content
            if (!ignoreInheritance && page.InheritContent && page.MasterID.HasValue)
            {
                pageID = page.MasterID.Value;
            }

            var filter = connector.CreateDataFilter();
            filter.Add(x => x.PageID, pageID);
            filter.AddOrder(x => x.SortOrder, Sushi.MicroORM.SortOrder.ASC);

            var result = await connector.FetchAllAsync(filter).ConfigureAwait(false);
            foreach (var component in result)
            {
                if (!ignoreInheritance && page.InheritContent && page.MasterID.HasValue)
                {
                    component.PageID = page.ID;
                }
            }

            return result.ToArray();
        }

        public static async Task<Component[]> SelectAllSharedAsync(int pageID)
        {
            var connector = ConnectorFactory.CreateConnector<Component>();

            Page page = await Page.SelectOneAsync(pageID, false).ConfigureAwait(false);

            //  If the page is set to hold inherited content, please take that content
            if (page.InheritContent && page.MasterID.HasValue)
            {
                pageID = page.MasterID.Value;
            }

            var filter = connector.CreateDataFilter();
            filter.AddParameter("@pageId", pageID);
            filter.AddParameter("@isShared", true);

            var result = await connector.FetchAllAsync(@"
                SELECT [wim_Components].* FROM [wim_Components] 
                JOIN [wim_ComponentTargets] ON [Component_GUID] = [ComponentTarget_Component_Source]
				LEFT JOIN [dbo].[wim_ComponentTemplates] ON [ComponentTemplate_Key] = Component_ComponentTemplate_Key
                WHERE [ComponentTarget_Page_Key] = @pageId
                AND [ComponentTemplate_IsShared] = @isShared", filter).ConfigureAwait(false);

            foreach (var component in result)
            {
                //  When the content is set to inherit please apply the child page ID
                if (page.InheritContent && page.MasterID.HasValue)
                {
                    component.PageID = page.ID;
                }
            }

            return result.ToArray();
        }
    }
}