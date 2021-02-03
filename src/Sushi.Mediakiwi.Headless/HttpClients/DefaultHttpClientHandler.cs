using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System.Net.Http;

namespace Sushi.Mediakiwi.Headless.HttpClients
{
    public class DefaultHttpClientHandler : HttpClientHandler
    {
        public DefaultHttpClientHandler(IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
            }

            AllowAutoRedirect = false;
            AutomaticDecompression = System.Net.DecompressionMethods.Deflate | System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Brotli;
        }
    }
}
