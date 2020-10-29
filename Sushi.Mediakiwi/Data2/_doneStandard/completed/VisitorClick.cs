using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Sushi.Mediakiwi.Data.DalReflection;
using Sushi.Mediakiwi.Data.Statistics.Parsers;
using Sushi.Mediakiwi.Framework;

namespace Sushi.Mediakiwi.Data.Statistics
{
    /// <summary>
    /// 
    /// </summary>
    [DatabaseTable("wim_VisitorClicks")]
    public class VisitorClick : IVisitorClick
    {
        static IVisitorClickParser _Parser;
        static IVisitorClickParser Parser
        {
            get
            {
                if (_Parser == null)
                    _Parser = Environment.GetInstance<IVisitorClickParser>();
                return _Parser;
            }
        }

        #region REPLICATED TO Sushi.Mediakiwi.Data.Standard

        #region Properties
        [DatabaseColumn("VisitorClick_Key", SqlDbType.Int, IsPrimaryKey = true)]
        public virtual int ID { get; set; }

        [DatabaseColumn("VisitorClick_AppUser_Key", SqlDbType.Int, IsNullable = true)]
        public virtual int? ApplicationUserID { get; set; }

        [DatabaseColumn("VisitorClick_VisitorLog_Key", SqlDbType.Int)]
        public virtual int VisitorLogID { get; set; }

        [DatabaseColumn("VisitorClick_Profile_Key", SqlDbType.Int, IsNullable = true)]
        public virtual int? ProfileID { get; set; }

        [DatabaseColumn("VisitorClick_Item_Key", SqlDbType.Int, IsNullable = true)]
        public virtual int? ItemID { get; set; }

        [DatabaseColumn("VisitorClick_Page_Key", SqlDbType.Int, IsNullable = true)]
        public virtual int? PageID { get; set; }

        /// <summary>
        /// Gets or sets the render time (in milliseconds).
        /// </summary>
        /// <value>
        /// The render time.
        /// </value>
        [DatabaseColumn("VisitorClick_RenderTime", SqlDbType.Int, IsNullable = true)]
        public virtual int? RenderTime { get; set; }

        /// <summary>
        /// Gets or sets the query.
        /// </summary>
        /// <value>The query.</value>
        [DatabaseColumn("VisitorClick_Query", SqlDbType.VarChar, Length = 50, IsNullable = true)]
        public virtual string Query { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is entry.
        /// </summary>
        /// <value><c>true</c> if this instance is entry; otherwise, <c>false</c>.</value>
        [DatabaseColumn("VisitorClick_IsEntry", SqlDbType.Bit, IsNullable = true)]
        public virtual bool IsEntry { get; set; }

        public virtual int Entry { get; set; }

        Sushi.Mediakiwi.Data.CustomData m_Data;
        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        /// <value>The data.</value>
        [DatabaseColumn("VisitorClick_Data", SqlDbType.Xml, IsNullable = true)]
        public virtual Sushi.Mediakiwi.Data.CustomData Data
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

        DateTime m_Created;
        /// <summary>
        /// Gets or sets the created.
        /// </summary>
        /// <value>The created.</value>
        [DatabaseColumn("VisitorClick_Created", SqlDbType.DateTime)]
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
        /// TODO: Needs to be moved
        /// </summary>
        [DatabaseColumn("VisitorClick_Campaign_Key", SqlDbType.Int, IsNullable = true)]
        public virtual int? CampaignID { get; set; }
        #endregion Properties


        /// Selects the one.
        /// </summary>
        /// <param name="ID">The ID.</param>
        /// <returns></returns>
        public static IVisitorClick SelectOne(int ID)
        {
            return Parser.SelectOne(ID);
        }

        /// <summary>
        /// Selects all.
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        /// <returns></returns>
        public static IVisitorClick[] SelectAll(DateTime from, DateTime to)
        {
            return Parser.SelectAll(from, to);
        }

        /// <summary>
        /// Gets the log.
        /// </summary>
        /// <value>The log.</value>
        public virtual VisitorLog Log()
        {
            return Parser.Log(this);
        }

        public virtual void Save()
        {
            Parser.Save(this);
        }


        #endregion REPLICATED TO Sushi.Mediakiwi.Data.Standard
    }
}
