using System.Collections.Generic;

namespace Sushi.Mediakiwi.PageModules.ExportPage.JSON
{
    [Newtonsoft.Json.JsonObject]
    public class ExportedPageComplete
    {
        [Newtonsoft.Json.JsonProperty]
        public string ExportedBy { get; set; }

        [Newtonsoft.Json.JsonProperty]
        public long ExportedOn { get; set; }

        [Newtonsoft.Json.JsonConstructor()]
        public ExportedPageComplete()
        {
            Page = new ExportedPage();
            PageTemplate = new ExportedPageTemplate();
            ComponentTemplates = new List<ExportedComponentTemplate>();
            Components = new List<ExportedComponent>();
            ComponentVersions = new List<ExportedComponentVersion>();
            Galleries = new List<ExportedGallery>();
        }

        [Newtonsoft.Json.JsonProperty]
        public ExportedPage Page { get; set; }

        [Newtonsoft.Json.JsonProperty]
        public ExportedPageTemplate PageTemplate { get; set; }

        [Newtonsoft.Json.JsonProperty]
        public List<ExportedComponentTemplate> ComponentTemplates { get; set; }

        [Newtonsoft.Json.JsonProperty]
        public List<ExportedComponent> Components { get; set; }

        [Newtonsoft.Json.JsonProperty]
        public List<ExportedComponentVersion> ComponentVersions { get; set; }

        [Newtonsoft.Json.JsonProperty]
        public List<ExportedGallery> Galleries { get; set; }

        [Newtonsoft.Json.JsonProperty]
        public List<ExportedSharedField> SharedFields { get; set; }
    }
}
