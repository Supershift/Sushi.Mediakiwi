using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Sushi.Mediakiwi.Headless.Config;
using Sushi.Mediakiwi.Headless.Data;
using System;
using System.ComponentModel;
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
        public event EventHandler OnContentSet;

        /// <summary>
        /// This contains the Original URL request, before any MediaKiwi URL Rewrite has taken place
        /// </summary>
        public string OriginalRequestURL { get; set; }

        [HttpPost]
        [ValidateAntiForgeryToken]
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
                    m_PageContent = ViewData[ContextItemNames.PageContent] as PageContentResponse;

                if (m_PageContent == null)
                    m_PageContent = new PageContentResponse();

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
            var handler = this.OnContentSet;
            if (handler == null)
            {
                return;
            }

            handler(this, new EventArgs());
        }

        public override async Task OnPageHandlerExecutionAsync(PageHandlerExecutingContext context, PageHandlerExecutionDelegate next)
        { 
            // Load config from requestservices
            Configuration = (T)context.HttpContext.RequestServices.GetService<ISushiApplicationSettings>();

            // If we landed here from the rewrite rule, we should already have the pagecontent
            // in the request items
            if (context.HttpContext.Items.ContainsKey(ContextItemNames.PageContent))
                PageContent = context.HttpContext.Items[ContextItemNames.PageContent] as PageContentResponse;

            // We dont have PageContent yet, call the service
            if (contentService != null && PageContent == null)
                PageContent = await contentService.GetPageContentAsync(context.HttpContext.Request);

            // Set PreviewMode
            IsPreview = context.HttpContext.Request.IsPreviewCall();

            // Set clearCache
            ClearCache = context.HttpContext.Request.IsClearCacheCall();

            // Set referrer from header
            if (context.HttpContext.Request.Headers.ContainsKey(HttpHeaderNames.OriginalRequestURL))
                OriginalRequestURL = context.HttpContext.Request.Headers[HttpHeaderNames.OriginalRequestURL].ToString();

            // Check if we have an action to do
            NotifyContentSet();

            await base.OnPageHandlerExecutionAsync(context, next);
        }
    }
}
