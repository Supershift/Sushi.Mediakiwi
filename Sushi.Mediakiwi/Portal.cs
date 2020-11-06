using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Sushi.Mediakiwi.Data;
using Sushi.Mediakiwi.UI;
using Microsoft.AspNetCore.Hosting;
using System.Collections.Concurrent;
using Sushi.Mediakiwi.Data.Caching;
using Sushi.Mediakiwi.Framework;
using Sushi.Mediakiwi.Data.Configuration;
using Sushi.MicroORM;
using System.Linq;

namespace Sushi.Mediakiwi
{
    public class Portal
    {
        private IHostingEnvironment _env;
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;

        public Portal(RequestDelegate next, IHostingEnvironment env, IConfiguration configuration)
        {
            _env = env;
            _next = next;
            _configuration = configuration;
        }

        internal static ConcurrentDictionary<string, ICacheManager> Caches;

        void Configure(HttpContext context)
        {
            if (Caches == null)
            {
                Caches = new ConcurrentDictionary<string, ICacheManager>();

                //set cache provider
                Configuration.CacheManagerProvider = () => {
                    //this serves a new instance of the cache for each unit test
                    //this ensures different unit tests running in the same application context don't share a cache
                    string key = "mediakiwi_cache";
                    var result = Caches.GetOrAdd(key, (string s) => { return new CacheManager(); });
                    return result;
                };

                // Assign json section to config
                WimServerConfiguration.LoadJsonConfig(_configuration);
                
                DatabaseConfiguration.SetDefaultConnectionString(Common.DatabaseConnectionString);
            }
        }

        public async Task Invoke(HttpContext context)
        {
            // Do something with context near the beginning of request processing.
            if (_env.IsDevelopment())
            {
                Configure(context);
                Monitor monitor = new Monitor(context, _env);
                await monitor.StartAsync();
            }
            else
            {
                try { 
                Configure(context);

                Monitor monitor = new Monitor(context, _env);
                await monitor.StartAsync();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            await _next.Invoke(context);
        }
    }
}
