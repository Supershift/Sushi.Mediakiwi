using Sushi.Mediakiwi.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Sushi.Mediakiwi.Framework.ContentInfoItem
{
    /// <summary>
    /// Possible return types: System.Int32, System.Int32[nullable], System.String, Sushi.Mediakiwi.Data.Image
    /// </summary>
    public class Binary_ImageAttribute : ContentSharedAttribute, IContentInfo
    {
        /// <summary>
        /// Possible return types: System.Int32, System.Int32[nullable], System.String, Sushi.Mediakiwi.Data.Image
        /// </summary>
        /// <param name="title"></param>
        public Binary_ImageAttribute(string title)
            : this(title, false) { }

        /// <summary>
        /// Possible return types: System.Int32, System.Int32[nullable], System.String, Sushi.Mediakiwi.Data.Image
        /// </summary>
        /// <param name="title"></param>
        /// <param name="mandatory"></param>
        public Binary_ImageAttribute(string title, bool mandatory)
            : this(title, mandatory, null) { }


        /// <summary>
        /// Possible return types: System.Int32, System.Int32[nullable], System.String, Sushi.Mediakiwi.Data.Image
        /// </summary>
        /// <param name="title"></param>
        /// <param name="mandatory"></param>
        /// <param name="interactiveHelp"></param>
        public Binary_ImageAttribute(string title, bool mandatory, string interactiveHelp)
            : this(title, mandatory, null, interactiveHelp) { }

        /// <summary>
        /// Possible return types: System.Int32, System.Int32[nullable], System.String, Sushi.Mediakiwi.Data.Image
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

        /// <summary>
        /// Gets or sets a value indicating whether this instance can only add.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can only add; otherwise, <c>false</c>.
        /// </value>
        bool _CanOnlyAdd = true;
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
            this.SetMultiFieldTitleHTML(Labels.ResourceManager.GetString("input_image", new CultureInfo(Console.CurrentApplicationUser.LanguageCulture)), "icon-photo");

            if (Property != null && Property.PropertyType == typeof(Data.CustomData))
                SetContentContainer(field);
            Data.Gallery gallery = null;
            if (!string.IsNullOrEmpty(this.GalleryProperty))
            {
                var galleryProperty = this.SenderInstance.GetType().GetProperty(this.GalleryProperty);
                var value = galleryProperty.GetValue(this.SenderInstance, null);

                if (value is Sushi.Mediakiwi.Data.Gallery)
                {
                    gallery = (Sushi.Mediakiwi.Data.Gallery)value;
                }
            }

            string galleryMap = gallery == null ? null : gallery.CompletePath;

            m_Candidate = new Sushi.Mediakiwi.Data.Image();
            if (IsInitialLoad || !isEditMode)
            {
                if (IsBluePrint)
                {
                    if (field != null)
                    {
                        int candidate = Data.Utility.ConvertToInt(field.Value);

                        if (candidate > 0)
                        {
                            var asset = Asset.SelectOne(candidate);
                            m_Candidate = asset.ImageInstance;
                        }
                    }
                }
                else
                {
                    if (Property.PropertyType == typeof(Data.CustomData))
                    {
                        int candidate = Data.Utility.ConvertToInt(m_ContentContainer[field.Property].Value);
                        if (candidate > 0)
                            m_Candidate = Data.Image.SelectOne(candidate);
                    }
                    else if (Property.PropertyType == typeof(Data.Image))
                        m_Candidate = Property.GetValue(SenderInstance, null) as Data.Image;
                    else if (Property.PropertyType == typeof(int?))
                    {
                        int? tmp = Property.GetValue(SenderInstance, null) as int?;
                        if (tmp.HasValue)
                        {
                            // CB 01-09-2014; dit lost het probleem op dat assets uit een andere database niet getoond werden; Zoals artikelbeheer ETL ImagesPageImagesList.cs
                            //if (gallery != null && gallery.DatabaseMappingPortal != null)
                            //{
                            //    m_Candidate = Data.Image.SelectOneByPortal(tmp.Value, gallery.DatabaseMappingPortal.Name);
                            //}
                            //else
                            m_Candidate = Data.Image.SelectOne(tmp.Value);

                        }
                    }
                    else
                    {
                        int candidate = Data.Utility.ConvertToInt(Property.GetValue(SenderInstance, null));
                        if (candidate > 0)
                            m_Candidate = Data.Image.SelectOne(candidate);
                    }
                }
            }
            else
            {
                var candidate = SelectedID;
                if (candidate.HasValue)
                {
                    m_Candidate = Data.Image.SelectOne(candidate.Value);
                }

            }

            if (!IsBluePrint && Property != null && Property.CanWrite)
            {
                if (Property.PropertyType == typeof(Data.CustomData))
                    ApplyContentContainer(field, m_Candidate.ID.ToString());
                else if (Property.PropertyType == typeof(int))
                    Property.SetValue(SenderInstance, m_Candidate.ID, null);
                else if (Property.PropertyType == typeof(int?))
                {
                    if (m_Candidate != null && m_Candidate.ID != 0)
                        Property.SetValue(SenderInstance, m_Candidate.ID, null);
                    else
                        Property.SetValue(SenderInstance, null, null);
                }
                else
                    Property.SetValue(SenderInstance, m_Candidate, null);
            }
            else if (!IsBluePrint && field != null)
            {
                field.Value = (m_Candidate == null || m_Candidate.ID == 0) ? null : m_Candidate.ID.ToString();
            }

            if (m_Candidate.ID > 0)
            {
                if (m_Candidate.IsImage)
                    OutputText = string.Format("{0} <span>({1}px / {2}px)</span>", m_Candidate.Title, m_Candidate.Width, m_Candidate.Height);
                else
                    OutputText = string.Format("{0} <span>({1} KB)</span>", m_Candidate.Title, m_Candidate.Size > 0 ? (m_Candidate.Size / 1024) : 0);
            }

            //  Inherited content section
            if (ShowInheritedData)
            {
                InhertitedOutputText = "None";
                if (field != null && !string.IsNullOrEmpty(field.InheritedValue))
                {
                    m_InheritedCandidate = Sushi.Mediakiwi.Data.Image.SelectOne(Data.Utility.ConvertToInt(field.InheritedValue));
                    if (m_InheritedCandidate != null && m_InheritedCandidate.ID > 0)
                    {
                        string inheritedImage = string.Format("<img class=\"preview\" alt=\"Preview\" src=\"{0}/thumbnail/{1}.jpg\"/> ", this.Console.WimRepository.Replace("/wim", String.Empty), m_InheritedCandidate.ID);
                        InhertitedOutputText = string.Format("{2}{0} ({1} KB)</a>", m_InheritedCandidate.Title, m_InheritedCandidate.Size > 0 ? (m_InheritedCandidate.Size / 1024) : 0, inheritedImage);
                    }
                }
            }
        }

        Sushi.Mediakiwi.Data.Image m_Candidate;
        Sushi.Mediakiwi.Data.Image m_InheritedCandidate;

        /// <summary>
        /// Writes the candidate.
        /// </summary>
        /// <param name="build">The build.</param>
        /// <param name="isEditMode">if set to <c>true</c> [is edit mode].</param>
        /// <param name="isRequired">if set to <c>true</c> [is required].</param>
        /// <returns></returns>
        public Field WriteCandidate(WimControlBuilder build, bool isEditMode, bool isRequired, bool isCloaked)
        {
            this.SetWriteEnvironment();

            this.IsCloaked = isCloaked;
            this.Mandatory = isRequired;
            if (OverrideEditMode) isEditMode = false;

            if (isEditMode && this.IsEnabled())
            {
                Data.Gallery gallery = null;

                if (!string.IsNullOrEmpty(this.Collection))
                {
                    gallery = Data.Gallery.Identify(this.Collection);
                    if (gallery == null || gallery.ID == 0)
                    {
                        if (this.Collection.StartsWith("/"))
                        {
                            var split = this.Collection.Split('/');
                            var root = Sushi.Mediakiwi.Data.Gallery.SelectOneRoot();
                            gallery = new Data.Gallery() {
                                BaseGalleryID = root.ID,
                                Name = split[split.Length - 1],
                                CompletePath = this.Collection,
                                IsActive = true,
                                IsFolder = true
                            };
                            gallery.Save();
                        }
                        else
                        {
                            OutputText = string.Format("<font color=red>Gallery not found '{0}'.</font>", this.Collection);
                            build.Append(GetSimpleTextElement(this.Title, this.Mandatory, OutputText, this.InteractiveHelp));
                            return ReadCandidate(m_Candidate.ID);
                        }
                    }
                }

                if (gallery == null || gallery.ID == 0)
                {
                    gallery = Data.Gallery.SelectOneRoot();
                }

                string addition = null;
                if (!string.IsNullOrEmpty(this.GalleryProperty))
                {
                    var galleryProperty = this.SenderInstance.GetType().GetProperty(this.GalleryProperty);
                    var value = galleryProperty.GetValue(this.SenderInstance, null);

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
                    this.SavedAssetTypeID > 0
                        ? string.Concat("&sat=", SavedAssetTypeID)
                        : null
                );

                string titleTag = string.Concat(Title, Mandatory ? "<em>*</em>" : "");

                string url = null;
                string lst = null;

                Data.IComponentList documentList = Data.ComponentList.SelectOne(
                    new Guid("f6252c60-cff3-4c8b-922e-f1d1299cca43"));
                lst = documentList.ID.ToString();

                if (CanOnlyAdd || m_Candidate.ID == 0)
                {
                    url = string.Concat("&gallery=", galleryUrlParam, "&item=0");
                }
                else
                {
                    url = string.Concat("&gallery=", galleryUrlParam);
                }

                int? key = null;
                if (m_Candidate != null)
                    key = m_Candidate.ID;

                ApplyItemSelect(build, true, true, titleTag, this.ID, lst, url, false, isRequired, false, false, (CanOnlyAdd ? LayerSize.Normal : LayerSize.Normal), (CanOnlyAdd ? false : true), 450
                    , null, new NameItemValue() { Name = this.ID, ID = key, Value = OutputText }
                    );
            }
            else
            {
                string image = null;
                OutputText = "None";
                if (m_Candidate != null && m_Candidate.ID != 0)
                {
                    //image = string.Format("<img class=\"preview\" alt=\"Preview\" src=\"{0}\"/> ", m_Candidate.ThumbnailPath);
                    image = m_Candidate.FileName;
                    OutputText = string.Format("{2}{0} ({1} KB)</a>", m_Candidate.Title, m_Candidate.Size > 0 ? (m_Candidate.Size / 1024) : 0, image);
                }
                build.Append(GetSimpleTextElement(this.Title, this.Mandatory, OutputText, this.InteractiveHelp));
            }

            if (m_Candidate == null)
                return ReadCandidate(0);
            return ReadCandidate(m_Candidate.ID);
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
                    return !(m_Candidate == null || m_Candidate.ID == 0);
            }
            return true;
        }
    }
}