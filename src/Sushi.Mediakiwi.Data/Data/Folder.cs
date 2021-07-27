using Sushi.Mediakiwi.Data.MicroORM;
using Sushi.MicroORM;
using Sushi.MicroORM.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Data
{
    /// <summary>
    /// Represents a Folder entity.
    /// </summary>
    [DataMap(typeof(FolderMap))]
    public class Folder : IExportable
    {
        public class FolderMap : DataMap<Folder>
        {   
            public FolderMap()
            {         
                Table("wim_Folders");
             
                Id(x => x.ID, "Folder_Key").Identity();
                Map(x => x.GUID, "Folder_GUID");
                Map(x => x.ParentID, "Folder_Folder_Key");
                Map(x => x.SortOrderMethod, "Folder_SortOrderMethod");
                Map(x => x.MasterID, "Folder_Master_Key");
                Map(x => x.IsVisible, "Folder_IsVisible");
                Map(x => x.SiteID, "Folder_Site_Key");
                Map(x => x.Type, "Folder_Type");
                Map(x => x.Name, "Folder_Name").Length(50);
                Map(x => x.Description, "Folder_Description").Length(1024);
                Map(x => x.CompletePath, "Folder_CompletePath").Length(1000);
                Map(x => x.Changed, "Folder_Created");                
            }
        }

        #region Properties

        public virtual bool IsNewInstance
        {
            get { return this.ID == 0; }
        }

        /// <summary>
        /// Gets or sets the ID.
        /// </summary>
        /// <value>The ID.</value>
        public int ID { get; set; }

        private Guid m_GUID;

        /// <summary>
        /// Unique identifier used for import/export
        /// </summary>
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

        private int? m_ParentID;

        /// <summary>
        /// Gets or sets the parent ID.
        /// </summary>
        /// <value>The parent ID.</value>
        public int? ParentID
        {
            get { return m_ParentID; }
            set
            {
                m_ParentID = value;
                m_Parent = null;
            }
        }

        /// <summary>
        /// Gets or sets the SortOrderMethod
        /// </summary>
        /// <value>The Folder_SortOrderMethod</value>
        public int? SortOrderMethod { get; set; } = 5;

        private Folder m_Parent;

        /// <summary>
        /// Gets the parent.
        /// </summary>
        /// <value>The parent.</value>
        public Folder Parent
        {
            get
            {
                if (m_Parent == null)
                {
                    if (this.ParentID.HasValue)
                        m_Parent = SelectOne(ParentID.Value);
                    else
                        m_Parent = new Folder();
                }
                return m_Parent;
            }
        }

        private int m_Level;

        /// <summary>
        /// Gets or sets the level.
        /// </summary>
        /// <value>The level.</value>
        public int Level
        {
            get
            {
                if (!string.IsNullOrEmpty(CompletePath))
                {
                    if (CompletePath == "/")
                        m_Level = 0;
                    else // if (m_Level == 0)
                        m_Level = CompletePath.Split('/').Length - 2;
                }
                return m_Level;
            }
            set { m_Level = value; }
        }

        /// <summary>
        /// Gets or sets the master ID.
        /// </summary>
        /// <value>The master ID.</value>
        public int? MasterID { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is visible.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is visible; otherwise, <c>false</c>.
        /// </value>
        public bool IsVisible { get; set; }

        /// <summary>
        /// Gets or sets the site ID.
        /// </summary>
        /// <value>The site ID.</value>
        public int SiteID { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        public FolderType Type { get; set; }

        private Site _site;
        /// <summary>
        /// Sites this instance.
        /// </summary>
        /// <returns></returns>
        public Site Site
        {
            get
            {
                if (_site == null)
                    _site = Site.SelectOne(this.SiteID);
                return _site;
            }
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        public string Description { get; set; }

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
                m_CompletePath = value;
                m_CompleteCleanPath = null;
            }
        }

        private string m_CompleteCleanPath;

        /// <summary>
        /// Completes the clean path.
        /// </summary>
        /// <returns></returns>
        public string CompleteCleanPath()
        {
            if (this.CompletePath == "/")
                return "/";

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

        private DateTime m_Changed;

        /// <summary>
        /// Gets or sets the changed.
        /// </summary>
        /// <value>The changed.</value>
        public DateTime Changed
        {
            get
            {
                if (this.m_Changed == DateTime.MinValue)
                    this.m_Changed = Common.DatabaseDateTime;
                return m_Changed;
            }
            set { m_Changed = value; }
        }

        /// <summary>
        /// Gets the child count.
        /// </summary>
        /// <value>The child count.</value>
        public int ChildCount
        {
            get
            {
                var connector = ConnectorFactory.CreateConnector<Folder>();
                var filter = connector.CreateDataFilter();
                filter.AddParameter("@thisId", ID);

                return connector.ExecuteScalar<int>(@"
SELECT COUNT(*) + (SELECT COUNT(*) FROM [Wim_Pages] WHERE [Page_Folder_Key] IN (SELECT [Folder_Key] FROM [Wim_Folders] WHERE ([Folder_Master_Key] = @thisId OR [Folder_Key] = @thisId))) + (SELECT COUNT(*) FROM [Wim_Folders] WHERE [Folder_Folder_Key] IN (SELECT [Folder_Key] FROM [Wim_Folders] WHERE ([Folder_Master_Key] = @thisId OR [Folder_Key] = @thisId)))
FROM [Wim_ComponentLists] WHERE [ComponentList_Folder_Key] in (SELECT [Folder_Key] FROM [Wim_Folders] WHERE ([Folder_Master_Key] = @thisId OR [Folder_Key] = @thisId))", filter);
            }
        }

        public DateTime? Updated
        {
            get { return null; }
        }

        #endregion Properties

        /// <summary>
        /// Delete an implementation record.
        /// </summary>
        /// <returns></returns>
        public bool Delete()
        {
            var connector = ConnectorFactory.CreateConnector<Folder>();
            var filter = connector.CreateDataFilter();
            filter.AddParameter("@thisId", ID);
            filter.AddSql("([Folder_Key] = @thisId OR [Folder_Master_Key] = @thisId)");
            try
            {
                connector.Delete(filter);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Delete an implementation record Async.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> DeleteAsync()
        {
            var connector = ConnectorFactory.CreateConnector<Folder>();
            var filter = connector.CreateDataFilter();
            filter.AddParameter("@thisId", ID);
            filter.AddSql("([Folder_Key] = @thisId OR [Folder_Master_Key] = @thisId)");
            try
            {
                await connector.DeleteAsync(filter);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Folder Clone()
        {
            Folder clone = new Folder();
            Utility.ReflectProperty(this, clone);
            clone.GUID = Guid.NewGuid();
            clone.ID = 0;
            return clone;
        }

        /// <summary>
        /// Save a database entity. This method will indentify the need for an INSERT or an UPDATE.
        /// </summary>
        /// <returns></returns>
        public bool Save()
        {
            var connector = ConnectorFactory.CreateConnector<Folder>();
            try
            {
                connector.Save(this);
                return true;
            }
            catch (Exception exc)
            {
                throw new Exception("SQL mist misschien: alter table wim_Folders add column Folder_SortOrderMethod int ", exc);
            }
        }

        /// <summary>
        /// Save a database entity Async. This method will indentify the need for an INSERT or an UPDATE.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> SaveAsync()
        {
            var connector = ConnectorFactory.CreateConnector<Folder>();
            try
            {
                await connector.SaveAsync(this);
                return true;
            }
            catch (Exception exc)
            {
                throw new Exception("SQL mist misschien: alter table wim_Folders add column Folder_SortOrderMethod int ", exc);
            }
        }

        /// <summary>
        /// Selects a single Folder by the Identifier.
        /// </summary>
        /// <param name="ID">The Folder Identifier.</param>
        /// <returns></returns>
        public static Folder SelectOne(string path, int site)
        {
            var connector = ConnectorFactory.CreateConnector<Folder>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.CompletePath, path);
            filter.Add(x => x.SiteID, site);

            return connector.FetchSingle(filter);
        }

        /// <summary>
        /// Selects a single Folder by the complete path
        /// </summary>
        /// <param name="ID">The Folder Identifier.</param>
        /// <returns></returns>
        public static async Task<Folder> SelectOneAsync(string path, int site)
        { 
            var connector = ConnectorFactory.CreateConnector<Folder>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.CompletePath, path);
            filter.Add(x => x.SiteID, site);

            return await connector.FetchSingleAsync(filter);
        }

        /// <summary>
        /// Selects a single Folder by the Identifier.
        /// </summary>
        /// <param name="ID">The Folder Identifier.</param>
        /// <returns></returns>
        public static Folder SelectOne(int ID)
        {
            var connector = ConnectorFactory.CreateConnector<Folder>();
            return connector.FetchSingle(ID);
        }

        /// <summary>
        /// Selects a single Folder by the Identifier Async.
        /// </summary>
        /// <param name="ID">The Folder Identifier.</param>
        /// <returns></returns>
        public static async Task<Folder> SelectOneAsync(int ID)
        {
            var connector = ConnectorFactory.CreateConnector<Folder>();
            return await connector.FetchSingleAsync(ID);
        }

        /// <summary>
        /// Selects a single Folder by the GUID.
        /// </summary>
        /// <param name="guid">The Folder GUID.</param>
        /// <returns></returns>
        public static Folder SelectOne(Guid guid)
        {
            var connector = ConnectorFactory.CreateConnector<Folder>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.GUID, guid);

            return connector.FetchSingle(filter);
        }

        /// <summary>
        /// Selects a single Folder by the GUID Async.
        /// </summary>
        /// <param name="guid">The Folder GUID.</param>
        /// <returns></returns>
        public static async Task<Folder> SelectOneAsync(Guid guid)
        {
            var connector = ConnectorFactory.CreateConnector<Folder>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.GUID, guid);

            return await connector.FetchSingleAsync(filter);
        }

        /// <summary>
        /// Updates the children (change folder name).
        /// </summary>
        /// <param name="newFolderCompletePath">The new folder complete path.</param>
        /// <returns></returns>
        public bool UpdateChildren(string newFolderCompletePath)
        {
            //[MR:03-01-2020] TEST this method !
            var connector = ConnectorFactory.CreateConnector<Folder>();
            var filter = connector.CreateDataFilter();
            filter.AddParameter("@typeId", (int)Type);
            filter.AddParameter("@completePath", CompletePath);
            filter.AddParameter("@newPath", newFolderCompletePath);
            filter.AddParameter("@siteId", SiteID);

            connector.ExecuteNonQuery(@"
                UPDATE [Wim_Folders]
                SET [Folder_CompletePath] = REPLACE ([Folder_CompletePath], '@completePath', '@newPath')
                WHERE [Folder_Type] = @typeId AND [Folder_CompletePath] like '@completePath%'
                    and Folder_Site_Key = @siteId"
                , filter);
		    connector.Cache?.FlushRegion(connector.CacheRegion);
            return true;
        }

        /// <summary>
        /// Updates the children (change folder name) Async.
        /// </summary>
        /// <param name="newFolderCompletePath">The new folder complete path.</param>
        /// <returns></returns>
        public async Task<bool> UpdateChildrenAsync(string newFolderCompletePath)
        {
            //[MR:03-01-2020] TEST this method !
            var connector = ConnectorFactory.CreateConnector<Folder>();
            var filter = connector.CreateDataFilter();
            filter.AddParameter("@typeId", (int)Type);
            filter.AddParameter("@completePath", CompletePath);
            filter.AddParameter("@newPath", newFolderCompletePath);
            filter.AddParameter("@siteId", SiteID);

            await connector.ExecuteNonQueryAsync(@"
                UPDATE [Wim_Folders]
                SET [Folder_CompletePath] = REPLACE ([Folder_CompletePath], '@completePath', '@newPath')
                WHERE [Folder_Type] = @typeId AND [Folder_CompletePath] like '@completePath%'
                    and Folder_Site_Key = @siteId"
                 , filter);
			connector.Cache?.FlushRegion(connector.CacheRegion);
            return true;
        }

        /// <summary>
        /// Verifies and sets the Complete Path
        /// </summary>
        public static void VerifyCompletePath()
        {
            var folders = SelectAll();
            foreach (var folder in folders)
            {
                string path = folder.CompletePath;
                if (folder.Parent == null || folder.Parent.IsNewInstance)
                    continue;

                string parentPath = folder.Parent.CompletePath;
                if (folder.Parent.Level == 0)
                    parentPath = "/";

                string shouldBePath = string.Concat(parentPath, folder.Name, "/");

                if (!path.Equals(shouldBePath))
                {
                    folder.CompletePath = shouldBePath;
                    folder.Save();
                }
            }
        }

        /// <summary>
        /// Verifies and sets the Complete Path
        /// </summary>
        public static async Task VerifyCompletePathAsync()
        {
            var folders = await SelectAllAsync();
            foreach (var folder in folders)
            {
                string path = folder.CompletePath;
                if (folder.Parent == null || folder.Parent.IsNewInstance)
                    continue;

                string parentPath = folder.Parent.CompletePath;
                if (folder.Parent.Level == 0)
                    parentPath = "/";

                string shouldBePath = string.Concat(parentPath, folder.Name, "/");

                if (!path.Equals(shouldBePath))
                {
                    folder.CompletePath = shouldBePath;
                    await folder.SaveAsync();
                }
            }
        }

        /// <summary>
        /// Selects a single Folder by SiteID and Type.
        /// </summary>
        /// <param name="siteID">The site ID.</param>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static Folder SelectOneBySite(int siteID, FolderType type)
        {
            if (type == FolderType.Administration)
                siteID = 0;

            var connector = ConnectorFactory.CreateConnector<Folder>();
            var filter = connector.CreateDataFilter();

            filter.Add(x => x.Type, type);
            if (siteID > 0)
                filter.Add(x => x.SiteID, siteID);

            return connector.FetchSingle(filter);
        }

        /// <summary>
        /// Selects a single Folder by SiteID and Type Async.
        /// </summary>
        /// <param name="siteID">The site ID.</param>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static async Task<Folder> SelectOneBySiteAsync(int siteID, FolderType type)
        {
            if (type == FolderType.Administration)
                siteID = 0;

            var connector = ConnectorFactory.CreateConnector<Folder>();
            var filter = connector.CreateDataFilter();

            filter.Add(x => x.Type, type);
            if (siteID > 0)
                filter.Add(x => x.SiteID, siteID);

            return await connector.FetchSingleAsync(filter);
        }

        /// <summary>
        /// Selects a single Folder by SiteID and Master Folder.
        /// </summary>
        /// <param name="masterFolderID">The master folder ID.</param>
        /// <param name="siteID">The site ID.</param>
        /// <returns></returns>
        public static Folder SelectOne(int masterFolderID, int siteID)
        {
            var connector = ConnectorFactory.CreateConnector<Folder>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.SiteID, siteID);
            filter.Add(x => x.MasterID, masterFolderID);

            return connector.FetchSingle(filter);
        }

        /// <summary>
        /// Selects a single Folder by SiteID and Master Folder Async.
        /// </summary>
        /// <param name="masterFolderID">The master folder ID.</param>
        /// <param name="siteID">The site ID.</param>
        /// <returns></returns>
        public static async Task<Folder> SelectOneAsync(int masterFolderID, int siteID)
        {
            var connector = ConnectorFactory.CreateConnector<Folder>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.SiteID, siteID);
            filter.Add(x => x.MasterID, masterFolderID);

            return await connector.FetchSingleAsync(filter);
        }

        /// <summary>
        /// Select all Folders
        /// </summary>
        /// <returns></returns>
        public static Folder[] SelectAll()
        {
            var connector = ConnectorFactory.CreateConnector<Folder>();
            var filter = connector.CreateDataFilter();
            filter.AddOrder(x => x.MasterID);
            filter.AddOrder(x => x.Name);
            filter.AddOrder(x => x.CompletePath);

            return connector.FetchAll(filter).ToArray();
        }

        /// <summary>
        /// Select all Folders Async
        /// </summary>
        /// <returns></returns>
        public static async Task<Folder[]> SelectAllAsync()
        {
            var connector = ConnectorFactory.CreateConnector<Folder>();
            var filter = connector.CreateDataFilter();
            filter.AddOrder(x => x.MasterID);
            filter.AddOrder(x => x.Name);
            filter.AddOrder(x => x.CompletePath);

            var result = await connector.FetchAllAsync(filter);
            return result.ToArray();
        }

        /// <summary>
        /// Select all folders based on a list of ID"s
        /// </summary>
        /// <returns></returns>
        public static Folder[] SelectAll(int[] folderIDs)
        {
            var connector = ConnectorFactory.CreateConnector<Folder>();
            var filter = connector.CreateDataFilter();
            filter.AddOrder(x => x.MasterID);
            filter.AddOrder(x => x.Name);
            filter.AddOrder(x => x.CompletePath);
            filter.Add(x => x.ID, folderIDs, ComparisonOperator.In);

            return connector.FetchAll(filter).ToArray();
        }

        /// <summary>
        /// Select all folders based on a list of ID"s Async
        /// </summary>
        /// <returns></returns>
        public static async Task<Folder[]> SelectAllAsync(int[] folderIDs)
        {
            var connector = ConnectorFactory.CreateConnector<Folder>();
            var filter = connector.CreateDataFilter();
            filter.AddOrder(x => x.MasterID);
            filter.AddOrder(x => x.Name);
            filter.AddOrder(x => x.CompletePath);
            filter.Add(x => x.ID, folderIDs, ComparisonOperator.In);

            var result = await connector.FetchAllAsync(filter);
            return result.ToArray();
        }

        /// <summary>
        /// Validates the access right.
        /// </summary>
        /// <param name="folders">The folders.</param>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        public static Folder[] ValidateAccessRight(Folder[] folders, IApplicationUser user)
        {
            return (from item in folders join relation in SelectAllAccessible(user) on item.ID equals relation.ID select item).ToArray();
        }

        /// <summary>
        /// Validates the access right.
        /// </summary>
        /// <param name="folders">The folders.</param>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        public static async Task<Folder[]> ValidateAccessRightAsync(Folder[] folders, IApplicationUser user)
        {
            var relations = await SelectAllAccessibleAsync(user).ConfigureAwait(false);
            return (from item in folders join relation in relations on item.ID equals relation.ID select item).ToArray();
        }

        /// <summary>
        /// Determines whether [has role access] [the specified role id].
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns>
        /// 	<c>true</c> if [has role access] [the specified role id]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasRoleAccess(IApplicationUser user)
        {
            if (this.CompletePath == "/")
                return true;

            if (this.ID == 0 || user.Role().All_Folders)
                return true;

            var selection = from item in SelectAllAccessible(user) where item.ID == this.ID select item;
            bool xs = selection.Count() == 1;
            return xs;
        }

        /// <summary>
        /// Selects all accessible.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        public static Folder[] SelectAllAccessible(IApplicationUser user)
        {
            Folder[] folders = null;
            if (!user.Role().All_Folders)
            {
                if (user.Role().IsAccessFolder)
                {
                    folders = (
                        from item in SelectAll()
                        join relation in RoleRight.SelectAll(user.Role().ID, RoleRightType.Folder) on item.ID equals relation.ItemID
                        select item).ToArray();
                }
                else
                {
                    var acl = (
                        from item in SelectAll()
                        join relation in RoleRight.SelectAll(user.Role().ID, RoleRightType.Folder) on item.ID equals relation.ItemID
                        into combination
                        from relation in combination.DefaultIfEmpty()
                        select new { ID = item.ID, HasAccess = relation == null });

                    folders = (
                        from item in acl
                        join relation in SelectAll() on item.ID equals relation.ID
                        where item.HasAccess
                        select relation).ToArray();
                }
            }
            else
                folders = SelectAll();
            return folders;
        }

        /// <summary>
        /// Selects all accessible.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        public static async Task<Folder[]> SelectAllAccessibleAsync(IApplicationUser user)
        {
            Folder[] folders = null;
            if (!user.Role().All_Folders)
            {
                if (user.Role().IsAccessFolder)
                {
                    folders = (
                        from item in await SelectAllAsync()
                        join relation in await RoleRight.SelectAllAsync(user.Role().ID, RoleRightType.Folder) on item.ID equals relation.ItemID
                        select item).ToArray();
                }
                else
                {
                    var acl = (
                        from item in await SelectAllAsync()
                        join relation in await RoleRight.SelectAllAsync(user.Role().ID, RoleRightType.Folder) on item.ID equals relation.ItemID
                        into combination
                        from relation in combination.DefaultIfEmpty()
                        select new { ID = item.ID, HasAccess = relation == null });

                    folders = (
                        from item in acl
                        join relation in await SelectAllAsync() on item.ID equals relation.ID
                        where item.HasAccess
                        select relation).ToArray();
                }
            }
            else
                folders = await SelectAllAsync();
            return folders;
        }

        /// <summary>
        /// Selects all.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="siteID">The site ID.</param>
        /// <returns></returns>
        public static Folder[] SelectAll(FolderType type, int siteID)
        {
            return SelectAll(type, siteID, null, false);
        }

        /// <summary>
        /// Selects all.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="siteID">The site ID.</param>
        /// <returns></returns>
        public static async Task<Folder[]> SelectAllAsync(FolderType type, int siteID)
        {
            return await SelectAllAsync(type, siteID, null, false);
        }

        /// <summary>
        /// Selects all.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="siteID">The site ID.</param>
        /// <param name="searchQuery">The search query.</param>
        /// <param name="isPartOfPath">if set to <c>true</c> [is part of path].</param>
        /// <returns></returns>
        public static Folder[] SelectAll(FolderType type, int siteID, string searchQuery, bool isPartOfPath)
        {
            IOrderedEnumerable<Folder> list = null;
            if (type != FolderType.Administration_Or_List)
            {
                list = (from item in SelectAll()
                        where item.SiteID == siteID && item.Type == type //&& item.Level > 0
                        orderby item.Site.Name, item.CompletePath
                        select item);
            }
            else
            {
                list = (from item in SelectAll()
                        where item.SiteID == siteID && (item.Type == FolderType.Administration || item.Type == FolderType.List)
                        orderby item.Site.Name, item.CompletePath
                        select item);
            }

            if (string.IsNullOrEmpty(searchQuery))
                return list.ToArray();

            if (isPartOfPath)
            {
                var search =
                    from item in list
                    where item.CompletePath != null && item.CompletePath.IndexOf(searchQuery, StringComparison.InvariantCultureIgnoreCase) > -1
                    select item;
                return search.ToArray();
            }
            else
            {
                var search =
                    from item in list
                    where item.Name != null && item.Name.IndexOf(searchQuery, StringComparison.InvariantCultureIgnoreCase) > -1
                    select item;
                return search.ToArray();
            }
        }

        /// <summary>
        /// Selects all.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="siteID">The site ID.</param>
        /// <param name="searchQuery">The search query.</param>
        /// <param name="isPartOfPath">if set to <c>true</c> [is part of path].</param>
        /// <returns></returns>
        public static async Task<Folder[]> SelectAllAsync(FolderType type, int siteID, string searchQuery, bool isPartOfPath)
        {
            IOrderedEnumerable<Folder> list = null;
            if (type != FolderType.Administration_Or_List)
            {
                list = (from item in await SelectAllAsync()
                        where item.SiteID == siteID && item.Type == type //&& item.Level > 0
                        orderby item.Site.Name, item.CompletePath
                        select item);
            }
            else
            {
                list = (from item in await SelectAllAsync()
                        where item.SiteID == siteID && (item.Type == FolderType.Administration || item.Type == FolderType.List)
                        orderby item.Site.Name, item.CompletePath
                        select item);
            }

            if (string.IsNullOrEmpty(searchQuery))
                return list.ToArray();

            if (isPartOfPath)
            {
                var search =
                    from item in list
                    where item.CompletePath != null && item.CompletePath.IndexOf(searchQuery, StringComparison.InvariantCultureIgnoreCase) > -1
                    select item;
                return search.ToArray();
            }
            else
            {
                var search =
                    from item in list
                    where item.Name != null && item.Name.IndexOf(searchQuery, StringComparison.InvariantCultureIgnoreCase) > -1
                    select item;
                return search.ToArray();
            }
        }


        /// <summary>
        /// Selects all Folders for a specific Site.
        /// </summary>
        /// <param name="siteID">The site ID.</param>
        /// <returns></returns>
        internal static Folder[] SelectAllForDeletion(int siteID)
        {
            var connector = ConnectorFactory.CreateConnector<Folder>();
            var filter = connector.CreateDataFilter();

            filter.AddOrder(x => x.Name, SortOrder.DESC);
            filter.AddOrder(x => x.CompletePath, SortOrder.DESC);
            filter.Add(x => x.SiteID, siteID);

            return connector.FetchAll(filter).ToArray();
        }

        /// <summary>
        /// Selects all Folders for a specific Site.
        /// </summary>
        /// <param name="siteID">The site ID.</param>
        /// <returns></returns>
        internal static async Task<Folder[]> SelectAllForDeletionAsync(int siteID)
        {
            var connector = ConnectorFactory.CreateConnector<Folder>();
            var filter = connector.CreateDataFilter();

            filter.AddOrder(x => x.Name, SortOrder.DESC);
            filter.AddOrder(x => x.CompletePath, SortOrder.DESC);
            filter.Add(x => x.SiteID, siteID);

            var result = await connector.FetchAllAsync(filter);
            return result.ToArray();
        }

        /// <summary>
        /// Select a single folder instance.
        /// </summary>
        /// <param name="parentID">The parent ID.</param>
        /// <param name="name">Name of the to be searched folder</param>
        /// <returns></returns>
        public static Folder SelectOne(int parentID, string name)
        {
            var connector = ConnectorFactory.CreateConnector<Folder>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.ParentID, parentID);
            filter.Add(x => x.Name, name);

            return connector.FetchSingle(filter);
        }

        /// <summary>
        /// Select a single folder instance Async.
        /// </summary>
        /// <param name="parentID">The parent ID.</param>
        /// <param name="name">Name of the to be searched folder</param>
        /// <returns></returns>
        public static async Task<Folder> SelectOneAsync(int parentID, string name)
        {
            var connector = ConnectorFactory.CreateConnector<Folder>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.ParentID, parentID);
            filter.Add(x => x.Name, name);

            return await connector.FetchSingleAsync(filter);
        }

        private static Folder FindSiteFolder(int folderID, int siteID)
        {
            var selected = (from item in SelectAll() where item.MasterID.HasValue && item.Type == FolderType.List && item.MasterID.Value == folderID select item);
            if (selected.Any() == false)
            {
                return null;
            }

            foreach (var item in selected)
            {
                if (item.SiteID == siteID)
                {
                    return item;
                }

                var tmp = FindSiteFolder(item.ID, siteID);
                if (tmp != null)
                {
                    return tmp;
                }
            }
            return null;
        }

        private static async Task<Folder> FindSiteFolderAsync(int folderID, int siteID)
        {
            var selected = (from item in await SelectAllAsync() where item.MasterID.HasValue && item.Type == FolderType.List && item.MasterID.Value == folderID select item);
            if (selected.Any() == false)
            {
                return null;
            }

            foreach (var item in selected)
            {
                if (item.SiteID == siteID)
                    return item;

                var tmp = await FindSiteFolderAsync(item.ID, siteID);
                if (tmp != null)
                    return tmp;
            }
            return null;
        }

        /// <summary>
        /// Selects the one child.
        /// </summary>
        /// <param name="folderID">The folder ID.</param>
        /// <param name="siteID">The site ID.</param>
        /// <returns></returns>
        public static Folder SelectOneChild(int folderID, int siteID)
        {
            Folder current = SelectOne(folderID);
            if (current.SiteID == siteID)
                return current;

            current = FindSiteFolder(folderID, siteID);
            if (current == null) return new Folder();
            return current;
        }

        /// <summary>
        /// Selects the one child.
        /// </summary>
        /// <param name="folderID">The folder ID.</param>
        /// <param name="siteID">The site ID.</param>
        /// <returns></returns>
        public static async Task<Folder> SelectOneChildAsync(int folderID, int siteID)
        {
            Folder current = await SelectOneAsync(folderID);
            if (current.SiteID == siteID)
                return current;

            current = await FindSiteFolderAsync(folderID, siteID);
            if (current == null) return new Folder();
            return current;
        }

        /// <summary>
        /// Insert an implementation record.
        /// </summary>
        /// <returns></returns>
        public bool Insert()
        {
            var connector = ConnectorFactory.CreateConnector<Folder>();
            if (this.IsNewInstance)
            {
                string name = this.Name;
                this.Name = GetPageNameProposal(this.ParentID.GetValueOrDefault(), this.Name);
                if (!this.Name.Equals(name))
                {
                    this.CompletePath = $"{this.CompletePath.Substring(0, this.CompletePath.Length - name.Length)}{this.Name}";
                }
            }

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
        /// Insert an implementation record Async.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> InsertAsync()
        {
            var connector = ConnectorFactory.CreateConnector<Folder>();
            if (this.IsNewInstance)
            {
                string name = this.Name;
                this.Name = await GetPageNameProposalAsync(this.ParentID.GetValueOrDefault(), this.Name);
                if (!this.Name.Equals(name))
                {
                    this.CompletePath = $"{this.CompletePath.Substring(0, this.CompletePath.Length - name.Length)}{this.Name}";
                }
            }

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
        /// <param name="folderID">The folder ID.</param>
        /// <param name="folder">The folder.</param>
        /// <returns></returns>
        public string GetPageNameProposal(int folderID, string folder)
        {
            if (folder == Common.FolderRoot || string.IsNullOrEmpty(folder))
                return folder;

            string nameProposal = Utility.GlobalRegularExpression.Implement.ReplaceNotAcceptableFilenameCharacter.Replace(folder, string.Empty);

            bool pageExistsInFolder = IsFolderAlreadyTaken(folderID, folder);
            int nameExtentionCount = 0;

            while (pageExistsInFolder)
            {
                nameExtentionCount++;
                nameProposal = string.Format("{0}_{1}", folder, nameExtentionCount);
                pageExistsInFolder = IsFolderAlreadyTaken(folderID, nameProposal);
            }
            return nameProposal;
        }

        /// <summary>
        /// Gets the page name proposal.
        /// </summary>
        /// <param name="folderID">The folder ID.</param>
        /// <param name="folder">The folder.</param>
        /// <returns></returns>
        public async Task<string> GetPageNameProposalAsync(int folderID, string folder)
        {
            if (folder == Common.FolderRoot || string.IsNullOrEmpty(folder))
                return folder;

            string nameProposal = Utility.GlobalRegularExpression.Implement.ReplaceNotAcceptableFilenameCharacter.Replace(folder, string.Empty);

            bool pageExistsInFolder = await IsFolderAlreadyTakenAsync(folderID, folder);
            int nameExtentionCount = 0;

            while (pageExistsInFolder)
            {
                nameExtentionCount++;
                nameProposal = string.Format("{0}_{1}", folder, nameExtentionCount);
                pageExistsInFolder = await IsFolderAlreadyTakenAsync(folderID, nameProposal);
            }
            return nameProposal;
        }

        /// <summary>
        /// Determines whether the name for a folder is already taken.
        /// </summary>
        /// <param name="folderID">The folder ID.</param>
        /// <param name="folder">The folder.</param>
        private bool IsFolderAlreadyTaken(int folderID, string folder)
        {
            var connector = ConnectorFactory.CreateConnector<Folder>();
            var filter = connector.CreateDataFilter();
            filter.AddParameter("@folderId", folderID);
            filter.AddParameter("@folderName", folder);
            var count = connector.ExecuteScalar<int>("SELECT COUNT(*) FROM [Wim_folders] WHERE [Folder_Folder_Key] = @folderId AND [Folder_Name] = @folderName", filter);

            return (count > 0);
        }

        /// <summary>
        /// Determines whether the name for a folder is already taken Async.
        /// </summary>
        /// <param name="folderID">The folder ID.</param>
        /// <param name="folder">The folder.</param>
        private async Task<bool> IsFolderAlreadyTakenAsync(int folderID, string folder)
        {
            var connector = ConnectorFactory.CreateConnector<Folder>();
            var filter = connector.CreateDataFilter();
            filter.AddParameter("@folderId", folderID);
            filter.AddParameter("@folderName", folder);
            var count = await connector.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM [Wim_folders] WHERE [Folder_Folder_Key] = @folderId AND [Folder_Name] = @folderName", filter);

            return (count > 0);
        }

        /// <summary>
        /// Select all childfolder in a folder
        /// Default: Return only published page folders
        /// </summary>
        /// <param name="folderID">The folder ID.</param>
        /// <returns></returns>
        public static Folder[] SelectAllByParent(int folderID)
        {
            return SelectAllByParent(folderID, FolderType.Undefined);
        }

        /// <summary>
        /// Select all childfolder in a folder
        /// Default: Return only published page folders
        /// </summary>
        /// <param name="folderID">The folder ID.</param>
        /// <returns></returns>
        public static async Task<Folder[]> SelectAllByParentAsync(int folderID)
        {
            return await SelectAllByParentAsync(folderID, FolderType.Undefined);
        }

        /// <summary>
        /// Select all childfolder in a folder
        /// Default: Return only published folders
        /// </summary>
        /// <param name="folderID">The folder ID.</param>
        /// <param name="type">Type of folder</param>
        /// <returns></returns>
        public static Folder[] SelectAllByParent(int folderID, FolderType type)
        {
            return SelectAllByParent(folderID, type, true);
        }

        /// <summary>
        /// Select all childfolder in a folder
        /// Default: Return only published folders
        /// </summary>
        /// <param name="folderID">The folder ID.</param>
        /// <param name="type">Type of folder</param>
        /// <returns></returns>
        public static async Task<Folder[]> SelectAllByParentAsync(int folderID, FolderType type)
        {
            return await SelectAllByParentAsync(folderID, type, true);
        }

        /// <summary>
        /// Select all childfolder in a folder
        /// </summary>
        /// <param name="folderID">The folder ID.</param>
        /// <param name="type">Type of folder</param>
        /// <param name="returnOnlyPublishedFolder">Should this method return only published folders?</param>
        /// <param name="roleID">The role ID.</param>
        /// <returns></returns>
        public static Folder[] SelectAllByParent(int folderID, FolderType type, bool returnOnlyPublishedFolder)
        {
            Folder[] folders;

            if (returnOnlyPublishedFolder)
            {
                folders = (
                    from item in SelectAll()
                    where
                        item.IsVisible && item.ParentID.HasValue && item.ParentID.Value == folderID
                    select item).ToArray();
            }
            else
            {
                folders = (
                    from item in SelectAll()
                    where item.ParentID.HasValue && item.ParentID.Value == folderID
                    select item).ToArray();
            }

            if (type != FolderType.Undefined)
            {
                folders = (
                    from item in folders
                    where
                        item.Type == type
                    select item).ToArray();
            }

            return folders;
        }

        /// <summary>
        /// Select all childfolder in a folder
        /// </summary>
        /// <param name="folderID">The folder ID.</param>
        /// <param name="type">Type of folder</param>
        /// <param name="returnOnlyPublishedFolder">Should this method return only published folders?</param>
        /// <param name="roleID">The role ID.</param>
        /// <returns></returns>
        public static async Task<Folder[]> SelectAllByParentAsync(int folderID, FolderType type, bool returnOnlyPublishedFolder)
        {
            Folder[] folders;

            if (returnOnlyPublishedFolder)
            {
                folders = (
                    from item in await SelectAllAsync()
                    where
                        item.IsVisible && item.ParentID.HasValue && item.ParentID.Value == folderID
                    select item).ToArray();
            }
            else
            {
                folders = (
                    from item in await SelectAllAsync()
                    where item.ParentID.HasValue && item.ParentID.Value == folderID
                    select item).ToArray();
            }

            if (type != FolderType.Undefined)
            {
                folders = (
                    from item in folders
                    where
                        item.Type == type
                    select item).ToArray();
            }

            return folders;
        }

        private static Folder GetParentFolder(Folder[] folders, int ID)
        {
            foreach (Folder folder in folders)
            {
                if (folder.ID == ID)
                    return folder;
            }
            return null;
        }

        /// <summary>
        /// Recursive backwards tracking towards the homepage
        /// </summary>
        /// <param name="folderID">The folder ID.</param>
        /// <returns></returns>
        public static Folder[] SelectAllByBackwardTrail(int folderID)
        {
            Folder folder = SelectOne(folderID);

            var connector = ConnectorFactory.CreateConnector<Folder>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.SiteID, folder.SiteID);
            filter.AddOrder(x => x.ParentID, SortOrder.DESC);

            List<Folder> listAll = connector.FetchAll(filter);
            List<Folder> list = new List<Folder>();
            list.Add(folder);

            Folder[] arr = listAll.ToArray();
            if (folder.ParentID.HasValue)
            {
                Folder innerFolder = GetParentFolder(arr, folder.ParentID.Value);
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
        /// Recursive backwards tracking towards the homepage
        /// </summary>
        /// <param name="folderID">The folder ID.</param>
        /// <returns></returns>
        public static async Task<Folder[]> SelectAllByBackwardTrailAsync(int folderID)
        {
            Folder folder = SelectOne(folderID);

            var connector = ConnectorFactory.CreateConnector<Folder>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.SiteID, folder.SiteID);
            filter.AddOrder(x => x.ParentID, SortOrder.DESC);

            List<Folder> listAll = await connector.FetchAllAsync(filter);
            List<Folder> list = new List<Folder>();
            list.Add(folder);

            Folder[] arr = listAll.ToArray();
            if (folder.ParentID.HasValue)
            {
                Folder innerFolder = GetParentFolder(arr, folder.ParentID.Value);
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
        /// Selects all uninherited folders.
        /// </summary>
        /// <param name="masterID">The master ID.</param>
        /// <param name="siteID">The site ID.</param>
        /// <param name="folderType">Type of the folder.</param>
        /// <returns></returns>
        public static Folder[] SelectAllUninherited(int masterID, int siteID, int folderType)
        {
            var connector = ConnectorFactory.CreateConnector<Folder>();
            var filter = connector.CreateDataFilter();
            filter.AddParameter("@folderType", folderType);
            filter.AddParameter("@siteId", siteID);
            filter.AddParameter("@masterId", masterID);

            List<Folder> list = connector.FetchAll($@"
    SELECT x.* FROM [wim_Folders] x join wim_Sites on Site_Key = [Folder_Site_Key]
    WHERE [Folder_Type] = @folderType 
    AND [Folder_Site_Key] = @masterId 
    AND NOT [Folder_Folder_Key] IS NULL 
    AND (SELECT COUNT(*) FROM [wim_Folders] WHERE [Folder_Site_Key] = @siteId AND [Folder_Master_Key] = x.[Folder_Key]) = 0
    ORDER BY [Site_Master_Key] ASC, [Site_Displayname] ASC, [Folder_CompletePath] ASC
", filter);

            return list.ToArray();
        }


        /// <summary>
        /// Selects all uninherited folders.
        /// </summary>
        /// <param name="masterID">The master ID.</param>
        /// <param name="siteID">The site ID.</param>
        /// <param name="folderType">Type of the folder.</param>
        /// <returns></returns>
        internal static async Task<Folder[]> SelectAllUninheritedAsync(int masterID, int siteID, int folderType)
        {
            var connector = ConnectorFactory.CreateConnector<Folder>();
            var filter = connector.CreateDataFilter();
            filter.AddParameter("@folderType", folderType);
            filter.AddParameter("@siteId", siteID);
            filter.AddParameter("@masterId", masterID);
            filter.AddParameter("@folderType", folderType);

            List<Folder> list = await connector.FetchAllAsync($@"
SELECT * FROM [wim_Folders] x 
WHERE [Folder_Type] = @folderType 
AND [Folder_Site_Key] = @masterId 
AND NOT [Folder_Folder_Key] IS NULL 
AND (SELECT COUNT(*) FROM [wim_Folders] WHERE [Folder_Site_Key] = @siteId AND [Folder_Master_Key] = x.[Folder_Key]) = 0
ORDER BY [Site_Master_Key] ASC, [Site_Displayname] ASC, [Folder_CompletePath] ASC
", filter);

            return list.ToArray();
        }



        /// <summary>
        /// Selects a folder level folder.
        /// </summary>
        /// <param name="folder">The folder.</param>
        /// <param name="level">The level (folder) which to return.</param>
        /// <returns></returns>
        public static Folder SelectOne(Folder folder, int level)  
        {
            if (level < 1) 
                return new Folder();

            if (folder.Name == CommonConfiguration.ROOT_FOLDER) 
                return folder;

            string[] split = folder.CompletePath.Split('/');
            int splitcount = split.Length;
            //  / = 2
            //  /level_01/ = 3
            //  /level_01/level_02/ = 4

            //  Is root
            if (splitcount == 2) 
                return new Folder();
            //  Non existant level
            if ((level - (splitcount - 2)) > 0) 
                return new Folder();
            //  Same level
            if ((level - (splitcount - 2)) == 0) 
                return folder;

            string searchPath = string.Format("/{0}/", split[level]);
            while (level > 1)
            {

                //searchPath = string.Format("/[*]{0}", searchPath);
                level--;
                searchPath = string.Format("/{0}{1}", split[level], searchPath);
            }
            searchPath = searchPath.Replace("//", "/");

            var list = (from item in SelectAll() where item.IsVisible && item.CompletePath == searchPath && item.Type == folder.Type && item.SiteID == folder.SiteID select item);
            return list.Any() == false ? new Folder() : list.ToArray()[0];
        }


        /// <summary>
        /// Selects a folder level folder.
        /// </summary>
        /// <param name="folder">The folder.</param>
        /// <param name="level">The level (folder) which to return.</param>
        /// <returns></returns>
        public static async Task<Folder> SelectOneAsync(Folder folder, int level)
        {
            if (level < 1)
                return new Folder();

            if (folder.Name == CommonConfiguration.ROOT_FOLDER)
                return folder;

            string[] split = folder.CompletePath.Split('/');
            int splitcount = split.Length;
            //  / = 2
            //  /level_01/ = 3
            //  /level_01/level_02/ = 4

            //  Is root
            if (splitcount == 2) 
                return new Folder();
            //  Non existant level
            if ((level - (splitcount - 2)) > 0) 
                return new Folder();
            //  Same level
            if ((level - (splitcount - 2)) == 0) 
                return folder;

            string searchPath = string.Format("/{0}/", split[level]);
            while (level > 1)
            {

                //searchPath = string.Format("/[*]{0}", searchPath);
                level--;
                searchPath = string.Format("/{0}{1}", split[level], searchPath);
            }
            searchPath = searchPath.Replace("//", "/");

            var list = (from item in await SelectAllAsync() where item.IsVisible && item.CompletePath == searchPath && item.Type == folder.Type && item.SiteID == folder.SiteID select item);
            return list.Any() == false ? new Folder() : list.ToArray()[0];
        }


    }
}