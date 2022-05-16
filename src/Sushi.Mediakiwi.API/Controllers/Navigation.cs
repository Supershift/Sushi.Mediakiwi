using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Sushi.Mediakiwi.API.Services;
using Sushi.Mediakiwi.API.Transport.Requests;
using Sushi.Mediakiwi.API.Transport.Responses;
using System.Linq;
using System;
using Sushi.Mediakiwi.API.Transport;
using System.Collections.Generic;
using System.Globalization;
using Sushi.Mediakiwi.API.Filters;

namespace Sushi.Mediakiwi.API.Controllers
{
    [ApiController]
    [MediakiwiApiAuthorize]
    [Route(Common.MK_CONTROLLERS_PREFIX + "navigation")]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public class Navigation : BaseMediakiwiApiController
    {
        private readonly INavigationService _navService;

        public Navigation(INavigationService _service)
        {
            _navService = _service;
        }

        #region Top Navigation

        /// <summary>
        /// Returns the Top navigation belonging to the URL being viewed.
        /// </summary>
        /// <param name="request">The request containing the needed information</param>
        /// <returns></returns>
        /// <response code="200">The Navigation is succesfully retrieved</response>
        /// <response code="400">Some needed information is missing from the request</response>
        /// <response code="401">The user is not succesfully authenticated</response>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpGet("GetTopnavigation")]
        public async Task<ActionResult<GetTopNavigationResponse>> GetTopnavigation([FromQuery] GetTopNavigationRequest request)
        {
            if (request == null || _navService == null || request.CurrentSiteID == 0)
            {
                return BadRequest();
            }

            GetTopNavigationResponse result = new GetTopNavigationResponse()
            {
                StatusCode = System.Net.HttpStatusCode.OK
            };

            // Get Appropriate role from DB
            var role = await MediakiwiUser.SelectRoleAsync().ConfigureAwait(false);

            // Get Menu for current site and Role
            var list = await Data.MenuItemView.SelectAllAsync(request.CurrentSiteID, role.ID, request.GroupTag).ConfigureAwait(false);

            if (list.Count > 0)
            {
                string className = "";

                var mainNav = list.Where(item => item.Sort == 1);
                foreach (var item in mainNav)
                {
                    bool isSelected = false;
                    if (!await _navService.HasRoleAccessAsync(item, role).ConfigureAwait(false))
                    {
                        continue;
                    }

                    // Get subnav for mainNav item
                    var subnavigation = list.Where(subNav => subNav.Sort != 1 && subNav.Position == item.Position).ToList();

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

                            if (!await _navService.HasRoleAccessAsync(subItem, role).ConfigureAwait(false))
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
                            bool isSubSelected = false;
                            if (index == 0)
                            {
                                if (item.TypeID == 8)
                                {
                                    firstInLineUrl = await _navService.GetUrlAsync(Resolver, subnavigation[index], request.CurrentSiteID).ConfigureAwait(false);
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
                                var addSubResult = await _navService.AddSubSubNavigationAsync(Resolver, navItem, subnavigation[index], className, role).ConfigureAwait(false);
                                if (addSubResult.isCurrent)
                                {
                                    isSubSelected = true;
                                }
                            }
                            else
                            {
                                if (subnavigation[index]?.ItemID == Resolver.List?.ID)
                                {
                                    isSubSelected = true;
                                }

                                navItem.Items.Add(new NavigationItem()
                                {
                                    IsHighlighted = isSubSelected,
                                    Text = subnavigation[index].Name,
                                    IconClass = className,
                                    Href = await _navService.GetUrlAsync(Resolver, subnavigation[index], request.CurrentSiteID).ConfigureAwait(false),
                                });
                            }
                        }
                    }

                    if (!isSelected)
                    {
                        isSelected = navItem.Items.Any(x => x.IsHighlighted);
                    }

                    if (!isSelected)
                    {
                        isSelected = _navService.IsRequestPartOfNavigation(item, Resolver);
                    }
                    
                    if (linkable)
                    {
                        navItem.Href = string.IsNullOrEmpty(firstInLineUrl) ? await _navService.GetUrlAsync(Resolver, item, request.CurrentSiteID).ConfigureAwait(false) : firstInLineUrl;
                        navItem.IsHighlighted = isSelected;
                        navItem.Text = item.Name;
                    }
                    else
                    {
                        navItem.Href = string.IsNullOrEmpty(firstInLineUrl) ? await _navService.GetUrlAsync(Resolver, item, request.CurrentSiteID).ConfigureAwait(false) : firstInLineUrl;
                        navItem.IsHighlighted = isSelected;
                        navItem.Text = item.Name;
                        navItem.IconClass += "noClick";
                    }

                    result.Items.Add(navItem);
                }
            }

            // Set Logo URl
            result.LogoUrl = _navService.GetLogoURL(Resolver);

            // Set homepage URL
            result.HomeUrl = await _navService.GetHomepageURLAsync(Resolver).ConfigureAwait(false);

            return Ok(result);
        }

        #endregion Top Navigation

        #region Side Navigation

        /// <summary>
        /// Returns the Side navigation belonging to the URL being viewed.
        /// </summary>
        /// <param name="request">The request containing the needed information</param>
        /// <returns></returns>
        /// <response code="200">The Navigation is succesfully retrieved</response>
        /// <response code="400">Some needed information is missing from the request</response>
        /// <response code="401">The user is not succesfully authenticated</response>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpGet("GetSidenavigation")]
        public async Task<ActionResult<GetSideNavigationResponse>> GetSidenavigation([FromQuery] GetSideNavigationRequest request)
        {
            GetSideNavigationResponse result = new GetSideNavigationResponse();

            // Should we include tabs ?
            bool showTabs = false;

            // Get Appropriate role from DB
            var role = await MediakiwiUser.SelectRoleAsync().ConfigureAwait(false);

            // Get current selected tab based off the URL
            int selectedTab = 0;
            if (string.IsNullOrWhiteSpace(Resolver.SelectedTab) == false)
            {
                selectedTab = Utils.ConvertToInt(Resolver.SelectedTab, 0);
            }

            // title placeholder
            string title = string.Empty;
            if (string.IsNullOrWhiteSpace(Resolver.List?.Name) == false)
            {
                title = Resolver.List.Name;
            }

            bool isPageProperty = Resolver.List.Type == Data.ComponentListType.PageProperties;

            // When in List Item mode, perform ListLoad
            if (Resolver.ItemType == RequestItemType.Item && Resolver.ItemID.HasValue && Resolver?.ListInstance?.wim?.HasListLoad == true)
            {
                Resolver.ListInstance.wim.DoListLoad(Resolver.ItemID.Value, 0);
            }

            #region Tabs

            if (Resolver.ListInstance?.wim?.Page?.HideTabs != true)
            {
                if (Resolver.List?.Type == Data.ComponentListType.Browsing || isPageProperty)
                {
                    title = Common.GetLabelFromResource("list_browsing", new CultureInfo(MediakiwiUser.LanguageCulture));

                    #region PAGE

                    if (Resolver.ItemType == RequestItemType.Page || isPageProperty)
                    {
                        Data.Page currentPage;
                        if (Resolver.List?.Type == Data.ComponentListType.PageProperties && Resolver.ItemID.HasValue)
                        {
                            currentPage = await Data.Page.SelectOneAsync(Resolver.ItemID.Value).ConfigureAwait(false);
                        }
                        else
                        {
                            currentPage = Resolver.Page;
                        }

                        if (currentPage?.Template?.GetPageSections()?.Length > 0)
                        {
                            var sections = currentPage.Template.GetPageSections();
                            bool? isSelected = null;

                            Data.IComponentList pageSettings = await Data.ComponentList.SelectOneAsync(new Guid("4E7BCF0F-844B-4877-AB2D-3154BE01BC0F")).ConfigureAwait(false);

                            if (sections.Length > 0)
                            {
                                result.Items.Add(new NavigationItem()
                                {
                                    Href = $"{Resolver.WimPagePath}?list={pageSettings.ID}&item={Resolver.ItemID.GetValueOrDefault(0)}",
                                    Text = Common.GetLabelFromResource("page_properties", new CultureInfo(MediakiwiUser.LanguageCulture)),
                                    IsTab = true
                                });

                                foreach (var section in sections)
                                {
                                    if (string.IsNullOrWhiteSpace(section))
                                    {
                                        continue;
                                    }

                                    if ((!isPageProperty && string.IsNullOrEmpty(Resolver.SelectedTab) && !isSelected.HasValue) || Resolver.SelectedTab == section)
                                    {
                                        isSelected = true;
                                    }
                                    else
                                    {
                                        isSelected = false;
                                    }

                                    var tabName = currentPage.Template.Data[string.Format("T[{0}]", section)].Value;
                                    if (string.IsNullOrEmpty(tabName))
                                    {
                                        tabName = section;
                                    }

                                    result.Items.Add(new NavigationItem()
                                    {
                                        Href = $"{Resolver.WimPagePath}?page={Resolver.ItemID.GetValueOrDefault(0)}&tab={section}",
                                        Text = tabName,
                                        IsHighlighted = isSelected.GetValueOrDefault(),
                                        IsTab = true
                                    });
                                }
                            }
                        }
                        else
                        {
                            result.Items.Add(new NavigationItem()
                            {
                                Href = $"{Resolver.WimPagePath}?page={Resolver.ItemID.GetValueOrDefault(0)}",
                                Text = Common.GetLabelFromResource("tab_Content", new CultureInfo(MediakiwiUser.LanguageCulture)),
                                IsHighlighted = selectedTab == 0,
                                IsTab = true
                            });
                        }
                    }

                    #endregion PAGE

                    else
                    {
                        result.Items.Add(new NavigationItem()
                        {
                            Href = Resolver.WimPagePath,
                            Text = title,
                            IsHighlighted = true,
                            IsTab = true
                        });
                    }
                }

                #region Folder & page

                else if (Resolver.List.Type == Data.ComponentListType.Folders || Resolver.List.Type == Data.ComponentListType.PageProperties)
                {
                    result.Items.Add(new NavigationItem()
                    {
                        Href = Resolver.WimPagePath,
                        Text = title,
                        IsHighlighted = true,
                        IsTab = true
                    });
                }

                #endregion

                #region Assets

                else if (Resolver.List.Type == Data.ComponentListType.Documents || Resolver.List.Type == Data.ComponentListType.Images)
                {
                    title = "Browsing";

                    int galleryID = Resolver.GalleryID.GetValueOrDefault(0);
                    if (galleryID == 0)
                    {
                        galleryID = (await Data.Asset.SelectOneAsync(Resolver.ItemID.Value).ConfigureAwait(false)).GalleryID;
                    }

                    result.Items.Add(new NavigationItem()
                    {
                        Href = $"{Resolver.WimPagePath}?gallery={galleryID}",
                        Text = title
                    });

                    result.Items.Add(new NavigationItem()
                    {
                        Href = $"{Resolver.WimPagePath}?gallery={galleryID}&gfx={Resolver.ItemID.GetValueOrDefault()}",
                        Text = Resolver.List.SingleItemName,
                        IsHighlighted = selectedTab == 0,
                        IsTab = true
                    });
                }

                #endregion

                #region Custom lists

                else
                {
                    bool isSingleItemList = (Resolver.List.IsSingleInstance || Resolver.ListInstance.wim.CanContainSingleInstancePerDefinedList);
                    string listIcon = string.Empty;

                    //Show NO tabs
                    if (isSingleItemList)
                    {
                        showTabs = false;
                    }

                    if (Resolver.ItemID.HasValue || isSingleItemList)
                    {
                        var master = Console;

                        int currentListId = Resolver.ListID.GetValueOrDefault(Console.Logic);
                        int currentListItemId = Resolver.ItemID.GetValueOrDefault();
                        string itemTitle = Resolver.List.SingleItemName;
                        listIcon = Resolver.List.Icon;

                        ICollection<Framework.WimComponentListRoot.Tabular> tabularList = null;

                        if (Resolver.GroupID.GetValueOrDefault(0) > 0)
                        {
                            if (Resolver.GroupID.Value != Resolver.List.ID)
                            {
                                if (Resolver.List.Type == Data.ComponentListType.ComponentListProperties)
                                {
                                    tabularList = Resolver.ListInstance.wim.GetTabs();
                                }
                                else
                                {
                                    Data.IComponentList innerlist = await Data.ComponentList.SelectOneAsync(Resolver.GroupID.Value).ConfigureAwait(false);

                                    //  The current requested list is not the list that is the base of the tabular menu
                                    master = Console.ReplicateInstance(innerlist);
                                    master.CurrentListInstance.wim.Console = master;
                                    master.CurrentListInstance.wim.Console.Item = Resolver.GroupItemID.GetValueOrDefault(0);
                                    master.CurrentListInstance.wim.DoListInit();

                                    if (!master.CurrentListInstance.wim.CurrentList.Option_FormAsync)
                                    {
                                        master.CurrentListInstance.wim.DoListLoad(Resolver.GroupItemID.Value, 0);
                                    }

                                    tabularList = master.CurrentListInstance.wim.GetTabs();

                                    currentListId = Resolver.GroupID.Value;
                                    currentListItemId = Resolver.GroupItemID.GetValueOrDefault(0);
                                    title = master.CurrentList.Name;
                                    itemTitle = master.CurrentList.SingleItemName;
                                    listIcon = master.CurrentList.Icon;
                                }
                            }
                        }
                        else
                        {
                            tabularList = Resolver.ListInstance.wim.GetTabs();
                        }

                        if (tabularList?.Count > 0)
                        {
                            foreach (var tab in tabularList)
                            {
                                if (tab.List.IsNewInstance)
                                {
                                    continue;
                                }

                                await _navService.ApplyTabularUrlAsync(Resolver, tab, 1, null).ConfigureAwait(false);
                                result.Items.Add(new NavigationItem()
                                {
                                    Text = tab.TitleValue,
                                    Href = tab.Url,
                                    IsHighlighted = tab.Selected,
                                    IsTab = true
                                });

                                if (tab.Selected)
                                {
                                    selectedTab = tab.List.ID;
                                }

                                if (!Resolver.GroupID.HasValue)
                                {
                                    continue;
                                }

                                if (Resolver.ListInstance.wim.CurrentList.ID == tab.List.ID)
                                {
                                    if (Resolver.ListInstance.wim.GetTabs()?.Count > 0)
                                    {
                                        foreach (var tab2 in Resolver.ListInstance.wim.GetTabs())
                                        {
                                            await _navService.ApplyTabularUrlAsync(Resolver, tab2, 2, null).ConfigureAwait(false);

                                            result.Items.Add(new NavigationItem()
                                            {
                                                Text = tab2.TitleValue,
                                                Href = tab2.Url,
                                                IsHighlighted = tab2.Selected,
                                                IsTab = true
                                            });

                                            if (tab2.Selected)
                                            {
                                                selectedTab = tab2.List.ID;
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    Framework.IComponentListTemplate cl = tab.List.GetInstance(Console);

                                    if (cl != null)
                                    {
                                        cl.wim.Console = master;

                                        if (tab.List.ID == Resolver.Group2ID)
                                        {
                                            cl.wim.DoListLoad(Resolver.Group2ItemID.Value, 0);
                                        }
                                        else
                                        {
                                            cl.wim.DoListLoad(Resolver.ItemID.Value, 0);
                                        }

                                        if (cl.wim.GetTabs()?.Count > 0)
                                        {
                                            foreach (var tab2 in cl.wim.GetTabs())
                                            {
                                                result.Items.Add(new NavigationItem()
                                                {
                                                    Text = tab2.TitleValue,
                                                    Href = tab2.Url,
                                                    IsHighlighted = tab2.Selected,
                                                    IsTab = true
                                                });

                                                if (tab2.Selected)
                                                {
                                                    selectedTab = tab2.List.ID;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }


                        var tmpTab = new Framework.WimComponentListRoot.Tabular();
                        tmpTab.SelectedItem = currentListItemId;
                        await _navService.ApplyTabularUrlAsync(Resolver, tmpTab, 0, currentListId).ConfigureAwait(false);

                        //  For use in property tabbing.

                        if (Resolver.BaseID.GetValueOrDefault(0) > 0)
                        {
                            Data.IComponentList list = await Data.ComponentList.SelectOneAsync(Resolver.BaseID.Value).ConfigureAwait(false);
                            title = list.Name;
                            currentListId = list.ID;
                            listIcon = list.Icon;
                        }

                        if (isSingleItemList)
                        {
                            result.Items.Add(new NavigationItem()
                            {
                                Text = itemTitle,
                                Href = Resolver.UrlBuild.GetListRequest(currentListId),
                                IsHighlighted = selectedTab == 0,
                                IconClass = listIcon,
                                IsTab = true
                            });
                        }
                        else
                        {
                            result.Items.Add(new NavigationItem()
                            {
                                Text = itemTitle,
                                Href = Resolver.UrlBuild.GetListRequest(currentListId, currentListItemId),
                                IsHighlighted = true, 
                                IconClass = listIcon,
                                //IsTab = true
                            });
                        }
                    }
           
                }

                #endregion

            }

            #endregion

            bool isFirstLevelRootnavigation = false;
            bool isFirst = true;

            Data.Folder currentFolder = Resolver.ListInstance.wim.CurrentFolder;

            //  If the request is in a tabular the left navigation should show the navigation of the primary list (group ID)
            if (Resolver.Query.ContainsKey("folder") == false && Resolver.GroupID.GetValueOrDefault(0) > 0)
            {
                Data.IComponentList folderList = await Data.ComponentList.SelectOneAsync(Resolver.GroupID.Value).ConfigureAwait(false);
                if (folderList.FolderID.HasValue)
                {
                    currentFolder = await Data.Folder.SelectOneAsync(folderList.FolderID.Value).ConfigureAwait(false);
                    if (Resolver.SiteID.GetValueOrDefault(0) > 0 && currentFolder.SiteID != Resolver.SiteID.Value)
                    {
                        if (currentFolder.MasterID.HasValue)
                        {
                            currentFolder = await Data.Folder.SelectOneAsync(currentFolder.MasterID.Value, Resolver.SiteID.Value);
                        }
                    }
                }
            }

            isPageProperty = Resolver?.List?.Type == Data.ComponentListType.PageProperties;

            #region Foldertype: Galleries

            if (currentFolder.Type == Data.FolderType.Gallery)
            {

                Data.Gallery root = await Data.Gallery.SelectOneRootAsync().ConfigureAwait(false);

                int rootID = root.ID;
                if (role.GalleryRoot.HasValue)
                {
                    rootID = role.GalleryRoot.Value;
                }

                Data.Gallery currentGallery = await Data.Gallery.SelectOneAsync(currentFolder.ID).ConfigureAwait(false);

                Data.Gallery level1 = await Data.Gallery.SelectOneAsync(currentGallery, 1).ConfigureAwait(false);

                //  LEVEL 1 : Folders
                Data.Gallery[] galleries1 = await Data.Gallery.SelectAllByParentAsync(rootID).ConfigureAwait(false);

                foreach (Data.Gallery folder in galleries1)
                {
                    bool isActive = currentGallery.ID == folder.ID || level1.ID == folder.ID;

                    string iconClass = "folder";
                    iconClass += isActive ? " active" : "";
                    iconClass += isFirst ? " first" : "";

                    var level1Item = new NavigationItem()
                    {
                        IconClass = iconClass,
                        Text = folder.Name,
                        Href = Resolver.UrlBuild.GetGalleryRequest(folder)
                    };

                    result.Items.Add(level1Item);

                    isFirst = false;
                }
            }

            #endregion

            #region Foldertype: Lists

            if (isPageProperty && Resolver.ItemID.HasValue)
            {
                showTabs = true;
                Data.Page p = await Data.Page.SelectOneAsync(Resolver.ItemID.Value).ConfigureAwait(false);
                string currentFolderName = p.Folder.Name;
                if (currentFolderName == "/")
                {
                    currentFolderName = p.Folder.Site.Name;
                }

                result.Items.Add(new NavigationItem()
                {
                    Text = currentFolderName,
                    Href = Resolver.UrlBuild.GetFolderRequest(p.Folder.ID),
                    IsBack = true,
                });
            }
            else if (currentFolder.Type == Data.FolderType.List || currentFolder.Type == Data.FolderType.Administration)
            {
                int currentListID = Resolver.GroupID.HasValue ? Resolver.GroupID.Value : Resolver.ListID.GetValueOrDefault(0);

                if (!Resolver.ItemID.HasValue)
                {
                    if (!CommonConfiguration.HIDE_BREADCRUMB)
                    {
                        result.Items.Add(new NavigationItem()
                        {
                            Text = "Home",
                            Href = await Resolver.UrlBuild.GetHomeRequestAsync().ConfigureAwait(false),
                            IsBack = true,
                        });
                    }
                }
                else
                {
                    var list = await Data.ComponentList.SelectOneAsync(currentListID).ConfigureAwait(false);

                    result.Items.Add(new NavigationItem()
                    {
                        Text = list.Name,
                        Href = await Resolver.UrlBuild.GetListRequestAsync(list).ConfigureAwait(false),
                        IconClass = string.IsNullOrWhiteSpace(list.Icon) ? null : list.Icon,
                        IsBack = true,
                    });
                }

                if (Resolver.List.Type != Data.ComponentListType.Browsing)
                {
                    Data.IComponentList[] lists1 = await Data.ComponentList.SelectAllAsync(currentFolder.ID).ConfigureAwait(false);
                    lists1 = await Data.ComponentList.ValidateAccessRightAsync(lists1, MediakiwiUser).ConfigureAwait(false);

                    foreach (var list in lists1)
                    {
                        if (Resolver.ItemID.HasValue)
                        {
                            if (Resolver.GroupID.HasValue)
                            {
                                if (Resolver.GroupID.Value != list.ID)
                                {
                                    continue;
                                }
                            }
                            else
                            {
                                if (list.ID != Resolver.List.ID)
                                {
                                    continue;
                                }
                            }
                        }

                        string dataReport = null;
                        Framework.ComponentDataReportEventArgs e = null;
                        if (list.Option_HasDataReport && !(list.ID == currentListID && Resolver.ItemID.HasValue))
                        {
                            var instance = list.GetInstance(Console);
                            if (instance != null)
                            {
                                e = instance.wim.DoListDataReport();
                                if (e != null && e.ReportCount.HasValue)
                                {
                                    dataReport = e.ReportCount.Value.ToString();
                                    if (e.ReportCount.Value > 99)
                                    {
                                        dataReport = "99+";
                                    }
                                }
                            }
                        }

                        if (list.IsVisible)
                        {
                            result.Items.Add(new NavigationItem()
                            {
                                Text = list.Name,
                                Href = Resolver.UrlBuild.GetListRequest(list),
                                IsBack = list.ID == currentListID && Resolver.ItemID.HasValue,
                                IsHighlighted = list.ID == currentListID && Resolver.ItemID.HasValue == false,
                                IconClass = $"{(isFirst ? "first " : "")}{(string.IsNullOrWhiteSpace(list.Icon) ? null : list.Icon)}",
                                BadgeContent = dataReport
                            });

                            showTabs = Resolver.ItemID.HasValue && list.ID == currentListID;
                            isFirst = false;
                        }
                    }
                }

                if (isFirst)
                {
                    showTabs = true;
                    if (Resolver.BaseID.HasValue)
                    {
                        var list = await Data.ComponentList.SelectOneAsync(Resolver.BaseID.Value).ConfigureAwait(false);
                        result.Items.Add(new NavigationItem()
                        {
                            Text = list.Name,
                            Href = Resolver.UrlBuild.GetListRequest(list),
                            IsBack = true,
                            IconClass = string.IsNullOrWhiteSpace(list.Icon) ? null : list.Icon
                        });

                    }
                }
            }

            #endregion

            #region Foldertype: Pages

            else if (currentFolder.Type == Data.FolderType.Page)
            {
                if (Resolver.ItemType == RequestItemType.Page)
                {
                    showTabs = true;
                    string currentFolderName = currentFolder.Name;
                    if (currentFolderName == "/")
                    {
                        currentFolderName = currentFolder.Site.Name;
                    }

                    result.Items.Add(new NavigationItem()
                    {
                        Text = currentFolderName,
                        Href = Resolver.UrlBuild.GetFolderRequest(currentFolder.ID),
                        IsBack = true,
                    });
                }
                else
                {
                    Data.Folder root;
                    if (isFirstLevelRootnavigation)
                    {
                        root = await Data.Folder.SelectOneAsync(currentFolder, 1).ConfigureAwait(false);
                    }
                    else
                    {
                        root = await Data.Folder.SelectOneBySiteAsync(Resolver.ListInstance.wim.CurrentSite.ID, currentFolder.Type).ConfigureAwait(false);
                    }

                    var arr = await Data.Folder.SelectAllByParentAsync(root.ID).ConfigureAwait(false);

                    foreach (var item in arr)
                    {
                        if (item.IsVisible)
                        {
                            result.Items.Add(new NavigationItem()
                            {
                                Href = Resolver.UrlBuild.GetFolderRequest(item),
                                Text = item.Name,
                                IconClass = $"list{(isFirst ? " first" : null)}",
                            });
                            isFirst = false;
                        }
                    }
                }
            }

            #endregion


            if (Resolver.List?.Type == Data.ComponentListType.InformationMessage)
            {
                result.Items.Add(new NavigationItem()
                {
                    Text = "Home",
                    Href = await Resolver.UrlBuild.GetHomeRequestAsync().ConfigureAwait(false),
                    IconClass = "list"
                });
            }

            // Reverse items
            result.StatusCode = System.Net.HttpStatusCode.OK;

            // Clean icon name
            foreach (var item in result.Items)
            {
                if (string.IsNullOrWhiteSpace(item.IconClass) == false)
                {
                    // Trim leading and trailing spaces
                    item.IconClass = item.IconClass.Trim();

                    // Clean double spaces
                    item.IconClass = System.Text.RegularExpressions.Regex.Replace(item.IconClass, @"\s+", " ");
                }
            }

            // Remove Tabs if they're not supposed to be shown
            if (showTabs == false)
            {
                var tabItems = result.Items.Where(x => x.IsTab == true);
                result.Items = result.Items.Except(tabItems).ToList();
            }
            // Add tabs below current Item
            else
            {
                var highlightedItem = result.Items.FirstOrDefault(x => x.IsHighlighted);
                if (highlightedItem != null)
                {
                    var tabItems = result.Items.Where(x => x.IsTab == true);
                    result.Items = result.Items.Except(tabItems).ToList();

                    highlightedItem.Items = tabItems.ToList();
                }
            }

            return Ok(result);
        }

        #endregion Side Navigation

        #region Get Sites

        /// <summary>
        /// Returns a list of Sites (Channels) available for this environment.
        /// </summary>
        /// <param name="request">The request containing the needed information</param>
        /// <returns></returns>
        /// <response code="200">The Navigation is succesfully retrieved</response>
        /// <response code="400">Some needed information is missing from the request</response>
        /// <response code="401">The user is not succesfully authenticated</response>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpGet("GetSites")]
        public async Task<ActionResult<GetSitesResponse>> GetSites([FromQuery] GetSitesRequest request)
        {
            GetSitesResponse result = new GetSitesResponse();

            if (request.CurrentSiteID == 0)
            {
                return BadRequest();
            }

            foreach (var site in await Data.Site.SelectAllAsync().ConfigureAwait(false))
            {
                CultureInfo ci = new CultureInfo(site.Culture);
                int weekStart = (int)ci.DateTimeFormat.FirstDayOfWeek;
                if (weekStart == 0)
                {
                    weekStart = 7;
                }

                result.Items.Add(new SiteItem()
                {
                    Culture = site.Culture,
                    ID = site.ID,
                    Title = site.Name,
                    WeekStart = weekStart
                });
            }

            result.StatusCode = System.Net.HttpStatusCode.OK;

            return Ok(result);
        }

        #endregion Get Sites
    }
}
