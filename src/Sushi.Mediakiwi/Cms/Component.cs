using Sushi.Mediakiwi.Data;
using Sushi.Mediakiwi.Framework;
using Sushi.Mediakiwi.Framework.ContentListItem;
using Sushi.Mediakiwi.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Beta.GeneratedCms.Source
{
    public class Component
    {
        bool m_IsNewDesign = true;

        #region Header

        void CreateBlock(Console container, string title, WimControlBuilder build, WimControlBuilder build2, ComponentVersion component)
        {
            CreateBlock(container, title, build, build2, component, true, false);
        }

        void CreateContentBlock(Console container, string title, WimControlBuilder build, WimControlBuilder build2, ComponentVersion component, bool isContainerClosed, bool skipHeader, bool skipTitle, bool isClosingStatement)
        {
            bool hasTableContent = (build2.Length > 0);
            bool hasContent = (build.Length > 0);
            if (m_IsNewDesign)
            {
                if (!hasContent && hasTableContent)
                {
                    CreateHeader(title, build, true, false, component);
                }

                if (hasTableContent)
                {
                    build.Append(build2);
                    CreateFooter(container, build);
                }

                if (isClosingStatement)
                {
                    return;
                }
            }
            else
            {
                if (!skipHeader)
                {
                    CreateHeader(container, title, build, build2, component, isContainerClosed);
                }

                if (component == null || !component.TemplateIsShared || container.IsComponent)
                {
                    build.Append(build2);
                }

                CreateFooter(container, build);

            }
        }

        void CreateBlock(Console container, string title, WimControlBuilder build, WimControlBuilder build2, ComponentVersion component, bool skipTitle, bool isClosingStatement)
        {
            bool hasTableContent = (build2.Length > 0);
            bool hasContent = (build.Length > 0);

            if (!hasContent && hasTableContent && component == null)
            {
                CreateHeader(null, build, true, true);
            }

            if (component != null)
            {
                CreateHeader(title, build, true, false, component);
            }

            if (hasTableContent)
            {
                build.Append(build2);
                if (build2.SearchGrid != null)
                {
                    if (build.SearchGrid == null)
                    {
                        build.SearchGrid = build2.SearchGrid;
                    }
                    else
                    {
                        build.SearchGrid += build2.SearchGrid;
                    }
                }
            }
            CreateFooter(container, build);

            build.Append("</dd>");

            if (build2.ApiResponse.Fields != null)
            {
                build.ApiResponse.Fields.AddRange(build2.ApiResponse.Fields);
            }

            if (isClosingStatement)
            {
                return;
            }

            if (component == null)
            {
                CreateHeader(title, build, true, skipTitle);
            }

            build.ApiResponse.Fields.Add(new Framework.Api.MediakiwiField()
            {
                Title = title,
                Value = title,
                Expression = OutputExpression.FullWidth,
                //PropertyName = infoItem.Property.ID,
                //PropertyType = Property.PropertyType.FullName,
                VueType = Framework.Api.MediakiwiFormVueType.wimSection
            });
        }


        /// <summary>
        /// Creates the generic error message.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="build">The build.</param>
        /// <param name="isValidInput">if set to <c>true</c> [is valid input].</param>
        void CreateGenericErrorMessage(Console container, WimControlBuilder build, ref bool isValidInput)
        {
            if (m_ErrorIsAdded)
            {
                return;
            }

            if (container.CurrentListInstance.wim.Notification.GenericErrors == null || container.CurrentListInstance.wim.Notification.GenericErrors.Count == 0)
            {
                if (isValidInput)
                {
                    CreateGenericInformationMessage(container, build, ref isValidInput);
                    return;
                }
                else
                {
                    container.CurrentListInstance.wim.Notification.AddError(
                        Labels.ResourceManager.GetString("required_fields", new System.Globalization.CultureInfo(container.CurrentApplicationUser.LanguageCulture))
                        );
                }
            }
            WimControlBuilder build2 = new WimControlBuilder();

            StringBuilder txtdata = new StringBuilder();
            foreach (string txt in container.CurrentListInstance.wim.Notification.GenericErrors)
            {
                if (txtdata.Length > 0)
                {
                    txtdata.Append("<br/>");
                }

                txtdata.Append(txt);
            }

            build.ApiResponse.Notifications.Add(new Framework.Api.MediakiwiNotification()
            {
                IsError = true,
                Message = txtdata.ToString()
            });


            build.Notifications.AppendFormat(@"
				<article class=""error"">
					{0}
				</article>"
                , txtdata);

            CreateGenericInformationMessage(container, build, ref isValidInput);
            m_ErrorIsAdded = true;
        }

        bool m_ErrorIsAdded;

        void CreateGenericInformationAlertMessage(Console container, WimControlBuilder build, ref bool isValidInput)
        {
            container.CurrentListInstance.wim.Notification.RestoreNotificationAlert();
            if (
                container.CurrentListInstance.wim.Notification.GenericInformationAlert == null ||
                container.CurrentListInstance.wim.Notification.GenericInformationAlert.Count == 0
            )
            {
                return;
            }


            container.CurrentListInstance.wim.Notification.ClearNotificationAlert();
            if (m_IsNewDesign)
            {
                StringBuilder txtdata = new StringBuilder();
                foreach (string txt in container.CurrentListInstance.wim.Notification.GenericInformationAlert)
                {
                    if (!string.IsNullOrEmpty(txt))
                    {
                        if (txtdata.Length > 0)
                        {
                            txtdata.Append("<br/>");
                        }
                        txtdata.Append(txt);
                    }
                }

                if (txtdata.Length > 0)
                {
                    build.ApiResponse.Notifications.Add(new Framework.Api.MediakiwiNotification()
                    {
                        IsError = false,
                        Message = txtdata.ToString()
                    });

                    build.Notifications.Append(@$"
                    <article class=""note ambiance"">
	                    {txtdata}
                    </article>");
                }
            }
        }

        void CreateGenericInformationMessage(Console container, WimControlBuilder build, ref bool isValidInput)
        {
            CreateGenericInformationAlertMessage(container, build, ref isValidInput);

            container.CurrentListInstance.wim.Notification.RestoreNotification();

            if (
                container.CurrentListInstance.wim.Notification.GenericInformation == null ||
                container.CurrentListInstance.wim.Notification.GenericInformation.Count == 0
            )
            {
                return;
            }

            container.CurrentListInstance.wim.Notification.ClearNotification();

            StringBuilder txtdata = new StringBuilder();
            foreach (string txt in container.CurrentListInstance.wim.Notification.GenericInformation)
            {
                if (!string.IsNullOrEmpty(txt))
                {
                    if (txtdata.Length > 0)
                    {
                        txtdata.Append("<br/>");
                    }

                    txtdata.Append(txt);
                }
            }
            if (txtdata.Length > 0)
            {
                build.ApiResponse.Notifications.Add(new Framework.Api.MediakiwiNotification()
                {
                    IsError = false,
                    Message = txtdata.ToString()
                });

                build.Notifications.Append($@"
                <article class=""note"">
                    <div class=""content"">
	                    {txtdata}
                    </div>
                </article>");
            }

        }

        public bool IsFirstHeader = true;

        void CreateHeader(Console container, string title, WimControlBuilder build, ComponentVersion component, bool isContainerClosed)
        {
            CreateHeader(container, title, build, null, component, isContainerClosed);
        }

        void CreateHeader(string title, WimControlBuilder build, bool addTable, bool skipTitle, ComponentVersion component = null)
        {
            if (!skipTitle)
            {
                string id = component == null ? "0" : component.ID.ToString();
                if (component == null || component.Template == null)
                {
                    build.AppendFormat("<div class=\"OrderingField\"><h3>{0}</h3><input type=\"hidden\" name=\"e{1}\" value=\"1\">", title, id);
                }
                else
                {
                    build.Append("<div class=\"OrderingField\">");


                    if (component.Template.CanDeactivate)
                        build.AppendFormat("<h3><input type=\"checkbox\" name=\"a{0}\" value=\"1\"{1} />{2}</h3><input type=\"hidden\" name=\"e{0}\" value=\"1\">"
                            , id
                            , (component.IsActive ? " checked=\"checked\"" : null)
                            , title);
                    else
                        build.AppendFormat("<h3>{0}</h3><input type=\"hidden\" name=\"e{1}\" value=\"1\">", title, id);

                    build.Append(@"<menu class=""componentMenu"">");

                    if (component.Template.CanMoveUpDown)
                    {
                        build.AppendFormat("<input type=\"hidden\" name=\"sort_{0}\" value=\"1\">", component.ID.ToString());
                        build.AppendFormat(@"<li class=""isSortable""><a href="""" onclick=""return mediakiwi.pageComponentControl.moveComponentUp(this, {0})"" >Up</a></li>
                                        <li><a href="""" onclick=""return mediakiwi.pageComponentControl.moveComponentDown(this, {0})"" >Down</a>", component.ID.ToString());
                    }
                    // MM:02-12-20 Headless
                    if (!component.Template.IsFixedOnPage)// && !component.IsFixed)
                    {
                        build.AppendFormat(@"<li><a href="""" onclick=""return mediakiwi.pageComponentControl.moveComponentDelete(this, {0}, '{1}', {2})"" class=""flaticon icon-trash-o""> </a></li>", component.ID.ToString(), component.Template.Name.Replace("'", "\\'"), component.TemplateID.ToString());
                    }
                    build.Append("</menu>");
                }
            }
            else
            {
                build.Append("<div class=\"OrderingField\">");
            }

            if (addTable)
            {
                build.Append("<table class=\"formTable\">");
            }
        }

        /// <summary>
        /// Creates the header.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="title">The title.</param>
        /// <param name="build">The build.</param>
        /// <param name="currentContent">Content of the current.</param>
        /// <param name="component">The component.</param>
        /// <param name="isContainerClosed">if set to <c>true</c> [is container closed].</param>
        void CreateHeader(Console container, string title, WimControlBuilder build, WimControlBuilder currentContent, ComponentVersion component, bool isContainerClosed)
        {
            bool hasClose = false;
            bool hasAdd = false;
            bool hasMoveUp = false;
            bool hasMoveDown = false;
            bool hasActiveState = false;
            bool hasRemove = false;

            hasClose = true;

            if (component != null)
            {
                if (container.ItemType == RequestItemType.Page && container.CurrentApplicationUser.IsDeveloper)
                {
                    var templateList = ComponentList.SelectOne(ComponentListType.ComponentTemplates);
                    ComponentTemplate template = ComponentTemplate.SelectOne(component.TemplateID);
                    title = string.Format("<a href=\"?list={2}&item={3}\" title=\"{1}\" style=\"color: #FFF\">{0}</a>", title, template.Location, templateList.ID, template.ID);
                }

                if (container.CurrentListInstance.wim.IsEditMode && container.ItemType == RequestItemType.Page)
                {
                    ComponentTemplate template = ComponentTemplate.SelectOne(component.TemplateID);

                    hasAdd = template.CanReplicate;

                    if (template.CanMoveUpDown && !template.IsHeader && !template.IsFooter)
                    {
                        hasMoveUp = true;
                        hasMoveDown = true;
                    }
                    hasActiveState = template.CanDeactivate;
                    hasRemove = !component.IsFixed;


                }
                if (!component.IsActive && hasActiveState)
                {
                    isContainerClosed = true;
                }
            }

            if (container.CurrentListInstance.wim.HideOpenCloseToggle)
            {
                isContainerClosed = false;
            }

            if (container.View == 2)
            {
                //  Search grid
                if (m_IsNewDesign)
                {//                    <h2>{0}</h2>

                    build.AppendFormat(@"
					<table class=""formTable"">
"
                        , title);
                }
                else
                {
                    //  Popup layer?
                    build.AppendFormat(@"
				<table class=""form{1}"">
					<tbody>
"
                        , title
                        ,
                            container.HasDoubleCols ? " fourColumnGrid" : (container.CurrentListInstance.wim.ShowInFullWidthMode ? " wideColumnGrid" : null)
                        );
                }
                return;
            }
            else if (container.View == 1)
            {
                string createNewTag = null;
                string listViewTag = null;

                if (m_IsNewDesign)
                {
                    //<h2>{0}</h2>
                    build.AppendFormat(@"
					<table class=""formTable"">
"
                        , title);
                }
                else
                {
                    build.AppendFormat(@"
		<dl class=""application"" id=""cp{4}"">
			<dt class=""link"">
				<span class=""label"">{0}</span>{1}{2}
			</dt>
			<dd>
				<table class=""form{3}"">
					<tbody>
"
                        , title
                        , createNewTag
                        , listViewTag
                        , container.HasDoubleCols ? " fourColumnGrid" : null
                        , (component != null) ? component.ID.ToString() : null
                        );
                }
                return;
            }

            if (container.View == 3 || container.View == 4)
            {
                if (m_IsNewDesign)
                {
                    //<h2>{0}</h2>
                    build.AppendFormat(@"
					<table class=""formTable"">
"
                        , title);
                }
                else
                {
                    build.Append(@"<dl class=""application"">");
                }
            }
            else
            {
                if (container.CurrentListInstance.wim.HideTopSectionTag && !m_IsNewDesign)
                {
                    if (m_IsNewDesign)
                    {
                        container.CurrentListInstance.wim.HideTopSectionTag = false;
                        build.Append(@"<table class=""formTable"">
");
                    }
                    else
                    {
                        //  Only hide the first.
                        container.CurrentListInstance.wim.HideTopSectionTag = false;
                        build.Append(@"
<dl class=""application"">
");
                    }
                }
                else
                {
                    if (component != null && component.TemplateIsShared && !container.IsComponent)
                    {
                        title += " - [SHARED]";
                    }

                    if (m_IsNewDesign)
                    {
                        //<h2>{0}</h2>
                        build.AppendFormat(@"
					<table class=""formTable"">
"
                            , title);
                    }
                    else
                    {

                        if (container.CurrentListInstance.wim.HideOpenCloseToggle || (component != null && component.TemplateIsShared))
                        {
                            build.AppendFormat(@"
<dl class=""application"" id=""cp{8}"">
	<dt class=""active"">{7}
        <span class=""label toggleNextNode parent_1"">{0}</span>{2}{3}{4}{5}{6}
	</dt>
"
                                , title
                                , ""
                                , hasAdd ? string.Format("<span class=\"add\"><button class=\"insertParentNode parent_3 src_frmXmlSource\">Add</button><input type=\"hidden\" id=\"frmXmlSource\" value=\"{0}?xml=component&id={1}&page={2}&cmpt={3}\"/></span>", container.WimPagePath, component.TemplateID, component.PageID, component.ID) : ""
                                , hasMoveUp ? string.Format("<span class=\"down\"><button class=\"moveParentNode parent_3 move_down\"><img alt=\"Down\" src=\"{0}/images/tool_empty.png\"/></button></span>", container.WimRepository) : ""
                                , hasMoveDown ? string.Format("<span class=\"up\"><button class=\"moveParentNode parent_3 move_up\"><img alt=\"Up\" src=\"{0}/images/tool_empty.png\"/></button></span>", container.WimRepository) : ""
                                , hasActiveState ? string.Format("<span class=\"activate\"><input type=\"checkbox\" name=\"active_{1}\"{2} value=\"1\" /><label>Active</label></span>", container.WimRepository, component.ID, component.IsActive ? " checked=\"checked\"" : null) : ""
                                , hasRemove ? string.Format("<span class=\"remove\"><button class=\"deleteParentNode parent_3 src_delsrc{0}\">Remove</button><input type=\"hidden\" id=\"delsrc{0}\" value=\"{1}?xml=delete&id={0}\"/></span>", component.ID, container.WimPagePath) : ""
                                , component == null ? null : string.Format(@"<input type=""hidden"" name=""element${0}"" value=""1""/>", component.ID)
                                , component == null ? null : component.ID.ToString()
                                );
                        }
                        else
                        {
                            build.AppendFormat(@"
<dl class=""application"" id=""cp{9}"">
	<dt class=""{8}"">{7}
        <span class=""label"">{0}</span>{1}{2}{3}{4}{5}{6}
	</dt>
"
                                , title
                                , hasClose ? string.Format("<span class=\"close toggleNextNode parent_1\"><button class=\"toClose\">&nbsp;</button><button class=\"toOpen\">&nbsp;</button></span>", container.WimRepository) : ""
                                , hasAdd ? string.Format("<span class=\"add\"><button class=\"insertParentNode parent_3 src_xmlsrc{3}\">Add</button><input type=\"hidden\" id=\"xmlsrc{3}\" value=\"{0}?xml=component&id={1}&page={2}&cmpt={3}\"/></span>", container.WimPagePath, component.TemplateID, component.PageID, component.ID) : ""
                                , hasMoveUp ? string.Format("<span class=\"down\"><button class=\"moveParentNode parent_3 move_down\"><img alt=\"Down\" src=\"{0}/images/tool_empty.png\"/></button></span>", container.WimRepository) : ""
                                , hasMoveDown ? string.Format("<span class=\"up\"><button class=\"moveParentNode parent_3 move_up\"><img alt=\"Up\" src=\"{0}/images/tool_empty.png\"/></button></span>", container.WimRepository) : ""
                                , hasActiveState ? string.Format("<span class=\"activate\"><input type=\"checkbox\" name=\"active_{1}\"{2} value=\"1\" /><label>Active</label></span>", container.WimRepository, component.ID, component.IsActive ? " checked=\"checked\"" : null) : ""
                                //, hasRemove ? string.Format("\n\t\t<span class=\"remove\"><button class=\"deleteParentNode parent_3\">Remove</button></span>", container.WimRepository) : ""
                                , hasRemove ? string.Format("<span class=\"remove\"><button class=\"deleteParentNode parent_3 src_delsrc{1}\">Remove</button><input type=\"hidden\" id=\"delsrc{1}\" value=\"{0}?xml=delete&id={1}\"/></span>", container.WimPagePath, component.ID) : ""
                                , component == null ? null : string.Format(@"<input type=""hidden"" name=""element${0}"" value=""1""/>", component.ID)
                                , isContainerClosed ? "link" : "active"
                                , component == null ? null : component.ID.ToString()
                                );
                        }
                    }
                }
            }

            if (m_IsNewDesign)
            {
                return;
            }

            string className = (container.CurrentListInstance.wim.CurrentList.Type == ComponentListType.Browsing && container.CurrentListInstance.wim.CurrentSite.MasterID.HasValue)
                    ? (container.CurrentApplicationUser.ShowTranslationView ? " translate" : null)
                    : null;

            if (container.CurrentListInstance.wim.Form.ShowTranslationData)
            {
                className = " translationGrid";
            }
            else if (container.HasDoubleCols)
            {
                className = " fourColumnGrid";
            }
            else if (container.CurrentListInstance.wim.ShowInFullWidthMode)
            {
                className = " wideColumnGrid";
            }

            build.AppendFormat("\n<dd class=\"{0}\"><table class=\"form{1}\"><tbody>",
                isContainerClosed ? "hideThisNode" : "showThisNode",
                className
                );
        }

        void CreateFooter(Console container, WimControlBuilder build)
        {
            if (container.ExpressionPrevious == OutputExpression.Left)
            {
                build.Append("<th><label>&nbsp;</label></th><td>&nbsp;</td></tr>");
            }
            container.ExpressionPrevious = OutputExpression.FullWidth;

            string cloaked = build.ToCloakedString(true);
            if (container.View == 2)
            {
                build.Append($"</table>{cloaked}");
                return;
            }
            if (!build.ToString().EndsWith("</article>", StringComparison.InvariantCultureIgnoreCase))
            {
                build.Append($"</table>{cloaked}</div>");
            }
        }
        #endregion

        /// <summary>
        /// Gets the property.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="sender">The sender.</param>
        /// <param name="property">The property.</param>
        /// <returns></returns>
        object GetProperty(Console container, object sender, string property)
        {
            if (sender is MetaData item)
            {
                return item.GetCollection(container);
            }

            foreach (System.Reflection.PropertyInfo info in sender.GetType().GetProperties())
            {
                //  Get all public properties
                if (info.CanRead && info.Name == property)
                {   // Get all writable public properties
                    return info.GetValue(sender, null);
                }
            }

            //  Fall back
            if (container.CurrentListInstance != sender)
            {
                foreach (System.Reflection.PropertyInfo info in container.CurrentListInstance.GetType().GetProperties())
                {
                    //  Get all public properties
                    if (info.CanRead && info.Name == property)
                    {   // Get all writable public properties
                        return info.GetValue(container.CurrentListInstance, null);
                    }
                }
            }

            throw new Exception(string.Format("Could not find the '{0}' property.", property));
        }

        /// <summary>
        /// Creates the content of the component.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="componentTemplateId">The component template id.</param>
        /// <param name="pageId">The page id.</param>
        /// <param name="copyComponentId">The copy component id.</param>
        /// <returns></returns>
        internal async Task<string> CreateComponentContentAsync(Console container, int componentTemplateId, int pageId, int copyComponentId, string target)
        {
            WimControlBuilder build = new WimControlBuilder();
            WimControlBuilder build2 = new WimControlBuilder();
            List<Field> fieldList;

            int count = 0;
            bool isContainerClosed = false;

            fieldList = new List<Field>();

            ComponentTemplate template = await ComponentTemplate.SelectOneAsync(componentTemplateId).ConfigureAwait(false);

            ComponentVersion copy = await ComponentVersion.SelectOneAsync(copyComponentId).ConfigureAwait(false);

            ComponentVersion version = new ComponentVersion();

            version.ApplicationUserID = container.CurrentApplicationUser.ID;
            version.TemplateID = template.ID;
            version.PageID = pageId;
            version.Name = template.Name;
            version.TemplateIsShared = template.IsShared;
            version.IsFixed = false;
            version.IsAlive = true;
            version.IsSecundary = copy.IsSecundary;
            version.SortOrder = 1000;
            version.IsActive = true;
            version.Target = (copy?.ID > 0) ? copy.Target : target;
            await version.SaveAsync().ConfigureAwait(false);


            MetaData[] metadata = (MetaData[])Utility.GetDeserialized(typeof(MetaData[]), template.MetaData);

            Content content = null;

            if (metadata != null)
            {
                foreach (MetaData item in metadata)
                {
                    IContentInfo x = item.GetContentInfo();

                    if (x != null)
                    {
                        ((ContentSharedAttribute)x).ComponentTemplateID = template.ID;
                        ((ContentSharedAttribute)x).Console = container;
                        ((ContentSharedAttribute)x).IsBluePrint = true;
                        count++;
                        
                        x.ID = string.Concat("e", version.ID, "c", count);
                        x.FieldName = item.Name;
                        ((ContentSharedAttribute)x).m_ListItemCollection = item.GetCollection(container);

                        Field field2;
                        if (content == null)
                        {
                            field2 = new Field();
                            field2.Value = item.Default;
                            field2.InheritedValue = field2.Value;
                            x.SetCandidate(field2, container.CurrentListInstance.wim.IsEditMode);
                        }
                        else
                        {
                            field2 = content[item.Name];
                            if (field2 == null)
                            {
                                field2 = new Field();
                                field2.Value = item.Default;
                                field2.InheritedValue = item.Default;
                            }

                            x.SetCandidate(field2, container.CurrentListInstance.wim.IsEditMode);
                        }

                        //  Validate input
                        if (!x.IsValid(x.Mandatory))
                        {
                            isContainerClosed = false;
                        }

                        Field field = x.WriteCandidate(build2, container.CurrentListInstance.wim.IsEditMode, x.Mandatory, x.IsCloaked);

                        
                        // IsCloacked will be set to true whenever a shared field is
                        // loaded that is not supposed to output to the page
                        if (field != null && ((item.IsSharedField == "0") || (item.IsSharedField != "0" && x.IsCloaked == false)))
                        {
                            fieldList.Add(field);
                        }
                        else 
                        { 
                            // Do nothing, this is a Shared field that should not be added to the output
                        }
                    }
                }
            }
            CreateBlock(container, template.Name, build, build2, version, isContainerClosed, false);

            return build.ToString();
        }

        ComponentVersion[] m_InheritedComponents;
        public ComponentVersion GetInheritedComponent(ComponentVersion component)
        {
            if (m_InheritedComponents == null)
            {
                return null;
            }

            foreach (ComponentVersion version in m_InheritedComponents)
            {
                if (component.MasterID.HasValue)
                {
                    if (component.MasterID.Value == version.ID)
                    {
                        return version;
                    }
                }
                else
                {
                    if (version.TemplateID != component.TemplateID)
                    {
                        continue;
                    }

                    if (version.FixedFieldName == component.FixedFieldName)
                    {
                        return version;
                    }

                    if (version.AvailableTemplateID == component.AvailableTemplateID)
                    {
                        return version;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Creates the page component shadow copy.
        /// </summary>
        /// <param name="applySortOrder">if set to <c>true</c> [apply sort order].</param>
        /// <param name="pageID">The page ID.</param>
        /// <param name="childComponents">The child components.</param>
        /// <param name="masterComponents">The master components.</param>
        /// <param name="foundChange">if set to <c>true</c> [found change].</param>
        void CreatePageComponentShadowCopy(bool applySortOrder, int pageID, ref ComponentVersion[] childComponents, ComponentVersion[] masterComponents, out bool foundChange)
        {
            foundChange = false;
            //Sushi.Mediakiwi.Data.Component masterComponent = GetInheritedComponent(

            //  Match all children
            foreach (ComponentVersion child in childComponents)
            {
                if (child.MasterID.HasValue)
                {
                    continue;
                }

                ComponentVersion match = null;
                foreach (ComponentVersion master in masterComponents)
                {
                    if (child.TemplateID != master.TemplateID)
                    {
                        continue;
                    }

                    if ((child.FixedFieldName == master.FixedFieldName) || (child.AvailableTemplateID == master.AvailableTemplateID))
                    {
                        match = master;
                        break;
                    }
                }
                if (match != null)
                {
                    //  Found it, please apply master relation
                    child.MasterID = match.ID;
                    child.IsActive = match.IsActive;
                    
                    if (applySortOrder)
                    {
                        child.SortOrder = match.SortOrder;
                    }

                    foundChange = true;

                    child.Save();
                }
            }

            foreach (ComponentVersion master in masterComponents)
            {
                bool foundChild = false;
                foreach (ComponentVersion child in childComponents)
                {
                    if (child.MasterID.HasValue && child.MasterID.Value == master.ID)
                    {
                        foundChild = true;
                        break;
                    }
                }
                if (!foundChild)
                {
                    ComponentVersion version = new ComponentVersion();
                    Utility.ReflectProperty(master, version);
                    version.ID = 0;
                    version.MasterID = master.ID;
                    version.PageID = pageID;
                    version.Serialized_XML = null;
                    version.Save();
                    foundChange = true;
                }

            }
        }

        internal WimControlBuilder CreateContentList(Console container, int openInFrame, bool showServiceColumn, out Page page, string section)
        {
            return CreateContentList(container, openInFrame, showServiceColumn, out page, section, false);
        }

        string m_CurrentPageVersionComponentXML;
        string m_CurrentPageVersionComponentXML_Hash;


        internal void SavePageVersion(Page page, ComponentVersion[] componentVersions, IApplicationUser user, bool isArchived)
        {
            SetPageVersionXml(componentVersions);
            SavePageVersion(page, user, isArchived);
        }

        internal void SetPageVersionXml(ComponentVersion[] componentVersions)
        {
            var copyList = new List<ComponentVersion>();

            StringBuilder contentHash = new StringBuilder();
            foreach (var version in componentVersions)
            {
                bool hasContent = false;

                var content = version.GetContent();

                if (content != null && content.Fields != null)
                {

                    foreach (var item in content.Fields)
                    {
                        if (!string.IsNullOrEmpty(item.Value))
                        {
                            if (
                                item.Type == (int)ContentType.Binary_Image
                                || item.Type == (int)ContentType.Hyperlink
                                || item.Type == (int)ContentType.Binary_Document
                                || item.Type == (int)ContentType.PageSelect
                                || item.Type == (int)ContentType.FolderSelect
                                || item.Type == (int)ContentType.Choice_Dropdown
                                )
                            {
                                if (item.Value == "0")
                                {
                                    continue;
                                }
                            }

                            hasContent = true;
                            contentHash.Append(item.Value);
                        }
                    }
                }

                if (hasContent)
                {
                    copyList.Add(version);
                }
            }

            if (copyList.Count == 0)
            {
                m_CurrentPageVersionComponentXML_Hash = null;
                m_CurrentPageVersionComponentXML = null;
                return;

            }
            m_CurrentPageVersionComponentXML_Hash = Utility.HashString(contentHash.ToString());
            m_CurrentPageVersionComponentXML = Utility.GetSerialized(copyList);
        }

        void SavePageVersion(Page page, IApplicationUser user, bool isArchived)
        {
            if (string.IsNullOrEmpty(m_CurrentPageVersionComponentXML) || page == null)
            {
                return;
            }

            // [MR:17-06-2021] must be remoived, for testing purposes only
            if (string.IsNullOrWhiteSpace(page.CompletePath))
            {
                page.SetInternalPath();
            }

            var pageVersion = new PageVersion();
            pageVersion.ContentXML = m_CurrentPageVersionComponentXML;
            pageVersion.MetaDataXML = Utility.GetSerialized(page);
            pageVersion.UserID = user.ID;
            pageVersion.PageID = page.ID;
            pageVersion.TemplateID = page.TemplateID;
            pageVersion.IsArchived = isArchived;
            pageVersion.Name = page.Name;
            pageVersion.CompletePath = page.CompletePath;
            pageVersion.Hash = m_CurrentPageVersionComponentXML_Hash;
            pageVersion.Save();
        }

        /// <summary>
        /// Creates the page content form.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="openInFrame">The open in frame.</param>
        /// <param name="showServiceColumn">if set to <c>true</c> [show service column].</param>
        /// <param name="page">The page.</param>
        /// <param name="section">The section.</param>
        /// <param name="isPartOfList">if set to <c>true</c> [is part of list].</param>
        /// <returns></returns>
        internal WimControlBuilder CreateContentList(Console container, int openInFrame, bool showServiceColumn, out Page page, string section, bool isPartOfList)
        {
            ComponentVersion[] components = null;
            WimControlBuilder build = new WimControlBuilder();
            WimControlBuilder build2 = new WimControlBuilder();
            List<Field> fieldList = null;

            if (container.IsComponent)
            {
                page = null;
                components = new ComponentVersion[] { ComponentVersion.SelectOne(container.Item.Value) };
                // [CB: 3-1-2016] dit is nodig om het in edit te open
                container.CurrentListInstance.wim.IsEditMode = true;
                container.CurrentListInstance.wim.CanSaveAndAddNew = false;

            }
            else
            {
                page = Page.SelectOne(container.Item.Value, false);
                page.Template.CheckComponentTemplates(page.ID);

                components = ComponentVersion.SelectAllOnPage(page.ID);

                if (page.MasterID.HasValue)
                {
                    Page master = page;
                    while (master.MasterID.HasValue)
                    {
                        master = Page.SelectOne(master.MasterID.Value);
                        if (!master.InheritContent)
                        {
                            break;
                        }
                    }
                    m_InheritedComponents = ComponentVersion.SelectAll(master.ID);
                }

                bool foundChange = false;
                if (m_InheritedComponents?.Length > 0)
                {
                    CreatePageComponentShadowCopy(true, page.ID, ref components, m_InheritedComponents, out foundChange);
                }

                if (foundChange)
                {
                    components = ComponentVersion.SelectAll(page.ID);
                }
            }

            int versionID = Utility.ConvertToInt(container.Request.Query["version"]);
            if (versionID > 0)
            {
                var version = PageVersion.SelectOne(versionID);
                if (!version.IsNewInstance)
                {
                    var versionlist = Utility.GetDeserialized(typeof(ComponentVersion[]), version.ContentXML) as ComponentVersion[];

                    foreach (var item in components)
                    {
                        var found = (from selected in versionlist where item.ID == selected.ID select selected).SingleOrDefault();
                        if (found == null)
                        {
                            item.Serialized_XML = null;
                        }
                        else
                        {
                            item.Serialized_XML = found.Serialized_XML;
                        }
                    }
                }
            }

            SetPageVersionXml(components);

            bool isValidPage = true;
            bool isValidInput = true;
            bool isContainerClosed = false;

            int count = 0;

            Field customDateField = null;
            //  If the page has custom date as an option this field is added to the top op the page
            if (page != null && page.Template.HasCustomDate && !showServiceColumn)
            {
                Framework.ContentInfoItem.DateTimeAttribute dt = new Framework.ContentInfoItem.DateTimeAttribute("Custom date", true);
                dt.Console = container;
                dt.IsBluePrint = true;
                dt.ID = string.Concat("element", count);
                dt.FieldName = "CustomDate";

                Field field1 = new Field();
                field1.Value = page.CustomDate.HasValue ? page.CustomDate.Value.Ticks.ToString() : string.Empty;

                dt.SetCandidate(field1, container.CurrentListInstance.wim.IsEditMode);
                customDateField = dt.WriteCandidate(build2, container.CurrentListInstance.wim.IsEditMode, false, false);

                if (!dt.IsValid(dt.Mandatory))
                {
                    isValidInput = false;
                    isValidPage = false;
                    isContainerClosed = false;
                }

                CreateBlock(container, "Generic", build, build2, null, isContainerClosed, false);
                build2 = new WimControlBuilder();
            }

            IComponent[] visible = null;
            if (page != null)
            {
                visible = Data.Component.VerifyVisualisation(page.Template, components, section, ref showServiceColumn, true);
            }
            else
            {
                visible = components;
            }

            var selection = new List<IComponent>();
            if (visible != null)
            {
                foreach (var component in visible)
                {
                    var version = (ComponentVersion)component;
                    if (container.CurrentListInstance.wim.IsEditMode && !version.IsFixed && container.IsPostBackSave && container.Form(string.Concat("e", component.ID)) != "1")
                    {
                        version.Delete();
                        continue;
                    }
                    selection.Add(component);
                }
            }

            int c_index = 0;
            foreach (ComponentVersion component in selection)
            {
                ComponentTemplate template = component.Template;
                MetaData[] metadata = (MetaData[])Utility.GetDeserialized(typeof(MetaData[]), template.MetaData);

                SetComponent(visible,
                    component,
                    ref c_index,
                    ref isValidInput,
                    ref isContainerClosed,
                    ref isValidPage,
                    ref showServiceColumn,
                    ref fieldList,
                    ref count,
                    container,
                    ref page,
                    ref build,
                    ref build2,
                    metadata,
                    template.CanDeactivate, true
                    );

                if (container.CurrentListInstance.wim.IsSaveMode && isValidInput)
                {
                    component.Save();
                }
            }

            CreateGenericErrorMessage(container, build, ref isValidInput);

            if (container.CurrentListInstance.wim.IsSaveMode && isValidPage)
            {
                //  Save the version
                SavePageVersion(page, container.CurrentApplicationUser, false);

                if (container.IsComponent)
                {
                    container.Response.Redirect(container.UrlBuild.GetListRequest(container.CurrentList, container.Item));
                    return build;
                }

                page.Updated = Data.Common.DatabaseDateTime;

                if (page.Template.HasCustomDate && customDateField != null && !string.IsNullOrEmpty(customDateField.Value))
                    page.CustomDate = new DateTime(long.Parse(customDateField.Value));
                page.Save();

                if (!isPartOfList && openInFrame == 0)
                {
                    if (container.Item.GetValueOrDefault(0) == 0)
                    {
                        container.Response.Redirect(string.Concat(container.WimPagePath, "?folder=", container.CurrentListInstance.wim.CurrentFolder.ID));
                    }
                    else
                    {
                        string set = container.Request.Query["tab"];
                        string redirect = string.IsNullOrEmpty(set) ? "" : string.Concat("&tab=", set);

                        if (container.IsNotFramed)
                        {
                            redirect += string.Concat("&nf=", container.Request.Query["nf"]);
                        }

                        container.Response.Redirect(string.Concat(container.WimPagePath, "?page=", container.Item.Value, redirect));
                    }
                }
            }

            return build;
        }

        internal void SetComponent(
            IComponent[] c,
            ComponentVersion component,
            ref int c_index,
            ref bool isValidInput,
            ref bool isContainerClosed,
            ref bool isValidPage,
            ref bool showServiceColumn,
            ref List<Field> fieldList,
            ref int count,
            Console container,
            ref Page page,
            ref WimControlBuilder build,
            ref WimControlBuilder build2,
            MetaData[] metadata,
            bool canDeactivate,
            bool createBlock = true, string templatePrefix = null
            )
        {
            c_index++;
            ComponentVersion inherited = GetInheritedComponent(component);

            count = 0;

            isValidInput = true;
            isContainerClosed = false;

            fieldList = new List<Field>();

            Content content = null;
            Content inheritedContent = null;
            if (!container.CurrentListInstance.IsPostBack || m_IsNewDesign)
            {
                content = Content.GetDeserialized(component.Serialized_XML);

                if (inherited != null)
                {
                    inheritedContent = Content.GetDeserialized(inherited.Serialized_XML);
                }
            }

            if (metadata != null)
            {
                foreach (MetaData item in metadata)
                {
                    IContentInfo x = item.GetContentInfo();
                    if (page != null && page.MasterID.HasValue)
                    {
                        x.ShowInheritedData = container.CurrentApplicationUser.ShowTranslationView;
                    }

                    if (x != null)
                    {
                        ((ContentSharedAttribute)x).Console = container;
                        ((ContentSharedAttribute)x).IsBluePrint = true;
                        ((ContentSharedAttribute)x).ComponentTemplateID = component.TemplateID;

                        count++;

                        if (!string.IsNullOrEmpty(templatePrefix))
                        {
                            x.ID = string.Concat(templatePrefix, "_", item.Name);
                        }
                        else
                        {
                            x.ID = string.Concat("e", component.ID, "c", count);
                        }

                        x.FieldName = item.Name;
                        x.IsSharedField = item.IsSharedField == "1";

                        ((ContentSharedAttribute)x).m_ListItemCollection = item.GetCollection(container);

                        Field field2;
                        if (content == null)
                        {
                            field2 = new Field();
                            field2.Value = item.Default;

                            if (inheritedContent != null)
                            {
                                field2.InheritedValue = inheritedContent[item.Name] == null ? null : inheritedContent[item.Name].Value;
                                ((ContentSharedAttribute)x).IsInheritedField = inheritedContent.HasProperty(item.Name);
                            }

                            // [2014.12.01:MM]  Take inheritance
                            // [MR:19-01-2022] Inheritance fix, was :
                            //((ContentSharedAttribute)x).IsInheritedField = !string.IsNullOrEmpty(item.IsInheritedField);
                            x.SetCandidate(field2, container.CurrentListInstance.wim.IsEditMode);
                        }
                        else
                        {
                            field2 = content[item.Name];
                            if (field2 == null)
                            {
                                field2 = new Field();
                                field2.Value = item.Default;
                            }

                            if (inheritedContent != null)
                            {
                                field2.InheritedValue = inheritedContent[item.Name] == null ? null : inheritedContent[item.Name].Value;
                                ((ContentSharedAttribute)x).IsInheritedField = inheritedContent.HasProperty(item.Name);
                            }

                            // [2014.12.01:MM]  Take inheritance
                            // [MR:19-01-2022] Inheritance fix, was :
                            //((ContentSharedAttribute)x).IsInheritedField = !string.IsNullOrEmpty(item.IsInheritedField);
                            x.SetCandidate(field2, container.CurrentListInstance.wim.IsEditMode);
                        }

                        //  Validate input
                        if (!x.IsValid(x.Mandatory) && (!component.TemplateIsShared || container.IsComponent))
                        {
                            bool isActive = container.Form(string.Concat("a", component.ID)) == "1";
                            if (isActive || !canDeactivate || container.IsComponent)
                            {
                                isValidInput = false;
                                isValidPage = false;
                                isContainerClosed = false;
                            }
                        }

                        Field field = x.WriteCandidate(build2, container.CurrentListInstance.wim.IsEditMode, x.Mandatory, x.IsCloaked);
                        if (field != null)
                        {
                            fieldList.Add(field);
                        }
                    }
                }
            }

            if (isValidInput && container.CurrentListInstance.wim.IsSaveMode)
            {
                isContainerClosed = true;
            }

            if (!component.IsActive && !container.CurrentListInstance.wim.IsSaveMode)
            {
                isContainerClosed = true;
            }

            bool isClosing = c.Length == c_index;

            if (createBlock)
            {
                CreateBlock(container, component.Name, build, build2, component, false, isClosing);
            }
            else
            {
                build.Append(build2);
            }

            build2 = new WimControlBuilder();

            if (content == null)
            {
                content = new Content();
            }

            if (fieldList != null && fieldList.Count > 0)
            {
                content.Fields = fieldList.ToArray();
            }

            int sortOrder = 0;
            //  sort_ID
            Hashtable sorted = new Hashtable();

            if (container.Request.HasFormContentType)
            {
                var keys = (from k in container.Request.Form.Keys where k.StartsWith("sort_") select k).ToList();
                keys.ForEach(x =>
                {
                    sortOrder++;
                    sorted.Add(Convert.ToInt32(x.Replace("sort_", string.Empty)), sortOrder);
                });
            }

            sortOrder = 0;

            if (container.CurrentListInstance.wim.IsSaveMode && isValidInput)
            {
                bool isActive = true;

                if (container.IsComponent)
                {
                    component.IsActive = true;
                    component.PageID = null;

                }
                else
                {
                    if (canDeactivate)
                    {
                        isActive = container.Form(string.Concat("a", component.ID)) == "1";
                    }
                    else
                    {
                        isActive = true;
                    }

                    component.IsActive = isActive;
                }

                component.SortOrder = sorted.ContainsKey(component.ID) ? (int)sorted[component.ID] : sortOrder;
                component.IsSecundary = showServiceColumn;
                component.Serialized_XML = Content.GetSerialized(content);
            }
        }

        #region ShouldActAsDataStore event subscriptions
        Task CurrentListInstance_ListLoad(ComponentListEventArgs e)
        {
            return Task.CompletedTask;
        }

        #endregion

        /// <summary>
        /// Creates the search list.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="openInFrame">The open in frame.</param>
        /// <returns></returns>
        public async Task<WimControlBuilder> CreateSearchListAsync(Console container, int openInFrame)
        {
            if (container.CurrentListInstance.wim.Page.Body.Filter.DisableDefaultSetup)
            {
                return await CreateSearchListAsync(container, openInFrame, null);
            }
            // [CB; 14-11-2016] change with aknowledgement from Mark de vries. This helps developers to assign a local cache for the filter options
            if (container.CurrentListInstance.wim.HasOwnSearchListCache)
            {
                return await CreateSearchListAsync(container, openInFrame, container.CurrentListInstance.wim.CurrentVisitor.Data["wim_FilterInfo_" + container.CurrentListInstance.wim.CurrentList.GUID].Value);
            }
            else
            {
                return await CreateSearchListAsync(container, openInFrame, container.CurrentListInstance.wim.CurrentVisitor.Data["wim_FilterInfo"].Value);
            }
        }

        /// <summary>
        /// Creates the search list.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="openInFrame">The open in frame.</param>
        /// <param name="serialized">The serialized.</param>
        /// <returns></returns>
        internal async Task<WimControlBuilder> CreateSearchListAsync(Console container, int openInFrame, string serialized)
        {
            m_AllListProperties = null;

            if (serialized != null && openInFrame == 0 && !container.CurrentListInstance.wim.Page.Body.Filter.DisableDefaultSetup)
            {
                m_Content = Content.GetDeserialized(serialized);
            }

            return await CreateSearchListAsync(container, openInFrame, m_Content, false);
        }

        bool m_IgnoreDataList;

        /// <summary>
        /// Creates the search list.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="openInFrame">The open in frame.</param>
        /// <param name="content">The content.</param>
        /// <param name="ignoreDataList">if set to <c>true</c> [ignore data list].</param>
        /// <returns></returns>
        internal async Task<WimControlBuilder> CreateSearchListAsync(Console container, int openInFrame, Content content, bool ignoreDataList)
        {
            m_ShowSearchButton = true;
            m_IgnoreDataList = ignoreDataList;

            WimControlBuilder build = new WimControlBuilder();
            bool isValidInput = true;

            if (openInFrame == 0)
            {
                m_Content = content;
                if (m_Content != null)
                {
                    Framework.Templates.PropertySet pset = new Framework.Templates.PropertySet();
                    pset.SetValue(container.CurrentListInstance.wim.CurrentSite, container.CurrentListInstance, m_Content, null);

                    if (string.IsNullOrEmpty(container.ListPagingValue) && m_Content.Fields != null)
                    {
                        foreach (Field item in m_Content.Fields)
                        {
                            if (item.Type == 0 && item.Property == string.Concat("wim_set#", container.CurrentList.ID))
                            {
                                container.ListPagingValue = item.Value;
                                break;
                            }
                        }
                    }
                }
            }

            
            await SetSavedInfoAsync(container);

            List<Field> list = new List<Field>();

            System.Diagnostics.Trace.WriteLine("Scanning component list (reading)");

            ScanClass(container, build, false, false, ref isValidInput, list);
            if (m_IgnoreDataList)
            {
                return null;
            }

            if (!string.IsNullOrEmpty(m_ClickedButton) && container.CurrentListInstance.wim.HasListAction)
            {
                System.Diagnostics.Trace.WriteLine("DoListAction");
                container.CurrentListInstance.wim.DoListAction(container.Item.GetValueOrDefault(0), 0, m_ClickedButton, isValidInput);
            }

            System.Diagnostics.Trace.WriteLine("DoListSearch");

            container.CurrentListInstance.SenderInstance = container.CurrentListInstance;
            container.CurrentListInstance.wim.DoListSearch();

            System.Diagnostics.Trace.WriteLine("Scanning component list (writing)");
            ScanClass(container, build, true, false, ref isValidInput, list);

            CreateGenericErrorMessage(container, build, ref isValidInput);

            if (openInFrame == 0)
            {
                if (m_Content == null)
                {
                    m_Content = new Content();
                }

                if (!string.IsNullOrEmpty(container.ListPagingValue))
                {
                    Field set = new Field();
                    set.Type = 0;
                    set.Property = string.Concat("wim_set#", container.CurrentList.ID);
                    set.Value = container.ListPagingValue;
                    list.Add(set);
                }

                m_Content.Fields = list.ToArray();

                //  Remember search parameters
                if (container.CurrentListInstance.wim.IsClearingListCache == false)
                {
                    if (container.CurrentListInstance.wim.HasOwnSearchListCache)
                    {
                        container.CurrentListInstance.wim.CurrentVisitor.Data.Apply("wim_FilterInfo_" + container.CurrentList.GUID.ToString(), Content.GetSerialized(m_Content));
                    }
                    else
                    {
                        container.CurrentListInstance.wim.CurrentVisitor.Data.Apply("wim_FilterInfo", Content.GetSerialized(m_Content));
                    }

                    await container.CurrentListInstance.wim.CurrentVisitor.SaveAsync();
                }
            }

            return build;
        }

        internal List<ButtonAttribute> m_ButtonList;
        public List<ButtonAttribute> ButtonList => m_ButtonList;

        async Task SetSavedInfoAsync(Console container)
        {
            if (!container.CurrentListInstance.wim.CurrentVisitor.Data["wim.note"].IsNull)
            {
                string note = container.CurrentListInstance.wim.CurrentVisitor.Data["wim.note"].Value;
                container.CurrentListInstance.wim.CurrentVisitor.Data.Apply("wim.note", null);
                await container.CurrentListInstance.wim.CurrentVisitor.SaveAsync();
                container.CurrentListInstance.wim.Notification.AddNotificationAlert(note);
            }
        }

        Content m_Content;
        /// <summary>
        /// Creates the list item form
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="openInFrame">The open in frame.</param>
        /// <returns></returns>
        public async Task<WimControlBuilder> CreateListAsync(Console container, int openInFrame, bool isJSONRequest = false)
        {
            int listId = container.CurrentList.ID;

            //  When the list acts as a datastore it can only contain one instance.
            if (container.CurrentListInstance.wim.ShouldActAsDataStore)
            {
                container.CurrentListInstance.wim.CanContainSingleInstancePerDefinedList = true;
                container.Item = container.CurrentListInstance.wim.CurrentSite.ID;

                System.Diagnostics.Trace.WriteLine("ShouldActAsDataStore");

                container.CurrentListInstance.ListLoad += CurrentListInstance_ListLoad;
            }

            await SetSavedInfoAsync(container);

            //  Obtain the current list version used for versioning
            ComponentListVersion currentVersion;
            if (openInFrame == -1)
            {
                currentVersion = new ComponentListVersion();
            }
            else
            {
                if (container.Item.HasValue || container.CurrentListInstance.wim.CanContainSingleInstancePerDefinedList)
                {
                    currentVersion = await ComponentListVersion.SelectOneAsync(listId, container.Item.GetValueOrDefault(0));
                }
                else
                {
                    currentVersion = new ComponentListVersion();
                }

                //  Get the content from the current versioned list
                m_Content = Content.GetDeserialized(currentVersion.Serialized_XML);
            }


            WimControlBuilder build = new WimControlBuilder();

            List<Field> fieldList = null;

            bool isValidInput = !(container.CurrentListInstance.wim.Notification.GenericErrors != null && container.CurrentListInstance.wim.Notification.GenericErrors.Count > 0);
            bool forceLoadEvent = false;

            //  Trigger the load event
            //  Should act as dataStore
            if (m_Content != null && container.CurrentListInstance.wim.ShouldActAsDataStore)
            {
                Framework.Templates.PropertySet pset = new Framework.Templates.PropertySet();
                pset.SetValue(container.CurrentListInstance.wim.CurrentSite, container.CurrentListInstance, m_Content, null);
            }
            else
            {
                container.CurrentListInstance.wim.IsCurrentList = true;
                System.Diagnostics.Trace.WriteLine("DoListLoad");

                container.CurrentListInstance.SenderInstance = container.CurrentListInstance;
                container.CurrentListInstance.wim.DoListLoad(container.Item.GetValueOrDefault(0), currentVersion.ID, Utility.ConvertToInt(container.Request.Query["pitem"]), isValidInput);
            }

            if (!string.IsNullOrEmpty(container.CurrentListInstance.wim.OnLoadScript))
            {
                build.Add(new LiteralControl(container.CurrentListInstance.wim.OnLoadScript));
            }

            System.Diagnostics.Trace.WriteLine("ScanClass (read)");
            ScanClass(container, build, false, false, ref isValidInput);

            //  [20090224:MM] Introduced for setting the correct data for the properties. First read and set all, and then set again so dependencies can work.
            if (container.Request.Method == "POST")// container.Request.HasFormContentType && container.Request.Form.Count > 0)
            {
                System.Diagnostics.Trace.WriteLine("ScanClass (read with postback)");
                m_AllListProperties = null;
                ScanClass(container, build, false, false, ref isValidInput);
            }
            //  [END]

            if (!container.CurrentListInstance.wim.IsSaveMode && m_TriggerSaveEvent)
            {
                System.Diagnostics.Trace.WriteLine("ScanClass (trigger save)");

                container.CurrentListInstance.wim.IsSaveMode = true;
                ScanClass(container, build, false, false, ref isValidInput);
            }

            // 19 nov 2017: Fix
            isValidInput = m_AllListProperties.Any(x => x.IsValid.HasValue && !x.IsValid.Value && x.IsVisible) == false;

            //  When there is an PreRender event the output HTML could be different, so first set the properties without HTML output
            if (container.CurrentListInstance.wim.HasListPreRender)
            {
                System.Diagnostics.Trace.WriteLine("DoListPreRender");
                container.CurrentListInstance.wim.DoListPreRender(container.Item.GetValueOrDefault(0), currentVersion.ID, isValidInput);
                fieldList = new List<Field>();

                isValidInput = !(container.CurrentListInstance.wim.Notification.GenericErrors != null && container.CurrentListInstance.wim.Notification.GenericErrors.Count > 0);

                System.Diagnostics.Trace.WriteLine("ScanClass (prerender write)");

                ScanClass(container, build, true, !container.CurrentListInstance.wim.ShouldPostPreRenderLoadFormRequest, ref isValidInput, fieldList);
            }

            // 19 nov 2017: Fix
            // 2018-02-22 MV: Also take into consideration the result of the pre-render event (which can generate generic errors)
            isValidInput = isValidInput && m_AllListProperties.Any(x => x.IsValid.HasValue && !x.IsValid.Value && x.IsVisible) == false;

            if (!string.IsNullOrEmpty(m_ClickedButton) && container.CurrentListInstance.wim.HasListAction)
            {
                if (!container.CurrentListInstance.wim.IsEditMode)
                {
                    container.CurrentListInstance.wim.IsCurrentList = true;
                    container.CurrentListInstance.wim.DoListLoad(container.Item.GetValueOrDefault(0), currentVersion.ID, Utility.ConvertToInt(container.Request.Query["pitem"]), isValidInput);
                    forceLoadEvent = true;
                }
                System.Diagnostics.Trace.WriteLine("DoListAction");
                container.CurrentListInstance.wim.DoListAction(container.Item.GetValueOrDefault(0), currentVersion.ID, m_ClickedButton, isValidInput);

                if (!container.CurrentListInstance.wim.IsEditMode)
                {
                    CreateGenericErrorMessage(container, build, ref isValidInput);
                }
            }

            CreateGenericErrorMessage(container, build, ref isValidInput);

            // [JP 06-08-2021: Added to enable ListDelete for JSON Request]
            if (container.CurrentListInstance.wim.IsDeleteMode && isJSONRequest)
            {
                //  Added delete event
                container.CurrentListInstance.wim.DoListDelete(container.Item.GetValueOrDefault(0), currentVersion.ID, isValidInput);
            }
            if (container.CurrentListInstance.wim.IsSaveMode)
            {

                if (isValidInput)
                {
                    System.Diagnostics.Trace.WriteLine("DoListSave");
                    container.CurrentListInstance.wim.DoListSave(container.Item.GetValueOrDefault(0), currentVersion.ID, isValidInput);
                    bool isIntroduction = false;

                    if (container.Item.GetValueOrDefault(0) == 0)
                    {
                        isIntroduction = true;
                        container.Item = container.CurrentListInstance.wim.AfterSaveElementIdentifier;
                    }

                    if (fieldList == null)
                    {
                        System.Diagnostics.Trace.WriteLine("ScanClass (versioning)");

                        fieldList = new List<Field>();
                        if (container.CurrentListInstance.wim.ShouldActAsDataStore)
                        {
                            ScanClass(container, build, true, false, ref isValidInput, fieldList);
                        }
                        else
                        {
                            ScanClass(container, build, true, true, ref isValidInput, fieldList);
                        }
                    }
                    if (fieldList != null)
                    {
                        Content content = new Content();
                        content.Fields = fieldList.ToArray();
                        string serialized = Content.GetSerialized(content);

                        ComponentListVersion version = new ComponentListVersion();
                        version.ComponentListID = listId;
                        version.SiteID = container.CurrentListInstance.wim.CurrentSite.ID;
                        version.ApplicationUserID = container.CurrentListInstance.wim.CurrentApplicationUser.ID;
                        version.ComponentListItemID = container.Item.GetValueOrDefault(0);
                        version.Serialized_XML = serialized;
                        version.TypeID = isIntroduction ? 0 : 1;
                        await version.SaveAsync();
                    }

                    // when a redirect is set, return with null.
                    if (container.CurrentListInstance.wim._IsRedirected)
                    {
                        build.IsTerminated = true;
                        return build;
                    }

                    var listurl = container.UrlBuild.GetListRequest(container.CurrentListInstance.wim.CurrentList);

                    #region Set Save Message

                    // [MR:27-07-2021] moved here from row 1778
                    // [MR:27-07-2021] When we aren't in this list because of a refer id, set a saved message
                    string refer = string.Empty;
                    if (container?.Context?.Request?.Query?.ContainsKey("referid") == true)
                    {
                        refer = container.Context.Request.Query["referid"];
                    }

                    if (string.IsNullOrWhiteSpace(refer))
                    {
                        string savedMessage = container.CurrentList.Label_Saved;
                        if (string.IsNullOrEmpty(savedMessage))
                        {
                            if (container.CurrentApplicationUser.Language == 2)
                            {
                                savedMessage = "De gegevens zijn opgeslagen.";
                            }
                            else
                            {
                                savedMessage = "The data has been saved.";
                            }
                        }

                        container.CurrentListInstance.wim.CurrentVisitor.Data.Apply("wim.note", savedMessage);
                        await container.CurrentListInstance.wim.CurrentVisitor.SaveAsync();
                    }
                    // [MR:27-07-2021] moved here from row 1778

                    #endregion Set Save Message

                    if (openInFrame == 0)
                    {
                        int folderID = Utility.ConvertToInt(container.Request.Query["folder"]);
                        string folderInfo = null;
                        if (folderID > 0)
                        {
                            folderInfo = string.Concat("folder=", folderID);
                        }

                        if (container.Form("saveNew") == "1" || container.IsPostBack("saveNew"))
                        {
                            if (container.CurrentListInstance.wim.CurrentFolder.Type == FolderType.Gallery)
                            {
                                container.Redirect(string.Concat(container.CurrentListInstance.wim.SearchResultItemPassthroughParameter, "=0&gallery=", container.CurrentListInstance.wim.CurrentFolder.ID, "&pitem=", container.Item), true);
                            }
                            else
                            {
                                container.Redirect(string.Concat(container.CurrentListInstance.wim.SearchResultItemPassthroughParameter, "=0&pitem=", container.Item), true);
                            }
                        }

                        if (container.CurrentListInstance.wim.CurrentFolder.Type == FolderType.Gallery)
                        {
                            if (container.Item.GetValueOrDefault(0) == 0)
                            {
                                container.Redirect(string.Concat(container.WimPagePath, "?gallery=", container.CurrentListInstance.wim.CurrentFolder.ID));
                            }
                            else
                            {
                                if (container.ItemType == RequestItemType.Asset)
                                {
                                    container.Redirect(string.Concat(container.WimPagePath, "?asset=", container.Item));
                                }
                            }
                        }
                        else if (container.CurrentListInstance is AppCentre.Data.Implementation.PageInstance)
                        {
                            if (container.Context.Request.Query["item"] == "0" && container.Item.GetValueOrDefault(0) > 0)
                            {
                                container.Redirect($"{container.WimPagePath}?page={container.Item.Value}");
                            }
                        }
                        else
                        {
                            // [MR:27-07-2021] FROM HERE

                            if (container.CurrentList.IsSingleInstance)
                            {
                                container.Redirect(listurl);
                            }

                            if (container.Item.GetValueOrDefault(0) == 0)
                            {
                                if (container.Group.HasValue)
                                {
                                    container.Redirect(string.Concat(listurl, $"?item=0&group={container.Group.Value}&groupitem={container.GroupItem.GetValueOrDefault(0)}&", folderInfo));
                                }
                                else
                                {
                                    if (folderInfo == null)
                                    {
                                        container.Redirect(listurl);
                                    }
                                    else
                                    {
                                        container.Redirect(string.Concat(listurl, "?", folderInfo));
                                    }
                                }
                            }
                            else
                            {
                                //  Redirect to list view
                                if (container.CurrentListInstance.wim.CurrentList.Data["wim_AfterSaveListView"].ParseBoolean())
                                {
                                    container.Redirect(listurl, true);
                                }
                                else if (!container.IsJson)
                                {
                                    container.Redirect(string.Concat(container.CurrentListInstance.wim.SearchResultItemPassthroughParameter, "=", container.Item), true);
                                }
                            }
                        }
                    }

                    if (string.IsNullOrEmpty(container.CurrentListInstance.wim.OnSaveScript))
                    {
                        if (container.Form("saveNew") == "1" || container.IsPostBack("saveNew"))
                        {
                            string url = container.CurrentListInstance.wim.GetUrl(new KeyValue() { Key = "item", Value = "0" }, new KeyValue() { Key = "pitem", Value = container.Item });
                            container.Redirect(url);
                        }
                        else
                        {
                            container.CurrentListInstance.wim.OnSaveScript = @"<input type=""hidden"" name=""close"" class=""postParent"">";
                        }
                    }
                }
                if (!container.CurrentListInstance.wim.HasListPreRender)
                {
                    ScanClass(container, build, true, false, ref isValidInput);
                }
            }
            else if (!container.CurrentListInstance.wim.HasListPreRender)
            {
                ScanClass(container, build, true, forceLoadEvent, ref isValidInput);
            }

            return build;
        }

        internal string m_ClickedButton;
        bool m_TriggerSaveEvent;

        /// <summary>
        /// Scans the class.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="build">The build.</param>
        /// <param name="writeOutput">if set to <c>true</c> [write output].</param>
        /// <param name="forceLoadEvent">if set to <c>true</c> [force load event].</param>
        /// <param name="isValidInput">if set to <c>true</c> [is valid input].</param>
        /// <returns></returns>
        bool ScanClass(Console container, WimControlBuilder build, bool writeOutput, bool forceLoadEvent, ref bool isValidInput)
        {
            return ScanClass(container, build, writeOutput, forceLoadEvent, ref isValidInput, null);
        }

        List<ListInfoItem> m_AllListProperties;
        public List<ListInfoItem> AllListProperties => m_AllListProperties;

        int m_PropertyListTypeID;

        /// <summary>
        /// Scans the class properties.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="senderInstance">The sender instance.</param>
        /// <param name="build">The build.</param>
        /// <param name="writeOutput">if set to <c>true</c> [write output].</param>
        /// <param name="forceLoadEvent">if set to <c>true</c> [force load event].</param>
        /// <param name="isValidInput">if set to <c>true</c> [is valid input].</param>
        /// <param name="fieldList">The field list.</param>
        /// <param name="build2">The build2.</param>
        /// <param name="infoList">The info list.</param>
        /// <param name="count">The count.</param>
        /// <param name="containerTitle">The container title.</param>
        /// <param name="isValidContainerInput">if set to <c>true</c> [is valid container input].</param>
        /// <param name="isContainerClosed">if set to <c>true</c> [is container closed].</param>
        /// <param name="skipHeader">if set to <c>true</c> [skip header].</param>
        /// <param name="previousContentType">Type of the previous content.</param>
        void ScanClassProperties(Console container, object senderInstance, ref WimControlBuilder build, bool writeOutput, bool forceLoadEvent, ref bool isValidInput, List<Field> fieldList, ref WimControlBuilder build2
            , System.Reflection.PropertyInfo[] infoList
            , ref int count
            , ref string containerTitle
            , ref bool isValidContainerInput
            , ref bool isContainerClosed
            , ref bool skipHeader
            , ref ContentType previousContentType
            )
        {
            if (infoList == null)
            {
                return;
            }

            Property[] properties = null;

            if (m_PropertyListTypeID != container.CurrentListInstance.wim.PropertyListTypeID)
            {
                m_AllListProperties = null;
            }

            int currentPropertyListID = container.CurrentListInstance.wim.CurrentList.ID;
            if (container.CurrentListInstance.wim.PropertyListOverrideID.HasValue)
            {
                currentPropertyListID = container.CurrentListInstance.wim.PropertyListOverrideID.Value;
            }

            if (container.View == 0)
            {
                m_PropertyListTypeID = container.CurrentListInstance.wim.PropertyListTypeID;

                if (container.CurrentListInstance.wim.PropertySubSelection != null)
                {
                    properties = Property.SelectAll(container.CurrentListInstance.wim.PropertySubSelection);
                }
                else if (container.CurrentListInstance.wim.PropertyListTypeID > 0)
                {
                    properties = Property.SelectAll(currentPropertyListID, container.CurrentListInstance.wim.PropertyListTypeID, true);
                }
                else
                {
                    properties = Property.SelectAll(currentPropertyListID);
                }

                if (container.CurrentListInstance.wim.PropertyListIgnoreList != null)
                {
                    List<Property> clean = new List<Property>();
                    foreach (var property in properties)
                    {
                        if (container.CurrentListInstance.wim.EditableItemList != null)
                        {
                            var search = (from item in container.CurrentListInstance.wim.EditableItemList where item.AssignedProperty == property.FieldName select item).DefaultIfEmpty().FirstOrDefault();
                            if (search != null)
                            {
                                property.IsOnlyRead = !search.State;
                            }
                        }

                        Property find = null;
                        foreach (var ignore in container.CurrentListInstance.wim.PropertyListIgnoreList)
                        {
                            if (ignore.ID == property.ID)
                            {
                                find = ignore;
                                break;
                            }
                        }
                        if (find == null)
                        {
                            clean.Add(property);
                        }
                    }
                    properties = clean.ToArray();
                }
            }

            System.Diagnostics.Trace.WriteLine($"Properties {(m_AllListProperties == null ? 0 : m_AllListProperties.Count)}");
            if (m_AllListProperties == null)
            {
                m_AllListProperties = new List<ListInfoItem>();
                SetAllProperties(container, senderInstance, infoList, ref properties, m_AllListProperties);

                System.Diagnostics.Trace.WriteLine($"Set properties {(m_AllListProperties == null ? 0 : m_AllListProperties.Count)}");

                List<ListInfoItem> sorted = new List<ListInfoItem>();

                //  For using extendedTop position with DataExtend types.
                bool extendedTopPosition = false;
                int extendedTopPositionIndex = 0;
                Type extendedType = null;

                foreach (ListInfoItem lii in m_AllListProperties)
                {
                    if (lii.ContentAttribute.ContentTypeSelection == ContentType.DataExtend)
                    {
                        extendedTopPositionIndex = 0;
                        extendedTopPosition = lii.HasTopPosition;
                        extendedType = lii.Info.PropertyType;
                    }

                    var hasTopPosition = lii.HasTopPosition;
                    var index = 0;

                    if (extendedType == lii.SenderInstance.GetType())
                    {
                        index = extendedTopPositionIndex;
                        hasTopPosition = extendedTopPosition;
                        extendedTopPositionIndex++;
                    }

                    if (hasTopPosition)
                    {
                        sorted.Insert(index, lii);
                    }
                    else
                    {
                        sorted.Add(lii);
                    }
                }
                m_AllListProperties = sorted;

                if (properties != null && properties.Length > 0)
                {
                    foreach (Property p in properties)
                    {
                        bool foundProperty = false;

                        if (!p.IsFixed && m_ContentContainerListInfoItem?.Info != null)
                        {
                            ListInfoItem lii = new ListInfoItem();
                            lii.ContentAttribute = m_ContentContainerListInfoItem.ContentAttribute;

                            if (p.IsPresentProperty && container.CurrentListInstanceItem != null)
                            {
                                foreach (System.Reflection.PropertyInfo info in container.CurrentListInstanceItem.GetType().GetProperties())
                                {
                                    if (info.Name == p.FieldName)
                                    {
                                        lii.Info = info;
                                        lii.SenderInstance = container.CurrentListInstanceItem;
                                        break;
                                    }
                                }
                            }

                            if (lii.Info == null)
                            {
                                lii.Info = m_ContentContainerListInfoItem.Info;
                                lii.SenderInstance = m_ContentContainerListInfoItem.SenderInstance;
                            }

                            lii.Property = p;
                            lii.IsVisible = m_ContentContainerListInfoItem.IsVisible;
                            lii.IsEditable = m_ContentContainerListInfoItem.IsEditable;

                            lii.IsRequired = container.CurrentListInstance.wim.IsRequired(lii.SenderInstance, container.CurrentListInstance, lii.Name, lii.IsRequired);
                            lii.IsEditable = container.CurrentListInstance.wim.IsEditable(lii.SenderInstance, container.CurrentListInstance, lii.Name, lii.IsEditable);
                            lii.IsVisible = container.CurrentListInstance.wim.IsVisible(lii.SenderInstance, container.CurrentListInstance, lii.Name, lii.IsVisible);

                            if (lii.Property != null && lii.Property.IsHidden)
                            {
                                lii.IsVisible = false;
                            }

                            if (p.IsOnlyRead)
                            {
                                lii.IsEditable = false;
                            }

                            foundProperty = true;

                            if (lii.HasTopPosition)
                            {
                                sorted.Insert(0, lii);
                            }
                            else
                            {
                                sorted.Add(lii);
                            }
                        }

                        if (!foundProperty && p.IsFixed)
                        {
                            p.Delete();
                        }
                    }
                }
            }
            else
            {
                if (senderInstance is IComponentListTemplate instance)
                {
                    if (instance.FormMaps != null && instance.FormMaps.Count > 0)
                    {
                        Map(container, instance.FormMaps.List, m_AllListProperties);
                    }
                }

                System.Diagnostics.Trace.WriteLine($"Validating properties");
                ValidateAllProperties(container, senderInstance);
                System.Diagnostics.Trace.WriteLine($"End validating properties");
            }

            if (container.CurrentListInstance.wim.m_infoList != null)
            {
                foreach (var custom in container.CurrentListInstance.wim.m_infoList)
                {
                    var found = from item in m_AllListProperties where item.Name == custom.Name select item;
                    if (!found.Any())
                    {
                        m_AllListProperties.Add(custom);
                    }
                }
            }

            if (container.CurrentListInstance.wim.AutoExpression)
            {
                container.HasDoubleCols = true;
            }
            else
            {
                foreach (ListInfoItem infoItem in m_AllListProperties)
                {
                    if (infoItem.ContentAttribute != null)
                    {
                        if (infoItem.ContentAttribute.Expression == OutputExpression.Alternating || infoItem.ContentAttribute.Expression == OutputExpression.Left || infoItem.ContentAttribute.Expression == OutputExpression.Right)
                        {
                            container.HasDoubleCols = true;
                            break;
                        }
                        if (infoItem.Property != null && infoItem.Property.IsShort)
                        {
                            container.HasDoubleCols = true;
                            break;
                        }
                    }
                }
            }

            m_IdValue = new Hashtable();

            if (!string.IsNullOrEmpty(container.CurrentListInstance.wim.Form.TranslationSectionStart))
            {
                container.CurrentListInstance.wim.Form.m_ShowTranslationDataStart = true;
            }

            container.CurrentListInstance.wim.Form.ListItemElementList = m_AllListProperties;

            var sorted1 = (from item in m_AllListProperties where item.ContentAttribute != null && item.ContentAttribute.ContentTypeSelection != ContentType.DataList select item).ToList();
            var sorted2 = (from item in m_AllListProperties where item.ContentAttribute != null && item.ContentAttribute.ContentTypeSelection == ContentType.DataList select item).ToList();

            if (sorted2.Count > 0)
            {
                m_AllListProperties.Clear();
                m_AllListProperties.AddRange(sorted1);
                m_AllListProperties.AddRange(sorted2);
            }

            System.Diagnostics.Trace.WriteLine($"Paging properties");

            //foreach (ListInfoItem infoItem in m_AllListProperties)
            for (int index = 0; index < m_AllListProperties.Count; index++)
            {
                ListInfoItem infoItem = m_AllListProperties[index];
                count++;

                if (!infoItem.IsVisible)
                {
                    continue;
                }

                if (m_IgnoreDataList && infoItem.ContentAttribute.ContentTypeSelection == ContentType.DataList)
                {
                    continue;
                }

                if (infoItem.Info == null || !infoItem.Info.PropertyType.Equals(typeof(CustomData)))
                {
                    infoItem.IsVisible = container.CurrentListInstance.wim.IsVisible(infoItem.SenderInstance, container.CurrentListInstance, infoItem.Name, infoItem.IsVisible);
                    if (!infoItem.IsVisible)
                    {
                        continue;
                    }
                    infoItem.IsRequired = container.CurrentListInstance.wim.IsRequired(infoItem.SenderInstance, container.CurrentListInstance, infoItem.Name, infoItem.IsRequired);
                    infoItem.IsEditable = container.CurrentListInstance.wim.IsEditable(infoItem.SenderInstance, container.CurrentListInstance, infoItem.Name, infoItem.IsEditable);
                }

                ApplyContentInfoItem(container, infoItem, ref build, writeOutput, forceLoadEvent, ref isValidInput, fieldList, ref build2, ref count, ref containerTitle, ref isValidContainerInput, ref isContainerClosed, ref skipHeader, ref previousContentType);
            }
            System.Diagnostics.Trace.WriteLine($"End paging properties");
        }

        Hashtable m_IdValue;

        public class ListInfoItem
        {
            public Field InputField { get; set; }

            public bool? IsValid { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public IContentInfo ContentAttribute;
            /// <summary>
            /// 
            /// </summary>
            System.Reflection.PropertyInfo m_Info;
            /// <summary>
            /// Gets or sets the info.
            /// </summary>
            /// <value>The info.</value>
            public System.Reflection.PropertyInfo Info
            {
                get { return m_Info; }
                set
                {
                    m_Info = value;
                    if (m_Info != null)
                    {
                        Name = m_Info.Name;
                    }
                }
            }

            /// <summary>
            /// Gets the value.
            /// </summary>
            /// <returns></returns>
            public object GetValue()
            {
                return Info.GetValue(SenderInstance, null);
            }

            public string VirtualPropertyValue { get; set; }
            public bool IsVirtualProperty { get; set; }

            Property m_Property;
            public Property Property
            {
                set
                {
                    m_Property = value;

                    if (IsVirtualProperty)
                    {
                        return;
                    }

                    ContentAttribute.Title = value.Title;
                    var meta = Utility.GetDeserialized(typeof(MetaData), value.Data) as MetaData;
                    Utility.ReflectProperty(meta, value);

                    ContentAttribute.InteractiveHelp = value.InteractiveHelp;
                }
                get { return m_Property; }
            }

            /// <summary>
            /// Gets or sets a value indicating whether this instance is visible.
            /// </summary>
            /// <value>
            /// 	<c>true</c> if this instance is visible; otherwise, <c>false</c>.
            /// </value>
            public bool IsVisible { get; set; }

            /// <summary>
            /// Gets or sets the name.
            /// </summary>
            /// <value>The name.</value>
            public string Name { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether this instance is editable.
            /// </summary>
            /// <value>
            /// 	<c>true</c> if this instance is editable; otherwise, <c>false</c>.
            /// </value>
            public bool IsEditable { get; set; }
            /// <summary>
            /// Gets or sets a value indicating whether this instance is required.
            /// </summary>
            /// <value>
            /// 	<c>true</c> if this instance is required; otherwise, <c>false</c>.
            /// </value>
            public bool IsRequired { get; set; }
            /// <summary>
            /// Gets or sets the sender instance.
            /// </summary>
            /// <value>The sender instance.</value>
            public object SenderInstance { get; set; }
            public object SenderSponsorInstance { get; set; }
            /// <summary>
            /// Gets or sets a value indicating whether this instance has top position.
            /// </summary>
            /// <value>
            /// 	<c>true</c> if this instance has top position; otherwise, <c>false</c>.
            /// </value>
            public bool HasTopPosition { get; set; }
            /// <summary>
            /// Gets or sets a value indicating whether this instance is cloaked.
            /// </summary>
            /// <value>
            /// 	<c>true</c> if this instance is cloaked; otherwise, <c>false</c>.
            /// </value>
            public bool IsCloaked { get; set; }

            public override string ToString()
            {
                if (IsValid.HasValue)
                {
                    return $"{Name} (Valid:{IsValid.Value}, Cloaked:{IsCloaked}, Visible:{IsVisible})";
                }

                return $"{Name}";
            }
        }

        ListInfoItem m_ContentContainerListInfoItem;

        void ValidateAllProperties(Console container, object senderInstance)
        {
            if (senderInstance is IComponentListTemplate instance)
            {
                if (instance.FormMaps != null && instance.FormMaps.Count > 0)
                {
                    IterateMaps(instance.FormMaps.List);
                }
            }
        }

        void IterateMaps(List<IFormMap> maps)
        {
            foreach (var map in maps)
            {
                map.Evaluate();
                ParseMap(map);
            }
        }
        void ParseMap(IFormMap map)
        {
            if (map.Elements != null && map.Elements.Count > 0)
            {
                foreach (var contentItem in map.Elements)
                {
                    if (contentItem.Property != null && map.SenderInstance != null)
                    {
                        foreach (var infoItem in m_AllListProperties)
                        {
                            if (infoItem?.Info?.Equals(contentItem.Property)  == true )//&& infoItem?.Info?.PropertyType?.Equals(typeof(CustomData)) == true)
                            {
                                infoItem.IsVisible = map.IsHidden.HasValue ? !map.IsHidden.Value : !contentItem.IsHidden;
                                infoItem.IsEditable = map.IsReadOnly.HasValue ? !map.IsReadOnly.Value : !contentItem.IsReadOnly;
                                infoItem.IsCloaked = map.IsCloacked.HasValue ? map.IsCloacked.Value : contentItem.IsCloaked;

                                infoItem.IsRequired = contentItem.Mandatory;
                            }
                        }
                    }
                }
            }
        }

        Dictionary<string, int> _mapdata;

        void Map(Console container, List<IFormMap> maps, List<ListInfoItem> all, int depth = 0)
        {
            //  Reset
            if (depth == 0)
            {
                _mapdata = new Dictionary<string, int>();
            }

            foreach (var map in maps)
            {
                if (map.Elements != null && map.Elements.Count > 0)
                {
                    var key = depth == 0 ? $"{map.GetType().Name}_" : $"{map.GetType().Name}{depth}_";
                    var index = 0;
                    if (_mapdata.ContainsKey(key))
                    {
                        index = _mapdata[key] + 1;
                        _mapdata[key] = index;
                    }
                    else
                    {
                        _mapdata.Add(key, 1);
                    }

                    foreach (var contentItem in map.Elements)
                    {
                        if (contentItem.Property != null && map.SenderInstance != null)
                        {
                            contentItem.InfoItem.IsVisible = map.IsHidden.HasValue ? map.IsHidden.Value : !contentItem.IsHidden;
                            contentItem.InfoItem.IsCloaked = map.IsCloacked.HasValue ? map.IsCloacked.Value : contentItem.IsCloaked;
                            contentItem.InfoItem.IsEditable = map.IsReadOnly.HasValue ? !map.IsReadOnly.Value : !contentItem.IsReadOnly;
                            contentItem.InfoItem.IsRequired = contentItem.Mandatory;

                            if (!all.Contains(contentItem.InfoItem))
                            {
                                all.Add(contentItem.InfoItem);
                            }
                        }
                    }
                    if (map.FormMaps != null && map.FormMaps.Count > 0)
                    {
                        Map(container, map.FormMaps, all, depth + 1);
                    }
                }
            }
        }

        /// <summary>
        /// Gets all properties (als the extended properties.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="senderInstance">The sender instance.</param>
        /// <param name="infoList">The info list.</param>
        /// <param name="properties">The properties.</param>
        /// <param name="all">All.</param>
        void SetAllProperties(Console container, object senderInstance, System.Reflection.PropertyInfo[] infoList, ref Property[] properties, List<ListInfoItem> all)
        {
            if (senderInstance is IComponentListTemplate instance)
            {
                if (instance.FormMaps != null && instance.FormMaps.Count > 0)
                {
                    Map(container, instance.FormMaps.List, all);
                }
            }

            foreach (System.Reflection.PropertyInfo info in infoList)
            {
                bool isEditable = container.CurrentListInstance.wim.IsEditMode;
                var contentitems = GetContentInfo(info, container, senderInstance, ref isEditable, out bool isVisible, out bool isRequired);

                foreach (IContentInfo contentAttribute in contentitems)
                {
                    if (contentAttribute == null)
                    {
                        continue;
                    }

                    ListInfoItem x = new ListInfoItem();
                    x.ContentAttribute = contentAttribute;
                    x.Info = info;
                    x.IsVisible = isVisible;
                    x.IsEditable = isEditable;
                    x.IsRequired = isRequired;
                    x.HasTopPosition = (info.GetCustomAttributes(typeof(TopListPosition), true).Length > 0);
                    x.SenderInstance = senderInstance;

                    if (!isVisible
                        && contentAttribute.ContentTypeSelection != ContentType.DataExtend
                        && contentAttribute.ContentTypeSelection != ContentType.DataList
                        && properties != null)
                    {
                        foreach (Property p in properties)
                        {
                            if (p.FieldName == info.Name)
                            {
                                x.Property = p;

                                if (x.Property != null && x.Property.IsHidden)
                                {
                                    x.IsVisible = false;
                                }

                                break;
                            }
                        }
                    }

                    all.Add(x);

                    if (contentAttribute.ContentTypeSelection == ContentType.DataExtend)
                    {
                        if (!isVisible)
                        {
                            continue;
                        }

                        object sender = info.GetValue(senderInstance, null);

                        if (sender == null)
                        {
                            sender = Activator.CreateInstance(info.PropertyType);
                        }

                        if (sender is ComponentListTemplate componentlist)
                        {
                            componentlist.wim.Console = container;
                        }


                        ((DataExtendAttribute)contentAttribute).m_Implement = sender;
                        if (((DataExtendAttribute)contentAttribute).m_Implement != null)
                        {
                            SetAllProperties(container, sender, ((DataExtendAttribute)contentAttribute).m_Implement.GetType().GetProperties(), ref properties, all);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Applies the content info item.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="infoItem">The info item.</param>
        /// <param name="build">The build.</param>
        /// <param name="writeOutput">if set to <c>true</c> [write output].</param>
        /// <param name="forceLoadEvent">if set to <c>true</c> [force load event].</param>
        /// <param name="isValidInput">if set to <c>true</c> [is valid input].</param>
        /// <param name="fieldList">The field list.</param>
        /// <param name="build2">The build2.</param>
        /// <param name="count">The count.</param>
        /// <param name="containerTitle">The container title.</param>
        /// <param name="isValidContainerInput">if set to <c>true</c> [is valid container input].</param>
        /// <param name="isContainerClosed">if set to <c>true</c> [is container closed].</param>
        /// <param name="skipHeader">if set to <c>true</c> [skip header].</param>
        /// <param name="previousContentType">Type of the previous content.</param>
        void ApplyContentInfoItem(Console container, ListInfoItem infoItem, ref WimControlBuilder build, bool writeOutput, bool forceLoadEvent, ref bool isValidInput, List<Field> fieldList, ref WimControlBuilder build2
            , ref int count
            , ref string containerTitle
            , ref bool isValidContainerInput
            , ref bool isContainerClosed
            , ref bool skipHeader
            , ref ContentType previousContentType
            )
        {
            ((ContentSharedAttribute)infoItem.ContentAttribute).Console = container;

            #region Expression

            bool skipExpression = false;

            if (writeOutput)
            {
                if (container.HasDoubleCols)
                {
                    if (infoItem.ContentAttribute.ContentTypeSelection == ContentType.Button)
                    {
                        skipExpression = !((ButtonAttribute)infoItem.ContentAttribute).m_IsFormElement;
                    }
                    else if (infoItem.ContentAttribute.ContentTypeSelection == ContentType.FileUpload && !container.CurrentListInstance.IsEditMode)
                    {
                        skipExpression = true;
                    }
                    else if (infoItem.ContentAttribute.ContentTypeSelection == ContentType.DataExtend)
                    {
                        skipExpression = true;
                    }
                    else if (infoItem.ContentAttribute.ContentTypeSelection == ContentType.DataList)
                    {
                        skipExpression = true;
                        if (writeOutput)
                        {
                            CreateFooter(container, build2);
                        }
                    }

                    if (container.CurrentListInstance.wim.AutoExpression && infoItem.ContentAttribute.Expression == OutputExpression.FullWidth)
                    {
                        infoItem.ContentAttribute.Expression = OutputExpression.Alternating;
                    }

                    if (!skipExpression && infoItem.ContentAttribute.CanHaveExpression)
                    {

                        if (infoItem.ContentAttribute.Expression == OutputExpression.Alternating)
                        {
                            if (container.ExpressionPrevious == OutputExpression.FullWidth)
                            {
                                infoItem.ContentAttribute.Expression = OutputExpression.Left;
                            }
                            else if (container.ExpressionPrevious == OutputExpression.Left)
                            {
                                infoItem.ContentAttribute.Expression = OutputExpression.Right;
                            }
                            else if (container.ExpressionPrevious == OutputExpression.Right)
                            {
                                infoItem.ContentAttribute.Expression = OutputExpression.Left;
                            }
                        }
                    }
                    else
                    {
                        infoItem.ContentAttribute.Expression = OutputExpression.FullWidth;
                    }
                }
                else
                {
                    infoItem.ContentAttribute.Expression = OutputExpression.FullWidth;

                    if (m_IsNewDesign && infoItem.ContentAttribute.ContentTypeSelection == ContentType.DataList && writeOutput)
                    {
                        CreateFooter(container, build2);
                    }
                }
            }

            #endregion

            Field inputField = null;

            if (infoItem.IsVirtualProperty)
            {
                inputField = new Field(infoItem.Name, infoItem.ContentAttribute.ContentTypeSelection, null);
                inputField.PropertyInfo = infoItem.Property;
                inputField.Property = infoItem.Name;
            }

            #region ContentContainer

            if (infoItem.ContentAttribute.ContentTypeSelection == ContentType.ContentContainer)
            {
                System.Diagnostics.Trace.WriteLine($"ContentContainer: {infoItem.Name}");

                if (infoItem.Property == null || infoItem.Property.Data == null)
                {
                    return;
                }

                //  BEWARE!!!!
                infoItem.ContentAttribute = null;

                if (infoItem.ContentAttribute == null)
                {
                    var meta = Utility.GetDeserialized(typeof(MetaData), infoItem.Property.Data) as MetaData;

                    infoItem.ContentAttribute = meta.GetContentInfo();
                    infoItem.ContentAttribute.SenderInstance = infoItem.SenderInstance;
                }
                ((ContentSharedAttribute)infoItem.ContentAttribute).Console = container;

                inputField = new Field(infoItem.Property.FieldName, infoItem.Property.ContentTypeID, null);
                inputField.PropertyInfo = infoItem.Property;

                if (infoItem.Property.IsShort)
                {
                    if (container.ExpressionPrevious == OutputExpression.FullWidth)
                    {
                        infoItem.ContentAttribute.Expression = OutputExpression.Left;
                    }
                    else if (container.ExpressionPrevious == OutputExpression.Left)
                    {
                        infoItem.ContentAttribute.Expression = OutputExpression.Right;
                    }
                    else if (container.ExpressionPrevious == OutputExpression.Right)
                    {
                        infoItem.ContentAttribute.Expression = OutputExpression.Left;
                    }
                }
                else
                {
                    infoItem.ContentAttribute.Expression = OutputExpression.FullWidth;
                }
            }

            #endregion

            #region Section

            if (infoItem.ContentAttribute.ContentTypeSelection == ContentType.Section || (infoItem.ContentAttribute.ContentTypeSelection == ContentType.TextLine && infoItem.ContentAttribute.Title == null))
            {
                //  Subtitle
                if (writeOutput)
                {
                    if (container.CurrentListInstance.wim.IsSaveMode)
                    {
                        isContainerClosed = isValidContainerInput;
                        isValidContainerInput = true;
                    }

                    if (m_IsNewDesign)
                    {
                        if (!infoItem.IsCloaked && infoItem.ContentAttribute.ContentTypeSelection == ContentType.Section)
                        {
                            containerTitle = infoItem.ContentAttribute.OutputText;
                            if (string.IsNullOrEmpty(containerTitle))
                            {
                                //  Legacy
                                object value = infoItem.Info.GetValue(infoItem.SenderInstance, null);
                                if (value is string)
                                {
                                    containerTitle = value.ToString();
                                }
                                else if (infoItem.Property != null)
                                {
                                    containerTitle = infoItem.Property.Title;
                                }
                            }

                            CreateBlock(container, containerTitle, build, build2, null, false, false);

                            build2 = new WimControlBuilder();
                        }
                    }
                    else
                    {
                        CreateBlock(container, containerTitle, build, build2, null, isContainerClosed, skipHeader);
                        skipHeader = false;
                        build2 = new WimControlBuilder();

                        container.CurrentListInstance.wim.Form.TranslationLastRequest = infoItem.Property == null ? infoItem.Name : infoItem.Property.FieldName;

                        object value = infoItem.Info.GetValue(infoItem.SenderInstance, null);
                        if (value is string)
                        {
                            containerTitle = value.ToString();
                        }
                        else if (infoItem.Property != null)
                        {
                            containerTitle = infoItem.Property.Title;
                        }

                        if (infoItem.ContentAttribute.ContentTypeSelection == ContentType.Section)
                        {
                            if (infoItem.ContentAttribute is SectionAttribute attribute)
                            {
                                isContainerClosed = attribute.m_IsClosedContainer;
                                string isClosedStateReferringProperty = attribute.m_IsClosedStateReferringProperty;
                                if (!string.IsNullOrEmpty(isClosedStateReferringProperty))
                                {
                                    isContainerClosed = attribute.IsClosedStateReferringProperty;
                                }
                            }
                        }
                        else
                        {
                            isContainerClosed = ((Framework.ContentInfoItem.TextLineAttribute)infoItem.ContentAttribute).m_IsClosedContainer;
                        }
                    }

                }
            }


            #endregion

            else

            #region Content

            {
                infoItem.ContentAttribute.SenderInstance = infoItem.SenderInstance;
                infoItem.ContentAttribute.SenderSponsorInstance = infoItem.SenderSponsorInstance;
                infoItem.ContentAttribute.Property = infoItem.Info;

                string ID;

                //  If ContentContainer then set property
                if (infoItem.ContentAttribute.ID != null)
                {
                    ID = infoItem.ContentAttribute.ID;
                }
                else if (inputField != null)
                {
                    ID = string.Concat("data_", inputField.Property);
                }
                else
                {
                    ID = infoItem.Name;
                }

                System.Diagnostics.Trace.WriteLine($"ID: {ID} ({infoItem.ContentAttribute.GetType().ToString()})");

                string root = ID;
                int add = 0;
                while (m_IdValue.Contains(ID))
                {
                    add++;
                    ID = string.Concat(root, add);
                }
                m_IdValue.Add(ID, null);

                infoItem.ContentAttribute.ID = ID;
                infoItem.ContentAttribute.ForceLoadEvent = forceLoadEvent;

                //  Set the data candidate
                if (infoItem.Property != null && infoItem.Property.Data != null)
                {
                    var meta = Utility.GetDeserialized(typeof(MetaData), infoItem.Property.Data) as MetaData;
                    if (meta.CollectionList != null)
                    {
                        infoItem.ContentAttribute.ApplyMetaDataList(meta.CollectionList);
                    }
                }

                infoItem.ContentAttribute.ShowInheritedData = container.CurrentListInstance.wim.Form.ShowTranslationData;

                if (inputField == null)
                {
                    if (infoItem.ContentAttribute.Property != null
                        && infoItem.ContentAttribute.Property.PropertyType == typeof(CustomData))
                    {
                        //  reset to already existing inputField, possible add this to all, but no time to test.
                        inputField = infoItem.InputField;

                        if (inputField == null)
                        {
                            inputField = new Field() { Property = infoItem.ContentAttribute.ID };
                        }
                        else
                        {
                            infoItem.ContentAttribute.ForceLoadEvent = !container.IsPostBackSave;
                        }
                    }
                    else
                    {
                        inputField = new Field();
                    }
                }

                if (inputField != null)
                {
                    inputField.InheritedValue = container.CurrentListInstance.wim.Form.GetTranslationMaster(infoItem.Property == null ? infoItem.Name : infoItem.Property.FieldName);
                    infoItem.InputField = inputField;
                    infoItem.ContentAttribute.SetCandidate(inputField, infoItem.IsEditable);
                }

                //  Validate input
                if (infoItem.IsEditable && !infoItem.ContentAttribute.IsValid(infoItem.IsRequired))
                {
                    isValidInput = false;
                    isValidContainerInput = false;
                    infoItem.IsValid = false;
                }
                else
                {
                    infoItem.IsValid = true;
                }

                //  Output the HTML
                if (writeOutput)
                {
                    if (m_IsNewDesign && previousContentType != ContentType.Undefined && infoItem.ContentAttribute.ContentTypeSelection == ContentType.DataList)
                    {
                        if (!build2.ToString().EndsWith("</table>"))
                        {
                            build2.Append("</table>");
                        }
                        skipHeader = false;
                    }

                    if (infoItem.IsCloaked)
                    {
                        skipExpression = true;
                    }

                    Field field = infoItem.ContentAttribute.WriteCandidate(build2, infoItem.IsEditable, infoItem.IsRequired, infoItem.IsCloaked);

                    if (fieldList != null && field != null)
                    {
                        fieldList.Add(field);
                    }

                    if (infoItem.ContentAttribute.ContentTypeSelection == ContentType.Button)
                    {
                        if (m_ButtonList == null)
                        {
                            m_ButtonList = new List<ButtonAttribute>();
                        }

                        bool shouldAdd = false;
                        if (container.Item.HasValue || container.CurrentListInstance.wim.CanContainSingleInstancePerDefinedList)
                        {
                            shouldAdd = infoItem.ContentAttribute.GetType().Equals(typeof(ButtonAttribute));
                        }
                        else
                        {
                            shouldAdd = infoItem.ContentAttribute.GetType().Equals(typeof(Framework.ContentListSearchItem.ButtonAttribute));
                        }

                        if (shouldAdd)
                        {
                            var attrib = (ButtonAttribute)infoItem.ContentAttribute;
                            var exist = (from item in m_ButtonList where item.ID == attrib.ID select item).FirstOrDefault();
                            if (exist == null)
                            {
                                m_ButtonList.Add(attrib);
                            }

                            //if (container.View > 0)
                            //{
                            //    var attrib = (ButtonAttribute)infoItem.ContentAttribute;
                            //    var exist = (from item in m_ButtonList where item.ID == attrib.ID select item).FirstOrDefault();
                            //    if (exist == null)
                            //        m_ButtonList.Add(attrib);
                            //}
                            //else
                            //{
                            //    var attrib = (ButtonAttribute)infoItem.ContentAttribute;
                            //    var exist = (from item in m_ButtonList where item.ID == attrib.ID select item).FirstOrDefault();
                            //    if (exist == null)
                            //        m_ButtonList.Add(attrib);
                            //}
                        }
                    }
                }

                if (infoItem.ContentAttribute.ContentTypeSelection == ContentType.Button && ((ButtonAttribute)infoItem.ContentAttribute).IsClicked)
                {
                    m_ClickedButton = infoItem.Info.Name;
                    m_TriggerSaveEvent = ((ButtonAttribute)infoItem.ContentAttribute).TriggerSaveEvent;
                }
            }
            #endregion

            //  Setting the previous type for dependencies (like for specialtypes)
            previousContentType = infoItem.ContentAttribute.ContentTypeSelection;

            if (writeOutput && !skipExpression)
            {
                container.ExpressionPrevious = infoItem.ContentAttribute.Expression;
            }
        }

        /// <summary>
        /// Gets the content info.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="container">The container.</param>
        /// <param name="senderInstance">The sender instance.</param>
        /// <param name="isEditable">if set to <c>true</c> [is editable].</param>
        /// <param name="isVisible">if set to <c>true</c> [is visible].</param>
        /// <param name="isRequired">if set to <c>true</c> [is required].</param>
        /// <returns></returns>
        IContentInfo[] GetContentInfo(System.Reflection.PropertyInfo info, Console container, object senderInstance, ref bool isEditable, out bool isVisible, out bool isRequired)
        {
            isVisible = true;
            isRequired = false;
            bool? isRequiredOverride = null;
            List<IContentInfo> arr = new List<IContentInfo>();
            IContentInfo contentAttribute = null;

            foreach (object attribute in info.GetCustomAttributes(false))
            {
                if (container.View == (int)ContainerView.ItemSelect && attribute is IListContentInfo)
                {
                    contentAttribute = attribute as IContentInfo;
                    arr.Add(contentAttribute);
                }
                else if ((container.View == (int)ContainerView.PopupLayerRequest || container.View == (int)ContainerView.BrowsingRequest || container.View == (int)ContainerView.DashboardRequest) && attribute is IListSearchContentInfo)
                {
                    contentAttribute = attribute as IContentInfo;
                    arr.Add(contentAttribute);
                }
                else if (container.View == (int)ContainerView.ListSettingRequest && attribute is IContentSettingInfo)
                {
                    contentAttribute = attribute as IContentInfo;
                    arr.Add(contentAttribute);
                }

                if (attribute is OnlyEditableWhenTrue onlyEditableWhenTrue && isEditable)
                {
                    bool check = (bool)GetProperty(container, senderInstance, onlyEditableWhenTrue.Property);

                    if (check && !onlyEditableWhenTrue.State)
                    {
                        isEditable = false;
                    }
                    if (!check && onlyEditableWhenTrue.State)
                    {
                        isEditable = false;
                    }
                }
                else if (attribute is OnlyEditableWhenFalse onlyEditableWhenFalse && isEditable)
                {
                    bool check = (bool)GetProperty(container, senderInstance, onlyEditableWhenFalse.Property);

                    //check = true && state = true
                    if (check && onlyEditableWhenFalse.State)
                    {
                        isEditable = false;
                    }

                    //check = false && state = false
                    if (!check && !onlyEditableWhenFalse.State)
                    {
                        isEditable = false;
                    }
                }
                if (attribute is OnlyVisibleWhenTrue onlyVisibleWhenTrue)
                {
                    bool check = (bool)GetProperty(container, senderInstance, onlyVisibleWhenTrue.Property);

                    //check = true && state = false
                    if (check && !onlyVisibleWhenTrue.State)
                    {
                        isVisible = false;
                    }

                    //check = false && state = true
                    if (!check && onlyVisibleWhenTrue.State)
                    {
                        isVisible = false;
                    }
                }
                else if (attribute is OnlyVisibleWhenFalse onlyVisibleWhenFalse)
                {
                    bool check = (bool)GetProperty(container, senderInstance, onlyVisibleWhenFalse.Property);

                    //check = true && state = true
                    if (check && onlyVisibleWhenFalse.State)
                    {
                        isVisible = false;
                    }

                    //check = false && state = false
                    if (!check && !onlyVisibleWhenFalse.State)
                    {
                        isVisible = false;
                    }
                }
                if (attribute is OnlyRequiredWhenTrue onlyRequiredWhenTrue)
                {
                    bool check = (bool)GetProperty(container, senderInstance, onlyRequiredWhenTrue.Property);

                    //check = true && state = false
                    if (check && !onlyRequiredWhenTrue.State)
                    {
                        isRequiredOverride = false;
                    }

                    //check = false && state = true
                    if (!check && onlyRequiredWhenTrue.State)
                    {
                        isRequiredOverride = false;
                    }
                }
                else if (attribute is OnlyRequiredWhenFalse onlyRequiredWhenFalse)
                {
                    bool check = (bool)GetProperty(container, senderInstance, onlyRequiredWhenFalse.Property);

                    //check = true && state = true
                    if (check && onlyRequiredWhenFalse.State)
                    {
                        isRequiredOverride = false;
                    }

                    //check = false && state = false
                    if (!check && !onlyRequiredWhenFalse.State)
                    {
                        isRequiredOverride = false;
                    }
                }
            }

            if (contentAttribute != null)
            {
                if (isRequiredOverride.HasValue)
                {
                    isRequired = isRequiredOverride.Value;
                }
                else
                {
                    isRequired = contentAttribute.Mandatory;
                }
            }
            return arr.ToArray();
        }

        bool m_ShowSearchButton;

        /// <summary>
        /// Scans the class.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="build">The build.</param>
        /// <param name="writeOutput">if set to <c>true</c> [write output].</param>
        /// <param name="forceLoadEvent">if set to <c>true</c> [force load event].</param>
        /// <param name="isValidInput">if set to <c>true</c> [is valid input].</param>
        /// <param name="fieldList">The field list.</param>
        /// <returns></returns>
        bool ScanClass(Console container, WimControlBuilder build, bool writeOutput, bool forceLoadEvent, ref bool isValidInput, List<Field> fieldList)
        {
            System.Diagnostics.Trace.WriteLine("Scanning component list");

            WimControlBuilder build2 = new WimControlBuilder();

            int count = 0;

            string containerTitle = string.IsNullOrEmpty(container.CurrentListInstance.wim.FirstSectionTitle) ? container.CurrentList.SingleItemName : container.CurrentListInstance.wim.FirstSectionTitle;
            bool isValidContainerInput = true;
            bool isContainerClosed = false;
            bool skipHeader = false;

            ContentType previousContentType = ContentType.Undefined;

            ScanClassProperties(container, 
                container.CurrentListInstance, 
                ref build, 
                writeOutput, 
                forceLoadEvent, 
                ref isValidInput, 
                fieldList, 
                ref build2, 
                container.CurrentListInstance.GetType().GetProperties(),
                ref count, 
                ref containerTitle, 
                ref isValidContainerInput, 
                ref isContainerClosed, 
                ref skipHeader, 
                ref previousContentType);

            if (writeOutput)
            {
                if (container.CurrentListInstance.wim.IsSaveMode)
                {
                    isContainerClosed = isValidContainerInput;
                }

                if (build2.Length > 0)
                {
                    CreateBlock(container, containerTitle, build, build2, null, true, true);
                    skipHeader = false;
                }
            }

            System.Diagnostics.Trace.WriteLine("Scanning done");
            if (count == 0)
            {
                return false;
            }

            return true;
        }
    }
}
