using System;
using System.Collections.Generic;
using System.Text;

namespace Sushi.Mediakiwi.Data.Caching
{
    public static class Configuration
    {
        /// <summary>
        /// Gets or sets a function which returns an instance of <see cref="ICacheManager"/>. This is the default cache manager used by the <see cref="CachedConnector{T}"/>.
        /// </summary>
        public static Func<ICacheManager> CacheManagerProvider { get; set; }

        public static ICacheManager CacheManager
        {
            get
            {
                return CacheManagerProvider?.Invoke();
            }
        }
    }
}
