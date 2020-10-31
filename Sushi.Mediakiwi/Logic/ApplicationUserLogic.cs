using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Http;
using Sushi.Mediakiwi.Data;
using Sushi.Mediakiwi.Utilities;

namespace Sushi.Mediakiwi.Data
{
    public class ApplicationUserLogic
    {
        /// <summary>
        /// Selects the specified username. This method also autosaves the application user when found
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <param name="shouldRememberMe">if set to <c>true</c> [should remember me].</param>
        /// <returns></returns>
        public static IApplicationUser Apply(string username, string password, bool shouldRememberMe, HttpContext context)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return new ApplicationUser();

            var tmp = ApplicationUser.SelectOne(username, password);

            if (!tmp.IsNewInstance && !tmp.IsActive)
                tmp = new ApplicationUser();

            if (tmp.IsNewInstance)
                return tmp;

            if (tmp.Type == 0)
            {
                using (Authentication auth = new Authentication())
                {
                    auth.Password = Common.EncryptionKey;

                    if (auth.Encrypt(password) == tmp.Password)
                    {
                        tmp.LastLoggedVisit = Common.DatabaseDateTime;
                        tmp.Save();
                    }
                    else
                    {
                        tmp = new ApplicationUser();
                        //  Insert a notification
                        Notification.InsertOne(Notification.Tags.InternalWimError, string.Format("Someone tried to login in with wrong credentials (username: {0})", username));
                    }
                }
            }
            else if (tmp.Type == 1)
            {
                string candidate = Utility.HashStringByMD5(string.Concat(tmp.Name, password));
                if (candidate.Equals(tmp.Password))
                {
                    tmp.LastLoggedVisit = Common.DatabaseDateTime;

                    var visitor = new VisitorManager(context).Select();
                    visitor.LastLoggedApplicationUserVisit = Common.DatabaseDateTime;
                    visitor.Save();

                    tmp.Save();
                }
                else
                {
                    tmp = new ApplicationUser();
                    //  Insert a notification
                    Notification.InsertOne(Notification.Tags.InternalWimError, string.Format("Someone tried to login in with wrong credentials (username: {0})", username));
                }
            }
            return tmp;
        }
    }
}