using Sushi.MicroORM;
using Sushi.MicroORM.Mapping;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Data.Data
{
    public class DocumentType
    {
        public DocumentType()
        {

        }
    }
}

namespace Sushi.Mediakiwi.Data
{
    public class DocumentType
    {
        public class DocumentTypeMap : DataMap<DocumentType>
        {
            public DocumentTypeMap()
            {
                Table("wim_DocumentTypes");
                Id(x => x.ID, "DocumentType_Key").ReadOnly();
                Map(x => x.Guid, "DocumentType_Guid");
                Map(x => x.Name, "DocumentType_Name");
                Map(x => x.Type, "DocumentType_Type");
                Map(x => x.ComponentListId, "DocumentType_ComponentList_Key");
                Map(x => x.Created, "DocumentType_Created");

            }
        }

        public int ID { get; set; }
        public Guid Guid { get; set; }
        public int? ComponentListId { get; set; }
        public int Type { get; set; }
        public string Name { get; set; }
        public DateTime? Created { get; set; }

        public static async Task<List<DocumentType>> FetchAllAsync()
        {
            var connector = new Connector<DocumentType>();
            var filter = connector.CreateDataFilter();
            var result = await connector.FetchAllAsync(filter);
            return result;
        }

        public static List<DocumentType> FetchAll()
        {
            var connector = new Connector<DocumentType>();
            var filter = connector.CreateDataFilter();
            var result = connector.FetchAll(filter);
            return result;
        }

        public static async Task<DocumentType> FetchSingleAsync(int id)
        {
            var connector = new Connector<DocumentType>();
            var result = await connector.FetchSingleAsync(id);
            return result;
        }

        public static DocumentType FetchSingle(int id)
        {
            var connector = new Connector<DocumentType>();
            var result = connector.FetchSingle(id);
            return result;
        }

        public async Task SaveAsync()
        {
            var connector = new Connector<DocumentType>();
            await connector.SaveAsync(this);
        }

        public void Save()
        {
            var connector = new Connector<DocumentType>();
            connector.Save(this);
        }

        public void Delete()
        {
            var connector = new Connector<DocumentType>();
            connector.Delete(this);
        }
    }
}
