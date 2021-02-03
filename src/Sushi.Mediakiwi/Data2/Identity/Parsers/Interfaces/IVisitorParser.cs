using System;
using Sushi.Mediakiwi.Data.Identity;

namespace Sushi.Mediakiwi.Data.Identity.Parsers
{
    public interface IVisitorParser
    {
        bool SaveData(IVisitor entity);
        string TicketName { get; }

        void ApplyCampaign(IVisitor entity, int campaignID, bool autoSave);
        bool Clear();
        void ClearCampaign(IVisitor entity, bool autoSave);
        bool Logout(IVisitor entity, int? redirectionPageID);
        //bool Save(IVisitor visitor);
        bool Save(IVisitor entity, bool shouldRememberVisitorForNextVisit = true);
        IVisitor Select();
        IVisitor Select(Guid visitorReference);
        IVisitor[] SelectAllByProfile(int profileId, int visitorID);
        IVisitor SelectOne(int ID);
        IVisitor SetInfoFromCookie();
        void SetCookie(int id, Guid guid, int? profileId, bool shouldRememberProfileForNextVisit);
        void SetCookie(IVisitor entity);
    }
}