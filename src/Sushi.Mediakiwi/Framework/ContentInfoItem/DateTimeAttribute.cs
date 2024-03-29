using Sushi.Mediakiwi.Data;
using System;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Framework.ContentInfoItem
{
    /// <summary>
    /// Possible return types: System.DateTime
    /// </summary>
    public class DateTimeAttribute : ContentSharedAttribute, IContentInfo
    {
        public async Task<Api.MediakiwiField> GetApiFieldAsync()
        {
            return new Api.MediakiwiField()
            {
                Event = AutoPostBack ? Api.MediakiwiJSEvent.Change : Api.MediakiwiJSEvent.None,
                Title = MandatoryWrap(Title),
                Value = OutputText,
                Expression = Expression,
                PropertyName = ID,
                PropertyType = (Property == null) ? typeof(DateTime).FullName : Property.PropertyType.FullName,
                VueType = Api.MediakiwiFormVueType.wimDateTime,
                ReadOnly = IsReadOnly,
                ContentTypeID = ContentTypeSelection,
                IsAutoPostback = AutoPostBack,
                IsMandatory = Mandatory,
                MaxLength = MaxValueLength,
                HelpText = InteractiveHelp,
                FormSection = GetFormMapClass()
            };
        }
        /// <summary>
        /// Possible return types: System.DateTime
        /// </summary>
        /// <param name="title"></param>
        public DateTimeAttribute(string title)
            : this(title, false) { }

        /// <summary>
        /// Possible return types: System.DateTime
        /// </summary>
        /// <param name="title"></param>
        /// <param name="mandatory"></param>
        public DateTimeAttribute(string title, bool mandatory)
            : this(title, mandatory, false) { }

        /// <summary>
        /// Possible return types: System.DateTime
        /// </summary>
        /// <param name="title"></param>
        /// <param name="mandatory"></param>
        /// <param name="isDbSortField"></param>
        public DateTimeAttribute(string title, bool mandatory, bool isDbSortField)
            : this(title, mandatory, isDbSortField, null) { }

        /// <summary>
        /// Possible return types: System.DateTime
        /// </summary>
        /// <param name="title"></param>
        /// <param name="mandatory"></param>
        /// <param name="interactiveHelp"></param>
        public DateTimeAttribute(string title, bool mandatory, string interactiveHelp)
            : this(title, mandatory, false, interactiveHelp) { }

        /// <summary>
        /// Possible return types: System.DateTime
        /// </summary>
        /// <param name="title"></param>
        /// <param name="mandatory"></param>
        /// <param name="isDbSortField"></param>
        /// <param name="interactiveHelp"></param>
        public DateTimeAttribute(string title, bool mandatory, bool isDbSortField, string interactiveHelp)
        {
            m_CanHaveExpression = true;
            ContentTypeSelection = ContentType.DateTime;
            Title = title;
            Mandatory = mandatory;
            IsDbSortField = isDbSortField;
            InteractiveHelp = interactiveHelp;
        }

        private bool m_IsDbSortField;
        /// <summary>
        /// 
        /// </summary>
        public bool IsDbSortField
        {
            set { m_IsDbSortField = value; }
            get { return m_IsDbSortField; }
        }

        /// <summary>
        /// Sets the candidate.
        /// </summary>
        public void SetCandidate(bool isEditMode)
        {
            SetCandidate(null, isEditMode);
        }

        public bool AutoPostBack
        {
            set { m_AutoPostBack = value; }
            get { return m_AutoPostBack; }
        }

        /// <summary>
        /// Sets the candidate.
        /// </summary>
        public void SetCandidate(Field field, bool isEditMode)
        {
            // Get formatting information for dates
            var dateInfo = Common.GetDateInformation();

            if (Property != null && Property.PropertyType == typeof(CustomData))
            {
                SetContentContainer(field);
            }

            m_Candidate = null;
            if (IsInitialLoad || !isEditMode)
            {
                if (IsBluePrint)
                {
                    if (field != null && !string.IsNullOrEmpty(field.Value))
                    {
                        if (field.Value.Contains("-"))
                        {
                            //  Previous WIM versions
                            DateTime tmp;
                            if (DateTime.TryParseExact(field.Value, dateInfo.DateTimeFormatShort, dateInfo.Culture, DateTimeStyles.None, out tmp))
                            {
                                m_Candidate = tmp;
                            }
                        }
                        else
                        {
                            m_Candidate = new DateTime(long.Parse(field.Value));
                        }
                    }
                }
                else
                {

                    if (Property.PropertyType == typeof(CustomData))
                    {
                        m_Candidate = m_ContentContainer[field.Property].ParseDateTime();
                    }
                    else if (Property.PropertyType == typeof(DateTime))
                    {
                        DateTime value = (DateTime)Property.GetValue(SenderInstance, null);
                        if (value != DateTime.MinValue)
                        {
                            m_Candidate = value;
                        }
                    }
                    else if (Property.PropertyType == typeof(DateTime?))
                    {
                        m_Candidate = (DateTime?)Property.GetValue(SenderInstance, null);
                    }
                }
            }
            else
            {

                if (!string.IsNullOrEmpty(Console.Form(ID)))
                {
                    try
                    {
                        string candidate = Console.Form(ID);
                        DateTime tmp;
                        if (DateTime.TryParseExact(candidate, dateInfo.DateFormatShort, dateInfo.Culture, DateTimeStyles.None, out tmp))
                        {
                            if (!string.IsNullOrEmpty(Console.Form(ID + "T")))
                            {
                                string[] timearr = Console.Form(ID + "T").ToString().Split(':');

                                m_Candidate = tmp.AddHours(Utility.ConvertToInt(timearr[0])).AddMinutes(Utility.ConvertToInt(timearr[1]));
                            }
                            else
                            {
                                m_Candidate = tmp;
                            }
                        }

                        // [MR:20-03-2019] Converts local timezone to UTC (database) time for saving
                        if (Console.CurrentList.Option_ConvertUTCToLocalTime && m_Candidate.HasValue && m_Candidate.Value.Kind != DateTimeKind.Utc)
                        {
                            m_Candidate = m_Candidate.Value.ToUniversalTime();
                        }
                    }
                    catch (Exception) { }
                }
            }

            if (!IsBluePrint && Property != null && Property.CanWrite)
            {
                if (Property.PropertyType == typeof(CustomData))
                {
                    ApplyContentContainer(field, (m_Candidate == null || m_Candidate == DateTime.MinValue) ? null : m_Candidate.Value.Ticks.ToString());
                }
                else
                {
                    if (m_Candidate.HasValue)
                    {
                        Property.SetValue(SenderInstance, m_Candidate, null);
                    }
                    else
                    {
                        Property.SetValue(SenderInstance, null, null);
                    }
                }
            }

            OutputText = null;
            if (m_Candidate.HasValue)
            {
                OutputText = m_Candidate.Value.ToString(dateInfo.DateTimeFormatShort, dateInfo.Culture);
            }

            //  Inherited content section
            if (ShowInheritedData && field != null && !string.IsNullOrEmpty(field.InheritedValue))
            {
                if (field.InheritedValue.Contains("-"))
                {
                    //  Previous WIM versions
                    DateTime tmp;
                    if (DateTime.TryParseExact(field.InheritedValue, dateInfo.DateTimeFormatShort, dateInfo.Culture, DateTimeStyles.None, out tmp))
                    {
                        InhertitedOutputText = tmp.ToString(dateInfo.DateTimeFormatShort, dateInfo.Culture);
                    }
                }
                else
                {
                    InhertitedOutputText = new DateTime(long.Parse(field.InheritedValue)).ToString(dateInfo.DateTimeFormatShort, dateInfo.Culture);
                }
            }
        }


        DateTime? m_Candidate;

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

            // Get formatting information for dates
            var dateInfo = Common.GetDateInformation();

            IsCloaked = isCloaked;
            Mandatory = isRequired;
            if (OverrideEditMode)
            {
                isEditMode = false;
            }

            bool isEnabled = IsEnabled();

            // [MR:03-06-2021] Apply shared field clickable icon.
            var sharedInfoApply = ApplySharedFieldInformation(isEnabled, OutputText);

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

                if (string.IsNullOrWhiteSpace(sharedInfoApply.outputValue) == false)
                {
                    OutputText = sharedInfoApply.outputValue;
                }
            }

            if (!string.IsNullOrWhiteSpace(InputPostText))
            {
                InputPostText = $"<label>{InputPostText}</label>";
            }

            if (isEditMode && isEnabled)
            {
                #region Element creation

                StringBuilder element = new StringBuilder();

                element.AppendFormat("<input class=\"date datepicker{3}\" name=\"{0}\" type=\"text\" id=\"{0}\" maxlength=\"10\" value=\"{1}\" placeholder=\"{2}\"/>"
                    , ID
                    , m_Candidate.HasValue ? m_Candidate.Value.ToString(dateInfo.DateFormatShort, dateInfo.Culture) : string.Empty
                    , dateInfo.DateFormatShort.ToLower()
                     , IsCloaked ? " hidden" : null
                    );

                element.AppendFormat("<input class=\"time timepicker{3}{4}{5}\" type=\"text\" name=\"{0}T\" maxlength=\"5\" placeholder=\"00:00\" value=\"{1}\" />{2}"
                   , ID
                   , m_Candidate.HasValue ? m_Candidate.Value.ToString("HH:mm") : "00:00"
                   , IsCloaked ? "" : InputPostText
                   , IsValid(isRequired) ? string.Empty : " error"
                   , AutoPostBack ? " postBack" : string.Empty // 4
                    , IsCloaked ? " hidden" : null
                   );

                #endregion Element creation

                if (IsCloaked)
                {
                    build.AppendCloaked(element.ToString());
                }
                else
                {
                    #region Wrapper

                    string titleTag = string.Concat(Title, Mandatory ? "<em>*</em>" : "");

                    if (ShowInheritedData)
                    {
                        ApplyTranslation(build);
                    }
                    else
                    {
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

                    build.Append($"<th><label for=\"{ID}\">{TitleLabel}</label></th>");

                    build.AppendFormat("<td{0}{1}>{2}"
                        , (Expression == OutputExpression.FullWidth && Console.HasDoubleCols) ? " colspan=\"3\"" : null
                        , InputCellClassName(IsValid(isRequired))
                        , CustomErrorText
                        );

                    build.Append($"<div class=\"{((Expression == OutputExpression.FullWidth) ? Class_Wide : "half")}\">");

                    build.Append(element.ToString());

                    build.Append("</div></td>");

                    if (Expression == OutputExpression.FullWidth || Expression == OutputExpression.Right)
                    {
                        build.Append("</tr>");
                    }

                    #endregion Wrapper
                }
            }
            else
            {
                build.Append(GetSimpleTextElement(OutputText));
            }

            // Get API field and add it to response
            var apiField = Task.Run(async () => await GetApiFieldAsync().ConfigureAwait(false)).Result;
            build.ApiResponse.Fields.Add(apiField);

            return ReadCandidate(m_Candidate.HasValue ? m_Candidate.Value.Ticks.ToString() : null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override bool IsValid(bool isRequired)
        {
            Mandatory = isRequired;
            if (Console?.CurrentListInstance?.wim?.IsSaveMode == true)
            {
                //  Custom error validation
                if (!base.IsValid(isRequired))
                {
                    return false;
                }

                if (Mandatory)
                {
                    var hasValue = HasSharedValue();
                    if (hasValue.isSharedField)
                    {
                        return hasValue.hasValue;
                    }

                    return m_Candidate.HasValue;
                }
            }
            return true;

        }

    }
}
