using System;
using System.Collections.Generic;
using System.Text;

namespace Sushi.Mediakiwi.Data.DalReflection
{
    /// <summary>
    /// The reflection columns can set grouped so the SQL statements (select, insert, update) can 
    /// be targeted to smaller data groups (f.e. only Id and title field required for select all).
    /// </summary>
    public enum DatabaseColumnGroup
    {
        /// <summary>
        /// Get all additional database columns
        /// </summary>
        Additional = -1,
        /// <summary>
        /// Get all possible database columns (DEFAULT) except Extra.
        /// </summary>
        Default = 0,
        /// <summary>
        /// Get a basic set of defined database columns 
        /// </summary>
        Basic = 1,
        /// <summary>
        /// Get the minimal set of defined database columns 
        /// </summary>
        Minimal = 2
    }
}
