using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Sushi.Mediakiwi.Data.DalReflection;
using Sushi.Mediakiwi.Framework;

namespace Sushi.Mediakiwi.Data.Parsers
{
    public class CacheItemParser : ICacheItemParser
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

        /// <summary>
        /// Clears this instance.
        /// </summary>
        public virtual void Clear()     //LOGIC
        {
            if (!Wim.CommonConfiguration.IS_LOAD_BALANCED) return;
            DataParser.Execute("truncate table wim_CacheItems");
        }

        /// <summary>
        /// Applies the load balanced cache check item.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="isIndexKey">if set to <c>true</c> [is index key].</param>
        public virtual void ApplyLoadBalancedCacheCheckItem(string key, bool isIndexKey)    //LOGIC
        {
            if (!Wim.CommonConfiguration.IS_LOAD_BALANCED) return;

            string checkKeyStart = string.Format("NHibernate-Cache:{0}:{0}#", "Sushi.Mediakiwi.Data.CacheItem");
            if (key.StartsWith(checkKeyStart, StringComparison.OrdinalIgnoreCase))
                return;

            
            var cache = Sushi.Mediakiwi.Data.Environment.GetInstance<ICacheItem>();
            cache.Name = key;
            cache.IsIndex = isIndexKey;
            cache.Created = Sushi.Mediakiwi.Data.Common.DatabaseDateTime;
            cache.Save();

            if (System.Web.HttpContext.Current == null)
                return;

            //  get last known timestamp first!!!!
            System.Web.HttpContext.Current.Cache.Insert("Node.TimeStamp", cache.Created, null, DateTime.Now.AddYears(1), TimeSpan.Zero);
        }

        public virtual void Save(ICacheItem entity)
        {
            DataParser.Save<ICacheItem>(entity);
        }

        /// <summary>
        /// Selects the one.
        /// </summary>
        /// <param name="Key">The key.</param>
        /// <returns></returns>
        public virtual ICacheItem SelectOne(int Key)
        {
            return DataParser.SelectOne<ICacheItem>(Key, false);
        }

        /// <summary>
        /// Selects all.
        /// </summary>
        /// <param name="dt">The dt.</param>
        /// <returns></returns>
        public virtual ICacheItem[] SelectAll(DateTime dt)
        {
            DataRequest data = new DataRequest();
            data.AddWhere(nameof(ICacheItem.Created), dt);
          

            List<DatabaseDataValueColumn> whereClause = new List<DatabaseDataValueColumn>();
            whereClause.Add(new DatabaseDataValueColumn("CacheItem_Created", SqlDbType.DateTime, dt, DatabaseDataValueCompareType.BiggerThen));
            return DataParser.SelectAll<ICacheItem>(whereClause).ToArray();
        }
    }
}