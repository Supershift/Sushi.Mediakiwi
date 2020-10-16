using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.Runtime.Serialization;

namespace Sushi.Mediakiwi.Data.Configuration
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract]
    public class WimServerConfiguration 
    {
        public static WimServerConfiguration Instance { get; set; }

        /// <summary>
        /// Will load the supplied Json filename, or the default 'appsettings.json'
        /// when no filename is supplied
        /// </summary>
        /// <param name="jsonFileName"></param>
        public static void LoadJsonConfig(string jsonFileName = "appsettings.json")
        {
            string fileName = string.Empty;
            if (jsonFileName.Contains("\\") == false)
                fileName = $"{AppDomain.CurrentDomain.BaseDirectory}{jsonFileName}";
            else
                fileName = jsonFileName;

            var configuration = new ConfigurationBuilder()
                 .AddJsonFile(fileName, false)
                 .Build();

            // Assign json section to config
            Instance = configuration.GetSection("wimServerConfiguration").Get<WimServerConfiguration>();
        }

        /// <summary>
        /// Gets the portal collection.
        /// </summary>
        /// <value>
        /// The portal collection.
        /// </value>
        [DataMember(Name ="portals")]
        public List<WimServerPortal> Portals { get; set; }

        /// <summary>
        /// Gets the map collection.
        /// </summary>
        /// <value>
        /// The map collection.
        /// </value>
        [DataMember(Name = "databaseMappings")]
        public List<WimServerMap> DatabaseMappings { get; set; }


        /// <summary>
        /// Gets the gallery mapping collection.
        /// </summary>
        [DataMember(Name = "galleryMappings")]
        public List<WimServerGalleryMapping> GalleryMappings { get; set; }

        /// <summary>
        /// Gets the URL mappingsg collection.
        /// </summary>
        [DataMember(Name = "urlMappings")]
        public List<WimServerUrlMapping> UrlMappings { get; set; }

        /// <summary>
        /// Gets the general.
        /// </summary>
        /// <value>
        /// The general.
        /// </value>
        [DataMember(Name = "general")]
        public List<WimServerGeneral> General { get; set; }

        /// <summary>
        /// Gets the default portal.
        /// </summary>
        /// <value>
        /// The default portal.
        /// </value>
        [DataMember(Name = "defaultPortal")]
        public string DefaultPortal { get; set; }
    }
}
