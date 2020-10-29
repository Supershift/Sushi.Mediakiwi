namespace Sushi.Mediakiwi.Data.Parsers
{
    public interface IComponentTargetPageParser
    {
        IComponentTargetPage[] SelectAll(int templateID, int siteID);
    }
}