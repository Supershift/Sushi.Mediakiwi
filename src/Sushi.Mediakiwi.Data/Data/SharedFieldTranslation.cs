using Sushi.MicroORM;
using Sushi.MicroORM.Mapping;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Data
{
    [DataMap(typeof(SharedFieldTranslationMap))]
    public class SharedFieldTranslation
    {
        public class SharedFieldTranslationMap : DataMap<SharedFieldTranslation>
        {
            public SharedFieldTranslationMap(bool isSaveMode = true)
            {
                if (isSaveMode)
                {
                    Table("wim_SharedFieldTranslations");
                }
                else
                {
                    Table("vw_SharedFields");
                    Map(x => x.FieldName, "SharedField_FieldName").ReadOnly();
                    Map(x => x.ContentTypeID, "SharedField_ContentTypeID").ReadOnly();
                }

                Id(x => x.ID, "SharedFieldTranslation_Key").Identity();
                Map(x => x.FieldID, "SharedFieldTranslation_Field_Key");
                Map(x => x.SiteID, "SharedFieldTranslation_Site_Key");
                Map(x => x.EditValue, "SharedFieldTranslation_EditValue").SqlType(SqlDbType.NVarChar);
                Map(x => x.Value, "SharedFieldTranslation_Value").SqlType(SqlDbType.NVarChar);
            }
        }

        public int ID { get; set; }
        public int SiteID { get; set; }
        public int FieldID { get; set; }
        public int ContentTypeID { get; set; }
        public string FieldName { get; set; }
        public string EditValue { get; set; }
        public string Value { get; set; }

        public string GetEditValue(int? maxChars = null)
        {
            var value = GetValue(EditValue);
            if (maxChars.GetValueOrDefault(0) > 0)
            {
                value = Utility.ConvertToFixedLengthText(value, maxChars.Value, "&hellip;");
            }

            return value;
        }

        public string GetPublishedValue(int? maxChars = null)
        {
            var value = GetValue(Value);
            if (maxChars.GetValueOrDefault(0) > 0)
            {
                value = Utility.ConvertToFixedLengthText(value, maxChars.Value, "&hellip;");
            }

            return value;
        }

        private string GetValue(string _value)
        {
            string returnValue = "";
            System.Globalization.CultureInfo dateCulture = new System.Globalization.CultureInfo("nl-NL");

            switch (ContentTypeID)
            {
                case (int)ContentType.FileUpload:
                case (int)ContentType.DocumentSelect:
                case (int)ContentType.Binary_Document:
                    {
                        int valueID = Utility.ConvertToInt(_value, 0);
                        if (valueID > 0)
                        {
                            returnValue = Document.SelectOne(valueID).RemoteLocation;
                        }
                    }
                    break;
                case (int)ContentType.Binary_Image:
                    {
                        int valueID = Utility.ConvertToInt(_value, 0);
                        if (valueID > 0)
                        {
                            returnValue = Image.SelectOne(valueID).RemoteLocation;
                        }
                    }
                    break;
                case (int)ContentType.Choice_Checkbox:
                    {
                        returnValue = (_value == "1").ToString();
                    }
                    break;
                case (int)ContentType.Date:
                    {
                        if (DateTime.TryParseExact(_value, "dd-MM-yyyy", dateCulture, System.Globalization.DateTimeStyles.None, out DateTime result))
                        {
                            returnValue = result.ToString("dd-MM-yyyy");
                        }
                    }
                    break;
                case (int)ContentType.DateTime:
                    {
                        if (DateTime.TryParseExact(_value, "dd-MM-yyyy HH:mm:ss", dateCulture, System.Globalization.DateTimeStyles.None, out DateTime result))
                        {
                            returnValue = result.ToString("dd-MM-yyyy HH:mm:ss");
                        }
                    }
                    break;
                case (int)ContentType.FolderSelect:
                    {
                        int valueID = Utility.ConvertToInt(_value, 0);
                        if (valueID > 0)
                        {
                            returnValue = Folder.SelectOne(valueID).CompletePath;
                        }
                    }
                    break;
                case (int)ContentType.Hyperlink:
                    {
                        int valueID = Utility.ConvertToInt(_value, 0);
                        if (valueID > 0)
                        {
                            returnValue = Link.SelectOne(valueID).GetHrefAsync().ConfigureAwait(false).GetAwaiter().GetResult();
                        }
                    }
                    break;
                case (int)ContentType.MultiField:
                    {
                        returnValue = "MultiField";
                    }
                    break;
                case (int)ContentType.PageSelect:
                    {
                        int valueID = Utility.ConvertToInt(_value, 0);
                        if (valueID > 0)
                        {
                            returnValue = Page.SelectOne(valueID).InternalPath;
                        }
                    }
                    break;
                case (int)ContentType.RichText:
                case (int)ContentType.Sourcecode:
                case (int)ContentType.TextArea:
                case (int)ContentType.TextField:
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
                        returnValue = _value;
                    }
                    break;
            }

            return returnValue;

        }

        public static ICollection<SharedFieldTranslation> FetchAll()
        {
            var connector = new Connector<SharedFieldTranslation>(new SharedFieldTranslationMap(false));
            var filter = connector.CreateDataFilter();
            var result = connector.FetchAll(filter);
            return result;
        }

        public static async Task<ICollection<SharedFieldTranslation>> FetchAllAsync()
        {
            var connector = new Connector<SharedFieldTranslation>(new SharedFieldTranslationMap(false));
            var filter = connector.CreateDataFilter();
            var result = await connector.FetchAllAsync(filter).ConfigureAwait(false);
            return result;
        }

        public static ICollection<SharedFieldTranslation> FetchAllForField(int sharedFieldID)
        {
            var connector = new Connector<SharedFieldTranslation>(new SharedFieldTranslationMap(false));
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.FieldID, sharedFieldID);
            var result = connector.FetchAll(filter);
            return result;
        }

        public static async Task<ICollection<SharedFieldTranslation>> FetchAllForFieldAsync(int sharedFieldID)
        {
            var connector = new Connector<SharedFieldTranslation>(new SharedFieldTranslationMap(false));
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.FieldID, sharedFieldID);
            var result = await connector.FetchAllAsync(filter).ConfigureAwait(false);
            return result;
        }

        public static ICollection<SharedFieldTranslation> FetchAllForSite(int siteId)
        {
            var connector = new Connector<SharedFieldTranslation>(new SharedFieldTranslationMap(false));
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.SiteID, siteId);
            var result = connector.FetchAll(filter);
            return result;
        }

        public static async Task<ICollection<SharedFieldTranslation>> FetchAllForSiteAsync(int siteId)
        {
            var connector = new Connector<SharedFieldTranslation>(new SharedFieldTranslationMap(false));
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.SiteID, siteId);
            var result = await connector.FetchAllAsync(filter).ConfigureAwait(false);
            return result;
        }

        public static SharedFieldTranslation FetchSingle(int id)
        {
            var connector = new Connector<SharedFieldTranslation>(new SharedFieldTranslationMap(false));
            var result = connector.FetchSingle(id);
            return result;
        }

        public static SharedFieldTranslation FetchSingleForFieldAndSite(int sharedFieldId, int siteId)
        {
            var connector = new Connector<SharedFieldTranslation>(new SharedFieldTranslationMap(false));
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.SiteID, siteId);
            filter.Add(x => x.FieldID, sharedFieldId);
            var result = connector.FetchSingle(filter);
            return result;
        }

        public static async Task<SharedFieldTranslation> FetchSingleForFieldAndSiteAsync(int sharedFieldId, int siteId)
        {
            var connector = new Connector<SharedFieldTranslation>(new SharedFieldTranslationMap(false));
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.SiteID, siteId);
            filter.Add(x => x.FieldID, sharedFieldId);
            var result = await connector.FetchSingleAsync(filter).ConfigureAwait(false);
            return result;
        }

        public void Save()
        {
            var connector = new Connector<SharedFieldTranslation>(new SharedFieldTranslationMap(true));
            connector.Save(this);
        }


        public async Task SaveAsync()
        {
            var connector = new Connector<SharedFieldTranslation>(new SharedFieldTranslationMap(true));
            await connector.SaveAsync(this).ConfigureAwait(false);
        }

        public void Delete()
        {
            var connector = new Connector<SharedFieldTranslation>(new SharedFieldTranslationMap(true));
            connector.Delete(this);
        }

        public async Task DeleteAsync()
        {
            var connector = new Connector<SharedFieldTranslation>(new SharedFieldTranslationMap(true));
            await connector.DeleteAsync(this).ConfigureAwait(false);
        }

        public async Task RevertAsync()
        {
            EditValue = Value;
            await SaveAsync().ConfigureAwait(false);
        }
    }
}
