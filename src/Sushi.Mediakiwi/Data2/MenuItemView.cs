using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Sushi.Mediakiwi.Data.DalReflection;
using Sushi.Mediakiwi.Framework;
using Wim.Utilities;

namespace Sushi.Mediakiwi.Data
{
    public class MenuItemView : IMenuItemView
    {
        static IMenuItemViewParser _Parser;
        static IMenuItemViewParser Parser
        {
            get
            {
                if (_Parser == null)
                    _Parser = Environment.GetInstance<IMenuItemViewParser>();
                return _Parser;
            }
        }

        [DatabaseColumn("Menu_Key", SqlDbType.Int, IsPrimaryKey = true)]
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
        [DatabaseColumn("SearchView_Title", SqlDbType.NVarChar, Length = 50)]
        public virtual string Name { get; set; }

        [DatabaseColumn("MenuItem_Section", SqlDbType.Int, IsNullable = true)]
        
        public virtual int Section { get; set; }

        [DatabaseColumn("SearchView_Site_Key", SqlDbType.Int, IsNullable = true)]

        public virtual int SiteID { get; set; }
    
        public virtual string Tag
        {
            get { return string.Format("{0}_{1}", this.TypeID, this.ItemID); }
        }

        public virtual string Url(int currentChannelID)
        {
            return Parser.Url(this, currentChannelID);
        }

        public static IMenuItemView[] SelectAll(int siteID, int roleID)
        {
            return Parser.SelectAll(siteID, roleID);
        }

        /// <summary>
        /// Selects all.
        /// </summary>
        /// <param name="siteID">The site ID.</param>
        /// <param name="roleID">The role ID.</param>
        /// <param name="items">The positions.</param>
        /// <returns></returns>
        public static IMenuItemView[] SelectAll(int siteID, int roleID, params int[] items)
        {
            return Parser.SelectAll(siteID, roleID, items);
        }

        /// <summary>
        /// Selects al menu items for use on the dashboard
        /// </summary>
        /// <param name="dashboardID">The dashboard identifier.</param>
        /// <returns></returns>
        public static IMenuItemView[] SelectAll_Dashboard(int dashboardID)
        {
            return Parser.SelectAll_Dashboard(dashboardID);
        }
    }
}
