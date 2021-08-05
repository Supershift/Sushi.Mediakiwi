using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;
using Sushi.Mediakiwi.Connectors;
using Sushi.Mediakiwi.Controllers.Data;
using Sushi.Mediakiwi.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Logic
{
    public class PageLogic
    {
        private IMemoryCache _cache;

        public PageLogic(IMemoryCache memoryCache)
        {
            _cache = memoryCache;
        }

        public async Task<PageContentResponse> GetPageContentAsync(string url, bool flushCache = false, bool isPreview = false, int? pageId = null)
        {
            var response = new PageContentResponse();

            try
            {
                var page = default(Page);
                string cacheKey = "";
                bool flush = flushCache;
                bool ispreview = isPreview;
                var pageMap = default(IPageMapping);

                // When a pageID is supplied, this takes presedence over the URL
                if (pageId.GetValueOrDefault(0) > 0)
                {
                    page = await Page.SelectOneAsync(pageId.Value, !ispreview).ConfigureAwait(false);
                    cacheKey = $"page{pageId.Value}";
                }
                else
                {
                    Uri uri = new Uri(WebUtility.UrlDecode(url), UriKind.Relative);
                    cacheKey = $"{uri}";

                    if ((uri.IsAbsoluteUri && !string.IsNullOrWhiteSpace(uri.Query) && uri.Query.Equals("?flush=me")))
                        flush = true;

                    if ((uri.IsAbsoluteUri && !string.IsNullOrWhiteSpace(uri.Query) && uri.Query.Equals("?preview=1")))
                        ispreview = true;

                    var maps = await PageMapping.SelectAllAsync(0, true).ConfigureAwait(false);
                    foreach (var map in maps)
                    {
                        var mapped = map as PageMapping;
                        if (map.Path.EndsWith("*"))
                        {
                            var path = map.Path.Substring(0, map.Path.Length - 1);

                            // found a match
                            if (url.StartsWith(path, StringComparison.InvariantCulture) && map.Page != null)
                            {
                                page = map.Page;
                                pageMap = map;
                            }
                        }
                        // replace extention, f.e. *.html => .
                        else if (map.Path.StartsWith(".") && mapped != null && !string.IsNullOrWhiteSpace(mapped.Expression))
                        {
                            // found a match
                            if (url.EndsWith(map.Path, StringComparison.InvariantCulture))
                            {
                                var redirection = url.Remove(url.Length - map.Path.Length, map.Path.Length);
                                var newurl = mapped.Expression.Equals(".") ? redirection : string.Concat(redirection, mapped.Expression);

                                // to do, replace with actual content
                                response = await GetPageNotFoundAsync(null).ConfigureAwait(false);
                                response.PageInternalPath = newurl;
                                response.StatusCode = GetStatusCode(map);
                                return response;
                            }
                        }
                        // map one to other
                        else if (mapped != null && map.Path.Equals(url, StringComparison.CurrentCultureIgnoreCase) && !string.IsNullOrWhiteSpace(mapped.Expression))
                        {
                            // to do, replace with actual content
                            response = await GetPageNotFoundAsync(null).ConfigureAwait(false);
                            response.PageInternalPath = mapped.Expression;
                            response.StatusCode = GetStatusCode(map);
                            return response;
                        }
                        // found a match
                        else if (url.Equals(map.Path, StringComparison.InvariantCulture) && map.Page != null)
                        {
                            // found a match
                            page = map.Page;
                            pageMap = map;
                        }
                    }

                    if (page == null || page?.ID.Equals(0) == true)
                    {
                        page = await Page.SelectOneAsync(uri.ToString(), !ispreview).ConfigureAwait(false);
                    }
                }

                if (flush)
                {
                    ClearCache();
                    _cache.Remove(cacheKey);
                }

                // Look for cache key.
                if (!ispreview && _cache.TryGetValue(cacheKey, out response))
                {
                    return response;
                }
                else
                {
                    // Key not in cache, so get data.
                    response = new PageContentResponse();

                    if (page == null || page?.ID == 0)
                    {
                        // Check if this page is the Homepage 
                        if (url == "/" || string.IsNullOrWhiteSpace(url))
                            response = await GetHomePageAsync(null).ConfigureAwait(false);
                        else
                            response = await GetPageNotFoundAsync(null).ConfigureAwait(false);
                    }
                    else
                    {
                        response = await GetPageContentAsync(page, pageMap, ispreview).ConfigureAwait(false);
                    }

                    // Save data in cache.
                    AddToCache(cacheKey, response);
                }

            }
            catch (Exception ex)
            {
                response.Exception = $"{ex.Message} {ex.StackTrace}";
            }

            return response;
        }


        /// <summary>
        /// Gets the content for the 'Homepage' page and returns it as an HttpStatus 200 Code
        /// </summary>
        /// <param name="siteId">The site for which to get this page (or default environment site when none supplied)</param>
        /// <returns></returns>
        private async Task<PageContentResponse> GetHomePageAsync(int? siteId)
        {
            PageContentResponse response = new PageContentResponse();
            int _siteId = (siteId.GetValueOrDefault(0) > 0) ? siteId.Value : 0;
            if (_siteId == 0 && Sushi.Mediakiwi.Data.Environment.Current?.DefaultSiteID.GetValueOrDefault(0) > 0)
                _siteId = Sushi.Mediakiwi.Data.Environment.Current.DefaultSiteID.Value;

            if (_siteId > 0)
            {
                var site = await Site.SelectOneAsync(_siteId).ConfigureAwait(false);
                if (site?.HomepageID.GetValueOrDefault(0) > 0)
                {
                    var page = await Page.SelectOneAsync(site.HomepageID.Value).ConfigureAwait(false);
                    if (page?.ID > 0)
                        response = await GetPageContentAsync(page, null, false).ConfigureAwait(false);
                }
            }
            return response;
        }


        /// <summary>
        /// Gets the content for the 'Page not found' page and returns it as an HttpStatus 404 Code
        /// </summary>
        /// <param name="siteId">The site for which to get this page (or default environment site when none supplied)</param>
        /// <returns></returns>
        private async Task<PageContentResponse> GetPageNotFoundAsync(int? siteId)
        {
            PageContentResponse response = new PageContentResponse();
            int _siteId = (siteId.GetValueOrDefault(0) > 0) ? siteId.Value : 0;
            if (_siteId == 0 && Sushi.Mediakiwi.Data.Environment.Current?.DefaultSiteID.GetValueOrDefault(0) > 0)
                _siteId = Sushi.Mediakiwi.Data.Environment.Current.DefaultSiteID.Value;

            if (_siteId > 0)
            {
                var site = await Site.SelectOneAsync(_siteId).ConfigureAwait(false);
                if (site?.PageNotFoundID.GetValueOrDefault(0) > 0)
                {
                    var page = await Page.SelectOneAsync(site.PageNotFoundID.Value).ConfigureAwait(false);
                    if (page?.ID > 0)
                        response = await GetPageContentAsync(page, null, false).ConfigureAwait(false);
                }
            }

            response.StatusCode = HttpStatusCode.NotFound;
            return response;
        }


        private bool ClearCache()
        {
            if (_resetCacheToken != null && !_resetCacheToken.IsCancellationRequested && _resetCacheToken.Token.CanBeCanceled)
            {
                _resetCacheToken.Cancel();
                _resetCacheToken.Dispose();
            }
            _resetCacheToken = new CancellationTokenSource();
            return true;
        }

        private static CancellationTokenSource _resetCacheToken = new CancellationTokenSource();

        private void AddToCache(string key, object item)
        {
            /* some other code removed for brevity */
            var options = new MemoryCacheEntryOptions().SetPriority(CacheItemPriority.Normal).SetAbsoluteExpiration(DateTimeOffset.UtcNow.AddDays(1));
            options.AddExpirationToken(new CancellationChangeToken(_resetCacheToken.Token));

            _cache.Set(key, item, options);
        }

        private async Task<Dictionary<string, ContentItem>> getMultiFieldContentAsync(HeadlessRequest request, Field inField)
        {
            if (string.IsNullOrWhiteSpace(inField.Value))
                return null;

            Dictionary<string, ContentItem> lst = new Dictionary<string, ContentItem>();

            var mfs = Sushi.Mediakiwi.Data.MultiField.GetDeserialized(inField.Value);
            if (mfs.Length > 0)
            {
                int idx = 0;
                foreach (var item in mfs)
                {
                    idx++;
                    (ContentItem contentItem, bool isFilled) result = await getContentItemFromFieldAsync(request, new Field(item.Property, item.Type, item.Value)).ConfigureAwait(false);
                    if (result.isFilled)
                        lst.Add($"Multifield_{idx}", result.contentItem);
                }
            }

            return lst;
        }

        public static iHeadlessListTemplate CreateInstance(string assemblyName, string className)
        {
            var type = default(Type);
            try
            {
                System.IO.FileInfo nfo = null;

                nfo = new System.IO.FileInfo(
                string.Concat(
                    Assembly.GetCallingAssembly().Location.Replace(string.Concat(Assembly.GetCallingAssembly().GetName().Name, ".dll"), string.Empty)
                    , assemblyName));

                Assembly assem = Assembly.LoadFrom(nfo.FullName);
                type = assem.GetType(className);

                return Activator.CreateInstance(type) as iHeadlessListTemplate;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        // TODO: replace this on the fly with real data
        private Dictionary<string, List<string>> NestedComponentCollection { get; set; } = new Dictionary<string, List<string>>()
        {
            {
                // PARENT :                               CHILDS :
                "C000_NavigationFR", new List<string>() { "C000_MegaMenuColumn", "C000_MegaMenuLink" }
            }
        };

        private async Task<(ContentItem contentItem, bool isFilled)> getContentItemFromFieldAsync(HeadlessRequest request, Field inField)
        {
            var content = new ContentItem();
            bool isFilled = false;

            switch ((ContentType)inField.Type)
            {
                case ContentType.PageSelect:
                    {
                        if (inField?.Page?.ID > 0)
                        {
                            isFilled = true;
                            content.Text = inField.Page?.LinkText;
                            content.Href = ConvertUrl(inField.Page?.InternalPath);
                        }
                    }
                    break;
                case ContentType.Binary_Image:
                    {
                        if (inField?.Image?.ID > 0)
                        {
                            isFilled = true;
                            content.Text = inField.Image?.Description;
                            if (inField?.Image.Width > 0)
                                content.Width = inField?.Image.Width;
                            if (inField?.Image.Height > 0)
                                content.Height = inField?.Image.Height;

                            content.Url = inField.Image?.RemoteLocation;
                            if (!string.IsNullOrWhiteSpace(AzureBlobConnector.AzurePrefix) && !string.IsNullOrWhiteSpace(content.Url))
                            {
                                Uri path = new Uri(inField.Image.RemoteLocation, UriKind.Absolute);
                                content.Url = $"{AzureBlobConnector.AzurePrefix}{path.AbsolutePath}";
                            }
                        }
                    }
                    break;
                case ContentType.Binary_Document:
                    {
                        if (inField?.Asset?.ID > 0)
                        {
                            isFilled = true;
                            content.Text = inField.Asset?.Description;
                            content.Url = inField.Asset?.RemoteLocation;
                            if (!string.IsNullOrWhiteSpace(AzureBlobConnector.AzurePrefix) && !string.IsNullOrWhiteSpace(content.Url))
                            {
                                Uri path = new Uri(inField.Asset.RemoteLocation, UriKind.Absolute);
                                content.Url = $"{AzureBlobConnector.AzurePrefix}{path.AbsolutePath}";
                            }
                        }
                    }
                    break;
                case ContentType.Hyperlink:
                    {
                        if (inField.Link?.ID > 0)
                        {
                            isFilled = true;
                            content.Text = inField.Link?.Text;
                            if (inField.Link != null)
                            {
                                if (inField.Link.IsInternal)
                                {
                                    var pagelink = await Page.SelectOneAsync(inField.Link.PageID.GetValueOrDefault()).ConfigureAwait(false);
                                    content.Href = ConvertUrl(pagelink?.InternalPath);
                                }
                                else
                                    content.Href = inField.Link?.ExternalUrl;
                            }
                        }
                    }
                    break;
                case ContentType.TextField:
                case ContentType.RichText:
                case ContentType.TextArea:
                case ContentType.Choice_Dropdown:
                case ContentType.Choice_Checkbox:
                case ContentType.Sourcecode:
                    {
                        if (!string.IsNullOrWhiteSpace(inField.Value))
                        {
                            isFilled = true;
                            content.Text = Sushi.Mediakiwi.Data.Utility.CleanConcurrentBreaks(inField.Value, true);
                        }
                    }
                    break;
                case ContentType.SubListSelect:
                    {
                        if (!string.IsNullOrWhiteSpace(inField.Value))
                        {
                            var data = SubList.GetDeserialized(inField.Value);
                            if (data != null && data.Items != null && data.Items.Any() && data.List.HasValue)
                            {
                                var list = await ComponentList.SelectOneAsync(data.List.Value).ConfigureAwait(false);
                                if (list != null && !list.IsNewInstance)
                                {
                                    var instance = CreateInstance(list.AssemblyName, list.ClassName);

                                    if (instance != null)
                                    {
                                        List<object> listcollection = new List<object>();

                                        foreach (var item in data.Items)
                                        {
                                            request.Listitem = item;
                                            instance.DoHeadLessFetch(request);
                                            if (request.Result != null)
                                            {
                                                listcollection.Add(request.Result);
                                            }
                                        }

                                        if (listcollection.Any())
                                        {

                                            isFilled = true;
                                            content.Items = listcollection.ToArray();
                                        }
                                    }
                                }
                            }
                        }
                    }
                    break;
                case ContentType.MultiField:
                    {
                        if (!string.IsNullOrWhiteSpace(inField.Value))
                        {
                            isFilled = true;
                            content.MultiFieldContent = await getMultiFieldContentAsync(request, inField);
                        }
                    }
                    break;
            }

            return (content, isFilled);
        }

        private string ConvertUrl(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return string.Empty;
            }


            return path
                .ToLowerInvariant()
                .Replace(" ", "-", StringComparison.InvariantCulture);
        }

        HttpStatusCode GetStatusCode(IPageMapping pageMap)
        {
            var status = HttpStatusCode.OK;

            if (pageMap != null)
            {
                if (pageMap.MappingType == PageMappingType.Redirect302)
                {
                    status = HttpStatusCode.Redirect;
                }
                else if (pageMap.MappingType == PageMappingType.Redirect301)
                {
                    status = HttpStatusCode.MovedPermanently;
                }
                else if (pageMap.MappingType == PageMappingType.NotFound404)
                {
                    status = HttpStatusCode.NotFound;
                }
            }
            return status;
        }

        public async Task<PageContentResponse> GetPageContentAsync(Page page, IPageMapping pageMap, bool ispreview)
        {
            var response = new PageContentResponse();

            //  Page status validation
            var status = GetStatusCode(pageMap);

            if (pageMap != null && status == HttpStatusCode.NotFound)
            {
                return await GetPageNotFoundAsync(page.SiteID).ConfigureAwait(false);
            }

            // validation parse url?
            response.PageID = page.ID;
            response.PageInternalPath = ConvertUrl(page.InternalPath);
            response.PageLocation = page.Template.Location;
            response.StatusCode = status;

            if (pageMap != null && (response.StatusCode == HttpStatusCode.Redirect || response.StatusCode == HttpStatusCode.MovedPermanently))
            {
                return response;
            }

            var pageTitle = default(string);
            if (pageMap == null || pageMap.Title == null)
            {
                pageTitle = page.Title;
            }
            else
            {
                pageTitle = pageMap.Title;
            }

            if (page.Site.DefaultPageTitle != null)
            {
                // validate the presence of a placeholder.
                if (page.Site.DefaultPageTitle.Contains("[title]", StringComparison.InvariantCultureIgnoreCase))
                {
                    pageTitle = page.Site.DefaultPageTitle.Replace("[title]"
                        , string.IsNullOrWhiteSpace(pageTitle)
                            ? string.Empty : pageTitle);
                }
                else if (string.IsNullOrEmpty(pageTitle))
                {
                    pageTitle = page.Site.DefaultPageTitle;
                }
            }

            response.MetaData.PageTitle = pageTitle;

            if (string.IsNullOrWhiteSpace(page.Site.Language) == false)
            {
                response.MetaData.HtmlLang = page.Site.Language.Split('-')[0];
            }

            if (string.IsNullOrWhiteSpace(page.Description) == false || string.IsNullOrWhiteSpace(page.Keywords) == false)
            {
                response.MetaData.MetaTags = new List<ContentMetaTag>();
                if (string.IsNullOrWhiteSpace(page.Description) == false)
                {
                    response.MetaData.MetaTags.Add(new ContentMetaTag("description", page.Description));
                }

                if (string.IsNullOrWhiteSpace(page.Keywords) == false)
                {
                    response.MetaData.MetaTags.Add(new ContentMetaTag("keywords", page.Keywords));
                }
            }

            Component[] components;
            if (ispreview)
            {
                var versions = await ComponentVersion.SelectAllAsync(page.ID).ConfigureAwait(false);
                List<Component> converted = new List<Component>();
                foreach (var version in versions)
                {
                    if (version.IsActive)
                    {
                        converted.Add(version.Convert());
                    }
                }
                components = converted.ToArray();
            }
            else
                components = await Component.SelectAllInheritedAsync(page.ID, false, false).ConfigureAwait(false);

            int sort = 0;
            foreach (var component in components.OrderBy(x => x.SortOrder))
            {
                if (component?.Template?.SourceTag?.Contains(" ") == true)
                    component.Template.SourceTag = component.Template.SourceTag.Replace(" ", "_");

                string slotTarget = "content";
                if (component?.Template?.SourceTag?.Equals("C000_Navigation", StringComparison.InvariantCultureIgnoreCase) == true)
                    slotTarget = "header";

                if (component?.Template?.IsFooter == true)
                    slotTarget = "footer";
                else if (component?.Template?.IsHeader == true)
                    slotTarget = "header";

                sort++;
                var mapped = new ContentComponent()
                {
                    ComponentName = component.Template.SourceTag,
                    IsShared = component.Template.IsShared,
                    Title = component.Template.Name,
                    ComponentID = component.ID,
                    //Location = component.ID == 3 ? "app/components/C000_Navigation.vue" : "app/components/C000_ContentHeader.vue",
                    Slot = slotTarget,
                    SortOrder = component.SortOrder.GetValueOrDefault(0),
                };
                mapped.Content = new Dictionary<string, ContentItem>();

                var request = new HeadlessRequest
                {
                    Page = page,
                    Component = component,
                    IsPreview = ispreview
                };

                if (component?.Content?.Fields?.Length > 0)
                {
                    foreach (var field in component.Content.Fields)
                    {
                        (ContentItem content, bool isFilled) result = await getContentItemFromFieldAsync(request, field).ConfigureAwait(false);

                        if (!mapped.Content.ContainsKey(field.Property) && result.isFilled)
                            mapped.Content.Add(field.Property, result.content);
                    }
                }

                // Check if we have a NestedComponent Collection match for this (child) item
                if (NestedComponentCollection.Any(x => x.Value.Contains(request.Component.Template.SourceTag)))
                {
                    // Find out to which parent this (child) component belongs
                    var nestedItem = NestedComponentCollection.FirstOrDefault(x => x.Value.Contains(request.Component.Template.SourceTag));

                    // Get the parent component in which this child component will be nested
                    var parent = response.Components.FirstOrDefault(x => x.ComponentName.Equals(nestedItem.Key));
                    if (parent != null)
                    {
                        if (parent.Nested == null)
                        {
                            parent.Nested = new List<ContentComponent>();
                        }

                        // Add this component to the Nested Collection of the Parent
                        parent.Nested.Add(mapped);

                        continue;
                    }
                }

                if (request.Component.Template.NestedType.HasValue)
                {
                    var parent = default(ContentComponent);
                    if (request.Component.Template.NestedType == 1)
                    {
                        parent = response.Components.FirstOrDefault(x => x.ComponentName.Equals(request.Component.Template.SourceTag));
                    }
                    else if (request.Component.Template.NestedType == 2)
                    {
                        var previous = response.Components.LastOrDefault();
                        if (parent.ComponentName.Equals(request.Component.Template.SourceTag))
                        {
                            parent = previous;
                        }
                    }
                    if (parent != null)
                    {
                        if (parent.Nested == null)
                        {
                            var clone = parent.Clone();

                            parent.Content = null;
                            parent.Nested = new List<ContentComponent>();
                            // lower the nested 
                            parent.Nested.Add(clone);
                        }
                        parent.Nested.Add(mapped);
                        continue;
                    }
                }

                response.Components.Add(mapped);
            }

            return response;
        }
    }
}
