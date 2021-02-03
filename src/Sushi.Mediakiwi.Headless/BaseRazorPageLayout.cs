using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Razor.Internal;
using Sushi.Mediakiwi.Headless.Config;
using Sushi.Mediakiwi.Headless.Data;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Headless
{
    public abstract class BaseRazorPageLayout<C> : RazorPage where C : ISushiApplicationSettings
    {
        [RazorInject]
        public C Configuration { get; set; }

        private PageContentResponse m_PageContent;
        public PageContentResponse PageContent
        {
            get
            {
                if (m_PageContent == null && Context.Items.ContainsKey(ContextItemNames.PageContent) == true)
                    m_PageContent = Context.Items[ContextItemNames.PageContent] as PageContentResponse;

                return m_PageContent;
            }
            private set
            {
                m_PageContent = value;
                Context.Items[ContextItemNames.PageContent] = m_PageContent;
            }
        }

        public void SetMetaInfo(string name, string content, MetaTagRenderKey renderKey = MetaTagRenderKey.NAME)
        {
            PageContent.MetaData.Add(name, content, renderKey, true);
        }
    }

    public abstract class BaseRazorPageLayout<T, C> : RazorPage<T> where C : ISushiApplicationSettings
    {
        [RazorInject]
        public C Configuration { get; set; }

        private PageContentResponse m_PageContent;
        public PageContentResponse PageContent
        {
            get
            {
                if (m_PageContent == null && Context.Items.ContainsKey(ContextItemNames.PageContent) == true)
                    m_PageContent = Context.Items[ContextItemNames.PageContent] as PageContentResponse;

                return m_PageContent;
            }
            private set
            {
                m_PageContent = value;
                Context.Items[ContextItemNames.PageContent] = m_PageContent;
            }
        }

        public void SetMetaInfo(string name, string content, MetaTagRenderKey renderKey = MetaTagRenderKey.NAME)
        {
            PageContent.MetaData.Add(name, content, renderKey, true);
        }

    }
}
