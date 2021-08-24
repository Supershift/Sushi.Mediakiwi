using Sushi.Mediakiwi.Data;
using System;
using System.Globalization;

namespace Sushi.Mediakiwi.Framework.ContentInfoItem
{
    /// <summary>
    /// Possible return types: System.Int32, System.String, Document
    /// </summary>
    public class Binary_DocumentAttribute : ContentSharedAttribute, IContentInfo
    {
        /// <summary>
        /// Possible return types: System.Int32, System.String, Document, AssetInfo
        /// </summary>
        /// <param name="title"></param>
        public Binary_DocumentAttribute(string title)
            : this(title, false) { }

        /// <summary>
        /// Possible return types: System.Int32, System.String, Document, AssetInfo
        /// </summary>
        /// <param name="title"></param>
        /// <param name="mandatory"></param>
        public Binary_DocumentAttribute(string title, bool mandatory)
            : this(title, mandatory, null) { }


        /// <summary>
        /// Possible return types: System.Int32, System.String, Document, AssetInfo
        /// </summary>
        /// <param name="title"></param>
        /// <param name="mandatory"></param>
        /// <param name="interactiveHelp"></param>
        public Binary_DocumentAttribute(string title, bool mandatory, string interactiveHelp)
            : this(title, mandatory, null, interactiveHelp) { }

        /// <summary>
        /// Possible return types: System.Int32, System.String, Document, AssetInfo
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="mandatory">if set to <c>true</c> [mandatory].</param>
        /// <param name="gallery">The indentifier GUID of the gallery.</param>
        /// <param name="interactiveHelp">The interactive help.</param>
        public Binary_DocumentAttribute(string title, bool mandatory, string gallery, string interactiveHelp)
        {
            m_CanHaveExpression = true;
            ContentTypeSelection = ContentType.Binary_Document;
            Title = title;
            Mandatory = mandatory;
            InteractiveHelp = interactiveHelp;
            Collection = gallery;
        }

        bool _CanOnlyCreate = true;
        /// <summary>
        /// Gets or sets a value indicating whether this instance can only create new documents (and not change/select other documents). 
        /// The gallery (GUID or INT32) should be set.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can only create; otherwise, <c>false</c>.
        /// </value>
        public bool CanOnlyCreate
        {
            get { return _CanOnlyCreate; }
            set { _CanOnlyCreate = value; }
        }

        /// <summary>
        /// Sets the candidate.
        /// </summary>
        /// <param name="isEditMode">if set to <c>true</c> [is edit mode].</param>
        public void SetCandidate(bool isEditMode)
        {
            SetCandidate(null, isEditMode);
        }

        AssetInfo m_Candidate2;

        /// <summary>
        /// Sets the candidate.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="isEditMode">if set to <c>true</c> [is edit mode].</param>
        public void SetCandidate(Field field, bool isEditMode)
        {
            SetMultiFieldTitleHTML(Labels.ResourceManager.GetString("input_doc", new CultureInfo(Console.CurrentApplicationUser.LanguageCulture)), "icon-file");

            //  Set because of new post _0 detection
            //IsMultiFile = m_IsNewDesign;

            if (Property != null && Property.PropertyType == typeof(CustomData))
            {
                SetContentContainer(field);
            }

            m_Candidate = new Document();

            if (Property != null && Property.PropertyType == typeof(AssetInfo))
            {
                m_Candidate2 = Property.GetValue(SenderInstance, null) as AssetInfo;

                if (m_Candidate2 != null)
                {
                    CanOnlyCreate = m_Candidate2.m_CanOnlyCreate;

                    if (m_Candidate2.m_GalleryID.HasValue)
                    {
                        Collection = m_Candidate2.m_GalleryID.GetValueOrDefault().ToString();
                    }
                }
            }


            if (IsInitialLoad || !isEditMode)
            {
                if (IsBluePrint)
                {
                    if (field != null)
                    {
                        int candidate = Utility.ConvertToInt(field.Value);
                        if (candidate > 0)
                        {
                            m_Candidate = Document.SelectOne(candidate);
                        }
                    }
                }
                else
                {
                    if (Property.PropertyType == typeof(CustomData))
                    {
                        m_Candidate = m_ContentContainer[field.Property].ParseDocument();
                    }
                    else if (Property.PropertyType == typeof(Document))
                    {
                        m_Candidate = Property.GetValue(SenderInstance, null) as Document;
                    }
                    else if (Property.PropertyType == typeof(AssetInfo))
                    {
                        if (m_Candidate2 != null)
                        {
                            m_Candidate = Document.SelectOne(m_Candidate2.AssetID);
                        }
                    }
                    else if (Property.PropertyType == typeof(int?))
                    {
                        int? tmp = Property.GetValue(SenderInstance, null) as int?;
                        if (tmp.HasValue)
                        {
                            m_Candidate = Document.SelectOne(tmp.Value);
                        }
                    }
                    else
                    {
                        int candidate = Utility.ConvertToInt(Property.GetValue(SenderInstance, null));
                        if (candidate > 0)
                        {
                            m_Candidate = Document.SelectOne(candidate);
                        }
                    }
                }
            }
            else
            {
                var candidate = SelectedID;
                if (candidate.HasValue)
                {
                    m_Candidate = Asset.SelectOne(candidate.Value).DocumentInstance;
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
                else if (Property.PropertyType == typeof(AssetInfo))
                {
                    if (m_Candidate2 == null)
                    {
                        m_Candidate2 = new AssetInfo();
                    }

                    m_Candidate2.AssetID = m_Candidate.ID;
                    Property.SetValue(SenderInstance, m_Candidate2, null);
                }
                else
                {
                    Property.SetValue(SenderInstance, m_Candidate, null);
                }
            }

            if (m_Candidate == null)
            {
                m_Candidate = new Document();
            }

            if (m_Candidate?.ID > 0)
            {
                OutputText = $"{m_Candidate.Title} <span>({(m_Candidate.Size > 0 ? (m_Candidate.Size / 1024) : 0)} KB)</span>";
            }


            //  Inherited content section
            if (ShowInheritedData && field != null && !string.IsNullOrEmpty(field.InheritedValue))
            {
                m_InheritedCandidate = Document.SelectOne(Utility.ConvertToInt(field.InheritedValue));

                if (m_InheritedCandidate?.ID > 0)
                {
                    InhertitedOutputText = $"<a href=\"{m_InheritedCandidate.DownloadUrl}\">{m_InheritedCandidate.Title} ({(m_InheritedCandidate.Size > 0 ? (m_InheritedCandidate.Size / 1024) : 0)} KB)</a>";
                }
            }
        }

        Document m_Candidate;
        Document m_InheritedCandidate;

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
                    m_Candidate = Document.SelectOne(Utility.ConvertToInt(sharedInfoApply.outputValue, 0));
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
                        if (Collection.StartsWith("/", StringComparison.InvariantCultureIgnoreCase))
                        {
                            var split = Collection.Split('/');
                            var root = Gallery.SelectOneRoot();
                            gallery = new Gallery()
                            {
                                ParentID = root.ID,
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

                string galleryUrlParam = gallery.ID.ToString();

                string titleTag = string.Concat(Title, Mandatory ? "<em>*</em>" : "");

                string url = null;
                string lst = null;

                IComponentList documentList = ComponentList.SelectOne(ComponentListType.Documents);
                lst = documentList.ID.ToString();

                if (m_Candidate.ID == 0)
                {
                    url = string.Concat("&gallery=", galleryUrlParam, "&item=0", "&isimage=0");
                }
                else
                {
                    url = string.Concat("&gallery=", galleryUrlParam, "&isimage=0");
                }

                int? key = null;

                if (m_Candidate?.ID > 0)
                {
                    key = m_Candidate.ID;
                }

                ApplyItemSelect(build, true, isEnabled, titleTag, ID, lst, url, false, isRequired, false, false, LayerSize.Small, false, 450,
                    null,
                    new NameItemValue() { Name = ID, ID = key, Value = OutputText }
                    );

            }
            else
            {
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
                    return false;

                if (Mandatory)
                    return m_Candidate.ID != 0;
            }
            return true;
        }
    }
}
