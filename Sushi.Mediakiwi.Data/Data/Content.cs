using System;
using System.IO;
using System.Xml.Serialization;

namespace Sushi.Mediakiwi.Data
{
    /// <summary>
    /// Represents a Content entity.
    /// </summary>
    public class Content
    {
        /// <summary>
        /// Gets the deserialized.
        /// </summary>
        /// <param name="xml">The XML.</param>
        /// <returns></returns>
        public static Content GetDeserialized(string xml)
        {
            if (xml == null) return null;
            TextReader reader = new StringReader(xml);
            XmlSerializer serializer = new XmlSerializer(typeof(Content));

            try
            {
                Content content = (Content)serializer.Deserialize(reader);
                return content;
            }
            catch (Exception)
            {
                return new Content();
            }
        }

        /// <summary>
        /// Gets the serialized.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <returns></returns>
        public static string GetSerialized(object content)
        {
            StringWriter writer = new StringWriter();
            XmlSerializer serializer = new XmlSerializer(typeof(Content));
            serializer.Serialize(writer, content);
            string xml = writer.ToString();
            return xml;
        }

        /// <summary>
        ///
        /// </summary>
        public Field[] Fields;

        private Field m_lastField;
        private string m_LastProperty;

        /// <summary>
        /// Determines whether the specified property is null.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns>
        /// 	<c>true</c> if the specified property is null; otherwise, <c>false</c>.
        /// </returns>
        public bool IsNull(string property)
        {
            return (SearchArray(property) == null);
        }

        /// <summary>
        /// Gets the <see cref="Wim.Data.Content.Field"/> with the specified property.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        public Field this[string property]
        {
            get
            {
                return SearchArray(property);
            }
        }

        /// <summary>
        /// Searches the array.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns></returns>
        private Field SearchArray(string property)
        {
            if (m_LastProperty == property) return m_lastField;
            m_LastProperty = property;

            if (Fields == null || Fields.Length == 0)
                return null;

            foreach (Field fieldItem in Fields)
            {
                if (fieldItem.Property == property)
                {
                    m_lastField = fieldItem;
                    return m_lastField;
                }
            }
            m_lastField = null;
            return null;
        }
    }
}