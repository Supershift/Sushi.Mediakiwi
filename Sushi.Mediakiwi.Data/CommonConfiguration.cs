using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Sushi.Mediakiwi.Data;
using Sushi.Mediakiwi.Data.Configuration;

namespace Sushi.Mediakiwi.Data
{
    public class CommonConfiguration
    {
        public static string DEFAULT_CONTENT_TAB { get { return "Content"; } }
        public static string DEFAULT_SERVICE_TAB { get { return "Service"; } }

        public static string ROOT_FOLDER { get { return "/"; } }

        /// <summary>
        /// Registry: Is the environment load balanced? if so caching across nodes needs to be controlled
        /// </summary>
        public static bool IS_LOAD_BALANCED
        {   //
            get
            {
                return WimServerConfiguration.Instance.Is_Load_Balanced;
            }
        }

        /// <summary>
        /// Registry: Is the environment load balanced? if so caching across nodes needs to be controlled
        /// </summary>
        public static bool HTML_ENCODE_HTMLINPUT
        {   //
            get
            {
                return WimServerConfiguration.Instance.Is_Load_Balanced;
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
    }
}
