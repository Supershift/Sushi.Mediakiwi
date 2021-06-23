using System;

namespace Sushi.Mediakiwi.PageModules.ExportPage.JSON
{
    public class ExportedComponentTemplate 
    {
        public Guid GUID { get; set; }
   
        [Newtonsoft.Json.JsonIgnore]
        public bool IsNewEntity { get; set; }
        
        [Newtonsoft.Json.JsonIgnore]
        public string NewBasedOn { get; set; }

        #region Applicable Wim.Data.ComponentTemplate fields

        public string Name { get; set; }
        public string Location { get; set; }
        public string TypeDefinition { get; set; }
        public string SourceTag { get; set; }
        public string Source { get; set; }
        public string ReferenceID { get; set; }
        public bool IsSearchable { get; set; }
        public int? SiteID { get; set; }
        public bool IsFixedOnPage { get; set; }
        public string Description { get; set; }
        public bool CanReplicate { get; set; }
        public int CacheLevel { get; set; }
        public string OutputCacheParams { get; set; }
        public bool CanDeactivate { get; set; }
        public int AjaxType { get; set; }
        public bool CanMoveUpDown { get; set; }
        public bool IsHeader { get; set; }
        public bool IsFooter { get; set; }
        public bool IsSecundaryContainerItem { get; set; }
        public bool IsShared { get; set; }
        public bool IsListTemplate { get; set; }
        public bool HasEditableSource { get; set; }
        public string MetaData { get; set; }
        public DateTime LastWriteTimeUtc { get; set; }

        #endregion

    }

}
