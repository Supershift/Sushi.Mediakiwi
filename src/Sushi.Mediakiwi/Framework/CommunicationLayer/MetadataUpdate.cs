using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace Sushi.Mediakiwi.Framework.CommunicationLayer
{
    /// <summary>
    /// 
    /// </summary>
    class MetadataUpdate
    {
        System.Web.UI.Page Page;
        System.Web.UI.WebControls.PlaceHolder Placeholder;
        System.Web.HttpResponse Response;
        System.Web.HttpRequest Request;

        internal int Initiate(System.Web.UI.WebControls.PlaceHolder placeHolder, int pageID)
        {
            Placeholder = placeHolder;
            var result = Initiate(placeHolder.Page, pageID);
            placeHolder.Controls.Clear();
            return result;
        }
        internal int Initiate(System.Web.UI.Page uipage, int pageID, bool isDebug = false, bool forceReload = false)
        {

            Page = uipage;
            Response = Page.Response;
            Request = Page.Request;

            int count = 0;

            Data.Page page = Sushi.Mediakiwi.Data.Page.SelectOne(pageID, false);

            System.Diagnostics.Trace.WriteLine($"Initiate page: {page.CompletePath}");

            HttpContext.Current.Items["Wim.Page"] = page;
            HttpContext.Current.Items["Wim.Site"] = page.Site;

            Sushi.Mediakiwi.Data.ComponentTemplate[] list = Sushi.Mediakiwi.Data.ComponentTemplate.SelectAllAvailable(page.TemplateID);
            foreach (Sushi.Mediakiwi.Data.ComponentTemplate template in list)
            {
                string path = Page.Server.MapPath(Utility.AddApplicationPath(template.Location));

                System.Diagnostics.Trace.WriteLine($"Initiate ct: {template.Location}");

                if (System.IO.File.Exists(path))
                {
                    System.IO.FileInfo nfo = new System.IO.FileInfo(path);

                    DateTime file = Wim.Utility.ConvertToSystemDataTime(nfo.LastWriteTimeUtc);
                    if (file.Ticks == template.LastWriteTimeUtc.Ticks)
                    {
                        if (!forceReload)
                        {
                            string flush = Request.QueryString["flush"];
                            if (string.IsNullOrEmpty(flush) || !flush.Equals("me", StringComparison.OrdinalIgnoreCase) )
                                continue;
                        }
                    }

                    bool isOk = false;
                    string xml = GetComponentTemplateXml(template.ID, ref isOk);
                    if (xml == null)
                    {
                        continue;
                    }
                    template.MetaData = xml;

                    if (isOk)
                        template.LastWriteTimeUtc = file;
                    template.Save();

                    if (isDebug)
                        Page.Response.Write(string.Format("Update Component#: {0} [{1}]<br/>", template.Name, template.Location));
                }
                else
                {
                    string flush = Request.QueryString["flush"];
                    if (!string.IsNullOrEmpty(flush) && flush.Equals("me", StringComparison.OrdinalIgnoreCase))
                    {
                        bool isOk = false;
                        string xml = GetComponentTemplateXml(template.ID, ref isOk);
                        if (xml == null)
                        {
                            continue;
                        }
                        template.MetaData = xml;
                        template.Save();
                    }

                    if (isDebug)
                        Page.Response.Write(string.Format("Could not find Component: {0} [{1}]<br/>", template.Name, template.Location));
                }

            }
            return count;
        }

        //internal int Initiate(System.Web.UI.Page callingPage, int pageId)
        //{
        //    Page = callingPage;
        //    Response = callingPage.Response;
        //    Request = callingPage.Request;

        //    int count = 0;

        //    Data.Page page = null;
        //    Sushi.Mediakiwi.Data.ComponentTemplate[] list = null;
        //    if (pageId > 0)
        //    {
        //        page = Sushi.Mediakiwi.Data.Page.SelectOne(pageId, false);
        //        HttpContext.Current.Items["Wim.Page"] = page;
        //        HttpContext.Current.Items["Wim.Site"] = page.Site;
        //        list = Sushi.Mediakiwi.Data.ComponentTemplate.SelectAllAvailable(page.TemplateID);
        //    }
        //    else
        //        list = Sushi.Mediakiwi.Data.ComponentTemplate.SelectAll();
            
        //    foreach (Sushi.Mediakiwi.Data.ComponentTemplate template in list)
        //    {
        //        if (pageId == 0)
        //        {
        //            page = Data.Page.SelectOneByComponentTemplate(template.ID);
        //            if (!page.IsNewInstance)
        //            {
        //                HttpContext.Current.Items["Wim.Page"] = page;
        //                HttpContext.Current.Items["Wim.Site"] = page.Site;
        //            }
        //        }

        //        string xml = GetComponentTemplateXml(template.ID);
        //        if (xml == null)
        //        {
        //            Page.Controls.Clear();
        //            continue;
        //        }

        //        count++;

        //        template.MetaData = xml;

        //        System.IO.FileInfo nfo = new System.IO.FileInfo(callingPage.Server.MapPath(Utility.AddApplicationPath(template.Location)));
        //        template.LastWriteTimeUtc = Wim.Utility.ConvertToSystemDataTime(nfo.LastWriteTimeUtc);

        //        template.Save();
        //        Page.Controls.Clear();
        //    }
        //    return count;
        //    //Page.Response.Write(string.Format("Update : {0} templates", count));
        //}

        string m_Error;
        System.Web.UI.Control LoadSpecifiedControl(string relativePath)
        {
            try
            {
                System.Diagnostics.Trace.WriteLine($"Load control [meta]: {relativePath}");
                System.Web.UI.Control control = Page.LoadControl(relativePath);
                System.Diagnostics.Trace.WriteLine($"control loaded [meta]");

                if (control == null) return null;

                if (Placeholder == null)
                    Page.Controls.Add(control);
                else
                    Placeholder.Controls.Add(control);

                Sushi.Mediakiwi.Framework.ComponentTemplate uc = control as Sushi.Mediakiwi.Framework.ComponentTemplate;
                if (uc == null)
                {
                    System.Web.UI.PartialCachingControl pcc = control as System.Web.UI.PartialCachingControl;
                    if (pcc != null)
                    {
                        uc = pcc.CachedControl as Sushi.Mediakiwi.Framework.ComponentTemplate;
                        if (uc != null)
                        {
                            uc.wim.IsLoadedInWim = true;
                        }
                    }
                }
                else
                    uc.wim.IsLoadedInWim = true;

                return uc;
            }
            catch (Exception ex)
            {
                m_Error = string.Format("<b>The following usercontrol could not be loaded:<br/>{0}.</b><br/><br/><b>{1}</b><br/>{2}", relativePath, ex.Message, ex.StackTrace == null ? null : ex.StackTrace.Replace(" at", "<br/>at"));
                return null;
            }
        }

        ListItemCollection GetProperty(object sender, string property)
        {
            foreach (System.Reflection.PropertyInfo info in sender.GetType().GetProperties())
            {   //  Get all public properties
                if (info.CanRead)
                {   // Get all writable public properties
                    if (info.Name == property)
                    {
                        return (ListItemCollection)info.GetValue(sender, null);
                    }
                }
            }
            throw new Exception(string.Format("Could not find the '{0}' property.", property));
        }

        string GetComponentTemplateXml(int fileReference, ref bool isOk)
        {
            System.Diagnostics.Trace.WriteLine($"GetComponentTemplateXml");

            isOk = true;
            //  Stop events
            HttpContext.Current.Items["Wim.stop"] = "1";

            Sushi.Mediakiwi.Data.ComponentTemplate template = Sushi.Mediakiwi.Data.ComponentTemplate.SelectOne(fileReference);
            
            string path = Wim.Utility.AddApplicationPath(template.Location);

            if (Request.IsLocal)
            {
                if (!System.IO.File.Exists(Page.Server.MapPath(path)))
                    return null;
            }

            System.Web.UI.Control loadedControl = LoadSpecifiedControl(path);

            List<MetaData> list = new List<MetaData>();

       

            if (loadedControl == null)
            {
                System.Diagnostics.Trace.WriteLine($"loadedControl error!");

                if (Request.IsLocal)
                {
                    MetaData error = new MetaData();
                    error.Title = string.Concat(">internal_error|", m_Error);
                    error.ContentTypeSelection = ((int)Sushi.Mediakiwi.Framework.ContentType.TextLine).ToString();
                    list.Add(error);
                    isOk = false;
                }
                else
                {
                    isOk = false;
                    return null;
                }
            }
            else
            {
                try
                {
                    foreach (System.Reflection.PropertyInfo info in loadedControl.GetType().GetProperties())
                    {   //  Get all public properties
                        if (info.CanWrite)
                        {   // Get all writable public properties
                            foreach (object attribute in info.GetCustomAttributes(false))
                            {
                                //  Get all custom attributes
                                if (attribute is Sushi.Mediakiwi.Framework.IContentInfo)
                                {
                                    if (attribute is Sushi.Mediakiwi.Framework.IListContentInfo)
                                    {
                                        continue;
                                    }
                                    if (attribute is Sushi.Mediakiwi.Framework.IListSearchContentInfo)
                                    {
                                        continue;
                                    }

                                    ListItemCollection x = new ListItemCollection();
                                    if (((Sushi.Mediakiwi.Framework.IContentInfo)attribute).ContentTypeSelection == Sushi.Mediakiwi.Framework.ContentType.Choice_Dropdown)
                                    {
                                        ((Sushi.Mediakiwi.Framework.ContentInfoItem.Choice_DropdownAttribute)attribute).IsMultiSelect = (info.PropertyType == typeof(List<int>))
                                            || (info.PropertyType == typeof(int[]))
                                            || (info.PropertyType == typeof(List<string>))
                                            || (info.PropertyType == typeof(string[]))
                                            || (info.PropertyType == typeof(Option[]))
                                            || (info.PropertyType == typeof(List<Option>));
                                        
                                        list.Add(((Sushi.Mediakiwi.Framework.IContentInfo)attribute).GetMetaData(info.Name,
                                            GetProperty(loadedControl, ((Sushi.Mediakiwi.Framework.ContentInfoItem.Choice_DropdownAttribute)attribute).CollectionProperty)
                                            ));
                                        continue;
                                    }
                                    else if (((Sushi.Mediakiwi.Framework.IContentInfo)attribute).ContentTypeSelection == Sushi.Mediakiwi.Framework.ContentType.Choice_Radio)
                                    {
                                        list.Add(((Sushi.Mediakiwi.Framework.IContentInfo)attribute).GetMetaData(info.Name,
                                            GetProperty(loadedControl, ((Sushi.Mediakiwi.Framework.ContentInfoItem.Choice_RadioAttribute)attribute).CollectionProperty)
                                            ));
                                        continue;
                                    }
                                    else if (((Sushi.Mediakiwi.Framework.IContentInfo)attribute).ContentTypeSelection == Sushi.Mediakiwi.Framework.ContentType.ListItemSelect)
                                    {
                                        list.Add(((Sushi.Mediakiwi.Framework.IContentInfo)attribute).GetMetaData(info.Name,
                                            GetProperty(loadedControl, ((Sushi.Mediakiwi.Framework.ContentInfoItem.ListItemSelectAttribute)attribute).CollectionProperty)
                                            ));
                                        continue;
                                    }

                                    list.Add(((Sushi.Mediakiwi.Framework.IContentInfo)attribute).GetMetaData(info.Name));
                                }
                            }
                        }
                    }
                    ScanForWimControls(loadedControl, loadedControl.Controls, list);
                }
                catch (Exception ex)
                {
                    isOk = false;
                    throw new Exception(string.Format("User control (component template) having this error: {0}", loadedControl.ToString()), ex);
                    //if (Request.IsLocal)
                    //{
                    //    //MetaData error = new MetaData();
                    //    //error.Title = string.Concat(">internal_error|", string.Format("<b>{0}</b><br/><br/>{1}", ex.Message, ex.StackTrace == null ? null : ex.StackTrace.Replace(" at", "<br/>at")));
                    //    //error.ContentTypeSelection = ((int)Sushi.Mediakiwi.Framework.ContentType.TextLine).ToString();
                    //    //list.Add(error);
                    //}
                    //return null;
                }
            }

            string xml = Wim.Utility.GetSerialized(list.ToArray());
            return xml;
        }

        /// <summary>
        /// Scans for wim controls.
        /// </summary>
        /// <param name="loadedControl">The loaded control.</param>
        /// <param name="collection">The collection.</param>
        /// <param name="list">The list.</param>
        void ScanForWimControls(Control loadedControl, ControlCollection collection, List<MetaData> list)
        {
            foreach (System.Web.UI.Control c in collection)
            {
                IContentInfo ic = null;
                string defaulValue = null;
                if (c.GetType().BaseType == typeof(Sushi.Mediakiwi.Framework.ControlLib.Base.ContentInfo))
                {
                    if (((Sushi.Mediakiwi.Framework.ControlLib.Base.ContentInfo)c).HasReflection)
                        continue;

                    if (c.GetType() == typeof(Sushi.Mediakiwi.Framework.ControlLib.WimText))
                    {
                        Sushi.Mediakiwi.Framework.ControlLib.WimText wimText = ((Sushi.Mediakiwi.Framework.ControlLib.WimText)c);
                        defaulValue = wimText.Text;

                        if (wimText.TextMode == Sushi.Mediakiwi.Framework.ControlLib.WimText.Type.TextArea)
                        {
                            ic = new Sushi.Mediakiwi.Framework.ContentInfoItem.TextAreaAttribute(
                                wimText.Title, wimText.MaxLength, wimText.Mandatory, wimText.HelpText);
                        }
                        else if (wimText.TextMode == Sushi.Mediakiwi.Framework.ControlLib.WimText.Type.RichText)
                        {
                            ic = new Sushi.Mediakiwi.Framework.ContentInfoItem.RichTextAttribute(
                                wimText.Title, wimText.MaxLength, wimText.Mandatory, wimText.HelpText)
                            { EnableTable = wimText.EnableTable };
                            
                        }
                      
                        else if (wimText.TextMode == Sushi.Mediakiwi.Framework.ControlLib.WimText.Type.TextLine)
                        {
                            ic = new Sushi.Mediakiwi.Framework.ContentInfoItem.TextLineAttribute(wimText.Title);
                        }
                        else
                        {
                            ic = new Sushi.Mediakiwi.Framework.ContentInfoItem.TextFieldAttribute(
                                wimText.Title, wimText.MaxLength, wimText.Mandatory, wimText.HelpText);
                        }
                    }
                    else if (c.GetType() == typeof(Sushi.Mediakiwi.Framework.ControlLib.WimMulti))
                    {
                        Sushi.Mediakiwi.Framework.ControlLib.WimMulti wimText = ((Sushi.Mediakiwi.Framework.ControlLib.WimMulti)c);
                        defaulValue = wimText.Text;

                        ic = new Sushi.Mediakiwi.Framework.ContentInfoItem.MultiFieldAttribute(
                                             wimText.Title, wimText.HelpText);
                    }
                    else if (c.GetType() == typeof(Sushi.Mediakiwi.Framework.ControlLib.WimLabel))
                    {
                        Sushi.Mediakiwi.Framework.ControlLib.WimLabel wimText = ((Sushi.Mediakiwi.Framework.ControlLib.WimLabel)c);
                        //CB; nu kan ook de wim label een default value hebben
                        if (wimText != null && !String.IsNullOrEmpty(wimText.Text))
                        defaulValue = wimText.Text;

                        ic = new Sushi.Mediakiwi.Framework.ContentInfoItem.TextFieldAttribute(
                                wimText.Title, wimText.MaxLength, wimText.Mandatory, wimText.HelpText);

                    }
                    else if (c.GetType() == typeof(Sushi.Mediakiwi.Framework.ControlLib.WimImage))
                    {
                        Sushi.Mediakiwi.Framework.ControlLib.WimImage wimImage = ((Sushi.Mediakiwi.Framework.ControlLib.WimImage)c);
                        ic = new Sushi.Mediakiwi.Framework.ContentInfoItem.Binary_ImageAttribute(
                                wimImage.Title, wimImage.Mandatory, wimImage.HelpText);
                    }
                    else if (c.GetType() == typeof(Sushi.Mediakiwi.Framework.ControlLib.WimLink))
                    {
                        Sushi.Mediakiwi.Framework.ControlLib.WimLink wimLink = ((Sushi.Mediakiwi.Framework.ControlLib.WimLink)c);

                        if (wimLink.IsPagelink)
                            ic = new Sushi.Mediakiwi.Framework.ContentInfoItem.PageSelectAttribute(wimLink.Title, wimLink.Mandatory, wimLink.HelpText);
                        else
                            ic = new Sushi.Mediakiwi.Framework.ContentInfoItem.HyperlinkAttribute(wimLink.Title, wimLink.Mandatory, wimLink.HelpText);
                    }
                    else if (c.GetType() == typeof(Sushi.Mediakiwi.Framework.ControlLib.WimDocument))
                    {
                        Sushi.Mediakiwi.Framework.ControlLib.WimDocument wimDocument = ((Sushi.Mediakiwi.Framework.ControlLib.WimDocument)c);
                        ic = new Sushi.Mediakiwi.Framework.ContentInfoItem.Binary_DocumentAttribute(
                            wimDocument.Title, wimDocument.Mandatory, wimDocument.HelpText);
                    }

                    if (ic != null) list.Add(ic.GetMetaData(c.ID, defaulValue));
                }

                Sushi.Mediakiwi.Framework.iMetaDataContainer imeta = c as Sushi.Mediakiwi.Framework.iMetaDataContainer;
                if (imeta != null)
                    ScanForWimControls(loadedControl, imeta.GetInnerControlCollection(), list);

                ScanForWimControls(loadedControl, c.Controls, list);
            }
        }
    }
}
