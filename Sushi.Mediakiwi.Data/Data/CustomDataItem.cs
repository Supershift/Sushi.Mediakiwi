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
        ///
        /// </summary>
        internal string m_Property;

        /// <summary>
        /// Gets the property.
        /// </summary>
        /// <value>The property.</value>
        public string Property
        {
            get { return m_Property; }
            set { m_Property = value; }
        }

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
                if (m_Customdata != null) m_Customdata.Apply(this.Property, m_Value);
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
            if (Sushi.Mediakiwi.Data.Utility.IsNumeric(this.Value, out time))
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
            if (Sushi.Mediakiwi.Data.Utility.IsNumeric(this.Value, out candidate))
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
            if (Sushi.Mediakiwi.Data.Utility.IsNumeric(this.Value, out candidate))
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
            if (Sushi.Mediakiwi.Data.Utility.IsNumeric(this.Value, out candidate))
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
                return new Asset();

            int candidate;
            if (Sushi.Mediakiwi.Data.Utility.IsNumeric(this.Value, out candidate))
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
                return new Page();

            int candidate;
            if (Sushi.Mediakiwi.Data.Utility.IsNumeric(this.Value, out candidate))
            {
                return Page.SelectOne(candidate);
            }
            return new Page();
        }

        public Folder ParseFolder()
        {
            if (string.IsNullOrEmpty(Value))
                return new Folder();

            int candidate;
            if (Sushi.Mediakiwi.Data.Utility.IsNumeric(this.Value, out candidate))
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
                return false;

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
                return defaultValue;

            return Value == "1";
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
                var candidate = Sushi.Mediakiwi.Data.Utility.ConvertToIntNullable(Value, false);
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
                return null;

            try
            {
                return Sushi.Mediakiwi.Data.Utility.ConvertToLongNullable(Value);
            }
            catch (OverflowException ex)
            {
                throw new OverflowException(string.Format("Property '{0}' exceeds the int32 limit.", this.Property), ex);
            }
        }

        /// <summary>
        /// Parses the type of the SQL parameter.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        internal System.Data.SqlDbType ParseSqlParameterType(Type type)
        {
            if (type == typeof(Int16)) return System.Data.SqlDbType.Int;
            if (type == typeof(Int32)) return System.Data.SqlDbType.Int;
            if (type == typeof(Int64)) return System.Data.SqlDbType.BigInt;
            if (type == typeof(Decimal)) return System.Data.SqlDbType.Decimal;
            if (type == typeof(String)) return System.Data.SqlDbType.NVarChar;
            if (type == typeof(DateTime)) return System.Data.SqlDbType.DateTime;
            if (type == typeof(Guid)) return System.Data.SqlDbType.UniqueIdentifier;
            if (type == typeof(Boolean)) return System.Data.SqlDbType.Bit;

            throw new Exception("Could not determine the SqlDbType for the Property.");
        }

        internal object ParseSqlParameterValue(Type type)
        {
            //  Set also in CustomDataItem: ParseSqlParameterValue
            //  Set also in CreateFilter
            //  IsNotFilterOrType

            if (type == typeof(Int16)) return ParseInt();
            if (type == typeof(Int32)) return ParseInt();
            if (type == typeof(Int64)) return ParseLong();
            if (type == typeof(Decimal)) return ParseDecimal();
            if (type == typeof(String)) return Value;
            if (type == typeof(DateTime)) return ParseDateTime();
            if (type == typeof(Guid)) return ParseGuid();
            if (type == typeof(Boolean)) return ParseBoolean();

            throw new Exception("Could not determine the SqlDbType for the Property.");
        }

        /// <summary>
        /// Parses the int array.
        /// </summary>
        /// <returns></returns>
        public int[] ParseIntArray()
        {
            if (string.IsNullOrEmpty(Value))
                return null;

            return Sushi.Mediakiwi.Data.Utility.ConvertToIntArray(Value.Split(','));
        }

        /// <summary>
        /// Parses the int list.
        /// </summary>
        /// <returns></returns>
        public List<int> ParseIntList()
        {
            if (string.IsNullOrEmpty(Value))
                return null;

            return Sushi.Mediakiwi.Data.Utility.ConvertToIntList(Value.Split(','));
        }

        /// <summary>
        /// Parses the sub list.
        /// </summary>
        /// <returns></returns>
        public SubList ParseSubList()
        {
            if (this.m_Customdata != null && this.m_Customdata.m_Table.Contains(this.m_Property))
            {
                SubList list2 = this.m_Customdata.m_Table[this.m_Property] as SubList;
                if (list2 == null)
                    return null;

                string urlAddition = list2.m_urlAddition;
                list2 = SubList.GetDeserialized(Value);
                if (list2 == null)
                    list2 = new SubList();
                list2.m_urlAddition = urlAddition;
                this.m_Customdata.m_Table[this.m_Property] = list2;
                return list2;
            }

            SubList list;
            if (string.IsNullOrEmpty(Value))
                list = new SubList();
            else
                list = SubList.GetDeserialized(Value);

            if (this.m_Customdata != null)
                this.m_Customdata.m_Table[this.m_Property] = list;

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

            return Sushi.Mediakiwi.Data.Utility.ConvertToDecimalNullable(Value);
        }

        /// <summary>
        /// Parses the GUID.
        /// </summary>
        /// <returns></returns>
        public Guid? ParseGuid()
        {
            if (string.IsNullOrEmpty(Value))
                return Guid.Empty;

            return Sushi.Mediakiwi.Data.Utility.ConvertToGuid(Value);
        }
    }
}