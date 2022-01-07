using Sushi.Mediakiwi.Data;
using Sushi.Mediakiwi.Framework;
using Sushi.Mediakiwi.PageModules.ExportPage.JSON;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.PageModules.ExportPage
{
    public class PageTransferExporter
    {
        public Site Site { get; set; }


        private static Regex m_rex2;
        internal static Regex GetCleaner2
        {
            get
            {
                if (m_rex2 == null)
                    m_rex2 = new Regex(@"<a(?<ATTRIBUTES>.*?)href=""wim:(?<ID>.*?)("">)(?<TEXT>.*?)(</a>)", RegexOptions.IgnoreCase);
                return m_rex2;
            }
        }

        private ExportedPageComplete CompleteExport;

        #region Export Folders

        private async Task ExportFoldersAsync(Folder inFolder)
        {
            // Set Folders to import
            var fold = inFolder;

            if (inFolder.ParentID.HasValue)
            {
                while (fold.ParentID.HasValue)
                {
                    // Check if the folder isnt added already
                    if (CompleteExport.Page.Folders.Count(x => x.GUID == fold.GUID) == 0)
                    {
                        CompleteExport.Page.Folders.Add(new ExportedFolder()
                        {
                            GUID = fold.GUID,
                            ID = fold.ID,
                            Level = fold.Level,
                            Name = fold.Name,
                            ParentName = fold.Parent.Name,
                            ParentID = fold.Parent.ID,
                            Type = (int)fold.Type,
                            CompletePath = fold.CompletePath,
                            SiteID = fold.SiteID,
                            IsVisible = fold.IsVisible
                        });
                    }
                    fold = fold.Parent;
                }

                // Check if the folder isnt added already
                if (CompleteExport.Page.Folders.Count(x => x.GUID == fold.GUID) == 0)
                {
                    CompleteExport.Page.Folders.Add(new ExportedFolder()
                    {
                        GUID = fold.GUID,
                        ID = fold.ID,
                        Level = fold.Level,
                        Name = fold.Name,
                        ParentName = string.Empty,
                        ParentID = null,
                        Type = (int)fold.Type,
                        CompletePath = fold.CompletePath,
                        SiteID = fold.SiteID,
                        IsVisible = fold.IsVisible
                    });
                }
            }
            else
            {
                // Check if the folder isnt added already
                if (CompleteExport.Page.Folders.Count(x => x.GUID == fold.GUID) == 0)
                {
                    CompleteExport.Page.Folders.Add(new ExportedFolder()
                    {
                        GUID = fold.GUID,
                        ID = fold.ID,
                        Level = fold.Level,
                        Name = fold.Name,
                        ParentName = string.Empty,
                        ParentID = null,
                        Type = (int)fold.Type,
                        CompletePath = fold.CompletePath,
                        SiteID = fold.SiteID,
                        IsVisible = fold.IsVisible
                    });
                }
            }

        }

        #endregion

        #region Export Component Templates

        private async Task ExportComponentTemplatesAsync(PageTemplate inPageTemplate)
        {
            // Get available templates and Copy ComponentTemplates
            Dictionary<string, ComponentTemplate> pageComponentTemplates = new Dictionary<string, ComponentTemplate>();
            foreach (var availableTemplate in await AvailableTemplate.SelectAllAsync(inPageTemplate.ID))
            {
                // Get Component Template, so we can add the location for the template, 
                // not the ID, since we do not know if the ID exists on target DB
                var cTemplate = ((AvailableTemplate)availableTemplate).Template;

                ExportedAvailableComponent eac = new ExportedAvailableComponent();

                Utility.ReflectProperty(availableTemplate, eac);
                eac.ComponentTemplateGUID = cTemplate.GUID;
                eac.PageTemplateGUID = inPageTemplate.GUID;

                CompleteExport.PageTemplate.AvailableComponentTemplates.Add(eac);

                if (pageComponentTemplates.ContainsKey(cTemplate.Location) == false)
                {
                    pageComponentTemplates.Add(cTemplate.Location, cTemplate);
                }
            }

            // Copy ComponentTemplates
            foreach (var item in pageComponentTemplates)
            {
                ExportedComponentTemplate ct = new ExportedComponentTemplate();
                Utility.ReflectProperty(item.Value, ct);
                if (ct != null)
                {
                    CompleteExport.ComponentTemplates.Add(ct);
                }
            }

        }

        #endregion

        #region Export Components

        private async Task ExportComponentsAsync(int pageID)
        {
            // Copy Components
            List<Component> pageComponents = (await Component.SelectAllInheritedAsync(pageID, false, false).ConfigureAwait(false)).ToList();
            foreach (var item in pageComponents)
            {
                // When the component template does not yet exist in the export, 
                // add it here.
                if (CompleteExport.ComponentTemplates.Exists(x => x.GUID == item.Template.GUID) == false)
                {
                    ExportedComponentTemplate ct = new ExportedComponentTemplate();
                    Utility.ReflectProperty(item.Template, ct);
                    if (ct != null)
                    {
                        CompleteExport.ComponentTemplates.Add(ct);
                    }
                }

                ExportedComponent t = new ExportedComponent();
                Utility.ReflectProperty(item, t);
                t.TemplateGUID = item.Template.GUID;
                t.Content = new ExportedContent();

                MetaData[] metadata = (MetaData[])Utility.GetDeserialized(typeof(MetaData[]), item.Template.MetaData);
                if (item?.Content?.Fields?.Length > 0)
                {
                    foreach (var field in item.Content.Fields)
                    {
                        var newField = new ExportedField()
                        {
                            Property = field.Property,
                            Type = field.Type,
                            Value = field.Value,
                        };

                        if (metadata.Count(x => x.Name == field.Property) > 0)
                        {
                            newField.Label = metadata.Where(x => x.Name == field.Property).FirstOrDefault().Title;
                            newField.IsShared = metadata.Where(x => x.Name == field.Property).FirstOrDefault().IsSharedField == "1";
                        }

                        await ApplyExtendedFieldInformationAsync(newField, metadata).ConfigureAwait(false);
                        t.Content.Fields.Add(newField);
                    }
                }

                if (t != null)
                {
                    CompleteExport.Components.Add(t);
                }
            }
        }

        #endregion

        #region Export ComponentVersions

        private async Task ExportComponentVersionsAsync(int pageID)
        {
            // Get ComponentVersions
            foreach (var item in await ComponentVersion.SelectAllOnPageAsync(pageID).ConfigureAwait(false))
            {
                ExportedComponentVersion tv = new ExportedComponentVersion();
                Utility.ReflectProperty(item, tv);
                tv.TemplateGUID = item.Template.GUID;
                tv.Content = new ExportedContent();

                MetaData[] metadata = (MetaData[])Utility.GetDeserialized(typeof(MetaData[]), item.Template.MetaData);
                if (item.GetContent()?.Fields?.Length > 0)
                {
                    foreach (var field in item.GetContent().Fields)
                    {
                        var newField = new ExportedField()
                        {
                            Property = field.Property,
                            Type = field.Type,
                            Value = field.Value
                        };

                        if (metadata.Count(x => x.Name == field.Property) > 0)
                        {
                            newField.Label = metadata.Where(x => x.Name == field.Property).FirstOrDefault().Title;
                            newField.IsShared = metadata.Where(x => x.Name == field.Property).FirstOrDefault().IsSharedField == "1";
                        }

                        await ApplyExtendedFieldInformationAsync(newField, metadata).ConfigureAwait(false);
                        tv.Content.Fields.Add(newField);
                    }
                }

                if (tv != null)
                {
                    CompleteExport.ComponentVersions.Add(tv);
                }
            }
        }
        #endregion

        #region Export Galleries

        private async Task ExportGalleriesAsync()
        {

            // Get all galleries
            List<int> galleryIDs = new List<int>();
            foreach (var item in CompleteExport.Components)
            {
                if (item?.Content?.Fields?.Count > 0)
                {
                    ExtractUsedGalleryIDs(item.Content.Fields, galleryIDs);
                }
            }

            // Do we have any galleries extracted frmo the content ?
            // loop through them (and their parents) and add them to the export
            if (galleryIDs?.Count > 0)
            {
                List<int> parentFolders = new List<int>();

                // First loop to get parents and add them to the collection
                foreach (var galID in galleryIDs)
                {
                    foreach (var parentGal in await Gallery.SelectAllByBackwardTrailAsync(galID))
                    {
                        if (parentFolders.Contains(parentGal.ID) == false)
                        {
                            parentFolders.Add(parentGal.ID);
                        }
                    }
                }

                if (parentFolders?.Count > 0)
                {
                    parentFolders.ForEach(g =>
                    {
                        if (galleryIDs.Contains(g) == false)
                        {
                            galleryIDs.Add(g);
                        }
                    });
                }

                foreach (var item in galleryIDs)
                {
                    Gallery gal = await Gallery.SelectOneAsync(item);

                    ExportedGallery g = new ExportedGallery();
                    g.BaseID = gal.BaseGalleryID;
                    g.CompletePath = gal.CompletePath;
                    g.Created = gal.Created;
                    g.Format = gal.Format;
                    g.FormatType = gal.FormatType;
                    g.GUID = gal.GUID;
                    g.IsActive = gal.IsActive;
                    g.IsFixed = gal.IsFixed;
                    g.IsFolder = gal.IsFolder;
                    g.IsHidden = gal.IsHidden;
                    g.Name = gal.Name;
                    if (gal.ParentID.GetValueOrDefault(0) > 0)
                    {
                        g.ParentGUID = gal.Parent.GUID;
                    }
                    g.Type = gal.TypeID;

                    CompleteExport.Galleries.Add(g);
                }
            }

        }
        #endregion

        #region Export Full DataSet

        public async Task<ExportedPageComplete> ExportPageAsync(Page inPage)
        {
            inPage = await Page.SelectOneAsync(inPage.ID, false).ConfigureAwait(false);

            // Set site for later, richtext clean
            Site = inPage.Site;

            CompleteExport = new ExportedPageComplete();

            // Copy Page Properties
            Utility.ReflectProperty(inPage, CompleteExport.Page);

            // Folders
            await ExportFoldersAsync(inPage.Folder).ConfigureAwait(false);

            // Copy PageTemplate Properties
            Utility.ReflectProperty(inPage.Template, CompleteExport.PageTemplate);

            // Copy DATA when applicable
            if (inPage.Template?.Data != null)
                CompleteExport.PageTemplate.Data = inPage.Template.Data;

            // Export Component Templates
            await ExportComponentTemplatesAsync(inPage.Template).ConfigureAwait(false);

            // Export Components
            await ExportComponentsAsync(inPage.ID).ConfigureAwait(false);

            // Export Component Versions
            await ExportComponentVersionsAsync(inPage.ID).ConfigureAwait(false);

            // Export galleries
            await ExportGalleriesAsync().ConfigureAwait(false);

            return CompleteExport;
        }

        #endregion

        #region Extract Used Gallery IDs

        private void ExtractUsedGalleryIDs(List<ExportedField> fields, List<int> galleryIDs)
        {
            if (fields != null && fields.Count > 0)
            {
                foreach (var field in fields)
                {
                    if (field?.AssetInfo?.GalleryID > 0 && galleryIDs.Contains(field.AssetInfo.GalleryID) == false)
                    {
                        galleryIDs.Add(field.AssetInfo.GalleryID);
                    }
                    if (field?.MultiFields?.Count > 0)
                    {
                        ExtractUsedGalleryIDs(field.MultiFields, galleryIDs);
                    }
                }
            }
        }

        #endregion

        #region Get Asset Info

        private async Task<ExportedFieldInfoAsset> GetAssetInfoAsync(string value)
        {
            ExportedFieldInfoAsset assetInfo = new ExportedFieldInfoAsset();

            if (Utility.ConvertToInt(value, 0) > 0)
            {
                var asset = await Asset.SelectOneAsync(Convert.ToInt32(value));

                if (asset?.ID > 0)
                {
                    var remLoc = asset.RemoteLocation;

                    if (string.IsNullOrWhiteSpace(remLoc))
                    {
                        assetInfo.Message = $"The document is here :<a href=\"{remLoc}\">Click to view</a>";
                        assetInfo.RemoteLocation = remLoc;
                    }

                    Utility.ReflectProperty(asset, assetInfo);
                    Gallery gal = await Gallery.SelectOneAsync(asset.GalleryID);
                    if (gal?.ID > 0)
                    {
                        assetInfo.GalleryGUID = gal.GUID;
                    }
                }
                else
                {
                    assetInfo.Message = $"The document cannot be found : {value}";
                }
            }
            else
            {
                assetInfo.Message = $"The AssetID is '{value}' which is incorrect";
            }
            return assetInfo;
        }

        #endregion

        #region Get Folder Info

        private async Task<ExportedFieldInfoFolder> GetFolderInfoAsync(string value)
        {
            ExportedFieldInfoFolder fieldInfo = new ExportedFieldInfoFolder();
            if (Utility.ConvertToInt(value, 0) > 0)
            {
                var fold = await Folder.SelectOneAsync(Convert.ToInt32(value));

                if (fold == null || fold.ID == 0)
                {
                    fieldInfo.Message = $"The folder cannot be found : {value}";
                    fieldInfo.RemoteLocation = string.Empty;
                }
                else
                {
                    fieldInfo.Message = $"The folder is <strong>'{fold.CompletePath}'</strong>";
                    fieldInfo.RemoteLocation = fold.CompletePath;
                    fieldInfo.GUID = fold.GUID;

                    // loop through selected folder up to the root and add them to the folders to import
                    await ExportFoldersAsync(fold);
                }
            }
            else
            {
                fieldInfo.Message = $"The Folder ID is '{value}' which is incorrect";
            }
            return fieldInfo;
        }

        #endregion

        #region Get Link Info

        private async Task<ExportedFieldInfoLink> GetLinkInfoAsync(string value)
        {
            ExportedFieldInfoLink LinkInfo = new ExportedFieldInfoLink();

            if (Utility.ConvertToInt(value, 0) > 0)
            {
                var link = await Link.SelectOneAsync(Convert.ToInt32(value));

                if (link?.IsInternal == true && link?.PageID > 0)
                {
                    LinkInfo.Message = "This is an internal link";
                    LinkInfo.RemoteLocation = (await Page.SelectOneAsync(link.PageID.Value)).CompletePath;
                }
                else if (link?.AssetID > 0)
                {
                    LinkInfo.Message = "This is a document link";
                    LinkInfo.RemoteLocation = link.ExternalUrl;
                    LinkInfo.AssetInfo = await GetAssetInfoAsync(link.AssetID.Value.ToString());
                }
                else
                {
                    LinkInfo.Message = "This is an external link";
                    LinkInfo.RemoteLocation = link.ExternalUrl;
                }

                if (link?.ID > 0)
                {
                    Utility.ReflectProperty(link, LinkInfo);
                }

                LinkInfo.LinkText = link.Text;
            }
            else
            {
                LinkInfo.Message = $"The LinkID is '{value}' which is incorrect";
            }
            return LinkInfo;
        }

        #endregion

        #region Get Page Info

        private async Task<ExportedFieldInfo> GetPageInfoAsync(string value)
        {
            ExportedFieldInfo PageInfo = new ExportedFieldInfo();

            if (Utility.ConvertToInt(value, 0) > 0)
            {
                var page = await Page.SelectOneAsync(Convert.ToInt32(value));

                if (page?.ID > 0)
                {
                    PageInfo.Message = "This is a page selection";
                    PageInfo.RemoteLocation = page.Name;
                }
                else
                {
                    PageInfo.Message = $"The linked page could not be found : {value}";
                }
            }
            else
            {
                PageInfo.Message = $"The PageID is '{value}' which is incorrect";
            }

            return PageInfo;
        }

        #endregion

        #region Get SubList Info

        private async Task<ExportedFieldInfoSublist> GetSublistInfoAsync(string value, string propertyName, MetaData[] metaData = null)
        {
            ExportedFieldInfoSublist slInfo = new ExportedFieldInfoSublist();
            if (string.IsNullOrWhiteSpace(value) == false)
            {
                SubList sl = SubList.GetDeserialized(value);
                var meta = metaData.Where(x => x.Name == propertyName).Where(x => x.ContentTypeSelection == ((int)ContentType.SubListSelect).ToString()).First();

                if (meta != null)
                {
                    if (string.IsNullOrWhiteSpace(meta.Componentlist) == false)
                    {
                        Guid listGuid = Utility.ConvertToGuid(meta.Componentlist, Guid.Empty);
                        if (listGuid != Guid.Empty)
                        {
                            var cList = await ComponentList.SelectOneAsync(listGuid);

                            slInfo.Assembly = cList.AssemblyName;
                            slInfo.ClassName = cList.ClassName;
                            slInfo.GUID = cList.GUID;
                            slInfo.Message = $"Sublist with label '{meta.Title}' relates to list '{cList.Name}'";
                            slInfo.RemoteLocation = cList.CompletePath();
                        }
                        else
                        {
                            slInfo.Message = $"The field points to a list GUID '{meta.Componentlist}' which doesn't exist";
                        }
                    }
                    else
                    {
                        slInfo.Message = $"The field doesn't point to a list GUID.";
                    }
                }
                else
                {
                    slInfo.Message = $"There is no metadata present for this field.";
                }
            }
            else
            {
                slInfo.Message = "The data for this sublistselect is empty, which is incorrect";
            }

            return slInfo;
        }

        #endregion

        #region Get RichText Info

        private async Task<ExportedFieldInfoRichText> GetRichtextInfoAsync(string value)
        {
            ExportedFieldInfoRichText rtInfo = new ExportedFieldInfoRichText();

            if (string.IsNullOrWhiteSpace(value) == false)
            {
                int linkKey;

                var matches = GetCleaner2.Matches(value);
                foreach (Match m in matches)
                {
                    string attributes = m.Groups["ATTRIBUTES"].Value;
                    string text = m.Groups["TEXT"].Value;
                    if (Utility.IsNumeric(m.Groups["ID"].Value, out linkKey))
                    {
                        ExportedFieldInfoLink eLink = await GetLinkInfoAsync(linkKey.ToString());
                        rtInfo.Links.Add(eLink);
                    }
                }
                rtInfo.CleanedText = Utils.ApplyRichtext(Site, value);
            }
            else
            {
                rtInfo.Message = "The content is empty";
            }


            return rtInfo;
        }

        #endregion

        #region Get Multi Fields

        private async Task<List<ExportedField>> GetMultiFieldsAsync(string value, ExportedFieldInfo genericInfo, string label)
        {
            List<ExportedField> tempList = new List<ExportedField>();
            var fields = Framework.MultiField.GetDeserialized(value);
            if (fields?.Length > 0)
            {
                genericInfo.Message = $"{fields.Length} fields have been found inside this multifield";
                foreach (var item in fields)
                {
                    ExportedField ef = new ExportedField();
                    ef.Type = item.Type;
                    ef.Value = item.Value;
                    ef.Property = item.Property;
                    ef.Label = $"{label}.{item.Property}";

                    await ApplyExtendedFieldInformationAsync(ef).ConfigureAwait(false);

                    tempList.Add(ef);
                }
            }
            else
            {
                genericInfo.Message = "No fields have been found inside this multifield";
            }
            return tempList;
        }
        #endregion

        #region Apply Extended Field Information

        private async Task ApplyExtendedFieldInformationAsync(ExportedField field, MetaData[] metaData = null)
        {
            if (string.IsNullOrWhiteSpace(field.Label))
            {
                if (metaData != null && metaData.Length > 0)
                    field.Label = metaData[0].Title;
                else
                    field.Label = $"PROPERTY:{field.Property}";
            }


            switch (field.Type)
            {

                case (int)ContentType.Binary_Image:
                case (int)ContentType.DocumentSelect:
                case (int)ContentType.Binary_Document:
                    {
                        field.AssetInfo = await GetAssetInfoAsync(field.Value).ConfigureAwait(false);
                    }
                    break;
                case (int)ContentType.FolderSelect:
                    {
                        field.FolderInfo = await GetFolderInfoAsync(field.Value).ConfigureAwait(false);
                    }
                    break;
                case (int)ContentType.Hyperlink:
                    {
                        field.LinkInfo = await GetLinkInfoAsync(field.Value).ConfigureAwait(false);
                    }
                    break;
                case (int)ContentType.PageSelect:
                    {
                        field.GenericInfo = await GetPageInfoAsync(field.Value).ConfigureAwait(false);
                    }
                    break;
                case (int)ContentType.SubListSelect:
                    {
                        field.SubListInfo = await GetSublistInfoAsync(field.Value, field.Property, metaData).ConfigureAwait(false);
                    }
                    break;
                case (int)ContentType.RichText:
                    {
                        field.RichTextInfo = await GetRichtextInfoAsync(field.Value).ConfigureAwait(false);
                    }
                    break;
                case (int)ContentType.MultiField:
                    {
                        field.GenericInfo = new ExportedFieldInfo();
                        field.MultiFields = await GetMultiFieldsAsync(field.Value, field.GenericInfo, field.Label).ConfigureAwait(false);
                    }
                    break;
                default:
                    {
                    }
                    break;
            }
        }
        #endregion

    }
}
