using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Sushi.Mediakiwi.API.Transport
{
    public class ButtonField : ContentField
    {
        /// <summary>
        /// Do we need to ask confirmation before triggering this button ?
        /// </summary>
        [JsonPropertyName("askConfirmation")]
        public bool AskConfirmation { get; set; }

        /// <summary>
        /// The question to ask for confirmation
        /// </summary>
        [JsonPropertyName("confirmationQuestion")]
        public string ConfirmationQuestion { get; set; }

        /// <summary>
        /// The title for the confirmation box
        /// </summary>
        [JsonPropertyName("confirmationTitle")]
        public string ConfirmationTitle { get; set; }

        /// <summary>
        /// The Label for rejecting (NO)
        /// </summary>
        [JsonPropertyName("confirmationRejectLabel")]
        public string ConfirmationRejectLabel { get; set; }

        /// <summary>
        /// The Label for accepting (YES)
        /// </summary>
        [JsonPropertyName("confirmationAcceptLabel")]
        public string ConfirmationAcceptLabel { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonPropertyName("target")]
        public string Target { get; set; }

        /// <summary>
        /// The URL to which this button is linked. 
        /// In case of an action button this is probably '#' or empty
        /// </summary>
        [JsonPropertyName("url")]
        public string Url { get; set; }

        /// <summary>
        /// Does this button trigger a Save event ?
        /// </summary>
        [JsonPropertyName("triggerSaveEvent")]
        public bool TriggerSaveEvent { get; set; }

        /// <summary>
        /// Is this button a primary button ?
        /// </summary>
        [JsonPropertyName("isPrimary")]
        public bool IsPrimary { get; set; }
    }
}
