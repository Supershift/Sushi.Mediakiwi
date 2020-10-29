using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sushi.Mediakiwi.Data.DalReflection;
using Sushi.Mediakiwi.Framework;

namespace Sushi.Mediakiwi.Data.Parsers
{
    public class ApplicationRoleParser : IApplicationRoleParser
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

        /// <summary>
        /// Clears this instance.
        /// </summary>
        public virtual void Clear()
        {
            if (!Wim.CommonConfiguration.IS_LOAD_BALANCED) return;
            DataParser.Execute("truncate table wim_Roles");
        }

        /// <summary>
        /// Selects all.
        /// </summary>
        /// <returns></returns>
        public virtual IApplicationRole[] SelectAll()
        {
            return DataParser.SelectAll<IApplicationRole>().ToArray();
        }

        //TODO; SQL JOIN
        /// <summary>
        /// Selects all roles that have access to a certain folder.
        /// </summary>
        /// <param name="folderID">The folder ID.</param>
        /// <returns></returns>
        public virtual IApplicationRole[] SelectAll(int folderID)
        {
            //implement.SqlJoin = "join wim_RoleRights on RoleRight_Role_Key = Role_Key";

            List<DatabaseDataValueColumn> where = new List<DatabaseDataValueColumn>();
            where.Add(new DatabaseDataValueColumn("RoleRight_Child_Type", SqlDbType.Int, 6));
            where.Add(new DatabaseDataValueColumn("RoleRight_Child_Key", SqlDbType.Int, folderID));

            return DataParser.SelectAll<IApplicationRole>(where).ToArray();
        }

        /// <summary>
        /// Delete an implementation record.
        /// </summary>
        /// <returns></returns>
        public virtual bool Delete(int ID)
        {
            //IApplicationRole implement = new IApplicationRole();
            //implement.Execute(string.Concat("delete from wim_RoleRights where RoleRight_Role_Key = ", this.ID));
            //return base.Delete();

            return DataParser.Execute(string.Concat("delete from wim_RoleRights where RoleRight_Role_Key = ", ID));
        }

        /// <summary>
        /// Selects the one.
        /// </summary>
        /// <param name="ID">The ID.</param>
        /// <returns></returns>
        public virtual IApplicationRole SelectOne(int ID)
        {
            return DataParser.SelectOne<IApplicationRole>(ID, true);
        }

        public virtual void Save(IApplicationRole entity)
        {
            DataParser.Save<IApplicationRole>(entity);
        }
    }
}