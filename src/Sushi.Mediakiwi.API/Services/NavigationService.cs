using Sushi.Mediakiwi.API.Transport;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.API.Services
{
    public class NavigationService : INavigationService
    {
        public string GetLogoURL(UrlResolver resolver)
        {
            if (string.IsNullOrWhiteSpace(CommonConfiguration.LOGO_URL))
            {
                var filePath = resolver.AddApplicationPath(CommonConfiguration.LOCAL_FILE_PATH, true);
                return CommonConfiguration.CDN_Folder(filePath, "images") + @"MK_logo.png";
            }

            return resolver.AddApplicationPath(CommonConfiguration.LOGO_URL, true);
        }

        public string GetHomepageURL(UrlResolver resolver)
        {
            return resolver.UrlBuild.GetHomeRequest();
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



        public async Task<string> GetUrlAsync(UrlResolver resolver, Data.IMenuItemView entity, int? channelId)
        {
            var querystring = string.Empty;

            switch (entity.TypeID)
            {
                case 1: return await resolver.UrlBuild.GetListRequestAsync(Convert.ToInt32(entity.ItemID)).ConfigureAwait(false);
                case 2: querystring = $"?folder={entity.ItemID}"; break;
                case 3: querystring = $"?page={entity.ItemID}"; break;
                case 4: querystring = $"?dashboard={entity.ItemID}"; break;
                case 5: querystring = $"?ligalleryst={entity.ItemID}"; break;
                case 6: querystring = $"?top=1"; break;
                case 7: querystring = $"?top={entity.ItemID}"; break;
                case 8: querystring = $"?folder={entity.ItemID}"; break;
                default: querystring = ""; break;
            }

            var wimPath = await resolver.GetWimPagePathAsync(channelId).ConfigureAwait(false);
            return $"{wimPath}{querystring}";
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

        public string GetQueryStringRecording(UrlResolver resolver)
        {
            string addition = string.Empty;
            if (resolver.ListInstance.wim?.GetQueryStringRecording()?.Any() == true)
            {
                resolver.ListInstance.wim.GetQueryStringRecording().ToList().ForEach(x =>
                {
                    if (!string.IsNullOrEmpty(resolver.Query[x]))
                    {
                        addition += $"&{x}={resolver.Query[x]}";
                    }
                });
            }
            return addition;
        }

        public void ApplyTabularUrl(UrlResolver resolver, Framework.WimComponentListRoot.Tabular tab, int levelEntry, int? currentListID)
        {
            int listID = resolver.List != null ? resolver.List.ID : Utils.ConvertToInt(resolver.Query["list"]);

            string addition = GetQueryStringRecording(resolver);

            string folderInfo = null;
            if (resolver.FolderID.GetValueOrDefault(0) > 0)
            {
                folderInfo = $"&folderInfo={resolver.FolderID.Value}";
            }

            string baseInfo = null;

            if (resolver.BaseID.GetValueOrDefault(0) > 0)
            {
                baseInfo = $"&base={resolver.BaseID.Value}";
            }

            if (levelEntry == 1)
            {
                if (resolver.GroupID.GetValueOrDefault(0) == 0)
                {
                    tab.Url = $"{resolver.UrlBuild.GetListRequest(tab.List, tab.SelectedItem)}&group={listID}{addition}{baseInfo}{folderInfo}&groupitem={resolver.ItemID.GetValueOrDefault(0)}&list={tab.List.ID}";
                }
                else
                {
                    tab.Url = $"{resolver.WimPagePath}?group={resolver.GroupID.Value}{addition}{baseInfo}{folderInfo}&groupitem={resolver.GroupItemID.GetValueOrDefault(0)}&list={tab.List.ID}&item={(resolver.Group2ID.GetValueOrDefault(0) == tab.List.ID ? resolver.Group2ItemID.GetValueOrDefault(0) : tab.SelectedItem)}";
                }
            }
            else if (levelEntry == 2)
            {
                tab.Url = $"{resolver.WimPagePath}?group={resolver.GroupID.GetValueOrDefault(0)}{addition}{baseInfo}{folderInfo}&groupitem={resolver.GroupItemID.GetValueOrDefault(0)}&group2={listID}&group2item={resolver.ItemID.GetValueOrDefault(0)}&list={tab.List.ID}&item={tab.SelectedItem}";
            }
            else
            {
                tab.Url = $"{resolver.WimPagePath}?list={listID}{addition}{baseInfo}{folderInfo}&item={tab.SelectedItem}";
            }

            if (resolver.OpenInFrame.HasValue && tab.Url.Contains("?"))
            {
                tab.Url += $"&openinframe={resolver.OpenInFrame.GetValueOrDefault()}";
            }
        }

        public async Task<(bool isCurrent, bool addEmpty)> AddSubSubNavigationAsync(UrlResolver resolver, NavigationItem topnav, Data.IMenuItemView item, string className, Data.IApplicationRole role)
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

                    if (resolver.ListID.HasValue && subItem.ItemID == resolver.ListID.Value)
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
