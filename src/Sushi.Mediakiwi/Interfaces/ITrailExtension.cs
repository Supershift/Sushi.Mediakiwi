using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Interfaces
{
    public interface ITrailExtension
    {
        Task<string> RenderWikiButtonsAsync(string propertyButton, Beta.GeneratedCms.Console console);
    }
}
