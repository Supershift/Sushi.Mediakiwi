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
    public class GenericInstance : Wim.Templates.Generic
    {
        /// <summary>
        /// Selects the one.
        /// </summary>
        /// <param name="listID">The list ID.</param>
        /// <param name="ID">The ID.</param>
        /// <returns></returns>
        internal static new GenericInstance SelectOne(int listID, int ID)
        {
            Sushi.Mediakiwi.Data.IComponentList list = Sushi.Mediakiwi.Data.ComponentList.SelectOne(listID);
            GenericInstance implement = new GenericInstance();
            if (list != null && list.Catalog() != null)
            {
                implement.SqlTable = list.Catalog().Table;
                implement.SqlOrder = string.Concat(list.Catalog().ColumnPrefix, "_SortOrder");
                implement.SqlColumnPrefix = list.Catalog().ColumnPrefix;
            }
            return (GenericInstance)implement._SelectOne(ID);
        }


        /// <summary>
        /// Selects the single instance.
        /// </summary>
        /// <param name="listID">The list ID.</param>
        /// <param name="siteID">The site ID.</param>
        /// <returns></returns>
        internal static GenericInstance SelectSingleInstance(int listID, int siteID)
        {
            Sushi.Mediakiwi.Data.IComponentList list = Sushi.Mediakiwi.Data.ComponentList.SelectOne(listID);

            if (list.CatalogID == 0)
                throw new Exception(string.Format("There is no catalog assigned to the requested list with the following properties:\n\nName: {2}\nID: {0}\nReference ID: {1}\n\nResolution (WIM): Assign a catalog under Administration > Templates > ComponentLists > Tab: Data.", list.ID, list.ReferenceID, list.Name));

            GenericInstance implement = new GenericInstance();
            implement.SqlTable = list.Catalog().Table;
            
            //if (List.Catalog().HasSortOrder)
            //    implement.SqlOrder = string.Concat(List.Catalog().ColumnPrefix, "_SortOrder");

            implement.SqlColumnPrefix = list.Catalog().ColumnPrefix;

            List<DatabaseDataValueColumn> where = new List<DatabaseDataValueColumn>();

            string cacheId = "";
            if (list.HasGenericSiteFilter)
            {
                where.Add(new DatabaseDataValueColumn(string.Concat(list.Catalog().ColumnPrefix, "_Site_Key"), SqlDbType.Int, siteID));
                cacheId += string.Concat("S", siteID);
            }
            if (list.HasGenericListFilter)
            {
                where.Add(new DatabaseDataValueColumn(string.Concat(list.Catalog().ColumnPrefix, "_List_Key"), SqlDbType.Int, listID));
                cacheId += string.Concat("L", listID);
            }
            GenericInstance xx = (GenericInstance)implement._SelectOne(where, "Instance", cacheId);

            string x = implement.SqlLastExecuted;
            return xx;
        }

        /// <summary>
        /// Selects all.
        /// </summary>
        /// <param name="listID">The list ID.</param>
        /// <param name="siteID">The site ID.</param>
        /// <param name="rowCount">The row count.</param>
        /// <returns></returns>
        public static GenericInstance[] SelectAll(int listID, int? siteID, int rowCount)
        {
            List<GenericInstance> list = new List<GenericInstance>();
            Sushi.Mediakiwi.Data.IComponentList clist = Sushi.Mediakiwi.Data.ComponentList.SelectOne(listID);

            if (clist.CatalogID == 0)
                throw new Exception(string.Format("There is no catalog assigned to the requested list with the following properties:\n\nName: {2}\nID: {0}\nReference ID: {1}\n\nResolution (WIM): Assign a catalog under Administration > Templates > ComponentLists > Tab: Data.", clist.ID, clist.ReferenceID, clist.Name));
            
            GenericInstance implement = new GenericInstance();

            implement.SqlRowCount = rowCount;
            implement.SqlTable = clist.Catalog().Table;
            implement.SqlOrder = string.Concat(clist.Catalog().ColumnPrefix, "_SortOrder");
            implement.SqlColumnPrefix = clist.Catalog().ColumnPrefix;

            List<DatabaseDataValueColumn> where = new List<DatabaseDataValueColumn>();

            if (siteID.HasValue && clist.Data["wim_GenericBySite"].ParseBoolean(true))
                where.Add(new DatabaseDataValueColumn(string.Concat(clist.Catalog().ColumnPrefix, "_Site_Key"), SqlDbType.Int, siteID.Value));

            if (clist.Data["wim_GenericByList"].ParseBoolean(true))
                where.Add(new DatabaseDataValueColumn(string.Concat(clist.Catalog().ColumnPrefix, "_List_Key"), SqlDbType.Int, listID));

            //if (onlyReturnCompleted)
            //    where.Add(new DatabaseDataValueColumn("NOT Order_Completed", SqlDbType.DateTime, null));

            foreach (object o in implement._SelectAll(where)) 
                list.Add((GenericInstance)o);

            return list.ToArray();
        }
    }
}
