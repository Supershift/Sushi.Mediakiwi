using System;
using System.Linq;
using System.Web;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Sushi.Mediakiwi.Data;
using Sushi.Mediakiwi.Data.DalReflection;
using Sushi.Mediakiwi.Framework;
using System.IO;
using System.Xml.Serialization;
using System.Xml;
using Sushi.Mediakiwi.Data.Parsers;

namespace Sushi.Mediakiwi.Data
{
    [DatabaseTable("wim_Notifications", Order = "Notification_Key desc")]
    public class Notification : INotification
    {
        static INotificationParser _Parser;
        static INotificationParser Parser
        {
            get
            {
                if (_Parser == null)
                    _Parser = Environment.GetInstance<INotificationParser>();
                return _Parser;
            }
        }

        /// <summary>
        /// Insert a notification
        /// </summary>
        /// <param name="groupName">Name of the group.</param>
        /// <param name="type">The type.</param>
        /// <param name="currentApplicationUser">The current application user.</param>
        /// <param name="exception">The exception.</param>
        /// <returns></returns>
        public static int InsertOne(string groupName, NotificationType type, IApplicationUser currentApplicationUser, Exception exception)
        {
            return Parser.InsertOne(groupName, type, currentApplicationUser, Utility.GetHtmlFormattedLastServerError(exception));
        }

        /// <summary>
        /// Insert a notification
        /// </summary>
        /// <param name="groupName">Name of the group.</param>
        /// <param name="type">The type.</param>
        /// <param name="exception">The exception.</param>
        /// <returns></returns>
        public static int InsertOne(string groupName, NotificationType type, Exception exception)
        {
            return Parser.InsertOne(groupName, type, Utility.GetHtmlFormattedLastServerError(exception));
        }

        /// <summary>
        /// Inserts the one.
        /// </summary>
        /// <param name="groupName">Name of the group.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="xml">The XML.</param>
        /// <returns></returns>
        public static int InsertOne(string groupName, Exception exception, params XmlDocument[] xml)
        {
            return Parser.InsertOne(groupName, NotificationType.Error, Utility.GetHtmlFormattedLastServerError(exception), xml);
        }

        /// <summary>
        /// Inserts the one.
        /// </summary>
        /// <param name="groupName">Name of the group.</param>
        /// <param name="notification">The notification.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="xml">The XML.</param>
        /// <returns></returns>
        public static int InsertOne(string groupName, string notification, Exception exception, params XmlDocument[] xml)
        {
            return Parser.InsertOne(groupName, NotificationType.Error, string.Format("{0}<br/><br/>{1}", notification, Utility.GetHtmlFormattedLastServerError(exception)), xml);
        }

        /// <summary>
        /// Inserts the one.
        /// </summary>
        /// <param name="groupName">Name of the group.</param>
        /// <param name="notification">The notification.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="xml">The XML.</param>
        /// <returns></returns>
        public static int InsertOne(string groupName, string notification, Exception exception)
        {
            return Parser.InsertOne(groupName, NotificationType.Error, null, string.Format("{0}<br/><br/>{1}", notification, Utility.GetHtmlFormattedLastServerError(exception)), null);
        }

        /// <summary>
        /// Inserts the one.
        /// </summary>
        /// <param name="groupName">Name of the group.</param>
        /// <param name="exception">The exception.</param>
        /// <returns></returns>
        public static int InsertOne(string groupName, Exception exception)
        {
            return Parser.InsertOne(groupName, NotificationType.Error, Utility.GetHtmlFormattedLastServerError(exception));
        }

        /// <summary>
        /// Inserts the one.
        /// </summary>
        /// <param name="groupName">Name of the group.</param>
        /// <param name="type">The type.</param>
        /// <param name="notification">The notification.</param>
        /// <param name="xml">The XML objects.</param>
        /// <returns></returns>
        public static int InsertOne(string groupName, NotificationType type, string notification, params XmlDocument[] xml)
        {
            return Parser.InsertOne(groupName, type, null, notification, xml);
        }

        #region REPLICATED TO Sushi.Mediakiwi.Data.Standard

        public class Tags
        {
            /// <summary>
            /// Gets the internal wim error.
            /// </summary>
            /// <value>The internal wim error.</value>
            public static string InternalWimError
            {
                get { return "Internal Wim error"; }
            }

            /// <summary>
            /// Gets the internal wim info.
            /// </summary>
            /// <value>The internal wim info.</value>
            public static string InternalWimInfo
            {
                get { return "Internal Wim info"; }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Notification()
        {
            Selection = (int)NotificationType.Error;
            Created = DateTime.UtcNow;// Sushi.Mediakiwi.Data.Common.DatabaseDateTime;
        }

        #region Properties
        /// <summary>
        /// 
        /// </summary>
        [DatabaseColumn("Notification_Key", SqlDbType.Int, IsPrimaryKey = true)]
        public virtual int ID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [DatabaseColumn("Notification_Type", SqlDbType.NVarChar, Length = 50)]
        public virtual string Group { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [DatabaseColumn("Notification_Text", SqlDbType.NText, IsNullable = true)]
        public virtual string Text { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [DatabaseColumn("Notification_Selection", SqlDbType.Int, IsNullable = true)]
        public virtual int? Selection { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [DatabaseColumn("Notification_User", SqlDbType.Int, IsNullable = true)]
        public virtual int? UserID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [DatabaseColumn("Notification_Visitor_Key", SqlDbType.Int, IsNullable = true)]
        public virtual int? VisitorID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [DatabaseColumn("Notification_Page_Key", SqlDbType.Int, IsNullable = true)]
        public virtual int? PageID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [DatabaseColumn("Notification_Created", SqlDbType.DateTime, IsNullable = true)]
        public virtual DateTime Created { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [DatabaseColumn("Notification_XML", SqlDbType.Xml, IsNullable = true)]
        public virtual System.Xml.XmlDocument XML { get; set; }
        #endregion Properties

        public bool Save()
        {
            return Parser.Save(this);
        }

        /// <summary>
        /// Delete all stored notifications
        /// </summary>
        public static void DeleteAll()
        {
            Parser.DeleteAll();
        }
        /// <summary>
        /// Deletes all.
        /// </summary>
        /// <param name="group">The group.</param>
        public static void DeleteAll(string group)
        {
            Parser.DeleteAll(group);
        }
        /// <summary>
        /// Selects the one.
        /// </summary>
        /// <param name="i">The i.</param>
        /// <returns></returns>
        public static INotification SelectOne(int i)
        {
            return Parser.SelectOne(i);
        }
        /// <summary>
        /// Selects the all_ groups.
        /// </summary>
        /// <returns></returns>
        public static string[] SelectAll_Groups()
        {
            return Parser.SelectAll_Groups();
        }
        /// <summary>
        /// Selects all.
        /// </summary>
        /// <param name="group">The group.</param>
        /// <param name="selection">The selection.</param>
        /// <returns></returns>
        public static INotification[] SelectAll(string group, int selection)
        {
            return Parser.SelectAll(group, selection);
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
        public static INotification[] SelectAll(string group, int selection, int? page, int? maxResult, out int maxPageCount)
        {
            return Parser.SelectAll(group, selection, page, maxResult, out maxPageCount);
        }

        /// <summary>
        /// Insert a notification
        /// </summary>
        /// <param name="groupName">Name of the group.</param>
        /// <param name="type">The type.</param>
        /// <param name="notification">The notification.</param>
        /// <returns></returns>
        public static int InsertOne(string groupName, NotificationType type, string notification)
        {
            return Parser.InsertOne(groupName, type, null, notification);
        }

        /// <summary>
        /// Converts to XML.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <returns></returns>
        public static System.Xml.XmlDocument[] ConvertToXml(params object[] document)
        {
            List<System.Xml.XmlDocument> tmp = new List<XmlDocument>();
            foreach (var item in document)
            {
                if (item == null)
                    continue;

                System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
                doc.LoadXml(Wim.Utility.GetSerialized(item));
                tmp.Add(doc);
            }
            return tmp.ToArray();
        }



        /// <summary>
        /// Inserts the one.
        /// </summary>
        /// <param name="groupName">Name of the group.</param>
        /// <param name="notification">The notification.</param>
        /// <returns></returns>
        public static int InsertOne(string groupName, string notification)
        {
            return Parser.InsertOne(groupName, NotificationType.Error, null, notification);
        }

        /// <summary>
        /// Inserts the one.
        /// </summary>
        /// <param name="groupName">Name of the group.</param>
        /// <param name="type">The type.</param>
        /// <param name="currentApplicationUser">The current application user.</param>
        /// <param name="notification">The notification.</param>
        /// <returns></returns>
        public static int InsertOne(string groupName, NotificationType type, Sushi.Mediakiwi.Data.IApplicationUser currentApplicationUser, string notification)
        {
            return Parser.InsertOne(groupName, type, currentApplicationUser, notification, null);
        }


        /// <summary>
        /// Inserts the one.
        /// </summary>
        /// <param name="groupName">Name of the group.</param>
        /// <param name="type">The type.</param>
        /// <param name="currentApplicationUser">The current application user.</param>
        /// <param name="notification">The notification.</param>
        /// <param name="xml">The XML.</param>
        /// <returns></returns>
        public static int InsertOne(string groupName, NotificationType type, Sushi.Mediakiwi.Data.ApplicationUser currentApplicationUser, string notification, params System.Xml.XmlDocument[] xml)
        {
            return Parser.InsertOne(groupName, type, currentApplicationUser, notification, xml);
        }

        /// <summary>
        /// Insert a notification
        /// </summary>
        /// <param name="groupName">Name of the group.</param>
        /// <param name="type">The type.</param>
        /// <param name="currentApplicationUser">The current application user.</param>
        /// <param name="notification">The notification.</param>
        /// <param name="xml">The XML objects.</param>
        /// <param name="visitorID">The visitor ID.</param>
        /// <returns></returns>
        public static int InsertOne(string groupName, NotificationType type, Sushi.Mediakiwi.Data.ApplicationUser currentApplicationUser, string notification, int? pageID, int? visitorID, params System.Xml.XmlDocument[] xml)
        {
            return Parser.InsertOne(groupName, type, currentApplicationUser, notification, pageID, visitorID, xml);
        }


        #endregion REPLICATED TO Sushi.Mediakiwi.Data.Standard
    }
}