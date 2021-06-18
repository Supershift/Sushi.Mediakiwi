using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Controllers
{
    public interface IController
    {
        bool IsAuthenticationRequired { get; set; }
        Task<string> CompleteAsync(HttpContext context);
    }
}
