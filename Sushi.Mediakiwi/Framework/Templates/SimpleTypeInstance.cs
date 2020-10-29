using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using Sushi.Mediakiwi.Data.DalReflection;

namespace Sushi.Mediakiwi.Framework.Templates
{
    /// <summary>
    /// 
    /// </summary>
    public class SimpleGenericsInstance : Wim.Templates.SimpleGenerics
    {
        /// <summary>
        /// Selects the one.
        /// </summary>
        /// <param name="listID">The list ID.</param>
        /// <param name="ID">The ID.</param>
        /// <returns></returns>
        internal static SimpleGenericsInstance SelectOne(int listID, int ID)
        {
            Sushi.Mediakiwi.Data.IComponentList list = Sushi.Mediakiwi.Data.ComponentList.SelectOne(listID);
            SimpleGenericsInstance implement = new SimpleGenericsInstance();
            implement.SqlTable = list.Catalog().Table;
            implement.SqlColumnPrefix = list.Catalog().ColumnPrefix;
            implement.ApplyListInformation(list);

            return (SimpleGenericsInstance)implement._SelectOne(ID);
        }

        /// <summary>
        /// Selects the single instance.
        /// </summary>
        /// <param name="listID">The list ID.</param>
        /// <param name="siteID">The site ID.</param>
        /// <returns></returns>
        internal static SimpleGenericsInstance SelectSingleInstance(int listID, int siteID)
        {
            Sushi.Mediakiwi.Data.IComponentList list = Sushi.Mediakiwi.Data.ComponentList.SelectOne(listID);

            if (list.CatalogID == 0)
                throw new Exception(string.Format("There is no catalog assigned to the requested list with the following properties:\n\nName: {2}\nID: {0}\nReference ID: {1}\n\nResolution (WIM): Assign a catalog under Administration > Templates > ComponentLists > Tab: Data.", list.ID, list.ReferenceID, list.Name));

            SimpleGenericsInstance implement = new SimpleGenericsInstance();
            implement.SqlTable = list.Catalog().Table;
            implement.ApplyListInformation(list);

            //if (List.Catalog().HasSortOrder)
            //    implement.SqlOrder = string.Concat(List.Catalog().ColumnPrefix, "_SortOrder");

            implement.SqlColumnPrefix = list.Catalog().ColumnPrefix;

            List<DatabaseDataValueColumn> where = new List<DatabaseDataValueColumn>();

            string cacheId = "";
            if (list.HasGenericSiteFilter)
            {
                where.Add(new DatabaseDataValueColumn(string.Concat(list.Catalog().ColumnPrefix, "_Site_Key"), SqlDbType.Int, siteID));
                cacheId += string.Concat("S", siteID);
                
                where.Add(new DatabaseDataValueColumn(string.Concat(list.Catalog().ColumnPrefix, "_List_Key"), SqlDbType.Int, listID));
                cacheId += string.Concat("L", listID);
            }
            SimpleGenericsInstance xx = (SimpleGenericsInstance)implement._SelectOne(where, "Instance", cacheId);

            string x = implement.SqlLastExecuted;
            return xx;
        }

        /// <summary>
        /// Selects all.
        /// </summary>
        /// <param name="componentList">The component list.</param>
        /// <param name="rowCount">The row count.</param>
        /// <returns></returns>
        public static SimpleGenericsInstance[] SelectAll(Sushi.Mediakiwi.Data.IComponentList componentList, int rowCount)
        {
            List<SimpleGenericsInstance> list = new List<SimpleGenericsInstance>();

            if (componentList.CatalogID == 0)
                throw new Exception(string.Format("There is no catalog assigned to the requested list with the following properties:\n\nName: {2}\nID: {0}\nReference ID: {1}\n\nResolution (WIM): Assign a catalog under Administration > Templates > ComponentLists > Tab: Data.", componentList.ID, componentList.ReferenceID, componentList.Name));

            SimpleGenericsInstance implement = new SimpleGenericsInstance();
            implement.ApplyListInformation(componentList);
            implement.SqlRowCount = rowCount;
            implement.SqlTable = componentList.Catalog().Table;
            //implement.SqlOrder = string.Concat(clist.Catalog().ColumnPrefix, "_SortOrder");
            implement.SqlColumnPrefix = componentList.Catalog().ColumnPrefix;

            List<DatabaseDataValueColumn> where = new List<DatabaseDataValueColumn>();

            foreach (object o in implement._SelectAll(where))
                list.Add((SimpleGenericsInstance)o);

            return list.ToArray();
        }
    }
}
