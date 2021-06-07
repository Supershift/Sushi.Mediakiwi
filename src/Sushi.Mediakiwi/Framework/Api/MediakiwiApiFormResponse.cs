using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Sushi.Mediakiwi.Data.Data;

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

        /// <summary>
        /// The description for this list
        /// </summary>
        public string ListDescription { get; set; }

        /// <summary>
        /// The settings URL for this list
        /// </summary>
        public string ListSettingsUrl { get; set; }

        /// <summary>
        /// To which site (channel) does this response belong ?
        /// </summary>
        public int CurrentSiteID { get; set; }

        /// <summary>
        /// Is this field a shared field ?
        /// </summary>
        public bool IsSharedField { get; set; }

        /// <summary>
        /// What is the current value of this shared field
        /// </summary>
        public string SharedFieldValue { get; set; }

        /// <summary>
        /// Apply the data (if any) for shared fields in this API response
        /// </summary>
        public async Task ApplySharedFieldDataAsync()
        {
            try
            {
                var _fieldName = Fields?.FirstOrDefault(x => string.IsNullOrWhiteSpace(x.PropertyName) == false && x.PropertyName == "FieldName" && x.Value != null);

                if (_fieldName != null)
                {
                    var _sharedField = await Data.SharedField.FetchSingleAsync(_fieldName.Value.ToString().ToUpperInvariant());
                    if (_sharedField?.ID > 0)
                    {
                        IsSharedField = true;
                        var _sharedFieldValue = await Data.SharedFieldTranslation.FetchSingleForFieldAndSiteAsync(_sharedField.ID, CurrentSiteID);

                        if (_sharedFieldValue?.ID > 0)
                        {
                            SharedFieldValue = _sharedFieldValue.Value;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await Data.Notification.InsertOneAsync("Monitor.ApplySharedFieldInformation", Data.NotificationType.Error, ex.Message);
            }
        }
    }
}