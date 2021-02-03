namespace Sushi.Mediakiwi.Data.Parsers
{
    public interface IDataFilterParser
    {
        void Clear();
        bool DeleteAll(int propertyID);
        void Save(IDataFilter entity);
        void Delete(IDataFilter entity);
        IDataFilter SelectOne(int ID);
        IDataFilter SelectOne(int propertyID, int itemID);
    }
}