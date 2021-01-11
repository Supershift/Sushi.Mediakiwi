using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using Sushi.Mediakiwi.Beta.GeneratedCms.Source;
using Sushi.Mediakiwi.Data;
using Sushi.Mediakiwi.Framework;
using Sushi.Mediakiwi.Interfaces;
using Sushi.Mediakiwi.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.UI
{
    public class Monitor
    {
        private IHostingEnvironment _env;
        private HttpContext _Context;
        private Beta.GeneratedCms.Console _Console;
        private iPresentationMonitor _PresentationMonitor;
        private iPresentationNavigation _PresentationNavigation;
        Dictionary<GlobalPlaceholder, string> _Placeholders;
        Dictionary<CallbackTarget, List<ICallback>> _Callbacks;
        bool IsLoadedInThinLayer;
        public StringBuilder outputHTML;
        internal WimControlBuilder GlobalWimControlBuilder;
        private readonly IConfiguration _configuration;

        public Monitor(HttpContext context, IHostingEnvironment env, IConfiguration configuration)
        {
            _env = env;
            _Context = context;
            _configuration = configuration;
            _Console = new Sushi.Mediakiwi.Beta.GeneratedCms.Console(context, _env);

            //Console.CurrentApplicationUser = user;

            _PresentationMonitor = new Framework.Presentation.Presentation();
            _PresentationNavigation = new Framework.Presentation.Logic.Navigation();
        }

        internal async Task StartAsync()
        {
            if (_env.IsDevelopment())
            {
                await StartAsync(false);
            }
            else
            {
                try
                {
                    await StartAsync(false);
                }
                catch (Exception ex)
                {
                    await Notification.InsertOneAsync("Uncaught exception", ex);
                    throw ex;
                }
            }
        }

        internal async Task StartAsync(bool reStartWithNotificationList)
        {
            //  Set the current environment
            _Console.CurrentEnvironment = Data.Environment.Current;

            var path = _Console.AddApplicationPath(CommonConfiguration.PORTAL_PATH);
            if (!_Console.Request.Path.Value.StartsWith(path, StringComparison.CurrentCultureIgnoreCase))
                return;

            _Console.SetDateFormat();

            bool forcelogin = 
                //_Console.Request.Path.Equals($"{Data.Environment.Current.RelativePath}/login", StringComparison.CurrentCultureIgnoreCase)
                _Console.Url.Contains($"{path}?reset=", StringComparison.CurrentCultureIgnoreCase)
                || _Console.Url.EndsWith($"{path}?reminder", StringComparison.CurrentCultureIgnoreCase)
                ;

            if (!await CheckRoamingApplicationUserAsync(forcelogin))
            {
                return;
            }
            _Console.SaveVisit();

            //  Obtain querystring requests
            this.SetRequestType();
            //  If an xml (ajax) request comes in output a correct response.
            if (!await this.OutputAjaxRequestAsync()) return;
            //  Define internal event-check value types
            var  isDeleteTriggered = await SetFormModesAsync();
            //  Checks and sets the current folder.
            if (!CheckFolder()) return;
            CheckSite();

            //  Check the role base security
            if (CheckSecurity(reStartWithNotificationList))
            {
                //  Is the request opened in a frame? 0 = no, 1 = yes, list mode, 2 = yes, form mode
                int openInFrame = Utility.ConvertToInt(_Console.Request.Query["openinframe"]);

                //  Create new instances
                DataGrid grid = new DataGrid();
                var component = new Beta.GeneratedCms.Source.Component();
                _Console.Component = component;

                await HandleRequestAsync(grid, component, isDeleteTriggered);
            }
            else
            {
                await _Console.Response.WriteAsync("no-access");
            }
        }

        /// <summary>
        /// Handles the page request.
        /// </summary>
        /// <param name="grid">The grid.</param>
        /// <param name="component">The component.</param>
        /// <param name="isDeleteTriggered">if set to <c>true</c> [is delete triggered].</param>
        async Task HandleRequestAsync(DataGrid grid, Beta.GeneratedCms.Source.Component component, bool isDeleteTriggered)
        {
            this.HandleActionRequest();
            if (await this.HandleAsyncRequestAsync(component))
                return;

            if ((_Console.ItemType == RequestItemType.Item) || _Console.CurrentListInstance.wim.CanContainSingleInstancePerDefinedList)
            {//  Handles the list item request.
                await HandleListItemRequestAsync(grid, component, isDeleteTriggered);
            }
            else
            {
                //  Handles the browsing request.
                _Console.CurrentListInstance.wim.IsSearchListMode = true;
                await HandleBrowsingRequestAsync(grid, component);
            }
        }


        /// <summary>
        /// Handles the list item request.
        /// </summary>
        /// <param name="grid">The grid.</param>
        /// <param name="component">The component.</param>
        /// <param name="isDeleteTriggered">if set to <c>true</c> [is delete triggered].</param>
        async Task HandleListItemRequestAsync(DataGrid grid, Beta.GeneratedCms.Source.Component component, bool isDeleteTriggered)
        {
            _Console.View = (int)ContainerView.ItemSelect;

            if (_Console.CurrentList.Type == Data.ComponentListType.ListSettings)
                _Console.View = (int)ContainerView.ListSettingRequest;

            _Console.AddTrace("Monitor", "GetExportOptionUrl(..)");
            if (await GetExportOptionUrlAsync(grid, component))
                return;

            //  Create the form
            _Console.CurrentListInstance.wim.HideTopSectionTag = true;

            if (_Console.Request.Query["DBM"] == "1")
                _Console.CurrentListInstance.wim.IsDashboardMode = true;

            if (!_Console.IsComponent)
            {
                if (_Console.CurrentList.Option_FormAsync && !this.IsFormatRequest_JSON)
                {
                    _Console.CurrentListInstance.wim.DoListInit();
                    this.GlobalWimControlBuilder = new WimControlBuilder();
              

                }
                else
                {
                    _Console.CurrentListInstance.wim.DoListInit();
                    this.GlobalWimControlBuilder = component.CreateList(_Console, _Console.OpenInFrame);
             

                    //if (isDeleteTriggered)
                    //    _Console.CurrentListInstance.wim.DoListDelete(component.item);
                }

                if (this.IsFormatRequest_JSON)
                {
                    _Console.Response.ContentType = "application/json";

                    _PresentationNavigation.RightSideNavigation(_Console, component.m_ButtonList != null ? component.m_ButtonList.ToArray() : null, this.GlobalWimControlBuilder);
                    _PresentationNavigation.NewBottomNavigation(_Console, component.m_ButtonList != null ? component.m_ButtonList.ToArray() : null, false, this.GlobalWimControlBuilder);

                    Dictionary<string, string> formFields = null;
                    if (formFields == null)
                        formFields = new Dictionary<string, string>();

                    this.GlobalWimControlBuilder.ApiResponse.ListDescription = _Console.CurrentListInstance.wim.CurrentList.Description;
                    this.GlobalWimControlBuilder.ApiResponse.RedirectUrl = _Console.RedirectionUrl;
                    if (!string.IsNullOrWhiteSpace(_Console.RedirectionUrl))
                        _Console.Response.StatusCode = 302;

                    this.GlobalWimControlBuilder.ApiResponse.IsEditMode = _Console.CurrentListInstance.IsEditMode;
                    this.GlobalWimControlBuilder.ApiResponse.ListTitle = _Console.CurrentListInstance.wim.ListTitle;
                    // if this item is a button add it to the button list

                    await AddToResponseAsync(Newtonsoft.Json.JsonConvert.SerializeObject(this.GlobalWimControlBuilder.ApiResponse));
                    return;
                }

                else if (this.IsFormatRequest_AJAX)
                {
                    _Console.Response.ContentType = "text/plain";
                    string searchListGrid = this.GlobalWimControlBuilder.SearchGrid;
                    await AddToResponseAsync(searchListGrid);
                    return;
                }
                else
                    //  Needed to NULLafy it as it was required for AJAX call
                    this.GlobalWimControlBuilder.SearchGrid = null;
            }

            bool isCopyTriggered = _Console.Form("copyparent") == "1";

            if (isCopyTriggered)
            {
                //
                //_Console.CurrentListInstance.DoListDelete(_Console.Item.GetValueOrDefault(0), 0);
                int childID = _Console.CurrentListInstance.wim.CurrentSite.ID;
                int parentID = _Console.CurrentListInstance.wim.CurrentSite.MasterID.GetValueOrDefault();

                _Console.CurrentListInstance.wim.CurrentSite = Data.Site.SelectOne(parentID);
                _Console.CurrentListInstance.wim.IsCurrentList = true;
                _Console.CurrentListInstance.wim.DoListLoad(_Console.Item.GetValueOrDefault(0), 0);


                await AddToResponseAsync(string.Format("{0}<br/>", _Console.CurrentListInstance.wim.CurrentSite.ID));
                await AddToResponseAsync(string.Format("{0}<br/>", _Console.CurrentListInstance.wim.CurrentSite.MasterID.GetValueOrDefault()));
                return;
            }

            //  Is the delete event triggered?
            if (isDeleteTriggered && _Console.CurrentListInstance.wim.HasListDelete)
            {
                _Console.CurrentListInstance.wim.DoListDelete(_Console.Item.GetValueOrDefault(0), 0, null);

                //  Add deletion entry
                ComponentListVersion version = new ComponentListVersion();
                version.SiteID = _Console.CurrentListInstance.wim.CurrentSite.ID;
                version.ComponentListID = _Console.CurrentListInstance.wim.CurrentList.ID;
                if (_Console.Item.HasValue)
                    version.ComponentListItemID = _Console.Item.Value;

                version.ApplicationUserID = _Console.CurrentApplicationUser.ID;
                version.TypeID = 2;
                version.Save();

                if (_Console.OpenInFrame > 0)
                    return;

                //  Redirect to the containing folder
                if (_Console.CurrentList.Type == ComponentListType.Documents)
                    _Console.Response.Redirect(string.Concat(_Console.WimPagePath, "?gallery=", _Console.CurrentListInstance.wim.CurrentFolder.ID));
                else if (_Console.CurrentListInstance.wim.CurrentFolder.Type == FolderType.Gallery)
                    _Console.Response.Redirect(string.Concat(_Console.WimPagePath, "?gallery=", _Console.CurrentListInstance.wim.CurrentFolder.ParentID));
                else if (_Console.CurrentList.Type == ComponentListType.Folders)
                    _Console.Response.Redirect(string.Concat(_Console.WimPagePath, "?folder=", _Console.CurrentListInstance.wim.CurrentFolder.ParentID));
                else if (_Console.Group.HasValue)
                    _Console.Response.Redirect(string.Concat(_Console.CurrentListInstance.wim.SearchResultItemPassthroughParameter, "=0"), true);
                else if (_Console.CurrentList.Type == ComponentListType.ComponentListProperties)
                    _Console.Response.Redirect(string.Concat(_Console.WimPagePath, "?folder=", _Console.CurrentListInstance.wim.CurrentFolder.ID));
                else
                    _Console.Response.Redirect(string.Concat(_Console.WimPagePath, "?list=", _Console.CurrentList.ID));
            }

            if (_Console.IsComponent)
            {
                _Console.CurrentListInstance.wim.DoListLoad(_Console.Item.GetValueOrDefault(), 0);

                bool isPagePublishTriggered = _Console.IsPostBack("pagepublish");
                bool isPageOfflineTriggered = _Console.IsPostBack("pageoffline");

                if (isPagePublishTriggered)
                {
                    ComponentVersion.SelectOne(_Console.Item.Value).Publish();
                }
                if (isPageOfflineTriggered)
                {
                    ComponentVersion.SelectOne(_Console.Item.Value).TakeDown();
                }

                if (isPageOfflineTriggered || isPagePublishTriggered)
                    _Console.Response.Redirect(_Console.UrlBuild.GetListRequest(_Console.CurrentList, (_Console.Item.Value)));

                Page pageInstance;

                this.GlobalWimControlBuilder = component.CreateContentList(_Console, 0, true, out pageInstance, null);
                this.GlobalWimControlBuilder.Canvas.Type = _Console.OpenInFrame > 0 ? CanvasType.ListItemInLayer : CanvasType.ListItem;
                this.GlobalWimControlBuilder.TopNavigation = _PresentationNavigation.TopNavigation(_Console);
                this.GlobalWimControlBuilder.Bottom = _PresentationNavigation.NewBottomNavigation(_Console, component.m_ButtonList != null ? component.m_ButtonList.ToArray() : null, false);
                this.GlobalWimControlBuilder.Tabularnav = Beta.GeneratedCms.Source.Template.GetTabularTagNewDesign(_Console, _Console.CurrentList.Name, 0, false);
                this.GlobalWimControlBuilder.Leftnav = _PresentationNavigation.NewLeftNavigation(_Console, component.m_ButtonList != null ? component.m_ButtonList.ToArray() : null);

                await AddToResponseAsync(_PresentationMonitor.GetTemplateWrapper(_Console, _Placeholders, _Callbacks, this.GlobalWimControlBuilder));

                return;
            }

            if (!_Console.IsAdminFooter)
            {
             
                this.GlobalWimControlBuilder.TopNavigation = _PresentationNavigation.TopNavigation(_Console);
                this.GlobalWimControlBuilder.Canvas.Type = _Console.OpenInFrame > 0 ? CanvasType.ListItemInLayer : CanvasType.ListItem;
                this.GlobalWimControlBuilder.Rightnav = _PresentationNavigation.RightSideNavigation(_Console, component.m_ButtonList != null ? component.m_ButtonList.ToArray() : null);


                if (
                    this.GlobalWimControlBuilder.Canvas.Type == CanvasType.ListInLayer ||
                    this.GlobalWimControlBuilder.Canvas.Type == CanvasType.ListItemInLayer
                    )
                {
                    //  Do nothing, this is an layer and has no leftnavigation.
                }
                else
                    this.GlobalWimControlBuilder.Leftnav = _PresentationNavigation.NewLeftNavigation(_Console, component.m_ButtonList != null ? component.m_ButtonList.ToArray() : null);
                this.GlobalWimControlBuilder.Bottom = _PresentationNavigation.NewBottomNavigation(_Console, component.m_ButtonList != null ? component.m_ButtonList.ToArray() : null, false);
                this.GlobalWimControlBuilder.Tabularnav = Beta.GeneratedCms.Source.Template.GetTabularTagNewDesign(_Console, _Console.CurrentList.Name, 0, false);

                await AddToResponseAsync(_PresentationMonitor.GetTemplateWrapper(_Console, _Placeholders, _Callbacks, this.GlobalWimControlBuilder));
            }
        }

        async Task<bool> GetExportOptionUrlAsync(DataGrid grid, Beta.GeneratedCms.Source.Component component)
        {
            string exportUrl = null;
            //  Export to XLS: XLS Creation URL
            if (_Console.IsPostBack("export_xls") || _Console.Request.Query["xls"] == "1")
            {
                //_Console.Response.Cache.SetCacheability(HttpCacheability.NoCache);
                //_Console.Response.Cache.SetAllowResponseInBrowserHistory(false);

                exportUrl = _Console.GetSafeUrl();

                _Console.CurrentListInstance.wim.IsExportMode_XLS = true;

                component.CreateSearchList(_Console, 0);
                var url = grid.GetGridFromListInstanceForXLS(_Console, _Console.CurrentListInstance, 0);
                if (_Console.Request.Query["xp"] == "1")
                {
                    await AddToResponseAsync(url);
                    return true;
                }
                else
                {
                    _Console.Response.Redirect(url);
                    return true;
                }
                //  Reset
                _Console.CurrentListInstance.wim.IsExportMode_XLS = false;
            }
            return false;
        }


        bool IsFormatRequest_AJAX { 
            get { 
                return !string.IsNullOrEmpty(_Console.Form(Constants.AJAX_PARAM)); 
            } 
        }
        bool IsFormatRequest_JSON
        {
            get
            {
                return _Console.Form(Constants.JSON_PARAM) == "1" || (_Console.Request.ContentType?.Contains("json")).GetValueOrDefault();
            }
        }

        /// <summary>
        /// Handles the browsing request.
        /// </summary>
        /// <param name="grid">The grid.</param>
        /// <param name="component">The component.</param>
        async Task HandleBrowsingRequestAsync(DataGrid grid, Beta.GeneratedCms.Source.Component component)
        {
            _Console.CurrentListInstance.wim.DoListInit();

            _Console.AddTrace("Monitor", "HandleListItemRequest.Init");
            _Console.View = 2;
            _Console.CurrentListInstance.wim.IsEditMode = true;

            if (_Console.Request.Query["DBM"] == "1")
                _Console.CurrentListInstance.wim.IsDashboardMode = true;

            //if (!this.IsFormatRequest)
            //{
            _Console.AddTrace("Monitor", "GetExportOptionUrl(..)");
            if (await GetExportOptionUrlAsync(grid, component))
                return;

            _Console.AddTrace("Monitor", "CreateSearchList(..)");

            this.GlobalWimControlBuilder = component.CreateSearchList(_Console, 0);
            this.GlobalWimControlBuilder.Canvas.Type = _Console.OpenInFrame > 0 ? CanvasType.ListInLayer : CanvasType.List;

            if (_Console.OpenInFrame > 0)
                _Console.CurrentListInstance.wim.Page.HideTabs = true;
            //}

            string searchListGrid;
            if (true)//_Console.CurrentList.Type != ComponentListType.Browsing || _Console.CurrentApplicationUser.ShowNewDesign2)
            {
                _Console.AddTrace("Monitor", "GetGridFromListInstance(..)");

                if (this.IsFormatRequest_JSON)
                {
                    _Console.Response.ContentType = "application/json";
                    searchListGrid = grid.GetGridFromListInstanceForJSON(_Console.CurrentListInstance.wim, _Console, 0, false, true);

                    await AddToResponseAsync(searchListGrid);
                    return;
                }
                if (this.IsFormatRequest_AJAX)
                {
                    _Console.Response.ContentType = "text/plain";
                    searchListGrid = null;
                    while (_Console.CurrentListInstance.wim.NextGrid())
                    {
                        bool hasNoTitle = string.IsNullOrEmpty(_Console.CurrentListInstance.wim.m_DataTitle);
                        searchListGrid +=
                            string.Concat(
                                hasNoTitle
                                    ? null
                                    : string.Format("</section><section class=\"{1}\"><h2>{0}</h2>"
                                    , _Console.CurrentListInstance.wim.m_DataTitle
                                    , _Console.CurrentListInstance.wim.Page.Body.Grid.ClassName
                            )
                            , grid.GetGridFromListInstance(_Console.CurrentListInstance.wim, _Console, 0, false, true)
                            , hasNoTitle
                                    ? null
                                    : ""

                            );
                    }
                    await AddToResponseAsync(searchListGrid);
                    return;
                }
                if (_Console.CurrentListInstance.wim.CurrentList.Option_SearchAsync && !_Console.CurrentListInstance.wim.IsDashboardMode)
                {
                    //  CLEANUP TWO LOCATIONS !!! (27.01.14:MM)
                    if (_Console.OpenInFrame > 0)
                        searchListGrid = string.Format("<section id=\"datagrid\" class=\"{0} async\"> </section>", _Console.CurrentListInstance.wim.Page.Body.Grid.ClassName);//"<section class=\"searchTable\"> </section>";//grid.GetGridFromListInstanceForKnockout(_Console.CurrentListInstance.wim, _Console, 0, false, IsNewDesignOutput, false);\
                    else
                        searchListGrid = " ";
                }
                else
                {
                    searchListGrid = null;// grid.GetGridFromListInstance(_Console.CurrentListInstance.wim, _Console, 0, false, IsNewDesignOutput);
                    while (_Console.CurrentListInstance.wim.NextGrid())
                    {
                        bool hasNoTitle = string.IsNullOrEmpty(_Console.CurrentListInstance.wim.m_DataTitle);
                        searchListGrid +=
                            string.Concat(
                                hasNoTitle
                                    ? null
                                    : string.Format("</section><section class=\"{1}\"><h2>{0}</h2>", _Console.CurrentListInstance.wim.m_DataTitle, _Console.CurrentListInstance.wim.Page.Body.Grid.ClassName
                            )
                            , grid.GetGridFromListInstance(_Console.CurrentListInstance.wim, _Console, 0, false, true)
                            , hasNoTitle
                                    ? null
                                    : ""
                            );
                    }
                }

                //  Replacement event of ListSearchedAction
                if (!string.IsNullOrEmpty(component.m_ClickedButton) && _Console.CurrentListInstance.wim.HasListAction)
                    _Console.CurrentListInstance.wim.DoListAction(_Console.Item.GetValueOrDefault(0), 0, component.m_ClickedButton, null);

                //_Console.CurrentListInstance.wim.SendReport(searchListGrid);
            }
            else
            {
                this.GlobalWimControlBuilder.Canvas.Type = CanvasType.Explorer;

                _Console.AddTrace("Monitor", "GetThumbnailGridFromListInstance(..)");
                searchListGrid = grid.GetThumbnailGridFromListInstance(_Console.CurrentListInstance.wim, _Console, 0, false);
            }

            _Console.AddTrace("Monitor", "AddToResponse(..)");

            this.GlobalWimControlBuilder.SearchGrid = searchListGrid;
            this.GlobalWimControlBuilder.TopNavigation = _PresentationNavigation.TopNavigation(_Console);
            this.GlobalWimControlBuilder.Bottom = _PresentationNavigation.NewBottomNavigation(
                _Console,
                component.m_ButtonList != null
                    ? component.m_ButtonList.ToArray()
                    : null,
                !this.GlobalWimControlBuilder.IsNull
            );

            this.GlobalWimControlBuilder.Tabularnav = Beta.GeneratedCms.Source.Template.GetTabularTagNewDesign(_Console, _Console.CurrentList.Name, 0, false);
            this.GlobalWimControlBuilder.Rightnav = _PresentationNavigation.RightSideNavigation(_Console, component.m_ButtonList != null ? component.m_ButtonList.ToArray() : null);
            this.GlobalWimControlBuilder.Leftnav = _PresentationNavigation.NewLeftNavigation(_Console, component.m_ButtonList != null ? component.m_ButtonList.ToArray() : null);

            await AddToResponseAsync(_PresentationMonitor.GetTemplateWrapper(_Console, _Placeholders, _Callbacks, this.GlobalWimControlBuilder));
        }


        async Task<bool> HandleAsyncRequestAsync(Beta.GeneratedCms.Source.Component component)
        {
            if (_Console.CurrentListInstance == null) return false;
            var async = Utils.GetAsyncQuery(_Console);
            if (async == null)
                return false;

            if (_Console.CurrentListInstance.wim.HasListAsync)
            {
                _Console.HasAsyncEvent = true;
                ComponentAsyncEventArgs eventArgs = new ComponentAsyncEventArgs(_Console.Item.GetValueOrDefault());

                eventArgs.Query = async.SearchQuery;
                eventArgs.SearchType = async.SearchType;
                eventArgs.Property = async.Property;

                eventArgs.Data = new ASyncResult();
                eventArgs.Data.Property = async.Property;
                eventArgs.ApplyData(component, _Console);
                eventArgs.SelectedGroupItemKey = _Console.GroupItem.GetValueOrDefault();
                eventArgs.SelectedGroupKey = _Console.Group.GetValueOrDefault();

                var result = _Console.CurrentListInstance.wim.DoListAsync(eventArgs);

                string val = JSON.Instance.ToJSON(result.Data,
                    new JSONParameters() {
                        EnableAnonymousTypes = true,
                        UsingGlobalTypes = false,
                        SerializeNullValues = false
                    }
                );
                _Console.Response.ContentType = "application/json";
                await AddToResponseAsync(val);

                return true;
            }
            return false;
        }

        /// <summary>
        /// Handles the action request.
        /// </summary>
        void HandleActionRequest()
        {
            if (_Console.CurrentListInstance == null) return;
            if (string.IsNullOrEmpty(_Console.CurrentListInstance.wim.PostbackValue)) return;

            switch (_Console.CurrentListInstance.wim.PostbackValue)
            {
                case "PageContentPublication":
                    //EnvironmentVersionLogic.Flush();
                    _Console.CurrentListInstance.wim.Notification.AddNotification("The webcontent has been refreshed.");
                    return;
            }
        }

        /// <summary>
        /// Redirects to channel home page.
        /// </summary>
        /// <param name="siteID">The site ID.</param>
        void RedirectToChannelHomePage(int siteID)
        {
            //string redirect = _Console.GetWimPagePath(siteID);

            //  Find the default homepage in the menu section
            var defaultHome = _Console.UrlBuild.GetHomeRequest(siteID);
            _Console.Response.Redirect(defaultHome);
            //MenuItemView.SelectAll(siteID, _Console.CurrentApplicationUser.RoleID, 0);
            //if (defaultHome != null && defaultHome.Length > 0)
            //{
            //    string redirect = _PresentationNavigation.GetUrl(_Console, defaultHome[0], siteID);
            //    _Console.Response.Redirect(redirect);
            //}
        }

        bool CheckSecurity(bool reStartWithNotificationList)
        {
            //  ACL: Sites
            if (!_Console.CurrentListInstance.wim.CurrentApplicationUserRole.All_Sites
                && _Console.CurrentListInstance.wim.CurrentApplicationUserRole.Sites(_Console.CurrentApplicationUser).Length == 0)
            {
                if (!reStartWithNotificationList)
                    throw new Exception(Framework.ErrorCode.GetMessage(1002, _Console.CurrentApplicationUser.LanguageCulture));
            }
            //  ACL: Sites
            if (!_Console.CurrentListInstance.wim.CurrentApplicationUserRole.All_Sites)
            {
                if (!_Console.CurrentListInstance.wim.CurrentSite.HasRoleAccess(_Console.CurrentListInstance.wim.CurrentApplicationUser))
                {
                    var allowed = _Console.CurrentListInstance.wim.CurrentApplicationUserRole.Sites(_Console.CurrentApplicationUser);
                    if (allowed != null && allowed.Length > 0)
                    {
                        RedirectToChannelHomePage(allowed[0].ID);
                        return false;
                    }
                    else
                    {
                        if (_Console.CurrentListInstance.wim.CurrentApplicationUser.Sites(Data.AccessFilter.RoleAndUser) != null && _Console.CurrentListInstance.wim.CurrentApplicationUser.Sites(Data.AccessFilter.RoleAndUser).Length > 0)
                        {
                            _Console.Response.Redirect(_Console.GetWimPagePath(_Console.CurrentListInstance.wim.CurrentApplicationUser.Sites(Data.AccessFilter.RoleAndUser)[0].ID));
                            return false;
                        }
                        else
                            throw new Exception("There are no active accessible channels available.");
                    }
                }
            }
            else
            {
                //  CHECK FOR UserBased exceptions!!!
            }


            //  20-01-13:MM Added dashboard hack
            if (_Console.CurrentListInstance.wim.CurrentFolder.ID == 0 && string.IsNullOrEmpty(_Console.Request.Query["dashboard"]))
            {
                RedirectToChannelHomePage(_Console.ChannelIndentifier);
                return false;
            }
            //  ACL: Folders
            if (_Console.CurrentListInstance.wim.CurrentFolder.Type != FolderType.Gallery
                && !_Console.CurrentListInstance.wim.CurrentApplicationUserRole.All_Folders)
            {
                if (!_Console.CurrentListInstance.wim.CurrentFolder.HasRoleAccess(_Console.CurrentListInstance.wim.CurrentApplicationUser))
                {
                    if (_Console.CurrentListInstance.wim.CurrentFolder.ParentID.HasValue)
                        _Console.Response.Redirect(_Console.UrlBuild.GetFolderRequest(_Console.CurrentListInstance.wim.CurrentFolder.ParentID.Value));

                    _Console.Response.Redirect(_Console.WimPagePath);
                    return false;
                }
            }

            //  Check environment
            bool approved = false;
            switch (_Console.CurrentListInstance.wim.CurrentFolder.Type)
            {
                case FolderType.Undefined:
                    approved = true; break;

                case FolderType.Page:
                    approved = _Console.CurrentListInstance.wim.CurrentApplicationUserRole.CanSeePage;

                    break;
                case FolderType.List:
                    if (_Console.CurrentListInstance.wim.CurrentList.Type == Data.ComponentListType.Browsing)
                    {
                        approved = _Console.CurrentListInstance.wim.CurrentApplicationUserRole.CanSeeFolder;
                        if (!approved)
                        {
                            _Console.Response.Redirect(_Console.WimPagePath);
                            return false;
                        }
                    }
                    approved = _Console.CurrentListInstance.wim.CurrentApplicationUserRole.CanSeeList;
                    if (_Console.CurrentList.Type == ComponentListType.Undefined && !_Console.CurrentListInstance.wim.CurrentApplicationUserRole.All_Lists)
                    {
                        if (!_Console.CurrentListInstance.wim.CurrentList.HasRoleAccess(_Console.CurrentListInstance.wim.CurrentApplicationUser))
                        {
                            _Console.Response.Redirect(_Console.WimPagePath); 
                            return false;

                        }
                    }
                    break;
                case FolderType.Gallery:
                    approved = true;
                    if (!_Console.CurrentListInstance.wim.CurrentApplicationUserRole.All_Galleries)
                    {
                        Gallery currentGallery = Gallery.SelectOne(_Console.CurrentListInstance.wim.CurrentFolder.ID);
                        if (!currentGallery.HasRoleAccess(_Console.CurrentListInstance.wim.CurrentApplicationUser))
                        {
                            if (currentGallery.ParentID.HasValue)
                            {
                                _Console.Response.Redirect(_Console.UrlBuild.GetGalleryRequest(currentGallery.ParentID.Value));
                            }

                            _Console.Response.Redirect(_Console.WimPagePath);
                            return false;
                        }
                    }
                    break;
                case FolderType.Administration:
                    approved = _Console.CurrentListInstance.wim.CurrentApplicationUserRole.CanSeeAdmin;
                    break;
            }

            if (!approved)
            {
                if (_Console.CurrentListInstance.wim.CurrentList.Type != ComponentListType.Images
                    && _Console.CurrentListInstance.wim.CurrentList.Type != ComponentListType.Documents
                    && _Console.CurrentListInstance.wim.CurrentList.Type != ComponentListType.Links)
                {
                    _Console.Response.Redirect(_Console.WimPagePath);
                    return false;
                }
            }
            return true;
        }


        /// <summary>
        /// Checks and sets the current folder.
        /// </summary>
        /// <returns>Continue processing page?</returns>
        bool CheckFolder()
        {
            if (_Console.CurrentListInstance.wim.CurrentFolder == null)
                throw new Exception("No containing folder found for the requested item!");

            //  If in browsing mode, set the page title to the current folder
            if (_Console.Logic == 0 && !_Console.Item.HasValue && !_Console.CurrentListInstance.wim.CurrentFolder.IsNewInstance)
                _Console.Title = _Console.CurrentListInstance.wim.CurrentFolder.Name;
            return true;
        }

        /// <summary>
        /// Checks the site for channel requests.
        /// </summary>
        void CheckSite()
        {
            if (_Console.CurrentList.SiteID.HasValue && _Console.ChannelIndentifier != _Console.CurrentList.SiteID.Value)
            {
                if (_Console.CurrentList.IsInherited) return;
                var site = Site.SelectOne(_Console.CurrentList.SiteID.Value);
                if (site.Type.HasValue) return;
                _Console.Response.Redirect(_Console.CurrentListInstance.wim.GetUrl());
            }
        }

        /// <summary>
        /// Sets the form modes (Edit/Save,Delete, etc)
        /// </summary>
        /// <param name="isDeleteMode">if set to <c>true</c> [is delete mode].</param>
        async Task<bool> SetFormModesAsync()
        {
            await _Console.LoadJsonStreamAsync();

            //  Is the form state in editmode?
            _Console.CurrentListInstance.wim.IsEditMode = _Console.IsPostBack("edit")
                || _Console.IsPostBack("save")
                || _Console.CurrentListInstance.wim.OpenInEditMode
                || _Console.Request.Query["item"] == "0"
                || _Console.Request.Query["asset"] == "0"
                || (_Console.JsonReferrer != null && _Console.JsonReferrer.Equals("edit"))
                || _Console.JsonForm != null;

            //  Create new page
            if (!_Console.CurrentListInstance.wim.IsEditMode
                && _Console.ItemType == RequestItemType.Page
                && _Console.Request.Headers["Referer"].FirstOrDefault() != null
                && _Console.Request.Headers["Referer"].FirstOrDefault().Contains("item=0")
                )
            {
                _Console.CurrentListInstance.wim.IsEditMode = true;
            }

            //  Is the save link clicked?
            _Console.CurrentListInstance.wim.IsSaveMode = _Console.IsPostBack("save") || _Console.IsPostBack("saveNew");
            _Console.IsAdminFooter = Utility.ConvertToInt(_Console.Request.Query["adminFooter"]) == 1;

            //  Is the delete link clicked?
            bool isDeleteMode = (_Console.IsPostBack("delete") || _Console.Request.Method == "DELETE");
            _Console.CurrentListInstance.wim.IsDeleteMode = isDeleteMode;

            //  Set the developer mode
            if (_Console.PostbackValue == "dev_showhidden")
            {
                _Console.CurrentApplicationUser.ShowHidden = true;
                _Console.CurrentApplicationUser.Save();
            }
            else if (_Console.PostbackValue == "dev_showvisible")
            {
                _Console.CurrentApplicationUser.ShowHidden = false;
                _Console.CurrentApplicationUser.Save();
            }
            return isDeleteMode;
        }

        /// <summary>
        /// Obtain request.path and querystring requests
        /// </summary>
        void SetRequestType()
        {
            var folderId = default(int?);
            var targetname = default(string);

            // extract url details
            var wim = CommonConfiguration.PORTAL_PATH.ToLower();
            var url = _Console.Request.Path.Value.ToLower();
            var split =
                wim == "/"
                    ? url.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries)
                    : url.Replace(wim, string.Empty).Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

            // set the channel identifier
            if (split.Any())
            {
                // position counter
                int n = 0;

                var candidate = Utils.FromUrl(split[0]);
                if (Data.Utility.IsNumeric(candidate))
                {
                    _Console.ChannelIndentifier = Data.Utility.ConvertToInt(candidate, Data.Environment.Current.DefaultSiteID.GetValueOrDefault());
                }
                else
                {
                    var selection = Site.SelectAll().Where(x => x.Name.Equals(candidate, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
                    if (selection != null)
                    {
                        n++;
                        _Console.ChannelIndentifier = selection.ID;
                    }
                }
                if (_Console.ChannelIndentifier == 0)
                {
                    _Console.ChannelIndentifier = Data.Environment.Current.DefaultSiteID.GetValueOrDefault();
                    if (_Console.ChannelIndentifier == 0)
                    {
                        _Console.ChannelIndentifier = Data.Site.SelectAll()[0].ID;
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
                        _Console.ItemType = RequestItemType.Page;
                    }
                    else if (typecandidate.Equals("assets", StringComparison.CurrentCultureIgnoreCase))
                    {
                        n++;
                        _Console.ItemType = RequestItemType.Asset;
                    }
                    else
                    {
                        _Console.ItemType = RequestItemType.Item;
                    }

                }

                if (split.Length - n - 1 > 0)
                {
                    var completepath = string.Concat("/", string.Join("/", split.Skip(n).Take(split.Length - n - 1)), "/");
                    _Console.CurrentFolder = Folder.SelectOne(completepath, _Console.ChannelIndentifier);

                    if (_Console.CurrentFolder != null)
                    {
                        folderId = _Console.CurrentFolder.ID;
                    }
                }

                if (split.Length - n > 0)
                {
                    targetname = split.Last();
                }
            }
            else
            {
                if (_Console.ChannelIndentifier == 0)
                {
                    _Console.ChannelIndentifier = Data.Environment.Current.DefaultSiteID.GetValueOrDefault();
                    if (_Console.ChannelIndentifier == 0)
                    {
                        _Console.ChannelIndentifier = Data.Site.SelectAll()[0].ID;
                    }
                }
            }

            _Console.ItemType = RequestItemType.Undefined;

            //  Verify paging page
            _Console.ListPagingValue = _Console.Request.Query["set"];

            _Console.Group = Utility.ConvertToIntNullable(_Console.Request.Query["group"]);
            _Console.GroupItem = Utility.ConvertToIntNullable(_Console.Request.Query["groupitem"]);

            //  Verify page request
            _Console.Item = Utility.ConvertToIntNullable(_Console.Request.Query["page"], false);
            if (_Console.Item.HasValue)
            {
                _Console.ItemType = RequestItemType.Page;
                return;
            }
            //  Verify asset request
            _Console.Item = Utility.ConvertToIntNullable(_Console.Request.Query["asset"], false);
            if (_Console.Item.HasValue)
            {
                _Console.ItemType = RequestItemType.Asset;
                return;
            }
            //  Verify dashboard request
            _Console.Item = Utility.ConvertToIntNullable(_Console.Request.Query["dashboard"], false);
            if (_Console.Item.HasValue)
            {
                _Console.ItemType = RequestItemType.Dashboard;
                return;
            }
            //  Verify list-item request
            _Console.Item = Utility.ConvertToIntNullable(_Console.Request.Query["item"], false);
            if (_Console.Item.HasValue) _Console.ItemType = RequestItemType.Item;

            //  Apply the current component list based on the roaming environment settings
            this.ApplyComponentList(folderId, targetname);
        }

        /// <summary>
        /// Outputs the ajax request.
        /// </summary>
        /// <returns>Continue processing page?</returns>
        async Task<bool> OutputAjaxRequestAsync()
        {
            if (!string.IsNullOrEmpty(_Console.Request.Query["xml"]))
            {
                _Console.Response.ContentType = "text/xml";
                //_Console.Response.Cache.SetCacheability(HttpCacheability.NoCache);
                //_Console.Response.Cache.SetAllowResponseInBrowserHistory(false);

                var type = Utility.ConvertToIntNullable(_Console.Request.Query["xml"]);
                if (type.HasValue)
                {
                    WimControlBuilder build = new WimControlBuilder();
                    MetaData meta = new MetaData();
                    meta.ContentTypeSelection = type.Value.ToString();
                    var element = meta.GetContentInfo();
                    string name = string.Concat(_Console.Request.Query["id"], "__", type.Value, "__", _Console.Request.Query["index"]);


                    ((Framework.ContentSharedAttribute)element).ID = name;
                    ((Framework.ContentSharedAttribute)element).OverrideTableGeneration = true;
                    ((Framework.ContentSharedAttribute)element).Expression = (_Console.Request.Query["w"] == "2" ? OutputExpression.FullWidth : OutputExpression.Alternating);
                    ((Framework.ContentSharedAttribute)element).Console = _Console;
                    ((Framework.ContentSharedAttribute)element).IsBluePrint = true;
                    if (element.ContentTypeSelection == ContentType.Binary_Image)
                        ((Framework.ContentInfoItem.Binary_ImageAttribute)element).GalleryPropertyUrl = _Console.Request.Query["gallery"];


                    element.SetCandidate(new Field(), true);
                    build.AppendFormat(@"
                    <div class=""cmsable"">
                        {0}
                        <table class=""formTable"">", element.GetMultiFieldTitleHTML(true));

                    build.AppendFormat(@"
                        <tbody>
                        <tr>
                        <td>");

                    element.WriteCandidate(build, true, false, false);

                    build.AppendFormat(@"
                        </td>
                        </tr>
                        </tbody>");

                    build.AppendFormat(@"
                        </table>
                    </div>");

                    await AddToResponseAsync(build.ToString());
                }


                if (_Console.Request.Query["xml"] == "component")
                {
                    var page = Utility.ConvertToInt(_Console.Request.Query["page"]);
                    var target = _Console.Request.Query["tab"];
                    if (String.IsNullOrEmpty(target) && page > 0)
                    {
                        var pageInstance = Page.SelectOne(page);
                        var sections = pageInstance.Template.GetPageSections();
                        if (sections != null)
                            target = sections.FirstOrDefault();
                    }

                    await AddToResponseAsync(Beta.GeneratedCms.Source.Xml.Component.Get(_Console, Utility.ConvertToInt(_Console.Request.Query["id"]), page, Utility.ConvertToInt(_Console.Request.Query["cmpt"]), target));
                }

                return false;
            }
            return true;
        }

        /// <summary>
        /// Apply the current component list based on the roaming environment settings
        /// </summary>
        private bool ApplyComponentList(int? folderId, string name)
        {
            //if (_Console.ItemType == RequestItemType.Undefined && !string.IsNullOrWhiteSpace(name))
            //{
            //    var urldecrypt = Utils.FromUrl(name);
            //    var list = ComponentList.SelectOne(urldecrypt, null);
            //    if (list != null && !list.IsNewInstance)
            //    {
            //        return _Console.ApplyList(list);
            //    }
            //}
            if (!string.IsNullOrWhiteSpace(name))
            {
                var urldecrypt = Utils.FromUrl(name);
                var list = ComponentList.SelectOne(urldecrypt, null);
                if (list != null && !list.IsNewInstance)
                {
                    return _Console.ApplyList(list);
                }
            }

            //  If the list is not know, take the default list in stead (browsing)
            if (!string.IsNullOrEmpty(_Console.Request.Query["list"]))
            {
                //  The list reference can be a INT or a GUID
                return _Console.ApplyList(_Console.Request.Query["list"]);
            }
            else if (_Console.ItemType == RequestItemType.Asset)
            {
                _Console.ApplyList(Data.ComponentListType.Documents);
            }
            else
                _Console.ApplyList(typeof(AppCentre.Data.Implementation.Browsing));
            return true;
        }

        /// <summary>
        /// When there is no roaming application user, redirect to the homepage.
        /// </summary>
        /// <returns>Continue processing page?</returns>
        async Task<bool> CheckRoamingApplicationUserAsync(bool showLogin = false)
        {
            if (!showLogin)
            {
                //  Check if logout request is performed
                if (_Console.IsPostBack("logout"))
                {
                    _Console.CurrentVisitor.ApplicationUserID = null;
                    _Console.CurrentVisitor.Save();

                    string wimPath = CommonConfiguration.PORTAL_PATH;

                    _Console.Response.Redirect(_Console.AddApplicationPath(wimPath), true);

                }
            }

            await AuthenticateViaSingleSignOnAsyc(false);

            //  Check roaming profile
            if (!showLogin && _Console.CurrentApplicationUser != null)
            {
                await ValidateHijackResetAsync();
                return true;
            }
            else
            {
                //  Check SSO
                await AuthenticateViaSingleSignOnAsyc(true);

                string reaction = _PresentationMonitor.GetLoginWrapper(_Console, _Placeholders, _Callbacks);
                if (!string.IsNullOrEmpty(reaction))
                {
                    await AddToResponseAsync(reaction);
                    return false;
                }
            }
            return true;
        }

        async Task ValidateHijackResetAsync()
        {
            if (_Console.Request.QueryString.HasValue 
                && _Console.Request.QueryString.Value.Equals("?reset", StringComparison.CurrentCultureIgnoreCase))
            {
                if (_Console.CurrentVisitor != null && _Console.CurrentVisitor.Data != null)
                {
                    if (!_Console.CurrentVisitor.Data["Wim.Reset.Me"].IsNull)
                    {
                        if (Guid.TryParse(_Console.CurrentVisitor.Data["Wim.Reset.Me"].Value, out var id))
                        {
                            var user = await ApplicationUser.SelectOneAsync(id);
                            if (user?.ID > 0)
                            {
                                _Console.CurrentVisitor.Data.Apply("Wim.Reset.Me", null);
                                _Console.CurrentVisitor.ApplicationUserID = user.ID;
                                _Console.CurrentApplicationUser = user;
                                _Console.SaveVisit();
                            }

                        }
                    }
                }
            }
        }


        async Task AuthenticateViaSingleSignOnAsyc(bool shouldredirect = true)
        {
            if (_configuration.GetValue<bool>("mediakiwi:authentication"))
            {
                if (!_Context.User.Identity.IsAuthenticated)
                {
                    if (!await IsValidIdentityAsync(_Context, _Console.CurrentDomain))
                    {
                        if (shouldredirect && 
                            !_Context.Request.Path.Value.EndsWith("/sso/negotiate", StringComparison.CurrentCultureIgnoreCase))
                        {
                            _Context.Response.Redirect($"{_Console.CurrentDomain}/.auth/login/aad?post_login_redirect_url={_Console.GetWimPagePath(null)}/sso/negotiate?redir={_Context.Request.Path}{_Context.Request.QueryString}");
                        }
                    }
                }
            }
        }

        async Task<bool> IsValidIdentityAsync(HttpContext context, string uriString)
        {
            var cookieContainer = new CookieContainer();
            var handler = new HttpClientHandler()
            {
                CookieContainer = cookieContainer
            };

            if (context.Request.Cookies.ContainsKey("AppServiceAuthSession"))
            {
                var cookie = new Cookie("AppServiceAuthSession", context.Request.Cookies["AppServiceAuthSession"].ToString());
                cookie.Domain = _Console.CurrentHost;
                cookieContainer.Add(cookie);
            }

            var jsonResult = string.Empty;

            using (var client = new HttpClient(handler))
            {
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                client.DefaultRequestHeaders.Add("Accept-Charset", "UTF-8");
                client.DefaultRequestHeaders.Add("Cache-Control", "no-cache");

                if (context.Request.Cookies.ContainsKey(".authtoken"))
                {
                    var authenticationToken = context.Request.Cookies[".authtoken"];
                    client.DefaultRequestHeaders.Add("X-ZUMO-AUTH", authenticationToken);
                }

                var res = await client.GetAsync($"{uriString}/.auth/me");
                jsonResult = await res.Content.ReadAsStringAsync();
            }

            if (!string.IsNullOrWhiteSpace(jsonResult))
            {
                if ((jsonResult.StartsWith("{") && jsonResult.EndsWith("}")) || //For object
                    (jsonResult.StartsWith("[") && jsonResult.EndsWith("]"))) //For array
                {
                    try
                    {
                        var obj = JArray.Parse(jsonResult);

                        var claims = new List<Claim>();
                        foreach (var claim in obj[0]["user_claims"])
                        {
                            claims.Add(new Claim(claim["typ"].ToString(), claim["val"].ToString()));
                        }
                        //claims.Add(new Claim("email", "test@test.nl"));

                        var identity = new GenericIdentity("DefaultUser");
                        identity.AddClaims(claims);

                        context.User = new GenericPrincipal(identity, null);

                        var visitor = new VisitorManager(context)
                            .SelectVisitorByCookie();

                        if (_Console.CurrentApplicationUser != null)
                            visitor.ApplicationUserID = _Console.CurrentApplicationUser.ID;
                        new VisitorManager(context).Save(visitor, true);

                        if (_Context.Request.Path.Value.EndsWith("/sso/negotiate", StringComparison.CurrentCultureIgnoreCase))
                        {
                            if (_Context.Request.Query.ContainsKey("redir"))
                            {
                                context.Response.Redirect(_Context.Request.Query["redir"]);
                            }
                            else
                            {
                                context.Response.Redirect(_Console.GetWimPagePath(null));
                            }
                        }
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Adds to response.
        /// </summary>
        /// <param name="output">The output.</param>
        async Task AddToResponseAsync(string output)
        {
            if (outputHTML == null)
                outputHTML = new StringBuilder();

            if (this.IsLoadedInThinLayer)
                outputHTML.Append(output);
            else
            {
                if (_Console.Request.Query["split"] == "homeArticle")
                {
                    var split = output.Split(new string[] { @"<article id=""homeArticle"">", "</article>" }, StringSplitOptions.RemoveEmptyEntries);
                    output = split[1];
                }
                Body = output;
                await _Console.Response.WriteAsync(output);
            }
        }

        public string Body { get; set; }
    }

}
