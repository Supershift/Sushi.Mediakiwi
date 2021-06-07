using Sushi.Mediakiwi.AppCentre.Data.Implementation;
using Sushi.Mediakiwi.Data;
using Sushi.Mediakiwi.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.AppCentre.Data.Implementation
{
    public class SharedFieldList : BaseImplementation
    {

        public static int LAYER_WIDTH = 800;
        public static int LAYER_HEIGHT = 750;

        #region Properties

        SharedFieldTranslation Implement;
        UI.Forms.SharedFieldFormMap FormMap;

        #endregion Properties

        #region FE UI Search Elements

        [Framework.ContentListSearchItem.TextField("Search", 50, false)]
        public string Filter_Search { get; set; }

        #endregion FE UI Search Elements

        #region CTor

        public SharedFieldList()
        {
            wim.OpenInEditMode = true;
            wim.HideCreateNew = true;
            wim.CanSaveAndAddNew = false;
            wim.HideSaveButtons = true;
            wim.HideExportOptions = true;

            ListAction += SharedFieldList_ListAction;
            ListSearch += SharedFieldList_ListSearch;
            ListLoad += SharedFieldList_ListLoad;
            ListSave += SharedFieldList_ListSave;
        }

        #endregion CTor

        #region List Action

        private async Task SharedFieldList_ListAction(ComponentActionEventArgs arg)
        {
            if (Button_Delete)
            {
                await Implement.DeleteAsync().ConfigureAwait(false);
                wim.Page.Body.Form.RefreshParent();
            }
            if (Button_Revert)
            {
                await Implement.RevertAsync().ConfigureAwait(false);
                wim.Page.Body.Form.RefreshParent();
            }
        }

        #endregion List Action

        #region List Save

        private async Task SharedFieldList_ListSave(ComponentListEventArgs arg)
        {
            if (FormMap != null)
            {
                if (Button_SavePublish)
                {
                   await FormMap.SaveAsync(true).ConfigureAwait(false);
                }
                else
                {
                   await FormMap.SaveAsync().ConfigureAwait(false);
                }
            }
        }

        #endregion List Save

        #region List Load

        private async Task SharedFieldList_ListLoad(ComponentListEventArgs arg)
        {
            Implement = await SharedFieldTranslation.FetchSingleForFieldAndSiteAsync(arg.SelectedKey, wim.CurrentSite.ID).ConfigureAwait(false);
            SharedField field = await SharedField.FetchSingleAsync(arg.SelectedKey);

            if (Implement == null || Implement.ID == 0)
            {
                Implement = new SharedFieldTranslation()
                {
                    ContentTypeID = field.ContentTypeID,
                    FieldID = field.ID,
                    SiteID = wim.CurrentSite.ID,
                    FieldName = field.FieldName
                };
            }

            FormMap = new UI.Forms.SharedFieldFormMap(wim, Implement);
            FormMaps.Add(FormMap);
        }

        #endregion List Load

        #region List Search

        private async Task SharedFieldList_ListSearch(ComponentListSearchEventArgs arg)
        {
            wim.ListDataColumns.Add(new ListDataColumn("ID", nameof(SharedField.ID), ListDataColumnType.UniqueIdentifier));
            wim.ListDataColumns.Add(new ListDataColumn("Field", nameof(SharedField.FieldName), ListDataColumnType.HighlightPresent));
            wim.ListDataColumns.Add(new ListDataColumn("Type", nameof(SharedField.List_ContentType), ListDataColumnType.Default));
            wim.ListDataColumns.Add(new ListDataColumn("Edit value", nameof(SharedField.List_EditValue), ListDataColumnType.Default));
            wim.ListDataColumns.Add(new ListDataColumn("Published value", nameof(SharedField.List_PublishedValue), ListDataColumnType.Default));
            wim.ListDataColumns.Add(new ListDataColumn("Published", nameof(SharedField.List_IsPublished), ListDataColumnType.Default));
            wim.ListDataColumns.Add(new ListDataColumn("Pages", nameof(SharedField.List_PageCount), ListDataColumnType.Default) { Alignment = Align.Left });

            var allFields = await SharedField.FetchAllAsync().ConfigureAwait(false);
            foreach (var field in allFields)
            {
                var fieldData = await SharedFieldTranslation.FetchSingleForFieldAndSiteAsync(field.ID, wim.CurrentSite.ID).ConfigureAwait(false);
                
                // We don't have any data yet
                if (fieldData == null || fieldData?.ID == 0)
                {
                    fieldData = new SharedFieldTranslation()
                    {
                        ContentTypeID = field.ContentTypeID,
                        EditValue = "",
                        FieldID = field.ID,
                        FieldName = field.FieldName,
                        SiteID = wim.CurrentSite.ID,
                        Value = ""
                    };
                }
                var matchingProps = await Property.SelectAllByFieldNameAsync(field.FieldName).ConfigureAwait(false);
                if (matchingProps?.Count > 0)
                {
                    foreach (var prop in matchingProps.Where(x => x.TemplateID > 0))
                    {
                        var cVersions = ComponentVersion.SelectAllForTemplate(prop.TemplateID);
                        var pages = Page.SelectAll(cVersions.Select(x => x.PageID.GetValueOrDefault(0)).ToArray());
                        field.List_PageCount = pages.Length;
                    }
                }
                
                field.List_EditValue = fieldData.GetEditValue(50);
                field.List_PublishedValue = fieldData.GetPublishedValue(50);
            }

            // Apply search filter across field + fielddata
            if (string.IsNullOrWhiteSpace(Filter_Search) == false)
            {
                allFields = allFields.Where(
                    x => x.FieldName.Contains(Filter_Search, StringComparison.InvariantCultureIgnoreCase) ||
                    x.List_PublishedValue.Contains(Filter_Search, StringComparison.InvariantCultureIgnoreCase) ||
                    x.List_EditValue.Contains(Filter_Search, StringComparison.InvariantCultureIgnoreCase)).ToList();
            }

            wim.ListDataAdd(allFields);

            wim.Page.Body.Grid.SetClickLayer(new Grid.LayerSpecification()
            {
                Height = LAYER_HEIGHT,
                Width = LAYER_WIDTH,
                Title = "Shared field"
            });
        }

 
        #endregion List Search

        #region FE UI Buttons


        [Framework.ContentListItem.Button("Revert"
            , AskConfirmation = true
            , ConfirmationAcceptLabel = "Confirm"
            , ConfirmationRejectLabel = "Cancel"
            , ConfirmationQuestion = "Are you sure you want to revert to the published version ?"
            , ConfirmationTitle = "Are you sure ?"
            , IconTarget = ButtonTarget.TopLeft)]
        public bool Button_Revert { get; set; }


        [Framework.ContentListItem.Button("Delete item"
     , AskConfirmation = true
     , ConfirmationAcceptLabel = "Confirm"
     , ConfirmationQuestion = "Are you sure you want to delete this Item ?"
     , ConfirmationRejectLabel = "Cancel"
     , ConfirmationTitle = "Are you sure ?"
     , IconTarget = ButtonTarget.TopLeft
     , IsPrimary = false
     )]
        public bool Button_Delete { get; set; }

        [Framework.ContentListItem.Button("Save", true, IconTarget = ButtonTarget.BottomRight)]
        public bool Button_Save { get; set; }

        [Framework.ContentListItem.Button("Save & Publish", true, IsPrimary = true, IconTarget = ButtonTarget.BottomRight)]
        public bool Button_SavePublish { get; set; }

        #endregion FE UI Buttons
    }
}
