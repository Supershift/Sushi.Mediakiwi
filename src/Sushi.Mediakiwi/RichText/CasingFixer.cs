using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Sushi.Mediakiwi.RichRext
{
    public class CasingFixer : BaseCleaner
    {
        private static Regex _completeTagRegex;
        private static Regex completeTagRegex
        {
            get
            {
                if (_completeTagRegex == null)
                    _completeTagRegex = new Regex(@"</?\w+((\s+\w+(\s*=\s*(?:"".*?""|'.*?'|[^'"">\s]+))?)+\s*|\s*)/?>", DefaultOptions | RegexOptions.Compiled);
                return _completeTagRegex;
            }
        }

        private static Regex _attributesRegex;
        private static Regex attributesRegex
        {
            get
            {
                if(_attributesRegex == null)                
                    _attributesRegex = new Regex(@"(?<Key>\S+)=(?<Quote>[""']?)(?<Value>(?:.(?![""']?\s+(?:\S+)=|[>""']))+.)[""']?", DefaultOptions | RegexOptions.Compiled);
                return _attributesRegex;
            }
        }

        private static Regex _tagRegex;
        private static Regex tagRegex
        {
            get
            {
                if (_tagRegex == null)
                    _tagRegex = new Regex(@"</?\w+", DefaultOptions | RegexOptions.Compiled);
                return _tagRegex;
            }
        }

        
        
        /// <summary>
        /// Converts all html-tags and attribute names to lowercase. Innertext and values are left alone.
        /// </summary>        
        public string ApplyLowercase(string inputText)
        {            
            string result = completeTagRegex.Replace(inputText, TagToLowerCase);

            return result;
        }

        private string TagToLowerCase(Match m)
        {
            string result = tagRegex.Replace(m.Value, TagToLower);
            result = attributesRegex.Replace(result, AttributeToLower);
            
            return result;
        }

        private string TagToLower(Match m)
        {   
            return m.Value.ToLower();
        }

        private string AttributeToLower(Match m)
        {
            string Key = m.Groups["Key"].Value;
            string Quote = m.Groups["Quote"].Value;
            string Value = m.Groups["Value"].Value;

            return string.Format(@"{0}={1}{2}{1}", Key.ToLower(), Quote, Value);
        }
    }
}
