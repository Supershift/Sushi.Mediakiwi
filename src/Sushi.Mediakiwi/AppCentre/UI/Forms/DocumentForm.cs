using Sushi.Mediakiwi.Data;
using Sushi.Mediakiwi.Framework;
using Sushi.Mediakiwi.UI;
using System.Globalization;

namespace Sushi.Mediakiwi.AppCentre.UI.Forms
{
    public class DocumentForm : FormMap<Asset>
    {
        public ListItemCollection CreateOrSelectOptions
        {
            get
            {
                ListItemCollection lst = new ListItemCollection();
                lst.Add(new ListItem(Labels.ResourceManager.GetString("yes", new CultureInfo(wim.CurrentApplicationUser.LanguageCulture)), "1"));
                lst.Add(new ListItem(Labels.ResourceManager.GetString("no", new CultureInfo(wim.CurrentApplicationUser.LanguageCulture)), "2"));

                return lst;
            }
        }

        public DocumentForm(WimComponentListRoot wim, Asset asset)
        {
            wim.OpenInEditMode = true;
            if (wim?.Console?.Request?.Query?.ContainsKey("isimage") == true)
            {
                IsImage = wim.Console.Request.Query["isimage"].Equals("1");
            }

            if (wim?.Console?.Request?.Query?.ContainsKey("onlycreate") == true)
            {
                OnlyCreate = wim.Console.Request.Query["onlycreate"].Equals("1");
            }

            Load(asset);
            
            if (Instance.ID == 0)
            {
                // only show Select Existing when user can see galleries
                Map(x => x.CreateOrSelect, this).Radio(Labels.ResourceManager.GetString("create_or_select", new CultureInfo(wim.CurrentApplicationUser.LanguageCulture)), nameof(CreateOrSelectOptions), "uxselectOrCreate", true, true);

                if (IsImage)
                {
                    Map(x => x.File, this).FileUpload("Upload", true, "image/*");
                }
                else
                {
                    Map(x => x.File, this).FileUpload("Upload", true);
                }

                Map(x => x.Title).TextField(Labels.ResourceManager.GetString("_title", new CultureInfo(wim.CurrentApplicationUser.LanguageCulture)), 255);
                Map(x => x.Description).TextArea(Labels.ResourceManager.GetString("_description", new CultureInfo(wim.CurrentApplicationUser.LanguageCulture)));
            }
            else
            {
                Map(x => x.Title).TextField(Labels.ResourceManager.GetString("_title", new CultureInfo(wim.CurrentApplicationUser.LanguageCulture)), 255);
                Map(x => x.Description).TextArea(Labels.ResourceManager.GetString("_description", new CultureInfo(wim.CurrentApplicationUser.LanguageCulture)));
                Map(x => x.File, this).FileUpload("Upload");
            }

            Map(x => x.GalleryID).FolderSelect(Labels.ResourceManager.GetString("_folder", new CultureInfo(wim.CurrentApplicationUser.LanguageCulture)), true, FolderType.Gallery);
            Map(x => x.RemoteLocation).TextLine("Url");
            Map(x => x.BrowsingFrame, this).HtmlContainer().Hide();
        }

        private void SetBrowsingFrame()
        {
            string referrer = string.Empty;
            string isImage = "0";
            if (wim?.Console?.Request?.Query?.ContainsKey("referid") == true)
            {
                referrer = wim.Console.Request.Query["referid"].ToString();
            }

            if (IsImage)
            { 
                isImage = wim.Console.Request.Query["isimage"].ToString();
            }

            string url = $"{wim.Console.WimPagePath}?top=3&referid={referrer}&openinframe=1&selectOnly=1&isimage={isImage}";
            BrowsingFrame = $"<iframe src=\"{url}\" title=\"Browse galleries\" style=\"border:none;\" width=\"100%\" height=\"300px\"></iframe>";
        }
        
        public override void Evaluate()
        {
            SetBrowsingFrame();
            if (wim?.CurrentApplicationUserRole?.CanSeeGallery != true || OnlyCreate)
            {
                var createOrSelect = Find(x => x.CreateOrSelect, this);
                if (createOrSelect != null)
                {
                    createOrSelect.Hide();
                }
            }

            if (wim?.Console?.Request?.Query?.ContainsKey("asset") == true && wim?.Console?.Request?.Query?.ContainsKey("selectOnly") == true)
            {
                int assetId = Utility.ConvertToInt(wim.Console.Request.Query["asset"], 0);
                if (assetId > 0)
                {
                    Asset file = Asset.SelectOne(assetId);
                    if (file?.ID > 0)
                    {
                        wim.Page.Body.Form.PostDataToSubSelect(file.ID, file.Title, editUrl: wim.GetUrl(new KeyValue[] { new KeyValue("item", file.ID) }), listTitle: wim.ListTitle);
                        wim.Page.Body.Form.CloseLayer();
                    }
                }
            }

            // Create new
            if (CreateOrSelect == 1)
            {
                Find(x => x.File, this).Show();
                Find(x => x.Title).Show();
                Find(x => x.Description).Show();
                Find(x => x.GalleryID).Show();
                Find(x => x.RemoteLocation).Show();

                Find(x => x.BrowsingFrame, this).Hide();
            }
            // Select existing
            else if (CreateOrSelect == 2)
            {
                Find(x => x.File, this).Hide();
                Find(x => x.Title).Hide();
                Find(x => x.Description).Hide();
                Find(x => x.GalleryID).Hide();
                Find(x => x.RemoteLocation).Hide();

                Find(x => x.BrowsingFrame, this).Show();
                wim.HideSaveButtons = true;
            }

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
        
        private bool OnlyCreate { get; set; }
        private bool IsImage { get; set; }
        public int CreateOrSelect { get; set; } = 1;
        public FileUpload File { get; set; }
        public string BrowsingFrame { get; set; }
    }
}
