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
        private IHostingEnvironment _env;
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;
        private readonly IServiceProvider _serviceProvider;

        public Portal(RequestDelegate next, IHostingEnvironment env, IConfiguration configuration, IServiceProvider serviceProvider)
        {
            _env = env;
            _next = next;
            _configuration = configuration;
            _serviceProvider = serviceProvider;
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

        string GetSafeUrl(HttpContext context)
        {
            return $"{context.Request.Path}";

            return $"{context.Request.PathBase}{context.Request.Path}";
        }

        public async Task Invoke(HttpContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var url = GetSafeUrl(context);
            var portal = _configuration.GetValue<string>("mediakiwi:portal_path");

            Configure(context);

            if (!await Monitor.StartControllerAsync(context, _env, _configuration, _serviceProvider).ConfigureAwait(false))
            {

                if (
                    url.Equals(portal, StringComparison.CurrentCultureIgnoreCase)
                    || url.StartsWith($"{portal}/", StringComparison.CurrentCultureIgnoreCase)
                    )
                {
                    if (_env.IsDevelopment())
                    {
                        Monitor monitor = new Monitor(context, _env, _configuration);
                        await monitor.StartAsync().ConfigureAwait(false);
                    }
                    else
                    {
                        try
                        {
                            Monitor monitor = new Monitor(context, _env, _configuration);
                            await monitor.StartAsync().ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    }
                }
            }
            await _next.Invoke(context).ConfigureAwait(false);
        }
    }

    public class Portal2
    {
        private IHostingEnvironment _env;
        private readonly IConfiguration _configuration;

        public Portal2(IHostingEnvironment env, IConfiguration configuration)
        {
            _env = env;
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

        public async Task<string> Invoke(HttpContext context)
        {
            // Do something with context near the beginning of request processing.
            if (_env.IsDevelopment())
            {
                Configure(context);
                Monitor monitor = new Monitor(context, _env, _configuration);
                await monitor.StartAsync();
                return monitor.Body;
            }
            else
            {
                try
                {
                    Configure(context);

                    Monitor monitor = new Monitor(context, _env, _configuration);
                    await monitor.StartAsync();
                    return monitor.Body;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
    }
}
