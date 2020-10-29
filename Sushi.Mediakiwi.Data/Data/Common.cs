using Sushi.Mediakiwi.Data.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace Sushi.Mediakiwi.Data
{
    /// <summary>
    /// Represents the defaults settings for the data connections and cached objects.
    /// </summary>
    public static class Common
    {
        private static System.Text.RegularExpressions.Regex _cleanUrlRegex;

        private static System.Text.RegularExpressions.Regex CleanUrlRegex
        {
            get
            {
                if (_cleanUrlRegex == null)
                    _cleanUrlRegex = new System.Text.RegularExpressions.Regex(@"[^a-zA-Z0-9-]+", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                return _cleanUrlRegex;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has wide interface.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has wide interface; otherwise, <c>false</c>.
        /// </value>
        public static bool HasWideInterface
        {
            get { return false; }
        }

        /// <summary>
        /// Gets the entity cache expiration.
        /// </summary>
        /// <value>The entity cache expiration.</value>
        public static DateTime EntityCacheExpiration
        {
            get { return DateTime.Now.AddHours(1); }
        }

        /// <summary>
        /// Gets the entity sliding expiration.
        /// </summary>
        /// <value>The entity sliding expiration.</value>
        public static TimeSpan EntitySlidingExpiration
        {
            get { return new TimeSpan(6, 0, 0); }
        }

        /// <summary>
        /// Gets the database dateTime from the environment settings.
        /// </summary>
        /// <value>
        /// The database date time.
        /// </value>
        public static DateTime DatabaseDateTime
        {
            get { return Environment.Current.CurrentTimezoneDateTime; }
        }

        /// <summary>
        /// Gets the encryption key.
        /// </summary>
        /// <value>The encryption key.</value>
        internal static string EncryptionKey
        {
            get { return "wimserver"; }
        }

        /// <summary>
        /// Gets the folder root.
        /// </summary>
        /// <value>The folder root.</value>
        public static string FolderRoot
        {
            get { return "/"; }
        }

        /// <summary>
        /// Gets the database connection.
        /// </summary>
        /// <value>The database connection.</value>
        public static string DatabaseConnection
        {
            get
            {
                return CurrentPortal.Connection;
            }
        }

        public static string DatabaseConnectionString
        {
            get
            {
                return CurrentPortal.Connection;
            }
        }

        static WimServerPortal m_CurrentPortal;
        /// <summary>
        /// Gets the current portal.
        /// </summary>
        /// <value>The current portal.</value>
        public static WimServerPortal CurrentPortal
        {
            get
            {
                if (m_CurrentPortal != null) return m_CurrentPortal;

                WimServerConfiguration config = WimServerConfiguration.GetConfig();
                if (config != null && config.Portals != null)
                {
                    foreach (WimServerPortal portal in config.Portals)
                    {
                        if (portal.Name == config.DefaultPortal)
                        {
                            m_CurrentPortal = portal;
                            return m_CurrentPortal;
                        }
                    }
                }
                return m_CurrentPortal;
            }
        }

        /// <summary>
        /// Gets the connection.
        /// </summary>
        /// <param name="portal">The portal.</param>
        /// <returns></returns>
        public static SqlConnection GetSqlConnection(string portal)
        {
            WimServerConfiguration config = WimServerConfiguration.GetConfig();
            if (config != null && config.Portals != null)
            {
                foreach (WimServerPortal instance in config.Portals)
                {
                    if (instance.Name == portal)
                    {
                        return new SqlConnection(instance.Connection);
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the current mapping connection.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static WimServerPortal GetCurrentMappingConnection(System.Type type)
        {
            return GetCurrentMappingConnection(type.ToString());
        }

        /// <summary>
        /// Gets the current gallery mapping URL.
        /// </summary>
        /// <param name="galleryPath">The gallery path.</param>
        /// <returns></returns>
        public static WimServerGalleryMapping GetCurrentGalleryMappingUrl(string galleryPath, string url = null)
        {
            if (string.IsNullOrEmpty(galleryPath)) return null;
            WimServerConfiguration config = WimServerConfiguration.GetConfig();
            if (config == null || config.GalleryMappings == null) return null;

            //string url = null;
            //if (System.Web.HttpContext.Current != null)
            //    url = System.Web.HttpContext.Current.Request.Url.OriginalString.Replace(":80", string.Empty);

            foreach (var map in config.GalleryMappings)
            {
                if (map.Path.EndsWith("*"))
                {
                    string tmp = map.Path.Remove(map.Path.Length - 1);
                    if (galleryPath.StartsWith(tmp, StringComparison.InvariantCultureIgnoreCase))
                    {
                        if ((url != null && !url.StartsWith(map.MappedUrl) || string.IsNullOrEmpty(map.MappedUrl)))
                            return map;
                    }
                }
                else
                {
                    if (galleryPath.Equals(map.Path, StringComparison.InvariantCultureIgnoreCase))
                        return map;
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the URL mapping.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static WimServerUrlMapping GetUrlMappingConfiguration(string name, int? type)
        {
            if (string.IsNullOrEmpty(name)) return null;
            var config = WimServerConfiguration.GetConfig();
            if (config == null || config.UrlMappings == null) return null;

            foreach (WimServerUrlMapping map in config.UrlMappings)
            {
                if (map.Name == name && map.Type.GetValueOrDefault() == type.GetValueOrDefault())
                    return map;
            }

            return null;
        }


        /// <summary>
        /// Gets the current mapping connection.
        /// </summary>
        /// <param name="typeName">Name of the type.</param>
        /// <returns></returns>
        public static WimServerPortal GetCurrentMappingConnection(string typeName)
        {
            var config = WimServerConfiguration.GetConfig();
            string portalToFind = null;
            if (config != null && config.DatabaseMappings != null)
            {
                foreach (var map in config.DatabaseMappings)
                {
                    if (typeName.StartsWith(map.NameSpace))
                    {
                        bool exclude = false;

                        if (!string.IsNullOrEmpty(map.Exclude))
                        {
                            foreach (string split in map.Exclude.Split(','))
                            {
                                if (typeName.Contains(split))
                                {
                                    exclude = true;
                                    break;
                                }
                            }
                        }

                        if (!exclude)
                        {
                            portalToFind = map.Portal;
                            break;
                        }
                    }
                }

                foreach (var portal in config.Portals)
                {
                    if (portal.Name == portalToFind)
                        return portal;
                }
            }
            return null;
        }

        public static WimServerPortal GetCurrentMappingConnectionByName(string name)
        {
            if (string.IsNullOrEmpty(name)) return null;
            WimServerConfiguration config = WimServerConfiguration.GetConfig();
            string portalToFind = null;
            if (config != null && config.Portals != null)
            {
                foreach (var map in config.DatabaseMappings)
                {
                    if (map.Name == name)
                    {
                        portalToFind = map.Portal;
                        break;
                    }
                }

                foreach (var portal in config.Portals)
                {
                    if (portal.Name == portalToFind)
                        return portal;
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the general.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static string GetGeneral(string name)
        {
            var config = WimServerConfiguration.GetConfig();
            if (config != null && config.General != null)
            {
                foreach (var map in config.General)
                    if (map.Name == name) return map.Value;
            }
            return string.Empty;
        }

        /// <summary>
        /// Gets the portal.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static WimServerPortal GetPortal(string name)
        {
            return GetPortal(name, true);
        }

        /// <summary>
        /// Gets the portal.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="throwExceptionWhenNotFound">if set to <c>true</c> [throw exception when not found].</param>
        /// <returns></returns>
        /// <exception cref="System.Exception">Could not find the portal</exception>
        public static WimServerPortal GetPortal(string name, bool throwExceptionWhenNotFound)
        {
            if (string.IsNullOrEmpty(name))
                return null;

            var config = WimServerConfiguration.GetConfig();
            if (config != null && config.Portals != null)
            {
                foreach (var portal in config.Portals)
                {
                    if (portal.Name == name)
                        return portal;
                }
            }
            if (throwExceptionWhenNotFound)
                throw new Exception("Could not find the portal");
            return null;
        }

        private static Dictionary<string, string> _WimServerUrlMappings;
        /// <summary>
        /// Gets the wim server URL mappings.
        /// </summary>
        private static Dictionary<string, string> WimServerUrlMappings
        {
            get
            {
                if (_WimServerUrlMappings == null)
                {
                    _WimServerUrlMappings = new Dictionary<string, string>();
                    WimServerConfiguration config = WimServerConfiguration.GetConfig();
                    if (config != null && config.UrlMappings != null)
                    {
                        foreach (var map in config.UrlMappings)
                        {
                            if (!String.IsNullOrEmpty(map.Name))
                                _WimServerUrlMappings.Add(map.Name, map.Path);
                        }

                    }
                }
                return _WimServerUrlMappings;
            }
        }

        /// <summary>
        /// Check if the web.config contains a pagemap for the specified urlMappingName
        /// </summary>
        /// <param name="urlMappingName">The urlMappingName to locate in the web.config</param>
        /// <returns></returns>
        public static bool CheckIfMappedUrlExists(string urlMappingName)
        {
            return WimServerUrlMappings.ContainsKey(urlMappingName);
        }

        /// <summary>
        /// Creates the mapped URL.
        /// </summary>
        /// <param name="urlMappingName">Name of the URL mapping.</param>
        /// <param name="args">The args.</param>
        /// <returns></returns>
        public static string CreateMappedUrl(string urlMappingName, params object[] args)
        {
            return CreateMappedUrl(urlMappingName, false, args);
        }
    }
}