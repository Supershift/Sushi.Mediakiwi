using Sushi.Mediakiwi.Data;
using Sushi.Mediakiwi.Framework;
using Sushi.Mediakiwi.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.AppCentre.UI.Forms
{
    public class SharedFieldFormMap : FormMap<SharedFieldFormMap>
    {
        public SharedFieldTranslation ImplementTranslation { get; set; }
        public SharedField Implement { get; set; }
        public ComponentTemplate Template { get; set; }

        private System.Globalization.CultureInfo dateCulture = new System.Globalization.CultureInfo("nl-NL");

        #region Save - Dependent on ContentType

        public async Task SaveAsync(bool andPublish = false)
        {
            switch (ImplementTranslation.ContentTypeID)
            {
                case ContentType.FileUpload:
                case ContentType.DocumentSelect:
                case ContentType.Binary_Image:
                case ContentType.Binary_Document:
                case ContentType.FolderSelect:
                case ContentType.Hyperlink:
                case ContentType.PageSelect:
                    {
                        ImplementTranslation.EditValue = EditValueInt.ToString();
                    }
                    break;
                case ContentType.Choice_Checkbox:
                    {
                        ImplementTranslation.EditValue = EditValueBool ? "1" : "0";
                    }
                    break;
                case ContentType.Date:
                    {
                        ImplementTranslation.EditValue = EditValueDateTime.ToString("dd-MM-yyyy");
                    }
                    break;
                case ContentType.DateTime:
                    {
                        ImplementTranslation.EditValue = EditValueDateTime.ToString("dd-MM-yyyy HH:mm:ss");
                    }
                    break;
                case ContentType.MultiField:
                    {
                        ImplementTranslation.EditValue = EditValueMulti;
                    }
                    break;
                case ContentType.SubListSelect:
                    {
                        ImplementTranslation.EditValue = null;
                        if (EditValueSubList != null)
                        {
                            ImplementTranslation.EditValue = EditValueSubList.Serialized;
                        }
                    }
                    break;
                case ContentType.RichText:
                case ContentType.Sourcecode:
                case ContentType.TextArea:
                case ContentType.Choice_Dropdown:
                case ContentType.ListItemSelect:
                case ContentType.TextField:
                    {
                        ImplementTranslation.EditValue = EditValueString;
                    }
                    break;
                case ContentType.Undefined:
                case ContentType.TextLine:
                case ContentType.TextDate:
                case ContentType.Section:
                case ContentType.MultiImageSelect:
                case ContentType.MultiAssetUpload:
                case ContentType.Choice_Radio:
                case ContentType.HtmlContainer:
                    break;
            }

            if (andPublish)
            {
                ImplementTranslation.Value = ImplementTranslation.EditValue;
            }

            await ImplementTranslation.SaveAsync().ConfigureAwait(false);

            Implement.IsHiddenOnPage = IsHiddenOnPage;
            await Implement.SaveAsync();
        }

        #endregion Save - Dependent on ContentType

        private ListItemCollection m_EditValueOptions;

        public ListItemCollection EditValueOptions
        {
            get 
            {
                if (m_EditValueOptions == null)
                {
                    m_EditValueOptions = new ListItemCollection();
                }
                return m_EditValueOptions; 
            }
        }

        #region Load - Dependent on ContentType

        public SharedFieldFormMap(WimComponentListRoot wim, SharedField implement, SharedFieldTranslation implementTranslation, int componentTemplateId)
        {
            ImplementTranslation = implementTranslation;
            Implement = implement;
            if (componentTemplateId > 0)
            {
                Template = ComponentTemplate.SelectOne(componentTemplateId);
            }

            FieldName = ImplementTranslation.FieldName;
            PublishedValue = "-";
            IsHiddenOnPage = Implement.IsHiddenOnPage;

            bool canEdit = true;
            bool canDelete = implementTranslation?.ID > 0;
            bool canRevert = false;
            if (string.IsNullOrWhiteSpace(implementTranslation.EditValue) == false && string.IsNullOrWhiteSpace(implementTranslation.Value) == false)
            {
                canRevert = !(implementTranslation.EditValue.Equals(implementTranslation.Value));
            }

            if (wim.CurrentEnvironment["FORM_DATEPICKER"].ToUpperInvariant() == "EN")
            {
                dateCulture = new System.Globalization.CultureInfo("en-US");
            }

            Map(x => x.FieldName).TextLine("Field name").Expression(OutputExpression.FullWidth);
            Map(x => x.IsHiddenOnPage).Checkbox("Hide on page", false, "When checked, this field will not be shown on the page").Expression(OutputExpression.FullWidth);
            Map(x => x.PublishedValue).TextLine("Published Value").Expression(OutputExpression.FullWidth);

            switch (ImplementTranslation.ContentTypeID)
            {
                case ContentType.FileUpload:
                case ContentType.DocumentSelect:
                case ContentType.Binary_Document:
                    {
                        EditValueInt = Utility.ConvertToInt(ImplementTranslation.EditValue, 0);
                        PublishedValue = implementTranslation.GetPublishedValue(true);

                        Map(x => x.EditValueInt).Document("Edit value", false);
                    }
                    break;
                case ContentType.Binary_Image:
                    {
                        EditValueInt = Utility.ConvertToInt(ImplementTranslation.EditValue, 0);
                        PublishedValue = implementTranslation.GetPublishedValue(true);

                        Map(x => x.EditValueInt).Image("Edit value", false);
                    }
                    break;
                case ContentType.Choice_Checkbox:
                    {
                        EditValueBool = (ImplementTranslation.EditValue == "1");
                        PublishedValue = implementTranslation.GetPublishedValue();

                        Map(x => x.EditValueBool).Checkbox("Edit value", false);
                    }
                    break;
                case ContentType.Date:
                    {
                        if (DateTime.TryParseExact(ImplementTranslation.EditValue, "dd-MM-yyyy", dateCulture, System.Globalization.DateTimeStyles.None, out DateTime resultEdit))
                        {
                            EditValueDateTime = resultEdit;
                        }

                        PublishedValue = implementTranslation.GetPublishedValue();
                        Map(x => x.EditValueDateTime).Date("Edit value", false);
                    }
                    break;
                case ContentType.DateTime:
                    {
                        if (DateTime.TryParseExact(ImplementTranslation.EditValue, "dd-MM-yyyy HH:mm:ss", dateCulture, System.Globalization.DateTimeStyles.None, out DateTime resultEdit))
                        {
                            EditValueDateTime = resultEdit;
                        }

                        PublishedValue = implementTranslation.GetPublishedValue();
                        Map(x => x.EditValueDateTime).DateTime("Edit value", false);
                    }
                    break;
                case ContentType.FolderSelect:
                    {
                        EditValueInt = Utility.ConvertToInt(ImplementTranslation.EditValue, 0);
                        PublishedValue = implementTranslation.GetPublishedValue();

                        Map(x => x.EditValueInt).FolderSelect("Edit value", false);
                    }
                    break;
                case ContentType.Hyperlink:
                    {
                        EditValueInt = Utility.ConvertToInt(ImplementTranslation.EditValue, 0);
                        PublishedValue = implementTranslation.GetPublishedValue();

                        Map(x => x.EditValueInt).Hyperlink("Edit value", false);
                    }
                    break;
                case ContentType.MultiField:
                    {
                        EditValueMulti = ImplementTranslation.EditValue;
                        var mfs = MultiField.GetDeserialized(ImplementTranslation.Value);
                        if (mfs != null)
                        {
                            MultiFieldParser mfp = new MultiFieldParser();
                            PublishedValue = mfp.WriteHTML(ImplementTranslation.Value, null);
                        }

                        Map(x => x.EditValueMulti).MultiField("Edit value");
                    }
                    break;
                case ContentType.PageSelect:
                    {
                        EditValueInt = Utility.ConvertToInt(ImplementTranslation.EditValue, 0);
                        PublishedValue = implementTranslation.GetPublishedValue();

                        Map(x => x.EditValueInt).PageSelect("Edit value", false);
                    }
                    break;
                case ContentType.RichText:
                    {
                        PublishedValue = implementTranslation.GetPublishedValue();
                        EditValueString = ImplementTranslation.EditValue;
                        Map(x => x.EditValueString).RichText("Edit value");
                    }
                    break;
                case ContentType.Sourcecode: {
                        EditValueString = ImplementTranslation.EditValue;
                        PublishedValue = implementTranslation.GetPublishedValue();
                        Map(x => x.EditValueString).TextArea("Edit value", null, false, "", null, true);
                    }
                    break;
                case ContentType.TextArea:
                    {
                        EditValueString = ImplementTranslation.EditValue;
                        PublishedValue = implementTranslation.GetPublishedValue();
                        Map(x => x.EditValueString).TextArea("Edit value");
                    }
                    break;
                case ContentType.TextField:
                    {
                        EditValueString = ImplementTranslation.EditValue;
                        PublishedValue = implementTranslation.GetPublishedValue();
                        Map(x => x.EditValueString).TextField("Edit value");
                    }
                    break;
                
                case ContentType.SubListSelect:
                    {
                        // Create an Editable sublist, based off the Edit Value
                        if (string.IsNullOrWhiteSpace(implementTranslation.EditValue) == false)
                        {
                            EditValueSubList = SubList.GetDeserialized(implementTranslation.EditValue);
                        }
                        if (EditValueSubList == null)
                        {
                            EditValueSubList = new SubList();
                        }

                        // Create published value for display purposes
                        PublishedValue = implementTranslation.GetPublishedValue();
                
                        // Create Edit value for display purposes
                        EditValueString = implementTranslation.GetEditValue();
        
                        // Assume we cannot edit (default list view)
                        canEdit = false;

                        // If we received a template (from the page instance), we have a shot at filling the options
                        if (Template?.ID > 0)
                        {
                            var thisProperty = Property.SelectAllByTemplate(Template.ID).FirstOrDefault(x => x.FieldName == implement.FieldName);
                            if (string.IsNullOrWhiteSpace(thisProperty.ListCollection) == false && Utility.IsGuid(thisProperty.ListCollection, out Guid listGuid))
                            {
                                Map(x => x.EditValueSubList).SubListSelect("Edit value", listGuid, thisProperty.IsMandatory, false, false, thisProperty.InteractiveHelp);
                                canEdit = true;
                            }
                        }

                        // No editing is possible
                        if (canEdit == false)
                        {
                            Map(x => x.EditValueString).TextLine("Edit value").ReadOnly();
                        }
                    }
                    break;
                case ContentType.ListItemSelect:
                case ContentType.Choice_Dropdown:
                    {
                        EditValueString = ImplementTranslation.EditValue;
                        PublishedValue = implementTranslation.GetPublishedValue();
                        canEdit = false;

                        // If we received a template, we have a shot at filling the options
                        if (Template?.ID > 0) 
                        {
                            var thisProperty = Property.SelectAllByTemplate(Template.ID).FirstOrDefault(x => x.FieldName == implement.FieldName);
                            if (thisProperty?.ID > 0)
                            {
                                foreach (var opt in thisProperty.Options)
                                {
                                    EditValueOptions.Add(new ListItem()
                                    {
                                        Text = opt.Name,
                                        Value = opt.Value,
                                        Selected = (implementTranslation?.ID > 0 && opt.Value == implementTranslation.EditValue)
                                    });
                                }

                                Map(x => x.EditValueString).Dropdown("Edit value", nameof(EditValueOptions), thisProperty.IsMandatory, false);
                                canEdit = true;
                            }
                        }

                        if (canEdit == false)
                        {
                            Map(x => x.EditValueString).TextLine("Edit value").ReadOnly();
                        }
                    }
                    break;
                case ContentType.Undefined:
                case ContentType.TextLine:
                case ContentType.TextDate:
                
                case ContentType.Section:
                case ContentType.MultiImageSelect:
                case ContentType.MultiAssetUpload:
                case ContentType.Choice_Radio:
                case ContentType.HtmlContainer:
                    {
                        Map(x => x.EditValueString).TextLine("Edit value").ReadOnly();
                        PublishedValue = implementTranslation.GetPublishedValue();
                        canEdit = false; 
                    }
                    break;
            }

            Load(this);

            wim.SetPropertyVisibility(nameof(Data.Implementation.SharedFieldList.Button_Revert), canRevert);
            wim.SetPropertyVisibility(nameof(Data.Implementation.SharedFieldList.Button_Delete), canDelete);

            if (canEdit == false)
            {
                wim.Notification.AddNotification("This field can only be edited in-page, since it relies on values not available here");
            }

            LoadPagesAndComponents(wim);
        }

        #endregion Load - Dependent on ContentType

        #region Load Pages and Components

        private void LoadPagesAndComponents(WimComponentListRoot wim)
        {
            List<Page> allPages = new List<Page>();
            List<ComponentTemplate> allComponentTemplates = new List<ComponentTemplate>();
            var pageList = ComponentList.SelectOne(typeof(Data.Implementation.Browsing));
            var componentTemplateList = ComponentList.SelectOne(typeof(Data.Implementation.ComponentTemplate));

            var matchingProps = Property.SelectAllByFieldName(ImplementTranslation.FieldName);
            if (matchingProps?.Count > 0)
            {
                foreach (var prop in matchingProps.Where(x => x.TemplateID > 0))
                {
                    allComponentTemplates.Add(ComponentTemplate.SelectOne(prop.TemplateID));
                    var cVersions = ComponentVersion.SelectAllForTemplate(prop.TemplateID);
                    var pages = Page.SelectAll(cVersions.Select(x => x.PageID.GetValueOrDefault(0)).ToArray());
                    allPages.AddRange(pages);
                }
            }

            // Add the pages collection 
            StringBuilder bld = new StringBuilder();
            bld.Append("<section class=\"searchTable\">");
            bld.Append("<article class=\"dataBlock\">");
            bld.Append("<table width=\"100%\" class=\"selections\">");
            bld.Append("<thead><tr><th>Title</th><th>Path</th><th>View</th></tr>");
            bld.Append("<tbody>");
            if (allPages?.Count > 0)
            {
                foreach (var page in allPages)
                {
                    string pageTitle = string.IsNullOrWhiteSpace(page.Title) ? page.Name : page.Title;
                    string pagePath = string.IsNullOrWhiteSpace(page.CompletePath) ? page.HRefFull : page.CompletePath;
                    string pageUrl = string.IsNullOrWhiteSpace(page.HRefFull) ? page.HRef : page.HRefFull;
                    if (pageList?.ID > 0)
                    {
                        pageUrl = wim.GetUrl(new KeyValue[] {
                            new KeyValue() {
                                Key = "list",
                                Value = pageList.ID
                            },
                            new KeyValue()
                            {
                                Key="page",
                                Value = page.ID
                            },
                            new KeyValue()
                            {
                                Key="openinframe",
                                RemoveKey = true
                            },
                            new KeyValue()
                            {
                                Key="item",
                                RemoveKey = true
                            }
                        });
                    }
                    bld.Append($"<tr><td>{pageTitle}</td><td>{pagePath}</td><td><a target=\"_top\" href=\"{pageUrl}\">View</a></td></tr>");
                }
            }
            else
            {
                bld.Append($"<tr><td>-</td><td>-</td><td>&nbsp;</td></tr>");
            }
            bld.Append("</tbody>");
            bld.Append("</table><br class=\"clear\">");
            bld.Append("</article>");
            bld.Append("</section>");

            Pages = bld.ToString();
            Map(x => x.sec_Pages).Section($"{allPages?.Count} Page(s) containing this field");
            Map(x => x.Pages).HtmlContainer(true);


            // Do we have a component templates collection ?
            bld.Clear();
            bld.Append("<section class=\"searchTable\">");
            bld.Append("<article class=\"dataBlock\">");
            bld.Append("<table width=\"100%\" class=\"selections\">");
            bld.Append("<thead><tr><th>Title</th><th>Source Tag</th><th>View</th></tr>");
            bld.Append("<tbody>");
            if (allComponentTemplates?.Count > 0)
            {
                foreach (var cTemplate in allComponentTemplates)
                {
                    string componentTemplateUrl = "";
                    if (pageList?.ID > 0)
                    {
                        componentTemplateUrl = wim.GetUrl(new KeyValue[] {
                            new KeyValue() {
                                Key = "list",
                                Value = componentTemplateList.ID
                            },
                            new KeyValue()
                            {
                                Key = "item",
                                Value = cTemplate.ID
                            },
                            new KeyValue()
                            {
                                Key="openinframe",
                                RemoveKey = true
                            }
                        });
                    }
                    bld.Append($"<tr><td width=\"30%\">{cTemplate.Name}</td><td>{cTemplate.SourceTag}</td><td><a target=\"_top\" href=\"{componentTemplateUrl}\">View</a></td></tr>");
                }
            }
            else
            {
                bld.Append($"<tr><td>-</td><td>-</td><td>&nbsp;</td></tr>");
            }
            bld.Append("</tbody>");
            bld.Append("</table><br class=\"clear\">");
            bld.Append("</article>");
            bld.Append("</section>");

            ComponentTemplates = bld.ToString();
            Map(x => x.sec_ComponentTemplates).Section($"{allComponentTemplates?.Count} Component template(s) containing this field");
            Map(x => x.ComponentTemplates).HtmlContainer(true);
        }

        #endregion Load Pages and Components

        public string FieldName { get; set; }
        public bool IsHiddenOnPage { get; set; }
        public string PublishedValue { get; set; }

        public SubList EditValueSubList { get; set; }
        public string EditValueString { get; set; }
        public int EditValueInt { get; set; }
        public bool EditValueBool { get; set; }
        public DateTime EditValueDateTime { get; set; }
        public string EditValueMulti { get; set; }
        public string sec_Pages { get; set; }
        public string Pages { get; set; }
        public string sec_ComponentTemplates { get; set; }
        public string ComponentTemplates { get; set; }
    }
}