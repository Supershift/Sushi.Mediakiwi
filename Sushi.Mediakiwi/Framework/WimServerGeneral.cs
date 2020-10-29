using System;
using System.Configuration;
using System.Collections.Generic;
using System.Text;

namespace Sushi.Mediakiwi.Framework
{
    /// <summary>
    /// 
    /// </summary>
    public class WimServerGeneral : ConfigurationElement
    {
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        [ConfigurationProperty("name", IsRequired = true)]
        public string Name
        {
            get
                {
                return this["name"] as string;
            }
        }

        /// <summary>
        /// This bool set the option for the system to look for a alternative configuration setting (dll) which is defined by CLOUD_SETTINGS
        /// </summary>
        [ConfigurationProperty("reconfigure", IsRequired = false)]
        public bool IsReconfigured
        {
            get
            {
                if (this["reconfigure"] == null || this["reconfigure"].ToString().ToLower() != "true")
                    return false;
                return true;
            }
        }

        bool m_IsValueSet;
        string m_Value;
        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>The value.</value>
        [ConfigurationProperty("value", IsRequired = true)]
        public string Value
        {
            get
            {
                if (string.IsNullOrEmpty(m_Value))
                {
                    var value = this["value"] as string;
                    if (this.IsReconfigured)
                        m_Value = Wim.Utility.GetConfigurationSetting(value, string.Concat("General.", this.Name));
                    else
                        m_Value = value;
                }
                if (!m_IsValueSet)
                    m_IsValueSet = true;

                return m_Value;
            }
        }
    }
}
