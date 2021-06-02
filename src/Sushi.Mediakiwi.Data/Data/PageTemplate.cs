using Sushi.MicroORM;
using Sushi.MicroORM.Mapping;
using System;
using System.Threading.Tasks;
using Sushi.Mediakiwi.Data.MicroORM;

namespace Sushi.Mediakiwi.Data
{
    /// <summary>
    /// Represents a PageTemplate entity.
    /// </summary>
    [DataMap(typeof(PageTemplateMap))]
    public class PageTemplate : IExportable
    {
        public class PageTemplateMap : DataMap<PageTemplate>
        {
            public PageTemplateMap()
            {
                Table("wim_PageTemplates");
                Id(x => x.ID, "PageTemplate_Key").Identity();
                Map(x => x.GUID, "PageTemplate_GUID");
                Map(x => x.SiteID, "PageTemplate_Site_Key");
                Map(x => x.OverwriteSiteKey, "PageTemplate_OverwriteSite_Key");
                Map(x => x.OverwriteTemplateKey, "PageTemplate_OverwritePageTemplate_Key");
                Map(x => x.IsSourceBased, "PageTemplate_IsSourceBased");
                Map(x => x.ReferenceID, "PageTemplate_ReferenceId");
                Map(x => x.DataString, "PageTemplate_Data").SqlType(System.Data.SqlDbType.Xml);
                Map(x => x.Description, "PageTemplate_Description").Length(500);
                Map(x => x.Source, "PageTemplate_Source");
                Map(x => x.Name, "PageTemplate_Name").Length(50);
                Map(x => x.OnlyOneInstancePossible, "PageTemplate_OnlyOneInstance");
                Map(x => x.HasCustomDate, "PageTemplate_HasCustomDate");
                Map(x => x.IsAddedOutputCache, "PageTemplate_AddToOutputCache");
                Map(x => x.OutputCacheDuration, "PageTemplate_OutputCache");
                Map(x => x.HasSecundaryContentContainer, "PageTemplate_HasSecundaryContainer");
                Map(x => x.Location, "PageTemplate_Location").Length(250);
                Map(x => x.LastWriteTimeUtc, "PageTemplate_LastWriteTimeUtc");

                Map(x => x.PageInstanceCount, "(SELECT COUNT(*) FROM [wim_Pages] A WHERE A.[Page_Template_Key] = [PageTemplate_Key])").Alias("PageCount").ReadOnly();
            }
        }

        /// <summary>
        /// Template primary key.
        /// </summary>
        public int ID { get; set; }

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
        /// Part of a specific site or site group (when the site is a master site with children this template is automatically part of those children).
        /// </summary>
        public int? SiteID { get; set; }

        public int? OverwriteSiteKey { get; set; }

        public int? OverwriteTemplateKey { get; set; }

        public bool IsSourceBased { get; set; }

        /// <summary>
        /// Gets or sets the reference id.
        /// </summary>
        /// <value>The reference id.</value>
        public int ReferenceID { get; set; }

        public string DataString { get; set; }

        private CustomData m_Data;

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
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        public string Description { get; set; }

        internal bool m_IsSourceCalled;
        internal bool m_IsSourceChanged;

        private string m_Source;

        /// <summary>
        /// The number of pages that have been implemented with this template
        /// </summary>
        /// <value>The page instance count.</value>
        public int PageInstanceCount { get; set; }

        /// <summary>
        /// Gets or sets the source.
        /// </summary>
        /// <value>
        /// The source.
        /// </value>
        public string Source
        {
            get { return m_Source; }
            set
            {
                if (m_IsSourceCalled)
                {
                    if (string.IsNullOrEmpty(m_Source) && string.IsNullOrEmpty(value))
                    {
                    }
                    else if (string.IsNullOrEmpty(m_Source) || string.IsNullOrEmpty(value))
                    {
                        m_IsSourceChanged = true;
                    }
                    else
                    {
                        string check1 = Sushi.Mediakiwi.Data.Utility.CreateChecksum(m_Source);
                        string check2 = Sushi.Mediakiwi.Data.Utility.CreateChecksum(value);

                        if (!check1.Equals(check2))
                            m_IsSourceChanged = true;
                    }
                }
                m_Source = value;
                m_IsSourceCalled = true;
            }
        }

        /// <summary>
        /// Gets the component text.
        /// </summary>
        /// <value>The component text.</value>
        public string ComponentListInfoText1
        {
            get
            {
                if (m_AvailableTemplateList == null)
                    m_AvailableTemplateList = AvailableTemplate.SelectAll(this.ID);

                if (m_AvailableTemplateList == null) return null;

                string candidate = "";
                foreach (AvailableTemplate t in m_AvailableTemplateList)
                {
                    if (t.IsSecundary)
                        continue;

                    if (candidate.Length > 0)
                        candidate += "\n";

                    //if (t.ReferenceId > 0)
                    //    candidate += string.Format("- {0} ({1})", t.ComponentTemplate, t.ReferenceId);
                    //else
                    candidate += string.Concat("- ", t.ComponentTemplate);
                }
                return candidate;
            }
        }

        private IAvailableTemplate[] m_AvailableTemplateList;

        /// <summary>
        /// Gets the component text.
        /// </summary>
        /// <value>The component text.</value>
        public string ComponentListInfoText2
        {
            get
            {
                if (m_AvailableTemplateList == null)
                    m_AvailableTemplateList = AvailableTemplate.SelectAll(this.ID);

                if (m_AvailableTemplateList == null) return null;

                string candidate = "";
                foreach (AvailableTemplate t in m_AvailableTemplateList)
                {
                    if (!t.IsSecundary)
                        continue;

                    if (candidate.Length > 0)
                        candidate += "\n";

                    candidate += string.Concat("- ", t.ComponentTemplate);
                }
                return candidate;
            }
        }

        /// <summary>
        /// Name of the page template.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Can only be used one time for a page instance.
        /// </summary>
        public bool OnlyOneInstancePossible { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance has custom date.
        /// </summary>
        public bool HasCustomDate { get; set; }

        /// <summary>
        /// Is cached as a whole. Beware with forms and dynamic pages.
        /// </summary>
        public bool IsAddedOutputCache { get; set; }

        public int? OutputCacheDuration { get; set; }

        /// <summary>
        /// Does the page template have a secundary (like a service column) content container.
        /// </summary>
        public bool HasSecundaryContentContainer { get; set; }

        /// <summary>
        /// Relative path of the template (.aspx).
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// Gets or sets the last write time UTC of the page template (ASPX).
        /// </summary>
        /// <value>The last write time UTC.</value>
        public DateTime? LastWriteTimeUtc { get; set; }

        public DateTime? Updated
        {
            get { return LastWriteTimeUtc; }
        }

        /// <summary>
        /// Checks the component templates.
        /// </summary>
        public void CheckComponentTemplates()
        {
            var pages = Page.SelectAllBasedOnPageTemplate(ID);
            foreach (var page in pages)
                CheckComponentTemplates(page.ID);
        }

        /// <summary>
        /// Checks the component templates.
        /// </summary>
        public async Task CheckComponentTemplatesAsync()
        {
            var pages = await Page.SelectAllBasedOnPageTemplateAsync(ID);
            foreach (var page in pages)
                await CheckComponentTemplatesAsync(page.ID);
        }

        /// <summary>
        /// Checks the component templates.
        /// </summary>
        /// <param name="pageID">The page ID.</param>
        public void CheckComponentTemplates(int pageID)
        {
            int count = 1000;

            IAvailableTemplate[] availableTemplateList = AvailableTemplate.SelectAll(ID, pageID, true);

            ComponentTemplate template;
            ComponentVersion version;

            foreach (AvailableTemplate availableTemplate in availableTemplateList)
            {
                template = ComponentTemplate.SelectOne(availableTemplate.ComponentTemplateID);
                if (template.IsHeader) 
                    count = 0;

                version = new ComponentVersion();
                version.ApplicationUserID = null;
                version.TemplateID = availableTemplate.ComponentTemplateID;
                version.PageID = pageID;
                version.Name = availableTemplate.ComponentTemplate;
                version.FixedFieldName = availableTemplate.FixedFieldName;
                version.IsSecundary = availableTemplate.IsSecundary;
                version.AvailableTemplateID = availableTemplate.ID;
                version.IsFixed = true;
                version.IsAlive = true;
                version.IsActive = true;// template.CanDeactivate ? false : true;
                version.SortOrder = count;
                version.Target = availableTemplate.Target;
                version.Save();
                if (template.IsHeader) 
                    count = 1000;
            }
        }

        /// <summary>
        /// Checks the component templates.
        /// </summary>
        /// <param name="pageID">The page ID.</param>
        public async Task CheckComponentTemplatesAsync(int pageID)
        {
            int count = 1000;

            IAvailableTemplate[] availableTemplateList = await AvailableTemplate.SelectAllAsync(ID, pageID, true);

            ComponentTemplate template;
            ComponentVersion version;

            foreach (AvailableTemplate availableTemplate in availableTemplateList)
            {
                template = await ComponentTemplate.SelectOneAsync(availableTemplate.ComponentTemplateID);
                if (template.IsHeader) 
                    count = 0;

                version = new ComponentVersion();
                version.ApplicationUserID = null;
                version.TemplateID = availableTemplate.ComponentTemplateID;
                version.PageID = pageID;
                version.Name = availableTemplate.ComponentTemplate;
                version.FixedFieldName = availableTemplate.FixedFieldName;
                version.IsSecundary = availableTemplate.IsSecundary;
                version.AvailableTemplateID = availableTemplate.ID;
                version.IsFixed = true;
                version.IsAlive = true;
                version.IsActive = true;// template.CanDeactivate ? false : true;
                version.SortOrder = count;
                version.Target = availableTemplate.Target;
                await version.SaveAsync();
                if (template.IsHeader) 
                    count = 1000;
            }
        }


        /// <summary>
        /// Save a database entity. This method will indentify the need for an INSERT or an UPDATE.
        /// </summary>
        /// <returns></returns>
        public bool Save()
        {
            var connector = ConnectorFactory.CreateConnector<PageTemplate>();
            try
            {
                connector.Save(this);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Save a database entity. This method will indentify the need for an INSERT or an UPDATE.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> SaveAsync()
        {
            var connector = ConnectorFactory.CreateConnector<PageTemplate>();
            try
            {
                await connector.SaveAsync(this);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Select a Page Template instance based on its primary key
        /// </summary>
        public static PageTemplate SelectOne(int ID)
        {
            var connector = ConnectorFactory.CreateConnector<PageTemplate>();
            return connector.FetchSingle(ID);
        }

        /// <summary>
        /// Select a Page Template instance based on its primary key Async
        /// </summary>
        public static async Task<PageTemplate> SelectOneAsync(int ID)
        {
            var connector = ConnectorFactory.CreateConnector<PageTemplate>();
            return await connector.FetchSingleAsync(ID);
        }

        /// <summary>
        /// Delete an implementation record.
        /// </summary>
        /// <returns></returns>
        public bool Delete()
        {
            var connector = ConnectorFactory.CreateConnector<PageTemplate>();
            var filter = connector.CreateDataFilter();
            filter.AddParameter("@thisId", ID);
            try
            {
                connector.ExecuteNonQuery("DELETE FROM [wim_AvailableTemplates] WHERE [AvailableTemplates_PageTemplate_Key] = @thisId", filter);
                connector.Delete(this);
				connector.Cache?.FlushRegion(connector.CacheRegion);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Delete an implementation record Async.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> DeleteAsync()
        {
            var connector = ConnectorFactory.CreateConnector<PageTemplate>();
            var filter = connector.CreateDataFilter();
            filter.AddParameter("@thisId", ID);
            try
            {
                await connector.ExecuteNonQueryAsync("DELETE FROM [wim_AvailableTemplates] WHERE [AvailableTemplates_PageTemplate_Key] = @thisId", filter);
                await connector.DeleteAsync(this);
				connector.Cache?.FlushRegion(connector.CacheRegion);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Select a Component Template instance based on its migration (GUID) key
        /// </summary>
        /// <param name="guid">The page template GUID.</param>
        /// <returns></returns>
        public static PageTemplate SelectOne(Guid guid)
        {
            var connector = ConnectorFactory.CreateConnector<PageTemplate>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.GUID, guid);
            return connector.FetchSingle(filter);
        }

        /// <summary>
        /// Select a Component Template instance based on its migration (GUID) key
        /// </summary>
        /// <param name="guid">The page template GUID.</param>
        /// <returns></returns>
        public static async Task<PageTemplate> SelectOneAsync(Guid guid)
        {
            var connector = ConnectorFactory.CreateConnector<PageTemplate>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.GUID, guid);
            return await connector.FetchSingleAsync(filter);
        }

        /// <summary>
        /// This finds the template which must be used instead of the normally (trough inheritance) associated
        /// </summary>
        /// <param name="siteID">The siteID from the URL that is being served</param>
        /// <param name="pageTemplateID">The pageTemplateID we are to find</param>
        /// <returns></returns>
        public static PageTemplate SelectOneOverwrite(int siteID, int pageTemplateID)
        {
            var connector = ConnectorFactory.CreateConnector<PageTemplate>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.OverwriteSiteKey, siteID);
            filter.Add(x => x.OverwriteTemplateKey, pageTemplateID);

            return connector.FetchSingle(filter);
        }

        /// <summary>
        /// This finds the template which must be used instead of the normally (trough inheritance) associated
        /// </summary>
        /// <param name="siteID">The siteID from the URL that is being served</param>
        /// <param name="pageTemplateID">The pageTemplateID we are to find</param>
        /// <returns></returns>
        public static async Task<PageTemplate> SelectOneOverwriteAsync(int siteID, int pageTemplateID)
        {
            var connector = ConnectorFactory.CreateConnector<PageTemplate>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.OverwriteSiteKey, siteID);
            filter.Add(x => x.OverwriteTemplateKey, pageTemplateID);

            return await connector.FetchSingleAsync(filter);
        }

        /// <summary>
        /// Selects one by the location.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <returns></returns>
        public static PageTemplate SelectOne(string location)
        {
            var connector = ConnectorFactory.CreateConnector<PageTemplate>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.Location, location);

            return connector.FetchSingle(filter);
        }

        /// <summary>
        /// Selects one by the location Async.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <returns></returns>
        public static async Task<PageTemplate> SelectOneAsync(string location)
        {
            var connector = ConnectorFactory.CreateConnector<PageTemplate>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.Location, location);

            return await connector.FetchSingleAsync(filter);
        }

        /// <summary>
        /// Selects one by reference.
        /// </summary>
        /// <param name="referenceID">The reference.</param>
        /// <returns></returns>
        public static PageTemplate SelectOneByReference(int referenceID)
        {
            var connector = ConnectorFactory.CreateConnector<PageTemplate>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.ReferenceID, referenceID);

            return connector.FetchSingle(filter);
        }

        /// <summary>
        /// Selects one by reference.
        /// </summary>
        /// <param name="referenceID">The reference.</param>
        /// <returns></returns>
        public static async Task<PageTemplate> SelectOneByReferenceAsync(int referenceID)
        {
            var connector = ConnectorFactory.CreateConnector<PageTemplate>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.ReferenceID, referenceID);

            return await connector.FetchSingleAsync(filter);
        }

        /// <summary>
        /// Select all Page Templates
        /// </summary>
        /// <returns></returns>
        public static PageTemplate[] SelectAll()
        {
            var connector = ConnectorFactory.CreateConnector<PageTemplate>();
            var filter = connector.CreateDataFilter();
            filter.AddOrder(x => x.ReferenceID);

            return connector.FetchAll(filter).ToArray();
        }

        /// <summary>
        /// Select all Page Templates Async
        /// </summary>
        /// <returns></returns>
        public static async Task<PageTemplate[]> SelectAllAsync()
        {
            var connector = ConnectorFactory.CreateConnector<PageTemplate>();
            var filter = connector.CreateDataFilter();
            filter.AddOrder(x => x.ReferenceID);

            var result = await connector.FetchAllAsync(filter);
            return result.ToArray();
        }

        /// <summary>
        /// Selects all Page Templates sorted by Name.
        /// </summary>
        /// <returns></returns>
        public static PageTemplate[] SelectAllSortedByName()
        {
            var connector = ConnectorFactory.CreateConnector<PageTemplate>();
            var filter = connector.CreateDataFilter();
            filter.AddOrder(x => x.Name);

            return connector.FetchAll(filter).ToArray();
        }

        /// <summary>
        /// Selects all Page Templates sorted by Name Async.
        /// </summary>
        /// <returns></returns>
        public static async Task<PageTemplate[]> SelectAllSortedByNameAsync()
        {
            var connector = ConnectorFactory.CreateConnector<PageTemplate>();
            var filter = connector.CreateDataFilter();
            filter.AddOrder(x => x.Name);

            var result = await connector.FetchAllAsync(filter);
            return result.ToArray();
        }

        /// <summary>
        /// Searches all by Siet ID and Name.
        /// </summary>
        /// <param name="siteID">The site ID.</param>
        /// <param name="name">The name to search for.</param>
        /// <returns></returns>
        public static PageTemplate[] SearchAll(int siteID, string name)
        {
            var connector = ConnectorFactory.CreateConnector<PageTemplate>();
            var filter = connector.CreateDataFilter();
            filter.AddOrder(x => x.ReferenceID);

            if (siteID > 0)
                filter.Add(x => x.SiteID, siteID);

            if (!string.IsNullOrEmpty(name))
            {
                name = string.Concat("%", name.Trim().Replace(" ", "%"), "%");
                filter.Add(x => x.Name, name, ComparisonOperator.Like);
            }

            return connector.FetchAll(filter).ToArray();
        }

        /// <summary>
        /// Searches all by Siet ID and Name Async.
        /// </summary>
        /// <param name="siteID">The site ID.</param>
        /// <param name="name">The name to search for.</param>
        /// <returns></returns>
        public static async Task<PageTemplate[]> SearchAllAsync(int siteID, string name)
        {
            var connector = ConnectorFactory.CreateConnector<PageTemplate>();
            var filter = connector.CreateDataFilter();
            filter.AddOrder(x => x.ReferenceID);

            if (siteID > 0)
                filter.Add(x => x.SiteID, siteID);

            if (!string.IsNullOrEmpty(name))
            {
                name = string.Concat("%", name.Trim().Replace(" ", "%"), "%");
                filter.Add(x => x.Name, name, ComparisonOperator.Like);
            }

            var result = await connector.FetchAllAsync(filter);
            return result.ToArray();
        }
    }
}