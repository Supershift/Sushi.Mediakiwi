using System;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Sushi.Mediakiwi.Data;
using Sushi.Mediakiwi.UI;

namespace Sushi.Mediakiwi.Framework
{
    ///// <summary>
    ///// 
    ///// </summary>
    //public interface iMetaDataContainer
    //{
    //    /// <summary>
    //    /// Gets the inner control collection.
    //    /// </summary>
    //    /// <returns></returns>
    //    ControlCollection GetInnerControlCollection();
    //}

    /// <summary>
    /// 
    /// </summary>
    public class MetaDataList
    {
        /// <summary>
        /// 
        /// </summary>
        public MetaDataList() { }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="value"></param>
        public MetaDataList(string text, string value)
        {
            this.Text = text;
            this.Value = value;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Text;
        /// <summary>
        /// 
        /// </summary>
        public string Value;
    }

    /// <summary>
    /// 
    /// </summary>
    public class MetaData
    {
        public MetaData()
        {
        }
        public MetaData(string name, ContentType type, string title, bool isRequired = false)
        {
            Name = name;
            Title = title;
            ContentTypeSelection = ((int)type).ToString();
            Mandatory = isRequired ? "1" : "0";
        }

        /// <summary>
        /// Gets the content info.
        /// </summary>
        /// <returns></returns>
        public IContentInfo GetContentInfo()
        {
            int ContentTypeSelection = Data.Utility.ConvertToInt(this.ContentTypeSelection);

            if (ContentTypeSelection == (int)ContentType.Binary_Document)
                return new ContentInfoItem.Binary_DocumentAttribute(Title, Mandatory == "1", InteractiveHelp);

            if (ContentTypeSelection == (int)ContentType.Binary_Image)
                return new ContentInfoItem.Binary_ImageAttribute(Title, Mandatory == "1", InteractiveHelp) { CanOnlyAdd = true };

            if (ContentTypeSelection == (int)ContentType.Choice_Dropdown)
                return new ContentInfoItem.Choice_DropdownAttribute(Title, Collection, Mandatory == "1", AutoPostBack == "1", InteractiveHelp)
                { IsMultiSelect = this.ContainsOption(ContentInfoItem.Choice_DropdownAttribute.OPTION_ENABLE_MULTI) };

            if (ContentTypeSelection == (int)ContentType.Choice_Checkbox)
                return new ContentInfoItem.Choice_CheckboxAttribute(Title, AutoPostBack == "1", InteractiveHelp);

            if (ContentTypeSelection == (int)ContentType.Choice_Radio)
                return new ContentInfoItem.Choice_RadioAttribute(Title, Collection, Groupname, Mandatory == "1", AutoPostBack == "1", InteractiveHelp);

            if (ContentTypeSelection == (int)ContentType.Date)
                return new ContentInfoItem.DateAttribute(Title, Mandatory == "1", InteractiveHelp);

            if (ContentTypeSelection == (int)ContentType.DateTime)
                return new ContentInfoItem.DateTimeAttribute(Title, Mandatory == "1", IsDbSortField == "1", InteractiveHelp);

            if (ContentTypeSelection == (int)ContentType.FolderSelect)
                return new ContentInfoItem.FolderSelectAttribute(Title, Mandatory == "1", InteractiveHelp);

            if (ContentTypeSelection == (int)ContentType.PageSelect)
                return new ContentInfoItem.PageSelectAttribute(Title, Mandatory == "1", InteractiveHelp);

            if (ContentTypeSelection == (int)ContentType.MultiField)
                return new ContentInfoItem.MultiFieldAttribute(Title, InteractiveHelp);

            if (ContentTypeSelection == (int)ContentType.RichText)
            {
                return new ContentInfoItem.RichTextAttribute(Title
                    , string.IsNullOrEmpty(MaxValueLength) ? 0 : Convert.ToInt32(MaxValueLength), Mandatory == "1", InteractiveHelp)
                { EnableTable = this.ContainsOption(ContentInfoItem.RichTextAttribute.OPTION_ENABLE_TABLE) };
            }

            if (ContentTypeSelection == (int)ContentType.SubListSelect)
                return new ContentInfoItem.SubListSelectAttribute(Title, Componentlist, Mandatory == "1", CanOnlySortOrder == "1",  CanContainOneItem == "1", CanClickOnItem=="1", InteractiveHelp);

            //if (ContentTypeSelection == (int)ContentType.MultiImageSelect)
            //    return new ContentInfoItem.MultiImageSelectAttribute(Title, InteractiveHelp);

            if (ContentTypeSelection == (int)ContentType.ListItemSelect)
                return new ContentInfoItem.ListItemSelectAttribute(Title, AvailableCollectionProperty, CanReuseItem == "1", InteractiveHelp);

            if (ContentTypeSelection == (int)ContentType.Hyperlink)
                return new ContentInfoItem.HyperlinkAttribute(Title, Mandatory == "1", InteractiveHelp);

            if (ContentTypeSelection == (int)ContentType.TextArea)
                return new ContentInfoItem.TextAreaAttribute(Title, string.IsNullOrEmpty(MaxValueLength) ? 0 : Convert.ToInt32(MaxValueLength), Mandatory == "1", InteractiveHelp, this.MustMatch);

            if (ContentTypeSelection == (int)ContentType.Sourcecode)
                return new ContentInfoItem.SourcecodeAttribute(Title, string.IsNullOrEmpty(MaxValueLength) ? 0 : Convert.ToInt32(MaxValueLength), Mandatory == "1", InteractiveHelp, this.MustMatch);

            if (ContentTypeSelection == (int)ContentType.TextField)
            {
                //if (!string.IsNullOrEmpty(this.MustMatchID))
                //    this.MustMatch = Sushi.Mediakiwi.Framework.ValidationRule.SelectOne(Convert.ToInt32(this.MustMatchID)).RegEx;

                return new ContentInfoItem.TextFieldAttribute(Title, string.IsNullOrEmpty(MaxValueLength) ? 0 : Convert.ToInt32(MaxValueLength), Mandatory == "1", InteractiveHelp, this.MustMatch);
            }

            if (ContentTypeSelection == (int)ContentType.TextLine)
                return new ContentInfoItem.TextLineAttribute(Title, InteractiveHelp);

            if (ContentTypeSelection == (int)ContentType.Section)
                return new ContentListItem.SectionAttribute(false, null, Title);
            return null;
        }

        private string m_Name;
        /// <summary>
        /// 
        /// </summary>
        [XmlElement("id")]
        public string Name
        {
            set { m_Name = value; }
            get { return m_Name; }
        }

        /// <summary>
        /// Add a custom option
        /// </summary>
        /// <param name="option"></param>
        public void AddOption(string option)
        {
            List<string> tags = new List<string>();
            if (!string.IsNullOrEmpty(this.Options))
            {
                tags.AddRange(this.Options.Split(','));
                if (tags.Contains(option))
                    return;
            }
            tags.Add(option);
            this.Options = Data.Utility.ConvertToCsvString(tags.ToArray(), false);
        }

        /// <summary>
        /// Remove a custom option
        /// </summary>
        /// <param name="option"></param>
        public void RemoveOption(string option)
        {
            if (string.IsNullOrEmpty(this.Options))
                return;

            List<string> tags = new List<string>();
            tags.AddRange(this.Options.Split(','));
            if (tags.Contains(option))
            {
                tags.Remove(option);
                this.Options = Data.Utility.ConvertToCsvString(tags.ToArray(), false);
            }
        }

        public bool ContainsOption(string option)
        {
            if (string.IsNullOrEmpty(this.Options))
                return false;

            List<string> tags = new List<string>();
            tags.AddRange(this.Options.Split(','));
            return tags.Contains(option);
        }


        [XmlElement("options")]
        public string Options { get; set; }


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

        [XmlElement("inherit")]
        public string IsInheritedField { get;set; }

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


        private string m_Mandatory;
        /// <summary>
        /// 
        /// </summary>
        [XmlElement("mandatory")]
        public string Mandatory
        {
            set { m_Mandatory = value; }
            get { return m_Mandatory; }
        }

        private string m_EmptyFirst;
        /// <summary>
        /// 
        /// </summary>
        [XmlElement("emptyfirst")]
        public string EmptyFirst
        {
            set { m_EmptyFirst = value; }
            get { return m_EmptyFirst; }
        }

        private string m_OnlyRead;
        /// <summary>
        /// 
        /// </summary>
        [XmlElement("onlyread")]
        public string OnlyRead
        {
            set { m_OnlyRead = value; }
            get { return m_OnlyRead; }
        }

        private string m_MaxValueLength;
        /// <summary>
        /// 
        /// </summary>
        [XmlElement("length")]
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

        //private string m_MustMatchID;
        ///// <summary>
        ///// Gets or sets the must match ID.
        ///// </summary>
        ///// <value>The must match ID.</value>
        //[XmlElement("regex_id")]
        //public string MustMatchID
        //{
        //    set { m_MustMatchID = value; }
        //    get { return m_MustMatchID; }
        //}

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
        public Sushi.Mediakiwi.UI.ListItemCollection GetCollection()
        {
            Sushi.Mediakiwi.UI.ListItemCollection Collection = new Sushi.Mediakiwi.UI.ListItemCollection();
            if (CollectionList != null && CollectionList.Length > 0)
            {
                foreach (MetaDataList item in CollectionList)
                    Collection.Add(new ListItem(item.Text, item.Value));
            }
            return Collection;
        }
    }
}
