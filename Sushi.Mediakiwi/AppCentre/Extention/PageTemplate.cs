using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Sushi.Mediakiwi.Data.DalReflection;

namespace Sushi.Mediakiwi.AppCentre.Data.Extention
{
    /// <summary>
    /// 
    /// </summary>
    [DatabaseTable("wim_PageTemplates", Order = "PageTemplate_ReferenceId ASC")]
    public class PageTemplate : Sushi.Mediakiwi.Data.PageTemplate
    {
        /// <summary>
        /// Select a Page Template instance based on designated site and title.
        /// </summary>
        public static List<PageTemplate> SelectAll_BasedOnTitle(string title)
        {
            title = string.Format("%{0}%", title);
            List<DatabaseDataValueColumn> valuelist = new List<DatabaseDataValueColumn>();
            valuelist.Add(new DatabaseDataValueColumn("PageTemplate_Name", SqlDbType.NVarChar, title, 50, DatabaseDataValueCompareType.Like));

            List<PageTemplate> list = new List<PageTemplate>();
            foreach (object o in new PageTemplate()._SelectAll(valuelist)) list.Add((PageTemplate)o);
            return list;
        }

        /// <summary>
        /// Select a Page Template instance based on designated site and title.
        /// </summary>
        public static List<PageTemplate> SelectAll_BasedOnSiteAndTitle(int? site, string title)
        {
            title = string.Format("%{0}%", title);
            List<DatabaseDataValueColumn> valuelist = new List<DatabaseDataValueColumn>();
            valuelist.Add(new DatabaseDataValueColumn("PageTemplate_Site_Key", SqlDbType.Int, site));
            valuelist.Add(new DatabaseDataValueColumn("PageTemplate_Name", SqlDbType.NVarChar, title, 50, DatabaseDataValueCompareType.Like));

            List<PageTemplate> list = new List<PageTemplate>();
            foreach (object o in new PageTemplate()._SelectAll(valuelist)) list.Add((PageTemplate)o);
            return list;
        }
    }
}
