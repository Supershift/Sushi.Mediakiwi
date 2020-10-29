using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Sushi.Mediakiwi.Data;
using System.Globalization;

namespace Sushi.Mediakiwi.Framework
{
    public enum ForceContentTypes
    {
        Image = 1,
        Header = 2,
        SourceCode = 4,
        Hyperlink = 8,
        Text = 16,
        File = 32,
    }
}
namespace Sushi.Mediakiwi.Framework.ContentInfoItem
{

   

    /// <summary>
    /// Possible return types: System.String
    /// </summary>
    public class MultiFieldAttribute : ContentSharedAttribute, IContentInfo, IListContentInfo 
    {
        private ForceContentTypes? _forceContenttype;
        /// <summary>
        /// Forces the selected types to apear in the Multiview attribute; concatenate types by using a pipe. For example
        /// ForceContentTypes.Image | ForceContentTypes.Header  
        /// Only above will work. If null all options show
        /// </summary>
        public ForceContentTypes? ForceContentTypeSelection
        {
            set { this._forceContenttype = value; }
            get { return this._forceContenttype; }
        }

        public MultiFieldAttribute(string title) 
            : this(title, null) { }

        public MultiFieldAttribute(string title, string interactiveHelp)
        {
            m_CanHaveExpression = true;
            Title = title;
            InteractiveHelp = interactiveHelp;
            ContentTypeSelection = ContentType.MultiField;
        }

        public MultiFieldAttribute(string title, string interactiveHelp, ForceContentTypes forcetypes)
        {
            m_CanHaveExpression = true;
            Title = title;
            InteractiveHelp = interactiveHelp;
            ForceContentTypeSelection = forcetypes;
            ContentTypeSelection = ContentType.MultiField;
        }

        public string GalleryProperty { get; set; }

        /// <summary>
        /// Sets the candidate.
        /// </summary>
        /// <param name="isEditMode"></param>
        public void SetCandidate(bool isEditMode)
        {
        }


        MultiField[] _MultiFields;

        IContentInfo[] ContentFields { get; set; }

        /// <summary>
        /// Sets the candidate.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="isEditMode">if set to <c>true</c> [is edit mode].</param>
        public void SetCandidate(Field field, bool isEditMode)
        {
            string serialized = null;
            if (IsInitialLoad)
            {
                if (IsBluePrint)
                {
                    if (field != null)
                        serialized = field.Value;
                }
                else
                {
                    if (Property != null && Property.PropertyType == typeof(string))
                    {
                        serialized = Property.GetValue(SenderInstance, null) as string;
                    }
                }
            }
            else
            {
                if (this.Console.Request.HasFormContentType)
                {
                    var keys = this.Console.Request.Form.Keys;
                    var multiFound = (from x in keys where x.StartsWith(string.Concat(this.ID, "__")) select x).ToList();
                    if (multiFound.Count > 0)
                    {
                        serialized = MultiField.GetSerialized(this.Console.Request, multiFound.ToArray());
                    }
                }
                else
                    serialized = null;
            }

            this._MultiFields = MultiField.GetDeserialized(serialized, this.ID);

            if (Property != null && Property.PropertyType == typeof(string))
            {
                Property.SetValue(SenderInstance, serialized, null);
            }

            int count = 0;
            List<IContentInfo> contentFields = new List<IContentInfo>();

            if (this._MultiFields != null)
            {
                foreach (var fieldinstance in this._MultiFields)
                {
                    MetaData meta = new MetaData() { ContentTypeSelection = fieldinstance.Type.ToString(), Name = fieldinstance.Property };
                    var element = meta.GetContentInfo();
                    if (element != null)
                    {
                        contentFields.Add(element);

                        ((Framework.ContentSharedAttribute)element).Console = this.Console;
                        ((Framework.ContentSharedAttribute)element).IsBluePrint = true;
                        count++;

                        //element.ID = fieldinstance.p
                        element.ID = string.Concat(this.ID, "__", meta.ContentTypeSelection, "__", count);
                        ((Framework.ContentSharedAttribute)element).m_ListItemCollection = meta.GetCollection();
                        //((Framework.ContentSharedAttribute)element).Collection = this.Collection;

                        if (element.ContentTypeSelection == ContentType.Binary_Image)
                        {
                            ((Framework.ContentInfoItem.Binary_ImageAttribute)element).GalleryProperty = this.GalleryProperty;
                        }
                   
                        element.SetCandidate(fieldinstance, Console.CurrentListInstance.wim.IsEditMode);
                    }
                    element.OverrideTableGeneration = true;
                }
            }
            this.ContentFields = contentFields.ToArray();
        }


        /// <summary>
        /// Writes the candidate.
        /// </summary>
        /// <param name="build">The build.</param>
        /// <param name="isEditMode">if set to <c>true</c> [is edit mode].</param>
        /// <param name="isRequired">if set to <c>true</c> [is required].</param>
        /// <returns></returns>
        public Field WriteCandidate(WimControlBuilder build, bool isEditMode, bool isRequired, bool isCloaked)
        {
            this.IsCloaked = isCloaked;

            if (ShowInheritedData)
            {
                this.ApplyTranslation(build);
                Expression = OutputExpression.Right;
            }
            else
            {
                //Expression = OutputExpression.Right;
                if ((Console.ExpressionPrevious == OutputExpression.Left && Expression == OutputExpression.FullWidth) || (Console.ExpressionPrevious == OutputExpression.Left && Expression == OutputExpression.Left))
                    build.Append("\t\t\t\t\t\t\t<th><label>&nbsp;</label></th><td>&nbsp;</td></tr>");

                if ((Console.ExpressionPrevious == OutputExpression.FullWidth && Expression == OutputExpression.Right) || (Console.ExpressionPrevious == OutputExpression.Right && Expression == OutputExpression.Right))
                    build.Append("\t\t\t\t\t\t<tr><th><label>&nbsp;</label></th>\n\t\t\t\t\t\t\t<td>&nbsp;</td>");

                if (Expression == OutputExpression.FullWidth || Expression == OutputExpression.Left)
                    build.Append("\t\t\t\t\t\t<tr>");
            }

            //if (Expression == OutputExpression.FullWidth)
            //    build.AppendFormat("\n\t\t\t\t\t\t\t<th colspan=\"{1}\"><label>{0}</label></th></tr></tr>", this.TitleLabel, Console.HasDoubleCols ? "4" : "2");

            build.AppendFormat("\n\t\t\t\t\t\t\t<th><label>{0}</label></th>", this.TitleLabel);

            build.AppendFormat("\n\t\t\t\t\t\t\t<td{0}{1}><div class=\"{3}\"> {2}"
                , (Expression == OutputExpression.FullWidth && Console.HasDoubleCols) ? " colspan=\"3\"" : null
                    //: " colspan=\"2\""//(Expression != OutputExpression.FullWidth) ? " colspan=\"2\"" : null
                , this.InputCellClassName(this.IsValid(isRequired))
                , CustomErrorText
                , isEditMode ? "multitarget" : "text"
            );

            if (Expression != OutputExpression.FullWidth)
                build.AppendFormat("\n\t\t\t\t\t\t\t\t<div>{0}</div>", this.TitleLabel);

            List<Field> fields = new List<Field>();
            foreach (var contentField in this.ContentFields)
            {
                
                build.AppendFormat(@"
                    <div class=""cmsable"">
                        {0}
                        <table class=""formTable"">", contentField.GetMultiFieldTitleHTML(isEditMode));

                build.AppendFormat(@"
                        <tbody>
                        <tr>
                        <td>");

                contentField.Expression = this.Expression;
                var outcome = contentField.WriteCandidate(build, Console.CurrentListInstance.wim.IsEditMode, contentField.Mandatory, contentField.IsCloaked);
                fields.Add(outcome);

                build.AppendFormat(@"
                        </td>
                        </tr>
                        </tbody>");

                build.AppendFormat(@"
                        </table>
                    </div>");
            }
            Console.CurrentListInstance.wim.Page.Head.EnableColorCodingLibrary = true;
            if (isEditMode)
            {
                string galleryUrlParam = null;
                Data.Gallery gallery = null;
                if (!string.IsNullOrEmpty(this.GalleryProperty))
                {
                    var galleryProperty = this.SenderInstance.GetType().GetProperty(this.GalleryProperty);
                    var value = galleryProperty.GetValue(this.SenderInstance, null);

                    if (value is Sushi.Mediakiwi.Data.Gallery)
                    {
                        gallery = (Sushi.Mediakiwi.Data.Gallery)value;
                    }
                    if (gallery != null && gallery.ID != 0)
                        galleryUrlParam = gallery.ID.ToString();//gallery.DatabaseMappingPortal == null ? gallery.ID.ToString() : gallery.CompletePath;
                }
                build.AppendFormat(@"                            </div><div class=""controls"" data-gallery=""{6}"" data-name=""{0}"" data-width=""{5}"" data-count=""{1}"">
                    <div class=""cmsBlocks"">
                        <input type=""hidden"" id=""{0}""  name=""{0}"" value=""1"" />", this.ID, this.ContentFields.Length.ToString()
                                         , Labels.ResourceManager.GetString("input_richtext", new CultureInfo(Console.CurrentApplicationUser.LanguageCulture))
                                         , Labels.ResourceManager.GetString("input_image", new CultureInfo(Console.CurrentApplicationUser.LanguageCulture))
                                         , Labels.ResourceManager.GetString("input_code", new CultureInfo(Console.CurrentApplicationUser.LanguageCulture))
                                         , Expression == OutputExpression.FullWidth ? "2" : "1"
                                         , galleryUrlParam
                                         , Labels.ResourceManager.GetString("input_link", new CultureInfo(Console.CurrentApplicationUser.LanguageCulture))
                                         );
                /// Text adding - RTE
                if (!ForceContentTypeSelection.HasValue || (ForceContentTypeSelection & ForceContentTypes.Text) != 0)
                {
                      build.AppendFormat(@"  <a href=""#"" class=""block addContentType"" data-type=""12"">
                                          <span class=""icon-align-justify""></span>
                                          {0}
                                      </a>", Labels.ResourceManager.GetString("input_richtext", new CultureInfo(Console.CurrentApplicationUser.LanguageCulture)));
                }
                /// Header - textline
                if (!ForceContentTypeSelection.HasValue || (ForceContentTypeSelection & ForceContentTypes.Header) != 0)
                {
                    build.Append(@"  
                                      <a href=""#"" class=""block addContentType"" data-type=""10"">
                                          <span class=""icon-header""></span>
                                          Header
                                      </a>");
                }
                /// Image
                if (!ForceContentTypeSelection.HasValue || (ForceContentTypeSelection & ForceContentTypes.Image) != 0)
                {
                    build.AppendFormat(@"  
                                          
                                      <a href=""#"" class=""block addContentType"" data-type=""19"">
                                          <span class=""icon-photo""></span>
                                          {0}
                                      </a>",  Labels.ResourceManager.GetString("input_image", new CultureInfo(Console.CurrentApplicationUser.LanguageCulture)));
                }
                // code
                if (!ForceContentTypeSelection.HasValue || (ForceContentTypeSelection &  ForceContentTypes.SourceCode) != 0)
                {
                    build.AppendFormat(@"  
                                       <a href=""#"" class=""block addContentType"" data-type=""39"">
                                          <span class=""icon-code""></span>
                                          {0}
                                      </a>", Labels.ResourceManager.GetString("input_code", new CultureInfo(Console.CurrentApplicationUser.LanguageCulture)));
                }
                // link
                if (!ForceContentTypeSelection.HasValue || (ForceContentTypeSelection &  ForceContentTypes.Hyperlink ) != 0)
                {
                    build.Append(@"  
                                        <a href=""#"" class=""block addContentType"" data-type=""21"">
                                          <span class=""icon-external-link""></span>
                                          Link
                                      </a>");
                }
                // File
                if (!ForceContentTypeSelection.HasValue || (ForceContentTypeSelection & ForceContentTypes.File) != 0)
                {
                    build.Append(@"  
                                        <a href=""#"" class=""block addContentType"" data-type=""20"">
                                          <span class=""icon-file""></span>
                                          File
                                      </a>");
                }
                
               
                build.Append(@"   
                                  </div>
                              </div>");
            }
            build.Append("\t\t\t\t\t\t\t</td>");
            build.Append("\t\t\t\t\t\t</tr>");

            //this._output.Content = new Content() { Fields = fields.ToArray() };

            string serialized = null;
            if (this._MultiFields != null)
                serialized = Data.Utility.GetSerialized(this._MultiFields);

            return ReadCandidate(serialized);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override bool IsValid(bool isRequired)
        {
            this.Mandatory = isRequired;
            return true;
        }
    }
}


    
