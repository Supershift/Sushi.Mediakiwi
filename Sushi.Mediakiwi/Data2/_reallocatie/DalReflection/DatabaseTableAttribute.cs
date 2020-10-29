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
    [AttributeUsage(AttributeTargets.Class)]
    public class DatabaseTableAttribute : Attribute
    {
        /// <summary>
        /// Name of the database table in question.
        /// </summary>
        public string Name;

        /// <summary>
        /// Additional joins for select and selectAll.
        /// </summary>
        public string Join;

        /// <summary>
        /// Additional order by for selectAll.
        /// </summary>
        public string Order;

        /// <summary>
        /// Additional group by for selectAll.
        /// </summary>
        public string Group;

        /// <summary>
        /// The designated portal
        /// </summary>
        public string Portal;

        /// <summary>
        /// An attribute defined for use in rapid DAL creation. This specific attribute is used for specifying the database table.
        /// </summary>
        public DatabaseTableAttribute(string name)
        {
            Name = name;
        }
    }
}
