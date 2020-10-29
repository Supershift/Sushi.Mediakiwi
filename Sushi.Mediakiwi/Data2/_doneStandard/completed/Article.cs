using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sushi.Mediakiwi.Data.DalReflection;

namespace Sushi.Mediakiwi.Data
{
    [DatabaseTable("wim_WikiArticles")]
    public class Article
    {
        #region REPLICATED TO Sushi.Mediakiwi.Data.Standard
        public Article()
        {
            Created = Sushi.Mediakiwi.Data.Common.DatabaseDateTime;
        }
        [DatabaseColumn("Wiki_Key", SqlDbType.Int, IsPrimaryKey = true)]
        public virtual int ID { get; set; }
        [DatabaseColumn("Wiki_Title", SqlDbType.NVarChar, Length = 100)]
        public string Title { get; set; }
        [DatabaseColumn("Wiki_Summary", SqlDbType.NVarChar, IsNullable = true)]
        public string Summary { get; set; }
        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        /// <value>The data.</value>
        [DatabaseColumn("Wiki_Data", SqlDbType.Xml, IsNullable = true)]
        public virtual string Data { get; set; }

        [DatabaseColumn("Wiki_Created", SqlDbType.DateTime)]
        public virtual DateTime Created { get; set; }
        [DatabaseColumn("Wiki_Updated", SqlDbType.DateTime)]
        public virtual DateTime Updated { get; set; }
        [DatabaseColumn("Wiki_IsActive", SqlDbType.Bit)]
        public virtual bool IsActive { get; set; }
        [DatabaseColumn("Wiki_ComponentList_Key", SqlDbType.Int)]
        public virtual int ComponentListID { get; set; }
        [DatabaseColumn("Wiki_Author_Key", SqlDbType.Int)]
        public virtual int AuthorID { get; set; }

        /// <summary>
        /// This tells the system the wiki article belongs to a list and needs to be shown 
        /// above as a ? icon. Is mutualy exlusive to BelongsToPageID
        /// </summary>
        [DatabaseColumn("Wiki_BelongsToList_Key", SqlDbType.Int)]
        public virtual int BelongsToListID { get; set; }

        /// <summary>
        /// This tells the system the wiki article belongs to a page and needs to be shown 
        /// above as a ? icon. Is mutualy exlusive to BelongsToListID
        /// </summary>
        [DatabaseColumn("Wiki_BelongsToPage_Key", SqlDbType.Int)]
        public virtual int BelongsToPageID { get; set; }
        #endregion REPLICATED TO Sushi.Mediakiwi.Data.Standard
    }
}
