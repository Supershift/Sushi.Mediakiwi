using Sushi.Mediakiwi.Data;
using static Sushi.Mediakiwi.Beta.GeneratedCms.Source.Component;

namespace Sushi.Mediakiwi.Framework
{
    /// <summary>
    /// 
    /// </summary>
    public interface IContentInfo
    {
        ListInfoItem InfoItem { get; set; }
        /// <summary>
        /// 
        /// </summary>
        string ID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        string OutputText { get; set; }
        /// <summary>
        /// 
        /// </summary>
        string FieldName { 
            get; 
            set; 
        }

        bool HasSenderInstance { get; }
        /// <summary>
        /// 
        /// </summary>
        object SenderInstance { get; set; }
        object SenderSponsorInstance { get; set; }
        /// <summary>
        /// 
        /// </summary>
        System.Reflection.PropertyInfo Property { get; set; }
        /// <summary>
        /// 
        /// </summary>
        ContentType ContentTypeSelection { get; set; }
        /// <summary>
        /// 
        /// </summary>
        string Title { get; set; }
        /// <summary>
        /// 
        /// </summary>
        string InteractiveHelp { get; set; }
        /// <summary>
        /// 
        /// </summary>
        bool Mandatory { get; set; }
        /// <summary>
        /// 
        /// </summary>
        int MaxValueLength { get; set; }

        bool IsSharedField { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        MetaData GetMetaData(string name, UI.ListItemCollection collectionPropertyValue);
        /// <summary>
        /// Sets the content container.
        /// </summary>
        /// <param name="listItems">The list items.</param>
        void ApplyMetaDataList(MetaDataList[] listItems);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        MetaData GetMetaData(string name);
        /// <summary>
        /// Gets the meta data.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
        MetaData GetMetaData(string name, string defaultValue);
        /// <summary>
        /// Applies the specified metadata.
        /// </summary>
        /// <param name="metadata">The metadata.</param>
        void Apply(MetaData metadata);
        /// <summary>
        /// 
        /// </summary>
        bool IsValid(bool isRequired);
        /// <summary>
        /// 
        /// </summary>
        bool ForceLoadEvent { set; }
        /// <summary>
        /// Sets the candidate.
        /// </summary>
        void SetCandidate(bool isEditMode);
        /// <summary>
        /// Sets the candidate.
        /// </summary>
        void SetCandidate(Field value, bool isEditMode);
        /// <summary>
        /// Writes the candidate.
        /// </summary>
        /// <param name="build">The build.</param>
        /// <param name="isEditMode">if set to <c>true</c> [is edit mode].</param>
        /// <param name="isRequired">if set to <c>true</c> [is required].</param>
        /// <returns></returns>
        Field WriteCandidate(WimControlBuilder build, bool isEditMode, bool isRequired, bool isCloaked);
        /// <summary>
        /// Reads the candidate.
        /// </summary>
        /// <returns></returns>
        Field ReadCandidate(object value);

        /// <summary>
        /// Gets or sets a value indicating whether [show inherited data].
        /// </summary>
        /// <value><c>true</c> if [show inherited data]; otherwise, <c>false</c>.</value>
        bool ShowInheritedData { get; set; }
        /// <summary>
        /// Gets or sets the inhertited output text.
        /// </summary>
        /// <value>The inhertited output text.</value>
        string InhertitedOutputText { get; set; }
        /// <summary>
        /// Gets or sets the expression.
        /// </summary>
        /// <value>The expression.</value>
        OutputExpression Expression { get; set; }
        /// <summary>
        /// Gets a value indicating whether this instance can have expression.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can have expression; otherwise, <c>false</c>.
        /// </value>
        bool CanHaveExpression { get; }
        bool OverrideTableGeneration { set; get; }
        string GetMultiFieldTitleHTML(bool isEditMode);
        bool IsHidden { get; set; }
        bool IsReadOnly { get; set; }
        bool IsCloaked { get; set; }

        void Chain(string id);
        event ContentInfoEventHandler OnChange;

        void Init(WimComponentListRoot wim);
    }
}
