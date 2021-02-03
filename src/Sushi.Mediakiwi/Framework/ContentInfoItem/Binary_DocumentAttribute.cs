using Sushi.Mediakiwi.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Sushi.Mediakiwi.Framework.ContentInfoItem
{
    /// <summary>
    /// Possible return types: System.Int32, System.String, Sushi.Mediakiwi.Data.Document
    /// </summary>
    public class Binary_DocumentAttribute : ContentSharedAttribute, IContentInfo
    {
        /// <summary>
        /// Possible return types: System.Int32, System.String, Sushi.Mediakiwi.Data.Document, Sushi.Mediakiwi.Data.AssetInfo
        /// </summary>
        /// <param name="title"></param>
        public Binary_DocumentAttribute(string title)
            : this(title, false) { }

        /// <summary>
        /// Possible return types: System.Int32, System.String, Sushi.Mediakiwi.Data.Document, Sushi.Mediakiwi.Data.AssetInfo
        /// </summary>
        /// <param name="title"></param>
        /// <param name="mandatory"></param>
        public Binary_DocumentAttribute(string title, bool mandatory)
            : this(title, mandatory, null) { }


        /// <summary>
        /// Possible return types: System.Int32, System.String, Sushi.Mediakiwi.Data.Document, Sushi.Mediakiwi.Data.AssetInfo
        /// </summary>
        /// <param name="title"></param>
        /// <param name="mandatory"></param>
        /// <param name="interactiveHelp"></param>
        public Binary_DocumentAttribute(string title, bool mandatory, string interactiveHelp)
            : this(title, mandatory, null, interactiveHelp) { }

        /// <summary>
        /// Possible return types: System.Int32, System.String, Sushi.Mediakiwi.Data.Document, Sushi.Mediakiwi.Data.AssetInfo
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

        Data.AssetInfo m_Candidate2;
 
        /// <summary>
        /// Sets the candidate.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="isEditMode">if set to <c>true</c> [is edit mode].</param>
        public void SetCandidate(Field field, bool isEditMode)
        {
            this.SetMultiFieldTitleHTML(Labels.ResourceManager.GetString("input_doc", new CultureInfo(Console.CurrentApplicationUser.LanguageCulture)), "icon-file");

            //  Set because of new post _0 detection
            //this.IsMultiFile = this.m_IsNewDesign;

            if (Property != null && Property.PropertyType == typeof(Data.CustomData))
                SetContentContainer(field);

            m_Candidate = new Sushi.Mediakiwi.Data.Document();

            if (Property != null && Property.PropertyType == typeof(Data.AssetInfo))
            {
                m_Candidate2 = Property.GetValue(SenderInstance, null) as Data.AssetInfo;

                if (m_Candidate2 != null)
                {
                    this.CanOnlyCreate = m_Candidate2.m_CanOnlyCreate;

                    if (m_Candidate2.m_GalleryID.HasValue)
                        this.Collection = m_Candidate2.m_GalleryID.GetValueOrDefault().ToString();
                }
            }


            if (IsInitialLoad || !isEditMode)
            {
                if (IsBluePrint)
                {
                    if (field != null)
                    {
                        int candidate = Data.Utility.ConvertToInt(field.Value);
                        if (candidate > 0)
                            m_Candidate = Data.Document.SelectOne(candidate);
                    }
                }
                else
                {
                    if (Property.PropertyType == typeof(Data.CustomData))
                    {
                        m_Candidate = m_ContentContainer[field.Property].ParseDocument();
                    }
                    else if (Property.PropertyType == typeof(Data.Document))
                    {
                        m_Candidate = Property.GetValue(SenderInstance, null) as Data.Document;
                    }
                    else if (Property.PropertyType == typeof(Data.AssetInfo))
                    {
                        if (m_Candidate2 != null)
                            m_Candidate = Data.Document.SelectOne(m_Candidate2.AssetID);
                    }
                    else if (Property.PropertyType == typeof(int?))
                    {
                        int? tmp = Property.GetValue(SenderInstance, null) as int?;
                        if (tmp.HasValue)
                            m_Candidate = Data.Document.SelectOne(tmp.Value);
                    }
                    else
                    {
                        int candidate = Data.Utility.ConvertToInt(Property.GetValue(SenderInstance, null));
                        if (candidate > 0)
                            m_Candidate = Data.Document.SelectOne(candidate);
                    }
                }
            }
            else
            {
                var candidate = SelectedID;
                if (candidate.HasValue)
                    m_Candidate = Data.Asset.SelectOne(candidate.Value).DocumentInstance;
            }

            if (!IsBluePrint && Property != null && Property.CanWrite)
            {
                if (Property.PropertyType == typeof(Data.CustomData))
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
                else if (Property.PropertyType == typeof(Data.AssetInfo))
                {
                    if (m_Candidate2 == null)
                    {
                        m_Candidate2 = new Sushi.Mediakiwi.Data.AssetInfo();
                    }

                    m_Candidate2.AssetID = m_Candidate.ID;
                    Property.SetValue(SenderInstance, m_Candidate2, null);
                }
                else
                    Property.SetValue(SenderInstance, m_Candidate, null);
            }

            if (m_Candidate == null)
                m_Candidate = new Sushi.Mediakiwi.Data.Document();

            if (m_Candidate != null && m_Candidate.ID > 0)
                OutputText = string.Format("{0} ({1} KB)", m_Candidate.Title, m_Candidate.Size > 0 ? (m_Candidate.Size / 1024) : 0);


            //  Inherited content section
            if (ShowInheritedData)
            {
                if (field != null && !string.IsNullOrEmpty(field.InheritedValue))
                {
                    m_InheritedCandidate = Sushi.Mediakiwi.Data.Document.SelectOne(Data.Utility.ConvertToInt(field.InheritedValue));

                    if (m_InheritedCandidate != null && m_InheritedCandidate.ID != 0)
                    {
                        InhertitedOutputText = string.Format("<a href=\"{2}\">{0} ({1} KB)</a>", m_InheritedCandidate.Title, m_InheritedCandidate.Size > 0 ? (m_InheritedCandidate.Size / 1024) : 0, m_InheritedCandidate.DownloadUrl);
                    }
                }
            }
        }

        Sushi.Mediakiwi.Data.Document m_Candidate;
        Sushi.Mediakiwi.Data.Document m_InheritedCandidate;

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
            if (OverrideEditMode) 
                isEditMode = false;

            if (m_Candidate != null && m_Candidate.ID != 0)
            {
                if (System.IO.File.Exists(m_Candidate.LocalFilePath) || !string.IsNullOrEmpty(m_Candidate.RemoteLocation))
                    OutputText = string.Format("<a href=\"{2}\">{0} ({1} KB)</a>"
                        , m_Candidate.Title
                        , m_Candidate.Size > 0 
                            ? (m_Candidate.Size / 1024) 
                            : 0
                        , m_Candidate.DownloadUrl
                    );
                else
                    OutputText = string.Format("<a>{0} ({1} KB) : (not present on server)</a>"
                        , m_Candidate.Title
                        , m_Candidate.Size > 0 ? (m_Candidate.Size / 1024) : 0
                        );

            }
            else
                OutputText = "";

            if (isEditMode && this.IsEnabled())
            {
                Sushi.Mediakiwi.Data.Gallery gallery = null;
               if (!string.IsNullOrEmpty(Collection))
               {
                   gallery = Data.Gallery.Identify(this.Collection);
                   if (gallery == null || gallery.ID == 0)
                   {
                       if (this.Collection.StartsWith("/"))
                       {
                           var split = this.Collection.Split('/');
                           var root = Sushi.Mediakiwi.Data.Gallery.SelectOneRoot();
                           gallery = new Data.Gallery()
                           {
                               ParentID = root.ID,
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
                    gallery = Sushi.Mediakiwi.Data.Gallery.SelectOneRoot();

                string galleryUrlParam = gallery.ID.ToString();
                //gallery.DatabaseMappingPortal == null ? gallery.ID.ToString() : gallery.CompletePath;

                string titleTag =  string.Concat(Title, Mandatory ? "<em>*</em>" : "");

                string url = null;
                string lst = null;
                if (CanOnlyCreate)
                {
                    Data.IComponentList documentList = Data.ComponentList.SelectOne(Sushi.Mediakiwi.Data.ComponentListType.Documents);
                    lst = documentList.ID.ToString();
                    url = string.Concat("&gallery=", galleryUrlParam, "&item=0");
                }
                else
                {
                    url = string.Concat("&gallery=", galleryUrlParam, "&item=0");

                }

                int? key = null;
                if (m_Candidate != null)
                    key = m_Candidate.ID;

                ApplyItemSelect(build, true, true, titleTag, this.ID, lst, url, false, isRequired, false, false, LayerSize.Small, false, 350,
                    null,
                    new NameItemValue() { Name = this.ID, ID = key, Value = OutputText }
                    );

            }
            else
            {
                build.Append(GetSimpleTextElement(this.Title, this.Mandatory, OutputText, this.InteractiveHelp));
            }
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
                        return m_Candidate.ID != 0;
                }
                return true;
        }
    }
}
