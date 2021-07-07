using Newtonsoft.Json;
using Sushi.Mediakiwi.Data;
using System;
using System.Collections.Generic;

namespace Sushi.Mediakiwi.PageModules.ExportPage.JSON
{
    public class ExportedPageTemplate
    {
        #region CTor

        public ExportedPageTemplate()
        {
            Data = new CustomData();
            AvailableComponentTemplates = new List<ExportedAvailableComponent>();
        }

        #endregion

        public Guid GUID { get; set; }

        [JsonIgnore]
        public bool IsNewEntity { get; set; }

        #region Applicable Wim.Data.PageTemplate fields

        public int? SiteID { get; set; }
        public int? OverwriteSiteKey { get; set; }
        public int? OverwriteTemplateKey { get; set; }
        public bool IsSourceBased { get; set; }
        public string ReferenceID { get; set; }

        [JsonIgnore]
        public CustomData Data { get; set; }
        public string Description { get; set; }
        public string Source { get; set; }
        public string Name { get; set; }
        public bool OnlyOneInstancePossible { get; set; }
        public bool HasCustomDate { get; set; }
        public bool IsAddedOutputCache { get; set; }
        public int? OutputCacheDuration { get; set; }
        public bool HasSecundaryContentContainer { get; set; }
        public string Location { get; set; }
        public DateTime LastWriteTimeUtc { get; set; }

        public List<ExportedAvailableComponent> AvailableComponentTemplates { get; set; }

        #endregion
    }
}
