using System.Collections.Generic;

namespace Sushi.Mediakiwi.PageModules.ExportPage.JSON
{

    public class ExportedField
    {
        public ExportedField()
        {

        }

        public int Type { get; set; }
        public string Property { get; set; }
        public string Value { get; set; }
        public string Label { get; set; }
        public bool IsShared { get; set; }

        public ExportedFieldInfoLink LinkInfo { get; set; }
        public ExportedFieldInfoAsset AssetInfo { get; set; }
        public ExportedFieldInfoSublist SubListInfo { get; set; }
        public ExportedFieldInfoRichText RichTextInfo { get; set; }
        public ExportedFieldInfo GenericInfo { get; set; }
        public ExportedFieldInfoFolder FolderInfo { get; set; }

        public List<ExportedField> MultiFields { get; set; }
    }

}
