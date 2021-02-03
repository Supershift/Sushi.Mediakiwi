using System;
using System.Data;
using System.Data.Linq.Mapping;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sushi.Mediakiwi.Data.DalReflection;
using Sushi.Mediakiwi.Framework;

namespace Sushi.Mediakiwi.Data
{
    [DatabaseTable("wim_AssetTypes", Order= "AssetType_Name asc")]
    public partial class AssetType : IAssetType
    {
        static IAssetTypeParser _Parser;
        static IAssetTypeParser Parser
        {
            get
            {
                if (_Parser == null)
                    _Parser = Environment.GetInstance<IAssetTypeParser>();
                return _Parser;
            }
        }

        #region REPLICATED TO Sushi.Mediakiwi.Data.Standard

        public AssetType() { }

        #region Context
        public static IAssetType SelectOne(int ID)
        {
            return Parser.SelectOne(ID);
        }

        public static IAssetType SelectOne(string Tag)
        {
            return Parser.SelectOne(Tag);
        }

        public static IAssetType[] SelectAll(bool onlyReturnVariant = false)
        {
            return Parser.SelectAll(onlyReturnVariant);
        }

        public virtual bool Save()
        {
            return Parser.Save(this);
        }

        #endregion Context

        #region Properties
        public virtual bool IsNewInstance
        {
            get { return this.ID == 0; }
        }

        [DatabaseColumn("AssetType_Key", SqlDbType.Int, IsPrimaryKey = true)]
        public virtual int ID { get; set; }

        [DatabaseColumn("AssetType_Guid", SqlDbType.UniqueIdentifier, IsMigrationKey = true)]
        public virtual Guid Guid { get; set; }

        [DatabaseColumn("AssetType_Tag", SqlDbType.VarChar)]
        public virtual string Tag { get; set; }

        [DatabaseColumn("AssetType_Name", SqlDbType.NVarChar)]
        public virtual string Name { get; set; }

        [DatabaseColumn("AssetType_Created", SqlDbType.DateTime)]
        public virtual DateTime Created { get; set; }

        [DatabaseColumn("AssetType_IsVariant", SqlDbType.Bit)]
        public virtual bool IsVariant { get; set; }
        #endregion properties

        #endregion REPLICATED TO Sushi.Mediakiwi.Data.Standard
    }
}