using System.Data;
using System.Linq;
using Sushi.Mediakiwi.Data.DalReflection;
using Sushi.Mediakiwi.Data.Parsers;

namespace Sushi.Mediakiwi.Data
{
    [DatabaseTable("wim_MenuItems", Order = "MenuItem_Position asc, MenuItem_Order asc")]
    public class MenuItem : IMenuItem
    {
        static IMenuItemParser _Parser;
        static IMenuItemParser Parser
        {
            get
            {
                if (_Parser == null)
                    _Parser = Environment.GetInstance<IMenuItemParser>();
                return _Parser;
            }
        }
		
		#region REPLICATED TO Sushi.Mediakiwi.Data.Standard
        [DatabaseColumn("MenuItem_Key", SqlDbType.Int, IsPrimaryKey = true)]
        public virtual int ID { get; set; }

        [DatabaseColumn("MenuItem_Menu_Key", SqlDbType.Int)]
        public virtual int MenuID { get; set; }
        [DatabaseColumn("MenuItem_Dashboard_Key", SqlDbType.Int, IsNullable = true)]
        public virtual int DashboardID { get; set; }
        [DatabaseColumn("MenuItem_Type_Key", SqlDbType.Int, IsNullable = true)]
        public virtual int TypeID { get; set; }
        [DatabaseColumn("MenuItem_Item_Key", SqlDbType.Int, IsNullable = true)]
        public virtual int ItemID { get; set; }
        [DatabaseColumn("MenuItem_Position", SqlDbType.Int, IsNullable = true)]
        public virtual int Position { get; set; }
        [DatabaseColumn("MenuItem_Order", SqlDbType.Int, IsNullable = true)]
        public virtual int Sort { get; set; }

        public virtual string Tag {
            get { return string.Format("{0}_{1}", this.TypeID, this.ItemID); }
        }

        public virtual string Url
        {
            get {
                switch (this.TypeID)
                {
                    case 1: 
                        return string.Concat("?list=", this.ItemID);
                    case 2:
                        return string.Concat("?folder=", this.ItemID);
                    case 3:
                        return string.Concat("?page=", this.ItemID);
                    case 4:
                        return string.Concat("?dashboard=", this.ItemID);
                    case 5:
                        return string.Concat("?gallery=", this.ItemID);
                    default:
                        return "#";
                }
            }
        }

        public virtual string Name
        {
            get
            {
                switch (this.TypeID)
                {
                    case 1:
                        return string.Concat("?list=", this.ItemID);
                    case 2:
                        return string.Concat("?folder=", this.ItemID);
                    case 3:
                        return string.Concat("?page=", this.ItemID);
                    case 4:
                        return string.Concat("?dashboard=", this.ItemID);
                    case 5:
                        return string.Concat("?gallery=", this.ItemID);
                    default:
                        return "#";
                }
            }
        }

        public bool Save()
        {
            return Parser.Save(this);
        }

        public void Delete()
        {
            Parser.Delete(this);
        }

        public static IMenuItem[] SelectAll(int menuID)
        {
            return Parser.SelectAll(menuID);
        }

        public static IMenuItem[] SelectAll_Dashboard(int dashboardID)
        {
            return Parser.SelectAll_Dashboard(dashboardID);
        }
		#endregion MOVED TO Sushi.Mediakiwi.Data.Standard
    }
}
