using Sushi.Mediakiwi.AppCentre.UI.Forms;
using Sushi.Mediakiwi.Data;
using Sushi.Mediakiwi.Framework;
using Sushi.Mediakiwi.UI;
using System;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.AppCentre.Data.Implementation
{
    public class PageMappingList : BaseImplementation
    {
        PageMappingForm Form { get; set; }



        public ListItemCollection Redirects
        {
            get
            {
                ListItemCollection lic = new ListItemCollection();
                lic.Add(new ListItem("Alle types", "0"));
                foreach (ListItem i in PageMappingForm.MappingTypes)
                {
                    lic.Add(i);
                }

                lic.Add(new ListItem("Bestand", "-2"));
                return lic;
            }
        }

        [Framework.ContentListSearchItem.Choice_Dropdown("Type redirect", nameof(Redirects))]
        public int Filter_TypeRedirect { get; set; }

        [Framework.ContentListSearchItem.Choice_Checkbox("Is actief")]
        public bool Filter_OnlyActive { get; set; }

        public IPageMapping Implement { get; set; }

        public PageMappingList()
        {
            ListSearch += PageMappingList_ListSearch;
            ListLoad += PageMappingList_ListLoad;
            ListSave += PageMappingList_ListSave;
            ListDelete += PageMappingList_ListDelete;
            ListDataItemCreated += PageMappingList_ListDataItemCreated;
        }

        private void PageMappingList_ListDataItemCreated(object sender, ListDataItemCreatedEventArgs e)
        {
            if (e.Item is IPageMapping pageMap && e.ColumnProperty == nameof(IPageMapping.MappingType))
            {
                string readableValue;
                if (pageMap.TargetType == PageMappingTargetType.FILE)
                {
                    readableValue = "File";
                }
                else
                {
                    readableValue = pageMap.MappingType switch
                    {
                        PageMappingType.Redirect302 => "Temporary redirect (302)",
                        PageMappingType.Redirect301 => "Permanent redirect (301)",
                        PageMappingType.NotFound404 => "Not found (404)",
                        PageMappingType.Rewrite200 => "Rewrite (200)",
                        _ => "",
                    };
                }

                e.InnerHTML = readableValue;
            }
        }

        private async Task PageMappingList_ListDelete(ComponentListEventArgs arg)
        {
            await Implement.DeleteAsync().ConfigureAwait(false);
        }

        private async Task PageMappingList_ListSave(ComponentListEventArgs arg)
        {
            if (!string.IsNullOrEmpty(Implement.Query) && !Implement.Query.StartsWith("?"))
            {
                Implement.Query = string.Concat("?", Implement.Query);
            }

            if (Implement.Path != null && !Implement.Path.StartsWith("/"))
            {
                Implement.Path = string.Concat("/", Implement.Path);
            }

            if (Implement.ID < 1)
            {
                Implement.Created = DateTime.Now;
            }

            await Implement.SaveAsync().ConfigureAwait(false);
        }

        private async Task PageMappingList_ListLoad(ComponentListEventArgs e)
        {
            Implement = await PageMapping.SelectOneAsync(e.SelectedKey).ConfigureAwait(false);
            if (Implement == null)
            {
                Implement = new PageMapping();
            }

            if (IsPostBack)
            {
                if (Request.Form[nameof(IPageMapping.TargetTypeID)] == "0")
                {
                    Implement.TargetTypeID = 0;
                }
                else if (Request.Form[nameof(IPageMapping.TargetTypeID)] == "1")
                {
                    Implement.TargetTypeID = 1;
                }
                else if (Request.Form[nameof(IPageMapping.TargetTypeID)] == "2")
                {
                    Implement.TargetTypeID = 2;
                }
            }

            Form = new PageMappingForm(Implement);
            FormMaps.Add(Form);
        }

        private async Task PageMappingList_ListSearch(ComponentListSearchEventArgs arg)
        {
            wim.ListDataColumns.Add(new ListDataColumn("ID", nameof(IPageMapping.ID), ListDataColumnType.UniqueIdentifier));
            wim.ListDataColumns.Add(new ListDataColumn("URL", nameof(IPageMapping.Path), ListDataColumnType.HighlightPresent));
            wim.ListDataColumns.Add(new ListDataColumn("Mapped to", nameof(IPageMapping.TargetURL)));
            wim.ListDataColumns.Add(new ListDataColumn("Type", nameof(IPageMapping.MappingType)) { Alignment = Align.Left });
            wim.ListDataColumns.Add(new ListDataColumn("Active", nameof(IPageMapping.IsActive)) { ColumnWidth = 30 });

            wim.ListDataAdd(await PageMapping.SelectAllAsync(Filter_TypeRedirect, Filter_OnlyActive).ConfigureAwait(false));
        }
    }
}
