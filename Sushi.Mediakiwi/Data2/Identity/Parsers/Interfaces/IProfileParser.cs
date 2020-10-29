using System;

namespace Sushi.Mediakiwi.Data.Identity
{
    public interface IProfileParser
    {
        IProfile Login(string email, string password);
        bool Logout(int? redirectionPageID = default(int?));
        bool Save(IProfile entity);
        bool Save(IProfile entity, bool shouldRememberVisitorForNextVisit, bool? isLoggedIn = default(bool?));
        IProfile Select();
        IProfile[] SelectAll();
        IProfile SelectOne(string email);
        IProfile SelectOne(int ID);
        IProfile SelectOne(Guid guid);
    }
}