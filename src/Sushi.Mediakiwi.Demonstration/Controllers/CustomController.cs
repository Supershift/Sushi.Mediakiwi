using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Demonstration.Controllers
{
    [ApiController]
    public class CustomController : ControllerBase
    {
        [HttpGet]
        [Route("api/custom/ping")]        
        public ActionResult Ping()
        {
            return Ok("hello world");
        }

        [HttpGet]
        [Route("api/custom/resource/{id}")]
        public ActionResult GetResource(string id)
        {
            return Ok($"Resource id: {id}");
        }
    }
}
