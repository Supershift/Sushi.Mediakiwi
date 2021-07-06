using Newtonsoft.Json;

namespace Sushi.Mediakiwi.Controllers.Data
{
    public class GetFieldsRequest
    {
        [JsonProperty("documentTypeID")]
        [System.Text.Json.Serialization.JsonPropertyName("documentTypeID")]
        public int DocumentTypeID { get; set; }
    }
}
