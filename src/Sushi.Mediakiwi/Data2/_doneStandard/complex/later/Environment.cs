using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Sushi.Mediakiwi.Data.DalReflection;
using Sushi.Mediakiwi.Framework;
using System.Web;
using Sushi.Mediakiwi.Data.Parsers;
using Sushi.Mediakiwi.Data.Identity.Parsers;
using Sushi.Mediakiwi.Data.Identity;
using System.Collections.Concurrent;

namespace Sushi.Mediakiwi.Data
{
    /// <summary>
    /// Wim environment settings.
    /// </summary>
    [DatabaseTable("wim_Environments")]
    public class Environment : IEnvironment
    {

        static IEnvironmentParser _Parser;
        static IEnvironmentParser Parser
        {
            get
            {
                if (_Parser == null)
                {
                    _Parser = new EnvironmentParser();
                }
                return _Parser;
            }
        }

        //static bool _ContainerIsVerified;
        //static Hashtable _ContainerLookup;
        //static SimpleInjector.Container _Container;
        //static SimpleInjector.Container DependencyContainer
        //{
        //    get
        //    {
        //        if (_Container == null)
        //            _Container = new SimpleInjector.Container();

        //        if (_ContainerLookup == null)
        //            _ContainerLookup = new Hashtable();

        //        return _Container;
        //    }
        //    set
        //    {
        //        _Container = value;
        //    }
        //}

        

        /// <summary>
        /// Gets the logo light href full.
        /// </summary>
        /// <value>
        /// The logo light href full.
        /// </value>
        public virtual string LogoHrefFull
        {
            get
            {
                if (this.LogoLight.GetValueOrDefault() == 0)
                    return "https://mediakiwi.azureedge.net/7-4/images/MK_logo.png";
                //return Wim.Utility.AddApplicationPath("/testdrive/files/MK_logo.png", true);

                return Sushi.Mediakiwi.Data.Image.SelectOne(this.LogoLight.Value).FullPath;
            }
        }


        /// <summary>
        /// Gets the path.
        /// </summary>
        /// <value>The path.</value>
        public virtual string Path
        {
            get { return Wim.Utility.AddApplicationPath(this.RelativePath); }
        }

        public static bool AddDependencyIfNotExist<TService, TImplementation>()
            where TService : class
            where TImplementation : class, TService
        {
            return Parser.AddDependencyIfNotExist<TService, TImplementation>();
        }

        public static bool ContainsDependency<TService>()
        {
            return Parser.ContainsDependency<TService>();
        }

        public static ICollection<System.Type> RegisteredDependencies()
        {
            return Parser.RegisteredDependencies();
        }

        public static TService GetInstance<TService>() where TService : class
        {
            return Parser.GetInstance<TService>();
        }

        public static object GetInstance(Type type)
        {
            return Parser.GetInstance(type);
        }

        public static void AddPageModule<TPageModule>(TPageModule pageModule) where TPageModule : class, IPageModule
        {
            Parser.AddPageModule(pageModule);
        }

        public static ICollection<IPageModule> GetPageModules()
        {
            return Parser.GetPageModules();
        }


        #region REPLICATED TO Sushi.Mediakiwi.Data.Standard

        public static IEnvironment SelectOne()
        {
            return Parser.SelectOne();
        }

        /// <summary>
        /// SMTPs the client.
        /// </summary>
        /// <returns></returns>
        public virtual System.Net.Mail.SmtpClient SmtpClient()
        {
            return Parser.SmtpClient(this);
        }

        #region Properties

        static System.TimeZoneInfo m_CurrentTimeZone;
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
                            m_CurrentTimeZone = System.TimeZoneInfo.Local;
                        else
                            m_CurrentTimeZone = System.TimeZoneInfo.FindSystemTimeZoneById(this.Timezone);
                    }
                    catch (Exception ex)
                    {
                        //edit MV 2014-07-16: fix to ensure backwards compatability
                        if (string.Equals(Timezone, "Coordinated Universal Time", StringComparison.InvariantCultureIgnoreCase))
                        {
                            try
                            {
                                m_CurrentTimeZone = System.TimeZoneInfo.FindSystemTimeZoneById("Co-ordinated Universal Time");
                            }
                            catch
                            {
                                m_CurrentTimeZone = System.TimeZoneInfo.Local;
                                Sushi.Mediakiwi.Data.Notification.InsertOne("Timezone", ex);
                            }
                        }
                        else
                        {
                            m_CurrentTimeZone = System.TimeZoneInfo.Local;
                            Sushi.Mediakiwi.Data.Notification.InsertOne("Timezone", ex);
                        }
                    }
                }
                return System.TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, m_CurrentTimeZone);
            }
        }

        //static Environment m_Current;
        /// <summary>
        /// Gets the current environment.
        /// </summary>
        /// <value>
        /// The current.
        /// </value>
        [System.Xml.Serialization.XmlIgnore()]
        public static IEnvironment Current { get { return SelectOne(); } }

        [DatabaseColumn("Environment_Key", SqlDbType.Int, IsPrimaryKey = true)]
        public virtual int ID { get; set; }
        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        /// <value>The display name.</value>
        [Sushi.Mediakiwi.Framework.ContentListItem.TextField("Portal name", 25, true)]
        [DatabaseColumn("Environment_Title", SqlDbType.NVarChar, Length = 25, IsNullable = true)]
        public virtual string DisplayName { get; set; }
        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        [Sushi.Mediakiwi.Framework.ContentListItem.TextField("Application title", 50, true)]
        [DatabaseColumn("Environment_Name", SqlDbType.NVarChar, Length = 50)]
        public virtual string Title { get; set; }
        /// <summary>
        /// Gets or sets the timezone.
        /// </summary>
        /// <value>
        /// The timezone.
        /// </value>
        [Sushi.Mediakiwi.Framework.ContentListItem.Choice_Dropdown("Timezone", "AvailableTimeZones", true)]
        [DatabaseColumn("Environment_Timezone", SqlDbType.VarChar, Length = 30, IsNullable = true)]
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
                //string unEncoded = Utility.GlobalRegularExpression.Implement.CleanRelativePathSlash.Replace(tmp, "/");
                //return unEncoded;
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
        [Sushi.Mediakiwi.Framework.ContentListItem.TextField("Wim interface URL", 255, false, "What is the access URL of this Wim instance")]
        [DatabaseColumn("Environment_Url", SqlDbType.VarChar, Length = 255, IsNullable = true)]
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
        [DatabaseColumn("Environment_Path", SqlDbType.VarChar, Length = 50, IsNullable = true)]
        [Sushi.Mediakiwi.Framework.ContentListItem.TextField("Wim interface path", 50, false, "What is the relative path of the access page of this Wim instance")]
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
        [Sushi.Mediakiwi.Framework.ContentListItem.TextField("Binairy repository", 255, false)]
        [DatabaseColumn("Environment_Repository", SqlDbType.VarChar, Length = 255, IsNullable = true)]
        public virtual string Repository { get; set; }
        /// <summary>
        /// Default repository folder
        /// </summary>
        [DatabaseColumn("Environment_RepositoryFolder", SqlDbType.VarChar, Length = 255)]
        [Sushi.Mediakiwi.Framework.ContentListItem.TextField("Default repository folder", 255, true)]
        public virtual string RepositoryFolder { get; set; }
        /// <summary>
        /// Gets or sets the default site id.
        /// </summary>
        /// <value>The default site id.</value>
        [DatabaseColumn("Environment_Default_Site_Key", SqlDbType.Int, IsNullable = true)]
        [Sushi.Mediakiwi.Framework.ContentListItem.Choice_Dropdown("Default website", "Sites", false)]
        public virtual int? DefaultSiteID { get; set; }
        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <value>The password.</value>
        [Sushi.Mediakiwi.Framework.OnlyVisibleWhenTrue("IsEditMode")]
        [DatabaseColumn("Environment_Password", SqlDbType.VarChar, Length = 50, IsNullable = true)]
        [Sushi.Mediakiwi.Framework.ContentListItem.TextField("Password", 50, true, IsPasswordField = true)]
        public virtual string Password { get; set; }

        private string m_SubText1 = "Mail settings";
        /// <summary>
        /// Gets or sets the sub text1.
        /// </summary>
        /// <value>The sub text1.</value>
        [Sushi.Mediakiwi.Framework.ContentListItem.Section(null)]
        public virtual string zz_SubText1
        {
            get { return m_SubText1; }
            set { m_SubText1 = value; }
        }
        /// <summary>
        /// SMTP server, default value 127.0.0.1
        /// </summary>
        [DatabaseColumn("Environment_Smtp", SqlDbType.VarChar, Length = 250)]
        [Sushi.Mediakiwi.Framework.ContentListItem.TextField("SMTP Server", 250, true)]
        public virtual string SmtpServer { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [SMTP enable SSL].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [SMTP enable SSL]; otherwise, <c>false</c>.
        /// </value>
        
        [Sushi.Mediakiwi.Framework.ContentListItem.Choice_Checkbox("SMTP use SSL")]
        [DatabaseColumn("Environment_SmtpEnableSSL", SqlDbType.Bit, IsNullable = true)]
        public virtual bool SmtpEnableSSL { get; set; }

        /// <summary>
        /// SMTP Server Username
        /// </summary>
        [Sushi.Mediakiwi.Framework.ContentListItem.TextField("SMTP Username", 250, false)]
        [DatabaseColumn("Environment_SmtpUser", SqlDbType.VarChar, Length = 250, IsNullable = true)]
        public virtual string SmtpServerUser { get; set; }
        /// <summary>
        /// SMTP Server Password
        /// </summary>
        [Sushi.Mediakiwi.Framework.ContentListItem.TextField("SMTP Password", 250, false)]
        [DatabaseColumn("Environment_SmtpPass", SqlDbType.VarChar, Length = 250, IsNullable = true)]
        public virtual string SmtpServerPass { get; set; }
        /// <summary>
        /// Errormail from address.
        /// </summary>
        [Sushi.Mediakiwi.Framework.ContentListItem.TextField("Default mail address", 255, true)]
        [DatabaseColumn("Environment_DefaultMail", SqlDbType.VarChar, Length = 255, IsNullable = true)]
        public virtual string DefaultMailAddress { get; set; }
        /// <summary>
        /// Errormail to address.
        /// </summary>
        [Sushi.Mediakiwi.Framework.ContentListItem.TextField("Error mail address", 255, true)]
        [DatabaseColumn("Environment_ErrorMail", SqlDbType.VarChar, Length = 255, IsNullable = true)]
        public virtual string ErrorMailAddress { get; set; }
        private DateTime m_Created;
        /// <summary>
        /// Creation date and time of the environment.
        /// </summary>
        [DatabaseColumn("Environment_Created", SqlDbType.DateTime, IsNullable = true)]
        public virtual DateTime Created
        {
            get
            {
                if (this.m_Created == DateTime.MinValue) this.m_Created = Sushi.Mediakiwi.Data.Common.DatabaseDateTime;
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
        [DatabaseColumn("Environment_Update", SqlDbType.DateTime, IsOnlyRead = true)]
        public virtual DateTime UpdateInfo { get; set; }
        /// <summary>
        /// Current Wim version.
        /// </summary>
        [DatabaseColumn("Environment_Version", SqlDbType.Decimal)]
        public virtual decimal Version { get; set; }
       
        private string m_SubText2 = "Logo settings";
        /// <summary>
        /// Gets or sets the sub text1.
        /// </summary>
        /// <value>The sub text1.</value>
        [Sushi.Mediakiwi.Framework.ContentListItem.Section(null)]
        public virtual string zz_SubText2
        {
            get { return m_SubText2; }
            set { m_SubText2 = value; }
        }
        /// <summary>
        /// Gets or sets the logo light.
        /// </summary>
        /// <value>
        /// The logo light.
        /// </value>
        [Sushi.Mediakiwi.Framework.ContentListItem.Binary_Image("Logo", CanOnlyAdd = true)]
        [DatabaseColumn("Environment_LogoL", SqlDbType.Int, IsNullable = true)]
        public virtual int? LogoLight { get; set; }
        
        #endregion Properties

        public bool Save()
        {
            return Parser.Save(this);
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

        public static void RegisterDependencyContainer()
        {
            Parser.RegisterDependencyContainer();
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
                                Sushi.Mediakiwi.Data.Registry item = new Registry();
                                item.Name = registry;
                                item.Value = defaultValue;
                                item.Description = description;
                                item.Type = 1;
                                item.Save();
                                if (!Registry.ContainsKey(item.Name))
                                    Registry.Add(item.Name, item.Value);
                            }
                            catch(SqlException)
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
            return Parser.GetRegistryValue(this, registry, defaultValue);
        }

        Hashtable m_Registry;
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
                    foreach (Sushi.Mediakiwi.Data.Registry item in Sushi.Mediakiwi.Data.Registry.SelectAll())
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



        #endregion REPLICATED TO Sushi.Mediakiwi.Data.Standard

    }
}