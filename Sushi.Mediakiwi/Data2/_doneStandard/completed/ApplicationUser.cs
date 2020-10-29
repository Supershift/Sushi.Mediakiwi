using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Security.Principal;
using System.Web;
using System.Web.Security;
using Sushi.Mediakiwi.Data.DalReflection;
using Sushi.Mediakiwi.Data.Identity;
using Sushi.Mediakiwi.Data.Parsers;

namespace Sushi.Mediakiwi.Data
{
    /// <summary>
    /// Represents a ApplicationUser entity.
    /// </summary>
    [DatabaseTable("wim_Users", Join = "left join wim_Roles on Role_Key = User_Role_Key", Order = "User_Displayname ASC")]
    public class ApplicationUser : IApplicationUser
    {

        static IApplicationUserParser _Parser;
        static IApplicationUserParser Parser
        {
            get
            {
                if (_Parser == null)
                    _Parser = Environment.GetInstance<IApplicationUserParser>();
                return _Parser;
            }
        }


        #region REPLICATED TO Sushi.Mediakiwi.Data.Standard

        /// <summary>
        /// Gets or sets a value indicating whether to show the environment in full width mode
        /// </summary>
        public bool ShowFullWidth
        {
            get
            {
                if (this.Data == null) return false;
                return this.Data["FW"].ParseBoolean(false);
            }
            set
            {
                if (this.Data != null)
                    this.Data.Apply("FW", value);
            }
        }


        /// <summary>
        /// Gets or sets a value indicating whether [show site navigation].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [show site navigation]; otherwise, <c>false</c>.
        /// </value>
        public bool ShowSiteNavigation
        {
            get
            {
                if (this.Data == null) return false;
                return this.Data["SiteNav"].ParseBoolean(false);
            }
            set
            {
                if (this.Data != null)
                    this.Data.Apply("SiteNav", value);
            }
        }

        /// <summary>
        /// Only posible in combination with "Is Developer"
        /// </summary>
        public bool ShowHidden
        {
            get
            {
                if (!this.IsDeveloper)
                    return false;
                if (this.Data == null) return false;
                return this.Data["ShowHidden"].ParseBoolean(false);
            }
            set
            {
                if (this.Data != null)
                {
                    if (value == false)
                        this.Data.Apply("ShowHidden", null);
                    else
                        this.Data.Apply("ShowHidden", value);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is developer.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is developer; otherwise, <c>false</c>.
        /// </value>
        public bool IsDeveloper
        {
            get
            {
                if (this.Data == null) return false;
                return this.Data["IsDeveloper"].ParseBoolean(false);
            }
            set
            {
                if (this.Data != null)
                    this.Data.Apply("IsDeveloper", value);
            }
        }

        /// <summary>
        /// Applies the password.
        /// </summary>
        /// <param name="password">The password.</param>
        public void ApplyPassword(string password)
        {
            this.Password = Wim.Utility.HashStringByMD5(string.Concat(this.Name, password));
            this.Type = 1;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationUser"/> class.
        /// </summary>
        public ApplicationUser()
        {
         
        }


        /// <summary>
        /// selects the one.
        /// </summary>
        /// <param name = "id" > the id.</param>
        /// <returns></returns>
        public static IApplicationUser SelectOne(int ID)
        {
            return Parser.SelectOne(ID);
        }

        const string CACHEKEYPREFIX = "Data.User.";

        /// <summary>
        /// Selects the one.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <returns></returns>
        public static IApplicationUser SelectOne(string username)
        {
            return Parser.SelectOne(username);
        }

        public static IApplicationUser SelectOneByEmail(string email)
        {
            return Parser.SelectOneByEmail(email);
        }

        public static IApplicationUser SelectOne(string username, string password)
        {
            return Parser.SelectOne(username, password);
        }

        /// <summary>
        /// Selects the one.
        /// </summary>
        /// <param name="applicationUserGUID">The application user GUID.</param>
        /// <returns></returns>
        public static IApplicationUser SelectOne(Guid applicationUserGUID)
        {
            return Parser.SelectOne(applicationUserGUID);
        }

        /// <summary>
        /// Selects all.
        /// </summary>
        /// <returns></returns>
        public static IApplicationUser[] SelectAll()
        {
            return Parser.SelectAll();
        }

        /// <summary>
        /// Selects all.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="role">The role.</param>
        /// <returns></returns>
        public static IApplicationUser[] SelectAll(string username, int role)
        {
            return Parser.SelectAll(username, role);
        }

        /// <summary>
        /// Selects all active application Users for a specific role.
        /// </summary>
        /// <param name="role">The role.</param>
        /// <returns></returns>
        public static IApplicationUser[] SelectAll(int? role)
        {
            return SelectAll(role, true);
        }

        /// <summary>
        /// Selects all.
        /// </summary>
        /// <param name="role">The role.</param>
        /// <param name="onlyReturnActive">if set to <c>true</c> [only return active].</param>
        /// <returns></returns>
        public static IApplicationUser[] SelectAll(int? role, bool onlyReturnActive)
        {
            return Parser.SelectAll(role, onlyReturnActive);
        }

        /// <summary>
        /// Determines whether [has user name] [the specified username].
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="ignoreApplicationUserID">The ignore application user ID.</param>
        /// <returns>
        /// 	<c>true</c> if [has user name] [the specified username]; otherwise, <c>false</c>.
        /// </returns>
        public static bool HasUserName(string username, int? ignoreApplicationUserID)
        {
            IApplicationUser tmp = Parser.SelectOneByUserName(username, ignoreApplicationUserID);

            if (tmp == null || tmp.IsNewInstance)
                return false;
            return true;
        }

        /// <summary>
        /// Determines whether [has user name] [the specified username].
        /// </summary>
        /// <param name="username">The username.</param>
        /// <returns>
        ///   <c>true</c> if [has user name] [the specified username]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasUserName(string username)
        {
            return HasUserName(username, this.ID);
        }

        public bool HasEmail(string email)
        {
            return HasEmail(email, this.ID);
        }

        /// <summary>
        /// Determines whether the specified email has email.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <param name="ignoreApplicationUserID">The ignore application user ID.</param>
        /// <returns>
        /// 	<c>true</c> if the specified email has email; otherwise, <c>false</c>.
        /// </returns>
        public static bool HasEmail(string email, int? ignoreApplicationUserID)
        {
            IApplicationUser tmp = Parser.SelectOne(email, ignoreApplicationUserID);

            if (tmp == null || tmp.IsNewInstance)
                return false;
            return true;
        }

        /// <summary>
        /// Selects the specified emailaddress.
        /// </summary>
        /// <param name="emailaddress">The emailaddress.</param>
        /// <returns></returns>
        public static IApplicationUser Select(string emailaddress)
        {
            var tmp = Parser.SelectOneByEmail(emailaddress);

            if (tmp == null || tmp.IsNewInstance || !tmp.isActive)
                return new ApplicationUser();

            return tmp;
        }

        public bool Delete()
        {
            return Parser.Delete(this);
        }

        /// <summary>
        /// Gets a value indicating whether this instance has a visitor reference (cookie).
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has visitor reference; otherwise, <c>false</c>.
        /// </value>
        public bool HasVisitorReference
        {
            get
            {
                if (this.VisitorReference == Guid.Empty)
                    return false;
                return true;
            }
        }

        /// <summary>
        /// By default the profile reference is NOT stored in a Cookie. Use Save(true) for saving a cookie.
        /// </summary>
        /// <returns></returns>
        public bool Save()
        {
            if (string.IsNullOrEmpty(Name))
                return false;

            if (HasUserName(this.Name, ID))
                throw new Exception("The applied username already exists");

            if (HasEmail(this.Email, ID))
                throw new Exception("The applied email already exists");

            bool save = Parser.Save(this);
            return save;
        }

        internal void SaveUpdated()
        {
            LastLoggedVisit = Sushi.Mediakiwi.Data.Common.DatabaseDateTime;
            bool save = Parser.Save(this);
        }
        
        bool m_ShowDetailView;
        /// <summary>
        /// Gets or sets a value indicating whether [show detail view].
        /// </summary>
        /// <value><c>true</c> if [show detail view]; otherwise, <c>false</c>.</value>
        [DatabaseColumn("User_Detailview", SqlDbType.Bit, IsNullable = false)]
        public bool ShowDetailView
        {
            get
            {
                if (ApplicationUserLogic.ShowNewDesignForUser(this))
                {
                    if (ApplicationUserLogic.ShowNewDesign2ForUser(this))
                        return true;

                    return false;
                }
                return m_ShowDetailView;
            }
            set
            {
                if (m_ShowDetailView != value)
                    m_ShowDetailView = value;
            }
        }

        bool m_ShowTranslationView;
        /// <summary>
        /// Gets or sets a value indicating whether [show translation view].
        /// </summary>
        /// <value><c>true</c> if [show translation view]; otherwise, <c>false</c>.</value>
        [DatabaseColumn("User_Translationview", SqlDbType.Bit, IsNullable = false)]
        public bool ShowTranslationView
        {
            get { return m_ShowTranslationView; }
            set
            {
                if (m_ShowTranslationView != value)
                    m_ShowTranslationView = value;
            }
        }

        string m_Attribute1 = "ApplicationUser";
        string m_Attribute2 = "Channel";
        string m_Attribute3 = "Portal";
        string m_Attribute4 = "TimeStamp";
        string m_Attribute5 = "ApplicationUserID";

        string m_Ticket;
        Data.Environment m_Environment;
        internal string Ticket
        {
            get
            {
                if (string.IsNullOrEmpty(m_Ticket))
                {
                    this.m_Ticket = string.Concat("w_", Sushi.Mediakiwi.Data.Environment.Current.Title.Replace(" ", "_"));
                }
                return m_Ticket;
            }
        }

        public bool IsNewInstance { get { return ID == 0; } }

        Guid m_VisitorReference;
        /// <summary>
        /// Gets the externally (cookie) stored profile reference.
        /// </summary>
        /// <value>The profile reference.</value>
        public Guid VisitorReference
        {
            get
            {
                //if (m_VisitorReference == Guid.Empty)
                //    SetInfoFromCookie();
                return m_VisitorReference;
            }
        }

        int m_ID;
        /// <summary>
        /// Gets or sets the ID.
        /// </summary>
        /// <value>The ID.</value>
        [DatabaseColumn("User_Key", SqlDbType.Int, IsPrimaryKey = true)]
        public int ID
        {
            get { return m_ID; }
            set
            {
                if (m_ID != value)
                    m_ID = value;
            }
        }

        private Guid m_GUID;
        /// <summary>
        /// Gets or sets the GUID.
        /// </summary>
        /// <value>The GUID.</value>
        [DatabaseColumn("User_Guid", SqlDbType.UniqueIdentifier, IsMigrationKey = true)]
        public Guid GUID
        {
            get
            {
                if (this.m_GUID == Guid.Empty) this.m_GUID = Guid.NewGuid();
                return m_GUID;
            }
            set { m_GUID = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        string m_Name;
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        [DatabaseColumn("User_Name", SqlDbType.NVarChar, Length = 50)]
        public string Name
        {
            get { return m_Name; }
            set
            {
                if (m_Name != value)
                    m_Name = value;
            }
        }

        private string m_Displayname;
        /// <summary>
        /// Gets or sets the displayname.
        /// </summary>
        /// <value>The displayname.</value>
        [DatabaseColumn("User_Displayname", SqlDbType.NVarChar, Length = 50)]
        public string Displayname
        {
            get { return m_Displayname; }
            set
            {
                if (m_Displayname != value)
                    m_Displayname = value;
            }
        }

        string m_RoleName;
        /// <summary>
        /// Gets or sets the name of the role.
        /// </summary>
        /// <value>The name of the role.</value>
        [DatabaseColumn("Role_Name", SqlDbType.NVarChar, Length = 50, IsOnlyRead = true, IsNullable = true)]
        public string RoleName
        {
            get { return m_RoleName; }
            set { m_RoleName = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <summary>
        /// Gets or sets the role.
        /// </summary>
        /// <value>The role.</value>
        [DatabaseColumn("User_Role_Key", SqlDbType.Int, IsNullable = true)]
        public int RoleID { get; set; }

        /// <summary>
        /// Roles this instance.
        /// </summary>
        /// <returns></returns>
        public IApplicationRole Role()
        {
            return ApplicationRole.SelectOne(RoleID);
        }


        int m_Language;
        /// <summary>
        /// Gets or sets the language.
        /// </summary>
        /// <value>The language.</value>
        [DatabaseColumn("User_Language", SqlDbType.Int)]
        public int Language
        {
            get
            {
                return m_Language;
            }
            set
            {
                if (m_Language != value)
                    m_Language = value;
            }
        }

        /// <summary>
        /// Gets the language culture.
        /// </summary>
        /// <value>The language culture.</value>
        public string LanguageCulture
        {
            get
            {
                if (Language == 2)
                    return "nl-NL";
                return "en-GB";
            }
        }

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        /// <value>The email.</value>
        [DatabaseColumn("User_Email", SqlDbType.NVarChar, Length = 255, IsNullable = true)]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the network identification.
        /// </summary>
        /// <value>The network identification.</value>
        [DatabaseColumn("User_Network", SqlDbType.VarChar, Length = 50, IsNullable = true)]
        public string NetworkIdentification { get; set; }

        /// <summary>
        /// Gets or sets the network identification.
        /// </summary>
        /// <value>
        /// The network identification.
        /// </value>
        [DatabaseColumn("User_Reset", SqlDbType.UniqueIdentifier, IsNullable = true)]
        public Guid? ResetKey { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        [DatabaseColumn("User_Type", SqlDbType.Int, IsNullable = true)]
        public int Type { get; set; }

        /// <summary>
        /// 
        /// </summary>
        string m_Password;
        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <value>The password.</value>
        [DatabaseColumn("User_Password", SqlDbType.NVarChar, Length = 50, IsNullable = true)]
        public string Password
        {
            get { return m_Password; }
            set
            {
                if (m_Password != value)
                    m_Password = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        bool m_isActive;
        /// <summary>
        /// Gets or sets a value indicating whether this instance is active.
        /// </summary>
        /// <value><c>true</c> if this instance is active; otherwise, <c>false</c>.</value>
        [DatabaseColumn("User_IsActive", SqlDbType.Bit)]
        public bool isActive
        {
            get { return m_isActive; }
            set
            {
                if (m_isActive != value)
                    m_isActive = value;
            }
        }

        DateTime m_Created;
        /// <summary>
        /// Gets or sets the created.
        /// </summary>
        /// <value>The created.</value>
        [DatabaseColumn("User_Created", SqlDbType.DateTime, IsNullable = true)]
        public DateTime Created
        {
            get
            {
                if (this.m_Created == DateTime.MinValue) this.m_Created = Sushi.Mediakiwi.Data.Common.DatabaseDateTime;
                return m_Created;
            }
            set { m_Created = value; }
        }

        //DateTime m_LastLoggedVisit;
        /// <summary>
        /// Gets or sets the last logged visit.
        /// </summary>
        /// <value>The last logged visit.</value>
        [DatabaseColumn("User_LastLoggedVisit", SqlDbType.DateTime, IsNullable = true)]
        public DateTime LastLoggedVisit { get; set; }

        bool m_RememberMe;
        /// <summary>
        /// Gets or sets a value indicating whether [remember me].
        /// </summary>
        /// <value><c>true</c> if [remember me]; otherwise, <c>false</c>.</value>
        [DatabaseColumn("User_RememberMe", SqlDbType.Bit)]
        public bool RememberMe
        {
            get { return m_RememberMe; }
            set
            {
                if (m_RememberMe != value)
                    m_RememberMe = value;
            }
        }

        Sushi.Mediakiwi.Data.CustomData m_Data;
        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        /// <value>The data.</value>
        [DatabaseColumn("User_Data", SqlDbType.Xml, IsNullable = true)]
        public Sushi.Mediakiwi.Data.CustomData Data
        {
            get
            {
                if (m_Data == null)
                    m_Data = new CustomData();
                return m_Data;
            }
            set { m_Data = value; }
        }


        #endregion MOVED TO Sushi.Mediakiwi.Data.Standard

        #region COMMENTED, not used anywhere

        //internal static IApplicationUser Get(string username, string password)
        //{
        //    if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        //        return new ApplicationUser();

        //    var tmp = Parser.SelectOne(username, password);

        //    if (!tmp.IsNewInstance && !tmp.isActive)
        //        tmp = new ApplicationUser();

        //    using (Utilities.Authentication auth = new Utilities.Authentication())
        //    {
        //        auth.EncryptionPassword = Sushi.Mediakiwi.Data.Common.EncryptionKey;

        //        if (auth.Encrypt(password) == tmp.Password)
        //            return tmp;
        //    }
        //    return new ApplicationUser();
        //}


        ///// <summary>
        ///// Gets the alternative name suggestion.
        ///// </summary>
        ///// <param name="name">The name.</param>
        ///// <param name="ignoreApplicationUserID">The ignore application user ID.</param>
        ///// <returns></returns>
        //public static string GetAlternativeNameSuggestion(string name, int? ignoreApplicationUserID)
        //{
        //    string candidate = name;
        //    int count = 0;
        //    while (HasUserName(candidate, ignoreApplicationUserID.GetValueOrDefault()))
        //    {
        //        count++;
        //        candidate = string.Concat(name, count);
        //    }
        //    return candidate;
        //}

        ///// <summary>
        ///// Remove the profile cookie
        ///// </summary>
        ///// <returns></returns>
        //public static bool Clear()
        //{
        //    ApplicationUser tmp = new ApplicationUser();
        //    tmp.RemoveCookie();
        //    return true;
        //}

        ///// <summary>
        ///// Removes the cookie.
        ///// </summary>
        //internal void RemoveCookie()
        //{
        //    using (Wim.Utilities.Authentication auth = new Wim.Utilities.Authentication())
        //        auth.RemoveCustomTicket(Ticket);
        //}


        #endregion COMMENTED, not used anywhere

        #region MOVED to EXTENSION / LOGIC

        //bool m_SkinRequested;
        ////string m_Skin;
        ///// <summary>
        ///// Gets or sets the skin.
        ///// </summary>
        ///// <value>The skin.</value>
        ////[DatabaseColumn("User_Skin", SqlDbType.NVarChar, Length = 25, IsNullable = true)]
        //public string Skin
        //{
        //    get
        //    {
        //        return Sushi.Mediakiwi.Data.Common.CurrentPortal.DefaultSkin;
        //    }
        //}

        //public void SendLoginMail()
        //{
        //    var userData = Sushi.Mediakiwi.Data.ComponentList.SelectOne("Sushi.Mediakiwi.AppCentre.Data.Implementation.User");
        //    string mail_Title = userData.Settings["Mail_Title"].Value;
        //    string mail_Intro = userData.Settings["Mail_Intro"].Value;

        //    if (string.IsNullOrEmpty(mail_Intro))
        //        mail_Intro = "Dear [name],<br><br>We have created an account for you to login. Please visit the following URL and apply your password using the credentials as noted below:<br><br>[url]<br><br><b>Your personal credentials</b>:<br><br>[credentials]";

        //    string url;
        //    ResetPassword(this, out url);

        //    string body = string.Format(@"Username: {0}<br/>Emailadres: {1}<br/>"
        //        , this.Name
        //        , this.Email);

        //    Wim.Utilities.Mail.Send(new System.Net.Mail.MailAddress(this.Email, this.Displayname),
        //        string.IsNullOrEmpty(mail_Title) ? "Your user account" : mail_Title,
        //        string.IsNullOrEmpty(mail_Intro) ? body
        //            : mail_Intro
        //                .Replace("[credentials]", body)
        //                .Replace("[name]", this.Displayname)
        //                .Replace("[login]", this.Name)
        //                .Replace("[email]", this.Email)
        //                .Replace("[url]", string.Format("<a href=\"{0}\">{0}</a>", url))
        //                .Replace("http://url", string.Format("{0}", url))
        //            ,
        //        url);
        //}

        ///// <summary>
        ///// Sends the login mail.
        ///// </summary>
        //public void ExtractLoginMailBody(out string subject, out string body, out string url)
        //{
        //    var userData = Sushi.Mediakiwi.Data.ComponentList.SelectOne("Sushi.Mediakiwi.AppCentre.Data.Implementation.User");
        //    subject = userData.Settings["Mail_Title"].Value;
        //    body = userData.Settings["Mail_Intro"].Value;

        //    if (string.IsNullOrWhiteSpace(body))
        //        body = "Dear [name],<br><br>We have created an account for you to login. Please visit the following URL and apply your password using the credentials as noted below:<br><br>[url]<br><br><b>Your personal credentials</b>:<br><br>[credentials]";

        //    this.ResetKey = Guid.NewGuid();
        //    this.Save();

        //    using (Wim.Utilities.Authentication auth = new Wim.Utilities.Authentication())
        //    {
        //        auth.EncryptionPassword = "urlinfo";
        //        string urlAddition = string.Concat("&u=", HttpUtility.UrlEncode(auth.Encrypt(this.Name)));

        //        string wimPath = Sushi.Mediakiwi.Data.Environment.Current.RelativePath;


        //        url = Wim.Utility.AddApplicationPath(string.Concat(wimPath, "?reset=", this.ResetKey, urlAddition), true);

        //        string credentials = string.Format(@"Username: {0}<br/>Emailadres: {1}<br/>"
        //            , this.Name
        //            , this.Email);

        //        body = body
        //            .Replace("[credentials]", credentials)
        //            .Replace("[name]", this.Displayname)
        //            .Replace("[login]", this.Name)
        //            .Replace("[email]", this.Email);
        //        //;
        //        //.Replace("http://url", string.Format("{0}", url));
        //    }
        //}


        //public void SendForgotPassword()
        //{
        //    var userData = Sushi.Mediakiwi.Data.ComponentList.SelectOne("Sushi.Mediakiwi.AppCentre.Data.Implementation.User");
        //    string mail_Title = userData.Settings["Mail_ForgotTitle"].Value;
        //    string mail_Intro = userData.Settings["Mail_ForgotIntro"].Value;

        //    if (string.IsNullOrEmpty(mail_Intro))
        //        mail_Intro = "Dear [name],<br><br>You have requested a password reset through the \"forgotten my password\" page. Please visit the following URL and (re)apply your password:<br><br>[url]";

        //    this.ResetKey = Guid.NewGuid();
        //    this.Save();

        //    string wimPath = Sushi.Mediakiwi.Data.Environment.Current.RelativePath;
        //    //if (!Wim.CommonConfiguration.FORCE_NEWSTYLE && this.ShowNewDesign && !string.IsNullOrEmpty(Wim.CommonConfiguration.NEWSTYLE_LOGIN_PATH))
        //    //{
        //    //    wimPath = Wim.CommonConfiguration.NEWSTYLE_LOGIN_PATH;
        //    //}

        //    string url;
        //    ResetPassword(this, out url);

        //    string body = string.Format(@"Username: {0}<br/>Emailadres: {1}<br/>"
        //        , this.Name
        //        , this.Email);

        //    string host = Wim.Utility.GetCurrentHost(true);
        //    //string url = string.Concat(host, Sushi.Mediakiwi.Data.Environment.Current.RelativePath, urlAddition);

        //    Wim.Utilities.Mail.Send(new System.Net.Mail.MailAddress(this.Email, this.Displayname),
        //        string.IsNullOrEmpty(mail_Title) ? "Forgotten password" : mail_Title,
        //        string.IsNullOrEmpty(mail_Intro) ? body
        //            : mail_Intro
        //                .Replace("[credentials]", body)
        //                .Replace("[name]", this.Displayname)
        //                .Replace("[login]", this.Name)
        //                .Replace("[email]", this.Email)
        //                .Replace("[url]", string.Format("<a href=\"{0}\">{0}</a>", url))
        //                .Replace("http://url", string.Format("{0}", url)),
        //        url);
        //}


        //internal static bool IsValid(IApplicationUser user, string password)
        //{
        //    if (user.Type == 0)
        //    {
        //        using (Utilities.Authentication auth = new Utilities.Authentication())
        //        {
        //            auth.EncryptionPassword = Sushi.Mediakiwi.Data.Common.EncryptionKey;
        //            if (auth.Encrypt(password) == user.Password)
        //                return true;
        //        }
        //    }
        //    else if (user.Type == 1)
        //    {
        //        string candidate = Wim.Utility.HashStringByMD5(string.Concat(user.Name, password));
        //        if (candidate.Equals(user.Password))
        //            return true;
        //    }
        //    return false;
        //}

        //internal static IApplicationUser Get(string username, string password)
        //{
        //    if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        //        return new ApplicationUser();

        //    var tmp = Parser.SelectOne(username, password);

        //    if (!tmp.IsNewInstance && !tmp.isActive)
        //        tmp = new ApplicationUser();

        //    using (Utilities.Authentication auth = new Utilities.Authentication())
        //    {
        //        auth.EncryptionPassword = Sushi.Mediakiwi.Data.Common.EncryptionKey;

        //        if (auth.Encrypt(password) == tmp.Password)
        //            return tmp;
        //    }
        //    return new ApplicationUser();
        //}

        //internal static IApplicationUser Store(IApplicationUser tmp, string password, bool shouldRememberMe)
        //{
        //    string username = tmp.Name;
        //    if (tmp.Type == 0)
        //    {
        //        using (Utilities.Authentication auth = new Utilities.Authentication())
        //        {
        //            auth.EncryptionPassword = Sushi.Mediakiwi.Data.Common.EncryptionKey;

        //            if (auth.Encrypt(password) == tmp.Password)
        //            {
        //                tmp.LastLoggedVisit = Sushi.Mediakiwi.Data.Common.DatabaseDateTime;
        //                tmp.Save(shouldRememberMe);
        //            }
        //            else
        //            {
        //                tmp = new ApplicationUser();
        //                //  Insert a notification
        //                Sushi.Mediakiwi.Data.Notification.InsertOne(Sushi.Mediakiwi.Data.Notification.Tags.InternalWimError, string.Format("Someone tried to login in with wrong credentials (username: {0})", username));
        //            }
        //        }
        //    }
        //    else if (tmp.Type == 1)
        //    {
        //        string candidate = Wim.Utility.HashStringByMD5(string.Concat(tmp.Name, password));
        //        if (candidate.Equals(tmp.Password))
        //        {
        //            tmp.LastLoggedVisit = Sushi.Mediakiwi.Data.Common.DatabaseDateTime;

        //            var visitor = Visitor.Select();
        //            visitor.LastLoggedApplicationUserVisit = Sushi.Mediakiwi.Data.Common.DatabaseDateTime;
        //            visitor.Save();

        //            tmp.Save(shouldRememberMe);
        //        }
        //        else
        //        {
        //            tmp = new ApplicationUser();
        //            //  Insert a notification
        //            Sushi.Mediakiwi.Data.Notification.InsertOne(Sushi.Mediakiwi.Data.Notification.Tags.InternalWimError, string.Format("Someone tried to login in with wrong credentials (username: {0})", username));
        //        }
        //    }
        //    return tmp;
        //}


        ///// <summary>
        ///// Selects the specified username. This method also autosaves the application user when found
        ///// </summary>
        ///// <param name="username">The username.</param>
        ///// <param name="password">The password.</param>
        ///// <param name="shouldRememberMe">if set to <c>true</c> [should remember me].</param>
        ///// <returns></returns>
        //public static IApplicationUser Apply(string username, string password, bool shouldRememberMe)
        //{
        //    if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        //        return new ApplicationUser();

        //    var tmp = Parser.SelectOne(username, password);

        //    if (!tmp.IsNewInstance && !tmp.isActive)
        //        tmp = new ApplicationUser();

        //    if (tmp.IsNewInstance)
        //        return tmp;

        //    if (tmp.Type == 0)
        //    {
        //        using (Utilities.Authentication auth = new Utilities.Authentication())
        //        {
        //            auth.EncryptionPassword = Sushi.Mediakiwi.Data.Common.EncryptionKey;

        //            if (auth.Encrypt(password) == tmp.Password)
        //            {
        //                tmp.LastLoggedVisit = Sushi.Mediakiwi.Data.Common.DatabaseDateTime;
        //                tmp.Save(shouldRememberMe);
        //            }
        //            else
        //            {
        //                tmp = new ApplicationUser();
        //                //  Insert a notification
        //                Sushi.Mediakiwi.Data.Notification.InsertOne(Sushi.Mediakiwi.Data.Notification.Tags.InternalWimError, string.Format("Someone tried to login in with wrong credentials (username: {0})", username));
        //            }
        //        }
        //    }
        //    else if (tmp.Type == 1)
        //    {
        //        string candidate = Wim.Utility.HashStringByMD5(string.Concat(tmp.Name, password));
        //        if (candidate.Equals(tmp.Password))
        //        {
        //            tmp.LastLoggedVisit = Sushi.Mediakiwi.Data.Common.DatabaseDateTime;

        //            var visitor = Visitor.Select();
        //            visitor.LastLoggedApplicationUserVisit = Sushi.Mediakiwi.Data.Common.DatabaseDateTime;
        //            visitor.Save();

        //            tmp.Save(shouldRememberMe);
        //        }
        //        else
        //        {
        //            tmp = new ApplicationUser();
        //            //  Insert a notification
        //            Sushi.Mediakiwi.Data.Notification.InsertOne(Sushi.Mediakiwi.Data.Notification.Tags.InternalWimError, string.Format("Someone tried to login in with wrong credentials (username: {0})", username));
        //        }
        //    }
        //    return tmp;
        //}

        ///// <summary>
        ///// Applies the specified user GUID.
        ///// </summary>
        ///// <param name="userGuid">The user GUID.</param>
        ///// <param name="shouldRememberMe">if set to <c>true</c> [should remember me].</param>
        ///// <returns></returns>
        //public static IApplicationUser Apply(Guid userGuid, bool shouldRememberMe)
        //{
        //    var tmp = Parser.SelectOne(userGuid);

        //    if (tmp.IsNewInstance || !tmp.isActive)
        //        return tmp;

        //    tmp.LastLoggedVisit = Common.DatabaseDateTime;
        //    tmp.Save(shouldRememberMe);

        //    return tmp;
        //}

        ///// <summary>
        ///// Applies the password.
        ///// </summary>
        ///// <param name="password">The password.</param>
        //public void ApplyPassword(string password)
        //{
        //    this.Password = Wim.Utility.HashStringByMD5(string.Concat(this.Name, password));
        //    this.Type = 1;
        //}

        ///// <summary>
        ///// Gets the alternative name suggestion.
        ///// </summary>
        ///// <param name="name">The name.</param>
        ///// <param name="ignoreApplicationUserID">The ignore application user ID.</param>
        ///// <returns></returns>
        //public static string GetAlternativeNameSuggestion(string name, int? ignoreApplicationUserID)
        //{
        //    string candidate = name;
        //    int count = 0;
        //    while (HasUserName(candidate, ignoreApplicationUserID.GetValueOrDefault()))
        //    {
        //        count++;
        //        candidate = string.Concat(name, count);
        //    }
        //    return candidate;
        //}

        ///// <summary>
        ///// Remove the profile cookie
        ///// </summary>
        ///// <returns></returns>
        //public static bool Clear()
        //{
        //    ApplicationUser tmp = new ApplicationUser();
        //    tmp.RemoveCookie();
        //    return true;
        //}

        ///// <summary>
        ///// Removes the cookie.
        ///// </summary>
        //internal void RemoveCookie()
        //{
        //    using (Wim.Utilities.Authentication auth = new Wim.Utilities.Authentication())
        //        auth.RemoveCustomTicket(Ticket);
        //}

        //public static void ResetPassword(IApplicationUser user, out string resetLink, bool shouldEncoded = true)
        //{
        //    user.ResetKey = Guid.NewGuid();
        //    user.Save();

        //    string wimPath = Sushi.Mediakiwi.Data.Environment.Current.RelativePath;
        //    resetLink = Wim.Utility.AddApplicationPath(string.Concat(wimPath, "?reset=", user.ResetKey, $"&u={user.Email}"), true);
        //}

        #endregion MOVED to EXTENSION / LOGIC
	}
}
