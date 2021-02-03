using System;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sushi.Mediakiwi.Framework;
using Sushi.Mediakiwi.Data;


namespace Sushi.Mediakiwi.AppCentre.Data
{
    public class ExportFile : ComponentListTemplate
    {
        public ExportFile()
        {
            wim.HideEditOption = true;
            wim.HideProperties = true;
            wim.Page.HideTabs = true;
            wim.CanContainSingleInstancePerDefinedList = true;
            this.ListLoad += ExportFile_ListLoad;
            this.ListSave += ExportFile_ListSave;
        }

        [Sushi.Mediakiwi.Framework.ContentListItem.TextLine("Type")]
        public string Type { get; set; }

        public string CancelURL { get; set; }
        [Sushi.Mediakiwi.Framework.ContentListItem.Button("Cancel", true, IconTarget = ButtonTarget.BottomLeft, NoPostBack = true)]
        public bool ButtonCancel { get; set; }

        public string ExportURL { get; set; }
        [Sushi.Mediakiwi.Framework.ContentListItem.Button("Export", true, IconTarget = ButtonTarget.BottomRight, NoPostBack = true)]
        public bool ButtonExport { get; set; }

        void ExportFile_ListLoad(object sender, ComponentListEventArgs e)
        {
            int componentListToExport = Wim.Utility.ConvertToInt(Request.QueryString["q"]);
            int exportType = Wim.Utility.ConvertToInt(Request.QueryString["t"], 1);
            var list = Sushi.Mediakiwi.Data.ComponentList.SelectOne(componentListToExport);

            //  XLS
            if (exportType == 1)
            {
                this.Type = "Microsoft Excel 97-2003";

                this.ExportURL = wim.GetCurrentQueryUrl(true
                    , new KeyValue() { Key = "list", Value = componentListToExport }
                    , new KeyValue() { Key = "openinframe", RemoveKey = true }
                    , new KeyValue() { Key = "xls", Value = 1 }
                    );
            }
         }

        void ExportFile_ListSave(object sender, ComponentListEventArgs e)
        {
            //Response.Cache.SetCacheability(HttpCacheability.NoCache);
            //Response.Cache.SetAllowResponseInBrowserHistory(false);

            //wim.IsExportMode_XLS = true;
            //var component = new Component() { m_IsNewDesign = true };
            //DataGrid grid;

            //component.CreateSearchList(m_Console, 0);
            //var url = grid.GetGridFromListInstanceForXLS(m_Console, m_Console.CurrentListInstance, 0);
            //m_Console.Response.Redirect(url);
            ////  Reset
            //m_Console.CurrentListInstance.wim.IsExportMode_XLS = false;

            //Response.Redirect(this.ExportURL);
        }
    }
}
