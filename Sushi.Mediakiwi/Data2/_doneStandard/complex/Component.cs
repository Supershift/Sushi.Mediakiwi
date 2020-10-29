using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Sushi.Mediakiwi.Data.DalReflection;

namespace Sushi.Mediakiwi.Data
{
    #region REPLICATED TO Sushi.Mediakiwi.Data.Standard

    public interface IComponent
    {
        int ID { get; set; }
        string Target { get; set; }
        bool IsSecundary { get; set; }
        Sushi.Mediakiwi.Data.ComponentTemplate Template { get; }
        string FixedFieldName { get; set; }

    }

    #endregion REPLICATED TO Sushi.Mediakiwi.Data.Standard

    /// <summary>
    /// Represents a Component entity.
    /// </summary>
    [DatabaseTable("wim_Components", Join = "inner join wim_ComponentTemplates on Component_ComponentTemplate_Key = ComponentTemplate_Key", Order = "Component_SortOrder")]
    public class Component : DatabaseEntity, iExportable, IComponent
    {
        [DatabaseColumn("ComponentTarget_Component_Target", SqlDbType.UniqueIdentifier, CollectionLevel = DatabaseColumnGroup.Additional, IsOnlyRead = true)]
        public Guid ComponentTarget { get; set; } // REVIEW MARC, dit zou een extra join betekenen

        #region REPLICATED TO Sushi.Mediakiwi.Data.Standard


        int m_ID;
        /// <summary>
        /// Unique identifier of this component
        /// </summary>
        /// <value>The ID.</value>
        [DatabaseColumn("Component_Key", SqlDbType.Int, IsPrimaryKey = true)]
        public int ID
        {
            get { return m_ID; }
            set { m_ID = value; }
        }

        Guid m_GUID;
        /// <summary>
        /// Global Unique identifier of this component
        /// </summary>
        /// <value>The GUID.</value>
        [DatabaseColumn("Component_GUID", SqlDbType.UniqueIdentifier, IsMigrationKey = true)]
        public Guid GUID
        {
            get
            {
                if (this.m_GUID == Guid.Empty) this.m_GUID = Guid.NewGuid();
                return m_GUID;
            }
            set { m_GUID = value; }
        }

        /// <summary>
        /// The page to which this componentversion belongs
        /// </summary>
        /// <value>The page ID.</value>
        [DatabaseColumn("Component_Page_Key", SqlDbType.Int, IsNullable = true)]
        public int? PageID { get; set; }

        int m_TemplateID;
        /// <summary>
        /// The corresponding componentTemplate identifier
        /// </summary>
        /// <value>The template ID.</value>
        [DatabaseColumn("Component_ComponentTemplate_Key", SqlDbType.Int)]
        public int TemplateID
        {
            get { return m_TemplateID; }
            set { m_TemplateID = value; }
        }

        int m_SortOrder;
        /// <summary>
        /// Sortorder
        /// </summary>
        /// <value>The sort order.</value>
        [DatabaseColumn("Component_SortOrder", SqlDbType.Int, IsNullable = true)]
        public int SortOrder
        {
            get { return m_SortOrder; }
            set { m_SortOrder = value; }
        }

        bool m_IsFixed;
        /// <summary>
        /// Is this component fixed on the page?
        /// </summary>
        /// <value><c>true</c> if this instance is fixed; otherwise, <c>false</c>.</value>
        [DatabaseColumn("Component_IsFixedOnTemplate", SqlDbType.Bit)]
        public bool IsFixed
        {
            get { return m_IsFixed; }
            set { m_IsFixed = value; }
        }

        bool m_IsAlive;
        /// <summary>
        /// Is this component alive? Alive: When a fixed component is removed from a page it is still stored.
        /// When this component is reintroduced this content will be restored.
        /// </summary>
        /// <value><c>true</c> if this instance is alive; otherwise, <c>false</c>.</value>
        [DatabaseColumn("Component_IsAlive", SqlDbType.Bit)]
        public bool IsAlive
        {
            get { return m_IsAlive; }
            set { m_IsAlive = value; }
        }

        bool m_IsSecundary;
        /// <summary>
        /// Does this componentversion belong to the secundary container (like f.e. the service column)?
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is secundary; otherwise, <c>false</c>.
        /// </value>
        //[DatabaseColumn("Component_IsSecundary", SqlDbType.Bit)]
        [DatabaseColumn("ComponentTemplate_IsSecundaryContainerItem", SqlDbType.Bit, IsOnlyRead = true)]
        public bool IsSecundary
        {
            get { return m_IsSecundary; }
            set { m_IsSecundary = value; }
        }

        [DatabaseColumn("ComponentTemplate_IsShared", SqlDbType.Bit, IsOnlyRead = true)]
        public bool IsShared { get; set; }

        bool m_IsSearchable;
        /// <summary>
        /// Is this component searchable?
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is searchable; otherwise, <c>false</c>.
        /// </value>
        [DatabaseColumn("ComponentTemplate_IsSearchable", SqlDbType.Bit, IsOnlyRead = true)]
        public bool IsSearchable
        {
            get { return m_IsSearchable; }
            set { m_IsSearchable = value; }
        }

        /// <summary>
        /// Gets or sets the type of the ajax.
        /// </summary>
        /// <value>The type of the ajax.</value>
        [DatabaseColumn("ComponentTemplate_AjaxType", SqlDbType.Int, IsOnlyRead = true, IsNullable = true)]
        public int AjaxType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is page based cache.
        /// 0 = no cache, 1 = component based cache, 2 = page level cache
        /// </summary>
        [DatabaseColumn("ComponentTemplate_CacheLevel", SqlDbType.Int, IsOnlyRead = true, IsNullable = true)]
        public int CacheLevel { get; set; }

        string m_FixedFieldName;
        /// <summary>
        /// The identifying name of the fixed component
        /// </summary>
        /// <value>The name of the fixed field.</value>
        [DatabaseColumn("Component_Fixed_Id", SqlDbType.NVarChar, Length = 50, IsNullable = true)]
        public string FixedFieldName
        {
            get { return m_FixedFieldName; }
            set { m_FixedFieldName = value; }
        }

        string m_Name;
        /// <summary>
        /// The name of the componentversion
        /// </summary>
        /// <value>The name.</value>
        //[DatabaseColumn("Component_Name", SqlDbType.VarChar, Length = 50)]
        [DatabaseColumn("ComponentTemplate_Name", SqlDbType.VarChar, Length = 50, IsOnlyRead = true)]
        public string Name
        {
            get { return m_Name; }
            set { m_Name = value; }
        }

        [DatabaseColumn("Component_Target", SqlDbType.VarChar, Length = 50, IsNullable = true)]
        public string Target { get; set; }

        string m_VaryByParam;
        /// <summary>
        /// Gets or sets the vary by param.
        /// </summary>
        /// <value>The vary by param.</value>
        [DatabaseColumn("ComponentTemplate_CacheParams", SqlDbType.VarChar, Length = 50, IsNullable = true, IsOnlyRead = true)]
        public string VaryByParam
        {
            get { return m_VaryByParam; }
            set { m_VaryByParam = value; }
        }

        string m_TemplateLocation;
        /// <summary>
        /// Gets or sets the template location.
        /// </summary>
        /// <value>The template location.</value>
        [DatabaseColumn("ComponentTemplate_Location", SqlDbType.VarChar, Length = 250, IsOnlyRead = true)]
        public string TemplateLocation
        {
            get { return m_TemplateLocation; }
            set { m_TemplateLocation = value; }
        }

        [DatabaseColumn("ComponentTemplate_Source", SqlDbType.NText, IsNullable = true, IsOnlyRead = true)]
        public string Source { get; set; }

        string m_Serialized_XML;
        /// <summary>
        /// The serialized content matching this componentversion
        /// </summary>
        /// <value>The serialized_ XML.</value>
        [DatabaseColumn("Component_XML", SqlDbType.NText, IsNullable = true)]
        public string Serialized_XML
        {
            get { return m_Serialized_XML; }
            set { m_Serialized_XML = value; }
        }

        DateTime m_Created;
        /// <summary>
        /// Gets or sets the created.
        /// </summary>
        /// <value>The created.</value>
        [DatabaseColumn("Component_Created", SqlDbType.DateTime, IsNullable = false)]
        public DateTime Created
        {
            get
            {
                if (this.m_Created == DateTime.MinValue) this.m_Created = Sushi.Mediakiwi.Data.Common.DatabaseDateTime;
                return m_Created;
            }
            set { m_Created = value; }
        }

        DateTime m_Updated;
        [DatabaseColumn("Component_Updated", SqlDbType.DateTime, IsNullable = false)]
        public DateTime Updated
        {
            get
            {
                if (this.m_Updated == DateTime.MinValue) this.m_Updated = Created;
                return m_Updated;
            }
            set { m_Updated = value; }
        }

        DateTime? m_SortField_Date;
        /// <summary>
        /// The sortfield's introduced date.
        /// </summary>
        /// <value>The sort field_ date.</value>
        [DatabaseColumn("Component_SortDate", SqlDbType.DateTime, IsNullable = true)]
        public DateTime? SortField_Date
        {
            get { return m_SortField_Date; }
            set { m_SortField_Date = value; }
        }

        DateTime? iExportable.Updated
        {
            get { return Updated; }
        }


        Sushi.Mediakiwi.Data.Content m_Content;
        /// <summary>
        /// Get the content of this component
        /// </summary>
        /// <value>The content.</value>
        public Sushi.Mediakiwi.Data.Content Content
        {
            get
            {
                if (m_Content != null) return m_Content;

                if (Serialized_XML == null || Serialized_XML.Trim().Length == 0) return null;
                m_Content = Sushi.Mediakiwi.Data.Content.GetDeserialized(Serialized_XML);
                return m_Content;
            }
        }


        Sushi.Mediakiwi.Data.ComponentTemplate m_Template;
        /// <summary>
        /// Gets the template.
        /// </summary>
        /// <value>The template.</value>
        public Sushi.Mediakiwi.Data.ComponentTemplate Template
        {
            get
            {
                if (m_Template == null)
                    m_Template = Sushi.Mediakiwi.Data.ComponentTemplate.SelectOne(this.TemplateID);
                return m_Template;
            }
        }
        


       /// <summary>
        /// Select all available components
        /// </summary>
        public static Component[] SelectAll()
        {
            List<Component> list = new List<Component>();
            Component component = new Component();
            if (!string.IsNullOrEmpty(SqlConnectionString2)) component.SqlConnectionString = SqlConnectionString2;
            foreach (object o in component._SelectAll())
            {
                list.Add((Component)o);
            }
            return list.ToArray();
        }

        /// <summary>
        /// Selects the one.
        /// </summary>
        /// <param name="ID">The ID.</param>
        /// <returns></returns>
        public static Component SelectOne(int ID)
        {
            return (Component)new Component()._SelectOne(ID);
        }

        /// <summary>
        /// Selects the one.
        /// </summary>
        /// <param name="componentGUID">The component GUID.</param>
        /// <returns></returns>
        public static Component SelectOne(Guid componentGUID)
        {
            List<DatabaseDataValueColumn> list = new List<DatabaseDataValueColumn>();
            list.Add(new DatabaseDataValueColumn("Component_GUID", SqlDbType.UniqueIdentifier, componentGUID));
            return (Component)new Component()._SelectOne(list);
        }

        /// <summary>
        /// Selects the one.
        /// </summary>
        /// <param name="pageID">The page ID.</param>
        /// <param name="componentID">The component ID.</param>
        /// <returns></returns>
        public static Sushi.Mediakiwi.Data.Component SelectOne(int pageID, string componentID)
        {
            List<DatabaseDataValueColumn> list = new List<DatabaseDataValueColumn>();
            list.Add(new DatabaseDataValueColumn("Component_Fixed_Id", SqlDbType.NVarChar, componentID));
            list.Add(new DatabaseDataValueColumn("Component_IsFixedOnTemplate", SqlDbType.Bit, true));
            list.Add(new DatabaseDataValueColumn("Component_Page_Key", SqlDbType.Int, pageID));
            return (Component)new Component()._SelectOne(list, "Fixed", string.Format("{0}#{1}", pageID, componentID));
        }

        /// <summary>
        /// Selects the type of the one based on.
        /// </summary>
        /// <param name="pageID">The page ID.</param>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static Sushi.Mediakiwi.Data.Component SelectOne(int pageID, System.Type type)
        {
            List<DatabaseDataValueColumn> list = new List<DatabaseDataValueColumn>();
            list.Add(new DatabaseDataValueColumn("ComponentTemplate_Type", SqlDbType.NVarChar, type.BaseType.ToString()));
            list.Add(new DatabaseDataValueColumn("Component_Page_Key", SqlDbType.Int, pageID));
            return (Component)new Component()._SelectOne(list, "TypePage", string.Concat(pageID, type.BaseType.ToString()));
        }

        /// <summary>
        /// Select all available components on a page with a particular component template as basis
        /// </summary>
        /// <param name="pageID">The page ID.</param>
        /// <param name="componentTemplateID">The component template ID.</param>
        /// <returns></returns>
        public static Component[] SelectAll(int pageID, int componentTemplateID)
        {
            List<Component> list = new List<Component>();

            foreach (Component c in SelectAll(pageID))
            {
                if (c.TemplateID == componentTemplateID)
                    list.Add(c);
            }
            return list.ToArray();
        }

        /// <summary>
        /// Selects all.
        /// </summary>
        /// <param name="pageID">The page ID.</param>
        /// <param name="isSecundary">if set to <c>true</c> [is secundary].</param>
        /// <returns></returns>
        public static Component[] SelectAll(int pageID, bool isSecundary)
        {
            List<Component> list = new List<Component>();

            foreach (Component c in SelectAll(pageID))
            {
                if (c.IsSecundary == isSecundary)
                    list.Add(c);
            }
            return list.ToArray();
        }

        public static IComponent[] VerifyVisualisation(Data.PageTemplate template, IAvailableTemplate[] components, string section, ref bool isSecundaryContentContainer, bool isCmsRequest = false)    //NAAR STANDARD
        {
            List<IComponent> selection = new List<IComponent>();
            foreach (var t in components)
            {
                var a = (AvailableTemplate)t;
                selection.Add(a);
            }
            return VerifyVisualisation(template, selection.ToArray(), section, ref isSecundaryContentContainer, isCmsRequest);
        }

        public static IComponent[] VerifyVisualisation(Data.PageTemplate template, IComponent[] components, string section, ref bool isSecundaryContentContainer, bool isCmsRequest = false)    //NAAR STANDARD
        {
            bool isLegacySection = section == null;
            string legacyContentTab, legacyServiceTab;
            if (template != null)
            {
                legacyContentTab = template.Data["TAB.LCT"].Value ?? Wim.CommonConfiguration.DEFAULT_CONTENT_TAB;
                legacyServiceTab = template.Data["TAB.LST"].Value ?? Wim.CommonConfiguration.DEFAULT_SERVICE_TAB;

                if (!string.IsNullOrEmpty(legacyServiceTab) && section == legacyServiceTab)
                    isSecundaryContentContainer = true;

                //if ((section == legacyContentTab && !string.IsNullOrEmpty(legacyContentTab)) || (section == legacyServiceTab && !string.IsNullOrEmpty(legacyServiceTab)))
                //{
                //    section = null;
                //    isLegacySection = true;
                //}
            }

            List<IComponent> selection = new List<IComponent>();
            foreach (var component in components)
            {
                //  Section 1 = service column
                //  This was !container.IsComponent
                if (template != null)
                {
                    if (component.Template.IsSecundaryContainerItem != isSecundaryContentContainer)
                        continue;

                    if (component.FixedFieldName != null)
                    {
                        if (!isCmsRequest)
                            continue;
                        else
                        {
                            if (isSecundaryContentContainer)
                                continue;
                        }
                    }

                    if (component.Target == null || component.Target == section)// || isLegacySection)
                    {
                        // ALL IS OK!
                    }
                    else
                    {
                        continue;
                    }

                    selection.Add(component);
                }
            }
            return selection.ToArray();
        }

        /// <summary>
        /// Select all available components on a page
        /// </summary>
        /// <param name="pageID">The page ID.</param>
        /// <returns></returns>
        public static Component[] SelectAll(int pageID)                                         //NAAR STANDARD
        {
            return SelectAllInherited(pageID, false);
        }

        public static Component[] SelectAllInherited(int pageID, bool ignoreInheritance)        //NAAR STANDARD
        {
            Component component = new Component();
            List<Component> list = new List<Component>();

            Object[] arr = component.GetCachedArray("Page", pageID.ToString());
            if (arr != null)
            {
                foreach (object o in arr)
                    list.Add((Component)o);
                return list.ToArray();
            }

            Page page = Page.SelectOne(pageID, false);
            //  If the page is set to hold inherited content, please take that content
            if (!ignoreInheritance && page.InheritContent && page.MasterID.HasValue)
                pageID = page.MasterID.Value;

            List<DatabaseDataValueColumn> whereClause = new List<DatabaseDataValueColumn>();
            whereClause.Add(new DatabaseDataValueColumn("Component_Page_Key", SqlDbType.Int, pageID));

            if (!string.IsNullOrEmpty(SqlConnectionString2)) component.SqlConnectionString = SqlConnectionString2;
            foreach (object o in component._SelectAll(whereClause, false, "Page", pageID.ToString()))
            {
                Component c = (Component)o;
                //  When the content is set to inherit please apply the child page ID
                if (!ignoreInheritance && page.InheritContent && page.MasterID.HasValue) c.PageID = page.ID;

                list.Add(c);
            }
            return list.ToArray();
        }

        public static Component[] SelectAllShared1(int pageID)      //NAAR STANDARD
        {
            Component component = new Component();
            List<Component> list = new List<Component>();

            Object[] arr = component.GetCachedArray("PageShared", pageID.ToString());
            if (arr != null)
            {
                foreach (object o in arr)
                    list.Add((Component)o);
                return list.ToArray();
            }

            Page page = Page.SelectOne(pageID, false);
            //  If the page is set to hold inherited content, please take that content
            if (page.InheritContent && page.MasterID.HasValue)
                pageID = page.MasterID.Value;

            component.CollectionLevel = DatabaseColumnGroup.Additional;
            component.SqlJoin += " join wim_ComponentTargets on Component_GUID = ComponentTarget_Component_Source";

            List<DatabaseDataValueColumn> whereClause = new List<DatabaseDataValueColumn>();
            whereClause.Add(new DatabaseDataValueColumn("ComponentTarget_Page_Key", SqlDbType.Int, pageID));
            whereClause.Add(new DatabaseDataValueColumn("ComponentTemplate_IsShared", SqlDbType.Bit, true));

            if (!string.IsNullOrEmpty(SqlConnectionString2)) component.SqlConnectionString = SqlConnectionString2;
            foreach (object o in component._SelectAll(whereClause, false, "PageShared", pageID.ToString()))
            {
                Component c = (Component)o;
                //  When the content is set to inherit please apply the child page ID
                if (page.InheritContent && page.MasterID.HasValue) c.PageID = page.ID;

                list.Add(c);
            }
            return list.ToArray();
        }

        #endregion REPLICATED TO Sushi.Mediakiwi.Data.Standard

    }
}