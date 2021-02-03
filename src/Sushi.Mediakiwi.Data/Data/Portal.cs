using Sushi.MicroORM;
using Sushi.MicroORM.Mapping;
using System;
using System.Threading.Tasks;
using Sushi.Mediakiwi.Data.MicroORM;

namespace Sushi.Mediakiwi.Data
{
    [DataMap(typeof(PortalMap))]
    public class Portal : IPortal
    {
        public class PortalMap : DataMap<Portal>
        {
            public PortalMap()
            {
                Table("wim_Portals");
                Id(x => x.ID, "Portal_Key").Identity();
                Map(x => x.GUID, "Portal_GUID");
                Map(x => x.UserID, "Portal_User_Key");
                Map(x => x.Name, "Portal_Name").Length(50);
                Map(x => x.Domain, "Portal_Domain").Length(50);
                Map(x => x.Authenticode, "Portal_Authenticode").Length(150);
                Map(x => x.Authentication, "Portal_Authentication").Length(150);
                Map(x => x.IsActive, "Portal_IsActive");
                Map(x => x.Created, "Portal_Created");
                Map(x => x.DataString, "Portal_Data").SqlType(System.Data.SqlDbType.Xml);
            }
        }

        #region Properties

        /// <summary>
        /// Gets or sets the ID.
        /// </summary>
        /// <value>The ID.</value>
        public int ID { get; set; }

        private Guid m_GUID;

        /// <summary>
        /// Unique identifier used for import/export
        /// </summary>
        /// <value>The GUID.</value>
        public Guid GUID
        {
            get
            {
                if (this.m_GUID == Guid.Empty) this.m_GUID = Guid.NewGuid();
                return m_GUID;
            }
            set { m_GUID = value; }
        }

        /// <summary>
        /// Gets or sets the user ID.
        /// </summary>
        /// <value>The user ID.</value>
        public int UserID { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the domain.
        /// </summary>
        /// <value>The domain.</value>
        public string Domain { get; set; }

        /// <summary>
        /// Gets or sets the authenticode.
        /// </summary>
        /// <value>The authenticode.</value>
        public string Authenticode { get; set; }

        /// <summary>
        /// Gets or sets the authentication.
        /// </summary>
        /// <value>The authentication.</value>
        public string Authentication { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is active.
        /// </summary>
        /// <value><c>true</c> if this instance is active; otherwise, <c>false</c>.</value>
        public bool IsActive { get; set; }

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
                    m_Created = Common.DatabaseDateTime.Date;
                return m_Created;
            }
            set { m_Created = value; }
        }

        private CustomData m_Data;

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

        public string DataString { get; set; }

        #endregion Properties

        public void Delete()
        {
            var connector = ConnectorFactory.CreateConnector<Portal>();
            connector.Delete(this);
        }

        public async Task DeleteAsync()
        {
            var connector = ConnectorFactory.CreateConnector<Portal>();
            await connector.DeleteAsync(this);
        }

        /// <summary>
        /// Selects a portal based on the Identifier.
        /// </summary>
        /// <param name="ID">The ID.</param>
        /// <returns></returns>
        public static IPortal SelectOne(int ID)
        {
            var connector = ConnectorFactory.CreateConnector<Portal>();
            return connector.FetchSingle(ID);
        }

        /// <summary>
        /// Selects a portal based on the Identifier Async.
        /// </summary>
        /// <param name="ID">The ID.</param>
        /// <returns></returns>
        public static async Task<IPortal> SelectOneAsync(int ID)
        {
            var connector = ConnectorFactory.CreateConnector<Portal>();
            return await connector.FetchSingleAsync(ID);
        }

        /// <summary>
        /// Selects a portal based on the authenticode.
        /// </summary>
        /// <param name="authenticode">The authenticode.</param>
        /// <returns></returns>
        public static IPortal SelectOne(string authenticode)
        {
            var connector = ConnectorFactory.CreateConnector<Portal>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.Authenticode, authenticode);

            return connector.FetchSingle(filter);
        }

        /// <summary>
        /// Selects a portal based on the authenticode Async.
        /// </summary>
        /// <param name="authenticode">The authenticode.</param>
        /// <returns></returns>
        public static async Task<IPortal> SelectOneAsync(string authenticode)
        {
            var connector = ConnectorFactory.CreateConnector<Portal>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.Authenticode, authenticode);

            return await connector.FetchSingleAsync(filter);
        }

        /// <summary>
        /// Selects a portal based on the guid.
        /// </summary>
        /// <param name="guid">The GUID.</param>
        /// <returns></returns>
        public static IPortal SelectOne(Guid guid)
        {
            var connector = ConnectorFactory.CreateConnector<Portal>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.GUID, guid);

            return connector.FetchSingle(filter);
        }

        /// <summary>
        /// Selects a portal based on the guid Async.
        /// </summary>
        /// <param name="guid">The GUID.</param>
        /// <returns></returns>
        public static async Task<IPortal> SelectOneAsync(Guid guid)
        {
            var connector = ConnectorFactory.CreateConnector<Portal>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.GUID, guid);

            return await connector.FetchSingleAsync(filter);
        }

        /// <summary>
        /// Selects all.
        /// </summary>
        /// <param name="userID">The user ID.</param>
        /// <returns></returns>
        public static IPortal[] SelectAll()
        {
            var connector = ConnectorFactory.CreateConnector<Portal>();
            var filter = connector.CreateDataFilter();

            return connector.FetchAll(filter).ToArray();
        }

        /// <summary>
        /// Selects all.
        /// </summary>
        /// <param name="userID">The user ID.</param>
        /// <returns></returns>
        public static async Task<IPortal[]> SelectAllAsync()
        {
            var connector = ConnectorFactory.CreateConnector<Portal>();
            var filter = connector.CreateDataFilter();

            var result = await connector.FetchAllAsync(filter);
            return result.ToArray();
        }

        public void Save()
        {
            var connector = ConnectorFactory.CreateConnector<Portal>();
            connector.Save(this);
        }

        public async Task SaveAsync()
        {
            var connector = ConnectorFactory.CreateConnector<Portal>();
            await connector.SaveAsync(this);
        }
    }
}