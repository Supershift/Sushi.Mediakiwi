using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Microsoft.Extensions.DependencyInjection;

namespace Sushi.Mediakiwi.API
{
    public class UrlResolver
    {
        #region Properties

        private readonly IServiceProvider _services;

        private readonly Beta.GeneratedCms.Console _console;
        public UrlBuilder UrlBuild { get; set; }

        private static Regex _CleanFormatting = new Regex(@"\/.", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private string Domain { get; set; }
        private PathString? PathBase { get; set; }
        public int? FolderID { get; set; }
        public object ItemObject { get; set; }
        public int? ItemID { get; set; }
        public int? ListID { get; set; }
        public int? PageID { get; set; }
        public int? AssetID { get; set; }
        public int? SectionID { get; set; }
        public int? SiteID { get; set; }
        public int? DashboardID { get; set; }
        public int? GroupID { get; set; }
        public int? GroupItemID { get; set; }
        public int? Group2ID { get; set; }
        public int? Group2ItemID { get; set; }
        public int? GalleryID { get; set; }
        public int? BaseID { get; set; }
        public string SelectedTab { get; set; }
        public int? OpenInFrame { get; set; }
        public string ReferID { get; set; }

        public Dictionary<string, Microsoft.Extensions.Primitives.StringValues> Query { get; set; } = new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>();

        public RequestItemType ItemType { get; set; } = RequestItemType.Undefined;

        private Framework.IComponentListTemplate m_ListInstance;
        public Framework.IComponentListTemplate ListInstance
        {
            get { return m_ListInstance; }
            private set { m_ListInstance = value; }
        }

        public Data.IApplicationUser ApplicationUser { get; internal set; }

        #endregion

        #region Calculated Properties

        string m_Channel;
        internal string Channel
        {
            get
            {
                if (m_Channel == null)
                {
                    if (!Data.Environment.Current.DefaultSiteID.GetValueOrDefault().Equals(SiteID.GetValueOrDefault(0)))
                    {
                        m_Channel = Utils.ToUrl(Site.Name);
                    }
                    else
                    {
                        m_Channel = string.Empty;
                    }
                }
                return m_Channel;
            }
        }

        /// <summary>
        /// Gets or sets the wim page path.
        /// </summary>
        /// <value>The wim page path.</value>
        public string WimPagePath
        {
            get
            {
                // set the correct wim page
                return (Channel != null && Channel.Any())
                    ? AddApplicationPath(string.Concat(CommonConfiguration.PORTAL_PATH, "/", Channel))
                    : AddApplicationPath(CommonConfiguration.PORTAL_PATH)
                    ;
            }
        }

        public string GetWimPagePath(int? siteId)
        {
            if (siteId.HasValue)
            {
                var candidate = Data.Site.SelectOne(siteId.Value);
                if (candidate != null)
                {
                    return AddApplicationPath(string.Concat(CommonConfiguration.PORTAL_PATH, "/", Utils.ToUrl(candidate.Name)), true);
                }
            }

            return WimPagePath;
        }

        public async Task<string> GetWimPagePathAsync(int? siteId)
        {
            if (siteId.HasValue)
            {
                var candidate = await Data.Site.SelectOneAsync(siteId.Value).ConfigureAwait(false);
                if (candidate != null)
                {
                    return AddApplicationPath(string.Concat(CommonConfiguration.PORTAL_PATH, "/", Utils.ToUrl(candidate.Name)), true);
                }
            }

            return WimPagePath;
        }

        private Data.Site m_Site;
        public Data.Site Site
        {
            get
            {
                if (m_Site == null && SiteID.GetValueOrDefault(0) > 0)
                {
                    m_Site = Data.Site.SelectOne(SiteID.Value);
                }
                return m_Site;
            }
            private set
            {
                m_Site = value;
            }
        }

        private Data.Folder m_Folder;
        public Data.Folder Folder
        {
            get
            {
                if (m_Folder == null && FolderID.GetValueOrDefault(0) > 0)
                {
                    m_Folder = Data.Folder.SelectOne(FolderID.Value);
                }
                //else if (m_Folder == null && GalleryID.GetValueOrDefault(0) > 0)
                //{
                //    m_Folder = Data.Folder.SelectOne(GalleryID.Value,);
                //}

                return m_Folder;
            }
            private set
            {
                m_Folder = value;
            }
        }

        private Data.Gallery m_Gallery;
        public Data.Gallery Gallery
        {
            get
            {
                if (m_Gallery == null && GalleryID.GetValueOrDefault(0) > 0)
                {
                    m_Gallery = Data.Gallery.SelectOne(GalleryID.Value);
                }
                else if (m_Gallery == null && Folder?.Type == Data.FolderType.Gallery)
                {
                    m_Gallery = Data.Gallery.SelectOne(Folder.ID);
                }

                return m_Gallery;
            }
            private set
            {
                m_Gallery = value;
            }
        }

        private Data.Page m_Page;
        public Data.Page Page
        {
            get
            {
                if (m_Page == null && PageID.GetValueOrDefault(0) > 0)
                {
                    m_Page = Data.Page.SelectOne(PageID.Value);
                }
                return m_Page;
            }
            private set
            {
                m_Page = value;
            }
        }

        private Data.Asset m_Asset;
        public Data.Asset Asset
        {
            get
            {
                if (m_Asset == null && AssetID.GetValueOrDefault(0) > 0)
                {
                    m_Asset = Data.Asset.SelectOne(AssetID.Value);
                }
                return m_Asset;
            }
            private set
            {
                m_Asset = value;
            }
        }

        private Data.IComponentList m_List;
        public Data.IComponentList List
        {
            get
            {
                if (m_List == null && ListID.GetValueOrDefault(0) > 0)
                {
                    m_List = Data.ComponentList.SelectOne(ListID.Value);
                }
                return m_List;
            }
            private set
            {
                m_List = value;
                if (value?.ID > 0)
                {
                    ListID = value.ID;
                }
            }
        }


        #endregion Calculated Properties

        #region CTor

        public UrlResolver(IServiceProvider services, Beta.GeneratedCms.Console console)
        {
            _services = services;
            if (console != null)
            {
                _console = console;
            }

            var accessor = services.GetService<IHttpContextAccessor>();
            if (accessor != null)
            {
                // if we have an ApplicationUser assign it to ApplicationUser property
                if (accessor?.HttpContext?.Items?.ContainsKey(Common.API_USER_CONTEXT) == true && accessor.HttpContext.Items[Common.API_USER_CONTEXT] is Data.IApplicationUser mkUser)
                {
                    ApplicationUser = mkUser;
                }
            }

            UrlBuild = new UrlBuilder(this);
        }

        public UrlResolver(IServiceProvider services) : this(services, null)
        {
            
        }

        #endregion CTor

        #region Add Application Path

        public string AddApplicationPath(string path, bool appendUrl = false)
        {
            if (!string.IsNullOrWhiteSpace(path))
            {
                if (path.StartsWith("http", StringComparison.CurrentCultureIgnoreCase))
                {
                    return path;
                }

                if (path.StartsWith('~'))
                {
                    path = path.Replace("~", PathBase, StringComparison.CurrentCultureIgnoreCase);
                    if (appendUrl)
                    {
                        return string.Concat(Domain, path);
                    }
                    return path;
                }
                else if (!path.StartsWith('/'))
                {
                    // expect a relative path
                    path = $"/{path}";
                }
            }

            var prefix = PathBase.HasValue ? PathBase.Value.ToString() : string.Empty;
            var url = string.Concat(prefix, path);

            if (url.Contains("//", StringComparison.CurrentCulture))
            {
                url = _CleanFormatting.Replace(url, "/");
            }

            if (appendUrl)
            {
                url = $"{Domain}{url}";
            }
            return url;
        }

        #endregion Add Application Path

        #region Resolve URL Async

        public async Task ResolveUrlAsync(string uriScheme, HostString uriHost, PathString uriPathBase, PathString uriPath, string query)
        {
            Domain = $"{uriScheme}://{uriHost.ToUriComponent()}";

            // Construct Absolute URL
            string fullUrl = $"{uriScheme}://{uriHost.ToUriComponent()}";
            if (uriPathBase.HasValue) 
            {
                PathBase = uriPathBase;
                fullUrl += uriPathBase.ToUriComponent();
            }

            if (uriPath.HasValue)
            {
                fullUrl += uriPath.ToUriComponent();
            }

            if (string.IsNullOrWhiteSpace(query) == false)
            {
                fullUrl += $"?{query.TrimStart('?')}";
            }

            // Create a UriBuilder for the supplied URL
            Uri uri = new Uri(fullUrl, UriKind.Absolute);
            try
            {
                // Create a QueryHelper for the supplied URL Query
                if (string.IsNullOrWhiteSpace(uri?.Query) == false)
                {
                    Query = QueryHelpers.ParseQuery(uri.Query);
                }

            }
            catch (Exception ex)
            {

                throw;
            }
    
            // For keeping track of any named list in the URL
            string targetName = string.Empty;
            string relativeUrl = uriPath.HasValue ? uriPath.ToUriComponent() : uri.AbsolutePath;

            // extract url details
            var wim = CommonConfiguration.PORTAL_PATH.ToLower();
            var split =
                wim == "/"
                    ? relativeUrl.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries)
                    : relativeUrl.Replace(wim, string.Empty).Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

            // set the channel identifier
            if (split.Any())
            {
                // position counter
                int n = 0;

                var candidate = Utils.FromUrl(split[0]);
                if (Data.Utility.IsNumeric(candidate))
                {
                    SiteID = Utils.ConvertToInt(candidate, Data.Environment.Current.DefaultSiteID.GetValueOrDefault());
                }
                else
                {
                    var allSites = await Data.Site.SelectAllAsync().ConfigureAwait(false);
                    var selection = allSites.FirstOrDefault(x => x.Name.Equals(candidate, StringComparison.CurrentCultureIgnoreCase));
                    if (selection != null)
                    {
                        n++;
                        SiteID = selection.ID;
                    }
                }

                if (SiteID.GetValueOrDefault(0) == 0)
                {
                    SiteID = Data.Environment.Current.DefaultSiteID.GetValueOrDefault();
                    if (SiteID.GetValueOrDefault(0) == 0)
                    {
                        var allSites = await Data.Site.SelectAllAsync().ConfigureAwait(false);
                        SiteID = allSites[0].ID;
                    }
                }

                // [wim-path]/[site:0]/[type:1]/[folder:x>y]/[list:y+1]
                // type can be web or assets else it is a list
                if (split.Length > 1)
                {
                    var typecandidate = split[1];
                    if (typecandidate.Equals("web", StringComparison.CurrentCultureIgnoreCase))
                    {
                        n++;
                        ItemType = RequestItemType.Page;
                    }
                    else if (typecandidate.Equals("assets", StringComparison.CurrentCultureIgnoreCase))
                    {
                        n++;
                        ItemType = RequestItemType.Asset;
                    }
                    else
                    {
                        ItemType = RequestItemType.Item;
                    }
                }

                if (split.Length - n - 1 > 0)
                {
                    var completepath = string.Concat("/", string.Join("/", split.Skip(n).Take(split.Length - n - 1)), "/");
                    Folder = await Data.Folder.SelectOneAsync(completepath, SiteID.Value).ConfigureAwait(false);
                }

                if (split.Length - n > 0)
                {
                    targetName = split.Last();
                }
            }
            else
            {
                if (SiteID.GetValueOrDefault(0) == 0)
                {
                    SiteID = Data.Environment.Current.DefaultSiteID.GetValueOrDefault();
                    if (SiteID == 0)
                    {
                        var allSites = await Data.Site.SelectAllAsync().ConfigureAwait(false);
                        SiteID = allSites[0].ID;
                    }
                }
            }

            //  Verify page request
            if (Query.ContainsKey("page"))
            {
                PageID = Utils.ConvertToInt(Query["page"].FirstOrDefault(), 0);
                if (Page?.FolderID > 0 && FolderID.GetValueOrDefault(0) == 0)
                {
                    FolderID = Page.FolderID;
                }
                ItemType = RequestItemType.Page;
            }

            //  Verify asset request
            if (Query.ContainsKey("asset"))
            {
                AssetID = Utils.ConvertToInt(Query["asset"].FirstOrDefault(), 0);
                if (Asset?.GalleryID > 0 && FolderID.GetValueOrDefault(0) == 0)
                {
                    GalleryID = Asset.GalleryID;
                }
                ItemType = RequestItemType.Asset;
            }

            //  Verify gallery request
            if (Query.ContainsKey("gallery"))
            {
                GalleryID = Utils.ConvertToInt(Query["gallery"].FirstOrDefault(), 0);
                ItemType = RequestItemType.Asset;
            }

            //  Verify dashboard request
            if (Query.ContainsKey("dashboard"))
            {
                DashboardID = Utils.ConvertToInt(Query["dashboard"].FirstOrDefault(), 0);
                ItemType = RequestItemType.Dashboard;
            }

            //  Verify list-item request
            if (Query.ContainsKey("item"))
            {
                ItemObject = Query["item"].FirstOrDefault();
                if (int.TryParse(ItemObject.ToString(), out int itemId))
                {
                    ItemID = itemId;
                }
                ItemType = RequestItemType.Item;
            }

            if (Query.ContainsKey("list"))
            {
                ListID = Utils.ConvertToInt(Query["list"].FirstOrDefault(), 0);
                if (List?.FolderID > 0 && FolderID.GetValueOrDefault(0) == 0)
                {
                    FolderID = List.FolderID;
                }
            }

            if (Query.ContainsKey("folder") && FolderID.GetValueOrDefault(0) == 0)
            {
                FolderID = Utils.ConvertToInt(Query["folder"].FirstOrDefault(), 0);
            }

            if (Query.ContainsKey("top"))
            {
                SectionID = Utils.ConvertToInt(Query["top"].FirstOrDefault(), 0);
            }

            if (Query.ContainsKey("group"))
            {
                GroupID = Utils.ConvertToInt(Query["group"].FirstOrDefault(), 0);
            }

            if (Query.ContainsKey("groupitem"))
            {
                GroupItemID = Utils.ConvertToInt(Query["groupitem"].FirstOrDefault(), 0);
            }

            if (Query.ContainsKey("group2"))
            {
                Group2ID = Utils.ConvertToInt(Query["group2"].FirstOrDefault(), 0);
            }

            if (Query.ContainsKey("group2item"))
            {
                Group2ItemID = Utils.ConvertToInt(Query["group2item"].FirstOrDefault(), 0);
            }

            if (Query.ContainsKey("base"))
            {
                BaseID = Utils.ConvertToInt(Query["base"].FirstOrDefault(), 0);
            }

            if (Query.ContainsKey("tab"))
            {
                SelectedTab = Query["tab"].FirstOrDefault();
            }

            if (Query.ContainsKey("openinframe"))
            {
                OpenInFrame = Utils.ConvertToInt(Query["openinframe"].FirstOrDefault());
            }

            if (Query.ContainsKey("referid"))
            {
                ReferID = Query["referid"].FirstOrDefault();
            }

            if (ListID.GetValueOrDefault(0) == 0 && !string.IsNullOrWhiteSpace(targetName))
            {
                var urldecrypt = Utils.FromUrl(targetName);
                var list = await Data.ComponentList.SelectOneAsync(urldecrypt, null).ConfigureAwait(false);
                if (list != null && !list.IsNewInstance)
                {
                    List = list;
                    if (List?.FolderID > 0 && FolderID.GetValueOrDefault(0) == 0)
                    {
                        FolderID = List.FolderID;
                    }
                }
            }

            // When nothing is selected, this is probably the landing page
            if (FolderID.GetValueOrDefault(0) == 0)
            {
                List = await Data.ComponentList.SelectOneAsync(typeof(AppCentre.Data.Implementation.Browsing)).ConfigureAwait(false);
            }

            // Set ListInstance
            if (ListID.GetValueOrDefault(0) > 0)
            {
                //
                var listinstance = List.GetInstance(_console);
                // var listinstance = Utils.CreateInstance(List.AssemblyName, List.ClassName, _services);

                if (listinstance is Framework.IComponentListTemplate m_instance)
                {
                    if (_console != null)
                    {
                        // Set itemID to Console
                        if (ItemID.HasValue)
                        {
                            _console.Item = ItemID.Value;
                        }

                        m_instance.Init(_console);
                        m_instance.wim.Console = _console;
                        m_instance.wim.CurrentList = List;
                        m_instance.wim.DoListInit();

                        // Determine Edit state
                        if (ItemID.HasValue || ItemObject != null)
                        {
                            var isEditPostBack = false;
                            var isSavePostBack = false;
                            var isDeletePostBack = false;

                            if (_console.Request.Method == HttpMethods.Post && _console.Request.Headers.ContainsKey("postedField"))
                            {
                                isEditPostBack = _console.Request.Headers["postedField"] == "edit";
                                isSavePostBack = _console.Request.Headers["postedField"] == "save";
                                isDeletePostBack = _console.Request.Headers["postedField"] == "delete";
                            }

                            //  Is the form in editmode?
                            m_instance.wim.IsEditMode = isEditPostBack
                                || isSavePostBack
                                || m_instance.wim.OpenInEditMode
                                || ItemID.GetValueOrDefault(-1) == 0
                                || ItemObject != null
                                || AssetID.GetValueOrDefault(-1) == 0
                                || (_console.JsonReferrer != null && _console.JsonReferrer.Equals("edit"))
                                || _console.JsonForm != null;


                            // Is the form in Save Mode
                            m_instance.wim.IsSaveMode = isSavePostBack;

                            // Is the form in delete mode
                            m_instance.wim.IsDeleteMode = isDeletePostBack;
                        }
                    }

                    m_instance.wim.CurrentList = List;
                    m_instance.wim.CurrentSite = Site;
                  
                    Folder = m_instance.wim.CurrentFolder;
                    ListInstance = m_instance;
                }
            }
        }

        #endregion Resolve URL Async
    }
}
