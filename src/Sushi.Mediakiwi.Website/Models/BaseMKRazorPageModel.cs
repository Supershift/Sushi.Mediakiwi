using Sushi.Mediakiwi.Headless;
using System;

namespace Sushi.Mediakiwi.Website.Models
{
    public class BaseMKRazorPageModel : BaseRazorPageModel<ApplicationSettings>
    {
        public BaseMKRazorPageModel(IMediaKiwiContentService service) : base(service) 
        {
            OnContentSet += BaseFTIRazorPageModel_OnContentSet;
        }

        private void BaseFTIRazorPageModel_OnContentSet(object sender, EventArgs e)
        {
            // Check if we can detect a bot
            if (Request?.Headers.ContainsKey("User-Agent") == true)
            {
                string agent = Request.Headers["User-Agent"];
                PageContent.InternalInfo.CustomData["useragent"] = agent;
                if (IsBot(agent))
                    PageContent.InternalInfo.CustomData["isbot"] = true;
            }
        }

        private bool IsBot(string userAgent)
        {
            bool retValue = false;

            if (!string.IsNullOrWhiteSpace(userAgent) && Configuration?.BotUserAgentParts?.Count > 0)
            {
                foreach (var item in Configuration.BotUserAgentParts)
                {
                    if (userAgent.Contains(item, StringComparison.OrdinalIgnoreCase))
                    {
                        retValue = true;
                        break;
                    }
                }
            }

            return retValue;
        }
    }
}
