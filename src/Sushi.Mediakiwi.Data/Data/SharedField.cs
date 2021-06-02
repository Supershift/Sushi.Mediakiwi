using Sushi.MicroORM;
using Sushi.MicroORM.Mapping;
using System.Collections.Generic;
using System.Data;
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
            }
        }

        public int ID { get; set; }
        public int ContentTypeID { get; set; }
        public string FieldName { get; set; }

        public string List_ContentType
        {
            get
            {
                switch (ContentTypeID)
                {
                    case (int)ContentType.FileUpload:
                        return "File upload";
                    case (int)ContentType.Binary_Document:
                    case (int)ContentType.DocumentSelect:
                        return "Document";
                    case (int)ContentType.Binary_Image:
                        return "Image";
                    case (int)ContentType.FolderSelect:
                        return "Folder";
                    case (int)ContentType.Hyperlink:
                        return "Link";
                    case (int)ContentType.PageSelect:
                        return "Page";
                    case (int)ContentType.Choice_Checkbox:
                        return "Checkbox";
                    case (int)ContentType.Date:
                        return "Date";
                    case (int)ContentType.DateTime:
                        return "Datetime";
                    case (int)ContentType.MultiField:
                        return "Multifield";
                    case (int)ContentType.RichText:
                        return "Richtext";
                    case (int)ContentType.Sourcecode:
                        return "Code";
                    case (int)ContentType.TextArea:
                        return "Textarea";
                    case (int)ContentType.TextField:
                        return "TextField";
                    case (int)ContentType.TextLine:
                        return "TextLine";
                    case (int)ContentType.TextDate:
                        return "TextDate";
                    case (int)ContentType.SubListSelect:
                        return "Sublistselect";
                    case (int)ContentType.Section:
                        return "Section";
                    case (int)ContentType.MultiImageSelect:
                        return "Multi image";
                    case (int)ContentType.MultiAssetUpload:
                        return "Multi document";
                    case (int)ContentType.ListItemSelect:
                        return "Listitem select";
                    case (int)ContentType.Choice_Dropdown:
                        return "Dropdown";
                    case (int)ContentType.Choice_Radio:
                        return "Radio";
                    case (int)ContentType.HtmlContainer:
                        return "HTML";
                    default:
                    case (int)ContentType.Undefined:
                        return "?";
                }
            }
        }
        public bool List_IsPublished
        {
            get
            {
                if (ID > 0)
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


        public static SharedField FetchSingle(string fieldName)
        {
            var connector = new Connector<SharedField>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.FieldName, fieldName);
            var result = connector.FetchSingle(filter);
            return result;
        }

        public static async Task<SharedField> FetchSingleAsync(string fieldName)
        {
            var connector = new Connector<SharedField>();
            var filter = connector.CreateDataFilter();

            filter.Add(x => x.FieldName, fieldName);
            var result = await connector.FetchSingleAsync(filter).ConfigureAwait(false);
            return result;
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

        public async Task DeleteAsnc()
        {
            var connector = new Connector<SharedField>();
            await connector.DeleteAsync(this).ConfigureAwait(false);
        }
    }
}
