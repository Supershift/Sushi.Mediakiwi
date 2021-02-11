﻿using System;
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
    public class RoleAccessFolder : ComponentListTemplate
    {
         /// <summary>
        /// Initializes a new instance of the <see cref="RoleAccessFolder"/> class.
        /// </summary>
        public RoleAccessFolder()
        {
            wim.HideOpenCloseToggle = true;
            wim.HideSaveButtons = true;
            
            this.ListSearch += new ComponentSearchEventHandler(RoleAccessFolder_ListSearch);
            this.ListAction += new ComponentActionEventHandler(RoleAccessFolder_ListAction);
            this.ListLoad += new ComponentListEventHandler(RoleAccessFolder_ListLoad);
        }

        void RoleAccessFolder_ListLoad(object sender, ComponentListEventArgs e)
        {
            Sushi.Mediakiwi.Data.IApplicationRole role = Sushi.Mediakiwi.Data.ApplicationRole.SelectOne(e.SelectedGroupItemKey);
            this.Role = role.Name;
            this.IsAccessAllowed = role.IsAccessFolder;
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
        /// Gets or sets a value indicating whether this <see cref="RoleAccessFolder"/> is save.
        /// </summary>
        /// <value><c>true</c> if save; otherwise, <c>false</c>.</value>
        [Sushi.Mediakiwi.Framework.ContentListItem.Button("Save access list", IconTarget = ButtonTarget.BottomRight)]
        public bool Save { get; set; }

        /// <summary>
        /// Handles the ListAction event of the RoleAccessFolder control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Sushi.Mediakiwi.Framework.ComponentActionEventArgs"/> instance containing the event data.</param>
        void RoleAccessFolder_ListAction(object sender, ComponentActionEventArgs e)
        {
            int[] checklist = wim.Grid.GetCheckboxChecked("HasAccess");

            Sushi.Mediakiwi.Data.IApplicationRole role = Sushi.Mediakiwi.Data.ApplicationRole.SelectOne(e.SelectedGroupItemKey);
            role.IsAccessFolder = this.IsAccessAllowed;
            role.Save();

            Sushi.Mediakiwi.Data.RoleRight.Update(checklist, Sushi.Mediakiwi.Data.RoleRightType.Folder, role.ID);
        }

        /// <summary>
        /// Handles the ListSearch event of the RoleAccessFolder control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Sushi.Mediakiwi.Framework.ComponentListSearchEventArgs"/> instance containing the event data.</param>
        void RoleAccessFolder_ListSearch(object sender, ComponentListSearchEventArgs e)
        {
            wim.CurrentList.Option_Search_MaxResultPerPage = 500;
            
            wim.SearchListCanClickThrough = false;
            wim.ListDataColumns.Add("ID", "ID", ListDataColumnType.UniqueIdentifier);
            wim.ListDataColumns.Add("Name", "Path", ListDataColumnType.HighlightPresent);
            wim.ListDataColumns.Add("Type", "Type");
            
            if (IsAccessAllowed)
                wim.ListDataColumns.Add("Allowed", "HasAccess", ListDataColumnType.Checkbox);
            else
                wim.ListDataColumns.Add("Denied", "HasAccess", ListDataColumnType.Checkbox);

            Sushi.Mediakiwi.Data.ApplicationUser temp = new Sushi.Mediakiwi.Data.ApplicationUser();
            temp.RoleID = e.SelectedGroupItemKey;
            Sushi.Mediakiwi.Data.IApplicationRole role = temp.Role();

            var selection =
                from item in Sushi.Mediakiwi.Data.Folder.SelectAll()
                join relation in role.Folders(temp) on item.GUID equals relation.GUID
                into combination
                from relation in combination.DefaultIfEmpty()
                //where item.Type != Sushi.Mediakiwi.Data.FolderType.Administration
                select new { SiteID = item.SiteID, ID = item.ID, Name = item.Name, HasAccess = relation == null ? false : true, Path = string.Concat(item.Site().Name, item.CompletePath).Replace("[internal]", "Admin"), Type = item.Type == Sushi.Mediakiwi.Data.FolderType.Page ? "Web" : item.Type == Sushi.Mediakiwi.Data.FolderType.List ? "Settings" : "Admin" };


            foreach (var item in selection)
            {
                
                if (role.IsAccessFolder)
                {
                    if (item.HasAccess)
                        wim.Grid.AddCheckboxValue("HasAccess", item.ID, true);
                }
                else if (!item.HasAccess)
                    wim.Grid.AddCheckboxValue("HasAccess", item.ID, true);
            }

            if (role.All_Sites)
            {
                var ordered =
                    from item in selection
                    orderby item.Type ascending, item.Path
                    select item;

                wim.ListData = ordered.ToArray();
            }
            else
            {
                var ordered =
                    from item in selection
                    join relation in role.Sites(wim.CurrentApplicationUser) on item.SiteID equals relation.ID
                    into combination
                    from relation in combination.DefaultIfEmpty()
                    select new { SiteID = item.SiteID, ID = item.ID, Name = item.Name, HasAccess = item.HasAccess, Type = item.Type, Path = item.Path, IsPresent = relation != null };

                ordered =
                    from item in ordered
                    where item.IsPresent || item.Type == "Admin"
                    orderby item.Type ascending, item.Path
                    select item;

                wim.ListData = ordered.ToArray();
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