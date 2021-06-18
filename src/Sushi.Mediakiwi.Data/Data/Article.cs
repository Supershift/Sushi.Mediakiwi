using Sushi.MicroORM.Mapping;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Sushi.Mediakiwi.Data.MicroORM;

namespace Sushi.Mediakiwi.Data
{
    [DataMap(typeof(ArticleMap))]
    public class Article
    {
        public Article()
        {
            Created = Common.DatabaseDateTime;
        }

        public class ArticleMap : DataMap<Article>
        {
            public ArticleMap()
            {
                Table("wim_WikiArticles");
                Id(x => x.ID, "Wiki_Key").Identity().SqlType(SqlDbType.Int);
                Map(x => x.Title, "Wiki_Title").SqlType(SqlDbType.NVarChar).Length(100);
                Map(x => x.Summary, "Wiki_Summary").SqlType(SqlDbType.NVarChar);
                Map(x => x.Data, "Wiki_Data").SqlType(SqlDbType.Xml);
                Map(x => x.Created, "Wiki_Created").SqlType(SqlDbType.DateTime);
                Map(x => x.Updated, "Wiki_Updated").SqlType(SqlDbType.DateTime);
                Map(x => x.IsActive, "Wiki_IsActive").SqlType(SqlDbType.Bit);
                Map(x => x.ComponentListID, "Wiki_ComponentList_Key").SqlType(SqlDbType.Int);
                Map(x => x.AuthorID, "Wiki_Author_Key").SqlType(SqlDbType.Int);
                Map(x => x.BelongsToListID, "Wiki_BelongsToList_Key").SqlType(SqlDbType.Int);
                Map(x => x.BelongsToPageID, "Wiki_BelongsToPage_Key").SqlType(SqlDbType.Int);
            }
        }

        #region properties

        /// <summary>
        /// Uniqe identifier of the Article
        /// </summary>
        public int ID { get; set; }

        public string Title { get; set; }
        public string Summary { get; set; }

        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        /// <value>The data.</value>
        public string Data { get; set; }

        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
        public bool IsActive { get; set; }
        public int ComponentListID { get; set; }
        public int AuthorID { get; set; }

        /// <summary>
        /// This tells the system the wiki article belongs to a list and needs to be shown
        /// above as a ? icon. Is mutualy exlusive to BelongsToPageID
        /// </summary>
        public int BelongsToListID { get; set; }

        /// <summary>
        /// This tells the system the wiki article belongs to a page and needs to be shown
        /// above as a ? icon. Is mutualy exlusive to BelongsToListID
        /// </summary>
        public int BelongsToPageID { get; set; }

        #endregion properties


        public static Article SelectOne(int ID)
        {
            var connector = ConnectorFactory.CreateConnector<Article>();
            return connector.FetchSingle(ID);
        }

        public static async Task<Article> SelectOneAsync(int ID)
        {
            var connector = ConnectorFactory.CreateConnector<Article>();
            return await connector.FetchSingleAsync(ID);
        }

        public static int Save(Article entity)
        {
            var connector = ConnectorFactory.CreateConnector<Article>();
            connector.Save(entity);
            return entity.ID;
        }

        public static async Task<int> SaveAsync(Article entity)
        {
            var connector = ConnectorFactory.CreateConnector<Article>();
            await connector.SaveAsync(entity);
            return entity.ID;
        }

        public static void Delete(Article entity)
        {
            var connector = ConnectorFactory.CreateConnector<Article>();
            connector.Delete(entity);
        }

        public static async Task DeleteAsync(Article entity)
        {
            var connector = ConnectorFactory.CreateConnector<Article>();
            await connector.DeleteAsync(entity);
        }

        public static List<ArticleList> SelectList(int listID)
        {
            var connector = ConnectorFactory.CreateConnector<ArticleList>();
            var filter = connector.CreateDataFilter();

            filter.AddParameter("@listId", listID);


            return connector.FetchAll(@"
SELECT [Wiki_Key]
      ,[Wiki_Title]
	  ,[Wiki_Summary]
      ,ISNULL([Wiki_Updated], [Wiki_Created]) as Wiki_Updated
	  ,[User_DisplayName]
  FROM [dbo].[wim_WikiArticles] 
  LEFT JOIN [wim_Users] ON [User_Key] = [Wiki_Author_Key]
  WHERE [Wiki_ComponentList_Key] = @listId
  ORDER BY [Wiki_Created] DESC", filter);
        }


        public static async Task<List<ArticleList>> SelectListAsync(int listID)
        {
            var connector = ConnectorFactory.CreateConnector<ArticleList>();
            var filter = connector.CreateDataFilter();

            filter.AddParameter("@listId", listID);

            return await connector.FetchAllAsync(@"
SELECT [Wiki_Key]
      ,[Wiki_Title]
	  ,[Wiki_Summary]
      ,ISNULL([Wiki_Updated], [Wiki_Created]) as Wiki_Updated
	  ,[User_DisplayName]
  FROM [dbo].[wim_WikiArticles] 
  LEFT JOIN [wim_Users] ON [User_Key] = [Wiki_Author_Key]
  WHERE [Wiki_ComponentList_Key] = @listId
  ORDER BY [Wiki_Created] DESC", filter);
        }

        public static Article CheckIfItemExists(int? wikiList, int? wikiPage)
        {
            try
            {
                var connector = ConnectorFactory.CreateConnector<Article>();
                var filter = connector.CreateDataFilter();

                if (wikiList.HasValue) 
                    filter.Add(x => x.BelongsToListID, wikiList.Value);

                if (wikiPage.HasValue)
                    filter.Add(x => x.BelongsToPageID, wikiPage.Value);

                return connector.FetchSingle(filter);
            }
            catch (Exception exc)
            {
                Notification.InsertOne("Wiki Article", exc.ToString());
                return new Article();
            }
        }


        public static async Task<Article> CheckIfItemExistsAsync(int? wikiList, int? wikiPage)
        {
            try
            {
                var connector = ConnectorFactory.CreateConnector<Article>();
                var filter = connector.CreateDataFilter();

                if (wikiList.HasValue)
                    filter.Add(x => x.BelongsToListID, wikiList.Value);

                if (wikiPage.HasValue)
                    filter.Add(x => x.BelongsToPageID, wikiPage.Value);

                return await connector.FetchSingleAsync(filter);
            }
            catch (Exception exc)
            {
                Notification.InsertOne("Wiki Article", exc.ToString());
                return new Article();
            }
        }


        public static Article SelectOneForListOrNew(int forListID, string defaultTitle)
        {
            var connector = ConnectorFactory.CreateConnector<Article>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.BelongsToListID, forListID);
            var item = connector.FetchSingle(filter);
            if (item == null || item.ID < 1)
                item = CreateNew(forListID, null, defaultTitle);

            return item;
        }


        public static async Task<Article> SelectOneForListOrNewAsync(int forListID, string defaultTitle)
        {
            var connector = ConnectorFactory.CreateConnector<Article>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.BelongsToListID, forListID);
            var item = await connector.FetchSingleAsync(filter);
            if (item == null || item.ID < 1)
                item = await CreateNewAsync(forListID, null, defaultTitle);

            return item;
        }

        public static Article CreateNew(int? forListID, int? forPageId, string defaultTitle)
        {
            var connector = ConnectorFactory.CreateConnector<Article>();
            var item = new Article();

            if (forListID.HasValue)
                item.BelongsToListID = forListID.Value;

            if (forPageId.HasValue)
                item.BelongsToPageID = forPageId.Value;

            item.Created = DateTime.UtcNow;
            item.IsActive = true;
            item.Title = defaultTitle;

            connector.Save(item);
            return item;
        }


        public static async Task<Article> CreateNewAsync(int? forListID, int? forPageId, string defaultTitle)
        {
            var connector = ConnectorFactory.CreateConnector<Article>();
            var item = new Article();

            if (forListID.HasValue)
                item.BelongsToListID = forListID.Value;

            if (forPageId.HasValue)
                item.BelongsToPageID = forPageId.Value;

            item.Created = DateTime.UtcNow;
            item.IsActive = true;
            item.Title = defaultTitle;

            await connector.SaveAsync(item);
            return item;
        }

        public static Article SelectOneForPageOrNew(int forPageID, string defaultTitle)
        {
            var connector = ConnectorFactory.CreateConnector<Article>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.BelongsToPageID, forPageID);

            var item = connector.FetchSingle(filter);
            if (item == null || item.ID < 1)
                item = CreateNew(null, forPageID, defaultTitle);

            return item;
        }

        public static async Task<Article> SelectOneForPageOrNewAsync(int forPageID, string defaultTitle)
        {
            var connector = ConnectorFactory.CreateConnector<Article>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.BelongsToPageID, forPageID);

            var item = await connector.FetchSingleAsync(filter);
            if (item == null || item.ID < 1)
                item = await CreateNewAsync(null, forPageID, defaultTitle);

            return item;
        }
    }
}