using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Sushi.Mediakiwi.Data.DalReflection;
using Sushi.Mediakiwi.Data.Parsers;
using Sushi.Mediakiwi.Framework;

namespace Sushi.Mediakiwi.Data
{
    [DatabaseTable("wim_CacheItems")]
    public class CacheItem : ICacheItem
    {
        static ICacheItemParser _Parser;
        static ICacheItemParser Parser
        {
            get
            {
                if (_Parser == null)
                    _Parser = Environment.GetInstance<ICacheItemParser>();
                return _Parser;
            }
        }

        #region REPLICATED TO Sushi.Mediakiwi.Data.Standard

        #region Properties
        [DatabaseColumn("CacheItem_Key", SqlDbType.Int, IsPrimaryKey = true)]
        public virtual int ID { get; set; }

        [DatabaseColumn("CacheItem_IsIndex", SqlDbType.Bit, IsNullable = true)]
        public virtual bool IsIndex { get; set; }
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        [DatabaseColumn("CacheItem_Name", SqlDbType.VarChar, Length = 512)]
        public virtual string Name { get; set; }
        /// <summary>
        /// Gets or sets the created.
        /// </summary>
        /// <value>
        /// The created.
        /// </value>
        [DatabaseColumn("CacheItem_Created", SqlDbType.DateTime)]
        public virtual DateTime Created { get; set; }
        #endregion

        public virtual void Save()
        {
            Parser.Save(this);
        }

        /// <summary>
        /// Selects the one.
        /// </summary>
        /// <param name="Key">The key.</param>
        /// <returns></returns>
        public static ICacheItem SelectOne(int Key)
        {
            return Parser.SelectOne(Key);
        }

        /// <summary>
        /// Selects all.
        /// </summary>
        /// <param name="dt">The dt.</param>
        /// <returns></returns>
        public static ICacheItem[] SelectAll(DateTime dt)
        {
            return Parser.SelectAll(dt);
        }
        #endregion REPLICATED TO Sushi.Mediakiwi.Data.Standard

        #region MOVED to EXTENSION / LOGIC

        ///// <summary>
        ///// Clears this instance.
        ///// </summary>
        //public static void Clear()  //LOGIC
        //{
        //    Parser.Clear();
        //}

        ///// <summary>
        ///// Applies the load balanced cache check item.
        ///// </summary>
        ///// <param name="key">The key.</param>
        ///// <param name="isIndexKey">if set to <c>true</c> [is index key].</param>
        //public static void ApplyLoadBalancedCacheCheckItem(string key, bool isIndexKey)
        //{
        //    Parser.ApplyLoadBalancedCacheCheckItem(key, isIndexKey);
        //}

        #endregion MOVED to EXTENSION / LOGIC
    }
}