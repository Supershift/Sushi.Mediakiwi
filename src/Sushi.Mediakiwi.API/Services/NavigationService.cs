using Sushi.Mediakiwi.API.Transport;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.API.Services
{
    public class NavigationService : INavigationService
    {
        public string GetLogoURL(Beta.GeneratedCms.Console container)
        {
            if (string.IsNullOrWhiteSpace(CommonConfiguration.LOGO_URL))
            {
                return CommonConfiguration.CDN_Folder(container, "images") + @"MK_logo.png";
            }
            return container.AddApplicationPath(CommonConfiguration.LOGO_URL, true);
        }

        public string GetHomepageURL(Beta.GeneratedCms.Console container)
        {
            return container.UrlBuild.GetHomeRequest();
        }

        public bool IsRequestPartOfNavigation(Data.IMenuItemView item, UrlResolver urlResolver)
        {
         
            //  When the topnav is a section
            if (item.TypeID == 7)
            {
                //  When the item is of type 'website'
                if (item.ItemID == 1 && urlResolver.Folder != null && !urlResolver.Folder.IsNewInstance && urlResolver.Folder.Type == Data.FolderType.Page && urlResolver.Folder.SiteID == urlResolver.SiteID)
                {
                    return true;
                }
                //  When the item is of type 'lists'
                if (item.ItemID == 2 && urlResolver.Folder != null && !urlResolver.Folder.IsNewInstance && urlResolver.Folder.Type == Data.FolderType.List)
                {
                    return true;
                }
                //  When the item is of type 'gallery'
                if (item.ItemID == 3 && urlResolver.Folder != null && !urlResolver.Folder.IsNewInstance && urlResolver.Folder.Type == Data.FolderType.Gallery)
                {
                    return true;
                }
                //  When the item is of type 'admin'
                if (item.ItemID == 4 && urlResolver.Folder != null && !urlResolver.Folder.IsNewInstance && urlResolver.Folder.Type == Data.FolderType.Administration)
                {
                    return true;
                }
            }

            //  When the topnav is a section
            if (item.TypeID == 6 && item.ItemID == urlResolver.SiteID && urlResolver.Folder != null && !urlResolver.Folder.IsNewInstance && urlResolver.Folder.Type == Data.FolderType.Page)
            {
                //  When the item is of type 'website'
                return true;
            }

            //  When the topnav item is a list
            if (item.TypeID == 1 && urlResolver.List != null && !urlResolver.List.IsNewInstance && item.ItemID == urlResolver.ListID.GetValueOrDefault(0))
            {

                return true;
            }

            //  When the topnav item is a folder
            if ((item.TypeID == 2 || item.TypeID == 8) && urlResolver.Folder != null && !urlResolver.Folder.IsNewInstance && item.ItemID == urlResolver.Folder.ID)
            {
                return true;
            }

            return false;
        }


        public string GetUrl(Beta.GeneratedCms.Console container, Data.IMenuItemView entity, int channel)
        {
            var querystring = string.Empty;

            switch (entity.TypeID)
            {
                case 1: return container.UrlBuild.GetListRequest(Convert.ToInt32(entity.ItemID));
                case 2: querystring = $"?folder={entity.ItemID}"; break;
                case 3: querystring = $"?page={entity.ItemID}"; break;
                case 4: querystring = $"?dashboard={entity.ItemID}"; break;
                case 5: querystring = $"?ligalleryst={entity.ItemID}"; break;
                case 6: querystring = $"?top=1"; break;
                case 7: querystring = $"?top={entity.ItemID}"; break;
                case 8: querystring = $"?folder={entity.ItemID}"; break;
                default: querystring = ""; break;
            }

            return string.Concat(container.GetWimPagePath(channel), querystring);
        }

        public async Task<bool> HasRoleAccessAsync(Data.IMenuItemView item, Data.IApplicationRole role)
        {
            if (item.TypeID == 1)
            {
                var tmp = await Data.ComponentList.SelectOneAsync(item.ItemID).ConfigureAwait(false);
                return await tmp.HasRoleAccessAsync(role).ConfigureAwait(false);
            }
            if (item.TypeID == 2 || item.TypeID == 8)
            {
                var tmp = Data.Folder.SelectOne(item.ItemID);
                return await tmp.HasRoleAccessAsync(role).ConfigureAwait(false);
            }

            return true;
        }

        public async Task<bool> HasRoleAccessAsync(Data.Interfaces.ISearchView item, Data.IApplicationRole role)
        {
            if (item.TypeID == 1)
            {
                var tmp = await Data.ComponentList.SelectOneAsync(item.ItemID).ConfigureAwait(false);
                return await tmp.HasRoleAccessAsync(role).ConfigureAwait(false);
            }
            if (item.TypeID == 2 || item.TypeID == 8)
            {
                var tmp = await Data.Folder.SelectOneAsync(item.ItemID).ConfigureAwait(false);
                return await tmp.HasRoleAccessAsync(role).ConfigureAwait(false);
            }

            return true;
        }

        public string GetQueryStringRecording(Beta.GeneratedCms.Console container)
        {
            string addition = string.Empty;
            if (container.CurrentListInstance.wim.GetQueryStringRecording().Count > 0)
            {
                container.CurrentListInstance.wim.GetQueryStringRecording().ToList().ForEach(x =>
                {
                    if (!string.IsNullOrEmpty(container.Request.Query[x]))
                    {
                        addition += string.Format("&{0}={1}", x, container.Request.Query[x]);
                    }
                });
            }
            return addition;
        }

        public void ApplyTabularUrl(Beta.GeneratedCms.Console container, Framework.WimComponentListRoot.Tabular tab, int levelEntry, int? currentListID)
        {
            int listID = container.CurrentList != null ? container.CurrentList.ID : Utils.ConvertToInt(container.Request.Query["list"]);

            string addition = GetQueryStringRecording(container);

            int itemID = Utils.ConvertToInt(container.Request.Query["item"]);

            int? openinframeID = Utils.ConvertToIntNullable(container.Request.Query["openinframe"]);

            int groupID = Utils.ConvertToInt(container.Request.Query["group"]);
            int groupItemID = Utils.ConvertToInt(container.Request.Query["groupitem"]);

            int group2ID = Utils.ConvertToInt(container.Request.Query["group2"]);
            int group2ItemID = Utils.ConvertToInt(container.Request.Query["group2item"]);

            int folderID = Utils.ConvertToInt(container.Request.Query["folder"]);
            string folderInfo = null;
            if (folderID > 0)
            {
                folderInfo = string.Concat("&folder=", folderID);
            }

            int baseID = Utils.ConvertToInt(container.Request.Query["base"]);
            string baseInfo = null;

            if (baseID > 0)
            {
                baseInfo = string.Concat("&base=", baseID);
            }

            if (levelEntry == 1)
            {
                if (groupID == 0)
                {
                    tab.Url = $"{container.UrlBuild.GetListRequest(tab.List, tab.SelectedItem)}&group={listID}{addition}{baseInfo}{folderInfo}&groupitem={itemID}&list={tab.List.ID}";
                }
                else
                {
                    tab.Url = $"{container.WimPagePath}?group={groupID}{addition}{baseInfo}{folderInfo}&groupitem={groupItemID}&list={tab.List.ID}&item={(group2ID == tab.List.ID ? group2ItemID : tab.SelectedItem)}";
                }
            }
            else if (levelEntry == 2)
            {
                tab.Url = $"{container.WimPagePath}?group={groupID}{addition}{baseInfo}{folderInfo}&groupitem={groupItemID}&group2={listID}&group2item={itemID}&list={tab.List.ID}&item={tab.SelectedItem}";
            }
            else
            {
                tab.Url = $"{container.WimPagePath}?list={listID}{addition}{baseInfo}{folderInfo}&item={tab.SelectedItem}";
            }

            if (openinframeID.HasValue && tab.Url.Contains("?"))
            {
                tab.Url += $"&openinframe={openinframeID.GetValueOrDefault()}";
            }
        }

        public async Task<(bool isCurrent, bool addEmpty)> AddSubSubNavigationAsync(Beta.GeneratedCms.Console container, NavigationItem topnav, Data.IMenuItemView item, string className, Data.IApplicationRole role)
        {
            bool isCurrent = false;
            bool addEmpty = false;

            var subSubnavigation = await Data.SearchView.SelectAllAsync(item.ItemID).ConfigureAwait(false);

            if (subSubnavigation.Length > 0)
            {
                foreach (var subItem in subSubnavigation)
                {
                    //  If the item is the same as the callee 
                    if (subItem.ID == item.Tag || subItem.ID == string.Concat("2_", item.ItemID))
                    {
                        continue;
                    }

                    if (!await HasRoleAccessAsync(subItem, role).ConfigureAwait(false))
                    {
                        continue;
                    }

                    if (topnav.ItemID == subItem.ItemID)
                    {
                        continue;
                    }

                    if (subItem.ItemID == container.CurrentList.ID)
                    {
                        isCurrent = true;
                    }
                }
                addEmpty = true;
            }

            return (isCurrent, addEmpty);
        }
    }
}
