using System;
using System.Data;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Text;
using Sushi.Mediakiwi.Data.DalReflection;

namespace Sushi.Mediakiwi.Data
{
    /// <summary>
    /// 
    /// </summary>
    [DatabaseTable("wim_PropertyOptions", Order = "PropertyOption_SortOrder ASC")]
    public class PropertyOption : DatabaseEntity
    {
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
                new PropertyOption().Execute(string.Concat(@"
update wim_PropertyOptions set PropertyOption_SortOrder = PropertyOption_Key where PropertyOption_Key = ", this.ID));

            return save;
        }

        /// <summary>
        /// Deletes the collection.
        /// </summary>
        /// <param name="formElementID">The form element ID.</param>
        /// <returns></returns>
        public static bool DeleteCollection(int formElementID)
        {
            if (formElementID == 0)
                return true;
            List<DatabaseDataValueColumn> whereClause = new List<DatabaseDataValueColumn>();
            whereClause.Add(new DatabaseDataValueColumn("PropertyOption_Property_Key", SqlDbType.Int, formElementID));

            return new PropertyOption().Delete(whereClause);
        }

        /// <summary>
        /// Selects the one.
        /// </summary>
        /// <param name="ID">The ID.</param>
        /// <returns></returns>
        public static PropertyOption SelectOne(int ID)
        {
            return (PropertyOption)new PropertyOption()._SelectOne(ID);
        }

        /// <summary>
        /// Selects all.
        /// </summary>
        /// <param name="propertyID">The property ID.</param>
        /// <returns></returns>
        public static PropertyOption[] SelectAll(int propertyID)
        {
            List<PropertyOption> list = new List<PropertyOption>();
            List<DatabaseDataValueColumn> whereClause = new List<DatabaseDataValueColumn>();
            whereClause.Add(new DatabaseDataValueColumn("PropertyOption_Property_Key", SqlDbType.Int, propertyID));

            foreach (object o in new PropertyOption()._SelectAll(whereClause)) list.Add((PropertyOption)o);
            return list.ToArray();
        }

        public static PropertyOption[] SelectAll()
        {
            List<PropertyOption> list = new List<PropertyOption>();
            foreach (object o in new PropertyOption()._SelectAll()) list.Add((PropertyOption)o);
            return list.ToArray();
        }

        //public static FormElementOption SelectOneByText(int elementID, string text)
        //{
        //    List<DatabaseDataValueColumn> whereClause = new List<DatabaseDataValueColumn>();
        //    whereClause.Add(new DatabaseDataValueColumn("FormElementOption_FormElement_Key", SqlDbType.Int, elementID));
        //    whereClause.Add(new DatabaseDataValueColumn("FormElementOption_Text", SqlDbType.NVarChar, text));
        //    return (FormElementOption)new FormElementOption()._SelectOne(whereClause);
        //}

        private int m_ID;
        /// <summary>
        /// Gets or sets the ID.
        /// </summary>
        /// <value>The ID.</value>
        [DatabaseColumn("PropertyOption_Key", SqlDbType.Int, IsPrimaryKey = true)]
        public int ID
        {
            get { return m_ID; }
            set { m_ID = value; }
        }

        private int m_PropertyID;
        /// <summary>
        /// Gets or sets the property ID.
        /// </summary>
        /// <value>The property ID.</value>
        [DatabaseColumn("PropertyOption_Property_Key", SqlDbType.Int, IsNullable = true)]
        public int PropertyID
        {
            get { return m_PropertyID; }
            set { m_PropertyID = value; }
        }

        private string m_Name;
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        [Sushi.Mediakiwi.Framework.ContentListItem.TextField("Text", 250, true, Expression = Sushi.Mediakiwi.Framework.OutputExpression.Alternating)]
        [DatabaseColumn("PropertyOption_Text", SqlDbType.NVarChar, Length = 250)]
        public string Name
        {
            get { return m_Name; }
            set { m_Name = value; }
        }

        private string m_Value;
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        [Sushi.Mediakiwi.Framework.ContentListItem.TextField("Value", 250, false, Expression = Sushi.Mediakiwi.Framework.OutputExpression.Alternating)]
        [DatabaseColumn("PropertyOption_Value", SqlDbType.NVarChar, Length = 250, IsNullable = true)]
        public string Value
        {
            get {
                return m_Value; 
            }
            set { m_Value = value; }
        }

        private int m_SortOrderID;
        /// <summary>
        /// Gets or sets the sort order ID.
        /// </summary>
        /// <value>The sort order ID.</value>
        [DatabaseColumn("PropertyOption_SortOrder", SqlDbType.Int, IsNullable = true)]
        public int SortOrderID
        {
            get { return m_SortOrderID; }
            set { m_SortOrderID = value; }
        }

        #endregion REPLICATED TO Sushi.Mediakiwi.Data.Standard
    }
}
