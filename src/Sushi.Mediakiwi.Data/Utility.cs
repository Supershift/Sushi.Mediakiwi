using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;

[assembly: InternalsVisibleTo("Sushi.Mediakiwi")]

namespace Sushi.Mediakiwi.Data
{
    public static class Utility
    {
        public static string ConvertUrl(string path)
        {
            return path
                .ToLowerInvariant()
                .Replace(" ", "-");
        }

        public static string GetHtmlFormattedLastServerError(Exception ex)
        {
            string error = "";
            while (ex != null)
            {
                string stackTrace = ex.StackTrace == null ? "" : ex.StackTrace;
                Regex cleanUpStrackTrace = new Regex("( at)");

                stackTrace = cleanUpStrackTrace.Replace(stackTrace, "<br/>at");

                if (error != "")
                    error += "<br/><br/>";
             
                error += string.Format(@"<b>Error:</b><br/>{0}
<br/><br/><b>Source:</b><br/>{1}
<br/><br/><b>Method:</b><br/>{2}
<br/><br/><b>Stacktrace:</b>{3}
"
                        , ex.Message, ex.Source, ex.TargetSite, stackTrace);
              
                ex = ex.InnerException;
            }
            return error;
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

        public static string ShortUrlEncoding(long input)
        {
            return Base36.Encode(input);
        }

        public static long ShortUrlDecoding(string input)
        {
            return Base36.Decode(input);
        }

        private static class Base36
        {
            private const string CharList = "A1B2C3D4E5F6G7H8K9LaMbNcPdQeRfSgThVjWkXmYnZopqrstuvwxyz";

            /// <summary>
            /// Encode the given number into a Base36 string
            /// </summary>
            /// <param name="input"></param>
            /// <returns></returns>
            public static string Encode(long input)
            {
                if (input < 0) throw new ArgumentOutOfRangeException(nameof(input), input, "input cannot be negative");

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
            public static long Decode(string input)
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
        /// <param name="queryString">The query string.</param>
        /// <param name="keyvalues">The keyvalues.</param>
        /// <returns></returns>
        public static string GetCustomQueryString(string queryString, params KeyValue[] keyvalues)
        {
            System.Collections.Specialized.NameValueCollection nv = new System.Collections.Specialized.NameValueCollection();
            string[] pairs = queryString.Split('&');
            foreach (string pair in pairs)
            {
                string[] namevalue = pair.Split('=');
                nv.Add(namevalue[0], namevalue[1]);
            }
            return GetCustomQueryString(nv, keyvalues);
        }

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

            text = CleanLineFeed(text, false);

            Regex urlHttp = new Regex(@"[\s|\n](http://[^\s|<]*)", RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.Compiled);
            text = urlHttp.Replace(" " + text + " ", new MatchEvaluator(replaceA));

            Regex urlHttps = new Regex(@"[\s|\n](https://[^\s|<]*)", RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.Compiled);
            text = urlHttps.Replace(" " + text + " ", new MatchEvaluator(replaceA));

            Regex urlftp = new Regex(@"[\s|\n](ftp://[^\s|<]*)", RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.Compiled);
            text = urlftp.Replace(" " + text + " ", new MatchEvaluator(replaceA));

            Regex urlWWW = new Regex(@"[\s|\n](www[^\s|<]*)", RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.Compiled);
            text = urlWWW.Replace(" " + text + " ", new MatchEvaluator(replaceA));

            text = CleanLineFeed(text, true);

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
        private static string replaceA(Match m)
        {
            string url = m.Groups[1].Value;
            string title = url;
            string text = url;

            if (text.Length > 50)
                text = text.Substring(0, 47) + "...";

            if (!url.Contains("http://") && !url.Contains("https://") && !url.Contains("ftp://"))
                url = string.Concat("http://", url);

            return string.Format(@"{3}<a href=""{0}"" title=""{2}"">{1}</a> ", url.ToLower(), text, title, m.Value.StartsWith("\n") ? "\n" : " ");
        }

        public static string ConvertFirstToUpper(string p)
        {
            if (string.IsNullOrEmpty(p))
                return string.Empty;
            if (p.Length == 1)
                return p.ToUpper();
            return p[0].ToString().ToUpper() + p.Substring(1);
        }

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
                                to.SetValue(propertyContainerTo, ConvertToCsvString((string[])fromPropertyValue), null);
                            }
                            if (from.PropertyType == typeof(int[]))
                            {
                                //  int[] --> String
                                to.SetValue(propertyContainerTo, ConvertToCsvString((int[])fromPropertyValue), null);
                            }

                            if (from.PropertyType == typeof(SubList))
                            {
                                if (fromPropertyValue == null)
                                    to.SetValue(propertyContainerTo, null, null);
                                else
                                {
                                    //  Sublist --> String
                                    SubList candidate = (SubList)fromPropertyValue;
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
                                if (IsDecimal(fromPropertyValue, out decimal tmp))
                                {
                                    CultureInfo info = new CultureInfo("en-US");
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

                        #endregion ? --> String

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
                                to.SetValue(propertyContainerTo, ConvertToIntArray(fromPropertyValue.ToString().Split(',')), null);
                            }

                            if (to.PropertyType == typeof(SubList))
                            {
                                //  String -- > Sublist
                                if (fromPropertyValue != null && !string.IsNullOrEmpty(fromPropertyValue.ToString()))
                                {
                                    SubList candidate = SubList.GetDeserialized(fromPropertyValue.ToString());
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
                                if (IsDecimal(fromPropertyValue, out decimal tmp))
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
                                if (IsGuid(fromPropertyValue, out Guid guid))
                                {
                                    to.SetValue(propertyContainerTo, guid, null);
                                }
                            }
                        }

                        #endregion String --> ?

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

                        #endregion nullable

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

            candidate = CleanLeadingAndTrailingLineFeed(candidate);
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

            while (candidate.Contains("<br/><br/><br/>"))
            {
                candidate = candidate.Replace("<br/><br/><br/>", "<br/><br/>");
            }

            if (hasLeadingPar)
                return string.Concat("<p>", candidate, "</p>");
            return candidate;
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

            if (m_CleanUrlSpaces == null)
                m_CleanUrlSpaces = new Regex(@"\s", RegexOptions.IgnoreCase);

            string replacement = Environment.Current["SPACE_REPLACEMENT"];

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
                            m_CleanRelativePathSlash = new Regex(ReplaceRelativePathSlash);
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
        /// Gets the deserialized.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="xml">The XML.</param>
        /// <returns></returns>
        public static object GetDeserialized(Type type, string xml)
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
        public static object GetDeserialized(Type type, string xml, bool onErrorThrowException)
        {
            if (string.IsNullOrEmpty(xml)) return null;

            if (type == typeof(CustomDataItem[]) && xml.Contains("ArrayOfField"))
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
                foreach (Attribute att in type.GetCustomAttributes(typeof(XmlTypeAttribute), true))
                {
                    typeName = ((XmlTypeAttribute)att).TypeName;
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

                    return Activator.CreateInstance(type);
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="type"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string GetSerialized(Type type, object content)
        {
            StringWriter writer = new StringWriter();
            XmlSerializer serializer = new XmlSerializer(type);
            serializer.Serialize(writer, content);
            string xml = writer.ToString();
            return xml;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="script"></param>
        /// <returns></returns>
        public static string WrapJavaScript(string script)
        {
            return string.Format("<script type=\"text/javascript\">{0}</script>", script);
        }

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
            byte[] byteValue = Encoding.UTF8.GetBytes(textToHash);
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
            byte[] byteValue = Encoding.UTF8.GetBytes(textToHash);
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
            byte[] byteValue = Encoding.UTF8.GetBytes(textToHash);
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
            if (candidate == null || candidate.ToString().Length == 0)
            {
                return DateTime.MinValue;
            }

            if (DateTime.TryParse(candidate.ToString(), WimCultureInfo, DateTimeStyles.None, out DateTime dt))
            {
                return dt;
            }

            return DateTime.MinValue;
        }

        private static CultureInfo WimCultureInfo
        {
            get
            {
                return new CultureInfo("nl-NL");
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static string ConvertToJavascriptString(string item)
        {
            if (item == null)
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
            if (item == null || item.ToString().Trim().Length == 0)
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
                byte[] encoded = encoding.GetBytes(c.ToString());

                foreach (byte b in encoded)
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
                if (maxWordLength > 0 && additionWhenLonger != null)
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

        #endregion ConvertToFixedLengthText

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

                Word word = new Word()
                {
                    additionWhenLonger = additionWhenLonger
                };

                string text = rex.Replace(m.Groups["TEXT"].Value, word.Breakup);
                if (text.Length < additionWhenLonger.Length)
                {
                    return text;
                }

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

        #endregion [CLASS:MatchCleanup]

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

                if (IsNumeric(item, out long itemInt))
                {
                    if (build.Length == 0)
                    {
                        build.Append(itemInt.ToString());
                    }
                    else
                    {
                        build.Append(string.Concat(",", itemInt.ToString()));
                    }
                }
                else
                {
                    if (shouldUseQuoteForStringValues)
                    {
                        if (build.Length == 0)
                        {
                            build.Append(string.Concat("'", item.ToString(), "'"));
                        }
                        else
                        {
                            build.Append(string.Concat(",'", item.ToString(), "'"));
                        }
                    }
                    else
                    {
                        if (build.Length == 0)
                        {
                            build.Append(item.ToString());
                        }
                        else
                        {
                            build.Append(string.Concat(",", item.ToString()));
                        }
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

        #endregion ConvertToIntArray

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

        #endregion ConvertToIntList

        //  Is X ?

        #region IsGuid

        /// <summary>
        /// Check if a possible object candidate is a GUID
        /// </summary>
        /// <param name="candidate">validation object</param>
        /// <param name="output">if the candidate is a valid GUID then this is set, else this parameter outs Guid.Empty</param>
        /// <returns>Is the supplied candidate a GUID?</returns>
        public static bool IsGuid(object candidate, out Guid output)
        {
            if (candidate != null)
            {
                string testCandidate = candidate.ToString();

                Regex test = new Regex(GlobalRegularExpression.OnlyGUID);
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

                Regex test = new Regex(GlobalRegularExpression.OnlyGUID);
                if (test.IsMatch(testCandidate))
                {
                    return true;
                }
            }
            return false;
        }

        #endregion IsGuid

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

        private static Regex _OnlyNumeric = new Regex(GlobalRegularExpression.OnlyNumeric, RegexOptions.Compiled | RegexOptions.Multiline);

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

        private static Regex _StrongPassword = new Regex("^(?=.*[A-Z])(?=.*\\W)(?=.*[0-9])(?=.*[a-z]).{8,}$", RegexOptions.Compiled);

        #endregion IsNumeric

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
                    CultureInfo info = new CultureInfo("en-US");
                    output = Convert.ToDecimal(candidate, info);
                    return true;
                }

                string testCandidate = candidate.ToString();

                Regex test = new Regex(GlobalRegularExpression.OnlyDecimal);
                if (test.IsMatch(testCandidate))
                {
                    CultureInfo info = new CultureInfo("en-US");
                    //  Only one comma or point is allowed by regex so the group separator can be replaced.
                    //testCandidate = testCandidate.Replace(
                    //    System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.NumberGroupSeparator,
                    //    System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator);

                    if (decimal.TryParse(testCandidate, NumberStyles.Any, info, out output))
                    {
                        return true;
                    }
                }
            }
            output = 0;
            return false;
        }

        #endregion IsDecimal

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
                    CultureInfo info = new CultureInfo("en-US");
                    output = Convert.ToDouble(candidate, info);
                    return true;
                }

                string testCandidate = candidate.ToString();

                Regex test = new Regex(GlobalRegularExpression.OnlyDecimal);
                if (test.IsMatch(testCandidate))
                {
                    CultureInfo info = new CultureInfo("en-US");
                    //  Only one comma or point is allowed by regex so the group separator can be replaced.
                    //testCandidate = testCandidate.Replace(
                    //    System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.NumberGroupSeparator,
                    //    System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator);

                    if (double.TryParse(testCandidate, NumberStyles.Any, info, out output))
                    {
                        return true;
                    }
                }
            }
            output = 0;
            return false;
        }

        #endregion IsDouble

        #region Convert to Guid

        /// <summary>
        /// Convert to Guid (if Error then return Guid.Empty)
        /// </summary>
        public static Guid ConvertToGuid(object item)
        {
            if (item == null || string.IsNullOrEmpty(item.ToString()))
            {
                return Guid.Empty;
            }

            if (IsGuid(item, out Guid candidate))
            {
                return candidate;
            }

            return Guid.Empty;
        }

        /// <summary>
        /// Convert to Guid (if Error then convert to [onError])
        /// </summary>
        public static Guid ConvertToGuid(object item, Guid onError)
        {
            Guid candidate = ConvertToGuid(item);
            if (candidate != Guid.Empty)
            {
                return candidate;
            }
            return onError;
        }

        #endregion Convert to Guid

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
            {
                return null;
            }

            if (item.ToString().Trim().Length == 0)
            {
                return null;
            }

            if (IsDecimal(item, out decimal dec))
            {
                return dec;
            }

            return null;
        }

        /// <summary>
        /// Convert to Decimal (if Error then convert to [onError])
        /// </summary>
        public static decimal ConvertToDecimal(object item, decimal onError)
        {
            if (item == null || item.ToString().Trim().Length == 0)
            {
                return onError;
            }

            if (IsDecimal(item, out decimal dec))
            {
                return dec;
            }

            return onError;
        }

        #endregion Convert to decimal

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
            {
                return onError;
            }

            if (IsDouble(item, out double dec))
            {
                return dec;
            }

            return onError;
        }

        #endregion Convert to double

        /// <summary>
        /// Gets the serialized.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <returns></returns>
        public static string GetSerialized(object content)
        {
            return GetSerialized(content.GetType(), content);
        }
        
        static bool m_HasCheckedSettings;
        static Sushi.Mediakiwi.Data.Interfaces.IConfigurationSetting m_Settings;

        public static string GetConfigurationSetting(string initialValue, string settingProperty)
        {
            if (!m_HasCheckedSettings)
                m_HasCheckedSettings = true;

            if (m_Settings != null)
                return m_Settings.GetValue(initialValue, settingProperty);

            return initialValue;
        }
    }
}