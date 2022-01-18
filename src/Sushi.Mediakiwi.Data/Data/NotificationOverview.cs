using Sushi.Mediakiwi.Data.MicroORM;
using Sushi.MicroORM.Mapping;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Data
{
    [DataMap(typeof(NotificationOverviewMap))]
    public class NotificationOverview
    {
        /// <summary>
        ///
        /// </summary>
        public NotificationOverview()
        {
        }

        public class NotificationOverviewMap : DataMap<NotificationOverview>
        {
            public NotificationOverviewMap()
            {
                Table("wim_Notifications");
                Map(x => x.Type, "Notification_Type");
                Map(x => x.Selection, "Notification_Selection");
                Map(x => x.Count, "Notification_Count");
                Map(x => x.Last, "Notification_Last");
            }
        }

        /// <summary>
        ///
        /// </summary>
        public virtual string Type { get; set; }

        /// <summary>
        ///
        /// </summary>
        public virtual string Group { get; set; }

        public virtual int Selection { get; set; }

        public virtual int Count { get; set; }
        /// <summary>
        ///
        /// </summary>
        public virtual DateTime Last { get; set; }


        /// <summary>
        /// Selects all.
        /// </summary>
        /// <param name="group">The group.</param>
        /// <param name="selection">The selection.</param>
        /// <param name="page">The page.</param>
        /// <param name="maxResult">The max result.</param>
        /// <param name="maxPageCount">The max page count.</param>
        /// <returns></returns>
        public static List<NotificationOverview> SelectAll(int filterSelection)
        {
            var connector = ConnectorFactory.CreateConnector<NotificationOverview>();
            var filter = connector.CreateQuery();
            filter.AddParameter("type", filterSelection);

            var sql = @"
    select 
	    Notification_Type
    ,	Notification_Selection
    ,	COUNT(*) as Notification_Count
    ,	MAX(Notification_Created) as Notification_Last
    from 
	    wim_Notifications
    where
        Notification_Selection = @type
    group by 
	    Notification_Type, Notification_Selection
    order by 
	    MAX(Notification_Created) desc";

            return connector.FetchAll(sql, filter);
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
        public static async Task<List<NotificationOverview>> SelectAllAsync(int filterSelection)
        {
            var connector = ConnectorFactory.CreateConnector<NotificationOverview>();
            var filter = connector.CreateQuery();
            filter.AddParameter("type", filterSelection);

            var sql = @"
    select 
	    Notification_Type
    ,	Notification_Selection
    ,	COUNT(*) as Notification_Count
    ,	MAX(Notification_Created) as Notification_Last
    from 
	    wim_Notifications
    where
        Notification_Selection = @type
    group by 
	    Notification_Type, Notification_Selection
    order by 
	    MAX(Notification_Created) desc";

            return await connector.FetchAllAsync(sql, filter);
        }
    }
}