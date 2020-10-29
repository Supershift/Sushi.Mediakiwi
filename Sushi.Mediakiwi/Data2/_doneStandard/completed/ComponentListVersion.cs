using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Sushi.Mediakiwi.Data.DalReflection;

namespace Sushi.Mediakiwi.Data
{
    /// <summary>
    /// Represents a ComponentListVersion entity.
    /// </summary>
    [DatabaseTable("wim_ComponentListVersions", Order = "ComponentListVersion_Key DESC")]
    public class ComponentListVersion : DatabaseEntity, iExportable
    {
        #region MOVED to EXTENSION / LOGIC

        //private Sushi.Mediakiwi.Data.Content m_content;
        ///// <summary>
        ///// Gets the property value.
        ///// </summary>
        ///// <param name="propertyName">Name of the property.</param>
        ///// <returns></returns>
        //public new string GetPropertyValue(string propertyName)
        //{
        //    if (m_content == null)
        //        m_content = Sushi.Mediakiwi.Data.Content.GetDeserialized(Serialized_XML);

        //    if (m_content == null)
        //        return null;

        //    if (m_content.Fields == null)
        //        return null;

        //    foreach (Sushi.Mediakiwi.Data.Content.Field field in m_content.Fields)
        //    {
        //        if (field.Property == propertyName)
        //        {
        //            if (field.Type == (int)Sushi.Mediakiwi.Framework.ContentType.RichText)
        //            {
        //                //Sushi.Mediakiwi.Framework.Templates.RichLink richLink =
        //                //    new Sushi.Mediakiwi.Framework.Templates.RichLink();

        //                string candidate = Wim.Utility.ApplyRichtextLinks(null, field.Value);
        //                //string candidate = Sushi.Mediakiwi.Framework.Templates.RichLink.GetCleaner.Replace(field.Value, richLink.CleanLinkInformation);
        //                return candidate;
        //            }
        //            return field.Value;
        //        }
        //    }
        //    return null;
        //}

        #endregion MOVED to EXTENSION / LOGIC

        #region REPLICATED TO Sushi.Mediakiwi.Data.Standard

        private int m_ID;
        /// <summary>
        /// Gets or sets the ID.
        /// </summary>
        /// <value>The ID.</value>
        [DatabaseColumn("ComponentListVersion_Key", SqlDbType.Int, IsPrimaryKey = true)]
        public int ID 
		{
            get { return m_ID; }
            set { m_ID = value; }
		}

        private int m_ApplicationUserID;
        /// <summary>
        /// Gets or sets the application user ID.
        /// </summary>
        /// <value>The application user ID.</value>
        [DatabaseColumn("ComponentListVersion_User_Key", SqlDbType.Int, IsNullable = true)]
        public int ApplicationUserID
        {
            get { return m_ApplicationUserID; }
            set { m_ApplicationUserID = value; }
        }

        private int m_Version;
        /// <summary>
        /// Gets or sets the application user ID.
        /// </summary>
        /// <value>The application user ID.</value>
        [DatabaseColumn("ComponentListVersion_Version", SqlDbType.Int, IsNullable = true)]
        public int Version
        {
            get { return m_Version; }
            set { m_Version = value; }
        }

        /// <summary>
        /// Save a database entity. This method will indentify the need for an INSERT or an UPDATE.
        /// </summary>
        /// <returns></returns>
        public override bool Save()
        {
            this.IsActive = true;
            this.Version = Wim.Utility.ConvertToInt(Execute(string.Format("select isnull(MAX(ComponentListVersion_Version), 0) +1 from wim_ComponentListVersions where ComponentListVersion_Site_Key = {0} and ComponentListVersion_ComponentList_Key = {1} and ComponentListVersion_Listitem_Key = {2}", this.SiteID, this.ComponentListID, ComponentListItemID)));
            bool isSaved = base.Save();
            Execute(string.Format("update wim_ComponentListVersions set ComponentListVersion_IsActive = 0 where ComponentListVersion_Site_Key = {0} and ComponentListVersion_ComponentList_Key = {1} and ComponentListVersion_Listitem_Key = {2} and ComponentListVersion_Version < {3}", this.SiteID, this.ComponentListID, ComponentListItemID, this.Version));
            return isSaved;
        }

        private int m_TypeID;
        /// <summary>
        /// Gets or sets the type ID ( 0 = created. 1 = updated, 2 = deleted).
        /// </summary>
        /// <value>The type ID.</value>
        [DatabaseColumn("ComponentListVersion_Type", SqlDbType.Int, IsNullable = true)]
        public int TypeID
        {
            get { return m_TypeID; }
            set { m_TypeID = value; }
        }

        //private int m_PageID;
        ///// <summary>
        ///// Gets or sets the application user ID.
        ///// </summary>
        ///// <value>The application user ID.</value>
        //[DatabaseColumn("ComponentVersion_Page_Key", SqlDbType.Int)]
        //public int PageID
        //{
        //    get { return m_PageID; }
        //    set { m_PageID = value; }
        //}

		private int m_SiteID;
        /// <summary>
        /// Gets or sets the site ID.
        /// </summary>
        /// <value>The site ID.</value>
        [DatabaseColumn("ComponentListVersion_Site_Key", SqlDbType.Int, IsNullable = true)]
        public int SiteID 
		{
            get { return m_SiteID; }
            set { m_SiteID = value; }
		}

        private int m_ComponentListID;
        /// <summary>
        /// Gets or sets the component list ID.
        /// </summary>
        /// <value>The component list ID.</value>
        [DatabaseColumn("ComponentListVersion_ComponentList_Key", SqlDbType.Int)]
        public int ComponentListID 
		{
            get { return m_ComponentListID; }
            set { m_ComponentListID = value; }
		}
        
		private int m_ComponentListItemID;
        /// <summary>
        /// Gets or sets the component list item ID.
        /// </summary>
        /// <value>The component list item ID.</value>
        [DatabaseColumn("ComponentListVersion_Listitem_Key", SqlDbType.Int)]
        public int ComponentListItemID 
		{
            get { return m_ComponentListItemID; }
            set { m_ComponentListItemID = value; }
		}
        
		private string m_Serialized_XML;
        /// <summary>
        /// Gets or sets the serialized_ XML.
        /// </summary>
        /// <value>The serialized_ XML.</value>
        [DatabaseColumn("ComponentListVersion_XML", SqlDbType.NText, IsNullable = true)]
        public string Serialized_XML 
		{
			get { return m_Serialized_XML; }
            set { m_Serialized_XML = value; }
		}
        
		private string m_DescriptiveTag;
        /// <summary>
        /// Gets or sets the descriptive tag.
        /// </summary>
        /// <value>The descriptive tag.</value>
        [DatabaseColumn("ComponentListVersion_DescriptionTag", SqlDbType.NVarChar, Length = 100, IsNullable = true)]
        public string DescriptiveTag 
		{
			get { return m_DescriptiveTag; }
            set { m_DescriptiveTag = value; }
            
		}
        
		private DateTime m_Created;
        /// <summary>
        /// Gets the created.
        /// </summary>
        /// <value>The created.</value>
        [DatabaseColumn("ComponentListVersion_Created", SqlDbType.DateTime)]
        public DateTime Created 
		{
			get {
                if (this.m_Created == DateTime.MinValue) this.m_Created = Sushi.Mediakiwi.Data.Common.DatabaseDateTime;
                return m_Created; }
            set { m_Created = value; }
		}


        private bool m_IsActive;
        /// <summary>
        /// Gets or sets a value indicating whether this instance is active.
        /// </summary>
        /// <value><c>true</c> if this instance is active; otherwise, <c>false</c>.</value>
        [DatabaseColumn("ComponentListVersion_IsActive", SqlDbType.Bit)]
        public bool IsActive
        {
            get { return m_IsActive; }
            set { m_IsActive = value; }
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

        /// <summary>
        /// Select a single ComponentlistVersion object
        /// </summary>
        /// <param name="componentlistVersionID">The componentlist version ID.</param>
        /// <returns>ComponentlistVersion object</returns>
        public static Sushi.Mediakiwi.Data.ComponentListVersion SelectOne(int componentlistVersionID)
        {
            List<DatabaseDataValueColumn> list = new List<DatabaseDataValueColumn>();
            list.Add(new DatabaseDataValueColumn("ComponentListVersion_Key", SqlDbType.Int, componentlistVersionID));
            return (ComponentListVersion)new ComponentListVersion()._SelectOne(list);
        }

        /// <summary>
        /// Select the last introduced ComponentlistVersion object based on the componentlist Key
        /// and the componentListVersionItemKey (if the componentlist is set as for single page configuration, this key is always the siteId)
        /// </summary>
        /// <param name="componentListID">The component list ID.</param>
        /// <param name="componentListVersionItemID">The component list version item ID.</param>
        /// <returns>ComponentlistVersion object</returns>
        public static Sushi.Mediakiwi.Data.ComponentListVersion SelectOne(int componentListID, int componentListVersionItemID)
        {
            if (componentListVersionItemID < 1)
                return new ComponentListVersion();

            List<DatabaseDataValueColumn> list;
            list = new List<DatabaseDataValueColumn>();
            list.Add(new DatabaseDataValueColumn("ComponentListVersion_ComponentList_Key", SqlDbType.Int, componentListID));
            list.Add(new DatabaseDataValueColumn("ComponentListVersion_Listitem_Key", SqlDbType.Int, componentListVersionItemID));

            string value = string.Concat(componentListID, "_", componentListVersionItemID);
            return (ComponentListVersion)new ComponentListVersion()._SelectOne(list, null, null);
        }

        /// <summary>
        /// Selects the one.
        /// </summary>
        /// <param name="siteID">The site ID.</param>
        /// <param name="componentListID">The component list ID.</param>
        /// <param name="componentListVersionItemID">The component list version item ID.</param>
        /// <returns></returns>
        public static Sushi.Mediakiwi.Data.ComponentListVersion SelectOne(int siteID, int componentListID, int componentListVersionItemID)
        {
            List<DatabaseDataValueColumn> list;
            list = new List<DatabaseDataValueColumn>();
            list.Add(new DatabaseDataValueColumn("ComponentListVersion_Site_Key", SqlDbType.Int, siteID)); // Changed 16-3-12 by casper; instead of siteID, compentListID was used here... strange
            list.Add(new DatabaseDataValueColumn("ComponentListVersion_ComponentList_Key", SqlDbType.Int, componentListID));
            list.Add(new DatabaseDataValueColumn("ComponentListVersion_Listitem_Key", SqlDbType.Int, componentListVersionItemID));

            string value = string.Concat(componentListID, "_", componentListVersionItemID);
            return (ComponentListVersion)new ComponentListVersion()._SelectOne(list);
        }

        /// <summary>
        /// Select all active ComponentlistVersion objects having a Descriptive tag
        /// </summary>
        /// <param name="componentListID">The component list ID.</param>
        /// <param name="siteID">The site ID.</param>
        /// <returns>array of ComponentListVersion</returns>
        public static Sushi.Mediakiwi.Data.ComponentListVersion[] SelectAll(int componentListID, int siteID)
        {
            List<ComponentListVersion> list = new List<ComponentListVersion>();

            List<DatabaseDataValueColumn> whereClause = new List<DatabaseDataValueColumn>();
            whereClause.Add(new DatabaseDataValueColumn("ComponentListVersion_IsActive", SqlDbType.Bit, true));
            whereClause.Add(new DatabaseDataValueColumn("NOT componentlistversion_DescriptionTag", SqlDbType.NVarChar, null));
            whereClause.Add(new DatabaseDataValueColumn("ComponentListVersion_ComponentList_Key", SqlDbType.Int, componentListID));
            whereClause.Add(new DatabaseDataValueColumn("ComponentListVersion_Site_Key", SqlDbType.Int, siteID));
            whereClause.Add(new DatabaseDataValueColumn("ComponentListVersion_Site_Key", SqlDbType.Int, null, DatabaseDataValueConnectType.Or));
            whereClause.Add(new DatabaseDataValueColumn("NOT ComponentListVersion_Listitem_Key", SqlDbType.Int, 0));
            whereClause.Add(new DatabaseDataValueColumn("NOT ComponentListVersion_XML", SqlDbType.NText, null));

            foreach (object o in new ComponentListVersion()._SelectAll(whereClause))
                list.Add((ComponentListVersion)o);
            return list.ToArray();
        }

        /// <summary>
        /// Delete an implementation record.
        /// </summary>
        /// <returns></returns>
        public override bool Delete()
        {
            //ComponentListVersion_IsActive = 0 
            this.IsActive = false;
            return Save();
        }

        #region iExportable Members


        public Guid GUID
        {
            get
            {
                return Guid.NewGuid();
            }
            set
            {
              
            }
        }

        public DateTime? Updated
        {
            get { return DateTime.Now; }
        }

        #endregion


        /// <summary>
        /// Select the last componentlist version based on the ComponentList Identifier (GUID)
        /// </summary>
        /// <param name="componentListVersionGUID">The component list version GUID.</param>
        /// <param name="siteID">The site ID.</param>
        /// <returns></returns>
        public static Sushi.Mediakiwi.Data.ComponentListVersion SelectOne(Guid componentListVersionGUID, int siteID)        //NAAR STANDARD
        {
            ComponentListVersion implement = new ComponentListVersion();
            implement.SqlJoin = "join wim_ComponentLists on ComponentListVersion_ComponentList_Key = ComponentList_Key";

            List<DatabaseDataValueColumn> list = new List<DatabaseDataValueColumn>();
            list.Add(new DatabaseDataValueColumn("ComponentList_GUID", SqlDbType.UniqueIdentifier, componentListVersionGUID));
            list.Add(new DatabaseDataValueColumn("ComponentListVersion_Site_Key", SqlDbType.Int, siteID));
            return (ComponentListVersion)implement._SelectOne(list);
        }

        /// <summary>
        /// Select the last componentlist version based on the componentTemplate key
        /// and the componentListVersionItemKey (if the componentlist is set as for single page configuration, this key is always the siteId)
        /// </summary>
        /// <param name="componentTemplateID">The component template ID.</param>
        /// <param name="componentListVersionItemID">The component list version item ID.</param>
        /// <returns>ComponentlistVersion object</returns>
        public static Sushi.Mediakiwi.Data.ComponentListVersion SelectOneByTemplate(int componentTemplateID, int componentListVersionItemID)        //NAAR STANDRD
        {
            ComponentListVersion implement = new ComponentListVersion();
            implement.SqlJoin = "join wim_ComponentLists on ComponentListVersion_ComponentList_Key = ComponentList_Key";

            List<DatabaseDataValueColumn> list = new List<DatabaseDataValueColumn>();
            list.Add(new DatabaseDataValueColumn("ComponentList_ComponentTemplate_Key", SqlDbType.Int, componentTemplateID));
            list.Add(new DatabaseDataValueColumn("ComponentListVersion_Listitem_Key", SqlDbType.Int, componentListVersionItemID));
            return (ComponentListVersion)implement._SelectOne(list);
        }

        #endregion REPLICATED TO Sushi.Mediakiwi.Data.Standard
    }
}
