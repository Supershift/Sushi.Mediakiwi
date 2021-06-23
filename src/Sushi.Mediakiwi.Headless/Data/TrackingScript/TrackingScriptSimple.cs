using System;
using System.Text.Json.Serialization;

namespace Sushi.Mediakiwi.Headless.Data.TrackingScript
{
    public class TrackingScriptSimple
    {
        [JsonPropertyName("id")]
        public int ID { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("code")]
        public string Code { get; set; }

        [JsonPropertyName("positionID")]
        public int PositionID { get; set; }

        [JsonIgnore]
        public TagPosition Position
        {
            get
            {
                TagPosition pos = TagPosition.BodyAfterCloseTag;
                Enum.TryParse(PositionID.ToString(), out pos);
                return pos;
            }
        }

        [JsonPropertyName("onlyShowIfAllParamsAreFilled")]
        public bool OnlyShowIfAllParamsAreFilled { get; set; }

        [JsonPropertyName("oncePerSession")]
        public bool OncePerSession { get; set; }

        [JsonPropertyName("isActive")]
        public bool IsActive { get; set; }
    }
}