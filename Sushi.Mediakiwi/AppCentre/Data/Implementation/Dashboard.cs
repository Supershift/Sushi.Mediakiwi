using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Threading;
using Sushi.Mediakiwi.AppCentre.Data.Supporting;
using Sushi.Mediakiwi.Framework;
using Sushi.Mediakiwi.Data;
using System.Linq;

namespace Sushi.Mediakiwi.AppCentre.Data.Implementation
{
    /// <summary>
    /// Represents a Dashboard entity.
    /// </summary>
    public class Dashboard : BaseImplementation
    {
        public string PreviewUrl { get; set; }
        [Sushi.Mediakiwi.Framework.ContentListItem.Button("", false, CustomUrlProperty = "PreviewUrl", Target = "dashboard", ButtonClassName = "flaticon icon-eye", InteractiveHelp = "View this dashboard")]
        public bool Preview { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Dashboard"/> class.
        /// </summary>
        public Dashboard()
        {
            wim.OpenInEditMode = true;
            this.ListSearch += new Sushi.Mediakiwi.Framework.ComponentSearchEventHandler(Dashboard_ListSearch);
            this.ListLoad += new Sushi.Mediakiwi.Framework.ComponentListEventHandler(Dashboard_ListLoad);
            this.ListSave += new Sushi.Mediakiwi.Framework.ComponentListEventHandler(Dashboard_ListSave);
            this.ListDelete += new Sushi.Mediakiwi.Framework.ComponentListEventHandler(Dashboard_ListDelete);
        }

        /// <summary>
        /// Handles the ListDelete event of the Dashboard control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Sushi.Mediakiwi.Framework.ComponentListEventArgs"/> instance containing the event data.</param>
        void Dashboard_ListDelete(object sender, Sushi.Mediakiwi.Framework.ComponentListEventArgs e)
        {
            Implement.Delete();
        }

        /// <summary>
        /// Handles the ListSave event of the Dashboard control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Sushi.Mediakiwi.Framework.ComponentListEventArgs"/> instance containing the event data.</param>
        void Dashboard_ListSave(object sender, Sushi.Mediakiwi.Framework.ComponentListEventArgs e)
        {
            Implement.Save();
            Sushi.Mediakiwi.Data.Dashboard.Update(Implement.ID, 0, this.DashboardTarget);
            SaveItem(1, this.ShortCuts);
        }

        void SaveItem(int position, SubList container)
        {
            int index = 0;
            //  Remove obsolete
            var existingMenuItems = (from item in this.Items where item.Position == position select item).ToArray();
            foreach (var item in existingMenuItems)
            {
                var seek = (from mi in container.Items where mi.TextID == item.Tag select mi).Count();
                if (seek == 0)
                    item.Delete();
            }
            //  Add Or Update
            index = 0;
            foreach (var item in container.Items)
            {
                index++;
                var split = item.TextID.Split('_');
                var menuItem = (from mi in existingMenuItems where mi.Tag == item.TextID select mi).FirstOrDefault();
                if (menuItem == null) menuItem = new Sushi.Mediakiwi.Data.MenuItem();
                menuItem.DashboardID = Implement.ID;
                menuItem.ItemID = Convert.ToInt32(split[1]);
                menuItem.TypeID = Convert.ToInt32(split[0].Replace("T", string.Empty));
                menuItem.Position = position;
                menuItem.Sort = index;
                menuItem.Save();
            }
        }
        Sushi.Mediakiwi.Data.IMenuItem[] Items { get; set; }
        void LoadItem(int position, SubList container)
        {
            var items = (from item in this.Items where item.Position == position select item).ToArray();

            List<string> tags = new List<string>();
            foreach (var item in items)
            {
                tags.Add(item.Tag);
            }
            var selection = SearchView.SelectAll(tags.ToArray());
            foreach (var item in items)
            {
                var find = (from x in selection where x.ID == item.Tag select x).FirstOrDefault();
                if (find != null)
                    container.Add(new SubList.SubListitem(find.ID, find.Title));
            }
        }

        /// <summary>
        /// Handles the ListLoad event of the Dashboard control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Sushi.Mediakiwi.Framework.ComponentListEventArgs"/> instance containing the event data.</param>
        void Dashboard_ListLoad(object sender, Sushi.Mediakiwi.Framework.ComponentListEventArgs e)
        {
            Implement = Sushi.Mediakiwi.Data.Dashboard.SelectOne(e.SelectedKey);
            Items = Sushi.Mediakiwi.Data.MenuItem.SelectAll_Dashboard(Implement.ID);
            this.ShortCuts = new SubList();
            LoadItem(1, this.ShortCuts);

            if (!this.Implement.IsNewInstance)
            {
                this.PreviewUrl = wim.GetCurrentQueryUrl(true
                    , new KeyValue() { Key = "dashboard", Value = this.Implement.ID }
                    , new KeyValue() { Key = "item", RemoveKey = true }
                    , new KeyValue() { Key = "list", RemoveKey = true }
                    );
            }
            else
                wim.SetPropertyVisibility("Preview", false);

            this.DashboardTarget = new Sushi.Mediakiwi.Data.SubList();
            foreach (Sushi.Mediakiwi.Data.ComponentList list in m_Implement.DashboardTarget)
                this.DashboardTarget.Add(new Sushi.Mediakiwi.Data.SubList.SubListitem(list.ID, list.Name));

            //if (wim.CurrentApplicationUser.ShowNewDesign)
            //{
            //    wim.SetPropertyVisibility("DashboardTarget1", false);
            //    wim.SetPropertyVisibility("DashboardTarget2", false);
            //    wim.SetPropertyVisibility("DashboardTarget3", false);
            //    wim.SetPropertyVisibility("DashboardTarget4", false);
            //    wim.SetPropertyVisibility("Body", false);
               wim.SetPropertyVisibility("Type", false);
            //}

        }

        /// <summary>
        /// Dashboard_s the list search.
        /// </summary>
        /// <param name="sender">The sender.</param>
        void Dashboard_ListSearch(object sender, ComponentListSearchEventArgs e)
        {
            wim.CanAddNewItem = true;

            wim.ListDataColumns.Add("ID", "ID", ListDataColumnType.UniqueIdentifier);
            wim.ListDataColumns.Add("Name", "Name", ListDataColumnType.HighlightPresent);
            wim.ListDataColumns.Add("Created", "Created");

            wim.ListData = Sushi.Mediakiwi.Data.Dashboard.SelectAll();
        }

        Sushi.Mediakiwi.Data.Dashboard m_Implement;
        /// <summary>
        /// Gets or sets the implement.
        /// </summary>
        /// <value>The implement.</value>
        [Sushi.Mediakiwi.Framework.ContentListItem.DataExtend()]
        public Sushi.Mediakiwi.Data.Dashboard Implement
        {
            get { return m_Implement; }
            set { m_Implement = value; }
        }


        private Sushi.Mediakiwi.Data.SubList _DashboardTarget;
        /// <summary>
        /// Gets or sets the dashboard target1.
        /// </summary>
        /// <value>The dashboard target1.</value>
        [Sushi.Mediakiwi.Framework.ContentListItem.SubListSelect("Lists", "814fbdcd-7823-4ef1-8dd5-a347d8fdc090", false, "")]
        public Sushi.Mediakiwi.Data.SubList DashboardTarget
        {
            get { return _DashboardTarget; }
            set { _DashboardTarget = value; }
        }

        [Sushi.Mediakiwi.Framework.ContentListItem.SubListSelect("Shortcuts", "1a1fe050-219c-4f63-a697-7e2e8e790521", true, "")]
        public Sushi.Mediakiwi.Data.SubList ShortCuts { get; set; }
    }
}
