using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Services;
using System.IO;

namespace Sushi.Mediakiwi.Framework
{
    /// <summary>
    /// 
    /// </summary>
    [WebService(Name = "Web Information Manager Server Service", Namespace = "http://www.wimserver.com")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class ThinCommunicationService : System.Web.Services.WebService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ThinCommunicationService"/> class.
        /// </summary>
        public ThinCommunicationService()
        {
        }

        /// <summary>
        /// Pings this instance.
        /// </summary>
        /// <returns></returns>
        [WebMethod]
        public string Ping()
        {
            return PingResponse();
        }

        /// <summary>
        /// Determines whether [is exclude path] [the specified path].
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>
        /// 	<c>true</c> if [is exclude path] [the specified path]; otherwise, <c>false</c>.
        /// </returns>
        bool IsExcludePath(string path)
        {
            if (path.Contains("\\alt\\") || path.Contains("\\_svn\\") || path.Contains("\\.svn\\")) return true;
            if (path.EndsWith("alt") || path.EndsWith("_svn") || path.EndsWith(".svn")) return true;
            return false;
        }

        /// <summary>
        /// Determines whether [is exclude file] [the specified file path].
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns>
        /// 	<c>true</c> if [is exclude file] [the specified file path]; otherwise, <c>false</c>.
        /// </returns>
        bool IsExcludeFile(string filePath)
        {
            if (filePath.EndsWith("thumbs.db", StringComparison.InvariantCultureIgnoreCase)) return true;
            return false;
        }

        /// <summary>
        /// Files the system clean.
        /// </summary>
        [WebMethod]
        public void FileSystemClean()
        {
            try
            {
                string root = Server.MapPath(Wim.Utility.AddApplicationPath(Sushi.Mediakiwi.Data.Environment.Current.RepositoryFolder));
                ScanFileSystem(Directory.GetDirectories(root), Sushi.Mediakiwi.Data.Gallery.SelectOneRoot().ID, root);

                ScanFileSystemFile(Directory.GetFiles(root), Sushi.Mediakiwi.Data.Gallery.SelectOneRoot().ID);

                foreach (Sushi.Mediakiwi.Data.Asset asset in Sushi.Mediakiwi.Data.Asset.SelectAll())
                {
                    if (!asset.Exists)
                    {
                        Sushi.Mediakiwi.Data.AssetLogic.UpdateActive(asset.ID);
                    }
                }
            }
            catch (Exception ex)
            {
                Sushi.Mediakiwi.Data.Notification.InsertOne("WIM Service: FileSystemClean", Sushi.Mediakiwi.Data.NotificationType.Error, ex);
            }
        }

        /// <summary>
        /// Scans the file system.
        /// </summary>
        /// <param name="folders">The folders.</param>
        /// <param name="searchFolder">The search folder.</param>
        /// <param name="findFolder">The find folder.</param>
        void ScanFileSystem(string[] folders, int searchFolder, string findFolder)
        {
            foreach (string folder in folders)
            {
                if (IsExcludePath(folder)) continue;

                string relativeFolder = string.Concat("/", folder.Replace(findFolder, string.Empty).Replace("\\", "/"));

                if (relativeFolder.Length > 1 && relativeFolder.EndsWith("/"))
                    relativeFolder = relativeFolder.Substring(0, relativeFolder.Length - 1);

                string[] folderSplit = relativeFolder.Split('/');

                Sushi.Mediakiwi.Data.Gallery gallery = Sushi.Mediakiwi.Data.Gallery.SelectOne(relativeFolder);
                if (gallery.IsNewInstance)
                {
                    gallery.ParentID = searchFolder;
                    gallery.Name = folderSplit[folderSplit.Length - 1];
                    gallery.CompletePath = relativeFolder;
                    gallery.Save();
                }
                else
                {
                    if (!gallery.IsActive)
                        gallery.Save();
                }
                ScanFileSystemFile(Directory.GetFiles(folder), gallery.ID);
                ScanFileSystem(Directory.GetDirectories(folder), gallery.ID, findFolder);
            }
        }

        void ScanFileSystemFile(string[] files, int searchFolder)
        {
            foreach (string filepath in files)
            {
                string[] fileParts = filepath.ToString().Split('\\');
                string file = fileParts[fileParts.Length - 1];
                string path = filepath.Replace(file, string.Empty);
                long size = new FileInfo(filepath).Length;

                FileData fileData = new FileData();
                fileData.Path = path;
                fileData.Size = size;
                fileData.File = file;

                if (IsExcludeFile(file))
                    continue;

                FileInfo(fileData, ChangeType.Created, null);
            }
        }

        /// <summary>
        /// Folders the info.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="state">The state.</param>
        /// <param name="newPath">The new path.</param>
        /// <returns></returns>
        [WebMethod]
        public Sushi.Mediakiwi.Data.Gallery FolderInfo(string path, ChangeType state, string newPath)
        {
            try
            {
                if (IsExcludePath(path)) return new Sushi.Mediakiwi.Data.Gallery();

                //\test\anders\xxxx
                //\test\anders2\xxxx

                string findFolder = Server.MapPath(Wim.Utility.AddApplicationPath(Sushi.Mediakiwi.Data.Environment.Current.RepositoryFolder));
                string relativeFolder = string.Concat("/", path.Replace(findFolder, string.Empty).Replace("\\", "/"));

                if (relativeFolder.Length > 1 && relativeFolder.EndsWith("/"))
                    relativeFolder = relativeFolder.Substring(0, relativeFolder.Length - 1);


                int changedIndex = 0;
                string changedFoldername = null;

                string relativeFolderNew = null;

                if (state == ChangeType.Renamed)
                {
                    if (!string.IsNullOrEmpty(newPath))
                    {
                        relativeFolderNew = string.Concat("/", newPath.Replace(findFolder, string.Empty).Replace("\\", "/"));

                        if (relativeFolderNew.Length > 1 && relativeFolderNew.EndsWith("/"))
                            relativeFolderNew = relativeFolderNew.Substring(0, relativeFolderNew.Length - 1);

                        string[] old1 = relativeFolder.Split('/');
                        string[] new1 = relativeFolderNew.Split('/');

                        for (int index = 0; index < old1.Length; index++)
                        {
                            if (old1[index] != new1[index])
                            {
                                changedFoldername = new1[index];
                                changedIndex = index;
                                break;
                            }
                        }
                    }
                }

                Sushi.Mediakiwi.Data.Gallery gallery = Sushi.Mediakiwi.Data.Gallery.SelectOne(relativeFolder);

                if (state == ChangeType.Deleted)
                {
                    if (!gallery.IsNewInstance)
                        Sushi.Mediakiwi.Data.Gallery.Deactivate(gallery.CompletePath);
                }
                else
                {
                    if (!gallery.IsNewInstance && !gallery.IsActive)
                        gallery.Save();

                    string[] folderSplit = relativeFolder.Split('/');

                    Sushi.Mediakiwi.Data.Gallery tmp;
                    string folderBuid = "";
                    string folderBuidChanged = "";

                    int previousFolder = 0;
                    int scanIndex = 0;

                    foreach (string folder in folderSplit)
                    {
                        if (changedIndex > 0 && changedIndex == scanIndex)
                            folderBuidChanged = folderBuid + (folderBuid == "/" ? changedFoldername : string.Concat("/", changedFoldername));

                        folderBuid += folderBuid == "/" ? folder : string.Concat("/", folder);

                        tmp = Sushi.Mediakiwi.Data.Gallery.SelectOne(folderBuid);

                        //  A foldername/part has changed
                        if (changedIndex > 0 && changedIndex == scanIndex)
                        {
                            tmp.Name = changedFoldername;
                            tmp.UpdateChildren(folderBuidChanged);
                            tmp.CompletePath = folderBuidChanged;
                            tmp.Save();
                            relativeFolder = relativeFolderNew;
                            break;
                        }
                        else
                        {
                            if (tmp.IsNewInstance)
                            {
                                gallery = new Sushi.Mediakiwi.Data.Gallery();
                                gallery.ParentID = previousFolder;
                                gallery.Name = folder;
                                gallery.CompletePath = folderBuid;
                                gallery.Save();
                                tmp = gallery;
                                previousFolder = gallery.ID;
                            }
                            else
                                previousFolder = tmp.ID;
                        }
                        scanIndex++;
                    }
                    gallery = Sushi.Mediakiwi.Data.Gallery.SelectOne(relativeFolder);
                }

                return gallery;
            }
            catch (Exception ex)
            {
                Sushi.Mediakiwi.Data.Notification.InsertOne("WIM Service: FolderInfo", Sushi.Mediakiwi.Data.NotificationType.Error, ex);
                return new Sushi.Mediakiwi.Data.Gallery();
            }

        }


        /// <summary>
        /// Files the info.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="state">The state.</param>
        /// <param name="newFile">The new file.</param>
        /// <returns></returns>
        [WebMethod]
        public Sushi.Mediakiwi.Data.Asset FileInfo(FileData file, ChangeType state, FileData newFile)
        {
            try
            {
                if (IsExcludePath(file.Path)) return new Sushi.Mediakiwi.Data.Asset();
                if (IsExcludeFile(file.File)) return new Sushi.Mediakiwi.Data.Asset();

                string findFolder = Server.MapPath(Wim.Utility.AddApplicationPath(Sushi.Mediakiwi.Data.Environment.Current.RepositoryFolder));
                string relativeFolder = string.Concat("/", file.Path.Replace(findFolder, string.Empty).Replace("\\", "/"));

                if (relativeFolder.Length > 1 && relativeFolder.EndsWith("/"))
                    relativeFolder = relativeFolder.Substring(0, relativeFolder.Length - 1);

                Sushi.Mediakiwi.Data.Gallery gallery = Sushi.Mediakiwi.Data.Gallery.SelectOne(relativeFolder);
                if (gallery.IsNewInstance)
                {
                    string[] folderSplit = relativeFolder.Split('/');

                    Sushi.Mediakiwi.Data.Gallery tmp;
                    string folderBuid = "";

                    int previousFolder = 0;
                    foreach (string folder in folderSplit)
                    {
                        folderBuid += folderBuid == "/" ? folder : string.Concat("/", folder);
                        tmp = Sushi.Mediakiwi.Data.Gallery.SelectOne(folderBuid);

                        if (tmp.IsNewInstance)
                        {
                            gallery = new Sushi.Mediakiwi.Data.Gallery();
                            gallery.ParentID = previousFolder;
                            gallery.Name = folder;
                            gallery.CompletePath = folderBuid;
                            gallery.Save();

                            previousFolder = gallery.ID;
                        }
                        else
                            previousFolder = tmp.ID;
                    }
                }
                gallery = Sushi.Mediakiwi.Data.Gallery.SelectOne(relativeFolder);

                Sushi.Mediakiwi.Data.Asset asset;
                asset = Sushi.Mediakiwi.Data.Asset.SelectOne(gallery.ID, file.File);
                if (asset.IsNewInstance)
                    asset = Sushi.Mediakiwi.Data.Asset.SelectOne(file.File, Convert.ToInt32(file.Size));

                if (asset.IsNewInstance)
                {
                    if (state == ChangeType.Changed || state == ChangeType.Renamed)
                        state = ChangeType.Created;
                }

                switch (state)
                {
                    case ChangeType.Changed:
                        asset.Size = Convert.ToInt32(file.Size);
                        asset.IsActive = true;
                        asset.Created = Sushi.Mediakiwi.Data.Common.DatabaseDateTime;
                        asset.Save();
                        break;
                    case ChangeType.Renamed:

                        string tmpTitle = asset.FileName.Replace(string.Concat(".", asset.Extention), string.Empty);
                        if (asset.Title.Equals(tmpTitle))
                            asset.Title = newFile.File.Replace(string.Concat(".", asset.Extention), string.Empty);

                        asset.FileName = newFile.File;

                        asset.IsActive = true;
                        asset.Created = Sushi.Mediakiwi.Data.Common.DatabaseDateTime;
                        asset.Save();
                        break;
                    case ChangeType.Deleted:
                        if (!asset.IsNewInstance)
                        {
                            Sushi.Mediakiwi.Data.AssetLogic.UpdateActive(asset.ID);
                        }
                        break;
                    case ChangeType.Created:
                        if (asset.IsNewInstance)
                        {
                            asset.GalleryID = gallery.ID;
                            asset.FileName = file.File;
                            asset.Size = file.Size;
                            asset.EvaluateAssetType();
                            asset.Save();
                        }
                        else
                        {
                            asset.GalleryID = gallery.ID;
                            asset.IsActive = true;
                            asset.Created = Sushi.Mediakiwi.Data.Common.DatabaseDateTime;
                            asset.Save();
                        }
                        break;
                }

                return asset;
            }
            catch (Exception ex)
            {
                Sushi.Mediakiwi.Data.Notification.InsertOne("WIM Service: FileInfo", Sushi.Mediakiwi.Data.NotificationType.Error, ex);
                return new Sushi.Mediakiwi.Data.Asset();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public class FileData
        {
            /// <summary>
            /// Gets or sets the path.
            /// </summary>
            /// <value>The path.</value>
            public string Path { get; set; }
            /// <summary>
            /// Gets or sets the file.
            /// </summary>
            /// <value>The file.</value>
            public string File { get; set; }
            /// <summary>
            /// Gets or sets the size.
            /// </summary>
            /// <value>The size.</value>
            public long Size { get; set; }
        }

        /// <summary>
        /// 
        /// </summary>
        public enum ChangeType
        {
            /// <summary>
            /// 
            /// </summary>
            Created,
            /// <summary>
            /// 
            /// </summary>
            Deleted,
            /// <summary>
            /// 
            /// </summary>
            Changed,
            /// <summary>
            /// 
            /// </summary>
            Renamed,
            /// <summary>
            /// 
            /// </summary>
            Moved
        }

        /// <summary>
        /// Pings the response.
        /// </summary>
        /// <returns></returns>
        public static string PingResponse()
        {
            return Wim.Utility.HashStringBySHA1(string.Concat(DateTime.Now.Date.Ticks, Sushi.Mediakiwi.Data.Environment.Current.Secret, Sushi.Mediakiwi.Data.Environment.Current.Title, Sushi.Mediakiwi.Data.Environment.Current.Created.Ticks.ToString())).ToUpper();
        }

        /// <summary>
        /// Authenticates me.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="callingDomain">The calling domain.</param>
        /// <returns></returns>
        [WebMethod]
        public AuthenticateResponse AuthenticateMe(Authenticate candidate, string callingDomain)
        {
            //Sushi.Mediakiwi.Data.Portal portal = new Sushi.Mediakiwi.Data.Portal();
            //AuthenticateResponse response = new AuthenticateResponse();

            //if (string.IsNullOrEmpty(Sushi.Mediakiwi.Data.Environment.Current.Password))
            //{
            //    Sushi.Mediakiwi.Data.Notification.InsertOne("Remote authentication", Sushi.Mediakiwi.Data.NotificationType.Warning, "There is no local environmental password");
            //    return null;
            //}
            //if (!Sushi.Mediakiwi.Data.Environment.Current.Password.Equals(candidate.Password))
            //{
            //    Sushi.Mediakiwi.Data.Notification.InsertOne("Remote authentication", Sushi.Mediakiwi.Data.NotificationType.Warning, "The supplied environmental password does not match");
            //    return null;
            //}

            //if (candidate.Domain == candidate.DomainLocal)
            //{
            //    Sushi.Mediakiwi.Data.Notification.InsertOne("Remote authentication", Sushi.Mediakiwi.Data.NotificationType.Warning, "The remote and local domains can not be simular");
            //    return null;
            //}

            //var user = Sushi.Mediakiwi.Data.ApplicationUser.SelectOne(candidate.Username);

            //if (user.IsNewInstance)
            //{
            //    Sushi.Mediakiwi.Data.Notification.InsertOne("Remote authentication", Sushi.Mediakiwi.Data.NotificationType.Warning, string.Format("The applied user ('{0}') does not exist", candidate.Username));
            //    return null;
            //}

            //wimServerCommunication.WebInformationManagerServerService wim = new Wim.wimServerCommunication.WebInformationManagerServerService();
            //wim.Url = string.Concat("http://", callingDomain, "/repository/tcl.asmx");

            //if (wim.Ping() != candidate.CRC)
            //{
            //    Sushi.Mediakiwi.Data.Notification.InsertOne("Remote authentication", Sushi.Mediakiwi.Data.NotificationType.Warning, string.Format("The applied CRC from domain '{0}' does not match", callingDomain));
            //    return null;
            //}

            //response.Authenticode = GetAuthenticode(portal.GUID, DateTime.Now.Date.Ticks.ToString());
            //response.Portal = Sushi.Mediakiwi.Data.Environment.Current.DisplayName;
            //response.Hash = GetAuthenticationCode(user, DateTime.Now.Date.Ticks.ToString());

            //if (candidate.BiDirection)
            //{
            //    wimServerCommunication.Authenticate validate = new wimServerCommunication.Authenticate();
            //    validate.BiDirection = false;
            //    validate.CRC = this.Ping();
            //    validate.Domain = candidate.DomainLocal;
            //    validate.Password = candidate.PasswordLocal;
            //    validate.Username = candidate.UsernameLocal;
            //    validate.DomainLocal = candidate.Domain;

            //    wimServerCommunication.AuthenticateResponse validationResponse = wim.AuthenticateMe(validate, candidate.Domain);
            //    portal.Name = validationResponse.Portal;
            //    portal.Authentication = validationResponse.Hash;
            //    portal.Authenticode = candidate.Authenticode;
            //    portal.Created = DateTime.Now.Date;
            //    portal.IsActive = true;
            //    portal.Domain = candidate.DomainLocal;
            //    portal.UserID = user.ID;
            //    portal.Save();
            //}
            
            //return response;
            return null;
        }

        /// <summary>
        /// Gets the authenticode.
        /// </summary>
        /// <param name="guid">The GUID.</param>
        /// <param name="ticks">The ticks.</param>
        /// <returns></returns>
        public static string GetAuthenticode(Guid guid, string ticks)
        {
            return Wim.Utilities.Encryption.Encrypt(guid.ToString(), string.Concat(Sushi.Mediakiwi.Data.Environment.Current.Secret, ticks));
        }

        /// <summary>
        /// Gets the authentication code.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="ticks">The ticks.</param>
        /// <returns></returns>
        public static string GetAuthenticationCode(Sushi.Mediakiwi.Data.IApplicationUser user, string ticks)
        {
            string hash = string.Concat(Sushi.Mediakiwi.Data.Environment.Current.Secret, user.Name, user.Password, user.Email, ticks);
            return Wim.Utility.HashStringBySHA1(hash);
        }

        /// <summary>
        /// Validates the authentication code.
        /// </summary>
        /// <param name="authenticationCode">The authentication code.</param>
        /// <param name="user">The user.</param>
        /// <param name="portal">The portal.</param>
        /// <returns></returns>
        public static bool ValidateAuthenticationCode(string authenticationCode, Sushi.Mediakiwi.Data.ApplicationUser user, Sushi.Mediakiwi.Data.Portal portal)
        {
            string hash = string.Concat(Sushi.Mediakiwi.Data.Environment.Current.Secret, user.Name, user.Password, user.Email, portal.Created.Ticks);
            string hash2 = Wim.Utility.HashStringBySHA1(hash);
            return
                hash2
                .Equals(authenticationCode);
        }

        /// <summary>
        /// Gets the authenticode.
        /// </summary>
        /// <param name="authenticode">The authenticode.</param>
        /// <param name="ticks">The ticks.</param>
        /// <returns></returns>
        public static Guid GetAuthenticode(string authenticode, string ticks)
        {
            string code = Wim.Utilities.Encryption.Decrypt(authenticode, string.Concat(Sushi.Mediakiwi.Data.Environment.Current.Secret, ticks));
            return new Guid(code);
        }

        /// <summary>
        /// 
        /// </summary>
        public class Authenticate
        {
            public string Username { get; set; }
            public string Password { get; set; }
            public string Domain { get; set; }
            public string UsernameLocal { get; set; }
            public string PasswordLocal { get; set; }
            public string DomainLocal { get; set; }
            public string CRC { get; set; }
            public string Authenticode { get; set; }
            public bool BiDirection { get; set; }
        }
        public class AuthenticateResponse
        {
            public string Portal { get; set; }
            public string Authenticode { get; set; }
            public string Hash { get; set; }
        }

    }
}
