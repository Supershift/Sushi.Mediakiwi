using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Microsoft.Extensions.Caching.Memory;
using Sushi.Mediakiwi.Data;

namespace Sushi.Mediakiwi.Framework
{
    //deze class is ook verhuis naar Sushi.Mediakiwi.Data.Standard
    
    /// <summary>
    /// This class manages objects in server cache. 
    /// </summary>
    public class Caching
    {
        static MemoryCache _Memory;
        static MemoryCache Memory
        {
            get
            {
                if (_Memory == null)
                {
                    _Memory = new MemoryCache(new MemoryCacheOptions() {  });
                    _CacheLookup.TryAdd("CACHE_STARTED", DateTime.UtcNow);
                }
                return _Memory;
            }
            set
            {
                _Memory = value;
            }
        }

        static void Reset(bool resetDefault = true)
        {
            //if (resetDefault)
            //    MemoryCache.Default.Dispose();

            if (_Memory == null)
                return;

            var mem = _Memory;
            _Memory = new MemoryCache(new MemoryCacheOptions() { });
            mem.Dispose();
        }

        internal static ConcurrentDictionary<string, DateTime> _CacheLookup = new ConcurrentDictionary<string, DateTime>();

        public static bool IsCached(string key)
        {
            if (CommonConfiguration.DISABLE_CACHE)
                return false;

            return Memory.TryGetValue(key, out _);
        }

        /// <summary>
        /// Determines whether [is cached object] [the specified key].
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="item">The item.</param>
        /// <returns>
        ///   <c>true</c> if [is cached object] [the specified key]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsCached<T>(string key, out T item)
        {
            item = default(T);
            if (CommonConfiguration.DISABLE_CACHE)
                return false;

            return Memory.TryGetValue<T>(key, out item);
        }

        public static bool IsCached<T>(string key, out List<T> item)
        {
            item = default(List<T>);
            if (CommonConfiguration.DISABLE_CACHE)
                return false;

            return Memory.TryGetValue<List<T>>(key, out item);
        }

        /// <summary>
        /// Determines whether [is cached index] [the specified cache key].
        /// </summary>
        /// <param name="cacheKey">The cache key.</param>
        /// <returns>
        ///   <c>true</c> if [is cached index] [the specified cache key]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsCachedIndex(string cacheKey)
        {
            foreach(var key in _CacheLookup.Keys)
            {
                if (key?.IndexOf(cacheKey) > -1)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Gets the item.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public T GetItem<T>(string key)
        {
            T candidate;
            IsCached(key, out candidate);
            return candidate;
        }

        public static void Add(string key, object item)
        {
            if (CommonConfiguration.DISABLE_CACHE)
                return;
            Add(key, item, CommonConfiguration.DefaultCacheTime);
        }

        /// <summary>
        /// Adds the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="item">The item.</param>
        /// <param name="expiration">The expiration.</param>
        public static void Add(string key, object item, DateTime expiration)
        {
            if (CommonConfiguration.DISABLE_CACHE)
                return;

            Memory.Set(key, item, expiration);
            _CacheLookup.TryAdd(key, DateTime.UtcNow);

            //if (Memory.Add(key, item, new CacheItemPolicy() { AbsoluteExpiration = new DateTimeOffset(expiration) }))
            //    _CacheLookup.TryAdd(key, DateTime.UtcNow);
        }

        /// <summary>
        /// Adds the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="item">The item.</param>
        /// <param name="slidingExpiration">The sliding expiration.</param>
        /// <param name="metaInformation">The meta information.</param>
        /// <param name="filename">The filename (full local path).</param>
        public static void Add(string key, object item, TimeSpan slidingExpiration)
        {
            if (CommonConfiguration.DISABLE_CACHE)
                return;
            //if (Context != null)
            //    Context.Cache.Insert(key, item, null, Cache.NoAbsoluteExpiration, slidingExpiration);
            //else
            Memory.Set(key, item, slidingExpiration);
            _CacheLookup.TryAdd(key, DateTime.UtcNow);

            //if (Memory.Add(key, item, new CacheItemPolicy() { SlidingExpiration = slidingExpiration }))
            //    _CacheLookup.TryAdd(key, DateTime.UtcNow);
        }

        /// <summary>
        /// Flushes the cache object.
        /// </summary>
        /// <param name="cacheKey">The cache key.</param>
        /// <returns></returns>
        public static bool FlushCache(string cacheKey)
        {
            return FlushCache(cacheKey, false);
        }

        /// <summary>
        /// Flushes the cache object.
        /// </summary>
        /// <param name="cacheKey">The cache key.</param>
        /// <param name="igNoreLoadBalancedCacheCheck">if set to <c>true</c> [ig nore load balanced cache check].</param>
        /// <returns></returns>
        public static bool FlushCache(string cacheKey, bool igNoreLoadBalancedCacheCheck)
        {
            if (Caching.IsCached(cacheKey))
            {
                Memory.Remove(cacheKey);

                if (Data.CommonConfiguration.IS_LOAD_BALANCED && !igNoreLoadBalancedCacheCheck)
                    CacheItemLogic.ApplyLoadBalancedCacheCheckItem(cacheKey, false);

                return true;
            }
            return false;
        }

        /// <summary>
        /// Flushes the index of cache objects.
        /// </summary>
        /// <param name="cacheKey">The cache key.</param>
        /// <param name="igNoreLoadBalancedCacheCheck">if set to <c>true</c> [ig nore load balanced cache check].</param>
        internal static void FlushIndexOfCache(string cacheKey, bool igNoreLoadBalancedCacheCheck = false)
        {
            bool hasFoundCachedItem = false;
            foreach (var key in _CacheLookup.Keys)
            {
                if (key.IndexOf(cacheKey) > -1)
                {
                    if (IsCached(key))
                    {
                        hasFoundCachedItem = true;
                        Memory.Remove(key);
                    }
                }
            }
            if (Data.CommonConfiguration.IS_LOAD_BALANCED && hasFoundCachedItem)
                CacheItemLogic.ApplyLoadBalancedCacheCheckItem(cacheKey, true);
        }

        /// <summary>
        /// Flushes all.
        /// </summary>
        /// <param name="Context">The context.</param>
        /// <param name="setEnvironment">if set to <c>true</c> [set environment].</param>
        public static void FlushAll(bool setEnvironment = false)
        {
            var keys = _CacheLookup.Keys;
            foreach (var key in keys)
            {
                if (IsCached(key))
                {
                    Memory.Remove(key);
                }
                DateTime output;
                _CacheLookup.TryRemove(key, out output);
            }
           
            Reset(true);
            _CacheLookup.Clear();
            Sushi.Mediakiwi.Portal.Caches.Clear();

            //if (setEnvironment)
            //    EnvironmentVersionLogic.Flush(true);
        }
    }
}