using System;

namespace Sushi.Mediakiwi.Data
{
    public interface IPageVersion
    {
        string CompletePath { get; set; }
        string ContentXML { get; set; }
        DateTime Created { get; set; }
        string Hash { get; set; }
        int ID { get; set; }
        bool IsArchived { get; set; }
        bool IsNewInstance { get; }
        string MetaDataXML { get; set; }
        string Name { get; set; }
        int PageID { get; set; }
        int TemplateID { get; set; }
        int UserID { get; set; }
        string RollBackTo { get; set; }

        bool Save();

       
    }
}