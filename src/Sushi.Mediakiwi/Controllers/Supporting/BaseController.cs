using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sushi.Mediakiwi.Data;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Controllers
{
    public class MediakiwiController : ControllerBase, IController
    {
        IVisitor _CurrentVisitor;
        public IVisitor CurrentVisitor
        {
            get
            {
                if (_CurrentVisitor == null)
                {
                    _CurrentVisitor = new VisitorManager(HttpContext).Select();
                }
                return _CurrentVisitor;
            }
        }

        IApplicationUser _CurrentApplicationUser;
        public IApplicationUser CurrentApplicationUser {
            get
            {
                if (_CurrentApplicationUser == null)
                {
                    if (CurrentVisitor != null && CurrentVisitor.ApplicationUserID.HasValue && CurrentVisitor.ApplicationUserID.Value > 0)
                    {
                        _CurrentApplicationUser = ApplicationUser.SelectOne(CurrentVisitor.ApplicationUserID.Value, true);
                    }
                }
               
                return _CurrentApplicationUser;
            }
        }
    }
}
