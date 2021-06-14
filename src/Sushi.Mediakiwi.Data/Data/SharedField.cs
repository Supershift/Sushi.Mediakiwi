using Sushi.MicroORM;
using Sushi.MicroORM.Mapping;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Data
{
    [DataMap(typeof(SharedFieldMap))]
    public class SharedField
    {
        public class SharedFieldMap : DataMap<SharedField>
        {
            public SharedFieldMap()
            {
                Table("wim_SharedFields");
                Id(x => x.ID, "SharedField_Key").Identity();
                Map(x => x.ContentTypeID, "SharedField_ContentTypeID");
                Map(x => x.FieldName, "SharedField_FieldName").SqlType(SqlDbType.NVarChar);
                Map(x => x.IsHiddenOnPage, "SharedField_IsHiddenOnPage");
            }
        }

        public int ID { get; set; }
        public ContentType ContentTypeID { get; set; }
        public string FieldName { get; set; }
        public bool IsHiddenOnPage { get; set; }

        public string List_ContentType
        {
            get
            {
                switch (ContentTypeID)
                {
                    case ContentType.FileUpload:
                        return "File upload";
                    case ContentType.Binary_Document:
                    case ContentType.DocumentSelect:
                        return "Document";
                    case ContentType.Binary_Image:
                        return "Image";
                    case ContentType.FolderSelect:
                        return "Folder";
                    case ContentType.Hyperlink:
                        return "Link";
                    case ContentType.PageSelect:
                        return "Page";
                    case ContentType.Choice_Checkbox:
                        return "Checkbox";
                    case ContentType.Date:
                        return "Date";
                    case ContentType.DateTime:
                        return "Datetime";
                    case ContentType.MultiField:
                        return "Multifield";
                    case ContentType.RichText:
                        return "Richtext";
                    case ContentType.Sourcecode:
                        return "Code";
                    case ContentType.TextArea:
                        return "Textarea";
                    case ContentType.TextField:
                        return "TextField";
                    case ContentType.TextLine:
                        return "TextLine";
                    case ContentType.TextDate:
                        return "TextDate";
                    case ContentType.SubListSelect:
                        return "Sublistselect";
                    case ContentType.Section:
                        return "Section";
                    case ContentType.MultiImageSelect:
                        return "Multi image";
                    case ContentType.MultiAssetUpload:
                        return "Multi document";
                    case ContentType.ListItemSelect:
                        return "Listitem select";
                    case ContentType.Choice_Dropdown:
                        return "Dropdown";
                    case ContentType.Choice_Radio:
                        return "Radio";
                    case ContentType.HtmlContainer:
                        return "HTML";
                    default:
                    case ContentType.Undefined:
                        return "?";
                }
            }
        }
        public bool List_IsPublished
        {
            get
            {
                if (ID > 0 && string.IsNullOrWhiteSpace(List_EditValue) == false && string.IsNullOrWhiteSpace(List_PublishedValue) == false)
                {
                    return List_EditValue.Equals(List_PublishedValue, System.StringComparison.InvariantCulture);
                }
                return false;
            }
        }

        public int List_PageCount { get; set; }
        public string List_PublishedValue { get; set; }
        public string List_EditValue { get; set; }


        public static ICollection<SharedField> FetchAll()
        {
            var connector = new Connector<SharedField>();
            var filter = connector.CreateDataFilter();
            filter.AddOrder(x => x.FieldName);
            var result = connector.FetchAll(filter);
            return result;
        }

        public static async Task<ICollection<SharedField>> FetchAllAsync()
        {
            return await FetchAllAsync(null);
        }

        public static async Task<ICollection<SharedField>> FetchAllAsync(string search)
        {
            var connector = new Connector<SharedField>();
            var filter = connector.CreateDataFilter();
            if (string.IsNullOrWhiteSpace(search) == false)
            {
                filter.Add(x => x.FieldName, $"%{search}%", ComparisonOperator.Like);
            }
            filter.AddOrder(x => x.FieldName);
            var result = await connector.FetchAllAsync(filter).ConfigureAwait(false);
            return result;
        }

        public static SharedField FetchSingle(int id)
        {
            var connector = new Connector<SharedField>();
            var result = connector.FetchSingle(id);
            return result;
        }

        public static async Task<SharedField> FetchSingleAsync(int id)
        {
            var connector = new Connector<SharedField>();
            var result = await connector.FetchSingleAsync(id).ConfigureAwait(false);
            return result;
        }


        public static SharedField FetchSingle(string fieldName, ContentType contentType)
        {
            var connector = new Connector<SharedField>();
            var filter = connector.CreateDataFilter();

            filter.Add(x => x.FieldName, fieldName);
            filter.Add(x => x.ContentTypeID, contentType);

            var result = connector.FetchSingle(filter);
            return result;
        }

        public static async Task<SharedField> FetchSingleAsync(string fieldName, ContentType contentType)
        {
            var connector = new Connector<SharedField>();
            var filter = connector.CreateDataFilter();

            filter.Add(x => x.FieldName, fieldName);
            filter.Add(x => x.ContentTypeID, contentType);

            var result = await connector.FetchSingleAsync(filter).ConfigureAwait(false);
            return result;
        }

        public static SharedField FetchSingleForComponentTemplate(string fieldName, ContentType contentType, int componentTemplateId)
        {
            var connector = new Connector<SharedField>();
            var filter = connector.CreateDataFilter();

            var props = Property.SelectAllByTemplate(componentTemplateId);
            var matchingProp = props.FirstOrDefault(x => x.FieldName == fieldName && x.IsSharedField);
            if (matchingProp?.ID > 0) 
            {
                filter.Add(x => x.FieldName, fieldName);
                filter.Add(x => x.ContentTypeID, contentType);

                return connector.FetchSingle(filter);
            }

            return default(SharedField);
        }

        public static async Task<SharedField> FetchSingleForComponentTemplateAsync(string fieldName, ContentType contentType, int componentTemplateId)
        {
            var connector = new Connector<SharedField>();
            var filter = connector.CreateDataFilter();

            var props = await Property.SelectAllByTemplateAsync(componentTemplateId);
            var matchingProp = props.FirstOrDefault(x => x.FieldName == fieldName && x.IsSharedField);
            if (matchingProp?.ID > 0)
            {
                filter.Add(x => x.FieldName, fieldName);
                filter.Add(x => x.ContentTypeID, contentType);

                return await connector.FetchSingleAsync(filter);
            }

            return default(SharedField);
        }

        public void Save()
        {
            var connector = new Connector<SharedField>();
            connector.Save(this);
        }

        public async Task SaveAsync()
        {
            var connector = new Connector<SharedField>();
            await connector.SaveAsync(this).ConfigureAwait(false);
        }

        public void Delete()
        {
            var connector = new Connector<SharedField>();
            connector.Delete(this);
        }

        public async Task DeleteAsync()
        {
            var connector = new Connector<SharedField>();
            await connector.DeleteAsync(this).ConfigureAwait(false);
        }
    }
}
