using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Sushi.Mediakiwi.Controllers;
using Sushi.Mediakiwi.Data.Caching;
using Sushi.Mediakiwi.Data.Configuration;
using Sushi.Mediakiwi.Framework;
using Sushi.Mediakiwi.UI;
using Sushi.MicroORM;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi
{
    public class Portal
    {
        private readonly IHostingEnvironment _env;
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;
        private readonly IServiceProvider _serviceProvider;

        public Portal(RequestDelegate next, IHostingEnvironment env, IConfiguration configuration, IServiceProvider serviceProvider)
        {
            _env = env;
            _next = next;
            _configuration = configuration;
            _serviceProvider = serviceProvider;

            // Assign json section to config
            WimServerConfiguration.LoadJsonConfig(configuration);
            DatabaseConfiguration.SetDefaultConnectionString(Common.DatabaseConnectionString);
        }

        internal static ConcurrentDictionary<string, ICacheManager> Caches;

        static void Configure()
        {
            if (Caches == null)
            {
                Caches = new ConcurrentDictionary<string, ICacheManager>();

                //set cache provider
                Configuration.CacheManagerProvider = () => {
                    //this serves a new instance of the cache for each unit test
                    //this ensures different unit tests running in the same application context don't share a cache
                    string key = "mediakiwi_cache";
                    var result = Caches.GetOrAdd(key, (string s) => 
                    { 
                        return new CacheManager(); 
                    });

                    return result;
                };
            }
        }

        internal static decimal? Version;

        async static Task VerifyDatabaseAsync()
        {
            if (!Version.HasValue)
            {
                var comparelogic = new Framework.Functions.DataBaseCompareLogic();
                
                await comparelogic.Verify(WimServerConfiguration.Instance.Sql_Install_Enabled
                    , WimServerConfiguration.Instance.Sql_Install_Actions_Enabled).ConfigureAwait(false);
                
                await comparelogic.Start().ConfigureAwait(false);

                Version = CommonConfiguration.Version;
            }
        }

        static string GetSafeUrl(HttpContext context)
        {
            return $"{context.Request.Path}";
        }

        public async Task Invoke(HttpContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var url = GetSafeUrl(context);
            var portal = _configuration.GetValue<string>("mediakiwi:portal_path");

            Configure();

            if (await Monitor.StartControllerAsync(context, _env, _configuration, _serviceProvider).ConfigureAwait(false))
            {
                // Do nothing: this is the controller entree point
            }
            else if (url.Equals(portal, StringComparison.CurrentCultureIgnoreCase)  || url.StartsWith($"{portal}/", StringComparison.CurrentCultureIgnoreCase))
            {
                await VerifyDatabaseAsync().ConfigureAwait(false);
                var monitor = new Monitor(context, _env, _configuration);
                await monitor.StartAsync().ConfigureAwait(false);
            }
            else
            {
                var monitor = new Monitor(context, _env, _configuration);
                await monitor.AuthenticateViaSingleSignOnAsyc(false, true).ConfigureAwait(false);
            }
            await _next.Invoke(context).ConfigureAwait(false);
        }
    }
}
