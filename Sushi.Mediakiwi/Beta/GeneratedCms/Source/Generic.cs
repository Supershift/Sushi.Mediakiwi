using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.Configuration;
using Sushi.Mediakiwi.Data;
using Wim.Utilities;

namespace Sushi.Mediakiwi.Beta.GeneratedCms.Source
{
    class Generic
    {
        /// <summary>
        /// Headers the specified container.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <returns></returns>
        internal static string Header(Console container)
        {
            return "head";
//            return string.Format(@"
//	<meta http-equiv=""Content-Type"" content=""text/html; charset=UTF-8"" />
//	<meta http-equiv=""Content-Style-Type"" content=""text/css"" />
//	<meta http-equiv=""Content-Language"" content=""en-GB"" />
//	<!-- common styles -->
//	<link rel=""stylesheet"" href=""{1}/styles/markup.css"" type=""text/css"" media=""all"" />
//	<link rel=""stylesheet"" href=""{1}/wim/css.ashx?layout"" type=""text/css"" media=""all"" id=""screenStyles"" />
//	<link rel=""stylesheet"" href=""{1}/styles/print.css"" type=""text/css"" media=""print"" id=""printStyles"" />{2}
//	<link rel=""stylesheet"" href=""{1}/styles/override.css"" type=""text/css"" media=""all"" id=""screenStyles"" />
//    <!--[if IE]><link rel=""stylesheet"" href=""{1}/styles/msiehax.css"" type=""text/css"" media=""all"" id=""msieStyles"" /><![endif]-->
//	<!-- /common styles -->
//	<!-- context links -->
//	<link rel=""icon"" href=""{1}/favicon.png"" type=""image/png"" />
//	<link rel=""shortcut icon"" href=""{1}/favicon.ico"" type=""image/x-icon"" />
//	<!-- /context links -->
//	<!-- javascript libraries -->
//	<script type=""text/javascript"" src=""{1}/scripts/library.js""></script>
//	<!-- /javascript libraries -->
//"
//                , "WIM - Web Information Management"
//                , container.WimRepository
//                , string.IsNullOrEmpty(container.CurrentApplicationUser.Skin) ? null : string.Format(@"<link rel=""stylesheet"" href=""{0}/styles/{1}.css"" type=""text/css"" media=""all"" id=""reskinStyles"" />", container.WimRepository, container.CurrentApplicationUser.Skin)
//                );
        }

        /// <summary>
        /// Tops the container.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <returns></returns>
        internal static string TopContainer(Console container)
        {
            //string top_page = container.CurrentListInstance.wim.CurrentFolder.Type == Sushi.Mediakiwi.Data.FolderType.Page ? "main_web_active" : "main_web_link";
            //string top_logic = container.CurrentListInstance.wim.CurrentFolder.Type == Sushi.Mediakiwi.Data.FolderType.List ? "main_logic_active" : "main_logic_link";
            //string top_gallery = container.CurrentListInstance.wim.CurrentFolder.Type == Sushi.Mediakiwi.Data.FolderType.Gallery ? "main_libraries_active" : "main_libraries_link";
            //string top_administration = container.CurrentListInstance.wim.CurrentFolder.Type == Sushi.Mediakiwi.Data.FolderType.Administration ? "main_administration_active" : "main_administration_link";

            string top_page = container.CurrentListInstance == null ? null : container.CurrentListInstance.wim.CurrentFolder.Type == Sushi.Mediakiwi.Data.FolderType.Page ? " class=\"active\"" : null;
            string top_logic = container.CurrentListInstance == null ? null : container.CurrentListInstance.wim.CurrentFolder.Type == Sushi.Mediakiwi.Data.FolderType.List ? " class=\"active\"" : null;
            string top_gallery = container.CurrentListInstance == null ? null : container.CurrentListInstance.wim.CurrentFolder.Type == Sushi.Mediakiwi.Data.FolderType.Gallery ? " class=\"active\"" : null;
            string top_administration = container.CurrentListInstance == null ? null : container.CurrentListInstance.wim.CurrentFolder.Type == Sushi.Mediakiwi.Data.FolderType.Administration ? " class=\"active\"" : null;


            string channels = "";

            if (!container.IsCodeUpdate)
            {
                //if (container.CurrentListInstance != null && container.CurrentListInstance.wim.CurrentFolder.SiteID > 0)
                //{
                //    if (container.CurrentListInstance.wim.CurrentFolder.Type == Sushi.Mediakiwi.Data.FolderType.Page || container.CurrentListInstance.wim.CurrentFolder.Type == Sushi.Mediakiwi.Data.FolderType.List)
                //        container.CurrentApplicationUser.Channel = container.CurrentListInstance.wim.CurrentFolder.SiteID;
                //}

                Data.Site[] allAccessible = Data.Site.SelectAllAccessible(container.CurrentApplicationUser, AccessFilter.RoleAndUser);

                //bool foundSite = false;
                //foreach (Data.Site site in allAccessible)
                //{
                //    if (site.ID == container.CurrentApplicationUser.Channel)
                //        foundSite = true;

                //    if (!site.HasLists && !site.HasPages && !site.IsActive)
                //        continue;

                //    channels += string.Format("<option value=\"{0}\"{2}>{1}</option>", site.ID, site.Name, site.ID == container.CurrentApplicationUser.Channel ? " selected=\"selected\"" : string.Empty);
                //}

                //if (!foundSite)
                //{
                //    if (allAccessible.Length > 0)
                //    {
                //        container.CurrentApplicationUser.Channel = allAccessible[0].ID;
                //        container.CurrentApplicationUser.Save(true);
                //        container.Response.Redirect(container.WimPagePath, true);
                //    }
                //}
            }

            Sushi.Mediakiwi.Data.IApplicationRole role = Sushi.Mediakiwi.Data.ApplicationRole.SelectOne(container.CurrentApplicationUser.RoleID);
            bool hasWeb = role.CanSeePage;
            bool hasLogic = role.CanSeeList;
            bool hasLibraries = role.CanSeeGallery;
            bool hasAdministration = role.CanSeeAdmin;

            if (container.CurrentListInstance != null)
            {
                if (hasWeb && !container.CurrentListInstance.wim.CurrentSite.HasPages) hasWeb = false;
                if (hasLogic && !container.CurrentListInstance.wim.CurrentSite.HasLists) hasLogic = false;
            }

            string skin = string.IsNullOrEmpty(container.CurrentApplicationUser.GetSkin()) ? null 
                : string.Concat(container.CurrentApplicationUser.GetSkin(), "/");

            //<input type=""image"" id=""frmSubmitPortals"" alt=""Submit"" src=""{2}/images/{8}icon_submit_link.png""/>

            string navigation = null;
            navigation = string.Concat(""
                , hasWeb ? string.Format(@"<li class=""first""><a href=""{0}?top=1""{2}>{4}</a></li>", container.WimPagePath, container.WimRepository, top_page, skin, Resource.ResourceManager.GetString("section_web", new CultureInfo(container.CurrentApplicationUser.LanguageCulture))) : string.Empty
                , hasLogic ? string.Format(@"<li><a href=""{0}?top=2""{2}>{4}</a></li>", container.WimPagePath, container.WimRepository, top_logic, skin, Resource.ResourceManager.GetString("section_logic", new CultureInfo(container.CurrentApplicationUser.LanguageCulture))) : string.Empty
                , hasLibraries ? string.Format(@"<li><a href=""{0}?top=3""{2}>{4}</a></li>", container.WimPagePath, container.WimRepository, top_gallery, skin, Resource.ResourceManager.GetString("section_galleries", new CultureInfo(container.CurrentApplicationUser.LanguageCulture))) : string.Empty
                , hasAdministration ? string.Format(@"<li><a href=""{0}?top=4""{2}>{4}</a></li>", container.WimPagePath, container.WimRepository, top_administration, skin, Resource.ResourceManager.GetString("section_admin", new CultureInfo(container.CurrentApplicationUser.LanguageCulture))) : string.Empty
                );

            //  If no portal is found show the non-portal view
            var portals = Sushi.Mediakiwi.Data.Portal.SelectAll(container.CurrentApplicationUser.RoleID);

            //"Wim.Reset.Me"
            string usernameLink = container.CurrentApplicationUser.Name;
            if (!container.CurrentVisitor.Data["Wim.Reset.Me"].IsNull)
            {
                if (container.Request.QueryString.ToString().EndsWith("reset"))
                {
                    int userID = container.CurrentApplicationUser.ID;
                    var user = ApplicationUserLogic.Apply(container.CurrentVisitor.Data["Wim.Reset.Me"].ParseGuid().Value, false);

                    container.CurrentVisitor.Data.Apply("Wim.Reset.Me", null);
                    container.CurrentVisitor.Save();

                    PortalAuthentication auth = new PortalAuthentication();
                    container.Response.Cookies.Add(auth.SetAuthentication(user, container.Request.IsSecureConnection));

                    IComponentList userList = Sushi.Mediakiwi.Data.ComponentList.SelectOne(ComponentListType.Users);
                    container.Response.Redirect(container.UrlBuild.GetListRequest(userList, userID));
                }
                usernameLink = string.Format("{0} (<a href=\"?reset\">reset</a>)", usernameLink);
            }

            if (portals.Length == 0)
            {
                return string.Format(@"    
<div id=""headerRow"">
    <div id=""headerContent"">
	    <div id=""logo""><a href=""{4}""><img alt=""WIM - Web Information Management"" src=""{2}/logo.png""/></a></div>
	    <dl id=""loggedIn"">
		    <dd class=""name"">{0}<input type=""hidden"" name=""logout"" /> <span>|</span> <a href=""#"" class=""logout postBack"">{8}</a> <span>|</span> <label for=""frmChannels"">{6}: <input type=""hidden"" name=""channel"" /><select id=""frmChannels"" class=""postBack"" name=""frmChannels"">{1}</select></label></dd>
            <dt>{7}:</dt>
	    </dl>
	    <ul id=""mainNavigation"">{3}
	    </ul>
    </div>
</div>
"
                    , usernameLink // 0
                    , channels // 1
                    , container.WimRepository // 2
                    , navigation // 3
                    , container.WimPagePath // 4
                    , skin // 5
                    , Resource.ResourceManager.GetString("channel", new CultureInfo(container.CurrentApplicationUser.LanguageCulture)) //6
                    , Resource.ResourceManager.GetString("logged_in_as", new CultureInfo(container.CurrentApplicationUser.LanguageCulture)) //7
                    , Resource.ResourceManager.GetString("logout", new CultureInfo(container.CurrentApplicationUser.LanguageCulture)) //8
                    );
            }


            string portalList = string.Format("<option value=\"{0}\">{1}</option>", "0", Sushi.Mediakiwi.Data.Environment.Current.DisplayName);
            string currentGuid = null;
            foreach (Portal portal in portals)
            {
                if (!portal.Name.Equals(Sushi.Mediakiwi.Data.Environment.Current.DisplayName, StringComparison.InvariantCulture))
                {
                    portalList += string.Format("<option value=\"{0}\">{1}</option>", portal.ID.ToString(), portal.Name);
                }
                else
                    currentGuid = portal.GUID.ToString();
            }

            if (!string.IsNullOrEmpty(container.Request.Form["portal"]))
            {
                var portal = Portal.SelectOne(Convert.ToInt32(container.Request.Form["portal"]));

                container.Response.Redirect(string.Format("{0}/repository/tcl.aspx?negotiate={1},{2},{3}"
                    , portal.Domain
                    , currentGuid
                    , portal.GUID
                    , container.CurrentApplicationUser.GUID));
            }

            // (<a href=""#"">wijzig</a>) 
            return string.Format(@"    
<div id=""headerRow"">
    <div id=""headerContent"">
	    <div id=""logo""><a href=""{4}""><img alt=""WIM - Web Information Management"" src=""{2}/logo.png""/></a></div>
	    <dl id=""loggedIn"">
		    <dd class=""name""> <span>|</span> <label for=""frmChannels"">{6}: <input type=""hidden"" name=""channel"" /><select id=""frmChannels"" class=""postBack"" name=""frmChannels"">{1}</select></label></dd>
            <dt><label for=""frmPortals"">Portaal: <input type=""hidden"" name=""portal"" /><select id=""frmPortals"" class=""postBack"" name=""frmPortals"">{9}</select></label></dt>
	    </dl>
	    <dl id=""loggedInAs"">
            <dd>{7}: {0}<input type=""hidden"" name=""logout"" /> <span>|</span> <a href=""#"" class=""logout postBack"">{8}</a></dd>
	    </dl>
	    <ul id=""mainNavigation"">{3}
	    </ul>
    </div>
</div>
"
                , usernameLink // 0
                , channels // 1
                , container.WimRepository // 2
                , navigation // 3
                , container.WimPagePath // 4
                , skin // 5
                , Resource.ResourceManager.GetString("channel", new CultureInfo(container.CurrentApplicationUser.LanguageCulture)) //6
                , Resource.ResourceManager.GetString("logged_in_as", new CultureInfo(container.CurrentApplicationUser.LanguageCulture)) //7
                , Resource.ResourceManager.GetString("logout", new CultureInfo(container.CurrentApplicationUser.LanguageCulture)) //8
                , portalList //9
                , Resource.ResourceManager.GetString("portal", new CultureInfo(container.CurrentApplicationUser.LanguageCulture)) //10
                );
        }

        /// <summary>
        /// Footers this instance.
        /// </summary>
        /// <returns></returns>
        internal static string Footer(bool isNewDesign)
        {
            string newRichTextBoxContent = String.Empty;
            //newRichTextBoxContent = Wim.RichtextEditor.RichtextEditorManager.WriteInstances(
            //    Wim.Utility.AddApplicationPath(Wim.RichtextEditor.RichtextEditor.NICEDIT_LIB),
            //    Wim.Utility.AddApplicationPath(Wim.RichtextEditor.RichtextEditor.NICEDIT_ICONS),
            //    Wim.Utility.AddApplicationPath(Wim.CommonConfiguration.USE_RICHTEXT_STYLEFILE),
            //        Wim.Utility.AddApplicationPath("/repository/wim/portal.ashx"), isNewDesign);

            //string jquery = null;// @"<script src=""http://ajax.aspnetcdn.com/ajax/jQuery/jquery-1.7.1.min.js"" type=""text/javascript""></script>";
            //if (System.Configuration.ConfigurationManager.AppSettings["EXCLUDE_JQUERY_IN_FOOTER"] == "true")
            //    jquery = string.Empty;
            return newRichTextBoxContent;

//            return String.Format(@"{2}
//                                    <script src=""{1}"" type=""text/javascript""></script>{0}", newRichTextBoxContent,
//                                                                                                Wim.Utility.AddApplicationPath("/repository/wim/scripts/jquery.classbehaviours.extend.js")
//                                                                                                , jquery);

//            string newRichTextBoxContent = Wim.RichtextEditor.RichtextEditorManager.WriteInstances(Wim.Utility.AddApplicationPath(Sushi.Mediakiwi.Data.Environment.Current.RelativePath));

//            string jquery = @"<script src=""http://ajax.aspnetcdn.com/ajax/jQuery/jquery-1.7.1.min.js"" type=""text/javascript""></script>";
//            if (System.Configuration.ConfigurationManager.AppSettings["EXCLUDE_JQUERY_IN_FOOTER"] == "true")
//                jquery = string.Empty;
//            return String.Format(@"{2}
//                                    <script src=""{1}"" type=""text/javascript""></script>{0}"
//                , newRichTextBoxContent
//                , Wim.Utility.AddApplicationPath("/repository/wim/scripts/jquery.classbehaviours.extend.js")
//                , jquery);

        }

    }
}
