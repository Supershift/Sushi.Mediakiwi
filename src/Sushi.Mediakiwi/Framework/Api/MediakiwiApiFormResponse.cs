using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Sushi.Mediakiwi.Framework.Api
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore, NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class MediakiwiApiFormResponse : MediakiwiApiResponse
    {
        public MediakiwiApiFormResponse()
        {
            Fields = new List<MediakiwiField>();
            Buttons = new List<MediakiwiField>();
            Notifications = new List<MediakiwiNotification>();
        }

        /// <summary>
        /// Declares if the fields should be rendered as labels or input 
        /// </summary>
        public bool IsEditMode { get; set; }

        /// <summary>
        /// Form fields
        /// </summary>
        public List<MediakiwiField> Fields { get; set; }
        
        /// <summary>
        /// Buttons
        /// </summary>
        public List<MediakiwiField> Buttons { get; set; }

        /// <summary>
        /// Buttons
        /// </summary>
        public List<MediakiwiNotification> Notifications { get; set; }

        /// <summary>
        /// If presented the page will load this page insead of the form
        /// </summary>
        public string RedirectUrl { get; set; }

        public string ListDescription { get; set; } 
        public string ListSettingsUrl { get; set; }
    }
}