using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Controllers.Data
{
    public class GetFieldsResponse
    {
        public async Task ApplySharedFieldInformationAsync(int componentTemplateID)
        {
            try
            {
                // Get all shared Fields
                ICollection<Mediakiwi.Data.SharedField> _sharedFields = await Mediakiwi.Data.SharedField.FetchAllAsync();

                // Get all component template properties
                var _componentProperties = await Mediakiwi.Data.Property.SelectAllByTemplateAsync(componentTemplateID);

                // Do we have fields and template properties ?
                if (Fields?.Count > 0 && _sharedFields?.Count > 0)
                {
                    foreach (var field in Fields.Where(x => string.IsNullOrWhiteSpace(x.FieldName) == false))
                    {
                        // Do we have an item in the shared field collection with this fieldname ?
                        var sharedField = _sharedFields.FirstOrDefault(x => x.FieldName.Equals(field.FieldName, StringComparison.InvariantCultureIgnoreCase));

                        // Only continue when we do
                        if (sharedField?.ID > 0)
                        {
                            // Do we have a property in the Component Template collection with this fieldname, marked as SharedField and with the same contenttype ?
                            var propertyIsSharedField = _componentProperties.Any(x => x.FieldName.Equals(field.FieldName, StringComparison.InvariantCultureIgnoreCase) && x.IsSharedField && x.ContentTypeID == sharedField.ContentTypeID);
                            
                            field.IsSharedField = propertyIsSharedField;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Mediakiwi.Data.Notification.InsertOne("Monitor.ApplySharedFieldInformation", ex.Message);
            }
        }

        public List<DocumentTypeFieldListItem> Fields { get; set; }
    }
}
