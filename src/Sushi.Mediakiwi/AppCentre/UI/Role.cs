using Sushi.Mediakiwi.Framework;
using Sushi.Mediakiwi.UI;
using System;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.AppCentre.Data.Implementation
{
    /// <summary>
    /// Represents a Role entity.
    /// </summary>
    public class Role : BaseImplementation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Role"/> class.
        /// </summary>
        public Role()
        {
            wim.OpenInEditMode = true;
            wim.HideOpenCloseToggle = true;

            ListSearch += Role_ListSearch;
            ListLoad += Role_ListLoad;
            ListSave += Role_ListSave;
            ListDelete += Role_ListDelete;
            ListPreRender += Role_ListPreRender;
        }

        private Task Role_ListPreRender(ComponentListEventArgs arg)
        {
            wim.SetPropertyVisibility("AllowedSites", !AllSites);
            return Task.CompletedTask;
        }

        async Task Role_ListDelete(ComponentListEventArgs e)
        {
            await m_Implement.DeleteAsync().ConfigureAwait(false);
        }

        Mediakiwi.Data.IApplicationRole m_Implement;

        async Task Role_ListSave(ComponentListEventArgs e)
        {
            m_Implement.Name = RoleName;
            m_Implement.Description = Description;

            m_Implement.CanSeePage = CanSeePage;
            m_Implement.CanSeeList = CanSeeList;
            m_Implement.CanSeeAdmin = CanSeeAdmin;
            m_Implement.CanSeeGallery = CanSeeGallery;
            m_Implement.CanSeeFolder = CanSeeFolder;

            m_Implement.CanChangePage = CanChange;
            m_Implement.CanCreatePage = CanCreate;
            m_Implement.CanPublishPage = CanPublish;
            m_Implement.CanDeletePage = CanDelete;

            m_Implement.CanCreateList = CanCreateList;
            m_Implement.CanChangeList = CanChangeList;

            m_Implement.GalleryRoot = GalleryRootID;

            if (AllSites || m_Implement.All_Sites)
            {
                m_Implement.IsAccessSite = false;
                await Mediakiwi.Data.RoleRight.UpdateAsync(new int[0], Mediakiwi.Data.RoleRightType.Site, m_Implement.ID).ConfigureAwait(false);
            }

            if (AllLists || m_Implement.All_Lists)
            {
                m_Implement.IsAccessList = false;
                await Mediakiwi.Data.RoleRight.UpdateAsync(new int[0], Mediakiwi.Data.RoleRightType.List, m_Implement.ID).ConfigureAwait(false);
            }

            if (AllFolders || m_Implement.All_Folders)
            {
                m_Implement.IsAccessFolder = false;
                await Mediakiwi.Data.RoleRight.UpdateAsync(new int[0], Mediakiwi.Data.RoleRightType.Folder, m_Implement.ID).ConfigureAwait(false);
            }

            if (AllGalleries || m_Implement.All_Galleries)
            {
                m_Implement.IsAccessGallery = false;
                await Mediakiwi.Data.RoleRight.UpdateAsync(new int[0], Mediakiwi.Data.RoleRightType.Gallery, m_Implement.ID).ConfigureAwait(false);
            }

            m_Implement.All_Sites = AllSites;
            m_Implement.All_Lists = AllLists;
            m_Implement.All_Folders = AllFolders;
            m_Implement.All_Galleries = AllGalleries;
            //m_Implement.Dashboard = 0;

            if (e.SelectedKey == 0)
            {
                //  Set default
                m_Implement.IsAccessFolder = false;
                m_Implement.IsAccessGallery = false;
                m_Implement.IsAccessList = false;
                m_Implement.IsAccessSite = false;
            }

            await m_Implement.SaveAsync().ConfigureAwait(false);
        }

        async Task Role_ListLoad(ComponentListEventArgs e)
        {
            m_Implement = await Mediakiwi.Data.ApplicationRole.SelectOneAsync(e.SelectedKey).ConfigureAwait(false);
            if (e.SelectedKey == 0)
            {
                return;
            }

            RoleName = m_Implement.Name;
            Description = m_Implement.Description;

            CanSeePage = m_Implement.CanSeePage;
            CanSeeList = m_Implement.CanSeeList;
            CanSeeGallery = m_Implement.CanSeeGallery;
            CanSeeFolder = m_Implement.CanSeeFolder;
            CanSeeAdmin = m_Implement.CanSeeAdmin;

            CanChange = m_Implement.CanChangePage;
            CanCreate = m_Implement.CanCreatePage;
            CanPublish = m_Implement.CanPublishPage;
            CanDelete = m_Implement.CanDeletePage;

            CanCreateList = m_Implement.CanCreateList;
            CanChangeList = m_Implement.CanChangeList;

            AllSites = m_Implement.All_Sites;
            AllLists = m_Implement.All_Lists;
            AllFolders = m_Implement.All_Folders;
            AllGalleries = m_Implement.All_Galleries;

            GalleryRootID = m_Implement.GalleryRoot;
            //Dashboard = m_Implement.Dashboard;

            if (!m_Implement.All_Sites)
            {
                wim.AddTab(new Guid("93D10F58-6A1A-493F-8ADB-E53FC7CEDE19"));
            }

            //if (!m_Implement.All_Folders)
            //    wim.AddTab(new Guid("AB58D901-7305-4584-9133-53FB92684A2C"));
            //if (!m_Implement.All_Galleries)
            //    wim.AddTab(new Guid("5A7DB5D1-8DCE-4510-8423-47E4925B6C8B"));
            //if (!m_Implement.All_Lists)
            //    wim.AddTab(new Guid("B502AA2C-8D85-4D2C-9470-DFC8A388AC72"));

            //Map<Role>(x => x.AllowedSites, this).SubListSelect("Sites", typeof(Site), false);
            //FormMaps.Add(this);
        }

        async Task Role_ListSearch(ComponentListSearchEventArgs e)
        {
            wim.CanAddNewItem = true;
            wim.Page.HideTabs = true;

            wim.ListDataColumns.Add(new ListDataColumn("ID", nameof(Mediakiwi.Data.ApplicationRole.ID), ListDataColumnType.UniqueIdentifier));
            wim.ListDataColumns.Add(new ListDataColumn("Role", nameof(Mediakiwi.Data.ApplicationRole.Name), ListDataColumnType.HighlightPresent));
            wim.ListDataColumns.Add(new ListDataColumn("Description", nameof(Mediakiwi.Data.ApplicationRole.Description)));
            //wim.ListDataColumns.Add("Settings", "CanSeeList");
            //wim.ListDataColumns.Add("Gallery", "CanSeeGallery");
            //wim.ListDataColumns.Add("Admin", "CanSeeAdmin");

            //wim.ListDataColumns.Add("Site (all)", "All_Sites");
            //wim.ListDataColumns.Add("List (all)", "All_Lists");
            //wim.ListDataColumns.Add("Folder (all)", "All_Folders");
            //wim.ListDataColumns.Add("Gallery (all)", "All_Galleries");

            var results = await Mediakiwi.Data.ApplicationRole.SelectAllAsync().ConfigureAwait(false);
            wim.ListDataAdd(results);
        }

        #region List attributes

        /// <summary>
        /// Gets or sets the name of the role.
        /// </summary>
        /// <value>The name of the role.</value>
        [Framework.ContentListItem.TextField("Title", 50, true)]
        public string RoleName { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        [Framework.ContentListItem.TextField("Description", 255, true)]
        public string Description { get; set; }

        ///// <summary>
        ///// Gets or sets the dashboard.
        ///// </summary>
        ///// <value>The dashboard.</value>
        //[Framework.ContentListItem.Choice_Dropdown("Dashboard", nameof(AvailableDashboards), false, false)]
        //public int Dashboard { get; set; }

        /// <summary>
        /// Gets or sets the sub text1.
        /// </summary>
        /// <value>The sub text1.</value>
        [Framework.ContentListItem.Section()]
        public string SubText1 { get; set; } = "Environment access";

        /// <summary>
        /// Gets or sets a value indicating whether this instance can see page.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can see page; otherwise, <c>false</c>.
        /// </value>
        [Framework.ContentListItem.Choice_Checkbox("Pages", true, "Does this role have the right to control pages?", Expression = OutputExpression.Alternating)]
        public bool CanSeePage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance can see list.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can see list; otherwise, <c>false</c>.
        /// </value>
        [Framework.ContentListItem.Choice_Checkbox("Lists", true, "Does this role have the right to control lists?", Expression = OutputExpression.Alternating)]
        public bool CanSeeList { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance can see gallery.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can see gallery; otherwise, <c>false</c>.
        /// </value>
        [Framework.ContentListItem.Choice_Checkbox("Galleries", "Does this role have the right to control galleries?", Expression = OutputExpression.Alternating)]
        public bool CanSeeGallery { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance can see admin.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can see admin; otherwise, <c>false</c>.
        /// </value>
        [Framework.ContentListItem.Choice_Checkbox("Administration", true, "Does this role have the right to access the administration section?", Expression = OutputExpression.Alternating)]
        public bool CanSeeAdmin { get; set; }

        [Framework.ContentListItem.Choice_Checkbox("Folder", false, "Does this role have the right to access any folder view?", Expression = OutputExpression.Alternating)]
        public bool CanSeeFolder { get; set; }

        /// <summary>
        /// Gets or sets the sub text2.
        /// </summary>
        /// <value>The sub text2.</value>
        [OnlyVisibleWhenTrue(nameof(CanSeePage))]
        [Framework.ContentListItem.Section()]
        public string SubText2 { get; set; } = "Page rights";

        /// <summary>
        /// Gets or sets a value indicating whether this instance can change.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can change; otherwise, <c>false</c>.
        /// </value>
        [OnlyVisibleWhenTrue(nameof(CanSeePage))]
        [Framework.ContentListItem.Choice_Checkbox("Change", "The right to change content (pages)", Expression = OutputExpression.Alternating)]
        public bool CanChange { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance can create.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can create; otherwise, <c>false</c>.
        /// </value>
        [OnlyVisibleWhenTrue(nameof(CanSeePage))]
        [Framework.ContentListItem.Choice_Checkbox("Create", "The right to create content (pages and folders)", Expression = OutputExpression.Alternating)]
        public bool CanCreate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance can publish.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can publish; otherwise, <c>false</c>.
        /// </value>
        [OnlyVisibleWhenTrue(nameof(CanSeePage))]
        [Framework.ContentListItem.Choice_Checkbox("Publish", "The right to publish content (pages)", Expression = OutputExpression.Alternating)]
        public bool CanPublish { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance can delete.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can delete; otherwise, <c>false</c>.
        /// </value>
        [OnlyVisibleWhenTrue(nameof(CanSeePage))]
        [Framework.ContentListItem.Choice_Checkbox("Remove", "The right to remove content (pages and folders)", Expression = OutputExpression.Alternating)]
        public bool CanDelete { get; set; }

        /// <summary>
        /// Gets or sets the sub text3.
        /// </summary>
        /// <value>The sub text3.</value>
        [OnlyVisibleWhenTrue(nameof(CanSeeList))]
        [Framework.ContentListItem.Section()]
        public string SubText4 { get; set; } = "List rights";

        /// <summary>
        /// Gets or sets a value indicating whether this instance can create list.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can create list; otherwise, <c>false</c>.
        /// </value>
        [OnlyVisibleWhenTrue(nameof(CanSeeList))]
        [Framework.ContentListItem.Choice_Checkbox("Create", "The right to create list instances / folder", Expression = OutputExpression.Alternating)]
        public bool CanCreateList { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance can change list.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can change list; otherwise, <c>false</c>.
        /// </value>
        [OnlyVisibleWhenTrue(nameof(CanSeeList))]
        [Framework.ContentListItem.Choice_Checkbox("Change", "The right to change list instances / folder", Expression = OutputExpression.Alternating)]
        public bool CanChangeList { get; set; }

        /// <summary>
        /// Gets or sets the sub text3.
        /// </summary>
        /// <value>The sub text3.</value>
        [Framework.ContentListItem.Section()]
        public string SubText3 { get; set; } = "Channel and list access";

        /// <summary>
        /// Gets or sets a value indicating whether [all sites].
        /// </summary>
        /// <value><c>true</c> if [all sites]; otherwise, <c>false</c>.</value>
        [Framework.ContentListItem.Choice_Checkbox("All sites", false, "Access to all sites within Wim", AutoPostBack = true, Expression = OutputExpression.Alternating)]
        public bool AllSites { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [all lists].
        /// </summary>
        /// <value><c>true</c> if [all lists]; otherwise, <c>false</c>.</value>
        [Framework.ContentListItem.Choice_Checkbox("All lists", false, "Access to all list (dependend of choice of accessible sites)", Expression = OutputExpression.Alternating)]
        public bool AllLists{ get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [all lists].
        /// </summary>
        /// <value><c>true</c> if [all lists]; otherwise, <c>false</c>.</value>
        [Framework.ContentListItem.Choice_Checkbox("All folders", false, "Access to all folders", Expression = OutputExpression.Alternating)]
        public bool AllFolders { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [all lists].
        /// </summary>
        /// <value><c>true</c> if [all lists]; otherwise, <c>false</c>.</value>
        [Framework.ContentListItem.Choice_Checkbox("All galleries", false, "Access to all galleries", Expression = OutputExpression.Alternating)]
        public bool AllGalleries { get; set; }

        ListItemCollection m_AvailableGalleries;
        /// <summary>
        /// Gets the available dashboards.
        /// </summary>
        /// <value>The available dashboards.</value>
        public ListItemCollection AvailableGalleries
        {
            get
            {
                if (m_AvailableGalleries == null)
                {
                    m_AvailableGalleries = new ListItemCollection();
                    m_AvailableGalleries.Add(new ListItem("", ""));
                    foreach (Mediakiwi.Data.Gallery item in Mediakiwi.Data.Gallery.SelectAllAccessible(wim.CurrentApplicationUser))
                    {
                        m_AvailableGalleries.Add(new ListItem(item.CompletePath, $"{item.ID}"));
                    }
                }
                return m_AvailableGalleries;
            }
        }

        //[Sushi.Mediakiwi.Framework.ContentListItem.Choice_Dropdown("Root gallery", "AvailableGalleries")]
        public int? GalleryRootID { get; set; }

        #endregion
    }
}
