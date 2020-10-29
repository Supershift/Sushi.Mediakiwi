using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Sushi.Mediakiwi.Data.DalReflection;

namespace Sushi.Mediakiwi.AppCentre.Data.Extention
{
    /// <summary>
    /// 
    /// </summary>
    [DatabaseTable("wim_ComponentTemplates", Join = "left join wim_Sites on ComponentTemplate_Site_Key = Site_Key", Order = "ComponentTemplate_ReferenceId ASC")]
    public class ComponentTemplate : Sushi.Mediakiwi.Data.ComponentTemplate
    {
        /// <summary>
        /// Select a Component Template instance based on designated site and title.
        /// </summary>
        public static List<ComponentTemplate> SelectAll_BasedOnTitle(string title)
        {
            title = string.Format("%{0}%", title);
            List<DatabaseDataValueColumn> valuelist = new List<DatabaseDataValueColumn>();
            valuelist.Add(new DatabaseDataValueColumn("Site_Type", SqlDbType.Int, null));
            valuelist.Add(new DatabaseDataValueColumn("ComponentTemplate_Name", SqlDbType.NVarChar, title, 50, DatabaseDataValueCompareType.Like));

            List<ComponentTemplate> list = new List<ComponentTemplate>();
            foreach (object o in new ComponentTemplate()._SelectAll(valuelist)) list.Add((ComponentTemplate)o);
            return list;
        }

        /// <summary>
        /// Select a Component Template instance based on designated site and title.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="site">The site.</param>
        /// <returns></returns>
        public static ComponentTemplate[] SelectAll(string title, int? site)
        {
            List<DatabaseDataValueColumn> valuelist = new List<DatabaseDataValueColumn>();

            if (site.HasValue && site.Value > 0)
                valuelist.Add(new DatabaseDataValueColumn("ComponentTemplate_Site_Key", SqlDbType.Int, site));

            if (!string.IsNullOrEmpty(title))
            {
                title = string.Format("%{0}%", title);
                valuelist.Add(new DatabaseDataValueColumn("ComponentTemplate_Name", SqlDbType.NVarChar, title, 50, DatabaseDataValueCompareType.Like));
            }

            List<ComponentTemplate> list = new List<ComponentTemplate>();
            foreach (object o in new ComponentTemplate()._SelectAll(valuelist)) list.Add((ComponentTemplate)o);
            return list.ToArray();
        }

        /// <summary>
        /// Searches all.
        /// </summary>
        /// <param name="site">The site.</param>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public static List<ComponentTemplate> SearchAll(int site, string text)
        {
            List<DatabaseDataValueColumn> valuelist = new List<DatabaseDataValueColumn>();
            valuelist.Add(new DatabaseDataValueColumn("ComponentTemplate_IsListTemplate", SqlDbType.Bit, false));

            if (site > 0)
                valuelist.Add(new DatabaseDataValueColumn("ComponentTemplate_Site_Key", SqlDbType.Int, site));

            Sushi.Mediakiwi.Data.IComponentList clist = Sushi.Mediakiwi.Data.ComponentList.SelectOne(Sushi.Mediakiwi.Data.ComponentListType.ComponentTemplates);

            List<ComponentTemplate> list = new List<ComponentTemplate>();
            foreach (object o in new ComponentTemplate()._SearchAll(clist.ID, null, text, valuelist)) list.Add((ComponentTemplate)o);
            return list;
        }

        /// <summary>
        /// Select a Component Template instance based on designated site and title.
        /// </summary>
        public static List<ComponentTemplate> SelectAll_BasedOnListAndTitle(string title)
        {
            title = string.Format("%{0}%", title);
            List<DatabaseDataValueColumn> valuelist = new List<DatabaseDataValueColumn>();
            valuelist.Add(new DatabaseDataValueColumn("ComponentTemplate_IsListTemplate", SqlDbType.Bit, true));
            valuelist.Add(new DatabaseDataValueColumn("ComponentTemplate_Name", SqlDbType.NVarChar, title, 50, DatabaseDataValueCompareType.Like));

            List<ComponentTemplate> list = new List<ComponentTemplate>();
            foreach (object o in new ComponentTemplate()._SelectAll(valuelist)) list.Add((ComponentTemplate)o);
            return list;
        }
    }
}