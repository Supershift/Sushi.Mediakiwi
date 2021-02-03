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
    public class PortalParser : IPortalParser
    {
        public void Delete(IPortal entity)
        {
            DataParser.Delete(entity);
        }

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
            DataParser.Execute("truncate table wim_Portals");
        }

        /// <summary>
        /// Connects the specified domain.
        /// </summary>
        /// <param name="domain">The domain.</param>
        /// <returns></returns>
        //public virtual wimServerCommunication.WebInformationManagerServerService Connect(string domain)
        //{
        //    wimServerCommunication.WebInformationManagerServerService wim = new Wim.wimServerCommunication.WebInformationManagerServerService();
        //    wim.Url = string.Concat("http://", domain, "/repository/tcl.asmx");
        //    return wim;
        //}

        /// <summary>
        /// Selects the one.
        /// </summary>
        /// <param name="ID">The ID.</param>
        /// <returns></returns>
        public virtual IPortal SelectOne(int ID)
        {
            return DataParser.SelectOne<IPortal>(ID, false);
        }

        /// <summary>
        /// Selects the one.
        /// </summary>
        /// <param name="authenticode">The authenticode.</param>
        /// <returns></returns>
        public virtual IPortal SelectOne(string authenticode)
        {
            List<DatabaseDataValueColumn> whereClause = new List<DatabaseDataValueColumn>();
            whereClause.Add(new DatabaseDataValueColumn("Portal_Authenticode", SqlDbType.VarChar, authenticode));

            return DataParser.SelectOne<IPortal>(whereClause);
        }

        /// <summary>
        /// Selects the one.
        /// </summary>
        /// <param name="guid">The GUID.</param>
        /// <returns></returns>
        public virtual IPortal SelectOne(Guid guid)
        {
            List<DatabaseDataValueColumn> whereClause = new List<DatabaseDataValueColumn>();
            whereClause.Add(new DatabaseDataValueColumn("Portal_GUID", SqlDbType.UniqueIdentifier, guid));

            return DataParser.SelectOne<IPortal>(whereClause);
        }

        //TODO; SQL JOIN Implementeren
        /// <summary>
        /// Selects all.
        /// </summary>
        /// <param name="userID">The user ID.</param>
        /// <returns></returns>
        public virtual IPortal[] SelectAll(int roleID)
        {
            //List<Portal> list = new List<Portal>();
            //Portal candidate = new Portal();
            //candidate.SqlJoin = string.Format("join wim_PortalRights on PortalRight_Portal_Key = Portal_Key and PortalRight_Role_Key = {0}", roleID);

            ////List<DatabaseDataValueColumn> whereClause = new List<DatabaseDataValueColumn>();
            ////whereClause.Add(new DatabaseDataValueColumn("Portal_User_Key", SqlDbType.Int, userID));

            //foreach (object o in candidate._SelectAll(null, false, "Role", roleID.ToString()))
            //    list.Add((Portal)o);
            //return list.ToArray();


            return DataParser.SelectAll<IPortal>().ToArray();
        }

        public virtual void Save(IPortal entity)
        {
            DataParser.Save<IPortal>(entity);
        }
    }
}