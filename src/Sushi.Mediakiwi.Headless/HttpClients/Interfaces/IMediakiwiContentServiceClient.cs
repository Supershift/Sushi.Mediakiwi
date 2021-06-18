using System;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Headless.HttpClients.Interfaces
{
    public interface IMediakiwiContentServiceClient
    {
        public Task<string> GetPageNotFoundContent(int? siteId);
        public Task<string> GetPageNotFoundContent(int? siteId, bool clearCache);

        public Task<string> GetPageContentStringAsync(string forUrl);
        public Task<string> GetPageContentStringAsync(string forUrl, string basePath);
        public Task<string> GetPageContentStringAsync(string forUrl, string basePath, bool clearCache);
        public Task<string> GetPageContentStringAsync(string forUrl, string basePath, bool clearCache, bool isPreview);
        public Task<string> GetPageContentStringAsync(string forUrl, string basePath, bool clearCache, bool isPreview, int? pageId);


        public Task<bool> GetCacheValidAsync(DateTime lastFlush);


        public Task<bool> PingAsync();
    }
}
