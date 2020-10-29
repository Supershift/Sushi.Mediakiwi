using System;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sushi.Mediakiwi.Framework;

namespace Sushi.Mediakiwi.AppCentre.Data.Implementation
{
    /// <summary>
    /// Represents a Property entity.
    /// </summary>
    public class Property : BaseImplementation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ComponentListInstance"/> class.
        /// </summary>
        public Property()
        {
            //wim.ShowInFullWidthMode = true;
            wim.CanSaveAndAddNew = true;
            wim.OpenInEditMode = true;
            wim.HasSortOrder = true;
            wim.HasSingleItemSortOrder = true;
            wim.SetSortOrder("wim_Properties", "Property_Key", "Property_SortOrder");

            this.ListSave += new ComponentListEventHandler(Property_ListSave);
            this.ListLoad += new ComponentListEventHandler(Property_ListLoad);
            this.ListSearch += new ComponentSearchEventHandler(Property_ListSearch);
            this.ListAction += new ComponentActionEventHandler(Property_ListAction);
            this.ListPreRender += new ComponentListEventHandler(Property_ListPreRender);
        }

        /// <summary>
        /// Handles the ListPreRender event of the Property control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Sushi.Mediakiwi.Framework.ComponentListEventArgs"/> instance containing the event data.</param>
        protected virtual void Property_ListPreRender(object sender, ComponentListEventArgs e)
        {
            foreach (Sushi.Mediakiwi.Data.Property prop in Sushi.Mediakiwi.Data.Property.SelectAll(e.SelectedGroupItemKey))
            {
                if (prop.ID == e.SelectedKey) continue;
                if (prop.FieldName == Implement.FieldName)
                    wim.Notification.AddError("FieldName", "This fieldname is in use!");
                if (prop.Filter == Implement.Filter)
                    wim.Notification.AddError("Filter", "This filter is in use!");
            }
            if (!string.IsNullOrEmpty(Implement.Filter))
            {
                foreach (Sushi.Mediakiwi.Data.Catalog.CatalogColumn col in CurrentSelectedList.Catalog().GetColumns())
                {
                    if (col.Name == Implement.Filter)
                    {
                        Implement.MaxValueLength = col.Length.ToString();
                        if (!col.IsNullable)
                            Implement.Mandatory = "1";
                        break;
                    }
                }
            }
            else
            {
                if (IsPostBack)
                {
                    if (CurrentSelectedList.GetInstance() is Wim.Templates.UI.iGenericLinqList)
                    {
                        if (!Implement.IsPresentProperty && !Implement.OnlyInput)
                        {
                            var catalog = Sushi.Mediakiwi.Data.Catalog.SelectOne(CurrentSelectedList.CatalogID);
                            if (string.IsNullOrEmpty(catalog.ColumnData))
                            {
                                wim.Notification.AddError("This catalog has no assigned Data (XML) column, so this field will not be mapped");
                                wim.Notification.AddError("Filter", "");
                            }
                        }
                    }
                    
                }
            }

            if (
                Implement.TypeID == (int)Sushi.Mediakiwi.Framework.ContentType.ListItemSelect ||
                Implement.TypeID == (int)Sushi.Mediakiwi.Framework.ContentType.Choice_Dropdown ||
                Implement.TypeID == (int)Sushi.Mediakiwi.Framework.ContentType.Choice_Radio
                )
            {
                Sushi.Mediakiwi.Data.IComponentList fieldOptionList = Sushi.Mediakiwi.Data.ComponentList.SelectOne(new Guid("9CA13663-54E0-4701-9658-E2DDDF769F78"));
                wim.AddTab(fieldOptionList, 0);
            }
        }

        protected virtual void Property_ListAction(object sender, ComponentActionEventArgs e)
        {
            int selectedList = Convert.ToInt32(Request.QueryString["groupitem"]);

            Sushi.Mediakiwi.Data.IComponentList list2 = Sushi.Mediakiwi.Data.ComponentList.SelectOne(selectedList);

            Sushi.Mediakiwi.Data.Property.ApplyProperties(list2, wim.Console, null);

            Wim.Utilities.CacheItemManager.FlushIndexOfCacheObjects(string.Concat("Data_Sushi.Mediakiwi.Data.Property.All$List_", list2.ID));
        }

        private bool m_Refresh;
        /// <summary>
        /// Gets or sets the search ID.
        /// </summary>
        /// <value>The search ID.</value>
        [Sushi.Mediakiwi.Framework.ContentListItem.Button("Refresh fields", false)]
        public bool Refresh
        {
            get { return m_Refresh; }
            set { m_Refresh = value; }
        }

        private int m_SearchID;
        /// <summary>
        /// Gets or sets the search ID.
        /// </summary>
        /// <value>The search ID.</value>
        [Sushi.Mediakiwi.Framework.ContentListSearchItem.TextField("ID", 255, false)]
        public int SearchID
        {
            get { return m_SearchID; }
            set { m_SearchID = value; }
        }

        private int m_SearchTypeID;
        /// <summary>
        /// Gets or sets the search type ID.
        /// </summary>
        /// <value>The search type ID.</value>
        [Sushi.Mediakiwi.Framework.ContentListSearchItem.TextField("TypeID", 255, false)]
        public int SearchTypeID
        {
            get { return m_SearchTypeID; }
            set { m_SearchTypeID = value; }
        }

        /// <summary>
        /// Handles the ListSearch event of the Property control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Sushi.Mediakiwi.Framework.ComponentListSearchEventArgs"/> instance containing the event data.</param>
        protected virtual void Property_ListSearch(object sender, ComponentListSearchEventArgs e)
        {
            wim.CurrentList.Option_Search_MaxResultPerPage = 250;

            wim.ListDataColumns.Add("ID", "ID", ListDataColumnType.UniqueIdentifier);
            wim.ListDataColumns.Add("Title", "Title");
            wim.ListDataColumns.Add("Type", "Type");
            
            wim.ListDataColumns.Add("Fieldname", "FieldName"); 
            wim.ListDataColumns.Add("Filter", "Filter");

            wim.ListDataColumns.Add("Short", "IsShort", 40);
            wim.ListDataColumns.Add("Fixed", "IsFixed", 40);
            wim.ListDataColumns.Add("Hidden", "IsHidden", 40);
            wim.ListDataColumns.Add("Mandatory", "IsMandatory", 60);

            if (SearchID > 0)
            {
                //  Custom properties
                //?group=25&groupitem=1&list=40&item=0
                wim.SearchResultItemPassthroughParameter = string.Format("group={0}&groupitem={1}&list={2}&item", wim.Console.Group.Value, wim.Console.GroupItem.Value, wim.Console.Logic);
                wim.ListData = Sushi.Mediakiwi.Data.Property.SelectAll(SearchID, SearchTypeID, false, false, false);
            }
            else
            {
                wim.ListData = Sushi.Mediakiwi.Data.Property.SelectAll(e.SelectedGroupItemKey, 0, false, false, false);

                if (wim.ListData.Count == 0)
                {
                    if (!Sushi.Mediakiwi.Data.Property.HasContentContainer(e.SelectedGroupItemKey))
                    {
                        Sushi.Mediakiwi.Data.IComponentList list2 = Sushi.Mediakiwi.Data.ComponentList.SelectOne(e.SelectedGroupItemKey);
                        Sushi.Mediakiwi.Data.Property.ApplyProperties(list2, wim.Console, null);

                        wim.ListData = Sushi.Mediakiwi.Data.Property.SelectAll(e.SelectedGroupItemKey, 0, false, false, false);

                        if (Sushi.Mediakiwi.Data.Property.HasContentContainer(e.SelectedGroupItemKey))
                            Response.Redirect(Wim.Utility.GetSafeUrl(Request));
                    }
                }
            }

            
        }

        ListItemCollection m_PropertyTypeList;
        /// <summary>
        /// Gets the data columns.
        /// </summary>
        /// <value>The data columns.</value>
        public ListItemCollection PropertyTypeList
        {
            get
            {
                if (m_PropertyTypeList == null)
                {
                    m_PropertyTypeList = new ListItemCollection();
                    m_PropertyTypeList.Add(new ListItem("Default", "0"));
                    m_PropertyTypeList.Add(new ListItem("String", "1"));
                    m_PropertyTypeList.Add(new ListItem("Integer", "2"));
                    m_PropertyTypeList.Add(new ListItem("Decimal", "3"));
                    m_PropertyTypeList.Add(new ListItem("DateTime", "4"));
                }
                return m_PropertyTypeList;
            }
        }

        ListItemCollection m_DataColumns;
        /// <summary>
        /// Gets the data columns.
        /// </summary>
        /// <value>The data columns.</value>
        public ListItemCollection DataColumns
        {
            get
            {
                if (m_DataColumns == null)
                {
                    m_DataColumns = new ListItemCollection();

                    if (CurrentSelectedList != null && CurrentSelectedList.Catalog() != null && !CurrentSelectedList.Catalog().IsNewInstance)
                    {
                        m_DataColumns.Add(new ListItem("", ""));
                        foreach (Sushi.Mediakiwi.Data.Catalog.CatalogColumn col in CurrentSelectedList.Catalog().GetColumns())
                        {
                            if (!IsExistingPropertyFilterName(col.Name) || this.Implement.Filter == col.Name)
                                m_DataColumns.Add(new ListItem(string.Format("{0} ({1})", col.Name, col.Length), col.Name));
                        }
                    }
                    else
                        m_DataColumns.Add(new ListItem("-- no catalog assigned --", ""));
                }
                return m_DataColumns;
            }
        }

        ListItemCollection m_ValidationRules;
        /// <summary>
        /// Gets the validation rules.
        /// </summary>
        /// <value>The validation rules.</value>
        public  ListItemCollection ValidationRules
        {
            get
            {
                if (m_ValidationRules == null)
                {
                    m_ValidationRules = new ListItemCollection();
                    m_ValidationRules.Add(new ListItem("", ""));

                    Sushi.Mediakiwi.Framework.ValidationRules ruleList = Sushi.Mediakiwi.Framework.ValidationRules.Load();
                    foreach (Sushi.Mediakiwi.Framework.ValidationRule rule in ruleList.Rules)
                    {
                        m_ValidationRules.Add(new ListItem(rule.Title, rule.ID.ToString()));
                    }
                }
                return m_ValidationRules;
            }
        }

        Sushi.Mediakiwi.Data.IComponentList m_CurrentSelectedList;
        /// <summary>
        /// Gets the current selected list.
        /// </summary>
        /// <value>The current selected list.</value>
        public Sushi.Mediakiwi.Data.IComponentList CurrentSelectedList
        {
            get {
                if (m_CurrentSelectedList == null)
                {
                    m_CurrentSelectedList = Sushi.Mediakiwi.Data.ComponentList.SelectOne(Wim.Utility.ConvertToInt(Request.QueryString["groupitem"]));
                }

                return m_CurrentSelectedList; }
            set
            {
                m_CurrentSelectedList = value;
            }
        }

        Sushi.Mediakiwi.Data.Property[] m_CurrentProperties;
        Sushi.Mediakiwi.Data.Property[] CurrentProperties
        {
            get
            {
                if (m_CurrentProperties == null)
                {
                    //  See also listSearch!
                    if (SearchID > 0)
                        m_CurrentProperties = Sushi.Mediakiwi.Data.Property.SelectAll(SearchID, SearchTypeID, false, false, false);
                    else
                        m_CurrentProperties = Sushi.Mediakiwi.Data.Property.SelectAll(Wim.Utility.ConvertToInt(Request.QueryString["groupitem"]), 0, false, false, false);
                }
                return m_CurrentProperties;
            }
        }

        /// <summary>
        /// Handles the ListLoad event of the ComponentList control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Sushi.Mediakiwi.Framework.ComponentListEventArgs"/> instance containing the event data.</param>
        protected virtual void Property_ListLoad(object sender, ComponentListEventArgs e)
        {
            if (m_CurrentSelectedList == null)
            {
                //wim.CanAddNewItem = Sushi.Mediakiwi.Data.Property.HasContentContainer(e.SelectedGroupItemKey);
            }

            if (Implement == null)
            {
                Implement = Sushi.Mediakiwi.Data.Property.SelectOne(e.SelectedKey);
            }

            if (!Implement.IsFixed)
                this.ListDelete += new ComponentListEventHandler(Property_ListDelete);

            int selectedType = IsPostBack ? Wim.Utility.ConvertToInt(Request.Params["TypeID"]) : Implement.TypeID;
            
            //  When true SHOW editmode
            m_IsFixedOrRadioDropdownSublist = (!m_Implement.IsFixed && (selectedType == (int)Sushi.Mediakiwi.Framework.ContentType.Choice_Dropdown || selectedType == (int)Sushi.Mediakiwi.Framework.ContentType.Choice_Radio || selectedType == (int)Sushi.Mediakiwi.Framework.ContentType.ListItemSelect));

            IsFixedOrRadioDropdown = (!m_Implement.IsFixed && (selectedType == (int)Sushi.Mediakiwi.Framework.ContentType.Choice_Dropdown || selectedType == (int)Sushi.Mediakiwi.Framework.ContentType.Choice_Radio || selectedType == (int)Sushi.Mediakiwi.Framework.ContentType.SubListSelect || selectedType == (int)Sushi.Mediakiwi.Framework.ContentType.ListItemSelect));

            int selectedList = IsPostBack ? Wim.Utility.ConvertToInt(Request.Params["ListSelect"]) : Implement.ListSelect.GetValueOrDefault();
            int selectedOptionList = IsPostBack ? Wim.Utility.ConvertToInt(Request.Params["OptionListSelect"]) : Implement.OptionListSelect.GetValueOrDefault();

            IsFixedOrRadioDropdownAndNoOptionList = (selectedOptionList == 0 && !m_Implement.IsFixed && (selectedType == (int)Sushi.Mediakiwi.Framework.ContentType.Choice_Dropdown || selectedType == (int)Sushi.Mediakiwi.Framework.ContentType.Choice_Radio || selectedType == (int)Sushi.Mediakiwi.Framework.ContentType.SubListSelect || selectedType == (int)Sushi.Mediakiwi.Framework.ContentType.ListItemSelect));
            IsFixedOrRadioDropdownAndNoListSelect = (selectedList == 0 && !m_Implement.IsFixed && (selectedType == (int)Sushi.Mediakiwi.Framework.ContentType.Choice_Dropdown || selectedType == (int)Sushi.Mediakiwi.Framework.ContentType.Choice_Radio || selectedType == (int)Sushi.Mediakiwi.Framework.ContentType.SubListSelect || selectedType == (int)Sushi.Mediakiwi.Framework.ContentType.ListItemSelect));
            IsFixedOrRadioDropdownAndListSelect = IsFixedOrRadioDropdownAndNoOptionList && selectedList > 0;
            
            m_IsSublistSelect = (!m_Implement.IsFixed && (selectedType == (int)Sushi.Mediakiwi.Framework.ContentType.SubListSelect));
            m_IsDropdown = (!m_Implement.IsFixed && (selectedType == (int)Sushi.Mediakiwi.Framework.ContentType.Choice_Dropdown));
            m_IsPostBackEnabled = (!m_Implement.IsFixed && (selectedType == (int)Sushi.Mediakiwi.Framework.ContentType.Choice_Dropdown || selectedType == (int)Sushi.Mediakiwi.Framework.ContentType.Choice_Radio || selectedType == (int)Sushi.Mediakiwi.Framework.ContentType.Choice_Checkbox || selectedType == (int)Sushi.Mediakiwi.Framework.ContentType.TextField));

            //  When true SHOW editmode
            m_IsFixedOrFreeInput = (!m_Implement.IsFixed && (selectedType == (int)Sushi.Mediakiwi.Framework.ContentType.TextField));

        }

        /// <summary>
        /// Handles the ListDelete event of the ComponentList control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Sushi.Mediakiwi.Framework.ComponentListEventArgs"/> instance containing the event data.</param>
        protected virtual void Property_ListDelete(object sender, ComponentListEventArgs e)
        {
            Sushi.Mediakiwi.Data.PropertyOption.DeleteCollection(Implement.ID);
            Implement.Delete();
        }

        /// <summary>
        /// Handles the ListSave event of the ComponentList control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Sushi.Mediakiwi.Framework.ComponentListEventArgs"/> instance containing the event data.</param>
        protected virtual void Property_ListSave(object sender, ComponentListEventArgs e)
        {
            if (Implement.ListID == 0)
                Implement.ListID = Convert.ToInt32(Request.QueryString["groupitem"]);

            Wim.Utilities.CacheItemManager.FlushIndexOfCacheObjects(string.Concat("Data_Sushi.Mediakiwi.Data.Property.All$List_", Implement.ListID));

            if (!String.IsNullOrEmpty(Implement.Filter))
            {
                foreach (Sushi.Mediakiwi.Data.Catalog.CatalogColumn col in CurrentSelectedList.Catalog().GetColumns())
                {
                    if (col.Name == Implement.Filter)
                    {
                        Implement.FilterType = col.Type.FullName;
                        break;
                    }
                }
            }


            Wim.Utility.ReflectProperty(Implement, Implement.MetaData);
            Implement.Data = Wim.Utility.GetSerialized(Implement.MetaData);

            if (Implement.IsPresentProperty)
            {
                Implement.FieldName = Implement.FieldName2;
                switch(Implement.PropertyType)
                {
                    default: Implement.FilterType = typeof(System.String).ToString(); break;
                    case 2: Implement.FilterType = typeof(int).ToString(); break;
                    case 3: Implement.FilterType = typeof(decimal).ToString(); break;
                    case 4: Implement.FilterType = typeof(DateTime).ToString(); break;
                }
            }

            Implement.Save();

            //if (CurrentSelectedList.GetInstance() is Wim.Templates.UI.iGenericLinqList)
            //{
            //    Sushi.Mediakiwi.Framework.CodeGeneration.GenericLinqList.CreateProxy(CurrentSelectedList.ID);
            //}
            
            //if (Implement.IsFilter)
            //{
            //    //Sushi.Mediakiwi.Framework.Templates.GenericInstance[] instances = Sushi.Mediakiwi.Framework.Templates.GenericInstance.SelectAll(Implement.ListID, wim.CurrentSite.ID, 1000);
            //    //foreach (Sushi.Mediakiwi.Framework.Templates.GenericInstance instance in instances)
            //    //{
            //    //    instance.Save();
            //    //}
            //}
        }

        bool m_IsFixedOrRadioDropdownSublist;
        /// <summary>
        /// Gets a value indicating whether this instance is is fixed or radio dropdown sublist.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is is fixed or radio dropdown sublist; otherwise, <c>false</c>.
        /// </value>
        public bool IsFixedOrRadioDropdownSublist
        {
            get { return m_IsFixedOrRadioDropdownSublist; }
        }

        bool m_IsSublistSelect;
        /// <summary>
        /// Gets a value indicating whether this instance is sublist select.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is sublist select; otherwise, <c>false</c>.
        /// </value>
        public bool IsSublistSelect
        {
            get { return m_IsSublistSelect; }
        }

        bool m_IsDropdown;
        public bool IsDropdown
        {
            get { return m_IsDropdown; }
        }

        bool m_IsPostBackEnabled;
        public bool IsPostBackEnabled
        {
            get { return m_IsPostBackEnabled; }
        }

        public bool IsFixedOrRadioDropdown { get; set; }
        public bool IsFixedOrRadioDropdownAndNoOptionList { get; set; }
        public bool IsFixedOrRadioDropdownAndNoListSelect { get; set; }
        public bool IsFixedOrRadioDropdownAndListSelect { get; set; }

        bool m_IsFixedOrFreeInput;
        /// <summary>
        /// Gets a value indicating whether this instance is is fixed or radio dropdown.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is is fixed or radio dropdown; otherwise, <c>false</c>.
        /// </value>
        public bool IsFixedOrFreeInput
        {
            get { return m_IsFixedOrFreeInput; }
        }

        Sushi.Mediakiwi.Data.Property m_Implement;
        /// <summary>
        /// Gets or sets the implement.
        /// </summary>
        /// <value>The implement.</value>
        [Sushi.Mediakiwi.Framework.ContentListItem.DataExtend()]
        public Sushi.Mediakiwi.Data.Property Implement
        {
            get {
                if (m_Implement == null)
                {
                    m_Implement = Sushi.Mediakiwi.Data.Property.SelectOne(Wim.Utility.ConvertToInt(Request.QueryString["item"]));
                }
                return m_Implement; 
            }
            set { m_Implement = value; }
        }


        /// <summary>
        /// Gets a value indicating whether this instance is filter or type.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is filter or type; otherwise, <c>false</c>.
        /// </value>
        public bool IsNotFilterOrType
        {
            get
            {
                //  Set also in CustomDataItem: ParseSqlParameterValue
                //  Set also in CreateFilter
                //  IsNotFilterOrType
                //if (Implement.IsFilter) return false;

                if (m_CurrentSelectedList.CatalogID == 0) return false;

                int typeID = IsPostBack ? Wim.Utility.ConvertToInt(Request.Form["TypeID"]) : Implement.TypeID;

                if (typeID == (int)Sushi.Mediakiwi.Framework.ContentType.Date) return true;
                if (typeID == (int)Sushi.Mediakiwi.Framework.ContentType.DateTime) return true;
                if (typeID == (int)Sushi.Mediakiwi.Framework.ContentType.Choice_Checkbox) return true;
                if (typeID == (int)Sushi.Mediakiwi.Framework.ContentType.Binary_Document) return true;
                if (typeID == (int)Sushi.Mediakiwi.Framework.ContentType.Binary_Image) return true;
                if (typeID == (int)Sushi.Mediakiwi.Framework.ContentType.Choice_Dropdown) return true;
                if (typeID == (int)Sushi.Mediakiwi.Framework.ContentType.Choice_Radio) return true;
                if (typeID == (int)Sushi.Mediakiwi.Framework.ContentType.FolderSelect) return true;
                if (typeID == (int)Sushi.Mediakiwi.Framework.ContentType.Hyperlink) return true;
                if (typeID == (int)Sushi.Mediakiwi.Framework.ContentType.PageSelect) return true;
                if (typeID == (int)Sushi.Mediakiwi.Framework.ContentType.RichText) return true;
                if (typeID == (int)Sushi.Mediakiwi.Framework.ContentType.TextArea) return true;
                if (typeID == (int)Sushi.Mediakiwi.Framework.ContentType.TextField) return true;

                return false;
            }
        }

        /// <summary>
        /// Gets or sets the list.
        /// </summary>
        /// <value>The list.</value>
        [Sushi.Mediakiwi.Framework.ContentListItem.DataList("6A7D5E6C-9DAA-4A6F-AEEA-21BD4782DA1E")]
        public Sushi.Mediakiwi.Data.DataList List { get; set; }

        ListItemCollection m_Lists;
        /// <summary>
        /// Gets the lists.
        /// </summary>
        /// <value>The lists.</value>
        public ListItemCollection Lists
        {
            get
            {
                if (m_Lists == null)
                {
                    m_Lists = new ListItemCollection();
                    m_Lists.Add(new ListItem(""));

                    var sorted = (from item in Sushi.Mediakiwi.Data.ComponentList.SelectAll() orderby item.Name select item).ToArray();
                    foreach (var tmp in sorted)
                    {
                        m_Lists.Add(new ListItem(tmp.Name, tmp.ID.ToString()));
                    }
                }
                return m_Lists;
            }
        }

        ListItemCollection m_OptionLists;
        /// <summary>
        /// Gets the lists.
        /// </summary>
        /// <value>The lists.</value>
        public ListItemCollection OptionLists
        {
            get
            {
                if (m_OptionLists == null)
                {
                    m_OptionLists = new ListItemCollection();
                    m_OptionLists.Add(new ListItem(""));
                    Sushi.Mediakiwi.Data.NameValue[] items = Sushi.Mediakiwi.AppCentre.Data.Implementation.Support.NameValues.SelectAll_FormElementOptionList();
                    foreach (Sushi.Mediakiwi.Data.NameValue item in items)
                    {
                        m_OptionLists.Add(new ListItem(item.Name, item.Value));
                    }
                }
                return m_OptionLists;
            }
        }

        bool IsValidProperty(System.Reflection.PropertyInfo info)
        {
            if (info.PropertyType == typeof(string) ||
                info.PropertyType == typeof(int) ||
                info.PropertyType == typeof(int?) ||
                info.PropertyType == typeof(decimal) ||
                info.PropertyType == typeof(decimal?) ||
                info.PropertyType == typeof(DateTime) ||
                info.PropertyType == typeof(DateTime?) ||
                info.PropertyType == typeof(bool) ||
                info.PropertyType == typeof(bool?)
                )
                return true;
            return false;
        }

        bool IsExistingProperty(string name)
        {
            foreach (Sushi.Mediakiwi.Data.Property p in CurrentProperties)
            {
                if (p.FieldName == name)
                {
                    return true;
                }
            }
            return false;
        }

        bool IsExistingPropertyFilterName(string name)
        {
            foreach (Sushi.Mediakiwi.Data.Property p in CurrentProperties)
            {
                if (p.Filter == name)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Gets a value indicating whether this instance can have entity property.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can have entity property; otherwise, <c>false</c>.
        /// </value>
        public bool CanHaveEntityProperty
        {
            get
            {
                return this.EntityProperties.Count > 0;
            }
        }

        ListItemCollection m_EntityProperties;
        /// <summary>
        /// Gets the entity properties.
        /// </summary>
        /// <value>The entity properties.</value>
        public ListItemCollection EntityProperties
        {
            get
            {
                m_EntityProperties = new ListItemCollection();
                
                Type candidate = null; 
                Object instance = null;
                if (CurrentSelectedList.ClassName == "Sushi.Mediakiwi.AppCentre.Data.Implementation.Identity.ProfileList")
                {
                    if (Wim.CommonConfiguration.HasCodeGeneration)
                    {
                        string className = string.Concat(Wim.CommonConfiguration.CODEGENERATION_NAMESPACE, ".Identity.RegisteredProfile");
                        instance = Wim.Utility.CreateInstance(Wim.CommonConfiguration.CODEGENERATION_ASSEMBLY, className, out candidate, false);
                    }
                }
                else if (CurrentSelectedList.ClassName == "Wim.Templates.Templates.UI.GenericsList" || CurrentSelectedList.ClassName == "Wim.Templates.Templates.UI.SimpleGenericsList")
                {
                    if (Wim.CommonConfiguration.HasCodeGeneration)
                    {
                        string groupName = string.IsNullOrEmpty(CurrentSelectedList.Group) ? null : string.Concat(CurrentSelectedList.Group.Replace(" ", "_"), ".");
                        string className = string.Concat(Wim.CommonConfiguration.CODEGENERATION_NAMESPACE, ".Generics.", groupName, CurrentSelectedList.Class);

                        instance = Wim.Utility.CreateInstance(Wim.CommonConfiguration.CODEGENERATION_ASSEMBLY, className, out candidate, false);
                    }
                }
                else if (CurrentSelectedList.GetInstance() is Wim.Templates.UI.iGenericLinqList)
                {
                    if (Wim.CommonConfiguration.HasCodeGeneration)
                    {
                        string groupName = string.IsNullOrEmpty(CurrentSelectedList.Group) ? null : string.Concat(CurrentSelectedList.Group.Replace(" ", "_"), ".");
                        string className = string.Concat(Wim.CommonConfiguration.CODEGENERATION_NAMESPACE, ".Generics.", groupName, CurrentSelectedList.Class);
                        instance = Wim.Utility.CreateInstance(Wim.CommonConfiguration.CODEGENERATION_ASSEMBLY, className, out candidate, false);
                    }
                }
                if (instance != null)
                {
                    System.Reflection.PropertyInfo[] infoList = candidate.GetProperties();
                    foreach (System.Reflection.PropertyInfo info in infoList)
                    {
                        if (!IsValidProperty(info)) continue;
                        if (!IsExistingProperty(info.Name) || this.Implement.FieldName == info.Name)
                            m_EntityProperties.Add(new ListItem(info.Name));
                    }

                }
                return m_EntityProperties;
            }
        }

        
        ListItem[] GetSortedListCollection(int listID)
        {
            SortedList<string, ListItem> sorted = new SortedList<string, ListItem>();

            Sushi.Mediakiwi.Data.IComponentList list = Sushi.Mediakiwi.Data.ComponentList.SelectOne(listID);
            System.Type type = null;
            object instance = null;
            if (list.ClassName == "Wim.Templates.Templates.UI.GenericsList")
            {
                string groupName = string.IsNullOrEmpty(list.Group) ? null : string.Concat(list.Group.Replace(" ", "_"), ".");
                string className = string.Concat(Wim.CommonConfiguration.CODEGENERATION_NAMESPACE, ".Generics.", groupName, "UI.", list.Class, "List");
                instance = Wim.Utility.CreateInstance(Wim.CommonConfiguration.CODEGENERATION_ASSEMBLY, className, out type, false);
            }

            if (instance == null)
                instance = Wim.Utility.CreateInstance(list.AssemblyName, list.ClassName, out type, false);

            if (instance == null) return new ListItem[0];

            foreach (System.Reflection.PropertyInfo info in type.GetProperties())
            {
                if (info.PropertyType == typeof(ListItemCollection))
                {
                    ExposedListCollection[] attribs = info.GetCustomAttributes(typeof(ExposedListCollection), false) as ExposedListCollection[];
                    if (attribs != null && attribs.Length == 1)
                    {
                        if (string.IsNullOrEmpty(attribs[0].CollectionReferencingMethod))
                        {
                            ListItem li = new ListItem(attribs[0].Description, info.Name);
                            sorted.Add(li.Text, li);
                        }
                        else
                        {
                            ListItemCollection collection = info.GetValue(instance, null) as ListItemCollection;
                            foreach (ListItem item in collection)
                            {
                                ListItem li = new ListItem(item.Text, string.Concat(attribs[0].CollectionReferencingMethod, ":", item.Value));
                                sorted.Add(li.Text, li);
                            }
                        }
                    }
                }
            }
            List<ListItem> lilist = new List<ListItem>();
            foreach (ListItem li in sorted.Values)
                lilist.Add(li);
            return lilist.ToArray();
        }

        ListItemCollection m_ListCollections;
        /// <summary>
        /// Gets the list collections.
        /// </summary>
        /// <value>The list collections.</value>
        public ListItemCollection ListCollections
        {
            get
            {
                m_ListCollections = new ListItemCollection();
                m_ListCollections.Add(new ListItem(""));

                if (Implement == null) return m_ListCollections;

                int listID = IsPostBack ? Wim.Utility.ConvertToInt(Request.Form["ListSelect"]) : Implement.ListSelect.GetValueOrDefault();
                if (listID > 0)
                {
                    foreach (ListItem li in GetSortedListCollection(listID))
                        m_ListCollections.Add(li);
                }

                return m_ListCollections;
            }
        }
    }
}
