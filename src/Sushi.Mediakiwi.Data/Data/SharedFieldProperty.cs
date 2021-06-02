using Sushi.MicroORM;
using Sushi.MicroORM.Mapping;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Data
{
    [DataMap(typeof(SharedFieldPropertyMap))]
    public class SharedFieldProperty
    {
        public class SharedFieldPropertyMap : DataMap<SharedFieldProperty>
        {
            public SharedFieldPropertyMap()
            {
                Table("wim_SharedFieldProperties");
                Id(x => x.ID, "SharedFieldProperty_Key").Identity();
                Map(x => x.SharedFieldID, "SharedFieldProperty_SharedField_Key");
                Map(x => x.PropertyID, "SharedFieldProperty_Property_Key");
                Map(x => x.TemplateID, "SharedFieldProperty_Template_Key");
            }
        }

        public int ID { get; set; }
        public int SharedFieldID { get; set; }
        public int PropertyID { get; set; }
        public int TemplateID { get; set; }


        public static ICollection<SharedFieldProperty> FetchAll()
        {
            var connector = new Connector<SharedFieldProperty>();
            var filter = connector.CreateDataFilter();
            var result = connector.FetchAll(filter);
            return result;
        }

        public static async Task<ICollection<SharedFieldProperty>> FetchAllAsync()
        {
            var connector = new Connector<SharedFieldProperty>();
            var filter = connector.CreateDataFilter();
            var result = await connector.FetchAllAsync(filter).ConfigureAwait(false);
            return result;
        }

        public static ICollection<SharedFieldProperty> FetchAllForField(int sharedFieldId)
        {
            var connector = new Connector<SharedFieldProperty>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.SharedFieldID, sharedFieldId);
            var result = connector.FetchAll(filter);
            return result;
        }

        public static async Task<ICollection<SharedFieldProperty>> FetchAllForFieldAsync(int sharedFieldId)
        {
            var connector = new Connector<SharedFieldProperty>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.SharedFieldID, sharedFieldId);
            var result = await connector.FetchAllAsync(filter).ConfigureAwait(false);
            return result;
        }

        public static SharedFieldProperty FetchSingle(int id)
        {
            var connector = new Connector<SharedFieldProperty>();
            var result = connector.FetchSingle(id);
            return result;
        }


        public static async Task<SharedFieldProperty> FetchSingleAsync(int id)
        {
            var connector = new Connector<SharedFieldProperty>();
            var result = await connector.FetchSingleAsync(id).ConfigureAwait(false);
            return result;
        }

        public void Save()
        {
            var connector = new Connector<SharedFieldProperty>();
            connector.Save(this);
        }

        public async Task SaveAsync()
        {
            var connector = new Connector<SharedFieldProperty>();
            await connector.SaveAsync(this).ConfigureAwait(false);
        }

        public void Delete()
        {
            var connector = new Connector<SharedFieldProperty>();
            connector.Delete(this);
        }

        public async Task DeleteAsync()
        {
            var connector = new Connector<SharedFieldProperty>();
            await connector.DeleteAsync(this).ConfigureAwait(false);
        }
    }
}
