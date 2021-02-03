namespace Sushi.Mediakiwi.Data.Parsers
{
    public interface IPageMappingParser
    {
        string ConvertUrl(string url);
        void Delete(IPageMapping entity);
        IPageMapping RegisterUrl(string url, string query, string pageTitle, int pageID, bool applyApplicationPath, int? listID, int? itemID);
        bool Save(IPageMapping entity);
        IPageMapping[] SelectAll();
        IPageMapping[] SelectAllBasedOnPageID(int pageID);
        IPageMapping[] SelectAllBasedOnPathPrefix(string prefix);
        IPageMapping[] SelectAllBasedOnPathPrefix(string prefix, int pageID);
        IPageMapping[] SelectAllNonList(int typeId, bool onlyActive);
        IPageMapping SelectOne(string relativePath);
        IPageMapping SelectOne(int ID);
        IPageMapping SelectOne(int? listID, int itemID);
        IPageMapping SelectOne(int? listID, int itemID, int pageID);
        IPageMapping SelectOneByPageAndQuery(int pageID, string query);
    }
}