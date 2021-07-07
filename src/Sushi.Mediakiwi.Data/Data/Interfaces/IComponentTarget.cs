using System;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Data
{
    public interface IComponentTarget
    {
        int ID { get; set; }
        bool IsNewInstance { get; }
        int PageID { get; set; }
        Guid Source { get; set; }
        Guid Target { get; set; }

        void Delete();

        Task DeleteAsync();

        void DeleteComplete();

        Task DeleteCompleteAsync();

        bool Save();

        Task<bool> SaveAsync();
    }
}