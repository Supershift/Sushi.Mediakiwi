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
                var _sharedFieldValues = await Data.SharedFieldTranslation.FetchAllForSiteAsync(CurrentSiteID).ConfigureAwait(false);

                if (Fields?.Count > 0 && _sharedFieldValues?.Count > 0)
                {
                    foreach (var field in Fields.Where(x => string.IsNullOrWhiteSpace(x.PropertyName) == false))
                    {
                        if (field.PropertyName == "FieldName")
                        {
                            var sharedFieldInstance = _sharedFieldValues.FirstOrDefault(x => x.FieldName.ToUpperInvariant() == field.Value.ToString().ToUpperInvariant());
                            if (sharedFieldInstance?.ID > 0)
                            {
                                IsSharedField = true;
                                SharedFieldValue = sharedFieldInstance.Value;
                            }
                        }
                    }
                }

                if (Fields.Any(x => x.PropertyName == nameof(IsSharedField) == false))
                {
                    Fields.Add(new MediakiwiField()
                    {
                        Event = MediakiwiJSEvent.change,
                        Expression = OutputExpression.Left,
                        PropertyName = nameof(IsSharedField),
                        PropertyType = typeof(bool).FullName,
                        VueType = MediakiwiFormVueType.wimChoiceCheckbox,
                        Title = "Is shared field",
                        Value = IsSharedField
                    });
                    Fields.Add(new MediakiwiField()
                    {
                        Event = MediakiwiJSEvent.none,
                        Expression = OutputExpression.Right,
                        PropertyName = nameof(SharedFieldValue),
                        PropertyType = typeof(string).FullName,
                        VueType = MediakiwiFormVueType.wimTextline,
                        Title = "Shared value",
                        Value = SharedFieldValue
                    });
                }
            }
            catch (Exception ex)
            {
                await Data.Notification.InsertOneAsync("Monitor.ApplySharedFieldInformation", Data.NotificationType.Error, ex.Message);
            }
        }
    }
}