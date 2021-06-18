using System;
using System.Collections.Generic;

namespace Sushi.Mediakiwi.Data
{
    /// <summary>
    ///
    /// </summary>
    [System.Xml.Serialization.XmlType("Data")]
    public class CustomDataItem
    {
        public Field ToField()
        {
            return new Field
            {
                Type = Convert.ToInt32(Type, System.Globalization.CultureInfo.InvariantCulture),
                Property = Property,
                Value = Value
            };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomDataItem"/> class.
        /// </summary>
        public CustomDataItem()
        {
        }

        internal CustomData m_Customdata;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomDataItem"/> class.
        /// </summary>
        /// <param name="customdata">The customdata.</param>
        public CustomDataItem(CustomData customdata)
        {
            m_Customdata = customdata;
        }

        /// <summary>
        /// Applies the value to a specific object instance property.
        /// </summary>
        /// <param name="instance">The instance.</param>
        public void Apply(object instance)
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance), "Should not be NULL");

            System.Reflection.PropertyInfo info = instance.GetType().GetProperty(this.Property);
            if (info == null) return;
            if (info.PropertyType == typeof(string)) info.SetValue(instance, this.Value, null);
            else if (info.PropertyType == typeof(bool) || info.PropertyType == typeof(bool?)) info.SetValue(instance, this.ParseBoolean(), null);
            else if (info.PropertyType == typeof(int) || info.PropertyType == typeof(int?)) info.SetValue(instance, this.ParseInt(), null);
            else if (info.PropertyType == typeof(DateTime) || info.PropertyType == typeof(DateTime?)) info.SetValue(instance, this.ParseDateTime(), null);
            else if (info.PropertyType == typeof(decimal) || info.PropertyType == typeof(decimal?)) info.SetValue(instance, this.ParseDecimal(), null);
            else if (info.PropertyType == typeof(Folder)) info.SetValue(instance, this.ParseFolder(), null);
            else if (info.PropertyType == typeof(Page)) info.SetValue(instance, this.ParsePage(), null);
            else if (info.PropertyType == typeof(Link)) info.SetValue(instance, this.ParseLink(), null);
            else if (info.PropertyType == typeof(long) || info.PropertyType == typeof(long?)) info.SetValue(instance, this.ParseLong(), null);
            else if (info.PropertyType == typeof(int[])) info.SetValue(instance, this.ParseIntArray(), null);
            else if (info.PropertyType == typeof(Image)) info.SetValue(instance, this.ParseImage(), null);
            else if (info.PropertyType == typeof(Document)) info.SetValue(instance, this.ParseDocument(), null);
            else if (info.PropertyType == typeof(Asset)) info.SetValue(instance, this.ParseAsset(), null);
            else if (info.PropertyType == typeof(Guid)) info.SetValue(instance, this.ParseGuid(), null);
            else if (info.PropertyType == typeof(SubList)) info.SetValue(instance, this.ParseSubList(), null);
        }

        /// <summary>
        ///
        /// </summary>
        [System.Xml.Serialization.XmlIgnore()]
        public bool IsNull
        {
            get
            {
                return string.IsNullOrEmpty(this.Value);
            }
        }

        /// <summary>
        /// Gets the property.
        /// </summary>
        /// <value>The property.</value>
        public string Type { get; set; }

        /// <summary>
        /// Gets the property.
        /// </summary>
        /// <value>The property.</value>
        public string Property { get; set; }

        /// <summary>
        ///
        /// </summary>
        internal string m_Value;

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>The value.</value>
        public string Value
        {
            get { return m_Value; }
            set
            {
                m_Value = value;
                if (m_Customdata != null) m_Customdata.Apply(Property, m_Value);
            }
        }

        /// <summary>
        /// Parses the date time.
        /// </summary>
        /// <returns></returns>
        public DateTime? ParseDateTime()
        {
            if (string.IsNullOrEmpty(Value))
                return null;

            long time;
            if (Utility.IsNumeric(this.Value, out time))
            {
                return new DateTime(time);
            }
            return null;
        }

        /// <summary>
        /// Parses the document.
        /// </summary>
        /// <returns></returns>
        public Document ParseDocument()
        {
            if (string.IsNullOrEmpty(Value))
                return new Document();

            int candidate;
            if (Utility.IsNumeric(this.Value, out candidate))
            {
                return Document.SelectOne(candidate);
            }
            return new Document();
        }

        /// <summary>
        /// Parses the image.
        /// </summary>
        /// <returns></returns>
        public Image ParseImage()
        {
            if (string.IsNullOrEmpty(Value))
                return new Image();

            int candidate;
            if (Utility.IsNumeric(this.Value, out candidate))
            {
                return Image.SelectOne(candidate);
            }
            return new Image();
        }

        public Link ParseLink()
        {
            if (string.IsNullOrEmpty(Value))
                return new Link();

            int candidate;
            if (Utility.IsNumeric(this.Value, out candidate))
            {
                return Link.SelectOne(candidate);
            }
            return new Link();
        }

        /// <summary>
        ///
        /// </summary>
        internal bool IsEditable = true;

        /// <summary>
        /// Parses the asset.
        /// </summary>
        /// <returns></returns>
        public Asset ParseAsset()
        {
            if (string.IsNullOrEmpty(Value))
            {
                return new Asset();
            }

            int candidate;
            if (Utility.IsNumeric(this.Value, out candidate))
            {
                return Asset.SelectOne(candidate);
            }
            return new Asset();
        }

        /// <summary>
        /// Parses the page.
        /// </summary>
        /// <returns></returns>
        public Page ParsePage()
        {
            if (string.IsNullOrEmpty(Value))
            {
                return new Page();
            }

            int candidate;
            if (Utility.IsNumeric(this.Value, out candidate))
            {
                return Page.SelectOne(candidate);
            }
            return new Page();
        }

        public Folder ParseFolder()
        {
            if (string.IsNullOrEmpty(Value))
            {
                return new Folder();
            }

            int candidate;
            if (Utility.IsNumeric(this.Value, out candidate))
            {
                return Folder.SelectOne(candidate);
            }
            return new Folder();
        }

        /// <summary>
        /// Parses the boolean.
        /// </summary>
        /// <returns></returns>
        public bool ParseBoolean()
        {
            if (string.IsNullOrEmpty(Value))
            {
                return false;
            }

            return Value == "1";
        }

        /// <summary>
        /// Parses the boolean.
        /// </summary>
        /// <param name="defaultValue">if set to <c>true</c> [default value].</param>
        /// <returns></returns>
        public bool ParseBoolean(bool defaultValue)
        {
            if (string.IsNullOrEmpty(Value))
            {
                return defaultValue;
            }

            return Value == "1";
        }

        /// <summary>
        /// Parses the int.
        /// </summary>
        /// <returns></returns>
        public int? ParseInt()
        {
            if (string.IsNullOrEmpty(Value))
            {
                return null;
            }

            try
            {
                var candidate = Utility.ConvertToIntNullable(Value, false);
                if (candidate == null)
                {
                    var sublist = SubList.GetDeserialized(Value);
                    if (sublist != null)
                    {
                        return sublist.GetID(0);
                    }
                }

                return candidate;
            }
            catch (OverflowException ex)
            {
                throw new OverflowException(string.Format("Property '{0}' exceeds the int32 limit.", this.Property), ex);
            }
        }

        public long? ParseLong()
        {
            if (string.IsNullOrEmpty(Value))
            {
                return null;
            }

            try
            {
                return Utility.ConvertToLongNullable(Value);
            }
            catch (OverflowException ex)
            {
                throw new OverflowException(string.Format("Property '{0}' exceeds the int32 limit.", this.Property), ex);
            }
        }

        /// <summary>
        /// Parses the int array.
        /// </summary>
        /// <returns></returns>
        public int[] ParseIntArray()
        {
            if (string.IsNullOrEmpty(Value))
            {
                return default;
            }

            return Utility.ConvertToIntArray(Value.Split(','));
        }

        /// <summary>
        /// Parses the int list.
        /// </summary>
        /// <returns></returns>
        public List<int> ParseIntList()
        {
            if (string.IsNullOrEmpty(Value))
            {
                return null;
            }

            return Utility.ConvertToIntList(Value.Split(','));
        }

        /// <summary>
        /// Parses the sub list.
        /// </summary>
        /// <returns></returns>
        public SubList ParseSubList()
        {
            if (this.m_Customdata != null && this.m_Customdata.m_Table.Contains(Property))
            {
                SubList list2 = this.m_Customdata.m_Table[Property] as SubList;
                if (list2 == null)
                    return null;

                string urlAddition = list2.m_urlAddition;
                list2 = SubList.GetDeserialized(Value);
                if (list2 == null)
                    list2 = new SubList();
                list2.m_urlAddition = urlAddition;
                this.m_Customdata.m_Table[Property] = list2;
                return list2;
            }

            SubList list;
            if (string.IsNullOrEmpty(Value))
                list = new SubList();
            else
                list = SubList.GetDeserialized(Value);

            if (this.m_Customdata != null)
                this.m_Customdata.m_Table[Property] = list;

            return list;
        }

        /// <summary>
        /// Parses the decimal.
        /// </summary>
        /// <returns></returns>
        public decimal? ParseDecimal()
        {
            if (string.IsNullOrEmpty(Value))
                return null;

            return Utility.ConvertToDecimalNullable(Value);
        }

        /// <summary>
        /// Parses the GUID.
        /// </summary>
        /// <returns></returns>
        public Guid? ParseGuid()
        {
            if (string.IsNullOrEmpty(Value))
                return Guid.Empty;

            return Utility.ConvertToGuid(Value);
        }
    }
}