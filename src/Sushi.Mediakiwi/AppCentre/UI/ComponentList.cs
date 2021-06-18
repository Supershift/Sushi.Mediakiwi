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
        private string m_FilterText;
        /// <summary>
        /// Gets or sets the filter client ID.
        /// </summary>
        /// <value>The filter client ID.</value>
        [Framework.ContentListSearchItem.TextField("Search for", 50, AutoPostBack = false, Expression = OutputExpression.Alternating)]
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
        [Framework.ContentListSearchItem.Choice_Dropdown("Channel", "SearchSites", false, false, Expression = OutputExpression.Alternating)]
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

                foreach (Mediakiwi.Data.Site site in Mediakiwi.Data.Site.SelectAll())
                {
                    if (site.MasterID.GetValueOrDefault() > 0) continue;
                    li = new ListItem(site.Name, site.ID.ToString());
                    m_SearchSites.Add(li);
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
                wim.CurrentSite = await Mediakiwi.Data.Site.SelectOneAsync(Implement.SiteID.Value);
        }

        bool m_IsConfiguration_Template;
        /// <summary>
        /// Gets or sets a value indicating whether this instance is configuration_ template.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is configuration_ template; otherwise, <c>false</c>.
        /// </value>
        public bool IsConfiguration_Template
        {
            get { return m_IsConfiguration_Template; }
            set { m_IsConfiguration_Template = value; }
        }

        bool m_IsConfiguration_Sense;
        /// <summary>
        /// Gets or sets a value indicating whether this instance is configuration_ sense.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is configuration_ sense; otherwise, <c>false</c>.
        /// </value>
        public bool IsConfiguration_Sense
        {
            get { return m_IsConfiguration_Sense; }
            set { m_IsConfiguration_Sense = value; }
        }

        bool m_IsConfiguration_Data;
        /// <summary>
        /// Gets or sets a value indicating whether this instance is configuration_ data.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is configuration_ data; otherwise, <c>false</c>.
        /// </value>
        public bool IsConfiguration_Data
        {
            get { return m_IsConfiguration_Data; }
            set { m_IsConfiguration_Data = value; }
        }

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
        /// Gets a value indicating whether this instance is generics.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is generics; otherwise, <c>false</c>.
        /// </value>
        public bool IsGenerics
        {
            get { return Implement.ClassName == "Wim.Templates.Templates.UI.GenericsList"
                || Implement.ClassName == "Wim.Templates.Templates.UI.GenericList"
                || Implement.ClassName == "Wim.Templates.Templates.UI.SimpleGenericsList"
                || Implement.ClassName == "Wim.Templates.UI.GenericLinqList"
                //|| Implement.GetInstance(Context) is Wim.Templates.UI.iGenericLinqList
                ; }
        }

        public bool IsGenericClassName
        {
            get
            {
                return Implement.ClassName == "Wim.Templates.Templates.UI.GenericsList"
                    || Implement.ClassName == "Wim.Templates.Templates.UI.GenericList"
                    || Implement.ClassName == "Wim.Templates.Templates.UI.SimpleGenericsList"
                    || Implement.ClassName == "Wim.Templates.UI.GenericLinqList"
                    ;
            }
        }

        public bool OldTypeGenerics
        {
            get
            {
                return Implement.ClassName == "Wim.Templates.Templates.UI.GenericsList"
                    || Implement.ClassName == "Wim.Templates.Templates.UI.GenericList"
                    || Implement.ClassName == "Wim.Templates.Templates.UI.SimpleGenericsList"
                    ;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is generics or configuration template.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is generics or configuration template; otherwise, <c>false</c>.
        /// </value>
        public bool IsGenericsOrConfigurationTemplate
        {
            get
            {
                if (IsGenerics && IsConfiguration_Template)
                    return true;
                return false;

            }
        }

        /// <summary>
        /// Handles the ListLoad event of the ComponentList control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ComponentListEventArgs"/> instance containing the event data.</param>
        async Task ComponentList_ListLoad(ComponentListEventArgs e)
        {
            if (wim.CurrentList.Type == ComponentListType.ComponentLists || wim.CurrentList.Type == ComponentListType.ComponentListProperties)
                this.IsConfiguration_Template = true;
            else if (wim.CurrentList.Type == ComponentListType.ComponentListScheduling)
                this.IsConfiguration_Sense = true;
            else if (wim.CurrentList.Type == ComponentListType.ComponentListData)
                this.IsConfiguration_Data = true;

            bool isListPropertySection = false;
            if (wim.CurrentList.Type == ComponentListType.ComponentListProperties || wim.CurrentList.Type == ComponentListType.ComponentListScheduling)
            {
                isListPropertySection = true;
                //wim.ShowInFullWidthMode = true;
            }

            int selectedKey = e.SelectedGroupItemKey > 0 ? e.SelectedGroupItemKey : e.SelectedKey;


            wim.CanSaveAndAddNew = true;
            wim.CanAddNewItem = true;

            Implement = await Mediakiwi.Data.ComponentList.SelectOneAsync(selectedKey);

            this.IsWimType = Implement.Type > 0;

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

            //Implement.HasServiceCall = (Implement.SenseInterval > 0);

            if (e.SelectedKey > 0 && (wim.CurrentList.Type == ComponentListType.ComponentLists || wim.CurrentList.Type == ComponentListType.ComponentListProperties))
            {
                //  Settings
                wim.AddTab(await Mediakiwi.Data.ComponentList.SelectOneAsync(new Guid("15C30414-F187-4021-B991-386653677767")), e.SelectedKey);

                if (isListPropertySection)
                {
                    if (string.IsNullOrEmpty(Implement.ClassName)) return;
                    if (Implement.Type == ComponentListType.Undefined && !Implement.ClassName.StartsWith("Wim.Module.", StringComparison.OrdinalIgnoreCase))
                    {
                        //  Fields
                        IComponentList fieldList = await Mediakiwi.Data.ComponentList.SelectOneAsync(new Guid("6a7d5e6c-9daa-4a6f-aeea-21bd4782da1e"));
                        wim.AddTab(fieldList, 0);

                        //  Columns
                        wim.AddTab(await Mediakiwi.Data.ComponentList.SelectOneAsync(new Guid("c7b2c21d-0121-4e58-a0c9-616b31c2e1b1")), 0);
                        //  Schedule
                        wim.AddTab(await Mediakiwi.Data.ComponentList.SelectOneAsync(new Guid("937A7D82-EABD-4747-A74A-7B9106E697B1")), e.SelectedKey);
                    }
                }

                var list = await Mediakiwi.Data.ComponentList.SelectOneAsync(new Guid("DAD14066-B9E3-4ED9-B951-BE64C93AE2D3"));
                //  Compare
                //wim.AddTab(Sushi.Mediakiwi.Data.ComponentList.SelectOne(new Guid("3ed45f19-2b30-4fcd-9a39-7a5855f7ebca")), e.SelectedKey);
                wim.AddTab(list, e.SelectedKey);
            }
            //Implement.zz_Generics0 = "Options";
            //Implement.zz_Generics1 = "Labels";
            //Implement.zz_Generics2 = "Search grid settings";
            //Implement.zz_Generics3 = "Generics";

            FormMaps.Add(new Forms.ComponentListForm(Implement));
        }
      
        public IComponentList CreateList(Guid guid)
        {
            return Mediakiwi.Data.ComponentList.Add(
                guid, typeof(ListVersioning),
                "Versioning", 
                "Version", 
                wim.CurrentFolder.ID, wim.CurrentSite.ID, false
            );
        }


        /// <summary>
        /// Handles the ListDelete event of the ComponentList control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ComponentListEventArgs"/> instance containing the event data.</param>
        async Task ComponentList_ListDelete(ComponentListEventArgs e)
        {
            await Implement.DeleteAsync();
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
                var site = await Mediakiwi.Data.Site.SelectOneAsync(Implement.SiteID.Value);
                if (site.Type.HasValue)
                    Implement.Target = ComponentListTarget.Administration;
            }

            if (!Implement.FolderID.HasValue && Implement.SiteID.HasValue)
            {
                Mediakiwi.Data.Folder Folder = await Mediakiwi.Data.Folder.SelectOneBySiteAsync(Implement.SiteID.Value, FolderType.List);

                if (!Folder.IsNewInstance)
                {
                    if (Implement.FolderID.HasValue)
                    {
                        Mediakiwi.Data.Folder Folder2 = await Mediakiwi.Data.Folder.SelectOneAsync(Implement.FolderID.Value);
                        if (Folder2.SiteID != Folder.SiteID)
                        {
                            //  New channel: reset folder
                            Implement.FolderID = Folder.ID;
                        }
                    }
                    else
                        Implement.FolderID = Folder.ID;
                }
            }

            // [MR:23-01-2020] WAS :
            //if (!Implement.HasServiceCall)
            //{
            //    Implement.SenseInterval = 0;
            //    Implement.SenseScheduled = null;
            //}

            // [MR:23-01-2020] IS NOW :
            Implement.SenseInterval = 0;
            Implement.SenseScheduled = null;
            /*********************************/
            await Implement.SaveAsync();
            //if (!string.IsNullOrEmpty(Implement.Class) && Implement.ClassName == "Wim.Templates.Templates.UI.GenericsList")
            //{
            //    Sushi.Mediakiwi.Framework.CodeGeneration.GenericList.CreateProxy(Implement.ID);
            //}
            wim.FlushCache();
        }

        /// <summary>
        /// Components the list_ list search.
        /// </summary>
        /// <param name="sender">The sender.</param>
        private async Task ComponentList_ListSearch(ComponentListSearchEventArgs e)
        {
            wim.CanAddNewItem = true;
            wim.ListDataColumns.Add("ID", "ID", ListDataColumnType.UniqueIdentifier);
            wim.ListDataColumns.Add("Ref", "ReferenceID", 30);
            wim.ListDataColumns.Add("Name", "Name", ListDataColumnType.HighlightPresent);
            wim.ListDataColumns.Add("Single item name", "SingleItemName");
            wim.ListDataColumns.Add("Channel", "SiteName");

            wim.ListDataColumns.Add("Visible", "IsVisible");
            wim.ListDataColumns.Add("Inherited", "IsInherited");

            wim.ForceLoad = true;

            var result = await Mediakiwi.Data.ComponentList.SelectAllAsync(this.FilterText, this.FilterSite);
            wim.ListDataAdd(result);
        }

        #region Lookup
       





        private ListItemCollection m_Catalogs;
        /// <summary>
        /// Gets the catalogs.
        /// </summary>
        /// <value>The catalogs.</value>
        public ListItemCollection Catalogs
        {
            get
            {
                if (m_Catalogs != null)
                    return m_Catalogs;

                m_Catalogs = new ListItemCollection();
                ListItem li;

                m_Catalogs.Add(new ListItem("", ""));

                foreach (Catalog item in Catalog.SelectAll())
                {
                    if (!item.IsActive)
                        continue;

                    li = new ListItem(string.Format("{0} - [{1}]", item.Title, item.Table), item.ID.ToString());
                    m_Catalogs.Add(li);
                }
                return m_Catalogs;
            }
        }
        #endregion
    }
}
