using Sushi.Mediakiwi.Data.MicroORM;
using Sushi.MicroORM;
using Sushi.MicroORM.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Data
{
    /// <summary>
    /// Represents a Gallery entity.
    /// </summary>
    [DataMap(typeof(GalleryMap))]
    public class Gallery : IExportable
    {
        public class GalleryMap : DataMap<Gallery>
        {
            public GalleryMap()
            {
                Table("wim_Galleries");
                Id(x => x.ID, "Gallery_Key").Identity();
                Map(x => x.Name, "Gallery_Name").Length(50);
                Map(x => x.AssetCount, "Gallery_Count");
                Map(x => x.TypeID, "Gallery_Type").SqlType(System.Data.SqlDbType.SmallInt);
                Map(x => x.FormatType, "Gallery_FormatType");
                Map(x => x.ParentID, "Gallery_Gallery_Key");
                Map(x => x.BaseGalleryID, "Gallery_Base_Key");
                Map(x => x.IsFolder, "Gallery_IsFolder");
                Map(x => x.IsFixed, "Gallery_IsFixed");
                Map(x => x.IsActive, "Gallery_IsActive");
                Map(x => x.IsHidden, "Gallery_IsHidden");
                Map(x => x.Format, "Gallery_Format").Length(50);
                Map(x => x.BackgroundRgb, "Gallery_BackgroundRgb").Length(14);
                Map(x => x.CompletePath, "Gallery_CompletePath").Length(1000);
                Map(x => x.GUID, "Gallery_GUID");
                Map(x => x.Created, "Gallery_Created");
            }
        }

        public void Delete()
        {
            var connector = ConnectorFactory.CreateConnector<Gallery>();
            connector.Delete(this);
        }

        public async Task DeleteAsync()
        {
            var connector = ConnectorFactory.CreateConnector<Gallery>();
            await connector.DeleteAsync(this);
        }

        public void Save()
        {
            var connector = ConnectorFactory.CreateConnector<Gallery>();
            connector.Save(this);
        }

        public async Task SaveAsync()
        {
            var connector = ConnectorFactory.CreateConnector<Gallery>();
            await connector.SaveAsync(this);
        }

        /// <summary>
        /// Identifies the specified identifier.
        /// </summary>
        /// <param name="identifier">The identifier, this can be numerical, GUID or a relative path.</param>
        /// <returns></returns>
        internal static Gallery Identify(string identifier)    
        {
            if (Utility.IsGuid(identifier))
            {
                return SelectOne(new Guid(identifier));
            }
            if (Utility.IsNumeric(identifier))
            {
                return SelectOne(Convert.ToInt32(identifier));
            }
            else
            {
                return SelectOne(identifier);
            }
        }

        /// <summary>
        /// Gets or sets the ID.
        /// </summary>
        /// <value>The ID.</value>
        public int ID { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the asset count.
        /// </summary>
        /// <value>The asset count.</value>
        public int AssetCount { get; set; }

        /// <summary>
        /// Gets or sets the type id.
        /// </summary>
        /// <value>The type id.</value>
        public short TypeID { get; set; }

        /// <summary>
        /// Gets or sets the type of the format.
        /// </summary>
        /// <value>The type of the format.</value>
        public int FormatType { get; set; }

        /// <summary>
        /// Gets or sets the parent ID.
        /// </summary>
        /// <value>The parent ID.</value>
        public int? ParentID { get; set; }

        /// <summary>
        /// Gets or sets the base gallery.
        /// </summary>
        /// <value>The base gallery.</value>
        public int? BaseGalleryID { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        public GalleryType Type
        {
            get { return (GalleryType)Enum.Parse(typeof(GalleryType), TypeID.ToString()); }
            set { TypeID = (short)value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is folder.
        /// </summary>
        /// <value><c>true</c> if this instance is folder; otherwise, <c>false</c>.</value>
        public bool IsFolder { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is folder.
        /// </summary>
        /// <value><c>true</c> if this instance is folder; otherwise, <c>false</c>.</value>
        public bool IsFixed { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is active.
        /// </summary>
        /// <value><c>true</c> if this instance is active; otherwise, <c>false</c>.</value>
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is hidden.
        /// </summary>
        /// <value><c>true</c> if this instance is hidden; otherwise, <c>false</c>.</value>
        public bool IsHidden { get; set; }

        /// <summary>
        /// Gets or sets the format.
        /// </summary>
        /// <value>The format.</value>
        public string Format { get; set; }

        /// <summary>
        /// Gets or sets the background RGB.
        /// </summary>
        /// <value>The background RGB.</value>
        public string BackgroundRgb { get; set; }

        /// <summary>
        /// Gets the background RGB.
        /// </summary>
        /// <returns></returns>
        public int[] GetBackgroundRgb()
        {
            if (string.IsNullOrEmpty(BackgroundRgb))
                return null;

            if (BackgroundRgb.Split('.').Length != 3)
                return null;

            return Utility.ConvertToIntArray(BackgroundRgb.Split('.'));
        }

        private string m_CompletePath;

        /// <summary>
        /// Gets or sets the complete path.
        /// </summary>
        /// <value>The complete path.</value>
        public string CompletePath
        {
            get { return m_CompletePath; }
            set
            {
                if (value != null)
                    value = value.Trim();
                m_CompletePath = value;
            }
        }

        private string m_CompleteCleanPath;

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
        public Guid GUID
        {
            get
            {
                if (this.m_GUID == Guid.Empty)
                    this.m_GUID = Guid.NewGuid();
                return m_GUID;
            }
            set { m_GUID = value; }
        }

        private DateTime m_Created;

        /// <summary>
        /// Gets or sets the created.
        /// </summary>
        /// <value>The created.</value>
        public DateTime Created
        {
            get
            {
                if (this.m_Created == DateTime.MinValue)
                    this.m_Created = Common.DatabaseDateTime;
                return m_Created;
            }
            set { m_Created = value; }
        }

        /// <summary>
        /// Selects the one.
        /// </summary>
        /// <param name="ID">The ID.</param>
        /// <returns></returns>
        public static Gallery SelectOne(int ID)
        {
            var connector = ConnectorFactory.CreateConnector<Gallery>();
            return connector.FetchSingle(ID);
        }

        /// <summary>
        /// Selects the one Async.
        /// </summary>
        /// <param name="ID">The ID.</param>
        /// <returns></returns>
        public static async Task<Gallery> SelectOneAsync(int ID)
        {
            var connector = ConnectorFactory.CreateConnector<Gallery>();
            return await connector.FetchSingleAsync(ID);
        }

        /// <summary>
        /// Selects the one.
        /// </summary>
        /// <param name="parentID">The parent ID.</param>
        /// <param name="gallery">The gallery.</param>
        /// <returns></returns>
        public static Gallery SelectOne(int parentID, string gallery)
        {
            var connector = ConnectorFactory.CreateConnector<Gallery>();
            var filter = connector.CreateQuery();
            filter.Add(x => x.Name, gallery);
            filter.Add(x => x.ParentID, parentID);

            return connector.FetchSingle(filter);
        }

        /// <summary>
        /// Selects the one aSYNC.
        /// </summary>
        /// <param name="parentID">The parent ID.</param>
        /// <param name="gallery">The gallery.</param>
        /// <returns></returns>
        public static async Task<Gallery> SelectOneAsync(int parentID, string gallery)
        {
            var connector = ConnectorFactory.CreateConnector<Gallery>();
            var filter = connector.CreateQuery();
            filter.Add(x => x.Name, gallery);
            filter.Add(x => x.ParentID, parentID);

            return await connector.FetchSingleAsync(filter);
        }

        /// <summary>
        /// Selects all.
        /// </summary>
        /// <returns></returns>
        public static Gallery[] SelectAll()
        {
            var connector = ConnectorFactory.CreateConnector<Gallery>();
            var filter = connector.CreateQuery();
            filter.AddOrder(x => x.CompletePath);
            filter.AddSql("ISNULL([Gallery_IsHidden], 0) = 0");

            return connector.FetchAll(filter).ToArray();
        }

        /// <summary>
        /// Selects all Async.
        /// </summary>
        /// <returns></returns>
        public static async Task<Gallery[]> SelectAllAsync()
        {
            var connector = ConnectorFactory.CreateConnector<Gallery>();
            var filter = connector.CreateQuery();
            filter.AddOrder(x => x.CompletePath);
            filter.AddSql("ISNULL([Gallery_IsHidden], 0) = 0");

            var result = await connector.FetchAllAsync(filter);
            return result.ToArray();
        }

        /// <summary>
        /// Searches all.
        /// </summary>
        /// <param name="searchCandidate">The search candidate.</param>
        /// <returns></returns>
        public static Gallery[] SelectAll(string searchCandidate)
        {
            var connector = ConnectorFactory.CreateConnector<Gallery>();
            var filter = connector.CreateQuery();
            filter.AddOrder(x => x.CompletePath);
            filter.Add(x => x.Name, $"%{searchCandidate}%", ComparisonOperator.Like);
            filter.Add(x => x.IsActive, true);

            return connector.FetchAll(filter).ToArray();
        }

        /// <summary>
        /// Searches all Async.
        /// </summary>
        /// <param name="searchCandidate">The search candidate.</param>
        /// <returns></returns>
        public static async Task<Gallery[]> SelectAllAsync(string searchCandidate)
        {
            var connector = ConnectorFactory.CreateConnector<Gallery>();
            var filter = connector.CreateQuery();
            filter.AddOrder(x => x.CompletePath);
            filter.Add(x => x.Name, $"%{searchCandidate}%", ComparisonOperator.Like);
            filter.Add(x => x.IsActive, true);

            var result = await connector.FetchAllAsync(filter);
            return result.ToArray();
        }

        /// <summary>
        /// Searches by path.
        /// </summary>
        /// <param name="searchPath">The search path.</param>
        /// <returns></returns>
        public static Gallery[] SearchAll_ByPath(string searchPath)
        {
            var connector = ConnectorFactory.CreateConnector<Gallery>();
            var filter = connector.CreateQuery();
            filter.AddOrder(x => x.CompletePath);
            filter.Add(x => x.CompletePath, $"{searchPath}%", ComparisonOperator.Like);

            return connector.FetchAll(filter).ToArray();
        }

        /// <summary>
        /// Searches by path Async.
        /// </summary>
        /// <param name="searchPath">The search path.</param>
        /// <returns></returns>
        public static async Task<Gallery[]> SearchAll_ByPathAsync(string searchPath)
        {
            var connector = ConnectorFactory.CreateConnector<Gallery>();
            var filter = connector.CreateQuery();
            filter.AddOrder(x => x.CompletePath);
            filter.Add(x => x.CompletePath, $"{searchPath}%", ComparisonOperator.Like);

            var result = await connector.FetchAllAsync(filter);
            return result.ToArray();
        }

        private static Gallery GetParentFolder(Gallery[] folders, int ID)
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
            Gallery[] arr = SelectAll();
            Gallery folder = SelectOne(galleryID);

            List<Gallery> list = new List<Gallery>();
            list.Add(folder);

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
        /// Selects all by backward trail.
        /// </summary>
        /// <param name="galleryID">The gallery ID.</param>
        /// <returns></returns>
        public static async Task<Gallery[]> SelectAllByBackwardTrailAsync(int galleryID)
        {
            Gallery[] arr = await SelectAllAsync();
            Gallery folder = await SelectOneAsync(galleryID);

            List<Gallery> list = new List<Gallery>();
            list.Add(folder);

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
        /// Selects the one By GUID.
        /// </summary>
        /// <param name="galleryGUID">The gallery GUID.</param>
        /// <returns></returns>
        public static Gallery SelectOne(Guid galleryGUID)
        {
            if (galleryGUID == Guid.Empty)
                return new Gallery();

            var connector = ConnectorFactory.CreateConnector<Gallery>();
            var filter = connector.CreateQuery();

            filter.Add(x => x.GUID, galleryGUID);
            filter.Add(x => x.IsActive, true);

            return connector.FetchSingle(filter);
        }

        public static Gallery SelectOne(string relativePath, bool onlyReturnActive = false)
        {
            var connector = ConnectorFactory.CreateConnector<Gallery>();
            var filter = connector.CreateQuery();

            filter.Add(x => x.CompletePath, relativePath);
            if (onlyReturnActive)
                filter.Add(x => x.IsActive, true);

            return connector.FetchSingle(filter);
        }

        public static async Task<Gallery> SelectOneAsync(string relativePath, bool onlyReturnActive = false)
        {
            var connector = ConnectorFactory.CreateConnector<Gallery>();
            var filter = connector.CreateQuery();

            filter.Add(x => x.CompletePath, relativePath);
            if (onlyReturnActive)
            {
                filter.Add(x => x.IsActive, true);
            }

            return await connector.FetchSingleAsync(filter).ConfigureAwait(false);
        }

        /// <summary>
        /// Select a lower level gallery
        /// </summary>
        /// <param name="gallery">The gallery.</param>
        /// <param name="level">The level.</param>
        /// <returns></returns>
        public static Gallery SelectOne(Gallery gallery, int level)
        {
            if (level < 1) return new Gallery();
            if (gallery == null || gallery.ID == 0) return new Gallery();
            if (gallery.Name == CommonConfiguration.ROOT_FOLDER) return gallery;

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

            return SelectOne(searchPath, true);
        }

        /// <summary>
        /// Select a lower level gallery
        /// </summary>
        /// <param name="gallery">The gallery.</param>
        /// <param name="level">The level.</param>
        /// <returns></returns>
        public static async Task<Gallery> SelectOneAsync(Gallery gallery, int level)
        {
            if (level < 1)
            {
                return new Gallery();
            }

            if (gallery == null || gallery.ID == 0)
            {
                return new Gallery();
            }

            if (gallery.Name == CommonConfiguration.ROOT_FOLDER)
            {
                return gallery;
            }

            //  Exception from folder
            string[] split = gallery.CompletePath.Split('/');
            int splitcount = split.Length;
            //  / = 2
            //  /level_01/ = 3
            //  /level_01/level_02/ = 4

            //  Is root
            if (splitcount == 1)
            {
                return new Gallery();
            }
            //  Non existant level
            if ((level - (splitcount - 1)) > 0)
            {
                return new Gallery();
            }
            //  Same level
            if ((level - (splitcount - 1)) == 0)
            {
                return gallery;
            }

            string searchPath = $"/{split[level]}";
            while (level > 1)
            {
                level--;
                searchPath = $"/{split[level]}{searchPath}";
            }
            searchPath = searchPath.Replace("//", "/");

            return await SelectOneAsync(searchPath, true).ConfigureAwait(false);
        }

        /// <summary>
        /// Selects the one By GUID Async.
        /// </summary>
        /// <param name="galleryGUID">The gallery GUID.</param>
        /// <returns></returns>
        public static async Task<Gallery> SelectOneAsync(Guid galleryGUID)
        {
            if (galleryGUID == Guid.Empty)
                return new Gallery();

            var connector = ConnectorFactory.CreateConnector<Gallery>();
            var filter = connector.CreateQuery();

            filter.Add(x => x.GUID, galleryGUID);
            filter.Add(x => x.IsActive, true);

            return await connector.FetchSingleAsync(filter);
        }

        /// <summary>
        /// Selects one root.
        /// </summary>
        /// <returns></returns>
        public static Gallery SelectOneRoot()
        {
            var connector = ConnectorFactory.CreateConnector<Gallery>();
            var filter = connector.CreateQuery();
            filter.AddOrder(x => x.ID);
            filter.Add(x => x.BaseGalleryID, null);

            return connector.FetchSingle(filter);
        }

        /// <summary>
        /// Selects one root.
        /// </summary>
        /// <returns></returns>
        public static async Task<Gallery> SelectOneRootAsync()
        {
            var connector = ConnectorFactory.CreateConnector<Gallery>();
            var filter = connector.CreateQuery();
            filter.AddOrder(x => x.ID);
            filter.Add(x => x.BaseGalleryID, null);

            return await connector.FetchSingleAsync(filter);
        }

        /// <summary>
        /// Selects the root.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static Gallery SelectOneRoot(GalleryType type)
        {
            var connector = ConnectorFactory.CreateConnector<Gallery>();
            var filter = connector.CreateQuery();
            filter.AddOrder(x => x.ID);
            filter.Add(x => x.TypeID, (int)type);
            filter.Add(x => x.IsFixed, true);

            return connector.FetchSingle(filter);
        }

        /// <summary>
        /// Selects the root.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static async Task<Gallery> SelectOneRootAsync(GalleryType type)
        {
            var connector = ConnectorFactory.CreateConnector<Gallery>();
            var filter = connector.CreateQuery();
            filter.AddOrder(x => x.ID);
            filter.Add(x => x.TypeID, (int)type);
            filter.Add(x => x.IsFixed, true);

            return await connector.FetchSingleAsync(filter);
        }

        /// <summary>
        /// Selects all.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="returnFolder">if set to <c>true</c> [return folder].</param>
        /// <returns></returns>
        public static Gallery[] SelectAll(GalleryType type, bool returnFolder)
        {
            var connector = ConnectorFactory.CreateConnector<Gallery>();
            var filter = connector.CreateQuery();

            filter.Add(x => x.IsFixed, false);
            filter.Add(x => x.IsActive, true);
            filter.Add(x => x.TypeID, (int)type);

            if (returnFolder == false)
                filter.Add(x => x.IsFolder, false);

            return connector.FetchAll(filter).ToArray();
        }

        /// <summary>
        /// Selects all Async.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="returnFolder">if set to <c>true</c> [return folder].</param>
        /// <returns></returns>
        public static async Task<Gallery[]> SelectAllAsync(GalleryType type, bool returnFolder)
        {
            var connector = ConnectorFactory.CreateConnector<Gallery>();
            var filter = connector.CreateQuery();

            filter.Add(x => x.IsFixed, false);
            filter.Add(x => x.IsActive, true);
            filter.Add(x => x.TypeID, (int)type);

            if (returnFolder == false)
                filter.Add(x => x.IsFolder, false);

            var result = await connector.FetchAllAsync(filter);
            return result.ToArray();
        }

        /// <summary>
        /// Selects all by parent.
        /// </summary>
        /// <param name="parentID">The parent ID.</param>
        /// <param name="returnHidden">if set to <c>true</c> [return hidden].</param>
        /// <returns></returns>
        public static Gallery[] SelectAllByParent(int parentID, bool returnHidden)
        {
            var connector = ConnectorFactory.CreateConnector<Gallery>();
            var filter = connector.CreateQuery();
            filter.AddOrder(x => x.Name);
            filter.Add(x => x.ParentID, parentID);
            filter.Add(x => x.IsActive, true);

            if (!returnHidden)
                filter.Add(x => x.IsHidden, false);

            return connector.FetchAll(filter).ToArray();
        }

        /// <summary>
        /// Selects all by parent Async.
        /// </summary>
        /// <param name="parentID">The parent ID.</param>
        /// <param name="returnHidden">if set to <c>true</c> [return hidden].</param>
        /// <returns></returns>
        public static async Task<Gallery[]> SelectAllByParentAsync(int parentID, bool returnHidden)
        {
            var connector = ConnectorFactory.CreateConnector<Gallery>();
            var filter = connector.CreateQuery();
            filter.AddOrder(x => x.Name);
            filter.Add(x => x.ParentID, parentID);
            filter.Add(x => x.IsActive, true);

            if (!returnHidden)
            {
                filter.Add(x => x.IsHidden, false);
            }

            var result = await connector.FetchAllAsync(filter);
            return result.ToArray();
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
        /// Selects all by parent.
        /// </summary>
        /// <param name="parentID">The parent ID.</param>
        /// <returns></returns>
        public static async Task<Gallery[]> SelectAllByParentAsync(int parentID)
        {
            return await SelectAllByParentAsync(parentID, false);
        }

        /// <summary>
        /// Selects the all_ by base.
        /// </summary>
        /// <param name="baseGalleryID">The base gallery ID.</param>
        /// <returns></returns>
        public static Gallery[] SelectAllByBase(int baseGalleryID)
        {
            var connector = ConnectorFactory.CreateConnector<Gallery>();
            var filter = connector.CreateQuery();
            filter.AddOrder(x => x.Name);
            filter.Add(x => x.BaseGalleryID, baseGalleryID);
            filter.Add(x => x.IsActive, true);

            return connector.FetchAll(filter).ToArray();
        }

        /// <summary>
        /// Selects the all_ by base.
        /// </summary>
        /// <param name="baseGalleryID">The base gallery ID.</param>
        /// <returns></returns>
        public static async Task<Gallery[]> SelectAllByBaseAsync(int baseGalleryID)
        {
            var connector = ConnectorFactory.CreateConnector<Gallery>();
            var filter = connector.CreateQuery();
            filter.AddOrder(x => x.Name);
            filter.Add(x => x.BaseGalleryID, baseGalleryID);
            filter.Add(x => x.IsActive, true);

            var result = await connector.FetchAllAsync(filter);
            return result.ToArray();
        }

        /// <summary>
        /// Insert an implementation record.
        /// </summary>
        /// <returns></returns>
        public bool Insert()
        {
            CleanPath();
            this.Name = GetPageNameProposal(this.ParentID.GetValueOrDefault(), this.Name);

            var connector = ConnectorFactory.CreateConnector<Gallery>();

            try
            {
                connector.Insert(this);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Insert an implementation record.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> InsertAsync()
        {
            CleanPath();
            this.Name = GetPageNameProposal(this.ParentID.GetValueOrDefault(), this.Name);

            var connector = ConnectorFactory.CreateConnector<Gallery>();

            try
            {
                await connector.InsertAsync(this);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Gets the page name proposal.
        /// </summary>
        /// <param name="galleryID">The gallery ID.</param>
        /// <param name="gallery">The gallery.</param>
        /// <returns></returns>
        private string GetPageNameProposal(int galleryID, string gallery)
        {
            string nameProposal = Utility.GlobalRegularExpression.Implement.ReplaceNotAcceptableFilenameCharacter.Replace(gallery, string.Empty);

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
                var connector = ConnectorFactory.CreateConnector<Gallery>();
                var filter = connector.CreateQuery();
                filter.AddParameter("@thisId", ID);

                return connector.ExecuteScalar<int>("SELECT (SELECT COUNT(*) FROM [wim_Galleries] WHERE [Gallery_Gallery_Key] = @thisId) + COUNT(*) FROM [wim_Assets] WHERE [Asset_Gallery_Key] = @thisId)", filter);
            }
        }

        private bool IsGalleryAlreadyTaken(int galleryID, string gallery)
        {
            var connector = ConnectorFactory.CreateConnector<Gallery>();
            var filter = connector.CreateQuery();
            filter.AddParameter("@galleryId", galleryID);
            filter.AddParameter("@galleryName", gallery);

            var count = connector.ExecuteScalar<int>("SELECT COUNT(*) FROM [wim_Galleries] WHERE [Gallery_Gallery_Key] = @galleryId AND [Gallery_Name] = @galleryName", filter);
            return (count > 0);
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
            get { return null; }
        }

        #endregion iExportable Members

        public Gallery Parent
        {
            get
            {
                if (ParentID.HasValue)
                    return SelectOne(ParentID.Value);
                else
                    return null;
            }
        }

        /// <summary>
        /// Validates the access right.
        /// </summary>
        /// <param name="galleries">The galleries.</param>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        public static Gallery[] ValidateAccessRight(Gallery[] galleries, IApplicationUser user)
        {
            return (from item in galleries join relation in SelectAllAccessible(user) on item.ID equals relation.ID select item).ToArray();
        }

        /// <summary>
        /// Selects all accessible.
        /// </summary>
        /// <param name="role">The user role.</param>
        /// <returns></returns>
        public static Gallery[] SelectAllAccessible(IApplicationRole role)
        {
            Gallery[] galleries = null;
            if (!role.All_Galleries)
            {
                if (role.IsAccessGallery)
                {
                    galleries = (
                        from item in SelectAll()
                        join relation in RoleRight.SelectAll(role.ID, RoleRightType.Gallery) on item.ID equals relation.ItemID
                        select item).ToArray();
                }
                else
                {
                    var acl = (
                        from item in SelectAll()
                        join relation in RoleRight.SelectAll(role.ID, RoleRightType.Gallery) on item.ID equals relation.ItemID
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
        /// Selects all accessible.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        public static Gallery[] SelectAllAccessible(IApplicationUser user)
        {
            return SelectAllAccessible(user.SelectRole());
        }


        /// <summary>
        /// Validates the access right.
        /// </summary>
        /// <param name="galleries">The galleries.</param>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        public static async Task<ICollection<Gallery>> ValidateAccessRightAsync(Gallery[] galleries, IApplicationUser user)
        {
            return (from item in galleries join relation in await SelectAllAccessibleAsync(user).ConfigureAwait(false) on item.ID equals relation.ID select item).ToList();
        }

        /// <summary>
        /// Validates the access right.
        /// </summary>
        /// <param name="galleries">The galleries.</param>
        /// <param name="role">The user role.</param>
        /// <returns></returns>
        public static async Task<ICollection<Gallery>> ValidateAccessRightAsync(Gallery[] galleries, IApplicationRole role)
        {
            return (from item in galleries join relation in await SelectAllAccessibleAsync(role).ConfigureAwait(false) on item.ID equals relation.ID select item).ToList();
        }

        /// <summary>
        /// Selects all accessible.
        /// </summary>
        /// <param name="role">The user role.</param>
        /// <returns></returns>
        public static async Task<ICollection<Gallery>> SelectAllAccessibleAsync(IApplicationRole role)
        {
            List<Gallery> galleries = null;
            if (!role.All_Galleries)
            {
                if (role.IsAccessGallery)
                {
                    galleries = (
                        from item in await SelectAllAsync().ConfigureAwait(false)
                        join relation in await RoleRight.SelectAllAsync(role.ID, RoleRightType.Gallery).ConfigureAwait(false) on item.ID equals relation.ItemID
                        select item).ToList();
                }
                else
                {
                    var acl = (
                        from item in await SelectAllAsync().ConfigureAwait(false)
                        join relation in await RoleRight.SelectAllAsync(role.ID, RoleRightType.Gallery).ConfigureAwait(false) on item.ID equals relation.ItemID
                        into combination
                        from relation in combination.DefaultIfEmpty()
                        select new { ID = item.ID, HasAccess = relation == null });

                    galleries = (
                        from item in acl
                        join relation in await SelectAllAsync().ConfigureAwait(false) on item.ID equals relation.ID
                        where item.HasAccess
                        select relation).ToList();
                }
            }
            else
            {
                galleries = (await SelectAllAsync().ConfigureAwait(false)).ToList();
            }

            return galleries.ToList();
        }

        /// <summary>
        /// Selects all accessible.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        public static async Task<ICollection<Gallery>> SelectAllAccessibleAsync(IApplicationUser user)
        {
            return await SelectAllAccessibleAsync(await user.SelectRoleAsync()).ConfigureAwait(false);
        }

        /// <summary>
        /// Update an implementaion record.
        /// </summary>
        /// <returns></returns>
        public bool Update()
        {
            var connector = ConnectorFactory.CreateConnector<Gallery>();
            try
            {
                CleanPath();
                connector.Update(this);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Update an implementaion record Async.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> UpdateAsync()
        {
            var connector = ConnectorFactory.CreateConnector<Gallery>();
            try
            {
                CleanPath();
                await connector.UpdateAsync(this);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Updates the AssetCount property in the database by counting all assets in this gallery
        /// </summary>
        public void UpdateCount()
        {
            var connector = ConnectorFactory.CreateConnector<Gallery>();
            var filter = connector.CreateQuery();
            filter.AddParameter("@thisId", ID);

            connector.ExecuteNonQuery($@"
UPDATE [wim_Galleries]
SET [Gallery_Count] = (SELECT COUNT(*) FROM [wim_Assets] WHERE [Asset_Gallery_Key] = [Gallery_Key])
WHERE [Gallery_Key] = @thisId", filter);
			connector.Cache?.FlushRegion(connector.CacheRegion);
			
            this.AssetCount = connector.ExecuteScalar<int>("SELECT COUNT(*) FROM [dbo].[wim_Assets] WHERE [Asset_Gallery_Key] = @thisId", filter);
        }

        /// <summary>
        /// Updates the AssetCount property in the database by counting all assets in this gallery
        /// </summary>
        public async Task UpdateCountAsync()
        {
            var connector = ConnectorFactory.CreateConnector<Gallery>();
            var filter = connector.CreateQuery();
            filter.AddParameter("@thisId", ID);

            await connector.ExecuteNonQueryAsync($@"
UPDATE [wim_Galleries]
SET [Gallery_Count] = (SELECT COUNT(*) FROM [dbo].[wim_Assets] WHERE [Asset_Gallery_Key] = [Gallery_Key])
WHERE [Gallery_Key] = @thisId", filter);
			connector.Cache?.FlushRegion(connector.CacheRegion);

            this.AssetCount = await connector.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM [wim_Assets] WHERE [Asset_Gallery_Key] = @thisId", filter);
        }

        /// <summary>
        /// Cleans the path.
        /// </summary>
        private void CleanPath()
        {
            Regex MustMatch = new Regex(@"^[^\|\?\:\*""<>]*$", RegexOptions.IgnoreCase);
            if (!MustMatch.IsMatch(this.CompletePath))
            {
                string[] split = this.CompletePath.Split('/');
                string candidate = "";
                foreach (string item in split)
                {
                    string tmp = Utility.GlobalRegularExpression.Implement.ReplaceNotAcceptableFilenameCharacter.Replace(item, string.Empty);
                    candidate += candidate == "/" ? tmp : string.Concat("/", tmp);
                }
                this.CompletePath = candidate;
            }
        }

        /// <summary>
        /// Deactivates all galleries in the set path (also all assets).
        /// </summary>
        /// <param name="completePath">The complete path.</param>
        public static void Deactivate(string completePath)
        {
            var connector = ConnectorFactory.CreateConnector<Gallery>();

            var filter = connector.CreateQuery();
            filter.AddParameter("@completePath", completePath.Replace("'", ""));

            connector.ExecuteNonQuery("UPDATE [dbo].[wim_Galleries] SET [Gallery_IsActive] = 0 WHERE [Gallery_CompletePath] LIKE @completePath + '%'", filter);
            connector.ExecuteNonQuery("UPDATE [dbo].[wim_Assets] SET [Asset_IsActive] = 0 WHERE [Asset_Gallery_Key] IN (SELECT [Gallery_Key] FROM [wim_Galleries] WHERE [Gallery_CompletePath] LIKE @completePath + '%')", filter);
            connector.Cache?.FlushRegion(connector.CacheRegion);
        }

        /// <summary>
        /// Deactivates all galleries in the set path (also all assets).
        /// </summary>
        /// <param name="completePath">The complete path.</param>
        public static async Task DeactivateAsync(string completePath)
        {
            var connector = ConnectorFactory.CreateConnector<Gallery>();
            var filter = connector.CreateQuery();
            filter.AddParameter("@completePath", completePath.Replace("'", ""));

            await connector.ExecuteNonQueryAsync("UPDATE [dbo].[wim_Galleries] SET [Gallery_IsActive] = 0 WHERE [Gallery_CompletePath] LIKE @completePath + '%'", filter);
            await connector.ExecuteNonQueryAsync("UPDATE [dbo].[wim_Assets] SET [Asset_IsActive] = 0 WHERE [Asset_Gallery_Key] IN (SELECT [Gallery_Key] FROM [wim_Galleries] WHERE [Gallery_CompletePath] LIKE @completePath + '%')", filter);
            connector.Cache?.FlushRegion(connector.CacheRegion);
        }

        /// <summary>
        /// Updates the children.
        /// </summary>
        /// <param name="newFolderCompletePath">The new folder complete path.</param>
        /// <returns></returns>
        public bool UpdateChildren(string newFolderCompletePath)
        {
            var connector = ConnectorFactory.CreateConnector<Gallery>();
            var filter = connector.CreateQuery();
            filter.AddParameter("@oldCompletePath", CompletePath.Replace("'", ""));
            filter.AddParameter("@newCompletePath", newFolderCompletePath.Replace("'", ""));

            try
            {
                connector.ExecuteNonQuery("UPDATE [dbo].[wim_Galleries] SET [Gallery_CompletePath] = REPLACE (Gallery_CompletePath, @oldCompletePath , @newCompletePath) WHERE [Gallery_CompletePath] LIKE @oldCompletePath + '%'", filter);
                connector.Cache?.FlushRegion(connector.CacheRegion);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Updates the children Async.
        /// </summary>
        /// <param name="newFolderCompletePath">The new folder complete path.</param>
        /// <returns></returns>
        public async Task<bool> UpdateChildrenAsync(string newFolderCompletePath)
        {
            var connector = ConnectorFactory.CreateConnector<Gallery>();
            var filter = connector.CreateQuery();
            filter.AddParameter("@oldCompletePath", CompletePath.Replace("'", "''"));
            filter.AddParameter("@newCompletePath", newFolderCompletePath.Replace("'", "''"));

            try
            {
                await connector.ExecuteNonQueryAsync("UPDATE [wim_Galleries] SET [Gallery_CompletePath] = REPLACE (Gallery_CompletePath, @oldCompletePath , @newCompletePath) WHERE [Gallery_CompletePath] LIKE @oldCompletePath + '%'", filter);
                connector.Cache?.FlushRegion(connector.CacheRegion);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Delete an implementation record.
        /// </summary>
        /// <returns></returns>
        public bool _DeleteForTest()
        {

            var connector = ConnectorFactory.CreateConnector<Gallery>();
            var filter = connector.CreateQuery();
            filter.AddParameter("@oldCompletePath", CompletePath.Replace("'", ""));         

            try
            {
                // Delete the assests
                connector.ExecuteNonQuery("DELETE FROM [dbo].[wim_Assets] where [Asset_Gallery_Key] IN (SELECT [Gallery_Key] FROM [dbo].[wim_Galleries] WHERE [Gallery_CompletePath] LIKE @oldCompletePath  + '%')", filter);
				connector.Cache?.FlushRegion(connector.CacheRegion);

                // Delete the gallery
                connector.Delete(this);

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}