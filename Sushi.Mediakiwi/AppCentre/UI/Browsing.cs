using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Web;
using Sushi.Mediakiwi.Framework;
using System.Globalization;
using System.Text;
using Sushi.Mediakiwi.Data;

namespace Sushi.Mediakiwi.AppCentre.Data.Implementation
{
    /// <summary>
    /// Represents a Browsing entity.
    /// </summary>
    public partial class Browsing : BaseImplementation
    {
        /// <summary>
        /// Gets or sets the filter title.
        /// </summary>
        /// <value>The filter title.</value>
        //[Sushi.Mediakiwi.Framework.OnlyVisibleWhenTrue("IsGalleryOrHasRoot")]
        [Sushi.Mediakiwi.Framework.ContentListSearchItem.TextField("_search_for", 50)]
        public string FilterTitle { get; set; }

        /// <summary>
        /// Gets or sets the filter path.
        /// </summary>
        /// <value>The filter path.</value>
        //[Sushi.Mediakiwi.Framework.OnlyVisibleWhenTrue("IsGalleryOrHasRoot")]
        //[Sushi.Mediakiwi.Framework.ContentListSearchItem.Choice_Checkbox("Onderdeel van pad", Expression = OutputExpression.Alternating)]
        public bool FilterPath { get; set; }

        bool m_IsGallerySet;
        bool m_IsGallery;
        /// <summary>
        /// Gets or sets a value indicating whether this instance is gallery.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is gallery; otherwise, <c>false</c>.
        /// </value>
        public bool IsGallery {
            get {
                if (!m_IsGallerySet)
                {
                    m_IsGallerySet = true;
                    m_IsGallery = (wim.CurrentFolder.Type == Sushi.Mediakiwi.Data.FolderType.Gallery);
                }
                return m_IsGallery; 
            }
            set { m_IsGallery = value; }
        }
        /// <summary>
        /// Gets a value indicating whether this instance is page.
        /// </summary>
        /// <value><c>true</c> if this instance is page; otherwise, <c>false</c>.</value>
        public bool IsPage
        {
            get
            {
                return (wim.CurrentFolder.Type == Sushi.Mediakiwi.Data.FolderType.Page);
            }
        }
        /// <summary>
        /// Gets a value indicating whether this instance is gallery or has root.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is gallery or has root; otherwise, <c>false</c>.
        /// </value>
        public bool IsGalleryOrHasRoot
        {
            get
            {
                if (IsGallery && string.IsNullOrEmpty(Request.Query["root"]))
                    return true;
                if (wim.CurrentFolder.Type == Sushi.Mediakiwi.Data.FolderType.Page)
                    return true;

                return false;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Browsing"/> class.
        /// </summary>
        public Browsing()
        {
            this.FilterPath = false;
            wim.HideProperties = true;
         

            this.ListLoad += new ComponentListEventHandler(Browsing_ListLoad);
            this.ListSearch += new ComponentSearchEventHandler(Browsing_ListSearch);
            this.ListAction += new ComponentActionEventHandler(Browsing_ListAction);
            this.ListSave += Browsing_ListSave;
        }

        void Browsing_ListSave(object sender, ComponentListEventArgs e)
        {
        }

        /// <summary>
        /// Handles the ListLoad event of the Browsing control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Sushi.Mediakiwi.Framework.ComponentListEventArgs"/> instance containing the event data.</param>
        void Browsing_ListLoad(object sender, ComponentListEventArgs e)
        {
       

        }

        /// <summary>
        /// Handles the ListAction event of the Browsing control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Sushi.Mediakiwi.Framework.ComponentActionEventArgs"/> instance containing the event data.</param>
        void Browsing_ListAction(object sender, ComponentActionEventArgs e)
        {
            int galleryId = Utility.ConvertToInt(Request.Query["gallery"]);
            if (galleryId > 0)
            {
                Sushi.Mediakiwi.Data.Gallery gallery = Sushi.Mediakiwi.Data.Gallery.SelectOne(galleryId);
                if (!(gallery != null && gallery.ID == 0))
                {
                    Sushi.Mediakiwi.Data.IComponentList list = Sushi.Mediakiwi.Data.ComponentList.SelectOne(Sushi.Mediakiwi.Data.ComponentListType.Documents);
                    Response.Redirect(wim.Console.GetSafeUrl() + "&item=0&list=" + list.ID);
                }
            }
        }

 

        /// <summary>
        /// Gets a value indicating whether [opened in popup].
        /// </summary>
        /// <value><c>true</c> if [opened in popup]; otherwise, <c>false</c>.</value>
        public bool OpenedInPopup
        {
            get
            {
                return !string.IsNullOrEmpty(Request.Query["openinframe"]);
            }
        }

        //private bool m_AddNew;
        ///// <summary>
        ///// Gets or sets a value indicating whether [add new].
        ///// </summary>
        ///// <value><c>true</c> if [add new]; otherwise, <c>false</c>.</value>
        //
        //[Sushi.Mediakiwi.Framework.ContentListSearchItem.Button("create new", false, true)]
        //public bool AddNew
        //{
        //    get { return m_AddNew; }
        //    set { m_AddNew = value; }
        //}

        void ShowBrowsing()
        {
            wim.CurrentList.Option_Search_MaxResultPerPage = 512;
            wim.SearchResultItemPassthroughParameterProperty = "PassThrough";

            wim.ListDataColumns.Add(new ListDataColumn("ID", "ID", ListDataColumnType.UniqueIdentifier));
            wim.ListDataColumns.Add(new ListDataColumn("", "Icon") { ColumnWidth = 20 });
            wim.ListDataColumns.Add(new ListDataColumn(Labels.ResourceManager.GetString("list_name", new CultureInfo(wim.CurrentApplicationUser.LanguageCulture)), "Title", ListDataColumnType.HighlightPresent));
            wim.ListDataColumns.Add(new ListDataColumn("", "Info1") { ColumnWidth = 20 });

            List<BrowseItem> list = new List<BrowseItem>();

            bool isSearchInitiate = !string.IsNullOrEmpty(this.FilterTitle);
            if (wim.CurrentFolder.Type == Sushi.Mediakiwi.Data.FolderType.Gallery)
            {
                GetGalleryList(isSearchInitiate, list);
            }
            else if (wim.CurrentFolder.Type == Sushi.Mediakiwi.Data.FolderType.List || wim.CurrentFolder.Type == Sushi.Mediakiwi.Data.FolderType.Administration)
            {
                GetListList(isSearchInitiate, list);
            }
            else if (wim.CurrentFolder.Type == Sushi.Mediakiwi.Data.FolderType.Page)
            {
                GetPageList(isSearchInitiate, list);
            }
            wim.ListDataApply(list);
        }

        string GetStatus(bool isEdited, bool isPublished, bool isSearchable, bool hasMaster, bool isLocalisedEditMode, bool isLocalisedPublicationMode)
        {
            string status = string.Empty;

            if (isPublished)
                status += "<span class=\"icon-globe abbr right\" title=\"Published\"></span>";
            if (hasMaster)
            {
                if (!isLocalisedEditMode && !isLocalisedPublicationMode)
                    status += "<span class=\"icon-download-01 abbr right\" title=\"Inherited page\"></span>";
                else if (!isLocalisedEditMode && isLocalisedPublicationMode)
                    status += "<span class=\"icon-download-01 abbr right inactive\" title=\"Inherited page (only edit)\"></span>";
                else if (isLocalisedEditMode && !isLocalisedPublicationMode)
                    status += "<span class=\"icon-download-01 abbr right inactive\" title=\"Inherited page (only publication)\"></span>";
            }
            if (isEdited)
                status += "<span class=\"icon-pen abbr right\" title=\"Edited\"></span>";

            if (!isSearchable)
                status += "<span class=\"icon-search-minus abbr right\" title=\"Searchable\"></span>";

            return status;
        }

        Sushi.Mediakiwi.Data.PageSortBy GetSort(int? sorderOrderMethod)
        {
            var sortBy = Sushi.Mediakiwi.Data.PageSortBy.SortOrder;

            switch (sorderOrderMethod)
            {
                case 1:
                    sortBy = Sushi.Mediakiwi.Data.PageSortBy.CustomDate;
                    break;
                case 2:
                    sortBy = Sushi.Mediakiwi.Data.PageSortBy.CustomDateDown;
                    break;
                case 3:
                    sortBy = Sushi.Mediakiwi.Data.PageSortBy.LinkText;
                    break;
                case 4:
                    sortBy = Sushi.Mediakiwi.Data.PageSortBy.Name;
                    break;
                case 5:
                default:
                    sortBy = Sushi.Mediakiwi.Data.PageSortBy.SortOrder;
                    break;
            }
            return sortBy;
        }

        void GetPageList(bool isSearchInitiate, List<BrowseItem> list)
        {
            wim.Page.Body.ShowInFullWidthMode = false;

            _columns = 1;

            #region Folder navigation
            Sushi.Mediakiwi.Data.Folder[] folders = null;

            bool isRootLevelView = false;// wim.CurrentFolder.Level == 0;
            if (isSearchInitiate || isRootLevelView)
            {
                isRootLevelView = true;
                folders = Sushi.Mediakiwi.Data.Folder.SelectAll(wim.CurrentFolder.Type, wim.CurrentSite.ID, this.FilterTitle, this.FilterPath);
            }
            else
                folders = Sushi.Mediakiwi.Data.Folder.SelectAllByParent(wim.CurrentFolder.ID, wim.CurrentFolder.Type, false);

            //  ACL determination
            folders = Sushi.Mediakiwi.Data.Folder.ValidateAccessRight(folders, wim.CurrentApplicationUser);

            if (wim.CurrentFolder.Level == 0 && folders.Length == 0 && !isRootLevelView && !isSearchInitiate)
            {
                isRootLevelView = true;
                folders = Sushi.Mediakiwi.Data.Folder.SelectAll(wim.CurrentFolder.Type, wim.CurrentSite.ID, this.FilterTitle, this.FilterPath);
                //  ACL determination
                folders = Sushi.Mediakiwi.Data.Folder.ValidateAccessRight(folders, wim.CurrentApplicationUser);
            }
            #endregion Folder navigation

            IEnumerable<Page> pages;
            if (!isSearchInitiate)
                pages = Page.SelectAll(wim.CurrentFolder.ID, PageFolderSortType.Folder, PageReturnProperySet.All, GetSort(wim.CurrentFolder.ID), false);
            else
            {
                pages = Page.SelectAll(this.FilterTitle, FilterPath);
            }

            pages = Sushi.Mediakiwi.Data.Page.ValidateAccessRight(pages, wim.CurrentApplicationUser);
            StringBuilder build = null;
            build = new StringBuilder();

            if (pages.Count() == 0 && (folders.Length == 0 || (folders.Length == 1 && folders[0].Name == "/")))
                return;

            build.AppendFormat("<div class=\"widget\">");

            //if (wim.CurrentFolder.ParentID.HasValue)
            //{
            //    build.AppendFormat("<a href=\"{0}?folder={1}\">{2}</a>", wim.Console.WimPagePath, wim.CurrentFolder.ParentID, "..."
            //        );
            //}

            if (pages.Count() > 0)
            {
              
                foreach (var entry in pages)
                {
                    build.AppendFormat("<a href=\"{0}?page={1}\"{4}>{2}{3}</a>", wim.Console.WimPagePath, entry.ID, entry.Name
                        , GetStatus(entry.IsEdited, entry.IsPublished, entry.IsSearchable, entry.MasterID.HasValue, entry.InheritContentEdited, entry.InheritContent)
                        , entry.IsPublished ? string.Empty : " class=\"inactive\""
                        );

                }
                build.AppendFormat("</div>");
            }
            else
            {

            }
            FindPage(folders, build);
             
            #region Close column view
            build.AppendFormat("</article>");
            build.AppendFormat("</section>");
            build.Insert(0, string.Format("<article class=\"column-{0}\">", _columns > 3 ? 3 : _columns));
            build.Insert(0, "<section id=\"startWidgets\" class=\"component widgets\">");
            wim.Page.Body.Grid.Add(build.ToString(), true);
            #endregion Close column view
        }

        int _columns = 0;
        void FindPage(Sushi.Mediakiwi.Data.Folder[] folders, StringBuilder build)
        {
            foreach (Sushi.Mediakiwi.Data.Folder entry in folders)
            {
                if (!entry.IsVisible && !wim.CurrentApplicationUser.ShowHidden)
                    continue;

                //  No root elements
                if (entry.Name == "/")
                    continue;

                IEnumerable<Page> pages = Sushi.Mediakiwi.Data.Page.SelectAll(entry.ID, Sushi.Mediakiwi.Data.PageFolderSortType.Folder, Sushi.Mediakiwi.Data.PageReturnProperySet.All, GetSort(entry.SortOrderMethod), false);
                pages = Sushi.Mediakiwi.Data.Page.ValidateAccessRight(pages, wim.CurrentApplicationUser);

                _columns++;
                //class=\"abbr\" 
                build.AppendFormat("<div class=\"widget\">");
                build.AppendFormat("<h2><a href=\"{0}?folder={1}\" title=\"{3}\">{2}</a></h2>", wim.Console.WimPagePath, entry.ID, entry.Name, entry.CompletePath);
                if (!string.IsNullOrEmpty(entry.Description))
                    build.AppendFormat("<p>{0}</p>", entry.Description);

                if (pages.Count() > 0)
                {
                    foreach (var i in pages)
                        build.AppendFormat("<a href=\"{0}?page={1}\"{4}>{2}{3}</a>", wim.Console.WimPagePath, i.ID, i.Name
                            , GetStatus(i.IsEdited, i.IsPublished, i.IsSearchable, i.MasterID.HasValue, i.InheritContentEdited, i.InheritContent)
                            , i.IsPublished ? string.Empty : " class=\"inactive\""
                            );
                }
                else
                {
                    //build.Append("<a class=\"inactive\">....</a>");
                }
                build.AppendFormat("</div>");
                var arr = Sushi.Mediakiwi.Data.Folder.SelectAllByParent(entry.ID);
                FindPage(arr, build);
            }
        }

        void GetGalleryList(bool isSearchInitiate, List<BrowseItem> list)
        {
            int baseGalleryID = wim.CurrentApplicationUserRole.GalleryRoot.GetValueOrDefault();

            Sushi.Mediakiwi.Data.IComponentList list0 = Sushi.Mediakiwi.Data.ComponentList.SelectOne(Sushi.Mediakiwi.Data.ComponentListType.Folders);

            var folderSettings = Sushi.Mediakiwi.Data.ComponentList.SelectOne(new Guid("97292dd5-ebda-4318-8aaf-4c49e887cdad"));
            //wim.ListDataColumns.Add("", "Info1", 90, Align.Right);
            ////wim.ListDataColumns.Add("Type", "Info2");
            //wim.ListDataColumns.Add(Labels.ResourceManager.GetString("list_datemodified", new CultureInfo(wim.CurrentApplicationUser.LanguageCulture)), "Info3", 110, Align.Center);
            //wim.ListDataColumns.Add("", "Info4", 20, Align.Center);

            Sushi.Mediakiwi.Data.Gallery[] galleries;
            Sushi.Mediakiwi.Data.Gallery rootGallery = Sushi.Mediakiwi.Data.Gallery.SelectOne(Utility.ConvertToGuid(Request.Query["root"]));
            if (!IsPostBack || string.IsNullOrEmpty(this.FilterTitle))
            {
                if (wim.CurrentFolder.ParentID.GetValueOrDefault(0) > 0 && wim.CurrentFolder.ID != rootGallery.ID && wim.CurrentFolder.ID != baseGalleryID)
                {
                    BrowseItem item = new BrowseItem();
                    item.ID = wim.CurrentFolder.ParentID.Value;
                    item.Title = "...";
                    item.PassThrough = "gallery";
                    item.Icon = "<figure class=\"icon-folder icon\"></figure>";
                    item.Info1 = string.Format("<a href=\"{0}?list={1}&gallery={2}&item={2}\"><figure class=\"icon-settings-02 icon\"></figure></a>"
                        , wim.Console.WimPagePath
                        , folderSettings.ID
                        , item.ID
                        );
   
                    list.Add(item);
                }

                Sushi.Mediakiwi.Data.Gallery current = Sushi.Mediakiwi.Data.Gallery.SelectOne(wim.CurrentFolder.ID);

                galleries = Sushi.Mediakiwi.Data.Gallery.SelectAllByParent(wim.CurrentFolder.ID);
            }
            else
                galleries = Sushi.Mediakiwi.Data.Gallery.SelectAll(this.FilterTitle);

            bool isRootLevelView = false;
            if (wim.CurrentFolder.Level == 0 && galleries.Length == 0 && !isRootLevelView && !isSearchInitiate)
            {
                isRootLevelView = true;
            }

            foreach (Sushi.Mediakiwi.Data.Gallery entry in galleries)
            {
                BrowseItem item = new BrowseItem();
                item.ID = entry.ID;
                item.Title = isRootLevelView ? entry.CompleteCleanPath() : entry.Name;
                item.PassThrough = "gallery";
                item.Icon = "<figure class=\"icon-folder icon\"></figure>";
                item.Info1 = string.Format("<a href=\"{0}?list={1}&gallery={2}&item={2}\"><figure class=\"icon-settings-02 icon\"></figure></a>"
                    , wim.Console.WimPagePath
                    , folderSettings.ID
                    , entry.ID
                    );
               
                item.Info3 = entry.Created;
                list.Add(item);
            }

            List<Asset> assets;

            if (!IsPostBack || string.IsNullOrEmpty(this.FilterTitle))
                assets = Asset.SelectAll(wim.CurrentFolder.ID);
            else
                assets = Asset.SearchAll(this.FilterTitle);

            foreach (Sushi.Mediakiwi.Data.Asset entry in assets)
            {
                BrowseItem item = new BrowseItem();
                item.ID = entry.ID;
                item.Title = entry.Title;
                item.PassThrough = "asset";

                item.Icon = "<figure class=\"icon-document icon\"></figure>";
                item.Info1 = string.Format("<a href=\"{0}?asset={1}\"><figure class=\"icon-settings-02 icon\"></figure></a>"
                    , wim.Console.WimPagePath
                    , entry.ID
                    , entry.Title
                    );

                item.Info2 = entry.Type;
                item.Info3 = entry.Created;

                if (!string.IsNullOrEmpty(entry.RemoteLocation) || entry.Exists)
                    item.Info4 = Utils.GetIconImageString(wim.Console, Utils.IconImage.File, Utils.IconSize.Normal, null, entry.DownloadFullUrl);
                else
                    item.Info4 = Utils.GetIconImageString(wim.Console, Utils.IconImage.NoFile, Utils.IconSize.Normal, null);

                item.HiddenField = string.Format("{0} ({1})", item.Title, item.Info1);

                list.Add(item);
            }
        }

        //public string ParentURL { get; set; }
        //[Sushi.Mediakiwi.Framework.ContentListSearchItem.Button(null, ButtonClassName = "flaticon icon-arrow-left-02", IconTarget = ButtonTarget.TopLeft, CustomUrlProperty = "ParentURL")]
        //public bool Parent { get; set; }

        void GetListList(bool isSearchInitiate, List<BrowseItem> list)
        {
            StringBuilder build = new StringBuilder();
            Sushi.Mediakiwi.Data.Folder[] f = new Sushi.Mediakiwi.Data.Folder[] { wim.CurrentFolder };

            FindList(f, build, false);

            build.AppendFormat("</article>");
            build.AppendFormat("</section>");
            build.Insert(0, string.Format("<article class=\"column-{0}\">", _columns > 3 ? 3 : _columns));
            build.Insert(0, "<section id=\"startWidgets\" class=\"component widgets\">");
            wim.Page.Body.Grid.Add(build.ToString(), true);
        }

        void FindList(Sushi.Mediakiwi.Data.Folder[] folders, StringBuilder build, bool ignoreHeader)
        {
            foreach (Sushi.Mediakiwi.Data.Folder entry in folders)
            {
                if (!entry.IsVisible && !wim.CurrentApplicationUser.ShowHidden)
                    continue;

                Sushi.Mediakiwi.Data.IComponentList[] all_lists = Sushi.Mediakiwi.Data.ComponentList.SelectAll(entry.ID, wim.CurrentApplicationUser, true);
                var allowed_lists = Sushi.Mediakiwi.Data.ComponentList.ValidateAccessRight(all_lists, wim.CurrentApplicationUser, wim.CurrentSite.ID);
                Sushi.Mediakiwi.Data.IComponentList[] selected_lists = null;

                if (wim.CurrentApplicationUser.ShowHidden)
                    selected_lists = allowed_lists;
                else
                    selected_lists = (from x in allowed_lists where x.IsVisible select x).ToArray();

                if (selected_lists.Length > 0)
                {
                    _columns++;
                    build.AppendFormat("<div class=\"widget\">");
                    if (!ignoreHeader && entry.Name != "/")
                    {
                        build.AppendFormat("<h2><a href=\"{0}?folder={1}\" {3}>{2}</a></h2>"
                            , wim.Console.WimPagePath
                            , entry.ID
                            , entry.Name
                            , (entry.IsVisible ? "" : " style=\"color: #d3d3d3\"")
                            );
                        if (!string.IsNullOrEmpty(entry.Description))
                            build.AppendFormat("<p>{0}</p>", entry.Description);
                    }

                    foreach (var i in selected_lists)
                    {
                        ComponentDataReportEventArgs e = null;
                        if (i.Option_HasDataReport)
                        {
                            var instance = i.GetInstance(Context);
                            if (instance != null)
                                e = instance.wim.DoListDataReport();
                        }
                        if (e == null || !e.ReportCount.HasValue)
                        {
                            build.AppendFormat("<a href=\"{0}?list={1}\"{3}>{2}</a>", wim.Console.WimPagePath, i.ID, i.Name
                                , i.IsVisible ? string.Empty : " class=\"inactive\""
                                );
                        }
                        else
                        {
                            string count = e.ReportCount.Value.ToString();
                            if (e.ReportCount.Value > 99)
                                count = "99+";

                            build.AppendFormat("<a href=\"{0}?list={1}\"{3}>{2} <span class=\"items{5}\">{4}</span></a>", wim.Console.WimPagePath, i.ID, i.Name
                                 , i.IsVisible ? string.Empty : " class=\"inactive\""
                                , count, e.IsAlert ? " attention" : null
                                 );
                        }
                    }

                    build.AppendFormat("</div>");
                }
                var arr = Sushi.Mediakiwi.Data.Folder.SelectAllByParent(entry.ID, Sushi.Mediakiwi.Data.FolderType.Undefined, !wim.CurrentApplicationUser.ShowHidden);

                FindList(arr, build, false);
            }
        }

        /// <summary>
        /// Browsing_s the list search.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="Sushi.Mediakiwi.Framework.ComponentListSearchEventArgs"/> instance containing the event data.</param>
        void Browsing_ListSearch(object sender, ComponentListSearchEventArgs e)
        {
            wim.HideSearchButton = true;

            wim.SetPropertyVisibility("Parent", false);
            
            ShowBrowsing();
            //return;
            
            //if (wim.CurrentVisitor.Data["wim.showmulti"].ParseBoolean())
            //{
            //    wim.CurrentVisitor.Data.Apply("wim.showmulti", null);
            //    wim.CurrentVisitor.Save();
            //}

            //if (!IsPostBack)
            //    this.FilterTitle = null;
            
            //wim.ListTitle = wim.CurrentFolder.Name;
            //wim.Page.HideTabs = true;

            //string browserInfo = string.Concat(Request.Browser.Browser.ToLower(), Request.Browser.MajorVersion.ToString());
            //bool isExplorer6 = browserInfo == "ie6";
            //string ext = isExplorer6 ? "gif" : "png";

            //wim.CanAddNewItem = true;
            //wim.SearchResultItemPassthroughParameterProperty = "PassThrough";
            //wim.CurrentList.Option_Search_MaxResultPerPage = 250;

            //bool isThumbnailView = !wim.CurrentApplicationUser.ShowDetailView;
            //bool isSearchInitiate = !string.IsNullOrEmpty(this.FilterTitle);

            //wim.ListDataColumns.Add("ID", "ID", ListDataColumnType.UniqueIdentifier);

            ////if (!wim.CurrentApplicationUser.ShowNewDesign2)
            //    wim.ListDataColumns.Add("", "Icon", ListDataColumnType.Default, ListDataContentType.Default, null, ListDataTotalType.Default, 20);

            //if (Utility.ConvertToInt(Request.Query["openinframe"]) == 0)
            //{
            //    wim.ListDataColumns.Add(Labels.ResourceManager.GetString("list_name", new CultureInfo(wim.CurrentApplicationUser.LanguageCulture)), "Title", ListDataColumnType.HighlightPresent);
            //}
            //else
            //{
            //    wim.ListDataColumns.Add("", "HiddenField", ListDataColumnType.Highlight);
            //    wim.ListDataColumns.Add("", "Title");
            //}

            //if (!wim.ShowNewDesign2)
            //    wim.ListDataColumns.Add("PassThrough", "PassThrough", ListDataColumnType.ExportOnly); 
            //wim.ForceLoad = true;

            //List<BrowseItem> list = new List<BrowseItem>();

            //int type = Utility.ConvertToInt(Request.Query["type"]);
            
            //if (wim.CurrentFolder.Type != Sushi.Mediakiwi.Data.FolderType.Gallery)
            //{
            //    Sushi.Mediakiwi.Data.IComponentList list0 = Sushi.Mediakiwi.Data.ComponentList.SelectOne(Sushi.Mediakiwi.Data.ComponentListType.Folders);

            //    bool showFolders = true;
                
            //    //if (wim.CurrentFolder.Type == Sushi.Mediakiwi.Data.FolderType.List)
            //        //showFolders = wim.Console.CurrentWimNavigation.ShowFolders;

            //    //if (wim.CurrentFolder.Level < 3)
            //    //    showFolders = false;

            //    if (showFolders)
            //    {
            //        if (wim.CurrentFolder.ParentID.GetValueOrDefault(0) > 0)
            //        {
            //            BrowseItem item = new BrowseItem();
            //            item.ID = wim.CurrentFolder.ParentID.Value;
            //            item.Title = "...";
            //            item.PassThrough = "folder";
                        

            //            if (wim.CurrentApplicationUser.ShowDetailView)
            //                item.Icon = string.Format("<img alt=\"\" src=\"{0}/images/icon_folder_link.png\"/>", wim.Console.WimRepository);
            //            else
            //            {
            //                if (wim.ShowNewDesign)
            //                {
            //                    item.Info1 = string.Format("<a class=\"multiple\" href=\"{0}?folder={1}\"><figure class=\"icon-folder\"></figure></a>"
            //                        , wim.Console.WimPagePath
            //                        , item.ID
            //                        );

            //                    if (wim.CurrentFolder.Type == Sushi.Mediakiwi.Data.FolderType.List || wim.CurrentFolder.Type == Sushi.Mediakiwi.Data.FolderType.Administration)
            //                        item.Icon = string.Concat(wim.Console.WimRepository, "/", "thumb_folder_logic.", ext);
            //                    else
            //                        item.Icon = string.Concat(wim.Console.WimRepository, "/", "thumb_folder.", ext);
            //                }
                         
            //                else
            //                {
            //                    if (wim.CurrentFolder.Type == Sushi.Mediakiwi.Data.FolderType.List || wim.CurrentFolder.Type == Sushi.Mediakiwi.Data.FolderType.Administration)
            //                        item.Icon = string.Concat(wim.Console.WimRepository, "/images/", "thumb_folder_logic.", ext);
            //                    else
            //                        item.Icon = string.Concat(wim.Console.WimRepository, "/images/", "thumb_folder.", ext);
            //                }
            //            }
            //            list.Add(item);
            //        }

            //        Sushi.Mediakiwi.Data.Folder[] folders = null;

            //        bool isRootLevelView = false;// wim.CurrentFolder.Level == 0;
            //        if (isSearchInitiate || isRootLevelView)
            //        {
            //            isRootLevelView = true;
            //            folders = Sushi.Mediakiwi.Data.Folder.SelectAll(wim.CurrentFolder.Type, wim.CurrentSite.ID, this.FilterTitle, this.FilterPath);
            //        }
            //        else
            //            folders = Sushi.Mediakiwi.Data.Folder.SelectAllByParent(wim.CurrentFolder.ID, wim.CurrentFolder.Type, false);

            //        //  ACL determination
            //        folders = Sushi.Mediakiwi.Data.Folder.ValidateAccessRight(folders, wim.CurrentApplicationUser);

            //        if (wim.CurrentFolder.Level == 0 && folders.Length == 0 && !isRootLevelView && !isSearchInitiate)
            //        {
            //            isRootLevelView = true;
            //            folders = Sushi.Mediakiwi.Data.Folder.SelectAll(wim.CurrentFolder.Type, wim.CurrentSite.ID, this.FilterTitle, this.FilterPath);
            //            //  ACL determination
            //            folders = Sushi.Mediakiwi.Data.Folder.ValidateAccessRight(folders, wim.CurrentApplicationUser);
            //        }

            //        foreach (Sushi.Mediakiwi.Data.Folder entry in folders)
            //        {
            //            BrowseItem item = new BrowseItem();
            //            item.ID = entry.ID;
            //            item.Title = isRootLevelView ? entry.CompleteCleanPath() : entry.Name;


            //            if (wim.Console.IsNewDesign)
            //            {
                            
            //                item.Icon = "<figure class=\"icon-folder icon\"></figure>";
            //                item.Info1 = string.Format("<a class=\"multiple\" href=\"{0}?folder={1}\"><figure class=\"icon-settings-02 icon\"></figure></a>"
            //                 , wim.Console.WimPagePath
            //                 , entry.ID
            //                 );
            //            }
            //            else
            //            {
            //                //if (entry.Type == Sushi.Mediakiwi.Data.FolderType.Page)
            //                item.Info1 = string.Format("<a href=\"{0}?list={1}&folder={2}&item={3}\"><img alt=\"\" title=\"{5}\" src=\"{4}/images/icon_folderOptions_link.png\"/></a>", wim.Console.WimPagePath, list0.ID, entry.ID, entry.ID, wim.Console.WimRepository
            //                    , Labels.ResourceManager.GetString("list_properties", new CultureInfo(wim.CurrentApplicationUser.LanguageCulture))
            //                    );
            //            }
            //            item.Info3 = entry.Changed;
            //            item.Info4 = entry.CompletePath;


            //            item.PassThrough = "folder";

            //            string thumbcandidate = string.Concat("thumb_folder.", ext);
            //            if (entry.Type == Sushi.Mediakiwi.Data.FolderType.List || entry.Type == Sushi.Mediakiwi.Data.FolderType.Administration)
            //                thumbcandidate = string.Concat("thumb_folder_logic.", ext);

            //            if (!wim.ShowNewDesign)
            //            {
            //                if (entry.MasterID.HasValue)
            //                {
            //                    thumbcandidate = string.Concat("thumb_folder_logic_inherit.", ext);
            //                    if (wim.CurrentApplicationUser.ShowDetailView)
            //                        item.Icon = string.Format("<img alt=\"\" src=\"{0}/images/icon_folder_iwhite.png\"/>", wim.Console.WimRepository);
            //                    else
            //                    {

            //                        item.Icon = string.Concat(wim.Console.WimRepository, "/images/", thumbcandidate);
            //                    }
            //                }
            //                else
            //                {
            //                    if (wim.CurrentApplicationUser.ShowDetailView)
            //                        item.Icon = string.Format("<img alt=\"\" src=\"{0}/images/icon_folder_link.png\"/>", wim.Console.WimRepository);
            //                    else
            //                        if (wim.ShowNewDesign)
            //                            item.Icon = string.Concat(wim.Console.WimRepository, "/", thumbcandidate);
            //                        else
            //                            item.Icon = string.Concat(wim.Console.WimRepository, "/images/", thumbcandidate);
            //                }
            //            }

            //            list.Add(item);
            //        }
            //    }
            //}
            //else
            //{
            //    int baseGalleryID = wim.CurrentApplicationUserRole.GalleryRoot.GetValueOrDefault();

            //    Sushi.Mediakiwi.Data.IComponentList list0 = Sushi.Mediakiwi.Data.ComponentList.SelectOne(Sushi.Mediakiwi.Data.ComponentListType.Folders);

            //    wim.ListDataColumns.Add("", "Info1", 90, Align.Right);
            //    //wim.ListDataColumns.Add("Type", "Info2");
            //    wim.ListDataColumns.Add(Labels.ResourceManager.GetString("list_datemodified", new CultureInfo(wim.CurrentApplicationUser.LanguageCulture)), "Info3", 110, Align.Center);
            //    wim.ListDataColumns.Add("", "Info4", 20, Align.Center);

            //    Sushi.Mediakiwi.Data.Gallery[] galleries;
            //    Sushi.Mediakiwi.Data.Gallery rootGallery = Sushi.Mediakiwi.Data.Gallery.SelectOne(Utility.ConvertToGuid(Request.Query["root"]));
            //    if (!IsPostBack || string.IsNullOrEmpty(this.FilterTitle))
            //    {
            //        if (wim.CurrentFolder.ParentID.GetValueOrDefault(0) > 0 && wim.CurrentFolder.ID != rootGallery.ID && wim.CurrentFolder.ID != baseGalleryID)
            //        {
            //            BrowseItem item = new BrowseItem();
            //            item.ID = wim.CurrentFolder.ParentID.Value;
            //            item.Title = "...";
            //            item.PassThrough = "gallery";

            //            if (wim.Console.IsNewDesign)
            //            {
            //                item.Info1 = string.Format("<a class=\"multiple\" href=\"{0}?gallery={1}\"><figure class=\"flaticon solid B13\"></figure><strong>{2}</strong></a>"
            //                 , wim.Console.WimPagePath
            //                 , item.ID
            //                 , item.Title
            //                 );
            //            }
            //            else
            //            {
            //                if (wim.CurrentApplicationUser.ShowDetailView)
            //                    item.Icon = string.Format("<img alt=\"\" src=\"{0}/images/icon_folder_link.png\"/>", wim.Console.WimRepository);
            //                else
            //                    item.Icon = string.Concat(wim.Console.WimRepository, "/images/", "thumb_folder.png");
            //            }


            //            list.Add(item);
            //        }

            //        Sushi.Mediakiwi.Data.Gallery current = Sushi.Mediakiwi.Data.Gallery.SelectOne(wim.CurrentFolder.ID);
                    
            //        //if (!Request.IsLocal)
            //        //current.UpdateFiles();

            //        galleries = Sushi.Mediakiwi.Data.Gallery.SelectAllByParent(wim.CurrentFolder.ID);
            //    }
            //    else
            //        galleries = Sushi.Mediakiwi.Data.Gallery.SelectAll(this.FilterTitle);

            //    //  [20110812:MM] Security turned off, takes to long.
            //    //if (!Wim.CommonConfiguration.RIGHTS_GALLERY_SUBS_ARE_ALLOWED)
            //    //    galleries = Sushi.Mediakiwi.Data.Gallery.ValidateAccessRight(galleries, wim.CurrentApplicationUser);

            //    bool isRootLevelView = false;
            //    if (wim.CurrentFolder.Level == 0 && galleries.Length == 0 && !isRootLevelView && !isSearchInitiate)
            //    {
            //        isRootLevelView = true;
            //        //galleries = Sushi.Mediakiwi.Data.Gallery.SelectAll();
            //        ////  ACL determination
            //        //galleries = Sushi.Mediakiwi.Data.Gallery.ValidateAccessRight(galleries, wim.CurrentApplicationUser);
            //    }

            //    foreach (Sushi.Mediakiwi.Data.Gallery entry in galleries)
            //    {
            //        BrowseItem item = new BrowseItem();
            //        item.ID = entry.ID;
            //        item.Title = isRootLevelView ? entry.CompleteCleanPath() : entry.Name;
            //        item.PassThrough = "gallery";
            //        //item.Info1 = entry.AssetCount2.ToString();

            //        if (wim.Console.IsNewDesign)
            //        {
            //            item.Info1 = string.Format("<a class=\"multiple\" href=\"{0}?gallery={1}\"><figure class=\"flaticon solid B13\"></figure><strong>{2}</strong></a>"
            //             , wim.Console.WimPagePath
            //             , entry.ID
            //             , entry.Name
            //             );
            //        }
            //        else
            //        {
            //            if (wim.CurrentApplicationUser.ShowDetailView)
            //                item.Icon = string.Format("<img alt=\"\" src=\"{0}/images/icon_folder_link.png\"/>", wim.Console.WimRepository);
            //            else
            //                item.Icon = string.Concat(wim.Console.WimRepository, "/images/", "thumb_folder.", ext);
            //        }

            //        if (!OpenedInPopup)
            //            item.Info4 = string.Format("<a href=\"{0}?list={1}&gallery={2}&item={3}\"><img alt=\"\" title=\"{5}\" src=\"{4}/images/icon_folderOptions_link.png\"/></a>", wim.Console.WimPagePath, list0.ID, entry.ID, entry.ID, wim.Console.WimRepository
            //            , Labels.ResourceManager.GetString("list_properties", new CultureInfo(wim.CurrentApplicationUser.LanguageCulture))
            //            );
            //        item.Info3 = entry.Created;
            //        list.Add(item);
            //    }
            //}

            //if (wim.CurrentFolder.Type == Sushi.Mediakiwi.Data.FolderType.Page)
            //{

            //    wim.ListDataColumns.Add(Labels.ResourceManager.GetString("list_datemodified", new CultureInfo(wim.CurrentApplicationUser.LanguageCulture)), "Info3", 110, Align.Center);
                
            //    if (wim.CurrentApplicationUserRole.CanChangePage)
            //        wim.ListDataColumns.Add("", "Info1", 20, Align.Center);

            //    Sushi.Mediakiwi.Data.IComponentList list0 = Sushi.Mediakiwi.Data.ComponentList.SelectOne(Sushi.Mediakiwi.Data.ComponentListType.PageProperties);
            //    Sushi.Mediakiwi.Data.PageSortBy sortBy = Sushi.Mediakiwi.Data.PageSortBy.SortOrder;
                
            //    switch ( wim.CurrentFolder.SortOrderMethod)
            //    {
            //        case 1:
            //            sortBy = Sushi.Mediakiwi.Data.PageSortBy.CustomDate;
            //            break;
            //        case 2:
            //            sortBy = Sushi.Mediakiwi.Data.PageSortBy.CustomDateDown;
            //            break;
            //        case 3:
            //            sortBy = Sushi.Mediakiwi.Data.PageSortBy.LinkText;
            //            break;
            //        case 4:
            //            sortBy = Sushi.Mediakiwi.Data.PageSortBy.Name;
            //            break;
            //        case 5:
            //        default:
            //            sortBy = Sushi.Mediakiwi.Data.PageSortBy.SortOrder;
            //            break;
            //    }

            //    Sushi.Mediakiwi.Data.Page[] pages;
            //    if (!isSearchInitiate)
            //        pages = Sushi.Mediakiwi.Data.Page.SelectAll(wim.CurrentFolder.ID, Sushi.Mediakiwi.Data.PageFolderSortType.Folder, Sushi.Mediakiwi.Data.PageReturnProperySet.All, sortBy, false);
            //    else
            //    {
            //        pages = Sushi.Mediakiwi.Data.Page.SelectAll(this.FilterTitle, FilterPath);
            //    }

            //    pages = Sushi.Mediakiwi.Data.Page.ValidateAccessRight(pages, wim.CurrentApplicationUser);
            //    foreach (Sushi.Mediakiwi.Data.Page entry in pages)
            //    {
                    
            //        string candidate;
            //        if (entry.MasterID.HasValue)
            //        {
            //            if (entry.InheritContent)
            //            {
            //                if (!entry.IsPublished)
            //                    candidate = string.Concat("thumb_page_inherit_unpub.", ext);
            //                else if (entry.IsEdited)
            //                    candidate = string.Concat("thumb_page_inherit_edit.", ext);
            //                else
            //                    candidate = string.Concat("thumb_page_inherit.", ext);
            //            }
            //            else
            //            {
            //                if (!entry.IsPublished)
            //                    candidate = string.Concat("thumb_page_local_unpub.", ext);
            //                else if (entry.IsEdited)
            //                    candidate = string.Concat("thumb_page_local_edit.", ext);
            //                else
            //                    candidate = string.Concat("thumb_page_local.", ext);
            //            }
            //        }
            //        else
            //        {

            //            if (!entry.IsPublished)
            //                candidate = string.Concat("thumb_page_unpub.", ext);
            //            else if (entry.IsEdited)
            //                candidate = string.Concat("thumb_page_edit.", ext);
            //            else
            //                candidate = string.Concat("thumb_unknown.", ext);
            //        }

            //        BrowseItem item = new BrowseItem();
            //        item.ID = entry.ID;

            //        item.Title = 
            //            isSearchInitiate
            //                ? (entry.IsFolderDefault ? string.Concat("<b>", entry.CompletePath, "</b> (default)") : entry.CompletePath)
            //                : (entry.IsFolderDefault ? string.Concat("<b>", entry.Name, "</b> (default)") : entry.Name);
                    
                    
            //        item.PassThrough = "page";
            //        item.Info1 = string.Format("<a href=\"{0}?list={1}&folder={2}&item={3}\"><img alt=\"\" title=\"{5}\" src=\"{4}/images/icon_properties_link.png\"/></a>", wim.Console.WimPagePath, list0.ID, entry.FolderID, entry.ID, wim.Console.WimRepository
            //            , Labels.ResourceManager.GetString("list_properties", new CultureInfo(wim.CurrentApplicationUser.LanguageCulture))
            //            );
            //        item.Info3 = entry.Published;

            //        if (wim.Console.IsNewDesign && !wim.ShowNewDesign2)
            //        {
            //            item.Info1 = string.Format("<a class=\"file\" href=\"{0}?page={1}\"><figure class=\"flaticon solid B9\"></figure><strong>{2}</strong></a>"
            //                , wim.Console.WimPagePath
            //                , entry.ID
            //                , entry.Name
            //                );
            //        }

            //        if (entry.IsPublished)
            //        {
            //            if (entry.MasterID.HasValue)
            //            {
            //                if (entry.InheritContent)
            //                {
            //                    //  Inherited content
            //                    if (wim.CurrentApplicationUser.ShowDetailView)
            //                    {
            //                        if (entry.IsEdited)
            //                            item.Icon = string.Format("<img alt=\"\" src=\"{0}/images/icon_ipage_edited.png\"/>", wim.Console.WimRepository);
            //                        else
            //                            item.Icon = string.Format("<img alt=\"\" src=\"{0}/images/icon_ipage.png\"/>", wim.Console.WimRepository);
            //                    }

            //                }
            //                else
            //                {
            //                    if (wim.CurrentApplicationUser.ShowDetailView)
            //                    {
            //                        //  Localised content
            //                        if (entry.IsEdited)
            //                            item.Icon = string.Format("<img alt=\"\" src=\"{0}/images/icon_lpage_edited.png\"/>", wim.Console.WimRepository);
            //                        else
            //                            item.Icon = string.Format("<img alt=\"\" src=\"{0}/images/icon_lpage.png\"/>", wim.Console.WimRepository);
            //                    }
            //                }
            //            }
            //            else
            //            {
            //                if (wim.CurrentApplicationUser.ShowDetailView)
            //                {
            //                    if (entry.IsEdited)
            //                        item.Icon = string.Format("<img alt=\"\" src=\"{0}/images/icon_page_edited.png\"/>", wim.Console.WimRepository);
            //                    else
            //                        item.Icon = string.Format("<img alt=\"\" src=\"{0}/images/icon_page_link.png\"/>", wim.Console.WimRepository);
            //                }
            //            }
            //        }
            //        else
            //        {
            //            if (entry.MasterID.HasValue)
            //            {
            //                if (entry.InheritContent)
            //                {
            //                    if (wim.CurrentApplicationUser.ShowDetailView)
            //                    {

            //                        //  Inherited content
            //                        item.Icon = string.Format("<img alt=\"\" src=\"{0}/images/icon_ipage_unpublished.png\"/>", wim.Console.WimRepository);
            //                    }
            //                }
            //                else
            //                {
            //                    if (wim.CurrentApplicationUser.ShowDetailView)
            //                    {
            //                        //  Localised content
            //                        item.Icon = string.Format("<img alt=\"\" src=\"{0}/images/icon_lpage_unpublished.png\"/>", wim.Console.WimRepository);
            //                    }
            //                }
            //            }
            //            else
            //            {
            //                if (wim.CurrentApplicationUser.ShowDetailView)
            //                {
            //                    item.Icon = string.Format("<img alt=\"\" src=\"{0}/images/icon_page_unpublished.png\"/>", wim.Console.WimRepository);
            //                }
            //            }

            //        }

            //        if (wim.ShowNewDesign)
            //            item.Icon = string.Concat(wim.Console.WimRepository, "/", candidate);
            //        //else
            //        //    item.Icon = string.Format("<img alt=\"\" src=\"{0}\"/>", string.Concat(wim.Console.WimRepository, "/images/", candidate));

            //        //if (!wim.CurrentApplicationUser.ShowDetailView)
            //        //    item.Icon = string.Concat(wim.Console.WimRepository, "/images/", candidate);
                    
            //        list.Add(item);
            //    }
            //}
            //else if (wim.CurrentFolder.Type == Sushi.Mediakiwi.Data.FolderType.List || wim.CurrentFolder.Type == Sushi.Mediakiwi.Data.FolderType.Administration)
            //{
            //    wim.ListDataColumns.Add(Labels.ResourceManager.GetString("list_description", new CultureInfo(wim.CurrentApplicationUser.LanguageCulture)), "Info2");
            //    if (wim.CurrentApplicationUserRole.CanChangeList || wim.Console.IsNewDesign)
            //    {
            //        if (!wim.ShowNewDesign2)
            //            wim.ListDataColumns.Add("", "Info1", 20, Align.Center);
            //    }
            //    //Sushi.Mediakiwi.Data.ComponentList[] clist = Sushi.Mediakiwi.Data.ComponentList.SelectAll(wim.CurrentFolder.ID); 
            //    //if (wim.CurrentApplicationUserRole.All_Lists || wim.CurrentFolder.Type == Sushi.Mediakiwi.Data.FolderType.Administration)
            //    //    clist = Sushi.Mediakiwi.Data.ComponentList.SelectAll(wim.CurrentFolder.ID);
            //    //else
            //    Sushi.Mediakiwi.Data.IComponentList[] clist = Sushi.Mediakiwi.Data.ComponentList.SelectAll(wim.CurrentFolder.ID, wim.CurrentApplicationUser, false);

            //    Sushi.Mediakiwi.Data.IComponentList list0 = Sushi.Mediakiwi.Data.ComponentList.SelectOne(Sushi.Mediakiwi.Data.ComponentListType.ComponentListProperties);

            //    clist = Sushi.Mediakiwi.Data.ComponentList.ValidateAccessRight(clist, wim.CurrentApplicationUser, wim.CurrentSite.ID);
            //    foreach (Sushi.Mediakiwi.Data.ComponentList entry in clist)
            //    {

            //        BrowseItem item = new BrowseItem();
            //        item.ID = entry.ID;

            //        //if (entry.Type != Sushi.Mediakiwi.Data.ComponentListType.Undefined)
            //        //{
            //        //    if (translation == null)
            //        //        translation = new Sushi.Mediakiwi.Beta.GeneratedCms.Translation();
            //        //    item.Title = translation.GetListTransation(entry.Type);
            //        //}
            //        //else
            //            item.Title = entry.Name;
                    
            //        item.PassThrough = "list";
            //        item.Info2 = entry.Description;

            //        if (wim.Console.IsNewDesign)
            //        {
            //            item.Info1 = string.Format("<a class=\"file\" href=\"{0}?list={3}\"><figure class=\"flaticon solid B9\"></figure><strong>{5}</strong></a>"
            //                , wim.Console.WimPagePath
            //                , list0.ID
            //                , wim.CurrentFolder.ID
            //                , entry.ID
            //                , wim.Console.WimRepository
            //                , entry.Name
            //                );
            //        }
            //        else
            //        {
            //            item.Info1 = string.Format("<a href=\"{0}?list={1}&folder={2}&item={3}\"><img alt=\"\" title=\"{5}\" src=\"{4}/images/icon_properties_link.png\"/></a>"
            //                , wim.Console.WimPagePath
            //                , list0.ID
            //                , wim.CurrentFolder.ID
            //                , entry.ID
            //                , wim.Console.WimRepository
            //                , Labels.ResourceManager.GetString("list_properties", new CultureInfo(wim.CurrentApplicationUser.LanguageCulture))
            //                );

            //            string candidate = string.Concat("thumb_logic.", ext);

            //            if (wim.CurrentApplicationUser.ShowDetailView)
            //                item.Icon = string.Format("<img alt=\"\" src=\"{0}/images/icon_logic_link.png\"/>", wim.Console.WimRepository);
            //            else
            //            {
            //                if (wim.ShowNewDesign)
            //                {
            //                    item.Icon = string.Concat(wim.Console.WimRepository, "/", candidate);
            //                }
            //                else
            //                    item.Icon = string.Concat(wim.Console.WimRepository, "/images/", candidate);
            //            }
            //        }

            //        list.Add(item);
            //    }
            //}
            //else if (wim.CurrentFolder.Type == Sushi.Mediakiwi.Data.FolderType.Gallery)
            //{
            //    Sushi.Mediakiwi.Data.Asset[] assets;

            //    if (!IsPostBack || string.IsNullOrEmpty(this.FilterTitle))
            //        assets = Sushi.Mediakiwi.Data.Asset.SelectAll(wim.CurrentFolder.ID);
            //    else
            //        assets = Sushi.Mediakiwi.Data.Asset.SearchAll(this.FilterTitle, null);

            //    //assets = Sushi.Mediakiwi.Data.Asset.ValidateAccessRight(assets, wim.CurrentApplicationUser);

            //    foreach (Sushi.Mediakiwi.Data.Asset entry in assets)
            //    {
            //        if (OpenedInPopup)
            //        {
            //            if (type == 1 && !entry.IsImage)
            //                continue;

            //            //if (type == 2 && entry.IsImage)
            //            //    continue;
            //        }

            //        BrowseItem item = new BrowseItem();
            //        item.ID = entry.ID;
            //        item.Title = entry.Title;
            //        item.PassThrough = "asset";

                   

            //        if (wim.CurrentApplicationUser.ShowDetailView)
            //        {
            //            string candidate = "undefined_16.png";

            //            if (entry.Extention == null)
            //                entry.Extention = "";

            //            switch (entry.Extention.ToLower())
            //            {
            //                case "docx": 
            //                case "doc": candidate = "doc_16.png"; break;
            //                case "pdf": candidate = "pdf_16.png"; break;
            //                case "jpeg":
            //                case "jpg":
            //                case "png":
            //                case "bmp":
            //                case "gif": candidate = "image_16.png"; break;
            //                case "xls":
            //                case "xlsx": candidate = "xls_16.png"; break;
            //                case "ppt":
            //                case "pptx": candidate = "ppt_16.png"; break;
            //                case "zip":
            //                case "rar": candidate = "zip_16.png"; break;
            //                case "vsd": candidate = "vsd_16.png"; break;
            //                case "eml":
            //                case "msg": candidate = "msg_16.png"; break;
            //                case "txt": candidate = "txt_16.png"; break;
            //                //case "ppt": candidate = "thumb_powerpoint.png"; break;
            //                //case "zip": candidate = "thumb_zip.png"; break;
            //                //case "mov": candidate = "thumb_mov.png"; break;
            //                //case "wmv": candidate = "thumb_wmv.png"; break;
            //                default: item.Title = string.Format("{0} ({1})", item.Title, entry.Extention); break;
            //            }
            //            item.Icon = string.Format("<img alt=\"\" src=\"{0}/images/icons/{1}\"/>", wim.Console.WimRepository, candidate);
            //        }
            //        else
            //        {
            //            //if (!Sushi.Mediakiwi.Data.Asset.HasCloudSetting)
            //            //    entry.CreateThumbnail();
                        
            //            string candidate = string.Concat("thumb_unknown.", ext);

            //            if (entry.IsImage)
            //            {
            //                if (Sushi.Mediakiwi.Data.Asset.HasCloudSetting || entry.Exists || entry.IsRemote)
            //                    item.Icon = entry.ThumbnailPath;
            //                else
            //                    item.Icon = string.Concat(wim.Console.WimRepository, "/images/", candidate);
            //            }
            //            else
            //            {
            //                if (!string.IsNullOrEmpty(entry.Extention))
            //                {
            //                    switch (entry.Extention.ToLower())
            //                    {
            //                        case "doc": candidate = "thumb_word.png"; break;
            //                        case "pdf": candidate = "thumb_acrobat.png"; break;
            //                        case "xls": candidate = "thumb_excel.png"; break;
            //                        case "ppt": candidate = "thumb_powerpoint.png"; break;
            //                        case "zip": candidate = "thumb_zip.png"; break;
            //                        case "mov": candidate = "thumb_mov.png"; break;
            //                        case "wmv": candidate = "thumb_wmv.png"; break;
            //                    }
            //                }
            //                item.Icon = string.Concat(wim.Console.WimRepository, "/images/", candidate);
            //            }
            //        }

            //        if (wim.Console.IsNewDesign)
            //        {
            //            item.Info1 = string.Format("<a class=\"file\" href=\"{0}?asset={1}\"><figure class=\"flaticon solid B9\"></figure><strong>{2}</strong></a>"
            //                , wim.Console.WimPagePath
            //                , entry.ID
            //                , entry.Title
            //                );
            //        }
            //        else
            //        {
            //            if (entry.Size > 0)
            //                item.Info1 = string.Format("{0} KB", entry.Size / 1024);
            //            else
            //                item.Info1 = "0 KB";
            //        }

            //        item.Info2 = entry.Type;
            //        item.Info3 = entry.Created;

            //        if (entry.IsRemote || !string.IsNullOrEmpty(entry.RemoteLocation) || entry.Exists)
            //            item.Info4 = Utility.GetIconImageString(Utility.IconImage.File, Utility.IconSize.Normal, null, entry.DownloadFullUrl);
            //        else
            //            item.Info4 = Utility.GetIconImageString(Utility.IconImage.NoFile, Utility.IconSize.Normal, null);

            //        item.HiddenField = string.Format("{0} ({1})", item.Title, item.Info1);

            //        list.Add(item);
            //    }
            //}

            //wim.ListData = list;
            //return;
        }
    }
}