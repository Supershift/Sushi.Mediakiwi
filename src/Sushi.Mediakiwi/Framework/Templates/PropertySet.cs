using Sushi.Mediakiwi.Data;
using System;

namespace Sushi.Mediakiwi.Framework.Templates
{
    /// <summary>
    /// 
    /// </summary>
	public class PropertySet
	{
        /// <summary>
        /// 
        /// </summary>
		public PropertySet( )
		{
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="site"></param>
        /// <param name="loadedControl"></param>
        /// <param name="content"></param>
        /// <param name="callingPage"></param>
        /// <returns></returns>
        public bool SetValue(Site site, object loadedControl, Content content, Page callingPage)
        {
            return SetValue(site, loadedControl, content, null, callingPage);
        }

        //void SetCustomControls(RichLink rlink, Sushi.Mediakiwi.Data.Site site, object loadedControl, Sushi.Mediakiwi.Data.Content content, Sushi.Mediakiwi.UI.ControlCollection collection, Data.Component component)
        //{
        //    foreach (Sushi.Mediakiwi.UI.Control c in collection)
        //    {
        //        if (c.GetType().BaseType == typeof(Sushi.Mediakiwi.Framework.ControlLib.Base.ContentInfo))
        //        {
        //            if (((Sushi.Mediakiwi.Framework.ControlLib.Base.ContentInfo)c).HasReflection)
        //                continue;

        //            foreach (Field field in content.Fields)
        //            {
        //                if (field.Property != c.ID) continue;
        //                if (field.Value == null || field.Value == string.Empty)
        //                {
        //                    if (c.GetType() == typeof(Sushi.Mediakiwi.Framework.ControlLib.WimText))
        //                        ((Sushi.Mediakiwi.Framework.ControlLib.WimText)c).Text = null;

        //                    break;
        //                }

        //                if (c.GetType() == typeof(Sushi.Mediakiwi.Framework.ControlLib.WimText))
        //                {
        //                    if (((Sushi.Mediakiwi.Framework.ControlLib.WimText)c).TextMode == Sushi.Mediakiwi.Framework.ControlLib.WimText.Type.RichText)
        //                    {
        //                    }
        //                    else if (((Sushi.Mediakiwi.Framework.ControlLib.WimText)c).TextMode == Sushi.Mediakiwi.Framework.ControlLib.WimText.Type.TextArea)
        //                    {
        //                        field.Value = Data.Utility.CleanLineFeed(field.Value);
        //                    }
        //                    ((Sushi.Mediakiwi.Framework.ControlLib.WimText)c)._Component = component;
        //                    ((Sushi.Mediakiwi.Framework.ControlLib.WimText)c).Text = field.Value;
        //                }
        //                else if (c.GetType() == typeof(Sushi.Mediakiwi.Framework.ControlLib.WimMulti))
        //                {
        //                    ((Sushi.Mediakiwi.Framework.ControlLib.WimMulti)c)._Component = component;
        //                    ((Sushi.Mediakiwi.Framework.ControlLib.WimMulti)c).Text = field.Value;
        //                }
        //                else if (c.GetType() == typeof(Sushi.Mediakiwi.Framework.ControlLib.WimLabel))
        //                {
        //                    ((Sushi.Mediakiwi.Framework.ControlLib.WimLabel)c).Text = field.Value;
        //                }
        //                else if (c.GetType() == typeof(Sushi.Mediakiwi.Framework.ControlLib.WimImage))
        //                {
        //                    int imageId = Data.Utility.ConvertToInt(field.Value.ToString());
        //                    if (imageId == 0) break;

        //                    ((Sushi.Mediakiwi.Framework.ControlLib.WimImage)c).Image = Sushi.Mediakiwi.Data.Image.SelectOne(imageId);
        //                    ((Sushi.Mediakiwi.Framework.ControlLib.WimImage)c).Set();
        //                }
        //                else if (c.GetType() == typeof(Sushi.Mediakiwi.Framework.ControlLib.WimLink))
        //                {
        //                    int linkId = Data.Utility.ConvertToInt(field.Value);
        //                    if (linkId == 0) break;
        //                    if (((Sushi.Mediakiwi.Framework.ControlLib.WimLink)c).IsPagelink)
        //                    {
        //                        if (site.MasterID.HasValue)
        //                            ((Sushi.Mediakiwi.Framework.ControlLib.WimLink)c).Page = Sushi.Mediakiwi.Data.Page.SelectOneChild(linkId, site.ID);
        //                        else
        //                            ((Sushi.Mediakiwi.Framework.ControlLib.WimLink)c).Page = Sushi.Mediakiwi.Data.Page.SelectOne(linkId);
        //                    }
        //                    else
        //                        ((Sushi.Mediakiwi.Framework.ControlLib.WimLink)c).Link = Sushi.Mediakiwi.Data.Link.SelectOne(linkId);

        //                    ((Sushi.Mediakiwi.Framework.ControlLib.WimLink)c).Set();

        //                }
        //                else if (c.GetType() == typeof(Sushi.Mediakiwi.Framework.ControlLib.WimDocument))
        //                {
        //                    int docId = Data.Utility.ConvertToInt(field.Value);
        //                    if (docId == 0) break;
        //                    ((Sushi.Mediakiwi.Framework.ControlLib.WimDocument)c).Document = Sushi.Mediakiwi.Data.Document.SelectOne(docId);

        //                    ((Sushi.Mediakiwi.Framework.ControlLib.WimDocument)c).Set();
        //                }
        //            }
        //        }

        //        SetCustomControls(rlink, site, loadedControl, content, c.Controls, component);
        //    }
        //}

        string CleanParagraphWrap(string input)
        {
            string candidate = input;
            if (candidate.StartsWith("<p>", true, System.Threading.Thread.CurrentThread.CurrentCulture))
            {
                candidate = candidate.Remove(0, 3);
                if (candidate.EndsWith("</p>", true, System.Threading.Thread.CurrentThread.CurrentCulture))
                    candidate = candidate.Substring(0, candidate.Length - 4);
            }
            return candidate;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="site"></param>
        /// <param name="loadedControl"></param>
        /// <param name="content"></param>
        /// <param name="component"></param>
        /// <param name="callingPage"></param>
        /// <returns></returns>
        public bool SetValue(Site site, object loadedControl, Content content, Component component, Page callingPage)
        {
            var dateInfo = Common.GetDateInformation(site);

            bool isInheritedContent = false;
            if ( callingPage != null && callingPage.InheritContent )
                isInheritedContent = true;

            if (loadedControl == null || content == null)
            {
                return false;
            }

            if (content.Fields == null)
            {
                return true;
            }

            System.Globalization.CultureInfo cultureInfo = new System.Globalization.CultureInfo("nl-NL");
            
            RichLink rlink = new RichLink(site);

            // 18-10-2020:MM
            //if (loadedControl is Sushi.Mediakiwi.UI.Control)
            //    SetCustomControls(rlink, site, (Sushi.Mediakiwi.UI.Control)loadedControl, content, ((Sushi.Mediakiwi.UI.Control)loadedControl).Controls, component);

            //  Set all fields
            foreach ( System.Reflection.PropertyInfo info in loadedControl.GetType().GetProperties() )
            {   
                //  Get all public properties
                if ( info.CanWrite )
                {
                    // Get all writable public properties
                    foreach( object attribute in info.GetCustomAttributes( false ) )
                    {   
                        //  Get all custom info attributes
                        if (attribute is IContentInfo)
                        {
                            foreach(var field in content.Fields )
                            {
                                if ( field.Property != info.Name )
                                    continue;   //  Just skip it
                                if ( field.Value == null || field.Value == string.Empty )
                                {
                                    info.SetValue(loadedControl, null, null );
                                    break;
                                }

                                object itemValue = field.Value;

                                switch( field.Type ) 
                                {
                                    case (int)ContentType.FileUpload:
                                        break;   //  Skip because this is handled internally (appcentre)
                                    case (int)ContentType.ListItemSelect:
                                        #region ContentListType.ListItemSelect
                                        string[] itemArr = itemValue.ToString().Split(',');
                                        if ( info.PropertyType == typeof(string[]) )
                                        {
                                            info.SetValue(loadedControl, itemArr, null );
                                            break;
                                        }
                                        if (info.PropertyType == typeof(int[]))
                                        {
                                            info.SetValue(loadedControl, Utility.ConvertToIntArray(itemArr), null);
                                            break;
                                        }
                                        
                                        SendNotification("ListItemSelect", loadedControl, info);
                                        break;
                                        #endregion ContentListType.ListItemSelect

                                    case (int)ContentType.MultiImageSelect:
                                        #region ContentListType.MultiImageSelect
                                        string[] itemImageArr = itemValue.ToString().Split(',');
                                        if (info.PropertyType == typeof(string[]))
                                        {
                                            info.SetValue(loadedControl, itemImageArr, null);
                                            break;
                                        }
                                        if (info.PropertyType == typeof(int[]))
                                        {
                                            info.SetValue(loadedControl, Utility.ConvertToIntArray(itemImageArr), null);
                                            break;
                                        }
                                        if (info.PropertyType == typeof(Image[]))
                                        {
                                            info.SetValue(loadedControl, Image.SelectAll(Utility.ConvertToIntArray(itemImageArr)), null);
                                            break;
                                        }

                                        SendNotification("MultiImageSelect", loadedControl, info);
                                        break;
                                        #endregion ContentListType.ListItemSelect
                                    
                                    case (int)ContentType.SubListSelect:
                                        #region ContentType.SubListSelect
                                        SubList sublist =
                                            SubList.GetDeserialized( itemValue.ToString() );
                                        if ( sublist == null )
                                            break;
                                        if ( info.PropertyType == typeof(SubList) )
                                        {
                                            info.SetValue(loadedControl, sublist, null );
                                            break;
                                        }
                                        SendNotification("SubListSelect", loadedControl, info);
                                        break;
                                        #endregion ContentType.SubListSelect
                                    case (int)ContentType.FolderSelect:
                                        #region ContentType.FolderSelect
                                        int folderId = Utility.ConvertToInt( field.Value );

                                        Folder folder = null;
                                        if (folderId != 0)
                                        {
                                            //  08/05/2006:MM Content inheritance
                                            if (isInheritedContent)
                                                folder = Folder.SelectOneChild(folderId, site.ID);
                                            else
                                                folder = Folder.SelectOne(folderId);

                                            if (folder == null)
                                                break;

                                            folderId = folder.ID;
                                        }

                                        //  Choices: int, Sushi.Mediakiwi.Data.Link
                                        if ( info.PropertyType == typeof(int) )
                                        {
                                            info.SetValue(loadedControl, folderId, null );
                                            break;
                                        }

                                        //  NOTE: Rest is only for ContentInfoSharedAttribute
                                        if ( attribute is IListContentInfo )
                                        {
                                            SendNotification("FolderSelect", loadedControl, info);
                                            break;
                                        }

                                        if ( info.PropertyType == typeof(Folder) )
                                        {
                                            info.SetValue(loadedControl, folder, null );
                                            break;
                                        }
                                        SendNotification("FolderSelect", loadedControl, info);

                                        break;
                                        #endregion ContentType.FolderSelect
                                    case (int)ContentType.Button:
                                        if (info.PropertyType == typeof(bool))
                                        {
                                            if (itemValue.ToString() == "1")
                                                info.SetValue(loadedControl, true, null);
                                            else
                                                info.SetValue(loadedControl, false, null);
                                        }
                                        break;

                                    case (int)ContentType.Choice_Checkbox:
                                        if (info.PropertyType == typeof(bool))
                                        {
                                            if (itemValue.ToString() == "1")
                                                info.SetValue(loadedControl, true, null);
                                            else
                                                info.SetValue(loadedControl, false, null);
                                        }
                                        break;

                                    case (int)ContentType.Date:
                                    case (int)ContentType.DateTime:
                                        #region ContentType.DateTime
                                        
                                        DateTime dtResult;

                                        long newDt;
                                        if (long.TryParse(itemValue.ToString(), out newDt))
                                        {
                                            dtResult = new DateTime(newDt);
                                            dtResult = ConvertDateTimeToLocal(loadedControl, dtResult);
                                            info.SetValue(loadedControl, dtResult, null);
                                        }
                                        else
                                        {
                                            if (DateTime.TryParseExact(itemValue.ToString(), dateInfo.DateTimeFormatShort, dateInfo.Culture, System.Globalization.DateTimeStyles.None, out dtResult))
                                            {
                                                dtResult = ConvertDateTimeToLocal(loadedControl, dtResult);
                                                info.SetValue(loadedControl, dtResult, null);
                                            }
                                            else
                                            {
                                                info.SetValue(loadedControl, DateTime.MinValue, null);
                                            }
                                        }
                                        break;

                                        #endregion ContentType.DateTime
                                    case (int)ContentType.PageSelect:
                                        #region ContentType.PageSelect
                                        int pageId = Utility.ConvertToInt( field.Value );

                                        //  Choices: int, Sushi.Mediakiwi.Data.Link
                                        if (info.PropertyType == typeof(int))
                                        {
                                            info.SetValue(loadedControl, pageId, null);
                                            break;
                                        }

                                        if ( pageId != 0 )
                                        {
                                            Page page = null;

                                            //  08/05/2006:MM Content inheritance
                                            if (isInheritedContent)
                                                page = Page.SelectOneChild(pageId, site.ID, (callingPage != null));
                                            else
                                                page = Page.SelectOne(pageId, (callingPage != null));

                                            if (page == null)
                                            {
                                                break;
                                            }

                                            //  NOTE: Rest is only for ContentInfoSharedAttribute
                                            if (attribute is IListContentInfo)
                                            {
                                                SendNotification("PageSelect", loadedControl, info);
                                                break;
                                            }

                                            if ( info.PropertyType == typeof(Page) )
                                            {
                                                info.SetValue(loadedControl, page, null );
                                                break;
                                            }
                                            if ( info.PropertyType == typeof(string) )
                                            {
                                                info.SetValue(loadedControl, page.HRef, null );
                                                break;
                                            }

                                            SendNotification("PageSelect", loadedControl, info);
                                        }

                                        break;
                                        #endregion ContentType.PageSelect
                                    case (int)ContentType.Hyperlink:
                                        #region ContentType.Hyperlink
                                        int hyperlink = Utility.ConvertToInt( field.Value );
                                        
                                        //if ( hyperlink == 0 && field.Value != "0" )
                                        //{
                                        //    //  This could be the old system, so for now
                                        //    #region OLD SYSTEM CODE
                                        //    //Sushi.Mediakiwi.Data.Link link = Sushi.Mediakiwi.Data.LinkManager.GetDeserialized( field.Value );
                                        //    //Sushi.Mediakiwi.Data.Link link2 = new Sushi.Mediakiwi.Data.Link();
                                        //    //link2.Text = link.Text;
                                        //    //link2.Alt = link.Alt;
                                        //    //link2.Target = link.Target;
                                        //    //if ( link.Url != null ) link2.Url = link.Url;

                                        //    //if ( link.IsInternal )
                                        //    //{
                                        //    //    Sushi.Mediakiwi.Data.Page page = null;
                                        //    //    if (link.PageId.HasValue)
                                        //    //        page = Sushi.Mediakiwi.Data.Page.SelectOne(link.PageId.Value);
                                        
                                        //    //    if ( page == null ||!page.IsPublished )
                                        //    //        link2 = null;
                                        //    //    else
                                        //    //        link2.Url = Data.Utility.AddApplicationPath(string.Format("{0}{1}.aspx", page.CompletePath, page.Name ));
                                        //    //}
                                        //    #endregion OLD SYSTEM CODE
                                        //    itemValue = link2;
                                        //    info.SetValue(loadedControl, link2, null );
                                        //}
                                        //else
                                        //{
                                            //  Choices: int, Sushi.Mediakiwi.Data.Link
                                            if ( info.PropertyType == typeof(int) )
                                            {
                                                info.SetValue(loadedControl, hyperlink, null );
                                                break;
                                            }

                                            //  NOTE: Rest is only for ContentInfoSharedAttribute
                                            if (attribute is IListContentInfo)
                                                break;

                                            if ( info.PropertyType == typeof(Link) )
                                            {
                                            Link link = Link.SelectOne(hyperlink);
                                                info.SetValue(loadedControl, link, null );
                                                break;
                                            }
                                        //}
                                        SendNotification("Hyperlink", loadedControl, info);

                                        break;
                                        #endregion ContentType.Hyperlink
                                    case (int)ContentType.Binary_Image:
                                        #region Binary_Image
                                        int imageId = Utility.ConvertToInt( itemValue.ToString() );
                                        if (imageId == 0)
                                        {
                                            info.SetValue(loadedControl, null, null);
                                            break;
                                        }

                                        Image image = Image.SelectOne(imageId );
                                        if ( image == null )
                                            break;

                                        if ( info.PropertyType == typeof(int) )
                                        {
                                            info.SetValue(loadedControl, imageId, null );
                                            break;
                                        }

                                        //  NOTE: Rest is only for ContentInfoSharedAttribute
                                        if (attribute is IListContentInfo)
                                        {
                                            SendNotification("Binary_Image", loadedControl, info);
                                            break;
                                        }

                                        if ( info.PropertyType == typeof(string) )
                                        {
                                            info.SetValue(loadedControl, image.Path, null );
                                            break;
                                        }
                                        if ( info.PropertyType == typeof(Image) )
                                        {
                                            info.SetValue(loadedControl, image, null );
                                            break;
                                        }

                                        SendNotification("Binary_Image", loadedControl, info);

                                        break;
                                        #endregion Binary_Image
                                    case (int)ContentType.Binary_Document:
                                        #region Binary_Document
                                        int documentId = Utility.ConvertToInt( itemValue.ToString() );
                                        if (documentId == 0)
                                        {
                                            info.SetValue(loadedControl, null, null);
                                            break;
                                        }

                                        Document document = Document.SelectOne(documentId );
                                        if ( document == null )
                                            break;

                                        if ( info.PropertyType == typeof(int) )
                                        {
                                            info.SetValue(loadedControl, documentId, null );
                                            break;
                                        }

                                        //  NOTE: Rest is only for ContentInfoSharedAttribute
                                        if (attribute is IListContentInfo)
                                        {
                                            SendNotification("Binary_Document", loadedControl, info);
                                            break;
                                        }

                                        if ( info.PropertyType == typeof(string) )
                                        {
                                            info.SetValue(loadedControl, document.Path, null );
                                            break;
                                        }
                                        if ( info.PropertyType == typeof(Document) )
                                        {
                                            info.SetValue(loadedControl, document, null );
                                            break;
                                        }
                                        SendNotification("Binary_Document", loadedControl, info);
                                        break;
                                        #endregion Binary_Document
                                    case (int)ContentType.RichText:

                                        if (attribute is IListContentInfo) 
                                        {
                                        }
                                        //else
                                            //itemValue = Data.Utility.ApplyRichtextLinks(rlink.Site, itemValue.ToString());

                                        if (attribute is IContentInfo)
                                        {
                                            if (((ContentInfoItem.RichTextAttribute)attribute).CleanParagraphWrap)
                                                itemValue = CleanParagraphWrap(itemValue.ToString());
                                        }

                                        info.SetValue(loadedControl, itemValue, null);
                                        
                                        break;

                                    case (int)ContentType.TextArea:
                                        itemValue = Utility.CleanLineFeed(itemValue.ToString());
                                        info.SetValue(loadedControl, itemValue, null);
                                        break;

                                    case (int)ContentType.Choice_Dropdown:

                                        if (info.PropertyType == typeof(string))
                                            info.SetValue(loadedControl, itemValue, null);

                                        else if (info.PropertyType == typeof(int))
                                            info.SetValue(loadedControl, Utility.ConvertToInt(itemValue), null);

                                        break;


                                    case (int)ContentType.Choice_Radio:

                                        if (info.PropertyType == typeof(string))
                                            info.SetValue(loadedControl, itemValue, null);

                                        else if (info.PropertyType == typeof(int))
                                            info.SetValue(loadedControl, Utility.ConvertToInt(itemValue), null);

                                        break;

                                    case (int)ContentType.TextField:

                                        if (info.PropertyType == typeof(string))
                                            info.SetValue(loadedControl, itemValue, null);

                                        else if (info.PropertyType == typeof(int))
                                            info.SetValue(loadedControl, Utility.ConvertToInt(itemValue), null);
                                        // [CB] now the list search history works when paging
                                        else if (info.PropertyType == typeof(int?))
                                            info.SetValue(loadedControl, Utility.ConvertToIntNullable(itemValue), null);

                                        else if (info.PropertyType == typeof(decimal))
                                            info.SetValue(loadedControl, Utility.ConvertToDecimal(itemValue), null);
                                        // [CB] now the list search history works when paging
                                        else if (info.PropertyType == typeof(decimal?))
                                            info.SetValue(loadedControl, Utility.ConvertToDecimalNullable(itemValue), null);

                                        break;
                                            
                                    default:
                                        try
                                        {
                                            info.SetValue(loadedControl, itemValue, null);
                                        }
                                        catch (Exception ex)
                                        {
                                            Notification.InsertOne("Internal Wim error", NotificationType.Error, null, ex.Message);
                                        }
                                        break;
                                }
                                break;
                            }
                        }
                    }
                }
            }
            return true;
        }

        private static DateTime ConvertDateTimeToLocal(object loadedControl, DateTime inDateTime)
        {
            if (loadedControl is ComponentList)
            {
                //if (((ComponentList)loadedControl).Option_ConvertUTCToLocalTime && inDateTime.Kind != DateTimeKind.Local)
                //{
                //    inDateTime = AppCentre.Data.Supporting.LocalDateTime.GetDate(inDateTime, ((ComponentList)loadedControl).GetInstance().wim.CurrentSite, true);
                //}
            }

            return inDateTime;
        }

        private static void SendNotification( string type, object loadedControl, System.Reflection.PropertyInfo info )
        {
            Notification.InsertOne("Internal Wim error", NotificationType.Error, 
                string.Format("Wrong type set for {0}<br/>Currently set: {1} for control: {2}", type, info.PropertyType.ToString(), loadedControl.GetType().ToString() ) 
                );
        }



	}
}
