using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Filters;
using Sushi.Mediakiwi.API.Extensions;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.API
{
    public class IMediakiwiConsoleFilter : IAsyncActionFilter
    {
        protected IHostingEnvironment environment { get; private set; }

        public IMediakiwiConsoleFilter(IHostingEnvironment _env) 
        {
            environment = _env;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var contextCopy = context.HttpContext.Clone(false);

            if (context.HttpContext.Request.Headers.TryGetValue("original-url", out Microsoft.Extensions.Primitives.StringValues setUrl))
            {
                // Construct absolute URL from the relative url received
                string schemeString = context.HttpContext.Request.Scheme;
                var hostString  = context.HttpContext.Request.Host;
                var pathBaseString = context.HttpContext.Request.PathBase;
                var pathString = new Microsoft.AspNetCore.Http.PathString(setUrl);
                var query = setUrl.ToString().Contains("?") ? setUrl.ToString().Split('?')[1] : "";

                UrlResolver resolver = new UrlResolver(context.HttpContext.RequestServices);
                await resolver.ResolveUrlAsync(schemeString, hostString, pathBaseString, pathString, query).ConfigureAwait(false);

                context.HttpContext.Items.Add("MKUrlResolver", resolver);
                contextCopy.Request.Path = new Microsoft.AspNetCore.Http.PathString(setUrl);
            }

            // try to create a Console with the 'fake' path
            context.HttpContext.Items.Add("MKConsole", new Beta.GeneratedCms.Console(contextCopy, environment));
            
            await next.Invoke();
        }
    }
}
