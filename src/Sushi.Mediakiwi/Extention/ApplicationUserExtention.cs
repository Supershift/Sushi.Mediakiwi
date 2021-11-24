using Microsoft.AspNetCore.Http;
using Sushi.Mediakiwi;
using Sushi.Mediakiwi.Authentication;
using Sushi.Mediakiwi.Data;
using Sushi.Mediakiwi.Utilities;
using System;
using System.Net;
using System.Threading.Tasks;

public static class ApplicationUserExtention
{
    public static Site[] Sites(this IApplicationUser inUser, AccessFilter accessFilter)
    {
        return Site.SelectAllAccessible(inUser, accessFilter);
    }

    public static async Task<Site[]> SitesAsync(this IApplicationUser inUser, AccessFilter accessFilter)
    {
        return await Site.SelectAllAccessibleAsync(inUser, accessFilter).ConfigureAwait(false);
    }

    /// <summary>
    /// Saves the specified should remember user for next visit.
    /// </summary>
    /// <param name="shouldRememberVisitorForNextVisit">if set to <c>true</c> [should remember user for next visit].</param>
    /// <returns></returns>
    public static bool Save(this IApplicationUser inUser, HttpContext context, bool shouldRememberVisitorForNextVisit)
    {
        if (inUser.Created == DateTime.MinValue)
            inUser.Created = DateTime.UtcNow;

        inUser.RememberMe = shouldRememberVisitorForNextVisit;

        bool isSaved = inUser.Save();

        if (isSaved)
        {
            var visitor = new VisitorManager(context).Select();
            visitor.ApplicationUserID = inUser.ID;
            visitor.Save();
        }
        return isSaved;
    }

    /// <summary>
    /// Gets the skin from the currentPortal.
    /// </summary>
    //public static string GetSkin(this IApplicationUser inUser)
    //{
    //    return Common.CurrentPortal.DefaultSkin;
    //}

    //public static bool IsLoggedIn(this IApplicationUser inUser)
    //{
    //    bool isLoggedIn = !inUser.IsNewInstance;

    //    var lastlogged = ApplicationUserLogic.Select().LastLoggedVisit;
    //    if (lastlogged == DateTime.MinValue)
    //        isLoggedIn = false;

    //    if (isLoggedIn)
    //    {
    //        int minutes = Wim.Utility.ConvertToInt(Sushi.Mediakiwi.Data.Environment.Current["EXPIRATION_COOKIE_APPUSER"], 0);
    //        if (minutes == 0) minutes = 60;

    //        TimeSpan lastclick = new TimeSpan(Common.DatabaseDateTime.Ticks - lastlogged.Ticks);

    //        if (lastclick.TotalMinutes > minutes)
    //            isLoggedIn = false;
    //    }

    //    if (!isLoggedIn)
    //    {
    //        if (System.Web.HttpContext.Current != null)
    //        {
    //            if (System.Web.HttpContext.Current.User != null && System.Web.HttpContext.Current.User.Identity != null)
    //                isLoggedIn = System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
    //        }
    //    }

    //    return isLoggedIn;
    //}

    //public static void RegisterLogin(this IApplicationUser inUser)
    //{
    //    try
    //    {
    //        if (inUser.LastLoggedVisit == DateTime.MinValue)
    //        {
    //            inUser.LastLoggedVisit = Common.DatabaseDateTime;
    //            inUser.Save();
    //        }
    //        else
    //        {
    //            var minutesAgo = new TimeSpan(Common.DatabaseDateTime.Ticks - inUser.LastLoggedVisit.Ticks).TotalMinutes;
    //            if (minutesAgo > 5)
    //            {
    //                inUser.LastLoggedVisit = Common.DatabaseDateTime;
    //                inUser.Save();
    //            }
    //        }
    //    }
    //    catch (Exception) { }
    //}

    /// <summary>
    /// Sends the inital login mail, asking to apply a password
    /// </summary>
    /// <param name="inUser"></param>
    public static void SendLoginMail(this IApplicationUser inUser, Sushi.Mediakiwi.Beta.GeneratedCms.Console container)
    {
        var userData = ComponentList.SelectOne("Sushi.Mediakiwi.AppCentre.Data.Implementation.User");
        string mail_Title = userData.Settings["Mail_Title"].Value;
        string mail_Intro = userData.Settings["Mail_Intro"].Value;

        if (string.IsNullOrEmpty(mail_Intro))
            mail_Intro = "Dear [name],<br><br>We have created an account for you to login. Please visit the following URL and apply your password using the credentials as noted below:<br><br>[url]<br><br><b>Your personal credentials</b>:<br><br>[credentials]";

        string url;
        ResetPassword(inUser, container, out url);

        string body = string.Format(@"Username: {0}<br/>Emailadres: {1}<br/>"
            , inUser.Name
            , inUser.Email);

        Mail.Send(new System.Net.Mail.MailAddress(inUser.Email, inUser.Displayname),
            string.IsNullOrEmpty(mail_Title) ? "Your user account" : mail_Title,
            string.IsNullOrEmpty(mail_Intro) ? body
                : mail_Intro
                    .Replace("[credentials]", body)
                    .Replace("[name]", inUser.Displayname)
                    .Replace("[login]", inUser.Name)
                    .Replace("[email]", inUser.Email)
                    .Replace("[url]", string.Format("<a href=\"{0}\">{0}</a>", url))
                    .Replace("http://url", string.Format("{0}", url))
                ,
            url);
    }

    /// <summary>
    /// Sends the 'I forgot my password' e-mail
    /// </summary>
    /// <param name="inUser"></param>
    public static void SendForgotPassword(this IApplicationUser inUser, Sushi.Mediakiwi.Beta.GeneratedCms.Console container)
    {
        var userData = ComponentList.SelectOne("Sushi.Mediakiwi.AppCentre.Data.Implementation.User");
        string mail_Title = userData.Settings["Mail_ForgotTitle"].Value;
        string mail_Intro = userData.Settings["Mail_ForgotIntro"].Value;

        if (string.IsNullOrEmpty(mail_Intro))
            mail_Intro = "Dear [name],<br><br>You have requested a password reset through the \"forgotten my password\" page. Please visit the following URL and (re)apply your password:<br><br>[url]";

        inUser.ResetKey = Guid.NewGuid();
        inUser.Save();

        string wimPath = Sushi.Mediakiwi.CommonConfiguration.PORTAL_PATH;

        string url;
        ResetPassword(inUser, container, out url);

        string body = string.Format(@"Username: {0}<br/>Emailadres: {1}<br/>"
            , inUser.Name
            , inUser.Email);

        Mail.Send(new System.Net.Mail.MailAddress(inUser.Email, inUser.Displayname),
            string.IsNullOrEmpty(mail_Title) ? "Forgotten password" : mail_Title,
            string.IsNullOrEmpty(mail_Intro) ? body
                : mail_Intro
                    .Replace("[credentials]", body)
                    .Replace("[name]", inUser.Displayname)
                    .Replace("[login]", inUser.Name)
                    .Replace("[email]", inUser.Email)
                    .Replace("[url]", string.Format("<a href=\"{0}\">{0}</a>", url))
                    .Replace("http://url", string.Format("{0}", url)),
            url);
    }

    /// <summary>
    /// Sends the 'I forgot my password' e-mail
    /// </summary>
    /// <param name="inUser"></param>
    /// <returns>TRUE when succeeded, FALSE when exception occurs</returns>
    public static async Task<bool> SendForgotPasswordAsync(this IApplicationUser inUser, Sushi.Mediakiwi.Beta.GeneratedCms.Console container)
    {
        var userData = await ComponentList.SelectOneAsync("Sushi.Mediakiwi.AppCentre.Data.Implementation.User").ConfigureAwait(false);
        string mail_Title = userData.Settings["Mail_ForgotTitle"].Value;
        string mail_Intro = userData.Settings["Mail_ForgotIntro"].Value;

        if (string.IsNullOrEmpty(mail_Intro))
        {
            mail_Intro = "Dear [name],<br><br>You have requested a password reset through the \"forgotten my password\" page. Please visit the following URL and (re)apply your password:<br><br>[url]";
        }

        string url = await ResetPasswordAsync(inUser, container).ConfigureAwait(false);

        string body = string.Format(@"Username: {0}<br/>Emailadres: {1}<br/>"
            , inUser.Name
            , inUser.Email);

        try
        {
            Mail.Send(new System.Net.Mail.MailAddress(inUser.Email, inUser.Displayname),
            string.IsNullOrEmpty(mail_Title) ? "Forgotten password" : mail_Title,
            string.IsNullOrEmpty(mail_Intro) ? body
                : mail_Intro
                    .Replace("[credentials]", body)
                    .Replace("[name]", inUser.Displayname)
                    .Replace("[login]", inUser.Name)
                    .Replace("[email]", inUser.Email)
                    .Replace("[url]", string.Format("<a href=\"{0}\">{0}</a>", url))
                    .Replace("http://url", string.Format("{0}", url)),
            url, 10000);

            return true;
        }
        catch (Exception ex)
        {
            await Notification.InsertOneAsync("SendMail", ex.Message).ConfigureAwait(false);

            return false;
        }
    }

    /// <summary>
    /// Creates the content for the Login Mail, but DOES NOT send out any email
    /// </summary>
    public static void ExtractLoginMailBody(this IApplicationUser inUser, Sushi.Mediakiwi.Beta.GeneratedCms.Console container, out string subject, out string body, out string url)
    {
        var userData = ComponentList.SelectOne("Sushi.Mediakiwi.AppCentre.Data.Implementation.User");
        subject = userData.Settings["Mail_Title"].Value;
        body = userData.Settings["Mail_Intro"].Value;

        if (string.IsNullOrWhiteSpace(body))
            body = "Dear [name],<br><br>We have created an account for you to login. Please visit the following URL and apply your password using the credentials as noted below:<br><br>[url]<br><br><b>Your personal credentials</b>:<br><br>[credentials]";

        inUser.ResetKey = Guid.NewGuid();
        inUser.Save();

        using (var auth = new AuthenticationLogic())
        {
            auth.Password = "urlinfo";
            string urlAddition = string.Concat("&u=", WebUtility.UrlEncode(auth.Encrypt(inUser.Name)));

            string wimPath = Sushi.Mediakiwi.CommonConfiguration.PORTAL_PATH;

            url = container.AddApplicationPath(string.Concat(wimPath, "?reset=", inUser.ResetKey, urlAddition), true);

            string credentials = string.Format(@"Username: {0}<br/>Emailadres: {1}<br/>"
                , inUser.Name
                , inUser.Email);

            body = body
                .Replace("[credentials]", credentials)
                .Replace("[name]", inUser.Displayname)
                .Replace("[login]", inUser.Name)
                .Replace("[email]", inUser.Email);
        }
    }


    /// <summary>
    /// Creates a reset GUID for the User so that he/she can set a new password
    /// </summary>
    /// <param name="user"></param>
    /// <param name="resetLink"></param>
    /// <param name="shouldEncoded"></param>
    public static void ResetPassword(this IApplicationUser user, Sushi.Mediakiwi.Beta.GeneratedCms.Console container, out string resetLink)
    {
        user.ResetKey = Guid.NewGuid();
        user.Save();

        string wimPath = Sushi.Mediakiwi.CommonConfiguration.PORTAL_PATH;
        resetLink = container.AddApplicationPath(string.Concat(wimPath, "?reset=", user.ResetKey, $"&u={user.Email}"), true);
    }

    /// <summary>
    /// Creates a reset GUID for the User so that he/she can set a new password
    /// </summary>
    /// <param name="user"></param>
    /// <param name="resetLink"></param>
    /// <param name="shouldEncoded"></param>
    public static async Task<string> ResetPasswordAsync(this IApplicationUser user, Sushi.Mediakiwi.Beta.GeneratedCms.Console container)
    {
        user.ResetKey = Guid.NewGuid();
        await user.SaveAsync().ConfigureAwait(false);

        string wimPath = Sushi.Mediakiwi.CommonConfiguration.PORTAL_PATH;
        return container.AddApplicationPath(string.Concat(wimPath, "?reset=", user.ResetKey, $"&u={user.Email}"), true);
    }


    public static bool IsValid(this IApplicationUser user, string password)
    {
        if (user.Type == 0)
        {
            using (var auth = new AuthenticationLogic())
            {
                auth.Password = Sushi.Mediakiwi.Common.EncryptionKey;
                if (auth.Encrypt(password) == user.Password)
                    return true;
            }
        }
        else if (user.Type == 1)
        {
            string candidate = Utility.HashStringByMD5(string.Concat(user.Name, password));
            if (candidate.Equals(user.Password, StringComparison.InvariantCulture))
                return true;
        }
        return false;
    }


    internal static IApplicationUser Store(this IApplicationUser inUser, HttpContext context, string password, bool shouldRememberMe)
    {
        string username = inUser.Name;
        if (inUser.Type == 0)
        {
            using (var auth = new AuthenticationLogic())
            {
                auth.Password = Sushi.Mediakiwi.Common.EncryptionKey;

                if (auth.Encrypt(password) == inUser.Password)
                {
                    inUser.ResetKey = null;
                    inUser.LastLoggedVisit = DateTime.UtcNow;
                    inUser.Save(context, shouldRememberMe);

                    new AuditTrail()
                    {
                        Action = ActionType.Login,
                        Type = ItemType.Undefined,
                        ItemID = inUser.ID,
                        Created = inUser.LastLoggedVisit.Value
                    }.Insert();
                }
                else
                {
                    inUser = new ApplicationUser();
                    //  Insert a notification
                    //Notification.InsertOne(Notification.Tags.InternalWimError, string.Format("Someone tried to login in with wrong credentials (username: {0})", username));

                    new AuditTrail()
                    {
                        Action = ActionType.LoginAttempt,
                        Type = ItemType.Undefined,
                        Message = $"(username: {username})"
                    }.Insert();

                }
            }
        }
        else if (inUser.Type == 1)
        {
            string candidate = Utility.HashStringByMD5(string.Concat(inUser.Name, password));
            if (candidate.Equals(inUser.Password))
            {
                inUser.ResetKey = null;
                inUser.LastLoggedVisit = DateTime.UtcNow;

                var visitor = new VisitorManager(context).Select();
                visitor.LastLoggedApplicationUserVisit = DateTime.UtcNow;
                visitor.Save();

                inUser.Save(context, shouldRememberMe);

                new AuditTrail()
                {
                    Action = ActionType.Login,
                    Type = ItemType.Undefined,
                    ItemID = inUser.ID,
                    Created = inUser.LastLoggedVisit.Value
                }.Insert();
            }
            else
            {
                inUser = new ApplicationUser();
                //  Insert a notification
                new AuditTrail()
                {
                    Action = ActionType.LoginAttempt,
                    Type = ItemType.Undefined,
                    Message = $"(username: {username})"
                }.Insert();
            }
        }
        return inUser;
    }

}