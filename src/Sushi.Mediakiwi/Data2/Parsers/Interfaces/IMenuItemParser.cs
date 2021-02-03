namespace Sushi.Mediakiwi.Data.Parsers
{
    public interface IMenuItemParser
    {
        void Delete(IMenuItem entity);
        bool Save(IMenuItem entity);
        IMenuItem[] SelectAll(int menuID);
        IMenuItem[] SelectAll_Dashboard(int dashboardID);
    }
}