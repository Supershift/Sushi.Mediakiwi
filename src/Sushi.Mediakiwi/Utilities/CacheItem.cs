using System;
using System.Collections;
using System.Collections.Specialized;
using System.Web;
using System.Linq;

using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using System.Web.Caching;
using System.Diagnostics;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using Sushi.Mediakiwi.Data;
using System.Runtime.Caching;
using System.Collections.Concurrent;

namespace Wim.Utilities
{
    /// <summary>
    /// This class manages objects in server cache. 
    /// </summary>
    public class CacheItemManager : IDisposable
    {
        #region Dispose
        //private bool disposed = false;
        
        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            //Dispose(true);
            //GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            //if(!this.disposed)
            //{
            //    if(disposing)
            //    {
            //        //  Components.Dispose();
            //    }
            //}
            //disposed = true;         
        }

        /// <summary>
        /// 
        /// </summary>
        ~CacheItemManager()      
        {
            //Dispose(false);
        }
        #endregion Dispose

        #region Members
        //static readonly HttpContext Context = HttpContext.Current;
        //static ObjectCache Memory = MemoryCache.Default;
        //static ConcurrentDictionary<string, DateTime> _CacheLookup = new ConcurrentDictionary<string, DateTime>();
        #endregion Members

        /// <summary>
        /// Determines whether the specified key is cached.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>
        /// 	<c>true</c> if the specified key is cached; otherwise, <c>false</c>.
        /// </returns>
        public bool IsCached(string key)
        {
            return Sushi.Mediakiwi.Framework.Caching.IsCached(key);
        }

        /// <summary>
        /// Determines whether [is cached object] [the specified key].
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>
        ///   <c>true</c> if [is cached object] [the specified key]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsCachedObject(string key)
        {
            return Sushi.Mediakiwi.Framework.Caching.IsCached(key);
        }

        /// <summary>
        /// Determines whether [is cached object] [the specified key].
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="item">The item.</param>
        /// <returns>
        ///   <c>true</c> if [is cached object] [the specified key]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsCachedObject(string key, out object item)
        {
            return Sushi.Mediakiwi.Framework.Caching.IsCached(key, out item);
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
            return Sushi.Mediakiwi.Framework.Caching.IsCachedIndex(cacheKey);
        }

        /// <summary>
        /// Gets the item.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public object GetItem(string key)
        {
            object candidate;
            if ( !IsCachedObject(key, out candidate) )
                return null;
            return candidate;
        }

        /// <summary>
        /// Adds the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="item">The item.</param>
        /// <param name="expiration">The expiration.</param>
        public void Add( string key, object item, DateTime expiration )
        {
            Sushi.Mediakiwi.Framework.Caching.Add(key, item, expiration);
        }

        /// <summary>
        /// Add an object with sliding expiration
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="item">The item.</param>
        /// <param name="expiration">The expiration.</param>
        /// <param name="metaInformation">The meta information.</param>
        internal void AddSliding(string key, object item, TimeSpan expiration, string metaInformation)
        {
            Sushi.Mediakiwi.Framework.Caching.Add(key, item, expiration);
        }

        /// <summary>
        /// Adds the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="item">The item.</param>
        /// <param name="expiration">The expiration.</param>
        /// <param name="metaInformation">The meta information.</param>
        public void Add(string key, object item, DateTime expiration, string metaInformation)
        {
            Sushi.Mediakiwi.Framework.Caching.Add(key, item, expiration);
        }

        /// <summary>
        /// Adds the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="item">The item.</param>
        /// <param name="slidingExpiration">The sliding expiration.</param>
        /// <param name="metaInformation">The meta information.</param>
        public void Add(string key, object item, TimeSpan slidingExpiration, string metaInformation)
        {
            Sushi.Mediakiwi.Framework.Caching.Add(key, item, slidingExpiration);
        }

        /// <summary>
        /// Adds the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="item">The item.</param>
        /// <param name="slidingExpiration">The sliding expiration.</param>
        /// <param name="metaInformation">The meta information.</param>
        /// <param name="filename">The filename (full local path).</param>
        public void Add(string key, object item, TimeSpan slidingExpiration, string metaInformation, string filename)
        {
            Sushi.Mediakiwi.Framework.Caching.Add(key, item, slidingExpiration);
        }

        //  STATIC METHODS

        #region Flush cache object
        /// <summary>
        /// Flushes the cache object.
        /// </summary>
        /// <param name="cacheKey">The cache key.</param>
        /// <returns></returns>
        public static bool FlushCacheObject(string cacheKey)
        {
            return Sushi.Mediakiwi.Framework.Caching.FlushCache(cacheKey);
        }

        /// <summary>
        /// Flushes the cache object.
        /// </summary>
        /// <param name="cacheKey">The cache key.</param>
        /// <param name="igNoreLoadBalancedCacheCheck">if set to <c>true</c> [ig nore load balanced cache check].</param>
        /// <returns></returns>
        public static bool FlushCacheObject(string cacheKey, bool igNoreLoadBalancedCacheCheck)
        {
            return Sushi.Mediakiwi.Framework.Caching.FlushCache(cacheKey, igNoreLoadBalancedCacheCheck);
        }
        #endregion Flush cache object

        #region Flush index of cache objects
        /// <summary>
        /// Flushes the index of cache objects.
        /// </summary>
        /// <param name="cacheKey">The cache key.</param>
        public static void FlushIndexOfCacheObjects(string cacheKey)
        {
            Sushi.Mediakiwi.Framework.Caching.FlushIndexOfCache(cacheKey);
        }

        /// <summary>
        /// Flushes the index of cache objects.
        /// </summary>
        /// <param name="cacheKey">The cache key.</param>
        /// <param name="igNoreLoadBalancedCacheCheck">if set to <c>true</c> [ig nore load balanced cache check].</param>
        internal static void FlushIndexOfCacheObjects(string cacheKey, bool igNoreLoadBalancedCacheCheck)
        {
            Sushi.Mediakiwi.Framework.Caching.FlushIndexOfCache(cacheKey, igNoreLoadBalancedCacheCheck);
        }

        /// <summary>
        /// Flush cache object with cacheKey as IndexOf
        /// </summary>
        /// <param name="cacheKey">Part of name of cache objects</param>
        /// <param name="context">The context.</param>
        /// <param name="igNoreLoadBalancedCacheCheck">if set to <c>true</c> [ig nore load balanced cache check].</param>
        internal static void FlushIndexOfCacheObjects(string cacheKey, HttpContext context, bool igNoreLoadBalancedCacheCheck)
        {
            Sushi.Mediakiwi.Framework.Caching.FlushIndexOfCacheObjects(cacheKey, context, igNoreLoadBalancedCacheCheck);
        }

        /// <summary>
        /// Flushes all.
        /// </summary>
        /// <param name="Context">The context.</param>
        /// <param name="setEnvironment">if set to <c>true</c> [set environment].</param>
        public static void FlushAll(bool setEnvironment = false)
        {
            Sushi.Mediakiwi.Framework.Caching.FlushAll(setEnvironment);
        }
        #endregion Flush index of cache objects
    }
}
