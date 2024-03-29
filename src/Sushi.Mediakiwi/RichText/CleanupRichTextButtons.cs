﻿using System.Text.RegularExpressions;

namespace Sushi.Mediakiwi.RichRext
{
    /// <summary>
    /// Provides methods to clean HTML generated by the browser when the richtext styling buttons are used.
    /// </summary>
    public class CleanupRichTextButtons : BaseCleaner
    {
        /// <summary>
        /// Replaces all posibly created wrong tags by the Richtext's styling buttons with correct tags.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public string ApplyFullClean(string input)
        {
            string result = ApplyStrongTags(input);
            result = ApplyItalic(result);
            result = ApplyUnderline(result);
            result = ApplyDelTags(result);
            result = ApplyBrTags(result);
            result = ApplyHrTags(result);
            result = new Regex(@"<(?=(p )).*?>", RegexOptions.IgnoreCase).Replace(result, "<p>");
            return result;
        }


        private static Regex _tagsToStrong;
        private static Regex tagsToStrong
        {
            get
            {
                if (_tagsToStrong == null)
                    _tagsToStrong = new Regex(@"<b>(.*?)</b>|<span\sstyle=""font-weight:\s*?bold;?\s*?"">(.*?)</span>", DefaultOptions | RegexOptions.Compiled);
                return _tagsToStrong;
            }
        }

        /// <summary>
        /// Replaces strong-tags and span font-weight:bold tags with a b tag
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public string ApplyStrongTags(string input)
        {
            string replacement = "<strong>$1$2</strong>";                        
            string result = tagsToStrong.Replace(input, replacement);
            return result;
        }

        private static Regex _tagsToB;
        private static Regex tagsToB
        {
            get
            {
                if (_tagsToB == null)
                    _tagsToB = new Regex(@"<strong>(.*?)</strong>|<span\sstyle=""font-weight:\s*?bold;?\s*?"">(.*?)</span>", DefaultOptions | RegexOptions.Compiled);
                return _tagsToB;
            }
        }

        /// <summary>
        /// Replaces strong tags with b tags, use this when displaying the editor box in Wim.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public string ApplyBold(string input)
        {            
            string replacement = "<b>$1$2</b>";            
            string result = tagsToB.Replace(input, replacement);
            return result;
        }

        private static Regex _tagsToI;
        private static Regex tagsToI
        {
            get
            {
                if (_tagsToI == null)
                    _tagsToI = new Regex(@"<i>(.*?)</i>|<blockquote>(.*?)</blockquote>|<span\sstyle=""font-style:\s*?italic;?\s*?"">(.*?)</span>|<span\sstyle=""font-style:\s*?oblique;?\s*?"">(.*?)</span>", DefaultOptions | RegexOptions.Compiled);
                return _tagsToI;
            }
        }
        /// <summary>
        /// Replaces i, blockquote, font-style italic and font-style oblique tags with em tag
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public string ApplyItalic(string input)
        {
            string replacement = "<em>$1$2$3$4</em>";
            string result = tagsToI.Replace(input, replacement);            
            return result;
        }

        private static Regex _tagsToU;
        private static Regex tagsToU
        {
            get
            {
                if (_tagsToU == null)
                    _tagsToU = new Regex(@"<span\sstyle=""text-decoration:\s*?underline;?\s*?"">(.*?)</span>", DefaultOptions | RegexOptions.Compiled);
                return _tagsToU;
            }
        }

        /// <summary>
        /// Replaces span text-decoration tags with u tag.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public string ApplyUnderline(string input)
        {
            string replacement = "<u>$1</u>";
            string result = tagsToU.Replace(input, replacement);
            return result;
        }

        private static Regex _tagsToDel;
        private static Regex tagsToDel
        {
            get
            {
                if (_tagsToDel == null)
                    _tagsToDel = new Regex(@"<s>(.*?)</s>|<strike>(.*?)</strike>|<span\sstyle=""text-decoration:\s*?line-through;?\s*?"">(.*?)</span>", DefaultOptions | RegexOptions.Compiled);
                return _tagsToDel;
            }
        }
        
        /// <summary>
        /// Replaces s, strike and span text-decoration:line-through tags with del tag.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public string ApplyDelTags(string input)
        {
            string replacement = "<del>$1$2$3</del>";
            string result = tagsToDel.Replace(input, replacement);
            return result;
        }

        private static Regex _tagsToBr;
        private static Regex tagsToBr
        {
            get
            {
                if (_tagsToBr == null)
                    _tagsToBr = new Regex(@"<br>", DefaultOptions | RegexOptions.Compiled);
                return _tagsToBr;
            }
        }

        /// <summary>
        /// Replaces &lt;br&gt; tags with &lt;br /&gt; tags.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public string ApplyBrTags(string input)
        {
            string replacement = @"<br />";
            string result = tagsToBr.Replace(input, replacement);
            return result;
        }

        private static Regex _tagsToHr;
        private static Regex tagsToHr
        {
            get
            {
                if (_tagsToHr == null)
                    _tagsToHr = new Regex(@"<hr>", DefaultOptions | RegexOptions.Compiled);
                return _tagsToHr;
            }
        }

        /// <summary>
        /// Replaces &lt;hr&gt; tags with &lt;hr /&gt; tags.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public string ApplyHrTags(string input)
        {
            string replacement = @"<hr />";
            string result = tagsToHr.Replace(input, replacement);
            return result;
        }
    }
}
