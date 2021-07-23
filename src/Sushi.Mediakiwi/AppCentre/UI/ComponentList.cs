using Sushi.Mediakiwi.AppCentre.UI;
using Sushi.Mediakiwi.Data;
using Sushi.Mediakiwi.Framework;
using Sushi.Mediakiwi.UI;
using System;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.AppCentre.Data.Implementation
{
    /// <summary>
    /// Represents a ComponentList entity.
    /// </summary>
    public class ComponentList : BaseImplementation
    {
        /// <summary>
        /// Gets or sets the filter client ID.
        /// </summary>
        /// <value>The filter client ID.</value>
        [Framework.ContentListSearchItem.TextField("Search for", 50, AutoPostBack = false, Expression = OutputExpression.Alternating)]
        public string FilterText { get; set; }

        /// <summary>
        /// Gets or sets the search template site.
        /// </summary>
        /// <value>The search template site.</value>
        [Framework.ContentListSearchItem.Choice_Dropdown("Channel", "SearchSites", false, false, Expression = OutputExpression.Alternating)]
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

                    foreach (Mediakiwi.Data.Site site in Mediakiwi.Data.Site.SelectAll())
                    {
                        if (site.MasterID.GetValueOrDefault() > 0)
                        {
                            continue;
                        }
                        m_SearchSites.Add(new ListItem(site.Name, $"{site}"));
                    }
                }
                return m_SearchSites;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ComponentList"/> class.
        /// </summary>
        public ComponentList()
        {
            wim.OpenInEditMode = true;

            ListSearch += ComponentList_ListSearch;
            ListSave += ComponentList_ListSave;
            ListDelete += ComponentList_ListDelete;
            ListLoad += ComponentList_ListLoad;
            ListPreRender += ComponentList_ListPreRender;
        }

        async Task ComponentList_ListPreRender(ComponentListEventArgs e)
        {
            if (Implement.SiteID.HasValue)
            {
                wim.CurrentSite = await Mediakiwi.Data.Site.SelectOneAsync(Implement.SiteID.Value).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is configuration_ template.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is configuration_ template; otherwise, <c>false</c>.
        /// </value>
        public bool IsConfiguration_Template { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is configuration_ data.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is configuration_ data; otherwise, <c>false</c>.
        /// </value>
        public bool IsConfiguration_Data { get; set; }

        /// <summary>
        /// Gets or sets the implement.
        /// </summary>
        /// <value>The implement.</value>
        public IComponentList Implement { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is wim type.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is wim type; otherwise, <c>false</c>.
        /// </value>
        public bool IsWimType { get; set; }

        /// <summary>
        /// Handles the ListLoad event of the ComponentList control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ComponentListEventArgs"/> instance containing the event data.</param>
        async Task ComponentList_ListLoad(ComponentListEventArgs e)
        {
            if (wim.CurrentList.Type == ComponentListType.ComponentLists || wim.CurrentList.Type == ComponentListType.ComponentListProperties)
            {
                IsConfiguration_Template = true;
            }
            else if (wim.CurrentList.Type == ComponentListType.ComponentListData)
            {
                IsConfiguration_Data = true;
            }

            bool isListPropertySection = (wim.CurrentList.Type == ComponentListType.ComponentListProperties);

            int selectedKey = e.SelectedGroupItemKey > 0 ? e.SelectedGroupItemKey : e.SelectedKey;

            wim.CanSaveAndAddNew = true;
            wim.CanAddNewItem = true;

            Implement = await Mediakiwi.Data.ComponentList.SelectOneAsync(selectedKey).ConfigureAwait(false);

            IsWimType = Implement.Type > 0;

            if (selectedKey == 0)
            {
                Implement.AssemblyName = "Sushi.Mediakiwi.Framework.dll";
                Implement.ClassName = "Wim.Templates.UI.GenericLinqList";
                Implement.IsVisible = true;
                Implement.IsInherited = true;

                int folder = Utility.ConvertToInt(Request.Query["folder"]);
                if (folder > 0)
                {
                    Implement.SiteID = wim.CurrentSite.ID;
                    Implement.FolderID = folder;
                }
            }


            if (e.SelectedKey > 0 && (wim.CurrentList.Type == ComponentListType.ComponentLists || wim.CurrentList.Type == ComponentListType.ComponentListProperties))
            {
                //  Settings
                var settingsList = await Mediakiwi.Data.ComponentList.SelectOneAsync(new Guid("15C30414-F187-4021-B991-386653677767")).ConfigureAwait(false);
                if (settingsList?.ID > 0)
                {
                    wim.AddTab(settingsList, e.SelectedKey);
                }

                if (isListPropertySection)
                {
                    if (string.IsNullOrEmpty(Implement.ClassName)) return;
                    if (Implement.Type == ComponentListType.Undefined && !Implement.ClassName.StartsWith("Wim.Module.", StringComparison.OrdinalIgnoreCase))
                    {
                        //  Fields
                        IComponentList fieldList = await Mediakiwi.Data.ComponentList.SelectOneAsync(new Guid("6a7d5e6c-9daa-4a6f-aeea-21bd4782da1e")).ConfigureAwait(false);
                        if (fieldList?.ID > 0)
                        {
                            wim.AddTab(fieldList, 0);
                        }

                        //  Columns
                        var columnsList = await Mediakiwi.Data.ComponentList.SelectOneAsync(new Guid("c7b2c21d-0121-4e58-a0c9-616b31c2e1b1")).ConfigureAwait(false);
                        if (columnsList?.ID > 0)
                        {
                            wim.AddTab(columnsList, 0);
                        }
                    }
                }

                // Versions
                var versionsList = await Mediakiwi.Data.ComponentList.SelectOneAsync(new Guid("DAD14066-B9E3-4ED9-B951-BE64C93AE2D3")).ConfigureAwait(false);
                if (versionsList?.ID > 0)
                {
                    wim.AddTab(versionsList, e.SelectedKey);
                }
            }

            FormMaps.Add(new Forms.ComponentListForm(Implement));
        }
      
        public async Task<IComponentList> CreateListAsync(Guid guid)
        {
            return await Mediakiwi.Data.ComponentList.AddAsync(
                guid, typeof(ListVersioning),
                "Versioning", 
                "Version", 
                wim.CurrentFolder.ID, wim.CurrentSite.ID, false
            ).ConfigureAwait(false);
        }


        /// <summary>
        /// Handles the ListDelete event of the ComponentList control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ComponentListEventArgs"/> instance containing the event data.</param>
        async Task ComponentList_ListDelete(ComponentListEventArgs e)
        {
            await Implement.DeleteAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Handles the ListSave event of the ComponentList control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ComponentListEventArgs"/> instance containing the event data.</param>
        async Task ComponentList_ListSave(ComponentListEventArgs e)
        {
            Implement.IsTemplate = true;
            Implement.Target = ComponentListTarget.List;

            //  [12.05.13:MM] Set the correct target type when moving to the Administration section.
            if (Implement.SiteID.HasValue)
            {
                var site = await Mediakiwi.Data.Site.SelectOneAsync(Implement.SiteID.Value).ConfigureAwait(false);
                if (site.Type.HasValue)
                {
                    Implement.Target = ComponentListTarget.Administration;
                }
            }

            if (!Implement.FolderID.HasValue && Implement.SiteID.HasValue)
            {
                Mediakiwi.Data.Folder Folder = await Mediakiwi.Data.Folder.SelectOneBySiteAsync(Implement.SiteID.Value, FolderType.List).ConfigureAwait(false);

                if (!Folder.IsNewInstance)
                {
                    if (Implement.FolderID.HasValue)
                    {
                        Mediakiwi.Data.Folder Folder2 = await Mediakiwi.Data.Folder.SelectOneAsync(Implement.FolderID.Value).ConfigureAwait(false);
                        if (Folder2.SiteID != Folder.SiteID)
                        {
                            Implement.FolderID = Folder.ID;
                        }
                    }
                    else
                    {
                        Implement.FolderID = Folder.ID;
                    }
                }
            }

            Implement.SenseInterval = 0;
            Implement.SenseScheduled = null;
            await Implement.SaveAsync().ConfigureAwait(false);

            wim.FlushCache();
        }

        /// <summary>
        /// Components the list_ list search.
        /// </summary>
        /// <param name="sender">The sender.</param>
        private async Task ComponentList_ListSearch(ComponentListSearchEventArgs e)
        {
            wim.CanAddNewItem = true;
            wim.ListDataColumns.Add(new ListDataColumn("ID", nameof(Mediakiwi.Data.ComponentList.ID), ListDataColumnType.UniqueIdentifier));
            wim.ListDataColumns.Add(new ListDataColumn("Name", nameof(Mediakiwi.Data.ComponentList.Name), ListDataColumnType.HighlightPresent));
            wim.ListDataColumns.Add(new ListDataColumn("Single item name", nameof(Mediakiwi.Data.ComponentList.SingleItemName)));
            wim.ListDataColumns.Add(new ListDataColumn("Channel", nameof(Mediakiwi.Data.ComponentList.SiteName)));
            wim.ListDataColumns.Add(new ListDataColumn("Visible", nameof(Mediakiwi.Data.ComponentList.IsVisible)));
            wim.ListDataColumns.Add(new ListDataColumn("Inherited", nameof(Mediakiwi.Data.ComponentList.IsInherited)));

            wim.ForceLoad = true;

            var result = await Mediakiwi.Data.ComponentList.SelectAllAsync(FilterText, FilterSite).ConfigureAwait(false);
            wim.ListDataAdd(result);
        }
    }
}
