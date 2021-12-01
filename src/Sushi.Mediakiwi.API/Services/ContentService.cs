using Sushi.Mediakiwi.API.Transport;
using Sushi.Mediakiwi.API.Transport.Responses;
using Sushi.Mediakiwi.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.API.Services
{
    public class ContentService : IContentService
    {
        private UrlResolver _resolver { get; set; }

        #region Get Grids

        private ICollection<ListGrid> GetGrids()
        {
            ICollection<ListGrid> result = new List<ListGrid>();

            ListGrid grid = new ListGrid();

            List<ListDataColumnType> hiddenTypes = new List<ListDataColumnType>()
            {
                ListDataColumnType.Highlight,
                ListDataColumnType.UniqueHighlightedIdentifier,
                ListDataColumnType.UniqueIdentifier
            };

            #region Columns

            foreach (var col in _resolver.ListInstance.wim.ListDataColumns.List)
            {
                grid.Columns.Add(new ListColumn()
                {
                    Align = (int)col.Alignment,
                    helpText = col.Tooltip,
                    IsAverage = col.Total == ListDataTotalType.Average,
                    IsSum = col.Total == ListDataTotalType.Sum,
                    IsHidden = hiddenTypes.Contains(col.Type),
                    Prefix = col.ColumnValuePrefix,
                    Suffix = col.ColumnValueSuffix,
                    Title = col.ColumnName,
                    Width = col.ColumnWidth
                });
            }

            #endregion Columns

            #region Rows

            foreach (var item in _resolver.ListInstance.wim.AppliedSearchGridItem)
            {
                // Get the type of item
                var itemType = item.GetType();

                // Create an instance of this type
                var tempComp = Activator.CreateInstance(itemType);

                // Reflect actual data to instance
                Utils.ReflectProperty(item, tempComp);

                var listRow = new ListRow()
                {
                    Items = new List<ListRowItem>()
                };

                foreach (var col in _resolver.ListInstance.wim.ListDataColumns.List)
                {
                    int? rowId = null;
                    var listRowItem = new ListRowItem()
                    {
                        CanWrap = false,
                        Value = itemType.GetProperty(col.ColumnValuePropertyName).GetValue(tempComp).ToString(),
                    };

                    switch (col.Type)
                    {
                        default:
                        case ListDataColumnType.Default: listRowItem.VueType = VueTypeEnum.FormTextline; break;
                        case ListDataColumnType.UniqueIdentifierPresent:
                        case ListDataColumnType.UniqueHighlightedIdentifier:
                        case ListDataColumnType.UniqueIdentifier:
                        case ListDataColumnType.UniqueHighlightedIdentifierPresent:
                            {
                                listRowItem.VueType = VueTypeEnum.FormTextline;
                                rowId = Utils.ConvertToInt(listRowItem.Value, -1);
                            }
                            break;
                        case ListDataColumnType.Highlight: listRowItem.VueType = VueTypeEnum.FormTextline; break;
                        case ListDataColumnType.HighlightPresent: listRowItem.VueType = VueTypeEnum.FormTextline; break;
                        case ListDataColumnType.ExportOnly: listRowItem.VueType = VueTypeEnum.FormTextline; break;
                        case ListDataColumnType.Checkbox: listRowItem.VueType = VueTypeEnum.FormChoiceCheckbox; break;
                        case ListDataColumnType.RadioBox: listRowItem.VueType = VueTypeEnum.FormChoiceRadio; break;
                        case ListDataColumnType.ViewOnly: listRowItem.VueType = VueTypeEnum.FormTextline; break;
                    }

                    listRow.Items.Add(listRowItem);
                    if (rowId.GetValueOrDefault(-1) > -1)
                    {
                        listRow.ID = rowId.Value;
                    }
                }

                grid.Rows.Add(listRow);
            }

            #endregion Rows

            #region Pagination

            if (_resolver.ListInstance.wim?.GridDataCommunication != null)
            {
                grid.Pagination = new ListPagination()
                {
                    CurrentPage = _resolver.ListInstance.wim.GridDataCommunication.CurrentPage,
                    ItemsPerPage = _resolver.ListInstance.wim.GridDataCommunication.PageSize,
                    TotalItems = _resolver.ListInstance.wim.GridDataCommunication.ResultCount.GetValueOrDefault(0)
                };
            }

            #endregion Pagination

            grid.Title = _resolver.ListInstance.wim.CurrentList.Name;

            result.Add(grid);

            return result;
        }

        #endregion Get Grids

        #region Get Notifications

        private ICollection<Notification> GetNotifications()
        {
            List<Notification> result = new List<Notification>();

            foreach (var error in _resolver.ListInstance.wim.Notification.GetPropertyErrors)
            {
                result.Add(new Notification()
                {
                    IsError = true,
                    Message = error.Value,
                    PropertyNames = new List<string>()
                        {
                            error.Key
                        }
                });
            }

            foreach (var error in _resolver.ListInstance.wim.Notification.GetGenericErrors)
            {
                result.Add(new Notification()
                {
                    IsError = true,
                    Message = error
                });
            }

            foreach (var error in _resolver.ListInstance.wim.Notification.GetGenericInformation)
            {
                result.Add(new Notification()
                {
                    IsError = false,
                    Message = error
                });
            }

            return result;
        }

        #endregion Get Notifications

        #region Get Resources

        private ICollection<ResourceItem> GetResources()
        {
            List<ResourceItem> result = new List<ResourceItem>();


            if (_resolver.ListInstance.wim?.Page?.Resources?.Items?.Any() == true)
            {
                foreach (var resource in _resolver.ListInstance.wim.Page.Resources.Items)
                {
                    var newResourceItem = new ResourceItem()
                    {
                        IsSync = resource.LoadAsync == false,
                        Path = resource.Path,
                    };

                    switch (resource.Location)
                    {
                        default:
                        case ResourceLocation.BODY_NESTED: newResourceItem.Position = 2; break;
                        case ResourceLocation.HEADER: newResourceItem.Position = 1; break;
                        case ResourceLocation.BODY_BELOW: newResourceItem.Position = 3; break;
                    }

                    switch (resource.ResourceType)
                    {
                        case ResourceType.JAVASCRIPT: newResourceItem.Type = 1; break;
                        case ResourceType.STYLESHEET: newResourceItem.Type = 2; break;
                        default:
                        case ResourceType.HTML:
                            {
                                newResourceItem.Type = 3;
                                newResourceItem.SourceCode = resource.Path;
                                newResourceItem.Path = string.Empty;
                            }
                            break;
                    }
                    result.Add(newResourceItem);
                }
            }

            return result;
        }

        #endregion Get Resources

        #region Convert Enums

        private JSEventEnum ConvertEnum(Framework.Api.MediakiwiJSEvent inType)
        {
            switch (inType)
            {
                default:
                case Framework.Api.MediakiwiJSEvent.none: return JSEventEnum.None;
                case Framework.Api.MediakiwiJSEvent.change: return JSEventEnum.Change;
                case Framework.Api.MediakiwiJSEvent.click: return JSEventEnum.Click;
                case Framework.Api.MediakiwiJSEvent.blur: return JSEventEnum.Blur;
                case Framework.Api.MediakiwiJSEvent.keyup: return JSEventEnum.KeyUp;
            }
        }

        private VueTypeEnum ConvertEnum(Framework.Api.MediakiwiFormVueType inType)
        {
            switch (inType)
            {
                default:
                case Framework.Api.MediakiwiFormVueType.undefined: return VueTypeEnum.Undefined;
                case Framework.Api.MediakiwiFormVueType.wimButton: return VueTypeEnum.FormButton;
                case Framework.Api.MediakiwiFormVueType.wimChoiceDropdown: return VueTypeEnum.FormChoiceDropdown;
                case Framework.Api.MediakiwiFormVueType.wimPlus: return VueTypeEnum.FormPlus;
                case Framework.Api.MediakiwiFormVueType.wimRichText: return VueTypeEnum.FormRichText;
                case Framework.Api.MediakiwiFormVueType.wimText: return VueTypeEnum.FormText;
                case Framework.Api.MediakiwiFormVueType.wimTextline: return VueTypeEnum.FormTextline;
                case Framework.Api.MediakiwiFormVueType.wimTagVue:
                case Framework.Api.MediakiwiFormVueType.wimTag: return VueTypeEnum.FormTag;
                case Framework.Api.MediakiwiFormVueType.wimChoiceCheckbox: return VueTypeEnum.FormChoiceCheckbox;
                case Framework.Api.MediakiwiFormVueType.wimTextArea: return VueTypeEnum.FormTextArea;
                case Framework.Api.MediakiwiFormVueType.wimChoiceRadio: return VueTypeEnum.FormChoiceRadio;
                case Framework.Api.MediakiwiFormVueType.wimDateTime: return VueTypeEnum.FormDateTime;
                case Framework.Api.MediakiwiFormVueType.wimDate: return VueTypeEnum.FormDate;
                case Framework.Api.MediakiwiFormVueType.wimSection: return VueTypeEnum.FormSection;

            }
        }

        private ContentTypeEnum ConvertEnum(Data.ContentType inType)
        {
            switch (inType)
            {
                default:
                case Data.ContentType.TimeSheetLine:
                case Data.ContentType.Undefined:
                case Data.ContentType.TextLine: return ContentTypeEnum.TextLine;
                case Data.ContentType.TextField: return ContentTypeEnum.TextField;
                case Data.ContentType.TextArea: return ContentTypeEnum.TextArea;
                case Data.ContentType.RichText: return ContentTypeEnum.RichText;
                case Data.ContentType.Date: return ContentTypeEnum.Date;
                case Data.ContentType.DateTime: return ContentTypeEnum.DateTime;
                case Data.ContentType.Choice_Radio: return ContentTypeEnum.ChoiceRadio;
                case Data.ContentType.Choice_Dropdown: return ContentTypeEnum.ChoiceDropdown;
                case Data.ContentType.Binary_Image: return ContentTypeEnum.BinaryImage;
                case Data.ContentType.Binary_Document: return ContentTypeEnum.BinaryDocument;
                case Data.ContentType.Hyperlink: return ContentTypeEnum.HyperLink;
                case Data.ContentType.PageSelect: return ContentTypeEnum.PageSelect;
                case Data.ContentType.FolderSelect: return ContentTypeEnum.FolderSelect;
                case Data.ContentType.ListItemSelect: return ContentTypeEnum.ListItemSelect;
                case Data.ContentType.SubListSelect: return ContentTypeEnum.SubListSelect;
                case Data.ContentType.FileUpload: return ContentTypeEnum.FileUpload;
                case Data.ContentType.Button: return ContentTypeEnum.Button;
                case Data.ContentType.Choice_Checkbox: return ContentTypeEnum.ChoiceCheckbox;
                case Data.ContentType.MultiImageSelect: return ContentTypeEnum.MultiImageSelect;
                case Data.ContentType.TextDate: return ContentTypeEnum.TextDate;
                case Data.ContentType.Section: return ContentTypeEnum.Section;
                case Data.ContentType.DataList: return ContentTypeEnum.DataList;
                case Data.ContentType.DataExtend: return ContentTypeEnum.DataExtend;
                case Data.ContentType.ContentContainer: return ContentTypeEnum.ContentContainer;
                case Data.ContentType.HtmlContainer: return ContentTypeEnum.HtmlContainer;
                case Data.ContentType.DocumentSelect: return ContentTypeEnum.DocumentSelect;
                case Data.ContentType.MultiField: return ContentTypeEnum.MultiField;
                case Data.ContentType.Sourcecode: return ContentTypeEnum.Sourcecode;
                case Data.ContentType.MultiAssetUpload: return ContentTypeEnum.MultiAssetUpload;
            }
        }


        #endregion Convert Enums

        #region Get Field

        private ContentField GetField(IContentInfo element)
        {
            ContentField newField = new ContentField();
            newField.ContentType = ConvertEnum(element.ContentTypeSelection);
            newField.Expression = (int)element.Expression;
            newField.HelpText = element.InteractiveHelp;
            newField.IsHidden = element.IsHidden;
            newField.IsMandatory = element.Mandatory;
            newField.IsReadOnly = element.IsReadOnly;
            newField.MaxLength = element.MaxValueLength;
            newField.PropertyName = element.FieldName;
            if (element.Property != null)
            {
                newField.PropertyType = element.Property.PropertyType.ToString();
            }
            newField.Title = element.Title;

            return newField;
        }

        private ContentField GetField(Framework.Api.MediakiwiField field)
        {
            var newField = new ContentField()
            {
                CanDeleteSection = field.CanDeleteSection,
                CanToggleSection = field.CanToggleSection,
                ClassName = field.ClassName,
                ContentType = ConvertEnum(field.ContentTypeID),
                Event = ConvertEnum(field.Event),
                Expression = (int)field.Expression,
                FormSection = field.FormSection,
                Groupname = field.GroupName,
                HelpText = field.HelpText,
                IsAutoPostback = field.IsAutoPostback,
                IsHidden = field.Hidden.GetValueOrDefault(false),
                IsMandatory = field.IsMandatory,
                IsReadOnly = field.ReadOnly,
                //LayerConfiguration = // TODO: add this
                MaxLength = field.MaxLength.GetValueOrDefault(0),
                Prefix = field.Prefix,
                PropertyName = field.PropertyName,
                PropertyType = field.PropertyType,
                Section = (int)field.Section,
                Suffix = field.Suffix,
                Title = field.Title,
                ToggleDefaultClosed = field.ToggleDefaultClosed,
                Value = (field.Value != null) ? field.Value.ToString() : "",
                VueType = ConvertEnum(field.VueType)
            };

            if (field?.Options?.Count > 0)
            {
                foreach (var item in field.Options)
                {
                    newField.Options.Add(new ListItemCollectionOption()
                    {
                        Text = item.Text,
                        IsEnabled = item.Enabled,
                        IsSelected = item.Selected,
                        Value = item.Value
                    });
                }
            }

            return newField;
        }

        #endregion Get Field

        #region Get Additional Data Value

        private T GetAdditionalDataValue<T>(Dictionary<string, object> dict, string fieldName)
        {
            if (dict?.ContainsKey(fieldName) == true && dict[fieldName] is T)
            {
                return (T)dict[fieldName];
            }

            return default(T);
        }

        #endregion Get Additional Data Value

        #region Get Button

        private async Task<ButtonField> GetButtonAsync(Framework.Api.MediakiwiField field)
        {
            // Result container
            ButtonField newButton = new ButtonField();

            // Get all standard Field content
            ContentField result = GetField(field);

            // Copy all field Content to button            
            Utils.ReflectProperty(result, newButton);

            // Reset expression
            newButton.Expression = 0;

            if (field.AdditionalData?.Count > 0)
            {
                newButton.AskConfirmation = GetAdditionalDataValue<bool>(field.AdditionalData, nameof(newButton.AskConfirmation));
                newButton.ConfirmationAcceptLabel = GetAdditionalDataValue<string>(field.AdditionalData, nameof(newButton.ConfirmationAcceptLabel));
                newButton.ConfirmationQuestion = GetAdditionalDataValue<string>(field.AdditionalData, nameof(newButton.ConfirmationQuestion));
                newButton.ConfirmationRejectLabel = GetAdditionalDataValue<string>(field.AdditionalData, nameof(newButton.ConfirmationRejectLabel));
                newButton.ConfirmationTitle = GetAdditionalDataValue<string>(field.AdditionalData, nameof(newButton.ConfirmationTitle));
                newButton.Target = GetAdditionalDataValue<string>(field.AdditionalData, nameof(newButton.Target));
                
                // Direct set URL
                var setUrl = GetAdditionalDataValue<string>(field.AdditionalData, "CustomUrl");

                // URL set by PropertyName
                var setUrlProperty = GetAdditionalDataValue<string>(field.AdditionalData, "CustomUrlProperty");

                // URL set as layer list
                var listInLayer = GetAdditionalDataValue<string>(field.AdditionalData, "ListInPopupLayer");

                // Construct the URL for the button
                if (string.IsNullOrWhiteSpace(setUrlProperty) == false)
                {
                    newButton.Url = _resolver.ListInstance.GetType().GetProperty(setUrlProperty).GetValue(_resolver.ListInstance, null) as string;
                }
                else if (string.IsNullOrWhiteSpace(listInLayer) == false && Utils.IsGuid(listInLayer, out Guid listGuid))
                {
                    var list2 = await Data.ComponentList.SelectOneAsync(listGuid).ConfigureAwait(false);
                    if (list2?.ID > 0)
                    {
                        string prefix = _resolver.UrlBuild.GetListRequest(_resolver.ListID.Value, _resolver.ItemID);
                        if (prefix.Contains("?"))
                            newButton.Url = string.Concat(prefix, "&openinframe=1");
                        else
                            newButton.Url = string.Concat(prefix, "?openinframe=1");
                    }
                }
                else if (string.IsNullOrWhiteSpace(setUrl) == false)
                {
                    newButton.Url = setUrl;
                }

                newButton.TriggerSaveEvent = GetAdditionalDataValue<bool>(field.AdditionalData, nameof(newButton.TriggerSaveEvent));
            }

            // Get layer configuration
            newButton.LayerConfiguration = GetLayerConfiguration(field);

            return newButton;
        }

        #endregion Get Button

        #region Get Layer Configuration

        private LayerConfiguration GetLayerConfiguration(Framework.Api.MediakiwiField field)
        {
            // Result container
            LayerConfiguration newLayer = null;

            if (field.AdditionalData?.Count > 0)
            {
                var openInPopupLayer = GetAdditionalDataValue<bool>(field.AdditionalData, "OpenInPopupLayer");
                if (openInPopupLayer)
                {
                    var popupHeight = GetAdditionalDataValue<string>(field.AdditionalData, "PopupLayerHeight");
                    var popupWidth = GetAdditionalDataValue<string>(field.AdditionalData, "PopupLayerWidth");
                    LayerSize popupSize = GetAdditionalDataValue<LayerSize>(field.AdditionalData, "PopupLayerSize");

                    newLayer.Title = GetAdditionalDataValue<string>(field.AdditionalData, "PopupTitle");
                    newLayer.HasScrollbar = GetAdditionalDataValue<bool>(field.AdditionalData, "PopupLayerHasScrollBar");

                    if (string.IsNullOrWhiteSpace(popupHeight) == false)
                    {
                        if (popupHeight.Contains("%"))
                        {
                            newLayer.HeightUnitType = UnitTypeEnum.Percentage;
                        }
                        else
                        {
                            newLayer.HeightUnitType = UnitTypeEnum.Pixels;
                        }
                        newLayer.Height = Convert.ToInt32(popupHeight.Replace("%", string.Empty).Replace("px", string.Empty));
                    }

                    if (string.IsNullOrWhiteSpace(popupWidth) == false)
                    {
                        if (popupWidth.Contains("%"))
                        {
                            newLayer.WidthUnitType = UnitTypeEnum.Percentage;
                        }
                        else
                        {
                            newLayer.WidthUnitType = UnitTypeEnum.Pixels;
                        }
                        newLayer.Width = Convert.ToInt32(popupWidth.Replace("%", string.Empty).Replace("px", string.Empty));
                    }

                    // When we received a enum layersize and no additional sizing info
                    if (popupSize != LayerSize.Undefined && newLayer.Width == 0 && newLayer.Height == 0)
                    {
                        switch (popupSize)
                        {
                            default:
                            case LayerSize.Undefined: break;
                            case LayerSize.Normal:
                                {
                                    newLayer.Height = 614;
                                    newLayer.HeightUnitType = UnitTypeEnum.Pixels;
                                    newLayer.Width = 864;
                                    newLayer.WidthUnitType = UnitTypeEnum.Pixels;
                                }
                                break;
                            case LayerSize.Small:
                                {
                                    newLayer.Height = 414;
                                    newLayer.HeightUnitType = UnitTypeEnum.Pixels;
                                    newLayer.Width = 760;
                                    newLayer.WidthUnitType = UnitTypeEnum.Pixels;
                                }
                                break;
                            case LayerSize.Tiny:
                                {
                                    newLayer.Height = 314;
                                    newLayer.HeightUnitType = UnitTypeEnum.Pixels;
                                    newLayer.Width = 472;
                                    newLayer.WidthUnitType = UnitTypeEnum.Pixels;
                                }
                                break;
                        }
                    }
                }
            }

            return newLayer;
        }

        #endregion Get Layer Configuration

        #region Get Form Maps

        private async Task<ICollection<Transport.FormMap>> GetFormMapsAsync()
        {
            List<Transport.FormMap> result = new List<Transport.FormMap>();

            // Then for any non-formmap field
            if (_resolver.ListInstance.wim.Console.Component == null)
            {
                _resolver.ListInstance.wim.Console.Component = new Beta.GeneratedCms.Source.Component();
            }

            var builder = _resolver.ListInstance.wim.Console.Component.CreateList(_resolver.ListInstance.wim.Console, _resolver.ListInstance.wim.Console.OpenInFrame, true);

            if (builder?.ApiResponse?.Fields?.Count > 0)
            {
                foreach (var formSection in builder.ApiResponse.Fields.GroupBy(x => x.FormSection))
                {
                    // Skip the Internal StateForm
                    if (formSection.Key.Equals("Sushi.Mediakiwi.Framework.StateForm", StringComparison.InvariantCultureIgnoreCase))
                    {
                        continue;
                    }

                    Transport.FormMap newFormMap = new Transport.FormMap()
                    {
                        ClassName = formSection.Key
                    };

                    if (formSection.Any() == true)
                    {
                        foreach (var field in formSection)
                        {
                            if (field.ContentTypeID == Data.ContentType.Button)
                            {
                                var newButtonField = await GetButtonAsync(field).ConfigureAwait(false);
                                if (newButtonField == null)
                                {
                                    continue;
                                }

                                // Since we are in a FormMap, the section will always be the same as the FormMap Classname
                                if (newButtonField.FormSection.Equals(newFormMap.ClassName, StringComparison.InvariantCultureIgnoreCase))
                                {
                                    newButtonField.FormSection = string.Empty;
                                }
                                newFormMap.Buttons.Add(newButtonField);
                            }
                            else
                            {
                                var newField = GetField(field);
                                if (newField == null)
                                {
                                    continue;
                                }

                                // Since we are in a FormMap, the section will always be the same as the FormMap Classname
                                if (newField.FormSection.Equals(newFormMap.ClassName, StringComparison.InvariantCultureIgnoreCase))
                                {
                                    newField.FormSection = string.Empty;
                                }
                                newFormMap.Fields.Add(newField);
                            }
                        }

                        // Add internal Buttons (EDIT, SAVE, DELETE)
                        newFormMap.Buttons.AddRange(GetInternalButtons());

                        result.Add(newFormMap);
                    }
                }
            }

            return result;
        }

        #endregion Get Form Maps

        #region Get Internal Buttons

        private List<ButtonField> GetInternalButtons()
        {
            List<ButtonField> result = new List<ButtonField>();


            // Add EDIT Button when not in Edit state
            if (
                  _resolver.ListInstance.wim.HasListSave
                  && _resolver.ListInstance.wim.CurrentList.Option_CanSave
                  && _resolver.ListInstance.wim.IsSubSelectMode == false
                  && _resolver.ListInstance.wim.HideCreateNew == false
                  && _resolver.ListInstance.wim.HideEditOption == false
                  && _resolver.ListInstance.wim.IsEditMode == false
                  && _resolver.ListInstance.wim.OpenInEditMode == false)
            {
                result.Add(new ButtonField()
                {
                    AskConfirmation = false,
                    PropertyName = "edit",
                    PropertyType = typeof(bool).FullName,
                    ContentType = ContentTypeEnum.Button,
                    Event = JSEventEnum.Click,
                    IsPrimary = true,
                    Title = Common.GetLabelFromResource("edit", new CultureInfo(_resolver.ListInstance.wim.CurrentApplicationUser.LanguageCulture)),
                    VueType = VueTypeEnum.FormButton,
                    ClassName = "action",
                    Section = (int)ButtonSection.Bottom,
                    Url = "#"
                });
            }

            // Add SAVE & DELETE Buttons when in Edit state
            if (_resolver.ListInstance.IsEditMode)
            {
                // SAVE Buttons
                if (_resolver.ListInstance.wim.HasListSave && (_resolver.ListInstance.wim.CanAddNewItem || _resolver.ItemID.GetValueOrDefault() > 0 || _resolver.ListInstance.wim.CurrentList.IsSingleInstance))
                {
                    string saveRecord = _resolver.List.Data["wim_LblSave"].Value;
                    if (string.IsNullOrEmpty(saveRecord))
                    {
                        saveRecord = Common.GetLabelFromResource("save", new CultureInfo(_resolver.ListInstance.wim.CurrentApplicationUser.LanguageCulture));
                    }

                    if (_resolver.ListInstance.wim.HideSaveButtons == false && _resolver.ListInstance.wim.CurrentList.Data["wim_CanSave"].ParseBoolean(true))
                    {
                        result.Add(new ButtonField()
                        {
                            AskConfirmation = false,
                            PropertyName = "save",
                            PropertyType = typeof(bool).FullName,
                            ContentType = ContentTypeEnum.Button,
                            Event = JSEventEnum.Click,
                            IsPrimary = true,
                            Title = saveRecord,
                            VueType = VueTypeEnum.FormButton,
                            ClassName = "action right",
                            Section = (int)ButtonSection.Bottom,
                            Url = "#",
                            TriggerSaveEvent = true
                        });

                        if (_resolver.ListInstance.wim.CanSaveAndAddNew && _resolver.ListInstance.wim.CanContainSingleInstancePerDefinedList == false)
                        {
                            result.Add(new ButtonField()
                            {
                                AskConfirmation = false,
                                PropertyName = "saveNew",
                                PropertyType = typeof(bool).FullName,
                                ContentType = ContentTypeEnum.Button,
                                Event = JSEventEnum.Click,
                                IsPrimary = true,
                                Title = Common.GetLabelFromResource("save_and_new", new CultureInfo(_resolver.ListInstance.wim.CurrentApplicationUser.LanguageCulture)),
                                VueType = VueTypeEnum.FormButton,
                                ClassName = "action right",
                                Section = (int)ButtonSection.Bottom,
                                Url = "#",
                                TriggerSaveEvent = true
                            });
                        }
                    }
                }

                // DELETE Button
                if (_resolver.ListInstance.wim.HasListDelete && _resolver.ItemID.GetValueOrDefault() > 0)
                {
                    ButtonSection section = ButtonSection.Top;

                    // Determine position of Delete button
                    switch (_resolver.ListInstance.wim.Page.Body.Navigation.Menu.DeleteButtonTarget)
                    {
                        default:
                        case ButtonTarget.TopRight: { section = ButtonSection.Top; } break;
                        case ButtonTarget.TopLeft: { section = ButtonSection.Top; } break;
                        case ButtonTarget.BottomLeft: { section = ButtonSection.Bottom; } break;
                        case ButtonTarget.BottomRight: { section = ButtonSection.Bottom; } break;
                    }

                    result.Add(new ButtonField()
                    {
                        PropertyName = "delete",
                        PropertyType = typeof(bool).FullName,
                        ContentType = ContentTypeEnum.Button,
                        Event = JSEventEnum.Click,
                        Title = Common.GetLabelFromResource("delete", new CultureInfo(_resolver.ListInstance.wim.CurrentApplicationUser.LanguageCulture)),
                        VueType = VueTypeEnum.FormButton,
                        ClassName = $"abbr type_confirm flaticon icon-trash-o",
                        Section = (int)section,
                        Url = "#",
                        AskConfirmation = true,
                        ConfirmationQuestion = Common.GetLabelFromResource("delete_confirm", new CultureInfo(_resolver.ListInstance.wim.CurrentApplicationUser.LanguageCulture)),
                        ConfirmationTitle = Common.GetLabelFromResource("delete_confirm_title", new CultureInfo(_resolver.ListInstance.wim.CurrentApplicationUser.LanguageCulture)),
                        ConfirmationAcceptLabel = Common.GetLabelFromResource("yes", new CultureInfo(_resolver.ListInstance.wim.CurrentApplicationUser.LanguageCulture)),
                        ConfirmationRejectLabel = Common.GetLabelFromResource("no", new CultureInfo(_resolver.ListInstance.wim.CurrentApplicationUser.LanguageCulture)),
                    });

                }
            }

            return result;
        }

        #endregion Get Internal Buttons

        public async Task<GetListResponse> GetListResponseAsync(UrlResolver resolver)
        {
            _resolver = resolver;
            GetListResponse result = new GetListResponse();

            if (resolver.List?.ID > 0)
            {
                result.Title = resolver.List.Name;
                result.Description = resolver.List.Description;
            }

            // We are looking at an Item
            if (resolver.ItemID.HasValue)
            {
                //resolver.ListInstance.wim.DoListLoad(resolver.ItemID.Value, 0);
                result.FormMaps = await GetFormMapsAsync().ConfigureAwait(false);
            }
            // We are looking at the overview
            else if (resolver.ListID.HasValue)
            {
                resolver.ListInstance.wim.DoListSearch();
                result.Grids = GetGrids();
            }

            result.Notifications = GetNotifications();
            result.Resources = GetResources();
            result.SettingsURL = resolver.UrlBuild.GetListPropertiesRequest();
            result.IsEditMode = resolver.ListInstance.wim.IsEditMode;
            
            return result;
        }
    }
}
