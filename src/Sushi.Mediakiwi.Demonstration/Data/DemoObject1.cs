using Sushi.MicroORM;
using Sushi.MicroORM.Mapping;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Demonstration.Data
{
    public class DemoObject1
    {
        public class DemoObject1Map : DataMap<DemoObject1>
        {
            public DemoObject1Map()
            {
                Table("cat_DemoObjects1");
                Id(x => x.ID, "DemoObject1_Key").Identity().ReadOnly();
                Map(x => x.Title, "DemoObject1_Title").Length(100);
                Map(x => x.Created, "DemoObject1_Created");
                Map(x => x.Updated, "DemoObject1_Updated");
            }
        }

        public int ID { get; set; }
        public string Title { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }

        public static async Task<List<DemoObject1>> FetchAllAsync()
        {
            var connector = new Connector<DemoObject1>();
            var filter = connector.CreateQuery();
    
            return await connector.FetchAllAsync(filter);
        }


        public static async Task<DemoObject1> FetchOneAsync(int key)
        {
            var connector = new Connector<DemoObject1>();

            return await connector.FetchSingleAsync(key);
        }

        public async Task SaveAsync()
        {
            var connector = new Connector<DemoObject1>();
            await connector.SaveAsync(this);
        }

        public async Task DeleteAsync()
        {
            var connector = new Connector<DemoObject1>();
            await connector.DeleteAsync(this);
        }
    }
}
