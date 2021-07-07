using System;

namespace Sushi.Mediakiwi.PageModules.ExportPage.JSON
{
    public class ExportedFolder 
    {
        public int ID { get; set; }
        public Guid GUID { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public bool IsNewEntity { get; set; }

        public string Name { get; set; }
        public int Level { get; set; }
        public int? ParentID { get; set; }
        public string ParentName { get; set; }
        public string CompletePath { get; set; }
        public string Description { get; set; }
        public bool IsVisible { get; set; }
        public int? MasterID { get; set; }
        public int SiteID { get; set; }
        public int? SortOrderMethod { get; set; }
        public int Type { get; set; }
    }
}
