using Sushi.Mediakiwi.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Interfaces
{
    public interface iPresentationMonitor
    {
        Task<string> GetTemplateWrapperAsync(Beta.GeneratedCms.Console container, Dictionary<GlobalPlaceholder, string> placeholders, Dictionary<CallbackTarget, List<ICallback>> callbacks, WimControlBuilder builder);
        Task<string> GetLoginWrapperAsync(Beta.GeneratedCms.Console container, Dictionary<GlobalPlaceholder, string> placeholders, Dictionary<CallbackTarget, List<ICallback>> callbacks);
    }
}
