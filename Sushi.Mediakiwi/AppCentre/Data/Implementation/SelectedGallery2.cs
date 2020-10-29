using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Sushi.Mediakiwi.Framework;
using System.Globalization;
using System.Collections;

namespace Sushi.Mediakiwi.AppCentre.Data.Implementation
{
    /// <summary>
    /// 
    /// </summary>
    public class SelectedGallery2List : ComponentListTemplate
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SelectedGalleryList"/> class.
        /// </summary>
        public SelectedGallery2List()
        {
            wim.HideOpenCloseToggle = true;
            //wim.HideProperties = true;
            wim.HideCreateNew = true;
            wim.HideExportOptions = true;

            wim.CanContainSingleInstancePerDefinedList = string.IsNullOrEmpty(Request.QueryString["group"]);
            if (this.ShowButtonMulti || !IsUploadView) wim.OpenInEditMode = true;
            //wim.ListTitle = Data.Product.SelectOne(Wim.Utility.ConvertToInt(Request.QueryString["groupitem"])).Title;
            
            this.ListLoad += new ComponentListEventHandler(ProductGalleryList_ListLoad);
            this.ListSearch += new ComponentSearchEventHandler(ProductGalleryList_ListSearch);
            this.ListAction += new ComponentActionEventHandler(ProductGalleryList_ListAction);
            this.ListPreRender += new ComponentListEventHandler(SelectedGalleryList_ListPreRender);
        }

        void SelectedGalleryList_ListPreRender(object sender, ComponentListEventArgs e)
        {
            if (ShowSingleAsset && string.IsNullOrEmpty(Implement.Title))
            {
                if (m_File != null && m_File.ContentLength > 0)
                {
                    string[] split = m_File.FileName.Split('\\');

                    int index = m_File.FileName.LastIndexOf('.') + 1;
                    string[] fileSplit = m_File.FileName.Split('\\');

                    string extention = m_File.FileName.Substring(index, m_File.FileName.Length - index);
                    this.Implement.Title = split[split.Length - 1].Replace(string.Concat(".", extention), string.Empty);
                }
            }
        }

        /// <summary>
        /// Handles the ListSave event of the SelectedGalleryList control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Sushi.Mediakiwi.Framework.ComponentListEventArgs"/> instance containing the event data.</param>
        void SelectedGalleryList_ListSave(object sender, ComponentListEventArgs e)
        {
            int type = Wim.Utility.ConvertToInt(Request.QueryString["type"]);
            if (ShowSingleAsset)
            {
                //this.Implement.Save();
                //if (!this.Implement.IsNewInstance)
                //{
                //    //  Delete existing asset!
                //    if (this.Implement.Exists)
                //        System.IO.File.Delete(this.Implement.LocalFilePath);
                //}
                Create(this.Implement, type);
                this.Implement.Save();

                if (type == 2)
                {
                    Response.Redirect(wim.GetCurrentQueryUrl(true,
                        new KeyValue() { Key = "item", Value = Implement.GalleryID },
                        new KeyValue() { Key = "type", Value = 1 }
                        )
                        );
                }
            }
        }

        /// <summary>
        /// Creates the specified document.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public Sushi.Mediakiwi.Data.Asset Create(Sushi.Mediakiwi.Data.Asset document, int type)
        {
            int galleryId;
            Guid guid;
            Sushi.Mediakiwi.Data.Gallery gallery = Sushi.Mediakiwi.Data.Gallery.SelectOne(document.GalleryID);
            //  Enter when type = upload new
            if (type != 2)
            {
                if (Wim.Utility.IsGuid(Request.QueryString["item"], out guid))
                {
                    gallery = Sushi.Mediakiwi.Data.Gallery.SelectOne(guid);
                    document.GalleryID = gallery.ID;
                }
                else if (Wim.Utility.IsNumeric(Request.QueryString["item"], out galleryId))
                {
                    if (galleryId > 0)
                    {
                        gallery = Sushi.Mediakiwi.Data.Gallery.SelectOne(galleryId);
                        document.GalleryID = gallery.ID;
                    }
                }

                if (gallery == null || gallery.IsNewInstance)
                {
                    gallery = Sushi.Mediakiwi.Data.Gallery.SelectOne(BaseGalleryID);
                    document.GalleryID = gallery.ID;
                }
            }
            if (gallery == null || gallery.IsNewInstance)
            {
                if (GalleryID.HasValue)
                    gallery = Sushi.Mediakiwi.Data.Gallery.SelectOne(GalleryID.Value);
                else
                    throw new Exception("No gallery is selected");
            }

            document.SaveStream(m_File, gallery);
            return document;
        }

        /// <summary>
        /// Handles the ListAction event of the ProductGalleryList control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Sushi.Mediakiwi.Framework.ComponentActionEventArgs"/> instance containing the event data.</param>
        void ProductGalleryList_ListAction(object sender, ComponentActionEventArgs e)
        {
            Sushi.Mediakiwi.Data.Asset asset = Sushi.Mediakiwi.Data.Asset.SelectOne(e.SelectedKey);
            if (FilterAddFile)
            {
                Response.Redirect(wim.GetCurrentQueryUrl(true,
                    new KeyValue() { Key = "Upload", Value = 1 }
                    )
                );
            }
            else if (ButtonRemove)
            {
                if (IsGridView)
                {
                    int galleryID = Wim.Utility.ConvertToInt(Request.QueryString[m_GALLERYKEY], BaseGalleryID);
                    if (galleryID == 0) galleryID = BaseGalleryID;

                    //  Register included
                    foreach (Sushi.Mediakiwi.Data.Asset item in Sushi.Mediakiwi.Data.Asset.SelectAll(galleryID))
                    {
                        if (wim.Grid.IsCheckboxChecked("IsIncluded", item.ID))
                        {
                            item.Delete();
                        }
                    }
                    wim.CurrentVisitor.Data.Apply("wim.showedit", null);
                    wim.CurrentVisitor.Save();
                    Response.Redirect(Request.Url.ToString());
                }
                else
                {
                    asset.Delete();
                    Response.Redirect(wim.GetCurrentQueryUrl(true,
                        new KeyValue() { Key = "item", Value = asset.GalleryID },
                        new KeyValue() { Key = "type", Value = 1 }
                        )
                        );
                }
            }
            //else if (ButtonSave)
            //{

            //    asset.Save();

            //    Response.Redirect(wim.GetCurrentQueryUrl(true,
            //        new KeyValue() { Key = "item", Value = asset.GalleryID },
            //        new KeyValue() { Key = "type", Value = 1 }
            //        )
            //        );
            //}
            else if (FilterGoBack)
            {
                if (Request.QueryString["type"] == "2")
                {
                    //  Asset
                    Response.Redirect(wim.GetCurrentQueryUrl(true,
                        new KeyValue() { Key = "item", Value = asset.GalleryID },
                        new KeyValue() { Key = "type", Value = 1 },
                        new KeyValue() { Key = "Upload", RemoveKey = true }
                        )
                        );
                }
                else
                {
                    Response.Redirect(wim.GetCurrentQueryUrl(true,
                        //new KeyValue() { Key = "item", Value = asset.GalleryID },
                        //new KeyValue() { Key = "type", Value = 1 },
                        new KeyValue() { Key = "Upload", RemoveKey = true }
                        )
                        );
                }
            }
            else if (ButtonSwitch)
            {
                wim.CurrentApplicationUser.ShowDetailView = !wim.CurrentApplicationUser.ShowDetailView;
                wim.CurrentApplicationUser.Save();
                Response.Redirect(Request.Url.ToString());
            }
            //else if (ButtonMulti)
            //{
            //    wim.CurrentVisitor.Data.Apply("wim.showmulti", true);
            //    wim.CurrentVisitor.Save();
            //    Response.Redirect(Request.Url.ToString());
            //}
            else if (FilterEditFiles)
            {
                wim.CurrentVisitor.Data.Apply("wim.showedit", true);
                wim.CurrentVisitor.Save();
                Response.Redirect(Request.Url.ToString());
            }
            else if (FilterResetEditFiles)
            {
                wim.CurrentVisitor.Data.Apply("wim.showedit", null);
                wim.CurrentVisitor.Save();
                Response.Redirect(Request.Url.ToString());
            }
            //else if (ButtonSingle)
            //{
            //    wim.CurrentVisitor.Data.Apply("wim.showmulti", null);
            //    wim.CurrentVisitor.Save();
            //    Response.Redirect(Request.Url.ToString());
            //}
        }

        /// <summary>
        /// Gets or sets the implement.
        /// </summary>
        /// <value>The implement.</value>
        [Sushi.Mediakiwi.Framework.ContentListItem.DataExtend()]
        public Sushi.Mediakiwi.Data.Asset Implement { get; set; }

        private HttpPostedFile m_File;
        /// <summary>
        /// Gets or sets the file.
        /// </summary>
        /// <value>The file.</value>
        [Sushi.Mediakiwi.Framework.OnlyVisibleWhenTrue("ShowSingleAsset")]
        [Sushi.Mediakiwi.Framework.ContentListItem.FileUpload("_selectfile", false)]
        public HttpPostedFile File
        {
            get { return m_File; }
            set { m_File = value; }
        }

        ListItemCollection m_AssetTypes;
        public ListItemCollection AssetTypes
        {
            get
            {
                if (m_AssetTypes == null)
                {
                    m_AssetTypes = new ListItemCollection();
                    m_AssetTypes.Add(new ListItem(""));
                    var selection = from item in Sushi.Mediakiwi.Data.AssetType.SelectAll() orderby item.Name select item;
                    foreach (var item in selection)
                    {
                        m_AssetTypes.Add(new ListItem(item.Name, item.ID.ToString()));
                    }
                }
                return m_AssetTypes;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [show single asset].
        /// </summary>
        /// <value><c>true</c> if [show single asset]; otherwise, <c>false</c>.</value>
        public bool ShowSingleAsset { get; set; }
        /// <summary>
        /// Gets a value indicating whether [show item instance].
        /// </summary>
        /// <value><c>true</c> if [show item instance]; otherwise, <c>false</c>.</value>
        public bool ShowItemInstance { get { return ShowSingleAsset || IsUploadView; } }

        public bool ShowBackToGallery { get; set; }
        public bool ShowRemove { get; set; }
        public bool ShowSwitch { get; set; }
        public bool ShowUpload { get; set; }
        public bool ShowEdit { get; set; }
        public bool ShowEditReset { get; set; }

        void ProductGalleryList_ListLoad(object sender, ComponentListEventArgs e)
        {
            ValidateAccessSecurity();

            int type = Wim.Utility.ConvertToInt(Request.QueryString["type"], 1);

            bool shouldShowMulti = wim.CurrentVisitor.Data["wim.showmulti"].ParseBoolean(false);
            bool shouldShowEdit = wim.CurrentVisitor.Data["wim.showedit"].ParseBoolean(false);

            this.IsListEditMode = shouldShowEdit;

            //  Asset view
            if (IsGridView)
            {
                ShowSwitch = true;

                if (shouldShowEdit)
                {
                    ShowEditReset = true;
                    ShowRemove = true;
                }
                else
                {
                    ShowEdit = true;
                    ShowUpload = true;
     
                }
                if (!wim.CurrentApplicationUser.ShowDetailView)
                {
                    this.IsListEditMode = false;
                    ShowEdit = false;
                    ShowEditReset = false;
                    ShowRemove = false;
                }
            }
            else
            {
                ShowBackToGallery = true;
            }

            if (type == 2 || this.ShowButtonMulti)
                this.ListSave += new ComponentListEventHandler(SelectedGalleryList_ListSave);

            Sushi.Mediakiwi.Data.Asset currentAsset;
            Sushi.Mediakiwi.Data.Gallery currentGallery;

            if (IsUploadView && shouldShowMulti)
            {
                wim.HideSaveButtons = true;
                m_IsUploadTypeMulti = true;
            }
            //  Set the asset input form visibility based on the state of the form (single or multi)
            ShowSingleAsset = this.ShowButtonMulti;

            if (type == 1)
            {
                int galleryID = Wim.Utility.ConvertToInt(Request.QueryString[m_GALLERYKEY], BaseGalleryID);
                galleryID = this.GalleryID.GetValueOrDefault();
                if (galleryID == 0) galleryID = BaseGalleryID;

                currentGallery = Sushi.Mediakiwi.Data.Gallery.SelectOne(galleryID);
                currentAsset = null;// Sushi.Mediakiwi.Data.Asset.SelectOne(key);
                Implement = new Sushi.Mediakiwi.Data.Asset();
            }
            else
            {
                //  Opening a existing document in its text or edit mode
                currentAsset = Sushi.Mediakiwi.Data.Asset.SelectOne(e.SelectedKey);
                Implement = currentAsset;
                currentGallery = Sushi.Mediakiwi.Data.Gallery.SelectOne(currentAsset.GalleryID);
                ShowSingleAsset = true;
                ShowRemove = true;
            }

            string handler = "/repository/Handler.ashx";

            wim.Debug(currentGallery.ID);
            wim.Debug(BaseGalleryID);


//            string file = Wim.Utility.AddApplicationPath(string.Format("ClientBin/SilverlightMultiFileUpload.xap?q={0}", Guid.NewGuid().ToString()));
//            MultiFile = string.Concat(@"
//        <object id=""SilverlightPlugin1"" width=""725"" height=""280""
//             data=""data:application/x-silverlight-2,""   
//             type=""application/x-silverlight-2"" >  
//             <param name=""initParams"" value=""MaxFileSizeKB=,MaxUploads=2,FileFilter=,DefaultColor=White,UploadHandlerName=" + handler + @",HttpUploader=True,CustomParam=" + currentGallery .ID + @""" />
//             <param name=""source"" value=""" + file + @"""/>  
//         </object>");
        }

        /// <summary>
        /// Gets or sets a value indicating whether [button remove].
        /// </summary>
        /// <value><c>true</c> if [button remove]; otherwise, <c>false</c>.</value>
        [Sushi.Mediakiwi.Framework.OnlyVisibleWhenTrue("ShowRemove")]
        [Sushi.Mediakiwi.Framework.ContentListItem.Button("Remove", false, ButtonIconType.Deny, IconTarget = ButtonTarget.TopRight , AskConfirmation = true)]
        public bool ButtonRemove { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [button switch].
        /// </summary>
        /// <value><c>true</c> if [button switch]; otherwise, <c>false</c>.</value>
        [Sushi.Mediakiwi.Framework.OnlyVisibleWhenTrue("ShowSwitch")]
        [Sushi.Mediakiwi.Framework.ContentListItem.Button("Switch view", false, ButtonIconType.Default, IconTarget = ButtonTarget.TopRight)]
        public bool ButtonSwitch { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [filter add file].
        /// </summary>
        /// <value><c>true</c> if [filter add file]; otherwise, <c>false</c>.</value>
        [Sushi.Mediakiwi.Framework.OnlyVisibleWhenTrue("ShowUpload")]
        [Sushi.Mediakiwi.Framework.ContentListItem.Button("Upload document(s)", false, IconType = ButtonIconType.NewItem )]
        public bool FilterAddFile { get; set; }

        [Sushi.Mediakiwi.Framework.OnlyVisibleWhenTrue("ShowEdit")]
        [Sushi.Mediakiwi.Framework.ContentListItem.Button("Edit files", false, IconType = ButtonIconType.Default, IconTarget = ButtonTarget.TopRight)]
        public bool FilterEditFiles { get; set; }

        [Sushi.Mediakiwi.Framework.OnlyVisibleWhenTrue("ShowEditReset")]
        [Sushi.Mediakiwi.Framework.ContentListItem.Button("Undo edit", false, IconType = ButtonIconType.Sorting, IconTarget = ButtonTarget.TopLeft)]
        public bool FilterResetEditFiles { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [filter go back].
        /// </summary>
        /// <value><c>true</c> if [filter go back]; otherwise, <c>false</c>.</value>
        [Sushi.Mediakiwi.Framework.OnlyVisibleWhenTrue("ShowBackToGallery")]
        [Sushi.Mediakiwi.Framework.ContentListItem.Button("Back to gallery", false, ButtonIconType.Default)]
        public bool FilterGoBack { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [button multi].
        /// </summary>
        /// <value><c>true</c> if [button multi]; otherwise, <c>false</c>.</value>
        //[Sushi.Mediakiwi.Framework.OnlyVisibleWhenTrue("ShowButtonMulti")]
        //[Sushi.Mediakiwi.Framework.ContentListItem.Button("Multi-file upload", false)]
        //public bool ButtonMulti { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [button single].
        /// </summary>
        /// <value><c>true</c> if [button single]; otherwise, <c>false</c>.</value>
        //[Sushi.Mediakiwi.Framework.OnlyVisibleWhenTrue("ShowButtonSingle")]
        //[Sushi.Mediakiwi.Framework.ContentListItem.Button("Single file upload", false)]
        //public bool ButtonSingle { get; set; }

        /// <summary>
        /// Gets or sets the filter title.
        /// </summary>
        /// <value>The filter title.</value>
        //[Sushi.Mediakiwi.Framework.OnlyVisibleWhenTrue("IsGridView")]
        //[Sushi.Mediakiwi.Framework.ContentListItem.TextField("_search_for", 50)]
        //public string FilterTitle { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [button search].
        /// </summary>
        /// <value><c>true</c> if [button search]; otherwise, <c>false</c>.</value>
        //[Sushi.Mediakiwi.Framework.OnlyVisibleWhenTrue("IsGridView")]
        //[Sushi.Mediakiwi.Framework.ContentListItem.Button("Zoek", false, IconTarget = ButtonTarget.BottomRight)]
        //public bool ButtonSearch { get; set; }

        protected int BaseGalleryID { get; set; }

        string m_GALLERYKEY = "item";

        void ValidateAccessSecurity()
        {
            //  Check for access to galleries or assets outside the set gallery.
            //  Check for possible gallery access restrictions for the current user.
        }

        //  Create custom props
        //  Create add/update/delete Folder options
        //  Create search function

        bool IsListEditMode = false;

        void ProductGalleryList_ListSearch(object sender, ComponentListSearchEventArgs e)
        {
            if (IsListEditMode)
            {
                ShowUpload = false;
                ShowBackToGallery = false;
                ShowSwitch = false;
            }

            ValidateAccessSecurity();
            if (ShowItemInstance) return;

            wim.HideTopSectionTag = true;

            List<Sushi.Mediakiwi.AppCentre.Data.Implementation.Browsing.BrowseItem> list = 
                new List<Sushi.Mediakiwi.AppCentre.Data.Implementation.Browsing.BrowseItem>();

            

            wim.ListDataColumns.Add("ID", "ID", Sushi.Mediakiwi.Framework.ListDataColumnType.UniqueIdentifier);

            if (IsListEditMode)
            {
                wim.SearchListCanClickThrough = false;
                wim.ListDataColumns.Add("", "IsIncluded", ListDataColumnType.Checkbox, 30);
            }
            else
                wim.ListDataColumns.Add("", "Icon", 20);
            
            wim.ListDataColumns.Add("", "Title", Sushi.Mediakiwi.Framework.ListDataColumnType.HighlightPresent);
            wim.ListDataColumns.Add("", "Info2", 250);
            wim.ListDataColumns.Add("", "Info4", 70, Align.Right);
            
            wim.ListDataColumns.Add("", "Info1", 60, Align.Right);
            wim.ListDataColumns.Add("Modified", "Info3", 90);
            wim.ListDataColumns.ColumnItemUrl = "TypeUrl";
            //wim.SearchResultItemPassthroughParameter = string.Format("list={0}&gallery", wim.CurrentList.ID);

            int type = Wim.Utility.ConvertToInt(Request.QueryString["type"], 1);
            Sushi.Mediakiwi.Data.Asset currentAsset;
            Sushi.Mediakiwi.Data.Gallery currentGallery;


            if (type == 1)
            {
                int galleryID = Wim.Utility.ConvertToInt(Request.QueryString[m_GALLERYKEY], BaseGalleryID);
                galleryID = this.GalleryID.GetValueOrDefault();
                if (galleryID == 0) galleryID = BaseGalleryID;
                currentGallery = Sushi.Mediakiwi.Data.Gallery.SelectOne(galleryID);
                currentAsset = null;// Sushi.Mediakiwi.Data.Asset.SelectOne(key);
            }
            else
            {
                currentAsset = Sushi.Mediakiwi.Data.Asset.SelectOne(e.SelectedItemKey);
                currentGallery = Sushi.Mediakiwi.Data.Gallery.SelectOne(currentAsset.GalleryID);
            }

            wim.ListTitle = currentGallery.Name;

            if (!IsListEditMode && !this.HideFolders)
            {
                //Product product = Product.SelectOne(e.SelectedGroupItemKey);
                if (currentGallery.ID != BaseGalleryID && currentGallery.ParentID.HasValue)
                {
                    list.Add(new Sushi.Mediakiwi.AppCentre.Data.Implementation.Browsing.BrowseItem()
                     {
                         TypeUrl = wim.GetCurrentQueryUrl(true, new KeyValue() { Key = m_GALLERYKEY, Value = currentGallery.ParentID.Value }, new KeyValue() { Key = "type", Value = 1 }),
                         Title = "...",
                         Icon = wim.CurrentApplicationUser.ShowDetailView ?
                              string.Format("<img alt=\"\" src=\"{0}/images/icon_folder_link.png\"/>", wim.Console.WimRepository)
                              : string.Concat(wim.Console.WimRepository, "/images/", "thumb_folder.png"),
                     });
                }

                foreach (Sushi.Mediakiwi.Data.Gallery gallery in Sushi.Mediakiwi.Data.Gallery.SelectAllByParent(currentGallery.ID, true))
                {
                    list.Add(new Sushi.Mediakiwi.AppCentre.Data.Implementation.Browsing.BrowseItem()
                        {
                            TypeUrl = wim.GetCurrentQueryUrl(true, new KeyValue() { Key = m_GALLERYKEY, Value = gallery.ID }, new KeyValue() { Key = "type", Value = 1 }),

                            Title = gallery.Name,
                            Icon = wim.CurrentApplicationUser.ShowDetailView ?
                                string.Format("<img alt=\"\" src=\"{0}/images/icon_folder_link.png\"/>", wim.Console.WimRepository)
                                : string.Concat(wim.Console.WimRepository, "/images/", "thumb_folder.png"),
                            Info1 = gallery.AssetCount.ToString(),
                            Info3 = gallery.Created
                        }
                    );
                }
            }

            foreach (Sushi.Mediakiwi.Data.Asset asset in Sushi.Mediakiwi.Data.Asset.SelectAll(currentGallery.ID))
            {
                string candidate = "undefined_16.png";
                string icon = "";

                if (wim.CurrentApplicationUser.ShowDetailView)
                {
                    switch (asset.Extention.ToLower())
                    {
                        case "docx":
                        case "doc": candidate = "doc_16.png"; break;
                        case "pdf": candidate = "pdf_16.png"; break;
                        case "jpeg":
                        case "jpg":
                        case "png":
                        case "bmp":
                        case "gif": candidate = "image_16.png"; break;
                        case "xls":
                        case "xlsx": candidate = "xls_16.png"; break;
                        case "ppt":
                        case "pptx": candidate = "ppt_16.png"; break;
                        case "zip":
                        case "rar": candidate = "zip_16.png"; break;
                        case "vsd": candidate = "vsd_16.png"; break;
                        case "eml":
                        case "msg": candidate = "msg_16.png"; break;
                        case "txt": candidate = "txt_16.png"; break;
                    }
                    icon = string.Format("<img alt=\"\" src=\"{0}/images/icons/{1}\"/>", wim.Console.WimRepository, candidate);
                }
                else
                {
                    string ext = "png";
                    string candidate2 = string.Concat("thumb_unknown.", ext);
                    //asset.CreateThumbnail();

                    if (asset.IsImage)
                    {
                        if (asset.Exists || asset.IsRemote || !asset.RemoteDownload)
                            icon = asset.ImageInstance.ThumbnailPath;
                        else
                            icon = string.Concat(wim.Console.WimRepository, "/images/", candidate2);
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(asset.Extention))
                        {
                            switch (asset.Extention.ToLower())
                            {
                                case "doc": candidate2 = "thumb_word.png"; break;
                                case "pdf": candidate2 = "thumb_acrobat.png"; break;
                                case "xls": candidate2 = "thumb_excel.png"; break;
                                case "ppt": candidate2 = "thumb_powerpoint.png"; break;
                                case "zip": candidate2 = "thumb_zip.png"; break;
                                case "mov": candidate2 = "thumb_mov.png"; break;
                                case "wmv": candidate2 = "thumb_wmv.png"; break;
                            }
                        }
                        icon = string.Concat(wim.Console.WimRepository, "/images/", candidate2);
                    }
                }

                list.Add(new Sushi.Mediakiwi.AppCentre.Data.Implementation.Browsing.BrowseItem()
                    {
                        ID = asset.ID,
                        TypeUrl = wim.GetCurrentQueryUrl(true, 
                            new KeyValue() { Key = "item", Value = asset.ID }, 
                            new KeyValue() { Key = "type", Value = 2 }                         
                            ),
                        Title = asset.Title,
                        Icon = icon,
                        Info1 = string.Format("{0} KB", asset.Size == 0 ? "?" : (asset.Size / 1024).ToString()),
                        Info2 = asset.AssetType,
                        Info3 = asset.Created,
                        Info4 = asset.IsImage ? 
                                asset.Width == 0 ? "" : string.Format("{0} x {1}", asset.Width, asset.Height) 
                            : null
                    }
                );
            }

            wim.ListData = list;
        }

        public bool IsUploadView 
        {
            get { return Request.Params["upload"] == "1"; }
        }
        public int SelectedType
        {
            get { return Convert.ToInt32(Request.Params["type"]); }
        }

        public bool IsGridView
        {
            get { return (!IsUploadView && SelectedType != 2); }
        }

        bool m_IsUploadTypeMulti;
        public bool ShowButtonMulti 
        {
            get { return IsUploadView && !m_IsUploadTypeMulti; }
        }
        public bool ShowButtonSingle
        {
            get { return IsUploadView && m_IsUploadTypeMulti; }
        }
        public bool ShowMultiUpload
        {
            get { return IsUploadView && !ShowButtonMulti; }
        }

        //[Sushi.Mediakiwi.Framework.OnlyVisibleWhenTrue("ShowMultiUpload")]
        //[Sushi.Mediakiwi.Framework.ContentListItem.HtmlContainer(true)]
        //public string MultiFile { get; set; }

        [Sushi.Mediakiwi.Framework.ContentListItem.DataList(HasThumbnailOption = true )]
        public Sushi.Mediakiwi.Data.DataList SearchGrid { get; set; }

        ListItemCollection m_GalleryList;
        public ListItemCollection GalleryList
        {
            get
            {
                if (m_GalleryList == null)
                {
                    m_GalleryList = new ListItemCollection();
                    var selection = Sushi.Mediakiwi.Data.Gallery.SelectAll();
                    foreach(var item in selection)
                        m_GalleryList.Add(new ListItem(item.CompletePath, item.ID.ToString()));
                }
                return m_GalleryList;
            }
        }

        [Sushi.Mediakiwi.Framework.ContentSettingItem.Choice_Dropdown("Gallery", "GalleryList", false)]
        public int? GalleryID { get; set; }

        [Sushi.Mediakiwi.Framework.ContentSettingItem.Choice_Checkbox("Hide folder")]
        public bool HideFolders { get; set; }
    }
}
