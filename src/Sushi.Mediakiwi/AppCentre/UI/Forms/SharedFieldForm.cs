using Sushi.Mediakiwi.Data;
using Sushi.Mediakiwi.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.AppCentre.UI.Forms
{
    public class SharedFieldFormMap : FormMap<SharedFieldFormMap>
    {
        public SharedFieldTranslation Implement { get; set; }
        private System.Globalization.CultureInfo dateCulture = new System.Globalization.CultureInfo("nl-NL");

        #region Save - Dependent on ContentType

        public async Task SaveAsync(bool andPublish = false)
        {
            switch (Implement.ContentTypeID)
            {
                case (int)ContentType.FileUpload:
                case (int)ContentType.DocumentSelect:
                case (int)ContentType.Binary_Image:
                case (int)ContentType.Binary_Document:
                case (int)ContentType.FolderSelect:
                case (int)ContentType.Hyperlink:
                case (int)ContentType.PageSelect:
                    {
                        Implement.EditValue = EditValueInt.ToString();
                    }
                    break;
                case (int)ContentType.Choice_Checkbox:
                    {
                        Implement.EditValue = EditValueBool ? "1" : "0";
                    }
                    break;
                case (int)ContentType.Date:
                    {
                        Implement.EditValue = EditValueDateTime.ToString("dd-MM-yyyy");
                    }
                    break;
                case (int)ContentType.DateTime:
                    {
                        Implement.EditValue = EditValueDateTime.ToString("dd-MM-yyyy HH:mm:ss");
                    }
                    break;
                case (int)ContentType.MultiField:
                    {
                        Implement.EditValue = EditValueMulti;
                    }
                    break;
                case (int)ContentType.RichText:
                case (int)ContentType.Sourcecode:
                case (int)ContentType.TextArea:
                case (int)ContentType.TextField:
                    {
                        Implement.EditValue = EditValueString;
                    }
                    break;
                case (int)ContentType.Undefined:
                case (int)ContentType.TextLine:
                case (int)ContentType.TextDate:
                case (int)ContentType.SubListSelect:
                case (int)ContentType.Section:
                case (int)ContentType.MultiImageSelect:
                case (int)ContentType.MultiAssetUpload:
                case (int)ContentType.ListItemSelect:
                case (int)ContentType.Choice_Dropdown:
                case (int)ContentType.Choice_Radio:
                case (int)ContentType.HtmlContainer:
                    break;
            }

            if (andPublish)
            {
                Implement.Value = Implement.EditValue;
            }

            await Implement.SaveAsync().ConfigureAwait(false);
        }

        #endregion Save - Dependent on ContentType

        #region Load - Dependent on ContentType

        public SharedFieldFormMap(WimComponentListRoot wim, SharedFieldTranslation implement)
        {
            Implement = implement;
            FieldName = Implement.FieldName;
            PublishedValue = "-";
            bool canEdit = true;
            bool canDelete = implement?.ID > 0;
            bool canRevert = false;
            if (string.IsNullOrWhiteSpace(implement.EditValue) == false && string.IsNullOrWhiteSpace(implement.Value) == false)
            {
                canRevert = !(implement.EditValue.Equals(implement.Value));
            }

            if (wim.CurrentEnvironment["FORM_DATEPICKER"].ToUpperInvariant() == "EN")
            {
                dateCulture = new System.Globalization.CultureInfo("en-US");
            }

            Map(x => x.FieldName).TextLine("Field name").Expression(OutputExpression.FullWidth);
            Map(x => x.PublishedValue).TextLine("Published Value").Expression(OutputExpression.FullWidth);

            switch (Implement.ContentTypeID)
            {
                case (int)ContentType.FileUpload:
                case (int)ContentType.DocumentSelect:
                case (int)ContentType.Binary_Document:
                    {
                        EditValueInt = Utility.ConvertToInt(Implement.EditValue, 0);
                        PublishedValue = implement.GetPublishedValue();

                        Map(x => x.EditValueInt).Document("Edit value", false);
                    }
                    break;
                case (int)ContentType.Binary_Image:
                    {
                        EditValueInt = Utility.ConvertToInt(Implement.EditValue, 0);
                        PublishedValue = implement.GetPublishedValue();

                        Map(x => x.EditValueInt).Image("Edit value", false);
                    }
                    break;
                case (int)ContentType.Choice_Checkbox:
                    {
                        EditValueBool = (Implement.EditValue == "1");
                        PublishedValue = implement.GetPublishedValue();

                        Map(x => x.EditValueBool).Checkbox("Edit value", false);
                    }
                    break;
                case (int)ContentType.Date:
                    {
                        if (DateTime.TryParseExact(Implement.EditValue, "dd-MM-yyyy", dateCulture, System.Globalization.DateTimeStyles.None, out DateTime resultEdit))
                        {
                            EditValueDateTime = resultEdit;
                        }

                        PublishedValue = implement.GetPublishedValue();
                        Map(x => x.EditValueDateTime).Date("Edit value", false);
                    }
                    break;
                case (int)ContentType.DateTime:
                    {
                        if (DateTime.TryParseExact(Implement.EditValue, "dd-MM-yyyy HH:mm:ss", dateCulture, System.Globalization.DateTimeStyles.None, out DateTime resultEdit))
                        {
                            EditValueDateTime = resultEdit;
                        }

                        PublishedValue = implement.GetPublishedValue();
                        Map(x => x.EditValueDateTime).DateTime("Edit value", false);
                    }
                    break;
                case (int)ContentType.FolderSelect:
                    {
                        EditValueInt = Utility.ConvertToInt(Implement.EditValue, 0);
                        PublishedValue = implement.GetPublishedValue();

                        Map(x => x.EditValueInt).FolderSelect("Edit value", false);
                    }
                    break;
                case (int)ContentType.Hyperlink:
                    {
                        EditValueInt = Utility.ConvertToInt(Implement.EditValue, 0);
                        PublishedValue = implement.GetPublishedValue();

                        Map(x => x.EditValueInt).Hyperlink("Edit value", false);
                    }
                    break;
                case (int)ContentType.MultiField:
                    {
                        EditValueMulti = Implement.EditValue;
                        var mfs = MultiField.GetDeserialized(Implement.Value);
                        if (mfs != null)
                        {
                            MultiFieldParser mfp = new MultiFieldParser();
                            PublishedValue = mfp.WriteHTML(Implement.Value, null);
                        }

                        Map(x => x.EditValueMulti).MultiField("Edit value");
                    }
                    break;
                case (int)ContentType.PageSelect:
                    {
                        EditValueInt = Utility.ConvertToInt(Implement.EditValue, 0);
                        PublishedValue = implement.GetPublishedValue();

                        Map(x => x.EditValueInt).PageSelect("Edit value", false);
                    }
                    break;
                case (int)ContentType.RichText:
                    {
                        PublishedValue = implement.GetPublishedValue();
                        EditValueString = Implement.EditValue;
                        Map(x => x.EditValueString).RichText("Edit value");
                    }
                    break;
                case (int)ContentType.Sourcecode:
                case (int)ContentType.TextArea:
                    {
                        EditValueString = Implement.EditValue;
                        PublishedValue = implement.GetPublishedValue();
                        Map(x => x.EditValueString).TextArea("Edit value");
                    }
                    break;
                case (int)ContentType.TextField:
                    {
                        EditValueString = Implement.EditValue;
                        PublishedValue = implement.GetPublishedValue();
                        Map(x => x.EditValueString).TextField("Edit value");
                    }
                    break;
                case (int)ContentType.Undefined:
                case (int)ContentType.TextLine:
                case (int)ContentType.TextDate:
                case (int)ContentType.SubListSelect:
                case (int)ContentType.Section:
                case (int)ContentType.MultiImageSelect:
                case (int)ContentType.MultiAssetUpload:
                case (int)ContentType.ListItemSelect:
                case (int)ContentType.Choice_Dropdown:
                case (int)ContentType.Choice_Radio:
                case (int)ContentType.HtmlContainer:
                    {
                        Map(x => x.EditValueString).TextLine("Edit value").ReadOnly();
                        PublishedValue = implement.GetPublishedValue();
                        canEdit = false;
                    }
                    break;
            }

            Load(this);

            wim.SetPropertyVisibility(nameof(Data.Implementation.SharedFieldList.Button_Revert), canRevert);
            wim.SetPropertyVisibility(nameof(Data.Implementation.SharedFieldList.Button_Delete), canDelete);
            wim.SetPropertyVisibility(nameof(Data.Implementation.SharedFieldList.Button_Save), canEdit);
            wim.SetPropertyVisibility(nameof(Data.Implementation.SharedFieldList.Button_SavePublish), canEdit);

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

            var matchingProps = Property.SelectAllByFieldName(Implement.FieldName);
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
        public string PublishedValue { get; set; }

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