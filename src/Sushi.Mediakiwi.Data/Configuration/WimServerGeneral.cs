using System;
using System.Configuration;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Sushi.Mediakiwi.Data.Configuration
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract]
    public class WimServerGeneral 
    {
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        [DataMember(Name = "name")]
        public string Name { get; set; }

        /// <summary>
        /// This bool set the option for the system to look for a alternative configuration setting (dll) which is defined by CLOUD_SETTINGS
        /// </summary>
        [DataMember(Name = "reconfigure")]
        public bool IsReconfigured { get; set; }

        bool m_IsValueSet;
        string m_Value;
        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>The value.</value>
        [DataMember(Name = "value")]
        public string Value
        {
            get
            {
                if (string.IsNullOrEmpty(m_Value))
                {
                    var value = string.Empty;
                    if (this.IsReconfigured)
                        m_Value = Utility.GetConfigurationSetting(value, string.Concat("General.", this.Name));
                    else
                        m_Value = value;
                }
                if (!m_IsValueSet)
                    m_IsValueSet = true;

                return m_Value;
            }
            set 
            {
                m_Value = value;
            }
        }
    }
}
