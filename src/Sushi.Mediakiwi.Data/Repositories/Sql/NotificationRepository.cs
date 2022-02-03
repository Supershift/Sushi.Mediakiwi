using sql = Sushi.Mediakiwi.Data.Sql;   
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Data.Repositories.Sql
{   
    /// <summary>
    /// Provides methods to store and retrieve instances of <see cref="Notification"/> from a Sql database.
    /// </summary>
    public class NotificationRepository : INotificationRepository
    {
        private readonly Sushi.MicroORM.Connector<sql.Notification> connector = new Sushi.MicroORM.Connector<sql.Notification>();
        
        public Notification Save(Notification notification)
        {
            var sqlNotification = new sql.Notification(notification);
            connector.Save(sqlNotification);
            return sqlNotification;
        }

        public async Task<Notification> SaveAsync(Notification notification)
        {
            var sqlNotification = new sql.Notification(notification);
            await connector.SaveAsync(sqlNotification);
            return sqlNotification;
        }

        /// <summary>
        /// Delete all stored notifications
        /// </summary>
        public void DeleteAll()
        {   
            connector.ExecuteNonQuery("TRUNCATE TABLE [wim_Notifications]");            
        }

        /// <summary>
        /// Delete all stored notifications
        /// </summary>
        public async Task DeleteAllAsync()
        {  
            await connector.ExecuteNonQueryAsync("TRUNCATE TABLE [wim_Notifications]").ConfigureAwait(false);         
        }

        /// <summary>
        /// Deletes all for a specific group (type).
        /// </summary>
        /// <param name="group">The group.</param>
        public void DeleteAll(string group)
        {   
            var filter = connector.CreateQuery();
            filter.AddParameter("@type", group);

            connector.ExecuteNonQuery("DELETE FROM [wim_Notifications] WHERE [Notification_Type] = @TYPE", filter);         
        }

        /// <summary>
        /// Deletes all for a specific group (type).
        /// </summary>
        /// <param name="group">The group.</param>
        public async Task DeleteAllAsync(string group)
        {   
            var filter = connector.CreateQuery();
            filter.AddParameter("@type", group);

            await connector.ExecuteNonQueryAsync("DELETE FROM [wim_Notifications] WHERE [Notification_Type] = @TYPE", filter).ConfigureAwait(false);
        }

        /// <summary>
        /// Selects a single sql.Notification by Identifier.
        /// </summary>
        /// <param name="Id">The i.</param>
        /// <returns></returns>
        public sql.Notification SelectOne(int Id)
        {   
            return connector.FetchSingle(Id);
        }

        /// <summary>
        /// Selects a single sql.Notification by Identifier Async.
        /// </summary>
        /// <param name="Id">The i.</param>
        /// <returns></returns>
        public async Task<sql.Notification> SelectOneAsync(int Id)
        {
            
            return await connector.FetchSingleAsync(Id).ConfigureAwait(false);
        }

        /// <summary>
        /// Selects all the distinct groups.
        /// </summary>
        /// <returns></returns>
        public string[] SelectAll_Groups()
        {   
            string sql = "SELECT DISTINCT [Notification_Type] FROM [wim_Notifications] ORDER BY [Notification_Type] ASC";

            return connector.ExecuteSet<string>(sql).ToArray();
        }

        /// <summary>
        /// Selects all the distinct groups Async.
        /// </summary>
        /// <returns></returns>
        public async Task<string[]> SelectAll_GroupsAsync()
        {
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
        public sql.Notification[] SelectAll(string group, int selection)
        {
            return SelectAll(group, selection, null);
        }

        /// <summary>
        /// Selects all Notifications by Group and Selection Identifier Async.
        /// </summary>
        /// <param name="group">The Group name.</param>
        /// <param name="selection">The Selection Identifier.</param>
        /// <returns></returns>
        public async Task<sql.Notification[]> SelectAllAsync(string group, int selection)
        {
            return await SelectAllAsync(group, selection, null).ConfigureAwait(false);
        }

        /// <summary>
        /// Selects all.
        /// </summary>
        /// <param name="group">The group.</param>
        /// <param name="selection">The selection.</param>
        /// <param name="maxResult">The max result.</param>
        /// <returns></returns>
        public sql.Notification[] SelectAll(string group, int selection, int? maxResult)
        {   
            var filter = connector.CreateQuery();
            filter.AddOrder(x => x.ID, Sushi.MicroORM.SortOrder.DESC);
            if (maxResult.GetValueOrDefault(0) > 0)
            {
                filter.MaxResults = maxResult.Value;
            }

            filter.Add(x => x.Group, group);
            filter.Add(x => x.Selection, selection);

            var result = connector.FetchAll(filter);
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
        public async Task<sql.Notification[]> SelectAllAsync(string group, int selection, int? maxResult)
        {   
            var filter = connector.CreateQuery();
            filter.AddOrder(x => x.ID, Sushi.MicroORM.SortOrder.DESC);
            filter.Add(x => x.Group, group);
            filter.Add(x => x.Selection, selection);
            if (maxResult.GetValueOrDefault(0) > 0)
            {
                filter.MaxResults = maxResult.Value;
            }

            var result = await connector.FetchAllAsync(filter).ConfigureAwait(false);
            return result.ToArray();
        }
    }
}
