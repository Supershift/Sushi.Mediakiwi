using System;

namespace Sushi.Mediakiwi.Data.Identity
{
    public interface IProfile
    {
        DateTime Created { get; set; }
        CustomData Data { get; set; }
        string Email { get; set; }
        Guid GUID { get; set; }
        int ID { get; set; }
        string Password { get; set; }
        Guid ProfileReference { get; }
        bool RememberMe { get; set; }

        bool Logout(int? redirectionPageID = default(int?));
        bool Save();
        bool Save(bool shouldRememberVisitorForNextVisit, bool? isLoggedIn = default(bool?));
        bool IsNewInstance { get; }
    }
}