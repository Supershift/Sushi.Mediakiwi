//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Data;
//using System.Data.SqlClient;
//using Sushi.Mediakiwi.Data.DalReflection;

//namespace Sushi.Mediakiwi.Framework2.Functions
//{
//    /// <summary>
//    /// 
//    /// </summary>
//    [DatabaseTable("wim_AuditTrails")]
//    public class AuditTrail : Sushi.Mediakiwi.Data.DalReflection.DatabaseEntity
//    {
//        ///// <summary>
//        ///// Inserts the specified user.
//        ///// </summary>
//        ///// <param name="user">The user.</param>
//        ///// <param name="list">The list.</param>
//        ///// <param name="listItemID">The list item ID.</param>
//        ///// <param name="action">The action.</param>
//        ///// <param name="versionID">The version ID.</param>
//        //public static void Insert(Sushi.Mediakiwi.Data.ApplicationUser user, Sushi.Mediakiwi.Data.IComponentList list, int listItemID, Auditing.ActionType action, int? versionID)
//        //{
//        //    AuditTrail item = new AuditTrail();
//        //    item.UserID = user.ID;
//        //    item.ItemTypeID = (int)Auditing.ItemType.List;
//        //    item.ItemID = list.ID;
//        //    item.ElementID = listItemID;
//        //    item.ActionTypeID = (int)action;
//        //    item.VersionID = versionID;

//        //    item.Data.Apply("User", user.Displayname);
//        //    item.Data.Apply("List", list.Name);

//        //    item.Save();
//        //}

//        /// <summary>
//        /// Inserts the specified user.
//        /// </summary>
//        /// <param name="user">The user.</param>
//        /// <param name="page">The page.</param>
//        /// <param name="action">The action.</param>
//        /// <param name="versionID">The version ID.</param>
//        public static void Insert(Sushi.Mediakiwi.Data.IApplicationUser user, Sushi.Mediakiwi.Data.Page page, Auditing.ActionType action, int? versionID)
//        {
//            AuditTrail item = new AuditTrail();
//            item.ActionID = user == null ? 0 : user.ID;
//            item.SectionID = (int)Auditing.ItemType.Page;
//            item.ItemID = page.ID;
//            item.ActionTypeID = (int)action;
//            item.VersionID = versionID;
//            item.Message = (string.Format("{0} :: {1} :: {2}", user.Displayname, page.CompletePath, action.ToString()));
//            item.Save();
//        }

//        ///// <summary>
//        ///// Inserts the specified user.
//        ///// </summary>
//        ///// <param name="user">The user.</param>
//        ///// <param name="asset">The asset.</param>
//        ///// <param name="action">The action.</param>
//        ///// <param name="versionID">The version ID.</param>
//        //public static void Insert(Sushi.Mediakiwi.Data.ApplicationUser user, Sushi.Mediakiwi.Data.Asset asset, Auditing.ActionType action, int? versionID)
//        //{
//        //    //AuditTrail item = new AuditTrail();
//        //    //item.UserID = user.ID;
//        //    //item.ItemTypeID = (int)Auditing.ItemType.Asset;
//        //    //item.ItemID = asset.ID;
//        //    //item.ActionTypeID = (int)action;
//        //    //item.VersionID = versionID;

//        //    //item.Data.Apply("User", user.Displayname);
//        //    //item.Data.Apply("Path", asset.Path);

//        //    //item.Save();
//        //}

//        ///// <summary>
//        ///// Inserts the specified user.
//        ///// </summary>
//        ///// <param name="user">The user.</param>
//        ///// <param name="gallery">The gallery.</param>
//        ///// <param name="action">The action.</param>
//        ///// <param name="versionID">The version ID.</param>
//        //public static void Insert(Sushi.Mediakiwi.Data.ApplicationUser user, Sushi.Mediakiwi.Data.Gallery gallery, Auditing.ActionType action, int? versionID)
//        //{
//        //    //AuditTrail item = new AuditTrail();
//        //    //item.UserID = user.ID;
//        //    //item.ItemTypeID = (int)Auditing.ItemType.Gallery;
//        //    //item.ItemID = gallery.ID;
//        //    //item.ActionTypeID = (int)action;
//        //    //item.VersionID = versionID;

//        //    //item.Data.Apply("User", user.Displayname);
//        //    //item.Data.Apply("Name", gallery.Name);

//        //    //item.Save();
//        //}

//        ///// <summary>
//        ///// Inserts the specified user.
//        ///// </summary>
//        ///// <param name="user">The user.</param>
//        ///// <param name="folder">The folder.</param>
//        ///// <param name="action">The action.</param>
//        ///// <param name="versionID">The version ID.</param>
//        //public static void Insert(Sushi.Mediakiwi.Data.ApplicationUser user, Sushi.Mediakiwi.Data.Folder folder, Auditing.ActionType action, int? versionID)
//        //{
//        //    //AuditTrail item = new AuditTrail();
//        //    //item.UserID = user.ID;
//        //    //item.ItemTypeID = (int)Auditing.ItemType.Folder;
//        //    //item.ItemID = folder.ID;
//        //    //item.ActionTypeID = (int)action;
//        //    //item.VersionID = versionID;

//        //    //item.Data.Apply("User", user.Displayname);
//        //    //item.Data.Apply("Path", folder.CompletePath);

//        //    //item.Save();
//        //}

//        ///// <summary>
//        ///// Inserts the specified user.
//        ///// </summary>
//        ///// <param name="user">The user.</param>
//        ///// <param name="action">The action.</param>
//        //public static void Insert(Sushi.Mediakiwi.Data.ApplicationUser user, Auditing.ActionType action)
//        //{
//        //    //AuditTrail item = new AuditTrail();
//        //    //item.UserID = user.ID;
//        //    //item.ActionTypeID = (int)action;

//        //    //if (user != null && !user.IsNewInstance)
//        //    //    item.Data.Apply("User", user.Displayname);

//        //    //item.Save();
//        //}

//        /// <summary>
//        /// Gets or sets the ID.
//        /// </summary>
//        /// <value>The ID.</value>
//        [DatabaseColumn("AuditTrail_Key", SqlDbType.Int, IsPrimaryKey = true)]
//        public int ID{ get; set; }

//        /// <summary>
//        /// Gets or sets the item ID.
//        /// </summary>
//        /// <value>The item ID.</value>
//        [DatabaseColumn("AuditTrail_Entity_Key", SqlDbType.Int, IsNullable = true)]
//        public int SectionID { get; set; }

//        /// <summary>
//        /// Gets or sets the item type ID.
//        /// </summary>
//        /// <value>The item type ID.</value>
//        [DatabaseColumn("AuditTrail_Action", SqlDbType.Int, IsNullable = true)]
//        public int ActionID { get; set; }


//        /// <summary>
//        /// Gets or sets the action type ID.
//        /// </summary>
//        /// <value>The action type ID.</value>
//        [DatabaseColumn("AuditTrail_Type", SqlDbType.Int, IsNullable = true)]
//        public int ActionTypeID { get; set; }

//        /// <summary>
//        /// Gets or sets the element ID.
//        /// </summary>
//        /// <value>The element ID.</value>
//        [DatabaseColumn("AuditTrail_Listitem_Key", SqlDbType.Int, IsNullable = true)]
//        public int? ItemID { get; set; }

//        /// <summary>
//        /// Gets or sets the version ID.
//        /// </summary>
//        /// <value>The version ID.</value>
//        [DatabaseColumn("AuditTrail_Versioning_Key", SqlDbType.Int, IsNullable = true)]
//        public int? VersionID { get; set; }

//        private DateTime m_Created;
//        /// <summary>
//        /// Gets or sets the created.
//        /// </summary>
//        /// <value>The created.</value>
//        [DatabaseColumn("AuditTrail_Created", SqlDbType.DateTime)]
//        public DateTime Created
//        {
//            get
//            {
//                if (this.m_Created == DateTime.MinValue) this.m_Created = Sushi.Mediakiwi.Data.Common.DatabaseDateTime;
//                return m_Created;
//            }
//            set { m_Created = value; }
//        }

//        [DatabaseColumn("AuditTrail_Message", SqlDbType.NVarChar, Length = 512, IsNullable = true)]
//        public string Message { get; set; }
//    }
//}
