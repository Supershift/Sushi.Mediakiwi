using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Filters;
using Sushi.Mediakiwi.API.Extensions;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.API.Filters
{
    public class MediakiwiConsoleFilter : IAsyncActionFilter
    {
        protected IHostingEnvironment environment { get; private set; }

        public MediakiwiConsoleFilter(IHostingEnvironment _env) 
        {
            environment = _env;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var contextCopy = context.HttpContext.Clone();
            Beta.GeneratedCms.Console console = default;

            if (context.HttpContext.Request.Headers.TryGetValue(Common.API_HEADER_URL, out Microsoft.Extensions.Primitives.StringValues setUrl))
            {
                // Construct absolute URL from the relative url received
                string schemeString = context.HttpContext.Request.Scheme;
                var hostString  = context.HttpContext.Request.Host;
                var pathBaseString = context.HttpContext.Request.PathBase;
                var pathString = new Microsoft.AspNetCore.Http.PathString(setUrl.ToString().Contains("?") ? setUrl.ToString().Split('?')[0] : setUrl.ToString());
                var query = setUrl.ToString().Contains("?") ? setUrl.ToString().Split('?')[1] : "";

                // Add proxy option
                if (string.IsNullOrWhiteSpace(context.HttpContext.Request.Headers["X-Forwarded-Host"]) == false)
                {
                    hostString = new Microsoft.AspNetCore.Http.HostString(context.HttpContext.Request.Headers["X-Forwarded-Host"]);
                }
        
                // Adjust the url for the context copy
                contextCopy.Request.Path = new Microsoft.AspNetCore.Http.PathString(setUrl);

                // try to create a Console with the 'fake' path
                console = new Beta.GeneratedCms.Console(contextCopy, environment);

                // Create an URL resolver
                UrlResolver resolver = new UrlResolver(context.HttpContext.RequestServices, console);

                // Resolve the supplied URL 
                await resolver.ResolveUrlAsync(schemeString, hostString, pathBaseString, pathString, query).ConfigureAwait(false);

                // Add the resolver to the HttpContext
                context.HttpContext.Items.Add(Common.API_HTTPCONTEXT_URLRESOLVER, resolver);
                
            }

            // Add the console to the HttpContext
            context.HttpContext.Items.Add(Common.API_HTTPCONTEXT_CONSOLE, console);
            
            await next.Invoke();
        }
    }
}
