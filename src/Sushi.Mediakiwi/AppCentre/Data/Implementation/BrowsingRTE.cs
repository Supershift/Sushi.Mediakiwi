using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using Sushi.Mediakiwi.Framework;

namespace Sushi.Mediakiwi.AppCentre.Data.Implementation
{
    //public class BrowsingRTE : Sushi.Mediakiwi.AppCentre.Data.Implementation.Document  
    //{
    //    public ListItemCollection AlignOptions
    //    {
    //        get
    //        {
    //            ListItemCollection lic = new ListItemCollection();
    //            lic.Add(new ListItem("Left", "left"));
    //            lic.Add(new ListItem("Right", "right"));
    //            return lic;
    //        }
    //    }
    //    [Sushi.Mediakiwi.Framework.ContentListItem.Choice_Radio("Align", "AlignOptions", "alignOptions")]
    //    public string Aligned { get; set; }

    //    public BrowsingRTE()
    //    {
            
    //    }

       
    //}

    public class BrowsingRTE : ComponentListTemplate
    {
        public ListItemCollection AlignOptions
        {
            get
            {
                ListItemCollection lic = new ListItemCollection();
                lic.Add(new ListItem("Left", "left"));
                lic.Add(new ListItem("Center", "center"));
                lic.Add(new ListItem("Right", "right"));
                return lic;
            }
        }
 
        [Sushi.Mediakiwi.Framework.ContentListItem.Choice_Radio("Align", "AlignOptions", "alignOptions")]
        public string Aligned { get; set; }

        [Sushi.Mediakiwi.Framework.ContentListItem.Binary_Image("Image")]
        public int AssetID { get; set; }

        [Sushi.Mediakiwi.Framework.ContentListItem.Button("Save",true)]
        public bool SaveChange { get; set; }

        public string GetBackUrl
        {
            get
            {

                // Note 1:  See library.js line 2328 for reference implementation
                // Note 2:  The return object should be serialized from a .net object to increase re-usability.
                // Note 3: returnFromWim function is located in nicEdit.js on line 1813
                return @"<script>         parent.currentNicWimImageButton.returnFromWim({
									            img: parent.document.getElementById('repository').value + '/../doc.ashx?"+AssetID+@"',
                                                align: '"+Aligned+"', "+
                                                @"id: "+AssetID+ @",
                                                value: -1
									        });
                        parent.classBehaviour.openLayerPopUp.hide(parent.document.getElementById('popUpWithIframe').getElementsByTagName('a')[0]);
                        </script>";
            }
            set { }
        }

        public BrowsingRTE()
        {
            wim.CanContainSingleInstancePerDefinedList = true;
            wim.OpenInEditMode = true;
            ListLoad += new Framework.ComponentListEventHandler(BrowsingRTE_ListLoad);
            ListSave += new Framework.ComponentListEventHandler(BrowsingRTE_ListSave);
         
        }

        void BrowsingRTE_ListSave(object sender, Framework.ComponentListEventArgs e)
        {
            wim.OnSaveScript = GetBackUrl;
        }

      

   

        void BrowsingRTE_ListLoad(object sender, Framework.ComponentListEventArgs e)
        {
            if (!String.IsNullOrEmpty(Request["align"])) Aligned = Request["align"];
            if (!String.IsNullOrEmpty(Request["image"])) AssetID = Wim.Utility.ConvertToInt(Request["image"]);
        }


    }
}
