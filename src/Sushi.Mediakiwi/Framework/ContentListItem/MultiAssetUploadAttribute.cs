using Sushi.Mediakiwi.Data;
using System;
using System.Collections.Generic;

namespace Sushi.Mediakiwi.Framework.ContentListItem
{
    /// <summary>
    /// Possible return types: Array of ints being the PK of the asset uploaded
    /// </summary>
    public class MultiAssetUploadAttribute : ContentSharedAttribute, IContentInfo, IListContentInfo
    {
        /// <summary>
        ///  Possible return types: Array of ints being the PK of the asset uploaded
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="galleryProperty">This should contain the guid of the gallery to upload to</param>
        /// <param name="mandatory">if set to <c>true</c> [mandatory].</param>
        public MultiAssetUploadAttribute(string title, string galleryProperty, bool mandatory)
            : this(title, mandatory, galleryProperty, null) { }

        /// <summary>
        ///  Possible return types: Array of ints being the PK of the asset uploaded
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="mandatory">if set to <c>true</c> [mandatory].</param>
        /// <param name="galleryProperty">This should contain the guid of the gallery to upload to</param>
        /// <param name="interactiveHelp">The interactive help.</param>
        public MultiAssetUploadAttribute(string title, bool mandatory, string galleryProperty, string interactiveHelp)
        {
            m_CanHaveExpression = true;
            ContentTypeSelection = ContentType.FileUpload;
            Title = title;
            Mandatory = mandatory;
            GalleryProperty = galleryProperty;
            InteractiveHelp = interactiveHelp;
            this.Expression = OutputExpression.FullWidth;
        }

        public string GalleryProperty { get; set; }

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
            m_Candidate = new int[0];
            if (Console.Form(this.ID) != null)
            {
                var items = new List<int>();
                var raw = Console.Form(this.ID);
                foreach (string item in raw.Split(','))
                {
                    var p = Utility.ConvertToInt(item);
                    if (p > 0)
                        items.Add(p);
                }

                m_Candidate = items.ToArray();
            }
            
            //  Possible return types: System.Web.HttpPostedFile

            Property.SetValue(SenderInstance, m_Candidate, null);
        }

        int[] m_Candidate;

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

            build.Append(string.Format(@"<tr> 
	<th><label for=""{1}"">{0}:</label></th>
	<td  colspan=""6"" class=""triple"">
        <div class=""multiBox""> 
        <a class=""multiAssetUpload_Button submit"">
        <i class=""icon-plus""></i>
        <span>Add files...</span> 
      
        </a> 
  <input galleryGUID=""{11}"" type=""file""   multiple accept=""{12}"">
      
    <ul id=""multiAssetUpload_resultFiles_{1}"" class=""multiAssetUpload_resultFiles"">
        <li>
            <span>Bogus test file.jpg</span>
            <span>40kb</span>
            <span  class=""remove""><a href=""#"">Remove</a></span>
   
  <li>
            <span>Bogus test 3 file.jpg</span>
            <span>40kb</span>
            <br/><progress max=""100""  value=""80""></progress>
    </ul>
    <br class=""clear"" />
    <footer>
        <a href=""#"" class=""submit clearButton""> <span>Clear all</span></a>
        <a href=""#"" class=""submit uploadButton""> <span>Upload all</span></a>
        <a href=""#"" class=""submit cancelButton""> <span>Cancel</span></a>
        <span class=""message""></span>
<input type=""hidden""  id=""{1}"" name=""{1}"" class=""resultCollection""></span>
<br class=""clear"" />
    <footer>
     </div>
	</td></tr>
"
                  , string.Concat(Title, Mandatory ? "<em>*</em>" : "") // 0
                  , this.ID // 1
                  , this.InteractiveHelp // 2
                  , IsValid(isRequired) ? string.Empty : " error" // 3
                  , ""// 4 geen custom errors
                  , ""// 5 kan niets links
                  , ""// 5 kan niets rechts van
                  , ""  //7, altijd voledige breedte pakken
                  , ""  //8, altijd voledige breedte pakken
                  , ""  //9, altijd voledige breedte pakken
                  ,  null // 10 geen autopostback
                  , GetProperty(Console.CurrentListInstance, GalleryProperty)// 11 
                  , Accept
                  )
              );


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

                if (Mandatory && (m_Candidate == null || m_Candidate.Length == 0))
                    return false;
            }
            return true;
        }

    }
}
