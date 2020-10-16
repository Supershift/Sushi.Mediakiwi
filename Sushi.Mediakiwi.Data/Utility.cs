using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;
using Sushi.Mediakiwi.Data;

namespace Sushi.Mediakiwi.Data
{
    public class Utility
    {
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

        public static String ShortUrlEncoding(long input)
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
                                Decimal tmp;
                                if (IsDecimal(fromPropertyValue, out tmp))
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
            if (candidate == null || candidate.ToString().Length == 0) return DateTime.MinValue;

            DateTime dt;
            if (DateTime.TryParse(candidate.ToString(), WimCultureInfo, DateTimeStyles.None, out dt))
                return dt;

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

                    if (Decimal.TryParse(testCandidate, NumberStyles.Any, info, out output))
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

                    if (Double.TryParse(testCandidate, NumberStyles.Any, info, out output))
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
                return onError;

            double dec;
            if (IsDouble(item, out dec))
                return dec;
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
        static Wim.Data.Interfaces.IConfigurationSetting m_Settings;

        public static String GetConfigurationSetting(string initialValue, string settingProperty) // [MR:24-01-2020] REVIEW BY MARC
        {
            if (m_Settings == null && !m_HasCheckedSettings)
            {
                // [MR:24-01-2020] commented this, because it opens up a can of worms..
                //if (!string.IsNullOrEmpty(CommonConfiguration.CLOUD_SETTINGS))
                //{
                //    var split = CommonConfiguration.CLOUD_SETTINGS.Split(',');
                //    m_Settings = Utility.CreateInstance(string.Format("{0}.dll", split[1].Trim()), split[0].Trim()) as Data.Interfaces.IConfigurationSetting;
                //}
            }
            if (!m_HasCheckedSettings)
                m_HasCheckedSettings = true;

            if (m_Settings != null)
                return m_Settings.GetValue(initialValue, settingProperty);

            return initialValue;
        }

        #region [MR:02-01-2020] NOT resolveable

        /* All of these could not be resolved, mostly due to missing HttpContext
         * or another Wim.Framework reference not copied to this project
         */

        ///// <summary>
        ///// Gets the icon image string.
        ///// </summary>
        ///// <param name="icon">The icon.</param>
        ///// <returns></returns>
        //public static string GetIconImageString(IconImage icon)
        //{
        //    return GetIconImageString(icon, null);
        //}

        ///// <summary>
        ///// Gets the icon image string.
        ///// </summary>
        ///// <param name="icon">The icon.</param>
        ///// <param name="tooltip">The tooltip.</param>
        ///// <returns></returns>
        //public static string GetIconImageString(IconImage icon, string tooltip)
        //{
        //    return GetIconImageString(icon, IconSize.Small, tooltip);
        //}

        ///// <summary>
        ///// Gets the icon image string.
        ///// </summary>
        ///// <param name="icon">The icon.</param>
        ///// <param name="size">The size.</param>
        ///// <param name="tooltip">The tooltip.</param>
        ///// <returns></returns>
        //public static string GetIconImageString(IconImage icon, IconSize size, string tooltip)
        //{
        //    int width = (int)size, height = (int)size;

        //    if (icon == IconImage.Yes) icon = IconImage.accept_16;
        //    else if (icon == IconImage.No) icon = IconImage.delete_16;

        //    if (icon.ToString().Contains("16")) width = 16;
        //    if (icon.ToString().Contains("80")) width = 80;
        //    if (icon.ToString().Contains("10")) width = 10;
        //    height = width;

        //    string className = null;
        //    switch (icon)
        //    {
        //        case IconImage.delete_16:
        //        case IconImage.No:
        //            className = "icon icon-times-circle-o neg";
        //            break;

        //        case IconImage.accept_16:
        //        case IconImage.Yes:
        //            className = "icon icon-check-circle-o pos";
        //            break;

        //        case IconImage.info_16:
        //            className = "flaticon solid question-1 icon green";
        //            break;

        //        case IconImage.Note:
        //            return string.Format("<img src=\"{2}{0}\" title=\"{1}\" height=\"{4}\" width=\"{3}\">",
        //                AddApplicationPath(string.Format("/repository/wim/images/icons/note_{0}.png", (int)size)), tooltip, Wim.Utility.GetCurrentHost(), width, height);
        //        case IconImage.New:
        //            return string.Format("<img src=\"{2}{0}\" title=\"{1}\" height=\"10\" width=\"21\">",
        //                AddApplicationPath(string.Format("/repository/wim/images/icons/new_10.png", (int)size)), tooltip, Wim.Utility.GetCurrentHost(), width, height);
        //        case IconImage.Rfc_Green:
        //            return string.Format("<img src=\"{2}{0}\" title=\"{1}\" height=\"10\" width=\"21\">",
        //                AddApplicationPath(string.Format("/repository/wim/images/icons/rfc_green_10.png", (int)size)), tooltip, Wim.Utility.GetCurrentHost(), width, height);
        //        case IconImage.Rfc_Orange:
        //            return string.Format("<img src=\"{2}{0}\" title=\"{1}\" height=\"10\" width=\"21\">",
        //                AddApplicationPath(string.Format("/repository/wim/images/icons/rfc_orange_10.png", (int)size)), tooltip, Wim.Utility.GetCurrentHost(), width, height);
        //        case IconImage.Rfc_Red:
        //            return string.Format("<img src=\"{2}{0}\" title=\"{1}\" height=\"10\" width=\"21\">",
        //                AddApplicationPath(string.Format("/repository/wim/images/icons/rfc_red_10.png", (int)size)), tooltip, Wim.Utility.GetCurrentHost(), width, height);
        //        case IconImage.Rfc_Purple:
        //            return string.Format("<img src=\"{2}{0}\" title=\"{1}\" height=\"10\" width=\"21\">",
        //                AddApplicationPath(string.Format("/repository/wim/images/icons/rfc_purple_10.png", (int)size)), tooltip, Wim.Utility.GetCurrentHost(), width, height);
        //        case IconImage.File:
        //            className = "flaticon solid paperclip-2 icon green";
        //            break;

        //        case IconImage.NoFile:
        //            className = "flaticon solid paperclip-2 icon red";
        //            break;
        //        default:
        //            return string.Format("<img src=\"{2}{0}\" title=\"{1}\" height=\"{4}\" width=\"{3}\">",
        //                AddApplicationPath(string.Format("/repository/wim/images/icons/{0}.png", icon.ToString())), tooltip, Wim.Utility.GetCurrentHost(), width, height);
        //    }

        //    if (!string.IsNullOrEmpty(className))
        //    {
        //        if (string.IsNullOrEmpty(tooltip))
        //            return string.Format("<figure class=\"{0}\"></figure>", className);
        //        return string.Format("<abbr title=\"{1}\"><label for=\"\" class=\"{0}\"></label></abbr>", className, tooltip);
        //    }

        //    return null;
        //}

        ///// <summary>
        ///// Gets the icon image string.
        ///// </summary>
        ///// <param name="icon">The icon.</param>
        ///// <param name="size">The size.</param>
        ///// <param name="tooltip">The tooltip.</param>
        ///// <param name="url">The URL.</param>
        ///// <returns></returns>
        //public static string GetIconImageString(IconImage icon, IconSize size, string tooltip, string url)
        //{
        //    return string.Format("<a href=\"{0}\" target=\"_blank\">{1}</a>", url, GetIconImageString(icon, size, tooltip));
        //}

        ///// <summary>
        ///// Gets the icon image string.
        ///// </summary>
        ///// <param name="icon">The icon.</param>
        ///// <param name="tooltip">The tooltip.</param>
        ///// <param name="url">The URL.</param>
        ///// <returns></returns>
        //public static string GetIconImageString(IconImage icon, string tooltip, string url)
        //{
        //    return string.Format("<a href=\"{0}\">{1}</a>", url, GetIconImageString(icon, tooltip));
        //}

        ///// <summary>
        ///// Gets the icon image string.
        ///// </summary>
        ///// <param name="icon">The icon.</param>
        ///// <param name="tooltip">The tooltip.</param>
        ///// <param name="url">The URL.</param>
        ///// <param name="text">The text.</param>
        ///// <returns></returns>
        //public static string GetIconImageString(IconImage icon, string tooltip, string url, string text)
        //{
        //    return string.Format("<a href=\"{0}\">{1} {2}</a>", url, GetIconImageString(icon, tooltip), text);
        //}

        //        /// <summary>
        //        ///
        //        /// </summary>


        //        /// <summary>
        //        /// Gets the asynchronous query properties.
        //        /// </summary>
        //        /// <returns></returns>
        //        public static ASyncQuery GetAsyncQuery()
        //        {
        //            var c = System.Web.HttpContext.Current;
        //            if (c == null)
        //                return null;

        //            bool isAsyncCall = c.Request[Wim.UI.Constants.JSON_PARAM] == "1";
        //            if (!isAsyncCall)
        //                return null;

        //            var id = c.Request.Params["async_id"];
        //            var recall_id = c.Request.Params["async_rid"];
        //            ASyncQuery q = new ASyncQuery()
        //            {
        //                Property = c.Request.Params["async"],
        //                SearchQuery = string.IsNullOrEmpty(id) ? (string.IsNullOrEmpty(recall_id) ? c.Request.Params["async_query"] : recall_id) : id,
        //                SearchType = string.IsNullOrEmpty(id) ? (string.IsNullOrEmpty(recall_id) ? ASyncQueryType.FindByText : ASyncQueryType.OnSelectCallBack) : ASyncQueryType.SelectOneByID,
        //            };
        //            return q;
        //        }

        //        /// <summary>
        //        /// Gets the safe post.
        //        /// </summary>
        //        /// <param name="name">The name.</param>
        //        /// <returns></returns>
        //        public static string GetSafePost(string name)
        //        {
        //            return GetSafePost(name, false);
        //        }

        //        /// <summary>
        //        /// Gets the safe post.
        //        /// </summary>
        //        /// <param name="name">The name.</param>
        //        /// <param name="allowHTML">if set to <c>true</c> [allow HTML].</param>
        //        /// <returns></returns>
        //        public static string GetSafePost(string name, bool allowHTML)
        //        {
        //            if (System.Web.HttpContext.Current == null)
        //                return null;

        //            var context = System.Web.HttpContext.Current;
        //            if (context.Request == null)
        //                return null;

        //            if (context.Request.Form.Count == 0)
        //                return null;

        //            if (context.Request.UrlReferrer == null)
        //                return null;

        //            if (context.Request.UrlReferrer.Host != context.Request.Url.Host)
        //                return null;

        //            if (allowHTML)
        //                return context.Request.Form[name];
        //            return context.Server.HtmlEncode(context.Request.Form[name]);
        //        }

        //        /// <summary>
        //        /// Gets the safe get.
        //        /// </summary>
        //        /// <param name="name">The name.</param>
        //        /// <returns></returns>
        //        public static string GetSafeGet(string name)
        //        {
        //            return GetSafeGet(name, false);
        //        }

        //        /// <summary>
        //        /// Gets the safe get.
        //        /// </summary>
        //        /// <param name="name">The name.</param>
        //        /// <param name="allowHTML">if set to <c>true</c> [allow HTML].</param>
        //        /// <returns></returns>
        //        public static string GetSafeGet(string name, bool allowHTML)
        //        {
        //            if (System.Web.HttpContext.Current == null)
        //                return null;

        //            var context = System.Web.HttpContext.Current;
        //            if (context.Request == null)
        //                return null;

        //            if (context.Request.QueryString.Count == 0)
        //                return null;

        //            if (allowHTML)
        //                return context.Request.QueryString[name];
        //            return context.Server.HtmlEncode(context.Request.QueryString[name]);
        //        }


        //        /// <summary>
        //        /// Gets the custom query string.
        //        /// </summary>
        //        /// <param name="keyvalues">The keyvalues.</param>
        //        /// <returns></returns>
        //        public static string GetCustomQueryString(params KeyValue[] keyvalues)
        //        {
        //            System.Web.HttpContext context = System.Web.HttpContext.Current;
        //            System.Collections.Specialized.NameValueCollection nv = context.Request.QueryString;
        //            return GetCustomQueryString(nv, keyvalues);
        //        }

        //        /// <summary>
        //        /// Writes the SQL history data.
        //        /// </summary>
        //        /// <param name="page">The page.</param>
        //        internal static void WriteSqlHistoryData(System.Web.UI.Page page)
        //        {
        //            if (!Wim.CommonConfiguration.SQL_DEBUG) return;

        //            //get the list with all queries
        //            object planArr = System.Web.HttpContext.Current.Items["wim.sqlexecutionPlan"];
        //            List<Data.DalReflection.BaseSqlEntity.SqlExecutionInformation> planList =
        //                planArr == null
        //                    ? new List<Data.DalReflection.BaseSqlEntity.SqlExecutionInformation>() : (List<Data.DalReflection.BaseSqlEntity.SqlExecutionInformation>)planArr;

        //            //calculate statistics
        //            object index = System.Web.HttpContext.Current.Items["wim.executionTime"];
        //            long start = index == null ? 0 : (long)index;
        //            double executionTime = new TimeSpan(DateTime.Now.Ticks - start).TotalSeconds;

        //            index = System.Web.HttpContext.Current.Items["wim.sqlexecutionTime"];
        //            double time = index == null ? 0 : (double)index;
        //            double code = executionTime - time;

        //            object count = System.Web.HttpContext.Current.Items["wim.sqlexecutionCount"];
        //            int spend = count == null ? 0 : (int)count;

        //            double avgQueryTime = time / spend;
        //            double max = planList.Count > 0 ? planList.Max(x => x.Time) : 0;

        //            page.Controls.Add(
        //                new LiteralControl(
        //                    string.Format("<table width=\"100%\" border=\"1\" cellspacing=\"2\" cellpadding=\"2\"><tr><td align=\"left\" style=\"background-color: white; color: black\">Execution time: {0:0.000} s - Database: {1:0.000} s (query count: {3}, avg: {4:0.000} s, max: {5:0.000} s) - Code: {2:0.000} s</td></tr>", executionTime, time, code, spend, avgQueryTime, max)));

        //            foreach (Data.DalReflection.BaseSqlEntity.SqlExecutionInformation info in planList)
        //            {
        //                string stackTrace = string.Empty;
        //                if (!string.IsNullOrWhiteSpace(info.StackTrace))
        //                    stackTrace = string.Format("<br><div onclick=\"$('.stacktrace', this).toggle()\">Stacktrace<br><div class='stacktrace' style='display: none'>{0}</div></div>", Wim.Utility.CleanLineFeed(info.StackTrace, true));

        //                string style;
        //                if (info.Time < 0.1D)
        //                    style = "\"background-color: white; color: black\"";
        //                else if (info.Time < 1D)
        //                    style = "\"background-color: Yellow; color: black\"";
        //                else
        //                    style = "\"background-color: Orange; color: black\"";

        //                page.Controls.Add(new LiteralControl(string.Format("<tr><td align=\"left\" style={7}><b>Sql execution on table '{5}:{0}' took {2:0.000} seconds (object: '{4}'):</b><br/>{1}<font color=\"red\">{3}</font>{6}</td></tr>", info.SqlTable, info.SqlText, info.Time, info.SqlWhere, info.ObjectType, info.SqlDatabase, stackTrace, style)));

        //            }
        //            page.Controls.Add(new LiteralControl("</table>"));

        //        }

        //        /// <summary>
        //        /// Writes the SQL history data to a stream
        //        /// </summary>
        //        /// <param name="page">The page.</param>
        //        public static void WriteSqlHistoryData(Stream stream)
        //        {
        //            if (!Wim.CommonConfiguration.SQL_DEBUG) return;

        //            using (var sw = new StreamWriter(stream, UTF8Encoding.UTF8, 1024, true))
        //            {
        //                //get the list with all queries
        //                object planArr = System.Web.HttpContext.Current.Items["wim.sqlexecutionPlan"];
        //                List<Data.DalReflection.BaseSqlEntity.SqlExecutionInformation> planList =
        //                    planArr == null
        //                        ? new List<Data.DalReflection.BaseSqlEntity.SqlExecutionInformation>() : (List<Data.DalReflection.BaseSqlEntity.SqlExecutionInformation>)planArr;

        //                //calculate statistics
        //                object index = System.Web.HttpContext.Current.Items["wim.executionTime"];
        //                long start = index == null ? 0 : (long)index;
        //                double executionTime = new TimeSpan(DateTime.Now.Ticks - start).TotalSeconds;

        //                index = System.Web.HttpContext.Current.Items["wim.sqlexecutionTime"];
        //                double time = index == null ? 0 : (double)index;
        //                double code = executionTime - time;

        //                object count = System.Web.HttpContext.Current.Items["wim.sqlexecutionCount"];
        //                int spend = count == null ? 0 : (int)count;

        //                double avgQueryTime = time / spend;
        //                double max = planList.Count > 0 ? planList.Max(x => x.Time) : 0;

        //                sw.WriteLine(string.Format("<table width=\"100%\" border=\"1\" cellspacing=\"2\" cellpadding=\"2\"><tr><td align=\"left\" style=\"background-color: white; color: black\">Execution time: {0:0.000} s - Database: {1:0.000} s (query count: {3}, avg: {4:0.000} s, max: {5:0.000} s) - Code: {2:0.000} s</td></tr>", executionTime, time, code, spend, avgQueryTime, max));

        //                foreach (Data.DalReflection.BaseSqlEntity.SqlExecutionInformation info in planList)
        //                {
        //                    string stackTrace = string.Empty;
        //                    if (!string.IsNullOrWhiteSpace(info.StackTrace))
        //                        stackTrace = string.Format("<br><div onclick=\"$('.stacktrace', this).toggle()\">Stacktrace<br><div class='stacktrace' style='display: none'>{0}</div></div>", Wim.Utility.CleanLineFeed(info.StackTrace, true));

        //                    string style;
        //                    if (info.Time < 0.1D)
        //                        style = "\"background-color: white; color: black\"";
        //                    else if (info.Time < 1D)
        //                        style = "\"background-color: Yellow; color: black\"";
        //                    else
        //                        style = "\"background-color: Orange; color: black\"";

        //                    sw.WriteLine(string.Format("<tr><td align=\"left\" style={7}><b>Sql execution on table '{5}:{0}' took {2:0.000} seconds (object: '{4}'):</b><br/>{1}<font color=\"red\">{3}</font>{6}</td></tr>", info.SqlTable, info.SqlText, info.Time, info.SqlWhere, info.ObjectType, info.SqlDatabase, stackTrace, style));

        //                }
        //                sw.WriteLine("</table>");
        //            }

        //        }

        //        internal static string GetSqlHistoryData(bool isDeveloper)
        //        {
        //            if (!Wim.CommonConfiguration.SQL_DEBUG || !isDeveloper) return null;

        //            StringBuilder build = new StringBuilder();
        //            object index = System.Web.HttpContext.Current.Items["wim.executionTime"];
        //            long start = index == null ? 0 : (long)index;
        //            double executionTime = new TimeSpan(DateTime.Now.Ticks - start).TotalSeconds;

        //            index = System.Web.HttpContext.Current.Items["wim.sqlexecutionTime"];
        //            double time = index == null ? 0 : (double)index;
        //            double code = executionTime - time;

        //            object count = System.Web.HttpContext.Current.Items["wim.sqlexecutionCount"];
        //            int spend = count == null ? 0 : (int)count;

        //            build.AppendFormat("<article class=\"dataBlock\"><table><thead><tr><th align=\"left\" style=\"background-color: white; color: black\">Execution time: {0} seconds - Database: {1} seconds (query count: {3}) - Code: {2} seconds</th><tr class=\"empty\"><th>&nbsp;</th></tr></thead><tbody>", executionTime, time, code, spend);

        //            object planArr = System.Web.HttpContext.Current.Items["wim.sqlexecutionPlan"];
        //            List<Data.DalReflection.BaseSqlEntity.SqlExecutionInformation> planList =
        //                planArr == null
        //                    ? new List<Data.DalReflection.BaseSqlEntity.SqlExecutionInformation>() : (List<Data.DalReflection.BaseSqlEntity.SqlExecutionInformation>)planArr;

        //            foreach (Data.DalReflection.BaseSqlEntity.SqlExecutionInformation info in planList)
        //            {
        //                if (info.IsCode)
        //                    build.AppendFormat("<tr><td align=\"left\" style=\"background-color: Green; color: black\"><b>Event '{1}' took {2} seconds (object: '{4}'):</b><br/>{1}{3}</td></tr>", info.SqlTable, info.SqlText, info.Time, info.SqlWhere, info.ObjectType, info.SqlDatabase);
        //                else if (info.Time < 0.1D)
        //                    build.AppendFormat("<tr><td align=\"left\" style=\"background-color: white; color: black\"><b>Sql execution on table '{5}:{0}' took {2} seconds (object: '{4}'):</b><br/>{1}<font color=\"red\">{3}</font></td></tr>", info.SqlTable, info.SqlText, info.Time, info.SqlWhere, info.ObjectType, info.SqlDatabase);
        //                else if (info.Time < 1D)
        //                    build.AppendFormat("<tr><td align=\"left\" style=\"background-color: Yellow; color: black\"><b>Sql execution on table '{5}:{0}' took {2} seconds (object: '{4}'):</b><br/>{1}<font color=\"red\">{3}</font></td></tr>", info.SqlTable, info.SqlText, info.Time, info.SqlWhere, info.ObjectType, info.SqlDatabase);
        //                else
        //                    build.AppendFormat("<tr><td align=\"left\" style=\"background-color: Orange; color: black\"><b>Sql execution on table '{5}:{0}' took {2} seconds (object: '{4}'):</b><br/>{1}<font color=\"red\">{3}</font></td></tr>", info.SqlTable, info.SqlText, info.Time, info.SqlWhere, info.ObjectType, info.SqlDatabase);
        //            }
        //            build.AppendFormat("</tbody></table></article>");
        //            return build.ToString();
        //        }

        //        /// <summary>
        //        /// Reflects the property.
        //        /// </summary>
        //        /// <param name="rowFrom">The row from.</param>
        //        /// <param name="propertyContainerTo">The property container to.</param>
        //        public static void ReflectDataRowProperty(System.Data.DataRow rowFrom, object propertyContainerTo)
        //        {
        //            System.Reflection.PropertyInfo[] propertiesTo = propertyContainerTo.GetType().GetProperties();
        //            foreach (System.Reflection.PropertyInfo to in propertiesTo)
        //            {
        //                Data.DalReflection.DatabaseColumnAttribute[] attr = to.GetCustomAttributes(typeof(Data.DalReflection.DatabaseColumnAttribute), true) as Data.DalReflection.DatabaseColumnAttribute[];
        //                if (attr != null && attr.Length == 1)
        //                {
        //                    if (!rowFrom.Table.Columns.Contains(attr[0].Column))
        //                        continue;

        //                    if (rowFrom[attr[0].Column] == System.DBNull.Value)
        //                        continue;

        //                    if (to.PropertyType == typeof(Guid))
        //                        to.SetValue(propertyContainerTo, Wim.Utility.ConvertToGuid(rowFrom[attr[0].Column]), null);

        //                    else if (to.PropertyType == typeof(String))
        //                        to.SetValue(propertyContainerTo, rowFrom[attr[0].Column].ToString(), null);

        //                    else if (to.PropertyType == typeof(Data.CustomData))
        //                    {
        //                        string tmp = rowFrom[attr[0].Column].ToString();

        //                        Data.CustomData candidate = new Data.CustomData();
        //                        candidate.ApplySerialized(tmp);

        //                        to.SetValue(propertyContainerTo, candidate, null);
        //                    }
        //                    else
        //                        to.SetValue(propertyContainerTo, rowFrom[attr[0].Column], null);
        //                }
        //            }
        //        }

        //        /// <summary>
        //        /// Gets the current host (including port number if other then 80). f.e. http://www.wimserver.com
        //        /// </summary>
        //        /// <returns></returns>
        //        /// <value>The current host.</value>
        //        public static string GetCurrentHost()
        //        {
        //            return GetCurrentHost(false);
        //        }

        //        public static string GetSafeUrl(HttpRequest request)
        //        {
        //            if (request.QueryString.Count == 0)
        //                return $"{request.Path}";
        //            return $"{request.Path}?{request.QueryString}";
        //        }

        //        public static string GetCurrentHost(bool applyApplicationPath, bool removeLastForslash = false)
        //        {
        //            if (!string.IsNullOrEmpty(Wim.CommonConfiguration.LOCAL_REQUEST_URL))
        //                return Wim.CommonConfiguration.LOCAL_REQUEST_URL;

        //            string portInfo = null;

        //            if (HttpContext.Current == null)
        //                return null;
        //            //throw new Exception("No host detected as of missing HttpContext.Current");

        //            if (HttpContext.Current.Request.Url.Port != 80 && HttpContext.Current.Request.Url.Port != 443)
        //                portInfo = string.Concat(":", HttpContext.Current.Request.Url.Port);

        //            string extra = "/";
        //            if (applyApplicationPath)
        //                extra = HttpContext.Current.Request.ApplicationPath;

        //            var host = HttpContext.Current.Request.Url.Host;
        //            if (host.Equals("127.0.0.1"))
        //                host = "localhost";

        //            var uri = string.Concat(HttpContext.Current.Request.Url.Scheme, "://", host, portInfo, extra);
        //            if (removeLastForslash)
        //            {
        //                if (uri.EndsWith("/"))
        //                    uri = uri.Substring(0, uri.Length - 1);
        //            }
        //            return uri;
        //        }

        //        /// <summary>
        //        /// Applies the richtext links.
        //        /// </summary>
        //        /// <param name="site">The site.</param>
        //        /// <param name="text">The text.</param>
        //        /// <returns></returns>
        //        public static string ApplyRichtextLinks(Site site, string text)
        //        {
        //            return ApplyRichtextLinks(site, text, null);
        //        }

        //        /// <summary>
        //        /// Apply correct hyperlinks to richtext fields.
        //        /// </summary>
        //        /// <param name="site"></param>
        //        /// <param name="text"></param>
        //        /// <returns></returns>
        //        public static string ApplyRichtextLinks(Site site, string text, string databaseMap)
        //        {
        //            if (text == null) return null;
        //            Framework.Templates.RichLink rlink = new Framework.Templates.RichLink();
        //            rlink.Site = site;
        //            rlink.DatabaseMap = databaseMap;

        //            string candidate = Wim.Framework.Templates.RichLink.GetCleaner.Replace(text.ToString(), rlink.CleanLinkData);

        //            //  Introduced for new richtext editor.
        //            candidate = Wim.Framework.Templates.RichLink.GetCleaner2.Replace(candidate, rlink.CleanLinkData);

        //            Framework.RichRext.AgilityCleaner cleaner = new Framework.RichRext.AgilityCleaner();
        //            candidate = cleaner.CleanAssetUrl(candidate);
        //            return candidate;
        //        }

        //        public static string CleanLink(string text)
        //        {
        //            if (text == null) return null;
        //            Framework.Templates.RichLink rlink = new Framework.Templates.RichLink();
        //            string candidate = Wim.Framework.Templates.RichLink.GetCleaner.Replace(text.ToString(), rlink.CleanEmptyLink);

        //            //  Introduced for new richtext editor.
        //            candidate = Wim.Framework.Templates.RichLink.GetCleaner2.Replace(candidate, rlink.CleanEmptyLink);

        //            return candidate;
        //        }

        //        /// <summary>
        //        /// Applies the richtext.
        //        /// </summary>
        //        /// <param name="site">The site.</param>
        //        /// <param name="text">The text.</param>
        //        /// <returns></returns>
        //        public static string ApplyRichtext(Data.Site site, string text)
        //        {
        //            return ApplyRichtext(site, text, null);
        //        }

        //        /// <summary>
        //        /// Applies the richtext cleanup (tables) and also calls ApplyRichtextLinks.
        //        /// </summary>
        //        /// <param name="site">The site.</param>
        //        /// <param name="text">The text.</param>
        //        /// <returns></returns>
        //        public static string ApplyRichtext(Data.Site site, string text, string databaseMap)
        //        {
        //            if (text == null) return null;
        //            text = ApplyRichtextLinks(site, text, databaseMap);

        //            Framework.ContentInfoItem.RichTextCleanup prepare = new Framework.ContentInfoItem.RichTextCleanup();
        //            return prepare.Clean(text);

        //            //Wim.Framework.RichTextPrepare prepare = new Wim.Framework.RichTextPrepare();
        //            //return prepare.Clean(text);
        //        }

        //        /// <summary>
        //        /// Gets the instance list collection.
        //        /// </summary>
        //        /// <param name="list">The list.</param>
        //        /// <param name="listName">Name of the list.</param>
        //        /// <returns></returns>
        //        public static System.Web.UI.WebControls.ListItemCollection GetInstanceListCollection(Data.IComponentList list, string listName)
        //        {
        //            return GetInstanceListCollection(list, listName, null);
        //        }

        //        /// <summary>
        //        /// Gets the instance list collection.
        //        /// </summary>
        //        /// <param name="site">The site.</param>
        //        /// <param name="list">The list.</param>
        //        /// <param name="listName">Name of the list.</param>
        //        /// <returns></returns>
        //        public static System.Web.UI.WebControls.ListItemCollection GetInstanceListCollection(Data.IComponentList list, string listName, Data.Site site)
        //        {
        //            Framework.IComponentListTemplate instance = list.GetInstance();
        //            return GetInstanceListCollection(list, listName, site, instance);
        //        }

        //        /// <summary>
        //        /// Gets the instance list collection.
        //        /// </summary>
        //        /// <param name="list">The list.</param>
        //        /// <param name="listName">Name of the list.</param>
        //        /// <param name="site">The site.</param>
        //        /// <param name="instance">The instance.</param>
        //        /// <returns></returns>
        //        public static System.Web.UI.WebControls.ListItemCollection GetInstanceListCollection(Data.IComponentList list, string listName, Data.Site site, Framework.IComponentListTemplate instance)
        //        {
        //            instance.wim.CurrentList = list;
        //            instance.wim.CurrentSite = site;

        //            Type type = instance.GetType();

        //            if (listName.Contains(":"))
        //            {
        //                string[] split = listName.Split(':');
        //                System.Reflection.MethodInfo method = type.GetMethod(split[0]);
        //                object[] index = new object[1] { Convert.ToInt32(split[1]) };

        //                if (method == null)
        //                    return new System.Web.UI.WebControls.ListItemCollection();

        //                return method.Invoke(instance, index) as System.Web.UI.WebControls.ListItemCollection;

        //            }

        //            System.Reflection.PropertyInfo info = type.GetProperty(listName);
        //            if (info == null)
        //                return new System.Web.UI.WebControls.ListItemCollection();

        //            return info.GetValue(instance, null) as System.Web.UI.WebControls.ListItemCollection;
        //        }

        //        /// <summary>
        //        /// Gets the instance options.
        //        /// </summary>
        //        /// <param name="assembly">The assembly.</param>
        //        /// <param name="typeName">Name of the type.</param>
        //        /// <returns></returns>
        //        public static Framework.iOption GetInstanceOptions(string assembly, string typeName)
        //        {
        //            Type type = System.Type.GetType(typeName);
        //            return CreateInstance(assembly, typeName) as Framework.iOption;
        //        }

        //        /// <summary>
        //        /// Creates the instance.
        //        /// </summary>
        //        /// <param name="assemblyName">Name of the assembly.</param>
        //        /// <param name="className">Name of the class.</param>
        //        /// <returns></returns>
        //        public static Object CreateInstance(string assemblyName, string className)
        //        {
        //            Type type;
        //            return CreateInstance(assemblyName, className, out type);
        //        }

        //        /// <summary>
        //        /// Creates the instance.
        //        /// </summary>
        //        /// <param name="list">The list.</param>
        //        /// <returns></returns>
        //        public static Object CreateInstance(IComponentList list)
        //        {
        //            Type type;
        //            return CreateInstance(list.AssemblyName, list.ClassName, out type);
        //        }

        //        /// <summary>
        //        /// Creates the instance.
        //        /// </summary>
        //        /// <param name="assemblyName">Name of the assembly.</param>
        //        /// <param name="className">Name of the class.</param>
        //        /// <param name="type">The type.</param>
        //        /// <returns></returns>
        //        public static Object CreateInstance(string assemblyName, string className, out Type type)
        //        {
        //            return CreateInstance(assemblyName, className, out type, true);
        //        }

        //        /// <summary>
        //        /// Creates the instance.
        //        /// </summary>
        //        /// <param name="assemblyName">Name of the assembly.</param>
        //        /// <param name="className">Name of the class.</param>
        //        /// <param name="type">The type.</param>
        //        /// <param name="onExceptionThrow">if set to <c>true</c> [on exception throw].</param>
        //        /// <returns></returns>
        //        public static Object CreateInstance(string assemblyName, string className, out Type type, bool onExceptionThrow)
        //        {
        //            type = null;
        //            if (string.IsNullOrEmpty(assemblyName) || string.IsNullOrEmpty(className))
        //            {
        //                if (onExceptionThrow)
        //                {
        //                    Wim.Utilities.CacheItemManager.FlushIndexOfCacheObjects("Data.wim_ComponentLists");
        //                    throw new Exception("No assemblyFile or classname provided!");
        //                    return null;
        //                }
        //            }
        //            if (assemblyName.Equals("wim.appcentre.dll", StringComparison.CurrentCultureIgnoreCase))
        //                assemblyName = "wim.Framework.dll";

        //            try
        //            {
        //                FileInfo nfo = null;

        //                if (HttpContext.Current == null)
        //                {
        //                    // [20130704:MR/DV] This check is added for when this instance is approached via a seperate (possible non-web) thread
        //                    // Such as a Fire-and-forget task.
        //                    if (System.Web.Hosting.HostingEnvironment.IsHosted)
        //                    {
        //                        nfo = new FileInfo(
        //                            System.Web.Hosting.HostingEnvironment.MapPath(string.Concat("~/bin/", assemblyName)));
        //                    }
        //                    else
        //                    {
        //                        nfo = new FileInfo(
        //                        string.Concat(
        //                            Assembly.GetCallingAssembly().Location.Replace(string.Concat(Assembly.GetCallingAssembly().GetName().Name, ".dll"), string.Empty)
        //                            , assemblyName));
        //                    }
        //                }
        //                else
        //                {
        //                    if (System.Web.Hosting.HostingEnvironment.IsHosted)
        //                    {
        //                        nfo = new FileInfo(
        //                            System.Web.Hosting.HostingEnvironment.MapPath(string.Concat("~/bin/", assemblyName)));
        //                    }
        //                    else
        //                    {
        //                        nfo = new FileInfo(
        //                            HttpContext.Current.Server.MapPath(
        //                                string.Concat(HttpContext.Current.Request.ApplicationPath, "/bin/", assemblyName)));
        //                    }
        //                }
        //                if (nfo == null)
        //                {
        //                    Wim.Utilities.CacheItemManager.FlushIndexOfCacheObjects("Data.wim_ComponentLists");
        //                    throw new Exception(string.Format("Could not find the assemblyFile[{0}] or the provided classname[{1}]!", assemblyName, className));
        //                }

        //                Assembly assem = Assembly.LoadFrom(nfo.FullName);
        //                type = assem.GetType(className);

        //                return System.Activator.CreateInstance(type);
        //            }
        //            catch (Exception ex)
        //            {
        //                if (ex.InnerException != null && ex.InnerException.GetType() == typeof(UnauthorizedAccessException))
        //                {
        //                    //Wim.Utilities.CacheItemManager.FlushIndexOfCacheObjects("Data");
        //                    throw ex.InnerException;
        //                    return null;
        //                }
        //                if (onExceptionThrow)
        //                {
        //                    //Wim.Utilities.CacheItemManager.FlushIndexOfCacheObjects("Data");
        //                    throw new Exception(string.Format("Could not initiate the requested type[{0}]!", className), ex);
        //                }
        //                return null;
        //            }
        //        }

        //        /// <summary>
        //        ///
        //        /// </summary>
        //        /// <returns></returns>
        //        public static string GetHtmlFormattedLastServerError()
        //        {
        //            HttpContext context = HttpContext.Current;
        //            Exception ex = context.Server.GetLastError();
        //            return GetHtmlFormattedLastServerError(ex);
        //        }

        //        /// <summary>
        //        ///
        //        /// </summary>
        //        /// <param name="ex"></param>
        //        /// <returns></returns>
        //        public static string GetHtmlFormattedLastServerError(Exception ex)
        //        {
        //            string error = "";
        //            while (ex != null)
        //            {
        //                string stackTrace = ex.StackTrace == null ? "" : ex.StackTrace;
        //                Regex cleanUpStrackTrace = new Regex("( at)");

        //                stackTrace = cleanUpStrackTrace.Replace(stackTrace, "<br/>at");

        //                if (error != "")
        //                    error += "<br/><br/>";
        //                if (CommonConfiguration.IS_LOCAL_DEVELOPMENT)
        //                {
        //                    error += string.Format(@"<b>Error:</b><br/>{0}
        //<br/><br/><b>Source:</b><br/>{1}
        //<br/><br/><b>Method:</b><br/>{2}
        //<br/><br/><b>Stacktrace:</b>{3}
        //"
        //                        , ex.Message, ex.Source, ex.TargetSite, stackTrace);
        //                }
        //                else
        //                {
        //                    error += string.Format(@"<b>Error:</b><br/>{0}"
        //                        , ex.Message);
        //                }
        //                ex = ex.InnerException;
        //            }
        //            return error;
        //        }

        /// <summary>
        /// Adds the application path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public static string AddApplicationPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return path;

            if (path.StartsWith("/") == true)
                path = path.Substring(1);

            if (path.Contains("/") == true)
                path = path.Replace("/", "\\");

            var exePath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase);
            Regex appPathMatcher = new Regex(@"(?<!fil)[A-Za-z]:\\+[\S\s]*?(?=\\+bin)");
            var appRoot = appPathMatcher.Match(exePath).Value;

            return Path.Combine(appRoot, path);
        }

        //        /// <summary>
        //        /// Adds the application path.
        //        /// </summary>
        //        /// <param name="path">The path.</param>
        //        /// <param name="appendUrl">if set to <c>true</c> [append URL].</param>
        //        /// <returns></returns>
        //        public static string AddApplicationPath(string path, bool appendUrl)
        //        {
        //            try
        //            {
        //                string appPath = CommonConfiguration.siteRoot;

        //                HttpContext context = HttpContext.Current;
        //                if (context != null && context.Request != null)
        //                {
        //                    appPath = context.Request.ApplicationPath;
        //                }
        //                string completePath;

        //                completePath = string.Format("/{0}/{1}", appPath, path);
        //                completePath = GlobalRegularExpression.Implement.CleanRelativePathSlash.Replace(completePath, "/");

        //                if (appendUrl)
        //                    //  Without the first slash from completePath
        //                    return string.Format("{0}{1}", Wim.Utility.GetCurrentHost(), completePath.Substring(1));
        //                else
        //                    return completePath;
        //            }
        //            catch (Exception ex)
        //            {
        //                throw new Exception(string.Format("Error at Page.AddApplicationPath '{0}': {1}", path, ex.Message));
        //            }
        //        }

        //        /// <summary>
        //        /// Adds the application path.
        //        /// </summary>
        //        /// <param name="path">The path.</param>
        //        /// <param name="hostUrl">The host URL.</param>
        //        /// <returns></returns>
        //        public static string AddApplicationPath(string path, string hostUrl)
        //        {
        //            try
        //            {
        //                string appPath = CommonConfiguration.siteRoot;

        //                HttpContext context = HttpContext.Current;
        //                if (context != null && context.Request != null)
        //                {
        //                    appPath = context.Request.ApplicationPath;
        //                }
        //                string completePath;

        //                completePath = string.Format("/{0}/{1}", appPath, path);
        //                completePath = GlobalRegularExpression.Implement.CleanRelativePathSlash.Replace(completePath, "/");

        //                return string.Format("{0}/{1}", hostUrl, completePath);
        //            }
        //            catch (Exception ex)
        //            {
        //                throw new Exception(string.Format("Error at Page.AddApplicationPath '{0}': {1}", path, ex.Message));
        //            }
        //        }

        //        /// <summary>
        //        /// Remove the application prefix path
        //        /// </summary>
        //        /// <param name="path">Complete relative path</param>
        //        /// <returns>Relative url without application path</returns>
        //        public static string RemApplicationPath(string path)
        //        {
        //            if (HttpContext.Current == null) return path;
        //            string appPath = HttpContext.Current.Request.ApplicationPath.ToLower();

        //            if (appPath == CommonConfiguration.siteRoot)
        //                return path;

        //            Regex replaceAppPath = new Regex(string.Format("^{0}", appPath));
        //            string result = path.ToLower();
        //            return replaceAppPath.Replace(result, "");
        //        }

        //        /// <summary>
        //        /// Gets the serialized.
        //        /// </summary>
        //        /// <param name="content">The content.</param>
        //        /// <returns></returns>
        //        public static string GetSerialized(object content)
        //        {
        //            return GetSerialized(content, false);
        //        }

        //        /// <summary>
        //        /// Gets the serialized.
        //        /// </summary>
        //        /// <param name="content">The content.</param>
        //        /// <param name="returnHtmlEncodedResponse">if set to <c>true</c> [return HTML encoded response].</param>
        //        /// <returns></returns>
        //        public static string GetSerialized(object content, bool returnHtmlEncodedResponse)
        //        {
        //            if (content == null) return null;

        //            var result = GetSerialized(content.GetType(), content);

        //            if (returnHtmlEncodedResponse && System.Web.HttpContext.Current != null)
        //                return System.Web.HttpContext.Current.Server.HtmlEncode(result);

        //            return result;
        //        }

        #endregion [MR:02-01-2020] NOT resolveable
    }
}