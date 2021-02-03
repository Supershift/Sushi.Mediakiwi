using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Sushi.Mediakiwi.Data.DalReflection;
using System.Linq;

namespace Sushi.Mediakiwi.Data
{
    /// <summary>
    /// Represents a ComponentTemplate entity.
    /// </summary>
    [DatabaseTable("wim_ComponentTemplates")]
    public class ComponentTemplate : DatabaseEntity, iExportable
    {



        #region REPLICATED TO Sushi.Mediakiwi.Data.Standard
        private int m_ID;
        /// <summary>
        /// Gets or sets the ID.
        /// </summary>
        /// <value>The ID.</value>
        [DatabaseColumn("ComponentTemplate_Key", SqlDbType.Int, IsPrimaryKey = true)]
        public int ID
        {
            get { return m_ID; }
            set { m_ID = value; }
        }

        private string m_Name;
        /// <summary>
        /// Name of the template
        /// </summary>
        [Sushi.Mediakiwi.Framework.ContentListItem.TextField("Name", 50, true, Expression = Sushi.Mediakiwi.Framework.OutputExpression.Alternating)]
        [DatabaseColumn("ComponentTemplate_Name", SqlDbType.NVarChar, Length = 50)]
        public string Name
        {
            get { return m_Name; }
            set { m_Name = value; }
        }

        [Sushi.Mediakiwi.Framework.OnlyVisibleWhenFalse("IS_HEADLESS")]
        [Sushi.Mediakiwi.Framework.ContentListItem.Choice_Checkbox("Headless", true, Expression = Sushi.Mediakiwi.Framework.OutputExpression.Alternating)]
        public bool HasEditableSource { get; set; }


        private string m_Location;
        /// <summary>
        /// Relative path of the component template file (.ascx)
        /// </summary>
        [Sushi.Mediakiwi.Framework.OnlyVisibleWhenFalse("IS_HEADLESS")]
        [Sushi.Mediakiwi.Framework.ContentListItem.TextField("Location", 250, true)]
        [DatabaseColumn("ComponentTemplate_Location", SqlDbType.VarChar, Length = 250)]
        public string Location
        {
            get { return m_Location; }
            set { m_Location = value; }
        }

        string m_TypeDefinition;
        /// <summary>
        /// Template type definition.
        /// </summary>
        [Sushi.Mediakiwi.Framework.OnlyVisibleWhenFalse("IS_HEADLESS")]
        [Sushi.Mediakiwi.Framework.ContentListItem.TextField("Type", 250, true)]
        [DatabaseColumn("ComponentTemplate_Type", SqlDbType.NVarChar, Length = 250, IsNullable = true)]
        public string TypeDefinition
        {
            get { return m_TypeDefinition; }
            set { m_TypeDefinition = value; }
        }

        [Sushi.Mediakiwi.Framework.ContentListItem.TextField("Source tag", 25, false, Expression = Sushi.Mediakiwi.Framework.OutputExpression.Alternating)]
        [DatabaseColumn("ComponentTemplate_Tag", SqlDbType.VarChar, Length = 25, IsNullable = true)]
        public string SourceTag { get; set; }

        public bool HasSourceTag
        {
            get { return string.IsNullOrWhiteSpace(SourceTag);  }
        }


        //[Sushi.Mediakiwi.Framework.ContentListItem.TextArea("Source", 0, IsSourceCode = true)]
        [Sushi.Mediakiwi.Framework.OnlyVisibleWhenFalse("IS_HEADLESS")]
        [DatabaseColumn("ComponentTemplate_Source", SqlDbType.NText, IsNullable = true)]
        public string Source { get; set; }

        //[Sushi.Mediakiwi.Framework.ContentListItem.Section()]
        //public string SubText0 { get { return "Properties"; } set { } }

        /// <summary>
        /// Gets or sets the reference id.
        /// </summary>
        /// <value>The reference id.</value>
        [Sushi.Mediakiwi.Framework.OnlyVisibleWhenFalse("IS_HEADLESS")]
        [Sushi.Mediakiwi.Framework.ContentListItem.TextField("Reference", 5, false, false, null, Expression = Sushi.Mediakiwi.Framework.OutputExpression.Alternating)]
        [DatabaseColumn("ComponentTemplate_ReferenceId", SqlDbType.NVarChar, Length = 5, IsNullable = true)]
        public string ReferenceID { get; set; }

        private bool m_IsSearchable;
        /// <summary>
        /// The possibility to search the content of this template 
        /// </summary>
        [Sushi.Mediakiwi.Framework.OnlyVisibleWhenFalse("IS_HEADLESS")]
        [Sushi.Mediakiwi.Framework.ContentListItem.Choice_Checkbox("Is searchable", Expression = Sushi.Mediakiwi.Framework.OutputExpression.Alternating)]
        [DatabaseColumn("ComponentTemplate_IsSearchable", SqlDbType.Bit)]
        public bool IsSearchable
        {
            get { return m_IsSearchable; }
            set { m_IsSearchable = value; }
        }

        private int? m_SiteID;
        /// <summary>
        /// Part of a specific site or site group (when the site is a master site with children this template is automatically part of those children).
        /// </summary>
        [Sushi.Mediakiwi.Framework.ContentListItem.Choice_Dropdown("Channel", "Sites", false, "When a master site with children is chosen this also applies for a child sites.", Expression = Sushi.Mediakiwi.Framework.OutputExpression.Alternating)]
        [DatabaseColumn("ComponentTemplate_Site_Key", SqlDbType.Int, IsNullable = true)]
        public int? SiteID
        {
            get { return m_SiteID; }
            set { m_SiteID = value; }
        }

        [Sushi.Mediakiwi.Framework.ContentListItem.Choice_Checkbox("Can delete", Expression = Sushi.Mediakiwi.Framework.OutputExpression.Alternating)]
        public bool CanDelete
        {
            get { return !m_IsFixedOnPage; }
            set { m_IsFixedOnPage = !value; }
        }

        bool m_IsFixedOnPage;
        /// <summary>
        /// Is the template fixed on a page?
        /// </summary>
        //[Sushi.Mediakiwi.Framework.ContentListItem.Choice_Checkbox("Fixed on page", Expression = Sushi.Mediakiwi.Framework.OutputExpression.Alternating)]
        [Sushi.Mediakiwi.Framework.OnlyVisibleWhenFalse("IS_HEADLESS")]
        [DatabaseColumn("ComponentTemplate_IsFixed", SqlDbType.Bit)]
        public bool IsFixedOnPage
        {
            get { return m_IsFixedOnPage; }
            set { m_IsFixedOnPage = value; }
        }

        private string m_Description;
        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        [Sushi.Mediakiwi.Framework.ContentListItem.TextField("Description", 500, false, Expression = Sushi.Mediakiwi.Framework.OutputExpression.Alternating)]
        [DatabaseColumn("ComponentTemplate_Description", SqlDbType.NVarChar, Length = 500, IsNullable = true)]
        public string Description
        {
            get { return m_Description; }
            set { m_Description = value; }
        }

        private bool m_CanReplicate;
        /// <summary>
        /// Gets or sets a value indicating whether this instance can replicate.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can replicate; otherwise, <c>false</c>.
        /// </value>
        [Sushi.Mediakiwi.Framework.ContentListItem.Choice_Checkbox("Can replicate", "If set to true this component has the option to create new instances on the page", Expression = Sushi.Mediakiwi.Framework.OutputExpression.Alternating)]
        [DatabaseColumn("ComponentTemplate_CanReplicate", SqlDbType.Bit)]
        public bool CanReplicate
        {
            get { return m_CanReplicate; }
            set { m_CanReplicate = value; }
        }


        /// <summary>
        /// Gets or sets the cache level.
        /// 0 = no cache, 1 = component based cache, 2 = page level cache
        /// </summary>
        /// <value>The cache level.</value>
        [Sushi.Mediakiwi.Framework.OnlyVisibleWhenFalse("IS_HEADLESS")]
        [DatabaseColumn("ComponentTemplate_CacheLevel", SqlDbType.Int, IsNullable = true)]
        [Sushi.Mediakiwi.Framework.ContentListItem.Choice_Dropdown("Cache level", "CacheTypes", Expression = Sushi.Mediakiwi.Framework.OutputExpression.Alternating)]
        public int CacheLevel { get; set; }


        /// <summary>
        /// Gets the cache level info.
        /// </summary>
        /// <value>The cache level info.</value>
        public string CacheLevelInfo
        {
            get
            {

                switch (CacheLevel)
                {
                    default: return "None";
                    case 1: return "Component";
                    case 2: return "Page";
                }
            }

        }

        string m_OutputCacheParams;
        /// <summary>
        /// Gets or sets the output cache params.
        /// </summary>
        /// <value>The output cache params.</value>
        //[Sushi.Mediakiwi.Framework.ContentListItem.TextField("Caching - Vary by parameters", 50, false)]
        [Sushi.Mediakiwi.Framework.OnlyVisibleWhenFalse("IS_HEADLESS")]
        [DatabaseColumn("ComponentTemplate_CacheParams", SqlDbType.VarChar, Length = 50, IsNullable = true)]
        public string OutputCacheParams
        {
            get { return m_OutputCacheParams; }
            set { m_OutputCacheParams = value; }
        }



        /// </summary>
        [Sushi.Mediakiwi.Framework.ContentListItem.Choice_Checkbox("Is shared", InteractiveHelp = "Is the content of this component shared amongst pages", Expression = Sushi.Mediakiwi.Framework.OutputExpression.Alternating)]
        [DatabaseColumn("ComponentTemplate_IsShared", SqlDbType.Bit)]
        public bool IsShared { get; set; }

        private bool m_CanDeactivate;
        /// <summary>
        /// Gets or sets a value indicating whether this instance can deactivate.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can deactivate; otherwise, <c>false</c>.
        /// </value>
        [Sushi.Mediakiwi.Framework.ContentListItem.Choice_Checkbox("Can deactivate", "If set to true this component can be set to invisible on the page", Expression = Sushi.Mediakiwi.Framework.OutputExpression.Alternating)]
        [DatabaseColumn("ComponentTemplate_CanDeactivate", SqlDbType.Bit)]
        public bool CanDeactivate
        {
            get { return m_CanDeactivate; }
            set { m_CanDeactivate = value; }
        }

        /// <summary>
        /// Gets or sets the type of the ajax.
        /// </summary>
        /// <value>The type of the ajax.</value>
        [Sushi.Mediakiwi.Framework.OnlyVisibleWhenFalse("IS_HEADLESS")]
        [Sushi.Mediakiwi.Framework.ContentListItem.Choice_Dropdown("Cache type", "AjaxTypes", Expression = Sushi.Mediakiwi.Framework.OutputExpression.Alternating)]
        [DatabaseColumn("ComponentTemplate_AjaxType", SqlDbType.Int, IsNullable = true)]
        public int AjaxType { get; set; }


        public bool HasAjaxCall
        {
            get { return this.AjaxType > 0; }
        }

        private bool m_IsHeader;
        /// <summary>
        /// Gets or sets a value indicating whether this instance is header.
        /// </summary>
        /// <value><c>true</c> if this instance is header; otherwise, <c>false</c>.</value>
        [Sushi.Mediakiwi.Framework.ContentListItem.Choice_Checkbox("Is header", "If set to true this component will always show up first on the page container", Expression = Sushi.Mediakiwi.Framework.OutputExpression.Alternating)]
        [DatabaseColumn("ComponentTemplate_IsHeader", SqlDbType.Bit)]
        public bool IsHeader
        {
            get { return m_IsHeader; }
            set { m_IsHeader = value; }
        }


        private bool m_CanMoveUpDown;
        /// <summary>
        /// Gets or sets a value indicating whether this instance can move up down.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can move up down; otherwise, <c>false</c>.
        /// </value>
        [Sushi.Mediakiwi.Framework.ContentListItem.Choice_Checkbox("Can move", "If set to true this component can be moved up and/or down", Expression = Sushi.Mediakiwi.Framework.OutputExpression.Alternating)]
        [DatabaseColumn("ComponentTemplate_CanMove", SqlDbType.Bit)]
        public bool CanMoveUpDown
        {
            get { return m_CanMoveUpDown; }
            set { m_CanMoveUpDown = value; }
        }


        private bool m_IsFooter;
        /// <summary>
        /// Gets or sets a value indicating whether this instance is footer.
        /// </summary>
        /// <value><c>true</c> if this instance is footer; otherwise, <c>false</c>.</value>
        [Sushi.Mediakiwi.Framework.ContentListItem.Choice_Checkbox("Is footer", "If set to true this component will always show up last on the page container", Expression = Sushi.Mediakiwi.Framework.OutputExpression.Alternating)]
        [DatabaseColumn("ComponentTemplate_IsFooter", SqlDbType.Bit)]
        public bool IsFooter
        {
            get { return m_IsFooter; }
            set { m_IsFooter = value; }
        }

        bool m_IsSecundaryContainerItem;
        /// <summary>
        /// Is this component template build for the secundary (mostly the service column) container?
        /// Only used if applied as page component. 
        /// </summary>
        [Sushi.Mediakiwi.Framework.OnlyVisibleWhenFalse("IS_HEADLESS")]
        [Sushi.Mediakiwi.Framework.ContentListItem.Choice_Checkbox("Service column", Expression = Sushi.Mediakiwi.Framework.OutputExpression.Alternating)]
        [DatabaseColumn("ComponentTemplate_IsSecundaryContainerItem", SqlDbType.Bit)]
        public bool IsSecundaryContainerItem
        {
            get { return m_IsSecundaryContainerItem; }
            set { m_IsSecundaryContainerItem = value; }
        }



        private Guid m_GUID;
        /// <summary>
        /// Unique identifier used for import/export
        /// </summary>
        [DatabaseColumn("ComponentTemplate_GUID", SqlDbType.UniqueIdentifier, IsMigrationKey = true)]
        public Guid GUID
        {
            get
            {
                if (this.m_GUID == Guid.Empty) this.m_GUID = Guid.NewGuid();
                return m_GUID;
            }
            set { m_GUID = value; }
        }

        bool m_IsListTemplate;
        /// <summary>
        /// Is this template basis for a list template?
        /// </summary>
        [DatabaseColumn("ComponentTemplate_IsListTemplate", SqlDbType.Bit)]
        public bool IsListTemplate
        {
            get { return m_IsListTemplate; }
            set { m_IsListTemplate = value; }
        }

        private string m_MetaData;
        /// <summary>
        /// Gets or sets the meta data.
        /// </summary>
        /// <value>The meta data.</value>
        //[Sushi.Mediakiwi.Framework.ContentListItem.TextArea("Metadata", 0, IsSourceCode = true, IsXml = true)]
        [DatabaseColumn("ComponentTemplate_MetaData", SqlDbType.NText, IsNullable = true)]
        public string MetaData
        {
            get { return m_MetaData; }
            set { m_MetaData = value; }
        }

        DateTime m_LastWriteTimeUtc;
        /// <summary>
        /// Gets or sets the last write time UTC of the page template (ASPX).
        /// </summary>
        /// <value>The last write time UTC.</value>
        [DatabaseColumn("ComponentTemplate_LastWriteTimeUtc", SqlDbType.DateTime)]
        public DateTime LastWriteTimeUtc
        {
            get { return m_LastWriteTimeUtc; }
            set { m_LastWriteTimeUtc = value; }
        }

        //[Sushi.Mediakiwi.Framework.ContentListItem.TextArea("Source2", 0, IsSourceCode = true)]
        public string Source2 { get; set; }

        public DateTime? Updated
        {
            get { return LastWriteTimeUtc; }
        }

        /// <summary>
        /// Selects the one.
        /// </summary>
        /// <param name="ID">The ID.</param>
        /// <returns></returns>
        public static ComponentTemplate SelectOne(int ID)
        {
            return (ComponentTemplate)new ComponentTemplate()._SelectOne(ID);
        }

        /// <summary>
        /// Select a Component Template instance based on its migration (GUID) key
        /// </summary>
        /// <param name="componentTemplateGUID">The component template GUID.</param>
        /// <returns></returns>
        public static ComponentTemplate SelectOne(Guid componentTemplateGUID)
        {
            ComponentTemplate item = new ComponentTemplate();
            List<DatabaseDataValueColumn> list = new List<DatabaseDataValueColumn>();
            list.Add(new DatabaseDataValueColumn(item.MigrationKeyColumn, SqlDbType.UniqueIdentifier, componentTemplateGUID));
            return (ComponentTemplate)item._SelectOne(list);
        }


        /// <summary>
        /// Selects the one.
        /// </summary>
        /// <param name="reference">The reference.</param>
        /// <returns></returns>
        public static ComponentTemplate SelectOneByReference(string reference)
        {
            ComponentTemplate item = new ComponentTemplate();
            List<DatabaseDataValueColumn> list = new List<DatabaseDataValueColumn>();
            list.Add(new DatabaseDataValueColumn("ComponentTemplate_ReferenceId", SqlDbType.NVarChar, reference));
            return (ComponentTemplate)item._SelectOne(list);
        }

        public static ComponentTemplate SelectOneBySourceTag(string tag)
        {
            ComponentTemplate item = new ComponentTemplate();
            List<DatabaseDataValueColumn> list = new List<DatabaseDataValueColumn>();
            list.Add(new DatabaseDataValueColumn("ComponentTemplate_Tag", SqlDbType.VarChar, tag));
            return (ComponentTemplate)item._SelectOne(list);
        }

        /// <summary>
        /// Selects all.
        /// </summary>
        /// <returns></returns>
        public static ComponentTemplate[] SelectAll()
        {
            List<ComponentTemplate> list = new List<ComponentTemplate>();
            ComponentTemplate componentTemplate = new ComponentTemplate();
            if (!string.IsNullOrEmpty(SqlConnectionString2)) componentTemplate.SqlConnectionString = SqlConnectionString2;
            foreach (object o in componentTemplate._SelectAll(true))
                list.Add((ComponentTemplate)o);
            return list.ToArray();
        }

        /// <summary>
        /// Selects the all_ available.
        /// </summary>
        /// <param name="pageTemplateID">The page template ID.</param>
        /// <returns></returns>
        public static Sushi.Mediakiwi.Data.ComponentTemplate[] SelectAllAvailable(int pageTemplateID)
        {
            //  Get all templates
            var all_template = SelectAll();
            //  Get all available templates
            var all_availableTemplate = AvailableTemplate.SelectAll();
            //  Select all available templates based on the page template ID
            var sel_availableTemplate =
                from availableTemplate in all_availableTemplate
                where availableTemplate.PageTemplateID == pageTemplateID && availableTemplate.IsPossible == true
                select availableTemplate;
            //  Select all templates based on the available templates
            var sel_template =
                from template in all_template
                join availableTemplate in sel_availableTemplate on template.ID equals availableTemplate.ComponentTemplateID
                select template;

            return sel_template.ToArray();
        }

        /// <summary>
        /// Selects the all available.
        /// </summary>
        /// <param name="pageTemplateId">The page template id.</param>
        /// <param name="isSecundary">if set to <c>true</c> [is secundary].</param>
        /// <returns></returns>
        public static Sushi.Mediakiwi.Data.ComponentTemplate[] SelectAllAvailable(int pageTemplateId, bool isSecundary)
        {
            return SelectAllAvailable(pageTemplateId, isSecundary, false);
        }

        /// <summary>
        /// Selects the all available.
        /// </summary>
        /// <param name="pageTemplateId">The page template id.</param>
        /// <param name="isSecundary">if set to <c>true</c> [is secundary].</param>
        /// <param name="isPresent">if set to <c>true</c> [is present].</param>
        /// <returns></returns>
        public static ComponentTemplate[] SelectAllAvailable(int pageTemplateId, bool isSecundary, bool isPresent)
        {
            ComponentTemplate implement = new ComponentTemplate();
            List<DatabaseDataValueColumn> whereClause = new List<DatabaseDataValueColumn>();
            whereClause.Add(new DatabaseDataValueColumn("AvailableTemplates_IsSecundary", SqlDbType.Bit, isSecundary));
            whereClause.Add(new DatabaseDataValueColumn("AvailableTemplates_IsPresent", SqlDbType.Bit, isPresent));
            whereClause.Add(new DatabaseDataValueColumn("AvailableTemplates_IsPossible", SqlDbType.Bit, true));
            whereClause.Add(new DatabaseDataValueColumn("AvailableTemplates_PageTemplate_Key", SqlDbType.Int, pageTemplateId));

            implement.SqlJoin = "join wim_AvailableTemplates on AvailableTemplates_ComponentTemplate_Key = ComponentTemplate_Key";

            List<ComponentTemplate> list = new List<ComponentTemplate>();
            foreach (object o in implement._SelectAll(whereClause))
                list.Add((ComponentTemplate)o);
            return list.ToArray();
        }

        /// <summary>
        /// Select all the newly assigned (available) component templates
        /// </summary>
        /// <param name="pageTemplateId">The page template id.</param>
        /// <param name="pageId">The page id.</param>
        /// <returns></returns>
        internal static ComponentTemplate[] SelectAll(int pageTemplateId, int pageId)
        {
            ComponentTemplate implement = new ComponentTemplate();
            implement.SqlJoin = string.Format(@"
 join wim_AvailableTemplates on AvailableTemplates_ComponentTemplate_Key = ComponentTemplate_Key 
 left join wim_ComponentVersions on ComponentTemplate_Key = ComponentVersion_ComponentTemplate_Key and ComponentVersion_Page_Key = {0}
"
                , pageId);

            List<DatabaseDataValueColumn> whereClause = new List<DatabaseDataValueColumn>();
            whereClause.Add(new DatabaseDataValueColumn("AvailableTemplates_PageTemplate_Key", SqlDbType.Int, pageTemplateId));
            whereClause.Add(new DatabaseDataValueColumn("ComponentVersion_Key", SqlDbType.Int, null));

            List<ComponentTemplate> list = new List<ComponentTemplate>();
            foreach (object o in implement._SelectAll(whereClause))
                list.Add((ComponentTemplate)o);
            return list.ToArray();
        }

        public static ComponentTemplate[] SelectAllShared()
        {
            ComponentTemplate implement = new ComponentTemplate();
            List<DatabaseDataValueColumn> whereClause = new List<DatabaseDataValueColumn>();
            whereClause.Add(new DatabaseDataValueColumn("ComponentTemplate_IsShared", SqlDbType.Bit, true));

            //implement.SqlJoin = "join wim_AvailableTemplates on AvailableTemplates_ComponentTemplate_Key = ComponentTemplate_Key";

            List<ComponentTemplate> list = new List<ComponentTemplate>();
            foreach (object o in implement._SelectAll(whereClause))
                list.Add((ComponentTemplate)o);
            return list.ToArray();
        }

        /// <summary>
        /// Select a Component Template instance based on its type
        /// </summary>
        public static ComponentTemplate SelectOne_BasedOnType(System.Type type)
        {
            //if (Wim.CommonConfiguration.ForceStaticLists)
            //{
            //    foreach (ComponentTemplate i in AllItems)
            //        if (i.TypeDefinition == type.ToString()) return i;
            //    return new ComponentTemplate();
            //}
            List<DatabaseDataValueColumn> list = new List<DatabaseDataValueColumn>();
            list.Add(new DatabaseDataValueColumn("ComponentTemplate_Type", SqlDbType.NVarChar, type.ToString(), 250));
            ComponentTemplate ct = (ComponentTemplate)new ComponentTemplate()._SelectOne(list, "TemplateType", type.ToString());

            return ct;
        }

        /// <summary>
        /// Save a database entity. This method will indentify the need for an INSERT or an UPDATE.
        /// </summary>
        /// <returns></returns>
        public override bool Save()
        {
            bool isSaved = base.Save();
            return isSaved;
        }

        /// <summary>
        /// Delete an implementation record.
        /// </summary>
        /// <returns></returns>
        public override bool Delete()
        {
            Execute(string.Concat("delete from wim_ComponentSearch where ComponentSearch_Ref_Key = ", this.ID));
            Execute(string.Concat("delete from wim_Components where Component_ComponentTemplate_Key = ", this.ID));
            Execute(string.Concat("delete from wim_ComponentVersions where ComponentVersion_ComponentTemplate_Key = ", this.ID));
            Execute(string.Concat("delete from wim_AvailableTemplates where AvailableTemplates_ComponentTemplate_Key = ", this.ID));
            return base.Delete();
        }

        #endregion REPLICATED TO Sushi.Mediakiwi.Data.Standard

    }
}
