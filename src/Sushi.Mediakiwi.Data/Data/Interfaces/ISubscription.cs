using System;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Data.Interfaces
{
    public interface ISubscription
    {
        int ComponentListID { get; set; }
        DateTime Created { get; set; }
        int ID { get; set; }
        int IntervalType { get; set; }
        bool IsActive { get; set; }
        bool IsNewInstance { get; }
        DateTime Scheduled { get; set; }
        string SetupXml { get; set; }
        int SiteID { get; set; }
        string Title { get; }
        string Title2 { get; set; }
        string User { get; }
        int UserID { get; set; }        

        void Delete();

        Task DeleteAsync();

        void Save();

        Task SaveAsync();
    }
}