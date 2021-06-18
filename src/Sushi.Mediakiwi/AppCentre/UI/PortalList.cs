//using System;
//using System.Collections.Generic;
//using System.Text;
//using Sushi.Mediakiwi.Framework;
//using Sushi.Mediakiwi.Data;

//namespace Sushi.Mediakiwi.AppCentre.Data.Implementation
//{
//    /// <summary>
//    /// 
//    /// </summary>
//    public class PortalList : ComponentListTemplate
//    {
//        /// <summary>
//        /// Initializes a new instance of the <see cref="PortalList"/> class.
//        /// </summary>
//        public PortalList()
//        {
//            ListLoad += new ComponentListEventHandler(PortalList_ListLoad);
//            ListSave += new ComponentListEventHandler(PortalList_ListSave);
//            ListSearch += new ComponentSearchEventHandler(PortalList_ListSearch);
//            ListDelete += new ComponentListEventHandler(PortalList_ListDelete);
//            ListPreRender += new ComponentListEventHandler(PortalList_ListPreRender);

//            this.Section1 = "Local username and portal specific password";
//            this.Section2 = "Remote username and portal specific password";
//        }

//        void PortalList_ListPreRender(object sender, ComponentListEventArgs e)
//        {
//            if (!IsPostBack || string.IsNullOrEmpty(Implement.Domain)) return;
//            var user = Sushi.Mediakiwi.Data.ApplicationUser.SelectOne(e.SelectedGroupItemKey);
//            wimServerCommunication.WebInformationManagerServerService connect = Portal.Connect(Implement.Domain);    
//            try
//            {
//                string ping = connect.Ping();
//                if (!string.IsNullOrEmpty(ping))
//                {
//                    string host = Request.Url.DnsSafeHost;
//                    if (Request.ApplicationPath != "/")
//                        host = string.Concat(host, Request.ApplicationPath);

//                    //if (host == Implement.Domain)
//                    //{
//                    //    wim.Notification.AddError("Domain", "Error");
//                    //    return;
//                    //}

//                    wimServerCommunication.Authenticate auth = new Wim.wimServerCommunication.Authenticate();
//                    auth.Username = Username;
//                    auth.Password = Wim.Utility.HashStringByMD5(this.Password);
//                    auth.UsernameLocal = user.Name;
//                    auth.PasswordLocal = Wim.Utility.HashStringByMD5(this.PasswordLocal);
//                    auth.CRC = ThinCommunicationService.PingResponse();
//                    auth.BiDirection = true;
//                    Implement.Created = DateTime.Now.Date;
//                    auth.Authenticode = Sushi.Mediakiwi.Framework.ThinCommunicationService.GetAuthenticode(Implement.GUID, Implement.Created.Ticks.ToString());
//                    auth.Domain = Implement.Domain;
//                    auth.DomainLocal = host;

//                    wimServerCommunication.AuthenticateResponse authResponse = connect.AuthenticateMe(auth, auth.DomainLocal);
//                    if (authResponse == null)
//                    {
//                        wim.Notification.AddError("There was no response for this domain");
//                        return;
//                    }

//                    Implement.Authenticode = authResponse.Authenticode;
//                    //Implement.Authentication = Wim.Utility.HashStringBySHA1(string.Concat(DateTime.Now.Date.Ticks, 
//                    //    Sushi.Mediakiwi.Framework.ThinCommunicationService.GetAuthenticode(ping, Username, authResponse.Hash, DateTime.Now.Date.Ticks.ToString())));

//                    Implement.Authentication = authResponse.Hash;
//                    Implement.Name = authResponse.Portal;
//                    Implement.IsActive = true;
//                    Implement.UserID = e.SelectedGroupItemKey;
//                }
//            }
//            catch (Exception ex)
//            {
//                wim.Notification.AddError("Domain", "");
//                wim.Notification.AddError(string.Concat("An error ocurred: ", ex.Message));
//            }
//        }

//        void PortalList_ListDelete(object sender, ComponentListEventArgs e)
//        {
//            Implement.Delete();
//        }

//        void PortalList_ListSearch(object sender, ComponentListSearchEventArgs e)
//        {
//            wim.ListDataColumns.Add("ID", "ID", ListDataColumnType.UniqueIdentifier);
//            wim.ListDataColumns.Add("Name", "Name");
//            wim.ListDataColumns.Add("Domain", "Domain");
//            wim.ListDataColumns.Add("Created", "Created", 50, Align.Center);
//            wim.ListDataColumns.Add("Active", "IsActive", 40, Align.Center);

//            wim.ListData = Sushi.Mediakiwi.Data.Portal.SelectAll(e.SelectedGroupItemKey);
//        }

//        void PortalList_ListSave(object sender, ComponentListEventArgs e)
//        {
//            Implement.Save();
//        }

//        void PortalList_ListLoad(object sender, ComponentListEventArgs e)
//        {
//            Implement = Portal.SelectOne(e.SelectedKey);
//            UsernameLocal = Sushi.Mediakiwi.Data.ApplicationUser.SelectOne(e.SelectedGroupItemKey).Name;
//            Username = Sushi.Mediakiwi.Data.ApplicationUser.SelectOne(e.SelectedGroupItemKey).Name;
//        }

//        public bool CanEdit { get; set; }

//        /// <summary>
//        /// Gets or sets the implement.
//        /// </summary>
//        /// <value>The implement.</value>
//        [Sushi.Mediakiwi.Framework.ContentListItem.DataExtend()]
//        public Sushi.Mediakiwi.Data.IPortal Implement { get; set; }

//        [Sushi.Mediakiwi.Framework.ContentListItem.Section()]
//        public string Section1 { get; set; }

//        /// <summary>
//        /// Gets or sets the username local.
//        /// </summary>
//        /// <value>The username local.</value>
//        [Sushi.Mediakiwi.Framework.OnlyEditableWhenTrue("CanEdit")]
//        [Sushi.Mediakiwi.Framework.ContentListItem.TextField("Username (local)", 50, true)]
//        public string UsernameLocal { get; set; }

//        /// <summary>
//        /// Gets or sets the password local.
//        /// </summary>
//        /// <value>The password local.</value>
//        [Sushi.Mediakiwi.Framework.ContentListItem.TextField("Password (local)", 50, true, IsPasswordField = true)]
//        public string PasswordLocal { get; set; }

//        [Sushi.Mediakiwi.Framework.ContentListItem.Section()]
//        public string Section2 { get; set; }

//        /// <summary>
//        /// Gets or sets the username.
//        /// </summary>
//        /// <value>The username.</value>
//        [Sushi.Mediakiwi.Framework.ContentListItem.TextField("Username", 50, true)]
//        public string Username { get; set; }

//        /// <summary>
//        /// Gets or sets the password.
//        /// </summary>
//        /// <value>The password.</value>
//        [Sushi.Mediakiwi.Framework.ContentListItem.TextField("Password", 50, true, IsPasswordField = true)]
//        public string Password { get; set; }


//        /// <summary>
//        /// Gets or sets the search grid.
//        /// </summary>
//        /// <value>The search grid.</value>
//        [Sushi.Mediakiwi.Framework.ContentListItem.DataList()]
//        public Sushi.Mediakiwi.Data.DataList SearchGrid { get; set; }
//    }
//}
