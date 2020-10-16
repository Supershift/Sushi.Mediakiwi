using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Sushi.Mediakiwi.Data;

namespace Sushi.Mediakiwi
{
    public class Portal
    {
        private readonly RequestDelegate _next;

        public Portal(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            // Do something with context near the beginning of request processing.
            await context.Response.WriteAsync("Hello Mediakiwi2:");

            await _next.Invoke(context);
        }
    }
}
