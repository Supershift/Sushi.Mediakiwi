using Sushi.Mediakiwi.Data.MicroORM;
using Sushi.MicroORM.Mapping;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Data
{
    [DataMap(typeof(ApplicationRoleMap))]
    public class ApplicationRole : IApplicationRole
    {
        #region Mapping
        internal class ApplicationRoleMap : DataMap<ApplicationRole>
        {
            public ApplicationRoleMap()
            {
                Table("wim_Roles");
                Id(x => x.ID, "Role_Key").Identity();
                Map(x => x.GUID, "Role_Guid").SqlType(SqlDbType.UniqueIdentifier);
                Map(x => x.Name, "Role_Name").SqlType(SqlDbType.NVarChar).Length(50);
                Map(x => x.Description, "Role_Description").SqlType(SqlDbType.NVarChar).Length(255);
                Map(x => x.CanSeePage, "Role_CanSeePage").SqlType(SqlDbType.Bit);
                Map(x => x.CanSeeList, "Role_CanSeeList").SqlType(SqlDbType.Bit);
                Map(x => x.CanSeeAdmin, "Role_CanSeeAdmin").SqlType(SqlDbType.Bit);
                Map(x => x.CanSeeGallery, "Role_CanSeeGallery").SqlType(SqlDbType.Bit);
                Map(x => x.CanSeeFolder, "Role_CanSeeFolder").SqlType(SqlDbType.Bit);
                Map(x => x.CanChangePage, "Role_Page_CanChange").SqlType(SqlDbType.Bit);
                Map(x => x.CanCreatePage, "Role_Page_CanCreate").SqlType(SqlDbType.Bit);
                Map(x => x.CanPublishPage, "Role_Page_CanPublish").SqlType(SqlDbType.Bit);
                Map(x => x.CanDeletePage, "Role_Page_CanDelete").SqlType(SqlDbType.Bit);
                Map(x => x.CanCreateList, "Role_List_CanCreate").SqlType(SqlDbType.Bit);
                Map(x => x.CanChangeList, "Role_List_CanChange").SqlType(SqlDbType.Bit);
                Map(x => x.IsAccessList, "Role_List_IsAccessList").SqlType(SqlDbType.Bit);
                Map(x => x.IsAccessSite, "Role_List_IsAccessSite").SqlType(SqlDbType.Bit);
                Map(x => x.IsAccessFolder, "Role_List_IsAccessFolder").SqlType(SqlDbType.Bit);
                Map(x => x.IsAccessGallery, "Role_List_IsAccessGallery").SqlType(SqlDbType.Bit);
                Map(x => x.All_Sites, "Role_All_Sites").SqlType(SqlDbType.Bit);
                Map(x => x.All_Folders, "Role_All_Folders").SqlType(SqlDbType.Bit);
                Map(x => x.All_Galleries, "Role_All_Galleries").SqlType(SqlDbType.Bit);
                Map(x => x.GalleryRoot, "Role_Gallery_Key").SqlType(SqlDbType.Int);
                Map(x => x.All_Lists, "Role_All_Lists").SqlType(SqlDbType.Bit);
                Map(x => x.Dashboard, "Role_Dashboard").SqlType(SqlDbType.Int);
            }
        }

        #endregion Mapping

        #region properties

        public bool IsNewInstance { get { return ID == 0; } }

        /// <summary>
        /// Uniqe identifier of the ApplicationRole
        /// </summary>
        public int ID { get; set; }

        private Guid m_GUID;

        /// <summary>
        /// Uniqe GUID of the ApplicationRole
        /// </summary>
        public Guid GUID
        {
            get
            {
                if (this.m_GUID == Guid.Empty) this.m_GUID = Guid.NewGuid();
                return m_GUID;
            }
            set { m_GUID = value; }
        }

        /// <summary>
        /// Display name of the ApplicationRole
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The description of the ApplicationRole
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance can see page.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can see page; otherwise, <c>false</c>.
        /// </value>
        public bool CanSeePage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance can see list.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can see list; otherwise, <c>false</c>.
        /// </value>
        public bool CanSeeList { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance can see admin.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can see admin; otherwise, <c>false</c>.
        /// </value>
        public bool CanSeeAdmin { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance can see gallery.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can see gallery; otherwise, <c>false</c>.
        /// </value>
        public bool CanSeeGallery { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance can see folder.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can see gallery; otherwise, <c>false</c>.
        /// </value>
        public bool CanSeeFolder { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance can change page.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can change page; otherwise, <c>false</c>.
        /// </value>
        public bool CanChangePage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance can create page.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can create page; otherwise, <c>false</c>.
        /// </value>
        public bool CanCreatePage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance can publish page.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can publish page; otherwise, <c>false</c>.
        /// </value>
        public bool CanPublishPage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance can delete page.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can delete page; otherwise, <c>false</c>.
        /// </value>
        public bool CanDeletePage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance can create list.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can create list; otherwise, <c>false</c>.
        /// </value>
        public bool CanCreateList { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance can change list.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can change list; otherwise, <c>false</c>.
        /// </value>
        public bool CanChangeList { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is access list.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is access list; otherwise, <c>false</c>.
        /// </value>
        public bool IsAccessList { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is access site.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is access site; otherwise, <c>false</c>.
        /// </value>
        public bool IsAccessSite { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is access folder.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is access folder; otherwise, <c>false</c>.
        /// </value>
        public bool IsAccessFolder { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is access gallery.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is access gallery; otherwise, <c>false</c>.
        /// </value>
        public bool IsAccessGallery { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [all_ sites].
        /// </summary>
        /// <value><c>true</c> if [all_ sites]; otherwise, <c>false</c>.</value>
        public bool All_Sites { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [all_ folders].
        /// </summary>
        /// <value><c>true</c> if [all_ folders]; otherwise, <c>false</c>.</value>
        public bool All_Folders { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [all_ galleries].
        /// </summary>
        /// <value><c>true</c> if [all_ galleries]; otherwise, <c>false</c>.</value>
        public bool All_Galleries { get; set; }

        /// <summary>
        ///
        /// </summary>
        public int? GalleryRoot { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [all_ lists].
        /// </summary>
        /// <value><c>true</c> if [all_ lists]; otherwise, <c>false</c>.</value>
        public bool All_Lists { get; set; }

        /// <summary>
        /// Gets or sets the dashboard.
        /// </summary>
        /// <value>The dashboard.</value>
        public int Dashboard { get; set; }

        #endregion properties

        /// <summary>
        /// Select an Application Role based on its primary key
        /// </summary>
        /// <param name="ID">Unique identifier of the Application Role</param>
        /// <returns></returns>
        public static IApplicationRole SelectOne(int ID)
        {
            var connector = ConnectorFactory.CreateConnector<ApplicationRole>();
            return connector.FetchSingle(ID);
        }

        /// <summary>
        /// Select an Application Role based on its primary key
        /// </summary>
        /// <param name="ID">Unique identifier of the Application Role</param>
        /// <returns></returns>
        public static async Task<IApplicationRole> SelectOneAsync(int ID)
        {
            var connector = ConnectorFactory.CreateConnector<ApplicationRole>();
            return await connector.FetchSingleAsync(ID).ConfigureAwait(false);
        }

        /// <summary>
        /// Selects all roles that have access to a certain folder.
        /// </summary>
        /// <returns></returns>
        public static IApplicationRole[] SelectAll()
        {
            //[MJ:03-01-2020] TEST this method !
            var connector = ConnectorFactory.CreateConnector<ApplicationRole>();
            var filter = connector.CreateDataFilter();

            return connector.FetchAll(filter).ToArray<IApplicationRole>();
        }

        /// <summary>
        /// Selects all roles that have access to a certain folder.
        /// </summary>
        /// <returns></returns>
        public static async Task<IApplicationRole[]> SelectAllAsync()
        {
            //[MJ:03-01-2020] TEST this method !
            var connector = ConnectorFactory.CreateConnector<ApplicationRole>();
            var filter = connector.CreateDataFilter();

            var result = await connector.FetchAllAsync(filter).ConfigureAwait(false);
            return result.ToArray<IApplicationRole>();
        }

        /// <summary>
        /// Selects all roles that have access to a certain folder.
        /// </summary>
        /// <param name="folderID">The folder ID.</param>
        /// <returns></returns>
        public static IApplicationRole[] SelectAll(int folderID)
        {
            //[MJ:03-01-2020] TEST this method !
            var connector = ConnectorFactory.CreateConnector<ApplicationRole>();
            var filter = connector.CreateDataFilter();
            filter.AddParameter("@folderID", folderID);

            return connector.FetchAll(
                @"SELECT * FROM [dbo].[wim_Roles]
                  LEFT JOIN [dbo].[wim_RoleRights] ON [RoleRight_Role_Key] = [Role_Key] AND [RoleRight_Child_Type] = 6
                  WHERE [RoleRight_Child_Key] = @folderID",
                filter).ToArray<IApplicationRole>(); 
        }

        /// <summary>
        /// Selects all roles that have access to a certain folder async.
        /// </summary>
        /// <param name="folderID">The folder ID.</param>
        /// <returns></returns>
        public static async Task<IApplicationRole[]> SelectAllAsync(int folderID)
        {
            //[MJ:03-01-2020] TEST this method !
            var connector = ConnectorFactory.CreateConnector<ApplicationRole>();
            var filter = connector.CreateDataFilter();
            filter.AddParameter("@folderID", folderID);

            var result = await connector.FetchAllAsync(
                @"SELECT * FROM [dbo].[wim_Roles]
                  LEFT JOIN [dbo].[wim_RoleRights] ON [RoleRight_Role_Key] = [Role_Key] AND [RoleRight_Child_Type] = 6
                  WHERE [RoleRight_Child_Key] = @folderID",
                filter).ConfigureAwait(false);

            return result.ToArray<IApplicationRole>();
        }

        /// <summary>
        /// Saves the Application Role to database
        /// </summary>
        public void Save()
        {
            var connector = ConnectorFactory.CreateConnector<ApplicationRole>();
            connector.Save(this);
        }

        /// <summary>
        /// Saves the Application Role to database
        /// </summary>
        public async Task SaveAsync()
        {
            var connector = ConnectorFactory.CreateConnector<ApplicationRole>();
            await connector.SaveAsync(this).ConfigureAwait(false);
        }

        /// <summary>
        /// Deletes the Application Role from the database
        /// </summary>
        public virtual void Delete()
        {
            var connector = ConnectorFactory.CreateConnector<ApplicationRole>();
            connector.Delete(this);
        }

        /// <summary>
        /// Deletes the Application Role from the database
        /// </summary>
        public async Task DeleteAsync()
        {
            var connector = ConnectorFactory.CreateConnector<ApplicationRole>();
            await connector.DeleteAsync(this).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the sites.
        /// </summary>
        /// <value>The sites.</value>
        public Site[] Sites(IApplicationUser user)
        {
            return Site.SelectAllAccessible(user, AccessFilter.RoleAndUser);
        }

        /// <summary>
        /// Gets the lists.
        /// </summary>
        /// <value>The lists.</value>
        public IComponentList[] Lists(IApplicationUser user)
        {
            return ComponentList.SelectAllAccessibleLists(user, RoleRightType.List);
        }

        /// <summary>
        /// Listses the specified user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        public Gallery[] Galleries(IApplicationUser user)
        {
            return Gallery.SelectAllAccessible(user);
        }

        /// <summary>
        /// Folderses this instance.
        /// </summary>
        /// <returns></returns>
        public Folder[] Folders(IApplicationUser user)
        {
            return Folder.SelectAllAccessible(user);
        }
    }
}