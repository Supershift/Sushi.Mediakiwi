using System;

namespace Sushi.Mediakiwi.PageModules.ExportPage.JSON
{

    public class ExportedComponentVersion
    {
        #region Applicable Wim.Data.ComponentVersion fields

        public int ApplicationUserID { get; set; }
        public int? MasterID { get; set; }
        public int? AvailableTemplateID { get; set; }
        public bool IsActive { get; set; }
        public int? SiteID { get; set; }
        public bool IsSecundary { get; set; }
        public string InstanceName { get; set; }
        public DateTime? SortField_Date { get; set; }

        public int PageID { get; set; }
        public Guid TemplateGUID { get; set; }
        public int SortOrder { get; set; }
        public bool IsFixed { get; set; }
        public bool IsAlive { get; set; }
        public string FixedFieldName { get; set; }
        public string Target { get; set; }
        //public string Serialized_XML { get; set; }
        public DateTime Created { get; set; }
        public ExportedContent Content { get; set; }

        #endregion
    }

}
