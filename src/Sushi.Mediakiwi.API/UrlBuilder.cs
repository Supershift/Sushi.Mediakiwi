using Sushi.Mediakiwi.Data;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.API
{
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

        UrlResolver Resolver;
        /// <summary>
        /// Initializes a new instance of the <see cref="UrlBuilder"/> class.
        /// </summary>
        /// <param name="resolver">The Url builder.</param>
        public UrlBuilder(UrlResolver resolver)
        {
            Resolver = resolver;
        }

        /// <summary>
        /// Gets the folder request.
        /// </summary>
        /// <param name="folder">The folder.</param>
        /// <returns></returns>
        public string GetFolderRequest(Folder folder)
        {
            return $"{Resolver.WimPagePath}?folder={folder.ID}";
        }

        /// <summary>
        /// Gets the folder create request.
        /// </summary>
        /// <returns></returns>
        public string GetFolderCreateRequest()
        {
            return $"{Resolver.WimPagePath}?list={List_Folders.ID}&folder={Resolver.ListInstance.wim.CurrentFolder.ID}&item=0&openinframe=2";
        }

        /// <summary>
        /// Gets the folder options request.
        /// </summary>
        /// <returns></returns>
        public string GetFolderOptionsRequest()
        {
            return $"{Resolver.WimPagePath}?list={List_Folders.ID}&folder={Resolver.ListInstance.wim.CurrentFolder.ID}&item={Resolver.ListInstance.wim.CurrentFolder.ID}";
        }

        public string GetFolderCopyRequest(int folder)
        {
            var list = ComponentList.SelectOne(typeof(AppCentre.Data.Implementation.Copy));
            return $"{Resolver.WimPagePath}?list={list.ID}&type=1&folder={Resolver.ListInstance.wim.CurrentFolder.ID}&item={folder}";
        }

        public string GetPageCopyRequest(int page)
        {
            var list = ComponentList.SelectOne(typeof(AppCentre.Data.Implementation.Copy));
            return $"{Resolver.WimPagePath}?list={list.ID}&type=2&folder={Resolver.ListInstance.wim.CurrentFolder.ID}&item={page}";
        }

        public string GetPageCopyContentRequest(int page)
        {
            var list = ComponentList.SelectOne(typeof(AppCentre.Data.Implementation.CopyContent));
            return $"{Resolver.WimPagePath}?list={list.ID}&type=2&item={page}";
        }

        public string GetPageHistoryRequest(int page)
        {
            var list = ComponentList.SelectOne(typeof(AppCentre.Data.Implementation.PageHistory));
            return $"{Resolver.WimPagePath}?list={list.ID}&type=2&pageItem={page}";
        }

        public string GetGalleryOptionsRequest()
        {
            return $"{Resolver.WimPagePath}?list={List_Folders.ID}&gallery={Resolver.ListInstance.wim.CurrentFolder.ID}&item={Resolver.ListInstance.wim.CurrentFolder.ID}";
        }

        /// <summary>
        /// Gets the folder request.
        /// </summary>
        /// <param name="folderID">The folder ID.</param>
        /// <returns></returns>
        public string GetFolderRequest(int folderID)
        {
            return $"{Resolver.WimPagePath}?folder={folderID}";
        }

        /// <summary>
        /// Gets the (current) folder request.
        /// </summary>
        /// <returns></returns>
        public string GetFolderRequest()
        {
            return GetFolderRequest(Resolver.ListInstance.wim.CurrentFolder.ID);
        }

        public string GetCustomQueryString(params KeyValue[] keyvalues)
        {
            Microsoft.AspNetCore.Http.Extensions.QueryBuilder builder = new Microsoft.AspNetCore.Http.Extensions.QueryBuilder();

            foreach (string key in Resolver.Query.Keys)
            {
                if (string.IsNullOrWhiteSpace(key) || key == "channel")
                {
                    continue;
                }

                string keyvalue = Resolver.Query[key];

                if (keyvalues != null)
                {
                    var selection = (from item in keyvalues where item.Key.ToLower() == key.ToLower() select item).ToArray();
                    if (selection.Length == 1)
                    {
                        if (selection[0].RemoveKey)
                        {
                            continue;
                        }
                        builder.Add(selection[0].Key, selection[0].Value.ToString());
                    }
                    else
                    {
                        builder.Add(key, keyvalue);
                    }
                }
                else
                {
                    builder.Add(key, keyvalue);
                }
            }

            if (keyvalues != null)
            {
                var remaining = (from item in keyvalues where builder.Any(x => x.Key.Equals(item.Key)) == false && !item.RemoveKey select item);
                foreach (KeyValue kv in remaining)
                {
                    builder.Add(kv.Key, kv.Value.ToString());
                }
            }

            return builder.ToString();
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

                    if (!listid.Equals(Resolver.ListID.GetValueOrDefault(0)))
                    {
                        return GetUrl(listid, keyvalues);
                    }
                }
            }

            string querystring = GetCustomQueryString(keyvalues);
            return $"{GetListRequest(Resolver.ListID.GetValueOrDefault(0))}{querystring}";
        }

        public string GetUrl(IComponentList componentlist, params KeyValue[] keyvalues)
        {
            string querystring = GetCustomQueryString(keyvalues);
            return $"{GetListRequest(componentlist)}{querystring}";
        }

        public string GetUrl(int componentlistId, params KeyValue[] keyvalues)
        {
            string querystring = GetCustomQueryString(keyvalues);
            return $"{GetListRequest(componentlistId)}{querystring}";
        }

        public string GetUrl(Type componentlist, params KeyValue[] keyvalues)
        {
            string querystring = GetCustomQueryString(keyvalues);
            return $"{GetListRequest(componentlist)}{querystring}";
        }

        /// <summary>
        /// Gets the list new record request.
        /// </summary>
        /// <returns></returns>
        public string GetListNewRecordRequest()
        {
            string newItemRequest = string.Concat(Resolver.ListInstance.wim.SearchResultItemPassthroughParameter, "=0");
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

            if (Resolver.OpenInFrame.GetValueOrDefault(0) > 0 && !newItemRequest.Contains("openinframe"))
            {
                newItemRequest += string.Concat("&openinframe=", Resolver.OpenInFrame.Value);
            }

            if (Resolver.List.Option_LayerResult && !newItemRequest.Contains("openinframe"))
            {
                newItemRequest += "&openinframe=2";
            }

            if (string.IsNullOrWhiteSpace(Resolver.ReferID) == false && !newItemRequest.Contains("referid"))
            {
                newItemRequest += string.Concat("&referid=", Resolver.ReferID);
            }

            return newItemRequest;
        }

        /// <summary>
        /// Gets the list properties request.
        /// </summary>
        /// <returns></returns>
        public string GetListPropertiesRequest()
        {
            return $"{Resolver.WimPagePath}?list={List_ComponentListProperties.ID}&folder={Resolver.ListInstance.wim.CurrentFolder.ID}&base={Resolver.List.ID}&item={Resolver.List.ID}";
        }


        /// <summary>
        /// Gets the list request.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="itemID">The item ID.</param>
        /// <returns></returns>
        public string GetListRequest(int listID, int? itemID = null)
        {
            var list = ComponentList.SelectOne(listID);
            return GetListRequest(list, itemID);
        }

        public string GetListRequest(Type listType, int? itemID = null)
        {
            var list = ComponentList.SelectOne(listType);
            return GetListRequest(list, itemID);
        }

        /// <summary>
        /// Gets the list request.
        /// </summary>
        public string GetListRequest(IComponentList list, int? itemID = null)
        {
            var path = string.Empty;
            if (list != null)
            {
                var folder = Folder.SelectOneChild(list.FolderID.GetValueOrDefault(), Resolver.SiteID.Value);
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
            else
            {
                return Resolver.WimPagePath;
            }

            if (itemID.HasValue)
            {
                return string.Concat(Resolver.WimPagePath, Utils.ToUrl(path), "?item=", itemID);
            }

            return string.Concat(Resolver.WimPagePath, Utils.ToUrl(path));
        }

        /// <summary>
        /// Gets the list request.
        /// </summary>
        /// <param name="listID">The list ID.</param>
        /// <param name="itemID">The item ID.</param>
        /// <returns></returns>
        public async Task<string> GetListRequestAsync(int listID, int? itemID = null)
        {
            var list = await ComponentList.SelectOneAsync(listID).ConfigureAwait(false);
            return await GetListRequestAsync(list, itemID).ConfigureAwait(false);
        }

        public async Task<string> GetListRequestAsync(Type listType, int? itemID = null)
        {
            var list = await ComponentList.SelectOneAsync(listType).ConfigureAwait(false);
            return await GetListRequestAsync(list, itemID).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the list request.
        /// </summary>
        public async Task<string> GetListRequestAsync(IComponentList list, int? itemID = null)
        {
            var path = string.Empty;
            if (list != null)
            {
                var folder = await Folder.SelectOneChildAsync(list.FolderID.GetValueOrDefault(), Resolver.SiteID.Value).ConfigureAwait(false);
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
            else
            {
                return Resolver.WimPagePath;
            }

            if (itemID.HasValue)
            {
                return $"{Resolver.WimPagePath}{Utils.ToUrl(path)}?item={itemID}";
            }

            return $"{Resolver.WimPagePath}{Utils.ToUrl(path)}";
        }

        /// <summary>
        /// Gets the home request.
        /// </summary>
        /// <returns></returns>
        public string GetHomeRequest(int? channelId = null)
        {
            if (channelId.HasValue)
            {
                var channel = Site.SelectOne(channelId.Value);
                if (channel?.ID > 0)
                {
                    return Resolver.AddApplicationPath(string.Concat(CommonConfiguration.PORTAL_PATH, "/", Utils.ToUrl(channel.Name)));
                }
            }

            return Resolver.WimPagePath;
        }

        /// <summary>
        /// Gets the home request.
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetHomeRequestAsync(int? channelId = null)
        {
            if (channelId.HasValue)
            {
                var channel = await Site.SelectOneAsync(channelId.Value).ConfigureAwait(false);
                if (channel?.ID > 0)
                {
                    return Resolver.AddApplicationPath(string.Concat(CommonConfiguration.PORTAL_PATH, "/", Utils.ToUrl(channel.Name)));
                }
            }

            return Resolver.WimPagePath;
        }

        /// <summary>
        /// Gets the page request.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <returns></returns>
        public string GetPageRequest(Page page)
        {
            return $"{Resolver.WimPagePath}?page={page.ID}";
        }

        /// <summary>
        /// Gets the section request.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public string GetSectionRequest(FolderType type)
        {
            switch (type)
            {
                case FolderType.Page: return $"{Resolver.WimPagePath}?top=1";
                case FolderType.List: return $"{Resolver.WimPagePath}?top=2";
                case FolderType.Gallery: return $"{Resolver.WimPagePath}?top=3";
                case FolderType.Administration: return $"{Resolver.WimPagePath}?top=4";
            }

            return Resolver.WimPagePath;
        }


        /// <summary>
        /// Gets the gallery request.
        /// </summary>
        /// <param name="gallery">The gallery.</param>
        /// <returns></returns>
        public string GetGalleryRequest(Gallery gallery)
        {
            return GetGalleryRequest(gallery.ID);
        }

        /// <summary>
        /// Gets the gallery request.
        /// </summary>
        /// <param name="galleryID">The gallery ID.</param>
        /// <returns></returns>
        public string GetGalleryRequest(int galleryID)
        {
            return $"{Resolver.WimPagePath}?gallery={galleryID}";
        }

        /// <summary>
        /// Gets the gallery new asset request.
        /// </summary>
        /// <returns></returns>
        public string GetGalleryNewAssetRequest()
        {
            return $"{Resolver.WimPagePath}?gallery={Resolver.ListInstance.wim.CurrentFolder.ID}&asset=0";
        }

        /// <summary>
        /// Gets the gallery new asset request in layer.
        /// </summary>
        /// <returns></returns>
        public string GetGalleryNewAssetRequestInLayer()
        {
            return $"{Resolver.WimPagePath}?gallery={Resolver.ListInstance.wim.CurrentFolder.ID}&openinframe=1&type=2&referid={Resolver.ReferID}&asset=0";
        }

        /// <summary>
        /// Gets the new gallery request.
        /// </summary>
        /// <returns></returns>
        public string GetNewGalleryRequest()
        {
            return $"{Resolver.WimPagePath}?list={List_Folders.ID}&gallery={Resolver.ListInstance.wim.CurrentFolder.ID}&item=0";
        }

        /// <summary>
        /// Gets the new page request.
        /// </summary>
        /// <returns></returns>
        public string GetNewPageRequest()
        {
            return $"{Resolver.WimPagePath}?list={List_PageProperties.ID}&folder={Resolver.ListInstance.wim.CurrentFolder.ID}&item=0";
        }


        /// <summary>
        /// Gets the new list request.
        /// </summary>
        /// <returns></returns>
        public string GetNewListRequest()
        {
            string url = $"{Resolver.WimPagePath}?list={List_ComponentListProperties.ID}&folder={Resolver.ListInstance.wim.CurrentFolder.ID}&item=0";
            if (Resolver.OpenInFrame.GetValueOrDefault(0) > 0)
            {
                url += $"&openinframe={Resolver.OpenInFrame.Value}";
            }
            return url;
        }

        /// <summary>
        /// Gets the subscribe request.
        /// </summary>
        /// <returns></returns>
        public string GetSubscribeRequest()
        {
            return $"{Resolver.WimPagePath}?list={List_SubscribeProperties.ID}&folder={Resolver.ListInstance.wim.CurrentFolder.ID}&base={Resolver.List.ID}&item=0";
        }

        /// <summary>
        /// Gets the page properties request.
        /// </summary>
        /// <returns></returns>
        public string GetPagePropertiesRequest()
        {
            return $"{Resolver.WimPagePath}?list={List_PageProperties.ID}&folder={Resolver.ListInstance.wim.CurrentFolder.ID}&item={Resolver.ItemID.Value}";
        }

        //
    }
}
