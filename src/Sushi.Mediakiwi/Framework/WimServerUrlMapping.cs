using System;
using System.Configuration;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Sushi.Mediakiwi.Framework
{
    /// <summary>
    /// 
    /// </summary>
    public class WimServerUrlMapping : ConfigurationElement
    {

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        [ConfigurationProperty("name", IsRequired = false)]
        public string Name
        {
            get
            {
                return this["name"] as string;
            }
        }
        /// <summary>
        /// Gets the path.
        /// </summary>
        /// <value>The path.</value>
        [ConfigurationProperty("path", IsRequired = false)]
        public string Path
        {
            get
            {
                return this["path"] as string;
            }
        }

        [ConfigurationProperty("site", IsRequired = false)]
        public int Site
        {
            get
            {
                return Convert.ToInt32(this["site"]);
            }
        }

        [ConfigurationProperty("numericFileMapping", IsRequired = false)]
        public int NumericFileMapping
        {
            get
            {
                return Wim.Utility.ConvertToInt(this["numericFileMapping"]);
            }
        }

        [ConfigurationProperty("mappedQueryString", IsRequired = false)]
        public string MappedQueryString
        {
            get
            {
                return this["mappedQueryString"] as string;
            }
        }

        /// <summary>
        /// Gets the type.
        /// </summary>
        [ConfigurationProperty("type", IsRequired = false)]
        public int? Type
        {
            get
            {
                return Wim.Utility.ConvertToInt(this["type"]);
            }
        }

        [ConfigurationProperty("mappedItem", IsRequired = false)]
        public int MappedItem
        {
            get
            {
                return Wim.Utility.ConvertToInt(this["mappedItem"]);
            }
        }

        [ConfigurationProperty("mappedPage", IsRequired = true)]
        public int MappedPage
        {
            get
            {
                return Wim.Utility.ConvertToInt(this["mappedPage"]);
            }
        }
        private static Dictionary<string, Regex> m_PathRegex;
        public Regex PathRegex
        {
            get
            {
                if (m_PathRegex == null)
                    m_PathRegex = new Dictionary<string, Regex>();
                if (!m_PathRegex.ContainsKey(Name))
                {
                    var r = new Regex(Wim.Utility.AddApplicationPath(Path), RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);
                    m_PathRegex.Add(Name, r);
                    return r;
                }
                return m_PathRegex[Name];
            }
        }
    }
}
