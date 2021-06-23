using Sushi.Mediakiwi.Data.Configuration;
using Sushi.Mediakiwi.Framework;
using System;
using System.Reflection;

namespace Sushi.Mediakiwi
{
    /// <summary>
    /// Summary description for CommonConfiguration.
    /// </summary>
    public class CommonConfiguration
    {
        public static string PORTAL_PATH
        {
            get
            {
                return WimServerConfiguration.Instance.Portal_Path;
            }
        }
        public static int AUTHENTICATION_TIMEOUT
        {
            get
            {
                return Data.Utility.ConvertToInt(WimServerConfiguration.Instance.Authentication_Timeout, 15);
            }
        }
        public static string AUTHENTICATION_COOKIE
        {
            get
            {
                if (string.IsNullOrEmpty(WimServerConfiguration.Instance.Authentication_Cookie))
                    return "mediakiwi";
                return WimServerConfiguration.Instance.Authentication_Cookie;
            }
        }
        public static bool RIGHTS_GALLERY_SUBS_ARE_ALLOWED { get { return true; } }
        public static string LOGIN_BACKGROUND { 
            get { 
                return WimServerConfiguration.Instance.Login_Background_Url; 
            }
        }

        static string _Domain = "https://sushi-mediakiwi.azureedge.net/";
        static string _FolderVersion;
        internal static string CDN_Folder(WimComponentListRoot wim, string subfolder)
        {
            if (_FolderVersion == null)
            {
                // CDN
                if (string.IsNullOrEmpty(LOCAL_FILE_PATH))
                {
                    _FolderVersion = string.Concat(_Domain, Utils.Version.Replace(".", "-"), "/");
                }
                else
                {
                    if (LOCAL_FILE_PATH.IndexOf("http", StringComparison.InvariantCultureIgnoreCase) > -1)
                        _FolderVersion = LOCAL_FILE_PATH;
                    else
                        _FolderVersion = wim.AddApplicationPath(LOCAL_FILE_PATH, true);
                }
            }

            if (!string.IsNullOrWhiteSpace(subfolder))
                return string.Concat(_FolderVersion, subfolder);

            return _FolderVersion;
        }

        public static string FILE_VERSION
        {
            get
            {
                return WimServerConfiguration.Instance.File_Version;
            }
        }
        public static string LOGIN_BOXLOGO
        {
            get
            {
                return WimServerConfiguration.Instance.Loginbox_Logo_Url;
            }
        }
        public static string LOGO_URL
        {
            get
            {
                return WimServerConfiguration.Instance.Logo_Url;
            }
        }

        /// <summary>
        /// Hide the breadcrumbs
        /// </summary>
        public static bool HIDE_BREADCRUMB
        {   //
            get
            {
                return WimServerConfiguration.Instance.Hide_Breadcrumbs;
            }
        }

        /// <summary>
        /// Is the current environment the local development environment?
        /// </summary>
        public static bool IS_LOCAL_DEVELOPMENT
        {   //
            get
            {
                return WimServerConfiguration.Instance.Is_Local_Development;
            }
        }
        /// <summary>
        /// HTML Encode in for a textarea
        /// </summary>
        public static bool HTML_ENCODE_TEXTAREA_INPUT
        {   //
            get
            {
                return WimServerConfiguration.Instance.Html_Encode_Textarea_iInput;
            }
        }
        public static string STYLE_INCLUDE
        {   
            get
            {
                return WimServerConfiguration.Instance.Stylesheet;
            }
        }
        public static string LOCAL_FILE_PATH
        {
            get
            {
                return WimServerConfiguration.Instance.Local_File_Path;
            }
        }
     
        public static bool DISABLE_CACHE
        {  
            get
            {
                return WimServerConfiguration.Instance.Disable_Caching;
            }
        }

        /// <summary>
        /// Gets the default cache time.
        /// </summary>
        /// <value>The default cache time.</value>
        public static DateTime DefaultCacheTime
        {
            get
            {
                return DateTime.Now.AddDays(6);
            }
        }

        /// <summary>
        /// Gets the version.
        /// </summary>
        public static decimal Version
        {
            get {
                Version version = Assembly.GetExecutingAssembly().GetName().Version;
                return Convert.ToDecimal(string.Format("{0}{2}{1}", version.Major, version.Minor, System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator));
            }
        }

        /// <summary>
        /// Gets the version full.
        /// </summary>
        /// <value>The version full.</value>
        public static string VersionFull
        {
            get
            {
                Version version = Assembly.GetExecutingAssembly().GetName().Version;
                return string.Format("{0}.{1:00}.{2:0000}.{3:0000}", version.Major, version.Minor, version.Build, version.Revision);
            }
        }

        public static string BuildNumber
        {
            get
            {
                Version version = Assembly.GetExecutingAssembly().GetName().Version;
                return string.Format("{0}{1:00}{2:0000}{3:0000}", version.Major, version.Minor, version.Build, version.Revision);
            }
        }
    }
}
