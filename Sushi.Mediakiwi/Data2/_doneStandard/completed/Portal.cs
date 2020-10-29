using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using Sushi.Mediakiwi.Data.DalReflection;
using Sushi.Mediakiwi.Data.Parsers;

namespace Sushi.Mediakiwi.Data
{
    /// <summary>
    /// 
    /// </summary>
    [DatabaseTable("wim_Portals")]
    public class Portal : IPortal
    {
 
        static IPortalParser _Parser;
        static IPortalParser Parser
        {
            get
            {
                if (_Parser == null)
                    _Parser = Environment.GetInstance<IPortalParser>();
                return _Parser;
            }
        }

        #region REPLICATED TO Sushi.Mediakiwi.Data.Standard

        public void Delete()
        {
            Parser.Delete(this);
        }

        ///// <summary>
        ///// Connects the specified domain.
        ///// </summary>
        ///// <param name="domain">The domain.</param>
        ///// <returns></returns>
        //public static wimServerCommunication.WebInformationManagerServerService Connect(string domain)
        //{
        //    return Parser.Connect(domain);
        //}

        /// <summary>
        /// Selects the one.
        /// </summary>
        /// <param name="ID">The ID.</param>
        /// <returns></returns>
        public static IPortal SelectOne(int ID)
        {
            return Parser.SelectOne(ID);
        }

        /// <summary>
        /// Selects the one.
        /// </summary>
        /// <param name="authenticode">The authenticode.</param>
        /// <returns></returns>
        public static IPortal SelectOne(string authenticode)
        {
            return Parser.SelectOne(authenticode);
        }

        /// <summary>
        /// Selects the one.
        /// </summary>
        /// <param name="guid">The GUID.</param>
        /// <returns></returns>
        public static IPortal SelectOne(Guid guid)
        {
            return Parser.SelectOne(guid);
        }

        /// <summary>
        /// Selects all.
        /// </summary>
        /// <param name="userID">The user ID.</param>
        /// <returns></returns>
        public static IPortal[] SelectAll(int roleID)
        {
            return Parser.SelectAll(roleID);
        }

        public void Save()
        {
            Parser.Save(this);
        }

        /// <summary>
        /// Gets or sets the ID.
        /// </summary>
        /// <value>The ID.</value>
        [DatabaseColumn("Portal_Key", SqlDbType.Int, IsPrimaryKey = true)]
        public int ID { get; set; }

        private Guid m_GUID;
        /// <summary>
        /// Unique identifier used for import/export
        /// </summary>
        /// <value>The GUID.</value>
        [DatabaseColumn("Portal_GUID", SqlDbType.UniqueIdentifier, IsMigrationKey = true)]
        public Guid GUID
        {
            get {
                if (this.m_GUID == Guid.Empty) this.m_GUID = Guid.NewGuid();
                return m_GUID; 
            }
            set { m_GUID = value; }
        }

        /// <summary>
        /// Gets or sets the user ID.
        /// </summary>
        /// <value>The user ID.</value>
        [DatabaseColumn("Portal_User_Key", SqlDbType.Int)]
        public int UserID { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        [DatabaseColumn("Portal_Name", SqlDbType.NVarChar, Length = 50)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the domain.
        /// </summary>
        /// <value>The domain.</value>
        [Sushi.Mediakiwi.Framework.ContentListItem.TextField("Domain", 50, true)]
        [DatabaseColumn("Portal_Domain", SqlDbType.NVarChar, Length = 50)]
        public string Domain { get; set; }

        /// <summary>
        /// Gets or sets the authenticode.
        /// </summary>
        /// <value>The authenticode.</value>
        [DatabaseColumn("Portal_Authenticode", SqlDbType.VarChar, Length = 150, IsNullable = true)]
        public string Authenticode { get; set; }

        /// <summary>
        /// Gets or sets the authentication.
        /// </summary>
        /// <value>The authentication.</value>
        [DatabaseColumn("Portal_Authentication", SqlDbType.VarChar, Length = 150, IsNullable = true)]
        public string Authentication { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is active.
        /// </summary>
        /// <value><c>true</c> if this instance is active; otherwise, <c>false</c>.</value>
        [DatabaseColumn("Portal_IsActive", SqlDbType.Bit)]
        public bool IsActive { get; set; }

        Sushi.Mediakiwi.Data.CustomData m_Data;
        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        /// <value>The data.</value>
        [DatabaseColumn("Portal_Data", SqlDbType.Xml, IsNullable = true)]
        public Sushi.Mediakiwi.Data.CustomData Data
        {
            get
            {
                if (m_Data == null)
                    m_Data = new CustomData();
                return m_Data;
            }
            set { m_Data = value; }
        }

        DateTime m_Created;
        /// <summary>
        /// Gets or sets the created.
        /// </summary>
        /// <value>The created.</value>
        [DatabaseColumn("Portal_Created", SqlDbType.DateTime)]
        public DateTime Created
        {
            get
            {
                if (this.m_Created == DateTime.MinValue) this.m_Created = Sushi.Mediakiwi.Data.Common.DatabaseDateTime.Date;
                return m_Created;
            }
            set { m_Created = value; }
        }

        #endregion REPLICATED TO Sushi.Mediakiwi.Data.Standard
    }
}
