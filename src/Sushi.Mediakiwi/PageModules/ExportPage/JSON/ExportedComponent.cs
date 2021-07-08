using System;

namespace Sushi.Mediakiwi.PageModules.ExportPage.JSON
{

    public class ExportedComponent
    {
        #region Applicable Wim.Data.Component fields
        public Guid TemplateGUID { get; set; }
        public int SortOrder { get; set; }
        public bool IsFixed { get; set; }
        public bool IsAlive { get; set; }
        public string FixedFieldName { get; set; }
        public string Target { get; set; }
        //public string Serialized_XML { get; set; }
        public DateTime Created { get; set; }
        public DateTime SortField_Date { get; set; }

        public ExportedContent Content { get; set; }

        #endregion
    }

}
