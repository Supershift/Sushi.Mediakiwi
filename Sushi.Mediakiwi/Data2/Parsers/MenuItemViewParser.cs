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
    public class MenuItemViewParser : IMenuItemViewParser
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

        public virtual string Url(IMenuItemView entity, int currentChannelID)
        {
            //get {
                
                switch (entity.TypeID)
                {
                    case 1: return Wim.Utility.AddApplicationPath(string.Format("/{2}{0}?list={1}", Sushi.Mediakiwi.Data.Environment.Current.RelativePath, entity.ItemID, currentChannelID));
                    case 2: return Wim.Utility.AddApplicationPath(string.Format("/{2}{0}?folder={1}", Sushi.Mediakiwi.Data.Environment.Current.RelativePath, entity.ItemID, currentChannelID));
                    case 3: return Wim.Utility.AddApplicationPath(string.Format("/{2}{0}?page={1}", Sushi.Mediakiwi.Data.Environment.Current.RelativePath, entity.ItemID, currentChannelID));
                    case 4: return Wim.Utility.AddApplicationPath(string.Format("/{2}{0}?dashboard={1}", Sushi.Mediakiwi.Data.Environment.Current.RelativePath, entity.ItemID, currentChannelID));
                    case 5: return Wim.Utility.AddApplicationPath(string.Format("/{2}{0}?gallery={1}", Sushi.Mediakiwi.Data.Environment.Current.RelativePath, entity.ItemID, currentChannelID));
                    case 6: return Wim.Utility.AddApplicationPath(string.Format("/{1}{0}?top=1", Sushi.Mediakiwi.Data.Environment.Current.RelativePath, entity.ItemID));
                    case 7: return Wim.Utility.AddApplicationPath(string.Format("/{2}{0}?top={1}", Sushi.Mediakiwi.Data.Environment.Current.RelativePath, entity.ItemID, currentChannelID));
                    case 8: return Wim.Utility.AddApplicationPath(string.Format("/{2}{0}?folder={1}", Sushi.Mediakiwi.Data.Environment.Current.RelativePath, entity.ItemID, currentChannelID));
                }
                return "#";
            //}
        }


        public virtual IMenuItemView[] SelectAll(int siteID, int roleID)
        {
            return SelectAll(siteID, roleID, 1,2,3,4,5,6,7,8);
        }

        /// <summary>
        /// Selects all.
        /// </summary>
        /// <param name="siteID">The site ID.</param>
        /// <param name="roleID">The role ID.</param>
        /// <param name="items">The positions.</param>
        /// <returns></returns>
        public virtual IMenuItemView[] SelectAll(int siteID, int roleID, params int[] items)
        {
            string sql = string.Format(@"
select 
    wim_MenuItems.*
,   SearchView_Title
,   SearchView_Site_Key 
from 
    wim_Menus
    join wim_MenuItems on Menu_Key = MenuItem_Menu_Key
	join wim_SearchView on SearchView_Type = MenuItem_Type_Key and MenuItem_Item_Key = SearchView_Item_Key
where
    ISNULL(Menu_Site_key, {0}) = {0}
    and ISNULL(Menu_Role_Key, {1}) = {1}
    and Menu_IsActive = 1{2}
order by
    MenuItem_Position asc, MenuItem_Order asc 
"
    , siteID, roleID, items == null || items.Length == 0 ? "" : string.Format(" and MenuItem_Position in ({0})", Wim.Utility.ConvertToCsvString(items)));

            return DataParser.ExecuteList<IMenuItemView>(sql).ToArray();
        }

        /// <summary>
        /// Selects al menu items for use on the dashboard
        /// </summary>
        /// <param name="dashboardID">The dashboard identifier.</param>
        /// <returns></returns>
        public virtual IMenuItemView[] SelectAll_Dashboard(int dashboardID)
        {
            string sql = string.Format(@"
select 
    wim_MenuItems.*
,   SearchView_Title
,   SearchView_Site_Key 
from 
    wim_MenuItems
	join wim_SearchView on SearchView_Type = MenuItem_Type_Key and MenuItem_Item_Key = SearchView_Item_Key
where
    MenuItem_Dashboard_key = {0}
order by
    MenuItem_Position asc, MenuItem_Order asc 
"
                , dashboardID);
            return DataParser.ExecuteList<IMenuItemView>(sql).ToArray();
        }
    }
}
