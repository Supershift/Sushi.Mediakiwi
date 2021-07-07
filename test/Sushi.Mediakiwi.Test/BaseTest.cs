using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sushi.Mediakiwi.Data.Caching;
using Sushi.Mediakiwi.Data.Configuration;
using Sushi.MicroORM;
using System;
using System.Collections.Concurrent;
using System.Linq;

namespace Sushi.Mediakiwi.Tests
{
    [TestClass]
    public abstract class BaseTest
    {
        private static ConcurrentDictionary<string, ICacheManager> Caches = new ConcurrentDictionary<string, ICacheManager>();
        [AssemblyInitialize]
        public static void Initialize(TestContext context)
        {
            //set cache provider
            Configuration.CacheManagerProvider = () =>
            {
                //this serves a new instance of the cache for each unit test
                //this ensures different unit tests running in the same application context don't share a cache
                string key = context.FullyQualifiedTestClassName + "_" + context.TestName;
                var result = Caches.GetOrAdd(key, (string s) => { return new TestCacheManager(); });
                return result;
            };

            // Assign json section to config
            WimServerConfiguration.LoadJsonConfig(AppDomain.CurrentDomain.BaseDirectory + "appsettings.json");

            // retrieve default portal and set connectionstring
            WimServerPortal portal = WimServerConfiguration.Instance.Portals.Where(x => x.Name == WimServerConfiguration.Instance.DefaultPortal).FirstOrDefault();
            DatabaseConfiguration.SetDefaultConnectionString(portal.Connection);
        }
    }
}
