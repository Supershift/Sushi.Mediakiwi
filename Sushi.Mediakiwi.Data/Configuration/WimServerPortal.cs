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
    public class WimServerPortal 
    {
        /// <summary>
        /// Gets the ID.
        /// </summary>
        /// <value>The ID.</value>
        [DataMember(Name = "name")]
        public string Name { get; set; }

        /// <summary>
        /// This bool set the option for the system to look for a alternative configuration setting (dll) which is defined by CLOUD_SETTINGS
        /// </summary>
        [DataMember(Name = "reconfigure")]
        public bool IsReconfigured { get; set; }

        string m_Connection;
        /// <summary>
        /// Gets the connection.
        /// </summary>
        /// <value>The connection.</value>
        [DataMember(Name = "connection")]
        public string Connection
        {
            get
            {
                if (string.IsNullOrEmpty(m_Connection))
                {
                    var connection = string.Empty;
                    if (this.IsReconfigured)
                        m_Connection = Utility.GetConfigurationSetting(connection, string.Concat(Name = "Portal.", this.Name));
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

        /// <summary>
        /// Gets the connection.
        /// </summary>
        /// <value>The connection.</value>
        [DataMember(Name = "skin")]
        public string DefaultSkin { get; set; }

        /// <summary>
        /// Gets the type.
        /// </summary>
        /// <value>The type.</value>
        //[DataMember(Name = "type")]
                
        //public Wim.Data.DataConnectionType Type     //[MR: 24-01-2020] REVIEW BY MARC
        //{
        //    get
        //    {
        //        if (this["type"] == null)
        //            return Wim.Data.DataConnectionType.SqlServer;
        //        return (Wim.Data.DataConnectionType)this["type"];
        //    }
        //}
    }
}
