using Microsoft.AspNetCore.Http;
using Sushi.Mediakiwi.Headless.Data;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Headless
{
    public interface IMediaKiwiContentService
    {
        /// <summary>
        /// Gets the content from memorycache or webservice
        /// </summary>
        /// <param name="forUrl">The url for which to get the content</param>
        /// <param name="basePath">The domain and basepath for which to get the content</param>
        /// <param name="clearCache">Get renewed content from service, skip caching</param>
        /// <param name="isPreview">This is a preview call, probably from the CMS</param>
        /// <param name="pageId">Get this page ID, will precede over the URL !</param>
        /// <returns></returns>
        public Task<PageContentResponse> GetPageContentAsync(string forUrl, string basePath, bool clearCache = false, bool isPreview = false, int? pageId = null);

        /// <summary>
        /// Gets the content from memorycache or webservice
        /// </summary>
        /// <param name="request">The Http Request object for which to get the content</param>
        /// <returns></returns>
        public Task<PageContentResponse> GetPageContentAsync(HttpRequest request);

        /// <summary>
        /// If the suppliewd request is in the EXCLUDED settings for this service,
        /// TRUE will be returned. If it's not excluded, FALSE will be returned
        /// </summary>
        /// <param name="request">The Http Request object to determine</param>
        /// <returns></returns>
        public bool IsPageExcluded(HttpRequest request);

        /// <summary>
        /// If the suppliewd URL is in the EXCLUDED settings for this service,
        /// TRUE will be returned. If it's not excluded, FALSE will be returned
        /// </summary>
        /// <param name="forUrl">The Url to determine</param>
        /// <returns></returns>
        public bool IsPageExcluded(string forUrl);

        /// <summary>
        /// This executes the Ping endpoint on the controller and returns true if a succesful connection 
        /// was made to the Ping endpoint.
        /// </summary>
        public Task<bool> PingSucceeded();
    }
}
