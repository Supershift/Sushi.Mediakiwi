using System;
using System.Data;
using Sushi.Mediakiwi.Data.DalReflection;
using Sushi.Mediakiwi.Data.Identity.Parsers;
using Sushi.Mediakiwi.Framework;

namespace Sushi.Mediakiwi.Data.Identity
{
    [DatabaseTable("wim_Visitors")]
    [ListReference("46B59076-FEF3-4BAA-9BE0-2C9D71DF3379")]
    public class Visitor : IVisitor, ISaveble
    {
        static IVisitorParser _Parser;
        static IVisitorParser Parser
        {
            get
            {
                if (_Parser == null)
                    _Parser = Sushi.Mediakiwi.Data.Environment.GetInstance<IVisitorParser>();
                return _Parser;
            }
        }

        const string CACHEKEYPREFIX = "Data.Visitor.";
        static string[] _AgentSplit;
        public Wim.Utilities.Authentication.TicketConversionInfo Info;
        public string CookieParserLog { get; set; }

        bool m_IsLoggedIn;
        /// <summary>
        /// Gets or sets a value indicating whether this instance is logged in.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is logged in; otherwise, <c>false</c>.
        /// </value>
        [DatabaseColumn("Visitor_IsLoggedIn", SqlDbType.Bit, IsNullable = true)]
        public virtual bool IsLoggedIn
        {
            get
            {
                int minutes = Wim.Utility.ConvertToInt(Sushi.Mediakiwi.Data.Environment.Current["EXPIRATION_COOKIE_PROFILE"], CommonConfiguration.MAX_SESSION_LENGTH);
                if (LastUpdateMinutes >= minutes || !this.ProfileID.HasValue)
                {
                    this.m_IsLoggedIn = false;
                }
                return m_IsLoggedIn;
            }
            set
            {
                m_IsLoggedIn = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is new session.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is new session; otherwise, <c>false</c>.
        /// </value>
        public bool IsNewSession
        {
            get
            {
                if (
                    string.IsNullOrEmpty(System.Web.HttpContext.Current.Request.Headers["Cookie"])
                    || LastUpdateMinutes > Wim.CommonConfiguration.MAX_SESSION_LENGTH
                    )
                    return true;
                return false;

            }
        }

        /// <summary>
        /// Select the current visitor object. If no visitor exists an instance is created including a database reference.
        /// By default, the visitor reference is not stored accross sessions. 
        /// </summary>
        /// <returns></returns>
        public static IVisitor Select()
        {
            return Parser.Select();
        }


        public bool Logout(int? redirectionPageID)
        {
            return Parser.Logout(this, redirectionPageID);
        }

        public virtual bool Save()
        {
            return Save(true);
        }

        /// <summary>
        /// Saves the specified should remember profile for next visit.
        /// </summary>
        /// <param name="shouldRememberVisitorForNextVisit">if set to <c>true</c> [should remember visitor for next visit].</param>
        /// <returns></returns>
        public virtual bool Save(bool shouldRememberVisitorForNextVisit)
        {
            return Parser.Save(this, shouldRememberVisitorForNextVisit);
        }

        public virtual bool SaveData()
        {
            return Parser.SaveData(this);
        }

        /// <summary>
        /// Sets the cookie.
        /// </summary>
        public void SetCookie()
        {
            Parser.SetCookie(this);
        }

        /// <summary>
        /// Sets the cookie.
        /// </summary>
        /// <param name="guid">The GUID.</param>
        /// <param name="profileId">The profile id.</param>
        /// <param name="shouldRememberProfileForNextVisit">if set to <c>true</c> [should remember profile for next visit].</param>
        public void SetCookie(Guid guid, int? profileId, bool shouldRememberProfileForNextVisit)
        {
            Parser.SetCookie(ID, guid, profileId, shouldRememberProfileForNextVisit);
        }


        /// <summary>
        /// Remove the profile cookie
        /// </summary>
        /// <returns></returns>
        public static bool Clear()
        {
            return Parser.Clear();
        }

        /// <summary>
        /// Sets the info from cookie.
        /// </summary>
        public IVisitor SetInfoFromCookie()
        {
            return Parser.SetInfoFromCookie();
        }


        /// <summary>
        /// Applies the campaign.
        /// </summary>
        /// <param name="campaignID">The campaign ID.</param>
        /// <param name="autoSave">if set to <c>true</c> [auto save].</param>
        public void ApplyCampaign(int campaignID, bool autoSave)
        {
            Parser.ApplyCampaign(this, campaignID, autoSave);
        }

        /// <summary>
        /// Clears the campaign.
        /// </summary>
        /// <param name="autoSave">if set to <c>true</c> [auto save].</param>
        public void ClearCampaign(bool autoSave)
        {
            Parser.ClearCampaign(this, autoSave);
        }



        #region REPLICATED TO Sushi.Mediakiwi.Data.Standard

        /// <summary>
        /// Initializes a new instance of the <see cref="Visitor"/> class.
        /// </summary>
        public Visitor()
        {
        }

        #region Properties
        /// <summary>
        /// Gets the last visited page (if none present an empty Page object is returned).
        /// </summary>
        /// <value>The last visited page.</value>
        public Page LastVisitedPage
        {
            get
            {
                int lastPageID = this.Data["wim_lastpageid"].ParseInt().GetValueOrDefault();
                Sushi.Mediakiwi.Data.Page page = Sushi.Mediakiwi.Data.Page.SelectOne(lastPageID);
                return page;
            }
        }

        #region properties
        /// <summary>
        /// Gets or sets the ID.
        /// </summary>
        /// <value>The ID.</value>
        [DatabaseColumn("Visitor_Key", SqlDbType.Int, IsPrimaryKey = true)]
        public int ID { get; set; }

        int? m_ProfileID;
        /// <summary>
        /// Gets or sets the profile ID.
        /// </summary>
        /// <value>The profile ID.</value>
        [DatabaseColumn("Visitor_Profile_Key", SqlDbType.Int, IsNullable = true)]
        public int? ProfileID
        {
            get { return m_ProfileID; }
            set
            {
                if (m_ProfileID != value)
                    m_ProfileID = value;
            }
        }

        /// <summary>
        /// Gets or sets the created.
        /// </summary>
        /// <value>The created.</value>
        [DatabaseColumn("Visitor_Updated", SqlDbType.DateTime, IsNullable = true)]
        public DateTime Updated { get; set; }
        /// <summary>
        /// Timestamp (UTC) obtained exclusively from the cookie
        /// </summary>
        public DateTime LastVisit { get; set; }

        /// <summary>
        /// Gets or sets the application user ID.
        /// </summary>
        /// <value>The application user ID.</value>
        public int? ApplicationUserID
        {
            get { return this.Data["wim_appuser_id"].ParseInt(); }
            set
            {
                if (ApplicationUserID != value)

                    if (value.HasValue)
                        this.Data.Apply("wim_appuser_id", value.Value);
                    else
                        this.Data.Apply("wim_appuser_id", null);
            }
        }

        /// <summary>
        /// Gets or sets the application user update.
        /// </summary>
        /// <value>
        /// The application user update.
        /// </value>
        public DateTime? LastLoggedApplicationUserVisit
        {
            get { return this.Data["wim_appuser_update"].ParseDateTime(); }
            set
            {
                if (value.HasValue)
                    this.Data.Apply("wim_appuser_update", value.Value);
                else
                    this.Data.Apply("wim_appuser_update", null);
            }
        }

        private Guid m_GUID;
        /// <summary>
        /// Gets or sets the GUID.
        /// </summary>
        /// <value>The GUID.</value>
        [DatabaseColumn("Visitor_Guid", SqlDbType.UniqueIdentifier, IsMigrationKey = true)]
        public Guid GUID
        {
            get
            {
                if (this.m_GUID == Guid.Empty) this.m_GUID = Guid.NewGuid();
                return m_GUID;
            }
            set { m_GUID = value; }
        }

        DateTime m_Created;
        /// <summary>
        /// Gets or sets the created.
        /// </summary>
        /// <value>The created.</value>
        [DatabaseColumn("Visitor_Created", SqlDbType.DateTime)]
        public DateTime Created
        {
            get
            {
                if (this.m_Created == DateTime.MinValue) this.m_Created = Sushi.Mediakiwi.Data.Common.DatabaseDateTime;
                return m_Created;
            }
            set { m_Created = value; }
        }

        //DateTime m_LastUpdated;
        ///// <summary>
        ///// Gets or sets the created.
        ///// </summary>
        ///// <value>The created.</value>
        //public DateTime LastUpdated
        //{
        //    get { return m_LastUpdated; }
        //}


        /// <summary>
        /// Gets or sets a value indicating whether this instance is new visitor.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is new visitor; otherwise, <c>false</c>.
        /// </value>
        public bool IsNewVisitor { get; set; }

        /// <summary>
        /// Gets the get last update in minutes.
        /// </summary>
        /// <value>The get last update minutes.</value>
        public int LastUpdateMinutes
        {
            get
            {
                return Convert.ToInt32(new TimeSpan(DateTime.Now.Ticks - Updated.Ticks).TotalMinutes);
            }
        }


        bool m_RememberMe;
        /// <summary>
        /// Gets or sets the remember me.
        /// </summary>
        /// <value>The remember me.</value>
        [DatabaseColumn("Visitor_RememberMe", SqlDbType.Bit)]
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
        [DatabaseColumn("Visitor_Data", SqlDbType.Xml, IsNullable = true)]
        public Sushi.Mediakiwi.Data.CustomData Data
        {
            get
            {
                if (m_Data == null)
                    m_Data = new CustomData();
                return m_Data;
            }
            set
            {

                m_Data = value;
            }
        }

        /// <summary>
        /// Gets or sets the market.
        /// </summary>
        /// <value>The market.</value>
        public int? CountryID
        {
            get { return this.Data["Country"].ParseInt(); }
            set
            {


                if (value.HasValue) this.Data.Apply("Country", value.Value);
                else
                    this.Data.Apply("Country", null);
            }
        }

        /// <summary>
        /// Gets or sets the language.
        /// </summary>
        /// <value>The language.</value>
        public string Language
        {
            get { return this.Data["Language"].Value; }
            set
            {

                this.Data.Apply("Language", value);
            }
        }

        public DateTime LastRequestDone
        {
            get { return this.Data["LastTimeRequestDone"].ParseDateTime() ?? DateTime.MinValue; }

        }
        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// Selects the specified visitor reference.
        /// </summary>
        /// <param name="visitorReference">The visitor reference.</param>
        /// <returns></returns>
        public static IVisitor Select(Guid visitorReference)
        {
            return Parser.Select(visitorReference);
        }
        /// <summary>
        /// Selects the one.
        /// </summary>
        /// <param name="ID">The ID.</param>
        /// <returns></returns>
        public static IVisitor SelectOne(int ID)
        {
            return Parser.SelectOne(ID);
        }
        ///// <summary>
        ///// Selects the one.
        ///// </summary>
        ///// <param name="ID">The ID.</param>
        ///// <param name="info">The info.</param>
        ///// <returns></returns>
        //public static Visitor SelectOne(int ID, SqlInfo info)
        //{
        //    Visitor implement = new Visitor();
        //    if (info != null)
        //    {
        //        implement.SqlRowCount = info.SqlRowCount;
        //        implement.SqlOrder = info.SqlOrder;
        //        implement.SqlJoin = info.SqlJoin;
        //        implement.SqlGroup = info.SqlGroup;
        //    }

        //    return (Visitor)implement._SelectOne(ID);
        //}

        ///// <summary>
        ///// Selects the one.
        ///// </summary>
        ///// <param name="where">The where.</param>
        ///// <returns></returns>
        //public static Visitor SelectOne(List<DatabaseDataValueColumn> where)
        //{
        //    return SelectOne(where, null);
        //}

        ///// <summary>
        ///// Selects the one.
        ///// </summary>
        ///// <param name="where">The where.</param>
        ///// <param name="info">The info.</param>
        ///// <returns></returns>
        //public static Visitor SelectOne(List<DatabaseDataValueColumn> where, SqlInfo info)
        //{
        //    Visitor implement = new Visitor();
        //    if (info != null)
        //    {
        //        implement.SqlRowCount = info.SqlRowCount;
        //        implement.SqlOrder = info.SqlOrder;
        //        implement.SqlJoin = info.SqlJoin;
        //        implement.SqlGroup = info.SqlGroup;
        //    }

        //    return (Visitor)implement._SelectOne(where);
        //}

        /// <summary>
        /// Selects all visitors with the profile id
        /// </summary>
        /// <param name="profileId">The profile id.</param>
        /// <param name="visitorID">The visitor ID.</param>
        /// <returns></returns>
        public static IVisitor[] SelectAllByProfile(int profileId, int visitorID)
        {
            return Parser.SelectAllByProfile(profileId, visitorID);
        }

        ///// <summary>
        ///// Selects all.
        ///// </summary>
        ///// <param name="where">The where.</param>
        ///// <returns></returns>
        //public static Visitor[] SelectAll(List<DatabaseDataValueColumn> where)
        //{
        //    return SelectAll(where, null);
        //}

        ///// <summary>
        ///// Selects all.
        ///// </summary>
        ///// <param name="where">The where.</param>
        ///// <param name="info">The info.</param>
        ///// <returns></returns>
        //public static Visitor[] SelectAll(List<DatabaseDataValueColumn> where, SqlInfo info)
        //{
        //    Visitor implement = new Visitor();

        //    if (info != null)
        //    {
        //        implement.SqlRowCount = info.SqlRowCount;
        //        implement.SqlOrder = info.SqlOrder;
        //        implement.SqlJoin = info.SqlJoin;
        //        implement.SqlGroup = info.SqlGroup;
        //    }

        //    List<Visitor> list = new List<Visitor>();
        //    foreach (object o in implement._SelectAll(where)) list.Add((Visitor)o);
        //    return list.ToArray();
        //}

        public bool IsNewInstance
        {
            get
            {
                return ID == 0;
            }
        }

        #endregion

        #endregion REPLICATED TO Sushi.Mediakiwi.Data.Standard
    }
}