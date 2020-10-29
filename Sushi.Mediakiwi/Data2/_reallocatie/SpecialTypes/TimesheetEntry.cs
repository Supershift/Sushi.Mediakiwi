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
    /// Represents a TimesheetEntry entity.
    /// </summary>
    [XmlRoot("entry")]
    public class TimesheetEntry
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TimesheetEntry"/> class.
        /// </summary>
        public TimesheetEntry()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TimesheetEntry"/> class.
        /// </summary>
        /// <param name="idText">The id text.</param>
        /// <param name="description">The description.</param>
        /// <param name="comment">The comment.</param>
        /// <param name="monday">The monday.</param>
        /// <param name="tuesday">The tuesday.</param>
        /// <param name="wednesday">The wednesday.</param>
        /// <param name="thursday">The thursday.</param>
        /// <param name="friday">The friday.</param>
        /// <param name="saturday">The saturday.</param>
        /// <param name="sunday">The sunday.</param>
        public TimesheetEntry(string idText, string description, string comment, decimal monday, decimal tuesday, decimal wednesday, decimal thursday, decimal friday, decimal saturday, decimal sunday)
        {
            this.IDText = idText;
            this.Description = description;
            this.Day_Monday = monday;
            this.Day_Tuesday = tuesday;
            this.Day_Wednesday = wednesday;
            this.Day_Thursday = thursday;
            this.Day_Friday = friday;
            this.Day_Saturday = saturday;
            this.Day_Sunday = sunday;
            this.Comment = comment;
        }

        string m_IDText;
        /// <summary>
        /// Gets or sets the id text.
        /// </summary>
        /// <value>The id text.</value>
        [XmlElement("id")]
        public string IDText
        {
            get { return m_IDText; }
            set { m_IDText = value; }
        }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>The id.</value>
        [XmlIgnore()]
        public int ID
        {
            get { return Utility.ConvertToInt(this.IDText); }
            set { m_IDText = value.ToString(); }
        }

        string m_Description;
        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        [XmlElement("description")]
        public string Description
        {
            get { return m_Description; }
            set { m_Description = value; }
        }

        string m_Comment;
        /// <summary>
        /// Gets or sets the comment.
        /// </summary>
        /// <value>The comment.</value>
        [XmlElement("comment")]
        public string Comment
        {
            get { return m_Comment; }
            set { m_Comment = value; }
        }

        decimal m_Day_Monday;
        /// <summary>
        /// Gets or sets the day_ monday.
        /// </summary>
        /// <value>The day_ monday.</value>
        [XmlElement("d1")]
        public decimal Day_Monday
        {
            get { return m_Day_Monday; }
            set { m_Day_Monday = value; }
        }

        decimal m_Day_Tuesday;
        /// <summary>
        /// Gets or sets the day_ tuesday.
        /// </summary>
        /// <value>The day_ tuesday.</value>
        [XmlElement("d2")]
        public decimal Day_Tuesday
        {
            get { return m_Day_Tuesday; }
            set { m_Day_Tuesday = value; }
        }

        decimal m_Day_Wednesday;
        /// <summary>
        /// Gets or sets the day_ wednesday.
        /// </summary>
        /// <value>The day_ wednesday.</value>
        [XmlElement("d3")]
        public decimal Day_Wednesday
        {
            get { return m_Day_Wednesday; }
            set { m_Day_Wednesday = value; }
        }

        decimal m_Day_Thursday;
        /// <summary>
        /// Gets or sets the day_ thursday.
        /// </summary>
        /// <value>The day_ thursday.</value>
        [XmlElement("d4")]
        public decimal Day_Thursday
        {
            get { return m_Day_Thursday; }
            set { m_Day_Thursday = value; }
        }

        decimal m_Day_Friday;
        /// <summary>
        /// Gets or sets the day_ friday.
        /// </summary>
        /// <value>The day_ friday.</value>
        [XmlElement("d5")]
        public decimal Day_Friday
        {
            get { return m_Day_Friday; }
            set { m_Day_Friday = value; }
        }

        decimal m_Day_Saturday;
        /// <summary>
        /// Gets or sets the day_ saturday.
        /// </summary>
        /// <value>The day_ saturday.</value>
        [XmlElement("d6")]
        public decimal Day_Saturday
        {
            get { return m_Day_Saturday; }
            set { m_Day_Saturday = value; }
        }

        decimal m_Day_Sunday;
        /// <summary>
        /// Gets or sets the day_ sunday.
        /// </summary>
        /// <value>The day_ sunday.</value>
        [XmlElement("d7")]
        public decimal Day_Sunday
        {
            get { return m_Day_Sunday; }
            set { m_Day_Sunday = value; }
        }

        /// <summary>
        /// Gets the day_ total.
        /// </summary>
        /// <value>The day_ total.</value>
        [XmlIgnore()]
        public decimal Day_Total
        {
            get { return decimal.Add(decimal.Add(decimal.Add(decimal.Add(decimal.Add(decimal.Add(this.Day_Monday, this.Day_Tuesday), this.Day_Wednesday), this.m_Day_Thursday), this.Day_Friday), this.Day_Saturday), this.Day_Sunday); }
        }

        /// <summary>
        /// Gets the serialized.
        /// </summary>
        /// <returns></returns>
        public string GetSerialized()
        {
            StringWriter writer = new StringWriter();
            XmlSerializer serializer = new XmlSerializer(typeof(Sushi.Mediakiwi.Data.SpecialTypes.TimesheetEntry));
            serializer.Serialize(writer, this);
            string xml = writer.ToString();
            return xml;
        }
    }
}
