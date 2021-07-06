using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sushi.Mediakiwi.Data;
using System.IO;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Controllers
{
    public class BaseController : ControllerBase, IController
    {
        IVisitor m_CurrentVisitor;
        /// <summary>
        /// Gets or sets the current visitor.
        /// </summary>
        /// <value>The current visitor.</value>
        protected IVisitor CurrentVisitor
        {
            get
            {
                if (m_CurrentVisitor == null)
                {
                    m_CurrentVisitor = new VisitorManager(HttpContext).Select();
                }
                return m_CurrentVisitor;
            }
            set { m_CurrentVisitor = value; }
        }

        IApplicationUser m_CurrentApplicationUser;
        /// <summary>
        /// Gets or sets the current application user.
        /// </summary>
        /// <value>The current application user.</value>
        protected IApplicationUser CurrentApplicationUser
        {
            get
            {
                if (m_CurrentApplicationUser == null
                    && CurrentVisitor != null
                    && CurrentVisitor.ApplicationUserID.HasValue
                    && CurrentVisitor.ApplicationUserID.Value > 0
                    )
                {
                    m_CurrentApplicationUser = ApplicationUser.SelectOne(CurrentVisitor.ApplicationUserID.Value, true);
                }
                return m_CurrentApplicationUser;
            }
            set { m_CurrentApplicationUser = value; }
        }

        protected HttpStatusCode? Authenticate()
        {
            // Authenticate            
            if (CurrentApplicationUser == null || CurrentApplicationUser.IsActive == false)
                return HttpStatusCode.Unauthorized;

            return null;
        }

        public bool IsAuthenticationRequired { get; set; }
    }
}
