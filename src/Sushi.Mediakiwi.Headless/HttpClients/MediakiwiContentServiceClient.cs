using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sushi.Mediakiwi.Headless.Config;
using Sushi.Mediakiwi.Headless.HttpClients.Data;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Headless.HttpClients
{
    public class MediakiwiContentServiceClient : Interfaces.IMediakiwiContentServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly ISushiApplicationSettings _settings;
        private ILogger _logger;

        public MediakiwiContentServiceClient(HttpClient client, IServiceProvider serviceProvider = null)
        {
            _httpClient = client;

            if (serviceProvider != null)
            {
                _settings = serviceProvider.GetService<ISushiApplicationSettings>();
                var _loggerFactory = serviceProvider.GetService<ILoggerFactory>();
                if (_loggerFactory != null)
                    _logger = _loggerFactory.CreateLogger<MediakiwiContentServiceClient>();
            }
        }

        private string getServiceUrl(string baseUrl, Dictionary<string, string> queryString)
        {
            return QueryHelpers.AddQueryString(baseUrl, queryString).ToString();
        }

        #region Get Page Not Found

        public async Task<string> GetPageNotFoundContent(int? siteId)
        {
            return await GetPageNotFoundContent(siteId, false);
        }

        public async Task<string> GetPageNotFoundContent(int? siteId, bool clearCache)
        {
            CancellationTokenSource cts = new CancellationTokenSource(_settings.MediaKiwi.ContentService.TimeOut); // 2 seconds timeout

            // Create querystring for adding SiteID to the Request
            Dictionary<string, string> queryString = new Dictionary<string, string>();
            if (siteId.GetValueOrDefault(0) > 0)
                queryString.Add("siteId", siteId.GetValueOrDefault(0).ToString());

            // Retrieve content via service
            var response = await _httpClient.PostAsync(getServiceUrl($"{_settings.MediaKiwi.ContentService.ServiceUrl}/getPageNotFoundContent", queryString), null, cts.Token).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
            else
            {
                return string.Empty;
            }
        }

        #endregion Get Page Not Found

        #region Get Page Content (String output)

        public async Task<string> GetPageContentStringAsync(string forUrl)
        {
            return await GetPageContentStringAsync(forUrl, null);
        }

        public async Task<string> GetPageContentStringAsync(string forUrl, string basePath)
        {
            return await GetPageContentStringAsync(forUrl, basePath, false);
        }

        public async Task<string> GetPageContentStringAsync(string forUrl, string basePath, bool clearCache)
        {
            return await GetPageContentStringAsync(forUrl, basePath, clearCache, false);
        }

        public async Task<string> GetPageContentStringAsync(string forUrl, string basePath, bool clearCache, bool isPreview)
        {
            return await GetPageContentStringAsync(forUrl, basePath, clearCache, isPreview, null);
        }

        public async Task<string> GetPageContentStringAsync(string forUrl, string basePath, bool clearCache, bool isPreview, int? pageId)
        {
            CancellationTokenSource cts = new CancellationTokenSource(_settings.MediaKiwi.ContentService.TimeOut); // 2 seconds timeout

            // Create Request to Post
            GetPageContentRequest requestObj = new GetPageContentRequest() {
                ClearCache = clearCache,
                IsPreview = isPreview,
                PageID = pageId,
                Path = forUrl,
                Domain = basePath
            };

            // Create Http Request object
            var request = new HttpRequestMessage(HttpMethod.Post, $"{_settings.MediaKiwi.ContentService.ServiceUrl}/getPageContent");
            request.Headers.Accept.Add(System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse("application/json"));
            request.Content = new StringContent(JsonSerializer.Serialize(requestObj), Encoding.UTF8, "application/json");

            // Retrieve content via service
            using var httpResponse = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead, cts.Token).ConfigureAwait(false);

            if (httpResponse.IsSuccessStatusCode)
            {
                return await httpResponse.Content.ReadAsStringAsync();
            }
            else
            {
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
            using var httpResponse = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead, cts.Token).ConfigureAwait(false);

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
            using var httpResponse = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead, cts.Token).ConfigureAwait(false);

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
