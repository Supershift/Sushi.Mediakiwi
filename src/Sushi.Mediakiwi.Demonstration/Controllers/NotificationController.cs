using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Demonstration.Controllers
{
    [ApiController]
    public class NotificationController : ControllerBase
    {
        [HttpGet]
        [Route("api/custom/notifications/generate")]
        public async Task<ActionResult> Generate()
        {
            var result = await Mediakiwi.Data.Notification.InsertOneAsync("Demo", Mediakiwi.Data.NotificationType.Error, "API generated demo notification");
            return Ok($"{result.GetType()} - {result.GetIdMessage()}");
        }
    }
}
