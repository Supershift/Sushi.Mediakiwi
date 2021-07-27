using Sushi.Mediakiwi.Data;
using Sushi.Mediakiwi.Framework;

namespace Sushi.Mediakiwi.AppCentre.UI.Forms
{
    public class DocumentForm : FormMap<Asset>
    {
        public DocumentForm(Asset asset, bool isImage)
        {
            Load(asset);

            if (asset.ID == 0)
            {
                if (isImage)
                {
                    Map(x => x.File, this).FileUpload("Upload", true, "image/*");
                }
                else
                {
                    Map(x => x.File, this).FileUpload("Upload", true);
                }

                Map(x => x.Title).TextField("Title", 255);
                Map(x => x.Description).TextArea("Description");
            }
            else
            {
                Map(x => x.Title).TextField("Title", 255);
                Map(x => x.Description).TextArea("Description");
                Map(x => x.File, this).FileUpload("Upload");
            }
            
            Map(x => x.GalleryID).FolderSelect("Folder", true, FolderType.Gallery);
        }

        public override void Evaluate()
        {
            if (File != null && File.File != null)
            {
                Instance.Type = File.File.ContentType;
                Instance.Extention = File.File.FileName.Substring(File.File.FileName.LastIndexOf('.'));
                Instance.FileName = File.File.FileName;
                Instance.Size = File.File.Length;

                if (string.IsNullOrWhiteSpace(Instance.Title))
                {
                    Instance.Title = Instance.FileName;
                }
            }
        }

        public FileUpload File { get; set; }
        public string Section { get; set; }
    }
}
