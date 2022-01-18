using Sushi.Mediakiwi.Data.MicroORM;
using Sushi.MicroORM.Mapping;
using System;
using System.Data;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Data
{
    [DataMap(typeof(AssetTypeMap))]
    public class AssetType : IAssetType
    {
        public AssetType()
        {
        }

        public bool IsNewInstance
        {
            get { return this.ID == 0; }
        }

        #region properties

        public class AssetTypeMap : DataMap<AssetType>
        {
            public AssetTypeMap()
            {
                Table("wim_AssetTypes");
                Id(x => x.ID, "AssetType_Key").Identity().SqlType(SqlDbType.Int);
                Map(x => x.Guid, "AssetType_Guid").SqlType(SqlDbType.UniqueIdentifier);
                Map(x => x.Tag, "AssetType_Tag").SqlType(SqlDbType.VarChar);
                Map(x => x.Name, "AssetType_Name").SqlType(SqlDbType.NVarChar);
                Map(x => x.Created, "AssetType_Created").SqlType(SqlDbType.DateTime);
                Map(x => x.IsVariant, "AssetType_IsVariant").SqlType(SqlDbType.Bit);
            }
        }

        public int ID { get; set; }

        public Guid Guid { get; set; }

        public string Tag { get; set; }

        public string Name { get; set; }

        public DateTime Created { get; set; }

        public bool IsVariant { get; set; }

        #endregion properties

        /// <summary>
        /// Select a AssetType based on its primary key
        /// </summary>
        /// <param name="ID">Uniqe identifier of the AssetType</param>
        public static IAssetType SelectOne(int ID)
        {
            var connector = ConnectorFactory.CreateConnector<AssetType>();
            return connector.FetchSingle(ID);
        }

        /// <summary>
        /// Select a AssetType based on its primary key
        /// </summary>
        /// <param name="ID">Uniqe identifier of the AssetType</param>
        public static async Task<IAssetType> SelectOneAsync(int ID)
        {
            var connector = ConnectorFactory.CreateConnector<AssetType>();
            return await connector.FetchSingleAsync(ID);
        }

        /// <summary>
        /// Select a AssetType based on AssetType_Tag
        /// </summary>
        /// <param name="Tag">Identifier of the Tag</param>
        public static IAssetType SelectOne(string Tag)
        {
            var connector = ConnectorFactory.CreateConnector<AssetType>();
            var filter = connector.CreateQuery();
            filter.Add(x => x.Tag, Tag);
            return connector.FetchSingle(filter);
        }

        /// <summary>
        /// Select a AssetType based on AssetType_Tag
        /// </summary>
        /// <param name="Tag">Identifier of the Tag</param>
        public static async Task<IAssetType> SelectOneAsync(string Tag)
        {
            var connector = ConnectorFactory.CreateConnector<AssetType>();
            var filter = connector.CreateQuery();
            filter.Add(x => x.Tag, Tag);
            return await connector.FetchSingleAsync(filter);
        }

        /// <summary>
        /// Select all AssetTypes
        /// </summary>
        /// <param name="onlyReturnVariant">When set to 'true', only AssetType_IsVariant == '1' will be selected.</param>
        public static IAssetType[] SelectAll(bool onlyReturnVariant = false)
        {
            var connector = ConnectorFactory.CreateConnector<AssetType>();
            var filter = connector.CreateQuery();
            if (onlyReturnVariant)
                filter.Add(x => x.IsVariant, true);
            filter.AddOrder(x => x.Name);
            return connector.FetchAll(filter).ToArray();
        }

        /// <summary>
        /// Select all AssetTypes
        /// </summary>
        /// <param name="onlyReturnVariant">When set to 'true', only AssetType_IsVariant == '1' will be selected.</param>
        public static async Task<IAssetType[]> SelectAllAsync(bool onlyReturnVariant = false)
        {
            var connector = ConnectorFactory.CreateConnector<AssetType>();
            var filter = connector.CreateQuery();
            if (onlyReturnVariant)
                filter.Add(x => x.IsVariant, true);
            filter.AddOrder(x => x.Name);
            var result = await connector.FetchAllAsync(filter);
            return result.ToArray();
        }

        public bool Save()
        {
            var connector = ConnectorFactory.CreateConnector<AssetType>();
            connector.Save(this);

            return true;
        }

        public async Task<bool> SaveAsync()
        {
            var connector = ConnectorFactory.CreateConnector<AssetType>();
            await connector.SaveAsync(this);

            return true;
        }
    }
}