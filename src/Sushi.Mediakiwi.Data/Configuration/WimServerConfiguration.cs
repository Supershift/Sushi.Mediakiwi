using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Sushi.Mediakiwi.Data.Configuration
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract]
    public class WimServerConfiguration 
    {
        public static WimServerConfiguration GetConfig()
        {
            if (Instance == null)
                LoadJsonConfig();
            return Instance;
        }

        public static WimServerConfiguration Instance { get; set; }

        public static void LoadJsonConfig(IConfiguration configuration)
        {
            // Assign json section to config
            Instance = configuration.GetSection("mediakiwi").Get<WimServerConfiguration>();

            Instance.DefaultPortal = Instance.Connection;

            Instance.Portals = new List<WimServerPortal>();

            var portals = configuration.GetSection("ConnectionStrings");

            foreach (var portal in portals.GetChildren())
            {
                Instance.Portals.Add(new WimServerPortal() {
                    Connection = configuration.GetConnectionString(portal.Key),
                    Name = portal.Key
                });
            }
        }

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

            LoadJsonConfig(configuration);
        }
        public string Portal_Path { get; set; }

        public string Connection { get; set; }
        public string Azure_Image_Container { get; set; }
        public string Azure_Cdn_Uri { get; set; }

        public string Encryption_key { get; set; }

        /// <summary>
        /// Is the environment load balanced? if so caching across nodes needs to be controlled.
        /// </summary>
        public bool Is_Load_Balanced { get; set; }

        /// <summary>
        /// Is the current environment the local development environment?
        /// </summary>
        public bool Is_Local_Development { get; set; }
        public bool Hide_Breadcrumbs { get; set; }

        /// <summary>
        /// HTML Encode in for a textarea
        /// </summary>
        public bool Html_Encode_Textarea_iInput { get; set; }

        public string Login_Background_Url { get; set; }

        public string File_Version { get; set; }
        
        public string Loginbox_Logo_Url { get; set; }
        public string Logo_Url { get; set; }

        public string Stylesheet { get; set; }
        
        public string Local_File_Path { get; set; }

        public bool Disable_Caching { get; set; }

        /// <summary>
        /// Gets the portal collection.
        /// </summary>
        /// <value>
        /// The portal collection.
        /// </value>
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
        //public List<WimServerGeneral> General { get; set; }

        /// <summary>
        /// Gets the default portal.
        /// </summary>
        /// <value>
        /// The default portal.
        /// </value>
        public string DefaultPortal { get; set; }

        public AuthenticationConfiguration Authentication { get; set; }
    }

    public class AuthenticationConfiguration
    {
        public string Cookie { get; set; }
        public int Timeout { get; set; }
        public ActiveDirectory Aad { get; set; }
        public TokenValidation Token { get; set; }
    }

    public class TokenValidation
    {
        public string Exponent { get; set; }
        public string Modulus { get; set; }
    }

    public class ActiveDirectory
    {
        public bool Enabled { get; set; }
        public System.Uri RedirectUrl { get; set; }
        public string Client { get; set; }
        public string Tenant { get; set; }
    }
}