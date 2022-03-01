using System.Data;
using Sushi.MicroORM;
using Sushi.MicroORM.Mapping;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Data
{
    [DataMap(typeof(MenuGroupMap))]
    public class MenuGroup
    {
        public class MenuGroupMap : DataMap<MenuGroup>
        {
            public MenuGroupMap()
            {
                Table("wim_MenuGroups");
                Id(x => x.ID, "MenuGroup_Key").Identity();
                Map(x => x.Title, "MenuGroup_Title").Length(50);
                Map(x => x.Description, "MenuGroup_Description").Length(500);
                Map(x => x.Tag, "MenuGroup_Tag").Length(20);
            }
        }

        public int ID { get; set; }
        public string Title { get; set; }
        public string Tag { get; set; }
        public string Description { get; set; }

        /// <summary>
        /// Returns all existing Menu Groups
        /// </summary>
        /// <returns></returns>
        public static List<MenuGroup> FetchAll()
        {
            var connector = new Connector<MenuGroup>();
            var filter = connector.CreateQuery();
            var result = connector.FetchAll(filter);
            return result;
        }


        /// <summary>
        /// Returns all existing Menu Groups Async
        /// </summary>
        /// <returns></returns>
        public static async Task<List<MenuGroup>> FetchAllAsync()
        {
            var connector = new Connector<MenuGroup>();
            var filter = connector.CreateQuery();
            var result = await connector.FetchAllAsync(filter);
            return result;
        }

        /// <summary>
        /// Returns a single Menu Group Async
        /// </summary>
        /// <param name="id">The Menu Group identifier </param>
        /// <returns></returns>
        public static MenuGroup FetchSingle(int id)
        {
            var connector = new Connector<MenuGroup>();
            var result = connector.FetchSingle(id);
            return result;
        }

        /// <summary>
        /// Returns a single Menu Group Async
        /// </summary>
        /// <param name="id">The Menu Group identifier </param>
        /// <returns></returns>
        public static async Task<MenuGroup> FetchSingleAsync(int id)
        {
            var connector = new Connector<MenuGroup>();
            var result = await connector.FetchSingleAsync(id);
            return result;
        }

        /// <summary>
        /// Returns a single Menu Group
        /// </summary>
        /// <param name="tag">The Menu Group tag</param>
        /// <returns></returns>
        public static MenuGroup FetchSingle(string tag)
        {
            var connector = new Connector<MenuGroup>();
            var filter = connector.CreateQuery();
            filter.Add(x => x.Tag, tag.ToUpperInvariant());

            var result = connector.FetchSingle(filter);
            return result;
        }

        /// <summary>
        /// Returns a single Menu Group Async
        /// </summary>
        /// <param name="tag">The Menu Group tag</param>
        /// <returns></returns>
        public static async Task<MenuGroup> FetchSingleAsync(string tag)
        {
            var connector = new Connector<MenuGroup>();
            var filter = connector.CreateQuery();
            filter.Add(x => x.Tag, tag.ToUpperInvariant());

            var result = await connector.FetchSingleAsync(filter);
            return result;
        }

        /// <summary>
        /// Saves the Menu Group
        /// </summary>
        public void Save()
        {
            Tag = Tag.ToUpperInvariant();

            var connector = new Connector<MenuGroup>();
            connector.Save(this);
        }

        /// <summary>
        /// Saves the Menu Group Async
        /// </summary>
        public async Task SaveAsync()
        {
            Tag = Tag.ToUpperInvariant();

            var connector = new Connector<MenuGroup>();
            await connector.SaveAsync(this);
        }

        /// <summary>
        /// Deletes the Menu Group
        /// </summary>
        public void Delete()
        {
            var connector = new Connector<MenuGroup>();
            connector.Delete(this);
        }

        /// <summary>
        /// Deletes the Menu Group Async
        /// </summary>
        public async Task DeleteAsync()
        {
            var connector = new Connector<MenuGroup>();
            await connector.DeleteAsync(this);
        }
    }
}
