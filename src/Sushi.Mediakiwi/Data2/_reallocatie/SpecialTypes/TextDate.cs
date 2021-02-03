using System;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Sushi.Mediakiwi.Data.SpecialTypes
{
    /// <summary>
    /// Represents a TextDate entity.
    /// </summary>
    [XmlRoot("item")]
    public class TextDate
    {
        DateTime m_Date;
        /// <summary>
        /// Gets or sets the date.
        /// </summary>
        /// <value>The date.</value>
        [XmlElement("date")]
        public DateTime Date
        {
            get { return m_Date; }
            set { m_Date = value; }
        }

        bool m_Disabled;
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="TextDate"/> is disabled.
        /// </summary>
        /// <value><c>true</c> if disabled; otherwise, <c>false</c>.</value>
        [XmlElement("disabled")]
        public bool Disabled
        {
            get { return m_Disabled; }
            set { m_Disabled = value; }
        }

        string m_Text;
        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
        [XmlElement("text")]
        public string Text
        {
            get { return m_Text; }
            set { m_Text = value; }
        }
    }
}
