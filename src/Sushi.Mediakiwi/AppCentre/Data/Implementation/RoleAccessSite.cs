using System.Linq;
using System.Data;
using Sushi.Mediakiwi.Framework;
using System.Threading.Tasks;
using Sushi.Mediakiwi.Data;

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
            
            this.ListSearch += RoleAccessSite_ListSearch;
            this.ListAction += RoleAccessSite_ListAction;
            this.ListLoad += RoleAccessSite_ListLoad;
        }


        /// <summary>
        /// Gets or sets a value indicating whether this instance is user section.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is user section; otherwise, <c>false</c>.
        /// </value>
        public bool IsUserSection { get; set; }

        Task RoleAccessSite_ListLoad(ComponentListEventArgs e)
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
            return Task.CompletedTask;
        }

        /// <summary>
        /// Gets or sets the user.
        /// </summary>
        /// <value>The user.</value>
        [Sushi.Mediakiwi.Framework.OnlyVisibleWhenTrue("IsUserSection"), Sushi.Mediakiwi.Framework.ContentListItem.TextLine("User", Expression = OutputExpression.Alternating)]
        public string User { get; set; }

        /// <summary>
        /// Gets or sets the role.
        /// </summary>
        /// <value>The role.</value>
        [Sushi.Mediakiwi.Framework.OnlyVisibleWhenTrue("IsUserSection", false), Sushi.Mediakiwi.Framework.ContentListItem.TextLine("Role", Expression = OutputExpression.Alternating)]
        public string Role { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is access allowed.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is access allowed; otherwise, <c>false</c>.
        /// </value>
        [Sushi.Mediakiwi.Framework.OnlyVisibleWhenTrue("IsUserSection", false)]
        [Sushi.Mediakiwi.Framework.ContentListItem.Choice_Checkbox("Grant access", true, "Checked selection means 'access granted', unchecked means 'access denied'", Expression = OutputExpression.Alternating)]
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
        Task RoleAccessSite_ListAction(ComponentActionEventArgs e)
        {
            if (IsUserSection)
            {
                var datalist = wim.Grid.GetRadioValue("HasAccess");

                var user = Sushi.Mediakiwi.Data.ApplicationUser.SelectOne(e.SelectedGroupItemKey);

                System.Collections.Generic.Dictionary<int, Access> dict = 
                    new System.Collections.Generic.Dictionary<int, Access>();

                foreach (var item in datalist)
                {
                    dict.Add(item.Key,
                        item.Value.Equals("Denied") ? Access.Denied :
                        item.Value.Equals("Granted") ? Access.Granted :
                        Access.Inherit);
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
            return Task.CompletedTask;
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
        Task RoleAccessSite_ListSearch(ComponentListSearchEventArgs e)
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
                        AccessType = (relation == null ? Access.Inherit : relation.AccessType)
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

                var allowed = role.Sites(temp);

                var selection =
                    from item in Sushi.Mediakiwi.Data.Site.SelectAll(true)
                    join relation in allowed on item.GUID equals relation.GUID
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
            return Task.CompletedTask;
        }

        /// <summary>
        /// Gets or sets the search list.
        /// </summary>
        /// <value>The search list.</value>
        [Sushi.Mediakiwi.Framework.ContentListItem.DataList()]
        public Sushi.Mediakiwi.Data.DataList SearchList { get; set; }
    }
}