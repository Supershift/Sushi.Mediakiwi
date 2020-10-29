using System;
using System.Linq;
using System.Web.Caching;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Diagnostics;
using System.Data;
using System.Web;
using System.Web.Util;
using System.Web.Hosting;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Principal;
using Sushi.Mediakiwi.Data;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Sushi.Mediakiwi.Data.Identity;
using Sushi.Mediakiwi.Framework2;
using System.Threading.Tasks;
using System.Web.Security;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System.Security.Claims;

namespace Sushi.Mediakiwi.Framework
{
    /// <summary>
    /// Request type definition
    /// </summary>
    public enum RequestItemType
    {
        Undefined = 0,
        /// <summary>
        /// A list item request
        /// </summary>
        Item = 1,
        /// <summary>
        /// A page request
        /// </summary>
        Page = 2,
        /// <summary>
        /// An asset request
        /// </summary>
        Asset = 3,
        /// <summary>
        /// The dashboard
        /// </summary>
        Dashboard = 4
    }

    public interface IRichTextDataCleaner
    {
        string ParseData(RequestItemType type, int? item, string id, string data);
    }

    public class RichTextDataCleaner : IRichTextDataCleaner
    {
        public string ParseData(RequestItemType type, int? item, string id, string data)
        {
            return data;
        }
    }

    public interface IRequestRouter
    {
        void VerifyRequest(HttpApplication application);
    }

    public interface IHtmlTableParser
    {
        string ParseData(Data.Page page, Data.Component component, string id, string html);
    }

    public class HtmlTableParser : IHtmlTableParser
    {
        public string ParseData(Data.Page page, Data.Component component, string id, string html)
        {
            return html;
        }
    }

    public class RequestRouter : IRequestRouter
    {
        public void VerifyRequest(HttpApplication application)
        {
            //  Do nothing
        }
    }

    public interface iPreMonitorHook
    {
        void VerifyRequest(HttpApplication application);
    }

    public class PreMonitorHook : iPreMonitorHook
    {
        public void VerifyRequest(HttpApplication application)
        {
            //  Do nothing
        }
    }

    public class PageAnalytics
    {
        public static PageAnalytics Default()
        {
            return new PageAnalytics();
        }
        public void Register(HttpRequest Request)
        {
            if (!string.IsNullOrEmpty(Request.QueryString["m"]))
            {
                try
                {
                    //if (Request.IsLocal) return;
                    if (Request.UrlReferrer == null) return;
                    int pageID = Wim.Utility.ConvertToInt(Request.QueryString["m"]);
                    Guid visitorGUID = Wim.Utility.ConvertToGuid(Request.QueryString["dc"]);

                    System.Uri url = new Uri(Request.QueryString["w"]);

                    IVisitor visitor;

                    if (visitorGUID == Guid.Empty)
                        visitor = Sushi.Mediakiwi.Data.Identity.Visitor.Select();
                    else
                        visitor = Sushi.Mediakiwi.Data.Identity.Visitor.Select(visitorGUID);

                    var user = Sushi.Mediakiwi.Data.ApplicationUserLogic.Select();

                    Sushi.Mediakiwi.Data.Page page = Sushi.Mediakiwi.Data.Page.SelectOne(pageID);

                    int logID = visitor.Data["wim_logid"].ParseInt().GetValueOrDefault();

                    bool isEntry = false;
                    bool isUnique;
                    bool isNewSession;
                    if (Request.QueryString["s"] == "2")
                    {
                        isNewSession = visitor.IsNewSession;
                        isUnique = visitor.IsNewVisitor;
                    }
                    else
                    {
                        isNewSession = Request.QueryString["s"] == "1";
                        isUnique = Request.QueryString["u"] == "1";
                    }

                    if (isNewSession || logID == 0)
                    {
                        Sushi.Mediakiwi.Data.Statistics.VisitorLog visit = new Sushi.Mediakiwi.Data.Statistics.VisitorLog();
                        visit.Agent = Request.UserAgent;
                        visit.Browser = string.Concat(Request.Browser.Browser, " ", Request.Browser.Version);
                        visit.Referrer = Request.QueryString["r"];
                        visit.VisitorID = visitor.ID;
                        visit.IsUnique = isUnique;
                        visit.HasCookie = Request.QueryString["c"] == "1";
                        if (string.IsNullOrEmpty(Wim.CommonConfiguration.LOAD_BALANCER_IP_HEADER))
                            visit.IP = Request.UserHostAddress;
                        else
                        {
                            visit.IP = Request.Headers[Wim.CommonConfiguration.LOAD_BALANCER_IP_HEADER];
                            if (string.IsNullOrEmpty(visit.IP))
                                visit.IP = Request.UserHostAddress;
                        }

                        visit.Save();
                        logID = visit.ID;
                        visitor.Data.Apply("wim_logid", logID);
                        isEntry = true;
                    }

                    int lastPageID = visitor.Data["wim_lastpageid"].ParseInt().GetValueOrDefault();

                    Guid urlguid = page.IsNewInstance ? Guid.Empty : page.Site.m_GUID;
                    Sushi.Mediakiwi.Data.Statistics.VisitorUrl visitedUrl = Sushi.Mediakiwi.Data.Statistics.VisitorUrl.SelectOne(urlguid, url.Host);
                    visitedUrl.Name = url.Host.ToString();
                    visitedUrl.GUID = urlguid;
                    if (visitedUrl.IsNewInstance)
                        visitedUrl.Save();

                    string path = Wim.Utility.RemApplicationPath(url.AbsolutePath);
                    Guid pageguid = page.IsNewInstance ? Guid.Empty : page.m_GUID;
                    Sushi.Mediakiwi.Data.Statistics.VisitorPage visitedPage = Sushi.Mediakiwi.Data.Statistics.VisitorPage.SelectOne(visitedUrl.ID, pageguid, path);
                    visitedPage.Name = path;
                    visitedPage.GUID = pageguid;
                    visitedPage.UrlID = visitedUrl.ID;

                    if (visitedPage.IsNewInstance)
                        visitedPage.Save();

                    Sushi.Mediakiwi.Data.Statistics.VisitorClick click = new Sushi.Mediakiwi.Data.Statistics.VisitorClick();
                    click.VisitorLogID = logID;
                    click.ProfileID = visitor.ProfileID.GetValueOrDefault();
                    click.ApplicationUserID = user.ID;
                    click.ItemID = page.ID;
                    click.RenderTime = Wim.Utility.ConvertToIntNullable(Request.QueryString["t"]);
                    click.IsEntry = isEntry;
                    click.Entry = isEntry ? 1 : 0;
                    click.Data = visitor.Data;
                    click.Query = string.IsNullOrEmpty(url.Query) ? null : url.Query.Replace("?", string.Empty);
                    click.PageID = visitedPage.ID;
                    click.CampaignID = visitor.Data["campaign.id"].ParseInt();
                    click.Save();

                    visitor.Data.Apply("wim_lastpageid", page.ID);
                    visitor.Save();

                    return;
                }
                catch (Exception ex)
                {
                    Sushi.Mediakiwi.Data.Notification.InsertOne("Statistics", ex);
                }
                return;
            }
        }
    }

    public enum GlobalPlaceholder
    {
        SIGNIN_AREA,
        SIGNIN_HEAD,
        SIGNIN_SIGNATURE,
        SIGNIN_LOGO_URL,
    }

    public enum CallbackTarget
    {
        //  Directly after the application user has logged in, but not written to the cookie
        POST_SIGNIN,
        //  Directly before the application user is validated against the user IDP
        PRE_SIGNIN
    }

    public interface ICallback
    {
        string UID { get; set; }
        bool Run(Sushi.Mediakiwi.Beta.GeneratedCms.Console console);
    }

    public interface ILink
    {
        string UID { get; set; }
        bool Init(HttpContext context);
        bool PreRender(HttpContext context);
        //void EndRequest(HttpContext context);
    }

    public partial class ReWire : Relink
    {
    }

    /// <summary>
    /// 
    /// </summary>
    public partial class Relink : IHttpModule
    {


        #region Dispose
        /// <summary>
        /// 
        /// </summary>
        public void Dispose() { }
        #endregion

        #region Delegate
        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <param name="e"></param>
        public delegate void MyEventHandler(Object s, EventArgs e);
        private MyEventHandler _eventHandler = null;
        /// <summary>
        /// 
        /// </summary>
        public event MyEventHandler MyEvent
        {
            add { _eventHandler += value; }
            remove { _eventHandler -= value; }
        }
        #endregion

        #region Event_Init
        System.Web.HttpApplication m_Application;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="application"></param>
        public void Init(HttpApplication application)
        {
            m_Application = application;

            application.PreRequestHandlerExecute += new EventHandler(m_Application_PreRequestHandlerExecute);
            //application.BeginRequest += new EventHandler(OnBeginRequest);
            application.PostAuthenticateRequest += new EventHandler(OnBeginRequest);


            application.AuthenticateRequest += Application_AuthenticateRequest;
            application.Error += M_Application_Error;
            application.PostRequestHandlerExecute += Application_PostRequestHandlerExecute;
        }

        private void Application_PostRequestHandlerExecute(object sender, EventArgs e)
        {
            var path = Wim.Utility.RemApplicationPath(m_Application.Context.Request.Path.ToLower());
            if (Links.ContainsKey(path))
            {
                var apps = Links[path];
                foreach (var app in apps)
                    if (!app.PreRender(m_Application.Context))
                        return;
            }

        }


        //private void Application_EndRequest(object sender, EventArgs e)
        //{
        //    var path = Wim.Utility.RemApplicationPath(m_Application.Context.Request.Path.ToLower());
        //    if (Links.ContainsKey(path))
        //    {
        //        var app = Links[path];
        //        app.EndRequest(m_Application.Context);
        //    }
        //}

        private void Application_AuthenticateRequest(object s, EventArgs e)
        {
            if (System.Configuration.ConfigurationManager.AppSettings["MediaKiwi.AuthenticationValidation"] != "1")
                return;

            bool iscms = m_Application.Context.Request.Path.ToLower().StartsWith(Sushi.Mediakiwi.Data.Environment.Current.GetPath().ToLower());
            if (iscms)
            {
                m_Application = (System.Web.HttpApplication)s;
                var appuser = Authenticate();

                if (!m_Application.Request.IsAuthenticated)
                {
                    //if (!m_Application.Request.QueryString.ToString().Equals("logout"))
                    //{
                    //    if (m_Application.Request.Cookies.Count > 0)
                    //    {
                    //        HttpCookie aCookie;
                    //        string cookieName;
                    //        int limit = m_Application.Request.Cookies.Count;
                    //        for (int i = 0; i < limit; i++)
                    //        {
                    //            cookieName = m_Application.Request.Cookies[i].Name;
                    //            aCookie = new HttpCookie(cookieName);
                    //            aCookie.Expires = DateTime.Now.AddDays(-1);
                    //            m_Application.Response.Cookies.Add(aCookie);
                    //        }
                    //        m_Application.Response.Redirect($"{m_Application.Request.Path}?logout");
                    //    }
                    //} else 
                    if (m_Application.Request.QueryString.ToString().Equals("logout"))
                    {
                        if (m_Application.Request.Cookies.Count > 0)
                        {
                            HttpCookie aCookie;
                            string cookieName;
                            int limit = m_Application.Request.Cookies.Count;
                            for (int i = 0; i < limit; i++)
                            {
                                cookieName = m_Application.Request.Cookies[i].Name;
                                aCookie = new HttpCookie(cookieName);
                                aCookie.Expires = DateTime.Now.AddDays(-1);
                                m_Application.Response.Cookies.Add(aCookie);
                            }
                        }
                        m_Application.Response.Redirect($"{m_Application.Request.Path}");
                    }
                }
            }
        }

        bool IsPostBack(string postBackValue)
        {
            return PostbackValue.Equals(postBackValue, StringComparison.InvariantCultureIgnoreCase);
        }

        string PostbackValue
        {
            get
            {
                string value = "";
                if (m_Application.Context == null) return value;

                value = m_Application.Context.Request.Form["autopostback"];
                if (value == null) value = "";
                else
                {
                    if (value.Contains("$"))
                        value = value.Split('$')[0];
                }
                return value;
            }
        }

        void SecureConnection()
        {
            var context = m_Application.Context;
            if (context.Response.Headers["X-Powered-By"] != null)
                context.Response.Headers.Remove("X-Powered-By");

            // nesting pages in frames (only allowed from SAME ORIGIN).
            // https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/X-Frame-Options
            SetHeader("X-Frame-Options", "SAMEORIGIN");

            // only access using HTTPS
            // https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Strict-Transport-Security
            SetHeader("Strict-Transport-Security", "1;mode=block");

            // Stops pages from loading when they detect reflected cross-site scripting
            // https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/X-XSS-Protection
            SetHeader("X-XSS-Protection", "1;mode=block");

            // indicate that the MIME types advertised in the Content-Type headers should not be changed and be followed.
            // https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/X-Content-Type-Options
            SetHeader("X-Content-Type-Options", "nosniff");

            // Specifies if a cross-domain policy file (crossdomain.xml) is allowed.
            // https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers
            SetHeader("X-Permitted-Cross-Domain-Policies", "none;");

            // The Referrer-Policy HTTP header controls how much referrer information (sent via the Referer header) should be included with requests.
            // https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Referrer-Policy
            SetHeader("Referrer-Policy", "strict-origin");

            // https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Content-Security-Policy
            SetHeader("Content-Security-Policy", "script-src 'self' mediakiwi.azureedge.net 'unsafe-inline'");

            SetHeader("Pragma", "NO-CACHE");
            SetHeader("Cache-control", "NO-STORE");
        }

        void SetHeader(string header, string value)
        {
            var context = m_Application.Context;

            if (context.Response.Headers[header] == null)
                context.Response.Headers.Add(header, value);
        }

        IApplicationUser Authenticate()
        {
            HttpCookie authCookie = m_Application.Request.Cookies[FormsAuthentication.FormsCookieName];

            if (authCookie != null)
            {
                try
                {
                    if (IsPostBack("logout") || m_Application.Request.Params["logout"] == "1" || m_Application.Request.QueryString.ToString().Equals("logout"))
                    {
                        //Extract the forms authentication cookie
                        FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);

                        // If caching roles in userData field then extract
                        string[] data = authTicket.UserData.Split(new char[] { '|' });

                        FormsAuthentication.SignOut();

                        // clear all authentication cookie
                        authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, "");
                        authCookie.Expires = DateTime.Now.AddDays(-1);
                        if (CommonConfiguration.COOKIEDOMAIN_LOCALHOST_AS_NULL && authCookie?.Domain?.ToLowerInvariant() == "localhost")
                            authCookie.Domain = null;

                        if (data.Length > 1)
                        {
                            Guid tenant;
                            if (Guid.TryParse(data[1], out tenant))
                            {
                                var uriString = $"{m_Application.Request.Url.Scheme}://{m_Application.Request.Url.Host}";
                                var logouturl = "https://login.microsoftonline.com";
                                var tenantredirect = $"{logouturl}/{data[1]}/oauth2/logout?post_logout_redirect_uri={uriString}";
                                m_Application.Response.Redirect(tenantredirect);
                            }
                            else
                            {
                                var url = data[1];
                                m_Application.Response.Redirect(url);
                            }
                        }
                        else
                        {
                            var url = $"{m_Application.Request.Url.Scheme}://{m_Application.Request.Url.Host}{Sushi.Mediakiwi.Data.Environment.Current.GetPath()}";
                            m_Application.Response.Redirect(url);
                        }
                        return null;
                    }
                    else
                    {

                        //Extract the forms authentication cookie
                        FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);

                        // If caching roles in userData field then extract
                        string[] data = authTicket.UserData.Split(new char[] { '|' });

                        // Create the IIdentity instance
                        System.Security.Principal.IIdentity id = new FormsIdentity(authTicket);

                        // Create the IPrinciple instance
                        IPrincipal principal = new GenericPrincipal(id, data);

                        // Set the context user 
                        m_Application.Context.User = principal;
                        SetCookie(authCookie);

                        var user = Sushi.Mediakiwi.Data.ApplicationUser.SelectOne(new Guid(data[0]));

                        if (user.ID > 0)
                            user.RegisterLogin();

                        var visitor = Sushi.Mediakiwi.Data.Identity.Visitor.Select();
                        if (visitor.ApplicationUserID != user.ID)
                        {
                            visitor.ApplicationUserID = user.ID;
                            visitor.Save();
                        }
                        return user;
                    }
                }
                catch (Exception ex) {
                    Trace.WriteLineIf(Wim.CommonConfiguration.IS_LOCAL_DEVELOPMENT, $"FormsAuthentication err: {ex.Message}, {ex.StackTrace}");
                }
            }

            // Create a user on current thread from provided header
            if (m_Application.Request.Headers.AllKeys.Contains("X-MS-CLIENT-PRINCIPAL-ID"))
            {
                try
                {
                    // Read headers from Azure
                    var azureAppServicePrincipalIdHeader = m_Application.Request.Headers["X-MS-CLIENT-PRINCIPAL-ID"];
                    var azureAppServicePrincipalNameHeader = m_Application.Request.Headers["X-MS-CLIENT-PRINCIPAL-NAME"];

                    #region extract claims via call /.auth/me
                    //invoke /.auth/me
                    var cookieContainer = new CookieContainer();
                    HttpClientHandler handler = new HttpClientHandler()
                    {
                        CookieContainer = cookieContainer,
                    };
                    string uriString = $"{m_Application.Request.Url.Scheme}://{m_Application.Request.Url.Host}";
                    foreach (var key in m_Application.Request.Cookies.AllKeys)
                    {
                        cookieContainer.Add(new Uri(uriString), new Cookie(key, m_Application.Request.Cookies[key].Value));
                    }
                    string jsonResult = string.Empty;
                    using (HttpClient client = new HttpClient(handler))
                    {
                        client.Timeout = TimeSpan.FromSeconds(10);
                        var res = client.GetAsync($"{uriString}/.auth/me").Result;
                        jsonResult = res.Content.ReadAsStringAsync().Result;
                    }

                    //parse json
                    var obj = JArray.Parse(jsonResult);
                    string user_id = obj[0]["user_id"].Value<string>(); //user_id

                    // Create claims id
                    var tenantid = string.Empty;

                    List<Claim> claims = new List<Claim>();
                    foreach (var claim in obj[0]["user_claims"])
                    {
                        if (claim["typ"].ToString().EndsWith("tenantid"))
                            tenantid = claim["val"].ToString();
                        claims.Add(new Claim(claim["typ"].ToString(), claim["val"].ToString()));
                    }

                    var email = obj[0]["user_id"].ToString();
                    if (Sushi.Mediakiwi.Data.ApplicationUser.HasEmail(email, 0))
                    {
                        var user = Sushi.Mediakiwi.Data.ApplicationUser.SelectOneByEmail(email);

                        string[] data = new[] { user.GUID.ToString(), tenantid };
                        FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(
                           1,                                       // ticket version
                           user.Displayname,                        // authenticated username
                           DateTime.Now,                            // issueDate
                           DateTime.Now.AddMinutes(30),             // expiryDate
                           user.RememberMe,                         // true to persist across browser sessions
                           string.Join("|", data),                  // can be used to store additional user data
                           FormsAuthentication.FormsCookiePath);    // the path for the cookie

                        // Encrypt the ticket using the machine key
                        string encryptedTicket = FormsAuthentication.Encrypt(ticket);

                        // Add the cookie to the request to save it
                        HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
                        //cookie.HttpOnly = true;
                        m_Application.Response.Cookies.Add(cookie);

                        // Set user in current context as claims principal
                        var identity = new GenericIdentity(azureAppServicePrincipalNameHeader);
                        identity.AddClaims(claims);
                        #endregion

                        // Set current thread user to identity
                        m_Application.Context.User = new GenericPrincipal(identity, null);
                        return user;
                    }
                }
                catch(Exception ex)
                {
                    Sushi.Mediakiwi.Data.Notification.InsertOne("X-MS-CLIENT-PRINCIPAL-ID", ex);
                }
            };
            return null;
        }

        void SetCookie(HttpCookie authCookie)
        {
            int sessionTimeout = Wim.Utility.ConvertToInt(System.Configuration.ConfigurationManager.AppSettings["MediaKiwi.SessionTimeout"], 60);
            DateTime expiration = DateTime.Now.AddMinutes(sessionTimeout);

            authCookie.HttpOnly = false;
            authCookie.Expires = expiration;
            authCookie.Secure = m_Application.Context.Request.IsSecureConnection;
            authCookie.Shareable = false;
            if (CommonConfiguration.COOKIEDOMAIN_LOCALHOST_AS_NULL && authCookie?.Domain?.ToLowerInvariant() == "localhost")
                authCookie.Domain = null;

            m_Application.Context.Response.Cookies.Add(authCookie);
        }

        private IAsyncResult OnBegin(object sender, EventArgs e, AsyncCallback cb, object extraData)
        {
            var tcs = new TaskCompletionSource<object>(extraData);
            DoAsyncWork(HttpContext.Current).ContinueWith(t =>
            {
                if (t.IsFaulted)
                {
                    tcs.SetException(t.Exception.InnerExceptions);
                }
                else
                {
                    tcs.SetResult(null);
                }
                if (cb != null) cb(tcs.Task);
            });
            return tcs.Task;
        }

        private void OnEnd(IAsyncResult ar)
        {
            var t = (System.Threading.Tasks.Task)ar;
            t.Wait();
        }

        async System.Threading.Tasks.Task DoAsyncWork(HttpContext ctx)
        {
            // USE RESULT
        }

        private void M_Application_Error(object sender, EventArgs e)
        {
            // Code that runs when an unhandled error occurs
            try
            {
                if (CommonConfiguration.LOG_UNHANDLED_ERRORS)
                {
                    Exception exc = m_Application.Server.GetLastError();

                    if (exc != null)
                        Sushi.Mediakiwi.Data.Notification.InsertOne("Unhandled error", exc);
                }
            }
            catch { }
        }

        void m_Application_Error(object sender, EventArgs e)
        {
            if (!m_Application.Request.IsLocal)
            {
                m_Application.Response.Write("");
                m_Application.Response.End();
            }
        }

        /// <summary>
        /// Verifies the load balanced caching.
        /// </summary>
        void VerifyLoadBalancedCaching()
        {

            if (!Wim.CommonConfiguration.IS_LOAD_BALANCED) return;

            //  Set first timestamp for future checking
            object candidate = null;
            if (!Wim.Utilities.CacheItemManager.IsCachedObject("Node.TimeStamp", out candidate))
            {
                candidate = Sushi.Mediakiwi.Data.Common.DatabaseDateTime;
                System.Web.HttpContext.Current.Cache.Insert("Node.TimeStamp", candidate, null, DateTime.Now.AddYears(1), TimeSpan.Zero);
            }
            DateTime dt = (DateTime)candidate;

            var cleanup = CacheItem.SelectAll(dt);
            if (cleanup == null)
                return;

            DateTime max = dt;
            foreach (var item in cleanup)
            {
                if (item.Created > max)
                    max = item.Created;

                if (item.IsIndex)
                    Wim.Utilities.CacheItemManager.FlushIndexOfCacheObjects(item.Name, true);
                else
                    Wim.Utilities.CacheItemManager.FlushCacheObject(item.Name);
            }
            System.Web.HttpContext.Current.Cache.Insert("Node.TimeStamp", max, null, DateTime.Now.AddYears(1), TimeSpan.Zero);
        }

        string m_ActionUrl;
        void m_Application_PreRequestHandlerExecute(object sender, EventArgs e)
        {
            //var path = Wim.Utility.RemApplicationPath(m_Application.Context.Request.Path.ToLower());
            //if (Links.ContainsKey(path))
            //{
            //    var app = Links[path];
            //    app.PostRequestHandler(m_Application.Context);
            //}
            //System.Web.Hosting.HostingEnvironment.RegisterVirtualPathProvider(new CustomVirtualPathProvider());

            Sushi.Mediakiwi.Data.Environment.RegisterDependencyContainer();

        }
        #endregion

        #region Event_OnBeginRequest
        #region OnBeginRequest

        string[] ignoreExtentionList = new string[] { ".png", ".jpg", ".jpeg", ".gif", ".js", ".css", ".ttf", ".axd", "tcl.aspx", ".ashx", ".asmx", "responsivepreview.aspx" };

        private bool IsPossibleWimCall(HttpContext context, ref IPageMapping pagemap)
        {
            System.Uri url = context.Request.Url;

            bool isWimCall = false;

            string url2 = Wim.Utility.RemApplicationPath(url.AbsolutePath).ToLower();
            string ext = Sushi.Mediakiwi.Data.Environment.Current.GetRegistryValue("PAGE_WILDCARD_EXTENTION", "aspx");

            if (string.IsNullOrEmpty(url2) || url2 == "/default.aspx" || url2 == "default.aspx" || url2 == "/")
                return true;

            foreach (var extention in ignoreExtentionList)
            {
                if (string.IsNullOrEmpty(extention))
                    continue;

                if (url2.EndsWith(extention)) return false;
            }

            
            pagemap = Sushi.Mediakiwi.Data.Common.GetCurrentUrlMapping(url.AbsolutePath);

            if (pagemap == null)
            {
                pagemap = Sushi.Mediakiwi.Data.PageMapping.SelectOne(url.AbsolutePath);
                if (!pagemap.IsNewInstance)
                {
                    HttpContext.Current.Items["Wim.PageMap"] = pagemap;
                    return true;
                }
            }

            if (ext == ".")
            {
                string[] split = url2.Split('/');
                string pageName = m_Application.Server.UrlDecode(split[split.Length - 1]);

                if (pageName.Contains(".aspx"))
                    pageName = pageName.Replace(".aspx", string.Empty);

                if (!pageName.Contains("."))
                    return true;
            }

            if (string.IsNullOrEmpty(url2) || url2 == "/") isWimCall = true;

            if (url2.EndsWith(ext)) isWimCall = true;

            return isWimCall;
        }

        /// <summary>
        /// Sets the cache variation.
        /// </summary>
        string GetCacheVariation(string keys)
        {
            if (keys == null) return null;
            string candidate = null;

            foreach (string key in keys.Split(';'))
            {
                string value = m_Application.Request.Params[key];
                if (value == null) continue;

                if (candidate == null)
                    candidate = string.Concat("?", key.ToLower(), "=", value);
                else
                    candidate += string.Concat("&", key.ToLower(), "=", value);
            }
            return candidate;
        }

        void RewritePage(Sushi.Mediakiwi.Data.Page page, Sushi.Mediakiwi.Data.Site site, Sushi.Mediakiwi.Data.IPageMapping map)
        {
            RewritePage(page, site, map, 200);
        }

        void ForceTopLevel_SSL()
        {
            if (!m_Application.Context.Request.IsLocal && Wim.CommonConfiguration.FORCE_SSL && !m_Application.Context.Request.IsSecureConnection)
            {
                string remapped = string.Concat(Wim.Utility.GetCurrentHost(false).ToLower().Replace("http", "https"), Wim.Utility.GetSafeUrl(m_Application.Context.Request).Remove(0, 1));
                m_Application.Context.Response.RedirectPermanent(remapped);
            }
        }

        void Force_SSL(Sushi.Mediakiwi.Data.Page page)
        {
            if (!m_Application.Context.Request.IsLocal && page != null && !page.IsNewInstance && (Wim.CommonConfiguration.FORCE_SSL || page.IsSecure))
            {
                if (!m_Application.Context.Request.IsSecureConnection)
                {
                    string remapped = string.Concat(Wim.Utility.GetCurrentHost(false).ToLower().Replace("http", "https"), Wim.Utility.GetSafeUrl(m_Application.Context.Request).Remove(0, 1));
                    m_Application.Context.Response.RedirectPermanent(remapped);
                }
            }
        }

        void RewritePage(Sushi.Mediakiwi.Data.Page page, Sushi.Mediakiwi.Data.Site site, Sushi.Mediakiwi.Data.IPageMapping map, int statusCode)
        {
            if (_OutputCompressor == null)
                _OutputCompressor = Data.Environment.GetInstance<IOutputCompressor>();

            if (_OutputCompressor.IsActive)
            {
                using (Wim.Utilities.CacheItemManager cman = new Utilities.CacheItemManager())
                {
                    string key = Wim.Utility.GetSafeUrl(m_Application.Request);
                    if (cman.IsCached(key) && !m_Application.Request.HttpMethod.Equals("POST", StringComparison.OrdinalIgnoreCase))
                    {
                        m_Application.Response.Write(cman.GetItem(key).ToString());
                        m_Application.Response.Flush();
                        m_Application.Response.End();
                    }
                    else
                    {
                        ResponseFilterStream filter = new ResponseFilterStream(m_Application.Response.Filter);
                        if (page != null && page.Template.OutputCacheDuration.GetValueOrDefault() > 0)
                        {
                            _outputCacheDuration = page.Template.OutputCacheDuration.Value;
                            filter.TransformStream += Filter_TransformCacheStream;
                        }
                        else
                            filter.TransformStream += Filter_TransformStream;

                        m_Application.Response.Filter = filter;
                    }
                }
            }

            m_Application.Context.Items.Add("Wim.Page", page);
            m_Application.Context.Items.Add("Wim.Site", site);

            m_ActionUrl = Wim.Utility.GetSafeUrl(m_Application.Request);
            if (!System.Uri.IsWellFormedUriString(m_ActionUrl, UriKind.Relative))
                throw new Exception("Ilegal characters in the url");

            m_Application.Context.Items.Add("Wim.Url", m_ActionUrl);

            m_CurrentPage = page;

            string url;
            if (map == null || map.IsNewInstance)
            {
                if ((page == null || page.IsNewInstance))
                {
                    if (!string.IsNullOrEmpty(Wim.CommonConfiguration.LOCAL_PAGE_NOT_FOUND))
                        url = string.Concat((m_Application.Context.Request.ApplicationPath + Wim.CommonConfiguration.LOCAL_PAGE_NOT_FOUND), GetQuery(map));
                    else
                        return;
                }
                else
                    url = (m_Application.Context.Request.ApplicationPath + page.Template.Location);
            }
            else
            {
                //m_ActionUrl = Wim.Utility.GetSafeUrl(m_Application.Request);
                url = string.Concat((m_Application.Context.Request.ApplicationPath + page.Template.Location), GetQuery(map));
            }

            m_Application.Context.Response.Cache.SetCacheability(HttpCacheability.NoCache);

            Force_SSL(page);


            if (map != null && map.IsActive && !page.Template.IsSourceBased )
            {
                if (map.TargetType == 1)
                {
                    if (map.AssetID > 0)
                    {
                        Asset a = Asset.SelectOne(map.AssetID);
                        m_Application.Context.Response.Redirect(a.DownloadFullUrl);
                    }
                }
                else
                {
                    switch (map.TypeID)
                    {
                        default:
                            m_Application.Context.RewritePath(url, false);
                            break;
                        case (int)PageMappingType.NotFound404:
                            if (map.IsInternalLink)
                            {
                                m_Application.Context.Response.StatusCode = 404;
                                m_Application.Context.Response.Status = "404 Not Found";
                                HttpContext.Current.Server.Transfer(url);
                            }
                            else
                            {
                                m_Application.Context.Response.StatusCode = 302;
                                m_Application.Context.Response.Status = "302 Found";
                                m_Application.Context.Response.AddHeader("Location", map.RedirectTo);
                            }
                            break;
                        case (int)PageMappingType.Redirect301:
                            m_Application.Context.Response.StatusCode = 301;
                            m_Application.Context.Response.Status = "301 Moved Permanently";
                            m_Application.Context.Response.AddHeader("Location", map.RedirectTo);
                            break;
                        case (int)PageMappingType.Redirect302:
                            m_Application.Context.Response.StatusCode = 302;
                            m_Application.Context.Response.Status = "302 Found";
                            m_Application.Context.Response.AddHeader("Location", map.RedirectTo);
                            break;
                    }
                }
            }
            else
            {
                switch (statusCode)
                {
                    case 404:

                        /// [CB:01-07-2015] removed the 302... this is wrong, 404 should return 404 httpstatus code
                       // if (page == null)
                       // {
                        url =  url.Replace("//", "/");
                        
                            m_Application.Context.Response.StatusCode = 404;
                            m_Application.Context.Response.Status = "404 Not Found";
                            HttpContext.Current.Server.Transfer(url);
                       // }
                       // else
                        //{
                         //   m_Application.Context.Response.StatusCode = 302;
                         //   m_Application.Context.Response.Status = "302 Found";
                      //      m_Application.Context.Response.AddHeader("Location", page.HRefFull);
                        //}
                        break;
                    case 500:
                        if (page == null)
                        {
                            m_Application.Context.Response.StatusCode = 500;
                            m_Application.Context.Response.Status = "500 Internal Error";
                        }
                        else
                        {
                            m_Application.Context.Response.StatusCode = 302;
                            m_Application.Context.Response.Status = "302 Found";
                            m_Application.Context.Response.AddHeader("Location", page.HRefFull);
                        }
                        break;
                    default:
                        if (page != null && page.Template.IsSourceBased)
                        {
                            DataTemplate dt = new DataTemplate();
                            //  Set HttpContext
                            dt.ApplyHttpContext(m_Application.Context, Query(map, true));
                            dt.ParseSourceData(page.Template);
                            dt.ParsePage(page, page.Template);

                            m_Application.Context.Response.AddHeader("Content-Type", "text/html; charset=utf-8");
                            m_Application.Context.Response.Write(dt.Rendered);
                            m_Application.Context.Response.End();
                        }
                        else
                            m_Application.Context.RewritePath(url, false);
                        break;
                }
            }
        }

        #region HeartBeat
        string m_Url;
        private const string CACHE_ITEM_KEY = "WIM_Heartbeat";
        private const string CACHE_TRAC_KEY = "WIM_Livetrack";

        //private void DoVisitorPingCleanup()
        //{
        //    Hashtable visitors = GetVistors();
        //    DateTime dt = visitors.ContainsKey("o") ? (DateTime)visitors["o"] : DateTime.MinValue;
        //    if (dt == DateTime.MinValue) return;
        //    if (dt.Ticks > DateTime.Now.AddSeconds(-Wim.CommonConfiguration.LIVETRACK_INTERVAL).Ticks) return;

        //    IDictionaryEnumerator idict = visitors.GetEnumerator();
        //    List<CurrentVisitor> arr = new List<CurrentVisitor>();
        //    while (idict.MoveNext())
        //    {
        //        string key = idict.Key.ToString();
        //        if (key == "o" || key == "c" || key == "p") continue;
        //        arr.Add(idict.Value as CurrentVisitor);
        //    }
        //    var selection = (from item in arr where item.Checked.Ticks < DateTime.Now.AddSeconds(-(Wim.CommonConfiguration.LIVETRACK_INTERVAL+2)).Ticks select new { ID = item.ID }).ToArray();
        //    foreach (var item in selection)
        //        visitors.Remove(item.ID);

        //    AddVisitorPingCache(visitors);
        //}

        //Hashtable GetVistors()
        //{
        //    Hashtable visitors = null;
        //    if (HttpContext.Current.Cache[CACHE_TRAC_KEY] != null)
        //        visitors = HttpContext.Current.Cache[CACHE_TRAC_KEY] as Hashtable;

        //    if (visitors == null)
        //        visitors = new Hashtable();
        //    return visitors;
        //}

        //void AddVisitorPingCache(Hashtable visitors)
        //{
        //    if (!visitors.ContainsKey("o")) visitors.Add("o", DateTime.Now);
        //    else
        //        visitors["o"] = DateTime.Now;
        //    if (!visitors.ContainsKey("c")) visitors.Add("c", DateTime.Now);
        //    if (!visitors.ContainsKey("p"))
        //        visitors.Add("p", string.Format("WIM live support, contains {0} visitor(s), last check {1}", visitors.Count - 2, DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss")));
        //    else
        //        visitors["p"] = string.Format("WIM live support, contains {0} visitor(s), last check {1}", visitors.Count - 3, DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"));

        //    HttpContext.Current.Cache.Add(CACHE_TRAC_KEY, visitors, null, System.Web.Caching.Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(1), CacheItemPriority.NotRemovable, null);
        //}

        /// <summary>
        /// Does the visitor ping.
        /// </summary>
        /// <param name="referrer">The referrer.</param>
        /// <param name="logID">The log ID.</param>
        /// <param name="callUsingAjax">if set to <c>true</c> [call using ajax].</param>
        //private void DoVisitorPing(Uri referrer, int logID, bool callUsingAjax)
        //{
        //    Sushi.Mediakiwi.Data.Identity.Visitor visitor = new Sushi.Mediakiwi.Data.Identity.Visitor();
        //    Sushi.Mediakiwi.Data.Identity.Profile profile = new Sushi.Mediakiwi.Data.Identity.Profile();
        //    //  Set info from cookie without DB
        //    if (visitor.HasVisitorReference) { }
        //    if (profile.HasVisitorReference) { }

        //    Hashtable visitors = GetVistors();

        //    CurrentVisitor current;
        //    if (visitors.ContainsKey(visitor.ID))
        //        current = visitors[visitor.ID] as CurrentVisitor;
        //    else
        //        current = new CurrentVisitor();

        //    current.ID = visitor.ID;
        //    current.LogID = logID;
        //    current.ProfileID = profile.ID;
        //    current.Checked = DateTime.Now;

        //    if (current.Url != referrer)
        //        current.Start = DateTime.Now;

        //    current.Url = referrer;
        //    visitors[visitor.ID] = current;

        //    if (!visitors.ContainsKey(visitor.ID))
        //        visitors.Add(visitor.ID, current);

        //    AddVisitorPingCache(visitors);
        //    m_Application.Response.Write(" ");
        //}

        public class CurrentVisitor
        {
            public int ID { get; set; }
            public int LogID { get; set; }
            public int ProfileID { get; set; }
            public DateTime Start { get; set; }
            public DateTime Checked { get; set; }
            public Uri Url { get; set; }
            public string GetTime()
            {
                TimeSpan ts = new TimeSpan(Checked.Ticks - Start.Ticks);
                return string.Format("{0:00}:{1:00}:{2:00}", ts.Hours, ts.Minutes, ts.Seconds);

            }
        }

        //private void DoHeartBeat(DateTime introduced)
        //{
        //    if (!Wim.CommonConfiguration.HAS_HEARTBEAT) return;
        //    // Prevent duplicate key addition
        //    if (null != HttpContext.Current.Cache[CACHE_ITEM_KEY]) return;

        //    if (string.IsNullOrEmpty(m_Url))
        //    {
        //        string license = Sushi.Mediakiwi.Data.Environment.Current.License;

        //        string prefix = Wim.Utility.GetCurrentHost();
        //        if (!string.IsNullOrEmpty(Wim.CommonConfiguration.HTTP_IMPERSONATION_URL))
        //            prefix = Wim.CommonConfiguration.HTTP_IMPERSONATION_URL;

        //        m_Url = string.Concat(prefix, Wim.Utility.AddApplicationPath("/repository/tcl.aspx?sense="), license);
        //    }

        //    DateTime expiration = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, 0);
        //    expiration = expiration.AddMinutes(1);

        //    Hashtable ht = new Hashtable();
        //    ht.Add("o", m_Url);
        //    ht.Add("c", introduced == DateTime.MinValue ? DateTime.Now : introduced);
        //    ht.Add("p", string.Concat("WIM Scheduler heartbeat, expires ", expiration.ToString("dd-MM-yyyy HH:mm:ss")));

        //    HttpContext.Current.Cache.Add(CACHE_ITEM_KEY, ht, null, expiration, TimeSpan.Zero, CacheItemPriority.NotRemovable, new CacheItemRemovedCallback(CacheItemRemovedCallback));

        //    //  Every full hour measure
        //    if (DateTime.Now.Minute == 0)
        //        Sushi.Mediakiwi.Data.Notification.InsertOne("Sense heartbeat", Sushi.Mediakiwi.Data.NotificationType.Information, "OK");

        //    ValidatePagePublication();
        //}

        /// <summary>
        /// Caches the item removed callback.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <param name="reason">The reason.</param>
        public void CacheItemRemovedCallback(string key, object value, CacheItemRemovedReason reason)
        {
            Hashtable ht = value as Hashtable;
            if (ht != null)
                HitPage(ht["o"].ToString(), (DateTime)ht["c"]);
        }

        /// <summary>
        /// Hits the page.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="startDate">The start date.</param>
        private void HitPage(string url, DateTime startDate)
        {
            try
            {
                WebClient client = new WebClient();
                if (!string.IsNullOrEmpty(Wim.CommonConfiguration.HTTP_IMPERSONATION_USER))
                    client.Credentials = new NetworkCredential(Wim.CommonConfiguration.HTTP_IMPERSONATION_USER, Wim.CommonConfiguration.HTTP_IMPERSONATION_PASSWORD);

                client.DownloadData(string.Concat(url, "&beat=", startDate.Ticks));
            }
            catch (Exception ex)
            {
                Sushi.Mediakiwi.Data.Notification.InsertOne("Sense heartbeat", Sushi.Mediakiwi.Data.NotificationType.Error, ex);
            }
        }

        void ValidatePagePublication()
        {
            var pagePublicationHandler = Sushi.Mediakiwi.Data.Environment.GetInstance<IPagePublication>();
            Sushi.Mediakiwi.Data.Page[] arr = Sushi.Mediakiwi.Data.Page.SelectAllDated();
            foreach (Sushi.Mediakiwi.Data.Page instance in arr)
            {
                if (instance.Expiration != DateTime.MinValue && DateTime.Now >= instance.Expiration)
                {
                    instance.TakeDown(pagePublicationHandler, null);
                    instance.Expiration = DateTime.MinValue;
                    instance.Save();
                }
                if (instance.Publication != DateTime.MinValue && DateTime.Now >= instance.Publication)
                {
                    instance.Publish(pagePublicationHandler, null);
                    instance.Publication = DateTime.MinValue;
                    instance.Save();
                }
            }
        }
        #endregion

        public void VerifyReservedCalls()
        {
            #region Sorting QueryString["list"]
            if (!string.IsNullOrEmpty(m_Application.Context.Request.QueryString["sortF"]))
            {
                int list = Utility.ConvertToInt(m_Application.Context.Request.QueryString["list"]);
                int sortF = Utility.ConvertToInt(m_Application.Context.Request.QueryString["sortF"]);
                int sortT = Utility.ConvertToInt(m_Application.Context.Request.QueryString["sortT"]);

                if (list > 0 && sortF > 0 && sortT > 0)
                {
                    Data.IComponentList implement = Data.ComponentList.SelectOne(list);

                    System.IO.FileInfo nfo = new System.IO.FileInfo(m_Application.Context.Server.MapPath(string.Concat(m_Application.Context.Request.ApplicationPath, "/bin/", implement.AssemblyName)));
                    Assembly assem = Assembly.LoadFrom(nfo.FullName);
                    Type m_LoadedType = assem.GetType(implement.ClassName);
                    IComponentListTemplate currentListInstance = System.Activator.CreateInstance(m_LoadedType) as IComponentListTemplate;
                    if (currentListInstance != null)
                    {
                        if (currentListInstance.wim.CurrentList.Type == Sushi.Mediakiwi.Data.ComponentListType.Property)
                        {
                            string cachekey = string.Concat("Data_Sushi.Mediakiwi.Data.Property.All$List_", currentListInstance.wim.CurrentList.ID);
                            Wim.Utilities.CacheItemManager.FlushIndexOfCacheObjects(cachekey);
                        }
                        if (currentListInstance.wim.m_sortOrderSqlTable.Contains("wim_Form"))
                        {
                            Wim.Utilities.CacheItemManager.FlushIndexOfCacheObjects("Data_FormElement_");
                        }

                        // [MR:10-09-2014] added the SortorderPortal to the arguments
                        if (Sushi.Mediakiwi.Beta.GeneratedCms.Supporting.SortOrderUpdate.UpdateSortOrder(currentListInstance.wim.m_sortOrderSqlTable, currentListInstance.wim.m_sortOrderSqlColumn, currentListInstance.wim.m_sortOrderSqlKey, sortF, sortT, currentListInstance.wim.m_sortOrderPortal))
                            m_Application.Context.Response.Write("OK");
                        else
                            m_Application.Context.Response.Write("ERROR#2");
                    }
                    else
                        m_Application.Context.Response.Write("ERROR#1");

                    m_Application.Context.Response.Flush();
                    m_Application.Context.Response.End();
                }
            }
            #endregion
        }

        /// <summary>
        /// Rewrites the asset path.
        /// </summary>
        /// <param name="context">The context.</param>
        public void RewriteAssetPath(HttpContext context)
        {
            try
            {
                string[] split1 = context.Request.Path.Split(new string[] { "/cdn/" }, StringSplitOptions.None);
                string[] split2 = split1[1].Split('/');

                string portal = null;
                if (split2.Length > 2)
                    portal = Wim.Utilities.Encryption.Decrypt(split2[split2.Length - 3], "cdn", true);

                string relative = Wim.Utility.AddApplicationPath(string.Concat("/repository/generated/", split1[1]));
                string filename = context.Server.MapPath(Wim.Utility.AddApplicationPath(string.Concat("/repository/generated/", split1[1])));

                var crypt = split2[split2.Length -2];
                var asset = split2[split2.Length -1].Split('.')[0];
                
                if (System.IO.File.Exists(filename))
                {
                   var gfx = Sushi.Mediakiwi.Data.Image.SelectOne(Wim.Utility.ConvertToInt(asset));

                   if (gfx.Created.Ticks < System.IO.File.GetLastWriteTime(filename).Ticks)
                   {
                       context.Response.Cache.SetCacheability(HttpCacheability.Public);
                       context.RewritePath(relative);
                       return;
                   }
                }

                var decrypt = Wim.Utilities.Encryption.Decrypt(crypt, "cdn", true);
                AssetHandler handler = new AssetHandler();
                handler.ProcessRequest(context, string.Concat(asset, ",", decrypt), portal);
            }
            catch (Exception ex)
            {
                Sushi.Mediakiwi.Data.Notification.InsertOne("RewriteAssetPath", ex);
            }
        }

        internal System.Collections.Specialized.NameValueCollection Query(Sushi.Mediakiwi.Data.IPageMapping map)
        {
            return Query(map, true);
        }

        internal System.Collections.Specialized.NameValueCollection Query(Sushi.Mediakiwi.Data.IPageMapping map, bool includeMapQuery)
        {
            System.Collections.Specialized.NameValueCollection query = null;
            if (!includeMapQuery || map == null || string.IsNullOrEmpty(map.Query))
                query = m_Application.Context.Request.QueryString;
            else
            {
                query = new System.Collections.Specialized.NameValueCollection();
                string[] split = map.Query.Split('&');

                foreach (var item in split)
                {
                    string[] element = item.Split('=');
                    if (element.Length == 2)
                    query.Add(element[0], element[1]);
                    else if(element.Length == 1)
                        query.Add(element[0], string.Empty);
                    
                }
                if (m_Application.Context.Request.QueryString.Count > 0)
                {
                    foreach (string key in m_Application.Context.Request.QueryString.AllKeys)
                    {
                        query.Add(key, m_Application.Context.Request.QueryString[key]);
                    }
                }
            }
            return query;
        }

        string GetQuery(Sushi.Mediakiwi.Data.IPageMapping map)
        {
            return GetQuery(map, true);
        }

        /// <summary>
        /// Gets the query.
        /// </summary>
        /// <param name="map">The map.</param>
        /// <param name="includeMapQuery">if set to <c>true</c> [include map query].</param>
        /// <returns></returns>
        string GetQuery(Sushi.Mediakiwi.Data.IPageMapping map, bool includeMapQuery)
        {
            var query = Query(map, includeMapQuery);
            string queryString = query.Count == 0 ? "" : "?";

            bool isFirst = true;
            foreach(string item in query.Keys)
            {
                if (isFirst)
                {
                    isFirst = false;
                    queryString += string.Concat(item, "=", query[item]);
                    continue;
                }
                queryString += string.Concat("&", item, "=", query[item]);
                
            }
            return queryString;
        }

        /// <summary>
        /// Verifies the version.
        /// </summary>
        void VerifyVersion()
        {
            bool isLocal = m_Application.Request.IsLocal || Wim.CommonConfiguration.IS_LOCAL_DEVELOPMENT;
            bool updateSystemForcing = (m_Application.Request.QueryString["updatesystem"] == "now");
            if (updateSystemForcing)
            {
                if (!isLocal)
                {
                    if (!Sushi.Mediakiwi.Data.ApplicationUserLogic.Select().IsDeveloper)
                        updateSystemForcing = false;
                }
            }

            if (!isLocal && !updateSystemForcing) 
                return;



            IEnvironmentVersion versionData = null;
            try
            {
                versionData = EnvironmentVersion.Select();
            }
            catch(Exception)
            {
                versionData = new EnvironmentVersion() {  };
            }
            decimal version = versionData.Version;

            if (version < Wim.CommonConfiguration.Version || updateSystemForcing)
            {
                try
                {
                    Sushi.Mediakiwi.Data.Notification.InsertOne("Version update", NotificationType.Information, string.Format("From {0} to {1}", Sushi.Mediakiwi.Data.Environment.Current.Version, Wim.CommonConfiguration.Version));
                }
                catch (Exception exc) {
                    //m_Application.Response.Write(exc);
                }

                m_Application.Response.Write("<html><body>");

                Sushi.Mediakiwi.Framework.Functions.DataCompare dc = new Framework.Functions.DataCompare();
                dc.Verify(Resource.wim_base);

                if (m_Application.Request.QueryString["Execute"] == "1")
                {
                    if (dc.m_DataList.Count > 0)
                    {
                        dc.Start();
                       
                        var versionInfo = EnvironmentVersion.Select();
                        var previousVersion = versionInfo.Version;

                        versionInfo.Version = Wim.CommonConfiguration.Version;
                        versionInfo.Save();

                        if (previousVersion < 1)
                            m_Application.Response.Redirect("wim.ashx?reset=11e69ff4-81b0-447a-a06d-1f566bbc75e8&u=dE0rHoD9oO6NYk3C8QKuvGQZjeLDNTW7LTxASdjUoUc%3d");
                        else
                            m_Application.Response.Redirect("wim.ashx");

                        //http://www.shiftnet.nl/login?reset=11e69ff4-81b0-447a-a06d-1f566bbc75e8&u=dE0rHoD9oO6NYk3C8QKuvGQZjeLDNTW7LTxASdjUoUc%3d
                        //if (m_Application.Request.IsLocal || updateSystemForcing)
                        //{
                        //    m_Application.Response.Write("<hr />If you see no errors, please refresh!");
                        //    m_Application.Response.Write("</body></html>");

                        //    m_Application.Response.End();
                        //}
                    }
                }
                else
                {
                    if (dc.m_DataList.Count > 0)
                    {
                        m_Application.Response.Write("<hr /><a href=\"?updatesystem=now&Execute=1\">Execute queries (required action)</a><br/>");
                        foreach (var item in dc.m_DataList)
                        {
                            m_Application.Response.Write(string.Format("<hr/>Type: <b>{0}</b><br/>{1}<br/>", item.Type, item.Script));
                        }
                        m_Application.Response.Write("</body></html>");

                        m_Application.Response.End();
                    }
                    else
                    {
                        Sushi.Mediakiwi.Data.Environment.Current.Version = Wim.CommonConfiguration.Version;
                        Sushi.Mediakiwi.Data.Environment.Current.Save();

                        m_Application.Response.Write("<hr />No SQL changed detected, press F5 to refresh.");
                        m_Application.Response.Write("</body></html>");

                        m_Application.Response.End();
                    }
                }
            }
        }

        static IRequestRouter _requestRouter;

        string[] _Exclude;
        bool CanProceed()
        {
            if (_Exclude == null)
            {
                if (string.IsNullOrEmpty(Wim.CommonConfiguration.EXCLUDE_PATH_HANDLER))
                    _Exclude = new string[0];
                else
                    _Exclude = Wim.CommonConfiguration.EXCLUDE_PATH_HANDLER.Split('|');
            }
            if (_Exclude.Length == 0)
                return true;
           
            foreach (var path in _Exclude)
            {
                if (m_Application.Context.Request.Path.Contains(path))
                    return false;
            }
            return true;

        }

        static Dictionary<CallbackTarget, List<ICallback>> Callbacks = new Dictionary<CallbackTarget, List<ICallback>>();
        public static void RegisterCallback(CallbackTarget target, ICallback @object)
        {
            if (Callbacks.ContainsKey(target))
            {
                var list = Callbacks[target];
                if (list.Where(x => x.UID.Equals(@object.UID)).Count() == 0)
                    Callbacks[target].Add(@object);
            }
            else
            {
                var list = new List<ICallback>();
                list.Add(@object);
                Callbacks.Add(target, list);
            }
        }

        static Dictionary<string, List<ILink>> Links = new Dictionary<string, List<ILink>>();
        /// <summary>
        /// Multi entities can be recorded, be sure to only add one instance
        /// </summary>
        /// <param name="path"></param>
        /// <param name="type"></param>
        public static void RegisterLink(string path, ILink type)
        {
            if (Links.ContainsKey(path))
            {
                var list = Links[path];
                if (list.Where(x => x.UID.Equals(type.UID)).Count() == 0)
                    Links[path].Add(type);
            }
            else
            {
                var list = new List<ILink>();
                list.Add(type);
                Links.Add(path, list);
            }
        }

      

        static Dictionary<GlobalPlaceholder, string> Placeholders = new Dictionary<GlobalPlaceholder, string>();
        public static void RegisterPlaceholder(GlobalPlaceholder placeholder, string markup)
        {
            if (Placeholders.ContainsKey(placeholder))
                Placeholders[placeholder] = markup;
            else
                Placeholders.Add(placeholder, markup);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <param name="e"></param>
        public void OnBeginRequest(Object s, EventArgs e)
        {
            m_Application = (System.Web.HttpApplication)s;

            var appuser = Authenticate();
            SecureConnection();

            if (HttpContext.Current != null)
                HttpContext.Current.Items["wim.applicationuser"] = appuser;

            var path2 = Wim.Utility.RemApplicationPath(m_Application.Context.Request.Path.ToLower());
            if (Links.ContainsKey(path2))
            {
                var apps = Links[path2];
                foreach(var app in apps)
                    if (!app.Init(m_Application.Context))
                        return;
            }


            //10 december 2017, MM: Resolves the need for a response.End as this event is called multiple times.
            if (m_Application.Response.HeadersWritten)
            {
                //m_Application.CompleteRequest();
                return;
            }
            m_Application.Context.Items["wim.executionTime"] = DateTime.Now.Ticks;

            this.ForceTopLevel_SSL();

            m_CurrentPage = null;
            string entryPointWim = null;

            try
            {
                entryPointWim = Sushi.Mediakiwi.Data.Environment.Current.RelativePath;
            }
            catch (Exception)
            {
                //  Some could have been changed on the enviroment
                this.VerifyVersion();
            }

            if (m_Application.Context.Request.Path.ToLower().EndsWith("multifileupload.ashx"))
            {
                MultifileUploadHandler handler = new MultifileUploadHandler();
                handler.ProcessRequest(m_Application.Context);
                
            }

            var endpoints = new string[] { "portal.ashx", "wim.ashx" };
            foreach (var endpoint in endpoints)
            {
                if (m_Application.Context.Request.Path.ToLower().EndsWith(endpoint)
                    && !entryPointWim.ToLower().EndsWith(endpoint)
                    )
                {
                    if (m_Application.Request.QueryString.Count == 0)
                        m_Application.Response.Redirect(Wim.Utility.AddApplicationPath(string.Concat(entryPointWim)), true);
                    else
                        m_Application.Response.Redirect(Wim.Utility.AddApplicationPath(string.Concat(entryPointWim, "?", m_Application.Request.QueryString.ToString())), true);

                    return;
                }
            }

            bool iscms = m_Application.Context.Request.Path.ToLower().StartsWith(Sushi.Mediakiwi.Data.Environment.Current.GetPath().ToLower());

            #region WIM
            if (iscms || (!string.IsNullOrEmpty(entryPointWim) && m_Application.Context.Request.Path.ToLower().EndsWith(entryPointWim, StringComparison.OrdinalIgnoreCase)))
            {
                this.VerifyVersion();
                this.VerifyEnvironment();
                this.VerifyLoadBalancedCaching();
                this.VerifyReservedCalls();

                #region [14-09-13:MM] Check for wim access URL
                if (!Wim.CommonConfiguration.IS_LOCAL_DEVELOPMENT 
                    && !string.IsNullOrEmpty(Sushi.Mediakiwi.Data.Environment.Current.Domain)
                    && Sushi.Mediakiwi.Data.Environment.Current.Domain != " " // Escape for older versions
                    )
                {
                    string representativeDomain = Sushi.Mediakiwi.Data.Environment.Current.Domain;
                    if (!representativeDomain.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                        representativeDomain = string.Concat("http://", representativeDomain);
                    if (!representativeDomain.EndsWith("/"))
                        representativeDomain = string.Concat(representativeDomain, "/");

                    if (!representativeDomain.Equals(Wim.Utility.GetCurrentHost()))
                    {
                        m_Application.Response.Redirect(string.Concat(representativeDomain, Wim.Utility.GetSafeUrl(m_Application.Context.Request)));
                        return;
                    }
                }
                #endregion [14-09-13:MM] Check for wim access URL

                if (Wim.CommonConfiguration.SECURITY_IP_ACCESS_RANGE.Length > 0)
                {
                    var query = (from item in Wim.CommonConfiguration.SECURITY_IP_ACCESS_RANGE where item == m_Application.Request.UserHostAddress select item).ToList();
                    if (query.Count == 0)
                    {
                        Sushi.Mediakiwi.Data.Notification.InsertOne("SECURITY_IP_ACCESS_RANGE", Data.NotificationType.Warning, string.Format("Unauthorized request for {0}", m_Application.Request.UserHostAddress));

                        if (string.IsNullOrEmpty(Wim.CommonConfiguration.SECURITY_NO_ACCESS_PAGE))
                        {
                            m_Application.Response.StatusCode = 401;
                            m_Application.Response.Status = "403 Forbidden";
                            m_Application.Response.Write("403 Forbidden");
                            m_Application.Response.End();
                        }
                        else
                        {
                            int pagekey;
                            if (Wim.Utility.IsNumeric(Wim.CommonConfiguration.SECURITY_NO_ACCESS_PAGE, out pagekey))
                            {
                                var p = Sushi.Mediakiwi.Data.Page.SelectOne(pagekey);
                                RewritePage(p, p.Site, null);
                            }
                            else
                            {
                                m_Application.Response.Redirect(Wim.CommonConfiguration.SECURITY_NO_ACCESS_PAGE, false);
                                return;
                            }
                        }
                    }
                }


                //var appuser = Sushi.Mediakiwi.Data.ApplicationUser.Select();
                bool isNewDesign = true;// Wim.CommonConfiguration.FORCE_NEWSTYLE || Wim.CommonConfiguration.FORCE_NEWSTYLE2 || appuser.ShowNewDesign;
                //if (!isNewDesign && !string.IsNullOrEmpty(Wim.CommonConfiguration.NEWSTYLE_LOGIN_PATH))
                //    isNewDesign = m_Application.Context.Request.Path.EndsWith(Wim.CommonConfiguration.NEWSTYLE_LOGIN_PATH, StringComparison.OrdinalIgnoreCase);

                if (string.IsNullOrEmpty(m_Application.Context.Request.QueryString["xml"]))
                {
                    //m_ActionUrl = string.Concat(Wim.Utility.GetCurrentHost(), Wim.Utility.AddApplicationPath(Sushi.Mediakiwi.Data.Environment.SelectOne().RelativePath), "?", m_Application.Server.UrlDecode(m_Application.Context.Request.QueryString.ToString()));
                    m_ActionUrl = Wim.Utility.GetSafeUrl(m_Application.Request);

                    bool ignoreStreamFilter = !string.IsNullOrEmpty(m_Application.Context.Request.QueryString["export_xls"]);
                    if (ignoreStreamFilter)
                    {
                        if (_OutputCompressor == null)
                            _OutputCompressor = Data.Environment.GetInstance<IOutputCompressor>();

                        if (_OutputCompressor.IsActive)
                        {
                            ReplaceAction(m_ActionUrl, false);
                            ResponseFilterStream filter = new ResponseFilterStream(m_Application.Response.Filter);
                            filter.TransformStream += Filter_TransformStreamAction;
                            m_Application.Response.Filter = filter;
                        }
                        else
                        {
                            //  Old (fast) system
                            OutputStream mStreamFilter = new OutputStream(m_Application.Response.Filter, m_ActionUrl);
                            m_Application.Response.Filter = mStreamFilter;
                            m_Application.Context.Items.Add("mStreamFilter", mStreamFilter);
                        }
                    }


                    int pageID = Wim.Utility.ConvertToInt(m_Application.Context.Request.QueryString["page"]);
                    if (pageID > 0)
                    {
                        Sushi.Mediakiwi.Data.Page pageInstance = Sushi.Mediakiwi.Data.Page.SelectOne(pageID, false);

                        if (!pageInstance.IsNewInstance)
                        {
                            string path = m_Application.Context.Server.MapPath(Utility.AddApplicationPath(pageInstance.Template.Location));
                            if (System.IO.File.Exists(path))
                            {
                                System.IO.FileInfo nfo = new System.IO.FileInfo(path);
                                DateTime file = Wim.Utility.ConvertToSystemDataTime(nfo.LastWriteTimeUtc);

                                bool refreshcode = false;
                                //if (appuser.IsDeveloper)
                                    refreshcode = m_Application.Request.QueryString["refreshcode"] == "1";

                                // [MR:26-04-2019] when a file is smaller than 120 bytes, it's probably a marker file and
                                // should not trigger an update.
                                if (refreshcode)
                                {
                                    if (pageInstance.Template.IsSourceBased)
                                    {
                                        DataTemplate dt = new DataTemplate();
                                        dt.ParseSourceData(pageInstance.Template);
                                        //dt.ParsePage(pageInstance);
                                    }
                                    else
                                    {
                                        m_Application.Context.Items["Wim.Page"] = pageInstance;
                                        m_Application.Context.Items["Wim.Site"] = pageInstance.Site;

                                        m_Application.Context.RewritePath(Wim.Utility.AddApplicationPath(string.Format("{0}?loadcomponentinstance=1&refreshcode={1}", pageInstance.Template.Location, refreshcode ? "1" : "0")));

                                        pageInstance.Template.LastWriteTimeUtc = file;
                                        pageInstance.Template.Save();
                                        return;
                                    }
                                }
                            }
                            else
                            {
                                if (Wim.CommonConfiguration.WEB_DEBUG && m_Application.Request.IsLocal)
                                {
                                    m_Application.Response.Write(string.Format("Template not found: {0}<br/>", path));
                                }
                            }
                        }
                    }
                  
                    if (isNewDesign)// && !string.IsNullOrEmpty(m_Application.Request.QueryString["list"]))
                    {
                        HttpContext.Current.Items.Add("NewDesign", "1");

                        var preMonitorHook = Sushi.Mediakiwi.Data.Environment.GetInstance<iPreMonitorHook>();
                        preMonitorHook.VerifyRequest(m_Application);

                        var display = new Sushi.Mediakiwi.Framework.UI.Monitor(m_Application, appuser, Placeholders, Callbacks, false) { IsNewDesignOutput = isNewDesign };
                        display.Start();
                    }
                    else
                    {
                        Beta.GeneratedCms.Monitor monitor = new Sushi.Mediakiwi.Beta.GeneratedCms.Monitor(m_Application);
                        m_Application.Context.RewritePath(string.Concat("~/repository/tcl.aspx?channel=", monitor.GetChannelIndentifier(), "&", m_Application.Context.Request.QueryString), false);
                    }
                    return;
                }
                else
                {
                    if (isNewDesign)
                    {
                        HttpContext.Current.Items.Add("NewDesign", "1");
                        var display = new Sushi.Mediakiwi.Framework.UI.Monitor(m_Application) { IsNewDesignOutput = isNewDesign };
                        display.Start();
                    }
                    else
                    {
                        Beta.GeneratedCms.Monitor monitor = new Sushi.Mediakiwi.Beta.GeneratedCms.Monitor(m_Application);
                        monitor.Start();
                        return;
                    }
                }
            }
            #endregion

            if (Wim.CommonConfiguration.IGNORE_WEBSITE_HANDLER)
                return;

            //  Content path
            if (m_Application.Context.Request.Path.Contains("/cdn/"))
            {
                RewriteAssetPath(m_Application.Context);
                return;
            }

            // CB 14-11-2014;  a patch for WCF ajax enabled webservices.
            //    02-12-2014;  a simalar patch for signalr 
            if (m_Application.Context.Request.Path.Contains(".svc") || m_Application.Context.Request.Path.Contains("/signalr/"))
                return;

            if (m_Application.Context.Request.Path.EndsWith("/mklytics"))
            {
                PageAnalytics.Default().Register(m_Application.Context.Request);
                m_Application.Context.Response.ClearContent();
                m_Application.Context.Response.End();
                return;
            }

            if (!CanProceed())
                return;

            //  Hook for custom page mappings
            if (_requestRouter == null)
                _requestRouter = Data.Environment.GetInstance<IRequestRouter>();
            _requestRouter.VerifyRequest(m_Application);

            Sushi.Mediakiwi.Data.IPageMapping map = null;
            bool isPossibleWimCall = IsPossibleWimCall(m_Application.Context, ref map);


            #region No Wim
            if (!isPossibleWimCall)
        
            {
                string ext = Utility.RemApplicationPath(m_Application.Context.Request.Url.AbsolutePath).ToLower();
                // first check for possible physical file.
                foreach (var extention in ignoreExtentionList)
                {
                    if (string.IsNullOrEmpty(extention))
                        continue;
                    
                    if (ext.EndsWith("css", StringComparison.InvariantCultureIgnoreCase)
                        || ext.EndsWith("js", StringComparison.InvariantCultureIgnoreCase) 
                        || ext.EndsWith("png", StringComparison.InvariantCultureIgnoreCase)
                        || ext.EndsWith("jpg", StringComparison.InvariantCultureIgnoreCase)
                        || ext.EndsWith("woff", StringComparison.InvariantCultureIgnoreCase)
                        || ext.EndsWith("woff2", StringComparison.InvariantCultureIgnoreCase)
                        || ext.EndsWith("ttf", StringComparison.InvariantCultureIgnoreCase)
                        || ext.EndsWith("ico", StringComparison.InvariantCultureIgnoreCase)
                        )
                    {
                        string cache = Sushi.Mediakiwi.Data.Environment.Current["CACHE-CONTROL", true, "public, max-age=2678400", "Cache-control"];

                        // 31 dagen
                        if (!string.IsNullOrEmpty(cache))
                            m_Application.Context.Response.AddHeader("cache-control", cache);
                        return;
                    }

                    if (ext.EndsWith(extention, StringComparison.InvariantCultureIgnoreCase))
                    {
                        return;
                    }
                }
                
                
                Sushi.Mediakiwi.Data.Site site2 = null;
                Sushi.Mediakiwi.Data.Page page2 = null;

                string filePath = m_Application.Server.MapPath(m_Application.Context.Request.Path);
                string sitedetectionPath = Wim.Utility.RemApplicationPath(m_Application.Request.Path);

                string path = "/";
                string searchPath = path;
                if (sitedetectionPath.LastIndexOf("/") > 0)
                {
                    path = string.Format("{0}/", sitedetectionPath.Substring(0, sitedetectionPath.LastIndexOf("/")));
                    searchPath = path.Replace("/", "/");
                }

                //  Trial 1
                site2 = Sushi.Mediakiwi.Data.Site.SelectOne(searchPath);

                //  Trial 2: Get the environmental default site
                if (site2.IsNewInstance)
                {
                    if (Sushi.Mediakiwi.Data.Environment.Current.DefaultSiteID.HasValue)
                        site2 = Sushi.Mediakiwi.Data.Site.SelectOne(Sushi.Mediakiwi.Data.Environment.Current.DefaultSiteID.Value);
                }

                //  Trial 3: Last resort.
                if (site2.IsNewInstance)
                {
                    site2 = Sushi.Mediakiwi.Data.Site.SelectOne(null);
                }


                if (!System.IO.File.Exists(filePath))
                {
                    int code = 200;
                    if (site2.PageNotFoundID.HasValue)
                    {
                        code = 404;
                        page2 = Sushi.Mediakiwi.Data.Page.SelectOne(site2.PageNotFoundID.Value);
                    }

                    if (page2 == null && site2.ErrorPageID.HasValue)
                    {
                        page2 = Sushi.Mediakiwi.Data.Page.SelectOne(site2.ErrorPageID.Value);
                    }

                    RewritePage(page2, site2, map, code);
                }
                return;
            }
            #endregion No Wim

            //  [MM:21.12.14] Force the management login
            if (Wim.CommonConfiguration.FORCE_MANAGEMENT_LOGIN)
                m_Application.Response.Redirect(Sushi.Mediakiwi.Data.Environment.Current.GetPath());

            Sushi.Mediakiwi.Data.Page page;
            if (map == null || map.IsNewInstance)
                page = this.GetPage(m_Application, m_Application.Context.Request.Url, out map);
            else
            {
                Sushi.Mediakiwi.Data.Site site2;
                string domainPrefix = Sushi.Mediakiwi.Data.Site.GetDomainPrefix(m_Application.Context.Request.Url, out site2);
                if (site2 != null && !site2.IsNewInstance)
                    page = Sushi.Mediakiwi.Data.Page.SelectOneChild(map.PageID, site2.ID);
                else
                    page = Sushi.Mediakiwi.Data.Page.SelectOne(map.PageID);

                //  Domain validation for mapping
                if (Wim.CommonConfiguration.REDIRECT_CHANNEL_PATH)
                {
                    if (!string.IsNullOrEmpty(page.Site.Domain) && !page.Site.IsDomain(m_Application.Request.Url))
                    {
                        string redirect = string.Concat("http://", 
                            Utility.GlobalRegularExpression.Implement.CleanRelativePathSlash.Replace(
                                string.Format("{0}/{1}", page.Site.Domains[0], Wim.Utility.RemApplicationPath(m_Application.Request.Url.PathAndQuery))
                            ,   "/"));

                        if (Wim.CommonConfiguration.WIM_DEBUG)
                            Sushi.Mediakiwi.Data.Notification.InsertOne("Domain validation (2)", Sushi.Mediakiwi.Data.NotificationType.Information, redirect);

                        m_Application.Response.Redirect(redirect);
                    }
                }
                //  Domain validation for mapping
            }


            Sushi.Mediakiwi.Data.Site site = null;

            if (page != null)
            {
                //  Only from within
                //this.VerifyVersion();
                this.VerifyEnvironment();
                this.VerifyLoadBalancedCaching();

                if (!string.IsNullOrEmpty(m_Application.Context.Request.QueryString["acc"]))
                {
                    string componentID = m_Application.Context.Request.QueryString["acc"];
                    //int componentID;
                    //if (Wim.Utility.IsNumeric(m_Application.Context.Request.QueryString["acc"], out componentID))
                    //{
                    m_Application.Context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
                    m_Application.Context.RewritePath(string.Concat("~/repository/tcl.aspx?c=", componentID, "&p=", page.ID, "&i=", m_Application.Context.Request.QueryString["i"]), false);
                    return;
                    //}
                }
                if (!string.IsNullOrEmpty(m_Application.Context.Request.QueryString["act"]))
                {
                    int componentID;
                    if (Wim.Utility.IsNumeric(m_Application.Context.Request.QueryString["act"], out componentID))
                    {
                        m_Application.Context.RewritePath(string.Concat("~/repository/tcl.aspx?ct=", componentID, "&p=", page.ID, "&i=", m_Application.Context.Request.QueryString["i"]), false);
                        return;
                    }
                }

                #region USE_PHYSICAL_PAGE_LEVEL_CACHE
                //if (!m_Application.Request.HttpMethod.Equals("POST", StringComparison.OrdinalIgnoreCase))
                //{
                   
                //    if (Sushi.Mediakiwi.Data.Environment.Current["USE_PHYSICAL_PAGE_LEVEL_CACHE"] == "1" && Wim.CommonConfiguration.HAS_PAGE_CACHE && page.IsPageFullyCachable())
                //    {
                //        var query = Query(map);
                //        if (page.Template.IsAddedOutputCache && System.IO.File.Exists(page.GetLocalCacheFile(query)))
                //        {
                //            System.IO.FileInfo nfo = new FileInfo(page.GetLocalCacheFile(query));
                //            DateTime dt1 = nfo.LastWriteTime.ToUniversalTime();
                //            DateTime dt2 = page.Published;
                //            DateTime dt3 = Sushi.Mediakiwi.Data.EnvironmentVersion.m_serverEnvironmentVersion.GetValueOrDefault();
                //            bool renew = (dt2.Ticks > dt1.Ticks || (dt3 != DateTime.MinValue && dt3.Ticks > dt1.Ticks));

                //            //  Renew on preview
                //            if (!renew && m_Application.Request.QueryString["preview"] == "1")
                //                renew = true;

                //            if (!renew && m_Application.Request.IsLocal)
                //            {
                //                //  Check for page template change
                //                System.IO.FileInfo nfo2 = new FileInfo(m_Application.Server.MapPath(Wim.Utility.AddApplicationPath(page.Template.Location)));
                //                System.IO.FileInfo nfo3 = new FileInfo(m_Application.Server.MapPath(Wim.Utility.AddApplicationPath(string.Concat(page.Template.Location, ".cs"))));
                //                renew = (nfo2.LastWriteTime.ToUniversalTime().Ticks > dt1.Ticks);
                //                renew = renew ? renew : (nfo3.LastWriteTime.ToUniversalTime().Ticks > dt1.Ticks);
                //                //  Check for componente template change
                //                if (!renew)
                //                {
                //                    foreach (Sushi.Mediakiwi.Data.ComponentTemplate ct in Sushi.Mediakiwi.Data.ComponentTemplate.SelectAllAvailable(page.TemplateID))
                //                    {
                //                        System.IO.FileInfo nfo4 = new FileInfo(m_Application.Server.MapPath(Wim.Utility.AddApplicationPath(ct.Location)));
                //                        System.IO.FileInfo nfo5 = new FileInfo(m_Application.Server.MapPath(Wim.Utility.AddApplicationPath(string.Concat(ct.Location, ".cs"))));
                //                        renew = renew ? renew : (nfo4.LastWriteTime.ToUniversalTime().Ticks > dt1.Ticks);
                //                        renew = renew ? renew : (nfo5.LastWriteTime.ToUniversalTime().Ticks > dt1.Ticks);
                //                        if (renew) break;
                //                    }
                //                }
                //            }

                //            if (!renew)
                //            {
                //                //string output = page.GetLocalCacheHref(Query(map));
                //                //m_Application.Context.RewritePath(output, false);
                //                //return;

                //                string url = string.Concat(Wim.Utility.AddApplicationPath("/repository/tcl.aspx"), "?p=", page.ID);

                //                foreach (string key in query.AllKeys)
                //                {
                //                    url += string.Concat("&", key, "=", query[key]);
                //                }

         
                //                m_Application.Context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
                //                m_Application.Context.RewritePath(url, false);
                //                return;

                //                //m_Application.Context.Response.Cache.SetCacheability(HttpCacheability.NoCache);

                //                //string output = page.GetLocalCacheHref(m_Application.Context.Request.QueryString);

                //                //m_Application.Context.RewritePath(output, false);
                //                //return;
                //            }
                //        }
                //    }
                   
                //}
                #endregion

                site = Sushi.Mediakiwi.Data.Site.SelectOne(page.SiteID);

                //  When the site is set to non-active and is not in preview mode: reset the page.
                if (!site.IsNewInstance && !site.IsActive && 
                   HttpContext.Current.Items["Wim.Preview"] != null &&
                    HttpContext.Current.Items["Wim.Preview"].ToString() != "1")
                {
                    return;
                }
            }

            string appPath = m_Application.Context.Request.ApplicationPath.ToLower();

            if (page != null)
            {
                RewritePage(page, site, map);
            }
            else
            {
                string filePath = m_Application.Server.MapPath(m_Application.Context.Request.Path);
                string sitedetectionPath = Wim.Utility.RemApplicationPath(m_Application.Request.Path);

                string path = "/";
                string searchPath = path;
                if (sitedetectionPath.LastIndexOf("/") > 0)
                {
                    path = string.Format("{0}/", sitedetectionPath.Substring(0, sitedetectionPath.LastIndexOf("/")));
                    searchPath = path.Replace("/", "/");
                }

                //  Trial 1
                // [MR:06-11-2013] This would not resolve a call to the root of the site.
                // In channel settings it states that the default path must NOT end with a /
                // But this trailing / is required for this call to succeed
                // [CB:01-07-2015] Remove slash only; this had an issue with youfone root 404
            
                if (searchPath == "/") searchPath = string.Empty;
                site = Sushi.Mediakiwi.Data.Site.SelectOneSiteResolution(searchPath, true);

                //  Trial 2: Get the environmental default site
                if (site.IsNewInstance)
                {
                    if (Sushi.Mediakiwi.Data.Environment.Current.DefaultSiteID.HasValue)
                        site = Sushi.Mediakiwi.Data.Site.SelectOne(Sushi.Mediakiwi.Data.Environment.Current.DefaultSiteID.Value);
                }

                //  Trial 3: Last resort.
                if (site.IsNewInstance)
                {
                    site = Sushi.Mediakiwi.Data.Site.SelectOne(null);
                }
                // [MR:06-11-2013] If there is a Site found with a homepage, but the Page is currently NULL,
                // Select the homepage for this Site
                else if (site.HomepageID.HasValue && page == null)
                {
                    page = Sushi.Mediakiwi.Data.Page.SelectOne(site.HomepageID.Value);
                }

                if (!System.IO.File.Exists(filePath))
                {
                    //  Trying to define which site to take
                    if (!site.IsNewInstance)
                    {
                        int code = 200;
                        if (site.PageNotFoundID.HasValue)
                        {
                            code = 404;
                            page = Sushi.Mediakiwi.Data.Page.SelectOne(site.PageNotFoundID.Value);
                        }

                        if (page == null && site.ErrorPageID.HasValue)
                        {
                            code = 500;
                            page = Sushi.Mediakiwi.Data.Page.SelectOne(site.ErrorPageID.Value);
                        }

                        //if (page != null)
                        //{
                            RewritePage(page, site, map, code);
                        //}
                    }
                }
            }
        }

        static IOutputCompressor _OutputCompressor;

        private MemoryStream Filter_TransformStream(MemoryStream ms)
        {
            Encoding encoding = HttpContext.Current.Response.ContentEncoding;

            string output = encoding.GetString(ms.ToArray());

            output = _OutputCompressor.Compress(output);

            ms = new MemoryStream(output.Length);

            byte[] buffer = encoding.GetBytes(output);
            ms.Write(buffer, 0, buffer.Length);

            return ms;
        }

        int _outputCacheDuration;
        private MemoryStream Filter_TransformCacheStream(MemoryStream ms)
        {
            using (Wim.Utilities.CacheItemManager cman = new Utilities.CacheItemManager())
            {
                Encoding encoding = HttpContext.Current.Response.ContentEncoding;

                string output = encoding.GetString(ms.ToArray());

                output = _OutputCompressor.Compress(output);
                var time = _outputCacheDuration;
                if (time > 0)
                    cman.Add(Wim.Utility.GetSafeUrl(m_Application.Request), output, DateTime.Now.AddSeconds(time));

                ms = new MemoryStream(output.Length);

                byte[] buffer = encoding.GetBytes(output);
                ms.Write(buffer, 0, buffer.Length);

                return ms;
            }
        }


        private Regex m_formAction = new Regex(@"action="".*?""", RegexOptions.IgnoreCase);
        private Regex m_source = new Regex(@"src=""http://", RegexOptions.IgnoreCase);
        private string m_sourceSecureReplacement = @"src=""//";
        private string m_formActionReplacement;

        private Regex m_robots = new Regex(@"meta name=""robots""", RegexOptions.IgnoreCase);
        private string m_robotsPlacement = "<meta name=\"robots\" content=\"noindex\">";
        private Regex m_head = new Regex(@"</head>", RegexOptions.IgnoreCase);


        void ReplaceAction(string href, bool skipQueryAddition)
        {
            m_formActionReplacement = string.Format("action=\"{0}\"", href);

            if (!skipQueryAddition && System.Web.HttpContext.Current.Request.QueryString.Keys.Count > 0 && !href.Contains("?"))
                m_formActionReplacement = string.Format("action=\"{0}?{1}\"", href, System.Web.HttpContext.Current.Request.QueryString.ToString());
            else
                m_formActionReplacement = string.Format("action=\"{0}\"", href);

        }

        private MemoryStream Filter_TransformStreamAction(MemoryStream ms)
        {
            Encoding encoding = HttpContext.Current.Response.ContentEncoding;

            string output = encoding.GetString(ms.ToArray());

            if (m_formActionReplacement != null)
            {
                output = m_formAction.Replace(output, m_formActionReplacement);
                //  Replace all HTTP source references when the line needs to be secure (so HTTP is replaced with HTTPS in case of src="http://"
                if (System.Web.HttpContext.Current.Request.IsSecureConnection)
                    output = m_source.Replace(output, m_sourceSecureReplacement);
            }

            if (this.CurrentPage != null && !this.CurrentPage.IsSearchable && !m_robots.IsMatch(output))
            {
                output = m_head.Replace(output, $"{m_robotsPlacement}\n</head>");
            }

            output = _OutputCompressor.Compress(output);

            ms = new MemoryStream(output.Length);

            byte[] buffer = encoding.GetBytes(output);
            ms.Write(buffer, 0, buffer.Length);

            return ms;
        }

        /// <summary>
        /// [CB]Checks if the current server environment version is still greater than the (multi node) envinroment. If not the server cache is flushed 
        /// </summary>
        private void VerifyCacheVersion()
        {
            var environmentversion = EnvironmentVersion.Select();
            if (environmentversion.ServerEnvironmentVersion.Ticks < environmentversion.Updated.GetValueOrDefault().Ticks)
                EnvironmentVersionLogic.Flush(false);
        }

        internal Sushi.Mediakiwi.Data.Page m_CurrentPage;
        internal Sushi.Mediakiwi.Data.Page CurrentPage
        {
            get { return m_CurrentPage; }
        }
        void VerifyEnvironment()
        {
            
            // CB First check if the enviroment updated is still equal to the server. This could be changed by another node or agent
            this.VerifyCacheVersion();
            // CB Moved from higher level to here; Only do this when the request is a wim.page
            if (m_Application.Context.Request.QueryString["flush"] == "me")
            {
                var environment = EnvironmentVersion.Select();
                EnvironmentVersionLogic.Flush(true);
                m_Application.Context.Response.Redirect(Wim.Utility.GetSafeUrl(m_Application.Context.Request).Replace("?flush=me", string.Empty).Replace("&flush=me", string.Empty), true);
            }
        }
        #endregion

        #region DownloadSiteDocument
        //private void DownloadSiteDocument(HttpContext content, Sushi.Mediakiwi.Data.Site site, int documentId)
        //{
        //    Sushi.Mediakiwi.Data.Environment environment = Sushi.Mediakiwi.Data.Environment.SelectOne();
        //    if (environment == null || environment.Repository == null)
        //        return;

        //    Sushi.Mediakiwi.Data.Document document = Sushi.Mediakiwi.Data.Document.SelectOne(documentId);
        //    if (document == null)
        //        return;

        //    string downloadPath = string.Format("{0}document/{1}.{2}", environment.Repository, document.Id, document.Extention);

        //    WebRequest req = WebRequest.Create(@downloadPath);
        //    WebResponse response = null;
        //    try
        //    {
        //        response = req.GetResponse();
        //        if (response.ContentLength > 0)
        //        {
        //            string path = string.Format("../{0}{1}.{2}", site.DocumentFolder, document.Id, document.Extention);
        //            string file = content.Server.MapPath(path);

        //            if (File.Exists(@file))
        //            {
        //                content.Response.Write("File_Exists: " + path);
        //                return;
        //            }
        //            using (WebClient wc = new WebClient())
        //            {
        //                wc.DownloadFile(downloadPath, @file);
        //                content.Response.Write("File_Downloaded: " + path);
        //                return;
        //            }
        //        }
        //        else
        //            content.Response.Write("File_NotFound: " + downloadPath);
        //    }
        //    catch (Exception ex)
        //    {
        //        System.Exception except = ex;

        //        string exceptionMessage = "<b>Exception occured when downloading document</b><br/>file: " + downloadPath;
        //        while (except != null)
        //        {
        //            exceptionMessage += string.Format("<br/>- {0}", except.Message);
        //            except = except.InnerException;
        //        }
        //        Sushi.Mediakiwi.Data.Notification.InsertOne(Sushi.Mediakiwi.Data.Notification.Tags.InternalWimError, exceptionMessage);
        //        content.Response.Write("ERROR");
        //    }
        //    finally
        //    {
        //        response.Close();
        //    }
        //}
        #endregion

        #region DownloadSiteImage
        //private void DownloadSiteImage(HttpContext content, Sushi.Mediakiwi.Data.Site site, int imageId)
        //{
        //    Sushi.Mediakiwi.Data.Environment environment = Sushi.Mediakiwi.Data.Environment.SelectOne();
        //    if (environment == null || environment.Repository == null)
        //        return;

        //    Sushi.Mediakiwi.Data.Image image = Sushi.Mediakiwi.Data.Image.SelectOne(imageId);
        //    if (image == null)
        //        return;

        //    string downloadPath = string.Format("{0}image/{1}.{2}", environment.Repository, image.Id, image.Extention);

        //    WebRequest req = WebRequest.Create(@downloadPath);
        //    req.Timeout = 5000;
            
        //    WebResponse response = null;
        //    try
        //    {
        //        response = req.GetResponse();
        //        if (response.ContentLength > 0)
        //        {
        //            string path = string.Format("../{0}{1}.{2}", site.ImageFolder, image.Id, image.Extention);
        //            string file = content.Server.MapPath(path);
        //            if (File.Exists(file))
        //            {
        //                content.Response.Write("File_Exists: " + path);
        //                return;
        //            }
                    
        //            using (WebClient wc = new WebClient())
        //            {
        //                wc.DownloadFile(downloadPath, @file);
        //                content.Response.Write("File_Downloaded: " + path);
        //                return;
        //            }
        //        }
        //        else
        //            content.Response.Write("File_NotFound: " + downloadPath);
        //    }
        //    catch (Exception ex)
        //    {
        //        System.Exception except = ex;

        //        string exceptionMessage = "<b>Exception occured when downloading image</b><br/>file: " + downloadPath;
        //        while (except != null)
        //        {
        //            exceptionMessage += string.Format("<br/>- {0}", except.Message);
        //            except = except.InnerException;
        //        }
        //        Sushi.Mediakiwi.Data.Notification.InsertOne(Sushi.Mediakiwi.Data.Notification.Tags.InternalWimError, exceptionMessage);
        //        content.Response.Write("ERROR");
        //    }
        //} 
        #endregion

        /// <summary>
        /// Gets the default.
        /// </summary>
        /// <param name="matchUrlSite">The match URL site.</param>
        /// <returns></returns>
        Sushi.Mediakiwi.Data.Page GetDefault(Sushi.Mediakiwi.Data.Site matchUrlSite)
        {
            if (matchUrlSite == null)
            {
                if (Sushi.Mediakiwi.Data.Environment.Current.DefaultSiteID.HasValue)
                {
                    Sushi.Mediakiwi.Data.Site site = Sushi.Mediakiwi.Data.Site.SelectOne(Sushi.Mediakiwi.Data.Environment.Current.DefaultSiteID.Value);
                    if (site.HomepageID.HasValue)
                    {
                        return Sushi.Mediakiwi.Data.Page.SelectOne(site.HomepageID.Value);
                    }
                }
            }
            else
            {
                if (matchUrlSite.HomepageID.HasValue)
                    return Sushi.Mediakiwi.Data.Page.SelectOne(matchUrlSite.HomepageID.Value);
            }
            return null;
        }

        #region GetPage
        private Sushi.Mediakiwi.Data.Page GetPage(HttpApplication m_Application, System.Uri url, out Sushi.Mediakiwi.Data.IPageMapping map)
        {
            Sushi.Mediakiwi.Data.IApplicationUser appUser = null;
            map = null;

            //m_Application.Response.Write("1# " + url.ToString() + "\n");
            Sushi.Mediakiwi.Data.Page page = null;
            string url2 = Wim.Utility.RemApplicationPath(url.AbsolutePath).ToLower();

            bool isDefaultHome = false;
            //  Default URL
            if (string.IsNullOrEmpty(url2) || url2 == "/default.aspx" || url2 == "default.aspx" || url2 == "/")
            {
                isDefaultHome = true;
            }

            bool isLocal = Wim.CommonConfiguration.IS_LOCAL_DEVELOPMENT;// m_Application.Request.IsLocal;
            bool isPreview = m_Application.Request.QueryString["preview"] == "1";
            
            if (page == null)
            {
                string[] split = url2.Split('/');

                string pageName = m_Application.Server.UrlDecode(
                    split[split.Length - 1]
                        .Replace(string.Concat(".", Sushi.Mediakiwi.Data.Environment.Current.GetRegistryValue("PAGE_WILDCARD_EXTENTION", "aspx")), string.Empty)
                    );

                if (pageName.Contains(".aspx"))
                    pageName = pageName.Replace(".aspx", string.Empty);

                string folderName = m_Application.Server.UrlDecode(
                    url2.Substring(0, url2.LastIndexOf('/') + 1)
                    );

                Sushi.Mediakiwi.Data.Site siteUrlMatch = null;
                
                if (!isLocal)
                {
                    #region Domain validation #1
                    //  Domain validation #1 BEGIN
                    // Line below edited by MarkR on 05-12-2012 on behalf of a non-working preview button.
                    string domainPrefix = Sushi.Mediakiwi.Data.Site.GetDomainPrefix(url, out siteUrlMatch);// isPreview ? null : Sushi.Mediakiwi.Data.Site.GetDomainPrefix(url, out siteUrlMatch);
                    if (isDefaultHome)
                    {
                        page = GetDefault(siteUrlMatch);
                    }

                    if (!string.IsNullOrEmpty(domainPrefix))
                    {
                        //  Do a deepfolder check
                        if (!isDefaultHome)
                        {
                            //  Only do a validation when the URL contains a channel default folder (so, not '/');
                            if (folderName != "/")
                            {
                                string tmpFolder = string.Concat(domainPrefix, folderName);
                                page = Sushi.Mediakiwi.Data.Page.SelectOne(tmpFolder, pageName, true);
                                if (page != null)
                                {
                                    //  When the default channel folder is not of that of the domein, return the deeper page
                                    if (page.SiteID != siteUrlMatch.ID)
                                        return page;
                                }
                            }
                        }

                        bool startsWith = folderName.StartsWith(string.Concat(domainPrefix, "/"), StringComparison.InvariantCulture);
                        if (startsWith && Wim.CommonConfiguration.REDIRECT_CHANNEL_PATH)
                        {
                            //  Example: www.url.fr/fr/page.html, should redirect to www.url.fr/page.html
                            string adjustedUrl = string.Concat(folderName.Remove(0, domainPrefix.Length)
                                , split[split.Length - 1]
                                , m_Application.Request.QueryString.Count == 0 ? "" :
                                    string.Concat("?", m_Application.Request.QueryString.ToString())
                                );

                            if (Wim.CommonConfiguration.WIM_DEBUG)
                                Sushi.Mediakiwi.Data.Notification.InsertOne("Domain validation (1)", Sushi.Mediakiwi.Data.NotificationType.Information, adjustedUrl);

                            m_Application.Response.Status = "301 Moved Permanently";
                            m_Application.Response.AddHeader("Location", adjustedUrl);
                            //m_Application.Response.Redirect(adjustedUrl);
                        }

                        //  Example: www.url.fr/page.html, should map to www.url.fr/fr/page.html for lookup
                        if (string.IsNullOrEmpty(folderName) || !startsWith)
                            folderName = string.Concat(domainPrefix, folderName);
                    }
                    //  Domain validation #1 END
                    #endregion
                }
                else
                {
                    if (isDefaultHome)
                        page = GetDefault(siteUrlMatch);
                }

                m_Application.Context.Trace.Write("Wim.Rewrite", string.Format("Search for page: {0} in folder {1}", pageName, folderName));
             

                bool returnOnlyPublishedPage = true;
                if (m_Application.Request.QueryString["preview"] == "1")
                {
                    appUser = Sushi.Mediakiwi.Data.ApplicationUserLogic.Select();
                    if (!appUser.IsNewInstance && appUser.isActive && appUser.IsLoggedIn())
                    {
                        HttpContext.Current.Items["Wim.Preview"] = "1";
                        returnOnlyPublishedPage = false;
                    }
                }
                if (m_Application.Context.Request["loadcomponentinstance"] == "1")
                {
                    returnOnlyPublishedPage = false;
                }


                if (page == null)
                    page = Sushi.Mediakiwi.Data.Page.SelectOne(folderName, pageName, returnOnlyPublishedPage);

                //  Page name only trial
                if (page == null)
                {
                    if (isDefaultHome)
                        page = GetDefault(null);
                    // [CB: 10-10-2016] This option has to be able to be disable. Client youfone doesn't want redirects on 404
                    var No404SelectOnBasedOnName  = Sushi.Mediakiwi.Data.Registry.SelectOneByName("No404SelectOnBasedOnName");
                    if (page == null && (No404SelectOnBasedOnName == null || No404SelectOnBasedOnName.Value == null || (No404SelectOnBasedOnName?.Value != "1" && No404SelectOnBasedOnName?.Value.ToLower() != "true" )) )
                        page = Sushi.Mediakiwi.Data.Page.SelectOneBasedOnName(pageName, returnOnlyPublishedPage);

                    if (page == null || page.IsNewInstance)
                        return null;

                    m_Application.Response.Status = "301 Moved Permanently";
                    m_Application.Response.AddHeader("Location", page.HRef);
                }
            }

            if (page == null)
                return null;

            //  Domain validation #2 BEGIN
            if (!isPreview && !isLocal && Wim.CommonConfiguration.REDIRECT_CHANNEL_PATH)
            {
                if (!string.IsNullOrEmpty(page.Site.Domain) && !page.Site.IsDomain(m_Application.Request.Url))
                {
                    string redirect = string.Concat(page.HRef
                        , m_Application.Request.QueryString.Count == 0 ? "" :
                                    string.Concat("?", m_Application.Request.QueryString.ToString()));

                    if (Wim.CommonConfiguration.WIM_DEBUG)
                        Sushi.Mediakiwi.Data.Notification.InsertOne("Domain validation (2)", Sushi.Mediakiwi.Data.NotificationType.Information, redirect);

                    m_Application.Response.Redirect(redirect);
                }
            }
            //  Domain validation #2 




            if (page.Publication != DateTime.MinValue)
            {
                if (DateTime.Now.Ticks < page.Publication.Ticks)
                    return null;
            }
            if (page.Expiration != DateTime.MinValue)
            {
                if (DateTime.Now.Ticks > page.Expiration.Ticks)
                    return null;
            }

            if (page != null && appUser != null)
            {
                Sushi.Mediakiwi.Framework2.Functions.AuditTrail.Insert(appUser, page, Framework2.Functions.Auditing.ActionType.Preview, null);
            }

            return page;
        } 
        #endregion
        #endregion
	}
}