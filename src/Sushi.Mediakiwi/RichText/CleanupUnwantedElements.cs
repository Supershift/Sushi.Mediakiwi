using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Sushi.Mediakiwi.RichRext
{
    /// <summary>
    /// Provides methods to remove unwanted html-tags from a string, including their content
    /// </summary>
    public class CleanupUnwantedElements : BaseCleaner
    {
        /// <summary>
        /// Initializes a CleanupUnwatedElements object.
        /// </summary>
        public CleanupUnwantedElements() : this(null, null)
        {

        }

        /// <summary>
        /// Initializes a CleanupUnwatedElements object.
        /// </summary>
        /// <param name="extraElements">Array of html tags that are to be removed, is appended to the default collection</param>
        public CleanupUnwantedElements(string[] extraElements) : this(extraElements, null)
        {            
            
        }

        /// <summary>
        /// Initializes a CleanupUnwatedElements object.
        /// </summary>
        /// <param name="extraElements">Array of html tags that are to be removed, is appended to the default collection</param>
        /// <param name="allowedElements">Array of html tags that are not to be removed.</param>
        public CleanupUnwantedElements(string[] extraElements, string[] allowedElements)
        {
            if (extraElements != null && extraElements.Length > 0)
                UnwantedElements = DefaultUnwantedElements.Concat(extraElements).ToArray();
                        
            if (allowedElements != null && allowedElements.Length > 0)
                UnwantedElements = UnwantedElements.Except(allowedElements).ToArray();

            System.Text.StringBuilder sb = new StringBuilder();

            foreach (string tag in UnwantedElements)
            {
                if (!string.IsNullOrEmpty(tag))
                {
                    sb.AppendFormat(completeTagRemoval, tag);
                    sb.Append('|');
                }
            }
            //add the comment cleaner
            sb.Append(@"<!--(.*?)-->");

            UnwantedElementsRegex = new Regex(sb.ToString(), DefaultOptions);            
        }

        private string[] _unwantedElements;
        protected string[] UnwantedElements
        {
            get
            {
                if (_unwantedElements == null)
                    _unwantedElements = DefaultUnwantedElements;
                return _unwantedElements;
            }
            private set
            {
                _unwantedElements = value;
            }
        }
        
        
        
        /// <summary>
        /// Array of unwanted html-tags.
        /// </summary>
        public static readonly string[] DefaultUnwantedElements = { 
                                                    "script", 
                                                    "meta",
                                                    "link",
                                                    "iframe",
                                                    "head",
                                                    "input"
                                                  };



        private Regex UnwantedElementsRegex { get; set; }
        
        
        /// <summary>
        /// Removes all unwanted html-tags from a string, including their content. 
        /// </summary>        
        /// <param name="input">string from which the html tags are removed.</param>
        public string RemoveUnwantedElements(string input)
        {
            string result;

            result = UnwantedElementsRegex.Replace(input, string.Empty);

            return result;
        }
        
        /// <summary>
        /// Specified tag including content and self containing tags
        /// </summary>
        private string completeTagRemoval = @"<{0}\b[^>]*>(.*?)</{0}>|<{0}\b[^>]*(.*?)/*>";

        
    }
}
