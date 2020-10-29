using System;
using System.Reflection;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Text;

namespace Sushi.Mediakiwi.Data.DalReflection
{
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class DatabaseEntityAttribute : Attribute
    {
        /// <summary>
        /// The reflection columns can set grouped so the SQL statements (select, insert, update) can 
        /// be targeted to smaller data groups (f.e. only Id and title field required for select all).
        /// </summary>
        public DatabaseColumnGroup CollectionLevel;

        /// <summary>
        /// This attribute is used when nested entity should be inclused in DAL reflection
        /// </summary>
        public DatabaseEntityAttribute()
        {
        }
    }
}
