using Sushi.Mediakiwi.Data.Caching;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Tests
{
    public class TestCacheManager : ICacheManager
    {
        public TestCacheManager()
        {

        }

        private ConcurrentDictionary<string, object> Cache { get; } = new ConcurrentDictionary<string, object>();

        public string GetCacheKey(string region, string key)
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
            var keysToDelete = Cache.Keys.Where(x => x.StartsWith(region + "_"));
            foreach (var keyToDelete in keysToDelete)
                Cache.TryRemove(keyToDelete, out object val);
        }

        public T Get<T>(string region, string key)
        {
            string cacheKey = GetCacheKey(region, key);
            if (Cache.TryGetValue(cacheKey, out object cachedValue))
            {
                if (cachedValue is T)
                    return (T)cachedValue;
            }
            
            return default(T);
        }

        public Task ValidateAsync(System.DateTime changed)
        {
            throw new System.NotImplementedException();
        }

        public Task FlushAsync()
        {
            throw new System.NotImplementedException();
        }
    }
}
