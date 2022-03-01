using Sushi.Mediakiwi.Data.Configuration;
using System;
using System.Reflection;

namespace Sushi.Mediakiwi.Data
{
    public class CommonConfiguration
    {
        public static string DEFAULT_CONTENT_TAB { get { return "Content"; } }
        public static string DEFAULT_SERVICE_TAB { get { return "Service"; } }

        public static string ROOT_FOLDER { get { return "/"; } }


        /// <summary>
        /// Is the environment load balanced? if so caching across nodes needs to be controlled
        /// </summary>
        public static bool IS_LOAD_BALANCED
        {
            get
            {
                return WimServerConfiguration.Instance.Is_Load_Balanced;
            }
        }

        public static bool HIDE_CHANNEL
        {
            get
            {
                return WimServerConfiguration.Instance.Hide_Channel;
            }
        }

        /// <summary>
        /// Registry: Is the environment load balanced? if so caching across nodes needs to be controlled
        /// </summary>
        public static string SPACE_REPLACEMENT
        {
            get
            {
                if (string.IsNullOrEmpty(WimServerConfiguration.Instance.Space_Replacement))
                {
                    return "-";
                }

                return WimServerConfiguration.Instance.Space_Replacement;
            }
        }

        public static int EXPIRATION_COOKIE_PROFILE
        {
            get
            {
                return 60;
            }
        }

        public static int EXPIRATION_COOKIE_VISITOR
        {
            get
            {
                return 40320;
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
    }
}
