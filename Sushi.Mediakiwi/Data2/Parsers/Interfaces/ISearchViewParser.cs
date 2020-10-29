namespace Sushi.Mediakiwi.Data.Parsers
{
    public interface ISearchViewParser
    {
        ISearchView[] SelectAll(string[] items);
        ISearchView[] SelectAll(int folderID);
        ISearchView[] SelectAll(int? siteID, int? filterType, string search);
    }
}