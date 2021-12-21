using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Interfaces
{
    public interface ITrailExtension
    {
        Task<string> RenderExtrasAsync(string propertyButton, Beta.GeneratedCms.Console console);
    }
}
