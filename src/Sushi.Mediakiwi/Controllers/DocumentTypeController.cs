using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sushi.Mediakiwi.Controllers.Data;
using Sushi.Mediakiwi.Data;
using Sushi.Mediakiwi.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Controllers
{
    [Authorize(AuthenticationSchemes = AuthenticationDefaults.AuthenticationScheme)]
    [Route("api/documentype")]
    public class DocumentTypeController : BaseController
    {
        public DocumentTypeController()
        {
            this.IsAuthenticationRequired = true;
        }

        [HttpPost("checkSharedField")]
        public async Task<string> CheckSharedFieldAsync()
        {
            var request = await GetPostAsync<CheckSharedFieldRequest>(HttpContext).ConfigureAwait(false);

            CheckSharedFieldResponse response = new CheckSharedFieldResponse();
            var matchingProps = await Property.SelectAllByFieldNameAsync(request.FieldName).ConfigureAwait(false);
            if (matchingProps?.Count > 0)
            {
                foreach (var prop in matchingProps.Where(x => x.TemplateID > 0))
                {
                    var cVersions = await ComponentVersion.SelectAllForTemplateAsync(prop.TemplateID).ConfigureAwait(false);
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

        [HttpPost("getFields")]
        public async Task<string> GetFieldsAsync()
        {
            var request = await GetPostAsync<GetFieldsRequest>(HttpContext).ConfigureAwait(false);

            Dictionary<string, int> req = new Dictionary<string, int>();
            foreach (var q in HttpContext.Request.Query)
            {
                if (int.TryParse(q.Value, out int result))
                {
                    req.Add(q.Key, result);
                }
            }

            GetFieldsResponse response = new GetFieldsResponse();
            response.Fields = new List<DocumentTypeFieldListItem>();

            var componentlist = await ComponentTemplate.SelectOneAsync(request.DocumentTypeID).ConfigureAwait(false);
            var metadata = (MetaData[])Utility.GetDeserialized(typeof(MetaData[]), componentlist.MetaData);

            int index = 1;

            var properties = await Property.SelectAllByTemplateAsync(request.DocumentTypeID).ConfigureAwait(false);

            bool reload = false;
            // go through all metadata properties and check if a related property is present
            if (metadata != null)
            {
                foreach (var meta in metadata)
                {
                    var property = properties.FirstOrDefault(x => x.FieldName.Equals(meta.Name));
                    // if not exits, create it
                    if (property == null)
                    {
                        property = new Property
                        {
                            FieldName = meta.Name,
                            Title = meta.Title,
                            AutoPostBack = meta.AutoPostBack.Equals("1"),
                            InteractiveHelp = meta.InteractiveHelp,
                            DefaultValue = meta.Default,
                            ContentTypeID = (ContentType)Enum.Parse(typeof(ContentType), meta.ContentTypeSelection),
                            MaxValueLength = Utility.ConvertToIntNullable(meta.MaxValueLength),
                            IsMandatory = meta.Mandatory.Equals("1"),
                            SortOrder = index,
                            TemplateID = request.DocumentTypeID,
                            IsSharedField = meta.IsSharedField.Equals("1")
                        };
                        await property.SaveAsync().ConfigureAwait(false);

                        // Create Shared Field if it doesn't exist yet.
                        await SharedField.CreateBasedOnPropertyAsync(property).ConfigureAwait(false);

                        reload = true;
                    }

                    // if the sort order is not correct, correct it.
                    if (!property.SortOrder.Equals(index))
                    {
                        property.SortOrder = index;
                        await property.SaveAsync().ConfigureAwait(false);
                        reload = true;
                    }
                    index++;
                }
            }

            // reload properties if it changed.
            if (reload)
            {
                properties = await Property.SelectAllByTemplateAsync(request.DocumentTypeID).ConfigureAwait(false);
            }

            foreach (var property in properties)
            {
                response.Fields.Add(new DocumentTypeFieldListItem()
                {
                    ID = property.ID,
                    Title = property.Title,
                    IsMandatory = property.IsMandatory,
                    ContentTypeID = (int)property.ContentTypeID,
                    SortOrder = property.SortOrder,
                    FieldName = property.FieldName,
                });
            }

            // [MR:29-04-2021] added for : https://supershift.atlassian.net/browse/FTD-147
            await response.ApplySharedFieldInformationAsync(request.DocumentTypeID).ConfigureAwait(false);

            return GetResponse(response);
        }
    }
}
