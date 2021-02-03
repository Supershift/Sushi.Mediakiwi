namespace Sushi.Mediakiwi.Data.Interfaces
{
    public interface ISearchView
    {
        string Description { get; set; }
        int FolderID { get; set; }
        string ID { get; set; }
        int ItemID { get; set; }
        int SiteID { get; set; }
        int SortOrder { get; set; }
        string Title { get; set; }
        string TitleHighlighted { get; }
        string Type { get; }
        int TypeID { get; set; }
    }
}