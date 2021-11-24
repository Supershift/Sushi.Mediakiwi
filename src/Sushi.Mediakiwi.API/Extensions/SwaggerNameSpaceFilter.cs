using Swashbuckle.Swagger;
using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Http.Description;

namespace Sushi.Mediakiwi.API.Extensions
{
    internal class SwaggerNameSpaceFilter : IDocumentFilter
    {
        void IDocumentFilter.Apply(SwaggerDocument swaggerDoc, SchemaRegistry schemaRegistry, IApiExplorer apiExplorer)
        {
            foreach (ApiDescription apiDescription in apiExplorer.ApiDescriptions)
            {
                if ((apiDescription.RelativePathSansQueryString().StartsWith("api/System/"))
                    || (apiDescription.RelativePath.StartsWith("api/Internal/"))
                    || (apiDescription.Route.RouteTemplate.StartsWith("api/OtherStuff/"))
                    )
                {
                    swaggerDoc.paths.Remove("/" + apiDescription.Route.RouteTemplate.TrimEnd('/'));
                }
            }
        }
    }
}
