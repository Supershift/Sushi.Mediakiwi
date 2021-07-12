using Sushi.MicroORM.Mapping;
using System.Data;

namespace Sushi.Mediakiwi.Data
{
    [DataMap(typeof(ArticleListMap))]
    public class ArticleList : Article
    {
        internal class ArticleListMap : DataMap<ArticleList>
        {
            public ArticleListMap() : this(false) { }
            
            public ArticleListMap(bool isSave)
            {
                if(isSave)
                    Table("wim_WikiArticles");
                else
                    Table("wim_WikiArticles LEFT JOIN wim_Users ON User_Key = Wiki_Author_Key");
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
                Map(x => x.Author, "User_DisplayName").ReadOnly();
            }
        }

        public string Author { get; set; }        
    }
}