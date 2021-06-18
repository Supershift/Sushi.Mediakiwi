using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Runtime.Serialization;

namespace Sushi.Mediakiwi.Data.Configuration
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract]
    public class WimServerUrlMapping 
    {

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        [DataMember(Name = "name")]
        public string Name { get; set; }
        
        /// <summary>
        /// Gets the path.
        /// </summary>
        /// <value>The path.</value>
        [DataMember(Name = "path")]
        public string Path { get; set; }

        [DataMember(Name = "site")]
        public int Site { get; set; }

        [DataMember(Name = "numericFileMapping")]
        public int NumericFileMapping { get; set; }

        [DataMember(Name = "mappedQueryString")]
        public string MappedQueryString { get; set; }

        /// <summary>
        /// Gets the type.
        /// </summary>
        [DataMember(Name = "type")]
        public int? Type { get; set; }

        [DataMember(Name = "mappedItem")]
        public int MappedItem { get; set; }

        [DataMember(Name = "mappedPage", IsRequired = true)]
        public int MappedPage { get; set; }
        
        private static Dictionary<string, Regex> m_PathRegex;
        public Regex PathRegex          //[MR: 24-01-2020] REVIEW BY MARC
        {
            get
            {
                if (m_PathRegex == null)
                    m_PathRegex = new Dictionary<string, Regex>();
                if (!m_PathRegex.ContainsKey(Name))
                {
                    //[MR: 24-01-2020] WAS :
                    //var r = new Regex(Wim.Utility.AddApplicationPath(Path), RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);
                    var r = new Regex(Path, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);
                    /*****************************/

                    m_PathRegex.Add(Name, r);
                    return r;
                }
                return m_PathRegex[Name];
            }
        }
    }
}
