using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;

namespace Sushi.Mediakiwi.Framework
{
    /// <summary>
    /// 
    /// </summary>
    public class WimOutput
    {
        public WimOutput()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WimOutput"/> class.
        /// </summary>
        /// <param name="output">The output.</param>
        public WimOutput(ref StringBuilder output)
        {
        }

        public void Debug(object message)
        {
            //if (message == null) return;

            //System.Web.HttpContext context = System.Web.HttpContext.Current;

            //string[] debugger = context.Items["Wim.Debug"] as string[];
            //if (debugger == null)
            //{
            //    debugger = new string[1] { message.ToString() };
            //    context.Items.Add("Wim.Debug", debugger);
            //}
            //else
            //{
            //    string[] new_debugger = new string[debugger.Length + 1];
            //    debugger.CopyTo(new_debugger, 0);
            //    new_debugger[debugger.Length] = message.ToString();
            //    context.Items["Wim.Debug"] = new_debugger;
            //}
        }

        /// <summary>
        /// Adds the scripting.
        /// </summary>
        /// <param name="visitor">The visitor.</param>
        /// <param name="pageID">The page ID.</param>
        /// <param name="output">The output.</param>
        /// <param name="isPageCached">if set to <c>true</c> [is page cached].</param>
        public void AddScripting(Data.Identity.IVisitor visitor, int pageID, ref string output, bool isPageCached)
        {
            System.Web.HttpContext context = System.Web.HttpContext.Current;

            Regex bodyClose = new Regex(@"</body>", RegexOptions.IgnoreCase);

            StringBuilder addToBodyClose = new StringBuilder();

            if (Sushi.Mediakiwi.Data.Environment.Current["ENABLE_MKLYTICS", true, "1", "Register statistics via mklytics"] == "1")
            {
                long startPageRendering = (long)context.Items["wim.executionTime"];
                var renderTime = decimal.Floor(Convert.ToDecimal(new TimeSpan(DateTime.Now.Ticks - startPageRendering).TotalMilliseconds));
                addToBodyClose.AppendFormat("<script type=\"text/javascript\">document.write(unescape(\"%3Cscript src='{1}?m={2}&u={3}&s={4}&t={5}&c=\"+(document.cookie.length>-1?1:0)+\"&w=\"+document.URL+\"&r=\"+document.referrer+\"' type='text/javascript'%3E%3C/script%3E\"));</script>"
                    , null
                    , Wim.Utility.AddApplicationPath("mklytics")
                    , pageID
                    , isPageCached ? "2" : visitor.IsNewVisitor ? "1" : "0"
                    , isPageCached ? "2" : visitor.IsNewSession ? "1" : "0"
                    , renderTime
                    );
            }

            string domainCookieHost = Sushi.Mediakiwi.Data.Environment.Current["MAIN_DOMAIN_COOKIE_HOST"];

            if (!string.IsNullOrEmpty(domainCookieHost) && context.Request.Url.Host != domainCookieHost)
                addToBodyClose.AppendFormat(@"<!-- DC --><script type=""text/javascript"">document.write(unescape(""%3Cscript src='{0}?dc={1}' type='text/javascript'%3E%3C/script%3E""));</script><!-- END DC -->", string.Format("http://{0}/repository/tcl.aspx", domainCookieHost), visitor.GUID);

            string subdomainsCookieHost = Sushi.Mediakiwi.Data.Environment.Current["SUB_DOMAINS_COOKIE_HOST"];
            if (!String.IsNullOrEmpty(subdomainsCookieHost))
            {
                foreach (string host in subdomainsCookieHost.Split(','))
                {
                    if (!string.IsNullOrEmpty(host) && context.Request.Url.Host != host)
                        addToBodyClose.AppendFormat(@"<!-- DC --><script type=""text/javascript"">document.write(unescape(""%3Cscript src='{0}?dc={1}' type='text/javascript'%3E%3C/script%3E""));</script><!-- END DC -->", string.Format("http://{0}/repository/tcl.aspx", host), visitor.GUID);
                }
            }

            if (addToBodyClose != null)
                output = bodyClose.Replace(output, string.Concat(addToBodyClose, "</body>"));
        }
    }
}
