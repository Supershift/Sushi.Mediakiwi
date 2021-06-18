using Sushi.Mediakiwi.Data.MicroORM;
using Sushi.Mediakiwi.Framework;
using Sushi.MicroORM.Mapping;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi
{
    public class Item
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
    }

    public class ItemMap : DataMap<ItemData>
    {
        public ItemMap()
        {
            Table("cat_Examples");
            Id(x => x.ID, "Example_Key");
            Map(x => x.Name, "Example_Name").Length(50);
            Map(x => x.Description, "Example_Description");
            Map(x => x.IsActive, "Example_IsActive");
        }
    }

    [DataMap(typeof(ItemMap))]
    public class ItemData : Item
    {
        public static async Task<ItemData> SelectOneAsync(int ID)
        {
            var connector = ConnectorFactory.CreateConnector<ItemData>();
            return await connector.FetchSingleAsync(ID).ConfigureAwait(false);
        }
        
        public static async Task<List<ItemData>> SelectAllAsync()
        {
            var connector = ConnectorFactory.CreateConnector<ItemData>();
            var filter = connector.CreateDataFilter();
            return await connector.FetchAllAsync(filter).ConfigureAwait(false);
        }

        public static List<ItemData> SelectAll()
        {
            var connector = ConnectorFactory.CreateConnector<ItemData>();
            var filter = connector.CreateDataFilter();
            return connector.FetchAll(filter);
        }

        public async Task SaveAsync()
        {
            var connector = ConnectorFactory.CreateConnector<ItemData>();
            await connector.SaveAsync(this).ConfigureAwait(false);
        }

        public async Task DeleteAsync()
        {
            var connector = ConnectorFactory.CreateConnector<ItemData>();
            await connector.DeleteAsync(this).ConfigureAwait(false);
        }
    }

    public class Example : ComponentListTemplate
    {
        public Example()
        {
            this.ListSearch += Example_ListSearch;
            this.ListLoad += Example_ListLoad;
            this.ListSave += Example_ListSave;
            this.ListDelete += Example_ListDelete;
        }

        ItemData m_instance;

        private async Task Example_ListLoad(ComponentListEventArgs arg)
        {
            m_instance = await ItemData.SelectOneAsync(arg.SelectedKey).ConfigureAwait(false);

            Map(x => x.Name, m_instance).TextField("Name", 50);
            Map(x => x.Description, m_instance).RichText("Description");
            Map(x => x.IsActive, m_instance).Checkbox("Is active");

            this.FormMaps.Add(this);
        }

        private async Task Example_ListSave(ComponentListEventArgs arg)
        {
            await m_instance.SaveAsync().ConfigureAwait(false);
        }

        private async Task Example_ListDelete(ComponentListEventArgs arg)
        {
            await m_instance.DeleteAsync().ConfigureAwait(false);
        }

        private async Task Example_ListSearch(ComponentListSearchEventArgs arg)
        {
            wim.ListDataColumns.Add(new ListDataColumn("", nameof(ItemData.ID), ListDataColumnType.UniqueIdentifier));
            wim.ListDataColumns.Add(new ListDataColumn("Name", nameof(ItemData.Name)));
            wim.ListDataColumns.Add(new ListDataColumn("Description", nameof(ItemData.Description)));
            wim.ListDataColumns.Add(new ListDataColumn("Active", nameof(ItemData.IsActive)) { ColumnWidth = 80 });

            var data = await ItemData.SelectAllAsync().ConfigureAwait(false);
            wim.ListDataAdd(data);
        }
    }
}
