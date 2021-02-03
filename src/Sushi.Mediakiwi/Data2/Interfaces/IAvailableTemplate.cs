using System;

namespace Sushi.Mediakiwi.Data
{
    public interface IAvailableTemplate
    {
        string ComponentTemplate { get; set; }
        int ComponentTemplateID { get; set; }
        string FixedFieldName { get; set; }
        Guid GUID { get; set; }
        int ID { get; set; }
        bool IsPossible { get; set; }
        bool IsPresent { get; set; }
        bool IsSecundary { get; set; }
        int PageTemplateID { get; set; }
        int SlotID { get; set; }
        int SortOrder { get; set; }
        string Target { get; set; }
        DateTime? Updated { get; }
        bool IsNewInstance { get; }
        void Save();
        bool Delete();
    }
}