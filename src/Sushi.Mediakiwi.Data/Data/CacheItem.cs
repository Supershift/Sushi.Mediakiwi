using Sushi.Mediakiwi.Data.MicroORM;
using Sushi.MicroORM;
using Sushi.MicroORM.Mapping;
using System;
using System.Data;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Data
{
    [DataMap(typeof(CacheItemMap))]
    public class CacheItem : ICacheItem
    {
        #region properties

        public class CacheItemMap : DataMap<CacheItem>
        {
            public CacheItemMap()
            {
                Table("wim_CacheItems");
                Id(x => x.ID, "CacheItem_Key").Identity().SqlType(SqlDbType.Int);
                Map(x => x.IsIndex, "CacheItem_IsIndex").SqlType(SqlDbType.Bit);
                Map(x => x.Name, "CacheItem_Name").SqlType(SqlDbType.VarChar).Length(512);
                Map(x => x.Created, "CacheItem_Created").SqlType(SqlDbType.DateTime);
            }
        }

        public int ID { get; set; }
        public bool IsIndex { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the created.
        /// </summary>
        /// <value>
        /// The created.
        /// </value>
        public DateTime Created { get; set; }

        #endregion properties

        /// <summary>
        /// Get an instande of the connector
        /// </summary>
        static Caching.CachedConnector<CacheItem> Connector
        {
            get
            {
                var connector = ConnectorFactory.CreateConnector<CacheItem>();
                // do not cache the select, also preventing from generating cache references 
                connector.UseCacheOnSelect = false;
                return connector;
            }
        }

        /// <summary>
        /// Selects the one.
        /// </summary>
        /// <param name="Key">The key.</param>
        /// <returns></returns>
        public static ICacheItem SelectOne(int Key)
        {
            var connector = Connector;
            return connector.FetchSingle(Key);
        }

        /// <summary>
        /// Select a CacheItem based on its primary key
        /// </summary>
        /// <param name="Key">Uniqe identifier of the Menu</param>
        public static async Task<ICacheItem> SelectOneAsync(int Key)
        {
            var connector = Connector;
            return await connector.FetchSingleAsync(Key);
        }

        /// <summary>
        /// Selects all.
        /// </summary>
        /// <param name="dt">The dt.</param>
        /// <returns></returns>
        public static ICacheItem[] SelectAll(DateTime dt)
        {
            var connector = Connector;
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.Created, dt, ComparisonOperator.GreaterThan);
            return connector.FetchAll(filter).ToArray();
        }

        /// <summary>
        /// Truncate the entire cache registry
        /// </summary>
        /// <returns></returns>
        public static async Task TruncateAsync()
        {
            var connector = Connector;
            await connector.ExecuteNonQueryAsync("truncate table wim_CacheItems").ConfigureAwait(false);
        }

        /// <summary>
        /// Selects all.
        /// </summary>
        /// <param name="dt">The dt.</param>
        /// <returns></returns>
        public static async Task<ICacheItem[]> SelectAllAsync(DateTime dt)
        {
            var connector = Connector;
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.Created, dt, ComparisonOperator.GreaterThan);
            var result = await connector.FetchAllAsync(filter);
            return result.ToArray();
        }

        public void Save()
        {
            var connector = Connector;
            connector.Save(this);
        }
    }
}