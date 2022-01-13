using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Sushi.Mediakiwi.API.Filters
{
    public class SwaggerEnumFilter : ISchemaFilter
    {
        private readonly XDocument _xmlComments;

        public SwaggerEnumFilter(string xmlPath)
        {
            if (File.Exists(xmlPath))
            {
                _xmlComments = XDocument.Load(xmlPath);
            }
        }

        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (_xmlComments == null || context.Type.AssemblyQualifiedName.Contains(Common.API_ASSEMBLY_NAME, StringComparison.InvariantCultureIgnoreCase) == false)
            {
                return;
            }

            Dictionary<string, string> dict = new Dictionary<string, string>();

            if (schema.Enum != null && schema.Enum.Count > 0 && context.Type != null && context.Type.IsEnum)
            {
                var fullTypeName = context.Type.FullName;
                
                foreach (var enumMemberValue in schema.Enum.OfType<OpenApiInteger>().Select(v => v.Value))
                {
                    var enumMemberName = Enum.GetName(context.Type, enumMemberValue);

                    var fullEnumMemberName = $"F:{fullTypeName}.{enumMemberName}";

                    var enumMemberComments = _xmlComments.Descendants("member")
                        .FirstOrDefault(m => m.Attribute("name").Value.Equals
                        (fullEnumMemberName, StringComparison.OrdinalIgnoreCase));

                    if (enumMemberComments == null)
                    {
                        continue;
                    }

                    var summary = enumMemberComments.Descendants("summary").FirstOrDefault();

                    if (summary == null)
                    {
                        continue;
                    }
                    dict.Add($"{enumMemberValue} : {enumMemberName}", summary.Value.Trim());
                }
            }

            if (dict?.Count > 0)
            {
                schema.Description += "<p>Members:</p><ul>";
                foreach (var item in dict)
                {
                    schema.Description += $"<li><i>{item.Key}</i> - {item.Value}</li>";
                }
                schema.Description += "</ul>";
            }
        }
    }
}
