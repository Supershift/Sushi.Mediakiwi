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
    public class RoleAccessSite : ComponentListTemplate
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RoleAccessSite"/> class.
        /// </summary>
        public RoleAccessSite()
        {
            wim.HideOpenCloseToggle = true;
            wim.HideSaveButtons = true;
            
            this.ListSearch += new ComponentSearchEventHandler(RoleAccessSite_ListSearch);
            this.ListAction += new ComponentActionEventHandler(RoleAccessSite_ListAction);
            this.ListLoad += new ComponentListEventHandler(RoleAccessSite_ListLoad);
        }


        /// <summary>
        /// Gets or sets a value indicating whether this instance is user section.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is user section; otherwise, <c>false</c>.
        /// </value>
        public bool IsUserSection { get; set; }

        void RoleAccessSite_ListLoad(object sender, ComponentListEventArgs e)
        {
            Sushi.Mediakiwi.Data.IApplicationRole role = Sushi.Mediakiwi.Data.ApplicationRole.SelectOne(e.SelectedGroupItemKey);
            this.Role = role.Name;
            this.IsAccessAllowed = role.IsAccessSite;

            Sushi.Mediakiwi.Data.IComponentList list = Sushi.Mediakiwi.Data.ComponentList.SelectOne(e.SelectedGroupKey);
            IsUserSection = (list.Type == Sushi.Mediakiwi.Data.ComponentListType.Users);
            if (IsUserSection)
            {
                var user = Sushi.Mediakiwi.Data.ApplicationUser.SelectOne(e.SelectedGroupItemKey);
                this.User = user.Displayname;
            }
        }

        /// <summary>
        /// Gets or sets the user.
        /// </summary>
        /// <value>The user.</value>
        [Sushi.Mediakiwi.Framework.OnlyVisibleWhenTrue("IsUserSection"), Sushi.Mediakiwi.Framework.ContentListItem.TextLine("User")]
        public string User { get; set; }

        /// <summary>
        /// Gets or sets the role.
        /// </summary>
        /// <value>The role.</value>
        [Sushi.Mediakiwi.Framework.OnlyVisibleWhenTrue("IsUserSection", false), Sushi.Mediakiwi.Framework.ContentListItem.TextLine("Role")]
        public string Role { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is access allowed.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is access allowed; otherwise, <c>false</c>.
        /// </value>
        [Sushi.Mediakiwi.Framework.OnlyVisibleWhenTrue("IsUserSection", false), Sushi.Mediakiwi.Framework.ContentListItem.TextLine("Role")]
        [Sushi.Mediakiwi.Framework.ContentListItem.Choice_Checkbox("Access list", true, "Checked selection means 'access granted', unchecked means 'access denied'")]
        public bool IsAccessAllowed { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="RoleAccessSite"/> is save.
        /// </summary>
        /// <value><c>true</c> if save; otherwise, <c>false</c>.</value>
        [Sushi.Mediakiwi.Framework.ContentListItem.Button("Save access list", IconTarget = ButtonTarget.BottomRight)]
        public bool Save { get; set; }

        /// <summary>
        /// Handles the ListAction event of the RoleAccessSite control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Sushi.Mediakiwi.Framework.ComponentActionEventArgs"/> instance containing the event data.</param>
        void RoleAccessSite_ListAction(object sender, ComponentActionEventArgs e)
        {
            if (IsUserSection)
            {
                var datalist = wim.Grid.GetRadioValue("HasAccess");

                var user = Sushi.Mediakiwi.Data.ApplicationUser.SelectOne(e.SelectedGroupItemKey);

                System.Collections.Generic.Dictionary<int, Sushi.Mediakiwi.Data.RoleRight.Access> dict = 
                    new System.Collections.Generic.Dictionary<int, Sushi.Mediakiwi.Data.RoleRight.Access>();

                foreach (var item in datalist)
                {
                    dict.Add(item.Key,
                        item.Value.Equals("Denied") ? Sushi.Mediakiwi.Data.RoleRight.Access.Denied :
                        item.Value.Equals("Granted") ? Sushi.Mediakiwi.Data.RoleRight.Access.Granted :
                        Sushi.Mediakiwi.Data.RoleRight.Access.Inherit);
                }
                Sushi.Mediakiwi.Data.RoleRight.Update(dict, Sushi.Mediakiwi.Data.RoleRightType.SiteByUser, user.ID);

                //user.IsGrantedSite = this.IsAccessAllowed;
                //user.Save();
                //
            }
            else
            {
                int[] checklist = wim.Grid.GetCheckboxChecked("HasAccess");
                Sushi.Mediakiwi.Data.IApplicationRole role = Sushi.Mediakiwi.Data.ApplicationRole.SelectOne(e.SelectedGroupItemKey);
                role.IsAccessSite = this.IsAccessAllowed;
                role.Save();

                Sushi.Mediakiwi.Data.RoleRight.Update(checklist, Sushi.Mediakiwi.Data.RoleRightType.Site, role.ID);
            }
        }

        Sushi.Mediakiwi.Data.Site[] m_AccessList;
        bool GetRoleGrantedAccess(Sushi.Mediakiwi.Data.IApplicationUser user, int siteID)
        {
            if (m_AccessList == null)
                m_AccessList = user.Sites(Sushi.Mediakiwi.Data.AccessFilter.RoleBased);

            int len = (from item in m_AccessList where item.ID == siteID select item).ToArray().Length;
            return len == 0 ? false : true;
        }

        /// <summary>
        /// Handles the ListSearch event of the RoleAccessSite control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Sushi.Mediakiwi.Framework.ComponentListSearchEventArgs"/> instance containing the event data.</param>
        void RoleAccessSite_ListSearch(object sender, ComponentListSearchEventArgs e)
        {
            Sushi.Mediakiwi.Data.IComponentList list = Sushi.Mediakiwi.Data.ComponentList.SelectOne(e.SelectedGroupKey);
            IsUserSection = (list.Type == Sushi.Mediakiwi.Data.ComponentListType.Users);

            wim.CurrentList.Option_Search_MaxResultPerPage = 500;
            
            wim.SearchListCanClickThrough = false;
            wim.ListDataColumns.Add("ID", "ID", ListDataColumnType.UniqueIdentifier);
            wim.ListDataColumns.Add("Name", "Name", ListDataColumnType.HighlightPresent);

            if (IsUserSection)
            {
                wim.ListDataColumns.Add("Granted", "HasAccess", ListDataColumnType.RadioBox, 60);
                wim.ListDataColumns.Add("Denied", "HasAccess", ListDataColumnType.RadioBox, 60);
                wim.ListDataColumns.Add("Inherit", "HasAccess", ListDataColumnType.RadioBox, 60);

                wim.ListDataColumns.Add("Role", "RoleStatus", 30);
                var user = Sushi.Mediakiwi.Data.ApplicationUser.SelectOne(e.SelectedGroupItemKey);

                var selection =
                    from item in Sushi.Mediakiwi.Data.Site.SelectAll()
                    join relation in Sushi.Mediakiwi.Data.RoleRight.SelectAll(user.ID, Sushi.Mediakiwi.Data.RoleRightType.SiteByUser) on item.ID equals relation.ItemID
                    into combination
                    from relation in combination.DefaultIfEmpty()
                    select new {
                        item.ID,
                        item.Name,
                        HasRoleReference = (relation == null ? false : true),
                        RoleStatus = GetRoleGrantedAccess(user, item.ID),
                        AccessType = (relation == null ? Sushi.Mediakiwi.Data.RoleRight.Access.Inherit : relation.AccessType)
                    };

                foreach (var item in selection)
                {
                    wim.Grid.AddRadioboxValue("HasAccess", item.ID, item.AccessType.ToString());
                }
                wim.ListData = selection.ToArray();
            }
            else
            {
                if (IsAccessAllowed)
                {
                    wim.ListDataColumns.Add("Granted", "HasAccess", ListDataColumnType.Checkbox, 60);
                }
                else
                    wim.ListDataColumns.Add("Denied", "HasAccess", ListDataColumnType.Checkbox, 60);


                Sushi.Mediakiwi.Data.ApplicationUser temp = new Sushi.Mediakiwi.Data.ApplicationUser();
                temp.RoleID = e.SelectedGroupItemKey;
                Sushi.Mediakiwi.Data.IApplicationRole role = temp.Role();

                var selection =
                    from item in Sushi.Mediakiwi.Data.Site.SelectAll()
                    join relation in role.Sites(temp) on item.GUID equals relation.GUID
                    into combination
                    from relation in combination.DefaultIfEmpty()
                    select new { ID = item.ID, Name = item.Name, HasAccess = relation == null ? false : true };

                foreach (var item in selection)
                {
                    if (role.IsAccessSite)
                    {
                        if (item.HasAccess)
                            wim.Grid.AddCheckboxValue("HasAccess", item.ID, true);
                    }
                    else if (!item.HasAccess)
                        wim.Grid.AddCheckboxValue("HasAccess", item.ID, true);
                }
                wim.ListData = selection.ToArray();
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