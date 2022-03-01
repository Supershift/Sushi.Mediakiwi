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
using Sushi.Mediakiwi.Extention;
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

namespace Sushi.Mediakiwi.UI
{
    public class Monitor
    {
        private IHostEnvironment _env;
        private HttpContext _Context;
        private Beta.GeneratedCms.Console _Console;
        private iPresentationMonitor _PresentationMonitor;
        private iPresentationNavigation _PresentationNavigation;
        private IServiceProvider _serviceProvider;
        Dictionary<GlobalPlaceholder, string> _Placeholders;
        Dictionary<CallbackTarget, List<ICallback>> _Callbacks;
        bool IsLoadedInThinLayer;
        public StringBuilder outputHTML;
        internal WimControlBuilder GlobalWimControlBuilder;
        private readonly IConfiguration _configuration;

        public Monitor(HttpContext context, IHostEnvironment env, IConfiguration configuration)
        {
            _env = env;
            _Context = context;
            _configuration = configuration;
            _Console = new Beta.GeneratedCms.Console(context, _env);
            _Console.Configuration = configuration;

            _PresentationMonitor = new Framework.Presentation.Presentation();
            _PresentationNavigation = new Framework.Presentation.Logic.Navigation();
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
                            await HandleExceptionAsync(_Context, ex, notification);
                        }
                    }
                    catch
                    {
                        await HandleExceptionAsync(_Context, ex, null);
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
                if (_Console.Request.Query.ContainsKey(dict))
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
            _Console.CurrentEnvironment = Data.Environment.Current;
            var environmentVersion = await EnvironmentVersion.SelectAsync().ConfigureAwait(false);

            // Validate load balanced cache state
            var updated = environmentVersion.Updated.GetValueOrDefault(DateTime.UtcNow);
            await Data.Caching.Configuration.CacheManager.ValidateAsync(updated).ConfigureAwait(false);

            var path = _Console.AddApplicationPath(CommonConfiguration.PORTAL_PATH);
            var requestPath = _Console.AddApplicationPath(_Console.Request.Path.Value);
            if (!requestPath.StartsWith(path, StringComparison.CurrentCultureIgnoreCase))
            {
                return;
            }

            await _Console.SetDateFormatAsync();

            bool forcelogin = 
                //_Console.Request.Path.Equals($"{Data.Environment.Current.RelativePath}/login", StringComparison.CurrentCultureIgnoreCase)
                _Console.Url.Contains($"{path}?reset=", StringComparison.CurrentCultureIgnoreCase)
                || _Console.Url.EndsWith($"{path}?reminder", StringComparison.CurrentCultureIgnoreCase)
                ;

            if (!await CheckRoamingApplicationUserAsync(forcelogin).ConfigureAwait(false))
            {
                return;
            }

            await _Console.SaveVisitAsync().ConfigureAwait(false);

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
            if (_Console.Request.Path.Equals(WimServerConfiguration.Instance.Portal_Path, StringComparison.InvariantCulture) && HasReservedRootQueryString() == false)
            {
                await HandleRootRequestAsync().ConfigureAwait(false);
            }

            //  Check the role base security
            if (await CheckSecurityAsync(reStartWithNotificationList).ConfigureAwait(false))
            {
                //  Create new instances
                DataGrid grid = new DataGrid();
                var component = new Beta.GeneratedCms.Source.Component();
                _Console.Component = component;

                await HandleRequestAsync(grid, component, isDeleteTriggered).ConfigureAwait(false);
            }
            else
            {
                // TODO: MR:10-12-2021 Add logging
                await _Console.Response.WriteAsync("no-access").ConfigureAwait(false);
                await _Console.Response.CompleteAsync();
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
            
 
            if (_Console.ItemType == RequestItemType.Item || _Console.ItemType == RequestItemType.Asset || _Console.CurrentListInstance.wim.CanContainSingleInstancePerDefinedList)
            {
                //  Handles the list item request.
                await HandleListItemRequestAsync(grid, component, isDeleteTriggered).ConfigureAwait(false);
            }
            else if (_Console.ItemType == RequestItemType.Page)
            {
                _Console.AddTrace("Monitor", "HandlePageItemRequest(...)");
                //  Handles the page request.
                await HandlePageItemRequestAsync(grid, component, isDeleteTriggered).ConfigureAwait(false);
            }
            else
            {
                //  Handles the browsing request.
                _Console.CurrentListInstance.wim.IsSearchListMode = true;
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
                IMenu roleMenu = roleMenus.FirstOrDefault(x => x.GroupID == null && x.IsActive == true && x.RoleID == _Console.CurrentApplicationUser.RoleID);

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
                                    await _Console.ApplyListAsync(implement);
                                }
                                break;
                            case 2:  // Folder
                            case 8:  // Container / Folder
                                {
                                    var folder = await Folder.SelectOneAsync(homepage.ItemID);

                                    _Console.CurrentListInstance.wim.IsSearchListMode = true;
                                    _Console.CurrentListInstance.wim.m_CurrentFolder = folder;
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

                                        _Console.CurrentListInstance.wim.IsSearchListMode = true;
                                        _Console.CurrentListInstance.wim.m_CurrentFolder = folderEntity;
                                    }
                                }
                                break;
                            case 3:  // Page
                                {
                                    _Console.Item = homepage.ItemID;
                                    _Console.ItemType = RequestItemType.Page;
                                }
                                break;
                            case 6: // Site (homepage)
                                {
                                    var hpId = (await Site.SelectOneAsync(homepage.ItemID).ConfigureAwait(false)).HomepageID;
                                    if (hpId.GetValueOrDefault(0) > 0)
                                    {
                                        _Console.Item = hpId;
                                        _Console.ItemType = RequestItemType.Page;
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
                                                folderEntity = Folder.SelectOneBySite(_Console.ChannelIndentifier, FolderType.Page);
                                            }
                                            break;
                                        case 2:
                                            {
                                                folderEntity = Folder.SelectOneBySite(_Console.ChannelIndentifier, FolderType.List);
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
                                                folderEntity = Folder.SelectOneBySite(_Console.ChannelIndentifier, FolderType.Administration);
                                            }
                                            break;
                                    }
                                    _Console.CurrentListInstance.wim.IsSearchListMode = true;
                                    _Console.CurrentListInstance.wim.m_CurrentFolder = folderEntity;
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
            _Console.CurrentListInstance.wim.IsEditMode = (await _Console.CurrentApplicationUser.SelectRoleAsync().ConfigureAwait(false)).CanChangePage;

            Page page = null;
            if (_Console.CurrentPage == null)
            {
                page = await Page.SelectOneAsync(_Console.Item.Value, false);
                _Context.Items.Add("Wim.Page", page);
                _Context.Items.Add("Wim.Site", page.Site);
            }

            _Console.View = 0;

            bool isPagePublishTriggered = _Console.IsPostBack("pagepublish");
            bool isPageOfflineTriggered = _Console.IsPostBack("pageoffline");

            bool isPageLocalised = _Console.IsPostBack("page.localize");
            bool isPageInherited = _Console.IsPostBack("page.inherit");

            int selectedTab = Utility.ConvertToInt(_Console.Request.Query["tab"]);
            string section = _Console.Request.Query["tab"];

            string pBack = string.Empty;
            if (_Console.PostBackStartsWith("pagemod_", out pBack))
            {
                pBack = pBack.Replace("pagemod_", "");
                ICollection<IPageModule> pageModules = default(List<IPageModule>);

                if (_Console.Context?.RequestServices?.GetServices<IPageModule>().Any() == true)
                {
                    pageModules = _Console.Context.RequestServices.GetServices<IPageModule>().ToList();
                }

                foreach (var pmodule in pageModules)
                {
                    if (pmodule.GetType().Name == pBack)
                    {
                        var moduleResult = await pmodule.ExecuteAsync(_Console.CurrentPage, _Console.CurrentApplicationUser, _Context);
                        if (moduleResult.IsSuccess && string.IsNullOrWhiteSpace(moduleResult.WimNotificationOutput) == false)
                        {
                            _Console.CurrentListInstance.wim.Notification.AddNotification(moduleResult.WimNotificationOutput);
                        }
                        else if (string.IsNullOrWhiteSpace(moduleResult.WimNotificationOutput) == false)
                        {
                            _Console.CurrentListInstance.wim.Notification.AddError(moduleResult.WimNotificationOutput);
                        }
                    }
                }
            }

            if (string.IsNullOrEmpty(section))
            {
                //  26-08-14:MM
                var sections = _Console.CurrentPage.Template.GetPageSections();
                if (sections.Length > 0)
                    section = sections[0];
            }

            string redirect = string.IsNullOrEmpty(section) ? "" : string.Concat("&tab=", section);

            if (_Console.IsPostBack("page.translate"))
            {
                _Console.CurrentApplicationUser.ShowTranslationView = true;
                await _Console.CurrentApplicationUser.SaveAsync().ConfigureAwait(false);

                if (!_Console.CurrentListInstance.IsEditMode)
                    _Console.Response.Redirect(string.Concat(_Console.WimPagePath, "?page=", _Console.Item.Value, redirect));
            }

            if (_Console.IsPostBack("page.copy"))
            {
                await Framework.Inheritance.Page.CopyFromMasterAsync(_Console.Item.Value).ConfigureAwait(false);
                
                // Flush all cache
                await EnvironmentVersion.SetUpdatedAsync().ConfigureAwait(false);

                _Console.Response.Redirect(string.Concat(_Console.WimPagePath, "?page=", _Console.Item.Value, redirect));
            }

            if (_Console.IsPostBack("page.normal"))
            {
                _Console.CurrentApplicationUser.ShowTranslationView = false;
                await _Console.CurrentApplicationUser.SaveAsync().ConfigureAwait(false);

                if (!_Console.CurrentListInstance.IsEditMode)
                    _Console.Response.Redirect(string.Concat(_Console.WimPagePath, "?page=", _Console.Item.Value, redirect));
            }

            if (isPagePublishTriggered)
            {
                var pagePublicationHandler = new PagePublication();

                await page.PublishAsync(pagePublicationHandler, _Console.CurrentApplicationUser);
                // Flush all cache
                await EnvironmentVersion.SetUpdatedAsync().ConfigureAwait(false);

                //Wim.Framework2.Functions.AuditTrail.Insert(_Console.CurrentApplicationUser, page, Framework2.Functions.Auditing.ActionType.Publish, null);
                _Console.Response.Redirect(string.Concat(_Console.WimPagePath, "?page=", _Console.Item.Value, redirect));
            }
            else if (isPageOfflineTriggered)
            {
                var pagePublicationHandler = new PagePublication();
                await page.TakeDownPageAsync().ConfigureAwait(false);
                // Flush all cache
                await EnvironmentVersion.SetUpdatedAsync().ConfigureAwait(false);

                //Wim.Framework2.Functions.AuditTrail.Insert(_Console.CurrentApplicationUser, page, Framework2.Functions.Auditing.ActionType.TakeOffline, null);
                _Console.Response.Redirect(string.Concat(_Console.WimPagePath, "?page=", _Console.Item.Value, redirect));
            }
            else if (isDeleteTriggered)
            {
                //  Save the version
                var currentversion = await ComponentVersion.SelectAllOnPageAsync(page.ID);
                component.SavePageVersion(page, currentversion, _Console.CurrentApplicationUser, true);

                await page.DeleteAsync().ConfigureAwait(false);
                //Wim.Framework2.Functions.AuditTrail.Insert(_Console.CurrentApplicationUser, page, Framework2.Functions.Auditing.ActionType.Remove, null);
                _Console.Response.Redirect(string.Concat(_Console.WimPagePath, "?folder=", _Console.CurrentListInstance.wim.CurrentFolder.ID));
            }
            else if (isPageLocalised)
            {
                page.IsLocalized = true;
                page.Updated = Common.DatabaseDateTime;
                await page.SaveAsync().ConfigureAwait(false);

                //Wim.Framework2.Functions.AuditTrail.Insert(_Console.CurrentApplicationUser, page, Framework2.Functions.Auditing.ActionType.Localised, null);
                _Console.Response.Redirect(string.Concat(_Console.WimPagePath, "?page=", _Console.Item.Value, redirect));

            }
            else if (isPageInherited)
            {
                page.IsLocalized = false;
                page.Updated = Common.DatabaseDateTime;
                await page.SaveAsync().ConfigureAwait(false);

                //Wim.Framework2.Functions.AuditTrail.Insert(_Console.CurrentApplicationUser, page, Framework2.Functions.Auditing.ActionType.Inherited, null);
                _Console.Response.Redirect(string.Concat(_Console.WimPagePath, "?page=", _Console.Item.Value, redirect));
            }

            Page pageInstance;

            GlobalWimControlBuilder = component.CreateContentList(_Console, 0, selectedTab == 1, out pageInstance, section);


            // Check if we have to create / update any inherited Page 
            if (_Console.IsPostBackSave && pageInstance?.ID > 0)
            {
                var hasInheritedPages = await pageInstance.HasInheritedPagesAsync().ConfigureAwait(false);
                if (_Console.Item.Value == 0 || hasInheritedPages == false)
                {
                    await Framework.Inheritance.Page.CreatePageAsync(pageInstance, page.Site).ConfigureAwait(false);
                }
                else
                {
                    await Framework.Inheritance.Page.MovePageAsync(pageInstance, page.Site).ConfigureAwait(false);
                }
            }

            if (!_Console.IsAdminFooter)
            {
                GlobalWimControlBuilder.Canvas.Type = CanvasType.ListItem;

                GlobalWimControlBuilder.Leftnav = await _PresentationNavigation.NewLeftNavigationAsync(_Console, component.m_ButtonList != null ? component.m_ButtonList.ToArray() : null).ConfigureAwait(false);

                GlobalWimControlBuilder.TopNavigation = _PresentationNavigation.TopNavigation(_Console);
                GlobalWimControlBuilder.Rightnav = _PresentationNavigation.RightSideNavigation(_Console, component.m_ButtonList != null ? component.m_ButtonList.ToArray() : null);
                GlobalWimControlBuilder.Bottom = _PresentationNavigation.NewBottomNavigation(_Console, component.m_ButtonList != null ? component.m_ButtonList.ToArray() : null, false);

                await AddToResponseAsync(await _PresentationMonitor.GetTemplateWrapperAsync(_Console, _Placeholders, _Callbacks, GlobalWimControlBuilder)).ConfigureAwait(false);
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

            if (_Console.CurrentList.Type == ComponentListType.ListSettings)
                _Console.View = (int)ContainerView.ListSettingRequest;

            _Console.AddTrace("Monitor", "GetExportOptionUrl(..)");
            if (await GetExportOptionUrlAsync(grid, component).ConfigureAwait(false))
                return;

            await HandleListModuleActionAsync(component, true);

            //  Create the form
            _Console.CurrentListInstance.wim.HideTopSectionTag = true;

            if (!_Console.IsComponent)
            {
                if (_Console.CurrentList.Option_FormAsync && !IsFormatRequest_JSON)
                {
                    _Console.CurrentListInstance.wim.DoListInit();
                    GlobalWimControlBuilder = new WimControlBuilder();
                }
                else
                {
                    _Console.CurrentListInstance.wim.DoListInit();
                    GlobalWimControlBuilder = await component.CreateListAsync(_Console, _Console.OpenInFrame, IsFormatRequest_JSON);
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
                    _Console.Response.ContentType = "application/json";

                    _PresentationNavigation.RightSideNavigation(_Console, component.m_ButtonList != null ? component.m_ButtonList.ToArray() : null, GlobalWimControlBuilder);
                    _PresentationNavigation.NewBottomNavigation(_Console, component.m_ButtonList != null ? component.m_ButtonList.ToArray() : null, false, GlobalWimControlBuilder);

                    GlobalWimControlBuilder.ApiResponse.CurrentSiteID = _Console.ChannelIndentifier;
                    GlobalWimControlBuilder.ApiResponse.ListDescription = _Console.CurrentListInstance.wim.CurrentList.Description;
                    GlobalWimControlBuilder.ApiResponse.RedirectUrl = _Console.RedirectionUrl;
                    if (!string.IsNullOrWhiteSpace(_Console.RedirectionUrl))
                    {
                        _Console.Response.StatusCode = 302;
                    }

                    GlobalWimControlBuilder.ApiResponse.IsEditMode = _Console.CurrentListInstance.IsEditMode;
                    GlobalWimControlBuilder.ApiResponse.ListTitle = _Console.CurrentListInstance.wim.ListTitle;
                    // if this item is a button add it to the button list

                    // [MR:25-05-2021] added for : https://supershift.atlassian.net/browse/FTD-147
                    await GlobalWimControlBuilder.ApiResponse.ApplySharedFieldDataAsync();

                    await AddToResponseAsync(JsonConvert.SerializeObject(GlobalWimControlBuilder.ApiResponse)).ConfigureAwait(false);
                    return;
                }

                else if (IsFormatRequest_AJAX)
                {
                    _Console.Response.ContentType = "text/plain";
                    var searchListGrid = GlobalWimControlBuilder.SearchGrid;
                    await AddToResponseAsync(searchListGrid).ConfigureAwait(false);
                    return;
                }
                else
                    //  Needed to NULLafy it as it was required for AJAX call
                    GlobalWimControlBuilder.SearchGrid = null;
            }

            // <------ was here
 
            bool isCopyTriggered = _Console.Form("copyparent") == "1";

            if (isCopyTriggered)
            {
                //
                //_Console.CurrentListInstance.DoListDelete(_Console.Item.GetValueOrDefault(0), 0);
                int childID = _Console.CurrentListInstance.wim.CurrentSite.ID;
                int parentID = _Console.CurrentListInstance.wim.CurrentSite.MasterID.GetValueOrDefault();

                _Console.CurrentListInstance.wim.CurrentSite = Site.SelectOne(parentID);
                _Console.CurrentListInstance.wim.IsCurrentList = true;
                _Console.CurrentListInstance.wim.DoListLoad(_Console.Item.GetValueOrDefault(0), 0);


                await AddToResponseAsync(string.Format("{0}<br/>", _Console.CurrentListInstance.wim.CurrentSite.ID)).ConfigureAwait(false);
                await AddToResponseAsync(string.Format("{0}<br/>", _Console.CurrentListInstance.wim.CurrentSite.MasterID.GetValueOrDefault())).ConfigureAwait(false);
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

                if (_Console.OpenInFrame == 0)
                {
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
                        _Console.Response.Redirect(_Console.CurrentListInstance.wim.GetUrl(new KeyValue[] { new KeyValue("item", true) }));
                }
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

                GlobalWimControlBuilder = component.CreateContentList(_Console, 0, true, out pageInstance, null);
                GlobalWimControlBuilder.Canvas.Type = _Console.OpenInFrame > 0 ? CanvasType.ListItemInLayer : CanvasType.ListItem;
                GlobalWimControlBuilder.TopNavigation = _PresentationNavigation.TopNavigation(_Console);
                GlobalWimControlBuilder.Bottom = _PresentationNavigation.NewBottomNavigation(_Console, component.m_ButtonList != null ? component.m_ButtonList.ToArray() : null, false);
                GlobalWimControlBuilder.Tabularnav = Template.GetTabularTagNewDesign(_Console, _Console.CurrentList.Name, 0, false);
                GlobalWimControlBuilder.Leftnav = await _PresentationNavigation.NewLeftNavigationAsync(_Console, component.m_ButtonList != null ? component.m_ButtonList.ToArray() : null).ConfigureAwait(false);

                await AddToResponseAsync(await _PresentationMonitor.GetTemplateWrapperAsync(_Console, _Placeholders, _Callbacks, GlobalWimControlBuilder));

                return;
            }

            if (!_Console.IsAdminFooter)
            {
             
                GlobalWimControlBuilder.TopNavigation = _PresentationNavigation.TopNavigation(_Console);
                GlobalWimControlBuilder.Canvas.Type = _Console.OpenInFrame > 0 ? CanvasType.ListItemInLayer : CanvasType.ListItem;
                GlobalWimControlBuilder.Rightnav = _PresentationNavigation.RightSideNavigation(_Console, component.m_ButtonList != null ? component.m_ButtonList.ToArray() : null);

                if (
                    GlobalWimControlBuilder.Canvas.Type == CanvasType.ListInLayer ||
                    GlobalWimControlBuilder.Canvas.Type == CanvasType.ListItemInLayer
                    )
                {
                    //  Do nothing, this is an layer and has no leftnavigation.
                }
                else
                    GlobalWimControlBuilder.Leftnav = await _PresentationNavigation.NewLeftNavigationAsync(_Console, component.m_ButtonList != null ? component.m_ButtonList.ToArray() : null).ConfigureAwait(false);
                GlobalWimControlBuilder.Bottom = _PresentationNavigation.NewBottomNavigation(_Console, component.m_ButtonList != null ? component.m_ButtonList.ToArray() : null, false);
                GlobalWimControlBuilder.Tabularnav = Template.GetTabularTagNewDesign(_Console, _Console.CurrentList.Name, 0, false);

                await AddToResponseAsync(await _PresentationMonitor.GetTemplateWrapperAsync(_Console, _Placeholders, _Callbacks, GlobalWimControlBuilder)).ConfigureAwait(false);
            }
        }

        async Task<bool> GetExportOptionUrlAsync(DataGrid grid, Beta.GeneratedCms.Source.Component component)
        {
            string exportUrl = null;
            //  Export to XLS: XLS Creation URL
            if (_Console.IsPostBack("export_xls") || _Console.Request.Query["xls"] == "1")
            {
                exportUrl = _Console.WimPagePath;

                _Console.CurrentListInstance.wim.IsExportMode_XLS = true;

                await component.CreateSearchListAsync(_Console, 0);
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
            }
            return false;
        }

        async Task<ModuleExecutionResult> HandleListModuleActionAsync(Beta.GeneratedCms.Source.Component component, bool isItemRequest = false)
        {
            var result = new ModuleExecutionResult() { IsSuccess = false };
            if (_Console.PostBackStartsWith("listmod_", out string pBack))
            {
                pBack = pBack.Replace("listmod_", "", StringComparison.InvariantCultureIgnoreCase);

                ICollection<IListModule> listModules = default(List<IListModule>);

                if (_Console.Context?.RequestServices?.GetServices<IListModule>().Any() == true)
                {
                    listModules = _Console.Context.RequestServices.GetServices<IListModule>().ToList();
                }

                foreach (var pmodule in listModules)
                {
                    if (pmodule.GetType().Name == pBack)
                    {
                        result = await pmodule.ExecuteAsync(_Console.CurrentListInstance, _Console.CurrentApplicationUser, _Context);
                    }
                }

                if (string.IsNullOrWhiteSpace(result.WimNotificationOutput) == false)
                {
                    _Console.CurrentListInstance.wim.Notification.AddNotification(result.WimNotificationOutput, result.IsSuccess ? "Success" : "Error", true);
                }
            }

            return result;
        }

        bool IsFormatRequest_AJAX
        {
            get
            {
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

            //if (!IsFormatRequest)
            //{
            _Console.AddTrace("Monitor", "GetExportOptionUrl(..)");
            if (await GetExportOptionUrlAsync(grid, component))
                return;


            // Execute module
            var executeModule = await HandleListModuleActionAsync(component, false);
            if (executeModule.IsSuccess)
            {
                if (string.IsNullOrWhiteSpace(executeModule.RedirectUrl) == false)
                { 
                    _Console.Redirect(executeModule.RedirectUrl, true);
                }
            }

            _Console.AddTrace("Monitor", "CreateSearchList(..)");

            GlobalWimControlBuilder = await component.CreateSearchListAsync(_Console, 0);
            GlobalWimControlBuilder.Canvas.Type = _Console.OpenInFrame > 0 ? CanvasType.ListInLayer : CanvasType.List;

            if (_Console.OpenInFrame > 0)
                _Console.CurrentListInstance.wim.Page.HideTabs = true;
            //}

            StringBuilder searchListGrid = new StringBuilder();
          
            _Console.AddTrace("Monitor", "GetGridFromListInstance(..)");

            if (IsFormatRequest_JSON)
            {
                _Console.Response.ContentType = "application/json";
                searchListGrid = grid.GetGridFromListInstanceForJSON(_Console.CurrentListInstance.wim, _Console, 0, false, true);

                await AddToResponseAsync(searchListGrid.ToString()).ConfigureAwait(false);
                return;
            }
            if (IsFormatRequest_AJAX)
            {
                _Console.Response.ContentType = "text/plain";
                searchListGrid = new StringBuilder();
                while (_Console.CurrentListInstance.wim.NextGrid())
                {
                    bool hasNoTitle = string.IsNullOrEmpty(_Console.CurrentListInstance.wim.m_DataTitle);
                    var titleText = hasNoTitle ? null : $"</section><section class=\"{_Console.CurrentListInstance.wim.Page.Body.Grid.ClassName}\"><h2>{_Console.CurrentListInstance.wim.m_DataTitle}</h2>";
                    var gridText = grid.GetGridFromListInstance(_Console.CurrentListInstance.wim, _Console, 0, false, _Console.CurrentListInstance);

                    searchListGrid.Append(titleText);
                    searchListGrid.Append(gridText);
                }

                await AddToResponseAsync(searchListGrid.ToString()).ConfigureAwait(false);
                return;
            }
            if (_Console.CurrentListInstance.wim.CurrentList.Option_SearchAsync)
            {
                if (_Console.OpenInFrame > 0)
                {
                    searchListGrid.Append($"<section id=\"datagrid\" class=\"{_Console.CurrentListInstance.wim.Page.Body.Grid.ClassName} async\"> </section>");
                }
                else
                {
                    searchListGrid.Append(' ');
                }
            }
            else
            {
                searchListGrid = new StringBuilder();
                while (_Console.CurrentListInstance.wim.NextGrid())
                {
                    bool hasNoTitle = string.IsNullOrEmpty(_Console.CurrentListInstance.wim.m_DataTitle);
                    var titleText = hasNoTitle ? null : $"</section><section class=\"{_Console.CurrentListInstance.wim.Page.Body.Grid.ClassName}\"><h2>{_Console.CurrentListInstance.wim.m_DataTitle}</h2>";
                    var gridText = grid.GetGridFromListInstance(_Console.CurrentListInstance.wim, _Console, 0, false, _Console.CurrentListInstance);

                    searchListGrid.Append(titleText);
                    searchListGrid.Append(gridText);
                }
            }

            //  Replacement event of ListSearchedAction
            if (!string.IsNullOrEmpty(component.m_ClickedButton) && _Console.CurrentListInstance.wim.HasListAction)
            {
                _Console.CurrentListInstance.wim.DoListAction(_Console.Item.GetValueOrDefault(0), 0, component.m_ClickedButton, null);
            }

            // <------ was here

            _Console.AddTrace("Monitor", "AddToResponse(..)");

            GlobalWimControlBuilder.SearchGrid = searchListGrid.ToString();
            GlobalWimControlBuilder.TopNavigation = _PresentationNavigation.TopNavigation(_Console);
            GlobalWimControlBuilder.Bottom = _PresentationNavigation.NewBottomNavigation(
                _Console,
                component.m_ButtonList != null
                    ? component.m_ButtonList.ToArray()
                    : null,
                !GlobalWimControlBuilder.IsNull
            );

            GlobalWimControlBuilder.Tabularnav = Template.GetTabularTagNewDesign(_Console, _Console.CurrentList.Name, 0, false);
            GlobalWimControlBuilder.Rightnav = _PresentationNavigation.RightSideNavigation(_Console, component.m_ButtonList != null ? component.m_ButtonList.ToArray() : null);
            GlobalWimControlBuilder.Leftnav = await _PresentationNavigation.NewLeftNavigationAsync(_Console, component.m_ButtonList != null ? component.m_ButtonList.ToArray() : null).ConfigureAwait(false);

            await AddToResponseAsync(await _PresentationMonitor.GetTemplateWrapperAsync(_Console, _Placeholders, _Callbacks, GlobalWimControlBuilder)).ConfigureAwait(false);
        }


        async Task<bool> HandleAsyncRequestAsync(Beta.GeneratedCms.Source.Component component)
        {
            if (_Console.CurrentListInstance == null) return false;
            var asyncResult = Utils.GetAsyncQuery(_Console);
            if (asyncResult == null)
                return false;

            if (_Console.CurrentListInstance.wim.HasListAsync)
            {
                _Console.HasAsyncEvent = true;
                ComponentAsyncEventArgs eventArgs = new ComponentAsyncEventArgs(_Console.Item.GetValueOrDefault());

                eventArgs.Query = asyncResult.SearchQuery;
                eventArgs.SearchType = asyncResult.SearchType;
                eventArgs.Property = asyncResult.Property;

                eventArgs.Data = new ASyncResult();
                eventArgs.Data.Property = asyncResult.Property;
                eventArgs.ApplyData(component, _Console);
                eventArgs.SelectedGroupItemKey = _Console.GroupItem.GetValueOrDefault();
                eventArgs.SelectedGroupKey = _Console.Group.GetValueOrDefault();

                var result = _Console.CurrentListInstance.wim.DoListAsync(eventArgs);

                var options = new JsonSerializerOptions { 
                    IgnoreNullValues = true,
                    PropertyNameCaseInsensitive = true,
                    IgnoreReadOnlyProperties = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                string val = System.Text.Json.JsonSerializer.Serialize(result.Data, options);
                _Console.Response.ContentType = "application/json";
                await AddToResponseAsync(val).ConfigureAwait(false);

                return true;
            }
            return false;
        }

        async Task<bool> HandleSortingRequestAsync()
        {
            // Is this a sorting Request ?
            if (_Console.Request?.Query?.ContainsKey("sortF") == true && _Console.Request?.Query?.ContainsKey("sortT") == true)
            {
                int list = Utility.ConvertToInt(_Console.Request.Query["list"]);
                int sortF = Utility.ConvertToInt(_Console.Request.Query["sortF"]);
                int sortT = Utility.ConvertToInt(_Console.Request.Query["sortT"]);

                if (list > 0 && sortF > 0 && sortT > 0)
                {
                    IComponentList implement = ComponentList.SelectOne(list);
                    IComponentListTemplate currentListInstance = implement.GetInstance(_Console);

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
            if (_Console.CurrentListInstance == null || string.IsNullOrEmpty(_Console.CurrentListInstance.wim.PostbackValue))
            {
                return;
            }

            // 
            switch (_Console.CurrentListInstance.wim.PostbackValue)
            {
                case "PageContentPublication":
                    {
                        _Console.CurrentListInstance.wim.Notification.AddNotification("The webcontent has been refreshed.");
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
            if (_Console.Request.PathBase.HasValue)
            {
                currentUrl = _Console.Request.PathBase.ToString();
            }
            currentUrl += _Console.Request.Path;

            //  Find the default homepage in the menu section
            var defaultHome = await _Console.UrlBuild.GetHomeRequestAsync(siteID).ConfigureAwait(false);
            if (currentUrl.Equals(defaultHome, StringComparison.InvariantCultureIgnoreCase) == false)
            {
                _Console.Response.Redirect(defaultHome);
            }
        }

        async Task<bool> CheckSecurityAsync(bool reStartWithNotificationList)
        {
            var currentUrl = "";
            if (_Console.Request.PathBase.HasValue)
            {
                currentUrl = _Console.Request.PathBase.ToString();
            }
            currentUrl += _Console.Request.Path;

            var sites = await _Console.CurrentListInstance.wim.CurrentApplicationUserRole.SitesAsync(_Console.CurrentApplicationUser).ConfigureAwait(false);

            //  ACL: Sites
            if (!_Console.CurrentListInstance.wim.CurrentApplicationUserRole.All_Sites
                && (sites).Length == 0
                && !reStartWithNotificationList)
            {
                throw new Exception(ErrorCode.GetMessage(1002, _Console.CurrentApplicationUser.LanguageCulture));
            }

            //  ACL: Sites
            if (!_Console.CurrentListInstance.wim.CurrentApplicationUserRole.All_Sites)
            {
                if (!await _Console.CurrentListInstance.wim.CurrentSite.HasRoleAccessAsync(_Console.CurrentListInstance.wim.CurrentApplicationUser).ConfigureAwait(false))
                {
                    var allowed = await _Console.CurrentListInstance.wim.CurrentApplicationUserRole.SitesAsync(_Console.CurrentApplicationUser).ConfigureAwait(false);
                    if (allowed != null && allowed.Length > 0)
                    {
                        await RedirectToChannelHomePageAsync(allowed[0].ID).ConfigureAwait(false);
                        return false;
                    }
                    else
                    {
                        if ((await _Console.CurrentListInstance.wim.CurrentApplicationUser.SitesAsync(AccessFilter.RoleAndUser).ConfigureAwait(false))?.Length > 0)
                        {
                            var siteId = (await _Console.CurrentListInstance.wim.CurrentApplicationUser.SitesAsync(AccessFilter.RoleAndUser).ConfigureAwait(false))[0].ID;
                            var newUrl = _Console.GetWimPagePath(siteId);
                            if (currentUrl.Equals(newUrl, StringComparison.InvariantCultureIgnoreCase) == false)
                            {
                                _Console.Response.Redirect(newUrl);
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
                _Console.CurrentListInstance.wim.CurrentFolder.ID == 0 && 
                string.IsNullOrEmpty(_Console.Request.Query["dashboard"]))
            {
                await RedirectToChannelHomePageAsync(_Console.ChannelIndentifier).ConfigureAwait(false);
                return false;
            }

            //  ACL: Folders
            if (_Console.CurrentListInstance.wim.CurrentFolder.Type != FolderType.Gallery
                && !_Console.CurrentListInstance.wim.CurrentApplicationUserRole.All_Folders)
            {
                if (!await _Console.CurrentListInstance.wim.CurrentFolder.HasRoleAccessAsync(_Console.CurrentListInstance.wim.CurrentApplicationUser).ConfigureAwait(false))
                {
                    if (_Console.CurrentListInstance.wim.CurrentFolder.ParentID.HasValue)
                    {
                        _Console.Response.Redirect(_Console.UrlBuild.GetFolderRequest(_Console.CurrentListInstance.wim.CurrentFolder.ParentID.Value));
                    }
                    var newUrl = _Console.WimPagePath;
                    if (currentUrl.Equals(newUrl, StringComparison.InvariantCultureIgnoreCase) == false)
                    {
                        _Console.Response.Redirect(newUrl);
                    }

                    return false;
                }
            }

            //  Check environment
            bool approved = false;
            switch (_Console.CurrentListInstance.wim.CurrentFolder.Type)
            {
                default:
                case FolderType.Undefined:
                    {
                        approved = true; break;
                    }
                case FolderType.Page:
                    {
                        approved = _Console.CurrentListInstance.wim.CurrentApplicationUserRole.CanSeePage;
                    }
                    break;
                case FolderType.List:
                    {
                        if (_Console.CurrentListInstance.wim.CurrentList.Type == ComponentListType.Browsing)
                        {
                            approved = _Console.CurrentListInstance.wim.CurrentApplicationUserRole.CanSeeFolder;
                            if (!approved)
                            {
                                var newUrl = _Console.WimPagePath;
                                
                                if (currentUrl.Equals(newUrl, StringComparison.InvariantCultureIgnoreCase) == false)
                                {
                                    _Console.Response.Redirect(newUrl);
                                }

                                return false;
                            }
                        }
                        approved = _Console.CurrentListInstance.wim.CurrentApplicationUserRole.CanSeeList;
                        if (_Console.CurrentList.Type == ComponentListType.Undefined && !_Console.CurrentListInstance.wim.CurrentApplicationUserRole.All_Lists)
                        {
                            if (!_Console.CurrentListInstance.wim.CurrentList.HasRoleAccess(_Console.CurrentListInstance.wim.CurrentApplicationUser))
                            {
                                var newUrl = _Console.WimPagePath;
                                if (currentUrl.Equals(newUrl, StringComparison.InvariantCultureIgnoreCase) == false)
                                {
                                    _Console.Response.Redirect(newUrl);
                                }

                                return false;
                            }
                        }
                    }
                    break;
                case FolderType.Gallery:
                    {
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

                                var newUrl = _Console.WimPagePath;
                                if (currentUrl.Equals(newUrl, StringComparison.InvariantCultureIgnoreCase) == false)
                                {
                                    _Console.Response.Redirect(newUrl);
                                }

                                return false;
                            }
                        }
                    }
                    break;
                case FolderType.Administration:
                    {
                        approved = _Console.CurrentListInstance.wim.CurrentApplicationUserRole.CanSeeAdmin;
                    }
                    break;
            }

            if (!approved)
            {
                if (_Console.CurrentListInstance.wim.CurrentList.Type != ComponentListType.Images
                    && _Console.CurrentListInstance.wim.CurrentList.Type != ComponentListType.Documents
                    && _Console.CurrentListInstance.wim.CurrentList.Type != ComponentListType.Links)
                {
                    var newUrl = _Console.WimPagePath;
                    if (currentUrl.Equals(newUrl, StringComparison.InvariantCultureIgnoreCase) == false)
                    {
                        _Console.Response.Redirect(newUrl);
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
        async Task CheckSiteAsync()
        {
            if (_Console.CurrentList.SiteID.HasValue && _Console.ChannelIndentifier != _Console.CurrentList.SiteID.Value)
            {
                if (_Console.CurrentList.IsInherited)
                {
                    return;
                }

                var site = await Site.SelectOneAsync(_Console.CurrentList.SiteID.Value).ConfigureAwait(false);
                if (site.Type.HasValue)
                {
                    return;
                }

                _Console.Response.Redirect(_Console.CurrentListInstance.wim.GetUrl());
            }
        }

        /// <summary>
        /// Sets the form modes (Edit/Save,Delete, etc)
        /// </summary>
        /// <param name="isDeleteMode">if set to <c>true</c> [is delete mode].</param>
        async Task<bool> SetFormModesAsync()
        {
            await _Console.LoadJsonStreamAsync().ConfigureAwait(false);

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
        async Task SetRequestTypeAsync()
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
                if (Utility.IsNumeric(candidate))
                {
                    _Console.ChannelIndentifier = Utility.ConvertToInt(candidate, Data.Environment.Current.DefaultSiteID.GetValueOrDefault());
                }
                else
                {
                    var allSites = await Site.SelectAllAsync().ConfigureAwait(false);
                    var selection = allSites.FirstOrDefault(x => x.Name.Equals(candidate, StringComparison.CurrentCultureIgnoreCase));
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
                        var allSites = await Site.SelectAllAsync().ConfigureAwait(false);
                        _Console.ChannelIndentifier = allSites[0].ID;
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
                    _Console.CurrentFolder = await Folder.SelectOneAsync(Utils.FromUrl(completepath), _Console.ChannelIndentifier).ConfigureAwait(false);

                    if (_Console.CurrentFolder?.ID > 0)
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
                        var allSites = await Site.SelectAllAsync().ConfigureAwait(false);
                        _Console.ChannelIndentifier = allSites[0].ID;
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
                await _Console.ApplyListAsync(ComponentListType.Browsing).ConfigureAwait(false);
                _Console.ItemType = RequestItemType.Page;
                return;
            }

            //  Verify asset request
            _Console.Item = Utility.ConvertToIntNullable(_Console.Request.Query["asset"], false);
            if (_Console.Item.HasValue)
            {
                await _Console.ApplyListAsync(ComponentListType.Documents).ConfigureAwait(false);
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
            if (_Console.Item.HasValue)
            {
                _Console.ItemType = RequestItemType.Item;
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


                    ((ContentSharedAttribute)element).ID = name;
                    ((ContentSharedAttribute)element).OverrideTableGeneration = true;
                    ((ContentSharedAttribute)element).Expression = (_Console.Request.Query["w"] == "2" ? OutputExpression.FullWidth : OutputExpression.Alternating);
                    ((ContentSharedAttribute)element).Console = _Console;
                    ((ContentSharedAttribute)element).IsBluePrint = true;
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

                    await AddToResponseAsync(build.ToString()).ConfigureAwait(false);
                }


                if (_Console.Request.Query["xml"] == "component")
                {
                    var page = Utility.ConvertToInt(_Console.Request.Query["page"]);
                    var target = _Console.Request.Query["tab"];
                    if (string.IsNullOrEmpty(target) && page > 0)
                    {
                        var pageInstance = await Page.SelectOneAsync(page).ConfigureAwait(false);
                        var sections = pageInstance.Template.GetPageSections();
                        if (sections != null)
                        {
                            target = sections.FirstOrDefault();
                        }
                    }

                    var content = await Beta.GeneratedCms.Source.Xml.Component.GetAsync(_Console, Utility.ConvertToInt(_Console.Request.Query["id"]), page, Utility.ConvertToInt(_Console.Request.Query["cmpt"]), target).ConfigureAwait(false);
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
                    return await _Console.ApplyListAsync(list).ConfigureAwait(false);
                }
            }

            //  If the list is not know, take the default list in stead (browsing)
            if (!string.IsNullOrEmpty(_Console.Request.Query["list"]))
            {
                //  The list reference can be a INT or a GUID
                return await _Console.ApplyListAsync(_Console.Request.Query["list"]).ConfigureAwait(false);
            }
            else if (_Console.ItemType == RequestItemType.Asset)
            {
                await _Console.ApplyListAsync(ComponentListType.Documents).ConfigureAwait(false);
            }
            else
            {
                await _Console.ApplyListAsync(typeof(AppCentre.Data.Implementation.Browsing)).ConfigureAwait(false);
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
                if (_Console.IsPostBack("logout") || _Console.Request.Query.ContainsKey("logout"))
                {
                    await LogoutViaSingleSignOnAsync().ConfigureAwait(false);
                }
            }

            await AuthenticateViaSingleSignOnAsync(true).ConfigureAwait(false);

            //  Check roaming profile
            if (!showLogin && _Console.CurrentApplicationUser != null)
            {
                await ValidateHijackResetAsync().ConfigureAwait(false);
                return true;
            }
            else
            {
                string reaction = await _PresentationMonitor.GetLoginWrapperAsync(_Console, _Placeholders, _Callbacks);
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
            if (_Console.Request.QueryString.HasValue 
                && _Console.Request.QueryString.Value.Equals("?reset", StringComparison.CurrentCultureIgnoreCase))
            {
                if (_Console.CurrentVisitor != null && _Console.CurrentVisitor.Data != null)
                {
                    if (!_Console.CurrentVisitor.Data["Wim.Reset.Me"].IsNull)
                    {
                        if (Guid.TryParse(_Console.CurrentVisitor.Data["Wim.Reset.Me"].Value, out var id))
                        {
                            var user = await ApplicationUser.SelectOneAsync(id).ConfigureAwait(false);
                            if (user?.ID > 0)
                            {
                                _Console.CurrentVisitor.Data.Apply("Wim.Reset.Me", null);
                                _Console.CurrentVisitor.ApplicationUserID = user.ID;
                                _Console.CurrentApplicationUser = user;
                                _Console.SaveVisit();

                                user.LastLoggedVisit = DateTime.UtcNow;
                                await user.SaveAsync().ConfigureAwait(false);

                                await new AuditTrail()
                                {
                                    Action = ActionType.Login,
                                    Type = ItemType.Undefined,
                                    ItemID = _Console.CurrentApplicationUser.ID,
                                    Message = "Reset impersonation",
                                    Created = user.LastLoggedVisit.Value
                                }.InsertAsync().ConfigureAwait(false);
                            }

                        }
                    }
                }
            }
        }

        async Task LogoutViaSingleSignOnAsync()
        {
            _Console.CurrentVisitor.ApplicationUserID = null;
            _Console.CurrentVisitor.Jwt = null;
            await _Console.CurrentVisitor.SaveAsync().ConfigureAwait(false);

            if (_configuration.GetValue<bool>("mediakiwi:authentication"))
            {
                _Context.Response.Redirect($"{_Console.CurrentDomain}/.auth/logout?post_logout_redirect_uri={_Console.GetWimPagePath(null)}");
            }
            else
            {
                _Console.Response.Redirect(_Console.WimPagePath);
            }
        }

        internal async Task AuthenticateViaSingleSignOnAsync(bool redirectOnAnonymous, bool outputRedirectPage = false)
        {
            if (WimServerConfiguration.Instance.Authentication != null && WimServerConfiguration.Instance.Authentication.Aad != null && WimServerConfiguration.Instance.Authentication.Aad.Enabled && _Console.CurrentApplicationUser == null)
            {
                var jwt = _Console.GetSafePost("id_token");
                if (!string.IsNullOrEmpty(jwt))
                {
                    string email = await OAuth2Logic.ExtractUpnAsync(WimServerConfiguration.Instance.Authentication, jwt).ConfigureAwait(false);
                
                    if (!string.IsNullOrEmpty(email))
                    {
                        // do login
                        var applicationUser = ApplicationUser.SelectOne(email, true);
                        if (applicationUser != null && !applicationUser.IsNewInstance)
                        {
                            _Console.CurrentApplicationUser = applicationUser;
                            var now = DateTime.UtcNow;

                            await new AuditTrail()
                            {
                                Action = ActionType.Login,
                                Type = ItemType.Undefined,
                                ItemID = _Console.CurrentApplicationUser.ID,
                                Message = "Claim based (id_token)",
                                Created = now
                            }.InsertAsync().ConfigureAwait(false);

                            _Console.CurrentVisitor.Jwt = jwt;
                            _Console.CurrentApplicationUser.LastLoggedVisit = now;
                            await _Console.CurrentApplicationUser.SaveAsync().ConfigureAwait(false);

                            _Console.SaveVisit();
                            _Console.SetClientRedirect(new Uri(_Console.GetSafePost("state")), true);

                            if (outputRedirectPage)
                            {
                                var presentation = new Framework.Presentation.Presentation();

                                await _Console.ApplyListAsync(ComponentListType.Browsing).ConfigureAwait(false);

                                var output = await presentation.GetTemplateWrapperAsync(_Console, null, null, null);
                                await _Console.Response.WriteAsync(output).ConfigureAwait(false);
                            }
                            return;
                        }
                    }
                }

                if (redirectOnAnonymous)
                {
                    await _Console.LoadCurrentApplicationUserAsync();
                    if (_Console.CurrentApplicationUser != null && _Console.CurrentApplicationUser.IsActive)
                    {
                        // do nothing, user is logged in.
                    }
                    else
                    {
                        var url = _Console.Url;
                        if (url.Contains("?logout", StringComparison.CurrentCultureIgnoreCase))
                        {
                            url = _Console.GetWimPagePath(null);
                        }
                        _Context.Response.Redirect(OAuth2Logic.AuthenticationUrl(url, _Console.CurrentDomain).ToString());
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
                if (_Console.Request.Query["split"] == "homeArticle")
                {
                    var split = output.Split(new string[] { @"<article id=""homeArticle"">", "</article>" }, StringSplitOptions.RemoveEmptyEntries);
                    output = split[1];
                }
                Body = output;
                await _Console.Response.WriteAsync(output).ConfigureAwait(false);
            }
        }

        public string Body { get; set; }
    }

}
