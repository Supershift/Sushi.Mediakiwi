using System;
using System.Collections.Generic;
using System.Data;
using Sushi.Mediakiwi.Data.DalReflection;
using Sushi.Mediakiwi.Framework;

namespace Sushi.Mediakiwi.Data
{
    public class ArticleList : Article
    {
        #region REPLICATED TO Sushi.Mediakiwi.Data.Standard
        [DatabaseColumn("User_DisplayName", SqlDbType.NVarChar, IsNullable = true, IsOnlyRead = true)]
        public string Author { get; set; }
        #endregion REPLICATED TO Sushi.Mediakiwi.Data.Standard
    }
}
