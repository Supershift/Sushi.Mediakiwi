using System;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Data
{
    public interface IDataFilter
    {
        bool? FilterB { get; set; }
        string FilterC { get; set; }
        decimal? FilterD { get; set; }
        int? FilterI { get; set; }
        DateTime? FilterT { get; set; }
        int ID { get; set; }
        bool IsNewInstance { get; }
        int ItemID { get; set; }
        int PropertyID { get; set; }
        string Type { get; set; }

        void Delete();

        Task DeleteAsync();

        void Save();

        Task SaveAsync();
    }
}