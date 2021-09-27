using Sushi.Mediakiwi.Data;
using Sushi.Mediakiwi.RichRext;
using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Sushi.Mediakiwi.Framework.ContentInfoItem
{
    /// <summary>
    /// Possible return types: System.String
    /// </summary>
    public class RichTextAttribute : ContentEditableSharedAttribute, IContentInfo
    {
        public const string OPTION_ENABLE_TABLE = "table";

        public override MetaData GetMetaData(string name, string defaultValue)
        {
            MetaData meta = new MetaData();
            Utility.ReflectProperty(this, meta);
            meta.Name = name;
            meta.Default = defaultValue;
            meta.ContentTypeSelection = ((int)ContentTypeSelection).ToString();

            if (EnableTable)
            {
                meta.AddOption(OPTION_ENABLE_TABLE);
            }
            else
            {
                meta.RemoveOption(OPTION_ENABLE_TABLE);
            }

            return meta;
        }

        /// <summary>
        /// Possible return types: System.String
        /// </summary>
        /// <param name="title"></param>
        /// <param name="maxlength"></param>
        public RichTextAttribute(string title, int maxlength)
            : this(title, maxlength, false) { }

        /// <summary>
        /// Possible return types: System.String
        /// </summary>
        /// <param name="title"></param>
        /// <param name="maxlength"></param>
        /// <param name="mandatory"></param>
        public RichTextAttribute(string title, int maxlength, bool mandatory)
            : this(title, maxlength, mandatory, null) { }

        /// <summary>
        /// Possible return types: System.String
        /// </summary>
        /// <param name="title"></param>
        /// <param name="maxlength"></param>
        /// <param name="mandatory"></param>
        /// <param name="interactiveHelp"></param>
        public RichTextAttribute(string title, int maxlength, bool mandatory, string interactiveHelp)
        {
            ContentTypeSelection = ContentType.RichText;
            Title = title;
            MaxValueLength = maxlength;
            Mandatory = mandatory;
            InteractiveHelp = interactiveHelp;

        }

        private bool m_CleanParagraphWrap;
        /// <summary>
        /// 
        /// </summary>
        public bool CleanParagraphWrap
        {
            set { m_CleanParagraphWrap = value; }
            get { return m_CleanParagraphWrap; }
        }

        public bool EnableTable { get; set; }


        /// <summary>
        /// This doesn't clean the html so anything is possible; be careful
        /// </summary>
        public bool NoCleaning { get; set; }

        /// <summary>
        /// Sets the candidate.
        /// </summary>
        /// <param name="isEditMode"></param>
        public void SetCandidate(bool isEditMode)
        {
            SetCandidate(null, isEditMode);
        }

        public static Regex _table = new Regex(@"(<table[^>]*>(?:.|\n)*?<\/table>)", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);
        static IRichTextDataCleaner _richTextDataCleanHandler;

        /// <summary>
        /// Sets the candidate.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="isEditMode">if set to <c>true</c> [is edit mode].</param>
        public void SetCandidate(Field field, bool isEditMode)
        {
            SetMultiFieldTitleHTML(Labels.ResourceManager.GetString("input_richtext", new CultureInfo(Console.CurrentApplicationUser.LanguageCulture)), "icon-align-justify");

            if (Property != null && Property.PropertyType == typeof(CustomData))
            {
                SetContentContainer(field);
            }

            string candidate = null;
            if (IsInitialLoad || !isEditMode)
            {
                if (IsBluePrint)
                {
                    if (field != null)
                    {
                        candidate = field.Value;
                    }
                }
                else
                {
                    if (Property.PropertyType == typeof(CustomData))
                    {
                        candidate = m_ContentContainer[field.Property].Value;
                    }
                    else if (Property.PropertyType == typeof(string))
                    {
                        object value = Property.GetValue(SenderInstance, null);
                        if (value != null)
                        {
                            candidate = value.ToString();
                        }
                    }
                }
            }
            else
            {
                candidate = Console.Form(ID.Replace("$", ""));

                if (candidate == null)
                {
                    candidate = string.Empty;
                }

                if (!EnableTable)
                {
                    candidate = _table.Replace(candidate, "");
                }

                candidate = Utils.CleanLink(Console.CurrentListInstance.wim.CurrentSite, candidate);
            }

            if (!string.IsNullOrEmpty(candidate))
            {
                candidate = candidate.Replace("\t", string.Empty).Replace("\r\n", " ").Replace("\r", string.Empty).Replace("\n", string.Empty);

                //if candidate is empy or contains only html tags and no real content, set candidate to null
                if (string.IsNullOrEmpty(Utility.CleanFormatting(candidate)))
                {
                    candidate = null;
                }
                else
                {
                    string emptyTest = candidate.Replace("&nbsp;", string.Empty).Trim();
                    if (string.IsNullOrEmpty(emptyTest) || string.IsNullOrEmpty(Utility.CleanFormatting(emptyTest)))
                    {
                        candidate = null;
                    }
                }
            }

            if (NoCleaning == false)
            {
                Cleaner cleaner = new Cleaner();
                candidate = cleaner.ApplyFullClean(candidate, false);
            }

            if (!IsBluePrint && Property != null && Property.CanWrite)
            {
                if (Property.PropertyType == typeof(CustomData))
                {
                    ApplyContentContainer(field, candidate);
                }
                else if (Property.PropertyType == typeof(string))
                {
                    Property.SetValue(SenderInstance, candidate, null);
                }
            }

            OutputText = candidate;

            //  Inherited content section
            if (ShowInheritedData && field != null && !string.IsNullOrEmpty(field.InheritedValue))
            {
                string candidate2 = field.InheritedValue;
                RichTextLink.CreateLinkPreview(ref candidate2);
                InhertitedOutputText = candidate2;
            }
        }

        static Regex striketroughFix = new Regex(@"<span\sstyle=""text-decoration:line-through;"">(.*?)</span>", RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);
        static Regex underlineFix = new Regex(@"<span\sstyle=""text-decoration:underline;"">(.*?)</span>", RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);
        static Regex tofillFails = new Regex(@"<a\shref=""TOFILL"">(.*?)</a>",  RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);

        Editor m_NewEditor;
        internal Editor NewEditor
        {
            get
            {
                if (m_NewEditor == null)
                {
                    SetNewEditor();
                }
                return m_NewEditor;
            }
        }

        void SetNewEditor()
        {

            m_NewEditor = new Editor()
            {
                ID = "ta_" + ID
            };
     
            m_NewEditor.ID = ID.Replace("$", string.Empty);
            string item = OutputText;

            //replace all strong tags with b tags, because of firefox bug. 
            if (item != null)
            {
                var buttonCleaner = new CleanupRichTextButtons();
                item = buttonCleaner.ApplyBold(item);
            }

            if (!string.IsNullOrEmpty(item) && item.StartsWith("<p>", StringComparison.OrdinalIgnoreCase))
            {
                m_NewEditor.Content = Utility.CleanParagraphWrap(item);
            }
            else
            {
                m_NewEditor.Content = item;
            }
        }

        public class Editor
        {

            public string EditorHTML(bool isEnabled, bool hasTable = false, bool isCloaked = false)
            {
                if (isCloaked)
                {
                    return $@"<textarea class=""long hidden"" id=""{ID}"" name=""{ID}""{(isEnabled ? null : " disabled=\"disabled\"")}>{CleanUpBadChars(Content)}</textarea>";
                }

                return $@"<textarea class=""long {(hasTable ? "table_rte" : "rte")}"" id=""{ID}"" name=""{ID}""{(isEnabled ? null : " disabled=\"disabled\"")}>{CleanUpBadChars(Content)}</textarea>";
            }

            public string ID { get; set; }

            private string CleanUpBadChars(string Content)
            {
                if (Content != null)
                {
                    return Content.Replace("{", "&#123;").Replace("}", "&#125;");
                }

                return Content;
            }
            public string Content { get; set; }
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
            SetWriteEnvironment();
            IsCloaked = isCloaked;
            Mandatory = isRequired;
            if (OverrideEditMode)
            {
                isEditMode = false;
            }

            string outputValue = OutputText;

            var isEnabled = IsEnabled();

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

                OutputText = sharedInfoApply.outputValue;
            }

            if (isEditMode && isEnabled)
            {
                #region Element creation

                StringBuilder element = new StringBuilder();
                element.AppendFormat(NewEditor.EditorHTML(isEnabled, EnableTable, IsCloaked));

                #endregion Element creation

                if (IsCloaked)
                {
                    build.AppendCloaked(element.ToString());
                }
                else
                {
                    #region Wrapper

                    //string titleTag = string.Concat(Title, Mandatory ? "<em>*</em>" : "");

                    //  If set all table cell/row creation will be ignored
                    if (!OverrideTableGeneration)
                    {
                        if (ShowInheritedData)
                        {
                            ApplyTranslation(build);
                        }
                        else
                        {
                            if (Console.ExpressionPrevious == OutputExpression.Left)
                            {
                                build.Append("<th><label>&nbsp;</label></th><td>&nbsp;</td></tr>");
                            }

                            if (Expression == OutputExpression.FullWidth || Expression == OutputExpression.Left)
                            {
                                build.Append("<tr>");
                            }
                        }

                        if (string.IsNullOrWhiteSpace(EditSharedFieldLink) == false)
                        {
                            build.Append($"<th><label for=\"{ID}\">{EditSharedFieldLink.Replace("[LABEL]", TitleLabel)}</label></th>");
                        }
                        else
                        {
                            build.Append($"<th><label for=\"{ID}\">{TitleLabel}</label></th>");
                        }

                        build.AppendFormat("<td{0}{1}>{2}"
                            , (Expression == OutputExpression.FullWidth && Console.HasDoubleCols) ? " colspan=\"3\"" : null
                            , InputCellClassName(IsValid(isRequired))
                            , CustomErrorText
                            );
                    }

                    build.AppendFormat("<div class=\"{0}\">", (Expression == OutputExpression.FullWidth) ? Class_Wide
                        : (OverrideTableGeneration ? "halfer" : "half")
                    );

                    // Add Shared Icon (if any)
                    if (string.IsNullOrWhiteSpace(SharedIcon) == false && IsCloaked == false)
                    {
                        build.Append(SharedIcon);
                    }

                    build.Append(element.ToString());

                    build.Append("</div>");

                    //  If set all table cell/row creation will be ignored
                    if (!OverrideTableGeneration)
                    {
                        build.Append("</td>");

                        if (Expression == OutputExpression.FullWidth || Expression == OutputExpression.Right)
                        {
                            build.Append("</tr>");
                        }
                    }
                    #endregion Wrapper
                }
            }
            else
            {
                string candidate = OutputText;
                RichTextLink.CreateLinkPreview(ref candidate);

                build.Append(GetSimpleTextElement(candidate));
            }

            build.ApiResponse.Fields.Add(new Api.MediakiwiField()
            {
                Event = Api.MediakiwiJSEvent.none,
                Title = MandatoryWrap(Title),
                Value = OutputText,
                Expression = OutputExpression.FullWidth,
                PropertyName = ID,
                PropertyType = (Property == null) ? typeof(string).FullName : Property.PropertyType.FullName,
                VueType = Api.MediakiwiFormVueType.wimRichText,
                ReadOnly = IsReadOnly,
                ContentTypeID = ContentTypeSelection
            });

            return ReadCandidate(OutputText);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override bool IsValid(bool isRequired)
        {
            Mandatory = isRequired;
            if (Console.CurrentListInstance.wim.IsSaveMode)
            {
                //  Custom error validation
                if (!base.IsValid(isRequired))
                {
                    return false;
                }

                if (Mandatory && string.IsNullOrEmpty(OutputText))
                {
                    var hasValue = HasSharedValue();
                    if (hasValue.isSharedField)
                    {
                        return hasValue.hasValue;
                    }

                    return false;
                }
            }
            return true;
        }

    }
}
