using Sushi.Mediakiwi.Framework;
using Sushi.Mediakiwi.UI;
using System;
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
                m_SearchSites.Add(new ListItem("", ""));

                foreach (var site in Mediakiwi.Data.Site.SelectAll())
                {
                    if (site.MasterID.GetValueOrDefault() > 0)
                    {
                        continue;
                    }

                    m_SearchSites.Add(new ListItem(site.Name, $"{site.ID}"));
                }
                return m_SearchSites;
            }
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
                if (m_Sites != null)
                {
                    return m_Sites;
                }

                m_Sites = new ListItemCollection();
                m_Sites.Add(new ListItem("", ""));

                foreach (Mediakiwi.Data.Site site in Mediakiwi.Data.Site.SelectAll(false))
                {
                    m_Sites.Add(new ListItem(site.Name, $"{site.ID}"));
                }
                return m_Sites;
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

            wim.ListDataAdd(templates);
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

            Map(x => x.SiteID, Implement).Dropdown("Channel", nameof(Sites)).Expression(OutputExpression.Alternating);
            Map(x => x.NestedType, Implement).Dropdown("Nested", nameof(NestTypes)).Expression(OutputExpression.Alternating);
            Map(x => x.Description, Implement).TextField("Description", 500).Expression(OutputExpression.FullWidth);
            Map(x => x.IsFixedOnPage, Implement).Checkbox("Fixed on page", interactiveHelp: "Always present on page").Expression(OutputExpression.Alternating);
            Map(x => x.CanReplicate, Implement).Checkbox("Can replicate").Expression(OutputExpression.Alternating);
            Map(x => x.IsShared, Implement).Checkbox("Is shared").Expression(OutputExpression.Alternating);
            Map(x => x.CanDeactivate, Implement).Checkbox("Can deactivate").Expression(OutputExpression.Alternating);
            Map(x => x.IsHeader, Implement).Checkbox("Is header").Expression(OutputExpression.Alternating);
            Map(x => x.CanMoveUpDown, Implement).Checkbox("Can move").Expression(OutputExpression.Alternating);
            Map(x => x.IsFooter, Implement).Checkbox("Is footer").Expression(OutputExpression.Alternating);

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
    }
}
