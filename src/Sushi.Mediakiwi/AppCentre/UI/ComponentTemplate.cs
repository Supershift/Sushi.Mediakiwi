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
    /// Represents a ComponentTemplate entity.
    /// </summary>
    public class ComponentTemplate : BaseImplementation
    {
        #region Collections

        public ListItemCollection NestTypes { get; set; }


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
                m_SearchSites.Add(new ListItem("", ""));

                foreach (var site in Mediakiwi.Data.Site.SelectAll())
                {
                    if (site.MasterID.GetValueOrDefault() > 0) continue;
                    li = new ListItem(site.Name, site.ID.ToString());
                    m_SearchSites.Add(li);
                }
                return m_SearchSites;
            }
        }


        #endregion Collections

        #region CTor

        /// <summary>
        /// Initializes a new instance of the <see cref="ComponentTemplate"/> class.
        /// </summary>
        public ComponentTemplate()
        {
            wim.OpenInEditMode = true;
            //IS_HEADLESS = true;

            ListSave += ComponentTemplate_ListSave;
            ListPreRender += ComponentTemplate_ListPreRender;
            ListDelete += ComponentTemplate_ListDelete;
            ListLoad += ComponentTemplate_ListLoad;
            ListSearch += ComponentTemplate_ListSearch;
        }
        #endregion CTor

        #region List Prerender

        Task ComponentTemplate_ListPreRender(ComponentListEventArgs e)
        {
            if (IsPostBack)
            {
                SetSourceVisibility(Implement.HasEditableSource);
            }

            return Task.CompletedTask;
        }

        #endregion List Prerender

        #region Set Source Visibility

        void SetSourceVisibility(bool isHTMLSource)
        {
            wim.SetPropertyVisibility(nameof(Mediakiwi.Data.ComponentTemplate.Location), !isHTMLSource);
            wim.SetPropertyVisibility(nameof(Mediakiwi.Data.ComponentTemplate.TypeDefinition), !isHTMLSource);
            wim.SetPropertyVisibility(nameof(Mediakiwi.Data.ComponentTemplate.Source), isHTMLSource);
            wim.SetPropertyVisibility(nameof(Mediakiwi.Data.ComponentTemplate.SourceTag), isHTMLSource);
            wim.SetPropertyVisibility(nameof(Mediakiwi.Data.ComponentTemplate.ReferenceID), !isHTMLSource);

            if (isHTMLSource)
            {
                Implement.Location = "#";
                Implement.TypeDefinition = "#";
            }
            else if (Implement.Location == "#")
            {
                Implement.Location = string.Empty;
                Implement.TypeDefinition = string.Empty;
            }
        }

        #endregion Set Source Visibility

        ///// <summary>
        ///// Gets or sets a value indicating whether [set all page cache].
        ///// </summary>
        ///// <value><c>true</c> if [set all page cache]; otherwise, <c>false</c>.</value>
        //[OnlyVisibleWhenFalse("IS_HEADLESS")]
        //[Framework.ContentListSearchItem.Button("Set pagelevel cache", AskConfirmation = true, IconTarget = ButtonTarget.TopRight)]
        //public bool SetAllPageCache { get; set; }

        //public bool IS_HEADLESS { get; set; }

        #region List Search

        /// <summary>
        /// Components the template_ list search.
        /// </summary>
        /// <param name="sender">The sender.</param>
        async Task ComponentTemplate_ListSearch(ComponentListSearchEventArgs e)
        {
            wim.CanAddNewItem = true;

            wim.ListDataColumns.Add(new ListDataColumn("ID", nameof(Mediakiwi.Data.ComponentTemplate.ID), ListDataColumnType.UniqueIdentifier));
            wim.ListDataColumns.Add(new ListDataColumn("Template", nameof(Mediakiwi.Data.ComponentTemplate.Name), ListDataColumnType.HighlightPresent));
            wim.ListDataColumns.Add(new ListDataColumn("Sourcetag", nameof(Mediakiwi.Data.ComponentTemplate.SourceTag)) { ColumnWidth = 250 });
            wim.ListDataColumns.Add(new ListDataColumn("Description", nameof(Mediakiwi.Data.ComponentTemplate.Description)));
            

            var templates = await Mediakiwi.Data.ComponentTemplate.SearchAllAsync(FilterText, FilterSite).ConfigureAwait(false);
            wim.ForceLoad = true;

            //if (FilterTarget > 0 || FilterTemplate > 0)
            //{
            //    if (FilterTemplate > 0)
            //    {
            //        bool checkForService = FilterTarget == 2;
            //        IAvailableTemplate[] available = AvailableTemplate.SelectAll(FilterTemplate);

            //        templates = (from item in templates
            //                     join relation in available on item.ID equals relation.ComponentTemplateID
            //                     select item).ToArray();
            //    }
            //    if (FilterTarget > 0)
            //    {
            //        if (FilterTarget == 1)
            //            templates = (from item in templates where item.IsSecundaryContainerItem == false select item).ToArray();
            //        else if (FilterTarget == 2)
            //            templates = (from item in templates where item.IsSecundaryContainerItem select item).ToArray();
            //    }

            //}

            //if (SetAllPageCache)
            //{
            //    foreach (var item in templates)
            //    {
            //        if (item.CacheLevel == 0)
            //        {
            //            item.CacheLevel = 2;
            //            item.Save();
            //        }
            //    }
            //}

            wim.ListDataAdd(templates);
          //  return Task.CompletedTask;
        }

        #endregion List Search

        #region List Load

        /// <summary>
        /// Handles the ListLoad event of the ComponentTemplate control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Wim.Framework.ComponentListEventArgs"/> instance containing the event data.</param>
        async Task ComponentTemplate_ListLoad(ComponentListEventArgs e)
        {
            NestTypes = new ListItemCollection();
            NestTypes.Add(new ListItem("Not nested", ""));
            NestTypes.Add(new ListItem("Equal source tags", "1"));
            NestTypes.Add(new ListItem("Adjacent source tags", "2"));

            wim.CanAddNewItem = true;
            Implement = await Mediakiwi.Data.ComponentTemplate.SelectOneAsync(e.SelectedKey).ConfigureAwait(false);

            if (e.SelectedKey == 0)
            {
                Implement.HasEditableSource = true;
                Implement.IsSearchable = true;
                Implement.CacheLevel = 2;
                Implement.AjaxType = 0;
                Implement.Location = "#";
            }
            Implement.HasEditableSource = (Implement.Location == "#");
            SetSourceVisibility(Implement.HasEditableSource);
            if (e.SelectedKey > 0 && Implement.HasEditableSource)
            {
                wim.AddTab(new Guid("36fc7157-d5c7-433c-8317-b601226f9bd0"), e.SelectedKey);
            }

            Map(x => x.Name, Implement).TextField("Name", 50, true).Expression(OutputExpression.Alternating);
            Map(x => x.SourceTag, Implement).TextField("Source tag", 25).Expression(OutputExpression.Alternating);

            FormMaps.Add(this);
        }

        #endregion List Load

        /// <summary>
        /// Gets or sets the implement.
        /// </summary>
        /// <value>The implement.</value>
        [Framework.ContentListItem.DataExtend()]
        public Mediakiwi.Data.ComponentTemplate Implement { get; set; }

        #region List Delete

        /// <summary>
        /// Handles the ListDelete event of the ComponentTemplate control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Wim.Framework.ComponentListEventArgs"/> instance containing the event data.</param>
        async Task ComponentTemplate_ListDelete(ComponentListEventArgs e)
        {
            await Implement.DeleteAsync().ConfigureAwait(false);
        }

        #endregion List Delete

        #region List Save

        /// <summary>
        /// Handles the ListSave event of the ComponentTemplate control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Wim.Framework.ComponentListEventArgs"/> instance containing the event data.</param>
        async Task ComponentTemplate_ListSave(ComponentListEventArgs e)
        {
            await Implement.SaveAsync().ConfigureAwait(false);
        }

        #endregion List Save
        
        #region Search UI Elements

        /// <summary>
        /// Gets or sets the filter client ID.
        /// </summary>
        /// <value>The filter client ID.</value>
        [Framework.ContentListSearchItem.TextField("Search for", 50, AutoPostBack = true, InteractiveHelp = "Searches in the Name, Description, Location and Sourcetag", Expression = OutputExpression.Alternating)]
        public string FilterText { get; set; }

        /// <summary>
        /// Gets or sets the filter site.
        /// </summary>
        /// <value>The filter site.</value>
        [Framework.ContentListSearchItem.Choice_Dropdown("Part of site", nameof(SearchSites), true, true, Expression = OutputExpression.Alternating)]
        public int FilterSite { get; set; }
        
        #endregion Search UI Elements

        //private ListItemCollection m_Sites;
        ///// <summary>
        ///// Gets the sites.
        ///// </summary>
        ///// <value>The sites.</value>
        //public ListItemCollection Sites
        //{
        //    get
        //    {
        //        if (m_Sites != null) return m_Sites;

        //        m_Sites = new ListItemCollection();
        //        ListItem li;
        //        m_Sites.Add(new ListItem("-- select a site --", ""));

        //        foreach (var site in Mediakiwi.Data.Site.SelectAll())
        //        {
        //            if (site.MasterID.GetValueOrDefault() > 0) continue;
        //            li = new ListItem(site.Name, site.ID.ToString());
        //            m_Sites.Add(li);
        //        }

        //        return m_Sites;
        //    }
        //}

        //[OnlyVisibleWhenFalse("IS_HEADLESS")]
        //[Framework.ContentListSearchItem.Choice_Dropdown("Page template", "PageTemplates", true, true, Expression = OutputExpression.Alternating)]
        //public int FilterTemplate { get; set; }



        //private ListItemCollection m_PageTemplates;
        ///// <summary>
        ///// Gets the page templates.
        ///// </summary>
        ///// <value>The page templates.</value>
        //public ListItemCollection PageTemplates
        //{
        //    get
        //    {
        //        if (m_PageTemplates != null) return m_PageTemplates;

        //        m_PageTemplates = new ListItemCollection();
        //        ListItem li;
        //        m_PageTemplates.Add(new ListItem("", ""));

        //        foreach (var template in Mediakiwi.Data.PageTemplate.SelectAll())
        //        {
        //            li = new ListItem(string.Format("{1:00}. {0}", template.Name, template.ReferenceID), template.ID.ToString());
        //            m_PageTemplates.Add(li);
        //        }

        //        return m_PageTemplates;
        //    }
        //}

        //[OnlyVisibleWhenFalse("IS_HEADLESS")]
        //[Framework.ContentListSearchItem.Choice_Dropdown("Target", "Targets", true, true, Expression = OutputExpression.Alternating)]
        //public int FilterTarget { get; set; }

        //private ListItemCollection m_Targets;
        ///// <summary>
        ///// Gets the page templates.
        ///// </summary>
        ///// <value>The page templates.</value>
        //public ListItemCollection Targets
        //{
        //    get
        //    {
        //        if (m_Targets != null) return m_Targets;

        //        m_Targets = new ListItemCollection();
        //        m_Targets.Add(new ListItem("", ""));
        //        m_Targets.Add(new ListItem("Main content", "1"));
        //        m_Targets.Add(new ListItem("Service column", "2"));

        //        return m_Targets;
        //    }
        //}

        //private ListItemCollection m_CacheTypes;
        ///// <summary>
        ///// Gets the cache types.
        ///// </summary>
        ///// <value>The cache types.</value>
        //public ListItemCollection CacheTypes
        //{
        //    get
        //    {
        //        if (m_CacheTypes != null) return m_CacheTypes;

        //        m_CacheTypes = new ListItemCollection();
        //        ListItem li;
        //        m_CacheTypes.Add(new ListItem("No cache", "0"));
        //        m_CacheTypes.Add(new ListItem("Component based", "1"));
        //        m_CacheTypes.Add(new ListItem("Page based", "2"));
        //        return m_CacheTypes;
        //    }
        //}


        //private ListItemCollection m_AjaxTypes;
        ///// <summary>
        ///// Gets the cache types.
        ///// </summary>
        ///// <value>The cache types.</value>
        //public ListItemCollection AjaxTypes
        //{
        //    get
        //    {
        //        if (m_AjaxTypes != null) return m_AjaxTypes;

        //        m_AjaxTypes = new ListItemCollection();
        //        m_AjaxTypes.Add(new ListItem("Fully cacheable", "0"));
        //        m_AjaxTypes.Add(new ListItem("Visitor based", "1"));
        //        m_AjaxTypes.Add(new ListItem("Update via ajax", "2"));
        //        m_AjaxTypes.Add(new ListItem("Visitor based & update via ajax", "3"));
        //        return m_AjaxTypes;
        //    }
        //}
    }
}
