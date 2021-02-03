using System.Text.RegularExpressions;

namespace Sushi.Mediakiwi.Headless.Config
{
    /// <summary>
    /// The URL mapping class for redirecting to an internal page based
    /// on the URLMatch 
    /// </summary>
    public class URLMappingConfig
    {
        /// <summary>
        /// The internal Page ID
        /// </summary>
        public int PageID { get; set; }

        /// <summary>
        /// The URL to match. 
        /// You can use the following syntax : /faq/{topic}/{question}
        /// In this case topic and question will be added as a HttpContext item
        /// </summary>
        public string URLMatch { get; set; }

        private Regex _Rex;
        public Regex Rex
        {
            get
            {
                if (_Rex == null && string.IsNullOrWhiteSpace(URLMatch) == false)
                {
                    string temp = URLMatch;
                    if (temp.Contains("{"))
                        temp = temp.Replace("{", @"(?<");
                    if (temp.Contains("}"))
                        temp = temp.Replace("}", @">.[^\/]*)");
                    temp = temp.Replace("/", @"\/");

                    _Rex = new Regex(@temp, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Singleline);
                }

                return _Rex;
            }
            set 
            {
                _Rex = value;
            }
        }
    }
}
