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

            var componentlist = ComponentTemplate.SelectOne(request.DocumentTypeID);
            var metadata = (MetaData[])Utility.GetDeserialized(typeof(MetaData[]), componentlist.MetaData);

            int index = 1;

            var properties = Property.SelectAllByTemplate(request.DocumentTypeID);

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
                            TemplateID = req["item"]
                        };
                        property.Save();
                        reload = true;
                    }

                    // if the sort order is not correct, correct it.
                    if (!property.SortOrder.Equals(index))
                    {
                        property.SortOrder = index;
                        property.Save();
                        reload = true;
                    }
                    index++;
                }
            }
            // reload properties if it changed.
            if (reload)
            {
                properties = Property.SelectAllByTemplate(req["item"]);
            }

            foreach (var property in properties)
            {
                response.Fields.Add(new DocumentTypeFieldListItem()
                {
                    ID = property.ID,
                    Title = property.Title,
                    IsMandatory = property.IsMandatory,
                    TypeID = property.TypeID,
                    SortOrder = property.SortOrder
                });
            }

            return GetResponse(response);
        }
    }
}
