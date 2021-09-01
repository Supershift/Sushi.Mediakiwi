using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Headless
{
    public interface IScriptTagContentResolver
    {
        Task SetContentAsync(Data.PageContentResponse pageContent, string url, List<Data.TrackingScript.TrackingScriptSimple> scripts);
    }
}
