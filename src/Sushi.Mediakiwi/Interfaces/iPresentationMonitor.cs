using Sushi.Mediakiwi.Framework;
using System.Collections.Generic;

namespace Sushi.Mediakiwi.Interfaces
{
    public interface iPresentationMonitor
    {
        string GetTemplateWrapper(Beta.GeneratedCms.Console container, Dictionary<GlobalPlaceholder, string> placeholders, Dictionary<CallbackTarget, List<ICallback>> callbacks, WimControlBuilder builder);
        string GetLoginWrapper(Beta.GeneratedCms.Console container, Dictionary<GlobalPlaceholder, string> placeholders, Dictionary<CallbackTarget, List<ICallback>> callbacks);
    }
}
