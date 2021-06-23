using System.Text.RegularExpressions;

namespace Sushi.Mediakiwi.Framework.ContentInfoItem
{
    /// <summary>
    /// 
    /// </summary>
    public class RichTextCleanup
    {
        /// <summary>
        /// Cleans the specified candidate.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <returns></returns>
        public string Clean(string candidate)
        {
            if (string.IsNullOrEmpty(candidate)) return null;

            candidate = new Regex(@"<table.*?>", RegexOptions.IgnoreCase).Replace(candidate, "<table class=\"dataTable\">");
            candidate = new Regex(@"<td.*?>", RegexOptions.IgnoreCase).Replace(candidate, "<td>");

            candidate = new Regex(@"<td.*?>", RegexOptions.IgnoreCase).Replace(candidate, "<td>");

            bool hasMatch = true;

            Regex rex1 = new Regex(@"<br /><table", RegexOptions.IgnoreCase);
            Regex rex2 = new Regex(@"<br /></td>", RegexOptions.IgnoreCase);
            while (hasMatch)
            {
                hasMatch = false;
                if (rex1.IsMatch(candidate))
                {
                    hasMatch = true;
                    candidate = rex1.Replace(candidate, "<table");
                }
                if (rex2.IsMatch(candidate))
                {
                    hasMatch = true;
                    candidate = rex2.Replace(candidate, "</td>");
                }
            }

            candidate = new Regex(@"<tbody>(?<TEXT>.*?)(</tbody>)", RegexOptions.IgnoreCase).Replace(candidate, this.OnlyReturnText);
            candidate = new Regex(@"<thead>(?<TEXT>.*?)(</thead>)", RegexOptions.IgnoreCase).Replace(candidate, this.OnlyReturnText);

            //candidate = candidate.Replace("<tbody>", string.Empty).Replace("</tbody>", string.Empty);

            candidate = new Regex(@"<span.*?(>)(?<TEXT>.*?)(</span>)", RegexOptions.IgnoreCase).Replace(candidate, this.OnlyReturnText);
            candidate = new Regex(@"<font.*?(>)(?<TEXT>.*?)(</font>)", RegexOptions.IgnoreCase).Replace(candidate, this.OnlyReturnText);
            
            //  Sometimes only a <span..> exists
            candidate = new Regex(@"<span.*?(>)", RegexOptions.IgnoreCase).Replace(candidate, string.Empty);
            candidate = new Regex(@"<font.*?(>)", RegexOptions.IgnoreCase).Replace(candidate, string.Empty);

            return Prepare.Clean(candidate);

            //return candidate;
        }

        public RichTextCleanup()
        {
            this.Prepare = new RichTextPrepare();
        }

        public RichTextPrepare Prepare { get; set; }

        internal string OnlyReturnText(Match m)
        {
            string text = m.Groups["TEXT"].Value;
            return text;
        }
    }
}
