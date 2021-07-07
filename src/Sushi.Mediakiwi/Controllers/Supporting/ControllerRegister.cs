using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Controllers
{
    internal class ControllerRegister
    {
        private HttpContext _Context;
        private string _Path;

        private readonly IActionDescriptorCollectionProvider _actionDescriptorCollectionProvider;
        private readonly IActionInvokerFactory _actionInvoker;
        private readonly ILogger _logger;

        public ControllerRegister(HttpContext context, IServiceProvider serviceProvider)
        {
            _Context = context;
            _Path = _Context.Request.Path.Value;
            _actionDescriptorCollectionProvider = serviceProvider.GetService<IActionDescriptorCollectionProvider>();
            _actionInvoker = serviceProvider.GetService<IActionInvokerFactory>();

            var _loggerFactory = serviceProvider.GetService<ILoggerFactory>();
            if (_loggerFactory != null)
            {
                _logger = _loggerFactory.CreateLogger<ControllerRegister>();
            }
        }

        internal async Task<bool> VerifyAsync()
        {
            var routes = _actionDescriptorCollectionProvider.ActionDescriptors.Items.Where(ad => ad.AttributeRouteInfo != null).ToList();
            foreach (var route in routes.Cast<ControllerActionDescriptor>().Where(x => typeof(IController).IsAssignableFrom(x.ControllerTypeInfo)))
            {
                if (Verify(route))
                {
                    //Use Invoker factory
                    if (_actionInvoker != null)
                    {
                        var invoker = _actionInvoker.CreateInvoker(new ActionContext(_Context, new Microsoft.AspNetCore.Routing.RouteData(), route));
                        await invoker.InvokeAsync().ConfigureAwait(false);
                    }
                    return true;
                }
            }
            return false;
        }

        bool Verify(ControllerActionDescriptor route)
        {
            var controllerRoute = $"/{route.AttributeRouteInfo.Template}";
            controllerRoute = controllerRoute.Replace("//", "/", StringComparison.InvariantCultureIgnoreCase);

            if (_Path.StartsWith(controllerRoute, StringComparison.InvariantCultureIgnoreCase))
            {
                // Does the request method match that of the called controller ?
                bool methodMatch = false;

                // Get the allowed HttpMethods for this method.
                var allowecHttpMethods = route.EndpointMetadata.OfType<HttpMethodAttribute>().ToList();

                // Do we have any ?
                if (allowecHttpMethods?.Count > 0)
                {
                    // Check if the currenty request method is allowed
                    foreach (var item in allowecHttpMethods)
                    {
                        if (item.HttpMethods.Contains(_Context.Request.Method.ToUpperInvariant()))
                        {
                            methodMatch = true;
                            break;
                        }
                    }
                }
                else
                {
                    _logger.LogWarning($"No Allowed HttpMethod is added to the controller route '{controllerRoute}'");
                }

                return methodMatch;
            }

            return false;
        }
    }
}
