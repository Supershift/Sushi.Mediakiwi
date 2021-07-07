using System;

namespace Sushi.Mediakiwi.PageModules.ExportPage.JSON
{
    public class ExportedAvailableComponent
    {
        public Guid GUID { get; set; }
        public Guid PageTemplateGUID { get; set; }
        public Guid ComponentTemplateGUID { get; set; }
        public string Target { get; set; }
        public bool IsPossible { get; set; }
        public bool IsSecundary { get; set; }
        public bool IsPresent { get; set; }
        public int SortOrder { get; set; }
        public string FixedFieldName { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public bool IsNewEntity { get; set; }
    }
}
