using System;
using System.Web;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Sushi.Mediakiwi.Data.DalReflection;

namespace Sushi.Mediakiwi.Beta.GeneratedCms.Supporting
{
    internal class SortOrderUpdate : DatabaseEntity
    {
        // [MR:10-09-2014] the serverPortal parameter is new
        internal static bool UpdateSortOrder(string sqlTable, string sqlColumn, string sqlKey, int sortF, int sortT, Sushi.Mediakiwi.Framework.WimServerPortal serverPortal = null)
        {
            if (string.IsNullOrEmpty(sqlTable) || string.IsNullOrEmpty(sqlColumn)) return false;
            SortOrderUpdate implement = new SortOrderUpdate();

            // [MR:10-09-2014] this is added, to implement a portal connection
            if (serverPortal != null)
                implement.DatabaseMappingPortal = serverPortal;

            int F = Utility.ConvertToInt(implement.Execute(string.Format("select {3} from {2} where {4} = {0}", sortF, sortT, sqlTable, sqlColumn, sqlKey)));
            int T = Utility.ConvertToInt(implement.Execute(string.Format("select {3} from {2} where {4} = {1}", sortF, sortT, sqlTable, sqlColumn, sqlKey)));

            if (F > T)
            {
                string q1 = string.Format("update {2} set {3} = {3} + 1 where {3} >= {6} and {3} <= {5}", sortF, sortT, sqlTable, sqlColumn, sqlKey, F, T);
                string q2 = string.Format("update {2} set {3} = (select {3} -1 from {2} where {4} = {1}) where {4} = {0}", sortF, sortT, sqlTable, sqlColumn, sqlKey);


                implement.Execute(q1);
                implement.Execute(q2);
            }
            else
            {
                string q1 = string.Format("update {2} set {3} = {3} - 1 where {3} >= {5} and {3} <= {6}", sortF, sortT, sqlTable, sqlColumn, sqlKey, F, T);
                string q2 = string.Format("update {2} set {3} = (select {3} from {2} where {4} = {1}) where {4} = {0}", sortF, sortT, sqlTable, sqlColumn, sqlKey);
                string q3 = string.Format("update {2} set {3} = (select {3} +1 from {2} where {4} = {1}) where {4} = {1}", sortF, sortT, sqlTable, sqlColumn, sqlKey);

                implement.Execute(q1);
                implement.Execute(q2);
                implement.Execute(q3);
            }

            return true;
//            implement.Execute(string.Format(@"DECLARE @F INT; DECLARE @T INT
// select @F = {3} from {2} where {4} = {0}
// select @T = {3} from {2} where {4} = {1}
// IF (@F > @T)
// BEGIN
//	update {2} set {3} = {3} + 1 where {3} >= @T and {3} <= @F	
//	update {2} set {3} = (select {3} -1 from {2} where {4} = {1}) where {4} = {0}
// END
// ELSE
// BEGIN
//	update {2} set {3} = {3} - 1 where {3} >= @F and {3} <= @T
//	update {2} set {3} = (select {3} from {2} where {4} = {1}) where {4} = {0}
//	update {2} set {3} = (select {3} +1 from {2} where {4} = {1}) where {4} = {1}
// END"
//                , sortF, sortT, sqlTable, sqlColumn, sqlKey));


            implement.Execute(string.Format(@"DECLARE @F INT; DECLARE @T INT
 select @F = FormElement_SortOrder from wim_FormElements where FormElement_Key = 7
 select @T = FormElement_SortOrder from wim_FormElements where FormElement_Key = 2
 IF (7 > 2)
 BEGIN
	update wim_FormElements set FormElement_SortOrder = FormElement_SortOrder + 1 where FormElement_SortOrder >= 2 and FormElement_SortOrder <= 7	
	update wim_FormElements set FormElement_SortOrder = (select FormElement_SortOrder -1 from wim_FormElements where FormElement_Key = 2) where FormElement_Key = 7
 END
 ELSE
 BEGIN
	update wim_FormElements set FormElement_SortOrder = FormElement_SortOrder - 1 where FormElement_SortOrder >= 7 and FormElement_SortOrder <= 2
	update wim_FormElements set FormElement_SortOrder = (select FormElement_SortOrder from wim_FormElements where FormElement_Key = 2) where FormElement_Key = 7
	update wim_FormElements set FormElement_SortOrder = (select FormElement_SortOrder +1 from wim_FormElements where FormElement_Key = 2) where FormElement_Key = 2
 END"
                , sortF, sortT, sqlTable, sqlColumn, sqlKey));


            return true;
        }
    }
}
