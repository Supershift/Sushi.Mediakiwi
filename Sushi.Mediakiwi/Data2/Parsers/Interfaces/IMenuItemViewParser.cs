namespace Sushi.Mediakiwi.Data
{
    public interface IMenuItemViewParser
    {
        IMenuItemView[] SelectAll(int siteID, int roleID);
        IMenuItemView[] SelectAll(int siteID, int roleID, params int[] items);
        string Url(IMenuItemView entity, int currentChannelID);
        IMenuItemView[] SelectAll_Dashboard(int dashboardID);
    }
}