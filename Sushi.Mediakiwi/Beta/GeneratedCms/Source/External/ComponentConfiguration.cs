using System;
using System.IO;
using System.Collections;
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
    public class ComponentConfiguration
    {
        /// <summary>
        /// Loads the specified configuration file.
        /// </summary>
        /// <returns></returns>
        public static Hashtable Load()
        {
            Hashtable ht = new Hashtable();

            string configurationFolder = string.Concat(Wim.CommonConfiguration.LocalConfigurationFolder, "/wim/components/");
            foreach (string file in Directory.GetFiles(configurationFolder, "*.config"))
            {
                string xml = System.IO.File.ReadAllText(file, System.Text.Encoding.UTF8);
                ComponentConfiguration cc = (ComponentConfiguration)Wim.Utility.GetDeserialized(typeof(ComponentConfiguration), xml);
                cc.Data = cc.Data.Replace("[APP]", System.Web.HttpContext.Current.Request.ApplicationPath == "/" ? null : System.Web.HttpContext.Current.Request.ApplicationPath);

                System.IO.FileInfo nfo = new System.IO.FileInfo(file);

                ht[nfo.Name.Replace(".config", string.Empty)] = cc;
            }
            return ht;
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

        string m_Processing;
        /// <summary>
        /// Gets or sets the processing.
        /// </summary>
        /// <value>The processing.</value>
        [XmlElement("Processing")]
        public string Processing
        {
            get { return m_Processing; }
            set { m_Processing = value; }
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
        string m_DataInstance;
        /// <summary>
        /// Gets or sets the data instance.
        /// </summary>
        /// <value>The data instance.</value>
        [XmlElement("DataInstance")]
        public string DataInstance
        {
            get { return m_DataInstance; }
            set { m_DataInstance = value; }
        }
    }
}
