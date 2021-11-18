using Sushi.Mediakiwi.Data.Repositories;
using Sushi.MicroORM.Mapping;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;

namespace Sushi.Mediakiwi.Data
{
    /// <summary>
    /// Represents a base class for log events. 
    /// </summary>
    public class Notification
    {
        /// <summary>
        ///
        /// </summary>
        public Notification()
        {
            Selection = NotificationType.Error;
            Created = DateTime.UtcNow;
        }

        
        public virtual DateTime Created { get; set; }
        public virtual string Group { get; set; }
        public virtual int? PageID { get; set; }
        public virtual NotificationType? Selection { get; set; }
        public virtual string Text { get; set; }
        public virtual int? UserID { get; set; }
        public virtual int? VisitorID { get; set; }
        [Obsolete("Will be removed in future version")]
        public virtual XmlDocument XML { get; set; }

        /// <summary>
        /// Returns a human readable representation of the ID to retrieve this instance from storage.
        /// </summary>
        /// <returns></returns>
        public virtual string GetIdMessage()
        {
            return "unknown";
        }

        /// <summary>
        /// Gets or sets the instance of <see cref="INotificationRepository" /> used when calling static Insert methods on <see cref="Notification"/>.
        /// </summary>
        public static INotificationRepository Repository { get; set; } = new Repositories.Sql.NotificationRepository();

        /// <summary>
        /// Insert a notification
        /// </summary>
        /// <param name="groupName">Name of the group.</param>
        /// <param name="type">The type.</param>
        /// <param name="notification">The notification.</param>
        /// <returns></returns>
        public static Notification InsertOne(string groupName, NotificationType type, string notification)
        {
            return InsertOne(groupName, type, null, notification);
        }

        /// <summary>
        /// Inserts the one.
        /// </summary>
        /// <param name="groupName">Name of the group.</param>
        /// <param name="notification">The notification.</param>
        /// <returns></returns>
        public static Notification InsertOne(string groupName, string notification)
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
        public static Notification InsertOne(string groupName, NotificationType type, IApplicationUser currentApplicationUser, string notification)
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
        public static Notification InsertOne(string groupName, NotificationType type, IApplicationUser currentApplicationUser, string notification, int? pageID, int? visitorID, params XmlDocument[] xml)
        {
            var implement = new Notification();

            if (xml != null && xml.Any())
            {
                implement.XML = new XmlDocument();
                implement.XML.LoadXml(Utility.GetSerialized(new XMLArray() { MessageCount = xml.Length }));
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
                groupName = Utility.ConvertToFixedLengthText(groupName, 47, "..");

            implement.Group = groupName;

            if (currentApplicationUser != null)
                implement.UserID = currentApplicationUser.ID;

            implement.Selection = type;
            implement.Text = notification;
            implement.PageID = pageID;
            implement.VisitorID = visitorID;
            var result = Repository.Save(implement);

            return result;
        }

        /// <summary>
        /// Insert a notification
        /// </summary>
        /// <param name="groupName">Name of the group.</param>
        /// <param name="type">The type.</param>
        /// <param name="notification">The notification.</param>
        /// <returns></returns>
        public static async Task<Notification> InsertOneAsync(string groupName, NotificationType type, string notification)
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
        public static async Task<Notification> InsertOneAsync(string groupName, Exception ex)
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
        public static async Task<Notification> InsertOneAsync(string groupName, string notification)
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
        public static async Task<Notification> InsertOneAsync(string groupName, NotificationType type, IApplicationUser currentApplicationUser, string notification)
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
        public static async Task<Notification> InsertOneAsync(string groupName, NotificationType type, IApplicationUser currentApplicationUser, string notification, int? pageID, int? visitorID, params XmlDocument[] xml)
        {
            var implement = new Notification();

            if (xml != null && xml.Any())
            {
                implement.XML = new XmlDocument();
                implement.XML.LoadXml(Utility.GetSerialized(new XMLArray() { MessageCount = xml.Length }));
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
                groupName = Utility.ConvertToFixedLengthText(groupName, 47, "..");

            implement.Group = groupName;

            if (currentApplicationUser != null)
                implement.UserID = currentApplicationUser.ID;

            implement.Selection = type;
            implement.Text = notification;
            implement.PageID = pageID;
            implement.VisitorID = visitorID;
            var result = await Repository.SaveAsync(implement);

            return result;
        }
    }
}