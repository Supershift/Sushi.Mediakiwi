using Sushi.Mediakiwi.AppCentre.UI.Forms;
using Sushi.Mediakiwi.Data;
using Sushi.Mediakiwi.Data.Configuration;
using Sushi.Mediakiwi.Framework;
using Sushi.Mediakiwi.Logic;
using Sushi.Mediakiwi.Persisters;
using Sushi.Mediakiwi.UI;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.AppCentre.Data.Implementation
{
    /// <summary>
    /// Summary description for FileHandler
    /// </summary>
    public class MultiUpload
    {
        /// <summary>
        /// This represents one row in the returned JSON object
        /// </summary>
        public class uploadResultFile
        {
            public int ID { get; set; }
            public string Thumbnail_url { get; set; }
            public string Name { get; set; }
            public int Length { get; set; }
            public string Type { get; set; }

        }

        /// <summary>
        /// 
        /// </summary>
        public class uploadResult
        {
            private List<uploadResultFile> _files;
            public List<uploadResultFile> files
            {
                get
                {
                    if (_files == null)
                    {
                        _files = new List<uploadResultFile>();
                    }

                    return _files;
                }
                set { _files = value; }
            }

        }

        
    }

    public class Image : Document
    {
        public Image(AssetService assetService) : base(assetService)
        {

        }
    }

    /// <summary>
    /// Represents a Document entity.
    /// </summary>
    public class Document : BaseImplementation
    {

        ListItemCollection m_AssetTypes;
        public ListItemCollection AssetTypes
        {
            get
            {
                if (m_AssetTypes == null)
                {
                    m_AssetTypes = new ListItemCollection();
                    m_AssetTypes.Add(new ListItem(""));
                    var selection = from item in AssetType.SelectAll() orderby item.Name select item;
                    foreach (var item in selection)
                    {
                        m_AssetTypes.Add(new ListItem(item.Name, item.ID.ToString()));
                    }
                }
                return m_AssetTypes;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Document"/> class.
        /// </summary>
        public Document(AssetService assetService)
        {
            ListInit += Document_ListInit;
            ListLoad += Document_ListLoad;
            ListSave += Document_ListSave;
            ListDelete += Document_ListDelete;
            ListSearch += Document_ListSearch;
            _assetService = assetService;
        }

        async Task Document_ListInit()
        {
            wim.HideEditOption = true;
            wim.HideOpenCloseToggle = true;
            wim.OpenInEditMode = true;
            wim.Page.HideBreadCrumbs = true;
            wim.Page.Body.Navigation.Menu.DeleteButtonTarget = ButtonTarget.BottomLeft;
        }

        /// <summary>
        /// Handles the ListDelete event of the Document control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ComponentListEventArgs"/> instance containing the event data.</param>
        async Task Document_ListDelete(ComponentListEventArgs e)
        {
            await _assetService.DeleteAsync(m_Implement, Azure_Image_Container);

            if (wim.IsLayerMode)
            {
                int rootAsset = Utility.ConvertToInt(Request.Query["base"]);
                string redirect = wim.GetUrl(
                    new KeyValue()
                    {
                        Key = "item",
                        Value = rootAsset
                    },
                    new KeyValue()
                    {
                        Key = "base",
                        RemoveKey = true
                    },
                    new KeyValue()
                    {
                        Key = "sat",
                        RemoveKey = true
                    }
                );
                Response.Redirect(redirect);

                wim.OnSaveScript = $@"<input type=""hidden"" class=""postparent"" id=""{m_Implement.ID}"" value=""{m_Implement.Title} ({(m_Implement.Size > 0 ? (m_Implement.Size / 1024) : 0)} KB)"" />";
            }
            else
            {
                string redirect = wim.GetUrl(
                    new KeyValue()
                    {
                        Key = "gallery",
                        Value = m_Implement.GalleryID
                    },
                    new KeyValue()
                    {
                        Key = "asset",
                        RemoveKey = true
                    },
                    new KeyValue()
                    {
                        Key = "item",
                        RemoveKey = true
                    });
                Response.Redirect(redirect);
            }
        }

        public virtual string Azure_Image_Container
        {
            get
            {
                return WimServerConfiguration.Instance?.Azure_Image_Container;
            }
        }

        public virtual string Azure_Cdn_Uri
        {
            get
            {
                return WimServerConfiguration.Instance?.Azure_Cdn_Uri;
            }
        }

        /// <summary>
        /// Handles the ListSave event of the Document control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ComponentListEventArgs"/> instance containing the event data.</param>
        async Task Document_ListSave(ComponentListEventArgs e)
        {
            int? parent = Utility.ConvertToIntNullable(Request.Query["base"]);
            if (parent.HasValue)
            {
                m_Implement.ParentID = parent;
            }

            Asset currentAsset = m_Implement;
            if (m_Implement.ParentID.HasValue)
            {
                currentAsset = await Asset.SelectOneAsync(m_Implement.ParentID.Value).ConfigureAwait(false);
                m_Implement.GalleryID = currentAsset.GalleryID;
            }

            if (_Form.File != null && _Form.File.File != null)
            {
                _Form.Evaluate();

                // open stream
                using var inputStream = _Form.File.File.OpenReadStream();

                // create or update asset
                m_Implement = await _assetService.UpsertAssetAsync(m_Implement, inputStream, Azure_Image_Container, _Form.File.File.FileName, _Form.File.File.ContentType);
            }

            string info;
            if (currentAsset.IsImage)
            {
                info = $"{currentAsset.Title} <span>({currentAsset.Width}px / {currentAsset.Height}px)</span>";
            }
            else
            {
                info = $"{currentAsset.Title} <span>({(currentAsset.Size > 0 ? (currentAsset.Size / 1024) : 0)} KB)</span>";
            }

            if (string.IsNullOrEmpty(Request.Query["referid"]))
            {
                wim.Page.Body.Form.RefreshParent();
            }
            else
            {
                wim.Page.Body.Form.PostDataToSubSelect(currentAsset.ID, info, editUrl: wim.GetUrl(new KeyValue[] { new KeyValue("item", currentAsset.ID) } ), listTitle: wim.ListTitle);
            }
        }

        [OnlyVisibleWhenTrue(nameof(ShowButtonMulti))]
        [Framework.ContentListItem.Choice_Dropdown("Gallery", nameof(GalleryCollection), true, true)]
        public int AssignedGalleryID2 { get; set; }

        /// <summary>
        /// Gets or sets the multi file.
        /// </summary>
        /// <value>The multi file.</value>
        [OnlyVisibleWhenTrue(nameof(ShowButtonMulti))]
        [Framework.ContentListItem.HtmlContainer(true)]
        public string MultiFile { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [button multi].
        /// </summary>
        /// <value><c>true</c> if [button multi]; otherwise, <c>false</c>.</value>
        [OnlyVisibleWhenTrue(nameof(ShowButtonMulti), false)]
        [Framework.ContentListItem.Button("Multi-file upload", false)]
        public bool ButtonMulti { get; set; }

        public string ButtonBack2URL { get; set; }
        [Framework.ContentListItem.Button("", false, 
            ButtonClassName = "flaticon icon-home", 
            IconTarget = ButtonTarget.TopRight, 
            CustomUrlProperty = nameof(ButtonBack2URL))]
        public bool ButtonBack2 { get; set; }


        [Framework.ContentListItem.Button("", false, 
            ButtonClassName = "flaticon icon-rotate", 
            IconTarget = ButtonTarget.TopRight, 
            InteractiveHelp = "Redo upload")]
        public bool ButtonBack { get; set; }

        public string AlternativeURL { get; set; }
        [Framework.ContentListItem.Button("", false, 
            ButtonClassName = "flaticon icon-platforms", 
            IconTarget = ButtonTarget.TopRight, 
            InteractiveHelp = "Add/update alternative image", 
            CustomUrlProperty = nameof(AlternativeURL))]
        public bool ButtonAlternative { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [show button multi].
        /// </summary>
        /// <value><c>true</c> if [show button multi]; otherwise, <c>false</c>.</value>
        public bool ShowButtonMulti { get; set; }

        /// <summary>
        /// Gets a value indicating whether [show single asset].
        /// </summary>
        /// <value><c>true</c> if [show single asset]; otherwise, <c>false</c>.</value>
        public bool ShowSingleAsset { get { return !ShowButtonMulti; } }

        /// <summary>
        /// Gets or sets a value indicating whether [button single].
        /// </summary>
        /// <value><c>true</c> if [button single]; otherwise, <c>false</c>.</value>
        [OnlyVisibleWhenTrue(nameof(ShowButtonMulti))]
        [Framework.ContentListItem.Button("Single file upload", false)]
        public bool ButtonSingle { get; set; }


        [Framework.ContentListItem.DataList()]
        public DataList AssetTypeSelectionList { get; set; }

        public class AssetTypeMapper
        {
            public AssetTypeMapped[] Map(IAssetType[] types, List<Asset> assets, string url)
            {
                List<AssetTypeMapped> arr = new List<AssetTypeMapped>();

                foreach (var type in types)
                {
                    int key = 0;
                    AssetTypeMapped candidate = new AssetTypeMapped();
                    candidate.AssetType = type.Name;
                    var match = (from item in assets where item.AssetTypeID.GetValueOrDefault() == type.ID select item).FirstOrDefault();
                    if (match != null)
                    {
                        candidate.AssetID = match.ID;
                        candidate.HasAsset = true;
                        if (match.IsImage)
                        {
                            candidate.DisplayName = $"{match.Title} ({match.Width}px / {match.Height}px)";
                        }
                        else
                        {
                            candidate.DisplayName = match.Title;
                        }
                        key = match.ID;
                    }
                    candidate.PassthroughParameter = $"{url}&item={key}&sat={type.ID}"; 
                    arr.Add(candidate);
                }

                return arr.ToArray();
            }
        }

        public class AssetTypeMapped
        {
            public int AssetID { get; set; }
            public string DisplayName { get; set; }
            public int AssetTypeID { get; set; }
            public string AssetType { get; set; }
            public bool HasAsset { get; set; }
            public string PassthroughParameter { get; set; }
        }

        async Task Document_ListSearch(ComponentListSearchEventArgs e)
        {
            var isImage = wim.Console.Request.Query.ContainsKey("isimage") && wim.Console.Request.Query["isimage"] == "1";
            var assets = await Asset.SelectAllAsync(isImage).ConfigureAwait(false);

            wim.ListDataColumns.Add(new ListDataColumn("ID", nameof(Asset.ID), ListDataColumnType.UniqueIdentifier));
            wim.ListDataColumns.Add(new ListDataColumn("Filename", nameof(Asset.FileName), ListDataColumnType.HighlightPresent));
            wim.ListDataColumns.Add(new ListDataColumn("Title", nameof(Asset.Title)));
            wim.ListDataColumns.Add(new ListDataColumn("Type", nameof(Asset.Type)));
            wim.ListDataAdd(assets);
        }

        DocumentForm _Form;
        Asset m_Implement;

        /// <summary>
        /// Handles the ListLoad event of the Document control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ComponentListEventArgs"/> instance containing the event data.</param>
        async Task Document_ListLoad(ComponentListEventArgs e)
        {
            wim.IsEditMode = true;

            AssetTypeSelectionList = new DataList();

            int rootAsset = Utility.ConvertToInt(Request.Query["base"], e.SelectedKey);
            bool isAlternativeImage = !string.IsNullOrEmpty(Request.Query["base"]);
            var alternativeTypeID = Utility.ConvertToIntNullable(Request.Query["sat"]);
            var isImage = Utility.ConvertToInt(Request.Query["isimage"], 0) == 1;
            var galleryId = Utility.ConvertToInt(Request.Query["gallery"], 0);

            wim.SetPropertyVisibility(nameof(ButtonAlternative), false);
            wim.SetPropertyVisibility(nameof(AssetTypeSelectionList), false);
            wim.SetPropertyVisibility(nameof(ButtonBack2), false);
            wim.SetPropertyVisibility("AssetTypeID", false);
            ShowButtonMulti = false;
            wim.SetPropertyVisibility(nameof(ButtonMulti), false);

            if (e.SelectedKey > 0)
            {
                var variantTypes = await AssetType.SelectAllAsync(true).ConfigureAwait(false);
                if (variantTypes.Length > 0)
                {
                    wim.SetPropertyVisibility(nameof(ButtonAlternative), true);
                }
            }

            ButtonBack2URL = wim.GetUrl(
                    new KeyValue() 
                    { 
                        Key = "base", 
                        RemoveKey = true 
                    }, 
                    new KeyValue() 
                    { 
                        Key = "item", 
                        Value = rootAsset 
                    },
                    new KeyValue() 
                    { 
                        Key = "sat", 
                        RemoveKey = true 
                    }, 
                    new KeyValue() 
                    { 
                        Key = "redo", 
                        RemoveKey = true 
                    });

            if (rootAsset > 0 && isAlternativeImage)
            {
                wim.SetPropertyVisibility(nameof(ButtonBack2), true);
            }

            AlternativeURL = wim.GetUrl(
                 new KeyValue() 
                 { 
                     Key = "base", 
                     Value = rootAsset 
                 }, 
                 new KeyValue() 
                 { 
                     Key = "item", 
                     Value = 0 
                 }, 
                 new KeyValue() 
                 { 
                     Key = "sat", 
                     RemoveKey = true 
                 }, 
                 new KeyValue() 
                 { 
                     Key = "redo", 
                     RemoveKey = true 
                 });

            if (isAlternativeImage && alternativeTypeID.HasValue)
            {
                wim.SetPropertyVisibility("AssetTypeID", true);
                wim.SetPropertyEditable("AssetTypeID", false);
            }

            if (isAlternativeImage && !alternativeTypeID.HasValue)
            {
                wim.SetPropertyVisibility("File", false);
                wim.SetPropertyVisibility(nameof(AssetTypeSelectionList), true);
            }

            if (Request.Query["ismulti"] == "1")
            {
                return;
            }

            wim.CanSaveAndAddNew = false;

            if (wim.CurrentVisitor.Data["wim.showmulti"].ParseBoolean(false))
            {
                wim.HideSaveButtons = true;
            }

            wim.SetPropertyVisibility(nameof(ButtonBack), false);

            //  In popup layer, hide the multi-file-upload.
            if (wim.Console.OpenInFrame > 0)
            {
                wim.SetPropertyVisibility(nameof(ButtonMulti), false);
            }

            //if (wim.CurrentList.Option_OpenInEditMode)
            //{
            //    wim.CurrentList.Option_OpenInEditMode = false;
            //    wim.CurrentList.Save();
            //}

            m_Implement = await Asset.SelectOneAsync(e.SelectedKey).ConfigureAwait(false);

            // If we land here from a gallery upload, preselect that gallery
            if (galleryId > 0 && (m_Implement == null || m_Implement.ID == 0))
            {
                m_Implement = new Asset()
                {
                    GalleryID = galleryId,
                    IsImage = isImage
                };
            }

            _Form = new DocumentForm(wim, m_Implement);

            FormMaps.Add(_Form);
        }

        #region List attributes
        private ListItemCollection m_GalleryCollection;
        /// <summary>
        /// Gets the gallery collection.
        /// </summary>  
        /// <value>The gallery collection.</value>
        public ListItemCollection GalleryCollection
        {
            get
            {
                if (m_GalleryCollection == null)
                {
                    m_GalleryCollection = new ListItemCollection();

                    Gallery[] galleries = Gallery.SelectAllAccessible(wim.CurrentApplicationUser);

                    foreach (Gallery gallery in galleries)
                    {
                        m_GalleryCollection.Add(new ListItem(gallery.CompletePath, $"{gallery.ID}"));
                    }
                }
                return m_GalleryCollection;
            }
        }

        /// <summary>
        /// Gets or sets the gallery.
        /// </summary>
        /// <value>The gallery.</value>
        [OnlyEditableWhenTrue(nameof(IsPopupRequest), false)]
        [OnlyVisibleWhenTrue(nameof(IsPresentOnServerOrSingleMultifile))]
        //[Sushi.Mediakiwi.Framework.ContentListItem.Choice_Dropdown("Gallery", "GalleryCollection")]
        public int AssignedGalleryID { get; set; }

        public bool IsPresentOnServerOrSingleMultifile { get; set; }

        public bool IsPopupRequest { get; set; }

        /// <summary>
        /// Gets or sets the thumbnail.
        /// </summary>
        /// <value>The thumbnail.</value>
        [OnlyVisibleWhenTrue(nameof(IsExistingImage))]
        //[Sushi.Mediakiwi.Framework.ContentListItem.TextLine("Thumbnail")]
        public string Thumbnail { get; set; }

        /// <summary>
        /// Gets or sets the size of the file.
        /// </summary>
        /// <value>The size of the file.</value>
        [OnlyVisibleWhenTrue(nameof(IsExisting))]
        //[Sushi.Mediakiwi.Framework.ContentListItem.TextLine("_filesize")]
        public string FileSize { get; set; }


        [OnlyVisibleWhenTrue(nameof(IsRTE_Set_Version))]
        //[Sushi.Mediakiwi.Framework.ContentListItem.TextLine("Alignment", Expression = Sushi.Mediakiwi.Framework.OutputExpression.Left)]
        public string RTE_Alignment
        {
            get
            {
                return @"
<select id=""align"" name=""align"" onchange=""ImageDialog.updateStyle('align');ImageDialog.changeAppearance();""> 
	<option value="""">-- Default --</option> 
	<option value=""baseline"">Baseline</option>
	<option value=""top"">Top</option>
	<option value=""middle"">Middle</option>
	<option value=""bottom"">Bottom</option>
	<option value=""text-top"">Text-top</option>
	<option value=""text-bottom"">Text-bottom</option>
	<option value=""left"">Left</option>
	<option value=""right"">Right</option>
	<option value=""none"">None</option>
</select>";
            }
        }

        public bool IsRTE_Set_Version { get; set; }

        //[Sushi.Mediakiwi.Framework.OnlyVisibleWhenTrue("IsRTE_Set_Version")]
        //[Sushi.Mediakiwi.Framework.ContentListItem.TextLine("Sample", Expression = Sushi.Mediakiwi.Framework.OutputExpression.Right)]
        public string RTE_Sample
        {
            get
            {
                string url = wim.AddApplicationPath("repository/wim/scripts/rte/plugins/advimage/img/sample.gif");
                string src = string.Empty;
                //if (m_Implement != null && !m_Implement.IsNewInstance)
                //{
                //    url = m_Implement.ImageInstance.ApplyForcedBorder(null, 45, 45, null, false, Sushi.Mediakiwi.Data.ImagePosition.Center, null, true, false);
                //    src = m_Implement.ImageInstance.DownloadUrl;
                //}

                return string.Format(@"
<div style=""border: 1px solid #000000; height: 140px; overflow: hidden; padding: 5px; width: 140px;"">
	<img id=""alignSampleImg"" src=""{0}"" />
	Lorem ipsum, Dolor sit amet, consectetuer adipiscing loreum ipsum edipiscing elit, sed diam
	nonummy nibh euismod tincidunt ut laoreet dolore magna aliquam erat volutpat.Loreum ipsum
	edipiscing elit, sed diam nonummy nibh euismod tincidunt ut laoreet dolore magna aliquam
	erat volutpat.
</div>
<input id=""style"" name=""style"" type=""hidden"" value=""" + Request.Form["style"] + @""" onchange=""ImageDialog.changeAppearance();"" />
<input id=""src"" name=""src"" type=""hidden"" value=""" + src + @""" />
"
                    //, Utility.AddApplicationPath("repository/wim/scripts/rte/plugins/advimage/img/sample.gif")
                    , url
                    );
            }
            set { }
        }

        public bool IsExisting { get; set; }
        public bool IsExistingImage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is not present on server.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is not present on server; otherwise, <c>false</c>.
        /// </value>
        public bool IsNotPresentOnServer { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance is gallery free.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is gallery free; otherwise, <c>false</c>.
        /// </value>
        public bool IsGalleryFree
        {
            get { return string.IsNullOrEmpty(Request.Query["g"]); }
        }

        private string m_Note = "<b>Document is not physically present on server!</b>";
        private readonly AssetService _assetService;

        /// <summary>
        /// Gets or sets the note.
        /// </summary>
        /// <value>The note.</value>
        //[Sushi.Mediakiwi.Framework.OnlyVisibleWhenTrue("IsNotPresentOnServer")]
        //[Sushi.Mediakiwi.Framework.ContentListItem.TextLine("Note")]
        public string Note
        {
            get { return m_Note; }
            set { m_Note = value; }
        }

        /// <summary>
        /// Gets a value indicating whether [inpopup layer].
        /// </summary>
        /// <value><c>true</c> if [inpopup layer]; otherwise, <c>false</c>.</value>
        public bool InpopupLayer
        {
            get { return (Request.Query["openinframe"] == "1"); }
        }

        #endregion List attributes
    }
}
