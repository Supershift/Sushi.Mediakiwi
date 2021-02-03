using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;
using Sushi.Mediakiwi.Data;
using Sushi.Mediakiwi.RichRext;

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
            Data.Utility.ReflectProperty(this, meta);
            meta.Name = name;
            meta.Default = defaultValue;
            meta.ContentTypeSelection = ((int)ContentTypeSelection).ToString();

            if (this.EnableTable)
                meta.AddOption(OPTION_ENABLE_TABLE);
            else
                meta.RemoveOption(OPTION_ENABLE_TABLE);

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
            this.SetMultiFieldTitleHTML(Labels.ResourceManager.GetString("input_richtext", new CultureInfo(Console.CurrentApplicationUser.LanguageCulture)), "icon-align-justify");

            //string _useNewRichTextCleaning = System.Configuration.ConfigurationManager.AppSettings["RICHTEXT_NEW_CLEAN"];

            //bool useNewRichTextCleaning =false;
            //if (!string.IsNullOrEmpty(_useNewRichTextCleaning) && _useNewRichTextCleaning == "1")
            //{
                //useNewRichTextCleaning = true;
            //}
            if (Property != null && Property.PropertyType == typeof(Data.CustomData))
                SetContentContainer(field);

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
                    if (Property.PropertyType == typeof(Data.CustomData))
                    {
                        candidate = m_ContentContainer[field.Property].Value;
                    }
                    else if (Property.PropertyType == typeof(string))
                    {
                        object value = Property.GetValue(SenderInstance, null);
                        if (value != null)
                            candidate = value.ToString();
                    }
                }
            }
            else
            {
                candidate = Console.Form(this.ID.Replace("$", ""));
                if (candidate == null) candidate = string.Empty;

                if (!this.EnableTable)
                {
                    candidate = _table.Replace(candidate, "");
                }
                //else
                //{
                //    var tableHandler = Data.Environment.GetInstance<IHtmlTableParser>();
                //}
            
                candidate = Utils.CleanLink(this.Console.CurrentListInstance.wim.CurrentSite, candidate);
            }

            if (!string.IsNullOrEmpty(candidate))
            {
                candidate = candidate.Replace("\t", string.Empty).Replace("\r\n", " ").Replace("\r", string.Empty).Replace("\n", string.Empty);

                //if candidate is empy or contains only html tags and no real content, set candidate to null
                if (string.IsNullOrEmpty(Data.Utility.CleanFormatting(candidate)))
                    candidate = null;
                else
                {
                    string emptyTest = candidate.Replace("&nbsp;", string.Empty).Trim();
                    if (string.IsNullOrEmpty(emptyTest))
                        candidate = null;
                    if (string.IsNullOrEmpty(Data.Utility.CleanFormatting(emptyTest)))
                        candidate = null;
                }
            }
            if (NoCleaning == false)
            {
                Cleaner cleaner = new Cleaner();
                candidate = cleaner.ApplyFullClean(candidate, false);


                //if (_richTextDataCleanHandler == null)
                //    _richTextDataCleanHandler = new RichTextDataCleaner();
                //candidate = _richTextDataCleanHandler.ParseData(this.Console.ItemType, this.Console.Item, this.ID, candidate);
            }
            if (!IsBluePrint && Property != null && Property.CanWrite)
            {
                if (Property.PropertyType == typeof(Data.CustomData))
                    ApplyContentContainer(field, candidate);
                else if (Property.PropertyType == typeof(string))
                    Property.SetValue(SenderInstance, candidate, null);
            }

            OutputText = candidate;

            //  Inherited content section
            if (ShowInheritedData)
            {
                //field.InheritedValue = field.Value;

                if (field != null && !string.IsNullOrEmpty(field.InheritedValue))
                {
                    string candidate2 = field.InheritedValue;
                    RichTextLink.CreateLinkPreview(ref candidate2);
                    InhertitedOutputText = candidate2;
                }
            }

        }
        static Regex striketroughFix = new Regex(@"<span\sstyle=""text-decoration:line-through;"">(.*?)</span>", RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);
        static Regex underlineFix = new Regex(@"<span\sstyle=""text-decoration:underline;"">(.*?)</span>", RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);
        static Regex tofillFails = new Regex(@"<a\shref=""TOFILL"">(.*?)</a>",  RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);
        private string PreCleanHTML(string candidate)
        {
            if (!String.IsNullOrEmpty(candidate))
            {
                // Underline            
                candidate = underlineFix.Replace(candidate, "<u>$1</u>");

                // Stiketrough
                candidate = striketroughFix.Replace(candidate, "<s>$1</s>");

                // To fill fails; if this part of the codes detects links with href="TOFILL" it means the editor fucked up
                // First Log, then remove
                if (tofillFails.IsMatch(candidate)) Sushi.Mediakiwi.Data.Notification.InsertOne("RichTextBoxEditor.Save", "Failed to create html link");
                candidate = tofillFails.Replace(candidate, "$1");
            }
            return candidate;
        }

        string ExtendParagraphWrap(string input)
        {
            if (string.IsNullOrEmpty(input)) return null;
            if (!input.StartsWith("<p>", StringComparison.OrdinalIgnoreCase))
                input = string.Concat("<p>", input, "</p>");
            return input;
        }

        //CuteEditor.Editor m_Editor;
        ///// <summary>
        ///// Gets the editor.
        ///// </summary>
        ///// <value>The editor.</value>
        //internal CuteEditor.Editor Editor
        //{
        //    get
        //    {
        //        if (m_Editor == null) SetEditor();
        //        return m_Editor;
        //    }
        //}
        Editor m_NewEditor;
        internal Editor NewEditor
        {
            get
            {
                if (m_NewEditor == null) SetNewEditor();
                return m_NewEditor;
            }
        }
        void SetNewEditor()
        {

            m_NewEditor = new Editor()
            {
                //BoldButton = true,
                //ItalicButton = true,
                //UnderlineButton = true,
                //LinkButton = true,
                //ListButton = true,
                //SubscriptButton = true,
                //SuperscriptButton = true,
                //StrikeThroughButton = true,
                //IndentButton = true,
                //HtmlButton = true,
                //HRButton = true,
                //RemoveformatButton = true,
                //ImageButton = CommonConfiguration.USE_RICHTEXT_IMAGEADD,
                //TableButton = CommonConfiguration.USE_RICHTEXT_TABLE,
                //AddTableRowButton = CommonConfiguration.USE_RICHTEXT_TABLE,
                //StyleButton = CommonConfiguration.USE_RICHTEXT_STYLE,
                //ShowToolTips = false,
                ID = "ta_" + ID
            };
            //if (ShowInheritedData)
            //{
            //    if (Console.CurrentListInstance.wim.ShowInFullWidthMode)
            //        m_NewEditor.Width = 450;
            //    else
            //        m_NewEditor.Width = 416;
            //}
            //else
            //{
            //    if (Console.CurrentListInstance.wim.ShowInFullWidthMode)
            //        m_NewEditor.Width = 776;
            //    else
            //        m_NewEditor.Width = 538;
            //}
            //m_NewEditor.Height = 50;
            m_NewEditor.ID = this.ID.Replace("$", string.Empty);
            string item = this.OutputText;

            //replace all strong tags with b tags, because of firefox bug. 
            if (item != null)
            {
                var buttonCleaner = new CleanupRichTextButtons();
                item = buttonCleaner.ApplyBold(item);
            }

            if (!string.IsNullOrEmpty(item) && item.StartsWith("<p>", StringComparison.OrdinalIgnoreCase))
            {
                m_NewEditor.Content = Data.Utility.CleanParagraphWrap(item);
            }
            else
                m_NewEditor.Content = item;

            //if (!this.m_IsNewDesign)
            //    Wim.RichtextEditor.RichtextEditorManager.RegisterInstance(m_NewEditor);
        }

        public class Editor
        {

            public string EditorHTML(bool isEnabled, bool hasTable = false, bool isCloaked = false)
            {
                if (isCloaked)
                {
                    return String.Format(@"<textarea class=""long {2}"" id=""{0}"" name=""{0}""{3}>{1}</textarea>",
                       ID,
                       CleanUpBadChars(Content),
                       "hidden",
                       isEnabled ? null : " disabled=\"disabled\"");
                }
                return String.Format(@"<textarea class=""long {2}"" id=""{0}"" name=""{0}""{3}>{1}</textarea>",
                    ID, 
                    CleanUpBadChars(Content), 
                    hasTable ? "table_rte" : "rte", 
                    isEnabled ? null : " disabled=\"disabled\"");

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
            this.SetWriteEnvironment();
            this.IsCloaked = isCloaked;
            this.Mandatory = isRequired;
            if (OverrideEditMode) isEditMode = false;
            if (isEditMode && this.IsEnabled())
            {
                #region Element creation
                StringBuilder element = new StringBuilder();
                element.AppendFormat(NewEditor.EditorHTML(this.IsEnabled(), this.EnableTable, this.IsCloaked));
                #endregion Element creation

                if (IsCloaked)
                {
                    build.AppendCloaked(element.ToString());
                }
                else
                {
                    #region Wrapper

            

                    string titleTag = string.Concat(Title, Mandatory ? "<em>*</em>" : "");
                    //  If set all table cell/row creation will be ignored
                    if (!OverrideTableGeneration)
                    {
                        if (ShowInheritedData)
                        {
                            this.ApplyTranslation(build);
                        }
                        else
                        {
                            if (Console.ExpressionPrevious == OutputExpression.Left)
                                build.Append("\t\t\t\t\t\t\t<th><label>&nbsp;</label></th><td>&nbsp;</td></tr>");

                            if (Expression == OutputExpression.FullWidth || Expression == OutputExpression.Left)
                                build.Append("\t\t\t\t\t\t<tr>");
                        }

                        build.AppendFormat("\n\t\t\t\t\t\t\t<th><label for=\"{0}\">{1}</label></th>", this.ID, this.TitleLabel);

                        //if (ShowInheritedData)
                        //    build.AppendFormat("\t\t\t\t\t\t\t<th class=\"local\"><label>{0}:</label></th>\t\t\t\t\t\t</tr>\t\t\t\t\t\t<tr>\t\t\t\t\t\t\t<td><div class=\"description\">{1}</div></td>\n", this.ID, this.TitleLabel);

                        build.AppendFormat("\n\t\t\t\t\t\t\t<td{0}{1}>{2}"
                            , (Expression == OutputExpression.FullWidth && Console.HasDoubleCols) ? " colspan=\"3\"" : null
                            , this.InputCellClassName(this.IsValid(isRequired))
                            , CustomErrorText
                            );
                    }

                    build.AppendFormat("\n\t\t\t\t\t\t\t\t<div class=\"{0}\">", (Expression == OutputExpression.FullWidth) ? this.Class_Wide
                        : (OverrideTableGeneration ? "halfer" : "half")
                    );

                    build.Append(element.ToString());

                    build.Append("\n\t\t\t\t\t\t\t\t</div>");

                    //  If set all table cell/row creation will be ignored
                    if (!OverrideTableGeneration)
                    {
                        build.Append("\n\t\t\t\t\t\t\t</td>");

                        if (Expression == OutputExpression.FullWidth || Expression == OutputExpression.Right)
                            build.Append("\n\t\t\t\t\t\t</tr>\n");
                    }
                    #endregion Wrapper
                }
            }
            else
            {
                string candidate = OutputText;
                RichTextLink.CreateLinkPreview(ref candidate);

                build.Append(GetSimpleTextElement(this.Title, this.Mandatory, candidate, this.InteractiveHelp));
            }

            build.ApiResponse.Fields.Add(new Api.MediakiwiField()
            {
                Event = Api.MediakiwiJSEvent.none,
                Title = MandatoryWrap(this.Title),
                Value = this.OutputText,
                Expression = OutputExpression.FullWidth,
                PropertyName = this.ID,
                PropertyType = (Property == null) ? typeof(string).FullName : Property.PropertyType.FullName,
                VueType = Api.MediakiwiFormVueType.wimRichText,
                ReadOnly = this.IsReadOnly
            });

            return ReadCandidate(this.OutputText);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override bool IsValid(bool isRequired)
        {
            this.Mandatory = isRequired;
            if (Console.CurrentListInstance.wim.IsSaveMode)
            {
                //  Custom error validation
                if (!base.IsValid(isRequired))
                    return false;

                if (Mandatory && string.IsNullOrEmpty(OutputText))
                    return false;

                //if (this.MaxValueLength > 0 && !string.IsNullOrEmpty(OutputText) && OutputText.Length > MaxValueLength)
                //    return false;


            }
            return true;
        }

    }
}
