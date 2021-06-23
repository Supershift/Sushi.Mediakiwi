using Sushi.Mediakiwi.Data.Data;
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
        public ContentType ContentTypeID { get; set; }
        public string FieldName { get; set; }
        public string EditValue { get; set; }
        public string Value { get; set; }

        /// <summary>
        /// Returns the published value 
        /// </summary>
        /// <returns></returns>
        public string GetPublishedValue() => GetPublishedValue(false, null);

        /// <summary>
        /// Returns the published value
        /// </summary>
        /// <param name="optimizeForDisplay">When TRUE, the output will be optimized for display</param>
        /// <returns></returns>
        public string GetPublishedValue(bool optimizeForDisplay) => GetPublishedValue(optimizeForDisplay, null);

        /// <summary>
        /// Returns the published value
        /// </summary>
        /// <param name="optimizeForDisplay">When TRUE, the output will be optimized for display</param>
        /// <param name="maxChars">Maximum chars to return</param>
        /// <returns></returns>
        public string GetPublishedValue(bool optimizeForDisplay, int? maxChars) => GetValue(Value, optimizeForDisplay, maxChars);


        /// <summary>
        /// Returns the edit value 
        /// </summary>
        /// <returns></returns>
        public string GetEditValue() => GetEditValue(false, null);

        /// <summary>
        /// Returns the edit value
        /// </summary>
        /// <param name="optimizeForDisplay">When TRUE, the output will be optimized for display</param>
        /// <returns></returns>
        public string GetEditValue(bool optimizeForDisplay) => GetEditValue(optimizeForDisplay, null);

        /// <summary>
        /// Returns the edit value
        /// </summary>
        /// <param name="optimizeForDisplay">When TRUE, the output will be optimized for display</param>
        /// <param name="maxChars">Maximum chars to return</param>
        /// <returns></returns>
        public string GetEditValue(bool optimizeForDisplay, int? maxChars) => GetValue(EditValue, optimizeForDisplay, maxChars);


        private string GetValue(string _value, bool optimizeForDisplay, int? maxChars)
        {
            string returnValue = "";
            System.Globalization.CultureInfo dateCulture = new System.Globalization.CultureInfo("nl-NL");

            switch (ContentTypeID)
            {
                case ContentType.FileUpload:
                case ContentType.DocumentSelect:
                case ContentType.Binary_Document:
                    {
                        if (string.IsNullOrWhiteSpace(_value) == false)
                        {
                            int valueID = Utility.ConvertToInt(_value, 0);
                            if (valueID > 0)
                            {
                                var doc = Document.SelectOne(valueID);
                                if (optimizeForDisplay == false)
                                {
                                    returnValue = (doc?.ID > 0) ? doc.ID.ToString() : "";
                                }
                                else
                                {
                                    returnValue = (doc?.ID > 0) ? doc.RemoteLocation : "";
                                }
                            }
                        }
                    }
                    break;
                case ContentType.Binary_Image:
                    {
                        if (string.IsNullOrWhiteSpace(_value) == false)
                        {
                            int valueID = Utility.ConvertToInt(_value, 0);
                            if (valueID > 0)
                            {
                                var doc = Image.SelectOne(valueID);
                                if (optimizeForDisplay == false)
                                {
                                    returnValue = (doc?.ID > 0) ? doc.ID.ToString() : "";
                                }
                                else
                                {
                                    returnValue = (doc?.ID > 0) ? doc.RemoteLocation : "";
                                }
                            }
                        }
                    }
                    break;
                case ContentType.Choice_Checkbox:
                    {
                        returnValue = (_value == "1").ToString();
                    }
                    break;
                case ContentType.Date:
                    {
                        if (DateTime.TryParseExact(_value, "dd-MM-yyyy", dateCulture, System.Globalization.DateTimeStyles.None, out DateTime result))
                        {
                            returnValue = result.ToString("dd-MM-yyyy");
                        }
                    }
                    break;
                case ContentType.DateTime:
                    {
                        if (DateTime.TryParseExact(_value, "dd-MM-yyyy HH:mm:ss", dateCulture, System.Globalization.DateTimeStyles.None, out DateTime result))
                        {
                            returnValue = result.ToString("dd-MM-yyyy HH:mm:ss");
                        }
                    }
                    break;
                case ContentType.FolderSelect:
                    {
                        if (string.IsNullOrWhiteSpace(_value) == false)
                        {
                            int valueID = Utility.ConvertToInt(_value, 0);
                            if (valueID > 0)
                            {
                                returnValue = Folder.SelectOne(valueID).CompletePath;
                            }
                        }
                    }
                    break;
                case ContentType.Hyperlink:
                    {
                        if (string.IsNullOrWhiteSpace(_value) == false)
                        {
                            int valueID = Utility.ConvertToInt(_value, 0);
                            if (valueID > 0)
                            {
                                returnValue = Link.SelectOne(valueID).GetHrefAsync().ConfigureAwait(false).GetAwaiter().GetResult();
                            }
                        }
                    }
                    break;
                case ContentType.MultiField:
                    {
                        returnValue = "MultiField";
                        var mfs = MultiField.GetDeserialized(_value);
                        if (mfs != null)
                        {
                            returnValue = _value;
                        }
                    }
                    break;
                case ContentType.PageSelect:
                    {
                        if (string.IsNullOrWhiteSpace(_value) == false)
                        {
                            int valueID = Utility.ConvertToInt(_value, 0);
                            if (valueID > 0)
                            {
                                returnValue = Page.SelectOne(valueID, false).InternalPath;
                            }
                        }
                    }
                    break;
                case ContentType.SubListSelect: {
                        if (string.IsNullOrWhiteSpace(_value) == false)
                        {
                            var subList = SubList.GetDeserialized(_value);
                            if (subList?.Items?.Length > 0)
                            {
                                StringBuilder temp = new StringBuilder();
                                foreach (var item in subList.Items)
                                {
                                    temp.Append($"{item.Description} (ID: {item.ID})<br/>");
                                }
                                returnValue = temp.ToString();
                            }
                        }
                    }
                    break;
                case ContentType.RichText:
                case ContentType.Sourcecode:
                case ContentType.TextArea:
                case ContentType.TextField:
                case ContentType.Undefined:
                case ContentType.TextLine:
                case ContentType.TextDate:
                
                case ContentType.Section:
                case ContentType.MultiImageSelect:
                case ContentType.MultiAssetUpload:
                case ContentType.ListItemSelect:
                case ContentType.Choice_Dropdown:
                case ContentType.Choice_Radio:
                case ContentType.HtmlContainer:
                    {
                        returnValue = _value;
                    }
                    break;
            }

            if (optimizeForDisplay && maxChars.GetValueOrDefault(0) > 0)
            {
                return Utility.ConvertToFixedLengthText(returnValue, maxChars.Value, "&hellip;");
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
