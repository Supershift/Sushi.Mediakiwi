using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Data.Caching
{
    public interface ICacheManager
    {
        T Get<T>(string region, string key);
        void Add<T>(string region, string key, T item);
        void FlushRegion(string region);
        Task ValidateAsync(System.DateTime environmentLastUpdate);
        Task FlushAsync();
    }
}
