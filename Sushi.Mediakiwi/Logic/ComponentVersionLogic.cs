using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sushi.Mediakiwi.Data;

namespace Sushi.Mediakiwi.Data
{
    public class ComponentVersionLogic
    {
     
        /// <summary>
        /// Copies from master.
        /// </summary>
        /// <param name="pageID">The page ID.</param>
        internal static void CopyFromMaster(int pageID)      
        {
            Page page = Page.SelectOne(pageID);
            //Page masterPage = Page.SelectOne(page.MasterID.GetValueOrDefault());

            ComponentVersion[] versions = ComponentVersion.SelectAll(pageID);
            ComponentVersion[] masterVersions = ComponentVersion.SelectAll(page.MasterID.GetValueOrDefault());

            foreach (ComponentVersion version in versions)
            {
                ComponentVersion master = null;
                foreach (ComponentVersion item in masterVersions)
                {
                    if (item.ID == version.MasterID)
                    {
                        master = item;
                        break;
                    }
                }
                if (master == null) continue;

                Content content = master.GetContent();
                version.Serialized_XML = master.Serialized_XML;

                if (content != null && content.Fields != null)
                {
                    foreach (Content.Field field in content.Fields)
                    {
                        if (string.IsNullOrEmpty(field.Value) || field.Value == "0")
                            continue;

                        if (field.Type == (int)Framework.ContentType.RichText)
                        {
                            string candidate = field.Value;
                            Framework.ContentInfoItem.RichTextLink.CreateLinkMasterCopy(ref candidate, page.SiteID);
                            field.Value = candidate;
                        }
                        else if (field.Type == (int)Framework.ContentType.FolderSelect)
                        {
                            Folder folderInstance = Folder.SelectOneChild(Utility.ConvertToInt(field.Value), page.SiteID);
                            field.Value = folderInstance.ID.ToString();
                        }
                        else if (field.Type == (int)Framework.ContentType.Hyperlink)
                        {
                            Link link = Link.SelectOne(Utility.ConvertToInt(field.Value));
                            if (link != null && !link.IsNewInstance)
                            {
                                if (link.Type == Link.LinkType.InternalPage)
                                {
                                    Page pageInstance = Page.SelectOneChild(link.PageID.Value, page.SiteID, false);
                                    if (page != null)
                                    {
                                        link.ID = 0;
                                        link.PageID = pageInstance.ID;
                                        link.Save();
                                        field.Value = link.ID.ToString();
                                    }
                                }
                                else
                                {
                                    link.ID = 0;
                                    link.Save();
                                    field.Value = link.ID.ToString();
                                }
                            }
                        }
                        else if (field.Type == (int)Framework.ContentType.PageSelect)
                        {
                            Page pageInstance = Page.SelectOneChild(Utility.ConvertToInt(field.Value), page.SiteID, false);
                            field.Value = pageInstance.ID.ToString();
                        }
                    }
                    version.Serialized_XML = Content.GetSerialized(content);
                }
                else
                    version.Serialized_XML = null;

                version.Save();
            }
        }

    }
}
