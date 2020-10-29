using System;
using System.Web.UI.WebControls;

namespace Sushi.Mediakiwi.Data
{
    public interface ISubscription
    {
        void Delete();
        bool IsNewInstance { get; }
        int ComponentListID { get; set; }
        DateTime Created { get; set; }
        int ID { get; set; }
        //ListItemCollection IntervalCollection { get; }
        //string IntervalText { get; }
        int IntervalType { get; set; }
        bool IsActive { get; set; }
        //string IsActiveIcon { get; }
        DateTime Scheduled { get; set; }
        string SetupXml { get; set; }
        int SiteID { get; set; }
        string Title { get; }
        string Title2 { get; set; }
        string User { get; set; }
        int UserID { get; set; }

        void Save();
    }
}