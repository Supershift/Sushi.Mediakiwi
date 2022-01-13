using Sushi.Mediakiwi.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

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
        public async Task<Api.MediakiwiField> GetApiFieldAsync()
        {
            return new Api.MediakiwiField()
            {
                Event = m_AutoPostBack ? Api.MediakiwiJSEvent.Change : Api.MediakiwiJSEvent.None,
                Title = MandatoryWrap(Title),
                Value = OutputText,
                Expression = Expression,
                PropertyName = ID,
                PropertyType = (Property == null) ? typeof(DateTime).FullName : Property.PropertyType.FullName,
                VueType = Api.MediakiwiFormVueType.undefined,
                ReadOnly = IsReadOnly,
                ContentTypeID = ContentTypeSelection,
                IsAutoPostback = m_AutoPostBack,
                IsMandatory = Mandatory,
                MaxLength = MaxValueLength,
                HelpText = InteractiveHelp,
                FormSection = GetFormMapClass()
            };
        }

        private ForceContentTypes? _forceContenttype;
        /// <summary>
        /// Forces the selected types to apear in the Multiview attribute; concatenate types by using a pipe. For example
        /// ForceContentTypes.Image | ForceContentTypes.Header  
        /// Only above will work. If null all options show
        /// </summary>
        public ForceContentTypes? ForceContentTypeSelection
        {
            set { _forceContenttype = value; }
            get { return _forceContenttype; }
        }

        public MultiFieldAttribute(string title)  : this(title, null) { }

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
                    {
                        serialized = field.Value;
                    }
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
                if (Console.Request.HasFormContentType)
                {
                    var keys = Console.Request.Form.Keys;
                    var multiFound = (from x in keys where x.StartsWith(string.Concat(ID, "__")) select x).ToList();
                    if (multiFound.Count > 0)
                    {
                        serialized = MultiField.GetSerialized(Console.Request, multiFound.ToArray());
                    }
                }
                else
                {
                    serialized = null;
                }
            }

            // [MR:03-06-2021] Apply shared field clickable icon.
            var sharedInfoApply = ApplySharedFieldInformation(true, serialized);

            // If we have a Multifield assigned, overwrite the current one
            if (sharedInfoApply.isShared)
            {
                // When Currently not cloaked, do so if its a shared field
                if (IsCloaked == false && sharedInfoApply.isHidden)
                {
                    IsCloaked = sharedInfoApply.isHidden;
                }

                serialized = sharedInfoApply.outputValue;
            }

            _MultiFields = MultiField.GetDeserialized(serialized, ID);

            if (Property != null && Property.PropertyType == typeof(string))
            {
                Property.SetValue(SenderInstance, serialized, null);
            }

            int count = 0;
            List<IContentInfo> contentFields = new List<IContentInfo>();

            if (_MultiFields != null)
            {
                foreach (var fieldinstance in _MultiFields)
                {
                    MetaData meta = new MetaData() { ContentTypeSelection = fieldinstance.Type.ToString(), Name = fieldinstance.Property };
                    var element = meta.GetContentInfo();
                    if (element != null)
                    {
                        contentFields.Add(element);

                        ((ContentSharedAttribute)element).Console = Console;
                        ((ContentSharedAttribute)element).IsBluePrint = true;
                        count++;

                        //element.ID = fieldinstance.p
                        element.ID = string.Concat(ID, "__", meta.ContentTypeSelection, "__", count);
                        ((ContentSharedAttribute)element).m_ListItemCollection = meta.GetCollection(Console);
                        //((Framework.ContentSharedAttribute)element).Collection = Collection;

                        if (element.ContentTypeSelection == ContentType.Binary_Image)
                        {
                            ((Binary_ImageAttribute)element).GalleryProperty = GalleryProperty;
                        }
                   
                        element.SetCandidate(fieldinstance, Console.CurrentListInstance.wim.IsEditMode);
                    }
                    element.OverrideTableGeneration = true;
                }
            }
            ContentFields = contentFields.ToArray();
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
            IsCloaked = isCloaked;

            if (ShowInheritedData)
            {
                ApplyTranslation(build);
                Expression = OutputExpression.Right;
            }
            else
            {
                //Expression = OutputExpression.Right;
                if ((Console.ExpressionPrevious == OutputExpression.Left && Expression == OutputExpression.FullWidth) || (Console.ExpressionPrevious == OutputExpression.Left && Expression == OutputExpression.Left))
                {
                    build.Append("<th><label>&nbsp;</label></th><td>&nbsp;</td></tr>");
                }

                if ((Console.ExpressionPrevious == OutputExpression.FullWidth && Expression == OutputExpression.Right) || (Console.ExpressionPrevious == OutputExpression.Right && Expression == OutputExpression.Right))
                {
                    build.Append("<tr><th><label>&nbsp;</label></th><td>&nbsp;</td>");
                }

                if (Expression == OutputExpression.FullWidth || Expression == OutputExpression.Left)
                {
                    build.Append("<tr>");
                }
            }

            bool isEnabled = IsEnabled();
            string outputValue = OutputText;

            // [MR:03-06-2021] Apply shared field clickable icon.
            var sharedInfoApply = ApplySharedFieldInformation(isEnabled, outputValue);

            // If we have a document assigned, overwrite the current one
            if (sharedInfoApply.isShared)
            {
                // Enable readonly when shared
                isEnabled = sharedInfoApply.isEnabled;

                // When Currently not cloaked, do so if its a shared field
                if (IsCloaked == false && sharedInfoApply.isHidden)
                {
                    IsCloaked = sharedInfoApply.isHidden;
                }

                outputValue = sharedInfoApply.outputValue;
            }

            //if (Expression == OutputExpression.FullWidth)
            //    build.AppendFormat("\n\t\t\t\t\t\t\t<th colspan=\"{1}\"><label>{0}</label></th></tr></tr>", TitleLabel, Console.HasDoubleCols ? "4" : "2");
           
         
            if (string.IsNullOrWhiteSpace(EditSharedFieldLink) == false)
            {
                build.Append($"<th><label for=\"{ID}\">{EditSharedFieldLink.Replace("[LABEL]", TitleLabel)}</label></th>");
            }
            else
            {
                build.Append($"<th><label for=\"{ID}\">{TitleLabel}</label></th>");
            }
 
            build.AppendFormat("<td{0}{1}><div class=\"{3}\"> {2}"
                , (Expression == OutputExpression.FullWidth && Console.HasDoubleCols) ? " colspan=\"3\"" : null
                    //: " colspan=\"2\""//(Expression != OutputExpression.FullWidth) ? " colspan=\"2\"" : null
                , InputCellClassName(IsValid(isRequired))
                , CustomErrorText
                , isEditMode ? "multitarget" : "text"
            );

            if (Expression != OutputExpression.FullWidth)
            {
                if (string.IsNullOrWhiteSpace(EditSharedFieldLink) == false)
                {
                    build.Append($"<div>{EditSharedFieldLink.Replace("[LABEL]", TitleLabel)}</div>");
                }
                else
                {
                    build.Append($"<div>{TitleLabel}</div>");
                }
            }

            List<Field> fields = new List<Field>();
            int idx = 0;

            foreach (var contentField in ContentFields)
            {
                idx++;
                string sharedIcon = (string.IsNullOrWhiteSpace(SharedIcon) == false && IsCloaked == false && idx == 1) ? SharedIcon : "";

                build.AppendFormat(@"
                    <div class=""cmsable"">
                        {0}{1}
                        <table class=""formTable"">", sharedIcon , contentField.GetMultiFieldTitleHTML(isEditMode && isEnabled));

                build.AppendFormat(@"
                        <tbody>
                        <tr>
                        <td>");

                contentField.Expression = Expression;
                var outcome = contentField.WriteCandidate(build, Console.CurrentListInstance.wim.IsEditMode && isEnabled, contentField.Mandatory, contentField.IsCloaked);
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

            if (isEditMode && isEnabled)
            {
                string galleryUrlParam = null;
                Gallery gallery = null;
                if (!string.IsNullOrEmpty(GalleryProperty))
                {
                    var galleryProperty = SenderInstance.GetType().GetProperty(GalleryProperty);
                    var value = galleryProperty.GetValue(SenderInstance, null);

                    if (value is Gallery gallery1)
                    {
                        gallery = gallery1;
                    }
                    if (gallery != null && gallery.ID != 0)
                    {
                        galleryUrlParam = gallery.ID.ToString();//gallery.DatabaseMappingPortal == null ? gallery.ID.ToString() : gallery.CompletePath;
                    }
                }

                build.AppendFormat(@"</div><div class=""controls"" data-gallery=""{6}"" data-name=""{0}"" data-width=""{5}"" data-count=""{1}"">
                    <div class=""cmsBlocks"">
                        <input type=""hidden"" id=""{0}""  name=""{0}"" value=""1"" />", ID, ContentFields.Length.ToString()
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
                      build.AppendFormat(@"<a href=""#"" class=""block addContentType"" data-type=""12"">
                                          <span class=""icon-align-justify""></span>
                                          {0}
                                      </a>", Labels.ResourceManager.GetString("input_richtext", new CultureInfo(Console.CurrentApplicationUser.LanguageCulture)));
                }

                /// Header - textline
                if (!ForceContentTypeSelection.HasValue || (ForceContentTypeSelection & ForceContentTypes.Header) != 0)
                {
                    build.Append(@"<a href=""#"" class=""block addContentType"" data-type=""10"">
                                          <span class=""icon-header""></span>
                                          Header
                                      </a>");
                }

                /// Image
                if (!ForceContentTypeSelection.HasValue || (ForceContentTypeSelection & ForceContentTypes.Image) != 0)
                {
                    build.AppendFormat(@"<a href=""#"" class=""block addContentType"" data-type=""19"">
                                          <span class=""icon-photo""></span>
                                          {0}
                                      </a>",  Labels.ResourceManager.GetString("input_image", new CultureInfo(Console.CurrentApplicationUser.LanguageCulture)));
                }

                // code
                if (!ForceContentTypeSelection.HasValue || (ForceContentTypeSelection &  ForceContentTypes.SourceCode) != 0)
                {
                    build.AppendFormat(@"<a href=""#"" class=""block addContentType"" data-type=""39"">
                                          <span class=""icon-code""></span>
                                          {0}
                                      </a>", Labels.ResourceManager.GetString("input_code", new CultureInfo(Console.CurrentApplicationUser.LanguageCulture)));
                }

                // link
                if (!ForceContentTypeSelection.HasValue || (ForceContentTypeSelection &  ForceContentTypes.Hyperlink ) != 0)
                {
                    build.Append(@"<a href=""#"" class=""block addContentType"" data-type=""21"">
                                          <span class=""icon-external-link""></span>
                                          Link
                                      </a>");
                }

                // File
                if (!ForceContentTypeSelection.HasValue || (ForceContentTypeSelection & ForceContentTypes.File) != 0)
                {
                    build.Append(@"<a href=""#"" class=""block addContentType"" data-type=""20"">
                                          <span class=""icon-file""></span>
                                          File
                                      </a>");
                }
                
               
                build.Append(@"</div></div>");
            }

            build.Append("</td></tr>");
            //_output.Content = new Content() { Fields = fields.ToArray() };

            string serialized = null;
            if (sharedInfoApply.isShared)
            {
                serialized = sharedInfoApply.outputValue;
            }
            else if (_MultiFields != null)
            {
                serialized = Utility.GetSerialized(_MultiFields);
            }

            return ReadCandidate(serialized);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override bool IsValid(bool isRequired)
        {
            Mandatory = isRequired;
            return true;
        }
    }
}


    
