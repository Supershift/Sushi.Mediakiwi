using Sushi.MicroORM;
using Sushi.MicroORM.Mapping;
using System.Threading.Tasks;
using Sushi.Mediakiwi.Data.MicroORM;

namespace Sushi.Mediakiwi.Data
{
    [DataMap(typeof(PropertyOptionMap))]
    public class PropertyOption
    {
        public class PropertyOptionMap : DataMap<PropertyOption>
        {
            public PropertyOptionMap()
            {
                Table("wim_PropertyOptions");
                Id(x => x.ID, "PropertyOption_Key").Identity();
                Map(x => x.PropertyID, "PropertyOption_Property_Key");
                Map(x => x.Name, "PropertyOption_Text").Length(250);
                Map(x => x.Value, "PropertyOption_Value").Length(250);
                Map(x => x.SortOrderID, "PropertyOption_SortOrder");
            }
        }

        #region Properties

        /// <summary>
        /// Gets or sets the ID.
        /// </summary>
        /// <value>The ID.</value>
        public int ID { get; set; }

        /// <summary>
        /// Gets or sets the property ID.
        /// </summary>
        /// <value>The property ID.</value>
        public int PropertyID { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Value { get; set; }

        /// <summary>
        /// Gets or sets the sort order ID.
        /// </summary>
        /// <value>The sort order ID.</value>
        public int SortOrderID { get; set; }

        #endregion Properties

        /// <summary>
        /// Save a database entity. This method will indentify the need for an INSERT or an UPDATE.
        /// And will also set the SortOrder
        /// </summary>
        /// <returns></returns>
        public bool Save()
        {
            var connector = ConnectorFactory.CreateConnector<PropertyOption>();
            bool shouldSetSortorder = (this.ID == 0);

            // Save this instance
            connector.Save(this);

            if (shouldSetSortorder)
            {
                var filter = connector.CreateDataFilter();
                filter.AddParameter("@thisID", ID);

                connector.ExecuteNonQuery("UPDATE [wim_PropertyOptions] SET [PropertyOption_SortOrder] = [PropertyOption_Key] WHERE [PropertyOption_Key] = @thisID", filter);
                connector.Cache?.FlushRegion(connector.CacheRegion);
            }

            return true;
        }

        /// <summary>
        /// Save a database entity. This method will indentify the need for an INSERT or an UPDATE Async.
        /// And will also set the SortOrder
        /// </summary>
        /// <returns></returns>
        public async Task<bool> SaveAsync()
        {
            var connector = ConnectorFactory.CreateConnector<PropertyOption>();
            bool shouldSetSortorder = (this.ID == 0);

            // Save this instance
            await connector.SaveAsync(this);

            if (shouldSetSortorder)
            {
                var filter = connector.CreateDataFilter();
                filter.AddParameter("@thisID", ID);

                await connector.ExecuteNonQueryAsync("UPDATE [wim_PropertyOptions] SET [PropertyOption_SortOrder] = [PropertyOption_Key] WHERE [PropertyOption_Key] = @thisID", filter);
                connector.Cache?.FlushRegion(connector.CacheRegion);
            }

            return true;
        }

        /// <summary>
        /// Deletes the collection by FormElementID.
        /// </summary>
        /// <param name="formElementID">The form element ID.</param>
        /// <returns></returns>
        public static bool DeleteCollection(int formElementID)
        {
            if (formElementID == 0)
                return true;

            var connector = ConnectorFactory.CreateConnector<PropertyOption>();
            var filter = connector.CreateDataFilter();
            filter.AddParameter("@formElementID", formElementID);

            connector.ExecuteNonQuery("DELETE FROM [wim_PropertyOptions] WHERE [PropertyOption_Property_Key] = @formElementID", filter);
            connector.Cache?.FlushRegion(connector.CacheRegion);

            return true;
        }

        /// <summary>
        /// Deletes the collection by FormElementID.
        /// </summary>
        /// <param name="formElementID">The form element ID.</param>
        /// <returns></returns>
        public static async Task<bool> DeleteCollectionAsync(int formElementID)
        {
            if (formElementID == 0)
                return true;

            var connector = ConnectorFactory.CreateConnector<PropertyOption>();
            var filter = connector.CreateDataFilter();
            filter.AddParameter("@formElementID", formElementID);

            await connector.ExecuteNonQueryAsync("DELETE FROM [wim_PropertyOptions] WHERE [PropertyOption_Property_Key] = @formElementID", filter);
            connector.Cache?.FlushRegion(connector.CacheRegion);

            return true;
        }

        /// <summary>
        /// Selects the one.
        /// </summary>
        /// <param name="ID">The ID.</param>
        /// <returns></returns>
        public static PropertyOption SelectOne(int ID)
        {
            var connector = ConnectorFactory.CreateConnector<PropertyOption>();
            
            return connector.FetchSingle(ID);
        }

        /// <summary>
        /// Selects the one.
        /// </summary>
        /// <param name="ID">The ID.</param>
        /// <returns></returns>
        public static async Task<PropertyOption> SelectOneAsync(int ID)
        {
            var connector = ConnectorFactory.CreateConnector<PropertyOption>();
            
            return await connector.FetchSingleAsync(ID);
        }

        /// <summary>
        /// Selects all By Property ID.
        /// </summary>
        /// <param name="propertyID">The property ID.</param>
        /// <returns></returns>
        public static PropertyOption[] SelectAll(int propertyID)
        {
            var connector = ConnectorFactory.CreateConnector<PropertyOption>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.PropertyID, propertyID);
            filter.AddOrder(x => x.SortOrderID);

            return connector.FetchAll(filter).ToArray();
        }

        /// <summary>
        /// Selects all By Property ID Async.
        /// </summary>
        /// <param name="propertyID">The property ID.</param>
        /// <returns></returns>
        public static async Task<PropertyOption[]> SelectAllAsync(int propertyID)
        {
            var connector = ConnectorFactory.CreateConnector<PropertyOption>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.PropertyID, propertyID);
            filter.AddOrder(x => x.SortOrderID);

            var result = await connector.FetchAllAsync(filter);
            return result.ToArray();
        }

        /// <summary>
        /// Selects all
        /// </summary>
        /// <returns></returns>
        public static PropertyOption[] SelectAll()
        {
            var connector = ConnectorFactory.CreateConnector<PropertyOption>();
            var filter = connector.CreateDataFilter();
            filter.AddOrder(x => x.SortOrderID);

            return connector.FetchAll(filter).ToArray();
        }

        /// <summary>
        /// Selects all Async
        /// </summary>
        /// <returns></returns>
        public static async Task<PropertyOption[]> SelectAllAsync()
        {
            var connector = ConnectorFactory.CreateConnector<PropertyOption>();
            var filter = connector.CreateDataFilter();
            filter.AddOrder(x => x.SortOrderID);

            var result = await connector.FetchAllAsync(filter);
            return result.ToArray();
        }
    }
}