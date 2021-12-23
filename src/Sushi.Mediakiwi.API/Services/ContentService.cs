using Sushi.Mediakiwi.API.Transport;
using Sushi.Mediakiwi.API.Transport.Responses;
using Sushi.Mediakiwi.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using Sushi.Mediakiwi.API.Extensions;

namespace Sushi.Mediakiwi.API.Services
{
    public class ContentService : IContentService
    {
        private UrlResolver _resolver { get; set; }

        #region Is Key Column

        /// <summary>
        /// Determines whether the supplied type is a Key Column
        /// </summary>
        bool IsKeyColumn(ListDataColumnType type)
        {
            if (type == ListDataColumnType.UniqueHighlightedIdentifier ||
                type == ListDataColumnType.UniqueHighlightedIdentifierPresent ||
                type == ListDataColumnType.UniqueIdentifier ||
                type == ListDataColumnType.UniqueIdentifierPresent)
                return true;
            return false;
        }

        #endregion Is Key Column

        #region Get Row Item URL

        private string GetRowItemUrl(PropertyInfo[] infoCollection, object item)
        {
            string passThrough = string.Empty;
            string uniqueIdentifier = string.Empty;

            // Check for a key column
            var keyColumn = _resolver.ListInstance.wim.ListDataColumns.List.First(x => IsKeyColumn(x.Type));
            if (keyColumn != null)
            {
                PropertyInfo info = infoCollection.FirstOrDefault(x => x.Name.Equals(keyColumn.ColumnValuePropertyName, StringComparison.InvariantCulture));
                if (info != null) 
                {
                    uniqueIdentifier = info.GetValue(item, null).ToString();
                }
            }

            
            if (string.IsNullOrWhiteSpace(_resolver.ListInstance.wim.SearchResultItemPassthroughParameterProperty) == false)
            {
                PropertyInfo info = infoCollection.FirstOrDefault(x => x.Name.Equals(_resolver.ListInstance.wim.SearchResultItemPassthroughParameterProperty, StringComparison.InvariantCulture));
                if (info != null)
                {
                    passThrough = info.GetValue(item, null).ToString();
                }
            }

            else if (string.IsNullOrWhiteSpace(_resolver.ListInstance.wim.SearchResultItemPassthroughParameter) == false)
            {
                passThrough = _resolver.ListInstance.wim.SearchResultItemPassthroughParameter;
            }

            if (_resolver.ListInstance.wim.SearchListCanClickThrough && string.IsNullOrWhiteSpace(uniqueIdentifier) == false && (uniqueIdentifier != "0" || string.IsNullOrWhiteSpace(passThrough) == false))
            {
                if (passThrough.EndsWith("item=", StringComparison.InvariantCultureIgnoreCase))
                {
                    passThrough += uniqueIdentifier;
                }
                else if (passThrough.EndsWith("item", StringComparison.InvariantCultureIgnoreCase))
                {
                    passThrough += $"={uniqueIdentifier}";
                }

                if (passThrough.Contains("[KEY]", StringComparison.InvariantCultureIgnoreCase))
                {
                    passThrough = passThrough.Replace("[KEY]", uniqueIdentifier);
                }
            }

            return passThrough;
        }

        #endregion Get Row Item URL

        #region Get Grids

        private async Task<ICollection<ListGrid>> GetGridsAsync()
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

            #region Layer Configuration

            if (_resolver?.List?.Option_LayerResult == true && _resolver.ListInstance?.wim?.Page?.Body?.Grid?.LayerConfiguration == null)
            {
                _resolver.ListInstance.wim.Page.Body.Grid.SetClickLayer(new Grid.LayerSpecification(LayerSize.Normal));
            }

            if (_resolver.ListInstance?.wim?.Page?.Body?.Grid?.LayerConfiguration != null)
            {
                var config = _resolver.ListInstance.wim.Page.Body.Grid.LayerConfiguration;
                grid.LayerConfiguration = new LayerConfiguration()
                {
                    HasScrollbar = config.HasScrolling.GetValueOrDefault(false),
                    Height = config.Height.GetValueOrDefault(),
                    HeightUnitType = config.IsHeightPercentage ? UnitTypeEnum.Percentage : UnitTypeEnum.Pixels,
                    Title = config.Title,
                    Width = config.Width.GetValueOrDefault(),
                    WidthUnitType = config.IsWidthPercentage ? UnitTypeEnum.Percentage : UnitTypeEnum.Pixels
                };
            }

            #endregion Layer Configuration

            #region Rows

            if (_resolver.ListInstance.wim.AppliedSearchGridItem != null)
            {
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
                        Items = new List<ListRowItem>(),
                        Href = GetRowItemUrl(itemType.GetProperties(), item)
                    };


                    foreach (var col in _resolver.ListInstance.wim.ListDataColumns.List)
                    {
                        int? rowId = null;

                        var listRowItem = new ListRowItem()
                        {
                            CanWrap = false,
                        };

                        var objValue = itemType.GetProperty(col.ColumnValuePropertyName).GetValue(tempComp);
                        if (objValue != null)
                        {
                            listRowItem.Value = objValue.ToString();
                        }

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
            }

            #endregion Rows

            #region Pagination

            if (_resolver.ListInstance.wim?.GridDataCommunication != null)
            {
                grid.Pagination = new ListPagination()
                {
                    CurrentPage = _resolver.ListInstance.wim.GridDataCommunication.CurrentPage,
                    ItemsPerPage = _resolver.ListInstance.wim.GridDataCommunication.PageSize,
                    TotalItems = _resolver.ListInstance.wim.ListDataRecordCount
                };

                // Set correct Page when it's currently ZERO
                if (grid.Pagination.CurrentPage == 0)
                {
                    grid.Pagination.CurrentPage = 1;
                }
            }

            #endregion Pagination

            #region Buttons

            foreach (var item in _resolver.ListInstance.GetType().GetProperties())
            {
                var buttonAttribute = item.GetCustomAttribute<Framework.ContentListSearchItem.ButtonAttribute>();
                if (buttonAttribute != null)
                { 
                    var newButton = await GetButtonAsync(buttonAttribute);
                    newButton.FormSection = grid.Title;
                    newButton.PropertyName = item.Name;
                    newButton.PropertyType = item.PropertyType.FullName;

                    grid.Buttons.Add(newButton);
                }
            }

            grid.Buttons.AddRange(GetInternalButtons());


            #endregion Buttons

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
                        case Framework.ResourceLocation.BODY_NESTED: newResourceItem.Position = 2; break;
                        case Framework.ResourceLocation.HEADER: newResourceItem.Position = 1; break;
                        case Framework.ResourceLocation.BODY_BELOW: newResourceItem.Position = 3; break;
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
                case Framework.Api.MediakiwiJSEvent.None: return JSEventEnum.None;
                case Framework.Api.MediakiwiJSEvent.Change: return JSEventEnum.Change;
                case Framework.Api.MediakiwiJSEvent.Click: return JSEventEnum.Click;
                case Framework.Api.MediakiwiJSEvent.Blur: return JSEventEnum.Blur;
                case Framework.Api.MediakiwiJSEvent.Keyup: return JSEventEnum.KeyUp;
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
                newField.Options = new List<ListItemCollectionOption>();

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


        private async Task<ButtonField> GetButtonAsync(Framework.ContentListItem.ButtonAttribute field)
        {
            // Result container
            ButtonField newButton = new ButtonField();

            // Reset expression
            newButton.Expression = 0;

            newButton.AskConfirmation = field.AskConfirmation;
            newButton.ConfirmationAcceptLabel = field.ConfirmationAcceptLabel;
            newButton.ConfirmationQuestion = field.ConfirmationQuestion;
            newButton.ConfirmationRejectLabel = field.ConfirmationRejectLabel;
            newButton.ConfirmationTitle = field.ConfirmationTitle;
            newButton.Target = field.Target;
            newButton.Title = field.Title;
            newButton.HelpText = field.InteractiveHelp;
            newButton.ContentType = ConvertEnum(field.ContentTypeSelection);
            newButton.Event = JSEventEnum.Click;
            newButton.IsHidden = field.IsHidden;
            newButton.IsPrimary = field.IsPrimary;
            newButton.VueType = VueTypeEnum.FormButton;

            // Direct set URL
            var setUrl = field.CustomUrl;

            // URL set by PropertyName
            var setUrlProperty = field.CustomUrlProperty;

            // URL set as layer list
            var listInLayer = field.ListInPopupLayer;

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

            newButton.TriggerSaveEvent = field.TriggerSaveEvent;


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
                    newLayer = new LayerConfiguration();
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

        private LayerConfiguration GetLayerConfiguration(Framework.ContentListItem.ButtonAttribute field)
        {
            // Result container
            LayerConfiguration newLayer = null;

            if (field.OpenInPopupLayer)
            {
                newLayer = new LayerConfiguration();
                LayerSize popupSize = field.PopupLayerSize;

                newLayer.Title = field.PopupTitle;
                newLayer.HasScrollbar = field.PopupLayerScrollBar;

                if (string.IsNullOrWhiteSpace(field.PopupLayerHeight) == false)
                {
                    if (field.PopupLayerHeight.Contains("%"))
                    {
                        newLayer.HeightUnitType = UnitTypeEnum.Percentage;
                    }
                    else
                    {
                        newLayer.HeightUnitType = UnitTypeEnum.Pixels;
                    }
                    newLayer.Height = Convert.ToInt32(field.PopupLayerHeight.Replace("%", string.Empty).Replace("px", string.Empty));
                }

                if (string.IsNullOrWhiteSpace(field.PopupLayerWidth) == false)
                {
                    if (field.PopupLayerWidth.Contains("%"))
                    {
                        newLayer.WidthUnitType = UnitTypeEnum.Percentage;
                    }
                    else
                    {
                        newLayer.WidthUnitType = UnitTypeEnum.Pixels;
                    }
                    newLayer.Width = Convert.ToInt32(field.PopupLayerWidth.Replace("%", string.Empty).Replace("px", string.Empty));
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
                    if (formSection.Key?.Equals("Sushi.Mediakiwi.Framework.StateForm", StringComparison.InvariantCultureIgnoreCase) == true)
                    {
                        continue;
                    }

                    Transport.FormMap newFormMap = new Transport.FormMap()
                    {
                        ClassName = formSection.Key,
                        Title = _resolver.ListInstance.wim.ListTitle
                    };

                    if (formSection.Any() == true)
                    {
                        foreach (var field in formSection)
                        {           
                            // Apply any Changes in Readonly / Mandatory / Hidden based on attributes
                            if (_resolver.ListInstance?.wim?.Console?.Component?.AllListProperties.Any() == true)
                            {
                                var prop = _resolver.ListInstance.wim.Console.Component.AllListProperties.FirstOrDefault(x => x.Name == field.PropertyName);
                                if (prop != null)
                                {
                                    // Don't show List Search fields
                                    if (prop.ContentAttribute is IListSearchContentInfo)
                                    {
                                        continue;
                                    }
                                    field.ReadOnly = !prop.IsEditable;
                                    field.Hidden = !prop.IsVisible;
                                    field.IsMandatory = prop.IsRequired;
                                }
                            }

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
                            else if (field.ContentTypeID == Data.ContentType.DataList)
                            {
                                // TODO: Add datalist output to somewhere
                                var newField = GetField(field);
                                newFormMap.Fields.Add(newField);
                            }
                            else 
                            {
                                var newField = GetField(field);
                                if (newField == null)
                                {
                                    continue;
                                }

                                // Since we are in a FormMap, the section will always be the same as the FormMap Classname
                                if (newField.FormSection?.Equals(newFormMap.ClassName, StringComparison.InvariantCultureIgnoreCase) == true)
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
            else if (_resolver.ListInstance.wim.Console.Component?.ButtonList?.Count > 0)
            {
                Transport.FormMap newFormMap = new Transport.FormMap();

                foreach (var button in _resolver.ListInstance.wim.Console.Component?.ButtonList)
                {
                    newFormMap.Buttons.Add(await GetButtonAsync(button));
                }

                result.Add(newFormMap);
            }

            return result;
        }

        private async Task<Transport.FormMap> GetFormMapListSearchAsync()
        {
            var formMap = new Transport.FormMap();

            // First loop through properties on List
            foreach (var prop in _resolver.ListInstance.GetType().GetProperties())
            {
                foreach (var attr in prop.GetCustomAttributes())
                {
                    // Get all ListSearch properties
                    if (attr is IListSearchContentInfo && attr is IContentInfo ci)
                    {
                        // Assign the Console, used for determining some CSS classes 
                        if (attr is ContentSharedAttribute sharedAttribute)
                        {
                            sharedAttribute.Console = _resolver.ListInstance.wim.Console;
                        }

                        // Buttons are added elsewhere, skip them
                        if (ci.ContentTypeSelection != Data.ContentType.Button)
                        {
                            ci.Property = prop;
                            ci.ID = prop.Name;
                            ci.SenderInstance = _resolver.ListInstance;
                            var apiField = await ci.GetApiFieldAsync();

                            formMap.Fields.Add(GetField(apiField));
                        }
                    }
                }
            }

            // Then loop through Formmaps on List
            if (_resolver.ListInstance?.FormMaps?.Count > 0)
            {
                foreach (var map in _resolver.ListInstance.FormMaps.List)
                {
                    // Skip internal StateForm
                    if (map is StateForm)
                    {
                        continue;
                    }

                    foreach (var mapElement in map.Elements)
                    {
                        mapElement.SenderInstance = _resolver.ListInstance;
                        var apiField = await mapElement.GetApiFieldAsync();

                        formMap.Fields.Add(GetField(apiField));
                    }
                }
            }

            // Add 'Search' button to list search, but ONLY if we have any fields
            if (_resolver.ListInstance.wim.HideSearchButton == false && formMap?.Fields?.Count > 0)
            {
                string searchLabel = _resolver.List.Label_Search;

                if (string.IsNullOrWhiteSpace(searchLabel)) {
                    searchLabel = Common.GetLabelFromResource("search", new CultureInfo(_resolver.ApplicationUser.LanguageCulture));
                }
                formMap.Buttons.Add(new ButtonField
                {
                    PropertyName = "searchBtn",
                    PropertyType = typeof(bool).FullName,
                    ClassName = "action",
                    Title = searchLabel,
                    ContentType = ConvertEnum(Data.ContentType.Button),
                    Event = JSEventEnum.Click,
                    VueType = VueTypeEnum.FormButton,
                });
            }

            return formMap;
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
                  && _resolver.ListInstance.wim.OpenInEditMode == false
                  && 
                  (_resolver.ListInstance.wim.CanContainSingleInstancePerDefinedList 
                  || _resolver.ItemObject != null))
            {
                result.Add(new ButtonField()
                {
                    AskConfirmation = false,
                    PropertyName = "edit",
                    PropertyType = typeof(bool).FullName,
                    ContentType = ContentTypeEnum.Button,
                    Event = JSEventEnum.Click,
                    IsPrimary = true,
                    Title = Common.GetLabelFromResource("edit", new CultureInfo(_resolver.ApplicationUser.LanguageCulture)),
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
                        saveRecord = Common.GetLabelFromResource("save", new CultureInfo(_resolver.ApplicationUser.LanguageCulture));
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
                                Title = Common.GetLabelFromResource("save_and_new", new CultureInfo(_resolver.ApplicationUser.LanguageCulture)),
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
                        Title = Common.GetLabelFromResource("delete", new CultureInfo(_resolver.ApplicationUser.LanguageCulture)),
                        VueType = VueTypeEnum.FormButton,
                        ClassName = $"abbr type_confirm flaticon icon-trash-o",
                        Section = (int)section,
                        Url = "#",
                        AskConfirmation = true,
                        ConfirmationQuestion = Common.GetLabelFromResource("delete_confirm", new CultureInfo(_resolver.ApplicationUser.LanguageCulture)),
                        ConfirmationTitle = Common.GetLabelFromResource("delete_confirm_title", new CultureInfo(_resolver.ApplicationUser.LanguageCulture)),
                        ConfirmationAcceptLabel = Common.GetLabelFromResource("yes", new CultureInfo(_resolver.ApplicationUser.LanguageCulture)),
                        ConfirmationRejectLabel = Common.GetLabelFromResource("no", new CultureInfo(_resolver.ApplicationUser.LanguageCulture)),
                    });

                }
            }
            else
            {
                // ADD the 'Create new' BUTTON when possible
                if (_resolver.ListInstance.wim.HideCreateNew == false 
                    && _resolver.ListInstance.wim.CanAddNewItem 
                    && _resolver.ListInstance.wim.HasListLoad 
                    && _resolver.ListInstance.wim.CanContainSingleInstancePerDefinedList == false)
                {                
                    
                    string newRecord = _resolver.List.Data["wim_LblNew"].Value;

                    if (string.IsNullOrWhiteSpace(newRecord))
                    {
                        newRecord = Common.GetLabelFromResource("new_record", new CultureInfo(_resolver.ApplicationUser.LanguageCulture));
                    }

                    bool hasPrimary = false;
                    if (!hasPrimary && result?.Count > 0)
                    {
                        foreach (var button in result)
                        {
                            if (button.IsPrimary)
                            {
                                hasPrimary = true;
                                break;
                            }
                        }
                    }
                    result.Add(new ButtonField()
                    {
                        Title = newRecord,
                        ClassName = hasPrimary ? null : "action ",
                        ContentType = ConvertEnum(Data.ContentType.Button),
                        VueType = VueTypeEnum.FormButton,
                        Event = JSEventEnum.Click,
                        PropertyName = "new",
                        PropertyType = typeof(bool).FullName,
                        IsPrimary = !hasPrimary,
                        Url = _resolver.UrlBuild.GetListNewRecordRequest()
                    });
                }


            }

            return result;
        }

        #endregion Get Internal Buttons

        #region Get List Explorer Response Async

        private async Task<GetExplorerResponse> GetListExplorerResponseAsync(Data.Folder[] folders, bool ignoreHeader)
        {
            int _columns = 0;

            GetExplorerResponse result = new GetExplorerResponse();

            foreach (Data.Folder entry in folders)
            {
                if (entry.IsVisible == false && _resolver.ApplicationUser.ShowHidden == false)
                {
                    continue;
                }

                var all_lists = await Data.ComponentList.SelectAllAsync(entry.ID, _resolver.ApplicationUser, true).ConfigureAwait(false);
                var allowed_lists = await Data.ComponentList.ValidateAccessRightAsync(all_lists, _resolver.ApplicationUser).ConfigureAwait(false);
                Data.IComponentList[] selected_lists = null;

                if (_resolver.ApplicationUser.ShowHidden)
                {
                    selected_lists = allowed_lists;
                }
                else
                {
                    selected_lists = (from x in allowed_lists where x.IsVisible select x).ToArray();
                }

                if (entry.ParentID.HasValue || selected_lists.Length > 0)
                {
                    _columns++;

                    BrowseFolder container = new BrowseFolder()
                    {
                        ID = entry.ID,
                        IconClasses = new List<string>() { "listfolder" }
                    };

                    if (!ignoreHeader && entry.Name != "/")
                    {
                        container.Title = entry.Name;
                        container.Href = $"{_resolver.WimPagePath}?folder={entry.ID}";

                        if (string.IsNullOrWhiteSpace(entry.Description) == false)
                        {
                            container.Description = entry.Description;
                        }
                    }

                    foreach (var i in selected_lists)
                    {
                        ComponentDataReportEventArgs e = null;
                        if (i.Option_HasDataReport)
                        {
                            var instance = i.GetInstance(_resolver.ListInstance.wim.Console);
                            if (instance != null)
                            {
                                e = instance.wim.DoListDataReport();
                            }
                        }

                        if (e == null || e.ReportCount.HasValue == false)
                        {
                            var url = _resolver.UrlBuild.GetListRequest(i);
                            container.Items.Add(new BrowseItem()
                            {
                                Title = i.Name,
                                Href = url,
                                ID = i.ID
                            });
                        }
                        else
                        {
                            var count = $"{e.ReportCount.Value}";
                            if (e.ReportCount.Value > 99)
                            {
                                count = "99+";
                            }

                            var url = _resolver.UrlBuild.GetListRequest(i);
                            container.Items.Add(new BrowseItem()
                            {
                                Href = $"{url}{i.Name}",
                                Title = i.Name,
                                BadgeContent = string.IsNullOrWhiteSpace(count) ? count : null,
                                ID = i.ID
                            });
                        }
                    }
                    result.Items.Add(container);
                }

                var arr = await Data.Folder.SelectAllByParentAsync(entry.ID, Data.FolderType.Undefined, _resolver.ApplicationUser.ShowHidden == false).ConfigureAwait(false);
                var items = await GetListExplorerResponseAsync(arr, false).ConfigureAwait(false);
                if (items?.Items?.Any() == true)
                {
                    result.Items.AddRange(items.Items);
                }
            }

            return result;
        }

        #endregion Get List Explorer Response Async

        #region Get List Response

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
            if (resolver.ItemObject != null || resolver.ListInstance.wim.CanContainSingleInstancePerDefinedList)
            {
                var formMaps = await GetFormMapsAsync().ConfigureAwait(false);
                
                // Only add the formmap to the output when we actually have any
                if (formMaps?.Count > 0)
                {
                    if (result.FormMaps == null)
                    {
                        result.FormMaps = new List<Transport.FormMap>();
                    }
                    result.FormMaps.AddRange(formMaps);
                }
            }

            // We are looking at the overview
            else if (resolver.ListID.HasValue)
            {
                int openInFrame = 0;
                if (resolver.Query.ContainsKey("openinframe"))
                {
                    openInFrame = Utils.ConvertToInt(resolver.Query["openinframe"].First(), 0);
                }

                if (resolver.ListInstance.wim.Console.Component == null)
                {
                    resolver.ListInstance.wim.Console.Component = new Beta.GeneratedCms.Source.Component();
                }

                //resolver.ListInstance.wim.Console.Component.CreateSearchList(resolver.ListInstance.wim.Console, openInFrame);
                resolver.ListInstance.wim.DoListSearch();

                // Only add the grids to the output when we actually have any
                var searchGrids = await GetGridsAsync().ConfigureAwait(false);
                if (searchGrids?.Count > 0)
                {
                    if (result.Grids == null)
                    {
                        result.Grids = new List<ListGrid>();
                    }
                    result.Grids.AddRange(searchGrids);
                }

                // Only add the formmap to the output when we actually have fields or buttons
                var searchFormMap = await GetFormMapListSearchAsync().ConfigureAwait(false);
                if (searchFormMap?.Buttons?.Count > 0 || searchFormMap?.Fields?.Count > 0)
                {
                    if (result.FormMaps == null)
                    {
                        result.FormMaps = new List<Transport.FormMap>();
                    }
                    result.FormMaps.Add(searchFormMap);
                }
            }

            // Only add the notifications to the output when we actually have any
            var notifications = GetNotifications();
            if (notifications?.Count > 0)
            {
                if (result.Notifications == null)
                {
                    result.Notifications = new List<Notification>();
                }
                result.Notifications.AddRange(notifications);
            }

            // Only add the resources to the output when we actually have any
            var resources = GetResources();
            if (resources?.Count > 0)
            {
                if (result.Resources == null)
                {
                    result.Resources = new List<ResourceItem>();
                }
                result.Resources.AddRange(resources);
            }
            result.SettingsURL = resolver.UrlBuild.GetListPropertiesRequest();
            result.IsEditMode = resolver.ListInstance.wim.IsEditMode;
            
            // The list title could have been changed along the way.
            if (string.IsNullOrWhiteSpace(resolver?.ListInstance?.wim?.ListTitle) == false)
            {
                result.Title = resolver.ListInstance.wim.ListTitle;
            }
            if (string.IsNullOrWhiteSpace(resolver?.ListInstance?.wim?.ListDescription) == false)
            {
                result.Description = resolver.ListInstance.wim.ListDescription;
            }

            return result;
        }

        #endregion Get List Response

        #region Get Sort

        Data.PageSortBy GetSort(int? sorderOrderMethod)
        {
            return sorderOrderMethod switch
            {
                1 => Data.PageSortBy.CustomDate,
                2 => Data.PageSortBy.CustomDateDown,
                3 => Data.PageSortBy.LinkText,
                4 => Data.PageSortBy.Name,
                _ => Data.PageSortBy.SortOrder,
            };
        }

        #endregion Get Sort

        #region Get Page Icon Classes

        private List<string> GetPageIconClasses(Data.Page inPage)
        {
            List<string> result = new List<string>();

            // Add the 'Published' icon class
            if (inPage.IsPublished)
            {
                result.Add("published");
            }
            // Add the 'Unpublished' icon class
            else
            {
                result.Add("unpublished");
            }

            // Add the 'Edit state' icon class
            if (inPage.IsEdited)
            {
                result.Add("edited");
            }

            // Has a master
            if (inPage.MasterID.HasValue)
            {
                if (inPage.InheritContentEdited == false && inPage.InheritContent == false)
                {
                    result.Add("inherited");
                }
                else if (inPage.InheritContentEdited == false && inPage.InheritContent)
                {
                    result.Add("inherited");
                    result.Add("unpublished");
                }
                else if (inPage.InheritContentEdited && inPage.InheritContent == false)
                {
                    result.Add("inherited");
                    result.Add("unpublished");
                    result.Add("edited");
                }
            }
            return result;
        }

        #endregion Get Page Icon Classes

        #region Get PageExplorer Response Async

        async Task<GetExplorerResponse> GetPageExplorerResponseAsync(bool isSearchInitiate, string filterTitle)
        {
            GetExplorerResponse result = new GetExplorerResponse();

            #region Folder navigation

            Data.Folder[] folders = null;

            bool isRootLevelView = false;
            if (isSearchInitiate || isRootLevelView)
            {
                isRootLevelView = true;
                folders = await Data.Folder.SelectAllAsync(_resolver.Folder.Type, _resolver.SiteID.GetValueOrDefault(), filterTitle, false).ConfigureAwait(false);
            }
            else
            {
                folders = await Data.Folder.SelectAllByParentAsync(_resolver.Folder.ID, _resolver.Folder.Type, false).ConfigureAwait(false);
            }

            //  ACL determination
            folders = await Data.Folder.ValidateAccessRightAsync(folders, _resolver.ApplicationUser).ConfigureAwait(false);

            if (_resolver.Folder.Level == 0 && folders.Length == 0 && !isRootLevelView && !isSearchInitiate)
            {
                isRootLevelView = true;
                folders = await Data.Folder.SelectAllAsync(_resolver.Folder.Type, _resolver.SiteID.GetValueOrDefault(), filterTitle, false).ConfigureAwait(false);
                //  ACL determination
                folders = await Data.Folder.ValidateAccessRightAsync(folders, _resolver.ApplicationUser).ConfigureAwait(false);
            }

            #endregion Folder navigation

            IEnumerable<Data.Page> pages;
            if (!isSearchInitiate)
            {
                pages = await Data.Page.SelectAllAsync(_resolver.Folder.ID, Data.PageFolderSortType.Folder, Data.PageReturnProperySet.All, GetSort(_resolver.Folder.ID), false).ConfigureAwait(false);
            }
            else
            {
                pages = await Data.Page.SelectAllAsync(filterTitle, false).ConfigureAwait(false);
            }

            pages = await Data.Page.ValidateAccessRightAsync(pages, _resolver.ApplicationUser).ConfigureAwait(false);

            if (pages.Any() == false && (folders.Length == 0 || (folders.Length == 1 && folders[0].Name == "/")))
            {
                return result;
            }


            if (pages.Any())
            {
                var container = new BrowseFolder()
                {
                    ID = _resolver.Folder.ID,
                    Title = _resolver.Folder.Name,
                    Description = _resolver.Folder.Description,
                    Href = $"{_resolver.WimPagePath}?folder={_resolver.Folder.ID}",
                    IconClasses = new List<string>() { "pagefolder" }
                };

                foreach (var entry in pages)
                {
                    container.Items.Add(new BrowseItem()
                    {
                        Href = $"{_resolver.WimPagePath}?page={entry.ID}",
                        Title = entry.Name,
                        ID = entry.ID,
                        IconClasses = GetPageIconClasses(entry)
                    });
                }
                result.Items.Add(container);
            }

            var otherFolders = await GetSubFoldersAsync(folders).ConfigureAwait(false);
            if (otherFolders?.Any() == true)
            {
                result.Items.AddRange(otherFolders);
            }

            return result;
        }

        #endregion Get PageExplorer Response Async

        #region Get SubFolders Async


        async Task<List<BrowseFolder>> GetSubFoldersAsync(Data.Folder[] folders)
        {
            List<BrowseFolder> result = new List<BrowseFolder>();

            foreach (Data.Folder entry in folders)
            {
                if ((entry.IsVisible == false && _resolver.ApplicationUser.ShowHidden == false) || (entry.Name == "/"))
                {
                    continue;
                }

                IEnumerable<Data.Page> pages = await Data.Page.SelectAllAsync(entry.ID, Data.PageFolderSortType.Folder, Data.PageReturnProperySet.All, GetSort(entry.SortOrderMethod), false).ConfigureAwait(false);
                pages = await Data.Page.ValidateAccessRightAsync(pages, _resolver.ApplicationUser).ConfigureAwait(false);

                BrowseFolder container = new BrowseFolder()
                {
                    Href = $"{_resolver.WimPagePath}?folder={entry.ID}",
                    Title = entry.Name,
                    ID = entry.ID,
                    Description = entry.Description,
                    IconClasses = new List<string>() { "pagefolder" }
                };

                if (pages.Any())
                {
                    foreach (var i in pages)
                    {
                        container.Items.Add(new BrowseItem()
                        {
                            Href = $"{_resolver.WimPagePath}?page={i.ID}",
                            Title = i.Name,
                            IconClasses = GetPageIconClasses(i),
                            ID = i.ID,
                        });
                    }
                }

                result.Add(container);

                var arr = await Data.Folder.SelectAllByParentAsync(entry.ID).ConfigureAwait(false);
                var results2 = await GetSubFoldersAsync(arr).ConfigureAwait(false);

                if (results2?.Any() == true) 
                {
                    result.AddRange(results2);
                }
            }

            return result;
        }

        #endregion Get SubFolders Async

        #region Get Gallery List Async

        async Task<GetExplorerResponse> GetGalleryExplorerAsync(bool isSearchInitiate, string filterTitle)
        {
            List<BrowseFolder> folders = new List<BrowseFolder>();

            var isOnlyImages = _resolver.Query.ContainsKey("isimage") && _resolver.Query["isimage"].Equals("1");
            var role = await _resolver.ApplicationUser.SelectRoleAsync().ConfigureAwait(false);
            int baseGalleryID = role.GalleryRoot.GetValueOrDefault();

            Data.Gallery[] galleries;
            Data.Gallery rootGallery = await Data.Gallery.SelectOneAsync(Utils.ConvertToGuid(_resolver.Query.GetValueOrDefault("root"))).ConfigureAwait(false);

            if (string.IsNullOrWhiteSpace(filterTitle))
            {
                if (_resolver.Gallery.ParentID.GetValueOrDefault(0) > 0 && _resolver.Gallery.ID != rootGallery.ID && _resolver.Gallery.ID != baseGalleryID)
                {
                    BrowseFolder container = new BrowseFolder()
                    {
                        ID = _resolver.Gallery.ParentID.Value,
                        Title = "...",
                        IconClasses = new List<string>()
                        {
                            "back",
                            "gallery"
                        },
                        Href = $"{_resolver.WimPagePath}?gallery={_resolver.Gallery.ParentID.Value}"
                    };

                    folders.Add(container);
                }

                galleries = await Data.Gallery.SelectAllByParentAsync(_resolver.Gallery.ID).ConfigureAwait(false);
            }
            else
            {
                galleries = await Data.Gallery.SelectAllAsync(filterTitle).ConfigureAwait(false);
            }

            bool isRootLevelView = false;
            //if (_resolver.GalleryID == .roo..Level == 0 && galleries.Length == 0 && !isSearchInitiate)
            //{
            //    isRootLevelView = true;
            //}

            foreach (Data.Gallery entry in galleries)
            {
                BrowseFolder newGalleryItem = new BrowseFolder()
                {
                    ID = entry.ID,
                    Title = isRootLevelView ? entry.CompleteCleanPath() : entry.Name,
                    IconClasses = new List<string>() { "gallery" },
                    Href = $"{_resolver.WimPagePath}?gallery={entry.ID}"
                };

                folders.Add(newGalleryItem);
            }

            List<Data.Asset> assets;

            if (string.IsNullOrEmpty(filterTitle))
            {
                assets = await Data.Asset.SelectAllAsync(_resolver.Gallery.ID, onlyReturnImages: isOnlyImages).ConfigureAwait(false);
            }
            else
            {
                assets = await Data.Asset.SearchAllAsync(filterTitle, onlyReturnImages: isOnlyImages).ConfigureAwait(false);
            }

            BrowseFolder rootFolder = new BrowseFolder();
            if (_resolver.GalleryID.GetValueOrDefault(0) > 0)
            {
                rootFolder.ID = _resolver.Gallery.ID;
                rootFolder.Title = _resolver.Gallery.Name;
                rootFolder.Href = $"{_resolver.WimPagePath}?gallery={_resolver.Gallery.ID}";
                rootFolder.IconClasses.Add("gallery");
            }

            foreach (Data.Asset entry in assets)
            {
                BrowseItem item = new BrowseItem()
                {
                    ID = entry.ID,
                    Title = entry.Title,
                    Href = $"{_resolver.WimPagePath}?asset={entry.ID}"
                };

                if (entry.IsImage)
                {
                    item.IconClasses.Add("image");
                }
                else
                {
                    item.IconClasses.Add("document");
                }

                rootFolder.Items.Add(item);
            }

            folders.Add(rootFolder);

            return new GetExplorerResponse()
            {
                Items = folders
            };
        }

        #endregion Get Gallery List Async

        public async Task<GetExplorerResponse> GetExplorerResponseAsync(UrlResolver resolver)
        {
            _resolver = resolver;
            GetExplorerResponse result = new GetExplorerResponse();

            // We are browsing lists
            if (resolver.Folder.Type == Data.FolderType.List || resolver.Folder.Type == Data.FolderType.Administration)
            {
                Data.Folder[] arr = new Data.Folder[] { resolver.Folder };

                result = await GetListExplorerResponseAsync(arr, false);
            }
            else if (resolver.Folder.Type == Data.FolderType.Page)
            {
                result = await GetPageExplorerResponseAsync(false, string.Empty);
            }
            else if (resolver.Folder.Type == Data.FolderType.Gallery)
            {
                result = await GetGalleryExplorerAsync(false, string.Empty);
            }

            return result;
        }
    }
}
