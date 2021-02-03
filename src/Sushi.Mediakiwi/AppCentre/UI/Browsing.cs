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
using Sushi.Mediakiwi.Logic;
using System.Threading.Tasks;

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
         
            this.ListLoad += Browsing_ListLoad;
            this.ListAction += Browsing_ListAction;
            this.ListSave += Browsing_ListSave;
            this.ListSearch += Browsing_ListSearch;
        }

        private Task Browsing_ListSearch(ComponentListSearchEventArgs arg)
        {
            wim.HideSearchButton = true;

            wim.SetPropertyVisibility("Parent", false);

            ShowBrowsing();
            
            return Task.CompletedTask;
        }

        Task Browsing_ListSave(ComponentListEventArgs e)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Handles the ListLoad event of the Browsing control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Sushi.Mediakiwi.Framework.ComponentListEventArgs"/> instance containing the event data.</param>
        Task Browsing_ListLoad(ComponentListEventArgs e)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Handles the ListAction event of the Browsing control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Sushi.Mediakiwi.Framework.ComponentActionEventArgs"/> instance containing the event data.</param>
        async Task Browsing_ListAction(ComponentActionEventArgs e)
        {
            int galleryId = Utility.ConvertToInt(Request.Query["gallery"]);
            if (galleryId > 0)
            {
                Sushi.Mediakiwi.Data.Gallery gallery = await Sushi.Mediakiwi.Data.Gallery.SelectOneAsync(galleryId);
                if (!(gallery != null && gallery.ID == 0))
                {
                    Sushi.Mediakiwi.Data.IComponentList list = await Sushi.Mediakiwi.Data.ComponentList.SelectOneAsync(Sushi.Mediakiwi.Data.ComponentListType.Documents);
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
            wim.ListDataAdd(list);
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

            bool isRootLevelView = false;
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
                var allowed_lists = Sushi.Mediakiwi.Data.ComponentList.ValidateAccessRight(all_lists, wim.CurrentApplicationUser);
                Sushi.Mediakiwi.Data.IComponentList[] selected_lists = null;

                if (wim.CurrentApplicationUser.ShowHidden)
                    selected_lists = allowed_lists;
                else
                    selected_lists = (from x in allowed_lists where x.IsVisible select x).ToArray();

                if (entry.ParentID.HasValue || selected_lists.Length > 0)
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
                            build.AppendFormat("<a href=\"{0}\"{2}>{1}</a>"
                                , wim.Console.UrlBuild.GetListRequest(i)
                                , i.Name
                                , i.IsVisible ? string.Empty : " class=\"inactive\""
                                );
                        }
                        else
                        {
                            string count = e.ReportCount.Value.ToString();
                            if (e.ReportCount.Value > 99)
                                count = "99+";

                            build.AppendFormat("<a href=\"{0}{1}\"{3}>{2} <span class=\"items{5}\">{4}</span></a>"
                                , wim.Console.UrlBuild.GetListRequest(i)
                                , i.Name
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
    }
}