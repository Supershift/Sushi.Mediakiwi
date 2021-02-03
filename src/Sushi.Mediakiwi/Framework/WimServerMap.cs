using System;
using System.Configuration;
using System.Collections.Generic;
using System.Text;

namespace Sushi.Mediakiwi.Framework
{
    /// <summary>
    /// 
    /// </summary>
    public class WimServerMap : ConfigurationElement
    {
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        [ConfigurationProperty("name", IsRequired = true)]
        public string Name
        {
            get
            {
                return this["name"] as string;
            }
        }

        /// <summary>
        /// Gets the exclude.
        /// </summary>
        /// <value>The exclude.</value>
        [ConfigurationProperty("exclude", IsRequired = false)]
        public string Exclude
        {
            get
            {
                return this["exclude"] as string;
            }
        }

        /// <summary>
        /// Gets the name space.
        /// </summary>
        /// <value>The name space.</value>
        [ConfigurationProperty("namespace", IsRequired = true)]
        public string NameSpace
        {
            get
            {
                return this["namespace"] as string;
            }
        }

        /// <summary>
        /// Gets the portal.
        /// </summary>
        /// <value>The portal.</value>
        [ConfigurationProperty("portal", IsRequired = true)]
        public string Portal
        {
            get
            {
                return this["portal"] as string;
            }
        }
    }
}
