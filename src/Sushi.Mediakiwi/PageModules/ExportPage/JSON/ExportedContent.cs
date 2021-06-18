using System.Collections.Generic;

namespace Sushi.Mediakiwi.PageModules.ExportPage.JSON
{
    public class ExportedContent
    {
        public ExportedContent()
        {
            Fields = new List<ExportedField>();
        }

        public List<ExportedField> Fields { get; set; }
    }


}
