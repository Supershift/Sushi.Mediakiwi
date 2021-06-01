using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Controllers
{
    internal class ControllerRegister
    {
        internal static Dictionary<string, IDocumentTypeController> Routes { get; set; }

        internal static void AddRoute(string route, IDocumentTypeController controller)
        {
            if (Routes == null)
            {
                Routes = new Dictionary<string, IDocumentTypeController>();
            }
            Routes.Add(route, controller);
        }

        private HttpContext _Context;
        private string _Path;

        public ControllerRegister(HttpContext context)
        {
            _Context = context;
            _Path = _Context.Request.Path.Value;
        }

        internal IDocumentTypeController Verify()
        {
            foreach (var route in Routes)
            {
                if (Verify(route.Key))
                {
                    return route.Value;
                }
            }
            return null;
        }

        bool Verify(string route)
        {
            if (_Path.EndsWith(route, StringComparison.CurrentCultureIgnoreCase))
            {
                return true;
            }
            return false;
        }
    }
}
