using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Sushi.Mediakiwi.API.Transport
{
    public class ListGrid
    {
        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("layerConfiguration")]
        public LayerConfiguration LayerConfiguration { get; set; }

        [JsonPropertyName("columns")]
        public ICollection<ListColumn> Columns { get; set; } = new List<ListColumn>();

        [JsonPropertyName("rows")]
        public ICollection<ListRow> Rows { get; set; } = new List<ListRow>();

        [JsonPropertyName("pagination")]
        public ListPagination Pagination { get; set; }
    }
}
