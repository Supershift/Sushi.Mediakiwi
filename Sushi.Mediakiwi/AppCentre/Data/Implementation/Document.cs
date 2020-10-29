using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Threading;
using Sushi.Mediakiwi.AppCentre.Data.Supporting;
using System.Text;
using Sushi.Mediakiwi.Data;

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
                        _files = new List<uploadResultFile>();

                    return _files;
                }
                set { _files = value; }
            }

        }

        // Handle Context
        /// <summary>
        /// Processes the data.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public void ProcessData(HttpContext context)
        {
            context.Response.Clear();
            context.Response.ContentType = "application/json";
            context.Response.ContentEncoding = System.Text.Encoding.UTF8;
            context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            context.Response.Write(Wim.Utilities.JSON.Instance.ToJSON(Parse(context)));
            //, false, true, true, true));
            context.Response.Flush();
            context.Response.End();
        }

        object Parse(HttpContext context)
        {
            uploadResult result = new uploadResult();

            // only do something when nonempty
            if (context.Request.Files.Count > 0)
            {
                // Assign Gallery ID
                var galleryID = Wim.Utility.ConvertToInt(context.Request.Form["gallery"], Sushi.Mediakiwi.Data.Gallery.SelectOneRoot().ID);

                // get all files from request
                HttpFileCollection files = context.Request.Files;
                Sushi.Mediakiwi.Data.Gallery gallery = Sushi.Mediakiwi.Data.Gallery.SelectOne(galleryID);

                // Loop through files to upload
                for (int i = 0; i < files.Count; i++)
                {
                    HttpPostedFile file = files[i];
                    Sushi.Mediakiwi.Data.Asset asset = new Sushi.Mediakiwi.Data.Asset();
                    asset.IsActive = false;
                    asset.SaveStream(file, gallery);

                    // JSON row for returning
                    uploadResultFile fileRow = new uploadResultFile();

                    fileRow.ID = asset.ID;

                    // Set size
                    fileRow.Length = file.ContentLength;

                    // Set Name
                    fileRow.Name = file.FileName;

                    // Set type
                    fileRow.Type = file.ContentType;

                    // Set Error or URL
                    fileRow.Thumbnail_url = asset.ThumbnailPath;

                    // Add uploaded file result to result
                    result.files.Add(fileRow);
                }
            }
            return result;
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
        /// Initializes a new instance of the <see cref="Document"/> class.
        /// </summary>
        public Document()
        {
            wim.HideOpenCloseToggle = true;
            wim.OpenInEditMode = true;

            this.ListAction += new Sushi.Mediakiwi.Framework.ComponentActionEventHandler(Document_ListAction);
            this.ListPreRender += new Sushi.Mediakiwi.Framework.ComponentListEventHandler(Document_ListPreRender);
            this.ListLoad += new Sushi.Mediakiwi.Framework.ComponentListEventHandler(Document_ListLoad);
            this.ListSave += new Sushi.Mediakiwi.Framework.ComponentListEventHandler(Document_ListSave);
            this.ListDelete += new Sushi.Mediakiwi.Framework.ComponentListEventHandler(Document_ListDelete);
            this.ListPreRender += new Sushi.Mediakiwi.Framework.ComponentListEventHandler(Document_ListPreRender);
            this.ListSearch += Document_ListSearch;
        }

      

        void Document_ListAction(object sender, Sushi.Mediakiwi.Framework.ComponentActionEventArgs e)
        {
            if (ButtonBack)
            {
                string url = wim.GetCurrentQueryUrl(true, new Framework.KeyValue() { Key = "redo", Value = "1" });
                Response.Redirect(url, true);
            }

            if (ButtonMulti)
                wim.CurrentVisitor.Data.Apply("wim.showmulti", true);

            if (ButtonSingle)
                wim.CurrentVisitor.Data.Apply("wim.showmulti", null);
            
            wim.CurrentVisitor.Save();
            Response.Redirect(Request.Url.ToString());
        }

        /// <summary>
        /// Handles the ListPreRender event of the Document control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Sushi.Mediakiwi.Framework.ComponentListEventArgs"/> instance containing the event data.</param>
        void Document_ListPreRender(object sender, Sushi.Mediakiwi.Framework.ComponentListEventArgs e)
        {
            if (Request.QueryString["ismulti"] == "1" && wim.Console.IsNewDesign)
            {
                return;
            }

            if (string.IsNullOrEmpty(Implement.Title))
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

            if (IsPostBack && e.SelectedKey == 0 && (m_File == null || m_File.ContentLength == 0))
                wim.Notification.AddError("Please select a file");
        }

        private Sushi.Mediakiwi.AppCentre.Data.Supporting.Section m_environment;
        /// <summary>
        /// Handles the ListDelete event of the Document control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Sushi.Mediakiwi.Framework.ComponentListEventArgs"/> instance containing the event data.</param>
        void Document_ListDelete(object sender, Sushi.Mediakiwi.Framework.ComponentListEventArgs e)
        {
            if (m_Implement == null)
                m_Implement = Sushi.Mediakiwi.Data.Document.SelectOne(e.SelectedKey);

            m_Implement.CloudDelete();

            if (m_Implement.IsImage)
                m_Implement.ImageInstance.Delete();
            else
                m_Implement.Delete();

            if (wim.Console.IsNewDesign)
            {
                if (wim.IsLayerMode)
                {
                    int rootAsset = Wim.Utility.ConvertToInt(Request.QueryString["base"]);
                    string redirect = wim.GetCurrentQueryUrl(true
                    , new Framework.KeyValue() { Key = "item", Value = rootAsset }
                    , new Framework.KeyValue() { Key = "base", RemoveKey = true }
                    , new Framework.KeyValue() { Key = "sat", RemoveKey = true }
                    );
                    Response.Redirect(redirect);

                    wim.OnSaveScript = string.Format(@"<input type=""hidden"" class=""postparent"" id=""{0}"" value=""{1} ({2} KB)"" />", m_Implement.ID, m_Implement.Title, m_Implement.Size > 0 ? (m_Implement.Size / 1024) : 0);
                }
                else
                {
                    string redirect = wim.GetCurrentQueryUrl(true
                      , new Framework.KeyValue() { Key = "gallery", Value = m_Implement.GalleryID }
                      , new Framework.KeyValue() { Key = "asset", RemoveKey = true }
                      , new Framework.KeyValue() { Key = "item", RemoveKey = true }
                      );
                    Response.Redirect(redirect);
                }
            }
            else
                wim.OnDeleteScript = "<script type=\"text/javascript\">ImageDialog.insert('');</script>";
        }

        /// <summary>
        /// Handles the ListSave event of the Document control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Sushi.Mediakiwi.Framework.ComponentListEventArgs"/> instance containing the event data.</param>
        void Document_ListSave(object sender, Sushi.Mediakiwi.Framework.ComponentListEventArgs e)
        {
            if (m_Implement == null)
                m_Implement = Sushi.Mediakiwi.Data.Document.SelectOne(e.SelectedKey);
            
            if (m_Implement.GalleryID > 0 && m_Implement.GalleryID != this.AssignedGalleryID)
            {
                if (m_Implement.MoveFile(AssignedGalleryID))
                    m_Implement.GalleryID = this.AssignedGalleryID;

                Wim.Utilities.CacheItemManager.FlushIndexOfCacheObjects("Data_Sushi.Mediakiwi.Data.Asset");
                Wim.Utilities.CacheItemManager.FlushIndexOfCacheObjects("Data_Sushi.Mediakiwi.Data.Gallery");
            }
            if (m_Implement.GalleryID == 0)
            {
                //  [20140724:MM] Set DB mapping
                m_Implement.DatabaseMappingPortal = wim.CurrentFolder.DatabaseMappingPortal;
            }

            bool hasUpload = m_File != null && m_File.ContentLength > 0;

            //  NEW DESIGN
            if (!hasUpload && e.SelectedKey == 0)
                return;

            if (hasUpload && !m_Implement.IsNewInstance)
            {
                // Delete existing asset!
                // MarkR (20120807) : First check file existence.
                if (System.IO.File.Exists(m_Implement.LocalFilePath))
                    System.IO.File.Delete(m_Implement.LocalFilePath);
                
                m_Implement.CloudDelete();
            }

            if (hasUpload)
            {
                bool doBlobInsert = false;
                m_Implement = Create(m_Implement, doBlobInsert);
                if (m_Implement.IsNewInstance)
                    return;

                long kb = m_Implement.Size / 1024;

                int? assetTypeID = Wim.Utility.ConvertToIntNullable(Request.QueryString["sat"]);
                if (assetTypeID.HasValue)
                    m_Implement.AssetTypeID = assetTypeID.Value;
            }

            int? parent =  Wim.Utility.ConvertToIntNullable(Request.QueryString["base"]);
            if (parent.HasValue)
                m_Implement.ParentID = parent;

            Sushi.Mediakiwi.Data.Asset currentAsset = m_Implement;
            if (m_Implement.ParentID.HasValue)
            {
                if (wim.CurrentFolder.DatabaseMappingPortal != null)
                    currentAsset = Sushi.Mediakiwi.Data.Asset.SelectOneByPortal(m_Implement.ParentID.Value, wim.CurrentFolder.DatabaseMappingPortal.Name);
                else
                    currentAsset = Sushi.Mediakiwi.Data.Asset.SelectOne(m_Implement.ParentID.Value);

                m_Implement.GalleryID = currentAsset.GalleryID;
            }

            m_Implement.Save();

            if (e.SelectedKey == 0 || Request.QueryString["redo"] == "1")
            {
                string url;
                if (IsPopupRequest)
                {
                    url = wim.GetCurrentQueryUrl(true
                        , new Sushi.Mediakiwi.Framework.KeyValue() { Key = "asset", Value = m_Implement.ID.ToString() }
                        , new Framework.KeyValue() { Key = "redo", RemoveKey = true }
                        );
                }
                else
                {
                    url = wim.GetCurrentQueryUrl(true
                        , new Sushi.Mediakiwi.Framework.KeyValue() { Key = "item", Value = m_Implement.ID.ToString() }
                        , new Framework.KeyValue() { Key = "redo", RemoveKey = true }
                        , new Framework.KeyValue() { Key = "asset", RemoveKey = true }
                        );
                }

                Response.Redirect(url);
            }
            else
            {
                string info = null;
                if (currentAsset.IsImage)
                    info = string.Format("{0} <span>({1}px / {2}px)</span>", currentAsset.Title, currentAsset.Width, currentAsset.Height);
                else
                    info = string.Format("{0} <span>({1} KB)</span>", currentAsset.Title, currentAsset.Size > 0 ? (currentAsset.Size / 1024) : 0);

                if (string.IsNullOrEmpty(Request.QueryString["referid"]))
                    wim.Page.Body.Form.RefreshParent();
                else
                    wim.Page.Body.Form.PostDataToSubSelect(currentAsset.ID, info);
            }

        }

        /// <summary>
        /// Sets the push thread.
        /// </summary>
        /// <param name="document">The document.</param>
        private void SetPushThread(Sushi.Mediakiwi.Data.Document document)
        {
            //List<string> urlList = new List<string>();
            //foreach (Sushi.Mediakiwi.Data.Site site in Sushi.Mediakiwi.Data.Site.SelectAll())
            //{
            //    if (site.StagingEnvironment != null)
            //    {
            //        string url = string.Format("{0}/{1}/push.aspx?s={2}&i={3}", site.StagingEnvironment, Sushi.Mediakiwi.Framework.Interfaces.BinaryPush.DocumentRecognition, site.Id, document.Id);
            //        urlList.Add(url);
            //    }
            //    if (site.ProductionEnvironment != null)
            //    {
            //        string url = string.Format("{0}/{1}/push.aspx?s={2}&i={3}", site.ProductionEnvironment, Sushi.Mediakiwi.Framework.Interfaces.BinaryPush.DocumentRecognition, site.Id, document.Id);
            //        urlList.Add(url);
            //    }
            //}

            //if (urlList.Count == 0)
            //    return;

            //Implementations.wim_Documents.url = urlList.ToArray();
            //Thread tr = new Thread(new ThreadStart(Implementations.wim_Documents.SetScrapeThread));
            //tr.Start();
        }

        /// <summary>
        /// Sets the scrape thread.
        /// </summary>
        private static void SetScrapeThread()
        {
            //int current = 0;
            //foreach (string uri in Implementations.wim_Documents.url)
            //{
            //    current++;
            //    string uriToCall = uri;
            //    try
            //    {
            //        string result = Wim.Utilities.WebScrape.GetUrlResponse(uriToCall);
            //    }
            //    catch (Exception ex)
            //    {
            //        System.Exception except = ex;
            //        string exceptionMessage = "<b>Exception occured when call page</b><br/>page: " + uriToCall;
            //        while (except != null)
            //        {
            //            exceptionMessage += string.Format("<br/>- {0}", except.Message);
            //            except = except.InnerException;
            //        }
            //        Sushi.Mediakiwi.Data.Notification.InsertOne(Sushi.Mediakiwi.Data.Notification.Tags.InternalWimError, null, exceptionMessage);
            //    }
            //}
        }

        /// <summary>
        /// Creates the specified document.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="doBlobInsert">if set to <c>true</c> [do BLOB insert].</param>
        /// <returns></returns>
        public Sushi.Mediakiwi.Data.Asset Create(Sushi.Mediakiwi.Data.Asset document, bool doBlobInsert)
        {
            int galleryId;
            Guid guid;
            Sushi.Mediakiwi.Data.Gallery gallery = Sushi.Mediakiwi.Data.Gallery.Identify(Request.QueryString["gallery"]);
            document.GalleryID = gallery.ID;
            document.DatabaseMappingPortal = gallery.DatabaseMappingPortal;

            if (File != null && File.ContentLength > 0)
            {
                document.Width = 0;
                document.Height = 0;
            }

            if (!AssetLogic.CloudUpload(document, gallery, File))
                document.SaveStream(m_File, gallery);

            return document;
        }

        

        [Sushi.Mediakiwi.Framework.OnlyVisibleWhenTrue("ShowButtonMulti")]
        [Sushi.Mediakiwi.Framework.ContentListItem.Choice_Dropdown("Gallery", "GalleryCollection", true, true)]
        public int AssignedGalleryID2 { get; set; }

        /// <summary>
        /// Gets or sets the multi file.
        /// </summary>
        /// <value>The multi file.</value>
        [Sushi.Mediakiwi.Framework.OnlyVisibleWhenTrue("ShowButtonMulti")]
        [Sushi.Mediakiwi.Framework.ContentListItem.HtmlContainer(true)]
        public string MultiFile { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [button multi].
        /// </summary>
        /// <value><c>true</c> if [button multi]; otherwise, <c>false</c>.</value>
        [Sushi.Mediakiwi.Framework.OnlyVisibleWhenTrue("ShowButtonMulti", false)]
        [Sushi.Mediakiwi.Framework.ContentListItem.Button("Multi-file upload", false)]
        public bool ButtonMulti { get; set; }

        public string ButtonBack2URL { get; set; }
        [Sushi.Mediakiwi.Framework.ContentListItem.Button("", false, ButtonClassName = "flaticon icon-home", IconTarget = Sushi.Mediakiwi.Framework.ButtonTarget.TopRight
            , CustomUrlProperty = "ButtonBack2URL")]
        public bool ButtonBack2 { get; set; }


        [Sushi.Mediakiwi.Framework.ContentListItem.Button("", false, ButtonClassName = "flaticon icon-rotate", IconTarget = Sushi.Mediakiwi.Framework.ButtonTarget.TopRight, InteractiveHelp = "Redo upload")]
        public bool ButtonBack { get; set; }

        public string AlternativeURL { get; set; }
        [Sushi.Mediakiwi.Framework.ContentListItem.Button("", false, ButtonClassName = "flaticon icon-platforms", IconTarget = Sushi.Mediakiwi.Framework.ButtonTarget.TopRight, InteractiveHelp = "Add/update alternative image"
            , CustomUrlProperty = "AlternativeURL")]
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
        /// Gets a value indicating whether [show single asset and not in cloud].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [show single asset and not in cloud]; otherwise, <c>false</c>.
        /// </value>
        public bool ShowSingleAssetAndNotInCloud { 
            get {
                if (Sushi.Mediakiwi.Data.Asset.HasCloudSetting)
                    return false;
                return ShowSingleAsset;
            }
        }

        void ParseStream()
        {

        }

        /// <summary>
        /// Gets or sets a value indicating whether [button single].
        /// </summary>
        /// <value><c>true</c> if [button single]; otherwise, <c>false</c>.</value>
        [Sushi.Mediakiwi.Framework.OnlyVisibleWhenTrue("ShowButtonMulti")]
        [Sushi.Mediakiwi.Framework.ContentListItem.Button("Single file upload", false)]
        public bool ButtonSingle { get; set; }

        void LoadMultiUpload()
        {

            if (
                wim.Console.Request.Form.Count > 0 &&
                wim.Console.Request.Files.Count > 0 && 
                wim.Console.Request.Files[0].ContentLength > 0)
            {
                MultiUpload parser = new MultiUpload();
                parser.ProcessData(wim.Console.Context);
                //  Do stream conversion
            }

            wim.HideSaveButtons = true;
            wim.SetPropertyVisibility("ButtonMulti", false);
            wim.SetPropertyVisibility("ButtonBack", false);
            wim.SetPropertyVisibility("File", false);
            wim.SetPropertyVisibility("RemoteLocation", false);
            wim.SetPropertyVisibility("RemoteDownload", false);
            wim.SetPropertyVisibility("Title", false);
            wim.SetPropertyVisibility("Description", false);
            wim.SetPropertyVisibility("AssetTypeID", false);
            wim.SetPropertyVisibility("AssignedGalleryID2", false);
            wim.SetPropertyVisibility("AssignedGalleryID", false);
            wim.SetPropertyVisibility("FileSize", false);

            string collection = Request.QueryString["col"];
            if (!string.IsNullOrEmpty(collection))
            {
                var range = Wim.Utility.ConvertToIntArray(collection.Split(','));


                var asset = Sushi.Mediakiwi.Data.Asset.SelectRange(range);

                StringBuilder build = new StringBuilder();
                bool isFormValid = true;
                foreach (var q in asset)
                {
                    if (!string.IsNullOrEmpty(Request["delete"]))
                    {
                        q.Delete();
                    }

                    bool isValid = true;
                    if (IsPostBack)
                    {
                        q.Title = Request.Form[string.Concat("uxTitle_", q.ID)];
                        if (string.IsNullOrEmpty(q.Title))
                            isValid = false;
                        q.Description = Request.Form[string.Concat("uxText_", q.ID)];

                        if (isValid)
                        {
                            q.IsActive = true;
                            q.Save();
                        }
                        else
                            isFormValid = false;
                    }

                    build.AppendFormat(@"
			<div class=""imageEdit"">
				<figure>
					<img src=""{2}"" style=""width: 216px; height: 207px"" />	
				</figure>
				<input class=""half" + (isValid ? string.Empty : " error") + @""" type=""text"" id=""uxTitle_{3}"" name=""uxTitle_{3}"" required=""required"" placeholder=""Titel"" value=""{0}"" />
				<div class=""half"">
					<textarea class=""nicEditor half"" id=""uxText_{3}"" name=""uxText_{3}"" cols=""10"" rows=""8"">{1}</textarea>
				</div>
				<br class=""clear"" />
			</div>
			<hr />
", q.Title, q.Description, (q.IsImage ? q.ImageInstance.ApplyForcedBorder(null, 216, 207, new int[] { 236, 236, 236 }, true, Sushi.Mediakiwi.Data.ImagePosition.TopCenter, null, true, false) : string.Empty), q.ID);
                
                }

                if (IsPostBack && isFormValid)
                {
                    
                    build.Append(@"<input type=""hidden"" name=""close"" class=""postParent"">");
                    //  Close window
                }

                wim.Page.Body.Add(@"
		<form method=""POST"" enctype=""multipart/form-data"" action=""#"">
        <input type=""hidden"" id=""state"" name=""state"" value=""0"">
        <header>
			<p>
				Wijzig hieronder de omschrijving van de bestanden. Pas na het opslaan van de afbeelding(en) worden deze actief.
			</p>
			<hr />
		</header>
		<article>"
            + build.ToString() + @"
            <span class=""clear""></span>
            <div>"
            + (range.Length == 1 ? @"<input id=""delete"" name=""delete"" class=""postBack submit left"" type=""submit"" value=""Verwijder"">" : string.Empty) + @"
                            <input name=""save"" class=""reset right"" type=""submit"" value=""Opslaan"">
            </div>
		</article>
        </form>
", true);

                return;
            }

//            wim.Page.Head.Add(@"
//        <script src=""http://blueimp.github.io/JavaScript-Templates/js/tmpl.min.js""></script>
//        <script src=""http://blueimp.github.io/JavaScript-Load-Image/js/load-image.min.js""></script>
//        <script src=""http://blueimp.github.io/JavaScript-Canvas-to-Blob/js/canvas-to-blob.min.js""></script>
//        <script src=""http://netdna.bootstrapcdn.com/bootstrap/3.0.0/js/bootstrap.min.js""></script>
//        <!-- blueimp Gallery script -->
//        <script src=""http://blueimp.github.io/Gallery/js/jquery.blueimp-gallery.min.js""></script>
//        <!-- The Iframe Transport is required for browsers without support for XHR file uploads -->
//        <script src=""http://blueimp.github.io/jQuery-File-Upload/js/jquery.iframe-transport.js""></script>
//        <!-- The basic File Upload plugin -->
//        <script src=""http://blueimp.github.io/jQuery-File-Upload/js/jquery.fileupload.js""></script>
//        <!-- The File Upload processing plugin -->
//        <script src=""http://blueimp.github.io/jQuery-File-Upload/js/jquery.fileupload-process.js""></script>
//        <!-- The File Upload image preview & resize plugin -->
//        <script src=""http://blueimp.github.io/jQuery-File-Upload/js/jquery.fileupload-image.js""></script>
//        <!-- The File Upload audio preview plugin -->
//        <script src=""http://blueimp.github.io/jQuery-File-Upload/js/jquery.fileupload-audio.js""></script>
//        <!-- The File Upload video preview plugin -->
//        <script src=""http://blueimp.github.io/jQuery-File-Upload/js/jquery.fileupload-video.js""></script>
//        <!-- The File Upload validation plugin -->
//        <script src=""http://blueimp.github.io/jQuery-File-Upload/js/jquery.fileupload-validate.js""></script>
//        <!-- The File Upload user interface plugin -->
//        <script src=""http://blueimp.github.io/jQuery-File-Upload/js/jquery.fileupload-ui.js""></script>
//        <!-- The main application script -->
//        <script src=""http://blueimp.github.io/jQuery-File-Upload/js/main.js""></script>
//		<style>
//			html, body {
//				overflow: auto;
//			}
//		</style>
//");
            wim.Page.Head.Add(@"
		<link rel=""stylesheet"" href=""testdrive/files/jquery.fileupload.css"">
		<script type=""text/javascript"" src=""testdrive/files/head.load.min.js""></script>
		<script type=""text/javascript"" src=""testdrive/files/preview.js""></script>
		<style>
			html, body {
				overflow: auto;
			}
		</style>");

            wim.Page.Body.Clear(true);
            wim.Page.Body.Add(@"
        <div id=""fileupload"" >
            <form method=""POST"" enctype=""multipart/form-data"" action=""#"">
		        <input type=""hidden"" id=""gallery"" name=""gallery"" value=""" + Request.QueryString["gallery"] + @""">
		        <input type=""hidden"" id=""state"" name=""state"" value=""0"">
		        <input type=""hidden"" id=""assets"" name=""assets"" value="""">
                <section id=""popupContent"">
                    <div class=""fileupload-buttonbar"">
			             <!-- The fileinput-button span is used to style the file input field as button -->
			            <span class=""fileinput-button"">
				            <i class=""glyphicon glyphicon-plus""></i>
				            <span>Add files...</span>
				            <!-- The file input field used as target for the file upload widget -->
				            <input id=""file"" type=""file"" name=""files[]"" multiple />
			            </span>
			            <button class=""start"" type=""submit"">
				            <i class=""glyphicon glyphicon-upload""></i>
				            <span>Start upload</span>
			            </button>
                    </div>
		        </section>
            </form>
            <div class=""fileupload-content"">
                <table class=""files""></table>
                <div class=""fileupload-progressbar""></div>
            </div>
        </div>
        <script id=""template-upload"" type=""text/x-jquery-tmpl"">
            <tr class=""template-upload{{if error}} ui-state-error{{/if}}"">
                <td class=""preview""></td>
                <td class=""name"">${name}</td>
                <td class=""size"">${sizef}</td>
                {{if error}}
                    <td class=""error"" colspan=""2"">Error:
                        {{if error === 'maxFileSize'}}File is too big
                        {{else error === 'minFileSize'}}File is too small
                        {{else error === 'acceptFileTypes'}}Filetype not allowed
                        {{else error === 'maxNumberOfFiles'}}Max number of files exceeded
                        {{else}}${error}
                        {{/if}}
                    </td>
                {{else}}
                    <td class=""start""><button>Start</button></td>
                {{/if}}
                <td class=""cancel""><button class=""cancel"">Cancel</button></td>
            </tr>
        </script>
        <script id=""template-download"" type=""text/x-jquery-tmpl"">" + 
            //<tr class=""template-download{{if error}} ui-state-error{{/if}}"">
            //    {{if error}}
            //        <td></td>
            //        <td class=""name"">${namefdsa}</td>
            //        <td class=""size"">${sizef}</td>
            //        <td class=""error"" colspan=""3"">Error:
            //            {{if error === 1}}File exceeds upload_max_filesize
            //            {{else error === 2}}File exceeds MAX_FILE_SIZE (HTML form directive)
            //            {{else error === 3}}File was only partially uploaded
            //            {{else error === 4}}No File was uploaded
            //            {{else error === 5}}Missing a temporary folder
            //            {{else error === 6}}Failed to write file to disk
            //            {{else error === 7}}File upload stopped by extension
            //            {{else error === 'maxFileSize'}}File is too big
            //            {{else error === 'minFileSize'}}File is too small
            //            {{else error === 'acceptFileTypes'}}Filetype not allowed
            //            {{else error === 'maxNumberOfFiles'}}Max number of files exceeded
            //            {{else error === 'uploadedBytes'}}Uploaded bytes exceed file size
            //            {{else error === 'emptyResult'}}Empty file upload result
            //            {{else}}${error}
            //            {{/if}}
            //        </td>
            //    {{/if}}
            //</tr>

        @"</script>
        <script type=""text/javascript"" src=""testdrive/files/jquery-1.7.1.js""></script>
		<script type=""text/javascript"" src=""testdrive/files/jquery-ui-1.8.17.custom.min.js""></script>
        <script src=""//ajax.aspnetcdn.com/ajax/jquery.templates/beta1/jquery.tmpl.min.js""></script>
	    <script src=""testdrive/files/jquery.iframe-transport.js""></script>
	    <script src=""testdrive/files/jquery.fileupload.js""></script>
        <script src=""testdrive/files/jquery.fileupload-ui.js""></script>
        <script src=""testdrive/files/jquery.fileupload.init.js""></script>
        <div id=""loader""></div>
");

        }


        [Sushi.Mediakiwi.Framework.ContentListItem.DataList()]
        public Sushi.Mediakiwi.Data.DataList AssetTypeSelectionList { get; set; }

        public class AssetTypeMapper
        {
            public AssetTypeMapped[] Map(IAssetType[] types, Asset[] assets, string url)
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
                            candidate.DisplayName = string.Format("{0} ({1}px / {2}px)", match.Title, match.Width, match.Height);
                        else
                            candidate.DisplayName = match.Title;
                        key = match.ID;
                    }
                    candidate.PassthroughParameter = string.Concat(url, "&item=", key, "&sat=", type.ID); // = KEY
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

        void Document_ListSearch(object sender, Framework.ComponentListSearchEventArgs e)
        {
            int rootAsset = Wim.Utility.ConvertToInt(Request.QueryString["base"]);

            var url = wim.GetCurrentQueryUrl(false
                , new Framework.KeyValue() { Key = "item", RemoveKey = true }
                );
            var types = Sushi.Mediakiwi.Data.AssetType.SelectAll(true);
            var assets = Sushi.Mediakiwi.Data.Asset.SelectAll_Variant(rootAsset, Request.QueryString["gallery"]);
            AssetTypeMapper mapper = new AssetTypeMapper();
            var mapped = mapper.Map(types, assets, url);

            wim.ListDataColumns.Add(new Framework.ListDataColumn("ID", "AssetID", Framework.ListDataColumnType.UniqueIdentifier));
            wim.ListDataColumns.Add(new Framework.ListDataColumn("Type", "AssetType", Framework.ListDataColumnType.HighlightPresent));
            wim.ListDataColumns.Add(new Framework.ListDataColumn("File", "DisplayName"));
            wim.ListDataColumns.Add(new Framework.ListDataColumn("", "HasAsset") { ColumnWidth = 30 });
            wim.ListDataApply(mapped);
            wim.SearchResultItemPassthroughParameterProperty = "PassthroughParameter";
            wim.Page.Body.Grid.IgnoreInLayerSubSelect = true;
        }

        Sushi.Mediakiwi.Data.Asset m_Implement;
        /// <summary>
        /// Handles the ListLoad event of the Document control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Sushi.Mediakiwi.Framework.ComponentListEventArgs"/> instance containing the event data.</param>
        void Document_ListLoad(object sender, Sushi.Mediakiwi.Framework.ComponentListEventArgs e)
        {
            this.AssetTypeSelectionList = new Sushi.Mediakiwi.Data.DataList();

            int rootAsset = Wim.Utility.ConvertToInt(Request.QueryString["base"], e.SelectedKey);
            bool isAlternativeImage = !string.IsNullOrEmpty(Request.QueryString["base"]);
            var alternativeTypeID = Wim.Utility.ConvertToIntNullable(Request.QueryString["sat"]);

            wim.SetPropertyVisibility("ButtonAlternative", false);
            wim.SetPropertyVisibility("AssetTypeSelectionList", false);
            wim.SetPropertyVisibility("ButtonBack2", false);
            wim.SetPropertyVisibility("AssetTypeID", false);
            ShowButtonMulti = false;
            wim.SetPropertyVisibility("ButtonMulti", false);
               
            if (e.SelectedKey > 0)
            {
                var variantTypes = Sushi.Mediakiwi.Data.AssetType.SelectAll(true);
                if (variantTypes.Length > 0)
                    wim.SetPropertyVisibility("ButtonAlternative", true);
            }

            ButtonBack2URL = wim.GetCurrentQueryUrl(true
                    , new Framework.KeyValue() { Key = "base", RemoveKey = true }
                    , new Framework.KeyValue() { Key = "item", Value = rootAsset }
                    , new Framework.KeyValue() { Key = "sat", RemoveKey = true }
                    , new Framework.KeyValue() { Key = "redo", RemoveKey = true }
                    );

            if (rootAsset > 0 && isAlternativeImage)
            {
        
                wim.SetPropertyVisibility("ButtonBack2", true);
            }

            AlternativeURL = wim.GetCurrentQueryUrl(true
                , new Framework.KeyValue() { Key = "base", Value = rootAsset }
                , new Framework.KeyValue() { Key = "item", Value = 0 }
                , new Framework.KeyValue() { Key = "sat", RemoveKey = true }
                , new Framework.KeyValue() { Key = "redo", RemoveKey = true }
                );

            if (isAlternativeImage && alternativeTypeID.HasValue)
            {
                wim.SetPropertyVisibility("AssetTypeID", true);
                wim.SetPropertyEditable("AssetTypeID", false);
            }

            if (isAlternativeImage && !alternativeTypeID.HasValue)
            {
                   
                wim.SetPropertyVisibility("File", false);
                wim.SetPropertyVisibility("AssetTypeSelectionList", true);
            }
                

            if (Request.QueryString["ismulti"] == "1" && wim.Console.IsNewDesign)
            {
                //LoadMultiUpload();
                return;
            }

            wim.CanSaveAndAddNew = false;

            if (wim.CurrentVisitor.Data["wim.showmulti"].ParseBoolean(false))
            {
                wim.HideSaveButtons = true;

                if (!wim.Console.IsNewDesign)
                    ShowButtonMulti = true;
            }


            wim.SetPropertyVisibility("ButtonBack", false);
           

            //  In popup layer, hide the multi-file-upload.
            if (wim.Console.OpenInFrame > 0)
            {
              
                wim.SetPropertyVisibility("ButtonMulti", false);
            }
            //if (Request.QueryString["referid"] == "tmce")
            //{
            //    IsRTE_Set_Version = true;
            //    wim.OnLoadScript = string.Format("<script type=\"text/javascript\" src=\"{0}\"></script>", Wim.Utility.AddApplicationPath("repository/wim/scripts/rte/tiny_mce_popup.js"));
            //    wim.OnLoadScript += string.Format("<script type=\"text/javascript\" src=\"{0}\"></script>", Wim.Utility.AddApplicationPath("repository/wim/scripts/rte/utils/form_utils.js"));
            //    wim.OnLoadScript += string.Format("<script type=\"text/javascript\" src=\"{0}\"></script>", Wim.Utility.AddApplicationPath("repository/wim/scripts/rte/utils/editable_selects.js"));
            //    wim.OnLoadScript += string.Format("<script type=\"text/javascript\" src=\"{0}\"></script>", Wim.Utility.AddApplicationPath("repository/wim/scripts/rte/plugins/advimage/js/image.js"));
            //    wim.SetPropertyVisibility("RemoteLocation", false);
            //    wim.SetPropertyVisibility("AssetTypeID", false);
            //    wim.SetPropertyVisibility("FileSize", false);
            //    wim.SetPropertyVisibility("Thumbnail", false);
            //}

            if (wim.CurrentList.Option_OpenInEditMode)
            {
                wim.CurrentList.Option_OpenInEditMode = false;
                wim.CurrentList.Save();
            }

            if (wim.CurrentFolder.DatabaseMappingPortal != null)
                Implement = Sushi.Mediakiwi.Data.Asset.SelectOneByPortal(e.SelectedKey, wim.CurrentFolder.DatabaseMappingPortal.Name);
            else
                Implement = Sushi.Mediakiwi.Data.Asset.SelectOne(e.SelectedKey);

            if (wim.Console.IsNewDesign)
            {
                wim.Page.HideMenuBar = true;
                if (e.SelectedKey == 0 || Request.QueryString["redo"] == "1")
                {
                    // Show the home buttons
                    if (Request.QueryString["redo"] == "1")
                        wim.SetPropertyVisibility("ButtonBack2", true);

                    wim.HideSaveButtons = true;
                    wim.SetPropertyVisibility("RemoteLocation", false);
                    wim.SetPropertyVisibility("RemoteDownload", false);
                    wim.SetPropertyVisibility("Title", false);
                    wim.SetPropertyVisibility("Description", false);
                    wim.SetPropertyVisibility("AssignedGalleryID2", false);
                    wim.SetPropertyVisibility("AssignedGalleryID", false);
                    wim.SetPropertyVisibility("FileSize", false);
                }
                else
                {
                    wim.SetPropertyVisibility("ButtonBack", true);
                    wim.SetPropertyVisibility("File", false);
                    wim.SetPropertyVisibility("RemoteLocation", !string.IsNullOrEmpty(this.Implement.RemoteLocation));
                    wim.SetPropertyVisibility("RemoteDownload", false);
                    wim.SetPropertyVisibility("AssignedGalleryID2", false);
                    wim.SetPropertyVisibility("AssignedGalleryID", false);
                    wim.SetPropertyVisibility("FileSize", false);
                }
            }

            IsExisting = (e.SelectedKey > 0);
            if (IsExisting)
            {
                IsExistingImage = Implement.IsImage;
            }
            else
                this.Implement.AssetTypeID = alternativeTypeID;

            IsPopupRequest = !string.IsNullOrEmpty(Request.QueryString["openinframe"]) && !string.IsNullOrEmpty(Request.QueryString["gallery"]);
            
            if (!this.IsPostBack && m_Implement.GalleryID == 0)
            {
                int galleryId;
                Guid guid;
                if (Wim.Utility.IsGuid(Request.QueryString["gallery"], out guid))
                {
                    Sushi.Mediakiwi.Data.Gallery gallery = Sushi.Mediakiwi.Data.Gallery.SelectOne(guid);
                    m_Implement.GalleryID = gallery.ID;
                }
                else if (Wim.Utility.IsNumeric(Request.QueryString["gallery"], out galleryId))
                {
                    Sushi.Mediakiwi.Data.Gallery gallery = Sushi.Mediakiwi.Data.Gallery.SelectOne(galleryId);
                    m_Implement.GalleryID = gallery.ID;
                }
            }
            this.AssignedGalleryID = m_Implement.GalleryID;
            this.AssignedGalleryID2 = Wim.Utility.ConvertToInt(Request.Params["AssignedGalleryID2"], m_Implement.GalleryID);

//            if (ShowButtonMulti)
//            {
//                string handler = "/repository/Handler.ashx";
//                string file = Wim.Utility.AddApplicationPath(string.Format("ClientBin/SilverlightMultiFileUpload.xap?q={0}", Guid.NewGuid().ToString()));
//                MultiFile = string.Concat(@"
//        <object id=""SilverlightPlugin1"" width=""725"" height=""280""
//             data=""data:application/x-silverlight-2,""   
//             type=""application/x-silverlight-2"" >  
//             <param name=""initParams"" value=""MaxFileSizeKB=,MaxUploads=2,FileFilter=,DefaultColor=White,UploadHandlerName=" + handler + @",HttpUploader=True,CustomParam=" + AssignedGalleryID2 + @""" />
//             <param name=""source"" value=""" + file + @"""/>  
//         </object>");
//            }

            m_environment = (Sushi.Mediakiwi.AppCentre.Data.Supporting.Section)Wim.Utility.ConvertToInt(Request.Params["env"]);

            if (e.SelectedKey == 0)
                return;

            // Changed by Mark R on 27-06-2012 12:30
            // was : IsNotPresentOnServer = Implement.Exists;
            // which is conflicting with the name of the property
            IsNotPresentOnServer = !Implement.Exists;
            IsPresentOnServerOrSingleMultifile = !IsNotPresentOnServer;
            // Changed by Mark R on 27-06-2012 11:53
            // On behalf of an issue not being able to move an asset
            // if (ShowButtonMulti)
            //     IsPresentOnServerOrSingleMultifile = false;

            m_Gallery = Implement.GalleryID.ToString();
            m_FileSize = string.Format("{0} kb", Implement.Size / 1024);

            wim.SetPropertyVisibility("Thumbnail", Implement.IsImage);

            m_Thumbnail = string.Format("<img src=\"{0}\" border=\"0\">", Implement.ThumbnailPath);
        }

        #region List attributes

        private HttpPostedFile m_File;
        /// <summary>
        /// Gets or sets the file.
        /// </summary>
        /// <value>The file.</value>
        [Sushi.Mediakiwi.Framework.OnlyVisibleWhenTrue("ShowButtonMulti", false)]
        [Sushi.Mediakiwi.Framework.ContentListItem.FileUpload("_selectfile", false, "", AutoPostBack = true)]
        public HttpPostedFile File
        {
            get { return m_File; }
            set { m_File = value; }
        }

        /// <summary>
        /// Gets or sets the implement.
        /// </summary>
        /// <value>The implement.</value>
        [Sushi.Mediakiwi.Framework.ContentListItem.DataExtend()]
        public Sushi.Mediakiwi.Data.Asset Implement
        {
            get { return m_Implement; }
            set { m_Implement = value; }
        }

        private ListItemCollection m_GalleryCollection;
        /// <summary>
        /// Gets the gallery collection.
        /// </summary>  
        /// <value>The gallery collection.</value>
        public ListItemCollection GalleryCollection
        {
            get
            {
                if (m_GalleryCollection != null)
                    return m_GalleryCollection;

                m_GalleryCollection = new ListItemCollection();

                Sushi.Mediakiwi.Data.Gallery[] galleries = Sushi.Mediakiwi.Data.Gallery.SelectAllAccessible(wim.CurrentApplicationUser);

                foreach (Sushi.Mediakiwi.Data.Gallery gallery in galleries)
                {
                    m_GalleryCollection.Add(new ListItem(gallery.CompletePath, gallery.ID.ToString()));
                }

                return m_GalleryCollection;
            }
        }

        private int m_AssignedGalleryID;
        /// <summary>
        /// Gets or sets the gallery.
        /// </summary>
        /// <value>The gallery.</value>
        [Sushi.Mediakiwi.Framework.OnlyEditableWhenTrue("IsPopupRequest", false)]
        [Sushi.Mediakiwi.Framework.OnlyVisibleWhenTrue("IsPresentOnServerOrSingleMultifile")]
        [Sushi.Mediakiwi.Framework.ContentListItem.Choice_Dropdown("Gallery", "GalleryCollection")]
        public int AssignedGalleryID
        {
            get { return m_AssignedGalleryID; }
            set { m_AssignedGalleryID = value; }
        }

        public bool IsPresentOnServerOrSingleMultifile { get; set; }



        public bool IsPopupRequest { get; set; }

        private string m_Gallery;

        private string m_Thumbnail;
        /// <summary>
        /// Gets or sets the thumbnail.
        /// </summary>
        /// <value>The thumbnail.</value>
        [Sushi.Mediakiwi.Framework.OnlyVisibleWhenTrue("IsExistingImage")]
        //[Sushi.Mediakiwi.Framework.ContentListItem.TextLine("Thumbnail")]
        public string Thumbnail
        {
            get { return m_Thumbnail; }
            set { m_Thumbnail = value; }
        }

        private string m_FileSize;
        /// <summary>
        /// Gets or sets the size of the file.
        /// </summary>
        /// <value>The size of the file.</value>
        [Sushi.Mediakiwi.Framework.OnlyVisibleWhenTrue("IsExisting")]
        [Sushi.Mediakiwi.Framework.ContentListItem.TextLine("_filesize")]
        public string FileSize
        {
            get { return m_FileSize; }
            set { m_FileSize = value; }
        }

        //plugins_advimage_img_sample


        [Sushi.Mediakiwi.Framework.OnlyVisibleWhenTrue("IsRTE_Set_Version")]
        [Sushi.Mediakiwi.Framework.ContentListItem.TextLine("Alignment", Expression = Sushi.Mediakiwi.Framework.OutputExpression.Left)]
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
            set { }
        }

        public bool IsRTE_Set_Version { get; set; }

        [Sushi.Mediakiwi.Framework.OnlyVisibleWhenTrue("IsRTE_Set_Version")]
        [Sushi.Mediakiwi.Framework.ContentListItem.TextLine("Sample", Expression = Sushi.Mediakiwi.Framework.OutputExpression.Right)]
        public string RTE_Sample
        {
            get
            {
                string url = Wim.Utility.AddApplicationPath("repository/wim/scripts/rte/plugins/advimage/img/sample.gif");
                string src = string.Empty;
                if (this.m_Implement != null && !this.m_Implement.IsNewInstance)
                {
                    url = this.m_Implement.ImageInstance.ApplyForcedBorder(null, 45, 45, null, false, Sushi.Mediakiwi.Data.ImagePosition.Center, null, true, false);
                    src = this.m_Implement.ImageInstance.DownloadUrl;
                }

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
                    //, Wim.Utility.AddApplicationPath("repository/wim/scripts/rte/plugins/advimage/img/sample.gif")
                    , url
                    );
            }
            set { }
        }

        //private string m_FileType;
        ///// <summary>
        ///// Gets or sets the type of the file.
        ///// </summary>
        ///// <value>The type of the file.</value>
        //[Sushi.Mediakiwi.Framework.OnlyVisibleWhenTrue("IsExisting")]
        //[Sushi.Mediakiwi.Framework.ContentListItem.TextLine("Type")]
        //public string FileType
        //{
        //    get { return m_FileType; }
        //    set { m_FileType = value; }
        //}

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
            get { return string.IsNullOrEmpty(Request.QueryString["g"]); }
        }

        private string m_Note = "<b>Document is not physically present on server!</b>";
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
            get { return (Request.QueryString["openinframe"] == "1"); }
        }
        #endregion List attributes
    }
}
