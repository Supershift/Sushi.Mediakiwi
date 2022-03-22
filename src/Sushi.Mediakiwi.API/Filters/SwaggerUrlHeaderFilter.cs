using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Linq;

namespace Sushi.Mediakiwi.API.Filters
{
    public class SwaggerUrlHeaderFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var allowAnonymous = context.MethodInfo.GetCustomAttributes(typeof(AllowAnonymousAttribute), true)?.Length > 0;

            if (!allowAnonymous)
            {
                if (operation.Parameters == null)
                {
                    operation.Parameters = new List<OpenApiParameter>();
                }
                // Skip this for the Asset Controller
                if (operation.Tags.Any(x => x.Name == "Asset"))
                {
                    return;
                }

                operation.Parameters.Add(new OpenApiParameter
                {
                    Name = Common.API_HEADER_URL,
                    In = ParameterLocation.Header,
                    Description = "Should contain the relative URL from the CMS",
                    Required = true,
                    Schema = new OpenApiSchema
                    {
                        Type = "string",
                        Default = new OpenApiString("/mediakiwi")
                    }
                });
            }
        }
    }
}
