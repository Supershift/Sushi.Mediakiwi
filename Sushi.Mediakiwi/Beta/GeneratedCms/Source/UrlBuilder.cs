using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;
using Sushi.Mediakiwi.Data;
using Sushi.Mediakiwi.Framework;

namespace Sushi.Mediakiwi.Beta.GeneratedCms.Source
{
    /// <summary>
    /// 
    /// </summary>
    public class UrlBuilder
    {
        Data.IComponentList m_List_Folders;
        Data.IComponentList List_Folders 
        {
            get
            {
                if (m_List_Folders == null)
                    m_List_Folders = Data.ComponentList.SelectOne(Sushi.Mediakiwi.Data.ComponentListType.Folders);
                return m_List_Folders;
            }
        }

        Data.IComponentList m_List_PageProperties;
        Data.IComponentList List_PageProperties
        {
            get
            {
                if (m_List_PageProperties == null)
                    m_List_PageProperties = Data.ComponentList.SelectOne(Sushi.Mediakiwi.Data.ComponentListType.PageProperties);
                return m_List_PageProperties;
            }
        }

        Data.IComponentList m_List_ComponentListProperties;
        Data.IComponentList List_ComponentListProperties
        {
            get
            {
                if (m_List_ComponentListProperties == null)
                    m_List_ComponentListProperties = Data.ComponentList.SelectOne(Sushi.Mediakiwi.Data.ComponentListType.ComponentListProperties);
                return m_List_ComponentListProperties;
            }
        }


        Data.IComponentList m_List_SubscribeProperties;
        Data.IComponentList List_SubscribeProperties
        {
            get
            {
                if (m_List_SubscribeProperties == null)
                    m_List_SubscribeProperties = Data.ComponentList.SelectOne(Sushi.Mediakiwi.Data.ComponentListType.Subscription);
                return m_List_SubscribeProperties;
            }
        }

        Sushi.Mediakiwi.Beta.GeneratedCms.Console Console;
        /// <summary>
        /// Initializes a new instance of the <see cref="UrlBuilder"/> class.
        /// </summary>
        /// <param name="console">The console.</param>
        public UrlBuilder(Sushi.Mediakiwi.Beta.GeneratedCms.Console console)
        {
            this.Console = console;
        }

        /// <summary>
        /// Gets the folder request.
        /// </summary>
        /// <param name="folder">The folder.</param>
        /// <returns></returns>
        public string GetFolderRequest(Data.Folder folder)
        {
            return string.Concat(Console.WimPagePath, "?folder=", folder.ID);
        }

        /// <summary>
        /// Gets the folder create request.
        /// </summary>
        /// <returns></returns>
        public string GetFolderCreateRequest()
        {
                return string.Concat(Console.WimPagePath, "?", "list=", List_Folders.ID, "&folder=", Console.CurrentListInstance.wim.CurrentFolder.ID, "&item=0");
        }

        /// <summary>
        /// Gets the folder options request.
        /// </summary>
        /// <returns></returns>
        public string GetFolderOptionsRequest()
        {
            return string.Concat(Console.WimPagePath, "?", "list=", List_Folders.ID, "&folder=", Console.CurrentListInstance.wim.CurrentFolder.ID, "&item=", Console.CurrentListInstance.wim.CurrentFolder.ID);
        }

        public string GetFolderCopyRequest(int folder)
        {
            var list = Sushi.Mediakiwi.Data.ComponentList.SelectOne(typeof(Sushi.Mediakiwi.AppCentre.Data.Implementation.Copy));
            return string.Concat(Console.WimPagePath, "?", "list=", list.ID, "&type=1&folder=", Console.CurrentListInstance.wim.CurrentFolder.ID, "&item=", folder);
        }

        public string GetPageCopyRequest(int page)
        {
            var list = Sushi.Mediakiwi.Data.ComponentList.SelectOne(typeof(Sushi.Mediakiwi.AppCentre.Data.Implementation.Copy));
            return string.Concat(Console.WimPagePath, "?", "list=", list.ID, "&type=2&folder=", Console.CurrentListInstance.wim.CurrentFolder.ID, "&item=", page);
        }

        public string GetPageCopyContentRequest(int page)
        {
            var list = Sushi.Mediakiwi.Data.ComponentList.SelectOne(typeof(Sushi.Mediakiwi.AppCentre.Data.Implementation.CopyContent));
            return string.Concat(Console.WimPagePath, "?", "list=", list.ID, "&type=2&item=", page);
        }

        public string GetPageHistoryRequest(int page)
        {
            var list = Sushi.Mediakiwi.Data.ComponentList.SelectOne(typeof(Sushi.Mediakiwi.AppCentre.Data.Implementation.PageHistory));
            return string.Concat(Console.WimPagePath, "?", "list=", list.ID, "&type=2&pageItem=", page);
        }

        public string GetGalleryOptionsRequest()
        {
            return string.Concat(Console.WimPagePath, "?", "list=", List_Folders.ID, "&gallery=", Console.CurrentListInstance.wim.CurrentFolder.ID, "&item=", Console.CurrentListInstance.wim.CurrentFolder.ID);
        }

        /// <summary>
        /// Gets the folder request.
        /// </summary>
        /// <param name="folderID">The folder ID.</param>
        /// <returns></returns>
        public string GetFolderRequest(int folderID)
        {
            return string.Concat(Console.WimPagePath, "?folder=", folderID);
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

            foreach (string key in context.Request.Query.Keys )
            {
                if (key == "channel" || key == null) continue;
                string keyvalue = context.Request.Query[key];

                if (keyvalues != null)
                {
                    var selection = (from item in keyvalues where item.Key.ToLower() == key.ToLower() select item).ToArray();
                    if (selection.Length == 1)
                    {
                        if (selection[0].RemoveKey)
                            continue;

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
                var listkey = keyvalues.Where(x => x.Key.Equals("list", StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
                if (listkey != null && !listkey.RemoveKey)
                {
                    if (Utils.IsNumeric(listkey.Value, out int listid))
                    {
                        if (!listid.Equals(Console.CurrentList.ID))
                        {
                            return GetUrl(listid, keyvalues);
                        }
                    }
                }
            }

            string querystring = GetCustomQueryString(Console.Context, keyvalues);
            return string.Concat(Console.UrlBuild.GetListRequest(Console.CurrentList), querystring);
        }

        public string GetUrl(Data.IComponentList componentlist, params KeyValue[] keyvalues)
        {
            string querystring = GetCustomQueryString(Console.Context, keyvalues);
            return string.Concat(Console.UrlBuild.GetListRequest(componentlist), querystring);
        }

        public string GetUrl(int componentlistId, params KeyValue[] keyvalues)
        {
            string querystring = GetCustomQueryString(Console.Context, keyvalues);
            return string.Concat(Console.UrlBuild.GetListRequest(componentlistId), querystring);
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
                newItemRequest += string.Concat("&openinframe=", Console.Request.Query["openinframe"]);

            if (Console.CurrentList.Option_LayerResult && !newItemRequest.Contains("openinframe"))
                newItemRequest += "&openinframe=2";

            if (!string.IsNullOrEmpty(Console.Request.Query["referid"]) && !newItemRequest.Contains("referid"))
                newItemRequest += string.Concat("&referid=", Console.Request.Query["referid"]);

            return newItemRequest;
        }

        /// <summary>
        /// Gets the list properties request.
        /// </summary>
        /// <returns></returns>
        public string GetListPropertiesRequest()
        {
            return string.Concat(Console.WimPagePath, "?", "list=", List_ComponentListProperties.ID, "&folder=", Console.CurrentListInstance.wim.CurrentFolder.ID, "&base=", Console.CurrentList.ID, "&item=", Console.CurrentList.ID);
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

        /// <summary>
        /// Gets the list request.
        /// </summary>
        public string GetListRequest(IComponentList list, int? itemID = null, int? channelId = null)
        {
            if (channelId == null)
                channelId = Console.ChannelIndentifier;

            var folder = Data.Folder.SelectOneChild(list.FolderID.GetValueOrDefault(), Console.ChannelIndentifier);
            var path = list.Name;

            if (folder != null && !folder.IsNewInstance)
            {
                path = $"{folder.CompletePath}{path}";
            }
            else
            {
                path = $"/{path}";
            }


            if (itemID.HasValue)
                return string.Concat(Console.WimPagePath, Utils.ToUrl(path), "?item=", itemID);

            return string.Concat(Console.WimPagePath, Utils.ToUrl(path));
        }

        /// <summary>
        /// Gets the home request.
        /// </summary>
        /// <returns></returns>
        public string GetHomeRequest()
        {
            return string.Concat(Console.WimPagePath);
        }

        /// <summary>
        /// Gets the page request.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <returns></returns>
        public string GetPageRequest(Data.Page page)
        {
            return string.Concat(Console.WimPagePath, "?page=", page.ID);
        }

        /// <summary>
        /// Gets the section request.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public string GetSectionRequest(Data.FolderType type)
        {
            switch (type)
            {
                case Sushi.Mediakiwi.Data.FolderType.Page: return string.Concat(Console.WimPagePath, "?top=", 1);
                case Sushi.Mediakiwi.Data.FolderType.List: return string.Concat(Console.WimPagePath, "?top=", 2);
                case Sushi.Mediakiwi.Data.FolderType.Gallery: return string.Concat(Console.WimPagePath, "?top=", 3);
                case Sushi.Mediakiwi.Data.FolderType.Administration: return string.Concat(Console.WimPagePath, "?top=", 4);
            }
            return string.Concat(Console.WimPagePath);
        }


        /// <summary>
        /// Gets the gallery request.
        /// </summary>
        /// <param name="gallery">The gallery.</param>
        /// <returns></returns>
        public string GetGalleryRequest(Data.Gallery gallery)
        {
            return string.Concat(Console.WimPagePath, "?gallery=", gallery.ID);
        }

        /// <summary>
        /// Gets the gallery request.
        /// </summary>
        /// <param name="galleryID">The gallery ID.</param>
        /// <returns></returns>
        public string GetGalleryRequest(int galleryID)
        {
            return string.Concat(Console.WimPagePath, "?gallery=", galleryID);
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
                url += string.Concat("&openinframe=", Console.Request.Query["openinframe"]);
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
