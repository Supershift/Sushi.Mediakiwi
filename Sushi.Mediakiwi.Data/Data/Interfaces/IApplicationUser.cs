using System;
using System.Threading.Tasks;

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
        bool IsActive { get; set; }
        bool IsDeveloper { get; set; }
        int Language { get; set; }
        string LanguageCulture { get; }
        DateTime? LastLoggedVisit { get; set; }
        string Name { get; set; }
        string NetworkIdentification { get; set; }
        string Password { get; set; }
        bool RememberMe { get; set; }
        Guid? ResetKey { get; set; }
        int RoleID { get; set; }
        string RoleName { get; }
        bool ShowDetailView { get; set; }
        bool ShowFullWidth { get; set; }
        bool ShowHidden { get; set; }

        bool ShowSiteNavigation { get; set; }
        bool ShowTranslationView { get; set; }

        int Type { get; set; }
        Guid VisitorReference { get; }

        void ApplyPassword(string password);

        bool HasEmail(string email);

        bool HasUserName(string username);

        IApplicationRole Role();

        bool Save();

        Task<bool> SaveAsync();

        bool Delete();

        Task<bool> DeleteAsync();

        bool IsNewInstance { get; }
    }
}