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
    public partial class AssetTypeParser : IAssetTypeParser
    {
        static ISqlEntityParser _DataParser;
        static ISqlEntityParser DataParser
        {
            get
            {
                if (_DataParser == null)
                    _DataParser = Environment.GetInstance<ISqlEntityParser>();
                return _DataParser;
            }
        }

        #region Context
        public virtual IAssetType SelectOne(int ID)
        {
            return DataParser.SelectOne<IAssetType>(ID, true);
        }

        public virtual IAssetType SelectOne(string Tag)
        {
            List<DatabaseDataValueColumn> whereClause = new List<DatabaseDataValueColumn>();
            whereClause.Add(new DatabaseDataValueColumn("AssetType_Tag", SqlDbType.VarChar, Tag));
            return DataParser.SelectOne<IAssetType>(whereClause);
        }

        public virtual IAssetType[] SelectAll(bool onlyReturnVariant = false)
        {
            List<DatabaseDataValueColumn> whereClause = new List<DatabaseDataValueColumn>();
            if (onlyReturnVariant)
                whereClause.Add(new DatabaseDataValueColumn("AssetType_IsVariant", SqlDbType.Bit, true));
            return DataParser.SelectAll<IAssetType>(whereClause).ToArray();
        }

        public virtual bool Save(IAssetType entity)
        {
            DataParser.Save<IAssetType>(entity);
            return true;
        }

        #endregion Context
    }
}