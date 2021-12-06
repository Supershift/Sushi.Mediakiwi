using System.Text.Json.Serialization;

namespace Sushi.Mediakiwi.API.Transport
{
    public class ListColumn
    {
        /// <summary>
        /// The title of this column
        /// </summary>
        [JsonPropertyName("title")]
        public string Title { get; set; }

        /// <summary>
        /// The width of this column (in Pixels)
        /// </summary>
        [JsonPropertyName("width")]
        public int Width { get; set; }

        /// <summary>
        /// The alignment for the column (0 = Default, 1 = Left, 2 = Center, 3 = Right)
        /// </summary>
        [JsonPropertyName("align")]
        public int Align { get; set; }

        /// <summary>
        /// Is this a hidden column ?
        /// </summary>
        [JsonPropertyName("isHidden")]
        public bool IsHidden { get; set; }

        /// <summary>
        /// The column value Prefix
        /// </summary>
        [JsonPropertyName("prefix")]
        public string Prefix { get; set; }

        /// <summary>
        /// The column value Suffix
        /// </summary>
        [JsonPropertyName("suffix")]
        public string Suffix { get; set; }

        /// <summary>
        /// The helptext for this column
        /// </summary>
        [JsonPropertyName("helpText")]
        public string helpText { get; set; }

        /// <summary>
        /// Does this column represent a Sum ?
        /// </summary>
        [JsonPropertyName("isSum")]
        public bool IsSum { get; set; }

        /// <summary>
        /// Does this column represent an Average ?
        /// </summary>
        [JsonPropertyName("isAverage")]
        public bool IsAverage { get; set; }

    }
}
