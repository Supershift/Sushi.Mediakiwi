using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Sushi.Mediakiwi.API.Transport
{
    public abstract class AuthenticatedRequest
    {
        [JsonPropertyName("currentSiteId")]
        [Required]
        public int CurrentSiteID { get; set; }
    }
}
