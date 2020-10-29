using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Sushi.Mediakiwi.Data.DalReflection;
using System.Linq;
using Sushi.Mediakiwi.Framework;

namespace Sushi.Mediakiwi.Data
{
    /// <summary>
    /// Represents a RoleRight entity.
    /// </summary>
    [DatabaseTable("wim_RoleRights", Order = "RoleRight_Child_Key, RoleRight_SortOrder")]
    public class RoleRight : DatabaseEntity
    {
        #region REPLICATED TO Sushi.Mediakiwi.Data.Standard

        /// <summary>
        /// TODO: Move to different locations
        /// </summary>
        public enum Access
        {
            /// <summary>
            /// 
            /// </summary>
            Granted = 1,
            /// <summary>
            /// 
            /// </summary>
            Denied = 2,
            /// <summary>
            /// 
            /// </summary>
            Inherit = 3
        }

        #region Properties
        /// <summary>
        /// Gets or sets the role ID.
        /// </summary>
        /// <value>The role ID.</value>
        [DatabaseColumn("RoleRight_Role_Key", SqlDbType.Int)]
        public int RoleID { get; set; }

        /// <summary>
        /// Gets or sets the item ID.
        /// </summary>
        /// <value>The item ID.</value>
        [DatabaseColumn("RoleRight_Child_Key", SqlDbType.Int)]
        public int ItemID { get; set; }

        /// <summary>
        /// Gets or sets the type ID.
        /// </summary>
        /// <value>The type ID.</value>
        [DatabaseColumn("RoleRight_Child_Type", SqlDbType.Int)]
        public int TypeID { get; set; }

        /// <summary>
        /// Gets or sets the type ID.
        /// </summary>
        /// <value>The type ID.</value>
        [DatabaseColumn("RoleRight_Type", SqlDbType.Int)]
        public Access AccessType { get; set; }
        #endregion Properties

        /// <summary>
        /// Selects all.
        /// </summary>
        /// <returns></returns>
        public static RoleRight[] SelectAll()
        {
            List<RoleRight> list = new List<RoleRight>();
            RoleRight candidate = new RoleRight();
            if (!string.IsNullOrEmpty(SqlConnectionString2)) candidate.SqlConnectionString = SqlConnectionString2;
            foreach (object obj in candidate._SelectAll(true)) list.Add((RoleRight)obj);
            SqlConnectionString2 = null;
            return list.ToArray();
        }

        /// <summary>
        /// Selects all.
        /// </summary>
        /// <param name="roleID">The role ID.</param>
        /// <returns></returns>
        public static RoleRight[] SelectAll(int roleID)
        {
            var candidate = (from item in SelectAll() where item.RoleID == roleID select item);
            return candidate.ToArray();
        }

        /// <summary>
        /// Selects all.
        /// </summary>
        /// <param name="roleID">The role ID.</param>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static RoleRight[] SelectAll(int roleID, RoleRightType type)
        {
            var candidate = (from item in SelectAll() where item.RoleID == roleID && item.TypeID == (int)type select item);
            return candidate.ToArray();
        }

        /// <summary>
        /// Prepares the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="roleID">The role ID.</param>
        static void Prepare(RoleRightType type, int roleID)
        {
            RoleRight implement = new RoleRight();
            implement.Execute(string.Format("update wim_RoleRights set RoleRight_Update = 0 where RoleRight_Role_Key = {0} and RoleRight_Child_Type = {1}", roleID, (int)type));
        }

        /// <summary>
        /// Prepares the folder.
        /// </summary>
        /// <param name="folderID">The folder ID.</param>
        static void PrepareFolder(int folderID)
        {
            RoleRightType type = RoleRightType.Folder;
            RoleRight implement = new RoleRight();
            implement.Execute(string.Format("update wim_RoleRights set RoleRight_Update = 0 where RoleRight_Child_Key = {0} and RoleRight_Child_Type = {1}", folderID, (int)type));
        }


        /// <summary>
        /// Cleans up folder.
        /// </summary>
        /// <param name="folderID">The folder ID.</param>
        static void CleanUpFolder(int folderID)
        {
            RoleRightType type = RoleRightType.Folder;

            RoleRight implement = new RoleRight();
            implement.Execute(string.Format("delete from wim_RoleRights where RoleRight_Update = 0 and RoleRight_Child_Key = {0} and RoleRight_Child_Type = {1}", folderID, (int)type));
        }

        /// <summary>
        /// Cleans up.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="roleID">The role ID.</param>
        static void CleanUp(RoleRightType type, int roleID)
        {
            RoleRight implement = new RoleRight();
            implement.Execute(string.Format("delete from wim_RoleRights where RoleRight_Update = 0 and RoleRight_Role_Key = {0} and RoleRight_Child_Type = {1}", roleID, (int)type));
        }


        /// <summary>
        /// Updates the specified sub list.
        /// </summary>
        /// <param name="subList">The sub list.</param>
        /// <param name="folderID">The folder ID.</param>
        public static void UpdateFolder(Sushi.Mediakiwi.Data.SubList subList, int folderID)
        {
            RoleRightType type = RoleRightType.Folder;

            PrepareFolder(folderID);
            if (subList != null && subList.Items != null && subList.Items.Length > 0)
            {
                int sortOrder = 0;
                RoleRight implement = new RoleRight();
                foreach (Sushi.Mediakiwi.Data.SubList.SubListitem item in subList.Items)
                {
                    sortOrder++;

                    int count = Wim.Utility.ConvertToInt(implement.Execute(string.Format("select COUNT(*) FROM wim_RoleRights where RoleRight_Role_Key = {0} and RoleRight_Child_Key = {1} and RoleRight_Child_Type = {2}", item.ID, folderID, (int)type)));
                    if (count == 0)
                        implement.Execute(string.Format("insert into wim_RoleRights (RoleRight_Role_Key, RoleRight_Child_Key, RoleRight_Child_Type, RoleRight_Update, RoleRight_SortOrder, RoleRight_Type) values ({0}, {1}, {2}, 1, {3}, 1)", item.ID, folderID, (int)type, sortOrder));
                    else
                        implement.Execute(string.Format("update wim_RoleRights set RoleRight_Update = 1, RoleRight_SortOrder = {3}, RoleRight_Type = 1 where RoleRight_Role_Key = {0} and RoleRight_Child_Key = {1} and RoleRight_Child_Type = {2}", item.ID, folderID, (int)type, sortOrder));
                }
            }
            CleanUpFolder(folderID);
        }
        /// <summary>
        /// Updates the specified sub list.
        /// </summary>
        /// <param name="subList">The sub list.</param>
        /// <param name="type">The type.</param>
        /// <param name="roleID">The role ID.</param>
        public static void Update(Sushi.Mediakiwi.Data.SubList subList, RoleRightType type, int roleID)
        {
            Prepare(type, roleID);
            if (subList != null && subList.Items != null && subList.Items.Length > 0)
            {
                int sortOrder = 0;
                RoleRight implement = new RoleRight();

                System.Collections.Hashtable ht = new System.Collections.Hashtable();


                foreach (Sushi.Mediakiwi.Data.SubList.SubListitem item in subList.Items)
                {
                    sortOrder++;
                    int count = Wim.Utility.ConvertToInt(implement.Execute(string.Format("select COUNT(*) FROM wim_RoleRights where RoleRight_Role_Key = {0} and RoleRight_Child_Key = {1} and RoleRight_Child_Type = {2}", roleID, item.ID, (int)type, sortOrder)));
                    if (count == 0)
                        implement.Execute(string.Format("insert into wim_RoleRights (RoleRight_Role_Key, RoleRight_Child_Key, RoleRight_Child_Type, RoleRight_Update, RoleRight_SortOrder, RoleRight_Type) values ({0}, {1}, {2}, 1, {3}, 1)", roleID, item.ID, (int)type, sortOrder));
                    else
                        implement.Execute(string.Format("update wim_RoleRights set RoleRight_Update = 1, RoleRight_SortOrder = {3}, RoleRight_Type = 1 where RoleRight_Role_Key = {0} and RoleRight_Child_Key = {1} and RoleRight_Child_Type = {2}", roleID, item.ID, (int)type, sortOrder));
                }
            }
            CleanUp(type, roleID);
        }

        /// <summary>
        /// Updates the specified sub list.
        /// </summary>
        /// <param name="subList">The sub list.</param>
        /// <param name="type">The type.</param>
        /// <param name="roleID">The role ID.</param>
        public static void Update(int[] subList, RoleRightType type, int roleID)
        {
            Prepare(type, roleID);
            if (subList != null && subList.Length > 0)
            {
                int sortOrder = 0;
                RoleRight implement = new RoleRight();
                foreach (int item in subList)
                {
                    sortOrder++;

                    int count = Wim.Utility.ConvertToInt(implement.Execute(string.Format("select COUNT(*) FROM wim_RoleRights where RoleRight_Role_Key = {0} and RoleRight_Child_Key = {1} and RoleRight_Child_Type = {2}", roleID, item, (int)type, sortOrder)));
                    if (count == 0)
                        implement.Execute(string.Format("insert into wim_RoleRights (RoleRight_Role_Key, RoleRight_Child_Key, RoleRight_Child_Type, RoleRight_Update, RoleRight_SortOrder, RoleRight_Type) values ({0}, {1}, {2}, 1, {3}, 1)", roleID, item, (int)type, sortOrder));
                    else
                        implement.Execute(string.Format("update wim_RoleRights set RoleRight_Update = 1, RoleRight_SortOrder = {3}, RoleRight_Type = 1 where RoleRight_Role_Key = {0} and RoleRight_Child_Key = {1} and RoleRight_Child_Type = {2}", roleID, item, (int)type, sortOrder));
                }
            }
            CleanUp(type, roleID);
            FlushCache(typeof(RoleRight));
        }

        /// <summary>
        /// Updates the specified granted.
        /// </summary>
        /// <param name="accessListing">The access listing.</param>
        /// <param name="type">The type.</param>
        /// <param name="userOrRoleID">The user or role ID.</param>
        public static void Update(Dictionary<int, Access> accessListing, RoleRightType type, int userOrRoleID)
        {
            Prepare(type, userOrRoleID);
            int sortOrder = 0;
            if (accessListing != null && accessListing.Count > 0)
            {
                foreach (KeyValuePair<int, Access> item in accessListing)
                {
                    if (item.Value == Access.Inherit) continue;

                    sortOrder++;
                    RoleRight implement = new RoleRight();
                    int count = Wim.Utility.ConvertToInt(implement.Execute(string.Format("select COUNT(*) FROM wim_RoleRights where RoleRight_Role_Key = {0} and RoleRight_Child_Key = {1} and RoleRight_Child_Type = {2}", userOrRoleID, item.Key, (int)type, sortOrder)));
                    if (count == 0)
                        implement.Execute(string.Format("insert into wim_RoleRights (RoleRight_Role_Key, RoleRight_Child_Key, RoleRight_Child_Type, RoleRight_Update, RoleRight_SortOrder, RoleRight_Type) values ({0}, {1}, {2}, 1, {3}, {4})", userOrRoleID, item.Key, (int)type, sortOrder, (int)item.Value));
                    else
                        implement.Execute(string.Format("update wim_RoleRights set RoleRight_Update = 1, RoleRight_SortOrder = {3}, RoleRight_Type = {4} where RoleRight_Role_Key = {0} and RoleRight_Child_Key = {1} and RoleRight_Child_Type = {2}", userOrRoleID, item.Key, (int)type, sortOrder, (int)item.Value));
                }
            }
            CleanUp(type, userOrRoleID);
            FlushCache(typeof(RoleRight));
        }

        #endregion REPLICATED TO Sushi.Mediakiwi.Data.Standard
    }
}
