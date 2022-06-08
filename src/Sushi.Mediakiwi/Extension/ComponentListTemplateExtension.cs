using Sushi.Mediakiwi.Data;
using Sushi.Mediakiwi.Framework;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Extension
{
    public static class ComponentListTemplateExtension
    {
        /// <summary>
        /// Updates the sortorder an item in this Component List
        /// </summary>
        /// <param name="template">The Component List Template</param>
        /// <param name="sortFrom">The SortOrder From</param>
        /// <param name="sortTo">The SortOrder TO</param>
        /// <returns>TRUE when succeeded, false when failed.</returns>
        public static async Task<bool> UpdateSortOrderAsync(this IComponentListTemplate template, int sortFrom, int sortTo)
        {
            if (template == null || string.IsNullOrWhiteSpace(template.wim.m_sortOrderSqlTable) || string.IsNullOrWhiteSpace(template.wim.m_sortOrderSqlColumn))
                return false;

            try
            {
                // Use the Page ORM object for accessing the DB 
                var connector = Data.MicroORM.ConnectorFactory.CreateConnector<Page>();

                // Disable caching for results
                connector.UseCacheOnSelect = false;

                // Override the Mapped table (wim_pages to the correct table)
                connector.Map.Table(template.wim.m_sortOrderSqlTable);

                var filter = connector.CreateQuery();

                // Query replacements
                string sqlColumn = $"[{template.wim.m_sortOrderSqlColumn}]";
                string sqlTable = $"[{template.wim.m_sortOrderSqlTable}]";
                string sqlKey = $"[{template.wim.m_sortOrderSqlKey}]";

                // Convert to SQL params
                filter.AddParameter("@sortF", sortFrom);
                filter.AddParameter("@sortT", sortTo);

                // Get current order
                int currentDbFrom = await connector.ExecuteScalarAsync<int>($"SELECT TOP 1 {sqlColumn} FROM {sqlTable} WHERE {sqlKey} = @sortF", filter).ConfigureAwait(false);
                int currentDbTo = await connector.ExecuteScalarAsync<int>($"SELECT TOP 1 {sqlColumn} FROM {sqlTable} WHERE {sqlKey} = @sortT", filter).ConfigureAwait(false);

                // Add current order to SQL params
                filter.AddParameter("@currentDbTo", currentDbTo);
                filter.AddParameter("@currentDbFrom", currentDbFrom);

                // When the FROM > To
                if (currentDbFrom > currentDbTo)
                {
                    string q1 = $"UPDATE {sqlTable} SET {sqlColumn} = {sqlColumn} + 1 WHERE {sqlColumn} >= @currentDbTo AND {sqlColumn} <= @currentDbFrom";
                    string q2 = $"UPDATE {sqlTable} SET {sqlColumn} = (SELECT {sqlColumn} -1 FROM {sqlTable} WHERE {sqlKey} = @sortT) WHERE {sqlKey} = @sortF";

                    await connector.ExecuteNonQueryAsync(q1, filter).ConfigureAwait(false);
                    await connector.ExecuteNonQueryAsync(q2, filter).ConfigureAwait(false);
                }
                // When the TO > FROM
                else
                {
                    string q1 = $"UPDATE {sqlTable} SET {sqlColumn} = {sqlColumn} - 1 WHERE {sqlColumn} >= @currentDbFrom AND {sqlColumn} <= @currentDbTo";
                    string q2 = $"UPDATE {sqlTable} SET {sqlColumn} = (SELECT {sqlColumn} FROM {sqlTable} WHERE {sqlKey} = @sortT) WHERE {sqlKey} = @sortF";
                    string q3 = $"UPDATE {sqlTable} SET {sqlColumn} = (SELECT {sqlColumn} +1 FROM {sqlTable} WHERE {sqlKey} = @sortT) WHERE {sqlKey} = @sortT";

                    await connector.ExecuteNonQueryAsync(q1, filter).ConfigureAwait(false);
                    await connector.ExecuteNonQueryAsync(q2, filter).ConfigureAwait(false);
                    await connector.ExecuteNonQueryAsync(q3, filter).ConfigureAwait(false);
                }

                return true;
            }
            catch (System.Exception ex)
            {
                await Notification.InsertOneAsync("SortOrder update", ex).ConfigureAwait(false);
                return false;
            }

        }
    }
}
