using System.Text.Json.Serialization;

namespace Sushi.Mediakiwi.API.Transport.Responses
{
    public class GetContentResponse : BasicResponse
    {
        [JsonPropertyName("type")]
        public ContentResponseTypeEnum Type { get; set; }

        [JsonPropertyName("list")]
        public GetListResponse List { get; set; }

        [JsonPropertyName("page")]
        public GetPageResponse Page { get; set; }

        [JsonPropertyName("explorer")]
        public GetExplorerResponse Explorer { get; set; }

        [JsonPropertyName("breadCrumbs")]
        public GetBreadCrumbsResponse BreadCrumbs { get; set; }

        [JsonPropertyName("isEditMode")]
        public bool IsEditMode { get; set; }
    }
}
