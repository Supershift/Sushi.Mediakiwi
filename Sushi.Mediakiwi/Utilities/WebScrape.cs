using System;
using System.Security;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Net;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace Wim.Utilities
{
    /// <summary>
    /// 'Screenscrape' a dynamic webresponse based on a static webtemplate for use in sending HTML mails.
    /// </summary>
    /// <remarks>
    /// Author: Marc Molenwijk
    /// Change history:
    /// - Created: 01-FEB-2005
    /// </remarks>
    public class WebScrape : IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        public enum Method
        {
            /// <summary>
            /// 
            /// </summary>
            GET,
            /// <summary>
            /// 
            /// </summary>
            POST
        }

        /// <summary>
        /// 
        /// </summary>
        public enum ContentType
        {
            /// <summary>
            /// application/x-www-form-urlencoded
            /// </summary>
            application,
            /// <summary>
            /// multipart/form-data
            /// </summary>
            multipart
        }

        #region Get the webscraped URL response ( overloaded )
        /// <summary>
        /// Gets the URL response.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns></returns>
        public static string GetUrlResponse(string url)
        {
            return WebScrape.GetUrlResponse(url, 60, null);
        }

        /// <summary>
        /// Gets the URL response.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="credentials">The credentials.</param>
        /// <returns></returns>
        public static string GetUrlResponse(string url, ICredentials credentials)
        {
            return WebScrape.GetUrlResponse(url, 60, null, null, credentials);
        }//

        /// <summary>
        /// Gets the URL response.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="replacementItem">The replacement item.</param>
        /// <returns></returns>
        public static string GetUrlResponse(string url, Hashtable replacementItem)
        {
            int timeOut = 60; // 1 minutes
            return WebScrape.GetUrlResponse(url, timeOut, replacementItem);
        }

        /// <summary>
        /// Gets the URL response.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="timeOut">The time out in seconds.</param>
        /// <param name="replacementItem">The replacement item.</param>
        /// <returns></returns>
        public static string GetUrlResponse(string url, int timeOut, Hashtable replacementItem)
        {
            return WebScrape.GetUrlResponse(url, timeOut, replacementItem, null);
        }

        /// <summary>
        /// Gets the URL response.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="timeOut">The time out.</param>
        /// <param name="replacementItem">The replacement item.</param>
        /// <param name="postData">The post data (name=John&value=Doe).</param>
        /// <returns></returns>
        public static string GetUrlResponse(string url, int timeOut, Hashtable replacementItem, Dictionary<string, string> postData)
        {
            return WebScrape.GetUrlResponse(url, timeOut, replacementItem, postData, null);
        }

        /// <summary>
        /// Gets the URL response.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="timeOut">The time out in seconds.</param>
        /// <param name="replacementItem">The replacement item.</param>
        /// <param name="postData">The post data (name=John&value=Doe).</param>
        /// <param name="credentials">The credentials.</param>
        /// <returns></returns>
        public static string GetUrlResponse(string url, int timeOut, Hashtable replacementItem, Dictionary<string, string> postData, ICredentials credentials)
        {
            return WebScrape.GetUrlResponse(url, timeOut, replacementItem, postData, credentials, null);
        }

        /// <summary>
        /// Gets the URL response.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="timeOut">The time out in seconds.</param>
        /// <param name="replacementItem">The replacement item.</param>
        /// <param name="postData">The post data (name=John&value=Doe).</param>
        /// <param name="credentials">The credentials.</param>
        /// <param name="headers">The headers.</param>
        /// <returns></returns>
        public static string GetUrlResponse(string url, int timeOut, Hashtable replacementItem, Dictionary<string, string> postData, ICredentials credentials, WebHeaderCollection headers)
        {
            string item;
            using (WebScrape scrape = new WebScrape())
            {
                scrape._timeOut = 1000 * timeOut;
                item = scrape.GetUrlOutputResponse(url, postData, credentials, headers);
                item = scrape.GetKeyValueReplacement(item, replacementItem);
            }
            return item;
        }
        #endregion Get the webscraped URL response ( overloaded )

        #region Web response screen scrape class

        #region Dispose
        private bool _disposed = false;

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    //  Dispose unmanaged objects
                }
            }
            this._disposed = true;
        }

        /// <summary>
        /// 
        /// </summary>
        ~WebScrape()
        {
            Dispose(false);
        }
        #endregion Dispose

        private int _timeOut;

        /// <summary>
        /// 
        /// </summary>
        public WebScrape()
        {
        }

        #region Get key/value replacement
        /// <summary>
        /// Replace dynamic values within a text defined by a key/value hashtable 
        /// </summary>
        private string GetKeyValueReplacement(string maintext, Hashtable replacementItem)
        {
            if (replacementItem == null)
                return maintext;

            //	Get key/value enumerator
            IDictionaryEnumerator replacement = replacementItem.GetEnumerator();
            //	Move through enumeration
            while (replacement.MoveNext())
            {
                //	Replace all values determined within the hashtable

                if (replacement == null || replacement.Key == null)
                    continue;

                if (replacement.Value == null)
                    maintext = maintext.Replace(replacement.Key.ToString(), string.Empty);
                else
                    maintext = maintext.Replace(replacement.Key.ToString(), replacement.Value.ToString());
            }

            return maintext;
        }
        #endregion Get key/value replacement

        static string m_BaseUrl;
        /// <summary>
        /// Gets the host.
        /// </summary>
        /// <value>The host.</value>
        public static string Host
        {
            get
            {
                if (m_BaseUrl == null)
                {
    
                    System.Web.HttpContext context = System.Web.HttpContext.Current;

                    bool isLocal = false;
                    m_BaseUrl = context.Request.Url.AbsoluteUri.Split('/')[2];
                    
                    if (context.Request.Url.AbsoluteUri.ToLower().Contains("http://"))
                        m_BaseUrl = string.Format("http://{0}/", m_BaseUrl);
                    else
                        m_BaseUrl = string.Format("https://{0}/", m_BaseUrl);

                    if (context != null && !isLocal && Sushi.Mediakiwi.Data.Environment.Current["USE_LOCAL_ADDR_FOR_WEBSCRAPE"] == "1")
                    {
                        if (m_BaseUrl.ToLower().Contains("http://"))
                            m_BaseUrl = string.Format("http://{0}/", context.Request.ServerVariables["LOCAL_ADDR"]);
                        else
                            m_BaseUrl = string.Format("https://{0}/", context.Request.ServerVariables["LOCAL_ADDR"]);
                    }
                    if (context != null && !isLocal && !string.IsNullOrEmpty(Sushi.Mediakiwi.Data.Environment.Current["USE_URLPREFIX_FOR_WEBSCRAPE"]))
                    {
                        m_BaseUrl = string.Format("{0}/", Sushi.Mediakiwi.Data.Environment.Current["USE_URLPREFIX_FOR_WEBSCRAPE"]);
                    }
                }
                return m_BaseUrl;
            }
        }

        #region Get URL output response
        /// <summary>
        /// Get an URL output response
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="postData">The post data (the method should be POST).</param>
        /// <param name="credentials">The credentials.</param>
        /// <param name="headers">The headers.</param>
        /// <returns></returns>
        private string GetUrlOutputResponse(string url, Dictionary<string, string> postData, ICredentials credentials, WebHeaderCollection headers)
        {
            bool hasConnection = !string.IsNullOrEmpty(Sushi.Mediakiwi.Data.Common.DatabaseConnectionString);
            System.Web.HttpContext context = System.Web.HttpContext.Current;

            if (hasConnection)
            {
                //context.Response.Write(string.Format("<!-- {0} -->\n", url));

                bool isLocal = context == null ? false : context.Request.IsLocal;
                //  Use the local address of this server when performing a HttpWebRequest using the Wim.Utilities.WebScrape class. 
                //  This is required when a server can not resolve a local assigned (IP/web)address. 
                ////  The default state is [False].
                if (context != null && !isLocal && Sushi.Mediakiwi.Data.Environment.Current["USE_LOCAL_ADDR_FOR_WEBSCRAPE"] == "1")
                {
                    string hostCurrent = context.Request.Url.AbsoluteUri.Split('/')[2];
                    string hostCall = url.Split('/')[2];

                    if (hostCurrent.Equals(hostCall, StringComparison.OrdinalIgnoreCase))
                    {
                        string buildUrl = url.ToLower().Replace("http://", string.Empty).Replace("https://", string.Empty);
                        int i = buildUrl.IndexOf('/');

                        if (url.ToLower().Contains("http://"))
                        {
                            url = string.Format("http://{0}/{1}"
                               , context.Request.ServerVariables["LOCAL_ADDR"]
                               , buildUrl.Substring(i, buildUrl.Length - i));
                        }
                        else
                        {
                            url = string.Format("https://{0}/{1}"
                                , context.Request.ServerVariables["LOCAL_ADDR"]
                                , buildUrl.Substring(i, buildUrl.Length - i));
                        }
                    }
                }
                if (context != null && !isLocal && !string.IsNullOrEmpty(Sushi.Mediakiwi.Data.Environment.Current["USE_URLPREFIX_FOR_WEBSCRAPE"]))
                {
                    //context.Response.Write(string.Format("<!-- {0} -->\n", "USE_URLPREFIX_FOR_WEBSCRAPE"));

                    string buildUrl = url.ToLower().Replace("http://", string.Empty).Replace("https://", string.Empty);
                    int i = buildUrl.IndexOf('/');

                    url = string.Format("{0}/{1}"
                        , Sushi.Mediakiwi.Data.Environment.Current["USE_URLPREFIX_FOR_WEBSCRAPE"]
                        , buildUrl.Substring(i, buildUrl.Length - i));
                }
            }
            string output;
            //	Create the URL request
            HttpWebRequest webRequest = (HttpWebRequest)HttpWebRequest.Create(url);
            webRequest.ContentType = "application/x-www-form-urlencoded";

            if (postData == null)
                webRequest.Method = "GET";
            else
                webRequest.Method = "POST";

            if (context == null)
                webRequest.UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1; .NET CLR 2.0.50727)";
            else
                webRequest.UserAgent = context.Request.UserAgent;

            webRequest.Accept = "HTTP_ACCEPT:*/* HTTP_ACCEPT_ENCODING:gzip, deflate HTTP_ACCEPT_LANGUAGE:nl-nl,en-us;q=0.5";

            webRequest.CookieContainer = new CookieContainer();

            if (headers != null)
            {
                webRequest.Headers = headers;
            }

            if (credentials != null)
            {
                webRequest.Credentials = credentials;
            }
            else
                webRequest.UseDefaultCredentials = true;


            StringBuilder build2 = new StringBuilder(); 
            System.Web.HttpCookie cookie;

            if (context != null)
            {
                Encoding utf = Encoding.GetEncoding("utf-8"); 

                for (int index = 0; index < context.Request.Cookies.Count; index++)
                {                   
                    cookie = context.Request.Cookies[index];
                    if (cookie.Name == "__utma" || cookie.Name == "__utmb" || cookie.Name == "__utmc" || cookie.Name == "__utmv" || cookie.Name == "__utmz")
                        continue;

                    webRequest.CookieContainer.Add(
                        new Cookie(
                            cookie.Name.Replace(" ", "_"), 
                            //System.Web.HttpUtility.UrlEncode(cookie.Value, utf),
                            cookie.Value, 
                            cookie.Path, 
                            context.Request.ServerVariables["HTTP_HOST"]
                            )
                        );

                    string xx = cookie.Value;
                    string yy = System.Web.HttpUtility.UrlEncode(cookie.Value, utf);
                    string zzz = System.Web.HttpUtility.UrlDecode(yy, utf);

                    build2.AppendFormat("{0}{1}|{2}", 
                        build2.Length == 0 ? "" : "&", //0
                        cookie.Name, //1
                        cookie.Value //2
                    );
                    //build2.AppendFormat("{0}{1}|{2}",
                    //    build2.Length == 0 ? "" : "&", //0
                    //    cookie.Name, //1
                    //    System.Web.HttpUtility.UrlEncode(cookie.Value, utf) //2
                    //);
                }
                //context.Response.Write(string.Format("<!-- WIM-Transport-Cookie {0} -->\n", build2.ToString()));
                //context.Response.Write(string.Format("<!-- WIM-Transport-IP {0} -->\n", context.Request.UserHostAddress));

                webRequest.Headers.Add("WIM-Transport-Visitor", Sushi.Mediakiwi.Data.Identity.Visitor.Select().GUID.ToString());
                webRequest.Headers.Add("WIM-Transport-Cookie", build2.ToString());
                webRequest.Headers.Add("WIM-Transport-IP", context.Request.UserHostAddress);
            }

            //	Set the request timeout
            webRequest.Timeout = this._timeOut;
            //	Get the reponse of the webrequest

            if (postData != null)
            {
                StringBuilder build = new StringBuilder();

                foreach (KeyValuePair<string, string> postItem in postData)
                {
                    if (context == null)
                        build.Append(string.Concat(build.Length == 0 ? null : "&", postItem.Key, "=", postItem.Value));
                    else
                        build.Append(string.Concat(build.Length == 0 ? null : "&", postItem.Key, "=", context.Server.UrlEncode(postItem.Value)));
                }

                byte[] byteArray = Encoding.UTF8.GetBytes(build.ToString());

                webRequest.ContentLength = byteArray.Length;

                System.Net.ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(ValidateServerCertificate);

                Stream dataStream = webRequest.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
            }

            try
            {
                HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse();
                //	Fetch the output response 
                output = new StreamReader(webResponse.GetResponseStream(), Encoding.UTF8).ReadToEnd();
                webResponse.Close();

                return output;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Concat("The following HttpWebResponse failed: ", url), ex);
            }




        }

        /// <summary>
        /// 
        /// </summary>
        public delegate bool RemoteCertificateValidationCallback(Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors);

        /// <summary>
        /// Validates the server certificate.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="certificate">The certificate.</param>
        /// <param name="chain">The chain.</param>
        /// <param name="sslPolicyErrors">The SSL policy errors.</param>
        /// <returns></returns>
        public static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None)
                return true;

            //Console.WriteLine("Certificate error: {0}", sslPolicyErrors);
            // Do not allow this client to communicate with unauthenticated servers.
            return true;
        }


        #endregion Get URL output response
        #endregion Web response screen scrape class
    }
}
