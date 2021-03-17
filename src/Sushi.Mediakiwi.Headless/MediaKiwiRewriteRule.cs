using Microsoft.AspNetCore.Rewrite;
using System;
using Microsoft.Extensions.DependencyInjection;
using Sushi.Mediakiwi.Headless.Config;
using Sushi.Mediakiwi.Headless.Data;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http.Extensions;

namespace Sushi.Mediakiwi.Headless
{
    public class MediaKiwiRewriteRule : IRule
    {
        private IMediaKiwiContentService ContentService;
        private ISushiApplicationSettings Configuration;
        private readonly ILogger _logger;

        public MediaKiwiRewriteRule(IServiceProvider serviceProvider)
        {
            var _loggerFactory = serviceProvider.GetService<ILoggerFactory>();
            _logger = _loggerFactory.CreateLogger<MediaKiwiRewriteRule>();

            ContentService = serviceProvider.GetService<IMediaKiwiContentService>();
            Configuration = serviceProvider.GetService<ISushiApplicationSettings>();
        }

        public void ApplyRule(RewriteContext context)
        {
            if (context == null)
                _logger.LogError("RewriteContext IS NULL, skipping the MK Rewrite rule");
            else if (context?.HttpContext == null)
                _logger.LogError("HttpContext IS NULL, skipping the MK Rewrite rule");
            else if (context?.HttpContext?.Request == null)
                _logger.LogError("HttpContext Request IS NULL, skipping the MK Rewrite rule");
            else if (context?.HttpContext?.Response == null)
                _logger.LogError("HttpContext Response IS NULL, skipping the MK Rewrite rule");
            else if (Configuration == null)
                _logger.LogError("Configuration IS NULL, skipping the MK Rewrite rule");
            else if (ContentService == null)
                _logger.LogError("ContentService IS NULL, skipping the MK Rewrite rule");
            else
            {
                var request = context.HttpContext.Request;
                var response = context.HttpContext.Response;

                // Create PageMapper
                MediaKiwiURLMapper mapper = new MediaKiwiURLMapper(Configuration);
                int? PageId = mapper.ApplyContextValues(request);

                if (ContentService != null)
                {
                    // Get Pagecontent from service
                    PageContentResponse PageContent = new PageContentResponse();

                    var dt1 = DateTime.UtcNow;

                    try
                    {
                        if (PageId.GetValueOrDefault(0) > 0)
                            PageContent = ContentService.GetPageContentAsync(null, request.IsClearCacheCall(), request.IsPreviewCall(), PageId.Value).ConfigureAwait(true).GetAwaiter().GetResult();
                        else
                            PageContent = ContentService.GetPageContentAsync(request).ConfigureAwait(true).GetAwaiter().GetResult();
                    }
                    catch(Exception ex)
                    {
                        _logger.LogError(ex.Message);
                    }

                    var dt2 = DateTime.UtcNow;

                    response.Headers.Add(HttpHeaderNames.TimeSpend,new TimeSpan(dt2.Ticks - dt1.Ticks).TotalMilliseconds.ToString());
                    response.Headers.Add(HttpHeaderNames.CachedData, PageContent.IsCached.ToString());
                    response.Headers.Add(HttpHeaderNames.CacheInvalidData, PageContent.IsCacheInvalidated.ToString());
                    
                    if (!PageContent.IsCached)
                        context.HttpContext.Items["mediakiwi.flush"] = "me";

                    // Set internal info in the Page
                    if (PageContent == null)
                    {
                        PageContent = new PageContentResponse();
                    }

                    // apply the proper status code based on the headless result
                    response.StatusCode = (int)PageContent.StatusCode;

                    // act upon status codes 301 & 302 resulting in a location change.
                    if (PageContent.StatusCode == System.Net.HttpStatusCode.Moved
                        || PageContent.StatusCode == System.Net.HttpStatusCode.MovedPermanently
                        || PageContent.StatusCode == System.Net.HttpStatusCode.Found
                        || PageContent.StatusCode == System.Net.HttpStatusCode.Redirect
                        )
                    {
                        // apply the new location
                        response.Headers.Add("Location", PageContent.PageInternalPath);
                    }

                    PageContent.InternalInfo.ClearCache = request.IsClearCacheCall();
                    PageContent.InternalInfo.IsPreview = request.IsPreviewCall();

                    // Set internal info in the Component
                    if (PageContent?.Components?.Count > 0)
                    {
                        foreach (var item in PageContent.Components)
                            item.InternalInfo = PageContent.InternalInfo;
                    }

                    // We have a result.
                    if (PageContent?.PageID > 0)
                    {
                        request.RouteValues["pageid"] = PageContent.PageID.ToString();
                        response.Headers.Add(HttpHeaderNames.PageId, PageContent.PageID.ToString());

                        // When we have this, this is a redirect so add our current URL as referrer
                        if (PageId.GetValueOrDefault(0) > 0)
                            request.Headers[HttpHeaderNames.OriginalRequestURL] = request.GetEncodedUrl();

                        if (context.HttpContext.Items.ContainsKey(ContextItemNames.PageContent) == false)
                            request.HttpContext.Items.Add(ContextItemNames.PageContent, PageContent);

                        if (response.Headers.ContainsKey(HttpHeaderNames.FullRequestURL) == false)
                            response.Headers.Add(HttpHeaderNames.FullRequestURL, request.GetEncodedUrl());

                        if (string.IsNullOrWhiteSpace(PageContent?.PageLocation) == false)
                        {
                            request.RouteValues["page"] = $"/{PageContent.PageLocation}";
                            request.Path = $"/{PageContent.PageLocation}";
                            response.StatusCode = (int)PageContent.StatusCode;

                            context.Result = RuleResult.SkipRemainingRules;
                        }
                    }
                }
                else
                {
                    context.Result = RuleResult.ContinueRules;
                }
            }
        }
    }
}