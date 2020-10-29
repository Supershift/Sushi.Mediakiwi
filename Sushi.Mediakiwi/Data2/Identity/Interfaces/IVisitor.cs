using System;

namespace Sushi.Mediakiwi.Data.Identity
{
    public interface IVisitor
    {
        bool IsNewInstance { get;  }
        int? ApplicationUserID { get; set; }
        string CookieParserLog { get; set; }
        int? CountryID { get; set; }
        DateTime Created { get; set; }
        CustomData Data { get; set; }
        Guid GUID { get; set; }
        int ID { get; set; }
        bool IsLoggedIn { get; set; }
        bool IsNewSession { get; }
        bool IsNewVisitor { get; set; }
        string Language { get; set; }
        DateTime? LastLoggedApplicationUserVisit { get; set; }
        DateTime LastRequestDone { get; }
        int LastUpdateMinutes { get; }
        Page LastVisitedPage { get; }
        int? ProfileID { get; set; }
        bool RememberMe { get; set; }
        DateTime Updated { get; set; }
        DateTime LastVisit { get; set; }

        void ApplyCampaign(int campaignID, bool autoSave);
        void ClearCampaign(bool autoSave);
        bool Logout(int? redirectionPageID);
        bool Save(bool shouldRememberVisitorForNextVisit = true);
        bool SaveData();
        void SetCookie();
        void SetCookie(Guid guid, int? profileId, bool shouldRememberProfileForNextVisit);
        [Obsolete("Use VisitorParser instead")]
        IVisitor SetInfoFromCookie();    
    }
}