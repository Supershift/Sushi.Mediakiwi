using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sushi.Mediakiwi.Framework;

namespace Sushi.Mediakiwi.AppCentre.Data.Implementation
{
    public class CopyContent : BaseImplementation
    {
        //public CopyContent()
        //{
        //    wim.CanAddNewItem = false;
        //    wim.CanContainSingleInstancePerDefinedList = true;
        //    wim.OpenInEditMode = true;
            

        //    this.ListLoad += CopyContent_ListLoad;
        //    this.ListAction += CopyContent_ListAction;
        //    this.ListPreRender += CopyContent_ListPreRender;
        //}

        //private void CopyContent_ListAction(object sender, ComponentActionEventArgs e)
        //{
        //    if (Save)
        //    {
        //        if (PageID.HasValue)
        //        {
        //            var sourcePage = Sushi.Mediakiwi.Data.Page.SelectOne(PageID.Value, false);
        //            CurrentPage.OverridePageContentFromPage(sourcePage, wim.CurrentApplicationUser);
        //        }
        //    }
            
        //}

        //[Sushi.Mediakiwi.Framework.ContentListItem.Button("_save", AskConfirmation =true, IconTarget= ButtonTarget.BottomRight, IsPrimary =true)]
        //public bool Save { get; set; }

        //private void CopyContent_ListPreRender(IComponentListTemplate sender, ComponentListEventArgs e)
        //{
        //    if (!PageID.HasValue && IsPostBack)
        //    {
        //        wim.Notification.AddError(Resource.Errror_SelectPage);
        //    }
        //    if (PageID.HasValue)
        //    {
        //        var sourcePage = Sushi.Mediakiwi.Data.Page.SelectOne(PageID.Value);
        //        var sourceComponents = Sushi.Mediakiwi.Data.ComponentVersion.SelectAll(PageID.Value);
        //        var targetComponents = Sushi.Mediakiwi.Data.ComponentVersion.SelectAll(CurrentPageID);
        //        Info = Resource.CopyContentInfo
        //                    .Replace("#sourcePageName#", sourcePage.Name).Replace("#sourceComponentLength#", sourceComponents.Length.ToString()).Replace("#sourceChanged#", sourcePage.Updated.ToString(Resource.DateFormat))
        //                    .Replace("#targetPageName#", CurrentPage.Name).Replace("#targetComponentLength#", targetComponents.Length.ToString()).Replace("#targetChanged#", CurrentPage.Updated.ToString(Resource.DateFormat));

                 
        //    }
        //}

        //public int CurrentPageID
        //{
        //    get { return Wim.Utility.ConvertToInt(Request["item"]); }
        //}
        //Sushi.Mediakiwi.Data.Page _CurrentPage;
        //public Sushi.Mediakiwi.Data.Page CurrentPage
        //{
        //    get
        //    {
        //        if (_CurrentPage == null)
        //        {
        //            _CurrentPage = Sushi.Mediakiwi.Data.Page.SelectOne(CurrentPageID);
        //        }
        //        return _CurrentPage;

        //    }
        //}

        //private ListItemCollection _UsablePages;
        //public ListItemCollection UsablePages
        //{
        //    get
        //    {
        //        if (_UsablePages == null)
        //        {
        //            _UsablePages = new ListItemCollection();
        //            _UsablePages.Add(new ListItem(Resource.Errror_SelectPage, ""));
        //            foreach (var apage in Sushi.Mediakiwi.Data.Page.SelectAllBasedOnPageTemplate(CurrentPage.TemplateID))
        //            {
        //                if (apage.ID != CurrentPageID)
        //                    _UsablePages.Add(new ListItem(apage.CompletePath, apage.ID.ToString()));
        //            }
        //        }
        //        return _UsablePages;
        //    }

        //}
    

        //[Sushi.Mediakiwi.Framework.ContentListItem.Choice_Dropdown("_CopyFromPage", "UsablePages", AutoPostBack =true)]
        //public int? PageID { get; set; }

        //[Sushi.Mediakiwi.Framework.ContentListItem.TextLine("_Info")]
        //public string Info { get; set; }


        //private void CopyContent_ListLoad(Framework.IComponentListTemplate sender, Framework.ComponentListEventArgs e)
        //{

        //}
    }
}
