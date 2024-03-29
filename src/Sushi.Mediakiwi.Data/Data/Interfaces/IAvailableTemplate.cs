﻿using System;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Data
{
    public interface IAvailableTemplate
    {   
        string ComponentTemplate { get; }
        int ComponentTemplateID { get; set; }
        string FixedFieldName { get; set; }
        Guid GUID { get; set; }
        int ID { get; set; }
        bool IsPossible { get; set; }
        bool IsPresent { get; set; }
        bool IsSecundary { get; set; }
        int PageTemplateID { get; set; }
        int SortOrder { get; set; }
        string Target { get; set; }
        DateTime? Updated { get; }
        bool IsNewInstance { get; }
        int? SlotID { get; set; }
        void Save();
        Task SaveAsync();
        void Delete();
        Task DeleteAsync();
    }
}