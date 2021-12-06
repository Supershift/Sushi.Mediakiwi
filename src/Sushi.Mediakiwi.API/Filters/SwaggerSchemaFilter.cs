using Microsoft.AspNetCore.Mvc.ApplicationModels;
using System;

namespace Sushi.Mediakiwi.API.Filters
{
    public class SwaggerSchemaFilter : IActionModelConvention
    {
        public void Apply(ActionModel action)
        {
            if (action.Controller.ControllerType.FullName.Contains(Common.API_ASSEMBLY_NAME, StringComparison.InvariantCultureIgnoreCase) == false)
            {
                action.ApiExplorer.IsVisible = false;
            }
        }
    }
}
