using Sushi.Mediakiwi.Framework;
using Sushi.Mediakiwi.UI;
using System;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.AppCentre.Data.Implementation
{
    public class CopyContent : BaseImplementation
    {
        public CopyContent()
        {
            wim.CanAddNewItem = false;
            wim.CanContainSingleInstancePerDefinedList = true;
            wim.OpenInEditMode = true;

            ListAction += CopyContent_ListAction;
            ListPreRender += CopyContent_ListPreRender;
        }

        private async Task CopyContent_ListPreRender(ComponentListEventArgs arg)
        {
            if (!PageID.HasValue && IsPostBack)
            {
                wim.Notification.AddError(Labels.Errror_SelectPage);
            }

            if (PageID.HasValue)
            {
                var sourcePage = await Mediakiwi.Data.Page.SelectOneAsync(PageID.Value).ConfigureAwait(false);
                var sourceComponents = await Mediakiwi.Data.ComponentVersion.SelectAllAsync(PageID.Value).ConfigureAwait(false);
                var targetComponents = await Mediakiwi.Data.ComponentVersion.SelectAllAsync(CurrentPageID).ConfigureAwait(false);

                Info = Labels.CopyContentInfo
                            .Replace("#sourcePageName#", sourcePage.Name, StringComparison.InvariantCultureIgnoreCase)
                            .Replace("#sourceComponentLength#", sourceComponents.Length.ToString(), StringComparison.InvariantCultureIgnoreCase)
                            .Replace("#sourceChanged#", sourcePage.Updated.ToString(Labels.DateFormat), StringComparison.InvariantCultureIgnoreCase)
                            .Replace("#targetPageName#", CurrentPage.Name, StringComparison.InvariantCultureIgnoreCase)
                            .Replace("#targetComponentLength#", targetComponents.Length.ToString(), StringComparison.InvariantCultureIgnoreCase)
                            .Replace("#targetChanged#", CurrentPage.Updated.ToString(Labels.DateFormat), StringComparison.InvariantCultureIgnoreCase);
            }
        }

        private async Task CopyContent_ListAction(ComponentActionEventArgs arg)
        {
            if (Save && PageID.HasValue)
            {
                var sourcePage = await Mediakiwi.Data.Page.SelectOneAsync(PageID.Value, false).ConfigureAwait(false);
                await CurrentPage.OverridePageContentFromPageAsync(sourcePage, wim.CurrentApplicationUser).ConfigureAwait(false);
            }
        }


        [Framework.ContentListItem.Button("_save", AskConfirmation = true, IconTarget = ButtonTarget.BottomRight, IsPrimary = true)]
        public bool Save { get; set; }

        public int CurrentPageID
        {
            get { return Mediakiwi.Data.Utility.ConvertToInt(Context.Request.Query["item"]); }
        }

        Mediakiwi.Data.Page _CurrentPage;
        public Mediakiwi.Data.Page CurrentPage
        {
            get
            {
                if (_CurrentPage == null)
                {
                    _CurrentPage = Mediakiwi.Data.Page.SelectOne(CurrentPageID);
                }
                return _CurrentPage;
            }
        }

        private ListItemCollection _UsablePages;
        public ListItemCollection UsablePages
        {
            get
            {
                if (_UsablePages == null)
                {
                    _UsablePages = new ListItemCollection();
                    _UsablePages.Add(new ListItem(Labels.Errror_SelectPage, ""));
                    foreach (var apage in Mediakiwi.Data.Page.SelectAllBasedOnPageTemplate(CurrentPage.TemplateID))
                    {
                        if (apage.ID != CurrentPageID)
                        {
                            _UsablePages.Add(new ListItem(apage.CompletePath, apage.ID.ToString()));
                        }
                    }
                }
                return _UsablePages;
            }
        }


        [Framework.ContentListItem.Choice_Dropdown("_CopyFromPage", nameof(UsablePages), AutoPostBack = true)]
        public int? PageID { get; set; }

        [Framework.ContentListItem.TextLine("_Info")]
        public string Info { get; set; }

    }
}
