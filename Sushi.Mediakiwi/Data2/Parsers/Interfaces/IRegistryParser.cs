namespace Sushi.Mediakiwi.Data.Parsers
{
    public interface IRegistryParser
    {
        void Save(IRegistry entity);
        IRegistry[] SelectAll();
        IRegistry SelectOne(int ID);
        IRegistry SelectOneByName(string name);
        void Delete(IRegistry entity);
    }
}