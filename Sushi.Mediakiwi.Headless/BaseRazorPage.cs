using Microsoft.AspNetCore.Mvc.RazorPages;
using Sushi.Mediakiwi.Headless.Data;

namespace Sushi.Mediakiwi.Headless
{
    public abstract class BaseRazorPage : PageBase
    {
        //public string SiteBasePath { get { return $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Host}{(HttpContext.Request.Host.Port != 80 && HttpContext.Request.Host.Port != 443 ? $":{HttpContext.Request.Host.Port}" : "")}"; } }
       
        private PageContentResponse m_PageContent;
        public PageContentResponse PageContent
        {
            get
            {
                if (m_PageContent == null && HttpContext.Items.ContainsKey(ContextItemNames.PageContent) == true)
                    m_PageContent = HttpContext.Items[ContextItemNames.PageContent] as PageContentResponse;

                return m_PageContent;
            }
            private set
            {
                m_PageContent = value;
                HttpContext.Items[ContextItemNames.PageContent] = m_PageContent;
            }
        }

        public BaseRazorPage()
        {
        }
    }
}
