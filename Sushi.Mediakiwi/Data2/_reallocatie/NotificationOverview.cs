using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sushi.Mediakiwi.Data.DalReflection;
using System.Data;

namespace Sushi.Mediakiwi.Data
{
    [DatabaseTable("wim_Notifications", Group = "Notification_Type, Notification_Selection", Order = "MAX(Notification_Created) desc")]
    public class NotificationOverview : DatabaseEntity
    {
        /// <summary>
        /// Selects all historically.
        /// </summary>
        /// <param name="selection">The selection.</param>
        /// <returns></returns>
        public static NotificationOverview[] SelectAllHistorically(int selection)
        {
            NotificationOverview implement = new NotificationOverview();

            List<DatabaseDataValueColumn> where = new List<DatabaseDataValueColumn>();
            where.Add(new DatabaseDataValueColumn("Notification_Selection", SqlDbType.Int, selection));

            //List<Overview> list = new List<Overview>();
            SortedList<string, NotificationOverview> list = new SortedList<string, NotificationOverview>();
            foreach (object o in implement._SelectAll(where))
            {
                NotificationOverview t = (NotificationOverview)o;
                list.Add(string.Concat(t.Calculated, t.Type), t);
            }
            NotificationOverview[] tmp = new NotificationOverview[list.Count];
            list.Values.CopyTo(tmp, 0);
            return tmp;
        }

        /// <summary>
        /// Selects all.
        /// </summary>
        /// <returns></returns>
        public static NotificationOverview[] SelectAll(int selection)
        {
            NotificationOverview implement = new NotificationOverview();
            List<NotificationOverview> list = new List<NotificationOverview>();

            List<DatabaseDataValueColumn> where = new List<DatabaseDataValueColumn>();
            where.Add(new DatabaseDataValueColumn("Notification_Selection", SqlDbType.Int, selection));


            foreach (object o in implement._SelectAll(where))
            {
                list.Add((NotificationOverview)o);
            }
            return list.ToArray();
        }

        [DatabaseColumn("Notification_Type", SqlDbType.NVarChar, Length = 50)]
        public string Type { get; set; }

        [DatabaseColumn("Notification_Selection", SqlDbType.Int)]
        public int Selection { get; set; }

        [DatabaseColumn("Count", SqlDbType.Int, ColumnSubQuery = "COUNT(*)")]
        public int Count { get; set; }

        [DatabaseColumn("Last", SqlDbType.DateTime, ColumnSubQuery = "MAX(Notification_Created)")]
        public DateTime Last { get; set; }

        public string LastInfo
        {
            get
            {
                if (this.Last.Date == DateTime.Today)
                    return string.Format("Today at {0}", Last.ToString("HH:mm"));
                if (this.Last.Date == DateTime.Now.AddDays(-1).Date)
                    return string.Format("Yesterday at {0}", Last.ToString("HH:mm"));

                return string.Format("{0} days ago ({1})", Decimal.Round(Convert.ToDecimal(new TimeSpan(DateTime.Now.Ticks - Last.Ticks).TotalDays), 0), Last.ToString("ddd dd-MMM"));
            }
        }

        internal long Calculated
        {
            get
            {
                return DateTime.MaxValue.Ticks - Last.Ticks;
            }
        }

        /// <summary>
        /// Gets the deeplink.
        /// </summary>
        /// <value>The deeplink.</value>
        public string Deeplink
        {
            get
            {
                var list = Sushi.Mediakiwi.Data.ComponentList.SelectOne(ComponentListType.Notifications);
                return string.Format("list={0}&q={1}", list.ID, this.Type);
            }
        }
    }
}
