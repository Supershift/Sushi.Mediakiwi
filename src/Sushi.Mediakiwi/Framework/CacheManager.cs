using Sushi.Mediakiwi.Data.Caching;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Framework
{
    public class CacheManager : ICacheManager
    {
        private ConcurrentDictionary<string, object> Cache { get; } = new ConcurrentDictionary<string, object>();

        public static string GetCacheKey(string region, string key)
        {
            return $"{region}_{key}";
        }

        public void Add<T>(string region, string key, T item)
        {
            string cacheKey = GetCacheKey(region, key);
            Cache.TryAdd(cacheKey, item);
        }

        public void FlushRegion(string region)
        {
            FlushLocalRegion(region);

            // When the application is loadbalanced, we should inform the other node(s) that they should be flushed.
            // We do this by added the flush command to the database with a timestamp with is extracted/validated on every request (order by this stamp > last Flush) 
            if (CommonConfiguration.IS_LOAD_BALANCED)
            {
                var cacheflush = new Data.CacheItem();
                cacheflush.Created = System.DateTime.UtcNow;
                cacheflush.Name = region;
                cacheflush.Save();
            }
        }

        void FlushLocalRegion(string region)
        {
            var keysToDelete = Cache.Keys.Where(x => x.StartsWith(region + "_", System.StringComparison.InvariantCultureIgnoreCase));
            foreach (var keyToDelete in keysToDelete)
            {
                Cache.TryRemove(keyToDelete, out object val);
            }
        }

        /// <summary>
        /// Validate load balanced caching, the flush commands are given via the datastore
        /// </summary>
        /// <returns></returns>
        public async Task ValidateAsync(System.DateTime environmentLastUpdate)
        {
            if (CommonConfiguration.IS_LOAD_BALANCED)
            {
                if (Data.Caching.Configuration.EnvironmentUpdated.HasValue && Data.Caching.Configuration.EnvironmentUpdated.Value < environmentLastUpdate)
                {
                    // Flush all
                    await FlushAsync().ConfigureAwait(false);
                }
                else
                {
                    // Validate flush regions
                    var lastFlushed = Data.Caching.Configuration.LocalCacheUpdated;
                    if (lastFlushed.HasValue)
                    {
                        var flushregions = await Data.CacheItem.SelectAllAsync(lastFlushed.Value).ConfigureAwait(false);
                        foreach (var region in flushregions)
                        {
                            FlushLocalRegion(region.Name);
                        }
                    }
                }
            }
            Data.Caching.Configuration.LocalCacheUpdated = System.DateTime.UtcNow;
            Data.Caching.Configuration.EnvironmentUpdated = environmentLastUpdate;
        }

        public async Task FlushAsync()
        {
            Cache.Clear();

            if (CommonConfiguration.IS_LOAD_BALANCED)
            {
                await Data.CacheItem.TruncateAsync().ConfigureAwait(false);
            }
        }

        public T Get<T>(string region, string key)
        {
            string cacheKey = GetCacheKey(region, key);
            if (Cache.TryGetValue(cacheKey, out object cachedValue) && cachedValue is T value)
            {
                return value;
            }

            return default(T);
        }
    }
}
