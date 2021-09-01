using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Sushi.Mediakiwi.Website.RazorAdditions
{
    public class DefaultHttpClientHandler : HttpClientHandler
    {
        public DefaultHttpClientHandler(IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };

            AllowAutoRedirect = false;
            AutomaticDecompression = System.Net.DecompressionMethods.Deflate | System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Brotli;
        }
    }

    public static class WebserviceConfiguration
    {
        public static void AddClients(this IServiceCollection services)
        {
            services.AddTransient<DefaultHttpClientHandler>();            
        }
    }
}
