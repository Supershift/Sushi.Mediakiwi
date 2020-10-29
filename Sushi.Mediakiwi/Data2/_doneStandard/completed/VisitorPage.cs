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
    [DatabaseTable("wim_VisitorPages")]
    public class VisitorPage : DatabaseEntity
    {


        #region REPLICATED TO Sushi.Mediakiwi.Data.Standard
        /// <summary>
        /// Selects the keys.
        /// </summary>
        /// <param name="domain">The domain.</param>
        /// <param name="absolutePath">The absolute path.</param>
        /// <returns></returns>
        public static int[] SelectKeys(int domain, string absolutePath)
        {
            VisitorPage candidate = new VisitorPage();
            candidate.SqlJoin = " join wim_VisitorUrls on VisitorUrl_Key = VisitorPage_Url_Key";

            List<DatabaseDataValueColumn> whereClause = new List<DatabaseDataValueColumn>();
            whereClause.Add(new DatabaseDataValueColumn("not VisitorUrl_SiteGuid is null and not VisitorUrl_Name = 'localhost'"));

            if (!string.IsNullOrEmpty(absolutePath))
            {
                absolutePath = string.Format("%{0}%", absolutePath);
                whereClause.Add(new DatabaseDataValueColumn("VisitorPage_Name", SqlDbType.VarChar, absolutePath, DatabaseDataValueCompareType.Like));
            }
            if (domain > 0)
            {
                whereClause.Add(new DatabaseDataValueColumn("VisitorUrl_Key", SqlDbType.Int, domain));
            }

            List<int> list = new List<int>();
            foreach (object obj in candidate._SelectAll(whereClause, true)) list.Add((int)obj);
            return list.ToArray();

        }

        /// <summary>
        /// Gets or sets the ID.
        /// </summary>
        /// <value>The ID.</value>
        [DatabaseColumn("VisitorPage_Key", SqlDbType.Int, IsPrimaryKey = true)]
        public int ID { get; set; }

        /// <summary>
        /// Gets or sets the profile ID.
        /// </summary>
        /// <value>The profile ID.</value>
        [DatabaseColumn("VisitorPage_Url_Key", SqlDbType.Int, IsNullable = true)]
        public int UrlID { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        [DatabaseColumn("VisitorPage_Name", SqlDbType.VarChar, IsNullable = true)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the GUID.
        /// </summary>
        /// <value>The GUID.</value>
        [DatabaseColumn("VisitorPage_PageGuid", SqlDbType.UniqueIdentifier, IsNullable = true)]
        public Guid GUID { get; set; }

        DateTime m_Created;
        /// <summary>
        /// Gets or sets the created.
        /// </summary>
        /// <value>The created.</value>
        [DatabaseColumn("VisitorPage_Created", SqlDbType.DateTime)]
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
        /// Selects the one.
        /// </summary>
        /// <param name="urlID">The URL ID.</param>
        /// <param name="guid">The GUID.</param>
        /// <param name="absolutePath">The absolute path.</param>
        /// <returns></returns>
        public static VisitorPage SelectOne(int urlID, Guid guid, string absolutePath)
        {
            List<DatabaseDataValueColumn> whereClause = new List<DatabaseDataValueColumn>();
            whereClause.Add(new DatabaseDataValueColumn("VisitorPage_Name", SqlDbType.VarChar, absolutePath));
            if (guid == Guid.Empty)
                whereClause.Add(new DatabaseDataValueColumn("VisitorPage_PageGuid", SqlDbType.UniqueIdentifier, null));
            else
                whereClause.Add(new DatabaseDataValueColumn("VisitorPage_PageGuid", SqlDbType.UniqueIdentifier, guid));

            whereClause.Add(new DatabaseDataValueColumn("VisitorPage_Url_Key", SqlDbType.Int, urlID));
            return (VisitorPage)new VisitorPage()._SelectOne(whereClause);
        }


        #endregion REPLICATED TO Sushi.Mediakiwi.Data.Standard
    }
}
