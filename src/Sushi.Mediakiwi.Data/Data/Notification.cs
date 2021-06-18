using Sushi.MicroORM.Mapping;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml;
using Sushi.Mediakiwi.Data.MicroORM;

namespace Sushi.Mediakiwi.Data
{
    [DataMap(typeof(NotificationMap))]
    public class Notification : INotification
    {
        /// <summary>
        ///
        /// </summary>
        public Notification()
        {
            Selection = (int)NotificationType.Error;
            Created = DateTime.UtcNow;
        }

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

        #region Properties

        /// <summary>
        ///
        /// </summary>
        public virtual int ID { get; set; }

        /// <summary>
        ///
        /// </summary>
        public virtual string Group { get; set; }

        /// <summary>
        ///
        /// </summary>
        public virtual string Text { get; set; }

        /// <summary>
        ///
        /// </summary>
        public virtual int? Selection { get; set; }

        /// <summary>
        ///
        /// </summary>
        public virtual int? UserID { get; set; }

        /// <summary>
        ///
        /// </summary>
        public virtual int? VisitorID { get; set; }

        /// <summary>
        ///
        /// </summary>
        public virtual int? PageID { get; set; }

        /// <summary>
        ///
        /// </summary>
        public virtual DateTime Created { get; set; }

        /// <summary>
        ///
        /// </summary>
        public virtual XmlDocument XML { get; set; }

        #endregion Properties

        public bool Save()
        {
            var connector = ConnectorFactory.CreateConnector<Notification>();
            connector.Save(this);
            return true;
        }

        public async Task<bool> SaveAsync()
        {
            var connector = ConnectorFactory.CreateConnector<Notification>();
            await connector.SaveAsync(this).ConfigureAwait(false);
            return true;
        }

        /// <summary>
        /// Delete all stored notifications
        /// </summary>
        public static void DeleteAll()
        {
            var connector = ConnectorFactory.CreateConnector<Notification>();
            connector.ExecuteNonQuery("TRUNCATE TABLE [wim_Notifications]");
			connector.Cache?.FlushRegion(connector.CacheRegion);
        }

        /// <summary>
        /// Delete all stored notifications
        /// </summary>
        public static async Task DeleteAllAsync()
        {
            var connector = ConnectorFactory.CreateConnector<Notification>();
            await connector.ExecuteNonQueryAsync("TRUNCATE TABLE [wim_Notifications]").ConfigureAwait(false);
			connector.Cache?.FlushRegion(connector.CacheRegion);
        }

        /// <summary>
        /// Deletes all for a specific group (type).
        /// </summary>
        /// <param name="group">The group.</param>
        public static void DeleteAll(string group)
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
        public static async Task DeleteAllAsync(string group)
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
        public static INotification SelectOne(int Id)
        {
            var connector = ConnectorFactory.CreateConnector<Notification>();
            return connector.FetchSingle(Id);
        }

        /// <summary>
        /// Selects a single Notification by Identifier Async.
        /// </summary>
        /// <param name="Id">The i.</param>
        /// <returns></returns>
        public static async Task<INotification> SelectOneAsync(int Id)
        {
            var connector = ConnectorFactory.CreateConnector<Notification>();
            return await connector.FetchSingleAsync(Id).ConfigureAwait(false);
        }

        /// <summary>
        /// Selects all the distinct groups.
        /// </summary>
        /// <returns></returns>
        public static string[] SelectAll_Groups()
        {
            var connector = ConnectorFactory.CreateConnector<Notification>();
            string sql = "SELECT DISTINCT [Notification_Type] FROM [wim_Notifications] ORDER BY [Notification_Type] ASC";

            return connector.ExecuteSet<string>(sql).ToArray();
        }

        /// <summary>
        /// Selects all the distinct groups Async.
        /// </summary>
        /// <returns></returns>
        public static async Task<string[]> SelectAll_GroupsAsync()
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
        public static INotification[] SelectAll(string group, int selection)
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
        public static async Task<INotification[]> SelectAllAsync(string group, int selection)
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
        public static INotification[] SelectAll(string group, int selection, int? maxResult, out int maxPageCount)
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
        public static async Task<INotification[]> SelectAllAsync(string group, int selection, int? maxResult)
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

        /// <summary>
        /// Insert a notification
        /// </summary>
        /// <param name="groupName">Name of the group.</param>
        /// <param name="type">The type.</param>
        /// <param name="notification">The notification.</param>
        /// <returns></returns>
        public static int InsertOne(string groupName, NotificationType type, string notification)
        {
            return InsertOne(groupName, type, null, notification);
        }

        /// <summary>
        /// Inserts the one.
        /// </summary>
        /// <param name="groupName">Name of the group.</param>
        /// <param name="notification">The notification.</param>
        /// <returns></returns>
        public static int InsertOne(string groupName, string notification)
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
        public static int InsertOne(string groupName, NotificationType type, IApplicationUser currentApplicationUser, string notification)
        {
            return InsertOne(groupName, type, currentApplicationUser, notification, null, null, null);
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
        public static int InsertOne(string groupName, NotificationType type, IApplicationUser currentApplicationUser, string notification, int? pageID, int? visitorID, params System.Xml.XmlDocument[] xml)
        {
            var implement = new Notification();

            if (xml != null && xml.Any())
            {
                implement.XML = new XmlDocument();
                implement.XML.LoadXml(Sushi.Mediakiwi.Data.Utility.GetSerialized(new XMLArray() { MessageCount = xml.Length }));
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
                groupName = Sushi.Mediakiwi.Data.Utility.ConvertToFixedLengthText(groupName, 47, "..");

            implement.Group = groupName;

            if (currentApplicationUser != null)
                implement.UserID = currentApplicationUser.ID;

            implement.Selection = (int)type;
            implement.Text = notification;
            implement.PageID = pageID;
            implement.VisitorID = visitorID;
            implement.Save();

            return implement.ID;
        }

        /// <summary>
        /// Insert a notification
        /// </summary>
        /// <param name="groupName">Name of the group.</param>
        /// <param name="type">The type.</param>
        /// <param name="notification">The notification.</param>
        /// <returns></returns>
        public static async Task<int> InsertOneAsync(string groupName, NotificationType type, string notification)
        {
            return await InsertOneAsync(groupName, type, null, notification).ConfigureAwait(false);
        }

        /// <summary>
        /// Insert a notification
        /// </summary>
        /// <param name="groupName">Name of the group.</param>
        /// <param name="type">The type.</param>
        /// <param name="notification">The notification.</param>
        /// <returns></returns>
        public static async Task<int> InsertOneAsync(string groupName, Exception ex)
        {
            var body = Utility.GetHtmlFormattedLastServerError(ex);
            return await InsertOneAsync(groupName, NotificationType.Error, null, body).ConfigureAwait(false);
        }

        /// <summary>
        /// Inserts the one.
        /// </summary>
        /// <param name="groupName">Name of the group.</param>
        /// <param name="notification">The notification.</param>
        /// <returns></returns>
        public static async Task<int> InsertOneAsync(string groupName, string notification)
        {
            return await InsertOneAsync(groupName, NotificationType.Error, null, notification).ConfigureAwait(false);
        }

        /// <summary>
        /// Inserts the one.
        /// </summary>
        /// <param name="groupName">Name of the group.</param>
        /// <param name="type">The type.</param>
        /// <param name="currentApplicationUser">The current application user.</param>
        /// <param name="notification">The notification.</param>
        /// <returns></returns>
        public static async Task<int> InsertOneAsync(string groupName, NotificationType type, IApplicationUser currentApplicationUser, string notification)
        {
            return await InsertOneAsync(groupName, type, currentApplicationUser, notification, null, null, null).ConfigureAwait(false);
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
        public static async Task<int> InsertOneAsync(string groupName, NotificationType type, IApplicationUser currentApplicationUser, string notification, int? pageID, int? visitorID, params System.Xml.XmlDocument[] xml)
        {
            var implement = new Notification();

            if (xml != null && xml.Any())
            {
                implement.XML = new XmlDocument();
                implement.XML.LoadXml(Sushi.Mediakiwi.Data.Utility.GetSerialized(new XMLArray() { MessageCount = xml.Length }));
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
                groupName = Sushi.Mediakiwi.Data.Utility.ConvertToFixedLengthText(groupName, 47, "..");

            implement.Group = groupName;

            if (currentApplicationUser != null)
                implement.UserID = currentApplicationUser.ID;

            implement.Selection = (int)type;
            implement.Text = notification;
            implement.PageID = pageID;
            implement.VisitorID = visitorID;
            await implement.SaveAsync().ConfigureAwait(false);

            return implement.ID;
        }

        /// <summary>
        /// Converts to XML.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <returns></returns>
        public static XmlDocument[] ConvertToXml(params object[] document)
        {
            List<XmlDocument> tmp = new List<XmlDocument>();
            foreach (var item in document)
            {
                if (item == null)
                    continue;

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(Sushi.Mediakiwi.Data.Utility.GetSerialized(item));
                tmp.Add(doc);
            }
            return tmp.ToArray();
        }
    }
}