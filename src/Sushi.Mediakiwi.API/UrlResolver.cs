using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.API
{
    public class UrlResolver
    {
        public int? FolderID { get; set; }
        public int? ItemID { get; set; }
        public int? ListID { get; set; }
        public int? PageID { get; set; }
        public int? AssetID { get; set; }
        public int? SectionID { get; set; }
        public int? SiteID { get; set; }
        public int? DashboardID { get; set; }
        public int? GroupID { get; set; }
        public Dictionary<string, Microsoft.Extensions.Primitives.StringValues> Query { get; set; } = new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>();

        public RequestItemType ItemType { get; set; } = RequestItemType.Undefined;

        private Data.Folder m_Folder;
        public Data.Folder Folder
        {
            get
            {
                if (m_Folder == null && FolderID.GetValueOrDefault(0) > 0)
                {
                    m_Folder = Data.Folder.SelectOne(FolderID.Value);
                }
                return m_Folder;
            }
            private set 
            {
                m_Folder = value;
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
            }
        }

        private Framework.IComponentListTemplate m_ListInstance;
        public Framework.IComponentListTemplate ListInstance
        {
            get { return m_ListInstance; }
            private set { m_ListInstance = value; }
        }


        private IServiceProvider services { get; set; }

        public UrlResolver(IServiceProvider _services)
        {
            services = _services;
        }

        public async Task ResolveUrlAsync(string uriScheme, HostString uriHost, PathString uriPathBase, PathString uriPath, string query)
        {
            // Construct Absolute URL
            string fullUrl = $"{uriScheme}://{uriHost.ToUriComponent()}";
            if (uriPathBase.HasValue) 
            {
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

            //  Verify paging page
            //_Console.ListPagingValue = _Console.Request.Query["set"];
            //_Console.Group = Utility.ConvertToIntNullable(_Console.Request.Query["group"]);
            //_Console.GroupItem = Utility.ConvertToIntNullable(_Console.Request.Query["groupitem"]);
            
            //  Verify page request
            if (Query.ContainsKey("page"))
            {
                PageID = Utils.ConvertToInt(Query["page"], 0);
                if (Page?.FolderID > 0 && FolderID.GetValueOrDefault(0) == 0)
                {
                    FolderID = Page.FolderID;
                }
                ItemType = RequestItemType.Page;
            }

            //  Verify asset request
            if (Query.ContainsKey("asset"))
            {
                AssetID = Utils.ConvertToInt(Query["asset"], 0);
                if (Asset?.GalleryID > 0 && FolderID.GetValueOrDefault(0) == 0)
                {
                    FolderID = Asset.GalleryID;
                }
                ItemType = RequestItemType.Asset;
            }

            //  Verify dashboard request
            if (Query.ContainsKey("dashboard"))
            {
                DashboardID = Utils.ConvertToInt(Query["dashboard"], 0);
                ItemType = RequestItemType.Dashboard;
            }

            //  Verify list-item request
            if (Query.ContainsKey("item"))
            {
                ItemID = Utils.ConvertToInt(Query["item"], 0);
                ItemType = RequestItemType.Item;
            }

            if (Query.ContainsKey("list"))
            {
                ListID = Utils.ConvertToInt(Query["list"], 0);
                if (List?.FolderID > 0 && FolderID.GetValueOrDefault(0) == 0)
                {
                    FolderID = List.FolderID;
                }
            }

            if (Query.ContainsKey("folder") && FolderID.GetValueOrDefault(0) == 0)
            {
                FolderID = Utils.ConvertToInt(Query["folder"], 0);
            }

            if (Query.ContainsKey("top"))
            {
                SectionID = Utils.ConvertToInt(Query["top"], 0);
            }

            if (Query.ContainsKey("group"))
            {
                GroupID = Utils.ConvertToInt(Query["group"], 0);
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
                var listinstance = Utils.CreateInstance(List.AssemblyName, List.ClassName, services);

                if (listinstance is Framework.IComponentListTemplate m_instance)
                {
                    //if (httpContext != null)
                    //{
                    //    m_instance.Init(httpContext);
                    //}
                    m_instance.wim.CurrentList = List;

                    ListInstance = m_instance;
                }
            }
        }
    }
}
