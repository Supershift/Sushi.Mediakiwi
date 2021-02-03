using System;
using System.Linq;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Text;

namespace Sushi.Mediakiwi.Data
{
    #region REPLICATED TO Sushi.Mediakiwi.Data.Standard

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
            else if (info.PropertyType == typeof(Sushi.Mediakiwi.Data.Folder)) info.SetValue(instance, this.ParseFolder(), null);
            else if (info.PropertyType == typeof(Sushi.Mediakiwi.Data.Page)) info.SetValue(instance, this.ParsePage(), null);
            else if (info.PropertyType == typeof(Sushi.Mediakiwi.Data.Link)) info.SetValue(instance, this.ParseLink(), null);
            else if (info.PropertyType == typeof(long)|| info.PropertyType == typeof(long?)) info.SetValue(instance, this.ParseLong(), null);
            else if (info.PropertyType == typeof(int[])) info.SetValue(instance, this.ParseIntArray(), null);
            else if (info.PropertyType == typeof(Sushi.Mediakiwi.Data.Image)) info.SetValue(instance, this.ParseImage(), null);
            else if (info.PropertyType == typeof(Sushi.Mediakiwi.Data.Document)) info.SetValue(instance, this.ParseDocument(), null);
            else if (info.PropertyType == typeof(Sushi.Mediakiwi.Data.Asset)) info.SetValue(instance, this.ParseAsset(), null);
            else if (info.PropertyType == typeof(System.Guid)) info.SetValue(instance, this.ParseGuid(), null);
            else if (info.PropertyType == typeof(Sushi.Mediakiwi.Data.SubList)) info.SetValue(instance, this.ParseSubList(), null);
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

         //Sushi.Mediakiwi.Data.Property m_PropertyInstance;
         ///// <summary>
         ///// Gets the property.
         ///// </summary>
         ///// <param name="listID">The list ID.</param>
         ///// <returns></returns>
         //public Sushi.Mediakiwi.Data.Property GetProperty(int listID)
         //{
         //    if (m_PropertyInstance == null)
         //        m_PropertyInstance = Sushi.Mediakiwi.Data.Property.SelectOne(listID, this.m_Property, null);
         //    return m_PropertyInstance;
         //}

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
             set {
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
             if (Wim.Utility.IsNumeric(this.Value, out time))
             {
                 return new DateTime(time);
             }
             return null;
         }

         /// <summary>
         /// Parses the document.
         /// </summary>
         /// <returns></returns>
         public Sushi.Mediakiwi.Data.Document ParseDocument()
         {
             if (string.IsNullOrEmpty(Value))
                 return new Sushi.Mediakiwi.Data.Document();

             int candidate;
             if (Wim.Utility.IsNumeric(this.Value, out candidate))
             {
                 return Sushi.Mediakiwi.Data.Document.SelectOne(candidate);
             }
             return new Sushi.Mediakiwi.Data.Document();
         }

         /// <summary>
         /// Parses the image.
         /// </summary>
         /// <returns></returns>
         public Sushi.Mediakiwi.Data.Image ParseImage()
         {
             if (string.IsNullOrEmpty(Value))
                 return new Sushi.Mediakiwi.Data.Image();

             int candidate;
             if (Wim.Utility.IsNumeric(this.Value, out candidate))
             {
                 return Sushi.Mediakiwi.Data.Image.SelectOne(candidate);
             }
             return new Sushi.Mediakiwi.Data.Image();
         }

         public Sushi.Mediakiwi.Data.Link ParseLink()
         {
             if (string.IsNullOrEmpty(Value))
                 return new Sushi.Mediakiwi.Data.Link();

             int candidate;
             if (Wim.Utility.IsNumeric(this.Value, out candidate))
             {
                 return Sushi.Mediakiwi.Data.Link.SelectOne(candidate);
             }
             return new Sushi.Mediakiwi.Data.Link();
         }

         /// <summary>
         /// 
         /// </summary>
         internal bool IsEditable = true;
         //internal bool IsParsed;

         /// <summary>
         /// Parses the asset.
         /// </summary>
         /// <returns></returns>
         public Sushi.Mediakiwi.Data.Asset ParseAsset()
         {
             if (string.IsNullOrEmpty(Value))
                 return new Sushi.Mediakiwi.Data.Asset();

             int candidate;
             if (Wim.Utility.IsNumeric(this.Value, out candidate))
             {
                 return Sushi.Mediakiwi.Data.Asset.SelectOne(candidate);
             }
             return new Sushi.Mediakiwi.Data.Asset();
         }

         /// <summary>
         /// Parses the page.
         /// </summary>
         /// <returns></returns>
         public Sushi.Mediakiwi.Data.Page ParsePage()
         {
             if (string.IsNullOrEmpty(Value))
                 return new Sushi.Mediakiwi.Data.Page();

             int candidate;
             if (Wim.Utility.IsNumeric(this.Value, out candidate))
             {
                 return Sushi.Mediakiwi.Data.Page.SelectOne(candidate);
             }
             return new Sushi.Mediakiwi.Data.Page();
         }

         public Sushi.Mediakiwi.Data.Folder ParseFolder()
         {
             if (string.IsNullOrEmpty(Value))
                 return new Sushi.Mediakiwi.Data.Folder();

             int candidate;
             if (Wim.Utility.IsNumeric(this.Value, out candidate))
             {
                 return Sushi.Mediakiwi.Data.Folder.SelectOne(candidate);
             }
             return new Sushi.Mediakiwi.Data.Folder();
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
                 var candidate = Wim.Utility.ConvertToIntNullable(Value, false);
                 if (candidate == null)
                 {
                     var sublist = Sushi.Mediakiwi.Data.SubList.GetDeserialized(Value);
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
                 return Wim.Utility.ConvertToLongNullable(Value);
             }
             catch (OverflowException ex)
             {
                 throw new OverflowException(string.Format("Property '{0}' exceeds the int32 limit.", this.Property), ex);
             }
         }

         //internal object ParseSqlParameterValue(Sushi.Mediakiwi.Framework.ContentType type)
         //{
         //    //  Set also in CustomDataItem: ParseSqlParameterValue
         //    //  Set also in CreateFilter
         //    //  IsNotFilterOrType

         //    switch (type)
         //    {
         //        case Sushi.Mediakiwi.Framework.ContentType.Date: 
         //        case Sushi.Mediakiwi.Framework.ContentType.DateTime: 
         //           return ParseDateTime();
         //        case Sushi.Mediakiwi.Framework.ContentType.Choice_Checkbox: 
         //            return ParseBoolean();

         //        case Sushi.Mediakiwi.Framework.ContentType.Binary_Image:
         //        case Sushi.Mediakiwi.Framework.ContentType.Choice_Radio:
         //        case Sushi.Mediakiwi.Framework.ContentType.FolderSelect:
         //        case Sushi.Mediakiwi.Framework.ContentType.PageSelect:
         //        case Sushi.Mediakiwi.Framework.ContentType.Binary_Document:
         //        case Sushi.Mediakiwi.Framework.ContentType.Hyperlink:
         //        case Sushi.Mediakiwi.Framework.ContentType.Choice_Dropdown:
         //            return ParseInt();

         //        case Sushi.Mediakiwi.Framework.ContentType.TextArea:
         //        case Sushi.Mediakiwi.Framework.ContentType.RichText:
         //        case Sushi.Mediakiwi.Framework.ContentType.TextField:
         //            return Value;
         //    }
         //    throw new Exception("Could not determine the SqlDbType for the Property.");
         //}

         /// <summary>
         /// Parses the type of the SQL parameter.
         /// </summary>
         /// <param name="type">The type.</param>
         /// <returns></returns>
         internal System.Data.SqlDbType ParseSqlParameterType(System.Type type)
         {
             if (type == typeof(System.Int16)) return System.Data.SqlDbType.Int;
             if (type == typeof(System.Int32)) return System.Data.SqlDbType.Int;
             if (type == typeof(System.Int64)) return System.Data.SqlDbType.BigInt;
             if (type == typeof(System.Decimal)) return System.Data.SqlDbType.Decimal;
             if (type == typeof(System.String)) return System.Data.SqlDbType.NVarChar;
             if (type == typeof(System.DateTime)) return System.Data.SqlDbType.DateTime;
             if (type == typeof(System.Guid)) return System.Data.SqlDbType.UniqueIdentifier;
             if (type == typeof(System.Boolean)) return System.Data.SqlDbType.Bit;

             throw new Exception("Could not determine the SqlDbType for the Property.");
         }

         internal object ParseSqlParameterValue(System.Type type)
         {
             //  Set also in CustomDataItem: ParseSqlParameterValue
             //  Set also in CreateFilter
             //  IsNotFilterOrType

             if (type == typeof(System.Int16)) return ParseInt();
             if (type == typeof(System.Int32)) return ParseInt();
             if (type == typeof(System.Int64)) return ParseLong();
             if (type == typeof(System.Decimal)) return ParseDecimal();
             if (type == typeof(System.String)) return Value;
             if (type == typeof(System.DateTime)) return ParseDateTime();
             if (type == typeof(System.Guid)) return ParseGuid();
             if (type == typeof(System.Boolean)) return ParseBoolean();

             throw new Exception("Could not determine the SqlDbType for the Property.");
         }

         //internal System.Data.SqlDbType ParseSqlParameterType(Sushi.Mediakiwi.Framework.ContentType type)
         //{
         //    //  Set also in CustomDataItem: ParseSqlParameterValue
         //    //  Set also in CreateFilter
         //    //  IsNotFilterOrType

         //    switch (type)
         //    {
         //        case Sushi.Mediakiwi.Framework.ContentType.DateTime:
         //        case Sushi.Mediakiwi.Framework.ContentType.Date: 
         //           return System.Data.SqlDbType.DateTime;
                 
         //        case Sushi.Mediakiwi.Framework.ContentType.Choice_Checkbox: 
         //           return System.Data.SqlDbType.Bit;

         //        case Sushi.Mediakiwi.Framework.ContentType.Binary_Image:
         //        case Sushi.Mediakiwi.Framework.ContentType.Choice_Radio:
         //        case Sushi.Mediakiwi.Framework.ContentType.FolderSelect:
         //        case Sushi.Mediakiwi.Framework.ContentType.PageSelect:
         //        case Sushi.Mediakiwi.Framework.ContentType.Binary_Document:
         //        case Sushi.Mediakiwi.Framework.ContentType.Hyperlink:
         //        case Sushi.Mediakiwi.Framework.ContentType.Choice_Dropdown:
         //           return System.Data.SqlDbType.Int;

         //        case Sushi.Mediakiwi.Framework.ContentType.TextArea:
         //        case Sushi.Mediakiwi.Framework.ContentType.RichText:
         //        case Sushi.Mediakiwi.Framework.ContentType.TextField:
         //           return System.Data.SqlDbType.NVarChar;
         //    }
         //    throw new Exception("Could not determine the SqlDbType for the Property.");
         //}

         /// <summary>
         /// Parses the int array.
         /// </summary>
         /// <returns></returns>
         public int[] ParseIntArray()
         {
             if (string.IsNullOrEmpty(Value))
                 return null;

             return Wim.Utility.ConvertToIntArray(Value.Split(','));
         }

         /// <summary>
         /// Parses the int list.
         /// </summary>
         /// <returns></returns>
         public List<int> ParseIntList()
         {
             if (string.IsNullOrEmpty(Value))
                 return null;

             return Wim.Utility.ConvertToIntList(Value.Split(','));
         }

         /// <summary>
         /// Parses the sub list.
         /// </summary>
         /// <returns></returns>
         public Data.SubList ParseSubList()
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

             return Wim.Utility.ConvertToDecimalNullable(Value);
         }

         /// <summary>
         /// Parses the GUID.
         /// </summary>
         /// <returns></returns>
         public Guid? ParseGuid()
         {
             if (string.IsNullOrEmpty(Value))
                 return Guid.Empty;

             return Wim.Utility.ConvertToGuid(Value);
         }
     }

    #endregion REPLICATED TO Sushi.Mediakiwi.Data.Standard

    /// <summary>
    /// 
    /// </summary>
    public class CustomData
    {

        public void CopyFromMaster(int propertylistID, CustomData parent, int currentSiteID)
        {
            var properties = Sushi.Mediakiwi.Data.Property.SelectAll(propertylistID);
            foreach (var item in parent.Items)
            {

                var property = (from candidate in properties where candidate.FieldName == item.Property select candidate).FirstOrDefault();
                if (property == null)
                {
                    this.ApplyObject(item.Property, item.Value);
                    continue;
                }

                if (string.IsNullOrEmpty(item.Value) || item.Value == "0")
                {
                    this.ApplyObject(item.Property, item.Value);
                    continue;
                }

                if (property.TypeID == (int)Sushi.Mediakiwi.Framework.ContentType.RichText)
                {
                    string candidate = item.Value;
                    Sushi.Mediakiwi.Framework.ContentInfoItem.RichTextLink.CreateLinkMasterCopy(ref candidate, currentSiteID);
                    this.ApplyObject(item.Property, candidate);
                }

                else if (property.TypeID == (int)Sushi.Mediakiwi.Framework.ContentType.FolderSelect)
                {
                    Sushi.Mediakiwi.Data.Folder folderInstance = Sushi.Mediakiwi.Data.Folder.SelectOneChild(Wim.Utility.ConvertToInt(item.Value), currentSiteID);
                    this.ApplyObject(item.Property, folderInstance.ID);
                }
                else if (property.TypeID == (int)Sushi.Mediakiwi.Framework.ContentType.Hyperlink)
                {
                    Sushi.Mediakiwi.Data.Link link = Sushi.Mediakiwi.Data.Link.SelectOne(Wim.Utility.ConvertToInt(item.Value));
                    if (link != null && !link.IsNewInstance)
                    {
                        if (link.Type == Sushi.Mediakiwi.Data.Link.LinkType.InternalPage)
                        {
                            Sushi.Mediakiwi.Data.Page pageInstance = Sushi.Mediakiwi.Data.Page.SelectOneChild(link.PageID.Value, currentSiteID, false);
                            if (pageInstance != null)
                            {
                                link.ID = 0;
                                link.PageID = pageInstance.ID;
                                link.Save();
                                this.ApplyObject(item.Property, link.ID);
                            }
                        }
                        else
                        {
                            link.ID = 0;
                            link.Save();
                            this.ApplyObject(item.Property, link.ID);
                        }
                    }
                }
                else if (property.TypeID == (int)Sushi.Mediakiwi.Framework.ContentType.PageSelect)
                {
                    Sushi.Mediakiwi.Data.Page pageInstance = Sushi.Mediakiwi.Data.Page.SelectOneChild(Wim.Utility.ConvertToInt(item.Value), currentSiteID, false);
                    this.ApplyObject(item.Property, pageInstance.ID);
                }
                else
                {
                    this.ApplyObject(item.Property, item.Value);
                }
            }
        }


        internal List<Property> m_TemporaryProperties;
        /// <summary>
        /// Adds the temporary property.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="title">The title.</param>
        /// <param name="isMandatory">if set to <c>true</c> [is mandatory].</param>
        /// <param name="maxLength">Length of the max.</param>
        /// <param name="listItemArray">The list item array.</param>
        /// <param name="interactiveHelp">The interactive help.</param>
        /// <value>The items.</value>
        public void AddTemporaryProperty(Sushi.Mediakiwi.Framework.ContentType type, string fieldName, string title, bool isMandatory, int maxLength, Sushi.Mediakiwi.Framework.MetaDataList[] listItemArray, string interactiveHelp)
        {
            bool foundProperty = false;
            Sushi.Mediakiwi.Data.Property tmp = new Sushi.Mediakiwi.Data.Property();

            if (m_TemporaryProperties == null)
                m_TemporaryProperties = new List<Property>();
            else
            {
                foreach (Property x in m_TemporaryProperties)
                {
                    if (x.FieldName == fieldName)
                    {
                        tmp = x;
                        foundProperty = true;
                        break;
                    }
                }
            }
            tmp.TypeID = (int)type;
            tmp.Title = title;
            tmp.FieldName = fieldName;

            Sushi.Mediakiwi.Framework.MetaData meta = new Sushi.Mediakiwi.Framework.MetaData();
            meta.Title = tmp.Title;
            meta.MaxValueLength = maxLength.ToString();
            meta.Mandatory = isMandatory ? "1" : "0";
            meta.InteractiveHelp = interactiveHelp;
            meta.CollectionList = listItemArray;

            tmp.Data = Wim.Utility.GetSerialized(meta);

            if (!foundProperty)
                m_TemporaryProperties.Add(tmp);
        }


        #region REPLICATED TO Sushi.Mediakiwi.Data.Standard

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

        ///// <summary>
        ///// Applies the state of the internal editable.
        ///// </summary>
        ///// <param name="property">The property.</param>
        //public void ApplyNotEditable(string property)
        //{
        //    this[property].IsEditable = false;
        //}

        /// <summary>
        /// Applies the specified property.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="value">The value.</param>
        public void Apply(string property, decimal value)
        {
            Apply(property, Wim.Utility.ConvertToDecimalString(value));
        }

        /// <summary>
        /// Combines the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        public void Combine(Sushi.Mediakiwi.Data.CustomData data)
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
            else if (value.GetType() == typeof(Sushi.Mediakiwi.Data.SubList))
                Apply(property, ((Sushi.Mediakiwi.Data.SubList)value).Serialized);
            else if (value.GetType() == typeof(int[]))
                Apply(property, Wim.Utility.ConvertToCsvString(value as int[]));
            else if (value.GetType() == typeof(List<int>))
                Apply(property, Wim.Utility.ConvertToCsvString(((List<int>)value).ToArray()));
            else
                Apply(property, value.ToString());
        }

        /// <summary>
        /// Applies the specified property (if the property exists it's value is updated).
        /// If the value = NULL it will remove the entry.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="value">The value.</param>
        public void Apply(string property, string value)
        {
            CustomDataItem data = this[property];

            if (value == null)
            {
                if (HasProperty(property))
                    m_Items.Remove(data);
                return;
            }

            data.m_Value = value;

            if (!HasProperty(property))
            {
                data.m_Property = property;
                m_Items.Add(data);
            }
            if (Changed != null) 
                Changed(this, null);
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

        CustomDataItem m_CurrentDataItem;
        internal System.Collections.Hashtable m_Table;

        /// <summary>
        /// Gets the <see cref="System.String"/> with the specified property.
        /// </summary>
        /// <value></value>
        public CustomDataItem this[string property]
        {
            get
            {
                if (m_Items == null) return new CustomDataItem(this);

                if (m_Table == null)
                    m_Table = new System.Collections.Hashtable();

                //  [20100128:MM] Removed as of multi-lingual caching problems!
                //if (m_CurrentDataItem != null && m_CurrentDataItem.Property == property)
                //{
                //    return m_CurrentDataItem;
                //}

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
                //tmp.IsNull = true;

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

        List<CustomDataItem> m_Items;
        /// <summary>
        /// Gets or sets the items.
        /// </summary>
        /// <value>The items.</value>
        public CustomDataItem[] Items
        {
            get { return m_Items.ToArray(); }
            set { 
                m_Items = new List<CustomDataItem>();
                foreach (CustomDataItem item in value)
                    m_Items.Add(item);
            }
        }

        public event EventHandler Changed;

        /// <summary>
        /// Gets the serialized.
        /// </summary>
        /// <value>The serialized.</value>
        public string Serialized
        {
            get {
                if (Items == null || Items.Length == 0)
                    return null;

                return Wim.Utility.GetSerialized(Items);
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

            serializedData = Wim.Utility.CleanXmlData(serializedData, true);

            CustomDataItem[] fields = Wim.Utility.GetDeserialized(typeof(CustomDataItem[]), serializedData) as CustomDataItem[];

            if (fields == null) return;

            string firstPropertyName = null;

            List<string> properties = new List<string>();
            foreach (CustomDataItem field in fields)
            {
                if (properties.Contains(field.Property))
                    continue;

                m_Items.Add(field);
                properties.Add(field.Property);
            }
        }

        #endregion REPLICATED TO Sushi.Mediakiwi.Data.Standard
    }
}
