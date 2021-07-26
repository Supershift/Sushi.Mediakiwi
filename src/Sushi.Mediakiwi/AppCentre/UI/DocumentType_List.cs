using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Sushi.Mediakiwi.AppCentre.Data.Implementation;
using Sushi.Mediakiwi.AppCentre.UI.Forms;
using Sushi.Mediakiwi.Data;
using Sushi.Mediakiwi.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.AppCentre.UI
{
    public class DocumentType_List : BaseImplementation
    {
        public DocumentType_List()
        {
            ListPreRender += DocumentType_List_ListPreRender;
            ListLoad += DocumentType_List_ListLoad;
            ListSave += DocumentType_List_ListSave;
            ListSearch += DocumentType_List_ListSearch;
            ListDelete += DocumentType_List_ListDelete;
            wim.SetSortOrder("wim_Properties", "Property_Key", "Property_SortOrder");
        }

        private async Task DocumentType_List_ListDelete(ComponentListEventArgs e)
        {
            if (FieldPropertiesFormMapImplement == null)
            {
                return;
            }

            Property.TemplateID = e.SelectedKey;
            await Property.DeleteAsync().ConfigureAwait(false);
        }

        private async Task DocumentType_List_ListSearch(ComponentListSearchEventArgs e)
        {
            var documenttypes = await DocumentType.FetchAllAsync().ConfigureAwait(false);

            wim.ListDataColumns.Add(new ListDataColumn(null, nameof(DocumentType.ID), ListDataColumnType.UniqueIdentifier));
            wim.ListDataColumns.Add(new ListDataColumn("Document type", nameof(DocumentType.Name)));

            wim.ListDataAdd(documenttypes);
        }

        private async Task DocumentType_List_ListPreRender(ComponentListEventArgs e)
        {
            if (wim.IsSaveMode && FieldPropertiesFormMapImplement != null && string.IsNullOrEmpty(Property.Title))
            {
                wim.Notification.AddError(nameof(Property.Title), "Name required");
            }
            if (FieldPropertiesFormMapImplement != null)
            {
                bool isChoiceType =
                    Property.ContentTypeID.Equals(ContentType.Choice_Dropdown) ||
                    Property.ContentTypeID.Equals(ContentType.ListItemSelect) ||
                    Property.ContentTypeID.Equals(ContentType.Choice_Radio);

                if (isChoiceType)
                {
                    FieldPropertiesFormMapImplement.Find(x => x.Data).Show();
                }
            }
        }

        private async Task DocumentType_List_ListSave(ComponentListEventArgs e)
        {
            if (FieldPropertiesFormMapImplement == null)
            {
                var properties = await Property.SelectAllByTemplateAsync(e.SelectedKey).ConfigureAwait(false);

                List<MetaData> meta = new List<MetaData>();
                foreach (var property in properties)
                {
                    MetaData item = new MetaData();
                    item.Name = property.FieldName;
                    item.Title = property.Title;
                    item.InteractiveHelp = property.InteractiveHelp;
                    item.Mandatory = property.IsMandatory ? "1" : "0";
                    item.IsSharedField = property.IsSharedField ? "1" : "0";

                    if (property.MaxValueLength.HasValue)
                    {
                        item.MaxValueLength = $"{property.MaxValueLength.Value}";
                    }
                    else
                    {
                        item.MaxValueLength = null;
                    }

                    item.Default = property.DefaultValue;
                    item.AutoPostBack = property.AutoPostBack ? "1" : "0";
                    item.ContentTypeSelection = $"{(int)property.ContentTypeID}";

                    if (property.CanContainOneItem)
                    {
                        item.CanContainOneItem = "1";
                    }
                    else
                    {
                        item.CanContainOneItem = null;
                    }

                    item.Collection = property.ListCollection;

                    bool isChoiceType =
                        property.ContentTypeID.Equals(ContentType.Choice_Dropdown) ||
                        property.ContentTypeID.Equals(ContentType.ListItemSelect) ||
                        property.ContentTypeID.Equals(ContentType.Choice_Radio);

                    if (isChoiceType)
                    {
                        var propertyoptions = await PropertyOption.SelectAllAsync(property.ID).ConfigureAwait(false);
                        List<MetaDataList> options = new List<MetaDataList>();
                        foreach (var dataitem in propertyoptions)
                        {
                            options.Add(new MetaDataList(dataitem.Name, dataitem.Value));
                        }

                        item.CollectionList = options.ToArray();
                    }

                    meta.Add(item);
                }

                var serialized = Utility.GetSerialized(meta.ToArray());

                var ct = await Mediakiwi.Data.ComponentTemplate.SelectOneAsync(e.SelectedKey).ConfigureAwait(false);
                if (ct.MetaData != serialized)
                {
                    ct.MetaData = serialized;
                    ct.LastWriteTimeUtc = DateTime.UtcNow;
                    await ct.SaveAsync().ConfigureAwait(false);
                    wim.FlushCache();
                }

                var templates = await AvailableTemplate.SelectAllByComponentTemplateAsync(ct.ID).ConfigureAwait(false);
                var slot = templates.Where(x => x.SlotID.Equals(1)).ToList();
                if (slot.Any() == false)
                {
                    var at = new AvailableTemplate();
                    at.ComponentTemplateID = ct.ID;
                    at.SlotID = 1;
                    await at.SaveAsync().ConfigureAwait(false);
                }

            }
            else
            {
                JsonSerializer se = new JsonSerializer();

                string[] options = null;


                if (!string.IsNullOrWhiteSpace(Property.Data))
                {
                    try
                    {
                        using (var tr = new StringReader(Property.Data))
                        {
                            JsonTextReader reader = new JsonTextReader(tr);
                            options = se.Deserialize<string[]>(reader);
                        }
                    }
                    catch (Exception) { }
                }


                Property.Data = null;
                Property.TemplateID = e.SelectedKey;
                await Property.SaveAsync().ConfigureAwait(false);

                // Create Shared Field if it doesn't exist yet.
                await SharedField.CreateBasedOnPropertyAsync(Property).ConfigureAwait(false);

                bool isChoiceType =
                        Property.ContentTypeID.Equals(ContentType.Choice_Dropdown) ||
                        Property.ContentTypeID.Equals(ContentType.ListItemSelect) ||
                        Property.ContentTypeID.Equals(ContentType.Choice_Radio);

                if (options == null)
                {
                    var properties = await PropertyOption.SelectAllAsync(Property.ID).ConfigureAwait(false);
                    if (properties.Length > 0)
                    {
                        foreach (var item in properties)
                        {
                            await item.DeleteAsync().ConfigureAwait(false);
                        }
                    }
                }

                if (isChoiceType && options != null)
                {
                    List<PropertyOption> toCreate = new List<PropertyOption>();
                    List<PropertyOption> toRemove = new List<PropertyOption>();
                    var properties = await PropertyOption.SelectAllAsync(Property.ID).ConfigureAwait(false);

                    // Identify new
                    foreach (var item in options)
                    {
                        var searched = properties.FirstOrDefault(x => x.Value.Equals(item, StringComparison.InvariantCultureIgnoreCase));
                        if (searched == null)
                        {
                            toCreate.Add(new PropertyOption() { Value = item, Name = item, PropertyID = Property.ID });
                        }
                    }
                    foreach (var item in toCreate)
                    {
                        await item.SaveAsync().ConfigureAwait(false);
                    }

                    // Identify removable
                    foreach (var item in properties)
                    {
                        var searched = options.FirstOrDefault(x => x.Equals(item.Value, StringComparison.InvariantCultureIgnoreCase));
                        if (searched == null)
                        {
                            toRemove.Add(item);
                        }
                    }
                    foreach (var item in toRemove)
                    {
                        await item.DeleteAsync().ConfigureAwait(false);
                    }
                }
            }
        }

        Property Property;

        private async Task DocumentType_List_ListLoad(ComponentListEventArgs e)
        {
            if (!string.IsNullOrEmpty(Request.Query["field"]))
            {
                var id = Utility.ConvertToInt(Request.Query["field"]);

                Property = await Property.SelectOneAsync(id).ConfigureAwait(false);
                if (Property == null || Property?.ID == 0)
                {
                    Property = new Property();
                    Property.ContentTypeID = ContentType.TextField;
                }
                FieldPropertiesFormMapImplement = new FieldPropertiesForm(wim, Property);
                FormMaps.Add(FieldPropertiesFormMapImplement);
            }
            else
            {
                var ct = await Mediakiwi.Data.ComponentTemplate.SelectOneAsync(e.SelectedKey).ConfigureAwait(false);
                wim.ListTitle = ct.Name;

                //if (Implement == null)
                //    Implement = new DocumentType();
                //DocumentTypeFormMapImplement = new DocumentTypeFormMap(wim, Implement);
                //FormMaps.Add(DocumentTypeFormMapImplement);
            }
            // ARTICLE WILL BE REPLACED WITH THE VUE TEMPLATE
            wim.Page.Body.Add(@"<article id=""app""></article>", false, Body.BodyTarget.Nested);

            wim.Page.Head.AddStyle(CommonConfiguration.CDN_Folder(wim, "styles/documentType.min.css"));

            wim.Page.Head.Add($@"<script> 
var rootPath = {JsonConvert.SerializeObject(wim.AddApplicationPath("", true))};
var documentTypeID = {e.SelectedKey};
</script>");

            if (CommonConfiguration.IS_LOCAL_DEVELOPMENT)
            {
                wim.Page.Head.AddScript(CommonConfiguration.CDN_Folder(wim, "app/dist/document-type-app.js"));
            }
            else
            {
                wim.Page.Head.AddScript(CommonConfiguration.CDN_Folder(wim, "app/dist/document-type-app.min.js"));
            }
        }

        /// <summary>
        /// Create the default serializerSettings to pass everyting in camelcase to JS
        /// </summary>
        public JsonSerializerSettings JsonSerializerSettings { get; } = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };


        public int FieldID { get; set; }

        //public DocumentTypeFormMap DocumentTypeFormMapImplement { get; set; }

        public FieldPropertiesForm FieldPropertiesFormMapImplement { get; set; }
    }
}
