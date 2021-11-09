using Sushi.Mediakiwi.Data;
using Sushi.Mediakiwi.UI;
using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace Sushi.Mediakiwi.Framework
{
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
            Text = text;
            Value = value;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Text { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string Value { get; set; }
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
            int contentTypeSelection = Utility.ConvertToInt(ContentTypeSelection);

            switch (contentTypeSelection)
            {
                case (int)ContentType.Binary_Document:
                    {
                        return new ContentInfoItem.Binary_DocumentAttribute(Title,
                            Mandatory == "1",
                            InteractiveHelp);
                    }
                case (int)ContentType.Binary_Image:
                    {
                        return new ContentInfoItem.Binary_ImageAttribute(Title,
                            Mandatory == "1",
                            InteractiveHelp)
                        {
                            CanOnlyAdd = CanOnlyCreate == "1"
                        };
                    }
                case (int)ContentType.Choice_Dropdown:
                    {
                        return new ContentInfoItem.Choice_DropdownAttribute(Title,
                            Collection,
                            Mandatory == "1",
                            AutoPostBack == "1",
                            InteractiveHelp)
                        {
                            IsMultiSelect = ContainsOption(ContentInfoItem.Choice_DropdownAttribute.OPTION_ENABLE_MULTI)
                        };
                    }
                case (int)ContentType.Choice_Checkbox:
                    {
                        return new ContentInfoItem.Choice_CheckboxAttribute(Title,
                            AutoPostBack == "1",
                            InteractiveHelp);
                    }
                case (int)ContentType.Choice_Radio:
                    {
                        return new ContentInfoItem.Choice_RadioAttribute(Title,
                            Collection,
                            Groupname,
                            Mandatory == "1",
                            AutoPostBack == "1",
                            InteractiveHelp);
                    }
                case (int)ContentType.Date:
                    {
                        return new ContentInfoItem.DateAttribute(Title,
                            Mandatory == "1",
                            InteractiveHelp);
                    }
                case (int)ContentType.DateTime:
                    {
                        return new ContentInfoItem.DateTimeAttribute(Title,
                            Mandatory == "1",
                            IsDbSortField == "1",
                            InteractiveHelp);
                    }
                case (int)ContentType.FolderSelect:
                    {
                        return new ContentInfoItem.FolderSelectAttribute(Title,
                            Mandatory == "1",
                            InteractiveHelp);
                    }
                case (int)ContentType.PageSelect:
                    {
                        return new ContentInfoItem.PageSelectAttribute(Title,
                            Mandatory == "1",
                            InteractiveHelp);
                    }
                case (int)ContentType.MultiField:
                    {
                        return new ContentInfoItem.MultiFieldAttribute(Title,
                            InteractiveHelp);
                    }
                case (int)ContentType.RichText:
                    {
                        return new ContentInfoItem.RichTextAttribute(
                            Title,
                            string.IsNullOrEmpty(MaxValueLength) ? 0 : Convert.ToInt32(MaxValueLength),
                            Mandatory == "1",
                            InteractiveHelp)
                        {
                            EnableTable = ContainsOption(ContentInfoItem.RichTextAttribute.OPTION_ENABLE_TABLE)
                        };
                    }
                case (int)ContentType.SubListSelect:
                    {
                        // [MR: 11-06-2021] added these lines, for as a datatype definition saves its GUID
                        // in the Collection property, not the ComponentList property
                        string componentListGuid = Componentlist;
                        if (string.IsNullOrWhiteSpace(Componentlist) && string.IsNullOrWhiteSpace(Collection) == false)
                        {
                            componentListGuid = Collection;
                        }
                        return new ContentInfoItem.SubListSelectAttribute(Title,
                            componentListGuid,
                            Mandatory == "1",
                            CanOnlySortOrder == "1",
                            CanContainOneItem == "1",
                            CanClickOnItem == "1",
                            InteractiveHelp);
                    }
                case (int)ContentType.ListItemSelect:
                    {
                        return new ContentInfoItem.ListItemSelectAttribute(Title,
                            Collection,
                            CanReuseItem == "1",
                            InteractiveHelp);
                    }
                case (int)ContentType.Hyperlink:
                    {
                        return new ContentInfoItem.HyperlinkAttribute(Title,
                            Mandatory == "1",
                            InteractiveHelp);
                    }
                case (int)ContentType.TextArea:
                    {
                        return new ContentInfoItem.TextAreaAttribute(Title,
                            string.IsNullOrEmpty(MaxValueLength) ? 0 : Convert.ToInt32(MaxValueLength),
                            Mandatory == "1",
                            InteractiveHelp,
                            MustMatch);
                    }
                case (int)ContentType.Sourcecode:
                    {
                        return new ContentInfoItem.SourcecodeAttribute(Title,
                            string.IsNullOrEmpty(MaxValueLength) ? 0 : Convert.ToInt32(MaxValueLength),
                            Mandatory == "1",
                            InteractiveHelp,
                            MustMatch);
                    }
                case (int)ContentType.TextField:
                    {
                        return new ContentInfoItem.TextFieldAttribute(Title,
                            string.IsNullOrEmpty(MaxValueLength) ? 0 : Convert.ToInt32(MaxValueLength),
                            Mandatory == "1",
                            InteractiveHelp,
                            MustMatch);
                    }
                case (int)ContentType.TextLine:
                    {
                        return new ContentInfoItem.TextLineAttribute(Title,
                            InteractiveHelp);
                    }
                case (int)ContentType.Section:
                    {
                        return new ContentListItem.SectionAttribute(false,
                            null,
                            Title);
                    }
                default:
                    {
                        return null;
                    }
            }
        }

        /// <summary>
        /// Add a custom option
        /// </summary>
        /// <param name="option"></param>
        public void AddOption(string option)
        {
            List<string> tags = new List<string>();
            if (!string.IsNullOrEmpty(Options))
            {
                tags.AddRange(Options.Split(','));
                if (tags.Contains(option))
                    return;
            }
            tags.Add(option);
            Options = Utility.ConvertToCsvString(tags.ToArray(), false);
        }

        /// <summary>
        /// Remove a custom option
        /// </summary>
        /// <param name="option"></param>
        public void RemoveOption(string option)
        {
            if (string.IsNullOrEmpty(Options))
            {
                return;
            }

            List<string> tags = new List<string>();
            tags.AddRange(Options.Split(','));
            if (tags.Contains(option))
            {
                tags.Remove(option);
                Options = Utility.ConvertToCsvString(tags.ToArray(), false);
            }
        }

        public bool ContainsOption(string option)
        {
            if (string.IsNullOrEmpty(Options))
            {
                return false;
            }

            List<string> tags = new List<string>();
            tags.AddRange(Options.Split(','));
            return tags.Contains(option);
        }

        /// <summary>
        /// 
        /// </summary>
        [XmlElement("id")]
        public string Name { get; set; }


        [XmlElement("options")]
        public string Options { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [XmlElement("title")]
        public string Title { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [XmlElement("type")]
        public string ContentTypeSelection { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [XmlElement("list")]
        public string Componentlist { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [XmlElement("help")]
        public string InteractiveHelp { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [XmlElement("reuse")]
        public string CanReuseItem { get; set; }

        [XmlElement("inherit")]
        public string IsInheritedField { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [XmlElement("sortorder")]
        public string CanOnlySortOrder { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [XmlElement("containone")]
        public string CanContainOneItem { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [XmlElement("CanClickOnItem")]
        public string CanClickOnItem { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [XmlElement("mandatory")]
        public string Mandatory { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [XmlElement("emptyfirst")]
        public string EmptyFirst { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [XmlElement("onlyread")]
        public string OnlyRead { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [XmlElement("length")]
        public string MaxValueLength { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [XmlElement("collection")]
        public string Collection { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [XmlElement("postback")]
        public string AutoPostBack { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [XmlElement("group")]
        public string Groupname { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [XmlElement("default")]
        public string Default { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [XmlElement("dbsort")]
        public string IsDbSortField { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [XmlElement("regex")]
        public string MustMatch { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [XmlElement("items")]
        public MetaDataList[] CollectionList { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [XmlElement("isshared")]
        public string IsSharedField { get; set; }

        [XmlElement("canonlycreate")]
        public string CanOnlyCreate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ListItemCollection GetCollection(Beta.GeneratedCms.Console console)
        {
            ListItemCollection _collection = new ListItemCollection();
            if (CollectionList != null && CollectionList.Length > 0)
            {
                foreach (MetaDataList item in CollectionList)
                {
                    _collection.Add(new ListItem(item.Text, item.Value));
                }
            }
            else if (Utils.IsGuid(Collection))
            {
                var clist = ComponentList.SelectOne(Utils.ConvertToGuid(Collection));
                if (clist?.ID > 0) 
                {
                    _collection = Utils.GetListCollection(console, clist);
                }
            }

            return _collection;
        }
    }
}
