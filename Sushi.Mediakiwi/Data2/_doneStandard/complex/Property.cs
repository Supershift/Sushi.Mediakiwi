using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Sushi.Mediakiwi.Data.DalReflection;
using System.Web.UI.WebControls;

namespace Sushi.Mediakiwi.Data
{
    #region REPLICATED TO Sushi.Mediakiwi.Data.Standard
    /// <summary>
    /// 
    /// </summary>
    public class PropertyFilter 
    {
        public bool IsSqlColumn { get; set; }
        public string PropertyName { get; set; }
        public string PropertyValue { get; set; }
        public SqlDbType PropertyType { get; set; }
    }

    #endregion REPLICATED TO Sushi.Mediakiwi.Data.Standard

    /// <summary>
    /// 
    /// </summary>
    [DatabaseTable("wim_Properties", Order = "Property_SortOrder ASC")]
    public class Property : DatabaseEntity, iExportable
    {
        /// <summary>
        /// Gets the type.
        /// </summary>
        /// <value>The type.</value>
        [System.Xml.Serialization.XmlIgnore()]
        public string Type
        {
            get
            {
                System.Web.UI.WebControls.ListItem li = TypeCollection.FindByValue(TypeID.ToString());
                if (li == null) return null;
                return li.Text;
            }
        }

        Sushi.Mediakiwi.Framework.MetaData m_MetaData;
        /// <summary>
        /// Gets or sets the met data.
        /// </summary>
        /// <value>The met data.</value>
        [System.Xml.Serialization.XmlIgnore()]
        public Sushi.Mediakiwi.Framework.MetaData MetaData
        {
            get
            {
                //if (m_MetaData == null && !string.IsNullOrEmpty(m_Data))
                //    m_MetaData = Wim.Utility.GetDeserialized(typeof(Sushi.Mediakiwi.Framework.MetaData), m_Data) as Sushi.Mediakiwi.Framework.MetaData;

                return m_MetaData;
            }
            set { m_MetaData = value; }
        }

        /// <summary>
        /// Applies the properties.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="console">The console.</param>
        /// <param name="listTypeID">The list type ID.</param>
        public static void ApplyProperties(IComponentList list, Beta.GeneratedCms.Console console, int? listTypeID)
        {
            ApplyProperties(list, console, listTypeID, false);
        }

        /// <summary>
        /// Applies the properties.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="console">The console.</param>
        /// <param name="listTypeID">The list type ID.</param>
        /// <param name="takeFromBaseProperties">When the listTypeID is applied this fills it with its base (parent) properties</param>
        public static void ApplyProperties(Sushi.Mediakiwi.Data.IComponentList list, Beta.GeneratedCms.Console console, int? listTypeID, bool takeFromBaseProperties)
        {
            ApplyProperties(list, console, listTypeID, takeFromBaseProperties, list.AssemblyName, list.ClassName);
        }

        public static void ApplyProperties(Sushi.Mediakiwi.Data.IComponentList list, Beta.GeneratedCms.Console console, int? listTypeID, bool takeFromBaseProperties, string customAssembly, string customClassName)
        {
            ParseProperties parser = new ParseProperties(list, console, listTypeID, takeFromBaseProperties);
            parser.Start(customAssembly, customClassName);
        }

        class ParseProperties
        {
            Sushi.Mediakiwi.Data.IComponentList m_list;
            Beta.GeneratedCms.Console m_Console;
            int? m_ListTypeID;
            bool m_takeFromBaseProperties;

            public ParseProperties(Sushi.Mediakiwi.Data.IComponentList list, Beta.GeneratedCms.Console console, int? listTypeID, bool takeFromBaseProperties)
            {
                m_list = list;
                m_Console = console;
                m_ListTypeID = listTypeID;
                m_takeFromBaseProperties = takeFromBaseProperties;
            }

            public void Start(string assembly, string className)
            {
                //if (m_takeFromBaseProperties)
                //{
                //    Sushi.Mediakiwi.Data.Property[] p = Sushi.Mediakiwi.Data.Property.SelectAll(m_list.ID);
                //}
                object instance = Wim.Utility.CreateInstance(assembly, className);
                Cycle(instance, false);
            }

            void Cycle(object instance, bool isExtention)
            {
                System.Reflection.PropertyInfo[] properties = instance.GetType().GetProperties();

                foreach (System.Reflection.PropertyInfo info in properties)
                {
                    if (!isExtention && !m_takeFromBaseProperties && info.DeclaringType != instance.GetType()) continue;

                    string name = info.Name;

                    bool isEditable = false;
                    bool isVisible;
                    Sushi.Mediakiwi.Framework.IContentInfo contentAttribute = GetContentInfo(info, m_Console, instance, ref isEditable, out isVisible);

                    if (contentAttribute != null
                        && m_list.Type == Sushi.Mediakiwi.Data.ComponentListType.Undefined
                        )
                    {
                        if (contentAttribute.ContentTypeSelection == Sushi.Mediakiwi.Framework.ContentType.DataExtend)
                        {
                            object sender = info.GetValue(instance, null);

                            if (sender == null)
                            {
                                if (info.PropertyType == typeof(Wim.Templates.IGeneric))
                                    sender = System.Activator.CreateInstance(typeof(Sushi.Mediakiwi.Framework.Templates.GenericInstance));
                                else if (info.PropertyType == typeof(Wim.Templates.ISimpleGenerics))
                                    sender = System.Activator.CreateInstance(typeof(Sushi.Mediakiwi.Framework.Templates.SimpleGenericsInstance));
                                else
                                    sender = System.Activator.CreateInstance(info.PropertyType);
                            }

                            Cycle(sender, true);
                        }

                        if (contentAttribute.ContentTypeSelection != Sushi.Mediakiwi.Framework.ContentType.DataExtend
                              && contentAttribute.ContentTypeSelection != Sushi.Mediakiwi.Framework.ContentType.DataList
                              )
                        {
                            Sushi.Mediakiwi.Data.Property prop = Sushi.Mediakiwi.Data.Property.SelectOne(m_list.ID, name, m_ListTypeID);
                            prop.MetaData = contentAttribute.GetMetaData(name);

                            if (prop.IsNewInstance)
                            {
                                Wim.Utility.ReflectProperty(prop.MetaData, prop);
                                if (prop.MetaData != null)
                                {
                                    prop.MetaData.Title = prop.Title;
                                    prop.MetaData.InteractiveHelp = prop.InteractiveHelp;
                                }
                            }

                            prop.FieldName = name;
                            prop.ListID = m_Console.CurrentListInstance.wim.CurrentList.ID;
                            prop.IsFixed = true;


                            prop.Data = Wim.Utility.GetSerialized(prop.MetaData);
                            prop.TypeID = (int)contentAttribute.ContentTypeSelection;
                            prop.ListID = m_list.ID;
                            prop.ListTypeID = m_ListTypeID;

                            if (prop.ListID > 0)
                            {
                                prop.Save();
                            }

                            if (contentAttribute.ContentTypeSelection != Sushi.Mediakiwi.Framework.ContentType.ContentContainer)
                            {
                                if (m_ListTypeID.HasValue && m_ListTypeID.Value > 0)
                                {
                                    Sushi.Mediakiwi.Data.Property[] inners = Sushi.Mediakiwi.Data.Property.SelectAll(m_list.ID, null, false, true);
                                    foreach (Sushi.Mediakiwi.Data.Property inner in inners)
                                    {
                                        prop = Sushi.Mediakiwi.Data.Property.SelectOne(m_list.ID, inner.FieldName, m_ListTypeID);

                                        if (prop.IsNewInstance)
                                        {
                                            Wim.Utility.ReflectProperty(inner, prop);
                                            prop.ID = 0;
                                        }
                                        prop.IsFixed = true;
                                        prop.InheritedID = inner.ID;
                                        prop.ListTypeID = m_ListTypeID;
                                        prop.Filter = inner.Filter;
                                        prop.FilterType = inner.FilterType;

                                        if (prop.ListID > 0)
                                        {
                                            prop.Save();
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            Sushi.Mediakiwi.Framework.IContentInfo GetContentInfo(System.Reflection.PropertyInfo info, Sushi.Mediakiwi.Beta.GeneratedCms.Console container, object senderInstance, ref bool isEditable, out bool isVisible)
            {
                isVisible = true;

                Sushi.Mediakiwi.Framework.IContentInfo contentAttribute = null;
                foreach (object attribute in info.GetCustomAttributes(false))
                {
                    if (container.View == 0 && attribute is Framework.IListContentInfo)
                    {
                        contentAttribute = attribute as Sushi.Mediakiwi.Framework.IContentInfo;
                    }
                    else if ((container.View == 1 || container.View == 2) && attribute is Framework.IListSearchContentInfo)
                    {
                        contentAttribute = attribute as Sushi.Mediakiwi.Framework.IContentInfo;
                    }

                    if (attribute is Sushi.Mediakiwi.Framework.OnlyEditableWhenTrue)
                    {
                        if (isEditable)
                        {
                            Framework.OnlyEditableWhenTrue editable = (Framework.OnlyEditableWhenTrue)attribute;
                            bool check = (Boolean)GetProperty(container, senderInstance, editable.Property);

                            if (check && !editable.State) isEditable = false;
                            if (!check && editable.State) isEditable = false;
                        }
                    }
                    if (attribute is Sushi.Mediakiwi.Framework.OnlyVisibleWhenTrue)
                    {
                        Framework.OnlyVisibleWhenTrue visible = attribute as Framework.OnlyVisibleWhenTrue;
                        
                        object pvalue = GetProperty(container, senderInstance, visible.Property);
                        bool check = pvalue == null ? true : (Boolean)pvalue;

                        if (check && !visible.State) isVisible = false;
                        if (!check && visible.State) isVisible = false;
                    }
                }
                return contentAttribute;
            }

            object GetProperty(Sushi.Mediakiwi.Beta.GeneratedCms.Console container, object sender, string property)
            {
                if (sender is Sushi.Mediakiwi.Framework.MetaData)
                {
                    Sushi.Mediakiwi.Framework.MetaData item = ((Sushi.Mediakiwi.Framework.MetaData)sender);
                    return item.GetCollection();
                }

                foreach (System.Reflection.PropertyInfo info in sender.GetType().GetProperties())
                {   //  Get all public properties
                    if (info.CanRead)
                    {   // Get all writable public properties
                        if (info.Name == property)
                        {
                            return info.GetValue(sender, null);
                        }
                    }
                }

                //  Fall back
                if (container.CurrentListInstance != sender)
                {
                    foreach (System.Reflection.PropertyInfo info in container.CurrentListInstance.GetType().GetProperties())
                    {   //  Get all public properties
                        if (info.CanRead)
                        {   // Get all writable public properties
                            if (info.Name == property)
                            {
                                return info.GetValue(container.CurrentListInstance, null);
                            }
                        }
                    }
                }


                return null;
                //throw new Exception(string.Format("Could not find the '{0}' property.", property));
            }

        }

        /// <summary>
        /// Converts the property value.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="value">The value.</param>
        /// <param name="listID">The list ID.</param>
        /// <param name="listTypeID">The list type ID.</param>
        /// <returns></returns>
        public static object ConvertPropertyValue(string propertyName, object value, int listID, int? listTypeID)
        {
            if (string.IsNullOrEmpty(propertyName))
                return null;

            if (value != null && value.GetType() == typeof(string) && value.ToString() != string.Empty)
            {
                //  [20090410:MM Trying to detect the type]
                foreach (Sushi.Mediakiwi.Data.Property p in Sushi.Mediakiwi.Data.Property.SelectAll(listID, listTypeID, false))
                {
                    if (p.FieldName == propertyName.Replace("Data.", ""))
                    {
                        if (p.FilterType == typeof(DateTime).ToString())
                            return new DateTime(long.Parse(value.ToString()));
                        if (p.FilterType == typeof(decimal).ToString())
                            return decimal.Parse(value.ToString());
                        if (p.FilterType == typeof(int).ToString())
                            return Utility.ConvertToInt(value);

                        if (p.TypeID == (int)Sushi.Mediakiwi.Framework.ContentType.Choice_Checkbox)
                        {
                            if (value.ToString() == "0" || value.ToString() == string.Empty) return false;
                            return true;
                        }
                    }
                }
            }
            else
            {
                if (value == null)
                {
                    foreach (Sushi.Mediakiwi.Data.Property p in Sushi.Mediakiwi.Data.Property.SelectAll(listID))
                    {
                        if (p.FieldName == propertyName.Replace("Data.", ""))
                        {
                            if (p.TypeID == (int)Sushi.Mediakiwi.Framework.ContentType.Choice_Checkbox)
                            {
                                return false;
                            }
                        }
                    }
                }
            }
            return value;
        }


        /// <summary>
        /// Selects all.
        /// </summary>
        /// <param name="listID">The list ID.</param>
        /// <returns></returns>
        public static Property[] SelectAll(int listID)
        {
            return SelectAll(listID, 0, false);
        }

        /// <summary>
        /// Selects the all_ import export.
        /// </summary>
        /// <param name="portal">The portal.</param>
        /// <returns></returns>
        public static List<Property> SelectAll_ImportExport(string portal)
        {
            Property implement = new Property();
            List<Property> list = new List<Property>();

            implement.SqlConnectionString = Sushi.Mediakiwi.Data.Common.GetPortal(portal).Connection;
            implement.ConnectionType = Sushi.Mediakiwi.Data.Common.GetPortal(portal).Type;

            foreach (object o in implement._SelectAll(null, false, "PropertyImportExport", portal))
                list.Add((Property)o);

            return list;
        }

        /// <summary>
        /// Selects the all_ import export.
        /// </summary>
        /// <param name="portal">The portal.</param>
        /// <param name="componentlistID">The componentlist ID.</param>
        /// <returns></returns>
        public static List<Property> SelectAll_ImportExport(string portal, int componentlistID)
        {
            List<DatabaseDataValueColumn> whereClause = new List<DatabaseDataValueColumn>();
            whereClause.Add(new DatabaseDataValueColumn("Property_List_Key", SqlDbType.Int, componentlistID));

            Property implement = new Property();
            List<Property> list = new List<Property>();

            implement.SqlConnectionString = Sushi.Mediakiwi.Data.Common.GetPortal(portal).Connection;
            implement.ConnectionType = Sushi.Mediakiwi.Data.Common.GetPortal(portal).Type;

            foreach (object o in implement._SelectAll(whereClause))
                list.Add((Property)o);

            return list;
        }

        /// <summary>
        /// Selects all.
        /// </summary>
        /// <param name="listID">The list ID.</param>
        /// <param name="listTypeID">The list type ID.</param>
        /// <param name="showEmptyTypes">if set to <c>true</c> [show empty types].</param>
        /// <returns></returns>
        public static Property[] SelectAll(int listID, int? listTypeID, bool showEmptyTypes)
        {
            return SelectAll(listID, listTypeID, showEmptyTypes, false);
        }


        /// <summary>
        /// Selects all.
        /// </summary>
        /// <param name="listID">The list ID.</param>
        /// <param name="listTypeID">The list type ID.</param>
        /// <param name="showEmptyTypes">if set to <c>true</c> [show empty types].</param>
        /// <param name="onlyReturnFlexibleProperties">if set to <c>true</c> [only return flexible properties].</param>
        /// <returns></returns>
        public static Property[] SelectAll(int listID, int? listTypeID, bool showEmptyTypes, bool onlyReturnFlexibleProperties)
        {
            return SelectAll(listID, listTypeID, showEmptyTypes, onlyReturnFlexibleProperties, true);
        }

        


        /// <summary>
        /// Selects all.
        /// </summary>
        /// <param name="listID">The list ID.</param>
        /// <param name="listTypeID">The list type ID.</param>
        /// <param name="showEmptyTypes">if set to <c>true</c> [show empty types].</param>
        /// <param name="onlyReturnFlexibleProperties">if set to <c>true</c> [only return flexible properties].</param>
        /// <param name="cacheresult">if set to <c>true</c> [cacheresult].</param>
        /// <returns></returns>
        public static Property[] SelectAll(int listID, int? listTypeID, bool showEmptyTypes, bool onlyReturnFlexibleProperties, bool cacheresult)
        {
            return SelectAll(listID, listTypeID, showEmptyTypes, onlyReturnFlexibleProperties, cacheresult, null);
        }

        /// <summary>
        /// Selects all.
        /// </summary>
        /// <param name="listID">The list ID.</param>
        /// <param name="listTypeID">The list type ID.</param>
        /// <param name="showEmptyTypes">if set to <c>true</c> [show empty types].</param>
        /// <param name="onlyReturnFlexibleProperties">if set to <c>true</c> [only return flexible properties].</param>
        /// <param name="cacheresult">if set to <c>true</c> [cacheresult].</param>
        /// <returns></returns>
        internal static Property[] SelectAll(int listID, int? listTypeID, bool showEmptyTypes, bool onlyReturnFlexibleProperties, bool cacheresult, Sushi.Mediakiwi.Framework.WimServerPortal DatabaseMappingPortal)
        {
            Property candidate = new Property();
            //  For Command Line Memory Allocation!
            #region Command Line Memory Allocation!
            if (System.Web.HttpContext.Current == null && cacheresult)
            {
                if (MemoryAllocationList == null)
                    MemoryAllocationList = new List<MemoryItemProperty>();

                foreach (MemoryItemProperty item in MemoryAllocationList)
                {
                    if (item.ListID == listID && item.ListTypeID == listTypeID && item.ShowEmptyTypes == showEmptyTypes && item.OnlyReturnFlexibleProperties == onlyReturnFlexibleProperties)
                        return item.Properties;
                }
            }
            #endregion

            List<Property> list = new List<Property>();

            List<DatabaseDataValueColumn> where = new List<DatabaseDataValueColumn>();
            where.Add(new DatabaseDataValueColumn("Property_List_Key", SqlDbType.Int, listID));

            if (listTypeID.HasValue && listTypeID.Value > 0)
            {
                candidate.SqlOrder = "Property_List_Type_Key ASC, Property_SortOrder ASC";

                if (showEmptyTypes)
                {
                    where.Add(new DatabaseDataValueColumn("Property_List_Type_Key", SqlDbType.Int, null));
                    where.Add(new DatabaseDataValueColumn("Property_List_Type_Key", SqlDbType.Int, listTypeID, DatabaseDataValueConnectType.Or));
                }
                else
                    where.Add(new DatabaseDataValueColumn("Property_List_Type_Key", SqlDbType.Int, listTypeID));

            }
            else
                where.Add(new DatabaseDataValueColumn("Property_List_Type_Key", SqlDbType.Int, null));

            where.Add(new DatabaseDataValueColumn("NOT Property_Type", SqlDbType.Int, 35));

            if (onlyReturnFlexibleProperties)
                where.Add(new DatabaseDataValueColumn("Property_IsFixed", SqlDbType.Bit, false));

            string cachevalue = string.Concat("List_", listID, "Type_", listTypeID.GetValueOrDefault(), "Empty_", showEmptyTypes ? "1" : "0", "Flex_", onlyReturnFlexibleProperties ? "1" : "0");

            object[] list2;

            if (DatabaseMappingPortal != null)
                candidate.SqlConnectionString = DatabaseMappingPortal.Connection;

            if (cacheresult)
                list2 = candidate._SelectAll(where, false, "All", cachevalue);
            else
                list2 = candidate._SelectAll(where);

            //SqlConnectionString2 = null;

            foreach (object o in list2)
            {
                Property t = (Property)o;
                t.LoadData();
                list.Add(t);
            }

            //  For Command Line Memory Allocation!
            #region Command Line Memory Allocation!
            if (MemoryAllocationList != null && cacheresult)
                MemoryAllocationList.Add(new MemoryItemProperty(listID, listTypeID, showEmptyTypes, onlyReturnFlexibleProperties, list.ToArray()));
            #endregion

            return list.ToArray();
        }

        /// <summary>
        /// Selects all having database column.
        /// </summary>
        /// <param name="listID">The list ID.</param>
        /// <param name="listTypeID">The list type ID.</param>
        /// <param name="cacheresult">if set to <c>true</c> [cacheresult].</param>
        /// <param name="DatabaseMappingPortal">The database mapping portal.</param>
        /// <returns></returns>
        public static Property[] SelectAll_HavingDatabaseColumn(int listID, int? listTypeID, bool cacheresult, Sushi.Mediakiwi.Framework.WimServerPortal DatabaseMappingPortal)
        {
            Property candidate = new Property();

            List<Property> list = new List<Property>();

            List<DatabaseDataValueColumn> where = new List<DatabaseDataValueColumn>();
            where.Add(new DatabaseDataValueColumn("Property_List_Key", SqlDbType.Int, listID));
            where.Add(new DatabaseDataValueColumn("NOT Property_Column is null"));

            //  20110712 ADDED
            if (listTypeID.HasValue && listTypeID.Value > 0)
            {
                where.Add(new DatabaseDataValueColumn("Property_List_Type_Key", SqlDbType.Int, null));
                where.Add(new DatabaseDataValueColumn("Property_List_Type_Key", SqlDbType.Int, listTypeID, DatabaseDataValueConnectType.Or));
                //where.Add(new DatabaseDataValueColumn("Property_List_Type_Key", SqlDbType.Int, listTypeID));

            }

            string cachevalue = string.Concat("List_", listID, "Type_", listTypeID.GetValueOrDefault(), "Columns");

            object[] list2;

            if (DatabaseMappingPortal != null)
                candidate.SqlConnectionString = DatabaseMappingPortal.Connection;

            if (cacheresult)
                list2 = candidate._SelectAll(where, false, "All", cachevalue);
            else
                list2 = candidate._SelectAll(where);

            foreach (object o in list2)
            {
                Property t = (Property)o;
                t.LoadData();
                list.Add(t);
            }
            return list.ToArray();
        }

        System.Web.UI.WebControls.ListItemCollection m_TypeCollection;
        /// <summary>
        /// Gets the type collection.
        /// </summary>
        /// <value>The type collection.</value>
        [System.Xml.Serialization.XmlIgnore()]
        public System.Web.UI.WebControls.ListItemCollection TypeCollection
        {
            get
            {
                if (m_TypeCollection == null)
                {
                    m_TypeCollection = new ListItemCollection();
                    m_TypeCollection.Add(new ListItem(""));
                    m_TypeCollection.Add(new ListItem("Textfield", ((int)Sushi.Mediakiwi.Framework.ContentType.TextField).ToString()));
                    m_TypeCollection.Add(new ListItem("Textarea", ((int)Sushi.Mediakiwi.Framework.ContentType.TextArea).ToString()));
                    m_TypeCollection.Add(new ListItem("Richtext", ((int)Sushi.Mediakiwi.Framework.ContentType.RichText).ToString()));
                    m_TypeCollection.Add(new ListItem("Textline", ((int)Sushi.Mediakiwi.Framework.ContentType.TextLine).ToString()));
                    m_TypeCollection.Add(new ListItem("Section", ((int)Sushi.Mediakiwi.Framework.ContentType.Section).ToString()));
                    m_TypeCollection.Add(new ListItem("Document", ((int)Sushi.Mediakiwi.Framework.ContentType.Binary_Document).ToString()));
                    m_TypeCollection.Add(new ListItem("Image", ((int)Sushi.Mediakiwi.Framework.ContentType.Binary_Image).ToString()));
                    m_TypeCollection.Add(new ListItem("Checkbox", ((int)Sushi.Mediakiwi.Framework.ContentType.Choice_Checkbox).ToString()));
                    m_TypeCollection.Add(new ListItem("Dropdown", ((int)Sushi.Mediakiwi.Framework.ContentType.Choice_Dropdown).ToString()));
                    m_TypeCollection.Add(new ListItem("Radio", ((int)Sushi.Mediakiwi.Framework.ContentType.Choice_Radio).ToString()));
                    m_TypeCollection.Add(new ListItem("Date", ((int)Sushi.Mediakiwi.Framework.ContentType.Date).ToString()));
                    m_TypeCollection.Add(new ListItem("Datetime", ((int)Sushi.Mediakiwi.Framework.ContentType.DateTime).ToString()));
                    m_TypeCollection.Add(new ListItem("Folder select", ((int)Sushi.Mediakiwi.Framework.ContentType.FolderSelect).ToString()));
                    m_TypeCollection.Add(new ListItem("Hyperlink", ((int)Sushi.Mediakiwi.Framework.ContentType.Hyperlink).ToString()));
                    m_TypeCollection.Add(new ListItem("Listitem select", ((int)Sushi.Mediakiwi.Framework.ContentType.ListItemSelect).ToString()));
                    m_TypeCollection.Add(new ListItem("Page select", ((int)Sushi.Mediakiwi.Framework.ContentType.PageSelect).ToString()));
                    m_TypeCollection.Add(new ListItem("Sublist select", ((int)Sushi.Mediakiwi.Framework.ContentType.SubListSelect).ToString()));
                }
                return m_TypeCollection;
            }
        }

      

        /// <summary>
        /// Selects the one.
        /// </summary>
        /// <param name="ID">The ID.</param>
        /// <returns></returns>
        public static Property SelectOne(int ID)
        {
            Property t = new Property();
            t.SqlConnectionString = SqlConnectionString2;
            t = (Property)t._SelectOne(ID);
            t.LoadData();
            SqlConnectionString2 = null;
            return t;
        }

        /// <summary>
        /// Selects the one.
        /// </summary>
        /// <param name="listID">The list ID.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="listTypeID">The list type ID.</param>
        /// <returns></returns>
        public static Property SelectOne(int listID, string fieldName, int? listTypeID)
        {
            List<DatabaseDataValueColumn> where = new List<DatabaseDataValueColumn>();
            where.Add(new DatabaseDataValueColumn("Property_List_Key", SqlDbType.Int, listID));
            where.Add(new DatabaseDataValueColumn("Property_List_Type_Key", SqlDbType.Int, listTypeID));
            where.Add(new DatabaseDataValueColumn("Property_FieldName", SqlDbType.VarChar, fieldName));
            //where.Add(new DatabaseDataValueColumn("Property_IsFixed", SqlDbType.Bit, true));
            Property t = (Property)new Property()._SelectOne(where);
            t.LoadData();

            return t;
        }


        #region MOVED to EXTENSION / LOGIC

        ///// <summary>
        ///// 
        ///// </summary>
        //internal ListItemCollection m_ListItemCollection;
        ///// <summary>
        ///// Lists the select options.
        ///// </summary>
        ///// <param name="currentSite">The current site.</param>
        ///// <returns></returns>
        //public ListItemCollection ListSelectOptions(Sushi.Mediakiwi.Data.Site currentSite)
        //{
        //    m_ListItemCollection = null;
        //    if (m_ListItemCollection == null)
        //    {
        //        if (ListSelect.HasValue)
        //        {
        //            Sushi.Mediakiwi.Data.IComponentList list = Sushi.Mediakiwi.Data.ComponentList.SelectOne(ListSelect.Value);
        //            m_ListItemCollection = Wim.Utility.GetInstanceListCollection(list, this.ListCollection, currentSite, list.GetInstance());
        //        }
        //        else if (OptionListSelect.HasValue)
        //        {
        //            Sushi.Mediakiwi.Framework.iOption options = Wim.Utility.GetInstanceOptions("Wim.Module.FormGenerator.dll", "Wim.Module.FormGenerator.Data.FormElementOptionList");
        //            if (options != null)
        //            {
        //                m_ListItemCollection = new System.Web.UI.WebControls.ListItemCollection();
        //                foreach (Sushi.Mediakiwi.Framework.iNameValue nv in options.Options(OptionListSelect.Value))
        //                {
        //                    m_ListItemCollection.Add(new System.Web.UI.WebControls.ListItem(nv.Name, nv.Value));
        //                }
        //            }
        //        }
        //        else
        //        {
        //            m_ListItemCollection = new ListItemCollection();
        //            foreach (PropertyOption option in this.Options)
        //                m_ListItemCollection.Add(new ListItem(option.Name, option.Value.ToString()));
        //        }
        //    }
        //    return m_ListItemCollection;
        //}


        ////System.Reflection.PropertyInfo info, Sushi.Mediakiwi.Beta.GeneratedCms.Console container, object senderInstance, ref bool isEditable, out bool isVisible
        //public Sushi.Mediakiwi.Framework.IContentInfo GetContentInfo()
        //{
        //    Sushi.Mediakiwi.Framework.IContentInfo contentAttribute = null;
        //    switch (this.TypeID)
        //    {
        //        default:
        //        case (int)Sushi.Mediakiwi.Framework.ContentType.Binary_Document:
        //        case (int)Sushi.Mediakiwi.Framework.ContentType.Binary_Image:
        //        case (int)Sushi.Mediakiwi.Framework.ContentType.Choice_Dropdown:
        //        case (int)Sushi.Mediakiwi.Framework.ContentType.Choice_Radio:
        //        case (int)Sushi.Mediakiwi.Framework.ContentType.FolderSelect:
        //        case (int)Sushi.Mediakiwi.Framework.ContentType.PageSelect:
        //        case (int)Sushi.Mediakiwi.Framework.ContentType.Choice_Checkbox:
        //        case (int)Sushi.Mediakiwi.Framework.ContentType.DateTime:
        //        case (int)Sushi.Mediakiwi.Framework.ContentType.Date:
        //            break;
        //        case (int)Sushi.Mediakiwi.Framework.ContentType.TextLine:
        //            return new Sushi.Mediakiwi.Framework.ContentInfoItem.TextLineAttribute(this.Title);
        //        case (int)Sushi.Mediakiwi.Framework.ContentType.TextField:
        //            return new Sushi.Mediakiwi.Framework.ContentInfoItem.TextFieldAttribute(this.Title, Wim.Utility.ConvertToInt(this.MaxValueLength), this.IsMandatory, this.InteractiveHelp);
        //        case (int)Sushi.Mediakiwi.Framework.ContentType.TextArea:
        //            return new Sushi.Mediakiwi.Framework.ContentInfoItem.TextAreaAttribute(this.Title, Wim.Utility.ConvertToInt(this.MaxValueLength), this.IsMandatory, this.InteractiveHelp);
        //        case (int)Sushi.Mediakiwi.Framework.ContentType.RichText:
        //            return new Sushi.Mediakiwi.Framework.ContentInfoItem.RichTextAttribute(this.Title, Wim.Utility.ConvertToInt(this.MaxValueLength), this.IsMandatory, this.InteractiveHelp);
        //    }
        //    return contentAttribute;
        //}

        ///// <summary>
        ///// Applies the filter.
        ///// </summary>
        ///// <param name="filter">The filter.</param>
        ///// <param name="customData">The custom data.</param>
        ///// <param name="itemID">The item ID.</param>
        //public void ApplyFilter(IDataFilter filter, Sushi.Mediakiwi.Data.CustomData customData, int itemID)
        //{
        //    switch (this.TypeID)
        //    {
        //        default:
        //            return;

        //        case (int)Sushi.Mediakiwi.Framework.ContentType.Binary_Document:
        //        case (int)Sushi.Mediakiwi.Framework.ContentType.Binary_Image:
        //        case (int)Sushi.Mediakiwi.Framework.ContentType.Choice_Dropdown:
        //        case (int)Sushi.Mediakiwi.Framework.ContentType.Choice_Radio:
        //        case (int)Sushi.Mediakiwi.Framework.ContentType.FolderSelect:
        //        case (int)Sushi.Mediakiwi.Framework.ContentType.PageSelect:
        //            filter.FilterI = customData[this.FieldName].ParseInt();

        //            if (!filter.FilterI.HasValue && !filter.IsNewInstance)
        //            {
        //                filter.Delete();
        //                return;
        //            }

        //            break;

        //        case (int)Sushi.Mediakiwi.Framework.ContentType.Choice_Checkbox:
        //            filter.FilterB = customData[this.FieldName].ParseBoolean();
        //            break;

        //        case (int)Sushi.Mediakiwi.Framework.ContentType.DateTime:
        //        case (int)Sushi.Mediakiwi.Framework.ContentType.Date:
        //            filter.FilterT = customData[this.FieldName].ParseDateTime();

        //            if (!filter.FilterT.HasValue && !filter.IsNewInstance)
        //            {
        //                filter.Delete();
        //                return;
        //            }

        //            break;

        //        case (int)Sushi.Mediakiwi.Framework.ContentType.TextLine:
        //        case (int)Sushi.Mediakiwi.Framework.ContentType.TextField:
        //        case (int)Sushi.Mediakiwi.Framework.ContentType.TextArea:
        //        case (int)Sushi.Mediakiwi.Framework.ContentType.RichText:

        //            //if (prop.ContentType == "Decimal")
        //            //{
        //            //    filter.FilterD = m_Generic.Data[prop.FieldName].ParseDecimal();
        //            //}
        //            //else
        //            //{
        //            string tmp = customData[this.FieldName].Value;

        //            if (!string.IsNullOrEmpty(tmp) && tmp.Length > 255)
        //                tmp = tmp.Substring(0, 255);

        //            filter.FilterC = tmp;
        //            //}

        //            if (string.IsNullOrEmpty(filter.FilterC) && !filter.IsNewInstance)
        //            {
        //                filter.Delete();
        //                return;
        //            }

        //            break;
        //    }

        //    filter.PropertyID = this.ID;
        //    filter.ItemID = itemID;
        //    filter.Save();
        //}
        ///// <summary>
        ///// Creates the filter.
        ///// </summary>
        //void CreateFilter()
        //{
        //    Sushi.Mediakiwi.Data.IComponentList list = Sushi.Mediakiwi.Data.ComponentList.SelectOne(this.ListID);
        //    try
        //    {
        //        this.Execute(string.Format("select top 1 {0}_{1} from {2}", list.Catalog().ColumnPrefix, this.FieldName, list.Catalog().Table));
        //    }
        //    catch (Exception)
        //    {
        //        //  Set also in CustomDataItem: ParseSqlParameterValue
        //        //  Set also in CreateFilter
        //        //  IsNotFilterOrType

        //        string type = null;
        //        switch ((Sushi.Mediakiwi.Framework.ContentType)this.TypeID)
        //        {

        //            case Sushi.Mediakiwi.Framework.ContentType.Date:
        //            case Sushi.Mediakiwi.Framework.ContentType.DateTime:
        //                type = "DateTime null";
        //                break;
        //            case Sushi.Mediakiwi.Framework.ContentType.Choice_Checkbox:
        //                type = "bit null";
        //                break;
        //            case Sushi.Mediakiwi.Framework.ContentType.Binary_Image:
        //            case Sushi.Mediakiwi.Framework.ContentType.Choice_Radio:
        //            case Sushi.Mediakiwi.Framework.ContentType.FolderSelect:
        //            case Sushi.Mediakiwi.Framework.ContentType.PageSelect:
        //            case Sushi.Mediakiwi.Framework.ContentType.Binary_Document:
        //            case Sushi.Mediakiwi.Framework.ContentType.Hyperlink:
        //            case Sushi.Mediakiwi.Framework.ContentType.Choice_Dropdown:
        //                type = "int null";
        //                break;
        //            case Sushi.Mediakiwi.Framework.ContentType.TextArea:
        //            case Sushi.Mediakiwi.Framework.ContentType.RichText:
        //            case Sushi.Mediakiwi.Framework.ContentType.TextField:
        //                int value = Wim.Utility.ConvertToInt(this.MaxValueLength);
        //                if (value > 0 && value < 4000)
        //                    type = string.Format("nvarchar({0}) null", value);
        //                else
        //                    type = "ntext null";
        //                break;
        //            default: return;
        //        }
        //        if (list.Catalog() != null)
        //            this.Execute(string.Format("alter table {0} add {1}_{2} {3}", list.Catalog().Table, list.Catalog().ColumnPrefix, this.FieldName, type));
        //    }
        //}

        ///// <summary>
        ///// Loads the data.
        ///// </summary>
        //void LoadData()
        //{
        //    if (this.MetaData == null)
        //    {
        //        this.MetaData = Wim.Utility.GetDeserialized(typeof(Sushi.Mediakiwi.Framework.MetaData), this.Data) as Sushi.Mediakiwi.Framework.MetaData;

        //        if (this.MetaData != null)
        //            Wim.Utility.ReflectProperty(this.MetaData, this);
        //    }

        //    if (this.MetaData == null)
        //        this.MetaData = new Sushi.Mediakiwi.Framework.MetaData();
        //}


        #endregion MOVED to EXTENSION / LOGIC

        #region REPLICATED TO Sushi.Mediakiwi.Data.Standard

        /// <summary>
        /// Selects all.
        /// </summary>
        /// <param name="ID">The ID.</param>
        /// <returns></returns>
        public static Property[] SelectAll(int[] ID)
        {
            List<Property> list = new List<Property>();

            List<DatabaseDataValueColumn> where = new List<DatabaseDataValueColumn>();
            where.Add(new DatabaseDataValueColumn("Property_Key", SqlDbType.Int, Wim.Utility.ConvertToCsvString(ID), DatabaseDataValueCompareType.In));

            foreach (object o in new Property()._SelectAll(where))
            {
                Property t = (Property)o;
                t.LoadData();
                list.Add(t);
            }

            return list.ToArray();
        }

        /// <summary>
        /// Save a database entity. This method will indentify the need for an INSERT or an UPDATE.
        /// </summary>
        /// <returns></returns>
        public override bool Save()
        {
            bool shouldSetSortorder = (this.ID == 0);
            bool save = base.Save();

            if (shouldSetSortorder)
                new Property().Execute(string.Concat(@"
update wim_Properties set Property_SortOrder = Property_Key where Property_Key = ", this.ID));

            //if (IsFilter)
            //{
            //    CreateFilter();
            //}

            return save;
        }

        int m_ID;
        /// <summary>
        /// Gets or sets the ID.
        /// </summary>
        /// <value>The ID.</value>
        [DatabaseColumn("Property_Key", SqlDbType.Int, IsPrimaryKey = true)]
        public int ID
        {
            get { return m_ID; }
            set { m_ID = value; }
        }

        /// <summary>
        /// Clones me.
        /// </summary>
        /// <returns></returns>
        public Property CloneMe()
        {
            return this.MemberwiseClone() as Property;
        }

        private Guid m_GUID;
        /// <summary>
        /// Unique identifier used for import/export
        /// </summary>
        /// <value>The GUID.</value>
        [DatabaseColumn("Property_GUID", SqlDbType.UniqueIdentifier, IsMigrationKey = true)]
        public Guid GUID
        {
            get
            {
                if (this.m_GUID == Guid.Empty) this.m_GUID = Guid.NewGuid();
                return m_GUID;
            }
            set { m_GUID = value; }
        }

        int m_ListID;
        /// <summary>
        /// Gets or sets the list ID.
        /// </summary>
        /// <value>The list ID.</value>
        [DatabaseColumn("Property_List_Key", SqlDbType.Int)]
        public int ListID
        {
            get { return m_ListID; }
            set { m_ListID = value; }
        }

        int? m_ListTypeID;
        /// <summary>
        /// Gets or sets the list type ID.
        /// </summary>
        /// <value>The list type ID.</value>
        [DatabaseColumn("Property_List_Type_Key", SqlDbType.Int, IsNullable = true)]
        public int? ListTypeID
        {
            get { return m_ListTypeID; }
            set { m_ListTypeID = value; }
        }

        internal System.Reflection.PropertyInfo Info;

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>The title.</value>
        [Sushi.Mediakiwi.Framework.ContentListItem.TextField("Title", 255, false, Expression = Sushi.Mediakiwi.Framework.OutputExpression.Alternating)]
        [DatabaseColumn("Property_Title", SqlDbType.NVarChar, Length = 255, IsNullable = true)]
        public string Title { get; set; }


        /// <summary>
        /// Gets or sets a value indicating whether this instance is present property.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is present property; otherwise, <c>false</c>.
        /// </value>
        [Sushi.Mediakiwi.Framework.OnlyEditableWhenTrue("CanHaveEntityProperty")]
        [Sushi.Mediakiwi.Framework.ContentListItem.Choice_Checkbox("Defined property", true, Expression = Sushi.Mediakiwi.Framework.OutputExpression.Right)]
        [DatabaseColumn("Property_IsPresent", SqlDbType.Bit, IsNullable = true)]
        public bool IsPresentProperty { get; set; }




        [Sushi.Mediakiwi.Framework.OnlyVisibleWhenTrue("IsPresentProperty", false)]
        [Sushi.Mediakiwi.Framework.ContentListItem.TextField("Property", 35, true, Expression = Sushi.Mediakiwi.Framework.OutputExpression.Alternating)]
        [DatabaseColumn("Property_FieldName", SqlDbType.VarChar, Length = 35)]
        public string FieldName { get; set; }

        [Sushi.Mediakiwi.Framework.OnlyVisibleWhenTrue("IsPresentProperty")]
        [Sushi.Mediakiwi.Framework.ContentListItem.Choice_Dropdown("Property (fixed)", "EntityProperties", Expression = Sushi.Mediakiwi.Framework.OutputExpression.Alternating)]
        public string FieldName2 { get; set; }

        [Sushi.Mediakiwi.Framework.OnlyEditableWhenTrue("IsFixedOrFreeInput")]
        [Sushi.Mediakiwi.Framework.ContentListItem.TextField("Maximum length", 10, false, Expression = Sushi.Mediakiwi.Framework.OutputExpression.Alternating)]
        public string MaxValueLength { get; set; }

        [Sushi.Mediakiwi.Framework.ContentListItem.Choice_Dropdown("Type", "TypeCollection", true, true, Expression = Sushi.Mediakiwi.Framework.OutputExpression.Alternating)]
        [DatabaseColumn("Property_Type", SqlDbType.Int)]
        public int TypeID { get; set; }

        [Sushi.Mediakiwi.Framework.OnlyEditableWhenTrue("IsFixed", false)]
        [Sushi.Mediakiwi.Framework.ContentListItem.Choice_Checkbox("Is mandatory", Expression = Sushi.Mediakiwi.Framework.OutputExpression.Alternating)]
        public string Mandatory { get; set; }


        private string m_FilterType;
        /// <summary>
        /// Gets or sets the type of the filter.
        /// </summary>
        /// <value>The type of the filter.</value>
        [DatabaseColumn("Property_ColumnType", SqlDbType.VarChar, Length = 15, IsNullable = true)]
        public string FilterType
        {
            get { return m_FilterType; }
            set { m_FilterType = value; }
        }

        //int? m_ValidationRuleID;
        ///// <summary>
        ///// Gets or sets the type ID.
        ///// </summary>
        ///// <value>The type ID.</value>
        ////[DatabaseColumn("Property_ValidationRule", SqlDbType.Int, IsNullable = true)]
        //public int? ValidationRuleID
        //{
        //    get { return m_ValidationRuleID; }
        //    set { m_ValidationRuleID = value; }
        //}

        private string m_MustMatchID;
        /// <summary>
        /// Gets or sets the must match ID.
        /// </summary>
        /// <value>The must match ID.</value>
        [Sushi.Mediakiwi.Framework.OnlyEditableWhenTrue("IsFixedOrFreeInput")]
        [Sushi.Mediakiwi.Framework.ContentListItem.Choice_Dropdown("Validatie", "ValidationRules", false, false, Expression = Sushi.Mediakiwi.Framework.OutputExpression.Alternating)]
        public string MustMatchID
        {
            set { m_MustMatchID = value; }
            get { return m_MustMatchID; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is mandatory.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is mandatory; otherwise, <c>false</c>.
        /// </value>
        [System.Xml.Serialization.XmlIgnore()]
        public bool IsMandatory
        {
            get { return Mandatory == "1"; }
        }


        [Sushi.Mediakiwi.Framework.ContentListItem.Choice_Checkbox("Only read", Expression = Sushi.Mediakiwi.Framework.OutputExpression.Alternating)]
        public string OnlyRead { get; set; }

        [System.Xml.Serialization.XmlIgnore()]
        public bool IsOnlyRead
        {
            get { return OnlyRead == "1"; }
            set { if (value) this.OnlyRead = "1"; else this.OnlyRead = "0"; }
        }

        [Sushi.Mediakiwi.Framework.ContentListItem.TextField("Interactive help", 512, false, Expression = Sushi.Mediakiwi.Framework.OutputExpression.Alternating)]
        public string InteractiveHelp { get; set; }

        /// <summary>
        /// Gets or sets the list select.
        /// </summary>
        /// <value>The list select.</value>
        [Sushi.Mediakiwi.Framework.OnlyEditableWhenTrue("IsPostBackEnabled")]
        [Sushi.Mediakiwi.Framework.ContentListItem.Choice_Checkbox("Auto postback", Expression = Sushi.Mediakiwi.Framework.OutputExpression.Alternating)]
        public bool AutoPostBack { get; set; }

        //ListItemCollection m_Lists;
        ///// <summary>
        ///// Gets the lists.
        ///// </summary>
        ///// <value>The lists.</value>
        //public ListItemCollection Lists
        //{
        //    get
        //    {
        //        if (m_Lists == null)
        //        {
        //            m_Lists = new ListItemCollection();
        //            m_Lists.Add(new ListItem(""));

        //            SortedList<string, ListItem> sorted = new SortedList<string, ListItem>();       
        //            foreach (Sushi.Mediakiwi.Data.IComponentList tmp in Sushi.Mediakiwi.Data.ComponentList.SelectAll())
        //            {
        //                ListItem[] list = GetSortedListCollection(tmp.ID);
        //                if (list.Length > 0)
        //                    sorted.Add(tmp.Name, new ListItem(tmp.Name, tmp.ID.ToString()));
        //            }
        //            foreach (ListItem li in sorted.Values)
        //                m_Lists.Add(li);
        //        }
        //        return m_Lists;
        //    }
        //}


        //ListItem[] GetSortedListCollection(int listID)
        //{
        //    SortedList<string, ListItem> sorted = new SortedList<string, ListItem>();

        //    Sushi.Mediakiwi.Data.IComponentList list = Sushi.Mediakiwi.Data.ComponentList.SelectOne(listID);
        //    System.Type type;
        //    object instance = Wim.Utility.CreateInstance(list.AssemblyName, list.ClassName, out type, false);
        //    if (instance == null) return new ListItem[0];

        //    foreach (System.Reflection.PropertyInfo info in type.GetProperties())
        //    {
        //        if (info.PropertyType == typeof(ListItemCollection))
        //        {
        //            Sushi.Mediakiwi.Framework.ExposedListCollection[] attribs = info.GetCustomAttributes(typeof(Sushi.Mediakiwi.Framework.ExposedListCollection), false) as Sushi.Mediakiwi.Framework.ExposedListCollection[];
        //            if (attribs != null && attribs.Length == 1)
        //            {
        //                if (string.IsNullOrEmpty(attribs[0].CollectionReferencingMethod))
        //                {
        //                    ListItem li = new ListItem(attribs[0].Description, info.Name);
        //                    sorted.Add(li.Text, li);
        //                }
        //                else
        //                {
        //                    ListItemCollection collection = info.GetValue(instance, null) as ListItemCollection;
        //                    foreach (ListItem item in collection)
        //                    {
        //                        ListItem li = new ListItem(item.Text, string.Concat(attribs[0].CollectionReferencingMethod, ":", item.Value));
        //                        sorted.Add(li.Text, li);
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    List<ListItem> lilist = new List<ListItem>();
        //    foreach (ListItem li in sorted.Values)
        //        lilist.Add(li);
        //    return lilist.ToArray();
        //}



        /// <summary>
        /// Gets or sets the option list select.
        /// </summary>
        /// <value>The option list select.</value>
        [Sushi.Mediakiwi.Framework.OnlyEditableWhenTrue("IsFixedOrRadioDropdownAndNoListSelect", true)]
        [DatabaseColumn("Property_OptionList_Key", SqlDbType.Int, IsNullable = true)]
        [Sushi.Mediakiwi.Framework.ContentListItem.Choice_Dropdown("Option list", "OptionLists", false, true, Expression = Sushi.Mediakiwi.Framework.OutputExpression.Alternating)]
        public int? OptionListSelect { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is short.
        /// </summary>
        /// <value><c>true</c> if this instance is short; otherwise, <c>false</c>.</value>
        [Sushi.Mediakiwi.Framework.ContentListItem.Choice_Checkbox("Is short (design)", Expression = Sushi.Mediakiwi.Framework.OutputExpression.Alternating)]
        [DatabaseColumn("Property_IsShort", SqlDbType.Bit)]
        public bool IsShort { get; set; }
        /// <summary>
        /// Gets or sets the list select.
        /// </summary>
        /// <value>The list select.</value>
        [Sushi.Mediakiwi.Framework.OnlyEditableWhenTrue("IsFixedOrRadioDropdownAndNoOptionList", true)]
        [DatabaseColumn("Property_ListBase_Key", SqlDbType.Int, IsNullable = true)]
        [Sushi.Mediakiwi.Framework.ContentListItem.Choice_Dropdown("List", "Lists", false, true, Expression = Sushi.Mediakiwi.Framework.OutputExpression.Alternating)]
        public int? ListSelect { get; set; }
     
        private string m_CanContainOneItem;
        /// <summary>
        /// Gets or sets the list select.
        /// </summary>
        /// <value>The list select.</value>
        [Sushi.Mediakiwi.Framework.OnlyEditableWhenTrue("IsSublistSelect")]
        [Sushi.Mediakiwi.Framework.ContentListItem.Choice_Checkbox("Single item", Expression = Sushi.Mediakiwi.Framework.OutputExpression.Alternating)]
        public string CanContainOneItem
        {
            set { m_CanContainOneItem = value; }
            get { return m_CanContainOneItem; }
        }
        private string m_ListCollection;
        /// <summary>
        /// Gets or sets the list select.
        /// </summary>
        /// <value>The list select.</value>
        //[Sushi.Mediakiwi.Framework.OnlyVisibleWhenTrue("IsSublistSelect", false)]
        [Sushi.Mediakiwi.Framework.OnlyEditableWhenTrue("IsFixedOrRadioDropdownAndListSelect", true)]
        [DatabaseColumn("Property_ListCollection", SqlDbType.VarChar, Length = 50, IsNullable = true)]
        [Sushi.Mediakiwi.Framework.ContentListItem.Choice_Dropdown("List collection", "ListCollections", false, false, Expression = Sushi.Mediakiwi.Framework.OutputExpression.Alternating)]
        public string ListCollection
        {
            set { m_ListCollection = value; }
            get { return m_ListCollection; }
        }


        [Sushi.Mediakiwi.Framework.OnlyEditableWhenTrue("IsDropdown")]
        [Sushi.Mediakiwi.Framework.ContentListItem.Choice_Checkbox("Is first empty", Expression = Sushi.Mediakiwi.Framework.OutputExpression.Alternating)]
        public string EmptyFirst { get; set; }

        [System.Xml.Serialization.XmlIgnore()]
        public bool IsEmptyFirst
        {
            get { return this.EmptyFirst == "1"; }
        }

        //[Sushi.Mediakiwi.Framework.OnlyVisibleWhenTrue("IsSublistSelect", false)]
        //[Sushi.Mediakiwi.Framework.OnlyEditableWhenTrue("IsFixedOrRadioDropdownSublist", true)]
        /// <summary>
        /// Gets or sets the type of the property.
        /// </summary>
        /// <value>The type of the property.</value>
        [DatabaseColumn("Property_CodeType", SqlDbType.Int, IsNullable = true)]
        [Sushi.Mediakiwi.Framework.ContentListItem.Choice_Dropdown("Property type", "PropertyTypeList", false, false, Expression = Sushi.Mediakiwi.Framework.OutputExpression.Alternating)]
        public int PropertyType { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is filter.
        /// </summary>
        /// <value><c>true</c> if this instance is filter; otherwise, <c>false</c>.</value>
        //[Sushi.Mediakiwi.Framework.OnlyEditableWhenTrue("IsNotFilterOrType")]
        [Sushi.Mediakiwi.Framework.ContentListItem.Choice_Checkbox("Is hidden", Expression = Sushi.Mediakiwi.Framework.OutputExpression.Alternating)]
        [DatabaseColumn("Property_IsHidden", SqlDbType.Bit, IsNullable = true)]
        public bool IsHidden { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is filter.
        /// </summary>
        /// <value><c>true</c> if this instance is filter; otherwise, <c>false</c>.</value>
        [Sushi.Mediakiwi.Framework.ContentListItem.Choice_Dropdown("Database column", "DataColumns", false, true, Expression = Sushi.Mediakiwi.Framework.OutputExpression.Alternating)]
        [DatabaseColumn("Property_Column", SqlDbType.VarChar, Length = 50, IsNullable = true)]
        public string Filter { get ;set; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance can filter.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can filter; otherwise, <c>false</c>.
        /// </value>
        [Sushi.Mediakiwi.Framework.ContentListItem.Choice_Checkbox("Can be filtered", Expression = Sushi.Mediakiwi.Framework.OutputExpression.Alternating)]
        [DatabaseColumn("Property_CanFilter", SqlDbType.Bit, IsNullable = true)]
        public bool CanFilter { get; set; }

        [Sushi.Mediakiwi.Framework.ContentListItem.Choice_Checkbox("Only for input", InteractiveHelp = "This field is not mapped to the database", Expression = Sushi.Mediakiwi.Framework.OutputExpression.Right)]
        [DatabaseColumn("Property_OnlyInput", SqlDbType.Bit, IsNullable = true)]
        public bool OnlyInput { get; set; }

        private string m_Data;
        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        /// <value>The data.</value>
        [DatabaseColumn("Property_Data", SqlDbType.Xml, IsNullable = true)]
        public string Data
        {
            get { 
                return m_Data; }
            set { m_Data = value; }
        }

        private bool m_IsFixed;
        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        /// <value>The data.</value>
        [DatabaseColumn("Property_IsFixed", SqlDbType.Bit)]
        public bool IsFixed
        {
            get { return m_IsFixed; }
            set { m_IsFixed = value; }
        }


        private int? m_InheritedID;
        /// <summary>
        /// Gets or sets the inherited ID. This property is inherited of a template property list.
        /// </summary>
        /// <value>The inherited ID.</value>
        [DatabaseColumn("Property_Property_Key", SqlDbType.Int, IsNullable = true)]
        public int? InheritedID
        {
            set { m_InheritedID = value; }
            get { return m_InheritedID; }
        }

        Sushi.Mediakiwi.Data.PropertyOption[] m_Options;

        /// <summary>
        /// Gets the options.
        /// </summary>
        /// <value>The options.</value>
        public Sushi.Mediakiwi.Data.PropertyOption[] Options
        {
            get {
                if (m_Options == null)
                    m_Options = Sushi.Mediakiwi.Data.PropertyOption.SelectAll(this.ID);
                return m_Options; 
            }
        }

        private int m_SortOrder;
        /// <summary>
        /// Gets or sets the sort order.
        /// </summary>
        /// <value>The sort order.</value>
        [DatabaseColumn("Property_SortOrder", SqlDbType.Int, IsNullable = true)]
        public int SortOrder
        {
            set { m_SortOrder = value; }
            get { return m_SortOrder; }
        }

        #region iExportable Members


        public DateTime? Updated
        {
            get { return DateTime.Now; }
        }

        #endregion


        /// <summary>
        /// Delete an implementation record.
        /// </summary>
        /// <returns></returns>
        public override bool Delete()
        {
            List<DatabaseDataValueColumn> where = new List<DatabaseDataValueColumn>();
            where.Add(new DatabaseDataValueColumn("Property_Property_Key", SqlDbType.Int, this.ID));

            Delete(where);

            //try
            //{
            //    var list = Sushi.Mediakiwi.Data.ComponentList.SelectOne(this.ListID);
            //    this.Execute(string.Format("alter table {2} drop column {0}_{1}", List.Catalog().ColumnPrefix, this.FieldName, List.Catalog().Table));
            //}
            //catch (Exception) {}

            return base.Delete();
        }


        /// <summary>
        /// Determines whether [has content container] [the specified list ID].
        /// </summary>
        /// <param name="listID">The list ID.</param>
        /// <returns>
        /// 	<c>true</c> if [has content container] [the specified list ID]; otherwise, <c>false</c>.
        /// </returns>
        public static bool HasContentContainer(int listID)
        {
            List<DatabaseDataValueColumn> where = new List<DatabaseDataValueColumn>();
            where.Add(new DatabaseDataValueColumn("Property_List_Key", SqlDbType.Int, listID));
            where.Add(new DatabaseDataValueColumn("Property_Type", SqlDbType.Int, 35));

            Property tmp = (Property)new Property()._SelectOne(where);
            return !tmp.IsNewInstance;
        }

        /// <summary>
        /// Selects all.
        /// </summary>
        /// <returns></returns>
        public static List<Property> SelectAll()
        {
            Property implement = new Property();
            List<Property> list = new List<Property>();

            foreach (object o in implement._SelectAll())
                list.Add((Property)o);

            return list;
        }

        /// <summary>
        /// This is a memory allocation list for when using command line tools
        /// </summary>
        static List<MemoryItemProperty> MemoryAllocationList;

        /// <summary>
        /// 
        /// </summary>
        class MemoryItemProperty
        {
            public MemoryItemProperty() { }
            public MemoryItemProperty(int listID, int? listTypeID, bool showEmptyTypes, bool onlyReturnFlexibleProperties, Property[] properties)
            {
                this.ListID = listID;
                this.ListTypeID = listTypeID;
                this.ShowEmptyTypes = showEmptyTypes;
                this.OnlyReturnFlexibleProperties = onlyReturnFlexibleProperties;
                this.Properties = properties;
            }

            internal int ListID;
            internal int? ListTypeID;
            internal bool ShowEmptyTypes;
            internal bool OnlyReturnFlexibleProperties;
            internal Property[] Properties;
        }

        #endregion REPLICATED TO Sushi.Mediakiwi.Data.Standard
    }
}

