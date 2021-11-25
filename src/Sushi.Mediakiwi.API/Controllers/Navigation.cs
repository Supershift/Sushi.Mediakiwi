using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Sushi.Mediakiwi.API.Services;
using Sushi.Mediakiwi.API.Transport.Requests;
using Sushi.Mediakiwi.API.Transport.Responses;
using System.Linq;
using System;
using Sushi.Mediakiwi.API.Transport;
using Sushi.Mediakiwi.API.Filters;

namespace Sushi.Mediakiwi.API.Controllers
{
    [ApiController]
    [MediakiwiApiAuthorize]
    [Route(Common.MK_CONTROLLERS_PREFIX + "navigation")]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public class Navigation : BaseMediakiwiApiController
    {
        private readonly INavigationService navService;

        public Navigation(INavigationService _service)
        {
            navService = _service;
        } 


        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpGet("GetTopnavigation")]
        public async Task<ActionResult<GetTopNavigationResponse>> GetTopnavigation([FromBody] GetTopNavigationRequest request)
        {
            if (request == null || navService == null || request.CurrentSiteID == 0)
            {
                return BadRequest();
            }

            GetTopNavigationResponse result = new GetTopNavigationResponse()
            {
                StatusCode = System.Net.HttpStatusCode.OK
            };

            var menus = await Data.Menu.SelectAllAsync().ConfigureAwait(false);

            // Get Appropriate role from DB
            var role = await MediakiwiUser.RoleAsync();

            // Get Menu for current site and Role
            var list = await Data.MenuItemView.SelectAllAsync(request.CurrentSiteID, role.ID).ConfigureAwait(false);

            if (list.Count > 0)
            {
                string className = "";

                var mainNav = (from item in list where item.Sort == 1 select item);
                foreach (var item in mainNav)
                {
                    bool isSelected = false;
                    if (!await navService.HasRoleAccessAsync(item, role).ConfigureAwait(false))
                    {
                        continue;
                    }

                    var subnavigation = (from subnav in list where subnav.Sort != 1 && subnav.Position == item.Position select subnav).ToList();

                    // Create nav Item
                    NavigationItem navItem = new NavigationItem()
                    {
                        Text = item.Name
                    };

                    if (item.TypeID == 8)
                    {
                        var subSubnavigation = await Data.SearchView.SelectAllAsync(item.ItemID).ConfigureAwait(false);

                        foreach (var subItem in subSubnavigation.Reverse())
                        {
                            //  If the item is the same as the callee 
                            if (subItem.ID == item.Tag || subItem.ID == string.Concat("2_", item.ItemID))
                            {
                                continue;
                            }

                            if (!await navService.HasRoleAccessAsync(subItem, role).ConfigureAwait(false))
                            {
                                continue;
                            }

                            subnavigation.Insert(0, new Data.MenuItemView() { ItemID = subItem.ItemID, TypeID = subItem.TypeID, Name = subItem.Title });
                        }
                    }
                    string firstInLineUrl = null;
                    bool linkable = true;

                    if (subnavigation.Count > 0)
                    {
                        className = "";
                        for (int index = 0; index < subnavigation.Count; index++)
                        {
                            if (index == 0)
                            {
                                if (item.TypeID == 8)
                                {
                                    firstInLineUrl = navService.GetUrl(Console, subnavigation[index], request.CurrentSiteID);
                                    linkable = false;
                                }
                                className = "first";
                            }
                            else
                            {
                                className = string.Empty;
                            }

                            //  Only for folders
                            if (subnavigation[index].TypeID == 8)
                            {
                                var addSubResult = await navService.AddSubSubNavigationAsync(Console, navItem, subnavigation[index], className, role).ConfigureAwait(false);
                                if (addSubResult.isCurrent)
                                {
                                    isSelected = true;
                                }
                            }
                            else
                            {
                                if (subnavigation[index]?.ItemID == Console?.CurrentList?.ID)
                                {
                                    isSelected = true;
                                }

                                navItem.Items.Add(new NavigationItem()
                                {
                                    IsHighlighted = isSelected,
                                    Text = subnavigation[index].Name,
                                    IconClass = className,
                                    Href = navService.GetUrl(Console, subnavigation[index], request.CurrentSiteID),
                                });
                            }
                        }
                    }

                    if (!isSelected)
                    {
                        isSelected = navService.IsRequestPartOfNavigation(item, Resolver);
                    }

                    if (linkable)
                    {
                        navItem.Href = string.IsNullOrEmpty(firstInLineUrl) ? navService.GetUrl(Console, item, request.CurrentSiteID) : firstInLineUrl;
                        navItem.IsHighlighted = isSelected;
                        navItem.Text = item.Name;
                    }
                    else
                    {
                        navItem.Href = string.IsNullOrEmpty(firstInLineUrl) ? navService.GetUrl(Console, item, request.CurrentSiteID) : firstInLineUrl;
                        navItem.IsHighlighted = isSelected;
                        navItem.Text = item.Name;
                        navItem.IconClass += "noClick";
                    }

                    result.Items.Add(navItem);
                }
            }

            // Set Logo URl
            result.LogoUrl = navService.GetLogoURL(Console);

            // Set homepage URL
            result.HomeUrl = navService.GetHomepageURL(Console);

            return Ok(result);
        }

        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpGet("GetSidenavigation")]
        public async Task<ActionResult<GetSideNavigationResponse>> GetSidenavigation([FromBody] GetSideNavigationRequest request)
        {
            GetSideNavigationResponse result = new GetSideNavigationResponse();

            // Get Appropriate role from DB
            var role = await MediakiwiUser.RoleAsync();

            // title placeholder
            string title = string.Empty;
            if (string.IsNullOrWhiteSpace(Resolver.List?.Name) == false)
            {
                title = Resolver.List.Name;
            }

     //       #region Tabs

            if (Resolver.ListInstance?.wim?.Page?.HideTabs != true)
            {
                return null;
            }

            string tabTag = null;
            
   //         bool isPageProperty = resolver.List.Type == Data.ComponentListType.PageProperties;

   //         if (resolver.List.Type == Data.ComponentListType.Browsing || isPageProperty)
   //         {
   //             title = ResourceManager.GetString("list_browsing", new CultureInfo(Console.CurrentApplicationUser.LanguageCulture));

   //             #region PAGE

   //             if (Console.ItemType == RequestItemType.Page || isPageProperty)
   //             {
   //                 Page currentPage;
   //                 if (Console.CurrentList.Type == ComponentListType.PageProperties && Console.Item.HasValue)
   //                     currentPage = Page.SelectOne(Console.Item.Value);
   //                 else
   //                     currentPage = Console.CurrentPage;

   //                 if (currentPage?.Template?.GetPageSections()?.Length > 0)
   //                 {
   //                     var sections = currentPage.Template.GetPageSections();

   //                     StringBuilder build = new StringBuilder();

   //                     var selected = Console.Request.Query["tab"];
   //                     bool? isSelected = null;

   //                     IComponentList pageSettings = ComponentList.SelectOne(new Guid("4E7BCF0F-844B-4877-AB2D-3154BE01BC0F"));

   //                     if (sections.Length > 0)
   //                     {
   //                         build.AppendFormat(string.Format(@" <li class=""active""><a href=""{0}"">{1}</a>"
   //                             , string.Concat(Console.WimPagePath, "?list=", pageSettings.ID, "&item=", Console.Item)//, "&tab=", sections[0])
   //                             , Labels.ResourceManager.GetString("page_properties", new CultureInfo(Console.CurrentApplicationUser.LanguageCulture))
   //                         ));

   //                         build.Append("<ul>");
   //                         foreach (var section in sections)
   //                         {
   //                             if (string.IsNullOrEmpty(section))
   //                                 continue;

   //                             if ((!isPageProperty && string.IsNullOrEmpty(selected) && !isSelected.HasValue) || selected == section)
   //                                 isSelected = true;
   //                             else
   //                                 isSelected = false;

   //                             var tabName = currentPage.Template.Data[string.Format("T[{0}]", section)].Value;
   //                             if (string.IsNullOrEmpty(tabName))
   //                                 tabName = section;

   //                             build.AppendFormat(string.Format(@" <li{2}><a href=""{0}"">{1}</a></li>"
   //                                 , string.Concat(Console.WimPagePath, "?page=", Console.Item, "&tab="
   //                                 , section
   //                                     )
   //                                 , tabName
   //                                 , isSelected.GetValueOrDefault() ? " class=\"active\"" : null
   //                             ));
   //                         }
   //                         build.Append("</ul>");
   //                     }
   //                     build.Append("</li>");
   //                     build.Append(@"</ul>");
   //                     tabTag = build.ToString();

   //                 }
   //                 else
   //                 {
   //                     title = Labels.ResourceManager.GetString("tab_Content", new CultureInfo(Console.CurrentApplicationUser.LanguageCulture));
   //                     //"Content"
   //                     tabTag = string.Format(@"
			//                     <li{2}><a href=""{1}"">{0}</a></li>{3}"
   //                         , title //0
   //                         , string.Concat(Console.WimPagePath, "?page=", Console.Item) //1
   //                         , selectedTab == 0 ? " class=\"active\"" : null //2
   //                         , showServiceUrl ?
   //                             string.Format("<li><a href=\"{0}\"{1}>{2}</a></li>"
   //                                 , string.Concat(Console.WimPagePath, "?page=", Console.Item, "&tab=1")//, selectedTab)// == 1 ? "0" : "1")
   //                                 , selectedTab == 1 ? " class=\"active\"" : null
   //                                 , Labels.ResourceManager.GetString("tab_ServiceColumn", new CultureInfo(Console.CurrentApplicationUser.LanguageCulture))
   //                                 ) : string.Empty //3

   //                         );
   //                 }
   //             }

   //             #endregion PAGE

   //             else
   //             {
   //                 //  Show NO tabs
   //                 tabTag = string.Format(@"
			//            <li class=""active""><a href=""{1}"">{0}</a></li>"
   //                     , title
   //                     , Console.WimPagePath
   //                     );
   //             }
   //         }

   //         #endregion

   //         #region Folder & page

   //         else if (Console.CurrentList.Type == ComponentListType.Folders || Console.CurrentList.Type == ComponentListType.PageProperties)
   //         {
   //             tabTag = string.Format(@"
			//            <li class=""active""><a href=""{1}"">{0}</a></li>"
   //                 , title
   //                 , Console.WimPagePath
   //                 );
   //         }

   //         #endregion

   //         #region Assets

   //         else if (Console.CurrentList.Type == ComponentListType.Documents || Console.CurrentList.Type == ComponentListType.Images)
   //         {
   //             title = "Browsing";

   //             int galleryID = Utility.ConvertToInt(Console.Request.Query["gallery"]);
   //             if (galleryID == 0)
   //             {
   //                 galleryID = Asset.SelectOne(Console.Item.Value).GalleryID;
   //             }

   //             tabTag = string.Format(@"
			//            <li><a href=""{1}"">{0}</a></li>
   //                     <li{3}><a href=""{2}"">{4}</a></li>"
   //                 , title
   //                 , string.Concat(Console.WimPagePath, "?gallery=", galleryID)
   //                 , string.Concat(Console.WimPagePath, "?gallery=", galleryID, (Console.CurrentList.Type == ComponentListType.Documents ? "&gfx=" : "&gfx=")
   //                 , Console.Item.GetValueOrDefault())
   //                 , selectedTab == 0 ? " class=\"active\"" : null
   //                 , Console.CurrentList.SingleItemName
   //                 );
   //         }

   //         #endregion


   //         #region Custom lists

   //         else
   //         {
   //             bool isSingleItemList = (Console.CurrentList.IsSingleInstance || Console.CurrentListInstance.wim.CanContainSingleInstancePerDefinedList);

   //             //  Show NO tabs
   //             if (isSingleItemList)
   //             {
   //                 return null;
   //             }

   //             if (Console.Item.HasValue || isSingleItemList)
   //             {
   //                 var master = Console;

   //                 int currentListId = Console.Logic;
   //                 int currentListItemId = Console.Item.GetValueOrDefault();
   //                 string itemTitle = Console.CurrentList.SingleItemName;

   //                 //  Testcode
   //                 List<WimComponentListRoot.Tabular> tabularList = null;
   //                 if (!string.IsNullOrEmpty(Console.Request.Query["group"]))
   //                 {
   //                     int groupId = Utility.ConvertToInt(Console.Request.Query["group"]);
   //                     int groupElementId = Utility.ConvertToInt(Console.Request.Query["groupitem"]);
   //                     if (groupId != Console.CurrentList.ID)
   //                     {

   //                         if (Console.CurrentList.Type == ComponentListType.ComponentListProperties)
   //                         {
   //                             tabularList = Console.CurrentListInstance.wim.m_Collection;
   //                         }
   //                         else
   //                         {
   //                             IComponentList innerlist = ComponentList.SelectOne(groupId);

   //                             //  The current requested list is not the list that is the base of the tabular menu
   //                             master = Console.ReplicateInstance(innerlist);
   //                             master.CurrentListInstance.wim.Console = master;
   //                             master.CurrentListInstance.wim.Console.Item = groupElementId;

   //                             master.CurrentListInstance.wim.DoListInit();
   //                             if (!master.CurrentListInstance.wim.CurrentList.Option_FormAsync)
   //                                 master.CurrentListInstance.wim.DoListLoad(groupElementId, 0);

   //                             tabularList = master.CurrentListInstance.wim.m_Collection;

   //                             currentListId = groupId;
   //                             currentListItemId = groupElementId;
   //                             title = master.CurrentList.Name;
   //                             itemTitle = master.CurrentList.SingleItemName;
   //                         }
   //                     }
   //                 }
   //                 else if (Console.CurrentListInstance.wim.m_Collection != null)
   //                     tabularList = Console.CurrentListInstance.wim.m_Collection;


   //                 StringBuilder tabulars = new StringBuilder();
   //                 if (tabularList != null)
   //                 {
   //                     foreach (WimComponentListRoot.Tabular t in tabularList)
   //                     {
   //                         if (t.List.IsNewInstance)
   //                         {
   //                             continue;
   //                         }

   //                         ApplyTabularUrl(Console, t, 1);

   //                         tabulars.Append($@"<li{(t.Selected ? " class=\"active\"" : null)}><a href=""{t.Url}"">{t.TitleValue}</a></li>");

   //                         if (t.Selected)
   //                         {
   //                             selectedTab = t.List.ID;
   //                         }

   //                         if (!Console.Group.HasValue)
   //                         {
   //                             continue;
   //                         }

   //                         if (Console.CurrentListInstance.wim.CurrentList.ID == t.List.ID)
   //                         {
   //                             if (Console.CurrentListInstance.wim.m_Collection != null)
   //                             {
   //                                 foreach (WimComponentListRoot.Tabular t2 in Console.CurrentListInstance.wim.m_Collection)
   //                                 {
   //                                     ApplyTabularUrl(Console, t2, 2);

   //                                     tabulars.Append($@"<li{(t2.Selected ? " class=\"active\"" : null)}><a href=""{t2.Url}"">{t2.TitleValue}</a></li>");

   //                                     if (t2.Selected)
   //                                     {
   //                                         selectedTab = t2.List.ID;
   //                                     }
   //                                 }
   //                             }
   //                         }
   //                         else
   //                         {
   //                             IComponentListTemplate cl = t.List.GetInstance(Console.Context);
   //                             if (cl != null)
   //                             {
   //                                 cl.wim.Console = master;

   //                                 int group2Id = Utility.ConvertToInt(Console.Request.Query["group2"]);
   //                                 int group2ElementId = Utility.ConvertToInt(Console.Request.Query["group2item"]);

   //                                 if (t.List.ID == group2Id)
   //                                 {
   //                                     cl.wim.DoListLoad(group2ElementId, 0);
   //                                 }
   //                                 else
   //                                 {
   //                                     cl.wim.DoListLoad(Console.Item.Value, 0);
   //                                 }

   //                                 if (cl.wim.m_Collection != null)
   //                                 {
   //                                     foreach (WimComponentListRoot.Tabular t2 in cl.wim.m_Collection)
   //                                     {
   //                                         tabulars.Append($@"<li><a href=""{t2.Url}""{(t2.Selected ? " class=\"active\"" : null)}>{t2.TitleValue}</a></li>");

   //                                         if (t2.Selected)
   //                                         {
   //                                             selectedTab = t2.List.ID;
   //                                         }
   //                                     }
   //                                 }
   //                             }
   //                         }
   //                     }
   //                 }


   //                 WimComponentListRoot.Tabular tmp = new WimComponentListRoot.Tabular();
   //                 tmp.SelectedItem = currentListItemId;
   //                 ApplyTabularUrl(Console, tmp, 0, currentListId);

   //                 //  For use in property tabbing.
   //                 int baseID = Utility.ConvertToInt(Console.Request.Query["base"]);
   //                 if (baseID > 0)
   //                 {
   //                     IComponentList list = ComponentList.SelectOne(baseID);
   //                     title = list.Name;
   //                     currentListId = list.ID;
   //                 }

   //                 if (isSingleItemList)
   //                 {
   //                     tabTag = string.Format(@"
   //                     <li{2}><a href=""{1}"">{3}</a></li>{4}"
   //                         , title
   //                         , Console.UrlBuild.GetListRequest(currentListId)
   //                         , selectedTab == 0 ? " class=\"active\"" : null
   //                         , itemTitle, tabulars
   //                         );
   //                 }
   //                 else
   //                 {
   //                     string addition = GetQueryStringRecording(Console);
   //                     tabTag = string.Format(@"
			//            <li class=""active""><a href=""{1}"">{0}</a>{2}
   //                     </li>
   //                     "
   //                         , itemTitle
   //                         , tmp.Url
   //                         , tabulars.Length > 0 ? $"<ul>{tabulars}</ul>" : string.Empty
   //                         );
   //                 }
   //             }
   //             else
   //             {
   //                 //  Show NO tabs
   //                 tabTag = string.Format(@"
			//            <li class=""active""><a href=""{1}"">{0}</a></li>"
   //                     , Console.CurrentList.Name
   //                     , Console.WimPagePath
   //                     );
   //             }
   //         }

   //         #endregion

   //         string tabs = GetTabularTagNewDesign(Console, Console.CurrentList.Name, 0, false);

   //         bool isFirstLevelRootnavigation = false;
   //         bool isFirst = true;

   //         Data.Folder currentFolder = resolver.Folder;

   //         //  If the request is in a tabular the left navigation should show the navigation of the primary list (group ID)
   //         if (resolver.Query.ContainsKey("folder") == false && resolver.GroupID.GetValueOrDefault(0) > 0)
   //         {
   //             Data.IComponentList folderList = Data.ComponentList.SelectOne(resolver.GroupID.Value);
   //             if (folderList.FolderID.HasValue)
   //             {
   //                 currentFolder = await Data.Folder.SelectOneAsync(folderList.FolderID.Value).ConfigureAwait(false);
   //                 if (resolver.SiteID.GetValueOrDefault(0) > 0 && currentFolder.SiteID != resolver.SiteID.Value)
   //                 {
   //                     if (currentFolder.MasterID.HasValue)
   //                     {
   //                         currentFolder = await Data.Folder.SelectOneAsync(currentFolder.MasterID.Value, resolver.SiteID.Value);
   //                     }
   //                 }
   //             }
   //         }

   //         string currentName = currentFolder.Name;
   //         string currentLink = "";
   //         bool isPageProperty = resolver?.List?.Type == Data.ComponentListType.PageProperties;

   //         #region Foldertype: Galleries

   //         if (currentFolder.Type == Data.FolderType.Gallery)
   //         {
   //             int currentListID = resolver.GroupID.HasValue ? resolver.GroupID.Value : resolver.ListID.Value;

   //             Data.Gallery root = await Data.Gallery.SelectOneRootAsync().ConfigureAwait(false);

   //             int rootID = root.ID;
   //             if (role.GalleryRoot.HasValue)
   //             {
   //                 rootID = role.GalleryRoot.Value;
   //             }

   //             currentName = "Documents";
   //             currentLink = "##";// TODO: get correct url

   //             Data.Gallery currentGallery = await Data.Gallery.SelectOneAsync(currentFolder.ID).ConfigureAwait(false);

   //             Data.Gallery level1 = await Data.Gallery.SelectOneAsync(currentGallery, 1).ConfigureAwait(false);
   //             Data.Gallery level2 = await Data.Gallery.SelectOneAsync(currentGallery, 2).ConfigureAwait(false);
   //             Data.Gallery level3 = await Data.Gallery.SelectOneAsync(currentGallery, 3).ConfigureAwait(false);

   //             //  LEVEL 1 : Folders
   //             Data.Gallery[] galleries1 = await Data.Gallery.SelectAllByParentAsync(rootID).ConfigureAwait(false);

   //             if (!CommonConfiguration.RIGHTS_GALLERY_SUBS_ARE_ALLOWED)
   //             {
   //                 galleries1 = (await Data.Gallery.ValidateAccessRightAsync(galleries1, role).ConfigureAwait(false)).ToArray();
   //             }

   //             foreach (Data.Gallery folder in galleries1)
   //             {
   //                 bool isActive = currentGallery.ID == folder.ID || level1.ID == folder.ID;
                    
   //                 string iconClass = "folder";
   //                 iconClass += isActive ? " active" : "";
   //                 iconClass += isFirst ? " first" : "";

   //                 result.Items.Add(new NavigationItem()
   //                 { 
   //                     IconClass = iconClass,
   //                     Text = folder.Name,
   //                     Href="##" // TODO: get correct url
   //                 });

   //                 isFirst = false;
   //             }
   //         }

   //         #endregion

   //         #region Foldertype: Lists

   //         if (isPageProperty && resolver.ItemID.HasValue)
   //         {
   //             Data.Page p = Data.Page.SelectOne(resolver.ItemID.Value);
   //             string currentFolderName = p.Folder.Name;
   //             if (currentFolderName == "/")
   //             {
   //                 currentFolderName = p.Folder.Site.Name;
   //             }

   //             result.Items.Add(new NavigationItem()
   //             {
   //                 Text = currentFolderName,
   //                 Href = "##",// TODO: get correct url
   //                 IsBack = true,
   //             });

   //             if (string.IsNullOrWhiteSpace(tabs) == false)
   //             {
   //                 build.AppendFormat(tabs);
   //             }
   //         }
   //         else if (currentFolder.Type == Data.FolderType.List || currentFolder.Type == Data.FolderType.Administration)
   //         {
   //             int currentListID = resolver.GroupID.HasValue ? resolver.GroupID.Value : resolver.ListID.GetValueOrDefault(0);

   //             if (!resolver.ItemID.HasValue)
   //             {
   //                 if (!CommonConfiguration.HIDE_BREADCRUMB)
   //                 {
   //                     result.Items.Add(new NavigationItem()
   //                     {
   //                         Text = "Home",
   //                         Href = "##",// TODO: get correct url
   //                         IsBack = true,
   //                     });
   //                 }
   //             }
   //             else
   //             {
   //                 var lists = await Data.ComponentList.SelectOneAsync(currentListID).ConfigureAwait(false);

   //                 result.Items.Add(new NavigationItem()
   //                 {
   //                     Text = lists.Name,
   //                     Href = "##",// TODO: get correct url
   //                     IsBack = true,
   //                 });
   //             }

   //             if (Console.CurrentList.Type != ComponentListType.Browsing)
   //             {
   //                 IComponentList[] lists1 = ComponentList.SelectAll(currentFolder.ID);
   //                 lists1 = ComponentList.ValidateAccessRight(lists1,
   //                     Console.CurrentApplicationUser);

   //                 foreach (ComponentList list in lists1)
   //                 {
   //                     if (Console.Item.HasValue)
   //                     {
   //                         if (Console.Group.HasValue)
   //                         {
   //                             if (Console.Group.Value != list.ID)
   //                                 continue;
   //                         }
   //                         else
   //                         {
   //                             if (list.ID != Console.CurrentList.ID)
   //                                 continue;
   //                         }
   //                     }

   //                     string dataReport = null;
   //                     ComponentDataReportEventArgs e = null;
   //                     if (list.Option_HasDataReport && !(list.ID == currentListID && Console.Item.HasValue))
   //                     {
   //                         var instance = list.GetInstance(Console.Context);
   //                         if (instance != null)
   //                         {
   //                             e = instance.wim.DoListDataReport();
   //                             if (e != null && e.ReportCount.HasValue)
   //                             {
   //                                 string count = e.ReportCount.Value.ToString();
   //                                 if (e.ReportCount.Value > 99)
   //                                     count = "99+";

   //                                 dataReport = string.Format(" <span class=\"items\">{0}</span>", count);
   //                             }
   //                         }
   //                     }

   //                     if (list.IsVisible)
   //                     {
   //                         string x = string.Format(@"<li{3}>{6}<a href=""{0}"" class=""{1}{4}"">{8}{2}{7}</a>{5}</li>"
   //                             , Console.UrlBuild.GetListRequest(list)
   //                             , "list"
   //                             , list.Name
   //                             , list.ID == currentListID ? (Console.Item.HasValue ? @" class=""back""" : @" class=""active""") : string.Empty
   //                             , isFirst ? " first" : null
   //                             //, list.ID == currentListID ? tabs : string.Empty
   //                             , Console.Item.HasValue ? (list.ID == currentListID ? tabs : string.Empty) : string.Empty
   //                             , list.ID == currentListID ? (Console.Item.HasValue ? @"<span class=""icon-arrow-left-04""></span>" : string.Empty) : string.Empty
   //                             , dataReport
   //                             , string.IsNullOrWhiteSpace(list.Icon) ? null : $"<i class=\"{list.Icon}\"></i> " //8
   //                             );
   //                         build.Append(x);
   //                         isFirst = false;
   //                     }
   //                 }
   //             }

   //             if (isFirst)
   //             {
   //                 if (!string.IsNullOrEmpty(Console.Request.Query["base"]))
   //                 {
   //                     var list = ComponentList.SelectOne(Convert.ToInt32(Console.Request.Query["base"]));
   //                     build.AppendFormat(@"<li class=""back""><span class=""icon-arrow-left-04""></span><a href=""{0}"">{2}{1}</a></li>"
   //                         , Console.UrlBuild.GetListRequest(list)
   //                         , list.Name
   //                         , string.IsNullOrWhiteSpace(list.Icon) ? null : $"<i class=\"{list.Icon}\"></i> " //2

   //                         );
   //                 }

   //                 build.Append(tabs);
   //             }
   //         }
   //         #endregion
   //         #region Foldertype: Pages
   //         else if (currentFolder.Type == FolderType.Page)
   //         {
   //             if (Console.ItemType == RequestItemType.Page)
   //             {
   //                 string currentFolderName = currentFolder.Name;
   //                 if (currentFolderName == "/")
   //                     currentFolderName = currentFolder.Site.Name;

   //                 build.AppendFormat(@"<li class=""back""><span class=""icon-arrow-left-04""></span><a href=""{0}"">{1}</a></li>"
   //                     , Console.UrlBuild.GetFolderRequest(currentFolder.ID), currentFolderName
   //                     );

   //                 if (string.IsNullOrWhiteSpace(tabs) == false)
   //                 {
   //                     build.AppendFormat(tabs);
   //                 }
   //             }
   //             else
   //             {
   //                 Folder root;
   //                 if (isFirstLevelRootnavigation)
   //                     root = Folder.SelectOne(currentFolder, 1);
   //                 else
   //                     root = Folder.SelectOneBySite(Console.CurrentListInstance.wim.CurrentSite.ID, currentFolder.Type);

   //                 var arr = Folder.SelectAllByParent(root.ID);

   //                 foreach (var item in arr)
   //                 {
   //                     if (item.IsVisible)
   //                     {
   //                         build.AppendFormat(@"<li{3}><a href=""{0}"" class=""{1}{4}"">{2}</a></li>"
   //                             , Console.UrlBuild.GetFolderRequest(item)
   //                             , "list"
   //                             , item.Name
   //                             , null
   //                             , isFirst ? " first" : null);
   //                         isFirst = false;
   //                     }
   //                 }
   //             }
   //         }
   //         #endregion


   //         if (Console.CurrentList.Type == ComponentListType.InformationMessage)
   //         {
   //             build = new StringBuilder();
   //             build.AppendFormat(@"<li><a href=""{0}"" class=""{1}"">{2}</a></li>", Console.UrlBuild.GetHomeRequest(), "list", "Home");
   //         }

   //         if (!string.IsNullOrEmpty(currentLink))
   //             currentName = string.Format("<a href=\"{1}\">{0}</a>", currentName, currentLink);

   //         return string.Format(@"
   //         <aside id=""homeAside"">
			//	<section id=""sideNav"">
	  //              <menu class=""sideNav"">
		 //               <ul>
   //                         {2}
		 //               </ul>
	  //              </menu>
   //             </section>
			//</aside>"
   //             , Console.WimRepository
   //             , currentName
   //             , build
   //             );


            return Ok(result);
        }

    }
}
