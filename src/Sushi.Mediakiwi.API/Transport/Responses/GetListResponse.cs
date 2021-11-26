using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Sushi.Mediakiwi.API.Transport.Responses
{
    public class GetListResponse
    {
        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("settingsUrl")]
        public string SettingsURL { get; set; }

        [JsonPropertyName("redirectUrl")]
        public string RedirectURL { get; set; }

        [JsonPropertyName("isEditMode")]
        public bool IsEditMode { get; set; }

        [JsonPropertyName("notifications")]
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();

        [JsonPropertyName("grids")]
        public ICollection<ListGrid> Grids { get; set; } = new List<ListGrid>();
        
        [JsonPropertyName("forms")]
        public ICollection<FormMap> FormMaps { get; set; } = new List<FormMap>();

        [JsonPropertyName("resources")]
        public ICollection<ResourceItem> Resources { get; set; } = new List<ResourceItem>();
    }
}
