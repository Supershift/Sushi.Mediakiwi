using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using System;

namespace Sushi.Mediakiwi.Headless
{
    public static class Common
    {
        /// <summary>
        /// Returns the Current host from the request, taking
        /// the 'X-Forwarded-Host' header into account.
        /// </summary>
        /// <param name="request">The request for which the host should be retrieved</param>
        /// <returns></returns>
        public static HostString GetHostString(HttpRequest request)
        {
            string host = string.Empty;

            // Try get path from X-Forwarded-Host header
            // Azure FD forwards .azurewebsites to correct domain
            if (string.IsNullOrWhiteSpace(request.Headers["X-Forwarded-Host"]) == false)
            {
                host = request.Headers["X-Forwarded-Host"];
            }
            else
            {
                host = request.Host.ToUriComponent();
            }

            return new HostString(host);
        }


        /// <summary>
        /// Returns the Current PathBase from the request, taking
        /// the 'X-Forwarded-Host' header into account.
        /// </summary>
        /// <param name="request">The request for which the PathBase should be retrieved</param>
        /// <returns></returns>
        public static PathString GetPathBaseString(HttpRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Headers["X-Forwarded-Host"]) && string.IsNullOrWhiteSpace(request.PathBase) == false)
            {
                return new PathString(request.PathBase);
            }
            else
            {
                return PathString.Empty;
            }
        }

        /// <summary>
        /// Returns the Current Scheme from the request, taking
        /// the 'X-Forwarded-Proto' header into account.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static string GetSchemeString(HttpRequest request)
        {
            string scheme = string.Empty;

            // Try get scheme from X-Forwarded-Proto header
            // Azure FD forwards .azurewebsites to correct domain
            if (!string.IsNullOrWhiteSpace(request.Headers["X-Forwarded-Proto"]))
            {
                scheme = request.Headers["X-Forwarded-Proto"];
            }
            // Use the request scheme if scheme is not set by X-Forwarded-Proto, 
            else
            {
                scheme = request.Scheme;
            }

            return scheme;
        }

        /// <summary>
        /// Creates a base url based on the 'X-Forwarded-Proto' and 'X-Forwarded-Host' headers
        /// If one of them isn't present on the request, the fallback request.Host and/or request.Scheme are used
        /// </summary>
        /// <returns>URL in string format</returns>
        public static string GetBaseUrl(HttpRequest request)
        {
            try
            {
                var pathBase = GetPathBaseString(request);
                string scheme = GetSchemeString(request);
                var host = GetHostString(request);

                return UriHelper.BuildAbsolute(scheme, host, pathBase, new PathString("/"));
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Constructs an URL 
        /// </summary>
        /// <param name="request">The request to get the Scheme, Host and BasePath from</param>
        /// <param name="pathAddition">The Path requested</param>
        /// <param name="queryString">The Querystring requested</param>
        /// <returns></returns>
        public static string ConstructUrl(HttpRequest request, PathString pathAddition = default, QueryString queryString = default)
        {
            try
            {
                var pathBase = GetPathBaseString(request);
                string scheme = GetSchemeString(request);
                var host = GetHostString(request);

                return UriHelper.BuildAbsolute(scheme, host, pathBase, pathAddition, queryString);
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Gets the encryption key.
        /// </summary>
        /// <value>The encryption key.</value>
        internal static string EncryptionKey
        {
            get { return "wimserver"; }
        }

        public static string GetPassPhraseFromContext(HttpContext context)
        {
            string returnValue = string.Empty;

            if (context?.Request?.Headers?.ContainsKey("User-Agent") == true)
            {
                returnValue = context.Request.Headers["User-Agent"];
            }
            else if (context?.Request?.Headers?.ContainsKey("Request-Id") == true)
            {
                returnValue = context.Request.Headers["Request-Id"];
            }
            else if (context?.Request?.Host != null && context?.Request?.Host.HasValue == true)
            {
                returnValue = context?.Request?.Host.Value;
            }
            else
            {
                returnValue = EncryptionKey;
            }

            return returnValue;
        }
    }
}
