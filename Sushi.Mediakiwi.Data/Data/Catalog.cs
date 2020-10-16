using Sushi.MicroORM;
using Sushi.MicroORM.Mapping;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Sushi.Mediakiwi.Data.MircoORM;

namespace Sushi.Mediakiwi.Data
{
    [DataMap(typeof(CatalogMap))]
    public class Catalog
    {
        public class CatalogMap : DataMap<Catalog>
        {
            public CatalogMap()
            {
                Table("wim_Catalogs");
                Id(x => x.ID, "Catalog_Key").Identity().SqlType(SqlDbType.Int);
                Map(x => x.GUID, "Catalog_GUID").SqlType(SqlDbType.UniqueIdentifier);
                Map(x => x.Title, "Catalog_Title").SqlType(SqlDbType.NVarChar).Length(50);
                Map(x => x.Table, "Catalog_Table");
                Map(x => x.IsActive, "Catalog_IsActive").SqlType(SqlDbType.Bit);
                Map(x => x.ColumnPrefix, "Catalog_ColumnPrefix").SqlType(SqlDbType.VarChar).Length(25);
                Map(x => x.Created, "Catalog_Created").SqlType(SqlDbType.DateTime);
                Map(x => x.HasSortOrder, "Catalog_HasSortOrder").SqlType(SqlDbType.Bit);
                Map(x => x.HasCatalogBaseStructure, "Catalog_HasInternalRef").SqlType(SqlDbType.Bit);
                Map(x => x.ConnectionIndex, "Catalog_Connection").SqlType(SqlDbType.Int);
                Map(x => x.PortalName, "Catalog_Portal").SqlType(SqlDbType.VarChar);
                Map(x => x.ColumnKey, "Catalog_ColumnKey").SqlType(SqlDbType.VarChar).Length(30);
                Map(x => x.ColumnGuid, "Catalog_ColumnGuid").SqlType(SqlDbType.VarChar).Length(30);
                Map(x => x.ColumnData, "Catalog_ColumnData").SqlType(SqlDbType.VarChar).Length(30);
            }
        }

        #region properties

        /// <summary>
        /// Gets or sets the ID.
        /// </summary>
        /// <value>The ID.</value>
        public int ID { get; set; }

        private Guid m_GUID;

        /// <summary>
        /// Gets or sets the GUID.
        /// </summary>
        /// <value>The GUID.</value>
        public Guid GUID
        {
            get
            {
                if (m_GUID == Guid.Empty) 
                    m_GUID = Guid.NewGuid();
                return m_GUID;
            }
            set { m_GUID = value; }
        }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>The title.</value>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the table.
        /// </summary>
        /// <value>The table.</value>
        public string Table { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is active.
        /// </summary>
        /// <value><c>true</c> if this instance is active; otherwise, <c>false</c>.</value>
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets the column prefix.
        /// </summary>
        /// <value>The column prefix.</value>
        public string ColumnPrefix { get; set; }

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
                    m_Created = Common.DatabaseDateTime;
                return m_Created;
            }
            set { m_Created = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance has data column.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has data column; otherwise, <c>false</c>.
        /// </value>
        public bool HasSortOrder { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance has internal references.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has internal references; otherwise, <c>false</c>.
        /// </value>
        public bool HasCatalogBaseStructure { get; set; }

        /// <summary>
        /// Gets or sets the connection.
        /// </summary>
        /// <value>The connection.</value>
        public int ConnectionIndex { get; set; }

        public string PortalName { get; set; }

        /// <summary>
        /// Gets or sets the column key.
        /// </summary>
        /// <value>The column key.</value>
        public string ColumnKey { get; set; }

        /// <summary>
        /// Gets or sets the column GUID.
        /// </summary>
        /// <value>The column GUID.</value>
        public string ColumnGuid { get; set; }

        public string ColumnData { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance is new.
        /// </summary>
        /// <value><c>true</c> if this instance is new; otherwise, <c>false</c>.</value>
        public bool IsNew
        {
            get { return this.ID == 0; }
        }

        #endregion properties

        /// <summary>
        /// Selects the one.
        /// </summary>
        /// <param name="title">The title.</param>
        public static Catalog SelectOne(string tableName)
        {
            var connector = ConnectorFactory.CreateConnector<Catalog>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.Table, tableName);
            return connector.FetchSingle(filter);
        }

        /// <summary>
        /// Selects the one.
        /// </summary>
        /// <param name="title">The title.</param>
        public static async Task<Catalog> SelectOneAsync(string tableName)
        {
            var connector = ConnectorFactory.CreateConnector<Catalog>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.Table, tableName);
            return await connector.FetchSingleAsync(filter);
        }

        /// <summary>
        /// Selects all.
        /// </summary>
        public static Catalog[] SelectAll()
        {
            var connector = ConnectorFactory.CreateConnector<Catalog>();
            var filter = connector.CreateDataFilter();
            // Order = "Catalog_Title ASC"
            filter.AddOrder(x => x.Title);
            return connector.FetchAll(filter).ToArray();
        }

        /// <summary>
        /// Selects all.
        /// </summary>
        public static async Task<Catalog[]> SelectAllAsync()
        {
            var connector = ConnectorFactory.CreateConnector<Catalog>();
            var filter = connector.CreateDataFilter();
            // Order = "Catalog_Title ASC"
            filter.AddOrder(x => x.Title);
            var result = await connector.FetchAllAsync(filter);
            return result.ToArray();
        }

        /// <summary>
        /// Deletes the SQL table.
        /// </summary>
        public void DeleteSqlTable()
        {
            var connector = ConnectorFactory.CreateConnector<Catalog>();
            connector.ExecuteNonQuery("DROP TABLE " + this.Table);
			connector.Cache.FlushRegion(connector.CacheRegion);
        }

        /// <summary>
        /// Deletes the SQL table.
        /// </summary>
        public async Task DeleteSqlTableAsync()
        {
            var connector = ConnectorFactory.CreateConnector<Catalog>();
            await connector.ExecuteNonQueryAsync("DROP TABLE " + this.Table);
			connector.Cache.FlushRegion(connector.CacheRegion);
        }
    }
}