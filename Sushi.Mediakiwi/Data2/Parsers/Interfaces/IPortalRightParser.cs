namespace Sushi.Mediakiwi.Data.Parsers
{
    public interface IPortalRightParser
    {
        IPortalRight[] SelectAll(int roleID);
        IPortalRight SelectOne(int ID);
    }
}