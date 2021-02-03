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
    [DatabaseTable("wim_VisitorUrls")]
    public class VisitorUrl : DatabaseEntity
    {
        #region REPLICATED TO Sushi.Mediakiwi.Data.Standard

        /// <summary>
        /// Selects the one.
        /// </summary>
        /// <param name="guid">The GUID.</param>
        /// <param name="host">The host.</param>
        /// <returns></returns>
        public static VisitorUrl SelectOne(Guid guid, string host)
        {
            List<DatabaseDataValueColumn> whereClause = new List<DatabaseDataValueColumn>();
            whereClause.Add(new DatabaseDataValueColumn("VisitorUrl_Name", SqlDbType.VarChar, host));
            
            if (guid == Guid.Empty)
                whereClause.Add(new DatabaseDataValueColumn("VisitorUrl_SiteGuid", SqlDbType.UniqueIdentifier, null));
            else
                whereClause.Add(new DatabaseDataValueColumn("VisitorUrl_SiteGuid", SqlDbType.UniqueIdentifier, guid));

            return (VisitorUrl)new VisitorUrl()._SelectOne(whereClause);
        }

        public static List<VisitorUrl> SelectAll()
        {
            List<VisitorUrl> list = new List<VisitorUrl>();
            foreach (object obj in new VisitorUrl()._SelectAll()) list.Add((VisitorUrl)obj);
            return list;
        }

        /// <summary>
        /// Gets or sets the ID.
        /// </summary>
        /// <value>The ID.</value>
        [DatabaseColumn("VisitorUrl_Key", SqlDbType.Int, IsPrimaryKey = true)]
        public int ID { get; set; }

        [DatabaseColumn("VisitorUrl_Name", SqlDbType.VarChar, IsNullable = true)]
        public string Name { get; set; }

        [DatabaseColumn("VisitorUrl_SiteGuid", SqlDbType.UniqueIdentifier, IsNullable = true)]
        public Guid GUID { get; set; }

        DateTime m_Created;
        /// <summary>
        /// Gets or sets the created.
        /// </summary>
        /// <value>The created.</value>
        [DatabaseColumn("VisitorUrl_Created", SqlDbType.DateTime)]
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
