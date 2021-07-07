using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Sushi.Mediakiwi.Headless.Config;
using System;
using System.Linq;

namespace Sushi.Mediakiwi.Headless
{
    public class MediaKiwiURLMapper
    {
        private ISushiApplicationSettings Configuration { get; set; }

        public MediaKiwiURLMapper(ISushiApplicationSettings configuration)
        {
            Configuration = configuration;
        }

        public MediaKiwiURLMapper(IServiceProvider serviceProvider)
        {
            Configuration = serviceProvider.GetService<ISushiApplicationSettings>();
        }

        /// <summary>
        /// Applies the correct items to the HttpContext. In the case of /faq/{topic} there will be
        /// a HttpContext item added with the name 'topic' and the value provided in that portion of the URL
        /// </summary>
        /// <param name="request">The current HTTP request</param>
        /// <returns>a PageID when there is a match found</returns>
        public int? ApplyContextValues(HttpRequest request)
        {
            // Get URL
            var url = request.GetDisplayUrl();
            if (url.Contains("?"))
                url = url.Substring(0, url.IndexOf('?'));
            if (url.Contains("#"))
                url = url.Substring(0, url.IndexOf('#'));

            if (Configuration?.MediaKiwi?.URLMappings?.Count > 0)
            {
                foreach (var item in Configuration.MediaKiwi.URLMappings.OrderByDescending(x => x.Rex?.GetGroupNames()?.Length).ThenByDescending(x => x.URLMatch.Length))
                {
                    if (item.Rex.IsMatch(url))
                    {
                        var match = item.Rex.Match(url);
                        if (match.Groups.Count > 1)
                        {
                            foreach (var groupName in match.Groups.Keys)
                            {
                                if (groupName != "0")
                                {
                                    request.HttpContext.Items.Add(groupName, match.Groups[groupName].Value);
                                }
                            }
                        }

                        return item.PageID;
                    }
                }
            }
            return null;
        }
    }

    
}
