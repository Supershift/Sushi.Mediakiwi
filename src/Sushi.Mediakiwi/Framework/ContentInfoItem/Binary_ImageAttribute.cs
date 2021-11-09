using Sushi.Mediakiwi.Data;
using System;
using System.Globalization;

namespace Sushi.Mediakiwi.Framework.ContentInfoItem
{
    /// <summary>
    /// Possible return types: System.Int32, System.Int32[nullable], System.String, Image
    /// </summary>
    public class Binary_ImageAttribute : ContentSharedAttribute, IContentInfo
    {
        /// <summary>
        /// Possible return types: System.Int32, System.Int32[nullable], System.String, Image
        /// </summary>
        /// <param name="title"></param>
        public Binary_ImageAttribute(string title)
            : this(title, false) { }

        /// <summary>
        /// Possible return types: System.Int32, System.Int32[nullable], System.String, Image
        /// </summary>
        /// <param name="title"></param>
        /// <param name="mandatory"></param>
        public Binary_ImageAttribute(string title, bool mandatory)
            : this(title, mandatory, null) { }


        /// <summary>
        /// Possible return types: System.Int32, System.Int32[nullable], System.String, Image
        /// </summary>
        /// <param name="title"></param>
        /// <param name="mandatory"></param>
        /// <param name="interactiveHelp"></param>
        public Binary_ImageAttribute(string title, bool mandatory, string interactiveHelp)
            : this(title, mandatory, null, interactiveHelp) { }

        /// <summary>
        /// Possible return types: System.Int32, System.Int32[nullable], System.String, Image
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="mandatory">if set to <c>true</c> [mandatory].</param>
        /// <param name="gallery">The indentifier GUID of the gallery or the path of the gallery; both are an option</param>
        /// <param name="interactiveHelp">The interactive help.</param>
        public Binary_ImageAttribute(string title, bool mandatory, string gallery, string interactiveHelp)
        {
            m_CanHaveExpression = true;
            ContentTypeSelection = ContentType.Binary_Image;
            Title = title;
            Mandatory = mandatory;
            InteractiveHelp = interactiveHelp;
            Collection = gallery;
        }

        /// <summary>
        /// Gets or sets the gallery assigning property.
        /// </summary>
        /// <value>
        /// The gallery property.
        /// </value>
        public string GalleryProperty { get; set; }
        internal string GalleryPropertyUrl { get; set; }
        /// <summary>
        /// Gets or sets the saved asset type ID.
        /// </summary>
        /// <value>
        /// The saved asset type ID.
        /// </value>
        public int SavedAssetTypeID { get; set; }

        bool _CanOnlyAdd = false;
        /// <summary>
        /// Gets or sets a value indicating whether this instance can only add.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance can only add; otherwise, <c>false</c>.
        /// </value>
        public bool CanOnlyAdd
        {
            get { return _CanOnlyAdd; }
            set { _CanOnlyAdd = value; }
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
        public void SetCandidate(Field field, bool isEditMode)
        {
            SetMultiFieldTitleHTML(Labels.ResourceManager.GetString("input_image", new CultureInfo(Console.CurrentApplicationUser.LanguageCulture)), "icon-photo");

            if (Property != null && Property.PropertyType == typeof(CustomData))
            {
                SetContentContainer(field);
            }

            Gallery gallery = null;
            if (!string.IsNullOrEmpty(GalleryProperty))
            {
                var galleryProperty = SenderInstance.GetType().GetProperty(GalleryProperty);
                var value = galleryProperty.GetValue(SenderInstance, null);

                if (value is Gallery)
                {
                    gallery = (Gallery)value;
                }
            }

            string galleryMap = gallery == null ? null : gallery.CompletePath;

            m_Candidate = new Image();
            if (IsInitialLoad || !isEditMode)
            {
                if (IsBluePrint)
                {
                    if (field != null)
                    {
                        int candidate = Utility.ConvertToInt(field.Value);

                        if (candidate > 0)
                        {
                            var asset = Asset.SelectOne(candidate);
                            m_Candidate = asset.ImageInstance;
                        }
                    }
                }
                else
                {
                    if (Property.PropertyType == typeof(CustomData))
                    {
                        int candidate = Utility.ConvertToInt(m_ContentContainer[field.Property].Value);
                        if (candidate > 0)
                        {
                            m_Candidate = Image.SelectOne(candidate);
                        }
                    }
                    else if (Property.PropertyType == typeof(Image))
                    {
                        m_Candidate = Property.GetValue(SenderInstance, null) as Image;
                    }
                    else if (Property.PropertyType == typeof(int?))
                    {
                        int? tmp = Property.GetValue(SenderInstance, null) as int?;
                        if (tmp.HasValue)
                        {
                            m_Candidate = Image.SelectOne(tmp.Value);
                        }
                    }
                    else
                    {
                        int candidate = Utility.ConvertToInt(Property.GetValue(SenderInstance, null));
                        if (candidate > 0)
                        {
                            m_Candidate = Image.SelectOne(candidate);
                        }
                    }
                }
            }
            else
            {
                var candidate = SelectedID;
                if (candidate.HasValue)
                {
                    m_Candidate = Image.SelectOne(candidate.Value);
                }

            }

            if (!IsBluePrint && Property != null && Property.CanWrite)
            {
                if (Property.PropertyType == typeof(CustomData))
                {
                    ApplyContentContainer(field, m_Candidate.ID.ToString());
                }
                else if (Property.PropertyType == typeof(int))
                {
                    Property.SetValue(SenderInstance, m_Candidate.ID, null);
                }
                else if (Property.PropertyType == typeof(int?))
                {
                    if (m_Candidate != null && m_Candidate.ID != 0)
                    {
                        Property.SetValue(SenderInstance, m_Candidate.ID, null);
                    }
                    else
                    {
                        Property.SetValue(SenderInstance, null, null);
                    }
                }
                else
                {
                    Property.SetValue(SenderInstance, m_Candidate, null);
                }
            }
            else if (!IsBluePrint && field != null)
            {
                field.Value = (m_Candidate == null || m_Candidate.ID == 0) ? null : m_Candidate.ID.ToString();
            }

            if (m_Candidate?.ID > 0)
            {
                if (m_Candidate.IsImage && m_Candidate.Width > 0 && m_Candidate.Height > 0)
                {
                    OutputText = $"{m_Candidate.Title} <span>({m_Candidate.Width}px / {m_Candidate.Height}px)</span>";
                }
                else
                {
                    OutputText = $"{m_Candidate.Title} <span>({(m_Candidate.Size > 0 ? (m_Candidate.Size / 1024) : 0)} KB)</span>";
                }
            }

            //  Inherited content section
            if (ShowInheritedData)
            {
                InhertitedOutputText = "None";
                if (field != null && !string.IsNullOrEmpty(field.InheritedValue))
                {
                    m_InheritedCandidate = Image.SelectOne(Utility.ConvertToInt(field.InheritedValue));
                    if (m_InheritedCandidate != null && m_InheritedCandidate.ID > 0)
                    {
                        string inheritedImage = $"<img class=\"preview\" alt=\"Preview\" src=\"{Console.WimRepository.Replace("/wim", string.Empty, StringComparison.InvariantCultureIgnoreCase)}/thumbnail/{m_InheritedCandidate.ID}.jpg\"/>";
                        InhertitedOutputText = $"{inheritedImage}{m_InheritedCandidate.Title} ({(m_InheritedCandidate.Size > 0 ? (m_InheritedCandidate.Size / 1024) : 0)} KB)</a>";
                    }
                }
            }
        }

        Image m_Candidate;
        Image m_InheritedCandidate;

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

                if (Utility.ConvertToInt(sharedInfoApply.outputValue, 0) > 0)
                {
                    m_Candidate = Image.SelectOne(Utility.ConvertToInt(sharedInfoApply.outputValue, 0));
                }
            }

            if (isEditMode && isEnabled)
            {
                Gallery gallery = null;

                if (!string.IsNullOrEmpty(Collection))
                {
                    gallery = Gallery.Identify(Collection);
                    if (gallery == null || gallery.ID == 0)
                    {
                        if (Collection.StartsWith("/"))
                        {
                            var split = Collection.Split('/');
                            var root = Gallery.SelectOneRoot();
                            gallery = new Gallery()
                            {
                                BaseGalleryID = root.ID,
                                Name = split[split.Length - 1],
                                CompletePath = Collection,
                                IsActive = true,
                                IsFolder = true
                            };
                            gallery.Save();
                        }
                        else
                        {
                            OutputText = $"<font color=red>Gallery not found '{Collection}'.</font>";
                            build.Append(GetSimpleTextElement(OutputText));
                            return ReadCandidate(m_Candidate.ID);
                        }
                    }
                }

                if (gallery == null || gallery.ID == 0)
                {
                    gallery = Gallery.SelectOneRoot();
                }

                string addition = null;
                if (!string.IsNullOrEmpty(GalleryProperty))
                {
                    var galleryProperty = SenderInstance.GetType().GetProperty(GalleryProperty);
                    var value = galleryProperty.GetValue(SenderInstance, null);

                    if (value is Gallery)
                    {
                        gallery = (Gallery)value;
                    }
                }

                string galleryUrlParam =
                    string.IsNullOrEmpty(GalleryPropertyUrl)
                        ? gallery.ID.ToString()//(gallery.DatabaseMappingPortal == null ? gallery.ID.ToString() : gallery.CompletePath)
                        : GalleryPropertyUrl
                    ;
                string initialview = string.Concat("gallery=", galleryUrlParam,
                    SavedAssetTypeID > 0
                        ? string.Concat("&sat=", SavedAssetTypeID)
                        : null
                );

                string titleTag = string.Concat(Title, Mandatory ? "<em>*</em>" : "");

                string url = null;
                string lst = null;

                IComponentList documentList = ComponentList.SelectOne(ComponentListType.Documents);
                lst = documentList.ID.ToString();

                if (CanOnlyAdd || m_Candidate.ID == 0)
                {
                    url = string.Concat("&gallery=", galleryUrlParam, "&item=0", "&isimage=1");
                }
                else
                {
                    url = string.Concat("&gallery=", galleryUrlParam, "&isimage=1");
                }

                if (CanOnlyAdd)
                {
                    url += "&onlycreate=1";
                }

                int? key = null;
                if (m_Candidate != null)
                {
                    key = m_Candidate.ID;
                }

                ApplyItemSelect(build, true, true, titleTag, ID, lst, url, false, isRequired, false, false, LayerSize.Normal, (CanOnlyAdd ? false : true), 500
                    , null, new NameItemValue() { Name = ID, ID = key, Value = OutputText }
                    );
            }
            else
            {
                OutputText = "None";
                if (m_Candidate != null && m_Candidate.ID != 0)
                {
                    if (m_Candidate.FileName.Equals(m_Candidate.Title, StringComparison.InvariantCultureIgnoreCase) == true)
                    {
                        OutputText = $"{m_Candidate.FileName} ({(m_Candidate.Size > 0 ? (m_Candidate.Size / 1024) : 0)} KB)</a>";
                    }
                    else
                    {
                        OutputText = $"{m_Candidate.FileName} ({m_Candidate.Title}) ({(m_Candidate.Size > 0 ? (m_Candidate.Size / 1024) : 0)} KB)</a>";
                    }
                }
                build.Append(GetSimpleTextElement(OutputText));
            }

            if (m_Candidate == null)
            {
                return ReadCandidate(0);
            }

            return ReadCandidate(m_Candidate.ID);
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

                if (Mandatory)
                {
                    var hasValue = HasSharedValue();
                    if (hasValue.isSharedField)
                    {
                        return hasValue.hasValue;
                    }

                    return (m_Candidate?.ID > 0);
                }
            }
            return true;
        }
    }
}