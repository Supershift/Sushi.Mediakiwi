using System;
using System.Collections.Generic;

namespace Sushi.Mediakiwi.PageModules.ExportPage.JSON
{
    public class ExportedPage 
    {
        public ExportedPage()
        {
            this.Folders = new List<ExportedFolder>();
        }
               
        public Guid GUID { get; set; }

        #region Applicable Wim.Data.Page fields

        public int SiteID { get; set; }
        public bool AddToOutputCache { get; set; }
        public bool InheritPublicationInfo { get; set; }
        public string Name { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public string NewName { get; set; }
        public string LinkText { get; set; }
        public string Title { get; set; }
        public string Keywords { get; set; }
        public string Description { get; set; }
        public int FolderID { get; set; }
        public int SubFolderID { get; set; }
        public int TemplateID { get; set; }
        public int? MasterID { get; set; }
        public DateTime Created { get; set; }
        public DateTime Published { get; set; }
        public bool InheritContent { get; set; }
        public bool InheritContentEdited { get; set; }
        public bool IsSearchable { get; set; }
        public bool IsFixed { get; set; }
        public bool IsPublished { get; set; }
        public bool IsFolderDefault { get; set; }
        public bool IsSecure { get; set; }
        public DateTime Publication { get; set; }
        public DateTime Expiration { get; set; }
        public DateTime CustomDate { get; set; }
        public string InternalPath { get; set; }
        public  List<ExportedFolder> Folders { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public bool IsNewEntity { get; set; }

        #endregion
    }

}
