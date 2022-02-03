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

        /// <summary>
        /// The current static Instance of the <c>WimServerConfiguration</c> class
        /// </summary>
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

        /// <summary>
        /// This will be used as FallBack culture whenever a Site Culture is not available
        /// </summary>
        public string Datepicker_Culture { get; set; }

        /// <summary>
        /// Setting this to True will execute non-destructive SQL scripts
        /// </summary>
        public bool Sql_Install_Enabled { get; set; }

        /// <summary>
        /// Setting this to True will also execute destructive SQL scripts
        /// </summary>
        public bool Sql_Install_Actions_Enabled { get; set; }

        /// <summary>
        /// Represents the Connectionstring name to use as default portal connection
        /// </summary>
        public string Connection { get; set; }

        /// <summary>
        /// In which Azure container should assets be saved ?
        /// </summary>
        public string Azure_Image_Container { get; set; }

        /// <summary>
        /// The Azure CDN url to use as prefix 
        /// </summary>
        public string Azure_Cdn_Uri { get; set; }

        /// <summary>
        /// The Encryption key to use for password encryption
        /// </summary>
        public string Encryption_key { get; set; }

        /// <summary>
        /// The Salt to add to the password encryption
        /// </summary>
        public string Encryption_Salt { get; set; }
        
        /// <summary>
        /// Is the environment load balanced? if so caching across nodes needs to be controlled.
        /// </summary>
        public bool Is_Load_Balanced { get; set; }

        /// <summary>
        /// The character to use as space replacement in all URLs
        /// </summary>
        public string Space_Replacement { get; set; }

        /// <summary>
        /// The ListID to use for the 'My Profile' link in the CMS
        /// </summary>
        public int? My_Profile_List_Id { get; set; }

        /// <summary>
        /// Should we hide the Channel dropdown ?
        /// </summary>
        public bool Hide_Channel { get; set; }

        /// <summary>
        /// Is the current environment the local development environment?
        /// </summary>
        public bool Is_Local_Development { get; set; }

        /// <summary>
        /// Should we hide breadcrumbs in Mediakiwi ?
        /// </summary>
        public bool Hide_Breadcrumbs { get; set; }

        /// <summary>
        /// Should we encode HTML for a textarea
        /// </summary>
        public bool Html_Encode_Textarea_Input { get; set; }

        /// <summary>
        /// The Login Background image URL
        /// </summary>
        public string Login_Background_Url { get; set; }

        /// <summary>
        /// What addition should be used to identify resource files ?
        /// This enables clearing of cached resources
        /// </summary>
        public string File_Version { get; set; }
        
        /// <summary>
        /// The Login Logo image URL
        /// </summary>
        public string Loginbox_Logo_Url { get; set; }

        /// <summary>
        /// The global mediakiwi Logo image URL
        /// </summary>
        public string Logo_Url { get; set; }

        /// <summary>
        /// Define a stylesheet file that should be included in every page in the Mediakiwi portal.
        /// </summary>
        public string Stylesheet { get; set; }

        /// <summary>
        /// Determines if the CSS file mentioned in <c>Stylesheet</c> should be appended.
        /// When set to <c>True</c> it will append it. When set to <c>False</c> it will replace it
        /// </summary>
        public bool Append_Stylesheet { get; set; }

        /// <summary>
        /// When this is set, all Mediakiwi portal files are loaded from here, instead of from the CDN
        /// </summary>
        public string Local_File_Path { get; set; }

        /// <summary>
        /// When set to <c>True</c>, this will disable the internal Mediakiwi caching
        /// </summary>
        public bool Disable_Caching { get; set; }

        /// <summary>
        /// Contains the Portal collection.
        /// </summary>
        public List<WimServerPortal> Portals { get; set; }

        /// <summary>
        /// Contains the database mapping collection.
        /// </summary>
        [DataMember(Name = "databaseMappings")]
        public List<WimServerMap> DatabaseMappings { get; set; }

        /// <summary>
        /// Contains the gallery mapping collection.
        /// </summary>
        [DataMember(Name = "galleryMappings")]
        public List<WimServerGalleryMapping> GalleryMappings { get; set; }

        /// <summary>
        /// Contains the URL mapping collection.
        /// </summary>
        [DataMember(Name = "urlMappings")]
        public List<WimServerUrlMapping> UrlMappings { get; set; }

        /// <summary>
        /// Gets the default portal.
        /// </summary>
        [Obsolete("Use 'Connection' property instead", false)]
        public string DefaultPortal { get; set; }

        /// <summary>
        /// Contains all Authentication configuration
        /// </summary>
        public AuthenticationConfiguration Authentication { get; set; }

        /// <summary>
        /// Contains all Wiki configuration
        /// </summary>
        public WikiConfiguration Wiki { get; set; } = new WikiConfiguration();

        /// <summary>
        /// Contains all Thumbnail configuration
        /// </summary>
        [ConfigurationKeyName("thumbnails")]
        public ThumbnailConfiguration Thumbnails { get; set; } = new ThumbnailConfiguration();
    }

    public class ThumbnailConfiguration
    {
        /// <summary>
        /// Should thumbnails be created when uploading an image ?
        /// </summary>
        [ConfigurationKeyName("create-thumbnails")]
        public bool CreateThumbnails { get; set; }

        /// <summary>
        /// The maximum width of the thumbnail that's created when <c>CreateThumbnails</c> is True
        /// </summary>
        [ConfigurationKeyName("create-thumbnail-width")]
        public int CreateThumbnailWidth { get; set; } = 320;

        /// <summary>
        /// The maximum height of the thumbnail that's created when <c>CreateThumbnails</c> is True
        /// </summary>
        [ConfigurationKeyName("create-thumbnail-height")]
        public int CreateThumbnailHeight { get; set; } = 240;

        /// <summary>
        /// Should thumbnails be Shown in the gallery overview ?
        /// </summary>
        [ConfigurationKeyName("show-thumbnails-in-gallery")]
        public bool ShowThumbnailsInGallery { get; set; }

        /// <summary>
        /// The maximum width of the thumbnail that being shown when <c>ShowThumbnailsInGallery</c> is True
        /// </summary>
        [ConfigurationKeyName("gallery-thumbnail-width")]
        public int GalleryThumbnailWidth { get; set; } = 128;

        /// <summary>
        /// The maximum height of the thumbnail that being shown when <c>ShowThumbnailsInGallery</c> is True
        /// </summary>
        [ConfigurationKeyName("gallery-thumbnail-height")]
        public int GalleryThumbnailHeight { get; set; } = 96;

    }

    public class AuthenticationConfiguration
    {
        /// <summary>
        /// The name of the cookie to set for Mediakiwi authentication
        /// </summary>
        public string Cookie { get; set; }

        /// <summary>
        /// The Cookie timeout in minutes
        /// </summary>
        public int Timeout { get; set; }

        /// <summary>
        /// Contains all Azure Active Directory configuration
        /// </summary>
        public ActiveDirectory Aad { get; set; }

        /// <summary>
        /// Contains all Azure Active Directory Token configuration
        /// </summary>
        public TokenValidation Token { get; set; }
    }

    public class WikiConfiguration
    {
        /// <summary>
        /// Should help on Lists be enabled ?
        /// </summary>
        public bool EnableHelpOnLists { get; set; }

        /// <summary>
        /// Should help on pages be enabled ?
        /// </summary>
        public bool EnableHelpOnPages { get; set; }
    }

    public class TokenValidation
    {
        /// <summary>
        /// The exponent for this AAD Token
        /// </summary>
        public string Exponent { get; set; }

        /// <summary>
        /// The Modulus for this AAD Token
        /// </summary>
        [Obsolete("Use the DiscoveryLogic class to load the modulus ")]
        public string Modulus { get; set; }

        /// <summary>
        /// The KeyType for this AAD Token
        /// </summary>
        public string KeyType { get; set; }
    }

    public class ActiveDirectory
    {
        /// <summary>
        /// Is AAD authentication enabled ?
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// The redirect URL where to land after Authentication
        /// </summary>
        public Uri RedirectUrl { get; set; }

        /// <summary>
        /// The Client name for this AAD
        /// </summary>
        public string Client { get; set; }

        /// <summary>
        /// The Tenant for this AAD
        /// </summary>
        public string Tenant { get; set; }

        /// <summary>
        /// The E-mail claim for this AAD
        /// </summary>
        public string EmailClaim { get; set; }
    }
}