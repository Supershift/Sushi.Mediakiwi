using System;
using System.Data;
using System.Data.Linq.Mapping;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sushi.Mediakiwi.Data.DalReflection;
using Sushi.Mediakiwi.Framework;
using Sushi.Mediakiwi.Data.Parsers;

namespace Sushi.Mediakiwi.Data
{
    /// <summary>
    /// 
    /// </summary>
    [DatabaseTable("wim_PortalRights")]
    public class PortalRight : IPortalRight
    {
        static IPortalRightParser _Parser;
        static IPortalRightParser Parser
        {
            get
            {
                if (_Parser == null)
                    _Parser = Environment.GetInstance<IPortalRightParser>();
                return _Parser;
            }
        }

        #region REPLICATED TO Sushi.Mediakiwi.Data.Standard

        public PortalRight()
        {
        }

        public static IPortalRight SelectOne(int ID)
        {
            return Parser.SelectOne(ID);
        }

        public static IPortalRight[] SelectAll(int roleID)
        {
            return Parser.SelectAll(roleID);
        }

        [DatabaseColumn("PortalRight_Key", SqlDbType.Int, IsPrimaryKey = true)]
        public virtual int ID { get; set; }

        [DatabaseColumn("PortalRight_Role_Key", SqlDbType.Int)]
        public virtual int RoleID { get; set; }

        [DatabaseColumn("PortalRight_Portal_Key", SqlDbType.Int)]
        public virtual int PortalID { get; set; }

        #endregion REPLICATED TO Sushi.Mediakiwi.Data.Standard
    }
}
