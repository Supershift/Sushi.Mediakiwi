using System;
using System.Web;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Sushi.Mediakiwi.Data.DalReflection;
using Sushi.Mediakiwi.Data.Parsers;

namespace Sushi.Mediakiwi.Data
{
    /// <summary>
    /// Represents a ApplicationRole entity.
    /// </summary>
    [DatabaseTable("wim_Roles")]
    public class ApplicationRole : IApplicationRole
    {
  

        static IApplicationRoleParser _Parser;
        static IApplicationRoleParser Parser
        {
            get
            {
                if (_Parser == null)
                    _Parser = Environment.GetInstance<IApplicationRoleParser>();
                return _Parser;
            }
        }

        #region REPLICATED TO Sushi.Mediakiwi.Data.Standard

        public bool IsNewInstance { get { return ID == 0; } }

        /// <summary>
        /// Gets or sets the ID.
        /// </summary>
        /// <value>The ID.</value>
        [DatabaseColumn("Role_Key", SqlDbType.Int, IsPrimaryKey = true)]
        public int ID { get; set; }


        private Guid m_GUID;
        /// <summary>
        /// Gets or sets the GUID.
        /// </summary>
        /// <value>The GUID.</value>
        [DatabaseColumn("Role_Guid", SqlDbType.UniqueIdentifier, IsMigrationKey = true)]
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
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        [DatabaseColumn("Role_Name", SqlDbType.NVarChar, Length = 50)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        [DatabaseColumn("Role_Description", SqlDbType.NVarChar, Length = 255)]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance can see page.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can see page; otherwise, <c>false</c>.
        /// </value>
        [DatabaseColumn("Role_CanSeePage", SqlDbType.Bit)]
        public bool CanSeePage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance can see list.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can see list; otherwise, <c>false</c>.
        /// </value>
        [DatabaseColumn("Role_CanSeeList", SqlDbType.Bit)]
        public bool CanSeeList { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance can see admin.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can see admin; otherwise, <c>false</c>.
        /// </value>
        [DatabaseColumn("Role_CanSeeAdmin", SqlDbType.Bit)]
        public bool CanSeeAdmin { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance can see gallery.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can see gallery; otherwise, <c>false</c>.
        /// </value>
        [DatabaseColumn("Role_CanSeeGallery", SqlDbType.Bit)]
        public bool CanSeeGallery { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance can see folder.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can see gallery; otherwise, <c>false</c>.
        /// </value>
        [DatabaseColumn("Role_CanSeeFolder", SqlDbType.Bit)]
        public bool CanSeeFolder { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance can see gallery.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can see gallery; otherwise, <c>false</c>.
        /// </value>
        [DatabaseColumn("Role_CanSeeFolder", SqlDbType.Bit)]
        public bool CanSeeFolders { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance can change page.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can change page; otherwise, <c>false</c>.
        /// </value>
        [DatabaseColumn("Role_Page_CanChange", SqlDbType.Bit)]
        public bool CanChangePage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance can create page.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can create page; otherwise, <c>false</c>.
        /// </value>
        [DatabaseColumn("Role_Page_CanCreate", SqlDbType.Bit)]
        public bool CanCreatePage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance can publish page.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can publish page; otherwise, <c>false</c>.
        /// </value>
        [DatabaseColumn("Role_Page_CanPublish", SqlDbType.Bit)]
        public bool CanPublishPage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance can delete page.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can delete page; otherwise, <c>false</c>.
        /// </value>
        [DatabaseColumn("Role_Page_CanDelete", SqlDbType.Bit)]
        public bool CanDeletePage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance can create list.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can create list; otherwise, <c>false</c>.
        /// </value>
        [DatabaseColumn("Role_List_CanCreate", SqlDbType.Bit)]
        public bool CanCreateList { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance can change list.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can change list; otherwise, <c>false</c>.
        /// </value>
        [DatabaseColumn("Role_List_CanChange", SqlDbType.Bit)]
        public bool CanChangeList { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is access list.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is access list; otherwise, <c>false</c>.
        /// </value>
        [DatabaseColumn("Role_List_IsAccessList", SqlDbType.Bit, IsNullable = true)]
        public bool IsAccessList { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is access site.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is access site; otherwise, <c>false</c>.
        /// </value>
        [DatabaseColumn("Role_List_IsAccessSite", SqlDbType.Bit, IsNullable = true)]
        public bool IsAccessSite { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is access folder.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is access folder; otherwise, <c>false</c>.
        /// </value>
        [DatabaseColumn("Role_List_IsAccessFolder", SqlDbType.Bit, IsNullable = true)]
        public bool IsAccessFolder { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is access gallery.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is access gallery; otherwise, <c>false</c>.
        /// </value>
        [DatabaseColumn("Role_List_IsAccessGallery", SqlDbType.Bit, IsNullable = true)]
        public bool IsAccessGallery { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [all_ sites].
        /// </summary>
        /// <value><c>true</c> if [all_ sites]; otherwise, <c>false</c>.</value>
        [DatabaseColumn("Role_All_Sites", SqlDbType.Bit)]
        public bool All_Sites { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [all_ folders].
        /// </summary>
        /// <value><c>true</c> if [all_ folders]; otherwise, <c>false</c>.</value>
        [DatabaseColumn("Role_All_Folders", SqlDbType.Bit)]
        public bool All_Folders { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [all_ galleries].
        /// </summary>
        /// <value><c>true</c> if [all_ galleries]; otherwise, <c>false</c>.</value>
        [DatabaseColumn("Role_All_Galleries", SqlDbType.Bit)]
        public bool All_Galleries { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [DatabaseColumn("Role_Gallery_Key", SqlDbType.Int, IsNullable = true)]
        public int? GalleryRoot { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [all_ lists].
        /// </summary>
        /// <value><c>true</c> if [all_ lists]; otherwise, <c>false</c>.</value>
        [DatabaseColumn("Role_All_Lists", SqlDbType.Bit)]
        public bool All_Lists { get; set; }

        /// <summary>
        /// Gets or sets the dashboard.
        /// </summary>
        /// <value>The dashboard.</value>
        [DatabaseColumn("Role_Dashboard", SqlDbType.Int)]
        public int Dashboard { get; set; }

        /// <summary>
        /// Selects all.
        /// </summary>
        /// <returns></returns>
        public static IApplicationRole[] SelectAll()
        {
            return Parser.SelectAll();
        }

        /// <summary>
        /// Selects all roles that have access to a certain folder.
        /// </summary>
        /// <param name="folderID">The folder ID.</param>
        /// <returns></returns>
        public static IApplicationRole[] SelectAll(int folderID)
        {
            return Parser.SelectAll(folderID);
        }

        /// <summary>
        /// Delete an implementation record.
        /// </summary>
        /// <returns></returns>
        public bool Delete()
        {
            return Parser.Delete(this.ID);
        }

        public void Save()
        {
            Parser.Save(this);
        }

        /// <summary>
        /// Selects the one.
        /// </summary>
        /// <param name="ID">The ID.</param>
        /// <returns></returns>
        public static IApplicationRole SelectOne(int ID)
        {
            return Parser.SelectOne(ID);
        }
        #endregion REPLICATED TO Sushi.Mediakiwi.Data.Standard

        #region COMMENTED, not used anywhere

  //      /// <summary>
  //      /// Gets the sites.
  //      /// </summary>
  //      /// <value>The sites.</value>
  //      public Data.Site[] Sites(Sushi.Mediakiwi.Data.IApplicationUser user) 
		//{
  //          return Site.SelectAllAccessible(user, AccessFilter.RoleAndUser);
		//}

  //      /// <summary>
  //      /// Gets the lists.
  //      /// </summary>
  //      /// <value>The lists.</value>
  //      public Data.IComponentList[] Lists(Sushi.Mediakiwi.Data.IApplicationUser user) 
		//{
  //          return ComponentList.SelectAllAccessibleLists(user, RoleRightType.List);
		//}

  //      /// <summary>
  //      /// Listses the specified user.
  //      /// </summary>
  //      /// <param name="user">The user.</param>
  //      /// <returns></returns>
  //      public Data.Gallery[] Galleries(Sushi.Mediakiwi.Data.IApplicationUser user)
  //      {
  //          return Gallery.SelectAllAccessible(user);
  //      }

  //      /// <summary>
  //      /// Folderses this instance.
  //      /// </summary>
  //      /// <returns></returns>
  //      public Data.Folder[] Folders(Sushi.Mediakiwi.Data.IApplicationUser user)
  //      {
  //          return Folder.SelectAllAccessible(user);
  //      }
        
		//private Data.Task[] m_OutboundTasks;
  //      /// <summary>
  //      /// Gets the outbound tasks.
  //      /// </summary>
  //      /// <value>The outbound tasks.</value>
  //      public Data.Task[] OutboundTasks 
		//{
		//	get {
  //              //if ( m_OutboundTasks == null)
  //              //    m_OutboundTasks = Task.SelectAll(this.ID, false);
                
  //              return m_OutboundTasks; 
  //          }
		//}
        
		//private Data.Task[] m_SubscribedTasks;
  //      /// <summary>
  //      /// Gets the subscribed tasks.
  //      /// </summary>
  //      /// <value>The subscribed tasks.</value>
  //      public Data.Task[] SubscribedTasks 
		//{
		//	get { 
  //              //if ( m_SubscribedTasks == null)
  //              //    m_SubscribedTasks = Task.SelectAll(this.ID, true);                
  //              return m_SubscribedTasks; 
  //          }
		//}

        #endregion COMMENTED, not used anywhere
    }
}