using Sushi.Mediakiwi.Data.MicroORM;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Data.Repositories.Sql
{
    /// <summary>
    /// Provides methods to store and retrieve instances of <see cref="Notification"/> from a Sql database.
    /// </summary>
    public class NotificationRepository : INotificationRepository
    {   
        public Notification Save(Notification notification)
        {
            var connector = new Sushi.MicroORM.Connector<Notification>();
            
            connector.Save(notification);
            return notification;
        }

        public async Task<Notification> SaveAsync(Notification notification)
        {
            var connector = new Sushi.MicroORM.Connector<Notification>();            
            
            await connector.SaveAsync(notification);
            return notification;
        }

        /// <summary>
        /// Delete all stored notifications
        /// </summary>
        public void DeleteAll()
        {
            var connector = ConnectorFactory.CreateConnector<Notification>();
            connector.ExecuteNonQuery("TRUNCATE TABLE [wim_Notifications]");
            connector.Cache?.FlushRegion(connector.CacheRegion);
        }

        /// <summary>
        /// Delete all stored notifications
        /// </summary>
        public async Task DeleteAllAsync()
        {
            var connector = ConnectorFactory.CreateConnector<Notification>();
            await connector.ExecuteNonQueryAsync("TRUNCATE TABLE [wim_Notifications]").ConfigureAwait(false);
            connector.Cache?.FlushRegion(connector.CacheRegion);
        }

        /// <summary>
        /// Deletes all for a specific group (type).
        /// </summary>
        /// <param name="group">The group.</param>
        public void DeleteAll(string group)
        {
            var connector = ConnectorFactory.CreateConnector<Notification>();
            var filter = connector.CreateDataFilter();
            filter.AddParameter("@type", group);

            connector.ExecuteNonQuery("DELETE FROM [wim_Notifications] WHERE [Notification_Type] = @TYPE", filter);
            connector.Cache?.FlushRegion(connector.CacheRegion);
        }

        /// <summary>
        /// Deletes all for a specific group (type).
        /// </summary>
        /// <param name="group">The group.</param>
        public async Task DeleteAllAsync(string group)
        {
            var connector = ConnectorFactory.CreateConnector<Notification>();
            var filter = connector.CreateDataFilter();
            filter.AddParameter("@type", group);

            await connector.ExecuteNonQueryAsync("DELETE FROM [wim_Notifications] WHERE [Notification_Type] = @TYPE", filter).ConfigureAwait(false);
            connector.Cache?.FlushRegion(connector.CacheRegion);
        }

        /// <summary>
        /// Selects a single Notification by Identifier.
        /// </summary>
        /// <param name="Id">The i.</param>
        /// <returns></returns>
        public Notification SelectOne(int Id)
        {
            var connector = ConnectorFactory.CreateConnector<Notification>();
            return connector.FetchSingle(Id);
        }

        /// <summary>
        /// Selects a single Notification by Identifier Async.
        /// </summary>
        /// <param name="Id">The i.</param>
        /// <returns></returns>
        public async Task<Notification> SelectOneAsync(int Id)
        {
            var connector = ConnectorFactory.CreateConnector<Notification>();
            return await connector.FetchSingleAsync(Id).ConfigureAwait(false);
        }

        /// <summary>
        /// Selects all the distinct groups.
        /// </summary>
        /// <returns></returns>
        public string[] SelectAll_Groups()
        {
            var connector = ConnectorFactory.CreateConnector<Notification>();
            string sql = "SELECT DISTINCT [Notification_Type] FROM [wim_Notifications] ORDER BY [Notification_Type] ASC";

            return connector.ExecuteSet<string>(sql).ToArray();
        }

        /// <summary>
        /// Selects all the distinct groups Async.
        /// </summary>
        /// <returns></returns>
        public async Task<string[]> SelectAll_GroupsAsync()
        {
            var connector = ConnectorFactory.CreateConnector<Notification>();
            string sql = "SELECT DISTINCT [Notification_Type] FROM [wim_Notifications] ORDER BY [Notification_Type] ASC";

            var result = await connector.ExecuteSetAsync<string>(sql).ConfigureAwait(false);
            return result.ToArray();
        }

        /// <summary>
        /// Selects all Notifications by Group and Selection Identifier.
        /// </summary>
        /// <param name="group">The Group name.</param>
        /// <param name="selection">The Selection Identifier.</param>
        /// <returns></returns>
        public Notification[] SelectAll(string group, int selection)
        {
            int maxPageCount;
            return SelectAll(group, selection, null, out maxPageCount);
        }

        /// <summary>
        /// Selects all Notifications by Group and Selection Identifier Async.
        /// </summary>
        /// <param name="group">The Group name.</param>
        /// <param name="selection">The Selection Identifier.</param>
        /// <returns></returns>
        public async Task<Notification[]> SelectAllAsync(string group, int selection)
        {
            return await SelectAllAsync(group, selection, null).ConfigureAwait(false);
        }

        /// <summary>
        /// Selects all.
        /// </summary>
        /// <param name="group">The group.</param>
        /// <param name="selection">The selection.</param>
        /// <param name="page">The page.</param>
        /// <param name="maxResult">The max result.</param>
        /// <param name="maxPageCount">The max page count.</param>
        /// <returns></returns>
        public Notification[] SelectAll(string group, int selection, int? maxResult, out int maxPageCount)
        {
            var connector = ConnectorFactory.CreateConnector<Notification>();
            var filter = connector.CreateDataFilter();
            filter.AddOrder(x => x.ID);
            if (maxResult.GetValueOrDefault(0) > 0)
                filter.MaxResults = maxResult.Value;

            filter.Add(x => x.Group, group);
            filter.Add(x => x.Selection, selection);

            var result = connector.FetchAll(filter);
            maxPageCount = result.Count;
            return result.ToArray();
        }

        /// <summary>
        /// Selects all Async.
        /// </summary>
        /// <param name="group">The group.</param>
        /// <param name="selection">The selection.</param>
        /// <param name="page">The page.</param>
        /// <param name="maxResult">The max result.</param>
        /// <param name="maxPageCount">The max page count.</param>
        /// <returns></returns>
        public async Task<Notification[]> SelectAllAsync(string group, int selection, int? maxResult)
        {
            var connector = ConnectorFactory.CreateConnector<Notification>();
            var filter = connector.CreateDataFilter();
            filter.AddOrder(x => x.ID);
            filter.Add(x => x.Group, group);
            filter.Add(x => x.Selection, selection);
            if (maxResult.GetValueOrDefault(0) > 0)
                filter.MaxResults = maxResult.Value;

            var result = await connector.FetchAllAsync(filter).ConfigureAwait(false);
            return result.ToArray();
        }
    }
}
