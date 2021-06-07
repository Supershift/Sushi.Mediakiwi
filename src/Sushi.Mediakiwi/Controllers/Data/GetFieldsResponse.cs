using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Controllers.Data
{
    public class GetFieldsResponse
    {
        public async Task ApplySharedFieldInformationAsync()
        {
            await ApplySharedFieldInformationAsync(null);
        }

        public async Task ApplySharedFieldInformationAsync(int? siteId)
        {
            try
            {
                ICollection<Mediakiwi.Data.SharedField> _sharedFields = await Mediakiwi.Data.SharedField.FetchAllAsync();

                if (Fields?.Count > 0 && _sharedFields?.Count > 0)
                {
                    foreach (var field in Fields.Where(x => string.IsNullOrWhiteSpace(x.FieldName) == false))
                    {
                        field.IsSharedField = _sharedFields.Any(x => x.FieldName.Equals(field.FieldName, StringComparison.InvariantCultureIgnoreCase));
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
