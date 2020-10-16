using System;

namespace Sushi.Mediakiwi.Data
{
    public interface IVisitor
    {
        int? ApplicationUserID { get; set; }
        string CookieParserLog { get; set; }
        int? CountryID { get; set; }
        DateTime Created { get; set; }
        CustomData Data { get; set; }
        string DataString { get; set; }
        Guid GUID { get; set; }
        int ID { get; set; }
        bool IsNewInstance { get; }
        bool IsNewVisitor { get; set; }
        string Language { get; set; }
        DateTime? LastLoggedApplicationUserVisit { get; set; }
        DateTime LastRequestDone { get; }
        int LastUpdateMinutes { get; }
        DateTime LastVisit { get; set; }
        Page LastVisitedPage { get; }
        int? ProfileID { get; set; }
        bool RememberMe { get; set; }
        DateTime Updated { get; set; }
    }
}