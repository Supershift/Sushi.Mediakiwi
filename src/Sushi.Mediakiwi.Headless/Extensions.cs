using Microsoft.AspNetCore.Http;
using Sushi.Mediakiwi.Headless.Data;

namespace Sushi.Mediakiwi.Headless
{
    public struct ContextItemNames
    {
        public static string PageContent { get; set; } = "PageContent";
        public static string ScriptTags { get; set; } = "ScriptTags";
    }

    public struct HttpHeaderNames
    {
        /// <summary>
        /// This would be the URL before any MediaKiwi URL redirect was done
        /// </summary>
        public static readonly string OriginalRequestURL = "x-mediakiwi-originalrequesturl";

        /// <summary>
        /// This would be the URL before the ContentService was called to transform the URL
        /// </summary>
        public static readonly string FullRequestURL = "x-mediakiwi-fullrequesturl";

        /// <summary>
        /// The resolved MediaKiwi Page ID
        /// </summary>
        public static readonly string PageId = "x-mediakiwi-pageid";

        public static readonly string TimeSpend = "x-mk-timed";
        public static readonly string CachedData = "x-mk-cached";
        public static readonly string CacheInvalidData = "x-mk-cache-invalid";
    }

    public static class Extensions
    {
        public static bool IsClearCacheCall(this HttpRequest request)
        {
            if (request != null && request.Query["flush"] == "me")
                return true;
            else if (request != null && request?.HttpContext?.Items?.ContainsKey("mediakiwi.flush") == true)
                return true;
            else
                return false;
        }

        public static bool IsPreviewCall(this HttpRequest request)
        {
            if (request != null && request.Query["preview"] == "1")
                return true;
            else
                return false;
        }

        public static bool? ParseBoolean(this ContentItem item)
        {
            if (item?.Text == "1" || item?.Text == "0")
                return item.Text == "1";
            return null;
        }
    }
}
