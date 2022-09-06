using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Hosting;
using Sushi.Mediakiwi.API.Extensions;
using Sushi.Mediakiwi.API.Transport.Requests;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.API.Filters
{
    public class MediakiwiConsoleFilter : IAsyncActionFilter
    {
        protected IHostEnvironment environment { get; private set; }

        public MediakiwiConsoleFilter(IHostEnvironment _env) 
        {
            environment = _env;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var contextCopy = context.HttpContext.Clone();
            contextCopy.Items = context.HttpContext.Items;

            Framework.Api.MediakiwiPostRequest postRequest = new Framework.Api.MediakiwiPostRequest();

            // If we are in POST, we should receive the field that caused the postback
            // as well as all the formmap data
            if (context.HttpContext.Request.Method == Microsoft.AspNetCore.Http.HttpMethods.Post)
            {

                if (context.ActionArguments?.Count > 0)
                {
                    foreach (var item in context.ActionArguments)
                    {
                        if (item.Value is PostContentRequest postContent)
                        {
                            if (string.IsNullOrWhiteSpace(postContent.PostedField) == false)
                            {
                                contextCopy.Request.Headers.Add("postedField", postContent.PostedField);
                                postRequest = postContent.FormMaps.GetPostRequest();

                                if (postRequest?.FormFields?.ContainsKey("autopostback") == false)
                                {
                                    postRequest.FormFields.Add("autopostback", postContent.PostedField);
                                }
                            }
                        }
                    }
                }
            }

            // Set the Console to be NULL
            Beta.GeneratedCms.Console console = null;

            // When a call is Anonymous, we should create a new Console anyway
            if (context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any())
            {
                console = new Beta.GeneratedCms.Console(contextCopy, environment);
            }

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
                contextCopy.Request.Path = pathString;
                if (string.IsNullOrWhiteSpace(query) == false)
                {
                    contextCopy.Request.QueryString = new Microsoft.AspNetCore.Http.QueryString($"?{query}");
                }
                contextCopy.RequestServices = context.HttpContext.RequestServices;

                // Remove internal query CurrentSiteID
                if (contextCopy.Request.QueryString.HasValue && contextCopy.Request.Query.ContainsKey("currentSiteId"))
                {
                    var items = contextCopy.Request.Query.SelectMany(x => x.Value, (col, value) => new KeyValuePair<string, string>(col.Key, value)).ToList();
                    items.RemoveAll(x => x.Key == "currentSiteId");
                    var qb = new Microsoft.AspNetCore.Http.Extensions.QueryBuilder(items);
                    contextCopy.Request.QueryString = qb.ToQueryString();
                }

                // try to create a Console with the 'fake' path
                console = new Beta.GeneratedCms.Console(contextCopy, environment);

                // Check if we have post Values assigned 
                if (postRequest?.FormFields?.Count > 0)
                {
                    console.JsonForm = postRequest.FormFields;
                }

                // Create an URL resolver
                UrlResolver resolver = new UrlResolver(context.HttpContext.RequestServices, console);

                // Set default status
                resolver.StatusCode = HttpStatusCode.OK;

                // Resolve the supplied URL 
                await resolver.ResolveUrlAsync(schemeString, hostString, pathBaseString, pathString, query).ConfigureAwait(false);

                // When the resolver created a list, assign it to the Console
                if (resolver.List != null)
                {
                    // Allow links, images and documents to all users
                    if (
                            resolver.List.Type == Data.ComponentListType.Images
                        ||  resolver.List.Type == Data.ComponentListType.Documents
                        ||  resolver.List.Type == Data.ComponentListType.Links)
                    {
                        console.CurrentList = resolver.List;
                    }
                    // Check if the user has rights to view that list.
                    else 
                    {
                        var userHasAccess = await resolver.List.HasRoleAccessAsync(resolver.ApplicationUser);
                        if (userHasAccess == false)
                        {
                            resolver.StatusCode = HttpStatusCode.Unauthorized;
                        }
                        else
                        {
                            console.CurrentList = resolver.List;
                        }
                    }
                }

                // When the resolver created a list instance, assign it to the Console
                if (resolver.ListInstance != null)
                {
                    console.CurrentListInstance = resolver.ListInstance;
                }

                // When the resolver created a page, assign it to the Context
                if (resolver.Page?.ID > 0) 
                {
                    contextCopy.Items.Add("Wim.Page", resolver.Page);
                }

                // Assign the item ID to the console
                console.Item = resolver.ItemID;
                
                // Add the resolver to the HttpContext
                context.HttpContext.Items.Add(Common.API_HTTPCONTEXT_URLRESOLVER, resolver);
                context.HttpContext.Items.Add(Common.API_HTTPCONTEXT_ITEM, "1");
            }

            // Add the console to the HttpContext
            if (console != null)
            {
                context.HttpContext.Items.Add(Common.API_HTTPCONTEXT_CONSOLE, console);
            }

            await next.Invoke();
        }
    }
}
