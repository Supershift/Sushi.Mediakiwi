using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Sushi.Mediakiwi.Headless.Config;
using Sushi.Mediakiwi.Headless.Data;
using Sushi.Mediakiwi.Headless.HttpClients.Interfaces;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Headless
{

    public static class MediaKiwiContentServiceExtensions
    {
        private static Action<HttpClient> DefaultHttpClient()
        {
            return client =>
            {
                client.Timeout = TimeSpan.FromSeconds(2); // Default two second timeout
            };
        }

        public static void AddMediaKiwiContentService(this IServiceCollection services)
        {
            services.AddTransient<HttpClients.DefaultHttpClientHandler>();
            services.AddHttpClient<IMediakiwiContentServiceClient, HttpClients.MediakiwiContentServiceClient>(nameof(HttpClients.MediakiwiContentServiceClient), client => DefaultHttpClient()).ConfigurePrimaryHttpMessageHandler<HttpClients.DefaultHttpClientHandler>();

            var mk = new MediaKiwiContentService(services.BuildServiceProvider());
            services.AddSingleton<IMediaKiwiContentService>(mk);
        }
    }

    public class MediaKiwiContentService : IMediaKiwiContentService
    {
        private static CancellationTokenSource _resetCacheToken = new CancellationTokenSource();
        private static DateTime Last_Flush = DateTime.UtcNow;
        private readonly IMediakiwiContentServiceClient _httpClient;
        private readonly IMemoryCache _memCache;
        private readonly ILogger _logger;
        private readonly ISushiApplicationSettings _configuration;

        public static MemoryCacheEntryOptions ExpirationToken()
        {
            return ExpirationToken(DateTimeOffset.UtcNow.AddDays(1));
        }

        public static MemoryCacheEntryOptions ExpirationToken(DateTimeOffset expiration)
        {
            var options = new MemoryCacheEntryOptions().SetPriority(CacheItemPriority.Normal).SetAbsoluteExpiration(expiration);
            options.AddExpirationToken(new CancellationChangeToken(_resetCacheToken.Token));
            return options;
        }

        /// <summary>
        /// Performs a cache flush and updates the Last_Flush date
        /// </summary>
        public static void FlushCache()
        {
            if (_resetCacheToken != null && !_resetCacheToken.IsCancellationRequested && _resetCacheToken.Token.CanBeCanceled)
            {
                Last_Flush = DateTime.UtcNow;

                _resetCacheToken.Cancel();
                _resetCacheToken.Dispose();
            }
            _resetCacheToken = new CancellationTokenSource();
        }

        public MediaKiwiContentService(IServiceProvider serviceProvider)
        {
            _memCache = serviceProvider.GetService<IMemoryCache>();
            _httpClient = serviceProvider.GetService<IMediakiwiContentServiceClient>();
            _configuration = serviceProvider.GetService<ISushiApplicationSettings>();

            var _loggerFactory = serviceProvider.GetService<ILoggerFactory>();
            if (_loggerFactory != null)
            {
                _logger = _loggerFactory.CreateLogger<MediaKiwiContentService>();
            }
        }

        #region Get Page Content - Not Found

        public async Task<PageContentResponse> GetPageNotFoundContent(int? siteId = null, bool clearCache = false)
        {
            PageContentResponse returnObj = new PageContentResponse();
            if (string.IsNullOrWhiteSpace(_configuration.MediaKiwi.ContentService.ServiceUrl))
            {
                return returnObj;
            }

            if (_configuration.MediaKiwi.ContentService.PingFirst && await PingSucceeded() == false)
            {
                return returnObj;
            }

            // Create cachekey
            string cacheKey = $"Page_NotFound_Site_{siteId.GetValueOrDefault(0)}";

            // Throw warning when we dont have memorycache
            if (_memCache == null)
            {
                _logger.LogWarning($"Memorycache is not enabled, please add it to services in the startup");
            }

            // Log that we requested a clear cache
            if (clearCache)
            {
                _logger.LogInformation($"A cache clear was requested for '{cacheKey}'");
            }

            // Call the Web API to validate our cache
            // Returns FALSE when a cache flush is needed
            bool isCached = await IsCacheValid();

            // Try to get from cache
            if (_memCache == null || _memCache.TryGetValue(cacheKey, out returnObj) == false || clearCache || isCached == false)
            {
                try
                {
                    // Fetch JSON content from service
                    string responseFromServer = await _httpClient.GetPageNotFoundContent(siteId, clearCache);

                    // Convert JSON content
                    if (string.IsNullOrWhiteSpace(responseFromServer) == false)
                    {
                        returnObj = JsonConvert.DeserializeObject<PageContentResponse>(responseFromServer);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message, null);
                    returnObj.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                }

                // If the Content API returned us a page that needs a cache flush, perform it here as well.
                if (returnObj?.InternalInfo?.ClearCache == true)
                {
                    FlushCache();
                }

                // Add content to cache if needed
                if (returnObj?.PageID > 0 && _memCache != null)
                {
                    // When stored to cache, make sure to set this value to False,
                    // else it will keep flushing the cache everytime it is collected from cache.
                    returnObj.InternalInfo.ClearCache = false;

                    // Add pagecontent to cache
                    AddToCache(cacheKey, returnObj);
                }
            }

            if (!isCached || clearCache)
            {
                returnObj.InternalInfo.ClearCache = true;
            }

            return returnObj;
        }

        #endregion Get Page Content - Not Found

        #region Get Page Content - HttpRequest

        public async Task<PageContentResponse> GetPageContentAsync(HttpRequest request)
        {
            // Get URL
            var url = request.GetDisplayUrl();
            if (url.Contains("?"))
            {
                url = url.Substring(0, url.IndexOf('?'));
            }

            if (url.Contains("#"))
            {
                url = url.Substring(0, url.IndexOf('#'));
            }

            // Remove current PathBase from the URL
            string AppBaseUrl = $"{request.Scheme}://{request.Host}{request.PathBase}";
            url = url.Replace(AppBaseUrl, "");

            // Determine if this is a call to clear the cache
            bool isClearCacheCall = request.IsClearCacheCall();

            // Determine if this is a Preview call
            bool isPreviewCall = request.IsPreviewCall();

            if (IsPageExcluded(request))
            {
                _logger.LogInformation($"This url ({url}) was Excluded from the content service");
                return new PageContentResponse();
            }
            else
            {
                return await GetPageContentAsync(
                    forUrl: url,
                    basePath: AppBaseUrl,
                    clearCache: isClearCacheCall,
                    isPreview: isPreviewCall,
                    queryCollection: request.Query
                    );
            }
        }

        #endregion Get Page Content - HttpRequest

        #region Get Page Content - Url / PageID

        public async Task<PageContentResponse> GetPageContentAsync(string forUrl, string basePath, bool clearCache = false, bool isPreview = false, int? pageId = null, IQueryCollection queryCollection = null)
        {
            PageContentResponse returnObj = new PageContentResponse();
            if (string.IsNullOrWhiteSpace(_configuration.MediaKiwi.ContentService.ServiceUrl))
            {
                returnObj.Exception = "No MediaKiwi ContentService URL has been set";
                returnObj.StatusCode = System.Net.HttpStatusCode.InternalServerError;

                return returnObj;
            }

            if (_configuration.MediaKiwi.ContentService.PingFirst && await PingSucceeded() == false)
            {
                returnObj.Exception = "Ping did not succeed";
                returnObj.StatusCode = System.Net.HttpStatusCode.NotFound;

                return returnObj;
            }

            // Create cachekey
            string cacheKey = "";
            if (pageId.GetValueOrDefault(0) == 0)
            {
                if (string.IsNullOrWhiteSpace(basePath) == false)
                {
                    cacheKey = new Uri($"{basePath}{forUrl}", UriKind.Absolute).ToString();
                }
                else
                {
                    cacheKey = new Uri(forUrl, UriKind.Relative).ToString();
                }
            }
            else
            {
                cacheKey = $"Page_{pageId.Value}";
            }

            // Throw warning when we dont have memorycache
            if (_memCache == null)
            {
                _logger.LogWarning($"Memorycache is not enabled, please add it to services in the startup");
            }

            // Log that we requested a clear cache
            if (clearCache)
            {
                _logger.LogInformation($"A cache clear was requested for '{cacheKey}'");
            }

            // Log that this was a preview call and set ClearCache to true, since we don't want caching for previews
            if (isPreview)
            {
                // Add queryparameters to cacheKey regardless of pageId or url
                if (queryCollection != null && queryCollection.Count > 0)
                {
                    // Order the query params and create a new collection of name=value items
                    var queryParams = queryCollection.OrderBy(x => x.Key).Select(x => { return $"{x.Key}={x.Value}"; }).ToList();
                    if (queryParams != null && queryParams.Count > 0)
                    {
                        // starting with ?
                        // join each name=value item separated with &
                        string queryString = $"?{string.Join("&", queryParams)}";

                        // Append to the cacheKey
                        cacheKey += queryString;
                    }
                }

                _logger.LogInformation($"A preview call was requested for '{cacheKey}' cache will be ignored");
                clearCache = true;
            }

            bool isCached = false;

            // Check if the URL should be excluded
            if (string.IsNullOrWhiteSpace(forUrl) == false && IsPageExcluded(forUrl))
            {
                _logger.LogInformation($"This url ({forUrl}) was Excluded from the content service");
            }
            else
            {
                isCached = await IsCacheValid();

                // Try to get from cache (ASYNC!)
                if (_memCache == null || _memCache.TryGetValue(cacheKey, out returnObj) == false || clearCache || isCached == false)
                {
                    try
                    {
                        // Read the JSON content.
                        string responseFromServer = await _httpClient.GetPageContentStringAsync(forUrl, basePath, clearCache, isPreview, pageId, queryCollection);

                        // Convert the JSON content.
                        if (string.IsNullOrWhiteSpace(responseFromServer) == false)
                        {
                            returnObj = JsonConvert.DeserializeObject<PageContentResponse>(responseFromServer);
                        }

                        if (returnObj != null)
                        {
                            // If the API returned us a page that needs a cache flush, perform it here as well.
                            if (returnObj?.InternalInfo?.ClearCache == true)
                            {
                                FlushCache();
                            }
   
                            // Set MetaData 
                            SetMetaInfo(returnObj, forUrl);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, ex.Message, null);
                        if (returnObj == null) 
                        {
                            returnObj = new PageContentResponse();
                        }
                        returnObj.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                    }

                    // Add content to cache if needed
                    if (returnObj?.PageID > 0 && _memCache != null)
                    {     
                        // When stored to cache, make sure to set this value to False,
                        // else it will keep flushing the cache everytime it is collected from cache.
                        returnObj.InternalInfo.ClearCache = false;
                        
                        // Add pagecontent to cache
                        AddToCache(cacheKey, returnObj);
                    }
                }
                else 
                {
                    _logger.LogInformation($"Got cached content for '{forUrl}' (Page: {returnObj.MetaData?.PageTitle} : {returnObj.PageID})");
                }
            }
            
            if (returnObj == null) 
            {
                returnObj = new PageContentResponse();
            }

            if (!isCached || clearCache)
            {
                returnObj.InternalInfo.ClearCache = true;
            }
            return returnObj;
        }

        #endregion Get Page Content - Url / PageID

        #region Set Meta Info (TEMPORARY FIX)

        /// <summary>
        /// Will set de default META tags for Twitter and Facebook
        /// </summary>
        /// <param name="PageContent">The pagecontent to enrich with META information</param>
        /// <param name="url">The URL to set for facebook</param>
        private void SetMetaInfo(PageContentResponse PageContent, string url)
        {
            string pageDescription = "";
            if (string.IsNullOrWhiteSpace(PageContent.MetaData.PageDescription) && PageContent.MetaData.ContainsKey("description"))
            {
                pageDescription = PageContent.MetaData["description"].Value;
            }
            else
            {
                pageDescription = PageContent.MetaData.PageDescription;
            }

            // Facebook 
            if (PageContent.MetaData.ContainsKey("og:title") == false)
            {
                PageContent.MetaData.Add("og:title", PageContent.MetaData.PageTitle, MetaTagRenderKey.PROPERTY);
            }

            if (PageContent.MetaData.ContainsKey("og:description") == false)
            {
                PageContent.MetaData.Add("og:description", pageDescription, MetaTagRenderKey.PROPERTY);
            }

            if (PageContent.MetaData.ContainsKey("og:url") == false)
            {
                PageContent.MetaData.Add("og:url", url);
            }

            // Twitter
            if (PageContent.MetaData.ContainsKey("twitter:title") == false)
            {
                PageContent.MetaData.Add("twitter:title", PageContent.MetaData.PageTitle);
            }

            if (PageContent.MetaData.ContainsKey("twitter:description") == false)
            {
                PageContent.MetaData.Add("twitter:description", pageDescription);
            }

            if (PageContent.MetaData.ContainsKey("twitter:card") == false)
            {
                PageContent.MetaData.Add("twitter:card", "summary_large_image");
            }

            // General
            if (PageContent.MetaData.ContainsKey("description") == false)
            {
                PageContent.MetaData.Add("description", pageDescription);
            }
        }

        #endregion Set Meta Info (TEMPORARY FIX)

        #region Is Page Excluded - HttpRequest

        public bool IsPageExcluded(HttpRequest request)
        {
            if (_configuration.MediaKiwi.ContentService.ExcludePaths == null || _configuration.MediaKiwi.ContentService.ExcludePaths.Count == 0)
            {
                return false;
            }

            // Get URL
            var url = request.GetDisplayUrl().ToLowerInvariant();

            if (url.Contains("?"))
            {
                url = url.Substring(0, url.IndexOf('?'));
            }
            if (url.Contains("#"))
            {
                url = url.Substring(0, url.IndexOf('#'));
            }

            foreach (var excludeRule in _configuration.MediaKiwi.ContentService.ExcludePaths)
            {
                if (url.Contains(excludeRule.ToLowerInvariant()))
                {
                    return true;
                }
            }

            return false;
        }

        #endregion Is Page Excluded - HttpRequest

        #region Is Page Excluded - Url

        public bool IsPageExcluded(string forUrl)
        {
            if (_configuration.MediaKiwi.ContentService.ExcludePaths == null || _configuration.MediaKiwi.ContentService.ExcludePaths.Count == 0)
            {
                return false;
            }

            string temp = forUrl.ToLowerInvariant();

            foreach (var excludeRule in _configuration.MediaKiwi.ContentService.ExcludePaths)
            {
                if (temp.Contains(excludeRule.ToLowerInvariant()))
                {
                    return true;
                }
            }

            return false;
        }

        #endregion Is Page Excluded - Url

        #region Is Cache Valid

        public async Task<bool> IsCacheValid()
        {
            if (string.IsNullOrWhiteSpace(_configuration.MediaKiwi.ContentService.ServiceUrl))
            {
                return true;
            }

            try
            {
                // Get content from service
                bool responseFromServer = await _httpClient.GetCacheValidAsync(Last_Flush);

                if (responseFromServer)
                {
                    FlushCache();
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }

            return true;
        }

        #endregion Is Cache Valid

        #region Add To Cache

        void AddToCache(string key, object item)
        {
            _memCache.Set(key, item, ExpirationToken());
            _logger.LogInformation($"Added content to cache for '{key}'", item);
        }

        #endregion Add To Cache

        #region Ping Succeeded

        public async Task<bool> PingSucceeded()
        {
            bool success = false;

            if (string.IsNullOrWhiteSpace(_configuration?.MediaKiwi?.ContentService?.ServiceUrl) == false)
            {
                try
                {
                    success = await _httpClient.PingAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message, null);
                }
            }

            return success;
        }

        #endregion Ping Succeeded
    }
}
