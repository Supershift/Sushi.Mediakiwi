using System;
using System.Collections.Generic;
using System.Text;

namespace Sushi.Mediakiwi.Data.DalReflection
{
    /// <summary>
    /// Database data value compare options
    /// </summary>
    public enum DatabaseDataValueCompareType
    {
        /// <summary>
        /// The default compare function. f.e. where PARAM = @PARAM or where PARAM IS NULL
        /// </summary>
        Default,
        /// <summary>
        /// A Like compare. f.e. where PARAM like @PARAM
        /// </summary>
        Like,
        /// <summary>
        /// An in group compare. Only the csv list should be supplied as value. f.e. "1,2,3,4".
        /// </summary>
        In,
        /// <summary>
        /// An or in group compare. Only a string[] with csv lists should be supplied as value. f.e "1,2,3","3,4,5". 
        /// It will result in an where clause like (Product_Key in (1,2,3) or Product_Key in (3,4,5)).
        /// </summary>
        OrIn,
        /// <summary>
        /// A bigger then (>) sql compare
        /// </summary>
        BiggerThen,
        /// <summary>
        /// 
        /// </summary>
        BiggerThenOrEquals,
        /// <summary>
        /// A smaller then (>) sql compare
        /// </summary>
        SmallerThen,
        /// <summary>
        /// 
        /// </summary>
        SmallerThenOrEquals
    }
}
