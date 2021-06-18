using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Sushi.Mediakiwi.RichRext
{
    /// <summary>
    /// Provides methods to remove all tags and attributes except the allowed tags and attributes.
    /// </summary>
    public class CleanupAllowedElements : BaseCleaner
    {
        /// <summary>
        /// Initializes a new CleanupAllowedElements object.
        /// </summary>
        public CleanupAllowedElements() : this(null, null, null, null) { }

        /// <summary>
        /// Initializes a new CleanupAllowedElements object.
        /// </summary>
        /// <param name="extraElements">Array of allowed elements(html tags) that is concatenated to the default allowed elements. Specify without brackets, i.e. 'div' or 'span'.</param>
        /// <param name="unallowedElements">Array of unallowed elements(html tags) that is extracted from the default allowed elements. Specify without brackets, i.e. 'div' or 'span'.</param>
        /// <param name="extraAttributes">Array of allowed attributes on an html tag. It is concatenated to the default allowed attributes.</param>
        /// <param name="unallowedAttributes">Array of unallowed attributes on an html tag. It is extracted from the default allowed attributes.</param>
        public CleanupAllowedElements(string[] extraElements, string[] unallowedElements, string[] extraAttributes, string[] unallowedAttributes)
        {
            string[] allowedElements = DefaultAllowedElements;
            string[] allowedAttributes = DefaultAllowedAttributes;

            //init tags/elements
            if (extraElements != null && extraElements.Length > 0)
            {
                allowedElements = allowedElements.Concat(extraElements).ToArray();
            }

            if (unallowedElements != null && unallowedElements.Length > 0)
            {
                allowedElements = allowedElements.Except(unallowedElements).ToArray();
            }

            System.Text.StringBuilder sb = new StringBuilder();

            foreach (string tag in allowedElements)
            {
                sb.AppendFormat(startTagRegex, tag);
                sb.Append('|');
                sb.AppendFormat(closingTagRegex, tag);
                sb.Append('|');
            }    
            if(sb.Length > 0)
                sb.Remove(sb.Length - 1, 1);

            AllowedTagsRegex = new Regex(sb.ToString(), DefaultOptions);

            //init attributes            
            if (extraAttributes != null && extraAttributes.Length > 0)
            {
                allowedAttributes = allowedAttributes.Concat(extraAttributes).ToArray();
            }

            if (unallowedAttributes != null && unallowedAttributes.Length > 0)
            {
                allowedAttributes = allowedAttributes.Except(unallowedAttributes).ToArray();
            }

            sb = new StringBuilder();
            foreach (string attribute in allowedAttributes)
            {
                sb.AppendFormat(attributeRegex, attribute);
                sb.Append('|');
            }
            if (sb.Length > 0)
                sb.Remove(sb.Length - 1, 1);

            AllowedAttributesRegex = new Regex(sb.ToString(), DefaultOptions);
        }

        /// <summary>
        /// matches a specific opening tag
        /// </summary>
        private string startTagRegex = @"<\s*{0}.*?>";
        /// <summary>
        /// matches a specific closing tag
        /// </summary>
        private string closingTagRegex = @"<\s*\/\s*{0}\s*.*?>";

        /// <summary>
        /// matches a specific attribute
        /// </summary>
        private string attributeRegex = @"({0})=[""']?((?:.(?![""']?\s+(?:\S+)=|[>""']))+.)[""']?";

        private static Regex _tagMatchingRegex;
        /// <summary>
        /// matches all starting and ending tags, without their content
        /// </summary>
        private Regex tagMatchingRegex
        {
            get
            {
                if (_tagMatchingRegex == null)
                {
                    //_tagMatchingRegex = new Regex(@"</?\w+((\s+\w+(\s*=\s*(?:"".*?""|'.*?'|[^'"">\s]+))?)+\s*|\s*)/?>", defaultOptions);//old, works for everything except <o:p> (because of the ':')
                    _tagMatchingRegex = new Regex(@"</?[\w:]+((\s+\w+(\s*=\s*(?:"".*?""|'.*?'|[^'"">\s]+))?)+\s*|\s*)/?>", DefaultOptions);
                }
                return _tagMatchingRegex;
            }
        }

        private static Regex _attributeMatchingRegex;
        /// <summary>
        /// matches all attributes, including their name and value
        /// </summary>
        private Regex attributeMatchingRegex
        {
            get
            {
                if (_attributeMatchingRegex == null)
                    _attributeMatchingRegex = new Regex(@"(\S+)=[""']?((?:.(?![""']?\s+(?:\S+)=|[>""']))+.)[""']?", DefaultOptions);
                return _attributeMatchingRegex;
            }
        }

        protected Regex AllowedTagsRegex { get; private set; }

        protected Regex AllowedAttributesRegex { get; private set; }

        public static readonly string[] DefaultAllowedAttributes = {
                                                                       "id",
                                                                       //"class",
                                                                       //"style",
                                                                       "href",
                                                                       "title",
                                                                       "target",
                                                                       "colspan",
                                                                       "alt",
                                                                       "src",
                                                                       "value",
                                                                       "name",
                                                                       "width",
                                                                       "height",
                                                                       "type"                                                                     
                                                                       //,"allowscriptaccess",
                                                                        //"allowfullscreen"
                                                                   };

        public static readonly string[] DefaultAllowedElements = { 
                                                            //text-level
                                                            "div",
                                                            "span",
                                                            "code",
                                                            "a",
                                                            "strong",
                                                            "b",//for displaying in wim
                                                            "em",
                                                            "u",
                                                            "del",
                                                            "sup",
                                                            "sub",
                                                            "br",
                                                            "hr",
                                                            //grouping
                                                            "ul",
                                                            "ol",
                                                            "li",
                                                            "p",
                                                            "figure",
                                                            "h1",
                                                            "h2",
                                                                "h3",
                                                            "h4",
                                                            "h5",
                                                            "h6",
                                                            //document sections
                                                            "section",
                                                            "header",
                                                            "article",
                                                            "footer",
                                                            "aside",
                                                            "address",
                                                            //tabular data
                                                            "table",
                                                            "tr",
                                                            "td",
                                                            "th",
                                                            "tbody",
                                                            "thead",
                                                            "tfoot",
                                                            //embedding content
                                                            "img",
                                                            "object",
                                                            "param",
                                                            "embed",                                                            
                                                        };
        
        public string RemoveUnwantedElements(string input)
        {            
            string result = tagMatchingRegex.Replace(input, checkAndRemoveTag);
            return result;
        }
        
        private string checkAndRemoveTag(Match m)
        {
            //it's a html-tag! now check if it's allowed            
            if (AllowedTagsRegex.IsMatch(m.Value))
            {
                //it's allowed, keep it and strip it of it's unallowed attributes
                string result = attributeMatchingRegex.Replace(m.Value, checkAndRemoveAttribute);

                return result;
            }
            else
            {
                //it's not on the allowed list, remove it
                return string.Empty;
            }
        }

        private string checkAndRemoveAttribute(Match m)
        {
            if (AllowedAttributesRegex.IsMatch(m.Value))
            {
                //it's allowed, return the complete attribute so it's kept
                return m.Value;
            }
            else
                return string.Empty;
        }
    }
}
