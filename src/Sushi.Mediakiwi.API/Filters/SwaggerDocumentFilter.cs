using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sushi.Mediakiwi.API.Filters
{
    public class SwaggerDocumentFilter : IDocumentFilter
    {
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            IDictionary<string, OpenApiPathItem> paths = new Dictionary<string, OpenApiPathItem>();
            OpenApiPaths correctedPaths = swaggerDoc.Paths;

            foreach (var item in correctedPaths.Where(x => x.Key.Contains("mkapi/", StringComparison.InvariantCulture) == false))
            {
                
                swaggerDoc.Paths.Remove(item.Key);
            }
        }
    }
}
