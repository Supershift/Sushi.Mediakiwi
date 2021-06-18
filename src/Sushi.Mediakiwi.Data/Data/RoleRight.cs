using Sushi.MicroORM.Mapping;
using System;
using System.Threading.Tasks;
using Sushi.Mediakiwi.Data.MicroORM;

namespace Sushi.Mediakiwi.Data
{
    /// <summary>
    /// Represents a RoleRight entity.
    /// </summary>
    [DataMap(typeof(RoleRightMap))]
    public class RoleRight
    {
        public class RoleRightMap : DataMap<RoleRight>
        {
            public RoleRightMap()
            {
                Table("wim_RoleRights");
                Map(x => x.RoleID, "RoleRight_Role_Key");
                Map(x => x.ItemID, "RoleRight_Child_Key");
                Map(x => x.TypeID, "RoleRight_Child_Type");
                Map(x => x.AccessType, "RoleRight_Type");
                Map(x => x.SortOrder, "RoleRight_SortOrder");
            }
        }

        #region Properties

        public int SortOrder { get; set; }

        /// <summary>
        /// Gets or sets the role ID.
        /// </summary>
        /// <value>The role ID.</value>
        public int RoleID { get; set; }

        /// <summary>
        /// Gets or sets the item ID.
        /// </summary>
        /// <value>The item ID.</value>
        public int ItemID { get; set; }

        /// <summary>
        /// Gets or sets the type ID.
        /// </summary>
        /// <value>The type ID.</value>
        public int TypeID { get; set; }

        /// <summary>
        /// Gets or sets the type ID.
        /// </summary>
        /// <value>The type ID.</value>
        public Access AccessType { get; set; }

        #endregion Properties

        /// <summary>
        /// Updates the specified sub list.
        /// </summary>
        /// <param name="subList">The sub list.</param>
        /// <param name="folderID">The folder ID.</param>
        [Obsolete("[MJ:26-02-2020] Old UpdateFolder function is not used any where. Is this an obsolete method?")]
        public static void UpdateFolder(SubList subList, int folderID)
        {
            var connector = ConnectorFactory.CreateConnector<RoleRight>();

            RoleRightType type = RoleRightType.Folder;

            PrepareFolder(folderID);
            if (subList != null && subList.Items != null && subList.Items.Length > 0)
            {
                int sortOrder = 0;
                foreach (SubList.SubListitem item in subList.Items)
                {
                    var filter = connector.CreateDataFilter();

                    filter.AddParameter("@itemID", item.ID);
                    filter.AddParameter("@folderID", folderID);
                    filter.AddParameter("@typeID", (int)type);

                    sortOrder++;
                    filter.AddParameter("@sortOrder", sortOrder);

                    int count = connector.ExecuteScalar<int>("SELECT COUNT(*) FROM [wim_RoleRights] where [RoleRight_Role_Key] = @itemID AND [RoleRight_Child_Key] = @folderID AND [RoleRight_Child_Type] = @typeID", filter);
                    if (count == 0)
                        connector.ExecuteNonQuery("INSERT INTO [Wim_RoleRights] ([RoleRight_Role_Key], [RoleRight_Child_Key], [RoleRight_Child_Type], [RoleRight_Update], [RoleRight_SortOrder], [RoleRight_Type]) VALUES (@itemID, @folderID, @typeID, 1, @sortOrder, 1)", filter);
                    else
                        connector.ExecuteNonQuery("UPDATE [Wim_RoleRights] SET [RoleRight_Update] = 1, [RoleRight_SortOrder] = @sortOrder, [RoleRight_Type] = 1 WHERE [RoleRight_Role_Key] = @itemID AND [RoleRight_Child_Key] = @folderID AND [RoleRight_Child_Type] = @typeID", filter);
                    connector.Cache?.FlushRegion(connector.CacheRegion);
                }
            }
            CleanUpFolder(folderID);
        }

        /// <summary>
        /// Updates the specified sub list.
        /// </summary>
        /// <param name="subList">The sub list.</param>
        /// <param name="folderID">The folder ID.</param>
        [Obsolete("[MJ:26-02-2020] Old UpdateFolder function is not used any where. Is this an obsolete method?")]
        public static async Task UpdateFolderAsync(SubList subList, int folderID)
        {
            var connector = ConnectorFactory.CreateConnector<RoleRight>();

            RoleRightType type = RoleRightType.Folder;

            await PrepareFolderAsync(folderID);
            if (subList != null && subList.Items != null && subList.Items.Length > 0)
            {
                int sortOrder = 0;
                foreach (SubList.SubListitem item in subList.Items)
                {
                    var filter = connector.CreateDataFilter();

                    filter.AddParameter("@itemID", item.ID);
                    filter.AddParameter("@folderID", folderID);
                    filter.AddParameter("@typeID", (int)type);

                    sortOrder++;
                    filter.AddParameter("@sortOrder", sortOrder);

                    int count = await connector.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM [wim_RoleRights] where [RoleRight_Role_Key] = @itemID AND [RoleRight_Child_Key] = @folderID AND [RoleRight_Child_Type] = @typeID", filter);
                    if (count == 0)
                        await connector.ExecuteNonQueryAsync("INSERT INTO [Wim_RoleRights] ([RoleRight_Role_Key], [RoleRight_Child_Key], [RoleRight_Child_Type], [RoleRight_Update], [RoleRight_SortOrder], [RoleRight_Type]) VALUES (@itemID, @folderID, @typeID, 1, @sortOrder, 1)", filter);
                    else
                        await connector.ExecuteNonQueryAsync("UPDATE [Wim_RoleRights] SET [RoleRight_Update] = 1, [RoleRight_SortOrder] = @sortOrder, [RoleRight_Type] = 1 WHERE [RoleRight_Role_Key] = @itemID AND [RoleRight_Child_Key] = @folderID AND [RoleRight_Child_Type] = @typeID", filter);
                    connector.Cache?.FlushRegion(connector.CacheRegion);
                }
            }
            await CleanUpFolderAsync(folderID);
        }

        /// <summary>
        /// Updates the specified sub list.
        /// </summary>
        /// <param name="subList">The sub list.</param>
        /// <param name="type">The type.</param>
        /// <param name="roleID">The role ID.</param>
        public static void Update(SubList subList, RoleRightType type, int roleID)
        {
            Prepare(type, roleID);

            var connector = ConnectorFactory.CreateConnector<RoleRight>();
            if (subList != null && subList.Items != null && subList.Items.Length > 0)
            {
                int sortOrder = 0;
                System.Collections.Hashtable ht = new System.Collections.Hashtable();

                foreach (SubList.SubListitem item in subList.Items)
                {
                    var filter = connector.CreateDataFilter();

                    filter.AddParameter("@itemID", roleID);
                    filter.AddParameter("@folderID", item.ID);
                    filter.AddParameter("@typeID", (int)type);

                    sortOrder++;
                    filter.AddParameter("@sortOrder", sortOrder);

                    int count = connector.ExecuteScalar<int>("SELECT COUNT(*) FROM [wim_RoleRights] where [RoleRight_Role_Key] = @itemID AND [RoleRight_Child_Key] = @folderID AND [RoleRight_Child_Type] = @typeID", filter);
                    if (count == 0)
                        connector.ExecuteNonQuery("INSERT INTO [Wim_RoleRights] ([RoleRight_Role_Key], [RoleRight_Child_Key], [RoleRight_Child_Type], [RoleRight_Update], [RoleRight_SortOrder], [RoleRight_Type]) VALUES (@itemID, @folderID, @typeID, 1, @sortOrder, 1)", filter);
                    else
                        connector.ExecuteNonQuery("UPDATE [Wim_RoleRights] SET [RoleRight_Update] = 1, [RoleRight_SortOrder] = @sortOrder, [RoleRight_Type] = 1 WHERE [RoleRight_Role_Key] = @itemID AND [RoleRight_Child_Key] = @folderID AND [RoleRight_Child_Type] = @typeID", filter);
                    connector.Cache?.FlushRegion(connector.CacheRegion);
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
        public static async Task UpdateAsync(SubList subList, RoleRightType type, int roleID)
        {
            Prepare(type, roleID);

            var connector = ConnectorFactory.CreateConnector<RoleRight>();
            if (subList != null && subList.Items != null && subList.Items.Length > 0)
            {
                int sortOrder = 0;
                System.Collections.Hashtable ht = new System.Collections.Hashtable();

                foreach (SubList.SubListitem item in subList.Items)
                {
                    var filter = connector.CreateDataFilter();

                    filter.AddParameter("@itemID", roleID);
                    filter.AddParameter("@folderID", item.ID);
                    filter.AddParameter("@typeID", (int)type);

                    sortOrder++;
                    filter.AddParameter("@sortOrder", sortOrder);

                    int count = await connector.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM [wim_RoleRights] where [RoleRight_Role_Key] = @itemID AND [RoleRight_Child_Key] = @folderID AND [RoleRight_Child_Type] = @typeID", filter);
                    if (count == 0)
                        await connector.ExecuteNonQueryAsync("INSERT INTO [Wim_RoleRights] ([RoleRight_Role_Key], [RoleRight_Child_Key], [RoleRight_Child_Type], [RoleRight_Update], [RoleRight_SortOrder], [RoleRight_Type]) VALUES (@itemID, @folderID, @typeID, 1, @sortOrder, 1)", filter);
                    else
                        await connector.ExecuteNonQueryAsync("UPDATE [Wim_RoleRights] SET [RoleRight_Update] = 1, [RoleRight_SortOrder] = @sortOrder, [RoleRight_Type] = 1 WHERE [RoleRight_Role_Key] = @itemID AND [RoleRight_Child_Key] = @folderID AND [RoleRight_Child_Type] = @typeID", filter);
                    connector.Cache?.FlushRegion(connector.CacheRegion);
                }
            }
            await CleanUpAsync(type, roleID);
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

            var connector = ConnectorFactory.CreateConnector<RoleRight>();
            if (subList != null && subList.Length > 0)
            {
                int sortOrder = 0;
                foreach (int item in subList)
                {
                    var filter = connector.CreateDataFilter();

                    filter.AddParameter("@itemID", roleID);
                    filter.AddParameter("@folderID", item);
                    filter.AddParameter("@typeID", (int)type);

                    sortOrder++;
                    filter.AddParameter("@sortOrder", sortOrder);

                    int count = connector.ExecuteScalar<int>("SELECT COUNT(*) FROM [wim_RoleRights] where [RoleRight_Role_Key] = @itemID AND [RoleRight_Child_Key] = @folderID AND [RoleRight_Child_Type] = @typeID", filter);
                    if (count == 0)
                        connector.ExecuteNonQuery("INSERT INTO [Wim_RoleRights] ([RoleRight_Role_Key], [RoleRight_Child_Key], [RoleRight_Child_Type], [RoleRight_Update], [RoleRight_SortOrder], [RoleRight_Type]) VALUES (@itemID, @folderID, @typeID, 1, @sortOrder, 1)", filter);
                    else
                        connector.ExecuteNonQuery("UPDATE [Wim_RoleRights] SET [RoleRight_Update] = 1, [RoleRight_SortOrder] = @sortOrder, [RoleRight_Type] = 1 WHERE [RoleRight_Role_Key] = @itemID AND [RoleRight_Child_Key] = @folderID AND [RoleRight_Child_Type] = @typeID", filter);
                    connector.Cache?.FlushRegion(connector.CacheRegion);
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
        public static async Task UpdateAsync(int[] subList, RoleRightType type, int roleID)
        {
            await PrepareAsync(type, roleID);

            var connector = ConnectorFactory.CreateConnector<RoleRight>();
            if (subList != null && subList.Length > 0)
            {
                int sortOrder = 0;
                foreach (int item in subList)
                {
                    var filter = connector.CreateDataFilter();

                    filter.AddParameter("@itemID", roleID);
                    filter.AddParameter("@folderID", item);
                    filter.AddParameter("@typeID", (int)type);

                    sortOrder++;
                    filter.AddParameter("@sortOrder", sortOrder);

                    int count = await connector.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM [wim_RoleRights] where [RoleRight_Role_Key] = @itemID AND [RoleRight_Child_Key] = @folderID AND [RoleRight_Child_Type] = @typeID", filter);
                    if (count == 0)
                        await connector.ExecuteNonQueryAsync("INSERT INTO [Wim_RoleRights] ([RoleRight_Role_Key], [RoleRight_Child_Key], [RoleRight_Child_Type], [RoleRight_Update], [RoleRight_SortOrder], [RoleRight_Type]) VALUES (@itemID, @folderID, @typeID, 1, @sortOrder, 1)", filter);
                    else
                        await connector.ExecuteNonQueryAsync("UPDATE [Wim_RoleRights] SET [RoleRight_Update] = 1, [RoleRight_SortOrder] = @sortOrder, [RoleRight_Type] = 1 WHERE [RoleRight_Role_Key] = @itemID AND [RoleRight_Child_Key] = @folderID AND [RoleRight_Child_Type] = @typeID", filter);
                    connector.Cache?.FlushRegion(connector.CacheRegion);
                }
            }
            await CleanUpAsync(type, roleID);
        }


        /// <summary>
        /// Updates the specified granted.
        /// </summary>
        /// <param name="accessListing">The access listing.</param>
        /// <param name="type">The type.</param>
        /// <param name="userOrRoleID">The user or role ID.</param>
        public static void Update(System.Collections.Generic.Dictionary<int, Access> accessListing, RoleRightType type, int userOrRoleID)
        {
            Prepare(type, userOrRoleID);
            int sortOrder = 0;

            var connector = ConnectorFactory.CreateConnector<RoleRight>();
            if (accessListing != null && accessListing.Count > 0)
            {
                foreach (System.Collections.Generic.KeyValuePair<int, Access> item in accessListing)
                {
                    if (item.Value == Access.Inherit) continue;

                    var filter = connector.CreateDataFilter();

                    filter.AddParameter("@itemID", userOrRoleID);
                    filter.AddParameter("@folderID", item.Key);
                    filter.AddParameter("@typeID", (int)type);
                    filter.AddParameter("@value", item.Value);

                    sortOrder++;
                    filter.AddParameter("@sortOrder", sortOrder);

                    int count = connector.ExecuteScalar<int>("SELECT COUNT(*) FROM [wim_RoleRights] where [RoleRight_Role_Key] = @itemID AND [RoleRight_Child_Key] = @folderID AND [RoleRight_Child_Type] = @typeID", filter);
                    if (count == 0)
                        connector.ExecuteNonQuery("INSERT INTO [Wim_RoleRights] ([RoleRight_Role_Key], [RoleRight_Child_Key], [RoleRight_Child_Type], [RoleRight_Update], [RoleRight_SortOrder], [RoleRight_Type]) VALUES (@itemID, @folderID, @typeID, 1, @sortOrder, @value)", filter);
                    else
                        connector.ExecuteNonQuery("UPDATE [Wim_RoleRights] SET [RoleRight_Update] = 1, [RoleRight_SortOrder] = @sortOrder, [RoleRight_Type] = @value WHERE [RoleRight_Role_Key] = @itemID AND [RoleRight_Child_Key] = @folderID AND [RoleRight_Child_Type] = @typeID", filter);
                    connector.Cache?.FlushRegion(connector.CacheRegion);
                }
            }
            CleanUp(type, userOrRoleID);
        }


        /// <summary>
        /// Updates the specified granted.
        /// </summary>
        /// <param name="accessListing">The access listing.</param>
        /// <param name="type">The type.</param>
        /// <param name="userOrRoleID">The user or role ID.</param>
        public static async Task UpdateAsync(System.Collections.Generic.Dictionary<int, Access> accessListing, RoleRightType type, int userOrRoleID)
        {
            await PrepareAsync(type, userOrRoleID);
            int sortOrder = 0;

            var connector = ConnectorFactory.CreateConnector<RoleRight>();
            if (accessListing != null && accessListing.Count > 0)
            {
                foreach (System.Collections.Generic.KeyValuePair<int, Access> item in accessListing)
                {
                    if (item.Value == Access.Inherit) continue;

                    var filter = connector.CreateDataFilter();

                    filter.AddParameter("@itemID", userOrRoleID);
                    filter.AddParameter("@folderID", item.Key);
                    filter.AddParameter("@typeID", (int)type);
                    filter.AddParameter("@value", item.Value);

                    sortOrder++;
                    filter.AddParameter("@sortOrder", sortOrder);

                    int count = await connector.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM [wim_RoleRights] where [RoleRight_Role_Key] = @itemID AND [RoleRight_Child_Key] = @folderID AND [RoleRight_Child_Type] = @typeID", filter);
                    if (count == 0)
                        await connector.ExecuteNonQueryAsync("INSERT INTO [Wim_RoleRights] ([RoleRight_Role_Key], [RoleRight_Child_Key], [RoleRight_Child_Type], [RoleRight_Update], [RoleRight_SortOrder], [RoleRight_Type]) VALUES (@itemID, @folderID, @typeID, 1, @sortOrder, @value)", filter);
                    else
                        await connector.ExecuteNonQueryAsync("UPDATE [Wim_RoleRights] SET [RoleRight_Update] = 1, [RoleRight_SortOrder] = @sortOrder, [RoleRight_Type] = @value WHERE [RoleRight_Role_Key] = @itemID AND [RoleRight_Child_Key] = @folderID AND [RoleRight_Child_Type] = @typeID", filter);
                    connector.Cache?.FlushRegion(connector.CacheRegion);
                }
            }
            await CleanUpAsync(type, userOrRoleID);
        }

        /// <summary>
        /// Selects all.
        /// </summary>
        /// <returns></returns>
        public static RoleRight[] SelectAll()
        {
            var connector = ConnectorFactory.CreateConnector<RoleRight>();
            var filter = connector.CreateDataFilter();
            filter.AddOrder(x => x.ItemID);
            filter.AddOrder(x => x.SortOrder);
            return connector.FetchAll(filter).ToArray();
        }

        /// <summary>
        /// Selects all.
        /// </summary>
        /// <returns></returns>
        public static async Task<RoleRight[]> SelectAllAsync()
        {
            var connector = ConnectorFactory.CreateConnector<RoleRight>();
            var filter = connector.CreateDataFilter();
            filter.AddOrder(x => x.ItemID);
            filter.AddOrder(x => x.SortOrder);

            var result = await connector.FetchAllAsync(filter);
            return result.ToArray();
        }

        /// <summary>
        /// Selects all by Role ID.
        /// </summary>
        /// <param name="roleID">The role ID.</param>
        /// <returns></returns>
        public static RoleRight[] SelectAll(int roleID)
        {
            var connector = ConnectorFactory.CreateConnector<RoleRight>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.RoleID, roleID);
            filter.AddOrder(x => x.ItemID);
            filter.AddOrder(x => x.SortOrder);

            return connector.FetchAll(filter).ToArray();
        }

        /// <summary>
        /// Selects all by Role ID Async.
        /// </summary>
        /// <param name="roleID">The role ID.</param>
        /// <returns></returns>
        public static async Task<RoleRight[]> SelectAllAsync(int roleID)
        {
            var connector = ConnectorFactory.CreateConnector<RoleRight>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.RoleID, roleID);
            filter.AddOrder(x => x.ItemID);
            filter.AddOrder(x => x.SortOrder);

            var result = await connector.FetchAllAsync(filter);
            return result.ToArray();
        }

        /// <summary>
        /// Selects all by Role ID and RoleRightType.
        /// </summary>
        /// <param name="roleID">The role ID.</param>
        /// <param name="type">The RoleRightType.</param>
        /// <returns></returns>
        public static RoleRight[] SelectAll(int roleID, RoleRightType type)
        {
            var connector = ConnectorFactory.CreateConnector<RoleRight>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.RoleID, roleID);
            filter.Add(x => x.TypeID, (int)type);
            filter.AddOrder(x => x.ItemID);
            filter.AddOrder(x => x.SortOrder);

            return connector.FetchAll(filter).ToArray();
        }

        /// <summary>
        /// Selects all by Role ID and RoleRightType Async.
        /// </summary>
        /// <param name="roleID">The role ID.</param>
        /// <param name="type">The RoleRightType.</param>
        /// <returns></returns>
        public static async Task<RoleRight[]> SelectAllAsync(int roleID, RoleRightType type)
        {
            var connector = ConnectorFactory.CreateConnector<RoleRight>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.RoleID, roleID);
            filter.Add(x => x.TypeID, (int)type);
            filter.AddOrder(x => x.ItemID);
            filter.AddOrder(x => x.SortOrder);

            var result = await connector.FetchAllAsync(filter);
            return result.ToArray();
        }

        /// <summary>
        /// Prepares the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="roleID">The role ID.</param>
        private static void Prepare(RoleRightType type, int roleID)
        {
            var connector = ConnectorFactory.CreateConnector<RoleRight>();
            var filter = connector.CreateDataFilter();
            filter.AddParameter("@roleID", roleID);
            filter.AddParameter("@childTypeID", (int)type);

            connector.ExecuteNonQuery("UPDATE [Wim_RoleRights] SET [RoleRight_Update] = 0 WHERE [RoleRight_Role_Key] = @roleID and [RoleRight_Child_Type] = @childTypeID", filter);
            connector.Cache?.FlushRegion(connector.CacheRegion);
        }

        /// <summary>
        /// Prepares the specified type Async.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="roleID">The role ID.</param>
        private static async Task PrepareAsync(RoleRightType type, int roleID)
        {
            var connector = ConnectorFactory.CreateConnector<RoleRight>();
            var filter = connector.CreateDataFilter();
            filter.AddParameter("@roleID", roleID);
            filter.AddParameter("@childTypeID", (int)type);

            await connector.ExecuteNonQueryAsync("UPDATE [Wim_RoleRights] SET [RoleRight_Update] = 0 WHERE [RoleRight_Role_Key] = @roleID AND [RoleRight_Child_Type] = @childTypeID", filter);
            connector.Cache?.FlushRegion(connector.CacheRegion);
        }

        /// <summary>
        /// Prepares the RoleRights for an update by folder.
        /// </summary>
        /// <param name="folderID">The folder ID.</param>
        private static void PrepareFolder(int folderID)
        {
            RoleRightType type = RoleRightType.Folder;

            var connector = ConnectorFactory.CreateConnector<RoleRight>();
            var filter = connector.CreateDataFilter();
            filter.AddParameter("@childID", folderID);
            filter.AddParameter("@childTypeID", (int)type);

            connector.ExecuteNonQuery("UPDATE [Wim_RoleRights] set [RoleRight_Update] = 0 where [RoleRight_Child_Key] = @childID AND [RoleRight_Child_Type] = @childTypeID", filter);
            connector.Cache?.FlushRegion(connector.CacheRegion);
        }

        /// <summary>
        /// Prepares the RoleRights for an update by folder Async.
        /// </summary>
        /// <param name="folderID">The folder ID.</param>
        private static async Task PrepareFolderAsync(int folderID)
        {
            RoleRightType type = RoleRightType.Folder;

            var connector = ConnectorFactory.CreateConnector<RoleRight>();
            var filter = connector.CreateDataFilter();
            filter.AddParameter("@childID", folderID);
            filter.AddParameter("@childTypeID", (int)type);

            await connector.ExecuteNonQueryAsync("UPDATE [Wim_RoleRights] set [RoleRight_Update] = 0 where [RoleRight_Child_Key] = @childID AND [RoleRight_Child_Type] = @childTypeID", filter);
            connector.Cache?.FlushRegion(connector.CacheRegion);
        }

        /// <summary>
        /// Cleans up the RoleRights by folder.
        /// </summary>
        /// <param name="folderID">The folder ID.</param>
        private static void CleanUpFolder(int folderID)
        {
            RoleRightType type = RoleRightType.Folder;

            var connector = ConnectorFactory.CreateConnector<RoleRight>();
            var filter = connector.CreateDataFilter();
            filter.AddParameter("@childID", folderID);
            filter.AddParameter("@childTypeID", (int)type);

            connector.ExecuteNonQuery("DELETE FROM [Wim_RoleRights] WHERE [RoleRight_Update] = 0 AND [RoleRight_Child_Key] = @childID AND [RoleRight_Child_Type] = @childTypeID", filter);
            connector.Cache?.FlushRegion(connector.CacheRegion);
        }

        /// <summary>
        /// Cleans up the RoleRights by folder Async.
        /// </summary>
        /// <param name="folderID">The folder ID.</param>
        private static async Task CleanUpFolderAsync(int folderID)
        {
            RoleRightType type = RoleRightType.Folder;

            var connector = ConnectorFactory.CreateConnector<RoleRight>();
            var filter = connector.CreateDataFilter();
            filter.AddParameter("@childID", folderID);
            filter.AddParameter("@childTypeID", (int)type);

            await connector.ExecuteNonQueryAsync("DELETE FROM [Wim_RoleRights] WHERE [RoleRight_Update] = 0 AND [RoleRight_Child_Key] = @childID AND [RoleRight_Child_Type] = @childTypeID", filter);
            connector.Cache?.FlushRegion(connector.CacheRegion);
        }

        /// <summary>
        /// Cleans up the RoleRights by RoleRightType and RoleID.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="roleID">The role ID.</param>
        private static void CleanUp(RoleRightType type, int roleID)
        {
            var connector = ConnectorFactory.CreateConnector<RoleRight>();
            var filter = connector.CreateDataFilter();
            filter.AddParameter("@roleID", roleID);
            filter.AddParameter("@childTypeID", (int)type);

            connector.ExecuteNonQuery("DELETE FROM [Wim_RoleRights] WHERE [RoleRight_Update] = 0 AND [RoleRight_Role_Key] = @roleID AND [RoleRight_Child_Type] = @childTypeID", filter);
            connector.Cache?.FlushRegion(connector.CacheRegion);
        }

        /// <summary>
        /// Cleans up the RoleRights by RoleRightType and RoleID Async.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="roleID">The role ID.</param>
        private static async Task CleanUpAsync(RoleRightType type, int roleID)
        {
            var connector = ConnectorFactory.CreateConnector<RoleRight>();
            var filter = connector.CreateDataFilter();
            filter.AddParameter("@roleID", roleID);
            filter.AddParameter("@childTypeID", (int)type);

            await connector.ExecuteNonQueryAsync("DELETE FROM [Wim_RoleRights] WHERE [RoleRight_Update] = 0 AND [RoleRight_Role_Key] = @roleID AND [RoleRight_Child_Type] = @childTypeID", filter);
            connector.Cache?.FlushRegion(connector.CacheRegion);
        }

    }
}