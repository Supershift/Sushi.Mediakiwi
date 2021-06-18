using Sushi.Mediakiwi.Data;
using System;

namespace Sushi.Mediakiwi.Framework.ContentInfoItem
{
    /// <summary>
    /// Possible return types: System.String
    /// </summary>
    public class HtmlContainerAttribute : ContentSharedAttribute, IContentInfo 
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlContainerAttribute"/> class.
        /// </summary>
        public HtmlContainerAttribute()
        {
            ContentTypeSelection = ContentType.HtmlContainer;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlContainerAttribute"/> class.
        /// </summary>
        /// <param name="noPadding">if set to <c>true</c> [no padding].</param>
        public HtmlContainerAttribute(bool noPadding)
        {
            ContentTypeSelection = ContentType.HtmlContainer;
            HasNoPadding = noPadding;
        }

        /// <summary>
        /// Sets the candidate.
        /// </summary>
        /// <param name="isEditMode"></param>
        public void SetCandidate(bool isEditMode)
        {
        }

        /// <summary>
        /// Sets the candidate.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="isEditMode">if set to <c>true</c> [is edit mode].</param>
        public void SetCandidate(Field field, bool isEditMode)
        {
            OutputText = Property.GetValue(SenderInstance, null) as string;
        }

        public bool HasNoPadding { get; set; }

        /// <summary>
        /// Writes the candidate.
        /// </summary>
        /// <param name="build">The build.</param>
        /// <param name="isEditMode"></param>
        /// <returns></returns>
        public Field WriteCandidate(WimControlBuilder build, bool isEditMode, bool isRequired, bool isCloaked)
        {
            this.SetWriteEnvironment();
            this.IsCloaked = isCloaked;
            this.Mandatory = isRequired;
            if (string.IsNullOrEmpty(OutputText)) return null;
            if (string.IsNullOrEmpty(OutputText.Trim())) return null;

            if (HasNoPadding)
            {
                build.AppendFormat("\t\t\t\t\t\t\t<td colspan=\"{1}\"{2}>{0}</td>"
                            , OutputText
                            , Console.HasDoubleCols ? "4" : "2"
                            , HasNoPadding ? " class=\"clear\"" : ""
                            , Console.HasDoubleCols && Expression != OutputExpression.FullWidth ? "half" : "long"  // 3                    
                            );
            }
            else
            {
                build.AppendFormat("\t\t\t\t\t\t\t<td colspan=\"{1}\"{2}><div class=\"{3}\">{0}</div></td>"
                                , OutputText
                                , Console.HasDoubleCols ? "4" : "2"
                                , HasNoPadding ? " class=\"clear\"" : ""
                                , Console.HasDoubleCols && Expression != OutputExpression.FullWidth ? "half" : "long"  // 3                    
                                );
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
