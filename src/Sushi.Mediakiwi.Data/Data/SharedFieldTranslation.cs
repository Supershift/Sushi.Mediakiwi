using Sushi.Mediakiwi.Data;
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
                    Table("wim_SharedFieldView");
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

        /// <summary>
        /// The unique identifier
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// The Mediakiwi Site (Channel) identifier
        /// </summary>
        public int SiteID { get; set; }

        /// <summary>
        /// The SharedField identifier
        /// </summary>
        public int FieldID { get; set; }

        /// <summary>
        /// What kind of content does this SharedFieldTranslation represent 
        /// </summary>
        public ContentType ContentTypeID { get; set; }

        /// <summary>
        /// The fieldname for this SharedFieldTranslation
        /// </summary>
        public string FieldName { get; set; }

        /// <summary>
        /// The non-published version of the value
        /// </summary>
        public string EditValue { get; set; }

        /// <summary>
        /// The published version of the value
        /// </summary>
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
            var site = Site.SelectOne(SiteID);

            // Get dateInformation from 
            var dateInfo = Mediakiwi.Common.GetDateInformation(site);

            string dateFormat = dateInfo.DateFormatShort;
            string dateTimeFormat = dateInfo.DateTimeFormatShort;


            string returnValue = "";
            

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
                        if (DateTime.TryParseExact(_value, dateFormat, dateInfo.Culture, System.Globalization.DateTimeStyles.None, out DateTime result))
                        {
                            returnValue = result.ToString(dateFormat, dateInfo.Culture);
                        }
                    }
                    break;
                case ContentType.DateTime:
                    {
                        if (DateTime.TryParseExact(_value, dateTimeFormat, dateInfo.Culture, System.Globalization.DateTimeStyles.None, out DateTime result))
                        {
                            returnValue = result.ToString(dateTimeFormat, dateInfo.Culture);
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

        /// <summary>
        /// Returns all existing SharedFieldTranslations 
        /// </summary>
        /// <returns></returns>
        public static ICollection<SharedFieldTranslation> FetchAll()
        {
            var connector = new Connector<SharedFieldTranslation>(new SharedFieldTranslationMap(false));
            var filter = connector.CreateQuery();
            var result = connector.FetchAll(filter);
            return result;
        }

        /// <summary>
        /// Returns all existing SharedFieldTranslations 
        /// </summary>
        /// <returns></returns>
        public static async Task<ICollection<SharedFieldTranslation>> FetchAllAsync()
        {
            var connector = new Connector<SharedFieldTranslation>(new SharedFieldTranslationMap(false));
            var filter = connector.CreateQuery();
            var result = await connector.FetchAllAsync(filter).ConfigureAwait(false);
            return result;
        }

        /// <summary>
        /// Returns all existing SharedFieldTranslations for a specific SharedField
        /// </summary>
        /// <param name="sharedFieldID">The SharedField identifier</param>
        /// <returns></returns>
        public static ICollection<SharedFieldTranslation> FetchAllForField(int sharedFieldID)
        {
            var connector = new Connector<SharedFieldTranslation>(new SharedFieldTranslationMap(false));
            var filter = connector.CreateQuery();
            filter.Add(x => x.FieldID, sharedFieldID);
            var result = connector.FetchAll(filter);
            return result;
        }

        /// <summary>
        /// Returns all existing SharedFieldTranslations for a specific SharedField
        /// </summary>
        /// <param name="sharedFieldID">The SharedField identifier</param>
        /// <returns></returns>
        public static async Task<ICollection<SharedFieldTranslation>> FetchAllForFieldAsync(int sharedFieldID)
        {
            var connector = new Connector<SharedFieldTranslation>(new SharedFieldTranslationMap(false));
            var filter = connector.CreateQuery();
            filter.Add(x => x.FieldID, sharedFieldID);
            var result = await connector.FetchAllAsync(filter).ConfigureAwait(false);
            return result;
        }

        /// <summary>
        /// Returns all existing SharedFieldTranslations for a specific Site
        /// </summary>
        /// <param name="siteId">The MediaKiwi Site (Channel) identifier</param>
        /// <returns></returns>
        public static ICollection<SharedFieldTranslation> FetchAllForSite(int siteId)
        {
            var connector = new Connector<SharedFieldTranslation>(new SharedFieldTranslationMap(false));
            var filter = connector.CreateQuery();
            filter.Add(x => x.SiteID, siteId);
            var result = connector.FetchAll(filter);
            return result;
        }

        /// <summary>
        /// Returns all existing SharedFieldTranslations for a specific Site
        /// </summary>
        /// <param name="siteId">The MediaKiwi Site (Channel) identifier</param>
        /// <returns></returns>
        public static async Task<ICollection<SharedFieldTranslation>> FetchAllForSiteAsync(int siteId)
        {
            var connector = new Connector<SharedFieldTranslation>(new SharedFieldTranslationMap(false));
            var filter = connector.CreateQuery();
            filter.Add(x => x.SiteID, siteId);
            var result = await connector.FetchAllAsync(filter).ConfigureAwait(false);
            return result;
        }

        /// <summary>
        /// Returns all existing SharedFieldTranslations for a specific Page
        /// </summary>
        /// <param name="pageId">The MediaKiwi Page ID</param>
        /// <returns></returns>
        public static ICollection<SharedFieldTranslation> FetchAllForPage(int pageId)
        {
            var connector = new Connector<SharedFieldTranslation>(new SharedFieldTranslationMap(false));
            var filter = connector.CreateQuery();
            filter.AddParameter("@pageId", pageId);

            var result = connector.FetchAll(@"SELECT fields.* FROM [dbo].[wim_SharedFieldView] AS fields
LEFT JOIN [dbo].[wim_Properties] AS props ON props.[Property_FieldName] = fields.[SharedField_FieldName]
AND props.[Property_Type] = fields.[SharedField_ContentTypeID]
LEFT JOIN [dbo].[wim_Components] AS comps ON comps.[Component_ComponentTemplate_Key] = props.[Property_Template_Key]
WHERE props.[Property_IsShared] = 1 AND comps.[Component_Page_Key] = @pageId", filter);
            return result;
        }

        /// <summary>
        /// Returns all existing SharedFieldTranslations for a specific Page
        /// </summary>
        /// <param name="pageId">The MediaKiwi Page ID</param>
        /// <returns></returns>
        public static async Task<ICollection<SharedFieldTranslation>> FetchAllForPageAsync(int pageId)
        {
            var connector = new Connector<SharedFieldTranslation>(new SharedFieldTranslationMap(false));
            var filter = connector.CreateQuery();
            filter.AddParameter("@pageId", pageId);

            var result = await connector.FetchAllAsync(@"SELECT fields.* FROM [dbo].[wim_SharedFieldView] AS fields
LEFT JOIN [dbo].[wim_Properties] AS props ON props.[Property_FieldName] = fields.[SharedField_FieldName]
AND props.[Property_Type] = fields.[SharedField_ContentTypeID]
LEFT JOIN [dbo].[wim_Components] AS comps ON comps.[Component_ComponentTemplate_Key] = props.[Property_Template_Key]
WHERE props.[Property_IsShared] = 1 AND comps.[Component_Page_Key] = @pageId", filter).ConfigureAwait(false);
            return result;
        }

        /// <summary>
        /// Fetches a single SharedFieldTranslations by it's identifier
        /// </summary>
        /// <param name="id">The SharedField identifier</param>
        /// <returns></returns>
        public static SharedFieldTranslation FetchSingle(int id)
        {
            var connector = new Connector<SharedFieldTranslation>(new SharedFieldTranslationMap(false));
            var result = connector.FetchSingle(id);
            return result;
        }

        /// <summary>
        /// Fetches a single SharedFieldTranslations by a specific SharedField identifier and Site identifier
        /// </summary>
        /// <param name="sharedFieldId">The SharedField identifier</param>
        /// <param name="siteId">The MediaKiwi Site (Channel) identifier</param>
        /// <returns></returns>
        public static SharedFieldTranslation FetchSingleForFieldAndSite(int sharedFieldId, int siteId)
        {
            var connector = new Connector<SharedFieldTranslation>(new SharedFieldTranslationMap(false));
            var filter = connector.CreateQuery();
            filter.Add(x => x.SiteID, siteId);
            filter.Add(x => x.FieldID, sharedFieldId);
            var result = connector.FetchSingle(filter);
            return result;
        }

        /// <summary>
        /// Fetches a single SharedFieldTranslations by a specific SharedField identifier and Site identifier
        /// </summary>
        /// <param name="sharedFieldId">The SharedField identifier</param>
        /// <param name="siteId">The MediaKiwi Site (Channel) identifier</param>
        /// <returns></returns>
        public static async Task<SharedFieldTranslation> FetchSingleForFieldAndSiteAsync(int sharedFieldId, int siteId)
        {
            var connector = new Connector<SharedFieldTranslation>(new SharedFieldTranslationMap(false));
            var filter = connector.CreateQuery();
            filter.Add(x => x.SiteID, siteId);
            filter.Add(x => x.FieldID, sharedFieldId);
            var result = await connector.FetchSingleAsync(filter).ConfigureAwait(false);
            return result;
        }

        /// <summary>
        /// Saves this SharedFieldTranslation
        /// </summary>
        public void Save()
        {
            var connector = new Connector<SharedFieldTranslation>(new SharedFieldTranslationMap(true));
            connector.Save(this);
        }

        /// <summary>
        /// Saves this SharedFieldTranslation
        /// </summary>
        public async Task SaveAsync()
        {
            var connector = new Connector<SharedFieldTranslation>(new SharedFieldTranslationMap(true));
            await connector.SaveAsync(this).ConfigureAwait(false);
        }

        /// <summary>
        /// Deletes this SharedFieldTranslation
        /// </summary>
        public void Delete()
        {
            var connector = new Connector<SharedFieldTranslation>(new SharedFieldTranslationMap(true));
            connector.Delete(this);
        }

        /// <summary>
        /// Deletes this SharedFieldTranslation
        /// </summary>
        public async Task DeleteAsync()
        {
            var connector = new Connector<SharedFieldTranslation>(new SharedFieldTranslationMap(true));
            await connector.DeleteAsync(this).ConfigureAwait(false);
        }

        /// <summary>
        /// Reverts this SharedFieldTranslation to it's published version, so that changes made to the
        /// Non-published version are disregarded
        /// </summary>
        public async Task RevertAsync()
        {
            EditValue = Value;
            await SaveAsync().ConfigureAwait(false);
        }
    }
}
