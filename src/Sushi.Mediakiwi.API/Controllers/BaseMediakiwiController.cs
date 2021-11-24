using Microsoft.AspNetCore.Mvc;

namespace Sushi.Mediakiwi.API.Controllers
{
    [TypeFilter(typeof(IMediakiwiConsoleFilter))]
    public abstract class BaseMediakiwiController : ControllerBase
    {
        public Beta.GeneratedCms.Console Console
        {
            get 
            {
                if (HttpContext.Items.ContainsKey("MKConsole"))
                {
                    return HttpContext.Items["MKConsole"] as Beta.GeneratedCms.Console;
                }
                else 
                {
                    return null;
                }
            }
        }

        public UrlResolver Resolver
        {
            get
            {
                if (HttpContext.Items.ContainsKey("MKUrlResolver"))
                {
                    return HttpContext.Items["MKUrlResolver"] as UrlResolver;
                }
                else
                {
                    return new UrlResolver(HttpContext.RequestServices);
                }
            }
        }

    }
}
