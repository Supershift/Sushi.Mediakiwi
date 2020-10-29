using System;

namespace Sushi.Mediakiwi.Data
{
    public interface IApplicationUser
    {
        DateTime Created { get; set; }
        CustomData Data { get; set; }
        string Displayname { get; set; }
        string Email { get; set; }
        Guid GUID { get; set; }
        bool HasVisitorReference { get; }
        int ID { get; set; }
        bool isActive { get; set; }
        bool IsDeveloper { get; set; }
        //bool IsLoggedIn { get; }
        int Language { get; set; }
        string LanguageCulture { get; }
        DateTime LastLoggedVisit { get; set; }
        string Name { get; set; }
        string NetworkIdentification { get; set; }
        string Password { get; set; }
        bool RememberMe { get; set; }
        Guid? ResetKey { get; set; }
        int RoleID { get; set; }
        string RoleName { get; set; }
        bool ShowDetailView { get; set; }
        bool ShowFullWidth { get; set; }
        bool ShowHidden { get; set; }
        //bool ShowNewDesign { get; set; }
        //bool ShowNewDesign2 { get; set; }
        bool ShowSiteNavigation { get; set; }
        bool ShowTranslationView { get; set; }
        // string Skin { get; }
        int Type { get; set; }
        Guid VisitorReference { get; }

        void ApplyPassword(string password);
        bool HasEmail(string email);
        bool HasUserName(string username);
        IApplicationRole Role();
        bool Save();
        //bool Save(bool shouldRememberVisitorForNextVisit);
        //void ExtractLoginMailBody(out string subject, out string body, out string url);
        //void SendForgotPassword();
        //void SendLoginMail();
        //Site[] Sites(AccessFilter accessFilter);

        bool IsNewInstance { get; }

        bool Delete();
        
    }
}