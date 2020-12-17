using Microsoft.AspNetCore.Http;
using Sushi.Mediakiwi.Framework;
using Sushi.Mediakiwi.UI;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;

namespace Sushi.Mediakiwi.AppCentre.UI.Forms
{
    public class DemonstrationForm : FormMap<DemonstrationForm>
    {
        public DemonstrationForm(bool isLayerMode = false)
        {
            Load(this);

            bool mandatory = false;

            Map(x => x.TextField).TextField("TextField", 50, mandatory, true, "Interactive help");
            Map(x => x.TextArea).TextArea("TextArea", 500, mandatory, "Interactive help");
            Map(x => x.Richtext).RichText("Richtext", 500, mandatory, "Interactive help");

            Map(x => x.Sourcecode).TextArea("Sourcecode", 500, mandatory, "Interactive help", null, true);
            Map(x => x.TextLine).TextLine("TextLine", false, "Interactive help");
            Map(x => x.Section).Section("Section", false, true, true);

            Map(x => x.Date).Date("Date", mandatory, "Interactive help").Expression(OutputExpression.Alternating);
            Map(x => x.DateTime).DateTime("DateTime", mandatory, false, "Interactive help").Expression(OutputExpression.Alternating);

            List = new ListItemCollection();
            List.Add(new ListItem("A"));
            List.Add(new ListItem("B"));
            List.Add(new ListItem("C"));

            Map(x => x.Radio).Radio("Radio", "List", "Radio", mandatory, true, "Interactive help").Expression(OutputExpression.Alternating); ;
            Map(x => x.Checkbox).Checkbox("Checkbox", mandatory, "Interactive help").Expression(OutputExpression.Alternating);
            Map(x => x.Dropdown1).Dropdown("Dropdown", "List", mandatory, true, false, "Interactive help");
            Map(x => x.Dropdown2).Dropdown("Dropdown-multi", "List", mandatory, false, true, "Interactive help");
            Map(x => x.Tagging).Tagging("Tagging", "List", mandatory, false, "Interactive help");
            Map(x => x.SubListSelect).SubListSelect("SubListSelect"
                , typeof(Sushi.Mediakiwi.AppCentre.Data.Implementation.ComponentList)
                , mandatory, false, "Interactive help");
           
            Map(x => x.Sortable).SortList("Sortable", "Interactive help");

            this.Sortable = new Mediakiwi.Data.SubList();
            
            for(var i = 0; i < 10; i ++)
                this.Sortable.Add(i, $"test {i}");

            Map(x => x.MultiField).MultiField("MultiField", "Interactive help");

            Map(x => x.Image).Image("Image", mandatory, null, "Interactive help");
            Map(x => x.Document).Document("Document", mandatory, null, "Interactive help");
            Map(x => x.PageSelect).PageSelect("PageSelect", mandatory, "Interactive help");
            Map(x => x.FolderSelect).FolderSelect("FolderSelect", mandatory, Mediakiwi.Data.FolderType.Administration_Or_List, "Interactive help");

            Map(x => x.ListItemSelect).ListItemSelect("ListItemSelect", "List", mandatory, "Interactive help");

            Map(x => x.Hyperlink).Hyperlink("Hyperlink", mandatory, "Interactive help");
            Map(x => x.HtmlContainer).HtmlContainer(true);
            HtmlContainer = "<b>HtmlContainer</b>";

            SubListSelect = new Mediakiwi.Data.SubList();

            System.Collections.Specialized.NameValueCollection nv = new System.Collections.Specialized.NameValueCollection();
            nv.Add("test", "1");
            SubListSelect.ApplyQueryStringParameter(nv);

            Map(x => x.PageContainer).PageContainer();

            Map(x => x.Upload).FileUpload("FileUpload", mandatory, ".txt", "Interactive help");

            Map(x => x.Button1).Button("Button Primairy", ButtonTarget.BottomLeft).Primary().Hide(isLayerMode);
            
            Map(x => x.Button2).Button("Button Confirmation", ButtonTarget.TopRight)
                .Confirmation("Confirmation", "Can you confirm?", "Yes i accept", "No, i reject");

            Map(x => x.Button3).Button("Button list with custom layersize", ButtonTarget.TopLeft)
                .OpenList(typeof(Demonstration)).Hide(isLayerMode)
                .Title("test 1234")
                .Width(90, true)
                .ScrollBar(true)
                ;
            
            Map(x => x.Button4).Button("External URL tab", ButtonTarget.TopRight)
                .OpenUrl(new Uri("https://www.google.com"), false).Hide();

            Map(x => x.Button4).Button("External URL (layer)", ButtonTarget.BottomRight)
                .OpenUrl(new Uri("https://www.google.com"), true).Hide();

        }
        public Mediakiwi.Data.Page PageContainer { get; set; }
        public string HtmlContainer { get; set; }
        public int Hyperlink { get; set; }
        public int Image { get; set; }
        public int Document { get; set; }
        public FileUpload Upload { get; set; }
        public int PageSelect { get; set; }
        public int FolderSelect { get; set; }
        public string MultiField { get; set; }
        public string ListItemSelect { get; set; }
        public ListItemCollection List { get; set; }
        public bool Checkbox { get; set; }
        public DateTime Date { get; set; }
        public DateTime DateTime { get; set; }
        public string Radio { get; set; }
        public string Dropdown1 { get; set; }
        public string Dropdown2 { get; set; }
        public string Tagging { get; set; }

        public Mediakiwi.Data.SubList SubListSelect { get; set; }
        public Mediakiwi.Data.SubList Sortable { get; set; }

        public string TextField { get; set; }
        public string TextArea { get; set; }
        public string Richtext { get; set; }
        public string Sourcecode { get; set; }
        public string TextLine { get; set; } = "Information assigned to textline";
        public string Section { get; set; }
        public bool Button1 { get; set; }
        public bool Button2 { get; set; }
        public bool Button3 { get; set; }
        public bool Button4 { get; set; }
        public bool Button5 { get; set; }

        public string OuterTextField { get; set; }
    }
}
