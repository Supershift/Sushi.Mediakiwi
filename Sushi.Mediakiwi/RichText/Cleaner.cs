using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sushi.Mediakiwi.RichRext
{
    /// <summary>
    /// Provides methods to clean the html input of a richtext editor instance.
    /// </summary>
    public class Cleaner
    {
        /// <summary>
        /// Initialize a new Cleaner instance.
        /// </summary>
        public Cleaner() : this(false){}

        /// <summary>
        /// Initialize a new Cleaner instance.
        /// </summary>
        /// <param name="useAppSettings">A value indicating whether the settings stored in AppSettings should be used to initialize this instance.</param>
        public Cleaner(bool useAppSettings)
        {
            this.UseAppSettings = useAppSettings;
        }
        
        /// <summary>
        /// Gets a value indicating whether the cleaner is initialized using settings stored in AppSettings.
        /// </summary>
        public bool UseAppSettings { get; private set; }

        private CasingFixer _casingFixer;
        protected CasingFixer CasingFixer
        {
            get
            {
                if (_casingFixer == null)
                {
                    _casingFixer = new CasingFixer();
                }
                return _casingFixer;
            }
        }

        private CleanupRichTextButtons _stylingCleaner;
        protected CleanupRichTextButtons StylingCleaner
        {
            get
            {
                if (_stylingCleaner == null)
                {
                    _stylingCleaner = new CleanupRichTextButtons();
                }
                return _stylingCleaner;
            }
        }

        private CleanupUnwantedElements _unwantedElementRemover;
        protected CleanupUnwantedElements UnwantedElementRemover
        {
            get
            {
                if (_unwantedElementRemover == null)
                {
                    string allowedTagsCsv = System.Configuration.ConfigurationManager.AppSettings["RICHTEXT_ALLOWED_TAGS"];
                    string unAllowedTagsCsv = System.Configuration.ConfigurationManager.AppSettings["RICHTEXT_UNALLOWED_TAGS_REMOVE_CONTENT"];

                    string[] allowedTags = null;
                    string[] unAllowedTags = null;
                    
                    if (!string.IsNullOrEmpty(allowedTagsCsv))                    
                        allowedTags = SplitAndCleanCsv(allowedTagsCsv);
                    if (!string.IsNullOrEmpty(unAllowedTagsCsv))                    
                        unAllowedTags = SplitAndCleanCsv(unAllowedTagsCsv);

                    _unwantedElementRemover = new CleanupUnwantedElements(unAllowedTags, allowedTags);
                }
                return _unwantedElementRemover;
            }
        }

        private CleanupAllowedElements _allowedElementFixer;
        protected CleanupAllowedElements AllowedElementFixer
        {
            get
            {
                if (_allowedElementFixer == null)
                {
                    string allowedTagsCsv = System.Configuration.ConfigurationManager.AppSettings["RICHTEXT_ALLOWED_TAGS"];
                    string unAllowedTagsCsv = System.Configuration.ConfigurationManager.AppSettings["RICHTEXT_UNALLOWED_TAGS_KEEP_CONTENT"];
                    string allowedAttributesCsv = System.Configuration.ConfigurationManager.AppSettings["RICHTEXT_ALLOWED_ATTRIBUTES"];
                    string unAllowedAttributesCsv = System.Configuration.ConfigurationManager.AppSettings["RICHTEXT_UNALLOWED_ATTRIBUTES"];

                    string[] allowedTags = null;
                    string[] unAllowedTags = null;
                    string[] allowedAttributes = null;
                    string[] unAllowedAttributes = null;

                    if (!string.IsNullOrEmpty(allowedTagsCsv))
                        allowedTags = SplitAndCleanCsv(allowedTagsCsv);
                    if (!string.IsNullOrEmpty(unAllowedTagsCsv))                    
                        unAllowedTags = SplitAndCleanCsv(unAllowedTagsCsv);
                    if (!string.IsNullOrEmpty(allowedAttributesCsv))
                        allowedAttributes = SplitAndCleanCsv(allowedAttributesCsv);
                    if (!string.IsNullOrEmpty(allowedTagsCsv))
                        unAllowedAttributes = SplitAndCleanCsv(unAllowedAttributesCsv);

                    _allowedElementFixer = new CleanupAllowedElements(allowedTags, unAllowedTags, allowedAttributes, unAllowedAttributes);
                }
                
                return _allowedElementFixer;
            }
        }

        private CharacterCleaner _charCleaner;
        protected CharacterCleaner CharCleaner
        {
            get
            {
                if (_charCleaner == null)
                {
                    _charCleaner = new CharacterCleaner();
                }
                return _charCleaner;
            }
        }

        private char[] _separatorChars;
        private char[] SeparatorChars
        {
            get
            {
                if(_separatorChars == null)
                    _separatorChars = new char[2] { ',', ';' };
                return _separatorChars;
            }
        }

        private string[] SplitAndCleanCsv(string csv)
        {
            string[] result = csv.Replace(" ", string.Empty).Split(SeparatorChars, StringSplitOptions.RemoveEmptyEntries);
            return result;
        }
        
        /// <summary>
        /// Applies all rich-text cleaning rules to an input string.
        /// </summary>
        /// <param name="input">input html string</param>         
        public string ApplyFullClean(string input)
        {
            return ApplyFullClean(input, false);
        }

        /// <summary>
        /// Applies all rich-text cleaning rules to an input string.
        /// </summary>
        /// <param name="input">input html string</param> 
        /// <param name="useBoldInsteadOfStrong">replace strong tags with b-tags, use this for displaying in Wim edit mode</param>
        public string ApplyFullClean(string input, bool useBoldInsteadOfStrong)
        {
            //string _useNewRichTextCleaning = System.Configuration.ConfigurationManager.AppSettings["RICHTEXT_NEW_CLEAN_HTMLAGILITYPACK"];

            if (input != null)
            {
                //if (_useNewRichTextCleaning == "1")
                //{
                    AgilityCleaner cleaner = new AgilityCleaner();
                    string result = cleaner.CleanHTML(input);
                    return result;
                //}
                //else
                //{
                //    string result = CasingFixer.ApplyLowercase(input);
                //    result = StylingCleaner.ApplyFullClean(result);
                //    result = UnwantedElementRemover.RemoveUnwantedElements(result);
                //    result = AllowedElementFixer.RemoveUnwantedElements(result);
                //    result = CharCleaner.ApplyCharacterClean(result);
                //    result = AdditionalCaseCleaners(result);
                //    if (useBoldInsteadOfStrong)
                //    {
                //        result = StylingCleaner.ApplyBold(result);
                //    }
                //    return result;
                //}
            }
            else
                return null;
        }
        /// <summary>
        /// Exceptional problem cases for blocks of HTML to clean
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        private string AdditionalCaseCleaners(string result)
        {
            return result.Replace(@"<strong ==""string"" )="""" {="""" elm=""(d"" ||="""" document).createelement(elm);="""" }="""" return="""" elm;="""" }""="""" this.style.border)="""" 0="""" :="""" 0;="""" [curleft+b,curtop+b+this.offsetheight];="""" (\\s|^)nicedit-'+cls+'(\\s|$)'));="""" +="" nicEdit-"" +cls="""" };="""" this;="""" (\\s|^)nicedit-'+cls+'(\\s|$)'),'="""" ');="""" float':="""" elmstyle['cssfloat']=""elmStyle['styleFloat']"" st[itm];="""" break;="""" case="""" 'opacity':="""" elmstyle.opacity=""st[itm];"" elmstyle.filter="")"" ;""="""" 'classname':="""" this.classname=""st[itm];"" default:="""" if(document.compatmode="""" itm="""" !=""cursor"" nasty="""" workaround="""" for="""" ie="""" 5.5="""" elmstyle[itm]=""st[itm];"">", "<strong>")
                .Replace(@" elmstyle[itm]=""st[itm];"" this.classname=""st[itm];"" elmstyle.filter="")"" elmstyle.opacity=""st[itm];"" elmstyle[?cssfloat?]=""elmStyle['styleFloat']"" elm=""(d"" +="" nicEdit-"" !=""cursor""", string.Empty)
                ;
        }
    }
}
