using System;
using System.Linq;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Sushi.Mediakiwi.Data.DalReflection;

namespace Sushi.Mediakiwi.Data
{
    /// <summary>
    /// Represents a Folder entity.
    /// </summary>
    [DatabaseTable("wim_Folders",
        Join = "join wim_Sites on Folder_Site_Key = Site_Key", Order = "Site_Master_Key ASC, Folder_CompletePath ASC, Site_DisplayName ASC")
    ]
    public class Folder : DatabaseEntity, iExportable
    {
        #region COMMENTED


        ///// <summary>
        ///// Gets the local cache directory.
        ///// </summary>
        ///// <value>The local cache directory.</value>
        //public string LocalCacheDirectory       //WEG
        //{
        //    get
        //    {
        //        string tmp = string.Concat("/repository/cache/", Sushi.Mediakiwi.Data.Site.SelectOne(this.SiteID).DefaultFolder, CompletePath);
        //        string replacement = Data.Environment.Current["SPACE_REPLACEMENT"];
        //        return System.Web.HttpContext.Current.Server.MapPath(Wim.Utility.AddApplicationPath(tmp.Replace(" ", replacement)));
        //    }
        //}

        ///// <summary>
        ///// Selects the all_ import export.
        ///// </summary>
        ///// <param name="portal">The portal.</param>
        ///// <returns></returns>
        //public static List<Folder> SelectAll_ImportExport(string portal)        //WEG
        //{
        //    Folder implement = new Folder();
        //    List<Folder> list = new List<Folder>();

        //    implement.SqlConnectionString = Sushi.Mediakiwi.Data.Common.GetPortal(portal).Connection;
        //    implement.ConnectionType = Sushi.Mediakiwi.Data.Common.GetPortal(portal).Type;

        //    foreach (object o in implement._SelectAll(null, false, "FolderImportExport", portal))
        //        list.Add((Folder)o);

        //    return list;
        //}

        ///// <summary>
        ///// Selects the one_ import export.
        ///// </summary>
        ///// <param name="portal">The portal.</param>
        ///// <param name="guid">The GUID.</param>
        ///// <param name="siteID">The site ID.</param>
        ///// <param name="completePath">The complete path.</param>
        ///// <returns></returns>
        //public static Folder SelectOne_ImportExport(string portal, Guid guid, int siteID, string completePath)      //WEG
        //{
        //    Folder implement = new Folder();

        //    implement.SqlConnectionString = Sushi.Mediakiwi.Data.Common.GetPortal(portal).Connection;
        //    implement.ConnectionType = Sushi.Mediakiwi.Data.Common.GetPortal(portal).Type;

        //    List<DatabaseDataValueColumn> where = new List<DatabaseDataValueColumn>();
        //    where.Add(new DatabaseDataValueColumn("Folder_Site_Key", SqlDbType.Int, siteID));
        //    where.Add(new DatabaseDataValueColumn("Folder_GUID", SqlDbType.UniqueIdentifier, guid));
        //    where.Add(new DatabaseDataValueColumn("Folder_CompletePath", SqlDbType.NVarChar, completePath, DatabaseDataValueConnectType.Or));

        //    return (Folder)implement._SelectOne(where);
        //}

        #endregion COMMENTED

        #region REPLICATED TO Sushi.Mediakiwi.Data.Standard


        /// <summary>
        /// Selects a folder level folder.
        /// </summary>
        /// <param name="folder">The folder.</param>
        /// <param name="level">The level (folder) which to return.</param>
        /// <returns></returns>
        public static Folder SelectOne(Sushi.Mediakiwi.Data.Folder folder, int level)   //NAAR STANDARD
        {
            if (level < 1) return new Folder();
            if (folder.Name == CommonConfiguration.siteRoot) return folder;

            string[] split = folder.CompletePath.Split('/');
            int splitcount = split.Length;
            //  / = 2
            //  /level_01/ = 3
            //  /level_01/level_02/ = 4

            //  Is root
            if (splitcount == 2) return new Folder();
            //  Non existant level
            if ((level - (splitcount - 2)) > 0) return new Folder();
            //  Same level
            if ((level - (splitcount - 2)) == 0) return folder;

            string searchPath = string.Format("/{0}/", split[level]);
            while (level > 1)
            {

                //searchPath = string.Format("/[*]{0}", searchPath);
                level--;
                searchPath = string.Format("/{0}{1}", split[level], searchPath);
            }
            searchPath = searchPath.Replace("//", "/");

            var list = (from item in SelectAll() where item.IsVisible && item.CompletePath == searchPath && item.Type == folder.Type && item.SiteID == folder.SiteID select item);
            return list.Count() == 0 ? new Folder() : list.ToArray()[0];
        }



        /// <summary>
        /// Selects all uninherited folders.
        /// </summary>
        /// <param name="masterID">The master ID.</param>
        /// <param name="siteID">The site ID.</param>
        /// <param name="folderType">Type of the folder.</param>
        /// <returns></returns>
        internal static Folder[] SelectAllUninherited(int masterID, int siteID, int folderType)     //NAAR STANDARD, CUSTOM QUERY
        {
            List<Folder> list = new List<Folder>();
            Folder folder = new Folder();
            folder.SqlOrder = "Site_Master_Key ASC, Site_Displayname ASC, Folder_CompletePath ASC";
            folder.SqlTable = "wim_Folders x";

            List<DatabaseDataValueColumn> where = new List<DatabaseDataValueColumn>();

            where.Add(new DatabaseDataValueColumn("Folder_Type", SqlDbType.Int, folderType));
            where.Add(new DatabaseDataValueColumn("Folder_Site_Key", SqlDbType.Int, masterID));
            where.Add(new DatabaseDataValueColumn("NOT Folder_Folder_Key", SqlDbType.Int, null));
            where.Add(new DatabaseDataValueColumn(string.Format("(select COUNT(*) from wim_Folders where Folder_Site_Key = {0} and Folder_Master_Key = x.Folder_Key) = 0", siteID)));

            foreach (object o in folder._SelectAll(where)) list.Add((Folder)o);
            return list.ToArray();
        }

        public Folder Clone()
        {
            Folder clone = new Folder();
            Wim.Utility.ReflectProperty(this, clone);
            clone.GUID = Guid.NewGuid();
            clone.ID = 0;
            return clone;
        }

        /// <summary>
        /// Save a database entity. This method will indentify the need for an INSERT or an UPDATE.
        /// </summary>
        /// <returns></returns>
        public override bool Save()
        {
            try
            {
                bool isSaved = base.Save();
                return isSaved;
            }
            catch (Exception exc)
            {
                throw new Exception("SQL mist misschien: alter table wim_Folders add column Folder_SortOrderMethod int ", exc);
            }
            return false;
        }

        /// <summary>
        /// Selects the one.
        /// </summary>
        /// <param name="ID">The ID.</param>
        /// <returns></returns>
        public static Folder SelectOne(int ID)
        {
            var list = (from item in SelectAll() where item.ID == ID select item);
            return list.Count() == 0 ? new Folder() : list.ToArray()[0];
        }

        /// <summary>
        /// Selects the one.
        /// </summary>
        /// <param name="ID">The ID.</param>
        /// <returns></returns>
        public static Folder SelectOne(Guid guid)
        {
            Folder implement = new Folder();

            List<DatabaseDataValueColumn> where = new List<DatabaseDataValueColumn>();
            where.Add(new DatabaseDataValueColumn("Folder_GUID", SqlDbType.UniqueIdentifier, guid));

            return (Folder)implement._SelectOne(where);
        }

        /// <summary>
        /// Updates the children (change folder name).
        /// </summary>
        /// <param name="newFolderCompletePath">The new folder complete path.</param>
        /// <returns></returns>
        public bool UpdateChildren(string newFolderCompletePath)
        {
            this.Execute(string.Format(@"
update wim_Folders set Folder_CompletePath = REPLACE (Folder_CompletePath,'{1}','{2}') where Folder_Type = {0} and Folder_CompletePath like '{1}%' and Folder_Site_Key = {3}
"
                , (int)this.Type
                , this.CompletePath
                , newFolderCompletePath
                , this.SiteID
                ));
            return true;
        }

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
        /// Selects the one by site.
        /// </summary>
        /// <param name="siteID">The site ID.</param>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static Folder SelectOneBySite(int siteID, FolderType type)
        {
            if (type == FolderType.Administration)
                siteID = 0;

            Folder[] arr;
            if (siteID == 0)
                arr = (from item in SelectAll() where item.Type == type select item).ToArray();
            else
                arr = (from item in SelectAll() where item.SiteID == siteID && item.Type == type select item).ToArray();

            return arr.Length == 0 ? new Folder() : arr[0];
        }

        /// <summary>
        /// Selects the one.
        /// </summary>
        /// <param name="masterFolderID">The master folder ID.</param>
        /// <param name="siteID">The site ID.</param>
        /// <returns></returns>
        public static Folder SelectOne(int masterFolderID, int siteID)
        {
            var list = (from item in SelectAll() where item.SiteID == siteID && item.MasterID.GetValueOrDefault() == masterFolderID select item);
            return list.Count() == 0 ? new Folder() : list.ToArray()[0];
        }

        /// <summary>
        /// Select all Folders
        /// </summary>
        /// <returns></returns>
        public static Folder[] SelectAll()
        {
            List<Folder> list = new List<Folder>();
            Folder folder = new Folder();
            folder.SqlOrder = "Site_Master_Key ASC, Site_Displayname ASC, Folder_CompletePath ASC";

            if (!string.IsNullOrEmpty(SqlConnectionString2)) folder.SqlConnectionString = SqlConnectionString2;
            foreach (object o in folder._SelectAll(true)) list.Add((Folder)o);
            return list.ToArray();
        }

        /// <summary>
        /// Select all folders based on a list of ID"s
        /// </summary>
        /// <returns></returns>
        public static Folder[] SelectAll(int[] folderID)
        {
            List<Folder> list = new List<Folder>();
            Folder folder = new Folder();
            folder.SqlOrder = "Site_Master_Key ASC, Site_Displayname ASC, Folder_CompletePath ASC";
            List<DatabaseDataValueColumn> where = new List<DatabaseDataValueColumn>();
            where.Add(new DatabaseDataValueColumn($"Folder_Key in ({string.Join(",", folderID)})"));

            if (!string.IsNullOrEmpty(SqlConnectionString2)) folder.SqlConnectionString = SqlConnectionString2;
            foreach (object o in folder._SelectAll(where)) list.Add((Folder)o);
            return list.ToArray();
        }


        /// <summary>
        /// Validates the access right.
        /// </summary>
        /// <param name="folders">The folders.</param>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        public static Sushi.Mediakiwi.Data.Folder[] ValidateAccessRight(Sushi.Mediakiwi.Data.Folder[] folders, Sushi.Mediakiwi.Data.IApplicationUser user)
        {
            return (from item in folders join relation in SelectAllAccessible(user) on item.ID equals relation.ID select item).ToArray();
        }

        /// <summary>
        /// Determines whether [has role access] [the specified role id].
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns>
        /// 	<c>true</c> if [has role access] [the specified role id]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasRoleAccess(Sushi.Mediakiwi.Data.IApplicationUser user)
        {
            if (this.CompletePath == "/") return true;
            if (this.ID == 0 || user.Role().All_Folders) return true;
            var selection = from item in SelectAllAccessible(user) where item.ID == this.ID select item;
            bool xs = selection.Count() == 1;
            return xs;
        }

        /// <summary>
        /// Selects all accessible.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        public static Folder[] SelectAllAccessible(Sushi.Mediakiwi.Data.IApplicationUser user)
        {
            Folder[] folders = null;
            if (!user.Role().All_Folders)
            {
                if (user.Role().IsAccessFolder)
                {
                    folders = (
                        from item in SelectAll()
                        join relation in Sushi.Mediakiwi.Data.RoleRight.SelectAll(user.Role().ID, Sushi.Mediakiwi.Data.RoleRightType.Folder) on item.ID equals relation.ItemID
                        select item).ToArray();
                }
                else
                {
                    var acl = (
                        from item in SelectAll()
                        join relation in Sushi.Mediakiwi.Data.RoleRight.SelectAll(user.Role().ID, Sushi.Mediakiwi.Data.RoleRightType.Folder) on item.ID equals relation.ItemID
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
        /// <param name="searchQuery">The search query.</param>
        /// <param name="isPartOfPath">if set to <c>true</c> [is part of path].</param>
        /// <returns></returns>
        public static Folder[] SelectAll(FolderType type, int siteID, string searchQuery, bool isPartOfPath)
        {
            //  && item.Level > 0 staat uit wegens de page properties waarbij je anders de root niet kan selecteren.
            //  item.IsVisible && 
            System.Linq.IOrderedEnumerable<Folder> list = null;
            if (type != FolderType.Administration_Or_List)
            {
                list = (from item in SelectAll()
                    where item.SiteID == siteID && item.Type == type //&& item.Level > 0
                    orderby item.Site().Name, item.CompletePath
                    select item);
            }
            else
            {
                list = (from item in SelectAll()
                        where item.SiteID == siteID && (item.Type == FolderType.Administration || item.Type == FolderType.List)
                        orderby item.Site().Name, item.CompletePath
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
        /// <param name="siteID">The site ID.</param>
        /// <returns></returns>
        internal static Folder[] SelectAllForDeletion(int siteID)
        {
            List<Folder> list = new List<Folder>();
            Folder folder = new Folder();

            //  Has to be reverse (DESC) for cache!
            folder.SqlOrder = "Site_Displayname ASC, Folder_CompletePath DESC";

            List<DatabaseDataValueColumn> where = new List<DatabaseDataValueColumn>();
            where.Add(new DatabaseDataValueColumn("Folder_Site_Key", SqlDbType.Int, siteID));

            foreach (object o in folder._SelectAll(where)) list.Add((Folder)o);
            return list.ToArray();
        }

        #region Properties
        int m_ID;
        /// <summary>
        /// Gets or sets the ID.
        /// </summary>
        /// <value>The ID.</value>
        [DatabaseColumn("Folder_Key", SqlDbType.Int, IsPrimaryKey = true)]
        public int ID
        {
            get { return m_ID; }
            set { m_ID = value; }
        }

        private Guid m_GUID;
        /// <summary>
        /// Unique identifier used for import/export
        /// </summary>
        [DatabaseColumn("Folder_GUID", SqlDbType.UniqueIdentifier, IsMigrationKey = true)]
        public Guid GUID
        {
            get { 
                if (this.m_GUID == Guid.Empty) this.m_GUID = Guid.NewGuid();
                return m_GUID; }
            set { m_GUID = value; }
        }

        int? m_ParentID;
        /// <summary>
        /// Gets or sets the parent ID.
        /// </summary>
        /// <value>The parent ID.</value>
        [DatabaseColumn("Folder_Folder_Key", SqlDbType.Int, IsNullable = true)]
        public int? ParentID
        {
            get { return m_ParentID; }
            set { m_ParentID = value; m_Parent = null; }
        }


        int? m_SortOrderMethod = 5;
        /// <summary>
        /// Gets or sets the SortOrderMethod
        /// </summary>
        /// <value>The Folder_SortOrderMethod</value>
        [DatabaseColumn("Folder_SortOrderMethod", SqlDbType.Int, IsNullable = true)]
        public int? SortOrderMethod
        {
            get { return m_SortOrderMethod; }
            set { m_SortOrderMethod = value; }
        }


        Sushi.Mediakiwi.Data.Folder m_Parent;
        /// <summary>
        /// Gets the parent.
        /// </summary>
        /// <value>The parent.</value>
        public Sushi.Mediakiwi.Data.Folder Parent
        {
            get {
                if (m_Parent == null)
                {
                    if (this.ParentID.HasValue)
                        m_Parent = Sushi.Mediakiwi.Data.Folder.SelectOne(ParentID.Value);
                    else
                        m_Parent = new Folder();
                }
                return m_Parent;
            }
        }

        int m_Level;
        /// <summary>
        /// Gets or sets the level.
        /// </summary>
        /// <value>The level.</value>
        public int Level
        {
            get
            {
                if (!String.IsNullOrEmpty(CompletePath))
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

        int? m_MasterID;
        /// <summary>
        /// Gets or sets the master ID.
        /// </summary>
        /// <value>The master ID.</value>
        [DatabaseColumn("Folder_Master_Key", SqlDbType.Int, IsNullable = true)]
        public int? MasterID
        {
            get { return m_MasterID; }
            set { m_MasterID = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is visible.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is visible; otherwise, <c>false</c>.
        /// </value>
        [DatabaseColumn("Folder_IsVisible", SqlDbType.Bit, IsNullable = true)]
        public bool IsVisible { get; set; }

        int m_SiteID;
        /// <summary>
        /// Gets or sets the site ID.
        /// </summary>
        /// <value>The site ID.</value>
        [DatabaseColumn("Folder_Site_Key", SqlDbType.Int)]
        public int SiteID
        {
            get { return m_SiteID; }
            set { m_SiteID = value; }
        }

        FolderType m_Type;
        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        [DatabaseColumn("Folder_Type", SqlDbType.Int)]
        public FolderType Type
        {
            get { return m_Type; }
            set { m_Type = value; }
        }

        //string m_Site;
        ///// <summary>
        ///// Gets or sets the site.
        ///// </summary>
        ///// <value>The site.</value>
        //public string Site
        //{
        //    get { return m_Site; }
        //    set { m_Site = value; }
        //}

        /// <summary>
        /// Sites this instance.
        /// </summary>
        /// <returns></returns>
        public Sushi.Mediakiwi.Data.Site Site()
        {
            return Sushi.Mediakiwi.Data.Site.SelectOne(this.SiteID);
        }

        string m_Name;
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        [DatabaseColumn("Folder_Name", SqlDbType.NVarChar, Length = 50)]
        public string Name
        {
            get { return m_Name; }
            set { m_Name = value; }
        }

        [DatabaseColumn("Folder_Description", SqlDbType.NVarChar, Length = 1024, IsNullable = true)]
        public string Description { get; set;  }

        string m_CompletePath;
        /// <summary>
        /// Gets or sets the complete path.
        /// </summary>
        /// <value>The complete path.</value>
        [DatabaseColumn("Folder_CompletePath", SqlDbType.NVarChar, Length = 1000, IsNullable = true)]
        public string CompletePath
        {
            get { return m_CompletePath; }
            set { 
                m_CompletePath = value;
                m_CompleteCleanPath = null;
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

        DateTime m_Changed;
        /// <summary>
        /// Gets or sets the changed.
        /// </summary>
        /// <value>The changed.</value>
        [DatabaseColumn("Folder_Created", SqlDbType.DateTime)]
        public DateTime Changed
        {
            get {
                if (this.m_Changed == DateTime.MinValue) this.m_Changed = Sushi.Mediakiwi.Data.Common.DatabaseDateTime;
                return m_Changed; 
            }
            set { m_Changed = value; }
        }

        /// <summary>
        /// Delete an implementation record.
        /// </summary>
        /// <returns></returns>
        public override bool Delete()
        {
            List<DatabaseDataValueColumn> whereColumns = new List<DatabaseDataValueColumn>();
            whereColumns.Add(new DatabaseDataValueColumn("Folder_Key", SqlDbType.Int, this.ID));
            whereColumns.Add(new DatabaseDataValueColumn("Folder_Master_Key", SqlDbType.Int, this.ID, DatabaseDataValueConnectType.Or));
            bool isDeleted = base.Delete(whereColumns);
            return isDeleted;
        }

        /// <summary>
        /// Gets the child count.
        /// </summary>
        /// <value>The child count.</value>
        public int ChildCount
        {
            get
            {
                Folder implement = new Folder();
                return Utility.ConvertToInt(implement.Execute(string.Format(@"
select count(*) + (select count(*) from wim_Pages where Page_Folder_Key in (select Folder_Key from wim_Folders where (Folder_Master_Key = {0} or Folder_Key = {0}))) + (select count(*) from wim_Folders where Folder_Folder_Key in (select Folder_Key from wim_Folders where (Folder_Master_Key = {0} or Folder_Key = {0})))
from wim_ComponentLists where ComponentList_Folder_Key in (select Folder_Key from wim_Folders where (Folder_Master_Key = {0} or Folder_Key = {0})) 
", this.ID)));
            }
        }
        #endregion

        /// <summary>
        /// Select a single folder instance.
        /// </summary>
        /// <param name="parentID">The parent ID.</param>
        /// <param name="name">Name of the to be searched folder</param>
        /// <returns></returns>
        public static Sushi.Mediakiwi.Data.Folder SelectOne(int parentID, string name)
        {
            var list = (from item in SelectAll() where item.ParentID.GetValueOrDefault() == parentID && item.Name == name select item);
            return list.Count() == 0 ? new Folder() : list.ToArray()[0];
        }

        static Sushi.Mediakiwi.Data.Folder FindSiteFolder(int folderID, int siteID)
        {
            var selected = (from item in SelectAll() where item.MasterID.HasValue && item.Type == FolderType.List && item.MasterID.Value == folderID select item);
            if (selected.Count() == 0) return null;
            foreach(var item in selected)
            {
                if (item.SiteID == siteID)
                    return item;

                var tmp = FindSiteFolder(item.ID, siteID);
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
        public static Sushi.Mediakiwi.Data.Folder SelectOneChild(int folderID, int siteID)
        {
            Sushi.Mediakiwi.Data.Folder current = Sushi.Mediakiwi.Data.Folder.SelectOne(folderID);
            if (current.SiteID == siteID) return current;

            current = FindSiteFolder(folderID, siteID);
            if (current == null) return new Folder();
            return current;

            //var list = (from item in SelectAll() where (item.ID == FolderID || item.MasterID.GetValueOrDefault() == FolderID) && item.SiteID == siteID select item);
            //return list.Count() == 0 ? new Folder() : list.ToArray()[0];
        }

        /// <summary>
        /// Insert an implementation record.
        /// </summary>
        /// <returns></returns>
        public override bool Insert()
        {
            if (this.IsNewInstance)
            {
                string name = this.Name;
                this.Name = GetPageNameProposal(this.ParentID.GetValueOrDefault(), this.Name);
                if (!this.Name.Equals(name))
                {
                    this.CompletePath = $"{this.CompletePath.Substring(0, this.CompletePath.Length - name.Length)}{this.Name}";
                }
            }

            return base.Insert();
        }

        /// <summary>
        /// Gets the page name proposal.
        /// </summary>
        /// <param name="folderID">The folder ID.</param>
        /// <param name="folder">The folder.</param>
        /// <returns></returns>
        public string GetPageNameProposal(int folderID, string folder)
        {
            if (folder == Sushi.Mediakiwi.Data.Common.FolderRoot || string.IsNullOrEmpty(folder)) return folder;

            string nameProposal = Wim.Utility.GlobalRegularExpression.Implement.ReplaceNotAcceptableFilenameCharacter.Replace(folder, string.Empty);

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
        /// Determines whether [is folder already taken] [the specified folder ID].
        /// </summary>
        /// <param name="folderID">The folder ID.</param>
        /// <param name="folder">The folder.</param>
        /// <returns>
        /// 	<c>true</c> if [is folder already taken] [the specified folder ID]; otherwise, <c>false</c>.
        /// </returns>
        bool IsFolderAlreadyTaken(int folderID, string folder)
        {
            using (Connection.DataCommander dac = new Connection.DataCommander(this.SqlConnectionString))
            {
                dac.ConnectionType = this.ConnectionType;
                dac.Text = "select count(*) from wim_folders where Folder_Folder_Key = @Folder_Key and Folder_Name = @Folder_Name";
                dac.SetParameterInput("@Folder_Key", folderID, SqlDbType.Int);
                dac.SetParameterInput("@Folder_Name", folder, SqlDbType.NVarChar, 50);
                int count = (int)dac.ExecScalar();
                if (count == 0)
                    return false;
                return true;
            }
        }

        /// <summary>
        /// Select all childfolder in a folder
        /// Default: Return only published page folders
        /// </summary>
        /// <param name="folderID">The folder ID.</param>
        /// <returns></returns>
        public static Sushi.Mediakiwi.Data.Folder[] SelectAllByParent(int folderID)
        {
            return SelectAllByParent(folderID, FolderType.Undefined);
        }

        /// <summary>
        /// Select all childfolder in a folder
        /// Default: Return only published folders
        /// </summary>
        /// <param name="folderID">The folder ID.</param>
        /// <param name="type">Type of folder</param>
        /// <returns></returns>
        public static Sushi.Mediakiwi.Data.Folder[] SelectAllByParent(int folderID, FolderType type)
        {
            return SelectAllByParent(folderID, type, true);
        }


        /// <summary>
        /// Select all childfolder in a folder
        /// </summary>
        /// <param name="folderID">The folder ID.</param>
        /// <param name="type">Type of folder</param>
        /// <param name="returnOnlyPublishedFolder">Should this method return only published folders?</param>
        /// <param name="roleID">The role ID.</param>
        /// <returns></returns>
        public static Sushi.Mediakiwi.Data.Folder[] SelectAllByParent(int folderID, FolderType type, bool returnOnlyPublishedFolder)
        {
            Sushi.Mediakiwi.Data.Folder[] folders;

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

        static Folder GetParentFolder(Folder[] folders, int ID)
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
        public static Sushi.Mediakiwi.Data.Folder[] SelectAllByBackwardTrail(int folderID)
        {
            Folder implement = new Folder();
            List<Folder> listAll = new List<Folder>();

            Folder folder = Folder.SelectOne(folderID);

            List<DatabaseDataValueColumn> whereclause = new List<DatabaseDataValueColumn>();
            whereclause.Add(new DatabaseDataValueColumn("Folder_Site_Key", SqlDbType.Int, folder.SiteID));
            implement.SqlOrder = "Folder_Folder_Key DESC";

            if (!string.IsNullOrEmpty(SqlConnectionString2)) implement.SqlConnectionString = SqlConnectionString2;
            foreach (object o in implement._SelectAll(whereclause))
                listAll.Add((Folder)o);

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


        public DateTime? Updated
        {
            get { return null; }
        }

        #endregion REPLICATED TO Sushi.Mediakiwi.Data.Standard
    }
}
