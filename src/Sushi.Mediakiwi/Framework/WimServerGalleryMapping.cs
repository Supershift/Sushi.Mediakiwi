using System;
using System.Configuration;
using System.Collections.Generic;
using System.Text;

namespace Sushi.Mediakiwi.Framework
{
    /// <summary>
    /// 
    /// </summary>
    public class WimServerGalleryMapping : ConfigurationElement
    {
        /// <summary>
        /// Gets the path.
        /// </summary>
        /// <value>The path.</value>
        [ConfigurationProperty("path", IsRequired = true)]
        public string Path
        {
            get
            {
                return this["path"] as string;
            }
        }

        /// <summary>
        /// Gets the mapped URL.
        /// </summary>
        /// <value>The mapped URL.</value>
        [ConfigurationProperty("mappedUrl", IsRequired = false)]
        public string MappedUrl
        {
            get
            {
                var url = this["mappedUrl"] as string;
                if (string.IsNullOrEmpty(url) && System.Web.HttpContext.Current != null)
                {
                    if (System.Web.HttpContext.Current.Request.ApplicationPath == "/")
                        return string.Empty;
                    return System.Web.HttpContext.Current.Request.ApplicationPath;
                }
                return url;
            }
        }

        /// <summary>
        /// Gets the asset folder.
        /// </summary>
        /// <value>The asset folder.</value>
        [ConfigurationProperty("assetFolder", IsRequired = false)]
        public string AssetFolder
        {
            get
            {
                return this["assetFolder"] as string;
            }
        }

        /// <summary>
        /// Gets the portal.
        /// </summary>
        /// <value>
        /// The portal.
        /// </value>
        [ConfigurationProperty("portal", IsRequired = false)]
        public string Portal
        {
            get
            {
                return this["portal"] as string;
            }
        }
    }
}
