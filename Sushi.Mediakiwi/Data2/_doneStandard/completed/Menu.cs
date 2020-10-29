using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Sushi.Mediakiwi.Data.DalReflection;
using Sushi.Mediakiwi.Data.Parsers;
using Sushi.Mediakiwi.Framework;

namespace Sushi.Mediakiwi.Data
{
    [DatabaseTable("wim_Menus")]
    public class Menu : IMenu
    {
        static IMenuParser _Parser;
        static IMenuParser Parser
        {
            get
            {
                if (_Parser == null)
                    _Parser = Sushi.Mediakiwi.Data.Environment.GetInstance<IMenuParser>();
                return _Parser;
            }
        }

        #region REPLICATED TO Sushi.Mediakiwi.Data.Standard
        [DatabaseColumn("Menu_Key", SqlDbType.Int, IsPrimaryKey = true)]
        public virtual int ID { get; set; }

        [DatabaseColumn("Menu_Name", SqlDbType.NVarChar, Length = 50)]
        public virtual string Name { get; set; }
        [DatabaseColumn("Menu_Site_key", SqlDbType.Int, IsNullable = true)]
        public virtual int? SiteID { get; set; }
        [DatabaseColumn("Menu_Role_Key", SqlDbType.Int, IsNullable = true)]
        public virtual int? RoleID { get; set; }
        [DatabaseColumn("Menu_IsActive", SqlDbType.Bit)]
        public virtual bool IsActive { get; set; }

        public virtual bool Save()
        {
            return Parser.Save(this);
        }

        public virtual void Delete()
        {
            Parser.Delete(this);
        }

        public static IMenu[] SelectAll()
        {
            return Parser.SelectAll();
        }

        public static IMenu SelectOne(int ID)
        {
            return Parser.SelectOne(ID);
        }
		#endregion MOVED TO Sushi.Mediakiwi.Data.Standard
    }
}
