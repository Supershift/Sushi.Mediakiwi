using Sushi.Mediakiwi.Data.Configuration;
using Sushi.Mediakiwi.Framework;
using System;
using System.Reflection;

namespace Sushi.Mediakiwi
{
    /// <summary>
    /// Summary description for CommonConfiguration.
    /// </summary>
    public class CommonConfiguration : Sushi.Mediakiwi.Data.CommonConfiguration
    {
        public static string PORTAL_PATH
        {
            get
            {
                return WimServerConfiguration.Instance.Portal_Path;
            }
        }

        /// <summary>
        /// Set the datepicker format, options 'nl' (EU) or 'en' (US)
        /// </summary>
        public static string FORM_DATEPICKER
        {
            get
            {
                if (string.IsNullOrWhiteSpace(WimServerConfiguration.Instance.Datepicker_Language))
                {
                    return "EN";
                }
                return WimServerConfiguration.Instance.Datepicker_Language;
            }
        }

        public static int AUTHENTICATION_TIMEOUT
        {
            get
            {
                return Data.Utility.ConvertToInt(WimServerConfiguration.Instance?.Authentication?.Timeout, 15);
            }
        }
        public static string AUTHENTICATION_COOKIE
        {
            get
            {
                if (string.IsNullOrWhiteSpace(WimServerConfiguration.Instance?.Authentication?.Cookie))
                    return "mediakiwi";
                return WimServerConfiguration.Instance.Authentication.Cookie;
            }
        }
        public static string LOGIN_BACKGROUND
        {
            get
            {
                return WimServerConfiguration.Instance.Login_Background_Url;
            }
        }

        static string _Domain = "https://sushi-mediakiwi.azureedge.net/";
        static string _FolderVersion;

        public static string CDN_Folder(WimComponentListRoot wim, string subfolder)
        {
            return CDN_Folder(wim.Console, subfolder);
        }

        public static string CDN_Folder(Beta.GeneratedCms.Console console, string subfolder)
        {
            return CDN_Folder(console.AddApplicationPath(LOCAL_FILE_PATH, true), subfolder);
        }

        public static string CDN_Folder(string applicationPath, string subfolder)
        {
            string appPath = string.Empty;

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
                    {
                        _FolderVersion = LOCAL_FILE_PATH;
                    }
                    else
                    {
                        _FolderVersion = applicationPath;
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(subfolder))
            {
                return string.Concat(_FolderVersion, subfolder, "/");
            }

            return _FolderVersion;
        }


        public static string VIEWPORT
        {
            get
            {
                return "width=device-width, user-scalable=yes, initial-scale=1";
            }
        }

        public static int? MY_PROFILE_LIST_ID
        {
            get
            {
                return WimServerConfiguration.Instance.My_Profile_List_Id;
            }
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
