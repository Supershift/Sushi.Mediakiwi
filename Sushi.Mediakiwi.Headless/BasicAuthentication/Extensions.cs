﻿using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sushi.Mediakiwi.Headless.BasicAuthentication
{
    public static class LoginPromptExtension
    {
        public static IApplicationBuilder UseLoginPrompt(this IApplicationBuilder builder, Action<Credential> credentials)
        {
            var options = new Credential();
            credentials(options);
            return builder.UseMiddleware<LoginPrompt>(options);
        }
    }
}