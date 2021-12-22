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
        public List<Notification> Notifications { get; set; }

        [JsonPropertyName("grids")]
        public List<ListGrid> Grids { get; set; }
        
        [JsonPropertyName("forms")]
        public List<FormMap> FormMaps { get; set; }

        [JsonPropertyName("resources")]
        public List<ResourceItem> Resources { get; set; }
    }
}
