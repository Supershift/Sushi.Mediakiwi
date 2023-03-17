using System;

namespace Sushi.Mediakiwi.Data
{
    /// <summary>
    /// Represents a Field entity.
    /// </summary>
    public class Field
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Field"/> class.
        /// </summary>
        public Field()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Field"/> class.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="type">The type.</param>
        /// <param name="value">The value.</param>
        public Field(string property, ContentType type, string value)
        {
            Type = (int)type;
            Property = property;
            Value = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Field"/> class.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="type">The type.</param>
        /// <param name="value">The value.</param>
        public Field(string property, int type, string value)
        {
            Type = type;
            Property = property;
            Value = value;
        }

        /// <summary>
        ///
        /// </summary>
        public string Property;

        /// <summary>
        ///
        /// </summary>
        public int Type;

        /// <summary>
        ///
        /// </summary>
        public string Value;

        /// <summary>
        ///
        /// </summary>
        [System.Xml.Serialization.XmlIgnore()]
        public string InheritedValue;

        /// <summary>
        ///
        /// </summary>
        [System.Xml.Serialization.XmlIgnore()]
        public Property PropertyInfo;

        /// <summary>
        /// If the Field value is of type DateTime this method return the dateTime.
        /// </summary>
        /// <returns></returns>
        public DateTime? GetDateValue()
        {
            if (Type == 13 || Type == 15)
            {
                if (!string.IsNullOrEmpty(Value))
                {
                    long ticks;
                    if (Utility.IsNumeric(Value, out ticks))
                    {
                        return new DateTime(ticks);
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Parses the int.
        /// </summary>
        /// <returns></returns>
        public int? ParseInt()
        {
            if (string.IsNullOrEmpty(Value))
                return null;

            try
            {
                return Utility.ConvertToIntNullable(Value, false);
            }
            catch (OverflowException ex)
            {
                throw new OverflowException(string.Format("Property '{0}' exceeds the int32 limit.", this.Property), ex);
            }
        }

        private Image m_Image;

        /// <summary>
        /// If the Field value is of type Binary Image this method returns it.
        /// </summary>
        /// <returns></returns>
        [System.Xml.Serialization.XmlIgnore()]
        public Image Image
        {
            get
            {
                if (Type == (int)ContentType.Binary_Image)
                {
                    if (string.IsNullOrEmpty(Value)) return null;
                    m_Image = Image.SelectOne(Convert.ToInt32(Value));
                }
                return m_Image;
            }
        }

        [System.Xml.Serialization.XmlIgnore()]
        internal object DataObject
        {
            get
            {
                if (Type == (int)ContentType.Binary_Image)
                    return this.Image;
                if (Type == (int)ContentType.Binary_Document)
                    return this.Document;
                if (Type == (int)ContentType.Hyperlink)
                    return this.Link;
                if (Type == (int)ContentType.PageSelect)
                    return this.Page;
                return null;
            }
        }

        private Page m_Page;

        /// <summary>
        /// If the Field value is of type Binary Image this method returns it.
        /// </summary>
        /// <returns></returns>
        [System.Xml.Serialization.XmlIgnore()]
        public Page Page
        {
            get
            {
                if (Type == (int)ContentType.PageSelect)
                {
                    if (string.IsNullOrEmpty(Value)) return null;
                    m_Page = Page.SelectOne(Convert.ToInt32(Value));
                }
                return m_Page;
            }
        }

        private Link m_Link;

        /// <summary>
        /// If the Field value is of type Binary Image this method returns it.
        /// </summary>
        /// <returns></returns>
        [System.Xml.Serialization.XmlIgnore()]
        public Link Link
        {
            get
            {
                if (Type == (int)ContentType.Hyperlink)
                {
                    if (string.IsNullOrEmpty(Value)) return null;
                    m_Link = Link.SelectOne(Convert.ToInt32(Value));
                }
                return m_Link;
            }
        }

        private Document m_Document;

        /// <summary>
        /// Gets the document.
        /// </summary>
        /// <value>
        /// The document.
        /// </value>
        [System.Xml.Serialization.XmlIgnore()]
        public Document Document
        {
            get
            {
                if (m_Document == null)
                {
                    if (Type == (int)ContentType.Binary_Document)
                    {
                        if (string.IsNullOrEmpty(Value)) return null;
                        m_Document = Document.SelectOne(Convert.ToInt32(Value));
                    }
                }
                return m_Document;
            }
        }

        private Asset m_Asset;

        /// <summary>
        /// Gets the document.
        /// </summary>
        /// <value>
        /// The document.
        /// </value>
        [System.Xml.Serialization.XmlIgnore()]
        public Asset Asset
        {
            get
            {
                if (m_Asset == null)
                {
                    if (Type == (int)ContentType.Binary_Document || Type == (int)ContentType.Binary_Image)
                    {
                        if (string.IsNullOrEmpty(Value)) return null;
                        m_Asset = Asset.SelectOne(Convert.ToInt32(Value));
                    }
                }
                return m_Asset;
            }
        }
    }
}