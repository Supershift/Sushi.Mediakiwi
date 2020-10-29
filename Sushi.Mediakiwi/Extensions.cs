using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;


namespace Sushi.Mediakiwi
{
    public static class Extensions
    {
        public static IApplicationBuilder UseMediakiwi(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<Portal>();
        }
    }
}
