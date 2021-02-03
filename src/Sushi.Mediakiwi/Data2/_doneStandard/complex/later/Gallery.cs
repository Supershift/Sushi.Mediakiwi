using System;
using System.Linq;
using System.Web;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Sushi.Mediakiwi.Data.DalReflection;
using System.Text.RegularExpressions;

namespace Sushi.Mediakiwi.Data
{
    /// <summary>
    /// Represents a Gallery entity.
    /// </summary>
    [DatabaseTable("wim_Galleries v")]
    public class Gallery : DatabaseEntity, iExportable
    {
        #region Paths

        static string m_LocalWebRoot;
        /// <summary>
        /// Gets or sets the local web root. Can be set for use in testing of paths
        /// </summary>
        /// <value>The local web root.</value>
        internal static string LocalWebRoot
        {
            get
            {
                if (string.IsNullOrEmpty(m_LocalWebRoot))
                {
                    System.Web.HttpContext context = System.Web.HttpContext.Current;
                    if (context != null)
                        m_LocalWebRoot = context.Server.MapPath(Wim.Utility.AddApplicationPath("/"));
                }
                return m_LocalWebRoot;
            }
            set { m_LocalWebRoot = value; }
        }

        /// <summary>
        /// Gets the local thumbnail repository base.
        /// </summary>
        /// <value>The local thumbnail repository base.</value>
        internal static string LocalThumbnailRepositoryBase
        {
            get { return string.Concat(LocalWebRoot, Wim.CommonConfiguration.RelativeRepositoryImageThumbnailUrl); }
        }

        /// <summary>
        /// Gets the local generated image repository base.
        /// </summary>
        /// <value>The local generated image repository base.</value>
        internal static string LocalGeneratedImageRepositoryBase
        {
            get { return string.Concat(LocalWebRoot, Wim.CommonConfiguration.RelativeRepositoryGeneratedImageUrl); }
        }

        /// <summary>
        /// Gets the local repository base.
        /// </summary>
        /// <value>The local repository base.</value>
        internal static string LocalRepositoryBase
        {
            get { return string.Concat(LocalWebRoot, Wim.CommonConfiguration.RelativeRepositoryUrl); }
        }

        #endregion

        /// <summary>
        /// Selects all.
        /// </summary>
        /// <param name="databaseMapName">Name of the database map.</param>
        /// <returns></returns>
        public static Gallery[] SelectAllByMap(string databaseMapName)
        {
            List<Gallery> list = new List<Gallery>();
            Gallery implement = new Gallery();
            implement.SqlOrder = "Gallery_CompletePath";

            //  Other database connection
            var connection = Sushi.Mediakiwi.Data.Common.GetCurrentMappingConnectionByName(databaseMapName);
            if (connection != null)
                implement.SqlConnectionString = connection.Connection;
            else
                return SelectAll();

            //if (!string.IsNullOrEmpty(SqlConnectionString2)) implement.SqlConnectionString = SqlConnectionString2;
            foreach (object o in implement._SelectAll(null, false, "All", databaseMapName))
                list.Add((Gallery)o);
            return list.ToArray();
        }


        string m_LocalFolderPath;
        /// <summary>
        /// Gets or sets the local folder path.
        /// </summary>
        /// <value>The local folder path.</value>
        public string LocalFolderPath
        {
            get
            {
                if (!string.IsNullOrEmpty(this.CompletePath) && HttpContext.Current != null)
                {
                    string prefix = Wim.Utility.AddApplicationPath(Wim.CommonConfiguration.RelativeRepositoryUrl);
                    m_LocalFolderPath = string.Concat(HttpContext.Current.Request.MapPath(prefix), this.CompletePath);
                }
                return m_LocalFolderPath;
            }
            set { m_LocalFolderPath = value; }
        }


        /// <summary>
        /// Selects the one.
        /// </summary>
        /// <param name="parentID">The parent ID.</param>
        /// <param name="gallery">The gallery.</param>
        /// <param name="databaseMapName">Name of the database map.</param>
        /// <returns></returns>
        public static Gallery SelectOne(int ID, string gallery, string databaseMapName)
        {
            Gallery implement = new Gallery();
            List<DatabaseDataValueColumn> list = new List<DatabaseDataValueColumn>();

            //  Other database connection
            var connection = Sushi.Mediakiwi.Data.Common.GetCurrentMappingConnectionByName(databaseMapName);
            if (connection != null)
                implement.SqlConnectionString = connection.Connection;

            if (string.IsNullOrEmpty(gallery))
            {
                list.Add(new DatabaseDataValueColumn("Gallery_Key", SqlDbType.Int, ID));
                return (Gallery)implement._SelectOne(list);
            }
            else
            {
                list.Add(new DatabaseDataValueColumn("Gallery_Name", SqlDbType.NVarChar, gallery));
                list.Add(new DatabaseDataValueColumn("Gallery_Gallery_Key", SqlDbType.Int, ID));
                return (Gallery)implement._SelectOne(list);
            }
        }



        public static Gallery SelectOneByMap(int ID, string databaseMapName)
        {
            Gallery implement = new Gallery();
            List<DatabaseDataValueColumn> list = new List<DatabaseDataValueColumn>();

            //  Other database connection
            var connection = Sushi.Mediakiwi.Data.Common.GetCurrentMappingConnectionByName(databaseMapName);
            if (connection != null)
                implement.SqlConnectionString = connection.Connection;

            list.Add(new DatabaseDataValueColumn("Gallery_Key", SqlDbType.Int, ID));
            return (Gallery)implement._SelectOne(list);

        }

        public static Gallery SelectOneByMap(int ID, Sushi.Mediakiwi.Framework.WimServerPortal connection)
        {
            Gallery implement = new Gallery();
            List<DatabaseDataValueColumn> list = new List<DatabaseDataValueColumn>();

            //  Other database connection
            if (connection != null)
                implement.SqlConnectionString = connection.Connection;

            list.Add(new DatabaseDataValueColumn("Gallery_Key", SqlDbType.Int, ID));
            return (Gallery)implement._SelectOne(list);

        }

        /// <summary>
        /// Selects the one.
        /// </summary>
        /// <param name="relativePath">The relative path.</param>
        /// <returns></returns>
        public static Gallery SelectOne(string relativePath)
        {
            var mapping = Sushi.Mediakiwi.Data.Common.GetCurrentGalleryMappingUrl(relativePath);
            if (mapping != null)
                return SelectOneByPortal(relativePath, mapping.Portal);

            Gallery implement = new Gallery();

            List<DatabaseDataValueColumn> list = new List<DatabaseDataValueColumn>();
            list.Add(new DatabaseDataValueColumn("Gallery_CompletePath", SqlDbType.NVarChar, relativePath));

            return (Gallery)implement._SelectOne(list);
        }

        /// <summary>
        /// Selects the one.
        /// </summary>
        /// <param name="relativePath">The relative path.</param>
        /// <param name="databaseMapName">Name of the database map.</param>
        /// <returns></returns>
        public static Gallery SelectOneByMap(string relativePath, string databaseMapName)
        {
            Gallery implement = new Gallery();
            List<DatabaseDataValueColumn> list = new List<DatabaseDataValueColumn>();

            //  Other database connection
            implement.DatabaseMappingPortal = Sushi.Mediakiwi.Data.Common.GetCurrentMappingConnectionByName(databaseMapName);

            list.Add(new DatabaseDataValueColumn("Gallery_CompletePath", SqlDbType.NVarChar, relativePath));
            return (Gallery)implement._SelectOne(list);
        }


        /// <summary>
        /// Selects the one by portal.
        /// </summary>
        /// <param name="ID">The identifier.</param>
        /// <param name="portalName">Name of the portal.</param>
        /// <returns></returns>
        public static Gallery SelectOneByPortal(int ID, string portalName)
        {
            Gallery implement = new Gallery();
            List<DatabaseDataValueColumn> list = new List<DatabaseDataValueColumn>();

            //  Other database connection
            implement.DatabaseMappingPortal = Sushi.Mediakiwi.Data.Common.GetPortal(portalName, false);

            list.Add(new DatabaseDataValueColumn("Gallery_Key", SqlDbType.Int, ID));
            return (Gallery)implement._SelectOne(list);
        }

        /// <summary>
        /// Selects the one by portal.
        /// </summary>
        /// <param name="relativePath">The relative path.</param>
        /// <param name="portalName">Name of the portal.</param>
        /// <returns></returns>
        public static Gallery SelectOneByPortal(string relativePath, string portalName)
        {
            Gallery implement = new Gallery();
            List<DatabaseDataValueColumn> list = new List<DatabaseDataValueColumn>();

            //  Other database connection
            implement.DatabaseMappingPortal = Sushi.Mediakiwi.Data.Common.GetPortal(portalName, false);

            list.Add(new DatabaseDataValueColumn("Gallery_CompletePath", SqlDbType.NVarChar, relativePath));
            return (Gallery)implement._SelectOne(list);
        }

        /// <summary>
        /// Selects the one.
        /// </summary>
        /// <param name="gallery">The gallery.</param>
        /// <param name="level">The level.</param>
        /// <returns></returns>
        public static Gallery SelectOne(Sushi.Mediakiwi.Data.Gallery gallery, int level)
        {
            if (level < 1) return new Gallery();
            if (gallery == null || gallery.IsNewInstance) return new Gallery();
            if (gallery.Name == CommonConfiguration.siteRoot) return gallery;

            //  Exception from folder
            string[] split = gallery.CompletePath.Split('/');
            int splitcount = split.Length;
            //  / = 2
            //  /level_01/ = 3
            //  /level_01/level_02/ = 4

            //  Is root
            if (splitcount == 1) return new Gallery();
            //  Non existant level
            if ((level - (splitcount - 1)) > 0) return new Gallery();
            //  Same level
            if ((level - (splitcount - 1)) == 0) return gallery;

            string searchPath = string.Format("/{0}", split[level]);
            while (level > 1)
            {
                level--;
                searchPath = string.Format("/{0}{1}", split[level], searchPath);
            }
            searchPath = searchPath.Replace("//", "/");

            Gallery implement = new Gallery();
            List<DatabaseDataValueColumn> list = new List<DatabaseDataValueColumn>();

            list.Add(new DatabaseDataValueColumn("Gallery_CompletePath", SqlDbType.NVarChar, searchPath));
            list.Add(new DatabaseDataValueColumn("Gallery_IsActive", SqlDbType.Bit, true));

            return (Gallery)implement._SelectOne(list);
        }

        /// <summary>
        /// Identifies the specified identifier.
        /// </summary>
        /// <param name="identifier">The identifier, this can be numerical, GUID or a relative path.</param>
        /// <returns></returns>
        internal static Gallery Identify(string identifier)     //NAAR STANDARD
        {
            if (Wim.Utility.IsGuid(identifier))
                return SelectOne(new Guid(identifier));
            if (Wim.Utility.IsNumeric(identifier))
                return SelectOne(Convert.ToInt32(identifier));
            else
                return SelectOne(identifier);
        }

        /// <summary>
        /// Delete an implementation record.
        /// </summary>
        /// <returns></returns>
        public override bool Delete()           //REVIEW BY MARC
        {
            this.Execute(string.Format("delete from wim_Assets where Asset_Gallery_Key in (select Gallery_Key from wim_Galleries where Gallery_CompletePath like '{0}%')", this.CompletePath.Replace("'", "''")));
            try
            {
                System.IO.Directory.Delete(this.LocalFolderPath, true);
            }
            catch (Exception) { }

            this.SqlTable = "wim_Galleries";
            return base.Delete();
        }

        /// <summary>
        /// Save a database entity. This method will indentify the need for an INSERT or an UPDATE.
        /// </summary>
        /// <returns></returns>
        public override bool Save()             //
        {
            this.SqlTable = "wim_Galleries";
            this.IsActive = true;

            bool saved = base.Save();
            this.CreateFolder();

            return saved;
        }

        #region MOVED to EXTENSION / LOGIC


        ///// <summary>
        ///// Cleans the path.
        ///// </summary>
        //void CleanPath()
        //{
        //    Regex MustMatch = new Regex(@"^[^\|\?\:\*""<>]*$", RegexOptions.IgnoreCase);
        //    if (!MustMatch.IsMatch(this.CompletePath))
        //    {
        //        string[] split = this.CompletePath.Split('/');
        //        string candidate = "";
        //        foreach (string item in split)
        //        {
        //            string tmp = Wim.Utility.GlobalRegularExpression.Implement.ReplaceNotAcceptableFilenameCharacter.Replace(item, string.Empty);
        //            candidate += candidate == "/" ? tmp : string.Concat("/", tmp);
        //        }
        //        this.CompletePath = candidate;
        //    }
        //}

        ///// <summary>
        ///// Creates the folder.
        ///// </summary>
        ///// <param name="request">The request.</param>
        //public void CreateFolder()
        //{
        //    CleanPath();

        //    //if (request == null)
        //    //{
        //    //    request = System.Web.HttpContext.Current.Request;
        //    //    if (request == null)
        //    //        return;
        //    //}

        //    if (ID == 0) return;
        //    //Gallery[] list = SelectAllByBackwardTrail(this.ID);

        //    string folderInfo = LocalRepositoryBase;

        //    string[] folders = this.CompletePath.Split('/');

        //    for (int i = 0; i < folders.Length; i++)
        //    {
        //        string folder = folders[i];
        //        folderInfo += string.Concat("\\", folder);

        //        if (!Directory.Exists(folderInfo))
        //            Directory.CreateDirectory(folderInfo);
        //    }

        //    //string folderInfo;
        //    //if (this.Type == GalleryType.Documents)
        //    //    folderInfo = request.MapPath(Wim.Utility.AddApplicationPath(String.Concat(Wim.CommonConfiguration.RelativeRepositoryDocumentUrl, this.BaseGalleryID)));
        //    //else
        //    //    folderInfo = request.MapPath(Wim.Utility.AddApplicationPath(String.Concat(Wim.CommonConfiguration.RelativeRepositoryImageUrl, this.BaseGalleryID)));

        //    //if (!Directory.Exists(folderInfo))
        //    //{
        //    //    DirectoryInfo info = Directory.CreateDirectory(folderInfo);
        //    //}
        //}


        ///// <summary>
        ///// Determines whether [has role access] [the specified user].
        ///// </summary>
        ///// <param name="user">The user.</param>
        ///// <returns>
        ///// 	<c>true</c> if [has role access] [the specified user]; otherwise, <c>false</c>.
        ///// </returns>
        //public bool HasRoleAccess(Sushi.Mediakiwi.Data.IApplicationUser user)
        //{
        //    if (this.CompletePath == "/") return true;
        //    if (this.ID == 0 || user.Role().All_Galleries) return true;
        //    var xslist = SelectAllAccessible(user);

        //    //  [MM:10-jan-2011] add for veiling3000, this should be moved to a StoredProcedure
        //    if (Wim.CommonConfiguration.RIGHTS_GALLERY_SUBS_ARE_ALLOWED)
        //    {
        //        foreach (var item in xslist)
        //        {
        //            if (this.CompletePath.StartsWith(item.CompletePath, StringComparison.InvariantCultureIgnoreCase))
        //            {
        //                return true;
        //            }
        //        }
        //    }

        //    var selection = from item in xslist where item.ID == this.ID select item;
        //    bool xs = selection.Count() == 1;
        //    return xs;
        //}

        ///// <summary>
        ///// Updates the files.
        ///// </summary>
        //public void UpdateFiles()
        //{
        //    if (this.ID == 0 || HttpContext.Current == null)
        //        return;
        //    Sushi.Mediakiwi.Framework.Functions.CleanRepository cleaner = new Sushi.Mediakiwi.Framework.Functions.CleanRepository();
        //    CreateFolder();
        //    cleaner.ScanFiles(this);
        //    cleaner.ScanFolders(this);
        //}



        #endregion MOVED to EXTENSION / LOGIC

        #region REPLICATED TO Sushi.Mediakiwi.Data.Standard

        /// <summary>
        /// Validates the access right.
        /// </summary>
        /// <param name="galleries">The galleries.</param>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        public static Gallery[] ValidateAccessRight(Sushi.Mediakiwi.Data.Gallery[] galleries, Sushi.Mediakiwi.Data.IApplicationUser user)
        {
            return (from item in galleries join relation in SelectAllAccessible(user) on item.ID equals relation.ID select item).ToArray();
        }

        /// <summary>
        /// Selects all accessible.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        public static Gallery[] SelectAllAccessible(Sushi.Mediakiwi.Data.IApplicationUser user)
        {
            Gallery[] galleries = null;
            if (!user.Role().All_Galleries)
            {
                if (user.Role().IsAccessGallery)
                {
                    galleries = (
                        from item in SelectAll()
                        join relation in Sushi.Mediakiwi.Data.RoleRight.SelectAll(user.Role().ID, Sushi.Mediakiwi.Data.RoleRightType.Gallery) on item.ID equals relation.ItemID
                        select item).ToArray();
                }
                else
                {
                    var acl = (
                        from item in SelectAll()
                        join relation in Sushi.Mediakiwi.Data.RoleRight.SelectAll(user.Role().ID, Sushi.Mediakiwi.Data.RoleRightType.Gallery) on item.ID equals relation.ItemID
                        into combination
                        from relation in combination.DefaultIfEmpty()
                        select new { ID = item.ID, HasAccess = relation == null });

                    galleries = (
                        from item in acl
                        join relation in SelectAll() on item.ID equals relation.ID
                        where item.HasAccess
                        select relation).ToArray();
                }
            }
            else
                galleries = SelectAll();
            return galleries;
        }


        /// <summary>
        /// Update an implementaion record.
        /// </summary>
        /// <returns></returns>
        public override bool Update()
        {
            this.SqlTable = "wim_Galleries";
            this.CleanPath();
            return base.Update();
        }

        /// <summary>
        /// Updates the AssetCount property in the database by counting all assets in this gallery
        /// </summary>
        public void UpdateCount()
        {
            Execute($@"
update wim_Galleries 
set Gallery_Count = (select count(*) from wim_Assets where Asset_Gallery_Key = Gallery_Key) 
where Gallery_Key = {this.ID}");
            this.AssetCount = Wim.Utility.ConvertToInt(Execute(string.Concat("select count(*) from wim_Assets where Asset_Gallery_Key = ", this.ID)));
        }


        /// <summary>
        /// Deactivates all galleries in the set path (also all assets).
        /// </summary>
        /// <param name="completePath">The complete path.</param>
        internal static void Deactivate(string completePath)
        {
            Gallery implement = new Gallery();
            implement.Execute(string.Format("update wim_Galleries set Gallery_IsActive = 0 where Gallery_CompletePath like '{0}%'", completePath.Replace("'", "''")));
            implement.Execute(string.Format("update wim_Assets set Asset_IsActive = 0 where Asset_Gallery_Key in (select Gallery_Key from wim_Galleries where Gallery_CompletePath like '{0}%')", completePath.Replace("'", "''")));
        }

        /// <summary>
        /// Updates the children.
        /// </summary>
        /// <param name="newFolderCompletePath">The new folder complete path.</param>
        /// <returns></returns>
        internal bool UpdateChildren(string newFolderCompletePath)
        {
            this.Execute(string.Format(@"
update wim_Galleries set Gallery_CompletePath = REPLACE (Gallery_CompletePath,'{0}','{1}') where Gallery_CompletePath like '{0}%'
"
                , this.CompletePath.Replace("'", "''")
                , newFolderCompletePath.Replace("'", "''")
                ));
            return true;
        }

        
        


        private int m_ID;
        /// <summary>
        /// Gets or sets the ID.
        /// </summary>
        /// <value>The ID.</value>
        [DatabaseColumn("Gallery_Key", SqlDbType.Int, IsPrimaryKey = true)]
        public int ID
        {
            get { return m_ID; }
            set { m_ID = value; }
        }

        private string m_Name;
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        [Sushi.Mediakiwi.Framework.ContentListItem.TextField("Title gal", 50, true)]
        [DatabaseColumn("Gallery_Name", SqlDbType.NVarChar, Length = 50)]
        public string Name
        {
            get { return m_Name; }
            set { m_Name = value; }
        }

        /// <summary>
        /// Gets or sets the asset count.
        /// </summary>
        /// <value>The asset count.</value>
        [DatabaseColumn("Gallery_Count", SqlDbType.Int, IsNullable = true)]
        public int AssetCount { get; set; }

        /// <summary>
        /// Gets or sets the asset count including all deeper folder.
        /// </summary>
        /// <value>The asset count2.</value>
        //[DatabaseColumn("Total", SqlDbType.Int, IsNullable = true, IsOnlyRead = true, ColumnSubQuery = "ISNULL((select SUM(Gallery_Count) from wim_Galleries where Gallery_CompletePath like (v.Gallery_CompletePath + '%')), 0)")]
        //public int AssetCount2 { get; set; }

        private short m_TypeID;
        /// <summary>
        /// Gets or sets the type id.
        /// </summary>
        /// <value>The type id.</value>
        [Sushi.Mediakiwi.Framework.OnlyEditableWhenTrue("IsNewElement")]
        [Sushi.Mediakiwi.Framework.ContentListItem.Choice_Radio("Type", "TypeCollection", "GalType", true, true)]
        [DatabaseColumn("Gallery_Type", SqlDbType.SmallInt, IsNullable = true)]
        public short TypeID
        {
            get { return m_TypeID; }
            set { m_TypeID = value; }
        }

        private int m_FormatType;
        /// <summary>
        /// Gets or sets the type of the format.
        /// </summary>
        /// <value>The type of the format.</value>
        [Sushi.Mediakiwi.Framework.OnlyVisibleWhenTrue("FormatTypeIsVisible")]
        [Sushi.Mediakiwi.Framework.ContentListItem.Choice_Dropdown("Format type", "FormatCollection", true, true)]
        [DatabaseColumn("Gallery_FormatType", SqlDbType.Int, IsNullable = true)]
        public int FormatType
        {
            get { return m_FormatType; }
            set { m_FormatType = value; }
        }

        private System.Nullable<int> m_ParentID;
        /// <summary>
        /// Gets or sets the parent ID.
        /// </summary>
        /// <value>The parent ID.</value>
        [DatabaseColumn("Gallery_Gallery_Key", SqlDbType.Int, IsNullable = true)]
        public System.Nullable<int> ParentID
        {
            get { return m_ParentID; }
            set { m_ParentID = value; }
        }

        private int? m_BaseGalleryID;
        /// <summary>
        /// Gets or sets the base gallery.
        /// </summary>
        /// <value>The base gallery.</value>
        [DatabaseColumn("Gallery_Base_Key", SqlDbType.Int, IsNullable = true)]
        public int? BaseGalleryID
        {
            get { return m_BaseGalleryID; }
            set { m_BaseGalleryID = value; }
        }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        public GalleryType Type
        {
            get { return (GalleryType)m_TypeID; }
            set { m_TypeID = (short)value; }
        }

        private bool m_IsFolder;
        /// <summary>
        /// Gets or sets a value indicating whether this instance is folder.
        /// </summary>
        /// <value><c>true</c> if this instance is folder; otherwise, <c>false</c>.</value>
        [DatabaseColumn("Gallery_IsFolder", SqlDbType.Bit)]
        public bool IsFolder
        {
            get { return m_IsFolder; }
            set { m_IsFolder = value; }
        }


        private bool m_IsFixed;
        /// <summary>
        /// Gets or sets a value indicating whether this instance is folder.
        /// </summary>
        /// <value><c>true</c> if this instance is folder; otherwise, <c>false</c>.</value>
        [DatabaseColumn("Gallery_IsFixed", SqlDbType.Bit)]
        public bool IsFixed
        {
            get { return m_IsFixed; }
            set { m_IsFixed = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is active.
        /// </summary>
        /// <value><c>true</c> if this instance is active; otherwise, <c>false</c>.</value>
        [DatabaseColumn("Gallery_IsActive", SqlDbType.Bit, IsNullable = true)]
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is hidden.
        /// </summary>
        /// <value><c>true</c> if this instance is hidden; otherwise, <c>false</c>.</value>
        [DatabaseColumn("Gallery_IsHidden", SqlDbType.Bit, IsNullable = true)]
        public bool IsHidden { get; set; }

        private string m_Format;
        /// <summary>
        /// Gets or sets the format.
        /// </summary>
        /// <value>The format.</value>
        [Sushi.Mediakiwi.Framework.OnlyVisibleWhenTrue("FormatIsVisible")]
        [Sushi.Mediakiwi.Framework.ContentListItem.TextField("Custom format(s)", 50, true, false, "If 0 is applied the image scaling is applied according to the ratio of FormatHeight.<br/>Multiple formats are allowed according the following setup: WIDTHxHEIGHT;WIDTHxHEIGHT")]
        [DatabaseColumn("Gallery_Format", SqlDbType.NVarChar, Length = 50, IsNullable = true)]
        public string Format
        {
            get { return m_Format; }
            set { m_Format = value; }
        }

        private string m_BackgroundRgb;
        /// <summary>
        /// Gets or sets the background RGB.
        /// </summary>
        /// <value>The background RGB.</value>
        [Sushi.Mediakiwi.Framework.OnlyVisibleWhenTrue("BorderIsVisible")]
        [Sushi.Mediakiwi.Framework.ContentListItem.TextField("Border RGB color", 11, true, false, "Apply the RGB values in rrr.ggg.bbb format (0-255)")]
        [DatabaseColumn("Gallery_BackgroundRgb", SqlDbType.VarChar, Length = 14, IsNullable = true)]
        public string BackgroundRgb
        {
            get { return m_BackgroundRgb; }
            set { m_BackgroundRgb = value; }
        }

        /// <summary>
        /// Gets the background RGB.
        /// </summary>
        /// <returns></returns>
        public int[] GetBackgroundRgb()
        {
            if (string.IsNullOrEmpty(m_BackgroundRgb)) return null;
            if (m_BackgroundRgb.Split('.').Length != 3) return null;
            return Wim.Utility.ConvertToIntArray(m_BackgroundRgb.Split('.'));
        }

        string m_CompletePath;
        /// <summary>
        /// Gets or sets the complete path.
        /// </summary>
        /// <value>The complete path.</value>
        [DatabaseColumn("Gallery_CompletePath", SqlDbType.NVarChar, Length = 1000, IsNullable = true)]
        public string CompletePath
        {
            get { return m_CompletePath; }
            set {
                if (value != null)
                {
                    value = value.Trim();

                }
                m_CompletePath = value; 
            }
        }

        string m_CompleteCleanPath;
        /// <summary>
        /// Completes the clean path.
        /// </summary>
        /// <returns></returns>
        public string CompleteCleanPath()
        {
            if (this.CompletePath == "/") return "/";
            if (m_CompleteCleanPath == null)
            {
                string candidate = this.CompletePath;
                if (candidate.StartsWith("/"))
                    candidate = this.CompletePath.Remove(0, 1);
                if (candidate.EndsWith("/"))
                    candidate = candidate.Remove(candidate.LastIndexOf("/"), 1);
                m_CompleteCleanPath = candidate;
            }
            return m_CompleteCleanPath;
        }

        private Guid m_GUID;
        /// <summary>
        /// Gets or sets the GUID.
        /// </summary>
        /// <value>The GUID.</value>
        [Sushi.Mediakiwi.Framework.ContentListItem.TextLine("Guid")]
        [DatabaseColumn("Gallery_GUID", SqlDbType.UniqueIdentifier, IsMigrationKey = true)]
        public Guid GUID
        {
            get {
                if (this.m_GUID == Guid.Empty) this.m_GUID = Guid.NewGuid();
                return m_GUID; }
            set { m_GUID = value; }
        }

        private DateTime m_Created;
        /// <summary>
        /// Gets or sets the created.
        /// </summary>
        /// <value>The created.</value>
        [Sushi.Mediakiwi.Framework.ContentListItem.TextLine("Created")]
        [DatabaseColumn("Gallery_Created", SqlDbType.DateTime)]
        public DateTime Created
        {
            get {
                if (this.m_Created == DateTime.MinValue) this.m_Created = Sushi.Mediakiwi.Data.Common.DatabaseDateTime;
                return m_Created; }
            set { m_Created = value; }
        }

        /// <summary>
        /// Selects the one.
        /// </summary>
        /// <param name="ID">The ID.</param>
        /// <returns></returns>
        public static Gallery SelectOne(int ID)
        {
            return (Gallery)new Gallery()._SelectOne(ID);
        }


        /// <summary>
        /// Selects the one.
        /// </summary>
        /// <param name="parentID">The parent ID.</param>
        /// <param name="gallery">The gallery.</param>
        /// <returns></returns>
        public static Gallery SelectOne(int parentID, string gallery)
        {
            Gallery implement = new Gallery();
            List<DatabaseDataValueColumn> list = new List<DatabaseDataValueColumn>();

            list.Add(new DatabaseDataValueColumn("Gallery_Name", SqlDbType.NVarChar, gallery));
            list.Add(new DatabaseDataValueColumn("Gallery_Gallery_Key", SqlDbType.Int, parentID));

            return (Gallery)implement._SelectOne(list);

        }

        /// <summary>
        /// Selects all.
        /// </summary>
        /// <returns></returns>
        public static Gallery[] SelectAll()
        {
            List<Gallery> list = new List<Gallery>();
            Gallery implement = new Gallery();
            implement.SqlOrder = "Gallery_CompletePath";

            List<DatabaseDataValueColumn> whereClause = new List<DatabaseDataValueColumn>();
            whereClause.Add(new DatabaseDataValueColumn("Gallery_IsHidden", SqlDbType.Bit, false));

            foreach (object o in implement._SelectAll(whereClause))
                list.Add((Gallery)o);
            return list.ToArray();
        }


        /// <summary>
        /// Searches all.
        /// </summary>
        /// <param name="searchCandidate">The search candidate.</param>
        /// <returns></returns>
        public static Gallery[] SelectAll(string searchCandidate)
        {
            //searchCandidate = string.Concat("%", searchCandidate.Trim().Replace(" ", "%"), "%");
            var selection = (from item in SelectAll() where item.Name.IndexOf(searchCandidate, StringComparison.InvariantCultureIgnoreCase) > -1 && item.IsActive select item).ToArray();
            return selection;
        }

        /// <summary>
        /// Searches the all_ by path.
        /// </summary>
        /// <param name="searchPath">The search path.</param>
        /// <returns></returns>
        public static Gallery[] SearchAll_ByPath(string searchPath)
        {
            var selection = (from item in SelectAll() where item.CompletePath.StartsWith(searchPath, StringComparison.InvariantCultureIgnoreCase) select item).ToArray();
            return selection;
        }

        static Gallery GetParentFolder(Gallery[] folders, int ID)
        {
            foreach (Gallery folder in folders)
            {
                if (folder.ID == ID)
                    return folder;
            }
            return null;
        }

        /// <summary>
        /// Selects all by backward trail.
        /// </summary>
        /// <param name="galleryID">The gallery ID.</param>
        /// <returns></returns>
        public static Gallery[] SelectAllByBackwardTrail(int galleryID)
        {
            Gallery implement = new Gallery();
            List<Gallery> listAll = new List<Gallery>();

            Gallery folder = Gallery.SelectOne(galleryID);

            if (!string.IsNullOrEmpty(SqlConnectionString2)) implement.SqlConnectionString = SqlConnectionString2;
            foreach (object o in implement._SelectAll())
                listAll.Add((Gallery)o);

            List<Gallery> list = new List<Gallery>();
            list.Add(folder);

            Gallery[] arr = listAll.ToArray();
            if (folder.ParentID.HasValue)
            {
                Gallery innerFolder = GetParentFolder(arr, folder.ParentID.Value);
                while (innerFolder != null)
                {
                    list.Insert(0, innerFolder);
                    if (innerFolder.ParentID.HasValue)
                        innerFolder = GetParentFolder(arr, innerFolder.ParentID.Value);
                    else
                        innerFolder = null;
                }
            }

            return list.ToArray();

        }

        /// <summary>
        /// Selects the one.
        /// </summary>
        /// <param name="galleryGUID">The gallery GUID.</param>
        /// <returns></returns>
        public static Gallery SelectOne(Guid galleryGUID)
        {
            if (galleryGUID == Guid.Empty) return new Gallery();
            Gallery implement = new Gallery();
            List<DatabaseDataValueColumn> list = new List<DatabaseDataValueColumn>();

            list.Add(new DatabaseDataValueColumn("Gallery_GUID", SqlDbType.UniqueIdentifier, galleryGUID));
            list.Add(new DatabaseDataValueColumn("Gallery_IsActive", SqlDbType.Bit, true));

            return (Gallery)implement._SelectOne(list);
        }

        /// <summary>
        /// Selects the one_ root.
        /// </summary>
        /// <returns></returns>
        public static Gallery SelectOneRoot()
        {
            Gallery implement = new Gallery();
            implement.SqlOrder = "Gallery_Key asc";
            List<DatabaseDataValueColumn> list = new List<DatabaseDataValueColumn>();
            list.Add(new DatabaseDataValueColumn("Gallery_Base_Key", SqlDbType.Int, null));
            return (Gallery)implement._SelectOne(list, "Root", "0"); 
            //return (Gallery)implement._SelectOne(list);
        }

        /// <summary>
        /// Selects the one_ root.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static Gallery SelectOneRoot(GalleryType type)
        {
            Gallery implement = new Gallery();
            List<DatabaseDataValueColumn> list = new List<DatabaseDataValueColumn>();
            list.Add(new DatabaseDataValueColumn("Gallery_Type", SqlDbType.SmallInt, (Int16)type));
            list.Add(new DatabaseDataValueColumn("Gallery_IsFixed", SqlDbType.Bit, true));

            return (Gallery)implement._SelectOne(list);
        }

        /// <summary>
        /// Selects all.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="returnFolder">if set to <c>true</c> [return folder].</param>
        /// <returns></returns>
        public static Gallery[] SelectAll(GalleryType type, bool returnFolder)
        {
            if (returnFolder)
            {
                return (from item in SelectAll() where !item.IsFixed && item.IsActive && item.TypeID == (Int16)type select item).ToArray();
            }
            return (from item in SelectAll() where !item.IsFixed && item.IsActive && item.TypeID == (Int16)type && !item.IsFolder select item).ToArray();
        }

        /// <summary>
        /// Selects all by parent.
        /// </summary>
        /// <param name="parentID">The parent ID.</param>
        /// <param name="returnHidden">if set to <c>true</c> [return hidden].</param>
        /// <returns></returns>
        public static Gallery[] SelectAllByParent(int parentID, bool returnHidden)
        {
            List<Gallery> list = new List<Gallery>();

            Gallery implement = new Gallery();
            implement.SqlOrder = "Gallery_Name";

            List<DatabaseDataValueColumn> whereClause = new List<DatabaseDataValueColumn>();
            whereClause.Add(new DatabaseDataValueColumn("Gallery_Gallery_Key", SqlDbType.Int, parentID));
            whereClause.Add(new DatabaseDataValueColumn("Gallery_IsActive", SqlDbType.Bit, true));

            if (!returnHidden)
                whereClause.Add(new DatabaseDataValueColumn("Gallery_IsHidden", SqlDbType.Bit, false));

            if (!string.IsNullOrEmpty(SqlConnectionString2)) implement.SqlConnectionString = SqlConnectionString2;
            
            foreach (object o in implement._SelectAll(whereClause))
                list.Add((Gallery)o);
            return list.ToArray();

        }

        /// <summary>
        /// Selects all by parent.
        /// </summary>
        /// <param name="parentID">The parent ID.</param>
        /// <returns></returns>
        public static Gallery[] SelectAllByParent(int parentID)
        {
            return SelectAllByParent(parentID, false);
        }

        /// <summary>
        /// Selects the all_ by base.
        /// </summary>
        /// <param name="baseGalleryID">The base gallery ID.</param>
        /// <returns></returns>
        public static Gallery[] SelectAllByBase(int baseGalleryID)
        {
            return (from item in SelectAll() where item.BaseGalleryID.HasValue && item.BaseGalleryID == baseGalleryID && item.IsActive orderby item.Name select item).ToArray();
        }

        /// <summary>
        /// Insert an implementation record.
        /// </summary>
        /// <returns></returns>
        public override bool Insert()
        {
            this.SqlTable = "wim_Galleries";
            this.CleanPath();
            this.Name = GetPageNameProposal(this.ParentID.GetValueOrDefault(), this.Name);
            return base.Insert();
        }

        /// <summary>
        /// Gets the page name proposal.
        /// </summary>
        /// <param name="galleryID">The gallery ID.</param>
        /// <param name="gallery">The gallery.</param>
        /// <returns></returns>
        string GetPageNameProposal(int galleryID, string gallery)
        {
            string nameProposal = Wim.Utility.GlobalRegularExpression.Implement.ReplaceNotAcceptableFilenameCharacter.Replace(gallery, string.Empty);

            bool galleryExistsInFolder = IsGalleryAlreadyTaken(galleryID, gallery);
            int nameExtentionCount = 0;

            while (galleryExistsInFolder)
            {
                nameExtentionCount++;
                nameProposal = string.Format("{0}_{1}", gallery, nameExtentionCount);
                galleryExistsInFolder = IsGalleryAlreadyTaken(galleryID, nameProposal);
            }
            return nameProposal;
        }

        /// <summary>
        /// Gets the child count.
        /// </summary>
        /// <value>The child count.</value>
        public int ChildCount
        {
            get
            {
                Gallery implement = new Gallery();
                return Utility.ConvertToInt(implement.Execute(string.Format("SELECT (select count(*) from wim_Galleries where Gallery_Gallery_Key = {0}) + count(*) from wim_Assets where Asset_Gallery_Key = {0}", this.ID)));
            }
        }

        bool IsGalleryAlreadyTaken(int galleryID, string gallery)
        {
            using (Connection.DataCommander dac = new Connection.DataCommander(this.SqlConnectionString))
            {
                dac.ConnectionType = this.ConnectionType;
                dac.Text = "select count(*) from wim_Galleries where Gallery_Gallery_Key = @Gallery_Key and Gallery_Name = @Gallery_Name";
                dac.SetParameterInput("@Gallery_Key", galleryID, SqlDbType.Int);
                dac.SetParameterInput("@Gallery_Name", gallery, SqlDbType.NVarChar, 50);
                int count = (int)dac.ExecScalar();
                if (count == 0)
                    return false;
                return true;
            }
        }

        /// <summary>
        /// Applies the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="parent">The parent.</param>
        public void Apply(string name, Gallery parent)
        {
            this.Name = name.Trim();
            this.CompletePath = string.Concat(parent.CompletePath, "/", this.Name);
            this.IsFolder = true;
            this.IsFixed = false;
            this.ParentID = parent.ID;
        }

        #region iExportable Members


        public DateTime? Updated
        {
            get { return null;  }
        }

        #endregion

        public Gallery Parent
        {
            get
            {
                if (ParentID.HasValue)
                    return Gallery.SelectOne(ParentID.Value);
                else
                    return null;
            }
        }

        #endregion REPLICATED TO Sushi.Mediakiwi.Data.Standard
    }
}
