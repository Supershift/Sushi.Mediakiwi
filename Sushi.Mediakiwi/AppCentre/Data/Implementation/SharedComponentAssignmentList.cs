using System;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sushi.Mediakiwi.Framework;

namespace Sushi.Mediakiwi.AppCentre.Data.Implementation
{
    /// <summary>
    /// Represents a ComponentList entity.
    /// </summary>
    public class SharedComponentAssignmentList : ComponentListTemplate
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SharedComponentAssignmentList"/> class.
        /// </summary>
        public SharedComponentAssignmentList()
        {
            wim.HideOpenCloseToggle = true;

            this.ListSearch += new ComponentSearchEventHandler(SharedComponentAssignmentList_ListSearch);
            this.ListSave += new ComponentListEventHandler(SharedComponentAssignmentList_ListSave);
        }

        [Sushi.Mediakiwi.Framework.ContentListItem.Button("Update list", true, IconTarget = Sushi.Mediakiwi.Framework.ButtonTarget.BottomRight, IconType = Sushi.Mediakiwi.Framework.ButtonIconType.Approve)]
        public bool ButtonSave { get; set; }

        void SharedComponentAssignmentList_ListSave(object sender, ComponentListEventArgs e)
        {
            var component = Sushi.Mediakiwi.Data.ComponentVersion.SelectOne(e.SelectedGroupItemKey);
            var selection = Sushi.Mediakiwi.Data.ComponentTargetPage.SelectAll(component.TemplateID, wim.CurrentSite.ID);

            var current = Sushi.Mediakiwi.Data.ComponentTarget.SelectAll(component.GUID);

            foreach (var item in selection)
            {
                bool isChecked = wim.Grid.IsCheckboxChecked("State", item.ID);
                var select = (from element in current where element.Target == item.Version_GUID select element).FirstOrDefault();
                if (!isChecked && select != null)
                    select.Delete();

                if (select == null && isChecked)
                {
                    var target = new Sushi.Mediakiwi.Data.ComponentTarget();
                    target.Target = item.Version_GUID;
                    target.Source = component.GUID;
                    target.PageID = item.PageID;
                    target.DeleteCompetion();
                    target.Save();
                }
                else if (isChecked)
                {
                    select.DeleteCompetion();
                }
            }
            Sushi.Mediakiwi.Data.EnvironmentVersionLogic.Flush();
            //Wim.Utilities.CacheItemManager.FlushAllPageData(System.Web.HttpContext.Current);
        }

        void SharedComponentAssignmentList_ListSearch(object sender, ComponentListSearchEventArgs e)
        {
            wim.ListDataColumns.Add("", "State", ListDataColumnType.Checkbox, 30);
            wim.ListDataColumns.Add("ID", "ID", ListDataColumnType.UniqueIdentifier);
            wim.ListDataColumns.Add("Path", "Path", ListDataColumnType.HighlightPresent);
            wim.ListDataColumns.Add("Position", "Position", 50, Align.Center);
            wim.ListDataColumns.Add("Assigned", "AssignedComponent");
            wim.ListDataColumns.Add("Page", "IsActivePage", 50);
            wim.ListDataColumns.Add("Component", "IsActive", 50);
            wim.ListDataColumns.Add("Live", "IsPublished", 50);

            var component = Sushi.Mediakiwi.Data.ComponentVersion.SelectOne(e.SelectedGroupItemKey);
            var items = Sushi.Mediakiwi.Data.ComponentTarget.SelectAll(component.GUID);

            wim.ListTitle = component.Name;

            foreach (var item in items)
            {
                //  Clean up
                var XX = Sushi.Mediakiwi.Data.ComponentVersion.SelectOne(item.Target);
                wim.Grid.AddCheckboxValue("State", XX.ID, true);
            }

            wim.SearchListCanClickThrough = false;
            wim.ListData = Sushi.Mediakiwi.Data.ComponentTargetPage.SelectAll(component.TemplateID, wim.CurrentSite.ID);
        }

        [Sushi.Mediakiwi.Framework.ContentListItem.DataList()]
        public Sushi.Mediakiwi.Data.DataList ImplementList { get; set; }
    }
}
