using Sushi.MicroORM.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sushi.Mediakiwi.Data.MicroORM;
using Sushi.Mediakiwi.Data.Interfaces;

namespace Sushi.Mediakiwi.Data
{
    [DataMap(typeof(SubscriptionMap))]
    public class Subscription : ISubscription
    {
        public class SubscriptionMap : DataMap<Subscription>
        {
            public SubscriptionMap() : this(false) { }

            public SubscriptionMap(bool isSave)
            {
                if(isSave)
                    Table("wim_Subscriptions");
                else
                    Table("wim_Subscriptions LEFT JOIN wim_Users ON User_Key = Subscription_User_Key");
                
                Id(x => x.ID, "Subscription_Key").Identity();
                Map(x => x.Title2, "Subscription_AlternativeTitle").Length(50);
                Map(x => x.UserID, "Subscription_User_Key");
                Map(x => x.ComponentListID, "Subscription_ComponentList_Key");
                Map(x => x.SiteID, "Subscription_Site_Key");
                Map(x => x.SetupXml, "Subscription_ComponentList_Setup").SqlType(System.Data.SqlDbType.Xml);
                Map(x => x.IntervalType, "Subscription_ScheduleInterval");
                Map(x => x.Scheduled, "Subscription_Scheduled");
                Map(x => x.Created, "Subscription_Created");
                Map(x => x.IsActive, "Subscription_IsActive");
                Map(x => x.User, "User_Displayname").ReadOnly();
            }
        }

        #region Properties

        public bool IsNewInstance { get { return this.ID == 0; } }

        public Subscription()
        {
            this.IsActive = true;
        }

        /// <summary>
        /// The overridden title for this subscription
        /// </summary>
        public string Title2 { get; set; }

        /// <summary>
        /// The title for this subscription (based on componentlist title, or overridden title if available)
        /// </summary>
        public string Title
        {
            get
            {
                if (string.IsNullOrEmpty(this.Title2))
                    return ComponentList.SelectOne(this.ComponentListID).Name;
                return this.Title2;
            }
        }

        /// <summary>
        /// The identifier for this subscription
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// The User id for who this subscription was made
        /// </summary>
        public int UserID { get; set; }

        /// <summary>
        /// The Username for who this subscription was made
        /// </summary>
        public string User { get; set; }

        /// <summary>
        /// The ComponentList ID for which this subscription was made
        /// </summary>
        public int ComponentListID { get; set; }

        /// <summary>
        /// The SiteID for which this subscription was made
        /// </summary>
        public int SiteID { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string SetupXml { get; set; }

        /// <summary>
        /// The interval type (hourly, daily, weekly, etc...)
        /// </summary>
        public int IntervalType { get; set; }

        /// <summary>
        /// When is the next instance scheduled ?
        /// </summary>
        public DateTime Scheduled { get; set; }

        private DateTime m_Created;

        /// <summary>
        /// The creation date/time (UTC) of this Subscription
        /// </summary>
        public DateTime Created
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
        /// Gets or sets a value indicating whether this subscription is active.
        /// </summary>
        public bool IsActive { get; set; }

        #endregion Properties

        /// <summary>
        /// Removes ALL Subscriptions from the database.
        /// </summary>
        public static void Clear()
        {
            var connector = ConnectorFactory.CreateConnector<Subscription>();
            connector.ExecuteNonQuery("DELETE FROM [wim_Subscriptions]");
			connector.Cache?.FlushRegion(connector.CacheRegion);
        }

        /// <summary>
        /// Removes ALL Subscriptions from the database Async.
        /// </summary>
        public static async Task ClearAsync()
        {
            var connector = ConnectorFactory.CreateConnector<Subscription>();
            await connector.ExecuteNonQueryAsync("DELETE FROM [wim_Subscriptions]");
			connector.Cache?.FlushRegion(connector.CacheRegion);
        }

        /// <summary>
        /// Deletes this instance from the database.
        /// </summary>
        public void Delete()
        {
            var connector = ConnectorFactory.CreateConnector<Subscription>(new SubscriptionMap(true));
            connector.Delete(this);
        }

        /// <summary>
        /// Deletes this instance from the database Async.
        /// </summary>
        public async Task DeleteAsync()
        {
            var connector = ConnectorFactory.CreateConnector<Subscription>(new SubscriptionMap(true));
            await connector.DeleteAsync(this);
        }

        /// <summary>
        /// Selects one Subscription by ID.
        /// </summary>
        /// <param name="ID">The Identifier.</param>
        /// <returns></returns>
        public static ISubscription SelectOne(int ID)
        {
            var connector = ConnectorFactory.CreateConnector<Subscription>();
            return connector.FetchSingle(ID);
        }

        /// <summary>
        /// Selects one Subscription by ID async.
        /// </summary>
        /// <param name="ID">The Identifier.</param>
        /// <returns></returns>
        public static async Task<ISubscription> SelectOneAsync(int ID)
        {
            var connector = ConnectorFactory.CreateConnector<Subscription>();
            return await connector.FetchSingleAsync(ID);
        }

        /// <summary>
        /// Selects all subscriptions which are active.
        /// </summary>
        /// <returns></returns>
        public static List<ISubscription> SelectAllActive()
        {
            var connector = ConnectorFactory.CreateConnector<Subscription>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.IsActive, true);
            var result = connector.FetchAll(filter);
            return result.ToList<ISubscription>();
        }

        /// <summary>
        /// Selects all subscriptions which are active Async.
        /// </summary>
        /// <returns></returns>
        public static async Task<List<ISubscription>> SelectAllActiveAsync()
        {
            var connector = ConnectorFactory.CreateConnector<Subscription>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.IsActive, true);
            var result = await connector.FetchAllAsync(filter);
            return result.ToList<ISubscription>();
        }

        /// <summary>
        /// Selects all subscriptions for a specific list and user.
        /// </summary>
        /// <param name="listID">The list ID.</param>
        /// <param name="userID">The user ID.</param>
        /// <returns></returns>
        public static List<ISubscription> SelectAll(int listID, int userID)
        {
            var connector = ConnectorFactory.CreateConnector<Subscription>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.ComponentListID, listID);
            filter.Add(x => x.UserID, userID);
            var result = connector.FetchAll(filter);
            return result.ToList<ISubscription>();
        }

        /// <summary>
        /// Selects all subscriptions for a specific list and user Async.
        /// </summary>
        /// <param name="listID">The list ID.</param>
        /// <param name="userID">The user ID.</param>
        /// <returns></returns>
        public static async Task<List<ISubscription>> SelectAllAsync(int listID, int userID)
        {
            var connector = ConnectorFactory.CreateConnector<Subscription>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.ComponentListID, listID);
            filter.Add(x => x.UserID, userID);
            var result = await connector.FetchAllAsync(filter);
            return result.ToList<ISubscription>();
        }

        /// <summary>
        /// Selects all subscriptions.
        /// </summary>
        public static List<ISubscription> SelectAll()
        {
            var connector = ConnectorFactory.CreateConnector<Subscription>();
            var filter = connector.CreateDataFilter();
            return connector.FetchAll(filter).ToList<ISubscription>();
        }

        /// <summary>
        /// Selects all subscriptions Async.
        /// </summary>
        public static async Task<List<ISubscription>> SelectAllAsync()
        {
            var connector = ConnectorFactory.CreateConnector<Subscription>();
            var filter = connector.CreateDataFilter();
            var result = await connector.FetchAllAsync(filter);
            return result.ToList<ISubscription>();
        }

        /// <summary>
        /// Saves this subscription instance
        /// </summary>
        public virtual void Save()
        {
            var connector = ConnectorFactory.CreateConnector<Subscription>(new SubscriptionMap(true));
            connector.Save(this);
        }

        /// <summary>
        /// Saves this subscription instance Async
        /// </summary>
        public virtual async Task SaveAsync()
        {
            var connector = ConnectorFactory.CreateConnector<Subscription>(new SubscriptionMap(true));
            await connector.SaveAsync(this);
        }


    }
}