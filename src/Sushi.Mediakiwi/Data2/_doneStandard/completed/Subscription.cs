using System;
using System.Web;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Sushi.Mediakiwi.Data;
using Sushi.Mediakiwi.Data.DalReflection;
using System.Web.UI.WebControls;
using Sushi.Mediakiwi.Data.Parsers;

namespace Sushi.Mediakiwi.Data
{
    /// <summary>
    /// 
    /// </summary>
    [DatabaseTable("wim_Subscriptions", Join = "join wim_Users on User_Key = Subscription_User_Key")]
    public class Subscription : ISubscription
    {

        static ISubscriptionParser _Parser;
        static ISubscriptionParser Parser
        {
            get
            {
                if (_Parser == null)
                    _Parser = Environment.GetInstance<ISubscriptionParser>();
                return _Parser;
            }
        }

        #region REPLICATED TO Sushi.Mediakiwi.Data.Standard

        public bool IsNewInstance { get { return this.ID == 0; } }

        public Subscription()
        {
            this.IsActive = true;
        }

        /// <summary>
        /// Clears this instance.
        /// </summary>
        public static void Clear()
        {
            Parser.Clear();
        }

        public void Delete()
        {
            Parser.Delete(this);
        }

        /// <summary>
        /// Selects the one.
        /// </summary>
        /// <param name="ID">The ID.</param>
        /// <returns></returns>
        public static ISubscription SelectOne(int ID)
        {
            return Parser.SelectOne(ID);
        }

        /// <summary>
        /// Selects all.
        /// </summary>
        /// <returns></returns>
        public static ISubscription[] SelectAllActive()
        {
            return Parser.SelectAllActive();
        }

        /// <summary>
        /// Selects all.
        /// </summary>
        /// <param name="listID">The list ID.</param>
        /// <param name="userID">The user ID.</param>
        /// <returns></returns>
        public static ISubscription[] SelectAll(int listID, int userID)
        {
            return Parser.SelectAll(listID, userID);
        }

        public static ISubscription[] SelectAll()
        {
            return Parser.SelectAll();
        }

        public virtual void Save()
        {
            Parser.Save(this);
        }

        [DatabaseColumn("Subscription_AlternativeTitle", SqlDbType.NVarChar, Length = 50, IsNullable = true)]
        [Sushi.Mediakiwi.Framework.ContentListItem.TextField("Alternative title", 50, false)]
        public string Title2 { get; set; }

        public string Title {
            get
            {
                if (string.IsNullOrEmpty(this.Title2))
                    return Sushi.Mediakiwi.Data.ComponentList.SelectOne(this.ComponentListID).Name;
                return this.Title2;
            }
        }

        [DatabaseColumn("Subscription_Key", SqlDbType.Int, IsPrimaryKey = true)]
        public int ID { get; set; }

        [DatabaseColumn("Subscription_User_Key", SqlDbType.Int)]
        public int UserID { get; set; }

        [DatabaseColumn("User_Displayname", SqlDbType.NVarChar, IsNullable = true, IsOnlyRead = true)]
        public string User { get; set; }

        [DatabaseColumn("Subscription_ComponentList_Key", SqlDbType.Int)]
        public int ComponentListID { get; set; }

        [DatabaseColumn("Subscription_Site_Key", SqlDbType.Int, IsNullable = true)]
        public int SiteID { get; set; }

        [DatabaseColumn("Subscription_ComponentList_Setup", SqlDbType.Xml, IsNullable = true)]
        public string SetupXml { get; set; }

        [Sushi.Mediakiwi.Framework.ContentListItem.Choice_Dropdown("Interval", "IntervalCollection", true, false)]
        [DatabaseColumn("Subscription_ScheduleInterval", SqlDbType.Int, IsNullable = true)]
        public int IntervalType { get; set; }

        [Sushi.Mediakiwi.Framework.ContentListItem.DateTime("Next run", false)]
        [DatabaseColumn("Subscription_Scheduled", SqlDbType.DateTime, IsNullable = true)]
        public DateTime Scheduled { get; set; }


        DateTime m_Created;
        /// <summary>
        /// The creation date/time (UTC) of this site
        /// </summary>
        [DatabaseColumn("Subscription_Created", SqlDbType.DateTime)]
        public DateTime Created
        {
            get
            {
                if (this.m_Created == DateTime.MinValue) this.m_Created = Sushi.Mediakiwi.Data.Common.DatabaseDateTime;
                return m_Created;
            }
            set { m_Created = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is active.
        /// </summary>
        /// <value><c>true</c> if this instance is active; otherwise, <c>false</c>.</value>
        [Sushi.Mediakiwi.Framework.ContentListItem.Choice_Checkbox("Is active")]
        [DatabaseColumn("Subscription_IsActive", SqlDbType.Bit, IsNullable = true)]
        public bool IsActive { get; set; }

        #endregion REPLICATED TO Sushi.Mediakiwi.Data.Standard

        #region MOVED to EXTENSION / LOGIC

        ///// <summary>
        ///// Gets the interval text.
        ///// </summary>
        ///// <value>The interval text.</value>
        //public string IntervalText
        //{
        //    get
        //    {
        //        foreach (ListItem li in IntervalCollection)
        //            if (li.Value == this.IntervalType.ToString()) return li.Text;
        //        return null;
        //    }
        //}

        ///// <summary>
        ///// Gets the is active icon.
        ///// </summary>
        ///// <value>The is active icon.</value>
        //public string IsActiveIcon
        //{
        //    get { return this.IsActive ? Wim.Utility.GetIconImageString(Utility.IconImage.Yes) : Wim.Utility.GetIconImageString(Utility.IconImage.No); }
        //}

        //ListItemCollection m_IntervalCollection;
        ///// <summary>
        ///// Gets the interval collection.
        ///// </summary>
        ///// <value>The interval collection.</value>
        //public ListItemCollection IntervalCollection
        //{
        //    get
        //    {
        //        if (m_IntervalCollection == null)
        //        {
        //            m_IntervalCollection = new ListItemCollection();

        //            m_IntervalCollection.Add(new ListItem(""));
        //            m_IntervalCollection.Add(new ListItem("Every hour", "60"));
        //            m_IntervalCollection.Add(new ListItem("Every 2 hours", "120"));
        //            m_IntervalCollection.Add(new ListItem("Every 4 hours", "240"));
        //            m_IntervalCollection.Add(new ListItem("Every 6 hours", "360"));
        //            m_IntervalCollection.Add(new ListItem("Every 12 hours", "720"));
        //            m_IntervalCollection.Add(new ListItem("Every day", "1440"));
        //            m_IntervalCollection.Add(new ListItem("Every 2 days", "2880"));
        //            m_IntervalCollection.Add(new ListItem("Every week", "3360"));
        //            //m_IntervalCollection.Add(new ListItem("Every 1th of the month", "-1"));
        //            //m_IntervalCollection.Add(new ListItem("Every 15th of the month", "-2"));
        //        }
        //        return m_IntervalCollection;
        //    }
        //}

        #endregion MOVED to EXTENSION / LOGIC
    }
}
