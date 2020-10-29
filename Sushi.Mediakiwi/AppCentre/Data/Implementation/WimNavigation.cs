//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Web;
//using System.Web.UI;
//using System.Web.UI.WebControls;
//using Sushi.Mediakiwi.Framework;

//namespace Sushi.Mediakiwi.AppCentre.Data.Implementation
//{
//    /// <summary>
//    /// 
//    /// </summary>
//    public class WimNavigation : BaseImplementation
//    {
//        /// <summary>
//        /// Initializes a new instance of the <see cref="WimNavigation"/> class.
//        /// </summary>
//        public WimNavigation()
//        {
//            this.ListSave += new ComponentListEventHandler(WimNavigation_ListSave);
//            this.ListDelete += new ComponentListEventHandler(WimNavigation_ListDelete);
//            this.ListLoad += new ComponentListEventHandler(WimNavigation_ListLoad);
//            this.ListSearch += new ComponentSearchEventHandler(WimNavigation_ListSearch);
//            this.ListDelete += new ComponentListEventHandler(WimNavigation_ListDelete);
//        }

//        /// <summary>
//        /// Gets the available roles.
//        /// </summary>
//        /// <value>The available roles.</value>
//        public ListItemCollection AvailableRoles
//        {
//            get
//            {
//                //Page.Trace.Write("AvailableRoles get{}", string.Format("{0}-{1}", SearchRole, SearchRole2));
//                ListItemCollection collection = new ListItemCollection();
//                collection.Add(new ListItem("Select a role", ""));
//                foreach (Sushi.Mediakiwi.Data.ApplicationRole role in Sushi.Mediakiwi.Data.ApplicationRole.SelectAll())
//                {
//                    collection.Add(new ListItem(role.Name, role.ID.ToString()));
//                }
//                return collection;
//            }
//        }

//        /// <summary>
//        /// Gets the available sites.
//        /// </summary>
//        /// <value>The available sites.</value>
//        public ListItemCollection AvailableSites
//        {
//            get
//            {
//                //Page.Trace.Write("AvailableRoles get{}", string.Format("{0}-{1}", SearchRole, SearchRole2));
//                ListItemCollection collection = new ListItemCollection();
//                collection.Add(new ListItem("Select a site", ""));
//                foreach (Sushi.Mediakiwi.Data.Site site in Sushi.Mediakiwi.Data.Site.SelectAll())
//                {
//                    collection.Add(new ListItem(site.Name, site.ID.ToString()));
//                }
//                return collection;
//            }
//        }

//        /// <summary>
//        /// Gets the available types.
//        /// </summary>
//        public ListItemCollection AvailableTypes
//        {
//            get
//            {
//                //Page.Trace.Write("AvailableRoles get{}", string.Format("{0}-{1}", SearchRole, SearchRole2));
//                ListItemCollection collection = new ListItemCollection();
//                collection.Add(new ListItem("", ""));
//                collection.Add(new ListItem("Section", "1"));
//                collection.Add(new ListItem("List", "2"));
//                collection.Add(new ListItem("Folder - Logic", "4"));
//                collection.Add(new ListItem("Folder - Page", "5"));
//                collection.Add(new ListItem("Dashboard", "6"));
//                return collection;
//            }
//        }

//        /// <summary>
//        /// Gets the available sections.
//        /// </summary>
//        public ListItemCollection AvailableSections
//        {
//            get
//            {
//                //Page.Trace.Write("AvailableRoles get{}", string.Format("{0}-{1}", SearchRole, SearchRole2));
//                ListItemCollection collection = new ListItemCollection();
//                collection.Add(new ListItem("", ""));
//                collection.Add(new ListItem("Page", "1"));
//                collection.Add(new ListItem("Logic", "2"));
//                collection.Add(new ListItem("Gallery", "3"));
//                collection.Add(new ListItem("Administration", "4"));
//                return collection;
//            }
//        }

//        ListItemCollection m_AvailableDashboards;
//        /// <summary>
//        /// Gets the available dashboards.
//        /// </summary>
//        public ListItemCollection AvailableDashboards
//        {
//            get
//            {
//                if (m_AvailableDashboards == null)
//                {
//                    m_AvailableDashboards = new ListItemCollection();
//                    m_AvailableDashboards.Add(new ListItem("Select a dashboard", ""));
//                    foreach (Sushi.Mediakiwi.Data.Dashboard item in Sushi.Mediakiwi.Data.Dashboard.SelectAll())
//                    {
//                        m_AvailableDashboards.Add(new ListItem(item.Name, item.ID.ToString()));
//                    }
//                }
//                return m_AvailableDashboards;
//            }
//        }

//        ListItemCollection m_AvailableListFolders;
//        /// <summary>
//        /// Gets the available list folders.
//        /// </summary>
//        public ListItemCollection AvailableListFolders
//        {
//            get
//            {
//                if (m_AvailableListFolders == null)
//                {
//                    int currentSite = IsPostBack ? Wim.Utility.ConvertToInt(Request.Form["SiteID"]) : this.Implement.SiteID;


//                    m_AvailableListFolders = new ListItemCollection();
//                    m_AvailableListFolders.Add(new ListItem("Select a folder", ""));
//                    foreach (Sushi.Mediakiwi.Data.Folder item in Sushi.Mediakiwi.Data.Folder.SelectAll(Sushi.Mediakiwi.Data.FolderType.List, currentSite))
//                    {
//                        if (item.Level == 0) continue;

//                        string title = item.Name;
//                        int spacer = item.Level - 1;
//                        while (spacer > 0)
//                        {
//                            title = string.Concat("__", title);
//                            spacer--;
//                        }

//                        m_AvailableListFolders.Add(new ListItem(title, item.ID.ToString()));
//                    }
//                }
//                return m_AvailableListFolders;
//            }
//        }

//        ListItemCollection m_AvailablePageFolders;
//        /// <summary>
//        /// Gets the available page folders.
//        /// </summary>
//        public ListItemCollection AvailablePageFolders
//        {
//            get
//            {
//                if (m_AvailablePageFolders == null)
//                {
//                    int currentSite = IsPostBack ? Wim.Utility.ConvertToInt(Request.Form["SiteID"]) : this.Implement.SiteID;

//                    m_AvailablePageFolders = new ListItemCollection();
//                    m_AvailablePageFolders.Add(new ListItem("Select a folder", ""));
//                    foreach (Sushi.Mediakiwi.Data.Folder item in Sushi.Mediakiwi.Data.Folder.SelectAll(Sushi.Mediakiwi.Data.FolderType.List, currentSite))
//                    {
//                        m_AvailablePageFolders.Add(new ListItem(item.CompletePath, item.ID.ToString()));
//                    }
//                }
//                return m_AvailablePageFolders;
//            }
//        }

//        ListItemCollection m_AvailableLists;
//        /// <summary>
//        /// Gets the available lists.
//        /// </summary>
//        /// <value>The available lists.</value>
//        public ListItemCollection AvailableLists
//        {
//            get
//            {
//                if (m_AvailableLists == null)
//                {

//                    m_AvailableLists = new ListItemCollection();
//                    m_AvailableLists.Add(new ListItem("Select a folder", ""));
//                    foreach (Sushi.Mediakiwi.Data.ComponentList item in Sushi.Mediakiwi.Data.ComponentList.SelectAllBySite(Implement.SiteID))
//                    {
//                        m_AvailableLists.Add(new ListItem(item.Name, item.ID.ToString()));
//                    }
//                }
//                return m_AvailableLists;
//            }
//        }

//        /// <summary>
//        /// Gets the type.
//        /// </summary>
//        /// <param name="requestName">Name of the request.</param>
//        /// <param name="typeID">The type ID.</param>
//        /// <returns></returns>
//        public ListItemCollection GetType(string requestName, int typeID)
//        {
//            int type = IsPostBack ? Wim.Utility.ConvertToInt(Request.Form[requestName]) : typeID;

//            switch (type)
//            {
//                case 1: return AvailableSections;
//                case 2: return AvailableLists;
//                case 4: return AvailableListFolders;
//                case 5: return AvailablePageFolders;
//                case 6: return AvailableDashboards;
//            }
//            return null;
//        }

//        /// <summary>
//        /// Gets the available item0.
//        /// </summary>
//        /// <value>The available item0.</value>
//        public ListItemCollection AvailableItem0
//        {
//            get { return GetType("Item0_Type", Implement.Item0_Type); }
//        }

//        /// <summary>
//        /// Gets the available item1.
//        /// </summary>
//        /// <value>The available item1.</value>
//        public ListItemCollection AvailableItem1
//        {
//            get { return GetType("Item1_Type", Implement.Item1_Type); }
//        }

//        /// <summary>
//        /// Gets the available item2.
//        /// </summary>
//        /// <value>The available item2.</value>
//        public ListItemCollection AvailableItem2
//        {
//            get { return GetType("Item2_Type", Implement.Item2_Type); }
//        }

//        /// <summary>
//        /// Gets the available item3.
//        /// </summary>
//        /// <value>The available item3.</value>
//        public ListItemCollection AvailableItem3
//        {
//            get { return GetType("Item3_Type", Implement.Item3_Type); }
//        }

//        /// <summary>
//        /// Gets the available item4.
//        /// </summary>
//        /// <value>The available item4.</value>
//        public ListItemCollection AvailableItem4
//        {
//            get { return GetType("Item4_Type", Implement.Item4_Type); }
//        }

//        /// <summary>
//        /// Gets the available item5.
//        /// </summary>
//        /// <value>The available item5.</value>
//        public ListItemCollection AvailableItem5
//        {
//            get { return GetType("Item5_Type", Implement.Item5_Type); }
//        }

//        /// <summary>
//        /// Gets the available item6.
//        /// </summary>
//        /// <value>The available item6.</value>
//        public ListItemCollection AvailableItem6
//        {
//            get { return GetType("Item6_Type", Implement.Item6_Type); }
//        }

//        /// <summary>
//        /// Gets the available item7.
//        /// </summary>
//        /// <value>The available item7.</value>
//        public ListItemCollection AvailableItem7
//        {
//            get { return GetType("Item7_Type", Implement.Item7_Type); }
//        }

//        /// <summary>
//        /// Gets the available item8.
//        /// </summary>
//        /// <value>The available item8.</value>
//        public ListItemCollection AvailableItem8
//        {
//            get { return GetType("Item8_Type", Implement.Item8_Type); }
//        }

//        void WimNavigation_ListSave(object sender, ComponentListEventArgs e)
//        {
//            Implement.Save();
//        }

//        void WimNavigation_ListDelete(object sender, ComponentListEventArgs e)
//        {
//            Implement.Delete();
//        }

//        void WimNavigation_ListLoad(object sender, ComponentListEventArgs e)
//        {
//            Implement = Sushi.Mediakiwi.Data.WimNavigation.SelectOne(e.SelectedKey);
//        }

//        void WimNavigation_ListSearch(object sender, ComponentListSearchEventArgs e)
//        {
//            wim.CanAddNewItem = true;
//            wim.ListDataColumns.Add("ID", "ID", ListDataColumnType.UniqueIdentifier);
//            wim.ListDataColumns.Add("Role", "Role", ListDataColumnType.HighlightPresent);
//            wim.ListDataColumns.Add("Site", "Site");

//            wim.ListData = Sushi.Mediakiwi.Data.WimNavigation.SelectAll();
//        }


//        Sushi.Mediakiwi.Data.WimNavigation m_Implement;
//        /// <summary>
//        /// Gets or sets the implement.
//        /// </summary>
//        /// <value>The implement.</value>
//        [Sushi.Mediakiwi.Framework.ContentListItem.DataExtend()]
//        public Sushi.Mediakiwi.Data.WimNavigation Implement
//        {
//            get { return m_Implement; }
//            set { m_Implement = value; }
//        }


//    }
//}
