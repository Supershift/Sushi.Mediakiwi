using System;
using System.Collections.Generic;
using System.Text;

namespace Sushi.Mediakiwi.Data
{
    /// <summary>
    /// 
    /// </summary>
    public class SqlInfo
    {
        /// <summary>
        /// Gets or sets the SQL row count.
        /// </summary>
        /// <value>The SQL row count.</value>
        public int SqlRowCount { get; set; }
        /// <summary>
        /// Gets or sets the SQL order.
        /// </summary>
        /// <value>The SQL order.</value>
        public string SqlOrder { get; set; }
        /// <summary>
        /// Gets or sets the SQL join.
        /// </summary>
        /// <value>The SQL join.</value>
        public string SqlJoin { get; set; }
        /// <summary>
        /// Gets or sets the SQL group.
        /// </summary>
        /// <value>The SQL group.</value>
        public string SqlGroup { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [SQL join is new].
        /// </summary>
        /// <value><c>true</c> if [SQL join is new]; otherwise, <c>false</c>.</value>
        public bool SqlJoinIsNew { get; set; }
    }
}
