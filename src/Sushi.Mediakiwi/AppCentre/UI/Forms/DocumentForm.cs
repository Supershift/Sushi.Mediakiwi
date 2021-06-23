using Sushi.Mediakiwi.Data;
using Sushi.Mediakiwi.Framework;

namespace Sushi.Mediakiwi.AppCentre.UI.Forms
{
    public class DocumentForm : FormMap<Asset>
    {
        public DocumentForm(Asset asset)
        {
            Load(asset);

            if (asset.ID == 0)
            {
                Map(x => x.File, this).FileUpload("Upload", true, "image/*");

                Map(x => x.Title).TextField("Title", 255, false);
                Map(x => x.Description).TextArea("Description");
            }
            else
            {
                Map(x => x.Title).TextField("Title", 255, false);
                Map(x => x.Description).TextArea("Description");
                Map(x => x.File, this).FileUpload("Upload");
            }

            Map(x => x.GalleryID).FolderSelect("Folder", true, FolderType.Gallery);
        }

        public override void Evaluate()
        {
            if (this.File != null && this.File.File != null)
            {
                this.Instance.Type = this.File.File.ContentType;
                this.Instance.Extention = this.File.File.FileName.Substring(this.File.File.FileName.LastIndexOf("."));
                this.Instance.FileName = this.File.File.FileName;
                this.Instance.Size = this.File.File.Length;
                
                if (string.IsNullOrWhiteSpace(this.Instance.Title))
                    this.Instance.Title = this.Instance.FileName;
            }
        }

        public FileUpload File { get; set; }
        public string Section { get; set; }
    }
}
