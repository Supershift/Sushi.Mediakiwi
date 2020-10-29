using System;
using System.Web.UI.WebControls;

namespace Sushi.Mediakiwi.Data
{
    public interface IPageMapping
    {
        int AssetID { get; set; }
        DateTime Created { get; set; }
        string EditLink { get; }
        int ID { get; set; }
        bool IsActive { get; set; }
        bool IsInternalDoc { get; set; }
        bool IsInternalLink { get; set; }
        bool IsNewInstance { get; }
        int? ItemID { get; set; }
        int? ListID { get; set; }
        string NavigateURL { get; }
        Page Page { get; }
        int PageID { get; set; }
        string PageName { get; set; }
        string Path { get; set; }
        string Query { get; set; }
        string RedirectTo { get; }
        int TargetType { get; set; }
        ListItemCollection TargetTypes { get; }
        string TestLink { get; }
        string Title { get; set; }
        string Type { get; }
        int TypeID { get; set; }

        void Delete();
        bool Save();
    }
}