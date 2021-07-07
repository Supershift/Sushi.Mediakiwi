using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Sushi.Mediakiwi.Framework.Api
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore, NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class MediakiwiApiGridResponse : MediakiwiApiResponse
    {
        public MediakiwiApiGridResponse()
        {
            this.Grid = new MediakiwiGrid();
        }
        /// <summary>
        /// Grid
        /// </summary>
        public MediakiwiGrid Grid { get; set; }
    }
}