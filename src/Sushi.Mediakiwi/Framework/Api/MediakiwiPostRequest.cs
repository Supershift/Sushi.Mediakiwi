using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Sushi.Mediakiwi.Framework.Api
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore, NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class MediakiwiPostRequest
    {
        /// <summary>
        /// Who initiated the post
        /// </summary>
        public string Referrer { get; set; }
        /// <summary>
        /// Name/Value collection of the properties and their values
        /// </summary>
        public object FormFields { get; set; }
    }
}