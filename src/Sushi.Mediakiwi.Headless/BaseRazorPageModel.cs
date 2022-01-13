using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Sushi.Mediakiwi.Headless.Config;
using Sushi.Mediakiwi.Headless.Data;
using System;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Headless
{
    public class BaseRazorPageModel<T> : PageModel where T : ISushiApplicationSettings 
    {
        /// <summary>
        /// Is this a Clear Cache call ?
        /// </summary>
        public bool ClearCache { get; set; }

        /// <summary>
        /// Is this a Preview mode call ?
        /// </summary>
        public bool IsPreview { get; set; }

        /// <summary>
        /// Contains the application configuration settings
        /// </summary>
        public T Configuration { get; set; }

        /// <summary>
        /// Triggered when the PageContent is set, just before continueing to the next middleware 
        /// </summary>
        public event Func<PageHandlerExecutingContext, EventArgs, Task> OnContentSetAsync;

        /// <summary>
        /// Triggered when the PageContent is set, just before continueing to the next middleware 
        /// </summary>
        public event EventHandler OnContentSet;

        /// <summary>
        /// This contains the Original URL request, before any MediaKiwi URL Rewrite has taken place
        /// </summary>
        public string OriginalRequestURL { get; set; }

        [HttpPost]
        //[MR:03-12-2020] commented because 1) its not possible to use resopnse caching in combination with AntiForgery
        // and 2) its not being used at the moment
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostGetJsonContent()
        {
            return Content(JsonConvert.SerializeObject(PageContent), "application/json");
        }

        private IMediaKiwiContentService contentService;

        public BaseRazorPageModel(IMediaKiwiContentService _service)
        {
            contentService = _service;
        }

        private PageContentResponse m_PageContent;
        public PageContentResponse PageContent
        {
            get 
            {
                if (m_PageContent == null && ViewData?.ContainsKey(ContextItemNames.PageContent) == true)
                {
                    m_PageContent = ViewData[ContextItemNames.PageContent] as PageContentResponse;
                }

                if (m_PageContent == null)
                {
                    m_PageContent = new PageContentResponse();
                }

                return m_PageContent;
            }
            set
            {
                m_PageContent = value;
                ViewData[ContextItemNames.PageContent] = m_PageContent;
            }
        }

        public void SetMetaInfo(string name, string content, MetaTagRenderKey renderKey = MetaTagRenderKey.NAME)
        {
            PageContent.MetaData.Add(name, content, renderKey, true);
        }

        protected void NotifyContentSet()
        {
            var handler = OnContentSet;
            if (handler == null)
            {
                return;
            }

            handler(this, new EventArgs());
        }

        async protected Task NotifyContentSetAsync(PageHandlerExecutingContext context)
        {
            Func<PageHandlerExecutingContext, EventArgs, Task> handler = OnContentSetAsync;

            if (handler == null)
            {
                return;
            }

            Delegate[] invocationList = handler.GetInvocationList();
            Task[] handlerTasks = new Task[invocationList.Length];

            for (int i = 0; i < invocationList.Length; i++)
            {
                handlerTasks[i] = ((Func<PageHandlerExecutingContext, EventArgs, Task>)invocationList[i])(context, EventArgs.Empty);
            }

            await Task.WhenAll(handlerTasks).ConfigureAwait(false);
        }

        public async override Task OnPageHandlerExecutionAsync(PageHandlerExecutingContext context, PageHandlerExecutionDelegate next)
        { 
            // Load config from requestservices
            Configuration = (T)context.HttpContext.RequestServices.GetService<ISushiApplicationSettings>();

            // If we landed here from the rewrite rule, we should already have the pagecontent
            // in the request items
            if (context.HttpContext.Items.ContainsKey(ContextItemNames.PageContent))
            {
                PageContent = context.HttpContext.Items[ContextItemNames.PageContent] as PageContentResponse;
            }

            // We dont have PageContent yet, call the service
            if (contentService != null && PageContent == null)
            {
                PageContent = await contentService.GetPageContentAsync(context.HttpContext.Request).ConfigureAwait(false);
            }

            // Set PreviewMode
            IsPreview = (PageContent?.InternalInfo?.IsPreview == true || context.HttpContext.Request.IsPreviewCall());

            // Set clearCache
            ClearCache = (PageContent?.InternalInfo?.ClearCache == true || context.HttpContext.Request.IsClearCacheCall());

            // Set referrer from header
            if (context.HttpContext.Request.Headers.ContainsKey(HttpHeaderNames.OriginalRequestURL))
            {
                OriginalRequestURL = context.HttpContext.Request.Headers[HttpHeaderNames.OriginalRequestURL].ToString();
            }

            // Check if we have an action to do (Async)
            await NotifyContentSetAsync(context).ConfigureAwait(false);

            // Check if we have an action to do (Sync)
            NotifyContentSet();

            await base.OnPageHandlerExecutionAsync(context, next);
        }
    }
}
