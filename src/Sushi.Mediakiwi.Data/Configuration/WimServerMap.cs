using System;
using System.Configuration;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Sushi.Mediakiwi.Data.Configuration
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract]
    public class WimServerMap
    {
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        [DataMember(Name = "name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets the exclude.
        /// </summary>
        /// <value>The exclude.</value>
        [DataMember(Name = "exclude")]
        public string Exclude { get; set; }

        /// <summary>
        /// Gets the name space.
        /// </summary>
        /// <value>The name space.</value>
        [DataMember(Name = "namespace")]
        public string NameSpace { get; set; }

        /// <summary>
        /// Gets the portal.
        /// </summary>
        /// <value>The portal.</value>
        [DataMember(Name = "portal")]
        public string Portal { get; set; }
    }
}
