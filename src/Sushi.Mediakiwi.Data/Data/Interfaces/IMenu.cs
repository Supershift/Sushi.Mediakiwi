using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Data
{
    public interface IMenu
    {
        int ID { get; set; }
        bool IsActive { get; set; }
        string Name { get; set; }
        int? RoleID { get; set; }
        int? SiteID { get; set; }

        void Save();

        Task SaveAsync();

        void Delete();

        Task DeleteAsync();
    }
}