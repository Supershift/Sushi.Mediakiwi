using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sushi.Mediakiwi.Framework.Api
{
    public enum MediakiwiFormVueType
    {
        undefined = 0,
        wimButton = 1,
        wimChoiceDropdown = 2,
        wimPlus = 3,
        wimRichText = 4,
        wimText = 5,
        wimTextline = 6,
        /// <summary>
        /// Tagging with the select2 library
        /// </summary>
        wimTag = 7,
        wimChoiceCheckbox = 8,
        /// <summary>
        /// Tagging with the http://vue-tags-input.com/#/ library
        /// </summary>
        wimTagVue = 9,
        wimTextArea = 10,
        wimChoiceRadio = 11,        
        wimDateTime = 12,
        wimDate = 13,
        wimSection = 14,
    }
}