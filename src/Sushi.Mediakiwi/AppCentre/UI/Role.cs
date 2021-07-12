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
            wim.SetPropertyVisibility("AllowedSites", !this.AllSites);
            return Task.CompletedTask;
        }

        async Task Role_ListDelete(ComponentListEventArgs e)
        {
            await m_Implement.DeleteAsync();
        }

        Mediakiwi.Data.IApplicationRole m_Implement;

        async Task Role_ListSave(ComponentListEventArgs e)
        {
            m_Implement.Name = m_RoleName;
            m_Implement.Description = m_Description;

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

            m_Implement.GalleryRoot = this.GalleryRootID;

            if (AllSites || m_Implement.All_Sites)
            {
                m_Implement.IsAccessSite = false;
                await Mediakiwi.Data.RoleRight.UpdateAsync(new int[0], Mediakiwi.Data.RoleRightType.Site, m_Implement.ID);
            }
            else
            {

            }

            if (AllLists || m_Implement.All_Lists)
            {
                m_Implement.IsAccessList = false;
                await Mediakiwi.Data.RoleRight.UpdateAsync(new int[0], Mediakiwi.Data.RoleRightType.List, m_Implement.ID);
            }
            if (AllFolders || m_Implement.All_Folders)
            {
                m_Implement.IsAccessFolder = false;
                await Mediakiwi.Data.RoleRight.UpdateAsync(new int[0], Mediakiwi.Data.RoleRightType.Folder, m_Implement.ID);
            }
            if (AllGalleries || m_Implement.All_Galleries)
            {
                m_Implement.IsAccessGallery = false;
                await Mediakiwi.Data.RoleRight.UpdateAsync(new int[0], Mediakiwi.Data.RoleRightType.Gallery, m_Implement.ID);
            }

            m_Implement.All_Sites = AllSites;
            m_Implement.All_Lists = AllLists;
            m_Implement.All_Folders = AllFolders;
            m_Implement.All_Galleries = AllGalleries;
            m_Implement.Dashboard = this.Dashboard;


            if (e.SelectedKey == 0)
            {
                //  Set default
                m_Implement.IsAccessFolder = false;
                m_Implement.IsAccessGallery = false;
                m_Implement.IsAccessList = false;
                m_Implement.IsAccessSite = false;
            }

            await m_Implement.SaveAsync();
        }

        async Task Role_ListLoad(ComponentListEventArgs e)
        {
            m_Implement = await Mediakiwi.Data.ApplicationRole.SelectOneAsync(e.SelectedKey);
            if (e.SelectedKey == 0)
                return;

            m_RoleName = m_Implement.Name;
            m_Description = m_Implement.Description;

            m_CanSeePage = m_Implement.CanSeePage;
            m_CanSeeList = m_Implement.CanSeeList;
            m_CanSeeGallery = m_Implement.CanSeeGallery;
            CanSeeFolder = m_Implement.CanSeeFolder;
            m_CanSeeAdmin = m_Implement.CanSeeAdmin;

            m_CanChange = m_Implement.CanChangePage;
            m_CanCreate = m_Implement.CanCreatePage;
            m_CanPublish = m_Implement.CanPublishPage;
            m_CanDelete = m_Implement.CanDeletePage;

            CanCreateList = m_Implement.CanCreateList;
            CanChangeList = m_Implement.CanChangeList;

            this.AllSites = m_Implement.All_Sites;
            this.AllLists = m_Implement.All_Lists;
            this.AllFolders = m_Implement.All_Folders;
            this.AllGalleries = m_Implement.All_Galleries;

            this.GalleryRootID = m_Implement.GalleryRoot;
            this.Dashboard = m_Implement.Dashboard;


            if (e.SelectedKey == 0) return;
            if (!m_Implement.All_Sites)
                wim.AddTab(new Guid("93D10F58-6A1A-493F-8ADB-E53FC7CEDE19"));
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

            wim.ListDataColumns.Add("ID", "ID", ListDataColumnType.UniqueIdentifier);
            wim.ListDataColumns.Add("Role", "Name", ListDataColumnType.HighlightPresent);
            wim.ListDataColumns.Add("Description", "Description");
            //wim.ListDataColumns.Add("Settings", "CanSeeList");
            //wim.ListDataColumns.Add("Gallery", "CanSeeGallery");
            //wim.ListDataColumns.Add("Admin", "CanSeeAdmin");

            //wim.ListDataColumns.Add("Site (all)", "All_Sites");
            //wim.ListDataColumns.Add("List (all)", "All_Lists");
            //wim.ListDataColumns.Add("Folder (all)", "All_Folders");
            //wim.ListDataColumns.Add("Gallery (all)", "All_Galleries");

            wim.ListDataAdd(await Mediakiwi.Data.ApplicationRole.SelectAllAsync());
        }


        #region List attributes
        private string m_RoleName;
        /// <summary>
        /// Gets or sets the name of the role.
        /// </summary>
        /// <value>The name of the role.</value>
        [Framework.ContentListItem.TextField("Title", 50, true)]
        public string RoleName
        {
            get { return m_RoleName; }
            set { m_RoleName = value; }
        }

        private string m_Description;
        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        [Framework.ContentListItem.TextField("Description", 255, true)]
        public string Description
        {
            get { return m_Description; }
            set { m_Description = value; }
        }

        private int m_Dashboard;
        /// <summary>
        /// Gets or sets the dashboard.
        /// </summary>
        /// <value>The dashboard.</value>
        [Framework.ContentListItem.Choice_Dropdown("Dashboard", "AvailableDashboards", false, false)]
        public int Dashboard
        {
            get { return m_Dashboard; }
            set { m_Dashboard = value; }
        }

        private string m_SubText1 = "Environment access";
        /// <summary>
        /// Gets or sets the sub text1.
        /// </summary>
        /// <value>The sub text1.</value>
        [Framework.ContentListItem.Section()]
        public string SubText1
        {
            get { return m_SubText1; }
            set { m_SubText1 = value; }
        }

        private bool m_CanSeePage;
        /// <summary>
        /// Gets or sets a value indicating whether this instance can see page.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can see page; otherwise, <c>false</c>.
        /// </value>
        [Framework.ContentListItem.Choice_Checkbox("Pages", true, "Does this role have the right to control pages?", Expression = OutputExpression.Alternating)]
        public bool CanSeePage
        {
            get { return m_CanSeePage; }
            set { m_CanSeePage = value; }
        }

        private bool m_CanSeeList;
        /// <summary>
        /// Gets or sets a value indicating whether this instance can see list.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can see list; otherwise, <c>false</c>.
        /// </value>
        [Framework.ContentListItem.Choice_Checkbox("Lists", true, "Does this role have the right to control lists?", Expression = OutputExpression.Alternating)]
        public bool CanSeeList
        {
            get { return m_CanSeeList; }
            set { m_CanSeeList = value; }
        }

        private bool m_CanSeeGallery;
        /// <summary>
        /// Gets or sets a value indicating whether this instance can see gallery.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can see gallery; otherwise, <c>false</c>.
        /// </value>
        [Framework.ContentListItem.Choice_Checkbox("Galleries", "Does this role have the right to control galleries?", Expression = OutputExpression.Alternating)]
        public bool CanSeeGallery
        {
            get { return m_CanSeeGallery; }
            set { m_CanSeeGallery = value; }
        }

        private bool m_CanSeeAdmin;
        /// <summary>
        /// Gets or sets a value indicating whether this instance can see admin.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can see admin; otherwise, <c>false</c>.
        /// </value>
        [Framework.ContentListItem.Choice_Checkbox("Administration", true, "Does this role have the right to access the administration section?", Expression = OutputExpression.Alternating)]
        public bool CanSeeAdmin
        {
            get { return m_CanSeeAdmin; }
            set { m_CanSeeAdmin = value; }
        }

        [Framework.ContentListItem.Choice_Checkbox("Folder", false, "Does this role have the right to access any folder view?", Expression = OutputExpression.Alternating)]
        public bool CanSeeFolder { get; set; }

        private string m_SubText2 = "Page rights";
        /// <summary>
        /// Gets or sets the sub text2.
        /// </summary>
        /// <value>The sub text2.</value>
        [OnlyVisibleWhenTrue("CanSeePage")]
        [Framework.ContentListItem.Section()]
        public string SubText2
        {
            get { return m_SubText2; }
            set { m_SubText2 = value; }
        }

        private bool m_CanChange;
        /// <summary>
        /// Gets or sets a value indicating whether this instance can change.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can change; otherwise, <c>false</c>.
        /// </value>
        [OnlyVisibleWhenTrue("CanSeePage")]
        [Framework.ContentListItem.Choice_Checkbox("Change", "The right to change content (pages)", Expression = OutputExpression.Alternating)]
        public bool CanChange
        {
            get { return m_CanChange; }
            set { m_CanChange = value; }
        }

        private bool m_CanCreate;
        /// <summary>
        /// Gets or sets a value indicating whether this instance can create.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can create; otherwise, <c>false</c>.
        /// </value>
        [OnlyVisibleWhenTrue("CanSeePage")]
        [Framework.ContentListItem.Choice_Checkbox("Create", "The right to create content (pages and folders)", Expression = OutputExpression.Alternating)]
        public bool CanCreate
        {
            get { return m_CanCreate; }
            set { m_CanCreate = value; }
        }

        private bool m_CanPublish;
        /// <summary>
        /// Gets or sets a value indicating whether this instance can publish.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can publish; otherwise, <c>false</c>.
        /// </value>
        [OnlyVisibleWhenTrue("CanSeePage")]
        [Framework.ContentListItem.Choice_Checkbox("Publish", "The right to publish content (pages)", Expression = OutputExpression.Alternating)]
        public bool CanPublish
        {
            get { return m_CanPublish; }
            set { m_CanPublish = value; }
        }

        private bool m_CanDelete;
        /// <summary>
        /// Gets or sets a value indicating whether this instance can delete.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can delete; otherwise, <c>false</c>.
        /// </value>
        [OnlyVisibleWhenTrue("CanSeePage")]
        [Framework.ContentListItem.Choice_Checkbox("Remove", "The right to remove content (pages and folders)", Expression = OutputExpression.Alternating)]
        public bool CanDelete
        {
            get { return m_CanDelete; }
            set { m_CanDelete = value; }
        }

        private string m_SubText4 = "List rights";
        /// <summary>
        /// Gets or sets the sub text3.
        /// </summary>
        /// <value>The sub text3.</value>
        [OnlyVisibleWhenTrue("CanSeeList")]
        [Framework.ContentListItem.Section()]
        public string SubText4
        {
            get { return m_SubText4; }
            set { m_SubText4 = value; }
        }

        private bool m_CanCreateList;
        /// <summary>
        /// Gets or sets a value indicating whether this instance can create list.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can create list; otherwise, <c>false</c>.
        /// </value>
        [OnlyVisibleWhenTrue("CanSeeList")]
        [Framework.ContentListItem.Choice_Checkbox("Create", "The right to create list instances / folder", Expression = OutputExpression.Alternating)]
        public bool CanCreateList
        {
            get { return m_CanCreateList; }
            set { m_CanCreateList = value; }
        }

        private bool m_CanChangeList;
        /// <summary>
        /// Gets or sets a value indicating whether this instance can change list.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can change list; otherwise, <c>false</c>.
        /// </value>
        [OnlyVisibleWhenTrue("CanSeeList")]
        [Framework.ContentListItem.Choice_Checkbox("Change", "The right to change list instances / folder", Expression = OutputExpression.Alternating)]
        public bool CanChangeList
        {
            get { return m_CanChangeList; }
            set { m_CanChangeList = value; }
        }


        private string m_SubText3 = "Channel and list access";
        /// <summary>
        /// Gets or sets the sub text3.
        /// </summary>
        /// <value>The sub text3.</value>
        [Framework.ContentListItem.Section()]
        public string SubText3
        {
            get { return m_SubText3; }
            set { m_SubText3 = value; }
        }

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

        //[Sushi.Mediakiwi.Framework.ContentListItem.SubListSelect("Channels", "18b297dc-7e01-404d-bc45-bb3ea3eb344e", false)]
        //public Mediakiwi.Data.SubList AllowedSites
        //{
        //    get; set;
        //}



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
                        m_AvailableGalleries.Add(new ListItem(item.CompletePath, item.ID.ToString()));
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
