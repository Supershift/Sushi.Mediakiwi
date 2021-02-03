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
    public class WimServerGalleryMapping 
    {
        /// <summary>
        /// Gets the path.
        /// </summary>
        /// <value>The path.</value>
        [DataMember(Name = "path")]
        public string Path { get; set; }

        /// <summary>
        /// Gets the mapped URL.
        /// </summary>
        /// <value>The mapped URL.</value>
        [DataMember(Name = "mappedUrl")]
        public string MappedUrl { get; set; } 

        /// <summary>
        /// Gets the asset folder.
        /// </summary>
        /// <value>The asset folder.</value>
        [DataMember(Name = "assetFolder")]
        public string AssetFolder { get; set; }

        /// <summary>
        /// Gets the portal.
        /// </summary>
        /// <value>
        /// The portal.
        /// </value>
        [DataMember(Name = "portal")]
        public string Portal { get; set; }
    }
}
