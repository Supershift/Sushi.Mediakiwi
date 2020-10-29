using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Sushi.Mediakiwi.Data.DalReflection;

namespace Sushi.Mediakiwi.Data.Statistics
{
    /// <summary>
    /// 
    /// </summary>
    [DatabaseTable("wim_VisitorLogs")]
    public class VisitorLog : DatabaseEntity
    {
        #region REPLICATED TO Sushi.Mediakiwi.Data.Standard

        /// <summary>
        /// Selects the one.
        /// </summary>
        /// <param name="ID">The ID.</param>
        /// <returns></returns>
        public static VisitorLog SelectOne(int ID)
        {
            return (VisitorLog)new VisitorLog()._SelectOne(ID);
        }

        /// <summary>
        /// Cleans the domain.
        /// </summary>
        /// <returns></returns>
        public static bool CleanDomain()
        {
            List<VisitorLog> list = new List<VisitorLog>();

            List<DatabaseDataValueColumn> whereClause = new List<DatabaseDataValueColumn>();
            whereClause.Add(new DatabaseDataValueColumn("not VisitorLog_Referrer", SqlDbType.VarChar, null));

            foreach (object obj in new VisitorLog()._SelectAll(whereClause))
            {
                VisitorLog log = (VisitorLog)obj;
                string[] split = log.Referrer.ToLower()
                    .Replace("http://", string.Empty)
                    .Replace("https://", string.Empty).Split('/');
                log.ReferrerDomain = split[0];

                if (log.ReferrerDomain == "www.oncologietv.nl"
                    || log.ReferrerDomain == "oncologietv.shiftdemo.net"
                    || log.ReferrerDomain == "192.168.1.112")
                    log.ReferrerDomain = null;

                log.Save();
            }
            return true;
        }

        //internal void CleanUpProfileReference()
        //{
        //    Execute("update wim_VisitorLogs set VisitorLog_Profile_Key = (select ISNULL(max(VisitorClick_Profile_Key),0) from wim_VisitorClicks where VisitorClick_VisitorLog_Key = VisitorLog_Key)	where VisitorLog_Profile_Key is null");
        //}

        //internal void CleanUpPageViewCount()
        //{
        //    Execute("update wim_VisitorLogs set VisitorLog_Pageview = (select count(*) from wim_VisitorClicks where VisitorClick_VisitorLog_Key = VisitorLog_Key)");
        //}

        void SetDomain()
        {
            string[] split = this.Referrer.ToLower()
                    .Replace("http://", string.Empty)
                    .Replace("https://", string.Empty).Split('/');
            this.ReferrerDomain = split[0];

            if (this.ReferrerDomain == "www.oncologietv.nl"
                || this.ReferrerDomain == "oncologietv.shiftdemo.net"
                || this.ReferrerDomain == "192.168.1.112")
                this.ReferrerDomain = null;
        }


        /// <summary>
        /// Selects all.
        /// </summary>
        /// <returns></returns>
        public static VisitorLog[] SelectAll()
        {
            List<VisitorLog> list = new List<VisitorLog>();
            foreach (object obj in new VisitorLog()._SelectAll()) list.Add((VisitorLog)obj);
            return list.ToArray();
        }

        int m_ID;
        /// <summary>
        /// Gets or sets the ID.
        /// </summary>
        /// <value>The ID.</value>
        [DatabaseColumn("VisitorLog_Key", SqlDbType.Int, IsPrimaryKey = true)]
        public int ID
        {
            get { return m_ID; }
            set { m_ID = value; }
        }

        int m_VisitorID;
        /// <summary>
        /// Gets or sets the visitor ID.
        /// </summary>
        /// <value>The visitor ID.</value>
        [DatabaseColumn("VisitorLog_Visitor_Key", SqlDbType.Int)]
        public int VisitorID
        {
            get { return m_VisitorID; }
            set { m_VisitorID = value; }
        }

        /// <summary>
        /// Gets or sets the page view count.
        /// </summary>
        /// <value>The page view count.</value>
        [DatabaseColumn("VisitorLog_Pageview", SqlDbType.Int)]
        public int PageViewCount { get; set; }

        string m_Referrer;
        /// <summary>
        /// Gets or sets the referrer.
        /// </summary>
        /// <value>The referrer.</value>
        [DatabaseColumn("VisitorLog_Referrer", SqlDbType.VarChar, Length = 512, IsNullable = true)]
        public string Referrer
        {
            get { return m_Referrer; }
            set { m_Referrer = value; 
                if (value != null)
                    SetDomain();
            }
        }

        /// <summary>
        /// Gets or sets the referrer domain.
        /// </summary>
        /// <value>The referrer domain.</value>
        [DatabaseColumn("VisitorLog_ReferrerDomain", SqlDbType.VarChar, Length = 50, IsNullable = true)]
        public string ReferrerDomain { get; set; }

        string m_Agent;
        /// <summary>
        /// Gets or sets the agent.
        /// </summary>
        /// <value>The agent.</value>
        [DatabaseColumn("VisitorLog_Agent", SqlDbType.VarChar, Length = 512, IsNullable = false)]
        public string Agent
        {
            get { return m_Agent; }
            set { m_Agent = value; }
        }

        string m_Browser;
        /// <summary>
        /// Gets or sets the browser.
        /// </summary>
        /// <value>The browser.</value>
        [DatabaseColumn("VisitorLog_Browser", SqlDbType.VarChar, Length = 20, IsNullable = true)]
        public string Browser
        {
            get { return m_Browser; }
            set { m_Browser = value; }
        }

        string m_IP;
        /// <summary>
        /// Gets or sets the IP.
        /// </summary>
        /// <value>The IP.</value>
        [DatabaseColumn("VisitorLog_IP", SqlDbType.VarChar, Length = 20, IsNullable = true)]
        public string IP
        {
            get { return m_IP; }
            set { m_IP = value; }
        }

        DateTime m_Created;
        /// <summary>
        /// Gets or sets the created.
        /// </summary>
        /// <value>The created.</value>
        [DatabaseColumn("VisitorLog_Created", SqlDbType.DateTime)]
        public DateTime Created
        {
            get
            {
                if (this.m_Created == DateTime.MinValue) this.m_Created = Sushi.Mediakiwi.Data.Common.DatabaseDateTime;
                return m_Created;
            }
            set { m_Created = value; }
        }

        /// <summary>
        /// Save a database entity. This method will indentify the need for an INSERT or an UPDATE.
        /// </summary>
        /// <returns></returns>
        public override bool Save()
        {
            this.Date = this.Created.ToString("yyyyMMdd");
            this.Date2 = this.Created.ToString("yyyyMM");
            if (this.IP == "::1") this.IP = "127.0.0.1";
            return base.Save();
        }
        /// <summary>
        /// Gets or sets the date.
        /// </summary>
        /// <value>The date.</value>
        [DatabaseColumn("VisitorLog_Date", SqlDbType.VarChar, Length = 8, IsNullable = true)]
        public string Date { get; set; }
        /// <summary>
        /// Gets or sets the date2.
        /// </summary>
        /// <value>The date2.</value>
        [DatabaseColumn("VisitorLog_Date2", SqlDbType.VarChar, Length = 6, IsNullable = true)]
        public string Date2 { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is unique.
        /// </summary>
        /// <value><c>true</c> if this instance is unique; otherwise, <c>false</c>.</value>
        [DatabaseColumn("VisitorLog_IsUnique", SqlDbType.Bit, IsNullable = false)]
        public bool IsUnique { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance has cookie.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has cookie; otherwise, <c>false</c>.
        /// </value>
        [DatabaseColumn("VisitorLog_HasCookie", SqlDbType.Bit, IsNullable = false)]
        public bool HasCookie { get; set; }

        #endregion REPLICATED TO Sushi.Mediakiwi.Data.Standard
    }
}
