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
    public class SelectedGalleryList : ComponentListTemplate
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SelectedGalleryList"/> class.
        /// </summary>
        public SelectedGalleryList()
        {
            wim.HideOpenCloseToggle = true;
            wim.HideProperties = true;
            wim.HideCreateNew = true;
            wim.HideExportOptions = true;

            wim.HasSortOrder = true;
            //wim.HasSingleItemSortOrder = true;
            wim.SetSortOrder("wim_Assets", "Asset_Key", "Asset_SortOrder");

            wim.CanContainSingleInstancePerDefinedList = string.IsNullOrEmpty(Request.QueryString["group"]);
            if (this.ShowButtonMulti || !IsUploadView) wim.OpenInEditMode = true;
            //wim.ListTitle = Data.Product.SelectOne(Wim.Utility.ConvertToInt(Request.QueryString["groupitem"])).Title;
            
            this.ListLoad += new ComponentListEventHandler(SelectedGalleryList_ListLoad);
            this.ListSearch += new ComponentSearchEventHandler(SelectedGalleryList_ListSearch);
            this.ListAction += new ComponentActionEventHandler(SelectedGalleryList_ListAction);
            this.ListPreRender += new ComponentListEventHandler(SelectedGalleryList_ListPreRender);
        }

        void SelectedGalleryList_ListPreRender(object sender, ComponentListEventArgs e)
        {
            if (ShowSingleAsset && string.IsNullOrEmpty(Implement.Title))
            {
                if (m_File != null && m_File.ContentLength > 0)
                    this.Implement.Title = m_File.FileName;
            }

            if (ShowSingleAsset && wim.IsSaveMode && (m_File == null || m_File.ContentLength == 0))
            {
                if (this.Implement.ID == 0)
                    wim.Notification.AddError("Please select a file to upload.");
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

                if (m_File != null && m_File.ContentLength > 0)
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
                else
                {
                    Response.Redirect(wim.GetCurrentQueryUrl(true,
                    //new KeyValue() { Key = "item", Value = 0 },
                    new KeyValue() { Key = "Upload", RemoveKey = true }
                    )
                    );
                }
            }
            else
            {
                if(MultiFile.Length > 0 && this.Implement.AssetTypeID > 0)
                {
                    var assets = Sushi.Mediakiwi.Data.Asset.SelectRange(MultiFile);
                    foreach(var asset in assets)
                    {
                        asset.AssetTypeID = this.Implement.AssetTypeID;
                        asset.Save();
                    }
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
                gallery = Sushi.Mediakiwi.Data.Gallery.Identify(Request.QueryString["item"]);
                document.GalleryID = gallery.ID;
                document.DatabaseMappingPortal = gallery.DatabaseMappingPortal;

                //if (Wim.Utility.IsGuid(Request.QueryString["item"], out guid))
                //{
                //    gallery = Sushi.Mediakiwi.Data.Gallery.SelectOne(guid);
                //    document.GalleryID = gallery.ID;
                //}
                //else if (Wim.Utility.IsNumeric(Request.QueryString["item"], out galleryId))
                //{
                //    if (galleryId > 0)
                //    {
                //        gallery = Sushi.Mediakiwi.Data.Gallery.SelectOne(galleryId);
                //        document.GalleryID = gallery.ID;
                //    }
                //}

                if (gallery == null || gallery.IsNewInstance)
                {
                    gallery = Sushi.Mediakiwi.Data.Gallery.SelectOne(BaseGalleryID);
                    document.GalleryID = gallery.ID;
                }
            }
            if (gallery == null || gallery.IsNewInstance) 
                throw new Exception("No gallery is selected");

            document.FileName = m_File.FileName;
            if (string.IsNullOrEmpty(Implement.Title))
                document.Title = document.FileName;

            if (!Sushi.Mediakiwi.Data.AssetLogic.CloudUpload(document, gallery, File))
            {
                document.SaveStream(m_File, gallery);
            }
            return document;
        }

        /// <summary>
        /// Handles the ListAction event of the ProductGalleryList control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Sushi.Mediakiwi.Framework.ComponentActionEventArgs"/> instance containing the event data.</param>
        void SelectedGalleryList_ListAction(object sender, ComponentActionEventArgs e)
        {
            if (ButtonGridSave)
            {
                int count = 0;
                if (wim.ChangedSearchGridItem != null)
                {
                    foreach (var item in wim.ChangedSearchGridItem)
                    {
                        var asset1 = item as Sushi.Mediakiwi.Data.Asset;
                        if (asset1 != null)
                        {
                            count++;
                            asset1.Save();
                        }
                    }
                }
                if (wim.Notification.GenericInformation == null || wim.Notification.GenericInformation.Count == 0)
                    wim.Notification.AddNotification(string.Format("{0} Asset(s) have changed and are saved.", count));
                else
                    wim.Notification.AddNotification(string.Format("<br/>{0} Asset(s) have changed and are saved.", count));
                return;
            }

            Sushi.Mediakiwi.Data.Asset asset = Sushi.Mediakiwi.Data.Asset.SelectOne(e.SelectedKey);
            if (FilterAddFile)
            {
                Response.Redirect(wim.GetCurrentQueryUrl(true,
                    new KeyValue() { Key = "Upload", Value = 1 }
                    )
                );
            }
            else if (ButtonRemove || ButtonRemove2)
            {
                if (IsGridView)
                {
                    int galleryID = Wim.Utility.ConvertToInt(Request.QueryString[m_GALLERYKEY], BaseGalleryID);
                    if (galleryID == 0) galleryID = BaseGalleryID;

                    //  Register included
                    foreach (Sushi.Mediakiwi.Data.Asset item in Sushi.Mediakiwi.Data.Asset.SelectAll(galleryID, null, true, false))
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
            else if (ButtonMulti)
            {
                wim.CurrentVisitor.Data.Apply("wim.showmulti", true);
                wim.CurrentVisitor.Save();
                Response.Redirect(Request.Url.ToString());
            }
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
            else if (ButtonSingle)
            {
                wim.CurrentVisitor.Data.Apply("wim.showmulti", null);
                wim.CurrentVisitor.Save();
                Response.Redirect(Request.Url.ToString());
            }
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
        /// Gets a value indicating whether [show single asset and not in cloud].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [show single asset and not in cloud]; otherwise, <c>false</c>.
        /// </value>
        public bool ShowSingleAssetAndNotInCloud
        {
            get
            {
                if (Sushi.Mediakiwi.Data.Asset.HasCloudSetting)
                    return false;
                return ShowSingleAsset;
            }
        }

        /// <summary>
        /// Gets a value indicating whether [show item instance].
        /// </summary>
        /// <value><c>true</c> if [show item instance]; otherwise, <c>false</c>.</value>
        public bool ShowItemInstance { get { return ShowSingleAsset || IsUploadView; } }

        public bool ShowBackToGallery { get; set; }
        public bool ShowRemove { get; set; }
        public bool ShowRemove2 { get; set; }
        public bool ShowSwitch { get; set; }
        public bool ShowUpload { get; set; }
        public bool ShowEdit { get; set; }
        public bool ShowEditReset { get; set; }

        public bool ShowFalse { get; set; }

        public string CurrentGalleryGUID { get; set; }

        void SelectedGalleryList_ListLoad(object sender, ComponentListEventArgs e)
        {
            if (this.AssetTypes.Count > 1)
                wim.SetPropertyRequired("AssetTypeID", true);

            ValidateAccessSecurity();

            int type = Wim.Utility.ConvertToInt(Request.QueryString["type"], 1);

            bool shouldShowMulti = wim.CurrentVisitor.Data["wim.showmulti"].ParseBoolean(false);
            bool shouldShowEdit = wim.CurrentVisitor.Data["wim.showedit"].ParseBoolean(false);

            this.IsListEditMode = shouldShowEdit;

            //  Asset view
            if (IsGridView)
            {
                if (!wim.Console.IsNewDesign)
                    ShowSwitch = true;

                if (shouldShowEdit)
                {
                    wim.HasSortOrder = false;
                    
                    ShowSwitch = false;
                    ShowEditReset = true;
                    ShowRemove = true;
                    ShowEdit = false;
                    ShowGridSave = true;
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
                ShowRemove = false;
                ShowBackToGallery = true;
            }

            if (type == 2 || this.ShowButtonMulti)
                this.ListSave += new ComponentListEventHandler(SelectedGalleryList_ListSave);

            Sushi.Mediakiwi.Data.Asset currentAsset;
            Sushi.Mediakiwi.Data.Gallery currentGallery;

            if (IsUploadView && shouldShowMulti)
            {
                //wim.HideSaveButtons = true;
                //m_IsUploadTypeMulti = !wim.Console.IsNewDesign;
                m_IsUploadTypeMulti = true;
            }

      
            //  Set the asset input form visibility based on the state of the form (single or multi)
            ShowSingleAsset = this.ShowButtonMulti;

            if (type == 1)
            {
                int galleryID = Wim.Utility.ConvertToInt(Request.QueryString[m_GALLERYKEY], BaseGalleryID);
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
                ShowRemove2 = true;
            }

            if (currentGallery != null && currentGallery.ID > 0)
                CurrentGalleryGUID = currentGallery.GUID.ToString();

            string handler = "/repository/Handler.ashx";

            wim.Debug(currentGallery.ID);
            wim.Debug(BaseGalleryID);

            int width = 725;
            if (wim.ShowInFullWidthMode)
                width = 936;            

            //wim.SetPropertyVisibility("ButtonMulti", !wim.Console.IsNewDesign);
            if (IsUploadView || ShowRemove2)
            {   
                if(shouldShowMulti)
                {
                    wim.SetPropertyVisibility("Title", false);
                    wim.SetPropertyVisibility("Description", false);
                    //wim.SetPropertyVisibility("AssetTypeID", false);
                    wim.SetPropertyVisibility("File", false);
                }
            }
            else
            {
                wim.SetPropertyVisibility("Title", false);
                wim.SetPropertyVisibility("Description", false);
                wim.SetPropertyVisibility("AssetTypeID", false);
                wim.SetPropertyVisibility("File", false);
            }

            if (this.Implement == null)
                wim.SetPropertyVisibility("RemoteLocation", false);
            else
                wim.SetPropertyVisibility("RemoteLocation", !string.IsNullOrEmpty(this.Implement.RemoteLocation));
        }

        /// <summary>
        /// Gets or sets a value indicating whether [button remove].
        /// </summary>
        /// <value><c>true</c> if [button remove]; otherwise, <c>false</c>.</value>
        [Sushi.Mediakiwi.Framework.OnlyVisibleWhenTrue("ShowRemove")]
        [Sushi.Mediakiwi.Framework.ContentListItem.Button("Remove", false, ButtonIconType.Deny, IconTarget = ButtonTarget.BottomLeft , AskConfirmation = true)]
        public bool ButtonRemove { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [button remove2].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [button remove2]; otherwise, <c>false</c>.
        /// </value>
        [Sushi.Mediakiwi.Framework.OnlyVisibleWhenTrue("ShowRemove2")]
        [Sushi.Mediakiwi.Framework.ContentListItem.Button("Remove", false, ButtonIconType.Deny, IconTarget = ButtonTarget.TopRight, AskConfirmation = true)]
        public bool ButtonRemove2 { get; set; }

        public bool ShowGridSave { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [button grid save].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [button grid save]; otherwise, <c>false</c>.
        /// </value>
        [Sushi.Mediakiwi.Framework.OnlyVisibleWhenTrue("ShowGridSave")]
        [Sushi.Mediakiwi.Framework.ContentListItem.Button("Save", false, ButtonIconType.Approve, IconTarget = ButtonTarget.BottomRight)]
        public bool ButtonGridSave { get; set; }

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
        [Sushi.Mediakiwi.Framework.ContentListItem.Button("Upload document", false, IconType = ButtonIconType.NewItem )]
        public bool FilterAddFile { get; set; }

        [Sushi.Mediakiwi.Framework.OnlyVisibleWhenTrue("ShowEdit")]
        [Sushi.Mediakiwi.Framework.ContentListItem.Button("Edit files", false, IconType = ButtonIconType.Default, IconTarget = ButtonTarget.TopRight)]
        public bool FilterEditFiles { get; set; }


        [Sushi.Mediakiwi.Framework.OnlyVisibleWhenTrue("ShowEditReset")]
        [Sushi.Mediakiwi.Framework.ContentListItem.Button("Undo edit", false, IconType = ButtonIconType.Sorting, IconTarget = ButtonTarget.TopRight)]
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
        [Sushi.Mediakiwi.Framework.OnlyVisibleWhenTrue("ShowButtonMulti")]
        [Sushi.Mediakiwi.Framework.ContentListItem.Button("Multi-file upload", false, IconTarget = Sushi.Mediakiwi.Framework.ButtonTarget.TopRight)]
        public bool ButtonMulti { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [button single].
        /// </summary>
        /// <value><c>true</c> if [button single]; otherwise, <c>false</c>.</value>
        [Sushi.Mediakiwi.Framework.OnlyVisibleWhenTrue("ShowButtonSingle")]
        [Sushi.Mediakiwi.Framework.ContentListItem.Button("Single file upload", false, IconTarget = Sushi.Mediakiwi.Framework.ButtonTarget.TopRight)]
        public bool ButtonSingle { get; set; }

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

        protected bool IsListEditMode = false;

        protected virtual void SelectedGalleryList_ListSearch(object sender, ComponentListSearchEventArgs e)
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

            int scale = 60;

            if (wim.ListDataColumns.List.Count == 0)
            {
                wim.ListDataColumns.Add("ID", "ID", Sushi.Mediakiwi.Framework.ListDataColumnType.UniqueIdentifier);

                if (IsListEditMode)
                {
                    wim.SearchListCanClickThrough = false;
                    wim.ListDataColumns.Add("", "IsIncluded", ListDataColumnType.Checkbox, 20);
                    wim.ListDataColumns.Add(new ListDataColumn("Title", "Title")
                    {
                        ColumnWidth = 270,
                        EditConfiguration = new ListDataEditConfiguration() { Type = ListDataEditConfigurationType.TextField, PropertyToSet = "Title", Width = 250 }
                    });

                    wim.ListDataColumns.Add(new ListDataColumn("Type", "Info2")
                    { 
                        EditConfiguration = new ListDataEditConfiguration() { Type = ListDataEditConfigurationType.Dropdown, PropertyToSet = "AssetTypeID", CollectionProperty = "AssetTypes", Width = 300 } 
                    });

                    //wim.ListDataColumns.Add(new ListDataColumn("Show", "Active")
                    //{
                    //    Alignment = Align.Center,
                    //    ColumnWidth = 40,
                    //    EditConfiguration = new ListDataEditConfiguration() { Type = ListDataEditConfigurationType.Checkbox, PropertyToSet = "IsActive" }
                    //});
                }
                else
                {
                    wim.SetPropertyVisibility("", false);
                    wim.ListDataColumns.Add("", "Icon", scale);
                    wim.ListDataColumns.Add("Title", "Title", Sushi.Mediakiwi.Framework.ListDataColumnType.HighlightPresent);
                    wim.ListDataColumns.Add("Type", "Info2", 250);
                    wim.ListDataColumns.Add("", "Info4", 90, Align.Right);

                    wim.ListDataColumns.Add("", "Info1", 90, Align.Right);
                    wim.ListDataColumns.Add("Modified", "Info3", 90);
                }
            }
            else
            {
                if (!IsListEditMode) 
                    wim.ListDataColumns.Add("", "Icon", 20);
            }


            wim.ListDataColumns.ColumnItemUrl = "TypeUrl";
            int type = Wim.Utility.ConvertToInt(Request.QueryString["type"], 1);

            Sushi.Mediakiwi.Data.Asset currentAsset;
            Sushi.Mediakiwi.Data.Gallery currentGallery;


            if (type == 1)
            {
                int galleryID = Wim.Utility.ConvertToInt(Request.QueryString[m_GALLERYKEY], BaseGalleryID);
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

          
            if (IsListEditMode)
            {
                wim.ListDataColumns.ColumnItemUrl = null;
                var assetArray = Sushi.Mediakiwi.Data.Asset.SelectAll(currentGallery.ID, null, true, false);
                wim.ListDataApply(assetArray);
                return;
            }
            else
            {
                //Product product = Product.SelectOne(e.SelectedGroupItemKey);
                if (currentGallery.ID != BaseGalleryID && currentGallery.ParentID.HasValue)
                {
                    string icon = wim.CurrentApplicationUser.ShowDetailView ?
                              string.Format("<img alt=\"\" src=\"{0}/images/icon_folder_link.png\"/>", wim.Console.WimRepository)
                              : string.Concat(wim.Console.WimRepository, "/images/", "thumb_folder.png");

                    icon = string.Format("<font class=\"icon-folder-down\" style=\"font-size:{0}px; color:#7eb32d\" />", scale);

                    list.Add(new Sushi.Mediakiwi.AppCentre.Data.Implementation.Browsing.BrowseItem()
                     {
                         TypeUrl = wim.GetCurrentQueryUrl(true, new KeyValue() { Key = m_GALLERYKEY, Value = currentGallery.ParentID.Value }, new KeyValue() { Key = "type", Value = 1 }),
                         Title = "...",

                         Icon = icon,
                         EditLink = string.Format("<a href=\"{0}\">Edit</a>", wim.GetCurrentQueryUrl(true, new KeyValue() { Key = m_GALLERYKEY, Value = currentGallery.ParentID.Value }, new KeyValue() { Key = "type", Value = 1 }))
                     });
                }

                foreach (Sushi.Mediakiwi.Data.Gallery gallery in Sushi.Mediakiwi.Data.Gallery.SelectAllByParent(currentGallery.ID, true))
                {
                     string icon = wim.CurrentApplicationUser.ShowDetailView ?
                                string.Format("<img alt=\"\" src=\"{0}/images/icon_folder_link.png\"/>", wim.Console.WimRepository)
                                : string.Concat(wim.Console.WimRepository, "/images/", "thumb_folder.png");

                     icon = string.Format("<font class=\"icon-files\" style=\"font-size:{0}px; color:#7eb32d\" />", scale);

                    list.Add(new Sushi.Mediakiwi.AppCentre.Data.Implementation.Browsing.BrowseItem()
                        {
                            TypeUrl = wim.GetCurrentQueryUrl(true, new KeyValue() { Key = m_GALLERYKEY, Value = gallery.ID }, new KeyValue() { Key = "type", Value = 1 }),

                            Title = gallery.Name,
                            Icon = icon,
                            Info1 = gallery.AssetCount.ToString(),
                            Info3 = gallery.Created,
                            EditLink = string.Format("<a href=\"{0}\">Edit</a>", wim.GetCurrentQueryUrl(true, new KeyValue() { Key = m_GALLERYKEY, Value = gallery.ID }, new KeyValue() { Key = "type", Value = 1 }))
                        }
                    );
                }


                foreach (Sushi.Mediakiwi.Data.Asset asset in Sushi.Mediakiwi.Data.Asset.SelectAll(currentGallery.ID, null, true, false))
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

                        if (asset.IsImage)
                        {
                            if (Sushi.Mediakiwi.Data.Asset.HasCloudSetting || asset.Exists || asset.IsRemote)
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

                    if (wim.Console.IsNewDesign)
                    {
                        icon = asset.ImageScaledUrl(scale, scale);
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
                                : null,
                            DownloadLink = string.Format("<a href=\"{0}\" target=\"_blank\">Open file</a>", asset.DownloadFullUrlByGUID),
                            EditLink = string.Format("<a href=\"{0}\">Edit</a>", wim.GetCurrentQueryUrl(true,
                                new KeyValue() { Key = "item", Value = asset.ID },
                                new KeyValue() { Key = "type", Value = 2 }
                                ))

                        }
                    );
                }
            }

           
            wim.ListDataApply(list);
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
            get {
                return IsUploadView && !m_IsUploadTypeMulti; }
        }
        public bool ShowButtonSingle
        {
            get {
                return IsUploadView && m_IsUploadTypeMulti; }
        }
        public bool ShowMultiUpload
        {
            get {
                return IsUploadView && !ShowButtonMulti; 
            }
        }

        [Sushi.Mediakiwi.Framework.OnlyVisibleWhenTrue(nameof(ShowMultiUpload))]
        [Sushi.Mediakiwi.Framework.ContentListItem.TextLine("&nbsp;")]
        public string MultiFileInfo { get; set; } = "After uploading you can use 'Save' to set the desired type on all assets";

        [Sushi.Mediakiwi.Framework.OnlyVisibleWhenTrue(nameof(ShowMultiUpload))]
        [Sushi.Mediakiwi.Framework.ContentListItem.MultiAssetUpload("Files", "CurrentGalleryGUID", false)]
        public int[] MultiFile { get; set; }

        [Sushi.Mediakiwi.Framework.ContentListItem.DataList(HasThumbnailOption = true )]
        public Sushi.Mediakiwi.Data.DataList SearchGrid { get; set; }
    }
}
