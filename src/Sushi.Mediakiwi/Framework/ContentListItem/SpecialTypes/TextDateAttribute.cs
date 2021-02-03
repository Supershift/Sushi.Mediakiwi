using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Sushi.Mediakiwi.Framework.ContentListItem.SpecialTypes
{
    /// <summary>
    /// Possible return types: System.DateTime
    /// </summary>
    public class TextDateAttribute : ContentSharedAttribute, IContentInfo, IListContentInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TextDateAttribute"/> class.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="maxlength">The maxlength.</param>
        /// <param name="mandatory">if set to <c>true</c> [mandatory].</param>
        /// <param name="interactiveHelp">The interactive help.</param>
        /// <param name="mustMatchRegex">The must match regex.</param>
        public TextDateAttribute(string title, int maxlength, bool mandatory, string interactiveHelp, string mustMatchRegex)
            : this(title, maxlength, mandatory, false, interactiveHelp, mustMatchRegex) { }

        /// <summary>
        /// Possible return types: Sushi.Mediakiwi.Data.SpecialTypes.TextDate
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="maxlength">The maxlength.</param>
        /// <param name="mandatory">if set to <c>true</c> [mandatory].</param>
        /// <param name="hasEnableOption">if set to <c>true</c> [has enable option].</param>
        /// <param name="interactiveHelp">The interactive help.</param>
        /// <param name="mustMatchRegex">The must match regex.</param>
        public TextDateAttribute(string title, int maxlength, bool mandatory, bool hasEnableOption, string interactiveHelp, string mustMatchRegex)
        {
            ContentTypeSelection = ContentType.TextDate;
            Title = title;
            Mandatory = mandatory;
            HasEnableOption = hasEnableOption;
            MaxValueLength = maxlength;
            InteractiveHelp = interactiveHelp;
            if (mustMatchRegex != null)
                MustMatch = new Regex(mustMatchRegex);
        }

        internal bool HasEnableOption;

        private Regex m_MustMatch;
        /// <summary>
        /// 
        /// </summary>
        public Regex MustMatch
        {
            set { m_MustMatch = value; }
            get { return m_MustMatch; }
        }

        /// <summary>
        /// Sets the candidate.
        /// </summary>
        /// <param name="isEditMode">if set to <c>true</c> [is edit mode].</param>
        public void SetCandidate(bool isEditMode)
        {
            SetCandidate(null, isEditMode);
        }

        /// <summary>
        /// Sets the candidate.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="isEditMode">if set to <c>true</c> [is edit mode].</param>
        public void SetCandidate(Data.Content.Field field, bool isEditMode)
        {
            if (IsInitialLoad || !isEditMode)
            {
                m_Candidate = Property.GetValue(SenderInstance, null) as Data.SpecialTypes.TextDate;
            }
            else
            {
                if (m_Candidate == null)
                    m_Candidate = new Sushi.Mediakiwi.Data.SpecialTypes.TextDate();

                DateTime dateCandidate = DateTime.MinValue;

                if (HasEnableOption)
                    m_Candidate.Disabled = Data.Utility.ConvertToInt(Context.Request.Form[this.ID + "_E"]) != 1;

                if (!m_Candidate.Disabled)
                {
                    m_Candidate.Text = Context.Request.Form[this.ID];

                    if (!string.IsNullOrEmpty(Context.Request.Form[this.ID + "_2"]) && !string.IsNullOrEmpty(Context.Request.Form[this.ID + "_1"]) && !string.IsNullOrEmpty(Context.Request.Form[this.ID + "_0"]))
                    {
                        try
                        {
                            dateCandidate = new DateTime(
                                Data.Utility.ConvertToInt(Context.Request.Form[this.ID + "_2"]),
                                Data.Utility.ConvertToInt(Context.Request.Form[this.ID + "_1"]),
                                Data.Utility.ConvertToInt(Context.Request.Form[this.ID + "_0"]),
                                0, 0, 0);
                        }
                        catch (Exception) { }
                    }


                    m_Candidate.Date = dateCandidate;
                }
            }

            if (m_Candidate == null)
                m_Candidate = new Sushi.Mediakiwi.Data.SpecialTypes.TextDate();

            Property.SetValue(SenderInstance, m_Candidate, null);

            OutputText = null;
            if (m_Candidate != null)
            {
                OutputText = string.Concat(OutputText, m_Candidate.Text);
                
                if (m_Candidate.Date != DateTime.MinValue)
                    OutputText = string.Concat(OutputText, string.IsNullOrEmpty(OutputText) ? "" : " ", "<b>(", m_Candidate.Date.ToString(Console.DateFormat), ")</b>");
            }
        }

        Data.SpecialTypes.TextDate m_Candidate;

        /// <summary>
        /// Writes the candidate.
        /// </summary>
        /// <param name="build">The build.</param>
        /// <param name="isEditMode"></param>
        /// <returns></returns>
        public Data.Content.Field WriteCandidate(WimControlBuilder build, bool isEditMode, bool isRequired, bool isCloaked)
        {
            this.IsCloaked = isCloaked;
            this.Mandatory = isRequired;
            if (isEditMode)
            {
                

                build.Append(string.Format(@"
<tr>
	<th><label for=""{1}"">{0}:</label></th>
	<td>{11}
		<fieldset class=""dateTime"">
			{12}<input type=""text"" class=""text {13}{7}"" id=""{1}"" name=""{1}""{10} value=""{9}"" />
			<input type=""text"" class=""text day{7}"" name=""{1}_0"" id=""{1}_0"" value=""{2}"" /> -
			<input type=""text"" class=""text month{7}"" name=""{1}_1"" id=""{1}_1"" value=""{3}"" /> -
			<input type=""text"" class=""text year{7}"" name=""{1}_2"" id=""{1}_2"" value=""{4}"" />
			<button class=""datePicker srcMouseHover""><img alt=""Pick"" src=""{6}/images/icon_dateTime_link.png""/></button>
			<div class=""hideThisNode dateCalendar""><input type=""hidden"" value=""{8}?xml=datetime""/></div>
			(dd-mm-yyyy)
		</fieldset>{5}
	</td>
</tr>
"
                   , string.Concat(Title, Mandatory ? "<em>*</em>" : "")
                   , this.ID
                   , m_Candidate.Date != DateTime.MinValue ? m_Candidate.Date.Day.ToString("00") : string.Empty
                   , m_Candidate.Date != DateTime.MinValue ? m_Candidate.Date.Month.ToString("00") : string.Empty
                   , m_Candidate.Date != DateTime.MinValue ? m_Candidate.Date.Year.ToString("00") : string.Empty
                   , this.InteractiveHelp
                   , this.Console.WimRepository
                   , this.IsValid(isRequired) ? string.Empty : " error"
                   , Console.WimPagePath
                   , m_Candidate.Text
                   , this.MaxValueLength > 0 ? string.Concat(" maxlength=\"", this.MaxValueLength, "\"") : string.Empty
                   , this.CustomErrorText
                   , HasEnableOption ? string.Format(" <input type=\"checkbox\" class=\"checked enabler\" name=\"{0}_E\" id=\"{0}_E\" value=\"1\"{1} />\n", this.ID, m_Candidate.Disabled ? null : " checked=\"checked\"") : null
                   , HasEnableOption ? "checkedlabel" : "label"
                   )
               );
            }
            else
            {
                build.Append(GetSimpleTextElement(this.Title, this.Mandatory, OutputText, this.InteractiveHelp));
            }
            return ReadCandidate(null);
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

                if (Mandatory)
                {
                    if (m_Candidate.Date == DateTime.MinValue || string.IsNullOrEmpty(m_Candidate.Text))
                        return false;
                }
            }
            return true;
        }


    }
}
