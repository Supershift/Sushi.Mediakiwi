using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;
using Sushi.Mediakiwi.Framework;

namespace Sushi.Mediakiwi.Framework.Api
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore, NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class MediakiwiGrid
    {
        public List<MediakiwiGridColumn> Columns { get; set; }
        public List<MediakiwiGridRow> Rows { get; set; }
    }

    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore, NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class MediakiwiGridColumn
    {
        /// <summary>
        /// Title of the column
        /// </summary>
        public string Title { get; set; }
        public int Width { get; set; }
        //public Align Align { get; set; }
    }

    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore, NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class MediakiwiGridRow
    {
        public List<MediakiwiGridItem> GridItems { get; set; }
        public int RowID { get; internal set; }
    }

    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore, NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class MediakiwiGridItem
    {
        /// <summary>
        /// Zero-based column index
        /// </summary>
        public int Column { get; set; }

        public object Value { get; set; }

        public string Href { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public MediakiwiGridVueType VueType { get; set; }
    }
}