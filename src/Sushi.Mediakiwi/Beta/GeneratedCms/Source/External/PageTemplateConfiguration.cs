using System;
using System.Web;
using System.Data;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Sushi.Mediakiwi.Data.DalReflection;

namespace Sushi.Mediakiwi.Beta.GeneratedCms.Source.External
{
    /// <summary>
    /// 
    /// </summary>
    public class PageTemplateConfiguration
    {
        static PageTemplateConfiguration m_Configuration;

        /// <summary>
        /// Loads the specified configuration file.
        /// </summary>
        /// <returns></returns>
        public static PageTemplateConfiguration Load()
        {
            m_Configuration = null;
            if (m_Configuration == null)
            {
                string configurationFile = string.Concat(Wim.CommonConfiguration.LocalConfigurationFolder, "/wim/pagetemplates/default.config");
                string xml = System.IO.File.ReadAllText(configurationFile, System.Text.Encoding.UTF8);

                //  Cache file dependency
                m_Configuration = (PageTemplateConfiguration)Wim.Utility.GetDeserialized(typeof(PageTemplateConfiguration), xml);
                m_Configuration.Data = m_Configuration.Data.Replace("[APP]", System.Web.HttpContext.Current.Request.ApplicationPath == "/" ? null : System.Web.HttpContext.Current.Request.ApplicationPath);
            }
            return m_Configuration;
        }

        string m_Title;
        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>The title.</value>
        [XmlElement("Title")]
        public string Title
        {
            get { return m_Title; }
            set { m_Title = value; }
        }

        string m_Data;
        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        /// <value>The data.</value>
        [XmlElement("Data")]
        public string Data
        {
            get { return m_Data; }
            set { m_Data = value; }
        }
    }

}
