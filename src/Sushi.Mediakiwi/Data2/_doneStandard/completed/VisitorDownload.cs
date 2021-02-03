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
    [DatabaseTable("wim_VisitorDownloads")]
    public class VisitorDownload : DatabaseEntity
    {
        #region REPLICATED TO Sushi.Mediakiwi.Data.Standard

        /// <summary>
        /// Selects the one.
        /// </summary>
        /// <param name="ID">The ID.</param>
        /// <returns></returns>
        public static VisitorDownload SelectOne(int ID)
        {
            return (VisitorDownload)new VisitorDownload()._SelectOne(ID);
        }

        /// <summary>
        /// Selects all.
        /// </summary>
        /// <returns></returns>
        public static VisitorDownload[] SelectAll()
        {
            List<VisitorDownload> list = new List<VisitorDownload>();
            foreach (object obj in new VisitorDownload()._SelectAll()) list.Add((VisitorDownload)obj);
            return list.ToArray();
        }

        /// <summary>
        /// Gets or sets the ID.
        /// </summary>
        /// <value>The ID.</value>
        [DatabaseColumn("VisitorDownload_Key", SqlDbType.Int, IsPrimaryKey = true)]
        public int ID { get; set; }


        /// <summary>
        /// Gets or sets the visitor log ID.
        /// </summary>
        /// <value>The visitor log ID.</value>
        [DatabaseColumn("VisitorDownload_VisitorLog_Key", SqlDbType.Int, IsNullable = true)]
        public int VisitorLogID { get; set; }

        /// <summary>
        /// Gets or sets the profile ID.
        /// </summary>
        /// <value>The profile ID.</value>
        [DatabaseColumn("VisitorDownload_Asset_Key", SqlDbType.Int, IsNullable = true)]
        public int AssetID { get; set; }

        DateTime m_Created;
        /// <summary>
        /// Gets or sets the created.
        /// </summary>
        /// <value>The created.</value>
        [DatabaseColumn("VisitorDownload_Created", SqlDbType.DateTime)]
        public DateTime Created
        {
            get
            {
                if (this.m_Created == DateTime.MinValue) this.m_Created = Sushi.Mediakiwi.Data.Common.DatabaseDateTime;
                return m_Created;
            }
            set { m_Created = value; }
        }


        #endregion REPLICATED TO Sushi.Mediakiwi.Data.Standard
    }
}
