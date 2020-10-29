using Sushi.MicroORM;
using Sushi.MicroORM.Mapping;
using System;
using System.Data;
using System.Threading.Tasks;
using Sushi.Mediakiwi.Data.MicroORM;
using Sushi.Mediakiwi.Data;

namespace Sushi.Mediakiwi.Data.Statistics
{
    /// <summary>
    /// 
    /// </summary>
    [DataMap(typeof(VisitorClickMap))]
    public class VisitorClick : IVisitorClick
    {
        public class VisitorClickMap : DataMap<VisitorClick>
        {
            public VisitorClickMap()
            {
                Table("wim_VisitorClicks");

                Id(x => x.ID, "VisitorClick_Key").Identity();
                Map(x => x.ApplicationUserID, "VisitorClick_AppUser_Key");
                Map(x => x.VisitorLogID, "VisitorClick_VisitorLog_Key");
                Map(x => x.ProfileID, "VisitorClick_Profile_Key");
                Map(x => x.ItemID, "VisitorClick_Item_Key");
                Map(x => x.PageID, "VisitorClick_Page_Key");
                Map(x => x.RenderTime, "VisitorClick_RenderTime");
                Map(x => x.Query, "VisitorClick_Query").Length(50);
                Map(x => x.IsEntry, "VisitorClick_IsEntry");
                Map(x => x.DataString, "VisitorClick_Data").SqlType(SqlDbType.Xml);
                Map(x => x.Created, "VisitorClick_Created");
                Map(x => x.CampaignID, "VisitorClick_Campaign_Key");


            }
        }

        #region Properties

        public int ID { get; set; }

        public int? ApplicationUserID { get; set; }

        public int VisitorLogID { get; set; }

        public int? ProfileID { get; set; }

        public int? ItemID { get; set; }

        public int? PageID { get; set; }

        /// <summary>
        /// Gets or sets the render time (in milliseconds).
        /// </summary>
        /// <value>
        /// The render time.
        /// </value>
        public int? RenderTime { get; set; }

        /// <summary>
        /// Gets or sets the query.
        /// </summary>
        /// <value>The query.</value>
        public string Query { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is entry.
        /// </summary>
        /// <value><c>true</c> if this instance is entry; otherwise, <c>false</c>.</value>
        public bool IsEntry { get; set; }

        public string DataString { get; set; }

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

        DateTime m_Created;
        /// <summary>
        /// Gets or sets the created.
        /// </summary>
        /// <value>The created.</value>
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
        /// TODO: Needs to be moved
        /// </summary>
        public int? CampaignID { get; set; }

        #endregion Properties

        /// Selects the one.
        /// </summary>
        /// <param name="ID">The ID.</param>
        /// <returns></returns>
        public static IVisitorClick SelectOne(int ID)
        {
            var connector = ConnectorFactory.CreateConnector<VisitorClick>();
            return connector.FetchSingle(ID);
        }

        /// Selects the one.
        /// </summary>
        /// <param name="ID">The ID.</param>
        /// <returns></returns>
        public async static Task<IVisitorClick> SelectOneAsync(int ID)
        {
            var connector = ConnectorFactory.CreateConnector<VisitorClick>();
            return await connector.FetchSingleAsync(ID);
        }

        /// <summary>
        /// Selects all.
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        /// <returns></returns>
        public IVisitorClick[] SelectAll(DateTime from, DateTime to)
        {
            var connector = ConnectorFactory.CreateConnector<VisitorClick>();
            var filter = connector.CreateDataFilter();
            filter.AddParameter<DateTime>("@D1", from);
            filter.AddParameter<DateTime>("@D2", to);
            filter.AddSql("[VisitorClick_Created] BETWEEN @D1 AND @D2");

            return connector.FetchAll(filter).ToArray();
        }

        /// <summary>
        /// Selects all.
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        /// <returns></returns>
        public async Task<IVisitorClick[]> SelectAllAsync(DateTime from, DateTime to)
        {
            var connector = ConnectorFactory.CreateConnector<VisitorClick>();
            var filter = connector.CreateDataFilter();
            filter.AddParameter<DateTime>("@D1", from);
            filter.AddParameter<DateTime>("@D2", to);
            filter.AddSql("[VisitorClick_Created] BETWEEN @D1 AND @D2");

            var result = await connector.FetchAllAsync(filter);
            return result.ToArray();
        }


        /// <summary>
        /// Gets the log.
        /// </summary>
        /// <value>The log.</value>
        public VisitorLog Log()
        {
            return VisitorLog.SelectOne(VisitorLogID);
        }

        /// <summary>
        /// Gets the log Async.
        /// </summary>
        /// <value>The log.</value>
        public async Task<VisitorLog> LogAsync()
        {
            return await VisitorLog.SelectOneAsync(VisitorLogID);
        }

        public void Save()
        {
            var connector = ConnectorFactory.CreateConnector<VisitorClick>();

            if (this.ProfileID > 0)
            {
                var filter = connector.CreateDataFilter();
                filter.AddParameter("@profileId", ProfileID);
                filter.AddParameter("@visitorLogId", VisitorLogID);
                connector.ExecuteNonQuery("UPDATE [wim_VisitorLogs] SET [VisitorLog_Profile_Key] = @profileId, [VisitorLog_Pageview] = [VisitorLog_Pageview] + 1 WHERE [VisitorLog_Key] = @visitorLogId", filter);
				connector.Cache?.FlushRegion(connector.CacheRegion);
            }
            else
            {
                var filter = connector.CreateDataFilter();
                filter.AddParameter("@visitorLogId", VisitorLogID);
                connector.ExecuteNonQuery("UPDATE [wim_VisitorLogs] SET [VisitorLog_Pageview] = [VisitorLog_Pageview] + 1 WHERE [VisitorLog_Key] = @visitorLogId", filter);
				connector.Cache?.FlushRegion(connector.CacheRegion);
            }

            connector.Save(this);
        }

        public async Task SaveAsync()
        {
            var connector = ConnectorFactory.CreateConnector<VisitorClick>();

            if (this.ProfileID > 0)
            {
                var filter = connector.CreateDataFilter();
                filter.AddParameter("@profileId", ProfileID);
                filter.AddParameter("@visitorLogId", VisitorLogID);
                await connector.ExecuteNonQueryAsync("UPDATE [wim_VisitorLogs] SET [VisitorLog_Profile_Key] = @profileId, [VisitorLog_Pageview] = [VisitorLog_Pageview] + 1 WHERE [VisitorLog_Key] = @visitorLogId", filter);
				connector.Cache?.FlushRegion(connector.CacheRegion);
            }
            else
            {
                var filter = connector.CreateDataFilter();
                filter.AddParameter("@visitorLogId", VisitorLogID);
                await connector.ExecuteNonQueryAsync("UPDATE [wim_VisitorLogs] SET [VisitorLog_Pageview] = [VisitorLog_Pageview] + 1 WHERE [VisitorLog_Key] = @visitorLogId", filter);
				connector.Cache?.FlushRegion(connector.CacheRegion);
            }

            await connector.SaveAsync(this);
        }

    }
}
