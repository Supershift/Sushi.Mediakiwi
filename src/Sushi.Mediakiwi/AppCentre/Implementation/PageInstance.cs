using Sushi.Mediakiwi.Data;
using Sushi.Mediakiwi.Framework;
using Sushi.Mediakiwi.UI;
using System;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.AppCentre.Data.Implementation
{
    /// <summary>
    /// Represents a PageInstance entity.
    /// </summary>
    public class PageInstance : BaseImplementation
    {
        #region Properties

        Page Implement;

        /// <summary>
        /// Gets the template list.
        /// </summary>
        /// <value>The template list.</value>
        public ListItemCollection TemplateList
        {
            get
            {
                ListItemCollection col = new ListItemCollection();
                ListItem li;
                li = new ListItem("Select template", string.Empty);
                col.Add(li);

                foreach (Mediakiwi.Data.PageTemplate template in Mediakiwi.Data.PageTemplate.SelectAllSortedByName())
                {
                    //  Only allow templates that are designated to this site
                    if (template.SiteID.HasValue && template.SiteID.Value != wim.CurrentSite.ID)
                    {
                        if (!wim.CurrentSite.MasterID.HasValue || template.SiteID.Value != wim.CurrentSite.MasterID.Value)
                        {
                            continue;
                        }
                    }

                    //  Check the multiple instance templates
                    if (Implement.ID == 0 && template.OnlyOneInstancePossible && template.PageInstanceCount > 0)
                    {
                        continue;
                    }

                    //if (!string.IsNullOrEmpty(template.ReferenceID))
                    //    col.Add(new ListItem(string.Format("{0} ({1})", template.Name, template.ReferenceID), template.ID.ToString()));
                    //else
                    col.Add(new ListItem(template.Name, template.ID.ToString()));
                }
                return col;
            }
        }

        #endregion Properties

        #region CTor

        /// <summary>
        /// Initializes a new instance of the <see cref="PageInstance"/> class.
        /// </summary>
        public PageInstance()
        {
            wim.CanContainSingleInstancePerDefinedList = true;
            wim.CanAddNewItem = true;
            wim.OpenInEditMode = true;

            ListLoad += PageInstance_ListLoad;
            ListSave += PageInstance_ListSave;
        }

        #endregion CTor
        
        #region List Save

        async Task PageInstance_ListSave(ComponentListEventArgs e)
        {
            //  int previousFolder = m_Implement.FolderID;

            Utility.ReflectProperty(this, Implement);

            if (e.SelectedKey == 0)
            {
                int folderID = wim.CurrentFolder.ID;

                System.Text.RegularExpressions.Regex rex = new System.Text.RegularExpressions.Regex(@"^[a-z|A-Z|0-9| _-]*$");
                string name = Name
                    .Replace("&", "")
                    .Replace("!", "")
                    .Replace("@", "")
                    .Replace("$", "")
                    .Replace("*", "")
                    .Replace(":", "")
                    .Replace(";", "")
                    .Replace(",", "")
                    .Replace("?", "")
                    .Replace("(", "")
                    .Replace(")", "")
                    .Replace("  ", " ");

                //if (IsOverview)
                //{
                //    Sushi.Mediakiwi.Data.Folder folder = new Sushi.Mediakiwi.Data.Folder();
                //    folder.Name = name;
                //    folder.ParentID = Sushi.Mediakiwi.CurrentFolder.ID;
                //    folder.Type = Sushi.Mediakiwi.CurrentFolder.Type;
                //    folder.SiteID = Sushi.Mediakiwi.CurrentFolder.SiteID;

                //    if (string.IsNullOrEmpty(Sushi.Mediakiwi.CurrentFolder.CompletePath))
                //        folder.CompletePath = string.Concat("/", name, "/");
                //    else
                //        folder.CompletePath = string.Concat(Sushi.Mediakiwi.CurrentFolder.CompletePath, name, "/");

                //    folder.Save();

                //    m_Implement.SubFolderID = folder.ID;

                //    //folderID = folder.ID;
                //    //  Replicate to children
                //    Sushi.Mediakiwi.Framework.Inheritance.Folder.CreateFolder(folder, Sushi.Mediakiwi.CurrentSite);
                //}

                Implement.Name = name;
                Implement.LinkText = LinkText?.Trim();
                Implement.Title = Title?.Trim();
                Implement.IsSearchable = true;
                //m_Implement.IsFolderDefault = IsOverview;
                Implement.FolderID = folderID;
                await Implement.SaveAsync();
            }
            else
            {
                Implement.Name = Implement.Name.Trim();
                Implement.Title = Implement.Title.Trim();
                Implement.IsSearchable = IsSearchable;

                if (string.IsNullOrEmpty(Title))
                {
                    Title = LinkText;
                }

                if (string.IsNullOrEmpty(Implement.LinkText))
                {
                    Implement.LinkText = Name;
                }

                await Implement.SaveAsync();
            }

            if (e.SelectedKey == 0)
            {
                await Framework.Inheritance.Page.CreatePageAsync(Implement, wim.CurrentSite);
            }
            else
            {
                await Framework.Inheritance.Page.MovePageAsync(Implement, wim.CurrentSite);
                //if (previousFolder != m_Implement.FolderID)
                //{
                //    Sushi.Mediakiwi.Framework.Inheritance.Page.MovePage(m_Implement, Sushi.Mediakiwi.CurrentSite);
                //}
            }

            await Framework.Functions.FolderPathLogic.UpdateCompletePathAsync(Implement.FolderID);

            wim.FlushCache();
          //  wim.Redirect(string.Concat(wim.Console.WimPagePath, "?page=", Implement.ID, "&tab=Content"));
        }

        #endregion List Save

        #region List Load

        async Task PageInstance_ListLoad(ComponentListEventArgs e)
        {
            Implement = await Page.SelectOneAsync(e.SelectedKey, false);
            Utility.ReflectProperty(Implement, this);

            if (e.SelectedKey == 0)
            {
                FolderID = Utility.ConvertToInt(Request.Query["folder"]);
                IsSearchable = true;
            }
            else
            {
                IsAdmin = wim.CurrentApplicationUser.ID == 1;
                IComponentList clist = await Mediakiwi.Data.ComponentList.SelectOneAsync(ComponentListType.PageTemplates);
                TemplateLink = string.Format("<a href=\"{2}?list={3}&item={0}\">{1}</a>", Implement.Template.ID, Implement.Template.Name, wim.Console.WimPagePath, clist.ID);
            }
        }

        #endregion List Load

        #region UI Properties

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        [Framework.ContentListItem.TextField("_name", 150, true, "The name of the page", Utility.GlobalRegularExpression.OnlyAcceptableFilenameCharacter, Expression = OutputExpression.Alternating)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the link text.
        /// </summary>
        /// <value>The link text.</value>
        [Framework.ContentListItem.TextField("_linktext", 150, false, "If this page is linked, this value is used", Expression = OutputExpression.Alternating)]
        public string LinkText { get; set; }

        /// <summary>
        /// Gets or sets the folder id.
        /// </summary>
        /// <value>The folder id.</value>
        [OnlyEditableWhenTrue("IsNotInherited")]
        [Framework.ContentListItem.FolderSelect("_folder", true, Expression = OutputExpression.Alternating)]
        public int FolderID { get; set; }

        /// <summary>
        /// Gets or sets the sub folder ID.
        /// </summary>
        /// <value>The sub folder ID.</value>
        //[Sushi.Mediakiwi.Framework.OnlyEditableWhenTrue("IsNotInherited")]
        [Framework.ContentListItem.FolderSelect("_subfolder", false, InteractiveHelp = "When this is a listing page you can assign the folder it needs to list.", Expression = OutputExpression.Alternating)]
        public int SubFolderID { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is secure.
        /// </summary>
        /// <value><c>true</c> if this instance is secure; otherwise, <c>false</c>.</value>
        //[Sushi.Mediakiwi.Framework.ContentListItem.Choice_Checkbox("_is_secure", "Is this page secured using SSL (a certificate is required on the server)?")]
        public bool IsSecure { get; set; } = true;


        /// <summary>
        /// Gets or sets the template.
        /// </summary>
        /// <value>The template.</value>
        [Framework.ContentListItem.Choice_Dropdown("Template", nameof(TemplateList), true)]
        public int TemplateID { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is admin.
        /// </summary>
        /// <value><c>true</c> if this instance is admin; otherwise, <c>false</c>.</value>
        public bool IsAdmin { get; set; }

        [OnlyVisibleWhenTrue(nameof(IsAdmin))]
        [Framework.ContentListItem.TextLine("Template")]
        public string TemplateLink { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is overview.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is overview; otherwise, <c>false</c>.
        /// </value>
        //[Sushi.Mediakiwi.Framework.ContentListItem.Choice_Checkbox("Is overzicht", "Is dit een overzichtspagina met subnavigatie?", Expression = OutputExpression.Alternating)]
        public bool IsOverview { get; set; }

        /// <summary>
        /// Gets or sets the template.
        /// </summary>
        /// <value>The template.</value>
        [Framework.ContentListItem.Section()]
        public string Info1 { get; set; } = "Metatags";
 
        /// <summary>
        /// Gets or sets the page title.
        /// </summary>
        /// <value>The page title.</value>
        [Framework.ContentListItem.TextField("Browser title", 250, false, InteractiveHelp = "If not applied the channel setting is used.")]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        [Framework.ContentListItem.TextArea("Description", 500, false)]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the keywords.
        /// </summary>
        /// <value>The keywords.</value>
        [Framework.ContentListItem.TextField("Keywords", 500, false)]
        public string Keywords { get; set; }

        /// <summary>
        /// Gets or sets the template.
        /// </summary>
        /// <value>The template.</value>
        [Framework.ContentListItem.Section()]
        public string Info { get; set; } = "Page publication";


        /// <summary>
        /// Gets a value indicating whether this instance is not new.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is not new; otherwise, <c>false</c>.
        /// </value>
        public bool IsNotNew
        {
            get { return Implement.ID != 0; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is not inherited.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is not inherited; otherwise, <c>false</c>.
        /// </value>
        public bool IsNotInherited
        {
            get { return !Implement.MasterID.HasValue; }
        }

        /// <summary>
        /// Gets or sets the template.
        /// </summary>
        /// <value>The template.</value>
        [Framework.ContentListItem.DateTime("Publish", false, "If applied the page will be published at this date and time.", Expression = OutputExpression.Alternating)]
        public DateTime? Publication { get; set; }

        /// <summary>
        /// Gets or sets the template.
        /// </summary>
        /// <value>The template.</value>
        [Framework.ContentListItem.Choice_Checkbox("_is_searchable", "Is this page indexed by search enigines?", Expression = OutputExpression.Alternating)]
        public bool IsSearchable { get; set; }

        /// <summary>
        /// Gets or sets the template.
        /// </summary>
        /// <value>The template.</value>
        [Framework.ContentListItem.DateTime("Expires", false, "If applied the page will expire at this date and time.", Expression = OutputExpression.Alternating)]
        public DateTime? Expiration { get; set; }

        /// <summary>
        /// Gets or sets the template.
        /// </summary>
        /// <value>The template.</value>
        [Framework.ContentListItem.Choice_Checkbox("_is_default", "Is dit de standaard pagina in de huidige folder?", Expression = OutputExpression.Alternating)]
        public bool IsFolderDefault { get; set; }

        #endregion UI Properties
    }
}
