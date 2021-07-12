using Sushi.Mediakiwi.Data.MicroORM;
using Sushi.MicroORM.Mapping;
using System;
using System.Collections;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Data
{
    /// <summary>
    /// Wim environment settings.
    /// </summary>
    [DataMap(typeof(EnvironmentMap))]
    public class Environment : IEnvironment
    {
        internal class EnvironmentMap : DataMap<Environment>
        {
            public EnvironmentMap()
            {
                Table("wim_Environments");
                Id(x => x.ID, "Environment_Key").Identity();
                Map(x => x.DisplayName, "Environment_Title").Length(25);
                Map(x => x.Title, "Environment_Name").Length(50);
                Map(x => x.Timezone, "Environment_Timezone").Length(30);
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
            return await connector.FetchSingleAsync(filter).ConfigureAwait(false);
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
        public virtual string LogoHrefFull { get; set; }
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

        public virtual string Secret
        {
            get { return "secret"; }
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
            connector.Save(this);
            return true;
        }

        public async Task<bool> SaveAsync()
        {
            var connector = ConnectorFactory.CreateConnector<Environment>();
            await connector.SaveAsync(this).ConfigureAwait(false);
            return true;
        }
    }
}