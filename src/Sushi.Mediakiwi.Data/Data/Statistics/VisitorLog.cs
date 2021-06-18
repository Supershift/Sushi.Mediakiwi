using Sushi.MicroORM;
using Sushi.MicroORM.Mapping;
using System;
using System.Threading.Tasks;
using Sushi.Mediakiwi.Data.MicroORM;

namespace Sushi.Mediakiwi.Data.Statistics
{
    /// <summary>
    /// 
    /// </summary>
    [DataMap(typeof(VisitorLogMap))]
    public class VisitorLog
    {
        public class VisitorLogMap : DataMap<VisitorLog>
        {
            public VisitorLogMap()
            {
                Table("wim_VisitorLogs");

                Id(x => x.ID, "VisitorLog_Key").Identity();
                Map(x => x.VisitorID, "VisitorLog_Visitor_Key");
                Map(x => x.PageViewCount, "VisitorLog_Pageview");
                Map(x => x.Referrer, "VisitorLog_Referrer").Length(512);
                Map(x => x.ReferrerDomain, "VisitorLog_ReferrerDomain").Length(50);
                Map(x => x.Agent, "VisitorLog_Agent").Length(512);
                Map(x => x.Browser, "VisitorLog_Browser").Length(20);
                Map(x => x.IP, "VisitorLog_IP").Length(20);
                Map(x => x.Created, "VisitorLog_Created");
                Map(x => x.IsUnique, "VisitorLog_IsUnique");
                Map(x => x.HasCookie, "VisitorLog_HasCookie");

                // [MR:17-01-2020] DEZE ZIJN NIET GOED GECONFIGUREERD IN DE DB, 
                // HIER IN DEZE CLASS WORDT UITGEGAAN VAN NVARCHAR MAAR IN DE DB ZIJN HET INTS
                Map(x => x.Date, "VisitorLog_Date").Length(8);
                Map(x => x.Date2, "VisitorLog_Date2").Length(6);
            }
        }

        #region Properties

        public int ID { get; set; }

        /// <summary>
        /// Gets or sets the visitor ID.
        /// </summary>
        /// <value>The visitor ID.</value>
        public int VisitorID { get; set; }

        /// <summary>
        /// Gets or sets the page view count.
        /// </summary>
        /// <value>The page view count.</value>
        public int PageViewCount { get; set; }

        string m_Referrer;
        /// <summary>
        /// Gets or sets the referrer.
        /// </summary>
        /// <value>The referrer.</value>

        public string Referrer
        {
            get { return m_Referrer; }
            set
            {
                m_Referrer = value;
                if (value != null)
                    SetDomain();
            }
        }

        /// <summary>
        /// Gets or sets the referrer domain.
        /// </summary>
        /// <value>The referrer domain.</value>
        public string ReferrerDomain { get; set; }

        /// <summary>
        /// Gets or sets the agent.
        /// </summary>
        /// <value>The agent.</value>
        public string Agent { get; set; }

        /// <summary>
        /// Gets or sets the browser.
        /// </summary>
        /// <value>The browser.</value>
        public string Browser { get; set; }

        /// <summary>
        /// Gets or sets the IP.
        /// </summary>
        /// <value>The IP.</value>
        public string IP { get; set; }

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
        /// Gets or sets the date.
        /// </summary>
        /// <value>The date.</value>
        public string Date { get; set; }

        /// <summary>
        /// Gets or sets the date2.
        /// </summary>
        /// <value>The date2.</value>
        public string Date2 { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is unique.
        /// </summary>
        /// <value><c>true</c> if this instance is unique; otherwise, <c>false</c>.</value>
        public bool IsUnique { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance has cookie.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has cookie; otherwise, <c>false</c>.
        /// </value>
        public bool HasCookie { get; set; }

        #endregion Properties

        /// <summary>
        /// Selects the one.
        /// </summary>
        /// <param name="ID">The ID.</param>
        /// <returns></returns>
        public static VisitorLog SelectOne(int ID)
        {
            var connector = ConnectorFactory.CreateConnector<VisitorLog>();
            return connector.FetchSingle(ID);
        }

        /// <summary>
        /// Selects the one.
        /// </summary>
        /// <param name="ID">The ID.</param>
        /// <returns></returns>
        public static async Task<VisitorLog> SelectOneAsync(int ID)
        {
            var connector = ConnectorFactory.CreateConnector<VisitorLog>();
            return await connector.FetchSingleAsync(ID);
        }

        /// <summary>
        /// Cleans the domain.
        /// </summary>
        /// <returns></returns>
        public static bool CleanDomain()
        {
            var connector = ConnectorFactory.CreateConnector<VisitorLog>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.Referrer, null, ComparisonOperator.NotEqualTo);

            foreach (VisitorLog logItem in connector.FetchAll(filter))
            {
                logItem.SetDomain();
                logItem.Save();
            }
            return true;
        }

        /// <summary>
        /// Cleans the domain.
        /// </summary>
        /// <returns></returns>
        public static async Task<bool> CleanDomainAsync()
        {
            var connector = ConnectorFactory.CreateConnector<VisitorLog>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.Referrer, null, ComparisonOperator.NotEqualTo);

            foreach (VisitorLog logItem in await connector.FetchAllAsync(filter))
            {
                logItem.SetDomain();
                await logItem.SaveAsync();
            }
            return true;
        }


        void SetDomain()
        {
            string[] split = this.Referrer.ToLower()
                    .Replace("http://", string.Empty)
                    .Replace("https://", string.Empty).Split('/');
            this.ReferrerDomain = split[0];

            //[MR:17-01-2020] yeahhh.... lets not let this linger
            //if (this.ReferrerDomain == "www.oncologietv.nl"
            //    || this.ReferrerDomain == "oncologietv.shiftdemo.net"
            //    || this.ReferrerDomain == "192.168.1.112")
            //    this.ReferrerDomain = null;
        }


        /// <summary>
        /// Selects all.
        /// </summary>
        /// <returns></returns>
        public static VisitorLog[] SelectAll()
        {
            var connector = ConnectorFactory.CreateConnector<VisitorLog>();
            var filter = connector.CreateDataFilter();

            return connector.FetchAll(filter).ToArray();
        }



        /// <summary>
        /// Selects all.
        /// </summary>
        /// <returns></returns>
        public static async Task<VisitorLog[]> SelectAllAsync()
        {
            var connector = ConnectorFactory.CreateConnector<VisitorLog>();
            var filter = connector.CreateDataFilter();

            var result = await connector.FetchAllAsync(filter);
            return result.ToArray();
        }

        /// <summary>
        /// Save a database entity. This method will indentify the need for an INSERT or an UPDATE.
        /// </summary>
        /// <returns></returns>
        public bool Save()
        {
            var connector = ConnectorFactory.CreateConnector<VisitorLog>();
            Date = Created.ToString("yyyyMMdd");
            Date2 = Created.ToString("yyyyMM");

            if (IP == "::1")
                IP = "127.0.0.1";

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

        /// <summary>
        /// Save a database entity. This method will indentify the need for an INSERT or an UPDATE.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> SaveAsync()
        {
            var connector = ConnectorFactory.CreateConnector<VisitorLog>();
            Date = Created.ToString("yyyyMMdd");
            Date2 = Created.ToString("yyyyMM");

            if (IP == "::1")
                IP = "127.0.0.1";

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
    }
}
