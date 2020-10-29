namespace Sushi.Mediakiwi.Data.Parsers
{
    public interface ISubscriptionParser
    {
        void Clear();
        void Save(ISubscription entity);
        ISubscription[] SelectAll();
        ISubscription[] SelectAll(int listID, int userID);
        ISubscription[] SelectAllActive();
        ISubscription SelectOne(int ID);
        void Delete(ISubscription entity);
    }
}