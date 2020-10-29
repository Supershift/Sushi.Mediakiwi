using System;
using System.Linq;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Sushi.Mediakiwi.Framework;

namespace Sushi.Mediakiwi.AppCentre.Data.Implementation
{
    /// <summary>
    /// 
    /// </summary>
    public class RoleAccessList : ComponentListTemplate
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RoleAccessList"/> class.
        /// </summary>
        public RoleAccessList()
        {
            wim.HideOpenCloseToggle = true;
            wim.HideSaveButtons = true;
            
            this.ListSearch += new ComponentSearchEventHandler(RoleAccessList_ListSearch);
            this.ListAction += new ComponentActionEventHandler(RoleAccessList_ListAction);
            this.ListLoad += new ComponentListEventHandler(RoleAccessList_ListLoad);
        }

        void RoleAccessList_ListLoad(object sender, ComponentListEventArgs e)
        {
            Sushi.Mediakiwi.Data.IApplicationRole role = Sushi.Mediakiwi.Data.ApplicationRole.SelectOne(e.SelectedGroupItemKey);
            this.Role = role.Name;
            this.IsAccessAllowed = role.IsAccessList;
        }

        /// <summary>
        /// Gets or sets the role.
        /// </summary>
        /// <value>The role.</value>
        [Sushi.Mediakiwi.Framework.ContentListItem.TextLine("Role")]
        public string Role { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is access allowed.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is access allowed; otherwise, <c>false</c>.
        /// </value>
        [Sushi.Mediakiwi.Framework.ContentListItem.Choice_Checkbox("Access list", true, "Checked selection means 'access allowed', unchecked means 'access denied'")]
        public bool IsAccessAllowed { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="RoleAccessList"/> is save.
        /// </summary>
        /// <value><c>true</c> if save; otherwise, <c>false</c>.</value>
        [Sushi.Mediakiwi.Framework.ContentListItem.Button("Save access list", IsPrimary = true, IconTarget = ButtonTarget.BottomRight)]
        public bool Save { get; set; }

        /// <summary>
        /// Handles the ListAction event of the RoleAccessList control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Sushi.Mediakiwi.Framework.ComponentActionEventArgs"/> instance containing the event data.</param>
        void RoleAccessList_ListAction(object sender, ComponentActionEventArgs e)
        {
            int[] checklist = wim.Grid.GetCheckboxChecked("HasAccess");

            var role = Sushi.Mediakiwi.Data.ApplicationRole.SelectOne(e.SelectedGroupItemKey);
            role.IsAccessList = this.IsAccessAllowed;
            role.Save();

            Sushi.Mediakiwi.Data.RoleRight.Update(checklist, Sushi.Mediakiwi.Data.RoleRightType.List, role.ID);
        }

        /// <summary>
        /// Handles the ListSearch event of the RoleAccessList control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Sushi.Mediakiwi.Framework.ComponentListSearchEventArgs"/> instance containing the event data.</param>
        void RoleAccessList_ListSearch(object sender, ComponentListSearchEventArgs e)
        {
            wim.CurrentList.Option_Search_MaxResultPerPage = 500;
            
            wim.SearchListCanClickThrough = false;

            wim.ListDataColumns.Add(new ListDataColumn("ID", "ID", ListDataColumnType.UniqueIdentifier));
            wim.ListDataColumns.Add(new ListDataColumn("Name", "Path", ListDataColumnType.HighlightPresent));
            wim.ListDataColumns.Add(new ListDataColumn("Folder access", "IsAllowed") { ColumnWidth = 100 });

            if (IsAccessAllowed)
                wim.ListDataColumns.Add(new ListDataColumn("Allowed", "HasAccess", ListDataColumnType.Checkbox) { ColumnWidth = 75, Alignment = Align.Center });
            else
                wim.ListDataColumns.Add(new ListDataColumn("Denied", "HasAccess", ListDataColumnType.Checkbox) { ColumnWidth = 75, Alignment = Align.Center });

            Sushi.Mediakiwi.Data.ApplicationUser temp = new Sushi.Mediakiwi.Data.ApplicationUser();
            temp.RoleID = e.SelectedGroupItemKey;
            Sushi.Mediakiwi.Data.IApplicationRole role = temp.Role();

            var selection =
                from item in Sushi.Mediakiwi.Data.ComponentList.SelectAll()
                join relation in role.Lists(temp) on item.GUID equals relation.GUID
                into combination
                from relation in combination.DefaultIfEmpty()
                where item.Type == Sushi.Mediakiwi.Data.ComponentListType.Undefined && item.FolderID.HasValue //&& item.IsVisible
                select new { ID = item.ID, Name = item.Name, HasAccess = relation == null ? false : true, Path = item.CompletePath(), FolderID = item.FolderID };

            if (role.All_Folders)
            {
                var ordered =
                    from item in selection
                    orderby item.Path
                    select new { ID = item.ID, Name = item.Name, HasAccess = item.HasAccess, Path = item.Path, IsAllowed = true };
                wim.ListData = ordered.ToArray();
            }
            else
            {
                var ordered =
                    from item in selection
                    join relation in role.Folders(temp) on item.FolderID equals relation.ID
                    into combination
                    from relation in combination.DefaultIfEmpty()
                    orderby item.Path
                    select new { ID = item.ID, Name = item.Name, HasAccess = item.HasAccess, Path = item.Path, IsAllowed = relation == null ? false : true }; ;
                wim.ListData = ordered.ToArray();
            }


            foreach (var item in selection)
            {
                if (temp.Role().IsAccessList)
                {
                    if (item.HasAccess)
                        wim.Grid.AddCheckboxValue("HasAccess", item.ID, true);
                }
                else if (!item.HasAccess)
                    wim.Grid.AddCheckboxValue("HasAccess", item.ID, true);
            }

        }

        /// <summary>
        /// Gets or sets the search list.
        /// </summary>
        /// <value>The search list.</value>
        [Sushi.Mediakiwi.Framework.ContentListItem.DataList()]
        public Sushi.Mediakiwi.Data.DataList SearchList { get; set; }
    }
}