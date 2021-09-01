using Sushi.Mediakiwi.Headless.Config;
using System.Collections.Generic;

namespace Sushi.Mediakiwi.Website.Models
{
    public class ApplicationSettings : ISushiApplicationSettings
    {
        public MediaKiwiConfig MediaKiwi { get; set; } = new MediaKiwiConfig();
        public bool EnableResponseCaching { get; set; }
        public bool EnableStaticFileCaching { get; set; }
        public List<string> BotUserAgentParts { get; set; } = new List<string>();
    }
}