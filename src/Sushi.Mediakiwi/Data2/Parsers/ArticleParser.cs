using System;
using System.Collections.Generic;
using System.Data;
using Sushi.Mediakiwi.Data.DalReflection;
using Sushi.Mediakiwi.Framework;

namespace Sushi.Mediakiwi.Data.Parsers
{
    #region REPLICATED TO Sushi.Mediakiwi.Data.Standard

    public class ArticleParser
    {
        public static Article SelectOne(int ID)
        {
            SqlEntityParser parser = new SqlEntityParser();
            return parser.SelectOne<Article>(ID, true);
        }

        public static int Save(Article entity)
        {
            SqlEntityParser parser = new SqlEntityParser();
            return parser.Save<Article>(entity);
        }

        public static void Delete(Article entity)
        {
            SqlEntityParser parser = new SqlEntityParser();
            parser.Delete<Article>(entity);
        }

        public static List<ArticleList> SelectList(int listID)
        {
            SqlEntityParser parser = new SqlEntityParser();
            DataRequest data = new DataRequest();
            data.AddParam("LIST", listID, SqlDbType.Int);
            string sql = @"
SELECT [Wiki_Key]
      ,[Wiki_Title]
	  ,	Wiki_Summary
      ,ISNULL([Wiki_Updated], [Wiki_Created]) as Wiki_Updated
	  ,User_DisplayName
  FROM [dbo].[wim_WikiArticles] left join wim_Users on User_Key = [Wiki_Author_Key]
  where [Wiki_ComponentList_Key] = @LIST
  order by [Wiki_Created] desc 
";
            return parser.ExecuteList<ArticleList>(sql, data);
        }

        public static Article CheckIfItemExists(int? wikiList, int? wikiPage)
        {
            try
            {
                SqlEntityParser parser = new SqlEntityParser();
                DataRequest data = new DataRequest();
                if (wikiList.HasValue)
                    data.WhereClause.Add(new DatabaseDataValueColumn("Wiki_BelongsToList_Key", SqlDbType.Int, wikiList.Value));
                if (wikiPage.HasValue)
                    data.WhereClause.Add(new DatabaseDataValueColumn("Wiki_BelongsToPage_Key", SqlDbType.Int, wikiPage.Value));
                var item = parser.SelectOne<Article>(data);
                return item;
            }
            catch (Exception exc)
            {
                Sushi.Mediakiwi.Data.Notification.InsertOne("Wiki Article", exc);
                return new Article();
            }
        }
        internal static Article SelectOneForListOrNew(int forListID, string defaultTitle)
        {
            SqlEntityParser parser = new SqlEntityParser();
            DataRequest data = new DataRequest();
            data.WhereClause.Add(new DatabaseDataValueColumn("Wiki_BelongsToList_Key", SqlDbType.Int, forListID));
            var item = parser.SelectOne<Article>(data);
            if (item == null || item.ID < 1)
            {
                item = CreateNew(forListID, null, defaultTitle);
            }
            return item;
        }

        private static Article CreateNew(int? forListID, int? forPageId, string defaultTitle)
        {
            SqlEntityParser parser = new SqlEntityParser();
            var item = new Data.Article();
            if (forListID.HasValue) item.BelongsToListID = forListID.Value;
            if (forPageId.HasValue) item.BelongsToPageID = forPageId.Value;
            item.Created = DateTime.UtcNow;
            item.IsActive = true;
            item.Title = defaultTitle;

            parser.Save<Article>(item);
            return item;
        }

        internal static Article SelectOneForPageOrNew(int forPageID, string defaultTitle)
        {
            SqlEntityParser parser = new SqlEntityParser();
            DataRequest data = new DataRequest();
            data.WhereClause.Add(new DatabaseDataValueColumn("Wiki_BelongsToPage_Key", SqlDbType.Int, forPageID));
            var item = parser.SelectOne<Article>(data);
            if (item == null || item.ID < 1)
            {
                item = CreateNew(null, forPageID, defaultTitle);
            }
            return item;
        }
    }

    #endregion REPLICATED TO Sushi.Mediakiwi.Data.Standard
}
