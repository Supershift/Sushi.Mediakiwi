using Sushi.MicroORM.Mapping;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sushi.Mediakiwi.Data.Sql
{
    public class Notification : Data.Notification
    {
        public class NotificationMap : DataMap<Notification>
        {
            public NotificationMap()
            {
                Table("wim_Notifications");
                Id(x => x.ID, "Notification_Key").Identity();
                Map(x => x.Group, "Notification_Type").Length(50);
                Map(x => x.Text, "Notification_Text");
                Map(x => x.Selection, "Notification_Selection");
                Map(x => x.UserID, "Notification_User");
                Map(x => x.VisitorID, "Notification_Visitor_Key");
                Map(x => x.PageID, "Notification_Page_Key");
                Map(x => x.Created, "Notification_Created");
                Map(x => x.XML, "Notification_XML").SqlType(System.Data.SqlDbType.Xml);
            }
        }

        public Notification()
        {

        }

        public Notification(Data.Notification notification)
        {
            Utility.ReflectProperty(notification, this);
        }

        public int ID { get; set; }

        public override string GetIdMessage()
        {
            return ID.ToString();
        }
    }
}
