using System;
using System.Configuration;
using System.Collections.Generic;
using System.Text;

namespace Sushi.Mediakiwi.Framework
{
    /// <summary>
    /// 
    /// </summary>
    public class WimServerPortal : ConfigurationElement
    {
        /// <summary>
        /// Gets the ID.
        /// </summary>
        /// <value>The ID.</value>
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

        string m_Connection;
        /// <summary>
        /// Gets the connection.
        /// </summary>
        /// <value>The connection.</value>
        [ConfigurationProperty("connection", IsRequired = true)]
        public string Connection
        {
            get
            {
                if (string.IsNullOrEmpty(m_Connection))
                {
                    var connection =  this["connection"] as string;
                    if (this.IsReconfigured)
                        m_Connection = Wim.Utility.GetConfigurationSetting(connection, string.Concat("Portal.", this.Name));
                    else
                        m_Connection = connection;

                }
                return m_Connection;
            }
            set
            {
                m_Connection = value;
            }
        }

        string m_Name1;
        /// <summary>
        /// Gets or sets the name1.
        /// </summary>
        /// <value>The name1.</value>
        [ConfigurationProperty("name1", IsRequired = false)]
        public string Name1
        {
            get
            {
                if (string.IsNullOrEmpty(m_Name1))
                    m_Name1 = this["name1"] as string;
                return m_Name1;
            }
            set
            {
                m_Name1 = value;
            }
        }

        string m_Connection1;
        /// <summary>
        /// Gets the connection.
        /// </summary>
        /// <value>The connection.</value>
        [ConfigurationProperty("connection1", IsRequired = false)]
        public string Connection1
        {
            get
            {
                if (string.IsNullOrEmpty(m_Connection1))
                    m_Connection1 = this["connection1"] as string;
                return m_Connection1;
            }
            set
            {
                m_Connection1 = value;
            }
        }

        /// <summary>
        /// Gets the connection.
        /// </summary>
        /// <value>The connection.</value>
        [ConfigurationProperty("skin", IsRequired = false)]
        public string DefaultSkin
        {
            get
            {
                return this["skin"].ToString();
            }
        }

        /// <summary>
        /// Gets the type.
        /// </summary>
        /// <value>The type.</value>
        [ConfigurationProperty("type", IsRequired = false)]
        public Sushi.Mediakiwi.Data.DataConnectionType Type
        {
            get
            {
                if (this["type"] == null)
                    return Sushi.Mediakiwi.Data.DataConnectionType.SqlServer;
                return (Sushi.Mediakiwi.Data.DataConnectionType)this["type"];
            }
        }
    }
}
