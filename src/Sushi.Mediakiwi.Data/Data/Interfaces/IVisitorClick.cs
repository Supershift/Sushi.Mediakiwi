using System;

namespace Sushi.Mediakiwi.Data
{
    public interface IVisitorClick
    {
        int? ApplicationUserID { get; set; }
        int? CampaignID { get; set; }
        DateTime Created { get; set; }
        CustomData Data { get; set; }
        string DataString { get; set; }
        int ID { get; set; }
        bool IsEntry { get; set; }
        int? ItemID { get; set; }
        int? PageID { get; set; }
        int? ProfileID { get; set; }
        string Query { get; set; }
        int? RenderTime { get; set; }
        int VisitorLogID { get; set; }
    }
}