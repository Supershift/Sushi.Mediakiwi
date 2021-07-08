using System;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Data.Interfaces
{
    public interface ICountry
    {
        string Country_EN { get; set; }
        string Country_NL { get; set; }
        Guid GUID { get; set; }
        int ID { get; set; }
        bool IsActive { get; set; }

        void Save();

        Task SaveAsync();
    }
}