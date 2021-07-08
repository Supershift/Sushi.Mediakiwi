using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Data
{
    /// <summary>
    ///
    /// </summary>
    public class CustomData
    {
        public async Task<Dictionary<string, ContentItem>> ToContentAsync(string ContentDeliveryPrefix = null)
        {
            return await ContentCreatorLogic.GetContentAsync(this, ContentDeliveryPrefix).ConfigureAwait(false);
        }

        public async Task<string> ToJsonAsync(string ContentDeliveryPrefix = null)
        {
            var result = await ToContentAsync(ContentDeliveryPrefix).ConfigureAwait(false);
            return JsonSerializer.Serialize(result, new JsonSerializerOptions
            {
                IgnoreNullValues = true,
                IgnoreReadOnlyProperties = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
        }

        /// <summary>
        /// Applies the specified property (if the property exists it's value is updated).
        /// If the value = NULL it will remove the entry.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="value">The value.</param>
        public void Apply(string property, int value)
        {
            Apply(property, value.ToString());
        }

        /// <summary>
        /// Applies the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        public void Apply(CustomData data)
        {
            if (data == null) return;
            foreach (var item in data.Items)
                Apply(item.Property, item.Value);
        }

        /// <summary>
        /// Applies the specified property.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="value">The value.</param>
        public void Apply(string property, DateTime value)
        {
            Apply(property, value.Ticks.ToString());
        }

        /// <summary>
        /// Applies the specified property.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        public void Apply(string property, bool value)
        {
            Apply(property, value ? "1" : "0");
        }

        /// <summary>
        /// Applies the specified property.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="value">The value.</param>
        public void Apply(string property, decimal value)
        {
            Apply(property, Utility.ConvertToDecimalString(value));
        }

        /// <summary>
        /// Combines the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        public void Combine(CustomData data)
        {
            if (data == null) return;
            if (data.Items == null) return;
            if (data.Items.Length == 0) return;

            foreach (var item in data.Items)
                this.Apply(item.Property, item.Value);
        }

        /// <summary>
        /// Applies the object.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="value">The value.</param>
        public void ApplyObject(string property, object value)
        {
            if (value == null)
                Apply(property, null);
            else if (value.GetType() == typeof(DateTime) || value.GetType() == typeof(DateTime?))
                Apply(property, (DateTime)value);
            else if (value.GetType() == typeof(int) || value.GetType() == typeof(int?))
                Apply(property, (int)value);
            else if (value.GetType() == typeof(bool) || value.GetType() == typeof(bool?))
                Apply(property, (bool)value);
            else if (value.GetType() == typeof(decimal) || value.GetType() == typeof(decimal?))
                Apply(property, (decimal)value);
            else if (value.GetType() == typeof(SubList))
                Apply(property, ((SubList)value).Serialized);
            else if (value.GetType() == typeof(int[]))
                Apply(property, Utility.ConvertToCsvString(value as int[]));
            else if (value.GetType() == typeof(List<int>))
                Apply(property, Utility.ConvertToCsvString(((List<int>)value).ToArray()));
            else
                Apply(property, value.ToString());
        }

        /// <summary>
        /// Applies the specified property (if the property exists it's value is updated).
        /// If the value = NULL it will remove the entry.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="value">The value.</param>
        public void Apply(string property, string value, int? type = null)
        {
            CustomDataItem data = this[property];

            if (value == null)
            {
                if (HasProperty(property))
                    m_Items.Remove(data);
                return;
            }

            data.Type = type.HasValue ? type.ToString() : null;
            data.m_Value = value;

            if (!HasProperty(property))
            {
                data.Property = property;
                m_Items.Add(data);
            }

            Changed?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Determines whether the specified property has property.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns>
        /// 	<c>true</c> if the specified property has property; otherwise, <c>false</c>.
        /// </returns>
        public bool HasProperty(string property)
        {
            foreach (CustomDataItem field in m_Items)
            {
                if (field.Property.Equals(property, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }

        public CustomData Clone()
        {
            return this.MemberwiseClone() as CustomData;
        }

        private CustomDataItem m_CurrentDataItem;
        internal System.Collections.Hashtable m_Table;

        /// <summary>
        /// Gets the <see cref="string"/> with the specified property.
        /// </summary>
        /// <value></value>
        public CustomDataItem this[string property]
        {
            get
            {
                if (m_Items == null) return new CustomDataItem(this);

                if (m_Table == null)
                    m_Table = new System.Collections.Hashtable();

                foreach (CustomDataItem field in m_Items)
                {
                    if (field == null || field.Property == null) continue;
                    if (field.Property.Equals(property, StringComparison.OrdinalIgnoreCase))
                    {
                        m_CurrentDataItem = field;
                        field.m_Customdata = this;
                        return field;
                    }
                }
                CustomDataItem tmp = new CustomDataItem(this);
                tmp.Property = property;

                return tmp;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomData"/> class.
        /// </summary>
        public CustomData()
        {
            m_Items = new List<CustomDataItem>();
        }

        public CustomData(string serializedData)
        {
            m_Items = new List<CustomDataItem>();
            this.ApplySerialized(serializedData);
        }

        private List<CustomDataItem> m_Items;

        /// <summary>
        /// Gets or sets the items.
        /// </summary>
        /// <value>The items.</value>
        public CustomDataItem[] Items
        {
            get { return m_Items.ToArray(); }
            set
            {
                m_Items = new List<CustomDataItem>();
                foreach (CustomDataItem item in value)
                    m_Items.Add(item);
            }
        }

        public event EventHandler Changed;

        /// <summary>
        /// Gets the XML serialized value.
        /// </summary>
        /// <value>The serialized value .</value>
        public string Serialized
        {
            get
            {
                if (Items == null || Items.Length == 0)
                    return null;

                return Utility.GetSerialized(Items);
            }
        }

        public System.Xml.Linq.XElement XmlLinqElement
        {
            get
            {
                TextReader sr = new StringReader(Serialized);
                return System.Xml.Linq.XElement.Load(sr);
            }
        }

        /// <summary>
        /// Applies the serialized.
        /// </summary>
        /// <param name="serializedData">The serialized data.</param>
        public void ApplySerialized(string serializedData)
        {
            if (m_Items == null || m_Items.Count > 0)
                m_Items = new List<CustomDataItem>();

            if (string.IsNullOrEmpty(serializedData) || serializedData == @"<ArrayOfCustomDataItem xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" />")
                return;

            serializedData = Utility.CleanXmlData(serializedData, true);

            CustomDataItem[] fields = Utility.GetDeserialized(typeof(CustomDataItem[]), serializedData) as CustomDataItem[];

            if (fields == null) return;

            List<string> properties = new List<string>();
            foreach (CustomDataItem field in fields)
            {
                if (properties.Contains(field.Property))
                    continue;

                m_Items.Add(field);
                properties.Add(field.Property);
            }
        }
    }
}