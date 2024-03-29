﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sushi.Mediakiwi.Headless.Config;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Headless.HttpClients
{
    public class MediakiwiContentServiceClient : Interfaces.IMediakiwiContentServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly ISushiApplicationSettings _settings;
        private readonly ILogger _logger;

        public MediakiwiContentServiceClient(HttpClient client, IServiceProvider serviceProvider = null)
        {
            _httpClient = client;

            if (serviceProvider != null)
            {
                _settings = serviceProvider.GetService<ISushiApplicationSettings>();
                var _loggerFactory = serviceProvider.GetService<ILoggerFactory>();
                if (_loggerFactory != null)
                {
                    _logger = _loggerFactory.CreateLogger<MediakiwiContentServiceClient>();
                }
            }
        }

        private string getServiceUrl(string baseUrl, Dictionary<string, string> queryString)
        {
            return QueryHelpers.AddQueryString(baseUrl, queryString);
        }

        #region Get Page Not Found

        public async Task<string> GetPageNotFoundContent(int? siteId)
        {
            return await GetPageNotFoundContent(siteId, false).ConfigureAwait(false);
        }

        public async Task<string> GetPageNotFoundContent(int? siteId, bool clearCache)
        {
            CancellationTokenSource cts = new CancellationTokenSource(_settings.MediaKiwi.ContentService.TimeOut); // 2 seconds timeout

            // Create querystring for adding SiteID to the Request
            Dictionary<string, string> queryString = new Dictionary<string, string>();
            if (siteId.GetValueOrDefault(0) > 0)
            {
                queryString.Add("siteId", siteId.GetValueOrDefault(0).ToString());
            }

            // Create Http Request object
            var request = new HttpRequestMessage(HttpMethod.Get, getServiceUrl($"{_settings.MediaKiwi.ContentService.ServiceUrl}/page/notfound", queryString));
            request.Headers.Accept.Add(System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse("application/json"));

            HttpResponseMessage response = new HttpResponseMessage(System.Net.HttpStatusCode.NotFound);

            // Retrieve content via service
            try
            {
                response = await _httpClient.SendAsync(request, cts.Token).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, null);
            }


            if (response.IsSuccessStatusCode == true)
            {
                return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            }
            else
            {
                _logger.LogInformation($"Method '{request.Method.Method}' to '{request.RequestUri}' failed with StatusCode {response.StatusCode}");
                return string.Empty;
            }
        }

        #endregion Get Page Not Found

        #region Get Page Content (String output)

        public async Task<string> GetPageContentStringAsync(string forUrl)
        {
            return await GetPageContentStringAsync(forUrl, null).ConfigureAwait(false);
        }

        public async Task<string> GetPageContentStringAsync(string forUrl, string basePath)
        {
            return await GetPageContentStringAsync(forUrl, basePath, false).ConfigureAwait(false);
        }

        public async Task<string> GetPageContentStringAsync(string forUrl, string basePath, bool clearCache)
        {
            return await GetPageContentStringAsync(forUrl, basePath, clearCache, false).ConfigureAwait(false);
        }

        public async Task<string> GetPageContentStringAsync(string forUrl, string basePath, bool clearCache, bool isPreview)
        {
            return await GetPageContentStringAsync(forUrl, basePath, clearCache, isPreview, null).ConfigureAwait(false);
        }

        public async Task<string> GetPageContentStringAsync(string forUrl, string basePath, bool clearCache, bool isPreview, int? pageId)
        {
            return await GetPageContentStringAsync(forUrl, basePath, clearCache, isPreview, null, null).ConfigureAwait(false);

        }

        public async Task<string> GetPageContentStringAsync(string forUrl, string basePath, bool clearCache, bool isPreview, int? pageId, IQueryCollection queryCollection)
        { 
            CancellationTokenSource cts = new CancellationTokenSource(_settings.MediaKiwi.ContentService.TimeOut); // 2 seconds timeout

            // Create querystring for adding SiteID to the Request
            Dictionary<string, string> queryString = new Dictionary<string, string>();
            if (!string.IsNullOrWhiteSpace(forUrl))
            {
                queryString.Add("url", Uri.EscapeUriString(forUrl));
            }

            if (clearCache)
            {
                queryString.Add("flush", "me");
            }

            if (isPreview)
            {
                queryString.Add("preview", "1");
            }

            if (pageId.GetValueOrDefault(0) > 0)
            {
                queryString.Add("pageId", pageId.GetValueOrDefault(0).ToString());
            }

            // Add remaining items from queryCollection to the queryString
            if (queryCollection != null)
            {
                foreach(var query in queryCollection)
                {
                    if(queryString.ContainsKey(query.Key) == false)
                    {
                        queryString.Add(query.Key, query.Value);
                    }
                }
            }

            // Create Http Request object
            var request = new HttpRequestMessage(HttpMethod.Get, getServiceUrl($"{_settings.MediaKiwi.ContentService.ServiceUrl}/page/content", queryString));
            request.Headers.Accept.Add(System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse("application/json"));

            HttpResponseMessage response = new HttpResponseMessage(System.Net.HttpStatusCode.NotFound);
            // Retrieve content via service
            try
            {
                response = await _httpClient.SendAsync(request, cts.Token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            }
            else
            {
                _logger.LogInformation($"Method '{request.Method.Method}' to '{request.RequestUri}' failed with StatusCode {response.StatusCode}");
                return string.Empty;
            }

        }

        #endregion Get Page Content (String output)

        #region Get Cache Valid

        public async Task<bool> GetCacheValidAsync(DateTime lastFlush)
        {
            CancellationTokenSource cts = new CancellationTokenSource(_settings.MediaKiwi.ContentService.TimeOut); // 2 seconds timeout

            // Create Http Request object
            var request = new HttpRequestMessage(HttpMethod.Get, $"{_settings.MediaKiwi.ContentService.ServiceUrl}/flush?now={lastFlush.Ticks}");
            request.Headers.Accept.Add(System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse("application/json"));

            // Retrieve content via service
            using var httpResponse = await _httpClient.SendAsync(request, cts.Token).ConfigureAwait(false);

            if (httpResponse.IsSuccessStatusCode)
            {
                var responseFromServer = await httpResponse.Content.ReadAsStringAsync();
                return responseFromServer.Equals("true", StringComparison.InvariantCultureIgnoreCase);
            }
            else
            {
                return false;
            }
        }

        #endregion Get Cache Valid

        #region Ping

        public async Task<bool> PingAsync()
        {
            CancellationTokenSource cts = new CancellationTokenSource(_settings.MediaKiwi.ContentService.TimeOut); // 2 seconds timeout

            // Create Http Request object
            var request = new HttpRequestMessage(HttpMethod.Get, $"{_settings.MediaKiwi.ContentService.ServiceUrl}/ping");
            request.Headers.Accept.Add(System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse("application/json"));

            // Retrieve Content via service
            using var httpResponse = await _httpClient.SendAsync(request, cts.Token).ConfigureAwait(false);

            if (httpResponse.IsSuccessStatusCode)
            {
                var responseFromServer = await httpResponse.Content.ReadAsStringAsync();
                return responseFromServer.Equals("\"hello\"", StringComparison.InvariantCultureIgnoreCase);
            }
            else
            {
                return false;
            }
        }

        #endregion Ping    
    }
}
