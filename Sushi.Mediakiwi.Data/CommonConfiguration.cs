﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Sushi.Mediakiwi.Data;

namespace Sushi.Mediakiwi.Data
{
    public class CommonConfiguration
    {
        public static string DEFAULT_CONTENT_TAB { get { return "Content"; } }      //OVERNEMEN
        public static string DEFAULT_SERVICE_TAB { get { return "Service"; } }      //OVERNEMEN
       
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
        /// Gets a value indicating whether this instance has code generation.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has code generation; otherwise, <c>false</c>.
        /// </value>
        public static bool HasCodeGeneration
        {
            get
            {
                if (string.IsNullOrEmpty(CODEGENERATION_ASSEMBLY) || string.IsNullOrEmpty(CODEGENERATION_FOLDER) || string.IsNullOrEmpty(CODEGENERATION_NAMESPACE)) return false;
                return true;
            }
        }

        public static bool IS_PRODUCTION { get { return Common.GetGeneral("IS_PRODUCTION") == "1"; } }
        public static bool HAS_HEARTBEAT { get { return Common.GetGeneral("HEARTBEAT") == "1"; } }
        public static bool NEW_NAVIGATION { get { return Common.GetGeneral("NEW_NAVIGATION") == "1"; } }
        public static bool LOG_UNHANDLED_ERRORS { get { return Common.GetGeneral("LOG_UNHANDLED_ERRORS") == "1"; } }
        public static bool HTML_ENCODE_LIST_PROPERTIES { get { return Environment.Current["HTML_ENCODE_LIST_PROPERTIES", true, "0", "If true, all postback values from ComponentListTemplate properties are HTML encoded except when explicitly allowed"] == "1"; } }

        //MV 2017-08-10: changed default to 'true'
        public static bool ALWAYS_RETRIEVE_VISITOR_FROM_DATABASE
        {
            get
            {
                string candidate = Environment.Current["ALWAYS_RETRIEVE_VISITOR_FROM_DATABASE", true, "1", "When the visitor object is loaded for the first time in a http request, always retrieve the latest version from the database"];
                return candidate != "0";
            }
        }

        public static bool RIGHTS_GALLERY_SUBS_ARE_ALLOWED { get { return Common.GetGeneral("RIGHTS_GALLERY_SUBS_ARE_ALLOWED") == "1"; } }

        public static int MAX_SESSION_LENGTH { get { return Utility.ConvertToInt(Common.GetGeneral("MAX_SESSION_LENGTH"), 20); } }

        public static bool HAS_POPUPLAYER_OPTION { get { return Common.GetGeneral("HAS_POPUPLAYER_OPTION") == "1"; } }
        //public static bool LINQ_SHOULD_RETURN_SINGLETON_INSTANCE { get { return Common.GetGeneral("WIM_LINQ_SINGLETON_INSTANCE") == "1"; } }
        public static string LOCAL_PAGE_NOT_FOUND { get { return Common.GetGeneral("LOCAL_PAGE_NOT_FOUND"); } }

        static string[] m_SECURITY_IP_ACCESS_RANGE;
        public static string[] SECURITY_IP_ACCESS_RANGE
        {
            get
            {
                if (m_SECURITY_IP_ACCESS_RANGE == null)
                {
                    var tst = Common.GetGeneral("SECURITY_IP_ACCESS_RANGE");
                    if (string.IsNullOrEmpty(tst))
                        m_SECURITY_IP_ACCESS_RANGE = new string[0];
                    else
                        m_SECURITY_IP_ACCESS_RANGE = tst.Split(',');
                }
                return m_SECURITY_IP_ACCESS_RANGE;
            }
        }
        public static string SECURITY_NO_ACCESS_PAGE { get { return Common.GetGeneral("SECURITY_NO_ACCESS_PAGE"); } }


        public static bool HAS_PAGE_CACHE { get { return Utility.ConvertToInt(Common.GetGeneral("NO_PAGE_CACHE")) == 0; } }
        public static bool HAS_PATH_IN_CDN { get { return Utility.ConvertToInt(Common.GetGeneral("HAS_PATH_IN_CDN")) == 1; } }

        public static string HTTP_IMPERSONATION_USER { get { return Common.GetGeneral("HTTP_IMPERSONATION_USER"); } }
        public static string HTTP_IMPERSONATION_PASSWORD { get { return Common.GetGeneral("HTTP_IMPERSONATION_PASSWORD"); } }
        public static string HTTP_IMPERSONATION_URL { get { return Common.GetGeneral("HTTP_IMPERSONATION_URL"); } }
        public static int[] DEFAULT_THUMB_BGCOLOR
        {
            get
            {
                string bglist = Common.GetGeneral("DEFAULT_THUMB_BGCOLOR");
                if (string.IsNullOrEmpty(bglist) || bglist.Split(',').Length != 3)
                    bglist = "255,255,255";
                return Utility.ConvertToIntArray(bglist.Split(','));
            }
        }
        public static int PROFILE_CHECK_INTERVAL { get { return Utility.ConvertToInt(Common.GetGeneral("PROFILE_CHECK_INTERVAL")); } }
        public static int NO_IMAGE_ASSET { get { return Utility.ConvertToInt(Common.GetGeneral("NO_IMAGE_ASSET")); } }
        public static string LOCAL_REQUEST_URL { get { return Common.GetGeneral("LOCAL_REQUEST_URL"); } }
        public static string EXCLUDE_PATH_HANDLER { get { return Common.GetGeneral("EXCLUDE_PATH_HANDLER"); } }
        public static string LOAD_BALANCER_IP_HEADER { get { return Common.GetGeneral("LOAD_BALANCER_IP_HEADER"); } }

        public static bool SQL_DEBUG { get { return Common.GetGeneral("SQL_DEBUG") == "1"; } }
        public static bool SQL_DEBUG_STACKTRACE { get { return Common.GetGeneral("SQL_DEBUG_STACKTRACE") == "1"; } }
        public static int PERFORMANCE_LISTENER_SQL_THRESHOLD { get { return Utility.ConvertToInt(Environment.Current["PERFORMANCE_LISTENER_SQL_THRESHOLD"], 1000); } }
        public static int PERFORMANCE_LISTENER_WEB_THRESHOLD { get { return Utility.ConvertToInt(Environment.Current["PERFORMANCE_LISTENER_WEB_THRESHOLD"], 2000); } }
        public static bool IGNORE_WEBSITE_HANDLER { get { return Common.GetGeneral("IGNORE_WEBSITE_HANDLER") == "1"; } }

        public static bool FORCE_SSL { get { return Common.GetGeneral("FORCE_SSL") == "1"; } }
        public static bool FORCE_NEWSTYLE { get { return Common.GetGeneral("FORCE_NEWSTYLE") == "1"; } }
        public static bool FORCE_NEWSTYLE2
        {
            get
            {
                return true;
            }
        }

        public static string LOGIN_BACKGROUND { get { return Common.GetGeneral("LOGIN_BACKGROUND"); } }
        public static string LOGIN_BOXLOGO { get { return Common.GetGeneral("LOGIN_BOXLOGO"); } }


        public static string FORCE_NEWSTYLE_ONLIST { get { return Common.GetGeneral("FORCE_NEWSTYLE_ONLIST"); } }
        public static bool FORCE_MANAGEMENT_LOGIN { get { return Common.GetGeneral("FORCE_MANAGEMENT_LOGIN") == "1"; } }

        public static string WIM_URL { get { return Common.GetGeneral("WIM_URL"); } }
        public static string WEB_URL { get { return Common.GetGeneral("WEB_URL"); } }
        public static string WIM_COOKIE { get { return Common.GetGeneral("VISITOR_COOKIE"); } }

        /// <summary>
        /// Will skip the HttpRequest.IsLocal check for cookie domain validation.
        /// NOTE: will only work when IS_LOCAL_DEVELOPMENT is also set to 1.
        /// </summary>
        public static bool WIM_COOKIE_SKIP_ISLOCAL_CHECK
        {
            get { return Common.GetGeneral("VISITOR_COOKIE_SKIP_ISLOCAL_CHECK") == "1"; }
        }

        public static string WIM_COOKIE_EXCLUDELIST { get { return Common.GetGeneral("VISITOR_AGENT_EXCLUDE"); } }
        /// <summary>
        /// Comma seperated encryption header, default = REMOTE_ADDR (or in case of proxy: HTTP_X_FORWARDED_FOR)
        /// </summary>
        public static string WIM_COOKIE_ENCRYPT_KEY { get { return Common.GetGeneral("VISITOR_COOKIE_ENCRYPTION_KEY"); } }
        public static bool WIM_COOKIE_HTTP_ONLY { get { return Common.GetGeneral("VISITOR_COOKIE_HTTP_ONLY") == "1"; } }
        public static bool WIM_COOKIE_SECURE { get { return Common.GetGeneral("VISITOR_COOKIE_SECURE") == "1"; } }

        // [MR:04-07-2019] added for a bug in IE11 (Yes, it's 2019 and im fixing an IE11 bug... help me)
        // For more info see : https://stackoverflow.com/questions/22690114/internet-explorer-11-wont-set-cookies-on-a-site
        public static bool COOKIEDOMAIN_LOCALHOST_AS_NULL { get { return Common.GetGeneral("COOKIEDOMAIN_LOCALHOST_AS_NULL") == "1"; } }

        public static bool WIM_DEBUG { get { return Common.GetGeneral("WIM_DEBUG") == "1"; } }
        public static string FLUENTMAPPINGS_ASSEMBLIES
        {
            get
            {
                string candidate = Common.GetGeneral("FLUENTMAPPINGS_ASSEMBLIES");
                if (string.IsNullOrEmpty(candidate))
                {
                    candidate = "Wim.Framework.dll";
                }
                return candidate;
            }
        }
        public static bool APPLY_FIXED_PATH { get { return true; } }
        /// <summary>
        /// Registry: Is the environment load balanced? if so caching across nodes needs to be controlled
        /// </summary>
        public static bool IS_LOAD_BALANCED
        {   //
            get
            {
                return Environment.Current["IS_LOAD_BALANCED", true, "1", "Is the environment load balanced? if so caching across nodes needs to be controlled."] == "1";
            }
        }
        public static string CLOUD_SETTINGS { get { return Common.GetGeneral("CLOUD_SETTINGS"); } }
        public static string CLOUD_TYPE { get { return Common.GetGeneral("CLOUD_TYPE"); } }

        public static string STYLE_INCLUDE { get { return Common.GetGeneral("STYLE_INCLUDE"); } }
        public static bool STYLE_COMPLEMENT { get { return Common.GetGeneral("STYLE_COMPLEMENT") == "1"; } }

        public static bool WEB_DEBUG { get { return Common.GetGeneral("WEB_DEBUG") == "1"; } }
        public static bool REDIRECT_CHANNEL_PATH
        {
            get
            {
                return Common.GetGeneral("REDIRECT_CHANNEL_PATH") == "1";
            }
        }
        public static int GENERATED_IMAGE_QUALITY
        {
            get
            {
                int genQual = 80;
                if (!string.IsNullOrEmpty(Common.GetGeneral("GENERATED_IMAGE_QUALITY")))
                    genQual = Utility.ConvertToInt(Common.GetGeneral("GENERATED_IMAGE_QUALITY"), 80);

                if (genQual > 100)
                    genQual = 100;

                if (genQual < 50)
                    genQual = 50;

                return genQual;
            }
        }
        //public static int VISUAL_VERSION { get { return 2; } }
        internal static bool IS_LOCAL_TEST { get { return Common.GetGeneral("IS_LOCAL_TEST") == "1"; } }
        internal static string LOCAL_COPY_PATH { get { return Common.GetGeneral("LOCAL_COPY_PATH"); } }

        public static bool DISABLE_CACHE { get { return Common.GetGeneral("DISABLE_CACHE") == "1"; } }
        public static bool IS_FULL_TRUST { get { return Common.GetGeneral("IS_FULL_TRUST") != "0"; } }
        public static bool IS_LOCAL_DEVELOPMENT { get { return Common.GetGeneral("IS_LOCAL_DEVELOPMENT") == "1"; } }
        public static bool IS_AJAX_ENABLED { get { return Common.GetGeneral("ENABLE_AJAX") == "1"; } }
        public static bool IS_AJAX_ENABLED_IN_WIM { get { return Common.GetGeneral("ENABLE_AJAX_IN_WIM") == "1"; } }
        public static bool USE_LINQ { get { return Common.GetGeneral("USE_LINQ") == "1"; } }
        public static bool ONLY_ONE_PROFILE_LOGIN { get { return Common.GetGeneral("ONLY_ONE_PROFILE_LOGIN") == "1"; } }
        public static bool USE_RICHTEXT_IMAGEADD { get { return Common.GetGeneral("USE_RICHTEXT_IMAGEADD") == "1"; } }
        public static bool USE_RICHTEXT_TABLE { get { return Common.GetGeneral("USE_RICHTEXT_TABLE") == "1"; } }
        public static bool USE_RICHTEXT_STYLE { get { return Common.GetGeneral("USE_RICHTEXT_STYLE") == "1"; } }
        public static string USE_RICHTEXT_STYLEFILE { get { return Common.GetGeneral("USE_RICHTEXT_STYLEFILE"); } }
        /// <summary>
        /// Gets the CODEGENERATIO n_ ASSEMBLY.
        /// </summary>
        /// <value>The CODEGENERATIO n_ ASSEMBLY.</value>
        public static string CODEGENERATION_ASSEMBLY
        {
            get
            {

                if (!string.IsNullOrEmpty(Common.GetGeneral("CODEGENERATION_ASSEMBLY")))
                    return Common.GetGeneral("CODEGENERATION_ASSEMBLY");

                return string.Concat(Environment.Current["CODEGENERATION_NAMESPACE"], ".dll");
            }
        }

        /// <summary>
        /// Gets the CODEGENERATIO n_ NAMESPACE.
        /// </summary>
        /// <value>The CODEGENERATIO n_ NAMESPACE.</value>
        public static string CODEGENERATION_NAMESPACE
        {
            get
            {

                if (!string.IsNullOrEmpty(Common.GetGeneral("CODEGENERATION_NAMESPACE")))
                    return Common.GetGeneral("CODEGENERATION_NAMESPACE");

                return Environment.Current["CODEGENERATION_NAMESPACE"];
            }
        }

        /// <summary>
        /// Gets the PAGEMA p_ AN d_ REPLACEMENT.
        /// </summary>
        public static string PAGEMAP_AND_REPLACEMENT
        {
            get
            {

                if (!string.IsNullOrEmpty(Common.GetGeneral("PAGEMAP_AND_REPLACEMENT")))
                    return Common.GetGeneral("PAGEMAP_AND_REPLACEMENT");

                return Environment.Current["SPACE_REPLACEMENT"];
            }
        }

        /// <summary>
        /// Gets the CODEGENERATIO n_ FOLDER.
        /// </summary>
        /// <value>The CODEGENERATIO n_ FOLDER.</value>
        public static string CODEGENERATION_FOLDER
        {
            get
            {

                if (!string.IsNullOrEmpty(Common.GetGeneral("CODEGENERATION_FOLDER")))
                    return Common.GetGeneral("CODEGENERATION_FOLDER");

                return Environment.Current["CODEGENERATION_FOLDER"];
            }
        }


        /// <summary>
        /// Gets the default cache time span.
        /// </summary>
        /// <value>The default cache time span.</value>
        public static TimeSpan DefaultCacheTimeSpan
        {
            get
            {
                return new TimeSpan(6, 0, 0);
            }
        }

        /// <summary>
        /// Gets a value indicating whether [force static lists].
        /// </summary>
        /// <value><c>true</c> if [force static lists]; otherwise, <c>false</c>.</value>
        public static bool ForceStaticLists
        {
            get
            {
                //if (System.Web.HttpContext.Current != null && System.Web.HttpContext.Current.Items.Contains("no-cache"))
                //    return true;
                return false;
            }
        }

        public static bool UseLocalPageCache
        {
            get
            {
                //if (System.Web.HttpContext.Current != null && System.Web.HttpContext.Current.Items.Contains("no-cache"))
                //    return true;
                return true;
            }
        }

        /// <summary>
        /// Gets a value indicating whether [force no cache].
        /// </summary>
        /// <value><c>true</c> if [force no cache]; otherwise, <c>false</c>.</value>
        public static bool ForceNoCache
        {
            get
            {
                //if (System.Web.HttpContext.Current != null && System.Web.HttpContext.Current.Items.Contains("no-cache"))
                //    return true;
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether [force update].
        /// </summary>
        /// <value><c>true</c> if [force update]; otherwise, <c>false</c>.</value>
        public static bool ForceUpdate
        {
            get
            {

                return System.Configuration.ConfigurationManager.AppSettings["ForceUpdate"] == "1";
            }
        }

        /// <summary>
        /// Gets the version.
        /// </summary>
        public static decimal Version
        {
            get
            {
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

        /// <summary>
        /// 
        /// </summary>
        public static string siteRoot   // naar STANDARD
        {
            get { return "/"; }
        }

        /// <summary>
        /// 
        /// </summary>
        public static string RelativeRepositoryUrl
        {
            get { return Environment.Current.RepositoryFolder; }
        }

        /// <summary>
        /// 
        /// </summary>
        public static string RelativeRepositoryImageUrl
        {
            get
            {
                return string.Format("{0}/image/", Environment.Current.RepositoryFolder);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static string RelativeRepositoryDocumentUrl
        {
            get { return string.Format("{0}/document/", Environment.Current.RepositoryFolder); }
        }

        /// <summary>
        /// 
        /// </summary>
        public static string RelativeRepositoryImageThumbnailUrl
        {
            get { return string.Format("{0}/thumbnail/", RelativeRepositoryBase); }
        }

        public static string RelativeRepositoryGeneratedImageUrl
        {
            get { return string.Format("{0}/generated/", RelativeRepositoryBase); }
        }

        /// <summary>
        /// Gets the relative repository TMP URL.
        /// </summary>
        /// <value>The relative repository TMP URL.</value>
        public static string RelativeRepositoryTmpUrl
        {
            get { return string.Format("{0}/tmp/", RelativeRepositoryBase); }
        }

        /// <summary>
        /// Gets the relative repository package URL.
        /// </summary>
        /// <value>The relative repository package URL.</value>
        public static string RelativeRepositoryPackageUrl
        {
            get { return string.Format("{0}/package/", RelativeRepositoryBase); }
        }

        /// <summary>
        /// Gets the relative repository script URL.
        /// </summary>
        /// <value>The relative repository script URL.</value>
        public static string RelativeRepositoryScriptUrl
        {
            get { return string.Format("{0}/scripts/", RelativeRepositoryBase); }
        }

        /// <summary>
        /// 
        /// </summary>
        public static string RelativeRepositoryBase     //NAAR STANDARD, FIND REFERENCES
        {
            get { return "/repository"; }
        }

        /// <summary>
        /// 
        /// </summary>
        public static string RelativeRepositoryWimUrl
        {
            get { return string.Format("{0}/wim/", RelativeRepositoryBase); }
        }


        public static bool USE_CSS_TICKS_FOR_PATHCONTROL
        {
            get { return Common.GetGeneral("USE_CSS_TICKS_FOR_PATHCONTROL") == "1"; }
        }
    }
}
