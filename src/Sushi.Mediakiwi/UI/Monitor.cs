using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Http.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sushi.Mediakiwi.Beta.GeneratedCms.Source;
using Sushi.Mediakiwi.Controllers;
using Sushi.Mediakiwi.Data;
using Sushi.Mediakiwi.Data.Configuration;
using Sushi.Mediakiwi.Framework;
using Sushi.Mediakiwi.Interfaces;
using Sushi.Mediakiwi.Logic;
using Sushi.Mediakiwi.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Sushi.Mediakiwi.Framework.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Sushi.Mediakiwi.Extension;

namespace Sushi.Mediakiwi.UI
{
    public class Monitor
    {
        private IHostEnvironment _env;
        private HttpContext _context;
        private Beta.GeneratedCms.Console _console;
        private iPresentationMonitor _presentationMonitor;
        private iPresentationNavigation _presentationNavigation;
        private IServiceProvider _serviceProvider;
        Dictionary<GlobalPlaceholder, string> _Placeholders;
        Dictionary<CallbackTarget, List<ICallback>> _Callbacks;
        bool IsLoadedInThinLayer;
        public StringBuilder outputHTML;
        internal WimControlBuilder GlobalWimControlBuilder;
        private readonly IConfiguration _configuration;
        private readonly OAuth2Logic _oAuth2Logic;

        public Monitor(HttpContext context, IHostEnvironment env, IConfiguration configuration)
        {
            _env = env;
            _context = context;
            _configuration = configuration;
            _console = new Beta.GeneratedCms.Console(context, _env);
            _console.Configuration = configuration;

            _presentationMonitor = new Framework.Presentation.Presentation();
            _presentationNavigation = new Framework.Presentation.Logic.Navigation();

            // todo: use constructor injection
            _oAuth2Logic = _context.RequestServices.GetRequiredService<OAuth2Logic>();
        }

        internal async Task StartAsync()
        {
            if (_env.IsDevelopment())
            {
                await StartAsync(false).ConfigureAwait(false);
            }
            else
            {
                try
                {
                    await StartAsync(false).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    try
                    {
                        var notification = await Notification.InsertOneAsync("Uncaught exception", ex).ConfigureAwait(false);
                        if (notification != null)
                        {
                            await HandleExceptionAsync(_context, ex, notification);
                        }
                    }
                    catch
                    {
                        await HandleExceptionAsync(_context, ex, null);
                    }
                }
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception, Notification notification)
        {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            var output = Utils.GetHtmlFormattedLastServerError(exception);
            if (notification != null)
            {
                output += $"<br/><br/><b>Detailed notification:</b><br/> Created on: {notification.Created:dd-MM-yyyy HH:mm}<br/>Group: {notification.Group}<br/>Identifier: {notification.GetIdMessage()}";
            }
            else
            {
                output += "<br/><br/><b>A detailed notification item could not be created</b>";
            }

            await context.Response.WriteAsync(output);
        }

        internal static async Task<bool> StartControllerAsync(HttpContext context, IHostEnvironment env, IConfiguration configuration, IServiceProvider serviceProvider)
        {
            var monitor = new Monitor(context, env, configuration);

            if (await new ControllerRegister(context, serviceProvider).VerifyAsync()
                .ConfigureAwait(false))
            {
                return true;
            }
            return false;
        }

        bool HasReservedRootQueryString()
        {
            List<string> reservedDict = new List<string>() { "list", "folder", "page", "gallery", "asset" };
            var found = false;

            foreach (var dict in reservedDict)
            {
                if (_console.Request.Query.ContainsKey(dict))
                {
                    found = true;
                    break;
                }
            }

            return found;
        }
        
        internal async Task StartAsync(bool reStartWithNotificationList)
        {
            //  Set the current environment
            _console.CurrentEnvironment = Data.Environment.Current;
            var environmentVersion = await EnvironmentVersion.SelectAsync().ConfigureAwait(false);

            // Validate load balanced cache state
            var updated = environmentVersion.Updated.GetValueOrDefault(DateTime.UtcNow);
            await Data.Caching.Configuration.CacheManager.ValidateAsync(updated).ConfigureAwait(false);

            var path = _console.AddApplicationPath(CommonConfiguration.PORTAL_PATH);
            var requestPath = _console.AddApplicationPath(_console.Request.Path.Value);
            if (!requestPath.StartsWith(path, StringComparison.CurrentCultureIgnoreCase))
            {
                return;
            }

            await _console.SetDateFormatAsync();

            bool forcelogin = 
                //_Console.Request.Path.Equals($"{Data.Environment.Current.RelativePath}/login", StringComparison.CurrentCultureIgnoreCase)
                _console.Url.Contains($"{path}?reset=", StringComparison.CurrentCultureIgnoreCase)
                || _console.Url.EndsWith($"{path}?reminder", StringComparison.CurrentCultureIgnoreCase)
                ;

            if (!await CheckRoamingApplicationUserAsync(forcelogin).ConfigureAwait(false))
            {
                return;
            }

            await _console.SaveVisitAsync().ConfigureAwait(false);

            //  Obtain querystring requests
            await SetRequestTypeAsync().ConfigureAwait(false);

            //  If an xml (ajax) request comes in output a correct response.
            if (!await OutputAjaxRequestAsync().ConfigureAwait(false))
            {
                return;
            }

            //  Define internal event-check value types
            var  isDeleteTriggered = await SetFormModesAsync().ConfigureAwait(false);

            //  Checks and sets the current folder.
            if (!CheckFolder())
            {
                return;
            }

            await CheckSiteAsync().ConfigureAwait(false);

            // If we are at the landing page, without any reserved querystring process as Root request
            if (_console.Request.Path.Equals(WimServerConfiguration.Instance.Portal_Path, StringComparison.InvariantCulture) && HasReservedRootQueryString() == false)
            {
                await HandleRootRequestAsync().ConfigureAwait(false);
            }

            //  Check the role base security
            if (await CheckSecurityAsync(reStartWithNotificationList).ConfigureAwait(false))
            {
                //  Create new instances
                DataGrid grid = new DataGrid();
                var component = new Beta.GeneratedCms.Source.Component();
                _console.Component = component;

                await HandleRequestAsync(grid, component, isDeleteTriggered).ConfigureAwait(false);
            }
            else
            {
                // TODO: MR:10-12-2021 Add logging
                await _console.Response.WriteAsync("no-access").ConfigureAwait(false);
                await _console.Response.CompleteAsync();
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
            // Check if it's a sorting request
            if (await HandleSortingRequestAsync().ConfigureAwait(false))
            {
                return;
            }

            // Check if it's an action request
            HandleActionRequest();

            if (await HandleAsyncRequestAsync(component).ConfigureAwait(false))
            {
                return;
            }
            
 
            if (_console.ItemType == RequestItemType.Item || _console.ItemType == RequestItemType.Asset || _console.CurrentListInstance.wim.CanContainSingleInstancePerDefinedList)
            {
                //  Handles the list item request.
                await HandleListItemRequestAsync(grid, component, isDeleteTriggered).ConfigureAwait(false);
            }
            else if (_console.ItemType == RequestItemType.Page)
            {
                _console.AddTrace("Monitor", "HandlePageItemRequest(...)");
                //  Handles the page request.
                await HandlePageItemRequestAsync(grid, component, isDeleteTriggered).ConfigureAwait(false);
            }
            else
            {
                //  Handles the browsing request.
                _console.CurrentListInstance.wim.IsSearchListMode = true;
                await HandleBrowsingRequestAsync(grid, component).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Handles a request to the Root of the CMS, this prepares for loading the setting from the Menu -> Homepage 
        /// </summary>
        /// <returns></returns>
        async Task HandleRootRequestAsync() 
        {
            var roleMenus = await Menu.SelectAllAsync().ConfigureAwait(false);
            if (roleMenus.Any())
            {
                IMenu roleMenu = roleMenus.FirstOrDefault(x => x.GroupID == null && x.IsActive == true && x.RoleID == _console.CurrentApplicationUser.RoleID);

                if (roleMenu?.ID > 0)
                {
                    var items = await MenuItem.SelectAllAsync(roleMenu.ID).ConfigureAwait(false);
                    var homepage = items?.FirstOrDefault(x => x.Position == 0);
                    if (homepage?.ID > 0)
                    {
                        switch (homepage.TypeID)
                        {
                            case 1:  // List
                                {
                                    IComponentList implement = await ComponentList.SelectOneAsync(homepage.ItemID);
                                    await _console.ApplyListAsync(implement);
                                }
                                break;
                            case 2:  // Folder
                            case 8:  // Container / Folder
                                {
                                    var folder = await Folder.SelectOneAsync(homepage.ItemID);

                                    _console.CurrentListInstance.wim.IsSearchListMode = true;
                                    _console.CurrentListInstance.wim.m_CurrentFolder = folder;
                                }
                                break;
                            case 5:  // Gallery
                                {
                                    var gallery = await Gallery.SelectOneAsync(homepage.ItemID);

                                    if (!(gallery == null || gallery.ID == 0))
                                    {
                                        var folderEntity = new Folder();
                                        folderEntity.ID = gallery.ID;
                                        folderEntity.ParentID = gallery.ParentID.GetValueOrDefault(0);
                                        folderEntity.Name = gallery.Name;
                                        //  Fix for galleries (add the / at the end for the Level)!
                                        folderEntity.CompletePath = gallery.CompletePath == "/" ? "/" : string.Concat(gallery.CompletePath, "/");
                                        folderEntity.Type = FolderType.Gallery;

                                        _console.CurrentListInstance.wim.IsSearchListMode = true;
                                        _console.CurrentListInstance.wim.m_CurrentFolder = folderEntity;
                                    }
                                }
                                break;
                            case 3:  // Page
                                {
                                    _console.Item = homepage.ItemID;
                                    _console.ItemType = RequestItemType.Page;
                                }
                                break;
                            case 6: // Site (homepage)
                                {
                                    var hpId = (await Site.SelectOneAsync(homepage.ItemID).ConfigureAwait(false)).HomepageID;
                                    if (hpId.GetValueOrDefault(0) > 0)
                                    {
                                        _console.Item = hpId;
                                        _console.ItemType = RequestItemType.Page;
                                    }
                                }
                                break;
                            case 7: // Section
                                {
                                    var folderEntity = new Folder();
                                    switch (homepage.ItemID)
                                    {
                                        case 0:
                                            {
                                                folderEntity = new Folder();
                                                folderEntity.ID = 0;
                                                folderEntity.Name = "Dashboard";
                                                folderEntity.Type = FolderType.Undefined;
                                            }
                                            break;
                                        case 1:
                                            {
                                                folderEntity = Folder.SelectOneBySite(_console.ChannelIndentifier, FolderType.Page);
                                            }
                                            break;
                                        case 2:
                                            {
                                                folderEntity = Folder.SelectOneBySite(_console.ChannelIndentifier, FolderType.List);
                                            }
                                            break;
                                        case 3:
                                            {
                                                Gallery gallery = Gallery.SelectOneRoot();
                                                folderEntity = new Folder();
                                                folderEntity.ID = gallery.ID;
                                                folderEntity.Name = gallery.Name;
                                                folderEntity.Type = FolderType.Gallery;
                                            }
                                            break;
                                        case 4:
                                            {
                                                folderEntity = Folder.SelectOneBySite(_console.ChannelIndentifier, FolderType.Administration);
                                            }
                                            break;
                                    }
                                    _console.CurrentListInstance.wim.IsSearchListMode = true;
                                    _console.CurrentListInstance.wim.m_CurrentFolder = folderEntity;
                                }
                                break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Handles the page item request.
        /// </summary>
        /// <param name="grid">The grid.</param>
        /// <param name="component">The component.</param>
        /// <param name="isDeleteTriggered">if set to <c>true</c> [is delete triggered].</param>
        async Task HandlePageItemRequestAsync(DataGrid grid, Beta.GeneratedCms.Source.Component component, bool isDeleteTriggered)
        {
            _console.CurrentListInstance.wim.IsEditMode = (await _console.CurrentApplicationUser.SelectRoleAsync().ConfigureAwait(false)).CanChangePage;

            Page page = null;
            if (_console.CurrentPage == null)
            {
                page = await Page.SelectOneAsync(_console.Item.Value, false);
                _context.Items.Add("Wim.Page", page);
                _context.Items.Add("Wim.Site", page.Site);
            }

            _console.View = 0;

            bool isPagePublishTriggered = _console.IsPostBack("pagepublish");
            bool isPageOfflineTriggered = _console.IsPostBack("pageoffline");

            bool isPageLocalised = _console.IsPostBack("page.localize");
            bool isPageInherited = _console.IsPostBack("page.inherit");

            int selectedTab = Utility.ConvertToInt(_console.Request.Query["tab"]);
            string section = _console.Request.Query["tab"];

            string pBack = string.Empty;
            if (_console.PostBackStartsWith("pagemod_", out pBack))
            {
                pBack = pBack.Replace("pagemod_", "");
                ICollection<IPageModule> pageModules = default(List<IPageModule>);

                if (_console.Context?.RequestServices?.GetServices<IPageModule>().Any() == true)
                {
                    pageModules = _console.Context.RequestServices.GetServices<IPageModule>().ToList();
                }

                foreach (var pmodule in pageModules)
                {
                    if (pmodule.GetType().Name == pBack)
                    {
                        var moduleResult = await pmodule.ExecuteAsync(_console.CurrentPage, _console.CurrentApplicationUser, _context);
                        if (moduleResult.IsSuccess && string.IsNullOrWhiteSpace(moduleResult.WimNotificationOutput) == false)
                        {
                            _console.CurrentListInstance.wim.Notification.AddNotification(moduleResult.WimNotificationOutput);
                        }
                        else if (string.IsNullOrWhiteSpace(moduleResult.WimNotificationOutput) == false)
                        {
                            _console.CurrentListInstance.wim.Notification.AddError(moduleResult.WimNotificationOutput);
                        }
                    }
                }
            }

            if (string.IsNullOrEmpty(section))
            {
                //  26-08-14:MM
                var sections = _console.CurrentPage.Template.GetPageSections();
                if (sections.Length > 0)
                    section = sections[0];
            }

            string redirect = string.IsNullOrEmpty(section) ? "" : string.Concat("&tab=", section);

            if (_console.IsPostBack("page.translate"))
            {
                _console.CurrentApplicationUser.ShowTranslationView = true;
                await _console.CurrentApplicationUser.SaveAsync().ConfigureAwait(false);

                if (!_console.CurrentListInstance.IsEditMode)
                    _console.Response.Redirect(string.Concat(_console.WimPagePath, "?page=", _console.Item.Value, redirect));
            }

            if (_console.IsPostBack("page.copy"))
            {
                await Framework.Inheritance.Page.CopyFromMasterAsync(_console.Item.Value).ConfigureAwait(false);
                
                // Flush all cache
                await EnvironmentVersion.SetUpdatedAsync().ConfigureAwait(false);

                _console.Response.Redirect(string.Concat(_console.WimPagePath, "?page=", _console.Item.Value, redirect));
            }

            if (_console.IsPostBack("page.normal"))
            {
                _console.CurrentApplicationUser.ShowTranslationView = false;
                await _console.CurrentApplicationUser.SaveAsync().ConfigureAwait(false);

                if (!_console.CurrentListInstance.IsEditMode)
                    _console.Response.Redirect(string.Concat(_console.WimPagePath, "?page=", _console.Item.Value, redirect));
            }

            if (isPagePublishTriggered)
            {
                var pagePublicationHandler = new PagePublication();

                await page.PublishAsync(pagePublicationHandler, _console.CurrentApplicationUser);
                // Flush all cache
                await EnvironmentVersion.SetUpdatedAsync().ConfigureAwait(false);

                //Wim.Framework2.Functions.AuditTrail.Insert(_Console.CurrentApplicationUser, page, Framework2.Functions.Auditing.ActionType.Publish, null);
                _console.Response.Redirect(string.Concat(_console.WimPagePath, "?page=", _console.Item.Value, redirect));
            }
            else if (isPageOfflineTriggered)
            {
                var pagePublicationHandler = new PagePublication();
                await page.TakeDownPageAsync().ConfigureAwait(false);
                // Flush all cache
                await EnvironmentVersion.SetUpdatedAsync().ConfigureAwait(false);

                //Wim.Framework2.Functions.AuditTrail.Insert(_Console.CurrentApplicationUser, page, Framework2.Functions.Auditing.ActionType.TakeOffline, null);
                _console.Response.Redirect(string.Concat(_console.WimPagePath, "?page=", _console.Item.Value, redirect));
            }
            else if (isDeleteTriggered)
            {
                //  Save the version
                var currentversion = await ComponentVersion.SelectAllOnPageAsync(page.ID);
                component.SavePageVersion(page, currentversion, _console.CurrentApplicationUser, true);

                await page.DeleteAsync().ConfigureAwait(false);
                //Wim.Framework2.Functions.AuditTrail.Insert(_Console.CurrentApplicationUser, page, Framework2.Functions.Auditing.ActionType.Remove, null);
                _console.Response.Redirect(string.Concat(_console.WimPagePath, "?folder=", _console.CurrentListInstance.wim.CurrentFolder.ID));
            }
            else if (isPageLocalised)
            {
                page.IsLocalized = true;
                page.Updated = Common.DatabaseDateTime;
                await page.SaveAsync().ConfigureAwait(false);

                //Wim.Framework2.Functions.AuditTrail.Insert(_Console.CurrentApplicationUser, page, Framework2.Functions.Auditing.ActionType.Localised, null);
                _console.Response.Redirect(string.Concat(_console.WimPagePath, "?page=", _console.Item.Value, redirect));

            }
            else if (isPageInherited)
            {
                page.IsLocalized = false;
                page.Updated = Common.DatabaseDateTime;
                await page.SaveAsync().ConfigureAwait(false);

                //Wim.Framework2.Functions.AuditTrail.Insert(_Console.CurrentApplicationUser, page, Framework2.Functions.Auditing.ActionType.Inherited, null);
                _console.Response.Redirect(string.Concat(_console.WimPagePath, "?page=", _console.Item.Value, redirect));
            }

            Page pageInstance;

            GlobalWimControlBuilder = component.CreateContentList(_console, 0, selectedTab == 1, out pageInstance, section);


            // Check if we have to create / update any inherited Page 
            if (_console.IsPostBackSave && pageInstance?.ID > 0)
            {
                var hasInheritedPages = await pageInstance.HasInheritedPagesAsync().ConfigureAwait(false);
                if (_console.Item.Value == 0 || hasInheritedPages == false)
                {
                    await Framework.Inheritance.Page.CreatePageAsync(pageInstance, page.Site).ConfigureAwait(false);
                }
                else
                {
                    await Framework.Inheritance.Page.MovePageAsync(pageInstance, page.Site).ConfigureAwait(false);
                }
            }

            if (!_console.IsAdminFooter)
            {
                GlobalWimControlBuilder.Canvas.Type = CanvasType.ListItem;

                GlobalWimControlBuilder.Leftnav = await _presentationNavigation.NewLeftNavigationAsync(_console, component.m_ButtonList != null ? component.m_ButtonList.ToArray() : null).ConfigureAwait(false);

                GlobalWimControlBuilder.TopNavigation = _presentationNavigation.TopNavigation(_console);
                GlobalWimControlBuilder.Rightnav = _presentationNavigation.RightSideNavigation(_console, component.m_ButtonList != null ? component.m_ButtonList.ToArray() : null);
                GlobalWimControlBuilder.Bottom = _presentationNavigation.NewBottomNavigation(_console, component.m_ButtonList != null ? component.m_ButtonList.ToArray() : null, false);

                await AddToResponseAsync(await _presentationMonitor.GetTemplateWrapperAsync(_console, _Placeholders, _Callbacks, GlobalWimControlBuilder)).ConfigureAwait(false);
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
            _console.View = (int)ContainerView.ItemSelect;

            if (_console.CurrentList.Type == ComponentListType.ListSettings)
                _console.View = (int)ContainerView.ListSettingRequest;

            _console.AddTrace("Monitor", "GetExportOptionUrl(..)");
            if (await GetExportOptionUrlAsync(grid, component).ConfigureAwait(false))
                return;

            await HandleListModuleActionAsync(component, true);

            //  Create the form
            _console.CurrentListInstance.wim.HideTopSectionTag = true;

            if (!_console.IsComponent)
            {
                if (_console.CurrentList.Option_FormAsync && !IsFormatRequest_JSON)
                {
                    _console.CurrentListInstance.wim.DoListInit();
                    GlobalWimControlBuilder = new WimControlBuilder();
                }
                else
                {
                    _console.CurrentListInstance.wim.DoListInit();
                    GlobalWimControlBuilder = await component.CreateListAsync(_console, _console.OpenInFrame, IsFormatRequest_JSON);
                    if (GlobalWimControlBuilder.IsTerminated)
                    {
                        return;
                    }

                    //if (isDeleteTriggered)
                    //{
                    //    //var searchListGrid = GlobalWimControlBuilder.;
                    //    //await AddToResponseAsync(searchListGrid).ConfigureAwait(false);
                    //}
                }

                if (IsFormatRequest_JSON)
                {
                    _console.Response.ContentType = "application/json";

                    _presentationNavigation.RightSideNavigation(_console, component.m_ButtonList != null ? component.m_ButtonList.ToArray() : null, GlobalWimControlBuilder);
                    _presentationNavigation.NewBottomNavigation(_console, component.m_ButtonList != null ? component.m_ButtonList.ToArray() : null, false, GlobalWimControlBuilder);

                    GlobalWimControlBuilder.ApiResponse.CurrentSiteID = _console.ChannelIndentifier;
                    GlobalWimControlBuilder.ApiResponse.ListDescription = _console.CurrentListInstance.wim.CurrentList.Description;
                    GlobalWimControlBuilder.ApiResponse.RedirectUrl = _console.RedirectionUrl;
                    if (!string.IsNullOrWhiteSpace(_console.RedirectionUrl))
                    {
                        _console.Response.StatusCode = 302;
                    }

                    GlobalWimControlBuilder.ApiResponse.IsEditMode = _console.CurrentListInstance.IsEditMode;
                    GlobalWimControlBuilder.ApiResponse.ListTitle = _console.CurrentListInstance.wim.ListTitle;
                    // if this item is a button add it to the button list

                    // [MR:25-05-2021] added for : https://supershift.atlassian.net/browse/FTD-147
                    await GlobalWimControlBuilder.ApiResponse.ApplySharedFieldDataAsync();

                    await AddToResponseAsync(JsonConvert.SerializeObject(GlobalWimControlBuilder.ApiResponse)).ConfigureAwait(false);
                    return;
                }

                else if (IsFormatRequest_AJAX)
                {
                    _console.Response.ContentType = "text/plain";
                    var searchListGrid = GlobalWimControlBuilder.SearchGrid;
                    await AddToResponseAsync(searchListGrid).ConfigureAwait(false);
                    return;
                }
                else
                    //  Needed to NULLafy it as it was required for AJAX call
                    GlobalWimControlBuilder.SearchGrid = null;
            }

            // <------ was here
 
            bool isCopyTriggered = _console.Form("copyparent") == "1";

            if (isCopyTriggered)
            {
                //
                //_Console.CurrentListInstance.DoListDelete(_Console.Item.GetValueOrDefault(0), 0);
                int childID = _console.CurrentListInstance.wim.CurrentSite.ID;
                int parentID = _console.CurrentListInstance.wim.CurrentSite.MasterID.GetValueOrDefault();

                _console.CurrentListInstance.wim.CurrentSite = Site.SelectOne(parentID);
                _console.CurrentListInstance.wim.IsCurrentList = true;
                _console.CurrentListInstance.wim.DoListLoad(_console.Item.GetValueOrDefault(0), 0);


                await AddToResponseAsync(string.Format("{0}<br/>", _console.CurrentListInstance.wim.CurrentSite.ID)).ConfigureAwait(false);
                await AddToResponseAsync(string.Format("{0}<br/>", _console.CurrentListInstance.wim.CurrentSite.MasterID.GetValueOrDefault())).ConfigureAwait(false);
                return;
            }

            //  Is the delete event triggered?
            if (isDeleteTriggered && _console.CurrentListInstance.wim.HasListDelete)
            {
                _console.CurrentListInstance.wim.DoListDelete(_console.Item.GetValueOrDefault(0), 0, null);

                //  Add deletion entry
                ComponentListVersion version = new ComponentListVersion();
                version.SiteID = _console.CurrentListInstance.wim.CurrentSite.ID;
                version.ComponentListID = _console.CurrentListInstance.wim.CurrentList.ID;
                if (_console.Item.HasValue)
                    version.ComponentListItemID = _console.Item.Value;

                version.ApplicationUserID = _console.CurrentApplicationUser.ID;
                version.TypeID = 2;
                version.Save();

                if (_console.OpenInFrame == 0)
                {
                    //  Redirect to the containing folder
                    if (_console.CurrentList.Type == ComponentListType.Documents)
                    {
                        _console.Response.Redirect(string.Concat(_console.WimPagePath, "?gallery=", _console.CurrentListInstance.wim.CurrentFolder.ID));
                    }
                    else if (_console.CurrentListInstance.wim.CurrentFolder.Type == FolderType.Gallery)
                    {
                        _console.Response.Redirect(string.Concat(_console.WimPagePath, "?gallery=", _console.CurrentListInstance.wim.CurrentFolder.ParentID));
                    }
                    else if (_console.CurrentList.Type == ComponentListType.Folders)
                    {
                        _console.Response.Redirect(string.Concat(_console.WimPagePath, "?folder=", _console.CurrentListInstance.wim.CurrentFolder.ParentID));
                    }
                    else if (_console.Group.HasValue)
                    {
                        if (string.IsNullOrWhiteSpace(_console.CurrentListInstance.wim.SearchResultItemPassthroughParameter) == false)
                        {
                            _console.Response.Redirect(_console.CurrentListInstance.wim.SearchResultItemPassthroughParameter, true);
                        }
                        else 
                        {
                            _console.Response.Redirect(_console.UrlBuild.GetListRequest(_console.Group.Value, _console.GroupItem), true);
                        }
                    }
                    else if (_console.CurrentList.Type == ComponentListType.ComponentListProperties)
                    {
                        _console.Response.Redirect(string.Concat(_console.WimPagePath, "?folder=", _console.CurrentListInstance.wim.CurrentFolder.ID));
                    }
                    else
                    {
                        _console.Response.Redirect(_console.CurrentListInstance.wim.GetUrl(new KeyValue[] { new KeyValue("item", true) }));
                    }
                }
            }

            if (_console.IsComponent)
            {
                _console.CurrentListInstance.wim.DoListLoad(_console.Item.GetValueOrDefault(), 0);

                bool isPagePublishTriggered = _console.IsPostBack("pagepublish");
                bool isPageOfflineTriggered = _console.IsPostBack("pageoffline");

                if (isPagePublishTriggered)
                {
                    ComponentVersion.SelectOne(_console.Item.Value).Publish();
                }
                if (isPageOfflineTriggered)
                {
                    ComponentVersion.SelectOne(_console.Item.Value).TakeDown();
                }

                if (isPageOfflineTriggered || isPagePublishTriggered)
                    _console.Response.Redirect(_console.UrlBuild.GetListRequest(_console.CurrentList, (_console.Item.Value)));

                Page pageInstance;

                GlobalWimControlBuilder = component.CreateContentList(_console, 0, true, out pageInstance, null);
                GlobalWimControlBuilder.Canvas.Type = _console.OpenInFrame > 0 ? CanvasType.ListItemInLayer : CanvasType.ListItem;
                GlobalWimControlBuilder.TopNavigation = _presentationNavigation.TopNavigation(_console);
                GlobalWimControlBuilder.Bottom = _presentationNavigation.NewBottomNavigation(_console, component.m_ButtonList != null ? component.m_ButtonList.ToArray() : null, false);
                GlobalWimControlBuilder.Tabularnav = Template.GetTabularTagNewDesign(_console, _console.CurrentList.Name, 0, false);
                GlobalWimControlBuilder.Leftnav = await _presentationNavigation.NewLeftNavigationAsync(_console, component.m_ButtonList != null ? component.m_ButtonList.ToArray() : null).ConfigureAwait(false);

                await AddToResponseAsync(await _presentationMonitor.GetTemplateWrapperAsync(_console, _Placeholders, _Callbacks, GlobalWimControlBuilder));

                return;
            }

            if (!_console.IsAdminFooter)
            {
             
                GlobalWimControlBuilder.TopNavigation = _presentationNavigation.TopNavigation(_console);
                GlobalWimControlBuilder.Canvas.Type = _console.OpenInFrame > 0 ? CanvasType.ListItemInLayer : CanvasType.ListItem;
                GlobalWimControlBuilder.Rightnav = _presentationNavigation.RightSideNavigation(_console, component.m_ButtonList != null ? component.m_ButtonList.ToArray() : null);

                if (
                    GlobalWimControlBuilder.Canvas.Type == CanvasType.ListInLayer ||
                    GlobalWimControlBuilder.Canvas.Type == CanvasType.ListItemInLayer
                    )
                {
                    //  Do nothing, this is an layer and has no leftnavigation.
                }
                else
                    GlobalWimControlBuilder.Leftnav = await _presentationNavigation.NewLeftNavigationAsync(_console, component.m_ButtonList != null ? component.m_ButtonList.ToArray() : null).ConfigureAwait(false);
                GlobalWimControlBuilder.Bottom = _presentationNavigation.NewBottomNavigation(_console, component.m_ButtonList != null ? component.m_ButtonList.ToArray() : null, false);
                GlobalWimControlBuilder.Tabularnav = Template.GetTabularTagNewDesign(_console, _console.CurrentList.Name, 0, false);

                await AddToResponseAsync(await _presentationMonitor.GetTemplateWrapperAsync(_console, _Placeholders, _Callbacks, GlobalWimControlBuilder)).ConfigureAwait(false);
            }
        }

        async Task<bool> GetExportOptionUrlAsync(DataGrid grid, Beta.GeneratedCms.Source.Component component)
        {
            string exportUrl = null;
            //  Export to XLS: XLS Creation URL
            if (_console.IsPostBack("export_xls") || _console.Request.Query["xls"] == "1")
            {
                exportUrl = _console.WimPagePath;

                _console.CurrentListInstance.wim.IsExportMode_XLS = true;

                await component.CreateSearchListAsync(_console, 0);
                var url = grid.GetGridFromListInstanceForXLS(_console, _console.CurrentListInstance, 0);
                if (_console.Request.Query["xp"] == "1")
                {
                    await AddToResponseAsync(url);
                    return true;
                }
                else
                {
                    _console.Response.Redirect(url);
                    return true;
                }
            }
            return false;
        }

        async Task<ModuleExecutionResult> HandleListModuleActionAsync(Beta.GeneratedCms.Source.Component component, bool isItemRequest = false)
        {
            var result = new ModuleExecutionResult() { IsSuccess = false };
            if (_console.PostBackStartsWith("listmod_", out string pBack))
            {
                pBack = pBack.Replace("listmod_", "", StringComparison.InvariantCultureIgnoreCase);

                ICollection<IListModule> listModules = default(List<IListModule>);

                if (_console.Context?.RequestServices?.GetServices<IListModule>().Any() == true)
                {
                    listModules = _console.Context.RequestServices.GetServices<IListModule>().ToList();
                }

                foreach (var pmodule in listModules)
                {
                    if (pmodule.GetType().Name == pBack)
                    {
                        result = await pmodule.ExecuteAsync(_console.CurrentListInstance, _console.CurrentApplicationUser, _context);
                    }
                }

                if (string.IsNullOrWhiteSpace(result.WimNotificationOutput) == false)
                {
                    _console.CurrentListInstance.wim.Notification.AddNotification(result.WimNotificationOutput, result.IsSuccess ? "Success" : "Error", true);
                }
            }

            return result;
        }

        bool IsFormatRequest_AJAX
        {
            get
            {
                return !string.IsNullOrEmpty(_console.Form(Constants.AJAX_PARAM));
            }
        }

        bool IsFormatRequest_JSON
        {
            get
            {
                return _console.Form(Constants.JSON_PARAM) == "1" || (_console.Request.ContentType?.Contains("json")).GetValueOrDefault();
            }
        }

        /// <summary>
        /// Handles the browsing request.
        /// </summary>
        /// <param name="grid">The grid.</param>
        /// <param name="component">The component.</param>
        async Task HandleBrowsingRequestAsync(DataGrid grid, Beta.GeneratedCms.Source.Component component)
        {
            _console.CurrentListInstance.wim.DoListInit();

            _console.AddTrace("Monitor", "HandleListItemRequest.Init");
            _console.View = 2;
            _console.CurrentListInstance.wim.IsEditMode = true;

            //if (!IsFormatRequest)
            //{
            _console.AddTrace("Monitor", "GetExportOptionUrl(..)");
            if (await GetExportOptionUrlAsync(grid, component))
                return;


            // Execute module
            var executeModule = await HandleListModuleActionAsync(component, false);
            if (executeModule.IsSuccess)
            {
                if (string.IsNullOrWhiteSpace(executeModule.RedirectUrl) == false)
                { 
                    _console.Redirect(executeModule.RedirectUrl, true);
                }
            }

            _console.AddTrace("Monitor", "CreateSearchList(..)");

            GlobalWimControlBuilder = await component.CreateSearchListAsync(_console, 0);
            GlobalWimControlBuilder.Canvas.Type = _console.OpenInFrame > 0 ? CanvasType.ListInLayer : CanvasType.List;

            if (_console.OpenInFrame > 0)
                _console.CurrentListInstance.wim.Page.HideTabs = true;
            //}

            StringBuilder searchListGrid = new StringBuilder();
          
            _console.AddTrace("Monitor", "GetGridFromListInstance(..)");

            if (IsFormatRequest_JSON)
            {
                _console.Response.ContentType = "application/json";
                searchListGrid = grid.GetGridFromListInstanceForJSON(_console.CurrentListInstance.wim, _console, 0, false, true);

                await AddToResponseAsync(searchListGrid.ToString()).ConfigureAwait(false);
                return;
            }
            if (IsFormatRequest_AJAX)
            {
                _console.Response.ContentType = "text/plain";
                searchListGrid = new StringBuilder();
                while (_console.CurrentListInstance.wim.NextGrid())
                {
                    bool hasNoTitle = string.IsNullOrEmpty(_console.CurrentListInstance.wim.m_DataTitle);
                    var titleText = hasNoTitle ? null : $"</section><section class=\"{_console.CurrentListInstance.wim.Page.Body.Grid.ClassName}\"><h2>{_console.CurrentListInstance.wim.m_DataTitle}</h2>";
                    var gridText = grid.GetGridFromListInstance(_console.CurrentListInstance.wim, _console, 0, false, _console.CurrentListInstance);

                    searchListGrid.Append(titleText);
                    searchListGrid.Append(gridText);
                }

                await AddToResponseAsync(searchListGrid.ToString()).ConfigureAwait(false);
                return;
            }
            if (_console.CurrentListInstance.wim.CurrentList.Option_SearchAsync)
            {
                if (_console.OpenInFrame > 0)
                {
                    searchListGrid.Append($"<section id=\"datagrid\" class=\"{_console.CurrentListInstance.wim.Page.Body.Grid.ClassName} async\"> </section>");
                }
                else
                {
                    searchListGrid.Append(' ');
                }
            }
            else
            {
                searchListGrid = new StringBuilder();
                while (_console.CurrentListInstance.wim.NextGrid())
                {
                    bool hasNoTitle = string.IsNullOrEmpty(_console.CurrentListInstance.wim.m_DataTitle);
                    var titleText = hasNoTitle ? null : $"</section><section class=\"{_console.CurrentListInstance.wim.Page.Body.Grid.ClassName}\"><h2>{_console.CurrentListInstance.wim.m_DataTitle}</h2>";
                    var gridText = grid.GetGridFromListInstance(_console.CurrentListInstance.wim, _console, 0, false, _console.CurrentListInstance);

                    searchListGrid.Append(titleText);
                    searchListGrid.Append(gridText);
                }
            }

            _Console.AddTrace("Monitor", "AddToResponse(..)");

            GlobalWimControlBuilder.SearchGrid = searchListGrid.ToString();
            GlobalWimControlBuilder.TopNavigation = _presentationNavigation.TopNavigation(_console);
            GlobalWimControlBuilder.Bottom = _presentationNavigation.NewBottomNavigation(
                _console,
                component.m_ButtonList != null
                    ? component.m_ButtonList.ToArray()
                    : null,
                !GlobalWimControlBuilder.IsNull
            );

            GlobalWimControlBuilder.Tabularnav = Template.GetTabularTagNewDesign(_console, _console.CurrentList.Name, 0, false);
            GlobalWimControlBuilder.Rightnav = _presentationNavigation.RightSideNavigation(_console, component.m_ButtonList != null ? component.m_ButtonList.ToArray() : null);
            GlobalWimControlBuilder.Leftnav = await _presentationNavigation.NewLeftNavigationAsync(_console, component.m_ButtonList != null ? component.m_ButtonList.ToArray() : null).ConfigureAwait(false);

            await AddToResponseAsync(await _presentationMonitor.GetTemplateWrapperAsync(_console, _Placeholders, _Callbacks, GlobalWimControlBuilder)).ConfigureAwait(false);
        }


        async Task<bool> HandleAsyncRequestAsync(Beta.GeneratedCms.Source.Component component)
        {
            if (_console.CurrentListInstance == null) return false;
            var asyncResult = Utils.GetAsyncQuery(_console);
            if (asyncResult == null)
                return false;

            if (_console.CurrentListInstance.wim.HasListAsync)
            {
                _console.HasAsyncEvent = true;
                ComponentAsyncEventArgs eventArgs = new ComponentAsyncEventArgs(_console.Item.GetValueOrDefault());

                eventArgs.Query = asyncResult.SearchQuery;
                eventArgs.SearchType = asyncResult.SearchType;
                eventArgs.Property = asyncResult.Property;

                eventArgs.Data = new ASyncResult();
                eventArgs.Data.Property = asyncResult.Property;
                eventArgs.ApplyData(component, _console);
                eventArgs.SelectedGroupItemKey = _console.GroupItem.GetValueOrDefault();
                eventArgs.SelectedGroupKey = _console.Group.GetValueOrDefault();

                var result = _console.CurrentListInstance.wim.DoListAsync(eventArgs);

                var options = new JsonSerializerOptions { 
                    IgnoreNullValues = true,
                    PropertyNameCaseInsensitive = true,
                    IgnoreReadOnlyProperties = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                string val = System.Text.Json.JsonSerializer.Serialize(result.Data, options);
                _console.Response.ContentType = "application/json";
                await AddToResponseAsync(val).ConfigureAwait(false);

                return true;
            }
            return false;
        }

        async Task<bool> HandleSortingRequestAsync()
        {
            // Is this a sorting Request ?
            if (_console.Request?.Query?.ContainsKey("sortF") == true && _console.Request?.Query?.ContainsKey("sortT") == true)
            {
                int list = Utility.ConvertToInt(_console.Request.Query["list"]);
                int sortF = Utility.ConvertToInt(_console.Request.Query["sortF"]);
                int sortT = Utility.ConvertToInt(_console.Request.Query["sortT"]);

                if (list > 0 && sortF > 0 && sortT > 0)
                {
                    IComponentList implement = ComponentList.SelectOne(list);
                    IComponentListTemplate currentListInstance = implement.GetInstance(_console);

                    if (currentListInstance != null)
                    {
                       return await currentListInstance.UpdateSortOrderAsync(sortF, sortT).ConfigureAwait(false);
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Handles the action request.
        /// </summary>
        void HandleActionRequest()
        {
            if (_console.CurrentListInstance == null || string.IsNullOrEmpty(_console.CurrentListInstance.wim.PostbackValue))
            {
                return;
            }

            // 
            switch (_console.CurrentListInstance.wim.PostbackValue)
            {
                case "PageContentPublication":
                    {
                        _console.CurrentListInstance.wim.Notification.AddNotification("The webcontent has been refreshed.");
                    } break;
            }
        }

        /// <summary>
        /// Redirects to channel home page.
        /// </summary>
        /// <param name="siteID">The site ID.</param>
        async Task RedirectToChannelHomePageAsync(int siteID)
        {
            var currentUrl = "";
            if (_console.Request.PathBase.HasValue)
            {
                currentUrl = _console.Request.PathBase.ToString();
            }
            currentUrl += _console.Request.Path;

            //  Find the default homepage in the menu section
            var defaultHome = await _console.UrlBuild.GetHomeRequestAsync(siteID).ConfigureAwait(false);
            if (currentUrl.Equals(defaultHome, StringComparison.InvariantCultureIgnoreCase) == false)
            {
                _console.Response.Redirect(defaultHome);
            }
        }

        async Task<bool> CheckSecurityAsync(bool reStartWithNotificationList)
        {
            var currentUrl = "";
            if (_console.Request.PathBase.HasValue)
            {
                currentUrl = _console.Request.PathBase.ToString();
            }
            currentUrl += _console.Request.Path;

            var sites = await _console.CurrentListInstance.wim.CurrentApplicationUserRole.SitesAsync(_console.CurrentApplicationUser).ConfigureAwait(false);

            //  ACL: Sites
            if (!_console.CurrentListInstance.wim.CurrentApplicationUserRole.All_Sites
                && (sites).Length == 0
                && !reStartWithNotificationList)
            {
                throw new Exception(ErrorCode.GetMessage(1002, _console.CurrentApplicationUser.LanguageCulture));
            }

            //  ACL: Sites
            if (!_console.CurrentListInstance.wim.CurrentApplicationUserRole.All_Sites)
            {
                if (!await _console.CurrentListInstance.wim.CurrentSite.HasRoleAccessAsync(_console.CurrentListInstance.wim.CurrentApplicationUser).ConfigureAwait(false))
                {
                    var allowed = await _console.CurrentListInstance.wim.CurrentApplicationUserRole.SitesAsync(_console.CurrentApplicationUser).ConfigureAwait(false);
                    if (allowed != null && allowed.Length > 0)
                    {
                        await RedirectToChannelHomePageAsync(allowed[0].ID).ConfigureAwait(false);
                        return false;
                    }
                    else
                    {
                        if ((await _console.CurrentListInstance.wim.CurrentApplicationUser.SitesAsync(AccessFilter.RoleAndUser).ConfigureAwait(false))?.Length > 0)
                        {
                            var siteId = (await _console.CurrentListInstance.wim.CurrentApplicationUser.SitesAsync(AccessFilter.RoleAndUser).ConfigureAwait(false))[0].ID;
                            var newUrl = _console.GetWimPagePath(siteId);
                            if (currentUrl.Equals(newUrl, StringComparison.InvariantCultureIgnoreCase) == false)
                            {
                                _console.Response.Redirect(newUrl);
                            }

                            return false;
                        }
                        else
                        {
                            throw new Exception("There are no active accessible channels available.");
                        }
                    }
                }
            }
            else
            {
                //  CHECK FOR UserBased exceptions!!!
            }

            if (
                _console.CurrentListInstance.wim.CurrentFolder.ID == 0 && 
                string.IsNullOrEmpty(_console.Request.Query["dashboard"]))
            {
                await RedirectToChannelHomePageAsync(_console.ChannelIndentifier).ConfigureAwait(false);
                return false;
            }

            //  ACL: Folders
            if (_console.CurrentListInstance.wim.CurrentFolder.Type != FolderType.Gallery
                && !_console.CurrentListInstance.wim.CurrentApplicationUserRole.All_Folders)
            {
                if (!await _console.CurrentListInstance.wim.CurrentFolder.HasRoleAccessAsync(_console.CurrentListInstance.wim.CurrentApplicationUser).ConfigureAwait(false))
                {
                    if (_console.CurrentListInstance.wim.CurrentFolder.ParentID.HasValue)
                    {
                        _console.Response.Redirect(_console.UrlBuild.GetFolderRequest(_console.CurrentListInstance.wim.CurrentFolder.ParentID.Value));
                    }
                    var newUrl = _console.WimPagePath;
                    if (currentUrl.Equals(newUrl, StringComparison.InvariantCultureIgnoreCase) == false)
                    {
                        _console.Response.Redirect(newUrl);
                    }

                    return false;
                }
            }

            //  Check environment
            bool approved = false;
            switch (_console.CurrentListInstance.wim.CurrentFolder.Type)
            {
                default:
                case FolderType.Undefined:
                    {
                        approved = true; break;
                    }
                case FolderType.Page:
                    {
                        approved = _console.CurrentListInstance.wim.CurrentApplicationUserRole.CanSeePage;
                    }
                    break;
                case FolderType.List:
                    {
                        if (_console.CurrentListInstance.wim.CurrentList.Type == ComponentListType.Browsing)
                        {
                            approved = _console.CurrentListInstance.wim.CurrentApplicationUserRole.CanSeeFolder;
                            if (!approved)
                            {
                                var newUrl = _console.WimPagePath;
                                
                                if (currentUrl.Equals(newUrl, StringComparison.InvariantCultureIgnoreCase) == false)
                                {
                                    _console.Response.Redirect(newUrl);
                                }

                                return false;
                            }
                        }
                        approved = _console.CurrentListInstance.wim.CurrentApplicationUserRole.CanSeeList;
                        if (_console.CurrentList.Type == ComponentListType.Undefined && !_console.CurrentListInstance.wim.CurrentApplicationUserRole.All_Lists)
                        {
                            if (!_console.CurrentListInstance.wim.CurrentList.HasRoleAccess(_console.CurrentListInstance.wim.CurrentApplicationUser))
                            {
                                var newUrl = _console.WimPagePath;
                                if (currentUrl.Equals(newUrl, StringComparison.InvariantCultureIgnoreCase) == false)
                                {
                                    _console.Response.Redirect(newUrl);
                                }

                                return false;
                            }
                        }
                    }
                    break;
                case FolderType.Gallery:
                    {
                        approved = true;
                        if (!_console.CurrentListInstance.wim.CurrentApplicationUserRole.All_Galleries)
                        {
                            Gallery currentGallery = Gallery.SelectOne(_console.CurrentListInstance.wim.CurrentFolder.ID);
                            if (!currentGallery.HasRoleAccess(_console.CurrentListInstance.wim.CurrentApplicationUser))
                            {
                                if (currentGallery.ParentID.HasValue)
                                {
                                    _console.Response.Redirect(_console.UrlBuild.GetGalleryRequest(currentGallery.ParentID.Value));
                                }

                                var newUrl = _console.WimPagePath;
                                if (currentUrl.Equals(newUrl, StringComparison.InvariantCultureIgnoreCase) == false)
                                {
                                    _console.Response.Redirect(newUrl);
                                }

                                return false;
                            }
                        }
                    }
                    break;
                case FolderType.Administration:
                    {
                        approved = _console.CurrentListInstance.wim.CurrentApplicationUserRole.CanSeeAdmin;
                    }
                    break;
            }

            if (!approved)
            {
                if (_console.CurrentListInstance.wim.CurrentList.Type != ComponentListType.Images
                    && _console.CurrentListInstance.wim.CurrentList.Type != ComponentListType.Documents
                    && _console.CurrentListInstance.wim.CurrentList.Type != ComponentListType.Links)
                {
                    var newUrl = _console.WimPagePath;
                    if (currentUrl.Equals(newUrl, StringComparison.InvariantCultureIgnoreCase) == false)
                    {
                        _console.Response.Redirect(newUrl);
                    }

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
            if (_console.CurrentListInstance.wim.CurrentFolder == null)
                throw new Exception("No containing folder found for the requested item!");

            //  If in browsing mode, set the page title to the current folder
            if (_console.Logic == 0 && !_console.Item.HasValue && !_console.CurrentListInstance.wim.CurrentFolder.IsNewInstance)
                _console.Title = _console.CurrentListInstance.wim.CurrentFolder.Name;
            return true;
        }

        /// <summary>
        /// Checks the site for channel requests.
        /// </summary>
        async Task CheckSiteAsync()
        {
            if (_console.CurrentList.SiteID.HasValue && _console.ChannelIndentifier != _console.CurrentList.SiteID.Value)
            {
                if (_console.CurrentList.IsInherited)
                {
                    return;
                }

                var site = await Site.SelectOneAsync(_console.CurrentList.SiteID.Value).ConfigureAwait(false);
                if (site.Type.HasValue)
                {
                    return;
                }

                _console.Response.Redirect(_console.CurrentListInstance.wim.GetUrl());
            }
        }

        /// <summary>
        /// Sets the form modes (Edit/Save,Delete, etc)
        /// </summary>
        /// <param name="isDeleteMode">if set to <c>true</c> [is delete mode].</param>
        async Task<bool> SetFormModesAsync()
        {
            await _console.LoadJsonStreamAsync().ConfigureAwait(false);

            //  Is the form state in editmode?
            _console.CurrentListInstance.wim.IsEditMode = _console.IsPostBack("edit")
                || _console.IsPostBack("save")
                || _console.CurrentListInstance.wim.OpenInEditMode
                || _console.Request.Query["item"] == "0"
                || _console.Request.Query["asset"] == "0"
                || (_console.JsonReferrer != null && _console.JsonReferrer.Equals("edit"))
                || _console.JsonForm != null;

            //  Create new page
            if (!_console.CurrentListInstance.wim.IsEditMode
                && _console.ItemType == RequestItemType.Page
                && _console.Request.Headers["Referer"].FirstOrDefault() != null
                && _console.Request.Headers["Referer"].FirstOrDefault().Contains("item=0")
                )
            {
                _console.CurrentListInstance.wim.IsEditMode = true;
            }

            //  Is the save link clicked?
            _console.CurrentListInstance.wim.IsSaveMode = _console.IsPostBack("save") || _console.IsPostBack("saveNew");
            _console.IsAdminFooter = Utility.ConvertToInt(_console.Request.Query["adminFooter"]) == 1;

            //  Is the delete link clicked?
            bool isDeleteMode = (_console.IsPostBack("delete") || _console.Request.Method == "DELETE");
            _console.CurrentListInstance.wim.IsDeleteMode = isDeleteMode;

            //  Set the developer mode
            if (_console.PostbackValue == "dev_showhidden")
            {
                _console.CurrentApplicationUser.ShowHidden = true;
                _console.CurrentApplicationUser.Save();
            }
            else if (_console.PostbackValue == "dev_showvisible")
            {
                _console.CurrentApplicationUser.ShowHidden = false;
                _console.CurrentApplicationUser.Save();
            }
            return isDeleteMode;
        }

        /// <summary>
        /// Obtain request.path and querystring requests
        /// </summary>
        async Task SetRequestTypeAsync()
        {
            var folderId = default(int?);
            var targetname = default(string);

            // extract url details
            var wim = CommonConfiguration.PORTAL_PATH.ToLower();
            var url = _console.Request.Path.Value.ToLower();
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
                if (Utility.IsNumeric(candidate))
                {
                    _console.ChannelIndentifier = Utility.ConvertToInt(candidate, Data.Environment.Current.DefaultSiteID.GetValueOrDefault());
                }
                else
                {
                    var allSites = await Site.SelectAllAsync().ConfigureAwait(false);
                    var selection = allSites.FirstOrDefault(x => x.Name.Equals(candidate, StringComparison.CurrentCultureIgnoreCase));
                    if (selection != null)
                    {
                        n++;
                        _console.ChannelIndentifier = selection.ID;
                    }
                }

                if (_console.ChannelIndentifier == 0)
                {
                    _console.ChannelIndentifier = Data.Environment.Current.DefaultSiteID.GetValueOrDefault();
                    if (_console.ChannelIndentifier == 0)
                    {
                        var allSites = await Site.SelectAllAsync().ConfigureAwait(false);
                        _console.ChannelIndentifier = allSites[0].ID;
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
                        _console.ItemType = RequestItemType.Page;
                    }
                    else if (typecandidate.Equals("assets", StringComparison.CurrentCultureIgnoreCase))
                    {
                        n++;
                        _console.ItemType = RequestItemType.Asset;
                    }
                    else
                    {
                        _console.ItemType = RequestItemType.Item;
                    }

                }

                if (split.Length - n - 1 > 0)
                {
                    var completepath = string.Concat("/", string.Join("/", split.Skip(n).Take(split.Length - n - 1)), "/");
                    _console.CurrentFolder = await Folder.SelectOneAsync(Utils.FromUrl(completepath), _console.ChannelIndentifier).ConfigureAwait(false);

                    if (_console.CurrentFolder?.ID > 0)
                    {
                        folderId = _console.CurrentFolder.ID;
                    }
                }

                if (split.Length - n > 0)
                {
                    targetname = split.Last();
                }
            }
            else
            {
                if (_console.ChannelIndentifier == 0)
                {
                    _console.ChannelIndentifier = Data.Environment.Current.DefaultSiteID.GetValueOrDefault();
                    if (_console.ChannelIndentifier == 0)
                    {
                        var allSites = await Site.SelectAllAsync().ConfigureAwait(false);
                        _console.ChannelIndentifier = allSites[0].ID;
                    }
                }
            }

            _console.ItemType = RequestItemType.Undefined;

            //  Verify paging page
            _console.ListPagingValue = _console.Request.Query["set"];
            _console.Group = Utility.ConvertToIntNullable(_console.Request.Query["group"]);
            _console.GroupItem = Utility.ConvertToIntNullable(_console.Request.Query["groupitem"]);

            //  Verify page request
            _console.Item = Utility.ConvertToIntNullable(_console.Request.Query["page"], false);
            if (_console.Item.HasValue)
            {
                await _console.ApplyListAsync(ComponentListType.Browsing).ConfigureAwait(false);
                _console.ItemType = RequestItemType.Page;
                return;
            }

            //  Verify asset request
            _console.Item = Utility.ConvertToIntNullable(_console.Request.Query["asset"], false);
            if (_console.Item.HasValue)
            {
                await _console.ApplyListAsync(ComponentListType.Documents).ConfigureAwait(false);
                _console.ItemType = RequestItemType.Asset;
                return;
            }

            //  Verify dashboard request
            _console.Item = Utility.ConvertToIntNullable(_console.Request.Query["dashboard"], false);
            if (_console.Item.HasValue)
            {
                _console.ItemType = RequestItemType.Dashboard;
                return;
            }

            //  Verify list-item request
            _console.Item = Utility.ConvertToIntNullable(_console.Request.Query["item"], false);
            if (_console.Item.HasValue)
            {
                _console.ItemType = RequestItemType.Item;
            }

            //  Apply the current component list based on the roaming environment settings
            await ApplyComponentListAsync(folderId, targetname).ConfigureAwait(false);
        }

        /// <summary>
        /// Outputs the ajax request.
        /// </summary>
        /// <returns>Continue processing page?</returns>
        async Task<bool> OutputAjaxRequestAsync()
        {
            if (!string.IsNullOrEmpty(_console.Request.Query["xml"]))
            {
                _console.Response.ContentType = "text/xml";
                //_Console.Response.Cache.SetCacheability(HttpCacheability.NoCache);
                //_Console.Response.Cache.SetAllowResponseInBrowserHistory(false);

                var type = Utility.ConvertToIntNullable(_console.Request.Query["xml"]);
                if (type.HasValue)
                {
                    WimControlBuilder build = new WimControlBuilder();
                    MetaData meta = new MetaData();
                    meta.ContentTypeSelection = type.Value.ToString();
                    var element = meta.GetContentInfo();
                    string name = string.Concat(_console.Request.Query["id"], "__", type.Value, "__", _console.Request.Query["index"]);


                    ((ContentSharedAttribute)element).ID = name;
                    ((ContentSharedAttribute)element).OverrideTableGeneration = true;
                    ((ContentSharedAttribute)element).Expression = (_console.Request.Query["w"] == "2" ? OutputExpression.FullWidth : OutputExpression.Alternating);
                    ((ContentSharedAttribute)element).Console = _console;
                    ((ContentSharedAttribute)element).IsBluePrint = true;
                    if (element.ContentTypeSelection == ContentType.Binary_Image)
                        ((Framework.ContentInfoItem.Binary_ImageAttribute)element).GalleryPropertyUrl = _console.Request.Query["gallery"];


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

                    await AddToResponseAsync(build.ToString()).ConfigureAwait(false);
                }


                if (_console.Request.Query["xml"] == "component")
                {
                    var page = Utility.ConvertToInt(_console.Request.Query["page"]);
                    var target = _console.Request.Query["tab"];
                    if (string.IsNullOrEmpty(target) && page > 0)
                    {
                        var pageInstance = await Page.SelectOneAsync(page).ConfigureAwait(false);
                        var sections = pageInstance.Template.GetPageSections();
                        if (sections != null)
                        {
                            target = sections.FirstOrDefault();
                        }
                    }

                    var content = await Beta.GeneratedCms.Source.Xml.Component.GetAsync(_console, Utility.ConvertToInt(_console.Request.Query["id"]), page, Utility.ConvertToInt(_console.Request.Query["cmpt"]), target).ConfigureAwait(false);
                    await AddToResponseAsync(content).ConfigureAwait(false);
                }

                return false;
            }
            return true;
        }

        /// <summary>
        /// Apply the current component list based on the roaming environment settings
        /// </summary>
        private async Task<bool> ApplyComponentListAsync(int? folderId, string name)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                var urldecrypt = Utils.FromUrl(name);
                var list = await ComponentList.SelectOneAsync(urldecrypt, folderId).ConfigureAwait(false);
                if (list?.ID > 0)
                {
                    return await _console.ApplyListAsync(list).ConfigureAwait(false);
                }
            }

            //  If the list is not know, take the default list in stead (browsing)
            if (!string.IsNullOrEmpty(_console.Request.Query["list"]))
            {
                //  The list reference can be a INT or a GUID
                return await _console.ApplyListAsync(_console.Request.Query["list"]).ConfigureAwait(false);
            }
            else if (_console.ItemType == RequestItemType.Asset)
            {
                await _console.ApplyListAsync(ComponentListType.Documents).ConfigureAwait(false);
            }
            else
            {
                await _console.ApplyListAsync(typeof(AppCentre.Data.Implementation.Browsing)).ConfigureAwait(false);
            }

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
                if (_console.IsPostBack("logout") || _console.Request.Query.ContainsKey("logout"))
                {
                    await LogoutViaSingleSignOnAsync().ConfigureAwait(false);
                    await LogoutIdentityAsync().ConfigureAwait(false);
                }
            }

            // Load the current application user
            await _console.LoadCurrentApplicationUserAsync();

            // Authenticate via Azure Active Directory
            await AuthenticateViaSingleSignOnAsync(true).ConfigureAwait(false);

            //  Check roaming profile
            if (!showLogin && _console.CurrentApplicationUser != null)
            {
                await ValidateHijackResetAsync().ConfigureAwait(false);
                return true;
            }
            else
            {
                string reaction = await _presentationMonitor.GetLoginWrapperAsync(_console, _Placeholders, _Callbacks);
                if (!string.IsNullOrEmpty(reaction))
                {
                    await AddToResponseAsync(reaction).ConfigureAwait(false);
                    return false;
                }
            }
            return true;
        }

        async Task ValidateHijackResetAsync()
        {
            if (_console.Request.QueryString.HasValue 
                && _console.Request.QueryString.Value.Equals("?reset", StringComparison.CurrentCultureIgnoreCase))
            {
                if (_console.CurrentVisitor != null && _console.CurrentVisitor.Data != null)
                {
                    if (!_console.CurrentVisitor.Data["Wim.Reset.Me"].IsNull)
                    {
                        if (Guid.TryParse(_console.CurrentVisitor.Data["Wim.Reset.Me"].Value, out var id))
                        {
                            var user = await ApplicationUser.SelectOneAsync(id).ConfigureAwait(false);
                            if (user?.ID > 0)
                            {
                                _console.CurrentVisitor.Data.Apply("Wim.Reset.Me", null);
                                _console.CurrentVisitor.ApplicationUserID = user.ID;
                                _console.CurrentApplicationUser = user;
                                _console.SaveVisit();

                                user.LastLoggedVisit = DateTime.UtcNow;
                                await user.SaveAsync().ConfigureAwait(false);

                                await new AuditTrail()
                                {
                                    Action = ActionType.Login,
                                    Type = ItemType.Undefined,
                                    ItemID = _console.CurrentApplicationUser.ID,
                                    Message = "Reset impersonation",
                                    Created = user.LastLoggedVisit.Value
                                }.InsertAsync().ConfigureAwait(false);
                            }

                        }
                    }
                }
            }
        }

        async Task LogoutIdentityAsync()
        {
            // Check if we have a context User signed in
            if (_context?.User?.Identity?.IsAuthenticated == true)
            {
                await _context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme).ConfigureAwait(false);
            }
        }

        async Task LogoutViaSingleSignOnAsync()
        {
            _console.CurrentVisitor.ApplicationUserID = null;
            _console.CurrentVisitor.Jwt = null;
            await _console.CurrentVisitor.SaveAsync().ConfigureAwait(false);
      
            if (_configuration.GetValue<bool>("mediakiwi:authentication"))
            {
                _context.Response.Redirect($"{_console.CurrentDomain}/.auth/logout?post_logout_redirect_uri={_console.GetWimPagePath(null)}");
            }
            else
            {
                _console.Response.Redirect(_console.WimPagePath);
            }
        }

        internal async Task AuthenticateViaSingleSignOnAsync(bool redirectOnAnonymous, bool outputRedirectPage = false)
        {
            if (WimServerConfiguration.Instance.Authentication != null && WimServerConfiguration.Instance.Authentication.Aad != null && WimServerConfiguration.Instance.Authentication.Aad.Enabled && _console.CurrentApplicationUser == null)
            {
                var jwt = _console.GetSafePost("id_token");
                if (!string.IsNullOrEmpty(jwt))
                {
                    string email = await _oAuth2Logic.ExtractUpnAsync(WimServerConfiguration.Instance.Authentication, jwt);
                
                    if (!string.IsNullOrEmpty(email))
                    {
                        // do login
                        var applicationUser = ApplicationUser.SelectOne(email, true);
                        if (applicationUser != null && !applicationUser.IsNewInstance)
                        {
                            _console.CurrentApplicationUser = applicationUser;
                            var now = DateTime.UtcNow;

                            await new AuditTrail()
                            {
                                Action = ActionType.Login,
                                Type = ItemType.Undefined,
                                ItemID = _console.CurrentApplicationUser.ID,
                                Message = "Claim based (id_token)",
                                Created = now
                            }.InsertAsync().ConfigureAwait(false);

                            _console.CurrentVisitor.Jwt = jwt;
                            _console.CurrentApplicationUser.LastLoggedVisit = now;
                            await _console.CurrentApplicationUser.SaveAsync().ConfigureAwait(false);

                            _console.SaveVisit();
                            _console.SetClientRedirect(new Uri(_console.GetSafePost("state")), true);

                            if (outputRedirectPage)
                            {
                                var presentation = new Framework.Presentation.Presentation();

                                await _console.ApplyListAsync(ComponentListType.Browsing).ConfigureAwait(false);

                                var output = await presentation.GetTemplateWrapperAsync(_console, null, null, null);
                                await _console.Response.WriteAsync(output).ConfigureAwait(false);
                            }
                            return;
                        }
                    }
                }

                if (redirectOnAnonymous)
                {
                    if (_console.CurrentApplicationUser != null && _console.CurrentApplicationUser.IsActive)
                    {
                        // do nothing, user is logged in.
                    }
                    else
                    {
                        var url = _console.Url;
                        if (url.Contains("?logout", StringComparison.CurrentCultureIgnoreCase))
                        {
                            url = _console.GetWimPagePath(null);
                        }
                        _context.Response.Redirect(OAuth2Logic.AuthenticationUrl(url, _console.CurrentDomain).ToString());
                    }
                }
            }
        }

        /// <summary>
        /// Adds to response.
        /// </summary>
        /// <param name="output">The output.</param>
        async Task AddToResponseAsync(string output)
        {
            if (outputHTML == null)
                outputHTML = new StringBuilder();

            if (IsLoadedInThinLayer)
                outputHTML.Append(output);
            else
            {
                if (_console.Request.Query["split"] == "homeArticle")
                {
                    var split = output.Split(new string[] { @"<article id=""homeArticle"">", "</article>" }, StringSplitOptions.RemoveEmptyEntries);
                    output = split[1];
                }
                Body = output;
                await _console.Response.WriteAsync(output).ConfigureAwait(false);
            }
        }

        public string Body { get; set; }
    }

}
