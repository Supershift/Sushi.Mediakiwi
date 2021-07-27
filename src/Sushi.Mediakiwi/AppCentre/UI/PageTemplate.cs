using Sushi.Mediakiwi.Data;
using Sushi.Mediakiwi.Framework;
using Sushi.Mediakiwi.UI;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

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
            IS_HEADLESS = true;

            ListSearch += PageTemplate_ListSearch;
            ListLoad += PageTemplate_ListLoad;
            ListSave += PageTemplate_ListSave;
            ListAction += PageTemplate_ListAction;
        }


        /// <summary>
        /// Gets or sets a value indicating whether [set cacheable].
        /// </summary>
        /// <value><c>true</c> if [set cacheable]; otherwise, <c>false</c>.</value>
        //[Sushi.Mediakiwi.Framework.ContentListItem.Button("Convert components to page cache")]
        public bool SetCacheable { get; set; }

        async Task PageTemplate_ListAction(ComponentActionEventArgs e)
        {
            if (SetCacheable)
            {
                if (AvailableComponent != null)
                    foreach (var item in AvailableComponent.Items)
                    {
                        int templateID = Convert.ToInt32(item.TextID.Split('.')[1]);
                        var ct = await Mediakiwi.Data.ComponentTemplate.SelectOneAsync(templateID).ConfigureAwait(false);
                        ct.CacheLevel = 2;
                        await ct.SaveAsync().ConfigureAwait(false);
                    }
                if (AvailableServiceComponent != null)
                    foreach (var item in AvailableServiceComponent.Items)
                    {
                        int templateID = Convert.ToInt32(item.TextID.Split('.')[1]);
                        var ct = await Mediakiwi.Data.ComponentTemplate.SelectOneAsync(templateID).ConfigureAwait(false);
                        ct.CacheLevel = 2;
                        await ct.SaveAsync().ConfigureAwait(false);
                    }
                wim.FlushCache();
            }
        }

        async Task PageTemplate_ListDelete(ComponentListEventArgs e)
        {
            await m_Instance.DeleteAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Sets the available templates.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="IsSecundary">if set to <c>true</c> [is secundary].</param>
        async Task SetAvailableTemplatesAsync(string[] items, bool IsSecundary, string target)
        {
            if (items == null || items.Length == 0)
            {
                await AvailableTemplate.DeleteAsync(m_Instance.ID, IsSecundary, target).ConfigureAwait(false);
            }
            else
            {
                //  Clenup all removed items
                foreach (var availableTemplate in m_AvailableTemplateList)
                {
                    if (availableTemplate.IsSecundary != IsSecundary)
                    {
                        continue;
                    }

                    if (availableTemplate.Target != target)
                    {
                        continue;
                    }

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
                    {
                        await availableTemplate.DeleteAsync().ConfigureAwait(false);
                    }
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
                        {
                            continue;
                        }

                        int newComponentTemplateId = Convert.ToInt32(split[0]);

                        var implement = new AvailableTemplate();
                        implement.ComponentTemplateID = newComponentTemplateId;
                        implement.PageTemplateID = m_Instance.ID;
                        implement.IsSecundary = IsSecundary;
                        implement.SortOrder = sortOrder;
                        implement.IsPossible = true;
                        implement.Target = target;
                        await implement.SaveAsync().ConfigureAwait(false);
                    }
                    else
                    {
                        if (split.Length == 2)
                        {
                            int availableTemplateId = Convert.ToInt32(split[0]);
                            foreach (var availableTemplate in m_AvailableTemplateList)
                            {
                                if (availableTemplateId == availableTemplate.ID)
                                {
                                    availableTemplate.SortOrder = sortOrder;
                                    await availableTemplate.SaveAsync().ConfigureAwait(false);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        async Task PageTemplate_ListSave(ComponentListEventArgs e)
        {
            m_Instance.Name = Name;

            if (IS_HEADLESS)
            {
                IsSourceBased = true;
            }

            m_Instance.OutputCacheDuration = OutputCacheDuration;
            m_Instance.IsAddedOutputCache = UsesOutputCache;
            m_Instance.HasSecundaryContentContainer = HasSecundary;
            m_Instance.OnlyOneInstancePossible = OnlyOneInstancePossible;
            m_Instance.SiteID = null;
            m_Instance.IsSourceBased = IsSourceBased;
            m_Instance.Location = Location;
            m_Instance.Source = Source;

            //m_Instance.ReferenceID = ReferenceID;
            m_Instance.Description = Description;
            m_Instance.HasCustomDate = HasCustomDate;

            if (!string.IsNullOrEmpty(SiteId))
            {
                m_Instance.SiteID = Convert.ToInt32(SiteId);
            }

            m_Instance.OverwriteTemplateKey = OverwriteTemplateKey;
            m_Instance.OverwriteSiteKey = OverwriteSiteKey;

            await m_Instance.SaveAsync().ConfigureAwait(false);

            if (e.SelectedKey == 0)
            {
                m_AvailableTemplateList = await AvailableTemplate.SelectAllAsync(m_Instance.ID).ConfigureAwait(false);
            }

            if (!m_Instance.IsSourceBased)
            {
                if (AvailableComponent != null)
                {
                    await SetAvailableTemplatesAsync(AvailableComponent.GetStringID(), false, null).ConfigureAwait(false);
                }

                if (AvailableServiceComponent != null)
                {
                    await SetAvailableTemplatesAsync(AvailableServiceComponent.GetStringID(), true, null).ConfigureAwait(false);
                }

                if (m_Instance.Data.HasProperty("TAB.INFO"))
                {
                    var legacyContentTab = m_Instance.Data["TAB.LCT"].Value;
                    var legacyServiceTab = m_Instance.Data["TAB.LST"].Value;

                    var sections = m_Instance.Data["TAB.INFO"].Value.Split(',');
                    foreach (var section in sections)
                    {
                        if (!string.IsNullOrEmpty(legacyContentTab) && section == legacyContentTab)
                        {
                            continue;
                        }

                        if (!string.IsNullOrEmpty(legacyServiceTab) && section == legacyServiceTab)
                        {
                            continue;
                        }

                        var result = wim.Form.GetValue(this, $"T_{section}").ParseSubList();

                        await SetAvailableTemplatesAsync(result.GetStringID(), false, section).ConfigureAwait(false);
                    }
                }
                if (e.SelectedKey > 0)
                {
                    wim.FlushCache();
                    await m_Instance.CheckComponentTemplatesAsync().ConfigureAwait(false);
                }
            }
            else
            {
                if (AvailableComponent != null)
                {
                    await SetAvailableTemplatesAsync(AvailableComponent.GetStringID(), false, null).ConfigureAwait(false);
                }
                wim.FlushCacheIndex("AvailableTemplate");
            }
        }

        Mediakiwi.Data.PageTemplate m_Instance;
        IAvailableTemplate[] m_AvailableTemplateList;

        async Task PageTemplate_ListLoad(ComponentListEventArgs e)
        {

            m_Instance = await Mediakiwi.Data.PageTemplate.SelectOneAsync(e.SelectedKey).ConfigureAwait(false);
            if (e.SelectedKey == 0)
            {
                m_Instance.IsAddedOutputCache = true;
            }

            if (m_Instance.PageInstanceCount == 0)
            {
                ListDelete += PageTemplate_ListDelete;
            }

            AvailableComponent = new SubList();
            AvailableServiceComponent = new SubList();
            m_AvailableTemplateList = await AvailableTemplate.SelectAllAsync(m_Instance.ID).ConfigureAwait(false);
            HasCustomDate = m_Instance.HasCustomDate;
            IsSourceBased = m_Instance.IsSourceBased;

            foreach (var availableTemplate in m_AvailableTemplateList)
            {
                string name = availableTemplate.ComponentTemplate;

                if (availableTemplate.IsSecundary)
                {
                    AvailableServiceComponent.Add(new SubList.SubListitem($"{availableTemplate.ID}.{availableTemplate.ComponentTemplateID}", name));
                }
                else if (string.IsNullOrEmpty(availableTemplate.Target))
                {
                    AvailableComponent.Add(new SubList.SubListitem($"{availableTemplate.ID}.{availableTemplate.ComponentTemplateID}", name));
                }
            }

            Name = m_Instance.Name;
            UsesOutputCache = m_Instance.IsAddedOutputCache;
            OutputCacheDuration = m_Instance.OutputCacheDuration;

            Location = m_Instance.Location;
            HasSecundary = m_Instance.HasSecundaryContentContainer;
            OnlyOneInstancePossible = m_Instance.OnlyOneInstancePossible;
            SiteId = (m_Instance.SiteID.HasValue ? m_Instance.SiteID.ToString() : "");
            OverwriteSiteKey = m_Instance.OverwriteSiteKey;
            OverwriteTemplateKey = m_Instance.OverwriteTemplateKey;

            Description = m_Instance.Description;
            Source2 = m_Instance.Source;


            if (Location != "#")
            {
                if (m_Instance.Data.HasProperty("TAB.INFO"))
                {
                    var legacyContentTab = m_Instance.Data["TAB.LCT"].Value;
                    var legacyServiceTab = m_Instance.Data["TAB.LST"].Value;

                    var sections = m_Instance.Data["TAB.INFO"].Value.Split(',');

                    hasNewContainers = true;
                    SubText3 = "Additional containers";
                    if (string.IsNullOrEmpty(legacyContentTab) && string.IsNullOrEmpty(legacyServiceTab))
                    {
                        SubText3 = "Component containers";
                    }

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

                        var title = m_Instance.Data[$"T[{section}]"].Value;

                        var selection = (from item in m_AvailableTemplateList where item.Target == section select item).ToArray();
                        var candidate = new SubList();

                        foreach (var item in selection)
                        {
                            string name = $"{item.ComponentTemplate}{(string.IsNullOrEmpty(item.FixedFieldName) ? string.Empty : " (fixed on page)")}";
                            candidate.Add(new SubList.SubListitem($"{item.ID}.{item.ComponentTemplateID}", name));
                        }

                        wim.Form.AddElement(this, $"T_{section}", true, true, new Framework.ContentInfoItem.SubListSelectAttribute(title, "a0d67c61-0fee-4f96-86e7-cdd53d89dc43", false)
                            , candidate, true);

                    }

                    if (!hasLegacy1)
                    {
                        var doubleCheck = (from item in m_AvailableTemplateList where item.Target == null && item.IsSecundary == false select item).Count();
                        if (doubleCheck > 0)
                        {
                            hasLegacy1 = true;
                        }
                    }
                    if (!hasLegacy2)
                    {
                        var doubleCheck = (from item in m_AvailableTemplateList where item.Target == null && item.IsSecundary == true select item).Count();
                        if (doubleCheck > 0)
                        {
                            hasLegacy2 = true;
                        }
                    }
                }
                else
                {
                    hasLegacy1 = true;
                    hasLegacy2 = true;
                }
            }
            hasLegacy1 = false;
            hasLegacy2 = false;
        }

        public bool hasLegacy1 { get; set; }
        public bool hasLegacy2 { get; set; }
        public bool hasNewContainers { get; set; }

        public bool IS_HEADLESS { get; set; }

        async Task PageTemplate_ListSearch(ComponentListSearchEventArgs e)
        {
            wim.CanAddNewItem = true;

            wim.ListDataColumns.Add(new ListDataColumn("ID", nameof(Mediakiwi.Data.PageTemplate.ID), ListDataColumnType.UniqueIdentifier));
            wim.ListDataColumns.Add(new ListDataColumn("Template", nameof(Mediakiwi.Data.PageTemplate.Name), ListDataColumnType.HighlightPresent));
            wim.ListDataColumns.Add(new ListDataColumn("Description", nameof(Mediakiwi.Data.PageTemplate.Description)));
            wim.ListDataColumns.Add(new ListDataColumn("Pages", nameof(Mediakiwi.Data.PageTemplate.PageInstanceCount)) { ColumnWidth = 40 });
            wim.ListDataColumns.Add(new ListDataColumn("Single", nameof(Mediakiwi.Data.PageTemplate.OnlyOneInstancePossible)) { ColumnWidth = 30 });

            wim.ForceLoad = true;

            var list = await Mediakiwi.Data.PageTemplate.SearchAllAsync(FilterSite, FilterText).ConfigureAwait(false);
            wim.ListDataAdd(list);
        }

        /// <summary>
        /// Gets or sets the filter client ID.
        /// </summary>
        /// <value>The filter client ID.</value>
        [Framework.ContentListSearchItem.TextField("Search for", 50, AutoPostBack = true, Expression = OutputExpression.Alternating)]
        public string FilterText { get; set; }

        /// <summary>
        /// Gets or sets the search template site.
        /// </summary>
        /// <value>The search template site.</value>
        [Framework.ContentListSearchItem.Choice_Dropdown("Part of site", nameof(SearchSites), true, true, Expression = OutputExpression.Alternating)]
        public int FilterSite { get; set; }

        private ListItemCollection m_SearchSites;
        /// <summary>
        /// Gets the search sites.
        /// </summary>
        /// <value>The search sites.</value>
        public ListItemCollection SearchSites
        {
            get
            {
                if (m_SearchSites == null)
                {
                    m_SearchSites = new ListItemCollection();
                    m_SearchSites.Add(new ListItem("-- select a site --", ""));

                    foreach (var site in Mediakiwi.Data.Site.SelectAll())
                    {
                        if (site.MasterID.GetValueOrDefault() > 0)
                        {
                            continue;
                        }
                        m_SearchSites.Add(new ListItem(site.Name, $"{site.ID}"));
                    }
                }
                return m_SearchSites;
            }
        }

        #region List attributes

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        [Framework.ContentListItem.TextField("Title", 50, true, Expression = OutputExpression.Alternating)]
        public string Name { get; set; }


        [OnlyVisibleWhenFalse(nameof(IS_HEADLESS))]
        [Framework.ContentListItem.Choice_Checkbox("Editable source", true, Expression = OutputExpression.Alternating)]
        public bool IsSourceBased { get; set; }

        /// <summary>
        /// Gets or sets the location.
        /// </summary>
        /// <value>The location.</value>
        [Framework.ContentListItem.TextField("Location", 250, false, Expression = OutputExpression.Alternating)]
        public string Location { get; set; }

        [OnlyVisibleWhenFalse(nameof(IS_HEADLESS))]
        [Framework.ContentListItem.TextArea("Code", 0, IsSourceCode = true)]
        public string Source { get; set; }

        //[Sushi.Mediakiwi.Framework.ContentListItem.TextArea("Code2", 0, IsSourceCode = true)]
        public string Source2 { get; set; }

        [OnlyVisibleWhenFalse(nameof(IS_HEADLESS))]
        [Framework.ContentListItem.Section()]
        public string SubText0 { get { return "Properties"; } set { } }

        /// <summary>
        /// Gets or sets the reference id.
        /// </summary>
        /// <value>The reference id.</value>
        [OnlyVisibleWhenFalse(nameof(IS_HEADLESS))]
        [Framework.ContentListItem.TextField("Reference", 5, false, false, null, Expression = OutputExpression.Alternating)]
        public string ReferenceID { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        [Framework.ContentListItem.TextField("Description", 500, false, Expression = OutputExpression.Alternating)]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [only one instance possible].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [only one instance possible]; otherwise, <c>false</c>.
        /// </value>
        [Framework.ContentListItem.Choice_Checkbox("Has custom date", "Does this template require a custom (sortable) date?", Expression = OutputExpression.Alternating)]
        public bool HasCustomDate { get; set; }

        /// <summary>
        /// Gets or sets the site id.
        /// </summary>
        /// <value>The site id.</value>
        [Framework.ContentListItem.Choice_Dropdown("Channel", nameof(Sites), false, false, "When a master site with children is chosen this also applies for a child sites.", Expression = OutputExpression.Alternating)]
        public string SiteId { get; set; } = "A";

        /// <summary>
        /// Gets or sets a value indicating whether [only one instance possible].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [only one instance possible]; otherwise, <c>false</c>.
        /// </value>
        [Framework.ContentListItem.Choice_Checkbox("Single use", "Can only be used one time for a page instance.", Expression = OutputExpression.Alternating)]
        public bool OnlyOneInstancePossible { get; set; } = true;

        private ListItemCollection m_Sites;
        /// <summary>
        /// Gets the sites.
        /// </summary>
        /// <value>The sites.</value>
        public ListItemCollection Sites
        {
            get
            {
                if (m_Sites == null)
                {
                    m_Sites = new ListItemCollection();
                    m_Sites.Add(new ListItem("Available for all channels", ""));

                    foreach (var site in Mediakiwi.Data.Site.SelectAll())
                    {
                        if (site.MasterID.HasValue)
                        {
                            continue;
                        }

                        m_Sites.Add(new ListItem(site.Name, $"{site.ID}"));
                    }
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
                if (m_AllSites == null)
                {

                    m_AllSites = new ListItemCollection();
                    m_AllSites.Add(new ListItem("", ""));

                    foreach (var site in Mediakiwi.Data.Site.SelectAll())
                    {
                        m_AllSites.Add(new ListItem(site.Name, $"{site.ID}"));
                    }
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
                if (m_AllPageTemplates == null)
                {
                    m_AllPageTemplates = new ListItemCollection();
                    m_AllPageTemplates.Add(new ListItem("", ""));

                    foreach (var pt in Mediakiwi.Data.PageTemplate.SelectAll())
                    {
                        m_AllPageTemplates.Add(new ListItem(pt.Name, $"{pt.ID}"));
                    }
                }
                return m_AllPageTemplates;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [uses output cache].
        /// </summary>
        /// <value><c>true</c> if [uses output cache]; otherwise, <c>false</c>.</value>
        //[Sushi.Mediakiwi.Framework.ContentListItem.Choice_Checkbox("Use output cache", "Should pages using this templates be added to the output cache? <br/>Please note that templates having dynamic validation (forms) or dependend content (like overview pages) do not respond well to complete page caching.", Expression = OutputExpression.Alternating)]
        public bool UsesOutputCache { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [uses output cache].
        /// </summary>
        [OnlyVisibleWhenFalse(nameof(IS_HEADLESS))]
        [Framework.ContentListItem.TextField("Cache duration", 10, InteractiveHelp = "Duration in the cache in seconds, if not applied the page is not cached.", TextType = InputType.Numeric, Expression = OutputExpression.Right)]
        public int? OutputCacheDuration { get; set; }


        // CB 15-09-2014; options for setting the pagetemplate of a specific channel to overwrite
        [OnlyVisibleWhenFalse(nameof(IS_HEADLESS))]
        [Framework.ContentListItem.Section()]
        public string SEC_PageTemplateOverwrite { get { return "Overwrite options"; } }

        [OnlyVisibleWhenFalse(nameof(IS_HEADLESS))]
        [Framework.ContentListItem.TextLine("Info")]
        public string Info { get { return "If the seleted site and page template are requested this pagetemplate will load instead"; } }

        [OnlyVisibleWhenFalse(nameof(IS_HEADLESS))]
        [Framework.ContentListItem.Choice_Dropdown("Channel", nameof(AllSites), Expression = OutputExpression.Left)]
        public int? OverwriteSiteKey { get; set; }

        [OnlyVisibleWhenFalse(nameof(IS_HEADLESS))]
        [Framework.ContentListItem.Choice_Dropdown("Page template", nameof(AllPageTemplates), Expression = OutputExpression.Right)]
        public int? OverwriteTemplateKey { get; set; }

        /// <summary>
        /// Gets or sets the sub text1.
        /// </summary>
        /// <value>The sub text1.</value>
        [OnlyVisibleWhenTrue(nameof(hasLegacy1))]
        [Framework.ContentListItem.TextLine(null)]
        public string SubText1 { get; set; } = "Primary container";

        /// <summary>
        /// Gets or sets the available component.
        /// </summary>
        /// <value>The available component.</value>
        //[Sushi.Mediakiwi.Framework.OnlyVisibleWhenTrue("hasLegacy1")]
        [Framework.ContentListItem.SubListSelect("Components", "a0d67c61-0fee-4f96-86e7-cdd53d89dc43", false)]
        public SubList AvailableComponent { get; set; }

        /// <summary>
        /// Gets or sets the sub text2.
        /// </summary>
        /// <value>The sub text2.</value>
        [OnlyVisibleWhenTrue(nameof(hasLegacy2))]
        [Framework.ContentListItem.TextLine(null)]
        public string SubText2 { get; set; } = "Secundary container";

        /// <summary>
        /// Gets or sets a value indicating whether this instance has secundary.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has secundary; otherwise, <c>false</c>.
        /// </value>
        [OnlyVisibleWhenTrue(nameof(hasLegacy2))]
        [Framework.ContentListItem.Choice_Checkbox("Is available", "Does this template have a secundary content container?")]
        public bool HasSecundary { get; set; }

        /// <summary>
        /// Gets or sets the available service component.
        /// </summary>
        /// <value>The available service component.</value>
        [OnlyVisibleWhenTrue(nameof(hasLegacy2))]
        [Framework.ContentListItem.SubListSelect("Service components", "a0d67c61-0fee-4f96-86e7-cdd53d89dc43", false)]
        public SubList AvailableServiceComponent { get; set; }

        #endregion List attributes

        /// <summary>
        /// Gets or sets the sub text2.
        /// </summary>
        /// <value>The sub text2.</value>
        [OnlyVisibleWhenTrue(nameof(hasNewContainers))]
        [Framework.ContentListItem.TextLine(null)]
        public string SubText3 { get; set; }

    }
}
