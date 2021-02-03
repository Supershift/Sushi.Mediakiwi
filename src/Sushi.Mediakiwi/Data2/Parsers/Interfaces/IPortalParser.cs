using System;
//using Wim.wimServerCommunication;

namespace Sushi.Mediakiwi.Data.Parsers
{
    public interface IPortalParser
    {
        void Delete(IPortal entity);
        void Clear();
        //WebInformationManagerServerService Connect(string domain);
        void Save(IPortal entity);
        IPortal[] SelectAll(int roleID);
        IPortal SelectOne(string authenticode);
        IPortal SelectOne(int ID);
        IPortal SelectOne(Guid guid);
    }
}