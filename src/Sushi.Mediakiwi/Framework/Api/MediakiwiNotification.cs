﻿using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Sushi.Mediakiwi.Framework.Api
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore, NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class MediakiwiNotification
    {
        public string Message { get; set; }
        public bool IsError { get; set; }
        /// <summary>
        ///  Corresponds to the MediakiwiField.PropertyName name
        /// </summary>
        public string PropertyName { get; set; }
    }
}