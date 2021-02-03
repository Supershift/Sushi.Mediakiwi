using System;
using System.Web;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Web.Hosting;
using System.Diagnostics;
using System.Net;

namespace Sushi.Mediakiwi.Framework
{
    
    /// <summary>
    /// 
    /// </summary>
    public class AssetHandler : IHttpHandler
    {
        void ProcessStatistics(HttpContext context, int assetID)
        {
            var visitor = Sushi.Mediakiwi.Data.Identity.Visitor.Select();
            int logID = visitor.Data["wim_logid"].ParseInt().GetValueOrDefault();

            if (logID > 0)
            {
                Sushi.Mediakiwi.Data.Statistics.VisitorDownload download = new Sushi.Mediakiwi.Data.Statistics.VisitorDownload();
                download.AssetID = assetID;
                download.VisitorLogID = logID;
                download.Save();
            }
        }

        public virtual void ProcessRequest(HttpContext context)
        {
            string query = context.Request.Url.Query.Replace("?", string.Empty);
            ProcessRequest(context, query, null);
        }

        public class ScaledAssetCacheManager
        {
            public static string GetUrl(string path)
            {
                using (Wim.Utilities.CacheItemManager cman = new Utilities.CacheItemManager())
                {
                    if (cman.IsCached(path))
                        return cman.GetItem(path) as string;
                    return path;
                }
            }
            public static void AddUrl(string key, string url)
            {
                using (Wim.Utilities.CacheItemManager cman = new Utilities.CacheItemManager())
                {
                    cman.Add(key, url, DateTime.UtcNow.AddMonths(1));
                }
            }

        }

        /// <summary>
        /// Processes the request.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="query">The query.</param>
        /// <param name="portal">The portal.</param>
        public virtual void ProcessRequest(HttpContext context, string query, string portal)
        {
            if (!string.IsNullOrEmpty(query))
            {
                int fileReference;
                Guid fileReferenceGuid;

                #region Image generation
                if (query.Contains(","))
                {
                    string[] elements = query.Split(',');

                    int? imageID;
                    Guid? imageGUID;

                    Sushi.Mediakiwi.Data.Image image = null;

                    string path = null;
                    if (elements.Length > 7)
                        path = elements[7];

                    string imageIdentifier = elements[0];
                    if (Wim.Utility.IsNumeric(imageIdentifier))
                    {
                        imageID = Wim.Utility.ConvertToInt(imageIdentifier);
                        if (string.IsNullOrEmpty(portal))
                            image = Sushi.Mediakiwi.Data.Image.SelectOneByGallery(path, imageID.Value);
                        else
                            image = Sushi.Mediakiwi.Data.Image.SelectOneByPortal(imageID.Value, portal);
                    }
                    else
                    {
                        imageGUID = Wim.Utility.ConvertToGuid(imageIdentifier);
                        image = Sushi.Mediakiwi.Data.Image.SelectOneByGallery(path, imageGUID.Value);
                    }

                    var formatType = (Sushi.Mediakiwi.Data.ImageFileFormat)Wim.Utility.ConvertToInt(elements[1]);

                    context.Response.Buffer = true;
                    context.Response.ContentType = image.Type;

                    Sushi.Mediakiwi.Data.ImageFileFormat t = ((Sushi.Mediakiwi.Data.ImageFileFormat)((elements.Length > 1) ? Wim.Utility.ConvertToInt(elements[1]) : 0));
                    if (t == Sushi.Mediakiwi.Data.ImageFileFormat.None)
                        context.Response.TransmitFile(context.Server.MapPath(image.Path));
                    else
                    {
                        int w = Wim.Utility.ConvertToInt(elements[2]);
                        int h = Wim.Utility.ConvertToInt(elements[3]);
                        var b = Wim.CommonConfiguration.DEFAULT_THUMB_BGCOLOR;

                        string file = null;
                        if (image.Width == w && image.Height == h)
                        {
                            file = image.Path;
                        }
                        else
                        {
                            if (elements.Length > 6)
                                b = new int[] { Wim.Utility.ConvertToInt(elements[4]), Wim.Utility.ConvertToInt(elements[5]), Wim.Utility.ConvertToInt(elements[6]) };
                            
                            if (formatType == Data.ImageFileFormat.ValidateMaximumWidth)
                                file = image.ApplyForcedMaximumWidth(null, w, Sushi.Mediakiwi.Data.ImagePosition.Center, false, true);

                            else if (formatType == Data.ImageFileFormat.ValidateMaximumHeight)
                                file = image.ApplyForcedMaximumHeight(null, h, false, true);

                            else if (formatType == Data.ImageFileFormat.ValidateMaximumWidthAndHeight)
                                file = image.ApplyForcedMaximum(null, w, h, false, true);

                            else
                                file = image.ApplyForcedBorder(null, w, h, b, false, Data.ImagePosition.Center, null, false, true);
                        }

                        context.Response.Cache.SetCacheability(HttpCacheability.Public);

                        if (Sushi.Mediakiwi.Data.Asset.HasCloudSetting)
                        {
                            if (string.IsNullOrEmpty(file))
                            {
                                context.Response.ContentType = "text/plain";
                                context.Response.Write("[no-file]");
                                context.Response.End();
                            }
                            ScaledAssetCacheManager.AddUrl(Wim.Utility.GetSafeUrl(context.Request), file);
                            context.Response.Redirect(file);
                        }

                        if (System.IO.File.Exists(context.Server.MapPath(file)))
                            context.Response.TransmitFile(context.Server.MapPath(file));
                        else
                        {
                            if (Wim.CommonConfiguration.NO_IMAGE_ASSET > 0)
                            {
                                image = Sushi.Mediakiwi.Data.Image.SelectOne(Wim.CommonConfiguration.NO_IMAGE_ASSET);
                                file = image.ApplyForcedBorder(null, w, h, b, false, Data.ImagePosition.Center);

                                if (System.IO.File.Exists(context.Server.MapPath(file)))
                                    context.Response.TransmitFile(context.Server.MapPath(file));
                                else
                                    context.Response.End();
                            }
                            else
                                context.Response.End();
                        }
                    }
                }
                #endregion
                else if (
                    !string.IsNullOrEmpty(context.Request.QueryString["file"])
                    )
                {
                    this.PerformUpload(context);
                    return;
                }
                #region Document response
                else if (Wim.Utility.IsNumeric(query, out fileReference))
                {
                    Sushi.Mediakiwi.Data.Document document = Sushi.Mediakiwi.Data.Document.SelectOne(fileReference);
                    PushDocument(context, document);
                }
                #endregion

                #region Document response via GUID

                else if (Wim.Utility.IsGuid(context.Request.Url.Query.Replace("?", ""), out fileReferenceGuid))
                {
                    Sushi.Mediakiwi.Data.Document document = Sushi.Mediakiwi.Data.Document.SelectOne(fileReferenceGuid);
                    PushDocument(context, document);

                }
                #endregion
                #region Document response via name
                else
                {
                    var appuser = Sushi.Mediakiwi.Data.ApplicationUserLogic.Select();
                    if (appuser.IsNewInstance)
                    {
                        context.Response.StatusCode = 403;
                        context.Response.Status = "403 Forbidden";
                        context.Response.Flush();
                        context.Response.End();
                        return;
                    }
                    
                    var split1 = context.Request.Url.Query.Replace("?", "").Split('\\');
                    var result = split1[split1.Length - 1];
                    var split2 = result.Split('/');
                    string fileName = split2[split2.Length - 1];
                    string filePath = Wim.Utility.AddApplicationPath(string.Concat(Wim.CommonConfiguration.RelativeRepositoryTmpUrl, fileName));

                    bool found = true;
                    if (!System.IO.File.Exists(context.Server.MapPath(filePath)))
                    {
                        filePath = Wim.Utility.AddApplicationPath(string.Concat(Wim.CommonConfiguration.RelativeRepositoryPackageUrl, fileName));
                        if (!System.IO.File.Exists(context.Server.MapPath(filePath)))
                        {
                            found = false;
                        }
                    }

                    context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
                    context.Response.Buffer = true;

                    string[] split = fileName.Split('.');
                    context.Response.ContentType = Sushi.Mediakiwi.Data.Asset.GuessType(split[split.Length - 1]);
                    context.Response.AppendHeader("Content-Disposition", string.Format("attachment;filename=\"{0}\"", fileName));
                    context.Response.Clear();

                    if (found)
                        context.Response.TransmitFile(context.Server.MapPath(filePath));
                    context.Response.End();
                }
                #endregion
            }
        }

        void PushDocument(HttpContext context, Data.Document document)
        {
            if (document != null)
            {
                if (string.IsNullOrEmpty(document.RemoteLocation))
                {
                    ProcessStatistics(context, document.ID);

                    context.Response.Clear();
                    context.Response.ContentType = System.Net.Mime.MediaTypeNames.Application.Octet;
                    context.Response.Cache.SetCacheability(HttpCacheability.Private);
                    context.Response.Expires = -1;
                    context.Response.Buffer = true;
                    context.Response.AppendHeader("Content-Disposition", string.Format("attachment;filename=\"{0}\"", document.FileName));

                    //context.Response.TransmitFile(context.Server.MapPath(document.Path));
                    //Edit DV 2012-06-27: Transmit File is asynchronous and doesn't buffer the complete file in memory. 
                    //Better performance for large files and high traffic
                    //Changed all occurences in this document, see image section earlier
                    //context.Response.WriteFile(context.Server.MapPath(document.Path));
                    context.Response.TransmitFile(context.Server.MapPath(document.Path));
                }
                else
                {
                    if (document.IsImage)
                    {
                        context.Response.Clear();
                        context.Response.ContentType = document.AssetType;
                        context.Response.Cache.SetCacheability(HttpCacheability.Private);
                        context.Response.Expires = -1;
                        context.Response.Buffer = true;
                        //context.Response.AppendHeader("Content-Disposition", string.Format("attachment;filename=\"{0}\"", document.FileName));


                        //context.Response.Redirect(document.RemoteLocation);
                        document.Stream.CopyTo(context.Response.OutputStream);
                    }
                    else
                    {
                        ProcessStatistics(context, document.ID);

                        context.Response.Clear();
                        context.Response.ContentType = System.Net.Mime.MediaTypeNames.Application.Octet;
                        context.Response.Cache.SetCacheability(HttpCacheability.Private);
                        context.Response.Expires = -1;
                        context.Response.Buffer = true;
                        context.Response.AppendHeader("Content-Disposition", string.Format("attachment;filename=\"{0}\"", document.FileName));

                        document.Stream.CopyTo(context.Response.OutputStream);
                    }
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether another request can use the <see cref="T:System.Web.IHttpHandler"/> instance.
        /// </summary>
        /// <value></value>
        /// <returns>true if the <see cref="T:System.Web.IHttpHandler"/> instance is reusable; otherwise, false.</returns>
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        #region MultiFileUpload
        private HttpContext _httpContext;
        private string _tempExtension = "_temp";
        private string _fileName;
        private string _parameters;
        private bool _lastChunk;
        private bool _firstChunk;
        private long _startByte;

        StreamWriter _debugFileStreamWriter;
        TextWriterTraceListener _debugListener;

        private static object CloudCleanupLock = new object();
        private static DateTime? LastCloudCleanup { get; set; }

        void PerformUpload(HttpContext context)
        {
            _httpContext = context;

            if (context.Request.InputStream.Length == 0)
                throw new ArgumentException("No file input");

            try
            {
                //StartDebugListener();                

                GetQueryStringParameters();

                string uploadFolder = GetUploadFolder();
                uploadFolder = uploadFolder.Replace("\\/", "\\");
                string tempFileName = _fileName + _tempExtension;
                
                if (Sushi.Mediakiwi.Data.Asset.HasCloudSetting && !string.IsNullOrEmpty(Sushi.Mediakiwi.Data.Asset.m_CloundInstance.LocalUploadFolder))
                {
                    //use a temp dir to upload
                    uploadFolder = Sushi.Mediakiwi.Data.Asset.m_CloundInstance.LocalUploadFolder;
                    if (!System.IO.Directory.Exists(uploadFolder))
                        System.IO.Directory.CreateDirectory(uploadFolder);

                    //if it is a cloud environment, we clean up the local dir every fifteen minutes
                    lock (CloudCleanupLock)
                    {
                        if (LastCloudCleanup == null || LastCloudCleanup.Value.AddMinutes(15) < DateTime.Now)
                        {
                            LastCloudCleanup = DateTime.Now;
                            try
                            {
                                //TO DO: do this on a different thread, I wanted to use all the nice new async features of 4.5, but apparently we're stuck in 3.5 world...
                                var files = System.IO.Directory.GetFiles(uploadFolder);
                                DateTime olderThan = DateTime.Now.AddMinutes(-30);
                                foreach (var file in files)
                                {
                                    if (System.IO.File.GetCreationTime(file) < olderThan)
                                    try
                                    {
                                        System.IO.File.Delete(file);
                                    }
                                    catch { }
                                }
                            }
                            catch (Exception ex)
                            {
                                Sushi.Mediakiwi.Data.Notification.InsertOne("Multi upload", ex);
                            }
                        }
                    }
                }

                //Is it the first chunk? Prepare by deleting any existing files with the same name
                if (_firstChunk)
                {
                    Debug.WriteLine("First chunk arrived at webservice");

                    //Delete temp file
                    if (File.Exists(@HostingEnvironment.ApplicationPhysicalPath + "/" + uploadFolder + "/" + tempFileName))
                        File.Delete(@HostingEnvironment.ApplicationPhysicalPath + "/" + uploadFolder + "/" + tempFileName);

                    //Delete target file
                    if (File.Exists(@HostingEnvironment.ApplicationPhysicalPath + "/" + uploadFolder + "/" + _fileName))
                        File.Delete(@HostingEnvironment.ApplicationPhysicalPath + "/" + uploadFolder + "/" + _fileName);

                }

                //Write the file
                Debug.WriteLine(string.Format("Write data to disk FOLDER: {0}", uploadFolder));
                //using (FileStream fs = File.Create(@HostingEnvironment.ApplicationPhysicalPath + "/" + uploadFolder + "/" + tempFileName))
                //{

                //}

                using (FileStream fs = File.Open(uploadFolder + "/" + tempFileName, FileMode.Append))
                {
                    SaveFile(context.Request.InputStream, fs);
                }

                Debug.WriteLine("Write data to disk SUCCESS");

                //Is it the last chunk? Then finish up...
                if (_lastChunk)
                {
                    Debug.WriteLine("Last chunk arrived");

                    string filepath = uploadFolder + "/" + _fileName;

                    //Rename file to original file
                    if (File.Exists(filepath))
                        File.Delete(filepath);
                    
                    File.Move(uploadFolder + "/" + tempFileName, filepath);

                    //Finish stuff....
                    FinishedFileUpload(_fileName, _parameters);

                    int galleryID = Wim.Utility.ConvertToInt(_parameters);

                    Sushi.Mediakiwi.Data.Asset newAsset = Sushi.Mediakiwi.Data.Asset.SelectOne(galleryID, _fileName);
                    newAsset.GalleryID = galleryID;
                    newAsset.CompletePath = Sushi.Mediakiwi.Data.Gallery.SelectOne(newAsset.GalleryID).CompletePath + "/" + _fileName;
                    newAsset.FileName = _fileName;

                    int index = _fileName.LastIndexOf('.') + 1;
                    string[] fileSplit = _fileName.Split('\\');

                    newAsset.Title = _fileName.Substring(0, index -1);
                    newAsset.Extention = _fileName.Substring(index, _fileName.Length - index);
                    newAsset.Type = Sushi.Mediakiwi.Data.Asset.GuessType(newAsset.Extention);
                    newAsset.Size = new FileInfo(filepath).Length;
                    newAsset.Created = Sushi.Mediakiwi.Data.Common.DatabaseDateTime;

                    newAsset.Save();
                    //newAsset.CreateThumbnail(true);
                    //newAsset.Save();

                    //to do: don't save the stream first to disk, save it directly to blob storage
                    //but this is not possible, because we receive chunks that we assemble
                    //solution 1. is to upload to a temp folder if HasCloudSetting = true
                    //the temp folder needs te be cleared on a regular basis
                    //we could do this everytime a multi-file upload is performed
                    //or we could use a background process/thread that starts as soon as the application starts 
                    //solution 2. is to change the multi file upload so it does not use chunks, but complete files instead, 
                    //but I don't think that's feasable
                    //Sushi.Mediakiwi.Data.Asset.CloudUpload(newAsset);
                    bool savedToCloud = false;
                    using (FileStream fs = File.Open(filepath, FileMode.Open))
                    {                        
                        savedToCloud = Sushi.Mediakiwi.Data.AssetLogic.CloudUpload(newAsset, null, fs, newAsset.FileName, newAsset.Type);                        
                    }
                    if (savedToCloud)
                    {
                        newAsset.Save();
                        File.Delete(filepath);                                                
                    }
                }

            }
            catch (Exception e)
            {
                // Sushi.Mediakiwi.Data.Notification.InsertOne("Multi Upload", "Path : " + @HostingEnvironment.ApplicationPhysicalPath + "/" + uploadFolder + "/" + tempFileName);
                Sushi.Mediakiwi.Data.Notification.InsertOne("Multi Upload", e);
                Debug.WriteLine(e.ToString());

                throw;
            }
            finally
            {
                //StopDebugListener();
            }
        }


        /// <summary>
        /// Get the querystring parameters
        /// </summary>
        private void GetQueryStringParameters()
        {
            _fileName = _httpContext.Request.QueryString["file"];
            _parameters = _httpContext.Request.QueryString["param"];
            _lastChunk = string.IsNullOrEmpty(_httpContext.Request.QueryString["last"]) ? true : bool.Parse(_httpContext.Request.QueryString["last"]);
            _firstChunk = string.IsNullOrEmpty(_httpContext.Request.QueryString["first"]) ? true : bool.Parse(_httpContext.Request.QueryString["first"]);
            _startByte = string.IsNullOrEmpty(_httpContext.Request.QueryString["offset"]) ? 0 : long.Parse(_httpContext.Request.QueryString["offset"]); ;
        }

        /// <summary>
        /// Save the contents of the Stream to a file
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="fs"></param>
        private void SaveFile(Stream stream, FileStream fs)
        {
            byte[] buffer = new byte[4096];
            int bytesRead;
            while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) != 0)
            {
                fs.Write(buffer, 0, bytesRead);
            }
        }

        /// <summary>
        /// Do your own stuff here when the file is finished uploading
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="parameters"></param>
        protected virtual void FinishedFileUpload(string fileName, string parameters)
        {
        }

        protected virtual string GetUploadFolder()
        {
            Sushi.Mediakiwi.Data.Gallery g = Sushi.Mediakiwi.Data.Gallery.SelectOne(Wim.Utility.ConvertToInt(_parameters));
            g.CreateFolder();
            return g.LocalFolderPath;
        }


        /// <summary>
        /// Write debug output to a textfile in debug mode
        /// </summary>
        [Conditional("DEBUG")]
        private void StartDebugListener()
        {
            try
            {
                _debugFileStreamWriter = System.IO.File.AppendText("debug.txt");
                _debugListener = new TextWriterTraceListener(_debugFileStreamWriter);
                Debug.Listeners.Add(_debugListener);
            }
            catch
            {
            }
        }

        /// <summary>
        /// Clean up the debug listener
        /// </summary>
        [Conditional("DEBUG")]
        private void StopDebugListener()
        {
            try
            {
                Debug.Flush();
                _debugFileStreamWriter.Close();
                Debug.Listeners.Remove(_debugListener);
            }
            catch
            {
            }
        }
        #endregion
    }
}
