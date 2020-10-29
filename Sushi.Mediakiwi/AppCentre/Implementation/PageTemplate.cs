using System;
using System.Linq;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Sushi.Mediakiwi.Framework;
using System.Collections.Generic;

namespace Sushi.Mediakiwi.AppCentre.Data.Implementation
{
    /// <summary>
    /// 
    /// </summary>
    public class PageTemplate : BaseImplementation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PageTemplate"/> class.
        /// </summary>
        public PageTemplate()
        {
            wim.OpenInEditMode = true;

            IS_HEADLESS = Wim.CommonConfiguration.HEADLESS_CMS;

            this.ListSearch += new ComponentSearchEventHandler(PageTemplate_ListSearch);
            this.ListLoad += new ComponentListEventHandler(PageTemplate_ListLoad);
            this.ListPreRender += PageTemplate_ListPreRender;
            this.ListSave += new ComponentListEventHandler(PageTemplate_ListSave);
            this.ListAction += new ComponentActionEventHandler(PageTemplate_ListAction);
        }

        void PageTemplate_ListPreRender(object sender, ComponentListEventArgs e)
        {
            if (IsPostBack)
            {
                SetSourceVisibility();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [set cacheable].
        /// </summary>
        /// <value><c>true</c> if [set cacheable]; otherwise, <c>false</c>.</value>
        //[Sushi.Mediakiwi.Framework.ContentListItem.Button("Convert components to page cache")]
        public bool SetCacheable { get; set; }

        void PageTemplate_ListAction(object sender, ComponentActionEventArgs e)
        {
            if (SetCacheable)
            {
                if (this.AvailableComponent != null)
                    foreach (Sushi.Mediakiwi.Data.SubList.SubListitem item in this.AvailableComponent.Items)
                    {
                        int templateID = Convert.ToInt32(item.TextID.Split('.')[1]);
                        Sushi.Mediakiwi.Data.ComponentTemplate ct = Sushi.Mediakiwi.Data.ComponentTemplate.SelectOne(templateID);
                        ct.CacheLevel = 2;
                        ct.SaveData();
                    }
                if (this.AvailableServiceComponent != null)
                    foreach (Sushi.Mediakiwi.Data.SubList.SubListitem item in this.AvailableServiceComponent.Items)
                    {
                        int templateID = Convert.ToInt32(item.TextID.Split('.')[1]);
                        Sushi.Mediakiwi.Data.ComponentTemplate ct = Sushi.Mediakiwi.Data.ComponentTemplate.SelectOne(templateID);
                        ct.CacheLevel = 2;
                        ct.SaveData();
                    }
                Sushi.Mediakiwi.Data.EnvironmentVersionLogic.Flush();
            }
        }

        void PageTemplate_ListDelete(object sender, ComponentListEventArgs e)
        {
            m_Instance.Delete();
        }

        /// <summary>
        /// Sets the available templates.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="IsSecundary">if set to <c>true</c> [is secundary].</param>
        void SetAvailableTemplates(string[] items, bool IsSecundary, string target)
        {
            if (items == null || items.Length == 0)
            {
                Sushi.Mediakiwi.Data.AvailableTemplate.Delete(m_Instance.ID, IsSecundary, target);
            }
            else
            {
                //  Clenup all removed items
                foreach (Sushi.Mediakiwi.Data.AvailableTemplate availableTemplate in m_AvailableTemplateList)
                {
                    if (availableTemplate.IsSecundary != IsSecundary)
                        continue;

                    if (availableTemplate.Target != target)
                        continue;

                    bool found = false;
                    foreach (var item in items)
                    {
                        string[] split = item.Split('.');
                        if (split.Length == 2)
                        {
                            int availableTemplateId = Convert.ToInt32(split[0]);
                            if (availableTemplateId == availableTemplate.ID)
                            {
                                found = true;
                                break;
                            }
                        }
                    }
                    //  Cleanup
                    if (!found) 
                        availableTemplate.Delete();
                }

                //  Scan for new items
                int sortOrder = 0;
                foreach (var item in items)
                {
                    sortOrder++;
                    string[] split = item.Split('.');
                    if (split.Length == 1)
                    {
                        if (string.IsNullOrEmpty(split[0]))
                            continue;

                        int newComponentTemplateId = Convert.ToInt32(split[0]);

                        Sushi.Mediakiwi.Data.AvailableTemplate implement = new Sushi.Mediakiwi.Data.AvailableTemplate();
                        implement.ComponentTemplateID = newComponentTemplateId;
                        implement.PageTemplateID = m_Instance.ID;
                        implement.IsSecundary = IsSecundary;
                        implement.SortOrder = sortOrder;
                        implement.IsPossible = true;
                        implement.Target = target;
                        implement.Save();
                    }
                    else
                    {
                        if (split.Length == 2)
                        {
                            int availableTemplateId = Convert.ToInt32(split[0]);
                            foreach (Sushi.Mediakiwi.Data.AvailableTemplate availableTemplate in m_AvailableTemplateList)
                            {
                                if (availableTemplateId == availableTemplate.ID)
                                {
                                    availableTemplate.SortOrder = sortOrder;
                                    availableTemplate.Save();
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        void PageTemplate_ListSave(object sender, ComponentListEventArgs e)
        {
            m_Instance.Name = m_Name;

            if (IS_HEADLESS)
                this.IsSourceBased = true;

            m_Instance.OutputCacheDuration = this.OutputCacheDuration;
            m_Instance.IsAddedOutputCache = m_UsesOutputCache;
            m_Instance.HasSecundaryContentContainer = m_HasSecundary;
            m_Instance.OnlyOneInstancePossible = m_OnlyOneInstancePossible;
            m_Instance.SiteID = null;
            m_Instance.IsSourceBased = this.IsSourceBased;
            m_Instance.Location = m_Location;
            m_Instance.Source = this.Source;

            m_Instance.ReferenceID = ReferenceID;
            m_Instance.Description = this.Description;
            m_Instance.HasCustomDate = this.HasCustomDate;

            if (!string.IsNullOrEmpty(m_Site)) 
                m_Instance.SiteID = Convert.ToInt32(m_Site);

            m_Instance.OverwriteTemplateKey = OverwriteTemplateKey;
            m_Instance.OverwriteSiteKey = OverwriteSiteKey;

            m_Instance.Save();

            if (e.SelectedKey == 0)
                m_AvailableTemplateList = Sushi.Mediakiwi.Data.AvailableTemplate.SelectAll(m_Instance.ID);

            if (!m_Instance.IsSourceBased)
            {
                if (m_AvailableComponent != null)
                    SetAvailableTemplates(m_AvailableComponent.GetStringID(), false, null);

                if (m_AvailableServiceComponent != null)
                    SetAvailableTemplates(m_AvailableServiceComponent.GetStringID(), true, null);

                if (m_Instance.Data.HasProperty("TAB.INFO"))
                {
                    var legacyContentTab = m_Instance.Data[string.Format("TAB.LCT")].Value;
                    var legacyServiceTab = m_Instance.Data[string.Format("TAB.LST")].Value;

                    var sections = m_Instance.Data["TAB.INFO"].Value.Split(',');
                    foreach (var section in sections)
                    {
                        if (!string.IsNullOrEmpty(legacyContentTab) && section == legacyContentTab)
                            continue;

                        if (!string.IsNullOrEmpty(legacyServiceTab) && section == legacyServiceTab)
                            continue;

                        var result = wim.Form.GetValue(this, string.Format("T_{0}", section)).ParseSubList();

                        SetAvailableTemplates(result.GetStringID(), false, section);
                    }
                }
                if (e.SelectedKey > 0)
                {
                    Sushi.Mediakiwi.Data.EnvironmentVersionLogic.Flush();
                    m_Instance.CheckComponentTemplates();
                }
            }
            else
            {
                //  HTML
                var dt = new Sushi.Mediakiwi.Data.DataTemplate();
                dt.ParseSourceData(this.m_Instance);
            }
           
        }

        Sushi.Mediakiwi.Data.PageTemplate m_Instance;
        Sushi.Mediakiwi.Data.IAvailableTemplate[] m_AvailableTemplateList;


        void SetSourceVisibility()
        {
            //return;
            //wim.SetPropertyRequired("Location", !this.IsSourceBased);
            //wim.SetPropertyVisibility("AvailableComponent", !this.IsSourceBased);
            //wim.SetPropertyVisibility("HasSecundary", !this.IsSourceBased);
            //wim.SetPropertyVisibility("AvailableServiceComponent", !this.IsSourceBased);
            //wim.SetPropertyVisibility("Source", this.IsSourceBased);
        }

        void PageTemplate_ListLoad(object sender, ComponentListEventArgs e)
        {

            var parseSource = Sushi.Mediakiwi.Data.Environment.Current["PARSE_SOURCE", true, "0", "When True, the source is updated when the changes"] == "1";
            if (parseSource || !wim.CurrentApplicationUser.IsDeveloper)
            {
                wim.SetPropertyEditable("Source", false);
            }

            var dt = new Sushi.Mediakiwi.Data.DataTemplate();
            m_Instance = Sushi.Mediakiwi.Data.PageTemplate.SelectOne(e.SelectedKey);
            if (e.SelectedKey == 0)
            {
                m_Instance.IsAddedOutputCache = true;
                return;
            }
            else
            {
                dt.ParseSourceData(this.m_Instance);
            }

            if (m_Instance.PageInstanceCount == 0)
                this.ListDelete += new ComponentListEventHandler(PageTemplate_ListDelete);

            m_AvailableComponent = new Sushi.Mediakiwi.Data.SubList();
            m_AvailableServiceComponent = new Sushi.Mediakiwi.Data.SubList();
            m_AvailableTemplateList = Sushi.Mediakiwi.Data.AvailableTemplate.SelectAll(m_Instance.ID);
            m_HasCustomDate = m_Instance.HasCustomDate;
            IsSourceBased = m_Instance.IsSourceBased;

            foreach (Sushi.Mediakiwi.Data.AvailableTemplate availableTemplate in m_AvailableTemplateList)
            {
                string name = string.Format("{0} / target: {2}{1}", availableTemplate.ComponentTemplate, string.IsNullOrEmpty(availableTemplate.FixedFieldName) ? string.Empty : " (fixed on page)", availableTemplate.Target);

                if (availableTemplate.IsSecundary)
                    m_AvailableServiceComponent.Add(new Sushi.Mediakiwi.Data.SubList.SubListitem(string.Concat(availableTemplate.ID, ".", availableTemplate.ComponentTemplateID), name));
                else if (string.IsNullOrEmpty(availableTemplate.Target))
                    m_AvailableComponent.Add(new Sushi.Mediakiwi.Data.SubList.SubListitem(string.Concat(availableTemplate.ID, ".", availableTemplate.ComponentTemplateID), name));
            }

            m_Name = m_Instance.Name;
            m_Instance.OutputCacheDuration = m_Instance.OutputCacheDuration;
            m_UsesOutputCache = m_Instance.IsAddedOutputCache;
            this.OutputCacheDuration = m_Instance.OutputCacheDuration;

            m_Location = m_Instance.Location;
            m_HasSecundary = m_Instance.HasSecundaryContentContainer;
            m_OnlyOneInstancePossible = m_Instance.OnlyOneInstancePossible;
            m_Site = (m_Instance.SiteID.HasValue ? m_Instance.SiteID.ToString() : "");
            OverwriteSiteKey = m_Instance.OverwriteSiteKey;
            OverwriteTemplateKey = m_Instance.OverwriteTemplateKey;

            ReferenceID = m_Instance.ReferenceID;
            m_Description = m_Instance.Description;

            this.Source = dt.CompleteComponentTemplates(m_Instance.Source);
            this.Source2 = m_Instance.Source;


            SetSourceVisibility();

            if (m_Location != "#")
            {
                if (m_Instance.Data.HasProperty("TAB.INFO"))
                {
                    var legacyContentTab = m_Instance.Data[string.Format("TAB.LCT")].Value;
                    var legacyServiceTab = m_Instance.Data[string.Format("TAB.LST")].Value;

                    var sections = m_Instance.Data["TAB.INFO"].Value.Split(',');

                    hasNewContainers = true;
                    SubText3 = "Additional containers";
                    if (string.IsNullOrEmpty(legacyContentTab) && string.IsNullOrEmpty(legacyServiceTab))
                        SubText3 = "Component containers";

                    foreach (var section in sections)
                    {
                        if (!string.IsNullOrEmpty(legacyContentTab) && section == legacyContentTab)
                        {
                            hasLegacy1 = true;
                            continue;
                        }

                        if (!string.IsNullOrEmpty(legacyServiceTab) && section == legacyServiceTab)
                        {
                            hasLegacy2 = true;
                            continue;
                        }

                        var title = m_Instance.Data[string.Format("T[{0}]", section)].Value;

                        var selection = (from item in m_AvailableTemplateList where item.Target == section select item).ToArray();
                        Sushi.Mediakiwi.Data.SubList candidate = new Sushi.Mediakiwi.Data.SubList();

                        foreach (var item in selection)
                        {
                            string name = string.Format("{0}{1}", item.ComponentTemplate, string.IsNullOrEmpty(item.FixedFieldName) ? string.Empty : " (fixed on page)");
                            candidate.Add(new Sushi.Mediakiwi.Data.SubList.SubListitem(string.Concat(item.ID, ".", item.ComponentTemplateID), name));
                        }

                        wim.Form.AddElement(this, string.Format("T_{0}", section), true, true, new Sushi.Mediakiwi.Framework.ContentInfoItem.SubListSelectAttribute(title, "a0d67c61-0fee-4f96-86e7-cdd53d89dc43", false)
                            , candidate, true);

                    }

                    if (!hasLegacy1)
                    {
                        var doubleCheck = (from item in m_AvailableTemplateList where item.Target == null && item.IsSecundary == false select item).Count();
                        if (doubleCheck > 0)
                            hasLegacy1 = true;
                    }
                    if (!hasLegacy2)
                    {
                        var doubleCheck = (from item in m_AvailableTemplateList where item.Target == null && item.IsSecundary == true select item).Count();
                        if (doubleCheck > 0)
                            hasLegacy2 = true;
                    }
                }
                else
                {
                    hasLegacy1 = true;
                    hasLegacy2 = true;
                }
            }
            hasLegacy1 = !Wim.CommonConfiguration.HEADLESS_CMS;
            hasLegacy2 = !Wim.CommonConfiguration.HEADLESS_CMS;

            wim.AddTab("Page list", Sushi.Mediakiwi.Data.ComponentList.SelectOne(Sushi.Mediakiwi.Data.ComponentListType.PageList));
        }

        public bool hasLegacy1 { get; set; }
        public bool hasLegacy2 { get; set; }
        public bool hasNewContainers { get; set; }

        public bool IS_HEADLESS { get; set; }

        void PageTemplate_ListSearch(object sender, ComponentListSearchEventArgs e)
        {
            wim.CanAddNewItem = true;

            if (Wim.CommonConfiguration.HEADLESS_CMS)
            {
                wim.ListDataColumns.Add("ID", "ID", ListDataColumnType.UniqueIdentifier);
                //wim.ListDataColumns.Add("Ref", "ReferenceID", 25);
                wim.ListDataColumns.Add("Template", "Name", ListDataColumnType.HighlightPresent);
                wim.ListDataColumns.Add("Description", "Description");
                wim.ListDataColumns.Add("Pages", "PageInstanceCount", 40);
                wim.ListDataColumns.Add("Single", "OnlyOneInstancePossible", 30);
                //wim.ListDataColumns.Add("Cachable", "IsAddedOutputCache", 80);
                //wim.ListDataColumns.Add("Has secundary", "HasSecundaryContentContainer", ListDataColumnType.ExportOnly);
                //wim.ListDataColumns.Add("Has custom date", "HasCustomDate", ListDataColumnType.ExportOnly);
                //wim.ListDataColumns.Add("Components #1", "ComponentListInfoText1", ListDataColumnType.ExportOnly);
                //wim.ListDataColumns.Add("Components #2", "ComponentListInfoText2", ListDataColumnType.ExportOnly);
            }
            else
            {
                wim.ListDataColumns.Add("ID", "ID", ListDataColumnType.UniqueIdentifier);
                wim.ListDataColumns.Add("Ref", "ReferenceID", 25);
                wim.ListDataColumns.Add("Template", "Name", ListDataColumnType.HighlightPresent);
                wim.ListDataColumns.Add("Description", "Description", ListDataColumnType.ExportOnly);
                wim.ListDataColumns.Add("Pages", "PageInstanceCount", 40);
                wim.ListDataColumns.Add("One instance", "OnlyOneInstancePossible", 80);
                wim.ListDataColumns.Add("Cachable", "IsAddedOutputCache", 80);
                wim.ListDataColumns.Add("Has secundary", "HasSecundaryContentContainer", ListDataColumnType.ExportOnly);
                wim.ListDataColumns.Add("Has custom date", "HasCustomDate", ListDataColumnType.ExportOnly);
                wim.ListDataColumns.Add("Components #1", "ComponentListInfoText1", ListDataColumnType.ExportOnly);
                wim.ListDataColumns.Add("Components #2", "ComponentListInfoText2", ListDataColumnType.ExportOnly);
            }
            wim.ForceLoad = true;

            wim.ListData = Sushi.Mediakiwi.Data.PageTemplate.SearchAll(FilterSite, FilterText); 
        }

        private string m_FilterText;
        /// <summary>
        /// Gets or sets the filter client ID.
        /// </summary>
        /// <value>The filter client ID.</value>
        [Sushi.Mediakiwi.Framework.ContentListSearchItem.TextField("Search for", 50, AutoPostBack = true, Expression = Sushi.Mediakiwi.Framework.OutputExpression.Alternating)]
        public string FilterText
        {
            get { return m_FilterText; }
            set { m_FilterText = value; }
        }

        private int m_FilterSite;
        /// <summary>
        /// Gets or sets the search template site.
        /// </summary>
        /// <value>The search template site.</value>
        [Sushi.Mediakiwi.Framework.ContentListSearchItem.Choice_Dropdown("Part of site", "SearchSites", true, true, Expression = Sushi.Mediakiwi.Framework.OutputExpression.Alternating)]
        public int FilterSite
        {
            get { return m_FilterSite; }
            set { m_FilterSite = value; }
        }

        private ListItemCollection m_SearchSites;
        /// <summary>
        /// Gets the search sites.
        /// </summary>
        /// <value>The search sites.</value>
        public ListItemCollection SearchSites
        {
            get
            {
                if (m_SearchSites != null) return m_SearchSites;

                m_SearchSites = new ListItemCollection();
                ListItem li;
                m_SearchSites.Add(new ListItem("-- select a site --", ""));

                foreach (Sushi.Mediakiwi.Data.Site site in Sushi.Mediakiwi.Data.Site.SelectAll())
                {
                    if (site.MasterID.GetValueOrDefault() > 0) continue;
                    li = new ListItem(site.Name, site.ID.ToString());
                    m_SearchSites.Add(li);
                }
                return m_SearchSites;
            }
        }

        #region List attributes

        private string m_Name;
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        [Sushi.Mediakiwi.Framework.ContentListItem.TextField("Title", 50, true, Expression = Sushi.Mediakiwi.Framework.OutputExpression.Alternating)]
        public string Name
        {
            get { return m_Name; }
            set { m_Name = value; }
        }


        [Sushi.Mediakiwi.Framework.OnlyVisibleWhenFalse("IS_HEADLESS")]
        [Sushi.Mediakiwi.Framework.ContentListItem.Choice_Checkbox("Editable source", true, Expression = Sushi.Mediakiwi.Framework.OutputExpression.Alternating)]
        public bool IsSourceBased { get; set; }


        private string m_Location;
        /// <summary>
        /// Gets or sets the location.
        /// </summary>
        /// <value>The location.</value>
        [Sushi.Mediakiwi.Framework.ContentListItem.TextField("Location", 250, false, Expression = Sushi.Mediakiwi.Framework.OutputExpression.Alternating)]
        public string Location
        {
            get { return m_Location; }
            set { m_Location = value; }
        }

        [Sushi.Mediakiwi.Framework.OnlyVisibleWhenFalse("IS_HEADLESS")]
        [Sushi.Mediakiwi.Framework.ContentListItem.TextArea("Code", 0, IsSourceCode = true)]
        public string Source { get; set; }

        //[Sushi.Mediakiwi.Framework.ContentListItem.TextArea("Code2", 0, IsSourceCode = true)]
        public string Source2 { get; set; }


        [Sushi.Mediakiwi.Framework.OnlyVisibleWhenFalse("IS_HEADLESS")]
        [Sushi.Mediakiwi.Framework.ContentListItem.Section()]
        public string SubText0 { get { return "Properties"; } set { } }


        /// <summary>
        /// Gets or sets the reference id.
        /// </summary>
        /// <value>The reference id.</value>
        [Sushi.Mediakiwi.Framework.OnlyVisibleWhenFalse("IS_HEADLESS")]
        [Sushi.Mediakiwi.Framework.ContentListItem.TextField("Reference", 5, false, false, null, Expression = Sushi.Mediakiwi.Framework.OutputExpression.Alternating)]
        public string ReferenceID { get; set; }



        private string m_Description;
        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        [Sushi.Mediakiwi.Framework.ContentListItem.TextField("Description", 500, false, Expression = Sushi.Mediakiwi.Framework.OutputExpression.Alternating)]
        public string Description
        {
            get { return m_Description; }
            set { m_Description = value; }
        }


        private bool m_HasCustomDate;
        /// <summary>
        /// Gets or sets a value indicating whether [only one instance possible].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [only one instance possible]; otherwise, <c>false</c>.
        /// </value>
        [Sushi.Mediakiwi.Framework.ContentListItem.Choice_Checkbox("Has custom date", "Does this template require a custom (sortable) date?", Expression = Sushi.Mediakiwi.Framework.OutputExpression.Alternating)]
        public bool HasCustomDate
        {
            get { return m_HasCustomDate; }
            set { m_HasCustomDate = value; }
        }



        private string m_Site = "A";
        /// <summary>
        /// Gets or sets the site id.
        /// </summary>
        /// <value>The site id.</value>
        [Sushi.Mediakiwi.Framework.ContentListItem.Choice_Dropdown("Channel", "Sites", false, false, "When a master site with children is chosen this also applies for a child sites.", Expression = Sushi.Mediakiwi.Framework.OutputExpression.Alternating)]
        public string SiteId
        {
            get { return m_Site; }
            set { m_Site = value; }
        }

        private bool m_OnlyOneInstancePossible = true;
        /// <summary>
        /// Gets or sets a value indicating whether [only one instance possible].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [only one instance possible]; otherwise, <c>false</c>.
        /// </value>
        [Sushi.Mediakiwi.Framework.ContentListItem.Choice_Checkbox("Single use", "Can only be used one time for a page instance.", Expression = Sushi.Mediakiwi.Framework.OutputExpression.Alternating)]
        public bool OnlyOneInstancePossible
        {
            get { return m_OnlyOneInstancePossible; }
            set { m_OnlyOneInstancePossible = value; }
        }

        private ListItemCollection m_Sites;
        /// <summary>
        /// Gets the sites.
        /// </summary>
        /// <value>The sites.</value>
        public ListItemCollection Sites
        {
            get
            {
                if (m_Sites != null) return m_Sites;

                m_Sites = new ListItemCollection();
                ListItem li;
                m_Sites.Add(new ListItem("Available for all channels", ""));
                foreach (Sushi.Mediakiwi.Data.Site site in Sushi.Mediakiwi.Data.Site.SelectAll())
                {
                    if (site.MasterID.HasValue) continue;
                    li = new ListItem(site.Name, site.ID.ToString());
                    m_Sites.Add(li);
                }
                return m_Sites;
            }
        }


        private ListItemCollection m_AllSites;
        /// <summary>
        /// Gets All the sites.
        /// </summary>
        /// <value>The all sites.</value>
        public ListItemCollection AllSites
        {
            get
            {
                if (m_AllSites != null) return m_AllSites;

                m_AllSites = new ListItemCollection();
                ListItem li;
                m_AllSites.Add(new ListItem("", ""));
                foreach (Sushi.Mediakiwi.Data.Site site in Sushi.Mediakiwi.Data.Site.SelectAll())
                { 
                    li = new ListItem(site.Name, site.ID.ToString());
                    m_AllSites.Add(li);
                }
                return m_AllSites;
            }
        }

        private ListItemCollection m_AllPageTemplates;
        /// <summary>
        /// Gets the page templates.
        /// </summary>
        /// <value>The pagetemplates.</value>
        public ListItemCollection AllPageTemplates
        {
            get
            {
                if (m_AllPageTemplates != null) return m_AllPageTemplates;

                m_AllPageTemplates = new ListItemCollection();
                ListItem li;
                m_AllPageTemplates.Add(new ListItem("", ""));
                foreach (Sushi.Mediakiwi.Data.PageTemplate pt in Sushi.Mediakiwi.Data.PageTemplate.SelectAll())
                {
                    li = new ListItem(pt.Name, pt.ID.ToString());
                    m_AllPageTemplates.Add(li);
                }
                return m_AllPageTemplates;
            }
        }

      

        private bool m_UsesOutputCache = false;
        /// <summary>
        /// Gets or sets a value indicating whether [uses output cache].
        /// </summary>
        /// <value><c>true</c> if [uses output cache]; otherwise, <c>false</c>.</value>
        //[Sushi.Mediakiwi.Framework.ContentListItem.Choice_Checkbox("Use output cache", "Should pages using this templates be added to the output cache? <br/>Please note that templates having dynamic validation (forms) or dependend content (like overview pages) do not respond well to complete page caching.", Expression = Sushi.Mediakiwi.Framework.OutputExpression.Alternating)]
        public bool UsesOutputCache
        {
            get { return m_UsesOutputCache; }
            set { m_UsesOutputCache = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [uses output cache].
        /// </summary>
        [Sushi.Mediakiwi.Framework.OnlyVisibleWhenFalse("IS_HEADLESS")]
        [Sushi.Mediakiwi.Framework.ContentListItem.TextField("Cache duration", 10, InteractiveHelp ="Duration in the cache in seconds, if not applied the page is not cached.", TextType = InputType.Numeric, Expression = Sushi.Mediakiwi.Framework.OutputExpression.Right)]
        public int? OutputCacheDuration { get; set; }


        // CB 15-09-2014; options for setting the pagetemplate of a specific channel to overwrite
        [Sushi.Mediakiwi.Framework.OnlyVisibleWhenFalse("IS_HEADLESS")]
        [Sushi.Mediakiwi.Framework.ContentListItem.Section()]
        public string SEC_PageTemplateOverwrite { get { return "Overwrite options"; } }

        [Sushi.Mediakiwi.Framework.OnlyVisibleWhenFalse("IS_HEADLESS")]
        [Sushi.Mediakiwi.Framework.ContentListItem.TextLine("Info")]
        public string Info { get { return "If the seleted site and page template are requested this pagetemplate will load instead"; } }

        [Sushi.Mediakiwi.Framework.OnlyVisibleWhenFalse("IS_HEADLESS")]
        [Sushi.Mediakiwi.Framework.ContentListItem.Choice_Dropdown("Channel", "AllSites", Expression = OutputExpression.Left)]
        public int? OverwriteSiteKey { get; set; }

        [Sushi.Mediakiwi.Framework.OnlyVisibleWhenFalse("IS_HEADLESS")]
        [Sushi.Mediakiwi.Framework.ContentListItem.Choice_Dropdown("Page template", "AllPageTemplates", Expression = OutputExpression.Right)]
        public int? OverwriteTemplateKey { get; set; }

        private string m_SubText1 = "Primary container";
        /// <summary>
        /// Gets or sets the sub text1.
        /// </summary>
        /// <value>The sub text1.</value>
        [Sushi.Mediakiwi.Framework.OnlyVisibleWhenTrue("hasLegacy1")]
        [Sushi.Mediakiwi.Framework.ContentListItem.TextLine(null)]
        public string SubText1
        {
            get { return m_SubText1; }
            set { m_SubText1 = value; }
        }

        private Sushi.Mediakiwi.Data.SubList m_AvailableComponent;
        /// <summary>
        /// Gets or sets the available component.
        /// </summary>
        /// <value>The available component.</value>
        [Sushi.Mediakiwi.Framework.OnlyVisibleWhenTrue("hasLegacy1")]
        [Sushi.Mediakiwi.Framework.ContentListItem.SubListSelect("Components", "a0d67c61-0fee-4f96-86e7-cdd53d89dc43", false)]
        public Sushi.Mediakiwi.Data.SubList AvailableComponent
        {
            get { return m_AvailableComponent; }
            set { m_AvailableComponent = value; }
        }

        private string m_SubText2 = "Secundary container";
        /// <summary>
        /// Gets or sets the sub text2.
        /// </summary>
        /// <value>The sub text2.</value>
        [Sushi.Mediakiwi.Framework.OnlyVisibleWhenTrue("hasLegacy2")]
        [Sushi.Mediakiwi.Framework.ContentListItem.TextLine(null)]
        public string SubText2
        {
            get { return m_SubText2; }
            set { m_SubText2 = value; }
        }

        private bool m_HasSecundary;
        /// <summary>
        /// Gets or sets a value indicating whether this instance has secundary.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has secundary; otherwise, <c>false</c>.
        /// </value>
        [Sushi.Mediakiwi.Framework.OnlyVisibleWhenTrue("hasLegacy2")]
        [Sushi.Mediakiwi.Framework.ContentListItem.Choice_Checkbox("Is available", "Does this template have a secundary content container?")]
        public bool HasSecundary
        {
            get { return m_HasSecundary; }
            set { m_HasSecundary = value; }
        }

        private Sushi.Mediakiwi.Data.SubList m_AvailableServiceComponent;
        /// <summary>
        /// Gets or sets the available service component.
        /// </summary>
        /// <value>The available service component.</value>
        [Sushi.Mediakiwi.Framework.OnlyVisibleWhenTrue("hasLegacy2")]
        [Sushi.Mediakiwi.Framework.ContentListItem.SubListSelect("Service components", "a0d67c61-0fee-4f96-86e7-cdd53d89dc43", false)]
        public Sushi.Mediakiwi.Data.SubList AvailableServiceComponent
        {
            get { return m_AvailableServiceComponent; }
            set { m_AvailableServiceComponent = value; }
        }
        #endregion List attributes

        /// <summary>
        /// Gets or sets the sub text2.
        /// </summary>
        /// <value>The sub text2.</value>
        [Sushi.Mediakiwi.Framework.OnlyVisibleWhenTrue("hasNewContainers")]
        [Sushi.Mediakiwi.Framework.ContentListItem.TextLine(null)]
        public string SubText3 { get; set; }

    }
}
