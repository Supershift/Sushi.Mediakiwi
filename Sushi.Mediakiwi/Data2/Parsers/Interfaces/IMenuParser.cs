namespace Sushi.Mediakiwi.Data.Parsers
{
    public interface IMenuParser
    {
        void Delete(IMenu entity);
        bool Save(IMenu entity);
        IMenu[] SelectAll();
        IMenu SelectOne(int ID);
    }
}