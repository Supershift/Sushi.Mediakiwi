using Sushi.Mediakiwi.Data;
using Sushi.Mediakiwi.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.AppCentre.Data.Implementation
{
    /// <summary>
    /// Represents a Browsing entity.
    /// </summary>
    public partial class Browsing : BaseImplementation
    {
        #region Properties

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

        /// <summary>
        /// Gets or sets the filter title.
        /// </summary>
        /// <value>The filter title.</value>
        //[Sushi.Mediakiwi.Framework.OnlyVisibleWhenTrue("IsGalleryOrHasRoot")]
        [Framework.ContentListSearchItem.TextField("_search_for", 50)]
        public string FilterTitle { get; set; }

        public bool FilterPath { get; set; }

        int _columns = 0;
        bool m_IsGallerySet;
        bool m_IsGallery;
        /// <summary>
        /// Gets or sets a value indicating whether  instance is gallery.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if  instance is gallery; otherwise, <c>false</c>.
        /// </value>
        public bool IsGallery
        {
            get
            {
                if (!m_IsGallerySet)
                {
                    m_IsGallerySet = true;
                    m_IsGallery = (wim.CurrentFolder.Type == FolderType.Gallery);
                }
                return m_IsGallery;
            }
            set { m_IsGallery = value; }
        }


        /// <summary>
        /// Gets a value indicating whether  instance is page.
        /// </summary>
        /// <value><c>true</c> if  instance is page; otherwise, <c>false</c>.</value>
        public bool IsPage
        {
            get
            {
                return (wim.CurrentFolder.Type == FolderType.Page);
            }
        }

        /// <summary>
        /// Gets a value indicating whether  instance is gallery or has root.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if  instance is gallery or has root; otherwise, <c>false</c>.
        /// </value>
        public bool IsGalleryOrHasRoot
        {
            get
            {
                if (IsGallery && string.IsNullOrEmpty(Request.Query["root"]))
                {
                    return true;
                }

                if (wim.CurrentFolder.Type == FolderType.Page)
                {
                    return true;
                }

                return false;
            }
        }
        #endregion Properties

        #region CTor

        /// <summary>
        /// Initializes a new instance of the <see cref="Browsing"/> class.
        /// </summary>
        public Browsing()
        {
            FilterPath = false;
            wim.HideProperties = true;

            ListDataItemCreated += Browsing_ListDataItemCreated;
            ListAction += Browsing_ListAction;
            ListSearch += Browsing_ListSearch;
            ListLoad += Browsing_ListLoad;
            ListSave += Browsing_ListSave;
        }

        private void Browsing_ListDataItemCreated(object sender, ListDataItemCreatedEventArgs e)
        {
            if (e.Item is BrowseItem item && item.Attributes?.Count > 0 && e.Type == DataItemType.TableRow)
            {
                foreach (var att in item.Attributes)
                {
                    if (att.Key.Equals("class", StringComparison.InvariantCultureIgnoreCase))
                    {
                        e.Attribute.Class = att.Value;
                    }
                    else if (att.Key.Equals("style", StringComparison.InvariantCultureIgnoreCase))
                    {
                        e.Attribute.Style.Add(att.Value.Split(':')[0], att.Value.Split(':')[1]);
                    }
                    else if (att.Key.Equals("id", StringComparison.InvariantCultureIgnoreCase))
                    {
                        e.Attribute.ID = att.Value;
                    }
                    else
                    {
                        e.Attribute.Add(att.Key, att.Value);
                    }
                }
            }
        }

        private Task Browsing_ListSave(ComponentListEventArgs arg)
        {
            return Task.CompletedTask;
        }

        private Task Browsing_ListLoad(ComponentListEventArgs arg)
        {
            return Task.CompletedTask;
        }

        #endregion CTor

        #region List Search

        async Task Browsing_ListSearch(ComponentListSearchEventArgs arg)
        {
            wim.HideSearchButton = true;

            wim.SetPropertyVisibility("Parent", false);

            await ShowBrowsingAsync().ConfigureAwait(false);
        }

        #endregion List Search

        #region List Action

        /// <summary>
        /// Handles the ListAction event of the Browsing control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ComponentActionEventArgs"/> instance containing the event data.</param>
        async Task Browsing_ListAction(ComponentActionEventArgs e)
        {
            int galleryId = Utility.ConvertToInt(Request.Query["gallery"]);
            if (galleryId > 0)
            {
                Gallery gallery = await Gallery.SelectOneAsync(galleryId).ConfigureAwait(false);
                if (!(gallery != null && gallery.ID == 0))
                {
                    IComponentList list = await Mediakiwi.Data.ComponentList.SelectOneAsync(ComponentListType.Documents).ConfigureAwait(false);
                    Response.Redirect(wim.Console.WimPagePath + "&item=0&list=" + list.ID);
                }
            }
        }

        #endregion List Action

        #region Show Browsing Async

        async Task ShowBrowsingAsync()
        {
            wim.CurrentList.Option_Search_MaxResultPerPage = 512;
            wim.SearchResultItemPassthroughParameterProperty = nameof(BrowseItem.PassThrough);

            wim.ListDataColumns.Add(new ListDataColumn("ID", nameof(BrowseItem.ID), ListDataColumnType.UniqueIdentifier));
            wim.ListDataColumns.Add(new ListDataColumn("", nameof(BrowseItem.Icon)) { ColumnWidth = 20 });
            wim.ListDataColumns.Add(new ListDataColumn(Labels.ResourceManager.GetString("list_name", new CultureInfo(wim.CurrentApplicationUser.LanguageCulture)), nameof(BrowseItem.Title), ListDataColumnType.HighlightPresent));
            wim.ListDataColumns.Add(new ListDataColumn("", nameof(BrowseItem.Info1)) { ColumnWidth = 20 });

            List<BrowseItem> list = new List<BrowseItem>();

            bool isSearchInitiate = !string.IsNullOrEmpty(FilterTitle);
            if (wim.CurrentFolder.Type == FolderType.Gallery)
            {
                list = await GetGalleryListAsync(isSearchInitiate).ConfigureAwait(false);
            }
            else if (wim.CurrentFolder.Type == FolderType.List || wim.CurrentFolder.Type == FolderType.Administration)
            {
                await GetListListAsync().ConfigureAwait(false);
            }
            else if (wim.CurrentFolder.Type == FolderType.Page)
            {
               await GetPageListAsync(isSearchInitiate).ConfigureAwait(false);
            }

            wim.ListDataAdd(list);
        }

        #endregion Show Browsing Async

        #region Get Status

        string GetStatus(bool isEdited, bool isPublished, bool isSearchable, bool hasMaster, bool isLocalisedEditMode, bool isLocalisedPublicationMode)
        {
            string status = string.Empty;

            if (isPublished)
            {
                status += "<span class=\"icon-globe abbr right\" title=\"Published\"></span>";
            }

            if (hasMaster)
            {
                if (!isLocalisedEditMode && !isLocalisedPublicationMode)
                {
                    status += "<span class=\"icon-download-01 abbr right\" title=\"Inherited page\"></span>";
                }
                else if (!isLocalisedEditMode && isLocalisedPublicationMode)
                {
                    status += "<span class=\"icon-download-01 abbr right inactive\" title=\"Inherited page (only edit)\"></span>";
                }
                else if (isLocalisedEditMode && !isLocalisedPublicationMode)
                {
                    status += "<span class=\"icon-download-01 abbr right inactive\" title=\"Inherited page (only publication)\"></span>";
                }
            }

            if (isEdited)
            {
                status += "<span class=\"icon-pen abbr right\" title=\"Edited\"></span>";
            }

            if (!isSearchable)
            {
                status += "<span class=\"icon-search-minus abbr right\" title=\"Searchable\"></span>";
            }

            return status;
        }

        #endregion Get Status

        #region Get Sort

        PageSortBy GetSort(int? sorderOrderMethod)
        {
            return sorderOrderMethod switch
            {
                1 => PageSortBy.CustomDate,
                2 => PageSortBy.CustomDateDown,
                3 => PageSortBy.LinkText,
                4 => PageSortBy.Name,
                _ => PageSortBy.SortOrder,
            };
        }

        #endregion Get Sort
        
        #region Get Page List

        async Task GetPageListAsync(bool isSearchInitiate)
        {
            wim.Page.Body.ShowInFullWidthMode = false;

            _columns = 1;

            #region Folder navigation

            Mediakiwi.Data.Folder[] folders = null;

            bool isRootLevelView = false;
            if (isSearchInitiate || isRootLevelView)
            {
                isRootLevelView = true;
                folders = await Mediakiwi.Data.Folder.SelectAllAsync(wim.CurrentFolder.Type, wim.CurrentSite.ID, FilterTitle, FilterPath).ConfigureAwait(false);
            }
            else
            {
                folders = await Mediakiwi.Data.Folder.SelectAllByParentAsync(wim.CurrentFolder.ID, wim.CurrentFolder.Type, false).ConfigureAwait(false);
            }

            //  ACL determination
            folders = await Mediakiwi.Data.Folder.ValidateAccessRightAsync(folders, wim.CurrentApplicationUser).ConfigureAwait(false);

            if (wim.CurrentFolder.Level == 0 && folders.Length == 0 && !isRootLevelView && !isSearchInitiate)
            {
                isRootLevelView = true;
                folders = await Mediakiwi.Data.Folder.SelectAllAsync(wim.CurrentFolder.Type, wim.CurrentSite.ID, FilterTitle, FilterPath).ConfigureAwait(false);
                //  ACL determination
                folders = await Mediakiwi.Data.Folder.ValidateAccessRightAsync(folders, wim.CurrentApplicationUser).ConfigureAwait(false);
            }

            #endregion Folder navigation

            IEnumerable<Page> pages;
            if (!isSearchInitiate)
            {
                pages = await Page.SelectAllAsync(wim.CurrentFolder.ID, PageFolderSortType.Folder, PageReturnProperySet.All, GetSort(wim.CurrentFolder.ID), false).ConfigureAwait(false);
            }
            else
            {
                pages = await Page.SelectAllAsync(FilterTitle, FilterPath).ConfigureAwait(false);
            }

            pages = await Page.ValidateAccessRightAsync(pages, wim.CurrentApplicationUser).ConfigureAwait(false);
            StringBuilder build = new StringBuilder();

            if (pages.Any() == false && (folders.Length == 0 || (folders.Length == 1 && folders[0].Name == "/")))
            {
                return;
            }

            build.Append("<div class=\"widget\">");

            if (pages.Any())
            {
                foreach (var entry in pages)
                {
                    var url = wim.Console.WimPagePath;
                    var status = GetStatus(entry.IsEdited, entry.IsPublished, entry.IsSearchable, entry.MasterID.HasValue, entry.InheritContentEdited, entry.InheritContent);
                    var published = (entry.IsPublished ? string.Empty : " class=\"inactive\"");

                    build.Append($"<a href=\"{url}?page={entry.ID}\"{published}>{entry.Name}{status}</a>");
                }
                build.Append("</div>");
            }

            await FindPageAsync(folders, build).ConfigureAwait(false);

            #region Close column view

            build.Append("</article>");
            build.Append("</section>");
            build.Insert(0, $"<article class=\"column-{(_columns > 3 ? 3 : _columns)}\">");
            build.Insert(0, "<section id=\"startWidgets\" class=\"component widgets\">");
            wim.Page.Body.Grid.Add(build.ToString(), true);

            #endregion Close column view
        }

        #endregion Get Page List

        #region Find Page Async


        async Task FindPageAsync(Mediakiwi.Data.Folder[] folders, StringBuilder build)
        {
            foreach (Mediakiwi.Data.Folder entry in folders)
            {
                if (!entry.IsVisible && !wim.CurrentApplicationUser.ShowHidden)
                {
                    continue;
                }

                //  No root elements
                if (entry.Name == "/")
                {
                    continue;
                }

                IEnumerable<Page> pages = await Page.SelectAllAsync(entry.ID, PageFolderSortType.Folder, PageReturnProperySet.All, GetSort(entry.SortOrderMethod), false).ConfigureAwait(false);
                pages = await Page.ValidateAccessRightAsync(pages, wim.CurrentApplicationUser).ConfigureAwait(false);

                _columns++;

                build.Append("<div class=\"widget\">");
                build.Append($"<h2><a href=\"{wim.Console.WimPagePath}?folder={entry.ID}\" title=\"{entry.CompletePath}\">{entry.Name}</a></h2>");
                if (!string.IsNullOrEmpty(entry.Description))
                {
                    build.Append($"<p>{entry.Description}</p>");
                }

                if (pages.Any())
                {
                    foreach (var i in pages)
                    {
                        var url = wim.Console.WimPagePath;
                        var status = GetStatus(i.IsEdited, i.IsPublished, i.IsSearchable, i.MasterID.HasValue, i.InheritContentEdited, i.InheritContent);
                        var published = (i.IsPublished ? string.Empty : " class=\"inactive\"");

                        build.Append($"<a href=\"{url}?page={i.ID}\"{published}>{i.Name}{status}</a>");
                    }
                }

                build.Append("</div>");
                var arr = await Mediakiwi.Data.Folder.SelectAllByParentAsync(entry.ID).ConfigureAwait(false);
                await FindPageAsync(arr, build).ConfigureAwait(false);
            }
        }
        #endregion Find Page Async

        #region Get Gallery List Async

        async Task<List<BrowseItem>> GetGalleryListAsync(bool isSearchInitiate)
        {
            var isOnlySelect = Request.Query.ContainsKey("selectOnly") && Request.Query["selectOnly"].Equals("1");
            var isOnlyImages = Request.Query.ContainsKey("isimage") && Request.Query["isimage"].Equals("1");

            var list = new List<BrowseItem>();
            int baseGalleryID = wim.CurrentApplicationUserRole.GalleryRoot.GetValueOrDefault();

            var folderSettings = await Mediakiwi.Data.ComponentList.SelectOneAsync(new Guid("97292dd5-ebda-4318-8aaf-4c49e887cdad")).ConfigureAwait(false);
            wim.Page.Body.Grid.IgnoreInLayerSubSelect = isOnlySelect;
            wim.Page.HideTopIconBar = isOnlySelect;
            wim.HideSearchButton = !isOnlySelect;

            Gallery[] galleries;

            Gallery rootGallery = await Gallery.SelectOneAsync(Utility.ConvertToGuid(Request.Query["root"])).ConfigureAwait(false);
            if (!IsPostBack || string.IsNullOrEmpty(FilterTitle))
            {
                if (wim.CurrentFolder.ParentID.GetValueOrDefault(0) > 0 && wim.CurrentFolder.ID != rootGallery.ID && wim.CurrentFolder.ID != baseGalleryID)
                {
                    BrowseItem item = new BrowseItem();
                    item.ID = wim.CurrentFolder.ParentID.Value;
                    item.Title = "...";
                    item.PassThrough = "?gallery";
                    item.Icon = "<figure class=\"icon-folder icon\"></figure>";
                    if (isOnlySelect == false)
                    {
                        item.Info1 = $"<a href=\"{wim.Console.WimPagePath}?list={folderSettings.ID}&gallery={item.ID}&item={item.ID}\"><figure class=\"icon-settings-02 icon\"></figure></a>";
                    }
                    else 
                    {
                        if (Request?.Query?.ContainsKey("referid") == true)
                        {
                            item.PassThrough = $"?referid={Request.Query["referid"]}&selectOnly=1&gallery";
                        }
                        else
                        {
                            item.PassThrough = "?selectOnly=1&gallery";
                        }
                    }
                    list.Add(item);
                }

                galleries = await Gallery.SelectAllByParentAsync(wim.CurrentFolder.ID).ConfigureAwait(false);
            }
            else
            {
                galleries = await Gallery.SelectAllAsync(FilterTitle).ConfigureAwait(false);
            }

            bool isRootLevelView = false;
            if (wim.CurrentFolder.Level == 0 && galleries.Length == 0 && !isSearchInitiate)
            {
                isRootLevelView = true;
            }

            foreach (Gallery entry in galleries)
            {
                BrowseItem item = new BrowseItem();
                item.ID = entry.ID;
                item.Title = isRootLevelView ? entry.CompleteCleanPath() : entry.Name;
                item.PassThrough = "?gallery";
                item.Icon = "<figure class=\"icon-folder icon\"></figure>";
                if (isOnlySelect == false)
                {
                    item.Info1 = $"<a href=\"{wim.Console.WimPagePath}?list={folderSettings.ID}&gallery={entry.ID}&item={entry.ID}\"><figure class=\"icon-settings-02 icon\"></figure></a>";
                }
                else
                {
                    if (Request?.Query?.ContainsKey("referid") == true)
                    {
                        item.PassThrough = $"?referid={Request.Query["referid"]}&selectOnly=1&gallery";
                    }
                    else
                    {
                        item.PassThrough = "?selectOnly=1&gallery";
                    }
                }
                item.Info3 = entry.Created;
                list.Add(item);
            }

            List<Asset> assets;

            if (!IsPostBack || string.IsNullOrEmpty(FilterTitle))
            {
                assets = await Asset.SelectAllAsync(wim.CurrentFolder.ID, onlyReturnImages: isOnlyImages).ConfigureAwait(false);
            }
            else
            {
                assets = await Asset.SearchAllAsync(FilterTitle, onlyReturnImages: isOnlyImages).ConfigureAwait(false);
            }

            foreach (Asset entry in assets)
            {
                BrowseItem item = new BrowseItem();
                item.ID = entry.ID;
                item.Title = entry.Title;
                item.PassThrough = "?asset";
                if (entry.IsImage)
                {
                    item.Icon = "<figure class=\"icon-picture icon\"></figure>";
                }
                else
                {
                    item.Icon = "<figure class=\"icon-document icon\"></figure>";
                }

                if (isOnlySelect == false)
                {
                    item.Info1 = $"<a href=\"{wim.Console.WimPagePath}?asset={entry.ID}\"><figure class=\"icon-settings-02 icon\"></figure></a>";
                }
                else 
                {
                    item.Attributes.Add("class", "hand postparentnow");
                    item.Attributes.Add("data-parentlevel", "2");
                    item.Attributes.Add("value", item.Title);
                    item.Attributes.Add("id", item.ID.ToString());
                    item.Attributes.Add("data-link", string.Empty);
                    item.PassThrough = "?selectOnly=1&asset";
                }

                item.Info2 = entry.Type;
                item.Info3 = entry.Created;

                if (!string.IsNullOrEmpty(entry.RemoteLocation) || entry.Exists)
                {
                    item.Info4 = Utils.GetIconImageString(wim.Console, Utils.IconImage.File, Utils.IconSize.Normal, null, entry.DownloadFullUrl);
                }
                else
                {
                    item.Info4 = Utils.GetIconImageString(wim.Console, Utils.IconImage.NoFile, Utils.IconSize.Normal, null);
                }

                item.HiddenField = $"{item.Title} ({item.Info1})";
                list.Add(item);
            }

            return list;
        }

        #endregion Get Gallery List Async

        #region Get List List Async

        async Task GetListListAsync()
        {
            StringBuilder build = new StringBuilder();
            Mediakiwi.Data.Folder[] f = new Mediakiwi.Data.Folder[]
            {
                wim.CurrentFolder
            };

            await FindListAsync(f, build, false).ConfigureAwait(false);

            build.Append("</article>");
            build.Append("</section>");
            build.Insert(0, $"<article class=\"column-{(_columns > 3 ? 3 : _columns)}\">");
            build.Insert(0, "<section id=\"startWidgets\" class=\"component widgets\">");
            wim.Page.Body.Grid.Add(build.ToString(), true);
        }
        
        #endregion Get List List Async
        
        #region Find List Async
        
        async Task FindListAsync(Mediakiwi.Data.Folder[] folders, StringBuilder build, bool ignoreHeader)
        {
            foreach (Mediakiwi.Data.Folder entry in folders)
            {
                if (!entry.IsVisible && !wim.CurrentApplicationUser.ShowHidden)
                {
                    continue;
                }

                IComponentList[] all_lists = await Mediakiwi.Data.ComponentList.SelectAllAsync(entry.ID, wim.CurrentApplicationUser, true).ConfigureAwait(false);
                var allowed_lists = await Mediakiwi.Data.ComponentList.ValidateAccessRightAsync(all_lists, wim.CurrentApplicationUser).ConfigureAwait(false);
                IComponentList[] selected_lists = null;

                if (wim.CurrentApplicationUser.ShowHidden)
                {
                    selected_lists = allowed_lists;
                }
                else
                {
                    selected_lists = (from x in allowed_lists where x.IsVisible select x).ToArray();
                }

                if (entry.ParentID.HasValue || selected_lists.Length > 0)
                {
                    _columns++;
                    build.Append("<div class=\"widget\">");

                    if (!ignoreHeader && entry.Name != "/")
                    {
                        var path = wim.Console.WimPagePath;
                        var visibility = (entry.IsVisible ? "" : " style=\"color: #d3d3d3\"");

                        build.Append($"<h2><a href=\"{path}?folder={entry.ID}\" {visibility}>{entry.Name}</a></h2>");

                        if (!string.IsNullOrEmpty(entry.Description))
                        {
                            build.Append($"<p>{entry.Description}</p>");
                        }
                    }

                    foreach (var i in selected_lists)
                    {
                        ComponentDataReportEventArgs e = null;
                        if (i.Option_HasDataReport)
                        {
                            var instance = i.GetInstance(Context);
                            if (instance != null)
                            {
                                e = instance.wim.DoListDataReport();
                            }
                        }

                        if (e == null || !e.ReportCount.HasValue)
                        {
                            var url = wim.Console.UrlBuild.GetListRequest(i);
                            var visibility = (i.IsVisible ? string.Empty : " class=\"inactive\"");

                            build.Append($"<a href=\"{url}\"{visibility}>{i.Name}</a>");
                        }
                        else
                        {
                            var count = $"{e.ReportCount.Value}";
                            if (e.ReportCount.Value > 99)
                            {
                                count = "99+";
                            }

                            var url = wim.Console.UrlBuild.GetListRequest(i);
                            var visibility = i.IsVisible ? string.Empty : " class=\"inactive\"";
                            var alert = e.IsAlert ? " attention" : string.Empty;

                            build.Append($"<a href=\"{url}{i.Name}\"{count}>{visibility} <span class=\"items\">{alert}</span></a>");
                        }
                    }

                    build.Append("</div>");
                }

                var arr = await Mediakiwi.Data.Folder.SelectAllAccessibleByParentAsync(entry.ID, FolderType.Undefined, !wim.CurrentApplicationUser.ShowHidden, wim.CurrentApplicationUser).ConfigureAwait(false);
                await FindListAsync(arr, build, false).ConfigureAwait(false);
            }
        }

        #endregion Find List Async
    }
}