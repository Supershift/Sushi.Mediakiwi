using Microsoft.AspNetCore.Http;
using Sushi.Mediakiwi.Data;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Beta.GeneratedCms.Source
{
    /// <summary>
    /// 
    /// </summary>
    public class UrlBuilder
    {
        IComponentList m_List_Folders;
        IComponentList List_Folders
        {
            get
            {
                if (m_List_Folders == null)
                {
                    m_List_Folders = ComponentList.SelectOne(ComponentListType.Folders);
                }
                return m_List_Folders;
            }
        }

        IComponentList m_List_PageProperties;
        IComponentList List_PageProperties
        {
            get
            {
                if (m_List_PageProperties == null)
                {
                    m_List_PageProperties = ComponentList.SelectOne(ComponentListType.PageProperties);
                }
                return m_List_PageProperties;
            }
        }

        IComponentList m_List_ComponentListProperties;
        IComponentList List_ComponentListProperties
        {
            get
            {
                if (m_List_ComponentListProperties == null)
                {
                    m_List_ComponentListProperties = ComponentList.SelectOne(ComponentListType.ComponentListProperties);
                }
                return m_List_ComponentListProperties;
            }
        }


        IComponentList m_List_SubscribeProperties;
        IComponentList List_SubscribeProperties
        {
            get
            {
                if (m_List_SubscribeProperties == null)
                {
                    m_List_SubscribeProperties = ComponentList.SelectOne(ComponentListType.Subscription);
                }
                return m_List_SubscribeProperties;
            }
        }

        Console Console;
        /// <summary>
        /// Initializes a new instance of the <see cref="UrlBuilder"/> class.
        /// </summary>
        /// <param name="console">The console.</param>
        public UrlBuilder(Console console)
        {
            Console = console;
        }

        /// <summary>
        /// Gets the folder request.
        /// </summary>
        /// <param name="folder">The folder.</param>
        /// <returns></returns>
        public string GetFolderRequest(Folder folder)
        {
            return $"{Console.WimPagePath}?folder={folder.ID}";
        }

        /// <summary>
        /// Gets the folder create request.
        /// </summary>
        /// <returns></returns>
        public string GetFolderCreateRequest()
        {
            return $"{Console.WimPagePath}?list={List_Folders.ID}&folder={Console.CurrentListInstance.wim.CurrentFolder.ID}&item=0&openinframe=2";
        }

        /// <summary>
        /// Gets the folder options request.
        /// </summary>
        /// <returns></returns>
        public string GetFolderOptionsRequest()
        {
            return $"{Console.WimPagePath}?list={List_Folders.ID}&folder={Console.CurrentListInstance.wim.CurrentFolder.ID}&item={Console.CurrentListInstance.wim.CurrentFolder.ID}";
        }

        public string GetFolderCopyRequest(int folder)
        {
            var list = ComponentList.SelectOne(typeof(AppCentre.Data.Implementation.Copy));
            return $"{Console.WimPagePath}?list={list.ID}&type=1&folder={Console.CurrentListInstance.wim.CurrentFolder.ID}&item={folder}";
        }

        public string GetPageCopyRequest(int page)
        {
            var list = ComponentList.SelectOne(typeof(AppCentre.Data.Implementation.Copy));
            return $"{Console.WimPagePath}?list={list.ID}&type=2&folder={Console.CurrentListInstance.wim.CurrentFolder.ID}&item={page}";
        }

        public string GetPageCopyContentRequest(int page)
        {
            var list = ComponentList.SelectOne(typeof(AppCentre.Data.Implementation.CopyContent));
            return $"{Console.WimPagePath}?list={list.ID}&type=2&item={page}";
        }

        public string GetPageHistoryRequest(int page)
        {
            var list = ComponentList.SelectOne(typeof(AppCentre.Data.Implementation.PageHistory));
            return $"{Console.WimPagePath}?list={list.ID}&type=2&pageItem={page}";
        }

        public string GetGalleryOptionsRequest()
        {
            return $"{Console.WimPagePath}?list={List_Folders.ID}&gallery={Console.CurrentListInstance.wim.CurrentFolder.ID}&item={Console.CurrentListInstance.wim.CurrentFolder.ID}";
        }

        /// <summary>
        /// Gets the folder request.
        /// </summary>
        /// <param name="folderID">The folder ID.</param>
        /// <returns></returns>
        public string GetFolderRequest(int folderID, int? channelId = null)
        {
            if (channelId.GetValueOrDefault(0) > 0)
            {
                return $"{Console.GetWimPagePath(channelId.Value)}?folder={folderID}";
            }
            else
            {
                return $"{Console.WimPagePath}?folder={folderID}";
            }
        }

        /// <summary>
        /// Gets the (current) folder request.
        /// </summary>
        /// <returns></returns>
        public string GetFolderRequest()
        {
            return GetFolderRequest(Console.CurrentListInstance.wim.CurrentFolder.ID);
        }

        public static string GetCustomQueryString(HttpContext context, params KeyValue[] keyvalues)
        {
            StringBuilder build = new StringBuilder();

            foreach (string key in context.Request.Query.Keys)
            {
                if (key == "channel" || key == null) continue;
                string keyvalue = context.Request.Query[key];

                if (keyvalues != null)
                {
                    var selection = (from item in keyvalues where item.Key.ToLower() == key.ToLower() select item).ToArray();
                    if (selection.Length == 1)
                    {
                        if (selection[0].RemoveKey)
                        {
                            continue;
                        }

                        build.AppendFormat("{0}{1}={2}"
                            , build.Length == 0 ? "?" : "&"
                            , selection[0].Key, selection[0].Value);
                        selection[0].Done = true;
                    }
                    else
                    {
                        build.AppendFormat("{0}{1}={2}"
                            , build.Length == 0 ? "?" : "&"
                            , key, keyvalue);
                    }
                }
                else
                {
                    build.AppendFormat("{0}{1}={2}"
                        , build.Length == 0 ? "?" : "&"
                        , key, keyvalue);
                }
            }


            if (keyvalues != null)
            {
                var remaining = (from item in keyvalues where !item.Done && !item.RemoveKey select item);
                foreach (KeyValue kv in remaining)
                {
                    build.AppendFormat("{0}{1}={2}"
                        , build.Length == 0 ? "?" : "&"
                        , kv.Key, kv.Value);
                }
            }
            return build.ToString();
        }

        public string GetUrl(params KeyValue[] keyvalues)
        {
            if (keyvalues != null && keyvalues.Any())
            {
                var listkey = keyvalues.FirstOrDefault(x => x.Key.Equals("list", StringComparison.CurrentCultureIgnoreCase));
                if (listkey != null && !listkey.RemoveKey && Utils.IsNumeric(listkey.Value, out int listid))
                {
                    // remote the key if applied as the url takes over.
                    listkey.RemoveKey = true;

                    if (!listid.Equals(Console.CurrentList.ID))
                    {
                        return GetUrl(listid, keyvalues);
                    }
                }
            }

            string querystring = GetCustomQueryString(Console.Context, keyvalues);
            return string.Concat(Console.UrlBuild.GetListRequest(Console.CurrentList), querystring);
        }

        public string GetUrl(IComponentList componentlist, params KeyValue[] keyvalues)
        {
            string querystring = GetCustomQueryString(Console.Context, keyvalues);
            return string.Concat(Console.UrlBuild.GetListRequest(componentlist), querystring);
        }

        public string GetUrl(int componentlistId, params KeyValue[] keyvalues)
        {
            string querystring = GetCustomQueryString(Console.Context, keyvalues);
            return string.Concat(Console.UrlBuild.GetListRequest(componentlistId), querystring);
        }

        public string GetUrl(Type componentlist, params KeyValue[] keyvalues)
        {
            string querystring = GetCustomQueryString(Console.Context, keyvalues);
            return string.Concat(Console.UrlBuild.GetListRequest(componentlist), querystring);
        }

        /// <summary>
        /// Gets the list new record request.
        /// </summary>
        /// <returns></returns>
        public string GetListNewRecordRequest()
        {
            string newItemRequest = string.Concat(Console.CurrentListInstance.wim.SearchResultItemPassthroughParameter, "=0");
            if (!newItemRequest.Contains("?item=0") && !newItemRequest.Contains("&item=0"))
            {
                if (newItemRequest.Contains("?"))
                {
                    return string.Concat(newItemRequest, "&item=0");
                }
                else
                {
                    return string.Concat(newItemRequest, "?item=0");
                }
            }

            if (!string.IsNullOrEmpty(Console.Request.Query["openinframe"]) && !newItemRequest.Contains("openinframe"))
            {
                newItemRequest += string.Concat("&openinframe=", Console.Request.Query["openinframe"]);
            }

            if (Console.CurrentList.Option_LayerResult && !newItemRequest.Contains("openinframe"))
            {
                newItemRequest += "&openinframe=2";
            }

            if (!string.IsNullOrEmpty(Console.Request.Query["referid"]) && !newItemRequest.Contains("referid"))
            {
                newItemRequest += string.Concat("&referid=", Console.Request.Query["referid"]);
            }

            return newItemRequest;
        }

        /// <summary>
        /// Gets the list properties request.
        /// </summary>
        /// <returns></returns>
        public string GetListPropertiesRequest()
        {
            return $"{Console.WimPagePath}?list={List_ComponentListProperties.ID}&folder={Console.CurrentListInstance.wim.CurrentFolder.ID}&base={Console.CurrentList.ID}&item={Console.CurrentList.ID}";
        }


        /// <summary>
        /// Gets the list request.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="itemID">The item ID.</param>
        /// <returns></returns>
        public string GetListRequest(int listID, int? itemID = null, int? channelId = null)
        {
            var list = ComponentList.SelectOne(listID);
            return GetListRequest(list, itemID, channelId);
        }

        public string GetListRequest(Type listType, int? itemID = null, int? channelId = null)
        {
            var list = ComponentList.SelectOne(listType);
            return GetListRequest(list, itemID, channelId);
        }
        /// <summary>
        /// Gets the list request.
        /// </summary>
        public string GetListRequest(IComponentList list, int? itemID = null, int? channelId = null)
        {
            var path = string.Empty;
            if (list != null)
            {
                var folder = Folder.SelectOneChild(list.FolderID.GetValueOrDefault(), channelId ?? Console.ChannelIndentifier);
                path = list.Name;

                if (folder != null && !folder.IsNewInstance)
                {
                    path = $"{folder.CompletePath}{path}";
                }
                else
                {
                    path = $"/{path}";
                }
            }
            else if (channelId.GetValueOrDefault(0) > 0)
            {
                return Console.GetWimPagePath(channelId);
            }
            else
            {
                return Console.WimPagePath;
            }


            if (channelId.GetValueOrDefault(0) > 0)
            {
                path = string.Concat(Console.GetWimPagePath(channelId), Utils.ToUrl(path));
            }
            else
            {
                path = string.Concat(Console.WimPagePath, Utils.ToUrl(path));
            }

            if (itemID.HasValue)
            {
                path = string.Concat(path, "?item=", itemID);
            }

            if (path.StartsWith(@"\\", StringComparison.InvariantCultureIgnoreCase) || path.StartsWith(@"//", StringComparison.InvariantCultureIgnoreCase))
            {
                path = path.Replace(@"\\", @"\", StringComparison.InvariantCultureIgnoreCase);
                path = path.Replace(@"//", @"/", StringComparison.InvariantCultureIgnoreCase);
            }

            return path;
        }

        private async Task<string> GetHomepageFromMenuAsync(int? channelId = null)
        {
            string path = "";

            var roleMenus = await Menu.SelectAllAsync().ConfigureAwait(false);
            IMenu roleMenu;

            if (channelId.GetValueOrDefault(0) > 0 && roleMenus.Any(x => x.SiteID == channelId.Value && x.IsActive == true && x.RoleID == Console.CurrentApplicationUser.RoleID))
            {
                roleMenu = roleMenus.FirstOrDefault(x => x.SiteID == channelId.Value && x.IsActive == true && x.RoleID == Console.CurrentApplicationUser.RoleID);
            }
            else
            {
                roleMenu = roleMenus.FirstOrDefault(x => x.IsActive == true && x.RoleID == Console.CurrentApplicationUser.RoleID);
            }

            if (roleMenu?.ID > 0)
            {
                var items = await MenuItem.SelectAllAsync(roleMenu.ID).ConfigureAwait(false);
                var homepage = items?.FirstOrDefault(x => x.Position == 0);
                if (homepage?.ID > 0)
                {
                    switch (homepage.TypeID)
                    {
                        default: { path = Console.WimPagePath; } break;
                        case 1: { path = GetListRequest(homepage.ItemID, null, channelId); } break;
                        case 2: { path = GetFolderRequest(homepage.ItemID, channelId); } break;
                        case 3: { path = GetPageRequest(homepage.ItemID, channelId); } break;
                        case 5: { path = GetGalleryRequest(homepage.ItemID, channelId); } break;
                        case 6: 
                            {
                                var hpId = (await Site.SelectOneAsync(homepage.ItemID).ConfigureAwait(false)).HomepageID;
                                if (hpId.GetValueOrDefault(0) > 0)
                                {
                                    path = GetPageRequest(hpId.Value, channelId);
                                }
                            } break;
                        case 7: 
                            {
                                if (Enum.TryParse(homepage.ItemID.ToString(), out FolderType type))
                                {
                                    path = GetSectionRequest(type, channelId);
                                }
                                
                            } break;
                        case 8: { path = GetFolderRequest(homepage.ItemID, channelId); } break;
                    }
                }
            }

            if (string.IsNullOrWhiteSpace(path))
            {
                path = Console.WimPagePath;
            }

            return path;
        }

        /// <summary>
        /// Gets the home request.
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetHomeRequestAsync(int? channelId = null)
        {
            return await GetHomepageFromMenuAsync(channelId).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the page request.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <returns></returns>
        public string GetPageRequest(Page page, int? channelId = null)
        {
            return GetPageRequest(page.ID, channelId);
        }

        /// <summary>
        /// Gets the page request.
        /// </summary>
        /// <param name="pageId">The page ID.</param>
        /// <returns></returns>
        public string GetPageRequest(int pageId, int? channelId = null)
        {
            if (channelId.GetValueOrDefault(0) > 0)
            {
                return string.Concat(Console.GetWimPagePath(channelId), "?page=", pageId);
            }
            else
            {
                return string.Concat(Console.WimPagePath, "?page=", pageId);
            }
        }

        /// <summary>
        /// Gets the section request.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public string GetSectionRequest(FolderType type, int? channelId = null)
        {
            var baseUrl = channelId.GetValueOrDefault(0) > 0 ? Console.GetWimPagePath(channelId) : Console.WimPagePath;

            switch (type)
            {
                case FolderType.Page: return $"{baseUrl}?top={1}";
                case FolderType.List: return $"{baseUrl}?top={2}";
                case FolderType.Gallery: return $"{baseUrl}?top={3}";
                case FolderType.Administration: return $"{baseUrl}?top={4}";
            }

            return baseUrl;
        }


        /// <summary>
        /// Gets the gallery request.
        /// </summary>
        /// <param name="gallery">The gallery.</param>
        /// <returns></returns>
        public string GetGalleryRequest(Gallery gallery, int? channelId = null)
        {
            return GetGalleryRequest(gallery.ID, channelId);
        }

        /// <summary>
        /// Gets the gallery request.
        /// </summary>
        /// <param name="galleryID">The gallery ID.</param>
        /// <returns></returns>
        public string GetGalleryRequest(int galleryID, int? channelId = null)
        {
            if (channelId.GetValueOrDefault(0) > 0)
            {
                return string.Concat(Console.GetWimPagePath(channelId), "?gallery=", galleryID);
            }
            else
            {
                return string.Concat(Console.WimPagePath, "?gallery=", galleryID);
            }
        }

        /// <summary>
        /// Gets the gallery new asset request.
        /// </summary>
        /// <returns></returns>
        public string GetGalleryNewAssetRequest()
        {
            return string.Concat(Console.WimPagePath, "?gallery=", Console.CurrentListInstance.wim.CurrentFolder.ID, "&asset=0");
        }

        /// <summary>
        /// Gets the gallery new asset request in layer.
        /// </summary>
        /// <returns></returns>
        public string GetGalleryNewAssetRequestInLayer()
        {
            //gallery=53&openinframe=1&type=2&referid=_Project_Contract&item=0&list=6
            return string.Concat(Console.WimPagePath, "?gallery=", Console.CurrentListInstance.wim.CurrentFolder.ID, "&openinframe=1&type=2&referid=", Console.Request.Query["referid"], "&asset=0");
        }

        /// <summary>
        /// Gets the new gallery request.
        /// </summary>
        /// <returns></returns>
        public string GetNewGalleryRequest()
        {
            return string.Concat(Console.WimPagePath, "?", "list=", List_Folders.ID, "&gallery=", Console.CurrentListInstance.wim.CurrentFolder.ID, "&item=0");
        }

        /// <summary>
        /// Gets the new page request.
        /// </summary>
        /// <returns></returns>
        public string GetNewPageRequest()
        {
            return string.Concat(Console.WimPagePath, "?", "list=", List_PageProperties.ID, "&folder=", Console.CurrentListInstance.wim.CurrentFolder.ID, "&item=0");
        }


        /// <summary>
        /// Gets the new list request.
        /// </summary>
        /// <returns></returns>
        public string GetNewListRequest()
        {
            string url = string.Concat(Console.WimPagePath, "?", "list=", List_ComponentListProperties.ID, "&folder=", Console.CurrentListInstance.wim.CurrentFolder.ID, "&item=0");
            if (!string.IsNullOrEmpty(Console.Request.Query["openinframe"]))
            {
                url += string.Concat("&openinframe=", Console.Request.Query["openinframe"]);
            }
            return url;
        }

        /// <summary>
        /// Gets the subscribe request.
        /// </summary>
        /// <returns></returns>
        public string GetSubscribeRequest()
        {
            return string.Concat(Console.WimPagePath, "?", "list=", List_SubscribeProperties.ID, "&folder=", Console.CurrentListInstance.wim.CurrentFolder.ID, "&base=", Console.CurrentList.ID, "&item=0");
        }

        /// <summary>
        /// Gets the page properties request.
        /// </summary>
        /// <returns></returns>
        public string GetPagePropertiesRequest()
        {
            return string.Concat(Console.WimPagePath, "?", "list=", List_PageProperties.ID, "&folder=", Console.CurrentListInstance.wim.CurrentFolder.ID, "&item=", Console.Item.Value);
        }

        //
    }
}
