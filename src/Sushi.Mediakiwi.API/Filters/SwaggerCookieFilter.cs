using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;

namespace Sushi.Mediakiwi.API.Filters
{
    public class SwaggerCookieFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var filterPipeline = context.ApiDescription.ActionDescriptor.FilterDescriptors;
            var allowAnonymous = context.MethodInfo.GetCustomAttributes(typeof(AllowAnonymousAttribute), true)?.Length > 0;

            if (!allowAnonymous)
            {
                if (operation.Parameters == null)
                {
                    operation.Parameters = new List<OpenApiParameter>();
                }

                operation.Parameters.Add(new OpenApiParameter
                {
                    Name = Common.API_COOKIE_KEY,
                    In = ParameterLocation.Cookie,
                    Description = "Contains the cookie containing the user information as a JWT token",
                    Required = true,
                });
            }
        }
    }
}
