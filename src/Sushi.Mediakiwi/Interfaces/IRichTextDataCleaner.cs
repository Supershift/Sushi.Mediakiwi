namespace Sushi.Mediakiwi
{
    public interface IRichTextDataCleaner
    {
        string ParseData(RequestItemType type, int? item, string id, string data);
    }
}
