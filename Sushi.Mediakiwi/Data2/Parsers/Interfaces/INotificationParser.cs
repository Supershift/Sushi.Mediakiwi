using System;
using System.Xml;

namespace Sushi.Mediakiwi.Data.Parsers
{
    public interface INotificationParser
    {
        XmlDocument[] ConvertToXml(params object[] document);
        void DeleteAll();
        void DeleteAll(string group);
        int InsertOne(string groupName, string notification);
        int InsertOne(string groupName, Exception exception);
        int InsertOne(string groupName, NotificationType type, string notification);
        int InsertOne(string groupName, string notification, Exception exception);
        int InsertOne(string groupName, NotificationType type, Exception exception);
        int InsertOne(string groupName, Exception exception, params XmlDocument[] xml);
        int InsertOne(string groupName, NotificationType type, IApplicationUser currentApplicationUser, string notification);
        int InsertOne(string groupName, string notification, Exception exception, params XmlDocument[] xml);
        int InsertOne(string groupName, NotificationType type, string notification, params XmlDocument[] xml);
        int InsertOne(string groupName, NotificationType type, IApplicationUser currentApplicationUser, Exception exception);
        int InsertOne(string groupName, NotificationType type, IApplicationUser currentApplicationUser, string notification, params XmlDocument[] xml);
        int InsertOne(string groupName, NotificationType type, IApplicationUser currentApplicationUser, string notification, int? pageID, int? visitorID, params XmlDocument[] xml);
        bool Save(INotification entity);
        INotification[] SelectAll(string group, int selection);
        INotification[] SelectAll(string group, int selection, int? page, int? maxResult, out int maxPageCount);
        string[] SelectAll_Groups();
        INotification SelectOne(int i);
    }
}