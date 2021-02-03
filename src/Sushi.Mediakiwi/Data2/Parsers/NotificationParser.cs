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

namespace Sushi.Mediakiwi.Data.Parsers
{
    public class NotificationParser : INotificationParser
    {
        static ISqlEntityParser _DataParser;
        static ISqlEntityParser DataParser
        {
            get
            {
                if (_DataParser == null)
                    _DataParser = Environment.GetInstance<ISqlEntityParser>();
                return _DataParser;
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
        public virtual int InsertOne(string groupName, NotificationType type, Sushi.Mediakiwi.Data.IApplicationUser currentApplicationUser, System.Exception exception)
        {
            return InsertOne(groupName, type, currentApplicationUser, Wim.Utility.GetHtmlFormattedLastServerError(exception));
        }

        /// <summary>
        /// Insert a notification
        /// </summary>
        /// <param name="groupName">Name of the group.</param>
        /// <param name="type">The type.</param>
        /// <param name="exception">The exception.</param>
        /// <returns></returns>
        public virtual int InsertOne(string groupName, NotificationType type, System.Exception exception)
        {
            return InsertOne(groupName, type, Wim.Utility.GetHtmlFormattedLastServerError(exception));
        }

        /// <summary>
        /// Inserts the one.
        /// </summary>
        /// <param name="groupName">Name of the group.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="xml">The XML.</param>
        /// <returns></returns>
        public virtual int InsertOne(string groupName, System.Exception exception, params System.Xml.XmlDocument[] xml)
        {
            return InsertOne(groupName, NotificationType.Error, Wim.Utility.GetHtmlFormattedLastServerError(exception), xml);
        }

        /// <summary>
        /// Inserts the one.
        /// </summary>
        /// <param name="groupName">Name of the group.</param>
        /// <param name="notification">The notification.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="xml">The XML.</param>
        /// <returns></returns>
        public virtual int InsertOne(string groupName, string notification, System.Exception exception, params System.Xml.XmlDocument[] xml)
        {
            return InsertOne(groupName, NotificationType.Error, string.Format("{0}<br/><br/>{1}", notification, Wim.Utility.GetHtmlFormattedLastServerError(exception)), xml);
        }

        /// <summary>
        /// Inserts the one.
        /// </summary>
        /// <param name="groupName">Name of the group.</param>
        /// <param name="notification">The notification.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="xml">The XML.</param>
        /// <returns></returns>
        public virtual int InsertOne(string groupName, string notification, System.Exception exception)
        {
            return InsertOne(groupName, NotificationType.Error, null, string.Format("{0}<br/><br/>{1}", notification, Wim.Utility.GetHtmlFormattedLastServerError(exception)), null);
        }

        /// <summary>
        /// Inserts the one.
        /// </summary>
        /// <param name="groupName">Name of the group.</param>
        /// <param name="exception">The exception.</param>
        /// <returns></returns>
        public virtual int InsertOne(string groupName, System.Exception exception)
        {
            return InsertOne(groupName, NotificationType.Error, Wim.Utility.GetHtmlFormattedLastServerError(exception));
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
        public virtual int InsertOne(string groupName, NotificationType type, Sushi.Mediakiwi.Data.IApplicationUser currentApplicationUser, string notification, params System.Xml.XmlDocument[] xml)
        {
            int? visitorID = null, pageID = null;
            if (HttpContext.Current != null)
            {
                var page = HttpContext.Current.Items["Wim.Page"] as Sushi.Mediakiwi.Data.Page;
                if (page != null && !page.IsNewInstance)
                    pageID = page.ID;

                visitorID = Sushi.Mediakiwi.Data.Identity.Visitor.Select().ID;
            }
            return InsertOne(groupName, type, currentApplicationUser, notification, pageID, visitorID, xml);
        }


        #region REPLICATED TO Sushi.Mediakiwi.Data.Standard


        public virtual bool Save(INotification entity)
        {
            entity.ID = DataParser.Save<INotification>(entity);
            return true;
        }

        /// <summary>
        /// Delete all stored notifications
        /// </summary>
        public virtual void DeleteAll()
        {
            DataParser.Execute("truncate table wim_Notifications");
        }
        /// <summary>
        /// Deletes all.
        /// </summary>
        /// <param name="group">The group.</param>
        public virtual void DeleteAll(string group)
        {
            DataRequest data = new DataRequest();
            data.AddParam("TYPE", group, SqlDbType.NVarChar);
            DataParser.Execute("delete from wim_Notifications where Notification_Type = @TYPE", data);
        }
        /// <summary>
        /// Selects the one.
        /// </summary>
        /// <param name="i">The i.</param>
        /// <returns></returns>
        public virtual INotification SelectOne(int i)
        {
            return DataParser.SelectOne<INotification>(i, false);
        }
        /// <summary>
        /// Selects the all_ groups.
        /// </summary>
        /// <returns></returns>
        public virtual string[] SelectAll_Groups()
        {
            string sql = "select distinct Notification_Type from wim_Notifications order by Notification_Type ASC";
            return DataParser.ExecuteList<string>(sql).ToArray();
        }
        /// <summary>
        /// Selects all.
        /// </summary>
        /// <param name="group">The group.</param>
        /// <param name="selection">The selection.</param>
        /// <returns></returns>
        public virtual INotification[] SelectAll(string group, int selection)
        {
            int maxPageCount;
            return SelectAll(group, selection, null, null, out maxPageCount);
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
        public virtual INotification[] SelectAll(string group, int selection, int? page, int? maxResult, out int maxPageCount)
        {
            DataRequest data = new DataRequest();
            data.AddWhere(nameof(INotification.Group), group);
            data.AddWhere(nameof(INotification.Selection), selection);

            var outcome = DataParser.SelectAll<INotification>(data);
            maxPageCount = outcome.Count;
            return outcome.ToArray();
        }

        /// <summary>
        /// Insert a notification
        /// </summary>
        /// <param name="groupName">Name of the group.</param>
        /// <param name="type">The type.</param>
        /// <param name="notification">The notification.</param>
        /// <returns></returns>
        public virtual int InsertOne(string groupName, NotificationType type, string notification)
        {
            return InsertOne(groupName, type, null, notification);
        }

        /// <summary>
        /// Converts to XML.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <returns></returns>
        public virtual System.Xml.XmlDocument[] ConvertToXml(params object[] document)
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
        /// Insert a notification
        /// </summary>
        /// <param name="groupName">Name of the group.</param>
        /// <param name="type">The type.</param>
        /// <param name="currentApplicationUser">The current application user.</param>
        /// <param name="notification">The notification.</param>
        /// <param name="xml">The XML objects.</param>
        /// <param name="visitorID">The visitor ID.</param>
        /// <returns></returns>
        public virtual int InsertOne(string groupName, NotificationType type, Sushi.Mediakiwi.Data.IApplicationUser currentApplicationUser, string notification, int? pageID, int? visitorID, params System.Xml.XmlDocument[] xml)
        {
            var implement = Environment.GetInstance<INotification>();

            if (xml != null)
            {
                implement.XML = new System.Xml.XmlDocument();
                implement.XML.LoadXml(Wim.Utility.GetSerialized(new XMLArray() { MessageCount = xml.Length }));
                string xml_concat = string.Empty;
                foreach (var item in xml)
                {
                    XmlNode importedDocument = implement.XML.ImportNode(item.DocumentElement, true);
                    implement.XML.DocumentElement.AppendChild(importedDocument);
                }
            }
            
            if (!string.IsNullOrEmpty(groupName))
                groupName = groupName.Trim();

            if (string.IsNullOrEmpty(groupName))
                groupName = "";
            else if (groupName.Length > 50)
                groupName = Wim.Utility.ConvertToFixedLengthText(groupName, 47, "..");

            implement.Group =  groupName;

            if (currentApplicationUser != null)
                implement.UserID = currentApplicationUser.ID;

            implement.Selection = (int)type;
            implement.Text = notification;
            implement.PageID = pageID;
            implement.VisitorID = visitorID;
            Save(implement);
            return implement.ID;
        }

        /// <summary>
        /// Inserts the one.
        /// </summary>
        /// <param name="groupName">Name of the group.</param>
        /// <param name="type">The type.</param>
        /// <param name="notification">The notification.</param>
        /// <param name="xml">The XML objects.</param>
        /// <returns></returns>
        public virtual int InsertOne(string groupName, NotificationType type, string notification, params System.Xml.XmlDocument[] xml)
        {
            return InsertOne(groupName, type, null, notification, xml);
        }

        /// <summary>
        /// Inserts the one.
        /// </summary>
        /// <param name="groupName">Name of the group.</param>
        /// <param name="notification">The notification.</param>
        /// <returns></returns>
        public virtual int InsertOne(string groupName, string notification)
        {
            return InsertOne(groupName, NotificationType.Error, null, notification);
        }

        /// <summary>
        /// Inserts the one.
        /// </summary>
        /// <param name="groupName">Name of the group.</param>
        /// <param name="type">The type.</param>
        /// <param name="currentApplicationUser">The current application user.</param>
        /// <param name="notification">The notification.</param>
        /// <returns></returns>
        public virtual int InsertOne(string groupName, NotificationType type, Sushi.Mediakiwi.Data.IApplicationUser currentApplicationUser, string notification)
        {
            return InsertOne(groupName, type, currentApplicationUser, notification, null);
        }

        public class XMLArray
        {
            public int MessageCount { get; set; }
        }


        #endregion REPLICATED TO Sushi.Mediakiwi.Data.Standard
    }
}