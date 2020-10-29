using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Sushi.Mediakiwi.Data.DalReflection;
using Sushi.Mediakiwi.Framework;

namespace Sushi.Mediakiwi.Data.Parsers
{
    /// <summary>
    /// 
    /// </summary>
    public class RegistryParser : IRegistryParser
    {
        static ISqlEntityParser _DataParser;
        static ISqlEntityParser DataParser
        {
            get
            {
                if (_DataParser == null)
                    _DataParser = Environment.GetInstance<ISqlEntityParser>();
                return _DataParser;
            }
        }

        public virtual void Save(IRegistry entity)
        {
            DataParser.Save<IRegistry>(entity);
        }

        public virtual void Delete(IRegistry entity)
        {
            DataParser.Delete<IRegistry>(entity);
        }

        /// <summary>
        /// Selects all.
        /// </summary>
        /// <returns></returns>
        public virtual IRegistry[] SelectAll()
        {
            return DataParser.SelectAll<IRegistry>().ToArray();
        }

        /// <summary>
        /// Selects the one.
        /// </summary>
        /// <param name="ID">The identifier.</param>
        /// <returns></returns>
        public virtual IRegistry SelectOne(int ID)
        {
            return DataParser.SelectOne<IRegistry>(ID, true);
        }

        /// <summary>
        /// Selects the name of the one by.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public virtual IRegistry SelectOneByName(string name)
        {
            List<DatabaseDataValueColumn> list = new List<DatabaseDataValueColumn>();
            list.Add(new DatabaseDataValueColumn("Registry_Name", SqlDbType.VarChar, name));
            return DataParser.SelectOne<IRegistry>(list);
        }
    }
}
