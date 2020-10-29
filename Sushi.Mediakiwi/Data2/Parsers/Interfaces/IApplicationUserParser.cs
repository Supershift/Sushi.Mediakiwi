using System;

namespace Sushi.Mediakiwi.Data.Parsers
{
    public interface IApplicationUserParser
    {
        void Clear();
        IApplicationUser SelectOne(int ID);
        IApplicationUser SelectOne(string username);
        IApplicationUser SelectOne(Guid applicationUserGUID);
        IApplicationUser SelectOne(string email, int? ignoreApplicationUserID);
        IApplicationUser SelectOneByUserName(string username, int? ignoreApplicationUserID);
        IApplicationUser SelectOne(string username, string password);
        IApplicationUser SelectOneByEmail(string emailaddress);
        IApplicationUser[] SelectAll();
        IApplicationUser[] SelectAll(string username, int role);
        IApplicationUser[] SelectAll(int? role);
        IApplicationUser[] SelectAll(int? role, bool onlyReturnActive);
        bool Save(IApplicationUser entity);
        bool Delete(IApplicationUser entity);
    }
}