using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Controllers
{
    public class BaseController : ControllerBase, IController
    {
        public bool IsAuthenticationRequired { get; set; }

        internal JsonSerializerOptions Settings { get; }
         = new JsonSerializerOptions
         {
             IgnoreNullValues = true,
             PropertyNameCaseInsensitive = true,
             PropertyNamingPolicy = JsonNamingPolicy.CamelCase
         };

        internal protected string GetResponse(object response)
        {
            return JsonSerializer.Serialize(response, response.GetType(), Settings);
        }

        internal async protected Task<T> GetPostAsync<T>(HttpContext context)
        {
            var stream = context.Request.Body;
            using (StreamReader sr = new StreamReader(stream))
            {
                var output = await sr.ReadToEndAsync();
                return JsonSerializer.Deserialize<T>(output, Settings);
            }
        }

        public async virtual Task<string> CompleteAsync(HttpContext context)
        {
            return "";
        }
    }
}
