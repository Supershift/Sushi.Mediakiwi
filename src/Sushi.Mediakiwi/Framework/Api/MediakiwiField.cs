using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Sushi.Mediakiwi.Data;
using Sushi.Mediakiwi.Framework2.Api.Logic;
using Sushi.Mediakiwi.UI;
using System.Collections.Generic;

namespace Sushi.Mediakiwi.Framework.Api
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore, NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class MediakiwiField
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public ContentType ContentTypeID { get; set; }
        public string PropertyName { get; set; }
        public string PropertyType { get; set; }
        public string Title { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public MediakiwiFormVueType VueType { get; set; }
        public OutputExpression Expression { get; set; }
        public object Value { get; set; }
        [JsonConverter(typeof(ListItemCollectionConverter))]
        public ListItemCollection Options { get; set; }
        public string ClassName { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public MediakiwiJSEvent Event { get; set; }
        public string InputPost { get; set; }
        public ButtonSection Section { get; set; }
        public bool? Hidden { get; set; }
        public string GroupName { get; set; }   
        
        /// <summary>
        /// Determines the html text after the input field
        /// </summary>
        public string Suffix { get; set; }

        /// <summary>
        /// Determines the html text before the input field
        /// </summary>
        public string Prefix { get; set; }  
        
        /// <summary>
        /// Determines what section (or formmap) this field belongs to
        /// </summary>
        public string FormSection { get; set; }

        /// <summary>
        /// Determines if this section can be opened or closed via the UI. 
        /// Only applicable for wimSection
        /// </summary>
        public bool CanToggleSection { get; set; }

        /// <summary>
        /// Determines if this section can be deleted via the UI. 
        /// Only applicable for wimSection
        /// </summary>
        public bool CanDeleteSection { get; set; }

        /// <summary>
        /// Determines if this section is default hidden when rendering the UI. 
        /// Only applicable for wimSection
        /// </summary>
        public bool ToggleDefaultClosed { get; set; }

        /// <summary>
        /// Determines that this field is set to readonly
        /// </summary>
        public bool ReadOnly { get; set; }

        /// <summary>
        /// The helptext for this field
        /// </summary>
        public string HelpText { get; set; }

        /// <summary>
        /// Does this field trigger a postback
        /// </summary>
        public bool IsAutoPostback { get; set; }

        /// <summary>
        /// Is this field mandatory ?
        /// </summary>
        public bool IsMandatory { get; set; }

        /// <summary>
        /// Does this field have a maximum length ?
        /// </summary>
        public int? MaxLength { get; set; }

        /// <summary>
        /// Contains additional data, will not be JSON transmitted
        /// </summary>
        [JsonIgnore]
        public Dictionary<string, object> AdditionalData { get; set; } = new Dictionary<string, object>();
    }
}