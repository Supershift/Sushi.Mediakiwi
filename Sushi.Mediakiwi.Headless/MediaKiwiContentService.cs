using FluentValidation.Validators;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Sushi.Mediakiwi.Headless.Config;
using Sushi.Mediakiwi.Headless.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Headless
{
    public static class MediaKiwiContentServiceExtensions
    {
        public static void AddMediaKiwiContentService(this IServiceCollection services)
        {
            var mk = new MediaKiwiContentService(services.BuildServiceProvider());
            services.AddSingleton<IMediaKiwiContentService>(mk);
        }
    }

    public class MediaKiwiContentService : IMediaKiwiContentService
    {
        private static CancellationTokenSource _resetCacheToken = new CancellationTokenSource();

        private readonly IMemoryCache _memCache;
        private readonly ILogger _logger;
        private ISushiApplicationSettings Configuration;


        public static MemoryCacheEntryOptions ExpirationToken()
        {
            return ExpirationToken(DateTimeOffset.UtcNow.AddDays(1));
        }

        public static MemoryCacheEntryOptions ExpirationToken(DateTimeOffset expiration)
        {
            expiration = DateTimeOffset.UtcNow.AddDays(1);

            var options = new MemoryCacheEntryOptions().SetPriority(CacheItemPriority.Normal).SetAbsoluteExpiration(expiration);
            options.AddExpirationToken(new CancellationChangeToken(_resetCacheToken.Token));
            return options;
        }

        public static void FlushCache()
        {
            if (_resetCacheToken != null && !_resetCacheToken.IsCancellationRequested && _resetCacheToken.Token.CanBeCanceled)
            {
                _resetCacheToken.Cancel();
                _resetCacheToken.Dispose();
            }
            _resetCacheToken = new CancellationTokenSource();
        }

        public MediaKiwiContentService(IServiceProvider serviceProvider)
        {
            _memCache = serviceProvider.GetService<IMemoryCache>();
            var _loggerFactory = serviceProvider.GetService<ILoggerFactory>();
            _logger = _loggerFactory.CreateLogger<MediaKiwiContentService>();
            Configuration = serviceProvider.GetService<ISushiApplicationSettings>();
        }

        public PageContentResponse GetPageNotFoundContent(int? siteId = null, bool clearCache = false)
        {
            PageContentResponse returnObj = new PageContentResponse();
            if (string.IsNullOrWhiteSpace(Configuration.MediaKiwi.ContentService.ServiceUrl))
                return returnObj;

            if (Configuration.MediaKiwi.ContentService.PingFirst == true)
            {
                if (PingSucceeded() == false)
                    return returnObj;
            }

            // Create cachekey
            string cacheKey = $"Page_NotFound_Site_{siteId.GetValueOrDefault(0)}";

            // Throw warning when we dont have memorycache
            if (_memCache == null)
                _logger.LogWarning($"Memorycache is not enabled, please add it to services in the startup");

            // Log that we requested a clear cache
            if (clearCache)
                _logger.LogInformation($"A cache clear was requested for '{cacheKey}'");


            // Try to get from cache
            if (_memCache == null || _memCache.TryGetValue(cacheKey, out returnObj) == false || clearCache)
            {
                // Create querystring for adding SiteID to the Request
                Dictionary<string, string> dict = new Dictionary<string, string>();
                if (siteId.GetValueOrDefault(0) > 0)
                    dict.Add("siteId", siteId.GetValueOrDefault(0).ToString());

                //when cache failed, retrieve via service
                HttpWebRequest request = HttpWebRequest.CreateHttp(getServiceUrl($"{Configuration.MediaKiwi.ContentService.ServiceUrl}/getPageNotFoundContent", dict));
                request.Method = "POST";
                request.ContentType = "application/json";
                request.Timeout = Configuration.MediaKiwi.ContentService.TimeOut;

                // TODO: needs to be removed for production,
                // this will now accept all SSL certificates 
                request.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
                //request.Expect = "application/json";

                // Get the response.
                WebResponse response = request.GetResponse();
                try
                {
                    // Get the stream containing content returned by the server.
                    // The using block ensures the stream is automatically closed.
                    using (Stream dataStream = response.GetResponseStream())
                    {
                        // Open the stream using a StreamReader for easy access.
                        StreamReader reader = new StreamReader(dataStream);

                        // Read the content.
                        string responseFromServer = reader.ReadToEnd();

                        // Display the content.
                        returnObj = JsonConvert.DeserializeObject<PageContentResponse>(responseFromServer);

                        // Apply default meta tags
                        // TODO: this function should not exist, this data should come from 
                        // the MediaKiwi API
                        //SetMetaInfo(returnObj, forUrl);
                    }

                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message, null);
                }
              
                // Add content to cache if needed
                if (returnObj?.PageID > 0 && _memCache != null)
                {
                    AddCache(cacheKey, returnObj);
                }

            }

            return returnObj;
        }

        void AddCache(string key, object item)
        {
            _memCache.Set(key, item, ExpirationToken());
            _logger.LogInformation($"Added content to cache for '{key}'", item);
        }

        #region Get Page Content - HttpRequest

        public PageContentResponse GetPageContent(HttpRequest request)
        {
            // Get URL
            var url = request.GetDisplayUrl();
            if (url.Contains("?"))
                url = url.Substring(0, url.IndexOf('?'));
            if (url.Contains("#"))
                url = url.Substring(0, url.IndexOf('#'));

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
                return  GetPageContent(url, isClearCacheCall, isPreviewCall);
            }
        }

        public async Task<PageContentResponse> GetPageContentAsync(HttpRequest request)
        {
            // Get URL
            var url = request.GetDisplayUrl();
            if (url.Contains("?"))
                url = url.Substring(0, url.IndexOf('?'));
            if (url.Contains("#"))
                url = url.Substring(0, url.IndexOf('#'));

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
                return await GetPageContentAsync(url, isClearCacheCall, isPreviewCall);
            }
        }

        #endregion Get Page Content - HttpRequest
        
        private string GetSubSiteURL(string inputUrl)
        {
            string temp = inputUrl;
            if (string.IsNullOrWhiteSpace(Configuration?.MediaKiwi?.ContentService?.SiteFolderPrefix) == false)
            {
                string prefix = Configuration.MediaKiwi.ContentService.SiteFolderPrefix.Trim().Trim('/');
                if (string.IsNullOrWhiteSpace(prefix) == false)
                {
                    temp = $"/{prefix}/{inputUrl}";
                    temp = temp.Replace("//", "/");

                    _logger.LogInformation($"Setting content SubSite URL to '{temp}'");
                }
            }
            return temp;
        }

        #region Get Page Content - Url / PageID

        public PageContentResponse GetPageContent(string forUrl, bool clearCache = false, bool isPreview = false, int? pageId = null)
        {
            PageContentResponse returnObj = new PageContentResponse();
            if (string.IsNullOrWhiteSpace(Configuration.MediaKiwi.ContentService.ServiceUrl))
                return returnObj;

            //if (Configuration.MediaKiwi.ContentService.PingFirst == true)
            //{
            //    if (PingSucceeded() == false)
            //        return returnObj;
            //}

            // Create cachekey
            string cacheKey = "";

            if (pageId.GetValueOrDefault(0) == 0)
                cacheKey = new Uri(forUrl, UriKind.Relative).ToString();
            else
                cacheKey = $"Page_{pageId.Value}";

            // Throw warning when we dont have memorycache
            if (_memCache == null)
                _logger.LogWarning($"Memorycache is not enabled, please add it to services in the startup");

            // Log that we requested a clear cache
            if (clearCache)
                _logger.LogInformation($"A cache clear was requested for '{cacheKey}'");

            // Log that this was a preview call and set ClearCache to true, since we don't want caching for previews
            if (isPreview)
            {
                _logger.LogInformation($"A preview call was requested for '{cacheKey}' cache will be ignored");
                clearCache = true;
            }

            bool isCached = false;
            bool invalidatedCache = false;

            // Check if the URL should be excluded
            if (string.IsNullOrWhiteSpace(forUrl) == false && IsPageExcluded(forUrl))
            {
                _logger.LogInformation($"This url ({forUrl}) was Excluded from the content service");
            }
            else
            {

                invalidatedCache = !IsCacheValid();
                // Try to get from cache
                if (_memCache == null || (_memCache.TryGetValue(cacheKey, out returnObj) == false || returnObj == null) || clearCache || invalidatedCache)
                {
                    //when cache failed, retrieve via service
                    HttpWebRequest request = HttpWebRequest.CreateHttp($"{Configuration.MediaKiwi.ContentService.ServiceUrl}/getPageContent");
                    request.Method = "POST";
                    request.ContentType = "application/json";
                    request.Timeout = Configuration.MediaKiwi.ContentService.TimeOut;

                    // TODO: needs to be removed for production,
                    // this will now accept all SSL certificates 
                    request.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;

                    string json = string.Empty;

                    using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                    {
                        json = JsonConvert.SerializeObject(new
                        {
                            Path = GetSubSiteURL(forUrl),
                            ClearCache = clearCache,
                            IsPreview = isPreview,
                            PageID = pageId
                        });
                        streamWriter.Write(json);
                    }

                    try
                    { 
                        // Get the response.
                        WebResponse response = request.GetResponse();

                        // Get the stream containing content returned by the server.
                        // The using block ensures the stream is automatically closed.
                        using (Stream dataStream = response.GetResponseStream())
                        {
                            // Open the stream using a StreamReader for easy access.
                            StreamReader reader = new StreamReader(dataStream);

                            // Read the content.
                            string responseFromServer = reader.ReadToEnd();

                            // Display the content.
                            returnObj = JsonConvert.DeserializeObject<PageContentResponse>(responseFromServer);

                            // Apply default meta tags
                            // TODO: this function should not exist, this data should come from 
                            // the MediaKiwi API
                            SetMetaInfo(returnObj, forUrl);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "The request was: '{0}'", json);
                    }
                   
                    // Add content to cache if needed
                    if (returnObj?.PageID > 0 && _memCache != null)
                    {
                        AddCache(cacheKey, returnObj);
                    }
                }
                else
                {
                    isCached = true;

                    _logger.LogInformation($"Got cached content for '{forUrl}' (Page: {returnObj.MetaData?.PageTitle} : {returnObj.PageID})");
                }
            }

            returnObj.IsCacheInvalidated = invalidatedCache;
            returnObj.IsCached = isCached;
            return returnObj;
        }

        public async Task<PageContentResponse> GetPageContentAsync(string forUrl, bool clearCache = false, bool isPreview = false, int? pageId = null)
        {
            PageContentResponse returnObj = new PageContentResponse();
            if (string.IsNullOrWhiteSpace(Configuration.MediaKiwi.ContentService.ServiceUrl))
                return returnObj;

            if (Configuration.MediaKiwi.ContentService.PingFirst == true)
            {
                if (PingSucceeded() == false)
                    return returnObj;
            }

            // Create cachekey
            string cacheKey = "";

            if (pageId.GetValueOrDefault(0) == 0)
                cacheKey = new Uri(forUrl, UriKind.Relative).ToString();
            else
                cacheKey = $"Page_{pageId.Value}";

            // Throw warning when we dont have memorycache
            if (_memCache == null)
                _logger.LogWarning($"Memorycache is not enabled, please add it to services in the startup");

            // Log that we requested a clear cache
            if (clearCache)
                _logger.LogInformation($"A cache clear was requested for '{cacheKey}'");

            // Log that this was a preview call and set ClearCache to true, since we don't want caching for previews
            if (isPreview)
            {
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
                isCached = IsCacheValid();
                // Try to get from cache (ASYNC!)
                if (_memCache == null || _memCache.TryGetValue(cacheKey, out returnObj) == false || clearCache)
                {
                    //when cache failed, retrieve via service
                    HttpWebRequest request = HttpWebRequest.CreateHttp($"{Configuration.MediaKiwi.ContentService.ServiceUrl}/getPageContent");
                    request.Method = "POST";
                    request.ContentType = "application/json";
                    request.Timeout = Configuration.MediaKiwi.ContentService.TimeOut;

                    // TODO: needs to be removed for production,
                    // this will now accept all SSL certificates 
                    request.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;

                    string json = string.Empty;
                    using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                    {
                        json = JsonConvert.SerializeObject(new
                        {
                            Path = GetSubSiteURL(forUrl),
                            ClearCache = clearCache,
                            IsPreview = isPreview,
                            PageID = pageId
                        });
                        await streamWriter.WriteAsync(json);
                    }

                    try
                    {
                        // Get the response.
                        WebResponse response = await request.GetResponseAsync();

                        // Get the stream containing content returned by the server.
                        // The using block ensures the stream is automatically closed.
                        using (Stream dataStream = response.GetResponseStream())
                        {
                            // Open the stream using a StreamReader for easy access.
                            StreamReader reader = new StreamReader(dataStream);

                            // Read the content.
                            string responseFromServer = await reader.ReadToEndAsync();

                            // Display the content.
                            returnObj = JsonConvert.DeserializeObject<PageContentResponse>(responseFromServer);

                            // Apply default meta tags
                            // TODO: this function should not exist, this data should come from 
                            // the MediaKiwi API
                            SetMetaInfo(returnObj, forUrl);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "The request was : '{0}'", json);
                    }
                  

                    // Add content to cache if needed
                    if (returnObj?.PageID > 0 && _memCache != null)
                    {
                        AddCache(cacheKey, returnObj);
                    }
                }
                else 
                {
                    _logger.LogInformation($"Got cached content for '{forUrl}' (Page: {returnObj.MetaData?.PageTitle} : {returnObj.PageID})");
                }
            }

            returnObj.IsCached = isCached;
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
                pageDescription = PageContent.MetaData["description"].Value;
            else
                pageDescription = PageContent.MetaData.PageDescription;

            // Facebook 
            if (PageContent.MetaData.ContainsKey("og:title") == false)
                PageContent.MetaData.Add("og:title", PageContent.MetaData.PageTitle, MetaTagRenderKey.PROPERTY);

            if (PageContent.MetaData.ContainsKey("og:description") == false)
                PageContent.MetaData.Add("og:description", pageDescription, MetaTagRenderKey.PROPERTY);

            //if (PageContent.MetaData.ContainsKey("og:image") == false)
            //    PageContent.MetaData.Add("og:image", "https://cdn-demo.supershift.nl/web/header1.jpg", MetaTagRenderKey.PROPERTY);

            if (PageContent.MetaData.ContainsKey("og:url") == false)
                PageContent.MetaData.Add("og:url", url);


            // Twitter
            if (PageContent.MetaData.ContainsKey("twitter:title") == false)
                PageContent.MetaData.Add("twitter:title", PageContent.MetaData.PageTitle);

            if (PageContent.MetaData.ContainsKey("twitter:description") == false)
                PageContent.MetaData.Add("twitter:description", pageDescription);

            //if (PageContent.MetaData.ContainsKey("twitter:image") == false)
            //    PageContent.MetaData.Add("twitter:image", "https://cdn-demo.supershift.nl/web/header1.jpg");

            if (PageContent.MetaData.ContainsKey("twitter:card") == false)
                PageContent.MetaData.Add("twitter:card", "summary_large_image");

            // General
            if (PageContent.MetaData.ContainsKey("description") == false)
                PageContent.MetaData.Add("description", pageDescription);
        }

        #endregion Set Meta Info (TEMPORARY FIX)

        #region Is Page Excluded - HttpRequest

        public bool IsPageExcluded(HttpRequest request)
        {
            if (Configuration.MediaKiwi.ContentService.ExcludePaths == null || Configuration.MediaKiwi.ContentService.ExcludePaths.Count == 0)
                return false;

            // Get URL
            var url = request.GetDisplayUrl().ToLower();
            if (url.Contains("?"))
                url = url.Substring(0, url.IndexOf('?'));
            if (url.Contains("#"))
                url = url.Substring(0, url.IndexOf('#'));

            foreach (var excludeRule in Configuration.MediaKiwi.ContentService.ExcludePaths)
            {
                if (url.Contains(excludeRule.ToLower()))
                    return true;
            }

            return false;
        }

        #endregion Is Page Excluded - HttpRequest

        #region Is Page Excluded - Url

        public bool IsPageExcluded(string forUrl)
        {
            if (Configuration.MediaKiwi.ContentService.ExcludePaths == null || Configuration.MediaKiwi.ContentService.ExcludePaths.Count == 0)
                return false;

            string temp = forUrl.ToLower();

            foreach (var excludeRule in Configuration.MediaKiwi.ContentService.ExcludePaths)
            {
                if (temp.Contains(excludeRule.ToLower()))
                    return true;
            }

            return false;
        }

        #endregion Is Page Excluded - Url

        private string getServiceUrl(string baseUrl, Dictionary<string, string> queryString)
        {
            return QueryHelpers.AddQueryString(baseUrl, queryString).ToString();
        }

        public static DateTime Last_Flush = DateTime.UtcNow;

        public bool IsCacheValid()
        {
            //_logger.LogInformation($"Calling cache server with Ticks : {Last_Flush.Ticks}");
            if (string.IsNullOrWhiteSpace(Configuration.MediaKiwi.ContentService.ServiceUrl))
                return true;

            //when cache failed, retrieve via service
            HttpWebRequest request = HttpWebRequest.CreateHttp($"{Configuration.MediaKiwi.ContentService.ServiceUrl}/flush?now={Last_Flush.Ticks}");
            request.Method = "GET";
            request.ContentType = "application/json";
            request.Timeout = Configuration.MediaKiwi.ContentService.TimeOut;

            // TODO: needs to be removed for production,
            // this will now accept all SSL certificates 
            request.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
            //request.Expect = "application/json";
            
            //try
            //{
                // Get the response.
                WebResponse response = request.GetResponse();
                bool success = false;

                // Get the stream containing content returned by the server.
                // The using block ensures the stream is automatically closed.
                using (Stream dataStream = response.GetResponseStream())
                {
                    // Open the stream using a StreamReader for easy access.
                    StreamReader reader = new StreamReader(dataStream);

                    // Read the content.
                    string responseFromServer = reader.ReadToEnd();
                   // _logger.LogInformation($"Response from caching server : {responseFromServer}");
                    
                    success = responseFromServer.Equals("true", StringComparison.InvariantCultureIgnoreCase);
                    if (success)
                    {
                        FlushCache();
                        Last_Flush = DateTime.UtcNow;
                        return false;
                    }
                }
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogError(ex, ex.Message, null);
            //    FlushCache();
            //    return false;
            //}
            return true;
        }

        public bool PingSucceeded()
        {
            if (string.IsNullOrWhiteSpace(Configuration.MediaKiwi.ContentService.ServiceUrl))
                return false;

            //when cache failed, retrieve via service
            HttpWebRequest request = HttpWebRequest.CreateHttp($"{Configuration.MediaKiwi.ContentService.ServiceUrl}/ping");
            request.Method = "GET";
            request.ContentType = "application/json";
            request.Timeout = Configuration.MediaKiwi.ContentService.TimeOut;

            // TODO: needs to be removed for production,
            // this will now accept all SSL certificates 
            request.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
            //request.Expect = "application/json";

            // Get the response.
            WebResponse response = request.GetResponse();
            bool success = false;

            try
            {
                // Get the stream containing content returned by the server.
                // The using block ensures the stream is automatically closed.
                using (Stream dataStream = response.GetResponseStream())
                {
                    // Open the stream using a StreamReader for easy access.
                    StreamReader reader = new StreamReader(dataStream);

                    // Read the content.
                    string responseFromServer = reader.ReadToEnd();

                    success = responseFromServer.Equals("\"hello\"", StringComparison.InvariantCultureIgnoreCase);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, null);
            }

            return success;
        }
    }
}
