using System;
using System.Xml;

namespace Sushi.Mediakiwi.Data
{
    public interface INotification
    {
        DateTime Created { get; set; }
        string Group { get; set; }
        int ID { get; set; }
        int? PageID { get; set; }
        int? Selection { get; set; }
        string Text { get; set; }
        int? UserID { get; set; }
        int? VisitorID { get; set; }
        XmlDocument XML { get; set; }

        bool Save();
    }
}