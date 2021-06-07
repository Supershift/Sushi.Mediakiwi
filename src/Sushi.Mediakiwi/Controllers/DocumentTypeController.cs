using Microsoft.AspNetCore.Http;
using Sushi.Mediakiwi.Controllers.Data;
using Sushi.Mediakiwi.Data;
using Sushi.Mediakiwi.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Controllers
{
    /// <summary>
    /// This Controller is initiated by adding a route in the following location:
    /// Sushi.Mediakiwi => Configure
    /// ControllerRegister.AddRoute("api/documentype/getfields", new DocumentTypeController());
    /// </summary>
    internal class DocumentTypeController : BaseController, IController
    {
     
        public async Task<string> CompleteAsync(HttpContext context)
        {
            var request = await GetPostAsync<GetFieldsRequest>(context).ConfigureAwait(false);

            Dictionary<string, int> req = new Dictionary<string, int>();
            foreach (var q in context.Request.Query)
            {
                if (int.TryParse(q.Value, out int result))
                {
                    req.Add(q.Key, result);
                }
            }

            GetFieldsResponse response = new GetFieldsResponse();
            response.Fields = new List<DocumentTypeFieldListItem>();

            var componentlist = await ComponentTemplate.SelectOneAsync(request.DocumentTypeID);
            var metadata = (MetaData[])Utility.GetDeserialized(typeof(MetaData[]), componentlist.MetaData);

            int index = 1;

            var properties = await Property.SelectAllByTemplateAsync(request.DocumentTypeID);

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
                            TypeID = Convert.ToInt32(meta.ContentTypeSelection),
                            MaxValueLength = Utility.ConvertToIntNullable(meta.MaxValueLength),
                            IsMandatory = meta.Mandatory.Equals("1"),
                            SortOrder = index,
                            TemplateID = request.DocumentTypeID,
                            IsSharedField = meta.IsSharedField
                        };
                        await property.SaveAsync();

                        // Set shared FIeld
                        await SaveSharedFieldAsync(property, meta.IsSharedField);

                        reload = true;
                    }

                    // if the sort order is not correct, correct it.
                    if (!property.SortOrder.Equals(index))
                    {
                        property.SortOrder = index;
                        await property.SaveAsync();
                        reload = true;
                    }
                    index++;
                }
            }

            // reload properties if it changed.
            if (reload)
            {
                properties = await Property.SelectAllByTemplateAsync(request.DocumentTypeID);
            }

            foreach (var property in properties)
            {
                response.Fields.Add(new DocumentTypeFieldListItem()
                {
                    ID = property.ID,
                    Title = property.Title,
                    IsMandatory = property.IsMandatory,
                    TypeID = property.TypeID,
                    SortOrder = property.SortOrder,
                    FieldName = property.FieldName,
                });
            }

            // [MR:29-04-2021] added for : https://supershift.atlassian.net/browse/FTD-147
            await response.ApplySharedFieldInformationAsync();

            return GetResponse(response);
        }

        private async Task SaveSharedFieldAsync(Property property, bool isShared)
        {
            // Check if there is an existing SharedField for this FieldName
            var existingSharedField = await SharedField.FetchSingleAsync(property.FieldName);

            // Field is marked As Shared field, but doesn't exist yet.
            // This means we need to add this property as a shared Field
            if (isShared && existingSharedField == null || existingSharedField?.ID == 0)
            {
                existingSharedField = new SharedField()
                {
                    ContentTypeID = property.TypeID,
                    FieldName = property.FieldName
                };

                // Save SharedField Entity
                await existingSharedField.SaveAsync();

                // Loop through existing properties that have the same fieldname and 
                // and add them to the SharedFIeldProperty collection
                foreach (var existingProp in await Property.SelectAllByFieldNameAsync(property.FieldName))
                {
                    // Create translations based off of the default value if we have any
                    if (string.IsNullOrWhiteSpace(property.DefaultValue) == false)
                    {
                        foreach (var site in await Site.SelectAllAsync())
                        {
                            SharedFieldTranslation translation = new SharedFieldTranslation()
                            {
                                ContentTypeID = property.TypeID,
                                EditValue = property.DefaultValue,
                                FieldID = existingSharedField.ID,
                                FieldName = property.FieldName,
                                SiteID = site.ID,
                                Value = property.DefaultValue
                            };

                            await translation.SaveAsync();
                        }
                    }
                }
            }

            // Field is NOT marked As Shared field, but is present as such
            // This means we need to delete everything connected to this shared FIeld
            if (isShared && existingSharedField?.ID > 0)
            {
                // Delete all translations
                var sharedFieldTranslations = await SharedFieldTranslation.FetchAllForFieldAsync(existingSharedField.ID);
                foreach (var sharedFieldTranslation in sharedFieldTranslations)
                {
                    await sharedFieldTranslation.DeleteAsync();
                }

                // Delete sharedfield
                await existingSharedField.DeleteAsync();
            }
        }

    }
}
