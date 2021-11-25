using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;

namespace Sushi.Mediakiwi.API.Filters
{
    public class SwaggerSchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (schema?.Properties?.Count > 0 // Do we have any properties
                && context.Type.FullName.Contains(Common.API_ASSEMBLY_NAME, StringComparison.InvariantCultureIgnoreCase) == false // not in the Sushi.Mediakiwi.API namespace
                && context.Type.FullName.Contains("Sushi.Mediakiwi", StringComparison.InvariantCultureIgnoreCase)) // in the Sushi.Mediakiwi* namespace
            {
              //  schema.Properties.Clear();
              //  context.SchemaRepository.Schemas.Clear();
            }
        }
    }
}
