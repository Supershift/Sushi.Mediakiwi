using Microsoft.AspNetCore.Mvc;
using Sushi.Mediakiwi.API.Filters;
using System;

namespace Sushi.Mediakiwi.API.Controllers
{
    [TypeFilter(typeof(MediakiwiConsoleFilter))]
    public abstract class BaseMediakiwiApiController : ControllerBase
    {
        protected Data.IApplicationUser MediakiwiUser
        {
            get
            {
                if (HttpContext.Items.ContainsKey(Common.API_USER_CONTEXT))
                {
                    return HttpContext.Items[Common.API_USER_CONTEXT] as Data.IApplicationUser;
                }
                else
                {
                    return null;
                }
            }
        }

        public Beta.GeneratedCms.Console Console
        {
            get 
            {
                if (HttpContext.Items.ContainsKey(Common.API_HTTPCONTEXT_CONSOLE))
                {
                    return HttpContext.Items[Common.API_HTTPCONTEXT_CONSOLE] as Beta.GeneratedCms.Console;
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
                if (HttpContext.Items.ContainsKey(Common.API_HTTPCONTEXT_URLRESOLVER))
                {
                    return HttpContext.Items[Common.API_HTTPCONTEXT_URLRESOLVER] as UrlResolver;
                }
                else
                {
                    return new UrlResolver(HttpContext.RequestServices, Console);
                }
            }
        }

    }
}
