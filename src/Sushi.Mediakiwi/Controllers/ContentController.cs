using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sushi.Mediakiwi.Data;
using System.Net;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;
using System.Threading;
using System.Reflection;
using Sushi.Mediakiwi.Controllers.Data;
using Sushi.Mediakiwi.Connectors;
using Sushi.Mediakiwi.Extension;

namespace Sushi.Mediakiwi.Controllers
{
    public class PageRequest
    {
        public string Path { get; set; }
        public bool ClearCache { get; set; } = false;
        public bool IsPreview { get; set; } = false;
        public int? PageID { get; set; }
    }

    [ApiController]
    [Route("api/content/v1.0")]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public class ContentController : MediakiwiController
    {
        private readonly IMemoryCache _cache;

        public ContentController(IMemoryCache memoryCache)
        {
            _cache = memoryCache;
        }

        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet]
        [Route("ping")]
        public async Task<ActionResult<string>> Ping()
        {
            return Ok("Hello");
        }

        static DateTime Last_Flush { get; set; } = DateTime.UtcNow;

        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet]
        [Route("flush")]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<ActionResult<bool>> Flush([FromQuery] long now)
        {
            var environment = await EnvironmentVersion.SelectAsync().ConfigureAwait(false);

            if (now == 0)
            {
                ClearCache();
                Last_Flush = DateTime.UtcNow;
                return true;
            }

            if (now < environment.Updated.GetValueOrDefault().Ticks)
            {
                if (Last_Flush < environment.Updated.GetValueOrDefault())
                {
                    ClearCache();
                    Last_Flush = DateTime.UtcNow;
                }
                return true;
            }
            return false;
        }

        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet]
        [Route("state")]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<ActionResult<string>> Status()
        {
            var environment = await EnvironmentVersion.SelectAsync().ConfigureAwait(false);
            var information = string.Empty;

            information += $"Data updated: {environment.Updated.Value} \n";
            information += $"Cache updated: {Last_Flush} \n";
            information += $"Timestamp: {DateTime.UtcNow.Ticks} \n";
            return Ok(information);
        }

        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet]
        [Route("page/notfound")]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<PageContentResponse> GetPageNotFoundContent([FromQuery] int? siteId = null)
        {
            return await GetPageNotFoundAsync(siteId).ConfigureAwait(false);
        }

        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPost]
        [Route("getPageContent")]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<ActionResult<PageContentResponse>> GetPageContent(PageRequest req)
        {
            return await GetPageContentInternalAsync(
                req.Path,
                req.ClearCache,
                req.IsPreview,
                req.PageID,
                Request?.Query)
                .ConfigureAwait(false);
        }

        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status405MethodNotAllowed)]
        [Route("page/content")]
        [HttpGet]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<ActionResult<PageContentResponse>> GetPageContentAsync([FromQuery] string url, [FromQuery] string flush, [FromQuery] string preview, [FromQuery] int? pageId)
        {
            return await GetPageContentInternalAsync(
                url,
                flush?.Equals("me", StringComparison.InvariantCultureIgnoreCase) == true,
                preview?.Equals("1", StringComparison.InvariantCultureIgnoreCase) == true,
                pageId,
                Request?.Query)
                .ConfigureAwait(false);
        }

        private async Task<ActionResult<PageContentResponse>> GetPageContentInternalAsync(string url, bool flushCache = false, bool isPreview = false, int? pageId = null, IQueryCollection queryParameters = null)
        {
            var response = new PageContentResponse();

            if (string.IsNullOrWhiteSpace(url) && pageId.GetValueOrDefault(0) == 0)
            {
                response.Exception = "There was no URL or PageID supplied";
                response.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest(response);
            }

            // Check if we received an explicit request to clear the cache (flushCache)
            // Or that the cache is invalid based on the Last_Flush datetime 
            bool isCacheInvalid = flushCache || (await Flush(Last_Flush.Ticks).ConfigureAwait(false)).Value;

            try
            {
                var page = default(Page);
                string cacheKey = "";
                var pageMap = default(IPageMapping);
                string queryString = string.Empty;

                if (queryParameters != null)
                {
                    // These params are already accounted for in url and flushCache, no need to pass them again
                    string[] excludeParams = new string[] { "url", "flush" };

                    // Create a new collection of name=value items, where the excludeParams are removed
                    var queryParams = queryParameters.Where(query => !excludeParams.Contains(query.Key)).Select(x => { return $"{x.Key}={x.Value}"; }).ToList();
                    if (queryParams != null && queryParams.Count > 0)
                    {
                        // create a querystring
                        // starting with ?
                        // join each name=value item separated with &
                        queryString = $"?{string.Join("&", queryParams)}";
                    }
                }

                // When a pageID is supplied, this takes presedence over the URL
                if (pageId.GetValueOrDefault(0) > 0)
                {
                    page = await Page.SelectOneAsync(pageId.Value, !isPreview).ConfigureAwait(false);
                    cacheKey = $"page{pageId.Value}";

                    if (isCacheInvalid)
                    {
                        ClearCache();
                        _cache.Remove(cacheKey);
                    }
                }
                else
                {
                    Uri uri = new Uri(WebUtility.UrlDecode(url), UriKind.Relative);
                    cacheKey = $"{uri}";

                    if (isCacheInvalid)
                    {
                        ClearCache();
                        _cache.Remove(cacheKey);
                    }
                    else
                    {
                        // Look for cache key.
                        if (!isPreview && _cache.TryGetValue(cacheKey, out response))
                        {
                            response.InternalInfo.ClearCache = false;
                            response.InternalInfo.IsPreview = false;

                            return Ok(response);
                        }
                    }

                    var maps = await PageMapping.SelectAllAsync(0, true).ConfigureAwait(false);
                    foreach (var map in maps)
                    {
                        var mapped = map as PageMapping;
                        if (map.Path.EndsWith("*", StringComparison.InvariantCultureIgnoreCase))
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
                        else if (map.Path.StartsWith(".", StringComparison.InvariantCultureIgnoreCase) && mapped != null && !string.IsNullOrWhiteSpace(mapped.Expression))
                        {
                            // found a match
                            if (url.EndsWith(map.Path, StringComparison.InvariantCulture))
                            {
                                var redirection = url.Remove(url.Length - map.Path.Length, map.Path.Length);
                                var newurl = mapped.Expression.Equals(".", StringComparison.InvariantCultureIgnoreCase) ? redirection : string.Concat(redirection, mapped.Expression);

                                // append queryString to the new url
                                newurl += queryString;

                                // to do, replace with actual content
                                response = await GetPageNotFoundAsync(null, isCacheInvalid).ConfigureAwait(false);
                                response.PageInternalPath = newurl;
                                response.StatusCode = GetStatusCode(map);
                                return Ok(response);
                            }
                        }
                        // map one to other
                        else if (mapped != null && map.Path.Equals(url, StringComparison.CurrentCultureIgnoreCase) && !string.IsNullOrWhiteSpace(mapped.Expression))
                        {
                            // to do, replace with actual content
                            response = await GetPageNotFoundAsync(null, isCacheInvalid).ConfigureAwait(false);

                            // append queryString to the new url
                            var newUrl = mapped.Expression + queryString;

                            response.PageInternalPath = newUrl;
                            response.StatusCode = GetStatusCode(map);
                            return Ok(response);
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
                        page = await Page.SelectOneAsync(uri.ToString(), !isPreview).ConfigureAwait(false);
                    }
                }


                // Key not in cache, so get data.
                if (response == null)
                {
                    response = new PageContentResponse();
                }

                if (page == null || page?.ID == 0)
                {
                    // Check if this page is the Homepage 
                    if (url == "/" || string.IsNullOrWhiteSpace(url))
                    {
                        response = await GetHomePageAsync(null, isPreview, isCacheInvalid).ConfigureAwait(false);
                    }
                    else
                    {
                        response = await GetPageNotFoundAsync(null, isCacheInvalid).ConfigureAwait(false);
                    }
                }
                else
                {
                    response = await GetPageContentAsync(page, pageMap, isCacheInvalid, isPreview).ConfigureAwait(false);
                }

                // BD 2021-08-05: Only add live pages to the cache to prevent collisions
                if (!isPreview)
                {
                    // Save data in cache.
                    response.InternalInfo.ClearCache = false;
                    AddToCache(cacheKey, response);
                }
            }
            catch (Exception ex)
            {
                response.Exception = $"{ex.Message} {ex.StackTrace}";
            }

            response.InternalInfo.ClearCache = isCacheInvalid;
            response.InternalInfo.IsPreview = isPreview;

            return Ok(response);
        }

        /// <summary>
        /// Gets the content for the 'Homepage' page and returns it as an HttpStatus 200 Code
        /// </summary>
        /// <param name="siteId">The site for which to get this page (or default environment site when none supplied)</param>
        /// <returns></returns>
        private async Task<PageContentResponse> GetHomePageAsync(int? siteId, bool isPreview = false, bool flushCache = false)
        {
            PageContentResponse response = new PageContentResponse();

            int _siteId = (siteId.GetValueOrDefault(0) > 0) ? siteId.Value : 0;
            if (_siteId == 0 && Mediakiwi.Data.Environment.Current?.DefaultSiteID.GetValueOrDefault(0) > 0)
            {
                _siteId = Mediakiwi.Data.Environment.Current.DefaultSiteID.Value;
            }

            if (_siteId > 0)
            {
                var site = await Site.SelectOneAsync(_siteId).ConfigureAwait(false);
                if (site?.HomepageID.GetValueOrDefault(0) > 0)
                {
                    var page = await Page.SelectOneAsync(site.HomepageID.Value).ConfigureAwait(false);
                    if (page?.ID > 0)
                    {
                        response = await GetPageContentAsync(page, null, flushCache, isPreview).ConfigureAwait(false);
                    }
                }
            }
            return response;
        }

        /// <summary>
        /// Gets the content for the 'Page not found' page and returns it as an HttpStatus 404 Code
        /// </summary>
        /// <param name="siteId">The site for which to get this page (or default environment site when none supplied)</param>
        /// <returns></returns>
        private async Task<PageContentResponse> GetPageNotFoundAsync(int? siteId, bool flushCache = false)
        {
            PageContentResponse response = new PageContentResponse();
            int _siteId = (siteId.GetValueOrDefault(0) > 0) ? siteId.Value : 0;
            if (_siteId == 0 && Mediakiwi.Data.Environment.Current?.DefaultSiteID.GetValueOrDefault(0) > 0)
            {
                _siteId = Mediakiwi.Data.Environment.Current.DefaultSiteID.Value;
            }

            if (_siteId > 0)
            {
                var site = await Site.SelectOneAsync(_siteId).ConfigureAwait(false);
                if (site?.PageNotFoundID.GetValueOrDefault(0) > 0)
                {
                    var page = await Page.SelectOneAsync(site.PageNotFoundID.Value).ConfigureAwait(false);
                    if (page?.ID > 0)
                    {
                        response = await GetPageContentAsync(page, null, flushCache, false).ConfigureAwait(false);
                    }
                }
            }

            response.StatusCode = HttpStatusCode.NotFound;
            return response;
        }

        private static void ClearCache()
        {
            if (_resetCacheToken != null && !_resetCacheToken.IsCancellationRequested && _resetCacheToken.Token.CanBeCanceled)
            {
                _resetCacheToken.Cancel();
                _resetCacheToken.Dispose();
            }
            _resetCacheToken = new CancellationTokenSource();
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
            {
                return null;
            }

            Dictionary<string, ContentItem> lst = new Dictionary<string, ContentItem>();

            var mfs = MultiField.GetDeserialized(inField.Value);
            if (mfs.Length > 0)
            {
                int idx = 0;
                foreach (var item in mfs)
                {
                    idx++;
                    (ContentItem contentItem, bool isFilled) result = await getContentItemFromFieldAsync(request, new Field(item.Property, item.Type, item.Value)).ConfigureAwait(false);
                    if (result.isFilled)
                    {
                        lst.Add($"Multifield_{idx}", result.contentItem);
                    }
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

        private async Task<(ContentItem contentItem, bool isFilled)> getContentItemFromFieldAsync(HeadlessRequest request, Field inField)
        {
            var content = new ContentItem();
            bool isFilled = false;
            var culture = "en-US";
            if (request.Page != null && request.Page.Site != null)
            {
                culture = request.Page.Site.Culture;
            }

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
                            {
                                content.Width = inField?.Image.Width;
                            }
                            if (inField?.Image.Height > 0)
                            {
                                content.Height = inField?.Image.Height;
                            }

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
                                {
                                    content.Href = inField.Link.ExternalUrl;
                                }
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
                            content.Text = Utility.CleanConcurrentBreaks(inField.Value, true);
                        }
                    }
                    break;
                case ContentType.SubListSelect:
                    {
                        if (!string.IsNullOrWhiteSpace(inField.Value))
                        {
                            var data = SubList.GetDeserialized(inField.Value);
                            if (data?.Items?.Any() == true && data.List.HasValue)
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
                                            await instance.DoHeadLessFetchAsync(request).ConfigureAwait(false);
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
                            content.MultiFieldContent = await getMultiFieldContentAsync(request, inField).ConfigureAwait(false);
                        }
                    }
                    break;
                case ContentType.Date:
                    {
                        if (!string.IsNullOrWhiteSpace(inField.Value) && long.TryParse(inField.Value, out long unitTime))
                        {
                            isFilled = true;
                            var dateTimeValue = new DateTime(unitTime);
                            System.Globalization.CultureInfo dateCulture = new System.Globalization.CultureInfo(culture);
                            content.Text = dateTimeValue.ToString(dateCulture.DateTimeFormat.SortableDateTimePattern, dateCulture);
                        }
                    }
                    break;
                case ContentType.DateTime:
                    {
                        if (!string.IsNullOrWhiteSpace(inField.Value) && long.TryParse(inField.Value, out long unitTime))
                        {
                            isFilled = true;
                            var dateTimeValue = new DateTime(unitTime);
                            System.Globalization.CultureInfo dateCulture = new System.Globalization.CultureInfo(culture);
                            content.Text = dateTimeValue.ToString(dateCulture.DateTimeFormat.SortableDateTimePattern, dateCulture);
                        }
                    }
                    break;
            }

            return (content, isFilled);
        }

        private static string ConvertUrl(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return string.Empty;
            }

            return path
                .ToLowerInvariant()
                .Replace(" ", "-", StringComparison.InvariantCulture);
        }

        static HttpStatusCode GetStatusCode(IPageMapping pageMap)
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

        // TODO: replace this on the fly with real data
        // Same as PriceLogic.cs line 276
        private Dictionary<string, List<string>> NestedComponentCollection { get; set; } = new Dictionary<string, List<string>>()
        {
            {
                // PARENT :                               CHILDS :
                "C000_NavigationFR", new List<string>() { "C000_MegaMenuColumn", "C000_MegaMenuLink" }
            }
        };

        /// <summary>
        /// Returns the full page content for the requested Page 
        /// </summary>
        /// <param name="page">The Mediakiwi page to get the content for</param>
        /// <param name="pageMap">The pagemap to use, if any</param>
        /// <param name="ispreview">Is this preview mode ? then the non-published version will be returned</param>
        /// <returns></returns>
        private async Task<PageContentResponse> GetPageContentAsync(Page page, IPageMapping pageMap, bool flushCache, bool ispreview)
        {
            var response = new PageContentResponse();

            //  Page status validation
            var status = GetStatusCode(pageMap);

            if (pageMap != null && status == HttpStatusCode.NotFound)
            {
                return await GetPageNotFoundAsync(page.SiteID, flushCache).ConfigureAwait(false);
            }

            // validation parse url?
            response.PageID = page.ID;
            response.ChannelID = page.SiteID;
            response.PageInternalPath = ConvertUrl(page.InternalPath);
            response.PageLocation = page.Template.Location;
            response.StatusCode = status;

            // If we have a pagemap and a redirect, return here
            if (pageMap != null && (response.StatusCode == HttpStatusCode.Redirect || response.StatusCode == HttpStatusCode.MovedPermanently))
            {
                return response;
            }

            // Extract the page title from the pagemap (if any) or the page
            var pageTitle = default(string);
            if (pageMap == null || pageMap.Title == null)
            {
                pageTitle = page.Title;
            }
            else
            {
                pageTitle = pageMap.Title;
            }

            // Do we have a default page title in place
            if (page.Site.DefaultPageTitle != null)
            {
                // validate the presence of a placeholder.
                if (page.Site.DefaultPageTitle.Contains("[title]", StringComparison.InvariantCultureIgnoreCase))
                {
                    pageTitle = page.Site.DefaultPageTitle.Replace("[title]"
                        , string.IsNullOrWhiteSpace(pageTitle)
                        ? string.Empty : pageTitle
                        , StringComparison.InvariantCultureIgnoreCase);
                }
                // Only set the page title when it's currently empty
                else if (string.IsNullOrEmpty(pageTitle))
                {
                    pageTitle = page.Site.DefaultPageTitle;
                }
            }

            response.MetaData.PageTitle = pageTitle;

            // Do we have a site language defined ?
            if (string.IsNullOrWhiteSpace(page.Site.Language) == false)
            {
                // Set it so the metadata HTML language
                response.MetaData.HtmlLang = page.Site.Language.Split('-')[0];
            }

            // Do we have a page description defined ?
            if (string.IsNullOrWhiteSpace(page.Description) == false || string.IsNullOrWhiteSpace(page.Keywords) == false)
            {
                // Add Page description to the metadata HTML Description
                response.MetaData.MetaTags = new List<ContentMetaTag>();
                if (string.IsNullOrWhiteSpace(page.Description) == false)
                {
                    response.MetaData.MetaTags.Add(new ContentMetaTag("description", page.Description));
                }

                // Add Page keywords to the metadata HTML Keywords
                if (string.IsNullOrWhiteSpace(page.Keywords) == false)
                {
                    response.MetaData.MetaTags.Add(new ContentMetaTag("keywords", page.Keywords));
                }
            }

            // Store all page components
            Component[] components;
            if (ispreview)
            {
                // Get the non-published versions of the components  for this page
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
            {
                // Get the published versions of the components for this page
                components = await Component.SelectAllInheritedAsync(page.ID, false, !flushCache).ConfigureAwait(false);
            }

            // Store all SharedFieldTranslations
            var allSharedFieldTranslations = await SharedFieldTranslation.FetchAllForPageAsync(page.ID, page.SiteID).ConfigureAwait(false);

            int sort = 0;
            foreach (var component in components)
            {
                // TODO: slot targets must be settable in the CMS
                if (component?.Template?.SourceTag?.Contains(" ", StringComparison.InvariantCultureIgnoreCase) == true)
                {
                    component.Template.SourceTag = component.Template.SourceTag.Replace(" ", "_", StringComparison.InvariantCultureIgnoreCase);
                }

                string slotTarget = "content";
                if (component?.Template?.SourceTag?.Equals("C000_Navigation", StringComparison.InvariantCultureIgnoreCase) == true)
                {
                    slotTarget = "header";
                }

                if (component?.Template?.IsFooter == true)
                {
                    slotTarget = "footer";
                }
                else if (component?.Template?.IsHeader == true)
                {
                    slotTarget = "header";
                }


                sort++;
                var mapped = new ContentComponent()
                {
                    ComponentName = component.Template.SourceTag,
                    IsShared = component.Template.IsShared,
                    Title = component.Template.Name,
                    ComponentID = component.ID,
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
                    // Store all wim_Properties 
                    var allWimProperties = await Property.SelectAllByTemplateAsync(component.Template.ID).ConfigureAwait(false);

                    foreach (var field in component.Content.Fields)
                    {
                        // Seperate field from foreach loop
                        Field fieldItem = field;

                        // get the Matching WimProperty for this content field
                        var wimProp = allWimProperties.FirstOrDefault(x => x.ContentTypeID == (ContentType)fieldItem.Type && x.FieldName.Equals(fieldItem.Property, StringComparison.InvariantCultureIgnoreCase));

                        // Is this property marked as shared field and do we have any shared field translations at all
                        if (wimProp?.IsSharedField == true && allSharedFieldTranslations?.Count > 0)
                        {
                            // Get the Value for this shared field for this site.
                            var sharedFieldValue = allSharedFieldTranslations.FirstOrDefault(x => x.ContentTypeID == wimProp.ContentTypeID && x.FieldName == wimProp.FieldName && x.SiteID == page.SiteID);

                            // Do we have a sharedFieldValue ?
                            // If so, apply its content to the fieldItem
                            if (sharedFieldValue?.ID > 0)
                            {
                                fieldItem = fieldItem.ApplySharedValue(sharedFieldValue, ispreview);
                            }
                            else
                            {
                                fieldItem.Value = "";
                            }
                        }

                        // Get the content based on the fieldItem, this will resolve based on the contenttype
                        (ContentItem content, bool isFilled) result = await getContentItemFromFieldAsync(request, fieldItem).ConfigureAwait(false);

                        // Only add this property when it's not yet added and the content for it is filled.
                        if (!mapped.Content.ContainsKey(fieldItem.Property) && result.isFilled)
                        {
                            mapped.Content.Add(fieldItem.Property, result.content);
                        }
                    }
                }

                // Check if we have a NestedComponent Collection match for this (child) item
                if (NestedComponentCollection.Any(x => x.Value.Contains(request.Component.Template.SourceTag)))
                {
                    // Find out to which parent this (child) component belongs
                    var nestedItem = NestedComponentCollection.FirstOrDefault(x => x.Value.Contains(request.Component.Template.SourceTag));

                    // Get the parent component in which this child component will be nested
                    var parent = response.Components.FirstOrDefault(x => x.ComponentName.Equals(nestedItem.Key, StringComparison.InvariantCultureIgnoreCase));
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

                // Check for nested components
                if (request.Component.Template.NestedType.HasValue)
                {
                    var parent = default(ContentComponent);
                    if (request.Component.Template.NestedType == 1)
                    {
                        parent = response.Components.FirstOrDefault(x => x.ComponentName.Equals(request.Component.Template.SourceTag, StringComparison.InvariantCultureIgnoreCase));
                    }
                    else if (request.Component.Template.NestedType == 2)
                    {
                        var previous = response.Components.LastOrDefault();
                        if (parent.ComponentName.Equals(request.Component.Template.SourceTag, StringComparison.InvariantCultureIgnoreCase))
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

            // Check for inherited pages
            if (page.MasterID.GetValueOrDefault(0) > 0)
            {
                var masterPage = await Page.SelectOneAsync(page.MasterID.Value);
                if (masterPage?.ID > 0)
                {
                    response.InheritedPages.Add(GetInheritedPage(masterPage));

                    foreach (var inheritedPage in await masterPage.GetInheritedPages())
                    {
                        response.InheritedPages.Add(GetInheritedPage(inheritedPage));
                    }
                }
            }
            else
            {
                var inheritedPages = await page.GetInheritedPages();
                if (inheritedPages?.Count > 0)
                {
                    response.InheritedPages.Add(GetInheritedPage(page));

                    foreach (var inheritedPage in inheritedPages)
                    {
                        response.InheritedPages.Add(GetInheritedPage(inheritedPage));
                    }
                }
            }

            return response;
        }

        private InheritedPage GetInheritedPage(Page inPage)
        {
            return new InheritedPage()
            {
                ChannelCulture = (string.IsNullOrWhiteSpace(inPage?.Site?.Culture) == false) ? inPage.Site.Culture : string.Empty,
                ChannelLanguage = (string.IsNullOrWhiteSpace(inPage?.Site?.Language) == false) ? inPage.Site.Language : string.Empty,
                ChannelID = inPage.SiteID,
                Href = inPage.InternalPath,
                PageID = inPage.ID,
            };
        }
    }
}
