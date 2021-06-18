using System;

namespace Sushi.Mediakiwi.PageModules.ExportPage.JSON
{
    public class ExportedGallery
    {
        [Newtonsoft.Json.JsonIgnore]
        public bool IsNewEntity { get; set; }
        public string BackgroundRgb { get; set; }

        public Guid GUID { get; set; }
        public string Name { get; set; }
        public string CompletePath { get; set; }
        public int Type { get; set; }
        public DateTime Created { get; set; }
        public bool IsFixed { get; set; }
        public int FormatType { get; set; }
        public string Format { get; set; }
        public bool IsFolder { get; set; }
        public int? BaseID { get; set; }
        public Guid? ParentGUID { get; set; }
        public bool IsActive { get; set; }
        public bool IsHidden { get; set; }
    }
}
