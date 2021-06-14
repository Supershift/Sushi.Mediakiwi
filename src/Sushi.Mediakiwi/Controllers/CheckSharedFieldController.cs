using Microsoft.AspNetCore.Http;
using Sushi.Mediakiwi.Controllers.Data;
using Sushi.Mediakiwi.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Controllers
{
    /// <summary>
    /// This Controller is initiated by adding a route in the following location:
    /// Sushi.Mediakiwi => Configure
    /// ControllerRegister.AddRoute("api/documentype/checksharedfield", new CheckSharedFieldController());
    /// </summary>
    internal class CheckSharedFieldController : BaseController
    {
        public override async Task<string> CompleteAsync(HttpContext context)
        {
            var request = await GetPostAsync<CheckSharedFieldRequest>(context).ConfigureAwait(false);

            CheckSharedFieldResponse response = new CheckSharedFieldResponse();
            var matchingProps = await Property.SelectAllByFieldNameAsync(request.FieldName);
            if (matchingProps?.Count > 0)
            {
                foreach (var prop in matchingProps.Where(x => x.TemplateID > 0))
                {
                    var cVersions = await ComponentVersion.SelectAllForTemplateAsync(prop.TemplateID);
                    var pages = await Page.SelectAllAsync(cVersions.Select(x => x.PageID.GetValueOrDefault(0)).ToArray());

                    foreach (var page in pages)
                    {
                        response.Pages.Add(new SharedFieldUsagePage()
                        {
                            PagePath = string.IsNullOrWhiteSpace(page.CompletePath) ? page.HRefFull : page.CompletePath,
                            PageTitle = string.IsNullOrWhiteSpace(page.Title) ? page.Name : page.Title,
                            PagePublished = page.IsPublished,
                            Components = cVersions.Where(x => x.PageID == page.ID).Select(x => x.Template.Name).ToList()
                        });
                    }
                }
            }

            return GetResponse(response);
        }
    }
}
