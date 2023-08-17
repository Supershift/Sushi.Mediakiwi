using Sushi.Mediakiwi.Data.MicroORM;
using Sushi.MicroORM;
using Sushi.MicroORM.Mapping;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Demonstration.Templates.Entities
{
    [DataMap(typeof(DemoObject1Map))]
    public class DemoObject1
    {
        public bool IsNewInstance { get { return ID == 0; } }

        public class DemoObject1Map : DataMap<DemoObject1>
        {
            public DemoObject1Map()
            {
                Table("cat_DemoObjects1");
                Id(x => x.ID, "DemoObject1_Key").Identity().SqlType(SqlDbType.Int);
                Map(x => x.Title, "DemoObject1_Title");
                Map(x => x.Created, "DemoObject1_Created");
                Map(x => x.Updated, "DemoObject1_Updated");
                Map(x => x.GroupName, "DemoObject1_GroupName");
                Map(x => x.IsActive, "DemoObject1_IsActive");
                Map(x => x.ImageID, "DemoObject1_Image_Key");
                Map(x => x.Text, "DemoObject1_Text");
            }
        }

        public int ID { get; set; }

        public string Title { get; set; }

        public DateTime Created { get; set; }

        public DateTime Updated { get; set; }

        public string GroupName { get; set; }

        public bool IsActive { get; set; }

        public int ImageID { get; set; }

        public string Text { get; set; } 

        public static async Task<List<DemoObject1>> FetchAll()
        {
            var connector = ConnectorFactory.CreateConnector<DemoObject1>();
            var filter = connector.CreateDataFilter();
            var result = await connector.FetchAllAsync(filter).ConfigureAwait(false);
            return result;
        }

        public static async Task<DemoObject1> FetchSingle(int id)
        {
            var connector = ConnectorFactory.CreateConnector<DemoObject1>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.ID, id);
            var result = await connector.FetchSingleAsync(filter).ConfigureAwait(false);
            return result;
        }

        public async Task SaveAsync()
        {
            var connector = ConnectorFactory.CreateConnector<DemoObject1>();
            await connector.SaveAsync(this).ConfigureAwait(false);
        }

        public async Task DeleteAsync()
        {
            var connector = ConnectorFactory.CreateConnector<DemoObject1>();
            await connector.DeleteAsync(this);
        }
    }
}