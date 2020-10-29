using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Sushi.Mediakiwi.Framework;
using System.Globalization;

namespace Sushi.Mediakiwi.AppCentre.Data.Implementation
{
    /// <summary>
    /// 
    /// </summary>
    public class SubscriptionList : ComponentListTemplate
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SubscriptionList"/> class.
        /// </summary>
        public SubscriptionList()
        {
            //wim.ShowInFullWidthMode = true;
            this.ListLoad += new ComponentListEventHandler(SubscriptionList_ListLoad);
            this.ListSave += new ComponentListEventHandler(SubscriptionList_ListSave);
            this.ListDelete += new ComponentListEventHandler(SubscriptionList_ListDelete);
            this.ListSearch += new ComponentSearchEventHandler(SubscriptionList_ListSearch);
        }

        void SubscriptionList_ListSearch(object sender, ComponentListSearchEventArgs e)
        {
            int listID = Wim.Utility.ConvertToInt(Request.QueryString["base"]);
            wim.ListDataColumns.Add("", "ID", ListDataColumnType.UniqueIdentifier);
            wim.ListDataColumns.Add("User", "User");
            wim.ListDataColumns.Add("Title", "Title");
            wim.ListDataColumns.Add("Interval", "IntervalText");
            wim.ListDataColumns.Add("Next run", "Scheduled", 50);
            wim.ListDataColumns.Add("Created", "Created", 50);
            wim.ListDataColumns.Add("", "IsActiveIcon", 20, Align.Center);

            wim.SearchResultItemPassthroughParameter = string.Concat(wim.GetCurrentUrlParameters(null), "&item");
            wim.ListData = Sushi.Mediakiwi.Data.Subscription.SelectAll(listID, wim.CurrentApplicationUser.ID);
        }

        private Sushi.Mediakiwi.Data.SubList m_User;
        /// <summary>
        /// Gets or sets the user.
        /// </summary>
        /// <value>The user.</value>
        [Sushi.Mediakiwi.Framework.ContentListItem.SubListSelect("User", "9870FA8F-D9B5-47ED-89AE-CF01B6D00591", true, CanContainOneItem = true)]
        public Sushi.Mediakiwi.Data.SubList User
        {
            get { return m_User; }
            set { m_User = value; }
        }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>The title.</value>
        [Sushi.Mediakiwi.Framework.ContentListItem.TextLine("List")]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the implement.
        /// </summary>
        /// <value>The implement.</value>
        [Sushi.Mediakiwi.Framework.ContentListItem.DataExtend()]
        public Sushi.Mediakiwi.Data.ISubscription Implement { get; set; }

        void SubscriptionList_ListDelete(object sender, ComponentListEventArgs e)
        {
            Implement.Delete();
            Response.Redirect(string.Concat("wim.ashx?", wim.GetCurrentUrlParameters(0)));
        }

        void SubscriptionList_ListSave(object sender, ComponentListEventArgs e)
        {
            int listID = Wim.Utility.ConvertToInt(Request.QueryString["base"]);

            if (Implement.IsNewInstance)
                Implement.SetupXml = wim.CurrentVisitor.Data["wim_FilterInfo"].Value;
            Implement.UserID = this.User.GetID(0);
            Implement.ComponentListID = listID;
            Implement.SiteID = wim.CurrentSite.ID;
            Implement.Save();
        }

        void SubscriptionList_ListLoad(object sender, ComponentListEventArgs e)
        {
            int listID = Wim.Utility.ConvertToInt(Request.QueryString["base"]);
            Sushi.Mediakiwi.Data.IComponentList list = Sushi.Mediakiwi.Data.ComponentList.SelectOne(listID);

            this.Title = list.Name;
            Implement = Sushi.Mediakiwi.Data.Subscription.SelectOne(e.SelectedKey);
            if (Implement.IsNewInstance)
                Implement.Scheduled = new DateTime(DateTime.Now.Date.Ticks);


            if (e.SelectedKey == 0)
                this.User = new Sushi.Mediakiwi.Data.SubList(wim.CurrentApplicationUser.ID, wim.CurrentApplicationUser.Displayname);
            else
            {
                var appUser = Sushi.Mediakiwi.Data.ApplicationUser.SelectOne(Implement.UserID);
                this.User = new Sushi.Mediakiwi.Data.SubList(appUser.ID, appUser.Displayname);
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
