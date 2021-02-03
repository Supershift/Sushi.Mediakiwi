using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Wim.Data.Interfaces;

namespace Sushi.Mediakiwi.Data
{
    /// <summary>
    /// Represents a SubList entity.
    /// </summary>
    [XmlRoot("list")]
    public class SubList
    {
        public SubList(int ID, string description)
        {
            m_Items = new SubListitem[0];
            this.Add(new SubListitem(ID, description));
        }

        public void SetValue(int ID, string description)
        {
            m_Items = new SubListitem[0];
            this.Add(new SubListitem(ID, description));
        }

        /// <summary>
        /// Gets the serialized.
        /// </summary>
        /// <param name="sublist">The sublist.</param>
        /// <returns></returns>
        public static string GetSerialized(object sublist)
        {
            StringWriter writer = new StringWriter();
            XmlSerializer serializer = new XmlSerializer(typeof(SubList));
            serializer.Serialize(writer, sublist);
            string xml = writer.ToString();
            return xml;
        }

        /// <summary>
        /// Gets the deserialized.
        /// </summary>
        /// <param name="xml">The XML.</param>
        /// <returns></returns>
        public static SubList GetDeserialized(string xml)
        {
            if (xml == null || xml.Trim() == string.Empty) return null;
            //  Temporary
            xml = xml.Replace("<Items>", string.Empty);
            xml = xml.Replace("</Items>", string.Empty);
            xml = xml.Replace("<SubListitem>", "<item>");
            xml = xml.Replace("</SubListitem>", "</item>");

            try
            {
                TextReader reader = new StringReader(xml);
                XmlSerializer serializer = new XmlSerializer(typeof(SubList));
                SubList list = (SubList)serializer.Deserialize(reader);
                return list;
            }
            catch (Exception) { return null; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SubList"/> class and apply the first record
        /// </summary>
        /// <param name="ID">The ID.</param>
        /// <param name="description">The description.</param>
        public SubList(string ID, string description)
        {
            m_Items = new SubListitem[0];
            this.Add(new SubListitem(ID, description));
        }

        public void SetValue(string ID, string description)
        {
            m_Items = new SubListitem[0];
            this.Add(new SubListitem(ID, description));
        }

        internal string m_urlAddition;

        /// <summary>
        /// Gets or sets the URL addition.
        /// </summary>
        /// <value>The URL addition.</value>
        [XmlElement("urlext")]
        public string UrlAddition
        {
            get { return m_urlAddition; }
            set { m_urlAddition = value; }
        }

        /// <summary>
        /// Applies the query string parameter.
        /// </summary>
        /// <param name="collection">The collection.</param>
        public void ApplyQueryStringParameter(System.Collections.Specialized.NameValueCollection collection)
        {
            m_urlAddition = "";
            foreach (string key in collection.AllKeys)
            {
                m_urlAddition += string.Concat("&", key, "=", collection[key]);
            }
        }

        private DateTime m_Created;

        /// <summary>
        /// Gets or sets the created.
        /// </summary>
        /// <value>The created.</value>
        [XmlElement("created")]
        public DateTime Created
        {
            get
            {
                if (this.m_Created == DateTime.MinValue) this.m_Created = Common.DatabaseDateTime;
                return m_Created;
            }
            set { m_Created = value; }
        }

        /// <summary>
        /// Gets a value indicating whether an item (1 to 3) in the array contains additional meta information.
        /// </summary>
        /// <param name="additionalField">The additional field.</param>
        /// <returns>
        /// 	<c>true</c> if [has meta information] [the specified additional field]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasMetaInformation(int additionalField)
        {
            if (Items != null)
            {
                foreach (SubListitem item in Items)
                {
                    if (additionalField == 1 && item.MetaInfo1 != null) return true;
                    if (additionalField == 2 && item.MetaInfo2 != null) return true;
                    if (additionalField == 3 && item.MetaInfo3 != null) return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Represents a SubListitem entity.
        /// </summary>
        public class SubListitem : ISubListitem
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="SubListitem"/> class.
            /// </summary>
            public SubListitem()
            {
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="SubListitem"/> class.
            /// </summary>
            /// <param name="ID">The ID.</param>
            /// <param name="description">The description.</param>
            public SubListitem(string ID, string description)
                : this(ID, description, null) { }

            /// <summary>
            /// Initializes a new instance of the <see cref="SubListitem"/> class.
            /// </summary>
            /// <param name="ID">The ID.</param>
            /// <param name="description">The description.</param>
            /// <param name="metaInfo1">The meta info1.</param>
            public SubListitem(string ID, string description, object metaInfo1)
                : this(ID, description, metaInfo1, null) { }

            /// <summary>
            /// Initializes a new instance of the <see cref="SubListitem"/> class.
            /// </summary>
            /// <param name="ID">The ID.</param>
            /// <param name="description">The description.</param>
            /// <param name="metaInfo1">The meta info1.</param>
            /// <param name="metaInfo2">The meta info2.</param>
            public SubListitem(string ID, string description, object metaInfo1, object metaInfo2)
                : this(ID, description, metaInfo1, metaInfo2, null) { }

            /// <summary>
            /// Initializes a new instance of the <see cref="SubListitem"/> class.
            /// </summary>
            /// <param name="ID">The ID.</param>
            /// <param name="description">The description.</param>
            /// <param name="metaInfo1">The meta info1.</param>
            /// <param name="metaInfo2">The meta info2.</param>
            /// <param name="metaInfo3">The meta info3.</param>
            public SubListitem(string ID, string description, object metaInfo1, object metaInfo2, object metaInfo3)
            {
                if (ID.StartsWith("T"))
                    ID = ID.Remove(0, 1);

                TextID = ID;
                Description = description;
                MetaInfo1 = metaInfo1;
                MetaInfo2 = metaInfo2;
                MetaInfo3 = metaInfo3;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="SubListitem"/> class.
            /// </summary>
            /// <param name="ID">The ID.</param>
            /// <param name="description">The description.</param>
            public SubListitem(int ID, string description)
                : this(ID, description, null) { }

            /// <summary>
            /// Initializes a new instance of the <see cref="SubListitem"/> class.
            /// </summary>
            /// <param name="ID">The ID.</param>
            /// <param name="description">The description.</param>
            /// <param name="metaInfo1">The meta info1.</param>
            public SubListitem(int ID, string description, object metaInfo1)
                : this(ID, description, metaInfo1, null) { }

            /// <summary>
            /// Initializes a new instance of the <see cref="SubListitem"/> class.
            /// </summary>
            /// <param name="ID">The ID.</param>
            /// <param name="description">The description.</param>
            /// <param name="metaInfo1">The meta info1.</param>
            /// <param name="metaInfo2">The meta info2.</param>
            public SubListitem(int ID, string description, object metaInfo1, object metaInfo2)
                : this(ID, description, metaInfo1, metaInfo2, null) { }

            /// <summary>
            /// Initializes a new instance of the <see cref="SubListitem"/> class.
            /// </summary>
            /// <param name="ID">The ID.</param>
            /// <param name="description">The description.</param>
            /// <param name="metaInfo1">The meta info1.</param>
            /// <param name="metaInfo2">The meta info2.</param>
            /// <param name="metaInfo3">The meta info3.</param>
            public SubListitem(int ID, string description, object metaInfo1, object metaInfo2, object metaInfo3)
            {
                TextID = ID.ToString();
                Description = description;
                MetaInfo1 = metaInfo1;
                MetaInfo2 = metaInfo2;
                MetaInfo3 = metaInfo3;
            }

            /// <summary>
            /// Gets or sets the id.
            /// </summary>
            /// <value>The id.</value>
            [XmlIgnore()]
            public int ID
            {
                get { return Sushi.Mediakiwi.Data.Utility.ConvertToInt(TextID); }
                set { TextID = value.ToString(); }
            }

            /// <summary>
            /// Gets or sets the text ID.
            /// </summary>
            /// <value>The text ID.</value>
            [XmlElement("id")]
            public string TextID { get; set; }

            /// <summary>
            /// Gets or sets the description.
            /// </summary>
            /// <value>The description.</value>
            [XmlElement("data")]
            public string Description { get; set; }

            /// <summary>
            /// Gets or sets the meta info1.
            /// </summary>
            /// <value>The meta info1.</value>
            [XmlElement("meta1")]
            public object MetaInfo1 { get; set; }

            /// <summary>
            /// Gets or sets the meta info2.
            /// </summary>
            /// <value>The meta info2.</value>
            [XmlElement("meta2")]
            public object MetaInfo2 { get; set; }

            /// <summary>
            /// Gets or sets the meta info3.
            /// </summary>
            /// <value>The meta info3.</value>
            [XmlElement("meta3")]
            public object MetaInfo3 { get; set; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SubList"/> class.
        /// </summary>
		public SubList()
        {
            m_Items = new SubListitem[0];
        }

        /// <summary>
        /// Gets the serialized.
        /// </summary>
        /// <value>The serialized.</value>
        public string Serialized
        {
            get
            {
                return Sushi.Mediakiwi.Data.Utility.GetSerialized(typeof(SubList), this);
            }
        }

        private List<SubListitem> m_List;

        /// <summary>
        /// Adds the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        public void Add(ISubListitem item)
        {
            InitializeList();
            if (item is SubListitem)
                m_List.Add((SubListitem)item);
            else if (string.IsNullOrEmpty(item.TextID))
                m_List.Add(new SubListitem(item.ID, item.Description));
            else
                m_List.Add(new SubListitem(item.TextID, item.Description));
        }
        public void Add(string id, string description)
        {
            InitializeList();
            m_List.Add(new SubListitem(id, description));
        }
        public void Add(int id, string description)
        {
            InitializeList();
            m_List.Add(new SubListitem(id, description));
        }

        /// <summary>
        /// Gets the id of a particular index. If the index is not present a 0 is returned.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public int GetID(int index)
        {
            if (this.Items == null || (index + 1) > this.Items.Length) return 0;
            return Sushi.Mediakiwi.Data.Utility.ConvertToInt(this.Items[index].ID);
        }

        public int? GetIDNullable(int index)
        {
            if (this.Items == null || (index + 1) > this.Items.Length) return null;
            return Sushi.Mediakiwi.Data.Utility.ConvertToInt(this.Items[index].ID);
        }

        /// <summary>
        /// Gets the array ID.
        /// </summary>
        /// <returns></returns>
        public int[] GetIDArray()
        {
            return GetIDList().ToArray();
        }

        /// <summary>
        /// Gets the ID list.
        /// </summary>
        /// <returns></returns>
        public List<int> GetIDList()
        {
            if (this.Items == null) return new List<int>();

            List<int> list = new List<int>();
            foreach (SubListitem item in this.Items)
                list.Add(item.ID);
            return list;
        }

        /// <summary>
        /// Gets the string identifier.
        /// </summary>
        /// <returns></returns>
        public string[] GetStringID()
        {
            if (this.Items == null) return null;

            List<string> list = new List<string>();
            foreach (SubListitem item in this.Items)
                list.Add(item.TextID);
            return list.ToArray();
        }

        /// <summary>
        /// Gets the id of a particular index. If the index is not present a 0 is returned.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public string GetStringID(int index)
        {
            if (this.Items == null || (index + 1) > this.Items.Length) return null;
            return this.Items[index].TextID;
        }

        /// <summary>
        /// Initializes the list.
        /// </summary>
        private void InitializeList()
        {
            if (m_List != null) return;

            m_List = new List<SubListitem>();
            if (m_Items != null)
            {
                foreach (SubListitem item in m_Items) m_List.Add(item);
            }
        }

        private SubListitem[] m_Items;

        /// <summary>
        /// Gets or sets the items.
        /// </summary>
        /// <value>The items.</value>
        [XmlElement("item")]
        public SubListitem[] Items
        {
            get
            {
                if (m_List != null) return m_List.ToArray();
                return m_Items;
            }
            set { m_Items = value; }
        }
    }
}