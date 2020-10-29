using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Data
{
    public class CacheItemLogic
    {
        /// <summary>
        /// Applies the load balanced cache check item.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="isIndexKey">if set to <c>true</c> [is index key].</param>
        public static void ApplyLoadBalancedCacheCheckItem(string key, bool isIndexKey)
        {
            if (!CommonConfiguration.IS_LOAD_BALANCED) 
                return;

            var cache = new CacheItem();
            cache.Name = key;
            cache.IsIndex = isIndexKey;
            cache.Created = Common.DatabaseDateTime;
            cache.Save();

            Framework.Caching.Add("Node.TimeStamp", cache.Created);
        }
    }
}
