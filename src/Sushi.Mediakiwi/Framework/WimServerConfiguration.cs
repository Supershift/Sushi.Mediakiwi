using System;
using System.Configuration;
using System.Collections.Generic;
using System.Text;

namespace Sushi.Mediakiwi.Framework
{
    /// <summary>
    /// 
    /// </summary>
    public class WimServerConfiguration : ConfigurationSection
    {
        /// <summary>
        /// Gets the config.
        /// </summary>
        /// <returns></returns>
        public static WimServerConfiguration GetConfig()
        {
            return ConfigurationManager.GetSection("wimServerConfiguration") as WimServerConfiguration;
        }

        /// <summary>
        /// Gets the portal collection.
        /// </summary>
        /// <value>
        /// The portal collection.
        /// </value>
        [ConfigurationProperty("portals")]
        public WimServerPortalCollection PortalCollection
        {
            get
            {
                return this["portals"] as WimServerPortalCollection;
            }
        }

        /// <summary>
        /// Gets the map collection.
        /// </summary>
        /// <value>
        /// The map collection.
        /// </value>
        [ConfigurationProperty("databasemap", IsRequired = false)]
        public WimServerMapCollection MapCollection
        {
            get
            {
                return this["databasemap"] as WimServerMapCollection;
            }
        }

        /// <summary>
        /// Gets the gallery mapping collection.
        /// </summary>
        [ConfigurationProperty("galleryMappings", IsRequired = false)]
        public WimServerGalleryMappingCollection GalleryMappingCollection
        {
            get
            {
                return this["galleryMappings"] as WimServerGalleryMappingCollection;
            }
        }

        /// <summary>
        /// Gets the URL mappingsg collection.
        /// </summary>
        [ConfigurationProperty("urlMappings", IsRequired = false)]
        public WimServerUrlMappingCollection UrlMappingsgCollection
        {
            get
            {
                return this["urlMappings"] as WimServerUrlMappingCollection;
            }
        }

        /// <summary>
        /// Gets the general.
        /// </summary>
        /// <value>
        /// The general.
        /// </value>
        [ConfigurationProperty("general", IsRequired = false)]
        public WimServerGeneralCollection General
        {
            get
            {
                return this["general"] as WimServerGeneralCollection;
            }
        }

        /// <summary>
        /// Gets the default portal.
        /// </summary>
        /// <value>
        /// The default portal.
        /// </value>
        [ConfigurationProperty("defaultPortal", IsRequired = true)]
        public string DefaultPortal
        {
            get
            {
                return this["defaultPortal"] as string;
            }
        }
    }
}
