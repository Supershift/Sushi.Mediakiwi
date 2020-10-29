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
        //HttpContext Context;
        //public ApplicationUserLogic(HttpContext context)
        //{
        //    Context = context;
        //}

        /// <summary>
        /// Gets or sets a value indicating whether [show new design].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [show new design]; otherwise, <c>false</c>.
        /// </value>
        public static bool ShowNewDesignForUser(IApplicationUser inUser)
        {

            if (CommonConfiguration.FORCE_NEWSTYLE) 
                return true;
            if (ShowNewDesign2ForUser(inUser)) 
                return true;
            if (inUser.Data == null) 
                return false;
            
            return inUser.Data["ForceNewStyle"].ParseBoolean(false);
        }

        public static bool ShowNewDesign2ForUser(IApplicationUser inUser)
        {
            if (CommonConfiguration.FORCE_NEWSTYLE2) 
                return true;

            if (inUser.Data == null) 
                return false;

            return inUser.Data["D-V2"].ParseBoolean(false);
        }

        /// <summary>
        /// Applies the specified user GUID.
        /// </summary>
        /// <param name="userGuid">The user GUID.</param>
        /// <param name="shouldRememberMe">if set to <c>true</c> [should remember me].</param>
        /// <returns></returns>
        public static IApplicationUser Apply(Guid userGuid, bool shouldRememberMe)
        {
            var tmp = ApplicationUser.SelectOne(userGuid);

            if (tmp.IsNewInstance || !tmp.IsActive)
                return tmp;

            tmp.LastLoggedVisit = Common.DatabaseDateTime;
            tmp.Save();

            return tmp;
        }

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

        /// <summary>
        /// Select the current application User object. If no visitor exists an instance is created including a database reference.
        /// By default, the visitor reference is not stored accross sessions. 
        /// </summary>
        /// <returns></returns>
        public static IApplicationUser Select(HttpContext context)
        {
            //if (context != null)
            //{
            //    HttpCookie authCookie = context.Request.Cookies[FormsAuthentication.FormsCookieName];
            //    if (authCookie != null)
            //    {
            //        try
            //        {
            //            //Extract the forms authentication cookie
            //            FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);

            //            // If caching roles in userData field then extract
            //            string[] data = authTicket.UserData.Split(new char[] { '|' });

            //            // Create the IIdentity instance
            //            System.Security.Principal.IIdentity id = new FormsIdentity(authTicket);

            //            // Create the IPrinciple instance
            //            IPrincipal principal = new GenericPrincipal(id, data);

            //            // Set the context user 
            //            context.User = principal;

            //            var user = Sushi.Mediakiwi.Data.ApplicationUser.SelectOne(new Guid(data[0]));

            //            var visitor = Sushi.Mediakiwi.Data.Identity.Visitor.Select();
            //            if (visitor.ApplicationUserID != user.ID)
            //            {
            //                visitor.ApplicationUserID = user.ID;
            //                visitor.Save();
            //            }
            //            if (user.IsActive)
            //            {
            //                user.RegisterLogin();
            //            }

            //            //p.Save();
            //            context.Items["wim.applicationuser"] = user;
            //            return user;
            //        }
            //        catch (Exception)
            //        {

            //        }
            //    }
            //}
            return new ApplicationUser();
        }
    }
}