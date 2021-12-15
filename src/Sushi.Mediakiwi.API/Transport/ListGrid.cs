using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Sushi.Mediakiwi.API.Transport
{
    public class ListGrid
    {
        /// <summary>
        /// The title for this grid
        /// </summary>
        [JsonPropertyName("title")]
        public string Title { get; set; }

        /// <summary>
        /// The layer configuration for this grid (if any)
        /// </summary>
        [JsonPropertyName("layerConfiguration")]
        public LayerConfiguration LayerConfiguration { get; set; }

        /// <summary>
        /// All the Columns within this grid
        /// </summary>
        [JsonPropertyName("columns")]
        public ICollection<ListColumn> Columns { get; set; } = new List<ListColumn>();

        /// <summary>
        /// All the Rows within this grid
        /// </summary>
        [JsonPropertyName("rows")]
        public ICollection<ListRow> Rows { get; set; } = new List<ListRow>();

        /// <summary>
        /// The pagination, keeping current page and total results
        /// </summary>
        [JsonPropertyName("pagination")]
        public ListPagination Pagination { get; set; }

        /// <summary>
        /// The buttons belonging to this grid
        /// </summary>
        [JsonPropertyName("buttons")]
        public ICollection<ButtonField> Buttons { get; set; } = new List<ButtonField>();
    }
}
