using Sushi.MicroORM;
using Sushi.MicroORM.Mapping;
using System;
using System.Collections;
using System.Threading.Tasks;
using Sushi.Mediakiwi.Data.MircoORM;

namespace Sushi.Mediakiwi.Data
{
    /// <summary>
    /// Wim environment settings.
    /// </summary>
    [DataMap(typeof(EnvironmentMap))]
    public class Environment : IEnvironment
    {
        public class EnvironmentMap : DataMap<Environment>
        {
            public EnvironmentMap()
            {
                Table("wim_Environments");
                Id(x => x.ID, "Environment_Key").Identity();
                Map(x => x.DisplayName, "Environment_Title").Length(25);
                Map(x => x.Title, "Environment_Name").Length(50);
                Map(x => x.Timezone, "Environment_Timezone").Length(30);
                Map(x => x.Domain, "Environment_Url").Length(255);
                Map(x => x.RelativePath, "Environment_Path").Length(50);
                Map(x => x.Repository, "Environment_Repository").Length(255);
                Map(x => x.RepositoryFolder, "Environment_RepositoryFolder").Length(255);
                Map(x => x.DefaultSiteID, "Environment_Default_Site_Key");
                Map(x => x.Password, "Environment_Password").Length(50);
                Map(x => x.SmtpServer, "Environment_Smtp").Length(250);
                Map(x => x.SmtpEnableSSL, "Environment_SmtpEnableSSL");
                Map(x => x.SmtpServerUser, "Environment_SmtpUser").Length(250);
                Map(x => x.SmtpServerPass, "Environment_SmtpPass").Length(250);
                Map(x => x.DefaultMailAddress, "Environment_DefaultMail").Length(255);
                Map(x => x.ErrorMailAddress, "Environment_ErrorMail").Length(255);
                Map(x => x.Created, "Environment_Created");
                Map(x => x.UpdateInfo, "Environment_Update");
                Map(x => x.Version, "Environment_Version");
                Map(x => x.LogoLight, "Environment_LogoL");
            }
        }

        /// <summary>
        /// Selects a single Environment
        /// </summary>
        /// <returns></returns>
        public static IEnvironment SelectOne()
        {
            var connector = ConnectorFactory.CreateConnector<Environment>();
            var filter = connector.CreateDataFilter();
            return connector.FetchSingle(filter);
        }

        /// <summary>
        /// Selects a single Environment Async
        /// </summary>
        /// <returns></returns>
        public static async Task<IEnvironment> SelectOneAsync()
        {
            var connector = ConnectorFactory.CreateConnector<Environment>();
            var filter = connector.CreateDataFilter();
            return await connector.FetchSingleAsync(filter);
        }

        /// <summary>
        /// SMTPs the client.
        /// </summary>
        /// <returns></returns>
        public virtual System.Net.Mail.SmtpClient SmtpClient()
        {
            string[] split = SmtpServer.Split(':');
            System.Net.Mail.SmtpClient m_Client = new System.Net.Mail.SmtpClient();

            if (split.Length > 1)
                m_Client.Port = Convert.ToInt32(split[1]);
            m_Client.Host = split[0];

            if (!string.IsNullOrEmpty(SmtpServerUser))
                m_Client.Credentials = new System.Net.NetworkCredential(SmtpServerUser, SmtpServerPass);

            m_Client.EnableSsl = SmtpEnableSSL;

            return m_Client;
        }

        #region Properties

        /// <summary>
        /// Gets the current environment.
        /// </summary>
        /// <value>
        /// The current.
        /// </value>
        [System.Xml.Serialization.XmlIgnore()]
        public static IEnvironment Current
        {
            get { return SelectOne(); }
        }

        public virtual int ID { get; set; }

        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        /// <value>The display name.</value>
        public virtual string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        public virtual string Title { get; set; }

        /// <summary>
        /// Gets or sets the timezone.
        /// </summary>
        /// <value>
        /// The timezone.
        /// </value>
        public virtual string Timezone { get; set; }

        /// <summary>
        /// Gets the URL.
        /// </summary>
        /// <value>The URL.</value>
        public virtual string Url
        {
            get
            {
                string tmp = string.Concat(this.Domain, this.RelativePath);
                return tmp;
            }
        }

        public virtual string Secret
        {
            get { return "secret"; }
        }

        private string m_Domain;

        /// <summary>
        /// Main Wim access URL. Used for reference in mail.
        /// </summary>
        public virtual string Domain
        {
            get
            {
                if (m_Domain != null && m_Domain.Contains("wim.ashx"))
                    m_Domain = m_Domain.Replace("wim.ashx", string.Empty);
                return m_Domain;
            }
            set { m_Domain = value; }
        }

        private string m_RelativePath;

        /// <summary>
        /// Main Wim access URL.
        /// </summary>
        /// <value>The relative path.</value>
        public virtual string RelativePath
        {
            get
            {
                if (string.IsNullOrEmpty(m_RelativePath))
                    m_RelativePath = "/repository/wim/portal.ashx";
                return m_RelativePath;
            }
            set { m_RelativePath = value; }
        }

        /// <summary>
        /// Main binary repository. Only required in mirroring situations.
        /// </summary>
        public virtual string Repository { get; set; }

        /// <summary>
        /// Default repository folder
        /// </summary>
        public virtual string RepositoryFolder { get; set; }

        /// <summary>
        /// Gets or sets the default site id.
        /// </summary>
        /// <value>The default site id.</value>
        public virtual int? DefaultSiteID { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <value>The password.</value>
        public virtual string Password { get; set; }

        /// <summary>
        /// SMTP server, default value 127.0.0.1
        /// </summary>
        public virtual string SmtpServer { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [SMTP enable SSL].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [SMTP enable SSL]; otherwise, <c>false</c>.
        /// </value>
        public virtual bool SmtpEnableSSL { get; set; }

        /// <summary>
        /// SMTP Server Username
        /// </summary>
        public virtual string SmtpServerUser { get; set; }

        /// <summary>
        /// SMTP Server Password
        /// </summary>
        public virtual string SmtpServerPass { get; set; }

        /// <summary>
        /// Errormail from address.
        /// </summary>
        public virtual string DefaultMailAddress { get; set; }

        /// <summary>
        /// Errormail to address.
        /// </summary>
        public virtual string ErrorMailAddress { get; set; }

        private DateTime m_Created;

        /// <summary>
        /// Creation date and time of the environment.
        /// </summary>
        public virtual DateTime Created
        {
            get
            {
                if (this.m_Created == DateTime.MinValue)
                    this.m_Created = Common.DatabaseDateTime;
                return m_Created;
            }
            set { m_Created = value; }
        }

        /// <summary>
        /// Gets or sets the update information.
        /// </summary>
        /// <value>
        /// The update information.
        /// </value>
        public virtual DateTime UpdateInfo { get; set; }

        /// <summary>
        /// Current Wim version.
        /// </summary>
        public virtual decimal Version { get; set; }

        /// <summary>
        /// Gets or sets the logo light.
        /// </summary>
        /// <value>
        /// The logo light.
        /// </value>
        public virtual int? LogoLight { get; set; }

        private static TimeZoneInfo m_CurrentTimeZone;

        /// <summary>
        /// Gets the current timezone date time.
        /// </summary>
        /// <value>
        /// The current timezone date time.
        /// </value>
        public virtual DateTime CurrentTimezoneDateTime
        {
            get
            {
                if (m_CurrentTimeZone == null)
                {
                    try
                    {
                        if (string.IsNullOrEmpty(Timezone))
                            m_CurrentTimeZone = TimeZoneInfo.Local;
                        else
                            m_CurrentTimeZone = TimeZoneInfo.FindSystemTimeZoneById(Timezone);
                    }
                    catch (Exception ex)
                    {
                        //edit MV 2014-07-16: fix to ensure backwards compatability
                        if (string.Equals(Timezone, "Coordinated Universal Time", StringComparison.InvariantCultureIgnoreCase))
                        {
                            try
                            {
                                m_CurrentTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Co-ordinated Universal Time");
                            }
                            catch
                            {
                                m_CurrentTimeZone = TimeZoneInfo.Local;
                                Notification.InsertOne("Timezone", ex.ToString());
                            }
                        }
                        else
                        {
                            m_CurrentTimeZone = TimeZoneInfo.Local;
                            Notification.InsertOne("Timezone", ex.ToString());
                        }
                    }
                }
                return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, m_CurrentTimeZone);
            }
        }

        #endregion Properties

        public bool Save()
        {
            var connector = ConnectorFactory.CreateConnector<Environment>();
            try
            {
                connector.Save(this);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> SaveAsync()
        {
            var connector = ConnectorFactory.CreateConnector<Environment>();
            try
            {
                await connector.SaveAsync(this);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #region registry

        /// <summary>
        /// Gets the <see cref="System.String"/> with the specified registry. OnError returns String.Empty!
        /// </summary>
        /// <value></value>
        public virtual string this[string registry]
        {
            get
            {
                return this[registry, false, null, null];
            }
        }

        private Object registerLock = new Object();

        /// <summary>
        ///
        /// </summary>
        /// <param name="registry"></param>
        /// <param name="ifNotPresentAdd"></param>
        /// <param name="defaultValue"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        public virtual string this[string registry, bool ifNotPresentAdd, string defaultValue, string description]
        {
            get
            {
                if (Registry != null)
                {
                    lock (registerLock)
                    {
                        if (Registry.ContainsKey(registry))
                        {
                            object candidate = Registry[registry];
                            if (candidate == null) return string.Empty;
                            if (candidate.ToString().Trim().Length == 0) return string.Empty;
                            return candidate.ToString();
                        }
                        else if (ifNotPresentAdd)
                        {
                            try
                            {
                                Registry item = new Registry();
                                item.Name = registry;
                                item.Value = defaultValue;
                                item.Description = description;
                                item.Type = 1;
                                item.Save();
                                if (!Registry.ContainsKey(item.Name))
                                    Registry.Add(item.Name, item.Value);
                            }
                            catch (Exception)
                            {
                                // Could go wrong.
                            }
                        }
                    }
                }
                return string.Empty;
            }
        }

        /// <summary>
        /// Gets the registry value.
        /// </summary>
        /// <param name="registry">The registry.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
        public virtual string GetRegistryValue(string registry, string defaultValue)
        {
            string candidate = this[registry];
            if (string.IsNullOrEmpty(candidate))
                return defaultValue;

            return candidate;
        }

        private Hashtable m_Registry;

        /// <summary>
        /// Gets or sets the registry.
        /// </summary>
        /// <value>
        /// The registry.
        /// </value>
        protected internal virtual Hashtable Registry
        {
            get
            {
                if (m_Registry == null)
                {
                    if (this.ID == 0)
                        return null;

                    m_Registry = new Hashtable();
                    foreach (Registry item in Data.Registry.SelectAll())
                    {
                        if (!m_Registry.ContainsKey(item.Name))
                            m_Registry.Add(item.Name, item.Value);
                    }
                }
                return m_Registry;
            }
            set
            {
                m_Registry = value;
            }
        }

        #endregion registry
    }
}