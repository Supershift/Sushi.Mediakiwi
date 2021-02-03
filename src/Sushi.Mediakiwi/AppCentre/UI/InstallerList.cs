using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using Sushi.Mediakiwi.Data;
using Sushi.Mediakiwi.Framework;

namespace Sushi.Mediakiwi.AppCentre.UI
{
    public class InstallerList : ComponentListTemplate
    {
        public IInstaller Implement { get; set; }
        [Sushi.Mediakiwi.Framework.ContentListItem.DataExtend()]
        public InstallerForm Form { get; set; }

        public InstallerList()
        {
            this.ListSearch += InstallerList_ListSearch;
            this.ListLoad += InstallerList_ListLoad;
            this.ListSave += InstallerList_ListSave;
            this.ListAction += InstallerList_ListAction;
            this.ListDataReport += InstallerList_ListDataReport;

            wim.HideProperties = true;
            wim.CurrentList.Option_CanSaveAndAddNew = false;
            wim.CurrentList.Label_Save = "Install";
            wim.CurrentList.Label_NewRecord = "Add new module";
            wim.CurrentList.Option_HasDataReport = true;
        }

        private void InstallerList_ListDataReport(object sender, ComponentDataReportEventArgs e)
        {
            int count = 0;
            var installed = Sushi.Mediakiwi.Data.Installer.SelectAll();
            foreach(var i in installed)
            {
                if (i.Version != i.GetActualVersion())
                    count++;
            }
            if (count == 0)
                return;

            e.ReportCount = count;
            e.IsAlert = true;
        }

        private void InstallerList_ListAction(object sender, ComponentActionEventArgs e)
        {
            if (Uninstall)
            {
                Implement.Uninstall();
                Implement.Delete();
                wim.Notification.AddNotificationAlert(string.Format("The uninstall of <b>{0}</b> has been succesfully executed", Implement.Name), true);
                Response.Redirect(wim.GetCurrentQueryUrl(true, new KeyValue() { RemoveKey = true, Key = "item" }));
            }
            else if (Upgrade)
            {
                Implement.Upgrade();
                Implement.Version = Implement.GetActualVersion();
                Implement.Save();
            }
            Response.Redirect(Wim.Utility.GetSafeUrl(Request));
        }

        private void InstallerList_ListSave(IComponentListTemplate sender, ComponentListEventArgs e)
        {
            Implement = Form.ToEntity();
            Implement.Installed = Sushi.Mediakiwi.Data.Common.DatabaseDateTime;
            Implement.Save();

            Implement.Setup();
            wim.Notification.AddNotificationAlert(string.Format("The installation of <b>{0}</b> has been succesfully executed", Implement.Name), true);
            Response.Redirect(wim.GetCurrentQueryUrl(true, new KeyValue() { RemoveKey = true, Key = "item" }));
        }

        private void InstallerList_ListLoad(IComponentListTemplate sender, ComponentListEventArgs e)
        {
            wim.SetPropertyVisibility("Upgrade", false);
            wim.SetPropertyVisibility("Uninstall", false);

            Implement = Installer.SelectOne(e.SelectedKey);
            Form = new InstallerForm(Implement, Context);
            if (e.SelectedKey > 0)
            {
                wim.HideEditOption = true;
                wim.SetPropertyVisibility("Installer", false);
                wim.SetPropertyVisibility("Folder", false);
                wim.ListInfoApply(string.Format("{0} (v{1})", Implement.Name, Implement.Version.ToString()), null, Implement.Description);

                if (Implement.Installed.HasValue)
                    wim.SetPropertyVisibility("Uninstall", true);

                if (Implement.Version != Implement.GetActualVersion())
                    wim.SetPropertyVisibility("Upgrade", true);
            }
        }

        [Sushi.Mediakiwi.Framework.ContentListItem.Button("Upgrade", IsPrimary = true)]
        public bool Upgrade { get; set; }
        [Sushi.Mediakiwi.Framework.ContentListItem.Button("Uninstall", AskConfirmation = true)]
        public bool Uninstall { get; set; }

        class ExtendendInstallableVersion : InstallableVersion
        {
            public bool IsDone { get; set; }
        }

        private void InstallerList_ListSearch(object sender, ComponentListSearchEventArgs e)
        {
            //  Inner load listSearch
            if (e.SelectedItemKey > 0)
            {
                wim.ListDataColumns.Add(new ListDataColumn("Log", "Information"));

                var info = Implement.GetInformation();

                if (info == null)
                {
                    info = new InstallableVersion[] { new InstallableVersion() { Version = 1, Information = "No information supplied." } };
                    wim.ListDataApply(info);

                }
                else
                {
                    List<ExtendendInstallableVersion> extend = new List<ExtendendInstallableVersion>();
                    var sorted = (from item in info orderby item.Version descending select item).ToList();
                    foreach (var x in sorted)
                    {
                        bool isDone = x.Version <= Implement.Version;
                        extend.Add(new ExtendendInstallableVersion() { Information = x.Information, Version = x.Version, IsDone = isDone });
                    }

                    wim.ListDataColumns.Add(new ListDataColumn("Version", "Version") { ColumnWidth = 40 });
                    wim.ListDataColumns.Add(new ListDataColumn("", "IsDone") { ColumnWidth = 40 });
                    wim.ListDataApply(extend);

                }

                return;
            }

            var list = Sushi.Mediakiwi.Data.Installer.SelectAll();
            wim.ListDataApply(list);

            wim.ListDataColumns.Add(new ListDataColumn("", "ID", ListDataColumnType.UniqueIdentifier));
            wim.ListDataColumns.Add(new ListDataColumn("Name", "Name", ListDataColumnType.HighlightPresent));
            wim.ListDataColumns.Add(new ListDataColumn("Description", "Description"));
            wim.ListDataColumns.Add(new ListDataColumn("Version", "Version") { ColumnWidth = 40 });
            wim.ListDataColumns.Add(new ListDataColumn("Upgrade", "HasUpdate") { ColumnWidth = 40 });
        }
    }
}
