using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Controllers
{
    public interface IController
    {
        Task<string> CompleteAsync(HttpContext context);
    }
}
