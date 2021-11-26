using System.Text.Json.Serialization;

namespace Sushi.Mediakiwi.API.Transport
{
    public class ListPagination
    {
        [JsonPropertyName("itemsPerPage")]
        public int ItemsPerPage { get; set; }

        [JsonPropertyName("currentPage")]
        public int CurrentPage { get; set; }

        [JsonPropertyName("totalItems")]
        public int TotalItems { get; set; }
    }
}
