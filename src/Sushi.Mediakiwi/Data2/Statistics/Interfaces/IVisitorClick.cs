using System;

namespace Sushi.Mediakiwi.Data.Statistics
{
    public interface IVisitorClick
    {
        int? ApplicationUserID { get; set; }
        int? CampaignID { get; set; }
        DateTime Created { get; set; }
        CustomData Data { get; set; }
        int Entry { get; set; }
        int ID { get; set; }
        bool IsEntry { get; set; }
        int? ItemID { get; set; }
        int? PageID { get; set; }
        int? ProfileID { get; set; }
        string Query { get; set; }
        int? RenderTime { get; set; }
        int VisitorLogID { get; set; }

        VisitorLog Log();
        void Save();
    }
}