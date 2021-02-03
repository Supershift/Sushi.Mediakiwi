using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sushi.Mediakiwi.Data;
using Sushi.Mediakiwi.Framework;
using System.Web.UI.WebControls;

namespace Sushi.Mediakiwi.AppCentre.Data.Implementation
{
    public class PageMappingList : ComponentListTemplate
    {
        public ListItemCollection Redirects
        {
            get
            {
                ListItemCollection lic = new ListItemCollection();
                lic.Add(new ListItem("Alle types", ""));
                foreach (ListItem i in PageMapping.MappingTypes)
                    lic.Add(i);
                lic.Add(new ListItem("Bestand", "-2"));
                return lic;
            }
        }
        [Sushi.Mediakiwi.Framework.ContentListSearchItem.Choice_Dropdown("Type redirect", "Redirects")]
        public int TypeRedirect { get; set; }

        [Sushi.Mediakiwi.Framework.ContentListSearchItem.Choice_Checkbox("Is actief")]
        public bool OnlyActive { get; set; }

        [Sushi.Mediakiwi.Framework.ContentListItem.DataExtend()]
        public IPageMapping Instance { get; set; }

        public PageMappingList()
        {
            //wim.ShowInFullWidthMode = true;
            wim.SearchListCanClickThrough = false;
            ListSearch += new Framework.ComponentSearchEventHandler(PageMappingList_ListSearch);
            ListLoad += new Framework.ComponentListEventHandler(PageMappingList_ListLoad);
            ListSave += new ComponentListEventHandler(PageMappingList_ListSave);
            ListDelete += new ComponentListEventHandler(PageMappingList_ListDelete);
        }

        void PageMappingList_ListDelete(object sender, ComponentListEventArgs e)
        {
            Instance.Delete();
        }

        void PageMappingList_ListSave(object sender, ComponentListEventArgs e)
        {
            if (!string.IsNullOrEmpty(Instance.Query))
            {
                if (!Instance.Query.StartsWith("?"))
                    Instance.Query = String.Concat("?", Instance.Query);
            }

            if (Instance.Path != null && !Instance.Path.StartsWith("/"))
                Instance.Path = String.Concat("/", Instance.Path);
           
            if (Instance.ID < 1)
                Instance.Created = DateTime.Now;
             
            Instance.Save();
        }

        void PageMappingList_ListSearch(object sender, Framework.ComponentListSearchEventArgs e)
        {
            
            wim.ListDataColumns.Add("ID", "ID", ListDataColumnType.UniqueIdentifier);
            wim.ListDataColumns.Add("URL", "NavigateURL");                 
            wim.ListDataColumns.Add("Gaat naar", "RedirectTo");
            wim.ListDataColumns.Add("Type", "Type");       
            wim.ListDataColumns.Add("Actief", "IsActive", 40);            
            wim.ListDataColumns.Add("", "TestLink", 45);
            wim.ListDataColumns.Add("", "EditLink", 20);
            wim.ListData = PageMapping.SelectAllNonList(TypeRedirect, OnlyActive);
        }

        void PageMappingList_ListLoad(object sender, Framework.ComponentListEventArgs e)
        {
            Instance = PageMapping.SelectOne(e.SelectedKey);
            if (IsPostBack)
            {
                if (Request.Params["TargetType"] == "0")
                {
                    Instance.IsInternalLink = true;
                    Instance.IsInternalDoc = false;
                }

                if (Request.Params["TargetType"] == "1")
                {
                    Instance.IsInternalLink = false;
                    Instance.IsInternalDoc = true;
                }
            }
            else
            {
                switch (Instance.TargetType)
                {
                    case 1:
                        Instance.IsInternalLink = false;
                        Instance.IsInternalDoc = true;
                        break;
                    
                    default:

                        Instance.IsInternalLink = true;
                        Instance.IsInternalDoc = false;
                        break;
                }
            }

        }
    }
}
