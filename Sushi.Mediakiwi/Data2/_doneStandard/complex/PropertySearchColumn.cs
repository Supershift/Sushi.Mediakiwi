using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Sushi.Mediakiwi.Data.DalReflection;

namespace Sushi.Mediakiwi.Data
{
    /// <summary>
    /// 
    /// </summary>
    [DatabaseTable("wim_PropertySearchColumns", Order = "PropertySearchColumn_SortOrder")]
    public class PropertySearchColumn : DatabaseEntity 
    {
        /// <summary>
        /// Selects the all_ import export.
        /// </summary>
        /// <param name="portal">The portal.</param>
        /// <param name="componentlistID">The componentlist ID.</param>
        /// <returns></returns>
        public static List<PropertySearchColumn> SelectAll_ImportExport(string portal, int componentlistID)
        {
            List<DatabaseDataValueColumn> whereClause = new List<DatabaseDataValueColumn>();
            whereClause.Add(new DatabaseDataValueColumn("PropertySearchColumn_List_Key", SqlDbType.Int, componentlistID));

            PropertySearchColumn implement = new PropertySearchColumn();
            List<PropertySearchColumn> list = new List<PropertySearchColumn>();

            implement.SqlConnectionString = Sushi.Mediakiwi.Data.Common.GetPortal(portal).Connection;
            implement.ConnectionType = Sushi.Mediakiwi.Data.Common.GetPortal(portal).Type;

            foreach (object o in implement._SelectAll(whereClause))
                list.Add((PropertySearchColumn)o);

            return list;
        }

        /// <summary>
        /// Selects the all_ import export.
        /// </summary>
        /// <param name="portal">The portal.</param>
        /// <returns></returns>
        public static List<PropertySearchColumn> SelectAll_ImportExport(string portal)
        {
            PropertySearchColumn implement = new PropertySearchColumn();
            List<PropertySearchColumn> list = new List<PropertySearchColumn>();

            implement.SqlConnectionString = Sushi.Mediakiwi.Data.Common.GetPortal(portal).Connection;
            implement.ConnectionType = Sushi.Mediakiwi.Data.Common.GetPortal(portal).Type;

            foreach (object o in implement._SelectAll(null, false, "PropertySearchColumnImportExport", portal))
                list.Add((PropertySearchColumn)o);

            return list;
        }

        #region REPLICATED TO Sushi.Mediakiwi.Data.Standard

        /// <summary>
        /// Save a database entity. This method will indentify the need for an INSERT or an UPDATE.
        /// </summary>
        /// <returns></returns>
        public override bool Save()
        {
            bool shouldSetSortorder = (this.ID == 0);
            bool save = base.Save();

            if (shouldSetSortorder)
                new PropertySearchColumn().Execute(string.Concat(@"
update wim_PropertySearchColumns set PropertySearchColumn_SortOrder = PropertySearchColumn_Key where PropertySearchColumn_Key = ", this.ID));

            return save;
        }
        /// <summary>
        /// Selects the one.
        /// </summary>
        /// <param name="ID">The ID.</param>
        /// <returns></returns>
        public static PropertySearchColumn SelectOne(int ID)
        {
            return (PropertySearchColumn)new PropertySearchColumn()._SelectOne(ID);
        }

        /// <summary>
        /// Selects the one.
        /// </summary>
        /// <param name="componentlistID">The componentlist ID.</param>
        /// <param name="ID">The ID.</param>
        /// <returns></returns>
        public static PropertySearchColumn SelectOneHighlight(int componentlistID)
        {
            List<DatabaseDataValueColumn> whereClause = new List<DatabaseDataValueColumn>();
            whereClause.Add(new DatabaseDataValueColumn("PropertySearchColumn_List_Key", SqlDbType.Int, componentlistID));
            whereClause.Add(new DatabaseDataValueColumn("PropertySearchColumn_IsHighlight", SqlDbType.Bit, true));

            return (PropertySearchColumn)new PropertySearchColumn()._SelectOne(whereClause);
        }

        /// <summary>
        /// Selects all.
        /// </summary>
        /// <param name="componentlistID">The componentlist ID.</param>
        /// <returns></returns>
        public static Sushi.Mediakiwi.Data.PropertySearchColumn[] SelectAll(int componentlistID)
        {
            PropertySearchColumn implement = new PropertySearchColumn();
            List<DatabaseDataValueColumn> whereClause = new List<DatabaseDataValueColumn>();
            whereClause.Add(new DatabaseDataValueColumn("PropertySearchColumn_List_Key", SqlDbType.Int, componentlistID));

            List<PropertySearchColumn> list = new List<PropertySearchColumn>();
            foreach (object o in implement._SelectAll(whereClause))
                list.Add((PropertySearchColumn)o);
            return list.ToArray();
        }

        int m_ID;
        /// <summary>
        /// Gets or sets the ID.
        /// </summary>
        /// <value>The ID.</value>
        [DatabaseColumn("PropertySearchColumn_Key", SqlDbType.Int, IsPrimaryKey = true)]
        public int ID
        {
            get { return m_ID; }
            set { m_ID = value; }
        }

        int m_ListID;
        /// <summary>
        /// Gets or sets the list ID.
        /// </summary>
        /// <value>The list ID.</value>
        [DatabaseColumn("PropertySearchColumn_List_Key", SqlDbType.Int)]
        public int ListID
        {
            get { return m_ListID; }
            set { m_ListID = value; }
        }

        /// <summary>
        /// Clones me.
        /// </summary>
        /// <returns></returns>
        public PropertySearchColumn CloneMe()
        {
            return this.MemberwiseClone() as PropertySearchColumn;
        }

        private int m_PropertyID;
        /// <summary>
        /// Gets or sets the property ID.
        /// </summary>
        /// <value>The property ID.</value>
        [Sushi.Mediakiwi.Framework.ContentListItem.Choice_Dropdown("Property", "Properties", true, Expression = Sushi.Mediakiwi.Framework.OutputExpression.Alternating)]
        [DatabaseColumn("PropertySearchColumn_Property_Key", SqlDbType.Int)]
        public int PropertyID
        {
            get { return m_PropertyID; }
            set { m_PropertyID = value; }
        }

        Sushi.Mediakiwi.Data.Property m_Property;
        /// <summary>
        /// Gets the property.
        /// </summary>
        /// <value>The property.</value>
        public Sushi.Mediakiwi.Data.Property Property
        {
            get
            {
                if (m_Property == null)
                    m_Property = Sushi.Mediakiwi.Data.Property.SelectOne(this.PropertyID);
                return m_Property;
            }
        }


        /// <summary>
        /// Gets the display title.
        /// </summary>
        /// <value>The display title.</value>
        public string DisplayTitle
        {
            get
            {
                if (string.IsNullOrEmpty(this.Title))
                    return Property.Title;
                return this.Title;
            }
        }

        private int? m_ColumnWidth;
        /// <summary>
        /// Gets or sets the width of the column.
        /// </summary>
        /// <value>The width of the column.</value>
        [Sushi.Mediakiwi.Framework.ContentListItem.TextField("Column width", 4, false, Expression = Sushi.Mediakiwi.Framework.OutputExpression.Alternating)]
        [DatabaseColumn("PropertySearchColumn_Width", SqlDbType.Int, IsNullable = true)]
        public int? ColumnWidth
        {
            get { return m_ColumnWidth; }
            set { m_ColumnWidth = value; }
        }

        private string m_Title;
        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>The title.</value>
        [Sushi.Mediakiwi.Framework.ContentListItem.TextField("Override title", 50, false, Expression = Sushi.Mediakiwi.Framework.OutputExpression.Alternating)]
        [DatabaseColumn("PropertySearchColumn_Title", SqlDbType.NVarChar, Length = 50, IsNullable = true)]
        public string Title
        {
            get { return m_Title; }
            set { m_Title = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is highlight.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is highlight; otherwise, <c>false</c>.
        /// </value>
        [Sushi.Mediakiwi.Framework.ContentListItem.Choice_Checkbox("Is highlight", false, Expression = Sushi.Mediakiwi.Framework.OutputExpression.Alternating)]
        [DatabaseColumn("PropertySearchColumn_IsHighlight", SqlDbType.Bit)]
        public bool IsHighlight { get; set; }

        private int m_TotalType;
        /// <summary>
        /// Gets or sets the total type.
        /// </summary>
        /// <value>The total type.</value>
        [Sushi.Mediakiwi.Framework.ContentListItem.Choice_Dropdown("Type", "TotalTypes", true, Expression = Sushi.Mediakiwi.Framework.OutputExpression.Alternating)]
        [DatabaseColumn("PropertySearchColumn_TotalType", SqlDbType.Int)]
        public int TotalType
        {
            get { return m_TotalType; }
            set { m_TotalType = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is default export.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is default export; otherwise, <c>false</c>.
        /// </value>
        [Sushi.Mediakiwi.Framework.ContentListItem.Choice_Checkbox("Only export", false, Expression = Sushi.Mediakiwi.Framework.OutputExpression.Alternating)]
        [DatabaseColumn("PropertySearchColumn_IsExport", SqlDbType.Bit)]
        public bool IsOnlyExport { get; set; }

        #endregion REPLICATED TO Sushi.Mediakiwi.Data.Standard
    }
}
