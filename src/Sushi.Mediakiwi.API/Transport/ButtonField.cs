using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Sushi.Mediakiwi.API.Transport
{
    public class ButtonField : ContentField
    {
        [JsonPropertyName("askConfirmation")]
        public bool AskConfirmation { get; set; }

        [JsonPropertyName("confirmationQuestion")]
        public string ConfirmationQuestion { get; set; }

        [JsonPropertyName("confirmationTitle")]
        public string ConfirmationTitle { get; set; }

        [JsonPropertyName("confirmationRejectLabel")]
        public string ConfirmationRejectLabel { get; set; }

        [JsonPropertyName("confirmationAcceptLabel")]
        public string ConfirmationAcceptLabel { get; set; }

        [JsonPropertyName("target")]
        public string Target { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("triggerSaveEvent")]
        public bool TriggerSaveEvent { get; set; }

        [JsonPropertyName("isPrimary")]
        public bool IsPrimary { get; set; }
    }
}
