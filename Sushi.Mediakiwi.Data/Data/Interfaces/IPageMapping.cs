using System;

namespace Sushi.Mediakiwi.Data
{
    public interface IPageMapping
    {
        int AssetID { get; set; }
        DateTime Created { get; set; }
        int ID { get; set; }
        bool IsActive { get; set; }
        bool IsInternalDoc { get; set; }
        bool IsInternalLink { get; set; }
        bool IsNewInstance { get; }
        int? ItemID { get; set; }
        int? ListID { get; set; }
        Page Page { get; }
        int PageID { get; set; }
        string Path { get; set; }
        string Query { get; set; }
        int TargetType { get; set; }
        string Title { get; set; }
        int TypeID { get; set; }

        void Delete();

        bool Save();

        IPageMapping SelectOne(int? listID, int itemID);

        System.Threading.Tasks.Task<IPageMapping> SelectOneAsync(int? listID, int itemID);
    }
}