using Sushi.Mediakiwi.Framework;
using System.Collections.Generic;

namespace Sushi.Mediakiwi.UI
{
    public class DataGridForJSON
    {
        public DataGridForJSON()
        {
            Rows = new List<Dictionary<string, object>>();
        }

        public static string ConvertToColumnName(ListDataColumn columnName)
        {
            return columnName.ColumnValuePropertyName.Replace(".", "_");
        }

        public List<Dictionary<string, object>> Rows { get; set; }
        public Dictionary<string, object> Sum { get; set; }
        //public List<Dictionary<string, object>> Pages { get; set; }
        //public bool Set { get; set; }
        public bool All { get; set; }

        public int Set { get; set; }
        public int Tot { get; set; }
        public int Max { get; set; }
        public int Lst { get; set; }

        public override string ToString()
        {
            return null;
            //return Wim.Utilities.JSON.Instance.ToJSON(this,
            //    new Utilities.JSONParameters()
            //    {
            //        EnableAnonymousTypes = true,
            //        UsingGlobalTypes = false
            //    }
            //);
        }
    }
}
