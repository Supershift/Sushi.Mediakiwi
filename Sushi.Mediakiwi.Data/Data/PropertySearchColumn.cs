using Sushi.MicroORM;
using Sushi.MicroORM.Mapping;
using System.Threading.Tasks;
using Sushi.Mediakiwi.Data.MicroORM;

namespace Sushi.Mediakiwi.Data
{
    /// <summary>
    ///
    /// </summary>
    [DataMap(typeof(PropertySearchColumnMap))]
    public class PropertySearchColumn
    {
        public class PropertySearchColumnMap : DataMap<PropertySearchColumn>
        {
            public PropertySearchColumnMap()
            {
                Table("wim_PropertySearchColumns");
                Id(x => x.ID, "PropertySearchColumn_Key").Identity();
                Map(x => x.ListID, "PropertySearchColumn_List_Key");
                Map(x => x.PropertyID, "PropertySearchColumn_Property_Key");
                Map(x => x.ColumnWidth, "PropertySearchColumn_Width");
                Map(x => x.Title, "PropertySearchColumn_Title").Length(50);
                Map(x => x.IsHighlight, "PropertySearchColumn_IsHighlight");
                Map(x => x.TotalType, "PropertySearchColumn_TotalType");
                Map(x => x.IsOnlyExport, "PropertySearchColumn_IsExport");
            }
        }

        #region Properties

        /// <summary>
        /// Gets or sets the ID.
        /// </summary>
        /// <value>The ID.</value>
        public int ID { get; set; }

        /// <summary>
        /// Gets or sets the list ID.
        /// </summary>
        /// <value>The list ID.</value>
        public int ListID { get; set; }

        /// <summary>
        /// Clones me.
        /// </summary>
        /// <returns></returns>
        public PropertySearchColumn CloneMe()
        {
            return this.MemberwiseClone() as PropertySearchColumn;
        }

        /// <summary>
        /// Gets or sets the property ID.
        /// </summary>
        /// <value>The property ID.</value>
        public int PropertyID { get; set; }

        private Property m_Property;

        /// <summary>
        /// Gets the property.
        /// </summary>
        /// <value>The property.</value>
        public Property Property
        {
            get
            {
                if (m_Property == null)
                    m_Property = Property.SelectOne(this.PropertyID);
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

        /// <summary>
        /// Gets or sets the width of the column.
        /// </summary>
        /// <value>The width of the column.</value>
        public int? ColumnWidth { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>The title.</value>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is highlighted.
        /// </summary>
        public bool IsHighlight { get; set; }

        /// <summary>
        /// Gets or sets the total type.
        /// </summary>
        /// <value>The total type.</value>
        public int TotalType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is default export.
        /// </summary>
        public bool IsOnlyExport { get; set; }

        #endregion Properties

        /// <summary>
        /// Save a database entity. This method will indentify the need for an INSERT or an UPDATE.
        /// And will also set the SortOrder
        /// </summary>
        /// <returns></returns>
        public bool Save()
        {
            var connector = ConnectorFactory.CreateConnector<PropertySearchColumn>();
            bool shouldSetSortorder = (this.ID == 0);

            // Save this instance
            connector.Save(this);

            if (shouldSetSortorder)
            {
                var filter = connector.CreateDataFilter();
                filter.AddParameter("@thisID", ID);

                connector.ExecuteNonQuery("UPDATE [Wim_PropertySearchColumns] SET [PropertySearchColumn_SortOrder] = [PropertySearchColumn_Key] WHERE [PropertySearchColumn_Key] = @thisID", filter);
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
            var connector = ConnectorFactory.CreateConnector<PropertySearchColumn>();
            bool shouldSetSortorder = (this.ID == 0);

            // Save this instance
            await connector.SaveAsync(this);

            if (shouldSetSortorder)
            {
                var filter = connector.CreateDataFilter();
                filter.AddParameter("@thisID", ID);

                await connector.ExecuteNonQueryAsync("UPDATE [Wim_PropertySearchColumns] SET [PropertySearchColumn_SortOrder] = [PropertySearchColumn_Key] WHERE [PropertySearchColumn_Key] = @thisID", filter);
				connector.Cache?.FlushRegion(connector.CacheRegion);
            }

            return true;
        }

        /// <summary>
        /// Selects a Single PropertySearchColumn by ID.
        /// </summary>
        /// <param name="ID">The ID.</param>
        public static PropertySearchColumn SelectOne(int ID)
        {
            var connector = ConnectorFactory.CreateConnector<PropertySearchColumn>();
            
            return connector.FetchSingle(ID);
        }

        /// <summary>
        /// Selects a Single PropertySearchColumn by ID.
        /// </summary>
        /// <param name="ID">The ID.</param>
        public static async Task<PropertySearchColumn> SelectOneAsync(int ID)
        {
            var connector = ConnectorFactory.CreateConnector<PropertySearchColumn>();
            
            return await connector.FetchSingleAsync(ID);
        }

        /// <summary>
        /// Selects a Single highlighted PropertySearchColumn by ComponentList ID.
        /// </summary>
        /// <param name="componentlistID">The componentlist ID.</param>
        /// <returns></returns>
        public static PropertySearchColumn SelectOneHighlight(int componentlistID)
        {
            var connector = ConnectorFactory.CreateConnector<PropertySearchColumn>();
            var filter = connector.CreateDataFilter();

            filter.Add(x => x.ListID, componentlistID);
            filter.Add(x => x.IsHighlight, true);
            return connector.FetchSingle(filter);
        }

        /// <summary>
        /// Selects a Single highlighted PropertySearchColumn by ComponentList ID Async.
        /// </summary>
        /// <param name="componentlistID">The componentlist ID.</param>
        /// <returns></returns>
        public static async Task<PropertySearchColumn> SelectOneHighlightAsync(int componentlistID)
        {
            var connector = ConnectorFactory.CreateConnector<PropertySearchColumn>();
            var filter = connector.CreateDataFilter();

            filter.Add(x => x.ListID, componentlistID);
            filter.Add(x => x.IsHighlight, true);
            return await connector.FetchSingleAsync(filter);
        }

        /// <summary>
        /// Selects all highlighted PropertySearchColumns by ComponentList ID.
        /// </summary>
        /// <param name="componentlistID">The componentlist ID.</param>
        /// <returns></returns>
        public static PropertySearchColumn[] SelectAll(int componentlistID)
        {
            var connector = ConnectorFactory.CreateConnector<PropertySearchColumn>();
            var filter = connector.CreateDataFilter();

            filter.Add(x => x.ListID, componentlistID);
            return connector.FetchAll(filter).ToArray();
        }

        /// <summary>
        /// Selects all highlighted PropertySearchColumns by ComponentList ID Async.
        /// </summary>
        /// <param name="componentlistID">The componentlist ID.</param>
        /// <returns></returns>
        public static async Task<PropertySearchColumn[]> SelectAllAsync(int componentlistID)
        {
            var connector = ConnectorFactory.CreateConnector<PropertySearchColumn>();
            var filter = connector.CreateDataFilter();

            filter.Add(x => x.ListID, componentlistID);
            var result = await connector.FetchAllAsync(filter);
            return result.ToArray();
        }
    }
}