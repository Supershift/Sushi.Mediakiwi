using System;

namespace Sushi.Mediakiwi.Data
{
    public interface IComponentTargetParser
    {
        void Delete(IComponentTarget entity);
        void DeleteCompetion(IComponentTarget entity);
        bool Save(IComponentTarget entity);
        IComponentTarget[] SelectAll(int pageID);
        IComponentTarget[] SelectAll(Guid componentGuid);
        IComponentTarget SelectOne(int ID);
    }
}