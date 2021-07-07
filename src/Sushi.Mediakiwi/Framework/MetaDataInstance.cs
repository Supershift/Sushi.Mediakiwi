using Sushi.Mediakiwi.UI;
using System.Xml;
using System.Xml.Serialization;

namespace Sushi.Mediakiwi.Framework
{
    /// <summary>
    /// 
    /// </summary>
    public class MetaDataInstance
    {
        private int m_Index;
        /// <summary>
        /// Gets or sets the index.
        /// </summary>
        /// <value>The index.</value>
        public int Index
        {
            set { m_Index = value; }
            get { return m_Index; }
        }

        private string m_Name;
        /// <summary>
        /// 
        /// </summary>
        [ContentListItem.TextField("Tag", 50)]
        public string Name
        {
            set { m_Name = value; }
            get { return m_Name; }
        }

        private string m_AvailableCollectionProperty;
        /// <summary>
        /// 
        /// </summary>
        [XmlElement("collectionprop")]
        public string AvailableCollectionProperty
        {
            set { m_AvailableCollectionProperty = value; }
            get { return m_AvailableCollectionProperty; }
        }

        private string m_Title;
        /// <summary>
        /// 
        /// </summary>
        [XmlElement("title")]
        [ContentListItem.TextField("Title", 255)]
        public string Title
        {
            set { m_Title = value; }
            get { return m_Title; }
        }

        private string m_contenttype;
        /// <summary>
        /// 
        /// </summary>
        [XmlElement("type")]
        [ContentListItem.Choice_Dropdown("Type", "ContentTypeSelectionList", true)]
        public string ContentTypeSelection
        {
            set { m_contenttype = value; }
            get { return m_contenttype; }
        }

        private string m_Componentlist;
        /// <summary>
        /// 
        /// </summary>
        [XmlElement("list")]
        public string Componentlist
        {
            set { m_Componentlist = value; }
            get { return m_Componentlist; }
        }

        private string m_InteractiveHelp;
        /// <summary>
        /// 
        /// </summary>
        [XmlElement("help")]
        public string InteractiveHelp
        {
            set { m_InteractiveHelp = value; }
            get { return m_InteractiveHelp; }
        }

        private string m_CanReuseItem;
        /// <summary>
        /// 
        /// </summary>
        [XmlElement("reuse")]
        public string CanReuseItem
        {
            set { m_CanReuseItem = value; }
            get { return m_CanReuseItem; }
        }

        private string m_CanOnlySortOrder;
        /// <summary>
        /// 
        /// </summary>
        [XmlElement("sortorder")]
        public string CanOnlySortOrder
        {
            set { m_CanOnlySortOrder = value; }
            get { return m_CanOnlySortOrder; }
        }

        private string m_CanClickOnItem;
        /// <summary>
        /// 
        /// </summary>
        [XmlElement("CanClickOnItem")]
        public string CanClickOnItem
        {
            set { m_CanClickOnItem = value; }
            get { return m_CanClickOnItem; }
        }

        private string m_CanContainOneItem;
        /// <summary>
        /// 
        /// </summary>
        [XmlElement("containone")]
        public string CanContainOneItem
        {
            set { m_CanContainOneItem = value; }
            get { return m_CanContainOneItem; }
        }

        private string m_Mandatory;
        /// <summary>
        /// 
        /// </summary>
        [ContentListItem.Choice_Checkbox("Mandatory")]
        public string Mandatory
        {
            set { m_Mandatory = value; }
            get { return m_Mandatory; }
        }

        private string m_MaxValueLength;
        /// <summary>
        /// 
        /// </summary>
        [XmlElement("length")]
        [ContentListItem.TextField("Maximum length", 10)]
        public string MaxValueLength
        {
            set { m_MaxValueLength = value; }
            get { return m_MaxValueLength; }
        }

        private string m_Collection;
        /// <summary>
        /// 
        /// </summary>
        [XmlElement("collection")]
        public string Collection
        {
            set { m_Collection = value; }
            get { return m_Collection; }
        }

        private string m_AutoPostBack;
        /// <summary>
        /// 
        /// </summary>
        [XmlElement("postback")]
        public string AutoPostBack
        {
            set { m_AutoPostBack = value; }
            get { return m_AutoPostBack; }
        }

        private string m_Groupname;
        /// <summary>
        /// 
        /// </summary>
        [XmlElement("group")]
        public string Groupname
        {
            set { m_Groupname = value; }
            get { return m_Groupname; }
        }

        private string m_Text;
        /// <summary>
        /// 
        /// </summary>
        [XmlElement("default")]
        public string Default
        {
            set { m_Text = value; }
            get { return m_Text; }
        }

        private string m_IsDbSortField;
        /// <summary>
        /// 
        /// </summary>
        [XmlElement("dbsort")]
        public string IsDbSortField
        {
            set { m_IsDbSortField = value; }
            get { return m_IsDbSortField; }
        }

        private string m_MustMatch;
        /// <summary>
        /// 
        /// </summary>
        [XmlElement("regex")]
        public string MustMatch
        {
            set { m_MustMatch = value; }
            get { return m_MustMatch; }
        }

        private MetaDataList[] m_CollectionList;
        /// <summary>
        /// 
        /// </summary>
        [XmlElement("items")]
        public MetaDataList[] CollectionList
        {
            set { m_CollectionList = value; }
            get { return m_CollectionList; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ListItemCollection GetCollection()
        {
            ListItemCollection Collection = new ListItemCollection();
            if (CollectionList != null && CollectionList.Length > 0)
            {
                foreach (MetaDataList item in CollectionList)
                    Collection.Add(new ListItem(item.Text, item.Value));
            }
            return Collection;
        }
    }
}
