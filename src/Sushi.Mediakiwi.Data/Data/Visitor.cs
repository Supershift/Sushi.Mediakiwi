using Sushi.Mediakiwi.Data.MicroORM;
using Sushi.MicroORM.Mapping;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Data
{
    [DataMap(typeof(VisitorMap))]
    public class Visitor : IVisitor
    {
        public class VisitorMap : DataMap<Visitor>
        {
            public VisitorMap()
            {
                Table("wim_Visitors");
                Id(x => x.ID, "Visitor_Key").Identity();
                Map(x => x.ProfileID, "Visitor_Profile_Key");
                Map(x => x.Updated, "Visitor_Updated");
                Map(x => x.GUID, "Visitor_Guid");
                Map(x => x.Created, "Visitor_Created");
                Map(x => x.RememberMe, "Visitor_RememberMe");
                Map(x => x.DataString, "Visitor_Data").SqlType(System.Data.SqlDbType.Xml);
            }
        }

        #region Properties

        public string CookieParserLog { get; set; }

        /// <summary>
        /// Gets or sets the ID.
        /// </summary>
        /// <value>The ID.</value>
        public int ID { get; set; }

        /// <summary>
        /// Gets or sets the profile ID.
        /// </summary>
        /// <value>The profile ID.</value>
        public int? ProfileID { get; set; }

        /// <summary>
        /// Gets or sets the created.
        /// </summary>
        /// <value>The created.</value>
        public DateTime Updated { get; set; } = DateTime.UtcNow;

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
                {
                    if (value.HasValue)
                        this.Data.Apply("wim_appuser_id", value.Value);
                    else
                        this.Data.Apply("wim_appuser_id", null);
                }
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
        public Guid GUID
        {
            get
            {
                if (m_GUID == Guid.Empty)
                    m_GUID = Guid.NewGuid();
                return m_GUID;
            }
            set { m_GUID = value; }
        }

        private DateTime m_Created;

        /// <summary>
        /// Gets or sets the created.
        /// </summary>
        /// <value>The created.</value>
        public DateTime Created
        {
            get
            {
                if (m_Created == DateTime.MinValue)
                    m_Created = Common.DatabaseDateTime;
                return m_Created;
            }
            set { m_Created = value; }
        }

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
                return Convert.ToInt32(new TimeSpan(DateTime.UtcNow.Ticks - Updated.Ticks).TotalMinutes);
            }
        }

        /// <summary>
        /// Gets or sets the remember me.
        /// </summary>
        /// <value>The remember me.</value>
        public bool RememberMe { get; set; }

        /// <summary>
        /// XML representation of the DATA property
        /// </summary>
        public virtual string DataString {
            get
            {
                if (m_Data == null)
                    return null;
                return m_Data.Serialized;
            }
            set
            {
                m_Data = new CustomData(value);
            }
        }

        private CustomData m_Data;

        /// <summary>
        /// Holds all customData properties
        /// </summary>
        public CustomData Data
        {
            get
            {
                if (m_Data == null)
                    m_Data = new CustomData(DataString);
                return m_Data;
            }
            set
            {
                m_Data = value;
                DataString = m_Data.Serialized;
            }
        }

        /// <summary>
        /// Gets or sets the market.
        /// </summary>
        /// <value>The market.</value>
        public int? CountryID
        {
            get { return Data["Country"].ParseInt(); }
            set
            {
                if (value.HasValue)
                    Data.Apply("Country", value.Value);
                else
                    Data.Apply("Country", null);
            }
        }

        /// <summary>
        /// Gets or sets the language.
        /// </summary>
        /// <value>The language.</value>
        public string Language
        {
            get { return Data["Language"].Value; }
            set { Data.Apply("Language", value); }
        }

        public DateTime LastRequestDone
        {
            get { return this.Data["LastTimeRequestDone"].ParseDateTime() ?? DateTime.MinValue; }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Selects the specified visitor reference.
        /// </summary>
        /// <param name="visitorReference">The visitor reference.</param>
        /// <returns></returns>
        public static IVisitor Select(Guid visitorReference)
        {
            var connector = ConnectorFactory.CreateConnector<Visitor>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.GUID, visitorReference);

            return connector.FetchSingle(filter);
        }


        /// <summary>
        /// Selects the specified visitor reference.
        /// </summary>
        /// <param name="visitorReference">The visitor reference.</param>
        /// <returns></returns>
        public static async Task<IVisitor> SelectAsync(Guid visitorReference)
        {
            var connector = ConnectorFactory.CreateConnector<Visitor>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.GUID, visitorReference);

            return await connector.FetchSingleAsync(filter);
        }

        /// <summary>
        /// Selects the one.
        /// </summary>
        /// <param name="ID">The ID.</param>
        /// <returns></returns>
        public static IVisitor SelectOne(int ID)
        {
            var connector = ConnectorFactory.CreateConnector<Visitor>();

            return connector.FetchSingle(ID);
        }

        public static void Save(Visitor entity)
        {
            var connector = ConnectorFactory.CreateConnector<Visitor>();
            connector.Save(entity);
        }


        public static async Task SaveAsync(Visitor entity)
        {
            var connector = ConnectorFactory.CreateConnector<Visitor>();
            await connector.SaveAsync(entity).ConfigureAwait(false);
        }

        public async Task<bool> SaveAsync()
        {
            await SaveAsync(this).ConfigureAwait(false);
            return true;
        }

        public bool Save()
        {
            Save(this);
            return true;
        }

        /// <summary>
        /// Selects the one.
        /// </summary>
        /// <param name="ID">The ID.</param>
        /// <returns></returns>
        public static async Task<IVisitor> SelectOneAsync(int ID)
        {
            var connector = ConnectorFactory.CreateConnector<Visitor>();

            return await connector.FetchSingleAsync(ID);
        }

        /// <summary>
        /// Selects all visitors with the profile id
        /// </summary>
        /// <param name="profileId">The profile id.</param>
        /// <param name="excludeVisitorID">The visitor ID to exclude.</param>
        /// <returns></returns>
        public static IVisitor[] SelectAllByProfile(int profileId, int excludeVisitorID)
        {
            var connector = ConnectorFactory.CreateConnector<Visitor>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.ProfileID, profileId);

            int minutes = Utility.ConvertToInt(CommonConfiguration.EXPIRATION_COOKIE_PROFILE, 0);

            var result = connector.FetchAll(filter);

            List<IVisitor> list = new List<IVisitor>();
            foreach (var visitor in result)
            {
                if (visitor.ID != excludeVisitorID && visitor.LastRequestDone.Subtract(DateTime.Now).TotalMinutes < minutes)
                    list.Add(visitor);
            }
            return list.ToArray();
        }

        /// <summary>
        /// Selects all visitors with the profile id
        /// </summary>
        /// <param name="profileId">The profile id.</param>
        /// <param name="excludeVisitorID">The visitor ID to exclude.</param>
        /// <returns></returns>
        public static async Task<IVisitor[]> SelectAllByProfileAsync(int profileId, int excludeVisitorID)
        {
            var connector = ConnectorFactory.CreateConnector<Visitor>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.ProfileID, profileId);

            int minutes = Utility.ConvertToInt(CommonConfiguration.EXPIRATION_COOKIE_VISITOR, 0);

            var result = await connector.FetchAllAsync(filter);

            List<IVisitor> list = new List<IVisitor>();
            foreach (var visitor in result)
            {
                if (visitor.ID != excludeVisitorID && visitor.LastRequestDone.Subtract(DateTime.Now).TotalMinutes < minutes)
                    list.Add(visitor);
            }
            return list.ToArray();
        }

        public bool IsNewInstance
        {
            get
            {
                return ID == 0;
            }
        }

        #endregion Methods
    }
}