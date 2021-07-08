using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sushi.Mediakiwi.Controllers;
using Sushi.Mediakiwi.Data;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi
{
    public class MediakiwiController : ControllerBase, IController
    {
        IApplicationUser _CurrentApplicationUser;
        public IApplicationUser CurrentApplicationUser
        {
            get
            {
                if (_CurrentApplicationUser == null)
                {
                    var currentVisitor = new VisitorManager(HttpContext).Select();
                    if (currentVisitor != null
                         && currentVisitor.ApplicationUserID.HasValue
                         && currentVisitor.ApplicationUserID.Value > 0
                         )
                    {
                        _CurrentApplicationUser = ApplicationUser.SelectOne(currentVisitor.ApplicationUserID.Value, true);
                    }
                }
                return _CurrentApplicationUser;
            }
        }
    }
}