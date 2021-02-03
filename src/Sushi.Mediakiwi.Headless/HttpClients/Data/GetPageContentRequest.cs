using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Sushi.Mediakiwi.Headless.HttpClients.Data
{
    public class GetPageContentRequest
    {
        [JsonPropertyName("path")]
        public string Path { get; set; }

        [JsonPropertyName("clearCache")]
        public bool ClearCache { get; set; }

        [JsonPropertyName("isPreview")]
        public bool IsPreview { get; set; }
        
        [JsonPropertyName("pageId")]
        public int? PageID { get; set; }
    }
}
