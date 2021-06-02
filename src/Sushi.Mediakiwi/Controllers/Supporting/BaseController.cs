using Microsoft.AspNetCore.Http;
using Sushi.Mediakiwi.Controllers.Data;
using Sushi.Mediakiwi.Data;
using Sushi.Mediakiwi.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace Sushi.Mediakiwi.Controllers
{
    internal class BaseController
    {
        internal JsonSerializerOptions Settings { get; }
         = new JsonSerializerOptions
         {
             IgnoreNullValues = true,
             PropertyNameCaseInsensitive = true,
             PropertyNamingPolicy = JsonNamingPolicy.CamelCase
         };

        protected internal string GetResponse(object response)
        {
            return JsonSerializer.Serialize(response, response.GetType(), Settings);
        }

        protected internal async Task<T> GetPostAsync<T>(HttpContext context)
        {
            var stream = context.Request.Body;
            using (StreamReader sr = new StreamReader(stream))
            {
                var output = await sr.ReadToEndAsync();
                return JsonSerializer.Deserialize<T>(output, Settings);
            }
        }
    }
}
