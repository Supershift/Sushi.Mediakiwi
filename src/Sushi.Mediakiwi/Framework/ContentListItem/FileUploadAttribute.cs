using Sushi.Mediakiwi.Data;
using System;

namespace Sushi.Mediakiwi.Framework.ContentListItem
{
    /// <summary>
    /// Possible return types: System.Web.HttpPostedFile
    /// </summary>
    public class FileUploadAttribute : ContentSharedAttribute, IContentInfo, IListContentInfo
    {
        /// <summary>
        ///  Possible return types: System.Web.HttpPostedFile
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="mandatory">if set to <c>true</c> [mandatory].</param>
        public FileUploadAttribute(string title, bool mandatory)
            : this(title, mandatory, null) { }

        public FileUploadAttribute(string title, bool mandatory, string accept)
            : this(title, mandatory, accept, null) { }

        /// <summary>
        ///  Possible return types: System.Web.HttpPostedFile
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="mandatory">if set to <c>true</c> [mandatory].</param>
        /// <param name="interactiveHelp">The interactive help.</param>
        public FileUploadAttribute(string title, bool mandatory, string accept, string interactiveHelp)
        {
            m_CanHaveExpression = true;
            ContentTypeSelection = ContentType.FileUpload;
            Title = title;
            Mandatory = mandatory;
            Accept = accept;
            InteractiveHelp = interactiveHelp;
        }

        public string Accept { get; set; }

        public bool AutoPostBack
        {
            set { m_AutoPostBack = value; }
            get { return m_AutoPostBack; }
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
        public void SetCandidate(Field field, bool isEditMode)
        {
            m_Candidate = null;
            if (Context.Request.HasFormContentType &&  Context.Request.Form.Files.Count > 0)
                m_Candidate = new FileUpload(Context.Request.Form.Files[this.ID]);

            //  Possible return types: System.Web.HttpPostedFile

            Property.SetValue(SenderInstance, m_Candidate, null);
        }

        FileUpload m_Candidate;

        /// <summary>
        /// Writes the candidate.
        /// </summary>
        /// <param name="build">The build.</param>
        /// <param name="isEditMode"></param>
        /// <returns></returns>
        public Field WriteCandidate(WimControlBuilder build, bool isEditMode, bool isRequired, bool isCloaked)
        {
            this.IsCloaked = isCloaked;
            this.Mandatory = isRequired;
            if (!isEditMode) return null;


            if (true)//this.Console.CurrentApplicationUser.ShowNewDesign2)
            {
                string accept = null;
                if (!string.IsNullOrWhiteSpace(this.Accept))
                    accept = $"accept=\"{Accept}\" ";
                build.Append(string.Format(@"{8}{9}{5}
	<th><label for=""{1}"">{0}:</label></th>
	<td{7}>{4}

			<fieldset class=""uploadFile"">
				<input name=""info_{1}"" id=""info_{1}"" value=""Browse for file"" class=""big uploadFile"" disabled=""disabled"">
				<div class=""fileUpload submit"">
					<span>Select file</span>
					<input type=""file"" name=""{1}"" id=""{1}"" {11}class=""upload{10}"">
				</div>
			</fieldset>{2}
	</td>{6}
"
                      , string.Concat(Title, Mandatory ? "<em>*</em>" : "") // 0
                      , this.ID // 1
                      , this.InteractiveHelp // 2
                      , IsValid(isRequired) ? string.Empty : " error" // 3
                      , CustomErrorText // 4
                      , (Expression == OutputExpression.FullWidth || Expression == OutputExpression.Left) ? "<tr>" : null // 5
                      , (Expression == OutputExpression.FullWidth || Expression == OutputExpression.Right) ? "</tr>" : null // 6
                      , (Expression == OutputExpression.FullWidth && Console.HasDoubleCols) ? " colspan=\"3\" class=\"triple\"" : null // 7
                      , ((Console.ExpressionPrevious == OutputExpression.Left && Expression == OutputExpression.FullWidth) || (Console.ExpressionPrevious == OutputExpression.Left && Expression == OutputExpression.Left)) ? "<th><label>&nbsp;</label></th><td>&nbsp;</td></tr>" : null // 8
                      , ((Console.ExpressionPrevious == OutputExpression.FullWidth && Expression == OutputExpression.Right) || (Console.ExpressionPrevious == OutputExpression.Right && Expression == OutputExpression.Right)) ? "<tr><th><label>&nbsp;</label></th><td>&nbsp;</td>" : null // 9
                      , AutoPostBack ? " autoupload" : null // 10
                      , accept //11
                      )
                  );//<br class=""clear"">
            }
            else
            {

                build.Append(string.Format(@"{8}{9}{5}
	<th><label for=""{1}"">{0}:</label></th>
	<td{7}>{4}
		<fieldset class=""uploadFile"">
			<input type=""file"" class=""text{3}"" name=""{1}"" id=""{1}""/>
		</fieldset>{2}
	</td>{6}
"
                    , string.Concat(Title, Mandatory ? "<em>*</em>" : "") // 0
                    , this.ID // 1
                    , this.InteractiveHelp // 2
                    , IsValid(isRequired) ? string.Empty : " error" // 3
                    , CustomErrorText // 4
                    , (Expression == OutputExpression.FullWidth || Expression == OutputExpression.Left) ? "<tr>" : null // 5
                    , (Expression == OutputExpression.FullWidth || Expression == OutputExpression.Right) ? "</tr>" : null // 6
                    , (Expression == OutputExpression.FullWidth && Console.HasDoubleCols) ? " colspan=\"3\" class=\"triple\"" : null // 7
                    , ((Console.ExpressionPrevious == OutputExpression.Left && Expression == OutputExpression.FullWidth) || (Console.ExpressionPrevious == OutputExpression.Left && Expression == OutputExpression.Left)) ? "<th><label>&nbsp;</label></th><td>&nbsp;</td></tr>" : null // 8
                    , ((Console.ExpressionPrevious == OutputExpression.FullWidth && Expression == OutputExpression.Right) || (Console.ExpressionPrevious == OutputExpression.Right && Expression == OutputExpression.Right)) ? "<tr><th><label>&nbsp;</label></th><td>&nbsp;</td>" : null // 9

                    )
                );
            }

            return null;
        }

        /// <summary>
        /// Gets a value indicating whether this instance is valid.
        /// </summary>
        /// <value><c>true</c> if this instance is valid; otherwise, <c>false</c>.</value>
        /// <returns></returns>
        public new bool IsValid(bool isRequired)
        {
                if (Console.CurrentListInstance.wim.IsSaveMode)
                {
                    //  Custom error validation
                    if (!base.IsValid(isRequired))
                        return false;

                    if (Mandatory && (m_Candidate == null || m_Candidate.File.Length == 0))
                        return false;
                }
                return true;
        }

    }
}
