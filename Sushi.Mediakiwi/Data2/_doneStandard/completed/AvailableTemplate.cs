using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Sushi.Mediakiwi.Data.DalReflection;
using Sushi.Mediakiwi.Data.Parsers;

namespace Sushi.Mediakiwi.Data
{
    /// <summary>
    /// Represents a AvailableTemplate entity.
    /// </summary>
    [DatabaseTable("wim_AvailableTemplates", 
        Join = "join wim_ComponentTemplates on ComponentTemplate_Key = AvailableTemplates_ComponentTemplate_Key", 
        Order = "AvailableTemplates_SortOrder ASC")
    ]
    public class AvailableTemplate : IAvailableTemplate, iExportable, IComponent
    {
        internal bool Checked;

        static IAvailableTemplateParser _Parser;
        static IAvailableTemplateParser Parser
        {
            get
            {
                if (_Parser == null)
                    _Parser = Environment.GetInstance<IAvailableTemplateParser>();
                return _Parser;
            }
        }

        #region REPLICATED TO Sushi.Mediakiwi.Data.Standard


        /// <summary>
        /// Gets or sets the ID.
        /// </summary>
        /// <value>The ID.</value>
        [DatabaseColumn("AvailableTemplates_Key", SqlDbType.Int, IsPrimaryKey = true)]
        public int ID { get; set; }

        private Guid m_GUID;
        /// <summary>
        /// Gets or sets the GUID.
        /// </summary>
        /// <value>The GUID.</value>
        [DatabaseColumn("AvailableTemplates_Guid", SqlDbType.UniqueIdentifier, IsMigrationKey = true)]
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
        /// Gets or sets the page template ID.
        /// </summary>
        /// <value>The page template ID.</value>
        [DatabaseColumn("AvailableTemplates_PageTemplate_Key", SqlDbType.Int)]
        public int PageTemplateID { get; set; }

        /// <summary>
        /// Gets or sets the component template ID.
        /// </summary>
        /// <value>The component template ID.</value>
        [DatabaseColumn("AvailableTemplates_ComponentTemplate_Key", SqlDbType.Int)]
        public int ComponentTemplateID { get; set; }

        /// <summary>
        /// Gets or sets the component template.
        /// </summary>
        /// <value>The component template.</value>
        [DatabaseColumn("ComponentTemplate_Name", SqlDbType.NVarChar, Length = 50, IsOnlyRead = true)]
        public string ComponentTemplate { get; set; }

        [DatabaseColumn("AvailableTemplates_Target", SqlDbType.VarChar, Length = 25, IsNullable = true)]
        public string Target { get; set; }

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
                    m_Template = Sushi.Mediakiwi.Data.ComponentTemplate.SelectOne(this.ComponentTemplateID);
                return m_Template;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is possible.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is possible; otherwise, <c>false</c>.
        /// </value>
        [DatabaseColumn("AvailableTemplates_IsPossible", SqlDbType.Bit)]
        public bool IsPossible { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is secundary.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is secundary; otherwise, <c>false</c>.
        /// </value>
        [DatabaseColumn("AvailableTemplates_IsSecundary", SqlDbType.Bit)]
        public bool IsSecundary { get; set; }

        //byte[] m_Timestamp;
        ///// <summary>
        ///// Gets or sets the timestamp.
        ///// </summary>
        ///// <value>The timestamp.</value>
        //[DatabaseColumn("AvailableTemplates_Timestamp", SqlDbType.Timestamp, IsOnlyRead = true)]
        //public byte[] Timestamp
        //{
        //    get { return m_Timestamp; }
        //    set { m_Timestamp = value; }
        //}

        /// <summary>
        /// Gets or sets a value indicating whether this instance is present.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is present; otherwise, <c>false</c>.
        /// </value>
        [DatabaseColumn("AvailableTemplates_IsPresent", SqlDbType.Bit)]
        public bool IsPresent { get; set; }

        /// <summary>
        /// Gets or sets the sort order.
        /// </summary>
        /// <value>The sort order.</value>
        [DatabaseColumn("AvailableTemplates_SortOrder", SqlDbType.Int)]
        public int SortOrder { get; set; }

        /// <summary>
        /// Gets or sets the name of the fixed field.
        /// </summary>
        /// <value>The name of the fixed field.</value>
        [DatabaseColumn("AvailableTemplates_Fixed_Id", SqlDbType.NVarChar, Length = 50, IsNullable = true)]
        public string FixedFieldName { get; set; }

        [DatabaseColumn("AvailableTemplates_Slot", SqlDbType.Int)]
        public int SlotID { get; set; }

        /// <summary>
        /// Select all available sites
        /// </summary>
        /// <returns>Array of site objects</returns>
        public static IAvailableTemplate[] SelectAll()
        {
            return Parser.SelectAll();
        }

        /// <summary>
        /// Deletes the specified page template ID.
        /// </summary>
        /// <param name="pageTemplateID">The page template ID.</param>
        /// <param name="isSecundary">if set to <c>true</c> [is secundary].</param>
        /// <param name="target">The target.</param>
        public static void Delete(int pageTemplateID, bool isSecundary, string target)
        {
            Parser.Delete(pageTemplateID, isSecundary, target);
        }

        /// <summary>
        /// Deletes the specified page template ID.
        /// </summary>
        /// <param name="pageTemplateID">The page template ID.</param>
        /// <param name="portal">The portal.</param>
        public static void Delete(int pageTemplateID, string portal)
        {
            Parser.Delete(pageTemplateID, portal);
        }

        /// <summary>
        /// Selects all.
        /// </summary>
        /// <param name="pageTemplateID">The page template ID.</param>
        /// <returns></returns>
        public static IAvailableTemplate[] SelectAll(int pageTemplateID, string target = null)
        {
            return Parser.SelectAll(pageTemplateID, target);
        }

        public static IAvailableTemplate[] SelectAllBySlot(int slotID)
        {
            return Parser.SelectAllBySlot(slotID);
        }

        public static IAvailableTemplate[] SelectAllByComponentTemplate(int templateID)
        {
            return Parser.SelectAllByComponentTemplate(templateID);
        }

        /// <summary>
        /// Selects the all template that are currently not on the page.
        /// </summary>
        /// <param name="pageTemplateID">The page template ID.</param>
        /// <param name="pageID">The page ID.</param>
        /// <returns></returns>
        internal static IAvailableTemplate[] SelectAll(int pageTemplateID, int pageID, bool onlyReturnFixedInCode = false)
        {
            return Parser.SelectAll(pageTemplateID, pageID, onlyReturnFixedInCode);
        }

        /// <summary>
        /// Selects the one.
        /// </summary>
        /// <param name="availableTemplateID">The available template identifier.</param>
        /// <returns></returns>
        public static IAvailableTemplate SelectOne(int availableTemplateID)
        {
            return Parser.SelectOne(availableTemplateID);
        }

        /// <summary>
        /// Selects the one.
        /// </summary>
        /// <param name="pageTemplateID">The page template identifier.</param>
        /// <param name="fixedTag">The fixed tag.</param>
        /// <returns></returns>
        public static IAvailableTemplate SelectOne(int pageTemplateID, string fixedTag)
        {
            return Parser.SelectOne(pageTemplateID, fixedTag);
        }




        public DateTime? Updated
        {
            get
            {
                return null;
            }
        }

        public void Save()
        {
            Parser.Save(this);
        }

        public bool Delete()
        {
            return Parser.Delete(this);
        }

        public bool IsNewInstance { get { return ID == 0; } }


        #endregion REPLICATED TO Sushi.Mediakiwi.Data.Standard

    }
}
