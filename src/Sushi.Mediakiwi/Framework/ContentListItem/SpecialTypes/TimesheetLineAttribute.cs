using System;
using System.Collections.Generic;
using System.Text;

namespace Sushi.Mediakiwi.Framework.ContentListItem.SpecialTypes
{
    /// <summary>
    /// Possible return types: Sushi.Mediakiwi.Data.SubList
    /// </summary>
    public class TimesheetLineAttribute : ContentSharedAttribute, IContentInfo, IListContentInfo
    {
        /// <summary>
        /// Possible return types: Sushi.Mediakiwi.Data.SpecialTypes.TimesheetEntry[]
        /// </summary>
        /// <param name="componentlistGuid">The componentlist GUID.</param>
        public TimesheetLineAttribute(string componentlistGuid)
        {
            ContentTypeSelection = ContentType.TimeSheetLine;
            Mandatory = false;
            Componentlist = componentlistGuid;
        }

        private string m_Componentlist;
        /// <summary>
        /// Gets or sets the componentlist.
        /// </summary>
        /// <value>The componentlist.</value>
        public string Componentlist
        {
            set { m_Componentlist = value; }
            get { return m_Componentlist; }
        }

        private bool m_CanOnlyOrderSort;
        /// <summary>
        /// Gets or sets the can only order sort.
        /// </summary>
        /// <value>The can only order sort.</value>
        public bool CanOnlyOrderSort
        {
            set { m_CanOnlyOrderSort = value; }
            get { return m_CanOnlyOrderSort; }
        }

        private bool m_CanContainOneItem;
        /// <summary>
        /// Gets or sets a value indicating whether this instance can contain one item.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can contain one item; otherwise, <c>false</c>.
        /// </value>
        public bool CanContainOneItem
        {
            set { m_CanContainOneItem = value; }
            get { return m_CanContainOneItem; }
        }

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
        public void SetCandidate(Data.Content.Field field, bool isEditMode)
        {
            m_CandidateList = new List<Sushi.Mediakiwi.Data.SpecialTypes.TimesheetEntry>();

            if (IsInitialLoad || !isEditMode)
            {
                Sushi.Mediakiwi.Data.SpecialTypes.TimesheetEntry[] candidates = Property.GetValue(SenderInstance, null) as Data.SpecialTypes.TimesheetEntry[];
                if (candidates != null && candidates.Length > 0)
                {
                    foreach (Sushi.Mediakiwi.Data.SpecialTypes.TimesheetEntry tmp in candidates)
                        m_CandidateList.Add(tmp);
                }
                else
                {
                    m_CandidateList.Add(new Sushi.Mediakiwi.Data.SpecialTypes.TimesheetEntry());
                }
            }
            else
            {
                //  If one is found
                string candidate = Context.Request.Form[this.ID];
                if (!string.IsNullOrEmpty(candidate) && candidate.Contains("|"))
                {
                    Sushi.Mediakiwi.Data.SpecialTypes.TimesheetEntry entry = new Sushi.Mediakiwi.Data.SpecialTypes.TimesheetEntry();

                    entry.IDText = candidate.Split('|')[0];
                    entry.Description = candidate.Split('|')[1];
                    entry.Comment = Context.Request.Form[string.Concat(this.ID, "_C")].Trim();
                    entry.Day_Monday = Data.Utility.ConvertToDecimal(Context.Request.Form[string.Concat(this.ID, "_1")]);
                    entry.Day_Tuesday = Data.Utility.ConvertToDecimal(Context.Request.Form[string.Concat(this.ID, "_2")]);
                    entry.Day_Wednesday = Data.Utility.ConvertToDecimal(Context.Request.Form[string.Concat(this.ID, "_3")]);
                    entry.Day_Thursday = Data.Utility.ConvertToDecimal(Context.Request.Form[string.Concat(this.ID, "_4")]);
                    entry.Day_Friday = Data.Utility.ConvertToDecimal(Context.Request.Form[string.Concat(this.ID, "_5")]);
                    entry.Day_Saturday = Data.Utility.ConvertToDecimal(Context.Request.Form[string.Concat(this.ID, "_6")]);
                    entry.Day_Sunday = Data.Utility.ConvertToDecimal(Context.Request.Form[string.Concat(this.ID, "_7")]);

                    m_CandidateList.Add(entry);
                }

                System.Collections.Specialized.NameObjectCollectionBase.KeysCollection keyCollection = Context.Request.Form.Keys;
                bool foundCurrent = false;
                string previousID = string.Concat(this.ID + "_");

                //  If multiple are found
                foreach(string key in keyCollection)
                {
                    if (foundCurrent || key == this.ID)
                    {
                        if (foundCurrent)
                        {
                            if (key.StartsWith(previousID, StringComparison.OrdinalIgnoreCase))
                                continue;

                            if (!key.StartsWith(string.Concat(this.ID + "$"), StringComparison.OrdinalIgnoreCase))
                            {
                                //  Autorepeat (AJAX) fields never start with element. When element is found in the string it mean the AJAX sequence is over.
                                if (key.StartsWith("element", StringComparison.OrdinalIgnoreCase))
                                    break;
                            }

                            previousID = key;

                            string candidateAjax = Context.Request.Form[key];
                            if (!string.IsNullOrEmpty(candidateAjax) && candidateAjax.Contains("|"))
                            {
                                Sushi.Mediakiwi.Data.SpecialTypes.TimesheetEntry entry = new Sushi.Mediakiwi.Data.SpecialTypes.TimesheetEntry();

                                entry.IDText = candidateAjax.Split('|')[0];
                                entry.Description = candidateAjax.Split('|')[1];
                                entry.Comment = Context.Request.Form[string.Concat(key, "_C")].Trim();
                                entry.Day_Monday = Data.Utility.ConvertToDecimal(Context.Request.Form[string.Concat(key, "_1")]);
                                entry.Day_Tuesday = Data.Utility.ConvertToDecimal(Context.Request.Form[string.Concat(key, "_2")]);
                                entry.Day_Wednesday = Data.Utility.ConvertToDecimal(Context.Request.Form[string.Concat(key, "_3")]);
                                entry.Day_Thursday = Data.Utility.ConvertToDecimal(Context.Request.Form[string.Concat(key, "_4")]);
                                entry.Day_Friday = Data.Utility.ConvertToDecimal(Context.Request.Form[string.Concat(key, "_5")]);
                                entry.Day_Saturday = Data.Utility.ConvertToDecimal(Context.Request.Form[string.Concat(key, "_6")]);
                                entry.Day_Sunday = Data.Utility.ConvertToDecimal(Context.Request.Form[string.Concat(key, "_7")]);

                                m_CandidateList.Add(entry);
                            }

                        }
                        foundCurrent = true;
                    }
                }

            }
            Property.SetValue(SenderInstance, m_CandidateList.ToArray(), null);

            m_OutputTextList = new string[m_CandidateList.Count];
            for (int index = 0; index < m_CandidateList.Count; index++)
            {
                if (!string.IsNullOrEmpty(m_CandidateList[index].IDText))
                    m_OutputTextList[index] = string.Concat(m_CandidateList[index].IDText, "|", m_CandidateList[index].Description);
            }
        }

        string[] m_OutputTextList;
        List<Data.SpecialTypes.TimesheetEntry> m_CandidateList;
        

        /// <summary>
        /// Writes the candidate.
        /// </summary>
        /// <param name="build">The build.</param>
        /// <param name="isEditMode">if set to <c>true</c> [is edit mode].</param>
        /// <returns></returns>
        public Data.Content.Field WriteCandidate(WimControlBuilder build, bool isEditMode, bool isRequired, bool isCloaked)
        {
            this.Mandatory = isRequired;
            this.IsCloaked = isCloaked;
            int count = Data.Utility.ConvertToInt(this.ID.Replace("element", string.Empty));

            if (isEditMode)
            {
                Data.IComponentList list = Data.ComponentList.SelectOne(Data.Utility.ConvertToGuid(this.Componentlist));
                int recordCount = 0;

                if (m_CandidateList == null)
                    WriteInstance(this.ID, new Sushi.Mediakiwi.Data.SpecialTypes.TimesheetEntry(), build, null, null);
                else
                {
                    foreach (Sushi.Mediakiwi.Data.SpecialTypes.TimesheetEntry entry in m_CandidateList)
                    {
                        string row = "even";
                        if (recordCount % 2 == 1) row = "odd";

                        if (recordCount > 0)
                            WriteInstance(string.Concat(this.ID, "$", recordCount), entry, build, row, m_OutputTextList[recordCount]);
                        else
                            WriteInstance(this.ID, entry, build, row, m_OutputTextList[recordCount]);
                        recordCount++;
                    }
                }
            }
            else
            {
                int counter = 0;
                foreach (Sushi.Mediakiwi.Data.SpecialTypes.TimesheetEntry entry in m_CandidateList)
                {
                    if (!string.IsNullOrEmpty(entry.Comment))
                        entry.Comment = entry.Comment.Trim();

                    string row = "even";
                    if (counter % 2 == 1) row = "odd";

                    counter++;
                    build.Append(string.Format(@"
<tr class=""textOptionList {9}"" id=""_{10}${11}"">
	<td class=""project"">
		<input type=""hidden"" id=""{10}${11}"" />
        <fieldset>
			{0}
		</fieldset>
	</td>
	<td class=""day""><input type=""hidden"" id=""{10}${11}_1"" value=""{1}"" />{1}<br/>-</td>
	<td class=""day""><input type=""hidden"" id=""{10}${11}_2"" value=""{2}"" />{2}<br/>-</td>
	<td class=""day""><input type=""hidden"" id=""{10}${11}_3"" value=""{3}"" />{3}<br/>-</td>
	<td class=""day""><input type=""hidden"" id=""{10}${11}_4"" value=""{4}"" />{4}<br/>-</td>
	<td class=""day""><input type=""hidden"" id=""{10}${11}_5"" value=""{5}"" />{5}<br/>-</td>
	<td class=""day""><input type=""hidden"" id=""{10}${11}_6"" value=""{6}"" />{6}<br/>-</td>
	<td class=""day""><input type=""hidden"" id=""{10}${11}_7"" value=""{7}"" />{7}<br/>-</td>
	<td class=""total""><span>{8}</span><br/>-</td>
</tr>
"
                        , string.IsNullOrEmpty(entry.Comment) ? entry.Description : string.Format("{1}<br/><font color=\"red\">{0}</font>", entry.Comment, entry.Description) //0
                        , GetOutput(entry.Day_Monday) //1
                        , GetOutput(entry.Day_Tuesday) //2
                        , GetOutput(entry.Day_Wednesday) //3
                        , GetOutput(entry.Day_Thursday) //4
                        , GetOutput(entry.Day_Friday) //5
                        , GetOutput(entry.Day_Saturday) //6
                        , GetOutput(entry.Day_Sunday) //7
                        , GetOutput(entry.Day_Total, false) //8
                        , row //9
                        , this.ID //10
                        , counter //11
                        )
                    );
                }
            }

            //if (m_Candidate == null)
            //    return ReadCandidate(null);
            //return ReadCandidate(m_Candidate.GetSerialized());
            return ReadCandidate(null);
        }

        void WriteInstance(string id, SpecialTypes.TimesheetEntry entry, WimControlBuilder build, string row, string outputText)
        {
            string candidate = null;
            if (m_CandidateList != null && m_CandidateList.Count > 0)
                candidate = string.Concat("<li>", entry.Description, "</li>");

            build.Append(string.Format(@"
<tr class=""textOptionList {15}"" id=""_{0}"">
	<td class=""project"">{17}
		<input type=""hidden"" id=""{0}"" name=""{0}"" value=""{1}""/>
		<div class=""optionText"">
			<ul{6}>{4}</ul>
		</div>
		<ul class=""buttons"">

	        <li><button class=""srcMouseHover clear remove""><img alt="""" src=""{5}/images/icon_remove_link.png""/></button></li>
	        <li>
		        <button class=""srcMouseHover note toggleNextNode link""><img alt="""" src=""{5}/images/icon_page_link.png""/></button>
		        <div class=""noteBorder hideThisNode"">
			        <p>
				        <button class=""closeNote""><img alt=""Close"" src=""{5}/images/icon_closeok_link.png""/></button>
				        <label for=""{0}_C"">Additional information:</label><br/>
				        <textarea id=""{0}_C"" name=""{0}_C"">{18}</textarea>
			        </p>
		        </div>
	        </li>
	        <li><a href=""{2}?list={3}&openinframe=1&referid=_{0}"" class=""button openLayerPopUp id_popUpWithIframe srcMouseHover add""><img alt=""Insert"" src=""{5}/images/cmd_search_link.png""/></a></li>
		</ul>
	</td>
	<td class=""day""><input type=""text"" id=""{0}_1"" name=""{0}_1"" value=""{7}"" maxlength=""5"" class=""text hours{16}""/>-</td>
	<td class=""day""><input type=""text"" id=""{0}_2"" name=""{0}_2"" value=""{8}"" maxlength=""5"" class=""text hours{16}""/>-</td>
	<td class=""day""><input type=""text"" id=""{0}_3"" name=""{0}_3"" value=""{9}"" maxlength=""5"" class=""text hours{16}""/>-</td>
	<td class=""day""><input type=""text"" id=""{0}_4"" name=""{0}_4"" value=""{10}"" maxlength=""5"" class=""text hours{16}""/>-</td>
	<td class=""day""><input type=""text"" id=""{0}_5"" name=""{0}_5"" value=""{11}"" maxlength=""5"" class=""text hours{16}""/>-</td>
	<td class=""day""><input type=""text"" id=""{0}_6"" name=""{0}_6"" value=""{12}"" maxlength=""5"" class=""text hours{16}""/>-</td>
	<td class=""day""><input type=""text"" id=""{0}_7"" name=""{0}_7"" value=""{13}"" maxlength=""5"" class=""text hours{16}""/>-</td>
	<td class=""total""><b><span>{14}</span></b><br/>-</td>
</tr>
"
                , id //0
                , outputText //1
                , this.Console.WimPagePath //2
                , this.Componentlist //3
                , candidate //4
                , this.Console.WimRepository //5
                , this.IsValid(this.Mandatory) ? string.Empty : " class=\"error\"" //6
                , GetOutput(entry.Day_Monday) //7
                , GetOutput(entry.Day_Tuesday) //8
                , GetOutput(entry.Day_Wednesday) //9
                , GetOutput(entry.Day_Thursday) //10
                , GetOutput(entry.Day_Friday) //11
                , GetOutput(entry.Day_Saturday) //12
                , GetOutput(entry.Day_Sunday) //13
                , GetOutput(entry.Day_Total, false) //14
                , row //15
                , this.IsValid(Mandatory) ? null : " error" //16
                , CustomErrorText //17
                , entry.Comment //18
                )
            );
        }

        string GetOutput(decimal item)
        {
            return GetOutput(item, true);
        }

        string GetOutput(decimal item, bool dayValidation)
        {
            if (item == 0) return "";
            if (dayValidation && item > 24) return "";
            
            string candidate = item.ToString();
            candidate = candidate.Replace(",", ".");
            return string.Format("{0}", candidate);
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