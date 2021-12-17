using Sushi.Mediakiwi.Data;
using Sushi.Mediakiwi.Logic;
using System;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Framework.ContentInfoItem
{
    /// <summary>
    /// Possible return types: System.String
    /// </summary>
    public class TextLineAttribute : ContentSharedAttribute, IContentInfo
    {
        public async Task<Api.MediakiwiField> GetApiFieldAsync()
        {
            return new Api.MediakiwiField()
            {
                Title = Title,
                Value = OutputText,
                Expression = Expression,
                PropertyName = ID,
                PropertyType = (Property == null) ? typeof(string).FullName : Property.PropertyType.FullName,
                VueType = Api.MediakiwiFormVueType.wimTextline,
                ContentTypeID = ContentTypeSelection,
                IsAutoPostback = m_AutoPostBack,
                IsMandatory = Mandatory,
                MaxLength = MaxValueLength,
                HelpText = InteractiveHelp,
                FormSection = GetFormMapClass()
            };
        }

        /// <summary>
        /// Possible return types: System.String
        /// </summary>
        /// <param name="title"></param>
        public TextLineAttribute(string title)
            : this(title, null) { }

        /// <summary>
        /// Possible return types: System.String
        /// </summary>
        /// <param name="title"></param>
        /// <param name="interactiveHelp"></param>
        public TextLineAttribute(string title, string interactiveHelp)
            : this(title, false, null) { }

        /// <summary>
        /// Possible return types: System.String
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="IsClosedContainer">If this text is set as a container (title = null), it has the option to be open or closed</param>
        /// <param name="interactiveHelp">The interactive help.</param>
        public TextLineAttribute(string title, bool IsClosedContainer, string interactiveHelp)
        {
            m_CanHaveExpression = true;
            ContentTypeSelection = ContentType.TextLine;
            Title = title;
            InteractiveHelp = interactiveHelp;
            m_IsClosedContainer = IsClosedContainer;
        }

        internal bool m_IsClosedContainer;

        /// <summary>
        /// Sets the candidate.
        /// </summary>
        /// <param name="isEditMode"></param>
        public void SetCandidate(bool isEditMode)
        {
            SetCandidate(null, isEditMode);
        }

        /// <summary>
        /// Sets the candidate.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="isEditMode">if set to <c>true</c> [is edit mode].</param>
        public void SetCandidate(Field field, bool isEditMode)
        {
            if (Property != null && Property.PropertyType == typeof(CustomData))
            {
                SetContentContainer(field);
            }

            string candidate = null;

            if (IsBluePrint || Property == null)
            {
                if (field != null)
                    candidate = field.Value;
            }
            else
            {
                if (Property.PropertyType == typeof(CustomData))
                {
                    candidate = m_ContentContainer[field.Property].Value;
                    object ob = PropertyLogic.ConvertPropertyValue(field.Property, candidate, this.Console.CurrentList.ID, null);
                    Type type = typeof(string);
                    if (ob != null) type = ob.GetType();

                    if (candidate != null)
                    {
                        if (type == typeof(decimal))
                            candidate = Utility.ConvertToDecimalString(((decimal)ob));
                        else if (type == typeof(DateTime) || type == typeof(DateTime?))
                        {
                            DateTime tmp = ((DateTime)ob);

                            // [MR:20-03-2019] Converts UTC (database) time to local timezone for display
                            //if (Console.CurrentList.Option_ConvertUTCToLocalTime && tmp.Kind != DateTimeKind.Local)
                            //    tmp = AppCentre.Data.Supporting.LocalDateTime.GetDate(tmp, Console.CurrentListInstance.wim.CurrentSite, true);

                            if (tmp == DateTime.MinValue)
                                candidate = null;
                            else if (tmp.Hour == 0 && tmp.Minute == 0 && tmp.Second == 0 && tmp.Millisecond == 0)
                                candidate = tmp.ToString(Console.DateFormat);
                            else
                                candidate = tmp.ToString(string.Concat(Console.DateFormat, " HH:mm"));
                        }
                    }

                }
                else 
                {
                    object value = Property.GetValue(SenderInstance, null);
                    if (value != null)
                    {
                        if (this.Property.PropertyType == typeof(decimal))
                            candidate = Utility.ConvertToDecimalString(((decimal)value));
                        else if (this.Property.PropertyType == typeof(DateTime) || this.Property.PropertyType == typeof(DateTime?))
                        {
                            DateTime tmp = ((DateTime)value);

                            // [MR:20-03-2019] Converts UTC (database) time to local timezone for display
                            //if (Console.CurrentList.Option_ConvertUTCToLocalTime && tmp.Kind != DateTimeKind.Local)
                            //    tmp = AppCentre.Data.Supporting.LocalDateTime.GetDate(tmp, Console.CurrentListInstance.wim.CurrentSite, true);

                            if (tmp == DateTime.MinValue)
                                candidate = null;
                            else if (tmp.Hour == 0 && tmp.Minute == 0 && tmp.Second == 0 && tmp.Millisecond == 0)
                                candidate = tmp.ToString(Console.DateFormat);
                            else
                                candidate = tmp.ToString(string.Concat(Console.DateFormat, " HH:mm"));
                        }
                        else
                            candidate = value.ToString();
                    }
                }
            }

            OutputText = candidate;



            if (this.Title != null && this.Title.StartsWith(">internal_error|"))
            {
                string[] split = this.Title.Split('|');
                if (split.Length == 2)
                {
                    OutputText = split[1];
                    Title = "Error";
                }
            }

            //  Inherited content section
            if (ShowInheritedData)
            {
                if (field != null && !string.IsNullOrEmpty(field.InheritedValue))
                {
                    InhertitedOutputText = field.InheritedValue;
                }
            }
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
            string formName = GetFormMapClass();

            IsCloaked = isCloaked;
            Mandatory = isRequired;

            if (!IsCloaked)
            {
                // Get API field and add it to response
                var apiField = Task.Run(async () => await GetApiFieldAsync()).Result;
                build.ApiResponse.Fields.Add(apiField);

                build.Append(GetSimpleTextElement(OutputText));
            }
            return null;
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
