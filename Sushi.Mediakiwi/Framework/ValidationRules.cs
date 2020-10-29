using System;
using System.Web;
using System.Data;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Sushi.Mediakiwi.Data.DalReflection;

namespace Sushi.Mediakiwi.Framework
{
    public class ValidationRules
    {
        //static FormValidationRules m_FormValidationRules;

        /// <summary>
        /// Loads the specified configuration file.
        /// </summary>
        /// <returns></returns>
        public static ValidationRules Load()
        {
            ValidationRules formValidationRules;
            string cachekey = string.Concat("wim_validation_rules");
            using (Wim.Utilities.CacheItemManager cman = new Wim.Utilities.CacheItemManager())
            {
                if (cman.IsCached(cachekey))
                {
                    formValidationRules = cman.GetItem(cachekey) as ValidationRules;
                }
                else
                {
                    string configurationFile = string.Concat(Wim.CommonConfiguration.LocalConfigurationFolder, "\\formvalidation.config");

                    if (System.IO.File.Exists(configurationFile))
                    {
                        string xml = System.IO.File.ReadAllText(configurationFile, System.Text.Encoding.UTF8);

                        //  Cache file dependency
                        formValidationRules = (ValidationRules)Wim.Utility.GetDeserialized(typeof(ValidationRules), xml);
                    }
                    else
                    {
                        formValidationRules = new ValidationRules();
                        formValidationRules.Rules = new ValidationRule[0];
                    }
                }
            }
            return formValidationRules;
        }

        ValidationRule[] m_Rules;
        [XmlArray("Rules")]
        public ValidationRule[] Rules
        {
            get { return m_Rules; }
            set { m_Rules = value; }
        }
    }
}
