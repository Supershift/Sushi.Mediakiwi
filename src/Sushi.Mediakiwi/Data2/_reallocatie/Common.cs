using System;
using System.Data;
using System.Configuration;
using System.Data.Odbc;
using System.Data.OleDb;
using System.Data.Common;
using System.Data.SqlTypes;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Diagnostics;
using System.Web;
using Sushi.Mediakiwi.Framework;
//using InterSystems.Data.CacheClient;
//using InterSystems.Data.CacheTypes;

namespace Sushi.Mediakiwi.Data
{
    /// <summary>
    /// Represents the defaults settings for the data connections and cached objects.
    /// </summary>
	public static class Common
	{
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
                    Sushi.Mediakiwi.Framework.WimServerConfiguration config = Sushi.Mediakiwi.Framework.WimServerConfiguration.GetConfig();
                    if (config != null && config.UrlMappingsgCollection != null)
                    {
                        foreach (Sushi.Mediakiwi.Framework.WimServerUrlMapping map in config.UrlMappingsgCollection)
                        {
                            if (!String.IsNullOrEmpty(map.Name))
                                _WimServerUrlMappings.Add(map.Name, map.Path);
                        }

                    }
                }
                return _WimServerUrlMappings;
            }
        }

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


        public class MappedUrlCreator : iMappedUrlCreator
        {
            public string CreateMappedUrl(string urlMappingName, bool UseSpaceReplacement = false, params object[] args)
            {
                if (WimServerUrlMappings.ContainsKey(urlMappingName))
                {
                    string spaceReplacement = Sushi.Mediakiwi.Data.Environment.Current["SPACE_REPLACEMENT"];
                    string path = WimServerUrlMappings[urlMappingName];
                    string reversepath = "(" + path.Replace("(", "¤").Replace(")", "(").Replace("¤", ")") + ")";
                    Regex p = new Regex(reversepath, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
                    Match m = p.Match(reversepath);
                    int groupCount = m.Groups.Count - 2;
                    if (args.Length < groupCount)
                    {
                        StackTrace stackTrace = new StackTrace();
                        Sushi.Mediakiwi.Data.Notification.InsertOne("CreateMappedUrl", Data.NotificationType.Error, String.Format("Params count didn't match on {0}; {1} groups and {2} params; called by {3}", urlMappingName, groupCount, args.Length, stackTrace.GetFrame(1).GetMethod().Name));
                    }
                    string replaceString = String.Empty;
                    for (int i = 1; i < m.Groups.Count; i++)
                    {
                        object a = 0;
                        if (i != m.Groups.Count - 1)
                        {
                            if (i - 1 < args.Length)
                            {
                                //a = HttpUtility.UrlEncode((args[i - 1] + "").Replace(" ", spaceReplacement)); --edit DV: does not properly remove symbols like ':' or '?'
                                if (UseSpaceReplacement)
                                {
                                    a = args[i - 1].ToString().Replace(" ", spaceReplacement);
                                    a = CleanUrlRegex.Replace(a + string.Empty, string.Empty);
                                }
                                else
                                {
                                    a = CleanUrlRegex.Replace(args[i - 1] + string.Empty, string.Empty);
                                }
                            }
                            replaceString += "${" + i + "}" + a;
                        }
                        else
                            replaceString += "${" + i + "}";
                    }

                    return Wim.Utility.AddApplicationPath(p.Replace(path, replaceString));
                }
                return Wim.Utility.AddApplicationPath("/") + "?nomappingfoundfor=" + urlMappingName;
            }

        }

        static iMappedUrlCreator _MappedUrlCreator;

        public static string CreateMappedUrl(string urlMappingName, bool UseSpaceReplacement = false, params object[] args)
        {
            if (_MappedUrlCreator == null)
                _MappedUrlCreator = Environment.GetInstance<iMappedUrlCreator>();
            return _MappedUrlCreator.CreateMappedUrl(urlMappingName, UseSpaceReplacement, args);
        }

        /// <summary>
        /// Gets the database connection.
        /// </summary>
        /// <value>The database connection.</value>
        public static string DatabaseConnection
        {
            get {
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

        /// <summary>
        /// Gets the database connection.
        /// </summary>
        /// <value>The database connection.</value>
        //public static IDbConnection DatabaseConnection
        //{
        //    get
        //    {
        //        switch (CurrentPortal.Type)
        //        {
        //            case Sushi.Mediakiwi.Data.DataConnectionType.SqlServer:
        //                return new SqlConnection(DatabaseConnectionString);
        //            case Sushi.Mediakiwi.Data.DataConnectionType.Odbc:
        //                return new OdbcConnection(DatabaseConnectionString);
        //            case Sushi.Mediakiwi.Data.DataConnectionType.OleDb:
        //                return new OleDbConnection(DatabaseConnectionString);
        //            case Sushi.Mediakiwi.Data.DataConnectionType.InterSystemsCache:
        //                return new CacheConnection(DatabaseConnectionString);
        //        }
        //        throw new Exception("Could not determine the connection Type");
        //    }
        //}

        /// <summary>
        /// Databases the connection get.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static IDbConnection DatabaseConnectionGet(System.Type type)
        {
            return DatabaseConnectionGet(type.ToString());
        }

        /// <summary>
        /// Databases the connection.
        /// </summary>
        /// <param name="typeName">Name of the type.</param>
        /// <returns></returns>
        public static IDbConnection DatabaseConnectionGet(string typeName)
        {
            Sushi.Mediakiwi.Framework.WimServerPortal portal = GetCurrentMappingConnection(typeName);
            string connection = portal == null ? CurrentPortal.Connection : portal.Connection;
            switch (CurrentPortal.Type)
            {
                case Sushi.Mediakiwi.Data.DataConnectionType.SqlServer: return new SqlConnection(connection);
                case Sushi.Mediakiwi.Data.DataConnectionType.Odbc: return new OdbcConnection(connection);
                case Sushi.Mediakiwi.Data.DataConnectionType.OleDb: return new OleDbConnection(connection);
                //case Sushi.Mediakiwi.Data.DataConnectionType.InterSystemsCache: return new CacheConnection(connection);
            }
            throw new Exception("Could not determine the connection Type");
        }

        /// <summary>
        /// Gets a value indicating whether [support silverlight].
        /// </summary>
        /// <value><c>true</c> if [support silverlight]; otherwise, <c>false</c>.</value>
        //public static bool SupportSilverlight
        //{
        //    get
        //    {
        //        return true;
        //    }
        //}

        /// <summary>
        /// Gets the type of the database connection.
        /// </summary>
        /// <value>The type of the database connection.</value>
        public static Sushi.Mediakiwi.Data.DataConnectionType DatabaseConnectionType
        {
            get
            {
                return (Sushi.Mediakiwi.Data.DataConnectionType)CurrentPortal.Type;
            }
        }

        static Sushi.Mediakiwi.Framework.WimServerPortal m_CurrentPortal;
        /// <summary>
        /// Gets the current portal.
        /// </summary>
        /// <value>The current portal.</value>
        public static Sushi.Mediakiwi.Framework.WimServerPortal CurrentPortal
        {
            get
            {
                if (m_CurrentPortal != null) return m_CurrentPortal;

                Sushi.Mediakiwi.Framework.WimServerConfiguration config = Sushi.Mediakiwi.Framework.WimServerConfiguration.GetConfig();
                if (config != null && config.PortalCollection != null)
                {
                    foreach (Sushi.Mediakiwi.Framework.WimServerPortal portal in config.PortalCollection)
                    {
                        if (portal.Name == config.DefaultPortal)
                        {
                            m_CurrentPortal = portal;
                            return m_CurrentPortal;
                        }
                    }
                }
                else
                {
                    m_CurrentPortal = new Sushi.Mediakiwi.Framework.WimServerPortal();
                    m_CurrentPortal.Connection = ConfigurationManager.AppSettings["connect"];
                    //m_CurrentPortal.DefaultSkin = ConfigurationManager.AppSettings["skin"];
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
            Sushi.Mediakiwi.Framework.WimServerConfiguration config = Sushi.Mediakiwi.Framework.WimServerConfiguration.GetConfig();
            if (config != null && config.PortalCollection != null)
            {
                foreach (Sushi.Mediakiwi.Framework.WimServerPortal instance in config.PortalCollection)
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
        public static Sushi.Mediakiwi.Framework.WimServerPortal GetCurrentMappingConnection(System.Type type)
        {
            return GetCurrentMappingConnection(type.ToString());
        }

        /// <summary>
        /// Gets the current gallery mapping URL.
        /// </summary>
        /// <param name="galleryPath">The gallery path.</param>
        /// <returns></returns>
        public static Sushi.Mediakiwi.Framework.WimServerGalleryMapping GetCurrentGalleryMappingUrl(string galleryPath)
        {
            if (string.IsNullOrEmpty(galleryPath)) return null;
            Sushi.Mediakiwi.Framework.WimServerConfiguration config = Sushi.Mediakiwi.Framework.WimServerConfiguration.GetConfig();
            if (config == null || config.GalleryMappingCollection == null) return null;

            string url = null;
            if(System.Web.HttpContext.Current != null)            
                url = System.Web.HttpContext.Current.Request.Url.OriginalString.Replace(":80", string.Empty);

            foreach (Sushi.Mediakiwi.Framework.WimServerGalleryMapping map in config.GalleryMappingCollection)
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
        public static Sushi.Mediakiwi.Framework.WimServerUrlMapping GetUrlMappingConfiguration(string name, int? type)
        {
            if (string.IsNullOrEmpty(name)) return null;
            Sushi.Mediakiwi.Framework.WimServerConfiguration config = Sushi.Mediakiwi.Framework.WimServerConfiguration.GetConfig();
            if (config == null || config.UrlMappingsgCollection == null) return null;

            foreach (Sushi.Mediakiwi.Framework.WimServerUrlMapping map in config.UrlMappingsgCollection)
            {
                if (map.Name == name && map.Type.GetValueOrDefault() == type.GetValueOrDefault())
                    return map;
            }

            return null;
        }

        /// <summary>
        /// Gets the current URL mapping.
        /// </summary>
        /// <param name="absolutePath">The absolute path.</param>
        /// <returns></returns>
        public static Sushi.Mediakiwi.Data.PageMapping GetCurrentUrlMapping(string absolutePath)
        {
            if (string.IsNullOrEmpty(absolutePath)) return null;
            Sushi.Mediakiwi.Framework.WimServerConfiguration config = Sushi.Mediakiwi.Framework.WimServerConfiguration.GetConfig();
            if (config == null || config.UrlMappingsgCollection == null) return null;

            int itemId = 0;
            var filesplit = absolutePath.Split('/');
            var filename = filesplit[filesplit.Length - 1].Split('.')[0];

            int itemTypeId = 0;
            var filetype = filesplit[filesplit.Length - 2];

            bool isFileNameNumeric = Wim.Utility.IsNumeric(filename, out itemId);
            bool isFileTypeNumeric = Wim.Utility.IsNumeric(filetype, out itemTypeId);

            foreach (Sushi.Mediakiwi.Framework.WimServerUrlMapping map in config.UrlMappingsgCollection)
            {
                if (map.Path.EndsWith("*"))
                {
                   string tmp = map.Path.Remove(map.Path.Length - 1);
                    if (absolutePath.StartsWith(Wim.Utility.AddApplicationPath(tmp), StringComparison.InvariantCulture))
                    {
                        if (map.NumericFileMapping == 1 && isFileNameNumeric)
                        {
                            if (map.Type.HasValue)
                            {
                                if (itemTypeId == map.Type.Value)
                                {
                                    return new PageMapping()
                                    {
                                        ID = -1,
                                        TypeID = 1,
                                        Path = absolutePath,
                                        Query = map.MappedQueryString.Replace("$n", itemId.ToString()),
                                        ItemID = itemId,
                                        IsActive = true,
                                        PageID = map.MappedPage
                                    };
                                }
                            }
                            else
                            {
                                return new PageMapping()
                                {
                                    ID = -1,
                                    TypeID = 1,
                                    Path = absolutePath,
                                    Query = map.MappedQueryString.Replace("$n", itemId.ToString()),
                                    ItemID = itemId,
                                    IsActive = true,
                                    PageID = map.MappedPage
                                };
                            }
                        }
                        //else
                        //    return map;
                    }
                }
                else
                {

                    Regex r = map.PathRegex;
                    if (r.IsMatch(absolutePath))
                    {
                        var mapping = new PageMapping()
                        {
                            ID = -1,
                            TypeID = 1,
                            Path = absolutePath,
                            Query = r.Replace(absolutePath, map.MappedQueryString),
                            ItemID = itemId,
                            IsActive = true,
                            PageID = map.MappedPage
                        };
                        return mapping;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the current mapping connection.
        /// </summary>
        /// <param name="typeName">Name of the type.</param>
        /// <returns></returns>
        public static Sushi.Mediakiwi.Framework.WimServerPortal GetCurrentMappingConnection(string typeName)
        {
            Sushi.Mediakiwi.Framework.WimServerConfiguration config = Sushi.Mediakiwi.Framework.WimServerConfiguration.GetConfig();
            string portalToFind = null;
            if (config != null && config.PortalCollection != null)
            {
                foreach (Sushi.Mediakiwi.Framework.WimServerMap map in config.MapCollection)
                {
                    if (typeName.StartsWith(map.NameSpace))
                    {
                        bool exclude = false;

                        if (!string.IsNullOrEmpty(map.Exclude))
                        {
                            foreach(string split in map.Exclude.Split(','))
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

                foreach (Sushi.Mediakiwi.Framework.WimServerPortal portal in config.PortalCollection)
                {
                    if (portal.Name == portalToFind)
                        return portal;
                }
            }
            return null;
        }

        public static Sushi.Mediakiwi.Framework.WimServerPortal GetCurrentMappingConnectionByName(string name)
        {
            if (string.IsNullOrEmpty(name)) return null;
            Sushi.Mediakiwi.Framework.WimServerConfiguration config = Sushi.Mediakiwi.Framework.WimServerConfiguration.GetConfig();
            string portalToFind = null;
            if (config != null && config.PortalCollection != null)
            {
                foreach (Sushi.Mediakiwi.Framework.WimServerMap map in config.MapCollection)
                {
                    if (map.Name == name)
                    {
                        portalToFind = map.Portal;
                        break;
                    }
                }

                foreach (Sushi.Mediakiwi.Framework.WimServerPortal portal in config.PortalCollection)
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
            Sushi.Mediakiwi.Framework.WimServerConfiguration config = Sushi.Mediakiwi.Framework.WimServerConfiguration.GetConfig();
            if (config != null && config.General != null)
            {
                foreach (Sushi.Mediakiwi.Framework.WimServerGeneral map in config.General)
                    if (map.Name == name) return map.Value;
            }
            return string.Empty;
        }

        /// <summary>
        /// Gets the portal.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static Sushi.Mediakiwi.Framework.WimServerPortal GetPortal(string name)
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
        public static Sushi.Mediakiwi.Framework.WimServerPortal GetPortal(string name, bool throwExceptionWhenNotFound)
        {
            if (string.IsNullOrEmpty(name))
                return null;

            Sushi.Mediakiwi.Framework.WimServerConfiguration config = Sushi.Mediakiwi.Framework.WimServerConfiguration.GetConfig();
            if (config != null && config.PortalCollection != null)
            {
                foreach (Sushi.Mediakiwi.Framework.WimServerPortal portal in config.PortalCollection)
                {
                    if (portal.Name == name)
                        return portal;
                }
            }
            if (throwExceptionWhenNotFound)
                throw new Exception("Could not find the portal");
            return null;
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
            get { return new TimeSpan(6,0,0); }
        }

        /// <summary>
        /// Gets the database dateTime from the environment settings.
        /// </summary>
        /// <value>
        /// The database date time.
        /// </value>
        public static DateTime DatabaseDateTime
        {
            get { return Sushi.Mediakiwi.Data.Environment.Current.CurrentTimezoneDateTime; }
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
            get { return "/";  }
        }

	}
}
