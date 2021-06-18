using System.Threading.Tasks;
using Sushi.Mediakiwi.Framework;

namespace Sushi.Mediakiwi.AppCentre.UI
{
    public class ListVersioning : ComponentListTemplate
    {
        public ListVersioning()
        {
            ListSearch += ListVersioning_ListSearch;
        }

        Task ListVersioning_ListSearch(ComponentListSearchEventArgs e)
        {
            //        SqlEntityParser parser = new SqlEntityParser();
            //        DataRequest data = new DataRequest();

            //        data.AddParam("LIST", e.SelectedItemKey, SqlDbType.Int);

            //        int selected = Wim.Utility.ConvertToInt(Request.QueryString["key"]);
            //        data.AddParam("ITEM", selected, SqlDbType.Int);

            //        string sql = string.Concat(@"
            //SELECT [ComponentListVersion_Key]
            //   ,[ComponentListVersion_Created]
            //   ,[User_Displayname]
            //      ,ComponentListVersion_User_Key
            //      ,[ComponentListVersion_Listitem_Key]
            //      ,[ComponentListVersion_IsActive]
            //      ,[ComponentListVersion_Version]
            //      ,[ComponentListVersion_Type]

            //  FROM [dbo].[wim_ComponentListVersions]
            //  left join wim_Users on user_key = ComponentListVersion_User_Key
            //  where
            // [ComponentListVersion_ComponentList_Key] = @LIST", selected > 0 ? " and ComponentListVersion_Listitem_Key = @ITEM" : null, @"
            //order by 
            // [ComponentListVersion_Created] desc");
            //        ;

            //        wim.ListDataAdd(parser.ExecuteList<ListVersionItem>(sql, data));

            wim.ListDataColumns.Add(new ListDataColumn("Created", "Created") { ColumnWidth = 90 });
            wim.ListDataColumns.Add(new ListDataColumn("User", "User") { ColumnWidth = 150 });
            wim.ListDataColumns.Add(new ListDataColumn("Change", "Type") { ColumnWidth = 90 });
            wim.ListDataColumns.Add(new ListDataColumn("Version", "Version") { ColumnWidth = 90 });
            wim.ListDataColumns.Add(new ListDataColumn("Key", "itemID") { Alignment = Align.Left });
            return Task.CompletedTask;
        }

        [Sushi.Mediakiwi.Framework.ContentListItem.DataList()]
        public Sushi.Mediakiwi.Data.DataList Listing { get; set; }

        //class ListVersionItem2 : ListVersionItem
        //{ }
        //class ListVersionItem
        //{
        //    [DatabaseColumn("ComponentListVersion_Key", SqlDbType.Int, IsPrimaryKey = true)]
        //    public virtual int ID { get; set; }

        //    [DatabaseColumn("User_Displayname", SqlDbType.NVarChar, IsNullable = true)]
        //    public virtual string User { get; set; }

        //    [DatabaseColumn("ComponentListVersion_Version", SqlDbType.Int, IsNullable = true)]
        //    public virtual int Version { get; set; }

        //    [DatabaseColumn("ComponentListVersion_Listitem_Key", SqlDbType.Int, IsNullable = true)]
        //    public virtual int itemID { get; set; }

        //    [DatabaseColumn("ComponentListVersion_Type", SqlDbType.Int, IsNullable = true)]
        //    public virtual int TypeID { get; set; }
        //    public virtual string Type
        //    {
        //        get {
        //            switch(TypeID)
        //            {
        //                default: return "Introduction";
        //                case 1: return "Updated";
        //                case 2: return "Removed";
        //            }
        //        }

        //    }
        

        //    [DatabaseColumn("ComponentListVersion_Created", SqlDbType.DateTime)]
        //    public virtual DateTime Created { get; set; }
        //}
    }
}
