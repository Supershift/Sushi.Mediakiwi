using Sushi.Mediakiwi.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sushi.Mediakiwi.Interfaces
{
    public interface iPresentationMonitor
    {
        string GetTemplateWrapper(Sushi.Mediakiwi.Beta.GeneratedCms.Console container, Dictionary<GlobalPlaceholder, string> placeholders, Dictionary<CallbackTarget, List<ICallback>> callbacks, Sushi.Mediakiwi.Framework.WimControlBuilder builder);
        string GetLoginWrapper(Sushi.Mediakiwi.Beta.GeneratedCms.Console container, Dictionary<GlobalPlaceholder, string> placeholders, Dictionary<CallbackTarget, List<ICallback>> callbacks);
    }
}
