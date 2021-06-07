using Sushi.MicroORM;
using Sushi.MicroORM.Mapping;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sushi.Mediakiwi.Data.MicroORM;

namespace Sushi.Mediakiwi.Data
{
    /// <summary>
    ///
    /// </summary>
    [DataMap(typeof(PropertyMap))]
    public class Property : IExportable
    {
        public class PropertyMap : DataMap<Property>
        {
            public PropertyMap()
            {
                Table("wim_Properties");
                Id(x => x.ID, "Property_Key").Identity();
                Map(x => x.GUID, "Property_GUID");
                Map(x => x.ListID, "Property_List_Key");
                Map(x => x.ListTypeID, "Property_List_Type_Key");
                Map(x => x.Title, "Property_Title").Length(255);
                Map(x => x.IsPresentProperty, "Property_IsPresent");
                Map(x => x.FieldName, "Property_FieldName").Length(35);
                Map(x => x.TypeID, "Property_Type");
                Map(x => x.FilterType, "Property_ColumnType").Length(15);
                Map(x => x.OptionListSelect, "Property_OptionList_Key");
                Map(x => x.IsShort, "Property_IsShort");
                Map(x => x.ListSelect, "Property_ListBase_Key");
                Map(x => x.ListCollection, "Property_ListCollection").Length(50);
                Map(x => x.PropertyType, "Property_CodeType");
                Map(x => x.Filter, "Property_Column").Length(50);
                Map(x => x.CanFilter, "Property_CanFilter");
                Map(x => x.OnlyInput, "Property_OnlyInput");
                Map(x => x.Data, "Property_Data");
                Map(x => x.IsFixed, "Property_IsFixed");
                Map(x => x.InheritedID, "Property_Property_Key");
                Map(x => x.SortOrder, "Property_SortOrder");

                Map(x => x.TemplateID, "Property_Template_Key");
                Map(x => x.InteractiveHelp, "Property_Help").Length(512);
                Map(x => x.IsMandatory, "Property_IsRequired");
                Map(x => x.MaxValueLength, "Property_MaxInput");
                Map(x => x.DefaultValue, "Property_Default");
            }
        }

        #region Properties

        public int TemplateID { get; set; }
        public string DefaultValue { get; set; }

        /// <summary>
        /// Gets or sets the ID.
        /// </summary>
        /// <value>The ID.</value>
        public int ID { get; set; }

        private Guid m_GUID;

        /// <summary>
        /// Unique identifier used for import/export
        /// </summary>
        /// <value>The GUID.</value>
        public Guid GUID
        {
            get
            {
                if (this.m_GUID == Guid.Empty)
                    this.m_GUID = Guid.NewGuid();
                return m_GUID;
            }
            set { m_GUID = value; }
        }

        /// <summary>
        /// Gets or sets the list ID.
        /// </summary>
        /// <value>The list ID.</value>
        public int ListID { get; set; }

        /// <summary>
        /// Gets or sets the list type ID.
        /// </summary>
        /// <value>The list type ID.</value>
        public int? ListTypeID { get; set; }

        /// <summary>
        /// Property information
        /// </summary>
        internal System.Reflection.PropertyInfo Info;

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>The title.</value>
        public string Title { get; set; }

        public string Section { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is present property.
        /// </summary>
        public bool IsPresentProperty { get; set; }

        public string FieldName { get; set; }

        public string FieldName2 { get; set; }

        public int? MaxValueLength { get; set; }

        public int TypeID { get; set; }

        public string Mandatory { get; set; }

        /// <summary>
        /// Gets or sets the type of the filter.
        /// </summary>
        /// <value>The type of the filter.</value>
        public string FilterType { get; set; }

        /// <summary>
        /// Gets or sets the must match ID.
        /// </summary>
        /// <value>The must match ID.</value>
        public string MustMatchID { get; set; }

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
            set
            {
                Mandatory = (value ? "1" : "0");
            }
        }

        public string OnlyRead { get; set; }

        [System.Xml.Serialization.XmlIgnore()]
        public bool IsOnlyRead
        {
            get { return OnlyRead == "1"; }
            set { if (value) this.OnlyRead = "1"; else this.OnlyRead = "0"; }
        }

        public string InteractiveHelp { get; set; }

        /// <summary>
        /// Gets or sets the list select.
        /// </summary>
        /// <value>The list select.</value>
        public bool AutoPostBack { get; set; }

        /// <summary>
        /// Gets or sets the option list select.
        /// </summary>
        /// <value>The option list select.</value>
        public int? OptionListSelect { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is short.
        /// </summary>
        /// <value><c>true</c> if this instance is short; otherwise, <c>false</c>.</value>
        public bool IsShort { get; set; }

        /// <summary>
        /// Gets or sets the list select.
        /// </summary>
        /// <value>The list select.</value>
        public int? ListSelect { get; set; }

        /// <summary>
        /// Gets or sets the list select.
        /// </summary>
        /// <value>The list select.</value>
        public bool CanContainOneItem
        {
            get
            {
                return this.MaxValueLength.GetValueOrDefault(0) == 1;
            }
            set
            {
                if (value)
                {
                    this.MaxValueLength = 1;
                }
                else
                {
                    this.MaxValueLength = null;
                }
            }
        }

        /// <summary>
        /// Gets or sets the list select.
        /// </summary>
        /// <value>The list select.</value>
        public string ListCollection { get; set; }

        public string EmptyFirst { get; set; }

        [System.Xml.Serialization.XmlIgnore()]
        public bool IsEmptyFirst
        {
            get { return this.EmptyFirst == "1"; }
        }

        /// <summary>
        /// Gets or sets the type of the property.
        /// </summary>
        /// <value>The type of the property.</value>
        public int PropertyType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is filter.
        /// </summary>
        /// <value><c>true</c> if this instance is filter; otherwise, <c>false</c>.</value>
        public bool IsHidden { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is filter.
        /// </summary>
        /// <value><c>true</c> if this instance is filter; otherwise, <c>false</c>.</value>
        public string Filter { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance can filter.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can filter; otherwise, <c>false</c>.
        /// </value>
        public bool CanFilter { get; set; }

        public bool OnlyInput { get; set; }

        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        /// <value>The data.</value>
        public string Data { get; set; }

        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        /// <value>The data.</value>
        public bool IsFixed { get; set; }

        /// <summary>
        /// Is this property Marked as a Shared Field ?
        /// </summary>
        public bool IsSharedField { get; set; }

        /// <summary>
        /// Gets or sets the inherited ID. This property is inherited of a template property list.
        /// </summary>
        /// <value>The inherited ID.</value>
        public int? InheritedID { get; set; }

        private PropertyOption[] m_Options;

        /// <summary>
        /// Gets the options.
        /// </summary>
        /// <value>The options.</value>
        public PropertyOption[] Options
        {
            get
            {
                if (m_Options == null)
                    m_Options = PropertyOption.SelectAll(this.ID);
                return m_Options;
            }
        }

        /// <summary>
        /// Gets or sets the sort order.
        /// </summary>
        /// <value>The sort order.</value>
        public int SortOrder { get; set; }

        public DateTime? Updated
        {
            get { return DateTime.Now; }
        }

        /// <summary>
        /// Clones me.
        /// </summary>
        /// <returns></returns>
        public Property CloneMe()
        {
            return this.MemberwiseClone() as Property;
        }

        #endregion Properties

        /// <summary>
        /// Delete an implementation record.
        /// </summary>
        /// <returns></returns>
        public bool Delete()
        {
            var connector = ConnectorFactory.CreateConnector<Property>();
            connector.Delete(this);

            return true;
        }

        /// <summary>
        /// Delete an implementation record.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> DeleteAsync()
        {
            var connector = ConnectorFactory.CreateConnector<Property>();
            await connector.DeleteAsync(this);

            return true;
        }

        /// <summary>
        /// Selects the one.
        /// </summary>
        /// <param name="ID">The ID.</param>
        /// <returns></returns>
        public static Property SelectOne(int ID)
        {
            var connector = ConnectorFactory.CreateConnector<Property>();
            return connector.FetchSingle(ID);
        }

        /// <summary>
        /// Selects the one Async.
        /// </summary>
        /// <param name="ID">The ID.</param>
        /// <returns></returns>
        public static async Task<Property> SelectOneAsync(int ID)
        {
            var connector = ConnectorFactory.CreateConnector<Property>();
            return await connector.FetchSingleAsync(ID);
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
            var connector = ConnectorFactory.CreateConnector<Property>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.ListID, listID);
            if (listTypeID.HasValue)
                filter.Add(x => x.TypeID, listTypeID.Value);
            filter.Add(x => x.FieldName, fieldName);

            return connector.FetchSingle(filter);
        }

        public static List<Property> SelectAllByTemplate(int templateid)
        {
            var connector = new Connector<Property>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.TemplateID, templateid);
            filter.AddOrder(x => x.SortOrder);
            return connector.FetchAll(filter);
        }

        public static async Task<List<Property>> SelectAllByTemplateAsync(int templateid)
        {
            var connector = new Connector<Property>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.TemplateID, templateid);
            filter.AddOrder(x => x.SortOrder);
            return await connector.FetchAllAsync(filter);
        }

        /// <summary>
        /// Selects the one.
        /// </summary>
        /// <param name="listID">The list ID.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="listTypeID">The list type ID.</param>
        /// <returns></returns>
        public static async Task<Property> SelectOneAsync(int listID, string fieldName, int? listTypeID)
        {
            var connector = ConnectorFactory.CreateConnector<Property>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.ListID, listID);
            if (listTypeID.HasValue)
                filter.Add(x => x.TypeID, listTypeID.Value);
            filter.Add(x => x.FieldName, fieldName);

            return await connector.FetchSingleAsync(filter);
        }

        /// <summary>
        /// Determines whether The specified list has a content container
        /// </summary>
        /// <param name="listID">The list ID.</param>
        public static bool HasContentContainer(int listID)
        {
            var connector = ConnectorFactory.CreateConnector<Property>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.ListID, listID);
            filter.Add(x => x.TypeID, 35);

            Property tmp = connector.FetchSingle(filter);
            return (tmp?.ID > 0);
        }

        /// <summary>
        /// Determines whether The specified list has a content container Async
        /// </summary>
        /// <param name="listID">The list ID.</param>
        public static async Task<bool> HasContentContainerAsync(int listID)
        {
            var connector = ConnectorFactory.CreateConnector<Property>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.ListID, listID);
            filter.Add(x => x.TypeID, 35);

            Property tmp = await connector.FetchSingleAsync(filter);
            return (tmp?.ID > 0);
        }

        /// <summary>
        /// Selects all.
        /// </summary>
        /// <returns></returns>
        public static List<Property> SelectAll()
        {
            var connector = ConnectorFactory.CreateConnector<Property>();
            var filter = connector.CreateDataFilter();
            return connector.FetchAll(filter);
        }

        /// <summary>
        /// Selects all Async.
        /// </summary>
        /// <returns></returns>
        public static async Task<List<Property>> SelectAllAsync()
        {
            var connector = ConnectorFactory.CreateConnector<Property>();
            var filter = connector.CreateDataFilter();
            return await connector.FetchAllAsync(filter);
        }

        ///// <summary>
        ///// This is a memory allocation list for when using command line tools
        ///// </summary>
        //private static List<MemoryItemProperty> MemoryAllocationList;

        /// <summary>
        /// Selects all Async.
        /// </summary>
        /// <returns></returns>
        public static List<Property> SelectAllByFieldName(string fieldName)
        {
            var connector = new Connector<Property>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.FieldName, fieldName);
            return connector.FetchAll(filter);
        }

        /// <summary>
        /// Selects all Async.
        /// </summary>
        /// <returns></returns>
        public static async Task<List<Property>> SelectAllByFieldNameAsync(string fieldName)
        {
            var connector = new Connector<Property>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.FieldName, fieldName);
            return await connector.FetchAllAsync(filter);
        }

        /// <summary>
        /// Selects all in the supplied ID array.
        /// </summary>
        /// <param name="IDs">The IDs.</param>
        /// <returns></returns>
        public static Property[] SelectAll(int[] IDs)
        {
            var connector = ConnectorFactory.CreateConnector<Property>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.ID, IDs, ComparisonOperator.In);

            return connector.FetchAll(filter).ToArray();
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
        public static Property[] SelectAll(int listID, int? listTypeID, bool showEmptyTypes, bool onlyReturnFlexibleProperties = false)
        {
            var connector = ConnectorFactory.CreateConnector<Property>();
            var filter = connector.CreateDataFilter();
        
            filter.Add(x => x.ListID, listID);

            if (listTypeID.HasValue && listTypeID.Value > 0)
            {
                filter.AddOrder(x => x.ListTypeID, Sushi.MicroORM.SortOrder.ASC);
                filter.AddOrder(x => x.SortOrder, Sushi.MicroORM.SortOrder.ASC);

                if (showEmptyTypes)
                {
                    filter.AddSql("Property_List_Type_Key = @Type OR Property_List_Type_Key is null");
                    filter.AddParameter<int>("Type", listTypeID.Value);
                }
                else
                    filter.Add(x => x.ListTypeID, listTypeID);

            }
            else
                filter.Add(x => x.ListTypeID, null);

            if (onlyReturnFlexibleProperties)
                filter.Add(x => x.IsFixed, false);

            var result = connector.FetchAll(filter);
            return result.ToArray();
        }

        public static Property[] SelectAll(int listID)
        {
            var connector = ConnectorFactory.CreateConnector<Property>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.ListID, listID);

            return connector.FetchAll(filter).ToArray();
        }

        /// <summary>
        /// Selects all in the supplied ID array.
        /// </summary>
        /// <param name="IDs">The IDs.</param>
        /// <returns></returns>
        public static async Task<Property[]> SelectAllAsync(int[] IDs)
        {
            var connector = ConnectorFactory.CreateConnector<Property>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.ID, IDs, ComparisonOperator.In);

            var result = await connector.FetchAllAsync(filter);
            return result.ToArray();
        }

        /// <summary>
        /// Save a database entity. This method will indentify the need for an INSERT or an UPDATE.
        /// And will also set the SortOrder
        /// </summary>
        /// <returns></returns>
        public bool Save()
        {
            var connector = ConnectorFactory.CreateConnector<Property>();
            bool shouldSetSortorder = (this.ID == 0);

            // Save this instance
            connector.Save(this);

            if (shouldSetSortorder)
            {
                var filter = connector.CreateDataFilter();
                filter.AddParameter("@thisID", ID);

                connector.ExecuteNonQuery("UPDATE [wim_Properties] SET [Property_SortOrder] = [Property_Key] WHERE [Property_Key] = @thisID", filter);
				connector.Cache?.FlushRegion(connector.CacheRegion);
            }

            return true;
        }

        /// <summary>
        /// Save a database entity. This method will indentify the need for an INSERT or an UPDATE.
        /// And will also set the SortOrder
        /// </summary>
        /// <returns></returns>
        public async Task<bool> SaveAsync()
        {
            var connector = ConnectorFactory.CreateConnector<Property>();
            bool shouldSetSortorder = (this.ID == 0);

            // Save this instance
            await connector.SaveAsync(this);

            if (shouldSetSortorder)
            {
                var filter = connector.CreateDataFilter();
                filter.AddParameter("@thisID", ID);

                await connector.ExecuteNonQueryAsync("UPDATE [wim_Properties] SET [Property_SortOrder] = [Property_Key] WHERE [Property_Key] = @thisID", filter);
				connector.Cache?.FlushRegion(connector.CacheRegion);
            }

            return true;
        }
    }
}