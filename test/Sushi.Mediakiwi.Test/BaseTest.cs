using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sushi.Mediakiwi.Data.Caching;
using Sushi.Mediakiwi.Data.Configuration;
using Sushi.MicroORM;
using System;
using System.Collections.Concurrent;
using System.Linq;

namespace Sushi.Mediakiwi.Test
{
    [TestClass]
    public abstract class BaseTest
    {
        private static ConcurrentDictionary<string, ICacheManager> Caches = new ConcurrentDictionary<string, ICacheManager>();
        public static Nest.IElasticClient ElasticClient { get; private set; }
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

            var config = new ConfigurationBuilder()
            .SetBasePath(context.TestDir)
            .AddJsonFile("appsettings.json", optional: true)
            .AddUserSecrets(System.Reflection.Assembly.GetExecutingAssembly(), optional: true)
            .Build();


            var elasticSettings = new Nest.ConnectionSettings(new Uri(config["ElasticUrl"]))
                .BasicAuthentication(config["ElasticUsername"], config["ElasticPassword"])                
                .ThrowExceptions(true);
            ElasticClient = new Nest.ElasticClient(elasticSettings);

            //// Assign json section to config
            //WimServerConfiguration.LoadJsonConfig(AppDomain.CurrentDomain.BaseDirectory + "appsettings.json");

            //// retrieve default portal and set connectionstring
            //WimServerPortal portal = WimServerConfiguration.Instance.Portals.Where(x => x.Name == WimServerConfiguration.Instance.DefaultPortal).FirstOrDefault();
            DatabaseConfiguration.SetDefaultConnectionString(config.GetConnectionString("datastore"));
        }

        public void WriteResult<T>(T result)
        {
            var type = typeof(T);
            if (result is string || type.IsPrimitive)
            {
                Console.WriteLine(result);
            }
            else
            {
                var line = Newtonsoft.Json.JsonConvert.SerializeObject(result, Newtonsoft.Json.Formatting.Indented);
                Console.WriteLine(line);
            }
        }
    }
}
