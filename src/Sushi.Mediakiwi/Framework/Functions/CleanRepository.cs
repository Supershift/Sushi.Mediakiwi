using System;
using System.IO;
using System.Web;
using System.Collections.Generic;
using System.Text;

namespace Sushi.Mediakiwi.Framework.Functions
{
    /// <summary>
    /// 
    /// </summary>
    public class CleanRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CleanRepository"/> class.
        /// </summary>
        public CleanRepository()
        {
        }

        HttpContext m_Context;

        /// <summary>
        /// Scans this server for assets
        /// </summary>
        /// <returns></returns>
        public bool Scan()
        {
            m_Debug = true;

            m_Context = HttpContext.Current;

            m_Root = m_Context.Request.MapPath(Utility.AddApplicationPath(Wim.CommonConfiguration.RelativeRepositoryUrl));
            m_GalleryRoot = Sushi.Mediakiwi.Data.Gallery.SelectOneRoot();

            string startDirectory = m_Context.Request.MapPath(Wim.Utility.AddApplicationPath(Wim.CommonConfiguration.RelativeRepositoryUrl));

            //Sushi.Mediakiwi.Data.Asset.UpdateActive();
            return Scan(startDirectory, true);
        }

        /// <summary>
        /// Scans the folders.
        /// </summary>
        /// <param name="gallery">The gallery.</param>
        public void ScanFolders(Sushi.Mediakiwi.Data.Gallery gallery)
        {
            m_Context = HttpContext.Current;
            string baseFolder = m_Context.Request.MapPath(Utility.AddApplicationPath(string.Concat(Wim.CommonConfiguration.RelativeRepositoryUrl, gallery.CompletePath)));
            m_Root = m_Context.Request.MapPath(Utility.AddApplicationPath(Wim.CommonConfiguration.RelativeRepositoryUrl));

            foreach (string folder in Directory.GetDirectories(baseFolder))
            {
                if (IsFolderException(folder))
                    continue;

                string galleryName = string.Concat("/", folder.Replace(m_Root, string.Empty).Replace("\\", "/"));

                Sushi.Mediakiwi.Data.Gallery item = Sushi.Mediakiwi.Data.Gallery.SelectOne(galleryName);
                if (item.IsNewInstance)
                {
                    //  Could not find gallery... Recreate path
                    string galleries = folder.Replace(m_Root, string.Empty);

                    item.Name = folder.Split('\\')[folder.Split('\\').Length - 1];
                    item.ParentID = gallery.ID;

                    if (gallery.CompletePath.EndsWith("/"))
                        item.CompletePath = string.Concat(gallery.CompletePath, item.Name);
                    else
                        item.CompletePath = string.Concat(gallery.CompletePath, "/", item.Name);

                    item.Save();
                }
            }
        }

        /// <summary>
        /// Scans the specified gallery.
        /// </summary>
        /// <param name="gallery">The gallery.</param>
        /// <returns></returns>
        public void ScanFiles(Sushi.Mediakiwi.Data.Gallery gallery)
        {
            m_Context = HttpContext.Current;
            string folder = m_Context.Request.MapPath(Utility.AddApplicationPath(string.Concat(Wim.CommonConfiguration.RelativeRepositoryUrl, gallery.CompletePath)));

            foreach (string file in Directory.GetFiles(folder))
            {
                if (IsFileException(file))
                    continue;

                string[] fileparts = file.Split('\\');
                Sushi.Mediakiwi.Data.Asset asset = Sushi.Mediakiwi.Data.Asset.SelectOne(folder, fileparts[fileparts.Length - 1]);

                if (asset.IsNewInstance)
                {
                    FileInfo nfo = new FileInfo(file);
                    asset.SaveStream(nfo, gallery, true);
                }
                asset.ValidateImage(true);
            }
        }

        /// <summary>
        /// Scans the specified start directory.
        /// </summary>
        /// <param name="startDirectory">The start directory.</param>
        /// <param name="scanSubFolders">if set to <c>true</c> [scan sub folders].</param>
        /// <returns></returns>
        public bool Scan(string startDirectory, bool scanSubFolders)
        {
            ScanDirectories(new string[] { startDirectory }, scanSubFolders);
            //CleanGalleries();

//            MessageLog = string.Format(@"
//Result of repository scan:<br/>
//- Galleries scanned: <b>{0}</b>
//- Galleries created: <b>{1}</b>
//- Galleries removed: <b>{5}</b><br/>
//- Files scanned: <b>{2}</b>
//- Files created: <b>{3}</b>
//- Files moved: <b>{4}</b>
//- Files not active: <b>{6}</b>
//", m_GalleryScanned, m_GalleryIntroduced, m_FileScanned, m_FileIntroduced, m_FileMoved, m_GalleryRemoved, Sushi.Mediakiwi.Data.Asset.SelectInActive());

            return true;
            //
        }

        /// <summary>
        /// Removes all alternative folders.
        /// </summary>
        /// <returns></returns>
        public bool RemoveAllAlternativeFolders()
        {
            m_Debug = true;

            m_Context = HttpContext.Current;

            m_Root = m_Context.Request.MapPath(Utility.AddApplicationPath(Wim.CommonConfiguration.RelativeRepositoryUrl));
            m_GalleryRoot = Sushi.Mediakiwi.Data.Gallery.SelectOneRoot();

            string startDirectory = m_Context.Request.MapPath(Wim.Utility.AddApplicationPath(Wim.CommonConfiguration.RelativeRepositoryUrl));

            DeleteDirectories("\\alt", new string[] { startDirectory });

            MessageLog = string.Format(@"Galleries removed: <b>{0}</b>", m_GalleryRemoved);

            return true;
            //
        }

        /// <summary>
        /// Recreates the thumbnails.
        /// </summary>
        /// <returns></returns>
        public bool RecreateThumbnails()
        {
            Sushi.Mediakiwi.Data.Image[] imgArr = Sushi.Mediakiwi.Data.Image.SelectAll();

            foreach (Sushi.Mediakiwi.Data.Image img in imgArr)
            {
                img.CreateThumbnail();
            }
            return true;
        }

        /// <summary>
        /// Removes the inactive assets.
        /// </summary>
        /// <returns></returns>
        public bool RemoveInactiveAssets()
        {
            return Sushi.Mediakiwi.Data.Asset.DeleteInactive();
        }

        int m_FileScanned;
        int m_FileIntroduced;
        int m_FileMoved;

        int m_GalleryScanned;
        int m_GalleryIntroduced;
        int m_GalleryRemoved;

        /// <summary>
        /// 
        /// </summary>
        public string MessageLog;

        /// <summary>
        /// Cleans the galleries.
        /// </summary>
        void CleanGalleries()
        {
            Sushi.Mediakiwi.Data.Gallery[] list = Sushi.Mediakiwi.Data.Gallery.SelectAll();
            foreach (Sushi.Mediakiwi.Data.Gallery g in list)
            {
                if (!Directory.Exists(g.LocalFolderPath))
                {
                    if (g.ChildCount == 0)
                    {
                        m_GalleryRemoved++;
                        
                        g.Delete();

                        //  Remove local folder
                        if (m_Debug) m_Context.Response.Write(string.Concat("<b>", g.LocalFolderPath, "</b> (removed gallery)<br/>"));
                    }
                }
            }
        }

        int m_fileCount = 0;

        bool IsFolderException(string folder)
        {
            if (folder.EndsWith("\\wim", StringComparison.OrdinalIgnoreCase)) return true;
            if (folder.EndsWith("\\thumbnail", StringComparison.OrdinalIgnoreCase)) return true;
            if (folder.EndsWith("\\.svn", StringComparison.OrdinalIgnoreCase)) return true;
            if (folder.EndsWith("\\_svn", StringComparison.OrdinalIgnoreCase)) return true;
            if (folder.EndsWith("\\alt", StringComparison.OrdinalIgnoreCase)) return true;

            return false;
        }

        bool IsFileException(string file)
        {
            if (file.EndsWith(".db", StringComparison.OrdinalIgnoreCase)) return true;
            if (file.EndsWith(".cs", StringComparison.OrdinalIgnoreCase)) return true;
            if (file.EndsWith(".aspx", StringComparison.OrdinalIgnoreCase)) return true;
            if (file.EndsWith(".js", StringComparison.OrdinalIgnoreCase)) return true;
            if (file.EndsWith(".exclude", StringComparison.OrdinalIgnoreCase)) return true;
            return false;
        }

        bool m_Debug;

        string m_Root;
        Sushi.Mediakiwi.Data.Gallery m_GalleryRoot;

        /// <summary>
        /// Scans the directories.
        /// </summary>
        /// <param name="folders">The folders.</param>
        /// <param name="scanSubFolders">if set to <c>true</c> [scan sub folders].</param>
        void ScanDirectories(string[] folders, bool scanSubFolders)
        {
            foreach (string folder in folders)
            {
                if (IsFolderException(folder))
                    continue;

                m_GalleryScanned++;

                //  Create the gallery
                //  //
                string gallery_Root = "/";
                string gallery = string.Concat("/", folder.Replace(m_Root, string.Empty).Replace("\\", "/"));

                Sushi.Mediakiwi.Data.Gallery item = Sushi.Mediakiwi.Data.Gallery.SelectOne(gallery);
                if (item.IsNewInstance)
                {
                    //  Could not find gallery... Recreate path
                    string galleries = folder.Replace(m_Root, string.Empty);

                    Sushi.Mediakiwi.Data.Gallery baseG = m_GalleryRoot;
                    foreach (string newGallery in galleries.Split('\\'))
                    {
                        gallery_Root = string.Concat(gallery_Root, gallery_Root.EndsWith("/") ? string.Empty : "/", newGallery);
                        Sushi.Mediakiwi.Data.Gallery item2 = Sushi.Mediakiwi.Data.Gallery.SelectOne(gallery_Root);

                        if (item2.IsNewInstance)
                        {
                            m_GalleryIntroduced++;

                            //  Create new
                            item2.ParentID = baseG.ID;
                            item2.Name = newGallery;
                            item2.IsFolder = true;
                            item2.CompletePath = gallery_Root;
                            item2.Save();

                            baseG = item2;
                            if (m_Debug) m_Context.Response.Write(string.Concat("<b>", newGallery, "</b> (introduce gallery)<br/>"));
                        }
                        baseG = item2;
                    }
                    //  Refind path
                    item = Sushi.Mediakiwi.Data.Gallery.SelectOne(gallery);
                }

                //  FILES

                if (!item.IsNewInstance)
                {
                    foreach (string file in Directory.GetFiles(folder))
                    {
                        if (IsFileException(file))
                            continue;

                        m_FileScanned++;

                        string[] fileparts = file.Split('\\');
                        Sushi.Mediakiwi.Data.Asset asset = Sushi.Mediakiwi.Data.Asset.SelectOne(folder, fileparts[fileparts.Length - 1]);
                        
                        if (asset.IsNewInstance)
                        {
                            ////  Create the file
                            FileInfo nfo = new FileInfo(file);
                            asset = Sushi.Mediakiwi.Data.Asset.SelectOne(fileparts[fileparts.Length - 1], Convert.ToInt32(nfo.Length));

                            if (asset.IsNewInstance)
                            {
                                m_FileIntroduced++;
                                asset.SaveStream(nfo, item);

                                if (m_Debug) m_Context.Response.Write(string.Concat("<b>", file, "</b> (new file) (ID:", asset.ID.ToString(), ") <br/>"));
                            }
                            else
                            {
                                m_FileMoved++;
                                asset.IsNewStyle = true;
                                asset.Size = Convert.ToInt32(nfo.Length);
                                asset.GalleryID = item.ID;
                                asset.Save();

                                if (m_Debug) m_Context.Response.Write(string.Concat("<b>", file, "</b> (relocated file) (ID:", asset.ID.ToString(), ") <br/>"));
                            }
                            m_fileCount++;
                        }
                        else
                        {
                            asset.IsNewStyle = true;
                            asset.Save();

                            if (m_Debug) m_Context.Response.Write(string.Concat("<b>", file, "</b> (found) (ID:", asset.ID.ToString(), ") <br/>"));
                        }
                    }
                }
                else
                {
                    throw new Exception("Could not determine the gallery");
                }

                if (scanSubFolders)
                {
                    string[] directories = Directory.GetDirectories(folder);

                    if (directories.Length > 0)
                        ScanDirectories(directories, scanSubFolders);
                }
            }

        }

        void DeleteDirectories(string foldertype, string[] folders)
        {
            foreach (string folder in folders)
            {
                if (folder.EndsWith(foldertype, StringComparison.OrdinalIgnoreCase))
                {
                    m_GalleryRemoved++;
                    Directory.Delete(folder, true);

                    continue;
                }

                if (IsFolderException(folder))
                    continue;

                string[] directories = Directory.GetDirectories(folder);

                if (directories.Length > 0)
                    DeleteDirectories(foldertype, directories);
            }
        }
    }
}
