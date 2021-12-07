using System.Text.Json.Serialization;

namespace Sushi.Mediakiwi.API.Transport
{
    public class ListPagination
    {
        /// <summary>
        /// How many items per page are shown
        /// </summary>
        [JsonPropertyName("itemsPerPage")]
        public int ItemsPerPage { get; set; }

        /// <summary>
        /// In which pageset are we currently
        /// </summary>
        [JsonPropertyName("currentPage")]
        public int CurrentPage { get; set; }

        /// <summary>
        /// The total amount of items available
        /// </summary>
        [JsonPropertyName("totalItems")]
        public int TotalItems { get; set; }
    }
}
