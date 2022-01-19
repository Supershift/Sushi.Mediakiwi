using System;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Data.Interfaces
{
    public interface ISite
    {
        bool AutoPublishInherited { get; set; }
        ICountry Country { get; }
        int? CountryID { get; set; }
        DateTime Created { get; set; }
        string Culture { get; set; }
        string CultureName { get; }
        string DefaultFolder { get; set; }
        string DefaultPageTitle { get; set; }
        string Domain { get; set; }
        string[] Domains { get; }
        int? ErrorPageID { get; set; }
        Guid GUID { get; set; }
        bool HasLists { get; set; }
        bool HasPages { get; set; }
        int? HomepageID { get; set; }
        int ID { get; set; }
        bool IsActive { get; set; }
        string Language { get; set; }
        string Master { get; }
        int? MasterID { get; set; }
        Site MasterImplement { get; set; }
        string Name { get; set; }
        int? PageNotFoundID { get; set; }
        TimeZoneInfo TimeZone { get; }
        string TimeZoneIndex { get; set; }
        int? Type { get; set; }

        void Delete();

        Task DeleteAsync();

        bool IsDomain(Uri url);

        void Save();

        Task SaveAsync();
    }
}