using System;
using System.Linq;
using System.Globalization;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Sushi.Mediakiwi.Data;
using Sushi.Mediakiwi.UI;
using System.Net;
using System.Diagnostics;
using Sushi.Mediakiwi.Framework;
using System.Threading.Tasks;
using System.Threading;

namespace Sushi.Mediakiwi
{
    /// <summary>
    /// 
    /// </summary>
	public class Utils
	{
        private static readonly TaskFactory _taskFactory = new
            TaskFactory(CancellationToken.None,
                        TaskCreationOptions.None,
                        TaskContinuationOptions.None,
                        TaskScheduler.Default);

        /// <summary>
        /// Executes an async Task method which has a void return value synchronously
        /// USAGE: AsyncUtil.RunSync(() => AsyncMethod());
        /// </summary>
        /// <param name="task">Task method to execute</param>
        public static void RunSync(Func<Task> task)
            => _taskFactory
                .StartNew(task)
                .Unwrap()
                .GetAwaiter()
                .GetResult();

        /// <summary>
        /// Executes an async Task<T> method which has a T return type synchronously
        /// USAGE: T result = AsyncUtil.RunSync(() => AsyncMethod<T>());
        /// </summary>
        /// <typeparam name="TResult">Return Type</typeparam>
        /// <param name="task">Task<T> method to execute</param>
        /// <returns></returns>
        public static TResult RunSync<TResult>(Func<Task<TResult>> task)
            => _taskFactory
                .StartNew(task)
                .Unwrap()
                .GetAwaiter()
                .GetResult();

        public static string ToUrl(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;
            return value.ToLower().Replace(" ", "-");
        }

        public static string FromUrl(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;
            return value.ToLower().Replace("-", " ");
        }


        /// <summary>
        /// Converts a datetime to Epoch. Note that if the dateTime is UTC that is has that kind set.
        /// </summary>
        /// <param name="dateTime">The date time.</param>
        /// <returns></returns>
        public static long ConvertEpoch(DateTime dateTime)
        {
            DateTime UTCBaseTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            long ticks = (dateTime.ToUniversalTime().Ticks - UTCBaseTime.Ticks) / TimeSpan.TicksPerMillisecond;
            return ticks;
        }

        /// <summary>
        /// Converts the epoch to UTC.
        /// </summary>
        /// <param name="unixTime">The unix time.</param>
        /// <returns></returns>
        public static DateTime ConvertEpoch(long unixTime)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddSeconds(unixTime);
        }

        static string _Version;
        internal static string Version
        {
            get
            {
                if (_Version == null)
                {
                    var split = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion.Split('.');
                    _Version = string.Format("{0}.{1}", split[0], split[1]);
                }
                return _Version;
            }
        }

        /// <summary>
        /// Gets the asynchronous query properties.
        /// </summary>
        /// <returns></returns>
        internal static ASyncQuery GetAsyncQuery(Beta.GeneratedCms.Console console)
        {
            if (console.Context == null)
                return null;

            bool isAsyncCall = console.Form(UI.Constants.JSON_PARAM) == "1";
            if (!isAsyncCall)
                return null;

            var id = console.Form("async_id");
            var recall_id = console.Form("async_rid");
            ASyncQuery q = new ASyncQuery(console.Context);
            q.Property = console.Form("async");
            q.SearchQuery = string.IsNullOrEmpty(id) ? (string.IsNullOrEmpty(recall_id) ? console.Form("async_query") : recall_id) : id;
            q.SearchType = string.IsNullOrEmpty(id) ? (string.IsNullOrEmpty(recall_id) ? ASyncQueryType.FindByText : ASyncQueryType.OnSelectCallBack) : ASyncQueryType.SelectOneByID;
            return q;
        }


        public static String ShortUrlEncoding(long input)
        {
            return Base36.Encode(input);
        }

        public static long ShortUrlDecoding(string input)
        {
            return Base36.Decode(input);
        }

        static class Base36
        {
            //private const string CharList = "0123456789abcdefghijkLmnopqrstuvwxyz";
            private const string CharList = "A1B2C3D4E5F6G7H8K9LaMbNcPdQeRfSgThVjWkXmYnZopqrstuvwxyz";

            /// <summary>
            /// Encode the given number into a Base36 string
            /// </summary>
            /// <param name="input"></param>
            /// <returns></returns>
            public static String Encode(long input)
            {
                if (input < 0) throw new ArgumentOutOfRangeException("input", input, "input cannot be negative");

                char[] clistarr = CharList.ToCharArray();
                var result = new Stack<char>();
                while (input != 0)
                {
                    result.Push(clistarr[input % 36]);
                    input /= 36;
                }
                return new string(result.ToArray());
            }

            /// <summary>
            /// Decode the Base36 Encoded string into a number
            /// </summary>
            /// <param name="input"></param>
            /// <returns></returns>
            public static Int64 Decode(string input)
            {
                var reversed = input.Reverse();
                long result = 0;
                int pos = 0;
                foreach (char c in reversed)
                {
                    result += CharList.IndexOf(c) * (long)Math.Pow(36, pos);
                    pos++;
                }
                return result;
            }
        }

        /// <summary>
        /// Gets the custom query string.
        /// </summary>
        /// <param name="keyvalues">The keyvalues.</param>
        /// <returns></returns>
        //public static string GetCustomQueryString(params KeyValue[] keyvalues)
        //{
        //    System.Web.HttpContext context = System.Web.HttpContext.Current;
        //    System.Collections.Specialized.NameValueCollection nv = context.Request.QueryString;
        //    return GetCustomQueryString(nv, keyvalues);
        //}

        /// <summary>
        /// Gets the custom query string.
        /// </summary>
        /// <param name="queryString">The query string.</param>
        /// <param name="keyvalues">The keyvalues.</param>
        /// <returns></returns>
        //public static string GetCustomQueryString(string queryString, params KeyValue[] keyvalues)
        //{
        //    System.Collections.Specialized.NameValueCollection nv = new System.Collections.Specialized.NameValueCollection();
        //    string[] pairs = queryString.Split('&');
        //    foreach (string pair in pairs)
        //    {
        //        string[] namevalue = pair.Split('=');
        //        nv.Add(namevalue[0], namevalue[1]);
        //    }
        //    return GetCustomQueryString(nv, keyvalues);
        //}

     
        /// <summary>
        /// Gets the custom query string.
        /// </summary>
        /// <param name="queryString">The query string.</param>
        /// <param name="keyvalues">The keyvalues.</param>
        /// <returns></returns>
        public static string GetCustomQueryString(System.Collections.Specialized.NameValueCollection queryString, params KeyValue[] keyvalues)
        {
            StringBuilder build = new StringBuilder();

            foreach (string key in queryString.AllKeys)
            {
                if (key == "channel" || key == null) continue;
                string keyvalue = queryString[key];

                if (keyvalues != null)
                {
                    var selection = (from item in keyvalues where item.Key.ToLower() == key.ToLower() select item).ToArray();
                    if (selection.Length == 1)
                    {
                        if (selection[0].RemoveKey)
                            continue;

                        build.AppendFormat("{0}{1}={2}"
                            , build.Length == 0 ? "?" : "&"
                            , selection[0].Key, selection[0].Value);
                        selection[0].Done = true;
                    }
                    else
                    {
                        build.AppendFormat("{0}{1}={2}"
                            , build.Length == 0 ? "?" : "&"
                            , key, keyvalue);
                    }
                }
                else
                {
                    build.AppendFormat("{0}{1}={2}"
                        , build.Length == 0 ? "?" : "&"
                        , key, keyvalue);
                }
            }


            if (keyvalues != null)
            {
                var remaining = (from item in keyvalues where !item.Done && !item.RemoveKey select item);
                foreach (KeyValue kv in remaining)
                {
                    build.AppendFormat("{0}{1}={2}"
                        , build.Length == 0 ? "?" : "&"
                        , kv.Key, kv.Value);
                }
            }


            return build.ToString();
        }

        /// <summary>
        /// Converts the text to HTML.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public static string ConvertTextToHTML(string text)
        {
            if (string.IsNullOrEmpty(text)) return text;

            if (
                !text.StartsWith("http://", StringComparison.InvariantCultureIgnoreCase) &&
                !text.StartsWith("www.", StringComparison.InvariantCultureIgnoreCase) &&
                !text.StartsWith("https://", StringComparison.InvariantCultureIgnoreCase) &&
                !text.StartsWith("ftp://", StringComparison.InvariantCultureIgnoreCase)
                )
            {
                text = ConvertFirstToUpper(text);
            }

            text = Utility.CleanLineFeed(text, false);

            Regex urlHttp = new Regex(@"[\s|\n](http://[^\s|<]*)", RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.Compiled);
            text = urlHttp.Replace(" " + text + " ", new MatchEvaluator(replaceA));

            Regex urlHttps = new Regex(@"[\s|\n](https://[^\s|<]*)", RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.Compiled);
            text = urlHttps.Replace(" " + text + " ", new MatchEvaluator(replaceA));

            Regex urlftp = new Regex(@"[\s|\n](ftp://[^\s|<]*)", RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.Compiled);
            text = urlftp.Replace(" " + text + " ", new MatchEvaluator(replaceA));

            Regex urlWWW = new Regex(@"[\s|\n](www[^\s|<]*)", RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.Compiled);
            text = urlWWW.Replace(" " + text + " ", new MatchEvaluator(replaceA));

            text = Utility.CleanLineFeed(text, true);

            text = ConvertEmailAddressInText(text);

            text.Trim();
            return text;
        }

        /// <summary>
        /// Converts the email address in text.
        /// </summary>
        /// <param name="p">The p.</param>
        /// <returns></returns>
        public static string ConvertEmailAddressInText(string p)
        {
            if (p.Contains("mailto:")) return p;
            Regex r = new Regex(@"\S*@\S*\.\S*", RegexOptions.Compiled);
            return r.Replace(p, @"<a href=""mailto:$0"">$0</a>");
        }

        /// <summary>
        /// Replaces the A.
        /// </summary>
        /// <param name="m">The m.</param>
        /// <returns></returns>
        static string replaceA(Match m)
        {
            string url = m.Groups[1].Value;
            string title = url;
            string text = url;

            if (text.Length > 50)
                text = text.Substring(0, 47) + "...";

            if (!url.Contains("http://") && !url.Contains("https://") && !url.Contains("ftp://"))
                url = String.Concat("http://", url);

            return String.Format(@"{3}<a href=""{0}"" title=""{2}"">{1}</a> ", url.ToLower(), text, title, m.Value.StartsWith("\n") ? "\n" : " ");
        }

        public static string ConvertFirstToUpper(string p)
        {
            if (String.IsNullOrEmpty(p))
                return String.Empty;
            if (p.Length == 1)
                return p.ToUpper();
            return p[0].ToString().ToUpper() + p.Substring(1);
        }

        /// <summary>
        /// Gets the current host (including port number if other then 80). f.e. http://www.wimserver.com
        /// </summary>
        /// <returns></returns>
        /// <value>The current host.</value>
        //public static string GetCurrentHost()
        //{
        //    return GetCurrentHost(false);
        //}

        //public static string GetSafeUrl(HttpRequest request)
        //{
        //    if (request.QueryString.Count == 0)
        //        return $"{request.Path}";
        //    return $"{request.Path}?{request.QueryString}";
        //}

        //public static string GetCurrentHost(bool applyApplicationPath, bool removeLastForslash = false)
        //{
        //    if (!string.IsNullOrEmpty(CommonConfiguration.LOCAL_REQUEST_URL))
        //        return CommonConfiguration.LOCAL_REQUEST_URL;

        //    string portInfo = null;

        //    if (HttpContext.Current == null)
        //        return null;
        //    //throw new Exception("No host detected as of missing HttpContext.Current");

        //    if (HttpContext.Current.Request.Url.Port != 80 && HttpContext.Current.Request.Url.Port != 443)
        //        portInfo = string.Concat(":", HttpContext.Current.Request.Url.Port);

        //    string extra = "/";
        //    if (applyApplicationPath)
        //        extra = HttpContext.Current.Request.ApplicationPath;

        //    var host = HttpContext.Current.Request.Url.Host;
        //    if (host.Equals("127.0.0.1"))
        //        host = "localhost";

        //    var uri = string.Concat(HttpContext.Current.Request.Url.Scheme, "://", host, portInfo, extra);
        //    if (removeLastForslash)
        //    {
        //        if (uri.EndsWith("/"))
        //            uri = uri.Substring(0, uri.Length - 1);
        //    }
        //    return uri;
        //}

        /// <summary>
        /// Copy get; set; properties with reflected Name and PropertyType.
        /// </summary>
        /// <param name="propertyContainerFrom">The property container from.</param>
        /// <param name="propertyContainerTo">The property container to.</param>
        public static void ReflectProperty(object propertyContainerFrom, object propertyContainerTo)
        {
            ReflectProperty(propertyContainerFrom, propertyContainerTo, false);
        }

        /// <summary>
        /// Copy get; set; properties with reflected Name and PropertyType.
        /// </summary>
        /// <param name="propertyContainerFrom">The property container from.</param>
        /// <param name="propertyContainerTo">The property container to.</param>
        /// <param name="reflectNull">if set to <c>true</c> [reflect null].</param>
        public static void ReflectProperty(object propertyContainerFrom, object propertyContainerTo, bool reflectNull)
        {
            if (propertyContainerFrom == null || propertyContainerTo == null) 
                return;

            System.Reflection.PropertyInfo[] propertiesFrom = propertyContainerFrom.GetType().GetProperties();
            System.Reflection.PropertyInfo[] propertiesTo = propertyContainerTo.GetType().GetProperties();

            foreach (System.Reflection.PropertyInfo from in propertiesFrom)
            {
                if (!from.CanRead) continue;
                foreach (System.Reflection.PropertyInfo to in propertiesTo)
                {
                    if (from.Name == to.Name)
                    {
                        if (!to.CanWrite) break;

                        object fromPropertyValue = from.GetValue(propertyContainerFrom, null);

                        if (fromPropertyValue == null)
                        {
                            if (reflectNull)
                                to.SetValue(propertyContainerTo, null, null);
                            continue;
                        }

                        #region ? --> String
                        else if (from.PropertyType != typeof(string) && to.PropertyType == typeof(string))
                        {
                            if (from.PropertyType == typeof(string[]))
                            {
                                //  String[] --> String
                                to.SetValue(propertyContainerTo, Utility.ConvertToCsvString((string[])fromPropertyValue), null);
                            }
                            if (from.PropertyType == typeof(int[]))
                            {
                                //  int[] --> String
                                to.SetValue(propertyContainerTo, Utility.ConvertToCsvString((int[])fromPropertyValue), null);
                            }

                            if (from.PropertyType == typeof(Data.SubList))
                            {
                                if (fromPropertyValue == null)
                                    to.SetValue(propertyContainerTo, null, null);
                                else
                                {
                                    //  Sublist --> String
                                    Data.SubList candidate = (Data.SubList)fromPropertyValue;
                                    if (candidate != null && candidate.Items != null && candidate.Items.Length > 0)
                                    {
                                        to.SetValue(propertyContainerTo, candidate.Serialized, null);
                                    }
                                }
                            }
                            if (from.PropertyType == typeof(DateTime))
                            {
                                //  Datetime --> String
                                DateTime dt = (DateTime)fromPropertyValue;
                                to.SetValue(propertyContainerTo, dt.ToString("dd/MM/yyyy"), null);
                            }
                            else if (from.PropertyType == typeof(decimal))
                            {
                                //  Decimal --> String
                                Decimal tmp;
                                if (IsDecimal(fromPropertyValue, out tmp))
                                {
                                    System.Globalization.CultureInfo info = new System.Globalization.CultureInfo("en-US");
                                    to.SetValue(propertyContainerTo, tmp.ToString(info), null);
                                }
                            }
                            else if (from.PropertyType == typeof(bool))
                            {
                                //  Boolean --> String
                                bool tmp = Convert.ToBoolean(fromPropertyValue);
                                if (tmp)
                                    to.SetValue(propertyContainerTo, "1", null);
                                else
                                    to.SetValue(propertyContainerTo, "0", null);
                            }
                            else if (from.PropertyType == typeof(int) || from.PropertyType == typeof(Guid))
                                to.SetValue(propertyContainerTo, fromPropertyValue.ToString(), null);
                        }
                        #endregion
                        #region String --> ?
                        else if (from.PropertyType == typeof(string) && to.PropertyType != typeof(string))
                        {
                            if (to.PropertyType == typeof(string[]))
                            {
                                //  String[] --> String[]

                                if (fromPropertyValue == null)
                                    to.SetValue(propertyContainerTo, null, null);
                                else
                                    to.SetValue(propertyContainerTo, fromPropertyValue.ToString().Split(','), null);
                            }
                            if (to.PropertyType == typeof(int[]))
                            {
                                //  String -- > int[]
                                to.SetValue(propertyContainerTo, Utility.ConvertToIntArray(fromPropertyValue.ToString().Split(',')), null);
                            }

                            if (to.PropertyType == typeof(Data.SubList))
                            {
                                //  String -- > Sublist
                                if (fromPropertyValue != null && !string.IsNullOrEmpty(fromPropertyValue.ToString()))
                                {
                                    Data.SubList candidate = Data.SubList.GetDeserialized(fromPropertyValue.ToString());
                                    to.SetValue(propertyContainerTo, candidate, null);
                                }
                            }
                            if (to.PropertyType == typeof(DateTime))
                            {
                                //  String --> Datetime
                                to.SetValue(propertyContainerTo, ConvertWimDateTime(fromPropertyValue), null);
                            }
                            else if (to.PropertyType == typeof(decimal))
                            {
                                //  String --> Decimal
                                Decimal tmp;
                                if (IsDecimal(fromPropertyValue, out tmp))
                                {
                                    to.SetValue(propertyContainerTo, tmp, null);
                                }
                            }
                            else if (to.PropertyType == typeof(bool))
                            {
                                //  String --> Boolean

                                string tmp1 = fromPropertyValue.ToString();
                                if (!string.IsNullOrEmpty(tmp1))
                                {
                                    bool tmp;
                                    if (tmp1 == "1") tmp = true;
                                    else if (tmp1 == "0") tmp = false;
                                    else
                                        tmp = Convert.ToBoolean(fromPropertyValue);

                                    to.SetValue(propertyContainerTo, tmp, null);
                                }
                            }
                            else if (to.PropertyType == typeof(int))
                            {
                                //  String --> Int
                                to.SetValue(propertyContainerTo, ConvertToInt(fromPropertyValue), null);
                            }
                            else if (from.PropertyType == typeof(Guid))
                            {
                                //  String --> Guid
                                Guid guid;
                                if (IsGuid(fromPropertyValue, out guid))
                                    to.SetValue(propertyContainerTo, guid, null);
                            }
                        }
                        #endregion
                        #region nullable
                        else if (from.PropertyType == typeof(decimal?) && to.PropertyType == typeof(decimal))
                        {
                            if (fromPropertyValue != null)
                                to.SetValue(propertyContainerTo, ((decimal?)fromPropertyValue).Value, null);
                        }
                        else if (from.PropertyType == typeof(decimal) && to.PropertyType == typeof(decimal?))
                        {
                            to.SetValue(propertyContainerTo, fromPropertyValue, null);
                        }
                        else if (from.PropertyType == typeof(int?) && to.PropertyType == typeof(int))
                        {
                            if (fromPropertyValue != null)
                                to.SetValue(propertyContainerTo, ((int?)fromPropertyValue).Value, null);
                        }
                        else if (from.PropertyType == typeof(int) && to.PropertyType == typeof(int?))
                        {
                            to.SetValue(propertyContainerTo, fromPropertyValue, null);
                        }
                        else if (from.PropertyType == typeof(long?) && to.PropertyType == typeof(long))
                        {
                            if (fromPropertyValue != null)
                                to.SetValue(propertyContainerTo, ((long?)fromPropertyValue).Value, null);
                        }
                        else if (from.PropertyType == typeof(long) && to.PropertyType == typeof(long?))
                        {
                            to.SetValue(propertyContainerTo, fromPropertyValue, null);
                        }
                        else if (from.PropertyType == typeof(DateTime?) && to.PropertyType == typeof(DateTime))
                        {
                            if (fromPropertyValue != null)
                                to.SetValue(propertyContainerTo, ((DateTime?)fromPropertyValue).Value, null);
                        }
                        else if (from.PropertyType == typeof(DateTime) && to.PropertyType == typeof(DateTime?))
                        {
                            to.SetValue(propertyContainerTo, fromPropertyValue, null);
                        }
                        #endregion
                        else if (from.PropertyType == to.PropertyType)
                        {
                            to.SetValue(propertyContainerTo, from.GetValue(propertyContainerFrom, null), null);
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Switch between line feed and &lt;br/&gt;
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string CleanLineFeed(string text)
        {
            return CleanLineFeed(text, true);
        }

        /// <summary>
        /// Clears the paragraph wrapper (removes the starting/ending paragraph tags
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        public static string CleanParagraphWrap(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;

            string candidate = input;
            
            Par link = new Par();
            Regex rex = new Regex(@"(?<TEXT><p\b[^>]*>(.*?)</p>)", RegexOptions.IgnoreCase);
            
            candidate = rex.Replace(candidate, link.Links);
            while (link.Count > 0)
            {
                link.Count = 0;
                candidate = rex.Replace(candidate, link.Links);
            }
            
            //  Replace lost P             
            candidate = candidate
                .Replace("<p>", string.Empty)
                .Replace("</p>", string.Empty)
                .Replace("<P>", string.Empty)
                .Replace("</P>", string.Empty)
                ;

            candidate = Utility.CleanLeadingAndTrailingLineFeed(candidate);
            return candidate;
        }

        internal class Par
        {
            internal int Count;
            /// <summary>
            /// Spanses the specified m.
            /// </summary>
            /// <param name="m">The m.</param>
            /// <returns></returns>
            internal string Links(Match m)
            {
                Regex rex = new Regex(@"<p.*?>(?<TEXT>(.|\n)*)(</p>)", RegexOptions.IgnoreCase);
                string candidate = rex.Replace(m.Groups["TEXT"].Value, this.Replace);
                this.Count++;
                return string.Concat(candidate, "<br/><br/>");
            }

            private string Replace(Match m)
            {
                return m.Groups["TEXT"].Value;
            }
        }

        /// <summary>
        /// Cleans the leading and trailing line feed.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        public static string CleanLeadingAndTrailingLineFeed(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;
            string candidate = input.Trim();

            bool hasLeadingPar = false;
            if (candidate.StartsWith("<p>", true, System.Threading.Thread.CurrentThread.CurrentCulture))
            {
                hasLeadingPar = true;
                candidate = candidate.Remove(0, 3);
                if (candidate.EndsWith("</p>", true, System.Threading.Thread.CurrentThread.CurrentCulture))
                    candidate = candidate.Substring(0, candidate.Length - 4);
            }

            bool check = true;

            string[] exceptionList = new string[] { "&nbsp;", "<br />", "<br/>", "<br>", "<p></p>", "<p>&nbsp;</p>" };

            while (check)
            {
                check = false;
                foreach (string exception in exceptionList)
                {
                    if (candidate.EndsWith(exception, true, System.Threading.Thread.CurrentThread.CurrentCulture))
                    {
                        check = true;
                        candidate = candidate.Substring(0, candidate.Length - exception.Length).Trim();
                    }
                    if (candidate.StartsWith(exception, true, System.Threading.Thread.CurrentThread.CurrentCulture))
                    {
                        check = true;
                        candidate = candidate.Remove(0, exception.Length).Trim();
                    }
                }
            }

            while(candidate.Contains("<br/><br/><br/>"))
            {
                candidate = candidate.Replace("<br/><br/><br/>", "<br/><br/>");
            }

            if (hasLeadingPar)
                return string.Concat("<p>", candidate, "</p>");
            return candidate;
        }

        /// <summary>
        /// Applies the richtext links.
        /// </summary>
        /// <param name="site">The site.</param>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public static string ApplyRichtextLinks(Data.Site site, string text)
        {
            return ApplyRichtextLinks(site, text, null);
        }

        /// <summary>
        /// Apply correct hyperlinks to richtext fields.
        /// </summary>
        /// <param name="site"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string ApplyRichtextLinks(Data.Site site, string text, string databaseMap)
        {
            if (text == null) return null;
            Framework.Templates.RichLink rlink = new Framework.Templates.RichLink(site);
            //rlink.DatabaseMap = databaseMap;

            string candidate = Framework.Templates.RichLink.GetCleaner.Replace(text.ToString(), rlink.CleanLinkData);
            
            //  Introduced for new richtext editor.
            candidate = Framework.Templates.RichLink.GetCleaner2.Replace(candidate, rlink.CleanLinkData);

            var cleaner = new RichRext.AgilityCleaner();
            candidate = cleaner.CleanAssetUrl(candidate);
            return candidate;
        }

        public static string CleanLink(Site site, string text)
        {
            if (text == null) return null;
            Framework.Templates.RichLink rlink = new Framework.Templates.RichLink(site);
            string candidate = Framework.Templates.RichLink.GetCleaner.Replace(text.ToString(), rlink.CleanEmptyLink);

            //  Introduced for new richtext editor.
            candidate = Framework.Templates.RichLink.GetCleaner2.Replace(candidate, rlink.CleanEmptyLink);

            return candidate;
        }

        /// <summary>
        /// Applies the richtext.
        /// </summary>
        /// <param name="site">The site.</param>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public static string ApplyRichtext(Data.Site site, string text)
        {
            return ApplyRichtext(site, text, null);
        }

        /// <summary>
        /// Applies the richtext cleanup (tables) and also calls ApplyRichtextLinks. 
        /// </summary>
        /// <param name="site">The site.</param>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public static string ApplyRichtext(Data.Site site, string text, string databaseMap)
        {
            if (text == null) return null;
            text = ApplyRichtextLinks(site, text, databaseMap);

            Framework.ContentInfoItem.RichTextCleanup prepare = new Framework.ContentInfoItem.RichTextCleanup();
            return prepare.Clean(text);

            //Framework.RichTextPrepare prepare = new Framework.RichTextPrepare();
            //return prepare.Clean(text);
        }


        /// <summary>
        /// Cleans the line feed.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="fromLfToBr">if set to <c>true</c> [from lf to br].</param>
        /// <returns></returns>
        public static string CleanLineFeed(string text, bool fromLfToBr)
        {
            return CleanLineFeed(text, fromLfToBr, false, false);
        }

        /// <summary>
        /// Switch between line feed and &lt;br/&gt;
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="fromLfToBr">if set to <c>true</c> [from lf to br].</param>
        /// <param name="cleanConcurrentBreakTags">if set to <c>true</c> [clean concurrent break tags].</param>
        /// <param name="cleanEndingBreakTags">if set to <c>true</c> [clean ending break tags].</param>
        /// <returns></returns>
        public static string CleanLineFeed(string text, bool fromLfToBr, bool cleanConcurrentBreakTags, bool cleanEndingBreakTags)
        {
            if (text == null) return null;

            if (fromLfToBr)
            {
                text = new Regex("\r\n", RegexOptions.IgnoreCase).Replace(text, "<br/>");
                text = new Regex("\r", RegexOptions.IgnoreCase).Replace(text, "<br/>");
                text = new Regex("\n", RegexOptions.IgnoreCase).Replace(text, "<br/>");
                if (cleanConcurrentBreakTags)
                    return CleanConcurrentBreaks(text, cleanEndingBreakTags);
                else
                    return text;
            }
            else
            {
                Regex rex = new Regex("<br.*?>", RegexOptions.IgnoreCase);
                text = rex.Replace(text, "\n");
                if (cleanConcurrentBreakTags)
                    return CleanConcurrentBreaks(text, cleanEndingBreakTags);
                else
                    return text;
            }
        }

        /// <summary>
        /// Cleans the concurrent breaks (max 2).
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="cleanEndingBreakTags">if set to <c>true</c> [clean ending break tags].</param>
        /// <returns></returns>
        public static string CleanConcurrentBreaks(string text, bool cleanEndingBreakTags)
        {
            if (text == null) return null;
            //return text;
            text = new Regex("<br.*?>", RegexOptions.IgnoreCase | RegexOptions.Multiline).Replace(text, "<br/>");
            text = new Regex("<br>&nbsp;<br>", RegexOptions.IgnoreCase | RegexOptions.Multiline).Replace(text, "<br/><br/>");
            text = new Regex("(<br>){2,}", RegexOptions.IgnoreCase | RegexOptions.Multiline).Replace(text, "<br><br/>");
            text = new Regex("<p>&nbsp;</p>", RegexOptions.IgnoreCase | RegexOptions.Multiline).Replace(text, string.Empty);
            if (cleanEndingBreakTags)
                return CleanEndingBreaks(text);
            return text;
        }

        /// <summary>
        /// Cleans the ending breaks.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public static string CleanEndingBreaks(string text)
        {
            if (text == null) return null;
            //return text;
            text = new Regex("(<br>)+$", RegexOptions.IgnoreCase | RegexOptions.Multiline).Replace(text, string.Empty);
            return text;
        }

        private static Regex m_CleanUrlSpaces;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string CleanUrl(string url)
        {
            if (string.IsNullOrEmpty(url)) return null;

            //if (!Data.Common.HasUrlSpaceCleanup)
            //    return url;

            if (m_CleanUrlSpaces == null)
                m_CleanUrlSpaces = new Regex(@"\s", RegexOptions.IgnoreCase);

            string replacement = Data.Environment.Current["SPACE_REPLACEMENT"];

            if (replacement == " ") return url;

            string candidate = m_CleanUrlSpaces.Replace(url, replacement);
            return candidate;
        }

        private static Regex m_CleanFormatting;
        /// <summary>
        /// Clear text from all Html formatting
        /// </summary>
        public static string CleanFormatting(string richtext)
        {
            if (string.IsNullOrEmpty(richtext)) return null;
            var candidate = richtext;

            candidate = CleanLineFeed(candidate, false);
            //  Clean Tab/Newline
            //candidate = new Regex(@"[\n\t]", RegexOptions.Multiline).Replace(richtext, "");
            //  Clean scripts
            candidate = new Regex(@"<script.*?/script>", RegexOptions.Multiline).Replace(candidate, "");
            //  Clean styles
            candidate = new Regex(@"<style.*?/style>", RegexOptions.Multiline).Replace(candidate, "");

            candidate = new Regex(@"<select.*?/select>", RegexOptions.Multiline).Replace(candidate, "");

            if (m_CleanFormatting == null)
                m_CleanFormatting = new Regex(@"<.*?>");

            candidate = m_CleanFormatting.Replace(candidate, "");
            
            //  Clean double spaces
          //  candidate = new Regex(@"\s{2,}", RegexOptions.Multiline).Replace(candidate, " ");
            return candidate.Trim();
        }

        /// <summary>
        /// Cleans the XML data (for the Carriage return).
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="fromHexToBackSlash">if set to <c>true</c> [from hex to back slash].</param>
        /// <returns></returns>
        public static string CleanXmlData(string candidate, bool fromHexToBackSlash)
        {
            if (fromHexToBackSlash)
                return candidate.Replace("#x9;", "\t").Replace("#xD;", "\n").Replace("#xA;", "\r");
            else
                return candidate.Replace("\t", "#x9;").Replace("\n", "#xD;").Replace("\r", "#xA;");
        }

        /// <summary>
        /// Gets the instance list collection.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="listName">Name of the list.</param>
        /// <returns></returns>
        public static ListItemCollection GetInstanceListCollection(Data.IComponentList list, string listName, HttpContext context)
        {
            return GetInstanceListCollection(list, listName, null, context);
        }

        /// <summary>
        /// 
        /// </summary>
        public enum IconImage
        {
            /// <summary>
            /// 
            /// </summary>
            Yes,
            /// <summary>
            /// 
            /// </summary>
            No,
            /// <summary>
            /// 
            /// </summary>
            Note,
            /// <summary>
            /// 
            /// </summary>
            New,
            /// <summary>
            /// 
            /// </summary>
            Rfc_Green, 
            Rfc_Orange,
            Rfc_Red,
            Rfc_Purple,
            /// <summary>
            /// 
            /// </summary>
            File,
            /// <summary>
            /// 
            /// </summary>
            NoFile,
            /// <summary>
            /// 
            /// </summary>
            Product_16,
            /// <summary>
            /// 
            /// </summary>
            Product_16n,
            /// <summary>
            /// 
            /// </summary>
            invoice_16,
            /// <summary>
            /// 
            /// </summary>
            invoice_16_plus,
            /// <summary>
            /// 
            /// </summary>
            invoice_16_stop,
            /// <summary>
            /// 
            /// </summary>
            invoice_16_ok,
            /// <summary>
            /// 
            /// </summary>
            invoice_16_info,
            /// <summary>
            /// 
            /// </summary>
            timesheet_16,
            /// <summary>
            /// 
            /// </summary>
            timesheet_16_info,
            /// <summary>
            /// 
            /// </summary>
            timesheet_16_ok,
            /// <summary>
            /// 
            /// </summary>
            delete_16,
            /// <summary>
            /// 
            /// </summary>
            accept_16,
            acceptno_16,
            star_16,
            accept_orange_16,
            /// <summary>
            /// 
            /// </summary>
            add_16,
            /// <summary>
            /// 
            /// </summary>
            help_16, 
            info_16,
            note_red_16,
            /// <summary>
            /// 
            /// </summary>
            next_16,
            /// <summary>
            /// 
            /// </summary>
            play_16,
            /// <summary>
            /// 
            /// </summary>
            remove_16,
            /// <summary>
            /// 
            /// </summary>
            up_16,
            /// <summary>
            /// 
            /// </summary>
            updown_16,
            /// <summary>
            /// 
            /// </summary>
            refresh_16,
            zoom_16,
            zoomPage_16
        }

        public enum IconSize
        {
            /// <summary>
            /// 
            /// </summary>
            Small = 10,
            /// <summary>
            /// 
            /// </summary>
            Normal = 16
        }

        /// <summary>
        /// Gets the icon image string.
        /// </summary>
        /// <param name="icon">The icon.</param>
        /// <returns></returns>
        public static string GetIconImageString(Sushi.Mediakiwi.Beta.GeneratedCms.Console container, IconImage icon)
        {
            return GetIconImageString(container, icon, null);
        }

        /// <summary>
        /// Gets the icon image string.
        /// </summary>
        /// <param name="icon">The icon.</param>
        /// <param name="tooltip">The tooltip.</param>
        /// <returns></returns>
        public static string GetIconImageString(Sushi.Mediakiwi.Beta.GeneratedCms.Console container, IconImage icon, string tooltip)
        {
            return GetIconImageString(container, icon, IconSize.Small, tooltip);
        }

        /// <summary>
        /// Gets the icon image string.
        /// </summary>
        /// <param name="icon">The icon.</param>
        /// <param name="size">The size.</param>
        /// <param name="tooltip">The tooltip.</param>
        /// <returns></returns>
        public static string GetIconImageString(Sushi.Mediakiwi.Beta.GeneratedCms.Console container, IconImage icon, IconSize size, string tooltip)
        {
            int width = (int)size, height = (int)size;
            
            if (icon == IconImage.Yes) icon = IconImage.accept_16;
            else if (icon == IconImage.No) icon = IconImage.delete_16;
            //else if (icon == IconImage.Note) icon = IconImage.info_16;
            //else if (icon == IconImage.New) icon = IconImage.add_16;

            if (icon.ToString().Contains("16")) width = 16;
            if (icon.ToString().Contains("80")) width = 80;
            if (icon.ToString().Contains("10")) width = 10;
            height = width;

            if (true)//CommonConfiguration.FORCE_NEWSTYLE)// || Data.ApplicationUser.Select().ShowNewDesign)
            {
                string className = null;
                switch (icon)
                {
                    case IconImage.delete_16:
                    case IconImage.No:
                        className = "icon icon-times-circle-o neg";
                        break;

                    case IconImage.accept_16:
                    case IconImage.Yes:
                        className = "icon icon-check-circle-o pos";
                        break;

                    case IconImage.info_16:
                        className = "flaticon solid question-1 icon green";
                        break;

                    //case IconImage.New:
                    //    className = "flaticon solid question-1 icon green";
                    //    break;


                    case IconImage.Note:
                        return string.Format("<img src=\"{0}\" title=\"{1}\" height=\"10\" width=\"21\">",
                            container.AddApplicationPath(string.Format("/repository/wim/images/icons/note_{0}.png", (int)size), true), tooltip);
                    case IconImage.New:
                        return string.Format("<img src=\"{0}\" title=\"{1}\" height=\"10\" width=\"21\">",
                            container.AddApplicationPath(string.Format("/repository/wim/images/icons/new_10.png", (int)size), true), tooltip);
                    case IconImage.Rfc_Green:
                        return string.Format("<img src=\"{0}\" title=\"{1}\" height=\"10\" width=\"21\">",
                            container.AddApplicationPath(string.Format("/repository/wim/images/icons/rfc_green_10.png", (int)size), true), tooltip);
                    case IconImage.Rfc_Orange:
                        return string.Format("<img src=\"{0}\" title=\"{1}\" height=\"10\" width=\"21\">",
                            container.AddApplicationPath(string.Format("/repository/wim/images/icons/rfc_orange_10.png", (int)size), true), tooltip);
                    case IconImage.Rfc_Red:
                        return string.Format("<img src=\"{0}\" title=\"{1}\" height=\"10\" width=\"21\">",
                            container.AddApplicationPath(string.Format("/repository/wim/images/icons/rfc_red_10.png", (int)size), true), tooltip);
                    case IconImage.Rfc_Purple:
                        return string.Format("<img src=\"{0}\" title=\"{1}\" height=\"10\" width=\"21\">",
                            container.AddApplicationPath(string.Format("/repository/wim/images/icons/rfc_purple_10.png", (int)size), true), tooltip);
                    case IconImage.File:
                        className = "flaticon solid paperclip-2 icon green";
                        break;

                    case IconImage.NoFile:
                        className = "flaticon solid paperclip-2 icon red";
                        break;
                    default:
                        return string.Format("<img src=\"{0}\" title=\"{1}\" height=\"10\" width=\"21\">",
                            container.AddApplicationPath(string.Format("/repository/wim/images/icons/{0}.png", icon.ToString()), true), tooltip);
                }

                if (!string.IsNullOrEmpty(className))
                {
                    if (string.IsNullOrEmpty(tooltip))
                        return string.Format("<figure class=\"{0}\"></figure>", className);
                    return string.Format("<abbr title=\"{1}\"><label for=\"\" class=\"{0}\"></label></abbr>", className, tooltip);
                }
                return null;
            }
        }

        /// <summary>
        /// Gets the icon image string.
        /// </summary>
        /// <param name="icon">The icon.</param>
        /// <param name="size">The size.</param>
        /// <param name="tooltip">The tooltip.</param>
        /// <param name="url">The URL.</param>
        /// <returns></returns>
        public static string GetIconImageString(Sushi.Mediakiwi.Beta.GeneratedCms.Console container, IconImage icon, IconSize size, string tooltip, string url)
        {
            return string.Format("<a href=\"{0}\" target=\"_blank\">{1}</a>", url, GetIconImageString(container, icon, size, tooltip));
        }

        /// <summary>
        /// Gets the icon image string.
        /// </summary>
        /// <param name="icon">The icon.</param>
        /// <param name="tooltip">The tooltip.</param>
        /// <param name="url">The URL.</param>
        /// <returns></returns>
        public static string GetIconImageString(Sushi.Mediakiwi.Beta.GeneratedCms.Console container, IconImage icon, string tooltip, string url)
        {
            return string.Format("<a href=\"{0}\">{1}</a>", url, GetIconImageString(container, icon, tooltip));
        }

        /// <summary>
        /// Gets the icon image string.
        /// </summary>
        /// <param name="icon">The icon.</param>
        /// <param name="tooltip">The tooltip.</param>
        /// <param name="url">The URL.</param>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public static string GetIconImageString(Sushi.Mediakiwi.Beta.GeneratedCms.Console container, IconImage icon, string tooltip, string url, string text)
        {
            return string.Format("<a href=\"{0}\">{1} {2}</a>", url, GetIconImageString(container, icon, tooltip), text);
        }

        /// <summary>
        /// Gets the instance list collection.
        /// </summary>
        /// <param name="site">The site.</param>
        /// <param name="list">The list.</param>
        /// <param name="listName">Name of the list.</param>
        /// <returns></returns>
        public static ListItemCollection GetInstanceListCollection(Data.IComponentList list, string listName, Data.Site site, HttpContext context)
        {
            Framework.IComponentListTemplate instance = list.GetInstance(context);
            return GetInstanceListCollection(list, listName, site, instance);
        }

        /// <summary>
        /// Gets the instance list collection.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="listName">Name of the list.</param>
        /// <param name="site">The site.</param>
        /// <param name="instance">The instance.</param>
        /// <returns></returns>
        public static ListItemCollection GetInstanceListCollection(Data.IComponentList list, string listName, Data.Site site, Framework.IComponentListTemplate instance)
        {
            //instance.CurrentList = list;
            //instance.CurrentSite = site;

            System.Type type = instance.GetType();
            
            if (listName.Contains(":"))
            {
                string[] split = listName.Split(':');
                System.Reflection.MethodInfo method = type.GetMethod(split[0]);
                object[] index = new object[1] { Convert.ToInt32(split[1]) };

                if (method == null)
                    return new ListItemCollection();

                return method.Invoke(instance, index) as ListItemCollection;

            }

            System.Reflection.PropertyInfo info = type.GetProperty(listName);
            if (info == null)
                return new ListItemCollection();
            
            return info.GetValue(instance, null) as ListItemCollection;
        }

        /// <summary>
        /// Gets the instance options.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <param name="typeName">Name of the type.</param>
        /// <returns></returns>
        public static Framework.iOption GetInstanceOptions(string assembly, string typeName)
        {
            System.Type type = System.Type.GetType(typeName);
            return CreateInstance(assembly, typeName) as Framework.iOption;
        }

        /// <summary>
        /// Creates the instance.
        /// </summary>
        /// <param name="assemblyName">Name of the assembly.</param>
        /// <param name="className">Name of the class.</param>
        /// <returns></returns>
        public static Object CreateInstance(string assemblyName, string className)
        {
            Type type;
            return CreateInstance(assemblyName, className, out type);
        }

        /// <summary>
        /// Creates the instance.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <returns></returns>
        public static Object CreateInstance(Data.IComponentList list)
        {
            Type type;
            return CreateInstance(list.AssemblyName, list.ClassName, out type);
        }

        /// <summary>
        /// Creates the instance.
        /// </summary>
        /// <param name="assemblyName">Name of the assembly.</param>
        /// <param name="className">Name of the class.</param>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static Object CreateInstance(string assemblyName, string className, out Type type)
        {
            return CreateInstance(assemblyName, className, out type, true);
        }

        /// <summary>
        /// Creates the instance.
        /// </summary>
        /// <param name="assemblyName">Name of the assembly.</param>
        /// <param name="className">Name of the class.</param>
        /// <param name="type">The type.</param>
        /// <param name="onExceptionThrow">if set to <c>true</c> [on exception throw].</param>
        /// <returns></returns>
        public static Object CreateInstance(string assemblyName, string className, out Type type, bool onExceptionThrow)
        {
            type = null;
            //if (string.IsNullOrEmpty(assemblyName) || string.IsNullOrEmpty(className))
            //{
            //    if (onExceptionThrow)
            //    {
            //        Utilities.CacheItemManager.FlushIndexOfCacheObjects("Data.wim_ComponentLists");
            //        throw new Exception("No assemblyFile or classname provided!");
            //        return null;
            //    }
            //}
            //if (assemblyName.Equals("appcentre.dll", StringComparison.CurrentCultureIgnoreCase))
            //    assemblyName = "Framework.dll";

            try
            {
                System.IO.FileInfo nfo = null;
                
                //if (HttpContext.Current == null)
                //{
                //    // [20130704:MR/DV] This check is added for when this instance is approached via a seperate (possible non-web) thread
                //    // Such as a Fire-and-forget task.
                //    if (System.Web.Hosting.HostingEnvironment.IsHosted)
                //    {
                //        nfo = new System.IO.FileInfo(
                //            System.Web.Hosting.HostingEnvironment.MapPath(string.Concat("~/bin/", assemblyName)));
                //    }
                //    else
                //    {
                        nfo = new System.IO.FileInfo(
                        string.Concat(
                            Assembly.GetCallingAssembly().Location.Replace(string.Concat(Assembly.GetCallingAssembly().GetName().Name, ".dll"), string.Empty)
                            , assemblyName));
                //    }
                //}
                //else
                //{
                //    if (System.Web.Hosting.HostingEnvironment.IsHosted)
                //    {
                //        nfo = new System.IO.FileInfo(
                //            System.Web.Hosting.HostingEnvironment.MapPath(string.Concat("~/bin/", assemblyName)));
                //    }
                //    else
                //    {
                //        nfo = new System.IO.FileInfo(
                //            HttpContext.Current.Server.MapPath(
                //                string.Concat(HttpContext.Current.Request.ApplicationPath, "/bin/", assemblyName)));
                //    }
                //}
                //if (nfo == null)
                //{
                //    Utilities.CacheItemManager.FlushIndexOfCacheObjects("Data.wim_ComponentLists");
                //    throw new Exception(string.Format("Could not find the assemblyFile[{0}] or the provided classname[{1}]!", assemblyName, className));
                //}

                Assembly assem = Assembly.LoadFrom(nfo.FullName);
                type = assem.GetType(className);

                return System.Activator.CreateInstance(type);
            }
            catch(Exception ex)
            {
                if (ex.InnerException != null && ex.InnerException.GetType() == typeof(UnauthorizedAccessException))
                {
                    //Utilities.CacheItemManager.FlushIndexOfCacheObjects("Data");
                    throw ex.InnerException;
                    return null;
                }
                if (onExceptionThrow)
                {
                    //Utilities.CacheItemManager.FlushIndexOfCacheObjects("Data");
                    throw new Exception(string.Format("Could not initiate the requested type[{0}]!", className), ex);
                }
                return null;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        public class GlobalRegularExpression
        {
            /// <summary>
            /// Determines whether [is dutch social fiscal number] [the specified candidate].
            /// </summary>
            /// <param name="candidate">The candidate.</param>
            /// <returns>
            /// 	<c>true</c> if [is dutch social fiscal number] [the specified candidate]; otherwise, <c>false</c>.
            /// </returns>
            public static bool IsDutchSocialFiscalNumber(string candidate)
            {
                if (candidate.Length != 9) return false; int i = 9;

                int total = 0; for (int j = 0; j < 8; j++)
                {

                    char t = candidate[j]; total += int.Parse(t.ToString()) * i; i--;

                }
                int rest = int.Parse(candidate[8].ToString());

                return ((total % 11) == rest);

            }

            /// <summary>
            /// 
            /// </summary>
            public const string ReplaceRelativePathSlash = @"//*|\\\\*";
            /// <summary>
            /// 
            /// </summary>
            public const string NotAcceptableFilenameCharacter = @"^[^\|\?\\/:\*""<>]*$";
            /// <summary>
            /// 
            /// </summary>
            //public const string ReplaceNotAcceptableFilenameCharacter = @"[\|\?\\/:\*""<>']";
            /// <summary>
            /// 
            /// </summary>
            public const string ReplaceNotAcceptableFilenameCharacter = @"[^A-Za-z0-9\s/\-_.]";
            /// <summary>
            /// 
            /// </summary>
            public const string OnlyNumeric = @"^[-+]?\d*$";
            /// <summary>
            /// Only can contain a '.' as decimal seperator
            /// </summary>
            public const string OnlyDecimal = @"^[-+]?\d*[.]?\d*$";
            /// <summary>
            /// 
            /// </summary>
            public const string OnlyPositiveDecimal = @"^\d*[.]?\d*$";
            /// <summary>
            /// 
            /// </summary>
            public const string OnlyGUID = @"^(\{){0,1}[0-9a-fA-F]{8}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{12}(\}){0,1}$";
            //public const string OnlyDecimal = @"^[-+]?\d*$";
            /// <summary>
            /// 
            /// </summary>
            public const string OnlyAcceptableFilenameCharacter = @"^[a-z|A-Z|0-9| _-]*$";
            /// <summary>
            /// 
            /// </summary>
            public const string EmailAddress = @"^[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,}$";

            /// <summary>
            /// 
            /// </summary>
            public class Implement
            {
                /// <summary>
                /// Clean the url from double or more repeating // and or double and or more repeating \\. 
                /// This should applied using a replacement value '/'.
                /// Note: This regex also replaces the two forwards slashes in http:// , so use with care.
                /// </summary>
                private static Regex m_CleanUrlSpaces;
                /// <summary>
                /// 
                /// </summary>
                public static Regex CleanUrlSpaces
                {
                    get
                    {
                        if (m_CleanUrlSpaces == null)
                            m_CleanUrlSpaces = new Regex(@"\s", RegexOptions.IgnoreCase);
                        return m_CleanUrlSpaces;
                    }
                }

                /// <summary>
                /// Clean the url from double or more repeating // and or double and or more repeating \\. 
                /// This should applied using a replacement value '/'.
                /// Note: This regex also replaces the two forwards slashes in http:// , so use with care.
                /// </summary>
                private static Regex m_CleanRelativePathSlash;
                /// <summary>
                /// 
                /// </summary>
                public static Regex CleanRelativePathSlash
                {
                    get
                    {
                        if (m_CleanRelativePathSlash == null)
                            m_CleanRelativePathSlash = new Regex(GlobalRegularExpression.ReplaceRelativePathSlash);
                        return m_CleanRelativePathSlash;
                    }
                }

                /// <summary>
                /// Used from checked if the requested value is of a numeric kind.
                /// Both negative and positive values are allowed except decimal values.
                /// </summary>
                private static Regex m_onlyNumeric;
                /// <summary>
                /// 
                /// </summary>
                public static Regex OnlyNumeric
                {
                    get
                    {
                        if (m_onlyNumeric == null)
                            m_onlyNumeric = new Regex(GlobalRegularExpression.OnlyNumeric);
                        return m_onlyNumeric;
                    }
                }

                /// <summary>
                /// Used from checked if the applied pagename or URL has all accepted characters in it.
                /// This can be used to replace non-valid characters. Excepted are a-z, A-Z, 0-9 and .+-
                /// </summary>
                private static Regex m_ReplaceNotAcceptableFilenameCharacter;
                /// <summary>
                /// 
                /// </summary>
                public static Regex ReplaceNotAcceptableFilenameCharacter
                {
                    get
                    {
                        if (m_ReplaceNotAcceptableFilenameCharacter == null)
                            m_ReplaceNotAcceptableFilenameCharacter = new Regex(GlobalRegularExpression.ReplaceNotAcceptableFilenameCharacter);
                        return m_ReplaceNotAcceptableFilenameCharacter;
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static string GetHtmlFormattedLastServerError(System.Exception ex)
        {
            string error = "";
            while (ex != null)
            {
                string stackTrace = ex.StackTrace == null ? "" : ex.StackTrace;
                Regex cleanUpStrackTrace = new Regex("( at)");
                
                stackTrace = cleanUpStrackTrace.Replace(stackTrace, "<br/>at");

                if (error != "")
                    error += "<br/><br/>";
                if (CommonConfiguration.IS_LOCAL_DEVELOPMENT)
                {
                    error += string.Format(@"<b>Error:</b><br/>{0}
<br/><br/><b>Source:</b><br/>{1}
<br/><br/><b>Method:</b><br/>{2}
<br/><br/><b>Stacktrace:</b>{3}
"
                        , ex.Message, ex.Source, ex.TargetSite, stackTrace);
                }
                else
                {
                    error += string.Format(@"<b>Error:</b><br/>{0}"
                        , ex.Message);
                }
                ex = ex.InnerException;
            }
            return error;          
        }

        /// <summary>
        /// Gets the deserialized.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="xml">The XML.</param>
        /// <returns></returns>
        public static object GetDeserialized(System.Type type, string xml)
        {
            return GetDeserialized(type, xml, true);
        }

        /// <summary>
        /// Gets the deserialized.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="xml">The XML.</param>
        /// <param name="onErrorThrowException">if set to <c>true</c> [on error throw exception].</param>
        /// <returns></returns>
        public static object GetDeserialized(System.Type type, string xml, bool onErrorThrowException)
        {
            if (string.IsNullOrEmpty(xml)) return null;

            if (type == typeof(Data.CustomDataItem[]) && xml.Contains("ArrayOfField"))
            {
                xml = xml.Replace("ArrayOfField", "ArrayOfData").Replace("<Field>", "<Data>").Replace("</Field>", "</Data>");
            }

            XmlDocument xmldoc = new XmlDocument();
            try
            {
                xmldoc.LoadXml(xml);

                //  This went wrong with ArrayOf...
                if (xmldoc.DocumentElement.ChildNodes.Count == 0) return null;

                XmlNodeReader reader = new XmlNodeReader(xmldoc.DocumentElement);

                string typeName = type.Name;
                foreach (Attribute att in type.GetCustomAttributes(typeof(System.Xml.Serialization.XmlTypeAttribute), true))
                {
                    typeName = ((System.Xml.Serialization.XmlTypeAttribute)att).TypeName;
                }

                if (xmldoc.DocumentElement.Name != typeName && !typeName.Contains("[]"))
                {
                    XmlDocument xmldoc2 = new XmlDocument();

                    // Write down the XML declaration
                    XmlDeclaration xmlDeclaration = xmldoc2.CreateXmlDeclaration("1.0", "utf-16", null);

                    // Create the root element
                    XmlElement rootNode = xmldoc2.CreateElement(type.Name);
                    xmldoc2.InsertBefore(xmlDeclaration, xmldoc2.DocumentElement);
                    xmldoc2.AppendChild(rootNode);

                    xmldoc2.DocumentElement.InnerXml = xmldoc.DocumentElement.InnerXml;

                    reader = new XmlNodeReader(xmldoc2.DocumentElement);
                }

                XmlSerializer serializer = new XmlSerializer(type);
                try
                {
                    return serializer.Deserialize(reader);
                }
                catch (Exception ex)
                {
                    if (onErrorThrowException)
                        throw new Exception(string.Format("Impossible to Deserialize XML with DocumentElement '{0}' to '{1}'", xmldoc.DocumentElement.Name, type.Name), ex);

                    return System.Activator.CreateInstance(type);
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the serialized.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <returns></returns>
        public static string GetSerialized(object content)
        {
            return GetSerialized(content, false);
        }

        /// <summary>
        /// Gets the serialized.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="returnHtmlEncodedResponse">if set to <c>true</c> [return HTML encoded response].</param>
        /// <returns></returns>
        public static string GetSerialized(object content, bool returnHtmlEncodedResponse)
        {
            if (content == null) return null;

            var result = GetSerialized(content.GetType(), content);
            return WebUtility.HtmlEncode(result);
            
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string GetSerialized(System.Type type, object content)
        {
            StringWriter writer = new StringWriter();
            XmlSerializer serializer = new XmlSerializer( type );
            serializer.Serialize( writer, content );
            string xml = writer.ToString();
            return xml;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="script"></param>
        /// <returns></returns>
        public static string WrapJavaScript( string script )
        {
            return string.Format("<script type=\"text/javascript\">{0}</script>", script);
        }

        /// <summary>
        /// Adds the application path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        //public static string AddApplicationPath(string path)
        //{
        //    return AddApplicationPath(path, false);
        //}

        /// <summary>
        /// Adds the application path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="appendUrl">if set to <c>true</c> [append URL].</param>
        /// <returns></returns>
        //public static string AddApplicationPath(string path, bool appendUrl)
        //{
        //    try
        //    {
        //        string appPath = CommonConfiguration.siteRoot;

        //        HttpContext context = HttpContext.Current;
        //        if (context != null && context.Request != null)
        //        {
        //            appPath = context.Request.ApplicationPath;
        //        }
        //        string completePath; 

        //        completePath = string.Format("/{0}/{1}", appPath, path);
        //        completePath = GlobalRegularExpression.Implement.CleanRelativePathSlash.Replace(completePath, "/");

        //        if (appendUrl)
        //            //  Without the first slash from completePath
        //            return string.Format("{0}{1}", Utility.GetCurrentHost(), completePath.Substring(1));
        //        else
        //            return completePath;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(string.Format("Error at Page.AddApplicationPath '{0}': {1}", path, ex.Message));
        //    }
        //}

        /// <summary>
        /// Adds the application path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="hostUrl">The host URL.</param>
        /// <returns></returns>
        //public static string AddApplicationPath(string path, string hostUrl)
        //{
        //    try
        //    {
        //        string appPath = CommonConfiguration.siteRoot;

        //        HttpContext context = HttpContext.Current;
        //        if (context != null && context.Request != null)
        //        {
        //            appPath = context.Request.ApplicationPath;
        //        }
        //        string completePath;

        //        completePath = string.Format("/{0}/{1}", appPath, path);
        //        completePath = GlobalRegularExpression.Implement.CleanRelativePathSlash.Replace(completePath, "/");

        //        return string.Format("{0}/{1}", hostUrl, completePath);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(string.Format("Error at Page.AddApplicationPath '{0}': {1}", path, ex.Message));
        //    }
        //}

        /// <summary>
        /// Remove the application prefix path
        /// </summary>
        /// <param name="path">Complete relative path</param>
        /// <returns>Relative url without application path</returns>
        //public static string RemApplicationPath( string path )
        //{
        //    if (HttpContext.Current == null) return path;
        //    string appPath = HttpContext.Current.Request.ApplicationPath.ToLower();

        //    if (appPath == CommonConfiguration.siteRoot)
        //        return path;

        //    Regex replaceAppPath = new Regex(string.Format("^{0}", appPath));
        //    string result = path.ToLower();
        //    return replaceAppPath.Replace(result, "");
        //}

        /// <summary>
        /// Hashes the string (SHA1).
        /// </summary>
        /// <param name="textToHash">The text to hash.</param>
        /// <returns></returns>
        public static string HashString(string textToHash)
        {
            return HashStringBySHA1(textToHash, false);
        }

        /// <summary>
        /// Hashes the string by SH a1.
        /// </summary>
        /// <param name="textToHash">The text to hash.</param>
        /// <returns></returns>
        public static string HashStringBySHA1(string textToHash)
        {
            return HashStringBySHA1(textToHash, false);
        }

        /// <summary>
        /// Hashes the string by SH a1.
        /// </summary>
        /// <param name="textToHash">The text to hash.</param>
        /// <param name="base64Response">if set to <c>true</c> [base64 response].</param>
        /// <returns></returns>
        public static string HashStringBySHA1(string textToHash, bool base64Response)
        {
            SHA1CryptoServiceProvider SHA1 = new SHA1CryptoServiceProvider();
            byte[] byteValue = System.Text.Encoding.UTF8.GetBytes(textToHash);
            byte[] byteHash = SHA1.ComputeHash(byteValue);
            SHA1.Clear();

            if (base64Response)
                return Convert.ToBase64String(byteHash);
            else
            {
                return BitConverter.ToString(byteHash).Replace("-", "").ToLower();
            }
        }

        /// <summary>
        /// Hashes the string by M d5.
        /// </summary>
        /// <param name="textToHash">The text to hash.</param>
        /// <returns></returns>
        public static string HashStringByMD5(string textToHash)
        {
            return HashStringByMD5(textToHash, false);
        }

        /// <summary>
        /// Hashes the string by M d5.
        /// </summary>
        /// <param name="textToHash">The text to hash.</param>
        /// <param name="base64Response">if set to <c>true</c> the response is base</param>
        /// <returns></returns>
        public static string HashStringByMD5(string textToHash, bool base64Response)
        {

            MD5CryptoServiceProvider MD5 = new MD5CryptoServiceProvider();
            byte[] byteValue = System.Text.Encoding.UTF8.GetBytes(textToHash);
            byte[] byteHash = MD5.ComputeHash(byteValue);
            MD5.Clear();

            if (base64Response)
                return Convert.ToBase64String(byteHash);
            else
                return BitConverter.ToString(byteHash).Replace("-", "").ToLower();
        }

        /// <summary>
        /// Create a Checksum using SHA256Managed 
        /// </summary>
        /// <param name="textToHash">The text to hash.</param>
        /// <returns></returns>
        public static string CreateChecksum(string textToHash)
        {
            SHA256Managed SHA = new SHA256Managed();
            byte[] byteValue = System.Text.Encoding.UTF8.GetBytes(textToHash);
            byte[] byteHash = SHA.ComputeHash(byteValue);
            SHA.Clear();
            return Convert.ToBase64String(byteHash);
        }

        /// <summary>
        /// Convert a Wim DateTime which is based on the nl-NL culture.
        /// </summary>
        /// <param name="candidate"></param>
        /// <returns></returns>
        public static DateTime ConvertWimDateTime(object candidate)
        {
            if (candidate == null || candidate.ToString().Length == 0) return DateTime.MinValue;

            DateTime dt;
            if (DateTime.TryParse(candidate.ToString(), WimCultureInfo, System.Globalization.DateTimeStyles.None, out dt))
                return dt;

            return DateTime.MinValue;
        }

        static System.Globalization.CultureInfo WimCultureInfo
        {
            get
            {
                return new System.Globalization.CultureInfo("nl-NL");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static string ConvertToJavascriptString( string item )
        {
            if ( item == null )
                return string.Empty;
            
            return item.Replace("'", "\\'");
        }


        /// <summary>
        /// Converts to system data time.
        /// </summary>
        /// <param name="dt">The dt.</param>
        /// <returns></returns>
        internal static DateTime ConvertToSystemDataTime(DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second, 0);
        }


        /// <summary>
        /// Converts to long.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public static long ConvertToLong(object item)
        {
            if (item == null)
                return 0;

            if (item.ToString().Trim().Length == 0)
                return 0;

            string candidate = item.ToString().Trim();
            if (GlobalRegularExpression.Implement.OnlyNumeric.IsMatch(candidate))
                return long.Parse(candidate);
            return 0;
        }

        /// <summary>
        /// Converts to int nullable.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public static long? ConvertToLongNullable(object item)
        {
            if (item == null)
                return null;

            if (item.ToString().Trim().Length == 0)
                return null;

            if (GlobalRegularExpression.Implement.OnlyNumeric.IsMatch(item.ToString()))
            {
                try
                {
                    return long.Parse(item.ToString());
                }
                catch (OverflowException ex)
                {
                    throw new OverflowException(string.Format("Value '{0}' exceeds the int64 limit.", item), ex);
                }
            }
            return null;
        }

        #region Convert to int
        /// <summary>
        /// Convert to int32 (if Error then return 0)
        /// </summary>
        public static int ConvertToInt(object item)
        {
            if (item == null)
                return 0;

            if (item.ToString().Trim().Length == 0)
                return 0;

            string candidate = item.ToString().Trim();
            if (GlobalRegularExpression.Implement.OnlyNumeric.IsMatch(candidate))
            {
                if (candidate == "-") return 0;
                return int.Parse(candidate);
            }
            return 0;
        }

        /// <summary>
        /// Convert to int32 (if Error then convert to [onError])
        /// </summary>
        public static int ConvertToInt(object item, int onError)
        {
            if ( item == null || item.ToString().Trim().Length == 0 )
                return onError;

            if (item == null)
                return onError;

            if (item.ToString().Trim().Length == 0)
                return onError;

            if (GlobalRegularExpression.Implement.OnlyNumeric.IsMatch(item.ToString()))
                return int.Parse(item.ToString());

            return onError;
        }
        #endregion Convert to int

        #region Convert to int?
        /// <summary>
        /// Converts to int nullable.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public static int? ConvertToIntNullable(object item)
        {
            return ConvertToIntNullable(item, true);
        }

        /// <summary>
        /// Converts to int nullable.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="whenZeroReturnNull">if set to <c>true</c> [when zero return null].</param>
        /// <returns></returns>
        public static int? ConvertToIntNullable(object item, bool whenZeroReturnNull)
        {
            if (item == null)
                return null;

            if (item.ToString().Trim().Length == 0)
                return null;

            if (GlobalRegularExpression.Implement.OnlyNumeric.IsMatch(item.ToString()))
            {
                try
                {
                    int i = int.Parse(item.ToString());
                    if (i == 0 && whenZeroReturnNull) return null;
                    return i;
                }
                catch (OverflowException ex)
                {
                    throw new OverflowException(string.Format("Value '{0}' exceeds the int32 limit.", item), ex);
                }
            }
            return null;
        }
        #endregion Convert to int?

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static string ConvertToHtmlText(string item)
        {
            Regex rex = new Regex(@"\p{IsBasicLatin}", RegexOptions.ECMAScript);

            UnicodeEncoding encoding = new UnicodeEncoding();
            bool firstByte = true;
            string tempChar = string.Empty;
            string secondChar = string.Empty;
            string returnItem = string.Empty;

            foreach (char c in item)
            {
                if (rex.IsMatch(c.ToString()))
                {
                    returnItem += c;
                    continue;
                }
                Byte[] encoded = encoding.GetBytes(c.ToString());

                foreach (Byte b in encoded)
                {
                    if (firstByte)
                        tempChar = b.ToString("X");
                    if (!firstByte)
                    {
                        secondChar = b.ToString("X");
                        if (tempChar.Length == 1)
                            tempChar = string.Format("0{0}", tempChar);
                        if (secondChar.Length == 1)
                            secondChar = string.Format("0{0}", secondChar);

                        returnItem += string.Format("&#x{1:00}{0:00};", tempChar, secondChar);
                    }
                    if (firstByte)
                        firstByte = false;
                    else
                        firstByte = true;
                }
            }
            return returnItem;
        }

        #region ConvertToFixedLengthText
        /// <summary>
        /// 
        /// </summary>
        public static Regex char_ex = new Regex(@"^[a-z][A-Z]*$");

        /// <summary>
        /// Convert a string to a fixed size if this exceeds a maximum set length.
        /// </summary>
        /// <param name="candidate">candidate text to validate</param>
        /// <param name="maxlength">maximum length of the text</param>        
        /// <returns>a text with a maximum length</returns>
        public static string ConvertToFixedLengthText(string candidate, int maxlength)
        {
            return ConvertToFixedLengthText(candidate, maxlength, null, 0, null);
        }

        /// <summary>
        /// Convert a string to a fixed size if this exceeds a maximum set length.
        /// </summary>
        /// <param name="candidate">candidate text to validate</param>
        /// <param name="maxlength">maximum length of the text</param>
        /// <param name="additionWhenBroken">when the text is broken what should be the text addition (like ...)</param>
        /// <returns>a text with a maximum length</returns>
        public static string ConvertToFixedLengthText(string candidate, int maxlength, string additionWhenBroken)
        {
            return ConvertToFixedLengthText(candidate, maxlength, additionWhenBroken, 0, null);
        }

        /// <summary>
        /// Convert a string to a fixed size if this exceeds a maximum set length.
        /// </summary>
        /// <param name="candidate">candidate text to validate</param>
        /// <param name="maxlength">maximum length of the text</param>
        /// <param name="additionWhenBroken">when the text is broken what should be the text addition (like ...)</param>
        /// <param name="maxWordLength">Words in the text should have a maximum length</param>
        /// <param name="additionWhenLonger">When words have a longer length break it up with this value</param>
        /// <returns>a text with a maximum length</returns>
        public static string ConvertToFixedLengthText(string candidate, int maxlength, string additionWhenBroken, int maxWordLength, string additionWhenLonger)
        {
            if (candidate != null && candidate.Length > 0)
            {
                if ( maxWordLength > 0 && additionWhenLonger != null ) 
                    candidate = ConvertToFixedLengthWord(candidate, maxWordLength, additionWhenLonger);

                if (candidate.Length > maxlength)
                {
                    int innerCount = maxlength;
                    char validate = candidate[innerCount];

                    while (validate != ' ')
                    {
                        innerCount--;
                        validate = candidate[innerCount];
                        if (innerCount == 0)
                        {
                            innerCount = maxlength;
                            break;
                        }
                    }

                    //if (char_ex.IsMatch(candidate[innerCount - 1].ToString()))
                        candidate = string.Format("{0}{1}", candidate.Substring(0, innerCount), additionWhenBroken);
                    //else
                    //    candidate = candidate.Substring(0, innerCount);
                }
            }
            return candidate;
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="candidate"></param>
        /// <param name="maxWordLength"></param>
        /// <param name="additionWhenLonger"></param>
        /// <returns></returns>
        public static string ConvertToFixedLengthWord(string candidate, int maxWordLength, string additionWhenLonger)
        {
            if (candidate == null) return null;

            Regex rex = new Regex(@"(?<TEXT>\S{" + maxWordLength.ToString() + ",})", RegexOptions.IgnoreCase);
            MatchCleanup cu = new MatchCleanup();
            cu.maxWordLength = maxWordLength;
            cu.additionWhenLonger = additionWhenLonger;
            string output = rex.Replace(candidate, cu.BreakWord);
            return output;
        }

        #region [CLASS:MatchCleanup]
        internal class MatchCleanup
        {
            internal int maxWordLength;
            internal string additionWhenLonger;  
            public string BreakWord(Match m)
            {
                Regex rex = new Regex(@"(?<TEXT>\S{0," + maxWordLength.ToString() + "})", RegexOptions.IgnoreCase);

                Word word = new Word();
                word.additionWhenLonger = additionWhenLonger;

                string text = rex.Replace(m.Groups["TEXT"].Value, word.Breakup);
                if (text.Length < additionWhenLonger.Length)
                    return text;

                return text.Substring(0, text.Length - additionWhenLonger.Length);
            }

            internal class Word
            {
                internal string additionWhenLonger;
                public string Breakup(Match m)
                {
                    string text = m.Groups["TEXT"].Value;
                    if (text.Length > 0)
                        return text + additionWhenLonger;
                    return null;
                }
            }
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeStampLong"></param>
        /// <returns></returns>
        public static byte[] ConvertToTimeStamp(long timeStampLong)
        {
            byte[] timestampBytes;
            timestampBytes = new byte[8];
            for (int c = 7; c >= 0; c--)
            {
                timestampBytes[c] = (byte)timeStampLong;
                timeStampLong >>= 8;
            }
            return timestampBytes;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="timestampBytes"></param>
        /// <returns></returns>
        public static long ConvertToLong(byte[] timestampBytes)
        {
            long timestampLong;
            timestampLong = 0;
            for (int c = 0; c <= 7; c++)
            {
                timestampLong <<= 8;
                timestampLong += timestampBytes[c];
            }
            return timestampLong;
        }

        /// <summary>
        /// Convert an INT array to a comma separated value string, like 1,2,3,4.
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static string ConvertToCsvString(int[] array)
        {
            if (array == null) return null;

            StringBuilder build = new StringBuilder();
            foreach (int item in array)
            {
                if (build.Length == 0) build.Append(item.ToString());
                else
                    build.Append(string.Concat(",", item.ToString()));
            }
            return build.ToString();
        }

        /// <summary>
        /// Convert an Object array to a comma separated value string, like 1,2,3,4 or 'a', 'b', 'c'
        /// </summary>
        /// <param name="array">The array.</param>
        /// <returns></returns>
        public static string ConvertToCsvString(object[] array)
        {
            return ConvertToCsvString(array, true);
        }


        /// <summary>
        /// Convert an Object array to a comma separated value string, like 1,2,3,4 or 'a', 'b', 'c'
        /// </summary>
        /// <param name="array">The array.</param>
        /// <param name="shouldUseQuoteForStringValues">if set to <c>true</c> [should use quote for string values].</param>
        /// <returns></returns>
        public static string ConvertToCsvString(object[] array, bool shouldUseQuoteForStringValues)
        {
            if (array == null) return null;

            StringBuilder build = new StringBuilder();
            foreach (object item in array)
            {
                if (item == null) continue;

                long itemInt;
                if (IsNumeric(item, out itemInt))
                {
                    if (build.Length == 0) build.Append(itemInt.ToString());
                    else
                        build.Append(string.Concat(",", itemInt.ToString()));
                }
                else
                {
                    if (shouldUseQuoteForStringValues)
                    {
                        if (build.Length == 0) build.Append(string.Concat("'", item.ToString(), "'"));
                        else
                            build.Append(string.Concat(",'", item.ToString(), "'"));
                    }
                    else
                    {
                        if (build.Length == 0) build.Append(item.ToString());
                        else
                            build.Append(string.Concat(",", item.ToString()));
                    }
                }
            }
            return build.ToString();
        }

        #region ConvertToIntArray
        /// <summary>
        /// Convert a string array to an integer array
        /// </summary>
        /// <param name="array">string array</param>
        /// <returns>integer array</returns>
        public static int[] ConvertToIntArray(string[] array)
        {
            return ConvertToIntList(array).ToArray();
        } 
        #endregion

        #region ConvertToIntList
        /// <summary>
        /// Convert a string array to an integer array
        /// </summary>
        /// <param name="array">string array</param>
        /// <returns>
        /// integer array
        /// </returns>
        public static List<int> ConvertToIntList(string[] array)
        {
            if (array == null)
                return null;

            List<int> list = new List<int>();
            foreach (string item in array)
            {
                int intItem = ConvertToInt(item, -1);
                if (intItem != -1)
                    list.Add(intItem);
            }
            return list;
        }
        #endregion


        //  Is X ?
        #region IsGuid
        /// <summary>
        /// Check if a possible object candidate is a GUID
        /// </summary>
        /// <param name="candidate">validation object</param>
        /// <param name="output">if the candidate is a valid GUID then this is set, else this parameter outs Guid.Empty</param>
        /// <returns>Is the supplied candidate a GUID?</returns>
        public static bool IsGuid(object candidate, out System.Guid output)
        {
            if (candidate != null)
            {
                string testCandidate = candidate.ToString();

                Regex test = new Regex(Utility.GlobalRegularExpression.OnlyGUID);
                if (test.IsMatch(testCandidate))
                {
                    output = new Guid(testCandidate);
                    return true;
                }
            }

            output = Guid.Empty;
            return false;
        }

        /// <summary>
        /// Determines whether the specified candidate is GUID.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <returns>
        /// 	<c>true</c> if the specified candidate is GUID; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsGuid(object candidate)
        {
            if (candidate != null)
            {
                string testCandidate = candidate.ToString();

                Regex test = new Regex(Utility.GlobalRegularExpression.OnlyGUID);
                if (test.IsMatch(testCandidate))
                {
                    return true;
                }
            }
            return false;
        } 
        #endregion

        #region IsNumeric
        /// <summary>
        /// Check if a possible object candidate is numeric
        /// </summary>
        /// <param name="candidate">validation object</param>
        /// <param name="output">if the candidate is a valid int32 then this is set, else this parameter outs 0</param>
        /// <returns>Is the supplied candidate numeric (int32)?</returns>
        public static bool IsNumeric(object candidate, out int output)
        {
            try
            {
                if (candidate != null && candidate.ToString().Length > 0)
                {
                    string testCandidate = candidate.ToString();

                    if (_OnlyNumeric.IsMatch(testCandidate))
                    {
                        output = Convert.ToInt32(testCandidate);
                        return true;
                    }
                }
                output = 0;
                return false;
            }
            catch (Exception)
            {
                output = 0;
                return false;
            }

        }

        /// <summary>
        /// Determines whether the specified candidate is numeric.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="output">The output.</param>
        /// <returns>
        /// 	<c>true</c> if the specified candidate is numeric; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNumeric(object candidate, out long output)
        {
            try
            {
                if (candidate != null && candidate.ToString().Length > 0)
                {
                    string testCandidate = candidate.ToString();

                    if (_OnlyNumeric.IsMatch(testCandidate))
                    {
                        output = Convert.ToInt64(testCandidate);
                        return true;
                    }
                }
                output = 0;
                return false;
            }
            catch (Exception)
            {
                output = 0;
                return false;
            }
        }

        static Regex _OnlyNumeric = new Regex(Utility.GlobalRegularExpression.OnlyNumeric, RegexOptions.Compiled | RegexOptions.Multiline);
        /// <summary>
        /// Determines whether the specified candidate is numeric.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <returns>
        /// 	<c>true</c> if the specified candidate is numeric; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNumeric(object candidate)
        {
            if (candidate != null && candidate.ToString().Length > 0)
            {
                string testCandidate = candidate.ToString();

                if (_OnlyNumeric.IsMatch(testCandidate))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsStrongPassword(string candidate)
        {
            if (candidate != null && candidate.ToString().Length > 0)
            {
                string testCandidate = candidate.ToString();

                if (_StrongPassword.IsMatch(testCandidate))
                {
                    return true;
                }
            }
            return false;
        }
        static Regex _StrongPassword = new Regex("^(?=.*[A-Z])(?=.*\\W)(?=.*[0-9])(?=.*[a-z]).{8,}$", RegexOptions.Compiled);

        #endregion

        #region IsDecimal
        /// <summary>
        /// Check if a possible object candidate is decimal according to the en-US culture (f.e.: nn.nn)
        /// </summary>
        /// <param name="candidate">validation object</param>
        /// <param name="output">if the candidate is a valid decimal then this is set, else this parameter outs 0</param>
        /// <returns></returns>
        public static bool IsDecimal(object candidate, out decimal output)
        {
            if (candidate != null && candidate.ToString().Length > 0)
            {
                if (candidate.GetType() == typeof(decimal))
                {
                    System.Globalization.CultureInfo info = new System.Globalization.CultureInfo("en-US");
                    output = Convert.ToDecimal(candidate, info);
                    return true;
                }

                string testCandidate = candidate.ToString();

                Regex test = new Regex(Utility.GlobalRegularExpression.OnlyDecimal);
                if (test.IsMatch(testCandidate))
                {
                    System.Globalization.CultureInfo info = new System.Globalization.CultureInfo("en-US");
                    //  Only one comma or point is allowed by regex so the group separator can be replaced.
                    //testCandidate = testCandidate.Replace(
                    //    System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.NumberGroupSeparator,
                    //    System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator);

                    if (Decimal.TryParse(testCandidate, NumberStyles.Any, info, out output))
                    {
                        return true;
                    }
                }
            }
            output = 0;
            return false;
        }
        #endregion


        #region IsDouble
        /// <summary>
        /// Check if a possible object candidate is decimal according to the en-US culture (f.e.: nn.nn)
        /// </summary>
        /// <param name="candidate">validation object</param>
        /// <param name="output">if the candidate is a valid decimal then this is set, else this parameter outs 0</param>
        /// <returns></returns>
        public static bool IsDouble(object candidate, out double output)
        {
            if (candidate != null && candidate.ToString().Length > 0)
            {
                if (candidate.GetType() == typeof(double))
                {
                    System.Globalization.CultureInfo info = new System.Globalization.CultureInfo("en-US");
                    output = Convert.ToDouble(candidate, info);
                    return true;
                }

                string testCandidate = candidate.ToString();

                Regex test = new Regex(Utility.GlobalRegularExpression.OnlyDecimal);
                if (test.IsMatch(testCandidate))
                {
                    System.Globalization.CultureInfo info = new System.Globalization.CultureInfo("en-US");
                    //  Only one comma or point is allowed by regex so the group separator can be replaced.
                    //testCandidate = testCandidate.Replace(
                    //    System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.NumberGroupSeparator,
                    //    System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator);

                    if (Double.TryParse(testCandidate, NumberStyles.Any, info, out output))
                    {
                        return true;
                    }
                }
            }
            output = 0;
            return false;
        }
        #endregion

        #region Convert to Guid
        /// <summary>
        /// Convert to Guid (if Error then return Guid.Empty)
        /// </summary>
        public static Guid ConvertToGuid(object item)
        {
            if (item == null || string.IsNullOrEmpty(item.ToString()))
                return Guid.Empty;

            Guid candidate;
            if (IsGuid(item, out candidate))
                return candidate;
            return Guid.Empty;
        }

        /// <summary>
        /// Convert to Guid (if Error then convert to [onError])
        /// </summary>
        public static Guid ConvertToGuid(object item, Guid onError)
        {
            Guid candidate = ConvertToGuid(item);
            if (candidate != Guid.Empty) return candidate;
            return onError;
        }
        #endregion


        #region Convert to decimal
        /// <summary>
        /// Converts to (wim) decimal string (US-EN).
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public static string ConvertToDecimalString(decimal item)
        {
            if (System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.CurrencyDecimalSeparator == ",")
                return item.ToString().Replace(".", "").Replace(",", ".");
            return item.ToString().Replace(",", ".");
        }
        
        /// <summary>
        /// Convert to Decimal (if Error then return 0)
        /// </summary>
        public static decimal ConvertToDecimal(object item)
        {
            return ConvertToDecimal(item, 0);
        }

        /// <summary>
        /// Converts to decimal nullable.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public static decimal? ConvertToDecimalNullable(object item)
        {
            if (item == null)
                return null;

            if (item.ToString().Trim().Length == 0)
                return null;

            decimal dec;
            if (IsDecimal(item, out dec))
                return dec;

            return null;
        }

        /// <summary>
        /// Convert to Decimal (if Error then convert to [onError])
        /// </summary>
        public static decimal ConvertToDecimal(object item, decimal onError)
        {
            if (item == null || item.ToString().Trim().Length == 0)
                return onError;

            decimal dec;
            if (IsDecimal(item, out dec))
                return dec;
            return onError;
        }
        #endregion

        #region Convert to double
        /// <summary>
        /// Converts to (wim) decimal string (US-EN).
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public static string ConvertToDoubleString(decimal item)
        {
            if (System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.CurrencyDecimalSeparator == ",")
                return item.ToString().Replace(".", "").Replace(",", ".");
            return item.ToString().Replace(",", ".");
        }

        /// <summary>
        /// Convert to Decimal (if Error then return 0)
        /// </summary>
        public static double ConvertToDouble(object item)
        {
            return ConvertToDouble(item, 0);
        }

        /// <summary>
        /// Convert to Decimal (if Error then convert to [onError])
        /// </summary>
        public static double ConvertToDouble(object item, double onError)
        {
            if (item == null || item.ToString().Trim().Length == 0)
                return onError;

            double dec;
            if (IsDouble(item, out dec))
                return dec;
            return onError;
        }
        #endregion
	}
}