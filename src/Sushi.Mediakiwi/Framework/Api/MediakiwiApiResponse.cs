using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Sushi.Mediakiwi.Framework.Api
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore, NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class MediakiwiApiResponse
    {
        public string ListTitle { get; set; }
        public string ListDescription { get; set; }
        public string ListSettingsUrl { get; set; }
    }
}