using System;
using System.Linq;
using System.Web;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Sushi.Mediakiwi.Data.DalReflection;

namespace Sushi.Mediakiwi.Data
{
    /// <summary>
    /// Represents a PageTemplate entity.
    /// </summary>
    [DatabaseTable("wim_PageTemplates", Order = "PageTemplate_ReferenceId ASC")]
    public class PageTemplate : DatabaseEntity, iExportable
    {
        /// <summary>
        /// Selects the all_ import export.
        /// </summary>
        /// <param name="portal">The portal.</param>
        /// <returns></returns>
        public static PageTemplate[] SelectAll_ImportExport(string portal)
        {
            PageTemplate implement = new PageTemplate();
            List<PageTemplate> list = new List<PageTemplate>();

            implement.SqlConnectionString = Sushi.Mediakiwi.Data.Common.GetPortal(portal).Connection;
            implement.ConnectionType = Sushi.Mediakiwi.Data.Common.GetPortal(portal).Type;

            foreach (object o in implement._SelectAll(null, false, "PageTemplateImportExport", portal))
                list.Add((PageTemplate)o);

            return list.ToArray();
        }

        /// <summary>
        /// Selects the one_ import export.
        /// </summary>
        /// <param name="portal">The portal.</param>
        /// <param name="location">The location.</param>
        /// <returns></returns>
        public static PageTemplate SelectOne_ImportExport(string portal, string location)
        {
            PageTemplate implement = new PageTemplate();

            implement.SqlConnectionString = Sushi.Mediakiwi.Data.Common.GetPortal(portal).Connection;
            implement.ConnectionType = Sushi.Mediakiwi.Data.Common.GetPortal(portal).Type;

            List<DatabaseDataValueColumn> list = new List<DatabaseDataValueColumn>();
            list.Add(new DatabaseDataValueColumn("PageTemplate_Location", SqlDbType.VarChar, location));
            return (PageTemplate)implement._SelectOne(list);
        }

        #region MOVED to EXTENSION / LOGIC



        //string[] _Sections;
        ///// <summary>
        ///// Gets the sections.
        ///// </summary>
        ///// <returns></returns>
        //public string[] GetPageSections()
        //{
        //    //if (_Sections == null)
        //    //{
        //    List<string> sections = new List<string>();
        //    if (this.Data != null && this.Data.HasProperty("TAB.INFO"))
        //    {
        //        sections.AddRange(this.Data["TAB.INFO"].Value.Split(','));
        //    }

        //    var availableTemplateList = Sushi.Mediakiwi.Data.AvailableTemplate.SelectAll(this.ID);
        //    var sectionViaTemplate = (from item in availableTemplateList select item.Target).Distinct().ToList();

        //    if (sectionViaTemplate.Count == 1)
        //    {
        //        if (sectionViaTemplate[0] == null)
        //        {
        //            var count1 = availableTemplateList.Count(x => !x.IsSecundary);
        //            var count2 = availableTemplateList.Count(x => x.IsSecundary);

        //            if (count1 > 0)
        //            {
        //                sectionViaTemplate[0] = Wim.CommonConfiguration.DEFAULT_CONTENT_TAB;
        //                if (count2 > 0)
        //                    sectionViaTemplate.Add(Wim.CommonConfiguration.DEFAULT_SERVICE_TAB);
        //            }
        //            else if (count2 > 0)
        //            {
        //                sectionViaTemplate[0] = Wim.CommonConfiguration.DEFAULT_SERVICE_TAB;
        //            }
        //        }
        //    }

        //    var legacyContentTab = this.Data["TAB.LCT"].Value;

        //    bool isEmpty = sections.Count == 0;
        //    sectionViaTemplate.ForEach(section => { sections.Add(section); });

        //    if (!isEmpty)
        //        sections = (from item in sections select item).Distinct().ToList();

        //    if (sections.Count == 0)
        //        sections.Add(Wim.CommonConfiguration.DEFAULT_CONTENT_TAB);

        //    _Sections = sections.ToArray();
        //    //}
        //    return _Sections;
        //}

        #endregion MOVED to EXTENSION / LOGIC

        #region REPLICATED TO Sushi.Mediakiwi.Data.Standard

        int m_PageInstanceCount;
        /// <summary>
        /// The number of pages that have been implemented with this template
        /// </summary>
        /// <value>The page instance count.</value>
        [DatabaseColumn("PageCount", SqlDbType.Int, ColumnSubQuery = "select COUNT(*) from wim_Pages a where a.Page_Template_Key = wim_PageTemplates.PageTemplate_Key")]
        public int PageInstanceCount
        {
            get { return m_PageInstanceCount; }
            set { m_PageInstanceCount = value; }
        }

        /// <summary>
        /// Checks the component templates.
        /// </summary>
        public void CheckComponentTemplates()
        {
            var pages = Sushi.Mediakiwi.Data.Page.SelectAllBasedOnPageTemplate(this.ID);
            foreach (var page in pages)
                CheckComponentTemplates(page.ID);
        }

        /// <summary>
        /// Checks the component templates.
        /// </summary>
        /// <param name="pageID">The page ID.</param>
        public void CheckComponentTemplates(int pageID)
        {
            int count = 1000;

            //  This one should be via a button
            //Sushi.Mediakiwi.Data.ComponentVersion.RemoveInvalidPageReference(pageID);

            Sushi.Mediakiwi.Data.IAvailableTemplate[] availableTemplateList = Sushi.Mediakiwi.Data.AvailableTemplate.SelectAll(this.ID, pageID, true);

            Data.ComponentTemplate template;
            Sushi.Mediakiwi.Data.ComponentVersion version;

            foreach (Sushi.Mediakiwi.Data.AvailableTemplate availableTemplate in availableTemplateList)
            {
                template = Sushi.Mediakiwi.Data.ComponentTemplate.SelectOne(availableTemplate.ComponentTemplateID);
                if (template.IsHeader) count = 0;

                version = new Sushi.Mediakiwi.Data.ComponentVersion();
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
                if (template.IsHeader) count = 1000;
            }
        }

        /// <summary>
        /// Save a database entity. This method will indentify the need for an INSERT or an UPDATE.
        /// </summary>
        /// <returns></returns>
        public override bool Save()
        {
            bool isSaved = base.Save();
            return isSaved;
        }

        /// <summary>
        /// Select a Page Template instance based on its primary key
        /// </summary>
        public static PageTemplate SelectOne(int ID)
        {
            return (PageTemplate)new PageTemplate()._SelectOne(ID);
        }

        /// <summary>
        /// Delete an implementation record.
        /// </summary>
        /// <returns></returns>
        public override bool Delete()
        {
            using (Sushi.Mediakiwi.Data.Connection.DataCommander dac = new Sushi.Mediakiwi.Data.Connection.DataCommander(Sushi.Mediakiwi.Data.Common.DatabaseConnectionString))
            {
                dac.Text = "delete from wim_AvailableTemplates where AvailableTemplates_PageTemplate_Key = @PageTemplate_Key";
                dac.SetParameterInput("@PageTemplate_Key", this.ID, SqlDbType.Int);
                dac.ExecNonQuery();
            }
            return base.Delete();
        }

        internal static PageTemplate SelectOneByChannelAndPage(int p1, int p2)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Select a Component Template instance based on its migration (GUID) key
        /// </summary>
        /// <param name="pageTemplateGUID">The page template GUID.</param>
        /// <returns></returns>
        public static PageTemplate SelectOne(Guid pageTemplateGUID)
        {
            PageTemplate item = new PageTemplate();
            List<DatabaseDataValueColumn> list = new List<DatabaseDataValueColumn>();
            list.Add(new DatabaseDataValueColumn(item.MigrationKeyColumn, SqlDbType.UniqueIdentifier, pageTemplateGUID));
            return (PageTemplate)item._SelectOne(list);
        }

        /// <summary>
        /// CB: This finds the template which must be used instead of the normally (trough inheritance) associated
        /// </summary>
        /// <param name="siteID">The siteID from the URL that is being served</param>
        /// <param name="pageTemplateID">The pageTemplateID we are to find</param>
        /// <returns></returns>
        internal static PageTemplate SelectOneOverwrite(int siteID, int pageTemplateID)
        {
            PageTemplate item = new PageTemplate();
            List<DatabaseDataValueColumn> list = new List<DatabaseDataValueColumn>();
            list.Add(new DatabaseDataValueColumn("PageTemplate_OverwriteSite_Key", SqlDbType.Int, siteID));
            list.Add(new DatabaseDataValueColumn("PageTemplate_OverwritePageTemplate_Key", SqlDbType.Int, pageTemplateID));
            return (PageTemplate)item._SelectOne(list, "PageTemplateOverwrite", siteID + "_" + pageTemplateID);
        }

        /// <summary>
        /// Selects the one.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <returns></returns>
        public static PageTemplate SelectOne(string location)
        {
            PageTemplate item = new PageTemplate();
            List<DatabaseDataValueColumn> list = new List<DatabaseDataValueColumn>();
            list.Add(new DatabaseDataValueColumn("PageTemplate_Location", SqlDbType.VarChar, location));
            return (PageTemplate)item._SelectOne(list);
        }

        /// <summary>
        /// Selects the one.
        /// </summary>
        /// <param name="reference">The reference.</param>
        /// <returns></returns>
        public static PageTemplate SelectOneByReference(string reference)
        {
            PageTemplate item = new PageTemplate();
            List<DatabaseDataValueColumn> list = new List<DatabaseDataValueColumn>();
            list.Add(new DatabaseDataValueColumn("PageTemplate_ReferenceId", SqlDbType.NVarChar, reference));
            return (PageTemplate)item._SelectOne(list);
        }

        /// <summary>
        /// Select all Page Templates
        /// </summary>
        /// <returns></returns>
        public static PageTemplate[] SelectAll()
        {
            List<PageTemplate> list = new List<PageTemplate>();
            PageTemplate pageTemplate = new PageTemplate();
            if (!string.IsNullOrEmpty(SqlConnectionString2)) pageTemplate.SqlConnectionString = SqlConnectionString2;
            foreach (object o in pageTemplate._SelectAll()) list.Add((PageTemplate)o);
            return list.ToArray();
        }

        /// <summary>
        /// Selects the name of all sorted by.
        /// </summary>
        /// <returns></returns>
        public static PageTemplate[] SelectAllSortedByName()
        {
            List<PageTemplate> list = new List<PageTemplate>();
            PageTemplate pageTemplate = new PageTemplate();
            pageTemplate.SqlOrder = "PageTemplate_Name ASC";

            if (!string.IsNullOrEmpty(SqlConnectionString2)) pageTemplate.SqlConnectionString = SqlConnectionString2;
            foreach (object o in pageTemplate._SelectAll()) list.Add((PageTemplate)o);
            return list.ToArray();
        }

        /// <summary>
        /// Searches all.
        /// </summary>
        /// <param name="siteID">The site ID.</param>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public static PageTemplate[] SearchAll(int siteID, string text)
        {
            List<PageTemplate> list = new List<PageTemplate>();
            PageTemplate pageTemplate = new PageTemplate();

            List<DatabaseDataValueColumn> whereClause = new List<DatabaseDataValueColumn>();

            if (siteID > 0)
                whereClause.Add(new DatabaseDataValueColumn("PageTemplate_Site_Key", SqlDbType.Int, siteID));

            if (!string.IsNullOrEmpty(text))
            {
                text = string.Concat("%", text.Trim().Replace(" ", "%"), "%");
                whereClause.Add(new DatabaseDataValueColumn("PageTemplate_Name", SqlDbType.NVarChar, text, DatabaseDataValueCompareType.Like));
            }

            //ComponentList clist = ComponentList.SelectOne(ComponentListType.PageTemplates);

            if (!string.IsNullOrEmpty(SqlConnectionString2)) pageTemplate.SqlConnectionString = SqlConnectionString2;
            foreach (object o in pageTemplate._SelectAll(whereClause, false, null, null)) list.Add((PageTemplate)o);
            return list.ToArray();
        }

        int m_ID;
        /// <summary>
        /// Template primary key.
        /// </summary>
        [DatabaseColumn("PageTemplate_Key", SqlDbType.Int, IsPrimaryKey = true)]
        public int ID
        {
            get { return m_ID; }
            set { m_ID = value; }
        }

        private Guid m_GUID;
        /// <summary>
        /// Unique identifier used for import/export
        /// </summary>
        [DatabaseColumn("PageTemplate_GUID", SqlDbType.UniqueIdentifier, IsMigrationKey = true)]
        public Guid GUID
        {
            get {
                if (this.m_GUID == Guid.Empty) this.m_GUID = Guid.NewGuid();
                return m_GUID; }
            set { m_GUID = value; }
        }

        private int? m_SiteID;
        /// <summary>
        /// Part of a specific site or site group (when the site is a master site with children this template is automatically part of those children).
        /// </summary>
        [DatabaseColumn("PageTemplate_Site_Key", SqlDbType.Int, IsNullable = true)]
        public int? SiteID
        {
            get { return m_SiteID; }
            set { m_SiteID = value; }
        }

        [DatabaseColumn("PageTemplate_OverwriteSite_Key", SqlDbType.Int, IsNullable = true)]
        public int? OverwriteSiteKey
        {
            get;
            set;
        }

        [DatabaseColumn("PageTemplate_OverwritePageTemplate_Key", SqlDbType.Int, IsNullable = true)]
        public int? OverwriteTemplateKey
        {
            get;
            set;
        }

        [DatabaseColumn("PageTemplate_IsSourceBased", SqlDbType.Bit, IsNullable = true)]
        public bool IsSourceBased { get; set; }

        /// <summary>
        /// Gets or sets the reference id.
        /// </summary>
        /// <value>The reference id.</value>
        [DatabaseColumn("PageTemplate_ReferenceId", SqlDbType.NVarChar, Length = 5, IsNullable = true)]
        public string ReferenceID { get; set; }

        Sushi.Mediakiwi.Data.CustomData m_Data;
        [DatabaseColumn("PageTemplate_Data", SqlDbType.Xml, IsNullable = true)]
        public Sushi.Mediakiwi.Data.CustomData Data
        {
            get
            {
                if (m_Data == null)
                    m_Data = new Sushi.Mediakiwi.Data.CustomData();
                return m_Data;
            }
            set { m_Data = value; }
        }

        private string m_Description;
        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        [DatabaseColumn("PageTemplate_Description", SqlDbType.NVarChar, Length = 500, IsNullable = true)]
        public string Description
        {
            get { return m_Description; }
            set { m_Description = value; }
        }

        internal bool m_IsSourceCalled;
        internal bool m_IsSourceChanged;
        string m_Source;
        /// <summary>
        /// Gets or sets the source.
        /// </summary>
        /// <value>
        /// The source.
        /// </value>
        [DatabaseColumn("PageTemplate_Source", SqlDbType.NText, IsNullable = true)]
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
                        string check1 = Wim.Utility.CreateChecksum(m_Source);
                        string check2 = Wim.Utility.CreateChecksum(value);

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
            get {
                if (m_AvailableTemplateList == null)
                    m_AvailableTemplateList = Sushi.Mediakiwi.Data.AvailableTemplate.SelectAll(this.ID);

                if (m_AvailableTemplateList == null) return null;

                string candidate = "";
                foreach (Data.AvailableTemplate t in m_AvailableTemplateList)
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

        Sushi.Mediakiwi.Data.IAvailableTemplate[] m_AvailableTemplateList;

        /// <summary>
        /// Gets the component text.
        /// </summary>
        /// <value>The component text.</value>
        public string ComponentListInfoText2
        {
            get
            {
                if (m_AvailableTemplateList == null)
                    m_AvailableTemplateList = Sushi.Mediakiwi.Data.AvailableTemplate.SelectAll(this.ID);

                if (m_AvailableTemplateList == null) return null;

                string candidate = "";
                foreach (Data.AvailableTemplate t in m_AvailableTemplateList)
                {
                    if (!t.IsSecundary)
                        continue;

                    if (candidate.Length > 0)
                        candidate += "\n";

                    //if (t.ReferenceId > 0)
                    //    candidate += string.Format("- {0} ({1})", t.Name, t.ReferenceId);
                    //else
                        candidate += string.Concat("- ", t.ComponentTemplate);
                }
                return candidate;
            }
        }

        string m_Name;
        /// <summary>
        /// Name of the page template.
        /// </summary>
        [DatabaseColumn("PageTemplate_Name", SqlDbType.NVarChar, Length=50)]
        public string Name
        {
            get { return m_Name; }
            set { m_Name = value; }
        }

        bool m_OnlyOneInstancePossible;
        /// <summary>
        /// Can only be used one time for a page instance.
        /// </summary>
        [DatabaseColumn("PageTemplate_OnlyOneInstance", SqlDbType.Bit)]
        public bool OnlyOneInstancePossible
        {
            get { return m_OnlyOneInstancePossible; }
            set { m_OnlyOneInstancePossible = value; }
        }

        bool m_HasCustomDate;
        /// <summary>
        /// Gets or sets a value indicating whether this instance has custom date.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has custom date; otherwise, <c>false</c>.
        /// </value>
        [DatabaseColumn("PageTemplate_HasCustomDate", SqlDbType.Bit)]
        public bool HasCustomDate
        {
            get { return m_HasCustomDate; }
            set { m_HasCustomDate = value; }
        }

        /// <summary>
        /// Is cached as a whole. Beware with forms and dynamic pages.
        /// </summary>
        [DatabaseColumn("PageTemplate_AddToOutputCache", SqlDbType.Bit)]
        public bool IsAddedOutputCache { get; set; }

        [DatabaseColumn("PageTemplate_OutputCache", SqlDbType.Int, IsNullable = true)]
        public int? OutputCacheDuration { get; set; }


        bool m_HasSecundaryContentContainer;
        /// <summary>
        /// Does the page template have a secundary (like a service column) content container.
        /// </summary>
        [DatabaseColumn("PageTemplate_HasSecundaryContainer", SqlDbType.Bit)]
        public bool HasSecundaryContentContainer
        {
            get { return m_HasSecundaryContentContainer; }
            set { m_HasSecundaryContentContainer = value; }
        }

        string m_Location;
        /// <summary>
        /// Relative path of the template (.aspx).
        /// </summary>
        [DatabaseColumn("PageTemplate_Location", SqlDbType.VarChar, Length = 250, IsNullable = true)]
        public string Location
        {
            get { return m_Location; }
            set { m_Location = value; }
        }

        DateTime m_LastWriteTimeUtc;
        /// <summary>
        /// Gets or sets the last write time UTC of the page template (ASPX).
        /// </summary>
        /// <value>The last write time UTC.</value>
        [DatabaseColumn("PageTemplate_LastWriteTimeUtc", SqlDbType.DateTime, IsNullable=true )]
        public DateTime LastWriteTimeUtc
        {
            get { return m_LastWriteTimeUtc; }
            set { m_LastWriteTimeUtc = value; }
        }


        public DateTime? Updated
        {
            get { return LastWriteTimeUtc; }
        }

        #endregion REPLICATED TO Sushi.Mediakiwi.Data.Standard
    }
}