﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
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

        public ControllerRegister(HttpContext context, IServiceProvider serviceProvider)
        {
            _Context = context;
            _Path = _Context.Request.Path.Value;
            _actionDescriptorCollectionProvider = serviceProvider.GetService<IActionDescriptorCollectionProvider>();
            _actionInvoker = serviceProvider.GetService<IActionInvokerFactory>();
        }

        internal async Task<IController> VerifyAsync(bool hasLoggedInUser = false)
        {
            var routes = _actionDescriptorCollectionProvider.ActionDescriptors.Items.Where(ad => ad.AttributeRouteInfo != null).ToList();
            foreach (var route in routes.Cast<ControllerActionDescriptor>().Where(x => typeof(IController).IsAssignableFrom(x.ControllerTypeInfo)))
            {
                if (Verify(route))
                {
                    //var iController = Activator.CreateInstance(route.ControllerTypeInfo.AsType()) as BaseController;
                    //if (iController.IsAuthenticationRequired && hasLoggedInUser == false)
                    //{
                    //    return null;
                    //}

                    //Use Invoker factory
                    if (_actionInvoker != null)
                    {
                        var invoker = _actionInvoker.CreateInvoker(new ActionContext(_Context, new Microsoft.AspNetCore.Routing.RouteData(), route));
                        await invoker.InvokeAsync().ConfigureAwait(false);
                    }
                    return new BaseController();
                }
            }
            return null;
        }

        bool Verify(ControllerActionDescriptor route)
        {
            var controllerRoute = $"/{route.AttributeRouteInfo.Template}";
            controllerRoute = controllerRoute.Replace("//", "/", StringComparison.InvariantCultureIgnoreCase);

            if (_Path.StartsWith(controllerRoute, StringComparison.InvariantCultureIgnoreCase))
            {
                return true;
            }

            return false;
        }
    }
}
