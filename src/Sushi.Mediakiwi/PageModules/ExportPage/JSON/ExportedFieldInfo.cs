using System;
using System.Collections.Generic;

namespace Sushi.Mediakiwi.PageModules.ExportPage.JSON
{
    public class ExportedFieldInfo
    {
        public string Message { get; set; }
        public string RemoteLocation { get; set; }
    }

    public class ExportedFieldInfoFolder : ExportedFieldInfo
    {
        public Guid? GUID { get; set; }
    }

    public class ExportedFieldInfoRichText : ExportedFieldInfo
    {
        public ExportedFieldInfoRichText()
        {
            Links = new List<ExportedFieldInfoLink>();
        }

        public string CleanedText { get; set; }

        public List<ExportedFieldInfoLink> Links { get; set; }
    }

    public class ExportedFieldInfoLink : ExportedFieldInfo
    {
        public int ID { get; set; }
        public bool IsInternal { get; set; }
        public string LinkText { get; set; }
        public int Target { get; set; }
        public int? PageID { get; set; }
        public int? AssetID { get; set; }
        public string ExternalUrl { get; set; }
        public string Alt { get; set; }
        public DateTime Created { get; set; }

        public ExportedFieldInfoAsset AssetInfo { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public int? ImportedLinkID { get; set; }
    }

    public class ExportedFieldInfoSublist : ExportedFieldInfo
    {
        public Guid GUID { get; set; }
        public string Assembly { get; set; }
        public string ClassName { get; set; }
    }

    public class ExportedFieldInfoAsset : ExportedFieldInfo
    {
        public ExportedFieldInfoAsset()
        {
//Data = new Wim.Data.CustomData();
        }

        [Newtonsoft.Json.JsonIgnore]
        public int GalleryID { get; set; }
        public Guid GalleryGUID { get; set; }
        public Guid GUID { get; set; }
        public string Title { get; set; }
        public string FileName { get; set; }
        public string Extention { get; set; }
        public long Size { get; set; }
        public string Type { get; set; }
        public int? AssetTypeID { get; set; }
        public int? ParentID { get; set; }
        public string Description { get; set; }
        public DateTime Created { get; set; }
        public bool IsOldStyle { get; set; }
        public bool IsNewStyle { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
       // public Wim.Data.CustomData Data { get; set; }
        public bool IsImage { get; set; }
        public bool IsActive { get; set; }
        public string RemoteLocation_Thumb { get; set; }
        public bool RemoteDownload { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public int? ImportedAssetID { get; set; }

    }
}
